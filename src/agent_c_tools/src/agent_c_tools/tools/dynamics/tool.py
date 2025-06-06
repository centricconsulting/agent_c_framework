import json
import os
import re
import logging
import datetime
import pandas as pd

from bs4 import BeautifulSoup

from agent_c.toolsets import json_schema, Toolset
from agent_c_tools.tools.dynamics.prompt import DynamicsCRMPrompt
from agent_c_tools.tools.dynamics.util.dynamics_api import DynamicsAPI, InvalidODataQueryError
# Using workspace tool directly via UNC paths now instead of casting
from agent_c_tools.helpers.dataframe_in_memory import create_excel_in_memory


class DynamicsTools(Toolset):
    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='dynamics_crm',
                         needed_keys=['DYNAMICS_ENDPOINT', 'DYNAMICS_CLIENT_ID', 'DYNAMICS_SCOPE',
                                      'REDIRECT_URI', 'TOKEN_ENDPOINT', 'CENTRIC_ID', 'CENTRIC_PW'])
        if not self.tool_valid:
            return

        # Various environment needsHel;
        self.base_url: str = kwargs.get('DYNAMICS_ENDPOINT', os.getenv('DYNAMICS_ENDPOINT'))
        self.client_id: str = kwargs.get('DYNAMICS_CLIENT_ID', os.getenv('DYNAMICS_CLIENT_ID'))
        self.dynamics_scope: str = kwargs.get('DYNAMICS_SCOPE', os.getenv('DYNAMICS_SCOPE'))
        self.redirect_uri: str = kwargs.get('REDIRECT_URI', os.getenv('REDIRECT_URI'))
        self.token_endpoint: str = kwargs.get('TOKEN_ENDPOINT', os.getenv('TOKEN_ENDPOINT'))
        self.access_token: str = kwargs.get('access_token', '')
        self.user_id: str = kwargs.get('CENTRIC_ID', os.getenv('CENTRIC_ID'))
        self.user_pw: str = kwargs.get('CENTRIC_PW', os.getenv('CENTRIC_PW'))
        # Workspace tool is accessed directly via tool_chest when needed
        # Main Dynamics API object to work with
        self.dynamics_object = DynamicsAPI(base_url=self.base_url, client_id=self.client_id,
                                           dynamics_scope=self.dynamics_scope, redirect_uri=self.redirect_uri,
                                           token_endpoint=self.token_endpoint, access_token=self.access_token,
                                           user_id=self.user_id, user_pw=self.user_pw)
        # Special Tool prompt inclusion
        self.section = DynamicsCRMPrompt()

        # Constants for class
        self.ENTITY_CLEAN_FIELDS = {
            'annotations': ('notetext', 'html.parser'),
            'posts': ('text', 'xml'),
            'emails': ('description', 'html.parser'),
            'phonecalls': ('description', 'html.parser'),
            'appointments': ('description', 'html.parser')
        }
        self.logger: logging.Logger = logging.getLogger(__name__)

    @staticmethod
    def clean_html_xml(text, parser='html.parser'):
        soup = BeautifulSoup(text, parser)
        if parser == 'xml':
            return ''.join(soup.findAll(text=True))
        else:
            return soup.get_text()

    @json_schema(
        description="Get one or more entities from Dynamics CRM.  "
                    "Entities include 'accounts', 'leads', 'opportunities', 'contacts', 'annotations', 'appointments', "
                    "'tasks', 'emails', 'posts', 'phonecalls'.\n"
                    "This enables you to retrieve data and analyze it from an CRM system.  All query_params must be "
                    "Dynamics compatible odata queries or they will not work.  There are custom entities in this instance:\n"
                    "cen_serviceofferingcapabilitieses are service offering names like Data & Analytics, Modern Software Delivery, "
                    "cen_industryverticalsubs are sub-categories of service offerings Software Quality Assurance and Testing, Agile, "
                    "businessunits are geographic city office locations like Boston, Columbus, Chicago,  "
                    "cen_industryverticals are industry verticals like Healthcare, Financial Services, Retail.\n"
                    "The function is optimized to return only necessary fields by default to improve efficiency and reduce token usage.",
        params={
            'entity_type': {
                'type': 'string',
                'description': 'The type of Dynamics entity to retrieve.  Common values are "accounts", "leads", "opportunities',
                'enum': ['accounts', 'leads', 'opportunities', 'contacts', 'annotations', 'appointments', 'tasks',
                         'emails', 'posts', 'phonecalls', 'cen_serviceofferingcapabilitieses', 'businessunits',
                         'cen_industryverticalsubs', 'cen_industryverticals'],
                'required': True,
                'default': 'opportunities'
            },
            'entity_id': {
                'type': 'string',
                'description': 'The unique ID assigned by Dynamics to the entity record',
                'required': False
            },
            'query_params': {
                'type': 'string',
                'description': 'Dynamics compatible query to filter the entity list.  Include $filter= when your query is a filter.',
                'required': False,
            },
            'limit': {
                'type': 'integer',
                'description': 'The maximum number of entities to return',
                'required': False,
            },
            'workspace_name': {
                'type': 'string',
                'description': 'Workspace name to use for saving files',
                'required': False,
                'default': 'project'
            },
            'force_save': {
                'type': 'boolean',
                'description': 'If user specifically requests the data be saved to a file',
                'required': False,
                'default': False
            },
            'file_path': {
                'type': 'string',
                'description': 'Path within the workspace to save the file (e.g., "reports/dynamics")',
                'required': False,
                'default': ''
            },
            'additional_fields': {
                'type': 'array',
                'items': {
                    'type': 'string'
                },
                'description': 'Optional additional fields to include in the response beyond the default fields',
                'required': False
            },
            'additional_expand': {
                'type': 'object',
                'description': 'Optional additional expand relationships to include in the response. Should be a dictionary with Keys representing relationship names and values are arrays of fields to expand.',
                'required': False,
            },
            'override_fields': {
                'type': 'array',
                'items': {
                    'type': 'string'
                },
                'description': 'List of specific fields to return for the entity. This will override the default fields returned. Do not use unless you absolutely must.',
                'required': False
            },
        }
    )
    async def get_entities(self, **kwargs):
        entity_id = kwargs.get('entity_id', None)
        workspace_name = kwargs.get('workspace_name', 'project')
        query_params = kwargs.get('query_params', '')
        limit = kwargs.get('limit', 0)
        entity_type = kwargs.get('entity_type', 'opportunities')
        statecode_filter = "statecode eq 0"  # we only want active entities
        force_save = kwargs.get('force_save', False)
        file_path = kwargs.get('file_path', '').strip()
        _OVERSIZE_ENTITY_CAP = 5

        # Allow specifying additional fields
        additional_fields = kwargs.get('additional_fields', None)
        # Allow specifying fields that completely override the defaults
        override_fields = kwargs.get('override_fields', None)
        # Allow specifying additional expand relationships
        additional_expand = kwargs.get('additional_expand', None)

        # get core lookups
        if self.dynamics_object.common_lookups is None:
            await self.dynamics_object.one_time_lookups()

        if query_params:
            # I don't love this, but we have a ton of closed accounts that nearly mirrors our open accounts in our CRM
            if "$filter" in query_params and ("accounts" in query_params):
                # Regex for checking if statecode is already in the filter.
                # Statecode determines if record is active or not
                filter_part = re.search(r'\$filter=([^&]*)', query_params)

                if filter_part:
                    filter_content = filter_part.group(1)
                    if 'statecode' not in filter_content:
                        # Add statecode filter if it's not already there
                        new_filter = f"({statecode_filter}) and ({filter_content})"
                        query_params = query_params.replace(filter_part.group(0), f"$filter={new_filter}")
                else:
                    # if no query params were passed, at least only pull the active entities
                    query_params += f"&$filter={statecode_filter}"

        if "$top" not in query_params and limit > 0:
            query_params += f"&$top={limit}"

        try:
            if entity_id:
                entities = await self.dynamics_object.get_entities(
                    entity_type=entity_type,
                    entity_id=entity_id,
                    additional_fields=additional_fields,
                    override_fields=override_fields,
                    additional_expand=additional_expand
                )
            else:
                entities = await self.dynamics_object.get_entities(
                    entity_type=entity_type,
                    query_params=query_params,
                    additional_fields=additional_fields,
                    override_fields=override_fields,
                    additional_expand=additional_expand
                )
        except InvalidODataQueryError as e:
            return json.dumps({"error": f"Invalid OData query: {str(e)}"})
        except Exception as e:
            return json.dumps({"error": f"Error fetching entities: {str(e)}"})

        # this is to clean fields like notes, annotations that contain either html or xml, reduce token consumption
        if entities is not None and entity_type in self.ENTITY_CLEAN_FIELDS:
            field, parser = self.ENTITY_CLEAN_FIELDS[entity_type]
            for entity in entities:
                if field in entity:
                    entity[field] = self.clean_html_xml(entity[field], parser)

        # deal with overly large datasets.
        response_size = self.tool_chest.agent.count_tokens(json.dumps(entities))
        if response_size > 25000 or len(entities) > _OVERSIZE_ENTITY_CAP or force_save:
            # Response is too large, user requested save, or the response is too many records to deal with save it.
            self.logger.info(f"User requested local save: ({force_save})")
            self.logger.info(f"Dataset token size: ({response_size})")
            self.logger.info(f"Number of records: ({len(entities)})")

            # Create unique file name based on date/time
            timestamp = datetime.datetime.now().strftime("%Y%m%d_%H%M%S")
            file_name = f'{entity_type}_{timestamp}.xlsx'

            # Construct the UNC path, incorporating the file_path if provided
            if file_path:
                # Normalize the path (remove leading/trailing slashes)
                normalized_path = file_path.strip('/')
                unc_path = f"//{workspace_name}/{normalized_path}/{file_name}"
            else:
                unc_path = f"//{workspace_name}/{file_name}"

            # Create the Excel file in memory
            try:
                # if the entities are not a list of dictionaries, this will fail, usually because dynamics returns
                # an error message, but a 200 status code like query has unbalanced brackets
                excel_buffer = create_excel_in_memory(pd.DataFrame(entities))
            except Exception as e:
                return json.dumps({"error": f"Error creating excel buffer bytes: {str(e)}\n\n{entities}"})

            # Write the file using the UNC path
            result = await self.tool_chest.available_tools['WorkspaceTools'].internal_write_bytes(
                path=unc_path,
                mode='write',
                data=excel_buffer.getvalue()
            )
            self.logger.debug(result)
            self.logger.info(f'Saved data to workspace: {workspace_name} with file name: {file_name}')

            # Create HTML content for the media event
            # Determine display path for the UI
            display_path = file_path if file_path else '(root)'

            # Get the actual OS-level filepath using the workspace's full_path method
            _, workspace_obj, rel_path = self.tool_chest.available_tools['WorkspaceTools']._parse_unc_path(unc_path)
            file_system_path = None
            if workspace_obj and hasattr(workspace_obj, 'full_path'):
                # The full_path method handles path normalization and joining with workspace root
                # Set mkdirs=False since we're just getting the path for a URL, not writing
                file_system_path = workspace_obj.full_path(rel_path, mkdirs=False)

            # Create a file:// URL from the system path
            file_url = None
            if file_system_path:
                # Convert backslashes to forward slashes for URL
                url_path = file_system_path.replace('\\', '/')

                # Ensure correct URL format (need 3 slashes for file:// URLs with absolute paths)
                if url_path.startswith('/'):
                    file_url = f"file://{url_path}"
                else:
                    file_url = f"file:///{url_path}"
            else:
                # Fallback if we couldn't get the actual path
                file_url = f"file:///{unc_path.replace('//', '').replace('\\', '/')}"

            # Determine open command based on path (Windows vs Mac/Linux)
            open_command = ""
            if file_system_path:
                if ':\\' in file_system_path or ':/' in file_system_path:  # Windows path
                    open_command = f'start "" "{file_system_path}"'
                else:  # Mac/Linux
                    open_command = f'open "{file_system_path}"'

            html_content = f"""
<div style="padding: 20px; font-family: system-ui, sans-serif; max-width: 800px; margin: 0 auto; border: 1px solid #e2e8f0; border-radius: 8px; background-color: #f8fafc;">
    <h2 style="color: #334155; margin-top: 0;">File Saved Successfully</h2>
     
    <div style="background-color: #fff7ed; border-left: 4px solid #f97316; padding: 16px; margin-bottom: 20px;">
        <p style="margin: 0; font-weight: 500; color: #9a3412;">Browser Security Notice</p>
        <p style="margin: 8px 0 0 0;">Due to browser security restrictions, you'll need to manually open the file:</p>
    </div>

    <div style="margin-bottom: 16px;">
        <p><strong>File path:</strong> <br/>
        <code style="background: #e2e8f0; padding: 8px; border-radius: 4px; display: block; margin-top: 8px; word-break: break-all;">{file_system_path if file_system_path else unc_path}</code>
        <p style="margin: 0;"><strong>Contents:</strong> {len(entities)} records saved.</p>
        </p>
    </div>

    <div style="margin-top: 16px;">
        <a href="{file_url}" target="_blank" style="display: inline-block; background-color: #3b82f6; color: white; text-decoration: none; padding: 10px 16px; border-radius: 6px; font-weight: 500;">Try Direct Link</a>
        <span style="margin-left: 8px; color: #6b7280;">(may not work due to browser restrictions)</span>
    </div>
</div>
            """

            # Raise the media event
            await self._raise_render_media(
                sent_by_class=self.__class__.__name__,
                sent_by_function='get_entities',
                content_type="text/html",
                content=html_content,
                tool_context=kwargs.get('tool_context'),
            )

            if entity_id is None:
                if len(entities) > _OVERSIZE_ENTITY_CAP:
                    entity_data = entities[_OVERSIZE_ENTITY_CAP]
                else:
                    entity_data = entities
            else:
                entity_data = entities[0]

            # Create a more informative user message that includes path information
            path_info = f' in {file_path}/' if file_path else ' in the root directory'

            return json.dumps({
                'user_message': f"Display this message to the user. The response was saved to a file {file_name}{path_info} of the {workspace_name} workspace. "
                                f"Here are the first {_OVERSIZE_ENTITY_CAP} of {len(entities)} entities. The dataset size is {response_size} tokens. "
                                f"The query_params used was: {query_params}"
                                f"{'The $top query_param limited records' if '$top' in query_params else ''}",
                'file_name': file_name,
                'workspace_name': workspace_name,
                'file_path': file_path,
                'unc_path': unc_path,
                'entity_data': entity_data,
                'entity_count': len(entities)
            })
        else:
            return json.dumps(entities)

    @json_schema(
        description="Force re-logging into Dynamics at user request only.",
        params={}
    )
    async def force_login(self):
        """force re-login on dynamics"""
        try:
            self.dynamics_object.access_token = None
            await self.dynamics_object.authorize_dynamics()
        except Exception as e:
            self.logger.error(f"Error fetching WhoAmI data: {str(e)}")
            raise e

    @json_schema(
        description="Get the Dynamics ID for the user making the request.",
        params={}
    )
    async def dynamics_user_id(self):
        """Get the whoami Dyanmics ID, if not available, re-authorize"""
        if self.dynamics_object.whoami_id is None:
            await self.dynamics_object.authorize_dynamics()

        if self.dynamics_object.common_lookups is None:
            await self.dynamics_object.one_time_lookups()

        return json.dumps({'whoami_id': f'{self.dynamics_object.whoami_id}'})

    @json_schema(
        description="Create a new entity in Dynamics CRM",
        params={
            'entity_type': {
                'type': 'string',
                'description': 'The type of entity to create (e.g., "accounts", "contacts")',
                'required': True
            },
            'entity_data': {
                'type': 'object',
                'description': 'A dictionary containing the data for the new entity',
                'required': True,
                'enum': ['annotations', 'appointments', 'tasks', 'phonecalls'],  # Limiting for now
                'default': 'tasks'
            }
        }
    )
    async def create_entity(self, **kwargs):
        entity_type = kwargs.get('entity_type', None)
        entity_data = kwargs.get('entity_data', None)
        if entity_data is None or entity_type is None:
            return json.dumps({"error": "Missing required parameters: entity_type, entity_data"})

        if entity_type not in ['annotations', 'appointments', 'tasks', 'phonecalls']:
            return json.dumps({"error": "Only allowing select entities to be created. "
                                        "Must be one of: 'annotations', 'appointments', 'tasks', 'phonecalls'"})

        new_entity = await self.dynamics_object.create_entity(entity_type, entity_data)

        if new_entity:
            return json.dumps(new_entity)
        else:
            return json.dumps({"error": "Failed to create entity"})


Toolset.register(DynamicsTools, required_tools=['WorkspaceTools'])
