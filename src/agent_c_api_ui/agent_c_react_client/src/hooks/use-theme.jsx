import { useContext } from 'react';
import { ThemeContext } from '@/contexts/ThemeContext';
import logger from '@/lib/logger';

/**
 * Custom hook that provides access to the ThemeContext
 * @param {string} componentName - Name of the component using this hook (for logging)
 * @returns {Object} The ThemeContext value with theme state and methods
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

export default useTheme;