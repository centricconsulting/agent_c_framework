# Code Quality TypeScript Component

A comprehensive TypeScript development pattern that ensures all TypeScript coding agents follow consistent quality standards and best practices throughout the development lifecycle using modern TypeScript features, npm ecosystem, and tooling.

## Binary Decision

**Does this agent write or modify TypeScript code?**

- **YES** → Use this component
- **NO** → Skip this component

## Who Uses This

**Target Agents**: TypeScript development agents (45% of coding agents)

**Scenarios**:
- Agents writing new TypeScript applications or services
- Agents modifying existing TypeScript codebases
- Agents performing TypeScript code refactoring or enhancements
- Agents creating TypeScript libraries or npm packages
- Agents implementing Node.js APIs or web services
- Agents writing React applications or components
- Agents working with Express, NestJS, or other TypeScript frameworks
- Agents creating frontend applications with modern bundlers (Vite, Webpack)
- Any agent that generates, reviews, or maintains TypeScript code

## Component Pattern

```markdown
## TypeScript Code Quality Requirements

### General Standards
- Prefer the use of existing npm packages over writing new code
- Unit testing is mandatory for project work using Jest, Vitest, or Mocha
- Maintain proper separation of concerns with clear architectural boundaries
- Use idiomatic TypeScript patterns and modern JavaScript/TypeScript features
- Include logging where appropriate using Winston, Pino, or console for simple cases
- Bias towards the most efficient solution leveraging TypeScript's type system
- Factor static code analysis into your planning (ESLint, Prettier, TypeScript compiler)
- Unless otherwise stated assume the user is using the latest version of TypeScript and Node.js/npm packages
- `Think` about any changes you're making and code you're generating
  - Double check that you're not using deprecated syntax or obsolete APIs
  - Consider "is this a change I should be making NOW or am I deviating from the plan?"

### Method Size and Complexity
- Keep functions and methods under 25 lines
- Use helper functions to break down complex logic
- Aim for a maximum cyclomatic complexity of 10 per function
- Each function should have a single responsibility
- Use array methods (map, filter, reduce) and modern ES6+ features for data transformations

### TypeScript-Specific Modularity
- Maintain proper modularity by:
  - Using one file per class/major component when appropriate
  - Following proper npm package layouts with clear entry points
  - Using barrel exports (index.ts files) for clean public APIs
  - Organizing code into logical modules with proper import/export patterns
- Keep your code DRY, and use utility functions and modules for common patterns
- Use dependency injection patterns for loose coupling between components
- Implement proper async patterns with Promises and async/await

### TypeScript Naming Conventions
- Use descriptive function and variable names that clearly indicate their purpose
- Follow TypeScript/JavaScript naming conventions consistently:
  - camelCase for variables, functions, and methods
  - PascalCase for classes, interfaces, types, and enums
  - UPPER_SNAKE_CASE for constants
  - Use meaningful names over abbreviated or cryptic identifiers
  - Prefix private class members with underscore when beneficial for clarity (_privateMethod)
  - Use descriptive names for interfaces (User, UserRepository) or I-prefix when appropriate (IUserService)

### TypeScript Type System and Modern Features
- Use strict TypeScript configuration with strict null checks enabled
- Leverage union types, intersection types, and literal types appropriately
- Use generics with proper constraints for reusable components
- Implement proper type guards and type assertions when necessary
- Use utility types (Partial, Pick, Omit, Record) for type transformations
- Prefer interfaces for object shapes and types for unions/primitives
- Use const assertions and as const for immutable data structures
- Implement proper async/await patterns with typed Promise returns
- Use destructuring assignment for cleaner parameter and object handling
- Leverage template literal types and mapped types for advanced scenarios

### TypeScript Error Handling
- Use custom error classes extending Error with proper stack traces
- Handle TypeScript/JavaScript-specific exceptions appropriately (TypeError, ReferenceError, etc.)
- Use try-catch blocks judiciously with proper error typing
- Provide clear error messages with relevant context information
- Log errors with structured data and proper error serialization
- Use proper error boundaries in React applications when applicable
- Implement Result patterns or Maybe/Option types for expected failure scenarios
- Use proper async error handling with rejected promises and async/await
- Validate external data at boundaries using libraries like Zod or Joi

### Frontend Considerations (when applicable)
- Follow React best practices with proper hooks usage and component patterns
- Implement proper state management patterns (useState, useReducer, external state)
- Use proper event handling with correct TypeScript event types
- Implement performance optimizations (React.memo, useMemo, useCallback)
- Consider bundle size and lazy loading for optimal performance
- Use proper accessibility attributes and semantic HTML

### Node.js Considerations (when applicable)
- Use proper async patterns for I/O operations and avoid blocking the event loop
- Implement proper error handling for async operations and Promise rejections
- Use appropriate Node.js APIs with proper TypeScript types (@types/node)
- Handle process signals and graceful shutdown patterns
- Implement proper logging and monitoring for production applications
- Use environment variables and configuration management appropriately
```

## Usage Notes

**Positioning**: Place in a dedicated "Code Quality Requirements" section within the agent persona, typically after core guidelines but before specific technical responsibilities.

**Implementation Notes**:
- **Comprehensive Coverage**: Addresses all aspects of TypeScript development from syntax to modern JavaScript features
- **Standard Enforcement**: Integrates industry-standard tools (ESLint, Prettier, TypeScript compiler) into the workflow
- **Modern TypeScript**: Assumes latest TypeScript features, strict type checking, and contemporary development patterns
- **Universal Application**: This complete pattern applies to all TypeScript coding agents - no variations or tiers
- **Full-Stack Coverage**: Addresses both frontend (React) and backend (Node.js) development scenarios
- **Quality Integration**: Embeds quality checks and static analysis into the natural development workflow

