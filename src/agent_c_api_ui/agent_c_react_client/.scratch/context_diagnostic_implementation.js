// Enhanced diagnostic utility for context initialization

/**
 * Creates and initializes the context diagnostics system.
 * This should be called at application startup before any context providers.
 * 
 * @returns {Object} The diagnostic object that will track context initialization
 */
export function initializeContextDiagnostics() {
    const diagnostics = {
        // Timestamp when diagnostics were initialized
        startTime: Date.now(),
        
        // Track individual context states
        contexts: {
            theme: {
                initialized: false,
                startTime: null,
                endTime: null,
                error: null,
                state: null,
                sequence: -1
            },
            auth: {
                initialized: false,
                startTime: null,
                endTime: null,
                error: null,
                sessionId: null,
                isAuthenticated: false,
                sequence: -1
            },
            model: {
                initialized: false,
                startTime: null,
                endTime: null,
                error: null,
                selectedModel: null,
                sequence: -1
            },
            session: {
                initialized: false,
                startTime: null,
                endTime: null,
                error: null,
                chatId: null,
                sequence: -1
            }
        },
        
        // Track initialization sequence
        sequence: {
            current: 0,
            events: []
        },
        
        // Track all errors during initialization
        errors: [],
        
        // Helper functions
        recordContextStart: function(contextName) {
            if (!this.contexts[contextName]) {
                console.error(`Unknown context: ${contextName}`);
                return;
            }
            
            this.contexts[contextName].startTime = Date.now();
            this.contexts[contextName].sequence = this.sequence.current++;
            
            this.sequence.events.push({
                time: Date.now(),
                type: 'contextStart',
                context: contextName,
                sequence: this.contexts[contextName].sequence
            });
            
            console.log(`[CONTEXT:${contextName}] Initialization started`);
        },
        
        recordContextSuccess: function(contextName, state) {
            if (!this.contexts[contextName]) {
                console.error(`Unknown context: ${contextName}`);
                return;
            }
            
            this.contexts[contextName].initialized = true;
            this.contexts[contextName].endTime = Date.now();
            this.contexts[contextName].state = state;
            
            // Record specific state fields based on context type
            if (contextName === 'auth') {
                this.contexts.auth.sessionId = state?.sessionId;
                this.contexts.auth.isAuthenticated = !!state?.sessionId;
            } else if (contextName === 'model') {
                this.contexts.model.selectedModel = state?.selectedModel;
            } else if (contextName === 'session') {
                this.contexts.session.chatId = state?.chatId;
            }
            
            this.sequence.events.push({
                time: Date.now(),
                type: 'contextSuccess',
                context: contextName,
                duration: this.contexts[contextName].endTime - this.contexts[contextName].startTime
            });
            
            console.log(`[CONTEXT:${contextName}] Initialization completed successfully in ${this.contexts[contextName].endTime - this.contexts[contextName].startTime}ms`);
        },
        
        recordContextError: function(contextName, error) {
            if (!this.contexts[contextName]) {
                console.error(`Unknown context: ${contextName}`);
                return;
            }
            
            this.contexts[contextName].error = error;
            this.contexts[contextName].endTime = Date.now();
            
            this.errors.push({
                time: Date.now(),
                context: contextName,
                error: error,
                message: error?.message || 'Unknown error'
            });
            
            this.sequence.events.push({
                time: Date.now(),
                type: 'contextError',
                context: contextName,
                error: error?.message || 'Unknown error'
            });
            
            console.error(`[CONTEXT:${contextName}] Initialization failed:`, error);
        },
        
        recordEvent: function(type, details) {
            this.sequence.events.push({
                time: Date.now(),
                type,
                ...details
            });
            
            console.log(`[EVENT:${type}]`, details);
        },
        
        getInitializationSummary: function() {
            const now = Date.now();
            const totalTime = now - this.startTime;
            
            const contextsInitialized = Object.keys(this.contexts).filter(
                key => this.contexts[key].initialized
            ).length;
            
            const contextsWithErrors = Object.keys(this.contexts).filter(
                key => this.contexts[key].error
            ).length;
            
            return {
                totalTime,
                started: new Date(this.startTime).toISOString(),
                completed: contextsInitialized === Object.keys(this.contexts).length,
                contextsInitialized,
                contextsWithErrors,
                errors: this.errors.length,
                initializationSequence: Object.keys(this.contexts)
                    .sort((a, b) => this.contexts[a].sequence - this.contexts[b].sequence)
                    .filter(key => this.contexts[key].sequence >= 0)
                    .map(key => ({
                        context: key,
                        sequence: this.contexts[key].sequence,
                        duration: this.contexts[key].endTime ? 
                            this.contexts[key].endTime - this.contexts[key].startTime : 
                            'incomplete',
                        status: this.contexts[key].initialized ? 'success' : 
                                this.contexts[key].error ? 'error' : 'incomplete'
                    }))
            };
        }
    };
    
    // Make diagnostics available globally for console debugging
    window.__CONTEXT_DIAGNOSTICS = diagnostics;
    
    // Log initialization
    console.log('[CONTEXT DIAGNOSTICS] Initialized at', new Date(diagnostics.startTime).toISOString());
    
    return diagnostics;
}

/**
 * Creates a diagnostic wrapper for a context provider function.
 * This automatically records start, success, and error events.
 * 
 * @param {string} contextName - The name of the context (theme, auth, model, session)
 * @param {Function} providerFunction - The provider function to wrap
 * @returns {Function} The wrapped provider function
 */
