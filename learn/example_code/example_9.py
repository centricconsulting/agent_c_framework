import asyncio
from dotenv import load_dotenv
load_dotenv(override=True)

from agent_c_core.agent_c import GPTChatAgent, ToolChest
from agent_c_tools.tools.weather import WeatherTools  # noqa

async def main():
    tool_chest = ToolChest()
    await tool_chest.init_tools()

    agent = GPTChatAgent(tool_chest=tool_chest)

    prompt: str = ("If the user message contains only the name of a location. "
                   "Use the weather_get_current_weather tool to get the weather forecast for that location "
                   "and report it to the user.")

    location: str = "Columbus, Ohio"
    model_names = ["gpt-3.5-turbo", "gpt-4-turbo-preview"]

    for model_name in model_names:
        result: str = await agent.one_shot(prompt=prompt, user_message=location, model_name=model_name)
        print(f"\n\nModel: {model_name}")
        print(result)

if __name__ == "__main__":
    asyncio.run(main())
