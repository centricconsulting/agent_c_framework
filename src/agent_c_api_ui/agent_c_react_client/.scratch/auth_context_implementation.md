# AuthContext Implementation

## Overview

The `AuthContext` provides authentication and session management functionality for the Agent C React UI. It handles session initialization, storage, and related API operations in a centralized and consistent way.

## Key Features

- Session initialization and management
- Authentication state tracking
- Secure session storage
- Error handling for auth-related operations
- Integration with API service for auth-related requests

## Usage

### Context Provider

The `AuthProvider` should be placed near the root of your component tree, but after `ThemeProvider`:

```jsx
// App.jsx
import { AuthProvider } from '@/contexts/AuthContext';

function App() {
  return (
    <ThemeProvider>
      <AuthProvider>
        {/* Other providers and app content */}
      </AuthProvider>
    </ThemeProvider>
  );
}
```

### Using the Hook

In your components, use the `useAuth` hook to access authentication functionality:

```jsx
import { useAuth } from '@/hooks/use-auth';

function MyComponent() {
  const { 
    sessionId, 
    isAuthenticated, 
    isInitializing,
    authError,
    initializeSession,
    logout,
    checkAuthError
  } = useAuth('MyComponent'); // Component name for logging
  
  // Use auth functionality here
  
  return (
    // Your component JSX
  );
}
```

## API Reference

### State

- `sessionId` (string|null): The current session ID, or null if not authenticated
- `isAuthenticated` (boolean): Whether the user is currently authenticated
- `isInitializing` (boolean): Whether authentication is currently initializing
- `authError` (string|null): Any error that occurred during authentication

### Methods

#### `initializeSession(sessionConfig, forceNew = false)`

Initializes a new session with the API or reuses an existing one.

- `sessionConfig` (Object): Configuration for the new session
- `forceNew` (boolean): Whether to force creation of a new session, even if one exists
- Returns: Promise<string|null> - The session ID if successful, null otherwise

#### `logout()`

Ends the current session and clears authentication state.

#### `checkAuthError(response)`

Checks if a fetch response contains authentication errors.

- `response` (Response): Fetch API response object
- Returns: Promise<boolean> - True if authentication error detected

## Implementation Details

- Uses `localStorage` (via storageService) to persist the session ID
- Handles initialization timeouts to prevent hanging
- Centralizes authentication error handling
- Uses our apiService for API interactions

## Error Handling

The context provides robust error handling:

- Initialization errors are captured and exposed via `authError`
- Timeouts are implemented to prevent hanging during initialization
- API errors are properly caught and logged