import importlib
from typing import Any, List
from pydantic_core import core_schema

from agent_c.util.logging_utils import LoggingManager

class SectionsList:
    @classmethod
    def __get_pydantic_core_schema__(
        cls, source_type: Any, handler
    ) -> core_schema.CoreSchema:
        # Deserialize: List[str] -> List[Instances]
        def deserialize_sections(class_names: List[str]) -> List[Any]:
            logger = LoggingManager(cls.__name__).get_logger()
            instances: List[Any] = []
            for class_path in class_names:
                try:
                    module_name, class_name = class_path.rsplit('.', 1)
                    module = importlib.import_module(module_name)
                    section_cls = getattr(module, class_name)
                    instances.append(section_cls())
                except (ImportError, AttributeError) as e:
                    logger.error(f"Failed to import {class_path}: {e}")
                except Exception as e:
                    logger.error(f"Error instantiating {class_path}: {e}")
            return instances

        # Serialize: List[Instances] -> List[str]
        def serialize_sections(
            sections: List[Any], info  # info is a ValidationInfo, here unused
        ) -> List[str]:
            return [
                f"{sect.__class__.__module__}.{sect.__class__.__qualname__}"
                for sect in sections
            ]

        # Core schema for a list of strings
        list_of_str_schema = core_schema.list_schema(core_schema.str_schema())

        return core_schema.with_info_after_validator_function(
            deserialize_sections,
            schema=list_of_str_schema,
            # attach a JSON‐only serializer
            serialization=core_schema.plain_serializer_function_ser_schema(
                serialize_sections,
                info_arg=True,                           # our func takes (value, info)
                return_schema=list_of_str_schema,        # output is List[str]
                when_used='json'                         # only for .model_dump_json()
            )
        )
