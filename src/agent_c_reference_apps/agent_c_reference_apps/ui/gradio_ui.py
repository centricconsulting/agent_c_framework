import os
import time
import json
import base64
import asyncio
import logging
import threading
import mimetypes
import webbrowser
from datetime import datetime

import gradio as gr

from typing import Union, List, Optional
from dotenv import load_dotenv
from spacy.tokens.doc import defaultdict

from agent_c_core.agent_c.models.input.audio_input import AudioInput
from agent_c_core.agent_c.toolsets import Toolset

from agent_c_core.agent_c.models.input.image_input import ImageInput
from agent_c_core.agent_c.prompting import DynamicPersonaSection

from agent_c_core.agent_c.util.response_format import align_tool_calls, question_response, system_prompt, \
    combine_debug_info, filtered_responses

from agent_c_reference_apps.ui.markdown_render import MarkdownTokenRenderer
from agent_c_tools.tools.workspaces.local_storage import LocalProjectWorkspace

# Note: we load the env file here so that it's loaded when we start loading the libs that depend on API KEYs.   I'm looking at you Eleven Labs
load_dotenv(override=True)


from agent_c_reference_apps.util.audio_cues import AudioCues
from agent_c_reference_apps.util.chat_commands import CommandHandler

# Without this none of the rest matter
from agent_c_core.agent_c import GPTChatAgent, ClaudeChatAgent, ChatSessionManager, ToolChest, ToolCache

from agent_c_core.agent_c.util import debugger_is_active
from agent_c_tools.tools.user_preferences import AssistantPersonalityPreference, AddressMeAsPreference, UserPreference
from agent_c_core.agent_c.prompting import CoreInstructionSection, HelpfulInfoStartSection, EndOperatingGuideLinesSection, \
    EnvironmentInfoSection, PromptBuilder
from agent_c_core.agent_c.models.events import MessageEvent, ToolCallEvent, InteractionEvent, TextDeltaEvent, HistoryEvent, CompletionEvent, ToolCallDeltaEvent, SessionEvent, RenderMediaEvent

from agent_c_tools import LocalStorageWorkspace
from agent_c_tools.tools.user_bio.prompt import UserBioSection
from agent_c_core.agent_c.util.oai_audio import OAIAudioPlayerAsync

ENHANCED_DEBUG_INFO = os.getenv('ENHANCED_DEBUG_INFO', 'False').lower() in ('true', '1', 'yes')

# Case Sensitive
ESSENTIAL_TOOLS = ['MemoryTools', 'WorkspaceTools', 'PreferenceTools', 'VoiceTools']


def get_available_tools():
    available_tools = []
    for tool_class in Toolset.tool_registry:
        tool_info = {
            'name': tool_class.__name__,
            'module': tool_class.__module__,
            'doc': tool_class.__doc__,
            'essential': tool_class.__name__ in ESSENTIAL_TOOLS
        }
        available_tools.append(tool_info)
    return sorted(available_tools, key=lambda x: x['name'].lower())


