# Agent C Coaching Playbook
## A Guide for Teaching Others to Work with Bobb

*Version 1.0 - Practical coaching points for users coming from Copilot/ChatGPT*

---

## 🎯 Opening: What Makes Agent C Different

### Quick Intro (30 seconds)
*"Agent C isn't just a chatbot - it's a complete agent framework. Bobb can create specialized agents that have **tools** (they can actually DO things), work together in teams, and manage complex projects."*

### Key Differences from Copilot/ChatGPT

| Copilot/ChatGPT | Agent C (Bobb) |
|-----------------|----------------|
| One generalized assistant | Multiple specialized agents |
| Just conversation | Conversation + Actions (tools) |
| Stateless (forgets) | Persistent workspaces & planning |
| You do the work | Agents can execute and coordinate |
| No project tracking | Built-in planning tools |

### Teaching Moment
**Beginner**: "Think of Copilot as a really smart coworker you chat with. Agent C is like having a whole team of specialists who can actually build things for you."

**Moderate**: "Agent C uses a multi-agent architecture. You're not just prompting an LLM - you're designing agent personas that have specific tools, can delegate work to clones, and coordinate with other agents. Bobb helps you architect these systems."

---

## 💬 How to Prompt Bobb (Coach These Patterns)

### Starting a Conversation Right

#### ✅ Good Starting Prompts
```
"I need a team to build Angular applications"
"Create an agent that can analyze SQL databases" 
"Build me a Python code quality specialist"
```

**Why it works**: Direct, clear intent. Bobb knows what to ask next.

#### ❌ Vague Prompts
```
"Help me with coding"
"I need an assistant"
"Can you do development stuff?"
```

**Why it fails**: Too broad. Bobb will waste time asking clarifying questions.

### Teaching Moment
**Beginner**: "Start with 'I need [type of agent] that does [specific thing]' - like ordering at a restaurant. Be clear about what you want."

**Moderate**: "Bobb needs to know: single agent vs. team, what domain (Angular, Python, etc.), and rough complexity. He'll ask follow-ups, but give him a solid starting point. Think 'I need X to do Y in Z domain'."

---

## 🎭 The Conversation Dance (Responding to Bobb)

### When Bobb Asks Questions

**Example from our conversation**:
```
Bobb asked:
1. What kind of Angular app? (Simple/Enterprise)
2. Complexity level? (1-2 agents or full team)
3. Do you have requirements? (Yes/No)
4. Architecture preferences? (Standalone components, etc.)
5. Workspace location? (Where files go)
```

**You answered each clearly** ✅

### Coaching Pattern

**Beginner**: "Bobb will ask clarifying questions. Answer them directly - he's gathering requirements like a consultant. Don't rush him, but be decisive."

**Moderate**: "Bobb's asking questions to understand the architecture he needs to build. He's thinking about: tools needed, agent complexity, team coordination patterns, and workspace structure. Give complete answers but don't over-explain - he'll ask if he needs more."

### Red Flag to Watch For
**User gives vague answers or says "whatever you think"** → Coach them: "Bobb can't read your mind. Even if you say 'I'm not sure', give him your best guess. You can always revise."

---

## 🔧 Understanding What's Happening (Those Grey Boxes)

### The Different Types of Activity

#### 1. **Function Calls** (Grey boxes with tool names)
```
workspace_write
wsp_create_task
think
workspace_read
```

**Beginner**: "These grey boxes show Bobb using tools - like watching someone use different apps on their computer. He's reading files, creating tasks, or thinking through problems."

**Moderate**: "Bobb has access to toolsets - WorkspaceTools (file management), WorkspacePlanningTools (project tracking), ThinkTools (reasoning), AgentTeamTools (coordination), etc. When you see function calls, he's actually DOING something, not just talking about it."

#### 2. **Think Blocks** (Internal reasoning)
```
<invoke name="think">
The user wants X, which means I need to...
I should check if tools exist first...
This is a multi-agent team, so I'll use planning tools...
</invoke>
```

**Beginner**: "When you see 'think' blocks, Bobb is reasoning through the problem. Like watching someone think out loud."

