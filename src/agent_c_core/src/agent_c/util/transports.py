"""
Transport Interface Abstraction for EventSessionLogger

This module provides the abstract TransportInterface and various concrete implementations
for different transport layers including queues, message buses, HTTP endpoints, and more.
"""

import asyncio
import json
import logging
import time
from abc import ABC, abstractmethod
from typing import Any, Optional, Dict, List, Callable, Union
from dataclasses import dataclass
from enum import Enum

from .logging_utils import LoggingManager
from .transport_exceptions import TransportError, TransportConnectionError, TransportTimeoutError


class TransportState(Enum):
    """Transport connection states"""
    DISCONNECTED = "disconnected"
    CONNECTING = "connecting"
    CONNECTED = "connected"
    RECONNECTING = "reconnecting"
    FAILED = "failed"
    CLOSED = "closed"


@dataclass
class TransportMetrics:
    """Transport performance and health metrics"""
    total_sent: int = 0
    total_failed: int = 0
    total_retries: int = 0
    last_success_time: Optional[float] = None
    last_failure_time: Optional[float] = None
    connection_count: int = 0
    average_send_time: float = 0.0
    
    @property
    def success_rate(self) -> float:
        """Calculate success rate as percentage"""
        total = self.total_sent + self.total_failed
        if total == 0:
            return 100.0
        return (self.total_sent / total) * 100.0
    
    @property
    def is_healthy(self) -> bool:
        """Determine if transport is healthy based on metrics"""
        return self.success_rate >= 90.0 and self.total_failed < 10


class TransportInterface(ABC):
    """
    Enhanced abstract interface for transport layers in the EventSessionLogger gateway pattern.
    
    Provides comprehensive transport abstraction with:
    - Async send operations with metadata support
    - Connection management and health monitoring
    - Retry mechanisms and error handling
    - Resource cleanup and lifecycle management
    - Performance metrics and monitoring
    """
    
    def __init__(self, name: str = None):
        self.name = name or self.__class__.__name__
        self.state = TransportState.DISCONNECTED
        self.metrics = TransportMetrics()
        logging_manager = LoggingManager(f"{__name__}.{self.name}")
        self._logger = logging_manager.get_logger()

    
    @abstractmethod
    async def send(self, event: Any, metadata: Optional[Dict[str, Any]] = None) -> bool:
        """
        Send an event through the transport layer.
        
        Args:
            event: Serialized event data
            metadata: Optional routing/transport metadata
            
        Returns:
            bool: True if send succeeded, False otherwise
            
        Raises:
            TransportError: For transport-specific failures
        """
        pass
    
    @abstractmethod
    async def connect(self) -> bool:
        """
        Establish connection to the transport layer.
        
        Returns:
            bool: True if connection succeeded
            
        Raises:
            TransportConnectionError: If connection fails
        """
        pass
    
    @abstractmethod
    async def disconnect(self) -> None:
        """Disconnect from the transport layer"""
        pass
    
    @abstractmethod
    async def close(self) -> None:
        """Clean shutdown of transport connections"""
        pass
    
    async def health_check(self) -> bool:
        """
        Check if transport is healthy and ready.
        
        Returns:
            bool: True if transport is healthy
        """
        return self.is_connected and self.metrics.is_healthy
    
    @property
    def is_connected(self) -> bool:
        """Check if transport is currently connected"""
        return self.state == TransportState.CONNECTED
    
    async def __aenter__(self):
        """Async context manager entry"""
        await self.connect()
        return self
    
    async def __aexit__(self, exc_type, exc_val, exc_tb):
        """Async context manager exit"""
        await self.close()


