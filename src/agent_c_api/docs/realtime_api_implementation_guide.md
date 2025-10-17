# Agent C Realtime API Implementation Guide

## Overview

The Agent C Realtime API provides a WebSocket-based interface for real-time communication with AI agents, integrated with HeyGen streaming avatars. This guide covers the complete event system, authentication, and payload structures for implementing clients.

## Architecture

The realtime API consists of:

- **REST Authentication Endpoints** - Initial authentication and configuration
- **WebSocket Bridge** - Bidirectional event streaming
- **Event System** - BaseEvent-based command/response event handling with inheritance hierarchy
- **Avatar Integration** - HeyGen streaming avatar support

### Event System Architecture

The Agent C realtime API uses a structured event system built on a clear inheritance hierarchy:

#### BaseEvent Structure

All events inherit from `BaseEvent` which provides:
- `type` (str): Event type identifier in snake_case format without "event" suffix
- Automatic type assignment: defaults to snake_case version of class name minus "Event"
- Automatic event registration with EventRegistry

#### Event Categories

**Control Events**: Inherit directly from `BaseEvent`
- Handle session management, configuration, and system operations
- Only contain the base `type` field plus event-specific data
- Examples: `get_agents`, `set_agent`, `avatar_list`, `tool_catalog`

**Session Events**: Inherit from `SessionEvent` (which extends `BaseEvent`)
- Handle chat interactions and content within active sessions
- Include session context fields:
  - `session_id` (required str): Current chat session identifier
  - `role` (required str): Role that triggered the event 
  - `parent_session_id` (optional str): Parent session if this is a child session
  - `user_session_id` (optional str): Top-level user session if this is nested
- Examples: `text_delta`, `completion`, `tool_call`, `render_media`

#### Event Type Naming Convention

Event types follow snake_case naming WITHOUT "event" suffix:
- Class: `TextDeltaEvent` → Type: `"text_delta"`
- Class: `SystemPromptEvent` → Type: `"system_prompt"`
- Class: `ToolCallEvent` → Type: `"tool_call"`

This convention is enforced automatically by the BaseEvent constructor.

---

## Identifier System: MnemonicSlugs

The Agent C Realtime API uses a human-friendly identifier system called **MnemonicSlugs** for session IDs, user IDs, and other identifiers throughout the system. This system replaces traditional opaque GUIDs with memorable word combinations.

### Why MnemonicSlugs?

Consider these identifiers:
- Traditional: `8f9240688685a1e9`
- MnemonicSlug: `magic-slang-crimson`

MnemonicSlugs are:
- **Human-readable** - Easy to read aloud, type, and remember
- **Error-resistant** - Easier to spot typos and communication errors
- **Support-friendly** - Better for error messages and user communication
- **Deterministic** - Same input always generates same slug
- **Hierarchical** - Support parent-child relationships

### Format and Structure

#### Basic Format
- **Single words**: `tiger` (1,633 possible values)
- **Two words**: `tiger-castle` (2.67 million combinations)
- **Three words**: `magic-slang-crimson` (4.35 billion combinations)

#### Hierarchical IDs
Used for nested relationships with `:` separating levels:
```
user-id:session-id:message-id
tiger-castle:apollo:banana
```

### Usage in Realtime API

#### Session Identifiers
- **UI Session IDs**: 3-word slugs (e.g., `"tiger-castle-moon"`)
- **Chat Session IDs**: 2-word slugs (e.g., `"purple-river"`)
- **User Session IDs**: 2-word slugs for user identification

#### Subsession Correlation
Parent-child session relationships use hierarchical format:
```json
{
  "session_id": "bright-cloud",
  "parent_session_id": "purple-river",
  "user_session_id": "purple-river"
}
```

#### Examples in API Responses
```json
// Login response
{
  "ui_session_id": "tiger-castle-moon"
}

// Chat session
{
  "session_id": "purple-river-sky",
  "session_name": "My Conversation"
}

// Session events
{
  "type": "text_delta",
  "session_id": "bright-cloud",
  "parent_session_id": "purple-river",
  "user_session_id": "purple-river"
}
```

### Implementation Considerations

#### Client Handling
- **Case Insensitive**: All operations are case-insensitive
- **Validation**: Expect format `word-word` or `word-word-word`
- **Display**: Show as-is to users (human-friendly)
- **Storage**: Store exactly as received from API

#### Benefits for Development
- **Debugging**: Easier to trace issues in logs
- **Testing**: Deterministic ID generation for test scenarios
- **User Support**: Users can communicate IDs verbally
- **Error Messages**: More meaningful error reporting

*Reference: Full MnemonicSlugs documentation at `//project/docs/developer/README_IDs.md`*

---

## REST API Endpoints

### Authentication & Configuration

#### `POST /rt/login`

Authenticate user and get initial configuration.

**Request Body:**

```json
{
  "username": "string",
  "password": "string"
}
```

**Response:**

```json
{
  "agent_c_token": "jwt_token_string",
  "heygen_token": "heygen_access_token",
  "ui_session_id": "three-word-slug"
}
```

**Note:** User data, agents, avatars, tools, and voices are provided via the WebSocket initialization events after connection, not in the login response.

