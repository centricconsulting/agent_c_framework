# SessionContext Refactor - Phase 3 (Revised): ModelContext Implementation

## Introduction

After careful analysis of the codebase and reviewing the execution of Phase 2, we've identified several critical issues that must be addressed in our Phase 3 implementation. The original plan failed to account for complex interactions between components, potential race conditions, and the intricate interdependencies of model-related state management.

This revised plan provides a more robust approach to implementing the ModelContext with focus on preventing race conditions, maintaining state synchronization, and ensuring a smooth transition for components.

## Critical Issues Identified

1. **Initialization Race Conditions**: The SessionContext and ModelContext have complex initialization dependencies. ModelContext needs session information but must initialize at the right moment to prevent errors.

2. **State Synchronization Problems**: Both LegacySessionContext and ModelContext could maintain duplicate model state, creating synchronization issues when changes occur in either context.

3. **Conflicting Storage Operations**: Both contexts might interact with the same localStorage keys for persisting preferences.

4. **API Call Redundancy**: Changes to model parameters could trigger redundant API calls from different contexts.

5. **Model Parameter Complexity**: Different model types have varying parameter requirements and validation rules that need careful handling.

6. **Update Propagation**: Changes in the ModelContext need to be correctly propagated to components that still use the LegacySessionContext.

## Revised Implementation Strategy

### 1. Clear State Ownership and Responsibility

- **SessionContext**: Single source of truth for session identity and status
- **ModelContext**: Single source of truth for all model-related state and operations
- **LegacySessionContext**: Acts as a bridge during transition, proxying to both contexts but not duplicating state

### 2. Controlled Initialization Sequence

- ModelContext must initialize only after SessionContext is ready
- ModelContext will handle its own initialization logic when given a valid session
- Error and loading states will be properly maintained during complex initialization

### 3. Unified API Call Paths

- All model-related API calls will go exclusively through ModelContext
- LegacySessionContext will proxy model operations to ModelContext
- Single implementation of debouncing for parameter updates

### 4. Detailed Validation and Error Handling

- Comprehensive validation for all model parameters
- Clear error messages for initialization and operation failures
- Consistent error handling across all API calls

## Implementation Plan

### Step 1: Create Core ModelContext Implementation