class CallbackTransport(TransportInterface):
    """
    Transport wrapper for callback functions (transition helper).
    
    Wraps existing callback functions to provide transport interface compatibility,
    enabling smooth migration from callback-based to transport-based patterns.
    """
    
    def __init__(self, callback: Callable, name: str = "CallbackTransport"):
        super().__init__(name)
        self.callback = callback
        self.state = TransportState.CONNECTED  # Always "connected"
    
    async def send(self, event: Any, metadata: Optional[Dict[str, Any]] = None) -> bool:
        """Send event through callback"""
        start_time = time.time()
        
        try:
            if asyncio.iscoroutinefunction(self.callback):
                await self.callback(event)
            else:
                self.callback(event)
            
            # Update metrics
            self.metrics.total_sent += 1
            self.metrics.last_success_time = time.time()
            send_time = time.time() - start_time
            self.metrics.average_send_time = (
                (self.metrics.average_send_time * (self.metrics.total_sent - 1) + send_time) 
                / self.metrics.total_sent
            )
            
            return True
            
        except Exception as e:
            self.metrics.total_failed += 1
            self.metrics.last_failure_time = time.time()
            self._logger.error(f"Callback transport failed: {e}")
            return False
    
    async def connect(self) -> bool:
        """Callback transport is always connected"""
        self.state = TransportState.CONNECTED
        self.metrics.connection_count += 1
        return True
    
    async def disconnect(self) -> None:
        """Disconnect callback transport"""
        self.state = TransportState.DISCONNECTED
    
    async def close(self) -> None:
        """Close callback transport"""
        await self.disconnect()
        self.state = TransportState.CLOSED


class LoggingTransport(TransportInterface):
    """
    Transport that logs events to standard logging system.
    
    Useful for:
    - Development and debugging
    - Testing transport patterns
    - Monitoring event flow
    - Fallback when other transports fail
    """
    
    def __init__(self, logger_name: str = "event_transport", log_level: int = logging.INFO):
        super().__init__("LoggingTransport")
        logging_manager = LoggingManager(logger_name)
        self.transport_logger = logging_manager.get_logger()
        self.log_level = log_level
        self.state = TransportState.CONNECTED  # Always "connected"
    
    async def send(self, event: Any, metadata: Optional[Dict[str, Any]] = None) -> bool:
        """Log event to logging system"""
        start_time = time.time()
        
        try:
            # Serialize event data
            if hasattr(event, 'model_dump'):
                event_data = event.model_dump()
            elif isinstance(event, dict):
                event_data = event
            else:
                event_data = {"content": str(event)}
            
            # Create log message
            log_message = {
                "transport": self.name,
                "event": event_data,
                "metadata": metadata or {},
                "timestamp": time.time()
            }
            
            # Log the event
            self.transport_logger.log(self.log_level, f"Transport event: {json.dumps(log_message, default=str)}")
            
            # Update metrics
            self.metrics.total_sent += 1
            self.metrics.last_success_time = time.time()
            send_time = time.time() - start_time
            self.metrics.average_send_time = (
                (self.metrics.average_send_time * (self.metrics.total_sent - 1) + send_time) 
                / self.metrics.total_sent
            )
            
            return True
            
        except Exception as e:
            self.metrics.total_failed += 1
            self.metrics.last_failure_time = time.time()
            self._logger.error(f"Logging transport failed: {e}")
            return False
    
    async def connect(self) -> bool:
        """Logging transport is always connected"""
        self.state = TransportState.CONNECTED
        self.metrics.connection_count += 1
        return True
    
    async def disconnect(self) -> None:
        """Disconnect logging transport"""
        self.state = TransportState.DISCONNECTED
    
    async def close(self) -> None:
        """Close logging transport"""
        await self.disconnect()
        self.state = TransportState.CLOSED


class NullTransport(TransportInterface):
    """
    No-op transport for testing and development.
    
    Always succeeds but doesn't actually send events anywhere.
    Useful for:
    - Performance testing
    - Disabling transport in development
    - Testing error handling paths
    - Baseline performance measurements
    """
    
    def __init__(self):
        super().__init__("NullTransport")
        self.state = TransportState.CONNECTED  # Always "connected"
    
    async def send(self, event: Any, metadata: Optional[Dict[str, Any]] = None) -> bool:
        """Always succeeds without doing anything"""
        start_time = time.time()
        
        # Simulate minimal processing time
        await asyncio.sleep(0.0001)  # 0.1ms delay
        
        # Update metrics
        self.metrics.total_sent += 1
        self.metrics.last_success_time = time.time()
        send_time = time.time() - start_time
        self.metrics.average_send_time = (
            (self.metrics.average_send_time * (self.metrics.total_sent - 1) + send_time) 
            / self.metrics.total_sent
        )
        
        return True
    
    async def connect(self) -> bool:
        """Null transport is always connected"""
        self.state = TransportState.CONNECTED
        self.metrics.connection_count += 1
        return True
    
    async def disconnect(self) -> None:
        """Disconnect null transport"""
        self.state = TransportState.DISCONNECTED
    
    async def close(self) -> None:
        """Close null transport"""
        await self.disconnect()
        self.state = TransportState.CLOSED


