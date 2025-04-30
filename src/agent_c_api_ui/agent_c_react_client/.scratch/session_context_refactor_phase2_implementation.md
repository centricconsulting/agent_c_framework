# Phase 2 Implementation Plan: Creating Core SessionContext

This document provides detailed step-by-step instructions for implementing Phase 2 of the SessionContext refactoring project. The plan is structured to provide a safe, incremental approach with clear verification points.

## Implementation Steps

### Step 1: Create the new SessionContext

1. Create the new `SessionContext.jsx` file with focused state and functionality:
   - Create the file at `src/contexts/SessionContext.jsx.new`
   - Implement core state: sessionId, isReady, isInitialized, error
   - Implement core functions: initializeSession, handleSessionsDeleted
   - Add session validation and error recovery mechanisms
   - See the design in the tracker document

2. Create a custom hook for accessing the SessionContext:
   - Create the file at `src/hooks/useSession.js`
   - Implement a hook that accesses the SessionContext and provides error checking
   - See the design in the tracker document

### Step 2: Create the LegacySessionContext

1. Duplicate the existing SessionContext:
   - Rename `src/contexts/SessionContext.jsx` to `src/contexts/LegacySessionContext.jsx`
   - Update exports to use `LegacySessionContext` and `LegacySessionProvider` names
   - Update import references within the file

2. Update LegacySessionContext to use the new SessionContext:
   - Import the new SessionContext provider and hook
   - Remove duplicated state and functions
   - Forward session state and functions to/from the SessionContext
   - Ensure it maintains the same interface for backward compatibility

### Step 3: Update App entry point

1. Replace the temporary SessionContext files:
   - Move `src/contexts/SessionContext.jsx.new` to `src/contexts/SessionContext.jsx`

2. Update the App component to use both providers:
   - Update `src/App.jsx` to nest the providers correctly
   - Ensure proper provider order (SessionProvider must be higher in the tree)

### Step 4: Test and verify

1. Test session initialization:
   - Verify a new session can be created
   - Verify session state is stored in localStorage
   - Verify both contexts have access to session data

2. Test session restoration:
   - Verify a session can be restored from localStorage
   - Verify session validation works correctly

3. Test error handling:
   - Verify initialization errors are handled correctly
   - Verify recovery mechanisms work properly

## Detailed Implementation Instructions

### Step 1.1: Create the new SessionContext

```jsx
// src/contexts/SessionContext.jsx.new
import React, { createContext, useState, useEffect, useCallback } from 'react';
import { session as sessionService } from '../services';

export const SessionContext = createContext();

export const SessionProvider = ({ children }) => {
  // Core session state only
  const [sessionId, setSessionId] = useState(null);
  const [isReady, setIsReady] = useState(false);
  const [isInitialized, setIsInitialized] = useState(false);
  const [error, setError] = useState(null);
  const [isValidating, setIsValidating] = useState(false);

  // Initialize a session with the given configuration
  const initializeSession = useCallback(async (config) => {
    setIsReady(false);
    try {
      console.log('SessionContext: Initializing session with config:', config);
      const data = await sessionService.initialize(config);
      if (data && data.ui_session_id) {
        localStorage.setItem("ui_session_id", data.ui_session_id);
        setSessionId(data.ui_session_id);
        setIsReady(true);
        setError(null);
        console.log(`SessionContext: Session initialized with ID ${data.ui_session_id}`);
        return data.ui_session_id;
      } else {
        throw new Error("No ui_session_id in response");
      }
    } catch (err) {
      console.error("SessionContext: Session initialization failed:", err);
      setIsReady(false);
      setError(`Session initialization failed: ${err.message}`);
      throw err;
    }
  }, []);

  // Validate an existing session
  const validateSession = useCallback(async (sessionId) => {
    if (!sessionId) return false;
    
    setIsValidating(true);
    try {
      console.log(`SessionContext: Validating session ${sessionId}`);
      const session = await sessionService.getSession(sessionId);
      setIsValidating(false);
      return !!session;
    } catch (err) {
      console.error("SessionContext: Session validation failed:", err);
      setIsValidating(false);
      return false;
    }
  }, []);

  // Clear session data when sessions are deleted
  const handleSessionsDeleted = useCallback(() => {
    console.log('SessionContext: Clearing session data');
    localStorage.removeItem("ui_session_id");
    setSessionId(null);
    setIsReady(false);
    setError(null);
  }, []);

  // Check for existing session on mount
  useEffect(() => {
    const savedSessionId = localStorage.getItem("ui_session_id");
    if (savedSessionId) {
      console.log(`SessionContext: Found saved session ID ${savedSessionId}`);
      // Set the session ID immediately
      setSessionId(savedSessionId);
      setIsReady(true);
      
      // Validate the session asynchronously
      validateSession(savedSessionId).then(isValid => {
        if (!isValid) {
          console.warn(`SessionContext: Saved session ${savedSessionId} is invalid`);
          handleSessionsDeleted();
        }
      });
    }
  }, [validateSession, handleSessionsDeleted]);

  const contextValue = {
    sessionId,
    isReady,
    isInitialized,
    setIsInitialized,
    error,
    setError,
    isValidating,
    initializeSession,
    handleSessionsDeleted,
    validateSession
  };

  return (
    <SessionContext.Provider value={contextValue}>
      {children}
    </SessionContext.Provider>
  );
};
```

