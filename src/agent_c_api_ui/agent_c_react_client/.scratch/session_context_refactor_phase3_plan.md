# SessionContext Refactor - Phase 3 Planning

After successfully completing Phase 2, this document outlines the plan for Phase 3: Creating the ModelContext.

## Phase 2 Review

Phase 2 involved:
1. Creating a new focused SessionContext for core session management
2. Renaming the original SessionContext to LegacySessionContext
3. Making LegacySessionContext use the new SessionContext internally
4. Fixing bugs and ensuring backward compatibility

We encountered several challenges:
- Components using the wrong context reference
- API error handling issues during session validation
- Context initialization and dependency management

## Phase 3 Overview: ModelContext

Phase 3 will extract model-related functionality from LegacySessionContext into a dedicated ModelContext. This will include:

- Model selection
- Model parameters management
- Model initialization and configuration
- API interactions specific to models

## Detailed Implementation Plan

### 1. Create the ModelContext

```jsx
// src/contexts/ModelContext.jsx
import React, { createContext, useState, useEffect, useRef, useContext } from 'react';
import { SessionContext } from './SessionContext';
import { model as modelService, showErrorToast } from '../services';

export const ModelContext = createContext();

export const ModelProvider = ({ children }) => {
  // Use the core SessionContext
  const { sessionId, isReady } = useContext(SessionContext);
  
  // State for model configuration
  const [modelConfigs, setModelConfigs] = useState([]);
  const [modelName, setModelName] = useState("");
  const [modelParameters, setModelParameters] = useState({});
  const [selectedModel, setSelectedModel] = useState(null);
  const [error, setError] = useState(null);
  
  // Helper for debouncing parameter updates
  const debouncedUpdateRef = useRef(null);
  
  // Fetch available models
  const fetchModels = async () => {
    try {
      const data = await modelService.getModels();
      setModelConfigs(data.models);
      return data.models;
    } catch (err) {
      setError(`Failed to fetch models: ${err.message}`);
      showErrorToast(err, 'Failed to fetch models');
      return [];
    }
  };
  
  // Change the active model
  const changeModel = async (newModelName) => {
    if (!sessionId || !isReady) return;
    
    try {
      const newModel = modelConfigs.find(model => model.id === newModelName);
      if (!newModel) throw new Error('Invalid model configuration');
      
      // Update the state first
      setModelName(newModelName);
      setSelectedModel(newModel);
      
      // ... implementation continues
    } catch (err) {
      setError(`Failed to change model: ${err.message}`);
      showErrorToast(err, 'Failed to change model');
    }
  };
  
  // Update model parameters
  const updateModelParameters = async (newParameters) => {
    // ... implementation
  };
  
  // Effects and other logic
  // ...
  
  return (
    <ModelContext.Provider
      value={{
        modelConfigs,
        modelName,
        modelParameters, 
        selectedModel,
        error,
        fetchModels,
        changeModel,
        updateModelParameters
      }}
    >
      {children}
    </ModelContext.Provider>
  );
};
```

### 2. Create useModel Hook

```jsx
// src/hooks/use-model.js
import { useContext } from 'react';
import { ModelContext } from '../contexts/ModelContext';

export function useModel() {
  const context = useContext(ModelContext);
  
  if (!context) {
    throw new Error('useModel must be used within a ModelProvider');
  }
  
  return context;
}
```

### 3. Update LegacySessionContext

Modify LegacySessionContext to use ModelContext for model-related state and operations.

### 4. Update App.jsx

Add ModelProvider to the context hierarchy:

```jsx
// src/App.jsx
import { SessionProvider } from '@/contexts/SessionContext';
import { ModelProvider } from '@/contexts/ModelContext';
import { LegacySessionProvider } from '@/contexts/LegacySessionContext';
import { ThemeProvider } from '@/contexts/ThemeProvider';

function App() {
  return (
    <SessionProvider>
      <ModelProvider>
        <LegacySessionProvider>
          <ThemeProvider>
            <Router>
              <AppRoutes />
            </Router>
          </ThemeProvider>
        </LegacySessionProvider>
      </ModelProvider>
    </SessionProvider>
  );
}
```

### 5. Update Components

Gradually update components to use the new ModelContext directly instead of going through LegacySessionContext.

## Implementation Strategy

1. **Parallel Development**: Implement the new ModelContext while maintaining LegacySessionContext
2. **Incremental Migration**: Update components one by one to use the new context
3. **Thorough Testing**: Test each component after updating
4. **Backward Compatibility**: Ensure LegacySessionContext remains functional throughout the process

## Potential Challenges

1. **State Synchronization**: Keeping model state synchronized between contexts during transition
2. **Initialization Order**: Ensuring contexts initialize in the correct order
3. **Session-Model Interaction**: Managing the interaction between session and model contexts
4. **API Integration**: Properly separating model API calls from session API calls

## Expected Outcome

After completing Phase 3, we will have:

1. A focused ModelContext that handles all model-related functionality
2. Components that directly use ModelContext for model operations
3. A cleaner LegacySessionContext with reduced responsibilities
4. Better separation of concerns between session and model management

## Next Steps

After completing Phase 3, we will proceed to Phase 4: Creating UIStateContext.