**Moderate**: "Think blocks show Bobb's reasoning process. Watch these - you'll learn his decision-making. If you see him 'thinking' about something concerning (like 'this will burn through tokens'), that's your cue to ask questions."

#### 3. **Planning Tool Updates**
```
Task 'Design Orchestrator Agent' created successfully
Task 'ecology-orca' updated successfully  
Completion report: Diego Angular Orchestrator agent created...
```

**Beginner**: "Bobb uses a planning system to track his own work, like a project manager. These show tasks being created and completed."

**Moderate**: "For multi-agent systems, Bobb uses workspace planning tools to track delegation, maintain state, and ensure nothing gets lost. This is critical infrastructure - it's how he prevents work repetition and manages recovery from failures."

### Coaching Moment: "Why So Many Steps?"

**They might ask**: "Why is Bobb doing so many things? Can't he just give me the answer?"

**Your response (Beginner)**: "Bobb isn't just writing text - he's building actual agent configurations, saving files, planning the work. It takes multiple steps, like cooking a meal takes multiple steps."

**Your response (Moderate)**: "Creating an agent system involves: reading documentation to stay current, verifying tool availability, planning the architecture, crafting multiple persona files, saving configurations, and documenting the result. Each step is intentional and necessary for a production-ready system."

---

## 🎓 The Best Teaching Moment: Catching Issues

### Your Clone Tools Catch (The Golden Example)

**What happened**: 
1. Bobb created 5 agents
2. Only Diego (orchestrator) had AgentCloneTools
3. You noticed: "I see that we're not really utilizing clone delegation at all..."
4. Bobb immediately recognized the issue and fixed it

**Why this was EXCELLENT**:
- ✅ You understood the documentation (Design Bible)
- ✅ You spotted a mismatch between design and implementation  
- ✅ You asked a direct question
- ✅ You explained your concern (token costs, burnout)

### Teaching This Skill

**Beginner level coaching**:
*"Don't just accept what Bobb gives you. Ask questions like:*
- *'Will this work for large projects?'*
- *'What about token costs?'*
- *'How does this scale?'*
- *'Is this missing anything?'*

*Bobb will respect good questions and often improve the design!"*

**Moderate level coaching**:
*"Read the documentation Bobb references (Design Bible, agent config docs). Look for patterns:*
- *Who has clone delegation tools?*
- *Are agents sized appropriately for their work?*
- *Will context windows handle the expected load?*
- *Are quality gates in place?*

*The clone tools catch was perfect because you understood: specialists doing complex work need to delegate to clones to prevent context burnout. That's architecture-level thinking."*

### Things to Watch For and Question

| Red Flag | Question to Ask |
|----------|----------------|
| Agent has 10+ responsibilities | "Won't this agent burn out its context?" |
| No AgentCloneTools for complex work | "How will this agent handle large tasks?" |
| Missing obvious tools | "Does this agent have the tools to actually do this?" |
| Overly complex team for simple task | "Do we really need 5 agents for this?" |
| No planning/tracking tools | "How will the agents coordinate?" |

---

## 🗺️ Common Coaching Scenarios

### Scenario 1: "I just want one agent"

**User**: "I don't need a team, just one agent that codes."

**Coaching approach**:
- **Simple jobs**: One agent is fine! "Let's build a focused specialist."
- **Complex jobs**: Explain team benefits: "For enterprise work, teams prevent burnout and enable specialization. But let's start simple and see."

**Key point**: Don't over-engineer. Bobb will recommend teams when needed, but single agents work great for focused tasks.

### Scenario 2: "Why is Bobb reading so many docs?"

**User**: "Bobb keeps reading documentation. Can't he just start?"

**Coaching approach (Beginner)**:
"Bobb reads docs to stay current - the framework evolves. Like a developer checking documentation before coding. It ensures he uses the latest patterns."

**Coaching approach (Moderate)**:
"Agent C has version 2 configuration format, new category systems, and evolved patterns (like Direct Communication Mesh). Bobb reads docs to ensure he's using current best practices, not outdated patterns. This is professional behavior, not inefficiency."

