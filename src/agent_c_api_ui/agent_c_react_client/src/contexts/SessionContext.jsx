import React, { createContext, useState, useEffect, useRef, useReducer } from 'react';
import { API_URL } from '@/config/config';
import logger from '@/lib/logger';
import apiService from '@/lib/apiService';
import storageService from '@/lib/storageService';
import { useAuth } from '@/hooks/use-auth';
import { useModel } from '@/hooks/use-model';
import { trackContextInitialization, completeContextInitialization } from '@/lib/diagnostic';

if (!API_URL) {
    logger.error('API_URL is not defined! Environment variables may not be loading correctly.', 'SessionContext', {
        'import.meta.env.VITE_API_URL': import.meta.env.VITE_API_URL,
        'process.env.VITE_API_URL': process.env?.VITE_API_URL,
        'NODE_ENV': process.env?.NODE_ENV
    });
}

// Initial state for the reducer
const initialState = {
    // Chat UI state
    isOptionsOpen: false,
    isStreaming: false,
    isLoading: true,
    isInitialized: false,
    isReady: false,
    error: null,
    settingsVersion: 0,
    
    // Agent settings & configuration
    persona: "",
    customPrompt: "",
    
    // Data from backend
    personas: [],
    availableTools: {
        essential_tools: [],
        groups: {},
        categories: []
    },
    activeTools: [],
    
    // Chat messages
    messages: [],
};

// Define action types
const SESSION_ACTIONS = {
    SET_OPTIONS_OPEN: 'SET_OPTIONS_OPEN',
    SET_STREAMING: 'SET_STREAMING',
    SET_LOADING: 'SET_LOADING',
    SET_INITIALIZED: 'SET_INITIALIZED',
    SET_READY: 'SET_READY',
    SET_ERROR: 'SET_ERROR',
    INCREMENT_SETTINGS_VERSION: 'INCREMENT_SETTINGS_VERSION',
    SET_PERSONA: 'SET_PERSONA',
    SET_CUSTOM_PROMPT: 'SET_CUSTOM_PROMPT',
    SET_PERSONAS: 'SET_PERSONAS',
    SET_AVAILABLE_TOOLS: 'SET_AVAILABLE_TOOLS',
    SET_ACTIVE_TOOLS: 'SET_ACTIVE_TOOLS',
    SET_MESSAGES: 'SET_MESSAGES',
};

// Reducer function to handle state updates
function sessionReducer(state, action) {
    switch (action.type) {
        case SESSION_ACTIONS.SET_OPTIONS_OPEN:
            return { ...state, isOptionsOpen: action.payload };
        case SESSION_ACTIONS.SET_STREAMING:
            return { ...state, isStreaming: action.payload };
        case SESSION_ACTIONS.SET_LOADING:
            return { ...state, isLoading: action.payload };
        case SESSION_ACTIONS.SET_INITIALIZED:
            return { ...state, isInitialized: action.payload };
        case SESSION_ACTIONS.SET_READY:
            return { ...state, isReady: action.payload };
        case SESSION_ACTIONS.SET_ERROR:
            return { ...state, error: action.payload };
        case SESSION_ACTIONS.INCREMENT_SETTINGS_VERSION:
            return { ...state, settingsVersion: state.settingsVersion + 1 };
        case SESSION_ACTIONS.SET_PERSONA:
            return { ...state, persona: action.payload };
        case SESSION_ACTIONS.SET_CUSTOM_PROMPT:
            return { ...state, customPrompt: action.payload };
        case SESSION_ACTIONS.SET_PERSONAS:
            return { ...state, personas: action.payload };
        case SESSION_ACTIONS.SET_AVAILABLE_TOOLS:
            return { ...state, availableTools: action.payload };
        case SESSION_ACTIONS.SET_ACTIVE_TOOLS:
            return { ...state, activeTools: action.payload };
        case SESSION_ACTIONS.SET_MESSAGES:
            return { ...state, messages: action.payload };
        default:
            return state;
    }
}

