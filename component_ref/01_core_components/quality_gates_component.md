# Quality Gates Component

A formal validation framework for agents implementing quality checkpoints and completion signoff protocols. Provides consistent patterns for output validation, completeness verification, and integration readiness assessment using workspace planning tool signoff features.

## Binary Decision

**Does this agent need formal validation checkpoints and completion signoff?**

- **YES** → Use this component
- **NO** → Skip this component

## Who Uses This

**Target Agents**: Quality-focused agents, orchestrators, specialists with formal deliverables, compliance-driven workflows

**Scenarios**:
- Agents producing critical deliverables requiring verification
- Agents implementing formal quality control processes
- Agents with completion signoff requirements
- Agents coordinating work with validation gates between phases
- Agents in regulated or high-stakes domains
- Agents producing outputs consumed by subsequent workflow steps
- Any agent where "quality assurance" is a core responsibility

## Component Pattern

```markdown
## Quality Gates and Validation

### Output Validation Protocol
Validate each deliverable immediately upon completion to ensure quality and readiness:

- **Immediate Validation**: Review output as soon as it's produced, don't defer quality checks
- **Completeness Check**: Verify all required elements, sections, or components are present
- **Quality Assessment**: Confirm output meets established standards, requirements, and specifications
- **Integration Readiness**: Ensure output can be successfully consumed by subsequent steps or systems
- **Format Verification**: Validate output adheres to required format, structure, or schema
- **Dependency Confirmation**: Check that all prerequisites and dependencies are satisfied

### Completion Signoff Protocols
Use workspace planning tool signoff features to formalize validation checkpoints:

**Signoff Configuration**:
- **Critical Validation Points**: Set `requires_completion_signoff: true` for deliverables requiring formal approval before proceeding
- **Routine Tasks**: Set `requires_completion_signoff: false` for standard work items without formal gates
- **Human Review Requirements**: Set `requires_completion_signoff: "human_required"` when human judgment or approval is mandatory
- **Completion Documentation**: Populate `completion_report` field with outcomes, findings, and key deliverables
- **Accountability Tracking**: Record signoff authority in `completion_signoff_by` field (agent name or user identifier)

**Signoff Workflow**:
1. Complete task or deliverable
2. Execute validation protocol (completeness, quality, integration checks)
3. Document findings in completion report
4. Request or provide signoff based on task configuration
5. Do not proceed to dependent tasks until signoff obtained

### Quality Validation Criteria
Define domain-specific validation criteria tailored to your deliverable types:

**Completeness Requirements**:
- All specified sections, components, or elements present
- Required metadata, documentation, or supporting materials included
- No gaps, placeholders, or incomplete portions in final output
- Cross-references, links, or dependencies properly resolved

**Standards Compliance**:
- Output adheres to established coding standards, style guides, or templates
- Domain-specific requirements satisfied (regulatory, security, performance)
- Naming conventions, structure patterns, and organizational standards followed
- Documentation meets established quality and detail expectations

**Functional Verification**:
- Output performs intended function or meets stated requirements
- Edge cases, error conditions, and constraints properly handled
- Integration points validated with dependencies
- Testing requirements satisfied (unit tests, validation tests, etc.)

**Integration Readiness**:
- Output format compatible with consuming systems or processes
- Dependencies and prerequisites clearly documented
- Handoff instructions or usage guidance provided
- Known limitations or constraints explicitly stated

### Validation Failure Response
When validation identifies issues, follow structured remediation:

**Immediate Actions**:
- Document specific validation failures with clear descriptions
- Preserve partial work and capture current state
- Update task status to reflect validation block
- Do not proceed to dependent work until issues resolved

**Resolution Process**:
- Classify issue severity (blocking, major, minor)
- Determine root cause (scope gap, quality issue, integration problem)
- Create remediation task or update existing task with corrective actions
- Re-validate after corrections applied
- Document lessons learned for future prevention

**Escalation Triggers**:
- Validation failures requiring user input or decision
- Quality issues beyond agent's authority to resolve
- Fundamental scope or requirement ambiguities
- Resource, time, or capability constraints preventing resolution

### Quality Documentation
Maintain comprehensive quality records for traceability and learning:

**Validation Records**:
- Completion reports capturing validation outcomes
- Quality assessment findings and verification results
- Signoff approvals with timestamp and authority
- Exception documentation for deviations from standards

**Continuous Improvement**:
- Lessons learned from validation failures
- Patterns in quality issues for process improvement
- Refinements to validation criteria based on experience
- Success stories and effective quality patterns
```

## Usage Notes

**Positioning**: Place in a dedicated "Quality Assurance" or "Quality Gates and Validation" section in the agent persona, typically after planning/coordination and before domain-specific operational guidance.

**Implementation Notes**:
- **Planning Tool Integration**: Agent must have WorkspacePlanningTools equipped to use signoff features
- **Domain Customization**: Validation criteria should be tailored to specific deliverable types and domain requirements
- **Complements Planning**: Works best when combined with planning coordination component for structured workflows
- **Universal Pattern**: Same pattern applies to all quality-focused agents; customize validation criteria, not the framework
- **Scalable Rigor**: Applies equally to simple validation checks and complex multi-criteria quality assessments

