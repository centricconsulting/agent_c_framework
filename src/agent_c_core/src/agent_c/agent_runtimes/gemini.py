import os
from typing import Optional, Tuple

from pydantic import Field

from openai import AsyncOpenAI
from agent_c.util.logging_utils import LoggingManager
from agent_c.models.config import BaseRuntimeConfig
from agent_c.agent_runtimes.gpt import GPTChatAgentRuntime
from agent_c.models.completion.gemini_auth_info import GeminiAuthInfo
from agent_c.config.system_config_loader import SystemConfigurationLoader


class GeminiConfig(BaseRuntimeConfig):
    auth: GeminiAuthInfo = Field(default_factory=lambda: GeminiAuthInfo(api_key=os.environ.get("GEMINI_API_KEY", None)),
                                 description="Authentication information for Gemini API, including API key and base URL.")

class GeminiUserConfig(BaseRuntimeConfig):
    auth: GeminiAuthInfo = Field(default_factory=lambda: GeminiAuthInfo(),
                                 description="Authentication information for Gemini API, including API key and base URL.")

class GeminiChatAgent(GPTChatAgentRuntime):
    config_type: str = "gemini"

    @classmethod
    def _get_auth_from_context(cls, context = None) -> Tuple[Optional[str], Optional[str]]:
        api_key: Optional[str] = None
        base_url: Optional[str] = None

        if context is not None:
            if hasattr(context, 'chat_session'):
                if context.chat_session.agent_config.runtime_params.auth is not None:
                    api_key = context.chat_session.agent_config.runtime_params.auth.api_key
                    base_url = context.chat_session.agent_config.runtime_params.auth.base_url
                if not api_key:
                    api_key = context.chat_session.user.config.runtimes.gemini.auth.api_key
                    base_url = context.chat_session.user.config.runtimes.gemini.auth.base_url
            elif hasattr(context, 'config'):
                api_key = context.config.runtimes.gemini.auth.api_key
                base_url = context.config.runtimes.gemini.auth.base_url

        if not api_key:
            api_key = SystemConfigurationLoader.instance().config.runtimes.gemini.auth.api_key
            base_url = SystemConfigurationLoader.instance().config.runtimes.gemini.auth.base_url

        return api_key, base_url

    @classmethod
    def client(cls, context=None):
        api_key, base_url = cls._get_auth_from_context(context)

        if not api_key:
            logger = LoggingManager(cls.__name__).get_logger()
            logger.warning("Waring an attempt was made to create an Gemini API client without an API key.  Set GEMINI_API_KEY to use this runtime ")
            return None

        return AsyncOpenAI(api_key=api_key, base_url=base_url)

    @classmethod
    def can_create(cls, context=None) -> bool:
        api_key, base_url = cls._get_auth_from_context(context)

        if not api_key:
            return False

        return True
