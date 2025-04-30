# SessionContext Refactor Phase 2 Implementation Guide

This document provides step-by-step instructions for implementing Phase 2 of the SessionContext refactoring.

## Step 1: Create the New SessionContext

Create a focused SessionContext that handles only core session management.

```jsx
// src/contexts/SessionContext.jsx.new
import React, { createContext, useState, useEffect } from 'react';
import { session as sessionService } from '../services';

export const SessionContext = createContext();

export const SessionProvider = ({ children }) => {
  // Core session state only
  const [sessionId, setSessionId] = useState(null);
  const [isReady, setIsReady] = useState(false);
  const [isInitialized, setIsInitialized] = useState(false);
  const [error, setError] = useState(null);

  // Initialize a session with the provided configuration
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

  // Clear session when sessions are deleted
  const handleSessionsDeleted = () => {
    localStorage.removeItem("ui_session_id");
    setSessionId(null);
    setIsReady(false);
    setError(null);
  };

  // Validate session by checking with backend
  const validateSession = async (sessionId) => {
    try {
      await sessionService.getSession(sessionId);
      return true;
    } catch (err) {
      console.error("Session validation failed:", err);
      return false;
    }
  };

  // Check for existing session on mount
  useEffect(() => {
    const checkExistingSession = async () => {
      const savedSessionId = localStorage.getItem("ui_session_id");
      if (savedSessionId) {
        try {
          // Optional: validate session with backend
          const isValid = await validateSession(savedSessionId);
          if (isValid) {
            setSessionId(savedSessionId);
            setIsReady(true);
          } else {
            // Session no longer valid on backend
            localStorage.removeItem("ui_session_id");
            setSessionId(null);
            setIsReady(false);
          }
        } catch (err) {
          // Error during validation, clear session
          localStorage.removeItem("ui_session_id");
          setError(`Session validation failed: ${err.message}`);
          setSessionId(null);
          setIsReady(false);
        }
      }
    };

    checkExistingSession();
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
        handleSessionsDeleted,
        validateSession
      }}
    >
      {children}
    </SessionContext.Provider>
  );
};
```

## Step 2: Create useSession Hook

Implement a custom hook for accessing the SessionContext.

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

## Step 3: Rename Current SessionContext to LegacySessionContext

1. Move the current SessionContext to a new file:

```jsx
// src/contexts/LegacySessionContext.jsx
import React, { createContext, useState, useEffect, useRef, useContext } from 'react';
import { SessionContext } from './SessionContext';

// Import services from the API service layer
import {
    api,
    session as sessionService,
    model as modelService,
    tools as toolsService,
    persona as personaService,
    showErrorToast
} from '../services';

export const LegacySessionContext = createContext();

export const LegacySessionProvider = ({ children }) => {
    // Use the core SessionContext
    const {
        sessionId, 
        isReady, 
        error: sessionError, 
        setError: setSessionError,
        initializeSession: coreInitializeSession,
        handleSessionsDeleted: coreHandleSessionsDeleted
    } = useContext(SessionContext);
    
    // Legacy state that will be moved to other contexts later
    const debouncedUpdateRef = useRef(null);
    const [error, setError] = useState(null);
    const [settingsVersion, setSettingsVersion] = useState(0);
    const [isOptionsOpen, setIsOptionsOpen] = useState(false);
    const [isStreaming, setIsStreaming] = useState(false);
    const [isLoading, setIsLoading] = useState(true);
    const [isInitialized, setIsInitialized] = useState(false);
    const [theme, setTheme] = useState(() => {
        // Check for saved theme preference in localStorage
        const savedTheme = localStorage.getItem('theme');
        return savedTheme || 'system';
    });

    // Agent settings & configuration
    const [persona, setPersona] = useState("");
    const [temperature, setTemperature] = useState(null);
    const [customPrompt, setCustomPrompt] = useState("");
    const [modelConfigs, setModelConfigs] = useState([]);
    const [modelName, setModelName] = useState("");
    const [modelParameters, setModelParameters] = useState({});
    const [selectedModel, setSelectedModel] = useState(null);

    // Data from backend
    const [personas, setPersonas] = useState([]);
    const [availableTools, setAvailableTools] = useState({
        essential_tools: [],
        groups: {},
        categories: []
    });
    const [activeTools, setActiveTools] = useState([]);

    // Synchronize error states
    useEffect(() => {
        if (sessionError) {
            setError(sessionError);
        }
    }, [sessionError]);

    // Synchronize ready state
    useEffect(() => {
        if (!isReady) {
            // If core session is not ready, set initialized to false
            setIsInitialized(false);
        }
    }, [isReady]);
    
    // Rest of current SessionContext implementation...
    // (Include all other methods and effects from the current SessionContext)
    
    // Modify initializeSession to use the core session context's method
    const initializeSession = async (forceNew = false, initialModel = null, modelConfigsData = null) => {
        try {
            // First, validate that we have model configurations available
            const models = modelConfigsData || modelConfigs;
            if (!models || models.length === 0) {
                throw new Error("No model configurations available");
            }

            // Determine which model to use
            let currentModel = null; // Initialize as null to ensure proper checking

            // Build session configuration (similar to current implementation)
            // ...

            // Use core initializeSession instead of direct API call
            await coreInitializeSession(sessionConfig);
            
            // Update modelName state to reflect the current model
            setModelName(currentModel.id);
            setSelectedModel(currentModel);
        } catch (err) {
            console.error("Session initialization failed:", err);
            setError(`Session initialization failed: ${err.message}`);
            showErrorToast(err, 'Session initialization failed');
        }
    };

    // Modify handleSessionsDeleted to use the core context's method
    const handleSessionsDeleted = () => {
        // Call core method first
        coreHandleSessionsDeleted();

        // Additional cleanup specific to LegacySessionContext
        localStorage.removeItem("agent_config");
        setActiveTools([]);
        setModelName("");
        setSelectedModel(null);
        setError(null);
    };
    
    // Return the complete context value
    return (
        <LegacySessionContext.Provider
            value={{
                // Forward the core session state
                sessionId,
                isReady,
                error,
                
                // Legacy state and methods
                settingsVersion,
                isOptionsOpen,
                setIsOptionsOpen,
                isStreaming,
                isLoading,
                isInitialized,
                persona,
                temperature,
                customPrompt,
                modelConfigs,
                modelName,
                modelParameters,
                selectedModel,
                personas,
                availableTools,
                activeTools,
                theme,
                handleThemeChange,
                fetchAgentTools,
                updateAgentSettings,
                handleEquipTools,
                handleProcessingStatus,
                handleSessionsDeleted,
                initializeSession,
                setIsInitialized
            }}
        >
            {children}
        </LegacySessionContext.Provider>
    );
};
```

## Step 4: Update App.jsx

Update the entry point to use both providers:

```jsx
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

## Step 5: Implementation Strategy

1. First, create the new files in the .scratch directory:
   - `//ui/.scratch/SessionContext.jsx.new`
   - `//ui/.scratch/useSession.js`
   - `//ui/.scratch/LegacySessionContext.jsx`

2. Review and test these files thoroughly for correctness

3. Make the actual changes:
   - Create `src/hooks/useSession.js`
   - Rename `src/contexts/SessionContext.jsx` to `src/contexts/LegacySessionContext.jsx`
   - Create new `src/contexts/SessionContext.jsx`
   - Update `src/App.jsx`

4. Test the application to ensure all functionality works correctly