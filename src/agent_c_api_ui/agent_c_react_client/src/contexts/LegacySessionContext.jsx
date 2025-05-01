import React, { createContext, useState, useEffect, useRef, useContext, useCallback } from 'react';
import { SessionContext } from './SessionContext';
import { useModel } from '../hooks/use-model';

// Import services from the API service layer
import {
    api,
    session as sessionService,
    model as modelService,
    tools as toolsService,
    persona as personaService,
    showErrorToast
} from '../services';

/**
 * LegacySessionContext maintains backward compatibility during the refactoring process.
 * It wraps the core SessionContext and provides all the original functionality.
 * 
 * This context will be gradually replaced by more focused contexts in future phases.
 */
export const LegacySessionContext = createContext();

export const LegacySessionProvider = ({ children }) => {
    console.log('🔥🔥🔥 LegacySessionContext MOUNT - PROVIDER INITIALIZING 🔥🔥🔥');
    // Use the core SessionContext
    const {
        sessionId, 
        isReady, 
        error: sessionError, 
        setError: setSessionError,
        initializeSession: coreInitializeSession,
        handleSessionsDeleted: coreHandleSessionsDeleted,
        isInitialized: coreIsInitialized,
        setIsInitialized: coreSetIsInitialized
    } = useContext(SessionContext);
    
    console.log('🔴 LegacySessionContext attempting to use model hook');
    
    // Try to use ModelContext if available, otherwise use empty defaults
    let modelData = {};
    let isModelReady = false;
    
    try {
        // Use the ModelContext via the hook for better safety
        modelData = useModel();
        isModelReady = modelData.isReady;
        console.log('🔴 LegacySessionContext: useModel hook returned isReady =', isModelReady);
    } catch (err) {
        console.error('🔴 LegacySessionContext: ModelContext not available yet:', err.message);
        // Provide empty defaults
        modelData = {
            modelName: '',
            modelParameters: {},
            modelConfigs: [],
            selectedModel: null,
            changeModel: () => Promise.reject(new Error('ModelContext not initialized')),
            updateModelParameters: () => Promise.reject(new Error('ModelContext not initialized'))
        };
    }
    
    // Destructure model data with defaults for safety
    const { 
      modelName = '',
      modelParameters = {},
      modelConfigs = [],
      selectedModel = null,
      changeModel = () => Promise.reject(new Error('ModelContext unavailable')),
      updateModelParameters = () => Promise.reject(new Error('ModelContext unavailable'))
    } = modelData;
    
    // Global session & UI state that will be moved to other contexts later
    const debouncedUpdateRef = useRef(null);
    const [error, setError] = useState(null);
    const [settingsVersion, setSettingsVersion] = useState(0);
    const [isOptionsOpen, setIsOptionsOpen] = useState(false);
    const [isStreaming, setIsStreaming] = useState(false);
    const [isLoading, setIsLoading] = useState(true);
    const [isInitialized, setIsInitialized] = useState(false);
    const [theme, setTheme] = useState(() => {
        // Check for saved theme preference in localStorage
        const savedTheme = localStorage.getItem('theme');
        return savedTheme || 'system';
    });

    // Agent settings & configuration
    const [persona, setPersona] = useState("");
    const [temperature, setTemperature] = useState(null);
    const [customPrompt, setCustomPrompt] = useState("");

    // Data from backend
    const [personas, setPersonas] = useState([]);
    const [availableTools, setAvailableTools] = useState({
        essential_tools: [],
        groups: {},
        categories: []
    });
    const [activeTools, setActiveTools] = useState([]);

    // Synchronize error states
    useEffect(() => {
        if (sessionError) {
            setError(sessionError);
        }
    }, [sessionError]);

    // Synchronize initialized state with core context
    useEffect(() => {
        coreSetIsInitialized(isInitialized);
    }, [isInitialized, coreSetIsInitialized]);
    
    // Fetch initial data (personas, tools)
    const fetchInitialData = async () => {
        try {
            console.log('Starting fetchInitialData for non-model data');
            setIsLoading(true);
            
            // Only fetch personas and tools - model data is handled by ModelContext
            const [personasData, toolsData] = await Promise.all([
                personaService.getPersonas(),
                toolsService.getTools()
            ]);
            
            // Store data returned from backend into their data structures
            setPersonas(personasData);
            setAvailableTools(toolsData);
            
            // Try pulling saved persona/prompt config from localstorage
            const savedConfig = localStorage.getItem("agent_config");
            if (savedConfig) {
                try {
                    const parsedConfig = JSON.parse(savedConfig);
                    
                    // Check if the configuration is expired (14 days)
                    if (parsedConfig.lastUpdated) {
                        const configAge = new Date() - new Date(parsedConfig.lastUpdated);
                        const maxAgeMs = 14 * 24 * 60 * 60 * 1000; // 14 days
                        
                        if (configAge <= maxAgeMs) {
                            // Apply saved persona settings
                            if (parsedConfig.persona) {
                                setPersona(parsedConfig.persona);
                            }
                            
                            if (parsedConfig.customPrompt) {
                                setCustomPrompt(parsedConfig.customPrompt);
                            }
                        } else {
                            console.log('Saved configuration is too old (>14 days), using defaults');
                        }
                    }
                } catch (err) {
                    console.error('Error parsing saved configuration:', err);
                }
            }
            
            // Choose a default persona if none was loaded (or fall back to the first)
            if (!persona && personasData.length > 0) {
                const defaultPersona = personasData.find(p => p.name === 'default');
                const initialPersona = defaultPersona || personasData[0];
                setPersona(initialPersona.name);
                setCustomPrompt(initialPersona.content);
            }
            
            // The session itself will be initialized by SessionContext and ModelContext
            // This avoids race conditions and ensures proper order of operations
        } catch (err) {
            console.error('Error fetching initial data:', err);
            setError(`Failed to load initial data: ${err.message}`);
            showErrorToast(err, 'Failed to load initial data');
        } finally {
            setIsLoading(false);
        }
    };

    // Initialize (or reinitialize) a session - Modified to use core context and ModelContext
    const initializeSession = async (forceNew = false, initialModel = null, modelConfigsData = null) => {
        try {
            console.log('LegacySessionContext: Starting session initialization with forceNew=', forceNew);
            
            // First, ensure we have model configurations available - use useModel hook data
            let models = modelConfigsData || modelConfigs;
            
            // If we still don't have models, fetch them directly
            if (!models || models.length === 0) {
                console.log('LegacySessionContext: No model configurations, fetching models');
                try {
                    const modelData = await modelService.getModels();
                    models = modelData.models || [];
                    if (models.length > 0) {
                        console.log('LegacySessionContext: Successfully fetched models:', models.map(m => m.id).join(', '));
                        // Models will be managed by ModelContext
                    } else {
                        console.error('LegacySessionContext: API returned no models');
                        throw new Error("No model configurations available from API");
                    }
                } catch (modelErr) {
                    console.error('LegacySessionContext: Failed to fetch models:', modelErr);
                    throw new Error("Could not retrieve model configurations");
                }
            } else {
                console.log('LegacySessionContext: Using existing models:', models.map(m => m.id).join(', '));
            }

            // Determine which model to use
            let currentModelId = null;
            let parameters = null;

            if (initialModel && initialModel.id) {
                console.log(`LegacySessionContext: Using initialModel with id ${initialModel.id}`);
                currentModelId = initialModel.id;
                
                // Extract parameters from initialModel
                parameters = {
                    temperature: initialModel.temperature,
                    reasoning_effort: initialModel.reasoning_effort,
                    extended_thinking: initialModel.extended_thinking,
                    budget_tokens: initialModel.budget_tokens
                };
                console.log('LegacySessionContext: Using parameters from initialModel:', parameters);
            } else if (isModelReady && modelName) {
                console.log(`LegacySessionContext: Using model from ModelContext: ${modelName}`);
                currentModelId = modelName;
                parameters = modelParameters;
            } else if (modelName) {
                console.log(`LegacySessionContext: Using fallback model from local state: ${modelName}`);
                // Fallback to local state if ModelContext isn't initialized yet
                currentModelId = modelName;
                parameters = modelParameters;
            } else if (models.length > 0) {
                // Last resort: use the first available model
                currentModelId = models[0].id;
                console.log(`LegacySessionContext: Using first available model: ${currentModelId}`);
                
                // Get default parameters for the model
                const defaultModel = models.find(m => m.id === currentModelId);
                if (defaultModel && defaultModel.parameters) {
                    parameters = {};
                    
                    // Extract temperature if available
                    if (defaultModel.parameters.temperature) {
                        parameters.temperature = defaultModel.parameters.temperature.default;
                    }
                    
                    // Extract reasoning effort if available
                    if (defaultModel.parameters.reasoning_effort) {
                        parameters.reasoning_effort = defaultModel.parameters.reasoning_effort.default;
                    }
                    
                    // Extract extended thinking if available
                    if (defaultModel.parameters.extended_thinking) {
                        const extThinking = defaultModel.parameters.extended_thinking;
                        parameters.extended_thinking = extThinking.enabled === true;
                        
                        if (parameters.extended_thinking && extThinking.budget_tokens) {
                            parameters.budget_tokens = extThinking.budget_tokens.default;
                        } else {
                            parameters.budget_tokens = 0;
                        }
                    }
                    
                    console.log('LegacySessionContext: Using default parameters for model:', parameters);
                }
            }

            if (!currentModelId) {
                console.error('LegacySessionContext: No valid model available');
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

            console.log('LegacySessionContext: initializeSession data being sent:', sessionConfig);

            // Initialize the core session
            await coreInitializeSession(sessionConfig);
            console.log('LegacySessionContext: Core session initialized');
            
            // Model state is now managed by ModelContext
            // The ModelContext will be updated through its own initialization flow
            
            // Explicitly set our initialization flag to true so the UI can proceed
            // even if ModelContext is still initializing
            setIsInitialized(true);
            console.log('LegacySessionContext: Session initialization complete');
            
        } catch (err) {
            console.error("Session initialization failed:", err);
            setError(`Session initialization failed: ${err.message}`);
            showErrorToast(err, 'Session initialization failed');
            throw err; // Rethrow to allow caller to handle
        }
    };

    // Helper function to add model parameters to query params
    const addModelParameters = (jsonData, model) => {
        // Add temperature if supported
        if (model.parameters?.temperature) {
            const currentTemp = temperature ?? model.parameters.temperature.default;
            jsonData.temperature = currentTemp;
        }

        // Handle Claude extended thinking parameters
        const hasExtendedThinking = !!model.parameters?.extended_thinking;
        if (hasExtendedThinking) {
            // Keep track of UI state but don't send to backend
            const extendedThinking = modelParameters.extended_thinking !== undefined
                ? modelParameters.extended_thinking
                : Boolean(model.parameters.extended_thinking.enabled) === true;

            // Budget tokens is what actually matters
            const defaultBudgetTokens = parseInt(
                model.parameters.extended_thinking.budget_tokens?.default || 5000
            );

            const budgetTokens = extendedThinking
                ? (modelParameters.budget_tokens !== undefined
                    ? modelParameters.budget_tokens
                    : defaultBudgetTokens)
                : 0;

            jsonData.budget_tokens = budgetTokens;
            console.log(`Setting budget_tokens=${budgetTokens}`);
        }

        // Handle OpenAI reasoning effort parameter if supported
        if (model.parameters?.reasoning_effort) {
            const reasoningEffortDefault = model.parameters.reasoning_effort.default;
            const reasoningEffort = modelParameters.reasoning_effort !== undefined
                ? modelParameters.reasoning_effort
                : reasoningEffortDefault;

            jsonData.reasoning_effort = reasoningEffort;
        }
    };

    // Fetch agent tools for the current session
    const fetchAgentTools = async () => {
        if (!sessionId || !isReady) return;
        try {
            const data = await toolsService.getSessionTools(sessionId);
            if (data.status === 'success' && Array.isArray(data.initialized_tools)) {
                setActiveTools(data.initialized_tools.map(tool => tool.class_name));
            }
        } catch (err) {
            console.error('Error fetching agent tools:', err);
            // We don't set global error or show toast here as this is not critical
        }
    };

    // Update agent settings (model change, settings update, parameter update)
    const updateAgentSettings = async (updateType, values) => {
        if (!sessionId || !isReady) return;
        try {
            switch (updateType) {
                case 'MODEL_CHANGE': {
                    // Only proceed if ModelContext is ready
                    if (!isModelReady) {
                        console.warn('LegacySessionContext: Cannot change model - ModelContext not ready');
                        throw new Error('Model context not ready');
                    }
                    
                    // Forward to ModelContext through our hook-based proxy
                    await handleModelChange(values.modelName);
                    setSettingsVersion(v => v + 1);
                    break;
                }
                case 'SETTINGS_UPDATE': {
                    const updatedPersona = values.persona_name || persona;
                    const updatedPrompt = values.customPrompt || customPrompt;
                    setPersona(updatedPersona);
                    setCustomPrompt(updatedPrompt);

                    const updateData = {
                        ui_session_id: sessionId,
                        model_name: modelName, // Use from useModel hook
                        backend: selectedModel?.backend, // Use from useModel hook
                        persona_name: updatedPersona,
                        custom_prompt: updatedPrompt
                    };

                    console.log('Settings update data being sent:', updateData);
                    await sessionService.updateSession(sessionId, updateData);
                    
                    setSettingsVersion(v => v + 1);
                    const updatedSettingsConfig = {
                        persona: updatedPersona,
                        customPrompt: updatedPrompt
                    };
                    saveConfigToStorage(updatedSettingsConfig);
                    break;
                }
                case 'PARAMETER_UPDATE': {
                    // Only proceed if ModelContext is ready
                    if (!isModelReady) {
                        console.warn('LegacySessionContext: Cannot update parameters - ModelContext not ready');
                        throw new Error('Model context not ready');
                    }
                    
                    // Forward to ModelContext through our hook-based proxy
                    updateModelParameters(values);
                    setSettingsVersion(v => v + 1);
                    break;
                }
                default:
                    break;
            }
        } catch (err) {
            setError(`Failed to update settings: ${err.message}`);
            showErrorToast(err, 'Failed to update settings');
        }
    };

    // save config to storage
    const saveConfigToStorage = (updatedConfig = {}) => {
        const configToSave = {
            modelName: updatedConfig.modelName || (isModelReady ? modelName : ''), // Use from useModel hook
            persona: updatedConfig.persona || persona,
            customPrompt: updatedConfig.customPrompt || customPrompt,
            modelParameters: updatedConfig.modelParameters || (isModelReady ? modelParameters : {}), // Use from useModel hook
            lastUpdated: new Date().toISOString()
        };

        console.log('Saving configuration to localStorage:', configToSave);
        localStorage.setItem("agent_config", JSON.stringify(configToSave));
    };
    
    // Proxy model operations to ModelContext through useModel hook
    const handleModelChange = useCallback((newModelName) => {
        // Only attempt to change model if we're fully initialized
        if (isModelReady) {
            console.log(`LegacySessionContext: Changing model to ${newModelName}`);
            return changeModel(newModelName);
        } else {
            console.warn(`LegacySessionContext: Cannot change model - ModelContext not ready`);
            return Promise.reject(new Error('Model context not ready'));
        }
    }, [isModelReady, changeModel]);
    
    const handleModelParameterChange = useCallback((paramName, paramValue) => {
        // Only attempt to update parameters if we're fully initialized
        if (isModelReady) {
            console.log(`LegacySessionContext: Updating parameter ${paramName} to ${paramValue}`);
            return updateModelParameters({ [paramName]: paramValue });
        } else {
            console.warn(`LegacySessionContext: Cannot update parameters - ModelContext not ready`);
            return Promise.reject(new Error('Model context not ready'));
        }
    }, [isModelReady, updateModelParameters]);

    // Handle equipping tools
    const handleEquipTools = async (tools) => {
        try {
            await toolsService.updateSessionTools(sessionId, tools);
            await fetchAgentTools();
            setSettingsVersion(v => v + 1);
            // saveConfigToStorage(); // this does nothing right now, but future request will be to pre-initialize tools
        } catch (err) {
            console.error("Failed to equip tools:", err);
            showErrorToast(err, 'Failed to equip tools');
            throw err;
        }
    };

    // Update processing status (for loading/spinner UI)
    const handleProcessingStatus = (status) => {
        setIsStreaming(status);
    };

    // Handle session deletion - use core context's method and add additional cleanup
    const handleSessionsDeleted = () => {
        // First call core context's method
        coreHandleSessionsDeleted();
        
        // Additional cleanup specific to legacy context
        localStorage.removeItem("agent_config");
        setActiveTools([]);
        setModelName("");
        setSelectedModel(null);
        setError(null);
    };
    
    // Handle theme change
    const handleThemeChange = (newTheme) => {
        setTheme(newTheme);
        localStorage.setItem('theme', newTheme);
    };

    // --- Effects ---
    useEffect(() => {
        fetchInitialData();
    }, []);
    
    // Sync initialization state with ModelContext readiness and session availability
    useEffect(() => {
        // When ModelContext becomes ready, set our own initialization flag
        if (isModelReady) {
            console.log('🔴 LegacySessionContext: ModelContext is now READY, marking as initialized');
            setIsInitialized(true);
        } else {
            console.log('🔴 LegacySessionContext: ModelContext not ready yet, waiting for initialization');
            
            // FIX: When we have a session but ModelContext isn't ready, don't try to initialize again
            // This was causing a circular loop where each context kept reinitializing the others
        }
    }, [isModelReady]);
    
    // Debug logging on mount and state changes
    useEffect(() => {
        console.log('🔴 LegacySessionContext STATE UPDATE:', {
            hasSessionId: !!sessionId,
            isSessionReady: isReady,
            isModelReady,
            ownIsInitialized: isInitialized
        });
    }, [sessionId, isReady, isModelReady, isInitialized]);

    useEffect(() => {
        if (sessionId && isReady) {
            fetchAgentTools();
        }
    }, [sessionId, isReady, modelName]);

    return (
        <LegacySessionContext.Provider
            value={{
                // Forward the core session state
                sessionId,
                isReady,
                error,
                
                // Include model state from useModel hook with fallbacks
                modelName: isModelReady ? modelName : (modelName || ''),
                modelParameters: isModelReady ? modelParameters : {},
                modelConfigs: modelConfigs.length > 0 ? modelConfigs : [],
                selectedModel: isModelReady ? selectedModel : null,
                
                // Legacy state and methods
                settingsVersion,
                isOptionsOpen,
                setIsOptionsOpen,
                isStreaming,
                isLoading,
                // For initialization status, use our own flag AND ModelContext's ready state
                // This ensures components won't try to use model data until it's fully ready
                isInitialized: isInitialized && (isModelReady || false),
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