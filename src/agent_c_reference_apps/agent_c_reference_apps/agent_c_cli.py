import asyncio
import json
import os
import queue
import logging
import argparse
import threading
from typing import Optional, List

from dotenv import load_dotenv


from agent_c.models.input import AudioInput
from agent_c.models.input.image_input import ImageInput
from agent_c_tools import LocalStorageWorkspace
from agent_c_tools.tools.workspaces.local_storage import LocalProjectWorkspace

# Note: we load the env file here so that it's loaded when we start loading the libs that depend on API KEYs.   I'm looking at you Eleven Labs
load_dotenv(override=True)

from agent_c_reference_apps.util.audio_cues import AudioCues
from agent_c_reference_apps.util.chat_commands import CommandHandler

# Without this none of the rest matter
from agent_c import GPTChatAgent, ClaudeChatAgent, ChatSessionManager, ToolChest, ToolCache


from agent_c.util import debugger_is_active
from agent_c_tools.tools.user_preferences import AssistantPersonalityPreference, AddressMeAsPreference, UserPreference #noqa
from agent_c.prompting import CoreInstructionSection, HelpfulInfoStartSection, EndOperatingGuideLinesSection, \
    EnvironmentInfoSection, PromptBuilder, PersonaSection



# Ensure all our toolsets get registered
#from agent_c_tools.tools import *  # noqa
from agent_c_tools.tools.user_bio.prompt import UserBioSection


# These are only for the CLI app.
from agent_c_reference_apps.ui.console_chat_ui import ConsoleChatUI

# These are the fake agent messages that show up when you start / end a chat.
NEW_SESSION_WELCOME: str = "New here?  Ask me about my toolsets."
OLD_SESSION_WELCOME: str = "[bold]Welcome back![/] Remember, I have toolsets available feel free to ask me about them."

# Temporary hack
MODELS_THAT_ACCEPT_VOICE: list[str] = ["gpt-4o-audio-preview"]


