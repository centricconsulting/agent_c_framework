# Reflection Rules Component

A foundational thinking pattern that ensures agents systematically capture and process new information through structured reflection using the think tool.

## Binary Decision

**Does this agent have access to the ThinkTools?**

- **YES** → Use this component
- **NO** → Skip this component

## Who Uses This

**Target Agents**: All agents equipped with ThinkTools (80% of agents)

**Scenarios**:
- Agents that read and analyze unfamiliar code
- Agents that process planning tool outputs
- Agents performing complex refactoring or enhancements
- Agents analyzing bugs and root causes
- Agents reading scratchpad content from other agents
- Agents evaluating solutions and impacts
- Any agent that needs to process and understand new information systematically

## Component Pattern

```markdown
# MUST FOLLOW: Reflection Rules
You MUST use the `think` tool to reflect on new information and record your thoughts in the following situations:
- Reading through unfamiliar code
- Reading plans from the planning tool
- Planning a complex refactoring or enhancement
- Analyzing potential bugs and their root causes
- After reading scratchpad content.
- When considering possible solutions to a problem
- When evaluating the impact of a proposed change
- When determining the root cause of an issue
- If you find yourself wanting to immediately fix something
```

## Usage Notes

**Positioning**: Place in a dedicated "Reflection Rules" section early in the agent persona, typically after core guidelines but before specific responsibilities.

**Implementation Notes**:
- **Mandatory Language**: The "MUST FOLLOW" and "MUST use" language ensures consistent application
- **Comprehensive Triggers**: Covers all major scenarios where reflection adds value to agent processing
- **Universal Application**: This complete pattern applies to all ThinkTools-equipped agents - no variations or tiers
- **Process Integration**: Integrates thinking into the agent's natural workflow rather than treating it as an afterthought

**Integration Tips**:
- Works independently - no dependencies on other components
- Essential for agents that delegate work to clones (helps with handoff analysis)
- Combines well with planning components for complex multi-step workflows
- Particularly valuable for technical agents analyzing code, systems, or processes

## Example Implementation

All ThinkTools-equipped agents use this identical pattern:

```markdown
# MUST FOLLOW: Reflection Rules
You MUST use the `think` tool to reflect on new information and record your thoughts in the following situations:
- Reading through unfamiliar code
- Reading plans from the planning tool
- Planning a complex refactoring or enhancement
- Analyzing potential bugs and their root causes
- After reading scratchpad content.
- When considering possible solutions to a problem
- When evaluating the impact of a proposed change
- When determining the root cause of an issue
- If you find yourself wanting to immediately fix something
```

## Component Benefits

- **Systematic Information Processing**: Ensures agents don't rush to action without proper analysis
- **Knowledge Capture**: Creates valuable thinking logs that can inform future decisions
- **Quality Improvement**: Reflection leads to better understanding and more thoughtful solutions
- **Debugging Support**: Thinking logs provide insight into agent reasoning processes
- **Consistency**: Standardizes reflection behavior across all ThinkTools-enabled agents
- **Binary Decision**: Clear YES/NO implementation - agents either have systematic reflection or they do not