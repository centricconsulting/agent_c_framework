# Proposing New Components

This guide provides a step-by-step process for identifying, validating, and proposing new component patterns for the Agent Component Reference Library.

## When to Propose a New Component

### Clear Indicator(s) for New Component Needs

**Strong Indicators** (Definitely propose):
- **Repeated Pattern**: You've used the same instruction pattern across 3+ different agents
- **Binary Usage**: Agents either fully implement this pattern or don't use it at all
- **Knowledge Gap**: Current library doesn't address a specific, proven capability
- **Community Need**: Multiple builders report similar missing functionality

**Good Indicators** (Consider proposing):
- **Proven Effectiveness**: Pattern significantly improves agent performance
- **Clear Focus**: Pattern addresses specific, well-defined capability
- **Reuse Potential**: Pattern would benefit from standardization across community
- **Quality Improvement**: Pattern represents better approach than existing practice

### When NOT to Propose New Components

**Don't propose when**:
- ❌ **Overlaps Existing**: Functionality already covered by existing component
- ❌ **Too Specific**: Pattern only applies to one narrow use case
- ❌ **Conditional Logic**: Pattern requires "if/then" decision making within component
- ❌ **Partial Implementation**: Pattern is incomplete or requires other components
- ❌ **Experimental**: Pattern hasn't been proven in multiple real agents

### Component Readiness Assessment

Before proposing, ensure your component idea meets these criteria:

**Pattern Validation**:
- [ ] Pattern used successfully in at least 3 different agents
- [ ] Pattern demonstrates measurable improvement in agent effectiveness
- [ ] Pattern has consistent implementation across different usage contexts
- [ ] Pattern represents stable best practice, not experimental approach

**Binary Decision Clarity**:
- [ ] Clear YES/NO criteria for when to use this component
- [ ] No ambiguous or conditional usage scenarios
- [ ] Complete implementation when selected (no partial usage)
- [ ] Obvious decision point for agent builders

**Component Independence**:
- [ ] Component works standalone without requiring other components
- [ ] Component provides complete functionality for its domain
- [ ] No dependencies on external tools or configurations
- [ ] Can be integrated with any other components without conflicts

**Community Value**:
- [ ] Multiple agent builders would benefit from this component
- [ ] Pattern addresses gap in current library coverage
- [ ] Component would reduce repetitive work across builders
- [ ] Pattern captures valuable institutional knowledge

## Component Identification Process

### 1. Pattern Recognition

**Look for Recurring Patterns**:
- Review your successful agents for repeated instruction blocks
- Identify patterns that appear across different agent types or domains
- Notice instructions that consistently improve agent effectiveness
- Document patterns that other builders have asked about or adopted

**Pattern Analysis Questions**:
- What specific capability does this pattern provide?
- How do agents behave differently with vs. without this pattern?
- Is the implementation consistent across different agents?
- Would this pattern benefit from standardization?

**Documentation Template**:
```markdown
**Pattern Name**: [Descriptive name for the pattern]
**Capability**: [What specific capability this pattern provides]
**Usage Context**: [When/where this pattern is typically used]
**Implementation**: [Core instruction block that implements pattern]
**Evidence**: [List of agents where pattern has been used successfully]
**Benefits**: [How this pattern improves agent effectiveness]
```

### 2. Binary Decision Validation

**Formulate Clear Decision Criteria**:
- Create simple YES/NO question for when to use this component
- Ensure decision criteria are unambiguous and actionable
- Test decision criteria with other builders for clarity
- Validate that agents either fully use the pattern or don't use it at all

**Decision Criteria Template**:
```markdown
**Binary Decision**: Does this agent [specific capability need]?
- **YES** → Use this component
- **NO** → Skip this component

**Decision Examples**:
- Agent Type A: YES because [specific reason]
- Agent Type B: NO because [specific reason]
- Agent Type C: YES because [specific reason]
```

**Validation Testing**:
- Ask other builders to make the YES/NO decision for 5 different agent scenarios
- Confirm all builders reach same decision for same scenarios
- Refine criteria if any ambiguity or disagreement emerges

