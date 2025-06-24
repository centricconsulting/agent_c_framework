# Dataframe Too Usage
This is a note to people using an agent with this specific dataframe tool.

## Key Issues
- large dataframes often exceed the size of the token window allowable for tool calls (~50k tokens)
- So this tool works well for loading a dataframe from a datasource, e.g. excel, csv, tool_cache, pkl, parquet, etc...
- But when performing filter/grouping/aggregating, often the dataframe is smaller than the tokens
- And most actions are a singular in nature, like sum revenue group by business unit.  Very few are chained actions like add a column with thhis expression, then group by this and aggregate that.

## Solution for this tool
- Any query (filter), group and aggregate, or aggregate function will return a copy of a modified dataframe, but leave teh original intact
- To allow for chaining, we have a prompt instruction that says after every step, save modified dataframe to cache, reload from cache into the dataframe tool, and perform the next step