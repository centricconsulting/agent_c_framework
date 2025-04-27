/**
 * DOM Visibility Utilities
 * 
 * Tools for checking if elements are not just in the DOM but actually visible,
 * and diagnosing parent element visibility issues.
 */

import logger from './logger';

/**
 * Check if an element is visible in the DOM
 * @param {Element|string} element - The element or selector to check
 * @param {Object} options - Options for the check
 * @param {boolean} options.logResults - Whether to log the results
 * @param {boolean} options.traceParents - Whether to trace parent elements
 * @param {string} options.context - Context for logging
 * @returns {Object} Visibility information
 */
export const checkElementVisibility = (element, options = {}) => {
  const {
    logResults = false,
    traceParents = false,
    context = 'dom-visibility'
  } = options;
  
  // Get the element if a selector was provided
  let targetElement = element;
  if (typeof element === 'string') {
    targetElement = document.querySelector(element);
  }
  
  // Check if the element exists
  if (!targetElement) {
    const result = {
      exists: false,
      visible: false,
      selector: typeof element === 'string' ? element : 'ElementObject',
      reason: 'Element not found in DOM'
    };
    
    if (logResults) {
      logger.debug(`Element visibility check: not in DOM`, context, result);
    }
    
    return result;
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
  
  // Collect basic result
  const result = {
    exists: true,
    visible: isVisible,
    selector: typeof element === 'string' ? element : targetElement.tagName,
    id: targetElement.id || null,
    className: targetElement.className || null,
    reason: reason,
    styles: {
      display: style.display,
      visibility: style.visibility,
      opacity: style.opacity,
      width: targetElement.offsetWidth,
      height: targetElement.offsetHeight,
      position: style.position,
      overflow: style.overflow
    }
  };
  
  // Check parent elements if requested
  if (traceParents && !isVisible) {
    result.parents = [];
    let parent = targetElement.parentElement;
    let depth = 0;
    const maxDepth = 10; // Prevent infinite loops
    
    while (parent && depth < maxDepth) {
      const parentStyle = window.getComputedStyle(parent);
      const parentInfo = {
        tag: parent.tagName,
        id: parent.id || null,
        className: parent.className || null,
        styles: {
          display: parentStyle.display,
          visibility: parentStyle.visibility,
          opacity: parentStyle.opacity,
          width: parent.offsetWidth,
          height: parent.offsetHeight,
          overflow: parentStyle.overflow,
          position: parentStyle.position
        },
        isHidden: (
          parentStyle.display === 'none' || 
          parentStyle.visibility === 'hidden' || 
          parseFloat(parentStyle.opacity) === 0 ||
          parent.offsetWidth === 0 || 
          parent.offsetHeight === 0
        )
      };
      
      result.parents.push(parentInfo);
      
      // If we found a hidden parent, mark it as the blocking element
      if (parentInfo.isHidden && !result.blockingElement) {
        result.blockingElement = {
          depth: depth + 1,
          tag: parent.tagName,
          id: parent.id,
          className: parent.className
        };
      }
      
      parent = parent.parentElement;
      depth++;
    }
  }
  
  if (logResults) {
    logger.debug(`Element visibility check: ${isVisible ? 'visible' : 'hidden'}`, context, result);
  }
  
  return result;
};

/**
 * Check chat interface visibility and diagnose any issues
 * @param {Object} options - Options for the check
 * @returns {Object} Visibility diagnostic information
 */
export const checkChatInterfaceVisibility = (options = {}) => {
  const chatPageVisibility = checkElementVisibility('[data-chat-page]', {
    ...options,
    context: 'chat-page'
  });
  
  const chatInterfaceVisibility = checkElementVisibility('[data-testid="chat-interface"]', {
    ...options,
    context: 'chat-interface'
  });
  
  const chatInterfaceCardVisibility = checkElementVisibility('.chat-interface-card', {
    ...options,
    context: 'chat-interface-card'
  });
  
  const result = {
    chatPage: chatPageVisibility,
    chatInterface: chatInterfaceVisibility,
    chatInterfaceCard: chatInterfaceCardVisibility,
    allComponentsExist: chatPageVisibility.exists && 
                       chatInterfaceVisibility.exists && 
                       chatInterfaceCardVisibility.exists,
    allComponentsVisible: chatPageVisibility.visible && 
                         chatInterfaceVisibility.visible && 
                         chatInterfaceCardVisibility.visible,
    diagnosis: determineVisibilityIssue(chatPageVisibility, chatInterfaceVisibility, chatInterfaceCardVisibility)
  };
  
  if (options.logResults) {
    logger.debug('Chat interface visibility check', 'dom-visibility', {
      diagnosis: result.diagnosis,
      allExist: result.allComponentsExist,
      allVisible: result.allComponentsVisible
    });
  }
  
  return result;
};

/**
 * Determine the specific issue causing visibility problems
 * @param {Object} chatPageVisibility - Chat page visibility result
 * @param {Object} chatInterfaceVisibility - Chat interface visibility result
 * @param {Object} chatInterfaceCardVisibility - Chat interface card visibility result
 * @returns {string} Diagnosis message
 */
function determineVisibilityIssue(chatPageVisibility, chatInterfaceVisibility, chatInterfaceCardVisibility) {
  if (!chatPageVisibility.exists) {
    return 'Chat page element is not in the DOM - routing issue or page not loaded';
  }
  
  if (!chatInterfaceVisibility.exists) {
    return 'Chat interface element is not in the DOM - likely a rendering condition in ChatPage is preventing it';
  }
  
  if (!chatInterfaceCardVisibility.exists) {
    return 'Chat interface card element is not in the DOM - ChatInterface component may have errors or conditional rendering issues';
  }
  
  if (!chatPageVisibility.visible) {
    return `Chat page element exists but is hidden (${chatPageVisibility.reason}) - check CSS or parent elements`;
  }
  
  if (!chatInterfaceVisibility.visible) {
    if (chatInterfaceVisibility.blockingElement) {
      return `Chat interface is hidden by parent element: ${chatInterfaceVisibility.blockingElement.tag}${chatInterfaceVisibility.blockingElement.id ? ' #' + chatInterfaceVisibility.blockingElement.id : ''}`;
    }
    return `Chat interface element exists but is hidden (${chatInterfaceVisibility.reason})`;
  }
  
  if (!chatInterfaceCardVisibility.visible) {
    if (chatInterfaceCardVisibility.blockingElement) {
      return `Chat interface card is hidden by parent element: ${chatInterfaceCardVisibility.blockingElement.tag}${chatInterfaceCardVisibility.blockingElement.id ? ' #' + chatInterfaceCardVisibility.blockingElement.id : ''}`;
    }
    return `Chat interface card exists but is hidden (${chatInterfaceCardVisibility.reason})`;
  }
  
  return 'All elements exist and appear to be visible - if interface is still not showing, check z-index issues or other CSS that might be affecting its display';
}

/**
 * Register global utility functions for debugging
 * NOTE: This function is no longer called automatically to prevent performance issues.
 * It should be called manually only when needed for debugging specific issues.
 */
export const registerDOMVisibilityUtils = () => {
  if (typeof window !== 'undefined') {
    // Only add these utilities if in development mode
    if (process.env.NODE_ENV === 'development') {
      // Check if an element is visible
      window.checkElementVisibility = (selector) => {
        return checkElementVisibility(selector, { logResults: true, traceParents: true });
      };
      
      // Check chat interface visibility
      window.checkChatInterfaceVisibility = () => {
        const result = checkChatInterfaceVisibility({ logResults: true });
        console.group('Chat Interface Visibility Check');
        console.log('Diagnosis:', result.diagnosis);
        console.log('All components exist:', result.allComponentsExist);
        console.log('All components visible:', result.allComponentsVisible);
        console.log('Detailed results:', result);
        console.groupEnd();
        return result;
      };
      
      // Fix attempt - force DOM update
      window.refreshChatInterface = () => {
        const chatPage = document.querySelector('[data-chat-page]');
        if (!chatPage) {
          console.error('Chat page element not found, cannot refresh');
          return false;
        }
        
        // Hide and show to force a repaint
        const originalDisplay = chatPage.style.display;
        chatPage.style.display = 'none';
        
        // Force a reflow
        void chatPage.offsetHeight;
        
        // Show again after a small delay
        setTimeout(() => {
          chatPage.style.display = originalDisplay || 'block';
          console.log('Chat page display refreshed');
          
          // Check visibility after refresh
          setTimeout(() => {
            window.checkChatInterfaceVisibility();
          }, 100);
        }, 50);
        
        return true;
      };
    }
  }
};

// No longer auto-registering global utility functions to prevent performance issues
// registerDOMVisibilityUtils();