from typing import List,  Union

from PIL import Image
from agent_c.models.extraction.bounding_box_model import BoundingBoxModel
from agent_c.models.input.image_input import ImageInput


from pdfplumber.display import PageImage


class PDFimageHelper:
    @classmethod
    def stack_page_images(cls, page_images: List[PageImage]) -> Image.Image:
        """
        Stack a list of pdfplumber.page.PageImage objects vertically.

        Args:
            page_images (List): List of pdfplumber.page.PageImage objects.

        Returns:
            Image.Image: A combined image stacked vertically.
        """
        # Convert all PageImage objects to PIL images
        pil_images = [img.original for img in page_images]
        return cls.stack_pil_images(pil_images)

    @classmethod
    def stack_pil_images(cls, pil_images: List[Image.Image]) -> Image.Image:
        """
        Stack a list of PIL.Image.Image objects vertically.

        Args:
            pil_images (List): List of PIL.Image.Image objects.

        Returns:
            Image.Image: A combined image stacked vertically.
        """
        # Get dimensions of all images
        widths, heights = zip(*(img.size for img in pil_images))

        # Create a new blank image big enough to hold them all
        total_width = max(widths)
        total_height = sum(heights)

        combined_image = Image.new('RGB', (total_width, total_height))

        # Paste each image into the new combined image
        y_offset = 0
        for idx, img in enumerate(pil_images):
            combined_image.paste(img, (0, y_offset))
            y_offset += img.size[1]

        return combined_image

    @classmethod
    def image_input_for_region(cls, pdf_file, box: BoundingBoxModel, file_name: Union[str, None]) -> ImageInput:
        file_name = file_name or box.label
        return ImageInput.from_pil_image(cls.page_image_for_region(pdf_file, box), file_name)

    @classmethod
    def image_input_for_page(cls, pdf_file, page_no: int, file_name: Union[str, None]) -> ImageInput:
        file_name = file_name or f"page_{page_no}"
        return ImageInput.from_pil_image(pdf_file.pdf.pages[page_no].to_image(resolution=pdf_file.image_resolution).original, file_name)

    @classmethod
    def page_image_for_region(cls, pdf_file, box: BoundingBoxModel) -> PageImage:
        region = pdf_file.pdf.pages[box.page_no].crop(box.tuple_coords)
        return region.to_image(resolution=pdf_file.image_resolution)

    @classmethod
    def pil_image_for_region(cls, pdf_file, box: BoundingBoxModel) -> Image:
        return cls.page_image_for_region(pdf_file, box).original

    @classmethod
    def h_slice_image(cls, image: Image.Image, num_slices: int) -> list[Image]:
        """
        Slices the given Pillow image horizontally into `num_slices` equal parts.

        :param image: The input Pillow Image object.
        :param num_slices: The number of equal horizontal slices to create.
        :return: A list of Pillow Image objects representing the slices.
        :raises ValueError: If `num_slices` is less than 1 or greater than the image height.
        """
        if num_slices < 1:
            raise ValueError("Number of slices must be at least 1.")

        img_width, img_height = image.size

        if num_slices > img_height:
            raise ValueError("Number of slices cannot exceed the image height.")

        slice_height = img_height // num_slices
        slices = []

        try:
            for i in range(num_slices):
                upper = i * slice_height
                lower = (i + 1) * slice_height if i < num_slices - 1 else img_height
                cropped_image = image.crop((0, upper, img_width, lower))
                slices.append(cropped_image)
                logging.debug(f"Slice {i + 1}/{num_slices} created: Upper={upper}, Lower={lower}")
        except Exception as e:
            logging.error(f"Failed to slice image: {e}")
            raise

        return slices