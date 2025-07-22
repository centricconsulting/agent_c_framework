"""
Pytest configuration and fixtures for agent testing.
"""
import pytest
import os
import sys
from typing import Dict, Any, List

# Add the web search tools to Python path
sys.path.insert(0, "../../../src/agent_c_tools/tools/web_search")


@pytest.fixture(scope="session")
def web_search_tools():
    """Fixture to provide WebSearchTools instance for agent testing."""
    from agent_c_tools.tools.web_search.web_search_tools import WebSearchTools
    return WebSearchTools()


@pytest.fixture(scope="session")
def agent_personas():
    """Fixture to provide different agent persona configurations."""
    return {
        "researcher": {
            "name": "Dr. Research",
            "role": "Academic Researcher",
            "needs": ["comprehensive information", "authoritative sources", "recent studies"],
            "preferences": ["detailed content", "academic sources", "multiple perspectives"],
            "search_methods": ["research_search", "educational_search"],
            "typical_queries": [
                "machine learning bias research",
                "climate change peer reviewed studies",
                "quantum computing applications"
            ]
        },
        "news_analyst": {
            "name": "NewsBot",
            "role": "News Analyst",
            "needs": ["current events", "breaking news", "trending topics"],
            "preferences": ["recent articles", "credible sources", "diverse viewpoints"],
            "search_methods": ["news_search", "web_search"],
            "typical_queries": [
                "technology industry layoffs",
                "political developments today",
                "economic indicators latest"
            ]
        },
        "educator": {
            "name": "EduBot",
            "role": "Educational Assistant",
            "needs": ["clear explanations", "authoritative sources", "structured information"],
            "preferences": ["educational content", "step-by-step guides", "verified facts"],
            "search_methods": ["educational_search", "web_search"],
            "typical_queries": [
                "photosynthesis explanation students",
                "algebra tutorial beginners",
                "historical events timeline"
            ]
        },
        "technical": {
            "name": "CodeBot",
            "role": "Technical Assistant",
            "needs": ["code examples", "technical documentation", "community discussions"],
            "preferences": ["developer resources", "practical solutions", "current best practices"],
            "search_methods": ["tech_search", "web_search"],
            "typical_queries": [
                "Python async programming",
                "React best practices",
                "database optimization techniques"
            ]
        },
        "assistant": {
            "name": "Assistant",
            "role": "Personal Assistant",
            "needs": ["quick answers", "practical information", "local results"],
            "preferences": ["concise responses", "actionable information", "reliable sources"],
            "search_methods": ["web_search"],
            "typical_queries": [
                "weather forecast today",
                "restaurant recommendations",
                "how to tie a tie"
            ]
        },
        "customer_service": {
            "name": "ServiceBot",
            "role": "Customer Service",
            "needs": ["product information", "troubleshooting guides", "policy details"],
            "preferences": ["accurate information", "step-by-step solutions", "official sources"],
            "search_methods": ["web_search", "educational_search"],
            "typical_queries": [
                "product specifications",
                "return policy information",
                "troubleshooting steps"
            ]
        },
        "content_creator": {
            "name": "ContentBot",
            "role": "Content Creator",
            "needs": ["trending topics", "competitor analysis", "audience insights"],
            "preferences": ["current trends", "viral content", "engagement metrics"],
            "search_methods": ["news_search", "research_search", "web_search"],
            "typical_queries": [
                "viral social media trends",
                "content marketing strategies",
                "audience engagement tips"
            ]
        },
        "marketing": {
            "name": "MarketBot",
            "role": "Marketing Specialist",
            "needs": ["market research", "competitor intelligence", "industry trends"],
            "preferences": ["data-driven insights", "current market data", "strategic information"],
            "search_methods": ["research_search", "news_search", "web_search"],
            "typical_queries": [
                "digital marketing trends",
                "competitor analysis",
                "consumer behavior insights"
            ]
        }
    }


@pytest.fixture(scope="function")
def agent_test_queries():
    """Fixture to provide test queries for different scenarios."""
    return {
        "factual": [
            "capital of France",
            "boiling point of water",
            "speed of light",
            "largest planet solar system"
        ],
        "how_to": [
            "how to tie a tie",
            "how to cook rice",
            "how to change a tire",
            "how to reset password"
        ],
        "current_events": [
            "latest technology news",
            "current political developments",
            "recent scientific discoveries",
            "trending social media topics"
        ],
        "educational": [
            "photosynthesis process",
            "World War 2 timeline",
            "algebra basics",
            "human anatomy systems"
        ],
        "technical": [
            "Python programming tutorial",
            "machine learning algorithms",
            "web development frameworks",
            "database optimization"
        ],
        "research": [
            "climate change research",
            "artificial intelligence ethics",
            "renewable energy studies",
            "medical breakthrough 2024"
        ],
        "local": [
            "restaurants near me",
            "weather forecast",
            "local news",
            "movie theaters nearby"
        ],
        "product": [
            "iPhone specifications",
            "laptop reviews",
            "car safety ratings",
            "book recommendations"
        ]
    }


