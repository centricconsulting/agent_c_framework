# Agent C API v2 Migration Guide

## Migration Checklist

Use this checklist to track your migration progress:

### 1. Assessment & Planning
- [ ] Inventory all v1 API endpoints currently in use
- [ ] Map v1 endpoints to their v2 equivalents using this guide
- [ ] Identify any gaps or breaking changes that will affect your application
- [ ] Create a migration timeline based on your application's needs
- [ ] Establish testing criteria for validating the migration

### 2. Development & Testing
- [ ] Update API client library or create a v2-compatible wrapper
- [ ] Migrate configuration endpoint calls (models, personas, tools)
- [ ] Update session management (creation, verification, listing, deletion)
- [ ] Adapt chat functionality to the new message format and streaming responses
- [ ] Update file handling code for the new endpoint structure
- [ ] Migrate history access and replay functionality
- [ ] Implement new features available only in v2 (if applicable)
- [ ] Run comprehensive tests comparing v1 and v2 behavior

### 3. Deployment & Monitoring
- [ ] Deploy v2 endpoints in a staging environment
- [ ] Validate end-to-end application flows
- [ ] Implement appropriate error handling for v2-specific responses
- [ ] Create rollback plan in case of issues
- [ ] Deploy to production with monitoring
- [ ] Deprecate v1 code once migration is complete

## Introduction

This guide is designed to help developers transition from the Agent C API v1 to v2. The v2 API represents a significant redesign focused on providing a more consistent, RESTful, and maintainable API while preserving the core functionality available in v1.

### Why a New API Version?

The decision to create a new major version of the API was driven by several factors:

1. **Improved Architecture**: The v2 API follows RESTful design principles more closely, with resource-oriented endpoints and appropriate HTTP methods

2. **Enhanced Consistency**: The v1 API evolved organically, leading to inconsistent naming conventions and response formats; v2 standardizes these patterns

3. **Better Developer Experience**: Structured responses, comprehensive documentation, and predictable behavior make the v2 API easier to work with

4. **Future Extensibility**: The new design provides a more flexible foundation for adding new features without breaking changes

5. **Performance Improvements**: Optimized endpoints and more efficient data handling improve overall API performance

While this means changes to how you interact with the API, the benefits of migrating to v2 include improved reliability, better documentation, and access to new features that will only be available in v2.

## API Overview

### Major Changes

- **RESTful Resource Structure**: The v2 API is organized around clear resource types with consistent HTTP methods
- **Consistent Naming**: Resources use clear, descriptive names that accurately reflect the domain
- **Logical Grouping**: Related endpoints are grouped under appropriate resource paths
- **Enhanced Validation**: Improved Pydantic models for request/response validation
- **Comprehensive Documentation**: Detailed OpenAPI documentation for all endpoints
- **Proper Status Codes**: Appropriate HTTP status codes for different responses

### API Structure

The v2 API is organized into four main resource categories:

1. **Configuration Resources** (`/api/v2/config/*`)
   - Read-only resources providing system configuration information
   - Models, personas, tools, and system configuration

2. **Session Resources** (`/api/v2/sessions/*`)
   - Resources for managing chat sessions and interactions
   - Session CRUD operations, agent configuration, chat messaging, file management

3. **History Resources** (`/api/v2/history/*`)
   - Resources for accessing and replaying past interactions
   - History listing, event access, streaming, and replay controls

4. **Debug Resources** (`/api/v2/debug/*`)
   - Development resources for debugging and troubleshooting
   - Detailed session and agent state information

## Migration Strategies

### Recommended Approaches

We offer three recommended migration paths depending on your specific needs:

#### 1. Phased Resource Migration

This approach involves migrating one resource type at a time, allowing for controlled transitions with minimal risk.

**Recommended Migration Order**:
1. **Configuration Resources**: Begin with these read-only endpoints that have the simplest mapping
2. **Session Management**: Update core session CRUD operations
3. **Agent Configuration**: Migrate agent and tool settings updates
4. **Chat Functionality**: Update the core messaging features
5. **File Handling**: Migrate file upload and management
6. **History Access**: Update history and replay components
7. **Debug Features**: Implement new debugging capabilities

**Pros**:
- Allows for incremental testing and validation
- Minimizes the scope of changes at each step
- Easier to roll back if issues are encountered

**Cons**:
- Requires maintaining two API client implementations during transition
- Longer overall migration timeline

#### 2. Full Microservice/Component Migration

This approach involves migrating entire application components or microservices all at once to v2.

**Approach**:
1. Select an isolated component of your application
2. Implement all v2 endpoints required by that component
3. Test the component thoroughly with v2 endpoints
4. Switch the component to use v2 exclusively
5. Repeat for other components

**Pros**:
- Cleaner implementation with less mixed API version code
- Shorter transition period for each component
- Better conceptual alignment with application architecture

**Cons**:
- Higher risk for each migration step
- Requires more upfront planning and analysis

#### 3. Parallel Implementation

This approach involves building a completely new client implementation for v2 alongside your existing v1 implementation.

**Approach**:
1. Create a new API client library for v2
2. Implement all required functionality using v2 endpoints
3. Test comprehensively against the v2 API
4. Switch applications to the new client once validated

**Pros**:
- Cleanest implementation with no mixed API versions
- Opportunity to improve client code architecture
- Easier testing and validation of the entire flow

**Cons**:
- Requires the most upfront investment
- Duplicate code during transition period

### Testing Strategy

For a smooth migration, follow these testing practices:

#### 1. Comprehensive Test Coverage

- Create test cases that cover all migrated endpoints
- Test both happy paths and error scenarios
- Validate all response fields and formats
- Test pagination, filtering, and other parameter variations

#### 2. Parallel Endpoint Validation

- Create tests that compare v1 and v2 responses for equivalent functionality
- Verify data consistency between versions
- Document any discrepancies for application code updates

