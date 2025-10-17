#!/usr/bin/env python
"""
Generate a visual representation of model relationships for the API v2 models.

This script creates a Graphviz DOT diagram showing the relationships between
model files and model classes, as well as dependencies between models.

Requirements:
    - graphviz (pip install graphviz)
    - Python 3.8+

Usage:
    python tools/generate_model_graph.py [output_path]

If output_path is not provided, the diagram will be saved as 'docs/api_v2/model_relationships'
"""

import ast
import os
import sys
from typing import Dict, List, Set, Tuple, Any, Optional
from pathlib import Path

try:
    import graphviz
except ImportError:
    print("Error: graphviz package not installed. Run 'pip install graphviz'")
    sys.exit(1)


def find_models_and_imports(file_path: str) -> Tuple[List[str], Dict[str, str], Dict[str, List[str]]]:
    """
    Find all Pydantic models and imports in a file.
    
    Args:
        file_path: Path to the Python file to analyze
        
    Returns:
        Tuple containing:
            - List of model names defined in the file
            - Dict mapping imported names to their source modules
            - Dict mapping model names to field types from other models
    """
    models = []
    imports = {}
    model_fields = {}
    
    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()
    
    try:
        tree = ast.parse(content)
        
        # Find imports
        for node in ast.walk(tree):
            if isinstance(node, ast.ImportFrom) and node.module:
                module = node.module
                for name in node.names:
                    imported_name = name.name
                    as_name = name.asname or imported_name
                    imports[as_name] = f"{module}.{imported_name}"
        
        # Find model definitions and their fields
        for node in ast.walk(tree):
            if isinstance(node, ast.ClassDef):
                is_pydantic_model = False
                
                # Check if it's a Pydantic model
                for base in node.bases:
                    if isinstance(base, ast.Name) and base.id == 'BaseModel':
                        is_pydantic_model = True
                    elif isinstance(base, ast.Attribute) and base.attr == 'BaseModel':
                        is_pydantic_model = True
                    # Check for inheritance from another model
                    elif isinstance(base, ast.Name) and base.id in imports:
                        is_pydantic_model = True
                        
                if is_pydantic_model:
                    models.append(node.name)
                    fields = []
                    
                    # Extract field types
                    for n in node.body:
                        if isinstance(n, ast.AnnAssign) and isinstance(n.annotation, ast.Subscript):
                            if isinstance(n.annotation.value, ast.Name):
                                # Handle Optional[Type]
                                if n.annotation.value.id == 'Optional' and isinstance(n.annotation.slice, ast.Name):
                                    fields.append(n.annotation.slice.id)
                                # Handle List[Type], Dict[str, Type], etc.
                                elif n.annotation.value.id in ('List', 'Dict', 'Set') and isinstance(n.annotation.slice, ast.Name):
                                    fields.append(n.annotation.slice.id)
                        elif isinstance(n, ast.AnnAssign) and isinstance(n.annotation, ast.Name):
                            # Handle simple types: Type
                            fields.append(n.annotation.id)
                    
                    model_fields[node.name] = fields
    except SyntaxError:
        print(f"Syntax error in {file_path}")
    
    return models, imports, model_fields


def find_model_relationships(base_path: str) -> Tuple[Dict[str, List[str]], Dict[str, str], Dict[str, List[str]]]:
    """
    Find relationships between models across all Python files in the models directory.
    
    Args:
        base_path: Base path to the project
        
    Returns:
        Tuple containing:
            - Dict mapping file names to model names defined in them
            - Dict mapping model names to the file they're defined in
            - Dict mapping model names to other models they reference
    """
    file_to_models = {}
    model_to_file = {}
    model_dependencies = {}
    imports_by_file = {}
    fields_by_model = {}
    
    models_dir = os.path.join(base_path, 'src/agent_c_api/api/v2/models')
    
    # Skip if the directory doesn't exist
    if not os.path.exists(models_dir):
        print(f"Error: Models directory not found at {models_dir}")
        sys.exit(1)
    
    # Process each Python file in the models directory
    for file_name in os.listdir(models_dir):
        if file_name.endswith('.py') and file_name != '__init__.py' and file_name != 'registry.py':
            file_path = os.path.join(models_dir, file_name)
            module_name = file_name[:-3]  # Remove .py extension
            
            models, imports, fields = find_models_and_imports(file_path)
            
            file_to_models[module_name] = models
            imports_by_file[module_name] = imports
            
            for model in models:
                model_to_file[model] = module_name
                fields_by_model[model] = fields.get(model, [])
    
    # Build dependency graph based on fields and imports
    for model, fields in fields_by_model.items():
        dependencies = []
        model_file = model_to_file.get(model)
        
        if not model_file:
            continue
            
        imports = imports_by_file.get(model_file, {})
        
        for field in fields:
            # Check if field type is another model
            if field in model_to_file:
                dependencies.append(field)
            # Check if field is imported from another module
            elif field in imports:
                imported_path = imports[field]
                if 'models' in imported_path:
                    # Extract the model name from import
                    parts = imported_path.split('.')
                    if len(parts) > 1:
                        module_part = parts[-2]
                        for src_model, src_file in model_to_file.items():
                            if src_file == module_part and src_model == field:
                                dependencies.append(field)
        
        model_dependencies[model] = dependencies
    
    return file_to_models, model_to_file, model_dependencies


