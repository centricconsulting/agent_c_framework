# Code Quality C# Component

A comprehensive C# development pattern that ensures all C# coding agents follow consistent quality standards and best practices throughout the development lifecycle using modern .NET frameworks and tooling.

## Binary Decision

**Does this agent write or modify C# code?**

- **YES** → Use this component
- **NO** → Skip this component

## Who Uses This

**Target Agents**: C# development agents (35% of coding agents)

**Scenarios**:
- Agents writing new C# applications or services
- Agents modifying existing C# codebases
- Agents performing C# code refactoring or enhancements
- Agents creating C# libraries or NuGet packages
- Agents implementing .NET APIs or web services
- Agents writing C# desktop applications or console tools
- Agents working with ASP.NET Core, Entity Framework, or other .NET frameworks
- Any agent that generates, reviews, or maintains C# code

## Component Pattern

```markdown
## C# Code Quality Requirements

### General Standards
- Prefer the use of existing NuGet packages over writing new code
- Unit testing is mandatory for project work using xUnit, NUnit, or MSTest
- Maintain proper separation of concerns with clear architectural boundaries
- Use idiomatic C# patterns and modern language features
- Include logging where appropriate using Microsoft.Extensions.Logging or Serilog
- Bias towards the most efficient solution leveraging .NET performance optimizations
- Factor static code analysis into your planning (Roslyn analyzers, SonarQube, StyleCop)
- Unless otherwise stated assume the user is using the latest version of .NET and any packages
- `Think` about any changes you're making and code you're generating
  - Double check that you're not using deprecated syntax or obsolete APIs
  - Consider "is this a change I should be making NOW or am I deviating from the plan?"

### Method Size and Complexity
- Keep methods under 25 lines
- Use helper methods to break down complex logic
- Aim for a maximum cyclomatic complexity of 10 per method
- Each method should have a single responsibility
- Use LINQ expressions for data transformations and filtering where appropriate

### C#-Specific Modularity
- Maintain proper modularity by:
  - Using one file per class as the standard practice
  - Following proper .NET project layouts with appropriate folder structures
  - Organizing code into logical namespaces that reflect project hierarchy
  - Using partial classes only when necessary (code generation scenarios)
- Keep your code DRY, and use extension methods and helper classes for common patterns
- Use dependency injection for loose coupling between components
- Implement proper disposal patterns with using statements and IDisposable

### C# Naming Conventions
- Use descriptive method and property names that clearly indicate their purpose
- Follow Microsoft C# naming guidelines consistently:
  - PascalCase for public members, properties, methods, and classes
  - camelCase for private fields and local variables
  - Use meaningful names over abbreviated or cryptic identifiers
  - Prefix private fields with underscore when beneficial for clarity (_privateField)
  - Use PascalCase for constants and static readonly fields
  - Use PascalCase for namespaces, interfaces (with 'I' prefix), and enums

### Type Safety and Modern C# Features
- Use nullable reference types and null-checking where appropriate
- Leverage pattern matching and switch expressions for cleaner code
- Use records for immutable data structures when suitable
- Implement async/await patterns properly for I/O-bound operations
- Use generic constraints and covariance/contravariance appropriately
- Prefer var keyword when type is obvious from context
- Use string interpolation over string concatenation or String.Format

### C# Error Handling
- Use custom exception classes derived from appropriate base exceptions
- Handle C#-specific exceptions appropriately (ArgumentException, InvalidOperationException, etc.)
- Use try-catch blocks judiciously, catching specific exception types
- Provide clear error messages with relevant context information
- Log errors with structured logging using semantic properties
- Use finally blocks or using statements for proper resource cleanup
- Implement global exception handling at application boundaries
- Use Result patterns or Option types for expected failure scenarios when appropriate
```

## Usage Notes

**Positioning**: Place in a dedicated "Code Quality Requirements" section within the agent persona, typically after core guidelines but before specific technical responsibilities.

**Implementation Notes**:
- **Comprehensive Coverage**: Addresses all aspects of C# development from syntax to .NET architecture
- **Standard Enforcement**: Integrates industry-standard tools (Roslyn analyzers, StyleCop, SonarQube) into the workflow
- **Modern .NET**: Assumes latest .NET features, nullable reference types, and contemporary C# language constructs
- **Universal Application**: This complete pattern applies to all C# coding agents - no variations or tiers
- **Quality Integration**: Embeds quality checks and static analysis into the natural development workflow

