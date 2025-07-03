
from jinja2 import BaseLoader, TemplateNotFound


from agent_c.config.agent_config_loader import AgentConfigLoader
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

    def get_template_from_agent_registry(self, env, name: str):
        try:
            agent_key: str = name.removeprefix("agents/")
            want_clone = False
            if agent_key.endswith('_clone'):
                want_clone = True
                agent_key = agent_key.removesuffix('_clone')

            agent_config = AgentConfigLoader.instance().catalog[agent_key]
            if not agent_config:
                raise TemplateNotFound(name)
        except:
            raise TemplateNotFound(name)
        if want_clone and len(agent_config.clones) > 0:
            source = agent_config.clone_instructions
        else:
            source = agent_config.instructions
        mtime = agent_config.load_time
        return source, None, lambda: mtime == agent_config.load_time

    def get_source(self, env, name):
        if name.startswith('agents/'):
            return self.get_template_from_agent_registry(env, name)
        else:
            return self.get_template_from_section_registry(env, name)
