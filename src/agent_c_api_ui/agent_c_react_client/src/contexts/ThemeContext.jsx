import React, { createContext, useState, useEffect } from 'react';
import logger from '@/lib/logger';
import { getStoredTheme, storeTheme } from '@/lib/theme';
import { trackContextInitialization } from '@/lib/diagnostic';

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
    try {
      const savedTheme = getStoredTheme();
      logger.debug('Initial theme loaded from localStorage', 'ThemeProvider', { savedTheme });
      return savedTheme;
    } catch (error) {
      logger.error('Failed to load theme from localStorage', 'ThemeProvider', { error });
      return 'system'; // Fall back to system theme
    }
  });

  // Handle theme change and persist to localStorage
  const setTheme = (newTheme) => {
    try {
      logger.info('Theme changed', 'ThemeProvider', { previousTheme: theme, newTheme });
      setThemeState(newTheme);
      storeTheme(newTheme);
    } catch (error) {
      logger.error('Failed to change theme', 'ThemeProvider', { error, previousTheme: theme, newTheme });
    }
  };

  // Apply theme class to the document root
  useEffect(() => {
    try {
      const root = window.document.documentElement;
      const startTime = performance.now();
      
      // Set data attribute for diagnostics
      root.setAttribute('data-theme', theme);
      root.setAttribute('data-theme-provider', 'active');
      
      // Remove old theme classes
      root.classList.remove('light', 'dark');
      
      // Determine if we should use dark mode
      let appliedTheme;
      if (theme === 'system') {
        // Check system preference
        const systemPrefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
        appliedTheme = systemPrefersDark ? 'dark' : 'light';
        root.classList.add(appliedTheme);
      } else {
        // Apply the selected theme
        appliedTheme = theme;
        root.classList.add(theme);
      }
      
      // Theme has been applied
      const duration = performance.now() - startTime;
      logger.debug(`Theme applied: ${theme} (effective: ${appliedTheme})`, 'ThemeProvider', { 
        duration: `${duration.toFixed(2)}ms`, 
        appliedTheme,
        selectedTheme: theme 
      });
    } catch (error) {
      logger.error('Failed to apply theme', 'ThemeProvider', { error, theme });
    }
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
      <div data-theme-provider="mounted" style={{ display: 'none' }}>
        ThemeProvider diagnostic element
      </div>
      {children}
    </ThemeContext.Provider>
  );
};

// The useTheme hook is now in src/hooks/use-theme.jsx