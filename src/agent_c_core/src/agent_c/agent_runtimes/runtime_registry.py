from agent_c.agent_runtimes.base import AgentRuntime

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
    def runtime_fpr_vendor(cls, vendor: str) -> type[AgentRuntime] | None:
        entry = cls._registry.get(vendor)
        if entry:
            return entry.runtime_cls

        return None

    @classmethod
    def is_vendor_runtime_useable(cls, vendor: str) -> bool:
        if vendor not in cls._registry:
            return False

        return cls._registry[vendor].runtime_cls