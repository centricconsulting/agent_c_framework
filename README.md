# Agent C Framework

## What is Agent C?

Agent C is a framework for building interactive, AI agents that make use of advanced planning and delegation patterns to accomplish much larger workloads per chat session than most. (to say the least) 

Created by Donavan Stanly, a Centric Consulting architect, it provides a runtime environment that provides:

- Vendor flexibility
  - Anthropic (base and via AWS Bedrock)
    - Note: Claude 4 Sonnet is FAR and away the top model for agentic work requiring problem solving.
  - Open AI (base and via Azure)
  - Google Gemini
- Stateless Agent Runtime
  - The agent runtime and tools maintain no state beyond internal caching.
  - All necessary context is provided on a per-call basis to the runtime / tools
  - Designed to allow decoupling the runtime from the application layer.
- Runtime components are designed to be non-blocking and fully asynchronous. 
- Event stream based interactions
  - The agent runtime and it's tools communicate with the application layer via events.
  - All events contain routing information to allow the application layer to route events based on user ID and/or session ID
  - This designed to be "pipe agnostic".  You supply a callback handler to receive events from the runtime and shove them in whatever pipe you want.
- Best-in-breed agent tools
  - Our tools are optimized to reduce token consumption by agents.
    - For example, MCP provides a file system tool with read, write, list operations.
      - Agent C provides a Workspace tool that provides those basic features plus
        - read_lines, grep, replace_strings, tree, examine_code and metadata
        - Support multiple workspaces
        - Allows you control which workspaces which agents can see
        - Allows you to control read/write status on a per agent basis.
        - AND it supports more AWS S3 and Azure as a backing stores
    - Other frameworks allow an agent to fetch a web page and feed it HTML wasting tokens.
      - Our web tool ALLOWS for HTML to be retrieved but by default content is ran through the "Readability" algorithm and then converted to Markdown before handing to the agent.
      - It allows for saving of content to any workspace.
        - And FORCES it if the content is too large.
    - Our tools can interconnect and build on each other
      - Tools can declare other tools as dependencies and make use of them at runtime
        - Tools that are loaded in this way are NOT shown to the agents, only their own list of tools is shown to them.
      - Many *many* tools make use of the Workspace tool as a place to store / share information.
    - Other frameworks return JSON to agents, again wasting precious tokens
      - Our tools use YAML as a serialization mechanism which saves tokens, time and money.
    - Our tools can bypass the agent and pass information back to the user.  Again saving time, tokens and money:
      - Media files
      - HTML
      - Markdown
      - etc
- Unique interaction model
  - Our agent to agent interaction model is born out of battle tested "agent as code" then later "agent as tool" technologies.  In Agent C, an agent is just a prompt and a set of tools.  It's easy to hide an agent behind a any generic method for someone to call, and because of that it's easy to make agent into a tool for another agent.
    - Agent C agents can be provided with advanced delegation tools:
      - **Agent Clone** allows an agent to make a copy of itself with an empty context window and task it with completing an objective.
      - **Agent Team** allows for the creation of agent teams, led by a supervisor agent under the direction of the user.  The user essentially becomes "AI Management".
      - **Agent Assist** allows agents to make use of special purpose non-interactive agents.
        - Digging through a bunch of company blogs for info you care about can brun a LOT of tokens. So agent C includes an example "news pro" assistant that can be used by any agent on your behalf. 
  - The user/agent interaction model is based around the "paired programming" model of development. Even for non-coding agents.
    - As the human side of the pair, your job is to provide oversight, verification of work, etc.
    - This becomes even more important when dealing with agent teams.
- Planning, tracking, reasoning
  - All work, done by agent C agents is first *planned*.
    - Our planning tool allows for hierarchical plans, with steps and step steps
      - Each step has explict process and context information
      - Steps can require signoff, by supervisor agents or by the human in the loop
      - NO top level step of any plan can be completed without human approval
        - But our agents make that easier by giving your a "validation and signoff package"
    - All plans include a "lesson learned" to prevent repeated mistakes.
  - At each step of the way our agents engage in reasoning, not just at the start of the interaction.  Explicit when:
    - Reading in a step of a plan.
    - Receiving information from a clone, team member or assistant agent.
    - Receiving information from an external source such as a file, tool or web page.
  - Every single step is tracked and shared memory updated along the way.
    - Resuming from a cold session is as simple as "bring yourself up to speed and tell me our next steps"
    - Providing supervision, even deep into an agent team, is as simple as hitting the "stop interaction" button and then sending: "I had to stop you because of X, please make AGENT_NAME aware of KEY_DETAIL and resume working"

