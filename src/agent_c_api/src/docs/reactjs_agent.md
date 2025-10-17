# ReactJSAgent Documentation

## Overview
The `ReactJSAgent` class serves as a bridge between the agent_c library and ReactJS applications, 
providing chat functionality with AI models and access to various tools. 
It manages chat sessions, user preferences, tool integrations, and streaming responses.

## Class Architecture
### Core Components
- **Session Management**: Handles chat sessions and user interactions
- **Tool Management**: Manages various tools and their initialization
- **Agent Configuration**: Configures the underlying AI model and its parameters
- **Event Handling**: Processes various types of events (text, tools, media, etc.)
- **Streaming**: Provides asynchronous streaming of chat responses

### Key Features
- Support for multiple AI backends (OpenAI, Claude)
- Dynamic tool loading and initialization
- Customizable persona management
- Asynchronous streaming responses
- Comprehensive event handling system
- Configurable logging

## Constructor Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| user_id | str | 'default' | Identifier for the user |
| session_manager | Union[ChatSessionManager, None] | None | Manager for chat sessions |
| backend | str | 'openai' | AI backend to use ('openai' or 'claude') |
| model_name | str | 'gpt-4o' | Name of the AI model |
| persona_name | str | 'default' | Name of the persona to load |
| custom_persona_text | str | '' | Custom persona text |
| essential_tools | List[str] | None | List of essential tools |
| additional_tools | List[str] | None | List of additional tools |

## Main Methods
### initialize()
```python
async def initialize()
```
Initializes the agent by setting up the session, tool chest, and internal agent configuration.

### stream_chat()
```python
async def stream_chat(user_message: str, custom_prompt: str = None) -> AsyncGenerator[str, None]
```
Streams chat responses for a given user message. Returns an async generator yielding JSON-formatted strings.

### update_tools()
```python
async def update_tools(new_tools: List[str])
```
Updates the agent's tool set without reinitializing the entire agent.

## Event Handling
The class handles various types of events through specialized handlers:
- Text delta events (`_handle_text_delta`)
- Tool call events (`_handle_tool_call`)
- Media render events (`_handle_render_media`)
- History events (`_handle_history`)
- Audio delta events (`_handle_audio_delta`)
- Completion events (`_handle_completion`)
- Interaction events (`_handle_interaction`)

## Configuration
Agent configuration includes:
- User preferences
- Tool settings
- Model parameters
- Logging settings
- Workspace configurations

## Error Handling
The class implements comprehensive error handling:
- Tool initialization errors
- Stream processing errors
- File loading errors
- Configuration errors

## Usage Example

```python
# Initialize the agent
agent = ReactJSAgent(
    user_id="user123",
    backend="claude",
    model_name="claude-3",
    additional_tools=["CustomTool1", "CustomTool2"]
)

# Initialize the agent
await agent.initialize()

# Stream chat responses
async for response in agent.stream_chat("Hello, how can you help me?"):
    print(response)
```

## Best Practices
1. Always await initialization before using the agent
2. Handle streaming responses appropriately
3. Properly configure essential tools
4. Manage chat sessions effectively
5. Handle errors and exceptions appropriately