**Integration Tips**:
- Works independently - no dependencies on other components
- Combines well with reflection rules for code analysis and debugging
- Essential for agents in multi-agent TypeScript development teams
- Pairs with workspace organization for proper npm/Node.js project structure
- Supports both new development and legacy JavaScript migration scenarios
- Adapts to both frontend and backend TypeScript development contexts

## Example Implementation

All TypeScript coding agents use this identical pattern:

```markdown
## TypeScript Code Quality Requirements

### General Standards
- Prefer the use of existing npm packages over writing new code
- Unit testing is mandatory for project work using Jest, Vitest, or Mocha
- Maintain proper separation of concerns with clear architectural boundaries
- Use idiomatic TypeScript patterns and modern JavaScript/TypeScript features
- Include logging where appropriate using Winston, Pino, or console for simple cases
- Bias towards the most efficient solution leveraging TypeScript's type system
- Factor static code analysis into your planning (ESLint, Prettier, TypeScript compiler)
- Unless otherwise stated assume the user is using the latest version of TypeScript and Node.js/npm packages
- `Think` about any changes you're making and code you're generating
  - Double check that you're not using deprecated syntax or obsolete APIs
  - Consider "is this a change I should be making NOW or am I deviating from the plan?"

### Method Size and Complexity
- Keep functions and methods under 25 lines
- Use helper functions to break down complex logic
- Aim for a maximum cyclomatic complexity of 10 per function
- Each function should have a single responsibility
- Use array methods (map, filter, reduce) and modern ES6+ features for data transformations

### TypeScript-Specific Modularity
- Maintain proper modularity by:
  - Using one file per class/major component when appropriate
  - Following proper npm package layouts with clear entry points
  - Using barrel exports (index.ts files) for clean public APIs
  - Organizing code into logical modules with proper import/export patterns
- Keep your code DRY, and use utility functions and modules for common patterns
- Use dependency injection patterns for loose coupling between components
- Implement proper async patterns with Promises and async/await

### TypeScript Naming Conventions
- Use descriptive function and variable names that clearly indicate their purpose
- Follow TypeScript/JavaScript naming conventions consistently:
  - camelCase for variables, functions, and methods
  - PascalCase for classes, interfaces, types, and enums
  - UPPER_SNAKE_CASE for constants
  - Use meaningful names over abbreviated or cryptic identifiers
  - Prefix private class members with underscore when beneficial for clarity (_privateMethod)
  - Use descriptive names for interfaces (User, UserRepository) or I-prefix when appropriate (IUserService)

### TypeScript Type System and Modern Features
- Use strict TypeScript configuration with strict null checks enabled
- Leverage union types, intersection types, and literal types appropriately
- Use generics with proper constraints for reusable components
- Implement proper type guards and type assertions when necessary
- Use utility types (Partial, Pick, Omit, Record) for type transformations
- Prefer interfaces for object shapes and types for unions/primitives
- Use const assertions and as const for immutable data structures
- Implement proper async/await patterns with typed Promise returns
- Use destructuring assignment for cleaner parameter and object handling
- Leverage template literal types and mapped types for advanced scenarios

### TypeScript Error Handling
- Use custom error classes extending Error with proper stack traces
- Handle TypeScript/JavaScript-specific exceptions appropriately (TypeError, ReferenceError, etc.)
- Use try-catch blocks judiciously with proper error typing
- Provide clear error messages with relevant context information
- Log errors with structured data and proper error serialization
- Use proper error boundaries in React applications when applicable
- Implement Result patterns or Maybe/Option types for expected failure scenarios
- Use proper async error handling with rejected promises and async/await
- Validate external data at boundaries using libraries like Zod or Joi

### Frontend Considerations (when applicable)
- Follow React best practices with proper hooks usage and component patterns
- Implement proper state management patterns (useState, useReducer, external state)
- Use proper event handling with correct TypeScript event types
- Implement performance optimizations (React.memo, useMemo, useCallback)
- Consider bundle size and lazy loading for optimal performance
- Use proper accessibility attributes and semantic HTML

### Node.js Considerations (when applicable)
- Use proper async patterns for I/O operations and avoid blocking the event loop
- Implement proper error handling for async operations and Promise rejections
- Use appropriate Node.js APIs with proper TypeScript types (@types/node)
- Handle process signals and graceful shutdown patterns
- Implement proper logging and monitoring for production applications
- Use environment variables and configuration management appropriately
```

## Component Benefits

- **Consistent Quality**: Standardizes TypeScript development practices across all coding agents
- **Modern Standards**: Incorporates latest TypeScript features, ES6+ syntax, and contemporary development patterns
- **Tool Integration**: Embeds static analysis and formatting tools into the development workflow
- **Error Prevention**: Proactive quality measures reduce bugs and improve type safety
- **Performance Optimization**: Leverages TypeScript's type system and modern JavaScript performance features
- **Maintainability**: Code structure requirements ensure long-term maintainability and team collaboration
- **Full-Stack Support**: Addresses both frontend and backend TypeScript development scenarios
- **Team Consistency**: All TypeScript agents follow identical standards for seamless multi-agent development
- **Binary Decision**: Clear YES/NO implementation - agents either follow comprehensive TypeScript standards or they do not