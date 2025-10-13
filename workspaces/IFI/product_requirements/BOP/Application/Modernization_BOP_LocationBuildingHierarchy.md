# BOP Location-Building Hierarchy Requirements

## Document Information
- **Feature**: Location-Building Hierarchy Management
- **Line of Business**: BOP (Business Owners Policy)
- **Analysis Date**: 2024-12-19
- **Source Analysis**: Rex (IFI Pattern Miner) - 92% Completeness
- **Document Author**: Mason (IFI Extraction & Conversion Craftsman)

## Business Overview

The BOP Location-Building Hierarchy system manages the one-to-many relationship between commercial property locations and their associated buildings within a Business Owners Policy application. This system supports multi-building commercial properties where each location can contain multiple buildings, each with unique characteristics, rating factors, and additional interest designations.

### Commercial Property Context
This system is specifically designed for **commercial property insurance** under BOP (Business Owners Policy) coverage, distinguishing it from personal lines such as homeowners or workers' compensation policies. The system accommodates complex commercial property structures including multi-tenant buildings, business complexes, and facilities with multiple structures at a single location.

## System Architecture

### Hierarchy Structure
```
BOP Application
└── Location Collection (1 to many)
    └── Building Collection (1 to many per location)
        ├── Building Properties
        ├── Rating Factors
        └── Additional Interests
```

### Data Model Overview
- **Location Entity**: `QuickQuoteLocation` - Represents a physical address/location
- **Building Entity**: `QuickQuoteBuilding` - Represents individual structures at a location
- **Index Management**: Zero-based indexing with user-friendly display numbering
- **Relationship**: One Location can contain multiple Buildings (1:N relationship)

## Location Management Requirements

### Functional Requirements

#### LM-001: Location Indexing and Navigation
**Requirement**: The system shall maintain zero-based indexing for locations with user-friendly display numbering starting from 1.

**Business Rules**:
- Internal index starts at 0, user display shows "Location #1", "Location #2", etc.
- LocationIndex property stored in ViewState for persistence across postbacks
- Index propagated to child building controls for hierarchy maintenance

**Source Reference**: `ctl_BOP_App_Location.ascx.vb`, Lines 6-14, 23-27
```vb
Public Property LocationIndex As Int32
    Get
        Return ViewState.GetInt32("vs_LocationIndex", -1)
    Set(value As Int32)
        ViewState("vs_LocationIndex") = value
        Me.ctl_BOP_App_BuildingList.LocationIndex = value
```

#### LM-002: Address Display and Validation
**Requirement**: The system shall display location addresses with intelligent truncation and validation patterns.

**Business Rules**:
- Address title display limited to 40 characters with ellipsis for overflow
- Format: "Location #[N] - [Address]" where N is user-friendly number (index + 1)
- Empty addresses display as "Location #[N]" without address suffix
- DisplayAddress property used for presentation layer formatting

**Acceptance Criteria**:
- ✅ Address longer than 40 characters truncated with "..." suffix
- ✅ Missing addresses handled gracefully without display errors
- ✅ Location number always displayed regardless of address availability

**Source Reference**: `ctl_BOP_App_Location.ascx.vb`, Lines 45-60
```vb
If MyLocation.Address.DisplayAddress.Length > titleLen Then
    title += MyLocation.Address.DisplayAddress.Substring(0, titleLen) & "..."
Else
    title += MyLocation.Address.DisplayAddress
End If
```

#### LM-003: Building Coordination
**Requirement**: The system shall coordinate building management within each location through child control integration.

**Business Rules**:
- Each location contains a BuildingList control for managing multiple buildings
- LocationIndex passed to building controls for hierarchical context
- Building operations coordinated through parent location control

**Source Reference**: `ctl_BOP_App_Location.ascx.vb`, Line 11; `ctl_BOP_App_Location.ascx`, Line 69

### User Stories

#### Story LM-US-001: Location Display Management
**As a** commercial insurance underwriter  
**I want** to view location information with clear identification and address display  
**So that** I can quickly identify and navigate between multiple property locations

**Acceptance Criteria**:
- Location header shows "Location #1 - 123 Main St..." format
- Long addresses truncated at 40 characters with ellipsis
- Missing addresses show "Location #N" format without errors
- Location accordion displays with appropriate hierarchy context

#### Story LM-US-002: Multi-Location Navigation
**As a** commercial insurance agent  
**I want** to navigate between multiple locations in a BOP application  
**So that** I can manage properties across different business locations

**Acceptance Criteria**:
- Each location maintains unique index for identification
- Navigation preserves location context across building management
- Location-specific operations isolated to appropriate location scope

## Building Management Requirements

### Functional Requirements

#### BM-001: Building Indexing and Identification
**Requirement**: The system shall maintain building indexes within each location with dual numbering context.

**Business Rules**:
- BuildingIndex zero-based within each location scope
- LocationIndex maintained for parent location context
- Building display format: "Building #[N] - [Description]" with 55-character limit
- Both indexes stored in ViewState for persistence

**Source Reference**: `ctl_BOP_App_Building.ascx.vb`, Lines 7-23
```vb
Public Property LocationIndex As Int32
Public Property BuildingIndex As Int32
```

#### BM-002: Building Property Management
**Requirement**: The system shall capture and validate building characteristics essential for commercial property rating.

**Building Characteristics**:
- Square Feet (numeric, required, > 0)
- Year Built (4-digit year, 1900 to current year)
- Number of Stories (numeric)
- Update Years: Roof, Plumbing, Electrical, Heating (all required, valid years)

**Validation Rules**:
- Square feet must be numeric and greater than zero
- Year fields must be between 1900 and current year
- All update years are mandatory fields
- NewCo configuration disables Square Feet and Year Built inputs

