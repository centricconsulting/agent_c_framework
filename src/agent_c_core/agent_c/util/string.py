import os
import re
from typing import List


def to_snake_case(s: str) -> str:
    """
    Converts a given string to snake_case format.

    - Replaces '.' and ' ' with '_'.
    - Removes all non-word characters.
    - Inserts an underscore before uppercase letters and converts them to lowercase.

    Args:
        s (str): The input string to convert to snake_case.

    Returns:
        str: The snake_case version of the input string.
    """
    s = re.sub(r'[\.\s]', '_', s)  # Replace '.' and ' ' with '_'
    s = re.sub(r'\W', '', s)  # Remove non-word characters
    return ''.join(['_' + i.lower() if i.isupper() else i for i in s]).lstrip('_')


def generate_path_tree(start_path: str, prefix: str = "") -> List[str]:
    """
    Generates a directory structure tree as a list of formatted lines indicating structure.

    Args:
        start_path (str): The root directory to start generating the tree from.
        prefix (str, optional): String prefix used to maintain visual alignment during recursion. Defaults to "".

    Returns:
        List[str]: A list of strings representing the recursive directory structure.
    """
    tree_structure: List[str] = []
    contents: List[str] = os.listdir(start_path)  # Get directory contents
    contents.sort()  # Sort for consistency

    for index, name in enumerate(contents):
        path: str = os.path.join(start_path, name)
        connector: str = "├── " if index < len(contents) - 1 else "└── "  # Connector icon for tree-like structure

        tree_structure.append(f"{prefix}{connector}{name}")  # Add current file/folder to the structure

        if os.path.isdir(path):
            # If it's a directory, recursively generate sub-path(s)
            extension = "│   " if index < len(contents) - 1 else "    "  # Extend prefix for consistent formatting
            tree_structure.extend(generate_path_tree(path, prefix + extension))

    return tree_structure