**Integration Tips**:
- Works independently - no dependencies on other components
- Combines well with reflection rules for code analysis and debugging
- Essential for agents in multi-agent .NET development teams
- Pairs with workspace organization for proper .NET solution and project structure
- Supports both greenfield development and legacy .NET Framework maintenance

## Example Implementation

All C# coding agents use this identical pattern:

```markdown
## C# Code Quality Requirements

### General Standards
- Prefer the use of existing NuGet packages over writing new code
- Unit testing is mandatory for project work using xUnit, NUnit, or MSTest
- Maintain proper separation of concerns with clear architectural boundaries
- Use idiomatic C# patterns and modern language features
- Include logging where appropriate using Microsoft.Extensions.Logging or Serilog
- Bias towards the most efficient solution leveraging .NET performance optimizations
- Factor static code analysis into your planning (Roslyn analyzers, SonarQube, StyleCop)
- Unless otherwise stated assume the user is using the latest version of .NET and any packages
- `Think` about any changes you're making and code you're generating
  - Double check that you're not using deprecated syntax or obsolete APIs
  - Consider "is this a change I should be making NOW or am I deviating from the plan?"

### Method Size and Complexity
- Keep methods under 25 lines
- Use helper methods to break down complex logic
- Aim for a maximum cyclomatic complexity of 10 per method
- Each method should have a single responsibility
- Use LINQ expressions for data transformations and filtering where appropriate

### C#-Specific Modularity
- Maintain proper modularity by:
  - Using one file per class as the standard practice
  - Following proper .NET project layouts with appropriate folder structures
  - Organizing code into logical namespaces that reflect project hierarchy
  - Using partial classes only when necessary (code generation scenarios)
- Keep your code DRY, and use extension methods and helper classes for common patterns
- Use dependency injection for loose coupling between components
- Implement proper disposal patterns with using statements and IDisposable

### C# Naming Conventions
- Use descriptive method and property names that clearly indicate their purpose
- Follow Microsoft C# naming guidelines consistently:
  - PascalCase for public members, properties, methods, and classes
  - camelCase for private fields and local variables
  - Use meaningful names over abbreviated or cryptic identifiers
  - Prefix private fields with underscore when beneficial for clarity (_privateField)
  - Use PascalCase for constants and static readonly fields
  - Use PascalCase for namespaces, interfaces (with 'I' prefix), and enums

### Type Safety and Modern C# Features
- Use nullable reference types and null-checking where appropriate
- Leverage pattern matching and switch expressions for cleaner code
- Use records for immutable data structures when suitable
- Implement async/await patterns properly for I/O-bound operations
- Use generic constraints and covariance/contravariance appropriately
- Prefer var keyword when type is obvious from context
- Use string interpolation over string concatenation or String.Format

### C# Error Handling
- Use custom exception classes derived from appropriate base exceptions
- Handle C#-specific exceptions appropriately (ArgumentException, InvalidOperationException, etc.)
- Use try-catch blocks judiciously, catching specific exception types
- Provide clear error messages with relevant context information
- Log errors with structured logging using semantic properties
- Use finally blocks or using statements for proper resource cleanup
- Implement global exception handling at application boundaries
- Use Result patterns or Option types for expected failure scenarios when appropriate
```

## Component Benefits

- **Consistent Quality**: Standardizes C# development practices across all coding agents
- **Modern Standards**: Incorporates latest .NET features, C# language constructs, and Microsoft guidelines
- **Tool Integration**: Embeds static analysis and quality tools into the development workflow
- **Error Prevention**: Proactive quality measures reduce bugs and improve code reliability
- **Performance Optimization**: Leverages .NET performance features and best practices
- **Maintainability**: Code structure requirements ensure long-term maintainability and team collaboration
- **Team Consistency**: All C# agents follow identical standards for seamless multi-agent development
- **Binary Decision**: Clear YES/NO implementation - agents either follow comprehensive C# standards or they do not