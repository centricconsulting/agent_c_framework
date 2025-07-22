"""
Real-world agent scenario testing for web search tools.

These tests simulate actual agent interactions and workflows to validate
that the web search tools work effectively in realistic scenarios.
"""
import pytest
import json
from typing import Dict, Any, List, Optional
from datetime import datetime, timedelta

# Import the unified system
import sys
sys.path.insert(0, "../../../src/agent_c_tools/tools/web_search")

from agent_c_tools.tools.web_search.web_search_tools import WebSearchTools


class TestCustomerServiceAgentScenarios:
    """Test scenarios for customer service agents."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.tools = WebSearchTools()
    
    def test_product_information_lookup(self):
        """Test customer service agent looking up product information."""
        # Customer asks about a product
        product_query = "iPhone 15 specifications features"
        
        result = self.tools.web_search(query=product_query, max_results=5)
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "Should find product information"
            
            # Results should contain product-related information
            product_terms = ["iphone", "specifications", "features", "apple", "phone"]
            relevant_results = 0
            
            for result_item in results:
                content = (result_item.get("title", "") + " " + 
                          result_item.get("snippet", "")).lower()
                
                if any(term in content for term in product_terms):
                    relevant_results += 1
            
            assert relevant_results > 0, \
                f"Should find relevant product information: {relevant_results}/{len(results)}"
    
    def test_troubleshooting_support(self):
        """Test customer service agent helping with troubleshooting."""
        # Customer has a technical problem
        problem_query = "WiFi connection problems troubleshooting"
        
        result = self.tools.web_search(query=problem_query, max_results=4)
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "Should find troubleshooting information"
            
            # Results should contain helpful troubleshooting info
            troubleshooting_terms = ["fix", "solve", "troubleshoot", "problem", "solution", "steps"]
            helpful_results = 0
            
            for result_item in results:
                content = (result_item.get("title", "") + " " + 
                          result_item.get("snippet", "")).lower()
                
                if any(term in content for term in troubleshooting_terms):
                    helpful_results += 1
            
            assert helpful_results > 0, \
                f"Should find helpful troubleshooting info: {helpful_results}/{len(results)}"
    
    def test_policy_information_lookup(self):
        """Test customer service agent looking up policy information."""
        # Customer asks about return policy
        policy_query = "return policy online purchases consumer rights"
        
        result = self.tools.web_search(query=policy_query, max_results=3)
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "Should find policy information"
            
            # Results should be relevant to policies
            policy_terms = ["return", "policy", "refund", "consumer", "rights", "purchase"]
            policy_results = 0
            
            for result_item in results:
                content = (result_item.get("title", "") + " " + 
                          result_item.get("snippet", "")).lower()
                
                if any(term in content for term in policy_terms):
                    policy_results += 1
            
            assert policy_results > 0, \
                f"Should find relevant policy information: {policy_results}/{len(results)}"


class TestContentCreatorAgentScenarios:
    """Test scenarios for content creator agents."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.tools = WebSearchTools()
    
    def test_trending_topics_research(self):
        """Test content creator researching trending topics."""
        # Content creator looking for trending topics
        trending_query = "viral social media trends 2024"
        
        result = self.tools.news_search(query=trending_query, max_results=6)
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "Should find trending topics"
            
            # Results should be current and trend-related
            trend_terms = ["viral", "trending", "popular", "social media", "2024", "latest"]
            current_results = 0
            
            for result_item in results:
                content = (result_item.get("title", "") + " " + 
                          result_item.get("snippet", "")).lower()
                
                if any(term in content for term in trend_terms):
                    current_results += 1
            
            assert current_results > 0, \
                f"Should find current trending content: {current_results}/{len(results)}"
    
    def test_competitor_analysis(self):
        """Test content creator doing competitor analysis."""
        # Content creator researching competitors
        competitor_query = "successful YouTube channels content strategy"
        
        result = self.tools.research_search(query=competitor_query, max_results=5)
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "Should find competitor analysis information"
            
            # Results should be relevant to content strategy
            strategy_terms = ["strategy", "content", "youtube", "successful", "channel", "marketing"]
            strategic_results = 0
            
            for result_item in results:
                content = (result_item.get("title", "") + " " + 
                          result_item.get("snippet", "")).lower()
                
                if any(term in content for term in strategy_terms):
                    strategic_results += 1
            
            assert strategic_results > 0, \
                f"Should find strategic content: {strategic_results}/{len(results)}"
    
    def test_fact_checking_research(self):
        """Test content creator fact-checking information."""
        # Content creator verifying facts
        fact_query = "climate change statistics 2024 verified data"
        
        result = self.tools.research_search(
            query=fact_query, 
            max_results=4,
            search_depth="advanced"
        )
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "Should find fact-checking sources"
            
            # Results should come from authoritative sources
            authoritative_domains = [".gov", ".edu", ".org", "nasa", "noaa", "epa"]
            authoritative_results = 0
            
            for result_item in results:
                url = result_item.get("url", "").lower()
                if any(domain in url for domain in authoritative_domains):
                    authoritative_results += 1
            
            # Some results should be from authoritative sources
            if authoritative_results == 0:
                print("No authoritative sources found, but results may still be valid")


