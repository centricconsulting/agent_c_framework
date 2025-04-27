import { useContext } from 'react';
import { ModelContext } from '@/contexts/ModelContext';
import logger from '@/lib/logger';

export function useModel(componentName = 'UnnamedComponent') {
    const context = useContext(ModelContext);
    
    if (context === undefined) {
        const error = new Error('useModel must be used within a ModelProvider');
        logger.error(`${componentName}: ${error.message}`, 'useModel');
        throw error;
    }
    
    logger.debug(`${componentName} using ModelContext`, 'useModel');
    
    return context;
}