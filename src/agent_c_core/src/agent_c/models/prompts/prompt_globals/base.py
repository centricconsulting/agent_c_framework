import asyncio
import inspect
from typing import Callable, Any, Optional, TYPE_CHECKING

from jinja2 import pass_context
from jinja2.utils import _PassArg
from pydantic import model_validator, Field, computed_field

from agent_c.models import BaseDynamicContext
from agent_c.registration import get_system_config
from agent_c.util import to_snake_case
from agent_c.models.completion import CompletionParams

if TYPE_CHECKING:
    from agent_c.models.config.system_config import SystemConfigFile
    from agent_c.models.chat.chat_session import ChatSession
    from agent_c.models.completion.agent_config import CurrentAgentConfiguration



class BasePromptGlobals(BaseDynamicContext):
    add_as_object: bool = Field(True)
    _system_config: Optional['SystemConfigFile'] = None

    @staticmethod
    @pass_context
    def _chat_session(ctx) -> 'ChatSession':
        return ctx['chat_session']

    @classmethod
    @pass_context
    def _agent_config(cls, ctx) -> 'CurrentAgentConfiguration':
        """
        Returns the current agent configuration for the chat session.
        This is used to access agent-specific settings and configurations.
        """
        return cls._chat_session(ctx).agent_config

    @classmethod
    @pass_context
    def _runtime_params(cls, ctx) -> CompletionParams:
        """
        Returns the runtime parameters for the current chat session.
        This is used to access parameters that may change during the session.
        """
        return cls._agent_config(ctx).runtime_params

    @classmethod
    def _call_async(cls, coro):
        """
        Helper method to call an async coroutine in a synchronous context.
        This is useful for methods that need to be called from Jinja2 templates.
        """
        loop = asyncio.get_event_loop()
        return loop.run_until_complete(coro)

    @computed_field
    @property
    def system_config(self) -> Any:
        """
        Returns the system configuration for Agent C.
        This is used to access system-wide settings and configurations.
        """
        if self._system_config is None:
            self._system_config = get_system_config()
        return self._system_config


    @model_validator(mode='before')
    @classmethod
    def ensure_context_type(cls, values):
        """
        Ensure the context type is set to the snake case class name and ends in globals
        """
        context_type = values.get('context_type', None)
        if not context_type:
            context_type = to_snake_case(cls.__name__.lower())

        if not context_type.endswith('_globals'):
            context_type += '_globals'

        values['context_type']  = context_type
        return context_type

    @property
    def globals(self) -> dict[str, Callable[..., Any]]:
        """
        Scan this instance for any methods decorated with @pass_context
        and return a dict of {name: bound_method} ready to merge into
        your Jinja2 environment globals.
        """
        funcs: dict[str, Callable[..., Any]] = {}
        for name, method in inspect.getmembers(self, predicate=inspect.ismethod):
            # _PassArg.from_obj looks for the .jinja_pass_arg attribute
            if _PassArg.from_obj(method) == _PassArg.context:
                funcs[name] = method

        return funcs



