import React from 'react';
import { BrowserRouter as Router } from 'react-router-dom';
import AppRoutes from '@/Routes';
import { SessionProvider } from '@/contexts/SessionContext';
import { ThemeProvider } from '@/contexts/ThemeProvider';
import { AuthProvider } from '@/contexts/AuthContext';
// import { Toaster } from '@/components/ui/toast';

function App() {
  return (
    <AuthProvider>
      <SessionProvider>
        <ThemeProvider>
          <Router>
            <AppRoutes />
          </Router>

        </ThemeProvider>
      </SessionProvider>
    </AuthProvider>
  );
}

export default App;