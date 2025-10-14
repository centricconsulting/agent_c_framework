# WARNING WIP
This is a ReactJS + FastAPI app that is a work in progress.  It is may not be ready for production use. It was developed by @justjoehere, who is not a developer.
It was also partially assisted by ChatGPT 4o, o3-mini-high, and Claude.

# Installation
## Backend
If you're loading from agent_c and using the scripts (setup, intall_deps) you can skip the backend setup.  
- Ensure agent_c, agent_c_tools, etc. are installed or available in the environment. See [agent_c/README.md](README.md) for more details.
- Run  
  - [initial_setup.bat](..%2F..%2F..%2F..%2F..%2Fscripts%2Finitial_setup.bat)
  - [install_deps.bat](..%2F..%2F..%2F..%2F..%2Fscripts%2Finstall_deps.bat)
**Alternate Setup** - If you're loading from the react_fastapi folder, you'll need to do the following: 
- activate your `.venv`
- `cd to src/agent_c_reference_apps/src/agent_c_reference_apps/react_fastapi/backend/backend_app`
- `pip install -e requirements.txt`
- To run the server
- `uvicorn --host 0.0.0.0 --port 8000 --log-level info`
**If debugging via Pycharm**
In your run configs, set the following
- application file: `C:\<path>\agent_c\src\agent_c_reference_apps\src\agent_c_reference_apps\react_fastapi\backend\main.py`
- Uvicorn options: `--host 0.0.0.0 --port 8000 --log-level info`
- Working Directory: `C:\<path>\agent_c`
****NOTE**** 
- personas are loaded from the root repo \personas - I didn't move them.  So I don't know if running the backend from the command line will bomb or not.

## Front End
- Open a new command window, navigate to `src/agent_c_reference_apps/src/agent_c_reference_apps/react_fastapi/frontend`
- Run `npm install`
- Then `run npm run dev`
- Open browser to http://localhost:5173/

# Working Directory
When running the FastAPI server, the working directory `\agent_c` - otherwise personas won't load
This is important to know when working with relative paths in your code.

# Usage
- Navigate to the URL the React dev server points to (e.g., http://localhost:5173).
- The front end should request a new session_id from /initialize.
- You can then type in messages, choose persona, adjust temperature, select model, and see streaming replies from the backend.
