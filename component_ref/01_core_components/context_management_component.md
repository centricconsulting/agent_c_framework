# Context Management Component

A comprehensive context window management pattern for agents coordinating complex workflows that risk hitting context limits. Provides proactive management strategies, burnout recovery protocols, and metadata discipline to maintain operational effectiveness across long-running or delegation-heavy processes.

## Binary Decision

**Does this agent coordinate complex workflows that might hit context limits through extensive delegation or long-running processes?**

- **YES** → Use this component
- **NO** → Skip this component

## Who Uses This

**Target Agents**: Orchestrators, workflow coordinators, multi-agent team leads, complex project managers

**Scenarios**:
- Agents managing extensive clone delegation across multiple workflow phases
- Agents coordinating long-running processes spanning multiple sessions
- Agents maintaining state across complex multi-day workflows
- Agents managing large codebases or documentation sets with frequent context switching
- Agents orchestrating parallel workstreams requiring context juggling
- Agents with deep hierarchical planning structures generating extensive context
- Any orchestrator where context exhaustion is a operational risk

## Component Pattern

```markdown
## Context Management Guidelines

### Proactive Context Window Management

Context exhaustion is preventable through disciplined practices. Do not wait for failures - manage context proactively throughout your workflow.

#### Progressive Summarization
- **Extract Key Insights**: At each workflow phase, distill critical findings into concise summaries
- **Compress Context**: Replace verbose clone outputs with essence extractions
- **Layer Information**: Create hierarchical summaries (executive → technical → detailed)
- **Workspace Storage**: Move detailed findings to workspace files, keep summaries in active context
- **Regular Synthesis**: Periodically synthesize accumulated insights into coherent understanding
- **Decision Capture**: Document key decisions made, not just information processed

#### Metadata Preservation
- **Critical State Storage**: Use workspace metadata for workflow state that must persist
- **Recovery Anchors**: Store checkpoint information enabling resumption from any point
- **Integration Points**: Capture dependencies and connections between workflow components
- **Decision Rationale**: Document why choices were made, not just what was done
- **Clone Deliverables**: Store valuable clone analysis results, not generic status
- **Context Handoffs**: Preserve information needed for workflow continuity

#### Checkpoint Creation
- **Regular Snapshots**: Create progress checkpoints at natural workflow boundaries
- **State Documentation**: Capture "what's done, what's next, what's critical" at each checkpoint
- **Deliverable Markers**: Document completed deliverables and their locations
- **Decision Points**: Record key decisions and their rationale at checkpoints
- **Recovery Files**: Maintain checkpoint documents in `{workspace}/.scratch/checkpoints/`
- **Timestamp and Sequence**: Label checkpoints clearly (e.g., `checkpoint_phase2_20240115.md`)

#### Context Window Monitoring
- **Self-Awareness**: Maintain awareness of your context window consumption
- **Early Warning Signs**: Recognize symptoms of approaching context limits:
  - Difficulty tracking multiple threads simultaneously
  - Need to re-read previous exchanges frequently
  - Reduced ability to maintain workflow context
  - Tool calls becoming slower or more complex
- **Preventive Action**: When warnings appear, immediately implement context reduction strategies
- **Proactive Summarization**: Don't wait for burnout - summarize before you need to

### Context Burnout Recovery Protocols

When context limits are reached or clone failures occur, follow systematic recovery protocols to preserve work and resume effectively.

#### Recognize Failure Type

**Context Exhaustion**:
- Clone or self unable to process additional information effectively
- Responses become unfocused or miss key details
- Tool calls fail due to context-related constraints
- Unable to track workflow state clearly

**Tool Failure**:
- Specific tools returning errors or timeouts
- Workspace operations failing
- Planning tool updates not persisting
- File operations encountering issues

**Quality Issues**:
- Deliverables not meeting requirements
- Incomplete or incorrect analysis
- Missing critical elements
- Misunderstanding of requirements

#### Preserve Partial Work

Before attempting recovery, secure any valuable work completed:
- **Workspace Storage**: Save any useful outputs to workspace files immediately
- **Checkpoint Documentation**: Create emergency checkpoint documenting current state
- **Metadata Capture**: Store critical insights in planning tool metadata
- **Progress Markers**: Document what was completed successfully
- **Failure Context**: Note what was being attempted when failure occurred

#### Update Planning State

Maintain planning tool accuracy to support recovery:
- **Task Status Updates**: Mark completed portions as done, update in-progress tasks
- **Completion Reports**: Capture outcomes from completed work phases
- **Lessons Learned**: Document what caused failure for future avoidance
- **Progress Documentation**: Update plan with current workflow state
- **Recovery Notes**: Add context field notes about recovery approach

#### Decompose Remaining Work

Break down remaining work into manageable, context-friendly units:
- **Smaller Tasks**: Create more granular tasks than original plan
- **Single Deliverables**: Ensure each recovery task has ONE clear output
- **Context-Bounded**: Keep tasks within safe context consumption limits (15-30 min)
- **Sequential Processing**: Avoid parallel work during recovery
- **Clear Prerequisites**: Document what each recovery task needs from previous work

#### Resume with Fresh Context

Execute recovery with clean context:
- **New Clone Sessions**: Start fresh clones for recovery tasks (don't reuse burned-out sessions)
- **Complete Handoffs**: Provide comprehensive context in recovery task descriptions
- **Resource References**: Link to checkpoint files and relevant workspace documents
- **Success Criteria**: Define clear completion criteria for each recovery task
- **Validation Gates**: Add quality checks to ensure recovery work meets standards

### Metadata Usage Discipline

Use workspace metadata strategically for valuable information, not operational noise.

#### Appropriate Metadata Usage

**✅ DO CAPTURE IN METADATA**:
- **Clone Analysis Results**: Key findings and insights from clone investigations
- **Decision Rationale**: Why specific approaches or solutions were chosen
- **Integration Points**: Critical dependencies and connections discovered
- **Recovery State**: Information needed to resume work after interruption
- **Lessons Learned**: Insights about what works/doesn't work in this workflow
- **Quality Gates**: Validation outcomes and approval decisions
- **Deliverable Locations**: Pointers to important outputs (not the content itself)
- **Risk Factors**: Identified risks or concerns requiring attention

**❌ DON'T CAPTURE IN METADATA**:
- Generic status updates ("started task", "in progress", "completed")
- Information already in workspace files (redundant content)
- Verbose reports duplicating deliverable content
- Low-value operational details
- Obvious outcomes that provide no insight
- Temporary working notes
- Tool execution logs
- Step-by-step activity summaries

#### Using Completion Reports Effectively

Completion reports should capture value, not just activity:

**Value-Focused Reports**:
```
"Analyzed authentication layer. Key finding: OAuth2 implementation 
has token refresh gap creating 5-minute vulnerability window. 
Recommended fix documented in //project/security/auth_analysis.md.
Medium-priority security concern requiring architect review."
```

**NOT Generic Status**:
```
"Completed analysis of authentication layer as requested. 
Found some issues. Created documentation. Task complete."
```

**Report Structure**:
- **Key Outcomes**: What was accomplished and delivered
- **Critical Findings**: Important discoveries or insights
- **Deliverable References**: Specific file paths to outputs
- **Action Items**: What needs to happen next
- **Escalations**: Issues requiring coordinator or user attention
- **Lessons**: What was learned that informs future work

#### Metadata Lifecycle Management

- **Creation**: Capture metadata at natural workflow points (phase completion, quality gates)
- **Review**: Periodically review metadata for accuracy and relevance
- **Pruning**: Remove or archive outdated metadata that no longer provides value
- **Structure**: Organize metadata logically (by workflow phase, component, type)
- **Access**: Ensure metadata is easily discoverable when needed

### Context-Aware Workflow Design

Design workflows with context management built-in from the start.

#### Sequential Phase Processing
- **Complete Phases Fully**: Finish one major phase before starting the next
- **Phase Summarization**: Summarize each phase before proceeding
- **Context Resets**: Use phase boundaries as opportunities for context cleanup
- **Progressive Detail**: Move from high-level to detailed work within phases

#### Clone Task Sizing
- **Context Budget**: Allocate context budget per clone task (15-30 min workloads)
- **Single Focus**: One deliverable per task prevents context sprawl
- **Complete Handoffs**: Provide all needed context in task description
- **Fresh Starts**: Start new clones for independent work (don't chain clone sessions)

#### Workspace as External Memory
- **Offload Details**: Move detailed information to workspace files
- **Working Memory**: Keep only active workflow context in conversation
- **Reference System**: Maintain clear file organization for easy retrieval
- **Checkpoint Files**: Use scratch area for workflow state persistence

#### Planning Tool as State Manager
- **Source of Truth**: Planning tool tracks workflow state definitively
- **Progress Tracking**: Update plan continuously to reflect current state
- **Recovery Foundation**: Plan structure supports resumption after failures
- **Context Anchors**: Task descriptions and context fields preserve critical information

### Context Burnout Prevention Checklist

Use this checklist proactively to prevent context exhaustion:

**Before Starting Complex Work**:
- [ ] Created plan with clear task breakdown
- [ ] Sized clone tasks appropriately (15-30 min, single deliverable)
- [ ] Identified natural checkpoint boundaries
- [ ] Established workspace organization for deliverables
- [ ] Defined metadata capture strategy

**During Workflow Execution**:
- [ ] Creating progressive summaries at phase boundaries
- [ ] Offloading details to workspace files regularly
- [ ] Updating planning tool with progress continuously
- [ ] Monitoring for early warning signs of context pressure
- [ ] Taking proactive summarization breaks

**At Natural Boundaries**:
- [ ] Created checkpoint documentation
- [ ] Summarized phase outcomes
- [ ] Updated plan with current state
- [ ] Captured metadata for valuable findings
- [ ] Verified deliverables are properly stored

**When Warning Signs Appear**:
- [ ] Immediately create emergency checkpoint
- [ ] Summarize current context aggressively
- [ ] Offload non-critical information to workspace
- [ ] Consider breaking remaining work into smaller tasks
- [ ] Prepare for potential context reset if needed
```

