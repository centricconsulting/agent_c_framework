from typing import Dict, Any, List, Mapping
import os
import re
from .base_validator import ValidationResult, CommandValidator

class PowerShellCommandValidator:
    """
    Policy-driven PowerShell validator that enforces security restrictions based on YAML configuration.
    
    SECURITY PHILOSOPHY:
    PowerShell is extremely powerful and dangerous. This validator implements enforcement logic
    for policy-defined security controls, including:
    - Cmdlet whitelisting (from policy.safe_cmdlets)
    - Dangerous pattern detection (from policy.dangerous_patterns) 
    - Parameter restrictions (from policy flags/deny_global_flags)
    - Required security flags (from policy.require_flags)
    
    All security lists come from the YAML policy - the validator only implements enforcement logic.
    
    Policy example:
      powershell:
        validator: powershell
        flags: ["-Help", "-NoProfile", "-NonInteractive", "-NoLogo"]
        deny_global_flags: ["-Command", "-c", "-EncodedCommand", "-enc", "-File", "-f"]
        require_flags: ["-NoProfile", "-NonInteractive"]
        safe_cmdlets: ["get-process", "get-service", "format-table", "out-string"]
        dangerous_patterns: ["invoke-expression", "set-", "new-", "\\$\\("]
        safe_env:
          PSExecutionPolicyPreference: "Restricted"
        default_timeout: 30
    """
    
    def validate(self, parts: List[str], policy: Mapping[str, Any]) -> ValidationResult:
        if not parts:
            return ValidationResult(False, "Empty command")
            
        # Check if this is actually a PowerShell command
        base_name = os.path.splitext(os.path.basename(parts[0]))[0].lower()
        if base_name not in ("powershell", "pwsh"):
            return ValidationResult(False, "Not a PowerShell command")
        
        # Get policy configuration
        allowed_flags = set(f.lower() for f in (policy.get("flags") or []))
        denied_flags = set(f.lower() for f in (policy.get("deny_global_flags") or []))
        required_flags = policy.get("require_flags") or []
        safe_cmdlets = set(c.lower() for c in (policy.get("safe_cmdlets") or []))
        dangerous_patterns = policy.get("dangerous_patterns") or []
        
        # Parse command line arguments
        args = parts[1:]
        
        # Check for denied flags from policy
        for arg in args:
            arg_lower = arg.lower()
            base_arg = arg_lower.split(":", 1)[0].split("=", 1)[0]
            if base_arg in denied_flags or arg_lower in denied_flags:
                return ValidationResult(False, f"Denied flag: {arg}")
        
        # Validate allowed flags (only check flags that start with -)
        for arg in args:
            if arg.startswith("-"):
                arg_lower = arg.lower()
                base_arg = arg_lower.split(":", 1)[0].split("=", 1)[0]
                
                # Must be in allowed flags
                if base_arg not in allowed_flags:
                    return ValidationResult(False, f"Flag not allowed: {arg}")
        
        # Check for required flags (security-critical)
        for req_flag in required_flags:
            if not any(arg.lower().startswith(req_flag.lower()) for arg in args):
                return ValidationResult(False, f"Required security flag missing: {req_flag}")
        
        # If we have non-flag arguments, they might be commands - validate them
        non_flag_args = [arg for arg in args if not arg.startswith("-")]
        if non_flag_args:
            # Join all non-flag arguments as they might be a single command
            command_content = " ".join(non_flag_args)
            
            # Check for dangerous patterns (if policy defines them)
            for pattern in dangerous_patterns:
                if re.search(pattern, command_content, re.IGNORECASE):
                    return ValidationResult(False, f"Dangerous pattern detected: {pattern}")
            
            # Validate that only safe cmdlets are used (if policy defines them)
            if safe_cmdlets:
                # Extract potential cmdlets (words that look like cmdlets)
                potential_cmdlets = re.findall(r'\b[a-z]+-[a-z]+\b', command_content, re.IGNORECASE)
                for cmdlet in potential_cmdlets:
                    if cmdlet.lower() not in safe_cmdlets:
                        return ValidationResult(False, f"Cmdlet not allowed: {cmdlet}")
        
        return ValidationResult(True, "OK", timeout=policy.get("default_timeout"))
    
    def adjust_environment(self, base_env: Dict[str, str], parts: List[str], policy: Mapping[str, Any]) -> Dict[str, str]:
        """Set up PowerShell environment based on policy configuration"""
        env = dict(base_env)
        
        # Apply policy environment overrides first
        env_overrides = policy.get("env_overrides") or {}
        env.update(env_overrides)
        
        # Add any additional security environment variables that aren't already set
        # These are common PowerShell security settings that should always be applied
        security_defaults = {
            # Only set if not already configured in policy
            "PSModuleAutoLoadingPreference": "None",
            "PSReadlineHistorySaveStyle": "SaveNothing", 
            "POWERSHELL_TELEMETRY_OPTOUT": "1",
            "NO_COLOR": "1",
            "TERM": "dumb",
            "CLICOLOR": "0",
        }
        
        # Only apply defaults that aren't already set by policy
        for key, value in security_defaults.items():
            if key not in env:
                env[key] = value
        
        return env
    
    def adjust_arguments(self, parts: List[str], policy: Mapping[str, Any]) -> List[str]:
        """Inject required security flags based on policy"""
        require_flags = policy.get("require_flags") or []
        if not require_flags:
            return parts
            
        # Make a copy to avoid modifying the original
        new_parts = list(parts)
        
        # Check which required flags are missing and add them
        existing_args = [arg.lower() for arg in parts[1:]]
        
        for req_flag in require_flags:
            req_lower = req_flag.lower()
            # Check if this required flag is already present
            if not any(arg.startswith(req_lower) for arg in existing_args):
                new_parts.append(req_flag)
        
        return new_parts