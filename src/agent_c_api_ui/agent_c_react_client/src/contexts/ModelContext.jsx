import React, { createContext, useState, useEffect, useRef } from 'react';
import logger from '@/lib/logger';
import apiService from '@/lib/apiService';
import storageService from '@/lib/storageService';
import { useAuth } from '@/hooks/use-auth';

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
            
            const data = await apiService.get('/models');
            if (data && Array.isArray(data.models)) {
                setModelConfigs(data.models);
                logger.info('Models fetched successfully', 'ModelContext', { 
                    count: data.models.length
                });
                return data.models;
            } else {
                throw new Error('Invalid models data structure');
            }
        } catch (error) {
            logger.error('Failed to fetch models', 'ModelContext', { error: error.message });
            setError(`Failed to load models: ${error.message}`);
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
            const existingConfig = storageService.get(AGENT_CONFIG_KEY) || {};
            
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
            const savedConfig = storageService.get(AGENT_CONFIG_KEY);
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
                }
            }
            
            // If no saved config or model not found, use first available model
            if (modelConfigs.length > 0) {
                const defaultModel = modelConfigs[0];
                return await changeModel(defaultModel.id);
            }
            
            logger.warn('No models available for initialization', 'ModelContext');
            return false;
        } catch (error) {
            logger.error('Failed to initialize model', 'ModelContext', { error: error.message });
            setError(`Failed to initialize model: ${error.message}`);
            return false;
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
            const models = await fetchModels();
            if (models.length > 0) {
                await initializeWithModel();
            }
        };
        
        initModels();
    }, []);
    
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
            {children}
        </ModelContext.Provider>
    );
};