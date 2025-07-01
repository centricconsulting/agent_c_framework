import os
import logging
from typing import List, Optional

from langchain.docstore.document import Document
from langchain_community.document_loaders import UnstructuredExcelLoader

from agent_c.agent_runtimes.gpt import TikTokenTokenCounter
from agent_c.util.segmentation.segmenters.centric_excel import ExcelStructuredTableSegmenter
from agent_c.util.segmentation.weaviate.text_segment import TextSegment


class EnhancedExcelLoader:
    """
    Enhanced Excel loader with DataFrame-based processing and fallback mechanism.
    Uses ExcelStructuredTableSegmenter for primary processing with fallback to UnstructuredExcelLoader.
    Supports processing multiple worksheets in an Excel file.
    """

    def __init__(self):
        self.logger = logging.getLogger(__name__)
        self.token_counter = TikTokenTokenCounter()

    def load_excel(self, file_path: str, chunk_size: int = 1000, worksheets: Optional[List[str]] = None) -> List[
        TextSegment]:
        """
        Load an Excel file using the enhanced approach with fallback.

        Args:
            file_path: Path to the Excel file
            chunk_size: Maximum token size for segments
            worksheets: List of specific worksheet names to process (default: all worksheets)

        Returns:
            List of TextSegment objects
        """
        # Primary approach: Use DataFrame-based segmentation
        self.logger.info(f"Processing Excel file {file_path} using DataFrame segmentation")
        excel_segmenter = ExcelStructuredTableSegmenter(
            file_path=file_path,
            chunk_size=chunk_size,
            token_counter=self.token_counter,
            worksheets=worksheets
        )

        # Create segments from all worksheets in the Excel file
        segments = excel_segmenter.segment_excel_file()
        parent_segments = [s for s in segments if s.parent_segment is None]
        child_segments = [s for s in segments if s.parent_segment is not None]

        self.logger.info(f"Excel segmentation complete: {len(segments)} total segments created")
        self.logger.info(f"  - {len(parent_segments)} parent segments")
        self.logger.info(f"  - {len(child_segments)} child segments")
        self.logger.info(f"  - Avg rows per parent: {len(child_segments) / max(1, len(parent_segments)):.1f}")

        # Convert TextSegments to Documents
        # return self._convert_segments_to_documents(segments, file_path)
        return segments

    def _fallback_load_excel(self, file_path: str) -> List[Document]:
        """
        Fallback method using UnstructuredExcelLoader.

        Args:
            file_path: Path to the Excel file

        Returns:
            List of Document objects
        """
        try:
            loader = UnstructuredExcelLoader(file_path, mode='elements')
            documents = loader.load()
            self.logger.info(f"Fallback method successfully loaded {len(documents)} elements from {file_path}")
            return documents
        except Exception as e:
            self.logger.error(f"Both primary and fallback methods failed for {file_path}: {e}")
            # Return an empty list rather than raising an exception
            return []

    def _convert_segments_to_documents(self, segments: List[TextSegment], source: str) -> List[Document]:
        """
        Convert TextSegment objects to Document objects.

        Args:
            segments: List of TextSegment objects
            source: Source file path

        Returns:
            List of Document objects
        """
        documents = []
        for segment in segments:
            # Extract worksheet name from citation if available
            worksheet = None
            if "#sheet=" in segment.citation:
                worksheet = segment.citation.split("#sheet=")[1]

            # Create metadata dictionary
            metadata = {
                "category": "Table",  # Assuming Excel content is tabular
                "source": source,
                "sequence": segment.sequence,
                "citation": segment.citation,
                "token_count": segment.token_count
            }

            # Add worksheet info if available
            if worksheet:
                metadata["worksheet"] = worksheet

            # Add parent_segment if available
            if segment.parent_segment:
                metadata["parent_segment"] = segment.parent_segment

            # Create Document
            doc = Document(
                page_content=segment.content,
                metadata=metadata
            )
            documents.append(doc)

        return documents