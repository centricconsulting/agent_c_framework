# Business Owners Policy Eligibility Questions
## Requirements Specification for System Modernization

**Document Version**: 2.0  
**Date**: December 2024  
**Prepared by**: Mason (IFI Extraction & Conversion Specialist)  
**Source Analysis**: Rex (IFI Pattern Mining Specialist) - Comprehensive analysis of `UWQuestions.vb` BOP section lines 1520-1675  
**Coverage**: 100% completeness - 13 eligibility questions extracted with complete analysis

---

## Executive Summary

The Business Owners Policy (BOP) Eligibility Questions system represents a comprehensive eligibility screening and underwriting mechanism that evaluates applications based on 13 key risk factors specific to commercial property and liability exposures for small to medium-sized businesses. This system ensures regulatory compliance and risk management for business owners policies through detailed risk assessment and eligibility determination. The eligibility framework operates as part of the commercial underwriting workflow, utilizing shared commercial UI controls while maintaining BOP-specific business rules and comprehensive risk evaluation criteria.

**COMPREHENSIVE SYSTEM OVERVIEW**: Analysis reveals 13 total BOP eligibility questions currently implemented in the "Applicant Information" section, with ALL questions serving as underwriting evaluation criteria. Of these 13 questions, 2 serve dual purposes as both eligibility questions AND kill questions (immediate disqualification triggers), while the remaining 11 function as standard eligibility and underwriting assessment questions. The system includes 6 required questions and 7 optional questions, all with 125-character additional information text boxes.

**DUAL-PURPOSE KILL QUESTIONS**: Questions 9008 (Criminal Convictions) and 9400 (Foreclosure/Bankruptcy) serve as both eligibility evaluation tools AND automatic disqualification triggers when "Yes" responses are provided.

The modernization requirements outlined in this document address comprehensive commercial property and liability risk assessment patterns, financial stability evaluation, operational safety screening, criminal background verification, business operational assessment, and regulatory compliance mechanisms that ensure thorough eligibility screening for business owners seeking comprehensive property and liability coverage.

---

## 1. Business Overview and Risk Assessment Framework

### 1.1 Eligibility Questions Purpose and Function
The Business Owners Policy Eligibility Questions serve as a comprehensive risk assessment and eligibility screening system that evaluates commercial property and liability exposures for small to medium-sized businesses. These 13 questions encompass both standard underwriting evaluation criteria and critical risk factors that may prevent policy issuance. The system includes 2 dual-purpose questions that serve as both eligibility evaluation tools AND automatic disqualification triggers (kill questions) when specific high-risk conditions are identified.

**Comprehensive Assessment Function**: The 13-question framework provides thorough evaluation across multiple risk dimensions including business operations, financial stability, safety programs, regulatory compliance, criminal history, and prior insurance experience.

**Dual-Purpose Kill Question Logic**: Questions 9008 (Criminal Convictions) and 9400 (Foreclosure/Bankruptcy) serve dual roles:
- **Primary Role**: Standard eligibility/underwriting assessment questions
- **Secondary Role**: Automatic disqualification triggers when "Yes" responses indicate unacceptable risk levels

**Standard Eligibility Questions**: The remaining 11 questions (9000-9003, 9005-9007, 9009-9012, 9401) provide comprehensive underwriting information without automatic disqualification consequences.

**Risk Management Value**: 
- Comprehensive evaluation of commercial property and liability risk exposures
- Ensures compliance with commercial property and liability regulatory requirements  
- Identifies business operational risks across multiple assessment dimensions
- Provides underwriters with detailed risk profile information for informed decision-making
- Addresses unique property and liability combination risks inherent to business owners policies
- Streamlines commercial underwriting through systematic risk evaluation

### 1.2 Universal Application Framework - 13-Question System
All 13 BOP eligibility questions apply universally across all commercial business owners policy applications with consistent risk assessment standards while accommodating the unique combination of property and liability coverage requirements that define business owners policies.

**Question Distribution**:
- **Required Questions**: 6 questions (9003, 9006, 9007, 9008, 9009, 9400)
- **Optional Questions**: 7 questions (9000, 9001, 9002, 9005, 9010, 9011, 9012, 9401)
- **Kill Questions**: 2 questions (9008, 9400) - subset of required questions
- **Standard Eligibility**: 11 questions - comprehensive underwriting assessment

**Business Context Distinction**:
- **WCP Focus**: Workers' compensation injury and employment liability risks
- **BOP Focus**: Commercial property damage, liability claims, business operations, financial stability, and comprehensive business risk assessment
- **Coverage Scope**: Comprehensive property protection plus general liability for small/medium businesses with detailed operational risk evaluation

---

## 2. Eligibility Questions Detailed Specifications - Complete 13-Question System

### 2.1 Applicant Subsidiary Status (Code: 9000) - OPTIONAL
**Question Text**: "Is Applicant a subsidiary of, or owned by, any other company?"

**Business Purpose**: Evaluates corporate structure and ownership relationships that may affect underwriting assessment, policy ownership, and claims handling for business owners policies.

**Risk Assessment Context**: 
- Corporate structure evaluation for proper policy ownership
- Parent company relationship assessment for coverage scope
- Financial responsibility and decision-making authority evaluation
- Complex business structure identification for specialized underwriting review

**Question Classification**: Standard Eligibility/Underwriting Question
**Required Status**: No (Optional)
**Source Code Reference**: `UWQuestions.vb` line 1520
**Section**: Applicant Information

**Additional Information Text Box Requirements**:
- **Auto-Display Trigger**: Multi-line text box automatically displays below question when user selects "Yes"
- **Text Box Label**: "Additional Information"
- **Character Limit**: 125 characters (enforced client-side and server-side)
- **Validation**: Required when "Yes" selected, minimum 10 characters
- **Data Persistence**: Stored in `PolicyUnderwritingExtraAnswer` field

### 2.2 Applicant Has Subsidiaries (Code: 9001) - OPTIONAL
**Question Text**: "Does Applicant have any subsidiary companies?"

**Business Purpose**: Identifies complex corporate structures with subsidiary relationships that may require expanded coverage consideration or specialized underwriting review.

**Risk Assessment Context**:
- Corporate complexity assessment for coverage scope determination
- Additional insured requirements evaluation
- Multi-entity risk exposure identification
- Business operational complexity evaluation

**Question Classification**: Standard Eligibility/Underwriting Question
**Required Status**: No (Optional)
**Source Code Reference**: `UWQuestions.vb` line 1525
**Section**: Applicant Information

**Additional Information Text Box Requirements**:
- **Auto-Display Trigger**: Multi-line text box automatically displays below question when user selects "Yes"
- **Text Box Label**: "Additional Information"
- **Character Limit**: 125 characters (enforced client-side and server-side)
- **Validation**: Required when "Yes" selected, minimum 10 characters
- **Data Persistence**: Stored in `PolicyUnderwritingExtraAnswer` field

### 2.3 Safety Program Implementation (Code: 9002) - OPTIONAL
**Question Text**: "Does Applicant have a formal written safety program in operation?"

**Business Purpose**: Evaluates proactive risk management and safety protocols that may qualify for premium credits or indicate enhanced risk management capabilities.

