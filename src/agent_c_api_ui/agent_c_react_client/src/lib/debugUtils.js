/**
 * Debug utilities for troubleshooting UI issues
 * Provides advanced debugging capabilities for the React UI
 */

import logger from './logger';

/**
 * Logs component rendering with detailed props and state information
 * @param {string} componentName - Name of the component being rendered
 * @param {Object} props - Component props
 * @param {Object|null} state - Component state (if available)
 * @param {string} phase - Rendering lifecycle phase
 */
export function logComponentRender(componentName, props, state, phase = 'render') {
  // Clean up props by removing circular references or complex objects
  const cleanProps = {};
  if (props) {
    Object.keys(props).forEach(key => {
      // Skip functions and complex objects, just log their presence
      if (typeof props[key] === 'function') {
        cleanProps[key] = '[Function]';
      } else if (typeof props[key] === 'object' && props[key] !== null) {
        if (Array.isArray(props[key])) {
          cleanProps[key] = `Array(${props[key].length})`;
        } else {
          cleanProps[key] = `Object: ${Object.keys(props[key]).join(', ')}`;
        }
      } else {
        cleanProps[key] = props[key];
      }
    });
  }
  
  // Clean up state similarly
  const cleanState = {};
  if (state) {
    Object.keys(state).forEach(key => {
      if (typeof state[key] === 'function') {
        cleanState[key] = '[Function]';
      } else if (typeof state[key] === 'object' && state[key] !== null) {
        if (Array.isArray(state[key])) {
          cleanState[key] = `Array(${state[key].length})`;
        } else {
          cleanState[key] = `Object: ${Object.keys(state[key]).join(', ')}`;
        }
      } else {
        cleanState[key] = state[key];
      }
    });
  }
  
  console.log(`🧩 ${componentName} ${phase}:`, {
    props: cleanProps,
    state: state ? cleanState : 'N/A',
    timestamp: new Date().toISOString()
  });
  
  // Also log to the logger for persistence
  logger.debug(`Component ${phase}`, componentName, {
    propsKeys: Object.keys(props || {}),
    stateKeys: state ? Object.keys(state) : []
  });
}

/**
 * Logs context access and updates
 * @param {string} contextName - Name of the context being accessed
 * @param {string} componentName - Component accessing the context
 * @param {Object} contextValue - The context value
 */
export function logContextAccess(contextName, componentName, contextValue) {
  // Extract key properties from the context value, avoiding circular references
  const contextSummary = {};
  
  if (contextValue) {
    // Common properties to check in contexts
    const keyProps = ['isInitialized', 'isReady', 'isLoading', 'error', 'sessionId'];
    keyProps.forEach(prop => {
      if (prop in contextValue) {
        contextSummary[prop] = contextValue[prop];
      }
    });
    
    // Add additional properties based on context type
    if (contextName === 'SessionContext') {
      contextSummary.settingsVersion = contextValue.settingsVersion;
      contextSummary.hasActiveTools = Array.isArray(contextValue.activeTools) && contextValue.activeTools.length > 0;
    } else if (contextName === 'ModelContext') {
      contextSummary.modelName = contextValue.modelName;
      contextSummary.selectedModel = contextValue.selectedModel;
    } else if (contextName === 'AuthContext') {
      contextSummary.isAuthenticated = contextValue.isAuthenticated;
      contextSummary.isInitializing = contextValue.isInitializing;
    }
  }
  
  logger.debug(`${contextName} accessed`, componentName, contextSummary);
}

/**
 * Force render the chat interface if it should be visible but isn't
 * This function attempts to force a render of the chat interface by
 * directly manipulating the DOM
 */
