# Project Manager Agent Guide

A comprehensive guide for building agents that track projects, provide visibility, and answer project questions without executing work or coordinating teams.

## When to Use Project Manager Agent Type

**Primary Purpose**: Agents designed to track project status, maintain project information, and provide visibility into project progress

**Core Characteristics**:
- Project tracking and status monitoring
- Information hub for project data (not executor)
- Status reporting and progress summaries
- Project documentation maintenance
- User-facing Q&A about projects
- Milestone and deadline tracking
- Resource allocation visibility
- Cross-project reporting and analytics

**Typical Scenarios**:
- Project status queries ("What's the status of Project X?")
- Cross-project visibility ("Show all my active projects")
- Time tracking and reporting
- Milestone and deadline monitoring
- Progress reporting and summaries
- Project documentation updates
- Stakeholder communication and reporting
- Resource utilization tracking

**Key Distinction from Orchestrator Agents**:
- **Project Manager**: TRACKS and REPORTS on projects (information management)
- **Orchestrator**: EXECUTES and COORDINATES work (workflow execution)
- Project Managers are observers and reporters, Orchestrators are executors and coordinators

**Key Distinction from General Domo Agents**:
- **Project Manager**: Specialized in project management, tracking, and organizational visibility
- **General Domo**: Broad-purpose user assistance without project management specialization
- Project Managers have domain expertise in project tracking methodologies

**Key Requirements**:
- Must include 'domo' in agent category array (user-facing)
- Should include workspace tools for project file access
- Focuses on information retrieval and reporting, not work execution
- Provides project visibility without coordinating execution
- Maintains professional project management communication

## Binary Component Decisions

For each component, make a clear **YES** or **NO** decision based on your agent's specific needs:

### 1. Critical Interaction Guidelines Component

**Does this Project Manager agent access workspaces or project file paths?**

- **YES** → Use this component *(Applies to 95% of Project Manager agents)*
- **NO** → Skip this component

**Reference**: [`critical_interaction_guidelines_component.md`](../01_core_components/critical_interaction_guidelines_component.md)

**Why Include**: Prevents wasted work on non-existent project paths, ensures consistent workspace verification for project files, critical for accessing project documentation and status files.

**When to Skip**: Theoretical PM agents that only answer questions without accessing any project files (extremely rare).

---

### 2. Reflection Rules Component

**Does this Project Manager have access to the ThinkTools?**

- **YES** → Use this component *(Applies to 90% of Project Manager agents)*
- **NO** → Skip this component

**Reference**: [`reflection_rules_component.md`](../01_core_components/reflection_rules_component.md)

**Why Include**: Ensures systematic analysis of project status, improves quality of progress assessments, enables thoughtful interpretation of project data, supports comprehensive status reporting.

**When to Skip**: Simple project status lookup agents without analytical requirements.

---

### 3. Workspace Organization Component

**Does this Project Manager use workspace tools for project file management?**

- **YES** → Use this component *(Applies to 95% of Project Manager agents)*
- **NO** → Skip this component

**Reference**: [`workspace_organization_component.md`](../01_core_components/workspace_organization_component.md)

**Why Include**: Standardizes project file organization, supports consistent documentation structure, enables effective project information management, facilitates stakeholder collaboration on project artifacts.

**When to Skip**: PM agents that only query external systems without workspace file management.

---

### 4. Planning & Coordination Component

**Does this Project Manager track project plans and milestones?**

- **YES** → Use this component *(Applies to 90% of Project Manager agents)*
- **NO** → Skip this component

**Reference**: [`planning_coordination_component.md`](../01_core_components/planning_coordination_component.md)

**Why Include**: Provides systematic project tracking capability, enables milestone monitoring, supports progress visualization, facilitates status reporting. **CRITICAL DISTINCTION**: PM agents use planning tools for TRACKING, not EXECUTION.

**When to Skip**: Simple status query agents without project plan tracking needs.

**IMPORTANT**: Project Managers use planning tools to TRACK and REPORT on plans created by others or the user. They do NOT create execution plans or coordinate work. That's the Orchestrator's job.

---

### 5. Clone Delegation Component

**Does this Project Manager delegate work execution to clones?**

- **NO** → Skip this component *(Applies to 95% of Project Manager agents)*
- **YES** → Reconsider agent type - might actually be an Orchestrator

