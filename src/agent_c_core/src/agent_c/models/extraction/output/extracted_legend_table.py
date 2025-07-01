from typing import List
from agent_c.models.extraction.output.extracted_table import ExtractedTable
from agent_c.models.extraction.output.legend import LegendEntry, LegendTable


class ExtractedLegendTable(ExtractedTable):
    entries: List[LegendEntry]

    @classmethod
    def from_legend_table(cls, legend_table: LegendTable, **opts):
        return cls(entries=legend_table.entries, **opts)