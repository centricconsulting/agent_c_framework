#!/usr/bin/env python3
"""
PLANE Cookie Refresh Utility

Automated cookie refresh using browser automation.
Requires: pip install playwright && playwright install chromium
"""

import sys
import asyncio
from pathlib import Path

# Add parent to path
sys.path.insert(0, str(Path(__file__).parent.parent.parent.parent))

from agent_c_tools.tools.plane.auth.cookie_refresh import PlaneCookieRefresh
from agent_c_tools.tools.plane.auth.cookie_manager import PlaneCookieManager


async def main():
    print("="*70)
    print("PLANE AUTOMATED COOKIE REFRESH")
    print("="*70)
    
    workspace = input("\nWorkspace slug (default: agent_c): ").strip() or "agent_c"
    base_url = input("PLANE URL (default: http://localhost): ").strip() or "http://localhost"
    
    print("\n" + "="*70)
    print("REFRESH OPTIONS")
    print("="*70)
    print("\n1. Refresh from existing browser session (easy)")
    print("2. Interactive login (manual)")
    print("3. Automated login with credentials (not yet supported)")
    
    choice = input("\nChoice (1-2): ").strip()
    
    refresher = PlaneCookieRefresh(base_url, workspace)
    cookie_manager = PlaneCookieManager(workspace)
    
    new_cookies = None
    
    if choice == "1":
        print("\n🔄 Refreshing from existing session...")
        print("   Opening browser briefly to extract cookies...")
        new_cookies = await refresher.refresh_from_existing_session()
        
    elif choice == "2":
        print("\n🔄 Interactive refresh...")
        new_cookies = await refresher.interactive_refresh()
    
    else:
        print(f"\n❌ Invalid choice: {choice}")
        return
    
    # Save and verify
    if new_cookies:
        print("\n✅ Cookies refreshed successfully!")
        print(f"   Obtained {len(new_cookies)} cookies:")
        for name in new_cookies.keys():
            print(f"     - {name}")
        
        # Save
        try:
            cookie_manager.save_cookies(new_cookies)
            print(f"\n✅ Cookies saved to: {cookie_manager._get_cookie_file()}")
        except Exception as e:
            print(f"\n❌ Failed to save cookies: {e}")
            return
        
        # Test
        print("\n🧪 Testing new cookies...")
        from agent_c_tools.tools.plane.auth.plane_session import PlaneSession
        
        try:
            session = PlaneSession(base_url, workspace, new_cookies)
            if session.test_connection():
                print("✅ New cookies are valid and working!")
            else:
                print("⚠️  Cookies saved but validation failed")
        except Exception as e:
            print(f"⚠️  Validation error: {e}")
    
    else:
        print("\n❌ Failed to refresh cookies")
        print("   Please check the browser window and try again")


if __name__ == "__main__":
    print("\n⚠️  Note: This requires Playwright to be installed:")
    print("   pip install playwright")
    print("   playwright install chromium\n")
    
    try:
        import playwright
        asyncio.run(main())
    except ImportError:
        print("❌ Playwright not installed!")
        print("\nInstall with:")
        print("  pip install playwright")
        print("  playwright install chromium")
        sys.exit(1)
