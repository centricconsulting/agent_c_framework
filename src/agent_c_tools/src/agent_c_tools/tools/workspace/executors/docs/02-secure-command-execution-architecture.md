# Secure Command Execution Architecture Overview

This document provides a comprehensive overview of the secure command execution architecture in the Agent C Framework, covering security mechanisms, architectural patterns, and implementation details.

## Executive Summary

The secure command execution system provides a robust, multi-layered security architecture that allows AI agents to execute operating system commands while maintaining strict security controls. The system employs whitelisting, validation, sandboxing, and monitoring to prevent malicious or accidental system damage.

## Core Architecture

### System Components

```
┌─────────────────────────────────────────────────────────────────┐
│                      Workspace Tool Layer                       │
├─────────────────────────────────────────────────────────────────┤
│  • UNC Path Validation   • Token Limits    • Error Handling    │
│  • Workspace Resolution  • Media Events    • Response Capping  │
└─────────────────────┬───────────────────────────────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────────────────────────────┐
│                    Workspace Implementations                    │
├─────────────────────────────────────────────────────────────────┤
│  LocalStorage │ S3Storage │ DockerWorkspace │ SSHWorkspace │ ... │
│  ✓ commands   │ ✗ no run  │ ✗ future       │ ✗ future     │     │
└─────────────────────┬───────────────────────────────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────────────────────────────┐
│                 Secure Command Executor                        │
├─────────────────────────────────────────────────────────────────┤
│  • Command Parsing    • Policy Enforcement  • Resource Limits  │
│  • Validator Dispatch • Environment Control • Timeout Mgmt     │
└─────────────────────┬───────────────────────────────────────────┘
                      │
          ┌───────────┼───────────┐
          ▼           ▼           ▼
┌─────────────┐ ┌─────────────┐ ┌─────────────┐
│   Policy    │ │ Validators  │ │  Execution  │
│  Provider   │ │             │ │   Engine    │
├─────────────┤ ├─────────────┤ ├─────────────┤
│ YAML Config │ │ Git, Pytest │ │ subprocess  │
│ File-based  │ │ Custom Impl │ │ Async I/O   │
│ Hot-reload  │ │ Extensible  │ │ Cross-plat  │
└─────────────┘ └─────────────┘ └─────────────┘
```

### Security Layers

The architecture implements defense-in-depth through multiple security layers:

#### Layer 1: Tool Layer Security
- **UNC Path Validation** - Ensures commands execute in authorized workspaces
- **Token Limits** - Prevents excessive output from consuming resources
- **Workspace Isolation** - Commands cannot cross workspace boundaries

#### Layer 2: Workspace Security  
- **Path Sandboxing** - All operations constrained to workspace directories
- **Permission Control** - Read-only workspaces block command execution
- **Type Safety** - Only workspace types that explicitly support commands

#### Layer 3: Command Security
- **Whitelist Enforcement** - Only explicitly allowed commands can execute
- **Policy Validation** - Every command checked against security policies
- **Argument Filtering** - Command arguments validated for safety

#### Layer 4: Execution Security
- **Environment Control** - Secure environment variable configuration
- **Resource Limits** - CPU time, memory, and output size constraints
- **Process Isolation** - Commands run in controlled subprocess environments

## Security Mechanisms

### 1. Whitelisting Strategy

The system uses an **explicit whitelist approach** where security is the default state:

```yaml
# Only commands with policies are allowed
git:
  subcommands:
    status: { flags: ["--porcelain", "-s"] }
    # Other git subcommands are blocked unless listed

# Commands without policies are automatically blocked  
# Example: "rm", "sudo", "curl" are blocked by default
```

**Key Principles:**
- **Default Deny** - Unknown commands are blocked
- **Explicit Allow** - Commands must be explicitly permitted
- **Granular Control** - Per-command and per-argument control
- **Fail Secure** - Parsing errors result in command blocking

### 2. Policy-Based Validation

