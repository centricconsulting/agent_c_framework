import json
import os
import re
import logging
import datetime
import pandas as pd
import yaml
from typing import Optional

from bs4 import BeautifulSoup

from agent_c.toolsets import json_schema, Toolset
from agent_c_tools.tools.dynamics_crm.prompt import DynamicsCRMPrompt
from agent_c_tools.tools.dynamics_crm.util.dynamics_api import DynamicsAPI, InvalidODataQueryError
from agent_c_tools.tools.dynamics_crm.query_config import QueryConfig
from agent_c_tools.tools.dynamics_crm.data_presenter import DataPresenter
from agent_c_tools.tools.dynamics_crm.error_handler import (
    format_invalid_odata_error,
    format_lookup_not_found_error,
    format_multiple_matches_error,
    format_api_error,
    format_excel_creation_error
)
# Using workspace tool directly via UNC paths now instead of casting
from agent_c_tools.helpers.dataframe_in_memory import create_excel_in_memory


class DynamicsCrmTools(Toolset):
    """
    Microsoft Dynamics 365 CRM integration toolset.
    
    This toolset provides comprehensive integration with Microsoft Dynamics 365 CRM, enabling
    agents to query, analyze, and create CRM data. It supports standard and custom entities,
    with optimized field selection to reduce token usage.
    
    Available Methods:
        - get_entities: Retrieve one or more entities (accounts, leads, opportunities, contacts, etc.)
        - force_login: Force re-authentication with Dynamics 365
        - dynamics_user_id: Get the Dynamics user ID for the authenticated user
        - create_entity: Create new entities (notes, appointments, tasks, phone calls)
    
    Key Features:
        - Support for standard entities (accounts, leads, opportunities, contacts, etc.)
        - Support for custom entities (service offerings, industry verticals, business units)
        - OData query support for advanced filtering
        - Automatic HTML/XML content cleaning for notes and emails
        - Large dataset handling with automatic Excel file export
        - Token-optimized field selection with override options
        - Batch entity detail retrieval
    
    Requirements:
        - DYNAMICS_ENDPOINT, DYNAMICS_CLIENT_ID, DYNAMICS_SCOPE environment variables
        - REDIRECT_URI, TOKEN_ENDPOINT for OAuth authentication
        - CENTRIC_ID, CENTRIC_PW for user credentials
        - WorkspaceTools (for large dataset file operations)
    
    Usage Notes:
        - Only active entities (statecode eq 0) are returned by default for accounts
        - Large responses (>25,000 tokens or >5 records) are automatically saved to Excel
        - Results include relationship data through $expand queries
        - Custom entities include service offerings, industry verticals, and business units
    """
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
    
    @staticmethod
    def _is_guid(value: str) -> bool:
        """
        Check if a string matches GUID pattern.
        
        Args:
            value: String to check
            
        Returns:
            True if value matches GUID pattern
        """
        guid_pattern = re.compile(
            r'^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$'
        )
        return bool(guid_pattern.match(value))
    
    def _resolve_lookup_filter(self, value: str, lookup_entity: str) -> str:
        """
        Convert name to GUID using cached lookups.
        
        If value is already a GUID, return it unchanged.
        If value is a name, look it up in common_lookups cache.
        
        Args:
            value: Name or GUID to resolve
            lookup_entity: Entity type to look in (e.g., 'businessunits')
            
        Returns:
            GUID string
            
        Raises:
            ValueError: If no match found or multiple matches (with formatted error)
        """
        # If already a GUID, return it
        if self._is_guid(value):
            return value
        
        # Look up in common_lookups
        lookups = self.dynamics_object.common_lookups.get(lookup_entity, {})
        if not lookups:
            available = list(self.dynamics_object.common_lookups.keys())
            raise ValueError(
                format_lookup_not_found_error(lookup_entity, value, available)
            )
        
        # Case-insensitive partial matching
        value_lower = value.lower()
        matches = []
        
        for guid, name in lookups.items():
            if value_lower in name.lower():
                matches.append((guid, name))
        
        # Handle match results
        if len(matches) == 0:
            available_options = list(lookups.values())
            raise ValueError(
                format_lookup_not_found_error(lookup_entity, value, available_options)
            )
        elif len(matches) > 1:
            # Check for exact match
            exact_matches = [(g, n) for g, n in matches if n.lower() == value_lower]
            if len(exact_matches) == 1:
                return exact_matches[0][0]
            
            match_names = [n for _, n in matches]
            raise ValueError(
                format_multiple_matches_error(lookup_entity, value, match_names)
            )
        
        return matches[0][0]
    
    def _build_named_filters(self, config: QueryConfig) -> Optional[str]:
        """
        Build OData filter strings from named filter parameters.
        
        Args:
            config: Query configuration with named filter fields
            
        Returns:
            OData filter string or None if no named filters
        """
        filter_parts = []
        
        # Service offering filter (checks 3 fields)
        if config.filter_by_service_offering:
            so_guids = []
            for so_name in config.filter_by_service_offering:
                try:
                    guid = self._resolve_lookup_filter(so_name, 'serviceofferings')
                    so_guids.append(guid)
                except ValueError as e:
                    self.logger.warning(f"Service offering lookup failed: {e}")
                    raise
            
            # Build filter for all 3 service offering fields
            so_conditions = []
            for guid in so_guids:
                so_conditions.extend([
                    f"_cen_serviceofferingcapabiity1_value eq '{guid}'",
                    f"_cen_serviceofferingcapability2_value eq '{guid}'",
                    f"_cen_serviceofferingcapability3_value eq '{guid}'"
                ])
            filter_parts.append(f"({' or '.join(so_conditions)})")
        
        # Business unit filter
        if config.filter_by_business_unit:
            bu_conditions = []
            for bu_name in config.filter_by_business_unit:
                try:
                    guid = self._resolve_lookup_filter(bu_name, 'businessunits')
                    bu_conditions.append(f"_owningbusinessunit_value eq '{guid}'")
                except ValueError as e:
                    self.logger.warning(f"Business unit lookup failed: {e}")
                    raise
            filter_parts.append(f"({' or '.join(bu_conditions)})")
        
        # Industry vertical filter
        if config.filter_by_industry_vertical:
            iv_conditions = []
            for iv_name in config.filter_by_industry_vertical:
                try:
                    guid = self._resolve_lookup_filter(iv_name, 'industryverticals')
                    iv_conditions.append(f"_cen_centricindustryvertical_value eq '{guid}'")
                except ValueError as e:
                    self.logger.warning(f"Industry vertical lookup failed: {e}")
                    raise
            filter_parts.append(f"({' or '.join(iv_conditions)})")
        
        if not filter_parts:
            return None
        
        # Combine all filter parts with 'and'
        return ' and '.join(filter_parts)

    def _build_query_config(self, **kwargs) -> QueryConfig:
        """
        Build and validate query configuration from kwargs.
        
        Args:
            **kwargs: Raw parameters from tool invocation
            
        Returns:
            QueryConfig: Validated configuration object
        """
        config = QueryConfig(
            entity_type=kwargs.get('entity_type', 'opportunities'),
            entity_id=kwargs.get('entity_id', None),
            query_params=kwargs.get('query_params', ''),
            limit=kwargs.get('limit', 10),
            additional_fields=kwargs.get('additional_fields', None),
            override_fields=kwargs.get('override_fields', None),
            additional_expand=kwargs.get('additional_expand', None),
            workspace_name=kwargs.get('workspace_name', 'project'),
            file_path=kwargs.get('file_path', '').strip(),
            force_save=kwargs.get('force_save', False),
            filter_by_service_offering=kwargs.get('filter_by_service_offering', None),
            filter_by_business_unit=kwargs.get('filter_by_business_unit', None),
            filter_by_industry_vertical=kwargs.get('filter_by_industry_vertical', None)
        )
        
        # Build named filters if any are specified
        named_filter = self._build_named_filters(config)
        
        # Integrate named filters into query_params
        if named_filter:
            if "$filter" in config.query_params:
                # Extract existing filter
                filter_match = re.search(r'\$filter=([^&]*)', config.query_params)
                if filter_match:
                    existing_filter = filter_match.group(1)
                    # Combine with named filter
                    combined_filter = f"({existing_filter}) and ({named_filter})"
                    config.query_params = config.query_params.replace(
                        filter_match.group(0), f"$filter={combined_filter}"
                    )
            else:
                # Add named filter as new $filter
                separator = "&" if config.query_params else ""
                config.query_params += f"{separator}$filter={named_filter}"
        
        # Handle statecode filter for accounts
        if config.entity_type == 'accounts':
            if "$filter" in config.query_params:
                filter_part = re.search(r'\$filter=([^&]*)', config.query_params)
                if filter_part:
                    filter_content = filter_part.group(1)
                    if 'statecode' not in filter_content:
                        statecode_filter = "statecode eq 0"
                        new_filter = f"({statecode_filter}) and ({filter_content})"
                        config.query_params = config.query_params.replace(
                            filter_part.group(0), f"$filter={new_filter}"
                        )
        
        # Add $top if not present and limit is specified
        if "$top" not in config.query_params and config.limit > 0:
            config.query_params += f"&$top={config.limit}"
        
        return config

    async def _fetch_entities(self, config: QueryConfig):
        """
        Fetch entities from Dynamics API based on configuration.
        
        Args:
            config: Query configuration
            
        Returns:
            List of entity dictionaries or None
            
        Raises:
            InvalidODataQueryError: If the OData query is malformed
            Exception: For other API errors
        """
        # Ensure common lookups are loaded
        if self.dynamics_object.common_lookups is None:
            await self.dynamics_object.one_time_lookups()
        
        # Fetch entities based on whether we have a specific ID or not
        if config.entity_id:
            return await self.dynamics_object.get_entities(
                entity_type=config.entity_type,
                entity_id=config.entity_id,
                additional_fields=config.additional_fields,
                override_fields=config.override_fields,
                additional_expand=config.additional_expand
            )
        else:
            return await self.dynamics_object.get_entities(
                entity_type=config.entity_type,
                query_params=config.query_params,
                additional_fields=config.additional_fields,
                override_fields=config.override_fields,
                additional_expand=config.additional_expand
            )

    def _clean_entity_data(self, entities, entity_type: str):
        """
        Clean HTML/XML content from entity fields.
        
        Args:
            entities: List of entity dictionaries or single entity
            entity_type: Type of entities being cleaned
            
        Returns:
            Cleaned entities (same structure as input)
        """
        if entities is None or entity_type not in self.ENTITY_CLEAN_FIELDS:
            return entities
        
        field, parser = self.ENTITY_CLEAN_FIELDS[entity_type]
        
        # Handle both single entity and list of entities
        entities_list = entities if isinstance(entities, list) else [entities]
        
        for entity in entities_list:
            if field in entity:
                entity[field] = self.clean_html_xml(entity[field], parser)
        
        return entities

    async def _present_results(self, entities, config: QueryConfig, **kwargs) -> str:
        """
        Present query results to the user using DataPresenter.
        
        Delegates to DataPresenter for smart presentation mode selection.
        Handles file saving for large datasets.
        
        Args:
            entities: List of entity dictionaries
            config: Query configuration
            **kwargs: Additional context (tool_context, etc.)
            
        Returns:
            YAML string containing results or file information
        """
        tool_context = kwargs.get('tool_context', {})
        bridge = tool_context.get('bridge', None)
        agent_runtime = tool_context.get('agent_runtime')
        
        # Create presenter
        presenter = DataPresenter(agent_runtime=agent_runtime)
        
        # Get presentation with automatic mode selection
        presentation = presenter.present(
            entities=entities,
            entity_type=config.entity_type,
            mode='auto'
        )
        
        # Check if we need to save to file (> 15 records)
        should_save = (
            config.force_save or 
            len(entities) > 15 or
            presentation['estimated_tokens'] > 25000
        )
        
        if should_save:
            # Save to file but still return summary/data
            file_info = await self._save_to_file(entities, config, presentation, tool_context)
            # Add file save notification to presentation
            presentation['file_saved'] = True
            presentation['file_unc_path'] = file_info['unc_path']
            presentation['note'] = presentation.get('note', '') + f' Full dataset saved to Excel file at {file_info["unc_path"]}.'
        
        # Return YAML format (with summary or full data, plus file notification if saved)
        return yaml.dump(presentation, default_flow_style=False, sort_keys=False, allow_unicode=True)
    
    async def _save_to_file(
        self,
        entities: list,
        config: QueryConfig,
        presentation: dict,
        tool_context: dict
    ) -> dict:
        """
        Save entities to Excel file and send notification.
        
        Args:
            entities: List of entity dictionaries
            config: Query configuration
            presentation: Presentation dictionary from DataPresenter
            tool_context: Tool context with bridge and runtime
            
        Returns:
            Dictionary with file_name, unc_path, workspace_name
        """
        bridge = tool_context.get('bridge', None)
        
        self.logger.info(
            f"Saving large dataset: force_save={config.force_save}, "
            f"tokens={presentation['estimated_tokens']}, records={len(entities)}"
        )
        
        # Create file name
        timestamp = datetime.datetime.now().strftime("%Y%m%d_%H%M%S")
        file_name = f'{config.entity_type}_{timestamp}.xlsx'
        
        # Construct UNC path
        unc_path = self._build_unc_path(config.workspace_name, config.file_path, file_name)
        
        # Create Excel file
        try:
            excel_buffer = create_excel_in_memory(pd.DataFrame(entities))
        except Exception as e:
            return format_excel_creation_error(str(e))
        
        # Write file
        await self.tool_chest.available_tools['WorkspaceTools'].internal_write_bytes(
            path=unc_path,
            mode='write',
            data=excel_buffer.getvalue()
        )
        self.logger.info(f'Saved to {config.workspace_name}: {file_name}')
        
        # Get file system path
        file_system_path = self._get_file_system_path(unc_path)
        display_path = file_system_path if file_system_path else unc_path
        file_url = self._create_file_url(file_system_path, unc_path)
        
        # Send markdown notification
        await self._send_file_notification(
            bridge, file_name, display_path, file_url,
            len(entities), presentation['estimated_tokens'], config.file_path
        )
        
        # Return file info for agent to use later
        return {
            'file_name': file_name,
            'unc_path': unc_path,
            'workspace_name': config.workspace_name,
            'file_path': config.file_path if config.file_path else None
        }
    
    def _build_unc_path(self, workspace_name: str, file_path: str, file_name: str) -> str:
        """Construct UNC path from components."""
        if file_path:
            return f"//{workspace_name}/{file_path}/{file_name}"
        else:
            return f"//{workspace_name}/{file_name}"
    
    def _get_file_system_path(self, unc_path: str) -> Optional[str]:
        """Get OS file system path from UNC path."""
        _, workspace_obj, rel_path = self.tool_chest.available_tools[
            'WorkspaceTools'
        ]._parse_unc_path(unc_path)
        
        if workspace_obj and hasattr(workspace_obj, 'full_path'):
            return workspace_obj.full_path(rel_path, mkdirs=False)
        return None
    
    def _create_file_url(self, file_system_path: Optional[str], unc_path: str) -> str:
        """Create file:// URL from paths."""
        if file_system_path:
            url_path = file_system_path.replace('\\', '/')
            return f"file://{url_path}" if url_path.startswith('/') else f"file:///{url_path}"
        else:
            return f"file:///{unc_path.replace('//', '').replace('\\', '/')}"
    
    async def _send_file_notification(
        self, bridge, file_name: str, display_path: str,
        file_url: str, record_count: int, token_count: int, file_path: str
    ):
        """Send markdown notification about saved file."""
        if bridge is None:
            return
        
        path_info = f' in {file_path}/' if file_path else ' in the root directory'
        markdown_message = f""":::IMPORTANT
📁 **File Saved Successfully**

**File:** `{file_name}`  
**Location:** [{display_path}]({file_url})  
**Contents:** {record_count} records saved  
**Token Size:** {token_count:,} tokens

⚠️ **Browser Security Notice:**  
Due to browser security restrictions, you may need to manually navigate to the file location to open it.

**Path to copy:** `{display_path}`
:::"""
        
        await bridge.raise_render_media_markdown(markdown_message, self.__class__.__name__)

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
                    "cen_airelated is an option set field to denote if an entity is AI related or not.\n"
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
                'description': "The maximum number of entities to return.  '0' means return all records. Default is all records.",
                'required': False,
                'default': 0
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
            'filter_by_service_offering': {
                'type': 'array',
                'items': {
                    'type': 'string'
                },
                'description': 'Filter by service offering name(s) or GUID(s). Examples: ["Data & Analytics", "Cybersecurity"]. Accepts friendly names or GUIDs.',
                'required': False
            },
            'filter_by_business_unit': {
                'type': 'array',
                'items': {
                    'type': 'string'
                },
                'description': 'Filter by business unit name(s) or GUID(s). Examples: ["Columbus", "Chicago"]. Accepts friendly names or GUIDs.',
                'required': False
            },
            'filter_by_industry_vertical': {
                'type': 'array',
                'items': {
                    'type': 'string'
                },
                'description': 'Filter by industry vertical name(s) or GUID(s). Examples: ["Healthcare", "Financial Services"]. Accepts friendly names or GUIDs.',
                'required': False
            },
        }
    )
    async def get_entities(self, **kwargs):
        """
        Get one or more entities from Dynamics CRM.
        
        This method orchestrates the entity retrieval process by delegating
        to specialized helper methods for each step.
        """
        tool_context = kwargs.get('tool_context', {})
        bridge = tool_context.get('bridge', None)
        
        # Handle default limit with user notification
        limit = kwargs.get('limit', 0)
        if limit > 0:
            if bridge is not None:
                message = (
                    ":::NOTE\n"
                    f"Limit parameter was provided, only returning {limit} records.\n"
                    "Please ask for a specific number of records you want if you need fewer. "
                    "Asking for 0 records returns all records."
                    ":::"
                )
                await bridge.raise_render_media_markdown(message, self.__class__.__name__)
        
        # Step 1: Build and validate query configuration
        config = self._build_query_config(**kwargs)
        
        # Step 2: Fetch entities from Dynamics API
        try:
            entities = await self._fetch_entities(config)
        except InvalidODataQueryError as e:
            return format_invalid_odata_error(config.query_params, str(e))
        except ValueError as e:
            # ValueError is raised by lookup resolution with formatted errors
            return str(e)
        except Exception as e:
            return format_api_error('fetching entities', str(e))
        
        # Step 3: Clean HTML/XML content from entity data
        entities = self._clean_entity_data(entities, config.entity_type)
        
        # Step 4: Present results to user (handles formatting and file saving)
        return await self._present_results(entities, config, **kwargs)

    @json_schema(
        description="Force re-logging into Dynamics at user request only.",
        params={}
    )
    async def force_login(self, **kwargs):
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
    async def dynamics_user_id(self, **kwargs):
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


Toolset.register(DynamicsCrmTools, required_tools=['WorkspaceTools'])
