from typing import Dict, Type, Optional, List, Any
from pathlib import Path
import json
from pydantic import ValidationError

from agent_c.models.process import ProcessDefinition, ProcessCollection


class ProcessRegistry:
    """Registry for managing process templates, similar to ConfigRegistry"""

    _templates: Dict[str, Type[ProcessDefinition]] = {}
    _instances: Dict[str, ProcessDefinition] = {}
    _collections: Dict[str, ProcessCollection] = {}

    @classmethod
    def register(cls, template_class: Type[ProcessDefinition]) -> None:
        """Register a process template class"""
        if not issubclass(template_class, ProcessDefinition):
            raise ValueError(f"Can only register ProcessTemplate subclasses, got {template_class}")

        # Use the normalized name from the class
        dummy_instance = template_class(name="dummy", steps=[], initial_step="dummy")
        name = dummy_instance.name.removesuffix('_process')

        cls._templates[name] = template_class

    @classmethod
    def register_instance(cls, instance: ProcessDefinition) -> None:
        """Register a process template instance"""
        name = instance.name.removesuffix('_process')
        cls._instances[name] = instance

    @classmethod
    def register_collection(cls, collection: ProcessCollection) -> None:
        """Register a process collection"""
        cls._collections[collection.name] = collection

        # Also register individual processes from the collection
        for process_name, process in collection.processes.items():
            cls.register_instance(process)

    @classmethod
    def get_template_class(cls, name: str) -> Optional[Type[ProcessDefinition]]:
        """Get a process template class by name"""
        return cls._templates.get(name)

    @classmethod
    def get_template_instance(cls, name: str) -> Optional[ProcessDefinition]:
        """Get a process template instance by name"""
        return cls._instances.get(name)

    @classmethod
    def get_collection(cls, name: str) -> Optional[ProcessCollection]:
        """Get a process collection by name"""
        return cls._collections.get(name)

    @classmethod
    def list_templates(cls) -> List[str]:
        """List all registered template names"""
        all_names = set(cls._templates.keys()) | set(cls._instances.keys())
        return sorted(list(all_names))

    @classmethod
    def list_collections(cls) -> List[str]:
        """List all registered collection names"""
        return sorted(list(cls._collections.keys()))

    @classmethod
    def create_instance(cls, name: str, **kwargs) -> Optional[ProcessDefinition]:
        """Create a new instance of a process template"""
        # Try template class first
        template_class = cls.get_template_class(name)
        if template_class:
            return template_class(**kwargs)

        # Try copying from registered instance
        template_instance = cls.get_template_instance(name)
        if template_instance:
            # Create a copy with updated parameters
            data = template_instance.model_dump()
            data.update(kwargs)
            return ProcessDefinition(**data)

        return None

    @classmethod
    def unregister(cls, name: str) -> bool:
        """Unregister a process template"""
        removed = False
        if name in cls._templates:
            del cls._templates[name]
            removed = True
        if name in cls._instances:
            del cls._instances[name]
            removed = True
        return removed

    @classmethod
    def unregister_collection(cls, name: str) -> bool:
        """Unregister a process collection and its processes"""
        if name not in cls._collections:
            return False

        collection = cls._collections[name]

        # Remove individual processes
        for process_name in collection.processes.keys():
            cls.unregister(process_name)

        # Remove collection
        del cls._collections[name]
        return True

    @classmethod
    def clear(cls) -> None:
        """Clear all registered templates and collections"""
        cls._templates.clear()
        cls._instances.clear()
        cls._collections.clear()

    @classmethod
    def load_from_file(cls, file_path: Path) -> ProcessCollection:
        """Load a process collection from a JSON file"""
        if not file_path.exists():
            raise FileNotFoundError(f"Process file not found: {file_path}")

        try:
            with open(file_path, 'r') as f:
                data = json.load(f)

            collection = ProcessCollection(**data)
            cls.register_collection(collection)
            return collection

        except json.JSONDecodeError as e:
            raise ValueError(f"Invalid JSON in process file {file_path}: {e}")
        except ValidationError as e:
            raise ValueError(f"Invalid process data in {file_path}: {e}")

    @classmethod
    def save_to_file(cls, collection_name: str, file_path: Path) -> None:
        """Save a process collection to a JSON file"""
        collection = cls.get_collection(collection_name)
        if not collection:
            raise ValueError(f"Collection '{collection_name}' not found")

        # Ensure directory exists
        file_path.parent.mkdir(parents=True, exist_ok=True)

        # Save with pretty formatting
        with open(file_path, 'w') as f:
            json.dump(
                collection.model_dump(exclude_none=True),
                f,
                indent=2,
                ensure_ascii=False
            )

    @classmethod
    def load_directory(cls, directory: Path) -> List[ProcessCollection]:
        """Load all process files from a directory"""
        if not directory.exists():
            raise FileNotFoundError(f"Directory not found: {directory}")

        collections = []
        for file_path in directory.glob("*.json"):
            try:
                collection = cls.load_from_file(file_path)
                collections.append(collection)
            except Exception as e:
                print(f"Warning: Failed to load process file {file_path}: {e}")

        return collections

    @classmethod
    def get_info(cls) -> Dict[str, Any]:
        """Get information about registered templates and collections"""
        return {
            "template_classes": len(cls._templates),
            "template_instances": len(cls._instances),
            "collections": len(cls._collections),
            "templates": cls.list_templates(),
            "collection_names": cls.list_collections(),
        }

    @classmethod
    def find_processes_with_tag(cls, tag: str) -> List[ProcessDefinition]:
        """Find all processes that have a specific tag"""
        matching_processes = []

        # Check registered instances
        for process in cls._instances.values():
            if tag in process.tags:
                matching_processes.append(process)

        # Check processes in collections
        for collection in cls._collections.values():
            if tag in collection.tags:
                # Add all processes from this collection
                matching_processes.extend(collection.processes.values())
            else:
                # Check individual processes
                for process in collection.processes.values():
                    if tag in process.tags:
                        matching_processes.append(process)

        return matching_processes

    @classmethod
    def find_processes_by_metadata(cls, key: str, value: Any) -> List[ProcessDefinition]:
        """Find processes by metadata key-value pair"""
        matching_processes = []

        # Check registered instances
        for process in cls._instances.values():
            if process.metadata.get(key) == value:
                matching_processes.append(process)

        # Check processes in collections
        for collection in cls._collections.values():
            for process in collection.processes.values():
                if process.metadata.get(key) == value:
                    matching_processes.append(process)

        return matching_processes

    @classmethod
    def validate_process_dependencies(cls, process_name: str) -> Dict[str, bool]:
        """Validate that all dependencies for a process are available"""
        result = {}

        # Find the process in collections that might have dependencies
        for collection in cls._collections.values():
            if process_name in collection.dependencies:
                for dep in collection.dependencies[process_name]:
                    result[dep] = (
                            cls.get_template_instance(dep) is not None or
                            cls.get_template_class(dep) is not None
                    )

        return result


# Helper functions for common operations
def register_process_template(template: ProcessDefinition) -> None:
    """Convenience function to register a process template instance"""
    ProcessRegistry.register_instance(template)


def get_process_template(name: str) -> Optional[ProcessDefinition]:
    """Convenience function to get a process template"""
    return ProcessRegistry.get_template_instance(name)


def create_process_instance(name: str, **kwargs) -> Optional[ProcessDefinition]:
    """Convenience function to create a process instance"""
    return ProcessRegistry.create_instance(name, **kwargs)


def load_processes_from_file(file_path: str) -> ProcessCollection:
    """Convenience function to load processes from file"""
    return ProcessRegistry.load_from_file(Path(file_path))


def save_processes_to_file(collection_name: str, file_path: str) -> None:
    """Convenience function to save processes to file"""
    ProcessRegistry.save_to_file(collection_name, Path(file_path))
