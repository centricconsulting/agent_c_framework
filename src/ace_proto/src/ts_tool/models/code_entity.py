"""Code entity models for representing code structure.

This module defines models for representing code entities like
classes, functions, methods, and variables in a structured way.
"""

from dataclasses import dataclass, field
from typing import List, Optional, Tuple, Dict, Any, Union


@dataclass
class CodeEntity:
    """Base class for all code entities.
    
    This class represents common properties shared by all code entities
    such as name, location, and documentation.
    """
    name: str
    line_range: Tuple[Tuple[int, int], Tuple[int, int]]
    byte_range: Tuple[int, int]
    docstring: Optional[str] = None
    language: str = ""
    
    @property
    def start_line(self) -> int:
        """Get the starting line number."""
        return self.line_range[0][0]
    
    @property
    def end_line(self) -> int:
        """Get the ending line number."""
        return self.line_range[1][0]
    
    @property
    def start_byte(self) -> int:
        """Get the starting byte index."""
        return self.byte_range[0]
    
    @property
    def end_byte(self) -> int:
        """Get the ending byte index."""
        return self.byte_range[1]
    
    @property
    def is_documented(self) -> bool:
        """Check if the entity has documentation."""
        return bool(self.docstring and self.docstring.strip())
    
    def to_dict(self) -> Dict[str, Any]:
        """Convert the entity to a dictionary.
        
        Returns:
            A dictionary representation of the entity.
        """
        return {
            'name': self.name,
            'type': self.__class__.__name__,
            'line_range': { 'start': self.start_line, 'end': self.end_line },
            'byte_range': { 'start': self.byte_range[0], 'send': self.byte_range[1]},
            'has_docstring': self.is_documented,
            'language': self.language
        }


@dataclass
class FunctionEntity(CodeEntity):
    """Model for function entities.
    
    This class represents functions with their parameters and return information.
    """
    signature: str = ""
    parameters: List[str] = field(default_factory=list)
    decorators: List[str] = field(default_factory=list)
    is_async: bool = False
    return_type: Optional[str] = None
    
    def to_dict(self) -> Dict[str, Any]:
        """Convert the function to a dictionary.
        
        Returns:
            A dictionary representation of the function.
        """
        result = super().to_dict()
        result.update({
            'signature': self.signature,
            'parameters': self.parameters,
            'decorators': self.decorators,
            'is_async': self.is_async,
            'return_type': self.return_type,
            'docstring': self.docstring if self.is_documented else None
        })
        return result


@dataclass
class MethodEntity(FunctionEntity):
    """Model for method entities.
    
    This class represents class methods, extending functions with
    class membership information.
    """
    class_name: str = ""
    is_static: bool = False
    is_class_method: bool = False
    is_property: bool = False

    
    def to_dict(self) -> Dict[str, Any]:
        """Convert the method to a dictionary.
        
        Returns:
            A dictionary representation of the method.
        """
        result = super().to_dict()
        result.update({
            'class_name': self.class_name,
            'is_static': self.is_static,
            'is_class_method': self.is_class_method,
            'is_property': self.is_property,
            'qualified_name': f"{self.class_name}.{self.name}"
        })
        return result


@dataclass
class ClassEntity(CodeEntity):
    """Model for class entities.
    
    This class represents classes with their methods and attributes.
    """
    methods: List[MethodEntity] = field(default_factory=list)
    base_classes: List[str] = field(default_factory=list)
    attributes: Dict[str, Any] = field(default_factory=dict)
    
    def to_dict(self) -> Dict[str, Any]:
        """Convert the class to a dictionary.
        
        Returns:
            A dictionary representation of the class.
        """
        result = super().to_dict()
        result.update({
            'methods': [method.to_dict() for method in self.methods],
            'base_classes': self.base_classes,
            'attributes': self.attributes,
            'method_count': len(self.methods),
            'docstring': self.docstring if self.is_documented else None
        })
        return result
    
    def get_method(self, method_name: str) -> Optional[MethodEntity]:
        """Get a method by name.
        
        Args:
            method_name: The name of the method to get.
            
        Returns:
            The method entity, or None if not found.
        """
        for method in self.methods:
            if method.name == method_name:
                return method
        return None


