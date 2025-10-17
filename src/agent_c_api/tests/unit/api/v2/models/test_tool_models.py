# tests/unit/api/v2/models/test_tool_models.py
import json
import pytest
from typing import Dict, List, Optional, Any
from pydantic import ValidationError

from agent_c_api.api.v2.models.tool_models import (
    ToolParameter, ToolInfo, ToolCategory, ToolsList,
    SessionTools, ToolsUpdate, ToolCall, ToolResult
)


@pytest.mark.unit
@pytest.mark.models
@pytest.mark.tools
class TestToolParameterModel:
    """Tests for the ToolParameter model.
    
    ToolParameter defines the parameters a tool accepts, including their name,
    type, description, whether they're required, and any default values.
    """
    
    def test_creation_minimal(self):
        """Test creation with minimal required fields."""
        param = ToolParameter(
            name="query",
            type="string",
            description="Search query"
        )
        assert param.name == "query"
        assert param.type == "string"
        assert param.description == "Search query"
        assert param.required is False  # Default value
        assert param.default is None  # Default value
    
    def test_creation_full(self):
        """Test creation with all available fields."""
        param = ToolParameter(
            name="max_results",
            type="integer",
            description="Maximum number of results",
            required=True,
            default=10
        )
        assert param.name == "max_results"
        assert param.type == "integer"
        assert param.description == "Maximum number of results"
        assert param.required is True
        assert param.default == 10
    
    def test_validation_constraints(self):
        """Test parameter validation constraints."""
        # Test with various parameter types
        param_types = ["string", "integer", "number", "boolean", "array", "object"]
        
        for p_type in param_types:
            param = ToolParameter(
                name=f"param_{p_type}",
                type=p_type,
                description=f"A {p_type} parameter"
            )
            assert param.type == p_type
            
        # Test with boolean required flag
        for req in [True, False]:
            param = ToolParameter(
                name="test_param",
                type="string",
                description="Test parameter",
                required=req
            )
            assert param.required is req
    
    def test_serialization(self):
        """Test serialization to and from JSON."""
        param = ToolParameter(
            name="limit",
            type="integer",
            description="Result limit",
            required=True,
            default=20
        )
        
        # Serialize to JSON
        json_str = param.model_dump_json()
        data = json.loads(json_str)
        
        # Verify JSON content
        assert data["name"] == "limit"
        assert data["type"] == "integer"
        assert data["description"] == "Result limit"
        assert data["required"] is True
        assert data["default"] == 20
        
        # Deserialize from JSON
        deserialized = ToolParameter.model_validate_json(json_str)
        assert deserialized.name == param.name
        assert deserialized.type == param.type
        assert deserialized.description == param.description
        assert deserialized.required == param.required
        assert deserialized.default == param.default
    
    def test_schema_documentation(self):
        """Test that schema documentation is correctly defined."""
        schema = ToolParameter.model_json_schema()
        
        # Check field descriptions
        for field in ["name", "type", "description", "required", "default"]:
            assert field in schema["properties"]
            assert "description" in schema["properties"][field]
        
        # Check for required fields
        assert "required" in schema
        assert set(schema["required"]) == {"name", "type", "description"}


