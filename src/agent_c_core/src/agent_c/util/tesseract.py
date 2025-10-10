import pytesseract
from PIL import Image

from typing import Union


def extract_text_from_image(image) -> Union[str, None]:
    text = None
    try:
        text = pytesseract.image_to_string(image)
    except Exception as e:
        pass

    return text

def extract_text_from_image_file(path_or_file_like) -> Union[str, None]:
    image = Image.open(path_or_file_like)
    return extract_text_from_image(image)



