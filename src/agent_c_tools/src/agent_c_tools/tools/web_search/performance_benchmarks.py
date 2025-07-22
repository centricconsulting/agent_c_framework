#!/usr/bin/env python3
"""
Performance Benchmarking Suite for WebSearchTools

This module provides comprehensive performance testing and benchmarking
for the unified web search system, including engine-specific performance
analysis, concurrent search testing, and optimization recommendations.
"""

import asyncio
import json
import time
import statistics
from datetime import datetime, timedelta
from typing import Dict, List, Optional, Tuple
import logging
from dataclasses import dataclass, asdict
import sys
import os

# Add the web_search directory to the path for imports
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

from web_search_tools import WebSearchTools

# Configure logging
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

@dataclass
class PerformanceMetrics:
    """Performance metrics for a single search operation"""
    engine: str
    search_type: str
    query: str
    response_time: float
    success: bool
    result_count: int
    error_message: Optional[str] = None
    cache_hit: bool = False
    timestamp: datetime = None
    
    def __post_init__(self):
        if self.timestamp is None:
            self.timestamp = datetime.now()

@dataclass
class EnginePerformanceProfile:
    """Performance profile for a specific engine"""
    engine_name: str
    total_requests: int
    successful_requests: int
    failed_requests: int
    average_response_time: float
    median_response_time: float
    min_response_time: float
    max_response_time: float
    success_rate: float
    average_result_count: float
    error_types: Dict[str, int]
    
