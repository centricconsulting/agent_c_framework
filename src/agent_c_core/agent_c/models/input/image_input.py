import os
import base64
import logging
import tempfile
from typing import Any

from agent_c_core.agent_c.models.input.file_input import FileInput


class ImageInput(FileInput):
    """
    A model class for handling image input with various formats and conversion methods.
    """
    @classmethod
    def from_pil_image(cls, pil_image: Any, filename: str = "image", **data) -> 'ImageInput':
        """
        Creates an ImageInput instance from a Pillow (PIL) image by converting it into base64 format.

        Args:
            pil_image: The Pillow image object to convert
            filename: The desired name of the file, which will be suffixed with '.png'

        Returns:
            ImageInput: An instance with the image's content in base64 format

        Raises:
            Exception: If the image conversion process fails
        """
        temp_file_path: str = ""
        try:
            with tempfile.NamedTemporaryFile(delete=False, suffix=f".{filename}.png") as temp_file:
                pil_image.save(temp_file, format="PNG")
                temp_file_path = temp_file.name

            logging.debug(f"Saved PIL image to temporary file: {temp_file_path}")

            with open(temp_file_path, 'rb') as file:
                image_bytes: bytes = file.read()

            base64_encoded_str: str = base64.b64encode(image_bytes).decode('utf-8')

            return cls(content_type="image/png", content=base64_encoded_str, file_name=f"{filename}.png", **data)

        except Exception as e:
            logging.error(f"Failed to convert PIL image to base64: {e}")
            raise
        finally:
            if os.path.exists(temp_file_path):
                os.remove(temp_file_path)

    @classmethod
    def from_page_image(cls, page_image: Any, filename: str = "image", **data) -> 'ImageInput':
        """
        Generates an ImageInput instance from a page image by leveraging the `from_pil_image` method.

        Args:
            page_image: A page image object that contains an 'original' attribute with the Pillow image
            filename: The desired name of the image file

        Returns:
            ImageInput: An instance populated from the given page image
        """
        return cls.from_pil_image(page_image.original, filename, **data)

    @property
    def data_url(self) -> str:
        """
        Converts the base64-encoded image content into a data URL.

        Returns:
            The data URL of the image, which can be directly used in HTML
        """
        return f"data:{self.content_type};base64,{self.content}"
