import base64
import io
import json
import os
import numpy as np
import pandas as pd
import seaborn as sns
import matplotlib.pyplot as plt
import matplotlib.dates as mdates
import logging
from typing import Dict, Optional
from matplotlib.ticker import FuncFormatter

from agent_c.toolsets import Toolset, json_schema
from agent_c.util.logging_utils import LoggingManager
from ...helpers.path_helper import create_unc_path, os_file_system_path


class DataVisualizationTools(Toolset):
    def __init__(self, **kwargs):
        super().__init__(**kwargs, name='data_visualization')
        self.workspace_tool = self.tool_chest.active_tools.get('WorkspaceTools')
        self.dataframe_tool = self.tool_chest.active_tools.get('DataframeTools')
        logging_manager = LoggingManager(self.__class__.__name__)
        self.logger = logging_manager.get_logger()

    async def _load_data(self, in_memory_or_cache: str, data_key: str = None) -> pd.DataFrame:
        if in_memory_or_cache == 'memory':
            df = self.dataframe_tool.dataframe
            if df is None:
                self.logger.error("No data available in memory. Please load data first.")
                raise ValueError("No data available in memory. Please load data first.")
        elif in_memory_or_cache == 'cache':
            if data_key is None:
                self.logger.error("data_key must be provided when loading from cache")
                raise ValueError("data_key must be provided when loading from cache")

            data = self.tool_cache.get(data_key)
            if data is None:
                self.logger.error(f"No data found in tool_cache for key: {data_key}")
                raise ValueError(f"No data found in tool_cache for key: {data_key}")

            if isinstance(data, bytes):
                # It's parquet data, read it into a DataFrame
                df = pd.read_parquet(io.BytesIO(data))
            elif isinstance(data, dict):
                # It's extracted data for plotting
                df = pd.DataFrame(data)
            else:
                self.logger.error(f"Unknown data format in tool_cache for key: {data_key}")
                raise ValueError(f"Unknown data format in tool_cache for key: {data_key}")
        else:
            self.logger.error("Invalid value for in_memory_or_cache. Must be 'memory' or 'cache'.")
            raise ValueError("Invalid value for in_memory_or_cache. Must be 'memory' or 'cache'.")
        self.logger.info(f"Data loaded successfully with shape: {df.shape}")
        return df

    async def _save_plot(self, fig: plt.Figure, output_filename: str, workspace_name: str = 'project', file_path: str = 'plots', tool_context: Optional[Dict] = None) -> Dict[str, str]:
        full_path = f"{file_path}/{output_filename}" if file_path else f"{output_filename}"
        unc_path = create_unc_path(workspace_name, full_path)

        buffer = io.BytesIO()
        fig.savefig(buffer, format='png', dpi=300)
        buffer.seek(0)

        data = buffer.getvalue()
        unc_path = await self.workspace_tool.internal_write_bytes(path=unc_path, data=data, mode="write")
        os_path = os_file_system_path(self.workspace_tool, unc_path)

        content_base64 = base64.b64encode(data).decode('utf-8')

        await self._raise_render_media(
            sent_by_class=self.__class__.__name__,
            sent_by_function='save_plot',
            content_type="image/png",
            content=content_base64,
            content_bytes=data,
            tool_context=tool_context,
        )

        # await self.chat_callback(render_media={"content-type": "image/png", "content_bytes": buffer.getvalue()},
        #                          session_id=session_id)
        self.logger.info(f"Plot created and saved as {os_path} ")
        return {"message": f"Plot created and saved as {unc_path}."}

    @staticmethod
    def _format_large_number(value, pos):
        if value >= 1e9:
            return f'{value / 1e9:.1f}B'
        elif value >= 1e6:
            return f'{value / 1e6:.1f}M'
        elif value >= 1e3:
            return f'{value / 1e3:.0f}K'
        else:
            return f'{value:.0f}'

    def _setup_date_axis(self, ax, df, x_column, x_date_format=None, x_date_locator='auto') -> None:
        try:
            df[x_column] = pd.to_datetime(df[x_column])
        except ValueError as e:
            self.logger.error(f"Failed to parse dates in x-axis column: {str(e)}")
            raise ValueError(f"Failed to parse dates in x-axis column: {str(e)}")

        # Set up date formatter
        if x_date_format:
            date_formatter = mdates.DateFormatter(x_date_format)
        else:
            date_formatter = mdates.AutoDateFormatter(mdates.AutoDateLocator())

        ax.xaxis.set_major_formatter(date_formatter)

        # Set up date locator
        locator_map = {
            'auto': mdates.AutoDateLocator(),
            'day': mdates.DayLocator(),
            'week': mdates.WeekdayLocator(),
            'month': mdates.MonthLocator(),
            'year': mdates.YearLocator()
        }
        ax.xaxis.set_major_locator(locator_map.get(x_date_locator, mdates.AutoDateLocator()))

        plt.xticks(rotation=45)
        ax.figure.autofmt_xdate()
        self.logger.info(f"Date axis for {x_column} set up successfully with format: {x_date_format} and locator: {x_date_locator}")
        return

    @json_schema(
        description="Create a bar chart with 1 to n bars.",
        params={
            'in_memory_or_cache': {'type': 'string', 'enum': ['memory', 'cache'], 'required': True},
            'data_key': {'type': 'string', 'required': False},
            'output_filename': {'type': 'string', 'required': True},
            'workspace_name': {'type': 'string', 'required': False, 'default': 'project'},
            'x_column': {'type': 'string', 'required': True},
            'y_columns': {'type': 'array', 'items': {'type': 'string'}, 'required': True},
            'title': {'type': 'string', 'required': False},
            'x_label': {'type': 'string', 'required': False},
            'y_label': {'type': 'string', 'required': False},
            'secondary_y': {'type': 'array', 'items': {'type': 'string'}, 'required': False},
            'show_data_labels': {'type': 'boolean', 'required': False, 'default': False},
            'x_date_format': {'type': 'string', 'required': False, 'default': '%Y-%m-%d'},
            'x_date_locator': {'type': 'string', 'enum': ['auto', 'day', 'week', 'month', 'year'], 'required': False,
                               'default': 'auto'}

        }
    )
    async def create_bar_chart(self, **kwargs):
        self.logger.info(f"Creating bar chart with parameters: {kwargs}")
        tool_context = kwargs.get('tool_context', {})
        try:
            df = await self._load_data(in_memory_or_cache=kwargs['in_memory_or_cache'], data_key=kwargs.get('data_key'))
            plt.figure(figsize=(12, 6))
            sns.set_style("whitegrid")

            x_column = kwargs['x_column']
            y_columns = kwargs['y_columns']
            secondary_y = kwargs.get('secondary_y', [])
            show_data_labels = kwargs.get('show_data_labels', False)

            # Check if x_column is a date
            is_date_axis = False
            try:
                df[x_column] = pd.to_datetime(df[x_column])
                is_date_axis = True
            except ValueError:
                self.logger.debug(f"Column {x_column} is not a date column. Moving on.")
                pass  # Not a date column, proceed with normal plotting

            # Melt the dataframe for seaborn
            df_melted = df.melt(id_vars=[x_column], value_vars=y_columns, var_name='Variable', value_name='Value')

            # Create the main plot
            ax = sns.barplot(x=x_column, y='Value', hue='Variable', data=df_melted)

            # Handle secondary y-axis
            if secondary_y:
                ax2 = ax.twinx()
                df_secondary = df_melted[df_melted['Variable'].isin(secondary_y)]
                sns.barplot(x=x_column, y='Value', hue='Variable', data=df_secondary, ax=ax2, alpha=0.5)
                ax2.grid(False)  # Turn off grid for secondary axis

            ax.yaxis.set_major_formatter(FuncFormatter(self._format_large_number))
            if secondary_y:
                ax2.yaxis.set_major_formatter(FuncFormatter(self._format_large_number))

            # Set labels and title
            plt.xlabel(kwargs.get('x_label', x_column))
            ax.set_ylabel(kwargs.get('y_label', ', '.join([col for col in y_columns if col not in secondary_y])))
            if secondary_y:
                ax2.set_ylabel(', '.join(secondary_y))
            plt.title(kwargs.get('title', f'Bar Chart of {", ".join(y_columns)} by {x_column}'))

            # Label x-axis and if necessary handle dates
            if is_date_axis:
                self._setup_date_axis(ax, df, x_column, kwargs.get('x_date_format', '%Y-%m-%d'),
                                      kwargs.get('x_date_locator', 'auto'))
            else:
                plt.xticks(rotation=45, ha='right')

            # Add data labels if requested
            if show_data_labels:
                for p in ax.patches:
                    height = p.get_height()
                    ax.annotate(self._format_large_number(height, 0),
                                (p.get_x() + p.get_width() / 2., height),
                                ha='center', va='bottom',
                                xytext=(0, 5), textcoords='offset points')
            # Adjust legend
            handles, labels = ax.get_legend_handles_labels()
            if secondary_y:
                handles2, labels2 = ax2.get_legend_handles_labels()
                handles += handles2
                labels += labels2
            plt.legend(handles, labels, loc='upper left', bbox_to_anchor=(1, 1))

            # Adjust layout and save
            plt.tight_layout()

            result = await self._save_plot(fig=plt.gcf(), output_filename=kwargs['output_filename'],
                                           workspace_name=kwargs.get('workspace_name', 'project'),
                                           tool_context=tool_context)
            self.logger.info(f"Bar chart created and saved successfully. Result: {result}")
            return json.dumps(result)
        except Exception as e:
            self.logger.error(f"Error creating bar chart: {str(e)}")
            return json.dumps({"error": f"Error creating bar chart: {str(e)}"})

    @json_schema(
        description="Create a line chart with 1 to n lines.",
        params={
            'in_memory_or_cache': {'type': 'string', 'enum': ['memory', 'cache'], 'required': True},
            'data_key': {'type': 'string', 'required': False},
            'output_filename': {'type': 'string', 'required': True},
            'workspace_name': {'type': 'string', 'required': False, 'default': 'project'},
            'x_column': {'type': 'string', 'required': True},
            'y_columns': {'type': 'array', 'items': {'type': 'string'}, 'required': True},
            'title': {'type': 'string', 'required': False},
            'x_label': {'type': 'string', 'required': False},
            'y_label': {'type': 'string', 'required': False},
            'secondary_y': {'type': 'array', 'items': {'type': 'string'}, 'required': False},
            'show_markers': {'type': 'boolean', 'required': False, 'default': False},
            'x_date_format': {'type': 'string', 'required': False, 'default': '%Y-%m-%d'},
            'x_date_locator': {'type': 'string', 'enum': ['auto', 'day', 'week', 'month', 'year'], 'required': False,
                               'default': 'auto'}
        }
    )
    async def create_line_chart(self, **kwargs):
        self.logger.info(f"Creating line chart with parameters: {kwargs}")
        tool_context = kwargs.get('tool_context', {})

        try:
            df = await self._load_data(in_memory_or_cache=kwargs['in_memory_or_cache'], data_key=kwargs.get('data_key'))
            plt.figure(figsize=(12, 6))
            sns.set_style("whitegrid")

            x_column = kwargs['x_column']
            y_columns = kwargs['y_columns']
            secondary_y = kwargs.get('secondary_y', [])
            show_markers = kwargs.get('show_markers', False)

            # Check if x_column is a date
            is_date_axis = False
            try:
                df[x_column] = pd.to_datetime(df[x_column])
                is_date_axis = True
            except ValueError:
                self.logger.debug(f"Column {x_column} is not a date column. Moving on.")
                pass  # Not a date column, proceed with normal plotting

            # Create the main plot
            ax = plt.gca()
            for column in y_columns:
                if column not in secondary_y:
                    ax.plot(df[x_column], df[column], marker='o' if show_markers else None, label=column)

            # Handle secondary y-axis
            if secondary_y:
                ax2 = ax.twinx()
                for column in secondary_y:
                    ax2.plot(df[x_column], df[column], marker='s' if show_markers else None, label=column,
                             linestyle='--')
                ax2.grid(False)  # Turn off grid for secondary axis

            ax.yaxis.set_major_formatter(FuncFormatter(self._format_large_number))
            if secondary_y:
                ax2.yaxis.set_major_formatter(FuncFormatter(self._format_large_number))

            # Set labels and title
            plt.xlabel(kwargs.get('x_label', x_column))
            ax.set_ylabel(kwargs.get('y_label', ', '.join([col for col in y_columns if col not in secondary_y])))
            if secondary_y:
                ax2.set_ylabel(', '.join(secondary_y))
            plt.title(kwargs.get('title', f'Line Chart of {", ".join(y_columns)} by {x_column}'))

            # Label x-axis and if necessary handle dates
            if is_date_axis:
                self._setup_date_axis(ax, df, x_column, kwargs.get('x_date_format', '%Y-%m-%d'),
                                      kwargs.get('x_date_locator', 'auto'))
            else:
                plt.xticks(rotation=45, ha='right')

            # Adjust legend
            handles, labels = ax.get_legend_handles_labels()
            if secondary_y:
                handles2, labels2 = ax2.get_legend_handles_labels()
                handles += handles2
                labels += labels2
            plt.legend(handles, labels, loc='upper left', bbox_to_anchor=(1, 1))

            # Adjust layout and save
            plt.tight_layout()

            result = await self._save_plot(fig=plt.gcf(), output_filename=kwargs['output_filename'],
                                           workspace_name=kwargs.get('workspace_name', 'project'),
                                           tool_context=tool_context)
            self.logger.info(f"Line chart created and saved successfully. Result: {result}")
            return json.dumps(result)
        except Exception as e:
            self.logger.error(f"Error creating line chart: {str(e)}")
            return json.dumps({"error": f"Error creating line chart: {str(e)}"})

    @json_schema(
        description="Create a pie chart with one value column and one label column.",
        params={
            'in_memory_or_cache': {'type': 'string', 'enum': ['memory', 'cache'], 'required': True},
            'data_key': {'type': 'string', 'required': False},
            'output_filename': {'type': 'string', 'required': True},
            'workspace_name': {'type': 'string', 'required': False, 'default': 'project'},
            'value_column': {'type': 'string', 'required': True},
            'label_column': {'type': 'string', 'required': True},
            'title': {'type': 'string', 'required': False},
            'sort_values': {'type': 'boolean', 'required': False, 'default': True},
            'show_percentages': {'type': 'boolean', 'required': False, 'default': True},
        }
    )
    async def create_pie_chart(self, **kwargs):
        self.logger.info(f"Creating pie chart with parameters: {kwargs}")
        tool_context = kwargs.get('tool_context', {})
        try:
            df = await self._load_data(in_memory_or_cache=kwargs['in_memory_or_cache'], data_key=kwargs.get('data_key'))
            plt.figure(figsize=(12, 8))
            sns.set_style("whitegrid")

            value_column = kwargs['value_column']
            label_column = kwargs['label_column']
            sort_values = kwargs.get('sort_values', True)
            show_percentages = kwargs.get('show_percentages', True)

            # Prepare the data
            data = df[[label_column, value_column]].copy()
            if sort_values:
                data = data.sort_values(by=value_column, ascending=False)

            values = data[value_column]
            labels = data[label_column]

            # Create the pie chart
            ax = plt.gca()
            autopct = '%1.1f%%' if show_percentages else None
            wedges, texts, autotexts = ax.pie(values, labels=labels, autopct=autopct, startangle=90)

            # Enhance the appearance
            plt.setp(autotexts, size=8, weight="bold")
            plt.setp(texts, size=10)

            # Add a title
            plt.title(kwargs.get('title', f'Pie Chart of {value_column} by {label_column}'))

            # Add a legend
            plt.legend(wedges, labels,
                       title="Categories",
                       loc="center left",
                       bbox_to_anchor=(1, 0, 0.5, 1))

            ax.axis('equal')  # Equal aspect ratio ensures that pie is drawn as a circle
            plt.tight_layout()

            result = await self._save_plot(fig=plt.gcf(), output_filename=kwargs['output_filename'],
                                           workspace_name=kwargs.get('workspace_name', 'project'),
                                           tool_context=tool_context)
            self.logger.info(f"Pie chart created and saved successfully. Result: {result}")
            return json.dumps(result)
        except Exception as e:
            self.logger.error(f"Error creating pie chart: {str(e)}")
            return json.dumps({"error": f"Error creating pie chart: {str(e)}"})

    @json_schema(
        description="Create a histogram plot for 1 to n columns using separate subplots.",
        params={
            'in_memory_or_cache': {'type': 'string', 'enum': ['memory', 'cache'], 'required': True},
            'data_key': {'type': 'string', 'required': False},
            'output_filename': {'type': 'string', 'required': True},
            'workspace_name': {'type': 'string', 'required': False, 'default': 'project'},
            'columns': {'type': 'array', 'items': {'type': 'string'}, 'minItems': 1, 'required': True},
            'title': {'type': 'string', 'required': False},
            'bins': {'type': 'integer', 'required': False, 'default': 30},
            'kde': {'type': 'boolean', 'required': False, 'default': False},
            'figsize': {'type': 'array', 'items': {'type': 'number'}, 'minItems': 2, 'maxItems': 2, 'required': False},
        }
    )
    async def create_histogram_plot(self, **kwargs):
        self.logger.info(f"Creating histogram plot with parameters: {kwargs}")
        try:
            df = await self._load_data(in_memory_or_cache=kwargs['in_memory_or_cache'], data_key=kwargs.get('data_key'))
            columns = kwargs['columns']
            bins = kwargs.get('bins', 30)
            kde = kwargs.get('kde', False)

            # Calculate number of rows and columns for subplots
            n_plots = len(columns)
            n_rows = (n_plots + 1) // 2  # 2 columns of subplots
            n_cols = min(n_plots, 2)

            figsize = kwargs.get('figsize', (12, 6 * n_rows))
            fig, axes = plt.subplots(n_rows, n_cols, figsize=figsize)
            if n_plots == 1:
                axes = np.array([axes])
            axes = axes.flatten()

            sns.set_style("whitegrid")
            colors = plt.cm.tab10(np.linspace(0, 1, n_plots))

            bin_info_dict = {}

            for i, column in enumerate(columns):
                ax = axes[i]
                sns.histplot(data=df, x=column, bins=bins, kde=kde, ax=ax, color=colors[i])

                counts, bin_edges = np.histogram(df[column], bins=bins)
                bin_info = [{"bin_range": f"{bin_edges[j]:.2f}-{bin_edges[j + 1]:.2f}", "count": int(count)}
                            for j, count in enumerate(counts)]
                bin_info_dict[column] = bin_info

                ax.set_title(column)
                ax.set_xlabel('Value')
                ax.set_ylabel('Frequency')

            # Remove any unused subplots
            for i in range(n_plots, len(axes)):
                fig.delaxes(axes[i])

            plt.tight_layout()
            fig.suptitle(kwargs.get('title', 'Multi-Column Histogram Plot'), fontsize=16, y=1.02)

            result = await self._save_plot(fig, kwargs['output_filename'], kwargs.get('workspace_name', 'project'))

            # Add bin information to the result
            result['bin_info'] = bin_info_dict
            self.logger.info(f"Histogram plot created and saved successfully. Result: {result}")
            return json.dumps(result)
        except Exception as e:
            self.logger.error(f"Error creating histogram plot: {str(e)}")
            return json.dumps({"error": f"Error creating histogram plot: {str(e)}"})

    @json_schema(
        description="Create a box plot to visualize the distribution of a numeric dataset across different categories."
                    "x_column is optional and typically represents a categorical variable."
                    "y_column is the numeric variable to be plotted.",
        params={
            'in_memory_or_cache': {'type': 'string', 'enum': ['memory', 'cache'], 'required': True},
            'data_key': {'type': 'string', 'required': False},
            'output_filename': {'type': 'string', 'required': True},
            'workspace_name': {'type': 'string', 'required': False, 'default': 'project'},
            'x_column': {'type': 'string', 'required': False},
            'y_column': {'type': 'string', 'required': True},
            'title': {'type': 'string', 'required': False},
            'x_label': {'type': 'string', 'required': False},
            'y_label': {'type': 'string', 'required': False}
        }
    )
    async def create_box_plot(self, **kwargs):
        self.logger.info(f"Creating box plot with parameters: {kwargs}")
        tool_context = kwargs.get('tool_context', {})
        try:
            # Load data from memory or cache
            df = await self._load_data(in_memory_or_cache=kwargs['in_memory_or_cache'], data_key=kwargs.get('data_key'))
            plt.figure(figsize=(12, 6))
            sns.set_style("whitegrid")

            x_column = kwargs.get('x_column')
            y_column = kwargs['y_column']

            # Create the box plot
            if x_column:
                ax = sns.boxplot(x=x_column, y=y_column, data=df)
            else:
                ax = sns.boxplot(y=y_column, data=df)

            # Format y-axis labels using the custom formatter
            ax.yaxis.set_major_formatter(FuncFormatter(self._format_large_number))

            # Set labels and title
            if x_column:
                plt.xlabel(kwargs.get('x_label', x_column))
            plt.ylabel(kwargs.get('y_label', y_column))
            plt.title(kwargs.get('title', f'Box Plot of {y_column}'))

            # Adjust layout and save
            plt.tight_layout()

            # Save the plot and return result
            result = await self._save_plot(fig=plt.gcf(), output_filename=kwargs['output_filename'],
                                           workspace_name=kwargs.get('workspace_name', 'project'),
                                           tool_context=tool_context)
            self.logger.info(f"Box plot created and saved successfully. Result: {result}")
            return json.dumps(result)
        except Exception as e:
            self.logger.error(f"Error creating box plot: {str(e)}")
            return json.dumps({"error": f"Error creating box plot: {str(e)}"})

    @json_schema(
        description="Create a scatter plot to visualize the relationship between two numeric variables, with an optional regression line.",
        params={
            'in_memory_or_cache': {'type': 'string', 'enum': ['memory', 'cache'], 'required': True},
            'data_key': {'type': 'string', 'required': False},
            'output_filename': {'type': 'string', 'required': True},
            'workspace_name': {'type': 'string', 'required': False, 'default': 'project'},
            'x_column': {'type': 'string', 'required': True},
            'y_column': {'type': 'string', 'required': True},
            'title': {'type': 'string', 'required': False},
            'x_label': {'type': 'string', 'required': False},
            'y_label': {'type': 'string', 'required': False},
            'include_regression_line': {'type': 'boolean', 'required': False, 'default': False},
            'regression_order': {'type': 'integer', 'required': False, 'default': 1},
        }
    )
    async def create_scatter_plot(self, **kwargs):
        self.logger.info(f"Creating scatter plot with parameters: {kwargs}")
        tool_context = kwargs.get('tool_context', {})
        try:
            # Load data from memory or cache
            df = await self._load_data(in_memory_or_cache=kwargs['in_memory_or_cache'], data_key=kwargs.get('data_key'))
            plt.figure(figsize=(12, 6))
            sns.set_style("whitegrid")

            x_column = kwargs['x_column']
            y_column = kwargs['y_column']
            include_regression_line = kwargs.get('include_regression_line', False)
            regression_order = kwargs.get('regression_order', 1)

            # Create the scatter plot
            if include_regression_line:
                # Create scatter plot with regression line
                ax = sns.lmplot(x=x_column, y=y_column, data=df, order=regression_order, aspect=2)
                ax.set_axis_labels(kwargs.get('x_label', x_column), kwargs.get('y_label', y_column))
                plt.title(kwargs.get('title', f'Scatter Plot of {y_column} vs {x_column} with Regression Line'))
            else:
                # Create a simple scatter plot without regression line
                ax = sns.scatterplot(x=x_column, y=y_column, data=df)
                plt.xlabel(kwargs.get('x_label', x_column))
                plt.ylabel(kwargs.get('y_label', y_column))
                plt.title(kwargs.get('title', f'Scatter Plot of {y_column} vs {x_column}'))

            # Format y-axis labels using the custom formatter
            plt.gca().yaxis.set_major_formatter(FuncFormatter(self._format_large_number))

            # Adjust layout and save
            plt.tight_layout()

            # Save the plot and return result
            result = await self._save_plot(fig=plt.gcf(), output_filename=kwargs['output_filename'],
                                           workspace_name=kwargs.get('workspace_name', 'project'),
                                           tool_context=tool_context)
            self.logger.info(f"Scatter plot created and saved successfully. Result: {result}")
            return json.dumps(result)
        except Exception as e:
            self.logger.error(f"Error creating scatter plot: {str(e)}")
            return json.dumps({"error": f"Error creating scatter plot: {str(e)}"})

    @json_schema(
        description="Create a violin plot to visualize the distribution of two or more numeric variables.",
        params={
            'in_memory_or_cache': {'type': 'string', 'enum': ['memory', 'cache'], 'required': True},
            'data_key': {'type': 'string', 'required': False},
            'output_filename': {'type': 'string', 'required': True},
            'workspace_name': {'type': 'string', 'required': False, 'default': 'project'},
            'x_column': {'type': 'string', 'required': True},
            'y_columns': {'type': 'array', 'items': {'type': 'string'}, 'minItems': 2, 'required': True},
            'title': {'type': 'string', 'required': False},
            'x_label': {'type': 'string', 'required': False},
            'y_label': {'type': 'string', 'required': False},
            'split': {'type': 'boolean', 'required': False, 'default': False},
        }
    )
    async def create_violin_plot(self, **kwargs):
        self.logger.info(f"Creating violin plot with parameters: {kwargs}")
        tool_context = kwargs.get('tool_context', {})
        try:
            # Load data from memory or cache
            df = await self._load_data(in_memory_or_cache=kwargs['in_memory_or_cache'], data_key=kwargs.get('data_key'))
            plt.figure(figsize=(12, 6))
            sns.set_style("whitegrid")

            x_column = kwargs['x_column']
            y_columns = kwargs['y_columns']
            split = kwargs.get('split', False)

            # Melt the dataframe for seaborn
            df_melted = df.melt(id_vars=[x_column], value_vars=y_columns, var_name='Variable', value_name='Value')

            # Create the violin plot
            ax = sns.violinplot(x=x_column, y='Value', hue='Variable', data=df_melted, split=split)

            # Format y-axis labels using the custom formatter
            ax.yaxis.set_major_formatter(FuncFormatter(self._format_large_number))

            # Set labels and title
            plt.xlabel(kwargs.get('x_label', x_column))
            plt.ylabel(kwargs.get('y_label', 'Value'))
            plt.title(kwargs.get('title', f'Violin Plot of {", ".join(y_columns)} by {x_column}'))

            # Adjust layout and save
            plt.tight_layout()

            # Save the plot and return result
            result = await self._save_plot(fig=plt.gcf(), output_filename=kwargs['output_filename'],
                                           workspace_name=kwargs.get('workspace_name', 'project'),
                                           tool_context=tool_context)
            self.logger.info(f"Violin plot created and saved successfully. Result: {result}")
            return json.dumps(result)
        except Exception as e:
            self.logger.error(f"Error creating violin plot: {str(e)}")
            return json.dumps({"error": f"Error creating violin plot: {str(e)}"})

    @json_schema(
        description="Create a heatmap to visualize the correlation matrix or data density of two-dimensional data.",
        params={
            'in_memory_or_cache': {'type': 'string', 'enum': ['memory', 'cache'], 'required': True},
            'data_key': {'type': 'string', 'required': False},
            'output_filename': {'type': 'string', 'required': True},
            'workspace_name': {'type': 'string', 'required': False, 'default': 'project'},
            'columns': {'type': 'array', 'items': {'type': 'string'}, 'minItems': 2, 'required': True},
            'title': {'type': 'string', 'required': False},
            'annot': {'type': 'boolean', 'required': False, 'default': True},
            'cmap': {'type': 'string', 'required': False, 'default': 'coolwarm'},
            'figsize': {'type': 'array', 'items': {'type': 'number'}, 'minItems': 2, 'maxItems': 2, 'required': False},
        }
    )
    async def create_heatmap(self, **kwargs):
        self.logger.info(f"Creating heatmap with parameters: {kwargs}")
        tool_context = kwargs.get('tool_context', {})
        try:
            # Load data from memory or cache
            df = await self._load_data(in_memory_or_cache=kwargs['in_memory_or_cache'], data_key=kwargs.get('data_key'))

            # Extract parameters
            columns = kwargs['columns']
            annot = kwargs.get('annot', True)
            cmap = kwargs.get('cmap', 'coolwarm')
            figsize = kwargs.get('figsize', (10, 8))

            # Subset the DataFrame to the specified columns
            df_subset = df[columns]

            # Compute the correlation matrix
            correlation_matrix = df_subset.corr()

            # Create the heatmap plot
            plt.figure(figsize=figsize)
            sns.set_style("whitegrid")
            ax = sns.heatmap(correlation_matrix, annot=annot, cmap=cmap, fmt='.2f')

            # Set title
            plt.title(kwargs.get('title', 'Heatmap of Correlation Matrix'))

            # Adjust layout and save
            plt.tight_layout()

            # Save the plot and return result
            result = await self._save_plot(fig=plt.gcf(), output_filename=kwargs['output_filename'],
                                           workspace_name=kwargs.get('workspace_name', 'project'),
                                           tool_context=tool_context)
            self.logger.info(f"Heatmap created and saved successfully. Result: {result}")
            return json.dumps(result)
        except Exception as e:
            self.logger.error(f"Error creating heatmap: {str(e)}")
            return json.dumps({"error": f"Error creating heatmap: {str(e)}"})

    @json_schema(
        description="Create a pair plot to visualize pairwise relationships in a dataset.",
        params={
            'in_memory_or_cache': {'type': 'string', 'enum': ['memory', 'cache'], 'required': True},
            'data_key': {'type': 'string', 'required': False},
            'output_filename': {'type': 'string', 'required': True},
            'workspace_name': {'type': 'string', 'required': False, 'default': 'project'},
            'columns': {'type': 'array', 'items': {'type': 'string'}, 'minItems': 2, 'required': True},
            'title': {'type': 'string', 'required': False},
            'hue': {'type': 'string', 'required': False},
            'diag_kind': {'type': 'string', 'enum': ['hist', 'kde'], 'required': False, 'default': 'kde'},
            'plot_kind': {'type': 'string', 'enum': ['scatter', 'reg'], 'required': False, 'default': 'scatter'},
            'figsize': {'type': 'array', 'items': {'type': 'number'}, 'minItems': 2, 'maxItems': 2, 'required': False},
        }
    )
    async def create_pairplot(self, **kwargs):
        self.logger.info(f"Creating pair plot with parameters: {kwargs}")
        tool_context = kwargs.get('tool_context', {})
        try:
            # Load data from memory or cache
            df = await self._load_data(in_memory_or_cache=kwargs['in_memory_or_cache'], data_key=kwargs.get('data_key'))

            # Extract parameters
            columns = kwargs['columns']
            hue = kwargs.get('hue', None)
            diag_kind = kwargs.get('diag_kind', 'kde')
            plot_kind = kwargs.get('plot_kind', 'scatter')
            figsize = kwargs.get('figsize', (10, 8))

            # Subset the DataFrame to the specified columns
            df_subset = df[columns]

            # Create the pair plot
            plt.figure(figsize=figsize)
            sns.set_style("whitegrid")
            pairplot = sns.pairplot(df_subset, hue=hue, diag_kind=diag_kind, kind=plot_kind,
                                    height=figsize[0] / len(columns))

            # Set title
            pairplot.fig.suptitle(kwargs.get('title', 'Pairwise Plot of Selected Columns'), y=1.02)

            # Adjust layout and save
            plt.tight_layout()

            # Save the plot and return result
            result = await self._save_plot(fig=pairplot.fig, output_filename=kwargs['output_filename'],
                                           workspace_name=kwargs.get('workspace_name', 'project'),
                                           tool_context=tool_context)
            self.logger.info(f"Pair plot created and saved successfully. Result: {result}")
            return json.dumps(result)
        except Exception as e:
            self.logger.error(f"Error creating pair plot: {str(e)}")
            return json.dumps({"error": f"Error creating pair plot: {str(e)}"})


Toolset.register(DataVisualizationTools, required_tools=['WorkspaceTools', 'DataframeTools'])

