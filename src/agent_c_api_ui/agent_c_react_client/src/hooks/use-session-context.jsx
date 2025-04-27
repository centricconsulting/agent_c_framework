/**
 * Enhanced useSessionContext hook with better error handling and debugging
 */

import { useContext } from 'react';
import { SessionContext } from '../contexts/SessionContext';
import logger from '../lib/logger';

/**
 * Custom hook that provides access to the SessionContext with better error handling
 * @param {string} componentName - Name of the component using this hook (for logging)
 * @returns {Object} The SessionContext value with session state and methods
 */
export const useSessionContext = (componentName = 'unknown') => {
  try {
    const context = useContext(SessionContext);
    
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
    
    // Dispatch logging (only in development)
    if (process.env.NODE_ENV === 'development') {
      logger.debug(`${componentName} using SessionContext`, 'useSessionContext');
    }
    
    return context;
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