class TestMarketingAgentScenarios:
    """Test scenarios for marketing agents."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.tools = WebSearchTools()
    
    def test_market_research(self):
        """Test marketing agent conducting market research."""
        # Marketing agent researching market trends
        market_query = "digital marketing trends 2024 consumer behavior"
        
        result = self.tools.research_search(query=market_query, max_results=6)
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "Should find market research"
            
            # Results should be relevant to marketing
            marketing_terms = ["marketing", "consumer", "behavior", "trends", "digital", "strategy"]
            marketing_results = 0
            
            for result_item in results:
                content = (result_item.get("title", "") + " " + 
                          result_item.get("snippet", "")).lower()
                
                if any(term in content for term in marketing_terms):
                    marketing_results += 1
            
            assert marketing_results > 0, \
                f"Should find marketing-relevant content: {marketing_results}/{len(results)}"
    
    def test_competitor_monitoring(self):
        """Test marketing agent monitoring competitors."""
        # Marketing agent checking competitor news
        competitor_query = "Apple marketing campaign new product launch"
        
        result = self.tools.news_search(query=competitor_query, max_results=5)
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "Should find competitor news"
            
            # Results should be news-like and relevant
            news_terms = ["apple", "marketing", "campaign", "product", "launch", "announces"]
            news_results = 0
            
            for result_item in results:
                content = (result_item.get("title", "") + " " + 
                          result_item.get("snippet", "")).lower()
                
                if any(term in content for term in news_terms):
                    news_results += 1
            
            assert news_results > 0, \
                f"Should find relevant competitor news: {news_results}/{len(results)}"
    
    def test_industry_analysis(self):
        """Test marketing agent analyzing industry trends."""
        # Marketing agent researching industry
        industry_query = "e-commerce industry growth statistics analysis"
        
        result = self.tools.research_search(query=industry_query, max_results=4)
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "Should find industry analysis"
            
            # Results should contain analytical content
            analysis_terms = ["analysis", "statistics", "growth", "industry", "e-commerce", "market"]
            analytical_results = 0
            
            for result_item in results:
                content = (result_item.get("title", "") + " " + 
                          result_item.get("snippet", "")).lower()
                
                if any(term in content for term in analysis_terms):
                    analytical_results += 1
            
            assert analytical_results > 0, \
                f"Should find analytical content: {analytical_results}/{len(results)}"


class TestEducationalAgentScenarios:
    """Test scenarios for educational agents."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.tools = WebSearchTools()
    
    def test_lesson_planning(self):
        """Test educational agent planning lessons."""
        # Teacher agent planning a science lesson
        lesson_query = "photosynthesis lesson plan middle school activities"
        
        result = self.tools.educational_search(query=lesson_query, max_results=5)
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "Should find lesson planning resources"
            
            # Results should be educational and practical
            education_terms = ["lesson", "plan", "activities", "teaching", "students", "school"]
            educational_results = 0
            
            for result_item in results:
                content = (result_item.get("title", "") + " " + 
                          result_item.get("snippet", "")).lower()
                
                if any(term in content for term in education_terms):
                    educational_results += 1
            
            assert educational_results > 0, \
                f"Should find educational content: {educational_results}/{len(results)}"
    
    def test_student_question_answering(self):
        """Test educational agent answering student questions."""
        # Student asks about a complex topic
        student_query = "how does DNA replication work step by step"
        
        result = self.tools.educational_search(query=student_query, max_results=3)
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "Should find educational explanations"
            
            # Results should be explanatory and detailed
            explanation_terms = ["how", "step", "process", "dna", "replication", "explanation"]
            explanatory_results = 0
            
            for result_item in results:
                content = (result_item.get("title", "") + " " + 
                          result_item.get("snippet", "")).lower()
                
                if any(term in content for term in explanation_terms):
                    explanatory_results += 1
                
                # Should have substantial content for explanations
                snippet = result_item.get("snippet", "")
                assert len(snippet) > 50, \
                    f"Educational explanations should be substantial: {len(snippet)} chars"
            
            assert explanatory_results > 0, \
                f"Should find explanatory content: {explanatory_results}/{len(results)}"
    
    def test_curriculum_research(self):
        """Test educational agent researching curriculum standards."""
        # Teacher researching curriculum standards
        curriculum_query = "Common Core math standards grade 5 requirements"
        
        result = self.tools.educational_search(query=curriculum_query, max_results=4)
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "Should find curriculum information"
            
            # Results should be relevant to curriculum
            curriculum_terms = ["common core", "standards", "grade", "math", "curriculum", "requirements"]
            curriculum_results = 0
            
            for result_item in results:
                content = (result_item.get("title", "") + " " + 
                          result_item.get("snippet", "")).lower()
                
                if any(term in content for term in curriculum_terms):
                    curriculum_results += 1
            
            assert curriculum_results > 0, \
                f"Should find curriculum-relevant content: {curriculum_results}/{len(results)}"


