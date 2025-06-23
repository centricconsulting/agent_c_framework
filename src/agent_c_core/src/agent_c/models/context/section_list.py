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
            logging_manager = LoggingManager(cls.__name__)
            logger = logging_manager.get_logger()
            instances = []
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
            sections: List[Any], info
        ) -> List[str]:
            return [
                f"{sect.__class__.__module__}.{sect.__class__.__qualname__}"
                for sect in sections
            ]

        # Base schema: we accept a list of strings
        list_of_str_schema = core_schema.list_schema(core_schema.str_schema())

        # Wrap the validator (deserialize) AND attach a serializer
        return core_schema.with_info_after_validator_function(
            deserialize_sections,
            schema=list_of_str_schema,
            # this schema is used when doing .model_dump_json() but not when doing .model_dump()
            serialization=core_schema.to_jsonable_schema(
                core_schema.list_schema(
                    core_schema.function_ser_schema(
                        serialize_sections,
                        type_=Any,
                        when_used='json',    # only for JSON dumps
                    )
                )
            )
        )