```jsx
// src/contexts/ModelContext.jsx
import React, { createContext, useState, useEffect, useRef, useContext, useCallback } from 'react';
import { SessionContext } from './SessionContext';
import { model as modelService } from '../services';
import { useToast } from '../hooks/use-toast';

// Constants
const MODEL_PARAM_UPDATE_DEBOUNCE = 500; // ms
const LOCAL_STORAGE_MODEL_KEY = 'agent-c-model-preference';

export const ModelContext = createContext(null);

export const ModelProvider = ({ children }) => {
  // Core dependencies
  const { sessionId, isAuthenticated, isReady: isSessionReady } = useContext(SessionContext);
  const { toast } = useToast();
  
  // State management
  const [isInitialized, setIsInitialized] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);
  
  // Model state
  const [modelConfigs, setModelConfigs] = useState([]);
  const [modelName, setModelName] = useState("");
  const [modelParameters, setModelParameters] = useState({});
  const [selectedModel, setSelectedModel] = useState(null);
  
  // Refs for internal operations
  const paramUpdateTimeoutRef = useRef(null);
  const modelInitAttempted = useRef(false);
  
  // Clear any error state
  const clearError = useCallback(() => {
    if (error) setError(null);
  }, [error]);

  // Load saved model preference from localStorage
  const getSavedModelPreference = useCallback(() => {
    try {
      const savedModel = localStorage.getItem(LOCAL_STORAGE_MODEL_KEY);
      return savedModel ? JSON.parse(savedModel) : null;
    } catch (e) {
      console.warn('Failed to parse saved model preference', e);
      return null;
    }
  }, []);
  
  // Save model preference to localStorage
  const saveModelPreference = useCallback((modelId) => {
    try {
      localStorage.setItem(LOCAL_STORAGE_MODEL_KEY, JSON.stringify(modelId));
    } catch (e) {
      console.warn('Failed to save model preference', e);
    }
  }, []);

  // Fetch available models
  const fetchModels = useCallback(async () => {
    if (!sessionId || !isAuthenticated) return [];
    
    setIsLoading(true);
    clearError();
    
    try {
      console.log('ModelContext: Fetching available models');
      const data = await modelService.getModels();
      setModelConfigs(data.models || []);
      return data.models || [];
    } catch (err) {
      const errorMsg = `Failed to fetch models: ${err.message || 'Unknown error'}`;
      console.error('ModelContext:', errorMsg, err);
      setError(errorMsg);
      toast({
        title: 'Error',
        description: 'Failed to fetch available models',
        variant: 'destructive',
      });
      return [];
    } finally {
      setIsLoading(false);
    }
  }, [sessionId, isAuthenticated, clearError, toast]);

  // Initialize model with saved preference or default
  const initializeModel = useCallback(async () => {
    if (!sessionId || !isAuthenticated || !isSessionReady || modelInitAttempted.current) {
      return;
    }
    
    modelInitAttempted.current = true;
    setIsLoading(true);
    clearError();
    
    try {
      console.log('ModelContext: Initializing model configuration');
      const models = await fetchModels();
      
      if (!models || models.length === 0) {
        throw new Error('No models available');
      }
      
      // Get saved preference or use first available model
      const savedPreference = getSavedModelPreference();
      const modelToUse = savedPreference && models.find(m => m.id === savedPreference) 
        ? savedPreference
        : models[0].id;
      
      await selectModel(modelToUse);
      setIsInitialized(true);
      console.log(`ModelContext: Successfully initialized with model ${modelToUse}`);
    } catch (err) {
      const errorMsg = `Model initialization failed: ${err.message || 'Unknown error'}`;
      console.error('ModelContext:', errorMsg, err);
      setError(errorMsg);
      toast({
        title: 'Error',
        description: 'Failed to initialize model configuration',
        variant: 'destructive',
      });
    } finally {
      setIsLoading(false);
    }
  }, [sessionId, isAuthenticated, isSessionReady, fetchModels, getSavedModelPreference, toast, clearError]);

  // Select a model by ID
  const selectModel = useCallback(async (modelId) => {
    if (!sessionId || !isAuthenticated || !modelId) {
      console.warn('ModelContext: Cannot select model - invalid state or modelId');
      return;
    }
    
    setIsLoading(true);
    clearError();
    
    try {
      const targetModel = modelConfigs.find(model => model.id === modelId);
      if (!targetModel) {
        throw new Error(`Model "${modelId}" not found in available models`);
      }
      
      console.log(`ModelContext: Selecting model ${modelId}`);
      
      // Update local state first for responsive UI
      setModelName(modelId);
      setSelectedModel(targetModel);
      
      // Set default parameters for the model
      const defaultParams = modelService.getDefaultParameters(targetModel);
      setModelParameters(defaultParams);
      
      // Update session with the model selection
      await modelService.setModel({ 
        sessionId, 
        modelName: modelId,
        parameters: defaultParams
      });
      
      // Save preference
      saveModelPreference(modelId);
      
      console.log(`ModelContext: Model ${modelId} successfully selected`);
    } catch (err) {
      const errorMsg = `Failed to select model: ${err.message || 'Unknown error'}`;
      console.error('ModelContext:', errorMsg, err);
      setError(errorMsg);
      toast({
        title: 'Error',
        description: `Could not select model "${modelId}"`,
        variant: 'destructive',
      });
    } finally {
      setIsLoading(false);
    }
  }, [sessionId, isAuthenticated, modelConfigs, clearError, saveModelPreference, toast]);

  // Change model (public method alias for selectModel)
  const changeModel = useCallback((newModelId) => {
    return selectModel(newModelId);
  }, [selectModel]);

  // Update model parameters with debouncing
  const updateModelParameters = useCallback((newParams) => {
    if (!sessionId || !isAuthenticated || !modelName) {
      console.warn('ModelContext: Cannot update parameters - invalid state');
      return;
    }
    
    // Clear any existing timeout
    if (paramUpdateTimeoutRef.current) {
      clearTimeout(paramUpdateTimeoutRef.current);
    }
    
    // Update state immediately for responsive UI
    setModelParameters(currentParams => ({
      ...currentParams,
      ...newParams
    }));
    
    // Debounce the API call
    paramUpdateTimeoutRef.current = setTimeout(async () => {
      clearError();
      try {
        console.log('ModelContext: Updating model parameters', newParams);
        const updatedParams = {
          ...modelParameters,
          ...newParams
        };
        
        await modelService.updateParameters({
          sessionId,
          modelName,
          parameters: updatedParams
        });
        
        console.log('ModelContext: Parameters updated successfully');
      } catch (err) {
        const errorMsg = `Failed to update model parameters: ${err.message || 'Unknown error'}`;
        console.error('ModelContext:', errorMsg, err);
        setError(errorMsg);
        toast({
          title: 'Error',
          description: 'Could not update model parameters',
          variant: 'destructive',
        });
      }
    }, MODEL_PARAM_UPDATE_DEBOUNCE);
  }, [sessionId, isAuthenticated, modelName, modelParameters, clearError, toast]);

  // Reset model parameters to defaults
  const resetModelParameters = useCallback(() => {
    if (!selectedModel) return;
    
    const defaultParams = modelService.getDefaultParameters(selectedModel);
    updateModelParameters(defaultParams);
  }, [selectedModel, updateModelParameters]);

  // Initialization effect - runs when session is ready
  useEffect(() => {
    if (isSessionReady && !isInitialized && !modelInitAttempted.current) {
      console.log('ModelContext: Session ready, initializing model');
      initializeModel();
    }
  }, [isSessionReady, isInitialized, initializeModel]);

  // Cleanup on unmount
  useEffect(() => {
    return () => {
      if (paramUpdateTimeoutRef.current) {
        clearTimeout(paramUpdateTimeoutRef.current);
      }
    };
  }, []);

  // The context value
  const contextValue = {
    // State
    isInitialized,
    isLoading,
    error,
    modelConfigs,
    modelName,
    modelParameters,
    selectedModel,
    
    // Actions
    fetchModels,
    changeModel,
    updateModelParameters,
    resetModelParameters,
    clearError,
  };

  return (
    <ModelContext.Provider value={contextValue}>
      {children}
    </ModelContext.Provider>
  );
};
```

