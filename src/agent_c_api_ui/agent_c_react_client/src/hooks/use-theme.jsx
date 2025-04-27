/**
 * Temporary placeholder for the actual useTheme hook
 * This will be implemented in a future PR
 */

import { useContext } from 'react';
import { ThemeContext } from '../contexts/ThemeContext';
import logger from '../lib/logger';

/**
 * Hook for using the ThemeContext
 * @param {string} componentName - Component name for logging
 * @returns {Object} Theme context value
 */
export const useTheme = (componentName = 'unknown') => {
  try {
    const context = useContext(ThemeContext);
    
    if (context === undefined) {
      const error = 'useTheme must be used within a ThemeProvider';
      logger.error(error, componentName);
      throw new Error(error);
    }
    
    logger.debug(`${componentName} using ThemeContext`, 'useTheme');
    return context;
  } catch (error) {
    logger.error(`useTheme error in ${componentName}`, 'useTheme', { error: error.message });
    throw error;
  }
};