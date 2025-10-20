# Specialist Agent Guide

A comprehensive guide for building specialist (assist) agents that serve as team members with deep domain expertise, focusing on the core components currently available in the Agent C framework.

## When to Use Specialist Agent Type

**Primary Purpose**: Agents that serve specific technical roles in multi-agent teams, providing deep domain expertise to other agents

**Core Characteristics**:
- Deep domain expertise and specialization
- Team member role (serves other agents, NOT end users)
- Technical excellence and deliverable quality focus
- Professional standards and best practices emphasis
- Collaborative problem-solving with other agents
- Clear role boundaries within team structure
- Exposed via `AgentAssistTool` or `AgentTeamTools`

**Typical Scenarios**:
- Software architecture and design expertise
- Code craftsmanship and implementation
- Test strategy and quality assurance
- Requirements analysis and mining
- Domain-specific technical consultation
- Specialized analysis and recommendations
- Technical review and validation
- Standards and compliance expertise

**Key Requirements**:
- Must include 'assist' in agent category array (NOT 'domo')
- Should include Domain Knowledge Template component (core of specialists)
- Must include Team Collaboration component for team integration
- Focus on technical deliverables and quality outputs
- Clear role definition within team context
- NO human interaction protocols (serves agents, not users)

## Binary Component Decisions

For each component, make a clear **YES** or **NO** decision based on your specialist agent's specific needs:

### 1. Critical Interaction Guidelines Component

**Does this specialist agent access workspaces or file paths?**

- **YES** → Use this component *(Applies to 75% of Specialist agents)*
- **NO** → Skip this component

**Reference**: [`critical_interaction_guidelines_component.md`](../01_core_components/critical_interaction_guidelines_component.md)

**Why Include**: Prevents wasted work on non-existent paths, ensures consistent workspace verification behavior when delivering technical artifacts.

**When to Skip**: Pure consultation specialists without any file system access.

---

### 2. Human Pairing Protocol Component

**Does this specialist agent interact directly with end users?**

- **YES** → Not a specialist - use Domo agent type instead
- **NO** → Skip this component *(Applies to 100% of Specialist agents)*

**Why Skip**: Specialist agents serve other agents, NOT end users. This component is exclusive to Domo agents. Including it would create confusion about the agent's role.

**Critical Distinction**: The absence of human pairing protocols is a defining characteristic of specialist agents. They focus on technical excellence for agent consumers.

---

### 3. Reflection Rules Component

**Does this specialist have access to the ThinkTools?**

- **YES** → Use this component *(Applies to 85% of Specialist agents)*
- **NO** → Skip this component

**Reference**: [`reflection_rules_component.md`](../01_core_components/reflection_rules_component.md)

**Why Include**: Ensures systematic analysis of technical problems, improves recommendation quality through structured thinking, creates valuable reasoning for complex domain decisions.

**When to Skip**: Simple lookup or reference specialists where deep thinking isn't beneficial.

---

### 4. Workspace Organization Component

**Does this specialist use workspace tools for artifact delivery?**

- **YES** → Use this component *(Applies to 70% of Specialist agents)*
- **NO** → Skip this component

**Reference**: [`workspace_organization_component.md`](../01_core_components/workspace_organization_component.md)

**Why Include**: Standardizes artifact delivery, supports team collaboration through organized outputs, provides systematic structure for technical deliverables.

**When to Skip**: Consultation-only specialists who deliver recommendations verbally without file artifacts.

---

### 5. Critical Working Rules Component

**Does this specialist coordinate complex multi-step workflows internally?**

- **YES** → Consider this component (rare for specialists)
- **NO** → Skip this component *(Applies to 95% of Specialist agents)*

**Reference**: [`critical_working_rules_component.md`](../01_core_components/critical_working_rules_component.md)

**Why Skip**: Most specialists focus on specific technical deliverables without complex internal workflow management. This component is more relevant for orchestrators.

**When to Include**: Only for specialists with complex internal coordination requirements.

---

### 6. Planning & Coordination Component

**Does this specialist coordinate tasks or manage workflow stages?**

