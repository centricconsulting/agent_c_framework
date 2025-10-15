from functools import singledispatchmethod

from agent_c.models.events import BaseEvent
from agent_c.models.events.chat import AudioInputDeltaEvent
from agent_c_api.api.rt.models.control_events import GetAgentsEvent, GetAvatarsEvent, TextInputEvent, SetAvatarEvent, SetAgentEvent, SetAvatarSessionEvent, ResumeChatSessionEvent, \
    NewChatSessionEvent, SetAgentVoiceEvent, GetUserSessionsEvent, PingEvent, PongEvent, \
    GetToolCatalogEvent, GetVoicesEvent, DeleteChatSessionEvent, ClientWantsCancelEvent, SetAgentToolsEvent
from agent_c_api.api.rt.models.control_events import SetChatSessionNameEvent, SetSessionMessagesEvent, SetSessionMetadataEvent


class ClientEventHandler:
    """
    Mixin class providing handlers for client events.
    Each handler method is registered to handle a specific event type.
    The default handler logs a warning for unhandled event types and notifies the client via a system message.
    """

    @singledispatchmethod
    async def handle_client_event(self, event: BaseEvent) -> None:
        """Default handler for unknown events"""
        self.logger.warning(f"RealtimeBridge {self.ui_session_id}: Unhandled event type: {event.type}")
        await self.send_system_message(f"RealtimeBridge did not handle event type: {event.type}.", severity="warning")

    @handle_client_event.register
    async def _(self, _: PingEvent) -> None:
        await self.send_event(PongEvent())

    @handle_client_event.register
    async def _(self, _: GetAgentsEvent) -> None:
        await self.reload_agents()

    @handle_client_event.register
    async def _(self, _: ClientWantsCancelEvent) -> None:
        await self.cancel_interaction()

    @handle_client_event.register
    async def _(self, event: DeleteChatSessionEvent):
        await self.delete_chat_session(event.session_id)

    @handle_client_event.register
    async def _(self, event: SetAgentVoiceEvent):
        await self.set_agent_voice(event.voice_id)


    @handle_client_event.register
    async def _(self, event: TextInputEvent) -> None:
        await self.process_text_input(event.text, event.file_ids)

    @handle_client_event.register
    async def _(self, _: GetAvatarsEvent) -> None:
        await self.send_avatar_list()

    @handle_client_event.register
    async def _(self, event: SetAvatarEvent) -> None:
        await self.set_avatar(event.avatar_id, event.quality, event.video_encoding)

    @handle_client_event.register
    async def _(self, event: AudioInputDeltaEvent) -> None:
        self.logger.info("RealtimeBridge received audio input delta event")

    @handle_client_event.register
    async def _(self, event: SetAvatarSessionEvent) -> None:
        """Set the avatar session using the provided access token and session ID"""
        await self.set_avatar_session(event.access_token, event.avatar_session_id)

    @handle_client_event.register
    async def _(self, event: ResumeChatSessionEvent) -> None:
        await self.resume_chat_session(event.session_id)

    @handle_client_event.register
    async def _(self, event: SetChatSessionNameEvent) -> None:
        await self.set_chat_session_name(event.session_name, event.session_id)

    @handle_client_event.register
    async def _(self, event: GetUserSessionsEvent) -> None:
        await self.send_user_sessions(event.offset, event.limit)

    @handle_client_event.register
    async def _(self, event: SetSessionMetadataEvent) -> None:
        await self.set_session_metadata(event.meta)

    @handle_client_event.register
    async def _(self, event: NewChatSessionEvent) -> None:
        await self.new_chat_session(event.agent_key)

    @handle_client_event.register
    async def _(self, event: GetVoicesEvent):
        await self.send_voices()

    @handle_client_event.register
    async def _(self, event: GetToolCatalogEvent):
        await self.send_tool_catalog()

    @handle_client_event.register
    async def _(self, event: SetSessionMessagesEvent) -> None:
        await self.set_session_messages(event.messages)

    @handle_client_event.register
    async def _(self, event: SetAgentEvent) -> None:
        await self.set_agent(event.agent_key)

    @handle_client_event.register
    async def _(self, event: SetAgentToolsEvent) -> None:
        await self.update_tools(event.tools)