**Reference**: [`clone_delegation_component.md`](../01_core_components/clone_delegation_component.md)

**Why Skip**: Project Managers track and report, they do not execute work or coordinate tasks. Work delegation is the Orchestrator's responsibility.

**When to Include**: Never for pure Project Manager agents. If delegation is needed, the agent is actually an Orchestrator, not a Project Manager.

---

### 6. Context Management Component

**Does this Project Manager need to handle many large projects or extensive historical data?**

- **MAYBE** → Use this component based on scale requirements
- **NO** → Skip if managing limited project data

**Reference**: [`context_management_component.md`](../01_core_components/context_management_component.md)

**Why Include**: For PM agents managing many large projects with extensive histories, context management prevents information overload and ensures sustainable operations.

**When to Skip**: PM agents tracking small numbers of projects or focused on recent status only.

**Decision Criteria**: Include if the PM regularly needs to process information from 5+ large active projects or extensive project histories.

---

### 7. Quality Gates Component

**Does this Project Manager track milestones, deliverables, or completion criteria?**

- **YES** → Use this component *(Applies to 85% of Project Manager agents)*
- **NO** → Skip this component

**Reference**: [`quality_gates_component.md`](../01_core_components/quality_gates_component.md)

**Why Include**: Enables milestone tracking, supports deliverable monitoring, facilitates completion criteria reporting, provides quality gate status visibility.

**When to Skip**: Simple status query agents without milestone or deliverable tracking.

**IMPORTANT**: Project Managers track and report on quality gates and milestones. They don't enforce them or make go/no-go decisions - that's for Orchestrators or stakeholders.

---

### 8. Team Collaboration Component

**Does this Project Manager coordinate communication between team members?**

- **NO** → Skip this component *(Applies to 90% of Project Manager agents)*
- **YES** → Reconsider agent type - might actually be an Orchestrator

**Reference**: [`team_collaboration_component.md`](../01_core_components/team_collaboration_component.md)

**Why Skip**: Project Managers provide information and visibility, they don't coordinate team activities. Team coordination is the Orchestrator's responsibility.

**When to Include**: Rarely. Only if the PM agent needs to gather status updates from other agents for consolidated reporting (not coordination).

---

### 9. Code Quality Standards Component

**Does this Project Manager write or modify code?**

- **NO** → Skip this component *(Applies to 99% of Project Manager agents)*
- **YES** → Reconsider agent purpose

**Reference**: 
- [`code_quality_python_component.md`](../01_core_components/code_quality_python_component.md)
- [`code_quality_csharp_component.md`](../01_core_components/code_quality_csharp_component.md)
- [`code_quality_typescript_component.md`](../01_core_components/code_quality_typescript_component.md)

**Why Skip**: Project Managers track projects, they don't write code. Code development should be handled by appropriate development agents.

**When to Include**: Almost never for Project Manager agents.

---

### 10. Domain Knowledge Template Component

**Does this Project Manager specialize in a specific domain or methodology?**

- **YES** → Use domain-specific variant *(Applies to 70% of Project Manager agents)*
- **NO** → Skip if general-purpose project tracking

**Reference**: [`domain_knowledge_template_component.md`](../01_core_components/domain_knowledge_template_component.md)

**Why Include**: Provides specialized project management methodologies (Agile, Waterfall, PRINCE2, etc.), ensures proper terminology and metrics for specific industries, supports compliance-specific tracking requirements.

**When to Skip**: General-purpose project tracking without domain specialization.

**Common PM Domain Knowledge**:
- Agile/Scrum methodologies
- Waterfall project management
- Software development lifecycle tracking
- Construction project management
- Event planning and coordination
- Research project tracking

## Typical Structure and Composition Order

Based on the binary component decisions above, here's the recommended persona organization for Project Manager agents:

### Component Ordering Principle

**Recommended Order (Foundation → Tracking → Domain → Personality)**:

1. **Core Guidelines First** (Critical Interaction Guidelines, Reflection Rules)
   - Establishes safety and systematic analysis patterns
   - Foundation for all project information access

2. **Project Tracking Framework** (Workspace Organization, Planning & Coordination, Quality Gates)
   - Defines core project tracking capabilities
   - Sets information management standards

