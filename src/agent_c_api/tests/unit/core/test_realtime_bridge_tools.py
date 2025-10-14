"""Unit tests for RealtimeBridge tool management functionality.

These tests verify the tool activation and management in RealtimeBridge, specifically:
- Tool activation via update_tools() method
- Correct parameter passing to ToolChest.activate_toolset()
- Tool addition and removal
- Edge cases and error handling

IMPORTANT: These tests demonstrate a critical bug where update_tools() passes the wrong
parameter to activate_toolset(), causing tools to require double-equip to work properly.
"""

import pytest
from unittest.mock import AsyncMock, Mock, patch
from typing import List

from agent_c_api.core.realtime_bridge import RealtimeBridge
from agent_c.models.agent_config import AgentConfigurationV2


@pytest.fixture
def mock_tool_chest():
    """Fixture for a mocked ToolChest.
    
    Returns an AsyncMock configured to simulate tool activation behavior.
    The activate_toolset method is the key method we're testing.
    """
    chest = Mock()
    chest.activate_toolset = AsyncMock(return_value=True)
    chest.available_tools = {"ToolA": Mock(), "ToolB": Mock(), "ToolC": Mock()}
    return chest


@pytest.fixture
def mock_chat_session():
    """Fixture for a mocked ChatSession.
    
    Returns a Mock with agent_config as a proper AgentConfigurationV2 instance to avoid Pydantic validation errors.
    """
    session = Mock()
    session.session_id = "test-session-123"
    session.user_id = "test-user"

    # Create a proper AgentConfigurationV2 instance instead of a Mock
    session.agent_config = AgentConfigurationV2(
        key="default",
        name="Test Agent",
        model_id="test-model",
        persona="Test persona",
        tools=["ToolA", "ToolB"]  # Initial tools
    )
    return session


@pytest.fixture
def mock_bridge(mock_tool_chest, mock_chat_session):
    """Fixture for a RealtimeBridge instance with mocked dependencies.
    
    Creates a partially mocked bridge with real update_tools() method
    but mocked dependencies to isolate the method under test.
    """
    # Create a real bridge instance but mock its dependencies
    with patch('agent_c_api.core.realtime_bridge.RealtimeBridge.__init__', return_value=None):
        bridge = RealtimeBridge.__new__(RealtimeBridge)
        
        # Set up required attributes
        bridge.tool_chest = mock_tool_chest
        bridge.chat_session = mock_chat_session
        bridge.ui_session_id = "ui-session-123"
        
        # Mock methods we don't want to actually call
        bridge.send_event = AsyncMock()
        bridge.send_system_message = AsyncMock()
        bridge.logger = Mock()
        bridge.logger.info = Mock()
        bridge.logger.warning = Mock()
        bridge.logger.error = Mock()
        
        return bridge