class CLIChat:
    """
    A reference app that implements a chat interface with a centric_one_shot.ChatAgent.

    Zep is used as a chat session manager to track chat history. Visit https://www.getzep.com/ and grab their docker
    compose file to set it up. This uses streaming responses from the agent.

    Note:
    - The console outputs Markdown. If you're seeing garbled output, you need a better shell.
    - Tested on *nix terminals and Windows Terminal Preview on Windows.
    """

    def __init__(self, **kwargs):
        self.__init_events()
        self.logger: logging.Logger = self.__setup_logging()
        self.user_id = kwargs['user_id']
        self.session_id: Optional[str] = kwargs.get('session_id', None)
        self.audio_cues = AudioCues()
        self.agent_voice: Optional[str] = kwargs.get('agent_voice', None)
        self.voice_model: str = kwargs.get('voice_model_name', 'gpt-4o-audio-preview')
        self.image_inputs = []
        self.audio_inputs = []


        self.__init_agent_params(**kwargs)
        self.cmd_handler = CommandHandler()

        self.chat_ui = ConsoleChatUI(audio_cues=self.audio_cues, debug_event=self.debug_event, input_active_event=self.input_active_event,
                                     exit_event=self.exit_event, ptt_keybind=os.environ.get("PTT_KEYBIND", "c-pagedown"),
                                     model_accepts_audio_event=self.model_accepts_audio_event)
        self.audio_cues.play_sound('app_start')
        self.__init_workspaces()

        # This sets up a few preferences
        # "Default voice" should be self-explanatory, the VoiceTools consume this.
        # "Address me as" defaults to "First name", but if you tell the agent "call me by my last name" or "call me big poppa"
        #                 it will do so in every session until you ask to change or reset it.
        # "Assistant Personality" allows the user to change the tone of the responses as long as the change doesn't conflict with rules in the
        #                         `Operating Guidelines` portion of the prompt. So "from not on talk like a pirate" works just fine.
        self.user_prefs: List[UserPreference] = [AddressMeAsPreference(), AssistantPersonalityPreference()]
        self.current_chat_log: Optional[list[dict]] = None
        self.can_use_tools = True

    def __init_workspaces(self):
        self.logger.debug("Initializing Workspaces...")
        self.workspaces = [LocalProjectWorkspace()]

        try:
            local_workspaces = json.load(open(".local_workspaces.json", "r"))
            for ws in local_workspaces['local_workspaces']:
                self.workspaces.append(LocalStorageWorkspace(**ws))
        except FileNotFoundError:
            pass

    def __init_events(self):
        self.exit_event: threading.Event = threading.Event()
        self.input_active_event: threading.Event = threading.Event()
        self.debug_event: threading.Event = threading.Event()
        self.model_accepts_audio_event: threading.Event = threading.Event()

        if debugger_is_active():
            self.debug_event.set()

    def __init_agent_params(self, **kwargs):
        self.logger.debug("Initializing Agent parameters...")
        self.backend = kwargs.get('backend', 'openai')
        self.prompt = kwargs['prompt']
        self.non_voice_model = kwargs.get('model_name', os.environ.get('MODEL_NAME', 'gpt-4o'))
        self.model_name = self.voice_model if self.agent_voice else self.non_voice_model
        if self.model_name in MODELS_THAT_ACCEPT_VOICE:
            self.model_accepts_audio_event.set()
        else:
            self.model_accepts_audio_event.clear()

        self.agent_output_format = kwargs.get('output_format', 'raw')

        # We keep the full message array resident in memory while the session is active
        # as Zep does not persist tool calls and tool call results (rightfully so).
        #
        # DOING IT THIS WAY IS NOT A GOOD IDEA IN PRODUCTION
        #
        # This needs to be made smarter before use in prod. By not allowing Zep to manage the message log
        # we could easily blow the context window.  This is a DEMO app being used in short bursts,
        # a tool call if the user asks a followup question.
        self.current_chat_log = None

        # The toolchest holds all the toolsets for the session
        self.tool_chest: Optional[ToolChest] = None

        # The tool cache allows the different toolsets to make use of a shared disk cache.
        self.tool_cache_dir = kwargs.get("tool_cache_dir", ".tool_cache")
        self.tool_cache = ToolCache(cache_dir=self.tool_cache_dir)


    async def run(self):
        """
        Initializes the console and starts the input loop to interact with the user.
        """
        await self.__init_session()

        agent_cls = ClaudeChatAgent if self.backend == 'claude' else GPTChatAgent

        await self.__init_chat_agent(agent_cls, self.prompt)

        self.logger.debug("Initializing complete, starting UI.")

        self.__print_session_banner()
        await self.__show_welcome_message()
        await self.__core_input_loop()

    async def __init_session(self):
        self.logger.debug("Initializing Session...")
        if os.environ.get("ZEP_CE_KEY") or os.environ.get("ZEP_API_KEY"):
            from agent_c.chat.zep_session_manager import ZepCESessionManager, ZepCloudSessionManager, AsyncZep
            client = AsyncZep(api_key=os.environ.get("ZEP_API_KEY"))
            cls = ZepCESessionManager if os.environ.get("ZEP_CE_KEY") else ZepCloudSessionManager
            self.session_manager = cls(client, allow_auto_user_create=True)
        else:
            self.session_manager = ChatSessionManager()

        await self.session_manager.init(self.user_id, self.session_id)
        self.session_id = self.session_manager.chat_session.session_id

    def __print_session_banner(self):
        self.chat_ui.show_session_info(self.session_manager)

    async def __init_chat_agent(self, agent_cls, persona_prompt: str):
        """
        This sets up the chat agent for the current session.
        Here's where we declare the toolsets we want to use with the agent as well as how our system prompt will look.

        Args:
            persona_prompt (str): The prompt to initialize the ChatAgent with.
        """
        self.logger.debug("Initializing Chat Agent...")
        self.tool_chest = ToolChest()
        tool_opts = {'tool_cache': self.tool_cache, 'session_manager': self.session_manager, 'user_preferences': self.user_prefs,
                     'workspaces': self.workspaces, 'streaming_callback': self.chat_callback}

        self.logger.debug("Initializing Chat Agent... Initializing toolsets...")
        await self.tool_chest.init_tools(**tool_opts)

        self.logger.debug("Initializing Chat Agent... Initializing sections...")
        operating_sections = [
            CoreInstructionSection(template="<operating_guidelines>\n"),
            PersonaSection(template=persona_prompt),
            EndOperatingGuideLinesSection()
        ]

        operating_sections.extend(self.tool_chest.active_tool_sections)

        # Anything outside the operating guidelines is basically helpful information for the model to know.
        # These are demo sections that tell the model a little about the user as well as things like the current date / time.
        info_sections = [HelpfulInfoStartSection(),
                         EnvironmentInfoSection(session_manager=self.session_manager, voice_tools=self.tool_chest.active_tools.get('voice'), agent_voice=self.agent_voice),
                         UserBioSection(session_manager=self.session_manager)]

        self.logger.debug("Initializing Chat Agent... Initializing agent...")
        # FINALLY we create the agent
        self.agent: GPTChatAgent = agent_cls(prompt_builder=PromptBuilder(sections=operating_sections + info_sections),
                                             model_name=self.model_name,
                                             tool_chest=self.tool_chest,
                                             streaming_callback=self.chat_callback,
                                             output_format=self.agent_output_format)

    def __setup_logging(self) -> logging.Logger:
        """
        Set up a logger instance with specified log level.

        Returns:
            logging.Logger: Configured logger instance.
        """
        logger = logging.getLogger(__name__)
        other_loggers = ['httpx', 'LiteLLM', 'openai', 'httpcore', 'websockets', 'speechmatics', 'asyncio', 'linkedin_api',
                         'urllib3']

        debug_other_loggers = []

        if self.debug_event.is_set():
            logger.setLevel(logging.DEBUG)
            for log in debug_other_loggers:
                logging.getLogger(log).setLevel(logging.DEBUG)
        else:
            logger.setLevel(logging.WARN)

        for log in other_loggers:
            logging.getLogger(log).setLevel(logging.WARN)

        return logger

    async def chat_callback(self, event):
        """
        Called by the ChatAgent and toolsets to notify us of events as they happen.
        """
        if event.type == 'history':
            self.current_chat_log = event.messages

        # Note we forward most events to the UI layer, if you want to see how to make a console UI feel free to dig in.
        # all this thing is doing is managing the output so that Markdown renders correctly.
        await self.chat_ui.chat_event(event)

    async def __build_prompt_metadata(self):
        """
        Pretty much all of this could be in a prompt sections instead but it's left as an example of how to inject data
        into the PromptBuilder chain
        """
        memory_summary = ""
        memory_context = ""
        if self.session_manager.active_memory is not None:
            memory = self.session_manager.active_memory
            if memory.summary is not None:
                memory_summary = memory.summary.content
            if memory.context is not None:
                memory_context = memory.context

        return {"session_id": self.session_id,
                "current_user_username": self.session_manager.user.user_id,
                "current_user_name": self.session_manager.user.first_name,
                "session_summary": memory_summary,
                "session_context": memory_context,
                "agent_voice": self.agent_voice}

    async def __core_input_loop(self):
        """
        This is the core loop for user input and chat interaction.

        Continuously prompts input from the user and sends it to the ChatAgent, unless '!exit' command is entered
        or an EOF(End-of-File)/KeyboardInterrupt exception occurs.

        The chat responses and events are handled asynchronously by a callback.

        If any other exception occurs, it is logged.
        """
        self.logger.debug("Starting core input loop...")
        while not self.exit_event.is_set():
            try:
                self.input_active_event.set()
                user_input = await self.chat_ui.get_user_input()
                self.input_active_event.clear()
                user_message: Optional[str] = None
                image_inputs: List[ImageInput] = []
                audio_inputs: List[AudioInput] = []

                if user_input.type == 'text':
                    if await self.cmd_handler.handle_command(user_input.content, self):
                        continue
                    user_message = user_input.content
                elif user_input.type == 'audio':
                    audio_inputs.append(user_input)
                elif user_input.type == 'multimodal':
                    for modal_content in user_input.content:
                        if modal_content.type == 'text':
                            user_message = modal_content.content
                        elif modal_content.type == 'image':
                            image_inputs.append(modal_content)
                        elif modal_content.type == 'audio':
                            audio_inputs.append(modal_content)

                if user_message is None and len(image_inputs) > 0:
                    user_message = "<admin_msg>If it's not clear from prior messages, ask the user what to do with this image.</admin_msg>"

                # We wait till the last minute to refresh out data from Zep so we can allow it to summarize and whatnot in the background
                await self.session_manager.update()

                await self.agent.chat(session_manager=self.session_manager, user_message=user_message, prompt_metadata=await self.__build_prompt_metadata(),
                                      messages=self.current_chat_log, output_format=self.agent_output_format, images=image_inputs, audio=audio_inputs,
                                      voice=self.agent_voice, budget_tokens=10000)

                await self.session_manager.flush()
            except (EOFError, KeyboardInterrupt):
                # Handle exit upon EOF (Ctrl-D) or Keyboard Interrupt (Ctrl-C)
                break
            except Exception as e:
                self.logger.exception(f"An error occurred: {e}")

        if self.session_manager.is_new_session:
            await self.session_manager.delete_session()

    def show_output_mode_hint(self):
        if self.agent_output_format == "markdown":
            mode_msg = f"Output mode is set to [bold gold3]markdown[/] mode.  Use [bold gold3]!r[/] to switch to raw mode."
        else:
            mode_msg = f"Output mode is set to [bold gold3]raw[/] mode.  Use [bold gold3]!m[/] to switch to markdown mode."

        self.chat_ui.fake_role_message("system", mode_msg)

    async def __show_welcome_message(self):
        """
        Basic UI stuff to show the welcome message and replay the chat log if resuming a session.
        """
        if self.session_manager.is_new_session:
            message = NEW_SESSION_WELCOME
        else:
            message = OLD_SESSION_WELCOME
            # TODO: Restore Zep
            #messages = await self.session_manager.zep_client.message.aget_session_messages(self.session_manager.chat_session.session_id)
            #for msg in messages:
            #    self.chat_ui.fake_role_message(msg.role, msg.content)

        self.chat_ui.fake_role_message("assistant", message)

        self.show_output_mode_hint()


