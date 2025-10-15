# Domo Agent Guide

A comprehensive guide for building agents that interact directly with users, focusing on the core components currently available in the Agent C framework.

## When to Use Domo Agent Type

**Primary Purpose**: Agents designed for direct user interaction and collaboration

**Core Characteristics**:
- Direct user interaction and conversation
- Safe workspace and path handling
- Structured thinking and reflection
- Organized file and workspace management
- Code quality standards (when applicable)
- Professional, approachable personality
- User-facing communication and support
- Collaborative problem-solving approach

**Typical Scenarios**:
- General-purpose user assistance and consultation
- Development pair programming and code collaboration
- Documentation creation and content development
- File and workspace organization
- Code review and quality assurance
- Educational and tutorial guidance
- Technical analysis and consultation

**Key Requirements**:
- Must include 'domo' in agent category array
- Should include appropriate core components based on capabilities
- Optimized for conversational interfaces and user collaboration
- Professional communication standards

## Binary Component Decisions

For each component, make a clear **YES** or **NO** decision based on your agent's specific needs:

### 1. Critical Interaction Guidelines Component

**Does this agent access workspaces or file paths?**

- **YES** → Use this component *(Applies to 85% of Domo agents)*
- **NO** → Skip this component

**Reference**: [`critical_interaction_guidelines_component.md`](../01_core_components/critical_interaction_guidelines_component.md)

**Why Include**: Prevents wasted work on non-existent paths, provides immediate user feedback on path errors, ensures consistent workspace verification behavior.

**When to Skip**: Pure conversational agents without any file system access.

---

### 2. Reflection Rules Component

**Does this agent have access to the ThinkTools?**

- **YES** → Use this component *(Applies to 80% of Domo agents)*
- **NO** → Skip this component

**Reference**: [`reflection_rules_component.md`](../01_core_components/reflection_rules_component.md)

**Why Include**: Ensures systematic processing of user requests, improves response quality through structured thinking, creates valuable reasoning logs for complex problems.

**When to Skip**: Simple conversational agents or agents where thinking logs aren't beneficial.

---

### 3. Workspace Organization Component

**Does this agent use workspace tools for file and directory management?**

- **YES** → Use this component *(Applies to 90% of workspace-enabled Domo agents)*
- **NO** → Skip this component

**Reference**: [`workspace_organization_component.md`](../01_core_components/workspace_organization_component.md)

**Why Include**: Standardizes file management, supports user collaboration, provides systematic organization for long-term projects, enables effective workspace handoffs.

**When to Skip**: Agents without any file management capabilities.

---

### 4A. Code Quality Standards Component (Python)

**Does this agent write or modify Python code?**

- **YES** → Use this component
- **NO** → Skip OR use language-appropriate variant

**Reference**: [`code_quality_python_component.md`](../01_core_components/code_quality_python_component.md)

**Why Include**: Ensures consistent Python code quality, prevents technical debt, provides systematic approach to development practices.

**When to Skip**: Non-coding agents or agents working in other languages.

---

### 4B. Code Quality Standards Component (C#)

**Does this agent write or modify C# code?**

- **YES** → Use this component
- **NO** → Skip OR use language-appropriate variant

**Reference**: [`code_quality_csharp_component.md`](../01_core_components/code_quality_csharp_component.md)

**Why Include**: Ensures consistent C# code quality, follows .NET best practices, maintains enterprise-grade development standards.

**When to Skip**: Non-coding agents or agents working in other languages.

---

### 4C. Code Quality Standards Component (TypeScript)

**Does this agent write or modify TypeScript/JavaScript code?**

- **YES** → Use this component
- **NO** → Skip OR use language-appropriate variant

**Reference**: [`code_quality_typescript_component.md`](../01_core_components/code_quality_typescript_component.md)

**Why Include**: Ensures consistent TypeScript development practices, maintains modern JavaScript standards, supports full-stack development quality.

**When to Skip**: Non-coding agents or agents working in other languages.

## Typical Structure and Composition Order

Based on the binary component decisions above, here's the recommended persona organization:

### Component Ordering Principle

**Recommended Order (Foundation → Specialization → Personality)**:

1. **Core Guidelines First** (Critical Interaction Guidelines, Reflection Rules)
   - Establishes safety and thinking patterns
   - Foundation that all other sections build upon

2. **Operational Standards** (Workspace Organization, Code Quality)
   - Defines how the agent performs its work
   - Sets quality and organizational standards

3. **Domain Expertise** (Custom sections)
   - Specialized knowledge and capabilities
   - Built on top of the foundational patterns

4. **Personality Last** (Communication style, approach)
   - Defines how the agent expresses itself
   - Applied across all previous sections

This ordering ensures that fundamental safety and quality patterns are established before adding specialized capabilities and personality customization.

### Standard Domo Agent Structure

