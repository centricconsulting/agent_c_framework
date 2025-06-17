#!/usr/bin/env python3
"""
Manual Workspace Planning HTML Report Generator

This standalone script takes an absolute path to a workspace planning YAML file
and generates an interactive HTML report in the same directory.

Usage:
    python z_manual_tool.py /path/to/your/planning_file.yaml [plan_id]

Arguments:
    yaml_file_path: Absolute path to the YAML file containing workspace planning data
    plan_id: Optional plan ID to export (if not provided, uses current_plan or first plan)

The script expects YAML files in the format:
```yaml
_plans:
  plan_id:
    title: "Plan Title"
    description: "Plan Description"
    tasks:
      task_id:
        title: "Task Title"
        description: "Task Description"
        priority: "high"
        # ... other task fields
current_plan: plan_id
```

Output:
    Creates an HTML file in the same directory as the input YAML file
    with the name: {original_filename}_report.html
"""

import os
import sys
import yaml
import argparse
from pathlib import Path
from datetime import datetime
from typing import Dict, Any, Optional

# Import the HTML converter
from html_converter import PlanHTMLConverter


def load_yaml_file(file_path: str) -> Dict[str, Any]:
    """
    Load and parse a YAML file.
    
    Args:
        file_path: Absolute path to the YAML file
        
    Returns:
        Parsed YAML content as dictionary
        
    Raises:
        FileNotFoundError: If the file doesn't exist
        yaml.YAMLError: If the file is not valid YAML
    """
    if not os.path.exists(file_path):
        raise FileNotFoundError(f"YAML file not found: {file_path}")
    
    try:
        with open(file_path, 'r', encoding='utf-8') as file:
            content = file.read()
            return yaml.safe_load(content)
    except yaml.YAMLError as e:
        raise yaml.YAMLError(f"Invalid YAML file: {e}")


def validate_yaml_structure(data: Dict[str, Any]) -> None:
    """
    Validate that the YAML has the expected workspace planning structure.
    
    Args:
        data: Parsed YAML data
        
    Raises:
        ValueError: If the structure is invalid
    """
    if not isinstance(data, dict):
        raise ValueError("YAML root must be a dictionary")
    
    if '_plans' not in data:
        raise ValueError("YAML must contain '_plans' section")
    
    plans = data['_plans']
    if not isinstance(plans, dict) or not plans:
        raise ValueError("'_plans' must be a non-empty dictionary")
    
    # Validate at least one plan has tasks
    has_tasks = False
    for plan_id, plan_data in plans.items():
        if isinstance(plan_data, dict) and 'tasks' in plan_data:
            tasks = plan_data['tasks']
            if isinstance(tasks, dict) and tasks:
                has_tasks = True
                break
    
    if not has_tasks:
        raise ValueError("At least one plan must contain tasks")


def get_plan_id_from_data(data: Dict[str, Any], requested_plan_id: Optional[str] = None) -> str:
    """
    Determine which plan ID to use for the report.
    
    Args:
        data: Parsed YAML data
        requested_plan_id: Optional specific plan ID requested by user
        
    Returns:
        Plan ID to use for the report
        
    Raises:
        ValueError: If the requested plan ID is not found
    """
    plans = data['_plans']
    
    # If specific plan requested, validate it exists
    if requested_plan_id:
        if requested_plan_id not in plans:
            available_plans = list(plans.keys())
            raise ValueError(f"Plan ID '{requested_plan_id}' not found. Available plans: {available_plans}")
        return requested_plan_id
    
    # Use current_plan if specified
    current_plan = data.get('current_plan')
    if current_plan and current_plan in plans:
        return current_plan
    
    # Use first available plan
    return list(plans.keys())[0]


def generate_output_path(input_path: str, plan_id: str) -> str:
    """
    Generate the output HTML file path in the same directory as the input file.
    
    Args:
        input_path: Path to the input YAML file
        plan_id: Plan ID being exported
        
    Returns:
        Path for the output HTML file
    """
    input_file = Path(input_path)
    input_dir = input_file.parent
    input_stem = input_file.stem  # filename without extension
    
    # Create output filename: {original_name}_{plan_id}_report.html
    output_filename = f"{input_stem}_{plan_id}_report.html"
    output_path = input_dir / output_filename
    
    return str(output_path)


def generate_html_report(yaml_file_path: str, plan_id: Optional[str] = None) -> str:
    """
    Generate an HTML report from a workspace planning YAML file.
    
    Args:
        yaml_file_path: Absolute path to the YAML file
        plan_id: Optional plan ID to export
        
    Returns:
        Path to the generated HTML file
        
    Raises:
        Various exceptions for file/validation errors
    """
    print(f"Loading YAML file: {yaml_file_path}")
    
    # Load and validate the YAML file
    data = load_yaml_file(yaml_file_path)
    validate_yaml_structure(data)
    
    # Determine which plan to export
    selected_plan_id = get_plan_id_from_data(data, plan_id)
    print(f"Using plan ID: {selected_plan_id}")

    
    # Generate HTML using the converter
    print("Converting to HTML...")
    converter = PlanHTMLConverter()
    html_content = converter.convert_plan_to_html(data)
    
    # Generate output path
    output_path = generate_output_path(yaml_file_path, selected_plan_id)
    
    # Write the HTML file
    print(f"Writing HTML report to: {output_path}")
    with open(output_path, 'w', encoding='utf-8') as output_file:
        output_file.write(html_content)
    
    return output_path


def main():
    """Main entry point for the script."""
    parser = argparse.ArgumentParser(
        description="Generate HTML reports from workspace planning YAML files",
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
Examples:
    python z_manual_tool.py /home/user/my_project_plan.yaml
    python z_manual_tool.py /home/user/my_project_plan.yaml specific_plan_id
    python z_manual_tool.py C:\\Users\\user\\Documents\\plan.yaml
        """
    )
    
    parser.add_argument(
        'yaml_file_path',
        help='Absolute path to the workspace planning YAML file'
    )
    
    parser.add_argument(
        'plan_id',
        nargs='?',
        help='Optional plan ID to export (if not provided, uses current_plan or first plan)'
    )
    
    parser.add_argument(
        '--verbose', '-v',
        action='store_true',
        help='Enable verbose output'
    )
    
    args = parser.parse_args()
    
    try:
        # Validate input path is absolute
        if not os.path.isabs(args.yaml_file_path):
            print(f"Error: Please provide an absolute path. Got: {args.yaml_file_path}")
            sys.exit(1)
        
        # Generate the report
        output_path = generate_html_report(args.yaml_file_path, args.plan_id)
        
        print(f"\n✅ Success! HTML report generated:")
        print(f"   📄 Input:  {args.yaml_file_path}")
        print(f"   📊 Output: {output_path}")
        print(f"\n🌐 Open the HTML file in your browser to view the interactive report.")
        
    except FileNotFoundError as e:
        print(f"❌ File Error: {e}")
        sys.exit(1)
    except yaml.YAMLError as e:
        print(f"❌ YAML Error: {e}")
        sys.exit(1)
    except ValueError as e:
        print(f"❌ Validation Error: {e}")
        sys.exit(1)
    except Exception as e:
        print(f"❌ Unexpected Error: {e}")
        if args.verbose:
            import traceback
            traceback.print_exc()
        sys.exit(1)


if __name__ == "__main__":
    main()