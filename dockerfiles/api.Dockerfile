FROM ghcr.io/centricconsulting/agent_c_python_base

# Set working directory and create required directories
WORKDIR /app
RUN mkdir -p /app/images
RUN mkdir -p /app/src/agent_c_api_ui
RUN mkdir -p /app/workspaces/desktop
RUN mkdir -p /app/workspaces/downloads
RUN mkdir -p /app/workspaces/documents



# Copy source code first
COPY src/agent_c_core ./src/agent_c_core
COPY src/agent_c_tools ./src/agent_c_tools
COPY src/agent_c_api_ui/agent_c_api ./src/agent_c_api_ui/agent_c_api
COPY compose_workspaces.json /app/.local_workspaces.json

# Copy in the personas - right now we mount in docker-compose
COPY personas /app/personas

# Upgrade pip
RUN pip install --no-cache-dir --upgrade pip
USER root
RUN chown -R agent_c:agent_c /app
USER agent_c
# Install Python dependencies
WORKDIR /app/src
RUN pip install  -e agent_c_core \
    && pip install -e agent_c_tools \
    && pip install -e agent_c_api_ui/agent_c_api

WORKDIR /app/test
RUN pip install -e agent_c_tools

# Return to the app's root
WORKDIR /app

# Set environment variables
ENV ENHANCED_DEBUG_INFO="True"
ENV ENVIRONMENT="LOCAL_DEV"
ENV CLI_CHAT_USER_ID="Taytay"
ENV DALLE_IMAGE_SAVE_FOLDER="/app/images"



# Expose the FastAPI server port
EXPOSE 8000

# Command to run the API
CMD ["python", "-m", "uvicorn", "agent_c_api.main:app", "--host", "0.0.0.0", "--port", "8000", "--log-level", "info"]