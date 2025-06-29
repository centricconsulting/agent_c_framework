from agent_c.models.prompts.tool import BaseToolSection

class WorkspaceToolsSection(BaseToolSection):
    """
    ## The Workspace toolset

    The tools with the prefix `workspace_` provide access to one or more file systems (referred to as workspaces). In addition to the typical file operations this toolset provides many tools that allow you to be more precise with file operations and/or save time/resources. It is ESSENTIAL to use the most efficient tools for the task at hand.

    ### Context window protections
    Most tools in this toolset contain some level of context window protection and will respond with a warning if the operation exceeds a configured token limit. When this happens,  refer to any explict instructions provided to you if any.  If explict instructions have been provided for handling large results you MUST immediately:

    1. Proceed no further with the operation
    2. Devise a plan to handle the large result using more efficient tools or methods
    3. Report back to the user / supervisor with your plan and WAIT for approval. YOU MUST suggest to the user that there may be a toolset that would allow to handle the operation more efficiently, and suggest they check the tool catalog first.

    ### Operations Guideline

    - Always use UNC-style paths in the format `//WORKSPACE/path/to/file` where WORKSPACE is the workspace name
    - Use read_lines` to read a portion of a file or`read` if you need the entire file contents
    - Parent directories are created automatically on `write` operations
    - Appending: Use `write` with the optional `mode` parameter set to append content to existing files
    = If available, use `grep` to search for patterns in files. This uses the same syntax as the grep CLI. Check the workspace list for availability.
    - Use `tree` for broader holistic views of the workspace. Use `ls` to list a specific directory contents
    -- Use `cp` to copy and `mv` to move files or directories
        - Both source and destination must be in the same workspace
    - Workspace text files are UTF-8 encoded by defaults, however, you can specify a different encoding if needed if a read operation result in mangled text.
    - Changes to existing files must be made using the `replace_strings tool whenever possible
        - Make ALL changes to the same file at once.
        - Batching saves resources and time!.
    """