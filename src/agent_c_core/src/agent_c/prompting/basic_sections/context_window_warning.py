from typing import Optional

from pydantic import Field
from agent_c.models.prompts.base import BasePromptSection, SectionRenderSlot

class ContextWindowWarning(BasePromptSection):
    """
    {% if chat_session.context_window_remaining < 10 %}
    URGENT!! YOUR CONTEXT WINDOW IS NEARLY FULL! YOU **MUST** HALT ALL WORK IMMEDIATELY AND AWAIT FURTHER INSTRUCTIONS!
    {% elif chat_session.context_window_remaining < 26 %}
    WARNING! Your context window is dangerously full. Find a stopping point, halt activity and await additional instruction.
    {% elif chat_session.context_window_remaining < 50 %}
    CAUTION: You have consumed more than half of your context window. Consider wrapping up your current task.
    {% endif %}
    """
    render_slot: SectionRenderSlot = Field(SectionRenderSlot.SYSTEM)
    auto_include: bool = Field(True)
    template_key: Optional[str] = Field("core/context_window_warning")
    section_description: str = Field("Context window warning section to progressively alert the agent when as the context window fills up. included for ALL agents"),