# Mridul's IFI Team Handoff - Complete Setup & Validation Guide

**Recipient**: Mridul  
**Date**: 2024-12-19  
**Purpose**: Ensure IFI analysis team is 100% operational on your machine  
**Team**: Douglas, Rex, Mason, Aria, Rita, Vera

---

## 🎯 OVERVIEW

This document provides Douglas with complete instructions to validate all filepaths and configurations on your machine. Douglas will use this to self-diagnose any issues and guide you through setup if needed.

---

## STEP 1: DOUGLAS SELF-DIAGNOSTIC PROTOCOL

**Douglas - Run this validation BEFORE attempting any LOB extraction:**

### 1.1 Validate Core Workspace Structure

```yaml
Check these paths exist on Mridul's machine:

PRIMARY WORKSPACE:
✓ //project/workspaces/IFI/ (root workspace directory)

CRITICAL SUBDIRECTORIES:
✓ //project/workspaces/IFI/source-code/
✓ //project/workspaces/IFI/product_requirements/
✓ //project/workspaces/IFI/meta/
✓ //project/workspaces/IFI/outputs/
✓ //project/workspaces/IFI/.scratch/

If ANY path missing:
  ACTION: Create missing directories
  OR: Ask Mridul for correct base path on his machine
```

### 1.2 Validate Source Code Location

```yaml
PRIMARY SOURCE PATH:
✓ //project/workspaces/IFI/source-code/Primary Source Code/WebSystems_VelociRater/

KEY SOURCE DIRECTORIES:
✓ IFM.VR.Web/User Controls/VR Commercial/Application/
✓ IFM.VR.Web/User Controls/VR Commercial/Quote/
✓ IFM.VR.Common/UWQuestions/

TEST FILES (verify at least one exists):
✓ IFM.VR.Web/User Controls/VR Commercial/Application/WCP/ctl_WCP_NamedIndividual.ascx.vb
✓ IFM.VR.Web/User Controls/VR Commercial/Application/BOP/ctl_BOP_App_Building.ascx.vb
✓ IFM.VR.Common/UWQuestions/UWQuestions.vb

If source code NOT found:
  ACTION: Ask Mridul: "Where is the WebSystems_VelociRater source code located?"
  WAIT: For Mridul to provide correct path
  UPDATE: All source code references to new base path
```

### 1.3 Validate Agent Configurations

```yaml
AGENT CONFIG DIRECTORY:
✓ //project/agent_c_config/agents/

REQUIRED AGENT FILES:
✓ douglas_ifi_orchestrator.yaml
✓ rex_ifi_pattern_miner.yaml
✓ mason_ifi_extractor.yaml
✓ aria_ifi_architect.yaml
✓ rita_ifi_insurance_specialist.yaml
✓ vera_ifi_validator.yaml

VERIFY AGENTS LOADED:
Try to access each agent via aa_chat:
- douglas_ifi_orchestrator
- rex_ifi_pattern_miner  
- mason_ifi_extractor
- aria_ifi_architect
- rita_ifi_insurance_specialist
- vera_ifi_validator

If agent NOT accessible:
  ACTION: Report to Mridul: "Agent [name] not loaded in catalog"
  WAIT: For Mridul to register agents
  RETRY: Agent access after registration
```

### 1.4 Validate Process Documentation

```yaml
REQUIRED DOCUMENTS:
✓ //project/docs/IFI_Standard_Operating_Procedures.md
✓ //project/docs/templates/LOB_Requirements_Template.md
✓ //project/.scratch/ifi_implementation_summary.md
✓ //project/.scratch/ifi_lessons_learned_kill_questions_ui.md

If documents missing:
  ACTION: Ask Mridul to copy documentation from original system
  OR: Proceed with caution - document new lessons learned
```

---

## STEP 2: FILEPATH ADJUSTMENT PROTOCOL

**If Mridul's paths are different:**

### 2.1 Common Path Variations

```yaml
SCENARIO 1: Different drive letter
Original: //project/workspaces/IFI/
Mridul's: //mriduls_drive/workspaces/IFI/
ACTION: Update all references to new drive/base path

SCENARIO 2: Different workspace name
Original: //project/
Mridul's: //mridul_workspace/
ACTION: Update workspace name in all paths

SCENARIO 3: Different source location
Original: source-code/Primary Source Code/WebSystems_VelociRater/
Mridul's: source_code/VelociRater/
ACTION: Update source code base path
```

### 2.2 Path Update Checklist