### 3. Scope Definition

**Define Component Boundaries**:
- Identify what capabilities are included in this component
- Clarify what capabilities are explicitly excluded
- Ensure no overlap with existing components
- Validate that component has single, clear purpose

**Scope Documentation**:
```markdown
**Included Capabilities**:
- [Specific capability 1]
- [Specific capability 2]
- [Specific capability 3]

**Excluded Capabilities**:
- [Related but separate capability 1]
- [Related but separate capability 2]

**Relationship to Existing Components**:
- [How this differs from Component X]
- [Why this doesn't overlap with Component Y]
```

### 4. Effectiveness Validation

**Measure Pattern Impact**:
- Document how agents perform with vs. without this pattern
- Collect feedback from builders who've used this pattern
- Identify specific problems this pattern solves
- Validate that pattern represents best practice

**Evidence Collection**:
```markdown
**Agent Examples Using Pattern**:
- [Agent Name]: [Brief description of successful usage]
- [Agent Name]: [Brief description of successful usage]
- [Agent Name]: [Brief description of successful usage]

**Effectiveness Evidence**:
- [Specific improvement in agent behavior]
- [Problem this pattern consistently solves]
- [Builder feedback on pattern value]

**Alternative Approaches Considered**:
- [Alternative approach 1]: [Why pattern is better]
- [Alternative approach 2]: [Why pattern is better]
```

## Component Proposal Process

### Phase 1: Initial Proposal

**1. Create Proposal Document**

Use this template for your initial proposal:

```markdown
# Component Proposal: [Component Name]

## Component Overview
**Component Name**: [Descriptive name]
**Component Purpose**: [One sentence describing what this component does]
**Target Agent Types**: [Which types of agents would use this]

## Binary Decision Criteria
**Decision Question**: Does this agent [specific need]?
- **YES** → Use this component
- **NO** → Skip this component

**Decision Examples**:
- [Agent Type/Scenario]: YES/NO because [reason]
- [Agent Type/Scenario]: YES/NO because [reason]
- [Agent Type/Scenario]: YES/NO because [reason]

## Pattern Evidence
**Existing Usage**:
- [Agent Name] - [How pattern is used and results]
- [Agent Name] - [How pattern is used and results]
- [Agent Name] - [How pattern is used and results]

**Effectiveness Data**:
- [Specific improvement this pattern provides]
- [Problem this pattern solves]
- [Feedback from builders using this pattern]

## Component Pattern
**Core Instructions**:
```markdown
[Complete instruction block for this component]
```

**Customization Guidance**:
[How builders can adapt this component for specific needs]

## Quality Validation
**Independence Check**: [How this component works standalone]
**Integration Check**: [How this integrates with other components]
**Completeness Check**: [How this provides complete functionality]
**Clarity Check**: [How instructions are clear and actionable]

## Implementation Plan
**Testing Strategy**: [How you plan to validate this component]
**Documentation Plan**: [What documentation you'll create]
**Community Validation**: [How you'll gather community feedback]
```

**2. Community Feedback Round**

- Share proposal in community channels
- Request feedback on binary decision criteria
- Gather input on component scope and boundaries
- Collect suggestions for improvements or refinements

**Feedback Integration**:
- Address all community concerns and questions
- Refine component based on feedback
- Update proposal document with improvements
- Confirm community support for moving forward

### Phase 2: Component Development

**1. Component Creation**

**Draft Complete Component**:
- Write full component following standard structure
- Include all required sections (decision criteria, pattern, usage notes, examples)
- Ensure component meets all quality guidelines
- Create comprehensive usage documentation

**Component Structure**:
```markdown
## [Component Name] Component

**Binary Decision**: [Clear yes/no criteria]
- **YES** → Use this component
- **NO** → Skip this component

**Who Uses This**: [Target agent types]

**Component Pattern**:
```markdown
[Complete instruction block with proper formatting]
```

**Usage Notes**:
- [Customization guidance]
- [Implementation considerations]
- [Integration recommendations]

**Examples in Use**:
- [Agent example 1] - [Usage description]
- [Agent example 2] - [Usage description]
- [Agent example 3] - [Usage description]
```