### Step 2: Create a ModelService Utility for Parameter Management

```js
// src/services/model-api.js
import { api } from './api';

// Helper to get default parameters for a model
function getDefaultParameters(modelConfig) {
  if (!modelConfig) return {};
  
  const defaultParams = {};
  
  // Handle different parameter types appropriately
  if (modelConfig.parameters) {
    modelConfig.parameters.forEach(param => {
      defaultParams[param.id] = param.default !== undefined ? param.default : null;
    });
  }
  
  return defaultParams;
}

// Get available models
async function getModels() {
  return api.get('/models');
}

// Set active model for session
async function setModel({ sessionId, modelName, parameters = {} }) {
  return api.post(`/session/${sessionId}/model`, {
    model_name: modelName,
    parameters
  });
}

// Update model parameters
async function updateParameters({ sessionId, modelName, parameters = {} }) {
  return api.patch(`/session/${sessionId}/model`, {
    model_name: modelName,
    parameters
  });
}

// Export the service
export const model = {
  getModels,
  setModel,
  updateParameters,
  getDefaultParameters
};
```

### Step 3: Create useModel Hook

```js
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

### Step 4: Update LegacySessionContext to Use ModelContext

```jsx
// Partial update to LegacySessionContext.jsx
import { ModelContext } from './ModelContext';