```

#### `GET /rt/refresh_token`

Refresh JWT and HeyGen tokens.

**Headers:** `Authorization: Bearer <jwt_token>`

**Response:**

```json
{
  "agent_c_token": "new_jwt_token",
  "heygen_token": "new_heygen_token"
}
```

## WebSocket Connection

### Connection Endpoint

`wss://host/rt/ws?token={jwt_token}&session_id={optional_session_id}`

**HTTPS Requirement:** The API requires HTTPS even in development environments to ensure proper audio support for web clients. This is necessary for WebRTC audio features and secure WebSocket connections.

**Parameters:**

- `token` (required) - JWT token from login
- `session_id` (optional) - Resume existing UI session

### Session Management

The system uses two types of session identifiers:

#### UI Session IDs

- **Purpose:** Represent a client connection to the server
- **Scope:** Each unique WebSocket connection gets one UI session ID
- **Usage:** Used for internal caching and session resumption after network issues
- **Persistence:** Temporary - tied to active connections
- **Format:** Three-word mnemonic slug (e.g., "tiger-castle-moon")

#### Chat Session IDs

- **Purpose:** Represent persistent chat conversations between user and agent
- **Scope:** Multiple UI sessions can access the same chat session
- **Usage:** Used for chat history persistence, session switching, and resumption
- **Persistence:** Permanent - stored in database
- **Format:** Mnemonic slug format

**Example Workflow:**

1. Client connects → receives `ui_session_id: "tiger-castle-moon"`
2. Client starts chat → receives `chat_session_id: "purple-river-sky"`
3. Network drops → client reconnects with same `ui_session_id`
4. Client can resume `chat_session_id: "purple-river-sky"` or switch to different chat

### Connection Flow

1. **Authentication** - JWT token validation
2. **Session Creation/Resume** - Initialize or load chat session
3. **Bridge Initialization** - Setup event handlers
4. **Initialization Events** - Server sends configuration data to client
5. **Event Loop** - Bidirectional event processing

#### Initialization Event Sequence

When a client successfully connects to the WebSocket, the server automatically sends a series of initialization events in a specific order. This sequence is triggered immediately after WebSocket connection acceptance and provides all necessary configuration data:

**Source Location:** `RealtimeBridge.run()` method in `//api/src/agent_c_api/core/realtime_bridge.py`

**Exact 7-Event Sequence:**

1. **`chat_user_data`** - Current user information and profile data
   - **Content:** `ChatUserData` object with user ID, username, email, roles, etc.
   - **Purpose:** Provides client with authenticated user context

2. **`avatar_list`** - Available HeyGen avatars for the user
   - **Content:** Array of `Avatar` objects with avatar IDs, previews, and configuration
   - **Purpose:** Populates avatar selection UI

3. **`voice_list`** - Available TTS voice models and configurations
   - **Content:** Array of `AvailableVoiceModel` objects including system voices (none, avatar) and OpenAI voices
   - **Purpose:** Enables voice selection and TTS configuration

4. **`agent_list`** - Available AI agents the user can interact with
   - **Content:** Array of `AgentCatalogEntry` objects with agent names, keys, descriptions, and categories
   - **Purpose:** Populates agent selection interface

5. **`tool_catalog`** - Available tools and their schemas for the current session
   - **Content:** Array of `ClientToolInfo` objects with tool schemas and descriptions
   - **Purpose:** Informs client of available agent capabilities

6. **`chat_session_changed`** - Current or newly created chat session state
   - **Content:** Complete `ChatSession` object with session ID, messages, agent config, metadata
   - **Purpose:** Establishes current conversation context

7. **`user_turn_start`** - ⚠️ **CRITICAL CLIENT READY SIGNAL**
   - **Content:** No additional data (marker event only)
   - **Purpose:** **Signals that the server is ready to accept user input** (text or audio)

**⚠️ IMPORTANT:** Clients MUST NOT send input until receiving `user_turn_start`. This event indicates the server has completed initialization and is ready for user interaction.

This exact sequence provides all necessary information for clients to:
- Render the user interface
- Configure available options
- Display current session state  
- Begin accepting user input safely

---

## Chat Session Models

### ChatSession Model

The core chat session model as defined in `//core/src/agent_c/models/chat_history/chat_session.py`:

```python
class ChatSession(BaseModel):
    version: int = 1
    session_id: str  # Generated mnemonic slug
    token_count: int = 0
    context_window_size: int = 0
    session_name: Optional[str] = None
    created_at: Optional[str]  # ISO timestamp
    updated_at: Optional[str]  # ISO timestamp
    deleted_at: Optional[str] = None
    user_id: Optional[str] = "admin"
    metadata: Optional[Dict[str, Any]] = {}
    messages: List[dict[str, Any]] = []  # Vendor-specific format
    agent_config: Optional[CurrentAgentConfiguration] = None
```

#### Computed Fields

**`vendor`** - Determines message format based on agent configuration:
- `"anthropic"` - When model_id starts with "claude" or "bedrock"
- `"openai"` - For all other model_ids
- `"none"` - When no agent_config is present

**`display_name`** - User-friendly session name:
- Returns `session_name` if set
- Otherwise returns `"New chat with {agent_name}"`

#### Message Formats by Vendor

**Anthropic (`vendor: "anthropic"`):**
Follows Anthropic MessageParam specification:
```json
{
  "role": "user|assistant",
  "content": "text" // or array of content blocks
}
```

