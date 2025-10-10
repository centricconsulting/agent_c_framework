import os
import logging
import traceback

from typing import List, Optional
from langchain.docstore.document import Document
from langchain_community.document_loaders import EverNoteLoader, UnstructuredFileLoader, UnstructuredODTLoader, \
    UnstructuredExcelLoader

from agent_c.util.loaders.structured_word_loader import StructuredWordDocumentLoader
from agent_c.util.loaders.pdf_form_loader import PDFFormLoader

class DocumentLoader:
    """
    Helper class to call the appropriate element loader for a given file extension.

    Most file extensions get loaded via the LangChain UnstructuredFileLoader class.

    Word documents and PowerPoint files each have their own loaders that address flaws
    in the LangChain and other text conversion tools for these formats.

    Each loader returns a list of objects, e.g. list[Document] or list[Element]. They should not be viewed
    as documents, but rather a list of components that make up a document.

    For example, PDFs return a list of pages.
    Word documents return a list of elements, e.g. headers, tables, pages, page breaks, paragraphs, etc.

    """

    # define the extension-loader mapping as a class attribute
    extension_to_loader_map = {
        '.csv': 'unstructured',
        '.doc': 'word_doc',
        '.docx': 'word_doc',
        '.eml': 'unstructured',
        '.enex': 'evernote',
        '.html': 'unstructured',
        '.json': 'unstructured',
        '.md': 'unstructured',
        '.odt': 'open_document',
        '.pdf': 'pdf',
        #'.pptx': 'power_point',
        '.rtf': 'unstructured',
        '.tsv': 'unstructured',
        '.txt': 'unstructured',
        '.xls': 'excel',
        '.xlsx': 'excel',
        '.xml': 'unstructured',
    }

    @staticmethod
    def load_word_doc(file_path: str) -> List[Document]:
        loader = StructuredWordDocumentLoader(file_path, mode='elements')
        try:

            return loader.load()
        except Exception as ex:
            logging.error(f"Exception loading {file_path}: {traceback.format_exc()}")

    @staticmethod
    def load_pdf(file_path: str, skip_regexes: Optional[List[str]] = None) -> List[Document]:

        form_loader = PDFFormLoader(skip_regexes=skip_regexes or []) # Use empty list if None
        pages: List[str] = form_loader.extract_content_pages(file_path)
        docs: List[Document] = list()
        for page in pages:
            parts = page.split("\n\n")
            for part in parts:
                type = "Text"
                if all(char.isupper() for char in part if char.isalpha()):
                    type = "Heading"

                docs.append(Document(page_content=part, metadata={"category": type}))


        return docs

    @staticmethod
    def load_evernote(file_path: str) -> List[Document]:
        loader = EverNoteLoader(file_path, load_single_document=False)
        return loader.load()

    @staticmethod
    def load_open_document(file_path: str) -> List[Document]:
        loader = UnstructuredODTLoader(file_path, load_single_document=False)
        return loader.load()

    # @staticmethod
    # def load_power_point(file_path: str) -> List[Document]:
    #     elements = load_pptx(file_path)
    #     docs: List[Document] = list()
    #     for element in elements:
    #         docs.append(Document(page_content=str(element), metadata=element.metadata.to_dict()))
    #
    #     return docs

    @staticmethod
    def load_unstructured(file_path: str) -> List[Document]:
        loader = UnstructuredFileLoader(file_path, mode='elements')
        return loader.load()

    @staticmethod
    def load_excel(file_path: str, worksheets: Optional[List[str]] = None) -> List[Document]:
        """
        Load Excel files using the Unstructured library.

        Args:
            file_path: Path to the Excel file
            worksheets: optional list of worksheet names to load, defaults to all when empty

        Returns:
            List of Document objects
        """
        loader = UnstructuredExcelLoader(file_path, mode='elements')
        return loader.load()

    def load_document(self, file_path: str, source: str = None, skip_regexes: Optional[List[str]] = None) -> List[Document]:
        if source is None:
            source = file_path

        _, extension = os.path.splitext(str(file_path))

        loader = self.__loader_for(extension)

        if not loader:
            raise Exception(f"No loader found for extension {extension}")

        if loader == self.load_pdf:
            results = loader(file_path, skip_regexes=skip_regexes)
        else:
            results = loader(file_path)

        for result in results:
            result.metadata["source"] = source

        return results

    def __loader_for(self, extension: str):
        loader_name = self.extension_to_loader_map.get(extension)

        if loader_name is not None:
            return getattr(self, f"load_{loader_name.lower()}", None)

        return None