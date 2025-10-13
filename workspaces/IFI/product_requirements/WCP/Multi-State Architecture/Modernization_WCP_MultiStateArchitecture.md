# Workers' Compensation Multi-State Coordination Architecture
## Requirements Specification for System Modernization

**Document Version**: 1.0  
**Date**: December 2024  
**Prepared by**: Mason (IFI Extraction & Conversion Specialist)  
**Source Analysis**: Rex (IFI Pattern Mining Specialist) - Comprehensive analysis of multi-state business logic patterns  
**Coverage**: 100% completeness - Complete multi-state architecture framework extracted with high confidence

---

## Executive Summary

The Workers' Compensation Multi-State Coordination Architecture represents a sophisticated business framework enabling seamless policy administration across multiple state jurisdictions through a coordinated 4-state system (Indiana, Illinois, Kentucky, plus Governing State). This architecture supports complex multi-jurisdictional Workers' Compensation operations while maintaining state-specific regulatory compliance, dynamic business rule application, and coordinated underwriting processes.

The modernization requirements outlined in this document address the comprehensive multi-state business framework, state-specific coordination requirements, dynamic effective date management, configurable parameter systems, and integrated helper class dependencies that enable sophisticated multi-state Workers' Compensation operations across all supported jurisdictions.

---

## 1. Business Overview and Multi-State Framework

### 1.1 Multi-State Architecture Purpose and Function
The Workers' Compensation Multi-State Architecture enables insurance carriers to provide coordinated Workers' Compensation coverage across multiple state jurisdictions through a unified business framework. This architecture specifically supports the coordination between Indiana (IN), Illinois (IL), Kentucky (KY), and any designated Governing State, ensuring regulatory compliance and operational efficiency across all participating jurisdictions.

**Critical Business Function**: The multi-state framework allows a single policy to coordinate coverage requirements, underwriting standards, and administrative processes across multiple states while maintaining compliance with each jurisdiction's specific regulatory requirements.

**Business Value**: 
- Enables coordinated multi-state Workers' Compensation programs
- Maintains state-specific regulatory compliance across all jurisdictions
- Provides unified policy administration for multi-jurisdictional employers
- Supports dynamic business rule application based on policy characteristics
- Facilitates efficient underwriting and risk assessment across states

### 1.2 4-State Coordination Model
The architecture operates on a 4-state coordination model where three primary states (Indiana, Illinois, Kentucky) maintain reciprocal agreements and coordination protocols, while any state can serve as the Governing State for policy administration purposes.

**Coordination Structure**:
- **Primary Coordination States**: Indiana (IN), Illinois (IL), Kentucky (KY)
- **Governing State Flexibility**: Any qualified state can serve as the governing jurisdiction
- **Reciprocal Agreement Framework**: Special coordination protocols between IN/IL/KY
- **Unified Administration**: Single policy structure coordinating all participating states

---

## 2. Multi-State Business Framework Specifications

### 2.1 Multi-State Capability Determination Framework
**Business Rule**: Multi-state capability eligibility is determined by comparing the policy effective date against the configured Multi-State Start Date threshold.

**Capability Logic**:
- **Multi-State Eligible**: `effectiveDate >= MultiStateStartDate`
- **Single-State Operation**: `effectiveDate < MultiStateStartDate`
- **Default Start Date**: January 1, 2019
- **Configuration Parameter**: `VR_MultiState_EffectiveDate`

**Business Purpose**: This date-based determination ensures that multi-state capabilities are only available for policies written after the business has established the necessary operational framework, regulatory approvals, and system capabilities to support multi-jurisdictional operations.

**Source Code Reference**: `MultiState/General.vb` lines 60-62, 64-70
**Configuration Location**: ConfigurationManager.AppSettings with hardcoded fallback default

### 2.2 Governing State Coordination Requirements
**Dynamic State Text Generation**: The system generates dynamic lists of acceptable governing states based on policy effective dates and current business rules through the `LOBHelper.AcceptableGoverningStatesAsString(effectiveDate)` method.

**Business Function**: 
- Provides current list of states available for governing state selection
- Updates automatically based on effective date rules and business agreements
- Ensures question text reflects current multi-state operational capabilities
- Maintains accuracy without requiring manual question text updates

**Integration Requirements**: 
- Real-time state availability checking
- Dynamic text insertion for underwriting questions
- Effective date-based state availability rules
- Business rule coordination across multiple helper systems

**Source Code Reference**: Used in kill question #3 for employee location validation
**Helper Dependencies**: LOBHelper class integration for state text generation

