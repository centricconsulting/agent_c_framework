import asyncio
import json
import re
import shutil
from datetime import datetime
import time
import shlex
import os
import platform
from typing import Dict, Any, List, Optional, Mapping, Tuple, Callable, Protocol
from dataclasses import dataclass

from agent_c.util.logging_utils import LoggingManager
from .validators.base_validator import BasicCommandValidator, CommandValidator, ValidationResult
from .validators.dotnet_validator import DotnetCommandValidator
from .validators.git_validator import GitCommandValidator
from .validators.node_validator import NodeCommandValidator
from .validators.npm_validator import NpmCommandValidator
from .validators.os_basic_validator import OSBasicValidator
from .validators.powershell_validator import PowerShellCommandValidator
from .validators.pytest_validator import PytestCommandValidator

# ANSI stripping Regex
ANSI_RE = re.compile(r'\x1b\[[0-9;?]*[ -/]*[@-~]')

@dataclass
class CommandExecutionResult:
    status: str  # "success", "error", "timeout", "blocked"
    return_code: Optional[int]
    stdout: str
    stderr: str
    command: str
    working_directory: str
    error_message: Optional[str] = None
    duration_ms: Optional[int] = None
    truncated_stdout: Optional[bool] = None
    truncated_stderr: Optional[bool] = None

    # The class modifications I added (to_dict() and to_yaml() methods) are optional conveniences.
    # The dataclass will work with yaml.dump(asdict(result)) without any modifications.

    def to_dict(self) -> Dict[str, Any]:
        """Convert to dictionary for serialization"""
        from dataclasses import asdict
        return asdict(self)

    def to_yaml(self) -> str:
        """Convert to YAML string"""
        import yaml
        return yaml.dump(self.to_dict(), default_flow_style=False, sort_keys=False, allow_unicode=True)

    def to_yaml_capped(
            self,
            max_tokens: int,
            count_tokens: Callable[[str], int],
            *,
            head_lines: int = 200,
            tail_lines: int = 50,
            min_head: int = 20,
            min_tail: int = 10,
    ) -> str:
        """
        Serialize to YAML, trimming stdout/stderr to satisfy a token budget.
        Strategy: keep head+tail for each stream; shrink windows until under budget.
        count_tokens is a function that returns the number of tokens in a string
        """
        import yaml

        def dump(obj: dict) -> str:
            return yaml.dump(obj, default_flow_style=False, sort_keys=False, allow_unicode=True)

        data = self.to_dict()  # includes stdout/stderr/truncated_* fields already
        text = dump(data)
        if count_tokens(text) <= max_tokens:
            return text

        # Prepare line views
        out_lines = (self.stdout or "").splitlines()
        err_lines = (self.stderr or "").splitlines()
        total_out = len(out_lines)
        total_err = len(err_lines)

        def trim(lines, h, t):
            if not lines:
                return "", False
            if len(lines) <= h + t:
                return "\n".join(lines), False
            return "\n".join(lines[:h] + ["… <TRUNCATED> …"] + lines[-t:]), True

        # Iteratively shrink windows
        h, t = head_lines, tail_lines
        while True:
            out_text, out_trunc = trim(out_lines, h, t)
            # stderr gets a smaller window
            err_h = max(min_head // 2, min(h // 4, 40))
            err_t = max(min_tail // 2, min(t // 5, 10))
            err_text, err_trunc = trim(err_lines, err_h, err_t)

            # Annotate streams with a brief note if clipped
            out_note = f"[stdout clipped] showing first {h} and last {t} of {total_out} lines\n" if out_trunc else ""
            err_note = f"[stderr clipped] showing first {err_h} and last {err_t} of {total_err} lines\n" if err_trunc else ""

            d = dict(data)
            d["stdout"] = (out_note + out_text) if out_text or out_note else ""
            d["stderr"] = (err_note + err_text) if err_text or err_note else ""
            d["truncated_stdout"] = bool(out_trunc or d.get("truncated_stdout"))
            d["truncated_stderr"] = bool(err_trunc or d.get("truncated_stderr"))

            text = dump(d)
            if count_tokens(text) <= max_tokens or (h <= min_head and t <= min_tail):
                break

            # shrink windows further
            h = max(min_head, h // 2)
            t = max(min_tail, t // 2)

        # Last resort: drop bodies entirely if still too big
        if count_tokens(text) > max_tokens:
            d = dict(data)
            d["stdout"] = f"(omitted; {total_out} lines; exceeded token budget)"
            d["stderr"] = "(omitted)" if total_err else ""
            d["truncated_stdout"] = True
            d["truncated_stderr"] = bool(total_err)
            text = dump(d)

        return text

    @property
    def is_success(self) -> bool:
        """Check if command executed successfully"""
        return self.status == "success"

    @property
    def is_error(self) -> bool:
        """Check if command failed"""
        return self.status in ["error", "timeout", "blocked"]

    def to_friendly_string(self, success_prefix: str = "OK") -> str:
        """
        Get appropriate response string for tool to return to LLM

        Args:
            success_prefix: Custom success message prefix

        Returns:
            Formatted string appropriate for tool response
        """
        if self.status == "blocked":
            return f"ERROR: Command blocked by security policy: {self.error_message}"

        elif self.status == "success":
            if self.stdout.strip():
                return f"{success_prefix}:\n{self.stdout}"
            else:
                return f"{success_prefix} (no output)"

        elif self.status == "timeout":
            return f"ERROR: Command timed out: {self.error_message}"

        else:
            # error
            msg = self.error_message or "Command failed."
            # Show stderr if present (trim to a few lines)
            tail = (self.stderr or "").strip().splitlines()[-10:]
            err_tail = ("\n" + "\n".join(tail)) if tail else ""
            return f"ERROR: {msg}{err_tail}"

    def get_error_details(self) -> dict[str, str | int | None] | None:
        """Get detailed error information for logging"""
        if self.is_success:
            return None

        return {
            'status': self.status,
            'return_code': self.return_code,
            'command': self.command,
            'working_directory': self.working_directory,
            'stdout': self.stdout[:500] + '...' if len(self.stdout) > 200 else self.stdout,
            'stderr': self.stderr[:500] + '...' if len(self.stderr) > 200 else self.stderr,
            'error_message': self.error_message
        }

class PolicyProvider(Protocol):
    def get_policy(self, base_cmd: str, parts: List[str]) -> Optional[Mapping[str, Any]]: ...


class SecureCommandExecutor:
    """
    Cross-platform secure command execution with whitelist and pattern detection.
    Handles Windows and Unix-like systems with appropriate platform-specific security measures.
    """


    def __init__(self, show_windows: bool = False,
                 log_output: bool = True,
                 default_timeout: int = 30,
                 max_output_size: int = 1024 * 1024,
                 *,
                 policy_provider: Optional[PolicyProvider] = None,
                 default_policies: Optional[Mapping[str, Any]] = None
                 ) -> None:
        # Platform detection - impacts how commands are parsed and executed
        self.platform = platform.system().lower()
        self.is_windows = platform.system().lower().startswith("win") or platform.system().lower().startswith("nt")# False is Unix-like

        # Resource limits
        self.default_timeout = int(default_timeout)
        self.max_output_size = int(max_output_size)

        # Debugging Options
        self.show_windows = show_windows

        # policy providers
        self.policy_provider = policy_provider
        self.default_policies = dict(default_policies or {})

        # Validators - Add your specific validator classes here!
        self.basic_validator = BasicCommandValidator()
        self.validators: Dict[str, CommandValidator] = {
            "git": GitCommandValidator(),
            "pytest": PytestCommandValidator(),
            "os_basic": OSBasicValidator(),
            "node": NodeCommandValidator(),
            "npm": NpmCommandValidator(),
            "dotnet": DotnetCommandValidator(),
            "powershell": PowerShellCommandValidator(),
        }

        # Logging setup
        if log_output:
            self.log_output = log_output
            logging_manager = LoggingManager(self.__class__.__name__)
            self.logger = logging_manager.get_logger()

    @staticmethod
    def has_ansi(s: str) -> bool:
        return bool(ANSI_RE.search(s))

    @staticmethod
    def strip_ansi(s: str) -> str:
        return ANSI_RE.sub('', s)

    @staticmethod
    def _resolve_base(parts: List[str]) -> str:
        # basename without key extensions
        base = os.path.basename(parts[0])
        lower = base.lower()
        for ext in (".exe", ".cmd", ".bat", ".com"):
            if lower.endswith(ext):
                base = base[:-len(ext)]
                break

        # Treat "python -m pytest ..." as pytest
        if base in ("python", "python3") and len(parts) >= 3 and parts[1] == "-m":
            module = parts[2]
            if module == "pytest":
                return "pytest"
        return base

    def _resolve_executable(self, cmd: str, env: Dict[str, str]) -> Optional[str]:
        """Resolve an executable using PATH/PATHEXT from the provided environment."""
        exe = shutil.which(cmd, path=env.get("PATH"))
        if exe:
            return exe
        # Belt-and-suspenders for Windows if PATHEXT is missing/odd
        if self.is_windows and not cmd.lower().endswith((".exe", ".cmd", ".bat", ".com")):
            for ext in (".cmd", ".bat", ".exe", ".com"):
                exe = shutil.which(cmd + ext, path=env.get("PATH"))
                if exe:
                    return exe
        return None

    def _log_command_execution(self, command: str, result: CommandExecutionResult):
        log_entry = {
            'timestamp': datetime.now().isoformat(),
            'command': command,
            'status': result.status,
            'return_code': result.return_code,
            'working_directory': result.working_directory
        }

        # Only log output if explicitly enabled and not too large
        if self.log_output and len(result.stdout) < 1024:
            log_entry['stdout_preview'] = result.stdout[:500]

        self.logger.info(json.dumps(log_entry))

    def register_validator(self, command_name: str, validator: CommandValidator) -> None:
        self.validators[command_name] = validator

    async def execute_command(self, command: str,
                              working_directory: str = None,
                              override_env: Optional[Dict[str, str]] = None,
                              timeout: Optional[int] = None) -> CommandExecutionResult:
        """
        Execute a single command under policy control.

        Args:
          command: e.g., "git status --porcelain"
          working_directory: absolute path; caller owns sandboxing
          override_env: base environment for the process (merged with validator's overrides)
          timeout: fallback timeout if neither per-command nor executor default is provided
        """
        start_ms = time.time()

        # Parse the command into parts - return errors if empty or cannot parse
        try:
            parts = shlex.split(command, posix=not self.is_windows)
        except ValueError as e:
            return self._blocked(command, working_directory, f"Parse error: {e}")

        if not parts:
            return self._blocked(command, working_directory, "Empty command")

        # get the base of the command - return an error if we cannot get the main command
        base = self._resolve_base(parts)
        if not base:
            return self._blocked(command, working_directory, "Unable to resolve base command")

        # Must be in the allowlist of base commands - meaning if they submit a command that we don't have a policy for, it's blocked
        policy = self.policy_provider.get_policy(base, parts)
        if not policy:
            return self._blocked(command, working_directory, f"No policy for '{base}'")

        # Now get the validator for that command - if we don't have a validator, block it.
        # We can create a validator that inherits from BasicCommandValidator if we want to have a generic validator for commands without a custom handler.
        # by forcing us to create one, we ensure we don't just allow arbitrary commands through without review.
        validator_key = (policy.get("validator") or base).lower()
        validator = self.validators.get(validator_key)
        if validator is None:
            return self._blocked(command, working_directory, f"No validator registered for '{base}'")

        # validate the command and its args against the policy. If not allowed, block it.
        vres: ValidationResult = validator.validate(parts, policy)
        if not vres.allowed:
            return self._blocked(command, working_directory, vres.reason or "Validation failed")

        # Override-or-append required flags as per `require_flags`, and normalize verbosity flags. Used by DOTNET validator
        # In future, we may want to do this prior to validator.validate
        if hasattr(validator, "adjust_arguments"):
            try:
                parts = validator.adjust_arguments(parts, policy)
            except Exception as e:
                # (optional) log at debug level; fall back to original parts
                pass

        # Build env
        effective_env = dict(os.environ)
        # Apply policy-provided safe env first
        safe_env = policy.get("safe_env") or {}
        effective_env.update({k: str(v) for (k, v) in safe_env.items()})

        # Let validator override / adjust environment
        if hasattr(validator, "adjust_environment"):
            # type: ignore[attr-defined]
            effective_env = validator.adjust_environment(effective_env, parts, policy)  # noqa

        # Finally apply user-supplied env (last writer wins)
        if override_env:
            effective_env.update(override_env)

        # Ensure Windows can resolve .cmd/.bat if caller didn't supply PATHEXT
        if self.is_windows:
            effective_env.setdefault("PATHEXT", ".COM;.EXE;.BAT;.CMD")

        # Optional: allow PATH_PREPEND if a validator/policy wants to inject it
        path_prepend = effective_env.pop("PATH_PREPEND", None)
        if path_prepend:
            sep = ";" if self.is_windows else ":"
            effective_env["PATH"] = f"{path_prepend}{sep}{effective_env.get('PATH', '')}"

        # Final timeout: arg > per-command > executor default
        effective_timeout = int(vres.timeout or timeout or policy.get("default_timeout", self.default_timeout))

        # Resolve the executable using the effective PATH/PATHEXT
        resolved = self._resolve_executable(parts[0], effective_env)
        if not resolved:
            return self._blocked(command, working_directory, f"Executable not found: {parts[0]}")
        parts[0] = resolved

        # Execute the command (async, shell=False)
        try:
            stdout, stderr, rc, truncated_out, truncated_err = await self._run_subprocess_async(
                parts, cwd=working_directory, env=effective_env, timeout=effective_timeout
            )
            duration = int((time.time() - start_ms) * 1000)
            status = "success" if rc == 0 else "error"
            result = CommandExecutionResult(
                status=status,
                return_code=rc,
                stdout=self.strip_ansi(stdout),
                stderr=self.strip_ansi(stderr),
                command=command,
                working_directory=working_directory,
                error_message=None if rc == 0 else f"Return code {rc}",
                duration_ms=duration,
                truncated_stdout=truncated_out,
                truncated_stderr=truncated_err,
            )
            self._log_command_execution(command, result)
            return result
        except asyncio.TimeoutError:
            duration = int((time.time() - start_ms) * 1000)
            result = CommandExecutionResult(
                status="timeout",
                return_code=None,
                stdout="",
                stderr="",
                command=command,
                working_directory=working_directory,
                error_message=f"Command timed out after {effective_timeout} seconds",
                duration_ms=duration,
            )
            self._log_command_execution(command, result)
            return result
        except FileNotFoundError:
            return self._blocked(command, working_directory, f"Executable not found: {parts[0]}")
        except Exception as e:
            duration = int((time.time() - start_ms) * 1000)
            result = CommandExecutionResult(
                status="error",
                return_code=None,
                stdout="",
                stderr="",
                command=command,
                working_directory=working_directory,
                error_message=f"Execution error: {e}",
                duration_ms=duration,
            )
            self._log_command_execution(command, result)
            return result

    def _blocked(self, command: str, working_dir: Optional[str], reason: str) -> CommandExecutionResult:
        result = CommandExecutionResult(
            status="blocked",
            return_code=None,
            stdout="",
            stderr="",
            command=command,
            working_directory=working_dir or os.getcwd(),
            error_message=reason
        )
        self._log_command_execution(command, result)
        return result

    async def _run_subprocess_async(
            self,
            parts: List[str],
            *,
            cwd: Optional[str],
            env: Dict[str, str],
            timeout: int
    ) -> Tuple[str, str, int, bool, bool]:
        """
        Spawn and capture stdout/stderr with caps to avoid runaway memory.
        """
        # Creation flags for hidden window on Windows (if desired)
        creationflags = 0
        if self.is_windows and not self.show_windows:
            CREATE_NO_WINDOW = 0x08000000
            creationflags |= CREATE_NO_WINDOW

        proc = await asyncio.create_subprocess_exec(
            parts[0],
            *parts[1:],
            cwd=cwd,
            env=env,
            stdout=asyncio.subprocess.PIPE,
            stderr=asyncio.subprocess.PIPE,
            # close_fds defaults True on posix; on Windows Python handles it safely
            creationflags=creationflags if self.is_windows else 0,
        )

        # Read with caps
        max_bytes = self.max_output_size
        out_chunks: List[bytes] = []
        err_chunks: List[bytes] = []
        out_total = 0
        err_total = 0
        truncated_out = False
        truncated_err = False

        async def _read_stream(stream, chunks, total_ref: List[int]) -> Tuple[bool, bytes]:
            truncated = False
            while True:
                chunk = await stream.read(65536)
                if not chunk:
                    break
                if total_ref[0] + len(chunk) > max_bytes:
                    # take only what fits
                    remain = max(0, max_bytes - total_ref[0])
                    if remain:
                        chunks.append(chunk[:remain])
                        total_ref[0] += remain
                    truncated = True
                    # drain the rest quickly but discard
                    while True:
                        more = await stream.read(65536)
                        if not more:
                            break
                    break
                else:
                    chunks.append(chunk)
                    total_ref[0] += len(chunk)
            return truncated, b"".join(chunks)

        async def _gather_both():
            nonlocal out_total, err_total
            (truncated_o, out_bytes), (truncated_e, err_bytes) = await asyncio.wait_for(
                asyncio.gather(
                    _read_stream(proc.stdout, out_chunks, [out_total]),
                    _read_stream(proc.stderr, err_chunks, [err_total]),
                ),
                timeout=timeout
            )
            return truncated_o, out_bytes, truncated_e, err_bytes

        truncated_out, out_bytes, truncated_err, err_bytes = await _gather_both()
        return_code = await asyncio.wait_for(proc.wait(), timeout=timeout)

        stdout = out_bytes.decode(errors="replace")
        stderr = err_bytes.decode(errors="replace")
        return stdout, stderr, return_code, truncated_out, truncated_err
