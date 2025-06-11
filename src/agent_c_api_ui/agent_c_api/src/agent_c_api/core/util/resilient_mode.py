"""
Redis Streams Resilient Mode Implementation

This module provides the core components for implementing resilient Redis Streams
operation with dynamic mode switching capabilities. It supports three operation modes:
- REDIS_ONLY: Redis Streams only
- HYBRID: Redis Streams with async queue fallback  
- ASYNC_ONLY: Async queue only

The implementation provides transparent switching between modes based on Redis health
and configuration, ensuring reliable event processing under various failure scenarios.
"""

import asyncio
import logging
import time
from dataclasses import dataclass
from enum import Enum, auto
from typing import Dict, List, Optional, Any, Callable, Awaitable
import threading
from abc import ABC, abstractmethod

logger = logging.getLogger(__name__)


class OperationMode(Enum):
    """Primary operation modes for event processing."""
    REDIS_ONLY = "redis_only"
    HYBRID = "hybrid"
    ASYNC_ONLY = "async_only"


class ModeState(Enum):
    """Internal state management for mode operations."""
    STABLE = auto()          # Mode operating normally
    DEGRADED = auto()        # Mode operating with limitations
    TRANSITIONING = auto()   # In process of switching modes
    FAILED = auto()          # Mode operation failed
    RECOVERING = auto()      # Attempting to recover mode


class TransitionReason(Enum):
    """Reasons for mode transitions."""
    MANUAL_REQUEST = auto()       # Administrator requested change
    HEALTH_CHECK_FAILURE = auto() # Health check detected issues
    CONNECTION_ERROR = auto()     # Redis connection failed
    PERFORMANCE_ISSUE = auto()    # Performance degradation detected
    MEMORY_PRESSURE = auto()      # Memory usage too high
    STARTUP_INITIALIZATION = auto() # System startup
    RECOVERY_COMPLETE = auto()    # Recovery operation finished
    CONFIGURATION_CHANGE = auto() # Configuration updated


@dataclass
class ModeTransition:
    """Represents a mode transition event."""
    from_mode: OperationMode
    to_mode: OperationMode
    from_state: ModeState
    to_state: ModeState
    reason: TransitionReason
    timestamp: float
    success: bool
    error_message: Optional[str] = None
    metadata: Optional[Dict[str, Any]] = None


@dataclass
class ResilientModeConfig:
    """Configuration for resilient mode operation."""
    #changed by rohan batra
    operation_mode: OperationMode = OperationMode.HYBRID
    mode_switch_delay_seconds: float = 30.0
    health_check_interval_seconds: float = 10.0
    redis_failure_threshold: int = 3
    circuit_breaker_timeout_seconds: float = 60.0
    enable_auto_recovery: bool = True
    max_transition_retries: int = 3
    performance_monitoring_enabled: bool = True


class CircuitBreaker:
    """Circuit breaker implementation for Redis operations."""
    
    def __init__(self, failure_threshold: int = 5, recovery_timeout: float = 30.0):
        self.failure_threshold = failure_threshold
        self.recovery_timeout = recovery_timeout
        self.failure_count = 0
        self.last_failure_time: Optional[float] = None
        self.state = "closed"  # closed, open, half-open
        self._lock = threading.Lock()
    
    def can_execute(self) -> bool:
        """Check if operations can be executed."""
        with self._lock:
            if self.state == "closed":
                return True
            elif self.state == "open":
                if (time.time() - (self.last_failure_time or 0)) > self.recovery_timeout:
                    self.state = "half-open"
                    return True
                return False
            else:  # half-open
                return True
    
    def record_success(self) -> None:
        """Record a successful operation."""
        with self._lock:
            if self.state == "half-open":
                self.state = "closed"
                self.failure_count = 0
                self.last_failure_time = None
    
    def record_failure(self) -> None:
        """Record a failed operation."""
        with self._lock:
            self.failure_count += 1
            self.last_failure_time = time.time()
            
            if self.failure_count >= self.failure_threshold:
                self.state = "open"
    
    def get_state(self) -> Dict[str, Any]:
        """Get current circuit breaker state."""
        with self._lock:
            return {
                "state": self.state,
                "failure_count": self.failure_count,
                "last_failure_time": self.last_failure_time,
                "can_execute": self.can_execute()
            }


