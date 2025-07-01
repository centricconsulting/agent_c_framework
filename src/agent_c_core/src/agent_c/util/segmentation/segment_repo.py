import os
import logging
import asyncio
import nltk
import weaviate

from functools import partial
from typing import Dict, Any, List
from weaviate.classes.query import MetadataQuery
from weaviate.collections.classes.config import Configure, DataType

from agent_c.util.segmentation.context import SourceChronoFormatter # noqa
from agent_c.util.segmentation.weaviate.text_segment import TextSegment
from agent_c.util.segmentation.context.models.agent_context import AgentContextModel
from agent_c.util.segmentation.context.context_formatter import  ContextFormatter
from agent_c.util.vector_transformers.vector_text_transformer import VectorTextTransformer, VectorTransformOptionsModel
from agent_c.util.token_counter import TokenCounter


class SegmentRepo:
    def __init__(self, weaviate_client: weaviate.WeaviateClient):
        """
        Initialize the SegmentRepo class.

        Args:
            weaviate_client (weaviate.client.Client): The Weaviate client object.
        """
        self.weaviate_client: weaviate.WeaviateClient = weaviate_client
        nltk.download('punkt')
        nltk.download('stopwords')
        self.logger: logging.Logger = logging.getLogger(__name__)

    def batch_load_segments(self, collection: str, segments: List[TextSegment], rate_limit: int = 5000) -> List[dict]:
        """
        Batch load segments into a collection.

        Args:
            collection (str): The name of the collection to load segments into.
            segments (List[TextSegment]): A list of TextSegment objects to be loaded into the collection.
            rate_limit (int, optional): The rate limit for the batch load in requests per minute. Default is 5000.
        Returns:
            List[dict]: A list of failed objects information dictionaries.
        """
        self.logger.debug(f"Starting batch load of {len(segments)} segments")

        collection = self.weaviate_client.collections.get(collection)
        # Get initial count
        initial_response = collection.aggregate.over_all(total_count=True)
        initial_count = initial_response.total_count
        self.logger.debug(f"Collection has {initial_count} segments before batch load")

        with collection.batch.rate_limit(requests_per_minute=rate_limit) as batch:
            # for segment in segments:
            #     opts = segment.as_weaviate_object()
            #     batch.add_object(**opts)
            self.logger.debug(f"Adding {len(segments)} segments for batch upload")
            for i, segment in enumerate(segments):
                opts = segment.as_weaviate_object()
                # self.logger.debug(f"Adding segment {i + 1}/{len(segments)}: UUID={segment.uuid}, parent={segment.parent_segment}")
                # self.logger.debug(f"Weaviate properties: {opts}") # this is noisy and contains segment content
                batch.add_object(**opts)

            batch.flush()

        # Get final count
        final_response = collection.aggregate.over_all(total_count=True)
        final_count = final_response.total_count
        self.logger.debug(f"Collection now has {final_count} segments after batch load")
        self.logger.debug(f"Net change in segments: {final_count - initial_count}")

        failed_objs = collection.batch.failed_objects
        if failed_objs:
            self.logger.error(f"Failed to load {len(failed_objs)} segments")
            for failed in failed_objs:
                self.logger.error(f"Failed object details: {failed}")
        else:
            self.logger.debug("All segments successfully processed in batch")
        return failed_objs


    async def model_context_for_query(self, query: str, **kwargs) -> AgentContextModel:
        """
        Query the vector store to find relevant documents, then convert them into a stuffing string.

        Args:
            query (str): The text to find relevant documents for.
            **kwargs: Additional keyword arguments.

        Kwargs:
        search_type (string, optional): Must be one of ['similarity', 'hybrid', 'bm25'].  Default is similarity.
        token_limit (int, optional): Limit the text to this token limit.
                                     If zero all relevant docs will be included.
                                     If non-zero the least relevant docs will be excluded as needed to fit under the limit.
        formatter (string, optional): The context formatter to use.
        llm_model (str, optional): The model name used for determining token counts when generating the stuffing text.
        not implemented: transform_options (VectorTransformOptionsModel, optional): The options to use when optimizing the query.
        result_limit (int, optional): The maximum number of relevant segments to return.
        not implemented: min_relevance (float, optional): The minimum relevance score for returned documents, ranging from 0.0 to 1.0.
        alpha (float, optional): For hybrid search, weighting between BM25 and vector search (0 to 1). Default is 0.75.
        properties (List[str], optional): For hybrid/bm25 search, list of properties to limit the BM25 search to.
        fusion_type (str, optional): For hybrid search, the type of fusion algorithm. Default is 'relativeScoreFusion'.

        Returns:
            AgentContextModel: A model containing the context suitable for prompting
        """
        search_type = kwargs.get("search_type", "similarity")
        kwargs['token_counter'] = kwargs.get('token_counter', TokenCounter.counter())

        token_limit: int = kwargs.get('token_limit', 5000)
        kwargs['token_limit'] = token_limit

        context_format: str = kwargs.get("formatter", "SourceChronoFormatter")
        formatter_class = globals()[context_format]
        formatter: ContextFormatter = formatter_class(**kwargs)
        collection_name: str = kwargs.get("collection_name", "default")

        result_limit: int = kwargs.get('result_limit', 500)
        offset: int = kwargs.get('offset', 0)

        try:
            if search_type == "hybrid":
                segments = self.hybrid_search(query, **kwargs)
            elif search_type == "bm25":
                segments = self.bm25_search(query, **kwargs)
            else:  # default to similarity search
                segments = self.similarity_search(query, **kwargs)

            context = formatter.format_segments(segments)
            return context

        except Exception as e:
            error_message: str = f"An unexpected error occurred: {e}"
            self.logger.error(error_message)
            raise

    def similarity_search(self, text: str, **kwargs) -> List[TextSegment]:
        """
        Perform a similarity search.

        Args:
            text (str): The query text.
            **kwargs (Dict[str, Any]): Additional keyword arguments.

        Kwargs:
            search_distance (float, optional): The search distance.

            embedded_query (str, optional): An embedding object representing the user query

            auto_cut (int, optional): Autocut takes a positive integer parameter N, looks at the score of each result,
                                      and stops returning results after the Nth "drop" in score.
                                      Because hybrid combines a vector search with a keyword (BM25F) search,
                                      their scores/distances cannot be directly compared, so the cut points may not be intuitive.

            where_clause (dict, optional): A dict containing a Weaviate where clause.

        Returns:
            List[SegmentModel]: List of segment models.
        """
        search_distance: float = kwargs.get("search_distance", float(os.environ.get("SS_SEARCH_DISTANCE", '0.0')))
        model_class: Any = self.__model_class(**kwargs)
        collection_name = kwargs.get("collection_name", "default")
        limit = kwargs.get("limit", 1000)
        offset = kwargs.get("offset", 0)


        query_opts = {'query': self.__optimize_query_if_needed(text, **kwargs),  'return_metadata': MetadataQuery(distance=True),
                      'limit': limit, 'offset': offset}

        if search_distance > 0:
            query_opts['distance'] = search_distance

        collection = self.weaviate_client.collections.get(collection_name)
        response = collection.query.near_text(**query_opts)

        return model_class.from_weaviate_response(response)

    async def similarity_search_async(self, query: str, **kwargs: Dict[str, Any]) -> List[TextSegment]:
        """
        Perform a similarity search asynchronously.

        Args:
            query (str): The query text.
            **kwargs (Dict[str, Any]): Additional keyword arguments.

        Kwargs:
            search_distance (float, optional): The search distance.

            embedded_query (str, optional): An embedding object representing the user query

            auto_cut (int, optional): Autocut takes a positive integer parameter N, looks at the score of each result,
                                      and stops returning results after the Nth "drop" in score.
                                      Because hybrid combines a vector search with a keyword (BM25F) search,
                                      their scores/distances cannot be directly compared, so the cut points may not be intuitive.

            where_clause (dict, optional): A dict containing a Weaviate where clause.

        Returns:
            List[SegmentModel]: List of segment models.
        """
        query, kwargs['embedded_query'] = await self.__process_query_async(query, **kwargs)
        func = partial(self.similarity_search, query, **kwargs)
        return await asyncio.get_event_loop().run_in_executor(None, func)

    def hybrid_search(self, text: str, **kwargs) -> List[TextSegment]:
        """
        Perform a hybrid search (combining BM25 and vector search).

        Args:
            text (str): The query text.
            **kwargs (Dict[str, Any]): Additional keyword arguments.

        Kwargs:
            alpha (float, optional): Weighting between BM25 and vector search (0 to 1). Default is 0.75.
                alpha = 0 forces using a pure keyword search method (BM25)
                alpha = 1 forces using a pure vector search method
                alpha = 0.5 weighs the BM25 and vector methods evenly
            properties (List[str], optional): List of properties to limit the BM25 search to. Default is all text properties.
            fusion_type (str, optional): The type of hybrid fusion algorithm. Can be 'rankedFusion' or 'relativeScoreFusion'.
                Default is 'relativeScoreFusion'.

        Returns:
            List[TextSegment]: List of segment models.
        """
        model_class: Any = self.__model_class(**kwargs)
        collection_name = kwargs.get("collection_name", "default")
        limit = kwargs.get("limit", 1000)
        offset = kwargs.get("offset", 0)
        alpha = kwargs.get("alpha", 0.75)
        properties = kwargs.get("properties", None)
        fusion_type = kwargs.get("fusion_type", "relativeScoreFusion")

        # Prepare the query options
        query_opts = {
            'query': text,
            'alpha': alpha,
            'return_metadata': MetadataQuery(distance=True, score=True),
            'limit': limit,
            'offset': offset
        }

        # Add optional properties parameter if provided
        if properties:
            query_opts['query_properties'] = properties

        # Add fusion_type if provided
        if fusion_type:
            query_opts['fusion_type'] = fusion_type

        # If there's an optimized query, optionally use it for vector part
        optimize_query = kwargs.get("optimize_query", True)
        if optimize_query:
            optimized_text = self.__optimize_query_if_needed(text, **kwargs)
            # We might need to pass in the optimized text vector in the future

        collection = self.weaviate_client.collections.get(collection_name)
        response = collection.query.hybrid(**query_opts)

        return model_class.from_weaviate_response(response)

    async def hybrid_search_async(self, query: str, **kwargs: Dict[str, Any]) -> List[TextSegment]:
        """
        Perform a hybrid search asynchronously.

        Args:
            query (str): The query text.
            **kwargs (Dict[str, Any]): Additional keyword arguments.

        Returns:
            List[TextSegment]: List of segment models.
        """
        func = partial(self.hybrid_search, query, **kwargs)
        return await asyncio.get_event_loop().run_in_executor(None, func)

    def bm25_search(self, text: str, **kwargs) -> List[TextSegment]:
        """
        Perform a BM25 keyword search.

        Args:
            text (str): The query text.
            **kwargs (Dict[str, Any]): Additional keyword arguments.

        Kwargs:
            properties (List[str], optional): List of properties to limit the BM25 search to. Default is all text properties.

        Returns:
            List[TextSegment]: List of segment models.
        """
        model_class: Any = self.__model_class(**kwargs)
        collection_name = kwargs.get("collection_name", "default")
        limit = kwargs.get("limit", 1000)
        offset = kwargs.get("offset", 0)
        properties = kwargs.get("properties", None)

        # Prepare the query options
        query_opts = {
            'query': text,
            'return_metadata': MetadataQuery(score=True),
            'limit': limit,
            'offset': offset
        }

        # Add optional properties parameter if provided
        if properties:
            query_opts['query_properties'] = properties

        collection = self.weaviate_client.collections.get(collection_name)
        response = collection.query.bm25(**query_opts)

        return model_class.from_weaviate_response(response)

    async def bm25_search_async(self, query: str, **kwargs: Dict[str, Any]) -> List[TextSegment]:
        """
        Perform a BM25 keyword search asynchronously.

        Args:
            query (str): The query text.
            **kwargs (Dict[str, Any]): Additional keyword arguments.

        Returns:
            List[TextSegment]: List of segment models.
        """
        func = partial(self.bm25_search, query, **kwargs)
        return await asyncio.get_event_loop().run_in_executor(None, func)

    @staticmethod
    def __model_class(**kwargs: Dict[str, Any]) -> Any:
        """
        Gets the actual model class from the string name of it.

        Args:
            **kwargs (Dict[str, Any]): Additional keyword arguments.

        Kwargs:
            model (str, optional): The model name.

        Returns:
            Any: The model class object.
        """
        model_name: str = kwargs.get("segment_model", "TextSegment")
        return globals()[model_name]


    @staticmethod
    def __check_batch_result(results: dict):
        """
        Callback function to check batch results for errors and log them.

        Args:
            results (dict): The Weaviate batch creation return value.
        """
        if results is not None:
            for result in results:
                if "result" in result and "errors" in result["result"]:
                    if "error" in result["result"]["errors"]:
                        logging.error(f"Batch import failed with {result['result']}")

    @staticmethod
    def __optimize_query_if_needed(query: str, **kwargs: Dict[str, Any]) -> str:
        """
        Optimize the query via vector text transform if asked to in the kwargs.
        This is intended to be sage to call if optimize0_query is false.

        Args:
            query (str): The query text.
            **kwargs (Dict[str, Any]): Additional keyword arguments.

        Kwargs:
            optimize_query (bool, optional): Whether to optimize the query.
            transform_options (VectorTransformOptionsModel, optional): The transform options.

        Returns:
            str: The optimized query.
        """
        optimize_query: bool = kwargs.get("optimize_query", True)
        if optimize_query:
            transform_opts = kwargs.get("transform_options", VectorTransformOptionsModel())
            vtt: VectorTextTransformer = VectorTextTransformer(transform_opts)
            return vtt.transform_text(query)

        return query

    # TODO: Replcae with abstraction
    @staticmethod
    def __collection_object():
        return [
            {'name': "created", 'description': "The timestamp of the segment creation", 'data_type': DataType.DATE, 'index_filterable': True, 'index_searchable': False, "vectorize_property_name": True},
            {'name': "content", 'description': "The content of the segment to be displayed to the model", 'data_type': DataType.TEXT, 'index_filterable': False, 'index_searchable': False},
            {'name': "index_content", 'description': "The content of the segment to be indexed for search", 'data_type': DataType.TEXT, 'index_filterable': False, 'index_searchable': True, "vectorize_property_name": False},
            {'name': "sequence", 'description': "The sequence number of the segment", 'data_type': DataType.INT, 'index_filterable': True, 'index_searchable': False, "vectorize_property_name": False},
            {'name': "token_count", 'description': "The number of tokens in the content property", 'data_type': DataType.INT, 'index_filterable': True, 'index_searchable': False, "vectorize_property_name": False},
            {'name': "citation", 'description': "The citation of the segment, should be a URI/filename or other unique human readable ID for the model to cite.", 'data_type': DataType.TEXT, 'index_filterable': True, 'index_searchable': True, "vectorize_property_name": True},
            {'name': "parent_segment", 'description': "A UUID pointing to a larger, parent segment.", 'data_type': DataType.UUID, 'index_filterable': True, 'index_searchable': False, "vectorize_property_name": False}
        ]


    def create_collection(self, collection_name):
        try:
            collection = self.weaviate_client.collections.get(collection_name)
            if collection.shards() == 0:
                collection = self.weaviate_client.collections.create(collection_name, properties=self.__collection_object(), vectorizer_config=Configure.Vectorizer.text2vec_openai())
        except Exception as e:
            self.logger.info(f"Creating collection {collection_name}")
            collection = self.weaviate_client.collections.create(collection_name, properties=self.__collection_object(), vectorizer_config=Configure.Vectorizer.text2vec_openai())

        return collection