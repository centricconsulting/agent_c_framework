import sys
import os

def debugger_is_active() -> bool:
    """
    Determines if a debugger is currently attached.

    This function checks for the existence of the `gettrace` attribute in the
    `sys` module, and verifies if a trace function is installed (i.e., if
    debugging mode is active).

    Returns:
        bool: True if a debugger is active, False otherwise.
    """
    # Check for PyCharm debug mode
    if os.getenv('PYCHARM_HOSTED') == '1' and os.getenv('PYTHONUNBUFFERED') == '1':
        return True

        # Check for presence of settrace function (Python's default debugger)
    gettrace = getattr(sys, 'gettrace', lambda: None)
    if gettrace() is not None:
        return True

    # Check for specific debugger modules
    debugger_modules = ['pydevd', 'pdb']
    debuggers = any(mod in sys.modules for mod in debugger_modules)
    if debuggers:
        return True