@pytest.mark.unit
@pytest.mark.models
@pytest.mark.tools
class TestToolInfoModel:
    """Tests for the ToolInfo model.
    
    ToolInfo represents information about a tool, including its id, name,
    description, category, parameters, and usage examples.
    """
    
    def test_creation_minimal(self):
        """Test creation with minimal required fields."""
        tool = ToolInfo(
            id="search",
            name="Web Search",
            description="Search the web for information",
            category="Information"
        )
        assert tool.id == "search"
        assert tool.name == "Web Search"
        assert tool.description == "Search the web for information"
        assert tool.category == "Information"
        assert tool.parameters == []  # Default empty list
        assert tool.examples == []  # Default empty list
    
    def test_creation_full(self):
        """Test creation with all available fields."""
        param = ToolParameter(name="query", type="string", description="Search query")
        tool = ToolInfo(
            id="calculator",
            name="Calculator",
            description="Perform mathematical calculations",
            category="Utilities",
            parameters=[param],
            examples=["calculator 2 + 2", "calculator sin(30)"]
        )
        assert tool.id == "calculator"
        assert tool.name == "Calculator"
        assert tool.description == "Perform mathematical calculations"
        assert tool.category == "Utilities"
        assert len(tool.parameters) == 1
        assert tool.parameters[0].name == "query"
        assert tool.examples == ["calculator 2 + 2", "calculator sin(30)"]
    
    def test_multiple_parameters(self):
        """Test creation with multiple parameters."""
        params = [
            ToolParameter(name="query", type="string", description="Search query", required=True),
            ToolParameter(name="limit", type="integer", description="Result limit", default=10),
            ToolParameter(name="filter", type="string", description="Result filter")
        ]
        
        tool = ToolInfo(
            id="advanced_search",
            name="Advanced Search",
            description="Advanced search functionality",
            category="Search",
            parameters=params
        )
        
        assert len(tool.parameters) == 3
        assert tool.parameters[0].name == "query"
        assert tool.parameters[0].required is True
        assert tool.parameters[1].name == "limit"
        assert tool.parameters[1].default == 10
        assert tool.parameters[2].name == "filter"
    
    def test_serialization(self):
        """Test serialization to and from JSON."""
        param = ToolParameter(name="query", type="string", description="Search query")
        tool = ToolInfo(
            id="search",
            name="Web Search",
            description="Search the web for information",
            category="Information",
            parameters=[param],
            examples=["search pandas documentation", "search python lists"]
        )
        
        # Serialize to JSON
        json_str = tool.model_dump_json()
        data = json.loads(json_str)
        
        # Verify JSON content
        assert data["id"] == "search"
        assert data["name"] == "Web Search"
        assert len(data["parameters"]) == 1
        assert data["parameters"][0]["name"] == "query"
        assert len(data["examples"]) == 2
        
        # Deserialize from JSON
        deserialized = ToolInfo.model_validate_json(json_str)
        assert deserialized.id == tool.id
        assert deserialized.name == tool.name
        assert deserialized.category == tool.category
        assert len(deserialized.parameters) == 1
        assert deserialized.parameters[0].name == "query"
        assert deserialized.examples == tool.examples
    
    def test_schema_documentation(self):
        """Test that schema documentation is correctly defined."""
        schema = ToolInfo.model_json_schema()
        
        # Check field descriptions
        for field in ["id", "name", "description", "category", "parameters", "examples"]:
            assert field in schema["properties"]
            assert "description" in schema["properties"][field]
        
        # Check for required fields
        assert "required" in schema
        assert set(schema["required"]) == {"id", "name", "description", "category"}
        
        # Check parameters field type
        assert schema["properties"]["parameters"]["type"] == "array"
        assert "$ref" in schema["properties"]["parameters"]["items"]


