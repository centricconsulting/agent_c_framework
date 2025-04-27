import React, { createContext, useState, useEffect, useRef } from 'react';
import logger from '@/lib/logger';
import { apiService } from '@/lib/apiService';
import { storageService } from '@/lib/storageService';

// Create the context
export const AuthContext = createContext();

// Constants
const SESSION_ID_KEY = 'ui_session_id';
const INIT_TIMEOUT = 5000; // ms for initial auth timeout
const SESSION_INIT_TIMEOUT = 10000; // ms for session initialization timeout

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
    
    // References for timeouts
    const initTimeoutRef = useRef(null);
    const sessionInitTimeoutRef = useRef(null);

    // Initialize auth from localStorage on mount
    useEffect(() => {
        const initStartTime = performance.now();
        logger.init('AuthProvider initializing', 'AuthContext');
        
        // Set a timeout to ensure initialization doesn't get stuck
        initTimeoutRef.current = setTimeout(() => {
            if (isInitializing) {
                logger.warn('Auth initialization timed out after 5 seconds', 'AuthContext');
                setIsInitializing(false);
            }
        }, INIT_TIMEOUT);
        
        // Try to load session from localStorage
        const storedSessionId = storageService.get(SESSION_ID_KEY);
        
        if (storedSessionId) {
            logger.init('Found stored session ID', 'AuthContext', { sessionId: storedSessionId });
            setSessionId(storedSessionId);
            setIsAuthenticated(true);
        } else {
            logger.init('No stored session ID found', 'AuthContext');
        }
        
        // Complete initialization with a small delay to ensure state updates
        setTimeout(() => {
            setIsInitializing(false);
            const initDuration = performance.now() - initStartTime;
            logger.init('Auth initialization complete', 'AuthContext', { 
                hasSessionId: !!storedSessionId,
                initializationTimeMs: Math.round(initDuration)
            });
            
            // Clear timeout if initialization completes normally
            if (initTimeoutRef.current) {
                clearTimeout(initTimeoutRef.current);
                initTimeoutRef.current = null;
            }
        }, 100);
        
        return () => {
            // Cleanup timeouts
            if (initTimeoutRef.current) {
                clearTimeout(initTimeoutRef.current);
                initTimeoutRef.current = null;
            }
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
        if (sessionInitTimeoutRef.current) {
            clearTimeout(sessionInitTimeoutRef.current);
        }
        
        sessionInitTimeoutRef.current = setTimeout(() => {
            logger.warn('Session initialization timed out', 'AuthContext', { timeoutMs: SESSION_INIT_TIMEOUT });
            setIsInitializing(false);
            setAuthError(`Session initialization timed out after ${SESSION_INIT_TIMEOUT/1000} seconds`);
        }, SESSION_INIT_TIMEOUT);
        
        try {
            // Build request body
            const requestBody = { ...sessionConfig };
            
            // If we have an existing session and we're not forcing a new one, include the session ID
            if (sessionId && !forceNew) {
                requestBody.ui_session_id = sessionId;
                logger.debug('Using existing session ID', 'AuthContext', { sessionId });
            }
            
            // Make API request using our centralized API service
            const data = await apiService.post('/initialize', requestBody);
            
            if (data.ui_session_id) {
                // Store session in localStorage via our storage service
                storageService.set(SESSION_ID_KEY, data.ui_session_id);
                
                // Update state
                setSessionId(data.ui_session_id);
                setIsAuthenticated(true);
                setAuthError(null);
                
                logger.info('Session initialized successfully', 'AuthContext', { 
                    sessionId: data.ui_session_id 
                });
                
                // Clear the timeout since initialization succeeded
                if (sessionInitTimeoutRef.current) {
                    clearTimeout(sessionInitTimeoutRef.current);
                    sessionInitTimeoutRef.current = null;
                }
                
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
            if (sessionInitTimeoutRef.current) {
                clearTimeout(sessionInitTimeoutRef.current);
                sessionInitTimeoutRef.current = null;
            }
            
            return null;
        } finally {
            setIsInitializing(false);
        }
    };

    /**
     * End the current session and clear authentication state
     */
    const logout = () => {
        logger.info('Logging out, removing session', 'AuthContext', { sessionId });
        
        // Remove from localStorage using our storage service
        storageService.remove(SESSION_ID_KEY);
        
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