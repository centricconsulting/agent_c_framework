import json
from typing import Any, Dict, List, Union
import logging

class ReplaceStringsHelper:
    """
    Helper class for the replace_strings functionality.
    Provides methods to perform string replacements in files with robust error handling.
    Fixes the 'str has no method get' error by properly validating input types.
    """
    
    def __init__(self, logger=None):
        """
        Initialize the ReplaceStringsHelper.
        
        Args:
            logger: Optional logger instance. If not provided, a new logger will be created.
        """
        self.logger = logger or logging.getLogger(__name__)
        
    async def process_replace_strings(
        self, 
        read_function, 
        write_function, 
        path: str, 
        updates: List[Dict[str, str]]
    ) -> Dict[str, Any]:
        """
        Process string replacements in a file with robust error handling.
        
        Args:
            read_function: Async function to read file content
            write_function: Async function to write to file
            path: Path to the file to update
            updates: List of update operations, each containing 'old_string' and 'new_string'
            
        Returns:
            Dict containing the result of the operation with success or error status
        """
        try:
            # Validate inputs
            validation_result = self._validate_updates(updates)
            if validation_result:
                return {'error': validation_result}
                
            # Read file content
            file_content_response = await read_function(path)
            
            # Parse the file content response
            file_content = self._parse_file_content(file_content_response)
            if isinstance(file_content, dict) and 'error' in file_content:
                return file_content
            
            # Process replacements
            updated_content, replacement_stats = self._process_replacements(file_content, updates)
            
            # Write updated content back to file
            write_response = await write_function(path, 'write', updated_content)
            
            # Check write response
            if isinstance(write_response, dict) and 'error' in write_response:
                return write_response
                
            return {
                'success': True,
                'message': f'Successfully updated file {path}',
                'operation': 'update',
                'replacement_stats': replacement_stats
            }
            
        except Exception as e:
            error_msg = f'Error in replace_strings operation: {str(e)}'
            self.logger.error(error_msg)
            return {'error': error_msg}
            
    def _validate_updates(self, updates: Any) -> Union[str, None]:
        """
        Validate the updates parameter and return an error message if invalid.
        
        Args:
            updates: The updates parameter to validate
            
        Returns:
            Error message string if validation fails, None otherwise
        """
        # Check if updates is None
        if updates is None:
            return 'updates parameter is required'
            
        # Check if updates is a list
        if not isinstance(updates, list):
            return f'updates must be a list, got {type(updates).__name__}'
            
        # Check if updates is empty
        if not updates:
            return 'updates list cannot be empty'
            
        # Validate each update operation
        for i, update in enumerate(updates):
            # Check if update is a dictionary
            if not isinstance(update, dict):
                return f'Update operation {i} must be a dictionary, got {type(update).__name__}'
                
            # Check for required keys
            if 'old_string' not in update:
                return f'Update operation {i} is missing required key "old_string"'
                
            if 'new_string' not in update:
                return f'Update operation {i} is missing required key "new_string"'
                
            # Check value types
            if not isinstance(update.get('old_string'), str):
                return f'old_string in update operation {i} must be a string'
                
            if not isinstance(update.get('new_string'), str):
                return f'new_string in update operation {i} must be a string'
                
        # All validations passed
        return None
        
    def _parse_file_content(self, response: Any) -> Union[str, Dict[str, Any]]:
        """
        Parse the file content response from the read operation.
        
        Args:
            response: The response from the read operation
            
        Returns:
            String content or error dictionary
        """
        if isinstance(response, str):
            try:
                # Try to parse as JSON first
                content_json = json.loads(response)
                if isinstance(content_json, dict) and 'error' in content_json:
                    return content_json  # Return the error
                if isinstance(content_json, dict) and 'contents' in content_json:
                    return content_json['contents']  # Extract contents
                return response  # Not a recognized format, return as is
            except json.JSONDecodeError:
                # Not JSON, return as is
                return response
        elif isinstance(response, dict):
            if 'error' in response:
                return response  # Return the error
            if 'contents' in response:
                return response['contents']  # Extract contents
        
        # Default case: return response as is
        return response
        
    def _process_replacements(self, content: str, updates: List[Dict[str, str]]) -> tuple[str, List[Dict[str, Any]]]:
        """
        Process multiple string replacements in the content.
        
        Args:
            content: The original file content
            updates: List of update operations
            
        Returns:
            Tuple of (updated content, replacement stats)
        """
        updated_content = content
        replacement_stats = []
        
        for i, update in enumerate(updates):
            old_string = update.get('old_string', '')
            new_string = update.get('new_string', '')
            
            # Skip invalid operations (should be caught by validation, but just in case)
            if not old_string:
                replacement_stats.append({
                    'operation': i,
                    'status': 'skipped',
                    'reason': 'Empty old_string'
                })
                continue
                
            # Check if old_string exists in the current content
            if old_string not in updated_content:
                replacement_stats.append({
                    'operation': i,
                    'status': 'skipped',
                    'reason': 'Old string not found'
                })
                continue
                
            # Replace the old string with the new string
            occurrences = updated_content.count(old_string)
            updated_content = updated_content.replace(old_string, new_string)
            
            replacement_stats.append({
                'operation': i,
                'status': 'success',
                'replacements': occurrences
            })
            
        return updated_content, replacement_stats