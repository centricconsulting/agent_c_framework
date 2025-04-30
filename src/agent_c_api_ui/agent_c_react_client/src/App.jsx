import React from 'react';
import { BrowserRouter as Router } from 'react-router-dom';
import AppRoutes from '@/Routes';
import { SessionProvider } from '@/contexts/SessionContext';
import { LegacySessionProvider } from '@/contexts/LegacySessionContext';
import { ThemeProvider } from '@/contexts/ThemeProvider';

function App() {
  return (
    <SessionProvider>
      <LegacySessionProvider>
        <ThemeProvider>
          <Router>
            <AppRoutes />
          </Router>
        </ThemeProvider>
      </LegacySessionProvider>
    </SessionProvider>
  );
}

export default App;