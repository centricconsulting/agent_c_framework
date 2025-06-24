FROM python:3.12-alpine

# Install system dependencies required for Python packages
# Install system dependencies
RUN apk update && apk add \
    gcc \
    g++ \
    musl-dev \
    python3-dev \
    libffi-dev \
    openssl-dev \
    cargo \
    jpeg-dev \
    zlib-dev \
    freetype-dev \
    lcms2-dev \
    openjpeg-dev \
    tiff-dev \
    tk-dev \
    tcl-dev



# Install commonly used Python packages
RUN pip install --no-cache-dir \
    requests \
    beautifulsoup4 \
    pandas \
    numpy \
    matplotlib \
    seaborn \
    scikit-learn \
    openpyxl \
    xlrd \
    python-dateutil \
    pytz \
    pyyaml \
    lxml \
    html5lib \
    pillow


# Set up directories
WORKDIR /workspace
RUN mkdir -p /input /output

# Set Python to unbuffered mode for better logging
ENV PYTHONUNBUFFERED=1

# Create a non-root user for better security
RUN adduser -D executor
RUN mkdir -p /workspace /input /output && \
    chown -R executor:executor /workspace /input /output && \
    chmod 755 /workspace /input /output
USER executor