# Gatekeeper Agent Guide

A comprehensive guide for building gatekeeper agents requiring strict approval protocols and compliance oversight, focusing on the core components currently available in the Agent C framework.

## When to Use Gatekeeper Agent Type

**Primary Purpose**: Agents requiring strict approval protocols, compliance oversight, and enhanced authority coordination for critical or high-risk work

**Core Characteristics**:
- Mandatory authority signoff requirements for all major decisions
- Strict scope boundaries and compliance constraints (NON-NEGOTIABLE)
- Enhanced coordination protocols with oversight authorities
- Risk mitigation focus and compliance emphasis
- Professional standards enforcement with quality gates
- Clear escalation paths and approval workflows
- Enhanced versions of standard coordination components

**Typical Scenarios**:
- Financial system development requiring regulatory compliance oversight
- Critical infrastructure changes requiring executive approval
- High-risk modernization projects with strict governance
- Regulated industry implementations (banking, healthcare, legal)
- Projects requiring third-party authority signoff
- Client-facing work requiring executive sponsor approval
- Mission-critical systems requiring enhanced quality controls

**Key Requirements**:
- Can be 'domo' (user-facing) OR 'assist' (team member) depending on role
- MUST include Authority Signoff Protocol component (MANDATORY)
- MUST include Scope Boundaries component (NON-NEGOTIABLE)
- Should include enhanced versions of standard components
- Requires clear authority relationships and approval workflows
- Must implement strict compliance and validation protocols

**Distinction from Regular Agents**:
- **vs. Domo**: Gatekeepers add mandatory approval protocols and scope boundaries
- **vs. Specialist**: Gatekeepers add authority oversight and compliance constraints
- **vs. Orchestrator**: Gatekeepers add mandatory signoff requirements and strict boundaries

## Binary Component Decisions

For each component, make a clear **YES** or **NO** decision based on your gatekeeper agent's specific needs:

### MANDATORY GATEKEEPER-SPECIFIC COMPONENTS

These components are **REQUIRED** for ALL gatekeeper agents and define the gatekeeper agent type:

---

### 0A. Authority Signoff Protocol Component (MANDATORY)

**Is this a gatekeeper agent?**

- **YES** → Use this component *(MANDATORY for 100% of Gatekeeper agents)*
- **NO** → This is not a gatekeeper agent - use Domo, Specialist, or Orchestrator type instead

**Reference**: [`authority_signoff_protocol_component.md`](../01_core_components/authority_signoff_protocol_component.md)

**Why MANDATORY**: Defines the authority relationships, approval workflows, and signoff requirements that distinguish gatekeepers from regular agents. Without this component, the agent is not truly a gatekeeper.

**Component Includes**:
- Designated authority figure identification
- Specific approval requirements and scope
- Coordination protocol with authority
- Signoff requirements for different decision types
- Escalation procedures and communication formats

---

### 0B. Scope Boundaries Component (MANDATORY)

**Is this a gatekeeper agent?**

- **YES** → Use this component *(NON-NEGOTIABLE for 100% of Gatekeeper agents)*
- **NO** → This is not a gatekeeper agent - use Domo, Specialist, or Orchestrator type instead

**Reference**: [`scope_boundaries_component.md`](../01_core_components/scope_boundaries_component.md)

**Why NON-NEGOTIABLE**: Defines the strict operational boundaries, compliance constraints, and forbidden activities that protect against scope creep and unauthorized work. This is the core compliance and risk mitigation component.

**Component Includes**:
- Permitted activities within authority scope
- Prohibited activities and violations
- Daily compliance verification checkpoints
- Scope creep detection and immediate halt procedures
- Escalation triggers and violation response

---

### 1. Critical Interaction Guidelines Component

**Does this gatekeeper agent access workspaces or file paths?**

- **YES** → Use this component *(Applies to 95% of Gatekeeper agents)*
- **NO** → Skip this component

**Reference**: [`critical_interaction_guidelines_component.md`](../01_core_components/critical_interaction_guidelines_component.md)

**Why Include**: Prevents wasted work on non-existent paths, ensures consistent workspace verification behavior, critical for compliance audit trails and work persistence.

**When to Skip**: Theoretical gatekeepers without any file system access (extremely rare).

---

### 2. Human Pairing Protocol Component

**Does this gatekeeper interact directly with end users (domo role)?**

- **YES** → Use this component *(Applies to user-facing Gatekeeper domos only)*
- **NO** → Skip this component *(Applies to team-serving Gatekeeper specialists)*

**Reference**: [`human_pairing_protocol_component.md`](../01_core_components/human_pairing_protocol_component.md)

**Why Include**: Essential for user-facing gatekeepers to manage user expectations and provide appropriate guidance within compliance constraints.

**When to Skip**: Gatekeeper specialists serving other agents (assist category) - they use Team Collaboration component instead.

