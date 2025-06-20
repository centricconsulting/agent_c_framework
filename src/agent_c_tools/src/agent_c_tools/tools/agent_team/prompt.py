from typing import Any, Optional, Dict, List

from agent_c.models.completion.agent_config import AgentConfiguration
from agent_c.prompting.prompt_section import PromptSection, property_bag_item


class AgentTeamSection(PromptSection):
    tool: Any

    def __init__(self, **data: Any):

        TEMPLATE = ("The Agent Team Toolset (ateam) provides you with a dedicated team of agents to leverage for performing tasks, answering questions "
                    "on behalf of the user or to execute your plans. These are highly capable agents with very specialized knowledge, they will allow "
                    "you to ensure both high quality and token efficiency by offloading the 'heavy lifting' to subject matter experts.\n\n"
                    "## Available Team Member IDs:\n${at_agent_ids}\n\n"
                    "$at_sessions")
        super().__init__(template=TEMPLATE, required=True, name="Agent Assist", render_section_header=True, **data)

    @property_bag_item
    async def at_agent_ids(self, prompt_context: Dict[str, Any]) -> str:
        agent_config = prompt_context.get('agent_config')
        available = self._filter_agent_catalog(agent_config)
        agent_descriptions = []
        for sub_agent in available:
            agent_descriptions.append(f"**{sub_agent.name}**, Agent Key: `{sub_agent.key}`\nTools:{", ".join(sub_agent.tools)}{sub_agent.agent_description}")

        return "\n".join(agent_descriptions)

    def _filter_agent_catalog(self, agent: Optional[AgentConfiguration]) -> List[AgentConfiguration]:
        catalog = self.tool.agent_loader.catalog
        available = []
        valid_keys = ["all_teams"]
        if agent is not None:
            valid_keys.append(agent.key)

        for key, agent_config in catalog.items():
            if any(valid_key in agent_config.category for valid_key in valid_keys):
                available.append(agent_config)

        return available

    @property_bag_item
    async def at_sessions(self, prompt_context: Dict[str, Any]) -> str:
        try:
            session_id = prompt_context.get('user_session_id', prompt_context.get('session_id', None))
            agent_sessions: List[Dict[str, Any]] = self.tool.list_active_sessions(session_id)
            sess_list = []

            if len(agent_sessions):
                for ses in agent_sessions:
                    agent_config: AgentConfiguration = self.tool.agent_loader.catalog.get(ses['agent_key'])
                    sess_list.append(f"- `{ses['agent_session_id']}` with {agent_config.name}. {len(ses['messages'])} messages")

                return f"\n\n## Active Team Sessions:\n{"\n".join(sess_list)}\n\n"
        except Exception as e:
            self.tool.logger.exception(f"Error retrieving active agent sessions: {e}", exc_info=True)
            return "\n\n## Active Team Sessions:\nError fetching team sessions. \n\n"

        return "\n"