class FileTransport(TransportInterface):
    """
    Transport that writes events to files.
    
    Useful for:
    - Event archival and auditing
    - Offline processing pipelines
    - Backup transport when primary fails
    - Development and testing
    """
    
    def __init__(self, file_path: str, format: str = "jsonl"):
        super().__init__("FileTransport")
        self.file_path = file_path
        self.format = format
        self._file_handle = None
    
    async def send(self, event: Any, metadata: Optional[Dict[str, Any]] = None) -> bool:
        """Write event to file"""
        if not self.is_connected:
            raise TransportConnectionError("FileTransport not connected")
        
        start_time = time.time()
        
        try:
            # Serialize event data
            if hasattr(event, 'model_dump'):
                event_data = event.model_dump()
            elif isinstance(event, dict):
                event_data = event
            else:
                event_data = {"content": str(event)}
            
            # Create transport entry
            transport_entry = {
                "timestamp": time.time(),
                "transport": self.name,
                "event": event_data,
                "metadata": metadata or {}
            }
            
            # Write to file
            if self.format == "jsonl":
                self._file_handle.write(json.dumps(transport_entry, default=str) + '\n')
            else:
                self._file_handle.write(str(transport_entry) + '\n')
            
            self._file_handle.flush_id()
            
            # Update metrics
            self.metrics.total_sent += 1
            self.metrics.last_success_time = time.time()
            send_time = time.time() - start_time
            self.metrics.average_send_time = (
                (self.metrics.average_send_time * (self.metrics.total_sent - 1) + send_time) 
                / self.metrics.total_sent
            )
            
            return True
            
        except Exception as e:
            self.metrics.total_failed += 1
            self.metrics.last_failure_time = time.time()
            self._logger.error(f"File transport failed: {e}")
            return False
    
    async def connect(self) -> bool:
        """Open file for writing"""
        try:
            self.state = TransportState.CONNECTING
            self._file_handle = open(self.file_path, 'a', encoding='utf-8')
            self.state = TransportState.CONNECTED
            self.metrics.connection_count += 1
            return True
        except Exception as e:
            self.state = TransportState.FAILED
            raise TransportConnectionError(f"Failed to open file {self.file_path}: {e}")
    
    async def disconnect(self) -> None:
        """Close file handle"""
        if self._file_handle:
            self._file_handle.close()
            self._file_handle = None
        self.state = TransportState.DISCONNECTED
    
    async def close(self) -> None:
        """Close file transport"""
        await self.disconnect()
        self.state = TransportState.CLOSED


