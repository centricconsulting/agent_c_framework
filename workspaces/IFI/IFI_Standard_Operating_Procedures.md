# IFI Standard Operating Procedures (SOP)
## Mandatory UI/UX Analysis Requirements - Lessons Learned Protocol

**Version**: 2.0  
**Effective Date**: 2024-12-19  
**Status**: MANDATORY - All IFI LOB Extractions  
**Supersedes**: Version 1.0 (pre-UI requirements)

---

## 🚨 CRITICAL UPDATE - UI/UX ANALYSIS NOW MANDATORY

**Background**: During WCP and BOP testing, UI specifications (character limits, error messages, visual indicators, auto-display behaviors) were initially missed, requiring 95K tokens of rework across both LOBs. This SOP update makes UI/UX analysis MANDATORY for ALL future LOB extractions.

---

## PHASE 1: REX ANALYSIS - MANDATORY UI/UX EXTRACTION

### Required Analysis Steps (EVERY LOB)

**1. Business Logic & Data Patterns** (Standard - No change)
- Extract business rules from code
- Document data flow patterns
- Map call graphs

**2. UI/UX ANALYSIS** ⭐ **MANDATORY - NEW**
- ✅ Review ALL .ascx markup files for UI controls
- ✅ Review ALL JavaScript functions for validation logic
- ✅ Document character limits (EXACT numbers)
- ✅ Document error messages (EXACT text)
- ✅ Document visual indicators (colors, borders, icons)
- ✅ Document auto-display/hide behaviors
- ✅ Identify shared UI controls across LOBs

**3. Completeness Report** (Enhanced)
- Business logic coverage: X%
- **UI/UX specifications coverage: X%** ⭐ **NEW**
- Gaps identified
- External dependencies

### Rex Deliverables Checklist

```yaml
Before marking Rex analysis complete, verify:
✓ Business logic patterns extracted
✓ Call graphs documented
✓ UI CONTROLS ANALYZED ⭐ NEW
✓ JAVASCRIPT VALIDATION DOCUMENTED ⭐ NEW
✓ CHARACTER LIMITS IDENTIFIED ⭐ NEW
✓ ERROR MESSAGES EXTRACTED ⭐ NEW
✓ VISUAL INDICATORS DOCUMENTED ⭐ NEW
✓ AUTO-DISPLAY BEHAVIORS DOCUMENTED ⭐ NEW
✓ Completeness report generated
✓ Metadata structure populated
✓ Compressed handoff prepared

IF ANY CHECKBOX UNCHECKED: Analysis INCOMPLETE
```

### Rex Metadata Structure (Enhanced)

```
//IFI/meta/code_analysis/{feature_name}/
├── business_logic/
├── call_graphs/
├── ui_specifications/ ⭐ NEW
│   ├── ui_controls.json
│   ├── javascript_functions.json
│   ├── character_limits.json
│   ├── error_messages.json
│   ├── auto_display_behaviors.json
│   └── shared_controls.json
└── completeness_report.md
```

---

## PHASE 2: DOUGLAS GAP COORDINATION (No change to process)

Douglas coordinates gap resolution for any identified external dependencies or missing content.

---

## PHASE 3: MASON REQUIREMENTS DOCUMENTATION - MANDATORY UI/UX SECTIONS

### Required Document Sections (EVERY LOB)

**MANDATORY Sections** (cannot be omitted):

1. Executive Summary
2. Business Overview
3. Detailed Feature Specifications
4. **UI/UX Requirements** ⭐ **MANDATORY - NEW**
5. **Validation Rules and Business Logic** ⭐ **MANDATORY - NEW**
6. User Stories with Acceptance Criteria
7. Testing Requirements
8. Source Attribution

### Section 4: UI/UX Requirements (Template)

**Required Subsections:**
- Auto-display/hide behaviors
- Text input specifications (with character limits)
- Validation visual indicators (with colors/borders)
- Error messages (with exact text)
- Interactive elements
- Accessibility requirements
- Responsive design requirements

**Example Content:**
```markdown
## 4. UI/UX Requirements

### 4.1 Auto-Display/Hide Behaviors
When user selects "Yes" for any eligibility question, an "Additional
Information" text box automatically displays below the question...

### 4.2 Text Input Specifications
- Character Limit: 125 characters (BOP) or 2000 characters (WCP)
- Real-time character counter displayed: "X/125 characters"
- Multi-line text area (3 rows, expandable)

### 4.3 Validation Visual Indicators
- Red border (#FF0000) when field empty and required
- Error message in red text: "Additional Information Response Required"
- Character counter turns red when limit exceeded
```

### Section 5: Validation Rules (Template)

**Required Subsections:**
- Client-side validation (JavaScript)
- Character limit validation
- Required field validation
- Business rule validation
- Server-side validation

### Mason Deliverables Checklist

```yaml
Before marking Mason documentation complete, verify:
✓ Executive Summary exists
✓ Business Overview exists
✓ Detailed Specifications exist
✓ UI/UX REQUIREMENTS SECTION EXISTS ⭐ NEW
✓ VALIDATION RULES SECTION EXISTS ⭐ NEW
✓ User Stories exist
✓ Testing Requirements exist
✓ Source Attribution exists
✓ Character limits MATCH Rex's analysis ⭐ NEW
✓ Error messages MATCH Rex's extraction ⭐ NEW

IF ANY CHECKBOX UNCHECKED: Documentation INCOMPLETE
```

---

## PHASE 4-5: ARIA & RITA (No change to process)

Aria creates architecture design, Rita validates insurance domain accuracy.

