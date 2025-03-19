from typing import Optional
from datetime import datetime, timezone
from email.utils import parsedate_to_datetime


def expires_header_to_cache_seconds(expires_header: Optional[str]) -> Optional[int]:
    """
    Convert an HTTP 'Expires' header value to the number of seconds until expiration.

    Args:
        expires_header (Optional[str]): The value of the 'Expires' header.

    Returns:
        Optional[int]: Number of seconds until expiration, or None if the header is missing or invalid.
    """
    if not expires_header:
        # No 'Expires' header present
        return None

    try:
        # Parse the Expires header into a datetime object
        expires_dt = parsedate_to_datetime(expires_header)

        # Ensure the datetime is timezone-aware in UTC
        if expires_dt.tzinfo is None:
            expires_dt = expires_dt.replace(tzinfo=timezone.utc)
        else:
            expires_dt = expires_dt.astimezone(timezone.utc)

        # Get the current UTC time
        now = datetime.now(timezone.utc)

        # Calculate the difference in seconds
        delta_seconds = int((expires_dt - now).total_seconds())

        # If the expiration time is in the past, return 0
        return max(delta_seconds, 0)

    except (TypeError, ValueError, OverflowError) as e:
        # Failed to parse the Expires header
        return None