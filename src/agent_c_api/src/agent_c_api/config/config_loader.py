import json
from agent_c_api.config.env_config import settings
from agent_c_api.core.util.logging_utils import LoggingManager

logging_manager = LoggingManager(__name__)
logger = logging_manager.get_logger()

MODELS_CONFIG = {}

if settings.MODEL_CONFIG_PATH.is_file():
    # Load the config file only once at module import
    with open(settings.MODEL_CONFIG_PATH, 'r', encoding='utf-8') as f:
        MODELS_CONFIG = json.load(f)
else:
    logger.warning(f"Model config file does not exist: {settings.MODEL_CONFIG_PATH}")



def get_allowed_params(vendor: str, model_id: str) -> dict:
    for vendor_conf in MODELS_CONFIG["vendors"]:
        if vendor_conf["vendor"] == vendor:
            for model in vendor_conf["models"]:
                if model["id"] == model_id:
                    return model["parameters"]
    raise ValueError("Model not found for the given vendor.")


def extract_params_schema(config: dict, path: str = "") -> dict:
    """
    Recursively extracts a flattened parameter schema from nested model configuration.
    Returns mapping of parameter names to their expected types.
    """
    result = {}

    for key, value in config.items():
        # Skip metadata keys
        if key in ["required", "min", "max", "values"]:
            continue

        param_path = f"{path}.{key}" if path else key

        if key == "default":
            # This is a leaf default value - determine type from the value
            if isinstance(value, float):
                result[path] = {"type": "float", "default": value}
            elif isinstance(value, int):
                result[path] = {"type": "int", "default": value}
            elif isinstance(value, bool):
                result[path] = {"type": "bool", "default": value}
            else:
                result[path] = {"type": "str", "default": value}

        elif key == "enabled" and isinstance(value, bool):
            # Feature toggle pattern (like extended_thinking.enabled)
            parent_path = path
            result[parent_path] = {"type": "bool", "default": value}

        elif isinstance(value, dict):
            # Recurse into nested structures
            nested_result = extract_params_schema(value, param_path)
            result.update(nested_result)

    return result

