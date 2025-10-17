from pydantic import Field
from agent_c.models.base import BaseModel
from agent_c.util import MnemonicSlugs
from agent_c_api.core.realtime_bridge import RealtimeBridge


class RealtimeSession(BaseModel):
    """
    Represents a real-time session for a user.
    """
    session_id: str = Field(..., description="Unique identifier for the real-time session")
    user_id: str = Field(..., description="The user ID associated with the session")
    bridge: RealtimeBridge = Field(..., description="The real-time bridge instance for managing the session")

    @classmethod
    def generate_session_id(cls, user_id: str) -> str:
        """
        Generate a unique session ID based on the user ID.

        Args:
            user_id: The user ID to base the session ID on.

        Returns:
            A unique session ID string.
        """
        return f"UI-{user_id}-{MnemonicSlugs.generate_slug(2)}"