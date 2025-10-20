#!/usr/bin/env python3
"""
Simulate EXACTLY what happens when the server starts.
This tests the auto-discovery mechanism that keeps failing.
"""

import sys
from pathlib import Path

# Add to path
project_root = Path(__file__).parent.parent.parent / "src" / "agent_c_tools" / "src"
sys.path.insert(0, str(project_root))

print("="*70)
print("SERVER STARTUP SIMULATION TEST")
print("="*70)
print("\nSimulating what happens when agent_c_tools is imported...\n")

try:
    # Step 1: This is what the server does - imports agent_c_tools
    print("1️⃣  Importing agent_c_tools (like server does)...")
    import agent_c_tools
    print("  ✅ agent_c_tools imported successfully")
    
    # Step 2: Check if PlaneTools is in the mapping
    print("\n2️⃣  Checking auto-discovery mapping...")
    if hasattr(agent_c_tools, '_tools_mapping'):
        mapping = agent_c_tools._tools_mapping
        print(f"  ✅ Found {len(mapping)} tools in mapping")
        
        if 'PlaneTools' in mapping:
            print(f"  ✅ PlaneTools found in mapping: {mapping['PlaneTools']}")
        else:
            print("  ❌ PlaneTools NOT in auto-discovery mapping!")
            print(f"  Available tools: {list(mapping.keys())}")
    
    # Step 3: Try to access PlaneTools (like server does)
    print("\n3️⃣  Accessing PlaneTools via __getattr__ (like server does)...")
    try:
        PlaneTools = agent_c_tools.PlaneTools
        print(f"  ✅ Got PlaneTools: {PlaneTools}")
        print(f"  ✅ PlaneTools is class: {type(PlaneTools)}")
    except AttributeError as e:
        print(f"  ❌ FAILED to get PlaneTools: {e}")
        raise
    
    # Step 4: Try to instantiate (like ToolChest does)
    print("\n4️⃣  Trying to instantiate PlaneTools...")
    try:
        instance = PlaneTools()
        print(f"  ✅ PlaneTools instantiated: {instance}")
        print(f"  ✅ Has name: {instance.name if hasattr(instance, 'name') else 'NO NAME'}")
    except Exception as e:
        print(f"  ❌ Failed to instantiate: {e}")
        raise
    
    # Step 5: Check registered toolsets
    print("\n5️⃣  Checking Toolset registry...")
    from agent_c.toolsets.tool_set import Toolset
    
    if hasattr(Toolset, '_registry'):
        registry = Toolset._registry
        print(f"  ✅ Toolset registry has {len(registry)} toolsets")
        
        plane_toolsets = [name for name in registry.keys() if 'plane' in name.lower()]
        if plane_toolsets:
            print(f"  ✅ Found PLANE toolsets in registry: {plane_toolsets}")
        else:
            print("  ⚠️  No PLANE toolsets in registry")
    
    print("\n" + "="*70)
    print("🎉 SERVER STARTUP SIMULATION PASSED!")
    print("="*70)
    print("\n✅ The server SHOULD start successfully with PLANE tools")
    print("✅ PlaneTools will be auto-discovered and loaded")
    print("\n🚀 SAFE TO RESTART SERVER")
    print("="*70)
    
except Exception as e:
    print("\n" + "="*70)
    print("❌ SERVER STARTUP SIMULATION FAILED!")
    print("="*70)
    print(f"\nError: {e}")
    print("\n⚠️  DO NOT RESTART SERVER - This exact error will occur!")
    print("="*70)
    import traceback
    traceback.print_exc()
    sys.exit(1)
