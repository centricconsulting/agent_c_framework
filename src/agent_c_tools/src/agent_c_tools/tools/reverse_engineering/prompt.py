from typing import Any, Optional

from agent_c.prompting.prompt_section import OldPromptSection, property_bag_item


class RevEngSection(OldPromptSection):

    def __init__(self, **data: Any):
        TEMPLATE = ("The Reverse Engineering Tools are designed to assist you in analyzing and understanding codebases. \n"
                    "- **analyze_source**: Perform an in-depth analysis of all code files matching a glob pattern. \n"
                    "  - This will perform a two-pass analysis of the code files matching a glob pattern\n"
                    "  - This tool will provide extremely detailed analysis of the code files as well as reverse engineer requirements for each file.\n"
                    "- **query_analysis**: Make a request of an expert on the analysis of the codebase in a given workspace. \n"
                    "  - The expert will be able to answer questions about the codebase and/or the analysis done on it.\n"
                    "  - The expert will need the output of `analyze_source` to be in the workspace.\n")
        super().__init__(template=TEMPLATE, required=True, name="Reverse Engineering", render_section_header=True, **data)

