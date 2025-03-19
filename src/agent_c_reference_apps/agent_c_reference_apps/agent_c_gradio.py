import os
import asyncio
import argparse
from dotenv import load_dotenv

# Note: we load the env file here so that it's loaded when we start loading the libs that depend on API KEYs.   I'm looking at you Eleven Labs
load_dotenv(override=True)


# Ensure all our toolsets get registered
from agent_c_tools.tools import *  # noqa
from agent_c_reference_apps.ui.gradio_ui import GradioChat
from my_agent_c.tools import *  # noqa


def main():
    parser = argparse.ArgumentParser(description="CLI Chat Interface")
    parser.add_argument('--session', type=str, help='Existing session ID to use', default=None)
    parser.add_argument('--model', type=str, help='The model name to use', default="gpt-4o")
    parser.add_argument('--voice', type=str, help='The name of a voice to use for the agent, (enables voice output)')
    parser.add_argument('--vmodel', type=str, help='The model name to use for voice output', default="gpt-4o-audio-preview")
    parser.add_argument('--persona', type=str,
                        help='The bare name of a markdown file in the personas folder to use. e.g. --persona toolman',
                        default='default')
    parser.add_argument('--userid', type=str, help='The userid for the session',
                        default=os.environ.get("CLI_CHAT_USER_ID", "default"))
    parser.add_argument('--claude', action='store_true',
                        help='Use Claude as the agent instead of GPT-4-1106-preview.  This is experimental.')
    args = parser.parse_args()
    load_dotenv(override=True)

    model: str = args.model
    backend: str = 'openai'
    if args.claude and args.model == 'gpt-4o':
        model = 'claude-3-7-sonnet-20250219'

    if args.claude:
        backend = 'claude'

    chat = GradioChat(user_id=args.userid, persona=args.persona, session_id=args.session,
                      model_name=model, backend=backend, agent_voice=args.voice, voice_model_name=args.vmodel)
    asyncio.run(chat.run())


if __name__ == "__main__":
    main()