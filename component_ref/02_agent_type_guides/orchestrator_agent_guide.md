# Orchestrator Agent Guide

A comprehensive guide for building agents that coordinate teams or manage complex workflows, focusing on the core components currently available in the Agent C framework.

## When to Use Orchestrator Agent Type

**Primary Purpose**: Agents designed to coordinate teams of specialist agents or manage complex multi-stage workflows

**Core Characteristics**:
- Team and workflow coordination
- Heavy clone delegation and task management
- Context management and state tracking
- Quality oversight and validation gates
- Progress monitoring and handoff coordination
- Strategic planning and decomposition
- Sequential processing of complex work

**Typical Scenarios**:
- Multi-stage workflow orchestration
- Team coordination with specialist agents
- Complex project management requiring multiple phases
- Quality-gated processes with validation checkpoints
- Sequential processing of interdependent tasks
- Context-sensitive delegation and monitoring
- Long-running projects requiring state persistence

**Key Requirements**:
- Must include 'domo' in category array if user-facing
- Should include WorkspacePlanningTools for delegation tracking
- Requires context management strategies to prevent burnout
- Must implement quality gates for critical validation points
- Needs clear handoff protocols between phases

## Binary Component Decisions

For each component, make a clear **YES** or **NO** decision based on your agent's specific needs:

### 1. Critical Interaction Guidelines Component

**Does this orchestrator agent access workspaces or file paths?**

- **YES** → Use this component *(Applies to 95% of Orchestrator agents)*
- **NO** → Skip this component

**Reference**: [`critical_interaction_guidelines_component.md`](../01_core_components/critical_interaction_guidelines_component.md)

**Why Include**: Prevents wasted work on non-existent paths, ensures consistent workspace verification behavior, critical for state persistence and handoff coordination.

**When to Skip**: Theoretical orchestrators without any file system access (extremely rare).

---

### 2. Reflection Rules Component

**Does this orchestrator have access to the ThinkTools?**

- **YES** → Use this component *(Applies to 100% of Orchestrator agents)*
- **NO** → Skip this component (not recommended for orchestrators)

**Reference**: [`reflection_rules_component.md`](../01_core_components/reflection_rules_component.md)

**Why Include**: Essential for strategic decision-making, task decomposition, delegation planning, progress assessment, and quality validation. Orchestrators must think systematically about complex workflows.

**When to Skip**: Never recommended - orchestrators require structured thinking for effective coordination.

---

### 3. Workspace Organization Component

**Does this orchestrator use workspace tools for state management and coordination?**

- **YES** → Use this component *(Applies to 95% of Orchestrator agents)*
- **NO** → Skip this component

**Reference**: [`workspace_organization_component.md`](../01_core_components/workspace_organization_component.md)

**Why Include**: Standardizes state tracking, enables handoff coordination, supports long-term project persistence, provides systematic organization for multi-phase workflows.

**When to Skip**: Orchestrators without state persistence needs (extremely rare).

---

### 4. Planning & Coordination Component

**Does this orchestrator coordinate multiple tasks, phases, or agents?**

- **YES** → Use this component *(Applies to 100% of Orchestrator agents)*
- **NO** → Not an orchestrator - reconsider agent type

**Reference**: [`planning_coordination_component.md`](../01_core_components/planning_coordination_component.md)

**Why Include**: Core orchestrator capability - provides systematic workflow management, delegation tracking, quality gates, and progress monitoring.

**When to Skip**: Never for orchestrators - this is a defining characteristic.

---

### 5. Clone Delegation Component

**Does this orchestrator delegate tasks to clone agents?**

- **YES** → Use this component *(Applies to 100% of Orchestrator agents)*
- **NO** → Not an orchestrator - reconsider agent type

**Reference**: [`clone_delegation_component.md`](../01_core_components/clone_delegation_component.md)

**Why Include**: Core orchestrator capability - provides systematic clone task creation, focused delegation, recovery protocols, and proper task sizing.

**When to Skip**: Never for orchestrators - clone delegation is fundamental.

---

### 6. Context Management Component

**Does this orchestrator need to manage context windows and prevent burnout?**

- **YES** → Use this component *(Applies to 100% of Orchestrator agents)*
- **NO** → Not recommended - context burnout is a critical risk

**Reference**: [`context_management_component.md`](../01_core_components/context_management_component.md)

