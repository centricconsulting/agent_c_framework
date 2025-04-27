import React, { createContext, useState, useEffect, useRef } from 'react';
import logger from '@/lib/logger';
import apiService from '@/lib/apiService';
import storageService from '@/lib/storageService';
import { trackContextInitialization } from '@/lib/diagnostic';

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
        
        // Track the start of AuthContext initialization
        trackContextInitialization('AuthContext', 'start', { initStartTime });
        
        // Set a timeout to ensure initialization doesn't get stuck
        initTimeoutRef.current = setTimeout(() => {
            if (isInitializing) {
                logger.warn('Auth initialization timed out after 5 seconds', 'AuthContext');
                setIsInitializing(false);
                trackContextInitialization('AuthContext', 'error', { 
                    error: `Authentication initialization timed out after ${INIT_TIMEOUT/1000} seconds`,
                    timeoutMs: INIT_TIMEOUT
                });
            }
        }, INIT_TIMEOUT);
        
        // Try to load session from localStorage
        try {
            const storedSessionId = storageService.getSessionId();
            
            if (storedSessionId) {
                logger.init('Found stored session ID', 'AuthContext', { sessionId: storedSessionId });
                trackContextInitialization('AuthContext', 'update', { 
                    foundSessionId: true, 
                    sessionIdLength: storedSessionId.length
                });
                setSessionId(storedSessionId);
                setIsAuthenticated(true);
            } else {
                logger.init('No stored session ID found', 'AuthContext');
                trackContextInitialization('AuthContext', 'update', { foundSessionId: false });
            }
        } catch (error) {
            logger.error('Error retrieving session ID from storage', 'AuthContext', { error });
            trackContextInitialization('AuthContext', 'error', { 
                error: error.message,
                operation: 'retrieveSessionId'
            });
        }
        
        // Complete initialization with a small delay to ensure state updates
        setTimeout(() => {
            setIsInitializing(false);
            const initDuration = performance.now() - initStartTime;
            
            logger.init('Auth initialization complete', 'AuthContext', { 
                hasSessionId: !!sessionId,
                initializationTimeMs: Math.round(initDuration)
            });
            
            // Track successful completion of AuthContext initialization
            trackContextInitialization('AuthContext', 'complete', {
                hasSessionId: !!sessionId,
                initializationTimeMs: Math.round(initDuration),
                isAuthenticated: !!sessionId
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
        trackContextInitialization('AuthContext', 'update', { 
            operation: 'initializeSession',
            forceNew,
            hasConfig: !!sessionConfig
        });
        
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
            trackContextInitialization('AuthContext', 'error', { 
                error: `Session initialization timed out after ${SESSION_INIT_TIMEOUT/1000} seconds`,
                timeoutMs: SESSION_INIT_TIMEOUT,
                operation: 'initializeSession'
            });
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
                storageService.saveSessionId(data.ui_session_id);
                
                // Update state
                setSessionId(data.ui_session_id);
                setIsAuthenticated(true);
                setAuthError(null);
                
                logger.info('Session initialized successfully', 'AuthContext', { 
                    sessionId: data.ui_session_id 
                });
                
                trackContextInitialization('AuthContext', 'update', {
                    sessionInitialized: true,
                    gotSessionId: true
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
            
            trackContextInitialization('AuthContext', 'error', {
                error: error.message,
                operation: 'initializeSession',
                stack: error.stack
            });
            
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
        storageService.removeSessionId();
        
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
            <div data-auth-provider="mounted" style={{ display: 'none' }}>
                AuthProvider diagnostic element
            </div>
            {children}
        </AuthContext.Provider>
    );
};