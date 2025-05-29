// Microsoft Authentication Library (MSAL) configuration
export const msalConfig = {
  auth: {
    clientId: import.meta.env.VITE_AZURE_CLIENT_ID,
    authority: `https://login.microsoftonline.com/${import.meta.env.VITE_AZURE_TENANT_ID}`,
    redirectUri: window.location.origin,
  },
  cache: {
    cacheLocation: 'localStorage',
    storeAuthStateInCookie: false,
  },
};

// Login request scopes
export const loginRequest = {
  scopes: ['openid', 'profile', 'User.Read'],
};

// Optional: Configure additional API scopes as needed
export const apiScopes = {
  agentC: ['api://agent-c/access']
};

// Configuration for checking authentication on app start
export const authConfig = {
  // Set to true to force authentication on app start
  requireAuth: true,
  // Default redirect path after successful login if no specific destination
  defaultRedirectPath: '/chat',
  // Path to redirect to if not authenticated
  loginPath: '/login',
  // Token refresh configuration
  tokenRefresh: {
    // Refresh token when it has less than 5 minutes remaining (300 seconds)
    refreshThreshold: 300,
    // Minimum time between refresh attempts (in seconds)
    minRefreshInterval: 30,
    // Maximum retry attempts for failed refreshes
    maxRetryAttempts: 3,
    // Debug mode for extra logging (set to true to enable refresh logging)
    debug: false,
    // Whether to force logout when refresh attempts are exhausted
    logoutOnRefreshExhaustion: true
  }
};
