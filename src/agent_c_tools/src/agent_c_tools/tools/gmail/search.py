from typing import Dict, Any, List
import json
import base64
import email
import re
import html
from bs4 import BeautifulSoup

from agent_c.toolsets import json_schema, Toolset
from .base import GmailBase, GmailError, GmailResourceType

class GmailSearch(GmailBase):
    """Tool for searching Gmail messages and threads."""

    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='gmail_search')
        self.logger.info("Initialized Gmail Search Tool")

    @json_schema(
        description='Search Gmail messages or threads using Gmail query syntax',
        params={
            'query': {
                'type': 'string',
                'description': 'The Gmail query. Example filters include from:sender, '
                               'to:recipient, subject:subject, -filtered_term, '
                               'in:folder, is:important|read|starred, after:year/mo/date, '
                               'before:year/mo/date, label:label_name, '
                               '"exact phrase". '
                               'Search newer/older than using d (day), m (month), and y (year): '
                               'newer_than:2d, older_than:1y. '
                               'Attachments with extension example: filename:pdf. '
                               'Multiple term matching example: from:amy OR from:david.'
                               'Ensure names are in quotes if they contain spaces, example.'
                               '**User**: find emails from Rachel Alley regarding a house on Goodman'
                               '**Tool**: uses tool gmail_search with query of from:"Rachel Alley" Goodman',
                'required': True
            },
            'resource': {
                'type': 'string',
                'description': 'Whether to search for threads or messages.',
                'enum': ['threads', 'messages'],
                'default': 'messages',
                'required': False
            },
            'max_results': {
                'type': 'integer',
                'description': 'Maximum number of results to return.',
                'default': 10,
                'required': False
            },
            'return_type': {
                'type': 'string',
                'description': 'Return the output of the tool call directly instead of back to the agent',
                'enum': ['both', 'agent', 'ui_only'],
                'required': False,
                'default': 'agent'
            }
        })
    async def search_gmail(self, **kwargs) -> str:
        query = kwargs.get('query')
        resource = kwargs.get('resource', 'messages')
        max_results = kwargs.get('max_results', 10)
        return_type = kwargs.get('return_type', 'agent')
        tool_context = kwargs.get('tool_context', {})

        try:
            # Create cache key
            cache_key = f"gmail_search_{query}_{resource}_{max_results}"
            cached_results = self.tool_cache.get(cache_key)

            if cached_results:
                self.logger.debug(f'Found cached search results for query: {query}')
                results = json.loads(cached_results)
            else:
                if resource == GmailResourceType.MESSAGES:
                    results = await self._search_messages(query, max_results, tool_context)
                else:
                    results = await self._search_threads(query, max_results)

                self.tool_cache.set(cache_key, json.dumps(results), expire=self.cache_expire)

            # Create UI message
            ui_message = self._format_search_results_for_ui(results, query, resource)

            return await self.handle_return_type(
                output=results,
                return_type=return_type,
                ui_message=ui_message,
                calling_function='search_gmail',
                tool_context=tool_context
            )

        except Exception as e:
            self.logger.error(f'Error in search_gmail: {str(e)}')
            return await self.handle_return_type(
                output=None,
                return_type=return_type,
                error=e,
                calling_function='search_gmail',
                tool_context=tool_context
            )

    async def _search_messages(self, query: str, max_results: int, tool_context) -> List[Dict[str, Any]]:
        """Search for messages and get their details."""
        try:
            messages = (
                self.service.users()
                .messages()
                .list(userId="me", q=query, maxResults=max_results)
                .execute()
                .get('messages', [])
            )

            results = []
            for message in messages:
                message_id = message['id']
                message_data = (
                    self.service.users()
                    .messages()
                    .get(userId="me", format="metadata", id=message_id,
                         metadataHeaders=["Subject", "From", "To", "Date"])
                    .execute()
                )

                # Process headers
                headers = {h['name']: h['value'] for h in message_data.get('payload', {}).get('headers', [])}

                # Create basic result
                result = {
                    "id": message["id"],
                    "threadId": message_data["threadId"],
                    "snippet": message_data["snippet"],
                    "subject": headers.get("Subject", ""),
                    "sender": headers.get("From", ""),
                    "recipient": headers.get("To", ""),
                    "date": headers.get("Date", "")
                }

                try:
                    full_message = (
                        self.service.users()
                        .messages()
                        .get(userId="me", format="raw", id=message_id)
                        .execute()
                    )

                    raw_message = base64.urlsafe_b64decode(full_message["raw"])
                    email_msg = email.message_from_bytes(raw_message)

                    # Process body with our smart handling
                    result["body"] = await self.process_email_content(email_msg=email_msg, max_tokens=self.MAX_EMAIL_BODY_TOKEN_SIZE, tool_context=tool_context)
                except Exception as e:
                    self.logger.error(f"Error processing email body: {e}")
                    result["body"] = f"[Error processing email content: {str(e)}]"

                results.append(result)

            return results

        except Exception as e:
            raise GmailError(f"Error searching messages: {str(e)}")

    async def _search_threads(self, query: str, max_results: int) -> List[Dict[str, Any]]:
        """Search for threads and get their details."""
        try:
            threads = (
                self.service.users()
                .threads()
                .list(userId="me", q=query, maxResults=max_results)
                .execute()
                .get('threads', [])
            )

            results = []
            for thread in threads:
                thread_id = thread["id"]
                thread_data = (
                    self.service.users()
                    .threads()
                    .get(userId="me", id=thread_id)
                    .execute()
                )

                # Extract thread details
                messages = []
                for message in thread_data["messages"]:
                    messages.append({
                        "id": message["id"],
                        "snippet": message["snippet"]
                    })

                results.append({
                    "id": thread["id"],
                    "messages": messages,
                    "message_count": len(messages)
                })

            return results

        except Exception as e:
            raise GmailError(f"Error searching threads: {str(e)}")

    def _get_email_body(self, email_msg: email.message.Message) -> str:
        """Extract and clean email body text."""
        body = ""
        if email_msg.is_multipart():
            for part in email_msg.walk():
                ctype = part.get_content_type()
                cdispo = str(part.get("Content-Disposition"))

                # Skip attachments and non-text content to save some tokens
                if not ctype.startswith("text/") or "attachment" in cdispo:
                    continue

                if ctype == "text/plain" and "attachment" not in cdispo:
                    try:
                        body = part.get_payload(decode=True).decode("utf-8")
                    except UnicodeDecodeError:
                        body = part.get_payload(decode=True).decode("latin-1")
                    break
        else:
            try:
                body = email_msg.get_payload(decode=True).decode("utf-8")
            except UnicodeDecodeError:
                body = email_msg.get_payload(decode=True).decode("latin-1")

        return self.clean_email_body(body)

    def _format_search_results_for_ui(
            self,
            results: List[Dict[str, Any]],
            query: str,
            resource: str
    ) -> str:
        """Format search results for UI display."""
        if resource == GmailResourceType.MESSAGES:
            message = f"Found {len(results)} messages matching query '{query}':<br><br>"
            for msg in results:
                message += (
                    f"<b>From:</b> {msg['sender']}<br>"
                    f"<b>Subject:</b> {msg['subject']}<br>"
                    f"<b>Date:</b> {msg['date']}<br>"
                    f"<b>Snippet:</b> {msg['snippet']}<br><br>"
                )
        else:
            message = f"Found {len(results)} threads matching query '{query}':<br><br>"
            for thread in results:
                message += (
                    f"<b>Thread ID:</b> {thread['id']}<br>"
                    f"<b>Messages:</b> {thread['message_count']}<br>"
                    f"<b>Latest snippet:</b> {thread['messages'][0]['snippet']}<br><br>"
                )

        return message


    async def process_email_content(self, email_msg: email.message.Message, tool_context, max_tokens: int = 2000) -> str:
        """
        Extract and prioritize email content, handling large emails intelligently.
        Returns plain text with important parts prioritized.
        """
        # First attempt to get plain text version (preferred)
        plain_text = await self._extract_plain_text(email_msg)

        if not plain_text or self._count_tokens(plain_text, tool_context) > max_tokens:
            html_content = await self._extract_html(email_msg)
            if html_content:
                plain_text = await self._html_to_plain_text(html_content)

        # If we have content to process
        if plain_text:
            # Clean and prioritize content
            cleaned_text = await self._clean_email_text(plain_text)

            # Check if it's still too large
            if self._count_tokens(cleaned_text, tool_context) > max_tokens:
                return await self._prioritize_content(cleaned_text, max_tokens, tool_context)
            return cleaned_text

        # Fallback
        return "[Email content could not be processed]"


    async def _extract_plain_text(self, email_msg: email.message.Message) -> str:
        """Extract plain text content from email."""
        if email_msg.is_multipart():
            for part in email_msg.walk():
                content_type = part.get_content_type()
                content_disposition = str(part.get("Content-Disposition", ""))

                if content_type == "text/plain" and "attachment" not in content_disposition:
                    try:
                        return part.get_payload(decode=True).decode("utf-8", errors="replace")
                    except Exception as e:
                        try:
                            return part.get_payload(decode=True).decode("latin-1", errors="replace")
                        except Exception as e:
                            self.logger.warning(f"Failed to decode plain text: {e}")
        else:
            if email_msg.get_content_type() == "text/plain":
                try:
                    return email_msg.get_payload(decode=True).decode("utf-8", errors="replace")
                except Exception as e:
                    try:
                        return email_msg.get_payload(decode=True).decode("latin-1", errors="replace")
                    except Exception as e:
                        self.logger.warning(f"Failed to decode plain text: {e}")

        return ""


    async def _extract_html(self, email_msg: email.message.Message) -> str:
        """Extract HTML content from email."""
        if email_msg.is_multipart():
            for part in email_msg.walk():
                content_type = part.get_content_type()
                content_disposition = str(part.get("Content-Disposition", ""))

                if content_type == "text/html" and "attachment" not in content_disposition:
                    try:
                        return part.get_payload(decode=True).decode("utf-8", errors="replace")
                    except Exception as e:
                        try:
                            return part.get_payload(decode=True).decode("latin-1", errors="replace")
                        except Exception as e:
                            self.logger.warning(f"Failed to decode HTML: {e}")
        else:
            if email_msg.get_content_type() == "text/html":
                try:
                    return email_msg.get_payload(decode=True).decode("utf-8", errors="replace")
                except Exception as e:
                    try:
                        return email_msg.get_payload(decode=True).decode("latin-1", errors="replace")
                    except Exception as e:
                        self.logger.warning(f"Failed to decode HTML: {e}")

        return ""


    async def _html_to_plain_text(self, html_content: str) -> str:
        """Convert HTML to plain text."""
        try:
            # Use BeautifulSoup to parse HTML
            soup = BeautifulSoup(html_content, 'html.parser')

            # Remove scripts, styles, and hidden elements
            for element in soup(["script", "style", "head", "title", "meta", "[document]"]):
                element.extract()

            # Get text and clean it
            text = soup.get_text(separator='\n')

            # Clean up whitespace
            lines = (line.strip() for line in text.splitlines())
            chunks = (phrase.strip() for line in lines for phrase in line.split("  "))
            text = '\n'.join(chunk for chunk in chunks if chunk)

            return text
        except Exception as e:
            self.logger.warning(f"HTML parsing error: {e}")
            # Fallback: basic HTML tag removal
            text = re.sub(r'<[^>]+>', ' ', html_content)
            return html.unescape(text)

    @staticmethod
    async def _clean_email_text(text: str) -> str:
        """Clean up common email artifacts."""
        # Remove forwarded message markers
        text = re.sub(r'---+ ?Forwarded message ?---+.*?(?=\n\n|\Z)', '', text, flags=re.DOTALL)

        # Remove email signature (common patterns)
        text = re.sub(r'\n--\s*\n.*', '', text, flags=re.DOTALL)  # Standard signature delimiter

        # Remove quoted text (lines starting with >)
        text = re.sub(r'(?m)^>.*$', '', text)

        # Remove legal disclaimers (common in corporate emails)
        text = re.sub(r'CONFIDENTIALITY NOTICE:.*', '', text, flags=re.IGNORECASE | re.DOTALL)
        text = re.sub(r'DISCLAIMER:.*', '', text, flags=re.IGNORECASE | re.DOTALL)

        # Normalize newlines
        text = re.sub(r'\n{3,}', '\n\n', text)

        return text.strip()


    async def _prioritize_content(self, text: str, max_tokens: int, tool_context) -> str:
        """
        Intelligently extract the most important parts of an email to fit within token limit.
        Prioritizes the beginning of the email with smart summarization.
        """
        # Split into paragraphs
        paragraphs = text.split('\n\n')

        # Always include the first paragraph (usually most important)
        result = paragraphs[0] + '\n\n'
        token_count = self._count_tokens(result, tool_context)

        # Try to include the first few paragraphs
        for i in range(1, min(3, len(paragraphs))):
            next_para = paragraphs[i] + '\n\n'
            next_tokens = self._count_tokens(next_para, tool_context)

            if token_count + next_tokens < max_tokens * 0.6:  # Leave room for middle/end selection
                result += next_para
                token_count += next_tokens
            else:
                break

        # Try to include some context from the middle (if it's a long email)
        if len(paragraphs) > 5 and token_count < max_tokens * 0.7:
            middle_idx = len(paragraphs) // 2
            middle_para = paragraphs[middle_idx] + '\n\n'
            middle_tokens = self._count_tokens(middle_para, tool_context)

            if token_count + middle_tokens < max_tokens * 0.8:
                result += "[...]\n\n" + middle_para
                token_count += middle_tokens + 10  # Approximate tokens for "[...]"

        # Try to include the last paragraph (often contains action items or conclusions)
        if len(paragraphs) > 1 and token_count < max_tokens * 0.9:
            last_para = paragraphs[-1]
            last_tokens = self._count_tokens(last_para, tool_context)

            if token_count + last_tokens < max_tokens:
                result += "[...]\n\n" + last_para
            else:
                # If last paragraph is too large, include as much as possible
                chars_per_token = len(last_para) / last_tokens
                chars_available = (max_tokens - token_count - 10) * chars_per_token  # Approximate

                if chars_available > 100:  # Only if we can include something meaningful
                    truncated_last = last_para[:int(chars_available)]
                    # Try to end at a sentence
                    last_period = max(truncated_last.rfind('.'), truncated_last.rfind('!'), truncated_last.rfind('?'))
                    if last_period > len(truncated_last) * 0.7:
                        truncated_last = truncated_last[:last_period + 1]

                    result += "[...]\n\n" + truncated_last

        # Add indicator that content was truncated
        if token_count >= max_tokens * 0.5 or len(paragraphs) > 3:
            result += "\n\n[Email was truncated due to size. Showing key portions only.]"

        return result

Toolset.register(GmailSearch, required_tools=['WorkspaceTools'])