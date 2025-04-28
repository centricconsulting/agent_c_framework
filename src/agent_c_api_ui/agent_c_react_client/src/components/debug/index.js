/**
 * Debug Components
 * 
 * This file exports debug components that are only used in development mode
 * to provide visibility into application state and behavior.
 */

import { lazy } from 'react';

// Check if we're in development mode
const isDevelopment = process.env.NODE_ENV === 'development';

// For production, export no-op components that render nothing
if (!isDevelopment) {
  const EmptyComponent = () => null;
  
  export const InitializationDebugPanel = EmptyComponent;
  export default {
    InitializationDebugPanel: EmptyComponent
  };
} else {
  // In development, lazily load the actual debug components
  export const InitializationDebugPanel = lazy(() => import('./InitializationDebugPanel'));
  
  export default {
    InitializationDebugPanel
  };
}