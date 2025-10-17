from datetime import datetime
import os
import subprocess
import sys
import time

import mysql.connector
import json
import pandas as pd
import decimal
import asyncio
import logging
import yaml
from typing import Dict, Any, List, Optional, Union

import sqlparse
from sqlparse.sql import IdentifierList, Identifier, Function
from sqlparse.tokens import Keyword, DML

from agent_c.toolsets.tool_set import Toolset
from agent_c.toolsets.json_schema import json_schema
from ...helpers.media_file_html_helper import get_file_html
from ..workspace import WorkspaceTools
from ...helpers.dataframe_in_memory import create_excel_in_memory
from ...helpers.path_helper import ensure_file_extension, create_unc_path, os_file_system_path


# Custom JSON encoder to handle Decimal types
class DecimalEncoder(json.JSONEncoder):
    """JSON encoder that handles Decimal types by converting them to float."""
    def default(self, obj):
        if isinstance(obj, decimal.Decimal):
            return float(obj)
        return super(DecimalEncoder, self).default(obj)


class MariaDBConnector:
    """
    Helper class for connecting to and querying a MariaDB database.
    This is the core database logic separated from the toolset interface.
    """
    # Configuration based on the HeidiSQL settings
    DEFAULT_CONFIG = {
        'host': '127.0.0.1',
        'port': 3306,
        'user': 'root',
        'password': '',  # Empty password as shown in the User Manager
        'database': 'auraroleplay'  # Default database
    }

    ALLOWED_FUNCTIONS = {
        'COUNT', 'SUM', 'AVG', 'MIN', 'MAX', 'GROUP_CONCAT',
        'TOTAL', 'AVG', 'COUNT', 'GROUP_CONCAT', 'MAX', 'MIN',
        'SUM', 'RANDOM', 'ABS', 'INSTR', 'LOWER', 'UPPER',
        'LENGTH', 'LTRIM', 'RTRIM', 'TRIM', 'REPLACE', 'ROUND',
        'TYPEOF'
    }

    def __init__(self, **kwargs):
        """
        Initialize the MariaDBConnector with connection parameters.
        Uses default settings if none provided.

        Args:
            **kwargs: Connection parameters to override defaults
        """
        # Override defaults with any provided kwargs
        self.config = self.DEFAULT_CONFIG.copy()
        self.config.update(kwargs)


    def _validate_subqueries_and_functions(self, stmt):
        """
        Validate SQL subqueries and functions to prevent SQL injection.
        """
        for token in stmt.tokens:
            if isinstance(token, IdentifierList):
                for identifier in token.get_identifiers():
                    if isinstance(identifier, Function):
                        if identifier.get_name().upper() not in self.ALLOWED_FUNCTIONS:
                            return f"Unsupported function: {identifier.get_name()}"
            elif isinstance(token, Identifier):
                if isinstance(token.tokens[0], Function):
                    if token.tokens[0].get_name().upper() not in self.ALLOWED_FUNCTIONS:
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
        """
        Validate an SQL query to ensure it's safe to execute.
        Returns None if valid, or an error message if invalid.
        """
        # Parse the SQL query
        parsed = sqlparse.parse(query)
        if not parsed:
            return "Empty or invalid SQL query."

        # Get the first statement
        stmt = parsed[0]

        # Check if it's a SELECT statement
        if stmt.get_type() != 'SELECT':
            return "Only SELECT statements are allowed for read-only operations."

        # Check for potential SQL injection patterns
        lower_query = query.lower()
        if any(keyword in lower_query for keyword in 
               ['insert', 'update', 'delete', 'drop', 'truncate', 'alter']):
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

    def connect(self):
        """
        Establish a connection to the MariaDB database.
        """
        try:
            conn_config = self.config.copy()
            conn_config['use_pure'] = True
            safe_config = {k: v for k, v in conn_config.items() if k != 'password'}
            self.logger.debug(f"Attempting connection with: {safe_config}")
            conn = mysql.connector.connect(**conn_config)
            return conn
        except mysql.connector.Error as e:
            error_msg = f"Error connecting to MariaDB: {e}"
            self.logger.error(error_msg)
            raise Exception(error_msg)
        except Exception as e:
            error_msg = f"Unexpected connection error: {e}"
            self.logger.error(error_msg)
            raise Exception(error_msg)

    async def execute_query(self, query: str):
        """
        Execute a read-only SQL query on the database.

        Args:
            query: SQL query to execute (SELECT statements only)

        Returns:
            Query results
        """
        try:
            # Validate the SQL query
            query = query.strip()
            if not query.lower().startswith('select'):
                return {"error": "Only SELECT queries are allowed for read-only operations."}

            validation_error = self._validate_sql_query(query)
            if validation_error:
                return {"error": validation_error}

            # Execute the query
            conn = self.connect()
            cursor = conn.cursor(dictionary=True)
            cursor.execute(query)
            results = cursor.fetchall()
            cursor.close()
            conn.close()

            self.logger.debug(f"Query executed successfully. {len(results)} rows returned.")

            return results
        except Exception as e:
            error_msg = f"Error executing query: {str(e)}"
            self.logger.error(error_msg)
            return {"error": error_msg}

    async def get_tables(self) -> Dict[str, Any]:
        """
        Get a list of all tables in the database.

        Returns:
            Dictionary containing list of tables and status message
        """
        try:
            conn = self.connect()
            cursor = conn.cursor()
            cursor.execute("SHOW TABLES;")
            results = cursor.fetchall()
            cursor.close()
            conn.close()

            # Extract table names from results (SHOW TABLES returns a list of tuples)
            tables = [row[0] for row in results]

            return {
                "tables": tables,
                "message": f"Successfully retrieved {len(tables)} tables."
            }
        except Exception as e:
            error_msg = f"Error retrieving tables: {str(e)}"
            self.logger.error(error_msg)
            return {"error": error_msg}

    async def get_table_schema(self, table_name: str) -> Dict[str, Any]:
        """
        Get the schema details for a specific table.

        Args:
            table_name: Name of the table to get schema for

        Returns:
            Dictionary with table schema information
        """
        try:
            conn = self.connect()
            cursor = conn.cursor(dictionary=True)

            # Get table columns
            cursor.execute(f"DESCRIBE `{table_name}`;")
            columns = cursor.fetchall()

            # Get primary keys
            cursor.execute(f"""
                SELECT k.COLUMN_NAME
                FROM information_schema.table_constraints t
                JOIN information_schema.key_column_usage k
                USING(constraint_name,table_schema,table_name)
                WHERE t.constraint_type='PRIMARY KEY'
                AND t.table_schema=DATABASE()
                AND t.table_name='{table_name}';
            """)
            primary_keys = [row['COLUMN_NAME'] for row in cursor.fetchall()]

            cursor.close()
            conn.close()

            # Format the schema in a more readable way
            schema = [{
                "column_name": col['Field'],
                "data_type": col['Type'],
                "is_nullable": col['Null'] == 'YES',
                "is_primary_key": col['Field'] in primary_keys,
                "default": col['Default'],
                "extra": col['Extra']
            } for col in columns]

            return {
                "schema": schema,
                "message": f"Successfully retrieved schema for table '{table_name}'."
            }
        except Exception as e:
            error_msg = f"Error retrieving table schema: {str(e)}"
            self.logger.error(error_msg)
            return {"error": error_msg}

    async def get_database_info(self) -> Dict[str, Any]:
        """
        Get general information about the database server and connection.

        Returns:
            Dictionary with database server information
        """
        try:
            conn = self.connect()
            cursor = conn.cursor(dictionary=True)

            # Server version
            cursor.execute("SELECT VERSION() as version;")
            version = cursor.fetchone()['version']

            # Server variables
            cursor.execute("SHOW VARIABLES LIKE 'character_set_database';")
            charset = cursor.fetchone()['Value']

            cursor.execute("SHOW VARIABLES LIKE 'collation_database';")
            collation = cursor.fetchone()['Value']

            # Database size
            cursor.execute("""
                SELECT 
                    table_schema as 'database',
                    ROUND(SUM(data_length + index_length) / 1024 / 1024, 2) as 'size_mb'
                FROM information_schema.tables
                WHERE table_schema = DATABASE()
                GROUP BY table_schema;
            """)
            size_info = cursor.fetchone()

            cursor.close()
            conn.close()

            return {
                "database_name": self.config['database'],
                "server_version": version,
                "character_set": charset,
                "collation": collation,
                "size_mb": size_info['size_mb'] if size_info else None,
                "host": self.config['host'],
                "user": self.config['user'],
                "message": "Successfully retrieved database information."
            }
        except Exception as e:
            error_msg = f"Error retrieving database info: {str(e)}"
            self.logger.error(error_msg)
            return {"error": error_msg}

    def to_json(self, data):
        """
        Convert data to JSON string, handling Decimal types
        """
        return json.dumps(data, cls=DecimalEncoder, indent=2)

    @staticmethod
    def launch_heidi(heidi_path=None):
        """
        Attempt to launch HeidiSQL if it's not already running

        Args:
            heidi_path: Path to HeidiSQL executable. If None, tries common locations.
        """
        # Common paths where HeidiSQL might be installed
        common_paths = [
            r"C:\Program Files\HeidiSQL\heidisql.exe",
            r"C:\Program Files (x86)\HeidiSQL\heidisql.exe",
        ]

        if heidi_path:
            common_paths.insert(0, heidi_path)

        # Check if HeidiSQL is already running
        if sys.platform == 'win32':
            result = subprocess.run(["tasklist", "/FI", "IMAGENAME eq heidisql.exe"],
                                    capture_output=True, text=True)
            if "heidisql.exe" in result.stdout:
                print("HeidiSQL is already running.")
                return True

        # Try to launch HeidiSQL
        for path in common_paths:
            if os.path.exists(path):
                try:
                    subprocess.Popen([path])
                    print(f"Launched HeidiSQL from {path}")
                    # Give HeidiSQL time to start
                    time.sleep(2)
                    return True
                except Exception as e:
                    print(f"Error launching HeidiSQL: {e}")

        print("Could not find or launch HeidiSQL. Please launch it manually.")
        return False


