from typing import Any, Optional

from agent_c.prompting.prompt_section import OldPromptSection, property_bag_item


class MathSection(OldPromptSection):

    def __init__(self, **data: Any):
        TEMPLATE = (
            "The Math Tools provide advanced mathematical calculation capabilities:"
            "- **evaluate_expression**: Safely evaluate mathematical expressions like \"2 * (3 + 4) ** 2\""
            "- **calculate**: Perform basic arithmetic operations (add, subtract, multiply, divide, power)"
            "- **differentiate**: Calculate function derivatives"
            "- **integrate_symbolic**: Symbolically integrate functions"
            "- **integrate_numeric**: Numerically calculate definite integrals"
            "- **solve_equation**: Solve single-variable equations"
            "- **solve_system**: Solve systems of equations"
            "- **simplify_expression**: Simplify algebraic expressions"
            "- **expand_expression**: Expand algebraic expressions"
            "- **factor_expression**: Factor expressions into products"
            "- **find_minimum**: Find the local minimum value of a function in a range"
            "- **find_root**: Find the root (zero) of a function in a range"
            "- **matrix_operation**: Perform matrix operations (multiply, inverse, determinant, eigenvalues, transpose)"
            "- **plot_function**: Plot a function graph"
            "- **plot_multiple_functions**: Compare multiple functions on the same graph"
            "- **statistics**: Calculate basic statistical measures (mean, median, std, var, min, max)"
            "- **summarize_data**: Generate a comprehensive statistical summary of a dataset"
            "- **correlation**: Calculate the correlation between two sets of data"
        )
        super().__init__(template=TEMPLATE, required=True, name="Math Tools", render_section_header=True, **data)