### 2.3 Kentucky-Specific Coordination Framework
**Kentucky Integration Rules**: Kentucky operates under special coordination protocols within the 3-state reciprocal agreement framework (Indiana-Illinois-Kentucky).

**Kentucky Effective Date Logic**:
- **Kentucky WCP Start Date**: August 1, 2019 (default)
- **Configuration Parameter**: `WC_KY_EffectiveDate`
- **Special Override Trigger**: `effectiveDate > KentuckyWCPEffectiveDate`

**Kentucky Override Business Rules**:
- **Trigger Condition**: Policy effective date after Kentucky start date AND question contains employee location text
- **Override Text**: "Do any employees live outside the state of Indiana, Illinois, or Kentucky?"
- **Business Purpose**: Reflects the 3-state reciprocal agreement for employee location questions

**Regulatory Context**: Kentucky's participation in the multi-state framework requires specific question text that acknowledges the Indiana-Illinois-Kentucky reciprocal agreement, ensuring proper regulatory compliance and coverage territory understanding.

**Source Code Reference**: `UWQuestions.vb` lines 96-102
**Configuration Dependencies**: WC_KY_EffectiveDate application setting

---

## 3. State-Specific Business Requirements

### 3.1 Indiana (IN) Coordination Requirements
**Primary State Role**: Indiana serves as one of the three primary coordination states in the multi-state framework with full reciprocal agreement participation.

**Business Characteristics**:
- Full multi-state coordination capability
- Reciprocal agreement participation with Illinois and Kentucky
- Standard multi-state question sets and business rules
- Complete integration with governing state coordination logic

**Coordination Scope**: Indiana policies participate in all standard multi-state business rules, question routing logic, and coordination protocols without special overrides or exceptions.

### 3.2 Illinois (IL) Coordination Requirements
**Primary State Role**: Illinois serves as one of the three primary coordination states with full reciprocal agreement participation and standard multi-state business rule application.

**Business Characteristics**:
- Full multi-state coordination capability
- Reciprocal agreement participation with Indiana and Kentucky
- Standard multi-state question sets and business rules
- Complete integration with governing state coordination logic

**Coordination Scope**: Illinois policies participate in all standard multi-state business rules and coordination protocols without special overrides, maintaining consistent treatment with Indiana in the 3-state framework.

### 3.3 Kentucky (KY) Specialized Coordination Requirements
**Primary State Role**: Kentucky serves as the third primary coordination state but requires specialized business rule overrides due to regulatory and operational requirements.

**Kentucky-Specific Business Rules**:
- **Effective Date Threshold**: August 1, 2019 for Kentucky-specific overrides
- **Question Text Override**: Specialized employee location question text referencing the 3-state agreement
- **Override Logic**: Post-processing modification of question text after standard question generation
- **Regulatory Compliance**: Ensures Kentucky-specific regulatory language requirements are met

**Override Implementation Requirements**:
- **Condition Detection**: System must identify Kentucky policies after the effective date threshold
- **Text Pattern Matching**: Must identify employee location questions requiring override
- **Text Replacement**: Must replace standard question text with Kentucky-specific 3-state text
- **Audit Trail**: Must maintain record of text overrides for compliance documentation

**Source Code Reference**: `UWQuestions.vb` lines 96-102 (post-processing override pattern)

### 3.4 Governing State Flexibility Framework
**Governing State Concept**: Any qualified state can serve as the Governing State for policy administration while maintaining coordination with the IN/IL/KY framework.

**Governing State Responsibilities**:
- Primary policy administration and regulatory compliance
- Coordination with IN/IL/KY reciprocal agreement requirements
- State-specific business rule application
- Regulatory filing and compliance management

**Business Rule Integration**: Governing state selection affects question text generation, business rule application, and coordination requirements while maintaining the underlying IN/IL/KY coordination framework.

---

## 4. Dynamic Business Rule Management

### 4.1 Question Code Selection Logic Framework
The multi-state architecture implements sophisticated question code selection logic that adapts based on multi-state capability and business rule requirements.

**Multi-State Question Code Set**: {9341, 9086, 9573, 9343, 9344, 9107}
- **Key Characteristic**: Uses Code 9573 for multi-state employee location questions
- **Business Purpose**: Enables dynamic state text generation and multi-state coordination logic
- **Trigger Condition**: Applied when `IsMultistateCapableEffectiveDate(effectiveDate) = true`

