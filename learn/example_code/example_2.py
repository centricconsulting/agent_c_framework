import asyncio
from dotenv import load_dotenv
load_dotenv(override=True)

from agent_c_core.agent_c import GPTChatAgent


async def main():
    agent = GPTChatAgent()

    temperature: float = 0.5
    model_name: str = "gpt-3.5-turbo"
    user_message: str = "What is the capital of France?"
    prompt: str = "You always respond as if you're annoyed"

    result: str = await agent.one_shot(model_name=model_name,
                                       user_message=user_message,
                                       prompt=prompt,
                                       temperature=temperature)

    print(result)


if __name__ == "__main__":
    asyncio.run(main())
