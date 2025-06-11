"""
EventHandlerModeManager for Agent C

Dynamic mode switching between Redis and async queue event handlers based on 
Redis health status. Provides strategy pattern for event handler selection,
transparent event routing, and notification callbacks for mode changes.

This module implements:
- Mode state tracking (REDIS, ASYNC, TRANSITIONING)
- Mode transition logic with safeguards and event buffering
- Strategy pattern for handler selection with factory methods
- Transparent event routing with consistent ordering guarantees
- Integration helpers for component registration and notifications
- Comprehensive logging and metrics for monitoring
"""

import asyncio
import logging
import threading
import time
from abc import ABC, abstractmethod
from collections import defaultdict, deque
from contextlib import contextmanager
from dataclasses import dataclass, field
from enum import Enum
from typing import Any, AsyncGenerator, Callable, Dict, List, Optional, Protocol, Set, Tuple, Union
from uuid import uuid4
from datetime import datetime

from .redis_health_monitor import RedisHealthMonitor, HealthStatus, HealthEvent
from .redis_stream_manager import RedisStreamManager, RedisConfig, OperationMode
from .event_serializer import EventContext, EventSerializer


logger = logging.getLogger(__name__)


class HandlerMode(Enum):
    """Event handler operation modes."""
    REDIS = "redis"           # Redis Streams active
    ASYNC = "async"           # Async queue active  
    TRANSITIONING = "transitioning"  # Mode switching in progress


class TransitionPhase(Enum):
    """Phases during mode transition."""
    PREPARING = "preparing"           # Preparing for transition
    DRAINING = "draining"             # Draining current handler
    BUFFERING = "buffering"           # Buffering events during switch
    SWITCHING = "switching"           # Activating new handler
    COMPLETING = "completing"         # Finalizing transition


@dataclass
class ModeChangeEvent:
    """Event representing a mode change."""
    from_mode: HandlerMode
    to_mode: HandlerMode
    phase: TransitionPhase
    reason: str
    timestamp: float = field(default_factory=time.time)
    event_id: str = field(default_factory=lambda: str(uuid4()))
    metadata: Dict[str, Any] = field(default_factory=dict)


@dataclass 
class HandlerMetrics:
    """Metrics for event handler performance."""
    events_processed: int = 0
    events_failed: int = 0
    average_latency_ms: float = 0.0
    last_activity: float = field(default_factory=time.time)
    error_count: int = 0
    last_error: Optional[str] = None
    
    def update_latency(self, latency_ms: float):
        """Update average latency with new sample."""
        if self.events_processed == 0:
            self.average_latency_ms = latency_ms
        else:
            # Exponentially weighted moving average
            weight = 0.1
            self.average_latency_ms = (1 - weight) * self.average_latency_ms + weight * latency_ms
    
    def record_success(self, latency_ms: float = 0.0):
        """Record successful event processing."""
        self.events_processed += 1
        self.last_activity = time.time()
        if latency_ms > 0:
            self.update_latency(latency_ms)
    
    def record_failure(self, error: str):
        """Record failed event processing."""
        self.events_failed += 1
        self.error_count += 1
        self.last_error = error
        self.last_activity = time.time()


@dataclass
class BufferedEvent:
    """Event stored in transition buffer."""
    event_type: str
    event_data: Dict[str, Any]
    context: EventContext
    timestamp: float = field(default_factory=time.time)
    retry_count: int = 0
    event_id: str = field(default_factory=lambda: str(uuid4()))


class EventHandler(Protocol):
    """Protocol for event handler implementations."""
    
    async def publish_event(self, 
                          event_type: str,
                          event_data: Dict[str, Any], 
                          context: EventContext) -> Optional[str]:
        """Publish an event."""
        ...
    
    async def consume_events(self, 
                           context: EventContext,
                           last_id: str = '0-0',
                           block: Optional[int] = None) -> AsyncGenerator[Dict[str, Any], None]:
        """Consume events."""
        ...
    
    async def initialize(self) -> bool:
        """Initialize the handler."""
        ...
    
    async def shutdown(self) -> None:
        """Shutdown the handler."""
        ...
    
    def is_healthy(self) -> bool:
        """Check handler health."""
        ...


