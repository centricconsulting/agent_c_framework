import asyncio
import logging
from agent_c_tools.tools.tool_debugger.debug_tool import ToolDebugger


async def run_example():
    """Simple example of how to use the ToolDebugger"""
    # Create the tool tester
    tester = ToolDebugger(log_level=logging.INFO)

    # Setup a weather tool as a simple example
    # tool_opts is how you pass in required keys to, without loading .env
    # tool_opts={'FLASHDOCS_API_KEY': api_key}
    await tester.setup_tool(tool_import_path='agent_c_tools.DynamicsCrmTools',
                            tool_opts={})

    # Get tool info
    # tester.print_tool_info()

    # Get the correct tool names
    # tool_names = tester.get_available_tool_names()
    # print(f"\nAvailable tool names: {tool_names}")

    # Initialize workspace if needed


    # Run a tool call, pass in parameters.  Format of the tool name is <toolset_name>_<function_name>
    # if tool requires prefix, don't forget to include before function name.
    result = await tester.run_tool_test(
        tool_name='dynamics_crm_get_entities',
        tool_params={
            "entity_type": "opportunities",
            "query_params": "$filter=cen_airelated eq 279120000 and statecode eq 0",
            "limit": 2,
            "force_save": False
        },
        )

    content = tester.extract_content_from_results(result)
    if content:
        print("Raw content:", content)

    # Get structured content (parsed JSON)
    structured_content = tester.extract_structured_content(result)

if __name__ == "__main__":
    asyncio.run(run_example())
