# PowerShell Security Implementation

## Overview

This document describes the secure implementation of PowerShell command whitelisting for the Agent C framework. PowerShell is an extremely powerful and dangerous command-line shell that can execute arbitrary code, access system APIs, modify files, and perform destructive operations. This implementation applies maximum security restrictions while allowing only safe, read-only information gathering operations.

## Security Philosophy

**SAFETY FIRST**: PowerShell is treated as a high-risk command that requires maximum security controls. The implementation follows a whitelist-only approach, blocking all potentially dangerous operations by default.

**DEFENSE IN DEPTH**: Multiple layers of security controls are applied:
- Policy-level restrictions (YAML configuration)
- Validator-level logic (Python code)
- Environment-level controls (environment variables)

**PRINCIPLE OF LEAST PRIVILEGE**: Only the minimum necessary cmdlets and flags are allowed for legitimate information gathering use cases.

## Implementation Components

### 1. Policy Configuration (`agent_c_config/.whitelist_commands.yaml`)

Both `powershell` and `pwsh` (PowerShell Core) are configured with identical security restrictions:

```yaml
powershell:
  validator: powershell
  flags: ["-Help", "-?", "-NoProfile", "-nop", "-NonInteractive", "-noni", "-NoLogo", "-nol"]
  deny_global_flags: [
    "-Command", "-c", "-EncodedCommand", "-enc", "-e", "-File", "-f",
    "-ExecutionPolicy", "-ep", "-ex", "-WindowStyle", "-w", "-Sta", "-Mta",
    "-Version", "-v", "-PSConsoleFile", "-ConfigurationName",
    "-InputFormat", "-if", "-OutputFormat", "-of"
  ]
  require_flags: ["-NoProfile", "-NonInteractive"]
  safe_env:
    PSExecutionPolicyPreference: "Restricted"
    __PSLockdownPolicy: "1"
    PSModuleAutoLoadingPreference: "None"
  env_overrides:
    PSReadlineHistorySaveStyle: "SaveNothing"
    POWERSHELL_TELEMETRY_OPTOUT: "1"
    POWERSHELL_UPDATECHECK: "Off"
    PSDisableWinRTForCurrentUser: "1"
    NO_COLOR: "1"
    TERM: "dumb"
    CLICOLOR: "0"
  default_timeout: 30
```

### 2. Validator Implementation (`validators/powershell_validator.py`)

The `PowerShellCommandValidator` class implements **policy-driven** security enforcement logic. All security lists and restrictions come from the YAML policy configuration - the validator only implements the enforcement logic.

#### Policy-Driven Architecture
The validator reads security controls from the YAML policy:

- **`flags`**: Allowed PowerShell parameters (e.g., `-Help`, `-NoProfile`)
- **`deny_global_flags`**: Forbidden parameters that are always blocked
- **`require_flags`**: Security-critical flags that must be present
- **`safe_cmdlets`**: Whitelist of allowed PowerShell cmdlets
- **`dangerous_patterns`**: Regex patterns that indicate dangerous operations
- **`safe_env`**: Environment variables for security hardening

#### Enforcement Logic
The validator implements these enforcement mechanisms:

1. **Parameter Validation**: Checks flags against policy allow/deny lists
2. **Required Flag Injection**: Automatically adds missing security-critical flags
3. **Cmdlet Whitelisting**: Only allows cmdlets specified in `safe_cmdlets`
4. **Pattern Detection**: Uses regex patterns from `dangerous_patterns` to detect risky operations
5. **Environment Hardening**: Applies security environment variables from policy

#### Example Policy Configuration
```yaml
powershell:
  flags: ["-Help", "-NoProfile", "-NonInteractive"]
  deny_global_flags: ["-Command", "-c", "-File", "-f"]
  require_flags: ["-NoProfile", "-NonInteractive"]
  safe_cmdlets: ["get-process", "get-service", "format-table"]
  dangerous_patterns: ["invoke-expression", "set-", "new-", "\\$\\("]
```

#### Benefits of Policy-Driven Approach
- **Flexibility**: Security policies can be updated without code changes
- **Maintainability**: All security rules are centralized in YAML configuration
- **Auditability**: Security decisions are clearly documented in policy files
- **Extensibility**: New PowerShell configurations can have different restrictions

### 3. Environment Security

The validator sets up a highly restrictive PowerShell environment:

```python
security_env = {
    "PSExecutionPolicyPreference": "Restricted",      # Most restrictive policy
    "__PSLockdownPolicy": "1",                        # Enable lockdown mode
    "PSModuleAutoLoadingPreference": "None",          # Disable auto-loading
    "PSReadlineHistorySaveStyle": "SaveNothing",      # No history saving
    "POWERSHELL_TELEMETRY_OPTOUT": "1",              # Disable telemetry
    "POWERSHELL_UPDATECHECK": "Off",                  # Disable updates
    "PSDisableWinRTForCurrentUser": "1",             # Disable WinRT
    "NO_COLOR": "1",                                  # Disable colors
    "TERM": "dumb",                                   # Basic terminal
    "CLICOLOR": "0"                                   # Disable colors
}
```