export const LegacySessionProvider = ({ children }) => {
  // Original SessionContext dependency
  const session = useContext(SessionContext);
  
  // Add ModelContext dependency
  const model = useContext(ModelContext);
  
  // ... existing state management ...
  
  // Proxy model-related properties from ModelContext
  // This ensures the legacy context reflects the ModelContext state
  useEffect(() => {
    if (model.isInitialized) {
      setSelectedModel(model.selectedModel);
      setModelName(model.modelName);
      setModelParameters(model.modelParameters);
    }
  }, [
    model.isInitialized,
    model.selectedModel,
    model.modelName,
    model.modelParameters
  ]);
  
  // Proxy model operations to ModelContext
  const handleModelChange = useCallback((newModelName) => {
    // Forward to ModelContext
    return model.changeModel(newModelName);
  }, [model]);
  
  const handleModelParameterChange = useCallback((paramName, paramValue) => {
    // Forward to ModelContext
    return model.updateModelParameters({ [paramName]: paramValue });
  }, [model]);
  
  // ... existing code ...
  
  // Include model-related properties in the legacy context
  const legacyContextValue = {
    // ... existing properties ...
    
    // Model properties proxied from ModelContext
    modelName: model.modelName,
    modelParameters: model.modelParameters,
    models: model.modelConfigs,
    selectedModel: model.selectedModel,
    
    // Model actions that delegate to ModelContext
    changeModel: handleModelChange,
    updateModelParameter: handleModelParameterChange,
    
    // ... other existing properties ...
  };
  
  return (
    <LegacySessionContext.Provider value={legacyContextValue}>
      {children}
    </LegacySessionContext.Provider>
  );
};
```

### Step 5: Update App Context Hierarchy

```jsx
// src/App.jsx
import { SessionProvider } from './contexts/SessionContext';
import { ModelProvider } from './contexts/ModelContext';
import { LegacySessionProvider } from './contexts/LegacySessionContext';
import { ThemeProvider } from './contexts/ThemeProvider';

function App() {
  return (
    <SessionProvider>
      <ModelProvider>
        <LegacySessionProvider>
          <ThemeProvider>
            {/* Router and other app content */}
          </ThemeProvider>
        </LegacySessionProvider>
      </ModelProvider>
    </SessionProvider>
  );
}
```

## Component Update Strategy

Instead of trying to migrate all components at once, we'll follow a phased approach:

1. **Identify Key Components**: First, identify components that directly interact with model state and operations:
   - ModelParameterControls
   - ModelSelector components
   - Any components that display or modify model parameters

2. **Targeted Migrations**: Update these key components one by one to use the useModel hook instead of the legacy context. For example:

```jsx
// Before
const { modelParameters, updateModelParameter } = useContext(LegacySessionContext);

// After
const { modelParameters, updateModelParameters } = useModel();
// Note: API signature change from updateModelParameter to updateModelParameters
```

3. **Compatibility Layer**: Where needed, create thin adapter functions to handle API differences between the old and new contexts.

## Debug Support

To aid in troubleshooting during this complex transition, we'll add enhanced logging:

1. **Initialization Logging**: Detailed logging of the initialization sequence and state changes
2. **Context State Diffing**: Log when model state differs between contexts
3. **API Call Logging**: Clear logging of all model-related API calls

## Testing Strategy

1. **Initialization Tests**: Verify ModelContext initializes correctly with SessionContext
2. **State Synchronization Tests**: Ensure model state remains consistent between contexts
3. **Component Integration Tests**: Test components with both contexts
4. **API Call Tests**: Verify correct API endpoints are called with proper parameters

## Risk Mitigation

1. **Fallback Mechanism**: Components should fall back to LegacySessionContext if ModelContext is unavailable
2. **Graceful Error Handling**: All errors should be caught and displayed appropriately
3. **State Recovery**: Implement recovery mechanisms for initialization failures

## Implementation Checklist

- [ ] Core ModelContext implementation
- [ ] ModelService utility functions
- [ ] useModel hook implementation
- [ ] LegacySessionContext integration with ModelContext
- [ ] App context hierarchy update
- [ ] Update at least one key component to use ModelContext directly
- [ ] Enhanced logging and debugging support
- [ ] Basic testing of the integration

## Next Steps

After completing Phase 3, we will proceed to Phase 4: Creating UIStateContext, which will follow a similar pattern but focus on UI state.