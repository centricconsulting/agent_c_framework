# Async Observable System Documentation

## Overview

The Async Observable system provides a reactive programming pattern for Pydantic models, enabling automatic responses to field changes through asynchronous callbacks. This system is particularly useful for maintaining data consistency, triggering side effects, and implementing reactive UI patterns.

## Core Components

### AsyncObservableWrapper

Located in `agent_c.util.async_observable`

A utility wrapper that provides observable functionality to any object, allowing you to register callbacks that fire when specific attributes change.

```python
from agent_c.util.async_observable import AsyncObservableWrapper, CallbackType

# Wrap any object to make it observable
wrapper = AsyncObservableWrapper(your_object)

# Register callbacks for attribute changes
async def on_change(old_value, new_value):
    print(f"Value changed from {old_value} to {new_value}")

wrapper.register_callback("attribute_name", on_change)
```

### AsyncObservableModel

Located in `agent_c.models.async_observable`

A Pydantic BaseModel subclass that provides built-in observable functionality for model fields.

```python
from agent_c.models.async_observable import AsyncObservableModel, AsyncObservableField

class MyModel(AsyncObservableModel):
    name: str = AsyncObservableField(default="")
    count: int = AsyncObservableField(default=0)
```

### AsyncObservableField

A Pydantic Field factory that marks fields as observable, enabling automatic callback registration and execution.

## Key Benefits

### 1. **Reactive Data Consistency**

Automatically maintain consistency between related data when changes occur:

```python
class AgentConfiguration(AsyncObservableModel):
    model_id: str = AsyncObservableField(default="")
    agent_params: dict = AsyncObservableField(default_factory=dict)

    async def _on_model_id_change(self, old_value: str, new_value: str):
        """Automatically update agent_params when model_id changes"""
        vendor, model = parse_model_id(new_value)
        self.agent_params = create_params_for_vendor(vendor, model)
```

### 2. **Decoupled Event Handling**

Separate business logic from data models while maintaining responsiveness:

```python
# Register external handlers
config.register_callback("model_id", update_ui_model_selector)
config.register_callback("agent_params", validate_parameters)
config.register_callback("agent_params", save_to_database)
```

### 3. **UI Synchronization**

Keep UI components synchronized with model state changes:

```python
async def sync_ui_on_model_change(old_value, new_value):
    """Update UI when model configuration changes"""
    await update_model_dropdown(new_value)
    await refresh_parameter_panel()
    await validate_model_compatibility()

agent_config.register_callback("model_id", sync_ui_on_model_change)
```

## Usage Patterns

### Basic Observable Model

```python
from agent_c.models.async_observable import AsyncObservableModel, AsyncObservableField

class UserProfile(AsyncObservableModel):
    username: str = AsyncObservableField(default="")
    email: str = AsyncObservableField(default="")
    preferences: dict = AsyncObservableField(default_factory=dict)

    async def _on_username_change(self, old_value: str, new_value: str):
        """Built-in callback for username changes"""
        print(f"Username changed: {old_value} -> {new_value}")
        await self.validate_username(new_value)
```

### External Callback Registration

```python
# Create model instance
profile = UserProfile()

# Register external callbacks
async def log_email_change(old_email: str, new_email: str):
    logger.info(f"Email updated from {old_email} to {new_email}")
    await send_verification_email(new_email)

async def update_user_cache(old_email: str, new_email: str):
    await cache.update_user_email(profile.username, new_email)

# Register multiple callbacks for the same field
profile.register_callback("email", log_email_change)
profile.register_callback("email", update_user_cache)
```

### Cascading Updates

```python
class AgentConfiguration(AsyncObservableModel):
    model_id: str = AsyncObservableField(default="")
    vendor: str = AsyncObservableField(default="")
    model_name: str = AsyncObservableField(default="")
    agent_params: dict = AsyncObservableField(default_factory=dict)

    async def _on_model_id_change(self, old_value: str, new_value: str):
        """Parse model_id and update dependent fields"""
        try:
            vendor, model = new_value.split("/", 1)
            self.vendor = vendor  # This will trigger _on_vendor_change
            self.model_name = model  # This will trigger _on_model_name_change
        except ValueError:
            logger.warning(f"Invalid model_id format: {new_value}")

    async def _on_vendor_change(self, old_value: str, new_value: str):
        """Update agent_params when vendor changes"""
        self.agent_params = get_default_params_for_vendor(new_value)

    async def _on_model_name_change(self, old_value: str, new_value: str):
        """Validate model name and update specific parameters"""
        if not await validate_model_exists(self.vendor, new_value):
            raise ValueError(f"Model {new_value} not found for vendor {self.vendor}")

        # Update model-specific parameters
        model_params = await get_model_specific_params(self.vendor, new_value)
        self.agent_params.update(model_params)
```

## Real-World Example: Agent Configuration Management

Here's how the system is actually implemented in the codebase:

