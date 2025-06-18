from typing import Any, Optional, Dict, List

from agent_c.models.config.agent_config import CurrentAgentConfiguration, AgentConfiguration
from agent_c.prompting.prompt_section import PromptSection, property_bag_item

class AssistantBehaviorSection(PromptSection):
    def __init__(self, **data: Any):
        TEMPLATE = """**Important**: Agent Assist mode has been activated.\n\n# Assistant Operating Context\n\n
                You are being contacted via the Agent Assist Tool by another agent  to handle a specific delegated task. Important operating guidelines:\n

                - You are operating as a specialized clone with focused responsibilities\n
                - Your role is to complete the specific task assigned by your requesting agent\n
                - Limit your actions and responses to the directives and scope provided\n
                - Only your final response will be relayed back to the requesting agent\n
                - Focus on delivering complete, actionable results within your assigned scope\n
                - Do not attempt to expand beyond your delegated responsibilities\n
                - Provide thorough, high-quality output that addresses the specific request\n
                \n\n$agent_process_directive\n
                
                """
        super().__init__(template=TEMPLATE, required=True, name="AGENT ASSIST MODE ACTIVE", render_section_header=True, **data)

    @property_bag_item
    async def agent_process_directive(self, prompt_context: Dict[str, Any]) -> str:
        """
        Returns the agent process directive from the prompt context.
        """
        context_str: Optional[str]  = prompt_context.get('process_context', None)
        if context_str:
            return f"# Critical Process Context:\n\n{context_str.replace('$', '$$')}\n\n"
        return ""



class AgentAssistSection(PromptSection):
    tool: Any

    def __init__(self, **data: Any):

        TEMPLATE = ("The Agent Assist Toolset (aa) allows you to leverage other agents to perform tasks, answer questions "
                    "on behalf of the user or to execute your plans..These are highly capable agents with very specialized knowledge, they will allow "
                    "you to ensure both high quality and token efficiency by offloading the 'heavy lifting' to subject matter experts.\n\n"
                    "## Available Agent IDS:\n${agent_ids}\n\n"
                    "## Interaction Guidelines:\n"
                    "- The user will often use casual languages such as `ask cora to do X`.\n"
                    "  - If only one agent persona matches that name than use that agent ID to "
                    "make an`aa_` tool call.\n"
                    "  - If more than one name matches, inform the user of the ambiguity and list the roles available.\n"
                    "    - For example: I'm sorry, do you mean 'Cora the Agent C core dev' or 'Cora the Fast API dev'?\n"
                    "$aa_sessions")
        super().__init__(template=TEMPLATE, required=True, name="Agent Assist", render_section_header=True, **data)

    @property_bag_item
    async def agent_ids(self, prompt_context: Dict[str, Any]) -> str:
        agent_config = prompt_context.get('active_agent', prompt_context.get('agent_config', None))
        available = self._filter_agent_catalog(agent_config)
        agent_descriptions = []
        for sub_agent in available:
            agent_descriptions.append(f"**{sub_agent.name}**, Agent Key: `{sub_agent.key}`\nTools:{", ".join(sub_agent.tools)}{sub_agent.agent_description}")

        return "\n".join(agent_descriptions)

    def _filter_agent_catalog(self, agent: Optional[AgentConfiguration]) -> List[AgentConfiguration]:
        catalog = self.tool.agent_loader.catalog
        available = []
        valid_keys = ["agent_assist"]
        if agent is not None:
            valid_keys.append(agent.key)

        for key, agent_config in catalog.items():
            if any(valid_key in agent_config.category for valid_key in valid_keys):
                available.append(agent_config)

        return available

    @property_bag_item
    async def aa_sessions(self, prompt_context: Dict[str, Any]) -> str:
        session_id = prompt_context.get('user_session_id', prompt_context.get('session_id', None))
        agent_sessions: List[Dict[str, Any]] = self.tool.list_active_sessions(session_id)
        sess_list = []

        if len(agent_sessions):
            for ses in agent_sessions:
                agent: CurrentAgentConfiguration = self.tool.agent_loader.catalog.get(ses['agent_key'])
                sess_list.append(f"- `{ses['session_id']}` with {agent.name}. {len(ses['messages'])} messages")

            return f"\n\n## Active Agent Sessions:\n{"\n".join(sess_list)}\n\n"

        return "\n"