from markdownify import markdownify
from readability import Document

from agent_c_tools.tools.web.formatters.base import ContentFormatter


class ReadableFormatter(ContentFormatter):
    """
    A Formatter that uses the readability algorithm to extract the main content.
    Inherits from ContentFormatter.
    """

    def format(self, content: str, url: str) -> str:
        """
        Override format method to use readability to extract main content.

        Args:
            content (str): Original HTML content.
            url (str): URL of the content.

        Returns:
            str: Main content formatted in Markdown.
        """
        doc = Document(content)
        return markdownify(doc.summary(), heading_style='ATX', escape_asterisks=False, escape_underscores=False)
