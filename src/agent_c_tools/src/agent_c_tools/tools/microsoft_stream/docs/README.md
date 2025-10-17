# Sharedown Slim

A streamlined SharePoint video downloader with manual authentication and automatic browser detection.

## Features

- ✅ **Smart Authentication** - Auto-detects if already logged in, prompts only if needed
- ✅ **Auto Browser Detection** - Automatically finds Chrome or Edge on your system
- ✅ **Video URL Capture** - Extracts video URLs from authenticated SharePoint pages
- ✅ **Automatic Download** - Downloads videos directly using yt-dlp with authentication
- ✅ **Comprehensive Logging** - Detailed logs for debugging and tracking
- ✅ **Cross-Platform** - Works on Windows, macOS, and Linux

## Requirements

### System Requirements
- Python 3.8 or higher
- Chrome or Edge browser installed
- yt-dlp installed and accessible in PATH

### Python Dependencies
- pyppeteer (browser automation)
- websockets (required by pyppeteer)

## Installation

### 1. Install Python Dependencies

```bash
pip install -r requirements.txt
```

### 2. Install yt-dlp

**Windows:**
```bash
pip install yt-dlp
```

**macOS (Homebrew):**
```bash
brew install yt-dlp
```

**Linux (pip):**
```bash
pip install yt-dlp
# or
sudo apt install yt-dlp  # Ubuntu/Debian
```

### 3. Verify Browser Detection

```bash
python sharedown_slim.py --show-config
```

This will display your configuration including the detected browser path.

## Usage

### Basic Usage

```bash
python sharedown_slim.py "https://university.sharepoint.com/sites/course/video.aspx"
```

### What Happens:

1. **Browser Launches** - A browser window opens to the video URL
2. **Smart Authentication** - Checks if you're already logged in (5 second check)
   - ✅ If video player found → continues immediately
   - ⚠️ If not found → prompts you to log in manually
3. **Auto Detection** - Application waits for the video player to appear
4. **URL Capture** - Extracts video stream URLs from the page
5. **Video Download** - Automatically downloads the video using yt-dlp
6. **Completion Report** - Shows file location and size

### Advanced Options

**Custom timeout (seconds to wait for login):**
```bash
python sharedown_slim.py "URL" --timeout 120
```

**Custom output directory:**
```bash
python sharedown_slim.py "URL" --output /path/to/videos
```

**Custom filename:**
```bash
python sharedown_slim.py "URL" --filename "Lecture_01"
```

**Keep browser open (for debugging):**
```bash
python sharedown_slim.py "URL" --keep-open
```

**Verbose logging:**
```bash
python sharedown_slim.py "URL" --verbose
```

**Show configuration:**
```bash
python sharedown_slim.py --show-config
```

## Configuration

Configuration is stored in a platform-specific location:

- **Windows:** `%APPDATA%\SharedownSlim\config.json`
- **macOS:** `~/Library/Application Support/SharedownSlim/config.json`
- **Linux:** `~/.config/sharedownslim/config.json`

### Default Settings

```json
{
  "custom_chrome_path": "",          // Auto-detected on first run
  "output_path": "",                 // Empty = ~/Videos/Sharedown
  "headless": false,                 // Show browser (required for manual login)
  "timeout": 90,                     // Seconds to wait for authentication
  "viewport_width": 1280,
  "viewport_height": 720,
  "enable_logging": true,
  "log_level": "INFO"
}
```

### Manual Browser Configuration

If auto-detection fails, you can manually set the browser path:

Edit `config.json` and set:
```json
{
  "custom_chrome_path": "C:\\Program Files\\Microsoft\\Edge\\Application\\msedge.exe"
}
```

Or on macOS/Linux:
```json
{
  "custom_chrome_path": "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome"
}
```

## Workflow Example

```bash
$ python sharedown_slim.py "https://myuniversity.sharepoint.com/video"

=== STEP 1: Authentication ===
Navigating to target URL...
✓ Page loaded
Checking if already authenticated...
✓ Already authenticated - video player detected immediately
✓ Authentication successful

=== STEP 2: Video URL Capture ===
✓ Video capture successful
  Method: javascript
  URLs found: 1
  Primary URL: https://myuniversity.sharepoint.com/file/12345...

=== STEP 3: Video Download ===
Preparing yt-dlp download...
✓ Command prepared successfully
Starting yt-dlp download...
======================================================================
[yt-dlp output appears here showing download progress]
======================================================================
✓ Download completed successfully!
✓ File saved to: ~/Videos/Sharedown/course_lecture_01.mp4
✓ File size: 245.67 MB
  Cookies used: 15

✓ Download completed successfully
```

## Troubleshooting

### Browser Not Found

**Error:** "No browser found. Please install Chrome or Edge."

**Solution:**
1. Install Chrome or Edge
2. Run `python sharedown_slim.py --show-config` to verify detection
3. If still not found, manually set path in config.json

### Authentication Timeout

**Error:** "Authentication failed or timed out"

**Solution:**
1. Increase timeout: `--timeout 180`
2. Ensure you're logging in within the time limit
3. Check that video player loads completely

### Video URL Not Captured

**Error:** "Failed to capture video URLs"

**Solution:**
1. Ensure you're fully authenticated (can see video player)
2. Wait for page to fully load before timeout
3. Try `--keep-open` to inspect page state
4. Check logs in `%APPDATA%\SharedownSlim\logs\`

### yt-dlp Command Fails

**Possible Issues:**
- Cookies may have expired (re-run the tool to get fresh cookies)
- URL may require specific headers (check error message)
- Network issues (try again)

## Logging

Logs are automatically saved to:
- **Windows:** `%APPDATA%\SharedownSlim\logs\`
- **macOS:** `~/Library/Application Support/SharedownSlim/logs/`
- **Linux:** `~/.config/sharedownslim/logs/`

Log files are named: `sharedown_slim_YYYYMMDD_HHMMSS.log`

View logs with `--verbose` flag for detailed output.

## Limitations

- **Manual Authentication Only** - You must log in yourself (no automation)
- **Cookie Lifetime** - Generated commands have limited validity (cookies expire)
- **Single Video** - One URL at a time (no batch processing)
- **No Download Execution** - Tool generates commands but doesn't download automatically

## Advantages

- ✅ **Simple & Focused** - Does one thing well
- ✅ **No Complex Configuration** - Auto-detects browser
- ✅ **Works with Any Login** - You handle authentication
- ✅ **Transparent Process** - See exactly what's happening
- ✅ **Reusable Commands** - Save and reuse generated commands

## Comparison: Slim vs Full

| Feature | Sharedown Slim | Sharedown Full |
|---------|---------------|----------------|
| Authentication | Manual only | Multiple modules |
| Browser Detection | Automatic | Automatic |
| Video Capture | Basic | Advanced |
| Download | Command only | Integrated |
| Configuration | Simple | Comprehensive |
| Use Case | Quick captures | Full automation |

## Contributing

This is a focused, minimal implementation. For feature requests or issues with the full version, see the main Sharedown project.

## License

See main Sharedown project for licensing information.

## Support

For issues:
1. Check `--show-config` for system info
2. Review logs in application data directory
3. Try with `--verbose` flag
4. Ensure yt-dlp is installed and accessible

---

**Sharedown Slim** - Simple, focused, effective. 🎥⚡
