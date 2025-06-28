from typing import Any

from agent_c.prompting.prompt_section import OldPromptSection


class TavilyResearchPrompt(OldPromptSection):
    def __init__(self, **data: Any):
        template = ("The search_tavily tool is used to perform research using Tavily's Large Language Model search retrieval approach.  "
                    "When using this tool to search for information, always include the search query. "
                    "The response will be a list of results that include the URL, the body of the content, and a score. "
                    "The score is a measure of the relevance of the content to the search query. The higher the better. \n"
                    "Exclude scores that are less than 0.75.\n"
                    "### Important Note\n"
                    "Always include the URL source in the response. \n "
                    "Always include score in the response.\n"
                    "The tool response will be a json string")
        super().__init__(template=template, required=True, name="Tavily Search", render_section_header=True, **data)
