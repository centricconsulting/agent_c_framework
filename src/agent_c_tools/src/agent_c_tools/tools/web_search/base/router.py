"""
Engine routing and selection logic.

This module provides intelligent routing of search requests to the most
appropriate search engine based on query analysis, search type, and
engine availability.
"""

import re
from typing import Dict, List, Optional, Pattern, Tuple
from datetime import datetime
from agent_c.util.structured_logging import get_logger

from .models import SearchType, SearchParameters
from .registry import EngineRegistry

logger = get_logger(__name__)


class QueryAnalyzer:
    """Analyzes search queries to determine optimal engine routing."""
    
    # Query analysis patterns for engine preference
    QUERY_PATTERNS = {
        'financial': {
            'patterns': [
                r'\b(stock|price|ticker|NYSE|NASDAQ|IPO|earnings|revenue|profit|loss|dividend)\b',
                r'\b[A-Z]{2,5}\b.*\b(stock|price|shares)\b',
                r'\$([\d,]+\.?\d*)\b.*(stock|price|market|cap)',
                r'\b(bull|bear|market|trading|investor|portfolio|hedge fund)\b',
                r'\b(SEC|10-K|10-Q|quarterly|annual report)\b'
            ],
            'preferred_engines': ['seeking_alpha', 'newsapi', 'google'],
            'search_type': SearchType.FINANCIAL
        },
        'technical': {
            'patterns': [
                r'\b(programming|coding|python|javascript|java|react|node|database|API|software|development)\b',
                r'\b(github|stackoverflow|tutorial|documentation|framework|library)\b',
                r'\b(startup|tech|YC|Y Combinator|venture|funding|Silicon Valley)\b',
                r'\b(algorithm|data structure|machine learning|AI|artificial intelligence)\b'
            ],
            'preferred_engines': ['hackernews', 'tavily', 'google'],
            'search_type': SearchType.TECH_COMMUNITY
        },
        'academic': {
            'patterns': [
                r'\b(research|study|university|academic|journal|paper|thesis|dissertation)\b',
                r'\b(science|biology|chemistry|physics|mathematics|history|literature)\b',
                r'\b(definition|explanation|how does|what is|theory|concept)\b',
                r'\b(peer review|citation|abstract|methodology)\b'
            ],
            'preferred_engines': ['wikipedia', 'google', 'tavily'],
            'search_type': SearchType.EDUCATIONAL
        },
        'news': {
            'patterns': [
                r'\b(breaking|news|today|latest|update|report|announced|confirmed)\b',
                r'\b(yesterday|today|this week|recent|current|now)\b',
                r'\b(politics|election|government|policy|law|regulation)\b',
                r'\b(headline|journalist|media|press release)\b'
            ],
            'preferred_engines': ['newsapi', 'google', 'seeking_alpha'],
            'search_type': SearchType.NEWS
        },
        'trends': {
            'patterns': [
                r'\b(trending|viral|popular|hot|buzz|social media)\b',
                r'\b(twitter|reddit|facebook|instagram|tiktok)\b',
                r'\b(meme|hashtag|influencer|social)\b'
            ],
            'preferred_engines': ['google_trends', 'hackernews', 'google'],
            'search_type': SearchType.TRENDS
        }
    }
    
    def __init__(self):
        """Initialize the query analyzer with compiled patterns."""
        self._compiled_patterns: Dict[str, List[Pattern]] = {}
        self._compile_patterns()
    
    def _compile_patterns(self) -> None:
        """Compile regex patterns for better performance."""
        for category, config in self.QUERY_PATTERNS.items():
            self._compiled_patterns[category] = [
                re.compile(pattern, re.IGNORECASE) 
                for pattern in config['patterns']
            ]
    
    def analyze_query(self, query: str) -> Dict[str, any]:
        """
        Analyze a query to determine characteristics and preferences.
        
        Args:
            query: The search query to analyze
            
        Returns:
            Dictionary with analysis results including:
            - detected_categories: List of detected query categories
            - preferred_engines: List of engines in preference order
            - confidence_scores: Confidence scores for each category
            - suggested_search_type: Recommended search type
        """
        results = {
            'detected_categories': [],
            'preferred_engines': [],
            'confidence_scores': {},
            'suggested_search_type': SearchType.WEB
        }
        
        query_lower = query.lower()
        category_scores = {}
        
        # Analyze query against each category
        for category, patterns in self._compiled_patterns.items():
            matches = 0
            total_patterns = len(patterns)
            
            for pattern in patterns:
                if pattern.search(query_lower):
                    matches += 1
            
            if matches > 0:
                confidence = matches / total_patterns
                category_scores[category] = confidence
                results['detected_categories'].append(category)
                results['confidence_scores'][category] = confidence
        
        # Sort categories by confidence
        sorted_categories = sorted(
            category_scores.items(), 
            key=lambda x: x[1], 
            reverse=True
        )
        
        # Determine preferred engines and search type
        if sorted_categories:
            top_category = sorted_categories[0][0]
            config = self.QUERY_PATTERNS[top_category]
            results['preferred_engines'] = config['preferred_engines'].copy()
            results['suggested_search_type'] = config['search_type']
        
        return results


