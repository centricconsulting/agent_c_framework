#!/usr/bin/env python
"""
Pre-commit hook to check for Pydantic model duplication in the codebase.

This script scans Python files for Pydantic model class definitions and checks for any
duplicate class names across different files. It's intended to be used as a pre-commit
hook to prevent accidental model duplication.

Usage:
    python check_model_duplication.py [path]

If path is not specified, the script will check the src/agent_c_api directory.
"""

import ast
import os
import sys
from collections import defaultdict
from typing import Dict, List, Set, Tuple


def find_pydantic_models(file_path: str) -> List[Tuple[str, str]]:
    """Find all Pydantic model class names in a file.
    
    Args:
        file_path: Path to the Python file to check
        
    Returns:
        List of tuples (class_name, file_path) for each Pydantic model
    """
    models = []
    
    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()
    
    try:
        tree = ast.parse(content)
        for node in ast.walk(tree):
            if isinstance(node, ast.ClassDef):
                # Check if it's a Pydantic model by looking at base classes
                is_pydantic_model = False
                for base in node.bases:
                    if isinstance(base, ast.Name) and base.id == 'BaseModel':
                        is_pydantic_model = True
                    elif isinstance(base, ast.Attribute) and base.attr == 'BaseModel':
                        is_pydantic_model = True
                
                if is_pydantic_model:
                    models.append((node.name, file_path))
    except SyntaxError:
        print(f"Syntax error in {file_path}")
    
    return models


def find_duplicate_models(base_path: str) -> Dict[str, List[str]]:
    """Find duplicate model definitions across all Python files in the path.
    
    Args:
        base_path: Base directory to scan
        
    Returns:
        Dictionary mapping model names to lists of file paths where they're defined
    """
    model_defs: Dict[str, List[str]] = defaultdict(list)
    
    for root, _, files in os.walk(base_path):
        for file in files:
            if file.endswith('.py'):
                file_path = os.path.join(root, file)
                
                # Skip the registry and __init__ files
                if 'registry.py' in file_path or '__init__.py' in file_path:
                    continue
                
                for model_name, path in find_pydantic_models(file_path):
                    model_defs[model_name].append(path)
    
    # Filter out non-duplicates
    return {model: paths for model, paths in model_defs.items() if len(paths) > 1}


def verify_re_exports(duplicates: Dict[str, List[str]]) -> bool:
    """Verify that duplicate models use the re-export pattern.
    
    Args:
        duplicates: Dictionary mapping model names to lists of file paths
        
    Returns:
        True if all duplicates follow the re-export pattern, False otherwise
    """
    all_valid = True
    
    for model_name, file_paths in duplicates.items():
        # Check for re-export pattern in each file except one (the source)
        export_found = False
        source_found = False
        
        for file_path in file_paths:
            with open(file_path, 'r', encoding='utf-8') as f:
                content = f.read()
            
            # Check for re-export from another file
            if f"from ." in content and f" {model_name}" in content:
                export_found = True
            else:
                # This is likely the source definition
                source_found = True
        
        # If we don't have both a source and an export, it's invalid
        if not (export_found and source_found):
            print(f"Warning: Model '{model_name}' appears to be duplicated without using the re-export pattern")
            for path in file_paths:
                print(f"  - {path}")
            all_valid = False
    
    return all_valid


def main():
    """Main entry point for the script."""
    # Determine the path to check
    if len(sys.argv) > 1:
        base_path = sys.argv[1]
    else:
        base_path = 'src/agent_c_api'
    
    # Ensure the path exists
    if not os.path.exists(base_path):
        print(f"Error: Path '{base_path}' does not exist")
        sys.exit(1)
    
    print(f"Checking for duplicate models in {base_path}...")
    
    # Find duplicate models
    duplicates = find_duplicate_models(base_path)
    
    if not duplicates:
        print("Success: No duplicate model definitions found.")
        sys.exit(0)
    
    # Check if duplicates follow the re-export pattern
    if verify_re_exports(duplicates):
        print("Success: Duplicate models follow the re-export pattern.")
        sys.exit(0)
    else:
        print("Error: Duplicate model definitions found that don't follow the re-export pattern.")
        sys.exit(1)


if __name__ == '__main__':
    main()