import React, { useEffect } from 'react';
import { BrowserRouter as Router } from 'react-router-dom';
import AppRoutes from '@/Routes';
import { SessionProvider } from '@/contexts/SessionContext';
import { ThemeProvider } from '@/contexts/ThemeContext';
import { AuthProvider } from '@/contexts/AuthContext';
import { ModelProvider } from '@/contexts/ModelContext';
import { trackContextInitialization, getContextInitializationStatus } from '@/lib/diagnostic';
import '@/lib/context-diagnostic-console';
import logger from '@/lib/logger';
import { ErrorBoundary } from '@/components/ui/error-boundary';

function App() {
  // Initialize diagnostic tracking when the app starts
  useEffect(() => {
    // Initialize diagnostics
    logger.info('Application initializing with diagnostic tracking', 'App');
    
    // Add diagnostic utility to window for console access
    window.diagnosticReport = getContextInitializationStatus;
    
    // Run a diagnostic check after the app has been mounted for 10 seconds
    const timeoutId = setTimeout(() => {
      const results = getContextInitializationStatus();
      logger.info('Automatic diagnostic check after 10s', 'App', {
        contextStatus: results
      });
    }, 10000);
    
    return () => {
      clearTimeout(timeoutId);
    };
  }, []);
  
  return (
    <ErrorBoundary name="AppRoot">
      <ErrorBoundary name="ThemeProvider">
        <ThemeProvider>
          <ErrorBoundary name="AuthProvider">
            <AuthProvider>
              <ErrorBoundary name="ModelProvider">
                <ModelProvider>
                  <ErrorBoundary name="SessionProvider">
                    <SessionProvider>
                      <Router>
                        <AppRoutes />
                      </Router>
                    </SessionProvider>
                  </ErrorBoundary>
                </ModelProvider>
              </ErrorBoundary>
            </AuthProvider>
          </ErrorBoundary>
        </ThemeProvider>
      </ErrorBoundary>
    </ErrorBoundary>
  );
}

export default App;