import time
import uuid
from typing import Callable

from fastapi import Request, Response
from starlette.middleware.base import BaseHTTPMiddleware
from starlette.types import ASGIApp

from agent_c_api.core.util.logging_utils import LoggingManager

logging_manager = LoggingManager(__name__)
logger = logging_manager.get_logger()


class APILoggingMiddleware(BaseHTTPMiddleware):
    """
    Middleware to log all API requests and responses.

    This middleware intercepts all HTTP requests and logs their details including:
    - Request method and path
    - Response status code
    - Processing time
    - Request headers and body (optional)
    - Response data (optional)

    It can be configured to log at different levels based on status codes.
    """

    def __init__(
            self,
            app: ASGIApp,
            log_request_body: bool = False,
            log_response_body: bool = False
    ):
        """
        Initialize the logging middleware.

        Args:
            app: The ASGI application
            log_request_body: Whether to log request bodies
            log_response_body: Whether to log response bodies
        """
        super().__init__(app)
        self.log_request_body = log_request_body
        self.log_response_body = log_response_body
        logger.info(f"API Logging Middleware initialized with log_request_body={log_request_body}")

    async def dispatch(
            self, request: Request, call_next: Callable
    ) -> Response:
        """
        Process the request, log details, and pass to the next middleware.

        Args:
            request: The incoming HTTP request
            call_next: Function that calls the next middleware/route handler

        Returns:
            The HTTP response
        """
        # Generate unique ID for this request
        request_id = str(uuid.uuid4())[:8]  # Use a shorter ID for readability

        # Log the request
        start_time = time.time()
        logger.info(
            f"[{request_id}] Request started: {request.method} {request.url.path}"
        )

        # Optional: Log request headers (but exclude sensitive ones)
        if logger.level <= 10:  # DEBUG level
            headers = dict(request.headers)
            if "authorization" in headers:
                headers["authorization"] = "[REDACTED]"
            if "cookie" in headers:
                headers["cookie"] = "[REDACTED]"
            logger.debug(f"[{request_id}] Request headers: {headers}")

        # Optional: Log request body
        if self.log_request_body:
            try:
                # Try to read the body without consuming it
                body = await request.body()

                # Try to decode as text, but handle binary data
                try:
                    body_text = body.decode("utf-8")
                    # Truncate long bodies
                    if len(body_text) > 500:
                        body_text = body_text[:500] + "... [truncated]"
                    logger.debug(f"[{request_id}] Request body: {body_text}")
                except UnicodeDecodeError:
                    logger.debug(f"[{request_id}] Request body: [binary data, {len(body)} bytes]")

                # Recreate the request since we've consumed the body
                async def receive():
                    return {"type": "http.request", "body": body}

                request = Request(request.scope, receive, request._send)

            except Exception as e:
                logger.warning(f"[{request_id}] Could not log request body: {str(e)}")

        # Process the request
        try:
            response = await call_next(request)

            # Calculate and log processing time
            process_time = time.time() - start_time

            # Choose log level based on status code
            if response.status_code < 400:
                logger.info(
                    f"[{request_id}] Request completed: {request.method} {request.url.path} "
                    f"- Status: {response.status_code} - Time: {process_time:.3f}s"
                )
            elif response.status_code < 500:
                logger.warning(
                    f"[{request_id}] Client error: {request.method} {request.url.path} "
                    f"- Status: {response.status_code} - Time: {process_time:.3f}s"
                )
            else:
                logger.error(
                    f"[{request_id}] Server error: {request.method} {request.url.path} "
                    f"- Status: {response.status_code} - Time: {process_time:.3f}s"
                )

            return response

        except Exception as e:
            # Log unhandled exceptions
            process_time = time.time() - start_time
            logger.error(
                f"[{request_id}] Request failed: {request.method} {request.url.path} "
                f"- Error: {str(e)} - Time: {process_time:.3f}s",
                exc_info=True
            )
            raise