class HTTPTransport(TransportInterface):
    """
    Transport that sends events via HTTP POST requests.
    
    Useful for:
    - Webhook integrations
    - REST API endpoints
    - Cloud service integration
    - Remote logging services
    """
    
    def __init__(self, endpoint_url: str, headers: Optional[Dict[str, str]] = None, timeout: float = 30.0):
        super().__init__("HTTPTransport")
        self.endpoint_url = endpoint_url
        self.headers = headers or {"Content-Type": "application/json"}
        self.timeout = timeout
        self._session = None
    
    async def send(self, event: Any, metadata: Optional[Dict[str, Any]] = None) -> bool:
        """Send event via HTTP POST"""
        if not self.is_connected:
            raise TransportConnectionError("HTTPTransport not connected")
        
        start_time = time.time()
        
        try:
            # Serialize event data
            if hasattr(event, 'model_dump'):
                event_data = event.model_dump()
            elif isinstance(event, dict):
                event_data = event
            else:
                event_data = {"content": str(event)}
            
            # Create HTTP payload
            payload = {
                "timestamp": time.time(),
                "transport": self.name,
                "event": event_data,
                "metadata": metadata or {}
            }
            
            # This is a placeholder - in real implementation would use aiohttp or similar
            # For now, just simulate the HTTP call
            await asyncio.sleep(0.01)  # Simulate network delay
            
            # In real implementation:
            # async with self._session.post(
            #     self.endpoint_url,
            #     json=payload,
            #     headers=self.headers,
            #     timeout=self.timeout
            # ) as response:
            #     response.raise_for_status()
            
            # Update metrics
            self.metrics.total_sent += 1
            self.metrics.last_success_time = time.time()
            send_time = time.time() - start_time
            self.metrics.average_send_time = (
                (self.metrics.average_send_time * (self.metrics.total_sent - 1) + send_time) 
                / self.metrics.total_sent
            )
            
            return True
            
        except Exception as e:
            self.metrics.total_failed += 1
            self.metrics.last_failure_time = time.time()
            self._logger.error(f"HTTP transport failed: {e}")
            return False
    
    async def connect(self) -> bool:
        """Initialize HTTP session"""
        try:
            self.state = TransportState.CONNECTING
            # In real implementation: self._session = aiohttp.ClientSession()
            self.state = TransportState.CONNECTED
            self.metrics.connection_count += 1
            return True
        except Exception as e:
            self.state = TransportState.FAILED
            raise TransportConnectionError(f"Failed to initialize HTTP session: {e}")
    
    async def disconnect(self) -> None:
        """Close HTTP session"""
        if self._session:
            # In real implementation: await self._session.close()
            self._session = None
        self.state = TransportState.DISCONNECTED
    
    async def close(self) -> None:
        """Close HTTP transport"""
        await self.disconnect()
        self.state = TransportState.CLOSED


class QueueTransport(TransportInterface):
    """
    Example queue transport (non-functional, for documentation).
    
    Demonstrates how to implement queue-based transports for:
    - RabbitMQ
    - Redis queues
    - AWS SQS
    - Azure Service Bus
    """
    
    def __init__(self, queue_name: str, connection_string: str):
        super().__init__("QueueTransport")
        self.queue_name = queue_name
        self.connection_string = connection_string
        self._connection = None
        self._channel = None
    
    async def send(self, event: Any, metadata: Optional[Dict[str, Any]] = None) -> bool:
        """Send event to queue"""
        if not self.is_connected:
            raise TransportConnectionError("QueueTransport not connected")
        
        start_time = time.time()
        
        try:
            # Serialize event data
            if hasattr(event, 'model_dump'):
                event_data = event.model_dump()
            elif isinstance(event, dict):
                event_data = event
            else:
                event_data = {"content": str(event)}
            
            # Create queue message
            message = {
                "timestamp": time.time(),
                "transport": self.name,
                "event": event_data,
                "metadata": metadata or {}
            }
            
            # Simulate queue send (in real implementation would use actual queue client)
            await asyncio.sleep(0.005)  # Simulate queue latency
            
            # In real implementation:
            # await self._channel.basic_publish(
            #     exchange='',
            #     routing_key=self.queue_name,
            #     body=json.dumps(message, default=str)
            # )
            
            # Update metrics
            self.metrics.total_sent += 1
            self.metrics.last_success_time = time.time()
            send_time = time.time() - start_time
            self.metrics.average_send_time = (
                (self.metrics.average_send_time * (self.metrics.total_sent - 1) + send_time) 
                / self.metrics.total_sent
            )
            
            return True
            
        except Exception as e:
            self.metrics.total_failed += 1
            self.metrics.last_failure_time = time.time()
            self._logger.error(f"Queue transport failed: {e}")
            return False
    
    async def connect(self) -> bool:
        """Connect to queue"""
        try:
            self.state = TransportState.CONNECTING
            # In real implementation: connect to queue service
            # self._connection = await aio_pika.connect_robust(self.connection_string)
            # self._channel = await self._connection.channel()
            self.state = TransportState.CONNECTED
            self.metrics.connection_count += 1
            return True
        except Exception as e:
            self.state = TransportState.FAILED
            raise TransportConnectionError(f"Failed to connect to queue: {e}")
    
    async def disconnect(self) -> None:
        """Disconnect from queue"""
        if self._connection:
            # In real implementation: await self._connection.close()
            self._connection = None
            self._channel = None
        self.state = TransportState.DISCONNECTED
    
    async def close(self) -> None:
        """Close queue transport"""
        await self.disconnect()
        self.state = TransportState.CLOSED


