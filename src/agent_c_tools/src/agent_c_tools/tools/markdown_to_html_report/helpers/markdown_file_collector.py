import logging
from typing import List, Dict, Any, Optional
import json
import re
from pathlib import Path

from ....helpers.path_helper import create_unc_path, ensure_file_extension, os_file_system_path, has_file_extension, normalize_path

logger = logging.getLogger(__name__)

class MarkdownFileCollector:
    """Helper class for collecting and processing markdown files."""

    def __init__(self, workspace_tool):
        self.workspace_tool = workspace_tool

    async def collect_markdown_files(self, root_path: str, files_to_ignore: List[str] = None) -> Dict[str, str]:
        """Collect all markdown files in the workspace directory structure."""
        markdown_files = {}
        files_to_ignore = [f.lower() for f in (files_to_ignore or [])]
        logger.debug(f"Searching for markdown files in {root_path} and subdirectories...")

        # Process a directory to find markdown files recursively
        async def process_directory(dir_path, rel_path=""):
            # Ensure proper UNC path format for directory listing
            normalized_dir_path = normalize_path(dir_path)
            # Ensure trailing slash for consistent path handling
            if not normalized_dir_path.endswith('/'):
                normalized_dir_path += '/'

            try:
                ls_result = await self.workspace_tool.ls(path=normalized_dir_path)
                ls_data = json.loads(ls_result)

                if 'error' in ls_data:
                    logger.error(f"Error listing directory '{normalized_dir_path}': {ls_data['error']}")
                    return

                items = ls_data.get('contents', [])
                logger.debug(f"Found {len(items)} items in {normalized_dir_path}")

                for item_name in items:
                    # Skip hidden files and directories
                    if item_name.startswith('.'):
                        continue

                    if item_name.lower() in files_to_ignore:
                        continue

                    # Create proper UNC item path
                    item_path = f"{normalized_dir_path}{item_name}"

                    # First check if it's a markdown file by extension
                    if has_file_extension(item_name, ['md', 'markdown']):
                        # Add markdown file to our collection
                        file_rel_path = f"{rel_path}/{item_name}" if rel_path else item_name
                        # Normalize path separators in keys
                        normalized_file_rel_path = normalize_path(file_rel_path)
                        markdown_files[normalized_file_rel_path] = item_path
                    else:
                        # Only check if it's a directory - we don't process non-markdown files
                        is_dir_result = await self.workspace_tool.is_directory(path=item_path)
                        is_dir_data = json.loads(is_dir_result)

                        if 'error' not in is_dir_data and is_dir_data.get('is_directory', False):
                            # Recursively process subdirectory
                            new_rel_path = f"{rel_path}/{item_name}" if rel_path else item_name
                            await process_directory(item_path, new_rel_path)
            except Exception as e:
                logger.error(f"Error processing directory {normalized_dir_path}: {e}")

        # Start processing from the root path
        normalized_root_path = normalize_path(root_path).rstrip('/')
        await process_directory(normalized_root_path)

        file_count = len(markdown_files)
        logger.info(f"Found {file_count} markdown files in {normalized_root_path}")
        return markdown_files

    async def build_file_structure(self, root_path: str, markdown_files: Dict[str, str]) -> List[Dict[str, Any]]:
        """Build file structure for the HTML viewer."""
        structure = []
        folders = {}

        # Normalize root path for consistent path handling
        normalized_root_path = normalize_path(root_path)

        # Sort paths to ensure proper order (parent folders before children)
        for rel_path, unc_path in sorted(markdown_files.items()):
            # Double-check that it's a markdown file before processing
            if not has_file_extension(Path(rel_path).name, ['md', 'markdown']):
                logger.debug(f"Skipping non-markdown file: {rel_path}")
                continue

            # Normalize the UNC path for consistent handling
            normalized_unc_path = normalize_path(unc_path)

            # Get path parts for folder hierarchy construction
            path_parts = rel_path.split('/')
            file_name = path_parts[-1]

            # Create folder hierarchy
            current_folders = structure
            current_path = ""

            # Process all path parts except the last one (which is the file)
            for part in path_parts[:-1]:
                if current_path:
                    current_path += f"/{part}"
                else:
                    current_path = part

                # Check if this folder already exists in our structure
                folder = next((f for f in current_folders if f['type'] == 'folder' and f['name'] == part), None)

                if not folder:
                    # Create new folder
                    folder = {
                        'name': part,
                        'type': 'folder',
                        'path': current_path,
                        'children': []
                    }
                    current_folders.append(folder)
                    folders[current_path] = folder['children']

                # Move to the next level
                current_folders = folder['children']

            try:
                # Read the file content
                error, workspace, relative_path = self.workspace_tool.validate_and_get_workspace_path(normalized_unc_path)
                if error:
                    raise ValueError(f"Error reading file: {error}")
                file_content = await workspace.read_internal(relative_path)

                # Process links in the content
                processed_content = await self._process_markdown_links(file_content, rel_path, normalized_root_path,
                                                                       markdown_files)

                # Add the file to the structure
                current_folders.append({
                    'name': file_name,
                    'type': 'file',
                    'path': rel_path,
                    'content': processed_content
                })
            except Exception as e:
                logger.error(f"Error processing file {normalized_unc_path}: {e}")
                # Add the file with error message
                current_folders.append({
                    'name': file_name,
                    'type': 'file',
                    'path': rel_path,
                    'content': f"Error processing file: {str(e)}"
                })

        return structure

    async def _process_markdown_links(self, content: str, file_rel_path: str, root_path: str,
                                      markdown_files: Dict[str, str]) -> str:
        """Convert markdown links to other markdown files into internal viewer links."""
        # Normalize path separators in file_rel_path
        normalized_file_rel_path = file_rel_path.replace('\\', '/')

        # Get the directory of the current file for resolving relative paths
        file_dir = Path(normalized_file_rel_path).parent.as_posix() if '/' in normalized_file_rel_path else ""

        # Regular expression to find markdown links
        link_pattern = r'\[([^\]]+)\]\(([^)]+)\)'

        def replace_link(match):
            link_text = match.group(1)
            link_target = match.group(2)

            # Handle internal document links (just anchors)
            if link_target.startswith('#'):
                anchor_id = link_target.replace("#", "")
                return f'[{link_text}](javascript:void(scrollToAnchor("{anchor_id}")))'

            # Skip already processed javascript links
            if link_target.startswith('javascript:'):
                return match.group(0)

            # Skip external links
            if link_target.startswith(('http://', 'https://')):
                return match.group(0)

            # Split target into file path and anchor if it exists
            parts = link_target.split('#', 1)
            target_file = parts[0]
            anchor = f"#{parts[1]}" if len(parts) > 1 else ""

            try:
                # Normalize target file path separators
                target_file = target_file.replace('\\', '/')

                # Handle UNC paths in links
                if re.match(self.workspace_tool.UNC_PATH_PATTERN, target_file):
                    # If the link target is a UNC path, extract the relative part
                    try:
                        # Extract the part after the workspace
                        unc_parts = target_file.split('/', 3)
                        if len(unc_parts) >= 4:
                            target_path = unc_parts[3]
                        else:
                            # Can't extract relative path, use as is
                            target_path = target_file
                    except Exception:
                        target_path = target_file
                else:
                    # Calculate the full path from the current file's directory
                    if target_file.startswith('/'):
                        # Handle absolute path (relative to root)
                        target_path = target_file[1:]
                    else:
                        # Handle relative path
                        if file_dir:
                            target_path = f"{file_dir}/{target_file}"
                        else:
                            target_path = target_file

                # Normalize path and separators
                target_path = Path(target_path).as_posix()

                # Check if the target is a markdown file in our collection
                if target_path in markdown_files:
                    # Replace with internal link
                    if anchor:
                        return f'[{link_text}](javascript:void(openMarkdownFile("{target_path}", "{anchor}")))'
                    else:
                        return f'[{link_text}](javascript:void(openMarkdownFile("{target_path}")))'

                # If it's potentially a directory link (ending with / or no extension)
                if target_path.endswith('/') or '.' not in target_path.split('/')[-1]:
                    # Ensure path ends with a slash for proper directory prefix matching
                    if not target_path.endswith('/'):
                        target_path = f"{target_path}/"

                    # Check if there are any markdown files in this directory
                    for md_path in markdown_files.keys():
                        if md_path.startswith(target_path):
                            # It's a directory with markdown files, link to the first one
                            return f'[{link_text}](javascript:void(openMarkdownFile("{md_path}")))'
            except Exception as e:
                # If there's an error resolving the path, log it and leave the link unchanged
                logger.error(f"Error processing link '{link_target}' in file '{normalized_file_rel_path}': {e}")

            # If target doesn't exist in our structure or path resolution failed, leave the link unchanged
            return match.group(0)

        # Process all links in the content
        processed_content = re.sub(link_pattern, replace_link, content)
        return processed_content

    async def validate_custom_structure(self, custom_structure: Dict[str, Any], base_path: Optional[str] = None) -> tuple[bool, str]:
        """Validate custom structure format and file accessibility.
        
        Args:
            custom_structure: The custom structure dictionary to validate
            base_path: Optional base path for resolving relative file paths
            
        Returns:
            Tuple of (is_valid, error_message)
        """
        try:
            # Check if custom_structure has required 'items' key
            if not isinstance(custom_structure, dict):
                return False, "Custom structure must be a dictionary"
            
            if 'items' not in custom_structure:
                return False, "Custom structure must contain 'items' key"
            
            items = custom_structure['items']
            if not isinstance(items, list):
                return False, "'items' must be a list"
            
            if len(items) == 0:
                return False, "'items' list cannot be empty"
            
            # Validate each item recursively
            return await self._validate_structure_items(items, base_path, level=0)
            
        except Exception as e:
            logger.error(f"Error validating custom structure: {e}")
            return False, f"Validation error: {str(e)}"
    
    async def _validate_structure_items(self, items: List[Dict[str, Any]], base_path: Optional[str], level: int = 0) -> tuple[bool, str]:
        """Recursively validate structure items."""
        # Prevent excessive nesting
        if level > 20:
            return False, "Maximum nesting depth (20) exceeded"
        
        for i, item in enumerate(items):
            if not isinstance(item, dict):
                return False, f"Item {i} must be a dictionary"
            
            # Check required fields
            if 'type' not in item:
                return False, f"Item {i} missing required 'type' field"
            
            if 'name' not in item:
                return False, f"Item {i} missing required 'name' field"
            
            item_type = item['type']
            item_name = item['name']
            
            # Validate type
            if item_type not in ['file', 'folder']:
                return False, f"Item {i} has invalid type '{item_type}'. Must be 'file' or 'folder'"
            
            # Validate name
            if not isinstance(item_name, str) or not item_name.strip():
                return False, f"Item {i} has invalid name. Must be a non-empty string"
            
            # Type-specific validation
            if item_type == 'file':
                if 'path' not in item:
                    return False, f"File item {i} ('{item_name}') missing required 'path' field"
                
                file_path = item['path']
                if not isinstance(file_path, str) or not file_path.strip():
                    return False, f"File item {i} ('{item_name}') has invalid path. Must be a non-empty string"
                
                # Validate file exists and is accessible
                resolved_path = self._resolve_file_path(file_path, base_path)
                is_valid, error_msg = await self._validate_file_exists(resolved_path, item_name)
                if not is_valid:
                    return False, error_msg
                    
            elif item_type == 'folder':
                if 'children' not in item:
                    return False, f"Folder item {i} ('{item_name}') missing required 'children' field"
                
                children = item['children']
                if not isinstance(children, list):
                    return False, f"Folder item {i} ('{item_name}') children must be a list"
                
                # Recursively validate children
                if children:  # Allow empty folders
                    is_valid, error_msg = await self._validate_structure_items(children, base_path, level + 1)
                    if not is_valid:
                        return False, f"In folder '{item_name}': {error_msg}"
        
        return True, ""
    
    def _resolve_file_path(self, file_path: str, base_path: Optional[str]) -> str:
        """Resolve file path to UNC format."""
        # If already UNC path, use as-is
        if file_path.startswith('//'):
            return file_path
        
        # If absolute path starting with /, treat as workspace-relative
        if file_path.startswith('/'):
            # Extract workspace from base_path if available
            if base_path and base_path.startswith('//'):
                workspace = base_path.split('/')[2]
                return f"//{workspace}{file_path}"
            else:
                return file_path
        
        # Relative path - resolve against base_path
        if base_path:
            if base_path.startswith('//'):
                # UNC base path - extract workspace and base directory
                base_parts = base_path.split('/', 3)  # ['', '', 'workspace', 'path']
                workspace = base_parts[2]
                
                # Check if file_path already starts with the base directory to avoid duplication
                if len(base_parts) >= 4:
                    base_dir = base_parts[3]
                    if file_path.startswith(f"{base_dir}/"):
                        # File path already includes base directory, use workspace root
                        return f"//{workspace}/{file_path}"
                
                # Normal case: combine base_path with file_path
                return f"{base_path.rstrip('/')}/{file_path}"
            else:
                # Regular base path - check for duplication
                if file_path.startswith(f"{base_path}/"):
                    # File path already includes base path, don't duplicate
                    return file_path
                return f"{base_path.rstrip('/')}/{file_path}"
        
        # No base path, assume workspace root
        return file_path
    
    async def _validate_file_exists(self, file_path: str, item_name: str) -> tuple[bool, str]:
        """Validate that a file exists and is accessible."""
        try:
            # Try to read the file to verify it exists and is accessible
            error, workspace, relative_path = self.workspace_tool.validate_and_get_workspace_path(file_path)
            if error:
                raise ValueError(f"Error reading file: {error}")
            result = await workspace.read_internal(relative_path)
            
            # Check for error response
            if result.startswith('{"error":'):
                error_data = json.loads(result)
                return False, f"File '{item_name}' (path: {file_path}) is not accessible: {error_data['error']}"
            
            # Verify it's a markdown file
            if not has_file_extension(file_path, ['md', 'markdown']):
                return False, f"File '{item_name}' (path: {file_path}) is not a markdown file (.md or .markdown extension required)"
            
            return True, ""
            
        except Exception as e:
            return False, f"Error accessing file '{item_name}' (path: {file_path}): {str(e)}"
    
    async def collect_custom_files(self, custom_structure: Dict[str, Any], base_path: Optional[str] = None) -> Dict[str, str]:
        """Collect files from custom structure.
        
        Args:
            custom_structure: The custom structure dictionary
            base_path: Optional base path for resolving relative file paths
            
        Returns:
            Dictionary mapping custom paths to UNC paths
        """
        markdown_files = {}
        
        # Validate structure first
        is_valid, error_msg = await self.validate_custom_structure(custom_structure, base_path)
        if not is_valid:
            raise ValueError(f"Invalid custom structure: {error_msg}")
        
        # Collect files from structure
        await self._collect_files_from_items(custom_structure['items'], markdown_files, base_path)
        
        logger.info(f"Collected {len(markdown_files)} files from custom structure")
        return markdown_files
    
    async def _collect_files_from_items(self, items: List[Dict[str, Any]], markdown_files: Dict[str, str], base_path: Optional[str], current_path: str = "") -> None:
        """Recursively collect files from structure items."""
        for item in items:
            item_type = item['type']
            item_name = item['name']
            
            if item_type == 'file':
                file_path = item['path']
                resolved_path = self._resolve_file_path(file_path, base_path)
                
                # Create a custom path for this file (used as key)
                custom_path = f"{current_path}/{item_name}" if current_path else item_name
                markdown_files[custom_path] = resolved_path
                
            elif item_type == 'folder':
                # Recursively process children
                folder_path = f"{current_path}/{item_name}" if current_path else item_name
                await self._collect_files_from_items(item['children'], markdown_files, base_path, folder_path)
    
    async def build_custom_structure(self, custom_structure: Dict[str, Any], base_path: Optional[str] = None) -> List[Dict[str, Any]]:
        """Build file structure for HTML viewer from custom hierarchy.
        
        Args:
            custom_structure: The custom structure dictionary
            base_path: Optional base path for resolving relative file paths
            
        Returns:
            List of structure items for HTML viewer
        """
        # First collect all files to create path mappings
        markdown_files = await self.collect_custom_files(custom_structure, base_path)
        
        # Build the structure
        structure = []
        await self._build_structure_from_items(custom_structure['items'], structure, markdown_files, base_path)
        
        return structure
    
    async def _build_structure_from_items(self, items: List[Dict[str, Any]], structure: List[Dict[str, Any]], markdown_files: Dict[str, str], base_path: Optional[str], current_path: str = "") -> None:
        """Recursively build structure from custom items."""
        for item in items:
            item_type = item['type']
            item_name = item['name']
            
            if item_type == 'file':
                file_path = item['path']
                resolved_path = self._resolve_file_path(file_path, base_path)
                custom_path = f"{current_path}/{item_name}" if current_path else item_name
                
                try:
                    # Read the file content
                    error, workspace, relative_path = self.workspace_tool.validate_and_get_workspace_path(resolved_path)
                    if error:
                        raise ValueError(f"Error reading file: {error}")
                    file_content = await workspace.read_internal(relative_path)

                    
                    # Process links in the content using custom path mappings
                    processed_content = await self._process_custom_markdown_links(file_content, custom_path, markdown_files)

                    # Ensure the custom path has .md extension if it's a file or the html viewer won't render the file properly
                    if item_type == 'file' and not custom_path.endswith('.md'):
                        custom_path = f"{custom_path}.md"

                    # Ensure the file has a markdown extension
                    item_name = ensure_file_extension(item_name, '.md')
                    # ensure it doesn't have two .. in it
                    item_name = item_name.replace('..md', '.md')

                    # Add the file to the structure
                    structure.append({
                        'name': item_name,
                        'type': 'file',
                        'path': custom_path,  # Use custom path for navigation
                        'content': processed_content
                    })
                    
                except Exception as e:
                    logger.error(f"Error processing file {resolved_path}: {e}")
                    # Add the file with error message
                    structure.append({
                        'name': item_name,
                        'type': 'file',
                        'path': custom_path,
                        'content': f"Error processing file: {str(e)}"
                    })
                    
            elif item_type == 'folder':
                # Create folder structure
                folder_path = f"{current_path}/{item_name}" if current_path else item_name
                folder_item = {
                    'name': item_name,
                    'type': 'folder',
                    'path': folder_path,
                    'children': []
                }
                
                # Recursively process children
                await self._build_structure_from_items(item['children'], folder_item['children'], markdown_files, base_path, folder_path)
                
                structure.append(folder_item)
    
    async def _process_custom_markdown_links(self, content: str, file_custom_path: str, markdown_files: Dict[str, str]) -> str:
        """Process markdown links for custom structure with custom path mappings."""
        # Get the directory of the current file for resolving relative paths
        file_dir = Path(file_custom_path).parent.as_posix() if '/' in file_custom_path else ""
        
        # Regular expression to find markdown links
        link_pattern = r'\[([^\]]+)\]\(([^)]+)\)'
        
        def replace_link(match):
            link_text = match.group(1)
            link_target = match.group(2)
            
            # Handle internal document links (just anchors)
            if link_target.startswith('#'):
                anchor_id = link_target.replace("#", "")
                return f'[{link_text}](javascript:void(scrollToAnchor("{anchor_id}")))'  
            
            # Skip already processed javascript links
            if link_target.startswith('javascript:'):
                return match.group(0)
            
            # Skip external links
            if link_target.startswith(('http://', 'https://')):
                return match.group(0)
            
            # Split target into file path and anchor if it exists
            parts = link_target.split('#', 1)
            target_file = parts[0]
            anchor = f"#{parts[1]}" if len(parts) > 1 else ""
            
            try:
                # Normalize target file path separators
                target_file = target_file.replace('\\', '/')
                
                # Try to resolve the link target to a custom path
                resolved_custom_path = self._resolve_custom_link_target(target_file, file_custom_path, markdown_files)
                
                if resolved_custom_path:
                    # Replace with internal link using custom path
                    if anchor:
                        return f'[{link_text}](javascript:void(openMarkdownFile("{resolved_custom_path}", "{anchor}")))'  
                    else:
                        return f'[{link_text}](javascript:void(openMarkdownFile("{resolved_custom_path}")))'  
                        
            except Exception as e:
                logger.error(f"Error processing link '{link_target}' in file '{file_custom_path}': {e}")
            
            # If target doesn't resolve, leave the link unchanged
            return match.group(0)
        
        # Process all links in the content
        processed_content = re.sub(link_pattern, replace_link, content)
        return processed_content
    
    def _resolve_custom_link_target(self, target_file: str, current_custom_path: str, markdown_files: Dict[str, str]) -> Optional[str]:
        """Resolve a link target to a custom path in the structure."""
        # Get current file directory for relative path resolution
        current_dir = Path(current_custom_path).parent.as_posix() if '/' in current_custom_path else ""
        
        # Try different resolution strategies
        
        # 1. Direct match to custom path (exact match)
        if target_file in markdown_files:
            return target_file
        
        # 2. Try relative path resolution from current directory
        if current_dir and not target_file.startswith('/'):
            relative_path = f"{current_dir}/{target_file}"
            if relative_path in markdown_files:
                return relative_path
        
        # 3. Try matching by filename only (search all custom paths)
        target_filename = Path(target_file).name
        for custom_path in markdown_files.keys():
            if Path(custom_path).name == target_filename:
                return custom_path
        
        # 4. Try matching by original file path (check UNC paths)
        for custom_path, unc_path in markdown_files.items():
            # Extract relative path from UNC path for comparison
            if unc_path.startswith('//'):
                try:
                    unc_parts = unc_path.split('/', 3)
                    if len(unc_parts) >= 4:
                        unc_relative = unc_parts[3]
                        if target_file == unc_relative or target_file.endswith(f"/{unc_relative}"):
                            return custom_path
                except Exception:
                    continue
        
        # 5. Try directory-based matching (if target ends with / or has no extension)
        if target_file.endswith('/') or '.' not in target_file.split('/')[-1]:
            search_prefix = target_file.rstrip('/') + '/'
            for custom_path in markdown_files.keys():
                if custom_path.startswith(search_prefix):
                    return custom_path
        
        return None