@dataclass
class VariableEntity(CodeEntity):
    """Model for variable entities.
    
    This class represents variables with their values and types.
    """
    value: Optional[str] = None
    type_hint: Optional[str] = None
    is_constant: bool = False
    
    def to_dict(self) -> Dict[str, Any]:
        """Convert the variable to a dictionary.
        
        Returns:
            A dictionary representation of the variable.
        """
        result = super().to_dict()
        result.update({
            'value': self.value,
            'type_hint': self.type_hint,
            'is_constant': self.is_constant
        })
        return result


@dataclass
class ModuleEntity:
    """Model for module entities.
    
    This class represents a complete code module with all its entities.
    """
    classes: List[ClassEntity] = field(default_factory=list)
    functions: List[FunctionEntity] = field(default_factory=list)
    variables: List[VariableEntity] = field(default_factory=list)
    imports: List[str] = field(default_factory=list)
    language: str = ""
    filename: Optional[str] = None
    docstring: Optional[str] = None
    
    @property
    def is_documented(self) -> bool:
        """Check if the module has documentation."""
        return bool(self.docstring and self.docstring.strip())
    
    def to_dict(self) -> Dict[str, Any]:
        """Convert the module to a dictionary.
        
        Returns:
            A dictionary representation of the module.
        """
        return {
            'type': 'ModuleEntity',
            'classes': [cls.to_dict() for cls in self.classes],
            'functions': [func.to_dict() for func in self.functions],
            'variables': [var.to_dict() for var in self.variables],
            'imports': self.imports,
            'language': self.language,
            'filename': self.filename,
            'docstring': self.docstring if self.is_documented else None,
            'class_count': len(self.classes),
            'function_count': len(self.functions),
            'variable_count': len(self.variables)
        }
    
    def get_class(self, class_name: str) -> Optional[ClassEntity]:
        """Get a class by name.
        
        Args:
            class_name: The name of the class to get.
            
        Returns:
            The class entity, or None if not found.
        """
        for cls in self.classes:
            if cls.name == class_name:
                return cls
        return None
    
    def get_function(self, function_name: str) -> Optional[FunctionEntity]:
        """Get a function by name.
        
        Args:
            function_name: The name of the function to get.
            
        Returns:
            The function entity, or None if not found.
        """
        for func in self.functions:
            if func.name == function_name:
                return func
        return None
    
    def get_variable(self, variable_name: str) -> Optional[VariableEntity]:
        """Get a variable by name.
        
        Args:
            variable_name: The name of the variable to get.
            
        Returns:
            The variable entity, or None if not found.
        """
        for var in self.variables:
            if var.name == variable_name:
                return var
        return None
    
    def get_method(self, qualified_name: str) -> Optional[MethodEntity]:
        """Get a method by qualified name (ClassName.method_name).
        
        Args:
            qualified_name: The qualified name of the method to get.
            
        Returns:
            The method entity, or None if not found.
        """
        if '.' in qualified_name:
            class_name, method_name = qualified_name.split('.', 1)
            cls = self.get_class(class_name)
            if cls:
                return cls.get_method(method_name)
        return None
    
    def get_entity(self, entity_type: str, entity_name: str) -> Optional[Union[ClassEntity, FunctionEntity, MethodEntity, VariableEntity]]:
        """Get an entity by type and name.
        
        Args:
            entity_type: The type of entity ('class', 'function', 'method', 'variable').
            entity_name: The name of the entity.
            
        Returns:
            The entity, or None if not found.
        """
        if entity_type == 'class':
            return self.get_class(entity_name)
        elif entity_type == 'function':
            return self.get_function(entity_name)
        elif entity_type == 'method':
            return self.get_method(entity_name)
        elif entity_type == 'variable':
            return self.get_variable(entity_name)
        return None