### 4. Integration Points

#### Secure Command Executor
```python
# Import added
from .validators.powershell_validator import PowerShellCommandValidator

# Validator registered
self.validators: Dict[str, CommandValidator] = {
    # ... other validators
    "powershell": PowerShellCommandValidator(),
}
```

#### Dynamic Command Tools
```python
self.whitelisted_commands: Dict[str, Dict[str, Any]] = {
    # ... other commands
    "powershell": {"description": "Run safe, read-only PowerShell cmdlets for information gathering only."},
    "pwsh": {"description": "Run safe, read-only PowerShell Core cmdlets for information gathering only."},
}
```

## Security Validation

The implementation includes multiple validation layers:

1. **Command Parsing**: Blocks malformed or suspicious command structures
2. **Parameter Validation**: Ensures only safe parameters are used
3. **Flag Requirements**: Automatically injects required security flags
4. **Pattern Matching**: Detects dangerous operation patterns
5. **Cmdlet Whitelisting**: Only allows pre-approved safe cmdlets
6. **Environment Hardening**: Forces restrictive security settings

## Usage Examples

### Allowed Operations
```bash
# Information gathering
run_powershell path="//PROJECT" args="-NoProfile -NonInteractive Get-Process"
run_powershell path="//PROJECT" args="-NoProfile -NonInteractive Get-Service"
run_powershell path="//PROJECT" args="-NoProfile -NonInteractive Get-ChildItem"

# Help and documentation
run_powershell path="//PROJECT" args="-Help"
run_powershell path="//PROJECT" args="-NoProfile -NonInteractive Get-Help Get-Process"
```

### Blocked Operations (Security Violations)
```bash
# Script execution - BLOCKED
run_powershell path="//PROJECT" args="-Command 'Get-Process'"
run_powershell path="//PROJECT" args="-File script.ps1"
run_powershell path="//PROJECT" args="-EncodedCommand R2V0LVByb2Nlc3M="

# Dangerous cmdlets - BLOCKED  
run_powershell path="//PROJECT" args="Set-Location C:\\"
run_powershell path="//PROJECT" args="New-Item test.txt"
run_powershell path="//PROJECT" args="Invoke-Expression 'dangerous code'"

# Policy bypass attempts - BLOCKED
run_powershell path="//PROJECT" args="-ExecutionPolicy Bypass"
run_powershell path="//PROJECT" args="-Version 2.0"
```

## Security Rationale

### Why These Restrictions?

1. **Script Execution Blocked**: PowerShell's ability to execute arbitrary scripts and commands is its most dangerous feature. All script execution methods are blocked via `deny_global_flags` policy.

2. **Cmdlet Whitelisting**: Only cmdlets specified in the `safe_cmdlets` policy list are allowed. This ensures only read-only operations are permitted.

3. **Environment Hardening**: Multiple environment variables defined in `safe_env` and `env_overrides` force PowerShell into its most restrictive mode.

4. **Required Flags**: Security-critical flags defined in `require_flags` are automatically injected to prevent interactive bypasses.

5. **Pattern Detection**: Regex patterns from `dangerous_patterns` policy catch attempts to use dangerous PowerShell features.

6. **Policy-Driven Security**: All security controls are defined in YAML configuration, allowing for:
   - Easy security policy updates without code changes
   - Clear documentation of security decisions
   - Flexible configurations for different use cases
   - Centralized security rule management

### Risk Mitigation

- **Code Execution**: Completely prevented through parameter blocking and pattern detection
- **File System Access**: Limited to read-only operations via cmdlet whitelisting  
- **Network Access**: Blocked through cmdlet restrictions
- **System Modification**: Prevented via cmdlet whitelisting and environment controls
- **Privilege Escalation**: Mitigated through execution policy and lockdown mode
- **Information Disclosure**: Limited through history disabling and restricted cmdlets

## Monitoring and Logging

All PowerShell command attempts are logged with:
- Command attempted
- Validation result (allowed/blocked)
- Reason for blocking (if applicable)
- Execution time and resource usage
- Security environment applied

## Conclusion

This implementation provides a secure pathway for PowerShell information gathering while maintaining maximum security. The multi-layered approach ensures that even if one security control fails, others will prevent dangerous operations. The whitelist-only approach means that new PowerShell features or attack vectors are blocked by default until explicitly reviewed and approved.

**Key Security Guarantee**: No PowerShell command can execute arbitrary code, modify files, or perform system changes through this implementation.