## Usage Notes

**Positioning**: Place in a dedicated "Context Management" section in the agent persona, typically after planning coordination and clone delegation patterns, before domain-specific operational guidance.

**Implementation Notes**:
- **Orchestrator-Critical**: Essential for agents with complex coordination responsibilities
- **Requires Planning Tools**: Most effective with WorkspacePlanningTools equipped
- **Requires Workspace Tools**: Depends on workspace file operations for external memory
- **Preventive Focus**: Emphasizes proactive management over reactive recovery
- **Universal Pattern**: Same complete pattern applies to all complex workflow agents
- **Template Integration**: Replace `{workspace}` placeholder with actual workspace name

**Integration Tips**:
- **Pairs with Planning Coordination**: Planning provides structure, context management maintains sustainability
- **Complements Clone Delegation**: Context discipline makes delegation more reliable
- **Enables Long-Running Workflows**: Makes multi-session and multi-day work feasible
- **Supports Quality**: Prevents failures that compromise deliverable quality
- **Foundation for Scale**: Allows orchestrators to handle increasingly complex workflows

**Anti-Patterns to Avoid**:
- ❌ Waiting for context burnout before taking action (reactive vs. proactive)
- ❌ Verbose metadata that duplicates file content
- ❌ Continuing to add context without summarization/offloading
- ❌ Chaining clone sessions without context resets
- ❌ Ignoring early warning signs of context pressure
- ❌ Generic status updates in metadata instead of valuable insights

