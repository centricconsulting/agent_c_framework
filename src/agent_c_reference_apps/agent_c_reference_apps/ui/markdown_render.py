import re
from queue import Queue
from typing import Union, List
from rich.console import Console
from rich.markdown import Markdown

MARKDOWN_CODE_BLOCK: str = "```"
CODE_BLOCK_REGEX: str = r"(.*?)(```.*?```)(.*)"


class MarkdownTokenRenderer:
    def __init__(self, console: Console):
        self.rich_console: Console = console
        self.pending_codeblock: Union[str, None] = None
        self.prior_token: str = ''
        self.chat_message_buffer: List[str] = []
        self.current_line: str = ''

    def render_token(self, content) -> Union[str, None]:
        if '`' in content:
            # Start forming code blocks upon detecting backticks.
            self.__handle_backticks(content)
        elif self.pending_codeblock:
            # If inside a code block, append all tokens to the pending code block.
            self.pending_codeblock += content
        else:
            # If the last token had a backtick that didn't result in a code block and
            # this token doesn't have a backtick then we need to remove the prior token
            # from the sliding window.
            ret = content
            if self.prior_token != "":
                ret = self.prior_token + content
                self.__handle_chat_token(self.prior_token)
                self.prior_token = ""

            # Output other tokens line-by-line to ensure proper Markdown rendering.
            self.__handle_chat_token(content)
            return ret

    def flush(self):
        if self.current_line != "":
            self.rich_console.print(Markdown(self.current_line), end="", soft_wrap=True)

            self.current_line = ""

    def __handle_chat_token(self, content):
        if self.rich_console is None:
            return

        self.current_line += content

        if "\n" in self.current_line:
            parts = self.current_line.split("\n")
            for part in parts[:-1]:
                if part != "":
                    self.rich_console.print(Markdown(part), end="", soft_wrap=True)

            if parts[-1] != "":
                self.current_line = parts[-1]
            else:
                self.current_line = ""

    def __handle_code_block_parts(self, content: str):
        """
        Adds tokens to the pending code block until a complete code block is detected.

        Args:
            content (str): The content that might contain a code block.
        """
        if self.pending_codeblock:
            self.pending_codeblock.append(content)
        else:
            self.pending_codeblock = [content]

        result = self.__extract_code_block_from_pending()
        if result:
            self.pending_codeblock = None
            left_hand_side, code_block, right_hand_side = result

            if left_hand_side:
                self.__handle_chat_token(left_hand_side)

            markdown = Markdown(code_block)
            if self.rich_console is not None:
                self.rich_console.print(markdown, end="", soft_wrap=True)

            if right_hand_side:
                self.__handle_chat_token(right_hand_side)

    def __handle_backticks(self, content: str):
        """
        Handles processing of backticks for markdown code blocks.

        Args:
            content (str): The content to check for the presence of backticks.
        """
        # If the content is exactly one code block delimiter, handle a potential code block.
        if content == MARKDOWN_CODE_BLOCK:
            self.__handle_code_block_parts(content)
        else:
            # Check if the current and prior tokens might start or end a code block.
            last_two = self.prior_token + content
            if MARKDOWN_CODE_BLOCK in last_two:
                # Either start or complete a pending code block.
                self.__handle_code_block_parts(last_two)
                self.prior_token = ""  # Reset the prior token.
            else:
                if self.prior_token != "":
                    # The prior token could be part of the text_iter outside a code block.
                    self.__handle_chat_token(self.prior_token)

                if content.endswith("`"):
                    self.prior_token = content  # Save current token as prior for next pass since it might make a code block.
                else:
                    self.__handle_chat_token(content)
                    self.prior_token = ""

    def __extract_code_block_from_pending(self) -> Union[tuple, None]:
        """
        Takes all the tokens gathered as part of a code block and returns them as three strings in a tuple.

        Returns:
            tuple: A tuple containing:
                   - Content to the left/above of the code block, if any (str or None).
                   - The code block itself (str).
                   - Content to the right/below of the code block, if any (str or None).
            None: If no complete code block is found.

        Note:
            Even if the sides are empty a tuple is returned to maintain a uniform interface. If there's no match,
            it means there's no complete code block found, and None is returned.
        """
        match = re.match(CODE_BLOCK_REGEX, "".join(self.pending_codeblock), re.DOTALL)

        if match:
            left_hand_side = match.group(1) or None
            code_block = match.group(2)
            right_hand_side = match.group(3) or None
            return left_hand_side, code_block, right_hand_side
        return None