```markdown
# Agent Identity and Core Purpose
[Custom agent identity, role, and primary mission]

## Critical Interaction Guidelines
[If workspace access - YES/NO decision]

## Reflection Rules
[If ThinkTools access - YES/NO decision]

## Workspace Organization Guidelines  
[If workspace tools - YES/NO decision]

## Code Quality Requirements
[If coding agent - Choose Python/C#/TypeScript variant]

## [Domain Name] Expertise
[Custom domain knowledge and specialized skills]

# Personality and Communication Style
[Custom personality traits, communication approach, and user interaction style]

# Reference Materials
[Links to relevant documentation, standards, or resources]
```

### Minimal Domo Agent Structure

For simple conversational agents:

```markdown
# Agent Identity and Core Purpose
[Custom agent identity and mission]

# Personality and Communication Style  
[Custom personality and communication approach]
```

### Development-Focused Domo Agent Structure

For coding and technical agents:

```markdown
# Agent Identity and Core Purpose
[Custom agent identity, role, and technical focus]

## Critical Interaction Guidelines
[Workspace safety and path verification]

## Reflection Rules  
[Systematic thinking and analysis requirements]

## Workspace Organization Guidelines
[Comprehensive file and collaboration management]

## Code Quality Requirements - [Language]
[Language-specific development standards and practices]

## [Technical Domain] Expertise
[Specialized technical knowledge and methodologies]

# Personality and Communication Style
[Professional, collaborative, technically precise]

# Reference Materials
[Technical standards, best practices, development resources]
```

## Customization Guidance

### Focus Area Adaptations

**General Purpose Domo Agents**:
- Focus on communication and collaboration skills
- Include broad problem-solving approaches
- Emphasize user guidance and support
- Consider Critical Interaction Guidelines if workspace access needed

**Development-Focused Domo Agents**:
- Include appropriate Code Quality Standards component
- Emphasize technical precision and standards
- Include Critical Interaction Guidelines for safe workspace operations
- Use Reflection Rules for systematic code analysis

**Documentation-Focused Domo Agents**:
- Include Workspace Organization component
- Emphasize content quality and organization
- Use structured approaches to document creation
- Consider Reflection Rules for comprehensive analysis

**Analysis/Research Domo Agents**:
- Include Reflection Rules for systematic thinking
- Focus on methodical information processing
- Emphasize analytical and recommendation generation
- Use workspace tools for information organization

### Domain-Specific Considerations

**Add Custom Sections For**:
- Industry-specific knowledge and compliance requirements
- Specialized tools and methodologies
- Domain-specific quality standards
- Professional communication protocols
- Technical standards and certifications

**Adapt Components For**:
- **Code Quality**: Choose language-appropriate variant and customize for specific frameworks
- **Workspace Organization**: Adapt file organization for domain conventions
- **Critical Interaction Guidelines**: Customize for domain-specific workspace patterns
- **Reflection Rules**: Tailor thinking triggers for domain-specific scenarios

### Personality Customization

**Communication Style Options**:
- **Professional Consultant**: Formal, analytical, recommendation-focused
- **Collaborative Partner**: Friendly, supportive, guidance-oriented  
- **Technical Expert**: Precise, detailed, standards-focused
- **Creative Collaborator**: Innovative, exploratory, idea-generating

**Maintain Domo Characteristics**:
- Always user-focused and helpful
- Clear communication and explanation
- Professional boundaries and ethics
- Collaborative rather than directive approach
- Respect for user expertise and preferences

## Real Examples from the Ecosystem

### General Purpose Domo Agent
**Agent**: `default.yaml`

**Component Selections**:
- ✅ Critical Interaction Guidelines  
- ✅ Reflection Rules
- ✅ Workspace Organization
- ❌ Code Quality Standards (not a coding agent)

**Characteristics**: Balanced general assistance, user collaboration focus, safe workspace operations

---

### Development-Focused Domo Agent  
**Agent**: `cora_agent_c_core_dev.yaml`

**Component Selections**:
- ✅ Critical Interaction Guidelines
- ✅ Reflection Rules  
- ✅ Workspace Organization
- ✅ Code Quality Standards (Python)

**Characteristics**: Development pair programming, code quality focus, technical precision, systematic development approach

---

### Specialist Tool-Building Domo Agent
**Agent**: `tim_the_toolman_agent_tool_builder.yaml`

**Component Selections**:
- ✅ Critical Interaction Guidelines
- ✅ Reflection Rules
- ✅ Workspace Organization  
- ✅ Code Quality Standards (Python)

**Characteristics**: Tool development specialization, systematic build approach, quality focus

---

### Documentation-Focused Domo Agent
**Agent**: `alexandra_strategic_document_architect.yaml`

**Component Selections**:
- ✅ Critical Interaction Guidelines
- ✅ Reflection Rules
- ✅ Workspace Organization
- ❌ Code Quality Standards (documentation focus)

