import { useContext } from 'react';
import { ModelContext } from '@/contexts/ModelContext';
import logger from '@/lib/logger';

/**
 * Custom hook that provides access to the ModelContext
 * @param {string} componentName - Name of the component using this hook (for logging)
 * @returns {Object} The ModelContext value with model state and methods
 */
export const useModel = (componentName = 'UnnamedComponent') => {
    try {
        const context = useContext(ModelContext);
        
        if (context === undefined) {
            const error = 'useModel must be used within a ModelProvider';
            logger.error(error, componentName);
            throw new Error(error);
        }
        
        logger.debug(`${componentName} using ModelContext`, 'useModel');
        return context;
    } catch (error) {
        logger.error(`useModel error in ${componentName}`, 'useModel', { error: error.message });
        throw error;
    }
};

export default useModel;