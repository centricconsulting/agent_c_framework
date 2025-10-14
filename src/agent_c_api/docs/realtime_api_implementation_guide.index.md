# Index of //api/docs/realtime_api_implementation_guide.md

The list below contains the line number of each header in the document to aid in reading individual sections.

3:## Overview
7:## Architecture
16:### Event System Architecture
20:#### BaseEvent Structure
27:#### Event Categories
43:#### Event Type Naming Convention
54:## Identifier System: MnemonicSlugs
58:### Why MnemonicSlugs?
71:### Format and Structure
73:#### Basic Format
78:#### Hierarchical IDs
85:### Usage in Realtime API
87:#### Session Identifiers
92:#### Subsession Correlation
102:#### Examples in API Responses
124:### Implementation Considerations
126:#### Client Handling
132:#### Benefits for Development
142:## REST API Endpoints
144:### Authentication & Configuration
146:#### `POST /rt/login`
173:#### `GET /rt/refresh_token`
188:## WebSocket Connection
190:### Connection Endpoint
201:### Session Management
205:#### UI Session IDs
213:#### Chat Session IDs
228:### Connection Flow
236:#### Initialization Event Sequence
282:## Chat Session Models
284:### ChatSession Model
304:#### Computed Fields
315:#### Message Formats by Vendor
348:### ChatSessionIndexEntry Model 
365:### ChatSessionQueryResponse Model
378:## Agent Configuration System
380:### Overview
389:### CurrentAgentConfiguration Model
393:#### Core Fields
405:#### Configuration Fields
416:### Agent Category System
420:#### Special Category Meanings
422:##### `'domo'` - User Collaboration Agents
428:##### `'realtime'` - Voice-Optimized Agents  
434:##### `'assist'` - Agent Helper Agents
440:#### Team Formation Through Categories
458:### Completion Parameters
462:#### Parameter Types
472:#### Common Parameters
480:### Example Configurations
482:#### Voice-Optimized User Agent
503:#### Technical Assistant Agent
525:#### Team Leader Agent
545:### Client Implementation Notes
547:#### Category-Based Behavior
556:#### Tool Filtering
563:#### Session Context
575:## Client → Server Events (Commands)
579:### Agent Management
581:#### `get_agents`
591:#### `set_agent`
602:### Avatar Management
604:#### `get_avatars`
614:#### `set_avatar_session`
626:### Voice Management
628:#### `get_voices`
638:#### `set_agent_voice`
649:### Tool Management
651:#### `get_tool_catalog`
661:### Session Management
663:#### `get_user_sessions`
675:### Connection Health
677:#### `ping`
687:### Chat Management
689:#### `text_input`
701:#### `new_chat_session`
712:#### `resume_chat_session`
723:#### `set_chat_session_name`
734:#### `set_session_metadata`
748:#### `set_session_messages`
790:## Server → Client Events (Responses & Updates)
794:### Control Events (BaseEvent)
798:### Agent Updates
800:#### `agent_list`
818:#### `agent_configuration_changed`
841:### Avatar Updates
843:#### `avatar_list`
864:#### `avatar_connection_changed`
900:### Chat Events
902:#### `chat_session_changed`
938:#### `chat_session_name_changed`
949:#### `session_metadata_changed`
963:### Session Events (SessionEvent → BaseEvent)
973:#### `text_delta`
991:#### `thought_delta`
1007:#### `completion`
1026:#### `interaction`
1042:#### `history`
1075:#### `tool_call`
1107:#### `system_message`
1124:#### `render_media`
1157:### Turn Management Events
1159:#### `user_turn_start`
1169:#### `user_turn_end`
1179:### Voice Events
1181:#### `voice_list`
1211:#### `agent_voice_changed`
1227:### Tool Events
1229:#### `tool_catalog`
1264:### User Events
1266:#### `chat_user_data`
1288:#### `get_user_sessions_response`
1315:### Connection Events
1317:#### `pong`
1327:### Error Events
1329:#### `error`
1343:## Binary Audio Streaming
1347:### Audio Input
1353:### Audio Output
1361:### Implementation Example
1381:### Audio Processing Pipeline
1389:### Special Voice Models
1393:#### Avatar Voice Model (`voice_id: "avatar"`)
1409:#### No Voice Model (`voice_id: "none"`)
1426:## Implementation Patterns
1428:### Client Connection Flow
1464:### Event Handling
1512:## Authentication & Security
1514:### JWT Token Structure
1525:### Token Refresh
1534:## Error Handling
1536:### Common Error Scenarios
1558:### Error Recovery
1567:## Performance Considerations
1569:### Message Buffering
1575:### Connection Management
1584:## Additional References
1586:### Core Components
1593:### Voice System
1598:### Client Tool Integration
1604:## Development Tips
