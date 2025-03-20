import time
import asyncio
from dotenv import load_dotenv
load_dotenv(override=True)

from agent_c_core.agent_c import GPTChatAgent


async def main():
    file_name: str = 'learn/sample_data/blog_posts.md'

    with open(file_name, 'r', encoding='utf-8') as file:
        user_message = file.read()

    agent = GPTChatAgent()

    model_name: str = "gpt-4-turbo-preview"
    prompt: str = ("The user message contains several blog posts in Markdown format. "
                   "Each post starts with a Markdown header indicating the title, author, and url."
                   "Produce a summary of each of the blog post in Markdown format."
                   )

    result: str = await agent.one_shot(model_name=model_name, user_message=user_message, prompt=prompt)

    print(result)

if __name__ == "__main__":
    start_time = time.time()

    asyncio.run(main())

    end_time = time.time()
    elapsed_time = end_time - start_time

    print(f"Main function took {elapsed_time:.6f} seconds to complete.")