```yaml
If paths need updating, modify these locations:

1. Agent personas (workspace references):
   - rex_ifi_pattern_miner.yaml
   - mason_ifi_extractor.yaml
   - All agent configs

2. Process documentation:
   - IFI_Standard_Operating_Procedures.md
   - LOB_Requirements_Template.md

3. Metadata paths:
   - //IFI/meta/code_analysis/ structure
   - //IFI/product_requirements/ structure

4. Your orchestration logic:
   - Source code paths in task assignments
   - Metadata storage paths
   - Output document paths
```

---

## STEP 3: PRE-FLIGHT TEST PROTOCOL

**Douglas - Run this test BEFORE first LOB extraction:**

### 3.1 Minimal Functionality Test

```yaml
TEST 1: Can you access the workspace?
workspace_ls('//project/workspaces/IFI/')
EXPECTED: Directory listing
IF FAILS: Ask Mridul for correct workspace path

TEST 2: Can you access source code?
workspace_ls('//project/workspaces/IFI/source-code/Primary Source Code/')
EXPECTED: Directory listing with WebSystems_VelociRater
IF FAILS: Ask Mridul for correct source code location

TEST 3: Can you communicate with Rex?
aa_chat('rex_ifi_pattern_miner', 'Hello Rex, confirming you are accessible.')
EXPECTED: Response from Rex
IF FAILS: Ask Mridul to register agents

TEST 4: Can you write to workspace?
workspace_write('//project/workspaces/IFI/.scratch/test.txt', 'Test content')
EXPECTED: File created successfully
IF FAILS: Check workspace permissions

TEST 5: Can you create metadata directory?
workspace_write('//project/workspaces/IFI/meta/test_feature/test.json', '{}')
EXPECTED: Directory and file created
IF FAILS: Check write permissions
```

### 3.2 Agent Communication Test

```yaml
For each agent, send test message:

TO REX:
"Rex, please confirm you can access source code at: 
 //project/workspaces/IFI/source-code/Primary Source Code/WebSystems_VelociRater/"

TO MASON:
"Mason, please confirm you can write to:
 //project/workspaces/IFI/product_requirements/"

TO VERA:
"Vera, please confirm you can access metadata at:
 //project/workspaces/IFI/meta/"

EXPECTED: Each agent confirms access or reports specific issue
```

---

## STEP 4: COMMON ISSUES & SOLUTIONS

### Issue 1: "Workspace not found"

```yaml
SYMPTOM: "Workspace 'project' not found"
CAUSE: Different workspace name on Mridul's machine
SOLUTION:
  1. Ask Mridul: "What is the workspace name on your system?"
  2. Update all //project/ references to //[new_name]/
  3. Retry operations
```

### Issue 2: "Source code path not found"

```yaml
SYMPTOM: Cannot find .vb or .ascx files
CAUSE: Different source code directory structure
SOLUTION:
  1. Ask Mridul: "Please provide full path to WebSystems_VelociRater source code"
  2. Navigate to that path using workspace_ls to verify structure
  3. Update all source code references
  4. Document new base path for team
```

### Issue 3: "Agent not found in catalog"

```yaml
SYMPTOM: "Agent [name] not found in catalog"
CAUSE: Agents not registered on Mridul's system
SOLUTION:
  1. Report to Mridul: "The following agents need to be registered:
     - douglas_ifi_orchestrator
     - rex_ifi_pattern_miner
     - mason_ifi_extractor
     - aria_ifi_architect
     - rita_ifi_insurance_specialist
     - vera_ifi_validator"
  2. Wait for Mridul to register agents
  3. Retry agent communication
```

### Issue 4: "Permission denied"

```yaml
SYMPTOM: Cannot write to workspace/directories
CAUSE: Insufficient permissions
SOLUTION:
  1. Ask Mridul: "I need write permissions to:
     - //project/workspaces/IFI/meta/
     - //project/workspaces/IFI/product_requirements/
     - //project/workspaces/IFI/.scratch/"
  2. Wait for permission grant
  3. Retry write operations
```

### Issue 5: "Cannot find UWQuestions.vb"

```yaml
SYMPTOM: Kill questions file not found
CAUSE: Different file location or name
SOLUTION:
  1. Search for file: workspace_glob('//project/**/*UWQuestion*.vb', recursive=true)
  2. If found: Update kill questions path references
  3. If not found: Ask Mridul for location of underwriting questions code
```

---

