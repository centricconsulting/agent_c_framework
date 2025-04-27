import { useContext } from 'react';
import { AuthContext } from '@/contexts/AuthContext';
import logger from '@/lib/logger';

/**
 * Custom hook that provides access to the AuthContext
 * @param {string} componentName - Name of the component using this hook (for logging)
 * @returns {Object} The AuthContext value
 */
export const useAuth = (componentName = 'unknown') => {
  console.log(`🔑 useAuth called from ${componentName}`);
  
  try {
    const context = useContext(AuthContext);
    
    if (context === undefined) {
      const error = 'useAuth must be used within an AuthProvider';
      logger.error(error, componentName);
      console.error(`❌ useAuth error: ${error}`);
      throw new Error(error);
    }
    
    // Check if initializeSession is available
    if (typeof context.initializeSession !== 'function') {
      console.error(`⚠️ CRITICAL: initializeSession is not a function in AuthContext, type:`, 
        typeof context.initializeSession);
      console.log('AuthContext contents:', JSON.stringify(Object.keys(context)));
    } else {
      console.log(`✅ useAuth: initializeSession is a function`);
    }
    
    // Log the returned context
    console.log(`🔑 useAuth returning context with sessionId:`, context.sessionId);
    
    return context;
  } catch (error) {
    console.error(`❌ useAuth error:`, error);
    logger.error(`useAuth error in ${componentName}`, 'useAuth', { error: error.message });
    throw error;
  }
};

export default useAuth;