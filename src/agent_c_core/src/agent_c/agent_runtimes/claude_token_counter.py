from anthropic import Anthropic

from agent_c.util import TokenCounter


class ClaudeTokenCounter(TokenCounter):

    def __init__(self):
        self.anthropic: Anthropic = Anthropic()

    def count_tokens(self, text: str) -> int:
        response = self.anthropic.messages.count_tokens(
            model="claude-3-7-sonnet-latest", system="",
            messages=[{"role": "user", "content": text}])

        return response.input_tokens
