import os
from diskcache import Cache

from typing import Any, Optional, Union


class ToolCache:
    """
    A delegate around a diskcache, or something that implements a diskcache interface

    Attributes:
        cache (Union[Cache, None]): An instance of a diskcache Cache or None.

    Methods:
        set: Set a key-value pair in the cache with optional expiration time.
        get: Get a value by key from the cache or return a default value if not found.
        delete: Delete a key-value pair from the cache.
        clear: Clear the entire cache.
    """

    def __init__(self, **kwargs: Any) -> None:
        """
        Keyword Arguments:
            cache (Optional[Cache]): An existing diskcache Cache instance. If not provided, one will be created.
            cache_dir (Optional[str]): The directory path for storing cache data if a new cache is created. Defaults to ".tool_cache".
        """
        self.cache: Optional[Cache] = kwargs.get('cache')
        if self.cache is None:
            cache_dir: str = kwargs.get('cache_dir', ".tool_cache")
            os.makedirs(cache_dir, exist_ok=True)
            self.cache = Cache(cache_dir)

    def set(self, key: Any, value: Any, expire: Optional[int] = None) -> None:
        """Set a key-value pair in the cache with optional expiration time.

        Args:
            key (Any): The key under which the value is stored.
            value (Any): The value to store.
            expire (Optional[int]): The number of seconds until this cache entry should expire.
        """
        self.cache.set(key, value, expire)

    def get(self, key: Any, default: Optional[Any] = None) -> Any:
        """Get a value by key from the cache or return a default value if not found.

        Args:
            key (Any): The key to retrieve the value.
            default (Optional[Any]): The default value to return if the key is not found.

        Returns:
            Any: The value from the cache or the default value.
        """
        return self.cache.get(key, default)

    def delete(self, key: Any) -> None:
        """Delete a key-value pair from the cache.

        Args:
            key (Any): The key to delete from the cache.
        """
        self.cache.delete(key)

    def clear(self) -> None:
        """Clear the entire cache."""
        self.cache.clear()
