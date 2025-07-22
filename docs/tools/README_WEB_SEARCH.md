# WebSearchTools - Unified Web Search Interface

## What This Tool Does

The WebSearchTools provides a unified interface for searching the internet across multiple search engines and specialized content sources. This powerful tool automatically selects the best search engine based on your query type and requirements, giving you access to current information, news, research, and specialized content from a variety of sources.

**Key Features:**
- **Unified Interface**: Single tool that replaces multiple individual search tools
- **Intelligent Routing**: Automatically selects the optimal search engine for your query
- **Multiple Search Types**: Web, news, educational, research, tech, flights, and events
- **6 Search Engines**: DuckDuckGo, Google SERP, Tavily, Wikipedia, NewsAPI, and HackerNews
- **Flexible Configuration**: Works with or without API keys, gracefully falling back to available engines

## Available Search Methods

The WebSearchTools provides 8 specialized search methods:

### Core Search Methods
- **`web_search()`**: General web search with intelligent engine selection
- **`news_search()`**: News-specific search with date filtering capabilities
- **`educational_search()`**: Academic and educational content from Wikipedia and research sources
- **`research_search()`**: Deep research with content analysis and domain filtering
- **`tech_search()`**: Technology and programming content from HackerNews and tech sources

### Specialized Search Methods
- **`flights_search()`**: Flight search using Google Flights with pricing and scheduling
- **`events_search()`**: Event discovery using Google Events with location filtering
- **`get_engine_info()`**: Engine status, capabilities, and health monitoring

## Supported Search Engines

### Engines with API Keys (Optional)
- **Google SERP**: Web, news, flights, events (requires SERPAPI_API_KEY)
- **Tavily**: Research and content analysis (requires TAVILY_API_KEY)
- **NewsAPI**: News articles with advanced filtering (requires NEWSAPI_API_KEY)

### Free Engines (No API Key Required)
- **DuckDuckGo**: Privacy-focused web search
- **Wikipedia**: Educational and reference content
- **HackerNews**: Technology community discussions and news

## Practical Use Cases

- **Current Events**: Stay informed about breaking news and recent developments
- **Research Support**: Gather information from multiple sources on specific topics
- **Decision Making**: Access up-to-date information to inform choices and plans
- **Learning**: Find educational content and resources on subjects you're exploring
- **Travel Planning**: Research destinations, events, and flight information
- **Market Intelligence**: Monitor financial news and company developments

## Example Interactions

### General Web Search

**User**: "What are the latest developments in quantum computing?"

**Agent**: Uses `web_search(query="latest quantum computing developments", search_type="research")` to automatically route to research-optimized engines and provide comprehensive, up-to-date information.

### News Search with Date Filtering

**User**: "What happened in AI news this week?"

**Agent**: Uses `news_search(query="artificial intelligence", start_date="2024-01-15")` to find recent AI news articles from the past week.

### Educational Content Search

**User**: "Explain machine learning algorithms for beginners"

**Agent**: Uses `educational_search(query="machine learning algorithms beginner")` to find educational content from Wikipedia and academic sources.

### Technical Problem Solving

**User**: "I'm getting a 'connection refused' error in my Python application"

**Agent**: Uses `tech_search(query="Python connection refused error solutions")` to find technical discussions and solutions from HackerNews and programming communities.

### Flight and Event Search

**User**: "Find flights from LAX to JFK on March 15th"

**Agent**: Uses `flights_search(departure_id="LAX", arrival_id="JFK", outbound_date="2024-03-15")` to search Google Flights for available options.

**User**: "What events are happening in San Francisco this weekend?"

**Agent**: Uses `events_search(query="weekend events", location="San Francisco")` to find local events and activities.

## Agent Configuration

To enable WebSearchTools for an agent, add it to the agent's YAML configuration:

```yaml
version: 2
name: "My Agent"
key: "my_agent"
model_id: "claude-sonnet-4-20250514"
tools:
  - ThinkTools
  - WebSearchTools  # Add this line
  - WorkspaceTools
```

## Environment Configuration

The WebSearchTools works with or without API keys. For full functionality, configure these environment variables:

```bash
# Optional API keys for enhanced functionality
SERPAPI_API_KEY=your_serpapi_key_here      # For Google search, flights, events
TAVILY_API_KEY=your_tavily_key_here        # For research and content analysis
NEWSAPI_API_KEY=your_newsapi_key_here      # For news search
```

**Note**: Even without API keys, the tool provides full functionality using free engines (DuckDuckGo, Wikipedia, HackerNews).

## Important Considerations

### Information Sources

The tool can access information from various sources including:
- General web search results (Google)
- News articles from multiple publishers
- Wikipedia articles for background information
- Research databases for academic or specialized content
- Financial news sources for market information
- Technical forums for specialized topics

### Information Currency

- Search results reflect what's currently available online
- The agent can find information published minutes ago to years ago
- For very recent events (within hours), news search is typically most effective
- Historical information can be found but may require more specific search terms

### Search Effectiveness

For best results when requesting searches:
- Be specific about what information you're looking for
- Mention time frames if relevant (e.g., "in the past month")
- Specify if you want a particular type of information (news, research papers, etc.)
- Indicate if you need information from specific sources or domains

### Migration from Legacy Tools

If you're upgrading from individual web search tools, simply replace them with WebSearchTools:

**Before (Legacy)**:
```yaml
tools:
  - GoogleSerpTools
  - WikipediaTools
  - HackerNewsTools
  - TavilyResearchTools
```

**After (Unified)**:
```yaml
tools:
  - WebSearchTools
```

### Limitations

- Cannot access information behind logins or paywalls
- Very recent breaking news (within minutes) may not be indexed
- API-dependent engines require valid API keys for full functionality
- Search operations may have usage limits based on API quotas
- Some specialized searches (flights, events) require Google SERP API access

### Testing and Validation

To verify WebSearchTools is properly registered and configured:

```bash
# Run the registration test
python src/agent_c_tools/test_websearch_registration.py

# Check engine availability
# Use get_engine_info() method in agent to see which engines are available
```