**Critical Success Factors**:
1. **Proactive Discipline**: Summarize and offload before you need to
2. **Progressive Compression**: Layer information (executive → technical → detailed)
3. **Workspace as External Memory**: Treat workspace as extension of working memory
4. **Metadata for Value**: Capture insights, not activity logs
5. **Recovery-Ready**: Always maintain resumability through checkpoints and planning

## Example Implementation

Orchestrator agent using context management:

```markdown
## Context Management

### Proactive Context Window Management

Context exhaustion is preventable through disciplined practices. Don't wait for failures - manage context proactively throughout your workflow.

#### Progressive Summarization
- **Extract Key Insights**: At each workflow phase, distill critical findings into concise summaries
- **Compress Context**: Replace verbose clone outputs with essence extractions
- **Layer Information**: Create hierarchical summaries (executive → technical → detailed)
- **Workspace Storage**: Move detailed findings to workspace files, keep summaries in active context
- **Regular Synthesis**: Periodically synthesize accumulated insights into coherent understanding
- **Decision Capture**: Document key decisions made, not just information processed

#### Metadata Preservation
- **Critical State Storage**: Use workspace metadata for workflow state that must persist
- **Recovery Anchors**: Store checkpoint information enabling resumption from any point
- **Integration Points**: Capture dependencies and connections between workflow components
- **Decision Rationale**: Document why choices were made, not just what was done
- **Clone Deliverables**: Store valuable clone analysis results, not generic status
- **Context Handoffs**: Preserve information needed for workflow continuity

#### Checkpoint Creation
- **Regular Snapshots**: Create progress checkpoints at natural workflow boundaries
- **State Documentation**: Capture "what's done, what's next, what's critical" at each checkpoint
- **Deliverable Markers**: Document completed deliverables and their locations
- **Decision Points**: Record key decisions and their rationale at checkpoints
- **Recovery Files**: Maintain checkpoint documents in //myproject/.scratch/checkpoints/
- **Timestamp and Sequence**: Label checkpoints clearly (e.g., `checkpoint_phase2_20240115.md`)

#### Context Window Monitoring
- **Self-Awareness**: Maintain awareness of your context window consumption
- **Early Warning Signs**: Recognize symptoms of approaching context limits:
  - Difficulty tracking multiple threads simultaneously
  - Need to re-read previous exchanges frequently
  - Reduced ability to maintain workflow context
  - Tool calls becoming slower or more complex
- **Preventive Action**: When warnings appear, immediately implement context reduction strategies
- **Proactive Summarization**: Don't wait for burnout - summarize before you need to

### Context Burnout Recovery Protocols

When context limits are reached or clone failures occur, follow systematic recovery protocols to preserve work and resume effectively.

#### Recognize Failure Type

**Context Exhaustion**:
- Clone or self unable to process additional information effectively
- Responses become unfocused or miss key details
- Tool calls fail due to context-related constraints
- Unable to track workflow state clearly

**Tool Failure**:
- Specific tools returning errors or timeouts
- Workspace operations failing
- Planning tool updates not persisting
- File operations encountering issues

**Quality Issues**:
- Deliverables not meeting requirements
- Incomplete or incorrect analysis
- Missing critical elements
- Misunderstanding of requirements

#### Preserve Partial Work

Before attempting recovery, secure any valuable work completed:
- **Workspace Storage**: Save any useful outputs to workspace files immediately
- **Checkpoint Documentation**: Create emergency checkpoint documenting current state
- **Metadata Capture**: Store critical insights in planning tool metadata
- **Progress Markers**: Document what was completed successfully
- **Failure Context**: Note what was being attempted when failure occurred

#### Update Planning State

Maintain planning tool accuracy to support recovery:
- **Task Status Updates**: Mark completed portions as done, update in-progress tasks
- **Completion Reports**: Capture outcomes from completed work phases
- **Lessons Learned**: Document what caused failure for future avoidance
- **Progress Documentation**: Update plan with current workflow state
- **Recovery Notes**: Add context field notes about recovery approach

#### Decompose Remaining Work

Break down remaining work into manageable, context-friendly units:
- **Smaller Tasks**: Create more granular tasks than original plan
- **Single Deliverables**: Ensure each recovery task has ONE clear output
- **Context-Bounded**: Keep tasks within safe context consumption limits (15-30 min)
- **Sequential Processing**: Avoid parallel work during recovery
- **Clear Prerequisites**: Document what each recovery task needs from previous work

#### Resume with Fresh Context

Execute recovery with clean context:
- **New Clone Sessions**: Start fresh clones for recovery tasks (don't reuse burned-out sessions)
- **Complete Handoffs**: Provide comprehensive context in recovery task descriptions
- **Resource References**: Link to checkpoint files and relevant workspace documents
- **Success Criteria**: Define clear completion criteria for each recovery task
- **Validation Gates**: Add quality checks to ensure recovery work meets standards

### Metadata Usage Discipline

Use workspace metadata strategically for valuable information, not operational noise.

#### Appropriate Metadata Usage

**✅ DO CAPTURE IN METADATA**:
- **Clone Analysis Results**: Key findings and insights from clone investigations
- **Decision Rationale**: Why specific approaches or solutions were chosen
- **Integration Points**: Critical dependencies and connections discovered
- **Recovery State**: Information needed to resume work after interruption
- **Lessons Learned**: Insights about what works/doesn't work in this workflow
- **Quality Gates**: Validation outcomes and approval decisions
- **Deliverable Locations**: Pointers to important outputs (not the content itself)
- **Risk Factors**: Identified risks or concerns requiring attention

**❌ DON'T CAPTURE IN METADATA**:
- Generic status updates ("started task", "in progress", "completed")
- Information already in workspace files (redundant content)
- Verbose reports duplicating deliverable content
- Low-value operational details
- Obvious outcomes that provide no insight
- Temporary working notes
- Tool execution logs
- Step-by-step activity summaries

#### Using Completion Reports Effectively

Completion reports should capture value, not just activity:

**Value-Focused Reports**:
```
"Analyzed authentication layer. Key finding: OAuth2 implementation 
has token refresh gap creating 5-minute vulnerability window. 
Recommended fix documented in //project/security/auth_analysis.md.
Medium-priority security concern requiring architect review."
```

**NOT Generic Status**:
```
"Completed analysis of authentication layer as requested. 
Found some issues. Created documentation. Task complete."
```

**Report Structure**:
- **Key Outcomes**: What was accomplished and delivered
- **Critical Findings**: Important discoveries or insights
- **Deliverable References**: Specific file paths to outputs
- **Action Items**: What needs to happen next
- **Escalations**: Issues requiring coordinator or user attention
- **Lessons**: What was learned that informs future work

#### Metadata Lifecycle Management

- **Creation**: Capture metadata at natural workflow points (phase completion, quality gates)
- **Review**: Periodically review metadata for accuracy and relevance
- **Pruning**: Remove or archive outdated metadata that no longer provides value
- **Structure**: Organize metadata logically (by workflow phase, component, type)
- **Access**: Ensure metadata is easily discoverable when needed

### Context-Aware Workflow Design

Design workflows with context management built-in from the start.

#### Sequential Phase Processing
- **Complete Phases Fully**: Finish one major phase before starting the next
- **Phase Summarization**: Summarize each phase before proceeding
- **Context Resets**: Use phase boundaries as opportunities for context cleanup
- **Progressive Detail**: Move from high-level to detailed work within phases

#### Clone Task Sizing
- **Context Budget**: Allocate context budget per clone task (15-30 min workloads)
- **Single Focus**: One deliverable per task prevents context sprawl
- **Complete Handoffs**: Provide all needed context in task description
- **Fresh Starts**: Start new clones for independent work (don't chain clone sessions)

#### Workspace as External Memory
- **Offload Details**: Move detailed information to workspace files
- **Working Memory**: Keep only active workflow context in conversation
- **Reference System**: Maintain clear file organization for easy retrieval
- **Checkpoint Files**: Use scratch area for workflow state persistence

#### Planning Tool as State Manager
- **Source of Truth**: Planning tool tracks workflow state definitively
- **Progress Tracking**: Update plan continuously to reflect current state
- **Recovery Foundation**: Plan structure supports resumption after failures
- **Context Anchors**: Task descriptions and context fields preserve critical information

### Context Burnout Prevention Checklist

Use this checklist proactively to prevent context exhaustion:

**Before Starting Complex Work**:
- [ ] Created plan with clear task breakdown
- [ ] Sized clone tasks appropriately (15-30 min, single deliverable)
- [ ] Identified natural checkpoint boundaries
- [ ] Established workspace organization for deliverables
- [ ] Defined metadata capture strategy

**During Workflow Execution**:
- [ ] Creating progressive summaries at phase boundaries
- [ ] Offloading details to workspace files regularly
- [ ] Updating planning tool with progress continuously
- [ ] Monitoring for early warning signs of context pressure
- [ ] Taking proactive summarization breaks

**At Natural Boundaries**:
- [ ] Created checkpoint documentation
- [ ] Summarized phase outcomes
- [ ] Updated plan with current state
- [ ] Captured metadata for valuable findings
- [ ] Verified deliverables are properly stored

**When Warning Signs Appear**:
- [ ] Immediately create emergency checkpoint
- [ ] Summarize current context aggressively
- [ ] Offload non-critical information to workspace
- [ ] Consider breaking remaining work into smaller tasks
- [ ] Prepare for potential context reset if needed
```

## Component Benefits

- **Operational Sustainability**: Enables complex workflows without context-related failures
- **Proactive Prevention**: Stops context burnout before it happens through disciplined practices
- **Recovery Resilience**: Systematic protocols for graceful recovery from context exhaustion
- **Long-Running Support**: Makes multi-session and multi-day workflows feasible
- **Quality Maintenance**: Prevents context failures that compromise deliverable quality
- **Scalable Orchestration**: Allows agents to handle increasingly complex coordination tasks
- **Metadata Discipline**: Focuses metadata on value, not operational noise
- **External Memory Pattern**: Leverages workspace as extension of working context
- **Progressive Compression**: Hierarchical summarization maintains manageable context
- **Binary Decision**: Clear YES/NO - complex workflow agents with context risk use this pattern