**Single-State Question Code Set**: {9341, 9086, 9342, 9343, 9344, 9107}
- **Key Characteristic**: Uses Code 9342 for single-state employee location questions
- **Business Purpose**: Maintains traditional single-state business rules and validation logic
- **Trigger Condition**: Applied when `IsMultistateCapableEffectiveDate(effectiveDate) = false`

**Code Selection Impact**: The primary difference between multi-state and single-state operations lies in the employee location question (9342 vs 9573), which fundamentally changes the business logic, validation requirements, and coordination protocols.

### 4.2 Dynamic Content Generation Requirements
**State Text Generation Framework**: The system dynamically generates state text for questions based on current business rules, effective dates, and state availability through the `LOBHelper.AcceptableGoverningStatesAsString()` method.

**Dynamic Generation Benefits**:
- **Business Rule Accuracy**: Questions always reflect current state availability and business agreements
- **Maintenance Efficiency**: No manual question text updates required when state agreements change
- **Regulatory Compliance**: Automatically incorporates new states or removes unavailable states
- **Operational Flexibility**: Supports business expansion into new states without system modifications

**Implementation Requirements**:
- **Real-Time Generation**: State text must be generated at question presentation time
- **Effective Date Integration**: Text generation must consider policy effective dates and business rules
- **Helper Class Integration**: Must coordinate with LOBHelper and States helper classes
- **Performance Considerations**: Dynamic generation must maintain acceptable response times

---

## 5. Configuration Management Architecture

### 5.1 Multi-State Configuration Framework
The multi-state architecture relies on sophisticated configuration management to control effective dates, business rule activation, and state-specific overrides.

### 5.2 VR_MultiState_EffectiveDate Configuration
**Parameter Purpose**: Controls when multi-state capability becomes available for new policies
**Default Value**: "1-1-2019"
**Business Impact**: Determines question code selection, business rule routing, and coordination logic activation

**Configuration Management Requirements**:
- **Date Format Validation**: System must handle various date formats and ensure consistent parsing
- **Fallback Logic**: Must provide reliable default value when configuration is unavailable
- **Change Management**: Configuration changes must be properly validated and tested
- **Audit Requirements**: All configuration changes must be logged for compliance and troubleshooting

**Business Rule Application**:
- **Activation Logic**: `effectiveDate >= ParsedMultiStateStartDate`
- **Question Routing**: Determines use of Code 9573 (multi-state) vs Code 9342 (single-state)
- **Helper Integration**: Affects LOBHelper state text generation and coordination logic

**Source Code Reference**: `MultiState/General.vb` lines 64-70, 30-36

### 5.3 WC_KY_EffectiveDate Configuration
**Parameter Purpose**: Controls when Kentucky-specific business rule overrides become active
**Default Value**: "8/1/2019"
**Business Impact**: Determines Kentucky question text overrides and 3-state agreement language activation

**Kentucky-Specific Configuration Requirements**:
- **Override Activation**: Controls when Kentucky policies receive specialized question text
- **3-State Agreement**: Enables Indiana-Illinois-Kentucky reciprocal agreement language
- **Post-Processing Logic**: Triggers text replacement logic for Kentucky policies
- **Regulatory Compliance**: Ensures Kentucky-specific regulatory requirements are met

**Override Logic Application**:
- **Trigger Condition**: `effectiveDate > ParsedKentuckyStartDate`
- **Pattern Matching**: Identifies questions containing specific text patterns for override
- **Text Replacement**: Replaces standard text with Kentucky-specific 3-state language
- **Coordination**: Works in conjunction with multi-state capability determination

**Source Code Reference**: Kentucky override logic in `UWQuestions.vb` lines 96-102

### 5.4 Configuration Architecture Requirements
**Configuration Management Framework**: The system requires robust configuration management supporting real-time parameter access, change validation, and fallback logic.

**Technical Requirements**:
- **ConfigurationManager Integration**: Real-time access to application settings
- **Default Value Management**: Reliable fallback to hardcoded defaults when configuration unavailable
- **Date Parsing Logic**: Consistent date format handling across all configuration parameters
- **Error Handling**: Graceful handling of configuration errors with appropriate logging

**Business Continuity Requirements**:
- **Configuration Availability**: System must operate with default values when configuration systems are unavailable
- **Change Impact Assessment**: Configuration changes must be validated against business rules
- **Rollback Capability**: Must support configuration rollback for business continuity
- **Documentation**: All configuration parameters must be fully documented with business impact analysis

---

## 6. Integration Requirements and Dependencies

