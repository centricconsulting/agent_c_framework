import itertools
import threading
from typing import Any, Dict, Optional

import yaml

from agent_c.models.agent_config import AgentConfiguration, AgentConfigurationV2
from agent_c.toolsets.json_schema import json_schema
from agent_c.toolsets.tool_set import Toolset
from agent_c_tools.tools.agent_assist.base import AgentAssistToolBase
from agent_c_tools.tools.reverse_engineering.prompt import RevEngSection


class ReverseEngineeringTools(AgentAssistToolBase):
    """
    CssExplorerTools provides methods for working with CSS files in workspaces.
    It enables efficient navigation and manipulation of large CSS files with component-based structure.
    """

    def __init__(self, **kwargs: Any):
        super().__init__(name='rev_eng', **kwargs)
        self.default_extensions = ['.js', '.ts', '.py', '.java', '.c', '.cpp', '.h', '.hpp', '.go',
                                   '.rs', '.rb', '.vb', '.cs', '.php', '.swift', '.kt', '.scala',
                                   '.css', '.scss', '.less', '.asp', '.aspx', '.html', '.htm']
        self.explainable_extensions = ['.js', '.cs', '.py']
        self.section = RevEngSection()
        self.recon_oneshot = self.agent_loader.catalog['recon_oneshot']
        self.recon_revise_oneshot = self.agent_loader.catalog['recon_revise_oneshot']
        self.recon_answers_oneshot = self.agent_loader.catalog['recon_answers_oneshot']

    def is_explainable(self, file: str) -> bool:
        """
        Check if the file is explainable based on its extension.
        :param file: The file path to check.
        :return: True if the file is explainable, False otherwise.
        """
        return any(file.endswith(ext) for ext in self.explainable_extensions)

    async def __try_explain_code(self, file: str) -> str:
        try:
            if not self.is_explainable(file):
                return file

            context = await self.workspace_tool.inspect_code(path=file)
            if context.startswith("{"):
                return file

            return context

        except Exception as e:
            return file


    @json_schema(
        'Perform an in depth analysis of all code files matching a glob pattern. Output will be saved to the scratchpad of the workspace containing the code.',
        {
            'glob': {
                'type': 'string',
                'description': 'A glob pattern in workspace workspace UNC format. Equivalent to `glob.glob` in Python with recursive=True. e.g. //workspace/**/*.js',
                'required': True
            }
        }
    )
    async def analyze_source(self, **kwargs) -> str:
        tool_context: Dict[str, Any] = kwargs["context"]
        client_wants_cancel: threading.Event = tool_context["client_wants_cancel"]
        glob: str = kwargs.get('glob')
        batch_size: int = kwargs.get('batch_size', 2)
        error, workspace, relative_path = self.workspace_tool.validate_and_get_workspace_path(glob)
        if error is not None:
            return f"Error: {error}"

        files = await workspace.glob(relative_path, True)

        await self._pass_one(workspace, files, batch_size, tool_context, client_wants_cancel )
        await self._pass_two(workspace, files, tool_context, client_wants_cancel)

        return f"Processed {len(files)} files. See //{workspace.name}/.scratch/analyze_source/ for results."

    @json_schema(
        'Perform an in depth analysis of all code in a source tree.  Output will be saved to the scratchpad of the workspace containing the code.',
        {
            'start_path': {
                'type': 'string',
                'description': 'A path to a folder, in workspace UNC format, to start the analysis from. Equivalent to `os.walk` in Python. e.g. //workspace/src',
                'required': True
            },
            'extensions': {
                'type': 'array',
                'description': 'A list of file extensions to include in the analysis. e.g. [".js", ".ts", ".py"]',
                'items': {
                    'type': 'string'
                },
                'required': False,
                'default': ['.js', '.ts', '.py', '.java', '.c', '.cpp', '.h', '.hpp', '.go',
                            '.rs', '.rb', '.vb', '.cs', '.php', '.swift', '.kt', '.scala',
                            '.css', '.scss', '.less', '.asp', '.aspx', '.html', '.htm']
            },
        }
    )
    async def analyze_tree(self, **kwargs) -> str:
        tool_context: Dict[str, Any] = kwargs["context"]
        client_wants_cancel: threading.Event = tool_context["client_wants_cancel"]
        start_path: str = kwargs.get('start_path')
        batch_size: int = kwargs.get('batch_size', 2)
        extensions: list[str] = kwargs.get('extensions', self.default_extensions)
        error, workspace, relative_path = self.workspace_tool.validate_and_get_workspace_path(start_path)
        if error is not None:
            return f"Error: {error}"

        error, files = await workspace.walk(relative_path, extensions)
        if error is not None:
            return f"Error: {error}"

        await self._pass_one(workspace, files, batch_size, tool_context, client_wants_cancel)
        await self._pass_two(workspace, files, tool_context, client_wants_cancel)

        return f"Processed {len(files)} files. See //{workspace.name}/.scratch/analyze_source/ for results."

    @json_schema(
        'Make a request of an expert on the analysis of the codebase in a given workspace. The expert will be able to examine any of the output of `analyze_source` in the workspace as well as the code itself to save you precious context window space.',
        {
            'request': {
                'type': 'string',
                'description': 'A question, or request for information about the codebase and/or the analysis done on it.',
                'required': True
            },
            'workspace': {
                'type': 'string',
                'description': 'The workspace containing the output of `analyze_source` you with to query.',
                'required': True
            },
        }
    )
    async def query_analysis(self, **kwargs) -> str:
        tool_context: Dict[str, Any] = kwargs["context"]
        client_wants_cancel: threading.Event = tool_context["client_wants_cancel"]
        request: str = kwargs.get('request')
        workspace_name: str = kwargs.get('workspace')

        workspace = self.workspace_tool.find_workspace_by_name(workspace_name)
        agent_config = self.recon_answers_oneshot.model_dump()
        agent_config['persona'] = agent_config['persona'].replace('[workspace]', workspace_name).replace('[workspace_tree]', await workspace.tree('.scratch/analyze_source/enhanced', 10, 5))
        agent = AgentConfigurationV2(**agent_config)

        messages = await self.agent_oneshot(request, agent, tool_context['session_id'], tool_context, client_wants_cancel=client_wants_cancel)
        last_message = messages[-1] if messages else None

        return yaml.dump(last_message, allow_unicode=True) if last_message else "No response from agent."


    async def _pass_one(self, workspace, files: list[str], batch_size: int, tool_context: Dict[str, any], client_wants_cancel: threading.Event) -> list[str]:
        pass_one_results = []
        if batch_size > 1:
            parallel: bool = True
        else:
            parallel: bool = False
            batch_size = 1

        if parallel:
            iterator = iter(files)
            while batch := list(itertools.islice(iterator, batch_size)):
                if client_wants_cancel.is_set():
                    await self._render_media_markdown("**Processing cancelled by user.**",
                                                      "analyze_source", tool_context=tool_context)
                    return pass_one_results

                paths = "\n- ".join([f"//{workspace.name}/{file}" for file in batch])
                await self._render_media_markdown(f"\n**Processing files (pass 1)**: {paths}... ", "analyze_source", tool_context=tool_context)
                true_batch = [await self.__try_explain_code(f"//{workspace.name}/{file}") for file in batch]
                results = await self.parallel_agent_oneshots(true_batch,
                                                             persona=self.recon_oneshot,
                                                             user_session_id=tool_context['session_id'],
                                                             tool_context=tool_context,
                                                             client_wants_cancel=client_wants_cancel)
                await self._render_media_markdown(f"\nfinished processing files (pass 1):\n- {paths}", "analyze_source", tool_context=tool_context)
                pass_one_results.extend(results)
        else:
            for file in files:
                if client_wants_cancel.is_set():
                    await self._render_media_markdown("**Processing cancelled by user.**", "analyze_source", tool_context=tool_context)
                    return pass_one_results

                await self._render_media_markdown(f"n**Processing file (pass 1)**: {file}... ", "analyze_source", tool_context=tool_context)
                result = await self.agent_oneshot(user_message=await self.__try_explain_code(f"//{workspace.name}/{file}"),
                                                  persona=self.recon_oneshot,
                                                  user_session_id=tool_context['session_id'],
                                                  tool_context=tool_context,
                                                  client_wants_cancel=client_wants_cancel)
                await self._render_media_markdown(f"\nDone processing file (pass 1): {file}", "analyze_source", tool_context=tool_context)
                pass_one_results.append(result)

        return pass_one_results


    async def _pass_two(self, workspace, files: list[str], tool_context: Dict[str, any], client_wants_cancel: threading.Event ) -> list[str]:
        pass_two_results = []

        for file in files:
            if client_wants_cancel.is_set():
                await self._render_media_markdown("**Processing cancelled by user.**", "analyze_source", tool_context=tool_context)
                return pass_two_results

            await self._render_media_markdown(f"\n**Processing file (pass 2)**: {file}... ", "analyze_source", tool_context=tool_context)
            result = await self.agent_oneshot(user_message=f"//{workspace.name}/{file}",
                                              persona=self.recon_revise_oneshot,
                                              user_session_id=tool_context['session_id'],
                                              tool_context=tool_context,
                                              client_wants_cancel=client_wants_cancel)
            pass_two_results.append(result)

        return pass_two_results


# Register the toolset
Toolset.register(ReverseEngineeringTools, required_tools=['WorkspaceTools'])
