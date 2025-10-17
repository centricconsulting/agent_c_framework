#!/usr/bin/env python3
"""
Simple wrapper to run the users CLI from anywhere in the project.
"""
import sys
from pathlib import Path

# Add the correct path for agent_c_api imports
api_src = Path(__file__).parent / "src" / "agent_c_api"
if api_src.exists():
    sys.path.insert(0, str(api_src.parent))

# Import and run the CLI
from agent_c_api.cli.users import main

if __name__ == "__main__":
    main()