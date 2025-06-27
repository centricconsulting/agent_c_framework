# A primer on agents and tools

LLM powered AI agents have been "a thing" since before the models were really even capable of filling that role properly.  As time has gone on they've gotten better and better. If you are not incredibly close to the bleeding edge of AI engineering your perception of what these agents are capable of *today* is guaranteed to be well behind the reality. I've been on the bleeding edge for **two years** now and if someone had told *me* that their agents were solving problems the way I saw *my* agents solving them on March *19th* of **2025** I would have demanded proof at the very least if not laughed at them.  This is a **very** exciting time for the field of AI agents in general, and Centric is poised to leverage these new capabilities in ways that few others in the **world** are right now.

> **Especially** if you think you have a good grasp on LLM agents, you need to read this. Almost everything you think you know is almost certainly out of date.
 

## Intro to agents

You'll often here Joe and I describe an agent as "just a prompt and a collection of tools".  To some that seems like an oversimplification but at the end of the day that's really all it is. Every model vendor has a web API to call that accepts a JSON payload.  Every single LLM backed agent product on the plant uses the same set of vendor APIs using the same JSON payload. We're all just sending a prompt, a collection of tools and, in the case of interactive agents, a list of all the chat messages each time.

> Everything an agent does is the result of us making an API call with our JSON payload and getting a result back.  Somtimes that result is "Here's a message for the user in response to their input".  Other times that result is "Hey, you know that tool you told me about?  Can you call it with these parameters and tell me the result"?


### What is a "prompt"

Most people familiar with LLMs are at least generally aware of what a prompt is, there's countless prompt guides, prompt lists, prompt templates, etc out there after all. In general a prompt is an input to an LLM either as audio or text and both with or without images.  What most people think of as a prompt is something they type/paste into ChatGPT or Copilot. 

This level of prompt is known as the "user prompt".  Any prompt provided at this level must compete with the prompt provided at the next tier up "developer", which in turn must compete with the prompt provided at the platform level. The final input into the LLM on the API side is one big ole block of text that begins with the platform prompt, then the developer prompt, then the information about all the tools, then finally the user prompt which is labeled for the model as the "user message".

> **Note:** The terms "developer prompt" and "system prompt" are interchangeable, however the vendors have all added a layer above "system" at this point. Open AI made it clear with a change in terminology making it clear THEY had final control and I've followed suit. Anthropic has not changed terminology yet, but they **have** added a layer about system.

Most negative impressions of LLMs and agents stems from the fact that they've only ever used consumer/business facing interfaces. The developer prompts of those systems are packed with rules and guidelines, anything *you* tell it to do comes second. With agents, the developer prompt is often quite large and only a portion of it contains the part most people would consider the prompt. 

> The **context window** defines how much information a given model can work with or output. What's important to understand is that this does not act like memory, disk space or the like even though it **seems** to.  On one level it is just that, a limit on how much information can go in before the model runs out of space. That only tells PART of the story. The more information in the context window there is that _doesn't_ contribute to the output of the model **hinders** the output of the model.  The more rules there are to follow, the less likely they'll _all_ be followed.  The farther from the top of the prompt something is, the more likely it is to be ignored. 
> 
> _With all that competition for space and attention, guess who comes LAST?_

#### Agent instructions

The specifics what makes any given agent's instructions is situational however, they're all, _or should be_, comprised of the same basic building blocks:

**Preamble / establishing clause**: This is a couple of sentences or short paragraph at the very top that serves to "prime the pump" so to speak for what comes after. Think of it as a lens over the glasses of the LLM, it changes how the model "sees" and more importantly interprets everything that follows.  For example, this is the opening for an agent that was recently built for a client:
> You are Aria, a C# Solution Architect who specializes in designing  clean, modern, maintainable architectures that showcase best practices while remaining straightforward and implementable. You transform detailed requirements into elegant technical solutions that impress through quality, not complexity.
 
**Context Information**: This is information the agent should "know" at all times in order to help it do it's job and /or pointers to where to find information it might need for a given task.  Think of it as a cheat sheet for the agent giving it the lay of the land. This is your best way to stave off a whole host of potential issues by cluing the agents in on critical pieces of information.
> **Be specific when providing context**, for example don't say we use "Windows" when you mean "Windows 11 Pro" unless there's no chance that matters.
> 
> One VERY common complaint developers have about LLMs is that they'll use outdated syntax for libraries.  A simple "We are using library X version Y, check your syntax" is enough to correct that if the model has been trained on that version. "We are using library X, version Y which has the following breaking changes:" when it hasn't been. 

