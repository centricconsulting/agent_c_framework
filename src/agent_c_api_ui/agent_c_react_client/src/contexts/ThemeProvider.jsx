import React, { useContext, useEffect } from 'react';
import { LegacySessionContext } from './LegacySessionContext';

export const ThemeProvider = ({ children }) => {
  const { theme } = useContext(LegacySessionContext);

  // Apply theme class to the document root
  useEffect(() => {
    const root = window.document.documentElement;
    
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
  }, [theme]);

  // Listen for changes in OS color scheme preference
  useEffect(() => {
    if (theme !== 'system') return;

    const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
    
    // Handler for preference changes
    const handleChange = () => {
      const root = window.document.documentElement;
      root.classList.remove('light', 'dark');
      root.classList.add(mediaQuery.matches ? 'dark' : 'light');
    };

    // Add event listener
    mediaQuery.addEventListener('change', handleChange);
    
    // Cleanup
    return () => mediaQuery.removeEventListener('change', handleChange);
  }, [theme]);

  return <>{children}</>;
};