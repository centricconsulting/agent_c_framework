import React, { useEffect } from 'react';
import { useSessionContext } from '../hooks/use-session-context';

/**
 * ThemeProvider component
 * Applies theme classes to the document root based on theme from SessionContext
 */
export const ThemeProvider = ({ children }) => {
  const { theme } = useSessionContext('ThemeProvider');
  
  // Apply theme based on context

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
      if (systemPrefersDark) {
        root.classList.add('dark');
      } else {
        root.classList.add('light');
      }
    } else {
      // Apply the selected theme
      root.classList.add(theme);
    }
    
    // Theme has been applied
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
    };

    // Add event listener
    mediaQuery.addEventListener('change', handleChange);
    // Media query listener added
    
    // Cleanup
    return () => {
      mediaQuery.removeEventListener('change', handleChange);
      // Media query listener removed
    };
  }, [theme]);

  return <>{children}</>;
};