Content blocks can include:
- Text blocks: `{"type": "text", "text": "content"}`
- Image blocks: `{"type": "image", "source": {...}}`
- Tool use blocks: `{"type": "tool_use", "id": "...", "name": "...", "input": {...}}`
- Tool result blocks: `{"type": "tool_result", "tool_use_id": "...", "content": "..."}`

**OpenAI (`vendor: "openai"`):**
Follows OpenAI ChatCompletionMessageParam union type format. Messages can be one of several types:

- **User messages**: `{"role": "user", "content": "text or content parts array", "name?": "participant_name"}`
- **Assistant messages**: `{"role": "assistant", "content": "text or content array", "tool_calls?": [...], "function_call?": {...}, "refusal?": "...", "audio?": {...}}`
- **System messages**: `{"role": "system", "content": "text or content parts array", "name?": "system_name"}`
- **Tool messages**: `{"role": "tool", "content": "text or content array", "tool_call_id": "call_id"}`
- **Function messages**: `{"role": "function", "content": "text", "name": "function_name"}`
- **Developer messages**: `{"role": "developer", "content": "text or content parts array"}`

Content can be string or array of content parts including text, images, files, and audio. Complex tool calls, function calls, and multi-modal content are supported.

*Reference: OpenAI ChatCompletionMessageParam specification*

**Important:** Messages do NOT contain timestamps. All temporal information is managed at the session level.

### ChatSessionIndexEntry Model

Lightweight session index used for session lists:

```python
class ChatSessionIndexEntry(BaseModel):
    session_id: str
    session_name: Optional[str] = None
    created_at: Optional[str]  # ISO timestamp
    updated_at: Optional[str]  # ISO timestamp 
    user_id: Optional[str] = "admin"
    agent_key: Optional[str] = None
    agent_name: Optional[str] = None
```

This model is used in session lists and queries where the full message history is not needed.

### ChatSessionQueryResponse Model

Response container for paginated session queries:

```python
class ChatSessionQueryResponse(BaseModel):
    chat_sessions: List[ChatSessionIndexEntry] = []
    total_sessions: int = 0
    offset: int = 0
```

---

## Agent Configuration System

### Overview

The Agent Configuration system defines how individual agents are configured and how they collaborate within the Agent C framework. Agent configurations appear in several contexts within the realtime API:

- **Session Events**: `AgentListEvent` provides available agent configurations
- **Chat Sessions**: Agent configurations determine behavior and capabilities
- **Subsessions**: Agent-to-agent conversations use these configurations
- **Team Collaboration**: The category system enables sophisticated agent teamwork

### CurrentAgentConfiguration Model

The `CurrentAgentConfiguration` model (version 2) defines the complete structure of an agent configuration:

#### Core Fields

| Field | Type | Description |
|-------|------|-----------|
| `version` | `int` | Configuration version (currently 2) |
| `key` | `string` | Unique identifier for the agent configuration |
| `name` | `string` | Human-readable name of the agent |
| `agent_description` | `string?` | Optional description of the agent's purpose and capabilities |
| `model_id` | `string` | ID of the LLM model used by the agent |
| `persona` | `string` | Core instructions that define the agent's behavior |
| `uid` | `string?` | Optional unique identifier in slug form |

#### Configuration Fields

| Field | Type | Description |
|-------|------|-----------|
| `agent_params` | `CompletionParams?` | Optional LLM interaction parameters |
| `prompt_metadata` | `object?` | Metadata for prompt generation and variable substitution |
| `tools` | `string[]` | List of enabled toolset names the agent can use |
| `blocked_tool_patterns` | `string[]` | Patterns for blocking specific tools (e.g., `"run_*"`) |
| `allowed_tool_patterns` | `string[]` | Patterns that override blocks (e.g., `"run_git"`) |
| `category` | `string[]` | List of categories from most to least general |

### Agent Category System

The category system is critical for understanding agent behavior and collaboration patterns. Categories serve multiple purposes:

#### Special Category Meanings

##### `'domo'` - User Collaboration Agents
- Agents designed for direct interaction with users
- Include human interaction rules and safety guidelines
- Optimized for conversational interfaces
- Can handle user requests and provide assistance

##### `'realtime'` - Voice-Optimized Agents  
- Specifically optimized for voice conversation
- **Always include `'domo'` as a category** (all realtime agents are also domos)
- Tuned for natural speech patterns and real-time interaction
- Include voice-specific behavioral guidelines

##### `'assist'` - Agent Helper Agents
- Designed to help other agents, not end users
- **Lack human interaction rules and safety guidelines**
- Exposed via the `AgentAssistTool` for agent-to-agent communication
- Focused on specific technical tasks or capabilities

#### Team Formation Through Categories

When an agent key appears in another agent's categories, it creates a **team relationship**:

```json
{
  "key": "agent_b",
  "name": "Agent B", 
  "category": ["agent_a", "assist"],
  // ... other fields
}
```

In this example:
- `agent_b` is part of `agent_a`'s team
- `agent_a` can use `AgentTeamTools` to communicate with `agent_b`
- This enables sophisticated multi-agent collaboration patterns

### Completion Parameters

