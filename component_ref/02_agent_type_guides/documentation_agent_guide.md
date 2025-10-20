# Documentation Agent Guide

A comprehensive guide for building agents specialized in content creation, organization, and client-ready preparation, focusing on the core components currently available in the Agent C framework.

## When to Use Documentation Agent Type

**Primary Purpose**: Agents designed for professional content creation, document organization, and client-ready deliverable preparation

**Core Characteristics**:
- Content quality focus and professional polish
- Organization and structure expertise
- Client-ready preparation protocols
- Navigation and usability optimization
- Technical accuracy with accessibility
- Stakeholder communication and collaboration
- Information architecture and taxonomy
- Multi-format content transformation
- Version control and document lifecycle management

**Typical Scenarios**:
- Technical documentation creation and maintenance
- Business documentation and proposal development
- Content organization and information architecture
- Document refinement and professional polish
- Multi-format document transformation (Markdown → HTML, DOCX, PDF)
- Documentation standards and style guide enforcement
- Client-facing deliverable preparation
- Knowledge base and documentation portal development
- Document quality assurance and review
- Content migration and consolidation

**Key Requirements**:
- Must include 'domo' in agent category array (user-facing content work)
- Should include appropriate core components based on documentation scope
- Optimized for content quality, organization, and professional polish
- Strong emphasis on client-ready standards

**Documentation vs. General Domo Distinction**:
- **Documentation Agents**: Specialized in content creation, structure, and polish for specific documentation outputs
- **General Domo Agents**: Broader user assistance without specific content delivery focus
- **Key Differentiator**: Documentation agents produce professional, client-ready documents as primary deliverable

## Binary Component Decisions

For each component, make a clear **YES** or **NO** decision based on your agent's specific needs:

### 1. Critical Interaction Guidelines Component

**Does this agent access workspaces or file paths for document management?**

- **YES** → Use this component *(Applies to 95% of Documentation agents)*
- **NO** → Skip this component

**Reference**: [`critical_interaction_guidelines_component.md`](../01_core_components/critical_interaction_guidelines_component.md)

**Why Include**: Documentation work requires extensive file operations, prevents wasted work on non-existent paths, ensures document integrity through safe operations.

**When to Skip**: Only for pure conversational documentation consultants without file access.

---

### 2. Human Pairing Protocol Component (General)

**Does this agent work collaboratively with users on documentation projects?**

- **YES** → Use this component *(Applies to 90% of Documentation agents)*
- **NO** → Skip this component

**Reference**: [`human_pairing_general_component.md`](../01_core_components/human_pairing_general_component.md)

**Why Include**: Documentation work is inherently collaborative, ensures effective stakeholder communication, supports iterative content refinement, facilitates user feedback integration.

**When to Skip**: Automated documentation processors without direct user collaboration.

---

### 3. Reflection Rules Component

**Does this agent have access to ThinkTools for content analysis and planning?**

- **YES** → Use this component *(Applies to 85% of Documentation agents)*
- **NO** → Skip this component

**Reference**: [`reflection_rules_component.md`](../01_core_components/reflection_rules_component.md)

**Why Include**: Supports systematic content planning, improves document structure through thoughtful analysis, ensures comprehensive coverage of topics.

**When to Skip**: Simple document formatting agents without complex decision-making.

---

### 4. Workspace Organization Component

**Does this agent use workspace tools for document and content management?**

- **YES** → Use this component *(Applies to 95% of Documentation agents)*
- **NO** → Skip this component

**Reference**: [`workspace_organization_component.md`](../01_core_components/workspace_organization_component.md)

**Why Include**: Essential for organizing documentation projects, supports document version control, enables systematic content library management, facilitates collaboration.

**When to Skip**: Agents without any file management capabilities.

---

### 5. Critical Working Rules Component

**Does this agent need systematic working protocols for documentation projects?**

- **YES** → Use this component *(Applies to 80% of Documentation agents)*
- **NO** → Skip this component

**Reference**: [`critical_working_rules_component.md`](../01_core_components/critical_working_rules_component.md)

**Why Include**: Ensures consistent documentation workflows, prevents scope creep in content projects, establishes quality checkpoints.

**When to Skip**: Simple document conversion agents with straightforward processes.

---

### 6. Planning & Coordination Component

**Does this agent manage large documentation efforts requiring project planning?**

- **YES** → Use this component *(Applies to 60% of Documentation agents)*
- **NO** → Skip this component

**Reference**: [`planning_coordination_component.md`](../01_core_components/planning_coordination_component.md)

**Why Include**: Large documentation projects require systematic planning, enables milestone tracking, supports phased content development.

