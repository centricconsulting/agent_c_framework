# API Documentation

This document provides details about the available API endpoints for sessions and events management.

## Events API

### Get Events

```
GET /events/{session_id}
```

- **Purpose**: Retrieves events for a specific session with filtering options
- **Parameters**:
  - `session_id`: Required path parameter identifying the session
  - `event_types`: Optional query parameter to filter by event types
  - `start_time`: Optional query parameter to filter events after this timestamp
  - `end_time`: Optional query parameter to filter events before this timestamp
  - `limit`: Optional query parameter (default: 1000) for maximum number of events to return
- **Response Format**: List of Event objects
- **Error Response**: 404 if no events found for the session

### Stream Events

```
GET /events/{session_id}/stream
```

- **Purpose**: Streams events for a specific session, optionally in real-time
- **Parameters**:
  - `session_id`: Required path parameter identifying the session
  - `event_types`: Optional query parameter to filter by event types
  - `real_time`: Optional query parameter (boolean, default: false) to replay events with original timing
  - `speed_factor`: Optional query parameter (float, default: 1.0) for speed multiplier in real-time replay
- **Response Format**: Server-sent events with media type "text/event-stream"

### Get Replay Status

```
GET /events/{session_id}/replay-status
```

- **Purpose**: Gets the current status of a session replay
- **Parameters**:
  - `session_id`: Required path parameter identifying the session
- **Response Format**: Replay status object
- **Error Response**: 404 if no active replay for the session

### Control Replay

```
POST /events/{session_id}/replay/control
```

- **Purpose**: Controls a session replay (play, pause, stop, seek)
- **Parameters**:
  - `session_id`: Required path parameter identifying the session
  - Request body: ReplayControlRequest object containing:
    - `action`: The control action to perform
    - `position`: Optional position parameter for seek actions
- **Response Format**: JSON with status and action fields
- **Error Response**: 400 if invalid replay control action

## Interactions API

### List Sessions

```
GET /interactions/
```

- **Purpose**: Lists all available sessions with pagination and sorting
- **Parameters**:
  - `limit`: Optional query parameter (default: 50) for maximum number of sessions to return
  - `offset`: Optional query parameter (default: 0) for number of sessions to skip
  - `sort_by`: Optional query parameter (default: "timestamp") for field to sort by
  - `sort_order`: Optional query parameter (default: "desc") for sort order (asc or desc)
- **Response Format**: List of InteractionSummary objects

### Get Session

```
GET /interactions/{session_id}
```

- **Purpose**: Gets detailed information about a specific session
- **Parameters**:
  - `session_id`: Required path parameter identifying the session
- **Response Format**: InteractionDetail object
- **Error Response**: 404 if session not found

### Get Session Files

```
GET /interactions/{session_id}/files
```

- **Purpose**: Gets a list of all JSONL files associated with a specific session
- **Parameters**:
  - `session_id`: Required path parameter identifying the session
- **Response Format**: List of strings representing file paths or names
- **Error Response**: 404 if no files found for the session