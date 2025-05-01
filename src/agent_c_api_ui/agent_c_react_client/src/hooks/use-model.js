import { useContext } from 'react';
import { ModelContext } from '../contexts/ModelContext';

/**
 * Hook for accessing the ModelContext
 * 
 * @returns {Object} The ModelContext value
 */
export function useModel() {
  const context = useContext(ModelContext);
  
  if (!context) {
    console.error('useModel must be used within a ModelProvider');
    // Return a fallback object rather than throwing an error
    return {
      isInitialized: false,
      isLoading: false,
      isReady: false,
      error: 'ModelContext not available',
      modelConfigs: [],
      selectedModel: null,
      modelName: '',
      modelParameters: {},
      fetchModels: () => console.error('ModelContext not available'),
      changeModel: () => console.error('ModelContext not available'),
      updateModelParameters: () => console.error('ModelContext not available'),
      clearError: () => console.error('ModelContext not available'),
      initialize: () => console.error('ModelContext not available')
    };
  }
  
  return context;
}

export default useModel;