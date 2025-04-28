import React from 'react';
import { BrowserRouter as Router } from 'react-router-dom';
import AppRoutes from '@/Routes';
import { SessionProvider } from '@/contexts/SessionContext';
import { ThemeProvider } from '@/contexts/ThemeContext';
import { AuthProvider } from '@/contexts/AuthContext';
import { ModelProvider } from '@/contexts/ModelContext';
import logger from '@/lib/logger';
import { ErrorBoundary } from '@/components/ui/error-boundary';

// Only import debug tools in development mode
const EnhancedDebugPanel = process.env.NODE_ENV === 'development' 
  ? React.lazy(() => import('@/components/ui/enhanced-debug-panel'))
  : () => null;

function App() {
  // Log application start - minimal impact
  logger.info('Application initializing', 'App');
  
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
                        {/* Only render debug panel in development mode */}
                        {process.env.NODE_ENV === 'development' && (
                          <React.Suspense fallback={null}>
                            <EnhancedDebugPanel />
                          </React.Suspense>
                        )}
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