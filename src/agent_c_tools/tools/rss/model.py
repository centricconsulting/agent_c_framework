from typing import List
from pydantic import BaseModel


class RSSToolFeed(BaseModel):
    """
    This class represents an RSS feed configuration for the RSS tool.

    Attributes:
        id (str): The unique identifier for the RSS feed.
        url (str): The URL of the RSS feed.
        fields_wanted (List[str]): A list of fields that are desired from the RSS feed.
        desc (str): A description of the RSS feed.
    """

    id: str
    url: str
    fields_wanted: List[str]
    desc: str

    def __str__(self) -> str:
        """
        String representation of the RSSToolFeed instance.

        Returns:
            str: A formatted string with the feed's ID and description.
        """
        return f"- `{self.id}` - {self.desc}"
