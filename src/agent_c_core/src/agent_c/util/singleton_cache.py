"""
Singleton cache infrastructure for Agent C config loaders.

This module provides a metaclass-based singleton implementation that creates
separate instances for different initialization parameters while sharing
instances for identical parameters. Designed for thread-safe operation with
explicit cache management capabilities.
"""

import hashlib
import threading
import time
import weakref
from collections import OrderedDict
from typing import Any, Dict, Optional, Tuple, Type, TypeVar, Callable
from pathlib import Path

T = TypeVar('T')


class SingletonCacheMeta(type):
    """
    Metaclass that implements cache key-based singleton pattern.
    
    Creates singleton instances based on cache keys generated from initialization
    parameters. Different parameter combinations get separate instances, while
    identical parameters share the same instance.
    
    Features:
    - Thread-safe instance creation and retrieval
    - Weak references to allow garbage collection
    - Cache invalidation support for testing
    - Deterministic cache key generation
    """
    
    def __init__(cls, name: str, bases: Tuple[Type, ...], namespace: Dict[str, Any]):
        super().__init__(name, bases, namespace)
        cls._instances: Dict[str, weakref.ReferenceType] = {}
        cls._lock = threading.RLock()
        cls._cache_stats = {
            'hits': 0,
            'misses': 0,
            'evictions': 0
        }
    
    def __call__(cls, *args, **kwargs):
        """
        Create or retrieve singleton instance based on cache key.
        
        Args:
            *args: Positional arguments for instance creation
            **kwargs: Keyword arguments for instance creation
            
        Returns:
            Singleton instance for the given parameters
        """
        cache_key = cls._generate_cache_key(*args, **kwargs)
        
        with cls._lock:
            # Check if we have a valid instance
            if cache_key in cls._instances:
                instance = cls._instances[cache_key]()
                if instance is not None:
                    cls._cache_stats['hits'] += 1
                    return instance
                else:
                    # Weak reference died, clean it up
                    del cls._instances[cache_key]
                    cls._cache_stats['evictions'] += 1
            
            # Create new instance
            cls._cache_stats['misses'] += 1
            instance = super(SingletonCacheMeta, cls).__call__(*args, **kwargs)
            
            # Store weak reference to allow garbage collection
            cls._instances[cache_key] = weakref.ref(instance)
            
            return instance
    
    def _generate_cache_key(cls, *args, **kwargs) -> str:
        """
        Generate deterministic cache key from initialization parameters.
        
        Args:
            *args: Positional arguments
            **kwargs: Keyword arguments
            
        Returns:
            String cache key that uniquely identifies the parameter combination
        """
        # Get the __init__ signature to map args to parameter names
        import inspect
        sig = inspect.signature(cls.__init__)
        bound_args = sig.bind_partial(None, *args, **kwargs)  # None for 'self'
        bound_args.apply_defaults()
        
        # Remove 'self' from the arguments
        params = dict(bound_args.arguments)
        params.pop('self', None)
        
        # Create cache key from normalized parameters
        key_parts = []
        for param_name in sorted(params.keys()):
            value = params[param_name]
            normalized_value = cls._normalize_cache_value(value)
            key_parts.append(f"{param_name}={normalized_value}")
        
        cache_key_str = "|".join(key_parts)
        
        # Hash the key to keep it manageable and avoid issues with special characters
        return hashlib.sha256(cache_key_str.encode('utf-8')).hexdigest()[:16]
    
    @staticmethod
    def _normalize_cache_value(value: Any) -> str:
        """
        Normalize a parameter value for cache key generation.
        
        Args:
            value: Parameter value to normalize
            
        Returns:
            String representation suitable for cache key
        """
        if value is None:
            return "None"
        elif isinstance(value, (str, int, float, bool)):
            return str(value)
        elif isinstance(value, Path):
            return str(value.resolve())
        elif hasattr(value, '__dict__'):
            # For objects, use their type and relevant attributes
            # This is a simplified approach - may need refinement per class
            return f"{type(value).__name__}:{id(value)}"
        else:
            # Fallback to string representation
            return str(value)
    
    def clear_cache(cls) -> int:
        """
        Clear all cached instances for this class.
        
        Returns:
            Number of instances that were cleared
        """
        with cls._lock:
            count = len(cls._instances)
            cls._instances.clear()
            cls._cache_stats['evictions'] += count
            return count
    
    def get_cache_stats(cls) -> Dict[str, int]:
        """
        Get cache performance statistics.
        
        Returns:
            Dictionary with cache hit/miss/eviction counts
        """
        with cls._lock:
            return cls._cache_stats.copy()
    
    def get_cached_instances(cls) -> int:
        """
        Get count of currently cached instances.
        
        Returns:
            Number of live instances in cache
        """
        with cls._lock:
            # Clean up dead weak references while counting
            live_instances = {}
            for key, weak_ref in cls._instances.items():
                if weak_ref() is not None:
                    live_instances[key] = weak_ref
                else:
                    cls._cache_stats['evictions'] += 1
            
            cls._instances = live_instances
            return len(live_instances)
    
    def invalidate_instance(cls, *args, **kwargs) -> bool:
        """
        Invalidate a specific cached instance.
        
        Args:
            *args: Positional arguments that were used to create the instance
            **kwargs: Keyword arguments that were used to create the instance
            
        Returns:
            True if an instance was invalidated, False if not found
        """
        cache_key = cls._generate_cache_key(*args, **kwargs)
        
        with cls._lock:
            if cache_key in cls._instances:
                del cls._instances[cache_key]
                cls._cache_stats['evictions'] += 1
                return True
            return False


