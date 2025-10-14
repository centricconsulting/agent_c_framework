'use client';

import React, { createContext, useContext, useState, useCallback, useEffect, useMemo } from 'react';
import {
  login as authLogin,
  logout as authLogout,
  getToken,
  isAuthenticated as checkIsAuthenticated,
  type LoginCredentials,
  type LoginResponse
} from '@/lib/auth';
import { Logger } from '@/utils/logger';

// DEBUG MODE - Set to true for verbose logging
const DEBUG_AUTH = false;

// Debug logger
const authLog = {
  info: (message: string, data?: any) => {
    if (DEBUG_AUTH) {
      Logger.info(`[AUTH-CONTEXT] ✅ ${message}`, data || '');
    }
  },
  error: (message: string, error?: any) => {
    Logger.error(`[AUTH-CONTEXT] ❌ ${message}`, error || '');
  },
  warn: (message: string, data?: any) => {
    if (DEBUG_AUTH) {
      Logger.warn(`[AUTH-CONTEXT] ⚠️ ${message}`, data || '');
    }
  },
  debug: (message: string, data?: any) => {
    if (DEBUG_AUTH) {
      Logger.debug(`[AUTH-CONTEXT] 🔍 ${message}`, data || '');
    }
  }
};

/**
 * Simplified Authentication context value
 * 
 * This context ONLY handles authentication state and tokens.
 * User data comes from the WebSocket connection via SDK hooks.
 */
interface AuthContextValue {
  // Authentication state
  isAuthenticated: boolean;
  isLoading: boolean;
  
  // Auth functions
  login: (credentials: LoginCredentials) => Promise<LoginResponse>;
  logout: () => void;
  
  // Token helpers
  getAuthToken: () => string | null;
  getUiSessionId: () => string | undefined;
}

/**
 * Auth context
 */
const AuthContext = createContext<AuthContextValue | null>(null);

/**
 * Hook to use auth context
 */
export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}

/**
 * Auth provider props
 */
interface AuthProviderProps {
  children: React.ReactNode;
}

/**
 * Simplified Auth Provider Component
 * 
 * This provider ONLY manages:
 * - Authentication state (logged in/out)
 * - Login/logout actions
 * - Token management
 * - UI session ID for reconnection
 * 
 * It does NOT manage:
 * - User profile data (comes from WebSocket)
 * - Agent/avatar/voice lists (come from WebSocket)
 * - Any configuration data (comes from WebSocket)
 */
export function AuthProvider({ children }: AuthProviderProps) {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [uiSessionId, setUiSessionId] = useState<string | undefined>();

  // Helper to get stored UI session ID
  // IMPORTANT: Uses sessionStorage (not localStorage) so each tab has its own UI session
  // This allows multiple tabs to have independent chat sessions
  const getStoredUiSessionId = (): string | undefined => {
    try {
      const stored = sessionStorage.getItem('agentc-ui-session-id');
      if (stored) {
        authLog.debug('Retrieved stored UI session ID:', stored);
        return stored;
      }
    } catch (error) {
      authLog.error('Failed to retrieve UI session ID', error);
    }
    return undefined;
  };

  // Helper to store UI session ID
  // IMPORTANT: Uses sessionStorage (not localStorage) so each tab has its own UI session
  const storeUiSessionId = (sessionId: string) => {
    try {
      sessionStorage.setItem('agentc-ui-session-id', sessionId);
      authLog.debug('Stored UI session ID:', sessionId);
    } catch (error) {
      authLog.error('Failed to store UI session ID', error);
    }
  };

  // Helper to clear UI session ID
  const clearUiSessionId = () => {
    try {
      sessionStorage.removeItem('agentc-ui-session-id');
      authLog.debug('Cleared UI session ID');
    } catch (error) {
      authLog.error('Failed to clear UI session ID', error);
    }
  };

  // Check authentication status on mount
  useEffect(() => {
    authLog.info('Checking authentication status...');
    
    const checkAuth = () => {
      try {
        // Check if we have a valid token
        const authenticated = checkIsAuthenticated();
        setIsAuthenticated(authenticated);
        
        if (authenticated) {
          authLog.info('User is authenticated');
          
          // Restore UI session ID if available
          const storedSessionId = getStoredUiSessionId();
          if (storedSessionId) {
            setUiSessionId(storedSessionId);
            authLog.info('Restored UI session ID');
          }
        } else {
          authLog.info('User is NOT authenticated');
          setUiSessionId(undefined);
          clearUiSessionId();
        }
      } catch (error) {
        authLog.error('Error checking authentication', error);
        setIsAuthenticated(false);
        setUiSessionId(undefined);
        clearUiSessionId();
      } finally {
        setIsLoading(false);
      }
    };

    // Initial auth check
    checkAuth();

    // Check auth status periodically to handle token expiration
    const interval = setInterval(() => {
      const authenticated = checkIsAuthenticated();
      setIsAuthenticated(prevAuthenticated => {
        if (authenticated !== prevAuthenticated) {
          authLog.info(`Auth state changed: ${authenticated ? 'authenticated' : 'not authenticated'}`);
          if (!authenticated) {
            setUiSessionId(undefined);
            clearUiSessionId();
          }
        }
        return authenticated;
      });
    }, 30000); // Check every 30 seconds
    
    return () => {
      clearInterval(interval);
    };
  }, []); // Run only once on mount

  /**
   * Login function
   * 
   * The new login response only contains:
   * - agent_c_token: JWT for authentication
   * - heygen_token: Token for HeyGen avatar service
   * - ui_session_id: Session identifier for reconnection
   * 
   * User data will come from WebSocket events after connection.
   */
  const login = useCallback(async (credentials: LoginCredentials): Promise<LoginResponse> => {
    authLog.info('Login attempt starting...');
    setIsLoading(true);
    
    try {
      // Call the auth library login function
      const response = await authLogin(credentials);
      
      authLog.info('Login successful', {
        hasAgentCToken: !!response?.agent_c_token,
        hasHeygenToken: !!response?.heygen_token,
        hasUiSessionId: !!response?.ui_session_id
      });
      
      // Store UI session ID for reconnection
      if (response.ui_session_id) {
        setUiSessionId(response.ui_session_id);
        storeUiSessionId(response.ui_session_id);
      }
      
      // Update authentication state
      setIsAuthenticated(true);
      
      // Return the response for the caller to handle
      // (e.g., to establish WebSocket connection)
      return response;
    } catch (error) {
      authLog.error('Login failed', error);
      setIsAuthenticated(false);
      setUiSessionId(undefined);
      clearUiSessionId();
      throw error;
    } finally {
      setIsLoading(false);
    }
  }, []);

  /**
   * Logout function
   */
  const logout = useCallback(() => {
    authLog.info('Logging out...');
    authLogout();
    setIsAuthenticated(false);
    setUiSessionId(undefined);
    clearUiSessionId();
  }, []);

  /**
   * Get auth token
   */
  const getAuthToken = useCallback(() => {
    return getToken();
  }, []);

  /**
   * Get UI session ID for reconnection
   */
  const getUiSessionId = useCallback(() => {
    return uiSessionId;
  }, [uiSessionId]);

  /**
   * Context value - simplified to only authentication concerns
   */
  const value = useMemo<AuthContextValue>(() => ({
    isAuthenticated,
    isLoading,
    login,
    logout,
    getAuthToken,
    getUiSessionId
  }), [
    isAuthenticated,
    isLoading,
    login,
    logout,
    getAuthToken,
    getUiSessionId
  ]);

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
}

// Export types and context for advanced use cases
export { AuthContext };
export type { AuthContextValue, AuthProviderProps };