The `agent_params` field contains provider-specific parameters for LLM interactions. The system supports multiple LLM providers through a discriminated union:

#### Parameter Types

**Claude Parameters**:
- **Non-Reasoning** (`claude_non_reasoning`): Standard Claude interaction
- **Reasoning** (`claude_reasoning`): Claude with reasoning capabilities

**GPT Parameters**:
- **Non-Reasoning** (`g_p_t_non_reasoning`): Standard GPT interaction 
- **Reasoning** (`g_p_t_reasoning`): GPT with reasoning capabilities

#### Common Parameters

All completion parameter types include:
- `model_name`: The specific LLM model to use
- `max_tokens`: Maximum tokens to generate (optional)
- `user_name`: Name of the user interacting with the agent (optional)
- `auth`: Authentication information for the LLM provider (optional)

### Example Configurations

#### Voice-Optimized User Agent

```json
{
  "version": 2,
  "key": "friendly_assistant",
  "name": "Friendly Assistant",
  "agent_description": "A helpful voice-enabled assistant for general user support",
  "model_id": "gpt-4o-realtime",
  "persona": "You are a friendly, helpful assistant optimized for voice conversation...",
  "category": ["realtime", "domo", "general"],
  "tools": ["web_search", "calculator"],
  "agent_params": {
    "type": "g_p_t_non_reasoning",
    "model_name": "gpt-4o",
    "voice": "alloy",
    "temperature": 0.8
  }
}
```

#### Technical Assistant Agent

```json
{
  "version": 2,
  "key": "code_helper",
  "name": "Code Helper",
  "agent_description": "Specialized assistant for code analysis and technical tasks",
  "model_id": "claude-3-5-sonnet",
  "persona": "You are a technical assistant specializing in code analysis...",
  "category": ["lead_developer", "assist", "technical"],
  "tools": ["code_analysis", "git_tools"],
  "blocked_tool_patterns": ["run_*"],
  "allowed_tool_patterns": ["run_git", "run_pytest"],
  "agent_params": {
    "type": "claude_non_reasoning",
    "model_name": "claude-3-5-sonnet-latest",
    "temperature": 0.3
  }
}
```

#### Team Leader Agent

```json
{
  "version": 2,
  "key": "lead_developer", 
  "name": "Lead Developer",
  "agent_description": "Senior developer agent that coordinates with team members",
  "model_id": "claude-3-5-sonnet",
  "persona": "You are a senior developer who coordinates with your team...",
  "category": ["domo", "technical", "leadership"],
  "tools": ["code_review", "agent_team_tools", "project_management"],
  "agent_params": {
    "type": "claude_non_reasoning",
    "model_name": "claude-3-5-sonnet-latest",
    "temperature": 0.5
  }
}
```

### Client Implementation Notes

#### Category-Based Behavior

When implementing clients, consider category-based behavior:

- **`domo` agents**: Can be exposed directly to users in chat interfaces
- **`realtime` agents**: Should be preferred for voice interaction features
- **`assist` agents**: Should not be exposed directly to end users
- **Team relationships**: Agents with other agent keys in their categories are part of those agents' teams

#### Tool Filtering

Agents automatically filter available tools based on their patterns:
1. Tools matching `blocked_tool_patterns` are removed
2. Tools matching `allowed_tool_patterns` are restored (overrides blocks)
3. This allows fine-grained control over agent capabilities

#### Session Context

Agent configurations appear in:
- **Initial connection**: `AgentListEvent` provides all available agents
- **Session changes**: When agents switch during conversation
- **Subsessions**: Agent-to-agent conversations use separate configurations
- **Team interactions**: When agents collaborate using team tools

Understanding these configurations is essential for building clients that can effectively display agent capabilities, handle voice interactions, and visualize complex multi-agent collaboration patterns.

---

## Client → Server Events (Commands)

These are events sent from the client to control the server.

### Agent Management

#### `get_agents`

Request list of available agents.

```json
{
  "type": "get_agents"
}
```

#### `set_agent`

Set the active agent for the session.

```json
{
  "type": "set_agent",
  "agent_key": "default_realtime"
}
```

### Avatar Management

#### `get_avatars`

Request list of available avatars.

```json
{
  "type": "get_avatars"
}
```

#### `set_avatar_session`

Connect to existing avatar session.

```json
{
  "type": "set_avatar_session",
  "access_token": "heygen_session_token",
  "avatar_session_id": "session_uuid"
}
```

### Voice Management

#### `get_voices`

Request list of available voices.

```json
{
  "type": "get_voices"
}
```

#### `set_agent_voice`

Set the TTS voice for the current agent.

```json
{
  "type": "set_agent_voice",
  "voice_id": "alloy"
}
```

### Tool Management

#### `get_tool_catalog`

Request the tool catalog.

```json
{
  "type": "get_tool_catalog"
}
```

### Session Management

#### `get_user_sessions`

Request list of user chat sessions.

```json
{
  "type": "get_user_sessions",
  "offset": 0,
  "limit": 50
}
```

### Connection Health

#### `ping`

Check if the connection is alive.

```json
{
  "type": "ping"
}
```

### Chat Management

#### `text_input`

Send text message to agent.

```json
{
  "type": "text_input",
  "text": "Hello, how are you?",
  "file_ids": ["file-id-1", "file-id-2"] // optional
}
```

