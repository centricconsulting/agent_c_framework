#!/usr/bin/env python3
"""
Simple test to verify PowerShell integration works correctly.
This tests the validator, policy loading, and basic security restrictions.
"""

import sys
import os
import yaml
import asyncio
from pathlib import Path

# Add the source directory to the path
sys.path.insert(0, str(Path(__file__).parent / "src" / "agent_c_tools" / "src"))

from agent_c_tools.tools.workspace.executors.local_storage.validators.powershell_validator import PowerShellCommandValidator
from agent_c_tools.tools.workspace.executors.local_storage.validators.base_validator import ValidationResult

def test_yaml_parsing():
    """Test that the YAML configuration is valid"""
    print("Testing YAML configuration parsing...")
    
    yaml_path = Path(__file__).parent / "agent_c_config" / ".whitelist_commands.yaml"
    
    try:
        with open(yaml_path, 'r') as f:
            config = yaml.safe_load(f)
        
        # Check that PowerShell configurations exist
        assert 'powershell' in config, "PowerShell configuration missing"
        assert 'pwsh' in config, "PowerShell Core configuration missing"
        
        # Check key security settings
        ps_config = config['powershell']
        assert ps_config['validator'] == 'powershell', "Wrong validator specified"
        assert '-Command' in ps_config['deny_global_flags'], "Command execution not blocked"
        assert '-NoProfile' in ps_config['require_flags'], "NoProfile not required"
        assert ps_config['safe_env']['PSExecutionPolicyPreference'] == 'Restricted', "Execution policy not restricted"
        
        print("✓ YAML configuration is valid and secure")
        return True
        
    except Exception as e:
        print(f"✗ YAML configuration error: {e}")
        return False

def test_validator_security():
    """Test that the validator blocks dangerous operations"""
    print("Testing PowerShell validator security...")
    
    validator = PowerShellCommandValidator()
    
    # Mock policy similar to our YAML config
    policy = {
        'flags': ['-Help', '-?', '-NoProfile', '-nop', '-NonInteractive', '-noni', '-NoLogo', '-nol'],
        'deny_global_flags': ['-Command', '-c', '-EncodedCommand', '-enc', '-e', '-File', '-f'],
        'require_flags': ['-NoProfile', '-NonInteractive'],
        'safe_cmdlets': ['get-process', 'get-service', 'get-childitem', 'format-table', 'out-string'],
        'dangerous_patterns': ['invoke-expression', 'set-', 'new-', '\\$\\(', 'start-process'],
        'safe_env': {
            'PSExecutionPolicyPreference': 'Restricted',
            '__PSLockdownPolicy': '1'
        },
        'default_timeout': 30
    }
    
    # Test cases that should be BLOCKED
    dangerous_commands = [
        (['powershell.exe', '-Command', 'Get-Process'], "denied flag -Command"),
        (['powershell.exe', '-c', 'Write-Host "Hello"'], "denied flag -c"),
        (['powershell.exe', '-EncodedCommand', 'R2V0LVByb2Nlc3M='], "denied flag -EncodedCommand"),
        (['powershell.exe', '-File', 'script.ps1'], "denied flag -File"),
        (['powershell.exe', '-ExecutionPolicy', 'Bypass'], "flag not allowed"),
        (['powershell.exe', '-NoProfile', '-NonInteractive', 'Invoke-Expression', '"dangerous code"'], "dangerous pattern"),
        (['powershell.exe', '-NoProfile', '-NonInteractive', 'Set-Location', 'C:\\'], "dangerous pattern set-"),
        (['powershell.exe', '-NoProfile', '-NonInteractive', 'New-Item', '-Path', 'test.txt'], "dangerous pattern new-"),
        (['powershell.exe', '-NoProfile', '-NonInteractive', 'Remove-Item', 'test.txt'], "cmdlet not allowed"),
    ]
    
    for cmd, expected_reason in dangerous_commands:
        result = validator.validate(cmd, policy)
        if result.allowed:
            print(f"✗ SECURITY FAILURE: Command should be blocked: {' '.join(cmd)}")
            return False
        else:
            print(f"✓ Correctly blocked: {' '.join(cmd[:2])} - {result.reason}")
    
    # Test cases that should be ALLOWED (with required flags)
    safe_commands = [
        ['powershell.exe', '-NoProfile', '-NonInteractive', '-Help'],
        ['powershell.exe', '-NoProfile', '-NonInteractive', '-NoLogo'],
        ['powershell.exe', '-NoProfile', '-NonInteractive', 'Get-Process'],
        ['powershell.exe', '-NoProfile', '-NonInteractive', 'Get-Service', '|', 'Format-Table'],
    ]
    
    for cmd in safe_commands:
        result = validator.validate(cmd, policy)
        if not result.allowed:
            print(f"✗ Safe command was blocked: {' '.join(cmd)} - {result.reason}")
            return False
        else:
            print(f"✓ Correctly allowed: {' '.join(cmd)}")
    
    print("✓ Validator security tests passed")
    return True

def test_environment_security():
    """Test that the validator sets up secure environment"""
    print("Testing PowerShell environment security...")
    
    validator = PowerShellCommandValidator()
    
    policy = {
        'safe_env': {
            'PSExecutionPolicyPreference': 'Restricted',
            '__PSLockdownPolicy': '1'
        },
        'env_overrides': {
            'NO_COLOR': '1',
            'POWERSHELL_TELEMETRY_OPTOUT': '1'
        }
    }
    
    base_env = {'PATH': '/usr/bin', 'HOME': '/home/user'}
    parts = ['powershell.exe', '-NoProfile', '-NonInteractive']
    
    result_env = validator.adjust_environment(base_env, parts, policy)
    
    # Check critical security settings
    security_checks = [
        ('PSExecutionPolicyPreference', 'Restricted'),
        ('__PSLockdownPolicy', '1'),
        ('NO_COLOR', '1'),
        ('POWERSHELL_TELEMETRY_OPTOUT', '1'),
        ('PSModuleAutoLoadingPreference', 'None'),
    ]
    
    for key, expected_value in security_checks:
        if result_env.get(key) != expected_value:
            print(f"✗ Security environment not set: {key} should be {expected_value}, got {result_env.get(key)}")
            return False
        else:
            print(f"✓ Security setting: {key} = {expected_value}")
    
    print("✓ Environment security tests passed")
    return True

def main():
    """Run all tests"""
    print("PowerShell Integration Security Test")
    print("=" * 50)
    
    tests = [
        test_yaml_parsing,
        test_validator_security,
        test_environment_security,
    ]
    
    passed = 0
    total = len(tests)
    
    for test in tests:
        print()
        try:
            if test():
                passed += 1
            else:
                print("Test failed!")
        except Exception as e:
            print(f"Test error: {e}")
    
    print()
    print("=" * 50)
    print(f"Test Results: {passed}/{total} passed")
    
    if passed == total:
        print("🎉 All tests passed! PowerShell integration is secure.")
        return 0
    else:
        print("❌ Some tests failed. Review security implementation.")
        return 1

if __name__ == "__main__":
    sys.exit(main())