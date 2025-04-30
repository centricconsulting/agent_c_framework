Based on my comprehensive inventory of the chat interface components and their interactions with model-related state, I've identified the key issues that have caused race conditions in previous attempts. Here's my plan for safely implementing Phase 3:

## Race Condition Analysis

1. **Initialization Sequencing**: The ModelContext must wait for SessionContext to be fully initialized before attempting its own initialization
   
2. **State Ownership Conflict**: Both contexts were trying to manage the same state
   
3. **API Call Redundancy**: Both contexts may try to make the same API calls

4. **Component Dependencies**: Components in the chat interface need to access model state consistently

## Safe Implementation Plan

Let's approach this methodically with a focus on preventing race conditions:

### Step 1: Create the ModelContext with Clear Initialization Logic

```jsx
// src/contexts/ModelContext.jsx
import React, { createContext, useState, useEffect, useRef, useContext, useCallback } from 'react';
import { SessionContext } from './SessionContext';
import { model as modelService } from '../services';
import { useToast } from '../hooks/use-toast';

export const ModelContext = createContext(null);

export const ModelProvider = ({ children }) => {
  // Get session information from SessionContext
  const { 
    sessionId, 
    isAuthenticated, 
    isReady: isSessionReady 
  } = useContext(SessionContext);
  
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
  
  // Critical refs for managing initialization and updates
  const paramUpdateTimeoutRef = useRef(null);
  const modelInitAttempted = useRef(false);
  const modelInitComplete = useRef(false);
  
  // Clear error state
  const clearError = useCallback(() => {
    if (error) setError(null);
  }, [error]);

  // Get saved model preference
  const getSavedModelPreference = useCallback(() => {
    try {
      const savedConfig = localStorage.getItem("agent_config");
      if (!savedConfig) return null;
      
      const parsedConfig = JSON.parse(savedConfig);
      
      // Check if the configuration is expired (14 days)
      if (parsedConfig.lastUpdated) {
        const configAge = new Date() - new Date(parsedConfig.lastUpdated);
        const maxAgeMs = 14 * 24 * 60 * 60 * 1000; // 14 days
        if (configAge > maxAgeMs) {
          console.log('ModelContext: Saved configuration is too old (>14 days)');
          return null;
        }
      }
      
      return {
        modelName: parsedConfig.modelName,
        modelParameters: parsedConfig.modelParameters
      };
    } catch (e) {
      console.warn('ModelContext: Failed to parse saved model preference', e);
      return null;
    }
  }, []);
  
  // Save model preference
  const saveModelPreference = useCallback((modelConfig) => {
    try {
      const existingConfig = localStorage.getItem("agent_config");
      let newConfig = {
        lastUpdated: new Date().toISOString()
      };
      
      if (existingConfig) {
        try {
          newConfig = {
            ...JSON.parse(existingConfig),
            ...newConfig
          };
        } catch (e) {
          console.warn('ModelContext: Failed to parse existing config', e);
        }
      }
      
      // Update only the model-related parts
      newConfig.modelName = modelConfig.modelName || newConfig.modelName;
      newConfig.modelParameters = modelConfig.modelParameters || newConfig.modelParameters;
      
      localStorage.setItem("agent_config", JSON.stringify(newConfig));
    } catch (e) {
      console.warn('ModelContext: Failed to save model preference', e);
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

  // Select a model by ID
  const selectModel = useCallback(async (modelId, customParameters = null) => {
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
      
      // Determine parameters to use - either custom provided or defaults
      let parameters;
      if (customParameters) {
        parameters = customParameters;
      } else {
        // Get default parameters for the model
        parameters = {};
        
        // Extract temperature if available
        if (targetModel.parameters?.temperature) {
          parameters.temperature = targetModel.parameters.temperature.default;
        }
        
        // Extract reasoning effort if available
        if (targetModel.parameters?.reasoning_effort) {
          parameters.reasoning_effort = targetModel.parameters.reasoning_effort.default;
        }
        
        // Extract extended thinking if available
        if (targetModel.parameters?.extended_thinking) {
          const extThinking = targetModel.parameters.extended_thinking;
          parameters.extended_thinking = extThinking.enabled === true;
          
          if (parameters.extended_thinking && extThinking.budget_tokens) {
            parameters.budget_tokens = extThinking.budget_tokens.default;
          } else {
            parameters.budget_tokens = 0;
          }
        }
      }
      
      setModelParameters(parameters);
      
      // Update session with the model selection
      // This will be called by LegacySessionContext to avoid duplication
      // when initializing - but for direct model change we need to call it
      if (modelInitComplete.current) {
        await modelService.setModel({ 
          sessionId, 
          modelName: modelId,
          parameters
        });
      }
      
      // Save preference
      saveModelPreference({ modelName: modelId, modelParameters: parameters });
      
      console.log(`ModelContext: Model ${modelId} successfully selected with parameters:`, parameters);
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

  // Initialize model with saved preference or default
  const initializeModel = useCallback(async () => {
    // CRITICAL: Only proceed if:
    // 1. We have a valid session
    // 2. Session is ready
    // 3. This is our first attempt (prevents multiple initialization)
    if (!sessionId || !isAuthenticated || !isSessionReady || modelInitAttempted.current) {
      console.log('ModelContext: Not initializing model - conditions not met', {
        sessionId: !!sessionId,
        isAuthenticated,
        isSessionReady,
        alreadyAttempted: modelInitAttempted.current
      });
      return;
    }
    
    console.log('ModelContext: Starting initialization sequence...');
    modelInitAttempted.current = true;
    setIsLoading(true);
    clearError();
    
    try {
      console.log('ModelContext: Fetching available models');
      const models = await fetchModels();
      
      if (!models || models.length === 0) {
        throw new Error('No models available');
      }
      
      // Get saved preference
      const savedPreference = getSavedModelPreference();
      console.log('ModelContext: Saved preference:', savedPreference);
      
      if (savedPreference && savedPreference.modelName) {
        // Check if saved model exists in available models
        const modelExists = models.some(m => m.id === savedPreference.modelName);
        
        if (modelExists) {
          console.log(`ModelContext: Found saved model ${savedPreference.modelName}`);
          await selectModel(savedPreference.modelName, savedPreference.modelParameters);
        } else {
          console.log(`ModelContext: Saved model ${savedPreference.modelName} not found, using first available`);
          await selectModel(models[0].id);
        }
      } else {
        console.log('ModelContext: No saved preference, using first available model');
        await selectModel(models[0].id);
      }
      
      modelInitComplete.current = true;
      setIsInitialized(true);
      console.log('ModelContext: Initialization complete');
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
  }, [
    sessionId, 
    isAuthenticated, 
    isSessionReady, 
    fetchModels, 
    getSavedModelPreference,
    selectModel, 
    clearError, 
    toast
  ]);

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
    setModelParameters(currentParams => {
      const updatedParams = {
        ...currentParams,
        ...newParams
      };
      return updatedParams;
    });
    
    // Debounce the API call
    paramUpdateTimeoutRef.current = setTimeout(async () => {
      clearError();
      try {
        // Get the latest parameters from state to ensure we have the most recent
        const latestParams = { ...modelParameters, ...newParams };
        console.log('ModelContext: Updating model parameters', latestParams);
        
        await modelService.updateParameters({
          sessionId,
          modelName,
          parameters: latestParams
        });
        
        // Save to local storage
        saveModelPreference({ modelParameters: latestParams });
        
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
    }, 500); // 500ms debounce
  }, [
    sessionId,
    isAuthenticated,
    modelName,
    modelParameters,
    clearError,
    saveModelPreference,
    toast
  ]);

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

  // Debug logging
  useEffect(() => {
    console.log('ModelContext state change:', {
      isInitialized,
      isLoading,
      modelName,
      hasParameters: !!Object.keys(modelParameters).length,
      hasSelectedModel: !!selectedModel,
      sessionReady: isSessionReady,
      initiated: modelInitAttempted.current,
      completed: modelInitComplete.current
    });
  }, [isInitialized, isLoading, modelName, modelParameters, selectedModel, isSessionReady]);

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
    changeModel: selectModel,
    updateModelParameters,
    clearError,
  };

  return (
    <ModelContext.Provider value={contextValue}>
      {children}
    </ModelContext.Provider>
  );
};
```

