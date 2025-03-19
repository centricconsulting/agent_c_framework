from pydantic import Field, ConfigDict
from agent_c_core.agent_c.models.base import BaseModel
from typing import Optional, List, Union

"""
These models along with the callbacks provided to agents and toolsets allows us 
to completely decouple the agent and toolsets from the UI.  The UI can consume
them via direct callback like the reference client, or they can be shoved into 
a output_queue and processed by a separate thread or process.

Another benefit is that the models can be used by toolsets to bypass the agent
and provide data directly to the UI.  This is useful for toolsets that are token dense, 
need to render media, want to stream data.  By bypassing the agent, for and
presenting the data directly to the UI, we can both save tokens and allow the 
agent to work with data indirectly. Fore example, a waveform tool could use
`render_media` to present a waveform to the user, and return some much smaller
string to the agent.

"""


class FunctionCall(BaseModel):
    """
    Model representing a function call with its name and arguments.
    """
    name: str
    arguments: Optional[str] = ""  # Will be a JSON string


class ToolCall(BaseModel):
    """
    Model representing a tool call with an ID, associated function details, and a constant 'type' field.
    """
    id: str
    function: FunctionCall
    type: str = 'function'


class TextContent(BaseModel):
    type: str
    text: str


class ImageUrlContent(BaseModel):
    url: str


class ImageContent(BaseModel):
    type: str
    image_url: ImageUrlContent


class Message(BaseModel):
    """
    An individual message with a role and content.
    """
    role: str
    content: Optional[Union[str, List[Union[TextContent, ImageContent]]]] = None
    tool_calls: Optional[List[ToolCall]] = None
    tool_call_id: Optional[str] = None
    name: Optional[str] = None
    function: Optional[FunctionCall] = None


class CompletionToolCall(BaseModel):
    """
     A tool call with an ID, function details, and a fixed type.
    """
    id: str
    name: str
    arguments: str


class RenderMedia(BaseModel):
    """
    Model for media rendering details, mapping traditional HTTP-style 'content-type'
    to a model field.
    """
    model_config = ConfigDict(populate_by_name=True)
    content_type: str = Field(..., alias="content-type")
    url: Union[str, None] = None
    name: Union[str, None] = None
    content: Union[str, None] = None
    content_bytes: Union[bytes, None] = None



class ChatEvent(BaseModel):
    """
    This model represents a chat event, which could be any number of things so most fields are optional.

    session_id and role are mandatory fields and will always be present.

    session_id - The ID of the chat session, which in the reference client comes from the sesion manager.
    role - The role that originated the event. 'assistant', 'user', 'system', amd 'tool' are predefined roles.
                 some toolsets may have their own roles, for example toolsets that are themselves agents might send their name as the role.

    completed - A boolean indicating that the current interaction is complete.
               This is used to signal the end of a chat message so that any buffered content can be flushed.

    completion_running - A boolean indicating that a completion is currently running or not
                         If present and True, the client should display a message indicating that a completion is running.
                         If present and False, the client should display a message indicating that the completion has completed.

    content - The content of the message, if any.  This can be as small as a token or as large as a full chat message.

    output_format - The format of the content.  This is used to signal the client how to interpret the content.
                    The default is 'markdown', 'raw' is also supported.

                    Clients should interpret 'raw' as plain text (that might still be in Markdown format),
                    'raw' is essentially a flag that indicates: Display every token as it comes in even if that means fancy
                    rendering doesn't happen.

    messages - Populated by agents and the end of an interaction to provide the complete list of messages,
               including any tool calls and returns.

               This array can be passed to the agent during the next interaction so that it doesn't need to repeat
               a tool call to answer a follow-up question.  This ends up consuming more and more tokens over time,
               but it's a trade-off for not needing to re-run the tool calls.

               When you don't pass the message array, back around the loop the message array will be constructed
               from the chat memory if a session manager has been provided.

               The reference client has the `!compact` command which takes advantage of this by resetting the
               copy of the message array it was holding onto and then caching it again after the next interaction.


    render_media - If present, this field will contain details for media to be rendered in the client.

    start - A boolean indicating that the beginning of a new stream of tokens from the agent is starting.

    tool_use_active - A boolean indicating that a tool is currently active.
                     If present and True, an agent is executing tool calls and the client should display a message to that effect.
                     If present and False, the client should display a message indicating that the tool has completed.

    tool_calls - If tool_use_active is True, this field will contain a list of tool calls that are currently being executed.




    """
    model_config = ConfigDict(populate_by_name=True, extra="forbid")
    session_id: str  # This field is mandatory.
    role: str
    content: Optional[str] = None
    completed: Optional[bool] = None
    tool_use_active: Optional[bool] = None
    output_format: str = Field(default="markdown")
    render_media: Optional[RenderMedia] = None
    completion_running: Optional[bool] = None
    stop_reason: Optional[str] = None
    tool_calls: Optional[List[CompletionToolCall]] = None
    messages: Optional[List[Message]] = None
    start: Optional[bool] = False
    input_tokens: Optional[int] = None
    output_tokens: Optional[int] = None