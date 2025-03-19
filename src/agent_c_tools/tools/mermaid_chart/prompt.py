from typing import Any

from agent_c_core.agent_c.prompting.prompt_section import PromptSection


class MermaidChatSection(PromptSection):
    def __init__(self, **data: Any):
        TEMPLATE = ("The mermaid_chart toolset allows you to generate 'Mermaid.js' charts\n"
                    "- Use the mermaid_chart-render_graph tool to generate an CVG for a graph definition in Markdown format\n"
                    "- The tool will display the graph Markdown for you, so you don't need to waist time on that. \n"
                    "- The tool will display the svg to the user.\n\n"
                    "### Important Note\n"
                    "When asked to produce a mermaid chart, the user will see the graph in their chat window. "
                    "You should not waste time and tokens displaying a Mermaid chart in Markdown unless EXPLICITLY "
                    "asked for it in Markdown format by the user.\n"
                    )
        super().__init__(template=TEMPLATE, required=True, name="Mermaid Charts", render_section_header=True, **data)

