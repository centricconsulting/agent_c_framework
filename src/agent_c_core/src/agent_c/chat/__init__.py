from agent_c.chat.session_manager import ChatSessionManager
from agent_c.models.chat_history.chat_session import ChatSession # noqa

DefaultChatSessionManager = ChatSessionManager

def get_default_chat_session_manager() -> ChatSessionManager:
    """
    Returns the default chat session manager instance.

    This function provides a singleton instance of the default chat session manager.

    Returns:
        ChatSessionManager: The default chat session manager instance.
    """
    return DefaultChatSessionManager.instance()