**Rules / Guidelines**: These lay out the standards for how things are done. For any given task, the models have been trained on a variety of ways to accomplish it, **most** of them not great, many of them not good. This is where you steer the model towards the great training data and away from the bad.
> These rules and guidelines are where some of the "art" of agent instruction can start to come into play. Two sets of rules that "say the same thing" but are phrased differently can have different results. Sloppy, imprecise language leads to sloppy imprecise results but too much specificity can also lead to bade results.  The "art" is in learning how to be both precise and concise, adding instructions to correct "bad behavior", removing others that aren't as necessary. 

##### Formatting, structure and congruity 
Because models are trained on human communications, they can infer meaning from structure, formatting, word choice, capitalization, often in very subtle and non-obvious ways. We can use this to our advantage when creating our agent instructions. Agent instructions should always be in Markdown format and the whole thing should be laid out using header "levels" to group related things.  Use emphasis, captivation and other markup to "draw the eye" of the agent to ceratin key parts of the prompt. These are the first few headers for one of my "orchestrator" agents:

```markdown
# CRITICAL MUST FOLLOW: Sequential Workflow Orchestration Framework
## Core Orchestration Principles
## Clone Delegation Discipline Framework
### Mandatory Clone Delegation Rules
# CRITICAL MUST FOLLOW: Quality Gate Implementation
```
 This "clone" stuff is very new, and there were many speed bumps along the way. One issue would rear it's head now and then seemingly no matter what I tried. Every so often one of the clones would get confused and think it was the original aka "prime".  The primes are supposed to delegate work to clones, but clones are not given the tool to makes clones of themselves.  A confused clone would see that it didn't have the ability to create clones, and follow the procedure for such a case and bring everything to a screeching halt. I tried all sorts of notifications and callouts in the system prompt, I even prefaced the input from the prime agent with `Your prime agent is making the following request of you` and still occasionally had issues. The thing that finally stopped it?  `Your prime agent is making the following request of YOU (the clone)` zero room for confusion.

### What is a "tool"?

Any normal method can be a tool for an agent provided it returns a string or something that can be converted to a string.  What turns a method into a tool for an agent is simply the fact that we tell them about it. For each method that we want to agent to be able to use, we include a bit of data in that JSON we send along that says "Here's a tool named `foo` it accepts the following parameters. You can use this tool to do X."  The basic mechanics of it are so simple, any developer that's used/written a REST API could write their first tool in hours.

> This is one of those areas where there's a lot of nuance and "art" that can make the difference between "meh" and outstanding. Tool developers that take the time to really polish their tools to minimize the number of "tokens" it takes a model to accomplish a task.  Every token saved in a tool call is a token that can be used somewhere else. 

When the agent wants to use a tool, instead of returning a message it will return one or more "tool call" models which just an ID to uniquely ID the tool call, the name of the tool, and a JSON dictionary with the arguments and the values it would like to use for them.  The runtime behind the agent will call the tool, grab the result then package that up into a "tool result" model that contains the ID of the tool call and a string with the result. The model, looks at the result and decides what it wants to do next.  Easy peasy, except, the chat log...

Each time an agent runtime makes a call to one of the vendor APIs they must send the entirety of the chat history along with it.  Every tool call and result gets sent up, consumes context space and competes for attention with the model, even when it was the wrong thing to do. Each time around the loop is the first time as far as the model is concerned. It has to start at the top, and work it's way down it's time till it reaches the bottom sees the tokens `Assistant: ` and starts predicting what the next token should be.

> Tools designed to minimize the number of token it takes to use them and the number of tokens they leave in the chat log can have an outsized impact on the quality of output you get, the runtime of your agent before needing a fresh session and the **costs** involved.  When everyone has access to the same models, and is under the same constraints as you are you've got to be able to do it faster/cheaper/better or all three to stand out. 

#### MCP

Some folks might have heard about Model Context Protocol from Anthropic which provides a way to provide tools to agents. Both vendors support them natively in their APIs now.  My personal stance on MCP is that if it's all you have available to you it's great.  If you have options however, there are better options. It's not that MCP is bad, it's that it's not designed for complex, long running, "production grade" processes like agents are capable of.  It was designed at time where an agent might make a "few" tool calls in an interaction, not hundreds.

## Agent C agents

The Agent C framework is vendor agnostic, it works with all major models and up until March of this year they were fairly interchangeable. Each had their strengths and weaknesses and for many things it simply did not matter which one you used. That is no longer the case.  I used to say "it really doesn't matter what framework you're using most of the time", I don't anymore. In a **very** short amount of time the Agent C framework has gone from a thin abstraction layer over the vendor APIs to make tool using agents easier to create to something resembling a full-on agent orchestration framework, without most of the complexity that entails.