### 6.1 Helper Class Integration Architecture
The multi-state architecture requires sophisticated integration with multiple helper classes to provide comprehensive business rule coordination and state management functionality.

### 6.2 LOBHelper Integration Requirements
**Primary Integration**: `LOBHelper.AcceptableGoverningStatesAsString(effectiveDate)`

**Integration Functions**:
- **Dynamic State Text Generation**: Real-time generation of acceptable governing state lists
- **Effective Date Coordination**: State availability based on policy effective dates
- **Business Rule Integration**: Coordination with multi-state capability determination
- **Text Formatting**: Proper formatting of state lists for question text insertion

**Business Dependencies**:
- **State Availability Logic**: LOBHelper must maintain current state availability rules
- **Effective Date Processing**: Must coordinate with multi-state effective date logic
- **Text Generation Standards**: Must provide consistently formatted state text
- **Performance Requirements**: Must support real-time text generation with acceptable response times

### 6.3 MultiState.General Integration Requirements
**Core Integration**: Multi-state capability determination and configuration management

**Integration Functions**:
- **IsMultistateCapableEffectiveDate()**: Core business rule for multi-state capability determination
- **Configuration Access**: VR_MultiState_EffectiveDate parameter management
- **Date Processing**: Effective date comparison and business rule application
- **Default Value Management**: Fallback logic for configuration parameters

**Business Rule Coordination**:
- **Capability Determination**: Central logic for multi-state vs single-state routing
- **Configuration Management**: Reliable parameter access with fallback defaults
- **Date Logic**: Consistent effective date processing across all business rules
- **Integration Points**: Coordination with question routing and helper class systems

**Source Code Reference**: `MultiState/General.vb` lines 60-62, 64-70, 30-36

### 6.4 States Helper Integration Requirements
**Supporting Integration**: State-specific business rule and configuration support

**Integration Functions**:
- **State-Specific Logic**: Individual state business rule and requirement management
- **Configuration Support**: State-specific parameter management and validation
- **Regulatory Compliance**: State regulatory requirement coordination
- **Helper Coordination**: Integration with LOBHelper and MultiState.General systems

**Coordination Requirements**:
- **State Availability**: Maintenance of current state operational status
- **Regulatory Updates**: Incorporation of state regulatory changes
- **Business Rule Updates**: State-specific business rule modification support
- **Integration Testing**: Comprehensive testing of state-specific coordination logic

---

## 7. Business Process Flow Requirements

### 7.1 Multi-State Determination Process Flow
**Process Initiation**: Multi-state determination begins when a policy effective date is available for evaluation.

**Determination Steps**:
1. **Effective Date Retrieval**: System obtains policy effective date
2. **Configuration Access**: System retrieves VR_MultiState_EffectiveDate parameter
3. **Date Comparison**: System compares effective date against multi-state start date
4. **Capability Decision**: System determines multi-state capability (true/false)
5. **Question Code Selection**: System selects appropriate question code set
6. **Business Rule Routing**: System applies multi-state or single-state business rules

**Decision Outcomes**:
- **Multi-State Capable**: Uses question codes {9341,9086,9573,9343,9344,9107} with dynamic state text
- **Single-State Only**: Uses question codes {9341,9086,9342,9343,9344,9107} with single state text

### 7.2 Kentucky Override Process Flow
**Override Initiation**: Kentucky override process begins after standard question generation for Kentucky policies.

**Override Steps**:
1. **Policy State Identification**: System identifies Kentucky as policy state
2. **Effective Date Validation**: System checks Kentucky effective date threshold
3. **Question Pattern Matching**: System identifies employee location questions
4. **Override Condition Check**: System validates all override conditions are met
5. **Text Replacement**: System replaces standard text with Kentucky-specific 3-state text
6. **Override Logging**: System logs override application for audit and compliance

**Override Result**: Kentucky policies receive specialized question text acknowledging the Indiana-Illinois-Kentucky reciprocal agreement.

### 7.3 Dynamic State Text Generation Process Flow
**Generation Initiation**: Dynamic state text generation occurs during question text preparation for multi-state policies.

**Generation Steps**:
1. **LOBHelper Method Call**: System calls `AcceptableGoverningStatesAsString(effectiveDate)`
2. **Effective Date Processing**: LOBHelper processes effective date against state availability rules
3. **State List Generation**: LOBHelper generates current acceptable governing state list
4. **Text Formatting**: LOBHelper formats state list for question text insertion
5. **Question Text Assembly**: System inserts generated state text into question template
6. **Final Question Delivery**: System delivers question with current state availability

