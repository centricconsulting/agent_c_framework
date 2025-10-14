# Angular Enterprise Development Team Guide

## Overview

This document describes your new Angular enterprise development team - a sophisticated multi-agent system designed to build complex Angular applications using modern standalone components, enterprise architecture patterns, and comprehensive testing strategies.

## Team Architecture

**Pattern**: Direct Communication Mesh (Pattern 4 from Design Bible)

This advanced pattern enables specialists to communicate directly with each other via `AgentTeamTools`, eliminating the "telephone game" effect while maintaining orchestrator oversight for workflow coordination and quality gates.

## Your Team Members

### 1. Diego - Angular Enterprise Orchestrator 🎭
**Agent Key**: `diego_angular_orchestrator`  
**Category**: `domo` (user-facing)  
**Your Main Point of Contact**

Diego is your team lead who coordinates the entire Angular development workflow. He manages quality gates, coordinates specialists, and maintains project state. You'll interact directly with Diego, who will then orchestrate the work of his specialist team.

**Key Capabilities**:
- Sequential phase workflow management (Requirements → Architecture → Implementation → Testing)
- Quality gates with completion signoff requirements
- Clone delegation for complex tasks
- Comprehensive workspace planning
- Enterprise Angular standards enforcement

**Tools**: WorkspaceTools, WorkspacePlanningTools, ThinkTools, AgentTeamTools, **AgentCloneTools**, AgentCloneTools

---

### 2. Rex - Angular Requirements Specialist 🔍
**Agent Key**: `rex_angular_requirements`  
**Category**: `assist` (agent-helper)

Rex transforms business requirements into crystal-clear Angular technical specifications. He bridges the gap between business needs and technical implementation, ensuring nothing gets lost in translation.

**Key Capabilities**:
- Business-to-technical requirement translation
- Angular-specific analysis (components, services, state management)
- Feasibility assessment and risk identification
- Technical specification creation
- Direct collaboration with Aria on technical feasibility
- **Delegates complex analysis to clones** (e.g., one clone per domain/feature area)

**Tools**: WorkspaceTools, WorkspacePlanningTools, ThinkTools, AgentTeamTools, **AgentCloneTools**

---

### 3. Aria - Angular Architect 🏛️
**Agent Key**: `aria_angular_architect`  
**Category**: `assist` (agent-helper)

Aria is the architectural visionary who designs elegant, scalable Angular architectures. She's the technical nexus of the team, collaborating with everyone to ensure architectural integrity.

**Key Capabilities**:
- Standalone component architecture design
- State management strategies (Signals, RxJS, NgRx)
- Smart/Dumb component pattern design
- Service layer architecture
- Technical specifications and blueprints
- Direct collaboration with Rex, Mason, and Vera
- **Delegates feature module architecture to clones** (e.g., one clone per module/subsystem)

**Tools**: WorkspaceTools, WorkspacePlanningTools, ThinkTools, AgentTeamTools, **AgentCloneTools**

---

### 4. Mason - Angular Craftsman 🔨
**Agent Key**: `mason_angular_craftsman`  
**Category**: `assist` (agent-helper)

Mason is the implementation coordinator who transforms Aria's blueprints into beautiful, functional Angular code. He coordinates implementation work and delegates component/service creation to clones. He takes pride in code quality, readability, and adherence to best practices.

**Key Capabilities**:
- Standalone component implementation coordination
- Service development with proper DI
- Reactive patterns (RxJS, Signals)
- TypeScript excellence with strict mode
- Enterprise coding standards
- Direct collaboration with Aria and Vera
- **Delegates component/service implementation to clones** (e.g., ONE component per clone)
- **Critical**: For 3+ components, ALWAYS uses clones to prevent context burnout

**Tools**: WorkspaceTools, WorkspacePlanningTools, ThinkTools, AgentTeamTools, **AgentCloneTools**

---

### 5. Vera - Angular Test Engineer 🛡️
**Agent Key**: `vera_angular_tester`  
**Category**: `assist` (agent-helper)

Vera is the quality guardian who coordinates comprehensive testing to ensure your Angular application meets the highest standards. She develops test strategies and delegates test implementation to clones for efficiency.

**Key Capabilities**:
- Test strategy development (test pyramid: 70% unit, 20% integration, 10% E2E)
- Playwright E2E test creation with Page Object Model
- Unit test specifications
- Accessibility testing
- Quality metrics and coverage analysis
- Direct collaboration with Aria and Mason
- **Delegates test creation to clones** (e.g., one clone per user flow or component group)
- **Critical**: For 3+ test flows, ALWAYS uses clones to prevent context burnout