class CacheManager:
    """
    Utility class for managing singleton caches across multiple classes.
    
    Provides centralized cache management operations and statistics collection.
    """
    
    @staticmethod
    def clear_all_caches(*classes: Type) -> Dict[str, int]:
        """
        Clear caches for multiple singleton classes.
        
        Args:
            *classes: Classes with SingletonCacheMeta metaclass
            
        Returns:
            Dictionary mapping class names to number of cleared instances
        """
        results = {}
        for cls in classes:
            if hasattr(cls, 'clear_cache'):
                results[cls.__name__] = cls.clear_cache()
            else:
                results[cls.__name__] = 0
        return results
    
    @staticmethod
    def get_all_cache_stats(*classes: Type) -> Dict[str, Dict[str, int]]:
        """
        Get cache statistics for multiple singleton classes.
        
        Args:
            *classes: Classes with SingletonCacheMeta metaclass
            
        Returns:
            Dictionary mapping class names to their cache statistics
        """
        results = {}
        for cls in classes:
            if hasattr(cls, 'get_cache_stats'):
                results[cls.__name__] = cls.get_cache_stats()
            else:
                results[cls.__name__] = {'hits': 0, 'misses': 0, 'evictions': 0}
        return results
    
    @staticmethod
    def get_cache_summary(*classes: Type) -> Dict[str, Any]:
        """
        Get comprehensive cache summary for multiple classes.
        
        Args:
            *classes: Classes with SingletonCacheMeta metaclass
            
        Returns:
            Dictionary with cache statistics and instance counts
        """
        summary = {
            'total_instances': 0,
            'total_hits': 0,
            'total_misses': 0,
            'total_evictions': 0,
            'classes': {}
        }
        
        for cls in classes:
            if hasattr(cls, 'get_cache_stats') and hasattr(cls, 'get_cached_instances'):
                stats = cls.get_cache_stats()
                instances = cls.get_cached_instances()
                
                summary['classes'][cls.__name__] = {
                    'instances': instances,
                    'stats': stats
                }
                
                summary['total_instances'] += instances
                summary['total_hits'] += stats['hits']
                summary['total_misses'] += stats['misses']
                summary['total_evictions'] += stats['evictions']
        
        # Calculate hit rate
        total_requests = summary['total_hits'] + summary['total_misses']
        summary['hit_rate'] = summary['total_hits'] / total_requests if total_requests > 0 else 0.0
        
        return summary