**2. Testing and Validation**

**Functional Testing**:
- Build at least 3 test agents using the component
- Validate that component produces expected behavior
- Test component integration with other common components
- Document any issues or refinements needed

**Usage Testing**:
- Have other builders use component to create agents
- Gather feedback on clarity and usability
- Identify any ambiguity or missing information
- Refine component based on usage experience

**Quality Validation**:
- Complete full quality checklist review
- Verify component meets all binary decision model requirements
- Confirm component independence and integration compatibility
- Validate evidence and pattern documentation

### Phase 3: Community Review

**1. Detailed Component Review**

**Share Complete Component**:
- Post complete component for community review
- Include all testing results and validation evidence
- Request specific feedback on quality and effectiveness
- Ask for integration testing with different agent types

**Review Areas**:
- Binary decision criteria clarity
- Component pattern completeness
- Usage guidance adequacy
- Integration compatibility
- Overall quality and value

**2. Consensus Building**

**Address Review Feedback**:
- Respond to all community concerns and suggestions
- Make necessary refinements to component
- Update documentation based on feedback
- Confirm component meets community standards

**Build Agreement**:
- Work toward community consensus on component value
- Resolve any disagreements through discussion
- Ensure broad support from active contributors
- Document final component version with community approval

### Phase 4: Integration

**1. Library Integration**

**Component Addition**:
- Add component to appropriate library location
- Update library documentation to reference new component
- Include component in relevant agent type guides
- Update examples and usage documentation

**Integration Testing**:
- Test component with all existing agent type guides
- Verify no conflicts with existing components
- Update any affected documentation
- Confirm library consistency maintained

**2. Launch and Documentation**

**Announcement**:
- Announce new component to community
- Provide usage guidance and examples
- Share success stories and validation results
- Encourage adoption and feedback

**Usage Support**:
- Monitor component adoption and usage patterns
- Provide support for builders using component
- Collect feedback on real-world effectiveness
- Document lessons learned for future improvements

## Common Proposal Challenges and Solutions

### Challenge 1: Scope Definition
**Problem**: Difficulty determining appropriate component boundaries
**Solution**:
- Start with proven pattern from your own agents
- Focus on single, clear capability
- Test scope boundaries with other builders
- Split into multiple focused components if needed

### Challenge 2: Binary Decision Criteria
**Problem**: Difficulty creating clear YES/NO criteria
**Solution**:
- Test criteria with multiple agent scenarios
- Get feedback from other builders on clarity
- Refine criteria based on actual usage decisions
- Avoid conditional or ambiguous language

### Challenge 3: Pattern Generalization
**Problem**: Making specific pattern work for general usage
**Solution**:
- Identify core pattern elements vs. customization details
- Provide clear customization guidance
- Test pattern with different agent types and domains
- Create focused variants if needed for different contexts

### Challenge 4: Evidence Collection
**Problem**: Demonstrating pattern effectiveness
**Solution**:
- Document specific problems pattern solves
- Collect before/after comparisons showing improvement
- Gather feedback from multiple builders who've used pattern
- Show pattern usage across different successful agents

## Success Indicators

### Proposal Quality Indicators
- Clear, unambiguous binary decision criteria
- Strong evidence from multiple real agent implementations
- Positive community feedback and support
- Component passes all quality validation checks

### Component Adoption Indicators
- Multiple builders successfully use component in new agents
- Component integrates well with existing library components
- Community reports component improves agent building effectiveness
- Component becomes referenced in agent type guides

### Long-term Success Indicators
- Component remains relevant and useful over time
- Pattern continues to be effective in new agent contexts
- Community maintains and improves component
- Component contributes to overall library quality and value

---

Following this process ensures new component proposals align with the binary decision model while providing maximum value to the agent building community. Remember: focus on proven patterns that solve real problems with clear YES/NO usage criteria.