import React, { createContext, useState, useEffect } from 'react';
import { useLogger } from '@/hooks/use-logger';
import { storageService } from '@/lib/storageService';
import { initializeContextDiagnostics, withContextDiagnostics, logContextStateChange } from '@/lib/diagnostic';

// Initialize diagnostics if not already done
if (typeof window !== 'undefined' && !window.__CONTEXT_DIAGNOSTICS) {
  initializeContextDiagnostics();
}

// Create the context
export const ThemeContext = createContext(null);

// Default theme state
const defaultTheme = {
  mode: 'system', // 'light', 'dark', or 'system'
  systemPreference: 'light', // detected system preference
  computed: 'light', // the actual theme applied (based on mode and system preference)
};

// Theme provider component with enhanced diagnostics
export const ThemeProvider = ({ children }) => {
  const logger = useLogger();
  const [themeState, setThemeState] = useState(() => {
    // Initialize diagnostics for this context
    if (window.__CONTEXT_DIAGNOSTICS) {
      window.__CONTEXT_DIAGNOSTICS.recordContextStart('theme');
    }
    
    try {
      // Try to load theme from storage
      logger.debug('Initializing theme state', 'ThemeProvider');
      const storedTheme = storageService.getItem('themePreference');
      
      // Detect system preference
      const prefersDark = typeof window !== 'undefined' &&
        window.matchMedia && 
        window.matchMedia('(prefers-color-scheme: dark)').matches;
      
      const systemPreference = prefersDark ? 'dark' : 'light';
      
      // Use stored theme or default
      const theme = storedTheme ? JSON.parse(storedTheme) : { mode: 'system' };
      
      // Calculate computed theme
      const computed = theme.mode === 'system' ? systemPreference : theme.mode;
      
      const initialState = {
        mode: theme.mode || 'system',
        systemPreference,
        computed
      };
      
      // Record successful initialization
      if (window.__CONTEXT_DIAGNOSTICS) {
        window.__CONTEXT_DIAGNOSTICS.recordContextSuccess('theme', initialState);
      }
      
      return initialState;
    } catch (error) {
      // Log error and fallback to defaults
      logger.error('Failed to initialize theme state', error, 'ThemeProvider');
      
      // Record error in diagnostics
      if (window.__CONTEXT_DIAGNOSTICS) {
        window.__CONTEXT_DIAGNOSTICS.recordContextError('theme', error);
      }
      
      return defaultTheme;
    }
  });

  // Function to update theme
  const setTheme = (mode) => {
    logger.debug(`Setting theme to ${mode}`, 'ThemeProvider');
    
    const prevState = { ...themeState };
    
    // Update state based on the new mode
    setThemeState(prev => {
      const newState = {
        ...prev,
        mode,
        computed: mode === 'system' ? prev.systemPreference : mode
      };
      
      // Log state change for diagnostics
      logContextStateChange('theme', prevState, newState);
      
      return newState;
    });
    
    // Persist theme preference
    storageService.setItem('themePreference', JSON.stringify({ mode }));
  };

  // Listen for system preference changes
  useEffect(() => {
    logger.debug('Setting up system theme preference listener', 'ThemeProvider');
    
    const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
    
    const handleChange = (e) => {
      const prevState = { ...themeState };
      const newSystemPreference = e.matches ? 'dark' : 'light';
      
      logger.debug(
        `System theme preference changed to ${newSystemPreference}`, 
        'ThemeProvider'
      );
      
      setThemeState(prev => {
        const newState = {
          ...prev,
          systemPreference: newSystemPreference,
          computed: prev.mode === 'system' ? newSystemPreference : prev.mode
        };
        
        // Log state change for diagnostics
        logContextStateChange('theme', prevState, newState);
        
        return newState;
      });
    };

    // Add event listener with error handling
    try {
      // Modern API (newer browsers)
      mediaQuery.addEventListener('change', handleChange);
    } catch (err) {
      try {
        // Legacy API (older browsers)
        mediaQuery.addListener(handleChange);
      } catch (fallbackErr) {
        logger.error(
          'Failed to add system theme listener', 
          fallbackErr, 
          'ThemeProvider'
        );
      }
    }

    // Cleanup function
    return () => {
      logger.debug('Cleaning up system theme preference listener', 'ThemeProvider');
      
      try {
        // Modern API
        mediaQuery.removeEventListener('change', handleChange);
      } catch (err) {
        try {
          // Legacy API
          mediaQuery.removeListener(handleChange);
        } catch (fallbackErr) {
          logger.error(
            'Failed to remove system theme listener', 
            fallbackErr, 
            'ThemeProvider'
          );
        }
      }
    };
  }, [logger, themeState]);

  // Apply theme to document element
  useEffect(() => {
    logger.debug(`Applying theme: ${themeState.computed}`, 'ThemeProvider');
    
    // Record event in diagnostics
    if (window.__CONTEXT_DIAGNOSTICS) {
      window.__CONTEXT_DIAGNOSTICS.recordEvent('themeApplied', {
        theme: themeState.computed
      });
    }
    
    const htmlElement = document.documentElement;
    
    if (themeState.computed === 'dark') {
      htmlElement.classList.add('dark');
    } else {
      htmlElement.classList.remove('dark');
    }
  }, [themeState.computed, logger]);

  // Create context value
  const contextValue = {
    // Theme state
    theme: themeState.computed, // Current applied theme ('light' or 'dark')
    mode: themeState.mode, // User's preference ('light', 'dark', or 'system')
    systemPreference: themeState.systemPreference, // System theme ('light' or 'dark')
    
    // Theme methods
    setTheme, // Function to change theme
    toggleTheme: () => {
      // Toggle between light and dark (ignoring system)
      const newMode = themeState.computed === 'light' ? 'dark' : 'light';
      setTheme(newMode);
    },
    resetTheme: () => {
      // Reset to system preference
      setTheme('system');
    }
  };

  return (
    <ThemeContext.Provider value={contextValue}>
      {children}
    </ThemeContext.Provider>
  );
};

// Enhanced error boundary for ThemeProvider
export const ThemeProviderWithErrorBoundary = ({ children }) => {
  const [error, setError] = React.useState(null);
  
  if (error) {
    // Render fallback UI for theme errors
    return (
      <div className="theme-error-boundary">
        <h3>Theme System Error</h3>
        <p>{error.message || 'An error occurred in the theme system'}</p>
        <p>The application will continue with default light theme.</p>
        {/* Apply default light theme to document in case of error */}
        {document?.documentElement?.classList.remove('dark')}
      </div>
    );
  }
  
  // Wrap ThemeProvider with error handling
  try {
    return <ThemeProvider>{children}</ThemeProvider>;
  } catch (e) {
    setError(e);
    return null;
  }
};