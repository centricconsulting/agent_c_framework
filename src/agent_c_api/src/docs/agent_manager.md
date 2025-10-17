# AgentManager Class Documentation

## Overview
The `AgentManager` class is responsible for managing agent sessions in a multi-agent environment. It handles session creation, cleanup, and response streaming for ReactJS agents, with support for different LLM backends and tool configurations.

## Class Attributes

- `ESSENTIAL_TOOLS`: List of required tool sets for all agents
  - Values: `['MemoryTools', 'WorkspaceTools', 'PreferenceTools', 'RandomNumberTools']`

## Instance Attributes

- `logger`: Logging instance for the class
- `sessions`: Dictionary storing session data, keyed by session ID
- `_locks`: Dictionary of asyncio locks for thread-safe session operations

## Methods

### create_session
```python
async def create_session(
    self,
    llm_model: str = "gpt-4o",
    backend: str = 'openai',
    persona_name: str = 'default',
    additional_tools: List[str] = None,
    existing_session_id: str = None,
    **kwargs
) -> str
```

Creates a new agent session or updates an existing one.

**Parameters:**
- `llm_model`: The language model to use (default: "gpt-4o")
- `backend`: LLM provider - either 'openai' or 'claude' (default: 'openai')
- `persona_name`: Name of the persona to use (default: 'default')
- `additional_tools`: List of additional tool class names to add to the agent
- `existing_session_id`: Optional ID of an existing session to update
- `**kwargs`: Additional keyword arguments passed to ReactJSAgent

**Returns:**
- `str`: Session ID (either new or existing)

**Behavior:**
1. Generates or uses existing session ID
2. Creates thread-safe lock if needed
3. Initializes ReactJSAgent with specified configuration
4. Transfers state from existing agent if updating
5. Stores session information

### cleanup_session
```python
async def cleanup_session(self, session_id: str)
```

Cleans up resources associated with a specific session.

**Parameters:**
- `session_id`: ID of the session to clean up

**Behavior:**
1. Removes session data from sessions dictionary
2. Removes associated lock
3. Handles cleanup gracefully with error logging

### stream_response
```python
async def stream_response(
    self,
    session_id: str,
    user_message: str,
    custom_prompt: Optional[str] = None
) -> AsyncGenerator[str, None]
```

Streams agent responses for a given user message.

**Parameters:**
- `session_id`: Session identifier
- `user_message`: User's input message
- `custom_prompt`: Optional custom prompt override

**Returns:**
- AsyncGenerator yielding response chunks

**Behavior:**
1. Validates session ID
2. Uses ReactJSAgent's stream_chat method
3. Yields response chunks as they become available
4. Handles errors with appropriate error messages

### get_session_data
```python
def get_session_data(self, session_id: str) -> Dict[str, Any]
```

Retrieves session data for a given session ID.

**Parameters:**
- `session_id`: Session identifier

**Returns:**
- Dictionary containing session data or None if not found

## Error Handling

The class implements comprehensive error handling:
- Invalid session IDs raise ValueError
- Session cleanup errors are logged but don't halt execution
- Streaming errors are caught and yielded as error messages

## Thread Safety

Thread safety is ensured through:
- Per-session asyncio locks
- Locked session creation/update operations
- Safe session data access patterns

## Usage Example

```python
# Create an agent manager
manager = AgentManager()

# Create a new session
session_id = await manager.create_session(
    llm_model="gpt-4",
    backend="openai",
    additional_tools=["CustomTool1", "CustomTool2"]
)

# Stream responses
async for chunk in manager.stream_response(session_id, "Hello, agent!"):
    print(chunk)

# Cleanup when done
await manager.cleanup_session(session_id)
```