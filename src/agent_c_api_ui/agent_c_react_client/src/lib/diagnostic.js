/**
 * Diagnostics utilities for Agent C React Client
 *
 * This module provides tools for diagnosing and debugging various aspects
 * of the application, particularly related to rendering and context issues.
 */

import logger from './logger';

// Track context initialization process
const contextInitializationStatus = {};

/**
 * Track the initialization status of a context
 * @param {string} contextName - Name of the context
 * @param {string} status - Status of initialization (start, update, error, complete)
 * @param {Object} data - Additional data about the initialization status
 */
export const trackContextInitialization = (contextName, status, data = {}) => {
  // Create context entry if it doesn't exist
  if (!contextInitializationStatus[contextName]) {
    contextInitializationStatus[contextName] = {
      startTime: Date.now(),
      status: 'initializing',
      updates: [],
      errors: [],
      complete: false,
      data: {}
    };
  }
  
  const context = contextInitializationStatus[contextName];
  
  // Update status based on status parameter
  switch (status) {
    case 'start':
      context.startTime = Date.now();
      context.status = 'initializing';
      context.data = { ...context.data, ...data };
      break;
      
    case 'update':
      context.updates.push({
        time: Date.now(),
        data
      });
      context.data = { ...context.data, ...data };
      break;
      
    case 'error':
      context.errors.push({
        time: Date.now(),
        data
      });
      context.status = 'error';
      context.data = { ...context.data, lastError: data };
      break;
      
    case 'complete':
      context.completeTime = Date.now();
      context.duration = context.completeTime - context.startTime;
      context.status = data?.success === false ? 'error' : 'complete';
      context.complete = true;
      context.data = { ...context.data, ...data };
      break;
      
    default:
      console.warn(`Unknown status '${status}' for context tracking`);
  }
  
  // Log context updates for debugging
  logger.debug(`Context ${contextName} ${status}`, 'diagnostic', {
    contextName,
    status,
    data,
    contextStatus: context.status
  });
  
  // Attach to window for debugging
  if (typeof window !== 'undefined') {
    window.contextInitializationStatus = contextInitializationStatus;
  }
};

/**
 * Signal that a context has completed initialization
 * @param {boolean} success - Whether initialization succeeded
 * @param {Object} data - Additional data about initialization
 */
export const completeContextInitialization = (success = true, data = {}) => {
  // Create a global status object if it doesn't exist
  if (typeof window !== 'undefined' && !window.appInitialization) {
    window.appInitialization = {
      initialized: false,
      contexts: contextInitializationStatus,
      startTime: Date.now(),
      completeTime: null,
      error: null
    };
  }
  
  // Update the app initialization status
  if (typeof window !== 'undefined') {
    window.appInitialization.initialized = success;
    window.appInitialization.completeTime = Date.now();
    window.appInitialization.duration = 
      window.appInitialization.completeTime - window.appInitialization.startTime;
      
    if (!success && data.error) {
      window.appInitialization.error = data.error;
    }
    
    window.appInitialization.data = { ...window.appInitialization.data, ...data };
  }
  
  logger.info(`App initialization ${success ? 'completed' : 'failed'}`, 'diagnostic', {
    success,
    ...data
  });
};

/**
 * Get the initialization status of all contexts
 * @returns {Object} Context initialization status
 */
export const getContextInitializationStatus = () => {
  return { ...contextInitializationStatus };
};

/**
 * Track the rendering of a component
 * @param {string} componentName - Name of the component
 * @param {string} status - Status of rendering (start, rendered, error, unmounted)
 * @param {Object} data - Additional data about the rendering
 */
export const trackComponentRendering = (componentName, status, data = {}) => {
  logger.debug(`Component ${componentName} ${status}`, 'diagnostic', {
    componentName,
    status,
    data
  });
  
  // Attach to window for debugging
  if (typeof window !== 'undefined' && !window.componentRendering) {
    window.componentRendering = {};
  }
  
  if (typeof window !== 'undefined') {
    if (!window.componentRendering[componentName]) {
      window.componentRendering[componentName] = {
        renders: 0,
        lastRender: null,
        errors: [],
        data: {}
      };
    }
    
    const component = window.componentRendering[componentName];
    
    switch (status) {
      case 'start':
        component.renderStartTime = Date.now();
        break;
        
      case 'rendered':
        component.renders++;
        component.lastRender = Date.now();
        component.renderDuration = component.lastRender - (component.renderStartTime || component.lastRender);
        component.data = { ...component.data, ...data };
        break;
        
      case 'error':
        component.errors.push({
          time: Date.now(),
          data
        });
        component.data = { ...component.data, lastError: data };
        break;
        
      case 'unmounted':
        component.unmountTime = Date.now();
        component.data = { ...component.data, ...data };
        break;
    }
  }
};

/**
 * Specifically track ChatInterface rendering to diagnose issues
 * @param {string} action - What happening with the ChatInterface
 * @param {Object} data - Additional data about the action
 */