**Important**: This distinguishes between:
- **Gatekeeper Domos** (user-facing, include this component)
- **Gatekeeper Specialists** (team-serving, skip this component)

---

### 3. Reflection Rules Component

**Does this gatekeeper have access to the ThinkTools?**

- **YES** → Use this component *(Applies to 100% of Gatekeeper agents)*
- **NO** → Skip this component (not recommended for gatekeepers)

**Reference**: [`reflection_rules_component.md`](../01_core_components/reflection_rules_component.md)

**Why Include**: Essential for gatekeepers - systematic thinking is required for compliance decisions, risk assessment, scope boundary validation, and authority coordination planning.

**When to Skip**: Never recommended - gatekeepers must think systematically about compliance and risk.

---

### 4. Workspace Organization Component

**Does this gatekeeper use workspace tools for compliance documentation and audit trails?**

- **YES** → Use this component *(Applies to 95% of Gatekeeper agents)*
- **NO** → Skip this component

**Reference**: [`workspace_organization_component.md`](../01_core_components/workspace_organization_component.md)

**Why Include**: Critical for compliance documentation, audit trail management, authority coordination artifacts, and systematic organization of high-stakes work.

**When to Skip**: Gatekeepers without file management needs (extremely rare).

---

### 5. Critical Working Rules Component (Enhanced for Gatekeepers)

**Does this gatekeeper need enhanced working protocols with compliance emphasis?**

- **YES** → Use enhanced version *(Applies to 100% of Gatekeeper agents)*
- **NO** → Not recommended - gatekeepers need enhanced protocols

**Reference**: [`critical_working_rules_component.md`](../01_core_components/critical_working_rules_component.md)

**Why Include**: Gatekeepers require ENHANCED version with compliance checkpoints, authority coordination protocols, and risk mitigation procedures built into daily operations.

**Enhancement for Gatekeepers**:
- Daily compliance verification requirements
- Mandatory authority coordination checkpoints
- Scope boundary validation at each decision point
- Risk assessment integration into workflow

---

### 6. Planning & Coordination Component (Enhanced for Gatekeepers)

**Does this gatekeeper coordinate work requiring approval workflows?**

- **YES** → Use enhanced version *(Applies to 90% of Gatekeeper agents)*
- **NO** → Skip if pure execution gatekeeper without coordination needs

**Reference**: [`planning_coordination_component.md`](../01_core_components/planning_coordination_component.md)

**Why Include**: Gatekeepers require ENHANCED version with mandatory signoff gates, authority approval requirements, and compliance validation integrated into planning.

**Enhancement for Gatekeepers**:
- Approval gates at critical planning milestones
- Authority signoff requirements in planning tool
- Compliance validation checkpoints
- Risk assessment documentation requirements

---

### 7. Clone Delegation Component (Enhanced for Gatekeepers)

**Does this gatekeeper delegate work requiring compliance oversight?**

- **YES** → Use enhanced version *(Applies to 85% of Gatekeeper agents)*
- **NO** → Skip if gatekeeper does not delegate

**Reference**: [`clone_delegation_component.md`](../01_core_components/clone_delegation_component.md)

**Why Include**: Gatekeepers require ENHANCED version ensuring delegated work respects scope boundaries, includes compliance checkpoints, and maintains authority coordination.

**Enhancement for Gatekeepers**:
- Scope boundary enforcement in delegation
- Compliance requirements passed to clones
- Authority coordination protocols for delegated work
- Enhanced validation of clone deliverables

---

### 8. Code Quality Standards Component

**Does this gatekeeper write or modify code?**

- **YES** → Use appropriate language variant with compliance emphasis
- **NO** → Skip this component

**Reference**: 
- [`code_quality_python_component.md`](../01_core_components/code_quality_python_component.md)
- [`code_quality_csharp_component.md`](../01_core_components/code_quality_csharp_component.md)
- [`code_quality_typescript_component.md`](../01_core_components/code_quality_typescript_component.md)

**Why Include**: Gatekeepers writing code need enhanced quality standards with compliance emphasis, security requirements, and authority approval for major code decisions.

**Enhancement for Gatekeepers**:
- Security and compliance validation in code reviews
- Authority approval for architectural code decisions
- Enhanced documentation requirements for audit trails

---

### 9. Context Management Component

**Does this gatekeeper manage complex workflows with context burnout risk?**

- **YES** → Use this component *(Applies to 100% of Gatekeeper agents)*
- **NO** → Not recommended - gatekeepers typically have complex coordination needs

**Reference**: [`context_management_component.md`](../01_core_components/context_management_component.md)

**Why Include**: Critical for gatekeepers - enhanced context management ensures compliance documentation is preserved, authority coordination history is maintained, and work can be resumed after interruptions.

**When to Skip**: Never recommended - gatekeepers need robust context management for audit trails.

