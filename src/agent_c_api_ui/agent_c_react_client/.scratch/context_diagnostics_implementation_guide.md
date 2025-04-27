# Context Diagnostics Implementation Guide

## Overview

This document provides an overview of the context diagnostics system, which is designed to track and diagnose issues with React context initialization and app state management.

## How the Diagnostic System Works

1. **Global Tracking Object**: The system uses `window.__CONTEXT_DIAGNOSTIC` to store context initialization state.

2. **Initialization Tracking**: Each context uses these functions to report its status:
   - `trackContextInitialization(contextName, 'start')` - When a context begins initializing
   - `trackContextInitialization(contextName, 'update', data)` - For progress updates
   - `trackContextInitialization(contextName, 'error', {error: 'message'})` - For errors
   - `trackContextInitialization(contextName, 'complete', data)` - When initialization finishes

3. **Global Completion Signal**: `completeContextInitialization(success, data)` signals that all contexts are ready.

## Using Diagnostic Tools

### From the Browser Console

1. **Get Full Diagnostic Report**:
   ```javascript
   window.getContextDiagnostics()
   // or
   window.diagnosticReport()
   ```

2. **Check Chat Interface Visibility Issues**:
   ```javascript
   window.checkChatVisibility()
   ```

3. **Get Model Information**:
   ```javascript
   window.getModelInfo()
   ```

4. **Check If a Specific Context Is Mounted**:
   ```javascript
   window.checkContextMounted('SessionContext')
   ```

5. **Get Raw Context Data**:
   ```javascript
   window.getContextData()
   ```

### Common Issues and Solutions

1. **Chat Interface Not Displaying**:
   - Check if `SessionContext` and `ModelContext` are both initialized
   - Verify that `sessionId` exists and `isInitialized` is true
   - Use `window.checkChatVisibility()` to diagnose

2. **Missing Models**:
   - Use `window.getModelInfo()` to see model state
   - Check if emergency fallback model was used

3. **Context Initialization Stuck**:
   - Look for errors in specific contexts
   - Check the initialization order for dependency issues

## Adding Tracking to New Contexts

When creating a new context, add these tracking calls:

```javascript
import { trackContextInitialization } from '@/lib/diagnostic';

export const MyNewContext = createContext();

export const MyNewProvider = ({ children }) => {
  // 1. Track start of initialization
  trackContextInitialization('MyNewContext', 'start');
  
  // Setup state, functions, etc.
  
  // 2. Track initialization progress/updates
  useEffect(() => {
    // Your initialization code
    trackContextInitialization('MyNewContext', 'update', { 
      someProgress: true 
    });
    
    // 3. Track completion
    trackContextInitialization('MyNewContext', 'complete', {
      success: true,
      importantData: data
    });
  }, []);
  
  return (
    <MyNewContext.Provider value={/* your value */}>
      {/* Diagnostic element for DOM checking */}
      <div data-my-new-provider="mounted" style={{ display: 'none' }}>
        MyNewProvider diagnostic element
      </div>
      {children}
    </MyNewContext.Provider>
  );
};
```

## Maintenance Notes

1. **Resetting Diagnostics**: Use `window.clearContextDiagnostics()` to reset tracking during development.

2. **Error Patterns**: Common error patterns to look for:
   - Contexts with `start` but no `complete` signal
   - Long gaps between `start` and `complete` (> 2000ms)
   - Multiple error signals in the same context

3. **Improving Resilience**: All contexts should implement fallback mechanisms:
   - Default/emergency data when API requests fail
   - Clear initialization signals even when errors occur
   - Appropriate user feedback for initialization failures

## Future Improvements

1. **Timeline Visualization**: Add a visual timeline of context initialization
2. **Automated Dependency Detection**: Auto-detect context dependencies
3. **Periodic Health Checks**: Regular checks of context health during app runtime
4. **Expanded Self-Healing**: More automatic recovery mechanisms for common failures

## Common Troubleshooting Commands

```javascript
// Full system diagnostic
window.getContextDiagnostics()

// Check if contexts appear properly mounted
window.checkContextMounted('ThemeContext')
window.checkContextMounted('AuthContext')
window.checkContextMounted('ModelContext')
window.checkContextMounted('SessionContext')

// Check why chat isn't visible
window.checkChatVisibility()

// Reset diagnostic tracking (developer tool)
window.clearContextDiagnostics()
```