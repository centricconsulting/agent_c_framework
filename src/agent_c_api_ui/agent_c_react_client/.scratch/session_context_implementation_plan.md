# SessionContext Refactoring Implementation Plan

## Overview

This document outlines the step-by-step approach for refactoring the SessionContext to focus only on chat-specific functionality, now that we've successfully extracted theme, authentication, and model management into their own contexts.

## Refactoring Goals

1. Focus SessionContext on chat-specific state management
2. Replace direct API and localStorage calls with service utilities
3. Use extracted contexts instead of duplicating functionality
4. Implement comprehensive logging
5. Improve organization and readability

## Implementation Steps

### Step 1: Identify and Remove Redundant State and Methods

1. Remove theme-related state and methods (moved to ThemeContext)
   - Remove theme state variable
   - Remove theme change handler
   - Remove theme-related useEffect

2. Remove authentication-related state and methods (moved to AuthContext)
   - Remove session initialization logic from fetchInitialData
   - Keep sessionId as a prop from AuthContext
   - Remove direct session storage access

3. Remove model-related state and methods (moved to ModelContext)
   - Remove model configuration state (modelConfigs, modelName, selectedModel)
   - Remove model parameter state (temperature, modelParameters)
   - Remove model switching/updating logic
   - Remove model storage operations

### Step 2: Refactor API Calls to Use apiService

1. Replace direct fetch calls with apiService methods
   - Replace `/tools` fetch with `apiService.getTools()`
   - Replace `/personas` fetch with `apiService.getPersonas()`
   - Replace `/get_agent_tools/${sessionId}` with `apiService.getAgentTools(sessionId)`
   - Replace `/update_tools` with `apiService.updateTools(sessionId, tools)`
   - Replace `/update_settings` with `apiService.updateSettings(sessionId, settings)`

2. Remove redundant API response processing
   - Remove checkResponse method (handled by apiService)
   - Update error handling to use logger

### Step 3: Refactor Storage Operations to Use storageService

1. Replace direct localStorage operations with storageService methods
   - Remove saveConfigToStorage method (handled by ModelContext)
   - Replace localStorage getItem/setItem/removeItem with appropriate storageService calls

### Step 4: Refactor Core Chat Functionality

1. Consolidate chat message management
   - Keep messages state and setMessages method
   - Keep getChatCopyContent and getChatCopyHTML methods

2. Refactor tool management
   - Keep availableTools state
   - Keep activeTools state
   - Keep handleEquipTools method (using apiService)
   - Remove duplicate fetchAgentTools from initializeSession effect

3. Refactor UI state management
   - Keep isOptionsOpen and setIsOptionsOpen
   - Keep isStreaming and setIsStreaming (handleProcessingStatus)
   - Keep isLoading and related states for more granular UI updates

### Step 5: Update Context Provider

1. Simplify the SessionProvider props and state
   - Update the initial state declarations
   - Remove unnecessary state variables
   - Organize remaining state by purpose (UI state, chat state, tool state)

2. Update useEffect hooks
   - Remove dependencies on removed state
   - Add effects to react to changes from other contexts
   - Ensure proper cleanup

3. Update error handling
   - Implement more granular error states
   - Use logger for comprehensive error tracking

### Step 6: Update Context Value and Hook

1. Update the SessionContext.Provider value object
   - Remove properties now provided by other contexts
   - Organize remaining properties by function
   - Add JSDoc comments for improved developer experience

2. Update the use-session-context hook
   - Add component name parameter for logging
   - Add better error handling

## Testing Approach

1. Add comprehensive logging
   - Log context initialization 
   - Log state changes
   - Log API operations
   - Log performance metrics

2. Test integration with other contexts
   - Verify that changes to AuthContext properly affect SessionContext
   - Verify that changes to ModelContext properly affect SessionContext
   - Verify that UI remains functional with the refactored context structure