class MariadbTools(Toolset):
    """
    Toolset for interacting with a MariaDB database.
    Provides tools for querying, exploring schema, and managing database connections.
    """
    def __init__(self, **kwargs):
        """
        Initialize the AuraDevTools toolset.
        
        Args:
            **kwargs: Passed to parent constructor
        """
        super().__init__(**kwargs, name="mariadb_tools", description="Tools for interacting with MariaDB databases")
        
        # Initialize the database connector
        self.connector: Optional[MariaDBConnector] = None

        # self.logger.debug(f"MariaDBTools initialized with streaming_callback: {self.streaming_callback is not None}")
        # Initialize connection parameters from environment or config
        self.connection_params = {}

        self.workspace_tool: Optional['WorkspaceTools'] = None

    async def post_init(self):
        # Get the workspace tool after all dependencies set up
        self.workspace_tool = self.tool_chest.available_tools.get("WorkspaceTools")

        # Get tool cached connections when tool_cache is guaranteed to be available
        self._check_cached_connection()

    def _check_cached_connection(self):
        """Check for cached connection parameters and load them if available"""
        cached_params = self.tool_cache.get("maria_connection_params")
        if cached_params:
            self.connection_params = cached_params
            self.logger.info("Loaded cached database connection parameters")
        
    async def _get_connector(self):
        """Get or create a database connector with current connection parameters"""
        if not self.connector:
            self.connector = MariaDBConnector(**self.connection_params)
        if self.connector is None:
            return {
                "status": "error",
                "message": "Database connection not initialized. Call connect_database first."
            }
        return self.connector

    @json_schema(
        description="Initialize a connection to a MariaDB database.  Uses hardcoded defaults if not provided.",
        params={
            "host": {
                "type": "string",
                "description": "Database host address",
                "required": False,
                "default": "127.0.0.1"
            },
            "port": {
                "type": "integer",
                "description": "Database port number",
                "required": False,
                "default": 3306
            },
            "user": {
                "type": "string",
                "description": "Database username",
                "required": False,
                "default": "root"
            },
            "password": {
                "type": "string",
                "description": "Database password",
                "required": False,
                "default": ""
            },
            "database": {
                "type": "string",
                "description": "Database name",
                "required": False,
                "default": "auraroleplay"
            },
            "save_connection": {
                "type": "boolean",
                "description": "Whether to save connection parameters for future use",
                "required": False,
                "default": False
            }
        }
    )
    async def connect_database(self, **kwargs) -> str:
        """
        Initialize a connection to the MariaDB database.
        
        Args:
            kwargs:
                host: Database host address (default: 127.0.0.1)
                port: Database port number (default: 3306)
                user: Database username (default: root)
                password: Database password (default: empty string)
                database: Database name (default: auraroleplay)
                save_connection: Whether to save connection parameters for future use
            
        Returns:
            String with connection status and database info
        """
        try:
            # Update connection parameters with non-None values
            params = {
                'host': kwargs.get('host') or MariaDBConnector.DEFAULT_CONFIG['host'],
                'port': kwargs.get('port') or MariaDBConnector.DEFAULT_CONFIG['port'],
                'user': kwargs.get('user') or MariaDBConnector.DEFAULT_CONFIG['user'],
                'password': kwargs.get('password') or MariaDBConnector.DEFAULT_CONFIG['password'],
                'database': kwargs.get('database') or MariaDBConnector.DEFAULT_CONFIG['database']
            }
            
            self.connection_params = params
            
            # Save connection params if requested
            if kwargs.get('save_connection', False):
                self.tool_cache.set("maria_connection_params", params)
                
            # Create a new connector with updated parameters
            self.connector = MariaDBConnector(**params)
            
            # Test the connection by getting database info
            db_info = await self.connector.get_database_info()
            
            if "error" in db_info:
                return f"ERROR: {db_info['error']}"
                
            result = {
                "status": "success", 
                "message": f"Successfully connected to {params['database']} database on {params['host']}",
                "database_info": db_info
            }
            
            return yaml.dump(result, allow_unicode=True)
            
        except Exception as e:
            error_msg = f"Failed to connect to database: {str(e)}"
            self.logger.error(error_msg)
            return f"ERROR: {error_msg}"

    @json_schema(
        description="Execute a SQL query on the MariaDB database",
        params={
            "query": {
                "type": "string",
                "description": "SQL query to execute (SELECT statements only)",
                "required": True
            },
            'workspace_name': {
                'type': 'string',
                'description': 'The name of the workspace to save records if desired.',
                'required': False,
                'default': 'project'
            },
            'file_path': {
                'type': 'string',
                'description': 'Relative path to save the file in the workspace. If not provided, default name will be used.',
                'required': False
            },
            'force_save': {
                'type': 'boolean',
                'description': 'Flag force saving file.',
                'required': False,
                'default': False
            },
        }
    )
    async def execute_query(self, **kwargs) -> str:
        """
        Execute a SQL query on the MariaDB database.
        
        Args:
            kwargs:
                query: SQL query to execute (SELECT statements only)
                workspace_name: The name of the workspace to save records if desired
                file_path: Relative path to save the file in the workspace
                force_save: Flag force saving file

        Returns:
            String with query results and status
        """
        try:
            query = kwargs.get("query")
            workspace_name = kwargs.get("workspace_name", "project")
            file_path = kwargs.get('file_path', f'mariadb_query_results_{datetime.now().strftime("%Y%m%d_%H%M%S")}.xlsx')
            force_save = kwargs.get("force_save", False)
            tool_context = kwargs.get('tool_context', None)

            if not query:
                return "ERROR: Query cannot be empty."

            if self.connector is None:
                await self._get_connector()
            
            # Run the query in a thread pool to avoid blocking
            result = await self.connector.execute_query(query)

            # Check for errors in result
            if isinstance(result, dict) and "error" in result:
                return f"ERROR: {result['error']}"

            await self._raise_render_media(
                sent_by_class=self.__class__.__name__,
                sent_by_function='execute_query',
                content_type="text/html",
                content="<b>Query executed successfully</b>",
                tool_context=tool_context
            )

            if force_save:
                try:
                    df = pd.DataFrame(result)
                    if tool_context is None or 'agent_runtime' not in tool_context:
                        self.logger.debug("No tool_context or agent_runtime available, cannot check token count for response size.")
                        force_save = True
                    else:
                        response_size = tool_context['agent_runtime'].count_tokens(df.to_json())
                        force_save = True if response_size > 25000 else force_save

                    if force_save:
                        file_path = ensure_file_extension(file_path, 'xlsx')
                        unc_path = create_unc_path(workspace_name, file_path)

                        excel_buffer = create_excel_in_memory(df)

                        save_result = await self.workspace_tool.internal_write_bytes(
                            path=unc_path,
                            mode='write',
                            data=excel_buffer.getvalue()
                        )
                        os_path = os_file_system_path(self.workspace_tool, unc_path)

                        await self._raise_render_media(
                            sent_by_class=self.__class__.__name__,
                            sent_by_function='execute_query',
                            content_type="text/html",
                            content=get_file_html(os_path, unc_path),
                            tool_context=tool_context,
                        )
                        self.logger.debug(save_result)
                except Exception as df_error:
                    self.logger.debug(f"Error converting result to DataFrame: {df_error}")
                    # Continue with normal result return even if save fails
            
            return yaml.dump(result, allow_unicode=True)
            
        except Exception as e:
            error_msg = f"Error executing query: {str(e)}"
            self.logger.error(error_msg)
            return f"ERROR: {error_msg}"

    @json_schema(
        description="Get a list of all tables in the MariaDB database",
        params={}
    )
    async def get_tables(self, **kwargs) -> str:
        """
        Get a list of all tables in the MariaDB database.
        
        Returns:
            String with list of tables and status
        """
        try:
            if self.connector is None:
                await self._get_connector()
                
            # Run in thread pool to avoid blocking
            result = await self.connector.get_tables()
            
            if "error" in result:
                return f"ERROR: {result['error']}"
                
            response = {
                "status": "success",
                "message": result["message"],
                "tables": result["tables"]
            }
            
            return yaml.dump(response, allow_unicode=True)
            
        except Exception as e:
            error_msg = f"Error getting tables: {str(e)}"
            self.logger.error(error_msg)
            return f"ERROR: {error_msg}"

    @json_schema(
        description="Get the schema for a specific table",
        params={
            "table_name": {
                "type": "string",
                "description": "Name of the table to get schema for",
                "required": True
            }
        }
    )
    async def get_table_schema(self, **kwargs) -> str:
        """
        Get the schema for a specific table.
        
        Args:
            kwargs:
                table_name: Name of the table to get schema for
            
        Returns:
            String with table schema information
        """
        tool_context = kwargs.get('tool_context', None)

        try:
            table_name = kwargs.get("table_name")
            if not table_name:
                return "ERROR: table_name parameter is required"
                
            if self.connector is None:
                await self._get_connector()
                
            # Run in thread pool to avoid blocking
            result = await self.connector.get_table_schema(table_name)
            
            if "error" in result:
                return f"ERROR: {result['error']}"
                
            # Render schema as an HTML table for better readability
            schema_html = self._schema_to_html(result['schema'], table_name)
            await self._raise_render_media(
                content_type="text/html",
                content=schema_html,
                name=f"{table_name}_schema.html",
                tool_context=tool_context
            )
            
            response = {
                "status": "success",
                "message": result["message"],
                "schema": result["schema"]
            }
            
            return yaml.dump(response, allow_unicode=True)
            
        except Exception as e:
            error_msg = f"Error getting table schema: {str(e)}"
            self.logger.error(error_msg)
            return f"ERROR: {error_msg}"

    @json_schema(
        description="Get database server information",
        params={}
    )
    async def get_database_info(self, **kwargs) -> str:
        """
        Get general information about the database server and connection.
        
        Returns:
            String with database server information
        """
        try:
            if self.connector is None:
                await self._get_connector()
                
            # Run in thread pool to avoid blocking
            result = await self.connector.get_database_info()
            
            if "error" in result:
                return f"ERROR: {result['error']}"
                
            response = {
                "status": "success",
                "message": result["message"],
                "info": {
                    "database_name": result["database_name"],
                    "server_version": result["server_version"],
                    "character_set": result["character_set"],
                    "collation": result["collation"],
                    "size_mb": result["size_mb"],
                    "host": result["host"],
                    "user": result["user"]
                }
            }
            
            return yaml.dump(response, allow_unicode=True)
            
        except Exception as e:
            error_msg = f"Error getting database info: {str(e)}"
            self.logger.error(error_msg)
            return f"ERROR: {error_msg}"

    def _schema_to_html(self, schema, table_name) -> str:
        """
        Convert schema information to HTML for display.
        
        Args:
            schema: List of column schema dictionaries
            table_name: Name of the table
            
        Returns:
            HTML string representation of the schema
        """
        html = f"""
        <html>
        <head>
            <style>
                table {{ border-collapse: collapse; width: 100%; font-family: sans-serif; }}
                th, td {{ border: 1px solid #ddd; padding: 8px; text-align: left; }}
                th {{ background-color: #f2f2f2; }}
                tr:nth-child(even) {{ background-color: #f9f9f9; }}
                .primary {{ color: #2E64FE; font-weight: bold; }}
                .nullable {{ color: #aaa; }}
            </style>
        </head>
        <body>
            <h2>Schema for table: {table_name}</h2>
            <table>
                <tr>
                    <th>Column Name</th>
                    <th>Data Type</th>
                    <th>Nullable</th>
                    <th>Primary Key</th>
                    <th>Default</th>
                    <th>Extra</th>
                </tr>
        """
        
        for col in schema:
            primary_class = 'class="primary"' if col["is_primary_key"] else ''
            nullable_class = 'class="nullable"' if col["is_nullable"] else ''
            
            html += f"""
                <tr {primary_class}>
                    <td>{col["column_name"]}</td>
                    <td>{col["data_type"]}</td>
                    <td {nullable_class}>{"YES" if col["is_nullable"] else "NO"}</td>
                    <td>{"YES" if col["is_primary_key"] else "NO"}</td>
                    <td>{col["default"] or "-"}</td>
                    <td>{col["extra"] or "-"}</td>
                </tr>
            """
            
        html += """
            </table>
        </body>
        </html>
        """
        
        return html


# Register the toolset with Agent C
Toolset.register(MariadbTools, required_tools=['WorkspaceTools'])
