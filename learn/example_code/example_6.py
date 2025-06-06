import time
import asyncio
from dotenv import load_dotenv
load_dotenv(override=True)

from agent_c import GPTChatAgent

def segment_blog_posts(content):
    posts = []
    current_post = []

    for line in content.split('\n'):
        if line.startswith('# Blog post title:'):
            if current_post:
                posts.append('\n'.join(current_post))
                current_post = []
            current_post.append(line)
        else:
            current_post.append(line)

    if current_post:
        posts.append('\n'.join(current_post))

    return posts


async def main():
    # Load and segment the blog posts
    file_name: str = 'learn/sample_data/blog_posts.md'
    with open(file_name, 'r', encoding='utf-8') as file:
        file_contents = file.read()

    blog_posts = segment_blog_posts(file_contents)

    agent = GPTChatAgent(concurrency_limit=15)
    model_name: str = "gpt-4-turbo-preview"
    prompt: str = ("The user message contains a blog post exported to Markdown format "
                   "Produce a concise summary of the blog post using no more than a paragraph.")

    results = await agent.parallel_one_shots(blog_posts, model_name=model_name, prompt=prompt)

    for index, result in enumerate(results):
        print(f"# Post {index + 1} summary:\n{result}\n\n")


if __name__ == "__main__":
    start_time = time.time()

    asyncio.run(main())

    end_time = time.time()
    elapsed_time = end_time - start_time

    print(f"Main function took {elapsed_time:.6f} seconds to complete.")
