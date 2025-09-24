"""
Integration tests for the Excel tool using debug_tool.
These tests call the actual ExcelTools interface that users interact with.
"""
import pytest
import json
import sys
import os
from pathlib import Path

# Add the tool_debugger directory to the path
current_dir = Path(__file__).parent
tool_dir = current_dir.parent
src_dir = tool_dir.parent.parent.parent  # Get to src level
agent_c_tools_dir = src_dir / "agent_c_tools"

# Find tool_debugger directory
tool_debugger_path = None
possible_paths = [
    src_dir / "agent_c_tools" / "tools" / "tool_debugger",
    src_dir.parent / "agent_c_tools" / "tools" / "tool_debugger", 
    Path(__file__).parent.parent.parent.parent.parent / "agent_c_tools" / "tools" / "tool_debugger"
]

for path in possible_paths:
    if path.exists():
        tool_debugger_path = path
        break

if tool_debugger_path:
    sys.path.insert(0, str(tool_debugger_path))

try:
    from debug_tool import ToolDebugger
    DEBUGGER_AVAILABLE = True
except ImportError as e:
    print(f"Warning: ToolDebugger not available: {e}")
    DEBUGGER_AVAILABLE = False


class TestExcelToolsIntegration:
    """Integration tests for ExcelTools that test the complete user interface."""
    
    def setup_method(self):
        self.debugger = None
    
    def teardown_method(self):
        if self.debugger:
            # Clean up any resources if needed
            pass
    
    async def setup_excel_tool(self):
        """Helper method to set up the Excel tool for testing."""
        if not DEBUGGER_AVAILABLE:
            pytest.skip("ToolDebugger not available")
        
        # ToolDebugger automatically loads .env and .local_workspaces.json 
        self.debugger = ToolDebugger(init_local_workspaces=False)
        await self.debugger.setup_tool(
            tool_import_path='agent_c_tools.tools.excel.ExcelTools',
            tool_opts={}  # Config loaded automatically from .env
        )
        return self.debugger
    
    @pytest.mark.integration
    @pytest.mark.asyncio
    async def test_tool_availability(self):
        """Test that the Excel tool is properly set up and available."""
        debugger = await self.setup_excel_tool()
        
        # Check that the tool is available
        available_tools = debugger.get_available_tool_names()
        expected_methods = [
            'excel_create_workbook',
            'excel_load_workbook', 
            'excel_save_workbook',
            'excel_list_sheets',
            'excel_create_sheet',
            'excel_append_records',
            'excel_reserve_rows',
            'excel_read_sheet_data'
        ]
        
        for method in expected_methods:
            assert method in available_tools, f"Method {method} not found in available tools"
        
        # Print debug info for manual verification
        print("\\n=== Available Excel Tools ===")
        debugger.print_tool_info()
    
    @pytest.mark.integration
    @pytest.mark.asyncio
    async def test_create_workbook_integration(self):
        """Test create_workbook through the tool interface."""
        debugger = await self.setup_excel_tool()
        
        # Call the tool through its actual interface
        results = await debugger.run_tool_test(
            tool_name='excel_create_workbook',
            tool_params={}
        )
        
        # Extract and validate content
        content = debugger.extract_content_from_results(results)
        assert content is not None
        assert not content.startswith("Error:")
        
        # Parse and validate structure
        structured_content = debugger.extract_structured_content(results)
        assert structured_content is not None
        assert structured_content.get("success") == True
        assert "message" in structured_content
    
    @pytest.mark.integration
    @pytest.mark.asyncio
    async def test_list_sheets_after_create(self):
        """Test list_sheets after creating a workbook."""
        debugger = await self.setup_excel_tool()
        
        # First create a workbook
        await debugger.run_tool_test(
            tool_name='excel_create_workbook',
            tool_params={}
        )
        
        # Then list sheets
        results = await debugger.run_tool_test(
            tool_name='excel_list_sheets',
            tool_params={}
        )
        
        # Extract and validate content
        content = debugger.extract_content_from_results(results)
        assert content is not None
        assert not content.startswith("Error:")
        
        # Parse and validate structure
        structured_content = debugger.extract_structured_content(results)
        assert structured_content is not None
        assert structured_content.get("success") == True
        assert "sheets" in structured_content
        assert isinstance(structured_content["sheets"], list)
        assert len(structured_content["sheets"]) > 0  # Should have default sheet
    
    @pytest.mark.integration
    @pytest.mark.asyncio
    async def test_create_sheet_integration(self):
        """Test create_sheet through the tool interface.""" 
        debugger = await self.setup_excel_tool()
        
        # First create a workbook
        await debugger.run_tool_test(
            tool_name='excel_create_workbook',
            tool_params={}
        )
        
        # Create a new sheet
        results = await debugger.run_tool_test(
            tool_name='excel_create_sheet',
            tool_params={'sheet_name': 'TestSheet'}
        )
        
        # Extract and validate content
        content = debugger.extract_content_from_results(results)
        assert content is not None
        assert not content.startswith("Error:")
        
        # Parse and validate structure
        structured_content = debugger.extract_structured_content(results)
        assert structured_content is not None
        assert structured_content.get("success") == True
        assert "TestSheet" in structured_content.get("message", "")
    
    @pytest.mark.integration
    @pytest.mark.asyncio
    async def test_reserve_rows_integration(self):
        """Test reserve_rows through the tool interface."""
        debugger = await self.setup_excel_tool()
        
        # First create a workbook
        await debugger.run_tool_test(
            tool_name='excel_create_workbook',
            tool_params={}
        )
        
        # Reserve rows
        results = await debugger.run_tool_test(
            tool_name='excel_reserve_rows',
            tool_params={
                'row_count': 10,
                'sheet_name': 'Sheet1',
                'agent_id': 'test_agent'
            }
        )
        
        # Extract and validate content
        content = debugger.extract_content_from_results(results)
        assert content is not None
        assert not content.startswith("Error:")
        
        # Parse and validate structure
        structured_content = debugger.extract_structured_content(results)
        assert structured_content is not None
        assert structured_content.get("success") == True
        assert "reservation_id" in structured_content
        assert "start_row" in structured_content
        assert "end_row" in structured_content
    
    @pytest.mark.integration
    @pytest.mark.asyncio
    async def test_append_records_integration(self):
        """Test append_records through the tool interface."""
        debugger = await self.setup_excel_tool()
        
        # First create a workbook
        await debugger.run_tool_test(
            tool_name='excel_create_workbook', 
            tool_params={}
        )
        
        # Append test records
        test_records = [
            ["Name", "Age", "City"],
            ["John Doe", "30", "New York"],
            ["Jane Smith", "25", "Los Angeles"]
        ]
        
        results = await debugger.run_tool_test(
            tool_name='excel_append_records',
            tool_params={
                'records': test_records,
                'sheet_name': 'Sheet1',
                'headers': ["Name", "Age", "City"],
                'agent_id': 'test_agent'
            }
        )
        
        # Extract and validate content
        content = debugger.extract_content_from_results(results)
        assert content is not None
        assert not content.startswith("Error:")
        
        # Parse and validate structure
        structured_content = debugger.extract_structured_content(results)
        assert structured_content is not None
        assert structured_content.get("success") == True
        assert "records_written" in structured_content
        assert structured_content["records_written"] == len(test_records)
    
    @pytest.mark.integration
    @pytest.mark.asyncio 
    async def test_read_sheet_data_integration(self):
        """Test read_sheet_data through the tool interface."""
        debugger = await self.setup_excel_tool()
        
        # First create a workbook and add some data
        await debugger.run_tool_test(
            tool_name='excel_create_workbook',
            tool_params={}
        )
        
        # Add test data
        test_records = [
            ["Name", "Age"],
            ["John", "30"],
            ["Jane", "25"]
        ]
        
        await debugger.run_tool_test(
            tool_name='excel_append_records',
            tool_params={
                'records': test_records,
                'sheet_name': 'Sheet1'
            }
        )
        
        # Now read the data back
        results = await debugger.run_tool_test(
            tool_name='excel_read_sheet_data',
            tool_params={
                'sheet_name': 'Sheet1',
                'include_headers': True,
                'max_rows': 10
            }
        )
        
        # Extract and validate content
        content = debugger.extract_content_from_results(results)
        assert content is not None
        assert not content.startswith("Error:")
        
        # Parse and validate structure
        structured_content = debugger.extract_structured_content(results)
        assert structured_content is not None
        assert structured_content.get("success") == True
        assert "rows_read" in structured_content
        assert structured_content["rows_read"] > 0
    
    @pytest.mark.integration
    @pytest.mark.asyncio
    async def test_tool_error_handling(self):
        """Test error handling through the tool interface."""
        debugger = await self.setup_excel_tool()
        
        # Try to list sheets without creating workbook first
        results = await debugger.run_tool_test(
            tool_name='excel_list_sheets',
            tool_params={}
        )
        
        # Should get an error message
        content = debugger.extract_content_from_results(results)
        assert content is not None
        
        # Parse response - should indicate no workbook loaded
        try:
            structured_content = debugger.extract_structured_content(results)
            if structured_content:
                assert not structured_content.get("success", True)
                assert "workbook" in structured_content.get("error", "").lower()
        except:
            # If parsing fails, check raw content for error indication
            assert "workbook" in content.lower() or "error" in content.lower()
    
    @pytest.mark.integration
    @pytest.mark.asyncio
    async def test_tool_parameter_validation(self):
        """Test parameter handling through tool interface."""
        debugger = await self.setup_excel_tool()
        
        # Test missing required parameters for append_records
        results = await debugger.run_tool_test(
            tool_name='excel_append_records',
            tool_params={}  # Missing required 'records' parameter
        )
        
        content = debugger.extract_content_from_results(results)
        assert content is not None
        
        # Should handle missing parameters gracefully
        try:
            structured_content = debugger.extract_structured_content(results)
            if structured_content:
                # Should return error for missing required parameters
                assert not structured_content.get("success", True)
        except:
            # If parsing fails, content should indicate parameter issue
            assert "required" in content.lower() or "parameter" in content.lower() or "records" in content.lower()


if __name__ == "__main__":
    if not DEBUGGER_AVAILABLE:
        print("Skipping integration tests - ToolDebugger not available")
        sys.exit(0)
    
    pytest.main([__file__, "-v"])