**When to Skip**: Small-scale documentation agents focused on single documents or simple tasks.

---

### 7. Clone Delegation Component

**Does this agent need to delegate content creation tasks to clones?**

- **YES** → Use this component *(Applies to 50% of Documentation agents)*
- **NO** → Skip this component

**Reference**: [`clone_delegation_component.md`](../01_core_components/clone_delegation_component.md)

**Why Include**: Large documentation efforts benefit from parallel content development, enables scalable documentation production, maintains consistency across delegated work.

**When to Skip**: Single-focus documentation agents handling all content creation directly.

---

### 8. Code Quality Standards Components (Python/C#/TypeScript)

**Does this agent write or modify code in technical documentation?**

- **YES** → Use language-appropriate component
- **NO** → Skip this component *(Applies to 80% of Documentation agents)*

**Reference**: 
- [`code_quality_python_component.md`](../01_core_components/code_quality_python_component.md)
- [`code_quality_csharp_component.md`](../01_core_components/code_quality_csharp_component.md)
- [`code_quality_typescript_component.md`](../01_core_components/code_quality_typescript_component.md)

**Why Include**: Technical documentation with code examples requires quality standards, ensures code accuracy in documentation.

**When to Skip**: Most documentation agents focused on content rather than code.

---

### 9. Context Management Component

**Does this agent work on large documentation projects that may approach context limits?**

- **YES** → Use this component *(Applies to 40% of Documentation agents)*
- **NO** → Skip this component

**Reference**: [`context_management_component.md`](../01_core_components/context_management_component.md)

**Why Include**: Large documentation projects can exhaust context windows, enables recovery strategies, maintains work continuity across sessions.

**When to Skip**: Small to medium documentation agents with manageable context requirements.

---

### 10. Team Collaboration Component

**Is this agent part of a documentation team with direct agent-to-agent communication?**

- **YES** → Use this component *(Applies to 20% of Documentation agents)*
- **NO** → Skip this component

**Reference**: [`team_collaboration_component.md`](../01_core_components/team_collaboration_component.md)

**Why Include**: Documentation teams benefit from direct specialist collaboration, enables content review workflows, supports documentation specialization.

**When to Skip**: Solo documentation agents without team coordination needs.

---

### 11. Quality Gates Component

**Does this agent need formal quality checkpoints for content approval?**

- **YES** → Use this component *(Applies to 75% of Documentation agents)*
- **NO** → Skip this component

**Reference**: [`quality_gates_component.md`](../01_core_components/quality_gates_component.md)

**Why Include**: Client-ready documentation requires quality assurance, establishes clear approval workflows, ensures stakeholder signoff, maintains professional standards.

**When to Skip**: Informal documentation agents without formal approval processes.

---

### 12. Domain Knowledge Template Component

**Does this agent need structured domain expertise in documentation practices?**

- **YES** → Use this component *(Applies to 85% of Documentation agents)*
- **NO** → Skip this component

**Reference**: [`domain_knowledge_template_component.md`](../01_core_components/domain_knowledge_template_component.md)

**Why Include**: Documentation has specialized methodologies and standards, ensures consistent application of documentation best practices, provides framework for documentation expertise.

**When to Skip**: Generic document formatting agents without specialized documentation knowledge.

## Typical Structure and Composition Order

Based on the binary component decisions above, here's the recommended persona organization:

### Component Ordering Principle

**Recommended Order (Foundation → Specialization → Personality)**:

1. **Core Guidelines First** (Critical Interaction Guidelines, Human Pairing Protocol, Reflection Rules)
   - Establishes safety, collaboration patterns, and thinking approach
   - Foundation for all documentation work

2. **Operational Standards** (Workspace Organization, Critical Working Rules, Quality Gates)
   - Defines how the agent performs documentation work
   - Sets quality and organizational standards

3. **Project Management** (Planning & Coordination, Clone Delegation, Context Management)
   - Enables large-scale documentation efforts
   - Manages complexity and resource allocation

4. **Domain Expertise** (Domain Knowledge Template with documentation-specific sections)
   - Specialized documentation knowledge and methodologies
   - Built on top of the foundational patterns

5. **Personality Last** (Communication style, approach)
   - Defines how the agent expresses itself
   - Applied across all previous sections

This ordering ensures that fundamental safety, collaboration, and quality patterns are established before adding specialized capabilities and personality customization.

### Standard Documentation Agent Structure