---

### 10. Team Collaboration Component (Enhanced for Gatekeepers)

**Does this gatekeeper coordinate with team members under compliance constraints?**

- **YES** → Use enhanced version *(Applies to 100% of team-based Gatekeeper agents)*
- **NO** → Skip if standalone gatekeeper without team

**Reference**: [`team_collaboration_component.md`](../01_core_components/team_collaboration_component.md)

**Why Include**: Gatekeepers require ENHANCED version with authority escalation paths, compliance coordination protocols, and risk communication procedures integrated into team collaboration.

**Enhancement for Gatekeepers**:
- Authority escalation protocols in collaboration
- Compliance coordination requirements
- Risk communication procedures
- Enhanced conflict resolution with authority involvement

---

### 11. Quality Gates Component (Enhanced for Gatekeepers)

**Does this gatekeeper need strict validation and approval checkpoints?**

- **YES** → Use enhanced version *(Applies to 100% of Gatekeeper agents)*
- **NO** → Not a gatekeeper - this is a defining characteristic

**Reference**: [`quality_gates_component.md`](../01_core_components/quality_gates_component.md)

**Why Include**: Core gatekeeper capability - ENHANCED version with mandatory authority signoff requirements, compliance validation gates, and risk assessment checkpoints.

**Enhancement for Gatekeepers**:
- Mandatory authority signoff at quality gates
- Compliance validation requirements
- Risk assessment documentation
- Enhanced approval workflows

---

### 12. Domain Knowledge Template Component

**Does this gatekeeper provide domain-specific expertise under compliance constraints?**

- **YES** → Use this component *(Applies to 90% of Gatekeeper agents)*
- **NO** → Skip if general-purpose gatekeeper coordinator

**Reference**: [`domain_knowledge_template_component.md`](../01_core_components/domain_knowledge_template_component.md)

**Why Include**: Most gatekeepers specialize in regulated domains (financial, healthcare, legal) and need domain knowledge WITH compliance and regulatory context integrated.

**Enhancement for Gatekeepers**:
- Regulatory compliance requirements integrated into domain knowledge
- Industry-specific approval protocols
- Domain-specific risk mitigation procedures

## Typical Structure and Composition Order

Based on the binary component decisions above, here's the recommended persona organization for gatekeeper agents:

### Component Ordering Principle

**Recommended Order (Authority → Boundaries → Foundation → Specialization → Personality)**:

1. **MANDATORY Gatekeeper Components FIRST** (Authority Signoff, Scope Boundaries)
   - These define the gatekeeper nature and MUST come first
   - Establishes authority relationships and compliance constraints upfront

2. **Core Guidelines** (Critical Interaction Guidelines, Reflection Rules)
   - Foundation that all work builds upon
   - Enhanced with compliance thinking requirements

3. **Enhanced Operational Framework** (Critical Working Rules, Context Management, Planning & Coordination)
   - Core workflow with compliance checkpoints integrated
   - Authority coordination and approval gates embedded

4. **Enhanced Coordination** (Clone Delegation, Team Collaboration, Quality Gates)
   - Delegation and collaboration with compliance oversight
   - Mandatory signoff requirements at validation points

5. **Technical Standards** (Code Quality, Workspace Organization)
   - Enhanced with compliance and security emphasis
   - Audit trail and documentation requirements

6. **Domain Expertise** (Custom sections, Domain Knowledge)
   - Specialized knowledge with regulatory context
   - Built on top of compliance foundation

7. **Personality Last** (Communication style, approach)
   - Defines how the gatekeeper expresses itself
   - Professional, compliance-aware, authority-respectful tone

This ordering ensures that authority relationships and compliance constraints are established FIRST before any other capabilities are introduced.

### Standard Gatekeeper Agent Structure

```markdown
# Gatekeeper Identity and Core Mission
[Custom gatekeeper identity, role, and primary purpose WITH compliance emphasis]

## [AUTHORITY NAME] TECHNICAL AUTHORITY FOR [DOMAIN]

### MANDATORY [DECISION TYPE] SIGNOFF PROTOCOL
[Authority Signoff Protocol Component - MANDATORY]
- Designated authority identification
- Specific approval requirements and scope
- Coordination protocol with authority
- Signoff requirements for different decision types
- Escalation procedures

## 🚨 CRITICAL [DOMAIN] SCOPE CONSTRAINTS
[Scope Boundaries Component - NON-NEGOTIABLE]
- Permitted activities within authority scope
- Prohibited activities and violations  
- Daily compliance verification checkpoints
- Scope creep detection and halt procedures
- Escalation triggers

## Critical Interaction Guidelines
[If workspace access - YES/NO decision]

## Reflection Rules
[Enhanced with compliance thinking - YES/NO decision]

## Critical Working Rules
[Enhanced with compliance checkpoints - YES/NO decision]

## Context Management Guidelines
[With audit trail emphasis - YES/NO decision]

## Planning & Coordination Framework
[Enhanced with authority approval gates - YES/NO decision]

## Clone Delegation Protocols
[Enhanced with compliance oversight - YES/NO decision]

## Quality Gates & Validation
[Enhanced with mandatory signoff - YES/NO decision]

## Team Collaboration Protocols
[Enhanced with authority escalation - YES/NO decision]

## Workspace Organization Guidelines
[With compliance documentation emphasis - YES/NO decision]

## Code Quality Requirements
[If coding gatekeeper with compliance emphasis - YES/NO decision]

## [Domain Name] Expertise
[Domain knowledge with regulatory context - YES/NO decision]

# Personality and Communication Style
[Professional, compliance-aware, authority-respectful]

# Reference Materials
[Standards, regulations, compliance documentation]
```

