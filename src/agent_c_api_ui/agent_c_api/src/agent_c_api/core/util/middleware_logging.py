import time
import uuid
from typing import Callable

from fastapi import Request, Response
from starlette.middleware.base import BaseHTTPMiddleware
from starlette.types import ASGIApp

from agent_c.util.structured_logging import get_logger, LoggingContext

logger = get_logger(__name__)


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
        logger.info("API Logging Middleware initialized", 
                    log_request_body=log_request_body, 
                    log_response_body=log_response_body)

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

        # Log the request with structured context
        start_time = time.time()

        logger.info("Request started",
                   method=request.method,
                   path=str(request.url.path),
                   url=str(request.url))

        # Optional: Log request headers (but exclude sensitive ones)
        if logger.level <= 10:  # DEBUG level
            headers = dict(request.headers)
            if "authorization" in headers:
                headers["authorization"] = "[REDACTED]"
            if "cookie" in headers:
                headers["cookie"] = "[REDACTED]"

                logger.debug("Request headers", headers=headers,request_id=request_id)

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

                        logger.debug("Request body",
                                     request_id=request_id,
                                    body=body_text, 
                                    body_size=len(body),
                                    truncated=len(body_text) != len(body.decode("utf-8")))
                except UnicodeDecodeError:

                        logger.debug("Request body (binary)", 
                                    body_type="binary", 
                                    body_size=len(body))

                # Recreate the request since we've consumed the body
                async def receive():
                    return {"type": "http.request", "body": body}

                request = Request(request.scope, receive, request._send)

            except Exception as e:

                logger.warning("Could not log request body",
                                request_id=request_id,
                                error=str(e),
                                exc_info=True)

        # Process the request
        try:
            response = await call_next(request)

            # Calculate and log processing time
            process_time = time.time() - start_time

            # Choose log level based on status code with structured logging

            common_fields = {
                "method": request.method,
                "path": str(request.url.path),
                "status_code": response.status_code,
                "process_time_seconds": round(process_time, 3)
            }

            if response.status_code < 400:
                logger.info("Request completed successfully", **common_fields)
            elif response.status_code < 500:
                logger.warning("Client error occurred", **common_fields)
            else:
                logger.error("Server error occurred", **common_fields)

            return response

        except Exception as e:
            # Log unhandled exceptions with structured data
            process_time = time.time() - start_time

            logger.error("Request failed with unhandled exception",
                        request_id=request_id,
                       method=request.method,
                       path=str(request.url.path),
                       error=str(e),
                       process_time_seconds=round(process_time, 3),
                       exc_info=True)
            raise