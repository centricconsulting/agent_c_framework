from typing import Any, Optional, cast

from agent_c.prompting.prompt_section import OldPromptSection, property_bag_item


class WorkspaceSequentialThinkingSection(OldPromptSection):

    def __init__(self, **data: Any):
        TEMPLATE = (
            "The Workspace Sequential Thinking Tools (wst) provide a structured approach to thinking through complex problems step-by-step."
            " This tool helps you break down problems, revise your thinking, and explore alternative paths of reasoning.\n\n"
            
            "## Sequential Thinking Process\n"
            "- Thoughts: Individual steps in your reasoning process\n"
            "- Branches: Alternative paths of thinking that explore different approaches\n"
            "- Revisions: Updates to previous thoughts based on new insights\n\n"
            
            "## Path Format\n"
            "- Use a UNC style path of `//[workspace]/[thinking_id]` to reference thinking processes\n"
            "- Example: `//project/problem_analysis` refers to a sequential thinking process named 'problem_analysis' in the 'project' workspace\n\n"
            
            "## Core Features\n"
            "- Create structured thinking processes with numbered thoughts\n"
            "- Revise previous thoughts when new information emerges\n"
            "- Branch into alternative thinking paths to explore different approaches\n"
            "- Track your reasoning process over time\n"
            "- Indicate when more thinking is needed on a topic\n\n"
            
            "## When to Use Sequential Thinking\n"
            "1. Breaking down complex problems into steps\n"
            "2. Planning and design with room for revision\n"
            "3. Analysis that might need course correction\n"
            "4. Problems where the full scope might not be clear initially\n"
            "5. Tasks that need to maintain context over multiple steps\n"
            "6. Situations where irrelevant information needs to be filtered out\n\n"
            
            "## Recommended Approach\n"
            "1. Start with an initial estimate of needed thoughts\n"
            "2. Feel free to question or revise previous thoughts\n"
            "3. Don't hesitate to add more thoughts if needed, even at the 'end'\n"
            "4. Express uncertainty when present\n"
            "5. Mark thoughts that revise previous thinking or branch into new paths\n"
            "6. Only set next_thought_needed to false when truly done\n"
        )
        super().__init__(template=TEMPLATE, required=True, name="Workspace Sequential Thinking", render_section_header=True, **data)