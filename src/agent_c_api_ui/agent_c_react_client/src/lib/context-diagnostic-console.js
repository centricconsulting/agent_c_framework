/**
 * Context Diagnostic Console
 * 
 * A utility that provides real-time visibility into React context initialization,
 * value changes, and dependency tracking to debug complex context issues.
 */

import logger from './logger';

// Create a singleton to track context lifecycle
class ContextDiagnosticConsole {
  constructor() {
    this.contexts = {};
    this.initializationSequence = [];
    this.valueChanges = {};
    this.errors = [];
    this.startTime = Date.now();
    
    // Store a reference in window for console access
    if (typeof window !== 'undefined') {
      window.__CONTEXT_DIAGNOSTIC = this;
      
      // Create a command to view current status
      window.viewContextDiagnostics = () => {
        console.group('📊 Context Diagnostic Console');
        console.log('Initialization Sequence:', this.initializationSequence);
        console.log('Contexts:', this.contexts);
        console.log('Value Changes:', this.valueChanges);
        console.log('Errors:', this.errors);
        console.groupEnd();
        return this.getStatus();
      };
      
      // Command to check session ID propagation specifically
      window.checkSessionIdPropagation = () => {
        return this.checkSessionIdPropagation();
      };
    }
  }
  
  /**
   * Initialize tracking for a context
   * @param {string} contextName - Name of the context
   * @param {Object} initialValue - Initial context value
   */
  trackContextInitialization(contextName, initialValue) {
    // Create context entry if it doesn't exist
    if (!this.contexts[contextName]) {
      this.contexts[contextName] = {
        name: contextName,
        mountCount: 0,
        initialized: false,
        initializedAt: null,
        lastUpdate: null,
        updateCount: 0,
        currentValue: null,
        valueHistory: [],
        errors: []
      };
    }
    
    const context = this.contexts[contextName];
    context.mountCount++;
    context.initialized = true;
    context.initializedAt = Date.now();
    context.currentValue = this.sanitizeValue(initialValue);
    
    // Add to initialization sequence
    this.initializationSequence.push({
      contextName,
      timestamp: Date.now(),
      timeSinceStart: Date.now() - this.startTime,
      action: 'initialize'
    });
    
    logger.debug(`Context ${contextName} initialized`, 'ContextDiagnosticConsole', {
      mountCount: context.mountCount,
      valueKeys: Object.keys(initialValue || {})
    });
  }
  
  /**
   * Track a context value update
   * @param {string} contextName - Name of the context
   * @param {Object} newValue - New context value
   * @param {string} [source] - Source of the update (e.g., component name)
   */
  trackContextUpdate(contextName, newValue, source = 'unknown') {
    if (!this.contexts[contextName]) {
      // Initialize if not tracked yet
      this.trackContextInitialization(contextName, newValue);
      return;
    }
    
    const context = this.contexts[contextName];
    context.lastUpdate = Date.now();
    context.updateCount++;
    
    // Sanitize the value to avoid circular references
    const sanitizedValue = this.sanitizeValue(newValue);
    
    // Find differences from previous value
    const changes = this.findValueChanges(context.currentValue, sanitizedValue);
    
    // Add to value changes tracking
    if (!this.valueChanges[contextName]) {
      this.valueChanges[contextName] = [];
    }
    
    if (Object.keys(changes).length > 0) {
      this.valueChanges[contextName].push({
        timestamp: Date.now(),
        timeSinceStart: Date.now() - this.startTime,
        source,
        changes
      });
      
      // Keep the history limited to avoid memory issues
      if (this.valueChanges[contextName].length > 100) {
        this.valueChanges[contextName].shift();
      }
      
      // Track value history (limited to 10 entries per context)
      context.valueHistory.push({
        timestamp: Date.now(),
        value: sanitizedValue,
        source
      });
      
      if (context.valueHistory.length > 10) {
        context.valueHistory.shift();
      }
      
      // Update current value
      context.currentValue = sanitizedValue;
      
      // Add to initialization sequence
      this.initializationSequence.push({
        contextName,
        timestamp: Date.now(),
        timeSinceStart: Date.now() - this.startTime,
        action: 'update',
        source,
        changeKeys: Object.keys(changes)
      });
      
      logger.debug(`Context ${contextName} updated`, 'ContextDiagnosticConsole', {
        source,
        updateCount: context.updateCount,
        changedKeys: Object.keys(changes)
      });
    }
  }
  