3. **Scale Management** (Context Management - if needed)
   - Handles large-scale project tracking
   - Ensures sustainable operations

4. **Domain Expertise** (Custom sections, Domain Knowledge)
   - Specialized project management methodologies
   - Industry-specific tracking requirements

5. **Personality Last** (Communication style, reporting approach)
   - Defines how the PM communicates project information
   - Applied across all previous sections

This ordering ensures that fundamental project tracking patterns are established before adding specialized domain capabilities and communication style.

### Standard Project Manager Agent Structure

```markdown
# Project Manager Identity and Core Purpose
[Custom PM identity, project tracking role, and primary mission]

## Critical Interaction Guidelines
[YES - workspace access for project files]

## Reflection Rules
[YES - systematic analysis of project status]

## Workspace Organization Guidelines
[YES - project file and documentation management]

## Planning & Coordination Framework
[YES - project tracking and milestone monitoring, NOT execution]

## Quality Gates & Milestone Tracking
[If tracking milestones/deliverables - YES/NO decision]

## Context Management Guidelines
[If managing many large projects - MAYBE decision]

## [Domain Name] Project Management Expertise
[Custom domain knowledge and specialized PM methodologies]

# Personality and Communication Style
[Custom personality traits, reporting approach, and stakeholder communication style]

# Reference Materials
[Links to PM methodologies, templates, or reporting standards]
```

### Minimal Project Manager Agent Structure

For simple project status query agents:

```markdown
# Project Manager Identity and Core Purpose
[Custom PM identity and tracking focus]

## Critical Interaction Guidelines
[Workspace safety for project files]

## Reflection Rules
[Systematic status analysis]

## Workspace Organization Guidelines
[Project file access and organization]

## Planning & Coordination Framework
[Basic project tracking]

# Personality and Communication Style
[Clear reporting and communication approach]
```

### Advanced Multi-Project Manager Structure

For PM agents managing many large projects:

```markdown
# Project Manager Identity and Core Purpose
[Custom PM identity, multi-project tracking role, and portfolio oversight mission]

## Critical Interaction Guidelines
[Workspace safety and project path verification]

## Reflection Rules
[Systematic analysis of project portfolios and status trends]

## Context Management Guidelines
[Proactive management of large project information volumes]

## Workspace Organization Guidelines
[Comprehensive project file and portfolio documentation management]

## Planning & Coordination Framework
[Multi-project tracking, cross-project reporting, portfolio visibility]

## Quality Gates & Milestone Tracking
[Portfolio-wide milestone monitoring and deliverable tracking]

## [Domain Name] Project Management Expertise
[Specialized PM methodologies and industry-specific practices]

# Personality and Communication Style
[Strategic, analytical, stakeholder-focused reporting approach]

# Reference Materials
[PM methodologies, reporting templates, portfolio management standards]
```

## Customization Guidance

### Focus Area Adaptations

**Single-Project Tracking Agents**:
- Focus on detailed status monitoring for one project
- Emphasize comprehensive documentation access
- Include milestone and deliverable tracking
- Use planning tools for single project visibility
- Provide detailed progress reporting

**Multi-Project Portfolio Agents**:
- Include Context Management component for scale
- Focus on cross-project reporting and analytics
- Emphasize portfolio-level visibility
- Use planning tools for portfolio tracking
- Provide executive summaries and trend analysis
- Include resource allocation visibility across projects

**Methodology-Specific PM Agents**:
- Include Domain Knowledge Template component
- Emphasize methodology-specific terminology (sprints, epics, phases)
- Focus on methodology-compliant reporting
- Include specialized metrics and KPIs
- Adapt tracking to methodology workflows (Agile, Waterfall, Hybrid)

**Stakeholder-Focused PM Agents**:
- Emphasize clear communication and reporting
- Focus on executive summaries and key metrics
- Include visualization and dashboard capabilities
- Adapt communication style to audience level
- Provide both detailed and high-level reporting options

### Domain-Specific Considerations

**Add Custom Sections For**:
- Industry-specific project terminology and phases
- Compliance and regulatory tracking requirements
- Specialized project metrics and KPIs
- Domain-specific milestone and gate definitions
- Professional communication protocols for stakeholders

