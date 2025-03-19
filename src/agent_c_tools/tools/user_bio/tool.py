from typing import Any, Union

from agent_c_core.agent_c.toolsets import json_schema, Toolset
from agent_c_tools.tools.user_bio.prompt import UserBioSection, UserBioSectionNoToolUse

class UserBioTools(Toolset):
    """
    UserBioTools  is a tool to allow the model to update the user record.
    """

    def __init__(self, **kwargs: Any):
        """Initialize MemoryTools with a MemorySection instance.

        Args:
            **kwargs (Any): Keyword arguments including those for ZepDependentToolset.
        """
        super().__init__(**kwargs, name='userbio', need_tool_user=False)

        if self.agent_can_use_tools:
            section_cls = UserBioSection
        else:
            section_cls = UserBioSectionNoToolUse

        self.section = kwargs.get('section', section_cls(session_manager=self.session_manager))

    @json_schema(
        (
            'Update the first and/or last name in the user record for the current chat user.'
        ),
        {
            'first': {
                'type': 'string',
                'description': "The user's first name",
                'required': False
            },
            'last': {
                'type': 'string',
                'description': "The user's last name.",
                'required': False
            }
        }
    )
    async def update_name(self, **kwargs: Any) -> str:
        first: Union[str, None] = kwargs.get("first", None)
        last: Union[str, None] = kwargs.get("last", None)

        if first is not None:
            self.session_manager.user.first_name = first

        if last is not None:
            self.session_manager.user.last_name = last

        if first is None and last is None:
            return "You need to supply `first` and/or `last` to update"

        return "User records updated"


Toolset.register(UserBioTools)
