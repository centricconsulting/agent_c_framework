import os
import json
from typing import List, Dict, Any
import aiofiles


def get_session_directory() -> str:
    """
    Get the absolute path to the sessions directory.
    """
    # Assuming project structure: project_root/app/...
    project_root = os.getcwd()
    return os.path.join(project_root, "logs", "sessions")


async def read_jsonl_file(file_path: str) -> List[Dict[str, Any]]:
    """
    Read a JSONL file and return its contents as a list of dictionaries.
    """
    events = []

    try:
        async with aiofiles.open(file_path, mode='r', encoding='utf-8') as f:
            contents = await f.read()
            for line in contents.strip().split('\n'):
                if line.strip():
                    try:
                        event = json.loads(line)
                        events.append(event)
                    except json.JSONDecodeError:
                        print(f"Error decoding JSON line: {line}")
    except Exception as e:
        print(f"Error reading file {file_path}: {e}")

    return events