### Step 1.2: Create useSession hook

```jsx
// src/hooks/useSession.js
import { useContext } from 'react';
import { SessionContext } from '../contexts/SessionContext';

export function useSession() {
  const context = useContext(SessionContext);
  if (context === undefined) {
    throw new Error('useSession must be used within a SessionProvider');
  }
  return context;
}
```

### Step 2.1: Rename existing SessionContext to LegacySessionContext

Create a new file `src/contexts/LegacySessionContext.jsx` based on the current SessionContext.jsx, changing the exports:

```jsx
// Beginning of the file
import React, { createContext, useState, useEffect, useRef } from 'react';

// Import services from the API service layer
import {
    api,
    session as sessionService,
    model as modelService,
    tools as toolsService,
    persona as personaService,
    showErrorToast
} from '../services';

// Change export name
export const LegacySessionContext = createContext();

// Change provider name
export const LegacySessionProvider = ({ children }) => {
    // ... existing implementation ...
    
    return (
        <LegacySessionContext.Provider
            value={{
                // ... existing values ...
            }}
        >
            {children}
        </LegacySessionContext.Provider>
    );
};
```

### Step 2.2: Update LegacySessionContext to use the new SessionContext

Modify `src/contexts/LegacySessionContext.jsx` to use the new SessionContext:

```jsx
// Near the top of the file
import { useSession } from '../hooks/useSession';

export const LegacySessionContext = createContext();

export const LegacySessionProvider = ({ children }) => {
    // Use the new SessionContext
    const {
        sessionId,
        isReady,
        error: sessionError,
        setError: setSessionError,
        initializeSession: coreInitializeSession,
        handleSessionsDeleted: coreHandleSessionsDeleted
    } = useSession();
    
    // Remove duplicated state
    // const [sessionId, setSessionId] = useState(null);
    // const [isReady, setIsReady] = useState(false);
    // Keep the rest of the state
    
    const debouncedUpdateRef = useRef(null);
    const [error, setError] = useState(null);
    // ... rest of the state ...
    
    // Use the error from SessionContext
    useEffect(() => {
        if (sessionError) {
            setError(sessionError);
        }
    }, [sessionError]);
    
    // Update the initializeSession function to use the core version
    const initializeSession = async (forceNew = false, initialModel = null, modelConfigsData = null) => {
        try {
            // ... existing model validation logic ...
            
            // Build session configuration
            const sessionConfig = { /* ... */ };
            
            // Use the core initializeSession
            await coreInitializeSession(sessionConfig);
            
            // Update local state that's not in the core context
            setModelName(currentModel.id);
            setSelectedModel(currentModel);
        } catch (err) {
            console.error("Session initialization failed:", err);
            setError(`Session initialization failed: ${err.message}`);
            showErrorToast(err, 'Session initialization failed');
        }
    };
    
    // Update handleSessionsDeleted to use the core version
    const handleSessionsDeleted = () => {
        // Call the core function
        coreHandleSessionsDeleted();
        
        // Handle additional cleanup
        localStorage.removeItem("agent_config");
        setActiveTools([]);
        setError(null);
        setModelName("");
        setSelectedModel(null);
    };
    
    // ... rest of the implementation ...
};
```

### Step 3.1: Rename the temporary file to the final location

Rename `src/contexts/SessionContext.jsx.new` to `src/contexts/SessionContext.jsx`, replacing the old file.

### Step 3.2: Update App.jsx

```jsx
// src/App.jsx
import React from 'react';
import { BrowserRouter as Router } from 'react-router-dom';
import AppRoutes from '@/Routes';
import { SessionProvider } from '@/contexts/SessionContext';
import { LegacySessionProvider } from '@/contexts/LegacySessionContext';
import { ThemeProvider } from '@/contexts/ThemeProvider';

function App() {
  return (
    <SessionProvider>
      <LegacySessionProvider>
        <ThemeProvider>
          <Router>
            <AppRoutes />
          </Router>
        </ThemeProvider>
      </LegacySessionProvider>
    </SessionProvider>
  );
}

export default App;
```

## Verification Steps

After each implementation step, verify:

1. The application builds without errors
2. The application loads correctly
3. Session initialization works
4. Session restoration works
5. All existing functionality continues to work

## Rollback Plan

If issues are encountered:

1. Revert changes to `App.jsx`
2. Restore the original `SessionContext.jsx` from the renamed file
3. Delete the new files (useSession.js, LegacySessionContext.jsx)

## Success Criteria

- The application works with both the new SessionContext and LegacySessionContext
- Session initialization, validation, and deletion work correctly
- Error handling is improved
- Code is more maintainable with clear separation of concerns