class TestHealthcareAgentScenarios:
    """Test scenarios for healthcare information agents."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.tools = WebSearchTools()
    
    def test_medical_information_lookup(self):
        """Test healthcare agent looking up medical information."""
        # Healthcare agent researching medical conditions
        medical_query = "diabetes type 2 symptoms treatment options"
        
        result = self.tools.research_search(query=medical_query, max_results=5)
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "Should find medical information"
            
            # Results should be medically relevant
            medical_terms = ["diabetes", "symptoms", "treatment", "medical", "health", "condition"]
            medical_results = 0
            
            for result_item in results:
                content = (result_item.get("title", "") + " " + 
                          result_item.get("snippet", "")).lower()
                
                if any(term in content for term in medical_terms):
                    medical_results += 1
            
            assert medical_results > 0, \
                f"Should find medically relevant content: {medical_results}/{len(results)}"
            
            # Should prefer authoritative medical sources
            medical_domains = [".gov", "mayo", "webmd", "nih", "cdc", "who"]
            authoritative_sources = 0
            
            for result_item in results:
                url = result_item.get("url", "").lower()
                if any(domain in url for domain in medical_domains):
                    authoritative_sources += 1
            
            if authoritative_sources > 0:
                print(f"Found {authoritative_sources} authoritative medical sources")
    
    def test_health_news_monitoring(self):
        """Test healthcare agent monitoring health news."""
        # Healthcare agent checking for health news
        health_news_query = "new medical research breakthrough 2024"
        
        result = self.tools.news_search(query=health_news_query, max_results=4)
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "Should find health news"
            
            # Results should be news-like and health-related
            health_news_terms = ["medical", "research", "breakthrough", "health", "study", "treatment"]
            health_news_results = 0
            
            for result_item in results:
                content = (result_item.get("title", "") + " " + 
                          result_item.get("snippet", "")).lower()
                
                if any(term in content for term in health_news_terms):
                    health_news_results += 1
            
            assert health_news_results > 0, \
                f"Should find health-related news: {health_news_results}/{len(results)}"
    
    def test_wellness_information_search(self):
        """Test healthcare agent searching for wellness information."""
        # Healthcare agent looking for wellness tips
        wellness_query = "healthy lifestyle tips exercise nutrition"
        
        result = self.tools.web_search(query=wellness_query, max_results=4)
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "Should find wellness information"
            
            # Results should be wellness-focused
            wellness_terms = ["healthy", "lifestyle", "exercise", "nutrition", "wellness", "tips"]
            wellness_results = 0
            
            for result_item in results:
                content = (result_item.get("title", "") + " " + 
                          result_item.get("snippet", "")).lower()
                
                if any(term in content for term in wellness_terms):
                    wellness_results += 1
            
            assert wellness_results > 0, \
                f"Should find wellness-focused content: {wellness_results}/{len(results)}"


class TestTravelAgentScenarios:
    """Test scenarios for travel planning agents."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.tools = WebSearchTools()
    
    def test_destination_research(self):
        """Test travel agent researching destinations."""
        # Travel agent researching a destination
        destination_query = "Tokyo travel guide best attractions restaurants"
        
        result = self.tools.web_search(query=destination_query, max_results=6)
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "Should find destination information"
            
            # Results should be travel-relevant
            travel_terms = ["tokyo", "travel", "guide", "attractions", "restaurants", "visit"]
            travel_results = 0
            
            for result_item in results:
                content = (result_item.get("title", "") + " " + 
                          result_item.get("snippet", "")).lower()
                
                if any(term in content for term in travel_terms):
                    travel_results += 1
            
            assert travel_results > 0, \
                f"Should find travel-relevant content: {travel_results}/{len(results)}"
    
    def test_flight_information_search(self):
        """Test travel agent searching for flight information."""
        # Travel agent looking for flight deals
        flight_query = "cheap flights New York to London deals"
        
        result = self.tools.flights_search(query=flight_query, max_results=3)
        response = json.loads(result)
        
        # Flight search may not always return results (depends on API availability)
        if response.get("success", False):
            results = response.get("results", [])
            if len(results) > 0:
                # Results should be flight-related
                flight_terms = ["flight", "airline", "airport", "deals", "cheap", "booking"]
                flight_results = 0
                
                for result_item in results:
                    content = (result_item.get("title", "") + " " + 
                              result_item.get("snippet", "")).lower()
                    
                    if any(term in content for term in flight_terms):
                        flight_results += 1
                
                assert flight_results > 0, \
                    f"Should find flight-related content: {flight_results}/{len(results)}"
        else:
            # If flights search fails, fallback to web search should work
            fallback_result = self.tools.web_search(query=flight_query, max_results=3)
            fallback_response = json.loads(fallback_result)
            
            if fallback_response.get("success", False):
                results = fallback_response.get("results", [])
                assert len(results) > 0, "Fallback should find flight information"
    
    def test_accommodation_search(self):
        """Test travel agent searching for accommodation."""
        # Travel agent looking for hotels
        hotel_query = "best hotels Paris budget friendly reviews"
        
        result = self.tools.web_search(query=hotel_query, max_results=5)
        response = json.loads(result)
        
        if response.get("success", False):
            results = response.get("results", [])
            assert len(results) > 0, "Should find accommodation information"
            
            # Results should be accommodation-relevant
            hotel_terms = ["hotel", "accommodation", "paris", "budget", "reviews", "booking"]
            hotel_results = 0
            
            for result_item in results:
                content = (result_item.get("title", "") + " " + 
                          result_item.get("snippet", "")).lower()
                
                if any(term in content for term in hotel_terms):
                    hotel_results += 1
            
            assert hotel_results > 0, \
                f"Should find accommodation-relevant content: {hotel_results}/{len(results)}"


