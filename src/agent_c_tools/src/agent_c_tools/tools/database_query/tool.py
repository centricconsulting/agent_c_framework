import json
import sqlite3
import sqlparse
import pandas as pd
from typing import List, Dict, Any, Union
from sqlparse.sql import IdentifierList, Identifier, Function
from sqlparse.tokens import Keyword

from agent_c.toolsets import Toolset, json_schema
from ...helpers.path_helper import create_unc_path, os_file_system_path


class DatabaseQueryTools(Toolset):
    DEMO_DB_NAME = 'demo_agent_data.db'
    DATA_FOLDER = 'data_demo'
    ALLOWED_FUNCTIONS = {
        'COUNT', 'SUM', 'AVG', 'MIN', 'MAX', 'GROUP_CONCAT',
        'TOTAL', 'AVG', 'COUNT', 'GROUP_CONCAT', 'MAX', 'MIN',
        'SUM', 'RANDOM', 'ABS', 'INSTR', 'LOWER', 'UPPER',
        'LENGTH', 'LTRIM', 'RTRIM', 'TRIM', 'REPLACE', 'ROUND',
        'TYPEOF'
    }
    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='database_query')
        self.workspace_tool = self.tool_chest.active_tools.get('WorkspaceTools')
        self.db_path = None
        self.dataframe_tool = self.tool_chest.active_tools.get('DataframeTools')

    def _validate_subqueries_and_functions(self, stmt):
        for token in stmt.tokens:
            if isinstance(token, IdentifierList):
                for identifier in token.get_identifiers():
                    if isinstance(identifier, Function):
                        if identifier.get_name().upper() not in self.allowed_functions:
                            return f"Unsupported function: {identifier.get_name()}"
            elif isinstance(token, Identifier):
                if isinstance(token.tokens[0], Function):
                    if token.tokens[0].get_name().upper() not in self.allowed_functions:
                        return f"Unsupported function: {token.tokens[0].get_name()}"
            elif token.ttype is Keyword:
                if token.value.upper() == 'SELECT':
                    # This could be a subquery
                    subquery = token.parent
                    error = self._validate_subqueries_and_functions(subquery)
                    if error:
                        return error
        return None

    def _validate_sql_query(self, query: str) -> Union[str, None]:
        # Parse the SQL query
        parsed = sqlparse.parse(query)
        if not parsed:
            return "Empty or invalid SQL query."

        # Get the first statement (we'll only validate the first one)
        stmt = parsed[0]

        # Check if it's a SELECT statement
        if stmt.get_type() != 'SELECT':
            return "Only SELECT statements are allowed for read-only operations."

        # Check for potential SQL injection patterns
        lower_query = query.lower()
        if any(keyword in lower_query for keyword in ['insert', 'update', 'delete', 'drop', 'truncate', 'alter']):
            return "Potential SQL injection detected. Query contains forbidden keywords."

        # Check for multiple statements
        if len(parsed) > 1:
            return "Multiple SQL statements are not allowed."

        # Validate subqueries and functions
        error = self._validate_subqueries_and_functions(stmt)
        if error:
            return error

        # If we've passed all checks, return None (indicating no issues)
        return None

    def _load_to_dataframe(self, results: List[Dict[str, Any]]) -> str:
        try:
            df = pd.DataFrame(results)
            self.dataframe_tool.dataframe = df
            return f"Data successfully loaded into DataframeTools. {self.dataframe_tool._standardized_result()}"
        except Exception as e:
            return f"Error loading data into DataframeTools: {str(e)}"

    def _execute_query(self, db_path: str, query: str) -> List[Dict[str, Any]]:
        with sqlite3.connect(db_path) as conn:
            conn.row_factory = sqlite3.Row
            cursor = conn.cursor()
            cursor.execute(query)
            results = [dict(row) for row in cursor.fetchall()]
        return results

    async def _set_db_path(self, workspace_name: str, file_path: str) -> str:
        unc_path = create_unc_path(workspace_name, file_path)

        error, workspace, relative_path = self.workspace_tool.validate_and_get_workspace_path(unc_path)
        if error:
            return f"Invalid path: {error}"

        self.db_path = os_file_system_path(self.workspace_tool, unc_path)

        return self.db_path

    @json_schema(
        description="Execute a read-only SQL query on the specified database.",
        params={
            'workspace_name': {
                'type': 'string',
                'description': 'The name of the workspace to save the transcript.',
                'required': False,
                'default': 'project'
            },
            'file_path': {
                'type': 'string',
                'description': 'Relative path the database file in the workspace. If not provided, a demo database will be used.',
                'required': False
            },
            'query': {
                'type': 'string',
                'description': 'The SQL query to execute (SELECT statements only).',
                'required': True
            },
            'load_to_dataframe': {
                'type': 'boolean',
                'description': 'If true, loads the query results into the DataframeTools.',
                'required': False,
                'default': False
            }
        }
    )
    async def execute_query(self, **kwargs) -> str:
        try:
            workspace_name = kwargs.get('workspace_name', 'project')
            file_path = kwargs.get('file_path', f"{self.DATA_FOLDER}/{self.DEMO_DB_NAME}")

            if self.db_path is None:
                await self._set_db_path(workspace_name,file_path)

            load_to_dataframe = kwargs.get('load_to_dataframe', False)

            query = kwargs['query'].strip()

            if not query.lower().startswith('select'):
                return json.dumps({"error": "Only SELECT queries are allowed for read-only operations."})

            # Validate the SQL query
            validation_error = self._validate_sql_query(query)
            if validation_error:
                return json.dumps({"error": validation_error})

            results = self._execute_query(self.db_path, query)

            message = f"Query executed successfully on {self.DEMO_DB_NAME}. {len(results)} rows returned."

            if load_to_dataframe:
                df_message = self._load_to_dataframe(results)
                message += f" {df_message}"

            return json.dumps({
                "results": results if not load_to_dataframe else "Data loaded to DataframeTools",
                "message": message
            })
        except Exception as e:
            return json.dumps({"error": f"Error executing query: {str(e)}"})

    @json_schema(
        description="Get the list of tables in the specified database.",
        params={
            'workspace_name': {
                'type': 'string',
                'description': 'The name of the workspace to save the transcript.',
                'required': False,
                'default': 'project'
            },
            'file_path': {
                'type': 'string',
                'description': 'Relative path the database file in the workspace. If not provided, a demo database will be used.',
                'required': False
            },
        }
    )
    async def get_tables(self, **kwargs) -> str:
        try:
            workspace_name = kwargs.get('workspace_name', 'project')
            file_path = kwargs.get('file_path', f"{self.DATA_FOLDER}/{self.DEMO_DB_NAME}")

            if self.db_path is None:
                await self._set_db_path(workspace_name, file_path)

            query = "SELECT name FROM sqlite_master WHERE type='table';"
            results = self._execute_query(self.db_path, query)

            tables = [row['name'] for row in results]

            return json.dumps({
                "tables": tables,
                "message": f"Successfully retrieved {len(tables)} tables."
            })
        except Exception as e:
            return json.dumps({"error": f"Error retrieving tables: {str(e)}"})

    @json_schema(
        description="Get the schema of a specified table in the database.",
        params={
            'workspace_name': {
                'type': 'string',
                'description': 'The name of the workspace to save the transcript.',
                'required': False,
                'default': 'project'
            },
            'file_path': {
                'type': 'string',
                'description': 'Relative path the database file in the workspace. If not provided, a demo database will be used.',
                'required': False
            },
            'table_name': {
                'type': 'string',
                'description': 'Name of the table to get the schema for.',
                'required': True
            }
        }
    )
    async def get_table_schema(self, **kwargs) -> str:
        try:
            workspace_name = kwargs.get('workspace_name', 'project')
            file_path = kwargs.get('file_path', f"{self.DATA_FOLDER}/{self.DEMO_DB_NAME}")
            if self.db_path is None:
                await self._set_db_path(workspace_name, file_path)

            table_name = kwargs['table_name']

            query = f"PRAGMA table_info('{table_name}');"
            results = self._execute_query(self.db_path, query)

            schema = [{
                "column_name": row['name'],
                "data_type": row['type'],
                "is_nullable": not row['notnull'],
                "is_primary_key": bool(row['pk'])
            } for row in results]

            return json.dumps({
                "schema": schema,
                "message": f"Successfully retrieved schema for table '{table_name}' from {self.DEMO_DB_NAME}."
            })
        except Exception as e:
            return json.dumps({"error": f"Error retrieving table schema: {str(e)}"})


Toolset.register(DatabaseQueryTools, required_tools=['WorkspaceTools', 'DataframeTools'])