@pytest.mark.unit
@pytest.mark.models
@pytest.mark.tools
class TestToolCategoryModel:
    """Tests for the ToolCategory model.
    
    ToolCategory represents a category of related tools, grouping them
    for organizational purposes.
    """
    
    def test_creation(self):
        """Test creation of a tool category with tools."""
        # Create a tool for the category
        tool = ToolInfo(
            id="search",
            name="Web Search",
            description="Search the web for information",
            category="Information"
        )
        
        # Create category
        category = ToolCategory(
            name="Information",
            description="Tools for finding information",
            tools=[tool]
        )
        
        assert category.name == "Information"
        assert category.description == "Tools for finding information"
        assert len(category.tools) == 1
        assert category.tools[0].id == "search"
    
    def test_empty_category(self):
        """Test creation of an empty category."""
        # Create category with no tools
        # Note: The model allows empty tools list, unlike what we expected
        category = ToolCategory(
            name="Empty",
            description="An empty category",
            tools=[]
        )
        
        assert category.name == "Empty"
        assert category.description == "An empty category"
        assert category.tools == []
    
    def test_multiple_tools(self):
        """Test category with multiple tools."""
        # Create multiple tools
        tools = [
            ToolInfo(
                id="search",
                name="Web Search",
                description="Search the web",
                category="Information"
            ),
            ToolInfo(
                id="news",
                name="News Service",
                description="Get latest news",
                category="Information"
            ),
            ToolInfo(
                id="wikipedia",
                name="Wikipedia",
                description="Search Wikipedia",
                category="Information"
            )
        ]
        
        # Create category with multiple tools
        category = ToolCategory(
            name="Information",
            description="Tools for finding information",
            tools=tools
        )
        
        assert len(category.tools) == 3
        assert category.tools[0].id == "search"
        assert category.tools[1].id == "news"
        assert category.tools[2].id == "wikipedia"
    
    def test_serialization(self):
        """Test serialization to and from JSON."""
        # Create a tool for the category
        tool = ToolInfo(
            id="search",
            name="Web Search",
            description="Search the web for information",
            category="Information"
        )
        
        # Create category
        category = ToolCategory(
            name="Information",
            description="Tools for finding information",
            tools=[tool]
        )
        
        # Serialize to JSON
        json_str = category.model_dump_json()
        data = json.loads(json_str)
        
        # Verify JSON content
        assert data["name"] == "Information"
        assert data["description"] == "Tools for finding information"
        assert len(data["tools"]) == 1
        assert data["tools"][0]["id"] == "search"
        
        # Deserialize from JSON
        deserialized = ToolCategory.model_validate_json(json_str)
        assert deserialized.name == category.name
        assert deserialized.description == category.description
        assert len(deserialized.tools) == 1
        assert deserialized.tools[0].id == "search"
    
    def test_schema_documentation(self):
        """Test that schema documentation is correctly defined."""
        schema = ToolCategory.model_json_schema()
        
        # Check field descriptions
        for field in ["name", "description", "tools"]:
            assert field in schema["properties"]
            assert "description" in schema["properties"][field]
        
        # Check for required fields
        assert "required" in schema
        assert set(schema["required"]) == {"name", "description", "tools"}
        
        # Check tools field type
        assert schema["properties"]["tools"]["type"] == "array"


