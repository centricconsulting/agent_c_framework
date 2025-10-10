from jinja2 import pass_context

from agent_c.models.prompts.prompt_globals.base import BasePromptGlobals
from agent_c_tools import WorkspaceTools
from agent_c.util.slugs import MnemonicSlugs


class WorkspaceToolGlobals(BasePromptGlobals):
    """
    Registers section import helpers in the Jinja 2 environment.
    """
    add_as_object = False

    @staticmethod
    def _workspace_tool(ctx) -> WorkspaceTools:
        """
        Helper method to retrieve the workspace from the context.
        """
        return ctx.interaction.toolsets["WorkspaceTools"]

    @classmethod
    @pass_context
    def path_block(cls, ctx, path: str, path_content: str, content_type: str, path_meta: str = "", token_count: int = 0, token_limit=25000) -> str:
        """
        Render the given template name only the first time it’s called
        during a single render pass. Subsequent calls return an empty string.
        """
        toggle_slug = MnemonicSlugs.generate_id_slug(2, seed=path)
        if path_meta:
            path_comment = f"  # {path_meta}"
        else:
            path_comment = f"  # {content_type}"

        if token_count > token_limit:
            content = f"Content too large ({token_count} tokens), not displaying."
        else:
            content = path_content

        block = ctx.collapsible(toggle_slug, f"``` {path}{path_comment}\n{content}\n```\n", f"`{path}` {content_type} ({token_count} tokens) collapsed")
        return f"### `{path}` {content_type}:\n{block}"

    @classmethod
    @pass_context
    def tree(cls, ctx, path: str, folder_depth: int = 5, file_depth = 3, max_tokens=25000) -> str:
        """
        Render the given template name only the first time it’s called
        during a single render pass. Subsequent calls return an empty string.
        """
        tree_content, token_count = cls._call_async(cls._workspace_tool(ctx).tree_internal(path=path,
                                                                     file_depth=file_depth,
                                                                     folder_depth=folder_depth,
                                                                     context=ctx.interaction))
        if token_count > max_tokens:
            return f"Tree content too large ({token_count} tokens), not displaying."

        return cls.path_block(path, tree_content, "tree", token_count)

    @classmethod
    @pass_context
    def file_content(cls, ctx, path: str) -> str:
        """
        Get the content of a file at the given path.
        """
        contents, token_count = cls._call_async(cls._workspace_tool(ctx).read_internal(path=path, context=ctx.interaction))
        return cls.path_block(path, contents, "file", token_count)

    @classmethod
    @pass_context
    def file_lines_content(cls, ctx, path: str, start_line: int, end_line: int) -> str:
        """
        Get the content of a file at the given path, limited to specific lines.
        """
        contents, line_count, token_count = cls._call_async(cls._workspace_tool(ctx).read_lines_internal(path=path,
                                                                                             start_line=start_line,
                                                                                             end_line=end_line,
                                                                                             context=ctx.interaction))
        meta = f"lines {start_line}-{end_line} ({line_count} lines, {token_count} tokens)"
        return cls.path_block(path, contents, "file lines", meta, token_count)

