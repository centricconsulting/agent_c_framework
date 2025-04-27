import React from 'react';
import { BrowserRouter as Router } from 'react-router-dom';
import AppRoutes from '@/Routes';
import { SessionProvider } from '@/contexts/SessionContext';
import { ThemeProvider } from '@/contexts/ThemeContext';
import { AuthProvider } from '@/contexts/AuthContext';
import { ModelProvider } from '@/contexts/ModelContext';

function App() {
  return (
    <ThemeProvider>
      <AuthProvider>
        <ModelProvider>
          <SessionProvider>
            <Router>
              <AppRoutes />
            </Router>
          </SessionProvider>
        </ModelProvider>
      </AuthProvider>
    </ThemeProvider>
  );
}

export default App;