import os
import platform
import datetime

from typing import Any,  Optional, TYPE_CHECKING

from agent_c.prompting.prompt_section import PromptSection, property_bag_item

if TYPE_CHECKING:
    from agent_c.models import ChatSession



class EnvironmentInfoSection(PromptSection):
    """
    This class represents a section that provides runtime environment information to the agent,
    including the current timestamp, environment name, session info, and voice mode settings.
    Args:
        **data (Any): Additional keyword arguments passed to the PromptSection during initialization.
    """

    def __init__(self, **data: Any) -> None:
        """
        Initializes the EnvironmentInfoSection with template, environment rules, and other relevant data.

        Args:
            **data (Any): Data passed to customize the prompt section.
                          This can include overrides for the 'template', 'voice_tools', and 'env_rules' attributes.
        """
        TEMPLATE: str = (
            "Current time: ${timestamp}.\n${session_info}"
        )
        super().__init__(template=TEMPLATE, name="Runtime Environment", **data)


    @property_bag_item
    async def session_info(self, **opts) -> str:
        """
        Generates a string representation of the current chat session's information and metadata,
        excluding AI-internal metadata.

        Returns:
            str: Formatted session information.
        """
        chat_session: Optional['ChatSession'] = opts.get('chat_session', None)
        if chat_session is None:
            return ""

        session_name = chat_session.session_name or "ALERT! This session has no name!  Use `bridge_set_session_name` to set a name once you know the topic of the session."

        session_data = f"Session ID: {chat_session.session_id}\nSession Name: {session_name}\n"
        user_data = f"User Name: {chat_session.user_name}\n"

        return  f"### Session Info\n{session_data}\n{user_data}"


    @property_bag_item
    async def timestamp(self) -> str:
        """
        Retrieves the current local timestamp formatted according to the OS platform.

        Returns:
            str: Formatted timestamp.

        Raises:
            Logs an error if formatting the timestamp fails, returning an error message.
        """
        try:
            local_time_with_tz = datetime.datetime.now(datetime.timezone.utc).astimezone()
            if platform.system() == "Windows":
                formatted_timestamp = local_time_with_tz.strftime('%A %B %#d, %Y %#I:%M%p (%Z %z)')
            else:
                formatted_timestamp = local_time_with_tz.strftime('%A %B %-d, %Y %-I:%M%p (%Z %z)')
            return formatted_timestamp
        except Exception:
            self._logger.error("An error occurred when formatting the timestamp.", exc_info=True)
            return 'Warn the user that there was an error formatting the timestamp.'

    @property_bag_item
    async def env_name(self) -> str:
        """
        Retrieves the current environment name and its associated behavior rules.

        Returns:
            str: Formatted environment name and its rules.
        """
        env = os.environ.get("ENVIRONMENT", "PRODUCTION").upper()
        inst = self.env_rules.get(env, '') if self.env_rules is not None else ""

        return f"{env}. {inst}"