**Why Include**: Critical for orchestrators - prevents context burnout during long workflows, enables proper state tracking, supports recovery protocols, ensures sustainable operation.

**When to Skip**: Never recommended - orchestrators are highly susceptible to context burnout.

---

### 7. Quality Gates Component

**Does this orchestrator need validation checkpoints before proceeding?**

- **YES** → Use this component *(Applies to 100% of Orchestrator agents)*
- **NO** → Not recommended - quality oversight is core responsibility

**Reference**: [`quality_gates_component.md`](../01_core_components/quality_gates_component.md)

**Why Include**: Core orchestrator responsibility - ensures output validation, provides signoff requirements, enables recovery from failures, maintains workflow quality.

**When to Skip**: Never recommended - quality oversight is fundamental to orchestration.

---

### 8. Team Collaboration Component

**Does this orchestrator coordinate specialist agents in a team?**

- **YES** → Use this component *(Applies to 60% of Orchestrator agents)*
- **NO** → Skip if only coordinating clones without specialist team members

**Reference**: [`team_collaboration_component.md`](../01_core_components/team_collaboration_component.md)

**Why Include**: Enables direct communication with specialist agents, reduces "telephone game" effects, supports complex collaborative work, provides escalation protocols.

**When to Skip**: Sequential workflow orchestrators that only delegate to clones without specialist team members.

---

### 9. Code Quality Standards Component

**Does this orchestrator write or modify code?**

- **YES** → Use appropriate language variant
- **NO** → Skip this component *(Applies to 80% of Orchestrator agents)*

**Reference**: 
- [`code_quality_python_component.md`](../01_core_components/code_quality_python_component.md)
- [`code_quality_csharp_component.md`](../01_core_components/code_quality_csharp_component.md)
- [`code_quality_typescript_component.md`](../01_core_components/code_quality_typescript_component.md)

**Why Include**: Only if orchestrator directly generates code rather than delegating coding tasks.

**When to Skip**: Most orchestrators delegate coding work to specialist agents or clones.

---

### 10. Domain Knowledge Template Component

**Does this orchestrator specialize in a specific domain?**

- **YES** → Use domain-specific variant *(Applies to 80% of Orchestrator agents)*
- **NO** → Skip if general-purpose orchestrator

**Reference**: [`domain_knowledge_template_component.md`](../01_core_components/domain_knowledge_template_component.md)

**Why Include**: Provides specialized expertise for domain-specific workflows, ensures proper process adherence, supports compliance requirements.

**When to Skip**: General-purpose workflow orchestrators without domain specialization.

## Typical Structure and Composition Order

Based on the binary component decisions above, here's the recommended persona organization for orchestrator agents:

### Component Ordering Principle

**Recommended Order (Foundation → Coordination → Specialization → Personality)**:

1. **Core Guidelines First** (Critical Interaction Guidelines, Reflection Rules)
   - Establishes safety and systematic thinking patterns
   - Foundation for all orchestration activities

2. **Orchestration Framework** (Context Management, Planning & Coordination, Clone Delegation, Quality Gates)
   - Defines core orchestration capabilities
   - Sets workflow management standards

3. **Collaboration Patterns** (Team Collaboration, Workspace Organization)
   - Defines agent coordination and state management
   - Establishes handoff and communication protocols

4. **Domain Expertise** (Custom sections, Domain Knowledge)
   - Specialized knowledge and workflows
   - Built on top of orchestration foundation

5. **Personality Last** (Communication style, approach)
   - Defines how the orchestrator expresses itself
   - Applied across all previous sections

This ordering ensures that fundamental orchestration patterns are established before adding specialized domain capabilities and personality customization.

### Standard Orchestrator Agent Structure

```markdown
# Orchestrator Identity and Core Mission
[Custom orchestrator identity, coordination role, and primary purpose]

## Critical Interaction Guidelines
[YES - workspace access for state persistence]

## Reflection Rules
[YES - systematic thinking for strategic decisions]

## Context Management Guidelines
[YES - critical for orchestrator sustainability]

## Planning & Coordination Framework
[YES - core orchestrator capability]

## Clone Delegation Protocols
[YES - core orchestrator capability]

## Quality Gates & Validation
[YES - core orchestrator responsibility]

## Workspace Organization Guidelines
[YES - state tracking and handoff coordination]

## Team Collaboration Protocols
[If coordinating specialist agents - YES/NO decision]

## [Domain Name] Workflow Expertise
[Custom domain knowledge and specialized processes]

# Personality and Coordination Style
[Custom personality traits, communication approach, and coordination style]

# Reference Materials
[Links to workflow documentation, standards, or process resources]
```