#### 3. Integration Testing

- Test complete user flows using the v2 API
- Validate multi-endpoint interactions (e.g., session creation → chat → history)
- Test streaming responses under various network conditions

#### 4. Feature Parity Verification

- Create a checklist of all v1 features and their v2 equivalents
- Track migration progress against feature requirements
- Identify and plan for handling removed or significantly changed features

### Backward Compatibility Considerations

During the transition period:

- Both v1 and v2 APIs will be available simultaneously
- No breaking changes will be made to the v1 API without advance notice
- New features will be added primarily to the v2 API
- Applications should plan to complete migration before v1 deprecation

### Sharing Data Between v1 and v2

Sessions created in one API version are not directly accessible through the other version. If you need to access the same sessions from both v1 and v2 during migration:

1. **For History Access**: Create sessions in v1, then access their history using v2 history endpoints by accessing the same session ID
2. **For Live Sessions**: Maintain separate session management in your application during the transition period
3. **For File Resources**: Files may need to be re-uploaded when switching API versions for the same interaction

## Endpoint Mapping

This section provides detailed mapping between v1 and v2 endpoints, including changes in URL structure, HTTP methods, request parameters, and response formats.

### Configuration Endpoints

#### Models Endpoint

| v1 Endpoint | v2 Endpoint | HTTP Method |
|-------------|-------------|-------------|
| `/api/v1/models` | `/api/v2/config/models` | GET |

**Request Changes**:
- No changes in request parameters

**Response Changes**:
- Response is now wrapped in an `APIResponse` object with `data` field containing the models list
- Additional metadata in response (status, message) 
- More consistent model properties

**Example Migration**:

```python
# v1 API call
async def get_models_v1(client):
    response = await client.get("/api/v1/models")
    return response.json()

# v2 API call
async def get_models_v2(client):
    response = await client.get("/api/v2/config/models")
    response_data = response.json()
    return response_data["data"]  # Access the data field of the APIResponse
```

#### Personas Endpoint

| v1 Endpoint | v2 Endpoint | HTTP Method |
|-------------|-------------|-------------|
| `/api/v1/personas` | `/api/v2/config/personas` | GET |

**Request Changes**:
- No changes in request parameters

**Response Changes**:
- Response is now wrapped in an `APIResponse` object with `data` field containing the personas list
- Additional metadata in response (status, message)
- More consistent persona properties

**Example Migration**:

```python
# v1 API call
async def get_personas_v1(client):
    response = await client.get("/api/v1/personas")
    return response.json()

# v2 API call
async def get_personas_v2(client):
    response = await client.get("/api/v2/config/personas")
    response_data = response.json()
    return response_data["data"]  # Access the data field of the APIResponse
```

#### Tools Endpoint

| v1 Endpoint | v2 Endpoint | HTTP Method |
|-------------|-------------|-------------|
| `/api/v1/tools` | `/api/v2/config/tools` | GET |

**Request Changes**:
- No changes in request parameters

**Response Changes**:
- Response is now wrapped in an `APIResponse` object with `data` field containing the tools list
- Additional metadata in response (status, message)
- More consistent tool properties and categorization

**Example Migration**:

```python
# v1 API call
async def get_tools_v1(client):
    response = await client.get("/api/v1/tools")
    return response.json()

# v2 API call
async def get_tools_v2(client):
    response = await client.get("/api/v2/config/tools")
    response_data = response.json()
    return response_data["data"]  # Access the data field of the APIResponse
```

#### New: System Configuration Endpoint

The v2 API introduces a new combined configuration endpoint with no v1 equivalent:

| v1 Endpoint | v2 Endpoint | HTTP Method |
|-------------|-------------|-------------|
| N/A | `/api/v2/config/system` | GET |

**Benefits**:
- Retrieves all configuration information in a single request
- Reduces the number of API calls needed for initialization

**Example Usage**:

```python
async def get_system_config(client):
    response = await client.get("/api/v2/config/system")
    response_data = response.json()
    
    # Access individual configuration components
    models = response_data["data"]["models"]
    personas = response_data["data"]["personas"]
    tools = response_data["data"]["tools"]
    
    return response_data["data"]
```

## Session Management Endpoints

Session management has been significantly restructured in v2 to follow RESTful conventions and provide more consistent behavior.

### Session Creation

| v1 Endpoint | v2 Endpoint | HTTP Method |
|-------------|-------------|-------------|
| `/api/v1/initialize` | `/api/v2/sessions` | POST |

**Request Changes**:
- `AgentInitializationParams` is now `SessionCreate`
- `model_name` is now `model_id`
- `persona_name` is now `persona_id`
- `ui_session_id` for transferring history is no longer supported (use history API instead)
- Added `name` field for giving sessions a user-friendly name

**Response Changes**:
- Session IDs are now UUIDs instead of strings
- Returns a `SessionDetail` object with comprehensive session information
- HTTP status code 201 (Created) instead of 200 (OK)

**Example Migration**:

```python
# v1 API call
async def create_session_v1(client, model_name, persona_name="default", temperature=0.7):
    payload = {
        "model_name": model_name,
        "agent_key": persona_name,
        "temperature": temperature,
        "backend": "openai"
    }
    response = await client.post("/api/v1/initialize", json=payload)
    data = response.json()
    return data["ui_session_id"]

# v2 API call
async def create_session_v2(client, model_id, persona_id="default", temperature=0.7):
    payload = {
        "model_id": model_id,
        "persona_id": persona_id,
        "temperature": temperature,
        "name": "New Session"  # Optional but recommended
    }
    response = await client.post("/api/v2/sessions", json=payload)
    data = response.json()
    return str(data["id"])  # UUID is returned
```

### Session Verification

