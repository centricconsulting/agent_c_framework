/**
 * Simple hook for using SessionContext
 */

import { useContext } from 'react';
import { SessionContext } from '../contexts/SessionContext';
import logger from '../lib/logger';

/**
 * Custom hook that wraps useContext(SessionContext) with minimal logging
 * @param {string} componentName - Name of the component using this hook
 * @returns {Object} The SessionContext value
 */
export const useSessionContext = (componentName) => {
  const context = useContext(SessionContext);
  
  // Optional: log once when first importing context
  // Uncomment if you want this minimal tracking
  // logger.debug(`Using SessionContext`, componentName);
  
  return context;
};