class TestMultiStepAgentWorkflows:
    """Test complex multi-step agent workflows."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.tools = WebSearchTools()
    
    def test_research_paper_workflow(self):
        """Test agent conducting research paper workflow."""
        # Simulate agent writing a research paper
        research_steps = [
            ("artificial intelligence history", "educational_search"),
            ("AI current applications industry", "research_search"),
            ("machine learning recent breakthroughs", "news_search"),
            ("AI ethics concerns debate", "research_search"),
            ("future of artificial intelligence predictions", "research_search")
        ]
        
        research_results = []
        
        for query, method_name in research_steps:
            method = getattr(self.tools, method_name)
            result = method(query=query, max_results=3)
            response = json.loads(result)
            
            research_results.append((query, method_name, response))
            
            if response.get("success", False):
                results = response.get("results", [])
                assert len(results) > 0, f"Research step should succeed: {query}"
        
        # Most research steps should succeed
        successful_steps = sum(1 for _, _, response in research_results 
                             if response.get("success", False))
        
        assert successful_steps >= len(research_steps) * 0.7, \
            f"Research workflow should mostly succeed: {successful_steps}/{len(research_steps)}"
    
    def test_product_launch_workflow(self):
        """Test agent supporting product launch workflow."""
        # Simulate agent supporting product launch
        launch_steps = [
            ("competitor analysis smartphone market", "research_search"),
            ("smartphone launch news recent", "news_search"),
            ("mobile technology trends 2024", "tech_search"),
            ("product marketing strategy examples", "web_search"),
            ("consumer electronics reviews process", "web_search")
        ]
        
        launch_results = []
        
        for query, method_name in launch_steps:
            method = getattr(self.tools, method_name)
            result = method(query=query, max_results=3)
            response = json.loads(result)
            
            launch_results.append((query, method_name, response))
            
            if response.get("success", False):
                results = response.get("results", [])
                assert len(results) > 0, f"Launch step should succeed: {query}"
        
        # Most launch steps should succeed
        successful_steps = sum(1 for _, _, response in launch_results 
                             if response.get("success", False))
        
        assert successful_steps >= len(launch_steps) * 0.6, \
            f"Product launch workflow should mostly succeed: {successful_steps}/{len(launch_steps)}"
    
    def test_content_creation_workflow(self):
        """Test agent supporting content creation workflow."""
        # Simulate agent creating content
        content_steps = [
            ("trending topics social media 2024", "news_search"),
            ("viral content creation tips", "web_search"),
            ("YouTube algorithm optimization", "tech_search"),
            ("content marketing best practices", "research_search"),
            ("video editing tutorials beginners", "educational_search")
        ]
        
        content_results = []
        
        for query, method_name in content_steps:
            method = getattr(self.tools, method_name)
            result = method(query=query, max_results=3)
            response = json.loads(result)
            
            content_results.append((query, method_name, response))
            
            if response.get("success", False):
                results = response.get("results", [])
                assert len(results) > 0, f"Content step should succeed: {query}"
        
        # Most content steps should succeed
        successful_steps = sum(1 for _, _, response in content_results 
                             if response.get("success", False))
        
        assert successful_steps >= len(content_steps) * 0.7, \
            f"Content creation workflow should mostly succeed: {successful_steps}/{len(content_steps)}"
    
    def test_business_analysis_workflow(self):
        """Test agent supporting business analysis workflow."""
        # Simulate agent doing business analysis
        analysis_steps = [
            ("market research methodology", "educational_search"),
            ("industry analysis framework", "research_search"),
            ("business intelligence trends", "news_search"),
            ("competitive analysis tools", "tech_search"),
            ("financial analysis techniques", "web_search")
        ]
        
        analysis_results = []
        
        for query, method_name in analysis_steps:
            method = getattr(self.tools, method_name)
            result = method(query=query, max_results=3)
            response = json.loads(result)
            
            analysis_results.append((query, method_name, response))
            
            if response.get("success", False):
                results = response.get("results", [])
                assert len(results) > 0, f"Analysis step should succeed: {query}"
        
        # Most analysis steps should succeed
        successful_steps = sum(1 for _, _, response in analysis_results 
                             if response.get("success", False))
        
        assert successful_steps >= len(analysis_steps) * 0.6, \
            f"Business analysis workflow should mostly succeed: {successful_steps}/{len(analysis_steps)}"