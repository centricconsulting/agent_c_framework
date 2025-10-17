import os
import re
import pytz
import json
import logging
import requests
import pandas as pd

from datetime import datetime, timedelta, date
from dateutil import parser

from agent_c_tools.tools.dynamics_crm.util import get_oauth_token, DEFAULT_FIELDS, ENTITY_MAP, \
    GUID_STRING_TO_ENTITY_RESOLUTION_MAP, LOOKUPS_ID_TO_COMMON_NAME_MAPPING, COMMON_DATE_FIELDS, \
    validate_odata_query
from agent_c_tools.tools.dynamics_crm.util.option_set_mappings import OPTION_SET_MAPPINGS, resolve_option_set


# Consider this approach in future
# https://github.com/GearPlug/dynamics365crm-python/blob/master/dynamics365crm/client.py

class InvalidODataQueryError(Exception):
    pass


class DynamicsAPI:
    def __init__(self, **kwargs):
        self.base_url: str = kwargs.get('base_url')
        if not self.base_url.endswith('/'):
            self.base_url += '/'
        self.client_id: str = kwargs.get('client_id')
        self.dynamics_scope: str = kwargs.get('dynamics_scope')
        self.redirect_uri: str = kwargs.get('redirect_uri')
        self.token_endpoint: str = kwargs.get('token_endpoint')
        self.access_token: str = kwargs.get('access_token')
        self.token_expiration = None
        self.user_id: str = kwargs.get('user_id')
        self.user_pw: str = kwargs.get('user_pw')
        self.whoami_id = None
        self.common_lookups = None
        self.ENTITY_MAP = ENTITY_MAP
        self.DEFAULT_FIELDS = DEFAULT_FIELDS
        self.fixed_field_list_return = True

        # for ease testing
        self.token_file = os.path.join(os.path.dirname(__file__), 'dynamics_token.json')
        self.load_token_from_file()
        self.logger: logging.Logger = logging.getLogger(__name__)

    def save_token_to_file(self):
        token_data = {
            'access_token': self.access_token,
            'expiration_time': self.token_expiration.isoformat() if hasattr(self, 'token_expiration') else None
        }
        with open(self.token_file, 'w') as f:
            json.dump(token_data, f)

    def load_token_from_file(self):
        if os.path.exists(self.token_file):
            with open(self.token_file, 'r') as f:
                token_data = json.load(f)
                self.access_token = token_data.get('access_token')
                expiration_time = token_data.get('expiration_time')
                if expiration_time:
                    self.token_expiration = datetime.fromisoformat(expiration_time)

    @staticmethod
    def convert_dates_to_iso_format(entity):
        date_fields = ['createdon', 'modifiedon', 'scheduledstart', 'scheduledend', 'estimatedclosedate',
                       'actualclosedate']

        for field in date_fields:
            if field in entity and entity[field]:
                try:
                    # Parse the date string to a datetime object
                    datetime_obj = parser.parse(entity[field])

                    # If the datetime is naive (no timezone info), assume it's UTC
                    if datetime_obj.tzinfo is None:
                        datetime_obj = datetime_obj.replace(tzinfo=pytz.UTC)
                    else:
                        # If it has timezone info, convert to UTC
                        datetime_obj = datetime_obj.astimezone(pytz.UTC)

                    # Convert to ISO 8601 format with UTC timezone
                    entity[field] = datetime_obj.isoformat()

                    # Ensure the string ends with 'Z' to indicate UTC
                    if not entity[field].endswith('Z'):
                        entity[field] = entity[field].replace('+00:00', 'Z')

                except (ValueError, TypeError):
                    entity[field] = None  # Set to None if conversion fails

        return entity

    def modify_query_params(self, query_params):
        # Split the query into parts
        parts = query_params.split('&')

        # Find and modify the main $select, if it exists
        select_index = next((i for i, part in enumerate(parts) if part.startswith('$select=')), None)

        if select_index is not None:
            # Replace existing $select with $select=*
            parts[select_index] = '$select=*'
        else:
            # If no $select, add it at the beginning
            parts.insert(0, '$select=*')

        # Rejoin the parts
        modified_query = '&'.join(parts)

        # Ensure we don't modify $select within $expand
        modified_query = re.sub(r'(\$expand=[^&]*)(\$select=[^&]*)', r'\1\2', modified_query)

        return modified_query

    async def fetch_common_lookups(self):
        self.common_lookups = {}
        for entity, fields in LOOKUPS_ID_TO_COMMON_NAME_MAPPING.items():
            id_field = fields['id_field']
            name_field = fields['name_field']

            result = await self.get_entities(entity_type=entity, query_params=f"$select={id_field},{name_field}")

            df = pd.DataFrame(result)
            df.set_index(id_field, inplace=True)
            self.common_lookups[entity] = df[[name_field]]

    def resolve_guids(self, entity_type, df):
        if entity_type.lower() not in GUID_STRING_TO_ENTITY_RESOLUTION_MAP:
            return df

        for field, lookup_entity in GUID_STRING_TO_ENTITY_RESOLUTION_MAP[entity_type].items():
            if field in df.columns:
                lookup_df = self.common_lookups.get(lookup_entity)
                if lookup_df is not None:
                    name_field = LOOKUPS_ID_TO_COMMON_NAME_MAPPING[lookup_entity]['name_field']
                    resolved_field = f"{field}_resolved"
                    df[resolved_field] = df[field].map(lookup_df[name_field])
        return df
    
    def resolve_option_sets(self, df):
        """
        Resolve option set numeric values to text labels.
        
        For fields that are option sets, add a _resolved field with the text value.
        
        Args:
            df: DataFrame with entity data
            
        Returns:
            DataFrame with resolved option set fields
        """
        for field_name in OPTION_SET_MAPPINGS.keys():
            if field_name in df.columns:
                resolved_field = f"{field_name}_resolved"
                df[resolved_field] = df[field_name].apply(
                    lambda x: resolve_option_set(field_name, x) if pd.notna(x) else None
                )
        return df

    def add_web_client_url_to_dataset(self, entity_type, df):
        # Add web client URL
        id_field = self.ENTITY_MAP.get(entity_type.lower(), {}).get('id_field', f"{self.ENTITY_MAP.get(entity_type, {}).get('singular', entity_type)}id")
        if id_field in df.columns:
            df['webclienturl'] = df.apply(lambda row: self.get_web_client_url(entity_type, row[id_field]), axis=1)

        return df

    def clean_date_fields(self, df):
        for field in COMMON_DATE_FIELDS:
            if field in df.columns:
                df[field] = pd.to_datetime(df[field], utc=True).dt.strftime('%Y-%m-%dT%H:%M:%SZ')

    def get_web_client_url(self, entity_type, entity_id):
        # Extract the org name from the base URL
        org_name = self.base_url.split('//')[1].split('.')[0]
        # Construct the web client URL
        singular_entity = self.ENTITY_MAP.get(entity_type, {}).get('singular', entity_type)
        web_url = f"https://{org_name}.crm.dynamics.com/main.aspx?etn={singular_entity}&id={entity_id}&pagetype=entityrecord"
        return web_url

    async def one_time_lookups(self):
        # Get common lookups like WhoAMI ID, BU, industry verticals, SOs, etc.
        # this is to reduce query load on Dynamics by grabbing commonly used entities/fields
        if self.whoami_id is None:
            try:
                # Look up whoami ID
                response = await self.get_entities(entity_type='WhoAmI')
                self.whoami_id = response[0]['UserId']
            except Exception as e:
                self.logger.info(f"Failed to get WhoAmI info: {str(e)}")
                pass

        if self.common_lookups is None:
            # I think this creates a risk for a bug in that I initialize self.common_lookups = None
            # but in the fetch_common_lookups method, I set it to an empty dict. If that method fails it'll be an
            # empty dict.  Therefore, in this exception block, I should set it to None again.
            try:
                # if common look ups fail, tool is still usable, just more expensive
                await self.fetch_common_lookups()
            except Exception as e:
                self.common_lookups = None
                self.logger.info(f"Failed to get common lookups - info: {str(e)}")
                pass

    async def authorize_dynamics(self):
        self.load_token_from_file()

        try:
            auth_code = get_oauth_token('dynamics',
                                        centric_id=self.user_id, centric_pw=self.user_pw,
                                        client_id=self.client_id, redirect_uri=self.redirect_uri,
                                        dynamics_scope=self.dynamics_scope)

            token_data = {
                'grant_type': 'authorization_code',
                'client_id': self.client_id,
                'code': auth_code,
                'redirect_uri': self.redirect_uri,
            }

            token_response = requests.post(self.token_endpoint, data=token_data)
            token_response.raise_for_status()
            access_token = token_response.json()['access_token']
            print(f"{access_token}\n")
            if not access_token:
                raise ValueError("Access token not found in the response")

            # Save the access token to object so we don't have to get it in the future
            self.access_token = access_token
            # for ease of testing
            self.token_expiration = datetime.now() + timedelta(hours=4)
            self.save_token_to_file()
            print("User authenticated, access token received")
        except requests.exceptions.HTTPError as http_err:
            print(f"HTTP error occurred: {http_err}")
            print(f"Response content: {token_response.content}")
        except json.JSONDecodeError:
            print(f"Failed to decode JSON. Response content: {token_response.content}")
        except Exception as e:
            print(f"An error occurred during authorization: {str(e)}")
        return False

    def process_response_data(self, entity_type, data, is_retrieve_multiple=False):
        if entity_type.lower() == 'whoami':
            return data  # Return WhoAmI data as is, without wrapping in a list
        elif is_retrieve_multiple or isinstance(data, dict) and 'value' in data:
            # Handle RetrieveMultiple and any response with a 'value' key
            return data.get('value', [])
        elif entity_type.lower().endswith('metadata'):
            # Handle metadata entities
            return data.get('value', data)
        elif isinstance(data, dict):
            return data  # Return single-record responses as is
        elif isinstance(data, list):
            return data
        else:
            print(f"Unexpected response format for {entity_type}: {type(data)}")
            self.logger.info(f"Unexpected response format for {entity_type}: {type(data)}")
            return data

    def generate_query_params(self, entity_type, user_query_params='', additional_fields=None, additional_expand=None, override_fields=None):
        default_config = self.DEFAULT_FIELDS.get(entity_type, {'select': [], 'expand': {}})

        # Handle select fields
        if override_fields:
            # If override_fields is provided, use only those fields
            select_fields = list(override_fields)
        else:
            # Otherwise use default fields plus any additional fields
            select_fields = list(default_config.get('select', []))
            if additional_fields:
                select_fields.extend(additional_fields)
        select_fields = list(dict.fromkeys(select_fields))  # Remove duplicates

        # Handle expand fields
        expand_config = default_config.get('expand', {}).copy()
        if additional_expand:
            for relation, fields in additional_expand.items():
                if relation in expand_config:
                    expand_config[relation] = list(set(expand_config[relation] + fields))
                else:
                    expand_config[relation] = fields

        # Construct query parts
        query_parts = [f"$select={','.join(select_fields)}"]

        if expand_config:
            expand_parts = []
            for relation, fields in expand_config.items():
                if fields == ['*']:
                    expand_parts.append(relation)
                else:
                    # only expand those that we can
                    expand_parts.append(f"{relation}($select={','.join(fields)})")
            query_parts.append(f"$expand={','.join(expand_parts)}")

        # Add user query params
        if user_query_params:
            # Remove any existing $select and $expand from user query params
            user_query_params = re.sub(r'(\$select|\$expand)=[^&]*(&|$)', '', user_query_params).strip('&')
            if user_query_params:
                query_parts.append(user_query_params)

        return "&".join(query_parts)

    async def get_entities(self, entity_type, entity_id=None, query_params=None, additional_fields=None,
                           additional_expand=None, override_fields=None):
        if self.access_token is None or not self.access_token.strip():
            await self.authorize_dynamics()

        if not validate_odata_query(query_params):
            raise InvalidODataQueryError("Invalid OData query parameters")

        fixed_field_query_params = self.generate_query_params(
            entity_type,
            query_params,
            additional_fields,
            additional_expand,
            override_fields
        )

        headers = {
            'Authorization': f'Bearer {self.access_token}',
            'Accept': 'application/json'
        }

        entity_type = entity_type.lstrip('/')
        url = f"{self.base_url}{entity_type}"

        if entity_id:
            url += f"({entity_id})"

        if self.fixed_field_list_return:
            # Only return fixed fields
            if fixed_field_query_params:
                url += f"?{fixed_field_query_params}"
        else:
            if query_params:
                modified_query_params = self.modify_query_params(query_params)
                url += f"?{modified_query_params}"

        all_entities = []
        while url:
            self.logger.info(f"Fetching entities of type {entity_type} using query: {query_params}\nURL: {url}")
            response = requests.get(url, headers=headers)

            # incase re-authorization is needed
            if response.status_code == 401:
                self.logger.info("Access token expired. Re-authorizing...")
                await self.authorize_dynamics()
                await self.common_lookups()
                headers['Authorization'] = f'Bearer {self.access_token}'
                response = requests.get(url, headers=headers)

            if response.status_code != 200:
                self.logger.info(
                    f"Error: {response.status_code}, {response.text}\nError on {entity_type} using {query_params}\nFull URL: {url}")
                return json.dumps({'error': response.text,
                                   'instruction': 'Evaluate your query parameters to ensure they are properly formed and correct.'})

            data = response.json()
            # entities can be returned in several different data formats, this generalized function should handle them.
            is_retrieve_multiple = 'RetrieveMultiple' in response.url  # or however you determine this
            processed_data = self.process_response_data(entity_type, data, is_retrieve_multiple)
            if isinstance(processed_data, list):
                all_entities.extend(processed_data)
            else:
                all_entities.append(processed_data)

            url = data.get('@odata.nextLink')

        df = pd.DataFrame(all_entities)
        df = self.resolve_guids(entity_type, df)
        df = self.resolve_option_sets(df)  # Add option set resolution
        df = self.add_web_client_url_to_dataset(entity_type, df)
        self.clean_date_fields(df)

        self.logger.info(f"Successfully fetched {len(df)} entities of type {entity_type}\n")
        return df.to_dict('records')

    async def create_entity(self, entity_type, entity_data):
        """
        Create a new entity in Dynamics CRM.

        :param entity_type: The type of entity to create (e.g., 'accounts', 'contacts')
        :param entity_data: A dictionary containing the data for the new entity
        :return: The created entity data if successful, None otherwise
        """
        print(f"Creating new {entity_type} in Dynamics")
        if self.access_token is None or self.access_token.strip() == '':
            await self.authorize_dynamics()

        headers = {
            'Authorization': f'Bearer {self.access_token}',
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        }

        # Remove leading slash from entity_type if present
        entity_type = entity_type.lstrip('/')

        url = f"{self.base_url}{entity_type}"

        try:
            response = requests.post(url, headers=headers, json=entity_data)
            response.raise_for_status()  # Raises an HTTPError for bad responses

            # For successful creation, Dynamics typically returns a 204 No Content
            # The entity ID is usually in the OData-EntityId header
            if response.status_code == 204:
                entity_id = response.headers.get('OData-EntityId')
                if entity_id:
                    # Extract the GUID from the URL in OData-EntityId
                    import re
                    guid_match = re.search(r'\((.*?)\)', entity_id)
                    if guid_match:
                        new_entity_id = guid_match.group(1)
                        print(f"Successfully created {entity_type} with ID: {new_entity_id}")

                        # Fetch the newly created entity
                        new_entity = await self.get_entities(entity_type, entity_id=new_entity_id)
                        return new_entity
                    else:
                        print(f"Created {entity_type}, but couldn't extract ID from response")
                        return None
                else:
                    print(f"Created {entity_type}, but no entity ID was returned")
                    return None
            else:
                print(f"Unexpected response status: {response.status_code}")
                return None

        except requests.exceptions.HTTPError as http_err:
            print(f"HTTP error occurred: {http_err}")
            print(f"Response content: {response.content}")
        except Exception as e:
            print(f"An error occurred while creating the entity: {str(e)}")

        return None
