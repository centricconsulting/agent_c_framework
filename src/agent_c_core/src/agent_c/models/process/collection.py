from typing import Optional, List, Any, Dict

from pydantic import field_validator, model_validator, Field

from agent_c.models import BaseModel
from agent_c.models.process.definition import ProcessDefinition
from agent_c.util import to_snake_case


class ProcessCollection(BaseModel):
    """Collection of multiple processes that can be saved together"""
    name: str = Field(..., description="Name of this process collection")
    description: Optional[str] = Field(None, description="Description of this collection")
    version: str = Field("1.0.0", description="Version of this collection")

    processes: Dict[str, ProcessDefinition] = Field(..., description="Dictionary of process templates keyed by name")

    # Shared resources
    shared_context: Dict[str, Any] = Field(default_factory=dict, description="Context variables shared across all processes")
    shared_templates: Dict[str, str] = Field(default_factory=dict, description="Jinja templates shared across processes")

    # Dependencies
    dependencies: Dict[str, List[str]] = Field(default_factory=dict, description="Process dependencies (process_name -> [required_processes])")

    # Metadata
    tags: List[str] = Field(default_factory=list, description="Tags for this collection")
    metadata: Dict[str, Any] = Field(default_factory=dict, description="Additional metadata")

    @field_validator('name', mode='after')
    @classmethod
    def normalize_collection_name(cls, value: str) -> str:
        """Normalize collection name"""
        return to_snake_case(value)

    @model_validator(mode='after')
    def validate_dependencies(self):
        """Validate that all dependencies exist in the collection"""
        process_names = set(self.processes.keys())

        for process_name, deps in self.dependencies.items():
            if process_name not in process_names:
                raise ValueError(f"Dependency definition references unknown process '{process_name}'")

            for dep in deps:
                if dep not in process_names:
                    raise ValueError(f"Process '{process_name}' depends on unknown process '{dep}'")

        return self

    def get_process(self, name: str) -> Optional[ProcessDefinition]:
        """Get a process by name"""
        return self.processes.get(name)

    def add_process(self, process: ProcessDefinition) -> None:
        """Add a process to the collection"""
        self.processes[process.name] = process

    def remove_process(self, name: str) -> bool:
        """Remove a process from the collection"""
        if name in self.processes:
            del self.processes[name]
            # Clean up any dependencies referencing this process
            self.dependencies = {
                k: [dep for dep in v if dep != name]
                for k, v in self.dependencies.items()
                if k != name
            }
            return True
        return False