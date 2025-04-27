/**
 * DOM Visibility Utilities
 * 
 * Tools for checking if elements are not just in the DOM but actually visible,
 * and diagnosing parent element visibility issues.
 * 
 * PERFORMANCE OPTIMIZED VERSION
 */

// Control debug utilities with environment
const IS_DEV = process.env.NODE_ENV === 'development';
const ENABLE_DEBUG = IS_DEV && false; // Default to disabled even in development
const ENABLE_LOGGING = IS_DEV && false; // Control logging independently

/**
 * Check if an element is visible in the DOM - optimized version
 * @param {Element|string} element - The element or selector to check
 * @param {Object} options - Options for the check
 * @returns {Object} Visibility information
 */
export const checkElementVisibility = (element, options = {}) => {
  // In production, perform minimal operations and never log
  if (!IS_DEV) {
    // Simplified implementation for production
    let targetElement = element;
    if (typeof element === 'string') {
      targetElement = document.querySelector(element);
    }
    
    // Basic visibility check with minimal operations
    if (!targetElement) {
      return { exists: false, visible: false, reason: 'Not found' };
    }
    
    const style = window.getComputedStyle(targetElement);
    const isVisible = style.display !== 'none' && 
                      style.visibility !== 'hidden' && 
                      parseFloat(style.opacity) > 0 && 
                      targetElement.offsetWidth > 0 && 
                      targetElement.offsetHeight > 0;
    
    return { exists: true, visible: isVisible, reason: isVisible ? null : 'Element hidden' };
  }
  
  // More detailed implementation for development if debug is enabled
  if (ENABLE_DEBUG) {
    // Get the element if a selector was provided
    let targetElement = element;
    if (typeof element === 'string') {
      targetElement = document.querySelector(element);
    }
    
    // Check if the element exists
    if (!targetElement) {
      return {
        exists: false,
        visible: false,
        selector: typeof element === 'string' ? element : 'ElementObject',
        reason: 'Element not found in DOM'
      };
    }
    
    // Get computed style
    const style = window.getComputedStyle(targetElement);
    
    // Check various properties that affect visibility
    const isDisplayNone = style.display === 'none';
    const isVisibilityHidden = style.visibility === 'hidden';
    const isOpacityZero = parseFloat(style.opacity) === 0;
    const hasZeroSize = targetElement.offsetWidth === 0 || targetElement.offsetHeight === 0;
    
    const isVisible = !isDisplayNone && 
                    !isVisibilityHidden && 
                    !isOpacityZero && 
                    !hasZeroSize;
    
    // Determine reason if not visible
    let reason = isVisible ? null : 'Element is hidden';
    if (!isVisible) {
      if (isDisplayNone) reason = 'display: none';
      else if (isVisibilityHidden) reason = 'visibility: hidden';
      else if (isOpacityZero) reason = 'opacity: 0';
      else if (hasZeroSize) reason = 'zero width/height';
    }
    
    return {
      exists: true,
      visible: isVisible,
      selector: typeof element === 'string' ? element : targetElement.tagName,
      id: targetElement.id || null,
      className: targetElement.className || null,
      reason: reason
    };
  }
  
  // Simplified fallback for development but not in debug mode
  let targetElement = element;
  if (typeof element === 'string') {
    targetElement = document.querySelector(element);
  }
  
  if (!targetElement) {
    return { exists: false, visible: false, reason: 'Not found' };
  }
  
  const style = window.getComputedStyle(targetElement);
  const isVisible = style.display !== 'none' && 
                    style.visibility !== 'hidden' && 
                    parseFloat(style.opacity) > 0 && 
                    targetElement.offsetWidth > 0 && 
                    targetElement.offsetHeight > 0;
  
  return { exists: true, visible: isVisible, reason: isVisible ? null : 'Element hidden' };
};

/**
 * Check chat interface visibility - optimized version
 * @returns {Object} Basic visibility information
 */
export const checkChatInterfaceVisibility = () => {
  // In production, minimal implementation
  if (!IS_DEV || !ENABLE_DEBUG) {
    const chatInterface = document.querySelector('.chat-interface-card');
    return {
      exists: !!chatInterface,
      visible: !!chatInterface && chatInterface.offsetWidth > 0 && chatInterface.offsetHeight > 0,
      allComponentsExist: !!chatInterface,
      allComponentsVisible: !!chatInterface && chatInterface.offsetWidth > 0 && chatInterface.offsetHeight > 0
    };
  }
  
  // Only in development with debug enabled
  if (ENABLE_DEBUG) {
    const chatPageVisibility = checkElementVisibility('[data-chat-page]');
    const chatInterfaceVisibility = checkElementVisibility('[data-testid="chat-interface"]');
    const chatInterfaceCardVisibility = checkElementVisibility('.chat-interface-card');
    
    return {
      chatPage: chatPageVisibility,
      chatInterface: chatInterfaceVisibility,
      chatInterfaceCard: chatInterfaceCardVisibility,
      allComponentsExist: chatPageVisibility.exists && 
                         chatInterfaceVisibility.exists && 
                         chatInterfaceCardVisibility.exists,
      allComponentsVisible: chatPageVisibility.visible && 
                           chatInterfaceVisibility.visible && 
                           chatInterfaceCardVisibility.visible
    };
  }
  
  // Fallback for development without debug
  const chatInterface = document.querySelector('.chat-interface-card');
  return {
    exists: !!chatInterface,
    visible: !!chatInterface && chatInterface.offsetWidth > 0 && chatInterface.offsetHeight > 0
  };
};

/**
 * Register global utility functions for debugging
 * This function MUST be explicitly called - it will never auto-register.
 * It will only work in development mode with debug enabled.
 */
export const registerDOMVisibilityUtils = () => {
  // Only register in development AND when debug is explicitly enabled
  if (IS_DEV && ENABLE_DEBUG && typeof window !== 'undefined') {
    console.warn('DOM visibility utilities registered for debugging. DO NOT USE IN PRODUCTION.');
    
    // Add developer debug functions to window
    window.__debugDOMVisibility = {
      checkElementVisibility: (selector) => checkElementVisibility(selector, { traceParents: true }),
      checkChatInterfaceVisibility: () => checkChatInterfaceVisibility()
    };
  }
};