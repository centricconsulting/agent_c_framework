# API Service Layer Implementation Plan

## Overview

This plan outlines the step-by-step process for implementing the improved API service layer for the v2 API. We'll follow a methodical approach to ensure minimal disruption to the application while transitioning to the new API.

NOTICE: We will execute a single step of a phase per session to ensure we include developer feedback and verification. Correct is better than fast. 

## Current Status (as of May 10, 2025 6:45PM EDT)

**Current Phase:** Phase 3
**Current Step:** Step 1 - Update Session API Service
**Previous Step:** Phase 2, Step 3 - Create Debug API Service (Completed)

## Phase 1: Base Infrastructure and Utility Functions

### Step 1: Update API Base

- [x] Update `api.js` with v2 base URL and configuration
- [x] Add `extractResponseData` utility for standardized v2 response handling
- [x] Enhance error handling for v2 error format
- [x] Add support for pagination in GET requests

## Phase 2: New API Services

### Step 1: Config API Service

- [x] Create `config-api.js` for all configuration endpoints
- [x] Implement `getModels()`, `getPersonas()`, `getTools()` and `getSystemConfig()`
- [x] Add tests for Config API Service

### Step 2: History API Service

- [x] Create `history-api.js` for history and replay endpoints
- [x] Implement methods for listing, retrieving, and streaming events
- [x] Implement replay control methods
- [x] Add tests for History API Service

### Step 3: Debug API Service

- [x] Create `debug-api.js` for debugging endpoints
- [x] Implement session and agent debug information methods
- [x] Add tests for Debug API Service

## Phase 3: Update Existing Services

### Step 1: Session API Service

- [ ] Update `session-api.js` for v2 session endpoints
- [ ] Update session creation, verification, listing, and deletion methods
- [ ] Add agent configuration methods
- [ ] Update tool management methods
- [ ] Add tests for updated Session API Service

### Step 2: Chat API Service

- [ ] Update `chat-api.js` for v2 chat endpoints
- [ ] Update message format and streaming support
- [ ] Update file upload, download, and management methods
- [ ] Add tests for updated Chat API Service

### Step 3: Update index.js

- [ ] Update `index.js` to export all services and methods
- [ ] Ensure backward compatibility for existing imports

## Phase 4: Adapter Layer

### Step 1: Create v1 Adapters

- [ ] Create adapter functions that match v1 API signatures
- [ ] Implement parameter transformation from v1 to v2 format
- [ ] Implement response transformation from v2 to v1 format
- [ ] Test adapter functions against v1 expectations

### Step 2: Replace v1 Implementations

- [ ] Update original v1 service files to use adapters
- [ ] Maintain the same exports and signatures
- [ ] Add detailed error logging for debugging
- [ ] Test to ensure component compatibility

## Phase 5: Integration and Testing

### Step 1: Unit Testing

- [ ] Create mocks for v2 API responses
- [ ] Test error handling and edge cases
- [ ] Test pagination and response extraction

### Step 2: Integration Testing

- [ ] Create test harness for testing with actual API
- [ ] Test full workflow scenarios (session creation → chat → history)
- [ ] Validate streaming responses and event handling

## Phase 6: Component Updates and Documentation

### Step 1: Update Components

- [ ] Identify all components using the API services
- [ ] Update components to use the new service methods directly
- [ ] Update response handling to match v2 format
- [ ] Test components with v2 services

### Step 2: Documentation

- [ ] Update API service documentation
- [ ] Add examples for common usage patterns
- [ ] Document migration notes for developers

## Rollout Strategy

We'll use the Adapter Pattern for a smooth migration:

1. Implement v2 services and adapters in parallel
2. Replace v1 implementations with adapters (no component changes needed)
3. Gradually update components to use v2 services directly
4. Remove adapters once all components are migrated

## Mitigation Strategy

1. **Rollback Plan**: Keep v1 services in a separate branch until v2 is fully validated
2. **Versioned Adapters**: If needed, create separate adapter versions for specific components
3. **Logging**: Add detailed logging for API calls to track issues
4. **Error Monitoring**: Implement enhanced error monitoring during transition

## Success Criteria

1. All API endpoints are correctly migrated to v2
2. All components correctly use the v2 services (directly or via adapters)
3. All tests pass with the v2 API
4. No regression in application functionality
5. Improved error handling and response processing