### Minimal Orchestrator Agent Structure

For simple sequential workflow orchestrators:

```markdown
# Orchestrator Identity and Core Mission
[Custom orchestrator identity and workflow focus]

## Critical Interaction Guidelines
[Workspace safety]

## Reflection Rules
[Systematic thinking]

## Context Management Guidelines
[Context burnout prevention]

## Planning & Coordination Framework
[Workflow management]

## Clone Delegation Protocols
[Task delegation]

## Quality Gates & Validation
[Quality oversight]

# Personality and Coordination Style
[Coordination approach and communication]
```

### Team-Based Orchestrator Structure

For orchestrators coordinating specialist agent teams:

```markdown
# Orchestrator Identity and Core Mission
[Custom orchestrator identity, team coordination role, and strategic purpose]

## Critical Interaction Guidelines
[Workspace safety and path verification]

## Reflection Rules
[Systematic strategic thinking and decision-making]

## Context Management Guidelines
[Proactive context window management and state preservation]

## Planning & Coordination Framework
[Comprehensive workflow and team coordination management]

## Clone Delegation Protocols
[Focused task delegation for time-bounded execution]

## Quality Gates & Validation
[Multi-phase validation and signoff requirements]

## Team Collaboration Protocols
[Direct specialist communication and escalation paths]

## Workspace Organization Guidelines
[State tracking, handoff coordination, and artifact management]

## [Domain Name] Workflow Expertise
[Specialized domain processes and methodologies]

# Personality and Coordination Style
[Strategic, systematic, quality-focused coordination approach]

# Reference Materials
[Workflow standards, team documentation, process guidelines]
```

## Customization Guidance

### Focus Area Adaptations

**Sequential Workflow Orchestrators**:
- Focus on phase-by-phase progression
- Emphasize quality gates between stages
- Include clear handoff protocols
- Use planning tools for stage tracking
- Implement context management for long workflows

**Team Coordination Orchestrators**:
- Include Team Collaboration component
- Emphasize specialist communication protocols
- Focus on parallel workstream coordination
- Include escalation and conflict resolution
- Use planning tools for delegation tracking

**Domain-Specific Orchestrators**:
- Include Domain Knowledge Template component
- Emphasize process compliance and standards
- Focus on domain-specific validation requirements
- Include specialized quality criteria
- Adapt planning approach to domain conventions

**Hybrid Orchestrators**:
- Combine sequential and team coordination patterns
- Balance direct work with delegation
- Include appropriate code quality if generating code
- Use flexible planning structures
- Implement robust recovery protocols

### Domain-Specific Considerations

**Add Custom Sections For**:
- Industry-specific workflow stages and requirements
- Compliance and regulatory oversight protocols
- Specialized validation criteria and quality standards
- Domain-specific team roles and responsibilities
- Technical standards and methodologies

**Adapt Components For**:
- **Planning & Coordination**: Customize for domain-specific workflow stages
- **Quality Gates**: Adapt validation criteria to domain requirements
- **Clone Delegation**: Tailor task templates for domain activities
- **Team Collaboration**: Define roles based on domain specializations
- **Context Management**: Adjust preservation strategies for workflow complexity

### Personality Customization

**Coordination Style Options**:
- **Strategic Manager**: High-level oversight, delegation-focused, quality-driven
- **Systematic Coordinator**: Methodical, detail-oriented, process-focused
- **Collaborative Leader**: Team-oriented, communication-focused, supportive
- **Technical Orchestrator**: Precision-focused, standards-driven, technically precise

**Maintain Orchestrator Characteristics**:
- Strategic planning and decomposition
- Clear delegation with focused tasks
- Quality oversight and validation
- Proactive context management
- Recovery-first design thinking
- State persistence and handoff clarity
- Professional coordination approach

## Real Examples from the Ecosystem

### Domain-Specific Workflow Orchestrator
**Agent**: `douglas_bokf_orchestrator.yaml`

