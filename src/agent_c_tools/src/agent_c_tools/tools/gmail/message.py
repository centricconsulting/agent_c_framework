import html
import base64
import mimetypes
import re
from email.mime.base import MIMEBase
from email.mime.multipart import MIMEMultipart
from email.mime.text import MIMEText
from email import encoders
import logging
from typing import Optional, Dict, Any, Union, List

from agent_c.toolsets import Toolset, json_schema
from .base import GmailBase, ReturnType
from ...helpers.path_helper import create_unc_path, os_file_system_path


class GmailMessage(GmailBase):
    """
    Gmail message management toolset for email operations.
    
    This toolset provides comprehensive Gmail integration for reading, sending, and drafting emails,
    including support for attachments from workspace. It handles email threads, message parsing,
    and content cleanup for optimal token usage.
    
    Available Methods:
        - get_message: Fetch an email by message ID, including attachments
        - send_message: Send emails with optional attachments from workspace
        - draft_message: Create email drafts with optional attachments
        - get_thread: Fetch an entire email thread by thread ID
    
    Key Features:
        - Full email content retrieval with HTML-to-text conversion
        - Attachment download and workspace integration
        - Email composition with cc, bcc, and multiple recipients
        - Thread-based email retrieval
        - Automatic content cleanup (removes quoted text, signatures, forwarded markers)
        - Token-optimized email body handling with automatic truncation
        - MIME multipart message support
        - Base64 encoding/decoding for attachments
    
    Requirements:
        - Gmail API credentials via GmailBase (OAuth2)
        - WorkspaceTools (for attachment operations)
        - google-api-python-client package
        - BeautifulSoup for HTML parsing
    
    Usage Notes:
        - Email bodies are automatically cleaned to remove noise and reduce token usage
        - Large email bodies (>configurable limit) are automatically truncated
        - HTML emails are converted to plain text
        - Attachments are saved with full workspace UNC paths
        - Supports both plain text and HTML email composition
    """
    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='gmail')
        self.logger: logging.Logger = logging.getLogger(__name__)

    @json_schema(
        description="Fetch an email by message ID, including attachments. Returns the thread ID, snippet, body, subject, sender, and attachment info.",
        params={
            "message_id": {
                "type": "string",
                "description": "The ID of the email message to retrieve.",
                "required": True
            },
            "save_attachments": {
                "type": "boolean",
                "description": "Whether to save attachments to the workspace.",
                "required": False,
                "default": False
            },
            'workspace_name': {
                'type': 'string',
                'description': 'The name of the workspace to save any attachments.',
                'required': False,
                'default': 'project'
            },
            'folder_path': {
                'type': 'string',
                'description': "'The folder_path for saving any attachments. Defaults to the workspace 'attachments'.'",
                'required': False
            },
            "return_type": {
                "type": "string",
                "description": "Specify where to return the results: 'agent', 'ui_only', or 'both'.",
                "enum": ["agent", "ui_only", "both"],
                "required": False
            }
        }
    )
    async def get_message(self, **kwargs) -> str:
        self.logger.info(f"Fetching email message with parameters: {kwargs}")
        message_id = kwargs.get('message_id')
        save_attachments = kwargs.get('save_attachments', False)
        workspace_name = kwargs.get('workspace_name', 'project')
        folder_path = kwargs.get('folder_path', 'attachments')
        return_type = kwargs.get('return_type', 'agent')
        tool_context = kwargs.get('tool_context', {})

        try:
            # Fetch the email message
            query = self.service.users().messages().get(userId="me", format="full", id=message_id)
            message_data = query.execute()

            # Parse the email content
            payload = message_data['payload']
            headers = payload.get('headers', [])
            parts = payload.get('parts', [])
            body = await self._get_body_from_payload(payload, tool_context)
            subject = self._get_header(headers, 'Subject')
            sender = self._get_header(headers, 'From')

            attachments_info = []

            # Process parts to find attachments
            if save_attachments and parts:
                for part in parts:
                    if part.get('filename'):
                        attachment_id = part['body']['attachmentId']
                        attachment = self.service.users().messages().attachments().get(
                            userId="me", messageId=message_id, id=attachment_id
                        ).execute()

                        file_data = base64.urlsafe_b64decode(attachment['data'])
                        filename = part['filename']
                        # Save the attachment to the workspace

                        # Save the attachment to the workspace using write_bytes
                        unc_path = create_unc_path(workspace_name, f"{folder_path}/{filename}")
                        error, workspace, relative_path = self.workspace_tool.validate_and_get_workspace_path(unc_path)
                        if error:
                            return f"Invalid path: {error}"

                        try:
                            await self.workspace_tool.internal_write_bytes(path=unc_path, data=file_data, mode="write")
                            os_path = os_file_system_path(self.workspace_tool, unc_path)

                            self.logger.info(f"Saved attachment {filename} to workspace")
                            attachments_info.append({
                                'filename': filename,
                                'path': unc_path,
                                'os_path': os_path
                            })
                        except Exception as e:
                            self.logger.error(f"Error saving attachment {filename}: {e}")

            # Prepare the result
            result = {
                "id": message_id,
                "threadId": message_data["threadId"],
                "snippet": message_data.get("snippet", ""),
                "body": body,
                "subject": subject,
                "sender": sender,
                "attachments": attachments_info
            }

            # Handle the return type
            return await self.handle_return_type(output=result, return_type=return_type, calling_function='get_message', tool_context=tool_context)

        except Exception as e:
            self.logger.error(f"Error fetching email message: {e}")
            return await self.handle_return_type(output={}, return_type=return_type, error=e, calling_function='get_message', tool_context=tool_context)

    async def _get_body_from_payload(self, payload, tool_context):
        """Extract the body from the email payload, prioritizing plain text."""
        # First check for plain text
        plain_text = await self._extract_mime_content(payload, 'text/plain')
        if plain_text:
            return await self._process_text_content(plain_text, tool_context)

        # Fall back to HTML if no plain text
        html_content = await self._extract_mime_content(payload, 'text/html')
        if html_content:
            return await self._convert_html_to_text(html_content, tool_context)

        # Last resort - try body data
        body_data = payload.get('body', {}).get('data')
        if body_data:
            try:
                text = base64.urlsafe_b64decode(body_data).decode('utf-8', errors='replace')
                return await self._process_text_content(text, tool_context)
            except Exception as e:
                self.logger.warning(f"Error decoding body content: {e}")

        return ""

    async def _extract_mime_content(self, payload, mime_type):
        """Extract content of a specific MIME type from payload."""
        if payload.get('mimeType') == mime_type:
            body_data = payload.get('body', {}).get('data')
            if body_data:
                try:
                    return base64.urlsafe_b64decode(body_data).decode('utf-8', errors='replace')
                except Exception:
                    try:
                        return base64.urlsafe_b64decode(body_data).decode('latin-1', errors='replace')
                    except Exception as e:
                        self.logger.warning(f"Failed to decode {mime_type} content: {e}")
                        return None

        # Recursive search through parts
        if 'parts' in payload:
            for part in payload['parts']:
                content = await self._extract_mime_content(part, mime_type)
                if content:
                    return content

        return None

    async def _process_text_content(self, text, tool_context):
        """Clean and process plain text content."""
        # Remove quoted text (lines starting with >)
        text = re.sub(r'(?m)^>.*$', '', text)

        # Remove forwarded message markers
        text = re.sub(r'---+ ?Forwarded message ?---+.*?(?=\n\n|\Z)', '', text, flags=re.DOTALL)

        # Remove email signature
        text = re.sub(r'\n--\s*\n.*', '', text, flags=re.DOTALL)

        # Clean up whitespace
        text = re.sub(r'\n{3,}', '\n\n', text)

        # Check if content is too large
        if self.is_content_too_large(text, tool_context):
            return self.truncate_content_by_approx_tokens(text, self.MAX_EMAIL_BODY_TOKEN_SIZE, tool_context)

        return text.strip()

    async def _convert_html_to_text(self, html_content, tool_context):
        """Convert HTML to plain text."""
        try:
            # Use BeautifulSoup to parse HTML
            from bs4 import BeautifulSoup
            soup = BeautifulSoup(html_content, 'html.parser')

            # Remove scripts, styles, and hidden elements
            for element in soup(["script", "style", "head", "title", "meta"]):
                element.extract()

            # Get text and clean it
            text = soup.get_text(separator='\n')

            # Clean up whitespace
            lines = (line.strip() for line in text.splitlines())
            text = '\n'.join(line for line in lines if line)

            return await self._process_text_content(text, tool_context)
        except ImportError:
            # Fallback: basic HTML tag removal
            text = re.sub(r'<[^>]+>', ' ', html_content)
            text = html.unescape(text)
            return await self._process_text_content(text, tool_context)

    @staticmethod
    def _get_header(headers, name):
        """Retrieve a header value from the email headers."""
        for header in headers:
            if header['name'] == name:
                return header['value']
        return ''

    @staticmethod
    def _format_addresses(addresses: Union[str, List[str]]) -> str:
        """Format email addresses for the MIME message."""
        if isinstance(addresses, str):
            addresses_list = [addr.strip() for addr in addresses.split(',')]
        else:
            addresses_list = addresses
        return ', '.join(addresses_list)

    @json_schema(
        description="Send an email message with optional attachments from the workspace. "
                    "Provide the message body, recipient(s), subject, optional cc, bcc, and attachment filenames.",
        params={
            "message": {
                "type": "string",
                "description": "The body of the email message to send.",
                "required": True
            },
            "to": {
                "type": "string",
                "description": "The email address(es) of the recipient(s). Separate multiple addresses with commas.",
                "required": True
            },
            "subject": {
                "type": "string",
                "description": "The subject of the email.",
                "required": True
            },
            "cc": {
                "type": "string",
                "description": "The email address(es) of the cc recipient(s). Separate multiple addresses with commas.",
                "required": False
            },
            "bcc": {
                "type": "string",
                "description": "The email address(es) of the bcc recipient(s). Separate multiple addresses with commas.",
                "required": False
            },
            "attachments": {
                "type": "array",
                "items": {
                    "type": "string"
                },
                "description": "List of full UNC workspace name, folder path, filenames of attachments from the workspace to attach.",
                "required": False
            },
        }
    )
    async def send_message(self, **kwargs) -> str:
        self.logger.info(f"Sending email message with parameters: {kwargs}")
        message_body = kwargs.get('message')
        to = kwargs.get('to')
        subject = kwargs.get('subject')
        cc = kwargs.get('cc')
        bcc = kwargs.get('bcc')
        attachments = kwargs.get('attachments', [])
        tool_context = kwargs.get('tool_context', {})

        # Validate required parameters
        if not message_body or not to or not subject:
            error_msg = "Missing required parameters: 'message', 'to', and 'subject' are required."
            self.logger.error(error_msg)
            return await self.handle_return_type(output={}, return_type=ReturnType.AGENT, error=Exception(error_msg), calling_function='send_message', tool_context=tool_context)

        try:
            email_message = await self._prepare_message(
                message=message_body,
                to=to,
                subject=subject,
                cc=cc,
                bcc=bcc,
                attachments=attachments
            )
            # Send the email
            send_message = self.service.users().messages().send(userId="me", body=email_message)
            sent_message = send_message.execute()
            result = f"Message sent. Message Id: {sent_message['id']}"

            return result

        except Exception as e:
            self.logger.error(f"Error sending email message: {e}")
            return f"Error sending email message: {e}"

    @json_schema(
        description="Draft an email message with optional attachments from the workspace. "
                    "Provide the message body, recipient(s), subject, optional cc, bcc, and attachment filenames.",
        params={
            "message": {
                "type": "string",
                "description": "The body of the email message to send.",
                "required": True
            },
            "to": {
                "type": "string",
                "description": "The email address(es) of the recipient(s). Separate multiple addresses with commas.",
                "required": False
            },
            "subject": {
                "type": "string",
                "description": "The subject of the email.",
                "required": False
            },
            "cc": {
                "type": "string",
                "description": "The email address(es) of the cc recipient(s). Separate multiple addresses with commas.",
                "required": False
            },
            "bcc": {
                "type": "string",
                "description": "The email address(es) of the bcc recipient(s). Separate multiple addresses with commas.",
                "required": False
            },
            "attachments": {
                "type": "array",
                "items": {
                    "type": "string"
                },
                "description": "List of filenames of attachments from the workspace to include.",
                "required": False
            },
        }
    )
    async def draft_message(self, **kwargs) -> str:
        self.logger.info(f"Drafting an email message with parameters: {kwargs}")
        message_body = kwargs.get('message')
        to = kwargs.get('to', '')
        subject = kwargs.get('subject', '')
        cc = kwargs.get('cc')
        bcc = kwargs.get('bcc')
        attachments = kwargs.get('attachments', [])
        tool_context = kwargs.get('tool_context', {})

        # Validate required parameters
        if not message_body:
            error_msg = "Missing required parameters: 'message' are required."
            self.logger.error(error_msg)
            return await self.handle_return_type(output={}, return_type=ReturnType.AGENT, error=Exception(error_msg), calling_function='draft_message', tool_context=tool_context)

        try:
            email_message = await self._prepare_message(
                message=message_body,
                to=to,
                subject=subject,
                cc=cc,
                bcc=bcc,
                attachments=attachments
            )
            # Send the email
            create_draft  = self.service.users().drafts().create(userId="me", body={"message": email_message})
            draft_message = create_draft.execute()
            result = f"Message sent. Message Id: {draft_message['id']}"

            return result

        except Exception as e:
            self.logger.error(f"Error drafting email message: {e}")
            return f"Error drafting email message: {e}"

    @json_schema(
        description="Fetch an email thread by thread ID. Returns the thread details and a list of messages.",
        params={
            "thread_id": {
                "type": "string",
                "description": "The ID of the email thread to retrieve.",
                "required": True
            },
            "return_type": {
                "type": "string",
                "description": "Specify where to return the results: 'agent', 'ui_only', or 'both'.",
                "enum": ["agent", "ui_only", "both"],
                "required": False
            }
        }
    )
    async def get_thread(self, **kwargs) -> str:
        self.logger.info(f"Fetching email thread with parameters: {kwargs}")
        thread_id = kwargs.get('thread_id')
        return_type = kwargs.get('return_type', 'agent')
        tool_context = kwargs.get('tool_context', {})

        try:
            # Fetch the email thread
            query = self.service.users().threads().get(userId="me", id=thread_id, format='full')
            thread_data = query.execute()

            messages = thread_data.get('messages', [])
            thread_info = {
                "id": thread_data.get('id'),
                "snippet": thread_data.get('snippet', ''),
                "historyId": thread_data.get('historyId', ''),
                "messages": []
            }

            for message in messages:
                message_id = message.get('id')
                message_snippet = message.get('snippet', '')
                # Parse headers
                payload = message.get('payload', {})
                headers = payload.get('headers', [])
                subject = self._get_header(headers, 'Subject')
                sender = self._get_header(headers, 'From')
                body = self._get_body_from_payload(payload, tool_context)

                message_info = {
                    "id": message_id,
                    "snippet": message_snippet,
                    "subject": subject,
                    "sender": sender,
                    "body": body
                }
                thread_info['messages'].append(message_info)

            # Handle the return type
            return await self.handle_return_type(output=thread_info, return_type=return_type, calling_function='get_thread', tool_context=tool_context)

        except Exception as e:
            self.logger.error(f"Error fetching email thread: {e}")
            return await self.handle_return_type(output={}, return_type=return_type, error=e, calling_function='get_thread', tool_context=tool_context)

    async def _prepare_message(
            self,
            message: str,
            to: Union[str, List[str]],
            subject: str,
            cc: Optional[Union[str, List[str]]] = None,
            bcc: Optional[Union[str, List[str]]] = None,
            attachments: Optional[List[str]] = None,
    ) -> Dict[str, Any]:
        """Create a message for an email, optionally including attachments."""
        mime_message = MIMEMultipart()
        mime_message.attach(MIMEText(message, "html"))

        # Handle recipients
        mime_message["To"] = self._format_addresses(to)
        mime_message["Subject"] = subject
        if cc:
            mime_message["Cc"] = self._format_addresses(cc)
        if bcc:
            mime_message["Bcc"] = self._format_addresses(bcc)

        # Handle attachments
        if attachments:
            for filename in attachments:
                # Read the file from the workspace
                error, workspace, relative_path = self.workspace_tool.validate_and_get_workspace_path(filename)
                if error:
                    return {"error": f"Invalid path: {error}"}

                try:
                    file_data = await workspace.read_bytes_internal(filename)
                except Exception as e:
                    # going to go ahead and stop here
                    self.logger.error(f"Error reading attachment {filename}: {e}")
                    # I could just return or continue here
                    # continue

                # Determine the MIME type of the file
                mime_type, _ = mimetypes.guess_type(filename)
                if mime_type is None:
                    mime_type = 'application/octet-stream'
                main_type, sub_type = mime_type.split('/', 1)

                # Create the attachment part
                attachment_part = MIMEBase(main_type, sub_type)
                attachment_part.set_payload(file_data)
                encoders.encode_base64(attachment_part)
                attachment_part.add_header('Content-Disposition', 'attachment', filename=filename)
                mime_message.attach(attachment_part)
                self.logger.info(f"Attached file {filename}")

        encoded_message = base64.urlsafe_b64encode(mime_message.as_bytes()).decode()
        return {"raw": encoded_message}


Toolset.register(GmailMessage, required_tools=['WorkspaceTools'])
