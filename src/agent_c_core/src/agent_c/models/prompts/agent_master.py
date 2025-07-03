from agent_c.models.prompts import BasePromptSection


class AgentMasterPrompt(BasePromptSection):
    """
    {# This is the master layout template for rendering the system prompt. #}
    {# It expects the following variables to be passed in via the context: #}
    {#  "before_agent_sections": List[str] - A list of prompt sections to render before the agent sections. #}
    {#  "agent_key": str - The key of the agent being rendered. #}
    {#  "after_agent_sections": List[str] - A list of prompt sections to render after the agent sections. #}
    {#  "tool_sections": List[str] - A list of tool sections to render. #}
    {#  "after_tool_sections": List[str] - A list of tool sections to render after the tool sections. #}

    {% set rendered_agent_prompt %}
        {% include agent_key %}
    {% endset %}

    """