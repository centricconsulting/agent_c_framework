from agent_c.models.context import * # noqa: F401, F403

def get_context_registry():
    from agent_c.util.registries.context_registry import ContextRegistry
    return ContextRegistry