```markdown
# Agent Identity and Core Purpose
[Custom agent identity, role, and documentation mission]

## Critical Interaction Guidelines
[Workspace safety and path verification - REQUIRED for file operations]

## Human Pairing Protocol
[User collaboration approach for documentation projects]

## Reflection Rules
[Systematic thinking for content planning and analysis]

## Workspace Organization Guidelines  
[Document management and organization standards]

## Critical Working Rules
[Documentation workflow protocols and boundaries]

## Quality Gates
[Content quality checkpoints and approval workflows]

## Planning & Coordination
[IF large documentation projects - project planning approach]

## Clone Delegation Protocols
[IF large content projects - task delegation framework]

## Context Management Strategy
[IF large documentation projects - context preservation approach]

## Documentation Expertise
[Using Domain Knowledge Template for documentation methodologies]
- Information Architecture and Content Organization
- Documentation Standards and Style Guidelines
- Client-Ready Preparation Protocols
- Multi-Format Transformation Expertise
- Content Quality Assurance Methods

# Personality and Communication Style
[Professional, detail-oriented, stakeholder-focused]

# Reference Materials
[Documentation standards, style guides, formatting resources]
```

### Minimal Documentation Agent Structure

For simple document formatting agents:

```markdown
# Agent Identity and Core Purpose
[Custom agent identity and documentation focus]

## Critical Interaction Guidelines
[Essential workspace safety]

## Workspace Organization Guidelines
[Basic document management]

## Documentation Expertise
[Core documentation knowledge]

# Personality and Communication Style  
[Professional, clear, quality-focused]
```

### Comprehensive Documentation Agent Structure

For large-scale documentation team coordinators:

```markdown
# Agent Identity and Core Purpose
[Custom agent identity, role, and strategic documentation mission]

## Critical Interaction Guidelines
[Comprehensive workspace safety and verification]

## Human Pairing Protocol
[Stakeholder collaboration and requirements gathering]

## Reflection Rules  
[Systematic content planning and strategic analysis]

## Workspace Organization Guidelines
[Enterprise documentation library management]

## Critical Working Rules
[Documentation governance and workflow enforcement]

## Quality Gates
[Multi-level content quality and stakeholder approval]

## Planning & Coordination
[Large-scale documentation project planning and milestone tracking]

## Clone Delegation Protocols
[Content creation task distribution and consistency management]

## Context Management Strategy
[Large project continuity and session handoff protocols]

## Team Collaboration Protocols
[IF documentation team - Direct specialist communication and review workflows]

## Documentation Expertise
[Comprehensive documentation methodologies and standards]
- Information Architecture and Taxonomy Design
- Documentation Lifecycle Management
- Multi-Stakeholder Communication Strategies
- Content Migration and Consolidation Methods
- Documentation Portal and Navigation Design
- Technical Accuracy with Accessibility Standards
- Version Control and Change Management

# Personality and Communication Style
[Strategic, professional, stakeholder-savvy, quality-obsessed]

# Reference Materials
[Enterprise documentation standards, industry best practices, compliance requirements]
```

## Customization Guidance

### Focus Area Adaptations

**Technical Documentation Agents**:
- Include appropriate Code Quality Standards component if documenting code
- Emphasize technical accuracy and precision
- Include specialized technical terminology management
- Consider Team Collaboration if working with technical specialists

**Business Documentation Agents**:
- Focus on stakeholder communication and professional polish
- Emphasize business language and executive summaries
- Include proposal and presentation preparation protocols
- Strong Quality Gates for client-facing deliverables

**Content Organization Specialists**:
- Heavy emphasis on Workspace Organization component
- Include information architecture expertise
- Focus on taxonomy development and navigation design
- Consider Planning & Coordination for large content migrations

**Documentation Project Managers**:
- Include Planning & Coordination and Clone Delegation components
- Emphasize project milestone tracking
- Include stakeholder communication protocols
- Strong Quality Gates with formal approval workflows

### Documentation-Specific Considerations

**Add Custom Sections For**:
- **Style Guidelines**: Industry-specific or company-specific documentation standards
- **Format Requirements**: Client deliverable format specifications (Word, PDF, HTML, etc.)
- **Audience Profiles**: Different stakeholder groups and their content needs
- **Compliance Standards**: Regulatory or certification documentation requirements
- **Template Management**: Reusable document templates and content blocks
- **Review Workflows**: Multi-stage review and approval processes

**Adapt Components For**:
- **Workspace Organization**: Adapt for documentation-specific file structures (drafts, reviews, finals, archives)
- **Quality Gates**: Customize for content quality criteria (accuracy, completeness, clarity, polish)
- **Domain Knowledge Template**: Fill with documentation-specific methodologies and best practices
- **Planning & Coordination**: Tailor for documentation project phases (outline, draft, review, finalize)