- **YES** → Consider this component (rare for specialists)
- **NO** → Skip this component *(Applies to 90% of Specialist agents)*

**Reference**: [`planning_coordination_component.md`](../01_core_components/planning_coordination_component.md)

**Why Skip**: Most specialists receive focused requests and deliver technical expertise without coordinating complex workflows. This is primarily an orchestrator concern.

**When to Include**: Only for specialists with significant coordination responsibilities.

---

### 7. Clone Delegation Component

**Does this specialist delegate work to clone agents?**

- **YES** → Consider this component (rare for specialists)
- **NO** → Skip this component *(Applies to 95% of Specialist agents)*

**Reference**: [`clone_delegation_component.md`](../01_core_components/clone_delegation_component.md)

**Why Skip**: Most specialists perform their specialized work directly rather than delegating. Clone delegation is primarily an orchestrator capability.

**When to Include**: Only for specialists that need to delegate portions of their specialized work.

---

### 8. Code Quality Standards Component

**Does this specialist write or modify code?**

- **YES** → Use appropriate language variant *(Applies to 60% of Specialist agents)*
- **NO** → Skip this component

**Reference**: 
- [`code_quality_python_component.md`](../01_core_components/code_quality_python_component.md)
- [`code_quality_csharp_component.md`](../01_core_components/code_quality_csharp_component.md)
- [`code_quality_typescript_component.md`](../01_core_components/code_quality_typescript_component.md)

**Why Include**: Ensures consistent code quality for coding specialists (architects, craftsmen, implementers), maintains professional development standards.

**When to Skip**: Non-coding specialists (test strategists, requirements analysts, process consultants).

---

### 9. Context Management Component

**Does this specialist manage long-running complex workflows with context burnout risk?**

- **YES** → Consider this component (rare for specialists)
- **NO** → Skip this component *(Applies to 90% of Specialist agents)*

**Reference**: [`context_management_component.md`](../01_core_components/context_management_component.md)

**Why Skip**: Most specialists handle focused requests without the long workflow risks that create context burnout. This is primarily an orchestrator concern.

**When to Include**: Only for specialists with unusually complex or long-running work patterns.

---

### 10. Team Collaboration Component

**Does this specialist work as part of a multi-agent team?**

- **YES** → Use this component *(Applies to 100% of team-based Specialist agents)*
- **NO** → Skip if standalone specialist (rare)

**Reference**: [`team_collaboration_component.md`](../01_core_components/team_collaboration_component.md)

**Why Include**: Essential for team integration - defines role boundaries, collaboration protocols, communication patterns with other specialists, escalation paths to orchestrator.

**When to Skip**: Only for standalone specialists not part of any team structure (rare).

---

### 11. Quality Gates Component

**Does this specialist need validation requirements for deliverables?**

- **YES** → Use this component *(Applies to 80% of Specialist agents)*
- **NO** → Skip this component

**Reference**: [`quality_gates_component.md`](../01_core_components/quality_gates_component.md)

**Why Include**: Ensures deliverable quality, provides clear success criteria, enables validation of specialist outputs, maintains professional standards.

**When to Skip**: Informal consultation specialists without formal deliverable requirements.

---

### 12. Domain Knowledge Template Component

**Does this specialist provide domain-specific expertise?**

- **YES** → Use this component *(Applies to 100% of Specialist agents)*
- **NO** → Not a specialist - reconsider agent type

**Reference**: [`domain_knowledge_template_component.md`](../01_core_components/domain_knowledge_template_component.md)

**Why Include**: Core of specialist agents - provides the deep expertise that defines their value, ensures proper methodologies, supports specialized decision-making.

**When to Skip**: Never for specialists - this is a defining characteristic.

## Typical Structure and Composition Order

Based on the binary component decisions above, here's the recommended persona organization for specialist agents:

### Component Ordering Principle

**Recommended Order (Foundation → Specialization → Collaboration → Quality)**:

1. **Core Guidelines First** (Critical Interaction Guidelines, Reflection Rules)
   - Establishes safety and thinking patterns
   - Foundation that all technical work builds upon

