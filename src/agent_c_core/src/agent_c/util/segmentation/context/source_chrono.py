import statistics
from typing import Dict, Any, List

from agent_c.util.segmentation.context.context_formatter import ContextFormatter, AgentContextModel
from agent_c.util.segmentation.weaviate.text_segment import TextSegment


class SourceChronoFormatter(ContextFormatter):
    def __init__(self, **kwargs: Dict[str, Any]):
        super().__init__(**kwargs)

    def format_segments(self, segments: list[TextSegment]) -> AgentContextModel:
        if len(segments) == 0:
            return ''

        self.segment_count = len(segments)

        deliminator = "@@@"
        model_instructions = ("The contextual information below as been ordered with the most relevant sources first.\n"
                              "- Each context source is wrapped in a <source> element with a citation to identify it.\n"
                              "- Under each source, text segments are ordered in the order they appeared in the source document.\n"
                              "- Parent content is contained within a <parent_context> tag\n"
                              f"- Specific matching segments are contained within a <segment> tag`\n"
                              "<CONTEXT>\n"
                             )

        segments = self.__prune_segments(sorted(segments, key=lambda segment: segment.distance), model_instructions, deliminator)
        grouped_segments: dict = self.__group_segments(segments)

        # We want to embed these in order of their min_distance, in the event of a tie we use med_distance, beyond that it doesn't matter
        sorted_keys = sorted(grouped_segments, key=lambda x: (grouped_segments[x]['min_distance'], grouped_segments[x]['med_distance']))

        self.source_count = len(sorted_keys)

        parts = [model_instructions]

        seen_parents = set()

        for source_name in sorted_keys:
            # Within a given source, the segments are put back in the order they appeared so they don't contextually compete
            source = grouped_segments[source_name]
            source_segments = sorted(source['segments'], key=lambda segment: segment.sequence)

            parts.append(f"<source citation=\"{source_name}\">\n")

            for model in source_segments:
                if model.parent_segment not in seen_parents:
                    parts.append(f"<parent_context>{model.content}</parent_context>\n")
                    seen_parents.add(model.parent_segment)

                parts.append(f"<segment>{model.index_content}</segment>\n")

            parts.append("</source>\n")

        full_context = ''.join(parts).rstrip('\n') + "\n</CONTEXT>\n"
        return AgentContextModel(text=full_context, token_count=self.token_counter.count_tokens(full_context), segment_count=self.segment_count, source_count=self.source_count)

    @staticmethod
    def reduce_segments(segments: List[TextSegment]) -> List[TextSegment]:
        seen_parent_segments = set()
        reduced_list = []

        for segment in segments:
            if segment.parent_segment not in seen_parent_segments:
                seen_parent_segments.add(segment.parent_segment)
                reduced_list.append(segment)

        return reduced_list

    def __prune_segments(self, full_segments: list[TextSegment], model_instructions, deliminator):

        segments = self.reduce_segments(full_segments)

        if self.token_limit == 0 or len(full_segments) == 0:
            return segments

        ins_count = self.token_counter.count_tokens(model_instructions)
        del_count = self.token_counter.count_tokens(f"\n<segment>\n</segment>\n")

        # Lambda to Figure out the number of tokens needed for the attributions...
        attributions_len = lambda seg_list: int(self.token_counter.count_tokens(deliminator.join(list({model.citation for model in seg_list}))))

        # Now the content, taking into account our deliminator
        content_len = lambda seg_list: sum(segment.token_count for segment in seg_list) + (del_count * len(seg_list) + del_count)

        # Now to keep it all readable
        total_len = lambda seg_list: ins_count + content_len(seg_list) + attributions_len(seg_list)

        while total_len(segments) > self.token_limit and len(segments) > 0:
            segments.pop()

        return segments


    def __group_segments(self, segments: list[TextSegment]):
        source_segments: Dict[str, Any] = {}
        empty_source = {'segments': [], 'total_distance': 0, 'min_distance': 0, 'med_distance': 0}
        for segment in segments:
            this_source = source_segments.get(segment.citation, empty_source.copy())
            this_source['segments'].append(segment)
            distances = [segment.distance for segment in this_source['segments']]
            this_source['total_distance'] = sum(distances)
            this_source['min_distance'] = min(distances)
            this_source['med_distance'] = statistics.median(distances)

            source_segments[segment.citation] = this_source

        return source_segments
