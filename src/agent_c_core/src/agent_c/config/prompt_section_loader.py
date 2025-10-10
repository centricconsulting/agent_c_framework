from pathlib import Path
from typing import TypeVar, Optional, List, Type

import yaml

from agent_c.util.logging_utils import LoggingManager
from agent_c.util import SingletonCacheMeta
from agent_c.config import ConfigLoader
from agent_c.models.prompts import BasePromptSection
from agent_c.util.registries.section_registry import SectionRegistry

T = TypeVar('T', bound=BasePromptSection)

_singleton_instance = None

class PromptSectionLoader(ConfigLoader, metaclass=SingletonCacheMeta):

    def __init__(self, config_path: Optional[str] = None):
        super().__init__(config_path)
        global _singleton_instance
        if _singleton_instance is None:
            _singleton_instance = self

        logging_manager = LoggingManager(__name__)
        self.logger = logging_manager.get_logger()
        self.sections_folder = Path(self.config_path).joinpath("sections")
        self.registry: Type[SectionRegistry] = SectionRegistry

    @classmethod
    def mock(cls, mock_instance):
        """
        Mock the AgentConfigLoader instance for testing purposes.

        Args:
            mock_instance: The mock instance to use for testing
        """
        global _singleton_instance
        _singleton_instance = mock_instance

    @classmethod
    def instance(cls) -> 'PromptSectionLoader':
        """
        Get the singleton instance of AgentConfigLoader.

        If it doesn't exist, create a new one with default parameters.
        """
        global _singleton_instance
        if _singleton_instance is None:
            _singleton_instance = cls()
        return _singleton_instance

    def _discover_sections(self) -> List[str]:
        """
        Globs the sections folder for all YAML files.
        Returns a list of their paths, relative to the sections folder.
        """
        if not self.sections_folder.exists():
            self.logger.warning(f"Sections folder {self.sections_folder} does not exist.")
            raise RuntimeError(f"Sections folder {self.sections_folder} does not exist.")

        # TODO: Check for markdown files as well, build and empty section model with just the prompt
        section_files = []
        for ext in ['.yaml', '.yml', '.md']:
            section_files.extend(list(self.sections_folder.glob(f"**/*.{ext}")))
        if not section_files:
            self.logger.warning("No section files found in the sections folder.")
            return []

        relative_paths = [str(file.relative_to(self.sections_folder)) for file in section_files]
        self.logger.info(f"Discovered {len(relative_paths)} section files")
        return relative_paths

    def load_sections(self):
        """
        Load all sections from the sections folder.
        Returns a list of section instances.
        """
        section_files = self._discover_sections()
        if not section_files:
            self.logger.warning("No sections to load.")
            return []

        for file_path in section_files:
            full_path = self.sections_folder.joinpath(file_path)
            try:
                self.load_section_from_file(full_path)
            except Exception as e:
                self.logger.error(f"Failed to load section from {file_path}: {e}")

    def load_section_from_file(self, file_path: Path) -> Optional[BasePromptSection]:
        """
        Load a section from a YAML file.
        Args:
            file_path: The path to the YAML file.
        Returns:
            An instance of the section.
        """
        if not file_path.exists():
            self.logger.error(f"Section file {file_path} does not exist.")
            raise FileNotFoundError(f"Section file {file_path} does not exist.")

        try:

            if file_path.suffix in ['.yml', '.yaml']:
                key = ("/".join(file_path.parts)).removesuffix(".yaml").removesuffix(".yml")
                with file_path.open('r', encoding='utf-8') as f:
                    section_data = yaml.load(f, Loader=yaml.FullLoader)
            else:
                key = ("/".join(file_path.parts)).removesuffix(".md")
                with file_path.open('r', encoding='utf-8') as f:
                    template = f.read()

                section_data = {
                    'section_type': key,
                    'section_description': f"This is a section loaded from {file_path}",
                    'template': template,
                    'is_include': True,
                }

            section_data['path_on_disk'] = str(file_path)
            section_instance = self.registry.register_section_dict(key, section_data)

            self.logger.info(f"Loaded section {key} from {file_path}")

            return section_instance
        except Exception as e:
            self.logger.error(f"Error loading section from {file_path}: {e}")
            return None

    def save_section_from_registry(self, section_type: str, file_path: Optional[Path] = None) -> bool:
        """
        Save a section from the registry to a YAML file.
        Args:
            section_type: The type of the section to save.
            file_path: Optional path to save the section. If None, uses the default path.
        Returns:
            True if saved successfully, False otherwise.
        """
        if not self.registry.is_section_registered(section_type):
            self.logger.error(f"Section type {section_type} is not registered.")
            return False

        section_instance = self.registry.create(section_type)
        if not section_instance:
            self.logger.error(f"No section instance found for type {section_type}.")
            return False

        file_path = section_instance.path_on_disk if file_path is None else file_path

        if file_path is None:
            file_path = self.sections_folder.joinpath(f"{section_type}.yaml")

        try:
            with file_path.open('w', encoding='utf-8') as f:
                yaml.dump(section_instance.model_dump(), f, allow_unicode=True, sort_keys=False)
            self.logger.info(f"Saved section {section_type} to {file_path}")
            return True
        except Exception as e:
            self.logger.error(f"Error saving section {section_type} to {file_path}: {e}")
            return False