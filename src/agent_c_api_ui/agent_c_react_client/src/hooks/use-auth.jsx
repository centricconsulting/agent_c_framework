import { useContext } from 'react';
import { AuthContext } from '@/contexts/AuthContext';
import logger from '@/lib/logger';

/**
 * Custom hook that provides access to the AuthContext
 * @param {string} componentName - Name of the component using this hook (for logging)
 * @returns {Object} The AuthContext value with authentication state and methods
 */
export const useAuth = (componentName = 'unknown') => {
  try {
    const context = useContext(AuthContext);
    
    if (context === undefined) {
      const error = 'useAuth must be used within an AuthProvider';
      logger.error(error, componentName);
      throw new Error(error);
    }
    
    logger.debug(`${componentName} using AuthContext`, 'useAuth');
    return context;
  } catch (error) {
    logger.error(`useAuth error in ${componentName}`, 'useAuth', { error: error.message });
    throw error;
  }
};

export default useAuth;