class RetryTransport(TransportInterface):
    """
    Wrapper transport that adds retry logic to any other transport.
    
    Provides:
    - Configurable retry attempts
    - Exponential backoff
    - Circuit breaker pattern
    - Fallback transport support
    """
    
    def __init__(
        self, 
        wrapped_transport: TransportInterface,
        max_retries: int = 3,
        base_delay: float = 1.0,
        max_delay: float = 60.0,
        backoff_multiplier: float = 2.0,
        fallback_transport: Optional[TransportInterface] = None
    ):
        super().__init__(f"Retry({wrapped_transport.name})")
        self.wrapped_transport = wrapped_transport
        self.max_retries = max_retries
        self.base_delay = base_delay
        self.max_delay = max_delay
        self.backoff_multiplier = backoff_multiplier
        self.fallback_transport = fallback_transport
        self._circuit_breaker_failures = 0
        self._circuit_breaker_threshold = 10
        self._circuit_breaker_reset_time = None
    
    async def send(self, event: Any, metadata: Optional[Dict[str, Any]] = None) -> bool:
        """Send with retry logic"""
        # Check circuit breaker
        if self._is_circuit_open():
            if self.fallback_transport:
                return await self.fallback_transport.send(event, metadata)
            return False
        
        last_exception = None
        
        for attempt in range(self.max_retries + 1):
            try:
                result = await self.wrapped_transport.send(event, metadata)
                if result:
                    # Reset circuit breaker on success
                    self._circuit_breaker_failures = 0
                    self._circuit_breaker_reset_time = None
                    
                    # Update metrics
                    if attempt > 0:
                        self.metrics.total_retries += attempt
                    self.metrics.total_sent += 1
                    self.metrics.last_success_time = time.time()
                    
                    return True
                else:
                    raise TransportError("Transport returned False")
                    
            except Exception as e:
                last_exception = e
                self._circuit_breaker_failures += 1
                
                if attempt < self.max_retries:
                    delay = min(self.base_delay * (self.backoff_multiplier ** attempt), self.max_delay)
                    await asyncio.sleep(delay)
                    self._logger.debug(f"Retrying transport (attempt {attempt + 2}/{self.max_retries + 1})")
        
        # All retries failed
        self.metrics.total_failed += 1
        self.metrics.total_retries += self.max_retries
        self.metrics.last_failure_time = time.time()
        
        # Try fallback if available
        if self.fallback_transport:
            try:
                return await self.fallback_transport.send(event, metadata)
            except Exception:
                pass
        
        self._logger.error(f"Transport failed after {self.max_retries + 1} attempts: {last_exception}")
        return False
    
    def _is_circuit_open(self) -> bool:
        """Check if circuit breaker is open"""
        if self._circuit_breaker_failures < self._circuit_breaker_threshold:
            return False
        
        if self._circuit_breaker_reset_time is None:
            self._circuit_breaker_reset_time = time.time() + 60  # 1 minute reset
            return True
        
        if time.time() > self._circuit_breaker_reset_time:
            self._circuit_breaker_failures = 0
            self._circuit_breaker_reset_time = None
            return False
        
        return True
    
    async def connect(self) -> bool:
        """Connect wrapped transport"""
        result = await self.wrapped_transport.connect()
        if result:
            self.state = TransportState.CONNECTED
            self.metrics.connection_count += 1
        return result
    
    async def disconnect(self) -> None:
        """Disconnect wrapped transport"""
        await self.wrapped_transport.disconnect()
        if self.fallback_transport:
            await self.fallback_transport.disconnect()
        self.state = TransportState.DISCONNECTED
    
    async def close(self) -> None:
        """Close wrapped transport"""
        await self.wrapped_transport.close()
        if self.fallback_transport:
            await self.fallback_transport.close()
        self.state = TransportState.CLOSED
    
    @property
    def is_connected(self) -> bool:
        """Check if wrapped transport is connected"""
        return self.wrapped_transport.is_connected