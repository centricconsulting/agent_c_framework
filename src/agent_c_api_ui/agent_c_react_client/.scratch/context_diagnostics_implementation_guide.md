# Context Diagnostics Implementation Guide

## Overview

This guide explains how to implement the enhanced context diagnostics system to help us diagnose and resolve the application loading issues. The diagnostics system provides detailed tracking of context initialization, state changes, errors, and performance metrics.

## Implementation Steps

### 1. Add the Diagnostic Utility to the Project

First, add the diagnostic utility functions to your project's existing `diagnostic.js` file:

```javascript
// Add these functions to src/lib/diagnostic.js

/**
 * Creates and initializes the context diagnostics system.
 * This should be called at application startup before any context providers.
 */
export function initializeContextDiagnostics() {
  // Implementation from context_diagnostic_implementation.js
}

/**
 * Creates a diagnostic wrapper for a context provider function.
 */
export function withContextDiagnostics(contextName, providerFunction) {
  // Implementation from context_diagnostic_implementation.js
}

/**
 * Creates an error boundary component specifically for context providers.
 */
export function ContextErrorBoundary({ contextName, children, fallback }) {
  // Implementation from context_diagnostic_implementation.js
}

/**
 * Logs detailed context state changes for debugging.
 */
export function logContextStateChange(contextName, prevState, nextState) {
  // Implementation from context_diagnostic_implementation.js
}

/**
 * Creates a diagnostic wrapper for useEffect cleanup functions
 */
export function trackContextConsumer(contextName, componentName) {
  // Implementation from context_diagnostic_implementation.js
}
```

### 2. Initialize Diagnostics in App.jsx

Update your `App.jsx` file to initialize the diagnostics system before any context providers are rendered:

```jsx
import { initializeContextDiagnostics } from '@/lib/diagnostic';

// Initialize diagnostics at the very start of the application
if (typeof window !== 'undefined' && !window.__CONTEXT_DIAGNOSTICS) {
  initializeContextDiagnostics();
}

function App() {
  // Existing app code...
}
```

### 3. Update Each Context Provider

Update each context provider to use the diagnostic utilities. Here's how to modify each one:

#### ThemeContext.jsx

See the full example in `enhanced_theme_context_example.jsx`. Key changes include:

- Recording context initialization start/success/error
- Logging state changes
- Adding error boundaries
- Tracking when themes are applied

#### AuthContext.jsx

Make similar changes to the AuthContext:

```jsx
// In the initialization code
if (window.__CONTEXT_DIAGNOSTICS) {
  window.__CONTEXT_DIAGNOSTICS.recordContextStart('auth');
}

// When initialization completes successfully
if (window.__CONTEXT_DIAGNOSTICS) {
  window.__CONTEXT_DIAGNOSTICS.recordContextSuccess('auth', { 
    sessionId, 
    isAuthenticated: !!sessionId 
  });
}

// When errors occur
if (window.__CONTEXT_DIAGNOSTICS) {
  window.__CONTEXT_DIAGNOSTICS.recordContextError('auth', error);
}
```

#### ModelContext.jsx and SessionContext.jsx

Apply the same pattern to the other contexts, making sure to:

1. Record the start of initialization
2. Record successful initialization with relevant state
3. Record any errors that occur
4. Log important state changes

### 4. Update Context Hooks

Enhance each context hook to track when components consume the contexts:

```jsx
export function useTheme(componentName = 'UnknownComponent') {
  const logger = useLogger();
  const context = useContext(ThemeContext);
  
  // Track this component as a consumer of ThemeContext
  useEffect(() => {
    return trackContextConsumer('theme', componentName);
  }, [componentName]);
  
  if (context === null) {
    logger.error(`useTheme must be used within a ThemeProvider`, null, componentName);
    throw new Error(`useTheme must be used within a ThemeProvider`);
  }
  
  return context;
}
```

### 5. Add Error Boundaries to App.jsx

Wrap each context provider with an error boundary in App.jsx:

```jsx
import { ContextErrorBoundary } from '@/lib/diagnostic';

function App() {
  return (
    <ContextErrorBoundary 
      contextName="theme"
      fallback={(error) => (
        // Light theme fallback UI
        <div className="theme-error-fallback">
          <h3>Theme Error</h3>
          <p>{error.message}</p>
          <button onClick={() => window.location.reload()}>Reload</button>
        </div>
      )}
    >
      <ThemeProvider>
        <ContextErrorBoundary contextName="auth">
          <AuthProvider>
            <ContextErrorBoundary contextName="model">
              <ModelProvider>
                <ContextErrorBoundary contextName="session">
                  <SessionProvider>
                    {/* App content */}
                  </SessionProvider>
                </ContextErrorBoundary>
              </ModelProvider>
            </ContextErrorBoundary>
          </AuthProvider>
        </ContextErrorBoundary>
      </ThemeProvider>
    </ContextErrorBoundary>
  );
}
```

### 6. Using the Diagnostics

Once implemented, you can access the diagnostics in the browser console:

```javascript
// Get a summary of context initialization
window.getContextDiagnostics();

// Access the full diagnostic object
window.__CONTEXT_DIAGNOSTICS;

// See all events in sequence
window.__CONTEXT_DIAGNOSTICS.sequence.events;

// Check for errors
window.__CONTEXT_DIAGNOSTICS.errors;
```

## Debugging Loading Issues

When the application fails to load, you can examine the diagnostics to identify exactly where the problem occurs:

1. Look for context initialization failures in `window.__CONTEXT_DIAGNOSTICS.errors`
2. Check the initialization sequence to see which context was initializing when the problem occurred
3. Look for circular dependencies by examining the sequence of context consumer mounting
4. Check for race conditions where a context is accessed before it's fully initialized

## Common Issues and Solutions

### 1. Context Not Initialized

**Symptom**: Error "useX must be used within a XProvider"

**Diagnostic**: Check sequence events to see if provider mounted before consumer

**Solution**: Ensure provider is higher in the component tree than the consumer

### 2. Circular Dependencies

**Symptom**: Infinite render loops or unexpected context values

**Diagnostic**: Examine sequence.events for unusual patterns of context initialization

**Solution**: Use hooks to access contexts instead of direct imports

### 3. Async Initialization Race Conditions

**Symptom**: Context appears to be initialized but has default/empty values

**Diagnostic**: Check timing between context success and first consumer mount

**Solution**: Add loading states and prevent access until initialization completes

## Next Steps

After implementing the diagnostics system:

1. Restart the application and check the console for diagnostic information
2. Identify the specific issue causing the loading problem
3. Fix the identified issue
4. Verify all contexts initialize properly in the correct order
5. Test all components to ensure they function correctly