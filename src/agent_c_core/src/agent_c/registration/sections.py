

def get_section_registry():
    """
    Returns the section registry for the agent_c package.
    This registry is used to manage and retrieve sections within the agent_c framework.
    """
    from agent_c.util.registries.section_registry import SectionRegistry
    return SectionRegistry