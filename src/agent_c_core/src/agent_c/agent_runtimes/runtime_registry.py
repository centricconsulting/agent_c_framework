from typing import Optional

from agent_c.agent_runtimes.base import AgentRuntime
from agent_c.config import ModelConfigurationLoader
from agent_c.models.model_config.models import ModelConfiguration

class RuntimeRegistryEntry:
    def __init__(self, runtime_cls: type[AgentRuntime], vendor: str):
        self.runtime_cls = runtime_cls
        self.vendor = vendor


class RuntimeRegistry:
    _registry: dict[str, RuntimeRegistryEntry] = {}

    @classmethod
    def register(cls, runtime_cls: type[AgentRuntime], vendor: str):
        if not issubclass(runtime_cls, AgentRuntime):
            raise TypeError(f"{runtime_cls.__name__} must be a subclass of BaseAgent")

        if runtime_cls.__name__ not in cls._registry:
            cls._registry[vendor] = RuntimeRegistryEntry(runtime_cls, vendor)

    @classmethod
    def runtime_for_vendor(cls, vendor: str) -> Optional[type[AgentRuntime]]:
        entry = cls._registry.get(vendor)
        if entry:
            return entry.runtime_cls

        return None

    @classmethod
    def runtime_for_model_id(cls, loader: ModelConfigurationLoader, model_id: str) -> Optional[type[AgentRuntime]]:
        model_config: ModelConfiguration = loader.model_id_map.get(model_id)
        if not model_config:
            return None

        return cls.runtime_for_vendor(model_config.vendor)

    @classmethod
    def is_vendor_runtime_usable(cls, vendor: str, context = None) -> bool:
        if vendor not in cls._registry:
            return False

        return cls._registry[vendor].runtime_cls.can_create(context)

    @classmethod
    def registered_runtime_names(cls) -> list[str]:
        return list(cls._registry.keys())

    @classmethod
    def registered_runtimes(cls) -> list[RuntimeRegistryEntry]:
        return [entry for entry in cls._registry.values()]

    @classmethod
    def usable_runtime_names(cls, context = None) -> list[str]:
        return [vendor for vendor in cls._registry if cls.is_vendor_runtime_usable(vendor, context)]

    @classmethod
    def usable_runtimes(cls, context = None) -> list[RuntimeRegistryEntry]:
        return [entry for entry in cls._registry.values() if cls.is_vendor_runtime_usable(entry.vendor, context)]

    @classmethod
    def instantiate_vendor_runtime(cls, vendor: str, context = None, max_retry_delay_secs: int = 300, concurrency_limit: int = 3) -> AgentRuntime | None:
        runtime_cls = cls.runtime_for_vendor(vendor)
        if runtime_cls is None:
            return None

        return runtime_cls(max_retry_delay_secs=max_retry_delay_secs,
                           concurrency_limit=concurrency_limit, context=context)

    @classmethod
    def instantiate_model_runtime(cls, model_loader: ModelConfigurationLoader,  model_id: str, context = None, max_retry_delay_secs: int = 300, concurrency_limit: int = 3) -> AgentRuntime | None:
        runtime_cls = cls.runtime_for_model_id(model_loader, model_id)
        if runtime_cls is None:
            return None

        return runtime_cls(max_retry_delay_secs=max_retry_delay_secs,
                           concurrency_limit=concurrency_limit, context=context)