**Tools**: WorkspaceTools, WorkspacePlanningTools, ThinkTools, AgentTeamTools, **AgentCloneTools**, BrowserPlaywrightTools

---

## Critical: Clone Delegation for Efficiency

### Why Clone Delegation Matters

**The Problem Without Clones**:
- Mason tries to implement 30 components in one session → Context burnout 💥
- Vera tries to write 100 tests in one session → Token costs explode 💸
- Rex tries to analyze 10 domains in one session → Quality degrades 📉
- Aria tries to design 15 modules in one session → Architectural inconsistency 🏚️

**The Solution With Clones**:
- Each specialist is a **coordinator**, not just an executor
- Clones handle **focused, time-bounded tasks** (15-30 min max)
- **ONE component per clone**, **ONE test flow per clone**, **ONE domain per clone**
- Specialists validate and integrate clone outputs
- Prevents context burnout and controls token costs

### Clone Delegation Strategy

| Specialist | When to Use Clones | Example Clone Task |
|------------|-------------------|--------------------|
| **Rex** | > 5 feature areas | "Analyze authentication requirements and identify Angular components needed" |
| **Aria** | > 5 modules | "Design user dashboard architecture with component structure and state management" |
| **Mason** | > 3 components | "Implement UserProfileComponent with form validation and avatar upload" |
| **Vera** | > 3 test flows | "Create E2E tests for authentication flow (login, logout, password reset)" |

### The Golden Rule

:::important
**Single Focus Rule**: Each clone gets **ONE** focused deliverable.

❌ **NEVER**: "Implement ComponentA, ComponentB, and ComponentC"  
✅ **ALWAYS**: "Implement ComponentA with these specific requirements"
:::

---

## Team Communication Pattern

### Direct Communication Mesh

```
        Diego (Orchestrator)
        ↓ (coordinates)
    ┌───┴───┬───────┬───────┐
    │       │       │       │
  Rex ←→ Aria ←→ Mason ←→ Vera
```

**Key Advantages**:
- ✅ Rex can clarify requirements directly with Aria
- ✅ Aria can guide Mason directly during implementation
- ✅ Aria and Vera collaborate on testability requirements
- ✅ Mason and Vera work together on test coverage
- ✅ No information loss through intermediary handoffs
- ✅ Diego maintains oversight and quality gates

### Escalation Protocol

Specialists escalate to Diego for:
- Conflicts or disagreements
- Resource issues or blockers
- Strategic decisions
- Scope changes
- Quality concerns

---

## Workspace Structure

**Primary Workspace**: `//angular_app`

```
//angular_app/
├── src/
│   ├── app/              # Application root (standalone components)
│   ├── components/       # Shared standalone components
│   ├── services/         # Angular services
│   ├── models/          # TypeScript interfaces/types
│   ├── directives/      # Custom directives
│   ├── pipes/           # Custom pipes
│   ├── guards/          # Route guards
│   └── interceptors/    # HTTP interceptors
├── tests/
│   ├── e2e/            # Playwright E2E tests
│   └── unit/           # Unit test specifications
├── docs/
│   ├── requirements/   # Requirements documentation
│   ├── architecture/   # Architecture documents
│   ├── implementation/ # Implementation notes
│   └── testing/        # Test strategies
├── .scratch/
│   ├── handoffs/       # Phase handoff documents
│   ├── analysis/       # Temporary analysis files
│   └── trash/          # Outdated files
└── Document_Library_Index.md  # Master document index
```

---

## Development Workflow

### Phase 1: Requirements Analysis (Rex)
1. Diego assigns requirements analysis to Rex
2. Rex analyzes requirements and creates technical specifications
3. Rex collaborates with Aria on feasibility
4. Diego validates specifications
5. Create handoff document for architecture phase

### Phase 2: Architecture Design (Aria)
1. Diego assigns architecture design to Aria
2. Aria designs component structure, state management, patterns
3. Aria collaborates with Rex for clarifications
4. Aria coordinates with Vera on testability
5. Diego validates architecture
6. Create handoff document for implementation phase

### Phase 3: Implementation (Mason)
1. Diego assigns implementation to Mason
2. Mason implements components, services, features
3. Mason consults Aria for architectural guidance
4. Mason coordinates with Vera on test coverage
5. Diego validates code quality
6. Create handoff document for testing phase

### Phase 4: Testing Strategy (Vera)
1. Diego assigns testing to Vera
2. Vera creates test strategies and Playwright tests
3. Vera collaborates with Aria on architecture
4. Vera works with Mason on implementation details
5. Diego validates test coverage
6. Document test results

### Phase 5: Integration & Delivery
1. Diego reviews all deliverables
2. Ensure documentation is complete
3. Final handoff to user
4. Lessons learned documentation

---

## Angular Enterprise Standards

