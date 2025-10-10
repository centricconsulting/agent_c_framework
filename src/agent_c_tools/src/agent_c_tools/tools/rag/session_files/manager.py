import logging

from typing import List, Any

from agent_c.util.segmentation.segment_repo import SegmentRepo
from agent_c.util.segmentation.segmenters.structured_paragrah import StructuredParagraphSegmenter
from agent_c.toolsets.tool_cache import  ToolCache
from agent_c.chat.session_manager import  ChatSessionManager
from agent_c.agent_runtimes.base import TokenCounter
from agent_c.util.loaders.document_loader import DocumentLoader


class SessionFileManager:

    def __init__(self, **kwargs):
        self.session_manager: ChatSessionManager = kwargs.get('session_mgr')
        self.session_files: dict[str, Any] = {}
        self.session_id: str = 'none'
        self.collection_name: str = 'default'
        self.tool_cache: ToolCache = kwargs.get('tool_cache')
        self.doc_loader: DocumentLoader = kwargs.get('doc_loader', DocumentLoader())
        self.logger = logging.getLogger(__name__)
        self.segment_repo: SegmentRepo = kwargs.get('segment_repo')


    def __refresh_session(self):
        if self.session_id != self.session_manager.chat_session.session_id:
            self.session_id = self.session_manager.chat_session.session_id
            self.session_files = self.session_manager.chat_session.metadata.get("session_files", {})
            self.collection_name = f"Session{self.session_id}"

    @property
    def ui_file_list(self) -> List[str]:
        self.__refresh_session()
        return self.session_files.get("ui_file_list", [])

    def __update_session(self):
        self.session_manager.chat_session.metadata["session_files"] = self.session_files

    def add_paths(self, files: List[str]):
        for file in files:
            self.index_file(file)

    def ui_file_list_changed(self, file_list):
        cur_files = self.ui_file_list

        old_set = set(cur_files)
        file_paths = []

        if file_list is not None:
            file_paths = [file.name for file in file_list]

        logging.info(f"UI File paths updated: {file_paths}.")

        new_set = set(file_paths)

        removed = old_set - new_set
        added = new_set - old_set

        logging.info(f"UI File paths removed: {removed}.")
        logging.info(f"UI File paths added: {added}.")

        self.session_files["ui_file_list"] = file_paths
        self.__update_session()

        for path in added:
            self.index_file(path)


    def __path_elements(self, file_path: str):
        elements = self.tool_cache.get(f"{file_path}_elements")
        if elements is None:
            try:
                self.logger.info(f"Loading elements for {file_path}")
                elements = self.doc_loader.load_document(file_path)
                self.tool_cache.set(f"{file_path}_elements", elements)
            except Exception as e:
                self.logger.exception(f"Failed to load document {file_path}: {e}")
                raise

        return elements

    def __path_segments(self, file_path: str, chunk_size: int = 500):
        segments = self.tool_cache.get(f"{file_path}_segments_{chunk_size}")

        if segments is None:
            try:
                elements = self.__path_elements(file_path)
                self.logger.info(f"Segmenting {file_path}")
                segmenter = StructuredParagraphSegmenter(chunk_size=chunk_size, token_counter=TokenCounter.counter())
                segments = segmenter.segment_elements(elements, file_path)
                self.tool_cache.set(f"{file_path}_segments_{chunk_size}", segments)
            except Exception as e:
                self.logger.exception(f"Failed to load document {file_path}: {e}")
                raise

        return segments

    def index_file(self, file_path: str):
        segments = self.__path_segments(file_path)

        self.segment_repo.create_collection(self.collection_name)
        self.segment_repo.batch_load_segments('session_files', segments)

