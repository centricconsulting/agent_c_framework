import os
import re
import sys
import json
import base64

from typing import cast, Literal
from openai import AsyncOpenAI
from openai.types import ImagesResponse

from agent_c.models.chat_event import RenderMedia
from agent_c.toolsets import json_schema, Toolset
from agent_c_tools.tools.workspaces.tool import WorkspaceTools
from agent_c_tools.tools.workspaces.local_storage import LocalStorageWorkspace
from agent_c_tools.tools.dall_e.prompt import DallESection


class DallETools(Toolset):

    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='dalle', tool_role="DALL-E 3")
        self.openai_client: AsyncOpenAI = kwargs.get('openai_client', AsyncOpenAI())
        self.section = DallESection()
        self.username = kwargs.get('username', None)
        self.workspace = kwargs.get('dalle_workspace', None)
        if self.workspace is None:
            self.workspace_path = kwargs.get('dalle_workspace_path', os.environ.get('DALLE_IMAGE_SAVE_FOLDER', None))
            if self.workspace_path is not None:
                self.workspace_tool: WorkspaceTools = cast(WorkspaceTools, self.tool_chest.active_tools.get('workspace'))
                if self.workspace_tool is not None:
                    self.workspace = LocalStorageWorkspace(workspace_path=self.workspace_path, max_size=sys.maxsize)
                    self.workspace_tool.add_workspace(self.workspace)

        if self.session_manager is not None and self.session_manager.user is not None:
            self.valid = True

    @json_schema(
        'Call this to get an image from DALL-E-3 based on a prompt.',
        {
            'prompt': {
                'type': 'string',
                'description': 'The prompt to generate an image from, e.g. "a cat in a forest"',
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
            }
        }
    )
    async def create_image(self, **kwargs):
        session_id = self.session_manager.chat_session.session_id

        prompt = kwargs.get('prompt')
        quality: Literal['hd', 'standard'] = 'standard' if kwargs.get('quality', 'standard') == 'standard' else 'hd'
        ratio = kwargs.get('ratio', 'square')
        style: Literal['vivid', 'natural'] = 'vivid' if kwargs.get('style', 'vivid') == 'vivid' else 'natural'

        size: Literal['1024x1024', '1024x1792', '1792x1024'] = '1024x1024'

        if ratio == 'wide':
            size = '1792x1024'
        elif ratio == 'tall':
            size = '1024x1792'

        response_format: Literal['url', 'b64_json'] = "url"

        if self.workspace is not None:
            response_format = "b64_json"

        await self.chat_callback(content=f"### Generating image from prompt:\n> {prompt}\n\n", session_id=session_id)

        if self.session_manager is not None:
            user = self.session_manager.user.user_id
        else:
            user = self.username

        try:
            response: ImagesResponse = await self.openai_client.images.generate(prompt=prompt, size=size, quality=quality, style=style,
                                                                                model='dall-e-3', user=user,
                                                                                response_format=response_format)
        except Exception as e:
            return str(e)

        revised_prompt = ''

        if self.workspace is None:
            url: str = response.data[0].url
            await self.chat_callback(content=f"\nGenerated Image: \"{url}\"\n", session_id=session_id)
            await self.chat_callback(render_media={"content-type": "image/png", "url": url}, session_id=session_id)
        else:
            await self.handle_base64_response(response, prompt, quality, ratio, style, size, session_id)

        if len(response.data[0].revised_prompt) > 0:
            revised_prompt = f"\n### Notice DALL-E-3 revised your prompt to:\n> {response.data[0].revised_prompt}"
            await self.chat_callback(content=f"\n{revised_prompt}\n\n", session_id=session_id)

        return f"Image generated, the client app has displayed it. Ask the user if they'd like to make any changes.{revised_prompt}"

    async def handle_base64_response(self, response: ImagesResponse, prompt: str, quality: str, ratio: str, style: str, size: str, session_id: str = 'None'):
        base64_json: str = response.data[0].b64_json
        image_bytes: bytes = base64.b64decode(base64_json)
        # Replace spaces with underscores
        file_name = prompt.replace(' ', '_')[:200]

        # Remove disallowed characters
        file_name = re.sub(r'[<>:"/\\|?*]', '', file_name)
        file_name_test = file_name
        counter = 1
        while await self.workspace.path_exists(file_name_test + '.png'):
            file_name_test = file_name + f'_{counter}'
            counter += 1

        image_file_name = file_name_test + '.png'
        await self.workspace.write_bytes(image_file_name, 'write', image_bytes)
        fp = self.workspace.full_path(image_file_name)
        await self.chat_callback(render_media={"content-type": "image/png", "url": f"file://{fp}",
                                               "name": image_file_name, "content_bytes": image_bytes, "content": base64_json},
                                 session_id=session_id)

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
        json_file_name = file_name_test + '.json'
        await self.workspace.write(json_file_name, 'write', json.dumps(metadata, indent=4))


Toolset.register(DallETools)