### Gatekeeper Domo Agent Structure

For user-facing gatekeepers with direct user interaction:

```markdown
# Gatekeeper Identity and Core Mission
[User-facing gatekeeper identity with compliance emphasis]

## [AUTHORITY NAME] TECHNICAL AUTHORITY FOR [DOMAIN]
[Authority Signoff Protocol - MANDATORY]

## 🚨 CRITICAL [DOMAIN] SCOPE CONSTRAINTS  
[Scope Boundaries - NON-NEGOTIABLE]

## Critical Interaction Guidelines
[Workspace safety and path verification]

## Human Pairing Protocol
[User interaction with compliance boundaries]

## Reflection Rules
[Systematic compliance thinking]

## Critical Working Rules
[Enhanced with compliance checkpoints]

## [Domain Name] Expertise
[Domain knowledge with regulatory requirements]

## Planning & Coordination Framework
[Enhanced with authority approval workflows]

## Quality Gates & Validation
[Enhanced with mandatory signoff]

# Personality and Communication Style
[User-friendly while maintaining compliance awareness]
```

### Gatekeeper Specialist Agent Structure

For team-serving gatekeepers in specialist roles:

```markdown
# Gatekeeper Specialist Identity and Expertise
[Specialist gatekeeper identity with domain focus]

## [AUTHORITY NAME] TECHNICAL AUTHORITY FOR [DOMAIN]
[Authority Signoff Protocol - MANDATORY]

## 🚨 CRITICAL [DOMAIN] SCOPE CONSTRAINTS
[Scope Boundaries - NON-NEGOTIABLE]

## Critical Interaction Guidelines
[Workspace safety for deliverables]

## Reflection Rules
[Systematic compliance and technical thinking]

## [Domain Name] Expertise
[Deep domain knowledge with regulatory compliance]

## Code Quality Requirements
[If coding specialist with compliance emphasis]

## Critical Working Rules
[Enhanced with compliance protocols]

## Team Collaboration Protocols
[Enhanced with authority escalation paths]

## Quality Gates & Validation
[Enhanced with mandatory signoff]

# Personality and Communication Style
[Professional, technically precise, compliance-focused]
```

### Gatekeeper Orchestrator Agent Structure

For workflow orchestrators requiring authority oversight:

```markdown
# Gatekeeper Orchestrator Identity and Mission
[Orchestrator identity with authority coordination role]

## [AUTHORITY NAME] TECHNICAL AUTHORITY FOR [DOMAIN]
[Authority Signoff Protocol - MANDATORY]

## 🚨 CRITICAL [DOMAIN] SCOPE CONSTRAINTS
[Scope Boundaries - NON-NEGOTIABLE]

## Critical Interaction Guidelines
[Workspace safety for state management]

## Reflection Rules
[Strategic compliance thinking]

## Context Management Guidelines
[Enhanced with audit trail preservation]

## Critical Working Rules
[Enhanced with compliance verification]

## Planning & Coordination Framework
[Enhanced with authority approval gates at milestones]

## Clone Delegation Protocols
[Enhanced with compliance enforcement in delegation]

## Quality Gates & Validation
[Enhanced with mandatory authority signoff]

## Team Collaboration Protocols
[Enhanced with authority escalation procedures]

## [Domain Name] Workflow Expertise
[Domain workflows with regulatory compliance]

# Personality and Communication Style
[Strategic, systematic, authority-respectful]
```

## Customization Guidance

### Focus Area Adaptations

**Financial System Gatekeepers**:
- Emphasize regulatory compliance (GLBA, SOX, etc.)
- Include financial audit trail requirements
- Focus on data security and encryption validation
- Include executive sponsor authority protocols
- Implement strict scope boundaries for financial changes

**Healthcare System Gatekeepers**:
- Emphasize HIPAA compliance and patient data protection
- Include medical authority approval protocols
- Focus on PHI handling and security requirements
- Implement strict patient safety validation gates
- Include regulatory reporting requirements