class EventHandlerModeManager:
    """
    Manages operation mode state and transitions for resilient event processing.
    
    This class handles the core mode management functionality including:
    - Current mode and state tracking
    - Mode transition validation and execution
    - Health monitoring integration
    - Circuit breaker management
    - Callback registration for mode change notifications
    """
    
    def __init__(self, config: ResilientModeConfig):
        self.config = config
        self.current_mode = config.operation_mode
        self.current_state = ModeState.STABLE
        self.transition_history: List[ModeTransition] = []
        self.state_change_callbacks: Dict[str, Callable] = {}
        self.transition_lock = asyncio.Lock()
        self.logger = logging.getLogger(__name__)
        
        # State tracking
        self.state_entry_time = time.time()
        self.transition_count = 0
        self.failure_count = 0
        self.last_successful_transition: Optional[ModeTransition] = None
        
        # Circuit breaker for Redis operations
        self.redis_circuit_breaker = CircuitBreaker(
            failure_threshold=config.redis_failure_threshold,
            recovery_timeout=config.circuit_breaker_timeout_seconds
        )
        
        # Health monitoring reference (set externally)
        self.health_monitor: Optional[Any] = None
    
    def set_health_monitor(self, health_monitor: Any) -> None:
        """Set the health monitor instance."""
        self.health_monitor = health_monitor
    
    async def request_mode_transition(
        self, 
        target_mode: OperationMode, 
        reason: TransitionReason,
        force: bool = False,
        metadata: Optional[Dict[str, Any]] = None
    ) -> bool:
        """
        Request a mode transition with validation.
        
        Args:
            target_mode: The desired operation mode
            reason: Reason for the transition
            force: Skip validation and timing checks
            metadata: Additional context for the transition
            
        Returns:
            bool: True if transition was successful
        """
        if not hasattr(self, 'transition_lock'):
            # Handle case where asyncio event loop isn't available during init
            self.transition_lock = asyncio.Lock()
            
        async with self.transition_lock:
            # Validate transition request
            if not force and not self._is_transition_allowed(target_mode, reason):
                self.logger.warning(
                    f"Transition from {self.current_mode} to {target_mode} "
                    f"not allowed (reason: {reason})"
                )
                return False
            
            # Check transition timing
            if not force and not self._check_transition_timing():
                self.logger.warning("Transition rejected due to timing constraints")
                return False
            
            return await self._execute_transition(target_mode, reason, metadata)
    
    def _is_transition_allowed(
        self, 
        target_mode: OperationMode, 
        reason: TransitionReason
    ) -> bool:
        """Check if transition is allowed based on current state."""
        
        # Allow emergency transitions
        if reason in [
            TransitionReason.CONNECTION_ERROR,
            TransitionReason.HEALTH_CHECK_FAILURE,
            TransitionReason.MEMORY_PRESSURE
        ]:
            return True
        
        # Check if already in target mode
        if self.current_mode == target_mode:
            return False
        
        # Check if currently transitioning
        if self.current_state == ModeState.TRANSITIONING:
            return False
        
        # Check mode-specific restrictions
        if self.current_mode == OperationMode.REDIS_ONLY:
            if self.current_state == ModeState.FAILED:
                # Can only transition to modes with fallback
                return target_mode in [OperationMode.HYBRID, OperationMode.ASYNC_ONLY]
        
        return True
    
    def _check_transition_timing(self) -> bool:
        """Check if sufficient time has passed for transition."""
        
        min_state_duration = self.config.mode_switch_delay_seconds
        time_in_state = time.time() - self.state_entry_time
        
        if time_in_state < min_state_duration:
            return False
        
        # Check for rapid transition prevention
        if self.transition_count > 3:
            # Last 3 transitions within 5 minutes indicates instability
            recent_transitions = [
                t for t in self.transition_history[-3:]
                if time.time() - t.timestamp < 300
            ]
            if len(recent_transitions) >= 3:
                self.logger.warning("Preventing rapid mode transitions")
                return False
        
        return True
    
    async def _execute_transition(
        self,
        target_mode: OperationMode,
        reason: TransitionReason,
        metadata: Optional[Dict[str, Any]]
    ) -> bool:
        """Execute the actual mode transition."""
        
        old_mode = self.current_mode
        old_state = self.current_state
        
        try:
            # Start transition
            self.current_state = ModeState.TRANSITIONING
            self.state_entry_time = time.time()
            
            transition = ModeTransition(
                from_mode=old_mode,
                to_mode=target_mode,
                from_state=old_state,
                to_state=ModeState.TRANSITIONING,
                reason=reason,
                timestamp=time.time(),
                success=False,
                metadata=metadata
            )
            
            # Notify transition start
            await self._notify_transition_start(transition)
            
            # Execute mode-specific transition logic
            success = await self._perform_mode_switch(old_mode, target_mode)
            
            if success:
                # Complete transition
                self.current_mode = target_mode
                self.current_state = ModeState.STABLE
                self.state_entry_time = time.time()
                self.transition_count += 1
                self.last_successful_transition = transition
                
                transition.success = True
                transition.to_state = ModeState.STABLE
                
                self.logger.info(
                    f"Mode transition successful: {old_mode.value} → {target_mode.value} "
                    f"(reason: {reason.name})"
                )
                
                # Notify transition success
                await self._notify_transition_complete(transition)
                
            else:
                # Rollback transition
                self.current_state = old_state
                self.state_entry_time = time.time()
                self.failure_count += 1
                
                transition.success = False
                transition.error_message = "Mode switch operation failed"
                
                self.logger.error(
                    f"Mode transition failed: {old_mode.value} → {target_mode.value} "
                    f"(reason: {reason.name})"
                )
                
                # Notify transition failure
                await self._notify_transition_failed(transition)
            
            # Record transition
            self.transition_history.append(transition)
            
            # Limit history size
            if len(self.transition_history) > 100:
                self.transition_history = self.transition_history[-50:]
            
            return success
            
        except Exception as e:
            # Handle unexpected errors during transition
            self.current_state = ModeState.FAILED
            self.state_entry_time = time.time()
            self.failure_count += 1
            
            transition.success = False
            transition.error_message = str(e)
            self.transition_history.append(transition)
            
            self.logger.error(f"Transition error: {e}")
            await self._notify_transition_error(transition, e)
            
            return False
    
    async def _perform_mode_switch(
        self, 
        from_mode: OperationMode, 
        to_mode: OperationMode
    ) -> bool:
        """Perform the actual mode switching logic."""
        
        try:
            # Mode-specific switching logic
            if to_mode == OperationMode.REDIS_ONLY:
                return await self._switch_to_redis_only(from_mode)
            elif to_mode == OperationMode.HYBRID:
                return await self._switch_to_hybrid(from_mode)
            elif to_mode == OperationMode.ASYNC_ONLY:
                return await self._switch_to_async_only(from_mode)
            else:
                return False
                
        except Exception as e:
            self.logger.error(f"Mode switch error: {e}")
            return False
    
    async def _switch_to_redis_only(self, from_mode: OperationMode) -> bool:
        """Switch to Redis-only mode."""
        
        # Verify Redis connectivity through health monitor
        if self.health_monitor and not await self.health_monitor.is_redis_healthy():
            self.logger.error("Cannot switch to REDIS_ONLY mode: Redis is not healthy")
            return False
        
        # Reset circuit breaker for fresh start
        self.redis_circuit_breaker.record_success()
        
        return True
    
    async def _switch_to_hybrid(self, from_mode: OperationMode) -> bool:
        """Switch to hybrid mode."""
        
        # Hybrid mode should always be available as it has fallback
        return True
    
    async def _switch_to_async_only(self, from_mode: OperationMode) -> bool:
        """Switch to async-only mode."""
        
        # Async-only mode should always be available
        return True
    
    async def _notify_transition_start(self, transition: ModeTransition):
        """Notify observers that transition is starting."""
        for callback_name, callback in self.state_change_callbacks.items():
            try:
                if asyncio.iscoroutinefunction(callback):
                    await callback('transition_start', transition)
                else:
                    callback('transition_start', transition)
            except Exception as e:
                self.logger.error(f"Transition callback {callback_name} error: {e}")
    
    async def _notify_transition_complete(self, transition: ModeTransition):
        """Notify observers that transition completed successfully."""
        for callback_name, callback in self.state_change_callbacks.items():
            try:
                if asyncio.iscoroutinefunction(callback):
                    await callback('transition_complete', transition)
                else:
                    callback('transition_complete', transition)
            except Exception as e:
                self.logger.error(f"Transition callback {callback_name} error: {e}")
    
    async def _notify_transition_failed(self, transition: ModeTransition):
        """Notify observers that transition failed."""
        for callback_name, callback in self.state_change_callbacks.items():
            try:
                if asyncio.iscoroutinefunction(callback):
                    await callback('transition_failed', transition)
                else:
                    callback('transition_failed', transition)
            except Exception as e:
                self.logger.error(f"Transition callback {callback_name} error: {e}")
    
    async def _notify_transition_error(self, transition: ModeTransition, error: Exception):
        """Notify observers of transition error."""
        for callback_name, callback in self.state_change_callbacks.items():
            try:
                if asyncio.iscoroutinefunction(callback):
                    await callback('transition_error', transition, error)
                else:
                    callback('transition_error', transition, error)
            except Exception as e:
                self.logger.error(f"Transition callback {callback_name} error: {e}")
    
    def get_current_status(self) -> Dict[str, Any]:
        """Get current mode and state status."""
        return {
            'mode': self.current_mode.value,
            'state': self.current_state.name,
            'state_duration': time.time() - self.state_entry_time,
            'transition_count': self.transition_count,
            'failure_count': self.failure_count,
            'last_transition': (
                self.last_successful_transition.timestamp 
                if self.last_successful_transition else None
            ),
            'circuit_breaker': self.redis_circuit_breaker.get_state()
        }
    
    def register_state_callback(self, name: str, callback: Callable) -> None:
        """Register callback for state change notifications."""
        self.state_change_callbacks[name] = callback
    
    def unregister_state_callback(self, name: str) -> None:
        """Unregister state change callback."""
        self.state_change_callbacks.pop(name, None)
    
    def can_use_redis(self) -> bool:
        """Check if Redis can be used based on current mode and circuit breaker state."""
        if self.current_mode == OperationMode.ASYNC_ONLY:
            return False
        
        if self.current_state in [ModeState.FAILED, ModeState.TRANSITIONING]:
            return False
        
        return self.redis_circuit_breaker.can_execute()
    
    def record_redis_success(self) -> None:
        """Record successful Redis operation."""
        self.redis_circuit_breaker.record_success()
    
    def record_redis_failure(self) -> None:
        """Record failed Redis operation."""
        self.redis_circuit_breaker.record_failure()
        
        # Check if we should trigger automatic mode transition
        if (self.current_mode == OperationMode.REDIS_ONLY and 
            self.redis_circuit_breaker.state == "open" and
            self.config.enable_auto_recovery):
            
            # Schedule async transition to hybrid mode
            asyncio.create_task(self.request_mode_transition(
                target_mode=OperationMode.HYBRID,
                reason=TransitionReason.CONNECTION_ERROR,
                force=True
            ))