@pytest.mark.unit
@pytest.mark.models
@pytest.mark.tools
class TestToolsListModel:
    """Tests for the ToolsList model.
    
    ToolsList provides a comprehensive list of available tools organized by categories.
    """
    
    def test_creation(self):
        """Test creation of a tools list with categories."""
        # Create tool and category
        tool = ToolInfo(
            id="search",
            name="Web Search",
            description="Search the web for information",
            category="Information"
        )
        category = ToolCategory(
            name="Information",
            description="Tools for finding information",
            tools=[tool]
        )
        
        # Create tools list
        tools_list = ToolsList(categories=[category])
        
        assert len(tools_list.categories) == 1
        assert tools_list.categories[0].name == "Information"
        assert len(tools_list.categories[0].tools) == 1
        assert tools_list.categories[0].tools[0].id == "search"
    
    def test_empty_list(self):
        """Test creation of an empty tools list."""
        # Create tools list with no categories
        # Note: The model allows empty categories list, unlike what we expected
        tools_list = ToolsList(categories=[])
        
        assert tools_list.categories == []
    
    def test_multiple_categories(self):
        """Test tools list with multiple categories."""
        # Create tools for different categories
        info_tools = [
            ToolInfo(
                id="search",
                name="Web Search",
                description="Search the web",
                category="Information"
            )
        ]
        
        utility_tools = [
            ToolInfo(
                id="calculator",
                name="Calculator",
                description="Perform calculations",
                category="Utilities"
            ),
            ToolInfo(
                id="timer",
                name="Timer",
                description="Set a timer",
                category="Utilities"
            )
        ]
        
        # Create categories
        info_category = ToolCategory(
            name="Information",
            description="Tools for finding information",
            tools=info_tools
        )
        
        utility_category = ToolCategory(
            name="Utilities",
            description="Utility tools",
            tools=utility_tools
        )
        
        # Create tools list with multiple categories
        tools_list = ToolsList(categories=[info_category, utility_category])
        
        assert len(tools_list.categories) == 2
        assert tools_list.categories[0].name == "Information"
        assert tools_list.categories[1].name == "Utilities"
        assert len(tools_list.categories[0].tools) == 1
        assert len(tools_list.categories[1].tools) == 2
    
    def test_serialization(self):
        """Test serialization to and from JSON."""
        # Create tool and category
        tool = ToolInfo(
            id="search",
            name="Web Search",
            description="Search the web for information",
            category="Information"
        )
        category = ToolCategory(
            name="Information",
            description="Tools for finding information",
            tools=[tool]
        )
        
        # Create tools list
        tools_list = ToolsList(categories=[category])
        
        # Serialize to JSON
        json_str = tools_list.model_dump_json()
        data = json.loads(json_str)
        
        # Verify JSON content
        assert len(data["categories"]) == 1
        assert data["categories"][0]["name"] == "Information"
        assert len(data["categories"][0]["tools"]) == 1
        assert data["categories"][0]["tools"][0]["id"] == "search"
        
        # Deserialize from JSON
        deserialized = ToolsList.model_validate_json(json_str)
        assert len(deserialized.categories) == 1
        assert deserialized.categories[0].name == "Information"
        assert len(deserialized.categories[0].tools) == 1
        assert deserialized.categories[0].tools[0].id == "search"
    
    def test_schema_documentation(self):
        """Test that schema documentation is correctly defined."""
        schema = ToolsList.model_json_schema()
        
        # Check field descriptions
        assert "categories" in schema["properties"]
        assert "description" in schema["properties"]["categories"]
        
        # Check for required fields
        assert "required" in schema
        assert "categories" in schema["required"]
        
        # Check categories field type
        assert schema["properties"]["categories"]["type"] == "array"


@pytest.mark.unit
@pytest.mark.models
@pytest.mark.tools
class TestSessionToolsModel:
    """Tests for the SessionTools model.
    
    SessionTools represents the tools currently enabled in a session and those
    that are available but not enabled.
    """
    
    def test_creation(self):
        """Test creation of a session tools object."""
        tools = SessionTools(
            enabled_tools=["search", "calculator"],
            available_tools=["weather", "translator"]
        )
        
        assert tools.enabled_tools == ["search", "calculator"]
        assert tools.available_tools == ["weather", "translator"]
    
    def test_empty_tools(self):
        """Test with empty tool lists."""
        # Both lists can be empty
        empty_tools = SessionTools(
            enabled_tools=[],
            available_tools=[]
        )
        
        assert empty_tools.enabled_tools == []
        assert empty_tools.available_tools == []
    
    def test_serialization(self):
        """Test serialization to and from JSON."""
        tools = SessionTools(
            enabled_tools=["search", "calculator"],
            available_tools=["weather", "translator"]
        )
        
        # Serialize to JSON
        json_str = tools.model_dump_json()
        data = json.loads(json_str)
        
        # Verify JSON content
        assert data["enabled_tools"] == ["search", "calculator"]
        assert data["available_tools"] == ["weather", "translator"]
        
        # Deserialize from JSON
        deserialized = SessionTools.model_validate_json(json_str)
        assert deserialized.enabled_tools == tools.enabled_tools
        assert deserialized.available_tools == tools.available_tools
    
    def test_schema_documentation(self):
        """Test that schema documentation is correctly defined."""
        schema = SessionTools.model_json_schema()
        
        # Check field descriptions
        for field in ["enabled_tools", "available_tools"]:
            assert field in schema["properties"]
            assert "description" in schema["properties"][field]
        
        # Check for required fields
        assert "required" in schema
        assert set(schema["required"]) == {"enabled_tools", "available_tools"}
        
        # Check field types
        assert schema["properties"]["enabled_tools"]["type"] == "array"
        assert schema["properties"]["available_tools"]["type"] == "array"