## Getting Started

Choose the setup path that best matches your needs:

### 🚀 Quick Start (Recommended for Most Users)

The Web UI is the recommended way to use Agent C:

1. Install [Docker Desktop](https://www.docker.com/products/docker-desktop/) (or Rancher Desktop)
2. Run the appropriate startup script:
   - Windows: `dockerfiles\start_agent_c.bat`
   - Mac/Linux: `dockerfiles/start_agent_c.sh`
3. On first run, a configuration file will be created in:
   - Windows: `%USERPROFILE%\.agent_c`
   - Mac/Linux: `~/.agent_c`
4. Edit this configuration file to add your API keys and set your username (see [Configuration File](#configuration-file) below)
5. Run the startup script again
6. Agent C will open automatically in your browser
7. Bookmark this page to access Agent C until you remove the Docker containers

For detailed information about the Web UI and its features, see the [Web UI Documentation](docs/web_ui_README.md).

### 💻 Developer Setup

#### Prerequisites

**EXPAND THE ONE FOR YOUR OS AND READ**
Notice: ALL platforms MUST use Python 3.12.x NOT 3.13 not 3.10

If you are NOT a Python developer who is used to working with Python virtual environments you are STRONGLY urged to use PyCharm as an IDE until you've gotten more comfortable with Python. Almost every single developer that has had issues getting started was using VSCode. The only exceptions have been those using PyCharm, but having the wrong version of Python. 

<details>
<summary><b>Windows Prerequisites</b> (click to expand)</summary>

- [Git](https://git-scm.com/downloads/win)
- [Python 3.12+](https://www.python.org/downloads/release/python-3129/)
- A Python IDE like [PyCharm](https://www.jetbrains.com/pycharm/download/) (Community Edition is free)
- [Microsoft Visual C++ Build Tools](https://visualstudio.microsoft.com/downloads/) (Be sure to check the "C++ development" option)
  - Or run: `winget install Microsoft.VisualStudio.2022.BuildTools`
- [Rust](https://www.rust-lang.org/tools/install)
- [Node.js](https://nodejs.org/en/download)
- [ffmpeg](https://ffmpeg.org/download.html#build-windows)
- Optional: [pyenv](https://github.com/pyenv-win/pyenv-win) (for managing Python versions)

</details>

<details>
<summary><b>Mac/Linux Prerequisites</b> (click to expand)</summary>

1. Install Xcode command line tools:
   ```bash
   xcode-select --install
   ```

2. Install Homebrew (if not already installed):
   ```bash
   /bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
   ```

3. Install required packages:
   ```bash
   brew install python@3.12 pyenv rust node ffmpeg
   ```

4. Optional: Install development tools:
   ```bash
   brew install visual-studio-code
   brew install --cask pycharm-ce
   ```
</details>

#### Step 1: Clone the Repository

```bash
git clone https://github.com/centricconsulting/agent_c_framework.git
cd agent_c_framework
```

#### Step 2: Set Up Your Python Environment

Python projects typically use "virtual environments" to keep dependencies isolated. This prevents conflicts between different projects on your system.

**Use the Setup Scripts (Recommended)**

Run the setup script for your platform - these scripts are kept up-to-date with the latest requirements:

```bash
# On Windows
.\scripts\initial_setup.bat

# On Mac/Linux
./scripts/initial_setup.sh
```

If you need to understand what dependencies are being installed, please review these setup scripts to see the exact packages and versions being used.

#### Step 3: Get a vendor API Key

Agent C requires at least one API key from a major vendor to function.

**NOTE: If you must choose only one, choose Anthropic.  Right now, NOTHING comes close to Claude for Agentic work.**

**Anthropic**
Follow the [Get Started With Claude](https://docs.anthropic.com/en/docs/get-started) guide 

> **Important**: Until you have reached "[Tier 3](https://docs.anthropic.com/en/api/rate-limits#requirements-to-advance-tier)" with Anthropic you will receive warnings in the chat stream about hitting your rate limit. The runtime will "back off" and slow down for an increasing amount of time until enough time has passed to allow number of tokens you have through.  Expect delays of up to 32 seconds when on Tier 2 or below.

**Open AI**
1. Sign up at the [OpenAI Platform](https://platform.openai.com/signup)
2. Navigate to [API Keys](https://platform.openai.com/settings/organization/api-keys)
3. Create a new API key
4. Prepay/deposit an amount (e.g., $20 USD) for usage credits

> **Important**: A ChatGPT Plus subscription is **not** sufficient - you need an actual API key.

#### Step 4: Configure Environment Variables

1. Copy the example environment file to create your own:
   ```bash
   cp example.env .env
   ```

2. Edit `.env` to add your OpenAI API key:
   ```
   OPENAI_API_KEY=your-api-key-here
   ```

## Running Agent C

### Casual: Docker mode

The simple way to run Agent C is through the Web UI using Docker. You will have fewer options for creating local workspaces in this mode.

```bash
# On Windows
dockerfiles\start_agent_c.bat

# On Mac/Linux
./dockerfiles/start_agent_c.sh
```

This provides a full-featured experience with an intuitive interface for configuring and using agents. For detailed information about the Web UI features, see the [Web UI Documentation](docs/web_ui_README.md).


## Configuration File

When you first run Agent C, it will create a configuration file (similar to `.env`) that needs to be modified before Agent C will function properly. This file contains API keys and other settings.

```
# Location of configuration file:
# Windows: %USERPROFILE%\.agent_c\agent_c.config
# Mac/Linux: ~/.agent_c/agent_c.config
```


### Professional: Native

For development or to allow for mapping of ANY folder as a workspace (Docker makes that hard to do on the fly) you may run the API and React frontend directly on your machine with:

**Windows**
```commandline
start scripts\start_api.bat
start scripts\start_fe.bat
```

**OSX/Liunx
```bash
scripts\start_api.bat &
scripts\start_fe.bat &
```

### Essential Configuration

These settings are required for basic functionality:

```bash
ANTHROPIC_API_KEY=your-anthropic-key-here
```

### Optional Configuration

The following sections allow additional functionality if you have these services:

```bash
# Optional Vendor Keys 
OPENAI_API_KEY=your-openai-api-key-here

# Optional: Azure OpenAI (uncomment if using Azure instead of OpenAI)
AZURE_OPENAI_API_KEY=your-azure-key-here
AZURE_OPENAI_ENDPOINT=https://your-resource.openai.azure.com/
AZURE_OPENAI_API_VERSION=2024-03-01-preview

# Optional: AWS Storage
AWS_REGION_NAME=your-region-name
AWS_SECRET_ACCESS_KEY=your-secret-access-key
AWS_ACCESS_KEY_ID=your-access-key-id


# Optional: Debug information
ENHANCED_DEBUG_INFO=False

# Optional: API keys for various tools
# Uncomment and add keys for tools you want to use
SERPAPI_API_KEY=your-serpapi-key-here        # For web searches
SEC_API_KEY=your-sec-api-key-here            # For SEC filings access
TAVILI_API_KEY=your-tavili-key-here          # For Tavili integration
NEWSAPI_API_KEY=your-newsapi-key-here        # For news access
```


## Working with Files

Agent C creates a `LocalStorageWorkspace` called `project` that gives the agent access to files in the project root. Use the `temp` folder for files you want the agent to work with.

To reference files, use: "Could you load temp/data.csv from the project workspace and..."


## Additional Documentation

- Each submodule has its own README
- `docs/tools.md`: Information on creating new tools and using existing ones
- `docs/prompts.md`: Details on the prompt builder
- `docs/web_tool.md`: Explains the Web tool and content formatters

## License

This project is licensed under the Business Source License 1.1.
- **Licensor**: Centric Consulting
- **Licensed Work**: Agent C Framework
- **Change Date**: 2024-12-31
- **Additional Use Grant**: None

For details, see the [LICENSE](./LICENSE) file or visit the [BSL 1.1 website](https://mariadb.com/bsl11/).
