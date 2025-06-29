from agent_c.models.context import *

def get_context_registry():
    from agent_c.util.registries.context_registry import ContextRegistry
    return ContextRegistry