import os
import json
import logging
import weaviate

from typing import Any, List
from weaviate.classes.query import HybridFusion

from agent_c.toolsets import json_schema, Toolset
from agent_c.util.segmentation.segment_repo import SegmentRepo


class WeaviateTools(Toolset):
    FUSION_TYPE_MAP = {
        "ranked": HybridFusion.RANKED,
        "relative_score": HybridFusion.RELATIVE_SCORE
    }


    """
    Toolset for interacting with a Weaviate client for performing various operations.

    Attributes:
        logger (logging.Logger): Logger for logging messages.
        client (weaviate.WeaviateClient): Weaviate client instance for database operations.
        collection_name (str): Name of the collection to perform the search on.
        repo (SegmentRepo): Repository for segmentation operations.
    """

    def __init__(self, **kwargs: Any) -> None:
        """
        Initialize the WeaviateTools instance.

        Args:
            **kwargs: Arbitrary keyword arguments. Expected keys are:
                client (weaviate.WeaviateClient): Weaviate client instance.
                collection_name (str): Name of the collection.
        """
        super().__init__(**kwargs, name='weaviate')
        self.logger: logging.Logger = logging.getLogger(__name__)
        self.client: weaviate.WeaviateClient = kwargs.get(
            'client',
            weaviate.connect_to_local(headers={"X-Openai-Api-Key": os.getenv("OPENAI_API_KEY"),
                                               "Authorization": f"Bearer {os.getenv('WEAVIATE_API_KEY')}"}))
        self.collection_name: str = 'default'
        self.repo: SegmentRepo = SegmentRepo(self.client)

    @json_schema(
        'This tool allows you to perform a similarity search against a vector database containing segments of documentation.',
        {
            'text': {
                'type': 'string',
                'description': 'The text to search for similar segments in the vector database.',
                'required': True
            },
            'collection_name': {
                'type': 'string',
                'description': 'The name of the collection to search in. If one is not provided, the default collection is used.',
                'required': False,
                'default': 'default'
            },
            'result_limit': {
                'type': 'integer',
                'description': 'Limit on the number of results. Default is 500.',
                'required': False,
                'default': 10
            },
        }
    )
    async def near_text(self, **kwargs: Any) -> str:
        """
        Perform near_text search on a Weaviate collection.

        Args:
            **kwargs: Arbitrary keyword arguments. Expected keys are:
                text (str): The text for the similarity search.
                result_limit (int, optional): Limit on the number of results. Default is 500.
                offset (int, optional): Offset for the results, for pagination. Default is 0.
                token_limit (int, optional): Token limit for the context. Default is 5000.
                collection_name (str, optional): Name of the collection to search in. Default is 'default'.
        Returns:
            str: The response from the Weaviate Collection.
                 In case of an error, returns the error message in JSON format.
        """
        query: str = kwargs.get('text')
        result_limit: int = kwargs.get('result_limit', 10)
        offset: int = kwargs.get('offset', 0)
        token_limit: int = kwargs.get('token_limit', 5000)
        self.collection_name: str = kwargs.get('collection_name', self.collection_name)

        try:
            context = await self.repo.model_context_for_query(
                query, collection_name=self.collection_name, result_limit=result_limit, offset=offset, token_limit=token_limit
            )
            return context.text

        except Exception as e:
            error_message: str = f"An unexpected error occurred: {e}"
            self.logger.error(error_message)
            return json.dumps({"error": error_message})

    @json_schema(
        'This tool allows you to perform a keyword (BM25) search against a vector database containing segments of documentation.',
        {
            'text': {
                'type': 'string',
                'description': 'The text to search for similar segments in the vector database.',
                'required': True
            },
            'collection_name': {
                'type': 'string',
                'description': 'The name of the collection to search in. If one is not provided, the default collection is used.',
                'required': False,
                'default': 'default'
            },
            'properties': {
                'type': 'array',
                'items': {
                    'type': 'string'
                },
                'description': 'List of properties (fields) to limit the BM25 search to. Default is all text properties.',
                'required': False
            },
            'result_limit': {
                'type': 'integer',
                'description': 'Limit on the number of results. Default is 500.',
                'required': False,
                'default': 10
            },
        }
    )
    async def bm25_search(self, **kwargs: Any) -> str:
        """
        Perform BM25 keyword search on a Weaviate collection.

        Args:
            **kwargs: Arbitrary keyword arguments. Expected keys are:
                text (str): The text for the keyword search.
                result_limit (int, optional): Limit on the number of results. Default is 500.
                offset (int, optional): Offset for the results, for pagination. Default is 0.
                token_limit (int, optional): Token limit for the context. Default is 5000.
                collection_name (str, optional): Name of the collection to search in. Default is 'default'.
                properties (List[str], optional): Array of properties (fields) to search in, defaulting to all properties in the collection.
        Returns:
            str: The response from the Weaviate Collection.
                 In case of an error, returns the error message in JSON format.
        """
        query: str = kwargs.get('text', '')
        result_limit: int = kwargs.get('result_limit', 10)
        offset: int = kwargs.get('offset', 0)
        token_limit: int = kwargs.get('token_limit', 5000)
        self.collection_name: str = kwargs.get('collection_name', self.collection_name)
        properties: List[str]|None = kwargs.get('properties', None)

        if query is None:
            error_message: str = "The 'text' parameter is required for the BM25 search."
            self.logger.error(error_message)
            return json.dumps({"error": error_message})

        try:
            context = await self.repo.model_context_for_query(
                query, search_type="bm25", collection_name=self.collection_name,
                result_limit=result_limit, offset=offset, token_limit=token_limit,
                properties=properties
            )
            return context.text

        except Exception as e:
            error_message: str = f"An unexpected error occurred: {e}"
            self.logger.error(error_message)
            return json.dumps({"error": error_message})


    def _convert_fusion_type(self, fusion_type):
        """
        Convert string fusion_type to HybridFusion enum value if needed.

        Args:
            fusion_type: String or HybridFusion enum

        Returns:
            HybridFusion enum value
        """
        if isinstance(fusion_type, str):
            if fusion_type.lower() in self.FUSION_TYPE_MAP:
                return self.FUSION_TYPE_MAP[fusion_type.lower()]
            else:
                self.logger.warning(
                    f"Unknown fusion_type: {fusion_type}. Valid values are: {list(self.FUSION_TYPE_MAP.keys())}")
                return HybridFusion.RELATIVE_SCORE

        # Already an enum or None
        return fusion_type


    @json_schema(
        'This tool allows you to perform a hybrid search (combining BM25 and vector search) against a vector database containing segments of documentation.',
        {
            'text': {
                'type': 'string',
                'description': 'The text to search for similar segments in the vector database.',
                'required': True
            },
            'collection_name': {
                'type': 'string',
                'description': 'The name of the collection to search in. If one is not provided, the default collection is used.',
                'required': False,
                'default': 'default'
            },
            'alpha': {
                'type': 'number',
                'description': 'Weighting between BM25 and vector search (0 to 1). 0 for pure BM25, 1 for pure vector search, 0.5 for equal weighting.',
                'required': False,
                'default': 0.50
            },
            'properties': {
                'type': 'array',
                'items': {
                    'type': 'string'
                },
                'description': 'List of properties (fields) to limit the BM25 search to. Default is all text properties.',
                'required': False
            },
            'fusion_type': {
                'type': 'string',
                'description': 'The type of hybrid fusion algorithm. Can be "ranked" or "relative_score". With the ranked algorithm, each object is scored according to its position in the results for that search (vector or keyword). The top-ranked objects in each search get the highest scores. In relative_score the vector search and keyword search scores are scaled between 0 and 1. The highest raw score becomes 1 in the scaled scores. The lowest value is assigned 0. The remaining values are ranked between 0 and 1. The total score is a scaled sum of the normalized vector similarity and normalized BM25 scores.',
                'enum': ['ranked', 'relative_score'],
                'required': False,
                'default': 'relativeScoreFusion'
            },
            'result_limit': {
                'type': 'integer',
                'description': 'Limit on the number of results. Default is 500.',
                'required': False,
                'default': 10
            },
        }
    )
    async def hybrid_search(self, **kwargs: Any) -> str:
        """
        Perform hybrid search (combining BM25 and vector search) on a Weaviate collection.

        Args:
            **kwargs: Arbitrary keyword arguments. Expected keys are:
                text (str): The text for the hybrid search.
                result_limit (int, optional): Limit on the number of results. Default is 500.
                offset (int, optional): Offset for the results, for pagination. Default is 0.
                token_limit (int, optional): Token limit for the context. Default is 5000.
                collection_name (str, optional): Name of the collection to search in. Default is 'default'.
                alpha (float, optional): Weighting between BM25 and vector search (0 to 1). Default is 0.75.
                properties (List[str], optional): Array of properties (fields) to search in, defaulting to all properties in the collection.
                fusion_type (str, optional): The type of hybrid fusion algorithm. Default is 'relativeScoreFusion'.
        Returns:
            str: The response from the Centric Knowledge Base.
                 In case of an error, returns the error message in JSON format.
        """
        query: str = kwargs.get('text', '')
        result_limit: int = kwargs.get('result_limit', 10)
        offset: int = kwargs.get('offset', 0)
        token_limit: int = kwargs.get('token_limit', 5000)
        self.collection_name: str = kwargs.get('collection_name', self.collection_name)
        alpha: float = kwargs.get('alpha', 0.50)
        properties: List[str]|None = kwargs.get('properties', None)
        fusion_type = kwargs.get("fusion_type", "relative_score")
        fusion_type = self._convert_fusion_type(fusion_type)

        try:
            context = await self.repo.model_context_for_query(
                query, search_type="hybrid", collection_name=self.collection_name,
                result_limit=result_limit, offset=offset, token_limit=token_limit,
                alpha=alpha, properties=properties, fusion_type=fusion_type
            )
            return context.text

        except Exception as e:
            error_message: str = f"An unexpected error occurred: {e}"
            self.logger.error(error_message)
            return json.dumps({"error": error_message})

# DS Note: Disabled for now
# Toolset.register(WeaviateTools)
