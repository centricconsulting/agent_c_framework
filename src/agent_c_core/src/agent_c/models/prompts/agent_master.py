from agent_c.models.prompts import BasePromptSection


class AgentMasterPrompt(BasePromptSection):
    """
    {# This is the master layout template for rendering the system prompt. #}
    {# It expects and AgentPromptCollection to be passed in via the context: #}
    {% for col in [prompt_collection.system_level, prompt_collection.before_agent] %}
        {% for section in col %}
            {{ prompt_section(section.template_key) }}
        {% endfor %}
    {% endfor %}

    {% include prompt_collection.agent_template %}

    #% for section in prompt_collection.after_agent %}
        {{ prompt_section(section.template_key) }}
    {% endfor %}

    {% for section in prompt_collection.tools %}
        {{ tool_section(section.template_key) }}
    {% endfor %}

    {% for col in [prompt_collection.default, prompt_collection.at_bottom] %}
        {% for section in col %}
            {{ prompt_section(section.template_key) }}
        {% endfor %}
    {% endfor %}
    """