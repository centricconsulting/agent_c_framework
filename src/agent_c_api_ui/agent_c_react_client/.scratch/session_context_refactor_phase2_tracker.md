# SessionContext Refactor - Phase 2 Tracker

This document tracks the progress of Phase 2 of the SessionContext refactoring project: Creating a Core SessionContext.

## Overview

In Phase 2, we're extracting core session management functionality from the monolithic SessionContext into a new, focused context. This will provide a foundation for the additional contexts we'll create in subsequent phases.

## Current Status

- Phase 1 (API Service Layer) is complete - Services are implemented and SessionContext is using them
- We're now starting Phase 2 - Creating a focused SessionContext for core session management

## Phase 2 Tasks Breakdown

### 1. Create New SessionContext

Create a focused SessionContext with only session-related functionality.

- [ ] Create SessionContext.jsx with core state (sessionId, isReady, isInitialized, error)
- [ ] Implement core functions (initializeSession, handleSessionsDeleted)
- [ ] Add session validation for stored sessionIds
- [ ] Add proper error recovery mechanisms
- [ ] Create useSession hook for easy access to session context

### 2. Create LegacySessionContext

Rename and refactor the existing context to use the new SessionContext internally.

- [ ] Move current SessionContext.jsx to LegacySessionContext.jsx
- [ ] Update LegacySessionContext to use new SessionContext internally
- [ ] Ensure backward compatibility for all existing components
- [ ] Add logging/monitoring for context usage statistics

### 3. Update Application Entry Point

Update the global context providers to include both contexts during transition.

- [ ] Update App.jsx to include both SessionProvider and LegacySessionProvider
- [ ] Ensure correct context nesting and initialization order

### 4. Testing and Validation

Ensure the application works with both contexts.

- [ ] Test session initialization
- [ ] Test session deletion
- [ ] Test session restoration from localStorage
- [ ] Test error handling and recovery
- [ ] Validate that all existing functionality works correctly

## Implementation Notes

### SessionContext Design

```jsx
// src/contexts/SessionContext.jsx (new version)
import React, { createContext, useState, useEffect } from 'react';
import { session as sessionService } from '../services';

export const SessionContext = createContext();

export const SessionProvider = ({ children }) => {
  // Core session state only
  const [sessionId, setSessionId] = useState(null);
  const [isReady, setIsReady] = useState(false);
  const [isInitialized, setIsInitialized] = useState(false);
  const [error, setError] = useState(null);

  // Core session functions
  const initializeSession = async (config) => {
    setIsReady(false);
    try {
      const data = await sessionService.initialize(config);
      if (data.ui_session_id) {
        localStorage.setItem("ui_session_id", data.ui_session_id);
        setSessionId(data.ui_session_id);
        setIsReady(true);
        setError(null);
        return data.ui_session_id;
      } else {
        throw new Error("No ui_session_id in response");
      }
    } catch (err) {
      setIsReady(false);
      setError(`Session initialization failed: ${err.message}`);
      throw err;
    }
  };

  const handleSessionsDeleted = () => {
    localStorage.removeItem("ui_session_id");
    setSessionId(null);
    setIsReady(false);
    setError(null);
  };

  // Check for existing session on mount
  useEffect(() => {
    const savedSessionId = localStorage.getItem("ui_session_id");
    if (savedSessionId) {
      setSessionId(savedSessionId);
      setIsReady(true);
    }
  }, []);

  return (
    <SessionContext.Provider
      value={{
        sessionId,
        isReady,
        isInitialized,
        setIsInitialized,
        error,
        setError,
        initializeSession,
        handleSessionsDeleted
      }}
    >
      {children}
    </SessionContext.Provider>
  );
};
```

### useSession Hook Design

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

## Potential Challenges

1. **Initialization Order**: Ensure proper initialization order between contexts
2. **Session Validation**: Add proper validation for stored sessionIds
3. **Error Recovery**: Implement robust error recovery mechanisms
4. **State Inconsistency**: Prevent state inconsistency between contexts

## Success Criteria

- Core session management logic is extracted to a focused SessionContext
- LegacySessionContext provides backward compatibility
- All existing functionality works correctly
- Error handling is improved and consistent
- Application can correctly recover from session errors

## Status Updates

| Date | Status | Notes |
|------|--------|-------|
| 2025-04-30 | Not Started | Phase 2 planning complete, ready to begin implementation |