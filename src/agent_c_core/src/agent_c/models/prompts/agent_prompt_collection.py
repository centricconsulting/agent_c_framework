from typing import List, TYPE_CHECKING

from pydantic import Field
from agent_c.models.base import BaseModel
from agent_c.models.prompts import BasePromptSection

if TYPE_CHECKING:
    from agent_c.models.context.interaction_context import InteractionContext

class AgentPromptCollection(BaseModel):
    """
    A model representing a collection of prompts for an agent, organized by their slots
    """
    agent_template: str = Field("agents/default",
                                 description="The template for the agent's prompt")
    imports: List[str] = Field(default_factory=list,
                               description="List of Jinja 2 template imports that should be included in the agent's prompt")
    system_level: List[BasePromptSection] = Field(default_factory=list,
                                                    description="System level prompts that are always included in the agent's prompt")
    before_agent: List[BasePromptSection] = Field(default_factory=list,
                                                    description="sections more important than the agent instructions")
    after_agent: List[BasePromptSection] = Field(default_factory=list,
                                                    description="sections less important than the agent instructions but more important than tools")
    tools: List[BasePromptSection] = Field(default_factory=list,
                                                description="Tool sections")
    default: List[BasePromptSection] = Field(default_factory=list,
                                                description="Default render slot for sections that do not fit into other categories")
    at_bottom: List[BasePromptSection] = Field(default_factory=list,
                                                description="For temporary sections that should be rendered at the bottom of the prompt")

    @classmethod
    def from_sections(cls, agent_template: str,  sections: List[BasePromptSection]) -> 'AgentPromptCollection':
        """
        Create an AgentPromptCollection from a list of BasePromptSection instances.
        This method categorizes the sections into their respective slots.
        """
        slots = { 'include_only': [],
                  'system': [],
                  'before_agent': [],
                  'after_agent': [],
                  'tool': [],
                  'default': [],
                  'at_bottom': [] }

        for section in sections:
            slots[section.render_slot].append(section)

        for key, value in slots.items():
            slots[key] = [section for section in value if not section.is_include]

        return cls(
            agent_template=agent_template,
            imports=slots['include_only'],
            system_level=slots['system'],
            before_agent=slots['before_agent'],
            after_agent=slots['after_agent'],
            tools=slots['tool'],
            default=slots['default'],
            at_bottom=slots['at_bottom']
        )

    @classmethod
    def from_interaction_context(cls, interaction_context: 'InteractionContext') -> 'AgentPromptCollection':
        """
        Create an AgentPromptCollection from the sections in the interaction context.
        This method uses the sections from the interaction context to populate the slots.
        """
        agent_config = interaction_context.chat_session.agent_config
        agent_key: str = agent_config.template_key
        if interaction_context.chat_session.is_clone_session and not agent_key.endswith("_clone"):
            agent_key += "_clone"

        return cls.from_sections(agent_key, list(interaction_context.sections.values()))