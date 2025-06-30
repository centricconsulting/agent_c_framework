#!/usr/bin/env python3

import sys
import os

# Add the project path to sys.path if needed
project_path = os.path.dirname(os.path.abspath(__file__))
if project_path not in sys.path:
    sys.path.insert(0, project_path)

exec(open('debug_json_schema.py').read())