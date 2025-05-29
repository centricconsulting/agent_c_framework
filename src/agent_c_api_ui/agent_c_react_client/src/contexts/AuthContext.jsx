import React, { createContext, useContext, useState, useEffect, useRef } from 'react';
import { PublicClientApplication } from '@azure/msal-browser';
import { msalConfig, loginRequest, authConfig } from '../config/authConfig';
import axios from 'axios';
import { registerTokenProvider, getTokenManager } from '../lib/auth-helper';
import { TokenManager } from '../lib/token-manager';

// Create context
const AuthContext = createContext();

// MSAL instance
const msalInstance = new PublicClientApplication(msalConfig);

// Provider component
export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [msalReady, setMsalReady] = useState(false);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [tokenRefreshError, setTokenRefreshError] = useState(null);
  const tokenRefreshCount = useRef(0);

  // Initialize MSAL on component mount
  useEffect(() => {
    const initMSAL = async () => {
      try {
        setLoading(true);
        await msalInstance.initialize();
        const accounts = msalInstance.getAllAccounts();
        
        if (accounts.length > 0) {
          setUser(accounts[0]);
          
          // Validate the token with the backend
          try {
            const token = await getAccessToken();
            // Call the backend validation endpoint
            const response = await axios.get('/api/v2/auth/validate', {
              headers: {
                Authorization: `Bearer ${token}`
              }
            });
            
            if (response.data.valid) {
              setIsAuthenticated(true);
            } else {
              // Token not valid on backend, force re-login
              console.warn('Token not validated by backend, requiring re-authentication');
              await msalInstance.logoutPopup();
              setUser(null);
              setIsAuthenticated(false);
            }
          } catch (validationError) {
            console.error('Token validation error:', validationError);
            // If validation fails, don't consider user authenticated
            setIsAuthenticated(false);
            
            // Only force logout if required by config
            if (authConfig.requireAuth) {
              try {
                await msalInstance.logoutPopup();
                setUser(null);
              } catch (e) {
                console.error('Logout error after validation failure:', e);
              }
            }
          }
        }
        
        setMsalReady(true);
        setError(null);
      } catch (error) {
        console.error('MSAL initialization error:', error);
        setError('Failed to initialize authentication service');
      } finally {
        setLoading(false);
      }
    };

    initMSAL();
  }, []);

  // Login function
  const login = async () => {
    try {
      if (!msalReady) throw new Error('Authentication service not ready');
      
      const response = await msalInstance.loginPopup(loginRequest);
      setUser(response.account);
      setIsAuthenticated(true);
      setError(null);
      
      return response.account;
    } catch (err) {
      console.error('Login error:', err);
      setError(err.message || 'Login failed');
      throw err;
    }
  };

  // Logout function
  const logout = async () => {
    try {
      await msalInstance.logoutPopup();
      setUser(null);
      setIsAuthenticated(false);
    } catch (err) {
      console.error('Logout error:', err);
      setError(err.message || 'Logout failed');
    }
  };

  // Get access token for API calls with enhanced error handling and refresh strategy
  const getAccessToken = async (options = {}) => {
    try {
      // Track refresh attempts for debugging
      if (options.forceRefresh) {
        tokenRefreshCount.current++;
      }
      
      // Check if user is authenticated
      if (!user) {
        console.debug('Token requested while user is not authenticated');
        return null; // Return null instead of throwing an error
      }
      
      const accounts = msalInstance.getAllAccounts();
      if (accounts.length === 0) {
        console.debug('No accounts found in MSAL instance');
        return null; // Return null instead of throwing an error
      }
      
      const silentRequest = {
        scopes: loginRequest.scopes,
        account: accounts[0],
        forceRefresh: options.forceRefresh === true
      };
      
      // Clear any previous refresh errors when starting a fresh attempt
      if (options.forceRefresh) {
        setTokenRefreshError(null);
      }
      
      try {
        const response = await msalInstance.acquireTokenSilent(silentRequest);
        return response.accessToken;
      } catch (silentErr) {
        // If silent token acquisition fails, try interactive method
        if (silentErr.name === 'InteractionRequiredAuthError') {
          // Only use popup for user-initiated actions, not background refreshes
          if (!options.background) {
            try {
              const response = await msalInstance.acquireTokenPopup(loginRequest);
              return response.accessToken;
            } catch (interactiveErr) {
              console.error('Interactive token acquisition failed:', interactiveErr);
              setTokenRefreshError(interactiveErr.message || 'Failed to refresh token interactively');
              return null;
            }
          } else {
            // Don't interrupt users with popups during background refresh
            console.warn('Silent token refresh failed and interactive refresh skipped for background operation');
            setTokenRefreshError('Background token refresh failed');
            return null;
          }
        } else {
          // Handle other errors
          console.error('Token acquisition failed:', silentErr);
          setTokenRefreshError(silentErr.message || 'Failed to refresh token');
          return null;
        }
      }
    } catch (err) {
      console.error('Unexpected error in getAccessToken:', err);
      setTokenRefreshError(err.message || 'Unexpected authentication error');
      return null;
    }
  };
  
  // Force refresh the token
  const forceRefreshToken = async () => {
    return await getAccessToken({ forceRefresh: true });
  };
  
  // Set up token refresh listeners
  useEffect(() => {
    const setupTokenManagerListeners = () => {
      const tokenManager = getTokenManager();
      if (!tokenManager) return;
      
      // Listen for token refresh events
      tokenManager.on('refreshStarted', () => {
        console.debug('Token refresh started');
      });
      
      tokenManager.on('refreshed', (token) => {
        console.debug('Token successfully refreshed');
        setTokenRefreshError(null);
      });
      
      tokenManager.on('refreshFailed', (error) => {
        console.warn('Token refresh failed:', error);
        setTokenRefreshError(error?.message || 'Failed to refresh token');
      });
      
      tokenManager.on('refreshExhausted', (error) => {
        console.error('Token refresh retry attempts exhausted:', error);
        setTokenRefreshError('Authentication session may have expired. Please log in again.');
        
        // Optionally force logout after multiple failed refreshes
        if (authConfig.tokenRefresh.logoutOnRefreshExhaustion) {
          logout().catch(e => console.error('Logout after refresh exhaustion failed:', e));
        }
      });
    };
    
    // Setup listeners after a short delay to ensure token manager is initialized
    const timerId = setTimeout(setupTokenManagerListeners, 500);
    
    return () => {
      clearTimeout(timerId);
    };
  }, [isAuthenticated]);
  
  // Register the token provider for global access
  useEffect(() => {
    // Register our getAccessToken method as the global token provider with the token refresh configuration
    registerTokenProvider(
      // Wrapper function to support background refreshes
      async () => getAccessToken({ background: true }),
      // Token refresh configuration
      authConfig.tokenRefresh
    );
    
    return () => {
      // Reset to default (null provider) when component unmounts
      registerTokenProvider(null);
    };
  }, []);
  

  // Context value
  const value = {
    user,
    msalReady,
    loading,
    error,
    tokenRefreshError,
    isAuthenticated,
    login,
    logout,
    getAccessToken,
    forceRefreshToken,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

// Custom hook for using the auth context
export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

export default AuthContext;