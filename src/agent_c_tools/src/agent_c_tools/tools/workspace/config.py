from typing import Annotated, Union, Optional, List, Literal

from pydantic import Field, model_validator
from agent_c.models.base import BaseModel
from agent_c.models.config.base import BaseToolsetConfig


class WorkspaceParamsBase(BaseModel):
    """Parameters for the workspace toolset."""
    workspace_type: Literal['base'] = Field('base',
                                                   description="Type of the workspace object this is for")
    name: str = Field(...,
                      description="Sort name for use in paths, e.g. 'desktop', 'documents'. ")
    description: str = Field("",
                             description="Description of the workspace, e.g. 'My desktop workspace'. ")
    read_only: bool = Field(False,
                            description="If True, all operations will be read-only.")

    @model_validator(mode='after')
    def _ensure_correct_type(self):
        self._init_observable()
        return self

class AllowedCommand(BaseModel):
    """Allowed command for the workspace toolset."""
    command_name: str = Field(...,
                              description="The name of the command, e.g. 'list_files', 'search_files'. ")
    command: str = Field(...,
                         description="The executable command to run, e.g. 'ls -l'. ")
    working_directory: str = Field("",
                                    description="Relative parth withing the workspace to use as the working directory for this command. "),
    description: str = Field("",
                             description="Description of the command, e.g. 'List files in the workspace'. ")
    uid: Optional[int] = Field(None,
                               description="If provided the command will run under the UID of this OS user"
                                           "If not provided, a unique ID will be generated automatically.")
    read_only: bool = Field(False,
                            description="If True, this command will not modify the workspace state.")
    parameters: Optional[List[str]] = Field(None,
                                            description="List of parameters that can be passed to the command. In addition to any passing in the definition")
    allow_extra_parameters: bool = Field(False,
                                         description="If True, extra parameters not listed in 'parameters' will be allowed.")
    example: Optional[str] = Field(None,
                                   description="Example usage of the command.")
    example_output: Optional[str] = Field(None,
                                          description="Example output of the command.")
    example_description: Optional[str] = Field(None,
                                               description="Description of the example output.")

class LocalStorageWorkspaceParams(WorkspaceParamsBase):
    workspace_type: Literal['local_storage'] = Field('local_storage',
                                                     description="Type of the workspace object this is for")
    path: str = Field(...,
                      description="Path to the local storage directory for this workspace. ")
    follow_symlinks: bool = Field(False,
                                   description="If True, symbolic links will be followed when searching for files in the workspace. "
                                               "If False, symbolic links will be treated as regular files.")

    allowed_commands: List[AllowedCommand] = Field(default_factory=list,
                                                   description="List of allowed commands that can be executed in this workspace. ")

class LocalProjectWorkspaceParams(WorkspaceParamsBase):
    workspace_type: Literal['local_project'] = Field('local_project',
                                                     description="Type of the workspace object this is for")
    path: str = Field(...,
                      description="Path to the local storage directory for this workspace. ")
    follow_symlinks: bool = Field(True,
                                   description="If True, symbolic links will be followed when searching for files in the workspace. "
                                               "If False, symbolic links will be treated as regular files.")
    allowed_commands: List[AllowedCommand] = Field(default_factory=list,
                                                   description="List of allowed commands that can be executed in this workspace. ")

class AzureBlobWorkspaceParams(WorkspaceParamsBase):
    workspace_type: Literal['azure_blob'] = Field('azure_blob',
                                                  description="Type of the workspace object this is for")
    container_name: str = Field(...,
                                description="Name of the Azure Blob Storage container for this workspace.")
    connection_string: Optional[str] = Field(None,
                                             description="Connection string for accessing the Azure Blob Storage account.")
    prefix: str = Field("", description="Prefix for the blobs in the container, used to organize blobs within the container.")
    account_name: Optional[str] = Field(None,
                                        description="Name of the Azure Storage account. Required if connection_string is not provided.")
    account_key: Optional[str] = Field(None,
                                       description="Key for the Azure Storage account. Required if connection_string is not provided.")
    sas_token: Optional[str] = Field(None,
                                     description="SAS token for accessing the Azure Blob Storage container. ")

class S3StorageWorkspaceParams(WorkspaceParamsBase):
    workspace_type: Literal['s3_storage'] = Field('s3_storage',
                                                  description="Type of the workspace object this is for")
    bucket_name: str = Field(...,
                             description="Name of the S3 bucket for this workspace.")
    prefix: str = Field("",
                        description="Prefix for the blobs in the container, used to organize blobs within the container.")
    aws_access_key: Optional[str] = Field(None,
                                          description="The AWS access key to use for the interaction")
    aws_secret_key: Optional[str] = Field(None,
                                          description="The AWS secret key to use for the interaction")
    aws_region: Optional[str] = Field(None,
                                      description="The AWS region to use for the interaction")
    aws_session_token: Optional[str] = Field(None,
                                             description="Temporary credentials can be used with aws_session_token.")

WorkspaceParams = Annotated[
    Union[
        LocalStorageWorkspaceParams,
        AzureBlobWorkspaceParams,
        S3StorageWorkspaceParams,
        LocalProjectWorkspaceParams
    ],
    Field(discriminator='workspace_type')
]

class WorkspaceToolsUserConfig(BaseToolsetConfig):
    """Configuration for the workspace tools."""
    workspaces: List[WorkspaceParams] = Field(default_factory=list,
                                              description="List of workspaces available for the agent to use. "
                                                          "Each workspace is defined by its type and parameters.")
    default_workspace: Optional[str] = Field(None,
                                             description="Default workspace for the user")

class WorkspaceToolsConfig(WorkspaceToolsUserConfig):
    path_to_grep: str = Field('grep',
                              description="Path to the grep executable if not on the path, used for searching files in the workspace.")

