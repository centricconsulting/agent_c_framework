from typing import Any, Optional, cast

from agent_c.prompting.prompt_section import OldPromptSection, property_bag_item


class WorkspaceKnowledgeSection(OldPromptSection):

    def __init__(self, **data: Any):
        TEMPLATE = (
            "The Workspace Knowledge Tools (wkg) allow you to create and manage knowledge graphs using the metadata of a workspace.\n\n"
            
            "## Knowledge Graph Structure\n"
            "- Knowledge Graph: A collection of entities and relations representing knowledge\n"
            "- Entities: Objects or concepts within the knowledge graph\n"
            "- Relations: Connections between entities describing how they are related\n"
            "- Observations: Facts or notes about entities\n\n"
            
            "## Path Format\n"
            "- Use a UNC style path of `//[workspace]/[knowledge_graph_id]` to reference knowledge graphs\n"
            "- Example: `//project/my_knowledge_graph` refers to a knowledge graph named 'my_knowledge_graph' in the 'project' workspace\n\n"
            
            "## Core Features\n"
            "- Create and manage knowledge graphs across workspaces\n"
            "- Add entities with different types\n"
            "- Create relations between entities\n"
            "- Add observations to entities to capture facts and information\n"
            "- Search for nodes based on names, types, or observation content\n"
            "- Navigate related entities through their connections\n\n"
            
            "## Recommended Usage\n"
            "1. Create a knowledge graph for the domain you're working in\n"
            "2. Add key entities and their types\n"
            "3. Connect related entities with appropriately named relations\n"
            "4. Add observations to entities as you learn more about them\n"
            "5. Use search to find relevant information quickly\n"
            "6. View connected entities to understand relationships and context\n"
        )
        super().__init__(template=TEMPLATE, required=True, name="Workspace Knowledge", render_section_header=True, **data)