import React, {createContext, useState, useEffect, useRef} from 'react';

// Import services from the new API service layer
import {
    api,
    session as sessionService,
    model as modelService,
    tools as toolsService,
    persona as personaService,
    showErrorToast
} from '../services';

export const SessionContext = createContext();

export const SessionProvider = ({children}) => {
    // Global session & UI state
    const [sessionId, setSessionId] = useState(null);
    const debouncedUpdateRef = useRef(null);
    const [error, setError] = useState(null);
    const [settingsVersion, setSettingsVersion] = useState(0);
    const [isOptionsOpen, setIsOptionsOpen] = useState(false);
    const [isStreaming, setIsStreaming] = useState(false);
    const [isLoading, setIsLoading] = useState(true);
    const [isInitialized, setIsInitialized] = useState(false);
    const [isReady, setIsReady] = useState(false);
    const [theme, setTheme] = useState(() => {
        // Check for saved theme preference in localStorage
        const savedTheme = localStorage.getItem('theme');
        return savedTheme || 'system';
    });

    // Agent settings & configuration
    const [persona, setPersona] = useState("");
    const [temperature, setTemperature] = useState(null);
    const [customPrompt, setCustomPrompt] = useState("");
    const [modelConfigs, setModelConfigs] = useState([]);
    const [modelName, setModelName] = useState("");
    const [modelParameters, setModelParameters] = useState({});
    const [selectedModel, setSelectedModel] = useState(null);

    // Data from backend
    const [personas, setPersonas] = useState([]);
    const [availableTools, setAvailableTools] = useState({
        essential_toolsets: [],
        groups: {},
        categories: []
    });
    const [activeTools, setActiveTools] = useState([]);

    // Fetch initial data (personas, tools, models)
    const fetchInitialData = async () => {
        try {
            console.log('Starting fetchInitialData');
            setIsLoading(true);
            setIsInitialized(false);

            // Parallel fetching using the new service layer
            const [personasData, toolsData, modelsData] = await Promise.all([
                personaService.getPersonas(),
                toolsService.getTools(),
                modelService.getModels()
            ]);
            
            // Store data returned from backend into their data structures
            setPersonas(personasData);
            setAvailableTools(toolsData);
            setModelConfigs(modelsData.models);

            // Try pulling config from localstorage and applying
            const savedConfig = localStorage.getItem("agent_config");
            if (savedConfig) {
                try {
                    const parsedConfig = JSON.parse(savedConfig);

                    // Check if the configuration is expired (14 days)
                    if (parsedConfig.lastUpdated) {
                        const configAge = new Date() - new Date(parsedConfig.lastUpdated);
                        const maxAgeMs = 14 * 24 * 60 * 60 * 1000; // 14 days

                        if (configAge > maxAgeMs) {
                            console.log('Saved configuration is too old (>14 days), using defaults');
                            localStorage.removeItem("agent_config");
                        } else {
                            // Verify the saved model still exists in the available models
                            const savedModel = modelsData.models.find(m => m.id === parsedConfig.modelName);

                            if (savedModel) {
                                console.log('Loading saved configuration:', parsedConfig);
                                // Use the saved model as the initial model
                                const initialModel = {
                                    id: savedModel.id,
                                    backend: savedModel.backend,
                                    persona_name: parsedConfig.persona || 'default',
                                    custom_prompt: parsedConfig.customPrompt || '',
                                    temperature: parsedConfig.modelParameters?.temperature,
                                    reasoning_effort: parsedConfig.modelParameters?.reasoning_effort,
                                    extended_thinking: parsedConfig.modelParameters?.extended_thinking,
                                    budget_tokens: parsedConfig.modelParameters?.budget_tokens
                                };

                                // Set state with saved values
                                setModelName(savedModel.id);
                                setSelectedModel(savedModel);
                                setPersona(parsedConfig.persona || 'default');
                                setCustomPrompt(parsedConfig.customPrompt || '');
                                setModelParameters(parsedConfig.modelParameters || {});

                                // Initialize session with saved configuration
                                await initializeSession(false, initialModel, modelsData.models);
                                setIsInitialized(true);
                                return; // Exit early as we've handled initialization
                            } else {
                                console.log('Saved model not found in available models, using defaults');
                            }
                        }
                    }
                } catch (err) {
                    console.error('Error parsing saved configuration:', err);
                    // Continue with default initialization if parsing fails
                }
            }

            // This is the default initialization path if a config is not saved
            if (modelsData.models.length > 0) {
                const initialModel = modelsData.models[0];
                setModelName(initialModel.id);
                setSelectedModel(initialModel);

                // non-reasoning model parameters
                const initialTemperature = initialModel.parameters?.temperature?.default ?? 0.3;
                // openai reasoning model parameters
                const initialReasoningEffort = initialModel.parameters?.reasoning_effort?.default;

                // claude reasoning model parameters
                const hasExtendedThinking = !!initialModel.parameters?.extended_thinking;
                const initialExtendedThinking = hasExtendedThinking ?
                    initialModel.parameters.extended_thinking.enabled === true : false;
                const initialBudgetTokens = hasExtendedThinking && initialExtendedThinking ?
                    initialModel.parameters.extended_thinking.budget_tokens?.default || 4000 : 0;

                const initialParameters = {
                    temperature: initialTemperature,
                    reasoning_effort: initialReasoningEffort,
                    extended_thinking: initialExtendedThinking,
                    budget_tokens: initialBudgetTokens
                };

                setModelParameters(initialParameters);

                // Choose a default persona (or fall back to the first)
                if (personasData.length > 0) {
                    const defaultPersona = personasData.find(p => p.name === 'default');
                    const initialPersona = defaultPersona || personasData[0];
                    setPersona(initialPersona.name);
                    setCustomPrompt(initialPersona.content);
                }

                // Verify that the persona exists in the list of available personas
                const personaExists = personasData.some(p => p.name === initialModel.persona_name);
                if (!personaExists && initialModel.persona_name && initialModel.persona_name !== 'default') {
                    console.warn(`Persona '${initialModel.persona_name}' not found in available personas, falling back to default`);
                    initialModel.persona_name = 'default';
                    setPersona('default');
                }

                // Initialize a session with the initial model
                await initializeSession(false, initialModel, modelsData.models);
                setIsInitialized(true);
            } else {
                throw new Error('No models available');
            }
        } catch (err) {
            console.error('Error fetching initial data:', err);
            setError(`Failed to load initial data: ${err.message}`);
            showErrorToast(err, 'Failed to load initial data');
        } finally {
            setIsLoading(false);
        }
    };

    // Initialize (or reinitialize) a session
    const initializeSession = async (forceNew = false, initialModel = null, modelConfigsData = null) => {
        setIsReady(false);
        try {
            // First, validate that we have model configurations available
            const models = modelConfigsData || modelConfigs;
            if (!models || models.length === 0) {
                throw new Error("No model configurations available");
            }

            // Determine which model to use
            let currentModel = null; // Initialize as null to ensure proper checking

            if (initialModel) {
                // Find the model in modelConfigs when we have an initialModel with an id
                if ('id' in initialModel && initialModel.id) {
                    currentModel = models.find(model => model.id === initialModel.id);
                    if (!currentModel) {
                        console.warn(`Model with id ${initialModel.id} not found in configurations, falling back to first model`);
                        // Fallback to the first model
                        if (models.length > 0) {
                            currentModel = models[0];
                            setModelName(currentModel.id);
                        }
                    }
                } else {
                    currentModel = initialModel;
                }
            } else if (modelName) {
                // Find the model by the current modelName state
                currentModel = models.find(model => model.id === modelName);
                if (!currentModel) {
                    console.warn(`Model with name ${modelName} not found in configurations. Falling back to first model`);
                    // Fallback to the first model
                    if (models.length > 0) {
                        currentModel = models[0];
                        setModelName(currentModel.id);
                    }
                }
            } else {
                // No model specified, use the first available model
                if (models.length > 0) {
                    currentModel = models[0];
                    console.log('No model specified, using default:', currentModel.id);
                    setModelName(currentModel.id);
                }
            }

            // Additional check to ensure we have a valid model before proceeding
            if (!currentModel) {
                throw new Error('No valid model configuration available');
            }

            console.log('Initializing session with model:', currentModel);

            // Build session configuration
            const sessionConfig = {
                model_name: currentModel.id,
                backend: currentModel.backend,
                persona_name: initialModel && initialModel.persona_name ? initialModel.persona_name : (persona || 'default')
            };

            // If we have an existing session and we're not forcing a new one, include the session ID
            if (sessionId && !forceNew) {
                sessionConfig.ui_session_id = sessionId;
                console.log(`Using existing session ID: ${sessionId} for model change`);
            }

            // Determine which custom prompt to use
            let promptToUse = null;

            // First priority: custom prompt from initialModel if provided
            if (initialModel && ('custom_prompt' in initialModel || 'customPrompt' in initialModel)) {
                promptToUse = initialModel.custom_prompt || initialModel.customPrompt;
                console.log('Using custom prompt from initialModel');
            }
            // Second priority: current state's custom prompt
            else if (customPrompt) {
                promptToUse = customPrompt;
            }

            // Always include custom prompt if available
            if (promptToUse) {
                sessionConfig.custom_prompt = promptToUse;
            }

            // Use parameters directly from initialModel if provided, otherwise use model config
            if (initialModel && typeof initialModel === 'object') {
                // Add temperature if available from initialModel
                if ('temperature' in initialModel) {
                    sessionConfig.temperature = initialModel.temperature;
                }

                // Add reasoning_effort if available from initialModel
                if ('reasoning_effort' in initialModel) {
                    sessionConfig.reasoning_effort = initialModel.reasoning_effort;
                }

                // Handle extended_thinking (either as boolean or object)
                if ('extended_thinking' in initialModel) {
                    const extThinking = initialModel.extended_thinking;

                    // Handle both object and boolean formats
                    if (typeof extThinking === 'object') {
                        // It's an object with enabled property
                        sessionConfig.extended_thinking = extThinking.enabled;

                        // If enabled is true, add budget_tokens
                        if (extThinking.enabled && ('budget_tokens' in initialModel)) {
                            sessionConfig.budget_tokens = initialModel.budget_tokens;
                        } else if (extThinking.enabled && ('budget_tokens' in extThinking)) {
                            sessionConfig.budget_tokens = extThinking.budget_tokens;
                        }

                        console.log(`Setting initial extended_thinking as object with enabled=${extThinking.enabled}`);
                    } else {
                        // It's a boolean
                        sessionConfig.extended_thinking = extThinking;

                        // If enabled is true, add budget_tokens
                        if (extThinking && ('budget_tokens' in initialModel)) {
                            sessionConfig.budget_tokens = initialModel.budget_tokens;
                        }
                    }
                }
                // Fallback to addModelParameters when no direct parameters were provided
                else {
                    addModelParameters(sessionConfig, currentModel);
                }
            } else {
                // Use model config parameters
                addModelParameters(sessionConfig, currentModel);
            }

            console.log('initializeSession data being sent:', sessionConfig);

            // Call the initialize endpoint via the session service
            const data = await sessionService.initialize(sessionConfig);
            
            if (data.ui_session_id) {
                localStorage.setItem("ui_session_id", data.ui_session_id);
                setSessionId(data.ui_session_id);
                setIsReady(true);
                setError(null);

                // Update modelName state to reflect the current model
                setModelName(currentModel.id);
                setSelectedModel(currentModel);
            } else {
                throw new Error("No ui_session_id in response");
            }
        } catch (err) {
            console.error("Session initialization failed:", err);
            setIsReady(false);
            setError(`Session initialization failed: ${err.message}`);
            showErrorToast(err, 'Session initialization failed');
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
                    setIsReady(false);
                    const newModel = modelConfigs.find(model => model.id === values.modelName);
                    if (!newModel) throw new Error('Invalid model configuration');

                    // Update the state first
                    setModelName(values.modelName);
                    setSelectedModel(newModel);

                    // Determine extended thinking parameters based on new model
                    const hasExtendedThinking = !!newModel.parameters?.extended_thinking;
                    const extendedThinkingDefault = hasExtendedThinking ?
                        newModel.parameters.extended_thinking.enabled === true : false;

                    const budgetTokensDefault = hasExtendedThinking && extendedThinkingDefault ?
                        newModel.parameters.extended_thinking.budget_tokens?.default || 5000 : 0;

                    const newParameters = {
                        temperature: newModel.parameters?.temperature?.default ?? modelParameters.temperature,
                        reasoning_effort: newModel.parameters?.reasoning_effort?.default ?? modelParameters.reasoning_effort,
                        extended_thinking: extendedThinkingDefault,
                        budget_tokens: budgetTokensDefault
                    };

                    setModelParameters(newParameters);

                    // Create the model object with all the data
                    const modelWithPersona = {
                        id: newModel.id,
                        backend: newModel.backend,
                        persona_name: persona,
                        custom_prompt: customPrompt,
                        extended_thinking: extendedThinkingDefault,
                        budget_tokens: budgetTokensDefault,
                        reasoning_effort: newParameters.reasoning_effort,
                        temperature: newParameters.temperature
                    };

                    // Initialize new session with the custom prompt
                    await initializeSession(false, modelWithPersona);
                    setSettingsVersion(v => v + 1);
                    const updatedModelConfig = {
                        modelName: values.modelName,
                        modelParameters: newParameters
                    };
                    saveConfigToStorage(updatedModelConfig);
                    break;
                }
                case 'SETTINGS_UPDATE': {
                    const updatedPersona = values.persona_name || persona;
                    const updatedPrompt = values.customPrompt || customPrompt;
                    setPersona(updatedPersona);
                    setCustomPrompt(updatedPrompt);

                    const updateData = {
                        ui_session_id: sessionId,
                        model_name: modelName,
                        backend: selectedModel?.backend,
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
                    if (debouncedUpdateRef.current) {
                        clearTimeout(debouncedUpdateRef.current);
                    }
                    debouncedUpdateRef.current = setTimeout(async () => {
                        const updatedParameters = {...modelParameters};

                        // ALWAYS include model_name and backend for all parameter updates
                        // This is critical for complex parameters like extended_thinking
                        const updateData = {
                            ui_session_id: sessionId,
                            model_name: modelName,
                            backend: selectedModel?.backend
                        };

                        // non-reasoning model parameters
                        if ('temperature' in values) {
                            updatedParameters.temperature = values.temperature;
                            updateData.temperature = values.temperature;
                        }

                        // openai reasoning model parameters
                        if ('reasoning_effort' in values) {
                            updatedParameters.reasoning_effort = values.reasoning_effort;
                            updateData.reasoning_effort = values.reasoning_effort;
                        }

                        // claude reasoning model parameters
                        if (selectedModel?.parameters?.extended_thinking) {
                            const extendedThinking = 'extended_thinking' in values
                                ? values.extended_thinking
                                : updatedParameters.extended_thinking || false;

                            updatedParameters.extended_thinking = extendedThinking;

                            if ('budget_tokens' in values) {
                                updatedParameters.budget_tokens = values.budget_tokens;
                                updateData.budget_tokens = values.budget_tokens;
                            } else if (extendedThinking) {
                                // If UI shows extended thinking enabled but no budget specified,
                                // use existing or default value
                                const budgetTokens = updatedParameters.budget_tokens || 5000;
                                updatedParameters.budget_tokens = budgetTokens;
                                updateData.budget_tokens = budgetTokens;
                            } else {
                                // If UI shows extended thinking disabled,
                                // ensure budget_tokens is 0
                                updatedParameters.budget_tokens = 0;
                                updateData.budget_tokens = 0;
                            }
                        }

                        console.log('Parameter update data being sent:', updateData);
                        setModelParameters(updatedParameters);
                        await modelService.updateModelParameters(sessionId, updateData);
                        
                        setSettingsVersion(v => v + 1);
                        const updatedParamConfig = {
                            modelParameters: updatedParameters
                        };
                        saveConfigToStorage(updatedParamConfig);
                    }, 300);
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
            modelName: updatedConfig.modelName || modelName,
            persona: updatedConfig.persona || persona,
            customPrompt: updatedConfig.customPrompt || customPrompt,
            modelParameters: updatedConfig.modelParameters || modelParameters,
            lastUpdated: new Date().toISOString()
        };

        console.log('Saving configuration to localStorage:', configToSave);
        localStorage.setItem("agent_config", JSON.stringify(configToSave));
    };

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

    // Handle session deletion
    const handleSessionsDeleted = () => {
        localStorage.removeItem("ui_session_id");
        localStorage.removeItem("agent_config");
        setSessionId(null);
        setIsReady(false);
        setActiveTools([]);
        setError(null);
        setModelName("");
        setSelectedModel(null);
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

    useEffect(() => {
        if (sessionId && isReady) {
            fetchAgentTools();
        }
    }, [sessionId, isReady, modelName]);

    return (
        <SessionContext.Provider
            value={{
                sessionId,
                error,
                settingsVersion,
                isOptionsOpen,
                setIsOptionsOpen,
                isStreaming,
                isLoading,
                isInitialized,
                isReady,
                persona,
                temperature,
                customPrompt,
                modelConfigs,
                modelName,
                modelParameters,
                selectedModel,
                personas,
                availableTools,
                activeTools,
                theme,
                handleThemeChange,
                fetchAgentTools,
                updateAgentSettings,
                handleEquipTools,
                handleProcessingStatus,
                handleSessionsDeleted
            }}
        >
            {children}
        </SessionContext.Provider>
    );
};