---

## PHASE 6: VERA QUALITY VALIDATION - MANDATORY UI/UX VALIDATION

### UI/UX Validation Checklist (EVERY LOB)

```yaml
Vera MUST validate:
✓ UI/UX Requirements section exists
✓ UI/UX section is comprehensive
✓ Character limits specified (EXACT numbers)
✓ Error messages documented (EXACT text)
✓ Visual indicators documented (colors, borders)
✓ Auto-display behaviors documented
✓ Accessibility requirements documented
✓ Validation Rules section exists
✓ Validation Rules section is comprehensive
✓ Character limits MATCH Rex's analysis
✓ Error messages MATCH Rex's extraction
✓ JavaScript functions documented

IF ANY CHECKBOX UNCHECKED: QUALITY FAILURE
```

### Quality Failure Protocol

```yaml
IF UI/UX section missing:
  STATUS: CRITICAL QUALITY FAILURE
  ACTION: REJECT requirements document
  MESSAGE: "UI/UX Requirements section missing. MANDATORY per
           lessons learned. Mason must add before approval."

IF UI/UX section incomplete:
  STATUS: MAJOR QUALITY FAILURE
  ACTION: Require Mason remediation
  MESSAGE: "UI/UX Requirements incomplete. Missing {items}.
           Mason must complete before approval."

IF cross-references don't match:
  STATUS: MAJOR QUALITY FAILURE
  ACTION: Require Mason correction
  MESSAGE: "UI/UX specifications don't match Rex's analysis.
           Character limits/error messages must be corrected."
```

---

## PHASE 7: DOUGLAS FINAL ORCHESTRATION

### Quality Gate Enforcement

Douglas MUST enforce UI/UX quality gates:

**Phase 1 Gate (Rex):**
- Verify UI specifications metadata exists
- Reject if UI analysis incomplete

**Phase 3 Gate (Mason):**
- Verify UI/UX Requirements section exists
- Reject if UI/UX sections missing

**Phase 6 Gate (Vera):**
- Verify Vera validated UI specifications
- Reject if UI validation not performed

---

## TOKEN EFFICIENCY TARGETS (Updated)

```yaml
Per Feature Token Budget (includes UI analysis):

Rex: 200K tokens
  - Business logic: 120K
  - UI/UX analysis: 50K ⭐ NEW
  - Call graphs: 20K
  - Synthesis: 10K

Mason: 150K tokens (was 125K)
  - Requirements: 80K
  - UI/UX section: 30K ⭐ NEW
  - Validation section: 20K ⭐ NEW
  - User stories: 20K

Vera: 100K tokens (was 95K)
  - Requirements validation: 50K
  - UI/UX validation: 20K ⭐ NEW
  - Traceability: 20K
  - Quality report: 10K

Total: 674K tokens (includes UI analysis overhead)
vs Baseline: 910K tokens
Improvement: 26% reduction maintained
```

---

## SUCCESS METRICS

**For EVERY LOB extraction, measure:**

1. **UI Specification Completeness**: 100% target
   - All character limits documented?
   - All error messages documented?
   - All visual indicators documented?

2. **First-Pass Completion**: 100% target
   - UI specifications complete without user intervention?
   - Zero rework required for UI specifications?

3. **Quality Gate Effectiveness**: 100% target
   - Rex delivered complete UI analysis?
   - Mason included mandatory UI sections?
   - Vera validated UI specifications?

4. **Token Efficiency**: ≤674K target
   - Stayed within enhanced token budgets?
   - No token waste on UI rework?

---

## ENFORCEMENT & ACCOUNTABILITY

**Agent Responsibilities:**

- **Rex**: MUST analyze UI/UX or analysis INCOMPLETE
- **Mason**: MUST include UI/UX sections or documentation INCOMPLETE
- **Vera**: MUST validate UI/UX or validation INCOMPLETE
- **Douglas**: MUST enforce quality gates or orchestration FAILED

**Escalation:**
- Any agent skipping UI analysis/documentation → Immediate rejection
- Any quality gate not enforced → Escalate to IFI Technical Authority
- Pattern of non-compliance → Agent persona review required

---

## LESSONS LEARNED SUMMARY

### What Went Wrong (WCP & BOP)
- Rex focused on business logic, skipped UI analysis
- Mason created requirements without UI/UX sections
- Vera didn't validate UI specifications
- Douglas didn't enforce UI quality gates
- Result: 95K tokens wasted on rework

### What's Fixed Now
- ✅ Rex persona updated with mandatory UI analysis
- ✅ Mason persona updated with mandatory UI/UX sections
- ✅ Vera persona updated with mandatory UI validation
- ✅ Douglas persona updated with quality gate enforcement
- ✅ This SOP documents mandatory procedures
- ✅ Requirements template includes UI/UX sections

### Expected Outcome
- ✅ Next LOB (CGL, CAP, CPR): Complete UI specifications on first pass
- ✅ Zero user intervention for UI specifications
- ✅ Zero token waste on UI rework
- ✅ 100% first-pass completeness

---

## DOCUMENT CONTROL

**Version History:**
- v1.0 (2024-11-01): Initial SOP without UI requirements
- v2.0 (2024-12-19): Added mandatory UI/UX analysis requirements

**Review Cycle**: Quarterly or after any major process failure

**Approval**: IFI Technical Authority

**Distribution**: All IFI analysis team agents (Rex, Mason, Aria, Rita, Vera, Douglas)

**Compliance**: MANDATORY - No exceptions

---

**END OF STANDARD OPERATING PROCEDURES**
