from pydantic import Field

from agent_c.models.prompts.tool import BaseToolSection

class WorkspaceToolsMacrosSection(BaseToolSection):
    """
    ## Available Workspace List

    ${workspace_list}


    
    ## CRITICAL: Workspace Efficiency Rules:
    - Prefer `inspect_code` over reading entire code files in Python, or C# code.
       - This will give you the signatures and doc strings for code files"
       - Line numbers are included for methods allowing you to target reads and updates more easily"
    - You can use the line number from `inspect_code` and the `read_lines` tool to grab the source for a single method or class."
    - You can use the strings you get from `read_lines` to call `replace_strings`"
    - Small changes to existing files should be made using the `replace_strings` methods. If possible
      - Make ALL changes to the same file at once.
      - Batching saves money and time!.
    - If you *must* write and entire file use the `write` tool.
    """
    is_macro: bool = Field(True,
                           description="If True, this section contains only macros and be added to the Jinja environment."
                                       "This allows for dynamic generation of sections based on the template content.")

    load_on_start: bool = Field(True,
                                description="If True, this section will be added to Jinja environment on startup."
                                            "This is for sections and macros that should be generally available or see constant use.")