**Component Selections**:
- ✅ Critical Interaction Guidelines
- ✅ Reflection Rules
- ✅ Context Management
- ✅ Planning & Coordination
- ✅ Clone Delegation
- ✅ Quality Gates
- ✅ Workspace Organization
- ✅ Team Collaboration (coordinates specialist team)
- ❌ Code Quality Standards (delegates coding work)
- ✅ Domain Knowledge (insurance underwriting workflows)

**Characteristics**: Insurance underwriting orchestration, team coordination with specialists, sequential multi-stage processing, quality-gated workflow, domain compliance focus

**Architecture Pattern**: Hub-and-Spoke Coordination with Direct Communication Mesh for specialists

---

## Component Integration Benefits

### Why Binary Decisions Work

**For Orchestrator Agents Specifically**:
- **Clear Coordination Model**: Binary decisions create predictable delegation and oversight patterns
- **Context Sustainability**: Component-based approach prevents context burnout through systematic management
- **Quality Assurance**: Components enforce validation gates and recovery protocols
- **Team Clarity**: Binary choices define clear roles and communication patterns

**Quality Outcomes**:
- **Sustainable Operations**: Context management prevents mid-workflow failures
- **Reliable Delegation**: Clone delegation component ensures focused, recoverable tasks
- **Quality Outputs**: Quality gates component maintains standards throughout workflow
- **Clear Progress**: Planning tools provide transparent state tracking

### Integration Patterns

**Essential Component Combinations**:
- Planning & Coordination + Clone Delegation = Systematic task management
- Context Management + Quality Gates = Sustainable quality oversight
- Team Collaboration + Planning & Coordination = Effective specialist coordination
- Workspace Organization + Context Management = Reliable state persistence

**Core Integration Benefits**:
- **Sustainable Orchestration**: Context management prevents operational failures
- **Quality Workflows**: Quality gates ensure validation at critical points
- **Clear Delegation**: Clone delegation provides focused, recoverable tasks
- **Transparent Progress**: Planning tools enable state tracking and recovery
- **Team Effectiveness**: Collaboration components support specialist coordination

### Orchestrator-Specific Success Patterns

**Context Management**:
- Proactive state preservation before context limits
- Planning tool usage for resumable workflows
- Clear handoff protocols between phases
- Workspace-based state tracking

**Clone Delegation**:
- Single focused deliverable per clone (15-30 min tasks)
- Never assign task sequences to a single clone
- Clear success criteria and validation requirements
- Recovery-first task design

**Quality Gates**:
- Use `requires_completion_signoff` for critical checkpoints
- Validate outputs before proceeding to next phase
- Implement fallback protocols for quality failures
- Document validation criteria explicitly

**Team Coordination**:
- Direct specialist communication reduces "telephone game"
- Clear escalation paths to orchestrator
- Role boundaries and collaboration protocols
- Conflict resolution procedures

## Proper YAML Configuration Structure

**CRITICAL**: Agent YAML files must follow the exact field order shown below to prevent agent loading failures.

### Required Field Order

```yaml
version: 2
key: your_orchestrator_key_here
name: "Your Orchestrator Display Name"
model_id: "claude-3.5-sonnet"
agent_description: |
  Brief description of the orchestrator's coordination purpose and scope.
tools:
  - ThinkTools
  - WorkspaceTools
  - WorkspacePlanningTools
  - AgentTools  # For clone delegation
  - AgentTeamTools  # If coordinating specialist team
  # Additional tools as needed
agent_params:
  type: "claude_reasoning"
  budget_tokens: 20000
  max_tokens: 64000
  # Additional parameters as needed
category:
  - "domo"  # If user-facing
  - "orchestrator"
  - "your_domain"
  # Specialist agent keys if team coordinator
  # Additional categories as needed
persona: |
  # The persona content MUST be LAST
  # This contains all your component selections and custom instructions
```

### Critical Structure Rules for Orchestrators

1. **Field Order Matters**: Fields must appear in the exact order shown above
2. **Persona Must Be LAST**: The persona field must always be the final field in the YAML file
3. **Required Tools**: Orchestrators typically need WorkspacePlanningTools and AgentTools minimum
4. **Team Categories**: If coordinating specialists, include their agent keys in category array
5. **Reasoning Model**: Orchestrators benefit from claude_reasoning type for strategic decisions

### Common Orchestrator Configuration Patterns

**User-Facing Orchestrator**:
```yaml
category:
  - "domo"
  - "orchestrator"
  - "domain_name"
```

