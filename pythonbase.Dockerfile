# Base Dockerfile for agent_c services
FROM python:3.12-trixie

# Set environment variables
ENV PYTHONPATH=/app
ENV PYTHONDONTWRITEBYTECODE=1
ENV PYTHONUNBUFFERED=1
ENV TERM=xterm-256color
ENV CHROME_BIN=/usr/bin/chromium
ENV CHROMEDRIVER_PATH=/usr/bin/chromium-driver

# Install system dependencies
RUN apt-get update && apt-get install -y \
    curl \
    build-essential \
    git \
    portaudio19-dev \
    python3-dev \
    libasound2-dev \
    ffmpeg \
    nodejs \
    npm \
    chromium \
    chromium-driver \
    cmake \
    gosu \
    && apt-get clean && rm -rf /var/lib/apt/lists/*

# Create a non-root user
RUN adduser --system --uid 1001 --group --home /home/agent_c agent_c

# Install Rust for agent_c user
RUN curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | \
    CARGO_HOME=/home/agent_c/.cargo RUSTUP_HOME=/home/agent_c/.rustup sh -s -- -y --no-modify-path

# Fix ownership
RUN chown -R agent_c:agent_c /home/agent_c/.cargo /home/agent_c/.rustup
ENV PATH="/home/agent_c/.cargo/bin:${PATH}"

# Install pnpm
RUN npm install -g pnpm@9
ADD  build_support/requirements.txt /tmp/
RUN mkdir -p /app && chown -R agent_c:agent_c /app
WORKDIR /app

# Copy requirements file and set correct ownership
RUN chown agent_c:agent_c /tmp/requirements.txt

# Switch to the non-root user
USER agent_c

# Install .NET SDK
RUN curl -L https://dot.net/v1/dotnet-install.sh -o dotnet-install.sh
RUN chmod +x dotnet-install.sh
RUN ./dotnet-install.sh --version latest --install-dir /home/agent_c/.dotnet
RUN rm dotnet-install.sh
ENV DOTNET_ROOT="/home/agent_c/.dotnet"
ENV PATH="$DOTNET_ROOT:$DOTNET_ROOT/tools:$PATH"

# Set up virtual environment as the non-root user
RUN python -m venv /home/agent_c/.venv
ENV PATH="/home/agent_c/.venv/bin:/home/agent_c/.cargo/bin:$PATH"

# Install Python dependencies inside the virtual environment
# Install packages with Rust dependencies first
RUN pip install --no-cache-dir --upgrade pip && \
    pip install --no-cache-dir tiktoken

# Then install the rest of the requirements
RUN pip install --no-cache-dir -r /tmp/requirements.txt || \
    pip install --no-cache-dir --no-binary=pyarrow -r /tmp/requirements.txt

# Set the entrypoint to the Python interpreter
#ENTRYPOINT ["python"]
