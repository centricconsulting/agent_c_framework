#!/usr/bin/env python3
"""
YAML to HTML Project Viewer Converter for Workspace Planning

Converts workspace planning YAML files with task hierarchies into interactive HTML viewers.
Supports collapsible/expandable task trees with nice styling.
Automatically converts numbered lists (1) Item 2) Item or 1. Item 2. Item) 
into proper HTML ordered lists for better readability.

Adapted for use within the Agent C framework workspace planning tools.
"""

import re
from datetime import datetime
from typing import Dict, List, Any, Optional

from agent_c_tools.tools.workspace_planning import PlanModel


class PlanHTMLConverter:
    """Converts workspace planning data to interactive HTML reports."""
    
    def __init__(self):
        self.html_template = """<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>{title}</title>
    <style>
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}

        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            color: #232d3d;
            background: linear-gradient(135deg, #ebebeb 0%, #6ca7d0 100%);
            min-height: 100vh;
            padding: 20px;
        }}

        .container {{
            max-width: 1200px;
            margin: 0 auto;
            background: white;
            border-radius: 15px;
            box-shadow: 0 20px 40px rgba(44,26,93,0.1);
            overflow: hidden;
        }}

        .header {{
            background: white;
            color: black;
            padding: 30px;
            text-align: center;
            position: relative;
        }}

        .header .logo {{
            margin-bottom: 20px;
        }}

        .header .logo svg {{
            width: 120px;
            height: 120px;
        }}

        .header h1 {{
            font-size: 2.5em;
            margin-bottom: 10px;
            font-weight: 300;
        }}

        .header .subtitle {{
            font-size: 1.1em;
            opacity: 0.9;
        }}

        .project-meta {{
            padding: 20px 30px;
            background: #ebebeb;
            border-bottom: 1px solid #67686a;
        }}

        .meta-item {{
            display: inline-block;
            margin: 5px 15px 5px 0;
            padding: 5px 12px;
            background: white;
            border-radius: 20px;
            font-size: 0.9em;
            box-shadow: 0 2px 4px rgba(44,26,93,0.1);
            border: 1px solid #fdb825;
        }}

        .content {{
            padding: 30px;
        }}

        .lessons-learned {{
            background: #ebebeb;
            padding: 20px;
            border-radius: 10px;
            margin-bottom: 30px;
            border-left: 4px solid #fdb825;
        }}

        .lessons-learned h3 {{
            color: #2c1a5d;
            margin-bottom: 15px;
            font-size: 1.3em;
        }}

        .lesson-item {{
            background: white;
            padding: 15px;
            margin: 10px 0;
            border-radius: 8px;
            border-left: 3px solid #2fb677;
        }}

        .task-tree {{
            list-style: none;
        }}

        .task-item {{
            margin: 8px 0;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 2px 8px rgba(44,26,93,0.08);
            background: white;
        }}

        .task-header {{
            padding: 15px 20px;
            cursor: pointer;
            display: flex;
            align-items: center;
            transition: all 0.3s ease;
            border-left: 4px solid #2c1a5d;
        }}

        .task-header:hover {{
            background: #ebebeb;
            transform: translateX(2px);
        }}

        .task-header.phase {{
            background: linear-gradient(90deg, #2c1a5d, #1f5d82);
            color: white;
            border-left: 4px solid #fdb825;
            font-weight: bold;
        }}

        .task-header.phase:hover {{
            background: linear-gradient(90deg, #1f1347, #1a4f6e);
        }}

        .task-header.completed {{
            background: linear-gradient(90deg, #2fb677, #2d84bb);
        }}
        
        .task-header.in-progress {{
            background: linear-gradient(90deg, #fdb825, #6ca7d0);
        }}

        .completion-status {{
            margin-right: 12px;
            font-size: 16px;
            width: 20px;
            text-align: center;
        }}

        .expand-icon {{
            margin-right: 12px;
            font-size: 12px;
            transition: transform 0.3s ease;
            width: 16px;
            text-align: center;
        }}

        .expand-icon.expanded {{
            transform: rotate(90deg);
        }}

        .task-title {{
            flex: 1;
            font-weight: 600;
            font-size: 1.1em;
        }}

        .task-priority {{
            padding: 4px 12px;
            border-radius: 12px;
            font-size: 0.8em;
            font-weight: bold;
            text-transform: uppercase;
        }}

        .priority-high {{
            background: white;
            color: #2c1a5d;
            border: 2px solid #2c1a5d;
            box-shadow: 0 2px 4px rgba(44,26,93,0.3);
        }}

        .priority-medium {{
            background: white;
            color: #1f5d82;
            border: 2px solid #1f5d82;
            font-weight: 900;
        }}

        .priority-low {{
            background: white;
            color: #6ca7d0;
            border: 2px solid #6ca7d0;
            box-shadow: 0 2px 4px rgba(31,93,130,0.3);
        }}

        .task-details {{
            display: none;
            padding: 20px;
            background: #ebebeb;
            border-top: 1px solid #67686a;
        }}

        .task-details.expanded {{
            display: block;
            animation: slideDown 0.3s ease;
        }}

        @keyframes slideDown {{
            from {{
                opacity: 0;
                max-height: 0;
            }}
            to {{
                opacity: 1;
                max-height: 1000px;
            }}
        }}

        .task-description {{
            margin-bottom: 15px;
            font-size: 1.05em;
            color: #232d3d;
        }}

        .task-context {{
            background: white;
            padding: 15px;
            border-radius: 8px;
            border-left: 4px solid #2d84bb;
            font-size: 0.95em;
            line-height: 1.7;
            white-space: pre-wrap;
        }}

        .stats {{
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
            gap: 20px;
            margin-bottom: 30px;
        }}

        .stat-card {{
            background: white;
            padding: 20px;
            border-radius: 10px;
            text-align: center;
            box-shadow: 0 4px 12px rgba(44,26,93,0.1);
            border-top: 3px solid #fdb825;
        }}

        .stat-number {{
            font-size: 2em;
            font-weight: bold;
            color: #2c1a5d;
        }}

        .stat-label {{
            color: #67686a;
            font-size: 0.9em;
            margin-top: 5px;
        }}

        .no-children {{
            opacity: 0.3;
        }}

        .child-tasks {{
            list-style: none;
            margin-left: 30px;
            margin-top: 10px;
        }}

        .task-meta {{
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 10px;
            margin-top: 15px;
        }}

        .meta-field {{
            background: white;
            padding: 10px;
            border-radius: 6px;
            font-size: 0.9em;
        }}

        .meta-label {{
            font-weight: bold;
            color: #67686a;
            display: block;
            margin-bottom: 4px;
        }}
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <div class="logo">
                <svg viewBox="0 0 500 500" xmlns="http://www.w3.org/2000/svg">
                    <!-- Orange concentric arcs (target/bullseye) -->
                    <path d="M250 150 A100 100 0 0 0 150 250 A100 100 0 0 0 250 350" stroke="#fdb825" stroke-width="30" fill="none"/>
                    <path d="M250 180 A70 70 0 0 0 180 250 A70 70 0 0 0 250 320" stroke="#fdb825" stroke-width="20" fill="none"/>

                    <!-- Purple center circle -->
                    <circle cx="250" cy="250" r="40" fill="#2c1a5d"/>

                    <!-- Connecting lines and nodes -->
                    <line x1="290" y1="250" x2="350" y2="250" stroke="#2c1a5d" stroke-width="10"/>
                    <circle cx="350" cy="250" r="15" fill="#2c1a5d"/>

                    <line x1="277" y1="213" x2="327" y2="163" stroke="#2c1a5d" stroke-width="10"/>
                    <circle cx="327" cy="163" r="15" fill="#2c1a5d"/>

                    <line x1="277" y1="287" x2="327" y2="337" stroke="#2c1a5d" stroke-width="10"/>
                    <circle cx="327" cy="337" r="15" fill="#2c1a5d"/>

                    <line x1="250" y1="290" x2="250" y2="350" stroke="#2c1a5d" stroke-width="10"/>
                    <circle cx="250" cy="350" r="15" fill="#2c1a5d"/>

                    <!-- Text: "AGENT C" -->
                    <text x="250" y="420" font-family="Arial, sans-serif" font-weight="bold" font-size="60" fill="#2c1a5d" text-anchor="middle">AGENT C</text>

                    <!-- Text: "CENTRIC CONSULTING" -->
                    <text x="250" y="460" font-family="Arial, sans-serif" font-weight="bold" font-size="24" fill="#2c1a5d" text-anchor="middle">CENTRIC CONSULTING</text>
                </svg>
            </div>
            <h1>{title}</h1>
            <p class="subtitle">{description}</p>
        </div>

        <div class="project-meta">
            {meta_items}
        </div>

        <div class="content">
            <div class="stats">
                {stats}
            </div>

            {lessons_learned}

            <ul class="task-tree" id="taskTree">
                {task_tree}
            </ul>
        </div>
    </div>

    <script>
        function toggleTask(element) {{
            const details = element.nextElementSibling;
            const icon = element.querySelector('.expand-icon');

            if (details && details.classList.contains('task-details')) {{
                if (details.classList.contains('expanded')) {{
                    details.classList.remove('expanded');
                    icon.classList.remove('expanded');
                }} else {{
                    details.classList.add('expanded');
                    icon.classList.add('expanded');
                }}
            }}
        }}

        // Auto-expand phase headers on load
        document.addEventListener('DOMContentLoaded', function() {{
            const phaseHeaders = document.querySelectorAll('.task-header.phase');
            phaseHeaders.forEach(header => {{
                // Uncomment the line below to auto-expand all phases
                // toggleTask(header);
            }});
        }});
    </script>
</body>
</html>"""

    @staticmethod
    def parse_workspace_yaml(yaml_content: str) -> Dict[str, Any]:
        """Parse workspace YAML content and return data structure"""
        try:
            import yaml
            return yaml.safe_load(yaml_content)
        except Exception as e:
            raise ValueError(f"Error parsing YAML content: {e}")

    @staticmethod
    def get_task_hierarchy(tasks: Dict[str, Dict]) -> List[Dict]:
        """Build hierarchical task structure"""
        # Find root tasks (no parent_id or parent_id is None)
        root_tasks = []
        task_map = {}

        # Create a map of all tasks
        for task_id, task_data in tasks.items():
            task_data['id'] = task_id
            task_map[task_id] = task_data

        # Build hierarchy
        for task_id, task_data in tasks.items():
            parent_id = task_data.get('parent_id')
            if not parent_id:
                root_tasks.append(task_data)
            else:
                if parent_id in task_map:
                    if 'children' not in task_map[parent_id]:
                        task_map[parent_id]['children'] = []
                    task_map[parent_id]['children'].append(task_data)

        # Sort by sequence if available
        def sort_by_sequence(task_list):
            return sorted(task_list, key=lambda x: x.get('sequence', 999) if x.get('sequence') is not None else 999)

        root_tasks = sort_by_sequence(root_tasks)
        for task in task_map.values():
            if 'children' in task:
                task['children'] = sort_by_sequence(task['children'])

        return root_tasks

    @staticmethod
    def convert_numbered_lists(text: str) -> str:
        """Convert numbered lists in text to HTML ordered lists (more conservative approach)"""
        if not text:
            return text

        # Look for pattern like "1) content 2) content 3) content"
        # Use a simpler approach: find all "number) " patterns and see if they form a sequence

        # Find all potential list items with pattern "digit) "
        pattern = r'(\d+)\)\s+'
        positions = []
        for match in re.finditer(pattern, text):
            positions.append((int(match.group(1)), match.start(), match.end()))

        # Check if we have a valid sequence
        if len(positions) < 2:
            return text

        # Sort by number
        positions.sort(key=lambda x: x[0])

        # Check if it starts with 1 and is reasonably sequential
        numbers = [pos[0] for pos in positions]
        if numbers[0] != 1:
            return text

        # Allow for reasonable sequence (1,2,3,4... or 1,2,3,5... but not big gaps)
        for i in range(1, len(numbers)):
            if numbers[i] <= numbers[i - 1] or numbers[i] - numbers[i - 1] > 3:
                return text

        # Extract the content between list items
        list_items = []
        for i, (num, start, end) in enumerate(positions):
            # Content starts after "digit) "
            content_start = end

            # Content ends at the start of the next item, or at end of text
            if i < len(positions) - 1:
                content_end = positions[i + 1][1]
            else:
                content_end = len(text)

            content = text[content_start:content_end].strip()

            # Validate content - should be reasonable length and not just fragments
            if len(content) < 5:
                return text

            list_items.append(content)

        # Check if this looks like a real list (not fragments)
        # Look for common sentence patterns that indicate it's not a list
        first_item_start = positions[0][1]
        text_before = text[:first_item_start].strip()

        # If text before ends with ":" it's probably a real list
        is_likely_list = text_before.endswith(':') or text_before.endswith(' of:')

        # Also check if items are substantial
        avg_length = sum(len(item) for item in list_items) / len(list_items)
        is_likely_list = is_likely_list or avg_length > 20

        if not is_likely_list:
            return text

        # Build the HTML
        last_item_end = positions[-1][1] + len(f"{positions[-1][0]}) {list_items[-1]}")
        before_list = text[:first_item_start].strip()
        after_list = text[last_item_end:].strip()

        result = ""
        if before_list:
            result += before_list + " "

        result += "<ol>"
        for item in list_items:
            clean_item = item.rstrip('.,;').strip()
            result += f"<li>{clean_item}</li>"
        result += "</ol>"

        if after_list:
            result += " " + after_list

        return result

    def generate_task_html(self, task: Dict, is_phase: bool = False) -> str:
        """Generate HTML for a single task"""
        task_id = task.get('id', '')
        title = task.get('title', 'Untitled Task')
        description = task.get('description', '')
        context = task.get('context', '')
        priority = task.get('priority', 'medium')
        children = task.get('children', [])
        completed = task.get('completed', False)
        status = task.get('status', 'not_started')

        # Convert numbered lists in description and context
        description_html = self.convert_numbered_lists(description)
        context_html = self.convert_numbered_lists(context)

        # Determine if task has children
        has_children = len(children) > 0
        icon_class = "expand-icon" if has_children else "expand-icon no-children"
        icon_symbol = "▶"  # if has_children else "•"

        # Priority styling
        priority_class = f"priority-{priority.lower()}"

        # Phase styling
        header_class = "task-header phase" if is_phase else "task-header"
        if completed or status == 'completed':
            header_class += " completed"
            completion_icon = "✅"
        elif status == 'in_progress':
            header_class += " in-progress"
            completion_icon = "⏳"
        else:  # not_started
            completion_icon = "🔘"

        # Build task HTML
        html = f'''
        <li class="task-item">
            <div class="{header_class}" onclick="toggleTask(this)">
            <span class="completion-status">{completion_icon}</span>
                <span class="{icon_class}">{icon_symbol}</span>
                <span class="task-title">{title}</span>
                <span class="task-priority {priority_class}">{priority.title()}</span>
            </div>
            <div class="task-details">
                <div class="task-description">{description_html}</div>
                <div class="task-context">{context_html}</div>'''

        # Add children if they exist
        if has_children:
            html += '<ul class="child-tasks">'
            for child in children:
                html += self.generate_task_html(child, is_phase=False)
            html += '</ul>'

        html += '''
            </div>
        </li>'''

        return html

    @staticmethod
    def calculate_stats(tasks: Dict[str, Dict]) -> Dict[str, Any]:
        """Calculate project statistics"""
        total_tasks = len(tasks)
        phases = sum(1 for task in tasks.values() if not task.get('parent_id'))
        completed_tasks = sum(
            1 for task in tasks.values() if task.get('completed', False) or task.get('status') == 'completed')
        in_progress_tasks = sum(1 for task in tasks.values() if task.get('status') == 'in_progress')
        priorities = {'high': 0, 'medium': 0, 'low': 0}

        for task in tasks.values():
            priority = task.get('priority', 'medium').lower()
            if priority in priorities:
                priorities[priority] += 1

        completion_percentage = round((completed_tasks / total_tasks * 100)) if total_tasks > 0 else 0

        return {
            'total_tasks': total_tasks,
            'phases': phases,
            'completed_tasks': completed_tasks,
            'in_progress_tasks': in_progress_tasks,
            'completion_percentage': completion_percentage,
            'high_priority': priorities['high'],
            'medium_priority': priorities['medium'],
            'low_priority': priorities['low']
        }

    @staticmethod
    def generate_stats_html(stats: Dict[str, Any]) -> str:
        """Generate statistics HTML"""
        return f'''
        <div class="stat-card">
            <div class="stat-number">{stats['phases']}</div>
            <div class="stat-label">Phases</div>
        </div>
        <div class="stat-card">
            <div class="stat-number">{stats['total_tasks']}</div>
            <div class="stat-label">Total Tasks</div>
        </div>
        <div class="stat-card">
            <div class="stat-number">{stats['completed_tasks']}</div>
            <div class="stat-label">Completed</div>
        </div>
        <div class="stat-card">
            <div class="stat-number">{stats['in_progress_tasks']}</div>
            <div class="stat-label">In Progress</div>
        </div>
        <div class="stat-card">
            <div class="stat-number">{stats['completion_percentage']}%</div>
            <div class="stat-label">Progress</div>
        </div>'''

    @staticmethod
    def generate_lessons_learned_html(plan_data: Dict) -> str:
        """Generate lessons learned HTML section"""
        lessons_learned = plan_data.get('lessons_learned', [])

        if not lessons_learned:
            return ""

        html = '''
        <div class="lessons-learned">
            <h3>📚 Lessons Learned</h3>'''

        for lesson in lessons_learned:
            # Handle both string lessons and dict lessons with text field
            lesson_text = lesson if isinstance(lesson, str) else lesson.get('lesson', str(lesson))
            html += f'<div class="lesson-item">{lesson_text}</div>'

        html += '</div>'
        return html

    @staticmethod
    def generate_meta_html(plan_data: Dict) -> str:
        """Generate project metadata HTML"""
        created = plan_data.get('created_at', '').split('T')[0] if plan_data.get('created_at') else 'Unknown'
        updated = plan_data.get('updated_at', '').split('T')[0] if plan_data.get('updated_at') else 'Unknown'

        meta_items = [
            f"<strong>Created:</strong> {created}",
            f"<strong>Updated:</strong> {updated}"
        ]

        return '\n'.join([f'<span class="meta-item">{item}</span>' for item in meta_items])

    def convert_plan_to_html(self, plan: PlanModel) -> str:
        """Convert workspace planning YAML content to HTML"""
        plan_data = plan.model_dump()
        tasks = plan_data.get('tasks', {})
        if not tasks:
            raise ValueError("No tasks found in plan")

        # Build task hierarchy
        task_hierarchy = self.get_task_hierarchy(tasks)

        # Calculate stats
        stats = self.calculate_stats(tasks)

        # Generate HTML components
        title = plan_data.get('title', 'Project Plan')
        description = plan_data.get('description', 'Project description')
        meta_html = self.generate_meta_html(plan_data)
        stats_html = self.generate_stats_html(stats)
        lessons_learned_html = self.generate_lessons_learned_html(plan_data)

        # Generate task tree HTML
        task_tree_html = ""
        for task in task_hierarchy:
            # Top-level tasks are considered phases
            task_tree_html += self.generate_task_html(task, is_phase=True)

        # Fill template
        html_content = self.html_template.format(
            title=title,
            description=description,
            meta_items=meta_html,
            stats=stats_html,
            lessons_learned=lessons_learned_html,
            task_tree=task_tree_html
        )

        return html_content