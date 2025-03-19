from re import Pattern

from markdownify import markdownify


class ContentFormatter:
    """
    Basic formatter class that converts entire page content to Markdown.

    Attributes:
        pattern (Pattern[str]): A compiled regular expression pattern to match URLs.
    """

    def __init__(self, pattern: Pattern[str]):
        self.pattern: Pattern[str] = pattern

    def match(self, url: str) -> bool:
        """
        Check if the given URL matches the pattern.

        Args:
            url (str): URL to be matched.

        Returns:
            bool: True if the pattern matches the URL, False otherwise.
        """
        return self.pattern.search(url) is not None

    def format(self, content: str, url: str) -> str:
        """
        Format the given content to Markdown.

        Args:
            content (str): Content to be formatted.
            url (str): Source URL of the content.

        Returns:
            str: Formatted content in Markdown.
        """
        return markdownify(content, heading_style='ATX', escape_asterisks=False, escape_underscores=False)
