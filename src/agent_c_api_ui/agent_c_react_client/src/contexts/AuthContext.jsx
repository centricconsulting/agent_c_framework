import React, { createContext, useState, useEffect, useContext, useCallback } from 'react';
import { PublicClientApplication, InteractionRequiredAuthError } from '@azure/msal-browser';
import { msalConfig, loginRequest } from '@/config/authConfig';
import { toast } from '@/hooks/use-toast';
import authService from '@/services/auth-api';

// Create the MSAL instance
const msalInstance = new PublicClientApplication(msalConfig);

// Create the Auth Context
export const AuthContext = createContext(null);

/**
 * AuthProvider component that manages authentication state
 * This wraps the application and provides authentication functionality
 */
export const AuthProvider = ({ children }) => {
  // State for authentication
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [user, setUser] = useState(null);
  const [account, setAccount] = useState(null);
  const [loading, setLoading] = useState(true);
  const [msalInitialized, setMsalInitialized] = useState(false);

  // Initialize MSAL
  useEffect(() => {
    const initializeMsal = async () => {
      try {
        await msalInstance.initialize();
        setMsalInitialized(true);
        
        // Check if we have any accounts
        const accounts = msalInstance.getAllAccounts();
        if (accounts.length > 0) {
          setAccount(accounts[0]);
          await validateUser(accounts[0]);
        }
      } catch (error) {
        console.error('MSAL initialization error:', error);
        toast({
          variant: 'destructive',
          title: 'Authentication Error',
          description: 'Failed to initialize authentication service.',
        });
      } finally {
        setLoading(false);
      }
    };

    initializeMsal();
  }, []);

  /**
   * Validate the user with the backend
   */
  const validateUser = async (currentAccount) => {
    try {
      if (!currentAccount) return;
      
      // Get token silently
      const tokenResponse = await msalInstance.acquireTokenSilent({
        scopes: loginRequest.scopes,
        account: currentAccount,
      });

      // Validate token with backend
      const userData = await authService.validateToken(tokenResponse.accessToken);
      setUser(userData);
      setIsAuthenticated(true);
      return userData;
    } catch (error) {
      console.error('Token validation error:', error);
      
      // If interaction is required, we should prompt the user to login again
      if (error instanceof InteractionRequiredAuthError) {
        setIsAuthenticated(false);
        setUser(null);
      }
      
      return null;
    }
  };

  /**
   * Handle user login
   */
  const login = useCallback(async () => {
    try {
      setLoading(true);
      if (!msalInitialized) {
        throw new Error('Authentication service not initialized');
      }

      // Login with popup
      const loginResponse = await msalInstance.loginPopup(loginRequest);
      setAccount(loginResponse.account);
      await validateUser(loginResponse.account);
    } catch (error) {
      console.error('Login error:', error);
      toast({
        variant: 'destructive',
        title: 'Login Failed',
        description: error.message || 'Failed to log in. Please try again.',
      });
    } finally {
      setLoading(false);
    }
  }, [msalInitialized]);

  /**
   * Handle user logout
   */
  const logout = useCallback(async () => {
    try {
      if (!msalInitialized) return;
      
      await msalInstance.logoutPopup();
      setIsAuthenticated(false);
      setUser(null);
      setAccount(null);
    } catch (error) {
      console.error('Logout error:', error);
      toast({
        variant: 'destructive',
        title: 'Logout Error',
        description: 'There was a problem logging out.',
      });
    }
  }, [msalInitialized]);

  /**
   * Get an access token for API calls
   */
  const getAccessToken = useCallback(async () => {
    try {
      if (!account) return null;
      
      const tokenResponse = await msalInstance.acquireTokenSilent({
        scopes: loginRequest.scopes,
        account: account,
      });
      
      return tokenResponse.accessToken;
    } catch (error) {
      console.error('Token acquisition error:', error);
      
      // If interaction is required, we should prompt the user to login again
      if (error instanceof InteractionRequiredAuthError) {
        try {
          const tokenResponse = await msalInstance.acquireTokenPopup(loginRequest);
          return tokenResponse.accessToken;
        } catch (popupError) {
          console.error('Token popup error:', popupError);
          throw popupError;
        }
      }
      
      throw error;
    }
  }, [account]);

  // Context value
  const contextValue = {
    isAuthenticated,
    user,
    account,
    loading,
    login,
    logout,
    getAccessToken,
    validateUser,
  };

  return (
    <AuthContext.Provider value={contextValue}>
      {children}
    </AuthContext.Provider>
  );
};

/**
 * Custom hook to use the auth context
 */
export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};