export function forceRenderChat() {
  const sessionId = localStorage.getItem('sessionId');
  const context = window.__CONTEXT_DIAGNOSTIC?.contexts?.SessionContext;
  const isInitialized = context?.initialized;
  
  // Check if conditions for rendering are met
  if (!sessionId || !isInitialized) {
    console.error('❌ Cannot force render chat - conditions not met');
    console.log('sessionId exists:', !!sessionId);
    console.log('session initialized:', isInitialized);
    return {
      success: false,
      reason: 'Rendering conditions not met',
      sessionId: !!sessionId,
      isInitialized
    };
  }
  
  // Check if ChatInterface component already exists
  const chatInterface = document.querySelector('.chat-interface-card');
  if (chatInterface) {
    console.log('⚠️ Chat interface already exists in DOM');
    console.log('Checking visibility...');
    
    // Check if it's hidden by CSS
    const style = window.getComputedStyle(chatInterface);
    if (style.display === 'none' || style.visibility === 'hidden') {
      console.log('🔍 Chat interface exists but is hidden by CSS');
      // Try to make it visible
      chatInterface.style.display = 'flex';
      chatInterface.style.visibility = 'visible';
      console.log('✅ Attempted to force visibility');
      return {
        success: true,
        action: 'Made existing component visible',
        element: chatInterface
      };
    }
    
    // Check parent visibility
    let parent = chatInterface.parentElement;
    let depth = 0;
    while (parent && depth < 5) {
      const parentStyle = window.getComputedStyle(parent);
      if (parentStyle.display === 'none' || parentStyle.visibility === 'hidden') {
        console.log(`🔍 Found hidden parent at depth ${depth}:`, parent);
        // Try to make it visible
        parent.style.display = 'flex';
        parent.style.visibility = 'visible';
        console.log('✅ Attempted to force parent visibility');
        return {
          success: true,
          action: 'Made parent element visible',
          element: parent
        };
      }
      parent = parent.parentElement;
      depth++;
    }
    
    return {
      success: true,
      action: 'Component already visible',
      element: chatInterface
    };
  }
  
  console.log('⚠️ Chat interface not found in DOM despite conditions being met');
  console.log('Attempting to force re-render...');
  
  // Try to force a re-render by toggling a state
  if (window.__FORCE_RENDER) {
    window.__FORCE_RENDER();
    console.log('✅ Triggered force re-render via hook');
    return {
      success: true,
      action: 'Triggered force re-render'
    };
  }
  
  // If all else fails, try refreshing the page
  console.log('⚠️ Could not force render chat interface');
  console.log('Recommend refreshing the page or checking for errors');
  
  return {
    success: false,
    reason: 'Could not force render',
    suggestion: 'Refresh the page or check console for errors'
  };
}

/**
 * Check the ChatPage component's rendering conditions
 */
export function inspectChatPageRendering() {
  const sessionId = localStorage.getItem('sessionId');
  const sessionContext = window.__CONTEXT_DIAGNOSTIC?.contexts?.SessionContext;
  const isInitialized = sessionContext?.initialized;
  
  // Key rendering condition: sessionId && isInitialized
  const shouldRender = !!sessionId && isInitialized;
  const didRender = !!document.querySelector('.chat-interface-card');
  
  console.group('🔍 ChatPage Rendering Inspection');
  console.log('Rendering conditions:', {
    sessionId: !!sessionId,
    isInitialized,
    shouldRender,
    didRender
  });
  
  // Check for detailed context state
  if (sessionContext) {
    console.log('SessionContext details:', {
      error: sessionContext.error,
      startTime: sessionContext.startTime ? new Date(sessionContext.startTime).toISOString() : 'N/A',
      completeTime: sessionContext.completeTime ? new Date(sessionContext.completeTime).toISOString() : 'N/A',
      duration: sessionContext.duration,
      data: sessionContext.data
    });
  }
  
  // Check if conditions met but no rendering
  if (shouldRender && !didRender) {
    console.warn('⚠️ Conditions for rendering are met, but chat interface is not in DOM!');
    console.log('This could be caused by:');
    console.log('1. Error in ChatInterface or its children');
    console.log('2. Error boundary catching an error silently');
    console.log('3. Issue with context data not propagating correctly');
    console.log('4. Conditional logic in ChatPage preventing rendering');
  }
  
  console.groupEnd();
  
  return {
    shouldRender,
    didRender,
    sessionId: !!sessionId,
    isInitialized,
    sessionContext: sessionContext ? {
      error: sessionContext.error,
      hasData: !!sessionContext.data,
      initialized: sessionContext.initialized
    } : null
  };
}

/**
 * Add these utilities to the global window object for console access
 */
if (typeof window !== 'undefined') {
  window.forceRenderChat = forceRenderChat;
  window.inspectChatPageRendering = inspectChatPageRendering;
  
  // Additional helper to get session context data
  window.getSessionContextData = () => {
    const context = window.__CONTEXT_DIAGNOSTIC?.contexts?.SessionContext;
    if (!context) {
      console.warn('Session context not found');
      return null;
    }
    
    console.log('Session context data:', context.data);
    return context.data;
  };
}