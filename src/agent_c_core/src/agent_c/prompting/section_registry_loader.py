
from jinja2 import BaseLoader, TemplateNotFound

from agent_c.models.prompts import BasePromptSection
from agent_c.util.registries.section_registry import SectionRegistry

class RegistryLoader(BaseLoader):
    def get_template_from_section_registry(self, env, name):
        try:
            section: BasePromptSection = SectionRegistry.create(name)
            if not section:
                raise TemplateNotFound(name)
        except:
            raise TemplateNotFound(name)

        source = section.template
        mtime = section.load_time
        return source, None, lambda: mtime == section.load_time

    def get_template_from_agent_registry(self, env, name):
        try:
            section: BasePromptSection = SectionRegistry.create(name)
            if not section:
                raise TemplateNotFound(name)
        except:
            raise TemplateNotFound(name)

        source = section.template
        mtime = section.load_time
        return source, None, lambda: mtime == section.load_time

    def get_source(self, env, name):
        if name.startswith('agent_') or name.startswith('clone_'):
            return self.get_template_from_agent_registry(env, name)
        else:
            return self.get_template_from_section_registry(env, name)