```python
class BaseAgentConfiguration(AsyncObservableModel):
    """Base configuration with common fields across all agent configuration versions"""
    name: str = Field(..., description="Name of the agent")
    key: str = Field(..., description="Key for the agent configuration, used for identification")
    agent_description: Optional[str] = Field(None, description="A description of the agent's purpose and capabilities")
    tools: List[str] = Field(default_factory=list, description="List of enabled toolset names the agent can use")
    agent_params: Optional[CompletionParams] = AsyncObservableField(None, description="Parameters for the interaction with the agent")
    prompt_metadata: Optional[dict[str, Any]] = Field(None, description="Metadata for the prompt")
    persona: str = Field(..., description="Persona prompt defining the agent's behavior")
    uid: Optional[str] = Field(None, description="Unique identifier for the configuration")

    def __init__(self, **data) -> None:
        super().__init__(**data)
        model_id = self.agent_params.model_id
        self._current_model_vendor: str = ModelConfigurationLoader.instance().model_id_map[model_id].vendor
        # Register observer on the nested agent_params object
        self.agent_params.add_observer(self.on_agent_parms_model_id_change, "model_id")

    def on_agent_parms_model_id_change(self, old_value: str, new_value: str) -> None:
        """
        Callback when the model_id in agent_params changes.
        Automatically updates agent_params to the correct vendor-specific type.
        """
        # Handle case where agent_params doesn't have a type field yet
        if 'type' not in self.agent_params:
            self._current_model_vendor = ModelConfigurationLoader.instance().model_id_map[new_value].vendor
            self.agent_params = ModelConfigurationLoader.instance().default_params_for_model(new_value)
            return

        # Skip if no actual change
        if old_value == new_value:
            return

        # Check if vendor has changed
        new_value_vendor = ModelConfigurationLoader.instance().model_id_map[new_value].vendor
        if self._current_model_vendor == new_value_vendor:
            return

        # Vendor changed - update to new vendor's default parameters
        self._current_model_vendor = new_value_vendor
        self.agent_params = ModelConfigurationLoader.instance().default_params_for_model(new_value)
```

### Key Implementation Details:

1. **Nested Observable**: The `agent_params` field is marked as `AsyncObservableField`, and the callback is registered on the nested `CompletionParams` object's `model_id` field.

2. **Vendor Tracking**: The system tracks the current model vendor (`_current_model_vendor`) to avoid unnecessary updates when switching between models from the same vendor.

3. **ModelConfigurationLoader Integration**: Uses the existing `ModelConfigurationLoader` singleton to:
   
   - Map model IDs to vendor information
   - Get default parameters for specific models

4. **Smart Updates**: Only triggers parameter replacement when the vendor actually changes, preserving user customizations when switching between models from the same vendor.

This implementation ensures that when the UI changes the model ID in `agent_params`, the system automatically updates the entire `agent_params` object to the correct vendor-specific type with appropriate defaults.

## Advanced Features

### Conditional Callbacks

```python
class ConditionalModel(AsyncObservableModel):
    status: str = AsyncObservableField(default="inactive")
    data: dict = AsyncObservableField(default_factory=dict)

    async def _on_data_change(self, old_value: dict, new_value: dict):
        """Only process data changes when status is active"""
        if self.status == "active":
            await self.process_data_change(old_value, new_value)
```

### Callback Priorities

```python
# Register callbacks with priorities (lower number = higher priority)
model.register_callback("field", high_priority_callback, priority=1)
model.register_callback("field", normal_callback, priority=5)
model.register_callback("field", low_priority_callback, priority=10)
```

### Error Handling

```python
async def safe_callback(old_value, new_value):
    try:
        await risky_operation(new_value)
    except Exception as e:
        logger.error(f"Callback failed: {e}")
        # Optionally revert change
        # model.field = old_value

model.register_callback("field", safe_callback)
```

## Best Practices

### 1. **Name Conventions**

- Use `_on_<field_name>_change` for built-in model callbacks
- Use descriptive names for external callbacks: `update_ui_on_model_change`

### 2. **Error Handling**

- Always handle exceptions in callbacks to prevent cascade failures
- Log errors appropriately for debugging
- Consider whether to revert changes on callback failures

### 3. **Performance Considerations**

- Avoid heavy computations in callbacks
- Use debouncing for rapid successive changes
- Consider async/await for I/O operations

### 4. **Testing**

- Mock external dependencies in callback tests
- Test both successful and error scenarios
- Verify callback execution order when multiple callbacks are registered

## Migration Guide

### From Regular Pydantic Models

```python
# Before
class OldModel(BaseModel):
    name: str = Field(default="")

# After  
class NewModel(AsyncObservableModel):
    name: str = AsyncObservableField(default="")

    async def _on_name_change(self, old_value: str, new_value: str):
        # Add reactive behavior
        pass
```

### Adding Observability to Existing Models

```python
# Existing model
class ExistingModel(BaseModel):
    field: str = Field(default="")

# Add observability
class ObservableExistingModel(AsyncObservableModel, ExistingModel):
    field: str = AsyncObservableField(default="")

    async def _on_field_change(self, old_value: str, new_value: str):
        # New reactive behavior
        pass
```

## Troubleshooting

### Common Issues

1. **Callbacks not firing**: Ensure fields are marked with `AsyncObservableField`
2. **Infinite loops**: Be careful with callbacks that modify other observed fields
3. **Performance issues**: Avoid synchronous operations in async callbacks
4. **Memory leaks**: Properly unregister callbacks when objects are destroyed

### Debugging

```python
# Enable debug logging
import logging
logging.getLogger('agent_c.models.async_observable').setLevel(logging.DEBUG)

# Add debug callback
async def debug_callback(old_value, new_value):
    print(f"Field changed: {old_value} -> {new_value}")

model.register_callback("field", debug_callback)
```

## API Reference

### AsyncObservableModel Methods

- `register_callback(field_name: str, callback: CallbackType, priority: int = 5)`
- `unregister_callback(field_name: str, callback: CallbackType)`
- `clear_callbacks(field_name: str = None)`

### AsyncObservableWrapper Methods

- `register_callback(attr_name: str, callback: CallbackType)`
- `unregister_callback(attr_name: str, callback: CallbackType)`
- `trigger_callbacks(attr_name: str, old_value, new_value)`

### CallbackType

```python
CallbackType = Callable[[Any, Any], Awaitable[None]]
```

Callbacks receive `(old_value, new_value)` and must be async functions returning None.