**Characteristics**: Document creation focus, systematic content development, organizational excellence

## Component Integration Benefits

### Why Binary Decisions Work

**For Domo Agents Specifically**:
- **User-Focused Clarity**: Binary decisions create predictable, consistent user experiences
- **Professional Standards**: Component-based approach maintains quality baseline across all user-facing agents
- **Customization Efficiency**: Binary choices speed up agent creation while preserving flexibility
- **Safe Operations**: Components ensure consistent safe workspace and file handling

**Quality Outcomes**:
- **Consistent User Experience**: Agents with similar components behave predictably for users
- **Professional Polish**: Component standards maintain professional interaction quality
- **Reduced Complexity**: Binary choices eliminate partial implementations that confuse users
- **Quality Assurance**: Components enforce best practices and standards

### Integration Patterns

**Essential Component Combinations**:
- Workspace Organization + Critical Interaction Guidelines = Safe file operations
- Reflection Rules + Code Quality Standards = Thoughtful development work
- Critical Interaction Guidelines + Reflection Rules = Safe, systematic operations

**Core Integration Benefits**:
- **Safe Operations**: Critical Interaction Guidelines prevent workspace errors
- **Quality Code**: Code Quality Standards ensure professional development
- **Organized Collaboration**: Workspace Organization supports user collaboration
- **Thoughtful Responses**: Reflection Rules improve response quality and analysis

## Proper YAML Configuration Structure

**CRITICAL**: Agent YAML files must follow the exact field order shown below to prevent agent loading failures.

### Required Field Order

```yaml
version: 2
key: your_agent_key_here
name: "Your Agent Display Name"
model_id: "claude-3.5-sonnet"
agent_description: |
  Brief description of the agent's purpose and capabilities.
tools:
  - ThinkTools
  - WorkspaceTools
  # Additional tools as needed
agent_params:
  type: "claude_reasoning"
  budget_tokens: 20000
  max_tokens: 64000
  # Additional parameters as needed
category:
  - "domo"
  - "your_domain"
  # Additional categories as needed
persona: |
  # The persona content MUST be LAST
  # This contains all your component selections and custom instructions
```

### Critical Structure Rules

1. **Field Order Matters**: Fields must appear in the exact order shown above
2. **Persona Must Be LAST**: The persona field must always be the final field in the YAML file
3. **Required Fields**: All fields shown above are required for proper agent function
4. **Category Array**: Must include "domo" for user-facing agents

### Common Structure Errors to Avoid

❌ **WRONG - Persona not last:**
```yaml
version: 2
key: my_agent
name: "My Agent"
persona: |  # ← WRONG! Persona should be last
  Agent instructions here...
tools:
  - ThinkTools
```

✅ **CORRECT - Persona is last:**
```yaml
version: 2
key: my_agent
name: "My Agent"
tools:
  - ThinkTools
persona: |  # ← CORRECT! Persona is last
  Agent instructions here...
```

## Getting Started

### Step-by-Step Agent Creation

1. **Define Agent Purpose**: Clearly identify the agent's primary role and user interaction model
2. **Make Binary Decisions**: Go through each component with clear YES/NO choices based on agent needs
3. **Choose Language Variants**: Select appropriate Code Quality component for programming language if applicable
4. **Create Proper YAML Structure**: Use the exact field order shown above
5. **Structure the Persona**: Use the typical structure as a template and arrange selected components logically
6. **Add Domain Expertise**: Include specialized knowledge and custom sections as needed
7. **Customize Personality**: Define communication style and interaction approach while maintaining Domo characteristics
8. **Validate YAML Structure**: Verify fields are in correct order with persona LAST
9. **Validate Composition**: Ensure component selections align with agent purpose and user needs
10. **Test Integration**: Verify components work together smoothly for intended use cases

### Quality Checklist

**Required for All Domo Agents**:
- ✅ Includes 'domo' in category array
- ✅ Clear user-focused communication style
- ✅ Professional interaction standards
- ✅ Appropriate component selection based on capabilities

**Recommended Best Practices**:  
- ✅ Safe workspace operations (Critical Interaction Guidelines for workspace-enabled agents)
- ✅ Structured thinking for quality responses (Reflection Rules for complex agents)
- ✅ Organized file management for collaboration (Workspace Organization for file management)
- ✅ Code quality standards (Language-appropriate Code Quality components for development agents)

**Quality Validation**:
- ✅ Component selections match agent capabilities and tools
- ✅ No conflicting or contradictory instructions between components
- ✅ Clear communication style and approach defined
- ✅ Professional standards maintained throughout persona
- ✅ Domain expertise appropriately structured

This binary component approach ensures that every Domo agent provides a consistent, professional, and effective user collaboration experience while maintaining the flexibility to specialize for specific domains and use cases using the core components currently available.