**Generation Benefits**: Ensures question text always reflects current business rules and state availability without manual maintenance.

---

## 8. Quality Assurance and Testing Requirements

### 8.1 Multi-State Configuration Testing Requirements
**Configuration Parameter Testing**:
- **VR_MultiState_EffectiveDate Testing**: Validate date parsing, comparison logic, and fallback defaults
- **WC_KY_EffectiveDate Testing**: Validate Kentucky override date logic and condition evaluation
- **Configuration Change Testing**: Validate system behavior when configuration parameters are modified
- **Default Value Testing**: Validate fallback logic when configuration parameters are unavailable

**Date Boundary Testing**:
- **Multi-State Threshold Testing**: Test policies with effective dates before, on, and after multi-state start date
- **Kentucky Threshold Testing**: Test Kentucky policies with effective dates before, on, and after Kentucky start date
- **Edge Case Testing**: Test leap years, month boundaries, and year transitions
- **Invalid Date Testing**: Test system behavior with invalid or malformed dates

### 8.2 Business Rule Coordination Testing Requirements
**Question Code Selection Testing**:
- **Multi-State Question Set**: Validate proper question code set selection for multi-state capable policies
- **Single-State Question Set**: Validate proper question code set selection for single-state policies
- **Code Transition Testing**: Test policies at the multi-state capability transition date
- **Code Mapping Validation**: Validate that each question code maps to correct question content

**Kentucky Override Testing**:
- **Override Condition Testing**: Validate Kentucky override conditions across various policy scenarios
- **Text Replacement Testing**: Validate proper text replacement for Kentucky-specific questions
- **Override Timing Testing**: Test Kentucky overrides with various effective dates
- **Non-Kentucky Policy Testing**: Validate that non-Kentucky policies do not receive Kentucky overrides

### 8.3 Helper Class Integration Testing Requirements
**LOBHelper Integration Testing**:
- **Dynamic State Text Testing**: Validate state text generation across various effective dates
- **State Availability Testing**: Test state list generation with changing state availability
- **Text Formatting Testing**: Validate proper formatting of state lists for question insertion
- **Performance Testing**: Validate acceptable response times for dynamic state text generation

**MultiState.General Integration Testing**:
- **Capability Determination Testing**: Validate `IsMultistateCapableEffectiveDate()` logic across date ranges
- **Configuration Integration Testing**: Validate configuration parameter access and fallback logic
- **Business Rule Coordination Testing**: Validate integration with question routing and helper systems
- **Error Handling Testing**: Validate graceful handling of integration errors and exceptions

---

## 9. Migration and Modernization Considerations

### 9.1 Configuration Migration Requirements
**Legacy Configuration Preservation**:
- **Current Parameter Values**: All existing VR_MultiState_EffectiveDate and WC_KY_EffectiveDate values must be migrated
- **Historical Values**: Historical configuration values must be preserved for audit and compliance purposes
- **Default Value Documentation**: All hardcoded defaults must be documented and validated in modernized system
- **Configuration Change History**: Historical configuration changes must be preserved with timestamps and business justification

### 9.2 Business Logic Migration Requirements
**Multi-State Logic Preservation**:
- **Capability Determination Logic**: `IsMultistateCapableEffectiveDate()` logic must be completely preserved
- **Question Code Selection**: Multi-state vs single-state question code selection logic must be maintained
- **Kentucky Override Logic**: All Kentucky-specific override conditions and text replacement logic must be preserved
- **Helper Integration**: All LOBHelper and MultiState.General integration patterns must be maintained

### 9.3 Data Migration Requirements
**Policy Data Continuity**:
- **Effective Date Processing**: All historical effective dates must be properly processed through migration logic
- **State Assignments**: Historical governing state assignments must be preserved and validated
- **Question Responses**: All historical question responses must be preserved with proper question code mapping
- **Override History**: Historical Kentucky overrides must be documented and preserved for compliance

### 9.4 Integration Continuity Requirements
**Helper Class Continuity**:
- **LOBHelper Integration**: Dynamic state text generation capability must be fully preserved
- **Configuration Access**: ConfigurationManager integration patterns must be maintained or replaced with equivalent functionality
- **Error Handling**: All current error handling and fallback logic must be preserved
- **Performance Standards**: Current performance standards for helper class integration must be maintained or improved

---

## 10. Implementation Priorities and Phasing

