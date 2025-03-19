from mermaid import Mermaid
from mermaid.graph import Graph
from agent_c_core.agent_c.toolsets import Toolset, json_schema
from agent_c_tools.tools.mermaid_chart.prompt import MermaidChatSection
from agent_c_core.agent_c.util.string import to_snake_case


class MermaidChartTools(Toolset):

    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='mermaidchart', tool_role='mermaid_chart', section=MermaidChatSection())

    @json_schema(
        'Render a mermaid.js graph and return the SVG link.',
        {
            'graph_definition': {
                'type': 'string',
                'description': 'The definition of the mermaid graph using Markdown syntax"',
                'required': True
            },
            'graph_name': {
                'type': 'string',
                'description': 'An optional name for the graph."'
            }
        }
    )
    async def render_graph(self, **kwargs):
        graph_definition = kwargs.get('graph_definition')
        graph_name = kwargs.get('graph_name', "graph")

        await self._raise_text_delta_event(content=f"Rendering graph:\n{graph_definition}")

        graph: Graph = Graph(graph_name, graph_definition)
        rendered_graph: Mermaid = Mermaid(graph)
        svg_link = rendered_graph.svg_response.url

        svg_name: str = to_snake_case(graph_name) + ".svg"
        cache_key = f"chart://{svg_name}"
        self.tool_cache.set(cache_key, rendered_graph.svg_response.text)

        await self._raise_text_delta_event(content=f"\n\nRender complete, cached as: `{cache_key}`.")
        await self._raise_render_media(content_type="image/svg+xml", url=svg_link,
                                       name=svg_name, content=rendered_graph.svg_response.text)

        return f"Render complete, cached as: `{cache_key}`. IF your client supports SVG, it will have been displayed by now."


Toolset.register(MermaidChartTools)
