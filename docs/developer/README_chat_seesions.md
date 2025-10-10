# ChatSession Architecture Guide

## Overview

The ChatSession system implements a sophisticated, reactive architecture that automatically manages configurations, contexts, and resources for AI agents and tools. It uses several advanced patterns to ensure that when tools are added or removed, all required dependencies are automatically handled.

## Core Architectural Patterns

### 1. Registry Pattern

The registry pattern allows for dynamic registration and creation of objects without tight coupling. Instead of hard-coding which classes to instantiate, we register them dynamically and create instances on-demand.

**Why we use it:**
- **Extensibility**: New section types can be added without modifying core code
- **Polymorphic deserialization**: We can recreate the correct object type from serialized data
- **Plugin architecture**: Tools and sections can register their own types
- **Version compatibility**: Old serialized data can be upgraded automatically

**Example from SectionRegistry:**
```python
# Registration (usually done at module import)
@SectionRegistry.register_section_class
class CustomToolSection(BasePromptSection):
    section_type: str = "custom_tool"
    # ... section implementation

# Later, dynamic creation
section = SectionRegistry.create("custom_tool", {"param": "value"})
# Returns a CustomToolSection instance
```

### 2. Observable Pattern

Observables allow objects to notify other objects when their state changes, enabling reactive programming where changes automatically propagate through the system.

**Why we use it:**
- **Automatic dependency management**: When a tool is added, all its required configs/contexts are automatically created
- **Consistency**: User models stay in sync with session requirements
- **Decoupling**: Objects don't need to know about each other directly
- **Event-driven architecture**: Changes trigger appropriate handlers

**Example from ChatSession:**
```python
def toolset_added(self, item: str, index: int) -> None:
    """Called automatically when a tool is added"""
    self._add_tool_extras(item)  # Creates required contexts/configs
    self.trigger('toolset_added', item, index)  # Notify other observers
```

### 3. Container Classes with Registry-Powered Auto-Creation

Our container classes (SectionBag, ContextBag) implement "lazy initialization" - if you ask for something that doesn't exist, they automatically request a default instance from the appropriate registry.

**Why we use it:**
- **Convenience**: No need to manually check if something exists before using it
- **Consistent defaults**: Every tool gets the same default configuration from the registry
- **Fail-safe**: Missing configurations don't break the system
- **Self-healing**: Corrupted or incomplete data gets filled in automatically via registry defaults

**Critical**: Always use dot notation (`container.item_name`) rather than bracket notation (`container["item_name"]`) to ensure the registry auto-creation mechanism is triggered.

**Example from SectionBag:**
```python
def __getattr__(self, name: str) -> Any:
    """Dot notation triggers registry-based auto-creation"""
    try:
        return self.__getitem__(name)
    except KeyError:
        # Ask registry to create default if type is registered
        if SectionRegistry.is_section_registered(name):
            value = SectionRegistry.create(name)  # Registry creates it
            self[name] = value  # Store for next time
            return value
```

## ChatSession Initialization Flow

### 1. Pre-Validation (`validate_session`)
```python
@model_validator(mode='before')
def validate_session(cls, values: Dict[str, Any]) -> Dict[str, Any]:
    if 'session_id' not in values or not values['session_id']:
        values['session_id'] = cls.__new_session_id(**values)
    return values
```
- Generates session ID if not provided
- Ensures required fields are present

### 2. Pydantic Model Creation
- All fields are validated and typed according to their field definitions
- Default factories create empty collections (SectionBag, ContextBag, etc.)

### 3. Post-Initialization (`post_init`)
This is where the magic happens:

```python
@model_validator(mode='after')
def post_init(self) -> 'ChatSession':
    self._build_machines()                           # 1
    # ... session ID logic ...                      # 2
    self._add_contexts_for_user()                   # 3
    self._ensure_extras_for_agent_config(...)       # 4
    DefaultChatSessionManager.instance().add_session(self)  # 5
    return self
```

#### Step 1: Build State Machines
Converts state machine templates into actual running instances.

#### Step 2: User Context Setup
```python
def _add_contexts_for_user(self) -> None:
    if self.user:
        for context_type, context in self.user.context:
            self.context[context_type] = self.user.context[context_type].duplicate()
```
Copies all user default contexts into the session context bag.

#### Step 3: Agent Configuration Setup
```python
def _ensure_extras_for_agent_config(self, agent_config) -> None:
    if agent_config:
        self._ensure_extras_for_agent_tools(agent_config)  # Tool setup
        
        # Copy agent sections to session
        for section_type in agent_config.sections.section_types:
            if section_type not in self.sections:
                section = agent_config.sections[section_type]
                self.sections[section_type] = section
```

This ensures the session has everything it needs to run the configured agent.

## Configuration Layering System

The system implements a three-layer configuration hierarchy:

### Layer 1: Registry Defaults
When a section/context/config type is first accessed, the registry provides a default instance with sensible defaults.

### Layer 2: User Defaults
Users can override registry defaults. These become the "user's normal settings" for that tool/context type.

### Layer 3: Session Overrides
Individual sessions can override user defaults for specific use cases.

**Example Flow:**
```python
# 1. Tool gets added to agent config
agent_config.tools.append("web_search")

# 2. Observable triggers in ChatSession
def toolset_added(self, item: str, index: int) -> None:
    self._add_tool_extras(item)

# 3. Tool extras get added
def _add_tool_extras(self, tool_name: str) -> None:
    cls = self._tool_registry.get(tool_name)
    
    # Ensure user has default config for this tool
    for config_type in cls.config_types:
        if config_type not in self.user.config:
            # This triggers auto-creation in the user's config bag
            self.user.config[config_type] = self.user.config[config_type]
    
    # Ensure session has required contexts
    for context_type in cls.context_types:
        if context_type not in self.context:
            self.context[context_type] = self.context[context_type]
```

## Event System and Reactivity

### Observable Collections
Most collections in the system are observable, meaning they emit events when items are added, removed, or changed.

**Key Observable Collections:**
- `agent_config.tools` - Tool list changes
- `sections` - Prompt section changes  
- `context` - Context bag changes
- `state_machines` - State machine changes

### Event Handlers
The ChatSession has handlers for various events:

```python
def on_machine_added(self, template: StateMachineTemplate) -> None:
    """Auto-compile state machine when added"""
    if template.machine is None:
        template.machine = self.machines.add_machine(template.name, template)

def toolset_added(self, item: str, index: int) -> None:
    """Auto-setup when tool added"""
    self._add_tool_extras(item)
    self.trigger('toolset_added', item, index)
```

### Cascading Updates
Changes propagate automatically:

1. **Tool Added** → Required configs created in user model → Required contexts created in session → Required sections registered
2. **Tool Removed** → Unused configs cleaned up → Unused contexts removed → Unused sections removed
3. **User Config Changed** → Session context updated → Dependent sections reconfigured

## Metaprogramming Features

### Dynamic Key Normalization
```python
def _normalize_key(item):
    """Convert various input types to consistent keys"""
    if isinstance(item, type):
        return to_snake_case(item.__name__)  # MyClass → "my_class"
    elif isinstance(item, str):
        return to_snake_case(item)           # "MyClass" → "my_class"
    # ...
```

This allows flexible access patterns, with dot notation being the preferred method:
```python
# PREFERRED - triggers auto-creation:
session.sections.my_tool_section    # Using attribute access (recommended)

# ALSO WORKS but less preferred:
session.sections[MyToolSection]     # Using class
session.sections["MyToolSection"]   # Using string
```

### Attribute/Dictionary Duality and Auto-Creation
Container classes support both dictionary and attribute access, but **dot notation should always be used** because it triggers the auto-creation mechanism:

```python
# CORRECT - triggers auto-creation if missing:
session.sections.web_search
session.context.user_preferences
interaction.context.search_results

# AVOID - may not trigger auto-creation:
session.sections["web_search"]
session.context["user_preferences"]
```

The dot notation ensures that if the requested item doesn't exist, the container will automatically create a default instance and add it to the collection.

### Auto-Upgrade on Deserialization
When loading from serialized data, the registry pattern ensures objects are recreated with current class definitions, automatically upgrading old data to new schemas.

## InteractionContext and Template Rendering

### The InteractionContext Model

The `InteractionContext` represents a single interaction with an agent. It contains temporary data that's only needed for the duration of that specific interaction, separate from the persistent `ChatSession` data.

**Key Features:**
- **Temporary Storage**: Holds context and sections that are discarded after the interaction
- **Token Optimization**: Allows injecting information only when needed, then removing it
- **Hierarchical Structure**: Can have parent-child relationships for complex interactions
- **Integration Layer**: Bridges between the chat session and the agent runtime

### Template Context Provisioning

When rendering system prompts with Jinja2, the system provides two main context objects to templates:

```python
# Template receives these variables:
{
    'context': chat_session.context,           # Persistent session context
    'interaction_context': current_interaction, # Temporary interaction data
    # ... other template variables
}
```

This design eliminates verbose template syntax. Instead of writing:
```jinja2
<!-- Verbose -->
{{ context.chat_session.context.user_preferences.timezone }}
{{ context.chat_session.sections.web_search.api_key }}
```

Templates can use clean, direct access:
```jinja2
<!-- Clean -->
{{ context.user_preferences.timezone }}
{{ interaction_context.sections.web_search.api_key }}
```

### Interaction-Specific Context Injection

The `InteractionContext` allows temporary injection of context and sections:

```python
# Example: Adding temporary context for a specific interaction
interaction = InteractionContext(
    chat_session=session,
    inputs=user_inputs,
    # ... other fields
)

# Registry creates default search results context automatically
search_results_context = interaction.context.search_results_context
search_results_context.query = "python asyncio"
search_results_context.results = [...]
search_results_context.timestamp = datetime.now()

# Registry creates default search formatter section automatically  
search_formatter = interaction.sections.search_formatter
search_formatter.format_style = "concise"
search_formatter.max_results = 5
```

**Template Usage:**
```jinja2
{% if interaction_context.context.search_results %}
## Recent Search Results
Query: {{ interaction_context.context.search_results.query }}
{% for result in interaction_context.context.search_results.results %}
- {{ result.title }}: {{ result.summary }}
{% endfor %}
{% endif %}

{% if interaction_context.sections.search_formatter %}
{{ interaction_context.sections.search_formatter.render() }}
{% endif %}
```

### Context Merging and Resolution

During interaction initialization, the system merges contexts:

```python
def _ensure_extras_for_sections(self):
    """Merge section contexts into interaction context"""
    for section_type, section in self.sections.items():
        self.context.merge(section.context)
```

This creates a unified context bag that includes:
1. **Base Context**: From the chat session's persistent context
2. **Section Context**: From any prompt sections in the interaction
3. **Temporary Context**: Injected specifically for this interaction

### Token Optimization Strategy

The interaction-based context system enables significant token savings:

**Before Interaction:**
```python
# Clean session context - only persistent data
session.context = {
    "user_preferences": UserPreferencesContext(...),
    "conversation_history": ConversationContext(...)
}
```

**During Interaction:**
```python
# Registry automatically creates enhanced context on first access
interaction.context = {
    # Inherited from session
    "user_preferences": UserPreferencesContext(...),
    "conversation_history": ConversationContext(...),
    
    # Registry creates these when first accessed via dot notation
    "current_search": SearchContext(results=[...]),
    "api_responses": APIResponseContext(data=[...]),
    "temp_calculations": CalculationContext(values=[...])
}
```

**After Interaction:**
```python
# Back to clean session context
# Temporary contexts are discarded automatically
session.context = {
    "user_preferences": UserPreferencesContext(...),
    "conversation_history": ConversationContext(...)
}
```

### Practical Examples

#### Temporary API Integration Context
```python
# Registry creates default file processor context automatically
file_processor = interaction.context.file_processor
file_processor.current_file = "data.csv"
file_processor.processing_options = {"encoding": "utf-8", "delimiter": ","}
file_processor.temp_results = processing_data

# Template can access this directly
```
```jinja2
{% if interaction_context.context.file_processor %}
Currently processing: {{ interaction_context.context.file_processor.current_file }}
Options: {{ interaction_context.context.file_processor.processing_options | tojson }}
{% endif %}
```

#### Dynamic Section Injection
```python
# Registry creates default web search instructions section automatically
if needs_web_search:
    web_search_section = interaction.sections.web_search_instructions
    web_search_section.search_depth = "thorough"
    web_search_section.source_requirements = ["academic", "recent"]
    web_search_section.result_format = "detailed"
```

#### Hierarchical Interactions
```python
# Parent interaction for complex task
parent_interaction = InteractionContext(
    interaction_id="task-main",
    chat_session=session,
    # ... setup
)

# Child interaction for subtask
child_interaction = InteractionContext(
    interaction_id="task-subtask-1",
    chat_session=session,
    parent_context=parent_interaction,
    # Inherits parent context but can add its own
)
```

### Accessing Auto-Created Objects via Registry
```python
# CORRECT - registry creates default if missing:
context = session.context.web_search_context
search_context = interaction.context.current_search_results  
section = session.sections.web_search

# The registry provides defaults, then user defaults override, then session can override
# No manual object creation needed - the registry handles everything
```

## Benefits of This Architecture

1. **Self-Healing**: Missing configurations are auto-created
2. **Consistent State**: User and session models stay synchronized
3. **Extensible**: New tools/sections can be added without core changes
4. **Version Safe**: Old serialized data gets automatically upgraded
5. **Developer Friendly**: Complex dependency management is handled automatically
6. **Type Safe**: Pydantic ensures data integrity throughout

## Debugging Tips

1. **Registry Issues**: Check if types are properly registered with `SectionRegistry.list_types()`
2. **Observable Events**: Look for event handlers that might be triggering unexpected behavior
3. **Auto-Creation**: If objects appear "magically", check the container's `__getattr__` method. Always use dot notation (`obj.property`) rather than bracket notation (`obj["property"]`) to ensure auto-creation works
4. **Serialization**: Use `model_dump_serializable()` to see what actually gets saved
5. **State Propagation**: Follow the event chain from tool addition through to final configuration

This architecture ensures that the complex dependency web between tools, configurations, contexts, and sessions is managed automatically and consistently, allowing developers to focus on business logic rather than infrastructure management.