class ThreadSafeLRUCache:
    """
    Thread-safe LRU cache with size limits and fine-grained locking.
    
    Provides additional caching layer for expensive operations like file I/O
    and path resolution that can be shared across singleton instances.
    """
    
    def __init__(self, max_size: int = 128, ttl_seconds: Optional[float] = None):
        """
        Initialize the LRU cache.
        
        Args:
            max_size: Maximum number of items to cache
            ttl_seconds: Time-to-live for cache entries (None for no expiration)
        """
        self.max_size = max_size
        self.ttl_seconds = ttl_seconds
        self._cache: OrderedDict[str, Any] = OrderedDict()
        self._timestamps: Dict[str, float] = {}
        self._lock = threading.RLock()
        self._stats = {
            'hits': 0,
            'misses': 0,
            'evictions': 0,
            'expired': 0
        }
    
    def get(self, key: str, default: Any = None) -> Any:
        """
        Get item from cache.
        
        Args:
            key: Cache key
            default: Default value if key not found
            
        Returns:
            Cached value or default
        """
        with self._lock:
            if key in self._cache:
                # Check TTL if configured
                if self._is_expired(key):
                    self._remove_key(key)
                    self._stats['expired'] += 1
                    self._stats['misses'] += 1
                    return default
                
                # Move to end (most recently used)
                self._cache.move_to_end(key)
                self._stats['hits'] += 1
                return self._cache[key]
            
            self._stats['misses'] += 1
            return default
    
    def put(self, key: str, value: Any) -> None:
        """
        Put item in cache.
        
        Args:
            key: Cache key
            value: Value to cache
        """
        with self._lock:
            # Update existing key
            if key in self._cache:
                self._cache[key] = value
                self._cache.move_to_end(key)
                self._timestamps[key] = time.time()
                return
            
            # Add new key, check size limit
            if len(self._cache) >= self.max_size:
                self._evict_lru()
            
            self._cache[key] = value
            self._timestamps[key] = time.time()
    
    def get_or_compute(self, key: str, compute_func: Callable[[], Any]) -> Any:
        """
        Get item from cache or compute and cache it.
        
        Args:
            key: Cache key
            compute_func: Function to compute value if not cached
            
        Returns:
            Cached or computed value
        """
        value = self.get(key)
        if value is None:
            value = compute_func()
            self.put(key, value)
        return value
    
    def invalidate(self, key: str) -> bool:
        """
        Remove specific key from cache.
        
        Args:
            key: Cache key to remove
            
        Returns:
            True if key was removed, False if not found
        """
        with self._lock:
            if key in self._cache:
                self._remove_key(key)
                return True
            return False
    
    def clear(self) -> int:
        """
        Clear all cache entries.
        
        Returns:
            Number of entries cleared
        """
        with self._lock:
            count = len(self._cache)
            self._cache.clear()
            self._timestamps.clear()
            self._stats['evictions'] += count
            return count
    
    def cleanup_expired(self) -> int:
        """
        Remove all expired entries.
        
        Returns:
            Number of expired entries removed
        """
        if self.ttl_seconds is None:
            return 0
        
        with self._lock:
            expired_keys = []
            for key in list(self._cache.keys()):
                if self._is_expired(key):
                    expired_keys.append(key)
            
            for key in expired_keys:
                self._remove_key(key)
            
            self._stats['expired'] += len(expired_keys)
            return len(expired_keys)
    
    def get_stats(self) -> Dict[str, Any]:
        """
        Get cache statistics.
        
        Returns:
            Dictionary with cache statistics
        """
        with self._lock:
            stats = self._stats.copy()
            stats['size'] = len(self._cache)
            stats['max_size'] = self.max_size
            
            total_requests = stats['hits'] + stats['misses']
            stats['hit_rate'] = stats['hits'] / total_requests if total_requests > 0 else 0.0
            
            return stats
    
    def _is_expired(self, key: str) -> bool:
        """
        Check if cache entry is expired.
        
        Args:
            key: Cache key to check
            
        Returns:
            True if expired, False otherwise
        """
        if self.ttl_seconds is None:
            return False
        
        timestamp = self._timestamps.get(key, 0)
        return time.time() - timestamp > self.ttl_seconds
    
    def _evict_lru(self) -> None:
        """
        Evict least recently used item.
        """
        if self._cache:
            key, _ = self._cache.popitem(last=False)  # Remove first (LRU)
            self._timestamps.pop(key, None)
            self._stats['evictions'] += 1
    
    def _remove_key(self, key: str) -> None:
        """
        Remove key from cache and timestamps.
        
        Args:
            key: Key to remove
        """
        self._cache.pop(key, None)
        self._timestamps.pop(key, None)


