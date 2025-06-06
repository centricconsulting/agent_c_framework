# Agent C - Reference tools
The tools in this directory are here to demonstrate the capabilities of the Agent C SDK.

## MCP Server
This package includes a robust MCP ToolChest Server implementation for exposing Agent C tools via the Model Context Protocol (MCP). See [README_mcp_server.md](README_mcp_server.md) for details.

When using the toolman persona, it will *NOT* create tools here, instead they will be created in the agent_c_demo area `agent_c_demo`.  

The tools here work and have proven to be stable over time.  Many of them are 'demonstration' or 'tutorial' level tools.

## Tools
- [dall_e](src/agent_c_tools/tools/dall_e) - A tool for generating images from text using OpenAI's DALL-E model
- [data_vis](src/agent_c_tools/tools/data_visualization) - A tool for visualizing data using Seaborn and Plotly library.  Requires the [dataframe](src/agent_c_tools/tools/dataframe) tool to enabled.
- [dataframe](src/agent_c_tools/tools/dataframe) - A tool for creating and manipulating dataframes in Python. Allows for loading from excel, csv, tool_cache or from other tools.  Can perform grouping/aggregations on the dataframe
- [mermaid](src/agent_c_tools/tools/mermaid_chart) - A tool for creating diagrams using the Mermaid library
- [rss](src/agent_c_tools/tools/rss) - A tool for reading RSS feeds
  - Centric
  - CNN
  - LA Times
  - OpenAI Blog
  - MIT
  - KD Nuggets
  - Towards AI and Data Science
  - AWS News
  - Azure News
- [user_bio](src/agent_c_tools/tools/user_bio) - A skeleton tool for updating a users first and last name to demonstrate how you can capture/use user bio information
- [user_preferences](src/agent_c_tools/tools/user_preferences) - A for updating a users preferences to demonstrate how you can capture/use user preferences using key/value pairs
- [weather](src/agent_c_tools/tools/weather) - A tool for getting the current weather for a location using the python's weather API and library
- [web](src/agent_c_tools/tools/web) - A tool for scraping web pages using headless Selenium browser. Can format pages as markdown and save to workspaces.
- [web_search](src/agent_c_tools/tools/web_search) - A collection of tools for searching elements of the web
  - duckduckgo - DuckDuckGo search engine queries
  - google via google serpapi - Google search engine via specified API
  - google trends - Get what's trending on google and related keyword searches
  - hackernews - Get top headlines from hacker news
  - NewsOrg.API - Get top Business News headlines
  - Seeking Alpha - Get top Stock News
  - Tavily - Use a LLM oriented search engine to return search results
  - Wikipedia - Limited to getting search results, not a specific wikipedia page
- [workspaces](src/agent_c_tools/tools/workspace) - A tool for managing workspaces in the agent_c_tools area.  This is a demonstration tool for managing workspaces and tools in the agent_c_tools area.  This allows you to read/write local file storage. Stubs exist for cloud storage areas for deployed agents.
- [youtube_transcript](src/agent_c_tools/tools/youtube_transcript) - A tool for getting the transcript of a youtube video using the youtube API