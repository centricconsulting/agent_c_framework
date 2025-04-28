import React, {createContext, useState, useEffect, useRef, useMemo, useContext} from 'react';
import { InitializationContext, InitState } from '@/contexts/InitializationContext';
// {useMemo} from 'react'; // debugging
import logger from '@/lib/logger';
import apiService from '@/lib/apiService';
import storageService from '@/lib/storageService';
import {trackContextInitialization} from '@/lib/diagnostic';
import {createInitTracer, InitilizationState} from '@/lib/initTracer';
import eventBus from '@/lib/eventBus';
import { AUTH_EVENTS, INIT_EVENTS } from '@/lib/events';

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
export const AuthProvider = ({children}) => {
    // Access initialization context
    const initialization = useContext(InitializationContext);
    
    // Session state
    const [sessionId, setSessionId] = useState(null);
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [isInitializing, setIsInitializing] = useState(true);
    const [authError, setAuthError] = useState(null);

    // References for timeouts
    const initTimeoutRef = useRef(null);
    const sessionInitTimeoutRef = useRef(null);

    // Debug purposes 33-68
    // Create a tracer for this context
    const tracer = useMemo(() => createInitTracer('AuthContext'), []);

    // In initialization logic
    useEffect(() => {
        tracer.setState(InitilizationState.AUTH_CHECKING_STORAGE);

        let foundSession = false;
        let storageError  = null;

        try {
            const stored = storageService.getSessionId();
            foundSession = Boolean(stored);
        } catch (err) {
            storageError = err;
        }

        // After checking storage
        if (foundSession) {
            tracer.setState(InitilizationState.AUTH_SESSION_FOUND);
        } else {
            tracer.setState(InitilizationState.AUTH_NO_SESSION);
        }

        // When creating a new session
        tracer.setState(InitilizationState.AUTH_CREATING_SESSION);

        // When ready
        tracer.setState(InitilizationState.AUTH_COMPLETE);

        // When error occurs
        if (storageError) {
            tracer.setError(storageError);
        }
    }, []);

    // Initialize auth from localStorage on mount
    useEffect(() => {
        const initStartTime = performance.now();
        logger.init('AuthProvider initializing', 'AuthContext');

        // Signal auth initialization is starting
        initialization.setInitState(InitState.AUTH_PENDING);
        initialization.startAuthPhase();
        
        // Publish auth phase started event
        eventBus.publish(
            INIT_EVENTS.AUTH_PHASE_STARTED, 
            { timestamp: Date.now() },
            { publisherName: 'AuthContext' }
        );

        // Track the start of AuthContext initialization
        trackContextInitialization('AuthContext', 'start', {initStartTime});

        // Set a timeout to ensure initialization doesn't get stuck
        initTimeoutRef.current = setTimeout(() => {
            if (isInitializing) {
                logger.warn('Auth initialization timed out after 5 seconds', 'AuthContext');
                setIsInitializing(false);
                trackContextInitialization('AuthContext', 'error', {
                    error: `Authentication initialization timed out after ${INIT_TIMEOUT / 1000} seconds`,
                    timeoutMs: INIT_TIMEOUT
                });
            }
        }, INIT_TIMEOUT);

        // Try to load session from localStorage
        try {
            const sessionData = storageService.getSessionData();

            if (sessionData && sessionData.sessionId) {
                logger.init('Found stored session data', 'AuthContext', {
                    sessionId: sessionData.sessionId,
                    created: sessionData.created
                });
                trackContextInitialization('AuthContext', 'update', {
                    foundSessionId: true,
                    sessionIdLength: sessionData.sessionId.length,
                    sessionAge: new Date() - new Date(sessionData.created)
                });
                setSessionId(sessionData.sessionId);
                setIsAuthenticated(true);
            } else {
                logger.init('No stored session data found', 'AuthContext');
                trackContextInitialization('AuthContext', 'update', {foundSessionId: false});
            }
        } catch (error) {
            logger.error('Error retrieving session ID from storage', 'AuthContext', {error});
            // Signal error in initialization context
            initialization.authError(error.message);
            
            // Publish auth error event
            eventBus.publish(
                INIT_EVENTS.AUTH_PHASE_ERROR,
                { 
                    error: error.message,
                    operation: 'retrieveSessionId'
                },
                { publisherName: 'AuthContext' }
            );
            
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

            // Automatically create a session if none exists
            if (!sessionId) {
                logger.info('No session ID found during initialization, preparing for session creation', 'AuthContext');
                // We don't create the session directly here, but signal readiness for SessionContext to do so
                setIsAuthenticated(true); // Allow the flow to continue even without a session ID
            }

            // Track successful completion of AuthContext initialization
            trackContextInitialization('AuthContext', 'complete', {
                hasSessionId: !!sessionId,
                initializationTimeMs: Math.round(initDuration),
                isAuthenticated: !!sessionId
            });
            
            // Signal auth initialization is complete
            initialization.completeAuthPhase();
            initialization.setInitState(InitState.AUTH_COMPLETE);
            
            // Publish auth phase completed event
            eventBus.publish(
                INIT_EVENTS.AUTH_PHASE_COMPLETED,
                { 
                    hasSessionId: !!sessionId,
                    initializationTimeMs: Math.round(initDuration),
                    isAuthenticated: !!sessionId
                },
                { publisherName: 'AuthContext' }
            );

            // Clear timeout if initialization completes normally
            if (initTimeoutRef.current) {
                clearTimeout(initTimeoutRef.current);
                initTimeoutRef.current = null;
            }
        }, 100);

        // Setup a new timeout to detect stuck initialization
        const initStateTimeout = setTimeout(() => {
            if (initialization.phases.auth.state !== 'complete') {
                logger.error('Auth initialization timed out in state machine', 'AuthContext');
                initialization.authError('Initialization timed out');
                
                // Publish auth error event
                eventBus.publish(
                    INIT_EVENTS.AUTH_PHASE_ERROR,
                    { 
                        error: 'Auth initialization timed out in state machine',
                        timeoutMs: INIT_TIMEOUT + 2000
                    },
                    { publisherName: 'AuthContext' }
                );
            }
        }, INIT_TIMEOUT + 2000); // Give a bit more time than the normal timeout

        return () => {
            // Cleanup timeouts
            if (initTimeoutRef.current) {
                clearTimeout(initTimeoutRef.current);
                initTimeoutRef.current = null;
            }
            clearTimeout(initStateTimeout);
        };
    }, []);

    /**
     * Initialize a new session with the API
     * @param {Object} sessionConfig - Configuration for the new session
     * @param {boolean} forceNew - Force creation of a new session even if one exists
     * @returns {Promise<string>} The session ID
     */
    const initializeSession = async (sessionConfig, forceNew = false) => {
        if (sessionConfig && sessionConfig.ui_session_id && !forceNew) {
            logger.info('Updating session ID locally without API call', 'AuthContext', {
                newSessionId: sessionConfig.ui_session_id
            });

            // Store session in localStorage via our storage service
            storageService.saveSessionData({
                sessionId: sessionConfig.ui_session_id,
                created: new Date().toISOString()
            });

            // Update state
            setSessionId(sessionConfig.ui_session_id);
            setIsAuthenticated(true);
            setAuthError(null);

            trackContextInitialization('AuthContext', 'update', {
                sessionUpdated: true,
                apiCallSkipped: true
            });

            return sessionConfig.ui_session_id;
        }

        logger.info('Initializing session', 'AuthContext', {forceNew, sessionConfig});
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
            logger.warn('Session initialization timed out', 'AuthContext', {timeoutMs: SESSION_INIT_TIMEOUT});
            setIsInitializing(false);
            setAuthError(`Session initialization timed out after ${SESSION_INIT_TIMEOUT / 1000} seconds`);
            trackContextInitialization('AuthContext', 'error', {
                error: `Session initialization timed out after ${SESSION_INIT_TIMEOUT / 1000} seconds`,
                timeoutMs: SESSION_INIT_TIMEOUT,
                operation: 'initializeSession'
            });
        }, SESSION_INIT_TIMEOUT);

        try {
            // Build request body
            const requestBody = {...sessionConfig};

            // If we have an existing session and we're not forcing a new one, include the session ID
            if (sessionId && !forceNew) {
                requestBody.ui_session_id = sessionId;
                logger.debug('Using existing session ID', 'AuthContext', {sessionId});
            }

            // Make API request using our centralized API service
            const data = await apiService.post('/initialize', requestBody);

            if (data.ui_session_id) {
                // Store session in localStorage via our storage service
                storageService.saveSessionData({
                    sessionId: data.ui_session_id,
                    created: new Date().toISOString(),
                    // Store any additional session metadata from the API response
                    apiResponse: data
                });

                // Update state
                setSessionId(data.ui_session_id);
                setIsAuthenticated(true);
                setAuthError(null);
                
                // Publish session created/updated event
                eventBus.publish(
                    sessionId && !forceNew ? AUTH_EVENTS.SESSION_UPDATED : AUTH_EVENTS.SESSION_CREATED, 
                    { sessionId: data.ui_session_id },
                    { publisherName: 'AuthContext' }
                );

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

            const errorMessage = `Session initialization failed: ${error.message}`;
            setAuthError(errorMessage);

            trackContextInitialization('AuthContext', 'error', {
                error: error.message,
                operation: 'initializeSession',
                stack: error.stack
            });
            
            // Publish auth error event
            eventBus.publish(
                AUTH_EVENTS.AUTH_ERROR,
                { 
                    message: errorMessage,
                    operation: 'initializeSession',
                    error: error.message,
                    stack: error.stack
                },
                { publisherName: 'AuthContext' }
            );

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
        logger.info('Logging out, removing session', 'AuthContext', {sessionId});

        // Remove from localStorage using our storage service
        storageService.removeSessionData();

        // Publish session deleted event before changing state
        eventBus.publish(
            AUTH_EVENTS.SESSION_DELETED,
            { sessionId },
            { publisherName: 'AuthContext' }
        );

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

            const errorMessage = `Authentication error: ${response.status} - ${errorText}`;
            setAuthError(errorMessage);
            
            // Publish auth error event
            eventBus.publish(
                AUTH_EVENTS.AUTH_ERROR,
                { 
                    message: errorMessage,
                    status: response.status,
                    responseText: errorText
                },
                { publisherName: 'AuthContext' }
            );
            
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
            <div data-auth-provider="mounted" style={{display: 'none'}}>
                AuthProvider diagnostic element
            </div>
            {children}
        </AuthContext.Provider>
    );
};