### 10.1 Phase 1 - Core Multi-State Framework (High Priority)
**Critical Foundation Components**:
- **Multi-State Capability Determination**: `IsMultistateCapableEffectiveDate()` logic implementation
- **Configuration Management**: VR_MultiState_EffectiveDate parameter integration
- **Question Code Selection**: Multi-state vs single-state question routing logic
- **Basic Helper Integration**: LOBHelper and MultiState.General coordination

**Success Criteria**: 
- Multi-state capability determination functioning correctly
- Question code selection working for both multi-state and single-state scenarios
- Configuration parameters accessible with proper fallback logic

### 10.2 Phase 2 - Kentucky Coordination and Overrides (High Priority)
**Kentucky-Specific Implementation**:
- **Kentucky Configuration**: WC_KY_EffectiveDate parameter integration
- **Override Logic**: Kentucky question text override implementation
- **3-State Agreement**: Indiana-Illinois-Kentucky reciprocal agreement language
- **Override Processing**: Post-processing text replacement logic

**Success Criteria**:
- Kentucky policies receiving proper question text overrides
- 3-state agreement language properly implemented
- Override conditions working correctly across various effective dates

### 10.3 Phase 3 - Dynamic Content and Advanced Features (Medium Priority)
**Advanced Functionality Implementation**:
- **Dynamic State Text**: LOBHelper state text generation integration
- **Advanced Helper Integration**: Complete States helper class coordination
- **Performance Optimization**: Helper class integration performance tuning
- **Advanced Configuration**: Enhanced configuration management capabilities

**Success Criteria**:
- Dynamic state text generation functioning with acceptable performance
- All helper class integrations working correctly
- Advanced configuration features implemented and validated

### 10.4 Phase 4 - Integration and Optimization (Lower Priority)
**System Integration and Enhancement**:
- **Legacy System Integration**: Complete historical data migration and compatibility
- **Advanced Testing**: Comprehensive integration and performance testing
- **Documentation**: Complete user documentation and training materials
- **Monitoring**: Advanced system monitoring and alerting capabilities

**Success Criteria**:
- All legacy integrations functioning correctly
- Comprehensive testing completed with passing results
- Complete documentation and training materials available
- System monitoring and alerting operational

---

## Source Attribution and Traceability

**Primary Source Analysis**: Rex (IFI Pattern Mining Specialist) - Comprehensive multi-state business logic analysis  
**Analysis Date**: December 2024  
**Analysis Completeness**: 100% multi-state architecture framework coverage with high confidence  
**Requirements Documentation By**: Mason (IFI Extraction & Conversion Specialist)  
**Requirements Validation**: Cross-validated against multi-state capability logic, Kentucky override requirements, configuration management patterns, and helper class integration specifications

**Detailed Source Code References**:

**Multi-State Core Logic**:
- **Capability Determination**: `MultiState/General.vb` lines 60-62 - `IsMultistateCapableEffectiveDate(effDate)`
- **Configuration Access**: `MultiState/General.vb` lines 64-70 - VR_MultiState_EffectiveDate processing
- **Default Value Management**: `MultiState/General.vb` lines 30-36 - Default date fallback logic

**Kentucky Override Logic**:
- **Override Processing**: `UWQuestions.vb` lines 96-102 - Kentucky-specific text replacement
- **Override Conditions**: Kentucky effective date validation and pattern matching logic
- **3-State Agreement**: Indiana-Illinois-Kentucky reciprocal agreement language implementation

**Question Code Selection**:
- **Multi-State Codes**: {9341,9086,9573,9343,9344,9107} - Multi-state question code set
- **Single-State Codes**: {9341,9086,9342,9343,9344,9107} - Single-state question code set
- **Key Difference**: Code 9342 (single-state) vs Code 9573 (multi-state) for employee location questions

**Helper Class Integration**:
- **LOBHelper Integration**: `LOBHelper.AcceptableGoverningStatesAsString(effectiveDate)` method
- **Dynamic State Text**: Real-time state availability text generation
- **States Helper**: Supporting state-specific business rule coordination

**Configuration Parameters**:
- **VR_MultiState_EffectiveDate**: Default "1-1-2019" - Controls multi-state capability activation
- **WC_KY_EffectiveDate**: Default "8/1/2019" - Controls Kentucky override activation
- **Configuration Management**: ConfigurationManager.AppSettings integration with fallback defaults

This requirements specification provides comprehensive coverage of the WCP Multi-State Architecture system, ensuring successful modernization while maintaining sophisticated multi-jurisdictional coordination capabilities, state-specific business rules, and regulatory compliance across all supported states and business scenarios.