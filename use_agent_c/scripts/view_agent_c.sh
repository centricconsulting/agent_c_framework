#!/bin/bash
set -e

# Function to open URL in the default browser
open_browser() {
    URL="https://localhost:5173/chat"

    # Try different commands based on the platform
    if command -v open &>/dev/null; then
        # macOS
        open "$URL"
    elif command -v xdg-open &>/dev/null; then
        # Linux with xdg-utils
        xdg-open "$URL"
    elif command -v gnome-open &>/dev/null; then
        # Gnome desktop
        gnome-open "$URL"
    elif command -v wslview &>/dev/null; then
        # Windows Subsystem for Linux
        wslview "$URL"
    else
        echo "Please open a browser and navigate to: $URL"
    fi
}

# Open the browser to Agent C's web interface
open_browser
