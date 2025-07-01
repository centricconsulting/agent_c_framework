import logging
import os, re
import json
import argparse
import traceback
import uuid

from typing import Any, List, Tuple
from langchain.docstore.document import Document
from unstructured.documents.elements import Text
from agent_c.util.segmentation.segmenters.element_segmenter import ElementSegmenter
from agent_c.util.segmentation.segmenters.centric_elements import TableHeader, TableRow, TableSeparator
from agent_c.util.segmentation.weaviate.text_segment import TextSegment
from agent_c.util.loaders.simple_document_loader import SimpleDocumentLoader
from agent_c.util.vector_transformers.vector_text_transformer import VectorTextTransformer
from agent_c.util.vector_transformers.vector_transform_options_model import VectorTransformOptionsModel
from nltk.tokenize import sent_tokenize


class StructuredParagraphSegmenter(ElementSegmenter):
    NEWLINE_PATTERN = r'\n\s*\n'
    TABLE_ELEMENT_PREFIX = ("Table", "TableHeader", "TableRow", "TableSeparator")
    LIST_ELEMENT_PREFIX = "List"
    HEADER_CATEGORIES = ("Heading", "Title")

    def __init__(self, **kwargs: Any) -> None:
        super().__init__(**kwargs)
        self.logger = logging.getLogger(__name__)

    def __segment_table(self, cur_segment: TextSegment, table_elements: list[Text], prior_element: Document, seq: int,
                        citation: str):
        metadata = table_elements[0].metadata

        # First lets see if the full table fits in the current segment
        full_table = '\n'.join([element.text for element in table_elements])
        full_table_len = self.token_counter.count_tokens(full_table)
        if full_table_len + cur_segment.token_count <= self.chunk_size:
            cur_segment.content = cur_segment.content + "\n" + full_table
            cur_segment.token_count = cur_segment.token_count + full_table_len
            cur_segment.index_content = cur_segment.content
            return [], cur_segment, seq

        chunks = [cur_segment]

        # Get table title if available
        table_title = ''
        if self.is_header_or_title(prior_element):
            table_title = prior_element.page_content + "\n"

        full_table = f"{table_title}{full_table}"
        full_table_len = self.token_counter.count_tokens(full_table)

        seq = seq + 1

        # Now see if the entire table can fit in a segment by itself
        if full_table_len <= self.chunk_size:
            cur_segment = TextSegment(
                content=str(full_table),
                index_content=str(full_table),  # Set index_content to full table content
                token_count=full_table_len,
                sequence=seq,
                citation=citation
            )
        else:
            # Since it won't fit, let's chunk the table
            # Identify the header - use only the first row as the actual header
            header_row = table_elements[0].text
            separator_row = ""
            if len(table_elements) > 1:
                separator_row = table_elements[1].text

            # Construct repeating header with title and header row
            repeating_header = f"{table_title}{header_row}\n{separator_row}"
            rh_len = self.token_counter.count_tokens(repeating_header)

            # Initialize with header only
            cur_segment = TextSegment(
                content=repeating_header,
                index_content=repeating_header,  # Set index_content
                token_count=rh_len,
                sequence=seq,
                citation=citation
            )

            # Process each row, starting from the third element (after header and separator)
            for row in table_elements[2:]:
                row_len = self.token_counter.count_tokens(row.text)

                # If this row pushes us over, start a new segment with the repeating header
                if row_len + cur_segment.token_count > self.chunk_size:
                    chunks.append(cur_segment)
                    seq = seq + 1

                    # Create new segment with header and current row
                    new_content = f"{repeating_header}\n{row.text}"
                    cur_segment = TextSegment(
                        content=new_content,
                        index_content=new_content,
                        token_count=rh_len + row_len,
                        sequence=seq,
                        citation=citation
                    )
                else:
                    # Row fits in current segment
                    cur_segment.append_content("\n" + row.text, row_len)
                    # Update index_content to match
                    cur_segment.index_content = cur_segment.content

        return chunks, cur_segment, seq

    def __segment_list(self, cur_segment: TextSegment, list_elements: list[Document], prior_element: Text, seq, citation: str):
        metadata = list_elements[0].metadata

        # First lets see if the full list fits in the current
        full_list = '\n'.join([element.page_content for element in list_elements])
        full_list_len = self.token_counter.count_tokens(full_list)
        if full_list_len + cur_segment.token_count <= self.chunk_size:
            cur_segment.append_content(str(full_list), full_list_len)
            return [], cur_segment, seq

        chunks = [cur_segment]

        list_title = ''
        if self.is_header_or_title(prior_element):
            list_title = prior_element.page_content + "\n"

        full_list = f"{list_title}{full_list}"
        full_list_len = self.token_counter.count_tokens(full_list)

        seq = seq + 1

        # Now see if the entire list can fit in a segment by itself
        if full_list_len <= self.chunk_size:
            cur_segment = TextSegment(content=str(full_list), index_content='', token_count=full_list_len, sequence=seq)
        else:
            # Since it won't let's chunk the list
            rh_len = self.token_counter.count_tokens(list_title)

            cur_segment = TextSegment(content=list_title, index_content='', token_count=rh_len, sequence=seq, citation=citation)


            for list_item in list_elements:
                item_len = self.token_counter.count_tokens(list_item.page_content)
                # If this row pushes us over start a new segment with the repeating header
                if item_len + cur_segment.token_count > self.chunk_size:
                    chunks.append(cur_segment)
                    cur_segment = TextSegment(content=list_title + "\n" + list_item.page_content, index_content='',  token_count=rh_len + item_len, sequence=seq, citation=citation)
                    seq = seq + 1
                else:
                    cur_segment.append_content(list_item.page_content, item_len)

        return chunks, cur_segment, seq

    # "Special" segments are ones that are multiple elements that make up a larger elemer
    # like tables or lists.  We want to handle them different
    def handle_special_segments(self, current_segment, special_segments, result_segments, table_elements, list_elements,
                                prior_element, sequence_number, citation):
        """
        Process structured elements like tables and lists, creating appropriate segments.

        Args:
            current_segment: Current text segment being built
            special_segments: Special segments processed in this step
            result_segments: Final list of segments to append to
            table_elements: Table elements to process
            list_elements: List elements to process
            prior_element: Previous element in the document
            sequence_number: Current sequence number
            citation: Citation information

        Returns:
            Tuple with updated current_segment, accumulated_segments, result_segments, sequence_number
        """
        if len(table_elements) > 0:
            special_segments, current_segment, sequence_number = self.__segment_table(current_segment, table_elements, prior_element, sequence_number, citation)
        elif len(list_elements) > 0:
            special_segments, current_segment, sequence_number = self.__segment_list(current_segment, list_elements, prior_element, sequence_number, citation)

        if len(special_segments):
            result_segments = result_segments + special_segments
            special_segments = []

        return current_segment, special_segments, result_segments, sequence_number

    @staticmethod
    def _normalize_text(text: str) -> str:
        """
        Normalize text by replacing multiple newlines with single newlines.
        Strips out multiple newlines so we can get to paragraphs via newlines

        Args:
            text: Raw text to normalize

        Returns:
            Normalized text with single newlines
        """
        return re.sub(StructuredParagraphSegmenter.NEWLINE_PATTERN, '\n', text)

    def _can_fit_in_current_segment(self, text: str, segment: TextSegment) -> bool:
        """
        Check if text can fit in the current segment without exceeding chunk size.

        Args:
            text: Text to check
            segment: Current segment

        Returns:
            True if text fits in segment, False otherwise
        """
        text_len = self.token_counter.count_tokens(text)
        return text_len + segment.token_count <= self.chunk_size

    def _add_to_segment(self, segment: TextSegment, text: str) -> None:
        """
        Add text to a segment and update its token count.

        Args:
            segment: Segment to update
            text: Text to add
        """
        text_len = self.token_counter.count_tokens(text)
        segment.append_content(text, text_len)

    @staticmethod
    def _create_new_segment(seq: int, citation: str) -> TextSegment:
        """
        Create a new empty text segment.

        Args:
            seq: Sequence number for the new segment
            citation: Citation for the new segment

        Returns:
            New empty TextSegment
        """
        return TextSegment(content='', index_content='', token_count=0,
                           sequence=seq, citation=citation)

    def _process_paragraphs(self, text: str, cur_segment: TextSegment,
                            seq: int, citation: str) -> Tuple[List[TextSegment], TextSegment, int]:
        """
        Process text by splitting into paragraphs and handling each appropriately.

        Args:
            text: Text to process
            cur_segment: Current segment being built
            seq: Current sequence number
            citation: Citation for the document

        Returns:
            Tuple of (completed segments, current segment, updated sequence number)
        """
        paragraphs = text.split("\n")
        chunks = []  # Completed segments to return

        # Process each paragraph
        for para in paragraphs:
            para_len = self.token_counter.count_tokens(para)

            # Skip empty paragraphs
            if para_len == 0:
                continue

            # Case 1: Paragraph fits in current segment
            if para_len + cur_segment.token_count <= self.chunk_size:
                self._add_to_segment(cur_segment, para)
                continue

            # Case 2: Current segment has content but paragraph won't fit
            if cur_segment.token_count > 0:
                chunks.append(cur_segment)
                seq += 1
                cur_segment = self._create_new_segment(seq, citation)

            # Case 3: Paragraph itself exceeds chunk size
            if para_len > self.chunk_size:
                new_chunks, cur_segment, seq = self._split_oversized_paragraph(
                    para, cur_segment, seq, citation)
                chunks.extend(new_chunks)
            else:
                # Paragraph fits in empty segment
                self._add_to_segment(cur_segment, para)

        # Handle the final segment
        return self._finalize_segments(chunks, cur_segment, seq)

    def _split_oversized_paragraph(self, para: str, cur_segment: TextSegment,
                                   seq: int, citation: str) -> Tuple[List[TextSegment], TextSegment, int]:
        """
        Split a paragraph that exceeds the maximum chunk size.

        This method first attempts to split by sentences, and if that doesn't work,
        falls back to splitting by individual words.

        Args:
            para: Paragraph text to split
            cur_segment: Current segment being built
            seq: Current sequence number
            citation: Citation for the document

        Returns:
            Tuple of (completed segments, current segment, updated sequence number)
        """
        chunks = []
        sentences = sent_tokenize(para)

        # Check if sentence tokenization failed or produced a single oversized sentence
        if not sentences or (len(sentences) == 1 and
                             self.token_counter.count_tokens(sentences[0]) > self.chunk_size):
            return self._split_by_words(para, cur_segment, seq, citation)

        # Process sentences normally
        return self._process_sentences(sentences, cur_segment, seq, citation)

    def _split_by_words(self, text: str, cur_segment: TextSegment,
                        seq: int, citation: str) -> Tuple[List[TextSegment], TextSegment, int]:
        """
        Split text by individual words when sentences are too large.

        Args:
            text: Text to split
            cur_segment: Current segment being built
            seq: Current sequence number
            citation: Citation for the document

        Returns:
            Tuple of (completed segments, current segment, updated sequence number)
        """
        chunks = []
        words = text.split()
        word_buffer = []
        buffer_len = 0

        for word in words:
            word_len = self.token_counter.count_tokens(word)

            # If adding this word would exceed chunk size
            if buffer_len + word_len > self.chunk_size and buffer_len > 0:
                # Complete the current segment with buffered content
                segment_text = ' '.join(word_buffer)
                self._add_to_segment(cur_segment, segment_text)
                chunks.append(cur_segment)

                # Start a new segment
                seq += 1
                cur_segment = self._create_new_segment(seq, citation)
                word_buffer = [word]
                buffer_len = word_len
            else:
                # Add word to buffer
                word_buffer.append(word)
                buffer_len += word_len

        # Add any remaining content in the buffer
        if buffer_len > 0:
            segment_text = ' '.join(word_buffer)
            self._add_to_segment(cur_segment, segment_text)

        return chunks, cur_segment, seq

    def _process_sentences(self, sentences: List[str], cur_segment: TextSegment,
                           seq: int, citation: str) -> Tuple[List[TextSegment], TextSegment, int]:
        """
        Process a list of sentences, combining them into segments as appropriate.

        Args:
            sentences: List of sentences to process
            cur_segment: Current segment being built
            seq: Current sequence number
            citation: Citation for the document

        Returns:
            Tuple of (completed segments, current segment, updated sequence number)
        """
        chunks = []
        sent_buffer = []
        buffer_len = 0

        for sent in sentences:
            sent_len = self.token_counter.count_tokens(sent)

            # Skip empty sentences
            if sent_len == 0:
                continue

            # Case 1: Single sentence exceeds chunk size
            if sent_len > self.chunk_size:
                # Flush buffer if needed
                if buffer_len > 0:
                    buffer_text = ' '.join(sent_buffer)
                    self._add_to_segment(cur_segment, buffer_text)
                    chunks.append(cur_segment)
                    seq += 1
                    cur_segment = self._create_new_segment(seq, citation)
                    sent_buffer = []
                    buffer_len = 0

                # Handle the oversized sentence by splitting into words
                word_chunks, cur_segment, seq = self._split_by_words(
                    sent, cur_segment, seq, citation)
                chunks.extend(word_chunks)

            # Case 2: Sentence fits in chunk but not with buffer
            elif buffer_len + sent_len > self.chunk_size:
                # Flush buffer and start new segment
                buffer_text = ' '.join(sent_buffer)
                self._add_to_segment(cur_segment, buffer_text)
                chunks.append(cur_segment)
                seq += 1
                cur_segment = self._create_new_segment(seq, citation)
                sent_buffer = [sent]
                buffer_len = sent_len

            # Case 3: Sentence fits with current buffer
            else:
                sent_buffer.append(sent)
                buffer_len += sent_len

        # Add any remaining sentences in the buffer
        if buffer_len > 0:
            buffer_text = ' '.join(sent_buffer)
            self._add_to_segment(cur_segment, buffer_text)

        return chunks, cur_segment, seq

    @staticmethod
    def _finalize_segments(chunks: List[TextSegment], cur_segment: TextSegment,
                           seq: int) -> Tuple[List[TextSegment], TextSegment, int]:
        """
        Finalize segments by handling edge cases with the current segment.

        Args:
            chunks: List of completed segments
            cur_segment: Current segment being built
            seq: Current sequence number

        Returns:
            Tuple of (completed segments, current segment, updated sequence number)
        """
        # Check if we need to add the final segment to chunks
        if cur_segment.token_count > 0 and cur_segment not in chunks:
            return chunks, cur_segment, seq

        # If current segment is empty but we have chunks, return the last one as current
        if cur_segment.token_count == 0 and chunks:
            return chunks[:-1], chunks[-1], seq

        # Otherwise, return all chunks and the current segment
        return chunks, cur_segment, seq

    def handle_text_segment(self, text_element, cur_segment: TextSegment, seq: int, citation: str) -> Tuple[
        List[TextSegment], TextSegment, int]:
        """
        Handle text segmentation with improved handling for paragraphs and large content.

        This implementation ensures:
        1. No empty parent segments are created
        2. Large paragraphs are properly split even without newlines
        3. All text is captured in some parent segment
        4. Better handling of the transition between segments

        Args:
            text_element: Document element containing text to process
            cur_segment: Current segment being built
            seq: Current sequence number
            citation: Citation string for the document

        Returns:
            Tuple of (completed segments, current segment, updated sequence number)
        """
        # Normalize and validate the input text
        text = self._normalize_text(text_element.page_content)
        if not text or text.isspace():
            return [], cur_segment, seq

        # Check if entire text fits in current segment
        if self._can_fit_in_current_segment(text, cur_segment):
            self._add_to_segment(cur_segment, text)
            return [], cur_segment, seq

        # Process text by paragraphs
        return self._process_paragraphs(text, cur_segment, seq, citation)

    def create_parent_child_segments(self, result):
        """
        Create child segments for each parent segment, ensuring every parent has at least one child.

        Args:
            result: List of parent segments

        Returns:
            Combined list of parent and child segments
        """
        sub_segments = []
        for segment in result:
            segment.generate_uuid()
            self.logger.debug(f"***Parent segment before transform: {segment.content[:50]}...")
            segment.index_content = self.vec_transformer.transform_text(segment.content)
            self.logger.debug(f"***Parent segment after transform: {segment.index_content[:50]}...")

            # Tokenize content into sentences and filter out empty/whitespace sentences
            sentences = sent_tokenize(segment.content)
            self.logger.debug(f"Creating {len(sentences)} sub-segments for parent {segment.uuid}")
            meaningful_sentences = [sent for sent in sentences if sent.strip()]

            # Ensure we have at least one sub-segment even if no meaningful sentences
            if not meaningful_sentences:
                self.logger.warning(
                    f"No sentences found in segment with UUID {segment.uuid}, creating a single child with identical content")
                sub_uuid = str(uuid.uuid5(uuid.UUID(segment.uuid), segment.content))
                sub_segment = TextSegment(
                    uuid=sub_uuid,
                    content=segment.content,
                    token_count=segment.token_count,
                    sequence=segment.sequence,
                    citation=segment.citation,
                    parent_segment=segment.uuid,
                    index_content=segment.index_content  # Use the same index_content as parent in this case
                )
                sub_segments.append(sub_segment)
            else:
                for sent in meaningful_sentences:
                    # TODO: Investigate if we need the random statement below.
                    # Create new UUID for each sub-segment, based on both parent ID and sentence content - adding random, because if child sentence is repeated in same parent, it won't get indexed.
                    sub_uuid = str(uuid.uuid5(uuid.UUID(segment.uuid), sent))
                    sub_segment = TextSegment(
                        uuid=sub_uuid,
                        content=segment.content,
                        token_count=segment.token_count,
                        sequence=segment.sequence,
                        citation=segment.citation,
                        parent_segment=segment.uuid,
                        index_content=self.vec_transformer.transform_text(sent)
                    )
                    # self.logger.debug(f"Created sub-segment with parent_segment={sub_segment.parent_segment}")
                    sub_segments.append(sub_segment)

                # If all sentences were empty (unlikely but possible)
                if not sub_segments or not any(s.parent_segment == segment.uuid for s in sub_segments):
                    self.logger.warning(f"All sentences were empty for segment {segment.uuid}, creating default child")
                    sub_uuid = str(uuid.uuid5(uuid.UUID(segment.uuid), segment.content))
                    sub_segment = TextSegment(
                        uuid=sub_uuid,
                        content=segment.content,
                        token_count=segment.token_count,
                        sequence=segment.sequence,
                        citation=segment.citation,
                        parent_segment=segment.uuid,
                        index_content=segment.index_content
                    )
                    sub_segments.append(sub_segment)

        self.logger.debug(f"Total segments created: {len(result) + len(sub_segments)}")
        return result + sub_segments

    @staticmethod
    def is_header_or_title(element):
        """
        Check if an element is a header or title.

        Args:
            element: Document element to check

        Returns:
            Boolean indicating if element is a header or title
        """
        return (element is not None and
                any(element.metadata['category'].startswith(prefix)
                    for prefix in StructuredParagraphSegmenter.HEADER_CATEGORIES))

    def segment_elements(self, elements, citation: str) -> List:
        """
        Segment a list of document elements into text segments with parent-child relationships.

        Args:
            elements: List of document elements to segment
            citation: Citation string for the document

        Returns:
            List of TextSegment objects (parents and children)
        """
        sequence_no = 0
        current_segment = self._create_new_segment(sequence_no, citation)
        result: List[TextSegment] = []
        list_elements: List[Text] = []
        table_elements: List[Text] = []
        special_segments: List[TextSegment] = []
        last_heading = None
        prior_element = None
        current_segment_is_all_headers = False
        for i, element in enumerate(elements):
            element_type = element.metadata['category']
            self.logger.debug(f"Processing element {i}: type={element_type}, content_start={element.page_content[:20]}")

            # Process headers and titles as their own segment
            if self.is_header_or_title(element):
                self.logger.info(f"Processing header or title: {element.page_content}")
                current_segment, special_segments, result, sequence_no = self.handle_special_segments(current_segment, special_segments, result, table_elements, list_elements, last_heading, sequence_no, citation)
                list_elements.clear()
                table_elements.clear()
                last_heading = element

                # Headings / Titles start new chunks, unless they follow each other
                if len(current_segment.content) > 1 and not current_segment_is_all_headers:
                    result.append(current_segment)
                    current_segment = TextSegment(content='', token_count=0, index_content='', sequence=sequence_no, citation=citation)
                    current_segment_is_all_headers = True
                    sequence_no = sequence_no + 1

                current_segment.append_content(element.page_content, self.token_counter.count_tokens(element.page_content))


                # Set the prior element so that the table / list code can use this as their header if it
                # directly proceeds the table / list.
                prior_element = element

            # Process tables as their own segment
            elif any(element_type.startswith(prefix) for prefix in self.TABLE_ELEMENT_PREFIX):
                current_segment_is_all_headers = False
                # We don't set the prior element for lists or tables so that we can keep track of the header
                # for them if one exists.
                current_segment, special_segments, result, sequence_no = self.handle_special_segments(current_segment, special_segments, result, [], list_elements, prior_element, sequence_no, citation)
                list_elements.clear()
                table_elements.clear()

                try:
                    table_parts = element.page_content.split("\n")
                    if len(table_parts) > 0:
                        header = TableHeader(table_parts[0])
                        header.metadata = element.metadata
                        table_elements.append(header)
                        if len(table_parts) > 1:
                            table_elements.append(TableSeparator(table_parts[1]))
                            for row in table_parts[2:]: # Changed index to avoid duplicating header row
                                table_elements.append(TableRow(row))
                        self.logger.info(f"Processing table with {len(table_elements)} elements")
                        table_segments, current_segment, sequence_no = self.__segment_table(
                            current_segment, table_elements, prior_element, sequence_no, citation)
                        result.extend(table_segments)
                        table_elements.clear()
                    else:
                        self.logger.info(f"Empty table detected, skipping")
                except Exception:
                    logging.error(f"Exception in segmenting a table element {element_type}: {traceback.format_exc()}")
                    self.logger.error(traceback.format_exc())
                    logging.info(f"Attempting fallback segmenting for table element {element_type}")
                    fallback_content = element.page_content
                    fallback_token_count = self.token_counter.count_tokens(fallback_content)
                    fallback_segment = TextSegment(
                        content=fallback_content,
                        index_content='',
                        token_count=fallback_token_count,
                        sequence=sequence_no,
                        citation=citation
                    )
                    result.append(fallback_segment)
                    sequence_no += 1

            # Process Lists as their own segment
            elif element_type.startswith(StructuredParagraphSegmenter.LIST_ELEMENT_PREFIX):
                # This probably needs try/except like for tables.
                current_segment_is_all_headers = False
                # We don't set the prior element for lists or tables so that we can keep track of the header for them if one exists.
                current_segment, special_segments, result, sequence_no = self.handle_special_segments(current_segment,
                                                                                         special_segments, result,
                                                                                         table_elements, [],
                                                                                         prior_element, sequence_no, citation)
                table_elements.clear()
                list_elements.clear()

                list_elements.append(element)

                self.logger.info(f"Processing list with {len(list_elements)} elements")
                list_segments, current_segment, sequence_no = self.__segment_list(
                    current_segment, list_elements, prior_element, sequence_no, citation)
                result.extend(list_segments)
                list_elements.clear()

            # Everything else must be a text paragraph
            else:
                current_segment_is_all_headers = False
                # Since this isn't a list or table element we'll set the prior element.
                prior_element = element
                current_segment, special_segments, result, sequence_no = self.handle_special_segments(current_segment, special_segments, result, table_elements, list_elements, last_heading, sequence_no, citation)
                table_elements.clear()
                list_elements.clear()

                # Split this into paragraph segments
                new_segments, current_segment, sequence_no = self.handle_text_segment(element, current_segment, sequence_no, citation)
                result.extend(new_segments)

        if len(current_segment.content) > 0:
            result.append(current_segment)

        # Create parent-child relationships
        return self.create_parent_child_segments(result)

def main():
    parser = argparse.ArgumentParser(description='Loader test app that outputs json')
    parser.add_argument('filename', help='The filename of the document to load.')

    args = parser.parse_args()
    opts = VectorTransformOptionsModel(lemmatize=True)

    loader = SimpleDocumentLoader()
    data = loader.load_document(args.filename)

    vec_t = VectorTextTransformer(opts)

    segmenter = StructuredParagraphSegmenter(vec_transformer=vec_t)
    segments = segmenter.segment_elements(data, args.filename)

    filename_without_extension, _ = os.path.splitext(args.filename)
    new_filename = filename_without_extension + '.json'

    with open(new_filename, 'w') as f:
        json.dump(segments, f, indent=4)


if __name__ == '__main__':
    main()