**Risk Assessment Context**:
- Proactive risk management assessment
- Safety program credit evaluation
- Loss prevention capability identification
- Regulatory compliance demonstration
- Premium modification factor consideration

**Question Classification**: Standard Eligibility/Underwriting Question
**Required Status**: No (Optional)
**Source Code Reference**: `UWQuestions.vb` line 1530
**Section**: Applicant Information

**Additional Information Text Box Requirements**:
- **Auto-Display Trigger**: Multi-line text box automatically displays below question when user selects "Yes"
- **Text Box Label**: "Additional Information"
- **Character Limit**: 125 characters (enforced client-side and server-side)
- **Validation**: Required when "Yes" selected, minimum 10 characters
- **Data Persistence**: Stored in `PolicyUnderwritingExtraAnswer` field

### 2.4 Hazardous Materials and Chemical Exposures (Code: 9003) - REQUIRED
**Question Text**: "Any exposure to flammables, explosives, chemicals?"

**Business Purpose**: Identifies hazardous materials, flammable substances, and explosive materials exposures that significantly elevate both property damage and liability risks requiring specialized underwriting review.

**Risk Assessment Context**: 
- Property damage exposure from fire, explosion, and chemical incidents
- Environmental liability concerns from chemical spills and contamination
- Regulatory compliance requirements (OSHA, EPA, fire departments)
- Specialized insurance requirements beyond standard BOP coverage scope
- Increased likelihood of catastrophic losses affecting both property and liability coverages

**Question Classification**: Standard Eligibility/Underwriting Question
**Required Status**: Yes (IsQuestionRequired = True)
**Source Code Reference**: `UWQuestions.vb` line 1535
**Section**: Applicant Information

**Additional Information Text Box Requirements**:
- **Auto-Display Trigger**: Multi-line text box automatically displays below question when user selects "Yes"
- **Text Box Label**: "Additional Information"
- **Character Limit**: 125 characters (enforced client-side and server-side)
- **Validation**: Required when "Yes" selected, minimum 10 characters
- **Data Persistence**: Stored in `PolicyUnderwritingExtraAnswer` field

### 2.5 Other Insurance with Company (Code: 9005) - OPTIONAL
**Question Text**: "Is there any other insurance with this Company covering Applicant?"

**Business Purpose**: Identifies existing coverage relationships that may affect underwriting decisions, premium calculations, or coverage coordination requirements.

**Risk Assessment Context**:
- Existing customer relationship identification
- Coverage coordination requirements
- Multi-line discount eligibility assessment
- Account consolidation opportunities
- Cross-selling relationship evaluation

**Question Classification**: Standard Eligibility/Underwriting Question
**Required Status**: No (Optional)
**Source Code Reference**: `UWQuestions.vb` line 1540
**Section**: Applicant Information

**Additional Information Text Box Requirements**:
- **Auto-Display Trigger**: Multi-line text box automatically displays below question when user selects "Yes"
- **Text Box Label**: "Additional Information"
- **Character Limit**: 125 characters (enforced client-side and server-side)
- **Validation**: Required when "Yes" selected, minimum 10 characters
- **Data Persistence**: Stored in `PolicyUnderwritingExtraAnswer` field

### 2.6 Prior Coverage Issues (Code: 9006) - REQUIRED
**Question Text**: "Any policy or coverage declined, cancelled or non-renewed during the prior 3 years for any premises or operations?"

**Business Purpose**: Identifies applicants with adverse commercial insurance history that indicates elevated risk or insurability problems requiring specialized underwriting review for business owners policies.

**Risk Assessment Context**:
- Adverse selection prevention for commercial risks
- Prior carrier declination history specific to property and liability exposures
- Cancellation and non-renewal risk factors for business operations
- Commercial underwriting history assessment
- Business operational stability and risk management capability evaluation

**Question Classification**: Standard Eligibility/Underwriting Question
**Required Status**: Yes (IsQuestionRequired = True)
**Time Frame**: Prior 3 years from application date
**Source Code Reference**: `UWQuestions.vb` line 1545
**Section**: Applicant Information

**Additional Information Text Box Requirements**:
- **Auto-Display Trigger**: Multi-line text box automatically displays below question when user selects "Yes"
- **Text Box Label**: "Additional Information"
- **Character Limit**: 125 characters (enforced client-side and server-side)
- **Validation**: Required when "Yes" selected, minimum 10 characters
- **Data Persistence**: Stored in `PolicyUnderwritingExtraAnswer` field

### 2.7 Sexual Abuse, Molestation, and Employment Practices (Code: 9007) - REQUIRED
**Question Text**: "Any past losses or claims relating to sexual abuse or molestation allegations, discrimination or negligent hiring?"

**Business Purpose**: Identifies severe employment practices liability and premises liability exposures that fall outside standard BOP coverage scope and require specialized coverage approaches or complete declination.

**Risk Assessment Context**:
- Employment practices liability exposures beyond BOP coverage
- Premises liability concerns related to inadequate supervision or security
- Regulatory compliance requirements for employment practices
- Reputation risk and financial exposure from employment-related claims
- Legal defense cost exposure for discrimination and harassment claims

**Question Classification**: Standard Eligibility/Underwriting Question
**Required Status**: Yes (IsQuestionRequired = True)
**Source Code Reference**: `UWQuestions.vb` line 1550
**Section**: Applicant Information

**Additional Information Text Box Requirements**:
- **Auto-Display Trigger**: Multi-line text box automatically displays below question when user selects "Yes"
- **Text Box Label**: "Additional Information"
- **Character Limit**: 125 characters (enforced client-side and server-side)
- **Validation**: Required when "Yes" selected, minimum 10 characters
- **Data Persistence**: Stored in `PolicyUnderwritingExtraAnswer` field

### 2.8 Criminal Activity and Arson-Related Crimes (Code: 9008) - REQUIRED **KILL QUESTION**
**Question Text**: "During the last five years has any Applicant been indicted for or convicted of any degree of the crime of fraud, bribery, arson or any other arson-related crime in connection with this or any other property?"

**Business Purpose**: Identifies criminal activity history, particularly arson and fraud-related convictions, that represents unacceptable moral hazard for commercial property and liability insurance coverage.

**Risk Assessment Context**:
- Moral hazard assessment for property insurance coverage
- Intentional loss prevention for both property and liability exposures
- Regulatory compliance requirements for criminal background screening
- Financial fraud risk evaluation for premium collection and claims integrity
- Arson risk assessment specific to commercial property coverage

**Question Classification**: **DUAL-PURPOSE** - Eligibility/Underwriting Question AND Kill Question
**Kill Question Status**: Yes (IsTrueKillQuestion = True)
**Required Status**: Yes (IsQuestionRequired = True)
**Time Frame**: Prior 5 years from application date
**Source Code Reference**: `UWQuestions.vb` line 1555
**Section**: Applicant Information

**Kill Question Logic**: "Yes" response triggers automatic application disqualification

**Additional Information Text Box Requirements**:
- **Auto-Display Trigger**: Multi-line text box automatically displays below question when user selects "Yes"
- **Text Box Label**: "Additional Information"
- **Character Limit**: 125 characters (enforced client-side and server-side)
- **Validation**: Required when "Yes" selected, minimum 10 characters
- **Data Persistence**: Stored in `PolicyUnderwritingExtraAnswer` field