class WebSearchPerformanceBenchmark:
    """Comprehensive performance benchmarking for WebSearchTools"""
    
    def __init__(self):
        self.search_tools = WebSearchTools()
        self.metrics: List[PerformanceMetrics] = []
        self.test_queries = {
            "web": [
                "artificial intelligence trends 2024",
                "climate change solutions",
                "quantum computing applications",
                "sustainable energy technologies",
                "machine learning algorithms"
            ],
            "news": [
                "AI breakthrough news",
                "technology industry updates",
                "renewable energy developments",
                "healthcare innovation news",
                "financial market trends"
            ],
            "research": [
                "machine learning bias detection research",
                "climate change impact studies",
                "quantum computing research papers",
                "sustainable technology research",
                "healthcare AI research"
            ],
            "educational": [
                "quantum mechanics principles",
                "machine learning basics",
                "climate science fundamentals",
                "renewable energy concepts",
                "artificial intelligence history"
            ],
            "tech": [
                "Python programming best practices",
                "React framework updates",
                "cloud computing trends",
                "cybersecurity developments",
                "software engineering practices"
            ]
        }
    
    async def run_comprehensive_benchmark(self) -> Dict:
        """Run comprehensive performance benchmark across all engines and search types"""
        
        logger.info("Starting comprehensive performance benchmark...")
        
        # Get available engines
        engine_info = await self.search_tools.get_engine_info()
        available_engines = [
            engine['name'] for engine in engine_info.get('available_engines', [])
            if engine['status'] == 'available'
        ]
        
        logger.info(f"Testing {len(available_engines)} available engines: {available_engines}")
        
        # Run benchmarks
        results = {
            "benchmark_summary": {
                "start_time": datetime.now().isoformat(),
                "engines_tested": available_engines,
                "total_tests": 0,
                "successful_tests": 0,
                "failed_tests": 0
            },
            "engine_profiles": {},
            "search_type_analysis": {},
            "concurrent_performance": {},
            "optimization_recommendations": []
        }
        
        # Test each engine individually
        for engine in available_engines:
            logger.info(f"Benchmarking engine: {engine}")
            engine_profile = await self._benchmark_engine(engine)
            results["engine_profiles"][engine] = asdict(engine_profile)
        
        # Test search types across engines
        for search_type in self.test_queries.keys():
            logger.info(f"Benchmarking search type: {search_type}")
            type_analysis = await self._benchmark_search_type(search_type)
            results["search_type_analysis"][search_type] = type_analysis
        
        # Test concurrent performance
        logger.info("Testing concurrent search performance...")
        concurrent_results = await self._benchmark_concurrent_searches()
        results["concurrent_performance"] = concurrent_results
        
        # Generate optimization recommendations
        results["optimization_recommendations"] = self._generate_optimization_recommendations()
        
        # Update summary
        results["benchmark_summary"].update({
            "end_time": datetime.now().isoformat(),
            "total_tests": len(self.metrics),
            "successful_tests": sum(1 for m in self.metrics if m.success),
            "failed_tests": sum(1 for m in self.metrics if not m.success),
            "overall_success_rate": sum(1 for m in self.metrics if m.success) / len(self.metrics) if self.metrics else 0
        })
        
        logger.info("Comprehensive benchmark completed")
        return results
    
    async def _benchmark_engine(self, engine: str) -> EnginePerformanceProfile:
        """Benchmark a specific engine across different search types"""
        
        engine_metrics = []
        
        # Test each search type with this engine
        for search_type, queries in self.test_queries.items():
            for query in queries[:3]:  # Test first 3 queries per type
                try:
                    start_time = time.time()
                    
                    # Perform search based on type
                    if search_type == "news":
                        result = await self.search_tools.news_search(
                            query=query, engine=engine, max_results=10
                        )
                    elif search_type == "research":
                        result = await self.search_tools.research_search(
                            query=query, engine=engine, max_results=10
                        )
                    elif search_type == "educational":
                        result = await self.search_tools.educational_search(
                            query=query, engine=engine, max_results=10
                        )
                    elif search_type == "tech":
                        result = await self.search_tools.tech_search(
                            query=query, engine=engine, max_results=10
                        )
                    else:
                        result = await self.search_tools.web_search(
                            query=query, engine=engine, max_results=15
                        )
                    
                    response_time = time.time() - start_time
                    
                    # Parse result to count results
                    result_data = json.loads(result) if isinstance(result, str) else result
                    result_count = len(result_data.get('results', []))
                    
                    metric = PerformanceMetrics(
                        engine=engine,
                        search_type=search_type,
                        query=query,
                        response_time=response_time,
                        success=True,
                        result_count=result_count
                    )
                    
                except Exception as e:
                    response_time = time.time() - start_time
                    metric = PerformanceMetrics(
                        engine=engine,
                        search_type=search_type,
                        query=query,
                        response_time=response_time,
                        success=False,
                        result_count=0,
                        error_message=str(e)
                    )
                
                engine_metrics.append(metric)
                self.metrics.append(metric)
                
                # Small delay to avoid overwhelming APIs
                await asyncio.sleep(0.5)
        
        return self._calculate_engine_profile(engine, engine_metrics)
    
    def _calculate_engine_profile(self, engine: str, metrics: List[PerformanceMetrics]) -> EnginePerformanceProfile:
        """Calculate performance profile for an engine"""
        
        successful_metrics = [m for m in metrics if m.success]
        failed_metrics = [m for m in metrics if not m.success]
        
        if successful_metrics:
            response_times = [m.response_time for m in successful_metrics]
            result_counts = [m.result_count for m in successful_metrics]
            
            avg_response_time = statistics.mean(response_times)
            median_response_time = statistics.median(response_times)
            min_response_time = min(response_times)
            max_response_time = max(response_times)
            avg_result_count = statistics.mean(result_counts)
        else:
            avg_response_time = median_response_time = min_response_time = max_response_time = 0
            avg_result_count = 0
        
        # Count error types
        error_types = {}
        for metric in failed_metrics:
            error_key = metric.error_message[:50] if metric.error_message else "Unknown error"
            error_types[error_key] = error_types.get(error_key, 0) + 1
        
        return EnginePerformanceProfile(
            engine_name=engine,
            total_requests=len(metrics),
            successful_requests=len(successful_metrics),
            failed_requests=len(failed_metrics),
            average_response_time=avg_response_time,
            median_response_time=median_response_time,
            min_response_time=min_response_time,
            max_response_time=max_response_time,
            success_rate=len(successful_metrics) / len(metrics) if metrics else 0,
            average_result_count=avg_result_count,
            error_types=error_types
        )
    
    async def _benchmark_search_type(self, search_type: str) -> Dict:
        """Benchmark a specific search type across available engines"""
        
        type_metrics = [m for m in self.metrics if m.search_type == search_type]
        
        if not type_metrics:
            return {"error": "No metrics available for this search type"}
        
        successful_metrics = [m for m in type_metrics if m.success]
        
        # Engine comparison for this search type
        engine_performance = {}
        for engine in set(m.engine for m in type_metrics):
            engine_metrics = [m for m in type_metrics if m.engine == engine]
            engine_successful = [m for m in engine_metrics if m.success]
            
            if engine_successful:
                engine_performance[engine] = {
                    "success_rate": len(engine_successful) / len(engine_metrics),
                    "average_response_time": statistics.mean([m.response_time for m in engine_successful]),
                    "average_result_count": statistics.mean([m.result_count for m in engine_successful])
                }
        
        # Overall statistics
        if successful_metrics:
            response_times = [m.response_time for m in successful_metrics]
            result_counts = [m.result_count for m in successful_metrics]
            
            overall_stats = {
                "total_tests": len(type_metrics),
                "successful_tests": len(successful_metrics),
                "success_rate": len(successful_metrics) / len(type_metrics),
                "average_response_time": statistics.mean(response_times),
                "median_response_time": statistics.median(response_times),
                "average_result_count": statistics.mean(result_counts),
                "best_performing_engine": max(engine_performance.items(), 
                                            key=lambda x: x[1]["success_rate"])[0] if engine_performance else None
            }
        else:
            overall_stats = {
                "total_tests": len(type_metrics),
                "successful_tests": 0,
                "success_rate": 0,
                "error": "No successful tests for this search type"
            }
        
        return {
            "overall_statistics": overall_stats,
            "engine_comparison": engine_performance,
            "recommendations": self._get_search_type_recommendations(search_type, engine_performance)
        }
    
    async def _benchmark_concurrent_searches(self) -> Dict:
        """Test concurrent search performance"""
        
        concurrent_levels = [1, 2, 5, 10, 20]
        concurrent_results = {}
        
        test_queries = [
            "artificial intelligence",
            "machine learning",
            "quantum computing",
            "blockchain technology",
            "renewable energy"
        ]
        
        for level in concurrent_levels:
            logger.info(f"Testing concurrent level: {level}")
            
            # Create tasks for concurrent execution
            tasks = []
            for i in range(level):
                query = test_queries[i % len(test_queries)]
                task = self._timed_search(query, "web")
                tasks.append(task)
            
            # Execute concurrently and measure total time
            start_time = time.time()
            results = await asyncio.gather(*tasks, return_exceptions=True)
            total_time = time.time() - start_time
            
            # Analyze results
            successful_results = [r for r in results if not isinstance(r, Exception) and r.get('success')]
            failed_results = [r for r in results if isinstance(r, Exception) or not r.get('success')]
            
            if successful_results:
                individual_times = [r['response_time'] for r in successful_results]
                avg_individual_time = statistics.mean(individual_times)
                
                concurrent_results[level] = {
                    "concurrent_level": level,
                    "total_execution_time": total_time,
                    "successful_requests": len(successful_results),
                    "failed_requests": len(failed_results),
                    "success_rate": len(successful_results) / level,
                    "average_individual_response_time": avg_individual_time,
                    "efficiency_ratio": (avg_individual_time * level) / total_time if total_time > 0 else 0,
                    "throughput_per_second": level / total_time if total_time > 0 else 0
                }
            else:
                concurrent_results[level] = {
                    "concurrent_level": level,
                    "total_execution_time": total_time,
                    "successful_requests": 0,
                    "failed_requests": level,
                    "success_rate": 0,
                    "error": "All concurrent requests failed"
                }
            
            # Small delay between concurrent level tests
            await asyncio.sleep(2)
        
        return {
            "concurrent_test_results": concurrent_results,
            "optimal_concurrent_level": self._find_optimal_concurrent_level(concurrent_results),
            "concurrent_recommendations": self._get_concurrent_recommendations(concurrent_results)
        }
    
    async def _timed_search(self, query: str, search_type: str) -> Dict:
        """Perform a timed search operation"""
        
        start_time = time.time()
        try:
            result = await self.search_tools.web_search(query=query, max_results=10)
            response_time = time.time() - start_time
            
            result_data = json.loads(result) if isinstance(result, str) else result
            result_count = len(result_data.get('results', []))
            
            return {
                "query": query,
                "search_type": search_type,
                "response_time": response_time,
                "success": True,
                "result_count": result_count
            }
        except Exception as e:
            response_time = time.time() - start_time
            return {
                "query": query,
                "search_type": search_type,
                "response_time": response_time,
                "success": False,
                "error": str(e)
            }
    
    def _find_optimal_concurrent_level(self, concurrent_results: Dict) -> Dict:
        """Find the optimal concurrent level based on efficiency and success rate"""
        
        valid_results = {
            level: data for level, data in concurrent_results.items()
            if data.get('success_rate', 0) > 0.8  # At least 80% success rate
        }
        
        if not valid_results:
            return {"optimal_level": 1, "reason": "No concurrent level achieved acceptable success rate"}
        
        # Find level with best efficiency ratio
        best_level = max(valid_results.items(), key=lambda x: x[1].get('efficiency_ratio', 0))
        
        return {
            "optimal_level": best_level[0],
            "efficiency_ratio": best_level[1]['efficiency_ratio'],
            "success_rate": best_level[1]['success_rate'],
            "throughput": best_level[1]['throughput_per_second'],
            "reason": "Best balance of efficiency and success rate"
        }
    
    def _generate_optimization_recommendations(self) -> List[Dict]:
        """Generate optimization recommendations based on benchmark results"""
        
        recommendations = []
        
        # Analyze engine performance
        engine_profiles = {}
        for metric in self.metrics:
            if metric.engine not in engine_profiles:
                engine_profiles[metric.engine] = []
            engine_profiles[metric.engine].append(metric)
        
        # Engine-specific recommendations
        for engine, metrics in engine_profiles.items():
            successful_metrics = [m for m in metrics if m.success]
            if successful_metrics:
                avg_response_time = statistics.mean([m.response_time for m in successful_metrics])
                success_rate = len(successful_metrics) / len(metrics)
                
                if avg_response_time > 5.0:
                    recommendations.append({
                        "type": "performance",
                        "priority": "high",
                        "engine": engine,
                        "issue": "Slow response time",
                        "recommendation": f"Consider implementing caching for {engine} or using as fallback only",
                        "metrics": {"average_response_time": avg_response_time}
                    })
                
                if success_rate < 0.8:
                    recommendations.append({
                        "type": "reliability",
                        "priority": "high",
                        "engine": engine,
                        "issue": "Low success rate",
                        "recommendation": f"Investigate {engine} configuration or implement better error handling",
                        "metrics": {"success_rate": success_rate}
                    })
        
        # General optimization recommendations
        all_successful = [m for m in self.metrics if m.success]
        if all_successful:
            avg_response_time = statistics.mean([m.response_time for m in all_successful])
            
            if avg_response_time > 3.0:
                recommendations.append({
                    "type": "caching",
                    "priority": "medium",
                    "recommendation": "Implement result caching to improve response times",
                    "expected_improvement": "50-70% reduction in response time for cached queries"
                })
            
            # Check for engines with consistently good performance
            fast_engines = []
            for engine, metrics in engine_profiles.items():
                successful = [m for m in metrics if m.success]
                if successful:
                    avg_time = statistics.mean([m.response_time for m in successful])
                    success_rate = len(successful) / len(metrics)
                    if avg_time < 2.0 and success_rate > 0.9:
                        fast_engines.append(engine)
            
            if fast_engines:
                recommendations.append({
                    "type": "routing",
                    "priority": "medium",
                    "recommendation": f"Prioritize fast, reliable engines: {', '.join(fast_engines)}",
                    "implementation": "Update engine routing logic to prefer these engines"
                })
        
        return recommendations
    
    def _get_search_type_recommendations(self, search_type: str, engine_performance: Dict) -> List[str]:
        """Get recommendations for a specific search type"""
        
        recommendations = []
        
        if not engine_performance:
            return ["No engines available for this search type"]
        
        # Find best performing engine
        best_engine = max(engine_performance.items(), key=lambda x: x[1]["success_rate"])
        recommendations.append(f"Best engine for {search_type}: {best_engine[0]} (success rate: {best_engine[1]['success_rate']:.1%})")
        
        # Find fastest engine
        fastest_engine = min(engine_performance.items(), key=lambda x: x[1]["average_response_time"])
        if fastest_engine[0] != best_engine[0]:
            recommendations.append(f"Fastest engine for {search_type}: {fastest_engine[0]} ({fastest_engine[1]['average_response_time']:.2f}s)")
        
        # Check for engines with poor performance
        poor_performers = [
            engine for engine, perf in engine_performance.items()
            if perf["success_rate"] < 0.7 or perf["average_response_time"] > 5.0
        ]
        
        if poor_performers:
            recommendations.append(f"Consider avoiding for {search_type}: {', '.join(poor_performers)}")
        
        return recommendations
    
    def _get_concurrent_recommendations(self, concurrent_results: Dict) -> List[str]:
        """Get recommendations for concurrent search optimization"""
        
        recommendations = []
        
        # Find the level with best throughput
        best_throughput = max(
            concurrent_results.items(),
            key=lambda x: x[1].get('throughput_per_second', 0)
        )
        
        recommendations.append(f"Optimal concurrent level: {best_throughput[0]} (throughput: {best_throughput[1]['throughput_per_second']:.2f} req/s)")
        
        # Check for performance degradation at higher levels
        degradation_levels = []
        for level, data in concurrent_results.items():
            if data.get('success_rate', 0) < 0.8:
                degradation_levels.append(level)
        
        if degradation_levels:
            recommendations.append(f"Performance degrades at concurrent levels: {', '.join(map(str, degradation_levels))}")
        
        # General concurrent recommendations
        recommendations.append("Implement semaphore-based concurrency control")
        recommendations.append("Consider connection pooling for better concurrent performance")
        recommendations.append("Monitor API rate limits when using high concurrency")
        
        return recommendations
    
    def save_benchmark_results(self, results: Dict, filename: str = None):
        """Save benchmark results to a JSON file"""
        
        if filename is None:
            timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
            filename = f"websearch_benchmark_{timestamp}.json"
        
        # Convert datetime objects to strings for JSON serialization
        def serialize_datetime(obj):
            if isinstance(obj, datetime):
                return obj.isoformat()
            raise TypeError(f"Object of type {type(obj)} is not JSON serializable")
        
        with open(filename, 'w') as f:
            json.dump(results, f, indent=2, default=serialize_datetime)
        
        logger.info(f"Benchmark results saved to {filename}")
        return filename