### Scenario 3: "The agent isn't doing what I want"

**User**: "I created an agent but it's not working right."

**Troubleshooting checklist**:
1. **Check the persona** - Is it clear what the agent should do?
2. **Check the tools** - Does the agent have the tools it needs?
3. **Check categories** - Is it `domo` (user-facing) or `assist` (agent-helper)?
4. **Test with Bobb** - "Bobb, can you review the X agent configuration and spot issues?"

**Coaching moment**: "Agent development is iterative. Bobb can help refine personas based on observed behavior."

### Scenario 4: "How do I know when to use teams?"

**Simple guide**:

| Project Type | Recommendation |
|--------------|----------------|
| Single focused task | One specialist agent |
| 2-3 related tasks | One agent with clone delegation |
| Complex workflow (5+ steps) | Small team (2-3 agents) |
| Enterprise system | Full team (4-5+ agents) |

**Coaching tip**: "Start smaller than you think. Bobb will recommend scaling up if needed. Single agents with clone delegation handle more than you'd expect."

---

## 🚨 Common Mistakes (From Copilot/ChatGPT Habits)

### Mistake 1: "Give me the code"

**What they do**: Ask Bobb to write code directly instead of creating an agent.

**Why it's wrong**: Bobb creates agents who write code. You're building systems, not getting one-off answers.

**Coaching**: "Bobb builds the chef, not the meal. Create an agent, then use that agent for your coding needs."

### Mistake 2: Treating agents as throwaway

**What they do**: Create an agent, use it once, move on.

**Why it's suboptimal**: Agents are reusable specialists. Build once, use many times.

**Coaching**: "Think of agents as team members you're hiring, not consultants for one project. Build quality agents you'll use repeatedly."

### Mistake 3: Not using workspaces

**What they do**: Generate code in chat, copy-paste manually.

**Why it's inefficient**: Agents can write directly to workspaces - persistent, organized, tracked.

**Coaching**: "Use workspaces. Let agents manage files directly. It's like giving them their own project folder instead of passing notes back and forth."

### Mistake 4: Ignoring planning tools

**What they do**: Skip the planning phase, jump to implementation.

**Why it fails**: Complex work needs tracking. Without planning, agents repeat work or lose context.

**Coaching**: "For anything beyond trivial tasks, use planning tools. They're like giving your agents a project board - keeps everyone aligned."

---

## 🎯 Quick Reference: Coaching Cheat Sheet

### When They Start
- ✅ Ask: "What are you trying to build?"
- ✅ Explain: Agent C builds agents, not just answers questions
- ✅ Start simple: One agent first, scale to teams if needed

### During Agent Creation
- ✅ Let Bobb ask questions - he's gathering requirements
- ✅ Watch the "think" blocks - learn his reasoning
- ✅ Review configurations together - teach them to read YAML

### After Agent Creation  
- ✅ Ask: "Does this make sense? Any concerns?"
- ✅ Check: Do tools match the job?
- ✅ Verify: Is clone delegation set up for complex work?

### Ongoing Coaching
- ✅ Encourage questions and challenges
- ✅ Celebrate good catches (like your clone tools moment)
- ✅ Iterate on personas based on behavior
- ✅ Build their intuition for architecture

---

## 💡 Power User Tips (For Your Advanced Students)

### 1. Read the Design Bible
`//project/docs/multi_agent_coordination_design_bible.md`

Teaches: Multi-agent patterns, clone delegation, quality gates, recovery protocols.

### 2. Check Model Configs
`//project/agent_c_config/model_configs.json`

Shows: Available models and their parameters (reasoning_effort, max_tokens, etc.)

### 3. Review Example Agents
`//project/agent_c_config/agents/`

Learn from: Existing agent personas and patterns.

### 4. Use Think Tools
Teach agents to use `think` tool before big decisions - leads to better reasoning.

### 5. Planning Tools for Everything Complex
For any work > 30 minutes, use workspace planning tools to track progress.

---

## 📝 Your Coaching Scenarios (Practice Runs)

