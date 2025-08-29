# Adding run_command to Other Workspace Types

This document explains how to implement secure command execution (`run_command`) for workspace types other than `LocalStorageWorkspace`.

## Overview

Currently, only `LocalStorageWorkspace` supports command execution. The architecture is designed to make it straightforward to add this capability to other workspace types while maintaining security controls.

## Architecture Components

### 1. Base Class Setup

The `BaseWorkspace` class provides the foundation:

```python
class BaseWorkspace:
    supports_run_command: bool = False  # Override this in subclasses
    
    async def run_command(self, command: str, working_directory: Optional[str] = None):
        """
        Override in subclasses that support commands.
        Having this on the base class satisfies static type checkers/IDEs.
        """
        raise NotImplementedError(f"{self.__class__.__name__} does not support run_command")
```

### 2. Tool Layer Integration

The `WorkspaceTools` class automatically detects and uses `run_command` support:

```python
# In tool.py - run_command method
if not workspace or not hasattr(workspace, 'run_command'):
    return f"ERROR: Workspace: {workspace.name if workspace else 'None'} does not support OS level run commands"

# Check the supports_run_command flag
if not workspace.supports_run_command:
    return f"ERROR: Workspace: {workspace.name} does not support command execution"

# Call the workspace's run_command method
result: CommandExecutionResult = await workspace.run_command(command=command, working_directory=abs_path)
```

### 3. Security Layer

All implementations should use the same security components:
- `SecureCommandExecutor` - Core security and execution logic
- `YamlPolicyProvider` - Policy configuration management
- `CommandValidator` classes - Command-specific validation

## Implementation Guide

### Step 1: Enable Command Support

Set the flag and implement the method in your workspace class:

```python
class MyWorkspaceType(BaseWorkspace):
    supports_run_command = True  # Enable command execution
    
    def __init__(self, **kwargs):
        super().__init__("my_workspace_type", **kwargs)
        # Initialize your workspace-specific components
        # ...
        
        # Initialize secure command executor
        policy_provider = YamlPolicyProvider(".whitelist_commands.yaml")
        self.executor = SecureCommandExecutor(
            debug_mode=kwargs.get('debug_mode', True),
            log_output=True,
            default_timeout=30,
            max_output_size=1024 * 1024,
            policy_provider=policy_provider
        )
    
    async def run_command(self, command: str, working_directory: Optional[str] = None) -> CommandExecutionResult:
        """Run a command with security controls."""
        # Workspace-specific path validation and resolution
        if working_directory:
            resolved_wd = await self._resolve_working_directory(working_directory)
            if not resolved_wd:
                return CommandExecutionResult(
                    status="error",
                    command=command,
                    stdout="",
                    stderr="Invalid working directory",
                    return_code=1,
                    working_directory=working_directory
                )
            working_directory = resolved_wd
        
        # Delegate to secure executor
        return await self.executor.execute_command(
            command=command,
            working_directory=working_directory,
            env=None,
            timeout=None
        )
```

### Step 2: Implement Workspace-Specific Logic

Each workspace type requires different approaches for path resolution and command execution:

#### Storage-Only Workspaces
- S3StorageWorkspace
- AzureBlobWorkspace
- GoogleCloudStorageWorkspace
- FTPWorkspace
- Any pure file storage service
Uses base class, where `supports_run_command = False`

#### Executable-Capable Workspaces

```python
class SSHWorkspace(BaseWorkspace):
    supports_run_command = True
    
    def __init__(self, host: str, username: str, **kwargs):
        super().__init__("ssh", **kwargs)
        self.host = host
        self.username = username
        self.ssh_key_path = kwargs.get('ssh_key_path')
        self.base_path = kwargs.get('base_path', '/tmp/ssh_workspace')
        
        # SSH-specific policies might be more restrictive
        # recommend you create a secure command whitelist approach and executor that is policy driven
    
    async def run_command(self, command: str, working_directory: Optional[str] = None) -> CommandExecutionResult:
        """Execute command over SSH."""
        
        # Build SSH command
        ssh_command = ["ssh"]
        
        # SSH security options
        ssh_command.extend([
            "-o", "BatchMode=yes",  # No interactive prompts
            "-o", "ConnectTimeout=10",
            "-o", "ServerAliveInterval=60",
            "-o", "StrictHostKeyChecking=accept-new"
        ])
        
        if self.ssh_key_path:
            ssh_command.extend(["-i", self.ssh_key_path])
        
        # Add user and host
        ssh_command.append(f"{self.username}@{self.host}")
        
        
```

### Step 3: Policy Configuration

Each workspace type should have appropriate policies - these are examples only, not implemented:

#### docker_policies.yaml
```yaml
docker:
  subcommands:
    exec:
      flags: ["-i", "-t", "-w", "-u", "--workdir", "--user"]
      timeout: 120
  default_timeout: 60
  safe_env:
    DOCKER_BUILDKIT: "0"
  deny_global_flags: ["--privileged", "-v", "--volume", "--mount"]
```

#### ssh_policies.yaml  
```yaml
ssh:
  flags: ["-i", "-o", "-p", "-l"]
  default_timeout: 45
  safe_env:
    SSH_AUTH_SOCK: ""
    SSH_AGENT_PID: ""
  deny_global_flags: ["-D", "-L", "-R", "-X", "-Y"]
```

### Step 4: Testing Implementation

Create comprehensive tests for your implementation:

```python
import pytest
from your_workspace import MyWorkspaceType

@pytest.fixture
async def workspace():
    workspace = MyWorkspaceType(
        name="test",
        workspace_path="/path/to/test/workspace"
    )
    yield workspace
    # Cleanup if needed

@pytest.mark.asyncio
async def test_run_command_success(workspace):
    """Test successful command execution."""
    result = await workspace.run_command("echo 'hello world'")
    
    assert result.is_success
    assert "hello world" in result.stdout
    assert result.return_code == 0

@pytest.mark.asyncio
async def test_run_command_blocked(workspace):
    """Test blocked command."""
    result = await workspace.run_command("rm -rf /")
    
    assert result.status == "blocked"
    assert "not allowed" in result.error_message.lower()

@pytest.mark.asyncio
async def test_working_directory(workspace):
    """Test working directory handling."""
    result = await workspace.run_command("pwd", working_directory="subdir")
    
    assert result.is_success
    assert "subdir" in result.stdout

@pytest.mark.asyncio
async def test_invalid_working_directory(workspace):
    """Test invalid working directory."""
    result = await workspace.run_command("pwd", working_directory="../../../etc")
    
    assert result.is_error
    assert "invalid" in result.stderr.lower() or "unsafe" in result.stderr.lower()
```

## Security Considerations

### Path Resolution Security

Each workspace type should implement secure path resolution:

```python
def _is_path_within_workspace(self, path: str) -> bool:
    """
    Implement workspace-specific path validation:
    - Prevent directory traversal (../)  
    - Ensure paths are within workspace bounds
    - Handle workspace-specific path formats
    """
    
def _resolve_working_directory(self, path: str) -> Optional[str]:
    """
    Convert workspace-relative path to execution-safe path:
    - Validate path safety
    - Handle workspace-specific path mapping
    - Return absolute path for execution or None if invalid
    """
```

### Environment Control

Control the execution environment:

```python
# Example: Secure environment setup
def get_execution_environment(self) -> Dict[str, str]:
    """Get safe environment variables for command execution."""
    base_env = {
        # Clear dangerous variables
        "LD_PRELOAD": "",
        "LD_LIBRARY_PATH": "",
        
        # Set safe defaults
        "PATH": "/usr/local/bin:/usr/bin:/bin",
        "HOME": "/tmp/safe_home",
        
        # Workspace-specific variables
        "WORKSPACE_TYPE": self.type_name,
        "WORKSPACE_NAME": self.name,
    }
    
    return base_env
```



## Implementation Checklist

When adding `run_command` to a workspace type:

### ✅ Core Implementation
- [ ] Set `supports_run_command = True`
- [ ] Implement `run_command` method
- [ ] Create a `SecureCommandExecutor` class/functions
- [ ] Create and configure an appropriate `YamlPolicyProvider` policy, preference is file based

### ✅ Security Implementation  
- [ ] Implement secure path validation
- [ ] Handle working directory resolution safely
- [ ] Configure secure environment variables
- [ ] Set appropriate timeouts and resource limits

### ✅ Workspace-Specific Logic
- [ ] Handle workspace-specific path mapping
- [ ] Implement any required synchronization (for remote storage)
- [ ] Handle workspace-specific command adaptation (SSH, Docker, etc.)
- [ ] Implement cleanup procedures if needed

### ✅ Configuration
- [ ] Create workspace-specific policy files
- [ ] Configure appropriate command validators
- [ ] Set reasonable default timeouts
- [ ] Define safe environment variables

### ✅ Testing
- [ ] Test successful command execution
- [ ] Test command blocking/validation
- [ ] Test working directory handling
- [ ] Test error conditions
- [ ] Test workspace-specific scenarios

### ✅ Documentation
- [ ] Document any workspace-specific limitations
- [ ] Provide configuration examples
- [ ] Document security considerations
- [ ] Add troubleshooting guidance

This architecture ensures that command execution capabilities can be safely added to any workspace type while maintaining consistent security controls across all implementations.
