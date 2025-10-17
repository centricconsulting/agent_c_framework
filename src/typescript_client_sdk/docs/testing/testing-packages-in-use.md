# Testing Packages in Use

## Overview
This document outlines all testing-related packages currently installed and configured in the Agent C Realtime SDK monorepo. All packages are installed at the workspace root level and shared across all packages.

## Core Testing Framework

### Vitest Suite
- **vitest**: `3.2.4` - Main test runner, provides fast unit testing with native ESM support
- **@vitest/ui**: `3.2.4` - Interactive UI for viewing and debugging tests
- **@vitest/coverage-v8**: `3.2.4` - Code coverage reporting using V8's built-in coverage

## React Testing Libraries

### Testing Library
- **@testing-library/react**: `16.3.0` - React component testing utilities, provides queries and utilities for testing React components
- **@testing-library/dom**: `10.4.1` - DOM testing utilities, foundation for React Testing Library
- **@testing-library/user-event**: `14.6.1` - Simulates user interactions with better accuracy than fireEvent
- **@testing-library/jest-dom**: `6.8.0` - Custom jest/vitest matchers for DOM assertions (toBeInTheDocument, toHaveClass, etc.)

## Test Environment

### DOM Implementation
- **happy-dom**: `18.0.1` - Lightweight DOM implementation for Node.js, faster alternative to jsdom for testing

## Mocking & Test Data

### API Mocking
- **msw**: `2.11.0` (Mock Service Worker) - API mocking library for intercepting network requests in tests

### Test Data Generation
- **@faker-js/faker**: `8.4.1` - Generate realistic test data (names, emails, dates, etc.)
- **superstruct**: `1.0.4` - Runtime data validation and coercion, useful for validating test data structures

## Package-Specific Configuration

### @agentc/realtime-core
- **Test Environment**: `node` (no DOM required)
- **Focus**: Business logic, event handling, WebSocket communication
- **Coverage Directory**: `.scratch/coverage/core`

### @agentc/realtime-react
- **Test Environment**: `happy-dom`
- **Additional Dependencies**: `@testing-library/jest-dom@6.8.0` (in package.json)
- **Focus**: React hooks, context providers, component lifecycle
- **Coverage Directory**: `.scratch/coverage/react`

### @agentc/realtime-ui
- **Test Environment**: `happy-dom`
- **Additional Dependencies**: 
  - `@testing-library/jest-dom@6.8.0` (in package.json)
  - `jest-axe@10.0.0` (for accessibility testing, currently unused)
- **Focus**: UI components, user interactions, accessibility
- **Coverage Directory**: `.scratch/coverage/ui`

### @agentc/demo-app
- **Test Environment**: `happy-dom`
- **Additional Dependencies**: 
  - `@testing-library/jest-dom@6.8.0` (in package.json)
  - `jest-axe@10.0.0` (for accessibility testing, currently unused)
- **Focus**: Integration testing, full application flows
- **Coverage Directory**: `.scratch/coverage/demo`

## Test Configuration

### Test Scripts Available
```json
{
  "test": "vitest run --reporter=dot",           // Run all tests once with minimal output
  "test:run": "lerna run test:run",              // Run tests in all packages
  "test:coverage": "lerna run test:coverage",     // Run tests with coverage reports
  "test:ui": "lerna run test:ui",                // Open Vitest UI
  "test:debug": "lerna run test:debug",          // Debug mode for troubleshooting
  "test:watch": "lerna run test:watch"           // Watch mode for development
}
```

### Test Output Configuration
- **Reporter**: `dot` - Concise terminal output, minimal tokens
- **Coverage Formats**: `text`, `json`, `html`, `lcov`
- **Coverage Thresholds**: Currently set to 0% (temporary during reset)
  - TODO: Restore to 80% for branches, functions, lines, statements

### Test File Patterns
- **Unit Tests**: `*.test.ts(x)`
- **Integration Tests**: `*.integration.test.ts(x)`
- **Test Location**: `src/**/__tests__/` directories
- **Excluded**: `.trash/**`, `node_modules/**`, `dist/**`, `coverage/**`

## Installation Commands

All packages are already installed. If reinstallation is needed:

```bash
# Install all testing packages (already installed)
pnpm add -D vitest@^3.2.4 @vitest/ui@^3.2.4 @vitest/coverage-v8@^3.2.4
pnpm add -D @testing-library/react@^16.3.0 @testing-library/dom@^10.4.1
pnpm add -D @testing-library/user-event@^14.6.1 @testing-library/jest-dom@^6.8.0
pnpm add -D happy-dom@^18.0.1 msw@^2.11.0
pnpm add -D @faker-js/faker@^8.4.1 superstruct@^1.0.4
```

## Notes

### Current State
- All test files have been removed and archived to `.trash/` directory
- One simple passing test exists in each package (`src/__tests__/simple.test.ts`)
- Test infrastructure is ready for actual test implementation

### Next Steps
1. Restore coverage thresholds to 80% once actual tests are written
2. Implement proper test setup files for each package
3. Create test utilities and mock files as needed
4. Begin writing actual unit and integration tests following the testing standards

### Important Reminders
- **NO console.log()** - Use Logger class exclusively
- Tests must be co-located with source code in `__tests__` directories
- All external dependencies must be properly mocked
- Follow AAA pattern (Arrange, Act, Assert) for test structure