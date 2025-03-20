import os
import asyncio
from dotenv import load_dotenv
load_dotenv(override=True)

from agent_c_core.agent_c.agents.gpt import GPTChatAgent
from agent_c_tools.tools.workspaces.local_storage import LocalStorageWorkspace
from agent_c_core.agent_c.models.input.image_input import ImageInput


async def main():
    agent = GPTChatAgent(model_name="gpt-4o")

    path = os.path.join(os.getcwd(), 'learn/sample_data')
    lab_workspace: LocalStorageWorkspace = LocalStorageWorkspace(name="data", workspace_path=path)

    images = [await lab_workspace.read_bytes_base64(f"image_{n}.jpg") for n in range(1, 4)]
    inputs = [ImageInput(content_type="image/jpeg",  content=image) for image in images]

    for index, image in enumerate(inputs):
        print(f"Image {index+1}:", end=" ")
        result: str = await agent.one_shot(user_message="Describe this image", images=inputs)
        print(result)


if __name__ == "__main__":
    asyncio.run(main())
