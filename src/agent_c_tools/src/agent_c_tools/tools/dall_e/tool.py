import os
import re
import sys
import json
import base64
from pydantic import Field

from typing import cast, Literal, Optional, Tuple
from openai import AsyncOpenAI
from openai.types import ImagesResponse

from agent_c.models import BaseContext


from agent_c.models.config import BaseToolsetConfig
from agent_c.models.context.interaction_context import InteractionContext
from agent_c.toolsets import json_schema, Toolset
from agent_c.util.uncish_path import UNCishPath
from agent_c_tools.tools.workspace.base import BaseWorkspace

from agent_c_tools.tools.workspace.tool import WorkspaceTools
from agent_c_tools.tools.dall_e.prompt import DallESection

class DallEToolsConfig(BaseToolsetConfig):
    default_save_folder: Optional[str] = Field(None,
                                                description="A UNC workspace path to save images to. If not provided, images will only be displayed in the UI.")

class DallEToolsContext(BaseContext):
    default_save_folder: Optional[str] = Field(None,
                                                description="A UNC workspace path to save images to. If not provided, images will only be displayed in the UI.")



class DallETools(Toolset):
    """### Toolset for creating images from text using DALL-E 3.

    **Notice:** This toolset requires either  Open AI or Azure Open AI API keys to function.

    **Usage:**
    The only *required* parameter is `prompt`, which is the text description of the image you want to generate.
    - Open AI recommends using a detailed prompt to get the best results.
    - If your prompt is too short or vague, DALL-E 3 will create a new prompt based on your input, which may not match your expectations.

    In addition to the `prompt`, you can specify:
    - `quality`: The quality of the image, either `standard` (default) or `hd`. HD images have finer details and greater consistency.
    - `ratio`: The aspect ratio of the image, which can be `square` (default), `wide`, or `tall`.
    - `style`: The style of the image, which can be `vivid` (default) or `natural`. Vivid images are hyper-real and dramatic, while natural images are more subdued and realistic.
    """
    name: str = "DALL-E-3 Image Generation"

    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='dalle', tool_role="DALL-E 3")
        self.openai_client: AsyncOpenAI = kwargs.get('openai_client', AsyncOpenAI())
        self.section = DallESection()
        self.workspace_tool: Optional[WorkspaceTools] = None

    async def post_init(self):
        """Post-initialization to set up required tools.
          This method is called by the toolchest after the main init is finished
          to allow for async calls in tools that need them.
        """
        self.workspace_tool = cast(WorkspaceTools, self.tool_chest.available_tools.get('WorkspaceTools'))

    @json_schema(
        'Generate an image from a text prompt using DALL-E 3. User descriptive prompts for best results.',
        {
            'prompt': {
                'type': 'string',
                'description': 'A rich and detailed description of the image you want to generate. DALL-E 3 will create a new prompt based on your input if it is too short or vague."',
                'required': True
            },
            'quality': {
                'type': 'string',
                "enum": ["standard", "hd"],
                'description': 'Default: standard. The quality of the image that will be generated. hd creates images with finer details and greater consistency across the image. ',
                'required': False
            },
            'ratio': {
                'type': 'string',
                'enum': ['square', 'wide', 'tall'],
                'description': 'Default: square. The aspect ratio of the image.',
                'required': False
            },
            'style': {
                'type': 'string',
                'enum': ['vivid', 'natual'],
                'description': 'Default: vivid. Vivid causes the model to lean towards generating hyper-real and dramatic images. Natural causes the model to produce more natural, less hyper-real looking images.',
                'required': False
            },
            'save_as': {
                'type': 'string',
                'description': 'A UNC workspace path to save the image to. If not provided, the image will only be displayed in the UI.',
                'required': False
            }
        }
    )
    async def create_image(self, **kwargs):
        tool_context: InteractionContext = kwargs.get("context")
        session_id = tool_context.user_session_id

        prompt = kwargs.get('prompt')
        quality: Literal['hd', 'standard'] = 'standard' if kwargs.get('quality', 'standard') == 'standard' else 'hd'
        ratio = kwargs.get('ratio', 'square')
        style: Literal['vivid', 'natural'] = 'vivid' if kwargs.get('style', 'vivid') == 'vivid' else 'natural'
        size: Literal['1024x1024', '1024x1792', '1792x1024'] = '1024x1024'
        save_path: Optional[str] = kwargs.get('save_as', None)

        if ratio == 'wide':
            size = '1792x1024'
        elif ratio == 'tall':
            size = '1024x1792'

        response_format: Literal['url', 'b64_json'] = "url"

        # If we have a save path, we will request base64 JSON response format
        # Otherwise, we will just get the URL of the image.
        if save_path is not None:
            response_format = "b64_json"

        await self._render_media_markdown(tool_context,
                                          f"### Generating image from prompt:\n> {prompt}\n\n",
                                          "DALL-E-3 Image Generation")

        user: str = tool_context.chat_session.user_id

        try:
            response: ImagesResponse = await self.openai_client.images.generate(prompt=prompt, size=size, quality=quality, style=style,
                                                                                model='dall-e-3', user=user,
                                                                                response_format=response_format)
        except Exception as e:
            error_message = f"DALL-E-3 Image Generation Error: {str(e)}"
            self.logger.exception(error_message, exc_info=sys.exc_info())
            await self._render_media_markdown(tool_context,
                                              "Error generating image.  See log for details",
                                              "DALL-E-3 Image Generation")
            return error_message


        if len(response.data[0].revised_prompt) > 0:
            revised_prompt = f"### Notice DALL-E-3 revised your prompt to:\n> {response.data[0].revised_prompt}\n\n"
            await self._render_media_markdown(tool_context,
                                              revised_prompt,
                                              "DALL-E-3 Image Generation")
        else:
            revised_prompt = ''

        if save_path is None:
            url: str = response.data[0].url
            await self._raise_render_media(tool_context,content_type="image/png", url=url)
            return f"{revised_prompt}Image URL: {url}\nThe client app has displayed it."


        save_response = await self.handle_base64_response(tool_context, save_path, response, prompt, quality, ratio, style, size)


        return f"Image generated, the client app has displayed it. \n{save_response}\nAsk the user if they'd like to make any changes.{revised_prompt}"

    async def _get_workspace_and_file_name(self, tool_context: InteractionContext, starting_path: str, file_name: str) -> Tuple[Optional[BaseWorkspace], Optional[str]]:
        """Generate a unique file name by appending a counter if the file already exists."""
        unc_path: UNCishPath = UNCishPath(starting_path)

        workspace: BaseWorkspace = self.workspace_tool.find_workspace_by_name(unc_path.source, tool_context)
        if workspace is None:
            return None, None

        starting_path = starting_path.removesuffix('/')

        test_path: str = f"{starting_path}/{file_name}.png"

        counter = 1
        while await workspace.path_exists(test_path + '.png'):
            file_name = f"{file_name}_{counter}"
            test_path = f"{starting_path}/{file_name}.png"
            counter += 1

        return workspace, f"{starting_path}/{file_name}.png"

    async def handle_base64_response(self, tool_context: InteractionContext, save_path: str,  response: ImagesResponse, prompt: str, quality: str, ratio: str, style: str, size: str) -> str:
        base64_json: str = response.data[0].b64_json
        image_bytes: bytes = base64.b64decode(base64_json)
        base_name: str = f"{tool_context.chat_session.user.user_id}-{prompt.replace(' ', '_')[:200]}"

        # Remove disallowed characters
        file_name = re.sub(r'[<>:"/\\|?*]', '', base_name)
        await self._raise_render_media(tool_context, "image/png", name=file_name,
                                       content_bytes=image_bytes, content=base64_json)

        workspace, safe_filename = await self._get_workspace_and_file_name(tool_context, save_path, file_name)
        if workspace is None or safe_filename is None:
            await self._render_media_markdown(tool_context,
                                              "Error: Unable to find or create the workspace for saving the image.",
                                              "DALL-E-3 Image Generation")

            return "Error: Unable to find or create the workspace for saving the image."


        await workspace.write_bytes(safe_filename, 'write', image_bytes)

        metadata = {
            'prompt': prompt,
            'revised_prompt': response.data[0].revised_prompt,
            'b64_json': base64_json,
            'quality': quality,
            'ratio': ratio,
            'style': style,
            'size': size,
            'model': 'dall-e-3'
        }
        yaml_filename = safe_filename + '.yml'
        await workspace.write(yaml_filename, 'write', self._yaml_dump(metadata))

        return f"Image saved to {safe_filename}. \nMetadata saved to {yaml_filename}.\nY"


Toolset.register(DallETools, required_tools=['WorkspaceTools'])