  /**
   * Track an error in a context
   * @param {string} contextName - Name of the context
   * @param {Error} error - The error that occurred
   * @param {string} [operation] - The operation that caused the error
   */
  trackContextError(contextName, error, operation = 'unknown') {
    if (!this.contexts[contextName]) {
      // Initialize tracking for this context if it doesn't exist
      this.contexts[contextName] = {
        name: contextName,
        mountCount: 0,
        initialized: false,
        initializedAt: null,
        lastUpdate: null,
        updateCount: 0,
        currentValue: null,
        valueHistory: [],
        errors: []
      };
    }
    
    const context = this.contexts[contextName];
    const errorInfo = {
      timestamp: Date.now(),
      message: error.message,
      stack: error.stack,
      operation
    };
    
    context.errors.push(errorInfo);
    this.errors.push({
      contextName,
      ...errorInfo
    });
    
    // Keep errors limited to avoid memory issues
    if (context.errors.length > 50) {
      context.errors.shift();
    }
    
    if (this.errors.length > 200) {
      this.errors.shift();
    }
    
    logger.error(`Context ${contextName} error: ${error.message}`, 'ContextDiagnosticConsole', {
      operation,
      stack: error.stack,
      contextState: this.getContextSummary(contextName)
    });
  }
  
  /**
   * Get a summary of all context states
   * @returns {Object} Summary of all contexts
   */
  getStatus() {
    const summary = {};
    
    for (const [name, context] of Object.entries(this.contexts)) {
      summary[name] = this.getContextSummary(name);
    }
    
    return {
      contextSummary: summary,
      initializationSequence: this.initializationSequence.slice(-10), // Just the last 10 events
      recentErrors: this.errors.slice(-5), // Last 5 errors
      recentValueChanges: Object.entries(this.valueChanges).reduce((acc, [ctxName, changes]) => {
        acc[ctxName] = changes.slice(-3); // Just the last 3 changes per context
        return acc;
      }, {})
    };
  }
  
  /**
   * Get a summary for a specific context
   * @param {string} contextName - Name of the context 
   * @returns {Object} Context summary
   */
  getContextSummary(contextName) {
    const context = this.contexts[contextName];
    
    if (!context) {
      return { exists: false, name: contextName };
    }
    
    return {
      exists: true,
      name: contextName,
      initialized: context.initialized,
      mountCount: context.mountCount,
      updateCount: context.updateCount,
      initializedAt: context.initializedAt,
      lastUpdate: context.lastUpdate,
      valueKeys: context.currentValue ? Object.keys(context.currentValue) : [],
      errorCount: context.errors.length,
      valuePreview: this.getValuePreview(context.currentValue)
    };
  }
  
  /**
   * Check session ID propagation across contexts and components
   * @returns {Object} Session ID propagation status
   */
  checkSessionIdPropagation() {
    // Extract session ID from different contexts
    const authContextSessionId = this.getContextValue('AuthContext', 'sessionId');
    const sessionContextSessionId = this.getContextValue('SessionContext', 'sessionId');
    const storedSessionId = typeof localStorage !== 'undefined' ? localStorage.getItem('ui_session_id') : null;
    
    // Get DOM element attributes
    let chatPageSessionId = null;
    let chatInterfaceSessionId = null;
    
    if (typeof document !== 'undefined') {
      const chatPageElement = document.querySelector('[data-chat-page]');
      if (chatPageElement) {
        chatPageSessionId = chatPageElement.getAttribute('data-session-id-value');
      }
      
      const chatInterfaceElement = document.querySelector('[data-testid="chat-interface"]');
      if (chatInterfaceElement) {
        chatInterfaceSessionId = chatInterfaceElement.getAttribute('data-session-id-value');
      }
    }
    
    const result = {
      authContext: {
        sessionId: authContextSessionId,
        type: typeof authContextSessionId,
        exists: authContextSessionId !== null && authContextSessionId !== undefined,
        valid: typeof authContextSessionId === 'string' && authContextSessionId.length > 0
      },
      sessionContext: {
        sessionId: sessionContextSessionId,
        type: typeof sessionContextSessionId,
        exists: sessionContextSessionId !== null && sessionContextSessionId !== undefined,
        valid: typeof sessionContextSessionId === 'string' && sessionContextSessionId.length > 0
      },
      localStorage: {
        sessionId: storedSessionId,
        type: typeof storedSessionId,
        exists: storedSessionId !== null && storedSessionId !== undefined,
        valid: typeof storedSessionId === 'string' && storedSessionId.length > 0
      },
      dom: {
        chatPage: {
          sessionId: chatPageSessionId,
          type: typeof chatPageSessionId,
          exists: chatPageSessionId !== null && chatPageSessionId !== undefined,
          valid: typeof chatPageSessionId === 'string' && chatPageSessionId.length > 0
        },
        chatInterface: {
          sessionId: chatInterfaceSessionId,
          type: typeof chatInterfaceSessionId,
          exists: chatInterfaceSessionId !== null && chatInterfaceSessionId !== undefined,
          valid: typeof chatInterfaceSessionId === 'string' && chatInterfaceSessionId.length > 0
        }
      },
      matches: {
        authToSession: authContextSessionId === sessionContextSessionId,
        authToStorage: authContextSessionId === storedSessionId,
        sessionToStorage: sessionContextSessionId === storedSessionId,
        authToChatPage: authContextSessionId === chatPageSessionId,
        sessionToChatPage: sessionContextSessionId === chatPageSessionId,
        storageToChatPage: storedSessionId === chatPageSessionId,
        chatPageToChatInterface: chatPageSessionId === chatInterfaceSessionId
      }
    };
    
    // Log for console debugging
    console.group('Session ID Propagation Check');
    console.log('Auth Context:', result.authContext);
    console.log('Session Context:', result.sessionContext);
    console.log('Local Storage:', result.localStorage);
    console.log('DOM Elements:', result.dom);
    console.log('Matches:', result.matches);
    console.groupEnd();
    
    return result;
  }
  
