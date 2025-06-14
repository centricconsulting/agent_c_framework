"""Main code explorer module for tree-sitter based code analysis.

This module provides the main CodeExplorer class that serves as the entry
point for AI agents to explore and extract information from code.
"""

from typing import Dict, List, Optional, Any, Union, Type
import os

from ts_tool.core.parser_manager import ParserManager
from ts_tool.languages.base import BaseLanguage
from ts_tool.languages import get_language_implementation, get_supported_languages
from ts_tool.models.code_entity import (
    ClassEntity, FunctionEntity, MethodEntity, 
    VariableEntity, ModuleEntity
)
from ts_tool.models.extraction_result import (
    DetailLevel, ExtractionResult, EntityExtractionResult,
    ModuleExtractionResult, PublicInterfaceResult, CodeSummaryResult
)

def public_method(test_param: str) -> str:
    """A public method for testing purposes.

    Args:
        test_param: A test parameter.

    Returns:
        A string indicating the method was called.
    """
    return f"Public method called with {test_param}"

class CodeExplorer:
    """Main class for exploring and extracting information from code.
    
    This class provides a high-level interface for AI agents to explore
    and extract information from code across multiple programming languages.
    """
    
    def __init__(self):
        """Initialize the code explorer."""
        self.parser_manager = ParserManager()
        self._language_instances: Dict[str, BaseLanguage] = {}
    
    def _get_language_impl(self, language_name: str) -> BaseLanguage:
        """Get a language implementation instance.
        
        Args:
            language_name: The name of the language.
            
        Returns:
            A language implementation instance.
            
        Raises:
            ValueError: If the language is not supported.
        """
        language_name = language_name.lower()
        
        # Return cached instance if available
        if language_name in self._language_instances:
            return self._language_instances[language_name]
        
        # Get the language implementation class
        try:
            impl_class = get_language_implementation(language_name)
        except ValueError as e:
            raise ValueError(f"Unsupported language: {language_name}") from e
        
        # Get parser for language
        parser = self.parser_manager.get_parser(language_name)
        
        # Create language implementation instance
        impl = impl_class(parser)
        
        # Cache the instance
        self._language_instances[language_name] = impl
        
        return impl
    
    def detect_language(self, code: str, filename: Optional[str] = None) -> Optional[str]:
        """Detect the programming language of the given code.
        
        Args:
            code: The source code to analyze.
            filename: Optional filename which may contain an extension hint.
            
        Returns:
            The detected language name, or None if detection failed.
        """
        return self.parser_manager.detect_language(code, filename)
    
    def get_supported_languages(self) -> List[str]:
        """Get a list of supported language names.
        
        Returns:
            A list of supported language names.
        """
        return get_supported_languages()
    
    def explore_code(self, code: str, language: Optional[str] = None, filename: Optional[str] = None) -> ModuleExtractionResult:
        """Explore code and extract its structure.
        
        This method parses the given code and extracts its complete structure,
        including all classes, functions, and variables.
        
        Args:
            code: The source code to explore.
            language: Optional language name. If not provided, will be detected.
            filename: Optional filename which may help with language detection.
            
        Returns:
            A ModuleExtractionResult containing the extracted structure.
        """
        try:
            # Detect language if not provided
            if not language:
                language = self.detect_language(code, filename)
                if not language:
                    return ModuleExtractionResult(
                        successful=False,
                        error_message="Could not detect language. Please specify the language explicitly."
                    )
            
            # Get language implementation
            language_impl = self._get_language_impl(language)
            
            # Parse the code
            tree = language_impl.parse(code)
            
            # Extract public interface
            interface = language_impl.get_public_interface(tree, code)
            
            # Build module entity
            module = ModuleEntity(
                language=language,
                filename=filename,
                docstring=language_impl.get_module_docstring(tree, code),
                imports=language_impl.get_imports(tree, code)
            )
            
            # Add classes
            for class_info in interface['classes']:
                class_entity = ClassEntity(
                    name=class_info['name'],
                    line_range=class_info['line_range'],
                    byte_range=class_info['byte_range'],
                    docstring=class_info.get('docstring'),
                    language=language
                )
                
                # Add methods to class
                for method_info in class_info['methods']:
                    method_entity = MethodEntity(
                        name=method_info['name'],
                        line_range=method_info['line_range'],
                        byte_range=method_info['byte_range'],
                        docstring=method_info.get('docstring'),
                        language=language,
                        signature=method_info['signature'],
                        class_name=class_info['name'],
                        return_type=method_info.get('return_type', '')
                    )
                    class_entity.methods.append(method_entity)
                
                module.classes.append(class_entity)
            
            # Add functions
            for func_info in interface['functions']:
                func_entity = FunctionEntity(
                    name=func_info['name'],
                    line_range=func_info['line_range'],
                    byte_range=func_info['byte_range'],
                    docstring=func_info.get('docstring'),
                    language=language,
                    signature=func_info['signature'],
                    return_type=func_info.get('return_type', '')
                )
                module.functions.append(func_entity)
            
            # Add variables
            for var_info in interface['variables']:
                var_entity = VariableEntity(
                    name=var_info['name'],
                    line_range=var_info['line_range'],
                    byte_range=var_info['byte_range'],
                    language=language,
                    value=var_info.get('value')
                )
                module.variables.append(var_entity)
            
            return ModuleExtractionResult(
                successful=True,
                language=language,
                module=module,
                source_code=code
            )
            
        except Exception as e:
            raise e  # Re-raise the exception for higher-level handling
            return ModuleExtractionResult(
                successful=False,
                error_message=str(e),
                language=language if language else ""
            )
    
    def get_public_interface(self, code: str, language: Optional[str] = None, filename: Optional[str] = None) -> PublicInterfaceResult:
        """Extract the public interface from code.
        
        This method extracts only the public elements of the code, such as
        public classes, functions, and constants.
        
        Args:
            code: The source code to analyze.
            language: Optional language name. If not provided, will be detected.
            filename: Optional filename which may help with language detection.
            
        Returns:
            A PublicInterfaceResult containing the public interface.
        """
        try:
            # Use explore_code to get the full module structure
            module_result = self.explore_code(code, language, filename)
            
            if not module_result.successful:
                return PublicInterfaceResult(
                    successful=False,
                    error_message=module_result.error_message,
                    language=module_result.language
                )
            
            # Filter for public elements only
            result = PublicInterfaceResult(
                successful=True,
                language=module_result.language,
                detail_level=DetailLevel.SIGNATURE
            )
            
            # Add public classes and their public methods
            for cls in module_result.module.classes:
                if not cls.name.startswith('_'):
                    # Only include public methods in the class
                    public_class = ClassEntity(
                        name=cls.name,
                        line_range=cls.line_range,
                        byte_range=cls.byte_range,
                        docstring=cls.docstring,
                        language=cls.language
                    )
                    
                    for method in cls.methods:
                        if not method.name.startswith('_') or method.name == '__init__':
                            public_class.methods.append(method)
                    
                    result.public_classes.append(public_class)
            
            # Add public functions
            for func in module_result.module.functions:
                if not func.name.startswith('_'):
                    result.public_functions.append(func)
            
            # Add public constants
            for var in module_result.module.variables:
                if not var.name.startswith('_'):
                    # Assume all-caps variables are constants
                    if var.name.isupper():
                        var.is_constant = True
                        result.public_constants.append(var)
            
            return result
            
        except Exception as e:
            return PublicInterfaceResult(
                successful=False,
                error_message=str(e),
                language=language if language else ""
            )
    
    def get_entity(self, code: str, entity_type: str, entity_name: str, 
                  detail_level: Union[str, DetailLevel] = DetailLevel.FULL,
                  language: Optional[str] = None, filename: Optional[str] = None) -> EntityExtractionResult:
        """Get a specific entity from code.
        
        This method extracts a specific named entity from the code, such as
        a class, function, or method, with the specified level of detail.
        
        Args:
            code: The source code to analyze.
            entity_type: The type of entity to extract ('class', 'function', 'method', 'variable').
            entity_name: The name of the entity to extract.
            detail_level: The level of detail to include (summary, signature, full).
            language: Optional language name. If not provided, will be detected.
            filename: Optional filename which may help with language detection.
            
        Returns:
            An EntityExtractionResult containing the extracted entity.
        """
        try:
            # Convert string detail level to enum if needed
            if isinstance(detail_level, str):
                detail_level = DetailLevel.from_string(detail_level)
            
            # Use explore_code to get the full module structure
            module_result = self.explore_code(code, language, filename)
            
            if not module_result.successful:
                return EntityExtractionResult(
                    successful=False,
                    error_message=module_result.error_message,
                    language=module_result.language,
                    entity_type=entity_type,
                    entity_name=entity_name
                )
            
            # Get the entity from the module
            entity = module_result.module.get_entity(entity_type, entity_name)
            
            if not entity:
                return EntityExtractionResult(
                    successful=False,
                    error_message=f"Entity not found: {entity_type} '{entity_name}'",
                    language=module_result.language,
                    entity_type=entity_type,
                    entity_name=entity_name
                )
            
            # Get source code for the entity if detail level is FULL
            source_code = None
            if detail_level == DetailLevel.FULL:
                start_byte, end_byte = entity.byte_range
                source_code = code[start_byte:end_byte]
            
            return EntityExtractionResult(
                successful=True,
                language=module_result.language,
                detail_level=detail_level,
                entity=entity,
                entity_type=entity_type,
                entity_name=entity_name,
                source_code=source_code
            )
            
        except Exception as e:
            return EntityExtractionResult(
                successful=False,
                error_message=str(e),
                language=language if language else "",
                entity_type=entity_type,
                entity_name=entity_name
            )
    
    def get_summary(self, code: str, language: Optional[str] = None, filename: Optional[str] = None) -> CodeSummaryResult:
        """Get a high-level summary of code structure.
        
        This method provides a quick overview of the code structure,
        including counts of classes, functions, and variables.
        
        Args:
            code: The source code to analyze.
            language: Optional language name. If not provided, will be detected.
            filename: Optional filename which may help with language detection.
            
        Returns:
            A CodeSummaryResult containing the summary information.
        """
        try:
            # Use explore_code to get the full module structure
            module_result = self.explore_code(code, language, filename)
            
            if not module_result.successful:
                return CodeSummaryResult(
                    successful=False,
                    error_message=module_result.error_message,
                    language=module_result.language
                )
            
            # Count documented entities to determine if code has documentation
            has_documentation = False
            if module_result.module.is_documented:
                has_documentation = True
            else:
                for cls in module_result.module.classes:
                    if cls.is_documented:
                        has_documentation = True
                        break
                    
                    for method in cls.methods:
                        if method.is_documented:
                            has_documentation = True
                            break
                    
                    if has_documentation:
                        break
                
                if not has_documentation:
                    for func in module_result.module.functions:
                        if func.is_documented:
                            has_documentation = True
                            break
            
            # Count lines of code
            total_lines = code.count('\n') + 1
            
            return CodeSummaryResult(
                successful=True,
                language=module_result.language,
                class_count=len(module_result.module.classes),
                function_count=len(module_result.module.functions),
                variable_count=len(module_result.module.variables),
                total_lines=total_lines,
                class_names=[cls.name for cls in module_result.module.classes if not cls.name.startswith('_')],
                function_names=[func.name for func in module_result.module.functions if not func.name.startswith('_')],
                has_documentation=has_documentation
            )
            
        except Exception as e:
            return CodeSummaryResult(
                successful=False,
                error_message=str(e),
                language=language if language else ""
            )