#### Policy Structure
```yaml
command_name:
  subcommands:                    # Allowed subcommands
    subcommand_name:
      flags: ["--safe-flag"]      # Allowed flags for this subcommand
      timeout: 60                 # Subcommand-specific timeout
      
  deny_global_flags: ["--exec"]   # Globally blocked flags
  safe_env:                       # Environment variables
    SAFE_VAR: "safe_value"
  default_timeout: 30             # Fallback timeout
```

#### Policy Resolution
1. **Policy Lookup** - Find policy for base command
2. **Subcommand Check** - Verify subcommand is allowed (if applicable)
3. **Flag Validation** - Check each flag against whitelist
4. **Global Restrictions** - Apply deny lists and restrictions
5. **Environment Setup** - Configure safe execution environment

### 3. Command Validation Pipeline

```python
async def execute_command(command, working_directory, env, timeout):
    # 1. Parse command into parts
    parts = shlex.split(command)
    
    # 2. Resolve base command
    base = resolve_base(parts)  # "git" from "git status"
    
    # 3. Policy lookup (must exist)
    policy = policy_provider.get_policy(base, parts)
    if not policy:
        return blocked("No policy found")
    
    # 4. Validator dispatch (must exist)
    validator = validators.get(base)
    if not validator:
        return blocked("No validator registered")
    
    # 5. Validation
    result = validator.validate(parts, policy)
    if not result.allowed:
        return blocked(result.reason)
    
    # 6. Environment setup
    env = validator.adjust_environment(base_env, parts, policy)
    
    # 7. Secure execution
    return await execute_subprocess(parts, env, working_directory, timeout)
```

### 4. Blacklisting (Deny Lists)

While the primary strategy is whitelisting, strategic blacklisting provides additional protection:

#### Global Flag Blacklisting
```yaml
git:
  deny_global_flags: [
    "-c",           # Arbitrary config overrides
    "--exec-path",  # Custom git executable paths  
    "--help",       # Information disclosure
    "-P"            # Pager control
  ]
```

#### Path Blacklisting - FUTURE, not currently implemented
```python
def validate_path(path):
    # Block directory traversal
    if ".." in path.split(os.sep):
        return False
        
    # Block absolute paths in arguments
    if os.path.isabs(path) and not explicitly_allowed:
        return False
        
    # Block dangerous system paths
    dangerous_paths = ["/etc", "/proc", "/sys", "/dev"]
    if any(path.startswith(p) for p in dangerous_paths):
        return False
```

#### Environment Blacklisting -  FUTURE, not currently implemented
```python
def secure_environment(base_env):
    # Remove dangerous variables
    dangerous_vars = [
        "LD_PRELOAD",        # Library injection
        "LD_LIBRARY_PATH",   # Library path manipulation
        "PYTHONPATH",        # Python path injection
        "NODE_PATH",         # Node.js path injection
    ]
    
    for var in dangerous_vars:
        base_env.pop(var, None)
```

### 5. Buffer Protection

The system implements multiple levels of buffer protection to prevent resource exhaustion:

#### Output Size Limits
```python
class SecureCommandExecutor:
    def __init__(self, max_output_size=1024*1024):  # 1MB default
        self.max_output_size = max_output_size
        
    async def _read_stream(self, stream):
        chunks = []
        total_size = 0
        
        while True:
            chunk = await stream.read(65536)  # 64KB chunks
            if not chunk:
                break
                
            if total_size + len(chunk) > self.max_output_size:
                # Truncate and mark as truncated
                remaining = self.max_output_size - total_size
                if remaining > 0:
                    chunks.append(chunk[:remaining])
                return True, b"".join(chunks)  # truncated=True
                
            chunks.append(chunk)
            total_size += len(chunk)
            
        return False, b"".join(chunks)  # truncated=False
```

#### Memory Protection
- **Streaming I/O** - Large outputs processed in chunks
- **Bounded Buffers** - Maximum memory usage per command
- **Early Termination** - Stop reading when limits exceeded
- **Graceful Degradation** - Continue processing despite truncation

#### Timeout Protection
```python
# Multi-level timeout system
effective_timeout = (
    validator_timeout or       # Validator-specified
    policy_timeout or          # Policy-specified
    user_timeout or            # User-requested
    default_timeout            # System default
)

# Async timeout enforcement
async with asyncio.timeout(effective_timeout):
    result = await subprocess_execution()
```