async def main():
    """Run the comprehensive benchmark suite"""
    
    print("🚀 Starting WebSearchTools Performance Benchmark Suite")
    print("=" * 60)
    
    benchmark = WebSearchPerformanceBenchmark()
    
    try:
        # Run comprehensive benchmark
        results = await benchmark.run_comprehensive_benchmark()
        
        # Save results
        filename = benchmark.save_benchmark_results(results)
        
        # Print summary
        print("\n📊 BENCHMARK SUMMARY")
        print("=" * 40)
        summary = results["benchmark_summary"]
        print(f"Total Tests: {summary['total_tests']}")
        print(f"Successful: {summary['successful_tests']}")
        print(f"Failed: {summary['failed_tests']}")
        print(f"Success Rate: {summary['overall_success_rate']:.1%}")
        
        print("\n🏆 TOP PERFORMING ENGINES")
        print("=" * 40)
        for engine, profile in results["engine_profiles"].items():
            print(f"{engine}: {profile['success_rate']:.1%} success, {profile['average_response_time']:.2f}s avg")
        
        print("\n🔧 KEY RECOMMENDATIONS")
        print("=" * 40)
        for i, rec in enumerate(results["optimization_recommendations"][:5], 1):
            print(f"{i}. {rec['recommendation']}")
        
        print(f"\n📁 Detailed results saved to: {filename}")
        
    except Exception as e:
        logger.error(f"Benchmark failed: {str(e)}")
        print(f"❌ Benchmark failed: {str(e)}")

if __name__ == "__main__":
    asyncio.run(main())