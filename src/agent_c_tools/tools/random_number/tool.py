import logging
import random
import json
from agent_c_core.agent_c.toolsets import Toolset, json_schema


class RandomNumberTools(Toolset):
    """
    A simple toolset that provides random number generation capabilities.
    """

    def __init__(self, **kwargs):
        """
        Initialize the RandomNumberTools.

        Args:
            **kwargs: Keyword arguments passed to parent Toolset class.
        """
        super().__init__(**kwargs, name='random_number_generator')
        self.logger = logging.getLogger(__name__)
        # self.logger.debug(
        #     f"RandomNumberTools initialized with streaming_callback: {self.streaming_callback is not None}")

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
                content="<div>tool chat call back demo</div>"
            )
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