| v1 Endpoint | v2 Endpoint | HTTP Method |
|-------------|-------------|-------------|
| `/api/v1/verify_session/{ui_session_id}` | `/api/v2/sessions/{session_id}` | GET |

**Request Changes**:
- `ui_session_id` is now `session_id` and must be a valid UUID

**Response Changes**:
- Instead of a boolean `valid` field, v2 returns the full session details
- HTTP 404 status code if session doesn't exist

**Example Migration**:

```python
# v1 API call
async def verify_session_v1(client, session_id):
    response = await client.get(f"/api/v1/verify_session/{session_id}")
    data = response.json()
    return data["valid"]

# v2 API call
async def verify_session_v2(client, session_id):
    try:
        response = await client.get(f"/api/v2/sessions/{session_id}")
        return True  # Session exists if we get a 200 response
    except Exception as e:
        if hasattr(e, 'status') and e.status == 404:
            return False  # Session doesn't exist
        raise  # Re-raise other exceptions
```

### Listing Sessions (New in v2)

| v1 Endpoint | v2 Endpoint | HTTP Method |
|-------------|-------------|-------------|
| N/A | `/api/v2/sessions` | GET |

The v2 API introduces the ability to list all active sessions, which wasn't available in v1:

**Request Parameters**:
- `limit`: Maximum number of sessions to return (default: 10)
- `offset`: Number of sessions to skip for pagination (default: 0)

**Response Format**:
- Returns a `SessionListResponse` with paginated results
- List of sessions with basic details in `items` array
- Includes `total`, `limit`, and `offset` for pagination

**Example Usage**:

```python
async def list_sessions(client, limit=10, offset=0):
    response = await client.get(
        "/api/v2/sessions",
        params={"limit": limit, "offset": offset}
    )
    data = response.json()
    
    # Process results
    sessions = data["items"]
    total = data["total"]
    
    return {
        "sessions": sessions,
        "total": total,
        "has_more": total > (offset + limit)
    }
```

### Updating Sessions (New in v2)

| v1 Endpoint | v2 Endpoint | HTTP Method |
|-------------|-------------|-------------|
| N/A | `/api/v2/sessions/{session_id}` | PATCH |

The v2 API introduces the ability to update session properties:

**Request Body**:
- `SessionUpdate` model with fields to update
- All fields are optional - only specified fields will be updated

**Response Format**:
- Returns the updated `SessionDetail` object

**Example Usage**:

```python
async def update_session(client, session_id, name=None, metadata=None):
    update_data = {}
    if name is not None:
        update_data["name"] = name
    if metadata is not None:
        update_data["metadata"] = metadata
        
    response = await client.patch(
        f"/api/v2/sessions/{session_id}",
        json=update_data
    )
    
    return response.json()
```

### Deleting Sessions

| v1 Endpoint | v2 Endpoint | HTTP Method |
|-------------|-------------|-------------|
| `/api/v1/sessions` (deletes all) | `/api/v2/sessions/{session_id}` | DELETE |

**Breaking Change**: The v1 API had a single endpoint to delete all sessions, while v2 allows deleting specific sessions but doesn't provide a "delete all" endpoint.

**Request Changes**:
- Now requires a specific session ID to delete

**Response Changes**:
- Returns HTTP 204 (No Content) on success instead of a JSON object

**Example Migration**:

```python
# v1 API call to delete all sessions
async def delete_all_sessions_v1(client):
    response = await client.delete("/api/v1/sessions")
    data = response.json()
    return data["deleted_count"]

# v2 API call to delete a specific session
async def delete_session_v2(client, session_id):
    response = await client.delete(f"/api/v2/sessions/{session_id}")
    return response.status_code == 204  # True if successful

# v2 function to delete all sessions by listing and deleting each
async def delete_all_sessions_v2(client):
    # List all sessions
    response = await client.get("/api/v2/sessions", params={"limit": 100})
    data = response.json()
    
    # Delete each session
    deleted_count = 0
    for session in data["items"]:
        session_id = session["id"]
        delete_response = await client.delete(f"/api/v2/sessions/{session_id}")
        if delete_response.status_code == 204:
            deleted_count += 1
    
    return deleted_count
```

### Agent Configuration

| v1 Endpoint | v2 Endpoint | HTTP Method |
|-------------|-------------|-------------|
| `/api/v1/get_agent_config/{ui_session_id}` | `/api/v2/sessions/{session_id}/agent` | GET |
| `/api/v1/update_settings` | `/api/v2/sessions/{session_id}/agent` | PATCH |

**Request Changes**:
- v1 used a separate `ui_session_id` parameter, v2 includes it in the URL path
- `persona_name` is now `persona_id`
- Request structure is now a simple JSON object with parameter fields

**Response Changes**:
- More consistent structure with all agent parameters in one object
- Updates return an `AgentUpdateResponse` with change tracking

**Example Migration**:

```python
# v1 API call to get agent config
async def get_agent_config_v1(client, session_id):
    response = await client.get(f"/api/v1/get_agent_config/{session_id}")
    data = response.json()
    return data["config"]

# v2 API call to get agent config
async def get_agent_config_v2(client, session_id):
    response = await client.get(f"/api/v2/sessions/{session_id}/agent")
    return response.json()

# v1 API call to update agent settings
async def update_agent_settings_v1(client, session_id, temperature=None, persona_name=None):
    update_data = {"ui_session_id": session_id}
    if temperature is not None:
        update_data["temperature"] = temperature
    if persona_name is not None:
        update_data["agent_key"] = persona_name
        
    response = await client.post("/api/v1/update_settings", json=update_data)
    return response.json()

# v2 API call to update agent settings
async def update_agent_settings_v2(client, session_id, temperature=None, persona_id=None):
    update_data = {}
    if temperature is not None:
        update_data["temperature"] = temperature
    if persona_id is not None:
        update_data["persona_id"] = persona_id
        
    response = await client.patch(
        f"/api/v2/sessions/{session_id}/agent",
        json=update_data
    )
    
    return response.json()
```

