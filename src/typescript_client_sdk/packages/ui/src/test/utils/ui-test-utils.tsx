/**
 * UI Component Testing Utilities
 * Helper functions for testing UI components
 */

import React, { ReactElement } from 'react';
import { render, RenderOptions, RenderResult } from '@testing-library/react';
import { userEvent, UserEvent } from '@testing-library/user-event';
import { vi } from 'vitest';

/**
 * Custom render with theme and styling setup
 */
export function renderUI(
  ui: ReactElement,
  options?: RenderOptions
): RenderResult {
  // Add any theme providers or global styling here
  return render(ui, options);
}

/**
 * Create user event instance with default options
 */
export function setupUser(): UserEvent {
  return userEvent.setup({
    delay: null, // Remove delay for faster tests
    pointerEventsCheck: 0 // Skip pointer events check
  });
}

/**
 * Animation and transition test utilities
 */
export const animation = {
  /**
   * Fast-forward all animations and transitions
   */
  fastForward: () => {
    vi.runAllTimers();
  },

  /**
   * Disable animations for testing
   */
  disable: () => {
    const style = document.createElement('style');
    style.innerHTML = `
      *, *::before, *::after {
        animation-duration: 0s !important;
        animation-delay: 0s !important;
        transition-duration: 0s !important;
        transition-delay: 0s !important;
      }
    `;
    document.head.appendChild(style);
    return () => document.head.removeChild(style);
  },

  /**
   * Wait for animation to complete
   */
  waitForAnimation: async (duration: number = 300): Promise<void> => {
    return new Promise(resolve => setTimeout(resolve, duration));
  }
};

/**
 * Style testing utilities
 */
export const styles = {
  /**
   * Get computed styles for an element
   */
  getComputed: (element: HTMLElement): CSSStyleDeclaration => {
    return window.getComputedStyle(element);
  },

  /**
   * Check if element has specific CSS class
   */
  hasClass: (element: HTMLElement, className: string): boolean => {
    return element.classList.contains(className);
  },

  /**
   * Check if element matches media query
   */
  matchesMedia: (query: string): boolean => {
    return window.matchMedia(query).matches;
  },

  /**
   * Set viewport size for responsive testing
   */
  setViewport: (width: number, height: number) => {
    Object.defineProperty(window, 'innerWidth', {
      writable: true,
      configurable: true,
      value: width
    });
    Object.defineProperty(window, 'innerHeight', {
      writable: true,
      configurable: true,
      value: height
    });
    window.dispatchEvent(new Event('resize'));
  }
};

/**
 * Component state testing utilities
 */
export const componentState = {
  /**
   * Check if component is in loading state
   */
  isLoading: (container: HTMLElement): boolean => {
    return !!(
      container.querySelector('[role="progressbar"]') ||
      container.querySelector('.loading') ||
      container.querySelector('[data-loading="true"]')
    );
  },

  /**
   * Check if component is in error state
   */
  hasError: (container: HTMLElement): boolean => {
    return !!(
      container.querySelector('[role="alert"]') ||
      container.querySelector('.error') ||
      container.querySelector('[data-error="true"]')
    );
  },

  /**
   * Check if component is disabled
   */
  isDisabled: (element: HTMLElement): boolean => {
    return element.hasAttribute('disabled') || 
           element.getAttribute('aria-disabled') === 'true';
  },

  /**
   * Check if component is visible
   */
  isVisible: (element: HTMLElement): boolean => {
    const style = window.getComputedStyle(element);
    return style.display !== 'none' && 
           style.visibility !== 'hidden' && 
           style.opacity !== '0';
  }
};

/**
 * Form testing utilities
 */
export const form = {
  /**
   * Fill form input
   */
  fill: async (user: UserEvent, input: HTMLElement, value: string) => {
    await user.clear(input);
    await user.type(input, value);
  },

  /**
   * Submit form
   */
  submit: async (user: UserEvent, formElement: HTMLFormElement) => {
    const submitButton = formElement.querySelector('button[type="submit"]') || 
                        formElement.querySelector('button:not([type="button"])');
    if (submitButton) {
      await user.click(submitButton);
    } else {
      formElement.dispatchEvent(new Event('submit', { bubbles: true }));
    }
  },

  /**
   * Check if form has validation errors
   */
  hasValidationErrors: (formElement: HTMLFormElement): boolean => {
    const invalidFields = formElement.querySelectorAll(':invalid');
    const errorMessages = formElement.querySelectorAll('[role="alert"], .error-message');
    return invalidFields.length > 0 || errorMessages.length > 0;
  },

  /**
   * Get form values
   */
  getValues: (formElement: HTMLFormElement): Record<string, any> => {
    const formData = new FormData(formElement);
    const values: Record<string, any> = {};
    formData.forEach((value, key) => {
      values[key] = value;
    });
    return values;
  }
};

/**
 * Focus management utilities
 */
