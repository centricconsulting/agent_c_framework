import re
import httpx
from typing import List, Optional, Dict, Tuple

from selenium import webdriver
from selenium.webdriver.chrome.options import Options

from agent_c.models.context.interaction_context import InteractionContext
from agent_c_tools.tools.web.formatters import *
from agent_c.toolsets import json_schema, Toolset
from agent_c_tools.tools.workspace.tool import WorkspaceTools
from agent_c_tools.tools.workspace.base import BaseWorkspace
from agent_c_tools.tools.web.util.expires_header import expires_header_to_cache_seconds

class WebTools(Toolset):
    """
    The Web Tool enables agents to access and interact with web pages during your conversation. This capability allows you to get information from specific websites without having to leave the chat, switch applications, or copy-paste content back and forth.

    ## Key Capabilities

    Agents equipped with this tool can help you work with web content in various ways:

    - **Information Extraction**: Get clean, readable content from articles, blog posts, and web pages
    - **Research Assistance**: Access specific information from websites you're interested in
    - **Content Summarization**: Obtain and summarize key points from longer web articles
    - **Reference Collection**: Save important web content for later use in your projects
    - **Visual Access**: View the original web page when needed for visual elements or interaction
    """

    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='web', use_prefix=False)
        self.default_formatter: ContentFormatter = kwargs.get('wt_default_formatter',
                                                              ReadableFormatter(re.compile(r".*")))
        self.formatters: List[ContentFormatter] = kwargs.get('wt_formatters', [])
        self.driver = self.__init__wd()
        self.workspace_tool: Optional[WorkspaceTools] = None

    async def post_init(self):
        self.workspace_tool: WorkspaceTools = self.tool_chest.available_tools.get("WorkspaceTools")

    def __init__wd(self):
        try:
            options = Options()
            options.add_argument("--headless=new")
            # This is to get chrome driver to ignore SSL handshake between chromium and chrome, that resulted in the following error:
            # [23312:116032:0716/133843.497:ERROR:ssl_client_socket_impl.cc(878)] handshake failed; returned -1, SSL error code 1, net_error -100
            options.add_argument('--ignore-certificate-errors-spki-list')
            options.add_argument('--ignore-certificate-errors')
            options.add_argument('--ignore-ssl-errors')
            # This is to get rid of the following error:
            # [115200:111428:0716/130536.999:ERROR:sandbox_win.cc(913)] Sandbox cannot access executable....
            # https://github.com/puppeteer/puppeteer/blob/main/docs/troubleshooting.md
            options.add_argument('--no-sandbox')  # This is riskier
            options.add_argument('--log-level=3')
            options.add_argument('--disable-dev-shm-usage')
            options.add_argument('--disable-gpu')
            driver = webdriver.Chrome(options=options)
        except Exception as e:
            self.logger.warning(f"Failed to initialize WebDriver: {e}")
            return None

        return driver

    def __del__(self):
        if self.driver:
            self.logger.info("Closing WebDriver")
            self.driver.close()

    def format_content(self, content: str, url: str) -> str:
        """
        Find a suitable formatter for the given URL and format the content.

        Args:
            content (str): Content to be formatted.
            url (str): URL of the content.

        Returns:
            str: Formatted content in Markdown.
        """
        formatter = next((f for f in self.formatters if f.match(url)), self.default_formatter)
        return formatter.format(content, url)

    async def _fetch_content(self, url: str, expire_secs: int, headers: Dict[str, str]) -> Tuple[Optional[str], Optional[str]]:
        if self.tool_cache.get(f"{url}_RAW") is not None:
            self.logger.debug(f'URL found in cache: {url}_RAW')
            return self.tool_cache.get(f"{url}_RAW")

        async with httpx.AsyncClient() as client:
            try:
                if "User-Agent" not in headers:
                    headers["User-Agent"] = "Mozilla/5.0 (iPad; CPU OS 12_2 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Mobile/15E148"

                response = await client.get(url, headers=headers)
                if response.status_code == 403 and self.driver is not None:
                    self.driver.get(url)
                    expires_in = 600
                    response_content = self.driver.page_source
                else:
                    response.raise_for_status()
                    expires_in = expires_header_to_cache_seconds(response.headers.get('expires'))
                    encoding = response.encoding or 'utf-8'
                    response_content = response.content.decode(encoding)

                if expires_in is not None:
                    self.tool_cache.set(f"{url}_RAW", response_content, expire=expires_in)
                    self.logger.debug(f'URL cached with expiration: {url}_RAW. Expires in {expires_in} seconds (from headers')
                else:
                    self.tool_cache.set(f"{url}_RAW", response_content, expire=expire_secs)
                    self.logger.debug(f'URL cached with with caller specified expiration: {url}_RAW. Expires in {expire_secs} seconds')

                return None, response_content
            except httpx.HTTPStatusError as e:
                self.logger.exception(f'HTTP error occurred while fetching {url}: {e}')
                return f'HTTP error occurred: {e}', None
            except httpx.RequestError as e:
                self.logger.exception(f'Request error occurred while fetching {url}: {e}')
                return f'Request error occurred: {e}', None
            except Exception as e:
                self.logger.exception(f'An error occurred while fetching {url}: {e}')
                return f'An error occurred: {e}', None

    @json_schema(
        'Fetch a web page in markdown format (preferred) or with raw output, optionally saving it to a workspace.',
        {
            'url': {
                'type': 'string',
                'description': 'The URL of the web page you would like to fetch',
                'required': True
            },
            'save_to_path': {
                'type': 'string',
                'description': 'An optional path workspace UNC path to save the content to.',
                'required': False
            },
            'save_only': {
                'type': 'boolean',
                'description': 'If true, the content will be saved to the workspace but not returned to the caller. Default is Fals',
                'required': False
            },
            'raw_output': {
                'type': 'boolean',
                'description': 'If true the raw content will be returned/saved',
                'required': False
            },
            'max_tokens': {
                'type': 'integer',
                'description': 'Maximum number of tokens to return. Default is 20k',
                'required': False
            },
            'additional_headers': {
                'type': 'object',
                'description': 'Additional headers to include in the request. As a dictionary of key-value pairs.',
                'required': False
            },
        }
    )
    async def get(self, **kwargs):
        url: Optional[str] = kwargs.get('url', None)
        save_only: bool = kwargs.get('save_only', False)
        save_to_path: Optional[str] = kwargs.get('save_to_path', None)
        raw_output: bool = kwargs.get('raw_output', False)
        max_tokens: int = kwargs.get('max_tokens', 20000)
        default_expire: int = kwargs.get("expire_secs", 3600)
        tool_context: InteractionContext = kwargs.get("context")
        additional_headers: Dict[str, str] = kwargs.get('additional_headers', {})
        workspace: Optional[BaseWorkspace] = None

        if url is None:
            return "Error: URL is required."

        if save_to_path is not None:
            error, workspace_name, file_path = self.workspace_tool.validate_and_get_workspace_path(save_to_path)
            if error:
                return f"Invalid save path: {error}"
            try:
                workspace: BaseWorkspace = self.workspace_tool.workspaces[workspace_name] if workspace_name else None
            except KeyError:
                self.logger.error(f"Workspace {workspace_name} not found in available workspaces.")
                return f"Error: Workspace {workspace_name} not found."

            if workspace is None:
                return f"Error: No workspace found with the name: {workspace_name}"


        if save_only and save_to_path is None:
            return "Error: save_to_path must be provided if save_only is True."

        error_message, content = await self._fetch_content(url=url, expire_secs=default_expire, headers=additional_headers)
        if error_message is not None:
            return error_message

        if not raw_output:
            content = self.format_content(content, url)
            await self._render_media_markdown(tool_context, content, "fetch_url")

        token_count = tool_context['agent_runtime'].count_tokens(content)

        if workspace is not None:
            await workspace.write(file_path, mode='write', data=content)
            if save_only:
                return f"{url} content saved to {save_to_path}. There are {token_count} tokens in the content."


        if token_count > max_tokens:
            error_msg = (f"ERROR: The content of this URL exceeds max_tokens limit of {max_tokens}. "
                         f"Content token count: {token_count}.")
            if workspace:
                error_msg += f"\nContent has been saved to {save_to_path}. You may use the workspace tools to access it."

            if raw_output is False:
                error_msg += "\nNOTICE: The content HAS been displayed to the user however."

            return error_msg

        return content


Toolset.register(WebTools, required_tools=['WorkspaceTools'])
