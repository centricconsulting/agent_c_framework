"""
PLANE Authentication Module

Handles cookie-based session management for PLANE API access.
"""

from .cookie_manager import PlaneCookieManager
from .plane_session import PlaneSession, PlaneSessionExpired, PlaneAPIError

__all__ = ['PlaneCookieManager', 'PlaneSession', 'PlaneSessionExpired', 'PlaneAPIError']