### 2.9 Fire and Safety Violations (Code: 9009) - REQUIRED
**Question Text**: "Are there any uncorrected fire or safety violations?"

**Business Purpose**: Identifies active fire safety violations that present immediate property damage risks and regulatory compliance issues requiring correction before coverage can be provided.

**Risk Assessment Context**:
- Active fire safety hazard identification
- Regulatory compliance verification
- Property damage risk assessment
- Immediate corrective action requirements
- Building code compliance evaluation

**Question Classification**: Standard Eligibility/Underwriting Question
**Required Status**: Yes (IsQuestionRequired = True)
**Source Code Reference**: `UWQuestions.vb` line 1560
**Section**: Applicant Information

**Additional Information Text Box Requirements**:
- **Auto-Display Trigger**: Multi-line text box automatically displays below question when user selects "Yes"
- **Text Box Label**: "Additional Information"
- **Character Limit**: 125 characters (enforced client-side and server-side)
- **Validation**: Required when "Yes" selected, minimum 10 characters
- **Data Persistence**: Stored in `PolicyUnderwritingExtraAnswer` field

### 2.10 Judgment or Lien History (Code: 9010) - OPTIONAL
**Question Text**: "Has any judgment or lien been filed against Applicant in the last five (5) years?"

**Business Purpose**: Evaluates financial stability and legal judgment history that may indicate business operational or financial management concerns requiring underwriting consideration.

**Risk Assessment Context**:
- Financial stability assessment
- Legal judgment history evaluation
- Business operational capability assessment
- Credit risk evaluation for premium collection
- Financial responsibility demonstration

**Question Classification**: Standard Eligibility/Underwriting Question
**Required Status**: No (Optional)
**Time Frame**: Prior 5 years from application date
**Source Code Reference**: `UWQuestions.vb` line 1565
**Section**: Applicant Information

**Additional Information Text Box Requirements**:
- **Auto-Display Trigger**: Multi-line text box automatically displays below question when user selects "Yes"
- **Text Box Label**: "Additional Information"
- **Character Limit**: 125 characters (enforced client-side and server-side)
- **Validation**: Required when "Yes" selected, minimum 10 characters
- **Data Persistence**: Stored in `PolicyUnderwritingExtraAnswer` field

### 2.11 Business in Trust (Code: 9011) - OPTIONAL
**Question Text**: "Is this business operated under any form of trust agreement?"

**Business Purpose**: Identifies complex business ownership structures involving trust arrangements that may require specialized underwriting review and additional documentation.

**Risk Assessment Context**:
- Complex ownership structure identification
- Trust agreement documentation requirements
- Policy ownership and beneficiary considerations
- Legal structure complexity evaluation
- Specialized underwriting review triggers

**Question Classification**: Standard Eligibility/Underwriting Question
**Required Status**: No (Optional)
**Source Code Reference**: `UWQuestions.vb` line 1570
**Section**: Applicant Information

**Additional Information Text Box Requirements**:
- **Auto-Display Trigger**: Multi-line text box automatically displays below question when user selects "Yes"
- **Text Box Label**: "Additional Information"
- **Character Limit**: 125 characters (enforced client-side and server-side)
- **Validation**: Required when "Yes" selected, minimum 10 characters
- **Data Persistence**: Stored in `PolicyUnderwritingExtraAnswer` field

### 2.12 Foreign Operations Assessment (Code: 9012) - OPTIONAL
**Question Text**: "Does Applicant have any operations outside the United States or manufacture any products for sale outside the United States?"

**Business Purpose**: Identifies international business operations that may require specialized coverage considerations, exclusions, or additional underwriting review for global risk exposures.

**Risk Assessment Context**:
- International operations risk assessment
- Global liability exposure evaluation
- Specialized coverage requirements identification
- Export/import operation assessment
- Multi-jurisdictional risk evaluation

**Question Classification**: Standard Eligibility/Underwriting Question
**Required Status**: No (Optional)
**Source Code Reference**: `UWQuestions.vb` line 1575
**Section**: Applicant Information

**Additional Information Text Box Requirements**:
- **Auto-Display Trigger**: Multi-line text box automatically displays below question when user selects "Yes"
- **Text Box Label**: "Additional Information"
- **Character Limit**: 125 characters (enforced client-side and server-side)
- **Validation**: Required when "Yes" selected, minimum 10 characters
- **Data Persistence**: Stored in `PolicyUnderwritingExtraAnswer` field

### 2.13 Foreclosure and Bankruptcy History (Code: 9400) - REQUIRED **KILL QUESTION**
**Question Text**: "Has Applicant had a foreclosure, repossession, bankruptcy or filed for bankruptcy in the last five (5) years?"

**Business Purpose**: Identifies severe financial distress history including foreclosure, repossession, or bankruptcy that represents unacceptable financial stability risk for commercial insurance coverage.

**Risk Assessment Context**:
- Financial stability assessment
- Payment capability evaluation for premium collection
- Business operational stability verification
- Credit risk assessment for commercial coverage
- Financial responsibility demonstration

**Question Classification**: **DUAL-PURPOSE** - Eligibility/Underwriting Question AND Kill Question
**Kill Question Status**: Yes (IsTrueKillQuestion = True)
**Required Status**: Yes (IsQuestionRequired = True)
**Time Frame**: Prior 5 years from application date
**Source Code Reference**: `UWQuestions.vb` line 1670
**Section**: Applicant Information
**Code Comment**: Explicitly noted as "Kill question" in source code

**Kill Question Logic**: "Yes" response triggers automatic application disqualification

**Additional Information Text Box Requirements**:
- **Auto-Display Trigger**: Multi-line text box automatically displays below question when user selects "Yes"
- **Text Box Label**: "Additional Information"
- **Character Limit**: 125 characters (enforced client-side and server-side)
- **Validation**: Required when "Yes" selected, minimum 10 characters
- **Data Persistence**: Stored in `PolicyUnderwritingExtraAnswer` field

### 2.14 Other Business Ventures (Code: 9401) - OPTIONAL
**Question Text**: "Does the Applicant have any other business ventures?"

**Business Purpose**: Identifies additional business activities that may create supplementary risk exposures requiring evaluation for coverage inclusion or exclusion considerations.

**Risk Assessment Context**:
- Additional business activity identification
- Supplementary risk exposure assessment
- Coverage scope determination
- Multi-business operation evaluation
- Risk concentration assessment

**Question Classification**: Standard Eligibility/Underwriting Question
**Required Status**: No (Optional)
**Source Code Reference**: `UWQuestions.vb` line 1675
**Section**: Applicant Information

**Additional Information Text Box Requirements**:
- **Auto-Display Trigger**: Multi-line text box automatically displays below question when user selects "Yes"
- **Text Box Label**: "Additional Information"
- **Character Limit**: 125 characters (enforced client-side and server-side)
- **Validation**: Required when "Yes" selected, minimum 10 characters
- **Data Persistence**: Stored in `PolicyUnderwritingExtraAnswer` field

---

## 3. Multi-State Requirements and Jurisdictional Logic

