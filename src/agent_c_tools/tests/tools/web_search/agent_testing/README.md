# Agent Testing and User Experience Validation

This directory contains tests that validate the web search tools from an agent's perspective, focusing on usability, discoverability, and real-world scenarios.

## Test Structure

```
agent_testing/
├── conftest.py                      # Pytest configuration and agent-specific fixtures
├── pytest.ini                      # Pytest settings for agent testing
├── README.md                        # This file
├── test_agent_tool_discovery.py     # Tool discovery and schema understanding tests
├── test_agent_personas.py           # Tests for different agent personas
├── test_agent_usability.py          # Usability and user experience tests
└── test_real_agent_scenarios.py     # Real-world agent scenario tests
```

## Test Categories

### Tool Discovery Tests (`test_agent_tool_discovery.py`)
- **Method Discovery**: Tests that agents can discover available search methods
- **Schema Understanding**: Tests that agents understand JSON schemas and parameters
- **Parameter Validation**: Tests that parameter errors are agent-friendly
- **Engine Selection**: Tests that agents can understand and use engine selection
- **Error Handling**: Tests that error messages are clear and actionable

### Agent Persona Tests (`test_agent_personas.py`)
- **Researcher Agent**: Academic research scenarios with comprehensive information needs
- **News Analyst Agent**: Current events monitoring and trend analysis
- **Educational Agent**: Teaching and learning scenarios with clear explanations
- **Technical Agent**: Programming and development scenarios with community resources
- **Personal Assistant Agent**: Quick answers and practical information needs
- **Specialized Agents**: Travel, health, finance, and other domain-specific scenarios

### Usability Tests (`test_agent_usability.py`)
- **Minimal Usage**: Tests that agents can use tools with minimal parameters
- **Parameter Flexibility**: Tests various parameter combinations and usage patterns
- **Result Quality**: Tests relevance, completeness, and diversity of results
- **Error Clarity**: Tests that error messages are helpful and actionable
- **Workflow Support**: Tests support for common agent workflows

### Real Agent Scenarios (`test_real_agent_scenarios.py`)
- **Customer Service**: Product lookup, troubleshooting, policy information
- **Content Creation**: Trending topics, competitor analysis, fact-checking
- **Marketing**: Market research, competitor monitoring, industry analysis
- **Education**: Lesson planning, student questions, curriculum research
- **Healthcare**: Medical information, health news, wellness guidance
- **Travel**: Destination research, flight information, accommodation search
- **Multi-Step Workflows**: Complex scenarios requiring multiple search steps

## Running Agent Tests

### Prerequisites

1. **Network Connection**: Tests require internet connectivity for real searches
2. **Python Environment**: Ensure proper Python path configuration

### Basic Test Execution

```bash
# Run all agent tests
cd /project/src/agent_c_tools/tests/tools/web_search/agent_testing
pytest

# Run specific test categories
pytest -m "agent_testing"              # All agent tests
pytest -m "persona"                    # Agent persona tests
pytest -m "usability"                  # Usability tests
pytest -m "workflow"                   # Workflow tests
pytest -m "real_scenario"              # Real-world scenarios
```

### Test Filtering Options

```bash
# Run tests for specific agent types
pytest test_agent_personas.py::TestResearcherAgentPersona
pytest test_agent_personas.py::TestTechnicalAgentPersona
pytest test_agent_personas.py::TestEducationalAgentPersona

# Run usability tests
pytest test_agent_usability.py::TestAgentUsabilityBasics
pytest test_agent_usability.py::TestAgentResultQuality

# Run real scenario tests
pytest test_real_agent_scenarios.py::TestCustomerServiceAgentScenarios
pytest test_real_agent_scenarios.py::TestContentCreatorAgentScenarios

# Run tool discovery tests
pytest test_agent_tool_discovery.py::TestAgentToolDiscovery
pytest test_agent_tool_discovery.py::TestAgentParameterUnderstanding
```

### Performance Testing

```bash
# Run with performance monitoring
pytest --durations=0 -v

# Run specific workflow tests
pytest -m "workflow" -v

# Run with detailed output
pytest -v -s
```

## Test Fixtures and Utilities

### Agent Personas Fixture
Provides configurations for different agent types:
- **Researcher**: Academic research needs
- **News Analyst**: Current events monitoring
- **Educator**: Teaching and learning support
- **Technical**: Programming and development
- **Assistant**: General personal assistance
- **Customer Service**: Support and troubleshooting
- **Content Creator**: Content development and trends
- **Marketing**: Market research and analysis

### Test Query Collections
Organized test queries for different scenarios:
- **Factual**: Basic fact-finding queries
- **How-to**: Instructional and tutorial queries
- **Current Events**: News and trending topics
- **Educational**: Learning and teaching content
- **Technical**: Programming and development
- **Research**: Academic and professional research
- **Local**: Location-based queries
- **Product**: Product information and reviews

### Quality Validation
Tools for validating search result quality:
- **Relevance Scoring**: Word overlap and content matching
- **Completeness Checking**: Required fields and content length
- **Diversity Analysis**: Source variety and domain distribution
- **Authority Assessment**: Credible source identification

## Expected Agent Behaviors