class GradioChat:
    """
    A reference app that implements a chat interface with a centric_one_shot.ChatAgent.

    Zep is used as a chat session manager to track chat history. Visit https://www.getzep.com/ and grab their docker
    compose file to set it up. This uses streaming responses from the agent.

    Note:
    - The console outputs Markdown. If you're seeing garbled output, you need a better shell.
    - Tested on *nix terminals and Windows Terminal Preview on Windows.
    """

    def __init__(self, **kwargs):

        self._agent_lock = asyncio.Lock()
        self.audio_player = OAIAudioPlayerAsync()
        self.agent_voice: Optional[str] = kwargs.get('agent_voice', None)
        self.voice_model: str = kwargs.get('voice_model_name', 'gpt-4o-audio-preview')
        self.last_audio_id = None

        self.available_tools = get_available_tools()
        self.selected_tools = [tool['name'] for tool in self.available_tools if
                               tool['essential'] or tool['name'] in ESSENTIAL_TOOLS]
        self.temperature: float = kwargs.get('temperature', 0.5)
        self.known_personas = []
        self.__locate_personas()
        self.custom_persona_text = ''
        self.persona_name = kwargs['persona']
        self.__init_events()
        self.logger: logging.Logger = self.__setup_logging()
        self.user_id = kwargs['user_id']
        self.session_id: Union[str, None] = kwargs.get('session_id', None)
        self.audio_cues = AudioCues()
        self.queue: asyncio.Queue = asyncio.Queue()
        self.token_renderer = MarkdownTokenRenderer(None)
        self.tool_note: str = ''
        self.tool_note: str = ''
        self.debug_response = []  # for enhanced debugging
        self.debug_info = None  # for enhanced debugging
        self.partial_output: str = ''
        self.tool_names: str = ''
        self.user_message = ''
        self.image_inputs = []
        self.audio_inputs = []
        self.last_role = "user"
        self.tts_roles = ['assistant']
        self.__init_agent_params(**kwargs)
        self.__init_ui()
        self.cmd_handler = CommandHandler()
        self.__init_workspaces()

        # This sets up a few preferences
        # "Default voice" should be self-explanatory, the VoiceTools consume this.
        # "Address me as" defaults to "First name", but if you tell the agent "call me by my last name" or "call me big poppa"
        #                 it will do so in every session until you ask to change or reset it.
        # "Assistant Personality" allows the user to change the tone of the responses as long as the change doesn't conflict with rules in the
        #                         `Operating Guidelines` portion of the prompt. So "from not on talk like a pirate" works just fine.
        self.user_prefs: List[UserPreference] = [AddressMeAsPreference(), AssistantPersonalityPreference()]

        self.can_use_tools = True

    def __locate_personas(self):
        try:
            files = os.listdir('./personas')
            markdown_files = ['Custom'] + [os.path.splitext(file)[0] for file in files if file.endswith('.md')]
            self.known_personas = markdown_files
        except Exception as e:
            logging.error(f"Failed to list Markdown files in directory ./personas: {e}")
            raise

    def load_persona(self, persona_name):
        if persona_name == 'Custom':
            return self.custom_persona_text

        self.persona_name = persona_name
        persona_path = os.path.join('personas', f"{persona_name}.md")
        with open(persona_path, 'r') as file:
            return file.read()


    def __init_workspaces(self):
        self.logger.debug("Initializing Workspaces...")
        self.workspaces = [LocalProjectWorkspace()]
        f = None
        try:
            f = open(".local_workspaces.json", "r")
            local_workspaces = json.load(f)
            for ws in local_workspaces['local_workspaces']:
                self.workspaces.append(LocalStorageWorkspace(**ws))
        except FileNotFoundError:
            pass
        finally:
            if f is not None:
                f.close()

    def __init_events(self):
        self.exit_event = threading.Event()
        self.input_active_event = threading.Event()
        self.cancel_tts_event = threading.Event()
        self.debug_event = threading.Event()

        if debugger_is_active():
            self.debug_event.set()

    def __init_agent_params(self, **kwargs):
        self.logger.debug("Initializing Agent parameters...")
        self.backend = kwargs.get('backend', 'openai')
        self.persona_prompt = self.load_persona(self.persona_name)
        self.non_voice_model = kwargs.get('model_name', os.environ.get('MODEL_NAME', 'gpt-4o'))
        self.model_name = self.voice_model if self.agent_voice else self.non_voice_model
        self.agent_output_format = kwargs.get('output_format', 'raw')

        # We keep the full message array resident in memory while the session is active
        # as Zep does not persist tool calls and tool call results (rightfully so).
        #
        # DOING IT THIS WAY IS NOT A GOOD IDEA IN PRODUCTION
        #
        # This needs to be made smarter before use in prod. By not allowing Zep to manage the message log
        # we could easily blow the context window.  This is a DEMO app being used in short bursts,
        # a tool call if the user asks a followup question.
        self.current_chat_Log: Union[list[dict], None] = None

        # The toolchest holds all the toolsets for the session
        self.tool_chest: Union[ToolChest, None] = None

        # The tool cache allows the different toolsets to make use of a shared disk cache.
        self.tool_cache_dir = kwargs.get("tool_cache_dir", ".tool_cache")
        self.tool_cache = ToolCache(cache_dir=self.tool_cache_dir)

        # Experimental support for Open Interpreter
        self.allow_oi = kwargs.get("allow_oi", False)
        # Temporarily disabled due to new user issues
        # self.oi_tool: Union[OpenInterpreterTools, None] = None


    @staticmethod
    def load_svg(svg_path):
        with open(svg_path, 'r') as file:
            return file.read()

    def banner(self):
        svg_content = self.load_svg('images/logo-color.svg')
        return gr.HTML(f"""<div style="display: flex; align-items: center;">
                                   <div style="width: 196px; height: 27px;">{svg_content}</div>
                                   <h1 style="margin-left: 10px;">Agent C</h1>
                               </div>""")

    def add_user_input(self, history, message):
        if history is None:
            history = []

        for filename in message["files"]:
            mime_type, _ = mimetypes.guess_type(filename)
            if 'image' in mime_type:
                self.image_inputs.append(ImageInput.from_file(filename))
            elif 'audio' in mime_type:
                try:
                    self.audio_inputs.append(AudioInput.from_file(filename))
                except Exception as e:
                    self.logger.error(f"Failed to load audio file {filename}: {e}")


            history.append({"role": "user", "content": {"path": filename}})

        if message["text"] is not None:
            self.user_message = message["text"]
            history.append({"role": "user", "content": message["text"]})

        self.last_role = "user"

        return history, gr.MultimodalTextbox(value=None, interactive=False)

    def load_new_persona(self, persona_name):
        self.persona_prompt = self.load_persona(persona_name)
        return self.persona_prompt

    def update_persona_prompt(self, persona_text):
        if self.persona_prompt == 'Custom':
            self.custom_persona_text = persona_text

        self.persona_prompt = persona_text
        return persona_text

    def set_temperature(self, temperature):
        self.temperature = temperature
        return temperature

    def __init_ui(self):
        def get_initial_tool_status():
            """Get the current status of tools for initial display"""
            essential_tools = [tool['name'] for tool in self.available_tools if
                               tool['essential'] or tool['name'] in ESSENTIAL_TOOLS]
            return f"🔧 Active Tools: {', '.join(essential_tools)}"

        def get_status_messages(agent_msg="Ready", tools=None):
            """Format status messages with emojis and improved style"""
            if tools is None:
                tools = ', '.join(self.tool_chest.active_tools.keys() if self.tool_chest else
                                  [tool['name'] for tool in self.available_tools if
                                   tool['essential'] or tool['name'] in ESSENTIAL_TOOLS])
            return (
                f"🤖 Agent Status: {agent_msg}",
                f"🔧 Active Tools: {tools}"
            )

        with gr.Blocks(title="Agent C") as ui:
            banner = self.banner()

            with gr.Row(elem_id="status_bar"):
                with gr.Column(scale=2):
                    agent_status = gr.Markdown(
                        value="🤖 Agent Status: Ready",
                        elem_id="agent_status",
                        elem_classes="status-display"
                    )
                with gr.Column(scale=3):
                    tool_status = gr.Markdown(
                        value=get_initial_tool_status(),  # Set initial tool status
                        elem_id="tool_status",
                        elem_classes="status-display"
                    )

            with gr.Accordion("Options", open=False):
                with gr.Row():
                    dropdown = gr.Dropdown(self.known_personas, label="Persona", value=self.persona_name,
                                           interactive=True)
                    temp_slider = gr.Slider(minimum=0.0, maximum=1.0, step=0.1, label="Temperature",
                                            value=self.temperature)
                    temp_slider.change(self.set_temperature, inputs=temp_slider, outputs=temp_slider)

                # Text box for persona prompt
                text_box = gr.Textbox(
                    label="Persona Prompt",
                    value=self.persona_prompt,
                    lines=4,
                    max_lines=10,
                    interactive=True
                )
                # Connect persona dropdown and text box events properly
                dropdown.change(
                    fn=self.load_new_persona,
                    inputs=[dropdown],
                    outputs=[text_box]
                )
                text_box.change(
                    fn=self.update_persona_prompt,
                    inputs=[text_box],
                    outputs=[text_box]
                )

                optional_tools = [tool['name'] for tool in self.available_tools if
                                  not tool['essential'] and tool['name'] not in ESSENTIAL_TOOLS]
                if optional_tools:  # Only show if there are optional toolsets
                    tool_selector = gr.CheckboxGroup(
                        choices=optional_tools,
                        value=[tool for tool in self.selected_tools if tool in optional_tools],
                        label="Select Additional Tools to Initialize",
                        interactive=True
                    )
                    tool_selection_state = gr.State([])

                    tool_selector.change(
                        fn=self.update_selected_tools,
                        inputs=tool_selector,
                        outputs=[tool_selection_state, agent_status, tool_status],
                        queue=False
                    ).then(
                        fn=self.reinit_agent,
                        outputs=[agent_status],
                        queue=True
                    ).then(
                        fn=get_status_messages,  # Use the helper function
                        outputs=[agent_status, tool_status],
                        queue=True
                    )

                    reinit_button = gr.Button("Reinitialize Agent", variant="primary")
                    reinit_button.click(
                        fn=self.reinit_agent,
                        outputs=[agent_status]
                    ).then(
                        fn=get_status_messages,  # Use the helper function
                        outputs=[agent_status, tool_status]
                    )
                    gr.HTML(
                        """
                        <style>
                            /* Existing styles */
                            #component-0 { height: 98vh; display: flex; flex-direction: column; }
                            .gr-blocks-container { height: 100vh; display: flex; flex-direction: column; }
                            #chatbot { height: calc(98vh - 200px); overflow-y: auto; }
                            .accordion { overflow-y: auto; max-height: calc(100vh - 100px); }
                            #chatbot > div:last-child { height: 100%; }

                            /* New styles for status bar */
                            #status_bar {
                                background-color: #f8f9fa;
                                padding: 10px 15px;
                                border-radius: 8px;
                                margin: 10px 0;
                                box-shadow: 0 2px 4px rgba(0,0,0,0.1);
                            }

                            .status-display {
                                font-size: 1rem;
                                font-weight: 500;
                                margin: 0;
                                padding: 5px 10px;
                                border-radius: 4px;
                                background-color: white;
                                border: 1px solid #e0e0e0;
                            }

                            #agent_status {
                                color: #2196F3;
                            }

                            #tool_status {
                                color: #4CAF50;
                            }

                            /* Make the reinit button more prominent */
                            .primary-button {
                                background-color: #2196F3 !important;
                                color: white !important;
                                font-weight: 500 !important;
                            }
                        </style>
                        """
                    )
                    gr.HTML(
                        """
                        <style>
                            /* Add to your existing styles */
                            .status-display {
                                transition: all 0.3s ease-in-out;
                            }

                            .status-display.updating {
                                background-color: #fff3e0;
                                border-color: #ff9800;
                            }

                            @keyframes statusUpdate {
                                0% { transform: scale(1); }
                                50% { transform: scale(1.02); }
                                100% { transform: scale(1); }
                            }

                            .status-update {
                                animation: statusUpdate 0.3s ease-in-out;
                            }
                        </style>

                        <script>
                            // Add animation when status updates
                            const observeStatus = () => {
                                const statusElements = document.querySelectorAll('.status-display');
                                statusElements.forEach(el => {
                                    el.addEventListener('DOMSubtreeModified', () => {
                                        el.classList.add('status-update');
                                        setTimeout(() => el.classList.remove('status-update'), 300);
                                    });
                                });
                            };

                            // Run after page loads
                            window.addEventListener('load', observeStatus);
                        </script>
                        """
                    )
                    gr.Markdown("""
                        ### Tool Dependencies:
                        - DataVisualizationTools requires DataFrameTools
                        - DatabaseQueryTools requires DataFrameTools
                        - SECTools requires FAISSVectorStore
                        - StockAnalysisTools requires SECTools, WebTools, NewsTools, and DataFrameTools
                    """)



            chatbot = gr.Chatbot([], label="Agent C", show_copy_button=True, elem_id="chatbot",
                                 bubble_full_width=False, show_label=False,
                                 scale=1, sanitize_html=False, height=600, type='messages')
            chat_input = gr.MultimodalTextbox(interactive=True, file_types=["image", "audio"], sources=["upload", "microphone"],
                                              placeholder="Enter message or upload file...", show_label=False)

            with gr.Accordion("Debug Information", open=False):
                self.debug_info_display = gr.JSON()

            chat_msg = chat_input.submit(
                self.add_user_input,
                [chatbot, chat_input],
                [chatbot, chat_input],
                queue=True  # Queue user input processing
            )
            bot_msg = chat_msg.then(
                self.handle_gradio_input,
                inputs=[chatbot],
                outputs=[chatbot, self.debug_info_display],
                api_name="bot_response",
                queue=True  # Queue bot response processing
            )
            bot_msg.then(
                lambda: gr.MultimodalTextbox(interactive=True),
                None,
                [chat_input],
                queue=True  # Queue input reset
            )
        gr.HTML(
            """
            <style>
                #component-0 { height: 98vh; display: flex; flex-direction: column; }
                .gr-blocks-container { height: 100vh; display: flex; flex-direction: column; }
                #chatbot { height: calc(98vh - 200px); overflow-y: auto; }
                .accordion { overflow-y: auto; max-height: calc(100vh - 100px); }
                #chatbot > div:last-child { height: 100%; }
            </style>
            """
        )

        ui.queue()
        self.ui = ui

    async def new_session(self):
        self.session_id = await self.session_manager.new_session()

    def update_selected_tools(self, selected_optional_tools):
        """Update selected tools with loading state."""
        try:
            essential_tools = [tool['name'] for tool in self.available_tools if
                               tool['essential'] or tool['name'] in ESSENTIAL_TOOLS]
            self.selected_tools = essential_tools + selected_optional_tools
            self.logger.info(f"Updated selected tools to: {self.selected_tools}")

            # Return tools list and set status to updating
            return (selected_optional_tools,
                    "🤖 Agent Status: Updating...",
                    "🔧 Active Tools: Updating...")
        except Exception as e:
            self.logger.error(f"Error updating tools: {str(e)}")
            return [], f"🤖 Agent Status: Error: {str(e)}", "🔧 Active Tools: Error occurred"

    async def handle_gradio_input(self, history):
        self.queue = asyncio.Queue()

        async def producer():
            try:
                await self.session_manager.update()
                handled = await self.cmd_handler.handle_command(self.user_message, self)
                if not handled:
                    self.debug_response = await self.agent.chat(
                        session_manager=self.session_manager,
                        user_message=self.user_message,
                        prompt_metadata=await self.__build_prompt_metadata(),
                        messages=self.current_chat_Log,
                        output_format='markdown',
                        voice=self.agent_voice,
                        images=self.image_inputs if self.image_inputs else None,
                        audio=self.audio_inputs if self.audio_inputs else None,
                        temperature=self.temperature,
                        budget_tokens=10000
                    )

                    if isinstance(self.debug_response, list):
                        for chunk in self.debug_response:
                            if isinstance(chunk, str):
                                await self.queue.put(chunk)
                            elif isinstance(chunk, dict):
                                # Handle dictionary responses if needed
                                pass
                    elif isinstance(self.debug_response, str):
                        await self.queue.put(self.debug_response)

                await self.session_manager.flush()
                self.user_message = ''
                self.image_inputs = []
            finally:
                await self.queue.put(None)  # Signal completion

        producer_task = asyncio.create_task(producer())
        history.append({"role": "assistant", "content": ""})
        self.partial_output = ""

        try:
            while True:
                content = await self.queue.get()
                if content is None:
                    break
                if isinstance(content, str):
                    self.partial_output += content
                    history[-1]['content'] = self.partial_output + self.tool_note
                    # Yield history and None for debug_info_display
                    yield history, None
                self.queue.task_done()
        finally:
            await producer_task
            await self.display_debug_info()
            # After response is complete, yield history and updated debug_info
            yield history, self.debug_info  # Yield the final outputs

    async def display_debug_info(self):
        if not ENHANCED_DEBUG_INFO:
            self.debug_info = {
                "info": "Enhanced Debug Info is currently turned off. Please set the environment variable ENHANCED_DEBUG_INFO=True to see additional debug information."
            }
            return

        if not self.debug_response or not isinstance(self.debug_response, list):
            self.debug_info = None
            return
        try:
            # these lines of code are for separating and displaying calls and responses for debugging purposes
            dicts_only = filtered_responses(self.debug_response)
            q_and_a = question_response(dicts_only)
            tool_calls = align_tool_calls(dicts_only)
            sys_prompt = system_prompt(dicts_only)
            additional_payload = {'user_id': self.user_id, 'session_id': self.session_id,
                                  'selected_tools': self.selected_tools,
                                  'tool_cache_dir': self.tool_cache_dir, 'output_tool_arguments': ENHANCED_DEBUG_INFO,
                                  'persona_name': self.persona_name, 'backend': self.backend,
                                  'model_name': self.model_name}
            self.debug_info = combine_debug_info([q_and_a, tool_calls, additional_payload, sys_prompt])
            self.debug_response = []
        except Exception as e:
            self.logger.error(f"Failed to display debug information: {e}")
            self.debug_info = [{'error': f"Failed to display debug information: {e}"}]
        finally:
            return

    def open_browser_after_delay(self, url, delay):
        def delayed_open():
            time.sleep(delay)  # Delay in seconds
            webbrowser.open(url)
            logging.info(f"Browser opened at {url} after {delay} seconds.")

        # Start the thread
        thread = threading.Thread(target=delayed_open)
        thread.daemon = True  # Daemonize thread to close with the main program
        thread.start()

    async def reinit_agent(self):
        # if self.backend == 'claude':
        #     await self.__init_claude_chat_agent()
        # else:
        #     await self.__init_gpt_chat_agent()
        #
        # self.logger.info(f"Agent reinitialized. Active tools: {list(self.tool_chest.active_tools.keys()) if self.tool_chest else 'None'}")
        # return None
        try:
            # Try to acquire the lock, but don't wait if already locked
            if not self._agent_lock.locked():
                async with self._agent_lock:
                    if self.backend == 'claude':
                        await self.__init_claude_chat_agent()
                    else:
                        await self.__init_gpt_chat_agent()

                    self.logger.info(
                        f"Agent reinitialized. Active tools: {list(self.tool_chest.active_tools.keys()) if self.tool_chest else 'None'}")
                    return "Agent reinitialized successfully"  # This will update agent_status
            else:
                self.logger.warning("Agent reinitialization already in progress, skipping...")
                return "Agent reinitialization already in progress, please wait..."  # This will update agent_status
        except Exception as e:
            self.logger.error(f"Error reinitializing agent: {str(e)}")
            return f"Error reinitializing agent: {str(e)}"

    async def run(self):
        """
        Initializes the console and starts the input loop with optimized startup.
        """
        try:
            # Initialize essential components first
            init_tasks = [
                self.__init_session()
            ]
            await asyncio.gather(*init_tasks)

            # Initialize the agent
            if self.backend == 'claude':
                await self.__init_claude_chat_agent()
            else:
                await self.__init_gpt_chat_agent()

            self.logger.debug("Core initialization complete, starting UI.")

            self.__init_ui()

            # Start the UI in the background
            #self.open_browser_after_delay("http://localhost:7860", 1)
            #self.audio_cues.play_sound('app_start')

            self.ui.queue()
            self.ui.launch(
                server_name='0.0.0.0',
                server_port=7860,
                share=False,
                inbrowser=False,  # Don't auto-open browser since we're using our own method
                debug=True,
                max_threads=40,  # Maximum number of threads for handling requests
                show_error=True,
                show_api=False,  # Disable API docs for faster loading
                quiet=False,  # Reduce console output
                enable_monitoring=False,  # Disable monitoring for faster startup
                height=500,
                width="100%",
                strict_cors=False
            )
        except Exception as e:
            self.logger.error(f"Error during startup: {str(e)}")
            raise

    async def __init_session(self):
        self.logger.debug("Initializing Session...")
        self.session_manager = ChatSessionManager()
        await self.session_manager.init(self.user_id, self.session_id)
        self.session_id = self.session_manager.chat_session.session_id

    async def __init_gpt_chat_agent(self):
        """
        This sets up the chat agent for the current session.
        Here's where we declare the toolsets we want to use with the agent as well as how our system prompt will look.

        Args:
            persona_prompt (str): The prompt to initialize the ChatAgent with.
        """
        self.logger.debug("Initializing GPT Chat Agent...")
        # Declare a "tool chest" of toolsets for the model.
        # In this reference app we're not supplying a tool_classes parameter so it will grab the "kitchen sink"
        # of all tool classes that have been registered.
        # self.tool_chest = ToolChest()
        self.tool_chest = ToolChest(tool_classes=[
            tool for tool in Toolset.tool_registry
            if tool.__name__ in self.selected_tools
        ])

        # These are the default and extra options for the various toolsets, since the toolsets all use kwargs
        # we can send them the whole bag of options without worry.
        tool_opts = {'tool_cache': self.tool_cache, 'session_manager': self.session_manager,
                     'user_preferences': self.user_prefs,
                     'workspaces': self.workspaces, 'streaming_callback': self.chat_callback}

        self.logger.debug("Initializing GPT Chat Agent... Initializing toolsets...")
        await self.tool_chest.init_tools(**tool_opts)

        active_tools = list(self.tool_chest.active_tools.keys())
        self.logger.info(f"Successfully initialized tools: {active_tools}")

        # The prompt sections are grouped into "operational" and informational categories.
        # Note the XML tag as the template for the Core Instruction template, by default the core instruction set template
        # contains stuff that make talking to a development agent harder, so we shortcircuit it here BUT we have to have the XML tag there
        # That tag anchors the critical parts of the prompt into a logical group that the model can "hold on to" easier.
        # this also provides us a way to CLEARLY delineate between things WHICH MUST BE FOLLOWED and things that are informational
        # or (more importantly) USER generated.  By establishing 'operating guidelines' as a thing we can make the model evaluate its output
        # as well as user requests based on them with far greater accuracy and reliability.
        self.logger.debug("Initializing GPT Chat Agent... Initializing sections...")
        operating_sections = [
            CoreInstructionSection(template="<operating_guidelines>\n"),
            DynamicPersonaSection(),
            EndOperatingGuideLinesSection()
        ]

        operating_sections.extend(self.tool_chest.active_tool_sections)

        # Anything outside the operating guidelines is basically helpful information for the model to know.
        # These are demo sections that tell the model a little about the user as well as things like the current date / time.
        info_sections = [HelpfulInfoStartSection(),
                         EnvironmentInfoSection(session_manager=self.session_manager,
                                                voice_tools=self.tool_chest.active_tools.get('voice'),
                                                agent_voice=self.agent_voice),
                         UserBioSection(session_manager=self.session_manager)]

        self.logger.debug("Initializing GPT Chat Agent... Initializing agent...")
        # FINALLY we create the agent
        self.agent: GPTChatAgent = GPTChatAgent(
            prompt_builder=PromptBuilder(sections=operating_sections + info_sections),
            model_name=self.model_name,
            tool_chest=self.tool_chest,
            streaming_callback=self.chat_callback,
            output_format=self.agent_output_format)

    async def __init_claude_chat_agent(self):
        self.logger.debug("Initializing Claude Chat Agent...")
        self.can_use_tools = True
        self.tool_chest = ToolChest(tool_classes=[
            tool for tool in Toolset.tool_registry
            if tool.__name__ in self.selected_tools
        ])
        # These are the default and extra options for the various toolsets, since the toolsets all use kwargs
        # we can send them the whole bag of options without worry.
        tool_opts = {'tool_cache': self.tool_cache, 'session_manager': self.session_manager,
                     'user_preferences': self.user_prefs,
                     'workspaces': self.workspaces, 'streaming_callback': self.chat_callback,
                     'agent_can_use_tools': self.can_use_tools}

        await self.tool_chest.init_tools(**tool_opts)
        active_tools = list(self.tool_chest.active_tools.keys())
        self.logger.info(f"Successfully initialized tools: {active_tools}")


        operating_sections = [
            CoreInstructionSection(template="<operating_guidelines>\n"),
            DynamicPersonaSection(),
            EndOperatingGuideLinesSection()
        ]
        operating_sections.extend(self.tool_chest.active_tool_sections)

        # These are demo sections that tell the model a little about the user as well as things like the current date / time.
        info_sections = [HelpfulInfoStartSection(),
                         EnvironmentInfoSection(session_manager=self.session_manager, voice_tools=None, agent_voice=self.agent_voice),
                         UserBioSection(session_manager=self.session_manager)]

        # FINALLY we create the agent
        self.agent: ClaudeChatAgent = ClaudeChatAgent(
            prompt_builder=PromptBuilder(sections=operating_sections + info_sections), model_name=self.model_name,
            streaming_callback=self.chat_callback, output_format=self.agent_output_format, tool_chest=self.tool_chest)

    def __setup_logging(self) -> logging.Logger:
        """
        Set up a logger instance with specified log level.

        Returns:
            logging.Logger: Configured logger instance.
        """
        logger = logging.getLogger(__name__)
        other_loggers = ['httpx', 'LiteLLM', 'openai', 'httpcore', 'websockets', 'speechmatics', 'asyncio',
                         'linkedin_api', 'httpcore', 'urllib3', 'gradio', ]

        debug_other_loggers = ['agent_c_core', 'agent_c_tools', 'agent_c_tools.tools', 'agent_c_demo',
                               'agent_c_demo.tools', ]

        if self.debug_event.is_set():
            logger.setLevel(logging.DEBUG)
            for log in debug_other_loggers:
                logging.getLogger(log).setLevel(logging.DEBUG)
        else:
            logger.setLevel(logging.WARN)

        for log in other_loggers:
            logging.getLogger(log).setLevel(logging.WARN)

        return logger

    def get_tool_note(self) -> str:
        return self.tool_note

    async def chat_callback(self, event: SessionEvent):
        role_name = " ".join(word.capitalize() for word in event.role.split("_"))
        if event.role != self.last_role:
            if event.role != 'assistant':
                await self.queue.put(f"\n## From {role_name}:\n")
            elif self.last_role != 'user':
                await self.queue.put(f"<hr>")

            self.last_role = event.role

        if event.type == 'text_delta':
            await self._handle_text_delta(event)
        elif event.type == 'thinking_delta':
            await self._handle_text_delta(event)
        elif event.type == 'tool_call':
            await self._handle_tool_call_event(event, role_name)
        elif event.type == 'render_media':
            await self._handle_render_media_event(event)
        elif event.type == 'history':
            await self._handle_history_event(event)
        elif event.type == 'audio_delta':
            if event.id != self.last_audio_id:
                self.audio_player.reset_frame_count()
                self.last_audio_id = event.id

            bytes_data = base64.b64decode(event.content)
            self.audio_player.add_data(bytes_data)

    async def _handle_text_delta(self, event: TextDeltaEvent):
        if event.content is not None:
            self.token_renderer.render_token(event.content)
            await self.queue.put(event.content)  # Put the content into the output_queue

    async def _handle_message_event(self, event: MessageEvent):
        if event.content is not None:
            self.token_renderer.render_token(event.content)
            await self.queue.put(event.content)  # Put the content into the output_queue

    async def _handle_tool_call_delta(self, event: ToolCallDeltaEvent):
        pass

    async def _handle_tool_call_event(self, event: ToolCallEvent, role_name: str):
        if event.active:
            tool_calls = event.tool_calls
            tc_hash = defaultdict(list)
            for tool_call in tool_calls:
                (belt, tool) = tool_call['name'].split(Toolset.tool_sep)
                belt_name = " ".join(word.capitalize() for word in belt.split("_"))
                tool_name = " ".join(word.capitalize() for word in tool.split("_"))
                tc_hash[belt_name].append(tool_name)

            tool_lis = []
            for belt, tools in tc_hash.items():
                tool_lis.append(f"<li>{belt}: {', '.join(tools)}</li")

            self.tool_names = "\n".join([tool_li for tool_li in tool_lis])
            self.tool_note = f"\n---\n## **{role_name}** is using: \n<ul>{self.tool_names}</ul>"
            await self.queue.put('')
        else:
            await self.queue.put(f"<h5>{role_name} used:</h5>\n<ul>{self.tool_names}</ul><hr>")

            self.tool_note = ''
            self.tool_names = ''

        return

    async def _handle_interaction_event(self, event: InteractionEvent):
        pass

    async def _handle_history_event(self, event: HistoryEvent):
        self.current_chat_Log = event.messages

    async def _handle_completion_event(self, event: CompletionEvent):
        pass

    async def _handle_render_media_event(self, event: RenderMediaEvent):
        if 'image' in event.content_type:
            if event.content is not None:
                content = event.content
            elif event.content_bytes is not None:
                content = base64.b64encode(event.content_bytes).decode('utf-8')
            else:
                content = None
                src = event.url

            if 'svg' in event.content_type:
                if content:
                    # For SVG, we can embed the content directly
                    await self.queue.put(f"<br>\n{content}\n")
                else:
                    # If we only have a URL, use an object tag
                    await self.queue.put(f"<br>\n<object type='image/svg+xml' data='{src}'></object>\n")
            else:
                # For other image types, use an img tag
                if content:
                    src = f"data:{event.content_type};base64,{content}"
                await self.queue.put(f"<br>\n<img src='{src}' style='max-width: 60%; height: auto;' >\n")
        elif 'text/html' in event.content_type:
            # Handle HTML content
            html_content = event.content
            if html_content:
                # Sanitize the HTML content if necessary
                # You might want to use a library like bleach for this
                # sanitized_html = html_content  # Replace with actual sanitization if needed
                await self.queue.put(f"<br>\n{html_content}\n")
            else:
                await self.queue.put("<br>\nEmpty HTML content received.\n")
        else:
            # Handle other content types or log unhandled types
            await self.queue.put(f"<br>\nUnsupported content type: {event.content_type}\n")


    async def __build_prompt_metadata(self):
        """
        Pretty much all of this could be in a prompt sections instead but it's left as an example of how to inject data
        into the PromptBuilder chain
        """
        return {"session_id": self.session_id,
                "current_user_username": self.session_manager.user.user_id,
                "current_user_name": self.session_manager.user.first_name,
                "session_summary": self.session_manager.chat_session.active_memory.summary,
                "persona_prompt": self.persona_prompt,
                "timestamp": datetime.now().isoformat(),
                "env_name": os.getenv('ENV_NAME', 'development'),
                "session_info": self.session_manager.chat_session.session_id if self.session_manager else None
                }

