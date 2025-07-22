"""
Unified Web Search Tools - Master class for all web search functionality.

This module provides a unified interface for all web search engines,
handling routing, configuration, and standardized responses across
different search providers.
"""

import json
from typing import Dict, List, Optional, Any, Union
from datetime import datetime
from agent_c.util.structured_logging import get_logger

from agent_c.toolsets import json_schema, Toolset

from .base.models import (
    SearchParameters, SearchType, SafeSearchLevel, SearchDepth,
    SearchResponse, WebSearchConfig, EngineCapabilities
)
from .base.registry import get_global_registry
from .base.router import EngineRouter
from .base.validator import ParameterValidator
from .base.error_handler import ErrorHandler
from .base.config_manager import get_config_manager, validate_web_search_configuration
from .engines import (
    create_duckduckgo_engine, create_google_serp_engine, create_tavily_engine,
    create_wikipedia_engine, create_news_api_engine, create_hacker_news_engine
)

logger = get_logger(__name__)


class WebSearchTools(Toolset):
    """
    Unified web search toolset providing access to multiple search engines
    through a single, consistent interface.
    
    This toolset automatically routes search requests to the most appropriate
    engine based on query analysis, search type, and engine availability.
    """
    
    def __init__(self, **kwargs):
        """Initialize the unified web search toolset."""
        super().__init__(**kwargs, name='web_search')
        
        # Initialize core components
        self.registry = get_global_registry()
        self.router = EngineRouter(self.registry)
        self.validator = ParameterValidator()
        self.error_handler = ErrorHandler()
        self.config_manager = get_config_manager()
        
        # Register all available engines
        self._register_engines()
        
        # Validate configuration and log status
        self._validate_and_log_configuration()
        
        logger.info("WebSearchTools initialized successfully")
    
    def _register_engines(self) -> None:
        """Register all available search engines."""
        engines_to_register = [
            ('duckduckgo', create_duckduckgo_engine),
            ('google_serp', create_google_serp_engine),
            ('tavily', create_tavily_engine),
            ('wikipedia', create_wikipedia_engine),
            ('newsapi', create_news_api_engine),
            ('hackernews', create_hacker_news_engine)
        ]
        
        for engine_name, factory_func in engines_to_register:
            try:
                engine = factory_func()
                self.registry.register_engine_instance(engine_name, engine)
                logger.info(f"Registered engine: {engine_name}")
            except Exception as e:
                logger.error(f"Failed to register engine {engine_name}: {e}")
    
    def _validate_and_log_configuration(self) -> None:
        """Validate configuration and log status information."""
        try:
            is_valid, error_messages = validate_web_search_configuration()
            
            if is_valid:
                config_status = self.config_manager.get_configuration_status()
                logger.info(f"Web search configuration valid. Available engines: {config_status.available_engines}")
                
                # Log missing optional configurations
                if config_status.missing_api_keys:
                    logger.info(f"Optional API keys not configured: {config_status.missing_api_keys}")
                    logger.info("Run get_engine_info() for setup instructions")
            else:
                logger.warning("Web search configuration issues detected:")
                for error in error_messages:
                    logger.warning(f"  - {error}")
                logger.warning("Run get_engine_info() for detailed configuration status")
                
        except Exception as e:
            logger.error(f"Configuration validation failed: {e}")
    
    def get_configuration_summary(self) -> str:
        """Get a human-readable configuration summary."""
        return self.config_manager.get_configuration_summary()
    
    @json_schema(
        'Perform a comprehensive web search across multiple search engines. '
        'Automatically selects the best engine based on your query and search type.',
        {
            'query': {
                'type': 'string',
                'description': 'The search query to perform.',
                'required': True
            },
            'engine': {
                'type': 'string',
                'description': 'Specific search engine to use. Options: duckduckgo, google_serp, tavily, wikipedia, newsapi, hackernews, or "auto" for automatic selection.',
                'required': False,
                'default': 'auto'
            },
            'search_type': {
                'type': 'string',
                'enum': ['web', 'news', 'trends', 'flights', 'events', 'research', 'educational', 'tech_community', 'financial'],
                'description': 'Type of search to perform. Affects engine selection and result formatting.',
                'required': False,
                'default': 'web'
            },
            'max_results': {
                'type': 'integer',
                'description': 'Maximum number of search results to return.',
                'required': False,
                'default': 10,
                'minimum': 1,
                'maximum': 100
            },
            'safesearch': {
                'type': 'string',
                'enum': ['on', 'moderate', 'off'],
                'description': 'Safe search level for adult content filtering.',
                'required': False,
                'default': 'moderate'
            },
            'language': {
                'type': 'string',
                'description': 'Language code for search results (e.g., "en", "es", "fr").',
                'required': False,
                'default': 'en'
            },
            'region': {
                'type': 'string',
                'description': 'Region code for localized results (e.g., "us", "uk", "ca").',
                'required': False,
                'default': 'us'
            },
            'include_domains': {
                'type': 'array',
                'items': {'type': 'string'},
                'description': 'List of domains to include in search results.',
                'required': False
            },
            'exclude_domains': {
                'type': 'array',
                'items': {'type': 'string'},
                'description': 'List of domains to exclude from search results.',
                'required': False
            },
            'search_depth': {
                'type': 'string',
                'enum': ['basic', 'standard', 'advanced'],
                'description': 'Search depth level (for engines that support it).',
                'required': False,
                'default': 'standard'
            },
            'start_date': {
                'type': 'string',
                'format': 'date',
                'description': 'Start date for date-filtered searches (YYYY-MM-DD).',
                'required': False
            },
            'end_date': {
                'type': 'string',
                'format': 'date',
                'description': 'End date for date-filtered searches (YYYY-MM-DD).',
                'required': False
            },
            'page': {
                'type': 'integer',
                'description': 'Page number for paginated results.',
                'required': False,
                'default': 1,
                'minimum': 1
            },
            'include_images': {
                'type': 'boolean',
                'description': 'Include image results where supported.',
                'required': False,
                'default': False
            }
        }
    )
    async def web_search(self, **kwargs) -> str:
        """
        Perform a unified web search across multiple engines.
        
        Returns:
            JSON string containing search results in standardized format
        """
        try:
            # Validate and normalize parameters
            params = self._build_search_parameters(kwargs)
            
            # Route to appropriate engine
            engine_name = self.router.route_search_request(params)
            engine = self.registry.get_engine(engine_name)
            
            if not engine:
                raise Exception(f"Engine {engine_name} not available")
            
            # Execute search
            response = engine.execute_search(params)
            
            # Convert to JSON for return
            return json.dumps(response.to_dict(), indent=2, default=str)
            
        except Exception as e:
            # Check if this is a configuration error
            error_message = str(e)
            if 'api key' in error_message.lower() or 'not configured' in error_message.lower():
                # Provide configuration-specific error response
                config_status = self.config_manager.get_configuration_status()
                error_response = {
                    'success': False,
                    'error': 'Configuration Error',
                    'message': error_message,
                    'query': kwargs.get('query', ''),
                    'available_engines': config_status.available_engines,
                    'suggestion': 'Run get_engine_info() to see configuration status and setup instructions'
                }
                
                # Add specific setup instructions if available
                requested_engine = kwargs.get('engine', 'auto')
                if requested_engine != 'auto' and requested_engine in config_status.engine_configs:
                    engine_config = config_status.engine_configs[requested_engine]
                    if engine_config.configuration_hints:
                        error_response['setup_instructions'] = engine_config.configuration_hints
                
                return json.dumps(error_response, indent=2, default=str)
            else:
                # Use standard error handling for other errors
                error_response = self.error_handler.handle_search_error(e, kwargs.get('query', ''))
                return json.dumps(error_response, indent=2, default=str)
    
    @json_schema(
        'Search for news articles using news-specific search engines.',
        {
            'query': {
                'type': 'string',
                'description': 'The news search query to perform.',
                'required': True
            },
            'engine': {
                'type': 'string',
                'description': 'Specific news engine to use. Options: newsapi, google_serp, or "auto".',
                'required': False,
                'default': 'auto'
            },
            'max_results': {
                'type': 'integer',
                'description': 'Maximum number of news articles to return.',
                'required': False,
                'default': 10,
                'minimum': 1,
                'maximum': 100
            },
            'start_date': {
                'type': 'string',
                'format': 'date',
                'description': 'Start date for news search (YYYY-MM-DD).',
                'required': False
            },
            'end_date': {
                'type': 'string',
                'format': 'date',
                'description': 'End date for news search (YYYY-MM-DD).',
                'required': False
            },
            'language': {
                'type': 'string',
                'description': 'Language code for news results.',
                'required': False,
                'default': 'en'
            }
        }
    )
    async def news_search(self, **kwargs) -> str:
        """
        Search for news articles with news-optimized engines.
        
        Returns:
            JSON string containing news search results
        """
        # Set search type to news and delegate to web_search
        kwargs['search_type'] = 'news'
        return await self.web_search(**kwargs)
    
    @json_schema(
        'Search for academic and educational content.',
        {
            'query': {
                'type': 'string',
                'description': 'The educational search query to perform.',
                'required': True
            },
            'engine': {
                'type': 'string',
                'description': 'Specific educational engine to use. Options: wikipedia, tavily, google_serp, or "auto".',
                'required': False,
                'default': 'auto'
            },
            'max_results': {
                'type': 'integer',
                'description': 'Maximum number of educational results to return.',
                'required': False,
                'default': 10,
                'minimum': 1,
                'maximum': 50
            },
            'language': {
                'type': 'string',
                'description': 'Language code for educational content.',
                'required': False,
                'default': 'en'
            }
        }
    )
    async def educational_search(self, **kwargs) -> str:
        """
        Search for educational and academic content.
        
        Returns:
            JSON string containing educational search results
        """
        # Set search type to educational and delegate to web_search
        kwargs['search_type'] = 'educational'
        return await self.web_search(**kwargs)
    
    @json_schema(
        'Search for research content with deep analysis capabilities.',
        {
            'query': {
                'type': 'string',
                'description': 'The research query to perform.',
                'required': True
            },
            'engine': {
                'type': 'string',
                'description': 'Specific research engine to use. Options: tavily, google_serp, or "auto".',
                'required': False,
                'default': 'auto'
            },
            'search_depth': {
                'type': 'string',
                'enum': ['basic', 'standard', 'advanced'],
                'description': 'Depth of research analysis.',
                'required': False,
                'default': 'advanced'
            },
            'max_results': {
                'type': 'integer',
                'description': 'Maximum number of research results to return.',
                'required': False,
                'default': 5,
                'minimum': 1,
                'maximum': 20
            },
            'include_domains': {
                'type': 'array',
                'items': {'type': 'string'},
                'description': 'Specific domains to include in research.',
                'required': False
            },
            'exclude_domains': {
                'type': 'array',
                'items': {'type': 'string'},
                'description': 'Domains to exclude from research.',
                'required': False
            }
        }
    )
    async def research_search(self, **kwargs) -> str:
        """
        Perform deep research search with content analysis.
        
        Returns:
            JSON string containing research results with detailed content
        """
        # Set search type to research and delegate to web_search
        kwargs['search_type'] = 'research'
        return await self.web_search(**kwargs)
    
    @json_schema(
        'Search for technology and programming related content.',
        {
            'query': {
                'type': 'string',
                'description': 'The tech search query to perform.',
                'required': True
            },
            'engine': {
                'type': 'string',
                'description': 'Specific tech engine to use. Options: hackernews, tavily, google_serp, or "auto".',
                'required': False,
                'default': 'auto'
            },
            'max_results': {
                'type': 'integer',
                'description': 'Maximum number of tech results to return.',
                'required': False,
                'default': 10,
                'minimum': 1,
                'maximum': 50
            }
        }
    )
    async def tech_search(self, **kwargs) -> str:
        """
        Search for technology and programming content.
        
        Returns:
            JSON string containing tech community search results
        """
        # Set search type to tech_community and delegate to web_search
        kwargs['search_type'] = 'tech_community'
        return await self.web_search(**kwargs)
    
    @json_schema(
        'Search for flights using Google Flights.',
        {
            'departure_id': {
                'type': 'string',
                'description': 'Departure airport ID (e.g., "LAX", "JFK").',
                'required': True
            },
            'arrival_id': {
                'type': 'string',
                'description': 'Arrival airport ID (e.g., "LAX", "JFK").',
                'required': True
            },
            'outbound_date': {
                'type': 'string',
                'format': 'date',
                'description': 'Departure date (YYYY-MM-DD).',
                'required': True
            },
            'return_date': {
                'type': 'string',
                'format': 'date',
                'description': 'Return date (YYYY-MM-DD) for round-trip flights.',
                'required': False
            },
            'currency': {
                'type': 'string',
                'description': 'Currency code for price display.',
                'required': False,
                'default': 'USD'
            },
            'search_locale': {
                'type': 'string',
                'description': 'Search locale for results.',
                'required': False,
                'default': 'us'
            }
        }
    )
    async def flights_search(self, **kwargs) -> str:
        """
        Search for flights using Google Flights.
        
        Returns:
            JSON string containing flight search results
        """
        # Build flight search query
        departure = kwargs.get('departure_id', '')
        arrival = kwargs.get('arrival_id', '')
        outbound = kwargs.get('outbound_date', '')
        
        query = f"flights from {departure} to {arrival} on {outbound}"
        if kwargs.get('return_date'):
            query += f" returning {kwargs['return_date']}"
        
        # Set up flight-specific parameters
        search_params = {
            'query': query,
            'search_type': 'flights',
            'engine': 'google_serp',  # Only Google supports flights
            'max_results': 20
        }
        
        # Add flight-specific additional parameters
        additional_params = {
            'departure_id': kwargs.get('departure_id'),
            'arrival_id': kwargs.get('arrival_id'),
            'outbound_date': kwargs.get('outbound_date'),
            'return_date': kwargs.get('return_date'),
            'currency': kwargs.get('currency', 'USD'),
            'search_locale': kwargs.get('search_locale', 'us')
        }
        
        # Filter out None values
        additional_params = {k: v for k, v in additional_params.items() if v is not None}
        search_params['additional_params'] = additional_params
        
        return await self.web_search(**search_params)
    
    @json_schema(
        'Search for events using Google Events.',
        {
            'query': {
                'type': 'string',
                'description': 'The event search query to perform.',
                'required': True
            },
            'location': {
                'type': 'string',
                'description': 'Location to search for events.',
                'required': False
            },
            'max_results': {
                'type': 'integer',
                'description': 'Maximum number of events to return.',
                'required': False,
                'default': 10,
                'minimum': 1,
                'maximum': 50
            }
        }
    )
    async def events_search(self, **kwargs) -> str:
        """
        Search for events using Google Events.
        
        Returns:
            JSON string containing event search results
        """
        # Set up event-specific parameters
        search_params = {
            'query': kwargs.get('query'),
            'search_type': 'events',
            'engine': 'google_serp',  # Only Google supports events
            'max_results': kwargs.get('max_results', 10)
        }
        
        # Add event-specific additional parameters
        if kwargs.get('location'):
            search_params['additional_params'] = {
                'location': kwargs['location']
            }
        
        return await self.web_search(**search_params)
    
    @json_schema(
        'Get information about available search engines and their capabilities.',
        {
            'engine_name': {
                'type': 'string',
                'description': 'Specific engine to get info about. If not provided, returns info for all engines.',
                'required': False
            },
            'include_setup_instructions': {
                'type': 'boolean',
                'description': 'Include setup instructions for missing configurations.',
                'required': False,
                'default': True
            }
        }
    )
    async def get_engine_info(self, **kwargs) -> str:
        """
        Get information about available search engines.
        
        Returns:
            JSON string containing engine information and capabilities
        """
        try:
            engine_name = kwargs.get('engine_name')
            include_setup = kwargs.get('include_setup_instructions', True)
            
            if engine_name:
                # Get info for specific engine
                engine = self.registry.get_engine(engine_name)
                if not engine:
                    # Check if it's a known engine that's not configured
                    config_status = self.config_manager.get_configuration_status()
                    if engine_name in config_status.engine_configs:
                        engine_config = config_status.engine_configs[engine_name]
                        info = {
                            'engine_name': engine_name,
                            'available': False,
                            'configuration_status': engine_config.to_dict(),
                            'error': f'Engine {engine_name} not available - configuration required'
                        }
                        if include_setup and engine_config.configuration_hints:
                            info['setup_instructions'] = engine_config.configuration_hints
                        return json.dumps(info, indent=2, default=str)
                    else:
                        return json.dumps({'error': f'Engine {engine_name} not found'})
                
                health_status = engine.get_health_status()
                supported_params = engine.get_supported_parameters()
                
                info = {
                    'engine_name': engine_name,
                    'available': True,
                    'health_status': health_status.to_dict(),
                    'supported_parameters': supported_params,
                    'capabilities': {
                        'search_types': [st.value for st in engine.capabilities.search_types],
                        'supports_pagination': engine.capabilities.supports_pagination,
                        'supports_date_filtering': engine.capabilities.supports_date_filtering,
                        'supports_domain_filtering': engine.capabilities.supports_domain_filtering,
                        'supports_safe_search': engine.capabilities.supports_safe_search,
                        'max_results_per_request': engine.capabilities.max_results_per_request
                    }
                }
            else:
                # Get comprehensive info for all engines
                config_status = self.config_manager.get_configuration_status()
                all_engines = self.registry.get_available_engines()
                health_status = self.registry.get_health_status_all()
                
                info = {
                    'configuration_summary': config_status.to_dict(),
                    'available_engines': all_engines,
                    'healthy_engines': self.registry.get_healthy_engines(),
                    'configured_engines': self.registry.get_engines_with_api_keys(),
                    'health_status': {name: status.to_dict() for name, status in health_status.items()},
                    'registry_stats': self.registry.get_registry_stats()
                }
                
                # Add setup instructions for missing configurations
                if include_setup and config_status.missing_api_keys:
                    missing_configs = self.config_manager.get_missing_configurations()
                    setup_instructions = {}
                    for engine_name, api_key_name, instructions in missing_configs:
                        setup_instructions[engine_name] = {
                            'api_key_name': api_key_name,
                            'instructions': instructions
                        }
                    info['setup_instructions'] = setup_instructions
                
                # Add configuration summary text
                info['configuration_summary_text'] = self.config_manager.get_configuration_summary()
            
            return json.dumps(info, indent=2, default=str)
            
        except Exception as e:
            logger.error(f"Error getting engine info: {e}")
            return json.dumps({
                'error': str(e),
                'suggestion': 'Check your environment variables and API key configuration'
            }, indent=2)
    
    def _build_search_parameters(self, kwargs: Dict[str, Any]) -> SearchParameters:
        """
        Build and validate SearchParameters from kwargs.
        
        Args:
            kwargs: Raw parameters from tool call
            
        Returns:
            Validated SearchParameters object
        """
        # Parse dates if provided
        start_date = None
        end_date = None
        
        if kwargs.get('start_date'):
            start_date = datetime.strptime(kwargs['start_date'], '%Y-%m-%d')
        if kwargs.get('end_date'):
            end_date = datetime.strptime(kwargs['end_date'], '%Y-%m-%d')
        
        # Build parameters
        params = SearchParameters(
            query=kwargs.get('query', ''),
            engine=kwargs.get('engine', 'auto'),
            search_type=SearchType(kwargs.get('search_type', 'web')),
            max_results=kwargs.get('max_results', 10),
            safesearch=SafeSearchLevel(kwargs.get('safesearch', 'moderate')),
            language=kwargs.get('language', 'en'),
            region=kwargs.get('region', 'us'),
            include_images=kwargs.get('include_images', False),
            include_domains=kwargs.get('include_domains'),
            exclude_domains=kwargs.get('exclude_domains'),
            search_depth=SearchDepth(kwargs.get('search_depth', 'standard')),
            start_date=start_date,
            end_date=end_date,
            page=kwargs.get('page', 1),
            additional_params=kwargs.get('additional_params', {})
        )
        
        # Validate parameters
        validation_result = self.validator.validate_parameters(params)
        if not validation_result.is_valid:
            raise ValueError(f"Parameter validation failed: {validation_result.errors}")
        
        return params


# Register the toolset
Toolset.register(WebSearchTools)