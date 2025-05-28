import React, { useEffect, useState } from 'react';
import { Navigate, useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

const LoginPage = () => {
  const { user, msalReady, login, isAuthenticated, loading, error } = useAuth();
  const [localLoading, setLocalLoading] = useState(false);
  const [localError, setLocalError] = useState(null);
  const location = useLocation();
  const navigate = useNavigate();

  // If user is already authenticated, redirect to the main page
  // or to the page they were trying to access before being redirected to login
  useEffect(() => {
    if (isAuthenticated) {
      const from = location.state?.from?.pathname || '/chat';
      navigate(from, { replace: true });
    }
  }, [isAuthenticated, location.state, navigate]);

  const handleLogin = async () => {
    try {
      if (!msalReady) return;
      setLocalLoading(true);
      setLocalError(null);
      await login();
      // Auth context will handle the redirect after successful login
    } catch (err) {
      console.error('Login error:', err);
      setLocalError('Login failed. Please try again.');
    } finally {
      setLocalLoading(false);
    }
  };

  // If already authenticated, redirect to main page
  if (isAuthenticated) {
    return <Navigate to="/chat" replace />;
  }

  return (
    <div className="flex flex-col items-center justify-center h-screen space-y-4 bg-background">
      <h1 className="text-3xl font-bold mb-20 text-foreground">Welcome to Agent C</h1>

      {loading || localLoading ? (
        <div className="flex flex-col items-center space-y-4">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-sidebar-primary"></div>
          <p className="text-muted-foreground">Connecting to authentication service...</p>
        </div>
      ) : (
        <div className="flex flex-col items-center space-y-6 w-full">
          {(error || localError) && (
            <div className="bg-destructive/10 border border-destructive text-destructive-foreground px-4 py-3 rounded relative">
              {error || localError}
            </div>
          )}

          <button
            onClick={handleLogin}
            disabled={!msalReady || loading || localLoading}
            className="px-4 py-2 w-64 bg-sidebar-primary text-sidebar-primary-foreground rounded hover:bg-sidebar-primary/90 transition-colors disabled:opacity-50"
          >
            Continue with Microsoft Account
          </button>

{/*           <button */}
{/*             disabled */}
{/*             className="px-4 py-2 w-64 bg-muted text-muted-foreground rounded cursor-not-allowed" */}
{/*           > */}
{/*             Continue with Google (coming soon) */}
{/*           </button> */}

{/*           <button */}
{/*             disabled */}
{/*             className="px-4 py-2 w-64 bg-muted text-muted-foreground rounded cursor-not-allowed" */}
{/*           > */}
{/*             Continue with Apple (coming soon) */}
{/*           </button> */}
        </div>
      )}
    </div>
  );
};

export default LoginPage;