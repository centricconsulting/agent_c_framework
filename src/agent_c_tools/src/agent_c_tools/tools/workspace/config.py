from typing import Annotated, Union, Optional, List, Literal

from pydantic import Field

from agent_c.models.base import BaseModel
from agent_c.models.config.base import BaseToolsetConfig
from agent_c.util import to_snake_case


class WorkspaceParamsBase(BaseModel):
    """Parameters for the workspace toolset."""
    name: str = Field(..., description="Sort name for use in paths, e.g. 'desktop', 'documents'. ")
    description: str = Field("", description="Description of the workspace, e.g. 'My desktop workspace'. ")
    read_only: bool = Field(False, description="If True, all operations will be read-only.")

    def __init__(self, **data) -> None:
        if 'workspace_type' not in data:
            data['workspace_type'] = to_snake_case(self.__class__.__name__.removesuffix('WorkspaceParams'))

        self.super().__init__(**data)

class LocalStorageWorkspaceParams(WorkspaceParamsBase):
    workspace_type: Literal['local_storage'] = Field('local_storage', description="Type of the workspace object this is for")
    path: str = Field(...,
                      description="Path to the local storage directory for this workspace. ")
    follow_symlinks: bool = Field(False,
                                   description="If True, symbolic links will be followed when searching for files in the workspace. "
                                               "If False, symbolic links will be treated as regular files.")

class LocalProjectWorkspaceParams(WorkspaceParamsBase):
    workspace_type: Literal['local_project'] = Field('local_project', description="Type of the workspace object this is for")
    path: str = Field(...,
                      description="Path to the local storage directory for this workspace. ")
    follow_symlinks: bool = Field(False,
                                   description="If True, symbolic links will be followed when searching for files in the workspace. "
                                               "If False, symbolic links will be treated as regular files.")

class AzureBlobWorkspaceParams(WorkspaceParamsBase):
    workspace_type: Literal['azure_blob'] = Field('local_storage', description="Type of the workspace object this is for")
    container_name: str = Field(..., description="Name of the Azure Blob Storage container for this workspace.")
    connection_string: Optional[str] = Field(None, description="Connection string for accessing the Azure Blob Storage account.")
    prefix: str = Field("", description="Prefix for the blobs in the container, used to organize blobs within the container.")
    account_name: Optional[str] = Field(None, description="Name of the Azure Storage account. Required if connection_string is not provided.")
    account_key: Optional[str] = Field(None, description="Key for the Azure Storage account. Required if connection_string is not provided.")
    sas_token: Optional[str] = Field(None, description="SAS token for accessing the Azure Blob Storage container. ")

class S3StorageWorkspaceParams(WorkspaceParamsBase):
    workspace_type: Literal['s3_storage'] = Field('s3_storage', description="Type of the workspace object this is for")
    bucket_name: str = Field(..., description="Name of the S3 bucket for this workspace.")
    prefix: str = Field("", description="Prefix for the blobs in the container, used to organize blobs within the container.")
    aws_access_key: Optional[str] = Field(None, description="The AWS access key to use for the interaction")
    aws_secret_key: Optional[str] = Field(None, description="The AWS secret key to use for the interaction")
    aws_region: Optional[str] = Field(None, description="The AWS region to use for the interaction")
    aws_session_token: Optional[str] = Field(None, description="Temporary credentials can be used with aws_session_token.")

WorkspaceParams = Annotated[
    Union[
        LocalStorageWorkspaceParams,
        AzureBlobWorkspaceParams,
        S3StorageWorkspaceParams,
        LocalProjectWorkspaceParams
    ],
    Field(discriminator='workspace_type')
]

class WorkspaceToolsConfig(BaseToolsetConfig):
    path_to_grep: str = Field('grep',
                              description="Path to the grep executable if not on the path, used for searching files in the workspace.")
    workspaces: List[WorkspaceParams] = Field(default_factory=list,
                                              description="List of workspaces available for the agent to use. "
                                                          "Each workspace is defined by its type and parameters.")