class RedisEventHandler:
    """Redis Streams event handler implementation."""
    
    def __init__(self, redis_manager: RedisStreamManager):
        self.redis_manager = redis_manager
        self.metrics = HandlerMetrics()
        self._initialized = False
    
    async def publish_event(self, 
                          event_type: str,
                          event_data: Dict[str, Any], 
                          context: EventContext) -> Optional[str]:
        """Publish event to Redis Streams."""
        start_time = time.time()
        try:
            # Convert context to required parameters for redis_manager
            result = self.redis_manager.publish_event(
                event_type=event_type,
                event_data=event_data,
                session_id=context.session_id,
                interaction_id=context.interaction_id,
                source=context.source,
                user_id=context.user_id
            )
            
            latency_ms = (time.time() - start_time) * 1000
            self.metrics.record_success(latency_ms)
            return result
            
        except Exception as e:
            self.metrics.record_failure(str(e))
            logger.error(f"Redis publish failed: {e}")
            raise
    
    async def consume_events(self, 
                           context: EventContext,
                           last_id: str = '0-0',
                           block: Optional[int] = None) -> AsyncGenerator[Dict[str, Any], None]:
        """Consume events from Redis Streams."""
        try:
            async for event in self.redis_manager.consume_events(
                session_id=context.session_id,
                interaction_id=context.interaction_id,
                last_id=last_id,
                block=block
            ):
                self.metrics.record_success()
                yield event
        except Exception as e:
            self.metrics.record_failure(str(e))
            logger.error(f"Redis consume failed: {e}")
            raise
    
    async def initialize(self) -> bool:
        """Initialize Redis handler."""
        try:
            self.redis_manager.initialize()
            self._initialized = True
            return True
        except Exception as e:
            logger.error(f"Redis handler initialization failed: {e}")
            return False
    
    async def shutdown(self) -> None:
        """Shutdown Redis handler."""
        try:
            self.redis_manager.close()
            self._initialized = False
        except Exception as e:
            logger.error(f"Redis handler shutdown failed: {e}")
    
    def is_healthy(self) -> bool:
        """Check Redis handler health."""
        return self._initialized and self.redis_manager.is_healthy()


class AsyncEventHandler:
    """Async queue event handler implementation."""
    
    def __init__(self, max_queue_size: int = 10000):
        self.event_queues: Dict[str, asyncio.Queue] = {}
        self.max_queue_size = max_queue_size
        self.metrics = HandlerMetrics()
        self._initialized = False
        self._lock = asyncio.Lock()
    
    async def publish_event(self, 
                          event_type: str,
                          event_data: Dict[str, Any], 
                          context: EventContext) -> Optional[str]:
        """Publish event to async queue."""
        start_time = time.time()
        try:
            queue_key = f"{context.session_id}:{context.interaction_id}"
            
            async with self._lock:
                if queue_key not in self.event_queues:
                    self.event_queues[queue_key] = asyncio.Queue(maxsize=self.max_queue_size)
            
            event = {
                'event_type': event_type,
                'event_data': event_data,
                'context': context.__dict__,
                'timestamp': time.time(),
                'event_id': str(uuid4())
            }
            
            await self.event_queues[queue_key].put(event)
            
            latency_ms = (time.time() - start_time) * 1000
            self.metrics.record_success(latency_ms)
            return event['event_id']
            
        except Exception as e:
            self.metrics.record_failure(str(e))
            logger.error(f"Async publish failed: {e}")
            raise
    
    async def consume_events(self, 
                           context: EventContext,
                           last_id: str = '0-0',
                           block: Optional[int] = None) -> AsyncGenerator[Dict[str, Any], None]:
        """Consume events from async queue."""
        queue_key = f"{context.session_id}:{context.interaction_id}"
        
        async with self._lock:
            if queue_key not in self.event_queues:
                self.event_queues[queue_key] = asyncio.Queue(maxsize=self.max_queue_size)
        
        queue = self.event_queues[queue_key]
        
        try:
            while True:
                if block is not None:
                    event = await asyncio.wait_for(queue.get(), timeout=block/1000.0)
                else:
                    try:
                        event = queue.get_nowait()
                    except asyncio.QueueEmpty:
                        return
                
                self.metrics.record_success()
                yield event
                
        except asyncio.TimeoutError:
            # Timeout is expected for blocking calls
            return
        except Exception as e:
            self.metrics.record_failure(str(e))
            logger.error(f"Async consume failed: {e}")
            raise
    
    async def initialize(self) -> bool:
        """Initialize async handler."""
        self._initialized = True
        return True
    
    async def shutdown(self) -> None:
        """Shutdown async handler."""
        async with self._lock:
            for queue in self.event_queues.values():
                while not queue.empty():
                    try:
                        queue.get_nowait()
                    except asyncio.QueueEmpty:
                        break
            self.event_queues.clear()
        self._initialized = False
    
    def is_healthy(self) -> bool:
        """Check async handler health."""
        return self._initialized


