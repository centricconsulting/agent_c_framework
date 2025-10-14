"""
Configuration management for Microsoft Stream Capture Slim Approach.

Handles browser auto-detection (Chrome/Edge), settings persistence,
and application configuration.
"""

import json
import logging
import os
import shutil
import sys
from pathlib import Path
from typing import Dict, Any, Optional




class SlimConfig:
    """Manages configuration for Microsoft Stream Capture."""
    
    CONFIG_VERSION = 1
    
    # Slim configuration - focused on essentials
    DEFAULT_SETTINGS = {
        '_version': CONFIG_VERSION,
        'output_path': '',
        'custom_chrome_path': '',  # Auto-detected on first run
        'headless': False,  # Show browser for manual login
        'timeout': 90,  # Seconds to wait for manual login
        'viewport_width': 1280,
        'viewport_height': 720,
        'enable_logging': True,
        'log_level': 'INFO'
    }
    
    # Browser launch configuration for Pyppeteer
    BROWSER_ARGS = [
        '--disable-dev-shm-usage',
        '--no-sandbox',
        '--disable-setuid-sandbox',
        '--disable-web-security',
        '--no-first-run',
        '--no-default-browser-check',
        '--disable-extensions',
        '--disable-default-apps',
        '--disable-sync',
        '--disable-background-networking',
        '--blink-settings=imagesEnabled=true',
        # Suppress verbose logging that causes noisy console output
        '--log-level=3',  # Only show fatal errors
        # '--silent',
        # '--disable-logging',
        # '--disable-gpu-sandbox',
        # '--disable-software-rasterizer',
        # '--disable-background-timer-throttling',
        # '--disable-backgrounding-occluded-windows',
        # '--disable-renderer-backgrounding',
        # '--disable-features=TranslateUI',
        # '--disable-ipc-flooding-protection',
        # '--disable-hang-monitor',
        # '--disable-client-side-phishing-detection',
        # '--disable-component-update',
        # '--no-zygote',
        # '--single-process'
    ]
    
    def __init__(self, app_name: str = 'MSFTStreamSlim'):
        self.app_name = app_name
        self.app_data_path = self._get_app_data_path()
        self.config_file = self.app_data_path / 'config.json'
        
        # Create directories
        self.app_data_path.mkdir(parents=True, exist_ok=True)
        self.logger = logging.getLogger(__name__)

        
        # Load or create configuration
        self.settings = self._load_config()
        
        # Auto-detect browser if not configured
        if not self.settings.get('custom_chrome_path'):
            chrome_path = self._detect_browser()
            if chrome_path:
                self.settings['custom_chrome_path'] = chrome_path
                self._save_config()
                self.logger.info(f"✓ Browser auto-detected and saved: {chrome_path}")
            else:
                self.logger.warning("⚠ No browser found. Please install Chrome or Edge.")
    
    def _get_app_data_path(self) -> Path:
        """Get platform-specific application data directory."""
        if os.name == 'nt':  # Windows
            base = Path(os.environ.get('APPDATA', ''))
        elif sys.platform == 'darwin':  # macOS
            base = Path.home() / 'Library' / 'Application Support'
        else:  # Linux/Unix
            base = Path.home() / '.config'
        
        return base / self.app_name
    
    def _load_config(self) -> Dict[str, Any]:
        """Load configuration from file or create defaults."""
        if self.config_file.exists():
            try:
                with open(self.config_file, 'r', encoding='utf-8') as f:
                    stored = json.load(f)
                
                # Merge with defaults to ensure all keys exist
                config = self.DEFAULT_SETTINGS.copy()
                config.update(stored)
                
                self.logger.debug(f"Configuration loaded from {self.config_file}")
                return config
            except (json.JSONDecodeError, IOError) as e:
                self.logger.error(f"Failed to load config: {e}, using defaults")
        
        # Return defaults for first run
        self.logger.info("No configuration found, creating defaults")
        return self.DEFAULT_SETTINGS.copy()
    
    def _save_config(self):
        """Save current configuration to file."""
        self.settings['_version'] = self.CONFIG_VERSION

        try:
            with open(self.config_file, 'w', encoding='utf-8') as f:
                json.dump(self.settings, f, indent=2, ensure_ascii=False)
            self.logger.debug(f"Configuration saved to {self.config_file}")
        except IOError as e:
            self.logger.error(f"Failed to save configuration: {e}")
    
    def _detect_browser(self) -> Optional[str]:
        """
        Auto-detect Chrome or Edge browser installation.
        
        Priority: Edge → Chrome (Edge more commonly available on Windows)
        
        Returns:
            Path to browser executable or None if not found
        """
        self.logger.info("Scanning for Chrome or Edge browser...")
        
        # First check PATH
        for command in ['msedge', 'chrome', 'google-chrome', 'chromium', 'chromium-browser']:
            path = shutil.which(command)
            if path:
                self.logger.info(f"✓ Found '{command}' in PATH: {path}")
                return path
        
        # Platform-specific common paths
        search_paths = []
        
        if os.name == 'nt':  # Windows
            program_files = os.environ.get('PROGRAMFILES', 'C:\\Program Files')
            program_files_x86 = os.environ.get('PROGRAMFILES(X86)', 'C:\\Program Files (x86)')
            local_appdata = Path.home() / 'AppData' / 'Local'
            
            search_paths = [
                # Microsoft Edge (most common on Windows 10/11)
                local_appdata / 'Microsoft' / 'Edge' / 'Application' / 'msedge.exe',
                Path(program_files_x86) / 'Microsoft' / 'Edge' / 'Application' / 'msedge.exe',
                Path(program_files) / 'Microsoft' / 'Edge' / 'Application' / 'msedge.exe',
                # Google Chrome
                local_appdata / 'Google' / 'Chrome' / 'Application' / 'chrome.exe',
                Path(program_files) / 'Google' / 'Chrome' / 'Application' / 'chrome.exe',
                Path(program_files_x86) / 'Google' / 'Chrome' / 'Application' / 'chrome.exe',
            ]
            
        elif sys.platform == 'darwin':  # macOS
            search_paths = [
                Path('/Applications/Google Chrome.app/Contents/MacOS/Google Chrome'),
                Path('/Applications/Microsoft Edge.app/Contents/MacOS/Microsoft Edge'),
                Path('/Applications/Chromium.app/Contents/MacOS/Chromium'),
                Path.home() / 'Applications' / 'Google Chrome.app' / 'Contents' / 'MacOS' / 'Google Chrome',
            ]
            
        else:  # Linux
            search_paths = [
                Path('/usr/bin/microsoft-edge'),
                Path('/usr/bin/google-chrome'),
                Path('/usr/bin/google-chrome-stable'),
                Path('/usr/bin/chromium'),
                Path('/usr/bin/chromium-browser'),
                Path('/snap/bin/chromium'),
            ]
        
        # Check each path
        for path in search_paths:
            if path.exists():
                self.logger.info(f"✓ Found browser: {path}")
                return str(path)
        
        self.logger.warning("✗ No Chrome or Edge installation found")
        return None
    
    def get_browser_path(self) -> Optional[str]:
        """Get configured browser path, re-detecting if necessary."""
        path = self.settings.get('custom_chrome_path', '')
        
        if path and Path(path).exists():
            return path
        
        # Re-detect if previous path is invalid
        self.logger.warning("Configured browser path invalid, re-detecting...")
        new_path = self._detect_browser()
        if new_path:
            self.settings['custom_chrome_path'] = new_path
            self._save_config()
        
        return new_path
    
    def get_browser_launch_options(self) -> dict:
        """
        Get Pyppeteer browser launch options.
        
        Returns:
            Dict ready to pass to pyppeteer.launch(**options)
        """
        options = {
            'headless': self.settings.get('headless', False),
            'autoClose': False,  # Prevent atexit errors
            'args': self.BROWSER_ARGS.copy()
        }
        
        # Add browser executable path
        chrome_path = self.get_browser_path()
        if chrome_path:
            options['executablePath'] = chrome_path
        
        return options


    
    def get_timeout(self) -> int:
        """Get timeout in seconds for manual login."""
        return self.settings.get('timeout', 90)
    
    def get_viewport(self) -> dict:
        """Get browser viewport size."""
        return {
            'width': self.settings.get('viewport_width', 1280),
            'height': self.settings.get('viewport_height', 720)
        }
    
    def update(self, key: str, value: Any):
        """Update a configuration value and save."""
        self.settings[key] = value
        self._save_config()
        self.logger.debug(f"Config updated: {key} = {value}")
    
    def show_info(self):
        """Display configuration information."""
        print("\n" + "=" * 70)
        print("  Sharedown Slim Configuration")
        print("=" * 70)
        
        browser = self.get_browser_path()
        browser_display = browser if browser else '❌ Not found (install Chrome or Edge)'
        
        print(f"\n📁 Config file:     {self.config_file}")
        print(f"🌐 Browser:         {browser_display}")
        # print(f"💾 Output directory: {self.get_output_path()}")
        print(f"⏱️  Timeout:          {self.get_timeout()}s")
        print(f"👁️  Headless mode:   {self.settings.get('headless')}")
        
        print("\n" + "=" * 70 + "\n")