#### `new_chat_session`

Create a new chat session.

```json
{
  "type": "new_chat_session",
  "agent_key": "optional_agent_key" // defaults to current agent
}
```

#### `resume_chat_session`

Resume an existing chat session.

```json
{
  "type": "resume_chat_session",
  "session_id": "existing-session-id"
}
```

#### `set_chat_session_name`

Set the name/title for the current session.

```json
{
  "type": "set_chat_session_name",
  "session_name": "My Conversation"
}
```

#### `set_session_metadata`

Update session metadata.

```json
{
  "type": "set_session_metadata",
  "meta": {
    "key1": "value1",
    "key2": "value2"
  }
}
```

#### `set_session_messages`

Replace all messages in the current session.

**Note:** Messages must be in vendor-specific format matching the session's vendor field ("anthropic", "openai", or "none").

```json
{
  "type": "set_session_messages",
  "messages": [
    {
      "role": "user",
      "content": "Hello"
    },
    {
      "role": "assistant", 
      "content": "Hello! How can I help you?"
    }
  ]
}
```

**Vendor-Specific Message Formats:**

*Anthropic (MessageParam format):*
```json
{
  "role": "user",
  "content": "Hello" // string or array of content blocks
}
```

*OpenAI format:*
```json
{
  "role": "user",
  "content": "Hello"
}
```

---

## Server → Client Events (Responses & Updates)

These are events sent from the server to update the client. Events are categorized by their inheritance from BaseEvent:

### Control Events (BaseEvent)

Control events manage system configuration and session lifecycle. They inherit directly from BaseEvent and only contain the base `type` field plus event-specific data.

### Agent Updates

#### `agent_list`

Response to `get_agents` with available agents.

```json
{
  "type": "agent_list",
  "agents": [
    {
      "name": "Agent Name",
      "key": "agent_key", 
      "agent_description": "Agent description|null",
      "category": ["array_of_categories"]
    }
  ]
}
```

#### `agent_configuration_changed`

Notification that agent configuration has changed.

```json
{
  "type": "agent_configuration_changed",
  "agent_config": {
    "version": 2,
    "name": "Agent Name",
    "key": "agent_key",
    "model_id": "model_identifier",
    "agent_description": "Agent description|null",
    "tools": ["tool1", "tool2"],
    "agent_params": { /* CompletionParams object */ },
    "prompt_metadata": { /* metadata object */ },
    "persona": "Agent personality description",
    "uid": "unique_id|null",
    "category": ["array_of_categories"]
  }
}
```

### Avatar Updates

#### `avatar_list`

Response to `get_avatars` with available avatars.

```json
{
  "type": "avatar_list", 
  "avatars": [
    {
      "avatar_id": "string",
      "created_at": 1234567890,
      "default_voice": "voice_id",
      "is_public": true,
      "normal_preview": "preview_url",
      "pose_name": "pose_name",
      "status": "status_string"
    }
  ]
}
```

#### `avatar_connection_changed`

Notification that avatar connection status has changed.

```json
{
  "type": "avatar_connection_changed",
  "avatar_session_request": {
    "avatar_id": "string|null",
    "quality": "string|null", 
    "voice": { /* VoiceSettings object */ },
    "language": "string|null",
    "version": "string|null",
    "video_encoding": "string|null",
    "source": "string|null",
    "stt_settings": { /* STTSettings object */ },
    "ia_is_livekit_transport": "boolean|null",
    "knowledge_base": "string|null",
    "knowledge_base_id": "string|null",
    "disable_idle_timeout": "boolean|null",
    "activity_idle_timeout": "integer|null"
  },
  "avatar_session": {
    "session_id": "string",
    "url": "string",
    "access_token": "string",
    "session_duration_limit": 0,
    "is_paid": true,
    "realtime_endpoint": "string",
    "sdp": "string|null",
    "ice_servers": "array|null",
    "ice_servers2": "array|null"
  }
}
```

### Chat Events

#### `chat_session_changed`

Notification that chat session has been updated.

```json
{
  "type": "chat_session_changed",
  "chat_session": {
    "version": 1,
    "session_id": "session-id",
    "token_count": 0,
    "context_window_size": 0,
    "session_name": "Session Name|null",
    "created_at": "iso_timestamp|null",
    "updated_at": "iso_timestamp|null",
    "deleted_at": "iso_timestamp|null",
    "user_id": "user-id|null",
    "metadata": { /* session metadata dict */ },
    "messages": [ /* array of vendor-specific message dicts */ ],
    "agent_config": { /* CurrentAgentConfiguration object */ },
    "vendor": "anthropic", // computed field: "anthropic", "openai", or "none"
    "display_name": "Session Display Name" // computed field
  }
}
```

**ChatSession Model Details:**

- **`vendor`** (computed): Indicates message format based on agent_config.model_id:
  - `"anthropic"` - For Claude/Bedrock models (Anthropic MessageParam format)
  - `"openai"` - For OpenAI models
  - `"none"` - When no agent_config is present
- **`messages`** - Array of vendor-specific message objects (NO timestamps in individual messages)
- **`display_name`** (computed): Returns session_name if set, otherwise "New chat with {agent_name}"
- **`version`** - Schema version (currently 1)

#### `chat_session_name_changed`

