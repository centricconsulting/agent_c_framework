import logging
from typing import List, Any, Dict

logger = logging.getLogger(__name__)


### THESE METHODS ARE SIMPLY FOR EASIER DEBUGGING VISUALIZATION ###
# used by both gradio and agent_as_tool child agents
# multiple uses of 'isintance' is to ensure that the responses from both the main gradio agent and the child agents can be processed.
def filtered_responses(response: List[Any]) -> List[Dict[str, Any]]:
    """
    Filter out Message objects from the response, keeping only dictionary items.

    Args:
        response (List[Any]): The original response list.

    Returns:
        List[Dict[str, Any]]: A list containing only dictionary items from the original response.
    """
    # This line will remove any Message objects from response, so we're only latest raw calls.  After the calls, the responses get converted to Message Objects
    # For this, we don't want them included as we're really debugging the latest calls only, not the entire conversation history.
    return [item for item in response if isinstance(item, dict)]


def system_prompt(response: List[Dict[str, Any]]) -> List[Dict[str, List[Dict[str, Any]]]]:
    """
    Extract system prompts from the response.

    Args:
        response (List[Dict[str, Any]]): The filtered response list.

    Returns:
        List[Dict[str, List[Dict[str, Any]]]]: A list containing a dictionary with system prompts.
    """
    # Ensure you use current_responses first to strip out Message objects
    return [{'system_prompt': [record for record in response if record.get('role') == 'system']}]


def question_response(response: List[Dict[str, Any]]) -> List[Dict[str, List[Dict[str, Any]]]]:
    """
    Extract user questions and assistant responses from the response.

    Args:
        response (List[Dict[str, Any]]): The filtered response list.

    Returns:
        List[Dict[str, List[Dict[str, Any]]]]: A list containing a dictionary with user questions and assistant responses.
    """
    # Ensure you use current_responses first to strip out Message objects
    user_question = [record for record in response if record.get('role') == 'user']
    final_response = [record for record in response if record.get('role') == 'assistant' and record.get('content')]
    return [{'user_question': user_question, 'final_response': final_response}]


def align_tool_calls(response: List[Dict[str, Any]]) -> List[Dict[str, Any]]:
    """
    Match tool calls with their responses for easier tracking.

    Args:
        response (List[Dict[str, Any]]): The filtered response list.

    Returns:
        List[Dict[str, Any]]: A list of matched tool calls and responses.
    """
    # Ensure you use current_responses first to strip out Message objects
    tool_calls = [record for record in response if record.get('role') == 'assistant' and record.get('tool_calls')]
    tool_responses = [record for record in response if record.get('role') == 'tool']

    # How to match tool call with responses for easier tracking
    # Step 1: Create a lookup dictionary for tool responses
    tool_response_lookup = {response['tool_call_id']: response for response in tool_responses}

    # Step 2: Create a list of matched tool calls and responses
    matched_tool_calls_responses = []

    # Step 3: Iterate over each assistant's tool calls and match them with their responses
    for assistant_call in tool_calls:
        for call in assistant_call['tool_calls']:
            tool_call_id = call['id']
            tool_response = tool_response_lookup.get(tool_call_id, None)  # Find the matching response
            if tool_response:
                matched_tool_calls_responses.append({
                    'tool_call': call,
                    'tool_response': tool_response
                })
    logger.info(f"Tool Calls and Responses: {matched_tool_calls_responses}")
    return matched_tool_calls_responses


def combine_debug_info(info_list: List[Any]) -> List[Any]:
    """
    Combine multiple pieces of debug information into a single list.

    Args:
        info_list (List[Any]): A list of debug information items.

    Returns:
        List[Any]: A flattened list of debug information.
    """
    # Combine multiple pieces of debug information into a single list.
    debug_info = []
    for info in info_list:
        if isinstance(info, list):
            debug_info.extend(info)
        else:
            debug_info.append(info)
    return debug_info
