#!/bin/bash
# entrypoint.sh

# Fix ownership of mounted volumes
chown -R agent_c:agent_c /app/agent_c_config

# Drop to agent_c user and run the actual command
exec gosu agent_c "$@"