Notification that session name was updated.

```json
{
  "type": "chat_session_name_changed",
  "session_name": "New Session Name"
}
```

#### `session_metadata_changed`

Notification that session metadata was updated.

```json
{
  "type": "session_metadata_changed", 
  "meta": {
    "key1": "updated_value1",
    "key2": "new_value2"
  }
}
```

### Session Events (SessionEvent → BaseEvent)

Session events are generated during chat interactions and contain session context. All session events inherit from SessionEvent and include these fields:
- `session_id` (str): Current chat session identifier
- `role` (str): Role that triggered this event ("user", "assistant", "system")
- `parent_session_id` (str, optional): Parent session ID if this is a child session
- `user_session_id` (str, optional): Top-level user session ID if this is nested

These events are generated by the agent runtime and forwarded to clients.

#### `text_delta`

Streaming text content from the agent.

```json
{
  "type": "text_delta",
  "session_id": "session-id",
  "role": "assistant", 
  "parent_session_id": "parent-session-id",
  "user_session_id": "user-session-id",
  "content": "chunk of text",
  "format": "markdown"
}
```

**Note**: `parent_session_id` and `user_session_id` are present on all session events but may be null for root-level sessions.

#### `thought_delta`

Streaming thought content from the agent (when using thinking tools).

```json
{
  "type": "thought_delta", 
  "session_id": "session-id",
  "role": "assistant (thought)",
  "parent_session_id": "parent-session-id",
  "user_session_id": "user-session-id", 
  "content": "agent thinking process",
  "format": "markdown"
}
```

#### `completion`

Completion status updates.

```json
{
  "type": "completion",
  "session_id": "session-id", 
  "role": "assistant",
  "parent_session_id": "parent-session-id",
  "user_session_id": "user-session-id",
  "running": false,
  "completion_options": { /* vendor-specific options */ },
  "stop_reason": "stop|length|tool_calls",
  "input_tokens": 150,
  "output_tokens": 75
}
```

#### `interaction`

Interaction lifecycle events.

```json
{
  "type": "interaction",
  "session_id": "session-id",
  "role": "assistant",
  "parent_session_id": "parent-session-id",
  "user_session_id": "user-session-id",
  "started": true, // false when completed
  "id": "interaction-uuid"
}
```

#### `history`

Complete message history update.

```json
{
  "type": "history",
  "session_id": "session-id", 
  "role": "system",
  "parent_session_id": "parent-session-id",
  "user_session_id": "user-session-id",
  "vendor": "openai",
  "messages": [
    {
      "role": "user",
      "content": "Hello"
    },
    {
      "role": "assistant",
      "content": "Hello! How can I help you?"
    }
  ]
}
```

**Message Format by Vendor:**

- **`vendor: "anthropic"`** - Messages follow Anthropic MessageParam format with `role` and `content` fields. Content can be string or array of content blocks (text, image, tool_use, etc.)
- **`vendor: "openai"`** - Messages follow OpenAI ChatCompletionMessageParam format (union of user/assistant/system/tool/function/developer message types with complex content support)
- **`vendor: "none"`** - Generic format when no specific vendor is configured

**Important:** Messages do NOT contain timestamps. All temporal information is managed at the session level via `created_at`/`updated_at` fields in the ChatSession object.

#### `tool_call`

Tool execution events.

```json
{
  "type": "tool_call",
  "session_id": "session-id",
  "role": "assistant", 
  "parent_session_id": "parent-session-id",
  "user_session_id": "user-session-id",
  "active": true, // false when completed
  "vendor": "openai",
  "tool_calls": [
    {
      "id": "call_uuid",
      "type": "function",
      "function": {
        "name": "tool_name",
        "arguments": "{\"param\": \"value\"}"
      }
    }
  ],
  "tool_results": [
    {
      "call_id": "call_uuid",
      "output": "tool execution result"
    }
  ]
}
```

#### `system_message`

System notifications and errors.

```json
{
  "type": "system_message",
  "session_id": "session-id",
  "role": "system",
  "parent_session_id": "parent-session-id",
  "user_session_id": "user-session-id",
  "content": "System message content",
  "format": "markdown",
  "severity": "info|warning|error"
}
```

#### `render_media`

**⚠️ SECURITY NOTICE**: Media content with security controls for foreign content.

```json
{
  "type": "render_media",
  "session_id": "session-id",
  "role": "assistant",
  "parent_session_id": "parent-session-id",
  "user_session_id": "user-session-id",
  "content_type": "image/png",
  "url": "https://example.com/image.png",
  "name": "Chart.png",
  "content": "base64_encoded_content",
  "content_bytes": null,
  "sent_by_class": "ToolsetClassName",
  "sent_by_function": "tool_function_name",
  "foreign_content": false
}
```

**Security Fields:**
- `foreign_content` (bool): **CRITICAL SECURITY FIELD**
  - `true`: Content is from third-party sources and **MUST BE TREATED AS UNTRUSTED**. Clients should apply security sanitization, sandboxing, and user warnings.
  - `false`: Content is from trusted internal sources and can be rendered with minimal sanitization.

**Content Delivery:**
- `url`: Direct URL to media content (preferred for foreign content)
- `content`: Base64-encoded media data (for small/internal content)
- `content_bytes`: Raw binary data (internal use)
- `name`: Display name/filename for the media