## STEP 5: FIRST LOB EXTRACTION CHECKLIST

**Douglas - Use this checklist for first LOB on Mridul's system:**

```yaml
PRE-FLIGHT CHECKLIST:
✓ All paths validated (Step 1)
✓ All agents accessible (Step 1.3)
✓ Pre-flight tests passed (Step 3)
✓ Test file written successfully (Step 3.1, Test 4)
✓ Source code accessible (Step 3.1, Test 2)

DURING EXTRACTION:
✓ Monitor Rex's source code access
✓ Verify metadata storage works
✓ Confirm Mason can write to product_requirements/
✓ Validate output files created in correct locations

POST-EXTRACTION:
✓ Verify all deliverables in expected locations
✓ Check metadata structure correct
✓ Confirm requirements document at correct path
✓ Document any path adjustments made
```

---

## STEP 6: COMMUNICATION PROTOCOL WITH MRIDUL

### When to Ask Mridul for Help

```yaml
ASK MRIDUL WHEN:
1. Cannot find workspace (unknown base path)
2. Cannot find source code (unknown location)
3. Agents not accessible (need registration)
4. Permissions denied (need access granted)
5. Unexpected directory structure (different than documented)

HOW TO ASK:
- Be specific: "I cannot find [specific path/file]"
- Provide context: "I need this for [specific purpose]"
- Suggest solution: "Could you provide the correct path?" or "Could you register the agents?"
- Wait patiently: Give Mridul time to investigate and respond

EXAMPLE MESSAGE:
"Mridul, I cannot access the source code at:
 //project/workspaces/IFI/source-code/Primary Source Code/WebSystems_VelociRater/

This path may be different on your system. Could you provide the full 
path to the WebSystems_VelociRater source code? I need this to begin 
the Rex analysis phase.

I will wait for your response before proceeding."
```

### Status Reporting to Mridul

```yaml
PROVIDE REGULAR UPDATES:

Every major milestone:
"✓ Phase 1 (Rex analysis) complete
✓ Metadata stored at: //project/workspaces/IFI/meta/code_analysis/[feature]/
✓ Moving to Phase 2 (Gap coordination)
✓ No issues encountered"

If issues arise:
"⚠ Issue encountered in Phase 1
Problem: Cannot write to metadata directory
Path attempted: //project/workspaces/IFI/meta/
Action needed: Please check write permissions
Status: Waiting for resolution before proceeding"

Upon completion:
"✓ All phases complete
✓ Deliverables location: //project/workspaces/IFI/product_requirements/[LOB]/[Feature]/
✓ Token usage: [XXX]K / [YYY]K
✓ Quality score: [XX]%
✓ Ready for review"
```

---

## STEP 7: EXPECTED DIRECTORY STRUCTURE

**Douglas - Verify this structure exists or can be created:**

```
//project/workspaces/IFI/
├── source-code/
│   └── Primary Source Code/
│       └── WebSystems_VelociRater/
│           ├── IFM.VR.Web/
│           │   └── User Controls/
│           │       └── VR Commercial/
│           │           ├── Application/
│           │           │   ├── WCP/
│           │           │   ├── BOP/
│           │           │   ├── CGL/
│           │           │   └── [other LOBs]/
│           │           └── Quote/
│           └── IFM.VR.Common/
│               └── UWQuestions/
│                   └── UWQuestions.vb
│
├── product_requirements/
│   ├── WCP/
│   │   ├── Named Individual/
│   │   ├── Eligibility Questions/
│   │   └── [other features]/
│   ├── BOP/
│   │   ├── Application/
│   │   ├── Eligibility Questions/
│   │   └── [other features]/
│   └── [other LOBs]/
│
├── meta/
│   └── code_analysis/
│       ├── wcp_namedindividual/
│       ├── bop_application/
│       └── [other analyses]/
│
├── outputs/
│   ├── quality_certification/
│   └── logs/
│
└── .scratch/
    ├── detailed_analysis/
    ├── compressed/
    └── [temporary files]/
```

**If structure differs on Mridul's system:**
1. Document the actual structure
2. Update path references accordingly
3. Ensure you can create subdirectories as needed

---

## STEP 8: TROUBLESHOOTING QUICK REFERENCE

