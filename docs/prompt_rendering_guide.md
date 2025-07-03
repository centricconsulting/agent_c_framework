# Agent C Prompt Rendering Guide

## Philosophy: Selective Rendering for Performance

**Core Principle:** Any instruction or context information in a prompt that's not ACTUALLY useful for the current task is a hindrance to task performance. Agent C's prompt rendering system is designed to include only what's needed, when it's needed.

## Overview: How Prompt Rendering Works

Agent C uses a multi-stage rendering process:

1. **Template Collection**: Gather all required prompt sections from tools and agent configuration
2. **Section Grouping**: Group sections by `SectionRenderSlot` for ordered rendering
3. **Master Template Rendering**: Process the agent instructions and section collections
4. **Selective Inclusion**: Use helper functions to conditionally include content
5. **Deduplication**: Ensure each template renders only once per render pass

## Section Render Slots

Sections are automatically grouped into render slots that determine their default ordering:

```python
class SectionRenderSlot(StrEnum):
    INCLUDE_ONLY = "include_only"    # Never auto-rendered, only via explicit include
    SYSTEM = "system"                # System-level context
    BEFORE_AGENT = "before_agent"    # Context before agent instructions
    AFTER_AGENT = "after_agent"      # Context after agent instructions  
    TOOL = "tool"                    # Tool-specific instructions
    DEFAULT = "default"              # General sections
    AT_BOTTOM = "at_bottom"          # Final context (examples, etc.)
```

## Master Template Flow

The master template follows this rendering order:

1. Render `SYSTEM` sections
2. Render `BEFORE_AGENT` sections
3. **Render agent instructions** (capturing any explicit includes)
4. Render `AFTER_AGENT` sections
5. Render `TOOL` sections (only those not already included)
6. Render `DEFAULT` sections (only those not already included)
7. Render `AT_BOTTOM` sections

**Key Insight:** Well-formed agent instructions explicitly place everything where it should be. The auto-rendering is a fallback for sections that weren't explicitly positioned.

## Core Helper Functions

### Section Inclusion Helpers

#### `prompt_section(name)`
Renders a template section exactly once per render pass.

```jinja
{# This will render the workspace list exactly once #}
{{ prompt_section('tools/workspace/workspace_list') }}

{# Subsequent calls return empty string #}
{{ prompt_section('tools/workspace/workspace_list') }}  {# No output #}
```

#### `skip_prompt_section(name)` 
Prevents a section from rendering, even during auto-rendering.

```jinja
{# Mark this section as "already rendered" without actually rendering it #}
{{ skip_prompt_section('tools/workspace/workspace_list') }}
```

#### `tool_section(section_name)`
Shorthand for `prompt_section('tools/{section_name}')`.

```jinja
{{ tool_section('workspace/workspace_list') }}
{# Equivalent to: {{ prompt_section('tools/workspace/workspace_list') }} #}
```

## State-Based Selective Rendering

### Toggle System

Create simple on/off switches for content sections:

```jinja
{# Basic toggle usage #}
{% if toggle('debug_info', default='closed') == 'open' %}
<detailed_debug_context>
  Session state: {{ session.state }}
  Active tools: {{ session.active_tools }}
</detailed_debug_context>
{% endif %}

{# Programmatic control #}
{{ toggle_open('verbose_mode') }}
{% if toggle('verbose_mode') == 'open' %}
  {{ tool_section('advanced_debugging') }}
{% endif %}
```

### State-Based Template Inclusion

Include different templates based on machine state:

```jinja
{# Include different contexts based on user authentication #}
{{ include_for_state('user_auth_machine', {
    'logged_in': 'prompts/authenticated_user',
    'guest': 'prompts/guest_user',
    'admin': 'prompts/admin_user',
    'default': 'prompts/fallback_user'
}) }}

{# Keyword argument syntax #}
{{ switch_on_state('debug_level_machine',
                  verbose='debug/verbose_context',
                  normal='debug/standard_context',
                  minimal='debug/minimal_context') }}
```

### Collapsible Content

Create user-controlled progressive disclosure:

```jinja

{# Collapsible inline content #}
{{ collapsible('debug_info', 
               'Current memory usage: 45MB\nActive connections: 3\nLast API call: 10:30:25',
               'Debug information available') }}

{# Collapsible template sections #}
{{ collapsible_section('advanced_features', 
                       'tools/advanced/feature_set',
                       'Advanced feature documentation available') }}

{# Collapsible tool sections #}
{{ collapsible_tool_section('search_tools', 
                           'elasticsearch',
                           'Advanced search capabilities available') }}

```

**Output when closed:**

```
Debug information available - *Use `toggle(debug_info)` to display.*
```

**Output when open:**

```
Current memory usage: 45MB
Active connections: 3
Last API call: 10:30:25

*Use `toggle(debug_info)` to hide this info and save context space.*
```

## Best Practices for Agent Prompt Authors

### 1. Explicit Positioning
Always explicitly position important content in your agent instructions:

```jinja
# Core Agent Instructions

## Context Setup
{{ prompt_section('context/user_profile') }}
{{ prompt_section('context/current_session') }}

## Core Capabilities
{{ tool_section('core/basic_tools') }}

## Advanced Features (Conditional)
{{ collapsible_tool_section('advanced_mode', 'advanced/power_tools', 'Advanced tools available') }}

## Working Memory
{{ include_for_toggle('working_memory', 'memory/detailed_context') }}
```

### 2. Use Selective Rendering
Don't include everything - include what's needed:

```jinja
{# Bad: Always include verbose debugging #}
{{ tool_section('debug/verbose_logging') }}

{# Good: Make it conditional #}
{{ collapsible_tool_section('debug_mode', 'debug/verbose_logging', 'Debug tools available') }}
```

### 3. Leverage State Machines
Use state machines to adapt content to context:

```jinja
{# Adapt tool instructions based on user skill level #}
{{ switch_on_state('user_skill_machine',
                  beginner='tools/basic_instructions',
                  intermediate='tools/standard_instructions', 
                  expert='tools/minimal_instructions') }}
```

### 4. Skip Redundant Sections
If you include something explicitly, skip its auto-rendering:

```jinja
{# Include workspace tools in a specific location #}
{{ tool_section('workspace/main') }}

{# Prevent auto-rendering of workspace sub-sections #}
{{ skip_tool_section('workspace/file_operations') }}
{{ skip_tool_section('workspace/directory_structure') }}
```

## Best Practices for Tool Authors

### 1. Design Modular Tool Sections
Break tool sections into composable parts:

```
tools/
  workspace/
    main.jinja          # Main workspace instructions
    file_operations.jinja     # File-specific operations  
    directory_structure.jinja # Directory operations
    advanced.jinja           # Advanced features
```

**In `tools/workspace/main.jinja`:**
```jinja
# Workspace Tool

## Basic Operations
{{ prompt_section('tools/workspace/file_operations') }}
{{ prompt_section('tools/workspace/directory_structure') }}

## Advanced Features
{{ collapsible_section('workspace_advanced', 
                       'tools/workspace/advanced',
                       'Advanced workspace operations available') }}
```

### 2. Use Appropriate Render Slots
Choose the right slot for your content:

- `INCLUDE_ONLY`: For sub-sections that should never auto-render
- `TOOL`: For main tool instructions
- `SYSTEM`: For system-level context that affects all tools
- `AT_BOTTOM`: For examples and reference material

### 3. Support Conditional Inclusion
Design your tool sections to work with selective rendering:

```jinja
{# In your tool section #}
## Search Tool

### Basic Search
{{ prompt_section('tools/search/basic') }}

### Advanced Search
{% if toggle('advanced_search') == 'open' %}
{{ prompt_section('tools/search/advanced') }}
*Use `toggle(advanced_search)` to hide advanced options.*
{% else %}
Advanced search options available - *Use `toggle(advanced_search)` to display.*
{% endif %}
```

### 4. Provide Clear Toggle Points
Give users control over verbosity:

```jinja
{{ collapsible('search_examples',
               prompt_section('tools/search/examples'),
               'Search examples and patterns available') }}
```

## Common Patterns

### Progressive Disclosure
```jinja
{# Start with basics, allow expansion #}
{{ tool_section('core_tools') }}

{{ collapsible_section('advanced_tools', 
                       'tools/advanced_capabilities',
                       'Advanced tools and features available') }}

{{ collapsible_section('examples', 
                       'examples/tool_usage',
                       'Usage examples and patterns available') }}
```

### Context-Aware Instructions
```jinja
{# Adapt instructions based on current context #}
{{ switch_on_state('task_complexity_machine',
                  simple='instructions/basic_guidance',
                  complex='instructions/detailed_guidance',
                  expert='instructions/minimal_guidance') }}
```

### Conditional Tool Loading
```jinja
{# Only include tools when they're actually needed #}
{% if toggle('file_operations') == 'open' %}
{{ tool_section('file_tools') }}
{% endif %}

{% if toggle('database_access') == 'open' %}  
{{ tool_section('database_tools') }}
{% endif %}
```

## Migration from Static Prompts

### Before: Static Everything
```jinja
{{ tool_section('all_tools') }}
{{ prompt_section('examples/everything') }}
{{ prompt_section('debug/verbose_context') }}
```

### After: Selective Rendering
```jinja
{# Core tools always available #}
{{ tool_section('core_tools') }}

{# Optional tools #}
{{ collapsible_tool_section('advanced_mode', 'advanced_tools', 'Advanced tools available') }}

{# Conditional examples #}
{{ collapsible_section('examples', 'examples/relevant_examples', 'Usage examples available') }}

{# Debug info on demand #}
{{ collapsible('debug_info', 
               'Session ID: {{ session.id }}\nUser: {{ session.user }}',
               'Debug information available') }}
```

## Result: Cleaner, More Effective Prompts

The selective rendering system enables:

- **Reduced token usage** by including only relevant content
- **Better task focus** by avoiding irrelevant instructions  
- **User-controlled verbosity** through toggles and collapsible sections
- **Context-aware adaptation** through state machines
- **Modular, reusable components** through the section system

Remember: Every piece of content in your prompt should earn its place by being useful for the current task. Use these tools to make that happen automatically.