class EngineRouter:
    """Routes search requests to the most appropriate engine."""
    
    # Search type to engine preferences mapping
    SEARCH_TYPE_PREFERENCES = {
        SearchType.WEB: ['tavily', 'google', 'duckduckgo'],
        SearchType.NEWS: ['newsapi', 'google', 'seeking_alpha'],
        SearchType.TRENDS: ['google_trends', 'hackernews'],
        SearchType.FLIGHTS: ['google'],
        SearchType.EVENTS: ['google'],
        SearchType.RESEARCH: ['tavily', 'google'],
        SearchType.EDUCATIONAL: ['wikipedia', 'google'],
        SearchType.TECH_COMMUNITY: ['hackernews', 'tavily'],
        SearchType.FINANCIAL: ['seeking_alpha', 'newsapi', 'google']
    }
    
    def __init__(self, registry: EngineRegistry):
        """
        Initialize the engine router.
        
        Args:
            registry: Engine registry for checking availability
        """
        self.registry = registry
        self.query_analyzer = QueryAnalyzer()
        self._routing_cache: Dict[str, Tuple[str, datetime]] = {}
        self._cache_ttl = 300  # 5 minutes
    
    def route_search_request(
        self, 
        params: SearchParameters,
        available_engines: Optional[List[str]] = None
    ) -> str:
        """
        Route a search request to the most appropriate engine.
        
        Args:
            params: Search parameters including query and preferences
            available_engines: Optional list of available engines to consider
            
        Returns:
            Name of the selected engine
            
        Raises:
            EngineUnavailableException: If no suitable engines are available
        """
        # Use provided available engines or get from registry
        if available_engines is None:
            available_engines = self.registry.get_healthy_engines()
        
        if not available_engines:
            from .engine import EngineUnavailableException
            raise EngineUnavailableException("No engines are currently available")
        
        # Step 1: Handle explicit engine selection
        if params.engine != "auto":
            if params.engine in available_engines:
                engine = self.registry.get_engine(params.engine)
                if engine and engine.supports_search_type(params.search_type):
                    logger.info(f"Using explicitly requested engine: {params.engine}")
                    return params.engine
                else:
                    logger.warning(
                        f"Requested engine '{params.engine}' doesn't support "
                        f"search type '{params.search_type.value}', falling back to auto-routing"
                    )
        
        # Step 2: Check routing cache
        cache_key = self._generate_cache_key(params)
        cached_result = self._get_cached_routing(cache_key)
        if cached_result and cached_result in available_engines:
            logger.debug(f"Using cached routing: {cached_result}")
            return cached_result
        
        # Step 3: Search type optimization
        optimized_engine = self._get_optimized_engine_for_search_type(
            params.search_type, available_engines
        )
        if optimized_engine:
            self._cache_routing(cache_key, optimized_engine)
            logger.info(f"Using search type optimized engine: {optimized_engine}")
            return optimized_engine
        
        # Step 4: Query analysis routing
        analyzed_engine = self._get_query_analyzed_engine(params, available_engines)
        if analyzed_engine:
            self._cache_routing(cache_key, analyzed_engine)
            logger.info(f"Using query-analyzed engine: {analyzed_engine}")
            return analyzed_engine
        
        # Step 5: Fallback to any available engine
        fallback_engine = self._get_fallback_engine(params.search_type, available_engines)
        if fallback_engine:
            self._cache_routing(cache_key, fallback_engine)
            logger.info(f"Using fallback engine: {fallback_engine}")
            return fallback_engine
        
        # No suitable engines found
        from .engine import EngineUnavailableException
        raise EngineUnavailableException(
            f"No engines available for search type: {params.search_type.value}"
        )
    
    def _get_optimized_engine_for_search_type(
        self, 
        search_type: SearchType, 
        available_engines: List[str]
    ) -> Optional[str]:
        """Get the optimal engine for a specific search type."""
        preferences = self.SEARCH_TYPE_PREFERENCES.get(search_type, [])
        
        for engine_name in preferences:
            if engine_name in available_engines:
                engine = self.registry.get_engine(engine_name)
                if engine and engine.supports_search_type(search_type):
                    return engine_name
        
        return None
    
    def _get_query_analyzed_engine(
        self, 
        params: SearchParameters, 
        available_engines: List[str]
    ) -> Optional[str]:
        """Use query analysis to determine the best engine."""
        analysis = self.query_analyzer.analyze_query(params.query)
        
        # If query analysis suggests a different search type, consider it
        suggested_type = analysis['suggested_search_type']
        if suggested_type != params.search_type:
            # Check if any engines support the suggested type
            for engine_name in analysis['preferred_engines']:
                if engine_name in available_engines:
                    engine = self.registry.get_engine(engine_name)
                    if engine and engine.supports_search_type(suggested_type):
                        logger.info(
                            f"Query analysis suggests search type '{suggested_type.value}' "
                            f"and engine '{engine_name}'"
                        )
                        return engine_name
        
        # Use preferred engines for the original search type
        for engine_name in analysis['preferred_engines']:
            if engine_name in available_engines:
                engine = self.registry.get_engine(engine_name)
                if engine and engine.supports_search_type(params.search_type):
                    return engine_name
        
        return None
    
    def _get_fallback_engine(
        self, 
        search_type: SearchType, 
        available_engines: List[str]
    ) -> Optional[str]:
        """Get any available engine that supports the search type."""
        for engine_name in available_engines:
            engine = self.registry.get_engine(engine_name)
            if engine and engine.supports_search_type(search_type):
                return engine_name
        
        return None
    
    def _generate_cache_key(self, params: SearchParameters) -> str:
        """Generate a cache key for routing decisions."""
        # Include factors that affect routing decisions
        key_parts = [
            params.search_type.value,
            params.engine,
            # Include a hash of the query for query-analysis caching
            str(hash(params.query.lower()) % 10000)  # Simplified hash for caching
        ]
        return "|".join(key_parts)
    
    def _get_cached_routing(self, cache_key: str) -> Optional[str]:
        """Get cached routing decision if still valid."""
        if cache_key in self._routing_cache:
            engine_name, timestamp = self._routing_cache[cache_key]
            if (datetime.now() - timestamp).total_seconds() < self._cache_ttl:
                return engine_name
            else:
                # Remove expired cache entry
                del self._routing_cache[cache_key]
        
        return None
    
    def _cache_routing(self, cache_key: str, engine_name: str) -> None:
        """Cache a routing decision."""
        self._routing_cache[cache_key] = (engine_name, datetime.now())
        
        # Clean up old cache entries periodically
        if len(self._routing_cache) > 1000:
            self._cleanup_cache()
    
    def _cleanup_cache(self) -> None:
        """Remove expired entries from the routing cache."""
        now = datetime.now()
        expired_keys = [
            key for key, (_, timestamp) in self._routing_cache.items()
            if (now - timestamp).total_seconds() > self._cache_ttl
        ]
        
        for key in expired_keys:
            del self._routing_cache[key]
        
        logger.debug(f"Cleaned up {len(expired_keys)} expired cache entries")
    
    def get_routing_recommendations(
        self, 
        params: SearchParameters
    ) -> Dict[str, any]:
        """
        Get detailed routing recommendations for a search request.
        
        Args:
            params: Search parameters
            
        Returns:
            Dictionary with routing analysis and recommendations
        """
        available_engines = self.registry.get_healthy_engines()
        query_analysis = self.query_analyzer.analyze_query(params.query)
        
        recommendations = {
            'available_engines': available_engines,
            'query_analysis': query_analysis,
            'search_type_preferences': self.SEARCH_TYPE_PREFERENCES.get(params.search_type, []),
            'recommended_engine': None,
            'reasoning': []
        }
        
        try:
            recommended_engine = self.route_search_request(params, available_engines)
            recommendations['recommended_engine'] = recommended_engine
            
            # Add reasoning
            if params.engine != "auto":
                recommendations['reasoning'].append(f"Explicit engine selection: {params.engine}")
            
            if query_analysis['detected_categories']:
                categories = ", ".join(query_analysis['detected_categories'])
                recommendations['reasoning'].append(f"Query categories detected: {categories}")
            
            search_type_prefs = self.SEARCH_TYPE_PREFERENCES.get(params.search_type, [])
            if recommended_engine in search_type_prefs:
                rank = search_type_prefs.index(recommended_engine) + 1
                recommendations['reasoning'].append(
                    f"Engine ranked #{rank} for search type '{params.search_type.value}'"
                )
            
        except Exception as e:
            recommendations['error'] = str(e)
        
        return recommendations
    
    def clear_cache(self) -> None:
        """Clear the routing cache."""
        self._routing_cache.clear()
        logger.info("Cleared routing cache")
    
    def get_cache_stats(self) -> Dict[str, int]:
        """Get statistics about the routing cache."""
        now = datetime.now()
        valid_entries = sum(
            1 for _, timestamp in self._routing_cache.values()
            if (now - timestamp).total_seconds() < self._cache_ttl
        )
        
        return {
            'total_entries': len(self._routing_cache),
            'valid_entries': valid_entries,
            'expired_entries': len(self._routing_cache) - valid_entries,
            'cache_ttl_seconds': self._cache_ttl
        }