@pytest.fixture(scope="function")
def search_quality_metrics():
    """Fixture to provide quality metrics for evaluating search results."""
    return {
        "relevance": {
            "min_word_overlap": 0.2,  # 20% of query words should appear in results
            "min_relevant_results": 0.5  # 50% of results should be relevant
        },
        "completeness": {
            "min_title_length": 5,
            "min_snippet_length": 20,
            "required_fields": ["title", "url", "snippet"]
        },
        "diversity": {
            "min_unique_domains": 2,  # Results should come from multiple sources
            "max_domain_dominance": 0.8  # No single domain should dominate
        },
        "freshness": {
            "current_indicators": ["2024", "latest", "recent", "new", "current", "today"],
            "min_fresh_results": 0.3  # 30% of results should appear fresh for current topics
        },
        "authority": {
            "educational_domains": [".edu", ".gov", ".org", "wikipedia", "britannica"],
            "news_domains": ["reuters", "ap", "bbc", "cnn", "npr"],
            "technical_domains": ["github", "stackoverflow", "developer", "docs"]
        }
    }


@pytest.fixture(scope="function")
def agent_workflow_scenarios():
    """Fixture to provide multi-step workflow scenarios."""
    return {
        "research_paper": [
            ("topic background", "educational_search"),
            ("current research", "research_search"),
            ("recent developments", "news_search"),
            ("expert opinions", "research_search"),
            ("future implications", "research_search")
        ],
        "product_launch": [
            ("market analysis", "research_search"),
            ("competitor news", "news_search"),
            ("technology trends", "tech_search"),
            ("marketing strategies", "web_search"),
            ("customer feedback", "web_search")
        ],
        "content_creation": [
            ("trending topics", "news_search"),
            ("viral content analysis", "web_search"),
            ("platform optimization", "tech_search"),
            ("audience insights", "research_search"),
            ("creation tutorials", "educational_search")
        ],
        "customer_support": [
            ("product information", "web_search"),
            ("troubleshooting guides", "educational_search"),
            ("policy details", "web_search"),
            ("known issues", "tech_search"),
            ("solution examples", "web_search")
        ],
        "business_analysis": [
            ("industry overview", "research_search"),
            ("market trends", "news_search"),
            ("competitive landscape", "research_search"),
            ("financial data", "web_search"),
            ("growth opportunities", "research_search")
        ]
    }


@pytest.fixture(scope="function")
def usability_test_cases():
    """Fixture to provide usability test cases."""
    return {
        "minimal_usage": [
            {"query": "Python tutorial"},
            {"query": "weather today"},
            {"query": "news headlines"}
        ],
        "parameter_flexibility": [
            {"query": "machine learning", "max_results": 3},
            {"query": "cooking recipes", "safe_search": "strict"},
            {"query": "travel guide", "engine": "duckduckgo"},
            {"query": "programming help", "language": "en"}
        ],
        "error_scenarios": [
            {"max_results": 5},  # Missing query
            {"query": "test", "max_results": -1},  # Invalid max_results
            {"query": "test", "safe_search": "invalid"},  # Invalid safe_search
            {"query": "test", "engine": "nonexistent"}  # Invalid engine
        ],
        "edge_cases": [
            {"query": ""},  # Empty query
            {"query": "a"},  # Very short query
            {"query": "x" * 1000},  # Very long query
            {"query": "!@#$%^&*()"},  # Special characters
            {"query": "query with unicode 中文"}  # Unicode characters
        ]
    }


def pytest_configure(config):
    """Configure pytest with custom markers for agent testing."""
    config.addinivalue_line(
        "markers", "agent_testing: marks tests as agent testing"
    )
    config.addinivalue_line(
        "markers", "persona: marks tests for specific agent personas"
    )
    config.addinivalue_line(
        "markers", "usability: marks tests for usability validation"
    )
    config.addinivalue_line(
        "markers", "workflow: marks tests for multi-step workflows"
    )
    config.addinivalue_line(
        "markers", "real_scenario: marks tests for real-world scenarios"
    )