@pytest.mark.unit
@pytest.mark.models
@pytest.mark.tools
class TestToolsUpdateModel:
    """Tests for the ToolsUpdate model.
    
    ToolsUpdate contains parameters for updating the enabled tools in a session.
    """
    
    def test_creation(self):
        """Test creation of a tools update object."""
        update = ToolsUpdate(enabled_tools=["search", "calculator"])
        
        assert update.enabled_tools == ["search", "calculator"]
    
    def test_empty_update(self):
        """Test with empty tools list."""
        update = ToolsUpdate(enabled_tools=[])
        
        assert update.enabled_tools == []
    
    def test_serialization(self):
        """Test serialization to and from JSON."""
        update = ToolsUpdate(enabled_tools=["search", "calculator", "translator"])
        
        # Serialize to JSON
        json_str = update.model_dump_json()
        data = json.loads(json_str)
        
        # Verify JSON content
        assert data["enabled_tools"] == ["search", "calculator", "translator"]
        
        # Deserialize from JSON
        deserialized = ToolsUpdate.model_validate_json(json_str)
        assert deserialized.enabled_tools == update.enabled_tools
    
    def test_schema_documentation(self):
        """Test that schema documentation is correctly defined."""
        schema = ToolsUpdate.model_json_schema()
        
        # Check field descriptions
        assert "enabled_tools" in schema["properties"]
        assert "description" in schema["properties"]["enabled_tools"]
        
        # Check for required fields
        assert "required" in schema
        assert "enabled_tools" in schema["required"]
        
        # Check field type
        assert schema["properties"]["enabled_tools"]["type"] == "array"


@pytest.mark.unit
@pytest.mark.models
@pytest.mark.tools
class TestToolCallModel:
    """Tests for the ToolCall model.
    
    ToolCall represents a call to a tool with its parameters.
    """
    
    def test_creation(self):
        """Test creation of a tool call object."""
        call = ToolCall(
            tool_id="calculator",
            parameters={"expression": "2 + 2"}
        )
        
        assert call.tool_id == "calculator"
        assert call.parameters == {"expression": "2 + 2"}
    
    def test_complex_parameters(self):
        """Test with complex nested parameters."""
        call = ToolCall(
            tool_id="search",
            parameters={
                "query": "python fastapi",
                "filters": {
                    "date": "past_year",
                    "domains": ["docs.python.org", "fastapi.tiangolo.com"]
                },
                "limit": 5
            }
        )
        
        assert call.tool_id == "search"
        assert call.parameters["query"] == "python fastapi"
        assert call.parameters["filters"]["date"] == "past_year"
        assert "domains" in call.parameters["filters"]
        assert call.parameters["limit"] == 5
    
    def test_empty_parameters(self):
        """Test with empty parameters dictionary."""
        call = ToolCall(
            tool_id="status",
            parameters={}
        )
        
        assert call.tool_id == "status"
        assert call.parameters == {}
    
    def test_serialization(self):
        """Test serialization to and from JSON."""
        call = ToolCall(
            tool_id="calculator",
            parameters={"expression": "2 + 2"}
        )
        
        # Serialize to JSON
        json_str = call.model_dump_json()
        data = json.loads(json_str)
        
        # Verify JSON content
        assert data["tool_id"] == "calculator"
        assert data["parameters"] == {"expression": "2 + 2"}
        
        # Deserialize from JSON
        deserialized = ToolCall.model_validate_json(json_str)
        assert deserialized.tool_id == call.tool_id
        assert deserialized.parameters == call.parameters
    
    def test_schema_documentation(self):
        """Test that schema documentation is correctly defined."""
        schema = ToolCall.model_json_schema()
        
        # Check field descriptions
        for field in ["tool_id", "parameters"]:
            assert field in schema["properties"]
            assert "description" in schema["properties"][field]
        
        # Check for required fields
        assert "required" in schema
        assert set(schema["required"]) == {"tool_id", "parameters"}
        
        # Check field types
        assert schema["properties"]["tool_id"]["type"] == "string"
        assert schema["properties"]["parameters"]["type"] == "object"