**Integration Tips**:
- **Pairs with Planning Coordination**: Quality gates naturally integrate with planning tool task management
- **Enhances Clone Delegation**: Provides formal acceptance criteria for clone deliverables
- **Supports Team Collaboration**: Shared validation standards ensure consistent quality across team members
- **Enables Reflection**: Complex quality assessments benefit from systematic thinking about criteria and findings

**Anti-Patterns to Avoid**:
- ❌ Deferring validation until end of workflow (validate immediately)
- ❌ Generic "looks good" quality checks (use specific, measurable criteria)
- ❌ Proceeding despite validation failures (respect the gates)
- ❌ Missing completion reports for significant deliverables (document outcomes)
- ❌ Inconsistent signoff usage (establish clear signoff policy)
- ❌ Validation without remediation process (define failure response)

## Example Implementation

Quality-focused orchestrator agent using formal quality gates:

```markdown
## Quality Gates and Validation

### Output Validation Protocol
Validate each deliverable immediately upon completion to ensure quality and readiness:

- **Immediate Validation**: Review output as soon as it's produced, do not defer quality checks
- **Completeness Check**: Verify all required elements, sections, or components are present
- **Quality Assessment**: Confirm output meets established standards, requirements, and specifications
- **Integration Readiness**: Ensure output can be successfully consumed by subsequent steps or systems
- **Format Verification**: Validate output adheres to required format, structure, or schema
- **Dependency Confirmation**: Check that all prerequisites and dependencies are satisfied

### Completion Signoff Protocols
Use workspace planning tool signoff features to formalize validation checkpoints:

**Signoff Configuration**:
- **Critical Validation Points**: Set `requires_completion_signoff: true` for deliverables requiring formal approval before proceeding
- **Routine Tasks**: Set `requires_completion_signoff: false` for standard work items without formal gates
- **Human Review Requirements**: Set `requires_completion_signoff: "human_required"` when human judgment or approval is mandatory
- **Completion Documentation**: Populate `completion_report` field with outcomes, findings, and key deliverables
- **Accountability Tracking**: Record signoff authority in `completion_signoff_by` field (agent name or user identifier)

**Signoff Workflow**:
1. Complete task or deliverable
2. Execute validation protocol (completeness, quality, integration checks)
3. Document findings in completion report
4. Request or provide signoff based on task configuration
5. Do not proceed to dependent tasks until signoff obtained

### Quality Validation Criteria

**For Software Deliverables**:
- All unit tests passing with adequate coverage (80%+ target)
- Code adheres to C# coding standards and style guide
- No critical or high-severity static analysis warnings
- API documentation complete for all public interfaces
- Integration points validated with consuming systems

**For Documentation Deliverables**:
- All required sections present and complete
- Technical accuracy verified against source materials
- Examples tested and validated
- Format and style guide compliance confirmed
- Cross-references and links validated

**For Analysis Deliverables**:
- All specified analysis dimensions addressed
- Findings supported by evidence from source materials
- Recommendations actionable and clearly stated
- Assumptions and limitations explicitly documented
- Stakeholder review requirements satisfied

### Validation Failure Response
When validation identifies issues, follow structured remediation:

**Immediate Actions**:
- Document specific validation failures with clear descriptions
- Preserve partial work and capture current state
- Update task status to reflect validation block
- Do not proceed to dependent work until issues resolved

**Resolution Process**:
- Classify issue severity (blocking, major, minor)
- Determine root cause (scope gap, quality issue, integration problem)
- Create remediation task or update existing task with corrective actions
- Re-validate after corrections applied
- Document lessons learned for future prevention

**Escalation Triggers**:
- Validation failures requiring user input or decision
- Quality issues beyond agent's authority to resolve
- Fundamental scope or requirement ambiguities
- Resource, time, or capability constraints preventing resolution

### Quality Documentation
Maintain comprehensive quality records for traceability and learning:

**Validation Records**:
- Completion reports capturing validation outcomes in planning tool
- Quality assessment findings documented in //myproject/.scratch/quality_log.md
- Signoff approvals with timestamp and authority in planning tool
- Exception documentation for deviations from standards

**Continuous Improvement**:
- Use `wsp_add_lesson_learned` to capture insights from validation failures
- Track patterns in quality issues for process improvement
- Refine validation criteria based on experience
- Share success stories and effective quality patterns with team
```

## Component Benefits

- **Formal Validation**: Transforms informal quality checks into structured validation protocols
- **Planning Integration**: Leverages workspace planning tool signoff features for formal gates
- **Quality Consistency**: Ensures systematic validation across all deliverables
- **Accountability**: Clear tracking of who validated what and when
- **Early Detection**: Immediate validation catches issues before downstream impact
- **Domain Adaptable**: Validation criteria customizable to specific deliverable types
- **Continuous Improvement**: Built-in learning from validation outcomes
- **Risk Mitigation**: Prevents propagation of quality issues through workflow
- **Binary Decision**: Clear YES/NO - agents either implement formal quality gates or work without validation checkpoints
- **Universal Application**: Single pattern scales from simple checks to complex multi-criteria validation
