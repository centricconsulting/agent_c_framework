import logging
import os
import json
from typing import Union

from linkedin_api import Linkedin

from agent_c.toolsets import json_schema, Toolset
from agent_c.util import filter_dict_by_keys

class LinkedInTools(Toolset):
    """
    LinkedInTools provides an interface for the agent to interact with LinkedIn profiles.

    This is a DEMO tool that was slapped together in minutes then made "not a complete hack" via AI assist.
    """

    def __init__(self, **kwargs):
        """
        Initializes LinkedInTools by setting up a Linkedin instance with the
        environment credentials and initializing a cache to store profile information.

        :param kwargs: Keyword arguments forwarded to Toolset.
        """
        super().__init__(**kwargs, name='linkedin', needed_keys=['LINKEDIN_UID', 'LINKEDIN_PASSWORD'])
        self.logger: logging.Logger = logging.getLogger(__name__)

    @property
    def linkedin(self) -> Union[Linkedin, None]:
        try:
            #  refresh_cookies=True
            return Linkedin(os.environ.get("LINKEDIN_UID"), os.environ.get("LINKEDIN_PASSWORD"), refresh_cookies=True)
        except Exception as e:
            self.valid = False
            self.logger.debug(f"Failed to initialize Linkedin tool: {e}")
        return None

    @json_schema(
        'Call this to fetch a LinkedIn profile and store it in the cache for additional analysis ',
        {
            'profile_name': {
                'type': 'string',
                'description': 'The profile name to fetch.',
                'required': True
            }
        }
    )
    async def get_profile(self, **kwargs):
        """
        Asynchronously fetches a LinkedIn profile by name and caches it.

        :param kwargs: Contains 'profile_name', the name of the LinkedIn profile to fetch.
                       If 'profile_name' starts with 'CURRENT_USER', fetches the profile for the current user.
        :return: A JSON string representation of the LinkedIn profile data or an error message.
        """
        profile_name: str = kwargs.get('profile_name').replace(" ", "")
        if profile_name.startswith('CURRENT_USER'):
            profile_name = os.environ.get('LINKEDIN_PROFILE')

        cached_profile: Union[str, None] = self.tool_cache.get(f"linkedin://{profile_name}")
        if cached_profile is not None and cached_profile !='{}':
            return cached_profile

        # These are the only keys that are useful to the model and some of these are questionable
        wanted_keys = ['summary', 'industryName', 'lastName', 'locationName', 'student',
                       'geoCountryName', 'firstName', 'geoLocationName', 'location', 'headline',
                       'displayPictureUrl', 'profile_id', 'public_id', 'experience',
                       'education', 'languages', 'publications', 'certifications',
                       'volunteer', 'honors', 'projects']

        try:
            profile = self.linkedin.get_profile(profile_name)
            profile__data = filter_dict_by_keys(profile, wanted_keys)
            cached_profile = json.dumps(profile__data)
        except Exception as e:
            return f"There was an error getting the profile details for {profile_name}.\n{e}"

        self.tool_cache.set(f"linkedin://{profile_name}", cached_profile)

        return cached_profile


Toolset.register(LinkedInTools)
