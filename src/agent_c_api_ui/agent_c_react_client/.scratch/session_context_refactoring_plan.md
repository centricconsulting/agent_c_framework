# SessionContext Refactoring Plan

## Current Issues

1. **Overloaded Responsibilities**: SessionContext currently handles authentication, model configuration, theme, chat state, and more
2. **Direct API and Storage Access**: The context makes direct API calls and localStorage operations
3. **Complex State Management**: Many interconnected pieces of state with complex initialization logic
4. **Redundant Functionality**: AuthContext and ModelContext now handle some of the same responsibilities

## Refactoring Goals

1. **Single Responsibility**: Refocus SessionContext on chat-specific state management
2. **Use Utility Services**: Replace direct API and localStorage calls with our utility services
3. **Use New Contexts**: Leverage AuthContext and ModelContext instead of duplicating functionality
4. **Clean Architecture**: Establish clear boundaries between contexts

## Implementation Plan

### Phase 1: Assess Current Usage

1. Identify all components that consume SessionContext
2. Map which parts of the context state/methods are used where
3. Document dependencies between different parts of the state

### Phase 2: Remove Redundant Functionality

1. Remove authentication functionality (now in AuthContext)
   - Remove session initialization
   - Remove session storage
   - Remove auth error handling

2. Remove model management functionality (now in ModelContext)
   - Remove model configuration state
   - Remove model parameter management
   - Remove model switching logic

### Phase 3: Refactor Core Functionality

1. Update remaining API calls to use apiService
2. Update remaining localStorage operations to use storageService
3. Refactor chat message management
4. Refactor tool management
5. Refactor options/UI state management

### Phase 4: Update Context Provider

1. Simplify the SessionProvider props
2. Update existing effects to use the new contexts
3. Optimize initialization logic
4. Add comprehensive error handling

### Phase 5: Update Hook and Usages

1. Update the use-session-context hook
2. Update components to use the appropriate contexts
   - For auth operations: useAuth
   - For model operations: useModel
   - For session/chat operations: useSessionContext

## Timeline

- **Day 1**: Complete Phase 1 and Phase 2
- **Day 2**: Complete Phase 3 and Phase 4
- **Day 3**: Complete Phase 5 and testing

## Risks and Mitigations

### Risks

1. **Breaking Changes**: Components relying on specific SessionContext behavior may break
2. **State Initialization Order**: Dependencies between contexts may cause initialization issues
3. **Performance Impact**: Changes to state management may affect performance

### Mitigations

1. **Careful Testing**: Thoroughly test each component after changes
2. **Logging**: Add detailed logging for state changes and initialization
3. **Incremental Approach**: Update one component at a time
4. **Fallback Mechanisms**: Provide fallbacks for initialization failures