// Create context with default values to prevent null/undefined issues
export const SessionContext = createContext({
    // Default state values
    ...initialState,
    // Default dispatch function that returns a Promise to avoid errors
    dispatch: (action) => {
        console.warn(`Session context dispatch called before initialization with action:`, action);
        return Promise.resolve(false); // Return false to indicate operation failed
    },
    // Default implementations of all methods to prevent null pointer errors
    setIsOptionsOpen: () => console.warn('Session context not initialized: setIsOptionsOpen'),
    setIsStreaming: () => console.warn('Session context not initialized: setIsStreaming'),
    setIsLoading: () => console.warn('Session context not initialized: setIsLoading'),
    setIsInitialized: () => console.warn('Session context not initialized: setIsInitialized'),
    setIsReady: () => console.warn('Session context not initialized: setIsReady'),
    setError: () => console.warn('Session context not initialized: setError'),
    initializeAgentSession: async () => {
        console.warn('Session context not initialized: initializeAgentSession');
        return false;
    },
    fetchAgentTools: async () => {
        console.warn('Session context not initialized: fetchAgentTools');
        return false;
    },
    updateAgentSettings: async () => {
        console.warn('Session context not initialized: updateAgentSettings');
        return false;
    },
    handleEquipTools: async () => {
        console.warn('Session context not initialized: handleEquipTools');
        return false;
    },
    handleProcessingStatus: () => console.warn('Session context not initialized: handleProcessingStatus'),
    getChatCopyContent: () => '',
    getChatCopyHTML: () => '',
    setMessages: () => console.warn('Session context not initialized: setMessages')
});

/**
 * SessionProvider focuses on chat-specific functionality.
 * Authentication is handled by AuthContext.
 * Model management is handled by ModelContext.
 * Theme management is handled by ThemeContext.
 */
