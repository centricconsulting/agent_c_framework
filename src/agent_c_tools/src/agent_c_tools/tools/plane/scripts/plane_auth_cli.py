#!/usr/bin/env python3
"""
PLANE Authentication CLI

Command-line tool to manage PLANE session cookies.
"""

import sys
from pathlib import Path

# Add parent directory to path for imports
sys.path.insert(0, str(Path(__file__).parent.parent.parent.parent))

from agent_c_tools.tools.plane.auth.cookie_manager import PlaneCookieManager
from agent_c_tools.tools.plane.auth.plane_session import PlaneSession


def setup_cookies(workspace_slug: str = "agent_c"):
    """Interactive cookie setup"""
    print("="*70)
    print("PLANE Cookie Setup")
    print("="*70)
    print(f"\nWorkspace: {workspace_slug}")
    print("\nTo get your cookies:")
    print("1. Open PLANE in your browser (http://localhost/agent_c/)")
    print("2. Press F12 → Application tab → Cookies → http://localhost")
    print("3. Copy the values for these cookies:")
    print()
    
    cookie_manager = PlaneCookieManager(workspace_slug)
    cookies = {}
    
    # Get required cookies
    for cookie_name in cookie_manager.required_cookies:
        while True:
            value = input(f"{cookie_name}: ").strip()
            if value:
                cookies[cookie_name] = value
                break
            print(f"  ⚠️  {cookie_name} is required!")
    
    # Get optional cookies
    print("\nOptional cookies (press Enter to skip):")
    for cookie_name in cookie_manager.optional_cookies:
        value = input(f"{cookie_name}: ").strip()
        if value:
            cookies[cookie_name] = value
    
    # Save cookies
    try:
        cookie_manager.save_cookies(cookies)
        print(f"\n✅ Cookies saved successfully!")
        print(f"   Location: {cookie_manager._get_cookie_file()}")
        
        # Test cookies
        print("\n🔍 Testing cookies...")
        session = PlaneSession("http://localhost", workspace_slug, cookies)
        if session.test_connection():
            print("✅ Cookies are valid and working!")
        else:
            print("⚠️  Cookies saved but validation failed. They may be expired.")
            
    except Exception as e:
        print(f"\n❌ Error saving cookies: {e}")
        return False
    
    return True


def show_cookies(workspace_slug: str = "agent_c"):
    """Show current cookies (masked)"""
    cookie_manager = PlaneCookieManager(workspace_slug)
    cookies = cookie_manager.load_cookies()
    
    if not cookies:
        print(f"❌ No cookies found for workspace '{workspace_slug}'")
        print("\nRun: python plane_auth_cli.py setup")
        return
    
    print("="*70)
    print(f"PLANE Cookies for '{workspace_slug}'")
    print("="*70)
    
    for name, value in cookies.items():
        # Mask value for security
        if len(value) > 20:
            masked = f"{value[:10]}...{value[-10:]}"
        else:
            masked = "*" * len(value)
        print(f"{name}: {masked}")
    
    print()


def test_cookies(workspace_slug: str = "agent_c"):
    """Test if current cookies work"""
    print("="*70)
    print("Testing PLANE Cookies")
    print("="*70)
    
    cookie_manager = PlaneCookieManager(workspace_slug)
    cookies = cookie_manager.load_cookies()
    
    if not cookies:
        print(f"❌ No cookies found for workspace '{workspace_slug}'")
        print("\nRun: python plane_auth_cli.py setup")
        return
    
    try:
        print(f"\nWorkspace: {workspace_slug}")
        print("Testing connection...")
        
        session = PlaneSession("http://localhost", workspace_slug, cookies)
        
        if session.test_connection():
            print("✅ Cookies are valid and working!")
            
            # Try to get user info
            response = session.get("/api/users/me/")
            if response.status_code == 200:
                user = response.json()
                print(f"\n👤 Authenticated as:")
                print(f"   Email: {user.get('email')}")
                print(f"   Name: {user.get('display_name', 'N/A')}")
        else:
            print("❌ Cookies are invalid or expired")
            print("\nRun: python plane_auth_cli.py setup")
            
    except Exception as e:
        print(f"❌ Error testing cookies: {e}")


def list_workspaces():
    """List all workspaces with saved cookies"""
    workspaces = PlaneCookieManager.list_workspaces()
    
    if not workspaces:
        print("No saved workspaces found")
        return
    
    print("="*70)
    print("Saved Workspaces")
    print("="*70)
    
    for workspace in workspaces:
        print(f"  - {workspace}")
    
    print()


def clear_cookies(workspace_slug: str = "agent_c"):
    """Clear saved cookies"""
    cookie_manager = PlaneCookieManager(workspace_slug)
    
    if not cookie_manager.cookies_exist():
        print(f"No cookies found for workspace '{workspace_slug}'")
        return
    
    confirm = input(f"Clear cookies for '{workspace_slug}'? (y/N): ").strip().lower()
    
    if confirm == 'y':
        cookie_manager.clear_cookies()
        print(f"✅ Cookies cleared for '{workspace_slug}'")
    else:
        print("Cancelled")


def main():
    """Main CLI entry point"""
    if len(sys.argv) < 2:
        print("PLANE Authentication CLI")
        print()
        print("Usage:")
        print("  python plane_auth_cli.py setup [workspace]       - Setup cookies")
        print("  python plane_auth_cli.py test [workspace]        - Test cookies")
        print("  python plane_auth_cli.py show [workspace]        - Show cookies")
        print("  python plane_auth_cli.py list                    - List workspaces")
        print("  python plane_auth_cli.py clear [workspace]       - Clear cookies")
        print()
        print("Default workspace: agent_c")
        return
    
    command = sys.argv[1].lower()
    workspace = sys.argv[2] if len(sys.argv) > 2 else "agent_c"
    
    if command == "setup":
        setup_cookies(workspace)
    elif command == "test":
        test_cookies(workspace)
    elif command == "show":
        show_cookies(workspace)
    elif command == "list":
        list_workspaces()
    elif command == "clear":
        clear_cookies(workspace)
    else:
        print(f"Unknown command: {command}")
        print("Run without arguments for help")


if __name__ == "__main__":
    main()