### Tool Management

| v1 Endpoint | v2 Endpoint | HTTP Method |
|-------------|-------------|-------------|
| `/api/v1/get_agent_tools/{ui_session_id}` | `/api/v2/sessions/{session_id}/tools` | GET |
| `/api/v1/update_tools` | `/api/v2/sessions/{session_id}/tools` | PUT |

**Request Changes**:
- v1 used a separate `ui_session_id` parameter, v2 includes it in the URL path
- Tool update now uses a simpler array format

**Response Changes**:
- More consistent structure for tool listings
- HTTP 204 (No Content) on successful updates

**Example Migration**:

```python
# v1 API call to get agent tools
async def get_agent_tools_v1(client, session_id):
    response = await client.get(f"/api/v1/get_agent_tools/{session_id}")
    data = response.json()
    return data["initialized_tools"]

# v2 API call to get agent tools
async def get_agent_tools_v2(client, session_id):
    response = await client.get(f"/api/v2/sessions/{session_id}/tools")
    data = response.json()
    return data["tools"]

# v1 API call to update tools
async def update_tools_v1(client, session_id, tool_list):
    update_data = {
        "ui_session_id": session_id,
        "tools": tool_list
    }
    response = await client.post("/api/v1/update_tools", json=update_data)
    return response.json()

# v2 API call to update tools
async def update_tools_v2(client, session_id, tool_list):
    response = await client.put(
        f"/api/v2/sessions/{session_id}/tools",
        json=tool_list  # Just send the array directly
    )
    
    return response.status_code == 204  # True if successful
```

## Chat & Files Endpoints

The chat and file handling endpoints have been redesigned to provide a more structured approach to managing conversations and file attachments in sessions.

### Chat Functionality

#### Sending Chat Messages

| v1 Endpoint | v2 Endpoint | HTTP Method |
|-------------|-------------|-------------|
| `/api/v1/chat` (form data) | `/api/v2/sessions/{session_id}/chat` (JSON) | POST |

**Request Changes**:
- Form-based request (`ui_session_id`, `message`, `file_ids`) → JSON-based request with structured message content
- Uses JSON format for message content with support for different content types
- More explicit file attachment references
- Added `stream` parameter to control response streaming (always `true` for now)

**Response Changes**:
- Response format is now a stream of JSON events instead of raw text chunks
- Each event has a specific type and structured content
- Events include text additions, tool calls, and completion status
- Improved error reporting with specific status codes

**Example Migration**:

```python
# v1 API call
async def send_chat_v1(client, session_id, message, file_ids=None):
    form_data = {
        "ui_session_id": session_id,
        "message": message
    }
    if file_ids:
        form_data["file_ids"] = json.dumps(file_ids)
        
    async with client.stream("POST", "/api/v1/chat", data=form_data) as response:
        full_response = ""
        async for chunk in response.aiter_text():
            full_response += chunk
            # Process each chunk as it arrives
            print(chunk, end="")
        return full_response

# v2 API call
async def send_chat_v2(client, session_id, message, file_ids=None):
    # Construct the chat message with content parts
    content = [{"type": "text", "text": message}]
    
    # Add file references if provided
    if file_ids:
        for file_id in file_ids:
            content.append({"type": "file", "file_id": file_id})
    
    # Create the request body
    request_body = {
        "message": {
            "role": "user",
            "content": content
        },
        "stream": True
    }
    
    # Stream the response
    async with client.stream(
        "POST", 
        f"/api/v2/sessions/{session_id}/chat", 
        json=request_body
    ) as response:
        full_response = ""
        text_content = ""
        
        # Process each event
        async for chunk in response.aiter_text():
            if not chunk.strip():
                continue
                
            # Parse the JSON event
            event = json.loads(chunk)
            event_type = event.get("type")
            
            # Handle different event types
            if event_type == "text_delta":
                text_piece = event.get("content", "")
                text_content += text_piece
                print(text_piece, end="")
            elif event_type == "tool_call":
                print(f"\n[Tool Call: {event.get('name')}]\n")
            elif event_type == "completion":
                if event.get("status") == "complete":
                    print("\n[Response Complete]")
                    
        return text_content
```

#### Cancelling Chat Interactions

| v1 Endpoint | v2 Endpoint | HTTP Method |
|-------------|-------------|-------------|
| `/api/v1/cancel` (form data) | `/api/v2/sessions/{session_id}/chat` | DELETE |

**Request Changes**:
- Form-based request (`ui_session_id`) → RESTful DELETE on chat resource
- Session ID now in URL path instead of form data

**Response Changes**:
- Consistent JSON response format with status and message
- Same status codes for error conditions

**Example Migration**:

```python
# v1 API call
async def cancel_chat_v1(client, session_id):
    response = await client.post(
        "/api/v1/cancel", 
        data={"ui_session_id": session_id}
    )
    return response.json()

# v2 API call
async def cancel_chat_v2(client, session_id):
    response = await client.delete(f"/api/v2/sessions/{session_id}/chat")
    return response.json()
```

### File Management

#### Uploading Files

| v1 Endpoint | v2 Endpoint | HTTP Method |
|-------------|-------------|-------------|
| `/api/v1/upload_file` | `/api/v2/sessions/{session_id}/files` | POST |

**Request Changes**:
- Form field `ui_session_id` → session ID in URL path
- Response structure is more consistent and detailed
- HTTP 201 (Created) status code instead of 200 (OK)

**Response Changes**:
- Field naming changes: `id` → `file_id`, `mime_type` → `content_type`
- HTTP 201 status code on successful creation

**Example Migration**:

