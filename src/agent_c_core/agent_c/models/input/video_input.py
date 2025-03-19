import os
import base64
import logging
import tempfile
from typing import Union, Optional, Any

from agent_c_core.agent_c.models.input.file_input import FileInput


class VideoInput(FileInput):
    """
    A model class for handling image input with various formats and conversion methods.
    """
