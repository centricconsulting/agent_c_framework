from typing import Dict, Any, Optional, List

class AgentSessionCache:
    def __init__(self):
        self._cache: Dict[str, Dict[str, Any]] = {}

    def set(self, key: str, session_data):
        self._cache[key] = session_data


    def get(self, key: str):
        return self._cache.get(key)

    def list_active(self) -> List[str]:
        return list(self._cache.keys())