```python
# v1 API call
async def upload_file_v1(client, session_id, file_path):
    filename = os.path.basename(file_path)
    
    with open(file_path, "rb") as f:
        files = {"file": (filename, f)}
        form_data = {"ui_session_id": session_id}
        
        response = await client.post(
            "/api/v1/upload_file",
            data=form_data,
            files=files
        )
        
    return response.json()

# v2 API call
async def upload_file_v2(client, session_id, file_path):
    filename = os.path.basename(file_path)
    
    with open(file_path, "rb") as f:
        files = {"file": (filename, f)}
        
        response = await client.post(
            f"/api/v2/sessions/{session_id}/files",
            files=files
        )
        
    return response.json()
```

#### Listing Files

| v1 Endpoint | v2 Endpoint | HTTP Method |
|-------------|-------------|-------------|
| `/api/v1/files/{ui_session_id}` | `/api/v2/sessions/{session_id}/files` | GET |

**Request Changes**:
- Session ID now follows RESTful pattern in URL path

**Response Changes**:
- Files are returned as a direct array instead of nested under a "files" property
- More consistent file metadata structure with clearer property names
- Additional metadata about processing status is more structured

**Example Migration**:

```python
# v1 API call
async def list_files_v1(client, session_id):
    response = await client.get(f"/api/v1/files/{session_id}")
    data = response.json()
    return data["files"]

# v2 API call
async def list_files_v2(client, session_id):
    response = await client.get(f"/api/v2/sessions/{session_id}/files")
    return response.json()  # Already returns array of files
```

#### Getting File Metadata

| v1 Endpoint | v2 Endpoint | HTTP Method |
|-------------|-------------|-------------|
| N/A | `/api/v2/sessions/{session_id}/files/{file_id}` | GET |

**New Endpoint**:
The v2 API introduces a dedicated endpoint for retrieving metadata about a specific file, which wasn't available in v1.

**Example Usage**:

```python
async def get_file_metadata(client, session_id, file_id):
    response = await client.get(
        f"/api/v2/sessions/{session_id}/files/{file_id}"
    )
    return response.json()
```

#### Downloading Files

| v1 Endpoint | v2 Endpoint | HTTP Method |
|-------------|-------------|-------------|
| `/api/v1/files/{ui_session_id}/{file_id}` | `/api/v2/sessions/{session_id}/files/{file_id}/content` | GET |

**Request Changes**:
- URL structure follows RESTful conventions with `/content` suffix
- Session and file IDs in URL path follow consistent pattern

**Response Changes**:
- No significant changes to the response format
- Both return file content with appropriate Content-Type headers

**Example Migration**:

```python
# v1 API call
async def download_file_v1(client, session_id, file_id):
    response = await client.get(
        f"/api/v1/files/{session_id}/{file_id}",
        stream=True
    )
    return response

# v2 API call
async def download_file_v2(client, session_id, file_id):
    response = await client.get(
        f"/api/v2/sessions/{session_id}/files/{file_id}/content",
        stream=True
    )
    return response
```

#### Deleting Files

| v1 Endpoint | v2 Endpoint | HTTP Method |
|-------------|-------------|-------------|
| `/api/v1/files/{ui_session_id}/{file_id}` | `/api/v2/sessions/{session_id}/files/{file_id}` | DELETE |

**Request Changes**:
- URL structure follows more consistent RESTful conventions

**Response Changes**:
- v1 returned JSON message on success
- v2 returns HTTP 204 (No Content) on success with no body

**Example Migration**:

```python
# v1 API call
async def delete_file_v1(client, session_id, file_id):
    response = await client.delete(
        f"/api/v1/files/{session_id}/{file_id}"
    )
    return response.json()["message"]  # Returns success message

# v2 API call
async def delete_file_v2(client, session_id, file_id):
    response = await client.delete(
        f"/api/v2/sessions/{session_id}/files/{file_id}"
    )
    return response.status_code == 204  # Returns true if deletion successful
```

## History & Debug Endpoints

The history and debug endpoints have been significantly enhanced in v2, providing more structured access to session history and debugging information.

### History Endpoints

#### Session History Listing

| v1 Endpoint | v2 Endpoint | HTTP Method |
|-------------|-------------|-------------|
| N/A | `/api/v2/history` | GET |

**New in v2**:
The v2 API introduces a dedicated endpoint for listing session histories which wasn't available in v1. This provides a way to discover available session histories for replay or analysis.

**Request Parameters**:
- `limit`: Maximum number of histories to return (1-100)
- `offset`: Number of histories to skip for pagination
- `sort_by`: Field to sort by (e.g., 'start_time', 'name', 'message_count')
- `sort_order`: Sort order ('asc' or 'desc')

**Example Usage**:

```python
async def list_histories(client, limit=50, offset=0, sort_by='start_time', sort_order='desc'):
    params = {
        'limit': limit,
        'offset': offset,
        'sort_by': sort_by,
        'sort_order': sort_order
    }
    response = await client.get('/api/v2/history', params=params)
    return response.json()
```

#### Session History Details

| v1 Endpoint | v2 Endpoint | HTTP Method |
|-------------|-------------|-------------|
| N/A | `/api/v2/history/{session_id}` | GET |

**New in v2**:
The v2 API introduces a dedicated endpoint for retrieving detailed information about a specific session history. This provides metadata about the session, including message counts, event types, and more.

**Example Usage**:

```python
async def get_history_details(client, session_id):
    response = await client.get(f'/api/v2/history/{session_id}')
    return response.json()
```

#### Event Retrieval

| v1 Endpoint | v2 Endpoint | HTTP Method |
|-------------|-------------|-------------|
| `/api/v1/events/{session_id}` | `/api/v2/history/{session_id}/events` | GET |

**Request Changes**:
- Query parameters remain similar (event_types, start_time, end_time, limit)
- Response is now wrapped in an `APIResponse` object with `data` field containing the events
- Pagination information is included in the response

**Response Changes**:
- More structured event format with clearer typing
- Additional metadata for each event
- Consistent pagination format

