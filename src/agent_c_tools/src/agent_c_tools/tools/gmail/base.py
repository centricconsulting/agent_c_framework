import os
import json
import logging
from enum import Enum
from typing import Optional, Any, Dict

from google.oauth2.credentials import Credentials
from google.auth.transport.requests import Request
from google_auth_oauthlib.flow import InstalledAppFlow
from googleapiclient.discovery import build

from agent_c.toolsets import Toolset


class GmailResourceType(str, Enum):
    """Enumerator of Resources to search."""
    THREADS = "threads"
    MESSAGES = "messages"

class ReturnType(str, Enum):
    """Enum for return type options."""
    BOTH = "both"  # Return to both agent and UI
    AGENT = "agent"  # Return only to agent
    UI_ONLY = "ui_only"  # Return only to UI

class GmailError(Exception):
    """Base exception for Gmail-related errors."""
    pass


class GmailBase(Toolset):
    """Base class for Gmail-related tools."""
    SCOPES = ['https://www.googleapis.com/auth/gmail.modify']
    MAX_EMAIL_BODY_TOKEN_SIZE = 25000  # Maximum token size for email body

    def __init__(self, **kwargs):
        super().__init__(**kwargs, needed_keys=['GOOGLE_CREDENTIALS_FILE'])
        self.logger: logging.Logger = logging.getLogger(__name__)
        self.workspace_tool = self.tool_chest.active_tools.get('WorkspaceTools')
        self.service = self._initialize_gmail_service()
        self.cache_expire = 300  # 5 minute default cache expiration for emails
        self.MAX_TOKEN_SIZE = 35000


    def _initialize_gmail_service(self):
        """Initialize Gmail API service with improved token refresh handling."""
        try:
            creds = None
            config_dir = 'config'
            token_path = os.path.join(config_dir, 'token.json')
            credentials_path = os.path.join(config_dir, os.getenv('GOOGLE_CREDENTIALS_FILE'))

            # Validate credentials path
            if not credentials_path:
                raise GmailError("GOOGLE_CREDENTIALS_FILE environment variable not set")

            # Resolve relative path if needed
            if not os.path.isabs(credentials_path):
                credentials_path = os.path.abspath(credentials_path)

            # Check if credentials file exists
            if not os.path.exists(credentials_path):
                raise GmailError(f"Credentials file not found at: {credentials_path}")

            self.logger.debug(f"Using credentials file: {credentials_path}")

            # Load existing token
            if os.path.exists(token_path):
                try:
                    creds = Credentials.from_authorized_user_file(filename=token_path, scopes=self.SCOPES)
                    self.logger.info("Loaded existing token")
                except Exception as e:
                    self.logger.error(f"Error loading existing token: {e}")
                    creds = None

            # Refresh or get new token
            needs_new_token = False
            if not creds or not creds.valid:
                if creds and creds.expired and creds.refresh_token:
                    self.logger.info("Attempting to refresh expired token")
                    try:
                        creds.refresh(Request())
                        self.logger.info("Successfully refreshed token")
                    except Exception as e:
                        self.logger.warning(f"Token refresh failed: {e}. Will attempt new OAuth flow.")
                        needs_new_token = True
                else:
                    needs_new_token = True

                if needs_new_token:
                    self.logger.info("Initiating new OAuth flow")
                    try:
                        # Remove the old token file if it exists
                        if os.path.exists(token_path):
                            os.remove(token_path)
                            self.logger.info("Removed old token file")

                        flow = InstalledAppFlow.from_client_secrets_file(credentials_path, self.SCOPES)
                        creds = flow.run_local_server(
                            port=0,
                            success_message="Authentication successful! You can close this window.",
                            authorization_prompt_message="Please authenticate in your browser.",
                            access_type='offline',
                            prompt='consent'
                        )
                        self.logger.info("OAuth flow completed successfully")
                    except Exception as e:
                        raise GmailError(f"Failed to authenticate: {str(e)}")

                # Save the token
                try:
                    with open(token_path, 'w') as token:
                        token.write(creds.to_json())
                    self.logger.info(f"Saved new token to {token_path}")
                except Exception as e:
                    self.logger.warning(f"Failed to save token: {e}")

            return build('gmail', 'v1', credentials=creds)

        except Exception as e:
            self.logger.error(f"Gmail service initialization failed: {str(e)}")
            raise GmailError(f"Failed to initialize Gmail service: {str(e)}")

    @staticmethod
    def clean_email_body(body: str) -> str:
        """Clean the email body text."""
        # Look at this for future https://python.langchain.com/api_reference/_modules/langchain_google_community/gmail/utils.html
        lines = body.splitlines()
        cleaned_lines = []
        for line in lines:
            line = line.strip()
            if line and not line.startswith('>'):
                cleaned_lines.append(line)
        return ' '.join(cleaned_lines)

    def is_content_too_large(self, content: str) -> bool:
        token_count = self.tool_chest.agent.count_tokens(content)
        return token_count > self.MAX_EMAIL_BODY_TOKEN_SIZE

    def truncate_content_by_approx_tokens(self, content: str, max_tokens: int) -> str:
        """Truncate content to stay within token budget while preserving readability."""
        if not self.is_content_too_large(content):
            return content

        # Simple but smarter truncation
        # Estimate characters per token (typically 4-5 chars per token for English)
        char_estimate = max_tokens * 4
        truncated = content[:char_estimate]

        # Try to end at a sentence boundary
        last_period = max(truncated.rfind('.'), truncated.rfind('!'), truncated.rfind('?'))
        if last_period > 0:
            truncated = truncated[:last_period + 1]

        return truncated + " [...content truncated due to size]"

    async def handle_return_type(
            self,
            output: Any,
            return_type: str,
            calling_function: str,
            ui_message: Optional[str] = None,
            error: Optional[Exception] = None,
            tool_context: Optional[Dict] = None
    ) -> str:
        """Handle different return types consistently across all tools."""
        if isinstance(output, (dict, list)):
            output_str = json.dumps(output)
        else:
            output_str = str(output)


        # handle the case where the output is too large
        output_str = self.truncate_content_by_approx_tokens(output_str, self.MAX_TOKEN_SIZE)

        if error:
            error_msg = str(error)
            if return_type in [ReturnType.UI_ONLY, ReturnType.BOTH]:
                await self._raise_render_media(
                    sent_by_class=self.__class__.__name__,
                    sent_by_function=calling_function,
                    content_type="text/html",
                    content=f"Error: {error_msg}",
                    tool_context=tool_context
                )
            if return_type == ReturnType.UI_ONLY:
                return ""
            return self._error_response(error_msg)

        if return_type == ReturnType.UI_ONLY:
            if ui_message:
                await self._raise_render_media(
                    sent_by_class=self.__class__.__name__,
                    sent_by_function=calling_function,
                    content_type="text/html",
                    content=ui_message,
                    tool_context=tool_context
                )
            else:
                await self._raise_render_media(
                    sent_by_class=self.__class__.__name__,
                    sent_by_function=calling_function,
                    content_type="text/html",
                    content=output_str,
                    tool_context=tool_context
                )
            return "Results returned to user directly"
        elif return_type == ReturnType.BOTH:
            if ui_message:
                await self._raise_render_media(
                    sent_by_class=self.__class__.__name__,
                    sent_by_function=calling_function,
                    content_type="text/html",
                    content=ui_message,
                    tool_context=tool_context
                )
            else:
                await self._raise_render_media(
                    sent_by_class=self.__class__.__name__,
                    sent_by_function=calling_function,
                    content_type="text/html",
                    content=output_str,
                    tool_context=tool_context
                )
            return output_str
        else:  # ReturnType.AGENT
            return output_str

    @staticmethod
    def _error_response(message: str) -> str:
        return json.dumps({'error': message})