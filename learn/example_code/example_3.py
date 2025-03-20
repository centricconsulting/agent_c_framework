import asyncio
from dotenv import load_dotenv
load_dotenv(override=True)

from agent_c_core.agent_c.agents.gpt import TikTokenTokenCounter


async def main():
    token_counter = TikTokenTokenCounter()
    file_name: str = 'learn/sample_data/blog_posts.md'
    file_text: str = ""

    with open(file_name, 'r', encoding='utf-8') as file:
        file_text = file.read()

    token_count = token_counter.count_tokens(file_text)
    print(f"Token count: {token_count}")


if __name__ == "__main__":
    asyncio.run(main())