@pytest.mark.unit
@pytest.mark.models
@pytest.mark.tools
class TestToolResultModel:
    """Tests for the ToolResult model.
    
    ToolResult represents the result of a tool call, including success status,
    the actual result value, and any error message if the call failed.
    """
    
    def test_creation_success(self):
        """Test creation of a successful tool result."""
        result = ToolResult(
            tool_id="calculator",
            success=True,
            result=4
        )
        
        assert result.tool_id == "calculator"
        assert result.success is True
        assert result.result == 4
        assert result.error is None
    
    def test_creation_error(self):
        """Test creation of a failed tool result with error."""
        result = ToolResult(
            tool_id="calculator",
            success=False,
            result=None,
            error="Invalid expression"
        )
        
        assert result.tool_id == "calculator"
        assert result.success is False
        assert result.result is None
        assert result.error == "Invalid expression"
    
    def test_complex_result(self):
        """Test with a complex result structure."""
        complex_result = {
            "items": [
                {"title": "Python Documentation", "url": "https://docs.python.org"},
                {"title": "FastAPI", "url": "https://fastapi.tiangolo.com"}
            ],
            "total": 2,
            "query_time_ms": 150
        }
        
        result = ToolResult(
            tool_id="search",
            success=True,
            result=complex_result
        )
        
        assert result.tool_id == "search"
        assert result.success is True
        assert result.result["total"] == 2
        assert len(result.result["items"]) == 2
        assert result.result["items"][0]["title"] == "Python Documentation"
    
    def test_serialization_success(self):
        """Test serialization of a successful result to and from JSON."""
        result = ToolResult(
            tool_id="calculator",
            success=True,
            result=4
        )
        
        # Serialize to JSON
        json_str = result.model_dump_json()
        data = json.loads(json_str)
        
        # Verify JSON content
        assert data["tool_id"] == "calculator"
        assert data["success"] is True
        assert data["result"] == 4
        assert data["error"] is None
        
        # Deserialize from JSON
        deserialized = ToolResult.model_validate_json(json_str)
        assert deserialized.tool_id == result.tool_id
        assert deserialized.success == result.success
        assert deserialized.result == result.result
        assert deserialized.error == result.error
    
    def test_serialization_error(self):
        """Test serialization of an error result to and from JSON."""
        result = ToolResult(
            tool_id="calculator",
            success=False,
            result=None,
            error="Invalid expression"
        )
        
        # Serialize to JSON
        json_str = result.model_dump_json()
        data = json.loads(json_str)
        
        # Verify JSON content
        assert data["tool_id"] == "calculator"
        assert data["success"] is False
        assert data["result"] is None
        assert data["error"] == "Invalid expression"
        
        # Deserialize from JSON
        deserialized = ToolResult.model_validate_json(json_str)
        assert deserialized.tool_id == result.tool_id
        assert deserialized.success == result.success
        assert deserialized.result == result.result
        assert deserialized.error == result.error
    
    def test_schema_documentation(self):
        """Test that schema documentation is correctly defined."""
        schema = ToolResult.model_json_schema()
        
        # Check field descriptions
        for field in ["tool_id", "success", "result", "error"]:
            assert field in schema["properties"]
            assert "description" in schema["properties"][field]
        
        # Check for required fields
        assert "required" in schema
        assert set(schema["required"]) >= {"tool_id", "success", "result"}
        
        # Check field types
        assert schema["properties"]["tool_id"]["type"] == "string"
        assert schema["properties"]["success"]["type"] == "boolean"
        # The error field might have a complex schema definition (nullable string)
        # So it could be represented in various ways in the schema
        assert "error" in schema["properties"]