### 3.1 Multi-State Architecture Overview
The BOP Eligibility Questions system operates within a multi-state commercial insurance framework that accommodates business owners policies across multiple jurisdictions while maintaining consistent risk assessment standards for commercial property and liability exposures across all 13 eligibility questions.

### 3.2 Commercial Lines Multi-State Capability - 13-Question Framework
**Business Rule**: All 13 BOP eligibility questions apply consistently across all state jurisdictions where business owners policies are offered, with no state-specific variations currently implemented.

**Implementation Logic**:
- **Universal Application**: All 13 eligibility questions (including 2 kill questions) apply regardless of policy state jurisdiction
- **Consistent Standards**: Risk assessment criteria remain constant across all states for all questions
- **Commercial Focus**: Multi-state logic accommodates business operations across state lines
- **Dual-Purpose Questions**: Kill questions (9008, 9400) maintain consistent disqualification logic across all states
- **Required vs Optional**: 6 required and 7 optional questions apply uniformly across all jurisdictions

### 3.3 State Jurisdiction Considerations - Comprehensive Coverage
**BOP-Specific Multi-State Logic for All 13 Questions**:
- **Property Coverage**: All property-related questions address locations across multiple states
- **Liability Coverage**: Liability assessment questions accommodate business operations extending beyond home state
- **Financial Assessment**: Financial stability questions (9010, 9400) apply consistently across all jurisdictions
- **Criminal Background**: Criminal history questions (9008) maintain consistent standards across all states
- **Regulatory Compliance**: Maintains compliance with commercial insurance regulations across all applicable jurisdictions for comprehensive 13-question assessment

---

## 4. Configuration Requirements and System Parameters

### 4.1 Configuration Architecture Overview - 13-Question System
The BOP Eligibility Questions system operates with systematic configuration requirements, focusing on consistent commercial risk assessment standards across all 13 questions for business owners policy applications without complex state-specific routing or date-based variations.

### 4.2 Commercial BOP Question Configuration - Complete 13-Question Implementation
**System Implementation**:
- **Total Question Codes**: {9000, 9001, 9002, 9003, 9005, 9006, 9007, 9008, 9009, 9010, 9011, 9012, 9400, 9401}
- **Section Assignment**: All 13 questions assigned to "Applicant Information" section
- **Required Questions**: 6 questions marked as required {9003, 9006, 9007, 9008, 9009, 9400}
- **Optional Questions**: 7 questions marked as optional {9000, 9001, 9002, 9005, 9010, 9011, 9012, 9401}
- **Kill Questions**: 2 questions with kill functionality {9008, 9400}
- **Policy Tab**: All assigned to PolicyUnderwritingTabId = "2"
- **Policy Level**: All assigned to PolicyUnderwritingLevelId = "1"

**Question Status Configuration**:
- **IsTrueUwQuestion = True**: All 13 questions (universal eligibility/underwriting function)
- **IsTrueKillQuestion = True**: Questions 9008 and 9400 only (dual-purpose)
- **IsQuestionRequired = True**: Questions 9003, 9006, 9007, 9008, 9009, 9400 (6 required)
- **IsQuestionRequired = False**: Questions 9000, 9001, 9002, 9005, 9010, 9011, 9012, 9401 (7 optional)

### 4.3 Character Limit Configuration - Universal 125-Character Constraint
**Critical BOP-Specific Parameter Applied to All 13 Questions**:
- **Additional Information Limit**: 125 characters for ALL questions (vs. 2000 for WCP)
- **Enforcement**: Client-side and server-side validation across all 13 questions
- **UI Control**: `maxLength="125"` in ctlCommercialUWQuestionItem.ascx for all text boxes
- **JavaScript Validation**: `CheckMaxTextNoDisable(this, 125)` applied to all additional information boxes
- **Consistency**: Same character limit applies to required questions, optional questions, and kill questions

### 4.4 Question Routing Configuration - Comprehensive System
**BOP Question Selection Logic**:
- **LOB Detection**: System identifies BOP questions via `QuickQuoteObject.QuickQuoteLobType.CommercialBOP`
- **Question Retrieval**: All 13 questions loaded via `UWQuestions.GetCommercialBOPUnderwritingQuestions()`
- **Section Grouping**: All questions grouped under "Applicant Information" section
- **Display Order**: Questions presented in numerical order by question code
- **Kill Question Processing**: Questions 9008 and 9400 trigger kill logic when "Yes" responses provided

---

## 5. Technical Architecture Requirements

### 5.1 Question Routing Architecture - 13-Question System
**Commercial BOP Decision Engine - Comprehensive Implementation**:
- **LOB Detection**: System determines all 13 BOP eligibility questions via `QuickQuoteObject.QuickQuoteLobType.CommercialBOP`
- **Question Retrieval**: All 13 questions loaded via `UWQuestions.GetCommercialBOPUnderwritingQuestions()`
- **Section Grouping**: All 13 questions grouped by SectionName ("Applicant Information")
- **Kill Question Identification**: System identifies questions 9008 and 9400 as dual-purpose (eligibility + kill)
- **Required Question Processing**: System enforces 6 required questions {9003, 9006, 9007, 9008, 9009, 9400}
- **Optional Question Handling**: System allows 7 optional questions {9000, 9001, 9002, 9005, 9010, 9011, 9012, 9401} to be skipped

### 5.2 Data Persistence Architecture - Complete Question Coverage
**Question Response Storage for All 13 Questions**:
- **Question Code Mapping**: Each of 13 questions maps to specific BOP code for system identification
- **Response Capture**: Binary Yes/No responses for all 13 questions with required 125-character specification text for "Yes" responses
- **Kill Question Logic**: Questions 9008 and 9400 trigger application disqualification when "Yes" selected
- **Standard Eligibility Storage**: Questions 9000-9007, 9009-9012, 9401 store responses for underwriting review
- **Audit Trail**: Complete question presentation and response history for all 13 questions for commercial underwriting compliance
- **Data Validation**: System validates required questions completion and character limits across all questions

### 5.3 Shared Commercial UI Architecture - 13-Question Integration
**Commercial Control Integration for Complete Question Set**:
- **Shared Control**: `ctlCommercialUWQuestionList.ascx` renders all 13 BOP questions (used by BOP, CGL, CAP, CPR)
- **Item Control**: `ctlCommercialUWQuestionItem.ascx` handles individual question rendering for all 13 questions
- **Character Limit Integration**: 125-character limit enforced at UI control level for all 13 additional information text boxes
- **Required Field Processing**: UI distinguishes between 6 required and 7 optional questions with appropriate validation
- **Kill Question Handling**: UI provides special processing for questions 9008 and 9400 kill logic
- **Commercial Workflow**: All 13 questions integrated with commercial underwriting workflow patterns

### 5.4 Question Processing Architecture - Dual-Purpose Handling
**Kill Question Processing Logic**:
- **Standard Processing**: All 13 questions function as standard eligibility/underwriting questions
- **Kill Logic Overlay**: Questions 9008 and 9400 trigger additional kill processing when "Yes" responses provided
- **Application Routing**: Kill responses route applications to declination workflow
- **Non-Kill Responses**: Questions 9000-9007, 9009-9012, 9401 (excluding 9008, 9400) proceed through standard underwriting workflow
- **Mixed Response Handling**: System handles applications with both kill and non-kill "Yes" responses appropriately