class EventHandlerModeManager:
    """
    Manages dynamic switching between Redis and async queue event handlers
    based on Redis health status and configuration.
    
    Features:
    - Automatic mode switching based on Redis health
    - Event buffering during transitions for reliability
    - Strategy pattern for transparent handler selection
    - Comprehensive metrics and monitoring
    - Manual override capabilities
    - Thread-safe operations
    """
    
    def __init__(self, 
                 redis_config: Optional[RedisConfig] = None,
                 health_monitor: Optional[RedisHealthMonitor] = None,
                 transition_timeout: float = 30.0,
                 buffer_max_size: int = 1000,
                 enable_metrics: bool = True):
        """
        Initialize EventHandlerModeManager.
        
        Args:
            redis_config: Redis configuration
            health_monitor: Redis health monitor
            transition_timeout: Max time for mode transitions in seconds
            buffer_max_size: Max events to buffer during transitions
            enable_metrics: Whether to collect detailed metrics
        """
        self.redis_config = redis_config or RedisConfig()
        self.health_monitor = health_monitor or RedisHealthMonitor(self.redis_config)
        self.transition_timeout = transition_timeout
        self.buffer_max_size = buffer_max_size
        self.enable_metrics = enable_metrics
        
        # Mode management
        self._current_mode = HandlerMode.ASYNC  # Start with async fallback
        self._target_mode = HandlerMode.ASYNC
        self._transition_phase = TransitionPhase.COMPLETING
        self._mode_lock = threading.RLock()
        self._transition_event = threading.Event()
        
        # Event handlers
        self._redis_handler: Optional[RedisEventHandler] = None
        self._async_handler: Optional[AsyncEventHandler] = None
        self._active_handler: Optional[EventHandler] = None
        
        # Event buffering during transitions
        self._event_buffer: deque = deque(maxlen=buffer_max_size)
        self._buffer_lock = asyncio.Lock()
        
        # Callbacks and monitoring
        self._mode_change_callbacks: Dict[str, Callable[[ModeChangeEvent], None]] = {}
        self._component_registry: Dict[str, Any] = {}
        
        # Metrics and diagnostics
        self._transition_history: List[ModeChangeEvent] = []
        self._handler_metrics: Dict[HandlerMode, HandlerMetrics] = {
            HandlerMode.REDIS: HandlerMetrics(),
            HandlerMode.ASYNC: HandlerMetrics()
        }
        
        # Manual override
        self._manual_override: Optional[HandlerMode] = None
        self._override_reason: Optional[str] = None
        
        # Shutdown flag
        self._shutdown = False
        
        logger.info(f"EventHandlerModeManager initialized with config: {self.redis_config.operation_mode}")
    
    async def initialize(self) -> bool:
        """
        Initialize the mode manager and handlers.
        
        Returns:
            True if initialization successful
        """
        try:
            # Initialize handlers
            redis_manager = RedisStreamManager(self.redis_config)
            self._redis_handler = RedisEventHandler(redis_manager)
            self._async_handler = AsyncEventHandler()
            
            # Always initialize async handler
            await self._async_handler.initialize()
            
            # Try to initialize Redis handler
            redis_initialized = await self._redis_handler.initialize()
            
            # Register for health status changes
            self.health_monitor.register_status_change_callback(self._on_health_status_change)
            
            # Start health monitoring
            self.health_monitor.start_monitoring()
            
            # Determine initial mode based on config and Redis health
            initial_mode = self._determine_initial_mode(redis_initialized)
            await self._transition_to_mode(initial_mode, "initialization")
            
            logger.info(f"EventHandlerModeManager initialized in {self._current_mode.value} mode")
            return True
            
        except Exception as e:
            logger.error(f"Failed to initialize EventHandlerModeManager: {e}")
            return False
    
    async def shutdown(self) -> None:
        """Shutdown the mode manager and cleanup resources."""
        self._shutdown = True
        
        with self._mode_lock:
            self._transition_event.set()
        
        # Stop health monitoring
        self.health_monitor.stop_monitoring()
        
        # Shutdown handlers
        if self._redis_handler:
            await self._redis_handler.shutdown()
        if self._async_handler:
            await self._async_handler.shutdown()
        
        logger.info("EventHandlerModeManager shutdown complete")
    
    # ========== Event Routing Methods ==========
    
    async def publish_event(self, 
                          event_type: str,
                          event_data: Dict[str, Any], 
                          context: EventContext) -> Optional[str]:
        """
        Publish event through active handler with fallback support.
        
        Args:
            event_type: Type of event
            event_data: Event data
            context: Event context
            
        Returns:
            Event ID if successful, None otherwise
        """
        if self._shutdown:
            raise RuntimeError("EventHandlerModeManager is shutdown")
        
        # Check if we're in transition and need to buffer
        with self._mode_lock:
            if self._current_mode == HandlerMode.TRANSITIONING:
                return await self._buffer_event(event_type, event_data, context)
        
        handler = self._get_active_handler()
        if not handler:
            raise RuntimeError("No active event handler available")
        
        try:
            start_time = time.time()
            result = await handler.publish_event(event_type, event_data, context)
            
            if self.enable_metrics:
                latency_ms = (time.time() - start_time) * 1000
                self._handler_metrics[self._current_mode].record_success(latency_ms)
            
            return result
            
        except Exception as e:
            if self.enable_metrics:
                self._handler_metrics[self._current_mode].record_failure(str(e))
            
            # Try fallback if Redis fails and fallback is enabled
            if (self._current_mode == HandlerMode.REDIS and 
                self.redis_config.is_failover_enabled()):
                logger.warning(f"Redis publish failed, trying fallback: {e}")
                try:
                    return await self._async_handler.publish_event(event_type, event_data, context)
                except Exception as fallback_error:
                    logger.error(f"Fallback publish also failed: {fallback_error}")
                    raise
            
            raise
    
    async def consume_events(self, 
                           context: EventContext,
                           last_id: str = '0-0',
                           block: Optional[int] = None) -> AsyncGenerator[Dict[str, Any], None]:
        """
        Consume events through active handler.
        
        Args:
            context: Event context
            last_id: Last message ID received
            block: Block time in milliseconds
            
        Yields:
            Event data dictionaries
        """
        if self._shutdown:
            raise RuntimeError("EventHandlerModeManager is shutdown")
        
        handler = self._get_active_handler()
        if not handler:
            raise RuntimeError("No active event handler available")
        
        try:
            async for event in handler.consume_events(context, last_id, block):
                if self.enable_metrics:
                    self._handler_metrics[self._current_mode].record_success()
                yield event
                
        except Exception as e:
            if self.enable_metrics:
                self._handler_metrics[self._current_mode].record_failure(str(e))
            
            # Try fallback if Redis fails and fallback is enabled
            if (self._current_mode == HandlerMode.REDIS and 
                self.redis_config.is_failover_enabled()):
                logger.warning(f"Redis consume failed, trying fallback: {e}")
                try:
                    async for event in self._async_handler.consume_events(context, last_id, block):
                        yield event
                except Exception as fallback_error:
                    logger.error(f"Fallback consume also failed: {fallback_error}")
                    raise
            else:
                raise
    
    # ========== Mode Management Methods ==========
    
    def get_current_mode(self) -> HandlerMode:
        """Get current handler mode."""
        with self._mode_lock:
            return self._current_mode
    
    def get_transition_phase(self) -> TransitionPhase:
        """Get current transition phase."""
        with self._mode_lock:
            return self._transition_phase
    
    def is_transitioning(self) -> bool:
        """Check if currently transitioning between modes."""
        with self._mode_lock:
            return self._current_mode == HandlerMode.TRANSITIONING
    
    async def set_manual_override(self, mode: HandlerMode, reason: str) -> bool:
        """
        Manually override the handler mode.
        
        Args:
            mode: Target mode to override to
            reason: Reason for manual override
            
        Returns:
            True if override successful
        """
        logger.info(f"Manual override requested: {mode.value} - {reason}")
        
        with self._mode_lock:
            self._manual_override = mode
            self._override_reason = reason
        
        return await self._transition_to_mode(mode, f"manual_override: {reason}")
    
    def clear_manual_override(self) -> None:
        """Clear manual mode override."""
        logger.info("Clearing manual override")
        
        with self._mode_lock:
            self._manual_override = None
            self._override_reason = None
        
        # Trigger re-evaluation of mode based on current health
        self._on_health_status_change(HealthEvent(
            old_status=self.health_monitor.status,
            new_status=self.health_monitor.status,
            timestamp=datetime.utcnow(),
            circuit_state=self.health_monitor.circuit_breaker.state,
            metrics=self.health_monitor.metrics,
            reason="manual_override_cleared",
            details={}
        ))
    
    # ========== Handler Selection and Factory Methods ==========
    
    def _get_active_handler(self) -> Optional[EventHandler]:
        """Get the currently active event handler."""
        with self._mode_lock:
            if self._current_mode == HandlerMode.REDIS:
                return self._redis_handler
            elif self._current_mode == HandlerMode.ASYNC:
                return self._async_handler
            else:
                # During transition, return the fallback
                return self._async_handler
    
    def _determine_initial_mode(self, redis_initialized: bool) -> HandlerMode:
        """Determine initial handler mode based on config and Redis health."""
        if self._manual_override:
            return self._manual_override
        
        if not redis_initialized:
            return HandlerMode.ASYNC
        
        # Check Redis health
        if not self.health_monitor.is_healthy():
            return HandlerMode.ASYNC
        
        # Use config to determine mode
        effective_mode = self.redis_config.get_effective_mode(True)
        
        if effective_mode == OperationMode.REDIS_ONLY:
            return HandlerMode.REDIS
        elif effective_mode == OperationMode.ASYNC_ONLY:
            return HandlerMode.ASYNC
        else:  # HYBRID
            return HandlerMode.REDIS  # Prefer Redis for hybrid mode when healthy
    
    def _should_use_redis_mode(self) -> bool:
        """Determine if Redis mode should be used based on current conditions."""
        if self._manual_override:
            return self._manual_override == HandlerMode.REDIS
        
        if not self.health_monitor.is_healthy():
            return False
        
        effective_mode = self.redis_config.get_effective_mode(True)
        return effective_mode in [OperationMode.REDIS_ONLY, OperationMode.HYBRID]
    
    # ========== Event Buffering Methods ==========
    
    async def _buffer_event(self, 
                          event_type: str, 
                          event_data: Dict[str, Any], 
                          context: EventContext) -> str:
        """Buffer event during mode transition."""
        buffered_event = BufferedEvent(
            event_type=event_type,
            event_data=event_data,
            context=context
        )
        
        async with self._buffer_lock:
            if len(self._event_buffer) >= self.buffer_max_size:
                # Remove oldest event if buffer is full
                dropped = self._event_buffer.popleft()
                logger.warning(f"Dropped buffered event due to full buffer: {dropped.event_id}")
            
            self._event_buffer.append(buffered_event)
        
        logger.debug(f"Buffered event during transition: {buffered_event.event_id}")
        return buffered_event.event_id
    
    async def _flush_buffered_events(self) -> None:
        """Flush buffered events through the active handler."""
        if not self._event_buffer:
            return
        
        handler = self._get_active_handler()
        if not handler:
            logger.error("No active handler to flush buffered events")
            return
        
        events_to_flush = []
        async with self._buffer_lock:
            events_to_flush = list(self._event_buffer)
            self._event_buffer.clear()
        
        logger.info(f"Flushing {len(events_to_flush)} buffered events")
        
        successful = 0
        failed = 0
        
        for event in events_to_flush:
            try:
                await handler.publish_event(
                    event.event_type,
                    event.event_data,
                    event.context
                )
                successful += 1
            except Exception as e:
                failed += 1
                logger.error(f"Failed to flush buffered event {event.event_id}: {e}")
        
        logger.info(f"Buffered event flush complete: {successful} successful, {failed} failed")
    
    # ========== Mode Transition Methods ==========
    
    async def _transition_to_mode(self, target_mode: HandlerMode, reason: str) -> bool:
        """
        Transition to target mode with proper safeguards and buffering.
        
        Args:
            target_mode: Mode to transition to
            reason: Reason for transition
            
        Returns:
            True if transition successful
        """
        with self._mode_lock:
            if self._current_mode == target_mode:
                logger.debug(f"Already in target mode {target_mode.value}")
                return True
            
            if self._current_mode == HandlerMode.TRANSITIONING:
                logger.warning(f"Transition already in progress, waiting for completion")
                # Wait for current transition to complete
                self._transition_event.wait(timeout=self.transition_timeout)
                return self._current_mode == target_mode
            
            # Start transition
            old_mode = self._current_mode
            self._current_mode = HandlerMode.TRANSITIONING
            self._target_mode = target_mode
            self._transition_event.clear()
        
        logger.info(f"Starting mode transition: {old_mode.value} -> {target_mode.value} ({reason})")
        
        try:
            # Phase 1: Preparing
            await self._notify_mode_change(ModeChangeEvent(
                from_mode=old_mode,
                to_mode=target_mode,
                phase=TransitionPhase.PREPARING,
                reason=reason
            ))
            
            # Phase 2: Draining current handler
            await self._notify_mode_change(ModeChangeEvent(
                from_mode=old_mode,
                to_mode=target_mode,
                phase=TransitionPhase.DRAINING,
                reason=reason
            ))
            
            # Allow current operations to complete
            await asyncio.sleep(0.1)
            
            # Phase 3: Buffering mode
            await self._notify_mode_change(ModeChangeEvent(
                from_mode=old_mode,
                to_mode=target_mode,
                phase=TransitionPhase.BUFFERING,
                reason=reason
            ))
            
            # Phase 4: Switching to new handler
            await self._notify_mode_change(ModeChangeEvent(
                from_mode=old_mode,
                to_mode=target_mode,
                phase=TransitionPhase.SWITCHING,
                reason=reason
            ))
            
            # Ensure target handler is ready
            target_handler = self._redis_handler if target_mode == HandlerMode.REDIS else self._async_handler
            if not target_handler or not target_handler.is_healthy():
                if target_mode == HandlerMode.REDIS:
                    # Try to reinitialize Redis handler
                    if self._redis_handler:
                        await self._redis_handler.initialize()
                
                if not target_handler or not target_handler.is_healthy():
                    raise RuntimeError(f"Target handler {target_mode.value} is not healthy")
            
            # Phase 5: Completing transition
            with self._mode_lock:
                self._current_mode = target_mode
                self._transition_phase = TransitionPhase.COMPLETING
            
            await self._notify_mode_change(ModeChangeEvent(
                from_mode=old_mode,
                to_mode=target_mode,
                phase=TransitionPhase.COMPLETING,
                reason=reason
            ))
            
            # Flush any buffered events
            await self._flush_buffered_events()
            
            with self._mode_lock:
                self._transition_event.set()
            
            logger.info(f"Mode transition completed: {old_mode.value} -> {target_mode.value}")
            return True
            
        except Exception as e:
            logger.error(f"Mode transition failed: {e}")
            
            # Rollback to previous mode
            with self._mode_lock:
                self._current_mode = old_mode
                self._transition_event.set()
            
            return False
    
    def _on_health_status_change(self, event: HealthEvent) -> None:
        """Handle Redis health status changes."""
        if self._shutdown:
            return
        
        logger.info(f"Redis health status changed: {event.old_status.value} -> {event.new_status.value}")
        
        # Don't override manual mode selection
        if self._manual_override:
            logger.debug("Manual override active, ignoring health status change")
            return
        
        # Determine appropriate mode based on new health status
        target_mode = HandlerMode.REDIS if self._should_use_redis_mode() else HandlerMode.ASYNC
        
        if target_mode != self._current_mode and not self.is_transitioning():
            # Trigger async transition
            asyncio.create_task(self._transition_to_mode(
                target_mode, 
                f"health_status_change: {event.new_status.value}"
            ))
    
    # ========== Integration and Notification Methods ==========
    
    def register_component(self, name: str, component: Any) -> None:
        """Register a component for mode change notifications."""
        self._component_registry[name] = component
        logger.debug(f"Registered component: {name}")
    
    def unregister_component(self, name: str) -> None:
        """Unregister a component."""
        if name in self._component_registry:
            del self._component_registry[name]
            logger.debug(f"Unregistered component: {name}")
    
    def register_mode_change_callback(self, 
                                    callback: Callable[[ModeChangeEvent], None],
                                    callback_id: Optional[str] = None) -> str:
        """
        Register callback for mode change events.
        
        Args:
            callback: Function to call on mode changes
            callback_id: Optional ID for callback
            
        Returns:
            Callback ID for later removal
        """
        if callback_id is None:
            callback_id = str(uuid4())
        
        self._mode_change_callbacks[callback_id] = callback
        logger.debug(f"Registered mode change callback: {callback_id}")
        return callback_id
    
    def unregister_mode_change_callback(self, callback_id: str) -> None:
        """Remove a mode change callback."""
        if callback_id in self._mode_change_callbacks:
            del self._mode_change_callbacks[callback_id]
            logger.debug(f"Unregistered mode change callback: {callback_id}")
    
    async def _notify_mode_change(self, event: ModeChangeEvent) -> None:
        """Notify registered callbacks of mode change."""
        self._transition_history.append(event)
        
        # Keep transition history bounded
        if len(self._transition_history) > 100:
            self._transition_history = self._transition_history[-50:]
        
        # Notify callbacks
        for callback_id, callback in self._mode_change_callbacks.items():
            try:
                callback(event)
            except Exception as e:
                logger.error(f"Mode change callback {callback_id} failed: {e}")
    
    # ========== Diagnostics and State Inspection Methods ==========
    
    def get_status(self) -> Dict[str, Any]:
        """Get comprehensive status information."""
        with self._mode_lock:
            return {
                'current_mode': self._current_mode.value,
                'target_mode': self._target_mode.value,
                'transition_phase': self._transition_phase.value,
                'is_transitioning': self._current_mode == HandlerMode.TRANSITIONING,
                'manual_override': self._manual_override.value if self._manual_override else None,
                'override_reason': self._override_reason,
                'redis_healthy': self.health_monitor.is_healthy(),
                'redis_circuit_open': self.health_monitor.is_circuit_open(),
                'buffered_events': len(self._event_buffer),
                'registered_components': list(self._component_registry.keys()),
                'active_callbacks': len(self._mode_change_callbacks),
                'transition_history_count': len(self._transition_history)
            }
    
    def get_metrics(self) -> Dict[str, Any]:
        """Get performance metrics for all handlers."""
        if not self.enable_metrics:
            return {'metrics_disabled': True}
        
        return {
            'redis_handler': {
                'events_processed': self._handler_metrics[HandlerMode.REDIS].events_processed,
                'events_failed': self._handler_metrics[HandlerMode.REDIS].events_failed,
                'average_latency_ms': self._handler_metrics[HandlerMode.REDIS].average_latency_ms,
                'error_count': self._handler_metrics[HandlerMode.REDIS].error_count,
                'last_error': self._handler_metrics[HandlerMode.REDIS].last_error,
                'last_activity': self._handler_metrics[HandlerMode.REDIS].last_activity
            },
            'async_handler': {
                'events_processed': self._handler_metrics[HandlerMode.ASYNC].events_processed,
                'events_failed': self._handler_metrics[HandlerMode.ASYNC].events_failed,
                'average_latency_ms': self._handler_metrics[HandlerMode.ASYNC].average_latency_ms,
                'error_count': self._handler_metrics[HandlerMode.ASYNC].error_count,
                'last_error': self._handler_metrics[HandlerMode.ASYNC].last_error,
                'last_activity': self._handler_metrics[HandlerMode.ASYNC].last_activity
            }
        }
    
    def get_transition_history(self, limit: int = 10) -> List[Dict[str, Any]]:
        """Get recent mode transition history."""
        return [
            {
                'from_mode': event.from_mode.value,
                'to_mode': event.to_mode.value,
                'phase': event.phase.value,
                'reason': event.reason,
                'timestamp': event.timestamp,
                'event_id': event.event_id,
                'metadata': event.metadata
            }
            for event in self._transition_history[-limit:]
        ]
    
    def get_health_summary(self) -> Dict[str, Any]:
        """Get health summary from Redis health monitor."""
        return self.health_monitor.get_health_summary()
    
    def force_health_check(self) -> bool:
        """Force immediate Redis health check."""
        return self.health_monitor.force_health_check()
    
    @contextmanager
    def diagnostic_mode(self):
        """Context manager for enhanced diagnostic logging."""
        original_level = logger.level
        logger.setLevel(logging.DEBUG)
        logger.debug("Entering diagnostic mode")
        
        try:
            yield
        finally:
            logger.debug("Exiting diagnostic mode")
            logger.setLevel(original_level)
    
    # ========== Context Manager Support ==========
    
    async def __aenter__(self):
        """Async context manager entry."""
        await self.initialize()
        return self
    
    async def __aexit__(self, exc_type, exc_val, exc_tb):
        """Async context manager exit."""
        await self.shutdown()