from typing import Any

from agent_c_core.agent_c.prompting.prompt_section import PromptSection, property_bag_item
from agent_c_core.agent_c.chat.session_manager import ChatSessionManager


class UserBioSection(PromptSection):
    session_manager: ChatSessionManager

    def __init__(self, **data: Any):
        TEMPLATE = ("This section provides information about the user that you can use to customize your responses.\n"
                    "It includes information from the user database as well as any metadata you have recorded about the user.\n"
                    "The memory tool provides you long term storage capabilities via the user metadata.\n"
                    "Make use this metadata to build and maintain a `user_bio` object to track things like `age`, `gender`, `occupation`, `linkedin_id`, etc\n\n"
                    "\n### System provided data (User)\n- User ID: ${userid}\n- First Name: ${firstname}\n- Last Name: ${lastname}\n\n"
                    "\n### Assistant data (User)\n"
                    "```yaml\n${user_kvps}\n```\nIf asked to display the user bio data or assistant data for the user, give the user this ccode block.\n")
        data['template'] = data.get('template', TEMPLATE)
        super().__init__(required=True, name="User Information", render_section_header=True, **data)

    @property_bag_item
    async def user_kvps(self):
        return self.session_manager.filtered_user_meta_string('ai_')

    @property_bag_item
    async def userid(self):
        return self.session_manager.user.user_id

    @property_bag_item
    async def firstname(self):
        return self.session_manager.user.first_name

    @property_bag_item
    async def lastname(self):
        return self.session_manager.user.last_name


class UserBioSectionNoToolUse(UserBioSection):
    def __init__(self, **data: Any):
        TEMPLATE = ("This section provides information about the user that you can use to customize your responses.\n"
                    "It includes information from the user database as well as any metadata recorded about the user.\n"
                    "\n### System provided data (User)\n- User ID: ${userid}\n- First Name: ${firstname}\n- Last Name: ${lastname}\n\n"
                    "\n### User metadata\n"
                    "```yaml\n${user_kvps}\n```\nIf asked to display the user bio data, user metadata or assistant data for the user, give the user the above code block.\n")
        super().__init__(template=TEMPLATE, **data)