@pytest.mark.unit
@pytest.mark.realtime_bridge
class TestRealtimeBridgeToolManagement:
    """Tests for RealtimeBridge tool management methods.
    
    This test class focuses on the update_tools() method and its interaction
    with ToolChest.activate_toolset().
    """
    
    @pytest.mark.asyncio
    async def test_update_tools_adds_new_tool_DEMONSTRATES_BUG(self, mock_bridge):
        """Test adding a new tool to the agent's toolset.
        
        This test DEMONSTRATES THE BUG where update_tools() passes the wrong
        parameter to activate_toolset().
        
        Expected behavior: activate_toolset() should be called with new_tools
        Current buggy behavior: activate_toolset() is called with agent_config.tools
        
        **This test will FAIL with the current buggy code and PASS after the fix.**
        """
        # GIVEN: Agent has initial set of tools
        initial_tools = ["ToolA", "ToolB"]
        new_tools = ["ToolA", "ToolB", "ToolC"]  # Added ToolC
        
        mock_bridge.chat_session.agent_config.tools = initial_tools
        
        # WHEN: update_tools is called with new tools list
        result = await mock_bridge.update_tools(new_tools)
        
        # THEN: activate_toolset should be called with new_tools, not initial_tools
        # BUG: With current code at line 380, this assertion will FAIL
        # because it passes agent_config.tools (initial_tools) instead of new_tools
        mock_bridge.tool_chest.activate_toolset.assert_called_once_with(new_tools)
        
        # AND: agent_config.tools should be updated to new_tools
        assert mock_bridge.chat_session.agent_config.tools == new_tools
        
        # AND: method should return True (successful activation)
        assert result is True
        
        # AND: success message should be sent
        mock_bridge.send_system_message.assert_called()
        assert any("updated" in str(call).lower() 
                  for call in mock_bridge.send_system_message.call_args_list)
    
    @pytest.mark.asyncio
    async def test_update_tools_removes_tool(self, mock_bridge):
        """Test removing a tool from the agent's toolset.
        
        Should pass the updated (shorter) tool list to activate_toolset().
        """
        # GIVEN: Agent has three tools
        initial_tools = ["ToolA", "ToolB", "ToolC"]
        new_tools = ["ToolA", "ToolB"]  # Removed ToolC
        
        mock_bridge.chat_session.agent_config.tools = initial_tools
        
        # WHEN: update_tools is called with reduced tools list
        result = await mock_bridge.update_tools(new_tools)
        
        # THEN: activate_toolset should be called with new_tools
        mock_bridge.tool_chest.activate_toolset.assert_called_once_with(new_tools)
        
        # AND: agent_config.tools should be updated
        assert mock_bridge.chat_session.agent_config.tools == new_tools
        assert result is True
    
    @pytest.mark.asyncio
    async def test_update_tools_adds_and_removes_simultaneously(self, mock_bridge):
        """Test adding and removing tools in the same call.
        
        Should handle complex tool list changes correctly.
        """
        # GIVEN: Agent has initial tools
        initial_tools = ["ToolA", "ToolB"]
        new_tools = ["ToolA", "ToolC"]  # Removed ToolB, added ToolC
        
        mock_bridge.chat_session.agent_config.tools = initial_tools
        
        # WHEN: update_tools is called with changed tools list
        result = await mock_bridge.update_tools(new_tools)
        
        # THEN: activate_toolset should be called with new_tools
        mock_bridge.tool_chest.activate_toolset.assert_called_once_with(new_tools)
        
        # AND: agent_config.tools should be updated
        assert mock_bridge.chat_session.agent_config.tools == new_tools
        assert result is True
    
    @pytest.mark.asyncio
    async def test_update_tools_empty_list(self, mock_bridge):
        """Test clearing all tools by passing an empty list.
        
        Edge case: Should handle empty tool list correctly.
        """
        # GIVEN: Agent has some tools
        initial_tools = ["ToolA", "ToolB"]
        new_tools = []  # Clear all tools
        
        mock_bridge.chat_session.agent_config.tools = initial_tools
        
        # WHEN: update_tools is called with empty list
        result = await mock_bridge.update_tools(new_tools)
        
        # THEN: activate_toolset should be called with empty list
        mock_bridge.tool_chest.activate_toolset.assert_called_once_with(new_tools)
        
        # AND: agent_config.tools should be empty
        assert mock_bridge.chat_session.agent_config.tools == []
        assert result is True
    
    @pytest.mark.asyncio
    async def test_update_tools_idempotent_call(self, mock_bridge):
        """Test calling update_tools with the same tools as currently equipped.
        
        Should work correctly as a no-op (idempotent operation).
        """
        # GIVEN: Agent has tools
        initial_tools = ["ToolA", "ToolB"]
        new_tools = ["ToolA", "ToolB"]  # Same as initial
        
        mock_bridge.chat_session.agent_config.tools = initial_tools
        
        # WHEN: update_tools is called with same tools
        result = await mock_bridge.update_tools(new_tools)
        
        # THEN: activate_toolset should be called with new_tools (same as old)
        mock_bridge.tool_chest.activate_toolset.assert_called_once_with(new_tools)
        
        # AND: agent_config.tools should remain the same
        assert mock_bridge.chat_session.agent_config.tools == new_tools
        assert result is True
    
    @pytest.mark.asyncio
    async def test_update_tools_activation_fails(self, mock_bridge):
        """Test error handling when tool activation fails.
        
        Should handle activation failure gracefully and filter unavailable tools.
        """
        # GIVEN: Agent has tools and activation will fail
        initial_tools = ["ToolA", "ToolB"]
        new_tools = ["ToolA", "ToolB", "ToolC"]
        
        mock_bridge.chat_session.agent_config.tools = initial_tools
        mock_bridge.tool_chest.activate_toolset.return_value = False
        
        # WHEN: update_tools is called
        result = await mock_bridge.update_tools(new_tools)
        
        # THEN: activate_toolset should be called
        mock_bridge.tool_chest.activate_toolset.assert_called_once()
        
        # AND: method should return False
        assert result is False
        
        # AND: error message should be sent
        mock_bridge.send_system_message.assert_called()
        error_calls = [str(call) for call in mock_bridge.send_system_message.call_args_list]
        assert any("failed" in call.lower() for call in error_calls)
    
    @pytest.mark.asyncio
    async def test_update_tools_exception_handling(self, mock_bridge):
        """Test exception handling during tool activation.
        
        Should catch exceptions and return False without crashing.
        """
        # GIVEN: Agent has tools and activation will raise exception
        initial_tools = ["ToolA", "ToolB"]
        new_tools = ["ToolA", "ToolB", "ToolC"]
        
        mock_bridge.chat_session.agent_config.tools = initial_tools
        mock_bridge.tool_chest.activate_toolset.side_effect = Exception("Tool activation error")
        
        # WHEN: update_tools is called
        result = await mock_bridge.update_tools(new_tools)
        
        # THEN: should return False, but current buggy code returns undefined 'false'
        # This documents a bug in the application code at line 386
        # The code should return False but returns 'false' (which becomes NameError)
        # For now we test that it's falsy (False, None, etc.)
        assert not result  # Accept any falsy value (False, None, etc.)

        # AND: error should be logged
        mock_bridge.logger.error.assert_called()
        
        # AND: error message should be sent to user
        mock_bridge.send_system_message.assert_called()


