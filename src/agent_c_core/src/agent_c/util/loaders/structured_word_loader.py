import os
import tempfile
import argparse
from typing import List
from langchain_community.document_loaders import UnstructuredFileLoader
from unstructured.partition.common.common import  convert_office_doc
from agent_c.util.loaders.centric_docx_old import partition_docx

class StructuredWordDocumentLoader(UnstructuredFileLoader):
    """Loader that uses unstructured to load word documents while preserving strcuture."""

    def _get_elements(self) -> List:
        _, extension = os.path.splitext(str(self.file_path))
        is_doc = extension == ".doc"

        if is_doc:
            _, filename_no_path = os.path.split(os.path.abspath(self.file_path))
            base_filename, _ = os.path.splitext(filename_no_path)
            if not os.path.exists(self.file_path):
                raise ValueError(f"The file {self.file_path} does not exist.")

            with tempfile.TemporaryDirectory() as tmpdir:
                convert_office_doc(self.file_path, tmpdir, target_format="docx", target_filter="MS Word 2007 XML", )
                docx_filename = os.path.join(tmpdir, f"{base_filename}.docx")
                return partition_docx(filename=docx_filename, **self.unstructured_kwargs)

        else:
            return partition_docx(filename=self.file_path, **self.unstructured_kwargs)



def main():
    parser = argparse.ArgumentParser(description='Prepare a word document for vectorization')
    parser.add_argument('filename', help='The filename of the document to load.')

    args = parser.parse_args()

    loader = StructuredWordDocumentLoader(args.filename,  mode="elements")
    data = loader.load()


    filename_without_extension, _ = os.path.splitext(args.filename)
    new_filename = filename_without_extension + '.md'

    with open(new_filename, 'w') as f:
        for element in data:
            print(element.page_content)
            f.write(element.page_content)
            f.write('\n')




if __name__ == '__main__':
    main()
