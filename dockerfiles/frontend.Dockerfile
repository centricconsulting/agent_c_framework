FROM node:20-slim

# Set working directory
WORKDIR /app

# Install dependencies
RUN apt-get update && apt-get install -y \
    git \
    && apt-get clean && rm -rf /var/lib/apt/lists/*

# Copy package files
COPY src/agent_c_api_ui/agent_c_react_client/package*.json ./

# Install dependencies
RUN npm install

# Copy the rest of the frontend code
COPY src/agent_c_api_ui/agent_c_react_client/ ./

# Build for production or use dev mode
ARG NODE_ENV=development
ENV NODE_ENV=${NODE_ENV}

# Set environment variables
# These can be overridden at runtime
ENV VITE_API_URL=http://localhost:8000/api/v1
ENV VITE_RAG_API_URL=http://localhost:8000/api/v1

# Expose the Vite dev server port
EXPOSE 5173

# Command to run development server
CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0"]