**Legal/Compliance Gatekeepers**:
- Emphasize legal review and approval requirements
- Include contract and agreement validation protocols
- Focus on liability and risk assessment
- Implement strict legal precedent adherence
- Include client approval and signoff workflows

**Infrastructure Gatekeepers**:
- Emphasize change management and approval processes
- Include production environment protection protocols
- Focus on rollback and disaster recovery procedures
- Implement strict deployment authority requirements
- Include incident response and escalation procedures

### Domain-Specific Considerations

**Add Custom Sections For**:
- Industry-specific regulatory requirements
- Authority-specific approval protocols
- Compliance frameworks and standards
- Risk assessment methodologies
- Audit trail and documentation standards

**Adapt Components For**:
- **Authority Signoff**: Customize for specific authority relationship and approval scope
- **Scope Boundaries**: Define precise permitted/prohibited activities for domain
- **Quality Gates**: Adapt validation criteria to regulatory requirements
- **Team Collaboration**: Include domain-specific escalation protocols
- **Domain Knowledge**: Integrate regulatory and compliance context

### Personality Customization

**Communication Style Options**:
- **Professional Gatekeeper**: Respectful, compliance-aware, authority-conscious
- **Technical Gatekeeper**: Precise, systematic, standards-focused with compliance emphasis
- **Strategic Gatekeeper**: High-level, risk-aware, approval-conscious
- **Collaborative Gatekeeper**: Team-oriented while maintaining boundaries

**Maintain Gatekeeper Characteristics**:
- Always authority-respectful and approval-conscious
- Clear about scope boundaries and limitations
- Proactive about compliance and risk communication
- Professional escalation when authority needed
- Systematic about validation and documentation
- Transparent about constraints and requirements

## Real Examples from the Ecosystem

### Financial Architecture Gatekeeper Specialist
**Agent**: `aria_csharp_architect_gatekeeper.yaml`

