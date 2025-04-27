import React, { createContext, useState, useEffect } from 'react';
import { API_URL } from '@/config/config';
import logger from '@/lib/logger';

// Create the context
export const AuthContext = createContext();

/**
 * AuthProvider handles session initialization and authentication state
 * This context is responsible for session management, API authentication, 
 * and related error handling
 */
export const AuthProvider = ({ children }) => {
    // Session state
    const [sessionId, setSessionId] = useState(null);
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [isInitializing, setIsInitializing] = useState(true);
    const [authError, setAuthError] = useState(null);

    // Initialize auth from localStorage on mount
    useEffect(() => {
        logger.init('AuthProvider initializing', 'AuthContext');
        console.log('🚀 AuthProvider initializing');
        
        // Set a timeout to ensure initialization doesn't get stuck
        const initTimeout = setTimeout(() => {
            if (isInitializing) {
                logger.warn('Auth initialization timed out after 5 seconds', 'AuthContext');
                console.warn('⚠️ AUTH TIMEOUT: Auth initialization timed out after 5 seconds');
                setIsInitializing(false);
                console.log('💢 DIAGNOSTIC: Force-completing auth initialization after timeout');
            }
        }, 5000);
        
        // Try to load session from localStorage
        let storedSessionId;
        try {
            storedSessionId = localStorage.getItem('ui_session_id');
            logger.storageOp('getItem', 'ui_session_id', true);
            console.log('📝 Found localStorage session ID:', storedSessionId);
        } catch (err) {
            logger.error('Error reading from localStorage', 'AuthContext', { error: err.message });
            logger.storageOp('getItem', 'ui_session_id', false);
            console.error('💢 Error reading from localStorage:', err.message);
        }
        
        if (storedSessionId) {
            logger.init('Found stored session ID', 'AuthContext', { sessionId: storedSessionId });
            console.log('📝 Found stored session ID:', storedSessionId);
            setSessionId(storedSessionId);
            setIsAuthenticated(true);
        } else {
            logger.init('No stored session ID found', 'AuthContext');
            console.log('📝 No stored session ID found, will need to create a new session');
        }
        
        // Complete initialization with a small delay to ensure state updates
        setTimeout(() => {
            setIsInitializing(false);
            logger.init('Auth initialization complete', 'AuthContext', { hasSessionId: !!storedSessionId });
            console.log('💡 Auth initialization complete, hasSessionId:', !!storedSessionId);
            
            // Clear timeout if initialization completes normally
            clearTimeout(initTimeout);
        }, 100);
        
        return () => {
            clearTimeout(initTimeout);
        };
    }, []);

    /**
     * Initialize a new session with the API
     * @param {Object} sessionConfig - Configuration for the new session
     * @param {boolean} forceNew - Force creation of a new session even if one exists
     * @returns {Promise<string>} The session ID
     */
    const initializeSession = async (sessionConfig, forceNew = false) => {
        logger.info('Initializing session', 'AuthContext', { forceNew, sessionConfig });
        
        // Set initializing state to true and clear any previous errors
        setIsInitializing(true);
        setAuthError(null);
        
        // Set a timeout to prevent hanging initialization
        const initTimeout = setTimeout(() => {
            logger.warn('Session initialization timed out after 10 seconds', 'AuthContext');
            setIsInitializing(false);
            setAuthError('Session initialization timed out after 10 seconds');
        }, 10000);
        
        try {
            if (!API_URL) {
                throw new Error('API_URL is undefined. Please check your environment variables.');
            }

            // Build request body
            const requestBody = { ...sessionConfig };
            
            // If we have an existing session and we're not forcing a new one, include the session ID
            if (sessionId && !forceNew) {
                requestBody.ui_session_id = sessionId;
                logger.debug('Using existing session ID', 'AuthContext', { sessionId });
            }
            
            logger.debug('Session initialization request', 'AuthContext', { requestBody });
            
            // Make API request
            const response = await fetch(`${API_URL}/initialize`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(requestBody)
            });
            
            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(`HTTP error! status: ${response.status} - ${errorText}`);
            }

            const data = await response.json();
            
            if (data.ui_session_id) {
                // Store session in localStorage
                localStorage.setItem('ui_session_id', data.ui_session_id);
                
                // Update state
                setSessionId(data.ui_session_id);
                setIsAuthenticated(true);
                setAuthError(null);
                
                logger.info('Session initialized successfully', 'AuthContext', { 
                    sessionId: data.ui_session_id 
                });
                
                // Clear the timeout since initialization succeeded
                clearTimeout(initTimeout);
                return data.ui_session_id;
            } else {
                throw new Error('No ui_session_id in response');
            }
        } catch (error) {
            logger.error('Session initialization failed', 'AuthContext', { 
                error: error.message, 
                stack: error.stack 
            });
            
            setAuthError(`Session initialization failed: ${error.message}`);
            // Clear the timeout since we've handled the error
            clearTimeout(initTimeout);
            return null;
        } finally {
            setIsInitializing(false);
        }
        // The timeout will be cleared in both success and error cases
        // No need to clear it here
    };

    /**
     * End the current session and clear authentication state
     */
    const logout = () => {
        logger.info('Logging out, removing session', 'AuthContext', { sessionId });
        
        try {
            // Remove from localStorage
            localStorage.removeItem('ui_session_id');
            logger.debug('Session ID removed from localStorage', 'AuthContext');
        } catch (error) {
            logger.error('Error removing session from localStorage', 'AuthContext', { 
                error: error.message 
            });
        }
        
        // Reset auth state
        setSessionId(null);
        setIsAuthenticated(false);
        setAuthError(null);
    };

    /**
     * Check if a response contains authentication errors
     * @param {Response} response - Fetch API response object
     * @returns {Promise<boolean>} True if authentication error detected
     */
    const checkAuthError = async (response) => {
        if (response.status === 401 || response.status === 403) {
            const errorText = await response.text();
            logger.warn('Authentication error from API', 'AuthContext', { 
                status: response.status,
                error: errorText 
            });
            
            setAuthError(`Authentication error: ${response.status} - ${errorText}`);
            return true;
        }
        return false;
    };

    // Context value
    const contextValue = {
        // State
        sessionId,
        isAuthenticated,
        isInitializing,
        authError,
        
        // Methods
        initializeSession,
        logout,
        checkAuthError
    };

    return (
        <AuthContext.Provider value={contextValue}>
            {children}
        </AuthContext.Provider>
    );
};