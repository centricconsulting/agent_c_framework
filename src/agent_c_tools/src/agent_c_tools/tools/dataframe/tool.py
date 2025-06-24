import io
import re
import os
import json
import time
import random
import string
import logging
import numpy as np
import pandas as pd
from pathlib import Path
from datetime import datetime

from sklearn.linear_model import LinearRegression
from scipy.stats import pearsonr, spearmanr, kendalltau

from agent_c.toolsets import json_schema, Toolset
from ...helpers.media_file_html_helper import get_file_html
from ...helpers.path_helper import ensure_file_extension, create_unc_path, os_file_system_path

from .prompt import DataframeToolsSection
from ...helpers.dataframe_in_memory import create_excel_in_memory

class DataframeTools(Toolset):
    DEFAULT_DATA_FOLDER = 'dataframe_data'
    MAX_DATAFRAME_TOKEN_SIZE = 35000
    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='dataframe')
        self.dataframe = None
        self.temp_dataframe = None
        self.file_path = None
        self.section = DataframeToolsSection()
        self.workspace_tool = self.tool_chest.active_tools.get('WorkspaceTools')

        self.key_prefix = 'df_'  # for unique dataframe cache keys

        self.logger: logging.Logger = logging.getLogger(__name__)

    def _standardized_result(self, additional_json=None, message=None) -> str:
        if self.dataframe is None:
            return json.dumps({"error": "No DataFrame is currently loaded."})

        result = {
            "shape": self.dataframe.shape,
            "columns": self.dataframe.columns.tolist(),
            "dtypes": self.dataframe.dtypes.astype(str).to_dict()
        }

        if message:
            result["message"] = message

        if additional_json:
            result.update(additional_json)

        return json.dumps(result)

    def _is_dataframe_too_big(self, dataframe: str) -> bool:
        # dataframe should be passed in as a string for counting tokens - however you plan to pass it back to the LLM
        _response_count = self.tool_chest.agent.count_tokens(dataframe)
        # self.logger.info(f"DataFrame token count: {_response_count}")
        return True if _response_count > self.MAX_DATAFRAME_TOKEN_SIZE else False


    def _generate_unique_key(self, base_key: str = None) -> str:
        """Generate a unique key for storing DataFrames."""
        timestamp = int(time.time() * 1000)  # millisecond timestamp
        random_string = ''.join(random.choices(string.ascii_lowercase + string.digits, k=6))

        if base_key:
            # If a base_key is provided, use it and append a number if necessary
            key = f"{self.key_prefix}{base_key}"
            counter = 1
            while self.tool_cache.get(key) is not None:
                key = f"{self.key_prefix}{base_key}_{counter}"
                counter += 1
        else:
            # If no base_key, use timestamp and random string
            key = f"{self.key_prefix}{timestamp}_{random_string}"

        return key

    def _load_data_from_file(self, file_path: str) -> str:
        # Check if the file exists
        if not os.path.exists(file_path):
            return f'Error: File not found at path: {file_path}'

        # Check if the file is readable
        if not os.access(file_path, os.R_OK):
            return f'Error: File is not readable: {file_path}'
        file_extension = Path(file_path).suffix.lower()
        # self.logger.info(f'Loading data from file: {file_path}')
        try:
            if file_extension == '.csv':
                self.dataframe = pd.read_csv(file_path, parse_dates=True)
            elif file_extension in ['.xlsx', '.xls']:
                self.dataframe = pd.read_excel(file_path, parse_dates=True)
            elif file_extension == '.pkl':
                self.dataframe = pd.read_pickle(file_path, parse_dates=True)
            elif file_extension == '.json':
                self.dataframe = pd.read_json(file_path)
            elif file_extension in ['.h5', '.hdf5']:
                self.dataframe = pd.read_hdf(file_path, parse_dates=True)
            elif file_extension == '.feather':
                self.dataframe = pd.read_feather(file_path, parse_dates=True)
            elif file_extension == '.parquet':
                self.dataframe = pd.read_parquet(file_path, parse_dates=True)
            else:
                return f"Unsupported file type: {file_extension}"

            if self.dataframe is None:
                return f'Error loading from file: {file_path}'

            self.logger.info(f"Data loaded from file: {file_path}\n")

            return self._standardized_result(message=f"DataFrame loaded successfully from file: {file_path}")
        except Exception as e:
            self.logger.error(f'Error loading from file: {e}')
            return f'Error loading from a file: {e}'
            
    def _load_data_from_bytes(self, file_bytes: bytes, file_extension: str, file_path: str=None) -> str:
        """Load data from bytes instead of a file path.
        
        Args:
            file_bytes: The file content as bytes
            file_extension: The file extension (e.g., '.csv', '.xlsx') to determine the bytes file type
            
        Returns:
            Result message as string
        """
        try:
            # Create a BytesIO object from the bytes
            bytes_io = io.BytesIO(file_bytes)
            
            # Use the appropriate pandas read method based on file extension
            if file_extension == '.csv':
                self.dataframe = pd.read_csv(bytes_io, parse_dates=True)
            elif file_extension in ['.xlsx', '.xls']:
                self.dataframe = pd.read_excel(bytes_io, parse_dates=True)
            elif file_extension == '.pkl':
                self.dataframe = pd.read_pickle(bytes_io)
            elif file_extension == '.json':
                self.dataframe = pd.read_json(bytes_io)
            elif file_extension in ['.h5', '.hdf5']:
                # HDF5 requires a filename, not possible with just bytes
                return f"HDF5 files must be loaded from a file path, not from bytes"
            elif file_extension == '.feather':
                self.dataframe = pd.read_feather(bytes_io)
            elif file_extension == '.parquet':
                self.dataframe = pd.read_parquet(bytes_io)
            else:
                return f"Unsupported file type: {file_extension}"

            if self.dataframe is None:
                return f'Error loading from bytes'

            return self._standardized_result(message=f"DataFrame loaded successfully from bytes. {f'Loaded from: {file_path}' if file_path else ''}")
        except Exception as e:
            self.logger.error(f'Error loading from bytes: {e}')
            return f'Error loading from bytes: {e}'

    @json_schema(
        description='Load a data file into a DataFrame from a native path or via a UNC style path for workspaces',
        params={
            'path': {
                'type': 'string',
                'description': 'The full UNC path to the file to load',
                'required': True
            }
        }
    )
    async def load_data(self, **kwargs):
        file_path: str = kwargs.get('path')

        if file_path is None:
            return "No file path provided."

        error, workspace, relative_path = self.workspace_tool.validate_and_get_workspace_path(file_path)
        if error:
            return f"Invalid path: {error}"

        os_path = os_file_system_path(self.workspace_tool, file_path)

        file_bytes = await workspace.read_bytes_internal(os_path)

        try:
            # Use read_bytes_internal to get the file content as bytes
            file_extension = Path(os_path).suffix.lower()
            # Use our new method to load from bytes
            return self._load_data_from_bytes(file_bytes, file_extension, file_path)
        except Exception as e:
            self.logger.error(f'Error reading file bytes from workspace: {e}')
            return f'Error reading file from workspace: {e}'


    @json_schema(
        description='Load data directly from bytes into a DataFrame',
        params={
            'file_bytes': {
                'type': 'string',  # We'll interpret this as a base64 string and decode it
                'description': 'The file content as a base64-encoded string',
                'required': True
            },
            'file_extension': {
                'type': 'string',
                'description': 'The file extension (e.g., ".csv", ".xlsx") to determine the file type',
                'required': True
            },
        }
    )
    async def load_data_from_bytes(self, **kwargs):
        import base64
        
        file_bytes_b64 = kwargs.get('file_bytes')
        file_extension = kwargs.get('file_extension')
        
        try:
            # Decode the base64 string to bytes
            file_bytes = base64.b64decode(file_bytes_b64)
            
            # Use our helper method to load from bytes
            return self._load_data_from_bytes(file_bytes, file_extension)
        except Exception as e:
            self.logger.error(f'Error loading data from bytes: {e}')
            return f'Error loading data from bytes: {e}'
            
    @json_schema(
        description="Store a DataFrame in the tool_cache using parquet format.  This is necessary if the dataframe "
                    "will be used by the visualization tool",
        params={
            'data_key': {
                'type': 'string',
                'description': 'The key for storing the DataFrame in the tool_cache',
                'required': False
            },
            'expire': {
                'type': 'integer',
                'description': 'Time in seconds until the cached data expires',
                'required': False,
                'default': 3600  # 1 hour default
            }
        }
    )
    async def store_dataframe_to_cache(self, **kwargs):
        # Hidden in kwargs is an optional dataframe parameter.
        # If set, we'll save that dataframe to cache, if not set, we'll default to the class's self.dataframe
        # This allows us to save either a temporary dataframe or the main dataframe to cache
        try:
            data_key = kwargs.get('data_key', self._generate_unique_key())
            expire = kwargs.get('expire', 3600)
            dataframe = kwargs.get('dataframe', self.dataframe)# Default to 1 hour if not specified
            if dataframe is None or dataframe.empty:
                return 'No DataFrame is in memory. Unable to save to cache. Please load data first'

            # self.logger.debug(f"dataframe shape: {dataframe.shape}")

            # Convert DataFrame to parquet format
            parquet_buffer = io.BytesIO()
            dataframe.to_parquet(parquet_buffer, engine='auto', compression='snappy')
            parquet_data = parquet_buffer.getvalue()

            # Store the parquet data in the tool_cache
            self.tool_cache.set(data_key, parquet_data, expire=expire)
            self.logger.info(f"DataFrame stored in tool_cache with key: {data_key}")
            return f"DataFrame stored in tool_cache with key: {data_key}"
        except Exception as e:
            self.logger.error(f"Error storing DataFrame in cache: {str(e)}")
            return json.dumps({"error": f"Error storing DataFrame: {str(e)}"})

    @json_schema(
        description="Load a parquet formatted DataFrame from the tool_cache",
        params={
            'data_key': {
                'type': 'string',
                'description': 'The key for storing the DataFrame in the tool_cache',
                'required': True
            },
        }
    )
    async def load_cached_dataframe(self, **kwargs)->str:
        try:
            data_key = kwargs.get('data_key', None)
            if data_key is None:
                return json.dumps({"error": "No data_key provided."})

            parquet_data = self.tool_cache.get(data_key)
            if parquet_data is None:
                return json.dumps({"error": f"DataFrame not found with key: {data_key}"})

            # Read the parquet data back into a DataFrame
            self.dataframe = pd.read_parquet(io.BytesIO(parquet_data))

            # Return basic info about the DataFrame
            return self._standardized_result(message=f"DataFrame retrieved successfully with key: {data_key}")
        except Exception as e:
            self.logger.error(f"Error loading DataFrame from cache: {str(e)}")
            return json.dumps({"error": f"Error retrieving DataFrame: {str(e)}"})


    @json_schema(
        description="Loads a JSON string into a pandas DataFrame",
        params={
            'json_data': {
                'type': 'string',
                'description': 'Convert JSON string into a DataFrame',
                'required': True
            },
        }
    )
    async def json_to_dataframe(self, **kwargs)->str:
        json_data = kwargs.get('json_data', '')

        if json_data == '':
            return "No JSON data provided."

        try:
            # Parse the JSON data
            data = json.loads(json_data)

            # Convert to DataFrame
            self.dataframe = pd.DataFrame(data)

            return self._standardized_result(message="DataFrame successfully created from JSON data")

        except json.JSONDecodeError:
            return json.dumps({"error": "Invalid JSON data"})
        except Exception as e:
            return json.dumps({"error": f"Error: {str(e)}"})

    @json_schema(
        description="Add a new column to the existing DataFrame. The column added can be a direct value or an expression to be evaluated. "
                    "For date math, use only valid pandas expressions. For example, "
                    "to add 5 days to a date column, use 'df['date_column'] + pd.Timedelta(days=5)' or  'df['date_column'] + pd.DateOffset(days=5)'."
                    "to subtract two dates from each other use '(pd.to_datetime(df['actualclosedate']) - pd.to_datetime(df['createdon'])).dt.days' "
                    "for extracting a date part, use 'df['new_column'].dt.year', 'df['new_column'].dt.month', 'df['new_column'].dt.day', etc.",
        params={
            'column_name': {
                'type': 'string',
                'description': 'The name of the new column to add',
                'required': True
            },
            'column_value_or_expression': {
                'type': 'string',
                'description': 'The value or expression for the new column',
                'required': False
            },
            'is_expression': {
                'type': 'boolean',
                'description': 'Indicates if the column_value_or_expression is an expression to be evaluated',
                'required': False,
                'default': False
            }
        }
    )
    async def add_column(self, **kwargs):
        # this always modifies the class's dataframe because adding a column doesn't really affect anything else you would do with a dataframe
        column_name = kwargs.get('column_name')
        column_value_or_expression = kwargs.get('column_value_or_expression')
        is_expression = kwargs.get('is_expression', False)

        if self.dataframe is None:
            return 'No DataFrame is loaded. Please load a file first.'
        self.logger.info(
            f'Adding column {column_name} with value {column_value_or_expression}. Expression flag is: {is_expression}')
        try:
            if is_expression:
                try:
                    eval_context = {
                        'pd': pd,
                        'np': np,
                        'datetime': datetime,
                        'dataframe': self.dataframe,
                        'df': self.dataframe
                    }

                    # Add all column names to the context
                    for col in self.dataframe.columns:
                        eval_context[col] = self.dataframe[col]

                    def replace_column_names(match):
                        column_name = match.group(1)
                        if column_name in self.dataframe.columns:
                            return f"dataframe['{column_name}']"
                        return match.group(0)

                    # Preprocess the expression
                    modified_value = column_value_or_expression.replace('df[', 'dataframe[')
                    modified_value = modified_value.replace('df.', 'dataframe.')

                    # Replace column names, but not within quotes or function calls
                    modified_value = re.sub(r'(?<![\w\'"])(\w+)(?![\w\'"])', replace_column_names, modified_value)

                    # Wrap the entire expression in parentheses to ensure it's evaluated as a whole
                    modified_value = f"({modified_value})"

                    result = eval(modified_value, eval_context)

                    if isinstance(result, pd.Series):
                        self.dataframe[column_name] = result
                    elif isinstance(result, pd.Timedelta):
                        self.dataframe[column_name] = result.total_seconds() / (24 * 60 * 60)  # Convert to days
                    else:
                        self.dataframe[column_name] = pd.Series(result, index=self.dataframe.index)

                    return f'Column {column_name} added successfully using expression?: {is_expression}, and column value or expression: {column_value_or_expression}.'
                except Exception as e:
                    return f'Error adding column: Custom eval failed. Error: "{str(e)}"'
            else:
                # If not an expression, just assign the value directly
                self.dataframe[column_name] = column_value_or_expression
                # self.logger.info(f"Column {column_name} added successfully with direct value assignment")
                return self._standardized_result(message=f"Dataframe has a new column.  See new list of columns and dataframe shape.")
        except Exception as e:
            self.logger.error(f'Error adding column: {str(e)}')
            return f'Error adding column: {str(e)}'

    @json_schema(
        description='Drop a column from the DataFrame',
        params={
            'column_name': {
                'type': 'string',
                'description': 'The name of the column to drop',
                'required': True
            }
        }
    )
    async def drop_column(self, **kwargs):
        # this always modifies the class's dataframe because dropping a column doesn't normally impact the size of the dataframe
        column_name = kwargs.get('column_name')
        if self.dataframe is None or self.dataframe.empty:
            return 'No DataFrame is loaded or it is empty. Please load data file first before dropping columns.'
        self.logger.info(f'Dropping column: {column_name}')
        if column_name not in self.dataframe.columns:
            return f'Column {column_name} does not exist in the DataFrame.'

        try:
            self.dataframe.drop(columns=[column_name], inplace=True)
            # self.logger.info(f'Column {column_name} dropped successfully.\n')
            return f'Column {column_name} dropped successfully.'
        except Exception as e:
            self.logger.error(f'Error dropping column: {e}')
            return f'Error dropping column: {e}'

    @json_schema(
        description='Rename columns in the DataFrame',
        params={
            'renaming_map': {
                'type': 'object',
                'description': 'A dictionary mapping old column names to new names',
                'required': True
            }
        }
    )
    async def rename_columns(self, **kwargs):
        # this always modifies the class's dataframe because renaming a column doesn't really affect anything else you would do with a dataframe or its size
        renaming_map = kwargs.get('renaming_map')
        if self.dataframe is None or self.dataframe.empty:
            return 'No DataFrame is loaded or it is empty. Please load data file first before renaming columns.'
        self.logger.info(f'Renaming columns: {renaming_map}')
        try:
            self.dataframe.rename(columns=renaming_map, inplace=True)
            return f'Columns renamed successfully to {list(renaming_map.values())}.'
        except Exception as e:
            self.logger.error(f'Error renaming columns: {e}')
            return f'Error renaming columns: {e}'

    @json_schema(
        description='Sort the DataFrame',
        params={
            'by': {
                'type': 'array',
                "items": {"type": "string"},
                'description': 'List of column names to sort by',
                'required': True
            },
            'ascending': {
                'type': 'boolean',
                'description': 'Sort in ascending order',
                'required': False,
                'default': True
            }
        }
    )
    async def sort_dataframe(self, **kwargs):
        # this always modifies the class's dataframe because sorting doesn't really affect anything else you would do with a dataframe or its size
        by = kwargs.get('by')
        ascending = kwargs.get('ascending', True)
        if self.dataframe is None or self.dataframe.empty:
            return 'No DataFrame is loaded or it is empty. Please load data file first before sorting'
        self.logger.info(f'Sorting DataFrame by {by} in {"ascending" if ascending else "descending"} order.')
        try:
            self.dataframe.sort_values(by=by, ascending=ascending, inplace=True)
            return f'DataFrame sorted by {by} in {"ascending" if ascending else "descending"} order.'
        except Exception as e:
            self.logger.error(f'Error sorting DataFrame: {e}')
            return f'Error sorting DataFrame: {e}'

    @json_schema(
        description="Returns rows in the DataFrame based on the provided pandas query condition. Use column names and "
                    "comparison operators to define conditions. Examples of valid conditions will be like 'age > 30', "
                    "\"status == 'active'\", 'income <= 50000'. Do not pass in functions such as mean, std, etc... as "
                    "filter criteria. This process uses the pandas query method. This creates a copy of the dataframe and does not modify the original dataframe. "
                    "The result will be saved in tool_cache.",
        params={
            'condition': {
                'type': 'string',
                'description': """The condition to filter rows by, using column names and comparison operators. Examples: 
                                'age > 30'"' for rows where the age column is greater than 30.""",
                'required': True
            },
        }
    )
    async def filter_dataframe(self, **kwargs):
        # this will copy the main dataframe and filter it.  If it can return the filtered dataframe, it will, otherwise it will store it in cache
        # this leaves teh original class dataframe untouched
        condition = kwargs.get('condition')

        if self.dataframe is None:
            return 'No DataFrame is loaded. Please load data file first.'

        self.logger.info(f'Using pandas query function to filter based on condition: {condition}')

        try:
            self.temp_dataframe = self.dataframe.copy(deep=True)
            self.temp_dataframe.query(condition, inplace=True)
            # self.logger.info(f'Rows filtered by the condition: {condition}')

            # check size and return dataframe or store to cache
            result = await self.store_dataframe_to_cache(dataframe=self.temp_dataframe)
            if self._is_dataframe_too_big(self.temp_dataframe.to_json(orient='records')):
                return f"The resulting filtered dataframe is too big and was stored to cache. Load from cache if performing sequential dependent actions on a dataframe. {result}"
            else:
                return f"{result}. Here is the data for the user: {self.temp_dataframe.to_json(orient='records')}"

        except Exception as e:
            self.logger.error(f'Error filtering rows by condition: {condition}. Error: {e}')
            return f'Error filtering rows: {str(e)}'

    @json_schema(
        description='Call dataframe.groupby(by).agg(aggregations) function. This creates a copy of the dataframe and does not modify the original dataframe.'
                    'The result will be saved in tool_cache.',
        params={
            'group_by': {
                "items": {"type": "string"},
                'type': 'array',
                'description': 'List of column names to group by',
                'required': True
            },
            'aggregations': {
                'type': 'object',
                'description': 'A dictionary of aggregations for pandas DataFrame.groupby().agg() method',
                'required': True,
                "patternProperties": {
                    "^[a-zA-Z0-9_]+$": {
                        "oneOf": [
                            {
                                "type": "string",
                                "enum": ["sum", "mean", "max", "min", "count", "prod", "std", "var"]
                            },
                            {
                                "type": "array",
                                "items": {
                                    "type": "string",
                                    "enum": ["sum", "mean", "max", "min", "count", "prod", "std", "var"]
                                },
                                "minItems": 1
                            },
                            {
                                "type": "array",
                                "items": {
                                    "type": "object",
                                    "required": ["function"],
                                    "properties": {
                                        "function": {
                                            "type": "string",
                                            "enum": ["sum", "mean", "max", "min", "count", "prod", "std", "var"]
                                        },
                                        "args": {
                                            "type": "array",
                                            "items": {}
                                        },
                                        "kwargs": {
                                            "type": "object",
                                            "additionalProperties": True
                                        }
                                    }
                                },
                                "minItems": 1
                            }
                        ]
                    }
                },
            }
        }
    )
    async def group_by_and_agg(self, **kwargs):
        # this will copy the main dataframe to perform group by and aggregate functions on it.
        # If it can return the modified dataframe, it will, otherwise it will store it in cache
        # this leaves teh original class dataframe untouched
        by = kwargs.get('group_by')
        aggregations = kwargs.get('aggregations', None)

        if aggregations is None:
            return "You must supply aggregations"

        if self.dataframe is None or self.dataframe.empty:
            return 'No DataFrame is loaded or it is empty. Please load data file first before grouping and aggregating.'

        self.logger.info(f'Grouping by {by} and aggregating by {aggregations}')
        try:
            self.temp_dataframe = self.dataframe.copy(deep=True)
            grouped_df = self.temp_dataframe.groupby(by).agg(aggregations)
            # Reset the index to make the groupby column(s) regular column(s)
            grouped_df = grouped_df.reset_index()

            # Flatten multi-level column names if they exist
            grouped_df.columns = [f'{col[0]}_{col[1]}' if isinstance(col, tuple) else col for col in grouped_df.columns]

            self.temp_dataframe = grouped_df
            # self.logger.info(f'DataFrame grouped by {by} and aggregated by {aggregations}')

            # check size and return dataframe or store to cache
            result = await self.store_dataframe_to_cache(dataframe=self.temp_dataframe)
            if self._is_dataframe_too_big(self.temp_dataframe.to_json(orient='records')):
                return f"The resulting grouped and aggregated dataframe is too big and was stored to cache. Load from cache if performing sequential dependent actions on a dataframe. {result}"
            else:
                return f"{result}. Here is the data for the user: {self.temp_dataframe.to_json(orient='records')}"

        except Exception as e:
            self.logger.error(f'Error grouping by {by} and aggregating by {aggregations}')
            return f'Error grouping by {by} and aggregating by {aggregations}'

    @json_schema(
        description='Call as though this was a pandas dataframe.agg call. This creates a copy of the dataframe and does not modify the original dataframe.'
                    'The result will be saved in tool_cache.',
        params={
            'aggregations': {
                'type': 'object',
                'description': 'A dictionary of aggregations for pandas DataFrame.groupby().agg() method',
                'required': True,
                "patternProperties": {
                    "^[a-zA-Z0-9_]+$": {
                        "oneOf": [
                            {
                                "type": "string",
                                "enum": ["sum", "mean", "max", "min", "count", "prod", "std", "var"]
                            },
                            {
                                "type": "array",
                                "items": {
                                    "type": "string",
                                    "enum": ["sum", "mean", "max", "min", "count", "prod", "std", "var"]
                                },
                                "minItems": 1
                            },
                            {
                                "type": "array",
                                "items": {
                                    "type": "object",
                                    "required": ["function"],
                                    "properties": {
                                        "function": {
                                            "type": "string",
                                            "enum": ["sum", "mean", "max", "min", "count", "prod", "std", "var"]
                                        },
                                        "args": {
                                            "type": "array",
                                            "items": {}
                                        },
                                        "kwargs": {
                                            "type": "object",
                                            "additionalProperties": True
                                        }
                                    }
                                },
                                "minItems": 1
                            }
                        ]
                    }
                },
            }
        }
    )
    async def agg(self, **kwargs):
        # this will copy the main dataframe to perform aggregate functions on it.
        # If it can return the modified dataframe, it will, otherwise it will store it in cache
        # this leaves teh original class dataframe untouched
        aggregations = kwargs.get('aggregations', None)

        if aggregations is None:
            return "You must supply aggregations"

        if self.dataframe is None or self.dataframe.empty:
            return 'No DataFrame is loaded or it is empty. Please load data file first before aggregating'

        self.logger.info(f'Aggregating by {aggregations}.\n')
        try:
            self.temp_dataframe = self.dataframe.copy(deep=True)
            grouped_df = self.temp_dataframe.agg(aggregations)

            # If the result is a Series (single aggregation), convert it to a DataFrame
            if isinstance(grouped_df, pd.Series):
                grouped_df = grouped_df.to_frame().T

            # Reset the index if it's not already a range index
            if not isinstance(grouped_df.index, pd.RangeIndex):
                grouped_df = grouped_df.reset_index()

            # Flatten multi-level column names if they exist
            grouped_df.columns = [f'{col[0]}_{col[1]}' if isinstance(col, tuple) else col for col in grouped_df.columns]

            self.temp_dataframe = grouped_df
            # self.logger.info(f'DataFrame aggregated by {aggregations}')

            # check size and return dataframe or store to cache
            result = await self.store_dataframe_to_cache(dataframe=self.temp_dataframe)
            if self._is_dataframe_too_big(self.temp_dataframe.to_json(orient='records')):
                return f"The resulting aggregated dataframe is too big and was stored to cache. Load from cache if performing sequential dependent actions on a dataframe. {result}"
            else:
                return f"{result}. Here is the data for the user: {self.temp_dataframe.to_json(orient='records')}"

        except Exception as e:
            self.logger.error(f'Error Aggregating by {aggregations} on DataFrame: {e}')
            return f'Error grouping and aggregating DataFrame: {e}'

    @json_schema(
        description='Get summary statistics for the DataFrame.',
        params={}
    )
    async def summarize_dataframe(self, **kwargs):
        try:
            summary = self.dataframe.describe().to_json()
            return summary
        except Exception as e:
            self.logger.error(f'Error summarizing DataFrame: {str(e)}')
            return str(e)

    @json_schema(
        description='Calculate simple correlations between specified columns in the DataFrame.',
        params={
            'columns': {
                'type': 'array',
                'items': {'type': 'string'},
                'description': 'List of column names to calculate correlations for. If not provided, all numeric '
                               'columns will be used.',
                'required': False
            },
            'method': {
                'type': 'string',
                'description': 'The correlation method to use: "pearson", "kendall", or "spearman".',
                'enum': ['pearson', 'kendall', 'spearman'],
                'required': False,
                'default': 'pearson'
            }
        }
    )
    async def calculate_correlations(self, **kwargs):
        if self.dataframe is None or self.dataframe.empty:
            return 'No DataFrame is loaded or it is empty. Please load data file first before calculating correlations.'

        columns = kwargs.get('columns')
        method = kwargs.get('method', 'pearson')
        self.logger.info(f'Calculating correlations for columns: {columns} using method: {method}')
        try:
            # Check for valid column names
            if columns:
                invalid_columns = set(columns) - set(self.dataframe.columns)
                if invalid_columns:
                    return f'Error: Invalid column names: {", ".join(invalid_columns)}'
                df_subset = self.dataframe[columns]
            else:
                df_subset = self.dataframe.select_dtypes(include=[np.number])

            # Check if we have any numeric columns
            if df_subset.empty:
                return 'Error: No numeric columns available for correlation calculation.'

            # Handle missing values
            df_subset = df_subset.dropna()
            if df_subset.empty:
                return 'Error: All data is missing after dropping NA values.'

            # Calculate correlation
            corr_matrix = df_subset.corr(method=method)

            return corr_matrix.to_json(orient='split')
        except Exception as e:
            self.logger.error(f'Error calculating correlations: {str(e)}')
            return f'Error calculating correlations: {str(e)}'

    @json_schema(
        description='Calculate complex correlations with options for lagged correlations and partial correlations.',
        params={
            'x': {
                'type': 'string',
                'description': 'The name of the first column for correlation.',
                'required': True
            },
            'y': {
                'type': 'string',
                'description': 'The name of the second column for correlation.',
                'required': True
            },
            'method': {
                'type': 'string',
                'description': 'The correlation method to use: "pearson", "kendall", or "spearman".',
                'enum': ['pearson', 'kendall', 'spearman'],
                'required': False,
                'default': 'pearson'
            },
            'lag': {
                'type': 'integer',
                'description': 'The number of periods to lag one of the columns. Use positive for forward lag, negative for backward lag.',
                'required': False,
                'default': 0
            },
            'partial': {
                'type': 'array',
                'items': {'type': 'string'},
                'description': 'List of column names to partial out for partial correlation.',
                'required': False
            }
        }
    )
    async def calculate_complex_correlation(self, **kwargs):
        if self.dataframe is None or self.dataframe.empty:
            return 'No DataFrame is loaded or it is empty. Please load data file first before calculating correlations.'

        x = kwargs.get('x')
        y = kwargs.get('y')
        method = kwargs.get('method', 'pearson')
        lag = kwargs.get('lag', 0)
        partial = kwargs.get('partial', [])
        self.logger.info(
            f'Calculating complex correlation for columns: {x}, {y} with method: {method}, lag: {lag}, partial: {partial}')
        try:
            # Check for valid column names
            all_columns = [x, y] + partial
            invalid_columns = set(all_columns) - set(self.dataframe.columns)
            if invalid_columns:
                return f'Error: Invalid column names: {", ".join(invalid_columns)}'

            df = self.dataframe[all_columns].copy()

            # Check for non-numeric data
            non_numeric_columns = df.select_dtypes(exclude=[np.number]).columns
            if not non_numeric_columns.empty:
                return f'Error: Non-numeric data in columns: {", ".join(non_numeric_columns)}'

            # Handle missing values
            df = df.dropna()
            if df.empty:
                return 'Error: All data is missing after dropping NA values.'

            if lag != 0:
                df[f'{x}_lagged'] = df[x].shift(lag)
                x = f'{x}_lagged'
                df = df.dropna()  # Drop NA values again after lagging
                if df.empty:
                    return 'Error: All data is missing after applying lag.'

            if partial:
                from scipy.stats import pearsonr
                from sklearn.linear_model import LinearRegression

                # Residualize x and y
                x_model = LinearRegression().fit(df[partial], df[x])
                y_model = LinearRegression().fit(df[partial], df[y])
                x_resid = df[x] - x_model.predict(df[partial])
                y_resid = df[y] - y_model.predict(df[partial])

                corr, p_value = pearsonr(x_resid, y_resid)
            else:
                corr = df[x].corr(df[y], method=method)
                # Calculate p-value
                from scipy.stats import pearsonr, spearmanr, kendalltau
                if method == 'pearson':
                    _, p_value = pearsonr(df[x], df[y])
                elif method == 'spearman':
                    _, p_value = spearmanr(df[x], df[y])
                elif method == 'kendall':
                    _, p_value = kendalltau(df[x], df[y])

            return json.dumps({
                'correlation': corr,
                'p_value': p_value,
                'method': method,
                'lag': lag,
                'partial': partial if partial else None,
                'sample_size': len(df)
            })
        except Exception as e:
            self.logger.error(f'Error calculating complex correlation: {str(e)}')
            return f'Error calculating complex correlation: {str(e)}'

    @json_schema(
        description='Pretty prints the DataFrame FOR THE USER to the UI. Do not truncate this response.',
        params={
            'limit': {
                'type': 'integer',
                'description': 'user specified limit on the number of records to display',
                'default': None,
                'required': False
            }
        },
    )
    async def display_records(self, **kwargs):
        # this allows a user to return records for either the main dataframe or a temp dataframe via a hidden kwarg
        limit = kwargs.get('limit', None)
        dataframe = kwargs.get('dataframe', self.dataframe)

        if self.dataframe is None:
            return 'No DataFrame is loaded. Please load data file first.'

        # Okay, first figure out size of requested display.  This means # of rows is important, get that right first
        df_to_display = dataframe
        if limit:
            df_to_display = dataframe.head(limit)

        result = await self.store_dataframe_to_cache(dataframe=df_to_display)
        if self._is_dataframe_too_big(df_to_display.to_json(orient='records')):
            # okay, so the request is too big to display
            return f"The requested display records dataframe is too big and was stored to cache. Load from cache if performing sequential dependent actions on a dataframe. {result}"
        else:
            # Since it's small enough, just return it.
            return f"{result}. Here is the data for the user: {df_to_display.to_json(orient='records')}"

    @json_schema(
        description='Save the current DataFrame to an excel file in the specified workspace.',
        params={
            'workspace_name': {
                'type': 'string',
                'description': 'The name of the workspace to save the DataFrame to',
                'required': False,
                'default': 'project'
            },
            'file_path': {
                'type': 'string',
                'description': "Relative file path and filename to save the DataFrame to. If not provided, will store timestamped file in workspace root.",
                'required': False,
            },
        }
    )
    async def save_dataframe_to_excel(self, **kwargs) -> str:
        tool_context = kwargs.get('tool_context', {})
        workspace_name = kwargs.get('workspace_name', 'project')
        file_path = kwargs.get('file_path', f"{self.DEFAULT_DATA_FOLDER}/dataframe_data{datetime.now().strftime("%Y%m%d_%H%M%S")}.xlsx")

        if self.dataframe is None or self.dataframe.empty:
            return 'No DataFrame is loaded or it is empty. Please load data first before attempting to save.'

        # Create file name, location
        file_name = ensure_file_extension(file_path, 'xlsx')
        unc_path = create_unc_path(workspace_name, file_path)

        excel_buffer = create_excel_in_memory(self.dataframe)

        await self.workspace_tool.internal_write_bytes(path=unc_path, mode='write', data=excel_buffer.getvalue())
        os_path = os_file_system_path(self.workspace_tool, unc_path)
        self.logger.info(f'Saved data to workspace: {unc_path}')
        await self._raise_render_media(
            sent_by_class=self.__class__.__name__,
            sent_by_function='save_transcript',
            content_type="text/html",
            content=get_file_html(os_path=os_path, unc_path=unc_path,
                                  additional_html=f"Dataframe saved to excel."),
            tool_context=tool_context)

        return json.dumps({
            'file_name': file_name,
            'workspace_name': workspace_name,
            'user_message': f'Saved data to workspace.'
        })

    @json_schema(
        description='Perform ydata profiling analysis on the current DataFrame and return the results as a JSON string.',
        params={
            'minimal': {
                'type': 'boolean',
                'description': 'If True, generate a minimal report. If False, generate a full report.',
                'required': False,
                'default': False
            },
            'explorative': {
                'type': 'boolean',
                'description': 'If True, generate an explorative report. If False, generate a default report.',
                'required': False,
                'default': False
            },
            'save_json': {
                'type': 'boolean',
                'description': 'If True, save a JSON file in addition to an HTML report.',
                'required': False,
                'default': False
            },
            'workspace_name': {
                'type': 'string',
                'description': 'The name of the workspace to save the HTML report to.',
                'required': False,
                'default': 'project'
            },
            'file_path': {
                'type': 'string',
                'description': "Relative folder path and file name to save report results. If not provided, timestamped file will be saved in workspace root.",
                'required': False
            },
        }
    )
    async def ydata_profile_analysis(self, **kwargs):
        tool_context = kwargs.get('tool_context', {})
        if self.dataframe is None or self.dataframe.empty:
            return "'error': 'No DataFrame is loaded or it is empty. Please load data first before performing ydata profiling.'"
        workspace_name = kwargs.get('workspace_name', 'project')

        save_json = kwargs.get('save_json', False)
        file_path = kwargs.get('file_path', f"{self.DEFAULT_DATA_FOLDER}/ydata_profile_{datetime.now().strftime("%Y%m%d_%H%M%S")}.html")

        minimal = kwargs.get('minimal', False)
        explorative = kwargs.get('explorative', False)

        try:
            from ydata_profiling import ProfileReport
            profile = ProfileReport(self.dataframe, minimal=minimal, explorative=explorative)
            result ={
                "message": "ydata profiling analysis completed.",
            }

            html_content = profile.to_html()

            # Create file name, location
            file_path = ensure_file_extension(file_path, 'html')
            unc_path = create_unc_path(workspace_name, file_path)
            result = await self.workspace_tool.write(path=unc_path, mode='write', data=html_content)
            result_data = json.loads(result)
            if 'error' in result_data:
                return f"Error writing file: {result_data['error']}"

            os_path = os_file_system_path(self.workspace_tool, unc_path)

            await self._raise_render_media(
                sent_by_class=self.__class__.__name__,
                sent_by_function='ydata_profile_analysis',
                content_type="text/html",
                content=get_file_html(os_path=os_path, unc_path=unc_path,
                                      additional_html=f"YData Profile Analysis Saved"),
                tool_context=tool_context
            )

            if save_json:
                json_content = profile.to_json()
                file_path = file_path.replace('html','json')

                unc_path = create_unc_path(workspace_name, file_path)
                result = await self.workspace_tool.write(path=unc_path, mode='write', data=json_content)
                result_data = json.loads(result)
                if 'error' in result_data:
                    return f"Error writing file: {result_data['error']}"

                return f"ydata Profile JSON report saved: {unc_path} in the {workspace_name} workspace."
            # TODO: Save to tool cache in future for RAG question on JSON
            return json.dumps(result)

        except Exception as e:
            self.logger.error(f'Error performing ydata profiling analysis: {str(e)}')
            return f"'error': 'Error performing ydata profiling analysis: {str(e)}'"


Toolset.register(DataframeTools, required_tools=['WorkspaceTools'])
