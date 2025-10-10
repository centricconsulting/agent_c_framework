import logging
from typing import Any, List, Optional, Tuple
import pandas as pd
import uuid

from agent_c.agent_runtimes.gpt import TikTokenTokenCounter
from agent_c.util.segmentation.segmenters.element_segmenter import ElementSegmenter
from agent_c.util.segmentation.weaviate.text_segment import TextSegment


class ExcelStructuredTableSegmenter(ElementSegmenter):
    """
    A specialized segmenter for Excel files that uses pandas DataFrame.
    Processes all worksheets in an Excel file and creates parent-child segments
    maintaining the table structure.
    """

    def __init__(self, file_path: str = '', chunk_size: int = 1000, token_counter=None,
                 worksheets: Optional[List[str]] = None, **kwargs: Any) -> None:
        """
        Initialize the Excel segmenter.

        Args:
            file_path: Path to the Excel file
            chunk_size: Maximum token size for segments
            token_counter: Counter for tokens (defaults to TikTokenTokenCounter)
            worksheets: List of specific worksheet names to process (default: all worksheets)
            **kwargs: Additional arguments
        """
        self.logger = logging.getLogger(__name__)
        kwargs['chunk_size'] = chunk_size

        # Initialize super class first so we have all base functionality
        super().__init__(**kwargs)

        # Set up token counter and chunk size
        self.token_counter = token_counter or TikTokenTokenCounter()
        self.chunk_size = chunk_size

        # Validate file path
        if not file_path:
            self.logger.error("No file path provided for ExcelStructuredTableSegmenter")
            raise ValueError("File path is required for ExcelStructuredTableSegmenter")

        self.file_path = file_path
        self.worksheets = worksheets

        # Load Excel file information
        try:
            # Get list of all sheet names
            with pd.ExcelFile(file_path) as excel_file:
                self.sheet_names = excel_file.sheet_names

                if worksheets:
                    # Filter to specified worksheets if provided
                    self.sheet_names = [sheet for sheet in self.sheet_names if sheet in worksheets]
                    if not self.sheet_names:
                        self.logger.warning(f"None of the specified worksheets {worksheets} found in {file_path}")

                self.logger.info(
                    f"Excel file contains {len(self.sheet_names)} worksheets to process: {', '.join(self.sheet_names)}")

        except Exception as e:
            self.logger.error(f"Failed to read Excel file structure: {e}")
            raise

    def segment_excel_file(self) -> List[TextSegment]:
        """
        Process the Excel file and create parent-child segments for all worksheets.

        Returns:
            A list of TextSegment objects representing both parent and child segments
        """
        all_segments = []
        sequence_counter = 0

        # Process each worksheet
        for sheet_name in self.sheet_names:
            self.logger.info(f"Processing worksheet: {sheet_name}")

            try:
                # Read the current worksheet
                df = pd.read_excel(self.file_path, sheet_name=sheet_name)

                # Skip empty worksheets
                if df.empty:
                    self.logger.warning(f"Worksheet '{sheet_name}' is empty, skipping")
                    continue

                # Create worksheet specific citation
                worksheet_citation = f"{self.file_path}#sheet={sheet_name}"

                # Process the worksheet to create parent and child segments
                worksheet_segments, next_sequence = self._process_worksheet(
                    df, sheet_name, worksheet_citation, sequence_counter
                )

                # Add worksheet segments to the combined list
                all_segments.extend(worksheet_segments)

                # Update the sequence counter for the next worksheet
                sequence_counter = next_sequence

            except Exception as e:
                self.logger.error(f"Error processing worksheet '{sheet_name}': {e}")
                # Continue with other worksheets instead of failing completely

        self.logger.info(f"Created {len(all_segments)} total segments from {len(self.sheet_names)} worksheets")
        return all_segments

    def _process_worksheet(
            self,
            df: pd.DataFrame,
            sheet_name: str,
            citation: str,
            sequence_start: int
    ) -> Tuple[List[TextSegment], int]:
        """
        Process a single worksheet, creating token-based parent segments
        and row-based child segments.

        Args:
            df: DataFrame containing the worksheet data
            sheet_name: Name of the worksheet
            citation: Citation string including worksheet reference
            sequence_start: Starting sequence number for this worksheet

        Returns:
            Tuple containing (list of segments, next available sequence number)
        """
        all_segments = []
        sequence = sequence_start

        # Create worksheet header
        worksheet_header = f"Worksheet: {sheet_name}"
        worksheet_header_tokens = self.token_counter.count_tokens(worksheet_header)

        # Create header row
        header_text = "\t".join(df.columns.astype(str))
        header_token_count = self.token_counter.count_tokens(header_text)
        newline_token_count = self.token_counter.count_tokens("\n")

        # Calculate the full header cost (worksheet name + column headers)
        full_header = f"{worksheet_header}\n{header_text}"
        full_header_tokens = worksheet_header_tokens + newline_token_count + header_token_count

        # Check if the header alone exceeds chunk size
        if full_header_tokens > self.chunk_size:
            self.logger.warning(f"Header exceeds chunk size for worksheet '{sheet_name}'. Attempting to split header.")
            return self._handle_oversized_header(df, worksheet_header, header_text, citation, sequence)

        # Process the worksheet row by row, creating parent segments
        parent_segments = []
        current_parent_content = full_header
        current_parent_tokens = full_header_tokens
        current_rows = []

        for index, row in df.iterrows():
            # Convert row to string
            row_text = "\t".join([str(item) for item in row.values])
            row_token_count = self.token_counter.count_tokens(row_text)

            # Check if adding this row would exceed chunk size
            row_addition_cost = newline_token_count + row_token_count

            # If a single row is too large for the chunk, we need special handling
            if row_token_count > self.chunk_size - full_header_tokens - newline_token_count:
                self.logger.warning(f"Row {index} exceeds chunk size even by itself. Handling oversized row.")

                # Finalize current parent if it exists
                if current_rows:
                    parent_segment, child_segments, next_seq = self._create_parent_and_children(
                        current_parent_content, current_parent_tokens, current_rows,
                        full_header, citation, sequence, sheet_name
                    )
                    parent_segments.append(parent_segment)
                    all_segments.append(parent_segment)
                    all_segments.extend(child_segments)
                    sequence = next_seq
                    current_parent_content = full_header
                    current_parent_tokens = full_header_tokens
                    current_rows = []

                # Handle the oversized row
                oversized_segments, next_seq = self._handle_oversized_row(
                    row_text, full_header, citation, sequence, sheet_name, index
                )
                all_segments.extend(oversized_segments)
                sequence = next_seq
                continue

            # Regular case: check if we can add this row to the current parent
            if current_parent_tokens + row_addition_cost <= self.chunk_size:
                # Add this row to the current parent
                current_parent_content += f"\n{row_text}"
                current_parent_tokens += row_addition_cost
                current_rows.append((index, row_text, row_token_count))
            else:
                # Current parent is full, finalize it and start a new one
                parent_segment, child_segments, next_seq = self._create_parent_and_children(
                    current_parent_content, current_parent_tokens, current_rows,
                    full_header, citation, sequence, sheet_name
                )
                parent_segments.append(parent_segment)
                all_segments.append(parent_segment)
                all_segments.extend(child_segments)
                sequence = next_seq

                # Start a new parent with just this row
                current_parent_content = f"{full_header}\n{row_text}"
                current_parent_tokens = full_header_tokens + newline_token_count + row_token_count
                current_rows = [(index, row_text, row_token_count)]

        # Finalize the last parent segment if it has content
        if current_rows:
            parent_segment, child_segments, next_seq = self._create_parent_and_children(
                current_parent_content, current_parent_tokens, current_rows,
                full_header, citation, sequence, sheet_name
            )
            parent_segments.append(parent_segment)
            all_segments.append(parent_segment)
            all_segments.extend(child_segments)
            sequence = next_seq

        self.logger.info(
            f"Created {len(parent_segments)} parent segments and {len(all_segments) - len(parent_segments)} child segments")
        return all_segments, sequence

    def _create_parent_and_children(
            self,
            parent_content: str,
            parent_token_count: int,
            rows: List[Tuple[int, str, int]],
            header_text: str,
            citation: str,
            sequence_start: int,
            sheet_name: str
    ) -> Tuple[TextSegment, List[TextSegment], int]:
        """
        Create a parent segment and its child segments.

        Args:
            parent_content: Full content for the parent segment
            parent_token_count: Token count for parent content
            rows: List of (index, row_text, row_token_count) tuples
            header_text: Header text to include in child segments
            citation: Citation string
            sequence_start: Starting sequence number
            sheet_name: Name of the worksheet

        Returns:
            Tuple of (parent_segment, child_segments, next_sequence)
        """
        sequence = sequence_start

        # Create parent segment with a unique UUID
        parent_uuid = str(uuid.uuid4())
        parent_segment = TextSegment(
            uuid=parent_uuid,
            content=parent_content,
            index_content=parent_content,  # Parent is also indexed
            token_count=parent_token_count,
            sequence=sequence,
            citation=citation,
            parent_segment=None  # This is a root segment
        )
        sequence += 1

        # Create child segments, one per row
        child_segments = []
        for row_index, row_text, row_token_count in rows:
            # Create child segment
            # index_content is just one row with the header - currently only indexing the row
            child_index_content = f"{row_text}"
            child_token_count = self.token_counter.count_tokens(child_index_content)

            child_segment = TextSegment(
                uuid=str(uuid.uuid4()),
                content=parent_content,  # Full parent content
                index_content=child_index_content,  # Just this row with header
                token_count=child_token_count,
                sequence=sequence,
                citation=citation,
                parent_segment=parent_uuid  # Link to parent
            )

            child_segments.append(child_segment)
            sequence += 1

        return parent_segment, child_segments, sequence

    def _handle_oversized_header(
            self,
            df: pd.DataFrame,
            worksheet_header: str,
            header_text: str,
            citation: str,
            sequence_start: int
    ) -> Tuple[List[TextSegment], int]:
        """
        Handle the case where the header alone exceeds chunk size.
        Split the header vertically.

        Args:
            df: DataFrame containing the worksheet data
            worksheet_header: Worksheet name header
            header_text: Column headers text
            citation: Citation string
            sequence_start: Starting sequence number

        Returns:
            Tuple containing (list of segments, next available sequence number)
        """
        self.logger.info("Handling oversized header by splitting columns vertically")
        all_segments = []
        sequence = sequence_start

        # Split columns into multiple chunks that fit in chunk size
        columns = df.columns.tolist()
        column_chunks = []
        current_chunk = []
        current_tokens = self.token_counter.count_tokens(worksheet_header)
        newline_token_count = self.token_counter.count_tokens("\n")
        current_tokens += newline_token_count  # Account for newline after worksheet header

        for col in columns:
            col_str = str(col)
            col_tokens = self.token_counter.count_tokens(col_str)

            # Add tab separator if not the first column in the chunk
            if current_chunk:
                col_tokens += self.token_counter.count_tokens("\t")

            if current_tokens + col_tokens <= self.chunk_size:
                current_chunk.append(col)
                current_tokens += col_tokens
            else:
                # This column would exceed chunk size, start a new chunk
                if current_chunk:
                    column_chunks.append(current_chunk)

                # Start a new chunk with just this column
                current_chunk = [col]
                current_tokens = self.token_counter.count_tokens(worksheet_header) + newline_token_count + col_tokens

        # Add the last chunk if it has columns
        if current_chunk:
            column_chunks.append(current_chunk)

        # Process each column chunk as a separate vertical slice of the table
        for chunk_idx, col_chunk in enumerate(column_chunks):
            # Create a subset of the DataFrame with just these columns
            df_subset = df[col_chunk].copy()

            # Generate header for this subset
            subset_header_text = "\t".join([str(col) for col in col_chunk])
            subset_full_header = f"{worksheet_header}\n{subset_header_text}"

            # Process this vertical slice as its own worksheet
            chunk_citation = f"{citation}#vertical_slice={chunk_idx + 1}"
            chunk_segments, next_seq = self._process_worksheet_subset(
                df_subset, subset_full_header, chunk_citation, sequence
            )

            all_segments.extend(chunk_segments)
            sequence = next_seq

        return all_segments, sequence

    def _process_worksheet_subset(
            self,
            df_subset: pd.DataFrame,
            full_header: str,
            citation: str,
            sequence_start: int
    ) -> Tuple[List[TextSegment], int]:
        """
        Process a subset of a worksheet (used for vertical splitting).

        Args:
            df_subset: DataFrame subset to process
            full_header: Full header text for this subset
            citation: Citation string
            sequence_start: Starting sequence number

        Returns:
            Tuple containing (list of segments, next available sequence number)
        """
        all_segments = []
        sequence = sequence_start

        # Calculate token counts
        full_header_tokens = self.token_counter.count_tokens(full_header)
        newline_token_count = self.token_counter.count_tokens("\n")

        # Process the subset row by row, creating parent segments
        current_parent_content = full_header
        current_parent_tokens = full_header_tokens
        current_rows = []

        for index, row in df_subset.iterrows():
            # Convert row to string
            row_text = "\t".join([str(item) for item in row.values])
            row_token_count = self.token_counter.count_tokens(row_text)

            # Check if adding this row would exceed chunk size
            row_addition_cost = newline_token_count + row_token_count

            if current_parent_tokens + row_addition_cost <= self.chunk_size:
                # Add this row to the current parent
                current_parent_content += f"\n{row_text}"
                current_parent_tokens += row_addition_cost
                current_rows.append((index, row_text, row_token_count))
            else:
                # Current parent is full, finalize it and start a new one
                parent_segment, child_segments, next_seq = self._create_parent_and_children(
                    current_parent_content, current_parent_tokens, current_rows,
                    full_header, citation, sequence, ""
                )
                all_segments.append(parent_segment)
                all_segments.extend(child_segments)
                sequence = next_seq

                # Start a new parent with just this row
                current_parent_content = f"{full_header}\n{row_text}"
                current_parent_tokens = full_header_tokens + newline_token_count + row_token_count
                current_rows = [(index, row_text, row_token_count)]

        # Finalize the last parent segment if it has content
        if current_rows:
            parent_segment, child_segments, next_seq = self._create_parent_and_children(
                current_parent_content, current_parent_tokens, current_rows,
                full_header, citation, sequence, ""
            )
            all_segments.append(parent_segment)
            all_segments.extend(child_segments)
            sequence = next_seq

        return all_segments, sequence

    def _handle_oversized_row(
            self,
            row_text: str,
            full_header: str,
            citation: str,
            sequence_start: int,
            sheet_name: str,
            row_index: int
    ) -> Tuple[List[TextSegment], int]:
        """
        Handle a row that's too large even with just the header.
        Create a parent with a truncated version and children with parts of the row.

        Args:
            row_text: Text representation of the row
            full_header: Full header text
            citation: Citation string
            sequence_start: Starting sequence number
            sheet_name: Worksheet name
            row_index: Index of the row

        Returns:
            Tuple containing (list of segments, next available sequence number)
        """
        all_segments = []
        sequence = sequence_start

        # Create a note about this being an oversized row
        note = f"Note: Row {row_index + 1} was too large and has been truncated in the parent segment."

        # Create a truncated version for the parent that fits in chunk size
        full_header_tokens = self.token_counter.count_tokens(full_header)
        note_tokens = self.token_counter.count_tokens(note)
        newline_token_count = self.token_counter.count_tokens("\n")

        # Calculate available tokens for the row in parent
        available_tokens = self.chunk_size - full_header_tokens - newline_token_count - note_tokens - newline_token_count

        # Estimate how many characters we can include
        avg_chars_per_token = 4  # Rough estimate
        char_limit = int(available_tokens * avg_chars_per_token)

        # Create a truncated version of the row
        truncated_row = row_text[:char_limit] + "..."

        # Create parent content with truncated row
        parent_content = f"{full_header}\n{truncated_row}\n{note}"
        parent_token_count = self.token_counter.count_tokens(parent_content)

        # Create parent segment
        parent_uuid = str(uuid.uuid4())
        parent_segment = TextSegment(
            uuid=parent_uuid,
            content=parent_content,
            index_content=parent_content,
            token_count=parent_token_count,
            sequence=sequence,
            citation=citation,
            parent_segment=None
        )
        all_segments.append(parent_segment)
        sequence += 1

        # Now split the oversized row into multiple child segments
        # Each will contain the header + a portion of the row
        remaining_text = row_text
        part = 1

        while remaining_text:
            # Calculate available tokens for this part after header
            available_tokens = self.chunk_size - full_header_tokens - newline_token_count

            # Estimate character limit
            char_limit = int(available_tokens * avg_chars_per_token)

            # Take a chunk of the text
            if len(remaining_text) <= char_limit:
                chunk = remaining_text
                remaining_text = ""
            else:
                # Find a good break point (tab, space, etc.)
                break_chars = ['\t', ';', ',', ' ']
                break_pos = min(char_limit, len(remaining_text) - 1)

                for char in break_chars:
                    pos = remaining_text.rfind(char, 0, break_pos)
                    if pos > 0:  # Found a break point
                        break_pos = pos + 1  # Include the break character
                        break

                chunk = remaining_text[:break_pos]
                remaining_text = remaining_text[break_pos:]

            # Create child segment for this part
            part_note = f"Row {row_index + 1}, Part {part} of the oversized row"
            child_index_content = f"{full_header}\n{part_note}: {chunk}"
            child_token_count = self.token_counter.count_tokens(child_index_content)

            child_segment = TextSegment(
                uuid=str(uuid.uuid4()),
                content=parent_content,  # Full parent content
                index_content=child_index_content,  # This part of the row with header
                token_count=child_token_count,
                sequence=sequence,
                citation=citation,
                parent_segment=parent_uuid
            )

            all_segments.append(child_segment)
            sequence += 1
            part += 1

        return all_segments, sequence