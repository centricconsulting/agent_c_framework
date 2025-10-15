"""
PLANE Cookie Manager

Securely stores and manages PLANE session cookies.
Cookies are encrypted before saving to disk.
"""

import json
import os
from pathlib import Path
from typing import Dict, Optional
from cryptography.fernet import Fernet
import base64
import hashlib


class PlaneCookieManager:
    """Manage PLANE session cookies with encryption"""
    
    def __init__(self, workspace_slug: str = "agent_c"):
        """
        Initialize cookie manager for a workspace
        
        Args:
            workspace_slug: The PLANE workspace identifier
        """
        self.workspace_slug = workspace_slug
        self.config_dir = Path.home() / ".plane"
        self.cookies_dir = self.config_dir / "cookies"
        self.key_file = self.config_dir / ".key"
        
        # Required cookie names for PLANE
        self.required_cookies = [
            'session-id',
            'agentc-auth-token',
            'csrftoken',
        ]
        
        # Optional cookies
        self.optional_cookies = [
            'ajs_anonymous_id',
        ]
        
        # Ensure directories exist
        self._ensure_directories()
        
        # Load or create encryption key
        self._encryption_key = self._get_or_create_key()
    
    def _ensure_directories(self):
        """Create config directories if they don't exist"""
        self.config_dir.mkdir(parents=True, exist_ok=True)
        self.cookies_dir.mkdir(parents=True, exist_ok=True)
        
        # Set restrictive permissions (owner only)
        os.chmod(self.config_dir, 0o700)
        os.chmod(self.cookies_dir, 0o700)
    
    def _get_or_create_key(self) -> bytes:
        """Get existing encryption key or create new one"""
        if self.key_file.exists():
            with open(self.key_file, 'rb') as f:
                return f.read()
        else:
            # Generate new key
            key = Fernet.generate_key()
            with open(self.key_file, 'wb') as f:
                f.write(key)
            os.chmod(self.key_file, 0o600)  # Owner read/write only
            return key
    
    def _encrypt(self, data: str) -> bytes:
        """Encrypt string data"""
        f = Fernet(self._encryption_key)
        return f.encrypt(data.encode('utf-8'))
    
    def _decrypt(self, encrypted_data: bytes) -> str:
        """Decrypt data back to string"""
        f = Fernet(self._encryption_key)
        return f.decrypt(encrypted_data).decode('utf-8')
    
    def _get_cookie_file(self) -> Path:
        """Get path to cookie file for this workspace"""
        return self.cookies_dir / f"{self.workspace_slug}.enc"
    
    def save_cookies(self, cookies: Dict[str, str]) -> None:
        """
        Save cookies to encrypted file
        
        Args:
            cookies: Dictionary of cookie name -> value
            
        Raises:
            ValueError: If required cookies are missing
        """
        # Validate required cookies
        missing = [name for name in self.required_cookies if name not in cookies]
        if missing:
            raise ValueError(f"Missing required cookies: {', '.join(missing)}")
        
        # Add metadata
        cookie_data = {
            'workspace_slug': self.workspace_slug,
            'cookies': cookies,
        }
        
        # Convert to JSON and encrypt
        json_data = json.dumps(cookie_data, indent=2)
        encrypted = self._encrypt(json_data)
        
        # Save to file
        cookie_file = self._get_cookie_file()
        with open(cookie_file, 'wb') as f:
            f.write(encrypted)
        
        os.chmod(cookie_file, 0o600)  # Owner read/write only
    
    def load_cookies(self) -> Optional[Dict[str, str]]:
        """
        Load cookies from encrypted file
        
        Returns:
            Dictionary of cookies or None if not found
        """
        cookie_file = self._get_cookie_file()
        
        if not cookie_file.exists():
            return None
        
        try:
            # Read and decrypt
            with open(cookie_file, 'rb') as f:
                encrypted = f.read()
            
            json_data = self._decrypt(encrypted)
            cookie_data = json.loads(json_data)
            
            return cookie_data.get('cookies')
            
        except Exception as e:
            # If decryption fails, return None
            # (Could be corrupted file, wrong key, etc.)
            return None
    
    def validate_cookies(self, cookies: Dict[str, str]) -> bool:
        """
        Check if cookies have all required fields
        
        Args:
            cookies: Dictionary of cookies to validate
            
        Returns:
            True if all required cookies present, False otherwise
        """
        return all(name in cookies for name in self.required_cookies)
    
    def clear_cookies(self) -> None:
        """Delete saved cookies for this workspace"""
        cookie_file = self._get_cookie_file()
        if cookie_file.exists():
            cookie_file.unlink()
    
    def cookies_exist(self) -> bool:
        """Check if cookies file exists for this workspace"""
        return self._get_cookie_file().exists()
    
    @classmethod
    def list_workspaces(cls) -> list[str]:
        """
        List all workspaces with saved cookies
        
        Returns:
            List of workspace slugs
        """
        config_dir = Path.home() / ".plane" / "cookies"
        
        if not config_dir.exists():
            return []
        
        workspaces = []
        for file in config_dir.glob("*.enc"):
            workspace_slug = file.stem
            workspaces.append(workspace_slug)
        
        return sorted(workspaces)
    
    def get_cookie_value(self, cookies: Dict[str, str], name: str) -> Optional[str]:
        """
        Safely get a cookie value
        
        Args:
            cookies: Cookie dictionary
            name: Cookie name
            
        Returns:
            Cookie value or None if not found
        """
        return cookies.get(name)
    
    def format_cookies_for_requests(self, cookies: Dict[str, str]) -> Dict[str, str]:
        """
        Format cookies for use with requests library
        
        Args:
            cookies: Raw cookie dictionary
            
        Returns:
            Formatted cookie dictionary
        """
        # Just return as-is for now
        # Could add transformations if needed
        return cookies
