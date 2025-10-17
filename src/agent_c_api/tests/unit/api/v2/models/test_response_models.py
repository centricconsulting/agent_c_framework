# tests/unit/api/v2/models/test_response_models.py
import json
import pytest
from typing import Dict, List, Optional, Any
from pydantic import BaseModel, ValidationError

from agent_c_api.api.v2.models.response_models import (
    APIStatus, APIResponse, PaginationMeta, PaginatedResponse
)

# Apply pytest markers
pytest.mark.unit
pytest.mark.models
pytest.mark.api_response


class TestAPIStatus:
    """Tests for the APIStatus model.
    
    APIStatus provides standardized status information for API responses,
    including success status, optional message, and optional error code.
    """
    
    def test_default_values(self):
        """Test that default values are correctly applied."""
        status = APIStatus()
        assert status.success is True
        assert status.message is None
        assert status.error_code is None
    
    def test_custom_values(self):
        """Test with custom values provided."""
        status = APIStatus(success=False, message="An error occurred", error_code="VALIDATION_ERROR")
        assert status.success is False
        assert status.message == "An error occurred"
        assert status.error_code == "VALIDATION_ERROR"
    
    def test_serialization(self):
        """Test serialization to and from JSON."""
        status = APIStatus(success=False, message="Test error", error_code="TEST_ERROR")
        json_str = status.model_dump_json()
        deserialized = APIStatus.model_validate_json(json_str)
        
        assert deserialized.success == status.success
        assert deserialized.message == status.message
        assert deserialized.error_code == status.error_code
    
    def test_schema_documentation(self):
        """Test that schema documentation is correctly defined."""
        schema = APIStatus.model_json_schema()
        
        # Check field descriptions
        assert "success" in schema["properties"]
        assert "description" in schema["properties"]["success"]
        assert "message" in schema["properties"]
        assert "description" in schema["properties"]["message"]
        assert "error_code" in schema["properties"]
        assert "description" in schema["properties"]["error_code"]
    
    def test_edge_cases(self):
        """Test edge cases like empty strings."""
        # Empty string message
        status = APIStatus(success=True, message="", error_code="")
        assert status.message == ""
        assert status.error_code == ""
        
        # Very long message
        long_message = "A" * 1000
        status = APIStatus(success=False, message=long_message)
        assert status.message == long_message


class ExampleData(BaseModel):
    """Simple test data model for use in APIResponse tests."""
    name: str
    value: int


class ComplexTestData(BaseModel):
    """More complex test data model with nested structures."""
    title: str
    items: List[ExampleData]
    metadata: Dict[str, Any] = {}


class TestAPIResponse:
    """Tests for the APIResponse generic model.
    
    APIResponse is a generic wrapper that includes status information
    and optional data of any type.
    """
    
    def test_with_default_status(self):
        """Test with default status (success=True)."""
        response = APIResponse[ExampleData](data=ExampleData(name="test", value=42))
        assert response.status.success is True
        assert response.data.name == "test"
        assert response.data.value == 42
    
    def test_with_custom_status(self):
        """Test with custom error status."""
        status = APIStatus(success=False, message="Error", error_code="TEST_ERROR")
        response = APIResponse[ExampleData](status=status, data=None)
        assert response.status.success is False
        assert response.status.message == "Error"
        assert response.data is None
    
    def test_with_various_data_types(self):
        """Test with different data types."""
        # String data
        str_response = APIResponse[str](data="test string")
        assert str_response.data == "test string"
        
        # Integer data
        int_response = APIResponse[int](data=42)
        assert int_response.data == 42
        
        # List data
        list_response = APIResponse[List[str]](data=["one", "two", "three"])
        assert list_response.data == ["one", "two", "three"]
        
        # Dict data
        dict_response = APIResponse[Dict[str, int]](data={"a": 1, "b": 2})
        assert dict_response.data == {"a": 1, "b": 2}
        
        # Complex model data
        complex_data = ComplexTestData(
            title="Test",
            items=[
                ExampleData(name="item1", value=1),
                ExampleData(name="item2", value=2)
            ],
            metadata={"key": "value"}
        )
        complex_response = APIResponse[ComplexTestData](data=complex_data)
        assert complex_response.data.title == "Test"
        assert len(complex_response.data.items) == 2
        assert complex_response.data.items[0].name == "item1"
    
    def test_serialization(self):
        """Test serialization to and from JSON."""
        response = APIResponse[ExampleData](
            status=APIStatus(success=True),
            data=ExampleData(name="test", value=123)
        )
        
        json_str = response.model_dump_json()
        data = json.loads(json_str)
        
        assert data["status"]["success"] is True
        assert data["data"]["name"] == "test"
        assert data["data"]["value"] == 123
        
        # Test deserialization
        deserialized = APIResponse[ExampleData].model_validate_json(json_str)
        assert deserialized.status.success is True
        assert deserialized.data.name == "test"
        assert deserialized.data.value == 123
    
    def test_schema_documentation(self):
        """Test that schema documentation is correctly defined."""
        schema = APIResponse[ExampleData].model_json_schema()
        
        assert "status" in schema["properties"]
        assert "data" in schema["properties"]
        assert "description" in schema["properties"]["status"]
        assert "description" in schema["properties"]["data"]