### Architecture Patterns
- ✅ **Standalone components** (no NgModules)
- ✅ **Reactive patterns** (RxJS, Angular Signals)
- ✅ **Smart/Dumb pattern** (Container vs Presentational)
- ✅ **Dependency injection** (Angular DI system)
- ✅ **Lazy loading** (Route-based code splitting)

### Code Quality
- ✅ **TypeScript strict mode** enforced
- ✅ **Component size**: Under 300 lines
- ✅ **Method size**: Under 25 lines
- ✅ **Cyclomatic complexity**: Maximum 10 per method
- ✅ **Single Responsibility Principle** throughout

### Testing Standards
- ✅ **Test pyramid**: 70% unit, 20% integration, 10% E2E
- ✅ **Unit test coverage**: 80% minimum
- ✅ **E2E tests**: All critical user flows
- ✅ **Accessibility testing**: WCAG compliance
- ✅ **Cross-browser testing**: Via Playwright

---

## How to Use Your Team

### Starting a New Feature

Talk to **Diego** with your requirements:

```
"Diego, I need to build a feature that allows users to manage their profile. 
Users should be able to view, edit, and save their profile information including 
name, email, and avatar. We also need validation and error handling."
```

Diego will:
1. Create a project plan in the workspace planning tool
2. Assign requirements analysis to Rex
3. Coordinate the sequential workflow through all phases
4. Maintain quality gates between phases
5. Deliver the complete, tested feature

### Getting Status Updates

Ask **Diego**:
```
"Diego, what's the status of the profile management feature?"
```

Diego will provide:
- Current phase (Requirements, Architecture, Implementation, Testing)
- Completed deliverables
- Next steps
- Any blockers or concerns

### Making Changes

If requirements change mid-development, tell **Diego**:
```
"Diego, we need to add the ability to upload profile photos."
```

Diego will:
- Assess the impact on current phase
- Consult with relevant specialists
- Update the project plan
- Coordinate the change implementation

---

## Important Notes

### What This Team CAN Do
✅ Write Angular/TypeScript code  
✅ Design architecture and components  
✅ Create Playwright E2E tests  
✅ Analyze requirements  
✅ Document everything  
✅ Plan and coordinate development  
✅ Review code quality  

### What This Team CANNOT Do
❌ Run `ng generate` commands (Angular CLI)  
❌ Execute `npm install` or `npm run` commands  
❌ Run TypeScript compiler  
❌ Execute build processes  
❌ Deploy applications  

**Note**: If you need command execution capabilities, consider working with **Tim the Toolman** to create those tools first.

---

## Configuration Files

All agent configurations are stored in:
- `//project/agent_c_config/agents/diego_angular_orchestrator.yaml`
- `//project/agent_c_config/agents/rex_angular_requirements.yaml`
- `//project/agent_c_config/agents/aria_angular_architect.yaml`
- `//project/agent_c_config/agents/mason_angular_craftsman.yaml`
- `//project/agent_c_config/agents/vera_angular_tester.yaml`

All agents use:
- **Model**: `claude-sonnet-4-20250514`
- **Type**: Claude reasoning with extended thinking
- **Budget tokens**: 15,000 - 20,000 depending on role

---

## Getting Started

1. **Ensure the workspace exists**: Create `//angular_app` workspace if needed
2. **Talk to Diego**: Start by interacting with `diego_angular_orchestrator`
3. **Provide requirements**: Give Diego your feature requirements or business needs
4. **Let the team work**: Diego will coordinate the specialists through each phase
5. **Review deliverables**: Check documentation and code at each quality gate

---

## Tips for Success

### 🎯 Be Clear with Requirements
The better your requirements, the better the output. Provide business context, user needs, and constraints.

### 📋 Trust the Process
The sequential workflow (Requirements → Architecture → Implementation → Testing) ensures quality. Don't skip phases!

### 🔍 Review at Quality Gates
Diego will stop for your approval at critical points. Use these moments to provide feedback.

### 💬 Communicate Changes Early
If requirements change, tell Diego immediately. It's easier to adjust early than late.

### 📚 Check Documentation
The team produces comprehensive documentation. Review it to understand architectural decisions.

---

## Questions or Issues?

If you encounter any issues with your Angular development team:

1. **Ask Diego first**: He can often resolve issues by coordinating specialists
2. **Check agent configurations**: Review YAML files if behavior seems unexpected
3. **Consult the Design Bible**: `//project/docs/multi_agent_coordination_design_bible.md`
4. **Talk to Bobb**: I can help adjust agent personas or fix configuration issues!

---

**Your Angular enterprise development team is ready to build! Start by talking to Diego and let the specialists work their magic! 🚀✨**
