"""
Math Toolset for Agent C - Providing advanced mathematical calculation functionality
"""

import io
import numpy as np
import sympy as sp
from scipy import integrate, optimize, stats
import matplotlib.pyplot as plt
from base64 import b64encode
from typing import List, Dict, Union, Optional, Any
import yaml

from agent_c.toolsets.tool_set import Toolset
from agent_c.toolsets.json_schema import json_schema
from agent_c_tools.tools.math.safe_eval import safe_eval, create_safe_function, safe_eval_with_x
from agent_c_tools.tools.math.prompt import MathSection


class MathTools(Toolset):
    """
    MathTools provides advanced mathematical calculation capabilities including:
    - Basic arithmetic
    - Calculus (differentiation, integration)
    - Algebra (solving equations, simplifying expressions)
    - Matrices and linear algebra
    - Statistics and probability
    - Function visualization
    - Numerical optimization
    """
    
    def __init__(self, **kwargs):
        """
        Initialize the Math toolset.
        """
        super().__init__(**kwargs)
        # self.section = MathSection()
    
    # ------ Basic Math Tools ------
    
    @json_schema(
        description="Safely evaluate mathematical expressions",
        params={
            "expression": {
                "type": "string",
                "description": "Mathematical expression string, e.g., \"2 * (3 + 4) ** 2\"",
                "required": True
            }
        }
    )
    async def evaluate_expression(self, **kwargs) -> str:
        """
        Safely evaluate mathematical expressions
        
        Args:
            expression: Mathematical expression string, e.g., "2 * (3 + 4) ** 2"
            
        Returns:
            The calculated result of the expression
        """
        expression = kwargs.get("expression")
        if not expression:
            return "ERROR: expression parameter is required"
        
        try:
            result = safe_eval(expression)
            return str(result)
        except Exception as e:
            return f"ERROR: {str(e)}"
    
    @json_schema(
        description="Perform basic arithmetic operations",
        params={
            "a": {
                "type": "number",
                "description": "First number",
                "required": True
            },
            "operation": {
                "type": "string",
                "description": "Operation type, can be \"add\", \"subtract\", \"multiply\", \"divide\", \"power\"",
                "required": True,
                "enum": ["add", "subtract", "multiply", "divide", "power"]
            },
            "b": {
                "type": "number",
                "description": "Second number",
                "required": True
            }
        }
    )
    async def calculate(self, **kwargs) -> str:
        """
        Perform basic arithmetic operations
        
        Args:
            a: First number
            operation: Operation type, can be "add", "subtract", "multiply", "divide", "power"
            b: Second number
            
        Returns:
            Calculation result
        """
        a = kwargs.get("a")
        operation = kwargs.get("operation")
        b = kwargs.get("b")
        
        if a is None or operation is None or b is None:
            return "ERROR: a, operation, and b parameters are required"
        
        try:
            if operation == "add":
                result = a + b
            elif operation == "subtract":
                result = a - b
            elif operation == "multiply":
                result = a * b
            elif operation == "divide":
                if b == 0:
                    return "ERROR: Divisor cannot be zero"
                result = a / b
            elif operation == "power":
                result = a ** b
            else:
                return f"ERROR: Unsupported operation: {operation}"
            
            return str(result)
        except Exception as e:
            return f"ERROR: {str(e)}"
    
    # ------ Symbolic Computation Tools ------
    
    @json_schema(
        description="Calculate the derivative of a function",
        params={
            "expression": {
                "type": "string",
                "description": "Function expression, e.g., \"x**2 + 2*x\"",
                "required": True
            },
            "variable": {
                "type": "string",
                "description": "Differentiation variable, e.g., \"x\"",
                "required": True
            },
            "order": {
                "type": "integer",
                "description": "Order of the derivative, default is 1",
                "required": False
            }
        }
    )
    async def differentiate(self, **kwargs) -> str:
        """
        Calculate the derivative of a function
        
        Args:
            expression: Function expression, e.g., "x**2 + 2*x"
            variable: Differentiation variable, e.g., "x"
            order: Order of the derivative, default is 1
            
        Returns:
            Expression of the derivative
        """
        expression = kwargs.get("expression")
        variable = kwargs.get("variable")
        order = kwargs.get("order", 1)
        
        if not expression or not variable:
            return "ERROR: expression and variable parameters are required"
        
        try:
            x = sp.Symbol(variable)
            expr = sp.sympify(expression)
            result = sp.diff(expr, x, order)
            return str(result)
        except Exception as e:
            return f"ERROR: {str(e)}"
    
    @json_schema(
        description="Calculate the integral of a function",
        params={
            "expression": {
                "type": "string",
                "description": "Function expression, e.g., \"x**2 + 2*x\"",
                "required": True
            },
            "variable": {
                "type": "string",
                "description": "Integration variable, e.g., \"x\"",
                "required": True
            },
            "lower_bound": {
                "type": "string",
                "description": "Lower bound of integration (optional)",
                "required": False
            },
            "upper_bound": {
                "type": "string",
                "description": "Upper bound of integration (optional)",
                "required": False
            }
        }
    )
    async def integrate_symbolic(self, **kwargs) -> str:
        """
        Calculate the integral of a function
        
        Args:
            expression: Function expression, e.g., "x**2 + 2*x"
            variable: Integration variable, e.g., "x"
            lower_bound: Lower bound of integration (optional)
            upper_bound: Upper bound of integration (optional)
            
        Returns:
            Expression of the integration result
        """
        expression = kwargs.get("expression")
        variable = kwargs.get("variable")
        lower_bound = kwargs.get("lower_bound")
        upper_bound = kwargs.get("upper_bound")
        
        if not expression or not variable:
            return "ERROR: expression and variable parameters are required"
        
        try:
            x = sp.Symbol(variable)
            expr = sp.sympify(expression)
            
            if lower_bound is not None and upper_bound is not None:
                # Definite integral
                lower = sp.sympify(lower_bound)
                upper = sp.sympify(upper_bound)
                result = sp.integrate(expr, (x, lower, upper))
            else:
                # Indefinite integral
                result = sp.integrate(expr, x)
            
            return str(result)
        except Exception as e:
            return f"ERROR: {str(e)}"
    
    @json_schema(
        description="Numerically calculate a definite integral",
        params={
            "expression": {
                "type": "string",
                "description": "Function expression, e.g., \"x**2 + 2*x\"",
                "required": True
            },
            "lower_bound": {
                "type": "number",
                "description": "Lower bound of integration",
                "required": True
            },
            "upper_bound": {
                "type": "number",
                "description": "Upper bound of integration",
                "required": True
            }
        }
    )
    async def integrate_numeric(self, **kwargs) -> str:
        """
        Numerically calculate a definite integral
        
        Args:
            expression: Function expression, e.g., "x**2 + 2*x"
            lower_bound: Lower bound of integration
            upper_bound: Upper bound of integration
            
        Returns:
            Numerical result of the integral
        """
        expression = kwargs.get("expression")
        lower_bound = kwargs.get("lower_bound")
        upper_bound = kwargs.get("upper_bound")
        
        if not expression or lower_bound is None or upper_bound is None:
            return "ERROR: expression, lower_bound, and upper_bound parameters are required"
        
        try:
            # Safely create function
            f = create_safe_function(expression)
            result, error = integrate.quad(f, lower_bound, upper_bound)
            return str(result)
        except Exception as e:
            return f"ERROR: {str(e)}"
    
    @json_schema(
        description="Solve an equation",
        params={
            "equation": {
                "type": "string",
                "description": "Equation expression, e.g., \"x**2 - 4 = 0\" or \"x**2 - 4\" (meaning x²=4)",
                "required": True
            },
            "variable": {
                "type": "string",
                "description": "Variable to solve for, e.g., \"x\"",
                "required": True
            }
        }
    )
    async def solve_equation(self, **kwargs) -> str:
        """
        Solve an equation
        
        Args:
            equation: Equation expression, e.g., "x**2 - 4 = 0" or "x**2 - 4" (meaning x²=4)
            variable: Variable to solve for, e.g., "x"
            
        Returns:
            Solution of the equation
        """
        equation = kwargs.get("equation")
        variable = kwargs.get("variable")
        
        if not equation or not variable:
            return "ERROR: equation and variable parameters are required"
        
        try:
            x = sp.Symbol(variable)
            # Handle cases containing an equals sign
            if "=" in equation:
                left, right = equation.split("=", 1)
                left = sp.sympify(left.strip())
                right = sp.sympify(right.strip())
                expr = left - right
            else:
                expr = sp.sympify(equation)
            
            solutions = sp.solve(expr, x)
            return str(solutions)
        except Exception as e:
            return f"ERROR: {str(e)}"
    
    @json_schema(
        description="Solve a system of equations",
        params={
            "equations": {
                "type": "array",
                "items": {"type": "string"},
                "description": "List of equations, e.g., [\"x + y = 10\", \"2*x - y = 5\"]",
                "required": True
            },
            "variables": {
                "type": "array",
                "items": {"type": "string"},
                "description": "List of variables, e.g., [\"x\", \"y\"]",
                "required": True
            }
        }
    )
    async def solve_system(self, **kwargs) -> str:
        """
        Solve a system of equations
        
        Args:
            equations: List of equations, e.g., ["x + y = 10", "2*x - y = 5"]
            variables: List of variables, e.g., ["x", "y"]
            
        Returns:
            Solution of the equation system
        """
        equations = kwargs.get("equations")
        variables = kwargs.get("variables")
        
        if not equations or not variables:
            return "ERROR: equations and variables parameters are required"
        
        try:
            # Create symbolic variables
            syms = [sp.Symbol(v) for v in variables]
            
            # Process equations
            exprs = []
            for eq in equations:
                if "=" in eq:
                    left, right = eq.split("=", 1)
                    left = sp.sympify(left.strip())
                    right = sp.sympify(right.strip())
                    exprs.append(left - right)
                else:
                    exprs.append(sp.sympify(eq))
            
            # Solve
            solutions = sp.solve(exprs, syms)
            return str(solutions)
        except Exception as e:
            return f"ERROR: {str(e)}"
    
    @json_schema(
        description="Simplify algebraic expression",
        params={
            "expression": {
                "type": "string",
                "description": "Expression to simplify",
                "required": True
            }
        }
    )
    async def simplify_expression(self, **kwargs) -> str:
        """
        Simplify algebraic expression
        
        Args:
            expression: Expression to simplify
            
        Returns:
            Simplified expression
        """
        expression = kwargs.get("expression")
        
        if not expression:
            return "ERROR: expression parameter is required"
        
        try:
            expr = sp.sympify(expression)
            result = sp.simplify(expr)
            return str(result)
        except Exception as e:
            return f"ERROR: {str(e)}"
    
    @json_schema(
        description="Expand algebraic expression",
        params={
            "expression": {
                "type": "string",
                "description": "Expression to expand",
                "required": True
            }
        }
    )
    async def expand_expression(self, **kwargs) -> str:
        """
        Expand algebraic expression
        
        Args:
            expression: Expression to expand
            
        Returns:
            Expanded expression
        """
        expression = kwargs.get("expression")
        
        if not expression:
            return "ERROR: expression parameter is required"
        
        try:
            expr = sp.sympify(expression)
            result = sp.expand(expr)
            return str(result)
        except Exception as e:
            return f"ERROR: {str(e)}"
    
    @json_schema(
        description="Factor an expression",
        params={
            "expression": {
                "type": "string",
                "description": "Expression to factor",
                "required": True
            }
        }
    )
    async def factor_expression(self, **kwargs) -> str:
        """
        Factor an expression
        
        Args:
            expression: Expression to factor
            
        Returns:
            Factored expression
        """
        expression = kwargs.get("expression")
        
        if not expression:
            return "ERROR: expression parameter is required"
        
        try:
            expr = sp.sympify(expression)
            result = sp.factor(expr)
            return str(result)
        except Exception as e:
            return f"ERROR: {str(e)}"
    
    # ------ Numerical Optimization Tools ------
    
    @json_schema(
        description="Find the local minimum value of a function",
        params={
            "expression": {
                "type": "string",
                "description": "Function expression, e.g., \"x**2 + 2*x\"",
                "required": True
            },
            "lower_bound": {
                "type": "number",
                "description": "Lower bound of the search range",
                "required": True
            },
            "upper_bound": {
                "type": "number",
                "description": "Upper bound of the search range",
                "required": True
            }
        }
    )
    async def find_minimum(self, **kwargs) -> str:
        """
        Find the local minimum value of a function
        
        Args:
            expression: Function expression, e.g., "x**2 + 2*x"
            lower_bound: Lower bound of the search range
            upper_bound: Upper bound of the search range
            
        Returns:
            Dictionary containing the minimum value and corresponding x value
        """
        expression = kwargs.get("expression")
        lower_bound = kwargs.get("lower_bound")
        upper_bound = kwargs.get("upper_bound")
        
        if not expression or lower_bound is None or upper_bound is None:
            return "ERROR: expression, lower_bound, and upper_bound parameters are required"
        
        try:
            f = create_safe_function(expression)
            result = optimize.minimize_scalar(f, bounds=(lower_bound, upper_bound), method='bounded')
            
            result_dict = {
                "x_value": float(result.x),
                "min_value": float(result.fun),
                "success": bool(result.success)
            }
            
            return yaml.dump(result_dict, allow_unicode=True)
        except Exception as e:
            return f"ERROR: {str(e)}"
    
    @json_schema(
        description="Find the root (zero) of a function",
        params={
            "expression": {
                "type": "string",
                "description": "Function expression, e.g., \"x**2 - 4\"",
                "required": True
            },
            "lower_bound": {
                "type": "number",
                "description": "Lower bound of the search range",
                "required": True
            },
            "upper_bound": {
                "type": "number",
                "description": "Upper bound of the search range",
                "required": True
            }
        }
    )
    async def find_root(self, **kwargs) -> str:
        """
        Find the root (zero) of a function
        
        Args:
            expression: Function expression, e.g., "x**2 - 4"
            lower_bound: Lower bound of the search range
            upper_bound: Upper bound of the search range
            
        Returns:
            Root of the function
        """
        expression = kwargs.get("expression")
        lower_bound = kwargs.get("lower_bound")
        upper_bound = kwargs.get("upper_bound")
        
        if not expression or lower_bound is None or upper_bound is None:
            return "ERROR: expression, lower_bound, and upper_bound parameters are required"
        
        try:
            f = create_safe_function(expression)
            result = optimize.brentq(f, lower_bound, upper_bound)
            return str(float(result))
        except Exception as e:
            return f"ERROR: {str(e)}"
    
    # ------ Matrix Calculation Tools ------
    
    @json_schema(
        description="Perform matrix operations",
        params={
            "operation": {
                "type": "string",
                "description": "Operation type, e.g., \"multiply\", \"inverse\", \"determinant\", \"eigenvalues\", \"transpose\"",
                "required": True,
                "enum": ["multiply", "inverse", "determinant", "eigenvalues", "transpose", "add", "subtract"]
            },
            "matrix_a": {
                "type": "array",
                "items": {"type": "array", "items": {"type": "number"}},
                "description": "First matrix, format like [[1, 2], [3, 4]]",
                "required": True
            },
            "matrix_b": {
                "type": "array",
                "items": {"type": "array", "items": {"type": "number"}},
                "description": "Second matrix (if needed), same format as above",
                "required": False
            }
        }
    )
    async def matrix_operation(self, **kwargs) -> str:
        """
        Perform matrix operations
        
        Args:
            operation: Operation type, e.g., "multiply", "inverse", "determinant", "eigenvalues", "transpose"
            matrix_a: First matrix, format like [[1, 2], [3, 4]]
            matrix_b: Second matrix (if needed), same format as above
            
        Returns:
            Operation result
        """
        operation = kwargs.get("operation")
        matrix_a = kwargs.get("matrix_a")
        matrix_b = kwargs.get("matrix_b")
        
        if not operation or not matrix_a:
            return "ERROR: operation and matrix_a parameters are required"
        
        try:
            # Convert to numpy arrays
            mat_a = np.array(matrix_a)
            
            if matrix_b is not None:
                mat_b = np.array(matrix_b)
        
            if operation == "determinant":
                result = np.linalg.det(mat_a)
                return str(result)
            elif operation == "inverse":
                result = np.linalg.inv(mat_a)
                return str(result.tolist())
            elif operation == "eigenvalues":
                result = np.linalg.eigvals(mat_a)
                return str(result.tolist())
            elif operation == "transpose":
                result = np.transpose(mat_a)
                return str(result.tolist())
            elif operation == "multiply" and matrix_b is not None:
                result = np.matmul(mat_a, mat_b)
                return str(result.tolist())
            elif operation == "add" and matrix_b is not None:
                result = mat_a + mat_b
                return str(result.tolist())
            elif operation == "subtract" and matrix_b is not None:
                result = mat_a - mat_b
                return str(result.tolist())
            else:
                return "ERROR: Unsupported operation or missing parameters"
        except Exception as e:
            return f"ERROR: {str(e)}"
    
    # ------ Visualization Tools ------
    
    @json_schema(
        description="Plot a function graph",
        params={
            "expression": {
                "type": "string",
                "description": "Function expression, e.g., \"x**2 + 2*x\"",
                "required": True
            },
            "x_min": {
                "type": "number",
                "description": "Minimum x-axis value",
                "required": False
            },
            "x_max": {
                "type": "number",
                "description": "Maximum x-axis value",
                "required": False
            },
            "points": {
                "type": "integer",
                "description": "Number of sampling points",
                "required": False
            }
        }
    )
    async def plot_function(self, **kwargs) -> str:
        """
        Plot a function graph
        
        Args:
            expression: Function expression, e.g., "x**2 + 2*x"
            x_min: Minimum x-axis value
            x_max: Maximum x-axis value
            points: Number of sampling points
            
        Returns:
            Function graph image
        """
        expression = kwargs.get("expression")
        x_min = kwargs.get("x_min", -10)
        x_max = kwargs.get("x_max", 10)
        points = kwargs.get("points", 1000)
        
        if not expression:
            return "ERROR: expression parameter is required"
        
        # Generate x values
        x_values = np.linspace(x_min, x_max, points)
        
        # Safely calculate y values
        try:
            f = create_safe_function(expression)
            y_values = [f(x_val) for x_val in x_values]
            
            # Create image
            plt.figure(figsize=(10, 6))
            plt.plot(x_values, y_values)
            plt.grid(True)
            plt.title(f"y = {expression}")
            plt.axhline(y=0, color='k', linestyle='-', alpha=0.3)
            plt.axvline(x=0, color='k', linestyle='-', alpha=0.3)
            plt.xlabel('x')
            plt.ylabel('y')
            
            # Convert image to binary data
            buffer = io.BytesIO()
            plt.savefig(buffer, format='png')
            plt.close()
            buffer.seek(0)
            
            # Get binary data and base64 encode it
            img_bytes = buffer.getvalue()
            base64_img = b64encode(img_bytes).decode('utf-8')
            
            # Create a name for the plot
            img_name = f"plot_{expression.replace(' ', '_')[:30]}.png"
            
            # Return the image by raising a render media event
            await self._raise_render_media(
                sent_by_class=self.__class__.__name__,
                sent_by_function='plot_function',
                content_type="image/png",
                name=img_name,
                content_bytes=img_bytes,
                content=base64_img,
                tool_context=kwargs.get('tool_context')
            )
            
            return f"Plot created for y = {expression}"
        
        except Exception as e:
            plt.close()
            return f"ERROR: Error when plotting function: {str(e)}"
    
    @json_schema(
        description="Plot graphs of multiple functions",
        params={
            "expressions": {
                "type": "array",
                "items": {"type": "string"},
                "description": "List of function expressions, e.g., [\"x**2\", \"2*x + 1\"]",
                "required": True
            },
            "labels": {
                "type": "array",
                "items": {"type": "string"},
                "description": "List of legend labels (optional)",
                "required": False
            },
            "x_min": {
                "type": "number",
                "description": "Minimum x-axis value",
                "required": False
            },
            "x_max": {
                "type": "number",
                "description": "Maximum x-axis value",
                "required": False
            },
            "points": {
                "type": "integer",
                "description": "Number of sampling points",
                "required": False
            }
        }
    )
    async def plot_multiple_functions(self, **kwargs) -> str:
        """
        Plot graphs of multiple functions
        
        Args:
            expressions: List of function expressions, e.g., ["x**2", "2*x + 1"]
            labels: List of legend labels (optional)
            x_min: Minimum x-axis value
            x_max: Maximum x-axis value
            points: Number of sampling points
            
        Returns:
            Function graph image
        """
        expressions = kwargs.get("expressions")
        labels = kwargs.get("labels")
        x_min = kwargs.get("x_min", -10)
        x_max = kwargs.get("x_max", 10)
        points = kwargs.get("points", 500)
        
        if not expressions:
            return "ERROR: expressions parameter is required"
        
        try:
            if labels is None:
                labels = [f"f_{i+1}(x) = {expr}" for i, expr in enumerate(expressions)]
            
            if len(labels) != len(expressions):
                labels = [f"f_{i+1}(x) = {expr}" for i, expr in enumerate(expressions)]
            
            # Generate x values
            x_values = np.linspace(x_min, x_max, points)
            
            plt.figure(figsize=(10, 6))
            
            # Plot each function
            for i, expr in enumerate(expressions):
                try:
                    f = create_safe_function(expr)
                    y_values = [f(x_val) for x_val in x_values]
                    plt.plot(x_values, y_values, label=labels[i])
                except Exception as e:
                    self.logger.error(f"Error plotting function '{expr}': {str(e)}")
                    return f"Error when plotting function '{expr}': {str(e)}"
            
            plt.grid(True)
            plt.title("Function Comparison")
            plt.axhline(y=0, color='k', linestyle='-', alpha=0.3)
            plt.axvline(x=0, color='k', linestyle='-', alpha=0.3)
            plt.xlabel('x')
            plt.ylabel('y')
            plt.legend()
            
            # Convert image to binary data
            buffer = io.BytesIO()
            plt.savefig(buffer, format='png')
            plt.close()
            buffer.seek(0)
            
            # Get binary data and base64 encode it
            img_bytes = buffer.getvalue()
            base64_img = b64encode(img_bytes).decode('utf-8')
            
            # Create a name for the plot
            img_name = f"multiplot_{len(expressions)}_functions.png"
            
            # Return the image by raising a render media event
            await self._raise_render_media(
                sent_by_class=self.__class__.__name__,
                sent_by_function='plot_multiple_functions',
                content_type="image/png",
                name=img_name,
                content_bytes=img_bytes,
                content=base64_img,
                tool_context=kwargs.get('tool_context')
            )
            
            return f"Plot created comparing {len(expressions)} functions"
        except Exception as e:
            plt.close()
            return f"ERROR: {str(e)}"

    # ------ Statistics Analysis Tools ------

    @json_schema(
        description="Perform statistical analysis",
        params={
            "data": {
                "type": "array",
                "items": {"type": "number"},
                "description": "List of data points, e.g., [1, 2, 3, 4, 5]",
                "required": True
            },
            "operation": {
                "type": "string",
                "description": "Statistical operation, such as \"mean\", \"median\", \"std\", \"var\", \"min\", \"max\"",
                "required": True,
                "enum": ["mean", "median", "std", "var", "min", "max", "sum", "count"]
            }
        }
    )
    async def statistics(self, **kwargs) -> str:
        """
        Perform statistical analysis
        
        Args:
            data: List of data points, e.g., [1, 2, 3, 4, 5]
            operation: Statistical operation, such as "mean", "median", "std", "var", "min", "max"
            
        Returns:
            Statistical result
        """
        data = kwargs.get("data")
        operation = kwargs.get("operation")
        
        if not data or not operation:
            return "ERROR: data and operation parameters are required"
        
        try:
            values = np.array(data)
            
            if operation == "mean":
                result = float(np.mean(values))
            elif operation == "median":
                result = float(np.median(values))
            elif operation == "std":
                result = float(np.std(values))
            elif operation == "var":
                result = float(np.var(values))
            elif operation == "min":
                result = float(np.min(values))
            elif operation == "max":
                result = float(np.max(values))
            elif operation == "sum":
                result = float(np.sum(values))
            elif operation == "count":
                result = len(values)
            else:
                return "ERROR: Unsupported statistical operation"
            
            return str(result)
        except Exception as e:
            return f"ERROR: {str(e)}"

    @json_schema(
        description="Generate statistical summary of data",
        params={
            "data": {
                "type": "array",
                "items": {"type": "number"},
                "description": "List of data points, e.g., [1, 2, 3, 4, 5]",
                "required": True
            }
        }
    )
    async def summarize_data(self, **kwargs) -> str:
        """
        Generate statistical summary of data
        
        Args:
            data: List of data points, e.g., [1, 2, 3, 4, 5]
            
        Returns:
            Dictionary containing multiple statistical values
        """
        data = kwargs.get("data")
        
        if not data:
            return "ERROR: data parameter is required"
        
        try:
            values = np.array(data)
            
            result_dict = {
                "count": len(values),
                "mean": float(np.mean(values)),
                "median": float(np.median(values)),
                "std": float(np.std(values)),
                "min": float(np.min(values)),
                "max": float(np.max(values)),
                "q1": float(np.percentile(values, 25)),
                "q3": float(np.percentile(values, 75)),
            }
            
            return yaml.dump(result_dict, allow_unicode=True)
        except Exception as e:
            return f"ERROR: {str(e)}"

    @json_schema(
        description="Calculate correlation between two sets of data",
        params={
            "data_x": {
                "type": "array",
                "items": {"type": "number"},
                "description": "First set of data, e.g., [1, 2, 3, 4, 5]",
                "required": True
            },
            "data_y": {
                "type": "array",
                "items": {"type": "number"},
                "description": "Second set of data, e.g., [2, 3, 4, 5, 6]",
                "required": True
            }
        }
    )
    async def correlation(self, **kwargs) -> str:
        """
        Calculate correlation between two sets of data
        
        Args:
            data_x: First set of data, e.g., [1, 2, 3, 4, 5]
            data_y: Second set of data, e.g., [2, 3, 4, 5, 6]
            
        Returns:
            Dictionary containing correlation coefficient and p-value
        """
        data_x = kwargs.get("data_x")
        data_y = kwargs.get("data_y")
        
        if not data_x or not data_y:
            return "ERROR: data_x and data_y parameters are required"
        
        try:
            x = np.array(data_x)
            y = np.array(data_y)
            
            if len(x) != len(y):
                return "ERROR: The two data sets must have the same length"
            
            r, p = stats.pearsonr(x, y)
            
            result_dict = {
                "correlation": float(r),
                "p_value": float(p)
            }
            
            return yaml.dump(result_dict, allow_unicode=True)
        except Exception as e:
            return f"ERROR: {str(e)}"


# Register the toolset
Toolset.register(MathTools)