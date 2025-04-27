/**
 * Diagnostics utilities for Agent C React Client
 *
 * This module provides tools for diagnosing and debugging various aspects
 * of the application, particularly related to rendering and context issues.
 */

import logger from './logger';
import { safeInspect, safeStringify } from './safeSerializer';

// Global debug mode toggle
export const DEBUG_MODE = (typeof window !== 'undefined') && 
  (localStorage.getItem('debug_mode') === 'true');

// Track context initialization process - using WeakMap to prevent memory leaks
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
  
  // Log context updates for debugging only when DEBUG_MODE is enabled
  if (DEBUG_MODE) {
    logger.debug(`Context ${contextName} ${status}`, 'diagnostic', {
      contextName,
      status,
      contextStatus: context.status,
      ...(data ? { data: safeInspect(data) } : {})
    });
    
    // Attach to window for debugging, but with limited data
    if (typeof window !== 'undefined') {
      // Use a safe copy to prevent circular references
      if (!window.contextInitializationStatus) {
        window.contextInitializationStatus = {};
      }
      window.contextInitializationStatus[contextName] = {
        status: context.status,
        startTime: context.startTime,
        complete: context.complete,
        updateCount: context.updates.length,
        errorCount: context.errors.length
      };
    }
  }
};

/**
 * Signal that a context has completed initialization
 * @param {boolean} success - Whether initialization succeeded
 * @param {Object} data - Additional data about initialization
 */
export const completeContextInitialization = (success = true, data = {}) => {
  // Only create global objects when DEBUG_MODE is enabled
  if (DEBUG_MODE && typeof window !== 'undefined') {
    // Create a global status object if it doesn't exist
    if (!window.appInitialization) {
      window.appInitialization = {
        initialized: false,
        startTime: Date.now(),
        completeTime: null,
        error: null
      };
    }
    
    // Update the app initialization status with limited data
    window.appInitialization.initialized = success;
    window.appInitialization.completeTime = Date.now();
    window.appInitialization.duration = 
      window.appInitialization.completeTime - window.appInitialization.startTime;
      
    if (!success && data.error) {
      window.appInitialization.error = safeStringify(data.error);
    }
    
    // Only store essential data without circular references
    const safeData = data ? safeInspect(data) : {};
    window.appInitialization.safeData = safeData;
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
  // Only log when DEBUG_MODE is enabled
  if (DEBUG_MODE) {
    logger.debug(`Component ${componentName} ${status}`, 'diagnostic', {
      componentName,
      status,
      ...(Object.keys(data).length > 0 ? { data: safeInspect(data) } : {})
    });
  
    // Attach to window for debugging only when DEBUG_MODE is enabled
    if (typeof window !== 'undefined') {
      if (!window.componentRendering) {
        window.componentRendering = {};
      }
      
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
          // Only store essential data without circular references
          component.data = { renderCount: component.renders };
          break;
          
        case 'error':
          component.errors.push({
            time: Date.now(),
            message: data.message || 'Unknown error'
          });
          component.data = { lastError: safeStringify(data) };
          break;
          
        case 'unmounted':
          component.unmountTime = Date.now();
          component.data = { unmountTime: component.unmountTime };
          break;
      }
    }
  }
};

/**
 * Specifically track ChatInterface rendering to diagnose issues
 * @param {string} action - What happening with the ChatInterface
 * @param {Object} data - Additional data about the action
 */
export const trackChatInterfaceRendering = (action, data = {}) => {
  // Only log when DEBUG_MODE is enabled
  if (DEBUG_MODE) {
    logger.debug(`ChatInterface: ${action}`, 'diagnostic', {
      action,
      timestamp: Date.now(),
      ...(Object.keys(data).length > 0 ? { data: safeInspect(data) } : {})
    });
  }
  
  // Create a global chat interface tracking object if it doesn't exist and DEBUG_MODE is enabled
  if (DEBUG_MODE && typeof window !== 'undefined') {
    if (!window.chatInterfaceTracking) {
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
        
        if (!isVisible && chatInterface) {
          // Check if the parent elements are visible, but limit depth
          const parents = [];
          let element = chatInterface;
          let depth = 0;
          
          while (element && element !== document.body && depth < 5) {
            const style = window.getComputedStyle(element);
            const isHidden = style.display === 'none' || style.visibility === 'hidden' || style.opacity === '0';
            
            parents.push({
              tag: element.tagName,
              id: element.id,
              className: element.className.substring(0, 50), // Limit class name length
              isHidden,
              display: style.display,
              visibility: style.visibility
            });
            
            element = element.parentElement;
            depth++;
          }
          
          console.table(parents);
        }
        
        return isVisible;
      };
      
      // Simplified utility to inspect the rendering path
      window.inspectRenderingPath = () => {
        console.log('Rendering path analysis:');
        
        // Check for critical render conditions
        const sessionContext = window.contextInitializationStatus?.SessionContext;
        const authContext = window.contextInitializationStatus?.AuthContext;
        
        console.log('\nKey rendering conditions:');
        console.log(`- Auth initialized: ${authContext?.status === 'complete'}`);
        console.log(`- Session initialized: ${sessionContext?.status === 'complete'}`);
        
        // Session ID details
        const storedSessionId = localStorage.getItem('ui_session_id');
        console.log('\nSession ID details:');
        console.log(`- localStorage session ID: ${storedSessionId || 'not found'}`);
        console.log(`- Session ID type: ${typeof storedSessionId}`);
      };
      
      // Simplified utility to force a render of the chat interface
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
    }
    
    // Add the event to the tracking object (limited to last 50 events)
    const events = window.chatInterfaceTracking.events;
    events.push({
      action,
      timestamp: Date.now(),
      // Store only safe data with limited size
      ...(Object.keys(data).length > 0 ? { data: safeInspect(data) } : {})
    });
    
    // Keep only the last 50 events
    if (events.length > 50) {
      window.chatInterfaceTracking.events = events.slice(events.length - 50);
    }
    
    window.chatInterfaceTracking.lastAction = action;
    
    if (action === 'rendered') {
      window.chatInterfaceTracking.renderCount++;
    }
    
    if (data.isVisible !== undefined) {
      window.chatInterfaceTracking.isVisible = data.isVisible;
    }
  }
};