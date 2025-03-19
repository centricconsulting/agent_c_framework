import logging
from openai import AsyncOpenAI
from typing import Callable, Optional, Tuple, Any, Dict

from agent_c import ImageInput


class StructuredOneshot:
    """
    This class handles a structured one-shot extraction of content using an OpenAI completion model.
    It supports passing user messages and optional image content to the model, then processes the model's output
    with potential validation and retry mechanisms.
    """

    def __init__(self, **kwargs: Any) -> None:
        """
        Initializes the StructuredOneshot class.

        Args:
            kwargs:
                output_model (str): The output format model.
                completion_callback (Callable[['StructuredOneshot', Any], None]): Callback function after completion.
                validation_callback (Callable[[int, Any, Any], Tuple[bool, str]]): Validation function with retries.
                model_name (str, optional): The name of the model to use. Defaults to 'gpt-4o-2024-08-06'.
                prompt (str): The prompt string for structured extraction.
                open_ai_client (AsyncOpenAI, optional): Client for OpenAI API calls. Defaults to AsyncOpenAI().
                temperature (float, optional): Temperature setting for the model. Defaults to 0.0.
                max_retries (int, optional): Maximum retry attempts for validation failures. Defaults to 3.
        """
        self.output_model: Optional[str] = kwargs.get('output_model')
        self._completion_callback: Optional[Callable[['StructuredOneshot', Any], None]] = kwargs.get('completion_callback')
        self._validation_callback: Optional[Callable[[int, Any, Any], Tuple[bool, str]]] = kwargs.get('validation_callback')
        self.model_name: str = kwargs.get('model_name', 'gpt-4o-2024-08-06')
        self.prompt: Optional[str] = kwargs.get('prompt')
        self.client: AsyncOpenAI = kwargs.get('open_ai_client', AsyncOpenAI())
        self.temperature: float = kwargs.get('temperature', 0.0)
        self.max_retries: int = kwargs.get('max_retries', 3)

    def completion_callback(self, result: Any) -> None:
        """
        Calls the registered completion callback function after a successful model invocation.

        Args:
            result (Any): The result from the model execution.
        """
        if self._completion_callback is not None:
            self._completion_callback(self, result)

    def validation_callback(self, attempt: int, result: Any, state: Any) -> Tuple[bool, Optional[str]]:
        """
        Calls the registered validation callback function to validate the model's output.

        Args:
            attempt (int): The current attempt number.
            result (Any): The result to validate.
            state (Any): The current validation state.

        Returns:
            Tuple[bool, Optional[str]]: A tuple containing a boolean indicating validation success or failure
                                        and an optional error message.
        """
        if self._validation_callback is not None:
            return self._validation_callback(attempt, result, state)
        return True, None

    async def run(self, user_message: str, image: Optional[ImageInput] = None,
                  response_format: Optional[str] = None, prompt: Optional[str] = None) -> Dict:
        """
        Performs a structured one-shot extraction of content from the user's message and optionally an image.

        Args:
            user_message (str): The input message from the user.
            image (Optional[ImageInput]): The image input that contains the content for analysis.
            response_format (Optional[str]): Desired format for the API response.
            prompt (Optional[str]): Override the default prompt, if specified. Defaults to None.

        Returns:
            Dict: A structured response from the model with parsed content.

        Raises:
            ValueError: If the prompt or response format is not provided.
        """
        # Use provided prompt or the default one
        prompt = prompt or self.prompt
        if prompt is None:
            raise ValueError("Prompt is required for structured image oneshot.")

        response_format = response_format or self.output_model
        if response_format is None:
            raise ValueError("Response format is required for structured image oneshot.")

        # Structure content with both the user message and the image if present
        if image is None:
            messages = [{"role": "system", "content": prompt}, {"role": "user", "text": user_message}]
        else:
            contents = [
                {"type": "text", "text": user_message},
                {"type": "image_url", "image_url": {"url": image.data_url}}
            ]
            messages = [{"role": "system", "content": prompt}, {"role": "user", "content": contents}]

        result = None
        attempt = 0
        state = None

        # Loop for retrying in case of validation failures
        while attempt < self.max_retries:
            attempt += 1

            # Make the API call for model completion
            completion = await self.client.beta.chat.completions.parse(
                model=self.model_name,
                temperature=self.temperature,
                messages=messages,
                response_format=response_format,
            )

            # Parse the response from the model
            agent_message = completion.choices[0].message
            result = agent_message.parsed

            # Validate the model response
            valid, val_error = self.validation_callback(attempt, result, state)
            if valid:
                break

            logging.warning(f"Validation failed on attempt {attempt} of {self.max_retries}. Validation message:\n{val_error}")
            val_message = (
                f"Validation failed on attempt {attempt} of {self.max_retries}.\n\nValidation message:\n{val_error}"
            )

            # Add validation error message to the conversation
            messages.append({"role": "assistant", "content": agent_message.content})
            messages.append({"role": "user", "content": val_message})

        # Call completion callback and return the result
        self.completion_callback(result)
        return result
