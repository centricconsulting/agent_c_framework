from typing import Optional

from jinja2 import pass_context
from pydantic import Field

from agent_c.models.prompts.prompt_globals.base import BasePromptGlobals


class SectionImportGlobals(BasePromptGlobals):
    """
    Registers section import helpers in the Jinja 2 environment.
    """
    add_as_object: bool = Field(False)

    @staticmethod
    @pass_context
    def prompt_section(ctx, name: str, override_seen: Optional[bool] = False) -> str:
        """
        Render the given template name only the first time it’s called
        during a single render pass. Subsequent calls return an empty string.
        """
        # ctx is the current Context; ctx.vars is fresh per render()
        seen = ctx.vars.setdefault("_seen_templates", set())

        if name in seen and not override_seen:
            # Already rendered this one in *this* pass
            return ""

        seen.add(name)

        # Grab and render the subtemplate with the *same* context
        tmpl = ctx.environment.get_template(name)
        return tmpl.render(**ctx.get_all())

    @staticmethod
    @pass_context
    def skip_prompt_section(ctx, name: str) -> str:
        """
        Skip rendering the given template name. This is useful for sections
        that should not be rendered in certain contexts.
        """
        # ctx is the current Context; ctx.vars is fresh per render()
        seen = ctx.vars.setdefault("_seen_templates", set())

        if name in seen:
            # Already rendered this one in *this* pass
            return ""

        seen.add(name)

        # Return an empty string to skip rendering
        return ""

    @pass_context
    def tool_section(self, ctx, section_name: str, override_seen: Optional[bool] = False) -> str:
        """
        Render a tool section with the given name and additional context variables.
        """
        key = section_name.startswith('tools/') and section_name or f"tools/{section_name}"
        return self.prompt_section(ctx, key, override_seen)

    @pass_context
    def skip_tool_section(self, ctx, section_name: str) -> str:
        """
        Skip rendering a tool section with the given name.
        """
        key = section_name.startswith('tools/') and section_name or f"tools/{section_name}"
        return self.skip_prompt_section(ctx, key)
