import React from 'react';
import { BrowserRouter as Router } from 'react-router-dom';
import AppRoutes from '@/Routes';
import { SessionProvider } from '@/contexts/SessionContext';
import { ThemeProvider } from '@/contexts/ThemeContext';

function App() {
  return (
    <ThemeProvider>
      <SessionProvider>
        <Router>
          <AppRoutes />
        </Router>
      </SessionProvider>
    </ThemeProvider>
  );
}

export default App;