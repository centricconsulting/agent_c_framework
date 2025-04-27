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
        
        // Enhanced logging to debug modelConfigs issue
        logger.debug(`${componentName} using ModelContext`, 'useModel', {
            hasModelConfigs: Array.isArray(context.modelConfigs),
            modelConfigsLength: Array.isArray(context.modelConfigs) ? context.modelConfigs.length : 0,
            modelConfigsType: typeof context.modelConfigs,
            sampleModelConfig: Array.isArray(context.modelConfigs) && context.modelConfigs.length > 0 ? {
                id: context.modelConfigs[0].id,
                backend: context.modelConfigs[0].backend,
                hasParams: !!context.modelConfigs[0].parameters
            } : null
        });
        
        // CRITICAL: Make sure modelConfigs is always an array
        if (!Array.isArray(context.modelConfigs)) {
            logger.warn(`useModel: modelConfigs is not an array in ${componentName}`, 'useModel', {
                actualType: typeof context.modelConfigs,
                value: context.modelConfigs
            });
            // Fix the context object by ensuring modelConfigs is at least an empty array
            context.modelConfigs = [];
        }
        
        return context;
    } catch (error) {
        logger.error(`useModel error in ${componentName}`, 'useModel', { error: error.message });
        throw error;
    }
};

export default useModel;