## Cross-Platform Considerations

### Platform Detection
```python
class SecureCommandExecutor:
    def __init__(self):
        self.platform = platform.system().lower()
        self.is_windows = self.platform.startswith("win")
        
    def _configure_subprocess(self):
        if self.is_windows:
            # Windows-specific configuration
            creationflags = CREATE_NO_WINDOW
            shell = False
        else:
            # Unix-like configuration  
            creationflags = 0
            shell = False
```

### Path Handling
```python
def normalize_path(path):
    # Handle different path separators
    normalized = path.replace("\\", "/")
    
    # Apply OS-specific separator if needed
    if os.sep != "/":
        normalized = normalized.replace("/", os.sep)
        
    return os.path.normpath(normalized)
```

### Command Resolution
```python
def resolve_base_command(parts):
    base = os.path.basename(parts[0])
    
    # Remove .exe extension on Windows
    if base.lower().endswith(".exe"):
        base = base[:-4]
        
    # Handle Python module execution
    if base in ("python", "python3") and len(parts) >= 3 and parts[1] == "-m":
        return parts[2]  # Return module name as base command
        
    return base
```

## Monitoring and Logging

### Structured Logging
```python
def log_command_execution(self, command, result):
    log_entry = {
        'timestamp': datetime.now().isoformat(),
        'workspace': self.workspace_name,
        'command': command,
        'status': result.status,
        'return_code': result.return_code,
        'duration_ms': result.duration_ms,
        'working_directory': result.working_directory,
        'truncated_output': result.truncated_stdout or result.truncated_stderr
    }
    
    # Include output preview for successful commands (if reasonable size)
    if result.is_success and len(result.stdout) < 1024:
        log_entry['stdout_preview'] = result.stdout[:500]
        
    self.logger.info(json.dumps(log_entry))
```

### Security Event Logging
```python
def log_security_event(self, event_type, details):
    security_log = {
        'timestamp': datetime.now().isoformat(),
        'event_type': event_type,  # 'command_blocked', 'policy_violation', etc.
        'severity': 'HIGH' if event_type == 'command_blocked' else 'INFO',
        'details': details,
        'workspace': self.workspace_name,
        'user_context': self.get_user_context()
    }
    
    self.security_logger.warning(json.dumps(security_log))
```

### Performance Monitoring
```python
@dataclass 
class CommandExecutionResult:
    # ... other fields ...
    duration_ms: Optional[int] = None
    truncated_stdout: Optional[bool] = None
    truncated_stderr: Optional[bool] = None
    
    def get_performance_metrics(self):
        return {
            'execution_time': self.duration_ms,
            'output_truncated': self.truncated_stdout or self.truncated_stderr,
            'memory_efficient': self.truncated_stdout or self.truncated_stderr,
            'timeout_risk': self.duration_ms > 25000 if self.duration_ms else False
        }
```

## Error Handling and Recovery

### Graceful Degradation
```python
async def execute_command(self, command, working_directory=None):
    try:
        # Normal execution path
        return await self._execute_secure(command, working_directory)
        
    except asyncio.TimeoutError:
        # Timeout handling
        return CommandExecutionResult(
            status="timeout",
            command=command,
            error_message="Command exceeded time limit",
            stdout="",
            stderr="",
            return_code=None
        )
        
    except FileNotFoundError:
        # Command not found
        return CommandExecutionResult(
            status="error", 
            command=command,
            error_message=f"Command not found: {command.split()[0]}",
            stdout="",
            stderr="",
            return_code=127
        )
        
    except Exception as e:
        # Unexpected errors
        self.logger.exception(f"Unexpected error executing command: {command}")
        return CommandExecutionResult(
            status="error",
            command=command,
            error_message=f"Internal execution error: {str(e)}",
            stdout="",
            stderr="",
            return_code=1
        )
```

