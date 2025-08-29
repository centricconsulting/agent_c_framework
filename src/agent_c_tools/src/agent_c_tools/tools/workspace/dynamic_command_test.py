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
    # from agent_c_tools.tools.workspace
    await tester.setup_tool(tool_import_path='agent_c_tools.tools.workspace.DynamicCommandTools',
                            tool_opts={})

    # Get tool info
    # tester.print_tool_info()

    # Get the correct tool names
    # tool_names = tester.get_available_tool_names()
    # print(f"\nAvailable tool names: {tool_names}")

    # Initialize workspace if needed


    # Run a tool call, pass in parameters.  Format of the tool name is <toolset_name>_<function_name>

    # result = await tester.run_tool_test(
    #     tool_name='workspace_run_command',
    #     tool_params={"path": "//deep_code",
    #                  "command": "npm install react",}
    # )
    result = await tester.run_tool_test(
        tool_name='run_npm',
        tool_params={"path": "//deep_code",
                     "args": "run test",}
    )
    # result = await tester.run_tool_test(
    #     tool_name='run_npm',
    #     tool_params={"path": "//deep_code",
    #                  "args": "diff lodash@4.17.20 lodash@4.17.21",}
    # )
    if "is not on a valid toolset" in str(result):
        print("Tool name is not valid, available tool names are:")
        print(tester.get_available_tool_names())
        return

    content = tester.extract_content_from_results(result)
    if content:
        print("Raw content:\n", content)

if __name__ == "__main__":
    asyncio.run(run_example())