**Example Migration**:

```python
# v1 API call
async def get_events_v1(client, session_id, event_types=None, start_time=None, end_time=None, limit=1000):
    params = {'limit': limit}
    if event_types:
        params['event_types'] = event_types
    if start_time:
        params['start_time'] = start_time
    if end_time:
        params['end_time'] = end_time
        
    response = await client.get(f'/api/v1/events/{session_id}', params=params)
    return response.json()

# v2 API call
async def get_events_v2(client, session_id, event_types=None, start_time=None, end_time=None, limit=100):
    params = {'limit': limit}
    if event_types:
        params['event_types'] = event_types
    if start_time:
        params['start_time'] = start_time
    if end_time:
        params['end_time'] = end_time
        
    response = await client.get(f'/api/v2/history/{session_id}/events', params=params)
    data = response.json()
    return data['data']  # Extract the data field from the APIResponse
```

#### Event Streaming

| v1 Endpoint | v2 Endpoint | HTTP Method |
|-------------|-------------|-------------|
| `/api/v1/events/{session_id}/stream` | `/api/v2/history/{session_id}/stream` | GET |

**Request Changes**:
- Query parameters remain similar (event_types, real_time, speed_factor)
- The path format follows RESTful conventions more closely

**Response Changes**:
- More structured event format with consistent typing
- Better error handling with appropriate status codes

**Example Migration**:

```python
# v1 API call
async def stream_events_v1(client, session_id, event_types=None, real_time=False, speed_factor=1.0):
    params = {
        'real_time': real_time,
        'speed_factor': speed_factor
    }
    if event_types:
        params['event_types'] = event_types
        
    async with client.stream('GET', f'/api/v1/events/{session_id}/stream', params=params) as response:
        async for line in response.aiter_lines():
            if line.strip():
                yield json.loads(line)

# v2 API call
async def stream_events_v2(client, session_id, event_types=None, real_time=False, speed_factor=1.0):
    params = {
        'real_time': real_time,
        'speed_factor': speed_factor
    }
    if event_types:
        params['event_types'] = event_types
        
    async with client.stream('GET', f'/api/v2/history/{session_id}/stream', params=params) as response:
        async for line in response.aiter_lines():
            if line.strip() and line.startswith('data: '):
                event_json = line[6:]  # Remove 'data: ' prefix for SSE
                yield json.loads(event_json)
```

#### Replay Status

| v1 Endpoint | v2 Endpoint | HTTP Method |
|-------------|-------------|-------------|
| `/api/v1/events/{session_id}/replay-status` | `/api/v2/history/{session_id}/replay` | GET |

**Request Changes**:
- URL path is more RESTful and consistent

**Response Changes**:
- Response is now wrapped in an `APIResponse` object with `data` field containing the status
- More detailed status information including timeline positions

**Example Migration**:

```python
# v1 API call
async def get_replay_status_v1(client, session_id):
    response = await client.get(f'/api/v1/events/{session_id}/replay-status')
    return response.json()

# v2 API call
async def get_replay_status_v2(client, session_id):
    response = await client.get(f'/api/v2/history/{session_id}/replay')
    data = response.json()
    return data['data']  # Extract the data field from the APIResponse
```

#### Replay Control

| v1 Endpoint | v2 Endpoint | HTTP Method |
|-------------|-------------|-------------|
| `/api/v1/events/{session_id}/replay/control` | `/api/v2/history/{session_id}/replay` | POST |

**Request Changes**:
- URL path is more RESTful and consistent
- Request body format is more structured with clear validation

**Response Changes**:
- Response is now wrapped in an `APIResponse` object
- More detailed success/error information

**Example Migration**:

```python
# v1 API call
async def control_replay_v1(client, session_id, action, position=None):
    control_data = {
        'action': action
    }
    if position:
        control_data['position'] = position
        
    response = await client.post(
        f'/api/v1/events/{session_id}/replay/control',
        json=control_data
    )
    return response.json()

# v2 API call
async def control_replay_v2(client, session_id, action, position=None, speed=1.0):
    control_data = {
        'action': action,
        'speed': speed
    }
    if position:
        control_data['position'] = position
        
    response = await client.post(
        f'/api/v2/history/{session_id}/replay',
        json=control_data
    )
    data = response.json()
    return data['data']  # Extract the data field from the APIResponse
```

### Debug Endpoints

#### Session Debug Information

| v1 Endpoint | v2 Endpoint | HTTP Method |
|-------------|-------------|-------------|
| N/A | `/api/v2/debug/sessions/{session_id}` | GET |

**New in v2**:
The v2 API introduces a dedicated endpoint for retrieving comprehensive debug information about a session. This includes details about the session state, chat history statistics, and tool configuration.

**Example Usage**:

```python
async def get_session_debug_info(client, session_id):
    response = await client.get(f'/api/v2/debug/sessions/{session_id}')
    data = response.json()
    return data['data']  # Extract the data field from the APIResponse
```

#### Agent Debug Information

| v1 Endpoint | v2 Endpoint | HTTP Method |
|-------------|-------------|-------------|
| N/A | `/api/v2/debug/agent/{session_id}` | GET |

**New in v2**:
The v2 API introduces a dedicated endpoint for retrieving debug information about an agent's state and configuration. This includes details about the agent's parameters, temperature settings, and other runtime properties.

**Example Usage**:

```python
async def get_agent_debug_info(client, session_id):
    response = await client.get(f'/api/v2/debug/agent/{session_id}')
    data = response.json()
    return data['data']  # Extract the data field from the APIResponse
```

## Breaking Changes

### Session Management Breaking Changes

1. **Session IDs**: Session IDs now use the MnemonicSlug format (e.g., "tiger-castle", "blue-ocean")
   - All endpoints that accept session IDs expect properly formatted string IDs
   - MnemonicSlugs provide more readable and memorable identifiers than UUIDs