export const trackChatInterfaceRendering = (action, data = {}) => {
  logger.debug(`ChatInterface: ${action}`, 'diagnostic', {
    action,
    timestamp: Date.now(),
    ...data
  });
  
  // Create a global chat interface tracking object if it doesn't exist
  if (typeof window !== 'undefined' && !window.chatInterfaceTracking) {
    window.chatInterfaceTracking = {
      events: [],
      lastAction: null,
      renderCount: 0,
      isVisible: null
    };
    
    // Add a utility function to check chat interface visibility
    window.checkChatVisibility = () => {
      const chatInterface = document.querySelector('[data-testid="chat-interface"]');
      const isVisible = !!(chatInterface && 
        chatInterface.offsetWidth > 0 && 
        chatInterface.offsetHeight > 0);
      
      // Get the sessionId attribute from the chat interface
      const sessionIdValue = chatInterface ? chatInterface.getAttribute('data-session-id-value') : null;
      const storedSessionId = localStorage.getItem('ui_session_id');
      
      console.log(`Chat interface is ${isVisible ? 'VISIBLE' : 'NOT VISIBLE'}`);
      console.log(`Session ID from attribute: ${sessionIdValue || 'missing'}`);
      console.log(`Session ID from localStorage: ${storedSessionId || 'missing'}`);
      console.log(`Session IDs match: ${sessionIdValue === storedSessionId}`);
      
      if (!isVisible) {
        // Check if the parent elements are visible
        const parents = [];
        let element = chatInterface;
        
        while (element && element !== document.body) {
          const style = window.getComputedStyle(element);
          const isHidden = style.display === 'none' || style.visibility === 'hidden' || style.opacity === '0';
          
          parents.push({
            tag: element.tagName,
            id: element.id,
            className: element.className,
            isHidden,
            display: style.display,
            visibility: style.visibility,
            opacity: style.opacity,
            width: element.offsetWidth,
            height: element.offsetHeight
          });
          
          element = element.parentElement;
        }
        
        console.table(parents);
      }
      
      return isVisible;
    };
    
    // Utility to inspect the rendering path
    window.inspectRenderingPath = () => {
      console.log('Rendering path analysis:');
      console.log('Context initialization:', window.contextInitializationStatus);
      console.log('Component rendering:', window.componentRendering);
      console.log('Chat interface tracking:', window.chatInterfaceTracking);
      
      // Check for critical render conditions
      const sessionContext = window.contextInitializationStatus?.SessionContext;
      const authContext = window.contextInitializationStatus?.AuthContext;
      
      console.log('\nKey rendering conditions:');
      console.log(`- Auth initialized: ${authContext?.status === 'complete'}`);
      console.log(`- Session initialized: ${sessionContext?.status === 'complete'}`);
      
      // Session ID details
      const storedSessionId = localStorage.getItem('ui_session_id');
      const sessionIdFromContext = sessionContext?.data?.sessionId;
      console.log('\nSession ID details:');
      console.log(`- localStorage session ID: ${storedSessionId || 'not found'}`);
      console.log(`- Context session ID: ${sessionIdFromContext || 'not found'}`);
      console.log(`- Session ID type: ${typeof sessionIdFromContext}`);
      console.log(`- Session ID valid: ${typeof sessionIdFromContext === 'string' && sessionIdFromContext?.length > 0}`);
      console.log(`- Session IDs match: ${storedSessionId === sessionIdFromContext}`);
      
      // Other conditions
      console.log('\nOther conditions:');
      console.log(`- Has session ID in context: ${!!sessionContext?.data?.sessionId}`);
      console.log(`- Session is ready: ${!!sessionContext?.data?.isReady}`);
      
      // Check ChatPage component state
      const chatPageState = window.componentRendering?.ChatPage;
      if (chatPageState) {
        console.log('\nChatPage component state:');
        console.log(`- Last render time: ${new Date(chatPageState.lastRender).toISOString()}`);
        console.log(`- Render count: ${chatPageState.renders}`);
        console.log(`- Session ID in component: ${chatPageState.data?.sessionId || 'unknown'}`);
        console.log(`- Should render interface: ${chatPageState.data?.shouldRenderChatInterface}`);
      }
    };
    
    // Utility to force a render of the chat interface
    window.forceRenderChat = () => {
      const chatPage = document.querySelector('[data-chat-page]');
      if (chatPage) {
        console.log('Forcing chat page re-render...');
        chatPage.style.display = 'none';
        setTimeout(() => {
          chatPage.style.display = '';
          console.log('Chat page display reset, check if interface appears');
        }, 100);
      } else {
        console.log('Chat page element not found');
      }
    };
    
    // Utility to get debug info
    window.getChatInterfaceDebugInfo = () => {
      return window.chatInterfaceTracking;
    };
  }
  
  // Add the event to the tracking object
  if (typeof window !== 'undefined') {
    window.chatInterfaceTracking.events.push({
      action,
      timestamp: Date.now(),
      ...data
    });
    
    window.chatInterfaceTracking.lastAction = action;
    
    if (action === 'rendered') {
      window.chatInterfaceTracking.renderCount++;
    }
    
    if (data.isVisible !== undefined) {
      window.chatInterfaceTracking.isVisible = data.isVisible;
    }
  }
};