def pytest_collection_modifyitems(config, items):
    """Modify test collection to add markers based on test names and paths."""
    for item in items:
        # Add agent_testing marker to all tests in agent_testing directory
        if "agent_testing" in str(item.fspath):
            item.add_marker(pytest.mark.agent_testing)
        
        # Add specific markers based on test file names
        if "persona" in item.name.lower():
            item.add_marker(pytest.mark.persona)
        
        if "usability" in item.name.lower():
            item.add_marker(pytest.mark.usability)
        
        if "workflow" in item.name.lower() or "scenario" in item.name.lower():
            item.add_marker(pytest.mark.workflow)
        
        if "real_agent" in str(item.fspath):
            item.add_marker(pytest.mark.real_scenario)


@pytest.fixture(scope="function")
def agent_performance_monitor():
    """Fixture to monitor agent performance during tests."""
    import time
    
    performance_data = {
        "start_time": time.time(),
        "search_count": 0,
        "successful_searches": 0,
        "failed_searches": 0,
        "total_results": 0,
        "response_times": []
    }
    
    def record_search(success: bool, result_count: int, response_time: float):
        performance_data["search_count"] += 1
        if success:
            performance_data["successful_searches"] += 1
            performance_data["total_results"] += result_count
        else:
            performance_data["failed_searches"] += 1
        performance_data["response_times"].append(response_time)
    
    def get_performance_summary():
        end_time = time.time()
        total_time = end_time - performance_data["start_time"]
        
        return {
            "total_time": total_time,
            "search_count": performance_data["search_count"],
            "success_rate": performance_data["successful_searches"] / max(performance_data["search_count"], 1),
            "avg_results_per_search": performance_data["total_results"] / max(performance_data["successful_searches"], 1),
            "avg_response_time": sum(performance_data["response_times"]) / max(len(performance_data["response_times"]), 1),
            "max_response_time": max(performance_data["response_times"]) if performance_data["response_times"] else 0
        }
    
    # Attach methods to the fixture
    performance_data["record_search"] = record_search
    performance_data["get_summary"] = get_performance_summary
    
    yield performance_data
    
    # Print performance summary at the end
    summary = get_performance_summary()
    print(f"\nAgent Performance Summary:")
    print(f"Total searches: {summary['search_count']}")
    print(f"Success rate: {summary['success_rate']:.2%}")
    print(f"Average results per search: {summary['avg_results_per_search']:.1f}")
    print(f"Average response time: {summary['avg_response_time']:.2f}s")
    print(f"Max response time: {summary['max_response_time']:.2f}s")


@pytest.fixture(scope="function")
def result_quality_validator():
    """Fixture to validate the quality of search results."""
    
    def validate_result_structure(result_item: Dict[str, Any]) -> List[str]:
        """Validate that a result item has the expected structure."""
        issues = []
        
        required_fields = ["title", "url", "snippet"]
        for field in required_fields:
            if field not in result_item:
                issues.append(f"Missing required field: {field}")
            elif not isinstance(result_item[field], str):
                issues.append(f"Field {field} should be a string")
            elif len(result_item[field].strip()) == 0:
                issues.append(f"Field {field} is empty")
        
        # Validate URL format
        if "url" in result_item:
            url = result_item["url"]
            if not url.startswith(("http://", "https://")):
                issues.append(f"Invalid URL format: {url}")
        
        return issues
    
    def validate_relevance(query: str, result_item: Dict[str, Any]) -> float:
        """Calculate relevance score for a result item."""
        query_words = set(query.lower().split())
        content = (result_item.get("title", "") + " " + result_item.get("snippet", "")).lower()
        content_words = set(content.split())
        
        if not query_words:
            return 0.0
        
        common_words = query_words.intersection(content_words)
        return len(common_words) / len(query_words)
    
    def validate_diversity(results: List[Dict[str, Any]]) -> Dict[str, Any]:
        """Validate diversity of search results."""
        if not results:
            return {"unique_domains": 0, "domain_distribution": {}}
        
        domains = []
        for result_item in results:
            url = result_item.get("url", "")
            if url:
                # Extract domain from URL
                import re
                domain_match = re.search(r'https?://([^/]+)', url)
                if domain_match:
                    domains.append(domain_match.group(1))
        
        unique_domains = len(set(domains))
        domain_distribution = {}
        for domain in domains:
            domain_distribution[domain] = domain_distribution.get(domain, 0) + 1
        
        return {
            "unique_domains": unique_domains,
            "domain_distribution": domain_distribution,
            "total_results": len(results)
        }
    
    return {
        "validate_structure": validate_result_structure,
        "validate_relevance": validate_relevance,
        "validate_diversity": validate_diversity
    }