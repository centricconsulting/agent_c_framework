# Workspace Organization Component

A foundational workspace management pattern that establishes standard conventions for file organization, scratchpad usage, and workspace operations across all agents with workspace tools.

## Binary Decision

**Does this agent use workspace tools for file and directory management?**

- **YES** → Use this component
- **NO** → Skip this component

## Who Uses This

**Target Agents**: All agents equipped with WorkspaceTools (90% of agents)

**Scenarios**:
- Agents that create, read, write, and organize files
- Agents that manage project documentation and resources
- Agents that collaborate through shared workspaces
- Agents that need long-term storage and knowledge repositories
- Agents that delegate work through workspace handoffs
- Agents managing user collaboration and file sharing
- Any agent that performs workspace-based operations

## Component Pattern

```markdown
## Workspace Organization Guidelines

### Core Workspace Structure
- **Primary Workspace**: Use the defined workspace for all operations unless otherwise specified
- **Long-term Storage**: Use workspace for persistent files, documentation, and knowledge repositories
- **User Collaboration**: Leverage workspace for shared resources and collaborative workflows
- **State Management**: Maintain operational state and progress tracking within workspace structure

### Scratchpad Management
- **Working Area**: Utilize `{workspace}/.scratch` as your primary working and temporary storage area
- **Session Files**: Store temporary analysis, working notes, and processing files in scratchpad
- **Handoff Notes**: Create unique handoff files (e.g., `step_1.2_handoff`, `analysis_summary`) in scratchpad for workflow continuity
- **Progress Tracking**: Maintain plan progress and state tracking files in scratchpad area

### File Operations Standards
- **File Writing**: Use workspace `write` tool with `append` mode for file appending operations
- **File Organization**: Create logical directory structures that support long-term maintenance
- **Document Indexing**: Maintain `{workspace}/Document_Library_Index.md` for tracking key documents and resources
- **Version Control**: Use clear, descriptive filenames that indicate purpose and currency

### Trash Management
- **Cleanup Protocol**: Use `workspace_mv` to move outdated or obsolete files to `{workspace}/.scratch/trash`
- **Safe Deletion**: Never permanently delete files - always move to trash for potential recovery
- **Trash Organization**: Organize trash by date or project for easier recovery if needed

### Workspace Conventions
- **Path Standards**: Always use UNC-style paths (//workspace/path) for all workspace operations
- **Directory Creation**: Establish clear directory hierarchies that scale with project complexity
- **Access Verification**: Always verify workspace and path existence before performing operations
- **Resource Management**: Maintain workspace organization to support efficient collaboration and knowledge sharing
```

## Usage Notes

**Positioning**: Place in a dedicated "Workspace Organization" section in the agent persona, typically after core guidelines but before specific operational procedures.

**Implementation Notes**:
- **Universal Application**: This complete pattern applies to all WorkspaceTools-equipped agents - no variations or conditional logic
- **Comprehensive Coverage**: Addresses all major workspace operations from file management to collaboration
- **Scalable Structure**: Designed to work for both simple and complex workspace scenarios
- **Template Integration**: Replace `{workspace}` placeholder with actual workspace name when implementing
- **Customizing {workspace} Placeholder**:
  - **Dedicated Workspace Agents**: Replace with specific workspace name (e.g., `myproject/.scratch`) if the agent is assigned to a single, known workspace
  - **Multi-Workspace Agents**: Use generic language (e.g., "your assigned workspace", ".scratch directory in your workspace") if the agent should adapt to any workspace it's given
  - **Domo Agents**: Typically use generic language since they may work across different user workspaces
  - **Specialist Agents**: Typically use specific workspace names since they're often dedicated to a particular project

**Integration Tips**:
- Works independently - no dependencies on other components
- Pairs well with Critical Interaction Guidelines for comprehensive workspace safety
- Essential for agents using planning tools for workspace-based delegation
- Combines effectively with reflection rules for complex workspace analysis

## Example Implementation

All WorkspaceTools-equipped agents use this identical pattern:

```markdown
## Workspace Organization Guidelines

### Core Workspace Structure
- **Primary Workspace**: Use the defined workspace for all operations unless otherwise specified
- **Long-term Storage**: Use workspace for persistent files, documentation, and knowledge repositories
- **User Collaboration**: Leverage workspace for shared resources and collaborative workflows
- **State Management**: Maintain operational state and progress tracking within workspace structure

### Scratchpad Management
- **Working Area**: Utilize `myproject/.scratch` as your primary working and temporary storage area
- **Session Files**: Store temporary analysis, working notes, and processing files in scratchpad
- **Handoff Notes**: Create unique handoff files (e.g., `step_1.2_handoff`, `analysis_summary`) in scratchpad for workflow continuity
- **Progress Tracking**: Maintain plan progress and state tracking files in scratchpad area

### File Operations Standards
- **File Writing**: Use workspace `write` tool with `append` mode for file appending operations
- **File Organization**: Create logical directory structures that support long-term maintenance
- **Document Indexing**: Maintain `myproject/Document_Library_Index.md` for tracking key documents and resources
- **Version Control**: Use clear, descriptive filenames that indicate purpose and currency

### Trash Management
- **Cleanup Protocol**: Use `workspace_mv` to move outdated or obsolete files to `myproject/.scratch/trash`
- **Safe Deletion**: Never permanently delete files - always move to trash for potential recovery
- **Trash Organization**: Organize trash by date or project for easier recovery if needed

### Workspace Conventions
- **Path Standards**: Always use UNC-style paths (//workspace/path) for all workspace operations
- **Directory Creation**: Establish clear directory hierarchies that scale with project complexity
- **Access Verification**: Always verify workspace and path existence before performing operations
- **Resource Management**: Maintain workspace organization to support efficient collaboration and knowledge sharing
```

## Component Benefits

- **Standardized Organization**: Ensures consistent workspace structure across all WorkspaceTools-enabled agents
- **Collaboration Support**: Facilitates smooth handoffs and shared workspace usage between agents
- **Knowledge Management**: Provides systematic approach to documentation and resource organization
- **Recovery Support**: Safe file management practices prevent accidental data loss
- **Scalability**: Structure supports both simple file operations and complex multi-agent workflows
- **Binary Decision**: Clear YES/NO implementation - agents either have comprehensive workspace organization or they do not