**Source Reference**: `ctl_BOP_App_Building.ascx.vb`, Lines 374-415 (ValidateControl method)

#### BM-003: Company Type Configuration Support
**Requirement**: The system shall support different input modes based on company type (NewCo vs OldCo).

**NewCo Mode**:
- Square Feet field disabled (populated from external source)
- Year Built field disabled (populated from external source)
- Number of Stories field disabled (populated from external source)
- Update years remain editable

**OldCo Mode**:
- All building characteristic fields editable
- Standard validation rules apply to all fields

**Source Reference**: `ctl_BOP_App_Building.ascx.vb`, Lines 132-154
```vb
If IsNewCo() Then
    txtSquareFeetNewCo.Enabled = False
    txtYearBuiltNewCo.Enabled = False
    txtNumOfStoriesNewCo.Enabled = False
```

#### BM-004: Additional Interest Management
**Requirement**: The system shall manage additional interests for both Building and Personal Property coverages.

**Additional Interest Types**:
- Building Additional Interests (linked to building coverage)
- Personal Property Additional Interests (linked to personal property coverage)
- ATIMA/ISAOA designation support (As Their Interest May Appear/Its Successors And/Or Assigns)

**Business Rules**:
- Additional interests loaded from quote-level collection
- Building-specific AI tagged with "Building" description
- Personal property AI tagged with "Business Personal Property" description  
- ATIMA/ISAOA combinations: None(0), ATIMA(1), ISAOA(2), Both(3)
- Loss payee name selection from available additional interests

**Source Reference**: `ctl_BOP_App_Building.ascx.vb`, Lines 245-320 (Save method AI handling)

### User Stories

#### Story BM-US-001: Building Characteristics Entry
**As a** commercial underwriter  
**I want** to capture building characteristics and rating factors  
**So that** I can accurately assess risk and calculate premiums

**Acceptance Criteria**:
- Building square footage captured and validated (numeric > 0)
- Year built captured within valid range (1900-current year)
- System update years captured for all major building systems
- Validation errors display with clear building context identification
- NewCo mode automatically disables certain fields with external data population

#### Story BM-US-002: Building Coverage Management
**As a** commercial insurance agent  
**I want** to view building and personal property coverage limits  
**So that** I can confirm appropriate coverage amounts for each building

**Acceptance Criteria**:
- Building limit displayed prominently in building header
- Personal property limit displayed prominently in building header
- Limits automatically populated from coverage configuration
- Display format consistent across all building instances

#### Story BM-US-003: Additional Interest Assignment
**As a** commercial underwriter  
**I want** to assign additional interests to building and personal property coverages  
**So that** I can properly designate loss payees and their interest types

**Acceptance Criteria**:
- Additional interests available from quote-level master list
- Separate assignment capability for building vs personal property
- ATIMA/ISAOA designation selectable for each interest type
- Loss payee type designation supported
- Changes saved appropriately to building-specific additional interest collection

## Technical Implementation Requirements

### Data Persistence
- **ViewState Storage**: LocationIndex and BuildingIndex maintained in ViewState
- **Quote Object Hierarchy**: Data persisted to QuickQuote.Locations.Buildings collection
- **Additional Interests**: Building-specific AI maintained within building scope, synchronized with quote-level master collection

### User Interface Patterns
- **Accordion Controls**: Location and building management through accordion UI pattern
- **Repeater Controls**: Dynamic generation of location and building controls
- **Client Script**: Accordion behavior and event propagation management
- **Responsive Design**: Appropriate display across commercial insurance application workflow

### Validation Framework
- **Group-based Validation**: Validation errors grouped by "Location #N Building #M" context
- **Cross-Control Validation**: Parent location coordinates child building validation
- **Business Rule Enforcement**: Validation consistent with commercial property underwriting requirements

## Source Code Traceability

### Primary Source Files
- **ctl_BOP_App_Location.ascx.vb**: Location management and address display (85% completeness)
- **ctl_BOP_App_Building.ascx.vb**: Building properties and additional interest management (85% completeness)
- **ctl_BOP_App_LocationList.ascx.vb**: Location collection management (95% completeness)
- **ctl_BOP_App_BuildingList.ascx.vb**: Building collection management (95% completeness)

### Key Method References
- **Location.Populate()**: Lines 36-82, address display and title formatting
- **Location.Save()**: Lines 84-95, removed functionality migrated to building level
- **Location.ValidateControl()**: Lines 97-163, validation logic relocated to building level
- **Building.Populate()**: Lines 115-245, building data population and AI loading
- **Building.Save()**: Lines 247-320, building data persistence and AI management
- **Building.ValidateControl()**: Lines 374-415, comprehensive building validation

### Architecture Notes
- 6/8/2017 Refactoring: Square footage, year built, and system update fields migrated from Location to Building level
- Building-level validation replaces location-level validation for building-specific attributes
- Additional interest management implemented at building level with quote-level synchronization

## Validation and Testing Considerations

### Test Scenarios
1. **Multi-Location Management**: Verify proper indexing and context maintenance across multiple locations
2. **Multi-Building per Location**: Validate building management within location scope
3. **Address Display**: Test address truncation and missing address handling
4. **Building Validation**: Verify all building characteristic validation rules
5. **Additional Interest Management**: Test AI assignment and ATIMA/ISAOA designations
6. **NewCo/OldCo Modes**: Validate field enabling/disabling based on company type

### Quality Assurance
- All source references verified through direct code examination
- Business rules extracted from actual implementation logic
- Validation requirements based on existing validation framework
- User interface patterns consistent with existing BOP application architecture

---

**Document Status**: Complete  
**Review Status**: Ready for business domain validation  
**Implementation Readiness**: High - Based on 92% source code completeness analysis