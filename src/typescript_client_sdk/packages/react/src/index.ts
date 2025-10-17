/**
 * Agent C Realtime SDK - React Bindings
 * 
 * This package provides React hooks and components for the Agent C Realtime SDK.
 * It's a thin layer over the core SDK that handles React lifecycle and state management.
 */

// Export hooks
export * from './hooks';

// Export providers and context
export * from './providers';

// Export types
export * from './types';

// Export utilities
export { AgentStorage } from './utils/agentStorage';
export { hasFileAttachments, countImages, getMessageDisplayText } from './utils/messageHelpers';

// Note: Components will be added later
// export * from './components';