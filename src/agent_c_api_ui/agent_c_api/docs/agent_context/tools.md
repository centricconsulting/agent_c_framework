# Tools API

> **Documentation generated**: May 1, 2025

Endpoints for managing tools available to agents.

## List Available Tools

```
GET /api/v1/tools
```

Retrieves a list of all available tools categorized by type.

### Response

| Field | Type | Description |
|-------|------|-------------|
| `groups` | object | Tools categorized by groups |
| `groups.{group_name}` | array | List of tools in a specific group |
| `groups.{group_name}[].name` | string | Tool class name |
| `groups.{group_name}[].module` | string | Python module containing the tool |
| `groups.{group_name}[].doc` | string | Documentation for the tool |
| `groups.{group_name}[].essential` | boolean | Whether the tool is automatically loaded |
| `categories` | array | List of available tool categories |
| `tool_name_mapping` | object | Mapping of tool names to internal identifiers (if available) |

#### Response Example

```json
{
  "essential_toolsets": [
    {
      "name": "WorkspaceTools",
      "module": "agent_c_tools.tools.workspace_tools",
      "doc": "Tools for working with workspaces",
      "essential": true
    },
    {
      "name": "ThinkTools",
      "module": "agent_c_tools.tools.think_tools",
      "doc": "Tools for explicit reasoning",
      "essential": true
    }
  ],
  "groups": {
    "Core Tools": [
      {
        "name": "TextTools",
        "module": "agent_c_tools.tools.text_tools",
        "doc": "Tools for text manipulation",
        "essential": false
      },
      {
        "name": "FileTools",
        "module": "agent_c_tools.tools.file_tools",
        "doc": "Tools for file operations",
        "essential": false
      }
    ],
    "Demo Tools": [
      {
        "name": "RandomNumberTools",
        "module": "agent_c_tools.tools.demo.random_tools",
        "doc": "Tools for generating random numbers",
        "essential": false
      }
    ],
    "Voice Tools": [
      {
        "name": "VoiceSynthesisTools",
        "module": "agent_c_tools.tools.voice.synthesis",
        "doc": "Tools for voice synthesis",
        "essential": false
      }
    ],
    "RAG Tools": [
      {
        "name": "VectorDatabaseTools",
        "module": "agent_c_tools.tools.rag.vector_db",
        "doc": "Tools for vector database operations",
        "essential": false
      }
    ]
  },
  "categories": [
    "Core Tools",
    "Demo Tools",
    "Voice Tools",
    "RAG Tools"
  ],
  "tool_name_mapping": {}
}
```

### Error Responses

| Status Code | Description |
|-------------|-------------|
| 500 | Internal Server Error - Error retrieving tools |