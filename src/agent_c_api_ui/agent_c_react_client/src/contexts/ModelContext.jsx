import React, { createContext, useState, useEffect, useRef } from 'react';
import logger from '@/lib/logger';
import apiService from '@/lib/apiService';
import storageService from '@/lib/storageService';
import { useAuth } from '@/hooks/use-auth';
import { trackContextInitialization } from '@/lib/diagnostic';

// Create the context
export const ModelContext = createContext();

// Constants
const AGENT_CONFIG_KEY = 'agent_config';
const CONFIG_MAX_AGE_DAYS = 14;

/**
 * ModelProvider manages model selection, configuration, and parameter settings
 * This context is responsible for model-related state and API interactions
 */
export const ModelProvider = ({ children }) => {
    logger.info('ModelProvider initializing', 'ModelProvider');
    
    // Track the start of ModelContext initialization
    trackContextInitialization('ModelContext', 'start');
    
    // Get auth context for session information
    const { sessionId, isAuthenticated } = useAuth('ModelProvider');
    
    // Model configuration state
    const [modelConfigs, setModelConfigs] = useState([]);
    const [modelName, setModelName] = useState("");
    const [selectedModel, setSelectedModel] = useState(null);
    const [modelParameters, setModelParameters] = useState({});
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState(null);
    
    // Ref for debounced parameter updates
    const debouncedUpdateRef = useRef(null);
    
    /**
     * Fetch available models from API
     */
    const fetchModels = async () => {
        try {
            setIsLoading(true);
            logger.debug('Fetching available models', 'ModelContext');
            
            const data = await apiService.getModels();
            if (data && Array.isArray(data.models)) {
                // Log models received for diagnostic purposes
                logger.debug('Raw models data received', 'ModelContext', { 
                    count: data.models.length,
                    firstModel: data.models[0] ? { id: data.models[0].id, name: data.models[0].name } : null
                });
                
                // Check if models array is valid 
                if (data.models.length === 0) {
                    logger.warn('Empty models array received from API', 'ModelContext');
                } else if (!data.models[0].id) {
                    logger.warn('Invalid model data structure - missing model ID', 'ModelContext', {
                        sampleModel: data.models[0]
                    });
                }
                
                // Update model configs in state
                setModelConfigs(data.models);
                logger.info('Models fetched successfully', 'ModelContext', { 
                    count: data.models.length
                });
                
                trackContextInitialization('ModelContext', 'update', {
                    operation: 'fetchModels',
                    success: true,
                    modelCount: data.models.length
                });
                
                return data.models;
            } else {
                // Log what we actually received for diagnostics
                logger.warn('Invalid models data structure received', 'ModelContext', {
                    received: data,
                    hasModelsProperty: data ? 'models' in data : false,
                    modelsType: data?.models ? typeof data.models : 'undefined'
                });
                throw new Error('Invalid models data structure');
            }
        } catch (error) {
            logger.error('Failed to fetch models', 'ModelContext', { error: error.message });
            setError(`Failed to load models: ${error.message}`);
            
            trackContextInitialization('ModelContext', 'error', {
                operation: 'fetchModels',
                error: error.message
            });
            
            return [];
        } finally {
            setIsLoading(false);
        }
    };

    /**
     * Save model config to localStorage
     */
    const saveConfigToStorage = (updatedConfig = {}) => {
        try {
            // Get existing config or create new object
            const existingConfig = storageService.getAgentConfig() || {};
            
            // Update with current model settings
            const configToSave = {
                ...existingConfig,
                modelName: updatedConfig.modelName || modelName,
                modelParameters: updatedConfig.modelParameters || modelParameters,
                lastUpdated: new Date().toISOString()
            };
            
            // Store updated config
            storageService.set(AGENT_CONFIG_KEY, configToSave);
            
            logger.debug('Saved model configuration to localStorage', 'ModelContext', {
                modelName: configToSave.modelName,
                parametersKeys: Object.keys(configToSave.modelParameters || {})
            });
        } catch (err) {
            logger.error('Failed to save model configuration', 'ModelContext', { error: err.message });
        }
    };

    /**
     * Load saved configuration from localStorage
     * @returns {Object|null} The saved configuration or null if invalid/expired
     */
    const loadSavedConfig = () => {
        try {
            const savedConfig = storageService.getAgentConfig();
            if (!savedConfig) {
                return null;
            }
            
            logger.debug('Found saved model configuration', 'ModelContext');
            
            // Check if the configuration is expired
            if (savedConfig.lastUpdated) {
                const lastUpdated = new Date(savedConfig.lastUpdated);
                const now = new Date();
                const ageMs = now - lastUpdated;
                const maxAgeMs = CONFIG_MAX_AGE_DAYS * 24 * 60 * 60 * 1000;
                
                if (ageMs > maxAgeMs) {
                    logger.info(`Saved configuration is too old (>${CONFIG_MAX_AGE_DAYS} days)`, 'ModelContext');
                    storageService.remove(AGENT_CONFIG_KEY);
                    return null;
                }
            }
            
            return savedConfig;
        } catch (err) {
            logger.error('Error loading saved configuration', 'ModelContext', { error: err.message });
            return null;
        }
    };

    /**
     * Change the current model
     * @param {string} newModelName - ID of the model to change to
     * @returns {Promise<boolean>} Success status
     */
    const changeModel = async (newModelName) => {
        try {
            if (!modelConfigs || modelConfigs.length === 0) {
                logger.error('Cannot change model - no model configurations available', 'ModelContext');
                return false;
            }
            
            // Find the model in configurations
            const model = modelConfigs.find(m => m.id === newModelName);
            if (!model) {
                logger.error(`Model with name ${newModelName} not found in configurations`, 'ModelContext');
                return false;
            }
            
            // Set the model name and selected model
            setModelName(newModelName);
            setSelectedModel(model);
            
            // Update parameters based on selected model's defaults
            const updatedParameters = { ...modelParameters };
            
            // Handle temperature
            if (model.parameters?.temperature) {
                updatedParameters.temperature = model.parameters.temperature.default;
            }
            
            // Handle reasoning_effort for OpenAI models
            if (model.parameters?.reasoning_effort) {
                updatedParameters.reasoning_effort = model.parameters.reasoning_effort.default;
            }
            
            // Handle extended_thinking for Claude models
            if (model.parameters?.extended_thinking) {
                // Get the default enabled state
                const defaultEnabled = (typeof model.parameters.extended_thinking === 'object')
                    ? model.parameters.extended_thinking.enabled
                    : model.parameters.extended_thinking;
                
                updatedParameters.extended_thinking = defaultEnabled;
                
                // Set budget_tokens if extended_thinking is enabled
                if (defaultEnabled) {
                    const defaultBudgetTokens = model.parameters.extended_thinking.budget_tokens?.default || 4000;
                    updatedParameters.budget_tokens = defaultBudgetTokens;
                }
            }
            
            // Set the updated parameters
            setModelParameters(updatedParameters);
            
            // If session is ready, send model change to API
            if (sessionId && isAuthenticated) {
                // Create model update object
                const modelUpdateData = {
                    ui_session_id: sessionId,
                    model_name: newModelName,
                    backend: model.backend,
                    ...updatedParameters
                };
                
                // Update model in API
                await apiService.post('/update_settings', modelUpdateData);
            }
            
            // Save changes to localStorage
            saveConfigToStorage({
                modelName: newModelName,
                modelParameters: updatedParameters
            });
            
            logger.info('Model changed successfully', 'ModelContext', { 
                modelName: newModelName, 
                parameters: updatedParameters
            });
            
            return true;
        } catch (error) {
            logger.error('Failed to change model', 'ModelContext', { error: error.message });
            setError(`Failed to change model: ${error.message}`);
            return false;
        }
    };
    
    /**
     * Update model parameters
     * @param {Object} parameterUpdates - Object containing parameter updates
     * @returns {Promise<boolean>} Success status
     */
    const updateModelParameters = async (parameterUpdates) => {
        try {
            if (!parameterUpdates || Object.keys(parameterUpdates).length === 0) {
                logger.warn('No parameter updates provided', 'ModelContext');
                return false;
            }
            
            // Apply parameter updates to current model parameters
            const updatedParameters = { ...modelParameters, ...parameterUpdates };
            setModelParameters(updatedParameters);
            
            // If session is ready, send updates to API with debouncing
            if (sessionId && isAuthenticated) {
                if (debouncedUpdateRef.current) {
                    clearTimeout(debouncedUpdateRef.current);
                }
                
                debouncedUpdateRef.current = setTimeout(async () => {
                    try {
                        // Create parameter update object
                        const parameterUpdateData = {
                            ui_session_id: sessionId,
                            model_name: modelName,
                            backend: selectedModel?.backend,
                            ...updatedParameters
                        };
                        
                        // Update parameters in API
                        await apiService.post('/update_settings', parameterUpdateData);
                        
                        logger.debug('Parameters updated in API', 'ModelContext');
                    } catch (error) {
                        logger.error('Failed to update parameters in API', 'ModelContext', { error: error.message });
                    }
                }, 300);
            }
            
            // Save changes to localStorage
            saveConfigToStorage({
                modelParameters: updatedParameters
            });
            
            logger.info('Model parameters updated locally', 'ModelContext', { 
                updates: parameterUpdates, 
                newParameters: updatedParameters
            });
            
            return true;
        } catch (error) {
            logger.error('Failed to update model parameters', 'ModelContext', { error: error.message });
            setError(`Failed to update parameters: ${error.message}`);
            return false;
        }
    };
    
    /**
     * Initialize with a model or restore from saved configuration
     * @param {Object} model - Optional model to initialize with
     * @returns {Promise<boolean>} Success status
     */
    const initializeWithModel = async (model = null) => {
        try {
            // Check and log state first for debugging
            logger.debug('initializeWithModel called', 'ModelContext', { 
                hasModelConfigs: !!modelConfigs, 
                modelConfigsLength: modelConfigs?.length || 0,
                specificModelProvided: !!model
            });
            
            // If specific model provided, use it
            if (model) {
                setModelName(model.id);
                setSelectedModel(model);
                
                // Extract parameters
                const initialParameters = {
                    temperature: model.parameters?.temperature?.default || 0.7,
                    reasoning_effort: model.parameters?.reasoning_effort?.default,
                    extended_thinking: model.parameters?.extended_thinking?.enabled || false,
                    budget_tokens: model.parameters?.extended_thinking?.budget_tokens?.default || 4000
                };
                
                setModelParameters(initialParameters);
                saveConfigToStorage({
                    modelName: model.id,
                    modelParameters: initialParameters
                });
                
                return true;
            }
            
            // Otherwise try to load saved config
            const savedConfig = loadSavedConfig();
            if (savedConfig && savedConfig.modelName) {
                // Verify we have model configurations before trying to find saved model
                if (modelConfigs && modelConfigs.length > 0) {
                    // Find the model in configurations
                    const savedModel = modelConfigs.find(m => m.id === savedConfig.modelName);
                    if (savedModel) {
                        setModelName(savedModel.id);
                        setSelectedModel(savedModel);
                        setModelParameters(savedConfig.modelParameters || {});
                        
                        logger.info('Restored saved model configuration', 'ModelContext', {
                            modelName: savedModel.id
                        });
                        
                        return true;
                    } else {
                        logger.warn(`Saved model '${savedConfig.modelName}' not found in available models`, 'ModelContext');
                    }
                } else {
                    logger.warn('Cannot restore saved model - no model configurations available', 'ModelContext');
                }
            }
            
            // Verify we actually have model configs available before trying to use them
            if (!modelConfigs || modelConfigs.length === 0) {
                logger.warn('No model configs available for initialization. Using emergency fallback.', 'ModelContext');
                
                // Create emergency fallback model
                const fallbackModel = {
                    id: 'emergency_fallback_model',
                    name: 'Emergency Fallback Model',
                    backend: 'unknown'
                };
                setModelName(fallbackModel.id);
                setSelectedModel(fallbackModel);
                setModelParameters({
                    temperature: 0.7
                });
                
                logger.info('Using emergency fallback model due to missing model configs', 'ModelContext', {
                    modelConfigsState: modelConfigs
                });
                return true; // Return success to allow UI to function
            }
            
            // Since we know we have model configs, use the first available model
            logger.debug('Using first available model for initialization', 'ModelContext', {
                modelCount: modelConfigs.length,
                firstModelId: modelConfigs[0]?.id,
                firstModelName: modelConfigs[0]?.name
            });
            
            // Use the first model as default if available
            return await changeModel(modelConfigs[0].id);
        } catch (error) {
            logger.error('Failed to initialize model', 'ModelContext', { error: error.message });
            setError(`Failed to initialize model: ${error.message}`);
            
            // Even in case of error, set a fallback model to prevent UI breakage
            const fallbackModel = {
                id: 'error_fallback_model',
                name: 'Error Recovery Model',
                backend: 'unknown'
            };
            setModelName(fallbackModel.id);
            setSelectedModel(fallbackModel);
            setModelParameters({
                temperature: 0.7
            });
            
            logger.warn('Using error fallback model', 'ModelContext');
            return true; // Return success to allow UI to continue functioning
        }
    };
    
    // Clean up on unmount
    useEffect(() => {
        return () => {
            if (debouncedUpdateRef.current) {
                clearTimeout(debouncedUpdateRef.current);
            }
        };
    }, []);
    
    // Load models on mount
    useEffect(() => {
        const initModels = async () => {
            try {
                const models = await fetchModels();
                
                // Log what we received from the API for diagnostics
                logger.debug('Models received from API', 'ModelContext', { 
                    count: models.length,
                    modelIds: models.map(m => m.id).join(', ')
                });
                
                // We now need to WAIT for the state update to complete before initializing
                if (models.length > 0) {
                    // Set the models in state and wait for update to complete
                    // by using a separate useEffect that depends on modelConfigs state
                } else {
                    // No models available, so log and use fallback directly
                    logger.warn('No models returned from API, using fallback', 'ModelContext');
                    
                    // Use fallback model
                    const fallbackModel = {
                        id: 'emergency_fallback_model',
                        name: 'Emergency Fallback Model',
                        backend: 'unknown'
                    };
                    setModelName(fallbackModel.id);
                    setSelectedModel(fallbackModel);
                    setModelParameters({
                        temperature: 0.7
                    });
                    
                    // Signal completion with fallback
                    trackContextInitialization('ModelContext', 'complete', {
                        modelsLoaded: 0,
                        initializeSuccess: true,
                        selectedModel: fallbackModel.id,
                        usedFallback: true
                    });
                }
            } catch (error) {
                logger.error('Failed to initialize models', 'ModelContext', { error: error.message });
                
                // Try fallback model as error recovery
                const fallbackModel = {
                    id: 'emergency_fallback_model',
                    name: 'Emergency Fallback Model',
                    backend: 'unknown'
                };
                setModelName(fallbackModel.id);
                setSelectedModel(fallbackModel);
                setModelParameters({
                    temperature: 0.7
                });
                
                // Signal completion even with error to avoid blocking UI
                trackContextInitialization('ModelContext', 'complete', {
                    error: error.message,
                    operation: 'initModels',
                    usedEmergencyFallback: true,
                    selectedModel: fallbackModel.id
                });
            }
        };
        
        initModels();
    }, []);
    
    // Add a new effect that depends on modelConfigs to handle initialization after state update
    useEffect(() => {
        const initializeModelsAfterStateUpdate = async () => {
            // Only proceed if we have models and initialization hasn't completed yet
            if (modelConfigs && modelConfigs.length > 0) {
                logger.debug('Models loaded in state, initializing model', 'ModelContext', { 
                    count: modelConfigs.length,
                    firstModel: modelConfigs[0]?.id 
                });
                
                const success = await initializeWithModel();
                
                // Signal that ModelContext has completed initialization
                trackContextInitialization('ModelContext', 'complete', {
                    modelsLoaded: modelConfigs.length,
                    initializeSuccess: success,
                    selectedModel: modelName
                });
            }
        };
        
        initializeModelsAfterStateUpdate();
    }, [modelConfigs]); // This effect runs whenever modelConfigs state changes
    
    return (
        <ModelContext.Provider
            value={{
                // State
                modelConfigs,
                modelName,
                selectedModel,
                modelParameters,
                isLoading,
                error,
                
                // Methods
                setModelConfigs,
                changeModel,
                updateModelParameters,
                fetchModels,
                initializeWithModel
            }}
        >
            <div data-model-provider="mounted" style={{ display: 'none' }}>
                ModelProvider diagnostic element
            </div>
            {children}
        </ModelContext.Provider>
    );
};