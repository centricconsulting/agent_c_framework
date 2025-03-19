from typing import Any

from agent_c_core.agent_c.prompting.prompt_section import PromptSection


class DallESection(PromptSection):
    def __init__(self, **data: Any):
        TEMPLATE = ("The dalle_create_image tool allows you to generate an image based on a prompt using DALL-E-3 .\n"
                    "- If the user requests a portrait make sure to specify a tall ratio.\n"
                    "- If the user requests a landscape, photo or cinematic image make sure to specify a wide ratio.\n"
                    "\n\nDetails are important, the more detailed the prompt the better the image will be. Work with the user to help refine their image prompt.\n"
                    "- For example, if the user wants a cat in a forest, suggest they specify the type of cat and the type of forest after their image generates.\n"
                    "- Images meant to emulate photos can benefit from appropriate camera terms being adding to the prompt such as the camera make, lens information etc. "
                    "If the user does not specify the camera / lens, adjust their prompt to add them.\n"
                    "  - If the user requests street photography, but does not otherwise specify, include Hasselblad as the camera in the prompt.\n"
                    "\n\nTips:\n"
                    "- If the user requests an HD images, make sure to specify the quality as hd.  Do not include it in the prompt.\n")

        super().__init__(template=TEMPLATE, required=False, name="DALL-E-3", render_section_header=True, **data)