**Adapt Components For**:
- **Planning & Coordination**: Customize for domain-specific project phases and milestones
- **Quality Gates**: Adapt tracking for domain-specific deliverables and completion criteria
- **Workspace Organization**: Structure for industry-standard project documentation
- **Domain Knowledge**: Include methodology-specific tracking practices (Agile ceremonies, Waterfall phases, etc.)

### Personality Customization

**Communication Style Options**:
- **Executive Reporter**: Concise, metrics-focused, strategic summaries
- **Detailed Analyst**: Comprehensive, thorough, data-driven reporting
- **Collaborative Partner**: Supportive, accessible, guidance-oriented
- **Proactive Monitor**: Alert-focused, risk-aware, proactive communication

**Maintain Project Manager Characteristics**:
- Information-focused (not execution-focused)
- Clear and transparent status reporting
- Systematic project documentation access
- Professional stakeholder communication
- Objective progress assessment
- Accessible for project questions
- Organized information presentation

## Real Examples from the Ecosystem

### Single-Project Tracking Agent
**Example Configuration**:

**Component Selections**:
- ✅ Critical Interaction Guidelines
- ✅ Reflection Rules
- ✅ Workspace Organization
- ✅ Planning & Coordination (for tracking)
- ✅ Quality Gates (milestone tracking)
- ❌ Context Management (single project, manageable scope)
- ❌ Clone Delegation (not executing work)
- ❌ Team Collaboration (not coordinating team)
- ❌ Code Quality Standards (not coding)
- ✅ Domain Knowledge (Agile methodology)

**Characteristics**: Single software project tracking, Agile sprint monitoring, user story status reporting, milestone tracking, stakeholder Q&A

**Use Cases**:
- "What's the status of the login feature?"
- "Show me all open user stories in the current sprint"
- "When is the next milestone due?"
- "Update the project documentation with today's decisions"

---

### Multi-Project Portfolio Manager
**Example Configuration**:

**Component Selections**:
- ✅ Critical Interaction Guidelines
- ✅ Reflection Rules
- ✅ Context Management (managing 10+ active projects)
- ✅ Workspace Organization
- ✅ Planning & Coordination (portfolio tracking)
- ✅ Quality Gates (cross-project milestone tracking)
- ❌ Clone Delegation (not executing work)
- ❌ Team Collaboration (not coordinating teams)
- ❌ Code Quality Standards (not coding)
- ✅ Domain Knowledge (Portfolio management, mixed methodologies)

**Characteristics**: Multi-project portfolio visibility, cross-project reporting, resource allocation tracking, executive summaries, trend analysis

**Use Cases**:
- "Show all projects with upcoming milestones this month"
- "Which projects are behind schedule?"
- "Give me an executive summary of portfolio status"
- "How is resource allocation across all active projects?"
- "Show me project status trends over the last quarter"

---

### Domain-Specific PM Agent (Construction)
**Example Configuration**:

**Component Selections**:
- ✅ Critical Interaction Guidelines
- ✅ Reflection Rules
- ✅ Workspace Organization
- ✅ Planning & Coordination (construction phase tracking)
- ✅ Quality Gates (inspection and milestone tracking)
- ❌ Context Management (focused scope)
- ❌ Clone Delegation (not executing work)
- ❌ Team Collaboration (not coordinating contractors)
- ❌ Code Quality Standards (not coding)
- ✅ Domain Knowledge (Construction project management, safety compliance)

**Characteristics**: Construction project tracking, phase completion monitoring, safety compliance tracking, permit status, subcontractor coordination visibility

**Use Cases**:
- "What's the status of the foundation phase?"
- "Are we on track with building permits?"
- "Show me all pending inspections"
- "Update the project log with today's site conditions"
- "What subcontractors are scheduled for next week?"

---

## Component Integration Benefits

### Why Binary Decisions Work

**For Project Manager Agents Specifically**:
- **Clear Role Boundaries**: Binary decisions prevent scope creep into execution territory
- **Information Focus**: Component-based approach maintains information management focus
- **Scalability**: Components support growth from single to multi-project tracking
- **Professional Reporting**: Components ensure consistent stakeholder communication

**Quality Outcomes**:
- **Clear Visibility**: Consistent project information access and reporting
- **Professional Communication**: Component standards maintain stakeholder communication quality
- **Organized Information**: Workspace organization ensures accessible project data
- **Accurate Tracking**: Planning tools enable systematic milestone and progress monitoring

