# ModelContext Implementation

## Overview

The `ModelContext` provides model management functionality for the Agent C React UI. It handles model selection, configuration, parameter management, and related API operations.

## Key Features

- Model configuration state management
- Parameter handling for different model types
- Persistent storage of model preferences
- API integration for model updates
- Error handling and loading states

## Usage

### Context Provider

The `ModelProvider` should be placed after `AuthProvider` but before `SessionProvider`:

```jsx
// App.jsx
import { ModelProvider } from '@/contexts/ModelContext';

function App() {
  return (
    <ThemeProvider>
      <AuthProvider>
        <ModelProvider>
          <SessionProvider>
            {/* App content */}
          </SessionProvider>
        </ModelProvider>
      </AuthProvider>
    </ThemeProvider>
  );
}
```

### Using the Hook

In your components, use the `useModel` hook to access model functionality:

```jsx
import { useModel } from '@/hooks/use-model';

function ModelSelector() {
  const { 
    modelConfigs,
    modelName,
    selectedModel,
    modelParameters,
    isLoading,
    error,
    changeModel,
    updateModelParameters
  } = useModel('ModelSelector'); // Component name for logging
  
  // Use model functionality here
  
  return (
    // Your component JSX
  );
}
```

## API Reference

### State

- `modelConfigs` (Array): Available model configurations
- `modelName` (string): Current selected model ID
- `selectedModel` (Object): The complete model object for the current selection
- `modelParameters` (Object): Current model parameters like temperature, reasoning_effort, etc.
- `isLoading` (boolean): Whether model data is being loaded
- `error` (string|null): Any error that occurred during model operations

### Methods

#### `fetchModels()`

Fetches available models from the API.

- Returns: Promise<Array> - The fetched models

#### `changeModel(newModelName)`

Changes the currently selected model.

- `newModelName` (string): ID of the model to change to
- Returns: Promise<boolean> - Success status

#### `updateModelParameters(parameterUpdates)`

Updates the parameters for the current model.

- `parameterUpdates` (Object): Object containing parameter updates
- Returns: Promise<boolean> - Success status

#### `initializeWithModel(model)`

Initializes with a specific model or restores from saved configuration.

- `model` (Object): Optional model to initialize with
- Returns: Promise<boolean> - Success status

## Implementation Details

- Uses `localStorage` (via storageService) to persist model preferences
- Implements debouncing for parameter updates to avoid excessive API calls
- Handles model-specific parameter defaults
- Supports both OpenAI's reasoning_effort and Claude's extended_thinking parameters
- Automatically fetches available models on mount

## Configuration Management

The context handles configuration persistence with:

- Automatic saving of model selections and parameters
- Restoration of saved preferences on initialization
- Configuration expiration (14 days by default)
- Version checking to handle model updates