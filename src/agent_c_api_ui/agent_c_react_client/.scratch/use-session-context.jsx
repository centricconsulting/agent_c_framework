/**
 * Custom hook for using SessionContext with improved logging and error handling
 */

import { useContext } from 'react';
import { SessionContext } from '../contexts/SessionContext';
import logger from '../lib/logger';

/**
 * Custom hook that provides access to the SessionContext
 * @param {string} componentName - Name of the component using this hook (for logging)
 * @returns {Object} The SessionContext value with session state and methods
 */
export const useSessionContext = (componentName = 'unknown') => {
  try {
    const context = useContext(SessionContext);
    
    if (context === undefined) {
      const error = 'useSessionContext must be used within a SessionProvider';
      logger.error(error, componentName);
      throw new Error(error);
    }
    
    logger.debug(`${componentName} using SessionContext`, 'useSessionContext');
    return context;
  } catch (error) {
    logger.error(`useSessionContext error in ${componentName}`, 'useSessionContext', { error: error.message });
    throw error;
  }
};