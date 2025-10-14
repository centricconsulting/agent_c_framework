from pydantic import BaseModel, ConfigDict
from typing import Optional, List

class ChatMessage(BaseModel):
    """
    A Pydantic model representing a chat message with associated metadata.

    This class defines the structure for chat messages in the system, including
    session management, message content, and LLM configuration parameters.

    Attributes:
        session_id (str): Unique identifier for the chat session
        message (str): The actual content of the chat message
        persona (str): The persona or character role for the message
        temperature (float): The temperature parameter for LLM response generation
        llm_model (str): The identifier for the specific LLM model to be used

    Example:
        >>> chat_msg = ChatMessage(
        ...     session_id="abc123",
        ...     message="Hello, how are you?",
        ...     persona="assistant",
        ...     temperature=0.7,
        ...     llm_model="gpt-4"
        ... )
    """
    model_config = ConfigDict(protected_namespaces=())

    session_id: str
    message: str
    persona: str
    temperature: float
    llm_model: str

class FileUploadRequest(BaseModel):
    """
    A Pydantic model for handling file upload requests.

    This class defines the structure for file upload requests, maintaining
    session context for uploaded files.

    Attributes:
        session_id (str): Unique identifier for the session associated with the file upload

    Example:
        >>> upload_request = FileUploadRequest(session_id="abc123")
    """
    session_id: str
    file_type: Optional[str] = None
    content_type: Optional[str] = None