from typing import Any

from agent_c.prompting.prompt_section import OldPromptSection, property_bag_item


class CodeInterpreterToolsSection(OldPromptSection):
    def __init__(self, **data: Any):
        TEMPLATE = ("""
        You are interacting with a Python code interpreter hosted in a Docker container. The interpreter supports the following capabilities:
    Code Execution: You can write and execute Python code. Make sure your code includes print statements to display any outputs.
    Library Support: You can specify Python libraries required for your code execution. These libraries will be made available in the environment. For example:
        libraries: ["numpy", "pandas"]
    Input Files: If your code requires input data, you can specify a list of file names from the workspace. These files will be accessible in the /input directory inside the container. Use the variable INPUT_DIR to reference this directory in your code. For example:
        input_files: ["data.csv", "config.json"]
        Access in code: os.path.join(INPUT_DIR, "data.csv")
    Output Files: Your code can save generated files to the /output directory, which corresponds to the workspace's output area. Use the variable OUTPUT_DIR to save files. For example:
        Save in code: os.path.join(OUTPUT_DIR, "results.txt")
    Error Handling: Any exceptions encountered during execution will be captured and displayed in the output for debugging.
    Here is an example schema for submitting a request:
        ```json
        {
            "code": "Python code to execute. Include print statements for output.",
            "libraries": ["list of required libraries, e.g., numpy"],
            "input_files": ["list of files to use from the workspace, optional"],
            "workspace_name": "name of the workspace to use for file operations, optional"
        }```
    When writing code:
    - Use the provided INPUT_DIR for reading files.
    - Use the provided OUTPUT_DIR for saving files.
    - Include print statements for any intermediate or final outputs you want to capture.
    For example:
    ```pyton
import os
import pandas as pd

# Read input data
input_file = os.path.join(INPUT_DIR, "data.csv")
data = pd.read_csv(input_file)

# Perform some processing
data["new_column"] = data["existing_column"] * 2

# Save output
output_file = os.path.join(OUTPUT_DIR, "processed_data.csv")
data.to_csv(output_file, index=False)

# Print summary
print(f"Processed {len(data)} rows. Output saved to {output_file}.")
```

Write Python code that meets these requirements. Include any necessary library imports, file handling, and outputs to achieve your task.
    """
                    )
        super().__init__(template=TEMPLATE, required=True, name="Working with Data Tools", render_section_header=True, **data)

