# This tool needs rethought in light of agent assist

import json
import logging
import os

from typing import Union, List, Any

from agent_c import GPTChatAgent, BaseAgent, PromptBuilder, Toolset, json_schema, ToolChest, PersonaSection
from agent_c_rag.tools.weaviate import WeaviateTools


class KnowledgebaseTools(Toolset):

    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='knowledgebase')
        self.logger: logging.Logger = logging.getLogger(__name__)
        self.collection_name: str = 'default'
        self.tool_chest = ToolChest(tool_classes=[WeaviateTools])
        self.agent: Union[BaseAgent, None] = None
        self.messages: Union[List[dict[str, Any]]] = []
        self.sections: List[PersonaSection] = []


    async def post_init(self):
        await self.tool_chest.init_tools(streaming_callback=self.streaming_callback)

        prompt: str = ("You are a helpful assistant equipped with a tool to allow your to locate information in a knowledgebase.\n"
                       "The weaviate tool will allow you to perform a similarity search to locate information. You will need to formulate "
                       "search text optimized for this sort of similarity search from the often messy user input\n"
                       "Limit yourself to 3 searches per user input, if an answer can not be found by them, simply inform the user the information couldn't be located.\n"
                       "Make sure to cite your sources when providing information to the user.\n")

        self.agent = GPTChatAgent(tool_chest=self.tool_chest, model_name=os.environ.get('MODEL_NAME', 'gpt-4o'),
                                  streaming_callback=self.streaming_callback)

        self.sections = [PersonaSection(template=prompt)] + self.tool_chest.active_tool_sections


    @json_schema(
        'This tool allows you to ask questions to a specialized knowledge base agent.  It is capable of handling direct questions and followup questions.',
        {
            'query': {
                'type': 'string',
                'description': 'The information you would like answered from the knowledge base',
                'required': True
            },
            'is_followup': {
                'type': 'boolean',
                'description': 'Set to true if this is a followup question.  Defaults to False',
                'required': False
            },
            'collection_name': {
                'type': 'string',
                'description': 'The name of the collection to search in.',
                'required': False,
                'default': 'default'
            }
        }
    )
    async def query_knowledgebase(self, **kwargs) -> str:
        query: str = kwargs.get('query')
        is_followup: bool = kwargs.get('is_followup', False)
        self.collection_name = kwargs.get('collection_name', self.collection_name)

        if not is_followup:
            self.messages = []

        try:
            # Set the collection name for the weaviate tool and pass in as metadata.
            # TODO: This should be done in a more robust way. Intermittently fails.
            prompt_metadata = {'collection_name': self.collection_name}
            self.tool_chest.active_tools['weaviate'].collection_name = self.collection_name
            self. messages = await self.agent.chat(user_message=query, messages=self.messages,
                                                   prompt_builder=PromptBuilder(sections=self.sections),
                                                   prompt_metadata=prompt_metadata, agent_role="knowledgebase")

            return "The answer to the query has been shown to the user. Check with the user to see if they need additional information or clarification."

        except Exception as e:
            error_message: str = f"An unexpected error occurred: {e}"

        self.logger.error(error_message)
        return json.dumps({"error": error_message})

Toolset.register(KnowledgebaseTools)