def main():
    parser = argparse.ArgumentParser(description="CLI Chat Interface")
    parser.add_argument('--session', type=str, help='Existing session ID to use', default=None)
    parser.add_argument('--model', type=str, help='The model name to use', default="gpt-4o")
    parser.add_argument('--prompt_file', type=str, help='Path to a file containing the system prompt to use.', default='personas/default.md')
    parser.add_argument('--userid', type=str, help='The userid for the session', default=os.environ.get("CLI_CHAT_USER_ID", "default"))
    parser.add_argument('--claude', action='store_true', help='Use Claude as the agent instead of GPT-4-1106-preview.  This is experimental.')
    parser.add_argument('--voice', type=str, help='The voice name to use')
    args = parser.parse_args()
    load_dotenv(override=True)

    if args.prompt_file is not None:
        try:
            # Read file content into prompt
            with open(args.prompt_file, 'r', encoding='utf-8') as file:
                prompt = file.read()
        except FileNotFoundError:
            print(f"The file {args.prompt_file} was not found.")
            exit(1)
        except Exception as e:
            print(f"An error occurred while reading the prompt file: {e}")
            exit(1)

    model: str = args.model
    backend: str = 'openai'
    if args.claude and args.model == 'gpt-4o':
        model = 'claude-3-7-sonnet-20250219'

    if args.claude:
        backend = 'claude'

    chat = CLIChat(user_id=args.userid, prompt=prompt, session_id=args.session,
                   model_name=model, backend=backend, agent_voice=args.voice)
    asyncio.run(chat.run())


if __name__ == "__main__":
    main()
