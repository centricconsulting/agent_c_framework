import os
from typing import List, Mapping, Any, Dict

from agent_c.util.logging_utils import LoggingManager
from .base_validator import ValidationResult
from .path_safety import is_within_workspace, looks_like_path, extract_file_part


class PytestCommandValidator:
    SAFE_FLAGS = {
        "-q", "-k", "-x", "-s", "-vv", "-v",
        "--maxfail", "--disable-warnings", "--maxfail=1",
        "--maxfail=2", "--maxfail=3", "--tb=short", "--tb=long",
        "--no-header", "--no-summary", "--tb=no", "--tb=line"
    }

    def validate(self, parts: List[str], policy: Mapping[str, Any]) -> ValidationResult:
        # parts[0] is 'pytest'
        if not parts:
            return ValidationResult(False, "empty argv")

        def stem(p: str) -> str:
            return os.path.splitext(os.path.basename(p))[0].lower()

        # Support `pytest …` and `python -m pytest …`
        start = None
        s0 = stem(parts[0])
        if s0 == "pytest":
            start = 1
        elif s0 in ("python", "python3", "py") and len(parts) >= 3 and parts[1] == "-m" and parts[
            2].lower() == "pytest":
            start = 3
        if start is None:
            return ValidationResult(False, "not a pytest invocation")


        used_flags = [p for p in parts[1:] if p.startswith("-")]
        for f in used_flags:
            base = f.split("=", 1)[0]
            if (base not in self.SAFE_FLAGS) and (f not in self.SAFE_FLAGS):
                return ValidationResult(False, f"Flag not allowed: {f}")

        argv = parts[start:]

        # --- Policy knobs & defaults
        allowed_flags = set(policy.get("allowed_flags") or policy.get("flags") or list(self.SAFE_FLAGS))
        # flags that *take* a value (either via "=…" or as the next token)
        value_flags = {"-k", "-m", "--maxfail", "-n", "--max-workers", "--tb", "--durations"}
        workspace_root = policy.get("workspace_root") or os.getcwd()
        max_selectors = int(policy.get("max_selector_args", 50))
        max_argv_len = int(policy.get("max_argv_len", 4096))

        # --- Helpers
        def _flag_base(tok: str) -> str:
            return tok.split("=", 1)[0]

        def _next_is_value(tokens, idx: int) -> bool:
            return idx + 1 < len(tokens) and not tokens[idx + 1].startswith("-")

        def _looks_like_path_or_nodeid(tok: str) -> bool:
            if not tok or tok.startswith("-"):
                return False
            return (
                    tok.endswith(".py")
                    or "::" in tok
                    or "/" in tok
                    or "\\" in tok
                    or (":" in tok)  # file:line or Windows drive; validated by fence below
            )

        # --- Scan argv: validate flags & collect positionals safely
        positionals: List[str] = []
        i = 0
        total_len = sum(len(t) + 1 for t in argv)
        if total_len > max_argv_len:
            return ValidationResult(False, f"argv too long ({total_len} > {max_argv_len})")

        while i < len(argv):
            t = argv[i]
            if t.startswith("-"):
                base = _flag_base(t)
                if base not in allowed_flags:
                    return ValidationResult(False, f"flag not permitted: {t}")
                if base in value_flags and "=" not in t:
                    if not _next_is_value(argv, i):
                        return ValidationResult(False, f"flag expects a value: {base}")
                    # consume value token
                    i += 2
                    continue
                i += 1
                continue

            # Positional token; treat as potential file/nodeid and fence it
            if _looks_like_path_or_nodeid(t):
                file_part = extract_file_part(t)
                if not is_within_workspace(workspace_root, file_part):
                    return ValidationResult(False, f"unsafe path outside workspace: {t}")
                positionals.append(t)
            else:
                # Non-path positional (rare with pytest, but allow)
                positionals.append(t)
            i += 1

        if len(positionals) > max_selectors:
            return ValidationResult(False, f"too many selectors ({len(positionals)} > {max_selectors})")

        timeout = policy.get("default_timeout")
        return ValidationResult(True, "OK", timeout=timeout, policy_spec=policy)

    def adjust_environment(self, base_env: Dict[str, str], parts: List[str], policy: Mapping[str, Any]) -> Dict[str, str]:
        """
        Adjust environment for pytest execution.
        This includes detecting and activating virtual environments.

        Note: This is now an instance method to access the executor for venv detection.
        """

        logger = LoggingManager(__name__).get_logger()
        env = dict(base_env)

        # Try to detect and activate virtual environment if enabled
        detect_venv = policy.get("detect_venv", True)  # Default: enabled
        logger.debug(f"pytest adjust_environment: detect_venv={detect_venv}")
        
        if detect_venv:
            cwd = env.get("CWD") or os.getcwd()
            workspace_root = env.get("WORKSPACE_ROOT") or policy.get("workspace_root")
            logger.debug(f"pytest adjust_environment: cwd={cwd}, workspace_root={workspace_root}")

            # Get the executor instance from the validator if available
            # The executor will pass itself when calling adjust_environment
            from ..secure_command_executor import SecureCommandExecutor
            executor = getattr(self, '_executor', None)
            logger.debug(f"pytest adjust_environment: executor={executor}, has_executor={executor is not None}")
            
            if executor and isinstance(executor, SecureCommandExecutor):
                logger.debug(f"pytest adjust_environment: calling find_and_prepare_venv({cwd}, {workspace_root})")
                venv_settings = executor.find_and_prepare_venv(cwd, workspace_root)
                logger.debug(f"pytest adjust_environment: venv_settings={venv_settings}")
                
                if venv_settings:
                    logger.info(f"Detected and activating virtual environment: {venv_settings.get('VIRTUAL_ENV')}")
                    # Merge venv settings, but don't override existing values
                    for key, value in venv_settings.items():
                        if value is None:
                            # None means unset the variable
                            env.pop(key, None)
                            logger.debug(f"pytest adjust_environment: unsetting {key}")
                        elif key not in env or key == "PATH_PREPEND":
                            # Always set PATH_PREPEND, and set others if not present
                            env[key] = value
                            logger.debug(f"pytest adjust_environment: setting {key}={value}")
                else:
                    logger.debug("pytest adjust_environment: no venv found")
            else:
                logger.warning(f"pytest adjust_environment: executor not available or wrong type (executor={type(executor).__name__ if executor else 'None'})")

        # Only disable plugin autoload if explicitly requested in policy
        # Note: Disabling plugins will prevent pytest-cov and other plugins from working
        if policy.get("disable_plugin_autoload", False):
            env["PYTEST_DISABLE_PLUGIN_AUTOLOAD"] = "1"

        # Apply any policy-defined environment overrides
        overrides = policy.get("env_overrides") or {}
        env.update(overrides)

        return env