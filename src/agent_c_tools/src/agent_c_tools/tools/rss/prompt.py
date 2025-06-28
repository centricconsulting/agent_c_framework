from agent_c.prompting import OldPromptSection, property_bag_item

class RSSSection(OldPromptSection):
    feeds: str

    def __init__(self, **data):
        TEMPLATE = ("The RSS tool provides access multiple RSS feeds via a feed ID.  The IDs for the avaliale feed as well as their "
                    "descriptions are listed below. You can use the web tool to fetch the full article if the user requests it.\n"
                    "The feed IDs are \"snake case\" versions of the English feed names.  When describing feeds to the user: convert the ID back into English.\n"
                    "### Available feeds.\n"
                    "${feed_list}\n")
        super().__init__(template=TEMPLATE, required=True, name="RSS Feeds", render_section_header=True, **data)

    @property_bag_item
    async def feed_list(self):
        return self.feeds
