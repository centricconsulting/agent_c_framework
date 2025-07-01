import io
import os
import mammoth
import pandas as pd

from typing import List, Union

from agent_c.util.loaders.pdf_form_loader import PDFFormLoader


class SimpleDocumentLoader:
    """
    A class for loading documents of various formats into a list of strings or a single string.
    This is distinct from the DocumentLoad that produce a list of Elements, indented for loading documents to
    be stored in a vector store.

    This class is intended cases where you just want to load the text from a document, without any further processing.
    for example if you're going to summarize it.

    This will eventually be a subclass of DocumentLoader, that just rolls up the Elements as text but I'm leaving it
    intact for now, as the way it loads PDFs is different from the way the DocumentLoader does it and I need to make
    sure the DocumentLoader way will work with the tabular extraction process.

    """
    extension_to_loader_map = {
        '.pdf': 'pdf',  # Probably need another custom loader here
        '.csv': 'text',
        '.txt': 'text',
        '.xls': 'excel',
        '.xlsx': 'excel',
        '.docx': 'docx',
        '.xml': 'unstructured',
    }

    @staticmethod
    def load_pdf(file_path_or_bytes: Union[str, io.BytesIO], **kwargs) -> Union[str, List[str]]:
        stop_regexes = [r"^\*\*APPLICABLE\s+IN ", r"^APPLICABLE\s+IN "]
        form_loader = PDFFormLoader(stop_regexes=stop_regexes)

        single_string: bool = kwargs.get("single_string", False)

        try:
            pages = form_loader.extract_content_pages(file_path_or_bytes)

            if single_string:
                return "\n".join(["\n".join(page) for page in pages])

            return pages
        except Exception as e:
            return f"An error occurred during text extraction: {e}"

    @staticmethod
    def extract_images_from_pdf(pdf_path_or_file, base_name: str = None):
        doc_images = []

        # TODO: Fix this to work with pdfplumber
        # if isinstance(pdf_path_or_file, str):
        #     doc = fitz.open(pdf_path_or_file)
        # else:
        #     doc = fitz.open(stream=pdf_path_or_file, filename=base_name)
        #
        # for page_num in range(doc.page_count):
        #     page = doc[page_num]
        #     images = page.get_images()
        #     page_images = []
        #
        #     for image_num, img in enumerate(images, start=1):
        #         if base_name is not None:
        #             image_filename = f"{base_name}_p{page_num}_i{image_num}.png"
        #         else:
        #             image_filename = f"image_p{page_num}_i{image_num}.png"
        #         try:
        #
        #             xref = img[0]
        #             pix = fitz.Pixmap(doc, xref)
        #             png_bytes = pix.tobytes("png")
        #
        #
        #             image_base64 = base64.b64encode(png_bytes).decode('utf-8')
        #
        #             page_images.append({
        #                 'filename': image_filename,
        #                 'contents': image_base64
        #             })
        #         except Exception as e:
        #             print(f"Error extracting image: {e}")
        #             logging.error(traceback.format_exc())
        #             continue
        #
        #     doc_images.append(page_images)
        #
        # doc.close()

        return doc_images

    @staticmethod
    def load_text(file_path: str, **kwargs) -> Union[str, List[List[str]]]:
        single_string: bool = kwargs.get("single_string", False)
        try:
            with open(file_path, "r") as file:
                lines: List[str] = file.readlines()
                if single_string:
                    return "\n".join(lines)

                return [lines]
        except Exception as e:
            return f"An error occurred during text extraction for {file_path}: {e}"

    @staticmethod
    def load_docx(file_like, **kwargs):
        style_map = """
                       p[style-name='header'] => h2:fresh
                       p[style-name='Header'] => h1:fresh
                       """

        def convert_image(image):
            return {
                "src": ""
            }

        return mammoth.convert_to_markdown(file_like,
                                           convert_image=mammoth.images.img_element(convert_image),
                                           style_map=style_map)

    @staticmethod
    def load_excel(file_path: str, **kwargs) -> Union[str, List[List[str]]]:
        single_string: bool = kwargs.get("single_string", False)
        try:
            xls = pd.ExcelFile(file_path)
            sheets: List[List[str]] = []

            # Iterate through each sheet
            for sheet_name in xls.sheet_names:
                # Read the sheet into a DataFrame
                df = pd.read_excel(xls, sheet_name)

                # Convert the DataFrame to a CSV string, then split into lines
                df_string = df.to_csv(index=False, header=True)
                csv_string = f"Sheet Name: {sheet_name}\n{df_string}"
                lines = csv_string.split('\n')

                # Assign the array of lines to the corresponding sheet name
                sheets.append(lines)

            if single_string:
                return "\n".join(["\n".join(sheet) for sheet in sheets])

            return sheets

        except Exception as e:
            return f"An error occurred during text extraction for {file_path}: {e}"

    def __loader_for(self, extension: str):
        loader_name = self.extension_to_loader_map.get(extension)

        if loader_name is not None:
            return getattr(self, f"load_{loader_name.lower()}", None)

        return None

    def load_document(self, file_path_or_bytes: Union[str, io.BytesIO], **kwargs) -> Union[str, List[str]]:

        if isinstance(file_path_or_bytes, io.BytesIO):
            _, extension = os.path.splitext(kwargs.get("file_name"))
        else:
            kwargs["file_name"] = kwargs.get("file_name", file_path_or_bytes)
            _, extension = os.path.splitext(str(file_path_or_bytes))

        loader = self.__loader_for(extension)

        if not loader:
            raise Exception(f"No loader found for extension {extension}")

        results = loader(file_path_or_bytes, **kwargs)

        return results