### Tool Discovery
- **Method Enumeration**: Agents should discover all available search methods
- **Schema Understanding**: Agents should understand parameter requirements
- **Error Interpretation**: Agents should understand error messages
- **Engine Awareness**: Agents should understand engine capabilities

### Parameter Usage
- **Minimal Parameters**: Agents should work with just a query
- **Parameter Flexibility**: Agents should handle various parameter combinations
- **Validation Feedback**: Agents should receive clear parameter validation
- **Default Handling**: Agents should understand default behaviors

### Result Processing
- **Structure Understanding**: Agents should parse result structure correctly
- **Relevance Assessment**: Agents should evaluate result relevance
- **Quality Recognition**: Agents should identify high-quality sources
- **Diversity Appreciation**: Agents should value diverse perspectives

### Error Handling
- **Clear Messages**: Error messages should be agent-friendly
- **Corrective Guidance**: Errors should suggest corrections
- **Graceful Degradation**: Agents should handle failures gracefully
- **Fallback Awareness**: Agents should understand fallback mechanisms

## Persona-Specific Expectations

### Researcher Agent
- **Comprehensive Results**: Needs detailed, authoritative information
- **Source Diversity**: Values multiple perspectives and sources
- **Academic Preference**: Prefers scholarly and peer-reviewed content
- **Depth Over Breadth**: Values detailed content over quantity

### News Analyst Agent
- **Current Content**: Needs recent and trending information
- **Credible Sources**: Values established news organizations
- **Breaking News**: Needs real-time or near-real-time updates
- **Trend Identification**: Looks for patterns and emerging topics

### Educational Agent
- **Clear Explanations**: Needs structured, easy-to-understand content
- **Authoritative Sources**: Values educational and institutional sources
- **Step-by-Step Guidance**: Prefers instructional and tutorial content
- **Age-Appropriate**: Considers audience level and complexity

### Technical Agent
- **Practical Solutions**: Needs code examples and implementation details
- **Community Resources**: Values developer forums and discussions
- **Current Practices**: Needs up-to-date technical information
- **Problem-Solving**: Focuses on solutions and troubleshooting

### Personal Assistant Agent
- **Quick Answers**: Needs concise, direct responses
- **Practical Information**: Values actionable and useful content
- **Local Relevance**: Considers location and context
- **Reliability**: Needs consistent and dependable results

## Quality Metrics

### Relevance Metrics
- **Word Overlap**: Minimum 20% query word overlap in results
- **Content Matching**: Results should relate to query intent
- **Topic Consistency**: Results should stay on topic
- **Context Awareness**: Results should consider query context

### Completeness Metrics
- **Required Fields**: All results must have title, URL, snippet
- **Content Length**: Adequate content for decision-making
- **Information Depth**: Sufficient detail for agent needs
- **Metadata Availability**: Useful metadata for agent processing

### Diversity Metrics
- **Source Variety**: Results from multiple domains/sources
- **Perspective Range**: Different viewpoints and approaches
- **Content Types**: Mix of content formats and styles
- **Authority Levels**: Range from authoritative to community sources

### Usability Metrics
- **Success Rate**: Percentage of successful searches
- **Response Time**: Average time to get results
- **Error Rate**: Frequency of errors and failures
- **Recovery Rate**: Ability to recover from errors

## Troubleshooting

### Common Issues

1. **Import Errors**:
   ```bash
   # Ensure PYTHONPATH is set correctly
   export PYTHONPATH="../../../../src/agent_c_tools/src/agent_c_tools/tools/web_search"
   ```

2. **Network Issues**:
   ```bash
   # Test with simple queries first
   pytest test_agent_usability.py::TestAgentUsabilityBasics::test_minimal_parameter_usage -v
   ```

3. **Performance Issues**:
   ```bash
   # Run with timeout adjustments
   pytest --timeout=300
   ```

### Debug Mode

```bash
# Run with verbose output
pytest -v -s

# Run single test with debugging
pytest test_agent_personas.py::TestResearcherAgentPersona::test_comprehensive_research_query -v -s

# Show all print statements
pytest --capture=no
```

## Contributing

When adding new agent tests:

1. **Use Appropriate Fixtures**: Leverage existing personas and test data
2. **Follow Naming Conventions**: Use descriptive test names
3. **Test Real Scenarios**: Focus on realistic agent use cases
4. **Validate Quality**: Use quality validation fixtures
5. **Document Expectations**: Clearly state what agents should achieve
6. **Consider Edge Cases**: Test boundary conditions and error scenarios

## Success Criteria

Agent tests should validate:

- **✅ Discoverability**: Agents can find and understand available tools
- **✅ Usability**: Agents can use tools effectively with minimal friction
- **✅ Reliability**: Tools work consistently across different scenarios
- **✅ Quality**: Results meet agent needs and expectations
- **✅ Flexibility**: Tools adapt to different agent personas and use cases
- **✅ Error Handling**: Failures are handled gracefully with clear guidance
- **✅ Performance**: Tools respond within acceptable time limits
- **✅ Scalability**: Tools support complex multi-step workflows

These tests ensure that the unified web search tools provide an excellent user experience for AI agents across a wide range of use cases and scenarios.