export const SessionProvider = ({ children }) => {
    // Initialize logger
    logger.info('SessionProvider initializing', 'SessionProvider');
    
    // Track the start of SessionContext initialization
    trackContextInitialization('SessionContext', 'start');
    
    // Debug the value passed to the provider
    logger.debug('SessionProvider initializing with value', 'SessionProvider', {
        hasChildren: !!children
    });
    
    // Access other contexts
    const { sessionId, isAuthenticated, isInitializing: authInitializing, initializeSession } = useAuth('SessionProvider');
    const { modelName, selectedModel, isLoading: modelLoading } = useModel('SessionProvider');
    
    // Enhanced debugging for auth context
    logger.debug('Auth context in SessionProvider:', 'SessionProvider', {
        hasSessionId: !!sessionId,
        sessionId: sessionId,
        isAuthenticated,
        authInitializing
    });
    
    // Track dependency state
    trackContextInitialization('SessionContext', 'update', {
        hasSessionId: !!sessionId,
        hasModelName: !!modelName,
        hasAuthData: isAuthenticated,
        authStillInitializing: authInitializing,
        modelStillLoading: modelLoading
    });
    
    // Initialize state with useReducer
    const [state, dispatch] = useReducer(sessionReducer, initialState);
    
    // Destructuring state for easier access
    const {
        isOptionsOpen, isStreaming, isLoading, isInitialized, isReady, error,
        settingsVersion, persona, customPrompt, personas, availableTools,
        activeTools, messages
    } = state;
    
    // Action creator helper functions to dispatch actions
    const setIsOptionsOpen = (value) => dispatch({ type: SESSION_ACTIONS.SET_OPTIONS_OPEN, payload: value });
    const setIsStreaming = (value) => dispatch({ type: SESSION_ACTIONS.SET_STREAMING, payload: value });
    const setIsLoading = (value) => dispatch({ type: SESSION_ACTIONS.SET_LOADING, payload: value });
    const setIsInitialized = (value) => dispatch({ type: SESSION_ACTIONS.SET_INITIALIZED, payload: value });
    const setIsReady = (value) => dispatch({ type: SESSION_ACTIONS.SET_READY, payload: value });
    const setError = (value) => dispatch({ type: SESSION_ACTIONS.SET_ERROR, payload: value });
    const incrementSettingsVersion = () => dispatch({ type: SESSION_ACTIONS.INCREMENT_SETTINGS_VERSION });
    const setPersona = (value) => dispatch({ type: SESSION_ACTIONS.SET_PERSONA, payload: value });
    const setCustomPrompt = (value) => dispatch({ type: SESSION_ACTIONS.SET_CUSTOM_PROMPT, payload: value });
    const setPersonas = (value) => dispatch({ type: SESSION_ACTIONS.SET_PERSONAS, payload: value });
    const setAvailableTools = (value) => dispatch({ type: SESSION_ACTIONS.SET_AVAILABLE_TOOLS, payload: value });
    const setActiveTools = (value) => dispatch({ type: SESSION_ACTIONS.SET_ACTIVE_TOOLS, payload: value });
    const setMessages = (value) => dispatch({ type: SESSION_ACTIONS.SET_MESSAGES, payload: value });
    
    // Ref to track dependency initialization
    const dependenciesInitializedRef = useRef(false);
    
    // Format the entire chat for copying
    const getChatCopyContent = () => {
        if (!messages || messages.length === 0) {
            return '';
        }
        
        // Import the function on-demand if we need to format
        const { createClipboardContent } = require('@/components/chat_interface/utils/htmlChatFormatter');
        const clipboardContent = createClipboardContent(messages);
        return clipboardContent.text;
    };
    
    // Get HTML version for rich copying
    const getChatCopyHTML = () => {
        if (!messages || messages.length === 0) {
            return '';
        }
        
        // Import the function on-demand if we need to format
        const { createClipboardContent } = require('@/components/chat_interface/utils/htmlChatFormatter');
        const clipboardContent = createClipboardContent(messages);
        return clipboardContent.html;
    };

    // Fetch initial data (personas, tools)
    const fetchInitialData = async () => {
        try {
            logger.info('Starting fetchInitialData', 'SessionContext');
            trackContextInitialization('SessionContext', 'update', { operation: 'fetchInitialData:start' });
            
            if (!API_URL) {
                const error = 'API_URL is undefined. Please check your environment variables.';
                trackContextInitialization('SessionContext', 'error', { error, operation: 'fetchInitialData' });
                throw new Error(error);
            }
            setIsLoading(true);
            setIsInitialized(false);

            // Parallel fetching using apiService
            logger.debug('Fetching personas and tools in parallel', 'SessionContext');
            const startTime = performance.now();
            const [personasData, toolsData] = await Promise.all([
                apiService.getPersonas(),
                apiService.getTools()
            ]);
            const fetchDuration = performance.now() - startTime;
            logger.performance('SessionContext', 'Initial API fetches', Math.round(fetchDuration));
            
            logger.debug('Initial data fetched successfully', 'SessionContext', {
                personasCount: personasData?.length,
                toolsCount: Object.keys(toolsData?.groups || {}).length
            });
            
            // Store data returned from backend
            setPersonas(personasData);
            setAvailableTools(toolsData);
            
            // Choose a default persona (or fall back to the first)
            if (personasData.length > 0) {
                const defaultPersona = personasData.find(p => p.name === 'default');
                const initialPersona = defaultPersona || personasData[0];
                setPersona(initialPersona.name);
                setCustomPrompt(initialPersona.content);
                logger.debug('Setting initial persona', 'SessionContext', {
                    personaName: initialPersona.name,
                    isDefault: initialPersona.name === 'default',
                    promptLength: initialPersona.content?.length
                });
            }

            setIsInitialized(true);
            
            trackContextInitialization('SessionContext', 'update', {
                operation: 'fetchInitialData:success',
                personasLoaded: personasData?.length || 0,
                toolsLoaded: Object.keys(toolsData?.groups || {}).length,
                initialPersona: personasData.length > 0 ? (
                    personasData.find(p => p.name === 'default')?.name || personasData[0].name
                ) : null
            });
        } catch (err) {
            logger.error('Error fetching initial data', 'SessionContext', { error: err.message, stack: err.stack });
            setError(`Failed to load initial data: ${err.message}`);
            
            trackContextInitialization('SessionContext', 'error', { 
                operation: 'fetchInitialData',
                error: err.message,
                stack: err.stack
            });
        } finally {
            setIsLoading(false);
            logger.debug('fetchInitialData completed', 'SessionContext', { 
                isInitialized: isInitialized
            });
        }
    };

    // Initialize (or reinitialize) the agent session with the API
    const initializeAgentSession = async () => {
        // Skip if auth isn't ready or we don't have a model yet
        if (authInitializing || modelLoading || !modelName) {
            logger.debug('Skipping initializeAgentSession - dependencies not ready', 'SessionContext', { 
                authInitializing, 
                modelLoading, 
                hasModelName: !!modelName 
            });
            trackContextInitialization('SessionContext', 'update', { 
                operation: 'initializeAgentSession:skipped',
                reason: 'dependencies not ready',
                authInitializing,
                modelLoading,
                hasModelName: !!modelName
            });
            return false;
        }
        
        try {
            logger.info('Initializing agent session with API', 'SessionContext', { 
                sessionId, 
                modelName, 
                persona 
            });
            trackContextInitialization('SessionContext', 'update', { 
                operation: 'initializeAgentSession:start',
                sessionId: !!sessionId,
                modelName
            });
            
            // Build the initialization payload
            const initPayload = {
                model_name: modelName,
                persona_name: persona || 'default',
                custom_prompt: customPrompt
            };
            
            // If we have an existing sessionId, include it for reconnection
            if (sessionId) {
                initPayload.ui_session_id = sessionId;
                logger.debug('Using existing session ID for initialization', 'SessionContext', { sessionId });
            }
            
            // Make the API call to initialize the agent
            const response = await apiService.initializeAgent(initPayload);
            
            // Enhanced debugging for response
            logger.debug('Api initializeAgent response received:', 'SessionContext', {
                hasResponse: !!response,
                responseType: typeof response,
                hasUiSessionId: !!(response && response.ui_session_id),
                uiSessionId: response && response.ui_session_id ? response.ui_session_id : null,
                rawResponse: JSON.stringify(response)
            });
            
            // The issue is here - the response coming back from the API has ui_session_id but it's not being handled correctly
            if (response && response.ui_session_id) {
                // Success - we got a session ID back
                logger.info('Agent session initialized successfully', 'SessionContext', { 
                    responseSessionId: response.ui_session_id,
                    matchesExisting: response.ui_session_id === sessionId
                });
                
                // Save the new session ID to our auth context if it's different from the current one
                if (response.ui_session_id !== sessionId) {
                    logger.info('Found new session ID, updating app state', 'SessionContext', {
                        newSessionId: response.ui_session_id,
                        oldSessionId: sessionId
                    });
                    
                    // Use our helper to update the session ID everywhere
                    const updated = await updateSessionIdAcrossApp(response.ui_session_id);
                    
                    if (!updated) {
                        logger.warn('Failed to update session ID across application, continuing anyway', 'SessionContext');
                    }
                }
                
                // Mark the session as ready now that the backend is initialized
                setIsReady(true);
                setError(null);
                
                trackContextInitialization('SessionContext', 'update', { 
                    operation: 'initializeAgentSession:success',
                    sessionId: response.ui_session_id,
                    isReady: true
                });
                
                return true;
            } else {
                // Failed to get a session ID from the response
                const error = 'Invalid response from initialization API';
                logger.error(error, 'SessionContext', { response });
                setError(`Session initialization failed: ${error}`);
                setIsReady(false);
                
                trackContextInitialization('SessionContext', 'error', { 
                    operation: 'initializeAgentSession',
                    error,
                    response
                });
                
                return false;
            }
        } catch (err) {
            // Handle any errors from the API call
            logger.error('Session initialization failed', 'SessionContext', { 
                error: err.message, 
                stack: err.stack 
            });
            setIsReady(false);
            setError(`Session initialization failed: ${err.message}`);
            
            trackContextInitialization('SessionContext', 'error', { 
                operation: 'initializeAgentSession',
                error: err.message,
                stack: err.stack
            });
            
            return false;
        }
    };
    
    // Fetch agent tools for the current session
    const fetchAgentTools = async () => {
        if (!sessionId || !isReady) {
            logger.debug('Skipping fetchAgentTools - session not ready', 'SessionContext', { sessionId, isReady });
            trackContextInitialization('SessionContext', 'update', { 
                operation: 'fetchAgentTools:skipped',
                reason: 'session not ready',
                hasSessionId: !!sessionId,
                isReady
            });
            return false;
        }
        try {
            logger.debug('Fetching agent tools', 'SessionContext', { sessionId });
            const data = await apiService.getAgentTools(sessionId);
            
            if (data.status === 'success' && Array.isArray(data.initialized_tools)) {
                const toolNames = data.initialized_tools.map(tool => tool.class_name);
                setActiveTools(toolNames);
                logger.debug('Agent tools fetched successfully', 'SessionContext', { toolCount: toolNames.length });
                return true;
            } else {
                logger.warn('Unexpected response from get_agent_tools', 'SessionContext', { data });
                return false;
            }
        } catch (err) {
            logger.error('Error fetching agent tools', 'SessionContext', { error: err.message, stack: err.stack });
            return false;
        }
    };

    // Update agent settings (settings update only - model changes handled by ModelContext)
    const updateAgentSettings = async (updateType, values) => {
        if (!sessionId || !isReady) {
            logger.warn('Skipping updateAgentSettings - session not ready', 'SessionContext', { sessionId, isReady, updateType });
            return;
        }
        try {
            logger.info('updateAgentSettings called', 'SessionContext', { updateType, values });
            
            switch (updateType) {
                case 'SETTINGS_UPDATE': {
                    const updatedPersona = values.persona_name || persona;
                    const updatedPrompt = values.customPrompt || customPrompt;
                    setPersona(updatedPersona);
                    setCustomPrompt(updatedPrompt);

                    const settings = {
                        model_name: modelName,
                        persona_name: updatedPersona,
                        custom_prompt: updatedPrompt
                    };

                    logger.debug('Settings update data being sent', 'SessionContext', { 
                        sessionId,
                        modelName,
                        persona: updatedPersona,
                        promptLength: updatedPrompt?.length 
                    });

                    await apiService.updateSettings(sessionId, settings);
                    incrementSettingsVersion();
                    logger.info('Settings updated successfully', 'SessionContext', { persona: updatedPersona });
                    break;
                }
                default:
                    logger.warn(`Unknown updateType: ${updateType}`, 'SessionContext');
                    break;
            }
        } catch (err) {
            logger.error(`Failed to update settings`, 'SessionContext', { updateType, error: err.message, stack: err.stack });
            setError(`Failed to update settings: ${err.message}`);
        }
    };

    // Handle equipping tools
    const handleEquipTools = async (tools) => {
        try {
            logger.info('Equipping tools', 'SessionContext', { toolCount: tools.length });
            await apiService.updateTools(sessionId, tools);
            await fetchAgentTools();
            incrementSettingsVersion();
            logger.info('Tools equipped successfully', 'SessionContext');
        } catch (err) {
            logger.error("Failed to equip tools", 'SessionContext', { error: err.message, stack: err.stack });
            throw err;
        }
    };

    // Update processing status (for loading/spinner UI)
    const handleProcessingStatus = (status) => {
        logger.debug('Processing status updated', 'SessionContext', { isStreaming: status });
        setIsStreaming(status);
    };

    // --- Effects ---
    useEffect(() => {
        logger.debug('SessionProvider mounted, checking dependencies', 'SessionContext', {
            authInitializing,
            modelLoading
        });
        
        // Only proceed with fetchInitialData if auth and model contexts are ready
        if (!authInitializing && !modelLoading) {
            logger.debug('Dependencies ready, fetching initial data', 'SessionContext', {
                hasSessionId: !!sessionId,
                hasModelName: !!modelName
            });
            
            fetchInitialData().then(() => {
                // Always mark the context as complete after initial data fetch
                // The initialization was successful even if we didn't get data
                setIsInitialized(true);
                trackContextInitialization('SessionContext', 'complete', {
                    success: true,
                    isInitialized: true,
                    isReady,
                    hasTools: availableTools?.categories?.length > 0,
                    hasPersonas: personas?.length > 0,
                    dependenciesInitialized: true
                });
                
                // After fetching initial data, we need to initialize the agent session
                if (sessionId && modelName) {
                    logger.info('Initial data loaded, now initializing agent session with API', 'SessionContext');
                    initializeAgentSession().then(success => {
                        if (success) {
                            logger.info('Agent session initialized successfully after initial data load', 'SessionContext');
                            // Now fetch agent tools since we're properly initialized
                            fetchAgentTools();
                        }
                    }).catch(err => {
                        logger.error('Failed to initialize agent after initial data load', 'SessionContext', { 
                            error: err.message, 
                            stack: err.stack 
                        });
                    });
                }
            }).catch(err => {
                logger.error('Fatal error in fetchInitialData', 'SessionContext', { 
                    error: err.message, 
                    stack: err.stack 
                });
                
                // Set initialized even with error to prevent UI blockage
                setIsInitialized(true);
                setError(`Error initializing session: ${err.message}`);
                
                trackContextInitialization('SessionContext', 'complete', {
                    success: false,
                    error: err.message,
                    isInitialized: true,
                    errorRecovery: true
                });
            });
        } else {
            logger.debug('Waiting for dependencies to initialize before fetching data', 'SessionContext', {
                authInitializing,
                modelLoading
            });
        }
        
        return () => {
            logger.debug('SessionProvider unmounting', 'SessionContext');
        };
    }, [authInitializing, modelLoading]);

    // Helper to update session ID across all relevant places
    const updateSessionIdAcrossApp = async (newSessionId) => {
        if (!newSessionId) {
            logger.error('updateSessionIdAcrossApp called with empty session ID', 'SessionContext');
            return false;
        }
        
        logger.info('Updating session ID across application', 'SessionContext', {
            newSessionId,
            oldSessionId: sessionId
        });
        
        try {
            // 1. Save to localStorage
            const storedSuccessfully = await storageService.saveSessionId(newSessionId);
            if (!storedSuccessfully) {
                logger.error('Failed to save session ID to localStorage', 'SessionContext');
            }
            
            // 2. Update Auth context if the function is available
            if (typeof initializeSession === 'function') {
                await initializeSession({ ui_session_id: newSessionId }, false);
                logger.debug('Called initializeSession on AuthContext', 'SessionContext');
            } else {
                logger.error('initializeSession function not available from AuthContext', 'SessionContext');
            }
            
            // 3. Additional logging
            logger.debug('Session ID updated across application', 'SessionContext', {
                newSessionId,
                savedToStorage: storedSuccessfully,
                updatedAuthContext: typeof initializeSession === 'function'
            });
            
            return true;
        } catch (err) {
            logger.error('Error updating session ID across application', 'SessionContext', {
                error: err.message,
                stack: err.stack
            });
            return false;
        }
    };

    // Effect to detect when sessionId and model are available but agent isn't initialized
    useEffect(() => {
        // This effect handles the case where we get auth and model contexts ready
        // after SessionContext is already mounted
        const initializeAgentIfNeeded = async () => {
            // Only run if we have both a sessionId and modelName, but agent isn't marked as ready yet
            if (sessionId && modelName && !isReady && isInitialized) {
                logger.info('Dependencies changed - sessionId and modelName available, initializing agent', 'SessionContext', {
                    sessionId, 
                    modelName,
                    isReady,
                    isInitialized
                });
                
                try {
                    // Initialize the agent with the API
                    const success = await initializeAgentSession();
                    if (success) {
                        logger.info('Agent successfully initialized after dependency changes', 'SessionContext');
                        // Agent is now initialized, fetch tools
                        await fetchAgentTools();
                    }
                } catch (err) {
                    logger.error('Error initializing agent after dependency changes', 'SessionContext', {
                        error: err.message,
                        stack: err.stack
                    });
                }
            }
        };
        
        initializeAgentIfNeeded();
    }, [sessionId, modelName, isReady, isInitialized]);

    // Effect to fetch tools when session is ready
    useEffect(() => {
        // Only fetch tools when session is fully ready
        if (sessionId && isReady && isInitialized) {
            logger.debug('Session is ready, fetching agent tools', 'SessionContext', { 
                sessionId,
                isAuthenticated,
                modelName
            });
            
            fetchAgentTools().then(success => {
                logger.debug('Agent tools fetch completed', 'SessionContext', { success });
                
                // Once everything is initialized and tools are fetched, signal app readiness
                completeContextInitialization(true, {
                    readyTimestamp: Date.now(),
                    sessionId: !!sessionId,
                    modelName: modelName,
                    isInitialized,
                    hasTools: success,
                    dependenciesInitialized: true
                });
            }).catch(err => {
                logger.error('Error fetching agent tools', 'SessionContext', { 
                    error: err.message, 
                    stack: err.stack 
                });
            });
        }
    }, [sessionId, isReady, isInitialized]);

    // Create a value object with all the state and actions
    const contextValue = {
        // State from reducer
        ...state,
        
        // Expose dispatch for advanced usage
        dispatch,
        
        // Action creators
        setIsOptionsOpen,
        setIsStreaming,
        setIsLoading,
        setIsInitialized,
        setIsReady,
        setError,
        
        // Additional methods
        initializeAgentSession,
        fetchAgentTools,
        updateAgentSettings,
        handleEquipTools,
        handleProcessingStatus,
        getChatCopyContent,
        getChatCopyHTML,
        setMessages
    };

    return (
        <SessionContext.Provider value={contextValue}>
            <div data-session-provider="mounted" data-session-initialized={isInitialized} style={{ display: 'none' }}>
                SessionProvider diagnostic element
            </div>
            {children}
        </SessionContext.Provider>
    );
};