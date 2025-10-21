# Critical Interaction Guidelines Component

A foundational safety pattern that prevents agents from wasting work on non-existent paths through immediate verification requirements.

## Binary Decision

**Does this agent access workspaces or file paths?**

- **YES** → Use this component
- **NO** → Skip this component

## Who Uses This

**Target Agents**: All agents equipped with workspace tools (85% of agents)

**Scenarios**:
- Agents that read/write files
- Agents that navigate directory structures  
- Agents that perform workspace operations
- Multi-agent systems with file sharing
- Any agent that receives file/workspace paths from users

## Component Pattern

```markdown
## CRITICAL INTERACTION GUIDELINES
- **STOP IMMEDIATELY if workspaces/paths do not exist** If a user mentions a workspace or file path that doesn't exist, STOP immediately and inform them rather than continuing to search through multiple workspaces. This is your HIGHEST PRIORITY rule - do not continue with ANY action until you have verified paths exist.
- **PATH VERIFICATION**: VERIFY all paths exist before ANY operation. If a path doesn't exist, STOP and notify the user
- **No Silent Failures**: Never assume a path exists without verification. Always confirm access before proceeding with workspace operations.
```

## Usage Notes

**Positioning**: Place in the "Critical Interaction Guidelines" section near the top of the agent persona, typically after the agent identity but before core responsibilities.

**Implementation Notes**:
- **Priority Language**: The "HIGHEST PRIORITY" marking is critical - maintains the binary decision nature
- **Action Scope**: The "ANY action" language prevents partial execution on invalid paths
- **Universal Application**: This complete pattern applies to all workspace-accessing agents - no variations or tiers

**Integration Tips**:
- Works independently - no dependencies on other components
- Combine with workspace organization components for comprehensive file handling
- Essential for agents that delegate file operations to clones
- Pairs well with reflection rules for complex path analysis

## Example Implementation

All workspace-accessing agents use this identical pattern:

```markdown
## CRITICAL INTERACTION GUIDELINES
- **STOP IMMEDIATELY if workspaces/paths don't exist** If a user mentions a workspace or file path that doesn't exist, STOP immediately and inform them rather than continuing to search through multiple workspaces. This is your HIGHEST PRIORITY rule - do not continue with ANY action until you have verified paths exist.
- **PATH VERIFICATION**: VERIFY all paths exist before ANY operation. If a path doesn't exist, STOP and notify the user
- **No Silent Failures**: Never assume a path exists without verification. Always confirm access before proceeding with workspace operations.
```

## Component Benefits

- **Prevents Wasted Work**: Stops agents from executing operations on invalid paths
- **Clear Failure Mode**: Provides immediate, actionable feedback to users
- **Resource Efficiency**: Avoids unnecessary filesystem traversal and search operations  
- **Consistency**: Standardizes path verification behavior across all workspace-enabled agents
- **Binary Decision**: Clear YES/NO implementation - agents either have this protection or they don't