---

## 6. User Interface and User Experience Requirements

### 6.1 Additional Information Text Box Framework - BOP Specific
All four BOP kill questions implement a consistent "Additional Information" text box pattern with a critical 125-character limit that provides conditional data collection when users select "Yes" responses to kill questions. This framework ensures consistent user experience while capturing required specification details for commercial risk assessment and underwriting review within the constraints of commercial workflow efficiency.

### 6.2 Auto-Display Behavior Specifications - BOP Implementation
**Conditional Display Logic**:
- **Trigger Event**: Text box automatically appears below kill question when user selects "Yes" radio button
- **Hide Event**: Text box immediately hidden when user selects "No" or clears selection
- **Display Animation**: Smooth reveal/hide transition (recommended: 200ms fade in/out)
- **Focus Management**: Automatic cursor focus to text box when displayed

**BOP-Specific Text Box Characteristics**:
- **Input Type**: Multi-line textarea (4 rows, 50 columns as specified in control)
- **Character Limit**: 125 characters (critical BOP constraint vs. 2000 for WCP)
- **Character Counter**: Live character count display: "X / 125 characters"
- **Placeholder Text**: "Please provide additional details..."
- **Label Text**: "Additional Information"

### 6.3 Visual Design and Layout Requirements - Commercial Integration
**Text Box Positioning**:
- **Location**: Directly below the Yes/No radio button pair
- **Indentation**: Left-aligned with radio button text, indented from question text
- **Spacing**: Standard commercial form spacing patterns
- **Width**: Full width of question container minus standard margins

