# SessionContext Migration Steps

## Overview

This document outlines the step-by-step process for migrating from the current overloaded SessionContext to the newly refactored version that focuses solely on chat-specific functionality.

## Migration Steps

### Step 1: Replace SessionContext.jsx

1. Back up the current SessionContext.jsx file
   ```bash
   cp //ui/src/contexts/SessionContext.jsx //ui/.scratch/trash/SessionContext.jsx.backup
   ```

2. Replace with the refactored version
   ```bash
   cp //ui/.scratch/session_context_refactored.jsx //ui/src/contexts/SessionContext.jsx
   ```

### Step 2: Update use-session-context.jsx

1. Back up the current use-session-context.jsx file
   ```bash
   cp //ui/src/hooks/use-session-context.jsx //ui/.scratch/trash/use-session-context.jsx.backup
   ```

2. Replace with the refactored version
   ```bash
   cp //ui/.scratch/use-session-context.jsx //ui/src/hooks/use-session-context.jsx
   ```

### Step 3: Update Components Using SessionContext

1. Update ChatInterface.jsx to use the appropriate contexts
   - Modify imports to include useAuth and useModel hooks
   - Update useContext(SessionContext) call to use multiple contexts
   - Verify it uses the appropriate context for each piece of state/functionality

2. Update SidebarCommandMenu.jsx to use the appropriate contexts
   - Modify imports to include useAuth and useModel hooks
   - Update useContext(SessionContext) call to use multiple contexts
   - Verify it uses the appropriate context for each piece of state/functionality

### Step 4: Test Functionality

1. Run the app locally and test all critical functionality:
   - Authentication flow
   - Model selection
   - Theme switching
   - Persona selection
   - Tool equipping
   - Message streaming
   - Chat export

2. Check console for errors and verify logging is working as expected

3. Verify integration with other contexts is working properly

### Step 5 (Optional): Add Enhanced Logging

1. Add more detailed logging to SessionContext for better debugging
   - Log state changes
   - Track API call performance
   - Log context initialization steps

2. Use the logger's diagnostic capabilities to validate the refactoring

## Before/After Component Usage Examples

### Before (using only SessionContext)

```jsx
import { useContext } from 'react';
import { SessionContext } from '@/contexts/SessionContext';

function MyComponent() {
  const {
    sessionId,          // From AuthContext now
    modelName,          // From ModelContext now
    theme,              // From ThemeContext now
    messages,           // Still from SessionContext
    activeTools,        // Still from SessionContext
    handleEquipTools,   // Still from SessionContext
    handleThemeChange,  // From ThemeContext now
  } = useContext(SessionContext);
  
  // Component logic...
}
```

### After (using multiple contexts)

```jsx
import { useAuth } from '@/hooks/use-auth';
import { useModel } from '@/hooks/use-model';
import { useTheme } from '@/hooks/use-theme';
import { useSessionContext } from '@/hooks/use-session-context';

function MyComponent() {
  const { sessionId } = useAuth('MyComponent');
  const { modelName } = useModel('MyComponent');
  const { theme, setTheme } = useTheme();
  const { 
    messages, 
    activeTools, 
    handleEquipTools 
  } = useSessionContext('MyComponent');
  
  // Component logic...
}
```

## Rollback Procedure

If issues are encountered, follow these steps to roll back:

1. Restore the original SessionContext.jsx
   ```bash
   cp //ui/.scratch/trash/SessionContext.jsx.backup //ui/src/contexts/SessionContext.jsx
   ```

2. Restore the original use-session-context.jsx
   ```bash
   cp //ui/.scratch/trash/use-session-context.jsx.backup //ui/src/hooks/use-session-context.jsx
   ```

3. Revert any changes made to components

4. Document the issues encountered for future resolution