class TestPaginationMeta:
    """Tests for the PaginationMeta model.
    
    PaginationMeta provides metadata for paginated responses including
    page number, page size, and total counts.
    """
    
    def test_valid_values(self):
        """Test with valid pagination metadata."""
        meta = PaginationMeta(page=2, page_size=10, total_items=25, total_pages=3)
        assert meta.page == 2
        assert meta.page_size == 10
        assert meta.total_items == 25
        assert meta.total_pages == 3
    
    def test_edge_cases(self):
        """Test edge cases for pagination."""
        # First page with exactly one page of items
        meta = PaginationMeta(page=1, page_size=10, total_items=10, total_pages=1)
        assert meta.page == 1
        assert meta.total_pages == 1
        
        # Large values
        meta = PaginationMeta(page=100, page_size=500, total_items=10000, total_pages=20)
        assert meta.page == 100
        assert meta.page_size == 500
        assert meta.total_items == 10000
        
        # Empty result set
        meta = PaginationMeta(page=1, page_size=10, total_items=0, total_pages=0)
        assert meta.total_items == 0
        assert meta.total_pages == 0
    
    def test_validation_errors(self):
        """Test validation errors for invalid values."""
        # Negative page number (page must be >= 1)
        with pytest.raises(ValidationError):
            PaginationMeta(page=-1, page_size=10, total_items=25, total_pages=3)
        
        # Zero page number (page must be >= 1)
        with pytest.raises(ValidationError):
            PaginationMeta(page=0, page_size=10, total_items=25, total_pages=3)
        
        # Negative page size (page_size must be >= 1)
        with pytest.raises(ValidationError):
            PaginationMeta(page=1, page_size=-10, total_items=25, total_pages=3)
        
        # Zero page size (page_size must be >= 1)
        with pytest.raises(ValidationError):
            PaginationMeta(page=1, page_size=0, total_items=25, total_pages=3)
        
        # Negative total items (total_items must be >= 0)
        with pytest.raises(ValidationError):
            PaginationMeta(page=1, page_size=10, total_items=-5, total_pages=3)
        
        # Negative total pages (total_pages must be >= 0)
        with pytest.raises(ValidationError):
            PaginationMeta(page=1, page_size=10, total_items=25, total_pages=-1)
    
    def test_serialization(self):
        """Test serialization to and from JSON."""
        meta = PaginationMeta(page=2, page_size=15, total_items=45, total_pages=3)
        json_str = meta.model_dump_json()
        data = json.loads(json_str)
        
        assert data["page"] == 2
        assert data["page_size"] == 15
        assert data["total_items"] == 45
        assert data["total_pages"] == 3
        
        # Test deserialization
        deserialized = PaginationMeta.model_validate_json(json_str)
        assert deserialized.page == meta.page
        assert deserialized.page_size == meta.page_size
        assert deserialized.total_items == meta.total_items
        assert deserialized.total_pages == meta.total_pages
    
    def test_schema_documentation(self):
        """Test that schema documentation is correctly defined."""
        schema = PaginationMeta.model_json_schema()
        
        for field in ["page", "page_size", "total_items", "total_pages"]:
            assert field in schema["properties"]
            assert "description" in schema["properties"][field]
            
        # All fields should be required
        assert set(schema.get("required", [])) == {"page", "page_size", "total_items", "total_pages"}


