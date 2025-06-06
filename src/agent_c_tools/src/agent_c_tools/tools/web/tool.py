import re
import httpx
import json
import logging
import datetime

from typing import List, Optional, Dict

from selenium import webdriver
from selenium.webdriver.chrome.options import Options

from agent_c.toolsets import json_schema, Toolset
from .formatters import *
from .util.expires_header import expires_header_to_cache_seconds
# Use local import to avoid circular dependencies
from ..workspace import WorkspaceTools
from ...helpers.path_helper import ensure_file_extension, create_unc_path


class WebTools(Toolset):
    """
    WebTools class to fetch and format web page content.

    Attributes:
        default_formatter (ContentFormatter): Default formatter to use when no specific formatter is found.
        formatters (List[ContentFormatter]): List of custom formatters for specific URL patterns.
        cache (Cache): Cache object for storing responses.
    """

    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='web', use_prefix=False)
        self.default_formatter: ContentFormatter = kwargs.get('wt_default_formatter',
                                                              ReadableFormatter(re.compile(r".*")))
        self.formatters: List[ContentFormatter] = kwargs.get('wt_formatters', [])
        self.driver = self.__init__wd()
        self.workspace_tool = Optional[WorkspaceTools]
        
        # Initialize tool_cache if not provided
        if not hasattr(self, 'tool_cache') or self.tool_cache is None:
            try:
                from agent_c.toolsets.tool_cache import ToolCache
                self.tool_cache = ToolCache()
                logging.info("Initialized default ToolCache for WebTools")
            except ImportError:
                logging.warning("Could not import ToolCache, caching will be disabled")
                self.tool_cache = None

    async def post_init(self):
        self.workspace_tool = self.tool_chest.available_tools.get("WorkspaceTools")
        # # Defensively access dependencies
        # if hasattr(self, 'tool_chest') and self.tool_chest is not None:
        #     self.workspace_tool = self.tool_chest.active_tools.get("WorkspaceTools")
        #
        #     # If tool_cache wasn't initialized in __init__, try to get it from tool_chest
        #     if not hasattr(self, 'tool_cache') or self.tool_cache is None:
        #         try:
        #             from agent_c.toolsets.tool_cache import ToolCache
        #             self.tool_cache = getattr(self.tool_chest, 'tool_cache', ToolCache())
        #             logging.info("Retrieved ToolCache from tool_chest or created a new one")
        #         except Exception as e:
        #             logging.warning(f"Failed to initialize tool_cache: {e}")
        #             self.tool_cache = None

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
            return None

        return driver

    def __del__(self):
        if self.driver:
            logging.info("Closing WebDriver")
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

    @json_schema(
        'Fetch the content of a web page and return it to the LLM in Markdown format. Do not save it to the workspace.',
        {
            'url': {
                'type': 'string',
                'description': 'The URL of the web page you would like to fetch',
                'required': True
            }
        }
    )
    async def fetch_webpage(self, **kwargs) -> str:
        """
        Fetch a webpage's content and convert it to Markdown format using the specified formatter.

        Args:
            **kwargs: Keyword arguments containing the 'url' and possibly other configuration details.
                url: The URL of the web page to fetch
                raw: Whether to return the raw HTML instead of formatted markdown
                expire_secs: How long to cache the content
                save_to_workspace: Whether to save the markdown content to a workspace
                workspace_name: Workspace name to use for saving the markdown file
                file_path: File path within the workspace to save the markdown file

        Returns:
            str: Page content in Markdown format or an error message if an exception occurs.
                 If saving to workspace, returns a JSON string with the status.
        """
        url: str = kwargs.get('url')
        raw: bool = kwargs.get("raw", False)
        default_expire: int = kwargs.get("expire_secs", 3600)

        if url is None:
            return 'url is required'
            
        # Handle case where tool_cache is not initialized
        # response_content: Optional[str] = None
        # if hasattr(self, 'tool_cache') and self.tool_cache is not None:
        response_content = self.tool_cache.get(url)
        if response_content is not None:
            return response_content

        async with httpx.AsyncClient() as client:
            try:
                headers = {
                    "User-Agent": "Mozilla/5.0 (iPad; CPU OS 12_2 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Mobile/15E148"}
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

                if not raw:
                    response_content = self.format_content(response_content, url)

                # Only cache if tool_cache is available
                if hasattr(self, 'tool_cache') and self.tool_cache is not None:
                    if expires_in is not None:
                        self.tool_cache.set(url, response_content, expire=expires_in)
                        logging.debug(f'URL cached with expiration: {url}')
                    else:
                        self.tool_cache.set(url, response_content, expire=default_expire)
                        logging.debug(f'URL cached with default expiration: {url}')

                return response_content
            except httpx.HTTPStatusError as e:
                logging.error(f'An error occurred: - HTTP error: {e}')
                return f'HTTP error occurred: {e}'
            except httpx.RequestError as e:
                logging.error(f'An error occurred - Request error: {e}')
                return f'Request error occurred: {e}'
            except Exception as e:
                logging.error(f'An error occurred - General error: {e}')
                return f'An error occurred: {e}'

    @json_schema(
        'Fetch and save webpage. Fetch the content of a web page, save it to a workspace and optionally return it to the LLM in Markdown format.',
        {
            'url': {
                'type': 'string',
                'description': 'The URL of the web page you would like to fetch',
                'required': True
            },
            'return_to_llm': {
                'type': 'boolean',
                'description': 'Whether return the browser page content to the LLM',
                'required': False,
                'default': True
            },
            'workspace': {
                'type': 'string',
                'description': 'Workspace to use for saving the markdown file',
                'required': True,
                'default': 'project'
            },
            'file_path': {
                'type': 'string',
                'description': 'Relative file path within the workspace to save the markdown file',
                'required': False
            }
        }
    )
    async def fetch_and_save(self, **kwargs):
        url: str = kwargs.get('url')
        raw: bool = kwargs.get("raw", False)
        default_expire: int = kwargs.get("expire_secs", 3600)
        return_to_llm: bool = kwargs.get("save_to_workspace", True)
        workspace_name: str = kwargs.get("workspace", "project")
        file_path: str = kwargs.get("file_path")

        if url is None:
            return 'url is required'

        response_content = await self.fetch_webpage(url=url, raw=raw, expire_secs=default_expire)

        if isinstance(response_content, str) and 'An error occurred:' in response_content:
            return f"An error occured fetching web page: {response_content}"

        try:
            # If file_path is not provided, generate one based on the URL
            if file_path is None:
                # Extract domain from URL and create a clean filename
                from urllib.parse import urlparse
                parsed_url = urlparse(url)
                domain = parsed_url.netloc.replace('.', '_')
                timestamp = datetime.datetime.now().strftime("%Y%m%d_%H%M%S")

                file_path = f"{domain}_{timestamp}.md"

            # Ensure file has .md extension
            file_path = ensure_file_extension(file_path, "md")

            # Get workspace toolset
            if self.workspace_tool is None:
                return json.dumps({
                    'error': "Workspace tool not available. Cannot save markdown.",
                    'content': response_content
                })

            # Find the workspace and save the content
            unc_path = create_unc_path(workspace_name, file_path)
            error, workspace, relative_path = self.workspace_tool.validate_and_get_workspace_path(unc_path)
            if error:
                return f"Invalid path: {error}"
            result = await self.workspace_tool.write(path=unc_path, mode='write', data=response_content)
            result_data = json.loads(result)
            if 'error' in result_data:
                return f"Error writing file: {result_data['error']}"

            # Get the full path of the file
            # os_path = os_file_system_path(self.workspace_tool, unc_path)

            # workspace_obj = workspace_tool.find_workspace_by_name(workspace)
            # if workspace_obj is None:
            #     return json.dumps({
            #         'error': f"No workspace found with the name: {workspace}",
            #         'content': response_content
            #     })
            #
            # # Save the markdown content to the workspace
            # result = await workspace_obj.write(file_path=file_path, mode='write', data=response_content)
            logging.info(f'Saved webpage content to workspace: {workspace_name} with file name: {file_path}')

            return_dict: Dict[str, str] = {
                'save_result': result,
                'message': f"Webpage content saved to {workspace_name}/{file_path}",
                'file_path': file_path,
                'workspace_name': workspace_name
            }

            if return_to_llm:
                return_dict['content'] = response_content

            return json.dumps(return_dict)

        except Exception as e:
            logging.error(f'Error saving to workspace: {str(e)}')
            return json.dumps({
                'error': f'Error saving to workspace: {str(e)}',
                'content': response_content,
                'result': result if 'result' in locals() else None
            })


Toolset.register(WebTools)
