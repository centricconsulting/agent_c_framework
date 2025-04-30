# SessionContext Refactor Phase 2 - Debugging Guide

This guide provides instructions for debugging common issues that might arise during Phase 2 of the SessionContext refactoring.

## Debugging Setup

To help debug issues with the new SessionContext architecture, we'll add logging throughout the code. This will help us track the flow of data and identify where problems might be occurring.

### Add Debug Logging to SessionContext

When implementing the new SessionContext, include console logging for key operations:

```jsx
// Add this at the top of SessionContext.jsx
const DEBUG = true; // Set to false to disable logging in production

function logDebug(...args) {
  if (DEBUG) {
    console.log('[SessionContext]', ...args);
  }
}

// Use throughout the component
const initializeSession = useCallback(async (config) => {
  setIsReady(false);
  logDebug('Initializing session with config:', config);
  try {
    // ... implementation ...
    logDebug(`Session initialized with ID ${data.ui_session_id}`);
  } catch (err) {
    logDebug('Session initialization failed:', err);
    // ... error handling ...
  }
}, []);
```

### Add Debug Component

Create a debug component that can be conditionally rendered to show context state:

```jsx
// src/components/SessionDebugger.jsx
import React from 'react';
import { useSession } from '../hooks/useSession';

export function SessionDebugger() {
  const session = useSession();
  
  if (process.env.NODE_ENV === 'production') {
    return null; // Don't render in production
  }
  
  return (
    <div className="fixed bottom-4 right-4 bg-black/80 text-white p-4 rounded-lg text-xs max-w-md max-h-80 overflow-auto z-50">
      <h3 className="font-bold mb-2">Session Context Debug</h3>
      <pre className="whitespace-pre-wrap">
        {JSON.stringify({
          sessionId: session.sessionId,
          isReady: session.isReady,
          isInitialized: session.isInitialized,
          error: session.error,
          isValidating: session.isValidating
        }, null, 2)}
      </pre>
    </div>
  );
}
```

## Common Issues and Solutions

### 1. Session Initialization Issues

**Symptoms:**
- Application fails to initialize a session
- Error messages related to session initialization
- Blank screen or loading spinner that never completes

**Debugging Steps:**

1. Check browser console for error messages
2. Verify network requests to `/initialize` endpoint
3. Inspect the request payload being sent to the API
4. Check the response from the API for error messages

**Solutions:**

- Verify the API service is correctly configured
- Check that the sessionId is being properly stored and retrieved
- Ensure config parameters are correctly formatted
- Verify the `initializeSession` function in SessionContext is being called

### 2. Context Provider Ordering Issues

**Symptoms:**
- React context errors ("cannot read property X of undefined")
- Components fail to render
- Errors about hooks being called out of order

**Debugging Steps:**

1. Check browser console for React errors
2. Verify provider nesting order in App.jsx
3. Check that LegacySessionProvider has access to SessionContext

**Solutions:**

- Ensure SessionProvider is higher in the tree than LegacySessionProvider
- Verify all imports are correct
- Check that useSession is being called within SessionProvider

### 3. Duplicate State Management Issues

**Symptoms:**
- State changes in one context not reflected in the other
- Inconsistent UI updates
- Functions seem to have no effect

**Debugging Steps:**

1. Add logging to track state changes in both contexts
2. Use React DevTools to inspect context values
3. Trace the flow of state updates between contexts

**Solutions:**

- Ensure LegacySessionContext properly forwards state changes to/from SessionContext
- Remove duplicate state management
- Add useEffect dependencies to react to changes

### 4. Session Restoration Issues

**Symptoms:**
- Saved session not being restored
- Application creates a new session on every load
- localStorage values not being used

**Debugging Steps:**

1. Check localStorage in browser devtools
2. Add logging to session restoration logic
3. Verify session validation calls

**Solutions:**

- Ensure localStorage keys match exactly
- Add session validation with proper error handling
- Verify the useEffect dependency array includes necessary functions

### 5. API Call Issues

**Symptoms:**
- API calls failing
- Network errors
- Timeout issues

**Debugging Steps:**

1. Check network tab for API calls
2. Verify request headers and payload
3. Check response status and content

**Solutions:**

- Ensure API services are correctly imported and used
- Verify API URL configuration
- Add proper error handling for API calls

## Validation Tests

After implementing changes, run these validation tests to verify everything works:

### 1. Session Initialization Test

1. Clear localStorage (delete ui_session_id)
2. Reload the application
3. Verify a new session is created
4. Check localStorage for the new session ID
5. Verify both contexts have the same session ID

### 2. Session Persistence Test

1. Initialize a session
2. Reload the application
3. Verify the same session is restored
4. Check that no new API calls to initialize are made

### 3. Error Handling Test

1. Modify code temporarily to cause an API error
2. Attempt to initialize a session
3. Verify error is displayed to the user
4. Verify application recovers gracefully

### 4. Context Integration Test

1. Change a setting in the UI (model, parameters, etc.)
2. Verify the change is reflected in both contexts
3. Reload and verify the change persists

## Performance Monitoring

Add performance marks to track timing of critical operations:

```jsx
const initializeSession = useCallback(async (config) => {
  performance.mark('session-initialize-start');
  // ... implementation ...
  performance.mark('session-initialize-end');
  performance.measure(
    'session-initialization',
    'session-initialize-start',
    'session-initialize-end'
  );
}, []);
```

You can view these measurements in the browser's Performance tab to identify bottlenecks.

## Emergency Rollback

If critical issues are encountered and cannot be resolved quickly:

1. Revert App.jsx to use only the original SessionProvider
2. Restore the original SessionContext.jsx
3. Remove the new files
4. Commit the rollback and deploy

This will restore the application to its pre-refactor state while you diagnose and fix the issues.