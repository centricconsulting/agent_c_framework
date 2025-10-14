# Multi-stage Dockerfile for Agent C Realtime Client SDK Demo App

# Stage 1: Base dependencies
FROM node:20-alpine AS base
RUN apk add --no-cache libc6-compat git openssh
WORKDIR /app

# Install pnpm globally
RUN npm install -g pnpm@9

# Stage 2: Install dependencies
FROM base AS deps
WORKDIR /app

# Copy workspace configuration files
COPY package.json pnpm-lock.yaml pnpm-workspace.yaml ./
COPY lerna.json ./

# Copy all package.json files for workspace packages
COPY packages/core/package.json ./packages/core/
COPY packages/react/package.json ./packages/react/
COPY packages/ui/package.json ./packages/ui/
COPY packages/demo/package.json ./packages/demo/

# Install dependencies using frozen lockfile
RUN pnpm install

# Stage 3: Build all packages from source
FROM base AS builder
WORKDIR /app

# Copy node_modules from deps stage
COPY --from=deps /app/node_modules ./node_modules
COPY --from=deps /app/packages/core/node_modules ./packages/core/node_modules
COPY --from=deps /app/packages/react/node_modules ./packages/react/node_modules
COPY --from=deps /app/packages/ui/node_modules ./packages/ui/node_modules
COPY --from=deps /app/packages/demo/node_modules ./packages/demo/node_modules

# Copy workspace configuration
COPY package.json pnpm-lock.yaml pnpm-workspace.yaml ./
COPY lerna.json tsconfig.json ./
COPY eslint.config.mts ./

# Copy source code for all packages
COPY packages/core ./packages/core
COPY packages/react ./packages/react
COPY packages/ui ./packages/ui
COPY packages/demo ./packages/demo

# Build packages in dependency order
# 1. Build core package first (no internal dependencies)
RUN cd packages/core && pnpm build

# 2. Build react package (depends on core)
RUN cd packages/react && pnpm build

# 3. Build ui package (depends on core and react)
RUN cd packages/ui && pnpm build

# Remove .next directory if it was accidentally included from builder
RUN rm -rf ./packages/demo/.next
RUN cd packages/demo && pnpm build

# Stage 4: Development runner
FROM node:20-alpine AS runner
WORKDIR /app

# Install git which pnpm needs
RUN apk add --no-cache git

# Add non-root user for security
RUN addgroup --system --gid 1001 agent_c
RUN adduser --system --uid 1001 agent_c

# Set NODE_ENV to development for dev mode
ENV NODE_ENV=development
ENV PORT=5173
ENV HOSTNAME="0.0.0.0"

# Install pnpm in runner for package resolution
RUN npm install -g pnpm@9

# Copy workspace configuration files
COPY --from=builder /app/package.json ./
COPY --from=builder /app/pnpm-lock.yaml ./
COPY --from=builder /app/pnpm-workspace.yaml ./
COPY --from=builder /app/lerna.json ./
COPY --from=builder /app/tsconfig.json ./
COPY --from=builder /app/eslint.config.mts ./

# Copy ALL source files for core package
COPY --from=builder --chown=agent_c:agent_c /app/packages/core ./packages/core

# Copy ALL source files for react package
COPY --from=builder --chown=agent_c:agent_c /app/packages/react ./packages/react

# Copy ALL source files for ui package
COPY --from=builder --chown=agent_c:agent_c /app/packages/ui ./packages/ui

# Copy ALL demo app files from builder (excluding .next if it exists)
# This includes all source, config files, certificates, etc.
COPY --from=builder --chown=agent_c:agent_c /app/packages/demo ./packages/demo
COPY ./packages/demo/.env.example ./packages/demo/.env
# Install ALL dependencies including devDependencies for development mode
# Remove --ignore-scripts to allow postinstall scripts to run (needed for some packages)
# This will install all dependencies and set up workspace links
RUN pnpm install --frozen-lockfile

# Switch to non-root user
USER agent_c

# Expose the port Next.js runs on
EXPOSE 5173

# Change to demo directory and start the application in development mode
WORKDIR /app/packages/demo

# Run in development mode using the docker-specific script
CMD ["pnpm", "start"]