class TestPaginatedResponse:
    """Tests for the PaginatedResponse generic model.
    
    PaginatedResponse extends APIResponse to include pagination metadata
    and a list of items of the generic type.
    """
    
    def test_basic_functionality(self):
        """Test basic functionality with string items."""
        pagination = PaginationMeta(page=1, page_size=2, total_items=3, total_pages=2)
        response = PaginatedResponse[str](
            data=["item1", "item2"],
            pagination=pagination
        )
        
        assert response.status.success is True
        assert response.data == ["item1", "item2"]
        assert response.pagination.page == 1
        assert response.pagination.total_items == 3
    
    def test_with_empty_data(self):
        """Test with an empty data list."""
        pagination = PaginationMeta(page=1, page_size=10, total_items=0, total_pages=0)
        response = PaginatedResponse[str](
            data=[],
            pagination=pagination
        )
        
        assert response.status.success is True
        assert response.data == []
        assert response.pagination.total_items == 0
    
    def test_with_complex_items(self):
        """Test with complex model objects as items."""
        pagination = PaginationMeta(page=1, page_size=2, total_items=2, total_pages=1)
        items = [
            ExampleData(name="first", value=1),
            ExampleData(name="second", value=2)
        ]
        
        response = PaginatedResponse[ExampleData](
            data=items,
            pagination=pagination
        )
        
        assert response.status.success is True
        assert len(response.data) == 2
        assert response.data[0].name == "first"
        assert response.data[1].value == 2
    
    def test_with_error_status(self):
        """Test with an error status."""
        pagination = PaginationMeta(page=1, page_size=10, total_items=0, total_pages=0)
        status = APIStatus(success=False, message="Failed to retrieve items", error_code="DB_ERROR")
        
        response = PaginatedResponse[str](
            status=status,
            data=[],
            pagination=pagination
        )
        
        assert response.status.success is False
        assert response.status.message == "Failed to retrieve items"
        assert response.status.error_code == "DB_ERROR"
    
    def test_serialization(self):
        """Test serialization to and from JSON."""
        pagination = PaginationMeta(page=2, page_size=3, total_items=10, total_pages=4)
        response = PaginatedResponse[str](
            data=["a", "b", "c"],
            pagination=pagination
        )
        
        json_str = response.model_dump_json()
        data = json.loads(json_str)
        
        assert data["status"]["success"] is True
        assert data["data"] == ["a", "b", "c"]
        assert data["pagination"]["page"] == 2
        assert data["pagination"]["total_items"] == 10
        
        # Test deserialization
        deserialized = PaginatedResponse[str].model_validate_json(json_str)
        assert deserialized.status.success is True
        assert deserialized.data == ["a", "b", "c"]
        assert deserialized.pagination.page == 2
    
    def test_schema_documentation(self):
        """Test that schema documentation is correctly defined."""
        schema = PaginatedResponse[ExampleData].model_json_schema()
        
        assert "status" in schema["properties"]
        assert "data" in schema["properties"]
        assert "pagination" in schema["properties"]
        assert "description" in schema["properties"]["status"]
        assert "description" in schema["properties"]["data"]
        
        # For pagination, the schema uses a reference to PaginationMeta
        assert "$ref" in schema["properties"]["pagination"]
        
        # Find the PaginationMeta definition and check its description
        assert "PaginationMeta" in schema["$defs"]
        assert "description" in schema["$defs"]["PaginationMeta"]
        
        # Pagination should be required
        assert "pagination" in schema.get("required", [])


class TestModelIntegration:
    """Tests for integration between the different response models."""
    
    def test_api_response_with_paginated_data(self):
        """Test using an APIResponse to contain paginated data."""
        pagination = PaginationMeta(page=1, page_size=2, total_items=2, total_pages=1)
        paginated = PaginatedResponse[str](
            data=["item1", "item2"],
            pagination=pagination
        )
        
        # Wrap a paginated response inside an API response
        response = APIResponse[PaginatedResponse[str]](data=paginated)
        
        assert response.status.success is True
        assert response.data.status.success is True
        assert response.data.data == ["item1", "item2"]
        assert response.data.pagination.page == 1
    
    def test_conversion_between_response_types(self):
        """Test converting between different response types."""
        # Start with a simple API response
        simple_response = APIResponse[str](data="test data")
        
        # Convert to a paginated response
        pagination = PaginationMeta(page=1, page_size=1, total_items=1, total_pages=1)
        paginated_response = PaginatedResponse[str](
            status=simple_response.status,
            data=[simple_response.data],
            pagination=pagination
        )
        
        assert paginated_response.status.success is True
        assert paginated_response.data[0] == "test data"
        assert paginated_response.pagination.total_items == 1