export const focus = {
  /**
   * Check if element has focus
   */
  hasFocus: (element: HTMLElement): boolean => {
    return document.activeElement === element;
  },

  /**
   * Get currently focused element
   */
  getCurrent: (): Element | null => {
    return document.activeElement;
  },

  /**
   * Check if focus is trapped within container
   */
  isTrapped: (container: HTMLElement): boolean => {
    const focusableElements = container.querySelectorAll(
      'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])'
    );
    return focusableElements.length > 0 && 
           container.contains(document.activeElement);
  },

  /**
   * Tab through focusable elements
   */
  tab: async (user: UserEvent, reverse: boolean = false) => {
    if (reverse) {
      await user.keyboard('{Shift>}{Tab}{/Shift}');
    } else {
      await user.keyboard('{Tab}');
    }
  }
};

/**
 * Keyboard interaction utilities
 */
export const keyboard = {
  /**
   * Press Enter key
   */
  enter: async (user: UserEvent) => {
    await user.keyboard('{Enter}');
  },

  /**
   * Press Escape key
   */
  escape: async (user: UserEvent) => {
    await user.keyboard('{Escape}');
  },

  /**
   * Press Space key
   */
  space: async (user: UserEvent) => {
    await user.keyboard(' ');
  },

  /**
   * Navigate with arrow keys
   */
  arrow: async (user: UserEvent, direction: 'up' | 'down' | 'left' | 'right') => {
    const keyMap = {
      up: '{ArrowUp}',
      down: '{ArrowDown}',
      left: '{ArrowLeft}',
      right: '{ArrowRight}'
    };
    await user.keyboard(keyMap[direction]);
  }
};

/**
 * Mock component props
 */
export const mockProps = {
  /**
   * Create mock callback function
   */
  callback: (name: string = 'callback') => {
    const fn = vi.fn();
    fn.mockName(name);
    return fn;
  },

  /**
   * Create mock event handler
   */
  eventHandler: (name: string = 'handler') => {
    const fn = vi.fn((event: Event) => {
      event.preventDefault();
    });
    fn.mockName(name);
    return fn;
  },

  /**
   * Create mock ref
   */
  ref: <T extends HTMLElement>() => {
    return { current: null as T | null };
  }
};

/**
 * Component variant testing
 */
export const variants = {
  /**
   * Test all component variants
   */
  testAll: async (
    Component: React.ComponentType<any>,
    variantProps: Record<string, any[]>,
    baseProps: Record<string, any> = {}
  ) => {
    const results: RenderResult[] = [];
    
    for (const [propName, values] of Object.entries(variantProps)) {
      for (const value of values) {
        const props = { ...baseProps, [propName]: value };
        const result = render(<Component {...props} />);
        results.push(result);
      }
    }
    
    return results;
  }
};

/**
 * Snapshot testing utilities
 */
export const snapshot = {
  /**
   * Create consistent snapshot by removing dynamic values
   */
  sanitize: (html: string): string => {
    return html
      .replace(/id="[^"]+"/g, 'id="[id]"')
      .replace(/data-testid="[^"]+"/g, 'data-testid="[testid]"')
      .replace(/class="[^"]*hash[^"]*"/g, 'class="[hash]"')
      .replace(/\d{13,}/g, '[timestamp]'); // Remove timestamps
  }
};

/**
 * Color and theme testing
 */
export const theme = {
  /**
   * Get CSS variable value
   */
  getCSSVariable: (name: string): string => {
    return getComputedStyle(document.documentElement)
      .getPropertyValue(name)
      .trim();
  },

  /**
   * Set CSS variable
   */
  setCSSVariable: (name: string, value: string) => {
    document.documentElement.style.setProperty(name, value);
  },

  /**
   * Toggle dark mode
   */
  toggleDarkMode: () => {
    document.documentElement.classList.toggle('dark');
  },

  /**
   * Check if in dark mode
   */
  isDarkMode: (): boolean => {
    return document.documentElement.classList.contains('dark') ||
           window.matchMedia('(prefers-color-scheme: dark)').matches;
  }
};

/**
 * Common test scenarios
 */
export const scenarios = {
  /**
   * Test hover state
   */
  testHover: async (user: UserEvent, element: HTMLElement) => {
    await user.hover(element);
    const hoverState = styles.getComputed(element);
    await user.unhover(element);
    return hoverState;
  },

  /**
   * Test click interaction
   */
  testClick: async (user: UserEvent, element: HTMLElement, callback?: jest.Mock) => {
    await user.click(element);
    if (callback) {
      expect(callback).toHaveBeenCalled();
    }
  },

  /**
   * Test keyboard navigation
   */
  testKeyboardNav: async (user: UserEvent, container: HTMLElement) => {
    const focusableElements = container.querySelectorAll(
      'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])'
    );
    
    for (let i = 0; i < focusableElements.length; i++) {
      await keyboard.tab(user);
      expect(document.activeElement).toBe(focusableElements[i]);
    }
  }
};