**Team Coordinator Orchestrator**:
```yaml
category:
  - "domo"
  - "orchestrator"
  - "specialist_agent_1_key"
  - "specialist_agent_2_key"
  - "domain_name"
tools:
  - ThinkTools
  - WorkspaceTools
  - WorkspacePlanningTools
  - AgentTools
  - AgentTeamTools
```

**Backend Orchestrator** (assists other agents):
```yaml
category:
  - "assist"
  - "orchestrator"
  - "domain_name"
```

## Getting Started

### Step-by-Step Orchestrator Creation

1. **Define Coordination Scope**: Clearly identify the workflow or team this orchestrator will coordinate
2. **Choose Architecture Pattern**: Sequential, Hub-and-Spoke, or Direct Communication Mesh
3. **Make Binary Decisions**: Go through each component with clear YES/NO choices based on orchestration needs
4. **Design Workflow Stages**: Map out phases, validation points, and handoff requirements
5. **Plan Delegation Strategy**: Identify what work will be cloned vs. delegated to specialists
6. **Create Proper YAML Structure**: Use the exact field order with persona LAST
7. **Structure the Persona**: Use the typical orchestrator structure as template
8. **Define Quality Gates**: Specify validation requirements and signoff criteria
9. **Implement Context Management**: Plan state preservation and recovery protocols
10. **Customize Domain Expertise**: Add specialized workflow knowledge as needed
11. **Define Coordination Style**: Set communication approach and oversight personality
12. **Validate YAML Structure**: Verify fields are in correct order with persona LAST
13. **Test Delegation Patterns**: Verify clone tasks are properly scoped (15-30 min)
14. **Validate Quality Gates**: Ensure validation points are properly positioned

### Quality Checklist

**Required for All Orchestrator Agents**:
- ✅ Includes 'domo' in category array if user-facing
- ✅ WorkspacePlanningTools equipped for delegation tracking
- ✅ Context Management component included
- ✅ Planning & Coordination component included
- ✅ Clone Delegation component included
- ✅ Quality Gates component included
- ✅ Clear workflow stages and handoff protocols defined

**Recommended Best Practices**:
- ✅ Reflection Rules for strategic decision-making
- ✅ Workspace Organization for state persistence
- ✅ Team Collaboration if coordinating specialists
- ✅ Recovery protocols for context burnout scenarios
- ✅ Clear validation criteria at quality gates
- ✅ Single focused deliverable per clone task
- ✅ Proactive context preservation strategies

**Quality Validation**:
- ✅ Component selections match orchestration needs
- ✅ Clone tasks properly scoped (15-30 min, single deliverable)
- ✅ Quality gates positioned at critical validation points
- ✅ Context management strategies prevent burnout
- ✅ Clear handoff protocols between phases
- ✅ Recovery procedures defined for failures
- ✅ Coordination style and approach clearly defined
- ✅ Domain-specific workflows properly structured

## Critical Orchestrator Patterns

### Clone Task Sizing

**❌ NEVER DO - Task Sequences**:
```
"1. Analyze domain, 2. Identify capabilities, 3. Document integration, 4. Create summary"
```

**✅ CORRECT - Single Focused Tasks**:
```
Task 1: "Analyze domain requirements and extract key business capabilities"
Task 2: "Identify integration points for the documented capabilities"  
Task 3: "Create stakeholder-friendly summary of domain analysis"
```

### Context Burnout Prevention

**Proactive Strategies**:
- Monitor conversation length and complexity
- Preserve state to workspace before limits
- Use planning tools for resumable workflows
- Implement clear handoff protocols
- Design for recovery from context exhaustion

**Recovery Protocols**:
1. Recognize failure type (context vs. tool vs. quality)
2. Preserve partial work to workspace
3. Update planning tool with progress
4. Decompose remaining work into fresh tasks
5. Resume with clear context from preserved state

### Quality Gate Implementation

**Use Planning Tool Features**:
- `requires_completion_signoff: true` for critical validations
- `completion_report` to capture key deliverables
- `completion_signoff_by` for accountability
- Task dependencies for sequential processing
- Clear validation criteria in task descriptions

This binary component approach ensures that every Orchestrator agent provides systematic coordination, sustainable operations, quality oversight, and effective delegation while maintaining the flexibility to specialize for specific domains and workflow patterns using the core components currently available.
