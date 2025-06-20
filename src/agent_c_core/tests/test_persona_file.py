import unittest

from agent_c.models.completion.agent_config import AgentConfiguration


class TestPersonaFile(unittest.TestCase):
    def test_to_yaml(self):
        """Test exporting AgentConfiguration to YAML."""
        # Create a sample AgentConfiguration instance
        persona_file = AgentConfiguration(
            persona="You are a helpful assistant.",
            model_id="claude-3-sonnet",
            tools=["ThinkTools", "WorkspaceTools"],
            temperature=0.7,
            budget_tokens=10000,
            max_tokens=64000
        )
        
        # Convert to YAML
        yaml_str = persona_file.to_yaml()
        
        # Basic validation
        self.assertIsInstance(yaml_str, str)
        self.assertIn("persona: You are a helpful assistant.", yaml_str)
        self.assertIn("model_id: claude-3-sonnet", yaml_str)
        self.assertIn("temperature: 0.7", yaml_str)
        self.assertIn("budget_tokens: 10000", yaml_str)
        self.assertIn("max_tokens: 64000", yaml_str)
        self.assertIn("tools:", yaml_str)
        self.assertIn("- ThinkTools", yaml_str)
        self.assertIn("- WorkspaceTools", yaml_str)

    def test_from_yaml(self):
        """Test creating AgentConfiguration from a YAML string."""
        # Create a YAML string
        yaml_str = """
        persona: You are a helpful assistant.
        model_id: claude-3-sonnet
        tools:
          - ThinkTools
          - WorkspaceTools
        temperature: 0.7
        budget_tokens: 10000
        max_tokens: 64000
        """
        
        # Create AgentConfiguration from YAML
        persona_file = AgentConfiguration.from_yaml(yaml_str)
        
        # Validate the parsed object
        self.assertEqual(persona_file.persona, "You are a helpful assistant.")
        self.assertEqual(persona_file.model_id, "claude-3-sonnet")
        self.assertEqual(persona_file.tools, ["ThinkTools", "WorkspaceTools"])
        self.assertEqual(persona_file.temperature, 0.7)
        self.assertEqual(persona_file.budget_tokens, 10000)
        self.assertEqual(persona_file.max_tokens, 64000)

    def test_round_trip(self):
        """Test round-trip conversion (to YAML and back)."""
        # Create original AgentConfiguration
        original = AgentConfiguration(
            persona="You are a coding assistant.",
            model_id="gpt-4",
            tools=["ThinkTools"],
            temperature=0.5,
            reasoning_effort=8,
            max_tokens=32000
        )
        
        # Convert to YAML and back
        yaml_str = original.to_yaml()
        roundtrip = AgentConfiguration.from_yaml(yaml_str)
        
        # Verify all fields match
        self.assertEqual(original.persona, roundtrip.persona)
        self.assertEqual(original.model_id, roundtrip.model_id)
        self.assertEqual(original.tools, roundtrip.tools)
        self.assertEqual(original.temperature, roundtrip.temperature)
        self.assertEqual(original.reasoning_effort, roundtrip.reasoning_effort)
        self.assertEqual(original.budget_tokens, roundtrip.budget_tokens)
        self.assertEqual(original.max_tokens, roundtrip.max_tokens)

    def test_partial_fields(self):
        """Test with only required fields."""
        # Create YAML with only required fields
        yaml_str = """
        persona: You are a minimal assistant.
        model_id: gpt-3.5-turbo
        """
        
        # Create AgentConfiguration from minimal YAML
        persona_file = AgentConfiguration.from_yaml(yaml_str)
        
        # Validate required fields
        self.assertEqual(persona_file.persona, "You are a minimal assistant.")
        self.assertEqual(persona_file.model_id, "gpt-3.5-turbo")
        
        # Validate optional fields have default values
        self.assertEqual(persona_file.tools, [])
        self.assertIsNone(persona_file.temperature)
        self.assertIsNone(persona_file.reasoning_effort)
        self.assertIsNone(persona_file.budget_tokens)
        self.assertIsNone(persona_file.max_tokens)


if __name__ == "__main__":
    unittest.main()