Mid-March I spent some time exploring how to leverage Claude 3.7 effectively as an agent. It was shockingly clever, and fantastic at working around problems however it tried to do too much and would often get "lost" trying to do so.  I worked out new protocols and processes that help a lot, but never to the degree I could count on it for real work. I ultimately came to the conclusion that it just wasn't there yet. I told Joe in my update to him something to the effect of "once they can reign it in some it'll be amazing" and then I set 3.7 aside.

The *very* next day, Anthropic told us all about a `Think` "tool" they had for Claude, the first version of what they now call "interleaved thinking". I read their article on the 20th, the whole time thinking "I wonder when they'll actually release that because it sounds like it'd solve all my problems with Claude". On the 21st, a random Redditor provided me, and many others the key piece of information about that `Think` tool, how it worked. The `Think` tool isn't a tool at all, but a clever way to "abuse" the tool call mechanisms by providing a "tool" that does nothing, except hold the reasoning output of the models in the chat log for it to see later in followup calls.  A small change with huge impact.

Just adding that non-tool tool to the agent I had been working with made it immediately better. Before my first test run was over I was sending Joe all-caps and expletive laden messages to call me ASAP. In the days that followed I made a flurry of small changes to things, to better take advantage of this.  For example, explicit callouts to make use of the `Think` tool were. Planning, tracking and reporting protocols were written and around the `Think` tool, in order to keep the agents on-plan, etc. What _those_ agents could do was science fiction level stuff to most people, and that was just the beginning.

### What makes Agent C agents special?
Interleaved thinking is the thing that makes this style of agent possible, but that's not something I invented. I **am** one of the first to leverage it so effectively but that's just a head start, and I'm certain I'm not the only one who dropped everything and started retooling around this.  What makes Agent C agents special is the patterns and design philosophy behind it, it's tools and interactions.

The fundamental difference in my approach to agent creation and LLMs in general boils down to thinking about them as a sort of "ditzy but really smart" intern instead of treating like software.  When an LLM makes a mistake on a task, I go over the instructions I've given it, the things I've said to it the WORDS I used when I said those things, etc to understand how **I** failed to be clear/explicit enough.  Instead of trying to build agents that did a lot (foreshadowing) I focused on creating agents that were really good and really _reliable_ for specific tasks and making them reusable by other agents as tools. 

Another difference is in my approach to agent tools.  I have spent a enormous amount watching agents work with the tools I've built for one task or another. I'm constantly monitoring the tools agents use, and how they use them. I keep an eye out for things that take an agent multiple steps to do, that I could turn into one.  I look for ways to reduce the number of tokens it takes agents to USE that tool.

#### The Workspace toolset
The Workspace toolset is a foundational toolset in Agent C it is one of the few tools almost every agent should have equipped. This toolset provides an agent with access to one or more workspaces.  A Workspace is an abstraction layer over a filesystem of some sort and currently come in three flavors:
- Local File System - A directory on the disk of the machine the Agent C API is running on.
- Amazon S3 - An S3 bucket
- Azure Blob - Bet you'll never guess...

The Workspace toolset is able to provide additional layers of functionality that work with all Workspace types by making use of lower level code in the those Workspaces that's not exposed to agents. it contains a large number of tools to allow agents to be _surgical_ with their reads and updtes of files.  In addition, many other tools make use of the workspace tools for their own file system needs and for sharing information.

The exact _how_ of configuring local workspaces is about to change, so I won't go into it in detail her.  If anyone needs help figuring it out say something in the Agent C Teams channel and someone can help until then. When using the Docker compose version you are limited to creating workspaces out of folders that exist below your desktop, documents and downloads folders.

Think of a workspace as a "shared drive" with remote colleges. All the interaction patterns with agents revolve around this toolset. There's a guide to this tool in the docs in the repo that's aimed at users to help them understand ways agents might be able to use this tool, it's worth a read. 

##### Key info
- You refer to files and folders within workspaces using a UNC style path in the form of `//[workspace]/path/to/file`
  - For example `//desktop/meeting transcripts/2025-07-04.txt` for a folder on your desktop if using Docker. 
- Regardless of you enter their names in the config workspace names are always lower case. Using an uppercase `D` won't matter nine times out of ten. But "dumber" models won't correct it and just say it doesn't exist.
- A  `.agentcingore` file may be used to hide files / folders from agents.  It follows the same rules af a git ignore file does. 
- Agents will create a folder with the name `.scratch` in the root folder of the workspace.  This is called "the scratchpad" and can be referred to as such with the agents. Telling the agent to "write that to the scratchpad" is easy consistent language to use that works regardless  
- `.agent_c.meta.yml` in the root folder of a workspace contains the saved form of the the "workspace metadata" which is a key/value store available to be used as shared memory by agents as well as by other tools such as:

#### The Workspace Planning toolset
This tool allows agents to create, follow and track plans in a structured 
