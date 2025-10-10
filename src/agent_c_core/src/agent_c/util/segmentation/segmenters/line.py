from typing import List

from agent_c.agent_runtimes.base import TokenCounter
from agent_c.util.segmentation.segmenters.simple_segment import Segment


def segment_lines(context_lines: List[str], context_source: str, chunk_size: int, line_overlap: int, counter: TokenCounter = None) -> List[Segment]:
    """
    Segments a given context into chunks based on the maximum token count.

    Args:
        context_lines (str): The context to be segmented.
        context_source (str): Identifies the source of this context being segmented
        chunk_size (int): The maximum size of a chunk in tokens.
        line_overlap (int): The number of lines to overlap between chunks.
        counter (TokenCounter): A token counter to use for counting tokens, if not given TokenCounter.counter is used.

    Returns:
        List[str]: A list of segmented text chunks.
    """
    # If the context is smaller than the chunk size, return it as a single segment
    counter = counter or TokenCounter.counter()
    full_context = "\n".join(context_lines)
    total_tokens = counter.count_tokens(full_context)
    if total_tokens <= chunk_size:
        return [Segment(content=full_context, token_count=total_tokens, source_name=context_source, sequence=1)]

    current_chunk_size: int = 0
    current_chunk: List[str] = []
    segments: List[Segment] = []
    current_seq: int = 1

    for line in context_lines:
        this_line_size: int =  counter.count_tokens(line)
        if current_chunk_size + this_line_size <= chunk_size:
            current_chunk.append(line)
            current_chunk_size += this_line_size
        else:
            segments.append(Segment(content="\n".join(current_chunk), token_count=current_chunk_size, source_name=context_source, sequence=current_seq))
            current_seq = current_seq + 1

            # Ensure overlap between chunks
            if line_overlap > 0:
                current_chunk = current_chunk[-line_overlap:]
                current_chunk_size = sum(counter.count_tokens(chunk_line) for chunk_line in current_chunk)
            else:
                current_chunk = []
                current_chunk_size = 0

            current_chunk.append(line)

            current_chunk_size += this_line_size

    # Add the last chunk if any remaining data
    if len(current_chunk) > line_overlap:
        segments.append(Segment(content="\n".join(current_chunk), token_count=current_chunk_size, source_name=context_source, sequence=current_seq))

    return segments
