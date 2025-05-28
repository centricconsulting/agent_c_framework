# Microsoft SSO Integration for Agent C

## Overview

This document outlines the implementation of Microsoft Single Sign-On (SSO) authentication for the Agent C application. The integration ensures that when the Agent C application is launched, users are first presented with a login page and are only redirected to the chat page after successful authentication.

## Implementation Details

### Frontend Implementation

1. **Authentication Context**
   - Enhanced `AuthContext` in `src/contexts/AuthContext.jsx` to handle Microsoft authentication
   - Added token validation with backend API
   - Registered token provider for global access

2. **Auth Configuration**
   - Updated `authConfig.js` to include authentication settings
   - Added configuration for requiring authentication on app start

3. **Route Protection**
   - Using `ProtectedRoute` component to secure all application routes
   - Redirects unauthenticated users to login page
   - Preserves intended destination for post-login redirect

4. **API Authentication**
   - Created global auth helper in `src/lib/auth-helper.js`
   - Updated API service to include authentication tokens in all requests

### Backend Implementation

1. **Token Verification**
   - Created `auth.py` in API core module to handle token verification
   - Implemented JWT validation using Microsoft identity platform

2. **Auth Endpoints**
   - Added `/api/v2/auth/me` endpoint to get user information
   - Added `/api/v2/auth/validate` endpoint to validate tokens

3. **Configuration**
   - Updated environment configuration to include authentication settings

## Flow

1. User accesses Agent C application
2. `AuthProvider` checks for existing authenticated session
3. If not authenticated, user is redirected to `/login`
4. User authenticates with Microsoft account
5. After successful authentication, token is validated with backend
6. User is redirected to intended destination or default chat page

## Testing

To test the authentication flow:

1. Ensure backend API is running
2. Launch the frontend application
3. Verify redirection to login page
4. Complete Microsoft authentication
5. Verify redirection to chat page
6. Test token refresh by keeping the page open for extended period

## Configuration

### Frontend Configuration

In `src/config/authConfig.js`:

```javascript
export const authConfig = {
  // Set to true to force authentication on app start
  requireAuth: true,
  // Default redirect path after successful login if no specific destination
  defaultRedirectPath: '/chat',
  // Path to redirect to if not authenticated
  loginPath: '/login'
};
```

### Backend Configuration

In `.env` file:

```
# Azure AD Authentication
AZURE_CLIENT_ID=4f531e8c-e1f7-4a71-b6a5-6f91e1930d26
AZURE_TENANT_ID=d6f8cc30-debb-41a6-9c78-0516c185fa0d
```