2. **Domain Expertise** (Domain Knowledge Template - CORE OF SPECIALIST)
   - The specialized knowledge that defines the specialist
   - Most important section - this is why the specialist exists

3. **Technical Standards** (Code Quality Standards if applicable)
   - Defines how technical work is performed
   - Sets quality and professional standards

4. **Collaboration Patterns** (Team Collaboration, Workspace Organization)
   - Defines how specialist integrates with team
   - Establishes communication and delivery protocols

5. **Quality Assurance** (Quality Gates)
   - Defines validation requirements for deliverables
   - Sets success criteria and standards

6. **Personality Last** (Communication style, approach)
   - Defines how the specialist expresses itself
   - Applied across all previous sections

This ordering ensures that the specialist's core domain expertise is prominent and well-supported by necessary technical and collaboration standards.

### Standard Specialist Agent Structure

```markdown
# Specialist Identity and Core Expertise
[Custom specialist identity, domain role, and primary technical focus]

## Critical Interaction Guidelines
[If workspace access - YES/NO decision]

## Reflection Rules
[If ThinkTools access - YES/NO decision]

## [Domain Name] Expertise
[CORE SECTION - Deep domain knowledge, methodologies, and specialized capabilities]

## Code Quality Requirements
[If coding specialist - Choose Python/C#/TypeScript variant - YES/NO decision]

## Team Collaboration Protocols
[YES - Team integration, role boundaries, communication patterns]

## Workspace Organization Guidelines
[If workspace tools - YES/NO decision]

## Quality Gates & Deliverable Standards
[YES - Validation requirements and success criteria]

# Personality and Technical Communication Style
[Professional, precise, technically focused communication approach]

# Reference Materials
[Links to technical standards, domain documentation, or methodology resources]
```

### Coding Specialist Structure

For architecture, craftsmanship, and implementation specialists:

```markdown
# Specialist Identity and Core Expertise
[Custom specialist identity, coding domain focus, and technical specialization]

## Critical Interaction Guidelines
[Workspace safety for code delivery]

## Reflection Rules
[Systematic analysis for technical decisions]

## [Language/Framework] Architecture/Craftsmanship Expertise
[CORE SECTION - Deep technical knowledge, design patterns, best practices]

## Code Quality Requirements - [Language]
[Language-specific development standards and professional practices]

## Team Collaboration Protocols
[Role in development team, communication with other specialists]

## Workspace Organization Guidelines
[Code delivery, artifact organization, documentation structure]

## Quality Gates & Deliverable Standards
[Code review criteria, validation requirements, acceptance standards]

# Personality and Technical Communication Style
[Precise, professional, technically accurate, collaborative]

# Reference Materials
[Language standards, framework documentation, design pattern resources]
```

### Analysis/Strategy Specialist Structure

For non-coding specialists like test strategists or requirements analysts:

```markdown
# Specialist Identity and Core Expertise
[Custom specialist identity, analysis domain focus, and expertise area]

## Critical Interaction Guidelines
[If workspace access for artifact delivery]

## Reflection Rules
[Systematic analysis for strategic recommendations]

## [Domain] Strategy/Analysis Expertise
[CORE SECTION - Deep domain knowledge, analytical methodologies, strategic frameworks]

## Team Collaboration Protocols
[Role in analysis team, communication with technical specialists]

## Workspace Organization Guidelines
[If workspace tools for deliverable organization]

## Quality Gates & Deliverable Standards
[Analysis criteria, recommendation validation, acceptance standards]

# Personality and Technical Communication Style
[Analytical, thorough, strategic, clearly articulated]

# Reference Materials
[Domain frameworks, analytical methodologies, industry standards]
```

### Minimal Specialist Structure

For simple consultation specialists:

```markdown
# Specialist Identity and Core Expertise
[Custom specialist identity and domain focus]

## [Domain Name] Expertise
[CORE SECTION - Specialized knowledge and capabilities]

## Team Collaboration Protocols
[Basic team integration and role boundaries]

# Personality and Technical Communication Style
[Professional and technically focused]
```

