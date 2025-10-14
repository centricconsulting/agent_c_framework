# Sharedown Slim - Quick Start Guide

Get up and running in 5 minutes! 🚀

## Prerequisites

- Python 3.8+ installed
- Chrome or Edge browser installed

## Installation

### Step 1: Install Python Dependencies

```bash
cd sharedown/slim
pip install -r requirements.txt
```

### Step 2: Install yt-dlp

```bash
pip install yt-dlp
```

### Step 3: Verify Configuration

```bash
python test_config.py
```

You should see:
```
✓ Browser found: C:\Program Files\Microsoft\Edge\Application\msedge.exe
✓ Configuration test completed successfully
```

## First Use

### Test with --show-config

```bash
python sharedown_slim.py --show-config
```

Output:
```
=== Sharedown Slim Configuration ===
Config file: C:\Users\YourName\AppData\Roaming\SharedownSlim\config.json
Browser: C:\Program Files\Microsoft\Edge\Application\msedge.exe
Output directory: C:\Users\YourName\Videos\Sharedown
...
```

### Download a Video

1. **Run the command** with your SharePoint video URL:

```bash
python sharedown_slim.py "https://university.sharepoint.com/sites/course/Shared%20Documents/video.mp4"
```

2. **Browser opens** - A browser window appears showing the video page

3. **Smart check** - Tool checks if you're already logged in (5 seconds)
   - Already logged in? → Continues automatically! ✅
   - Not logged in? → Shows instructions to log in manually ⚠️

4. **Wait for detection** - The tool detects when the video player loads

5. **Video downloads** - yt-dlp automatically downloads the video:

```
=== STEP 3: Video Download ===
Starting yt-dlp download...
[download] Downloading item 1 of 1
[download] 100% of 245.67MiB in 02:15

✓ Download completed successfully!
✓ File saved to: ~/Videos/Sharedown/Lecture_01.mp4
✓ File size: 245.67 MB
```

6. **Done!** - Video is ready to watch in your Videos folder

## Common Options

### Increase timeout (for slow logins)
```bash
python sharedown_slim.py "URL" --timeout 180
```

### Save command to file
```bash
python sharedown_slim.py "URL" --save-command
```

### Custom output directory
```bash
python sharedown_slim.py "URL" --output ~/Desktop/videos
```

### Verbose output (for debugging)
```bash
python sharedown_slim.py "URL" --verbose
```

## Troubleshooting

### "No browser found"
- Install Chrome or Edge
- Check with: `python test_config.py`
- Manually set path in config.json if needed

### "Authentication timeout"
- Use `--timeout 180` to allow more time
- Make sure video player fully loads
- Check logs in `%APPDATA%\SharedownSlim\logs\`

### "Failed to capture video URLs"
- Ensure you're fully logged in
- Wait for video player to appear
- Try `--keep-open` to inspect the page
- Use `--verbose` to see detailed capture attempts

## Tips

✅ **Save commands** - Use `--save-command` to create a file with the yt-dlp command  
✅ **Reuse cookies** - Generated commands work for ~1 hour (cookie lifetime)  
✅ **Check logs** - Detailed logs saved automatically for troubleshooting  
✅ **Batch processing** - Save commands for multiple videos, then download all at once  

## Workflow for Multiple Videos

```bash
# Capture command for video 1
python sharedown_slim.py "URL1" --save-command --output ./batch

# Capture command for video 2  
python sharedown_slim.py "URL2" --save-command --output ./batch

# Now run all downloads
cd batch
# Run each saved yt-dlp command
```

## Advanced: Custom Configuration

Edit config file at:
- Windows: `%APPDATA%\SharedownSlim\config.json`
- macOS: `~/Library/Application Support/SharedownSlim/config.json`
- Linux: `~/.config/sharedownslim/config.json`

```json
{
  "timeout": 120,
  "output_path": "D:\\Videos\\SharePoint",
  "custom_chrome_path": "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe",
  "enable_logging": true,
  "log_level": "DEBUG"
}
```

## Next Steps

- Read full [README.md](README.md) for detailed documentation
- Check logs if issues occur
- Use `--verbose` for debugging

---

**Happy downloading! 🎥**