export function withContextDiagnostics(contextName, providerFunction) {
    return function(...args) {
        try {
            // Check if diagnostics are initialized
            if (!window.__CONTEXT_DIAGNOSTICS) {
                console.warn(`[CONTEXT:${contextName}] Diagnostics not initialized`);
                return providerFunction(...args);
            }
            
            // Record start of context initialization
            window.__CONTEXT_DIAGNOSTICS.recordContextStart(contextName);
            
            // Call the original provider function
            const result = providerFunction(...args);
            
            // Handle promise results
            if (result && typeof result.then === 'function') {
                return result
                    .then(value => {
                        window.__CONTEXT_DIAGNOSTICS.recordContextSuccess(contextName, value);
                        return value;
                    })
                    .catch(error => {
                        window.__CONTEXT_DIAGNOSTICS.recordContextError(contextName, error);
                        throw error;
                    });
            }
            
            // Handle synchronous results
            window.__CONTEXT_DIAGNOSTICS.recordContextSuccess(contextName, result);
            return result;
        } catch (error) {
            // Handle synchronous errors
            if (window.__CONTEXT_DIAGNOSTICS) {
                window.__CONTEXT_DIAGNOSTICS.recordContextError(contextName, error);
            } else {
                console.error(`[CONTEXT:${contextName}] Error:`, error);
            }
            throw error;
        }
    };
}

/**
 * Creates an error boundary component specifically for context providers.
 * This component will catch errors in context initialization and render a fallback UI.
 * 
 * @param {Object} props - Component props
 * @param {string} props.contextName - The name of the context (theme, auth, model, session)
 * @param {React.ReactNode} props.children - The context provider and its children
 * @param {Function} props.fallback - Function to render fallback UI when error occurs
 * @returns {React.ReactNode}
 */
export function ContextErrorBoundary({ contextName, children, fallback }) {
    const [error, setError] = React.useState(null);
    
    React.useEffect(() => {
        return () => {
            // Clean up on unmount
            if (error && window.__CONTEXT_DIAGNOSTICS) {
                window.__CONTEXT_DIAGNOSTICS.recordEvent('errorBoundaryUnmount', {
                    context: contextName,
                    error: error?.message || 'Unknown error'
                });
            }
        };
    }, [contextName, error]);
    
    if (error) {
        if (window.__CONTEXT_DIAGNOSTICS) {
            window.__CONTEXT_DIAGNOSTICS.recordEvent('errorBoundaryRender', {
                context: contextName,
                error: error?.message || 'Unknown error'
            });
        }
        
        return fallback ? fallback(error) : (
            <div className="context-error-boundary">
                <h3>Error in {contextName} Context</h3>
                <p>{error?.message || 'An unknown error occurred'}</p>
                <button onClick={() => window.location.reload()}>
                    Reload Application
                </button>
            </div>
        );
    }
    
    try {
        return children;
    } catch (e) {
        if (window.__CONTEXT_DIAGNOSTICS) {
            window.__CONTEXT_DIAGNOSTICS.recordContextError(contextName, e);
        }
        setError(e);
        return null;
    }
}

/**
 * Logs detailed context state changes for debugging.
 * 
 * @param {string} contextName - The name of the context
 * @param {Object} prevState - Previous state
 * @param {Object} nextState - New state
 */
export function logContextStateChange(contextName, prevState, nextState) {
    if (!window.__CONTEXT_DIAGNOSTICS) return;
    
    // Find changed properties
    const changes = {};
    Object.keys(nextState || {}).forEach(key => {
        // Skip functions and complex objects for simple comparison
        if (
            typeof nextState[key] !== 'function' && 
            (typeof nextState[key] !== 'object' || nextState[key] === null)
        ) {
            if (JSON.stringify(prevState?.[key]) !== JSON.stringify(nextState[key])) {
                changes[key] = {
                    from: prevState?.[key],
                    to: nextState[key]
                };
            }
        }
    });
    
    if (Object.keys(changes).length > 0) {
        window.__CONTEXT_DIAGNOSTICS.recordEvent('stateChange', {
            context: contextName,
            changes
        });
    }
}

/**
 * Creates a diagnostic wrapper for useEffect cleanup functions
 * to track when components using contexts are mounted and unmounted.
 * 
 * @param {string} contextName - The name of the context
 * @param {string} componentName - The name of the component using the context
 * @returns {Function} Cleanup function that records the unmount event
 */
export function trackContextConsumer(contextName, componentName) {
    if (!window.__CONTEXT_DIAGNOSTICS) return () => {};
    
    window.__CONTEXT_DIAGNOSTICS.recordEvent('contextConsumerMounted', {
        context: contextName,
        component: componentName
    });
    
    return () => {
        window.__CONTEXT_DIAGNOSTICS.recordEvent('contextConsumerUnmounted', {
            context: contextName,
            component: componentName
        });
    };
}

/**
 * Exports a function to be called in the browser console to get the current diagnostics status.
 */
if (typeof window !== 'undefined') {
    window.getContextDiagnostics = function() {
        if (!window.__CONTEXT_DIAGNOSTICS) {
            console.warn('Context diagnostics not initialized');
            return null;
        }
        return window.__CONTEXT_DIAGNOSTICS.getInitializationSummary();
    };
}