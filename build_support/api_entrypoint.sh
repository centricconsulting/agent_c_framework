#!/bin/bash
# entrypoint.sh

# Fix ownership of mounted volumes
chown -R agent_c:agent_c /app/agent_c_config

# Drop to agent_c user and run the server
exec gosu agent_c python -m uvicorn agent_c_api.main:app --host 0.0.0.0 --port 8000 --log-level info