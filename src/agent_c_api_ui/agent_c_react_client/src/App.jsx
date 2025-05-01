import React from 'react';
import { BrowserRouter as Router } from 'react-router-dom';
import AppRoutes from '@/Routes';
import { SessionProvider } from '@/contexts/SessionContext';
import { ModelProvider } from '@/contexts/ModelContext';
import { LegacySessionProvider } from '@/contexts/LegacySessionContext';
import { ThemeProvider } from '@/contexts/ThemeProvider';

function App() {
  return (
    <SessionProvider>
      <ModelProvider>
        <LegacySessionProvider>
          <ThemeProvider>
            <Router>
              <AppRoutes />
            </Router>
          </ThemeProvider>
        </LegacySessionProvider>
      </ModelProvider>
    </SessionProvider>
  );
}

export default App;