# Context Refactoring Plan

## Current Issues

1. **SessionContext is Overloaded with Responsibilities**
   - Handling authentication, model configuration, chat state, theming, file management, and more
   - Violates the single responsibility principle making the code harder to maintain

2. **Circular Dependency Pattern**
   - ThemeProvider wraps SessionProvider in App.jsx
   - Yet components using SessionContext might need theme data

3. **Theme Management is Fragmented**
   - Theme state in SessionContext
   - Theme application in ThemeProvider
   - Theme utility functions in theme.ts
   - Theme toggling scattered across components

4. **Poor Separation of Concerns**
   - API calls embedded directly in SessionContext
   - Direct localStorage access scattered throughout the code
   - No clear layer separation between UI, state management, and data access

5. **Complex State Management**
   - Many interconnected pieces of state managed with useState
   - Complex initialization logic with multiple fallbacks
   - Potential for race conditions during initialization

## Solution Architecture

### Proposed Context Architecture

1. **AuthContext**
   - Session management and authentication
   - Token handling and session persistence
   - Login/logout functionality

2. **ModelContext**
   - Model configuration and selection
   - Parameter management for models
   - Storing/retrieving model preferences

3. **ThemeContext**
   - Theme management (light/dark/system)
   - Theme persistence
   - CSS variable application

4. **ChatContext**
   - Chat messaging state
   - Streaming state management
   - Message formatting utilities

5. **FileContext** *(Future, not in first phase)*
   - File upload management
   - File previews and handling

### Service Layer

1. **API Service**
   - Centralized API calls
   - Request/response handling
   - Error management

2. **Storage Service**
   - localStorage abstraction
   - Data versioning
   - Centralized storage keys

## Implementation Plan

### Phase 1: Preparation (Current Session)

1. ✅ Analyze current context structure and dependencies
2. ✅ Document current issues and plan architecture
3. ✅ Create utility service files
   - ✅ apiService.js - API interaction functions
   - ✅ storageService.js - localStorage interaction
   - ✅ logger.js enhancements

### Phase 2: Context Extraction (Next Session)

4. [ ] Extract and implement AuthContext
   - [ ] Move auth functions from SessionContext
   - [ ] Create use-auth hook
   - [ ] Test authentication flow
5. [ ] Extract and implement ModelContext
   - [ ] Move model management from SessionContext
   - [ ] Create use-model hook
   - [ ] Test model selection and parameter storage

### Phase 3: Theme Refactoring (Next Session)

6. [ ] Refactor ThemeContext
   - [ ] Consolidate theme utilities from theme.ts
   - [ ] Improve theme state management
   - [ ] Test theme switching

### Phase 4: Chat Context (Later Session)

7. [ ] Extract and implement ChatContext
   - [ ] Move chat state from SessionContext
   - [ ] Create use-chat hook
   - [ ] Test chat interactions

### Phase 5: Integration (Final Session)

8. [ ] Update App.jsx context provider hierarchy
9. [ ] Migrate components to use new contexts
10. [ ] Test full application flow
11. [ ] Add comprehensive logging
12. [ ] Clean up any remaining dependencies

## Context Provider Hierarchy

Proposed hierarchy after refactoring:

```jsx
<AuthProvider>
  <ThemeProvider>
    <ModelProvider>
      <ChatProvider>
        <Router>
          <AppRoutes />
        </Router>
      </ChatProvider>
    </ModelProvider>
  </ThemeProvider>
</AuthProvider>
```

## Session Tracking

| Phase | Task | Status | Notes |
|-------|------|--------|-------|
| 1 | Analyze current context structure | ✅ | Completed |
| 1 | Document issues and plan architecture | ✅ | Completed |
| 1 | Create utility service files | 🔄 | In progress |
| 2 | Extract AuthContext | ⏱️ | Not started |
| 2 | Extract ModelContext | ⏱️ | Not started |
| 3 | Refactor ThemeContext | ⏱️ | Not started |
| 4 | Extract ChatContext | ⏱️ | Not started |
| 5 | Integration and testing | ⏱️ | Not started |