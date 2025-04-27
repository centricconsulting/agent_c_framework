import React, { createContext, useState, useEffect, useRef } from 'react';
import { API_URL } from '@/config/config';
import logger from '@/lib/logger';
import apiService from '@/lib/apiService';
import storageService from '@/lib/storageService';
import { useAuth } from '@/hooks/use-auth';
import { useModel } from '@/hooks/use-model';

export const SessionContext = createContext();

/**
 * SessionProvider focuses on chat-specific functionality.
 * Authentication is handled by AuthContext.
 * Model management is handled by ModelContext.
 * Theme management is handled by ThemeContext.
 */
export const SessionProvider = ({ children }) => {
    // Initialize logger
    logger.info('SessionProvider initializing', 'SessionProvider');
    
    // Access other contexts
    const { sessionId, isAuthenticated } = useAuth('SessionProvider');
    const { modelName, selectedModel } = useModel('SessionProvider');
    
    // Chat UI state
    const [isOptionsOpen, setIsOptionsOpen] = useState(false);
    const [isStreaming, setIsStreaming] = useState(false);
    const [isLoading, setIsLoading] = useState(true);
    const [isInitialized, setIsInitialized] = useState(false);
    const [isReady, setIsReady] = useState(false);
    const [error, setError] = useState(null);
    const [settingsVersion, setSettingsVersion] = useState(0);
    
    // Agent settings & configuration
    const [persona, setPersona] = useState("");
    const [customPrompt, setCustomPrompt] = useState("");
    
    // Data from backend
    const [personas, setPersonas] = useState([]);
    const [availableTools, setAvailableTools] = useState({
        essential_tools: [],
        groups: {},
        categories: []
    });
    const [activeTools, setActiveTools] = useState([]);
    
    // Chat messages
    const [messages, setMessages] = useState([]);
    
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
            if (!API_URL) {
                throw new Error('API_URL is undefined. Please check your environment variables.');
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
        } catch (err) {
            logger.error('Error fetching initial data', 'SessionContext', { error: err.message, stack: err.stack });
            setError(`Failed to load initial data: ${err.message}`);
        } finally {
            setIsLoading(false);
            logger.debug('fetchInitialData completed', 'SessionContext', { 
                isInitialized: isInitialized
            });
        }
    };

    // Fetch agent tools for the current session
    const fetchAgentTools = async () => {
        if (!sessionId || !isReady) {
            logger.debug('Skipping fetchAgentTools - session not ready', 'SessionContext', { sessionId, isReady });
            return;
        }
        try {
            logger.debug('Fetching agent tools', 'SessionContext', { sessionId });
            const data = await apiService.getAgentTools(sessionId);
            
            if (data.status === 'success' && Array.isArray(data.initialized_tools)) {
                const toolNames = data.initialized_tools.map(tool => tool.class_name);
                setActiveTools(toolNames);
                logger.debug('Agent tools fetched successfully', 'SessionContext', { toolCount: toolNames.length });
            } else {
                logger.warn('Unexpected response from get_agent_tools', 'SessionContext', { data });
            }
        } catch (err) {
            logger.error('Error fetching agent tools', 'SessionContext', { error: err.message, stack: err.stack });
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
                    setSettingsVersion(v => v + 1);
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
            setSettingsVersion(v => v + 1);
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
        logger.debug('SessionProvider mounted, fetching initial data', 'SessionContext');
        fetchInitialData();
        
        return () => {
            logger.debug('SessionProvider unmounting', 'SessionContext');
        };
    }, []);

    useEffect(() => {
        if (sessionId && isReady) {
            logger.debug('Session is ready, fetching agent tools', 'SessionContext', { sessionId });
            fetchAgentTools();
        }
    }, [sessionId, isReady]);

    // Effect to update isReady when session/model state changes
    useEffect(() => {
        // Session is ready when we have a valid session ID and model
        const ready = !!sessionId && !!modelName;
        if (ready !== isReady) {
            logger.debug(`Session ready state changed to ${ready}`, 'SessionContext', { sessionId, modelName });
            setIsReady(ready);
        }
    }, [sessionId, modelName, isReady]);

    return (
        <SessionContext.Provider
            value={{
                // Session state
                error,
                settingsVersion,
                isOptionsOpen,
                setIsOptionsOpen,
                isStreaming,
                isLoading,
                isInitialized,
                isReady,
                
                // Agent configuration state
                persona,
                customPrompt,
                
                // Backend data
                personas,
                availableTools,
                activeTools,
                
                // Methods
                fetchAgentTools,
                updateAgentSettings,
                handleEquipTools,
                handleProcessingStatus,
                
                // Chat message management
                messages,
                setMessages,
                getChatCopyContent,
                getChatCopyHTML
            }}
        >
            {children}
        </SessionContext.Provider>
    );
};