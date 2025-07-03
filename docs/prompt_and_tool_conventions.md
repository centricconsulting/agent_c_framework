
- Toolsets MUST have a name that matches their folder when converted to title case with "Tools" at their end so that the automatic discovery can work to import them:
  - The toolset in `src/tools/zoominfo/tool.py` MUST be named `ZomminfoTools`, 
    - Using `ZoomInfoTools` will not work as it does not match the folder
    - Using `ZoomInfoTool` or any other suffix aside from "Tools" also will not work
- Tool prompt sections SHOULD be named after their tool with the suffix "Section" i.e. "ZoomInfoToolsSection"
  - If you do NOT follow this convention, you MUST populate `tool_class_name` with the class name of your tool
- Prompt referenced by jinja by their `template_key` field, this is auto populated by can be overridden
  - For prompt sections created in code, the default key is the snake class name of the section minus "Section" so `MyClassSection` becomes `my_class`
  - For prompt sections loaded from disk the filename relative to the root folder of templates is used, so `agent_config/sections/boilerplate/standards/general.yaml` is registered as `boilerplate/standards/general`
  - For tool sections crated in code `tools/` is automatically added to the key.
    - for Tool sections loaded from disk the normal section rules apply, which is why they're auto prefixed. 
  - Context models for prompt sections are registered as `[section_type]_section_context` 
  - Agent instruction templates are provided to Jinja with the preface `agents/`
    - `agents/agent_key` - Loads the template from `agent_instructions` in the `AgentConfiguration` model.
    - `agents/agent_ket_clone` - Loads from `clone_instructions`

I've got an idea, that's not quite fully baked...  I have a Jinja based template system for rendering system prompts for agents. I call these "Prompt Sections" as their model contains additional information, like the ability to define state machines.  These machines are often used to control what parts of the system prompt get rendered so we can hide token dense regions when they're not needed.  This so very common I'm thinking I might be able to add some sort of template magic where I could let them declare a "toggle block" something like `toggle_block('some_name', 'open')`.  This would add a state machine named `toggle_some_name` with the initial state of "open" on first use, then sue it's state to control the visiblity of the block it wraps...  Seem sound?