  /**
   * Get a specific value from a context
   * @param {string} contextName - Name of the context
   * @param {string} key - Key of the value to retrieve
   * @returns {any} The value or null if not found
   */
  getContextValue(contextName, key) {
    const context = this.contexts[contextName];
    if (!context || !context.currentValue) {
      return null;
    }
    
    return context.currentValue[key];
  }
  
  /**
   * Creates a preview of an object value suitable for logging
   * @param {any} value - The value to preview
   * @returns {Object} A simplified preview of the value
   */
  getValuePreview(value) {
    if (!value) return null;
    
    if (typeof value !== 'object') {
      return value;
    }
    
    // Create a limited preview for objects
    const preview = {};
    const keys = Object.keys(value).slice(0, 10); // Limit to 10 keys
    
    for (const key of keys) {
      if (typeof value[key] === 'function') {
        preview[key] = '(function)';
      } else if (typeof value[key] === 'object' && value[key] !== null) {
        preview[key] = Array.isArray(value[key]) ? 
          `(array:${value[key].length})` : 
          `(object:${Object.keys(value[key]).length})`;
      } else {
        preview[key] = value[key];
      }
    }
    
    if (Object.keys(value).length > 10) {
      preview['...'] = `(${Object.keys(value).length - 10} more keys)`;
    }
    
    return preview;
  }
  
  /**
   * Find changes between two objects
   * @param {Object} oldValue - Previous object value
   * @param {Object} newValue - New object value
   * @returns {Object} Object containing changed values
   */
  findValueChanges(oldValue, newValue) {
    if (!oldValue || !newValue) {
      return newValue ? { _all: 'new-value' } : {};
    }
    
    const changes = {};
    const allKeys = new Set([...Object.keys(oldValue), ...Object.keys(newValue)]);
    
    for (const key of allKeys) {
      // Skip functions
      if (typeof newValue[key] === 'function' || typeof oldValue[key] === 'function') {
        continue;
      }
      
      // Handle cases where a key is added/removed
      if (!(key in oldValue)) {
        changes[key] = { added: newValue[key] };
        continue;
      }
      
      if (!(key in newValue)) {
        changes[key] = { removed: true };
        continue;
      }
      
      // Check for value changes (simple comparison)
      if (oldValue[key] !== newValue[key]) {
        if (typeof oldValue[key] === 'object' && typeof newValue[key] === 'object' &&
            oldValue[key] !== null && newValue[key] !== null) {
          // For objects, just note that they changed to avoid deep comparison
          changes[key] = { changed: true, from: this.getValuePreview(oldValue[key]), to: this.getValuePreview(newValue[key]) };
        } else {
          changes[key] = { from: oldValue[key], to: newValue[key] };
        }
      }
    }
    
    return changes;
  }
  
  /**
   * Sanitize a value to avoid circular references
   * @param {any} value - The value to sanitize
   * @returns {any} Sanitized value safe for serialization
   */
  sanitizeValue(value) {
    if (!value || typeof value !== 'object') {
      return value;
    }
    
    try {
      // Test if the value can be serialized
      JSON.stringify(value);
      return value;
    } catch (e) {
      // If there's a circular reference, we need to sanitize
      const sanitized = {};
      
      for (const [key, val] of Object.entries(value)) {
        if (typeof val === 'function') {
          sanitized[key] = '(function)';
        } else if (typeof val === 'object' && val !== null) {
          sanitized[key] = Array.isArray(val) ? 
            `(array:${val.length})` : 
            `(object:${Object.keys(val).length})`;
        } else {
          sanitized[key] = val;
        }
      }
      
      return sanitized;
    }
  }
}

// Create and export singleton instance
const contextDiagnosticConsole = new ContextDiagnosticConsole();
export default contextDiagnosticConsole;

// Shorthand functions for easy access
export const trackContext = (contextName, value) => 
  contextDiagnosticConsole.trackContextInitialization(contextName, value);

export const updateContext = (contextName, value, source) => 
  contextDiagnosticConsole.trackContextUpdate(contextName, value, source);

export const contextError = (contextName, error, operation) => 
  contextDiagnosticConsole.trackContextError(contextName, error, operation);

export const getContextsStatus = () => 
  contextDiagnosticConsole.getStatus();

export const checkSessionPropagation = () =>
  contextDiagnosticConsole.checkSessionIdPropagation();