### Step 2: Update App.jsx to Properly Nest the Context Providers

```jsx
// src/App.jsx
import React from 'react';
import { BrowserRouter as Router } from 'react-router-dom';
import AppRoutes from '@/Routes';
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

export default App;
```

### Step 3: Update LegacySessionContext to Proxy Model Operations

```jsx
// Partial update to LegacySessionContext.jsx
import { ModelContext } from './ModelContext';

export const LegacySessionProvider = ({ children }) => {
  // Original SessionContext dependency
  const session = useContext(SessionContext);
  
  // Add ModelContext dependency
  const model = useContext(ModelContext);
  
  // ... existing state management ...
  
  // Critical change: For model initialization, use ModelContext
  const initializeSession = async (forceNew = false, initialModel = null, modelConfigsData = null) => {
    try {
      // First validate models are available
      const models = modelConfigsData || model.modelConfigs;
      if (!models || models.length === 0) {
        throw new Error("No model configurations available");
      }

      // Determine which model to use based on initialModel or current model state
      let currentModelId = null;
      let parameters = null;

      if (initialModel && initialModel.id) {
        currentModelId = initialModel.id;
        
        // Extract parameters from initialModel
        parameters = {
          temperature: initialModel.temperature,
          reasoning_effort: initialModel.reasoning_effort,
          extended_thinking: initialModel.extended_thinking,
          budget_tokens: initialModel.budget_tokens
        };
      } else if (model.modelName) {
        currentModelId = model.modelName;
        parameters = model.modelParameters;
      } else if (models.length > 0) {
        currentModelId = models[0].id;
      }

      if (!currentModelId) {
        throw new Error('No valid model configuration available');
      }

      // Build session configuration
      const sessionConfig = {
        model_name: currentModelId,
        backend: models.find(m => m.id === currentModelId)?.backend,
        persona_name: initialModel?.persona_name || persona || 'default'
      };

      // If we have an existing session and we're not forcing a new one, include the session ID
      if (sessionId && !forceNew) {
        sessionConfig.ui_session_id = sessionId;
      }

      // Determine which custom prompt to use
      let promptToUse = null;
      if (initialModel && (initialModel.custom_prompt || initialModel.customPrompt)) {
        promptToUse = initialModel.custom_prompt || initialModel.customPrompt;
      } else if (customPrompt) {
        promptToUse = customPrompt;
      }
      if (promptToUse) {
        sessionConfig.custom_prompt = promptToUse;
      }

      // Add model parameters
      if (parameters) {
        if (parameters.temperature !== undefined) {
          sessionConfig.temperature = parameters.temperature;
        }
        if (parameters.reasoning_effort !== undefined) {
          sessionConfig.reasoning_effort = parameters.reasoning_effort;
        }
        if (parameters.extended_thinking !== undefined) {
          sessionConfig.extended_thinking = parameters.extended_thinking;
        }
        if (parameters.budget_tokens !== undefined) {
          sessionConfig.budget_tokens = parameters.budget_tokens;
        }
      }

      console.log('initializeSession data being sent:', sessionConfig);

      // Initialize the core session
      await coreInitializeSession(sessionConfig);
      
      // No need to directly update model here - ModelContext will handle this via its own initialization
      // or update mechanisms. This prevents race conditions and redundant API calls.
    } catch (err) {
      console.error("Session initialization failed:", err);
      setError(`Session initialization failed: ${err.message}`);
      showErrorToast(err, 'Session initialization failed');
      throw err;
    }
  };
  
  // Proxy model operations to ModelContext
  const handleModelChange = useCallback((newModelName) => {
    return model.changeModel(newModelName);
  }, [model]);
  
  const handleModelParameterChange = useCallback((paramName, paramValue) => {
    return model.updateModelParameters({ [paramName]: paramValue });
  }, [model]);
  
  // For model-related updates in updateAgentSettings
  const updateAgentSettings = async (updateType, values) => {
    if (!sessionId || !isReady) return;
    try {
      switch (updateType) {
        case 'MODEL_CHANGE': 
          // Forward to ModelContext instead of handling here
          await model.changeModel(values.modelName);
          setSettingsVersion(v => v + 1);
          break;
        
        case 'SETTINGS_UPDATE': {
          // This is still handled here since it's not model-specific
          const updatedPersona = values.persona_name || persona;
          const updatedPrompt = values.customPrompt || customPrompt;
          setPersona(updatedPersona);
          setCustomPrompt(updatedPrompt);

          const updateData = {
            ui_session_id: sessionId,
            model_name: model.modelName, // Use from ModelContext
            backend: model.selectedModel?.backend,
            persona_name: updatedPersona,
            custom_prompt: updatedPrompt
          };

          console.log('Settings update data being sent:', updateData);
          await sessionService.updateSession(sessionId, updateData);
          
          setSettingsVersion(v => v + 1);
          saveConfigToStorage({
            persona: updatedPersona,
            customPrompt: updatedPrompt
          });
          break;
        }
          
        case 'PARAMETER_UPDATE':
          // Forward to ModelContext
          model.updateModelParameters(values);
          setSettingsVersion(v => v + 1);
          break;
          
        default:
          break;
      }
    } catch (err) {
      setError(`Failed to update settings: ${err.message}`);
      showErrorToast(err, 'Failed to update settings');
    }
  };
  
  // Sync with ModelContext state
  useEffect(() => {
    if (model.isInitialized) {
      setModelName(model.modelName);
      setSelectedModel(model.selectedModel);
      setModelParameters(model.modelParameters);
      setIsInitialized(model.isInitialized);
    }
  }, [
    model.isInitialized,
    model.modelName,
    model.selectedModel,
    model.modelParameters
  ]);
  
  // Other LegacySessionContext code...
  
  return (
    <LegacySessionContext.Provider
      value={{
        // Forward the core session state
        sessionId,
        isReady,
        error,
        
        // Include model state from ModelContext
        modelName: model.modelName,
        modelParameters: model.modelParameters,
        modelConfigs: model.modelConfigs,
        selectedModel: model.selectedModel,
        
        // Rest of the context value
        settingsVersion,
        isOptionsOpen,
        setIsOptionsOpen,
        isStreaming,
        isLoading,
        isInitialized: model.isInitialized,
        persona,
        temperature,
        customPrompt,
        personas,
        availableTools,
        activeTools,
        theme,
        handleThemeChange,
        fetchAgentTools,
        updateAgentSettings,
        handleEquipTools,
        handleProcessingStatus,
        handleSessionsDeleted,
        initializeSession,
        setIsInitialized,
        
        // Add model-specific operations
        changeModel: handleModelChange,
        updateModelParameter: handleModelParameterChange
      }}
    >
      {children}
    </LegacySessionContext.Provider>
  );
};
```

## Implementation Plan

1. First, I'll create the ModelContext.jsx file with robust initialization logic that includes proper synchronization with SessionContext

2. Then I'll update App.jsx to include the ModelProvider in the proper nesting order

3. Next, I'll modify the LegacySessionContext to proxy model operations to the ModelContext

4. Finally, I'll test the implementation by ensuring:
   - The initialization sequence works correctly
   - Model changes work properly
   - Parameter updates are correctly reflected
   - All components display the same model state
   - No race conditions occur between contexts

This approach addresses the critical issues by:

1. Ensuring clear ownership of model state (ModelContext is the source of truth)
2. Establishing a proper initialization sequence (ModelContext waits for SessionContext)
3. Preventing redundant API calls (operations are proxied, not duplicated)
4. Maintaining backward compatibility (LegacySessionContext continues to work)