### Documentation Quality Standards

**Content Quality Criteria**:
- **Accuracy**: Technical correctness and factual precision
- **Clarity**: Clear, accessible language for target audience
- **Completeness**: Comprehensive coverage of required topics
- **Consistency**: Uniform terminology, style, and formatting
- **Professionalism**: Client-ready polish and presentation
- **Usability**: Effective navigation and information architecture

**Documentation Deliverable Standards**:
- Professional formatting and visual presentation
- Proper table of contents and navigation aids
- Consistent heading hierarchy and structure
- Appropriate use of visual elements (diagrams, tables, callouts)
- Version control and change tracking
- Metadata and document properties

### Personality Customization

**Communication Style Options**:
- **Professional Consultant**: Formal, polished, client-facing
- **Technical Communicator**: Precise, accurate, clarity-focused
- **Content Strategist**: Organizational, structural, systematic
- **Quality Specialist**: Detail-oriented, excellence-focused, thorough

**Maintain Documentation Agent Characteristics**:
- Always quality-focused and professional
- Client-ready orientation in all outputs
- Strong organizational and structural thinking
- Attention to detail and precision
- Stakeholder communication awareness
- Professional polish and presentation standards

## Real Examples from the Ecosystem

### Document Preparation Specialist
**Agent**: `diana_doc_prep_specialist.yaml`

**Component Selections**:
- ✅ Critical Interaction Guidelines  
- ✅ Human Pairing Protocol (General)
- ✅ Reflection Rules
- ✅ Workspace Organization
- ✅ Critical Working Rules
- ✅ Quality Gates
- ❌ Planning & Coordination (focused on individual document preparation)
- ❌ Clone Delegation (handles work directly)

**Characteristics**: Professional document refinement, formatting excellence, client-ready preparation, quality assurance focus

**Use Cases**: Final document polish, format transformation, professional presentation preparation

---

### Strategic Document Architect
**Agent**: `alexandra_strategic_document_architect.yaml`

**Component Selections**:
- ✅ Critical Interaction Guidelines
- ✅ Human Pairing Protocol (General)
- ✅ Reflection Rules  
- ✅ Workspace Organization
- ✅ Critical Working Rules
- ✅ Quality Gates
- ✅ Planning & Coordination (large documentation projects)
- ✅ Domain Knowledge Template (documentation expertise)
- ❌ Clone Delegation (strategic planning focus)

**Characteristics**: Strategic documentation planning, information architecture, large-scale project organization, stakeholder alignment

**Use Cases**: Documentation strategy development, content organization planning, multi-document project coordination

---

### Documentation Refiner
**Agent**: `doc_documentation_refiner.yaml`

**Component Selections**:
- ✅ Critical Interaction Guidelines
- ✅ Human Pairing Protocol (General)
- ✅ Reflection Rules
- ✅ Workspace Organization  
- ✅ Quality Gates
- ✅ Domain Knowledge Template (content quality expertise)
- ❌ Planning & Coordination (focused on content improvement)
- ❌ Clone Delegation (direct refinement work)

**Characteristics**: Content quality improvement, clarity enhancement, style consistency, professional polish

**Use Cases**: Documentation review and improvement, style guide enforcement, content clarity enhancement

## Component Integration Benefits

### Why Binary Decisions Work

**For Documentation Agents Specifically**:
- **Quality Consistency**: Binary decisions ensure all documentation agents maintain professional standards
- **Client-Ready Focus**: Component-based approach guarantees deliverable polish across all documentation work
- **Scalability**: Clear component selections enable documentation agents to scale from small to enterprise projects
- **Collaboration Efficiency**: Components ensure effective stakeholder communication and content approval workflows

**Quality Outcomes**:
- **Professional Deliverables**: Components enforce client-ready standards throughout documentation lifecycle
- **Organized Content**: Workspace Organization and Quality Gates ensure systematic document management
- **Stakeholder Alignment**: Human Pairing Protocol ensures effective communication and requirement gathering
- **Efficient Workflows**: Critical Working Rules and Planning components prevent documentation project drift

### Integration Patterns

**Essential Component Combinations for Documentation**:
- **Workspace Organization + Critical Interaction Guidelines** = Safe, organized document management
- **Quality Gates + Human Pairing Protocol** = Effective stakeholder approval workflows
- **Planning & Coordination + Clone Delegation** = Scalable large documentation projects
- **Domain Knowledge Template + Reflection Rules** = Thoughtful application of documentation best practices

