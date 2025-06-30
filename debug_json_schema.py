#!/usr/bin/env python3
"""
Debug JSON schema decoration
"""

def test_json_schema():
    print("🔍 Testing JSON Schema Decoration...")
    
    # Test with a known working tool
    print("\n1️⃣ Testing Random Number Tool (known working)...")
    try:
        from agent_c_tools.tools.random_number.tool import RandomNumberTools
        rng_tool = RandomNumberTools()
        
        print(f"Random tool method: {rng_tool.generate_random_number}")
        print(f"Has json_schema: {hasattr(rng_tool.generate_random_number, 'json_schema')}")
        
        if hasattr(rng_tool.generate_random_number, 'json_schema'):
            schema = rng_tool.generate_random_number.json_schema
            print(f"Schema keys: {list(schema.keys())}")
        
        # Check all attributes
        attrs = [attr for attr in dir(rng_tool.generate_random_number) if not attr.startswith('_')]
        print(f"Method attributes: {attrs}")
        
    except Exception as e:
        print(f"❌ Random tool test failed: {e}")
    
    # Test with our PDF tool
    print("\n2️⃣ Testing PDF Converter Tool...")
    try:
        from agent_c_tools.tools.pdf_converter.tool import PDFConverterTools
        pdf_tool = PDFConverterTools()
        
        print(f"PDF tool method: {pdf_tool.pdf_to_json}")
        print(f"Has json_schema: {hasattr(pdf_tool.pdf_to_json, 'json_schema')}")
        
        # Check all attributes
        attrs = [attr for attr in dir(pdf_tool.pdf_to_json) if not attr.startswith('_')]
        print(f"Method attributes: {attrs}")
        
        # Check if it's a different attribute name
        for attr in attrs:
            attr_value = getattr(pdf_tool.pdf_to_json, attr)
            if isinstance(attr_value, dict) and 'description' in str(attr_value):
                print(f"Found potential schema in attribute '{attr}': {attr_value}")
        
    except Exception as e:
        print(f"❌ PDF tool test failed: {e}")
    
    # Test the decorator directly
    print("\n3️⃣ Testing json_schema decorator directly...")
    try:
        from agent_c.toolsets import json_schema
        
        @json_schema(
            description="Test function",
            params={"test": {"type": "string", "description": "Test param"}}
        )
        async def test_function(**kwargs):
            return "test"
        
        print(f"Test function: {test_function}")
        print(f"Has json_schema: {hasattr(test_function, 'json_schema')}")
        
        # Check all attributes
        attrs = [attr for attr in dir(test_function) if not attr.startswith('_')]
        print(f"Test function attributes: {attrs}")
        
    except Exception as e:
        print(f"❌ Direct decorator test failed: {e}")

if __name__ == "__main__":
    test_json_schema()