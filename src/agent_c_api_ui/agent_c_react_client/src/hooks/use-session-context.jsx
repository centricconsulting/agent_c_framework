/**
 * Enhanced useSessionContext hook with better error handling and debugging
 */

import { useContext, useEffect, useRef } from 'react';
import { SessionContext } from '../contexts/SessionContext';
import logger from '../lib/logger';

/**
 * Custom hook that provides access to the SessionContext with better error handling
 * @param {string} componentName - Name of the component using this hook (for logging)
 * @returns {Object} The SessionContext value with session state and methods
 */
export const useSessionContext = (componentName = 'unknown') => {
  // Create a ref to detect if this is the first render
  const isFirstRender = useRef(true);
  
  try {
    const context = useContext(SessionContext);
    
    // Track component using this context
    useEffect(() => {
      logger.debug(`${componentName} mounted with SessionContext`, 'useSessionContext');
      
      // Add this component to the list of components using SessionContext
      if (typeof window !== 'undefined' && !window.__SESSION_CONTEXT_USERS) {
        window.__SESSION_CONTEXT_USERS = {};
      }
      
      if (typeof window !== 'undefined') {
        window.__SESSION_CONTEXT_USERS[componentName] = {
          mountTime: Date.now(),
          isInitialized: context?.isInitialized || false,
          isReady: context?.isReady || false
        };
      }
      
      return () => {
        logger.debug(`${componentName} unmounted`, 'useSessionContext');
        if (typeof window !== 'undefined' && window.__SESSION_CONTEXT_USERS) {
          window.__SESSION_CONTEXT_USERS[componentName] = {
            ...window.__SESSION_CONTEXT_USERS[componentName],
            unmountTime: Date.now()
          };
        }
      };
    }, []);
    
    // Debug logging for the first render only
    if (isFirstRender.current) {
      logger.debug(`${componentName} using SessionContext (first render)`, 'useSessionContext', {
        hasContext: !!context,
        hasSessionId: !!(context && context.sessionId),
        sessionIdType: context ? typeof context.sessionId : 'undefined',
        hasDispatcher: !!(context && context.dispatch),
        isInitialized: context ? context.isInitialized : false,
        isReady: context ? context.isReady : false
      });
      
      isFirstRender.current = false;
    }
    
    if (!context) {
      const error = 'useSessionContext must be used within a SessionProvider';
      logger.error(error, 'useSessionContext', { componentName, context });
      throw new Error(error);
    }
    
    // Check for dispatcher being null
    if (!context.dispatch) {
      const error = 'dispatcher is null';
      logger.error(`useSessionContext error in ${componentName}`, 'useSessionContext', { 
        error,
        contextKeys: Object.keys(context),
        componentName
      });
      
      // Log additional diagnostic information to help debug this issue
      logger.debug('SessionContext detailed diagnostic', 'useSessionContext', {
        hasDispatcher: !!context.dispatch,
        contextType: typeof context,
        hasSessionProps: !!(context.isInitialized !== undefined),
        sessionId: context.sessionId,
        sessionIdType: typeof context.sessionId,
        sessionIdValid: typeof context.sessionId === 'string' && context.sessionId.length > 0,
        isInitialized: context.isInitialized,
        isReady: context.isReady,
        callingComponent: componentName
      });
      
      // Fallback to provide a safe object that won't crash the app, but will log errors
      // This is better than crashing the entire application
      const fallbackContext = { 
        ...context,
        // Safe fallback for dispatch function
        dispatch: (action) => {
          logger.error(`Attempted to dispatch action but dispatcher is null`, 'useSessionContext', { 
            action, 
            componentName 
          });
          // Return a resolved promise to prevent downstream errors
          return Promise.resolve(false);
        },
        // Add error property so components can display a proper error message
        error: error || 'Session context dispatcher is not initialized properly',
        // Ensure all vital methods have fallbacks
        setIsOptionsOpen: () => {
          logger.error('Called setIsOptionsOpen on fallback context', 'useSessionContext');
        },
        setIsStreaming: () => {
          logger.error('Called setIsStreaming on fallback context', 'useSessionContext');
        },
        updateAgentSettings: async () => {
          logger.error('Called updateAgentSettings on fallback context', 'useSessionContext');
          return false;
        },
        handleEquipTools: async () => {
          logger.error('Called handleEquipTools on fallback context', 'useSessionContext');
          return false;
        },
        handleProcessingStatus: () => {
          logger.error('Called handleProcessingStatus on fallback context', 'useSessionContext');
        },
        // Ensure these are at least initialized to reasonable values
        isInitialized: false,
        isReady: false,
        isLoading: false
      };
      
      // Also log this fallback to window for debugging
      if (typeof window !== 'undefined') {
        window.__SESSION_CONTEXT_FALLBACK = fallbackContext;
        window.__SESSION_CONTEXT_ORIGINAL = context;
        console.error('SessionContext fallback created - inspect window.__SESSION_CONTEXT_FALLBACK for details');
      }
      
      return fallbackContext;
    }
    
    // Normalize the sessionId to ensure consistent typing
    // This is crucial for 'shouldRenderChatInterface' in ChatPage.jsx
    const normalizedContext = {
      ...context,
      // Ensure sessionId is always a string or null, never undefined
      sessionId: context.sessionId || null
    };
    
    // Track usage of context
    if (typeof window !== 'undefined' && process.env.NODE_ENV === 'development') {
      if (!window.__SESSION_CONTEXT_USAGE) {
        window.__SESSION_CONTEXT_USAGE = {};
      }
      
      window.__SESSION_CONTEXT_USAGE[componentName] = {
        lastAccessed: new Date().toISOString(),
        isInitialized: normalizedContext.isInitialized,
        isReady: normalizedContext.isReady,
        hasSessionId: !!normalizedContext.sessionId,
        sessionIdType: typeof normalizedContext.sessionId
      };
    }
    
    // Log substantial changes to the context values (like session ID changes)
    useEffect(() => {
      if (normalizedContext.sessionId) {
        logger.debug(`${componentName} received session ID from context`, 'useSessionContext', {
          sessionId: normalizedContext.sessionId,
          sessionIdType: typeof normalizedContext.sessionId,
          isInitialized: normalizedContext.isInitialized,
          isReady: normalizedContext.isReady
        });
      }
    }, [normalizedContext.sessionId, componentName]);
    
    return normalizedContext;
  } catch (error) {
    logger.error(`useSessionContext error in ${componentName}`, 'useSessionContext', { 
      error: error.message,
      stack: error.stack 
    });
    
    // Provide a fallback context so the app doesn't completely crash
    return {
      isLoading: false,
      isInitialized: false,
      isReady: false,
      error: `Session context error: ${error.message}`,
      // Fallback dispatch function
      dispatch: () => {
        logger.error(`Attempted to use dispatch in error state`, 'useSessionContext');
      },
      // Empty methods that gracefully fail
      setIsOptionsOpen: () => {},
      setIsStreaming: () => {},
      updateAgentSettings: async () => {},
      handleEquipTools: async () => {},
      handleProcessingStatus: () => {},
      initializeAgentSession: async () => false,
      fetchAgentTools: async () => false,
      getChatCopyContent: () => '',
      getChatCopyHTML: () => '',
      setMessages: () => {}
    };
  }
};