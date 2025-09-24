from agent_c.toolsets import Toolset, json_schema


class BridgeTools(Toolset):
    """
    Provides your agent with capabilities to interact with the RealTimeBridge for managing 
    session properties and user interface interactions. This toolset allows agents to 
    control aspects of their session presentation and communicate directly with the bridge.
    """

    def __init__(self, **kwargs):
        """
        Initialize the BridgeTools.

        Args:
            **kwargs: Keyword arguments passed to parent Toolset class.
        """
        super().__init__(**kwargs, name='bridge')

    @json_schema(
        description='Set the display name for the current chat session',
        params={
            'session_name': {
                'type': 'string',
                'description': 'The new name to set for the current chat session',
                'required': True
            }
        }
    )
    async def set_session_name(self, **kwargs) -> str:
        """
        Set the name of the current chat session via the RealTimeBridge.

        Args:
            session_name (str): The new name for the chat session

        Returns:
            str: Success message or error description
        """
        try:
            session_name = kwargs.get('session_name')
            tool_context = kwargs.get('tool_context')
            
            if not session_name:
                return "ERROR: session_name parameter is required"
                
            if not tool_context or 'bridge' not in tool_context:
                return "ERROR: Bridge not available in tool context"

            bridge = tool_context['bridge']
            
            # Call the bridge method to rename the session
            await bridge.rename_current_session(session_name)
            
            self.logger.info(f"Successfully set session name to: {session_name}")
            return f"Session name has been changed to: {session_name}"
            
        except Exception as e:
            error_msg = f"Failed to set session name: {str(e)}"
            self.logger.error(error_msg)
            return f"ERROR: {error_msg}"


# Register the toolset
Toolset.register(BridgeTools)