### Integration Patterns

**Essential Component Combinations**:
- Critical Interaction Guidelines + Workspace Organization = Safe project file access
- Reflection Rules + Planning & Coordination = Thoughtful project status analysis
- Quality Gates + Planning & Coordination = Comprehensive milestone tracking
- Context Management + Workspace Organization = Sustainable multi-project tracking

**Core Integration Benefits**:
- **Accessible Information**: Critical Interaction Guidelines ensure reliable project file access
- **Quality Analysis**: Reflection Rules improve status assessment and reporting quality
- **Organized Tracking**: Workspace Organization maintains structured project information
- **Comprehensive Visibility**: Planning tools enable systematic progress monitoring

### Project Manager-Specific Success Patterns

**Information Management**:
- Systematic access to project documentation
- Organized storage of project status updates
- Clear documentation of milestones and deliverables
- Accessible historical project information

**Status Reporting**:
- Regular progress summaries for stakeholders
- Clear milestone and deadline visibility
- Risk and issue tracking (not resolution)
- Resource allocation transparency

**Stakeholder Communication**:
- Professional project status updates
- Accessible for project questions
- Clear and transparent reporting
- Appropriate level of detail for audience

**Project Documentation**:
- Maintain up-to-date project files
- Document decisions and changes
- Track project artifacts and deliverables
- Preserve project history

## Proper YAML Configuration Structure

**CRITICAL**: Agent YAML files must follow the exact field order shown below to prevent agent loading failures.

### Required Field Order

```yaml
version: 2
key: your_pm_agent_key_here
name: "Your Project Manager Display Name"
model_id: "claude-3.5-sonnet"
agent_description: |
  Brief description of the PM agent's tracking purpose and project scope.
tools:
  - ThinkTools
  - WorkspaceTools
  - WorkspacePlanningTools  # For project tracking
  # Additional tools as needed for reporting
agent_params:
  type: "claude_reasoning"
  budget_tokens: 15000
  max_tokens: 32000
  # Additional parameters as needed
category:
  - "domo"  # User-facing project information
  - "project_manager"
  - "your_domain"  # e.g., "agile", "construction", "software_development"
  # Additional categories as needed
persona: |
  # The persona content MUST be LAST
  # This contains all your component selections and custom instructions
```

### Critical Structure Rules for Project Managers

1. **Field Order Matters**: Fields must appear in the exact order shown above
2. **Persona Must Be LAST**: The persona field must always be the final field in the YAML file
3. **Required Tools**: PM agents typically need ThinkTools, WorkspaceTools, and WorkspacePlanningTools
4. **Domo Category Required**: All PM agents must include "domo" - they are user-facing
5. **Reasoning Model Recommended**: PM agents benefit from claude_reasoning for status analysis

### Common PM Configuration Patterns

**Single-Project Tracking PM**:
```yaml
category:
  - "domo"
  - "project_manager"
  - "methodology_name"  # e.g., "agile", "waterfall"
tools:
  - ThinkTools
  - WorkspaceTools
  - WorkspacePlanningTools
agent_params:
  type: "claude_reasoning"
  budget_tokens: 10000
  max_tokens: 16000
```

**Multi-Project Portfolio PM**:
```yaml
category:
  - "domo"
  - "project_manager"
  - "portfolio_management"
tools:
  - ThinkTools
  - WorkspaceTools
  - WorkspacePlanningTools
agent_params:
  type: "claude_reasoning"
  budget_tokens: 20000  # Higher for managing multiple projects
  max_tokens: 64000
```

**Domain-Specific PM** (e.g., Construction):
```yaml
category:
  - "domo"
  - "project_manager"
  - "construction"
  - "compliance"
tools:
  - ThinkTools
  - WorkspaceTools
  - WorkspacePlanningTools
agent_params:
  type: "claude_reasoning"
  budget_tokens: 15000
  max_tokens: 32000
```

## Getting Started

### Step-by-Step Project Manager Agent Creation