### Resource Cleanup
```python
async def _execute_with_cleanup(self, parts, env, working_directory, timeout):
    proc = None
    try:
        # Create subprocess
        proc = await asyncio.create_subprocess_exec(
            *parts,
            env=env,
            cwd=working_directory,
            stdout=asyncio.subprocess.PIPE,
            stderr=asyncio.subprocess.PIPE
        )
        
        # Execute with timeout
        stdout, stderr, return_code = await self._read_with_limits(proc, timeout)
        return stdout, stderr, return_code
        
    finally:
        # Ensure process cleanup
        if proc and proc.returncode is None:
            try:
                proc.terminate()
                await asyncio.wait_for(proc.wait(), timeout=5.0)
            except asyncio.TimeoutError:
                proc.kill()
                await proc.wait()
```

## Configuration Management

### Policy File Resolution
```python
class YamlPolicyProvider:
    def _resolve_path(self, policy_path):
        # 1. Environment override
        env_path = os.getenv("AGENTC_POLICIES_FILE")
        if env_path:
            return Path(env_path).resolve()
            
        # 2. Workspace-specific policies  
        if policy_path and Path(policy_path).exists():
            return Path(policy_path).resolve()
            
        # 3. Default location (next to provider)
        return (Path(__file__).parent / ".whitelist_commands.yaml").resolve()
```

### Hot Reloading
```python
def _load_if_changed(self):
    mtime = self.policy_file.stat().st_mtime
    
    if self._last_mtime != mtime:
        self.logger.info(f"Reloading policy file: {self.policy_file}")
        
        with open(self.policy_file, 'r', encoding='utf-8') as f:
            self._policies = yaml.safe_load(f) or {}
            
        self._last_mtime = mtime
        return True
        
    return False
```

### Environment-Specific Configuration
```python
# Development environment - more permissive
dev_config = {
    "show_windows": True,
    "log_output": True,
    "default_timeout": 60,
    "allow_experimental_commands": True
}

# Production environment - strict security
prod_config = {
    "show_windows": False,
    "log_output": True, 
    "default_timeout": 30,
    "allow_experimental_commands": False,
    "enhanced_monitoring": True
}
```

## Security Guarantees

### What the System Prevents

1. **Command Injection** - All commands parsed and validated before execution
2. **Path Traversal** - Working directories confined to workspace boundaries  
3. **Privilege Escalation** - No sudo, su, or privilege-granting commands allowed
4. **Resource Exhaustion** - Output size limits and timeouts prevent DOS
5. **Information Disclosure** - Dangerous flags blocked, output sanitized
6. **Environment Pollution** - Controlled environment variable management

### What the System Allows

1. **Safe Development Commands** - git, testing frameworks, build tools
2. **Read-Only Operations** - File inspection, status checking, information gathering
3. **Contained Modifications** - Changes within workspace boundaries only
4. **Standard Tools** - Common development and analysis utilities
5. **Custom Validation** - Extensible validator system for new tools

### Threat Model

The system is designed to protect against:

- **Malicious AI Behavior** - Preventing harmful command execution
- **Prompt Injection Attacks** - Commands disguised as legitimate requests  
- **Accidental Damage** - Protecting against unintended destructive operations
- **Resource Abuse** - Preventing system resource exhaustion
- **Data Exfiltration** - Blocking unauthorized data access or transmission

## Best Practices for Implementation

### Security-First Development
1. **Default Deny** - Start with everything blocked, add permissions as needed
2. **Minimal Permissions** - Grant only the minimum required access
3. **Defense in Depth** - Implement multiple layers of security controls
4. **Fail Securely** - Errors should result in blocked execution, not bypassed security

### Performance Considerations  
1. **Async Execution** - Non-blocking command execution
2. **Resource Limits** - Prevent runaway processes
3. **Efficient I/O** - Stream processing for large outputs
4. **Caching** - Policy caching to reduce file system overhead

### Monitoring and Maintenance
1. **Comprehensive Logging** - Log all command attempts and results
2. **Security Monitoring** - Alert on blocked commands and policy violations
3. **Regular Policy Reviews** - Periodic security policy audits
4. **Performance Monitoring** - Track execution times and resource usage

This architecture provides a robust foundation for secure command execution that balances security, functionality, and performance while remaining extensible for future enhancements and additional workspace types.
