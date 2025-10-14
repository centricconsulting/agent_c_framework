import os
import json
import logging
import pandas as pd

from typing import cast
from collections import OrderedDict
from datetime import datetime, date

from simple_salesforce import Salesforce


from ...helpers.media_file_html_helper import get_file_html
from ...helpers.path_helper import ensure_file_extension, create_unc_path, os_file_system_path
from agent_c_tools.tools.workspace import WorkspaceTools
from agent_c.toolsets import Toolset, json_schema
from ...helpers.dataframe_in_memory import create_excel_in_memory
from .prompt import SalesforcePrompt
from .util import validate_soql_query, SalesforceQueryError, clean_salesforce_record


class SalesforceTools(Toolset):
    """
    Salesforce CRM integration toolset.
    
    This toolset provides full integration with Salesforce CRM, enabling agents to query data,
    create records, update existing records, and delete records using SOQL (Salesforce Object Query Language).
    
    Available Methods:
        - query_salesforce: Execute SOQL queries to retrieve Salesforce data
        - create_record: Create new records in Salesforce objects
        - update_record: Update existing Salesforce records
        - delete_record: Delete records from Salesforce
    
    Key Features:
        - Full SOQL query support for advanced data retrieval
        - CRUD operations on all standard and custom Salesforce objects
        - Automatic date format conversion to ISO-8601
        - Large dataset handling with automatic Excel export
        - Automatic field deduplication
        - Workspace integration for result file storage
    
    Requirements:
        - SALESFORCE_USERID, SALESFORCE_PASSWORD, SALESFORCE_SECURITY_TOKEN environment variables
        - SALESFORCE_DOMAIN for custom domain configuration
        - WorkspaceTools (for file operations)
        - simple-salesforce Python package
    
    Usage Notes:
        - Uses query_all() to retrieve records including deleted/archived items
        - Large responses (>25,000 tokens) are automatically saved to Excel
        - When creating records with relationships, use the related record's ID (e.g., AccountId)
        - Field names must match Salesforce API names exactly
        - Results include data type conversions for dates and datetime objects
    """
    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='salesforce',
                         needed_keys=['SALESFORCE_USERID', 'SALESFORCE_PASSWORD', 'SALESFORCE_SECURITY_TOKEN',
                                      'SALESFORCE_DOMAIN'])
        self.logger: logging.Logger = logging.getLogger(__name__)
        self.workspace_tool = self.tool_chest.available_tools.get('WorkspaceTools')

        self.section = SalesforcePrompt()
        # adding domain to the object causes validation errors. This still works without the domain being passed
        self.sf = Salesforce(
            username=kwargs.get('username', os.getenv('SALESFORCE_USERID')),
            password=kwargs.get('password', os.getenv('SALESFORCE_PASSWORD')),
            security_token=kwargs.get('security_token', os.getenv('SALESFORCE_SECURITY_TOKEN'))
        )


    @staticmethod
    def convert_dates_to_iso_format(record):
        for key, value in record.items():
            if isinstance(value, (datetime, date)):
                record[key] = value.isoformat()
        return record

    @staticmethod
    def deduplicate_keys(data):
        deduplicated_data = OrderedDict()
        for key, value in data.items():
            deduplicated_data[key] = value
        return dict(deduplicated_data)

    @json_schema(
        description="Query Salesforce data using SOQL (Salesforce Object Query Language).",
        params={
            'soql_query': {
                'type': 'string',
                'description': 'The SOQL query to execute',
                'required': True
            },
            'workspace_name': {
                'type': 'string',
                'description': 'Workspace name to use for saving files',
                'required': False,
                'default': 'project'
            },
            'file_path': {
                'type': 'string',
                'description': "The relative file path and name in the workspace for saving the results file. Should be an excel file with '.xlsx' extension.",
                'required': False
            },
            'force_save': {
                'type': 'boolean',
                'description': 'If user specifically requests the data be saved to a file regardless of size. '
                               'Set to True if you need to perform analysis on the results. ',
                'required': False,
                'default': False
            },
        }
    )
    async def query_salesforce(self, **kwargs) -> str:
        soql_query = kwargs.get('soql_query')
        workspace_name = kwargs.get('workspace_name', 'project')
        force_save = kwargs.get('force_save', False)
        file_path = kwargs.get('file_path', f'salesforce_query_results_{datetime.now().strftime("%Y%m%d_%H%M%S")}.xlsx')
        tool_context = kwargs.get('tool_context', None)

        self.logger.info(f"Querying Salesforce with SOQL: {soql_query}")
        if not self.sf:
            return json.dumps({"error": "Salesforce connection not initialized"})

        try:
            if not validate_soql_query(soql_query):
                raise SalesforceQueryError("Invalid SOQL query")

            results = self.sf.query_all(soql_query)

            if not results['records']:
                return json.dumps({"message": "No records found"})

            # Clean and process the records
            cleaned_records = [clean_salesforce_record(record) for record in results['records']]
            cleaned_records = [self.convert_dates_to_iso_format(record) for record in cleaned_records]

            # Convert to DataFrame
            df = pd.DataFrame(cleaned_records)

            if tool_context is None or 'agent_runtime' not in tool_context:
                raise ValueError("tool_context with agent_runtime is required for token counting")
            else:
                response_size = tool_context['agent_runtime'].count_tokens(df.to_json())

            if response_size > 25000 or force_save:
                file_path = ensure_file_extension(file_path, 'xlsx')
                unc_path = create_unc_path(workspace_name, file_path)

                excel_buffer = create_excel_in_memory(df)

                result = await self.workspace_tool.internal_write_bytes(
                    path=unc_path,
                    mode='write',
                    data=excel_buffer.getvalue()
                )
                os_path = os_file_system_path(self.workspace_tool, unc_path)

                await self._raise_render_media(
                    sent_by_class=self.__class__.__name__,
                    sent_by_function='save_salesforce_query_results',
                    content_type="text/html",
                    content=get_file_html(os_path, unc_path),
                    tool_context=tool_context
                )
                self.logger.debug(result)

                return json.dumps({
                    'file_path': file_path,
                    'workspace_name': workspace_name,
                    'total_records': len(df),
                    'message': f"Query results saved to file {unc_path}",
                    'preview': df.head(5).to_dict(orient='records')
                })
            else:
                return df.to_json(orient='records', date_format='iso')

        except SalesforceQueryError as e:
            return json.dumps({"error": str(e)})
        except Exception as e:
            self.logger.error(f"Error executing Salesforce query: {str(e)}")
            return json.dumps({"error": f"Error executing Salesforce query: {str(e)}"})

    @json_schema(
        description="Create a new record in Salesforce.  Use Default Salesforce object names (e.g., 'Account', 'Contact') "
                    "and provide a dictionary of data for the new record. This is a default Salesforce installation, ensure"
                    "that the data dictionary has the right field names for the object. When associating records, use the ID of the record. "
                    "For example, to associate a contact with an account, you must pass in the AccountId of the Account in the Contact record."
                    "This means you may have to look up accounts before adding contacts.",
        params={
            'object_name': {
                'type': 'string',
                'description': 'The name of the Salesforce object (e.g., "Account", "Contact")',
                'required': True
            },
            'data': {
                'type': 'object',
                'description': 'A hash dictionary containing the data for the new record',
                'required': True
            }
        }
    )
    async def create_record(self, **kwargs) -> str:
        object_name = kwargs.get('object_name')
        data = kwargs.get('data')
        tool_context = kwargs.get('tool_context', None)
        self.logger.info(f"Creating {object_name} in Salesforce with: {data}")

        if not self.sf:
            return json.dumps({"error": "Salesforce connection not initialized"})

        if object_name is None or data is None:
            return json.dumps({"error": f"Missing required parameters 'object_name': {object_name} or 'data': {data}"})

        self.logger.info(f"Creating record in Salesforce object {object_name} with data: {data}")

        try:
            # De-duplicate keys in the data dictionary
            clean_data = self.deduplicate_keys(data)

            # Create the record using the object_name and clean_data
            result = self.sf.__getattr__(object_name).create(clean_data)

            if result['success']:
                return json.dumps({
                    "message": f"Successfully created {object_name} record",
                    "id": result['id']
                })
            else:
                return json.dumps({
                    "error": "Failed to create record",
                    "details": result.get('errors', [])
                })
        except Exception as e:
            self.logger.error(f"Error creating Salesforce record: {str(e)}")
            return json.dumps({"error": f"Error creating Salesforce record: {str(e)}"})

    @json_schema(
        description="Update an existing record in Salesforce",
        params={
            'object_name': {
                'type': 'string',
                'description': 'The name of the Salesforce object (e.g., "Account", "Contact")',
                'required': True
            },
            'record_id': {
                'type': 'string',
                'description': 'The ID of the record to update',
                'required': True
            },
            'data': {
                'type': 'object',
                'description': 'A dictionary containing the updated data for the record',
                'required': True
            }
        }
    )
    async def update_record(self, **kwargs) -> str:
        object_name = kwargs.get('object_name')
        record_id = kwargs.get('record_id')
        data = kwargs.get('data')
        tool_context = kwargs.get('tool_context', None)
        self.logger.info(f"Updating {object_name} with id of {record_id} in Salesforce with: {data}")

        if not self.sf:
            return json.dumps({"error": "Salesforce connection not initialized"})

        if object_name is None or data is None or record_id is None:
            return json.dumps({"error": f"Missing required parameters 'object_name': {object_name}, 'data': {data}, or 'record_id': {record_id}"})

        self.logger.info(f"Updating record in Salesforce object {object_name} where id is {id} with data: {data}")
        clean_data = self.deduplicate_keys(data)
        try:
            result = self.sf.__getattr__(object_name).update(record_id, clean_data)
            return json.dumps({
                "message": f"Successfully updated {object_name} record with ID {record_id}"
            })
        except Exception as e:
            self.logger.error(f"Error updating Salesforce record: {str(e)}")
            return json.dumps({"error": f"Error updating Salesforce record: {str(e)}"})

    @json_schema(
        description="Delete a record from Salesforce",
        params={
            'object_name': {
                'type': 'string',
                'description': 'The name of the Salesforce object (e.g., "Account", "Contact")',
                'required': True
            },
            'record_id': {
                'type': 'string',
                'description': 'The ID of the record to delete',
                'required': True
            }
        }
    )
    async def delete_record(self, **kwargs) -> str:
        object_name = kwargs.get('object_name')
        record_id = kwargs.get('record_id')
        tool_context = kwargs.get('tool_context', None)
        print(f"Deleting {object_name} with id of {record_id} in Salesforce")

        if not self.sf:
            return json.dumps({"error": "Salesforce connection not initialized"})

        try:
            self.sf.delete(record_id)
            return json.dumps({
                "message": f"Successfully deleted {object_name} record with ID {record_id}"
            })
        except Exception as e:
            self.logger.error(f"Error deleting Salesforce record: {str(e)}")
            return json.dumps({"error": f"Error deleting Salesforce record: {str(e)}"})


Toolset.register(SalesforceTools, required_tools=['WorkspaceTools'])