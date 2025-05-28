import React, { createContext, useContext, useState, useEffect } from 'react';
import { PublicClientApplication } from '@azure/msal-browser';
import { msalConfig, loginRequest, authConfig } from '../config/authConfig';
import axios from 'axios';
import { registerTokenProvider } from '../lib/auth-helper';

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

  // Get access token for API calls
  const getAccessToken = async () => {
    try {
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
        forceRefresh: false
      };
      
      const response = await msalInstance.acquireTokenSilent(silentRequest);
      return response.accessToken;
    } catch (err) {
      // If silent token acquisition fails, try interactive method
      if (err.name === 'InteractionRequiredAuthError') {
        try {
          const response = await msalInstance.acquireTokenPopup(loginRequest);
          return response.accessToken;
        } catch (interactiveErr) {
          console.error('Interactive token acquisition failed:', interactiveErr);
          // Return null instead of throwing
          return null;
        }
      } else {
        console.error('Token acquisition failed:', err);
        // Return null instead of throwing
        return null;
      }
    }
  };
  
  // Register the token provider for global access
  useEffect(() => {
    // Register our getAccessToken method as the global token provider
    registerTokenProvider(getAccessToken);
    
    return () => {
      // Reset to default (null provider) when component unmounts
      registerTokenProvider(async () => null);
    };
  }, []);
  

  // Context value
  const value = {
    user,
    msalReady,
    loading,
    error,
    isAuthenticated,
    login,
    logout,
    getAccessToken,
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