### Turn Management Events

#### `user_turn_start`

Notification that the server is ready to accept user input (text or audio).

```json
{
  "type": "user_turn_start"
}
```

#### `user_turn_end`

Notification that the server has received user input and is no longer accepting input.

```json
{
  "type": "user_turn_end"
}
```

### Voice Events

#### `voice_list`

Response to `get_voices` with available voices.

```json
{
  "type": "voice_list",
  "voices": [
    {
      "voice_id": "none",
      "vendor": "system",
      "description": "No Voice (text only)",
      "output_format": "none"
    },
    {
      "voice_id": "avatar",
      "vendor": "heygen",
      "description": "HeyGen Avatar Voice Model",
      "output_format": "special"
    },
    {
      "voice_id": "alloy",
      "vendor": "openai",
      "description": "OpenAI Alloy voice",
      "output_format": "pcm16"
    }
  ]
}
```

#### `agent_voice_changed`

Notification that the agent's voice has been updated.

```json
{
  "type": "agent_voice_changed",
  "voice": {
    "voice_id": "alloy",
    "vendor": "openai",
    "description": "OpenAI Alloy voice",
    "output_format": "pcm16"
  }
}
```

### Tool Events

#### `tool_catalog`

Response to `get_tool_catalog` with available tools.

```json
{
  "type": "tool_catalog",
  "tools": [
    {
      "name": "toolset_name",
      "description": "Description of the toolset functionality",
      "schemas": {
        "tool_name": {
          "type": "function",
          "function": {
            "name": "tool_name",
            "description": "Tool description",
            "parameters": {
              "type": "object",
              "properties": {
                "param_name": {
                  "type": "string",
                  "description": "Parameter description"
                }
              },
              "required": ["param_name"]
            }
          }
        }
      }
    }
  ]
}
```

### User Events

#### `chat_user_data`

Sent during initialization with current user information.

```json
{
  "type": "chat_user_data",
  "user": {
    "user_id": "string",
    "user_name": "string",
    "email": "string|null",
    "first_name": "string|null",
    "last_name": "string|null",
    "is_active": true,
    "roles": ["array_of_strings"],
    "groups": ["array_of_strings"],
    "created_at": "iso_timestamp|null",
    "last_login": "iso_timestamp|null"
  }
}
```

#### `get_user_sessions_response`

Response to `get_user_sessions` with user's chat sessions.

```json
{
  "type": "get_user_sessions_response",
  "sessions": {
    "chat_sessions": [
      {
        "session_id": "session-id",
        "session_name": "Session Name|null",
        "created_at": "iso_timestamp",
        "updated_at": "iso_timestamp",
        "user_id": "user-id",
        "agent_key": "agent_key|null",
        "agent_name": "Agent Name|null"
      }
    ],
    "total_sessions": 10,
    "offset": 0
  }
}
```

**Note:** Sessions list uses `ChatSessionIndexEntry` model which contains lightweight session metadata without the full message history. The full `ChatSession` object with messages is only provided in `chat_session_changed` events.

### Connection Events

#### `pong`

Response to `ping` event for connection health check.

```json
{
  "type": "pong"
}
```

### Error Events

#### `error`

Error notifications.

```json
{
  "type": "error",
  "message": "Error description",
  "source": "component_name" // optional
}
```

---

## Binary Audio Streaming

The WebSocket API supports binary audio packets for low-latency voice interaction:

### Audio Input

- **Binary data sent to server** via WebSocket is interpreted as PCM audio chunks for agent input
- **Format:** Raw PCM audio data (16-bit, mono, 16kHz recommended)
- **Processing:** Audio is processed in real-time for speech-to-text conversion

### Audio Output

- **Binary data from server** contains TTS audio chunks being streamed from the text-to-speech engine
- **Format:** Depends on the selected voice model's `output_format` field
  - OpenAI voices: `pcm16` format
  - HeyGen avatar: handled by HeyGen SDK (no binary output)
  - No voice: no audio output

### Implementation Example

```javascript
// Send audio input
const audioData = new Uint8Array(pcmBuffer);
websocket.send(audioData);

// Receive audio output
websocket.onmessage = (event) => {
  if (event.data instanceof Blob || event.data instanceof ArrayBuffer) {
    // This is binary audio data from TTS
    playAudioChunk(event.data);
  } else {
    // This is JSON event data
    const jsonEvent = JSON.parse(event.data);
    handleEvent(jsonEvent);
  }
};
```

### Audio Processing Pipeline

1. **Client → Server:** Raw PCM audio chunks via binary WebSocket messages
2. **Server Processing:** Speech-to-text conversion using configured STT engine
3. **Agent Processing:** Text input processed by AI agent
4. **TTS Generation:** Agent response converted to speech (if voice enabled)
5. **Server → Client:** Binary audio chunks streamed back via WebSocket

### Special Voice Models

The system includes special voice models for specific use cases:

#### Avatar Voice Model (`voice_id: "avatar"`)

- **Purpose:** Indicates HeyGen streaming avatar handles both audio and video
- **Behavior:** No binary audio sent from server to client
- **Integration:** Agent C sends "repeat" tasks to HeyGen avatar for speech synthesis
- **Usage:** Automatically set when avatar session is active

