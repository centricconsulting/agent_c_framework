from typing import Any, Dict, List, Optional, Union, cast
from datetime import datetime
import json

import yaml

from agent_c.toolsets.tool_set import Toolset
from agent_c.toolsets.json_schema import json_schema
from agent_c_tools.tools.workspace_knowledge.prompt import WorkspaceKnowledgeSection
from agent_c_tools.tools.workspace_knowledge.models import KnowledgeGraph, Entity, Relation
from agent_c_tools.tools.workspace.tool import WorkspaceTools


class WorkspaceKnowledgeTools(Toolset):
    """
    WorkspaceKnowledgeTools provides methods for creating and managing knowledge graphs using the metadata of a workspace.
    """

    def __init__(self, **kwargs: Any):
        super().__init__(name='wkg', **kwargs)
        self.section = WorkspaceKnowledgeSection()
        self.workspace_tool: Optional[WorkspaceTools] = None

    async def post_init(self):
        self.workspace_tool = cast(WorkspaceTools, self.tool_chest.available_tools.get('WorkspaceTools'))
    
    def _parse_kg_path(self, kg_path: str) -> tuple[str, str]:
        """Parse a knowledge graph path into workspace name and knowledge graph ID."""
        if not kg_path.startswith("//"):
            raise ValueError(f"Invalid knowledge graph path format: {kg_path}. Must start with //")
        
        parts = kg_path.split("/")
        if len(parts) < 3:
            raise ValueError(f"Invalid knowledge graph path format: {kg_path}. Format should be //workspace/knowledge_graph_id")
        
        workspace_name = parts[2]
        kg_id = "/".join(parts[3:]) if len(parts) > 3 else "default"
        
        return workspace_name, kg_id
    
    async def _get_kg_meta(self, workspace_name: str) -> Dict[str, Any]:
        """Get the knowledge graphs metadata dictionary for a workspace."""
        if not self.workspace_tool:
            raise RuntimeError("WorkspaceTools not available")

        error, workspace, key = self.workspace_tool.validate_and_get_workspace_path(f"//{workspace_name}/_kg")
        if error is not None:
            raise ValueError(f"Invalid workspace path: {workspace_name}. Error: {error}")
        
        kg_meta = await workspace.safe_metadata("_kg")
        return kg_meta or {}
    
    async def _save_kg_meta(self, workspace_name: str, kg_meta: Dict[str, Any]) -> Any:
        """Save the knowledge graphs metadata dictionary for a workspace."""
        if not self.workspace_tool:
            raise RuntimeError("WorkspaceTools not available")

        error, workspace, key = self.workspace_tool.validate_and_get_workspace_path(f"//{workspace_name}/_kg")
        if error is not None:
            raise ValueError(f"Invalid workspace path: {workspace_name}. Error: {error}")

        val = await workspace.safe_metadata_write("_kg", kg_meta)
        await workspace.save_metadata()
        return val

    async def _get_kg(self, kg_path: str) -> Optional[KnowledgeGraph]:
        """Get a knowledge graph by its path."""
        workspace_name, kg_id = self._parse_kg_path(kg_path)
        kg_meta = await self._get_kg_meta(workspace_name)
        
        if kg_id not in kg_meta:
            return None
        
        # Convert the JSON dict back to a KnowledgeGraph
        try:
            return KnowledgeGraph.model_validate(kg_meta[kg_id])
        except Exception as e:
            self._log.error(f"Error deserializing knowledge graph: {e}")
            return None
    
    async def _save_kg(self, kg_path: str, kg: KnowledgeGraph) -> None:
        """Save a knowledge graph to its path."""
        workspace_name, kg_id = self._parse_kg_path(kg_path)
        kg_meta = await self._get_kg_meta(workspace_name)
        
        # Update the knowledge graph's updated_at timestamp
        kg.updated_at = datetime.now()
        
        # Convert the KnowledgeGraph to a dict for storage
        kg_meta[kg_id] = kg.model_dump()
        await self._save_kg_meta(workspace_name, kg_meta)
    
    @json_schema(
        description="Create a new knowledge graph in a workspace",
        params={
            "kg_path": {
                "type": "string",
                "description": "Path to the knowledge graph in the format //workspace/knowledge_graph_id",
                "required": True
            },
            "title": {
                "type": "string",
                "description": "Title of the knowledge graph",
                "required": True
            },
            "description": {
                "type": "string",
                "description": "Description of the knowledge graph"
            }
        }
    )
    async def create_knowledge_graph(self, **kwargs) -> str:
        """Create a new knowledge graph in the specified workspace."""
        kg_path = kwargs.get("kg_path")
        title = kwargs.get("title")
        description = kwargs.get("description", "")
        
        workspace_name, kg_id = self._parse_kg_path(kg_path)
        kg_meta = await self._get_kg_meta(workspace_name)
        
        if kg_id in kg_meta:
            return  f"Knowledge graph with ID '{kg_id}' already exists"
        
        new_kg = KnowledgeGraph(title=title, description=description)
        await self._save_kg(kg_path, new_kg)
        
        return yaml.dump(new_kg.model_dump(), allow_unicode=True, sort_keys=False)
    
    @json_schema(
        description="List all knowledge graphs in a workspace",
        params={
            "workspace": {
                "type": "string",
                "description": "Name of the workspace",
                "required": True
            }
        }
    )
    async def list_knowledge_graphs(self, **kwargs) -> str:
        """List all knowledge graphs in the specified workspace."""
        workspace = kwargs.get("workspace")
        
        kg_meta = await self._get_kg_meta(workspace)
        
        kg_list = []
        for kg_id, kg_data in kg_meta.items():
            kg_list.append({
                "id": kg_id,
                "title": kg_data.get("title", ""),
                "description": kg_data.get("description", ""),
                "created_at": kg_data.get("created_at", ""),
                "updated_at": kg_data.get("updated_at", "")
            })
        
        return yaml.dump({"knowledge_graphs": kg_list}, allow_unicode=True, sort_keys=False)
    
    @json_schema(
        description="Get details of a knowledge graph",
        params={
            "kg_path": {
                "type": "string",
                "description": "Path to the knowledge graph in the format //workspace/knowledge_graph_id",
                "required": True
            }
        }
    )
    async def get_knowledge_graph(self, **kwargs) -> str:
        """Get details of a knowledge graph."""
        kg_path = kwargs.get("kg_path")
        
        kg = await self._get_kg(kg_path)
        
        if not kg:
            return f"Knowledge graph not found at path: {kg_path}"
        
        return yaml.dump(kg.model_dump(), allow_unicode=True, sort_keys=False)

    
    @json_schema(
        description="Create new entities in the knowledge graph",
        params={
            "kg_path": {
                "type": "string",
                "description": "Path to the knowledge graph in the format //workspace/knowledge_graph_id",
                "required": True
            },
            "entities": {
                "type": "array",
                "description": "List of entities to create",
                "items": {
                    "type": "object",
                    "properties": {
                        "name": {
                            "type": "string",
                            "description": "Name of the entity"
                        },
                        "entity_type": {
                            "type": "string",
                            "description": "Type of the entity"
                        },
                        "observations": {
                            "type": "array",
                            "description": "List of observations about the entity",
                            "items": {
                                "type": "string"
                            }
                        }
                    },
                    "required": ["name", "entity_type"]
                },
                "required": True
            }
        }
    )
    async def create_entities(self, **kwargs) -> str:
        """Create new entities in the knowledge graph."""
        kg_path = kwargs.get("kg_path")
        entities = kwargs.get("entities")
        
        kg = await self._get_kg(kg_path)
        
        if not kg:
            return  f"Knowledge graph not found at path: {kg_path}"
        
        created_entities = []
        for entity_data in entities:
            name = entity_data.get("name")
            entity_type = entity_data.get("entity_type")
            observations = entity_data.get("observations", [])
            
            if name in kg.entities:
                continue  # Skip existing entities
            
            entity = Entity(
                name=name,
                entity_type=entity_type,
                observations=observations
            )
            
            kg.entities[name] = entity
            created_entities.append(entity.model_dump())
        
        await self._save_kg(kg_path, kg)
        
        return yaml.dump({"created_entities": created_entities}, allow_unicode=True, sort_keys=False)
    
    @json_schema(
        description="Create new relations between entities in the knowledge graph",
        params={
            "kg_path": {
                "type": "string",
                "description": "Path to the knowledge graph in the format //workspace/knowledge_graph_id",
                "required": True
            },
            "relations": {
                "type": "array",
                "description": "List of relations to create",
                "items": {
                    "type": "object",
                    "properties": {
                        "from_entity": {
                            "type": "string",
                            "description": "Name of the source entity"
                        },
                        "to_entity": {
                            "type": "string",
                            "description": "Name of the target entity"
                        },
                        "relation_type": {
                            "type": "string",
                            "description": "Type of the relation"
                        }
                    },
                    "required": ["from_entity", "to_entity", "relation_type"]
                },
                "required": True
            }
        }
    )
    async def create_relations(self, **kwargs) -> str:
        """Create new relations between entities in the knowledge graph."""
        kg_path = kwargs.get("kg_path")
        relations = kwargs.get("relations")
        
        kg = await self._get_kg(kg_path)
        
        if not kg:
            return f"Knowledge graph not found at path: {kg_path}"
        
        created_relations = []
        for relation_data in relations:
            from_entity = relation_data.get("from_entity")
            to_entity = relation_data.get("to_entity")
            relation_type = relation_data.get("relation_type")
            
            # Verify that both entities exist
            if from_entity not in kg.entities:
                return  f"Source entity '{from_entity}' not found"
            
            if to_entity not in kg.entities:
                return f"Target entity '{to_entity}' not found"
            
            # Check if the relation already exists
            if any(r.from_entity == from_entity and r.to_entity == to_entity and r.relation_type == relation_type 
                   for r in kg.relations):
                continue  # Skip existing relations
            
            relation = Relation(
                from_entity=from_entity,
                to_entity=to_entity,
                relation_type=relation_type
            )
            
            kg.relations.append(relation)
            created_relations.append(relation.model_dump())
        
        await self._save_kg(kg_path, kg)
        
        return yaml.dump({"created_relations": created_relations}, allow_unicode=True, sort_keys=False)

    
    @json_schema(
        description="Add observations to existing entities",
        params={
            "kg_path": {
                "type": "string",
                "description": "Path to the knowledge graph in the format //workspace/knowledge_graph_id",
                "required": True
            },
            "observations": {
                "type": "array",
                "description": "List of observations to add",
                "items": {
                    "type": "object",
                    "properties": {
                        "entity_name": {
                            "type": "string",
                            "description": "Name of the entity to add observations to"
                        },
                        "contents": {
                            "type": "array",
                            "description": "List of observation contents to add",
                            "items": {
                                "type": "string"
                            }
                        }
                    },
                    "required": ["entity_name", "contents"]
                },
                "required": True
            }
        }
    )
    async def add_observations(self, **kwargs) -> str:
        """Add observations to existing entities in the knowledge graph."""
        kg_path = kwargs.get("kg_path")
        observations = kwargs.get("observations")
        
        kg = await self._get_kg(kg_path)
        
        if not kg:
            return f"Knowledge graph not found at path: {kg_path}"
        
        results = []
        for observation_data in observations:
            entity_name = observation_data.get("entity_name")
            contents = observation_data.get("contents", [])
            
            if entity_name not in kg.entities:
                return  f"Entity '{entity_name}' not found"
            
            entity = kg.entities[entity_name]
            added_observations = []
            
            for content in contents:
                if content not in entity.observations:
                    entity.observations.append(content)
                    added_observations.append(content)
            
            # Update the entity's updated_at timestamp
            entity.updated_at = datetime.now()
            
            results.append({
                "entity_name": entity_name,
                "added_observations": added_observations
            })
        
        await self._save_kg(kg_path, kg)
        
        return yaml.dump({"results": results}, allow_unicode=True, sort_keys=False)
    
    @json_schema(
        description="Delete entities from the knowledge graph",
        params={
            "kg_path": {
                "type": "string",
                "description": "Path to the knowledge graph in the format //workspace/knowledge_graph_id",
                "required": True
            },
            "entity_names": {
                "type": "array",
                "description": "List of entity names to delete",
                "items": {
                    "type": "string"
                },
                "required": True
            }
        }
    )
    async def delete_entities(self, **kwargs) -> str:
        """Delete entities and their associated relations from the knowledge graph."""
        kg_path = kwargs.get("kg_path")
        entity_names = kwargs.get("entity_names")
        
        kg = await self._get_kg(kg_path)
        
        if not kg:
            return  f"Knowledge graph not found at path: {kg_path}"
        
        deleted_entities = []
        for entity_name in entity_names:
            if entity_name in kg.entities:
                deleted_entities.append(entity_name)
                del kg.entities[entity_name]
        
        # Delete any relations involving these entities
        kg.relations = [r for r in kg.relations 
                       if r.from_entity not in entity_names and r.to_entity not in entity_names]
        
        await self._save_kg(kg_path, kg)
        
        return yaml.dump( { "deleted_entities": deleted_entities }, allow_unicode=True,sort_keys=False)
    
    @json_schema(
        description="Delete observations from entities",
        params={
            "kg_path": {
                "type": "string",
                "description": "Path to the knowledge graph in the format //workspace/knowledge_graph_id",
                "required": True
            },
            "deletions": {
                "type": "array",
                "description": "List of observation deletions",
                "items": {
                    "type": "object",
                    "properties": {
                        "entity_name": {
                            "type": "string",
                            "description": "Name of the entity containing the observations"
                        },
                        "observations": {
                            "type": "array",
                            "description": "List of observations to delete",
                            "items": {
                                "type": "string"
                            }
                        }
                    },
                    "required": ["entity_name", "observations"]
                },
                "required": True
            }
        }
    )
    async def delete_observations(self, **kwargs) -> str:
        """Delete specific observations from entities in the knowledge graph."""
        kg_path = kwargs.get("kg_path")
        deletions = kwargs.get("deletions")
        
        kg = await self._get_kg(kg_path)
        
        if not kg:
            return f"Knowledge graph not found at path: {kg_path}"
        
        for deletion in deletions:
            entity_name = deletion.get("entity_name")
            observations = deletion.get("observations", [])
            
            if entity_name not in kg.entities:
                continue
            
            entity = kg.entities[entity_name]
            entity.observations = [o for o in entity.observations if o not in observations]
            
            # Update the entity's updated_at timestamp
            entity.updated_at = datetime.now()
        
        await self._save_kg(kg_path, kg)
        
        return "Observations deleted successfully"

    
    @json_schema(
        description="Delete relations from the knowledge graph",
        params={
            "kg_path": {
                "type": "string",
                "description": "Path to the knowledge graph in the format //workspace/knowledge_graph_id",
                "required": True
            },
            "relations": {
                "type": "array",
                "description": "List of relations to delete",
                "items": {
                    "type": "object",
                    "properties": {
                        "from_entity": {
                            "type": "string",
                            "description": "Name of the source entity"
                        },
                        "to_entity": {
                            "type": "string",
                            "description": "Name of the target entity"
                        },
                        "relation_type": {
                            "type": "string",
                            "description": "Type of the relation"
                        }
                    },
                    "required": ["from_entity", "to_entity", "relation_type"]
                },
                "required": True
            }
        }
    )
    async def delete_relations(self, **kwargs) -> str:
        """Delete specific relations from the knowledge graph."""
        kg_path = kwargs.get("kg_path")
        relations = kwargs.get("relations")
        
        kg = await self._get_kg(kg_path)
        
        if not kg:
            return f"Knowledge graph not found at path: {kg_path}"
        
        initial_count = len(kg.relations)
        for relation_data in relations:
            from_entity = relation_data.get("from_entity")
            to_entity = relation_data.get("to_entity")
            relation_type = relation_data.get("relation_type")
            
            kg.relations = [r for r in kg.relations 
                          if not (r.from_entity == from_entity and 
                                 r.to_entity == to_entity and 
                                 r.relation_type == relation_type)]
        
        deleted_count = initial_count - len(kg.relations)
        await self._save_kg(kg_path, kg)
        
        return f"deleted_count: {deleted_count}"

    
    @json_schema(
        description="Search for nodes in the knowledge graph",
        params={
            "kg_path": {
                "type": "string",
                "description": "Path to the knowledge graph in the format //workspace/knowledge_graph_id",
                "required": True
            },
            "query": {
                "type": "string",
                "description": "Search query to match against entity names, types, and observation content",
                "required": True
            }
        }
    )
    async def search_nodes(self, **kwargs) -> str:
        """Search for nodes in the knowledge graph based on a query."""
        kg_path = kwargs.get("kg_path")
        query = kwargs.get("query")
        
        kg = await self._get_kg(kg_path)
        
        if not kg:
            return  f"Knowledge graph not found at path: {kg_path}"
        
        # Filter entities
        matching_entities = {}
        for name, entity in kg.entities.items():
            if (query.lower() in name.lower() or 
                query.lower() in entity.entity_type.lower() or 
                any(query.lower() in obs.lower() for obs in entity.observations)):
                matching_entities[name] = entity.model_dump()
        
        # Filter relations to only include those between matching entities
        matching_relations = []
        for relation in kg.relations:
            if relation.from_entity in matching_entities and relation.to_entity in matching_entities:
                matching_relations.append(relation.model_dump())
        
        return yaml.dump({"entities": matching_entities,"relations": matching_relations},  allow_unicode=True, sort_keys=False)
    
    @json_schema(
        description="Get specific nodes in the knowledge graph by their names",
        params={
            "kg_path": {
                "type": "string",
                "description": "Path to the knowledge graph in the format //workspace/knowledge_graph_id",
                "required": True
            },
            "names": {
                "type": "array",
                "description": "List of entity names to retrieve",
                "items": {
                    "type": "string"
                },
                "required": True
            }
        }
    )
    async def get_nodes(self, **kwargs) -> str:
        """Get specific nodes in the knowledge graph by their names."""
        kg_path = kwargs.get("kg_path")
        names = kwargs.get("names")
        
        kg = await self._get_kg(kg_path)
        
        if not kg:
            return f"Knowledge graph not found at path: {kg_path}"
        
        # Filter entities
        matching_entities = {}
        for name in names:
            if name in kg.entities:
                matching_entities[name] = kg.entities[name].model_dump()
        
        # Filter relations to only include those between matching entities
        matching_relations = []
        for relation in kg.relations:
            if relation.from_entity in matching_entities and relation.to_entity in matching_entities:
                matching_relations.append(relation.model_dump())
        
        return yaml.dump({"entities": matching_entities,"relations": matching_relations}, allow_unicode=True, sort_keys=False)



Toolset.register(WorkspaceKnowledgeTools, required_tools=['WorkspaceTools'])