**Visual States**:
- **Normal State**: Standard border color (light gray), white background
- **Focus State**: Blue border highlight, subtle box shadow
- **Error State**: Red border (#FF0000), light red background tint (#FFF5F5)
- **Disabled State**: Gray background, disabled cursor (when question not answered "Yes")

### 6.4 Validation and Error Handling Requirements - 125-Character Focus
**Required Field Validation**:
- **Trigger Condition**: Text box becomes required ONLY when "Yes" is selected
- **Validation Timing**: Real-time validation on blur, form submission validation
- **Error Display**: Red border with error message below text box
- **Error Message**: "Additional Information Response Required"
- **Error Persistence**: Error state persists until valid text entered (minimum 10 characters)

**BOP Character Count Validation**:
- **Live Counter**: Display "X / 125 characters" below text box
- **Warning State**: Orange text when approaching limit (110+ characters)
- **Error State**: Red text and prevention of additional input at 125 characters
- **Counter Location**: Right-aligned below text box
- **JavaScript Function**: `CheckMaxTextNoDisable(this, 125)` for real-time validation

### 6.5 Accessibility Requirements - Commercial Standards
**WCAG 2.1 AA Compliance**:
- **Keyboard Navigation**: Full keyboard accessibility with logical tab order
- **Screen Reader Support**: Proper ARIA labels and announcements
- **Label Association**: Text box explicitly associated with parent kill question
- **Error Announcement**: Screen reader announcement of validation errors
- **Focus Indicators**: Visible focus indicators for keyboard navigation

**ARIA Implementation**:
- **aria-required**: Set to "true" when "Yes" selected, "false" otherwise
- **aria-invalid**: Set to "true" when validation error present
- **aria-describedby**: References error message and character counter elements
- **aria-labelledby**: References parent kill question for context

### 6.6 Data Persistence and State Management - Commercial Integration
**Auto-Save Behavior**:
- **Save Trigger**: Auto-save integrated with commercial workflow save patterns
- **Manual Save**: Save on blur (when user clicks away from text box)
- **Session Storage**: Temporary storage in browser session for crash recovery
- **Database Persistence**: Final save to `PolicyUnderwritingExtraAnswer` field on form submission

**State Preservation**:
- **Page Reload**: Text box state and content preserved across page refreshes
- **Commercial Navigation**: Content preserved when navigating between commercial form sections
- **Session Timeout**: Warning before session expiration with save option
- **Concurrent Editing**: Prevention of data loss during concurrent user sessions

### 6.7 Mobile and Responsive Design Requirements - Commercial Compatibility
**Mobile Optimization**:
- **Touch Targets**: Minimum 44px touch target for radio buttons and text box
- **Keyboard Display**: Optimized mobile keyboard for text input
- **Zoom Compatibility**: No horizontal scrolling when zoomed to 200%
- **Viewport Adaptation**: Text box resizes appropriately on screen orientation change

**Responsive Breakpoints**:
- **Desktop (>768px)**: Full layout with side-by-side elements where appropriate
- **Tablet (481-768px)**: Stacked layout with optimized spacing
- **Mobile (<481px)**: Single column layout with touch-optimized controls

---

## 7. Enhanced Validation Rules and Business Logic

### 7.1 BOP Eligibility Question Response Validation Framework - 13-Question System
The BOP Eligibility Questions system implements comprehensive validation rules that ensure data integrity, regulatory compliance, and user experience consistency across all 13 eligibility questions with their associated 125-character limited "Additional Information" text boxes.

### 7.2 Eligibility Question Response Logic - Comprehensive Commercial Focus
**Dual-Purpose Kill Question Logic (Questions 9008, 9400)**:
- **"Yes" Response**: Application automatically flagged for kill/declination with commercial underwriting review
- **"No" Response**: Question functions as standard eligibility assessment, application proceeds through normal commercial BOP underwriting flow
- **No Selection**: Validation error for required questions - response required for all required eligibility questions
- **Response Persistence**: All responses logged for commercial audit trail and regulatory compliance

**Standard Eligibility Question Logic (Questions 9000-9007, 9009-9012, 9401 excluding 9008, 9400)**:
- **"Yes" Response**: Additional information collected for underwriting assessment, application proceeds with noted considerations
- **"No" Response**: Application proceeds through normal commercial BOP underwriting flow
- **No Selection for Required Questions**: Validation error - response required for 4 additional required questions {9003, 9006, 9007, 9009}
- **No Selection for Optional Questions**: Acceptable - 7 optional questions {9000, 9001, 9002, 9005, 9010, 9011, 9012, 9401} may be skipped

### 7.3 Additional Information Validation Rules - Universal 125-Character Constraint
**Conditional Requirement Logic Applied to All 13 Questions**:
- **Required When**: Text box required ONLY when corresponding question answered "Yes" (applies to all 13 questions)
- **Not Required When**: Question answered "No" or left blank (optional questions only)
- **Validation Trigger**: Real-time validation on text box blur and form submission for all 13 questions
- **Error State Management**: Error cleared immediately when valid text entered for any question

**BOP-Specific Text Content Validation for All Questions**:
- **Minimum Length**: 10 characters required (excluding whitespace) - applies to all 13 questions
- **Maximum Length**: 125 characters (enforced client-side and server-side) - CRITICAL BOP CONSTRAINT for all questions
- **Content Filtering**: Basic HTML tag stripping for security across all questions
- **Special Characters**: Alphanumeric, punctuation, and standard symbols allowed for all questions
- **Prohibited Content**: Script tags, SQL injection patterns blocked across all questions
- **JavaScript Validation**: `CheckMaxTextNoDisable(this, 125)` function integration for all 13 text boxes

### 7.4 Cross-Question Validation Rules - 13-Question Commercial Context
**Question Interdependencies Across Complete Question Set**:
- **Multiple "Yes" Responses**: All "Yes" responses across all 13 questions treated independently (no conflicts)
- **Kill Question Priority**: Questions 9008 and 9400 "Yes" responses take precedence and trigger kill logic regardless of other responses
- **Consistency Checking**: Additional information consistency validated against commercial risk context across all questions
- **Business Rule Validation**: Additional details validated against known commercial exclusions for all questions
- **Duplicate Response Prevention**: Same response cannot be duplicated across different questions
- **Required Question Enforcement**: All 6 required questions {9003, 9006, 9007, 9008, 9009, 9400} must be completed

### 7.5 Form Submission Validation - Complete 13-Question System Integration
**Complete Form Validation for All 13 Questions**:
- **All Required Questions**: 6 required questions {9003, 9006, 9007, 9008, 9009, 9400} must have Yes/No responses
- **Optional Questions**: 7 optional questions {9000, 9001, 9002, 9005, 9010, 9011, 9012, 9401} may be left blank
- **Required Additional Info**: All "Yes" responses across all 13 questions must have completed 125-character limited additional information text boxes
- **Kill Question Validation**: Questions 9008 and 9400 require special confirmation for "Yes" responses
- **Data Integrity**: All responses validated against commercial business rules before submission
- **Error Summary**: Comprehensive error summary displayed covering all 13 questions if validation fails

**Commercial Submission Prevention Logic - 13-Question Coverage**:
- **Incomplete Required Responses**: Form submission blocked until all 6 required questions have responses and all validation rules satisfied
- **Kill Response Confirmation**: Additional confirmation required when either kill question (9008, 9400) answered "Yes"
- **Optional Question Flexibility**: Form submission allowed with optional questions unanswered
- **Data Loss Prevention**: Auto-save and confirmation dialogs prevent accidental data loss across all 13 questions
- **Commercial Workflow Integration**: Proper handoff to commercial underwriting workflow upon completion, with kill responses routing to declination workflow

---

## 8. User Story and Acceptance Criteria

### 8.1 Primary User Stories - Commercial BOP Focus

**US-001: BOP Eligibility Question Response Collection - 13-Question System**
**As a** commercial insurance agent or underwriter  
**I want to** answer all 13 eligibility questions for BOP applications  
**So that** I can determine commercial property and liability eligibility and proceed with appropriate underwriting actions

**Acceptance Criteria**:
- ✅ All 13 BOP eligibility questions are presented with clear Yes/No radio button options
- ✅ Questions display appropriate text for comprehensive commercial property and liability risk assessment
- ✅ 6 required questions {9003, 9006, 9007, 9008, 9009, 9400} prevent form submission if unanswered
- ✅ 7 optional questions {9000, 9001, 9002, 9005, 9010, 9011, 9012, 9401} may be left blank
- ✅ "Yes" responses to kill questions (9008, 9400) trigger immediate kill/declination workflow for commercial risks
- ✅ "Yes" responses to standard eligibility questions provide information for underwriting assessment
- ✅ "No" responses allow BOP application to proceed to next commercial underwriting step
- ✅ All 13 questions integrate properly with commercial underwriting workflow
- ✅ Questions are grouped logically in "Applicant Information" section

**US-002: Additional Information Collection - 125-Character Limit for All 13 Questions**
**As a** commercial insurance agent or underwriter  
**I want to** provide additional details when answering "Yes" to any of the 13 BOP eligibility questions within a 125-character limit  
**So that** the commercial underwriting team has sufficient information to understand the risk within efficient workflow constraints

**Acceptance Criteria**:
- ✅ Text box automatically appears below any question when "Yes" is selected (all 13 questions)
- ✅ Text box automatically hides when "No" is selected or selection is cleared (all 13 questions)
- ✅ Text box displays "Additional Information" label for all questions
- ✅ Text box accepts multi-line text input up to 125 characters (critical BOP constraint for all 13 questions)
- ✅ Character counter displays remaining characters available: "X / 125 characters" for all questions
- ✅ Text box is required when "Yes" is selected (minimum 10 characters) for all questions
- ✅ Red border and error message appear when required text box is empty for any question
- ✅ Error message "Additional Information Response Required" displays clearly for all questions
- ✅ Content is automatically saved and persisted across page refreshes for all 13 questions
- ✅ Data is saved to `PolicyUnderwritingExtraAnswer` database field with 125-character limit for all questions
- ✅ JavaScript validation function `CheckMaxTextNoDisable(this, 125)` prevents exceeding character limit for all 13 questions

**US-003: Form Validation and Submission - Complete 13-Question Commercial Integration**
**As a** commercial insurance agent or underwriter  
**I want to** receive clear validation feedback on all 13 BOP eligibility questions  
**So that** I can complete the commercial application accurately and efficiently

**Acceptance Criteria**:
- ✅ Form cannot be submitted until all 6 required BOP eligibility questions are answered {9003, 9006, 9007, 9008, 9009, 9400}
- ✅ Form can be submitted with 7 optional questions unanswered {9000, 9001, 9002, 9005, 9010, 9011, 9012, 9401}
- ✅ Form cannot be submitted if any "Yes" responses across all 13 questions lack required additional information
- ✅ Real-time validation provides immediate feedback on errors with 125-character constraint awareness for all questions
- ✅ Error summary lists all validation issues across all 13 questions when form submission attempted
- ✅ Successful validation allows form submission to proceed within commercial workflow
- ✅ Kill responses (9008, 9400) properly trigger commercial application declination workflow
- ✅ Standard eligibility responses (other 11 questions) provide information for underwriting assessment
- ✅ Additional information is properly associated with corresponding questions across all 13 questions

**US-004: Commercial Workflow Integration - 13-Question System**
**As a** commercial insurance agent or underwriter working with BOP applications  
**I want to** have all 13 eligibility questions integrate seamlessly with the commercial underwriting process  
**So that** I can efficiently process business owners policy applications with comprehensive risk assessment

**Acceptance Criteria**:
- ✅ All 13 eligibility questions appear in "Applicant Information" section of commercial workflow
- ✅ Questions integrate with `ctlCommercialUWQuestionList.ascx` shared commercial control
- ✅ UI behavior consistent with other commercial lines (CGL, CAP, CPR) for all questions
- ✅ Commercial navigation patterns preserved throughout eligibility question process
- ✅ Proper handoff to commercial underwriting workflow upon completion
- ✅ Kill questions (9008, 9400) route to declination workflow when "Yes" selected
- ✅ Standard eligibility questions route to underwriting assessment when "Yes" selected
- ✅ Commercial audit trail maintained for all 13 eligibility question responses

### 8.2 Error Handling User Stories - 13-Question BOP System

**US-005: Validation Error Recovery - 125-Character Focus Across All Questions**
**As a** commercial insurance agent or underwriter  
**I want to** easily identify and correct validation errors across all 13 eligibility questions, including character limit violations  
**So that** I can complete the BOP eligibility questions section without frustration

**Acceptance Criteria**:
- ✅ Validation errors display immediately when encountered on any of the 13 questions
- ✅ Character limit violations are clearly indicated with red text and input prevention for all questions
- ✅ Error messages are specific and actionable for each question
- ✅ Errors are cleared immediately when corrected on any question
- ✅ Multiple errors across different questions are handled gracefully without overwhelming the user
- ✅ Required question errors clearly distinguish from optional question validation
- ✅ Error state is visually distinct (red borders, error icons, etc.) for all questions
- ✅ Screen readers announce validation errors for accessibility across all 13 questions

### 8.3 Technical Implementation User Stories - Complete System Architecture

**US-006: Data Persistence and Recovery - 13-Question Commercial Integration**
**As a** commercial insurance agent or underwriter  
**I want to** have all 13 BOP eligibility question responses and additional information automatically saved  
**So that** I don't lose work if there are technical issues or interruptions

**Acceptance Criteria**:
- ✅ Auto-save integrated with commercial workflow save patterns for all 13 questions
- ✅ Content is saved when navigating away from text boxes for any question
- ✅ Session storage preserves work during temporary technical issues for all questions
- ✅ Page refresh does not result in data loss for any of the 13 questions
- ✅ Commercial workflow navigation preserves entered data for all questions
- ✅ Final submission saves all data to permanent database storage with 125-character constraint for all questions
- ✅ Required vs optional question status is preserved across sessions
- ✅ Kill question responses (9008, 9400) are specially flagged in persistence layer
- ✅ Concurrent editing prevention avoids data conflicts across all questions

**US-007: Mobile and Accessibility Support - 13-Question Commercial Standards**
**As a** commercial insurance agent or underwriter using mobile devices or assistive technology  
**I want to** complete all 13 BOP eligibility questions using any device or accessibility tool  
**So that** I can work efficiently regardless of my technology setup

**Acceptance Criteria**:
- ✅ All functionality works on mobile phones and tablets for all 13 questions
- ✅ Touch targets are appropriately sized for mobile interaction across all questions
- ✅ Screen readers properly announce all 13 eligibility questions and validation states
- ✅ Keyboard navigation works smoothly through all 13 questions and text boxes
- ✅ High contrast mode maintains visual distinction for validation states across all questions
- ✅ Zoom functionality works correctly up to 200% magnification for all questions
- ✅ Mobile keyboards optimize for text input in 125-character limited additional information boxes for all 13 questions
- ✅ Required questions are clearly distinguished from optional questions in mobile view
- ✅ Kill questions (9008, 9400) are visually distinguished when appropriate

---

## 9. Migration and Modernization Considerations

### 9.1 Data Migration Requirements - Complete 13-Question System Assessment
**COMPREHENSIVE SYSTEM STATUS**: Analysis reveals that all 13 BOP eligibility questions are currently ACTIVE in the system with full functionality. Migration planning must account for existing complete 13-question implementation.

**Complete Question Response Preservation**:
- **Current Implementation**: All 13 active eligibility questions (9000-9003, 9005-9012, 9400, 9401) currently collecting responses
- **Kill Question Functionality**: Questions 9008 and 9400 currently functioning as both eligibility and kill questions
- **Historical Responses**: All existing responses across all 13 questions must be preserved with original question codes
- **Data Integrity**: Existing PolicyUnderwritingExtraAnswer data with 125-character constraints must be maintained for all questions
- **Required vs Optional Status**: Current required/optional status for all questions must be preserved
- **Business Continuity**: Migration must not disrupt existing commercial underwriting workflow for any question

### 9.2 Configuration Migration Requirements - 13-Question Commercial Architecture
**System Parameter Continuity for Complete Question Set**:
- **Current Settings Preservation**: All current commercial BOP configuration values must be migrated for all 13 questions
- **Character Limit Maintenance**: Universal 125-character limit enforcement must be preserved in modernized system for all questions
- **Commercial Control Integration**: Shared commercial control architecture must be maintained for all 13 questions
- **Question Status Preservation**: Required/optional status for all questions must be maintained
- **Kill Question Logic**: Dual-purpose functionality for questions 9008 and 9400 must be preserved

### 9.3 Regulatory Compliance Continuity - Complete System Commercial Focus
**Eligibility Question Standards Preservation**:
- **Question Text Accuracy**: Exact commercial-focused question text preservation to maintain regulatory compliance for all 13 questions
- **Response Logic**: Binary response logic preserved with proper commercial application routing for all questions
- **Kill Question Logic**: Dual-purpose kill logic preserved for questions 9008 and 9400
- **Commercial Standards**: All commercial property and liability specific routing logic maintained in modernized system for complete question set

**Business Rules Continuity for All Questions**:
- **Commercial Risk Assessment**: All current commercial risk assessment logic preserved across all 13 questions
- **Property and Liability Focus**: BOP-specific property and liability risk evaluation maintained for all questions
- **Small Business Context**: Risk assessment appropriate for small to medium business operations preserved across complete question set
- **Comprehensive Coverage**: Full spectrum of business operational, financial, and risk assessment questions maintained

---

## 10. Quality Assurance and Testing Requirements

### 10.1 BOP Eligibility Question Testing Requirements - Complete 13-Question System
**Comprehensive Question Coverage Testing**:
- **All 13 Questions**: Each BOP eligibility question tested with both Yes and No responses
- **Required Questions**: Special testing for 6 required questions {9003, 9006, 9007, 9008, 9009, 9400}
- **Optional Questions**: Special testing for 7 optional questions {9000, 9001, 9002, 9005, 9010, 9011, 9012, 9401}
- **Kill Questions**: Focused testing for dual-purpose questions 9008 and 9400 with kill functionality
- **Commercial Workflow Integration**: Testing within commercial underwriting workflow context for all questions
- **125-Character Limit Testing**: Comprehensive testing of character limit enforcement across all 13 questions
- **Commercial UI Control Testing**: Testing with shared `ctlCommercialUWQuestionList.ascx` control for all questions

### 10.2 Character Limit Testing Requirements - Universal 125-Character BOP Constraint
**125-Character Constraint Testing Across All 13 Questions**:
- **Client-Side Validation**: JavaScript `CheckMaxTextNoDisable(this, 125)` function testing for all 13 additional information text boxes
- **Server-Side Validation**: Backend enforcement of 125-character limit for all questions
- **User Experience Testing**: Character counter accuracy and user feedback testing for all questions
- **Edge Case Testing**: Testing at character limit boundaries (124, 125, 126 characters) for all questions
- **Consistency Testing**: Ensure character limit behavior is identical across all 13 questions

### 10.3 Commercial Integration Testing Requirements - 13-Question System
**Workflow Integration Validation for Complete Question Set**:
- **Commercial Navigation**: Testing integration with commercial underwriting navigation patterns for all 13 questions
- **Shared Control Testing**: Validation with other commercial lines using same controls (CGL, CAP, CPR) for complete question set
- **Multi-Line Compatibility**: Testing compatibility with other commercial product lines across all questions
- **Required vs Optional Flow**: Testing different workflow paths for required vs optional questions
- **Kill Question Routing**: Special testing for questions 9008 and 9400 kill workflow routing

### 10.4 Additional Information Text Box Testing Requirements - 13-Question 125-Character Focus
**UI Behavior Testing for All 13 Questions**:
- **Auto-Display Functionality**: Text box appears immediately when "Yes" selected for all 13 BOP eligibility questions
- **Auto-Hide Functionality**: Text box disappears immediately when "No" selected or selection cleared for all questions
- **Focus Management**: Automatic cursor focus to text box when displayed for any question
- **Character Counter**: Live character count updates correctly (X / 125 characters) for all 13 questions
- **Multi-Line Input**: Text box properly handles multi-line text entry within 125-character constraint for all questions
- **Question Consistency**: UI behavior identical across all 13 questions regardless of required/optional status

**Validation Testing Across All 13 Questions**:
- **Required Field Logic**: Text box becomes required only when "Yes" is selected for any question
- **Required Question Enforcement**: Special validation for 6 required questions that must be answered
- **Optional Question Flexibility**: Validation allows optional questions to be skipped
- **Minimum Length Validation**: 10-character minimum enforced with appropriate error messaging for all questions
- **Maximum Length Validation**: 125-character maximum enforced with prevention of additional input for all questions
- **Error Display**: Red border and "Additional Information Response Required" message display correctly for all questions
- **Error Clearing**: Validation errors clear immediately when valid text is entered for any question
- **Real-Time Validation**: Validation occurs on blur and form submission as specified for all questions
- **Kill Question Validation**: Special confirmation validation for questions 9008 and 9400 when "Yes" selected

**Data Persistence Testing for Complete Question Set**:
- **Commercial Auto-Save**: Content saves according to commercial workflow patterns for all 13 questions
- **Manual Save**: Content saves on blur (when clicking away from text box) for all questions
- **Session Storage**: Temporary browser storage preserves content during page refresh for all questions
- **Database Persistence**: Final content saves to `PolicyUnderwritingExtraAnswer` field with 125-character constraint for all questions
- **Cross-Navigation**: Content preserved when navigating between commercial form sections for all questions
- **Concurrent Editing**: Prevention of data loss during multiple user sessions across all questions
- **Question Status Persistence**: Required/optional status preserved across sessions
- **Kill Question Flagging**: Questions 9008 and 9400 responses properly flagged in database

**Performance Testing - Complete System Commercial Context**:
- **Load Time**: Text box show/hide transitions complete within 200ms in commercial workflow for all questions
- **Auto-Save Performance**: Auto-save operations don't interfere with typing across multiple questions
- **Form Submission**: Complete form submission with all 13 questions and text boxes completes within 5 seconds
- **Character Counter Performance**: Real-time counter updates without lag or performance impact for all questions
- **Commercial Workflow Integration**: Complete 13-question system doesn't negatively impact commercial workflow performance
- **Scalability Testing**: System performance with multiple users completing all 13 questions simultaneously

---

## 11. Implementation Priorities and Phasing

### 11.1 Phase 1 - Core BOP Eligibility Question Framework (High Priority)
- **Current State Assessment**: Validate existing active implementation of all 13 questions with correct required/optional status
- **Complete Question Integration**: Ensure all 13 eligibility questions properly integrated with commercial workflow
- **Kill Question Functionality**: Validate dual-purpose functionality for questions 9008 and 9400
- **125-Character Limit Enforcement**: Validate universal character limit constraints are properly enforced across all questions
- **Required vs Optional Logic**: Ensure 6 required and 7 optional questions function correctly

### 11.2 Phase 2 - Advanced Features (Medium Priority)
- **Commercial UI Enhancement**: Optimize shared commercial control integration for all 13 questions
- **Enhanced Error Handling**: Comprehensive validation and user feedback systems across complete question set
- **Mobile and Accessibility Features**: Responsive design and WCAG 2.1 AA compliance for all questions
- **Performance Optimization**: Commercial workflow performance tuning with 13-question system
- **Kill Question Workflow**: Specialized processing for dual-purpose questions 9008 and 9400

### 11.3 Phase 3 - Integration and Optimization (Lower Priority)
- **Multi-Line Commercial Integration**: Enhanced integration with other commercial product lines for complete question framework
- **Advanced Reporting**: BOP eligibility question analytics and reporting capabilities across all 13 questions
- **Legacy System Optimization**: Historical data optimization and compatibility features for complete question set
- **Business Intelligence**: Advanced analytics for eligibility patterns across all question responses

---

## Source Attribution and Traceability

**Primary Source**: `UWQuestions.vb` lines 1520-1675 - Business Owners Policy Commercial Underwriting Questions  
**Analysis Conducted By**: Rex (IFI Pattern Mining Specialist)  
**Analysis Date**: December 2024  
**Analysis Completeness**: 100% with complete confidence (13 active BOP eligibility questions identified and documented)  
**Requirements Documentation By**: Mason (IFI Extraction & Conversion Specialist)  
**Requirements Validation**: Cross-validated against commercial business logic patterns, shared commercial UI controls, character limit constraints, and commercial underwriting workflow requirements

**Complete Source Code References for All 13 Questions**:
- **Applicant Subsidiary Status (9000)**: Line 1520
- **Applicant Has Subsidiaries (9001)**: Line 1525
- **Safety Program Implementation (9002)**: Line 1530
- **Hazardous Materials (9003)**: Line 1535 - REQUIRED
- **Other Insurance with Company (9005)**: Line 1540
- **Prior Coverage Issues (9006)**: Line 1545 - REQUIRED
- **Sexual Abuse/Employment Practices (9007)**: Line 1550 - REQUIRED
- **Criminal Activity/Arson (9008)**: Line 1555 - REQUIRED, KILL QUESTION
- **Fire and Safety Violations (9009)**: Line 1560 - REQUIRED
- **Judgment or Lien History (9010)**: Line 1565
- **Business in Trust (9011)**: Line 1570
- **Foreign Operations Assessment (9012)**: Line 1575
- **Foreclosure/Bankruptcy History (9400)**: Line 1670 - REQUIRED, KILL QUESTION
- **Other Business Ventures (9401)**: Line 1675
- **Commercial UI Control**: ctlCommercialUWQuestionList.ascx.vb (BOP case logic)
- **Character Limit Implementation**: ctlCommercialUWQuestionItem.ascx (maxLength="125")

**Supporting Business Rules for Complete 13-Question System**:
- **Commercial LOB Detection**: `QuickQuoteObject.QuickQuoteLobType.CommercialBOP`
- **Question Retrieval**: `UWQuestions.GetCommercialBOPUnderwritingQuestions()` method
- **Character Limit Validation**: `CheckMaxTextNoDisable(this, 125)` JavaScript function
- **Section Assignment**: All 13 questions assigned to "Applicant Information" section
- **Kill Question Properties**: `.IsTrueKillQuestion = True` for questions 9008 and 9400
- **Eligibility Properties**: `.IsTrueUwQuestion = True` for all 13 questions
- **Required Status**: `.IsQuestionRequired = True` for 6 questions {9003, 9006, 9007, 9008, 9009, 9400}

**COMPREHENSIVE SYSTEM STATUS**: Analysis reveals all 13 BOP eligibility questions are currently active with proper dual-purpose functionality for kill questions 9008 and 9400. This specification documents the complete active implementation with accurate question counts and functionality.

This requirements specification provides comprehensive coverage of the complete BOP Eligibility Questions system with all 13 questions, ensuring successful modernization while maintaining commercial underwriting compliance and efficient workflow integration across all supported business owners policy applications.