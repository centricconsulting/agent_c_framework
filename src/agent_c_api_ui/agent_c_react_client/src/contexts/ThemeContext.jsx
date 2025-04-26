import React, { createContext, useState, useEffect, useContext } from 'react';
import logger from '../lib/logger';
import { getStoredTheme, storeTheme } from '../lib/theme';

/**
 * @typedef {'light' | 'dark' | 'system'} ThemeType
 */

/**
 * Context for theme management across the application
 */
export const ThemeContext = createContext({
  theme: 'system',
  setTheme: () => {}
});

/**
 * ThemeContext Provider component
 * Manages theme state and persistence
 */
export const ThemeProvider = ({ children }) => {
  // Initialize theme from localStorage or default to 'system'
  const [theme, setThemeState] = useState(() => {
    const savedTheme = getStoredTheme();
    logger.debug('Initial theme loaded from localStorage', 'ThemeProvider', { savedTheme });
    return savedTheme;
  });

  // Handle theme change and persist to localStorage
  const setTheme = (newTheme) => {
    logger.info('Theme changed', 'ThemeProvider', { previousTheme: theme, newTheme });
    setThemeState(newTheme);
    storeTheme(newTheme);
  };

  // Apply theme class to the document root
  useEffect(() => {
    const root = window.document.documentElement;
    const startTime = performance.now();
    
    // Remove old theme classes
    root.classList.remove('light', 'dark');
    
    // Determine if we should use dark mode
    if (theme === 'system') {
      // Check system preference
      const systemPrefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
      root.classList.add(systemPrefersDark ? 'dark' : 'light');
    } else {
      // Apply the selected theme
      root.classList.add(theme);
    }
    
    // Theme has been applied
    const duration = performance.now() - startTime;
    logger.debug(`Theme applied: ${theme}`, 'ThemeProvider', { duration: `${duration.toFixed(2)}ms` });
  }, [theme]);

  // Listen for changes in OS color scheme preference
  useEffect(() => {
    if (theme !== 'system') return;

    const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
    
    // Handler for preference changes
    const handleChange = () => {
      const root = window.document.documentElement;
      root.classList.remove('light', 'dark');
      const newTheme = mediaQuery.matches ? 'dark' : 'light';
      root.classList.add(newTheme);
      logger.debug(`System preference changed to ${newTheme}`, 'ThemeProvider');
    };

    // Add event listener
    mediaQuery.addEventListener('change', handleChange);
    logger.debug('Media query listener added', 'ThemeProvider');
    
    // Cleanup
    return () => {
      mediaQuery.removeEventListener('change', handleChange);
      logger.debug('Media query listener removed', 'ThemeProvider');
    };
  }, [theme]);

  // Context value
  const contextValue = {
    theme,
    setTheme
  };

  return (
    <ThemeContext.Provider value={contextValue}>
      {children}
    </ThemeContext.Provider>
  );
};

/**
 * Custom hook for consuming the ThemeContext
 * @returns {Object} The ThemeContext value { theme, setTheme }
 */
export const useTheme = () => {
  const context = useContext(ThemeContext);
  
  if (context === undefined) {
    throw new Error('useTheme must be used within a ThemeProvider');
  }
  
  return context;
};