class SharedCacheRegistry:
    """
    Registry for shared cache instances used across singleton config loaders.
    
    Provides centralized management of shared caches for common operations
    like file I/O, path resolution, and configuration parsing.
    """
    
    _instance: Optional['SharedCacheRegistry'] = None
    _lock = threading.Lock()
    
    def __new__(cls) -> 'SharedCacheRegistry':
        """Singleton pattern for the registry itself."""
        if cls._instance is None:
            with cls._lock:
                if cls._instance is None:
                    cls._instance = super().__new__(cls)
                    cls._instance._initialized = False
        return cls._instance
    
    def __init__(self):
        """Initialize the registry (only once)."""
        if hasattr(self, '_initialized') and self._initialized:
            return
        
        self._caches: Dict[str, ThreadSafeLRUCache] = {}
        self._registry_lock = threading.RLock()
        self._initialized = True
    
    def get_cache(self, name: str, max_size: int = 128, ttl_seconds: Optional[float] = None) -> ThreadSafeLRUCache:
        """
        Get or create a named cache.
        
        Args:
            name: Cache name
            max_size: Maximum cache size
            ttl_seconds: Time-to-live for entries
            
        Returns:
            ThreadSafeLRUCache instance
        """
        with self._registry_lock:
            if name not in self._caches:
                self._caches[name] = ThreadSafeLRUCache(max_size, ttl_seconds)
            return self._caches[name]
    
    def clear_cache(self, name: str) -> int:
        """
        Clear a specific named cache.
        
        Args:
            name: Cache name to clear
            
        Returns:
            Number of entries cleared
        """
        with self._registry_lock:
            if name in self._caches:
                return self._caches[name].clear()
            return 0
    
    def clear_all_caches(self) -> Dict[str, int]:
        """
        Clear all registered caches.
        
        Returns:
            Dictionary mapping cache names to cleared entry counts
        """
        with self._registry_lock:
            results = {}
            for name, cache in self._caches.items():
                results[name] = cache.clear()
            return results
    
    def cleanup_expired(self) -> Dict[str, int]:
        """
        Clean up expired entries in all caches.
        
        Returns:
            Dictionary mapping cache names to expired entry counts
        """
        with self._registry_lock:
            results = {}
            for name, cache in self._caches.items():
                results[name] = cache.cleanup_expired()
            return results
    
    def get_all_stats(self) -> Dict[str, Dict[str, Any]]:
        """
        Get statistics for all registered caches.
        
        Returns:
            Dictionary mapping cache names to their statistics
        """
        with self._registry_lock:
            return {name: cache.get_stats() for name, cache in self._caches.items()}
    
    def get_cache_names(self) -> list[str]:
        """
        Get list of all registered cache names.
        
        Returns:
            List of cache names
        """
        with self._registry_lock:
            return list(self._caches.keys())


# Global shared cache registry instance
shared_cache_registry = SharedCacheRegistry()


# Predefined cache names for common operations
class CacheNames:
    """Standard cache names for config loader operations."""
    PATH_RESOLUTION = "path_resolution"
    FILE_CONTENT = "file_content"
    JSON_PARSING = "json_parsing"
    AGENT_CONFIGS = "agent_configs"
    MODEL_CONFIGS = "model_configs"
    YAML_PARSING = "yaml_parsing"
    CONFIG_FILES = "config_files"
    PROMPT_FILEs = "prompt_files"
