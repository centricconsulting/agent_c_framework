# Command Line Execution Tool

## What This Tool Does

The Command Line Execution Tool enables agents to run shell commands on the local system with explicit user approval. This powerful capability allows agents to interact with local tools, applications, and services while maintaining a security boundary through mandatory user confirmation.

## Key Capabilities

- **Execute shell commands** with user approval
- **Work with local applications** like Docker, security scanners, and build tools
- **Stream real-time output** from long-running commands
- **Set custom environment variables** for command execution
- **Specify working directories** for command context
- **Apply timeouts** to prevent hung processes

## Practical Use Cases

### Docker Container Management

Agents can help build, scan, and deploy Docker containers by executing the appropriate Docker CLI commands, with your approval for each operation.

### Security Scanning

Agents can run security scanning tools and analyze the results, identifying vulnerabilities and suggesting remediation steps.

### System Diagnostics

Agents can help troubleshoot system issues by running diagnostic commands and interpreting the results.

### Task Automation

Agents can automate complex workflows by executing sequences of commands, waiting for approval at each critical step.

## Example Interactions

### Docker Image Building

**User**: "Build a secure Python web application container from this Dockerfile."

**Agent**: "I'll help you build this container. First, I need to run a Docker build command:"

```
[COMMAND APPROVAL REQUIRED]
Command: docker build -t secure-webapp:latest .

Do you approve execution of this command? (yes/no):
```

*User approves*

```
[EXECUTING COMMAND]

Sending build context to Docker daemon  4.096kB
Step 1/10 : FROM python:3.9-slim-buster
...

[COMMAND COMPLETED]
Exit Code: 0
```

"Great! The image built successfully. Now I'll scan it for vulnerabilities:"

```
[COMMAND APPROVAL REQUIRED]
Command: docker scout quickview secure-webapp:latest

Do you approve execution of this command? (yes/no):
```

### Security Scanning

**User**: "Check my server for open ports."

**Agent**: "I'll check for open ports using the netstat command:"

```
[COMMAND APPROVAL REQUIRED]
Command: netstat -tuln

Do you approve execution of this command? (yes/no):
```

*User approves*

```
[EXECUTING COMMAND]

Active Internet connections (only servers)
Proto Recv-Q Send-Q Local Address           Foreign Address         State      
tcp        0      0 0.0.0.0:22              0.0.0.0:*               LISTEN     
tcp        0      0 127.0.0.1:5432          0.0.0.0:*               LISTEN     
...

[COMMAND COMPLETED]
Exit Code: 0
```

"I can see several open ports on your system. Port 22 is open for SSH connections and port 5432 suggests you're running PostgreSQL locally..."

## Configuration Requirements

### System Requirements

- Python 3.6+ with subprocess module available
- Shell access with appropriate permissions for commands being executed
- User with abilities to approve and monitor command execution

### Security Considerations

- Commands run with the same permissions as the user running the Agent C framework
- All commands require explicit user approval before execution
- Sensitive commands should be carefully reviewed before approval
- Consider the security implications of environment variables and working directories

## Important Considerations

### Command Security

**Always review commands before approval.** This tool executes commands with your user permissions, which could potentially modify your system or access sensitive data. Never approve commands if you're unsure about what they do.

### Command Context

Commands execute in the context of:
- The working directory specified (or the current directory if none provided)
- The environment variables provided (plus system environment variables)

### Command Timeouts

By default, commands timeout after 60 seconds. For long-running operations, the agent may request a longer timeout or use the streaming option to show progress in real-time.

### Output Handling

For commands with large output, the agent will typically summarize the results. You can request the full output if needed for detailed analysis.