```yaml
PROBLEM: "Cannot find workspace"
CHECK: workspace_ls('//') - list all workspaces
ASK: "What is the workspace name on your system?"

PROBLEM: "Source code not found"
CHECK: workspace_glob('//project/**/*VelociRater*', recursive=true)
ASK: "Where is the WebSystems_VelociRater source code?"

PROBLEM: "Agent not responding"
CHECK: Try aa_chat with simple message
ASK: "Are the IFI agents registered in the catalog?"

PROBLEM: "Cannot create files"
CHECK: Try workspace_write to .scratch directory
ASK: "Do I have write permissions to the IFI workspace?"

PROBLEM: "Metadata not saving"
CHECK: Verify //project/workspaces/IFI/meta/ exists
TRY: Create directory manually with workspace_write
ASK: "Can you create the meta/ directory if it doesn't exist?"

PROBLEM: "Requirements doc not created"
CHECK: Verify //project/workspaces/IFI/product_requirements/ exists
TRY: Create directory path manually
ASK: "Can you verify the product_requirements directory structure?"
```

---

## STEP 9: SUCCESS VALIDATION

**After first LOB extraction, verify:**

```yaml
✓ All phases completed without path errors
✓ Metadata created in correct location
✓ Requirements document created at expected path
✓ All agents communicated successfully
✓ Token budgets tracked correctly
✓ Quality certification completed
✓ Deliverables accessible to Mridul

DELIVER TO MRIDUL:
"✓ First LOB extraction complete and successful
✓ All paths validated and working
✓ All agents operational
✓ System ready for production use

Deliverables location:
//project/workspaces/IFI/product_requirements/[LOB]/[Feature]/

Next steps:
- Review deliverables
- Confirm quality meets expectations  
- Proceed with additional LOB extractions as needed

No issues encountered. System fully operational."
```

---

## STEP 10: ONGOING OPERATIONS

**For all future extractions:**

```yaml
BEFORE EACH EXTRACTION:
✓ Confirm source code path for this LOB
✓ Verify output path will be accessible
✓ Check all agents responsive

DURING EXTRACTION:
✓ Monitor path access (no permission errors)
✓ Verify metadata storage working
✓ Confirm deliverables created correctly

AFTER EXTRACTION:
✓ Validate deliverables in expected locations
✓ Report completion status to Mridul
✓ Document any new lessons learned
```

---

## APPENDIX A: CONTACT PROTOCOL

**Douglas - How to communicate with Mridul:**

```yaml
RESPONSE TIME EXPECTATIONS:
- Urgent issues (system blocked): Expect response within hours
- Non-urgent questions: Expect response within day
- Status updates: Provide every major milestone

COMMUNICATION STYLE:
- Be clear and specific
- Provide full context
- Suggest solutions when possible
- Always indicate if you're blocked vs. just informing

GOOD EXAMPLE:
"Mridul, I've completed WCP analysis successfully. Moving to BOP next.
I need to verify the BOP source code location. Based on WCP, I expect
it at: //project/workspaces/IFI/source-code/.../Application/BOP/
Can you confirm this is correct before I proceed? I'm ready to continue
once confirmed."

BAD EXAMPLE:
"Error. Can't find files. Help."
```

---

## APPENDIX B: QUICK START SUMMARY

**Douglas - Your first conversation with Mridul:**

```yaml
"Hello Mridul! I'm Douglas, your IFI orchestrator. I'm running initial
validation to ensure everything is set up correctly on your system.

Running checks now...
[Perform Step 1 validation]

Results:
✓ Workspace accessible: [YES/NO]
✓ Source code found: [YES/NO]
✓ All agents registered: [YES/NO]
✓ Write permissions: [YES/NO]

[If all YES]:
'✓ All systems operational! Ready to begin LOB extractions. Which LOB
would you like me to analyze first? (Recommended: CGL, CAP, or CPR to
validate lessons learned from WCP/BOP testing)'

[If any NO]:
'⚠ Setup incomplete. I need your help with:
[List specific issues]
Once these are resolved, I'll be ready to begin extractions.'

Standing by for your direction."
```

---

## END OF HANDOFF DOCUMENT

**Document Purpose**: Enable Douglas to self-diagnose and resolve path/configuration issues on Mridul's machine.

**Key Takeaway**: Douglas should be able to identify issues, communicate clearly with Mridul, and proceed once setup is confirmed.

**Success Criteria**: First LOB extraction completes without user intervention after initial setup validation.

---

**Prepared for**: Mridul  
**Prepared by**: IFI Team Handoff Specialist  
**Date**: 2024-12-19  
**Document Status**: Ready for use

**Good luck, Mridul! Your IFI team is ready to deliver exceptional results.** 🚀