1. **Define Tracking Scope**: Clearly identify what projects or domains this PM will track
2. **Determine Scale**: Single project vs. multi-project portfolio tracking
3. **Make Binary Decisions**: Go through each component with clear YES/NO choices based on PM needs
4. **Identify Domain**: Determine if specialized methodology knowledge is needed (Agile, Waterfall, etc.)
5. **Plan Information Access**: Map project documentation locations and access patterns
6. **Create Proper YAML Structure**: Use the exact field order with persona LAST
7. **Structure the Persona**: Use the typical PM structure as template
8. **Define Tracking Approach**: Specify what project information will be tracked and reported
9. **Establish Reporting Style**: Set communication approach for different stakeholder audiences
10. **Customize Domain Expertise**: Add specialized PM methodology knowledge as needed
11. **Define Communication Style**: Set professional reporting personality and approach
12. **Validate YAML Structure**: Verify fields are in correct order with persona LAST
13. **Test Information Access**: Verify PM can access all required project files
14. **Validate Reporting**: Ensure status reports provide appropriate visibility

### Quality Checklist

**Required for All Project Manager Agents**:
- ✅ Includes 'domo' in category array (user-facing)
- ✅ WorkspaceTools equipped for project file access
- ✅ WorkspacePlanningTools equipped for project tracking
- ✅ Clear role boundaries (tracking, not executing)
- ✅ Professional stakeholder communication approach
- ✅ NO work execution or team coordination responsibilities

**Recommended Best Practices**:
- ✅ Critical Interaction Guidelines for safe project file access
- ✅ Reflection Rules for systematic status analysis
- ✅ Workspace Organization for structured project documentation
- ✅ Quality Gates component for milestone tracking
- ✅ Context Management if handling many large projects
- ✅ Domain Knowledge for methodology-specific tracking
- ✅ Clear distinction from Orchestrator agents (tracking vs. execution)

**Quality Validation**:
- ✅ Component selections match information management focus
- ✅ NO execution or coordination components included
- ✅ Planning tools used for TRACKING, not execution
- ✅ Professional reporting and communication defined
- ✅ Project file access patterns clearly specified
- ✅ Stakeholder communication approach clearly defined
- ✅ Role boundaries clearly maintained (observer, not executor)
- ✅ Domain-specific tracking properly structured if applicable

## Critical Project Manager Patterns

### Role Boundary Clarity

**✅ PROJECT MANAGER RESPONSIBILITIES**:
- Track project status and progress
- Report on milestones and deliverables
- Maintain project documentation
- Answer project status questions
- Provide visibility into project information
- Monitor (not enforce) quality gates
- Document project decisions and changes

**❌ NOT PROJECT MANAGER RESPONSIBILITIES** (These belong to Orchestrators):
- Execute project work
- Coordinate team activities
- Delegate tasks to other agents
- Make go/no-go decisions
- Resolve project issues
- Allocate resources
- Drive project execution

### Information vs. Execution Focus

**Information Management (PM Role)**:
```
"Based on the project plan, we have 3 user stories completed out of 8 
in the current sprint, with 2 in progress and 3 not started. 
The sprint ends in 4 days."
```

**Work Coordination (Orchestrator Role - NOT PM)**:
```
"I'll assign the remaining user stories to developers and coordinate
the testing activities to complete the sprint on time."
```

### Using Planning Tools for Tracking

**✅ CORRECT - PM Using Planning Tools**:
- Read existing plans to report status
- Update task completion status based on information received
- Track milestone progress
- Document deliverable status
- Report on project health

**❌ INCORRECT - PM Acting Like Orchestrator**:
- Create execution plans for teams
- Assign tasks to team members or clones
- Coordinate work between agents
- Make quality gate decisions
- Drive task completion

## Context Management for Multi-Project PMs

**When to Include Context Management**:
- Managing 5+ active projects simultaneously
- Accessing extensive project histories
- Portfolio-level reporting and analytics
- Cross-project trend analysis

**Proactive Strategies**:
- Batch project status updates
- Use summarization for historical data
- Implement progressive disclosure for detailed information
- Maintain project summaries for quick access

**Recovery Protocols**:
1. Recognize context limits approaching
2. Save current analysis to workspace
3. Resume with focused project scope
4. Use project summaries for overview
5. Drill into details as needed

This binary component approach ensures that every Project Manager agent provides clear project visibility, professional status reporting, and effective information management while maintaining strict boundaries between tracking (PM) and execution (Orchestrator) responsibilities using the core components currently available.