## Customization Guidance

### Focus Area Adaptations

**Software Architecture Specialists**:
- Deep focus on design patterns and architectural principles
- Include Code Quality Standards component
- Emphasize system design and structural decisions
- Include Reflection Rules for complex design analysis
- Focus on long-term technical vision and standards

**Code Craftsmanship Specialists**:
- Deep focus on implementation excellence and best practices
- Include Code Quality Standards component
- Emphasize clean code, refactoring, and maintainability
- Include Workspace Organization for code delivery
- Focus on tactical excellence and code quality

**Test Strategy Specialists**:
- Deep focus on testing methodologies and quality assurance
- NO Code Quality Standards (analysis focus, not implementation)
- Emphasize test planning, coverage, and validation strategies
- Include Quality Gates for test deliverable validation
- Focus on quality assurance and risk mitigation

**Requirements/Analysis Specialists**:
- Deep focus on domain understanding and requirements extraction
- NO Code Quality Standards (analysis focus, not implementation)
- Emphasize stakeholder communication and documentation
- Include Workspace Organization for requirements artifacts
- Focus on clarity, completeness, and traceability

### Domain-Specific Considerations

**Add Custom Sections For**:
- Specialized technical methodologies and frameworks
- Domain-specific tools and technologies
- Industry standards and compliance requirements
- Professional certifications and expertise areas
- Specialized communication protocols

**Adapt Components For**:
- **Domain Knowledge**: Customize depth and breadth for specialization area
- **Code Quality**: Choose language-appropriate variant and customize for frameworks
- **Team Collaboration**: Define specialist's specific role and boundaries
- **Quality Gates**: Adapt validation criteria to specialist's deliverables
- **Workspace Organization**: Customize for domain-specific artifact types

### Personality Customization

**Communication Style Options**:
- **Technical Expert**: Precise, detailed, standards-focused, authoritative
- **Collaborative Specialist**: Supportive, explanatory, guidance-oriented, helpful
- **Analytical Consultant**: Thorough, systematic, recommendation-focused, strategic
- **Pragmatic Craftsman**: Practical, efficient, solution-focused, experienced

**Maintain Specialist Characteristics**:
- Always technically focused and precise
- Professional standards and excellence emphasis
- Clear role boundaries within team
- Collaborative with other specialists
- Quality-driven deliverable focus
- NO user-facing communication patterns
- Agent-to-agent communication style

## Real Examples from the Ecosystem

### Software Architecture Specialist
**Agent**: `aria_csharp_architect.yaml`

