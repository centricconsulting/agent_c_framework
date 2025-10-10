from typing import List

from agent_c.models.config import BaseCoreConfig
from pydantic import Field


default_jinja_extensions: List[str] = ['jinja2.ext.i18n',
                                        'jinja2.ext.debug',
                                        'jinja2.ext.loopcontrols',
                                        'jinja2.ext.do'
                                       ]
class PromptRenderingConfig(BaseCoreConfig):
    """
    Configuration for Jinja2 templating engine.
    """
    jinja2_auto_reload: bool = Field(default=True,
                                     description="Enable auto-reloading of Jinja2 templates. "
                                                  "If True, templates will be reloaded on each request if changed.")
    jinja2_cache_size: int = Field(default=500, description="Maximum number of templates to cache in Jinja2.")

    jinja2_extensions: list[str] = Field(default=default_jinja_extensions,
                                         description="List of Jinja2 extensions to enable. "
                                                    "These extensions provide additional functionality to Jinja2 templates.")
    jinja2_auto_escape: bool = Field(default=False,
                                     description="Enable auto-escaping of Jinja2 templates. "
                                                 "If True, Jinja2 will automatically escape HTML and XML characters in templates.")
    jinja2_trim_blocks: bool = Field(default=True,
                                    description="Enable trimming of whitespace in Jinja2 blocks. "
                                                "If True, Jinja2 will remove leading and trailing whitespace in blocks.")
    jinja2_lstrip_blocks: bool = Field(default=True,
                                        description="Enable left-stripping of whitespace in Jinja2 blocks. "
                                                    "If True, Jinja2 will remove leading whitespace in blocks.")
    jinja2_keep_trailing_newline: bool = Field(default=True,
                                                description="Keep trailing newlines in Jinja2 templates. "
                                                            "If True, Jinja2 will preserve trailing newlines in templates.")
    jinja2_newstyle_gettext: bool = Field(default=True,
                                            description="Use new-style gettext in Jinja2 templates. "
                                                        "If True, Jinja2 will use the new-style gettext for translations.")

