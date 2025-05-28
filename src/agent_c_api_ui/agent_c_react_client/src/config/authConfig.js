/**
 * Authentication configuration for Microsoft SSO integration
 */

// These values should match the backend configuration
// In production, these should be environment variables
export const msalConfig = {
  auth: {
    clientId: '4f531e8c-e1f7-4a71-b6a5-6f91e1930d26',
    authority: 'https://login.microsoftonline.com/d6f8cc30-debb-41a6-9c78-0516c185fa0d',
    redirectUri: window.location.origin,
  },
  cache: {
    cacheLocation: 'localStorage',
    storeAuthStateInCookie: false,
  },
};

export const loginRequest = {
  scopes: ['openid', 'profile', 'User.Read'],
};

// Routes that require authentication
export const protectedRoutes = [
  '/chat',
  '/rag',
  '/settings',
  '/interactions',
  '/replay',
];

// Routes that are public (don't require authentication)
export const publicRoutes = [
  '/',
  '/home',
  '/login',
];