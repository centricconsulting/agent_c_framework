import os
from openai import AsyncOpenAI
from agent_c.agent_runtimes.gpt import GPTChatAgentRuntime

class GeminiChatAgent(GPTChatAgentRuntime):
    @classmethod
    def client(cls, **opts):
        return AsyncOpenAI(api_key= os.environ.get("GEMINI_API_KEY"),
                           base_url="https://generativelanguage.googleapis.com/v1beta/openai/")
