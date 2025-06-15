"""Tests for the ToolChest class."""

import asyncio
import pytest
from unittest import mock

from agent_c.toolsets.tool_chest import ToolChest
from agent_c.toolsets.tool_set import Toolset


class MockToolset(Toolset):
    """A mock toolset for testing."""
    
    def __init__(self, name, **kwargs):
        """Initialize the mock toolset."""
        kwargs["name"] = name
        super().__init__(**kwargs)
        self.post_init_called = False
    
    async def post_init(self):
        """Record that post_init was called."""
        self.post_init_called = True


class EssentialMockToolset(MockToolset):
    """An essential mock toolset for testing."""
    pass


class NonEssentialMockToolset(MockToolset):
    """A non-essential mock toolset for testing."""
    pass


@pytest.fixture
def mock_toolset_classes():
    """Create mock toolset classes for testing."""
    # Clear the tool registry before and after tests
    original_registry = Toolset.tool_registry.copy()
    Toolset.tool_registry = []
    
    # Register our mock classes
    Toolset.register(EssentialMockToolset)
    Toolset.register(NonEssentialMockToolset)
    
    yield
    
    # Restore the original registry
    Toolset.tool_registry = original_registry


class TestToolChest:
    """Tests for the ToolChest class."""
    

    @pytest.mark.asyncio
    async def test_backward_compatibility(self, mock_toolset_classes):
        """Test backward compatibility (no essential toolsets)."""
        chest = ToolChest()
        
        # Initialize tools
        await chest.init_tools()
        
        # All toolsets should be active
        assert len(chest.active_tools) == 2
        assert "EssentialMockToolset" in chest.active_tools
        assert "NonEssentialMockToolset" in chest.active_tools
    
    @pytest.mark.asyncio
    async def test_activate_toolset(self, mock_toolset_classes):
        """Test activating a toolset."""
        chest = ToolChest()
        await chest.init_tools()
        
        # Activate a non-essential toolset
        success = await chest.initialize_toolsets("NonEssentialMockToolset")
        assert success
        
        # Both toolsets should now be active
        assert len(chest.active_tools) == 2
        assert "EssentialMockToolset" in chest.active_tools
        assert "NonEssentialMockToolset" in chest.active_tools
        assert chest.active_tools["NonEssentialMockToolset"].post_init_called
    
    @pytest.mark.asyncio
    async def test_deactivate_toolset(self, mock_toolset_classes):
        """Test deactivating a toolset."""
        chest = ToolChest()
        await chest.init_tools()
        
        # Deactivate a non-essential toolset
        success = chest.deactivate_toolset("NonEssentialMockToolset")
        assert success
        
        # Only one toolset should now be active
        assert len(chest.active_tools) == 1
        assert "EssentialMockToolset" in chest.active_tools
        assert "NonEssentialMockToolset" not in chest.active_tools
    
    @pytest.mark.asyncio
    async def test_cannot_deactivate_essential_toolset(self, mock_toolset_classes):
        """Test that essential toolsets cannot be deactivated."""
        chest = ToolChest(essential_toolset_names=["EssentialMockToolset"])
        await chest.init_tools()
        
        # Try to deactivate an essential toolset
        success = chest.deactivate_toolset("EssentialMockToolset")
        assert not success
        
        # Essential toolset should still be active
        assert len(chest.active_tools) == 1
        assert "EssentialMockToolset" in chest.active_tools
    
    @pytest.mark.asyncio
    async def test_set_active_toolsets(self, mock_toolset_classes):
        """Test setting the complete list of active toolsets."""
        chest = ToolChest()
        await chest.init_tools({})
        
        # Activate both toolsets
        await chest.initialize_toolsets("NonEssentialMockToolset")
        assert len(chest.available_tools) == 2

    @pytest.mark.asyncio
    async def test_activate_nonexistent_toolset(self, mock_toolset_classes):
        """Test activating a toolset that doesn't exist."""
        chest = ToolChest()
        
        # Try to activate a nonexistent toolset
        success = await chest.initialize_toolsets("NonexistentToolset")
        assert not success
        
        # No toolsets should be active
        assert len(chest.active_tools) == 0
    
    @pytest.mark.asyncio
    async def test_multiple_activations(self, mock_toolset_classes):
        """Test activating multiple toolsets at once."""
        chest = ToolChest()
        
        # Activate both toolsets at once
        success = await chest.initialize_toolsets(["EssentialMockToolset", "NonEssentialMockToolset"])
        assert success
        
        # Both toolsets should be active
        assert len(chest.active_tools) == 2
        assert "EssentialMockToolset" in chest.active_tools
        assert "NonEssentialMockToolset" in chest.active_tools
    
    @pytest.mark.asyncio
    async def test_multiple_deactivations(self, mock_toolset_classes):
        """Test deactivating multiple toolsets at once."""
        chest = ToolChest()
        await chest.init_tools()
        
        # Deactivate both toolsets at once
        success = chest.deactivate_toolset(["EssentialMockToolset", "NonEssentialMockToolset"])
        assert not success  # Should fail because EssentialMockToolset cannot be deactivated
        
        # Only NonEssentialMockToolset should be deactivated
        assert len(chest.active_tools) == 1
        assert "EssentialMockToolset" in chest.active_tools
        assert "NonEssentialMockToolset" not in chest.active_tools
    
    @pytest.mark.asyncio
    async def test_activate_tool_compatibility(self, mock_toolset_classes):
        """Test backward compatibility of activate_tool method."""
        chest = ToolChest()
        
        # Use the old method name
        success = await chest.activate_tool("NonEssentialMockToolset")
        assert success
        
        # Toolset should be active
        assert "NonEssentialMockToolset" in chest.active_tools