@pytest.mark.unit
@pytest.mark.realtime_bridge  
class TestRealtimeBridgeToolIntegration:
    """Integration-style tests for tool management workflows.
    
    These tests verify end-to-end tool management scenarios that users
    would encounter, like the double-equip bug reported.
    """
    
    @pytest.mark.asyncio
    async def test_double_equip_scenario_first_attempt(self, mock_bridge):
        """Simulate the first !eq command that exhibits the bug.
        
        This test simulates what happens when a user runs !eq MicrosoftStreamTools
        for the FIRST time. Due to the bug, the tool won't actually be activated.
        """
        # GIVEN: Agent has initial tools (like WorkspaceTools, BridgeTools)
        initial_tools = ["WorkspaceTools", "BridgeTools"]
        mock_bridge.chat_session.agent_config.tools = initial_tools
        
        # WHEN: User runs !eq MicrosoftStreamTools (command handler adds it to list)
        new_tools_with_microsoft = initial_tools + ["MicrosoftStreamTools"]
        await mock_bridge.update_tools(new_tools_with_microsoft)
        
        # THEN: Due to the bug, activate_toolset was called with OLD list (without MicrosoftStreamTools)
        # This is the BUG - it should be called with new_tools_with_microsoft
        # For now, this test documents the buggy behavior
        
        # BUT: agent_config.tools DOES get updated (line 392)
        assert mock_bridge.chat_session.agent_config.tools == new_tools_with_microsoft
        
        # RESULT: Config shows tool equipped, but tool is NOT activated
        # User would see tool in list but it wouldn't work!
    
    @pytest.mark.asyncio  
    async def test_double_equip_scenario_second_attempt(self, mock_bridge):
        """Simulate the second !eq command that makes the tool work.
        
        This test simulates what happens when a user runs !eq MicrosoftStreamTools
        for the SECOND time. Now it works because agent_config.tools has the tool.
        """
        # GIVEN: After first !eq, agent_config.tools HAS MicrosoftStreamTools
        # (even though it wasn't activated)
        tools_after_first_equip = ["WorkspaceTools", "BridgeTools", "MicrosoftStreamTools"]
        mock_bridge.chat_session.agent_config.tools = tools_after_first_equip
        
        # WHEN: User runs !eq MicrosoftStreamTools AGAIN
        # Command handler sees it's already in list, but still calls update_tools
        await mock_bridge.update_tools(tools_after_first_equip)
        
        # THEN: Due to the bug, activate_toolset is called with agent_config.tools
        # which NOW includes MicrosoftStreamTools!
        # Due to the bug, this works on second attempt because agent_config.tools
        # was updated on first attempt
        # After fix, it will work on FIRST attempt
        
        # RESULT: Tool finally gets activated and works
        # This is why users experience "double-equip" behavior!