2. **Parameter Naming**:
   - `model_name` → `model_id`
   - `persona_name` → `persona_id`
   - `ui_session_id` → `session_id` (in URL paths)

3. **Response Structures**:
   - Consistent use of HTTP status codes (201 for creation, 204 for deletion)
   - Session verification now returns full details instead of a boolean

4. **Bulk Operations**:
   - No direct endpoint to delete all sessions at once
   - Multiple operations must be performed individually

### Chat & Files Breaking Changes

1. **Chat Message Format**:
   - Form-based API replaced with JSON-based structured messages
   - Chat messages now use a structured format with explicit content types
   - File references now use the `content` array with typed objects

2. **Chat Response Format**:
   - Streaming responses now return structured JSON events instead of raw text
   - Each event has a specific type (text_delta, tool_call, completion, etc.)
   - Clients must parse JSON events to reconstruct the complete response

3. **File Management URLs**:
   - All file operations now under `/api/v2/sessions/{session_id}/files`
   - File downloads require `/content` suffix
   - File deletion returns 204 (No Content) instead of success message

4. **File Response Fields**:
   - `id` → `file_id`
   - `mime_type` → `content_type`
   - Processing status information now in structured `metadata` object

### History & Debug Breaking Changes

1. **History Endpoints Structure**:
   - All history endpoints now under `/api/v2/history`
   - Event-related endpoints moved from `/api/v1/events/{session_id}` to `/api/v2/history/{session_id}/events`
   - Replay endpoints consolidated to a single RESTful resource

2. **Event Format**:
   - Events use a more consistent and structured format
   - Fields standardized across all event types
   - Additional metadata included for better traceability

3. **Response Wrapping**:
   - All responses now wrapped in an `APIResponse` object with `status` and `data` fields
   - Pagination included for event listings
   - Consistent error response format across all endpoints

4. **Debug Endpoints**:
   - New debug endpoints with no v1 equivalent
   - Comprehensive debug information for sessions and agents
   - Structured format for debug data

## Complete Migration Examples

This section provides complete, end-to-end examples of migrating from v1 to v2 for common scenarios.

### Example 1: Basic Chat Session

This example shows how to create a session, send messages, and handle responses.

```python
# v1 Implementation
async def basic_chat_flow_v1(client, model_name="gpt-4", persona_name="default"):
    # Initialize session
    init_response = await client.post("/api/v1/initialize", json={
        "model_name": model_name,
        "agent_key": persona_name,
        "temperature": 0.7
    })
    session_id = init_response.json()["ui_session_id"]
    
    # Send a message
    async with client.stream(
        "POST", 
        "/api/v1/chat", 
        data={
            "ui_session_id": session_id,
            "message": "Hello, what can you help me with today?"
        }
    ) as response:
        full_response = ""
        async for chunk in response.aiter_text():
            full_response += chunk
            print(chunk, end="")
    
    # Clean up when done
    await client.delete("/api/v1/sessions")
    return session_id, full_response

# v2 Implementation
async def basic_chat_flow_v2(client, model_id="gpt-4", persona_id="default"):
    # Create session
    create_response = await client.post("/api/v2/sessions", json={
        "model_id": model_id,
        "persona_id": persona_id,
        "temperature": 0.7,
        "name": "Basic Chat Session"
    })
    session_data = create_response.json()
    session_id = session_data["id"]
    
    # Send a message
    chat_request = {
        "message": {
            "role": "user",
            "content": [{"type": "text", "text": "Hello, what can you help me with today?"}]
        },
        "stream": True
    }
    
    full_text = ""
    async with client.stream(
        "POST", 
        f"/api/v2/sessions/{session_id}/chat", 
        json=chat_request
    ) as response:
        async for chunk in response.aiter_text():
            if not chunk.strip():
                continue
                
            # Parse the JSON event
            event = json.loads(chunk)
            event_type = event.get("type")
            
            # Handle different event types
            if event_type == "text_delta":
                text_piece = event.get("content", "")
                full_text += text_piece
                print(text_piece, end="")
    
    # Clean up when done
    await client.delete(f"/api/v2/sessions/{session_id}")
    return session_id, full_text
```

### Example 2: Complete Application Integration

This example shows a more complete application flow, including configuration loading, file upload, and chat interactions.

