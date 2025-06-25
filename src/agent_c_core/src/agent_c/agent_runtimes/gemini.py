import os
from pydantic import Field

from openai import AsyncOpenAI
from agent_c.agent_runtimes.gpt import GPTChatAgentRuntime
from agent_c.models.completion.gemini_auth_info import GeminiAuthInfo
from agent_c.models.config import BaseRuntimeConfig
from agent_c.util.logging_utils import LoggingManager

class GeminiConfig(BaseRuntimeConfig):
    auth: GeminiAuthInfo = Field(default_factory=lambda: GeminiAuthInfo(api_key=os.environ.get("GEMINI_API_KEY", None)),
                                 description="Authentication information for Gemini API, including API key and base URL.")


class GeminiChatAgent(GPTChatAgentRuntime):
    config_type: str = "gemini"
    DEFAULT_BASE_URL = "https://generativelanguage.googleapis.com/v1beta/openai/"

    @classmethod
    def client(cls, context=None):
        if context and hasattr(context, 'chat_session'):
            auth = context.chat_session.agent_config.agent_params.auth
            api_key = auth.api_key if hasattr(auth, 'api_key') else os.environ.get("GEMINI_API_KEY")
            base_url = auth.base_url if hasattr(auth, 'base_url') else cls.DEFAULT_BASE_URL
        else:
            api_key = os.environ.get("GEMINI_API_KEY")
            base_url = cls.DEFAULT_BASE_URL

        if not api_key:
            logger = LoggingManager(cls.__name__).get_logger()
            logger.warning("Waring an attempt was made to create an Gemini API client without an API key.  Set GEMINI_API_KEY to use this runtime ")
            return None

        return AsyncOpenAI(api_key=api_key, base_url=base_url)

    @classmethod
    def can_create(cls, context=None) -> bool:
        server_key = os.environ.get("GEMINI_API_KEY")
        if context and hasattr(context, 'chat_session'):
            auth = context.chat_session.agent_config.agent_params.auth
            context_key = auth.api_key if hasattr(auth, 'api_key') else None
        else:
            context_key = None

        if not server_key and not context_key:
            return False

        return True