import logging
import random
import json
from agent_c.toolsets import Toolset, json_schema


class RandomNumberTools(Toolset):
    """
    Provides your agent with random number generation capabilities for games, simulations, sampling,
    and any situation where you need unpredictable values. Your agent can generate random numbers
    within specified ranges and use seeds for reproducible results.
    """

    def __init__(self, **kwargs):
        """
        Initialize the RandomNumberTools.

        Args:
            **kwargs: Keyword arguments passed to parent Toolset class.
        """
        super().__init__(**kwargs, name='rng')


    @json_schema(
        description='Generate a random number (inclusive)',
        params={
            'min': {
                'type': 'integer',
                'description': 'Minimum number to generate, default is 0',
                'required': False,
                'default': 0
            },
            'max': {
                'type': 'integer',
                'description': 'Maximum number to generate, default is 100',
                'required': False,
                'default': 100
            },
            'seed': {
                'type': 'integer',
                'description': 'Optional seed for random number generation',
                'required': False
            }
        }
    )
    async def generate_random_number(self, **kwargs) -> str:
        """
        Generate a random number between 0 and 100.

        Args:
            seed (optional): Seed for random number generation

        Returns:
            str: JSON string containing the generated number or error message
        """
        try:
            # get tool context
            tool_context = kwargs.get('tool_context', {})
            # Set seed if provided
            seed = kwargs.get('seed')
            min_level = kwargs.get('min', 0)
            max_ceiling = kwargs.get('max', 100)

            if seed is not None:
                random.seed(seed)
                self.logger.debug(f"Random seed set to {seed}")

            # Generate random number
            number = random.randint(min_level, max_ceiling)
            self.logger.info(f"Generated random number between {min_level} and {max_ceiling}: {number}")
            # self.logger.debug(
            #     f"About to call chat_callback, streaming_callback exists: {self.streaming_callback is not None}")
            await self._raise_render_media(
                sent_by_class=self.__class__.__name__,
                sent_by_function='generate_random_number',
                content_type="text/html",
                content=f"<div>Example Raise Media Event via tool context: Number is <b>{number}</b></div>",
                tool_context=tool_context
            )

            bridge = tool_context.get('bridge')
            message = f":::IMPORTANT\n\n- *Number Generated* {number} using bridge render event\n:::"
            await bridge.raise_render_media_markdown(message, "RandomNumberTools")
            return json.dumps({
                "number": number,
                "message": "Random number generated successfully"
            })

        except Exception as e:
            error_msg = f"Error generating random number: {str(e)}"
            self.logger.error(error_msg)
            return json.dumps({
                "error": error_msg
            })


# Register the tool
Toolset.register(RandomNumberTools)
