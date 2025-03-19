import re

from readability import Document
from markdownify import markdownify

from agent_c_tools.tools.web.formatters.base import ContentFormatter


class CentricBlogFormatter(ContentFormatter):
    """
    A Formatter specific to 'centricconsulting.com/blog' that adds additional information before the content.
    Inherits from ContentFormatter.

    Uses the readability Document for main content and regex to extract meta information such as the author and post date.
    """

    def __init__(self):
        super().__init__(re.compile(r".*centricconsulting\.com/blog"))

    def format(self, content: str, url: str) -> str:
        """
        Override format method to add blog post specific information.

        Args:
            content (str): Original HTML content.
            url (str): URL of the blog post.

        Returns:
            str: Enhanced blog post content in Markdown format, along with meta data.
        """
        doc = Document(content)
        summary = markdownify(doc.summary(), heading_style='ATX', escape_asterisks=False, escape_underscores=False)

        author_pattern = re.compile(r'author-meta__name">By (.*?)</')
        a_matches = author_pattern.findall(content)

        date_pattern = re.compile(r'<div class="post-meta__date">(.*?)</')
        d_matches = date_pattern.findall(content)

        markdown = f"# Blog Post URL: {url}\n# Blog Post Title: {doc.title()}\n"

        if a_matches:
            markdown += f"# Blog Post Author: {a_matches[0]}\n"
        if d_matches:
            markdown += f"# Blog Post Date: {d_matches[0]}\n***\n"

        return f"{markdown}{summary}"