**Component Selections**:
- ✅ Critical Interaction Guidelines
- ✅ Reflection Rules
- ❌ Human Pairing Protocol (assist agent, not domo)
- ✅ Workspace Organization
- ✅ Code Quality Standards (C#)
- ✅ Team Collaboration
- ✅ Quality Gates
- ✅ Domain Knowledge (C# architecture and design patterns)
- ❌ Planning & Coordination (not an orchestrator role)
- ❌ Clone Delegation (direct work, not coordination)

**Characteristics**: C# architecture expertise, design pattern focus, system design, structural decisions, long-term technical vision, team collaboration with other specialists

**Category**: `['douglas_bokf_orchestrator', 'assist', 'csharp', 'architecture']`

---

### Code Craftsmanship Specialist
**Agent**: `mason_csharp_craftsman.yaml`

**Component Selections**:
- ✅ Critical Interaction Guidelines
- ✅ Reflection Rules
- ❌ Human Pairing Protocol (assist agent, not domo)
- ✅ Workspace Organization
- ✅ Code Quality Standards (C#)
- ✅ Team Collaboration
- ✅ Quality Gates
- ✅ Domain Knowledge (C# implementation excellence)
- ❌ Planning & Coordination (not an orchestrator role)
- ❌ Clone Delegation (direct work, not coordination)

**Characteristics**: C# implementation expertise, clean code focus, refactoring mastery, tactical excellence, code quality emphasis, team collaboration for implementation work

**Category**: `['douglas_bokf_orchestrator', 'assist', 'csharp', 'craftsmanship']`

---

### Test Strategy Specialist
**Agent**: `vera_test_strategist.yaml`

**Component Selections**:
- ✅ Critical Interaction Guidelines
- ✅ Reflection Rules
- ❌ Human Pairing Protocol (assist agent, not domo)
- ✅ Workspace Organization
- ❌ Code Quality Standards (analysis focus, not implementation)
- ✅ Team Collaboration
- ✅ Quality Gates
- ✅ Domain Knowledge (Testing methodologies and strategies)
- ❌ Planning & Coordination (not an orchestrator role)
- ❌ Clone Delegation (direct work, not coordination)

**Characteristics**: Test strategy expertise, quality assurance focus, test planning and coverage analysis, risk-based testing, validation strategies, team collaboration for quality oversight

**Category**: `['douglas_bokf_orchestrator', 'assist', 'testing', 'quality_assurance']`

---

### Requirements Analysis Specialist
**Agent**: `rex_requirements_miner.yaml`

**Component Selections**:
- ✅ Critical Interaction Guidelines
- ✅ Reflection Rules
- ❌ Human Pairing Protocol (assist agent, not domo)
- ✅ Workspace Organization
- ❌ Code Quality Standards (analysis focus, not implementation)
- ✅ Team Collaboration
- ✅ Quality Gates
- ✅ Domain Knowledge (Requirements analysis and domain modeling)
- ❌ Planning & Coordination (not an orchestrator role)
- ❌ Clone Delegation (direct work, not coordination)

**Characteristics**: Requirements extraction expertise, domain analysis focus, stakeholder communication, documentation excellence, traceability emphasis, team collaboration for requirements clarity

**Category**: `['douglas_bokf_orchestrator', 'assist', 'requirements', 'analysis']`

---

## Component Integration Benefits

### Why Binary Decisions Work

**For Specialist Agents Specifically**:
- **Technical Focus Clarity**: Binary decisions keep specialists focused on domain expertise without orchestration overhead
- **Role Boundary Definition**: Component-based approach clearly defines specialist vs. orchestrator vs. domo roles
- **Quality Standards**: Components enforce professional excellence in specialized deliverables
- **Team Integration**: Binary choices create predictable collaboration patterns

**Quality Outcomes**:
- **Deep Expertise**: Domain Knowledge Template ensures comprehensive specialization
- **Professional Standards**: Code Quality components maintain technical excellence
- **Clear Collaboration**: Team Collaboration component defines effective integration
- **Quality Deliverables**: Quality Gates component ensures output validation
- **NO Role Confusion**: Absence of Human Pairing Protocol clearly defines agent-serving role

### Integration Patterns

**Essential Component Combinations**:
- Domain Knowledge + Team Collaboration = Effective specialist integration
- Domain Knowledge + Quality Gates = Validated expertise delivery
- Code Quality + Domain Knowledge = Technical excellence (for coding specialists)
- Reflection Rules + Domain Knowledge = Deep analytical capability
- Workspace Organization + Team Collaboration = Organized artifact delivery

**Core Integration Benefits**:
- **Technical Excellence**: Domain knowledge + quality standards ensure professional work
- **Team Effectiveness**: Collaboration protocols enable smooth multi-agent coordination
- **Clear Deliverables**: Quality gates + workspace organization provide validated outputs
- **Deep Analysis**: Reflection rules support complex domain decision-making
- **Role Clarity**: 'assist' category + NO human protocols define agent-serving purpose

### Specialist-Specific Success Patterns

**Domain Expertise Delivery**:
- Deep specialized knowledge as core value proposition
- Clear methodologies and frameworks
- Professional standards and best practices
- Systematic approach to domain problems

**Team Collaboration**:
- Direct specialist-to-specialist communication (via AgentTeamTools)
- Clear role boundaries and responsibilities
- Escalation paths to orchestrator
- Collaborative problem-solving protocols

**Quality Deliverables**:
- Clear success criteria and validation requirements
- Professional standards for all outputs
- Systematic quality assurance
- Artifact organization and documentation

**Agent-to-Agent Communication**:
- Technical precision without user-facing protocols
- Efficient information exchange
- Assumption of technical competence in consumer
- Focus on deliverable quality over explanation

## Proper YAML Configuration Structure

**CRITICAL**: Agent YAML files must follow the exact field order shown below to prevent agent loading failures.

### Required Field Order

```yaml
version: 2
key: your_specialist_key_here
name: "Your Specialist Display Name"
model_id: "claude-3.5-sonnet"
agent_description: |
  Brief description of the specialist's domain expertise and role.
tools:
  - ThinkTools
  - WorkspaceTools
  # Additional tools as needed for specialization
  # NOTE: Specialists typically do NOT have AgentTools or WorkspacePlanningTools
agent_params:
  type: "claude_reasoning"
  budget_tokens: 20000
  max_tokens: 64000
  # Additional parameters as needed
category:
  - "orchestrator_agent_key"  # If part of orchestrated team
  - "assist"  # REQUIRED - defines agent-serving role
  - "domain_area"
  - "specialization"
  # Additional categories as needed
persona: |
  # The persona content MUST be LAST
  # This contains all your component selections and custom instructions
```

### Critical Structure Rules for Specialists

1. **Field Order Matters**: Fields must appear in the exact order shown above
2. **Persona Must Be LAST**: The persona field must always be the final field in the YAML file
3. **Category Requirements**: 
   - MUST include "assist" (NOT "domo")
   - Include orchestrator's agent key if part of team
   - Include domain and specialization identifiers
4. **Tool Selection**: Specialists typically have focused toolsets (ThinkTools, WorkspaceTools) without orchestration tools
5. **Reasoning Model**: Specialists benefit from claude_reasoning type for deep domain analysis

### Common Specialist Configuration Patterns

**Team Member Specialist**:
```yaml
category:
  - "orchestrator_agent_key"  # Team lead's key
  - "assist"
  - "domain_area"
  - "specialization"
tools:
  - ThinkTools
  - WorkspaceTools
```

**Coding Specialist**:
```yaml
category:
  - "orchestrator_agent_key"
  - "assist"
  - "language_name"
  - "specialization"  # architecture, craftsmanship, etc.
tools:
  - ThinkTools
  - WorkspaceTools
  # Language-specific tools if needed
```

**Analysis/Strategy Specialist**:
```yaml
category:
  - "orchestrator_agent_key"
  - "assist"
  - "domain_area"  # testing, requirements, etc.
  - "specialization"
tools:
  - ThinkTools
  - WorkspaceTools
  # Analysis-specific tools if needed
```

**Standalone Specialist** (not part of team):
```yaml
category:
  - "assist"
  - "domain_area"
  - "specialization"
```

### Configuration Anti-Patterns to Avoid

❌ **WRONG - Includes 'domo' category:**
```yaml
category:
  - "domo"  # ← WRONG! Specialists use 'assist', not 'domo'
  - "assist"
```

❌ **WRONG - Includes orchestration tools unnecessarily:**
```yaml
tools:
  - ThinkTools
  - WorkspaceTools
  - WorkspacePlanningTools  # ← WRONG! Not needed for most specialists
  - AgentTools  # ← WRONG! Specialists don't delegate
```

❌ **WRONG - Includes Human Pairing Protocol in persona:**
```markdown
## Human Pairing Protocol  # ← WRONG! Specialists don't interact with users
```

✅ **CORRECT - Specialist configuration:**
```yaml
category:
  - "orchestrator_key"
  - "assist"  # ← CORRECT! Agent-serving role
  - "domain"
tools:
  - ThinkTools
  - WorkspaceTools  # ← CORRECT! Focused toolset
```

## Getting Started

### Step-by-Step Specialist Creation

1. **Define Domain Expertise**: Clearly identify the specialized knowledge and skills this specialist provides
2. **Identify Team Context**: Determine which orchestrator (if any) this specialist serves
3. **Make Binary Decisions**: Go through each component with clear YES/NO choices based on specialist needs
4. **Design Domain Knowledge Section**: Create comprehensive expertise section (CORE OF SPECIALIST)
5. **Choose Technical Standards**: Select appropriate Code Quality component if coding specialist
6. **Define Team Collaboration**: Specify role boundaries and communication patterns
7. **Create Proper YAML Structure**: Use the exact field order with persona LAST
8. **Structure the Persona**: Use the typical specialist structure as template
9. **Define Quality Requirements**: Specify deliverable validation criteria
10. **Customize Communication Style**: Define professional, technical communication approach
11. **Validate YAML Structure**: Verify fields are in correct order with persona LAST
12. **Validate Category Configuration**: Ensure 'assist' is present (NOT 'domo')
13. **Test Domain Expertise**: Verify specialist provides deep, valuable domain knowledge
14. **Validate Team Integration**: Ensure collaboration protocols enable effective teamwork

### Quality Checklist

**Required for All Specialist Agents**:
- ✅ Includes 'assist' in category array (NOT 'domo')
- ✅ Domain Knowledge Template component included (CORE OF SPECIALIST)
- ✅ Team Collaboration component included (if part of team)
- ✅ Clear technical focus and specialized expertise
- ✅ Professional deliverable standards defined
- ❌ Does NOT include Human Pairing Protocol
- ❌ Does NOT include orchestration tools unnecessarily

**Recommended Best Practices**:
- ✅ Reflection Rules for deep domain analysis
- ✅ Workspace Organization for artifact delivery
- ✅ Code Quality Standards for coding specialists
- ✅ Quality Gates for deliverable validation
- ✅ Clear role boundaries within team context
- ✅ Technical precision in communication
- ✅ Professional standards emphasis

**Quality Validation**:
- ✅ Component selections match specialist's domain and role
- ✅ Domain Knowledge section is comprehensive and deep
- ✅ Category configuration properly defines agent-serving role
- ✅ Tool selection is focused on specialization needs
- ✅ Team collaboration protocols are clear and effective
- ✅ Quality standards for deliverables are explicit
- ✅ Communication style is professional and technically precise
- ✅ NO user-facing protocols or patterns included

## Critical Specialist Patterns

### Domain Knowledge as Core Value

**What Makes a Great Specialist**:
- **Deep Expertise**: Comprehensive knowledge in specialization area
- **Systematic Methodologies**: Clear frameworks and approaches
- **Professional Standards**: Industry best practices and quality focus
- **Collaborative Integration**: Effective teamwork with other specialists
- **Quality Deliverables**: Validated, professional outputs

### Role Boundary Clarity

**Specialist vs. Orchestrator**:
- Specialists perform specialized work directly
- Orchestrators coordinate and delegate
- Specialists have deep domain expertise
- Orchestrators have broad coordination capability

**Specialist vs. Domo**:
- Specialists serve other agents
- Domos serve end users
- Specialists use 'assist' category
- Domos use 'domo' category
- Specialists have NO human pairing protocols
- Domos include user interaction guidelines

### Team Integration Best Practices

**Direct Communication via AgentTeamTools**:
- Specialist-to-specialist collaboration without "telephone game"
- Clear role boundaries and responsibilities
- Escalation paths to orchestrator for conflicts
- Collaborative problem-solving protocols

**Category-Based Team Formation**:
```yaml
# Specialist includes orchestrator key in category
category:
  - "douglas_bokf_orchestrator"  # Team membership
  - "assist"
  - "csharp"
  - "architecture"
```

**Orchestrator Configures Team Access**:
```yaml
# Orchestrator includes specialist keys in category
category:
  - "domo"
  - "orchestrator"
  - "aria_csharp_architect"
  - "mason_csharp_craftsman"
  - "vera_test_strategist"
  - "rex_requirements_miner"
```

This binary component approach ensures that every Specialist agent provides deep domain expertise, effective team collaboration, professional deliverables, and clear role boundaries while maintaining the flexibility to specialize for specific technical domains using the core components currently available.
