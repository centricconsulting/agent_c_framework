import React, { createContext, useState, useEffect, useRef, useCallback } from 'react';
import { model as modelService } from '../services';
import { useToast } from '../hooks/use-toast';

export const ModelContext = createContext(null);

export const ModelProvider = ({ children }) => {
  console.log('🌟 ModelContext: Provider Initializing');
  const { toast } = useToast();
  
  // Simple state management
  const [isLoading, setIsLoading] = useState(false);
  const [isInitialized, setIsInitialized] = useState(false);
  const [error, setError] = useState(null);
  
  // Model state
  const [modelConfigs, setModelConfigs] = useState([]);
  const [selectedModel, setSelectedModel] = useState(null);
  const [modelParameters, setModelParameters] = useState({});
  
  // Prevent duplicate initialization
  const initAttempted = useRef(false);
  
  // Simple error handler
  const clearError = () => error && setError(null);

  // Fetch all available models - completely independent of session
  const fetchModels = useCallback(async () => {
    console.log('ModelContext: Fetching models...');
    setIsLoading(true);
    
    try {
      const data = await modelService.getModels();
      console.log('ModelContext: Models fetched successfully', data);
      
      if (data && data.models && data.models.length > 0) {
        setModelConfigs(data.models);
        
        // Set the first model as selected by default
        const defaultModel = data.models[0];
        setSelectedModel(defaultModel);
        
        // Extract default parameters
        const defaultParams = {};
        if (defaultModel.parameters) {
          Object.entries(defaultModel.parameters).forEach(([key, config]) => {
            if (config.default !== undefined) {
              defaultParams[key] = config.default;
            }
          });
        }
        
        setModelParameters(defaultParams);
        return data.models;
      } else {
        console.warn('ModelContext: No models returned');
        return [];
      }
    } catch (err) {
      console.error('ModelContext: Error fetching models:', err);
      setError('Failed to fetch models');
      toast({
        title: 'Error',
        description: 'Failed to fetch available models',
        variant: 'destructive',
      });
      return [];
    } finally {
      setIsLoading(false);
    }
  }, [toast]);

  // Change the selected model locally - no session dependency
  const changeModel = useCallback((modelId, parameters = null) => {
    console.log('ModelContext: Changing model to', modelId);
    
    // Find the model in our configs
    const model = modelConfigs.find(m => m.id === modelId);
    if (!model) {
      console.warn(`ModelContext: Model ${modelId} not found in configs`);
      return;
    }
    
    // Update selected model
    setSelectedModel(model);
    
    // Set default parameters if none provided
    if (!parameters) {
      const defaultParams = {};
      if (model.parameters) {
        Object.entries(model.parameters).forEach(([key, config]) => {
          if (config.default !== undefined) {
            defaultParams[key] = config.default;
          }
        });
      }
      setModelParameters(defaultParams);
    } else {
      setModelParameters(parameters);
    }
  }, [modelConfigs]);

  // Update model parameters locally
  const updateModelParameters = useCallback((newParams) => {
    console.log('ModelContext: Updating parameters', newParams);
    setModelParameters(current => ({
      ...current,
      ...newParams
    }));
  }, []);

  // Initialize the context - completely self-contained
  const initialize = useCallback(async () => {
    // Prevent multiple initialization attempts
    if (initAttempted.current || isInitialized) {
      console.log('ModelContext: Already initialized or attempted');
      return;
    }
    
    console.log('ModelContext: Starting initialization');
    initAttempted.current = true;
    
    try {
      const models = await fetchModels();
      if (models && models.length > 0) {
        console.log('ModelContext: Initialization complete');
        setIsInitialized(true);
      }
    } catch (err) {
      console.error('ModelContext: Initialization failed', err);
      setError('Failed to initialize models');
    }
  }, [fetchModels, isInitialized]);

  // Initialize on mount - completely independent of session
  useEffect(() => {
    console.log('ModelContext: Initializing on mount');
    if (!isInitialized && !initAttempted.current) {
      initialize();
    }
  }, [isInitialized, initialize]);

  // The context value
  const value = {
    // State
    isLoading,
    isInitialized,
    isReady: isInitialized, // Ready when initialized - no session dependency
    error,
    modelConfigs,
    selectedModel,
    modelName: selectedModel?.id || '',
    modelParameters,
    
    // Actions
    fetchModels,
    changeModel,
    updateModelParameters,
    clearError,
    initialize
  };

  return (
    <ModelContext.Provider value={value}>
      {children}
    </ModelContext.Provider>
  );
};