def generate_graph(base_path: str, output_path: Optional[str] = None) -> None:
    """
    Generate a visual representation of model relationships.
    
    Args:
        base_path: Base path to the project
        output_path: Path to save the output graph (without extension)
    """
    if output_path is None:
        output_path = os.path.join(base_path, 'docs/api_v2/model_relationships')
    
    file_to_models, model_to_file, model_dependencies = find_model_relationships(base_path)
    
    # Create graph
    dot = graphviz.Digraph(comment='API v2 Model Relationships', format='png')
    dot.attr(rankdir='LR', size='12,8', ratio='compress')
    
    # Define node styles
    dot.attr('node', shape='box', style='filled', fillcolor='lightblue')
    
    # Add module nodes
    for module_name in file_to_models.keys():
        dot.node(module_name, module_name + '.py', shape='folder', fillcolor='lightgrey')
    
    # Add model nodes and connect to their modules
    for model, module in model_to_file.items():
        # Custom color based on module
        color = 'lightblue'
        if 'session' in module:
            color = '#C5E1A5'  # Light green
        elif 'agent' in module:
            color = '#FFF176'  # Light yellow
        elif 'chat' in module:
            color = '#81D4FA'  # Light blue
        elif 'tool' in module:
            color = '#FFB74D'  # Light orange
        elif 'file' in module:
            color = '#E1BEE7'  # Light purple
        elif 'history' in module:
            color = '#F8BBD0'  # Light pink
        elif 'response' in module:
            color = '#BBDEFB'  # Lighter blue
        elif 'debug' in module:
            color = '#B0BEC5'  # Light grey-blue
            
        dot.node(model, model, fillcolor=color)
        dot.edge(module, model, style='solid', arrowhead='none')
    
    # Add dependency edges between models
    for model, dependencies in model_dependencies.items():
        for dep in dependencies:
            if dep in model_to_file:
                dot.edge(model, dep, style='dashed', color='blue')
    
    # Add special edges for re-exported models
    if 'agent_models' in file_to_models and 'session_models' in file_to_models:
        agent_models = file_to_models['agent_models']
        session_models = file_to_models['session_models']
        
        for model in session_models:
            if model in agent_models:
                dot.edge('session_models', model, style='solid', arrowhead='none')
                dot.edge('agent_models', model, style='dotted', color='red', label='re-exports')
    
    # Add legend
    with dot.subgraph(name='cluster_legend') as legend:
        legend.attr(label='Legend', style='filled', fillcolor='white')
        legend.node('module_node', 'Module file', shape='folder', fillcolor='lightgrey')
        legend.node('model_node', 'Model class', fillcolor='lightblue')
        legend.edge('source', 'target', style='dashed', color='blue', label='references')
        legend.edge('source2', 'target2', style='dotted', color='red', label='re-exports')
        
        # Hide these nodes from the main graph
        for node in ['module_node', 'model_node', 'source', 'target', 'source2', 'target2']:
            legend.node(node, style='invis')
    
    # Render the graph
    try:
        dot.render(output_path, view=False, cleanup=True)
        print(f"Model relationship graph generated at {output_path}.png")
    except Exception as e:
        print(f"Error generating graph: {e}")
        # If graphviz executable is not available, save the DOT file
        with open(f"{output_path}.dot", 'w') as f:
            f.write(dot.source)
        print(f"DOT file saved at {output_path}.dot")


def main():
    """
    Main entry point for the script.
    """
    # Get base path (assumed to be the project root)
    if len(sys.argv) > 1:
        output_path = sys.argv[1]
    else:
        output_path = None
    
    # Get the base path (project root)
    base_path = str(Path(__file__).parent.parent)
    
    print(f"Generating model relationship graph from {base_path}...")
    generate_graph(base_path, output_path)


if __name__ == '__main__':
    main()