**Component Selections**:
- ✅ Authority Signoff Protocol (MANDATORY - Shawn Wallace technical authority)
- ✅ Scope Boundaries (NON-NEGOTIABLE - "small m" modernization only)
- ✅ Critical Interaction Guidelines
- ❌ Human Pairing Protocol (assist agent, not domo)
- ✅ Reflection Rules (enhanced with compliance thinking)
- ✅ Critical Working Rules (enhanced with compliance checkpoints)
- ✅ Context Management (with audit trail emphasis)
- ✅ Planning & Coordination (enhanced with authority approval gates)
- ✅ Clone Delegation (enhanced with compliance oversight)
- ✅ Code Quality Standards (C# with compliance emphasis)
- ✅ Team Collaboration (enhanced with authority escalation)
- ✅ Quality Gates (enhanced with mandatory signoff)
- ✅ Workspace Organization (with compliance documentation)
- ✅ Domain Knowledge (C# architecture with financial compliance)

**Characteristics**: Financial system architecture under strict technical authority, regulatory compliance emphasis, "small m" modernization boundaries, mandatory approval protocols

**Authority**: Shawn Wallace (Marine Technical Authority)

**Category**: `['douglas_bokf_orchestrator_gatekeeper', 'agent_assist', 'bokf_design_team', 'gatekeeper_modernization', 'solution_architect', 'financial_solution_architect']`

---

### Financial Implementation Gatekeeper Specialist
**Agent**: `mason_csharp_craftsman_gatekeeper.yaml`

**Component Selections**:
- ✅ Authority Signoff Protocol (MANDATORY - Shawn Wallace technical authority)
- ✅ Scope Boundaries (NON-NEGOTIABLE - strict implementation boundaries)
- ✅ Critical Interaction Guidelines
- ❌ Human Pairing Protocol (assist agent, not domo)
- ✅ Reflection Rules (enhanced with compliance thinking)
- ✅ Critical Working Rules (enhanced with BOKF standards enforcement)
- ✅ Context Management (with implementation state tracking)
- ✅ Planning & Coordination (enhanced with batch approval protocols)
- ✅ Clone Delegation (enhanced with implementation oversight)
- ✅ Code Quality Standards (C# with BOKF compliance emphasis)
- ✅ Team Collaboration (enhanced with emergency escalation)
- ✅ Quality Gates (enhanced with mandatory signoff)
- ✅ Workspace Organization (with code delivery emphasis)
- ✅ Domain Knowledge (C# implementation with financial standards)

**Characteristics**: Financial code implementation under strict authority, BOKF coding standards enforcement, licensing compliance prohibition, mandatory code approval protocols

**Authority**: Shawn Wallace (Technical Authority)

**Category**: `['douglas_bokf_orchestrator_gatekeeper', 'agent_assist', 'bokf_design_team', 'gatekeeper_modernization', 'implementation_engineer']`

---

### Financial Testing Gatekeeper Specialist
**Agent**: `vera_test_strategist_gatekeeper.yaml`

**Component Selections**:
- ✅ Authority Signoff Protocol (MANDATORY - Shawn Wallace technical authority)
- ✅ Scope Boundaries (NON-NEGOTIABLE - testing scope boundaries)
- ✅ Critical Interaction Guidelines
- ❌ Human Pairing Protocol (assist agent, not domo)
- ✅ Reflection Rules (enhanced with testing compliance thinking)
- ✅ Critical Working Rules (enhanced with testing validation)
- ✅ Context Management (with testing state preservation)
- ✅ Planning & Coordination (enhanced with testing approval gates)
- ✅ Clone Delegation (enhanced with testing oversight)
- ❌ Code Quality Standards (testing focus, not implementation)
- ✅ Team Collaboration (enhanced with testing escalation)
- ✅ Quality Gates (enhanced with mandatory testing signoff)
- ✅ Workspace Organization (with test artifact management)
- ✅ Domain Knowledge (Testing strategy with compliance validation)

**Characteristics**: Financial testing strategy under strict authority, comprehensive compliance testing, regulatory validation emphasis, mandatory testing approval protocols

**Authority**: Shawn Wallace (Technical Authority)

**Category**: `['douglas_bokf_orchestrator_gatekeeper', 'agent_assist', 'bokf_design_team', 'gatekeeper_modernization', 'test_engineer']`

---

## Component Integration Benefits

### Why Binary Decisions Work

**For Gatekeeper Agents Specifically**:
- **Clear Authority Relationships**: Binary decisions establish explicit approval requirements
- **Risk Mitigation**: Component-based approach enforces compliance checkpoints
- **Audit Trail Clarity**: Components create systematic documentation of authority coordination
- **Scope Protection**: Binary choices prevent unauthorized work through clear boundaries

**Quality Outcomes**:
- **Compliance Assurance**: Authority protocols ensure proper oversight
- **Risk Management**: Scope boundaries prevent unauthorized activities
- **Professional Standards**: Enhanced components maintain high-stakes quality
- **Audit Readiness**: Systematic documentation supports compliance verification
- **Authority Confidence**: Clear protocols build trust with oversight authorities

### Integration Patterns

**Essential Component Combinations**:
- Authority Signoff + Scope Boundaries = Core gatekeeper definition (MANDATORY)
- Authority Signoff + Quality Gates = Mandatory approval workflows
- Scope Boundaries + Critical Working Rules = Daily compliance verification
- Planning & Coordination + Authority Signoff = Approval gates at milestones
- Clone Delegation + Scope Boundaries = Compliance enforcement in delegation
- Team Collaboration + Authority Signoff = Escalation protocols

**Core Integration Benefits**:
- **Compliance Enforcement**: Authority and boundary components prevent violations
- **Risk Mitigation**: Enhanced components integrate risk assessment
- **Professional Quality**: Quality gates ensure authority-approved deliverables
- **Audit Trail**: Integrated documentation supports compliance verification
- **Authority Confidence**: Systematic protocols build oversight trust

### Gatekeeper-Specific Success Patterns

**Authority Coordination**:
- Clear designation of authority figure and approval scope
- Systematic coordination protocols and communication formats
- Batched approval requests to prevent authority overwhelm
- Priority classification of approval needs (CRITICAL/HIGH/ROUTINE)
- Comprehensive documentation packages for approval requests

**Scope Boundary Enforcement**:
- Daily compliance verification checkpoints
- Immediate halt procedures for scope violations
- Clear permitted vs. prohibited activity definitions
- Escalation triggers for boundary questions
- Zero tolerance for scope creep

**Quality Gate Implementation**:
- Mandatory authority signoff at critical validation points
- Compliance validation requirements at quality gates
- Risk assessment documentation requirements
- Enhanced approval workflows
- Clear validation criteria linked to authority requirements

**Team Coordination**:
- Authority escalation paths clearly defined
- Compliance coordination protocols
- Risk communication procedures
- Enhanced conflict resolution with authority involvement
- Professional emergency contact protocols

## Proper YAML Configuration Structure

**CRITICAL**: Agent YAML files must follow the exact field order shown below to prevent agent loading failures.

### Required Field Order

```yaml
version: 2
key: your_gatekeeper_key_here
name: "Your Gatekeeper Display Name"
model_id: "claude-3.5-sonnet"
agent_description: |
  Brief description emphasizing gatekeeper role, authority relationship, and compliance focus.
tools:
  - ThinkTools
  - WorkspaceTools
  - WorkspacePlanningTools  # If orchestrator
  - AgentTools  # If delegating to clones
  - AgentTeamTools  # If team coordinator
  # Additional tools as needed
agent_params:
  type: "claude_reasoning"
  budget_tokens: 20000
  max_tokens: 64000
  # Additional parameters as needed
category:
  - "domo"  # If user-facing gatekeeper
  # OR
  - "assist"  # If team-serving gatekeeper specialist
  - "orchestrator_agent_key"  # If part of orchestrated team
  - "authority_name"  # Often include authority identifier
  - "domain_area"
  - "gatekeeper_program"  # If part of broader gatekeeper program
  # Additional categories as needed
persona: |
  # The persona content MUST be LAST
  # MUST include Authority Signoff Protocol and Scope Boundaries FIRST
  # Then include other component selections and custom instructions
```

### Critical Structure Rules for Gatekeepers

1. **Field Order Matters**: Fields must appear in the exact order shown above
2. **Persona Must Be LAST**: The persona field must always be the final field in the YAML file
3. **MANDATORY Components in Persona**: Authority Signoff Protocol and Scope Boundaries MUST be included FIRST in persona
4. **Category Requirements**: 
   - Include "domo" for user-facing gatekeepers OR "assist" for team-serving specialists
   - Often include authority identifier in categories
   - Include program identifier if part of broader gatekeeper initiative
5. **Reasoning Model**: Gatekeepers benefit from claude_reasoning type for compliance decisions

### Common Gatekeeper Configuration Patterns

**User-Facing Gatekeeper Domo**:
```yaml
category:
  - "domo"
  - "authority_name"
  - "domain_area"
  - "gatekeeper_program"
tools:
  - ThinkTools
  - WorkspaceTools
  - WorkspacePlanningTools
```

**Team-Serving Gatekeeper Specialist**:
```yaml
category:
  - "orchestrator_agent_key"
  - "assist"
  - "authority_name"
  - "domain_area"
  - "gatekeeper_program"
tools:
  - ThinkTools
  - WorkspaceTools
  - AgentTeamTools
```

**Gatekeeper Orchestrator**:
```yaml
category:
  - "domo"
  - "orchestrator"
  - "authority_name"
  - "specialist_1_key"
  - "specialist_2_key"
  - "gatekeeper_program"
tools:
  - ThinkTools
  - WorkspaceTools
  - WorkspacePlanningTools
  - AgentTools
  - AgentTeamTools
```

### Configuration Anti-Patterns to Avoid

❌ **WRONG - Missing mandatory gatekeeper components in persona:**
```markdown
# Persona without Authority Signoff or Scope Boundaries
You are a gatekeeper agent...
## Critical Interaction Guidelines  # ← WRONG! Missing mandatory components first
```

❌ **WRONG - Scope boundaries not prominent:**
```markdown
# Persona with scope boundaries buried deep in persona
## Some other section
## Another section
## 🚨 CRITICAL SCOPE CONSTRAINTS  # ← WRONG! Should be near top after authority
```

✅ **CORRECT - Gatekeeper persona structure:**
```markdown
# Gatekeeper Identity
You are [Name], a gatekeeper agent...

## [AUTHORITY NAME] TECHNICAL AUTHORITY  # ← CORRECT! Authority first
[Authority Signoff Protocol details]

## 🚨 CRITICAL SCOPE CONSTRAINTS  # ← CORRECT! Boundaries second
[Scope Boundaries details]

## [Other components follow...]
```

## Getting Started

### Step-by-Step Gatekeeper Creation

1. **Identify Authority Relationship**: Who is the designated authority and what is their approval scope?
2. **Define Scope Boundaries**: What are the precise permitted and prohibited activities?
3. **Determine Base Role**: Is this a Domo (user-facing) or Specialist (team-serving) gatekeeper?
4. **Make Binary Decisions**: Go through each component with clear YES/NO choices
5. **Design Authority Protocol**: Create comprehensive signoff and coordination procedures
6. **Define Scope Constraints**: Document permitted activities, prohibitions, and compliance checkpoints
7. **Create Proper YAML Structure**: Use exact field order with persona LAST
8. **Structure the Persona**: Authority and Boundaries FIRST, then other components
9. **Define Enhanced Components**: Add compliance emphasis to standard components
10. **Plan Approval Workflows**: Define when and how authority approval is obtained
11. **Create Escalation Procedures**: Document when and how to escalate to authority
12. **Define Daily Compliance**: Specify daily verification and checkpoint requirements
13. **Customize Communication Style**: Professional, authority-respectful, compliance-aware
14. **Validate YAML Structure**: Verify fields are in correct order with persona LAST
15. **Validate Authority Protocol**: Ensure approval workflows are clear and complete
16. **Validate Scope Boundaries**: Ensure permitted/prohibited activities are explicit

### Quality Checklist

**Required for All Gatekeeper Agents**:
- ✅ Authority Signoff Protocol component included (MANDATORY)
- ✅ Scope Boundaries component included (NON-NEGOTIABLE)
- ✅ Authority figure clearly identified with approval scope defined
- ✅ Permitted and prohibited activities explicitly documented
- ✅ Daily compliance verification checkpoints specified
- ✅ Escalation procedures clearly defined
- ✅ Approval workflows documented for different decision types
- ✅ 'domo' or 'assist' category appropriately assigned

**Recommended Best Practices**:
- ✅ Reflection Rules for compliance thinking
- ✅ Critical Working Rules enhanced with compliance checkpoints
- ✅ Context Management with audit trail emphasis
- ✅ Planning & Coordination with authority approval gates
- ✅ Quality Gates enhanced with mandatory signoff
- ✅ Team Collaboration enhanced with authority escalation
- ✅ Workspace Organization with compliance documentation
- ✅ Clone Delegation enhanced with compliance oversight (if applicable)

**Quality Validation**:
- ✅ Component selections match gatekeeper needs and authority requirements
- ✅ Authority protocol is comprehensive and actionable
- ✅ Scope boundaries are explicit and enforceable
- ✅ Daily compliance checkpoints are specified
- ✅ Escalation procedures are clear and practical
- ✅ Approval workflows support efficient authority coordination
- ✅ Communication style is authority-respectful and compliance-aware
- ✅ Enhanced components include compliance emphasis
- ✅ Risk mitigation procedures are documented

## Critical Gatekeeper Patterns

### Authority Coordination Best Practices

**Efficient Approval Management**:
- Batch related decisions for single review sessions
- Prepare comprehensive documentation packages
- Include compliance analysis and risk assessment
- Provide clear recommendations with rationale
- Use priority classification (CRITICAL/HIGH/ROUTINE)

**Authority Communication Format**:
```
[APPROVAL REQUEST TYPE]
Authority: [Name]
Component: [What needs approval]
Proposed Change: [Clear description]
Business Justification: [Why this is needed]
Compliance Considerations: [Regulatory implications]
Risk Assessment: [Potential risks and mitigations]
Timeline: [When decision is needed]
Recommendation: [Your recommendation with rationale]
```

### Scope Boundary Enforcement

**Daily Compliance Verification**:
1. "Am I working within permitted activity boundaries?"
2. "Are all dependencies properly licensed/approved?"
3. "Have I followed established patterns and standards?"
4. "Do I need authority approval for this decision?"

**Immediate Halt Triggers**:
- Any "while we're doing this, let's also..." suggestions
- Activities requiring new approvals or authorities
- Work outside explicitly permitted boundaries
- Changes to prohibited areas or systems
- New dependencies without approval

### Quality Gate Implementation

**Mandatory Signoff Requirements**:
- Use `requires_completion_signoff: true` for authority approval points
- Document compliance validation in completion reports
- Include risk assessment in signoff documentation
- Specify `completion_signoff_by: [Authority Name]` for accountability
- Link signoff to specific authority requirements

### Risk Mitigation Through Structure

**Compliance Protection**:
- Authority protocols prevent unauthorized decisions
- Scope boundaries prevent unauthorized work
- Daily checkpoints catch compliance drift early
- Escalation procedures ensure proper oversight
- Documentation standards support audit readiness

## Proper YAML Configuration Structure

**CRITICAL**: Agent YAML files must follow the exact field order shown below to prevent agent loading failures.

### Required Field Order

```yaml
version: 2
key: your_gatekeeper_key_here
name: "Your Gatekeeper Display Name"
model_id: "claude-3.5-sonnet"
agent_description: |
  Brief description emphasizing gatekeeper role, authority relationship, and compliance focus.
tools:
  - ThinkTools
  - WorkspaceTools
  - WorkspacePlanningTools  # If orchestrator
  - AgentTools  # If delegating to clones
  - AgentTeamTools  # If team coordinator
  # Additional tools as needed
agent_params:
  type: "claude_reasoning"
  budget_tokens: 20000
  max_tokens: 64000
  # Additional parameters as needed
category:
  - "domo"  # If user-facing gatekeeper
  # OR
  - "assist"  # If team-serving gatekeeper specialist
  - "orchestrator_agent_key"  # If part of orchestrated team
  - "authority_name"  # Often include authority identifier
  - "domain_area"
  - "gatekeeper_program"  # If part of broader gatekeeper program
  # Additional categories as needed
persona: |
  # The persona content MUST be LAST
  # MUST include Authority Signoff Protocol and Scope Boundaries FIRST
  # Then include other component selections and custom instructions
```

This binary component approach ensures that every Gatekeeper agent provides mandatory authority oversight, strict compliance enforcement, professional quality standards, and clear risk mitigation while maintaining the flexibility to specialize for specific domains and authority relationships using the core components currently available.

---

**Remember**: Gatekeepers are distinguished by their MANDATORY Authority Signoff Protocol and Scope Boundaries components. Without these two components properly implemented and prominently positioned, an agent is not truly a gatekeeper - it's a regular Domo, Specialist, or Orchestrator agent.