```python
async def complete_app_flow_v1(client, file_path=None):
    # 1. Get available models and personas
    models_response = await client.get("/api/v1/models")
    models = models_response.json()
    
    personas_response = await client.get("/api/v1/personas")
    personas = personas_response.json()
    
    # Choose first available model and persona
    model_name = models[0]["name"]
    persona_name = personas[0]["name"]
    
    # 2. Initialize session
    init_response = await client.post("/api/v1/initialize", json={
        "model_name": model_name,
        "agent_key": persona_name,
        "temperature": 0.7
    })
    session_id = init_response.json()["ui_session_id"]
    
    # 3. Upload file if provided
    file_id = None
    if file_path:
        filename = os.path.basename(file_path)
        with open(file_path, "rb") as f:
            files = {"file": (filename, f)}
            form_data = {"ui_session_id": session_id}
            
            upload_response = await client.post(
                "/api/v1/upload_file",
                data=form_data,
                files=files
            )
        file_id = upload_response.json()["id"]
    
    # 4. Send message with file reference if available
    form_data = {
        "ui_session_id": session_id,
        "message": "Please analyze this information."
    }
    
    if file_id:
        form_data["file_ids"] = json.dumps([file_id])
    
    chat_responses = []
    async with client.stream("POST", "/api/v1/chat", data=form_data) as response:
        async for chunk in response.aiter_text():
            chat_responses.append(chunk)
    
    # 5. Get session history
    events_response = await client.get(f"/api/v1/events/{session_id}")
    events = events_response.json()
    
    return {
        "session_id": session_id,
        "model": model_name,
        "persona": persona_name,
        "file_id": file_id,
        "chat_responses": chat_responses,
        "events": events
    }

async def complete_app_flow_v2(client, file_path=None):
    # 1. Get all configuration in one request
    config_response = await client.get("/api/v2/config/system")
    system_config = config_response.json()["data"]
    
    # Choose first available model and persona
    model_id = system_config["models"][0]["id"]
    persona_id = system_config["personas"][0]["id"]
    
    # 2. Create session
    create_response = await client.post("/api/v2/sessions", json={
        "model_id": model_id,
        "persona_id": persona_id,
        "temperature": 0.7,
        "name": "Analysis Session"
    })
    session_data = create_response.json()
    session_id = session_data["id"]
    
    # 3. Upload file if provided
    file_id = None
    if file_path:
        filename = os.path.basename(file_path)
        with open(file_path, "rb") as f:
            files = {"file": (filename, f)}
            
            upload_response = await client.post(
                f"/api/v2/sessions/{session_id}/files",
                files=files
            )
        file_id = upload_response.json()["file_id"]
    
    # 4. Send message with file reference if available
    content = [{"type": "text", "text": "Please analyze this information."}]
    
    if file_id:
        content.append({"type": "file", "file_id": file_id})
    
    chat_request = {
        "message": {
            "role": "user",
            "content": content
        },
        "stream": True
    }
    
    chat_events = []
    async with client.stream(
        "POST", 
        f"/api/v2/sessions/{session_id}/chat", 
        json=chat_request
    ) as response:
        async for chunk in response.aiter_text():
            if chunk.strip():
                chat_events.append(json.loads(chunk))
    
    # 5. Get session history
    events_response = await client.get(f"/api/v2/history/{session_id}/events")
    events = events_response.json()["data"]
    
    return {
        "session_id": session_id,
        "model": model_id,
        "persona": persona_id,
        "file_id": file_id,
        "chat_events": chat_events,
        "events": events
    }
```

### Example 3: Migrating a History Replay Application

This example shows how to migrate an application that accesses and replays session history.

```python
async def history_replay_app_v1(client, session_id):
    # 1. Get replay status
    status_response = await client.get(f"/api/v1/events/{session_id}/replay-status")
    replay_status = status_response.json()
    
    # 2. Start replay from beginning
    control_response = await client.post(
        f"/api/v1/events/{session_id}/replay/control",
        json={
            "action": "start",
            "position": 0
        }
    )
    
    # 3. Stream events with real-time playback
    events = []
    async with client.stream(
        "GET", 
        f"/api/v1/events/{session_id}/stream", 
        params={"real_time": True, "speed_factor": 2.0}
    ) as response:
        async for line in response.aiter_lines():
            if line.strip():
                events.append(json.loads(line))
                
    # 4. Pause replay
    await client.post(
        f"/api/v1/events/{session_id}/replay/control",
        json={"action": "pause"}
    )
    
    return events

async def history_replay_app_v2(client, session_id):
    # 1. Get replay status
    status_response = await client.get(f"/api/v2/history/{session_id}/replay")
    replay_status = status_response.json()["data"]
    
    # 2. Start replay from beginning
    control_response = await client.post(
        f"/api/v2/history/{session_id}/replay",
        json={
            "action": "start",
            "position": 0,
            "speed": 2.0
        }
    )
    
    # 3. Stream events with real-time playback
    events = []
    async with client.stream(
        "GET", 
        f"/api/v2/history/{session_id}/stream", 
        params={"real_time": True, "speed_factor": 2.0}
    ) as response:
        async for line in response.aiter_lines():
            if line.strip() and line.startswith('data: '):
                event_json = line[6:]  # Remove 'data: ' prefix for SSE
                events.append(json.loads(event_json))
                
    # 4. Pause replay
    await client.post(
        f"/api/v2/history/{session_id}/replay",
        json={"action": "pause"}
    )
    
    return events
```

## Migration Timeline and Support

### Transition Timeline

| Phase | Timeframe | Status | Details |
|-------|-----------|--------|---------|
| **Dual Availability** | Current - 3 months | Active | Both v1 and v2 APIs available with full support |
| **New Feature Freeze for v1** | 3 months | Planned | New features added exclusively to v2 API |
| **Deprecation Notice** | 3-6 months | Planned | v1 API officially deprecated; users encouraged to migrate |
| **End of Life Planning** | 6-9 months | Planned | Final transition period with limited v1 support |
| **v1 API Removal** | 9-12 months | Planned | v1 API endpoints discontinued |

### Recommended Migration Schedule

- **Months 0-1**: Assessment and planning
  - Inventory existing API usage
  - Identify affected application components
  - Create migration roadmap
  - Begin testing v2 configuration endpoints

- **Months 1-3**: Core functionality migration
  - Update session management code
  - Migrate chat functionality
  - Implement new request/response handling

- **Months 3-6**: Complete migration
  - Migrate history and replay functionality
  - Implement new features available only in v2
  - Complete testing and validation
  - Switch production traffic to v2

### Support and Resources

#### Documentation Resources
- [API Reference](https://agent-c-api.example.com/docs)
- [Code Examples Repository](https://github.com/example/agent-c-examples)
- [Migration Tool](https://github.com/example/agent-c-migration)

#### Support Channels
- Technical Support: [support@example.com](mailto:support@example.com)
- API Status: [status.agent-c-api.example.com](https://status.agent-c-api.example.com)
- Community Forum: [community.agent-c.example.com](https://community.agent-c.example.com)

#### Office Hours
We offer weekly migration support office hours during the transition period:
- Wednesdays 10:00-11:00 AM EST
- Fridays 3:00-4:00 PM EST

Please contact [migrations@example.com](mailto:migrations@example.com) to schedule a session..