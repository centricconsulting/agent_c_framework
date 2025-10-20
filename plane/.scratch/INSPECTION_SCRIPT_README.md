# PLANE API Issue Response Inspection Script

## Purpose

This script investigates the actual PLANE API response to find where sequence IDs (like "AC-9") are stored in the API data.

## What It Does

1. **Initializes PlaneClient** - Uses the working cookies from `~/.plane/cookies/agent_c.enc`

2. **Fetches Issues** - Gets all issues from project `dad9fe27-de38-4dd6-865f-0455e426339a`

3. **Checks Response Structure** - Determines if the response is:
   - A simple list: `[{issue1}, {issue2}]`
   - Or a dict with results: `{"results": [{issue1}, {issue2}]}`

4. **Prints Complete First Issue** - Shows ALL fields in the raw JSON response

5. **Searches for Sequence ID Fields** - Looks for these potential field names:
   - `sequence_id` ⭐ (most likely based on PlaneClient code)
   - `sequence`
   - `number`
   - `display_id`
   - `identifier`
   - `project_identifier`
   - `issue_identifier`
   - `issue_number`
   - `project_id`
   - `project`

6. **Searches for "AC" String** - Recursively searches through all fields for any occurrence of "AC"

7. **Lists All Top-Level Fields** - Shows every field name and type in the issue object

## How to Run

```bash
cd //plane/.scratch
python inspect_issue_response.py
```

## Expected Findings

Based on the `PlaneClient.get_issue_by_identifier()` method, we expect to find:
- **`sequence_id`** field containing the numeric part (e.g., `9` for "AC-9")
- The project identifier "AC" likely comes from the project configuration or project data

## What to Look For

1. **Is there a `sequence_id` field?** This is used in `get_issue_by_identifier()`
2. **Where is the project code "AC"?** 
   - In the issue itself?
   - In the project data?
   - Constructed from workspace/project name?
3. **Is the full identifier "AC-9" stored anywhere?**
4. **What other interesting fields exist?**

## Next Steps

After running this script, we'll know:
- ✅ The exact field name for the sequence number
- ✅ Where the project identifier comes from  
- ✅ The complete response structure
- ✅ How to reliably extract sequence IDs

This information will help us fix the identifier lookup in the PLANE tools.