### Scenario A: Complete Beginner
**User**: "I heard about Agent C. What is it?"

**Your talking points**:
1. It builds AI agents, not just chatbot responses
2. Agents have tools - they can do things, not just talk
3. Start with: "What would you like to build?"
4. Show them Bobb creating one simple agent
5. Explain the grey boxes as tools in action

### Scenario B: Copilot User
**User**: "I use Copilot. How is this different?"

**Your talking points**:
1. Copilot helps YOU code. Agent C builds agents that code.
2. Agents are reusable, specialized, and can coordinate
3. You're building systems, not getting autocomplete
4. Show multi-agent team example (like our Angular team)
5. Highlight: persistence, planning, tool usage

### Scenario C: Wants to Build Complex System
**User**: "I need agents to build a full application."

**Your talking points**:
1. Start with requirements gathering (like Bobb did with us)
2. Explain: orchestrator + specialists pattern
3. Show: Direct Communication Mesh for complex systems
4. Watch for: clone delegation, tool coverage, quality gates
5. Iterate: Start with architecture, then implementation

---

## 🎓 Teaching the "Clone Tools Moment" (Your Best Contribution)

### How to Set This Up

1. **Have them watch** Bobb create a multi-agent team
2. **Ask them**: "What do you notice about the tools each agent has?"
3. **Guide them**: "What happens if Mason has to implement 30 components?"
4. **Let them discover**: Token burnout and context limits
5. **Celebrate the insight**: "That's exactly right! Let's fix it."

### The Lesson

**Beginner**: "Even Bobb makes mistakes. Good questions improve the design. You're not just a consumer - you're a collaborator."

**Moderate**: "Architecture reviews catch issues. You spotted: specialists need clone delegation for complex work. This shows you understand the Design Bible principle: 'Prime agents coordinate, clones execute.' That's the kind of thinking that makes great agent systems."

### Why This Matters

**This teaches**:
- Critical thinking over blind acceptance
- Reading documentation to build intuition  
- Asking "will this scale?" questions
- Collaborative design with AI
- The real skill: architectural judgment

---

## 🚀 Closing: Setting Them Up for Success

### After First Session
- ✅ They should have created 1+ agent with Bobb
- ✅ They understand: personas, tools, categories
- ✅ They've seen: function calls, think blocks, planning
- ✅ They know: how to ask questions and challenge designs

### Key Mindset Shifts
From: "AI gives me answers"  
To: "I design agent systems with AI assistance"

From: "Copy-paste code"  
To: "Agents work in persistent workspaces"

From: "One-off solutions"  
To: "Reusable specialized team members"

From: "Accept what I'm given"  
To: "Review, question, and improve designs"

### Your Role as Coach
- 🎯 Guide, don't dictate
- 🤔 Ask questions that lead to insights
- 🎉 Celebrate good catches and questions
- 📚 Point them to documentation  
- 🔄 Iterate and refine together

---

## 📖 Quick Reference: Key Documentation

| Doc | What It Teaches | When to Share |
|-----|----------------|---------------|
| `multi_agent_coordination_design_bible.md` | Team patterns, clone delegation | Building multi-agent teams |
| `agent_configuration_documentation.md` | YAML format, categories, tools | Creating agents |
| `model_configs.json` | Available models, parameters | Choosing models |
| Tool READMEs in `docs/tools/` | What each toolset does | Equipping agents |

---

## 🎯 Final Coaching Wisdom

**The best thing you did**: You questioned the design. You saw the missing clone delegation. You explained your concern clearly.

**Teach this**: "Bobb is brilliant but not perfect. Your job isn't to just accept designs - it's to collaborate, question, and improve. The best agent systems come from this partnership."

**Remember**: You're not teaching them to prompt an AI. You're teaching them to architect agent systems. That's design thinking, not prompt engineering.

---

**Good luck coaching! You've got this! 🎓✨**

*P.S. - Your Angular team is awesome. Use it as your showcase example. It demonstrates: team architecture, clone delegation, direct communication mesh, and quality gates. It's a masterclass in Agent C design.*