**Documentation-Specific Integration Benefits**:
- **Content Quality**: Quality Gates + Domain Knowledge Template ensure professional documentation standards
- **Project Management**: Planning & Coordination + Clone Delegation enable large-scale content development
- **Stakeholder Collaboration**: Human Pairing Protocol + Quality Gates facilitate effective client engagement
- **Knowledge Preservation**: Workspace Organization + Context Management maintain documentation continuity

## Proper YAML Configuration Structure

**CRITICAL**: Agent YAML files must follow the exact field order shown below to prevent agent loading failures.

### Required Field Order

```yaml
version: 2
key: your_documentation_agent_key_here
name: "Your Documentation Agent Display Name"
model_id: "claude-3.5-sonnet"
agent_description: |
  Brief description of the documentation agent's purpose and capabilities.
tools:
  - ThinkTools
  - WorkspaceTools
  - MarkdownTools  # Common for documentation agents
  # Additional tools as needed
agent_params:
  type: "claude_reasoning"
  budget_tokens: 20000
  max_tokens: 64000
  # Additional parameters as needed
category:
  - "domo"
  - "documentation"
  # Additional categories as needed
persona: |
  # The persona content MUST be LAST
  # This contains all your component selections and custom instructions
```

### Critical Structure Rules

1. **Field Order Matters**: Fields must appear in the exact order shown above
2. **Persona Must Be LAST**: The persona field must always be the final field in the YAML file
3. **Required Fields**: All fields shown above are required for proper agent function
4. **Category Array**: Must include "domo" for user-facing documentation agents
5. **Documentation Category**: Consider adding "documentation" for agent discovery

### Common Structure Errors to Avoid

❌ **WRONG - Persona not last:**
```yaml
version: 2
key: my_doc_agent
name: "My Documentation Agent"
persona: |  # ← WRONG! Persona should be last
  Agent instructions here...
tools:
  - WorkspaceTools
```

✅ **CORRECT - Persona is last:**
```yaml
version: 2
key: my_doc_agent
name: "My Documentation Agent"
tools:
  - WorkspaceTools
  - MarkdownTools
persona: |  # ← CORRECT! Persona is last
  Agent instructions here...
```

## Getting Started

### Step-by-Step Documentation Agent Creation

1. **Define Documentation Scope**: Clearly identify the types of documents and content the agent will handle
2. **Identify Stakeholders**: Determine who will use the documentation and approval workflows needed
3. **Make Binary Decisions**: Go through each component with clear YES/NO choices based on documentation needs
4. **Assess Project Scale**: Determine if Planning & Coordination and Clone Delegation are needed for scope
5. **Create Proper YAML Structure**: Use the exact field order shown above with documentation-specific tools
6. **Structure the Persona**: Use the typical structure as a template and arrange selected components logically
7. **Add Documentation Expertise**: Customize Domain Knowledge Template with specific documentation methodologies
8. **Define Quality Standards**: Specify content quality criteria and deliverable standards
9. **Customize Personality**: Define professional, stakeholder-appropriate communication style
10. **Validate YAML Structure**: Verify fields are in correct order with persona LAST
11. **Validate Component Integration**: Ensure components work together for documentation workflows
12. **Test Documentation Workflows**: Verify end-to-end document creation and approval processes

### Quality Checklist

**Required for All Documentation Agents**:
- ✅ Includes 'domo' in category array (user-facing content work)
- ✅ Professional, client-ready communication style
- ✅ Content quality and polish standards defined
- ✅ Appropriate component selection based on documentation scope

**Recommended Best Practices**:  
- ✅ Safe workspace operations (Critical Interaction Guidelines)
- ✅ Effective stakeholder collaboration (Human Pairing Protocol)
- ✅ Systematic content planning (Reflection Rules)
- ✅ Organized document management (Workspace Organization)
- ✅ Content quality assurance (Quality Gates)
- ✅ Documentation expertise structured (Domain Knowledge Template)

**Documentation-Specific Validation**:
- ✅ Component selections match documentation scope and scale
- ✅ Quality standards align with client-ready requirements
- ✅ Stakeholder communication protocols defined
- ✅ Document organization and version control specified
- ✅ Professional polish and presentation standards maintained
- ✅ Multi-format transformation capabilities if needed

### Documentation Agent Success Indicators

**Effective Documentation Agents Demonstrate**:
- Consistent professional quality across all deliverables
- Clear stakeholder communication and requirement gathering
- Systematic content organization and structure
- Efficient document lifecycle management
- Professional polish and client-ready presentation
- Effective quality assurance and approval workflows

This binary component approach ensures that every Documentation Agent provides consistent, professional, client-ready documentation outputs while maintaining the flexibility to specialize for specific content types, industries, and project scales using the core components currently available.
