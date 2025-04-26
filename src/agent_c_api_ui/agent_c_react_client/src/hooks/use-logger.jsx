/**
 * Minimal logger hooks for component tracking
 */

import { useEffect } from 'react';
import logger from '../lib/logger';

/**
 * Simple hook to log component mounting/unmounting
 * @param {string} componentName - Name of the component
 */
export const useLogger = (componentName) => {
  useEffect(() => {
    logger.debug(`Component mounted`, componentName);
    return () => {
      logger.debug(`Component unmounted`, componentName);
    };
  }, [componentName]);
};