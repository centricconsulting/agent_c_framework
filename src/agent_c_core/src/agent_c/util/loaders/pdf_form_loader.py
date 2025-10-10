import re
import pdfplumber
from typing import List, Union


class PDFFormLoader:
    """
    A class to load and process PDF forms, allowing for text extraction with specific filtering based on regular expressions.
    """
    def __init__(self, **kwargs):
        """
        Initializes the PDFFormLoader with optional regex patterns and density settings for text extraction.

        Args:
            stop_regexes (list): A list of regex patterns to stop processing when matched.
            skip_regexes (list): A list of regex patterns to skip lines when matched.
            x_density (float): The horizontal density of text extraction.
            y_density (float): The vertical density of text extraction.
        """
        stop_regexes = kwargs.get("stop_regexes", [])
        skip_regexes = kwargs.get("skip_regexes", [])
        self.x_density = float(kwargs.get("x_density", 6.5))
        self.y_density = float(kwargs.get("y_density", 7))
        self.stop_regexes = [re.compile(regex) for regex in stop_regexes]
        self.skip_regexes = [re.compile(regex) for regex in skip_regexes]

    def extract_content_pages(self, path_or_file_like: Union[str, bytes], stop_on_first_match: bool = False) -> List[str]:
        """
        Extracts content from the given PDF file or path, applying regex filters.

        Args:
            path_or_file_like (str or bytes): The PDF file path or bytes object.
            stop_on_first_match (bool): Whether to stop processing when the first stop regex is matched.

        Returns:
            List[str]: A list of processed text from each page.
        """
        content_pages: List[str] = []
        cur_page_lines = []

        try:
            with pdfplumber.open(path_or_file_like) as pdf:
                for page in pdf.pages:
                    try:
                        text = page.extract_text(layout=True, x_density=self.x_density, y_density=self.y_density)
                        if text is None:
                            continue
                        for line in text.splitlines():
                            search_line = line.strip()
                            if any(regex.search(search_line) for regex in self.stop_regexes):
                                if len(cur_page_lines) > 0:
                                    page_text = "\n".join(cur_page_lines)
                                    content_pages.append(re.subn(r"\n{3,}", "\n\n", page_text)[0])
                                if stop_on_first_match:
                                    return content_pages
                                cur_page_lines = []
                                continue
                            if any(regex.search(search_line) for regex in self.skip_regexes):
                                continue
                            if len(line.strip()) == 0:
                                cur_page_lines.append("")
                            else:
                                cur_page_lines.append(line.rstrip(' '))
                        page_text = "\n".join(cur_page_lines)
                        content_pages.append(re.subn(r"\n{3,}", "\n\n", page_text)[0])
                        cur_page_lines = []
                    except Exception as e:
                        print(f"Error processing page: {e}")
        except Exception as e:
            print(f"Error opening or reading PDF file: {e}")
        return content_pages