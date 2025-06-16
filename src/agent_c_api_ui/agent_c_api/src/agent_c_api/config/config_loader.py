from typing import Dict, Any

from agent_c.config import ModelConfigurationLoader



def get_allowed_params(model_id: str) -> Dict[str, Any]:
    loader: ModelConfigurationLoader = ModelConfigurationLoader()
    model_entry = loader.model_id_map.get(model_id)
    if model_entry:
        return model_entry.parameters

    raise ValueError("Model not found for the given vendor.")


