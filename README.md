# Agent C Framework

## What is Agent C?

Agent C is a minimalist framework for building interactive, tool-using AI agents. Created by Centric Consulting, it provides a runtime environment that handles:

- Streaming responses
- Parallel tool execution
- Asynchronous operations
- Multiple ways to consume AI content (callbacks or async generators)

All components are designed to be non-blocking and fully asynchronous.

## Getting Started

Choose the setup path that best matches your needs:

### 🚀 Quick Start (No Development)

If you just want to use Agent C without modifying code:

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

### 💻 Developer Setup

#### Prerequisites

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

**Option A: Automatic Setup (Recommended for Beginners)**

Run the setup script for your platform:

```bash
# On Windows
.\scripts\initial_setup.bat

# On Mac/Linux
./scripts/initial_setup.sh
```

**Option B: Manual Setup**

If you prefer to understand each step or if the script doesn't work:

1. Create a virtual environment:
   ```bash
   python -m venv venv
   ```

2. Activate the virtual environment:
   ```bash
   # On Windows
   venv\Scripts\activate.bat

   # On Mac/Linux
   source venv/bin/activate
   ```

3. Install the project packages:
   ```bash
   cd src
   pip install -e agent_c_core
   pip install -e agent_c_tools
   pip install -e agent_c_reference_apps
   pip install -e test\agent_c_tools
   ```

> **Note**: The `-e` flag installs packages in "editable" mode, so your code changes are immediately available without reinstalling.

#### Step 3: Get an OpenAI API Key

Agent C requires an OpenAI API key to function:

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

You can run Agent C in two modes:

### Command Line Interface

```bash
agent_c-cli [options]
```

### Web Interface

```bash
agent_c-web [options]
```

### Common Command Line Options

- `--model [model_name]`: Override the default AI model
- `--prompt_file [/path/to/file]`: Supply a custom persona prompt file
- `--userid [user_id]`: Set a specific user ID for the chat session

### CLI Commands (Available Within the CLI App)

When using the CLI version, you can use these special commands:

- `!exit` or `!!!`: Exit the app
- `!keep`: Mark the current session to be saved after exiting
- `!!!!`: Exit without deleting the current session
- `!compact`: Reduce the message history to save tokens
- `!?`: Show all available commands

The up/down arrow keys let you navigate through your command history.

## Configuration File

When you first run Agent C, it will create a configuration file (similar to `.env`) that needs to be modified before Agent C will function properly. This file contains API keys and other settings.

```
# Location of configuration file:
# Windows: %USERPROFILE%\.agent_c\agent_c.config
# Mac/Linux: ~/.agent_c/agent_c.config
```

### Essential Configuration

These settings are required for basic functionality:

```
# Required: OpenAI API key (you MUST set this)
OPENAI_API_KEY=your-openai-api-key-here

# Required: Set your user ID (default is "default")
CLI_CHAT_USER_ID=YourName

# Required: Zep API key for memory management
# Create a free Zep Cloud account at: https://app.getzep.com/api/auth/register
# Zep API keys are specific to a project. Visit "Project Settings" in the Zep dashboard to manage your keys.
ZEP_API_KEY=your-zep-api-key-here
```

### Optional Configuration

The following sections allow additional functionality if you have these services:

```
# Optional: Claude/Anthropic API key (if you want to use Claude models)
#ANTHROPIC_API_KEY=your-anthropic-key-here

# Optional: Azure OpenAI (uncomment if using Azure instead of OpenAI)
#AZURE_OPENAI_API_KEY=your-azure-key-here
#AZURE_OPENAI_ENDPOINT=https://your-resource.openai.azure.com/
#AZURE_OPENAI_API_VERSION=2024-03-01-preview

# Optional: If you're using Zep CE locally instead of Zep Cloud
#ZEP_CE_KEY=your-zep-ce-key-here
#ZEP_URL=http://localhost:8001

# Optional: Debug information
ENHANCED_DEBUG_INFO=False

# Optional: API keys for various tools
# Uncomment and add keys for tools you want to use
#SERPAPI_API_KEY=your-serpapi-key-here        # For web searches
#SEC_API_KEY=your-sec-api-key-here            # For SEC filings access
#TAVILI_API_KEY=your-tavili-key-here          # For Tavili integration
#NEWSAPI_API_KEY=your-newsapi-key-here        # For news access
```

### Example Configuration

A minimal configuration to get started would look like:

```
OPENAI_API_KEY=sk-abcd1234567890abcdefg
CLI_CHAT_USER_ID=JaneDoe
ZEP_API_KEY=zep-cloud-api-key-12345abcde
```

After updating your configuration file, run the startup script again to launch Agent C with your settings.

## Working with Files

Agent C creates a `LocalStorageWorkspace` called `project` that gives the agent access to files in the project root. Use the `temp` folder for files you want the agent to work with.

To reference files, use: "Could you load temp/data.csv from the project workspace and..."

## Personas

The `personas` folder contains various persona files that give Agent C different capabilities:

- Use them with the `--prompt_file` option
- For example, the `py_polish.md` persona specializes in cleaning up Python code, adding type hints, and improving documentation

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