```json
{
  "voice_id": "avatar",
  "vendor": "heygen",
  "description": "HeyGen Avatar Voice Model",
  "output_format": "special"
}
```

#### No Voice Model (`voice_id: "none"`)

- **Purpose:** Text-only mode with no TTS output
- **Behavior:** Agent responses displayed as text only
- **Usage:** For clients that don't want voice output

```json
{
  "voice_id": "none",
  "vendor": "system",
  "description": "No Voice (text only)",
  "output_format": "none"
}
```

---

## Implementation Patterns

### Client Connection Flow

```javascript
// 1. Authenticate
const loginResponse = await fetch('/rt/login', {
  method: 'POST',
  body: JSON.stringify({ username, password })
});
const { agent_c_token, ui_session_id } = await loginResponse.json();

// 2. Connect WebSocket (HTTPS required)
const ws = new WebSocket(`wss://host/rt/ws?token=${agent_c_token}&session_id=${ui_session_id}`);

// 3. Handle events (both JSON and binary)
ws.onmessage = (event) => {
  if (event.data instanceof Blob || event.data instanceof ArrayBuffer) {
    // Binary audio data from TTS
    handleAudioOutput(event.data);
  } else {
    // JSON event data
    const data = JSON.parse(event.data);
    handleServerEvent(data);
  }
};

// 4. Send text commands
ws.send(JSON.stringify({
  type: 'text_input',
  text: 'Hello!'
}));

// 5. Send binary audio (PCM format)
const audioData = new Uint8Array(pcmBuffer);
ws.send(audioData);
```

### Event Handling

```javascript
function handleServerEvent(event) {
  switch(event.type) {
    case 'text_delta':
      appendToMessage(event.content);
      break;
    case 'completion':
      if (!event.running) {
        enableUserInput();
      }
      break;
    case 'user_turn_start':
      enableUserInput();
      break;
    case 'user_turn_end':
      disableUserInput();
      showProcessingIndicator();
      break;
    case 'agent_voice_changed':
      updateVoiceSettings(event.voice);
      break;
    case 'avatar_connection_changed':
      // Handle avatar connection status change
      // Implementation depends on your avatar integration
      break;
    case 'error':
      showError(event.message);
      break;
    // ... handle other events
  }
}

function handleAudioOutput(audioData) {
  // Play received audio data from TTS
  playAudioChunk(audioData);
}

// Send audio input
function sendAudioInput(pcmAudioBuffer) {
  const audioData = new Uint8Array(pcmAudioBuffer);
  ws.send(audioData);
}
```

---

## Authentication & Security

### JWT Token Structure

```json
{
  "user_id": "user-uuid",
  "permissions": ["permission1", "permission2"],
  "exp": 1234567890,
  "iat": 1234567890
}
```

### Token Refresh

- JWT tokens expire after a set time
- Use `/rt/refresh_token` endpoint to get new tokens
- HeyGen tokens also need periodic refresh
- Implement automatic token refresh in clients

---

## Error Handling

### Common Error Scenarios

1. **Authentication Failure**
   
   - Invalid credentials → 401 response
   - Expired token → WebSocket disconnect + 401

2. **Session Errors**
   
   - Invalid session_id → Error event
   - Session access denied → Error event  

3. **Avatar Errors**
   
   - Avatar creation failed → Error event
   - Avatar session timeout → Error event

4. **Runtime Errors**
   
   - Agent processing error → Error event with details
   - Tool execution failure → Error event

### Error Recovery

- Implement exponential backoff for reconnections
- Handle token refresh automatically  
- Gracefully degrade when avatar features fail
- Provide user feedback for all error states

---

## Performance Considerations

### Message Buffering

- `text_delta` events arrive rapidly during agent responses
- Implement client-side buffering to avoid UI blocking
- Consider debouncing UI updates for smooth rendering

### Connection Management

- Monitor WebSocket connection health
- Implement ping/pong heartbeat if needed
- Handle network disconnections gracefully
- Cache session state for reconnection

---

## Additional References

### Core Components

- **`//api/src/agent_c_api/core/realtime_bridge.py`** - WebSocket event handler and main bridge implementation
- **`//api/src/agent_c_api/core/voice/voice_io_manager.py`** - Voice input/output management and audio pipeline
- **`//api/src/agent_c_api/api/rt/session.py`** - Reduced footprint REST endpoints for authentication
- **`//api/src/agent_c_api/api/rt/models/client_events.py`** - Complete event model definitions

### Voice System

- **`//api/src/agent_c_api/core/voice/models.py`** - Available voice model definitions
- **`//api/src/agent_c_api/core/voice/rt_bridge_workflow.py`** - Voice processing workflow integration

### Client Tool Integration

- **`//core/src/agent_c/models/client_tool_info.py`** - Tool information models for client consumption

---

## Development Tips

1. **Event Registration** - All events auto-register with the EventRegistry
2. **Type Safety** - Use Pydantic models for strong typing
3. **Logging** - All components use structured logging
4. **Testing** - Mock WebSocket connections for testing
5. **Documentation** - Events are self-documenting via Pydantic

This guide provides the complete specification for implementing clients that interact with the Agent C Realtime API. The event-driven architecture allows for flexible, responsive real-time applications with avatar integration support.