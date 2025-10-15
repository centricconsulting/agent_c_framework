Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' object used to store policy-level lob-specific information (that applies to governing state) for a quote; also includes properties that were previously on QuickQuote only
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()>
    Public Class QuickQuoteLobPolicyLevelInfoExtended_AppliedToGoverningState 'added 8/17/2018
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        'PolicyLevel
        'Private _HasEnhancementEndorsement As Boolean 'BusinessMasterEnhancement in QuickQuoteObject
        'Private _EnhancementEndorsementQuotedPremium As String 'BusinessMasterEnhancement in QuickQuoteObject
        'Private _Has_PackageGL_EnhancementEndorsement As Boolean
        'Private _PackageGL_EnhancementEndorsementQuotedPremium As String
        'Private _Has_PackageCPR_EnhancementEndorsement As Boolean
        'Private _PackageCPR_EnhancementEndorsementQuotedPremium As String
        'Private _AdditionalInsuredsCount As Integer
        'Private _AdditionalInsuredsCheckboxBOP As List(Of QuickQuoteAdditionalInsured)
        'Private _HasAdditionalInsuredsCheckboxBOP As Boolean
        'Private _AdditionalInsuredsManualCharge As String
        'Private _AdditionalInsuredsQuotedPremium As String
        'Private _AdditionalInsureds As Generic.List(Of QuickQuoteAdditionalInsured)
        'Private _AdditionalInsuredsBackup As List(Of QuickQuoteAdditionalInsured)
        'Private _EmployeeBenefitsLiabilityText As String
        'Private _EmployeeBenefitsLiabilityOccurrenceLimit As String
        'Private _EmployeeBenefitsLiabilityOccurrenceLimitId As String
        'Private _EmployeeBenefitsLiabilityQuotedPremium As String
        'Private _EmployeeBenefitsLiabilityRetroactiveDate As String
        'Private _EmployeeBenefitsLiabilityAggregateLimit As String
        'Private _EmployeeBenefitsLiabilityDeductible As String
        Private _ContractorsEquipmentScheduledCoverages As List(Of QuickQuoteContractorsEquipmentScheduledCoverage)
        Private _ContractorsEquipmentScheduleCoinsuranceTypeId As String 'may need static data placeholder; may be defaulted as there's just one value in dropdown (1 = per 100); note: previous comment is likely talking about CoverageBasisTypeId
        Private _ContractorsEquipmentScheduleDeductibleId As String 'may need static data placeholder
        Private _ContractorsEquipmentScheduleRate As String
        Private _ContractorsEquipmentScheduleQuotedPremium As String
        Private _ContractorsEquipmentLeasedRentedFromOthersLimit As String
        Private _ContractorsEquipmentLeasedRentedFromOthersCoverageLimitId As String 'may not need this; unable to see where value in xml came from when looking at the UI
        Private _ContractorsEquipmentLeasedRentedFromOthersRate As String
        Private _ContractorsEquipmentLeasedRentedFromOthersQuotedPremium As String
        Private _ContractorsEquipmentRentalReimbursementLimit As String
        Private _ContractorsEquipmentRentalReimbursementRate As String
        Private _ContractorsEquipmentRentalReimbursementQuotedPremium As String
        Private _ContractorsEquipmentSmallToolsEndorsementPerOccurrenceLimit As String
        Private _ContractorsEquipmentSmallToolsEndorsementPerOccurrenceRate As String
        Private _ContractorsEquipmentSmallToolsEndorsementPerOccurrenceDeductibleId As String 'static data
        Private _ContractorsEquipmentSmallToolsEndorsementPerOccurrenceQuotedPremium As String
        Private _ContractorsEquipmentSmallToolsEndorsementPerToolLimit As String
        Private _ContractorsEquipmentSmallToolsEndorsementPerToolQuotedPremium As String
        Private _SmallToolsLimit As String
        Private _SmallToolsRate As String
        Private _SmallToolsDeductibleId As String 'static data
        Private _SmallToolsAdditionalInterests As List(Of QuickQuoteAdditionalInterest)
        Private _SmallToolsCanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean
        Private _SmallToolsQuotedPremium As String
        Private _SmallToolsIsEmployeeTools As Boolean 'small tools floater
        Private _SmallToolsIsToolsLeasedOrRented As Boolean 'small tools floater
        Private _SmallToolsAnyOneLossCatastropheLimit As String
        Private _SmallToolsAnyOneLossCatastropheQuotedPremium As String
        Private _InstallationScheduledLocations As List(Of QuickQuoteInstallationScheduledLocation)
        Private _InstallationQuotedPremium As String
        Private _InstallationAdditionalInterests As List(Of QuickQuoteAdditionalInterest)
        Private _InstallationCanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean
        Private _InstallationBlanketLimit As String
        Private _InstallationBlanketCoinsuranceTypeId As String 'may need static data placeholder; may be defaulted as there's just one value in dropdown (1 = per 100); note: previous comment is likely talking about CoverageBasisTypeId
        Private _InstallationBlanketDeductibleId As String 'may need static data placeholder
        Private _InstallationBlanketRate As String
        Private _InstallationBlanketQuotedPremium As String
        Private _InstallationBlanketAnyOneLossCatastropheLimit As String
        Private _InstallationBlanketAnyOneLossCatastropheQuotedPremium As String
        Private _InstallationAdditionalDebrisRemovalExpenseLimit As String 'cov also has CoverageBasisTypeId set to 1
        Private _InstallationAdditionalDebrisRemovalExpenseQuotedPremium As String
        Private _InstallationStorageLocationsLimit As String 'cov also has CoverageBasisTypeId set to 1
        Private _InstallationStorageLocationsQuotedPremium As String
        Private _InstallationTransitLimit As String 'cov also has CoverageBasisTypeId set to 1
        Private _InstallationTransitQuotedPremium As String
        Private _InstallationTestingLimit As String 'cov also has CoverageBasisTypeId set to 1
        Private _InstallationTestingQuotedPremium As String
        Private _InstallationSewerBackupLimit As String 'cov also has CoverageBasisTypeId set to 1
        Private _InstallationSewerBackupDeductible As String
        Private _InstallationSewerBackupQuotedPremium As String
        Private _InstallationSewerBackupCatastropheLimit As String 'cov also has CoverageBasisTypeId set to 1
        Private _InstallationSewerBackupCatastropheQuotedPremium As String
        Private _InstallationEarthquakeLimit As String 'cov also has CoverageBasisTypeId set to 1
        Private _InstallationEarthquakeDeductible As String
        Private _InstallationEarthquakeQuotedPremium As String
        Private _InstallationEarthquakeCatastropheLimit As String 'cov also has CoverageBasisTypeId set to 1
        Private _InstallationEarthquakeCatastropheQuotedPremium As String
        Private _BusinessPersonalPropertyLimit As String 'cov also has CoverageBasisTypeId set to 1; shown in UI Installation Coverage Extensions section, but may not be specific to Installation
        Private _BusinessPersonalPropertyQuotedPremium As String
        Private _ScheduledPropertyItems As List(Of QuickQuoteScheduledPropertyItem) 'cov also has CoverageBasisTypeId set to 1
        Private _ScheduledPropertyAdditionalInterests As List(Of QuickQuoteAdditionalInterest) 'cov also has CoverageBasisTypeId set to 1
        Private _ScheduledPropertyCanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean
        Private _ScheduledPropertyCoinsuranceTypeId As String 'may need static data placeholder
        Private _ScheduledPropertyDeductibleId As String 'may need static data placeholder
        Private _ScheduledPropertyRate As String
        Private _ScheduledPropertyNamedPerils As Boolean
        Private _ScheduledPropertyQuotedPremium As String
        '12/18/2018 - moved computer props to AlliedToIndividualState object since it's specific to Location Buildings (and their parent state quotes)
        'Private _ComputerCoinsuranceTypeId As String 'cov also has CoverageBasisTypeId set to 1
        'Private _ComputerExcludeEarthquake As Boolean
        'Private _ComputerValuationMethodTypeId As String 'static data
        'Private _ComputerAdditionalInterests As List(Of QuickQuoteAdditionalInterest)
        'Private _ComputerCanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean
        'Private _ComputerQuotedPremium As String
        'Private _ComputerAllPerilsDeductibleId As String 'cov also has CoverageBasisTypeId set to 1; may also need boolean prop for hasCoverage; static data
        'Private _ComputerAllPerilsQuotedPremium As String
        'Private _ComputerEarthquakeVolcanicEruptionDeductible As String 'cov also has CoverageBasisTypeId set to 1; example also has ApplyToWrittenPremiuim set to true; may also need boolean prop for hasCoverage
        'Private _ComputerEarthquakeVolcanicEruptionQuotedPremium As String
        'Private _ComputerMechanicalBreakdownDeductible As String 'cov also has CoverageBasisTypeId set to 1; example also has ApplyToWrittenPremiuim set to true; may also need boolean prop for hasCoverage
        'Private _ComputerMechanicalBreakdownQuotedPremium As String
        Private _BuildersRiskDeductibleId As String 'cov also has CoverageBasisTypeId set to 1; static data
        Private _BuildersRiskRate As String
        Private _BuildersRiskAdditionalInterests As List(Of QuickQuoteAdditionalInterest)
        Private _BuildersRiskCanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean
        Private _BuildersRiskQuotedPremium As String
        Private _BuildersRiskScheduledLocations As List(Of QuickQuoteBuildersRiskScheduledLocation) 'ScheduledCoverage cov also has CoverageBasisTypeId set to 1
        Private _BuildersRiskScheduleStorageLocationsLimit As String
        Private _BuildersRiskScheduleStorageLocationsQuotedPremium As String
        Private _BuildersRiskScheduleTransitLimit As String
        Private _BuildersRiskScheduleTransitQuotedPremium As String
        Private _BuildersRiskScheduleTestingLimit As String
        Private _BuildersRiskScheduleTestingQuotedPremium As String
        '12/18/2018 - moved fineArts props to AlliedToIndividualState object since it's specific to Location Buildings (and their parent state quotes)
        'Private _FineArtsDeductibleCategoryTypeId As String 'static data; note: cov also has CoverageBasisTypeId set to 1
        'Private _FineArtsRate As String
        'Private _FineArtsDeductibleId As String 'static data
        'Private _FineArtsQuotedPremium As String
        'Private _FineArtsAdditionalInterests As List(Of QuickQuoteAdditionalInterest)
        'Private _FineArtsCanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean
        'Private _FineArtsBreakageMarringOrScratching As Boolean 'renamed 3/17/2015 from _HasFineArtsBreakageMarringOrScratching; note: cov also has CoverageBasisTypeId set to 1
        'Private _FineArtsBreakageMarringOrScratchingQuotedPremium As String
        Private _OwnersCargoAnyOneOwnedVehicleLimit As String 'note: cov also has CoverageBasisTypeId set to 1
        Private _OwnersCargoAnyOneOwnedVehicleDeductibleId As String 'static data
        Private _OwnersCargoAnyOneOwnedVehicleRate As String
        Private _OwnersCargoAnyOneOwnedVehicleDescription As String
        Private _OwnersCargoAnyOneOwnedVehicleAdditionalInterests As List(Of QuickQuoteAdditionalInterest)
        Private _OwnersCargoAnyOneOwnedVehicleCanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean
        Private _OwnersCargoAnyOneOwnedVehicleLoadingUnloading As Boolean 'CoverageDetail
        Private _OwnersCargoAnyOneOwnedVehicleNamedPerils As Boolean 'CoverageDetail
        Private _OwnersCargoAnyOneOwnedVehicleQuotedPremium As String
        Private _OwnersCargoCatastropheLimit As String 'note: cov also has CoverageBasisTypeId set to 1
        Private _OwnersCargoCatastropheQuotedPremium As String
        Private _TransportationCatastropheLimit As String 'note: cov also has CoverageBasisTypeId set to 1
        Private _TransportationCatastropheDeductibleId As String 'static data
        Private _TransportationCatastropheDescription As String
        Private _TransportationCatastropheAdditionalInterests As List(Of QuickQuoteAdditionalInterest)
        Private _TransportationCatastropheCanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean
        Private _TransportationCatastropheLoadingUnloading As Boolean 'CoverageDetail
        Private _TransportationCatastropheNamedPerils As Boolean 'CoverageDetail
        Private _TransportationCatastropheQuotedPremium As String
        Private _TransportationAnyOneOwnedVehicleLimit As String 'note: cov also has CoverageBasisTypeId set to 1
        Private _TransportationAnyOneOwnedVehicleNumberOfVehicles As String 'CoverageDetail
        Private _TransportationAnyOneOwnedVehicleRate As String
        Private _TransportationAnyOneOwnedVehicleQuotedPremium As String
        Private _MotorTruckCargoScheduledVehicles As List(Of QuickQuoteScheduledVehicle) 'note: cov also has CoverageBasisTypeId set to 1
        Private _MotorTruckCargoScheduledVehicleAdditionalInterests As List(Of QuickQuoteAdditionalInterest) 'note: cov also has CoverageBasisTypeId set to 1
        Private _MotorTruckCargoScheduledVehicleCanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean
        Private _MotorTruckCargoScheduledVehicleLoadingUnloading As Boolean 'CoverageDetail
        Private _MotorTruckCargoScheduledVehicleNamedPerils As Boolean 'CoverageDetail
        Private _MotorTruckCargoScheduledVehicleOperatingRadius As String 'CoverageDetail
        Private _MotorTruckCargoScheduledVehicleRate As String 'CoverageDetail
        Private _MotorTruckCargoScheduledVehicleDeductibleId As String 'static data
        Private _MotorTruckCargoScheduledVehicleDescription As String
        Private _MotorTruckCargoScheduledVehicleQuotedPremium As String
        Private _MotorTruckCargoScheduledVehicleCatastropheLimit As String 'note: cov also has CoverageBasisTypeId set to 1
        Private _MotorTruckCargoScheduledVehicleCatastropheQuotedPremium As String
        'Added 7/8/2025
        Private _MotorTruckCargoUnScheduledVehicleAdditionalInterests As List(Of QuickQuoteAdditionalInterest) 'note: cov also has CoverageBasisTypeId set to 1
        Private _MotorTruckCargoUnScheduledVehicleCanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean
        Private _MotorTruckCargoUnScheduledVehicleLoadingUnloading As Boolean 'CoverageDetail
        Private _MotorTruckCargoUnScheduledVehicleNamedPerils As Boolean 'CoverageDetail
        Private _MotorTruckCargoUnScheduledVehicleOperatingRadius As String 'CoverageDetail
        Private _MotorTruckCargoUnScheduledVehicleRate As String 'CoverageDetail
        Private _MotorTruckCargoUnScheduledVehicleDeductibleId As String 'static data
        Private _MotorTruckCargoUnScheduledVehicleDescription As String
        Private _MotorTruckCargoUnScheduledVehicleQuotedPremium As String
        Private _MotorTruckCargoUnScheduledVehicleCatastropheLimit As String 'note: cov also has CoverageBasisTypeId set to 1
        Private _MotorTruckCargoUnScheduledAnyVehicleLimit As String
        Private _MotorTruckCargoUnScheduledNumberOfVehicles As String
        Private _MotorTruckCargoUnScheduledVehicleCatastropheQuotedPremium As String

        ''12/18/2018 - moved signs props to AlliedToIndividualState object since it's specific to Location Buildings (and their parent state quotes)
        'Private _SignsAdditionalInterests As List(Of QuickQuoteAdditionalInterest) 'note: cov also has CoverageBasisTypeId set to 1
        'Private _SignsCanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean
        'Private _SignsMaximumDeductible As String 'CoverageDetail
        'Private _SignsMinimumDeductible As String 'CoverageDetail
        'Private _SignsValuationMethodTypeId As String 'CoverageDetail; static data
        'Private _SignsDeductibleId As String 'static data
        'Private _SignsQuotedPremium As String
        'Private _SignsAnyOneLossCatastropheLimit As String 'note: cov also has CoverageBasisTypeId set to 1
        'Private _SignsAnyOneLossCatastropheQuotedPremium As String
        Private _ContractorsEquipmentCatastropheLimit As String
        Private _ContractorsEquipmentCatastropheQuotedPremium As String
        Private _EmployeeTheftLimit As String 'note: cov also has CoverageBasisTypeId 1
        Private _EmployeeTheftDeductibleId As String 'static data
        Private _EmployeeTheftNumberOfRatableEmployees As String 'CoverageDetail
        Private _EmployeeTheftNumberOfAdditionalPremises As String 'CoverageDetail
        Private _EmployeeTheftFaithfulPerformanceOfDutyTypeId As String 'CoverageDetail; static data
        Private _EmployeeTheftScheduledEmployeeBenefitPlans As List(Of String)
        Private _EmployeeTheftIncludedPersonsOrClasses As List(Of String)
        Private _EmployeeTheftIncludedChairpersonsAndSpecifiedCommitteeMembers As List(Of String)
        Private _EmployeeTheftScheduledPartners As List(Of String)
        Private _EmployeeTheftScheduledLLCMembers As List(Of String)
        Private _EmployeeTheftScheduledNonCompensatedOfficers As List(Of String)
        Private _EmployeeTheftExcludedPersonsOrClasses As List(Of String)
        Private _EmployeeTheftQuotedPremium As String
        Private _InsidePremisesTheftOfMoneyAndSecuritiesLimit As String 'note: cov also has CoverageBasisTypeId 1
        Private _InsidePremisesTheftOfMoneyAndSecuritiesDeductibleId As String 'static data
        Private _InsidePremisesTheftOfMoneyAndSecuritiesNumberOfPremises As String 'CoverageDetail
        Private _InsidePremisesTheftOfMoneyAndSecuritiesIncludeGuestsProperty As Boolean 'CoverageDetail
        Private _InsidePremisesTheftOfMoneyAndSecuritiesRequireRecordOfChecks As Boolean 'CoverageDetail
        Private _InsidePremisesTheftOfMoneyAndSecuritiesQuotedPremium As String
        Private _OutsideThePremisesLimit As String 'note: cov also has CoverageBasisTypeId 1
        Private _OutsideThePremisesDeductibleId As String 'static data
        Private _OutsideThePremisesNumberOfPremises As String 'CoverageDetail
        Private _OutsideThePremisesIncludeSellingPrice As Boolean 'CoverageDetail
        Private _OutsideThePremisesLimitToRobberyOnly As Boolean 'CoverageDetail
        Private _OutsideThePremisesRequireRecordOfChecks As Boolean 'CoverageDetail
        Private _OutsideThePremisesQuotedPremium As String
        'Private _HasContractorsEnhancement As Boolean
        'Private _ContractorsEnhancementQuotedPremium As String
        'Private _CPP_CPR_ContractorsEnhancementQuotedPremium As String
        'Private _CPP_CGL_ContractorsEnhancementQuotedPremium As String
        'Private _CPP_CIM_ContractorsEnhancementQuotedPremium As String
        'Private _HasManufacturersEnhancement As Boolean
        'Private _ManufacturersEnhancementQuotedPremium As String
        'Private _CPP_CPR_ManufacturersEnhancementQuotedPremium As String
        'Private _CPP_CGL_ManufacturersEnhancementQuotedPremium As String
        'Private _HasAutoPlusEnhancement As Boolean
        'Private _AutoPlusEnhancement_QuotedPremium As String
        Private _ScheduledGolfCourses As List(Of QuickQuoteScheduledGolfCourse)
        Private _ScheduledGolfCartCourses As List(Of QuickQuoteScheduledGolfCartCourse)
        Private _GolfCourseQuotedPremium As String 'covCodeId 21341
        Private _GolfCourseCoverageLimitId As String 'covCodeId 21341
        Private _GolfCourseDeductibleId As String 'covCodeId 21341
        Private _GolfCourseCoinsuranceTypeId As String 'covCodeId 21341
        Private _GolfCourseRate As String 'covCodeId 21341
        Private _GolfCartQuotedPremium As String 'covCodeId 50121
        Private _GolfCartManualLimitAmount As String 'covCodeId 50121
        Private _GolfCartDeductibleId As String 'covCodeId 50121
        Private _GolfCartCoinsuranceTypeId As String 'covCodeId 50121
        Private _GolfCartRate As String 'covCodeId 50121
        Private _GolfCartCatastropheManualLimitAmount As String 'covCodeId 21343
        Private _GolfCartDebrisRemovalCoverageLimitId As String 'covCodeId 80223

        'added 10/15/2018 - moved from AlliedToIndividualState object
        Private _HasInclusionOfSoleProprietorsPartnersOfficersAndOthers As Boolean
        Private _HasWaiverOfSubrogation As Boolean
        Private _WaiverOfSubrogationNumberOfWaivers As Integer
        Private _WaiverOfSubrogationPremium As String
        Private _WaiverOfSubrogationPremiumId As String
        Private _NeedsToUpdateWaiverOfSubrogationPremiumId As Boolean
        Private _InclusionOfSoleProprietorRecords As Generic.List(Of QuickQuoteInclusionOfSoleProprietorRecord)
        Private _WaiverOfSubrogationRecords As Generic.List(Of QuickQuoteWaiverOfSubrogationRecord)
        Private _InclusionOfSoleProprietorRecordsBackup As Generic.List(Of QuickQuoteInclusionOfSoleProprietorRecord)
        Private _WaiverOfSubrogationRecordsBackup As Generic.List(Of QuickQuoteWaiverOfSubrogationRecord)
        Private _WCP_WaiverPremium As String 'covCodeId 10124 CovAddInfo w/ "Waiver Premium" in desc
        'added 10/15/2018 - moved from AllStates object
        Private _BlanketWaiverOfSubrogation As String
        Private _BlanketWaiverOfSubrogationQuotedPremium As String

        Private _QuoteEffectiveDate As String
        Private _QuoteTransactionType As QuickQuoteObject.QuickQuoteTransactionType
        Private _LobType As QuickQuoteObject.QuickQuoteLobType

        Private _BasePolicyLevelInfo As QuickQuoteLobPolicyLevelInfo_AppliedToGoverningState


        'PolicyLevel

        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 286 or 80094 (PPA); property covers Enhancement Endorsement for all LOBs</remarks>
        'Public Property HasEnhancementEndorsement As Boolean
        '    Get
        '        Return _HasEnhancementEndorsement
        '    End Get
        '    Set(value As Boolean)
        '        _HasEnhancementEndorsement = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 286 or 80094 (PPA); property covers Enhancement Endorsement for all LOBs</remarks>
        'Public Property EnhancementEndorsementQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_EnhancementEndorsementQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _EnhancementEndorsementQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_EnhancementEndorsementQuotedPremium)
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 286; specific to the GL package part for CPP</remarks>
        'Public Property Has_PackageGL_EnhancementEndorsement As Boolean
        '    Get
        '        Return _Has_PackageGL_EnhancementEndorsement
        '    End Get
        '    Set(value As Boolean)
        '        _Has_PackageGL_EnhancementEndorsement = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 286; specific to the GL package part for CPP</remarks>
        'Public Property PackageGL_EnhancementEndorsementQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_PackageGL_EnhancementEndorsementQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _PackageGL_EnhancementEndorsementQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_PackageGL_EnhancementEndorsementQuotedPremium)
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 286; specific to the CPR package part for CPP</remarks>
        'Public Property Has_PackageCPR_EnhancementEndorsement As Boolean
        '    Get
        '        Return _Has_PackageCPR_EnhancementEndorsement
        '    End Get
        '    Set(value As Boolean)
        '        _Has_PackageCPR_EnhancementEndorsement = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 286; specific to the CPR package part for CPP</remarks>
        'Public Property PackageCPR_EnhancementEndorsementQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_PackageCPR_EnhancementEndorsementQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _PackageCPR_EnhancementEndorsementQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_PackageCPR_EnhancementEndorsementQuotedPremium)
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverages w/ coveragecode_ids 926, 21018, 501, 21022, 21019, 21023, 21020, 21053, 21054, 21055, 21024, 21025, 21026, 21016, 21017, or 21021</remarks>
        'Public Property AdditionalInsuredsCount As Integer
        '    Get
        '        If QuickQuoteHelperClass.UseComparativeRaterForLob(lob:=_LobType, quoteTransactionType:=QuoteTransactionType) <> QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then 'using <> Yes to also allow on Maybe
        '            Return qqHelper.AdditionalInsuredsCountFromList(_AdditionalInsureds)
        '        Else
        '            Return _AdditionalInsuredsCount
        '        End If
        '    End Get
        '    Set(value As Integer)
        '        If QuickQuoteHelperClass.UseComparativeRaterForLob(lob:=_LobType, quoteTransactionType:=QuoteTransactionType) <> QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then 'using <> Yes to also allow on Maybe
        '            qqHelper.UpdateAdditionalInsuredsListBasedOnCount(value, _AdditionalInsureds, additionalInsuredsBackup:=_AdditionalInsuredsBackup, updateBackupListBeforeRemoving:=True, effDate:=QuoteEffectiveDate, lobType:=_LobType, isAdditionalInsuredCheckboxBOP:=False)
        '        Else
        '            _AdditionalInsuredsCount = value
        '        End If
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverages w/ coveragecode_ids 80371</remarks>
        'Public Property HasAdditionalInsuredsCheckboxBOP As Boolean
        '    Get
        '        Return _HasAdditionalInsuredsCheckboxBOP
        '    End Get
        '    Set(value As Boolean)
        '        _HasAdditionalInsuredsCheckboxBOP = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverages w/ coveragecode_ids 80371</remarks>
        'Public Property AdditionalInsuredsCheckboxBOP As List(Of QuickQuoteAdditionalInsured)
        '    Get
        '        Return _AdditionalInsuredsCheckboxBOP
        '    End Get
        '    Set(value As List(Of QuickQuoteAdditionalInsured))
        '        _AdditionalInsuredsCheckboxBOP = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverages w/ coveragecode_ids 80371</remarks>
        'Public ReadOnly Property AdditionalInsuredsCheckboxBOPPremium As String
        '    Get
        '        Dim prem As Decimal = 0
        '        If AdditionalInsuredsCheckboxBOP IsNot Nothing AndAlso AdditionalInsuredsCheckboxBOP.Count > 0 Then
        '            For Each ai As QuickQuoteAdditionalInsured In AdditionalInsuredsCheckboxBOP
        '                If ai IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(ai.FullTermPremium) AndAlso IsNumeric(ai.FullTermPremium) AndAlso CDec(ai.FullTermPremium > 0) Then
        '                    prem += CDec(ai.FullTermPremium)
        '                End If
        '            Next
        '        End If
        '        Return prem.ToString()
        '    End Get
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverages w/ coveragecode_ids 926, 21018, 501, 21022, 21019, 21023, 21020, 21053, 21054, 21055, 21024, 21025, 21026, 21016, 21017, or 21021</remarks>
        'Public Property AdditionalInsuredsManualCharge As String
        '    Get
        '        If QuickQuoteHelperClass.UseComparativeRaterForLob(lob:=_LobType, quoteTransactionType:=QuoteTransactionType) <> QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then 'using <> Yes to also allow on Maybe
        '            Dim totalAddInsManChrg As String = qqHelper.AdditionalInsuredsTotalManualChargeFromList(_AdditionalInsureds)
        '            If qqHelper.IsZeroAmount(totalAddInsManChrg) = True Then
        '                totalAddInsManChrg = ""
        '            End If
        '            Return totalAddInsManChrg
        '        Else
        '            Return _AdditionalInsuredsManualCharge
        '        End If
        '    End Get
        '    Set(value As String)
        '        If QuickQuoteHelperClass.UseComparativeRaterForLob(lob:=_LobType, quoteTransactionType:=QuoteTransactionType) <> QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then 'using <> Yes to also allow on Maybe
        '            qqHelper.UpdateAdditionalInsuredsListBasedOnManualCharge(value, _AdditionalInsureds, additionalInsuredsBackup:=_AdditionalInsuredsBackup, updateBackupListBeforeRemoving:=True, effDate:=QuoteEffectiveDate, lobType:=_LobType, isAdditionalInsuredCheckboxBOP:=False, maintainOneItemFromOriginalListWhenResetting:=False, maintainFirstOrLastItem:=QuickQuoteHelperClass.FirstOrLast.First, maintainItemsOnUnresolvedDifference:=False, applicableItemToApplyDifferenceTo:=QuickQuoteHelperClass.FirstLastOrAll.All, firstOrLastItemOrderWhenApplyingDifferenceToAll:=QuickQuoteHelperClass.FirstOrLast.First, treatAmountsAsIntegerOverDecimal:=False)
        '        Else
        '            _AdditionalInsuredsManualCharge = value
        '        End If
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverages w/ coveragecode_ids 926, 21018, 501, 21022, 21019, 21023, 21020, 21053, 21054, 21055, 21024, 21025, 21026, 21016, 21017, or 21021</remarks>
        'Public Property AdditionalInsuredsQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_AdditionalInsuredsQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _AdditionalInsuredsQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_AdditionalInsuredsQuotedPremium)
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverages w/ coveragecode_ids 926, 21018, 501, 21022, 21019, 21023, 21020, 21053, 21054, 21055, 21024, 21025, 21026, 21016, 21017, or 21021</remarks>
        'Public Property AdditionalInsureds As Generic.List(Of QuickQuoteAdditionalInsured)
        '    Get
        '        Return _AdditionalInsureds
        '    End Get
        '    Set(value As Generic.List(Of QuickQuoteAdditionalInsured))
        '        _AdditionalInsureds = value
        '    End Set
        'End Property
        'Public ReadOnly Property AdditionalInsuredsBackup As List(Of QuickQuoteAdditionalInsured)
        '    Get
        '        Return _AdditionalInsuredsBackup
        '    End Get
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 185 (this property is specific to NumberOfEmployees)</remarks>
        'Public Property EmployeeBenefitsLiabilityText As String
        '    Get
        '        Return _EmployeeBenefitsLiabilityText
        '    End Get
        '    Set(value As String)
        '        _EmployeeBenefitsLiabilityText = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 185</remarks>
        'Public Property EmployeeBenefitsLiabilityOccurrenceLimit As String
        '    Get
        '        Return _EmployeeBenefitsLiabilityOccurrenceLimit
        '    End Get
        '    Set(value As String)
        '        _EmployeeBenefitsLiabilityOccurrenceLimit = value
        '        Select Case _EmployeeBenefitsLiabilityOccurrenceLimit
        '            Case "N/A"
        '                _EmployeeBenefitsLiabilityOccurrenceLimitId = "0"
        '            Case "300,000"
        '                _EmployeeBenefitsLiabilityOccurrenceLimitId = "33"
        '            Case "500,000"
        '                _EmployeeBenefitsLiabilityOccurrenceLimitId = "34"
        '            Case "1,000,000"
        '                _EmployeeBenefitsLiabilityOccurrenceLimitId = "56"
        '            Case Else
        '                _EmployeeBenefitsLiabilityOccurrenceLimitId = ""
        '        End Select
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 185</remarks>
        'Public Property EmployeeBenefitsLiabilityOccurrenceLimitId As String
        '    Get
        '        Return _EmployeeBenefitsLiabilityOccurrenceLimitId
        '    End Get
        '    Set(value As String)
        '        _EmployeeBenefitsLiabilityOccurrenceLimitId = value
        '        _EmployeeBenefitsLiabilityOccurrenceLimit = ""
        '        If IsNumeric(_EmployeeBenefitsLiabilityOccurrenceLimitId) = True Then
        '            Select Case _EmployeeBenefitsLiabilityOccurrenceLimitId
        '                Case "0"
        '                    _EmployeeBenefitsLiabilityOccurrenceLimit = "N/A"
        '                Case "33"
        '                    _EmployeeBenefitsLiabilityOccurrenceLimit = "300,000"
        '                Case "34"
        '                    _EmployeeBenefitsLiabilityOccurrenceLimit = "500,000"
        '                Case "56"
        '                    _EmployeeBenefitsLiabilityOccurrenceLimit = "1,000,000"
        '            End Select
        '        End If
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 185</remarks>
        'Public Property EmployeeBenefitsLiabilityQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_EmployeeBenefitsLiabilityQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _EmployeeBenefitsLiabilityQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_EmployeeBenefitsLiabilityQuotedPremium)
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 185</remarks>
        'Public Property EmployeeBenefitsLiabilityRetroactiveDate As String
        '    Get
        '        Return _EmployeeBenefitsLiabilityRetroactiveDate
        '    End Get
        '    Set(value As String)
        '        _EmployeeBenefitsLiabilityRetroactiveDate = value
        '        qqHelper.ConvertToShortDate(_EmployeeBenefitsLiabilityRetroactiveDate)
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 185</remarks>
        'Public Property EmployeeBenefitsLiabilityAggregateLimit As String
        '    Get
        '        Return _EmployeeBenefitsLiabilityAggregateLimit
        '    End Get
        '    Set(value As String)
        '        _EmployeeBenefitsLiabilityAggregateLimit = value 'might need limit formatting
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 185</remarks>
        'Public Property EmployeeBenefitsLiabilityDeductible As String
        '    Get
        '        Return _EmployeeBenefitsLiabilityDeductible
        '    End Get
        '    Set(value As String)
        '        _EmployeeBenefitsLiabilityDeductible = value 'might need limit formatting
        '    End Set
        'End Property
        Public Property ContractorsEquipmentScheduledCoverages As List(Of QuickQuoteContractorsEquipmentScheduledCoverage)
            Get
                Return _ContractorsEquipmentScheduledCoverages
            End Get
            Set(value As List(Of QuickQuoteContractorsEquipmentScheduledCoverage))
                _ContractorsEquipmentScheduledCoverages = value
            End Set
        End Property
        Public ReadOnly Property ContractorsEquipmentScheduledCoveragesTotalQuotedPremium As String 'won't use private variable; just ReadOnly prop... should equal sum of Coverage.FullTermPremium for ContractorsEquipmentScheduledCoverages
            Get
                Dim tot As String = "0"
                If _ContractorsEquipmentScheduledCoverages IsNot Nothing AndAlso _ContractorsEquipmentScheduledCoverages.Count > 0 Then
                    For Each sc As QuickQuoteContractorsEquipmentScheduledCoverage In _ContractorsEquipmentScheduledCoverages
                        tot = qqHelper.getSum(tot, sc.QuotedPremium)
                    Next
                End If
                qqHelper.ConvertToQuotedPremiumFormat(tot)

                Return tot
            End Get
        End Property
        Public Property ContractorsEquipmentScheduleCoinsuranceTypeId As String 'may need static data placeholder; may be defaulted as there's just one value in dropdown (1 = per 100)
            Get
                Return _ContractorsEquipmentScheduleCoinsuranceTypeId
            End Get
            Set(value As String)
                _ContractorsEquipmentScheduleCoinsuranceTypeId = value
            End Set
        End Property
        Public Property ContractorsEquipmentScheduleDeductibleId As String 'may need static data placeholder
            Get
                Return _ContractorsEquipmentScheduleDeductibleId
            End Get
            Set(value As String)
                _ContractorsEquipmentScheduleDeductibleId = value
            End Set
        End Property
        Public Property ContractorsEquipmentScheduleRate As String
            Get
                Return _ContractorsEquipmentScheduleRate
            End Get
            Set(value As String)
                _ContractorsEquipmentScheduleRate = value
            End Set
        End Property
        Public Property ContractorsEquipmentScheduleQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_ContractorsEquipmentScheduleQuotedPremium)
            End Get
            Set(value As String)
                _ContractorsEquipmentScheduleQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_ContractorsEquipmentScheduleQuotedPremium)
            End Set
        End Property
        Public ReadOnly Property ContractorsEquipmentScheduleManualLimitAmount As String 'won't use private variable; just ReadOnly prop... should equal sum of Coverage.ManualLimitAmount for ContractorsEquipmentScheduledCoverages
            Get
                Dim tot As String = "0"
                If _ContractorsEquipmentScheduledCoverages IsNot Nothing AndAlso _ContractorsEquipmentScheduledCoverages.Count > 0 Then
                    For Each ce As QuickQuoteContractorsEquipmentScheduledCoverage In _ContractorsEquipmentScheduledCoverages
                        tot = qqHelper.getSum(tot, ce.ManualLimitAmount)
                    Next
                End If
                qqHelper.ConvertToLimitFormat(tot)

                Return tot
            End Get
        End Property
        Public Property ContractorsEquipmentLeasedRentedFromOthersLimit As String
            Get
                Return _ContractorsEquipmentLeasedRentedFromOthersLimit
            End Get
            Set(value As String)
                _ContractorsEquipmentLeasedRentedFromOthersLimit = value
                qqHelper.ConvertToLimitFormat(_ContractorsEquipmentLeasedRentedFromOthersLimit)
            End Set
        End Property
        Public Property ContractorsEquipmentLeasedRentedFromOthersCoverageLimitId As String
            Get
                Return _ContractorsEquipmentLeasedRentedFromOthersCoverageLimitId
            End Get
            Set(value As String)
                _ContractorsEquipmentLeasedRentedFromOthersCoverageLimitId = value
            End Set
        End Property
        Public Property ContractorsEquipmentLeasedRentedFromOthersRate As String
            Get
                Return _ContractorsEquipmentLeasedRentedFromOthersRate
            End Get
            Set(value As String)
                _ContractorsEquipmentLeasedRentedFromOthersRate = value
            End Set
        End Property
        Public Property ContractorsEquipmentLeasedRentedFromOthersQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_ContractorsEquipmentLeasedRentedFromOthersQuotedPremium)
            End Get
            Set(value As String)
                _ContractorsEquipmentLeasedRentedFromOthersQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_ContractorsEquipmentLeasedRentedFromOthersQuotedPremium)
            End Set
        End Property
        Public Property ContractorsEquipmentRentalReimbursementLimit As String
            Get
                Return _ContractorsEquipmentRentalReimbursementLimit
            End Get
            Set(value As String)
                _ContractorsEquipmentRentalReimbursementLimit = value
                qqHelper.ConvertToLimitFormat(_ContractorsEquipmentRentalReimbursementLimit)
            End Set
        End Property
        Public Property ContractorsEquipmentRentalReimbursementRate As String
            Get
                Return _ContractorsEquipmentRentalReimbursementRate
            End Get
            Set(value As String)
                _ContractorsEquipmentRentalReimbursementRate = value
            End Set
        End Property
        Public Property ContractorsEquipmentRentalReimbursementQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_ContractorsEquipmentRentalReimbursementQuotedPremium)
            End Get
            Set(value As String)
                _ContractorsEquipmentRentalReimbursementQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_ContractorsEquipmentRentalReimbursementQuotedPremium)
            End Set
        End Property
        Public Property ContractorsEquipmentSmallToolsEndorsementPerOccurrenceLimit As String
            Get
                Return _ContractorsEquipmentSmallToolsEndorsementPerOccurrenceLimit
            End Get
            Set(value As String)
                _ContractorsEquipmentSmallToolsEndorsementPerOccurrenceLimit = value
                qqHelper.ConvertToLimitFormat(_ContractorsEquipmentSmallToolsEndorsementPerOccurrenceLimit)
            End Set
        End Property
        Public Property ContractorsEquipmentSmallToolsEndorsementPerOccurrenceRate As String
            Get
                Return _ContractorsEquipmentSmallToolsEndorsementPerOccurrenceRate
            End Get
            Set(value As String)
                _ContractorsEquipmentSmallToolsEndorsementPerOccurrenceRate = value
            End Set
        End Property
        Public Property ContractorsEquipmentSmallToolsEndorsementPerOccurrenceDeductibleId As String
            Get
                Return _ContractorsEquipmentSmallToolsEndorsementPerOccurrenceDeductibleId
            End Get
            Set(value As String)
                _ContractorsEquipmentSmallToolsEndorsementPerOccurrenceDeductibleId = value
            End Set
        End Property
        Public Property ContractorsEquipmentSmallToolsEndorsementPerOccurrenceQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_ContractorsEquipmentSmallToolsEndorsementPerOccurrenceQuotedPremium)
            End Get
            Set(value As String)
                _ContractorsEquipmentSmallToolsEndorsementPerOccurrenceQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_ContractorsEquipmentSmallToolsEndorsementPerOccurrenceQuotedPremium)
            End Set
        End Property
        Public Property ContractorsEquipmentSmallToolsEndorsementPerToolLimit As String
            Get
                Return _ContractorsEquipmentSmallToolsEndorsementPerToolLimit
            End Get
            Set(value As String)
                _ContractorsEquipmentSmallToolsEndorsementPerToolLimit = value
                qqHelper.ConvertToLimitFormat(_ContractorsEquipmentSmallToolsEndorsementPerToolLimit)
            End Set
        End Property
        Public Property ContractorsEquipmentSmallToolsEndorsementPerToolQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_ContractorsEquipmentSmallToolsEndorsementPerToolQuotedPremium)
            End Get
            Set(value As String)
                _ContractorsEquipmentSmallToolsEndorsementPerToolQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_ContractorsEquipmentSmallToolsEndorsementPerToolQuotedPremium)
            End Set
        End Property
        Public Property SmallToolsLimit As String
            Get
                Return _SmallToolsLimit
            End Get
            Set(value As String)
                _SmallToolsLimit = value
                qqHelper.ConvertToLimitFormat(_SmallToolsLimit)
            End Set
        End Property
        Public Property SmallToolsRate As String
            Get
                Return _SmallToolsRate
            End Get
            Set(value As String)
                _SmallToolsRate = value
            End Set
        End Property
        Public Property SmallToolsDeductibleId As String
            Get
                Return _SmallToolsDeductibleId
            End Get
            Set(value As String)
                _SmallToolsDeductibleId = value
            End Set
        End Property
        Public Property SmallToolsAdditionalInterests As List(Of QuickQuoteAdditionalInterest)
            Get
                Return _SmallToolsAdditionalInterests
            End Get
            Set(value As List(Of QuickQuoteAdditionalInterest))
                _SmallToolsAdditionalInterests = value
            End Set
        End Property
        Public Property SmallToolsCanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean
            Get
                Return _SmallToolsCanUseAdditionalInterestNumForAdditionalInterestReconciliation
            End Get
            Set(value As Boolean)
                _SmallToolsCanUseAdditionalInterestNumForAdditionalInterestReconciliation = value
            End Set
        End Property
        Public Property SmallToolsQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_SmallToolsQuotedPremium)
            End Get
            Set(value As String)
                _SmallToolsQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_SmallToolsQuotedPremium)
            End Set
        End Property
        Public Property SmallToolsIsEmployeeTools As Boolean 'small tools floater
            Get
                Return _SmallToolsIsEmployeeTools
            End Get
            Set(value As Boolean)
                _SmallToolsIsEmployeeTools = value
            End Set
        End Property
        Public Property SmallToolsIsToolsLeasedOrRented As Boolean 'small tools floater
            Get
                Return _SmallToolsIsToolsLeasedOrRented
            End Get
            Set(value As Boolean)
                _SmallToolsIsToolsLeasedOrRented = value
            End Set
        End Property
        Public Property SmallToolsAnyOneLossCatastropheLimit As String
            Get
                Return _SmallToolsAnyOneLossCatastropheLimit
            End Get
            Set(value As String)
                _SmallToolsAnyOneLossCatastropheLimit = value
                qqHelper.ConvertToLimitFormat(_SmallToolsAnyOneLossCatastropheLimit)
            End Set
        End Property
        Public Property SmallToolsAnyOneLossCatastropheQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_SmallToolsAnyOneLossCatastropheQuotedPremium)
            End Get
            Set(value As String)
                _SmallToolsAnyOneLossCatastropheQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_SmallToolsAnyOneLossCatastropheQuotedPremium)
            End Set
        End Property
        Public Property InstallationScheduledLocations As List(Of QuickQuoteInstallationScheduledLocation)
            Get
                Return _InstallationScheduledLocations
            End Get
            Set(value As List(Of QuickQuoteInstallationScheduledLocation))
                _InstallationScheduledLocations = value
            End Set
        End Property
        Public ReadOnly Property InstallationScheduledLocationsTotalLimit As String 'won't use private variable; just ReadOnly prop... should equal sum of Coverage.ManualLimitAmount for InstallationScheduledLocations
            Get
                Dim tot As String = "0"
                If _InstallationScheduledLocations IsNot Nothing AndAlso _InstallationScheduledLocations.Count > 0 Then
                    For Each isl As QuickQuoteInstallationScheduledLocation In _InstallationScheduledLocations
                        tot = qqHelper.getSum(tot, isl.Limit)
                    Next
                End If
                qqHelper.ConvertToLimitFormat(tot)

                Return tot
            End Get
        End Property
        Public ReadOnly Property InstallationScheduledLocationsTotalQuotedPremium As String 'won't use private variable; just ReadOnly prop... should equal sum of Coverage.FullTermPremium for InstallationScheduledLocations
            Get
                Dim tot As String = "0"
                If _InstallationScheduledLocations IsNot Nothing AndAlso _InstallationScheduledLocations.Count > 0 Then
                    For Each isl As QuickQuoteInstallationScheduledLocation In _InstallationScheduledLocations
                        tot = qqHelper.getSum(tot, isl.QuotedPremium)
                    Next
                End If
                qqHelper.ConvertToQuotedPremiumFormat(tot)

                Return tot
            End Get
        End Property
        Public Property InstallationQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_InstallationQuotedPremium)
            End Get
            Set(value As String)
                _InstallationQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_InstallationQuotedPremium)
            End Set
        End Property
        Public Property InstallationAdditionalInterests As List(Of QuickQuoteAdditionalInterest)
            Get
                Return _InstallationAdditionalInterests
            End Get
            Set(value As List(Of QuickQuoteAdditionalInterest))
                _InstallationAdditionalInterests = value
            End Set
        End Property
        Public Property InstallationCanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean
            Get
                Return _InstallationCanUseAdditionalInterestNumForAdditionalInterestReconciliation
            End Get
            Set(value As Boolean)
                _InstallationCanUseAdditionalInterestNumForAdditionalInterestReconciliation = value
            End Set
        End Property
        Public Property InstallationBlanketLimit As String
            Get
                Return _InstallationBlanketLimit
            End Get
            Set(value As String)
                _InstallationBlanketLimit = value
                qqHelper.ConvertToLimitFormat(_InstallationBlanketLimit)
            End Set
        End Property
        Public Property InstallationBlanketCoinsuranceTypeId As String
            Get
                Return _InstallationBlanketCoinsuranceTypeId
            End Get
            Set(value As String)
                _InstallationBlanketCoinsuranceTypeId = value
            End Set
        End Property
        Public Property InstallationBlanketDeductibleId As String
            Get
                Return _InstallationBlanketDeductibleId
            End Get
            Set(value As String)
                _InstallationBlanketDeductibleId = value
            End Set
        End Property
        Public Property InstallationBlanketRate As String
            Get
                Return _InstallationBlanketRate
            End Get
            Set(value As String)
                _InstallationBlanketRate = value
            End Set
        End Property
        Public Property InstallationBlanketQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_InstallationBlanketQuotedPremium)
            End Get
            Set(value As String)
                _InstallationBlanketQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_InstallationBlanketQuotedPremium)
            End Set
        End Property
        Public Property InstallationBlanketAnyOneLossCatastropheLimit As String
            Get
                Return _InstallationBlanketAnyOneLossCatastropheLimit
            End Get
            Set(value As String)
                _InstallationBlanketAnyOneLossCatastropheLimit = value
                qqHelper.ConvertToLimitFormat(_InstallationBlanketAnyOneLossCatastropheLimit)
            End Set
        End Property
        Public Property InstallationBlanketAnyOneLossCatastropheQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_InstallationBlanketAnyOneLossCatastropheQuotedPremium)
            End Get
            Set(value As String)
                _InstallationBlanketAnyOneLossCatastropheQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_InstallationBlanketAnyOneLossCatastropheQuotedPremium)
            End Set
        End Property
        Public Property InstallationAdditionalDebrisRemovalExpenseLimit As String
            Get
                Return _InstallationAdditionalDebrisRemovalExpenseLimit
            End Get
            Set(value As String)
                _InstallationAdditionalDebrisRemovalExpenseLimit = value
                qqHelper.ConvertToLimitFormat(_InstallationAdditionalDebrisRemovalExpenseLimit)
            End Set
        End Property
        Public Property InstallationAdditionalDebrisRemovalExpenseQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_InstallationAdditionalDebrisRemovalExpenseQuotedPremium)
            End Get
            Set(value As String)
                _InstallationAdditionalDebrisRemovalExpenseQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_InstallationAdditionalDebrisRemovalExpenseQuotedPremium)
            End Set
        End Property
        Public Property InstallationStorageLocationsLimit As String
            Get
                Return _InstallationStorageLocationsLimit
            End Get
            Set(value As String)
                _InstallationStorageLocationsLimit = value
                qqHelper.ConvertToLimitFormat(_InstallationStorageLocationsLimit)
            End Set
        End Property
        Public Property InstallationStorageLocationsQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_InstallationStorageLocationsQuotedPremium)
            End Get
            Set(value As String)
                _InstallationStorageLocationsQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_InstallationStorageLocationsQuotedPremium)
            End Set
        End Property
        Public Property InstallationTransitLimit As String
            Get
                Return _InstallationTransitLimit
            End Get
            Set(value As String)
                _InstallationTransitLimit = value
                qqHelper.ConvertToLimitFormat(_InstallationTransitLimit)
            End Set
        End Property
        Public Property InstallationTransitQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_InstallationTransitQuotedPremium)
            End Get
            Set(value As String)
                _InstallationTransitQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_InstallationTransitQuotedPremium)
            End Set
        End Property
        Public Property InstallationTestingLimit As String
            Get
                Return _InstallationTestingLimit
            End Get
            Set(value As String)
                _InstallationTestingLimit = value
                qqHelper.ConvertToLimitFormat(_InstallationTestingLimit)
            End Set
        End Property
        Public Property InstallationTestingQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_InstallationTestingQuotedPremium)
            End Get
            Set(value As String)
                _InstallationTestingQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_InstallationTestingQuotedPremium)
            End Set
        End Property
        Public Property InstallationSewerBackupLimit As String
            Get
                Return _InstallationSewerBackupLimit
            End Get
            Set(value As String)
                _InstallationSewerBackupLimit = value
                qqHelper.ConvertToLimitFormat(_InstallationSewerBackupLimit)
            End Set
        End Property
        Public Property InstallationSewerBackupDeductible As String
            Get
                Return _InstallationSewerBackupDeductible
            End Get
            Set(value As String)
                _InstallationSewerBackupDeductible = value 'might need limit formatting
            End Set
        End Property
        Public Property InstallationSewerBackupQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_InstallationSewerBackupQuotedPremium)
            End Get
            Set(value As String)
                _InstallationSewerBackupQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_InstallationSewerBackupQuotedPremium)
            End Set
        End Property
        Public Property InstallationSewerBackupCatastropheLimit As String
            Get
                Return _InstallationSewerBackupCatastropheLimit
            End Get
            Set(value As String)
                _InstallationSewerBackupCatastropheLimit = value
                qqHelper.ConvertToLimitFormat(_InstallationSewerBackupCatastropheLimit)
            End Set
        End Property
        Public Property InstallationSewerBackupCatastropheQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_InstallationSewerBackupCatastropheQuotedPremium)
            End Get
            Set(value As String)
                _InstallationSewerBackupCatastropheQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_InstallationSewerBackupCatastropheQuotedPremium)
            End Set
        End Property
        Public Property InstallationEarthquakeLimit As String
            Get
                Return _InstallationEarthquakeLimit
            End Get
            Set(value As String)
                _InstallationEarthquakeLimit = value
                qqHelper.ConvertToLimitFormat(_InstallationEarthquakeLimit)
            End Set
        End Property
        Public Property InstallationEarthquakeDeductible As String
            Get
                Return _InstallationEarthquakeDeductible
            End Get
            Set(value As String)
                _InstallationEarthquakeDeductible = value 'might need limit formatting
            End Set
        End Property
        Public Property InstallationEarthquakeQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_InstallationEarthquakeQuotedPremium)
            End Get
            Set(value As String)
                _InstallationEarthquakeQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_InstallationEarthquakeQuotedPremium)
            End Set
        End Property
        Public Property InstallationEarthquakeCatastropheLimit As String
            Get
                Return _InstallationEarthquakeCatastropheLimit
            End Get
            Set(value As String)
                _InstallationEarthquakeCatastropheLimit = value
                qqHelper.ConvertToLimitFormat(_InstallationEarthquakeCatastropheLimit)
            End Set
        End Property
        Public Property InstallationEarthquakeCatastropheQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_InstallationEarthquakeCatastropheQuotedPremium)
            End Get
            Set(value As String)
                _InstallationEarthquakeCatastropheQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_InstallationEarthquakeCatastropheQuotedPremium)
            End Set
        End Property
        Public Property BusinessPersonalPropertyLimit As String 'shown in UI Installation Coverage Extensions section, but may not be specific to Installation
            Get
                Return _BusinessPersonalPropertyLimit
            End Get
            Set(value As String)
                _BusinessPersonalPropertyLimit = value
                qqHelper.ConvertToLimitFormat(_BusinessPersonalPropertyLimit)
            End Set
        End Property
        Public Property BusinessPersonalPropertyQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_BusinessPersonalPropertyQuotedPremium)
            End Get
            Set(value As String)
                _BusinessPersonalPropertyQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_BusinessPersonalPropertyQuotedPremium)
            End Set
        End Property
        Public Property ScheduledPropertyItems As List(Of QuickQuoteScheduledPropertyItem)
            Get
                Return _ScheduledPropertyItems
            End Get
            Set(value As List(Of QuickQuoteScheduledPropertyItem))
                _ScheduledPropertyItems = value
            End Set
        End Property
        Public ReadOnly Property ScheduledPropertyItemsTotalLimit As String 'won't use private variable; just ReadOnly prop... should equal sum of Coverage.ManualLimitAmount for ScheduledPropertyItems
            Get
                Dim tot As String = "0"
                If _ScheduledPropertyItems IsNot Nothing AndAlso _ScheduledPropertyItems.Count > 0 Then
                    For Each sp As QuickQuoteScheduledPropertyItem In _ScheduledPropertyItems
                        tot = qqHelper.getSum(tot, sp.Limit)
                    Next
                End If
                qqHelper.ConvertToLimitFormat(tot)

                Return tot
            End Get
        End Property
        Public ReadOnly Property ScheduledPropertyItemsTotalQuotedPremium As String 'won't use private variable; just ReadOnly prop... should equal sum of Coverage.FullTermPremium for ScheduledPropertyItems
            Get
                Dim tot As String = "0"
                If _ScheduledPropertyItems IsNot Nothing AndAlso _ScheduledPropertyItems.Count > 0 Then
                    For Each sp As QuickQuoteScheduledPropertyItem In _ScheduledPropertyItems
                        tot = qqHelper.getSum(tot, sp.QuotedPremium)
                    Next
                End If
                qqHelper.ConvertToQuotedPremiumFormat(tot)

                Return tot
            End Get
        End Property
        Public Property ScheduledPropertyAdditionalInterests As List(Of QuickQuoteAdditionalInterest)
            Get
                Return _ScheduledPropertyAdditionalInterests
            End Get
            Set(value As List(Of QuickQuoteAdditionalInterest))
                _ScheduledPropertyAdditionalInterests = value
            End Set
        End Property
        Public Property ScheduledPropertyCanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean
            Get
                Return _ScheduledPropertyCanUseAdditionalInterestNumForAdditionalInterestReconciliation
            End Get
            Set(value As Boolean)
                _ScheduledPropertyCanUseAdditionalInterestNumForAdditionalInterestReconciliation = value
            End Set
        End Property
        Public Property ScheduledPropertyCoinsuranceTypeId As String
            Get
                Return _ScheduledPropertyCoinsuranceTypeId
            End Get
            Set(value As String)
                _ScheduledPropertyCoinsuranceTypeId = value
            End Set
        End Property
        Public Property ScheduledPropertyDeductibleId As String
            Get
                Return _ScheduledPropertyDeductibleId
            End Get
            Set(value As String)
                _ScheduledPropertyDeductibleId = value
            End Set
        End Property
        Public Property ScheduledPropertyRate As String
            Get
                Return _ScheduledPropertyRate
            End Get
            Set(value As String)
                _ScheduledPropertyRate = value
            End Set
        End Property
        Public Property ScheduledPropertyNamedPerils As Boolean
            Get
                Return _ScheduledPropertyNamedPerils
            End Get
            Set(value As Boolean)
                _ScheduledPropertyNamedPerils = value
            End Set
        End Property
        Public Property ScheduledPropertyQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_ScheduledPropertyQuotedPremium)
            End Get
            Set(value As String)
                _ScheduledPropertyQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_ScheduledPropertyQuotedPremium)
            End Set
        End Property
        'Public ReadOnly Property ComputerBuildingsTotalLimit As String 'won't use private variable; just ReadOnly prop... should equal sum of Coverage.ManualLimitAmount for applicable buildings covs; moved to RiskLevel object 7/23/2018
        '    Get
        '        Dim tot As String = "0"
        '        If Locations IsNot Nothing AndAlso Locations.Count > 0 Then 'updated 7/21/2018 from private variable to public property
        '            For Each l As QuickQuoteLocation In Locations
        '                If l.Buildings IsNot Nothing AndAlso l.Buildings.Count > 0 Then
        '                    For Each b As QuickQuoteBuilding In l.Buildings
        '                        tot = qqHelper.getSum(tot, b.ComputerHardwareLimit)
        '                        tot = qqHelper.getSum(tot, b.ComputerProgramsApplicationsAndMediaLimit)
        '                        tot = qqHelper.getSum(tot, b.ComputerBusinessIncomeLimit)
        '                    Next
        '                End If
        '            Next
        '        End If
        '        qqHelper.ConvertToLimitFormat(tot)

        '        Return tot
        '    End Get
        'End Property
        'Public ReadOnly Property ComputerBuildingsTotalQuotedPremium As String 'won't use private variable; just ReadOnly prop... should equal sum of Coverage.FullTermPremium for applicable buildings covs; moved to RiskLevel object 7/23/2018
        '    Get
        '        Dim tot As String = "0"
        '        If Locations IsNot Nothing AndAlso Locations.Count > 0 Then 'updated 7/21/2018 from private variable to public property
        '            For Each l As QuickQuoteLocation In Locations
        '                If l.Buildings IsNot Nothing AndAlso l.Buildings.Count > 0 Then
        '                    For Each b As QuickQuoteBuilding In l.Buildings
        '                        tot = qqHelper.getSum(tot, b.ComputerHardwareQuotedPremium)
        '                        tot = qqHelper.getSum(tot, b.ComputerProgramsApplicationsAndMediaQuotedPremium)
        '                        tot = qqHelper.getSum(tot, b.ComputerBusinessIncomeQuotedPremium)
        '                    Next
        '                End If
        '            Next
        '        End If
        '        qqHelper.ConvertToQuotedPremiumFormat(tot)

        '        Return tot
        '    End Get
        'End Property
        'Public Property ComputerCoinsuranceTypeId As String 'cov also has CoverageBasisTypeId set to 1
        '    Get
        '        Return _ComputerCoinsuranceTypeId
        '    End Get
        '    Set(value As String)
        '        _ComputerCoinsuranceTypeId = value
        '    End Set
        'End Property
        'Public Property ComputerExcludeEarthquake As Boolean
        '    Get
        '        Return _ComputerExcludeEarthquake
        '    End Get
        '    Set(value As Boolean)
        '        _ComputerExcludeEarthquake = value
        '    End Set
        'End Property
        'Public Property ComputerValuationMethodTypeId As String
        '    Get
        '        Return _ComputerValuationMethodTypeId
        '    End Get
        '    Set(value As String)
        '        _ComputerValuationMethodTypeId = value
        '    End Set
        'End Property
        'Public Property ComputerAdditionalInterests As List(Of QuickQuoteAdditionalInterest)
        '    Get
        '        Return _ComputerAdditionalInterests
        '    End Get
        '    Set(value As List(Of QuickQuoteAdditionalInterest))
        '        _ComputerAdditionalInterests = value
        '    End Set
        'End Property
        'Public Property ComputerCanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean
        '    Get
        '        Return _ComputerCanUseAdditionalInterestNumForAdditionalInterestReconciliation
        '    End Get
        '    Set(value As Boolean)
        '        _ComputerCanUseAdditionalInterestNumForAdditionalInterestReconciliation = value
        '    End Set
        'End Property
        'Public Property ComputerQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_ComputerQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _ComputerQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_ComputerQuotedPremium)
        '    End Set
        'End Property
        'Public Property ComputerAllPerilsDeductibleId As String 'cov also has CoverageBasisTypeId set to 1
        '    Get
        '        Return _ComputerAllPerilsDeductibleId
        '    End Get
        '    Set(value As String)
        '        _ComputerAllPerilsDeductibleId = value
        '    End Set
        'End Property
        'Public Property ComputerAllPerilsQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_ComputerAllPerilsQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _ComputerAllPerilsQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_ComputerAllPerilsQuotedPremium)
        '    End Set
        'End Property
        'Public Property ComputerEarthquakeVolcanicEruptionDeductible As String 'cov also has CoverageBasisTypeId set to 1; example also has ApplyToWrittenPremiuim set to true
        '    Get
        '        Return _ComputerEarthquakeVolcanicEruptionDeductible
        '    End Get
        '    Set(value As String)
        '        _ComputerEarthquakeVolcanicEruptionDeductible = value 'might need limit formatting
        '    End Set
        'End Property
        'Public Property ComputerEarthquakeVolcanicEruptionQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_ComputerEarthquakeVolcanicEruptionQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _ComputerEarthquakeVolcanicEruptionQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_ComputerEarthquakeVolcanicEruptionQuotedPremium)
        '    End Set
        'End Property
        'Public Property ComputerMechanicalBreakdownDeductible As String 'cov also has CoverageBasisTypeId set to 1; example also has ApplyToWrittenPremiuim set to true
        '    Get
        '        Return _ComputerMechanicalBreakdownDeductible
        '    End Get
        '    Set(value As String)
        '        _ComputerMechanicalBreakdownDeductible = value 'might need limit formatting
        '    End Set
        'End Property
        'Public Property ComputerMechanicalBreakdownQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_ComputerMechanicalBreakdownQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _ComputerMechanicalBreakdownQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_ComputerMechanicalBreakdownQuotedPremium)
        '    End Set
        'End Property
        Public Property BuildersRiskDeductibleId As String 'cov also has CoverageBasisTypeId set to 1
            Get
                Return _BuildersRiskDeductibleId
            End Get
            Set(value As String)
                _BuildersRiskDeductibleId = value
            End Set
        End Property
        Public Property BuildersRiskRate As String
            Get
                Return _BuildersRiskRate
            End Get
            Set(value As String)
                _BuildersRiskRate = value
            End Set
        End Property
        Public Property BuildersRiskAdditionalInterests As List(Of QuickQuoteAdditionalInterest)
            Get
                Return _BuildersRiskAdditionalInterests
            End Get
            Set(value As List(Of QuickQuoteAdditionalInterest))
                _BuildersRiskAdditionalInterests = value
            End Set
        End Property
        Public Property BuildersRiskCanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean
            Get
                Return _BuildersRiskCanUseAdditionalInterestNumForAdditionalInterestReconciliation
            End Get
            Set(value As Boolean)
                _BuildersRiskCanUseAdditionalInterestNumForAdditionalInterestReconciliation = value
            End Set
        End Property
        Public Property BuildersRiskQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_BuildersRiskQuotedPremium)
            End Get
            Set(value As String)
                _BuildersRiskQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_BuildersRiskQuotedPremium)
            End Set
        End Property
        Public Property BuildersRiskScheduledLocations As List(Of QuickQuoteBuildersRiskScheduledLocation)
            Get
                Return _BuildersRiskScheduledLocations
            End Get
            Set(value As List(Of QuickQuoteBuildersRiskScheduledLocation))
                _BuildersRiskScheduledLocations = value
            End Set
        End Property
        Public ReadOnly Property BuildersRiskScheduledLocationsTotalLimit As String 'won't use private variable; just ReadOnly prop... should equal sum of Coverage.ManualLimitAmount for BuildersRiskScheduledLocations; 2/19/2015 note: this should also include Coverage.ManualLimitAmount for other covs under ScheduledCoverage... currently looking at 21348 (Builder's Risk - Schedule) only
            Get
                Dim tot As String = "0"
                If _BuildersRiskScheduledLocations IsNot Nothing AndAlso _BuildersRiskScheduledLocations.Count > 0 Then
                    For Each isl As QuickQuoteBuildersRiskScheduledLocation In _BuildersRiskScheduledLocations
                        tot = qqHelper.getSum(tot, isl.Limit)
                    Next
                End If
                qqHelper.ConvertToLimitFormat(tot)

                Return tot
            End Get
        End Property
        Public ReadOnly Property BuildersRiskScheduledLocationsTotalQuotedPremium As String 'won't use private variable; just ReadOnly prop... should equal sum of Coverage.FullTermPremium for BuildersRiskScheduledLocations
            Get
                Dim tot As String = "0"
                If _BuildersRiskScheduledLocations IsNot Nothing AndAlso _BuildersRiskScheduledLocations.Count > 0 Then
                    For Each isl As QuickQuoteBuildersRiskScheduledLocation In _BuildersRiskScheduledLocations
                        tot = qqHelper.getSum(tot, isl.QuotedPremium)
                    Next
                End If
                qqHelper.ConvertToQuotedPremiumFormat(tot)

                Return tot
            End Get
        End Property
        Public Property BuildersRiskScheduleStorageLocationsLimit As String
            Get
                Return _BuildersRiskScheduleStorageLocationsLimit
            End Get
            Set(value As String)
                _BuildersRiskScheduleStorageLocationsLimit = value
                qqHelper.ConvertToLimitFormat(_BuildersRiskScheduleStorageLocationsLimit)
            End Set
        End Property
        Public Property BuildersRiskScheduleStorageLocationsQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_BuildersRiskScheduleStorageLocationsQuotedPremium)
            End Get
            Set(value As String)
                _BuildersRiskScheduleStorageLocationsQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_BuildersRiskScheduleStorageLocationsQuotedPremium)
            End Set
        End Property
        Public Property BuildersRiskScheduleTransitLimit As String
            Get
                Return _BuildersRiskScheduleTransitLimit
            End Get
            Set(value As String)
                _BuildersRiskScheduleTransitLimit = value
                qqHelper.ConvertToLimitFormat(_BuildersRiskScheduleTransitLimit)
            End Set
        End Property
        Public Property BuildersRiskScheduleTransitQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_BuildersRiskScheduleTransitQuotedPremium)
            End Get
            Set(value As String)
                _BuildersRiskScheduleTransitQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_BuildersRiskScheduleTransitQuotedPremium)
            End Set
        End Property
        Public Property BuildersRiskScheduleTestingLimit As String
            Get
                Return _BuildersRiskScheduleTestingLimit
            End Get
            Set(value As String)
                _BuildersRiskScheduleTestingLimit = value
                qqHelper.ConvertToLimitFormat(_BuildersRiskScheduleTestingLimit)
            End Set
        End Property
        Public Property BuildersRiskScheduleTestingQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_BuildersRiskScheduleTestingQuotedPremium)
            End Get
            Set(value As String)
                _BuildersRiskScheduleTestingQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_BuildersRiskScheduleTestingQuotedPremium)
            End Set
        End Property
        'Public Property FineArtsDeductibleCategoryTypeId As String 'static data
        '    Get
        '        Return _FineArtsDeductibleCategoryTypeId
        '    End Get
        '    Set(value As String)
        '        _FineArtsDeductibleCategoryTypeId = value
        '    End Set
        'End Property
        'Public Property FineArtsRate As String
        '    Get
        '        Return _FineArtsRate
        '    End Get
        '    Set(value As String)
        '        _FineArtsRate = value
        '    End Set
        'End Property
        'Public Property FineArtsDeductibleId As String 'static data
        '    Get
        '        Return _FineArtsDeductibleId
        '    End Get
        '    Set(value As String)
        '        _FineArtsDeductibleId = value
        '    End Set
        'End Property
        'Public Property FineArtsQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_FineArtsQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _FineArtsQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_FineArtsQuotedPremium)
        '    End Set
        'End Property
        'Public Property FineArtsAdditionalInterests As List(Of QuickQuoteAdditionalInterest)
        '    Get
        '        Return _FineArtsAdditionalInterests
        '    End Get
        '    Set(value As List(Of QuickQuoteAdditionalInterest))
        '        _FineArtsAdditionalInterests = value
        '    End Set
        'End Property
        'Public Property FineArtsCanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean
        '    Get
        '        Return _FineArtsCanUseAdditionalInterestNumForAdditionalInterestReconciliation
        '    End Get
        '    Set(value As Boolean)
        '        _FineArtsCanUseAdditionalInterestNumForAdditionalInterestReconciliation = value
        '    End Set
        'End Property
        'Public ReadOnly Property FineArtsBuildingsTotalLimit As String 'won't use private variable; just ReadOnly prop... should equal sum of Coverage.ManualLimitAmount for applicable buildings covs; moved to RiskLevel object 7/23/2018
        '    Get
        '        Dim tot As String = "0"
        '        If Locations IsNot Nothing AndAlso Locations.Count > 0 Then 'updated 7/21/2018 from private variable to public property
        '            For Each l As QuickQuoteLocation In Locations
        '                If l.Buildings IsNot Nothing AndAlso l.Buildings.Count > 0 Then
        '                    For Each b As QuickQuoteBuilding In l.Buildings
        '                        If b.FineArtsScheduledItems IsNot Nothing AndAlso b.FineArtsScheduledItems.Count > 0 Then
        '                            For Each fa As QuickQuoteFineArtsScheduledItem In b.FineArtsScheduledItems
        '                                tot = qqHelper.getSum(tot, fa.Limit)
        '                            Next
        '                        End If
        '                    Next
        '                End If
        '            Next
        '        End If
        '        qqHelper.ConvertToLimitFormat(tot)

        '        Return tot
        '    End Get
        'End Property
        'Public ReadOnly Property FineArtsBuildingsTotalQuotedPremium As String 'won't use private variable; just ReadOnly prop... should equal sum of Coverage.FullTermPremium for applicable buildings covs; moved to RiskLevel object 7/23/2018
        '    Get
        '        Dim tot As String = "0"
        '        If Locations IsNot Nothing AndAlso Locations.Count > 0 Then 'updated 7/21/2018 from private variable to public property
        '            For Each l As QuickQuoteLocation In Locations
        '                If l.Buildings IsNot Nothing AndAlso l.Buildings.Count > 0 Then
        '                    For Each b As QuickQuoteBuilding In l.Buildings
        '                        If b.FineArtsScheduledItems IsNot Nothing AndAlso b.FineArtsScheduledItems.Count > 0 Then
        '                            For Each fa As QuickQuoteFineArtsScheduledItem In b.FineArtsScheduledItems
        '                                tot = qqHelper.getSum(tot, fa.QuotedPremium)
        '                            Next
        '                        End If
        '                    Next
        '                End If
        '            Next
        '        End If
        '        qqHelper.ConvertToQuotedPremiumFormat(tot)

        '        Return tot
        '    End Get
        'End Property
        'Public Property FineArtsBreakageMarringOrScratching As Boolean 'renamed from HasFineArtsBreakageMarringOrScratching
        '    Get
        '        Return _FineArtsBreakageMarringOrScratching
        '    End Get
        '    Set(value As Boolean)
        '        _FineArtsBreakageMarringOrScratching = value
        '    End Set
        'End Property
        'Public Property FineArtsBreakageMarringOrScratchingQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_FineArtsBreakageMarringOrScratchingQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _FineArtsBreakageMarringOrScratchingQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_FineArtsBreakageMarringOrScratchingQuotedPremium)
        '    End Set
        'End Property
        Public Property OwnersCargoAnyOneOwnedVehicleLimit As String
            Get
                Return _OwnersCargoAnyOneOwnedVehicleLimit
            End Get
            Set(value As String)
                _OwnersCargoAnyOneOwnedVehicleLimit = value
                qqHelper.ConvertToLimitFormat(_OwnersCargoAnyOneOwnedVehicleLimit)
            End Set
        End Property
        Public Property OwnersCargoAnyOneOwnedVehicleDeductibleId As String 'static data
            Get
                Return _OwnersCargoAnyOneOwnedVehicleDeductibleId
            End Get
            Set(value As String)
                _OwnersCargoAnyOneOwnedVehicleDeductibleId = value
            End Set
        End Property
        Public Property OwnersCargoAnyOneOwnedVehicleRate As String
            Get
                Return _OwnersCargoAnyOneOwnedVehicleRate
            End Get
            Set(value As String)
                _OwnersCargoAnyOneOwnedVehicleRate = value
            End Set
        End Property
        Public Property OwnersCargoAnyOneOwnedVehicleDescription As String
            Get
                Return _OwnersCargoAnyOneOwnedVehicleDescription
            End Get
            Set(value As String)
                _OwnersCargoAnyOneOwnedVehicleDescription = value
            End Set
        End Property
        Public Property OwnersCargoAnyOneOwnedVehicleAdditionalInterests As List(Of QuickQuoteAdditionalInterest)
            Get
                Return _OwnersCargoAnyOneOwnedVehicleAdditionalInterests
            End Get
            Set(value As List(Of QuickQuoteAdditionalInterest))
                _OwnersCargoAnyOneOwnedVehicleAdditionalInterests = value
            End Set
        End Property
        Public Property OwnersCargoAnyOneOwnedVehicleCanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean
            Get
                Return _OwnersCargoAnyOneOwnedVehicleCanUseAdditionalInterestNumForAdditionalInterestReconciliation
            End Get
            Set(value As Boolean)
                _OwnersCargoAnyOneOwnedVehicleCanUseAdditionalInterestNumForAdditionalInterestReconciliation = value
            End Set
        End Property
        Public Property OwnersCargoAnyOneOwnedVehicleLoadingUnloading As Boolean
            Get
                Return _OwnersCargoAnyOneOwnedVehicleLoadingUnloading
            End Get
            Set(value As Boolean)
                _OwnersCargoAnyOneOwnedVehicleLoadingUnloading = value
            End Set
        End Property
        Public Property OwnersCargoAnyOneOwnedVehicleNamedPerils As Boolean
            Get
                Return _OwnersCargoAnyOneOwnedVehicleNamedPerils
            End Get
            Set(value As Boolean)
                _OwnersCargoAnyOneOwnedVehicleNamedPerils = value
            End Set
        End Property
        Public Property OwnersCargoAnyOneOwnedVehicleQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_OwnersCargoAnyOneOwnedVehicleQuotedPremium)
            End Get
            Set(value As String)
                _OwnersCargoAnyOneOwnedVehicleQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_OwnersCargoAnyOneOwnedVehicleQuotedPremium)
            End Set
        End Property
        Public Property OwnersCargoCatastropheLimit As String
            Get
                Return _OwnersCargoCatastropheLimit
            End Get
            Set(value As String)
                _OwnersCargoCatastropheLimit = value
                qqHelper.ConvertToLimitFormat(_OwnersCargoCatastropheLimit)
            End Set
        End Property
        Public Property OwnersCargoCatastropheQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_OwnersCargoCatastropheQuotedPremium)
            End Get
            Set(value As String)
                _OwnersCargoCatastropheQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_OwnersCargoCatastropheQuotedPremium)
            End Set
        End Property
        Public Property TransportationCatastropheLimit As String
            Get
                Return _TransportationCatastropheLimit
            End Get
            Set(value As String)
                _TransportationCatastropheLimit = value
                qqHelper.ConvertToLimitFormat(_TransportationCatastropheLimit)
            End Set
        End Property
        Public Property TransportationCatastropheDeductibleId As String 'static data
            Get
                Return _TransportationCatastropheDeductibleId
            End Get
            Set(value As String)
                _TransportationCatastropheDeductibleId = value
            End Set
        End Property
        Public Property TransportationCatastropheDescription As String
            Get
                Return _TransportationCatastropheDescription
            End Get
            Set(value As String)
                _TransportationCatastropheDescription = value
            End Set
        End Property
        Public Property TransportationCatastropheAdditionalInterests As List(Of QuickQuoteAdditionalInterest)
            Get
                Return _TransportationCatastropheAdditionalInterests
            End Get
            Set(value As List(Of QuickQuoteAdditionalInterest))
                _TransportationCatastropheAdditionalInterests = value
            End Set
        End Property
        Public Property TransportationCatastropheCanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean
            Get
                Return _TransportationCatastropheCanUseAdditionalInterestNumForAdditionalInterestReconciliation
            End Get
            Set(value As Boolean)
                _TransportationCatastropheCanUseAdditionalInterestNumForAdditionalInterestReconciliation = value
            End Set
        End Property
        Public Property TransportationCatastropheLoadingUnloading As Boolean
            Get
                Return _TransportationCatastropheLoadingUnloading
            End Get
            Set(value As Boolean)
                _TransportationCatastropheLoadingUnloading = value
            End Set
        End Property
        Public Property TransportationCatastropheNamedPerils As Boolean
            Get
                Return _TransportationCatastropheNamedPerils
            End Get
            Set(value As Boolean)
                _TransportationCatastropheNamedPerils = value
            End Set
        End Property
        Public Property TransportationCatastropheQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_TransportationCatastropheQuotedPremium)
            End Get
            Set(value As String)
                _TransportationCatastropheQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_TransportationCatastropheQuotedPremium)
            End Set
        End Property
        Public Property TransportationAnyOneOwnedVehicleLimit As String 'note: cov also has CoverageBasisTypeId set to 1
            Get
                Return _TransportationAnyOneOwnedVehicleLimit
            End Get
            Set(value As String)
                _TransportationAnyOneOwnedVehicleLimit = value
                qqHelper.ConvertToLimitFormat(_TransportationAnyOneOwnedVehicleLimit)
            End Set
        End Property
        Public Property TransportationAnyOneOwnedVehicleNumberOfVehicles As String 'CoverageDetail
            Get
                Return _TransportationAnyOneOwnedVehicleNumberOfVehicles
            End Get
            Set(value As String)
                _TransportationAnyOneOwnedVehicleNumberOfVehicles = value
            End Set
        End Property
        Public Property TransportationAnyOneOwnedVehicleRate As String
            Get
                Return _TransportationAnyOneOwnedVehicleRate
            End Get
            Set(value As String)
                _TransportationAnyOneOwnedVehicleRate = value
            End Set
        End Property
        Public Property TransportationAnyOneOwnedVehicleQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_TransportationAnyOneOwnedVehicleQuotedPremium)
            End Get
            Set(value As String)
                _TransportationAnyOneOwnedVehicleQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_TransportationAnyOneOwnedVehicleQuotedPremium)
            End Set
        End Property
        Public Property MotorTruckCargoScheduledVehicles As List(Of QuickQuoteScheduledVehicle)
            Get
                Return _MotorTruckCargoScheduledVehicles
            End Get
            Set(value As List(Of QuickQuoteScheduledVehicle))
                _MotorTruckCargoScheduledVehicles = value
            End Set
        End Property
        Public ReadOnly Property MotorTruckCargoScheduledVehiclesTotalLimit As String 'won't use private variable; just ReadOnly prop... should equal sum of Coverage.ManualLimitAmount for MotorTruckCargoScheduledVehicles
            Get
                Dim tot As String = "0"
                If _MotorTruckCargoScheduledVehicles IsNot Nothing AndAlso _MotorTruckCargoScheduledVehicles.Count > 0 Then
                    For Each sv As QuickQuoteScheduledVehicle In _MotorTruckCargoScheduledVehicles
                        tot = qqHelper.getSum(tot, sv.Limit)
                    Next
                End If
                qqHelper.ConvertToLimitFormat(tot)

                Return tot
            End Get
        End Property
        Public ReadOnly Property MotorTruckCargoScheduledVehiclesTotalQuotedPremium As String 'won't use private variable; just ReadOnly prop... should equal sum of Coverage.FullTermPremium for MotorTruckCargoScheduledVehicles
            Get
                Dim tot As String = "0"
                If _MotorTruckCargoScheduledVehicles IsNot Nothing AndAlso _MotorTruckCargoScheduledVehicles.Count > 0 Then
                    For Each sv As QuickQuoteScheduledVehicle In _MotorTruckCargoScheduledVehicles
                        tot = qqHelper.getSum(tot, sv.QuotedPremium)
                    Next
                End If
                qqHelper.ConvertToQuotedPremiumFormat(tot)

                Return tot
            End Get
        End Property
        Public Property MotorTruckCargoScheduledVehicleAdditionalInterests As List(Of QuickQuoteAdditionalInterest)
            Get
                Return _MotorTruckCargoScheduledVehicleAdditionalInterests
            End Get
            Set(value As List(Of QuickQuoteAdditionalInterest))
                _MotorTruckCargoScheduledVehicleAdditionalInterests = value
            End Set
        End Property
        Public Property MotorTruckCargoScheduledVehicleCanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean
            Get
                Return _MotorTruckCargoScheduledVehicleCanUseAdditionalInterestNumForAdditionalInterestReconciliation
            End Get
            Set(value As Boolean)
                _MotorTruckCargoScheduledVehicleCanUseAdditionalInterestNumForAdditionalInterestReconciliation = value
            End Set
        End Property
        Public Property MotorTruckCargoScheduledVehicleLoadingUnloading As Boolean 'CoverageDetail
            Get
                Return _MotorTruckCargoScheduledVehicleLoadingUnloading
            End Get
            Set(value As Boolean)
                _MotorTruckCargoScheduledVehicleLoadingUnloading = value
            End Set
        End Property
        Public Property MotorTruckCargoScheduledVehicleNamedPerils As Boolean 'CoverageDetail
            Get
                Return _MotorTruckCargoScheduledVehicleNamedPerils
            End Get
            Set(value As Boolean)
                _MotorTruckCargoScheduledVehicleNamedPerils = value
            End Set
        End Property
        Public Property MotorTruckCargoScheduledVehicleOperatingRadius As String 'CoverageDetail
            Get
                Return _MotorTruckCargoScheduledVehicleOperatingRadius
            End Get
            Set(value As String)
                _MotorTruckCargoScheduledVehicleOperatingRadius = value
            End Set
        End Property
        Public Property MotorTruckCargoScheduledVehicleRate As String 'CoverageDetail
            Get
                Return _MotorTruckCargoScheduledVehicleRate
            End Get
            Set(value As String)
                _MotorTruckCargoScheduledVehicleRate = value
            End Set
        End Property
        Public Property MotorTruckCargoScheduledVehicleDeductibleId As String 'static data
            Get
                Return _MotorTruckCargoScheduledVehicleDeductibleId
            End Get
            Set(value As String)
                _MotorTruckCargoScheduledVehicleDeductibleId = value
            End Set
        End Property
        Public Property MotorTruckCargoScheduledVehicleDescription As String
            Get
                Return _MotorTruckCargoScheduledVehicleDescription
            End Get
            Set(value As String)
                _MotorTruckCargoScheduledVehicleDescription = value
            End Set
        End Property
        Public Property MotorTruckCargoScheduledVehicleQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_MotorTruckCargoScheduledVehicleQuotedPremium)
            End Get
            Set(value As String)
                _MotorTruckCargoScheduledVehicleQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_MotorTruckCargoScheduledVehicleQuotedPremium)
            End Set
        End Property
        Public Property MotorTruckCargoScheduledVehicleCatastropheLimit As String
            Get
                Return _MotorTruckCargoScheduledVehicleCatastropheLimit
            End Get
            Set(value As String)
                _MotorTruckCargoScheduledVehicleCatastropheLimit = value
                qqHelper.ConvertToLimitFormat(_MotorTruckCargoScheduledVehicleCatastropheLimit)
            End Set
        End Property
        Public Property MotorTruckCargoScheduledVehicleCatastropheQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_MotorTruckCargoScheduledVehicleCatastropheQuotedPremium)
            End Get
            Set(value As String)
                _MotorTruckCargoScheduledVehicleCatastropheQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_MotorTruckCargoScheduledVehicleCatastropheQuotedPremium)
            End Set
        End Property

        Public Property MotorTruckCargoUnScheduledVehicleAdditionalInterests As List(Of QuickQuoteAdditionalInterest)
            Get
                Return _MotorTruckCargoUnScheduledVehicleAdditionalInterests
            End Get
            Set(value As List(Of QuickQuoteAdditionalInterest))
                _MotorTruckCargoUnScheduledVehicleAdditionalInterests = value
            End Set
        End Property
        Public Property MotorTruckCargoUnScheduledVehicleCanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean
            Get
                Return _MotorTruckCargoUnScheduledVehicleCanUseAdditionalInterestNumForAdditionalInterestReconciliation
            End Get
            Set(value As Boolean)
                _MotorTruckCargoUnScheduledVehicleCanUseAdditionalInterestNumForAdditionalInterestReconciliation = value
            End Set
        End Property
        Public Property MotorTruckCargoUnScheduledVehicleLoadingUnloading As Boolean 'CoverageDetail
            Get
                Return _MotorTruckCargoUnScheduledVehicleLoadingUnloading
            End Get
            Set(value As Boolean)
                _MotorTruckCargoUnScheduledVehicleLoadingUnloading = value
            End Set
        End Property
        Public Property MotorTruckCargoUnScheduledVehicleNamedPerils As Boolean 'CoverageDetail
            Get
                Return _MotorTruckCargoUnScheduledVehicleNamedPerils
            End Get
            Set(value As Boolean)
                _MotorTruckCargoUnScheduledVehicleNamedPerils = value
            End Set
        End Property
        Public Property MotorTruckCargoUnScheduledVehicleOperatingRadius As String 'CoverageDetail
            Get
                Return _MotorTruckCargoUnScheduledVehicleOperatingRadius
            End Get
            Set(value As String)
                _MotorTruckCargoUnScheduledVehicleOperatingRadius = value
            End Set
        End Property
        Public Property MotorTruckCargoUnScheduledVehicleRate As String 'CoverageDetail
            Get
                Return _MotorTruckCargoUnScheduledVehicleRate
            End Get
            Set(value As String)
                _MotorTruckCargoUnScheduledVehicleRate = value
            End Set
        End Property
        Public Property MotorTruckCargoUnScheduledVehicleDeductibleId As String 'static data
            Get
                Return _MotorTruckCargoUnScheduledVehicleDeductibleId
            End Get
            Set(value As String)
                _MotorTruckCargoUnScheduledVehicleDeductibleId = value
            End Set
        End Property
        Public Property MotorTruckCargoUnScheduledVehicleDescription As String
            Get
                Return _MotorTruckCargoUnScheduledVehicleDescription
            End Get
            Set(value As String)
                _MotorTruckCargoUnScheduledVehicleDescription = value
            End Set
        End Property
        Public Property MotorTruckCargoUnScheduledNumberOfVehicles As String
            Get
                Return _MotorTruckCargoUnScheduledNumberOfVehicles
            End Get
            Set(value As String)
                _MotorTruckCargoUnScheduledNumberOfVehicles = value
            End Set
        End Property
        Public Property MotorTruckCargoUnScheduledAnyVehicleLimit As String
            Get
                Return _MotorTruckCargoUnScheduledAnyVehicleLimit
            End Get
            Set(value As String)
                _MotorTruckCargoUnScheduledAnyVehicleLimit = value
                qqHelper.ConvertToLimitFormat(_MotorTruckCargoUnScheduledAnyVehicleLimit)
            End Set
        End Property
        Public Property MotorTruckCargoUnScheduledVehicleQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_MotorTruckCargoUnScheduledVehicleQuotedPremium)
            End Get
            Set(value As String)
                _MotorTruckCargoUnScheduledVehicleQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_MotorTruckCargoUnScheduledVehicleQuotedPremium)
            End Set
        End Property
        Public Property MotorTruckCargoUnScheduledVehicleCatastropheLimit As String
            Get
                Return _MotorTruckCargoUnScheduledVehicleCatastropheLimit
            End Get
            Set(value As String)
                _MotorTruckCargoUnScheduledVehicleCatastropheLimit = value
                qqHelper.ConvertToLimitFormat(_MotorTruckCargoUnScheduledVehicleCatastropheLimit)
            End Set
        End Property
        Public Property MotorTruckCargoUnScheduledVehicleCatastropheQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_MotorTruckCargoUnScheduledVehicleCatastropheQuotedPremium)
            End Get
            Set(value As String)
                _MotorTruckCargoUnScheduledVehicleCatastropheQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_MotorTruckCargoUnScheduledVehicleCatastropheQuotedPremium)
            End Set
        End Property

        'Public Property SignsAdditionalInterests As List(Of QuickQuoteAdditionalInterest) 'note: cov also has CoverageBasisTypeId set to 1
        '    Get
        '        Return _SignsAdditionalInterests
        '    End Get
        '    Set(value As List(Of QuickQuoteAdditionalInterest))
        '        _SignsAdditionalInterests = value
        '    End Set
        'End Property
        'Public Property SignsCanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean
        '    Get
        '        Return _SignsCanUseAdditionalInterestNumForAdditionalInterestReconciliation
        '    End Get
        '    Set(value As Boolean)
        '        _SignsCanUseAdditionalInterestNumForAdditionalInterestReconciliation = value
        '    End Set
        'End Property
        'Public Property SignsMaximumDeductible As String 'CoverageDetail; may need limit formatting
        '    Get
        '        Return _SignsMaximumDeductible
        '    End Get
        '    Set(value As String)
        '        _SignsMaximumDeductible = value
        '    End Set
        'End Property
        'Public Property SignsMinimumDeductible As String 'CoverageDetail; may need limit formatting
        '    Get
        '        Return _SignsMinimumDeductible
        '    End Get
        '    Set(value As String)
        '        _SignsMinimumDeductible = value
        '    End Set
        'End Property
        'Public Property SignsValuationMethodTypeId As String 'CoverageDetail; static data
        '    Get
        '        Return _SignsValuationMethodTypeId
        '    End Get
        '    Set(value As String)
        '        _SignsValuationMethodTypeId = value
        '    End Set
        'End Property
        'Public Property SignsDeductibleId As String 'static data
        '    Get
        '        Return _SignsDeductibleId
        '    End Get
        '    Set(value As String)
        '        _SignsDeductibleId = value
        '    End Set
        'End Property
        'Public Property SignsQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_SignsQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _SignsQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_SignsQuotedPremium)
        '    End Set
        'End Property
        'Public ReadOnly Property SignsBuildingTotalLimit As String 'won't use private variable; just ReadOnly prop... should equal sum of Coverage.ManualLimitAmount for building scheduled/unscheduled signs; moved to RiskLevel object 7/23/2018
        '    Get
        '        Dim tot As String = "0"
        '        If Locations IsNot Nothing AndAlso Locations.Count > 0 Then 'updated 7/19/2018 from private variable to public property
        '            For Each l As QuickQuoteLocation In Locations
        '                If l.Buildings IsNot Nothing AndAlso l.Buildings.Count > 0 Then
        '                    For Each b As QuickQuoteBuilding In l.Buildings
        '                        If b.ScheduledSigns IsNot Nothing AndAlso b.ScheduledSigns.Count > 0 Then
        '                            For Each ss As QuickQuoteScheduledSign In b.ScheduledSigns
        '                                tot = qqHelper.getSum(tot, ss.Limit)
        '                            Next
        '                        End If
        '                        tot = qqHelper.getSum(tot, b.UnscheduledSignsLimit)
        '                    Next
        '                End If
        '            Next
        '        End If
        '        qqHelper.ConvertToLimitFormat(tot)

        '        Return tot
        '    End Get
        'End Property
        'Public ReadOnly Property SignsBuildingTotalQuotedPremium As String 'won't use private variable; just ReadOnly prop... should equal sum of Coverage.FullTermPremium for building scheduled/unscheduled signs; moved to RiskLevel object 7/23/2018
        '    Get
        '        Dim tot As String = "0"
        '        If Locations IsNot Nothing AndAlso Locations.Count > 0 Then 'updated 7/19/2018 from private variable to public property
        '            For Each l As QuickQuoteLocation In Locations
        '                If l.Buildings IsNot Nothing AndAlso l.Buildings.Count > 0 Then
        '                    For Each b As QuickQuoteBuilding In l.Buildings
        '                        If b.ScheduledSigns IsNot Nothing AndAlso b.ScheduledSigns.Count > 0 Then
        '                            For Each ss As QuickQuoteScheduledSign In b.ScheduledSigns
        '                                tot = qqHelper.getSum(tot, ss.QuotedPremium)
        '                            Next
        '                        End If
        '                        tot = qqHelper.getSum(tot, b.UnscheduledSignsQuotedPremium)
        '                    Next
        '                End If
        '            Next
        '        End If
        '        qqHelper.ConvertToQuotedPremiumFormat(tot)

        '        Return tot
        '    End Get
        'End Property
        'Public Property SignsAnyOneLossCatastropheLimit As String 'note: cov also has CoverageBasisTypeId set to 1
        '    Get
        '        Return _SignsAnyOneLossCatastropheLimit
        '    End Get
        '    Set(value As String)
        '        _SignsAnyOneLossCatastropheLimit = value
        '        qqHelper.ConvertToLimitFormat(_SignsAnyOneLossCatastropheLimit)
        '    End Set
        'End Property
        'Public Property SignsAnyOneLossCatastropheQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_SignsAnyOneLossCatastropheQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _SignsAnyOneLossCatastropheQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_SignsAnyOneLossCatastropheQuotedPremium)
        '    End Set
        'End Property
        Public Property ContractorsEquipmentCatastropheLimit As String
            Get
                Return _ContractorsEquipmentCatastropheLimit
            End Get
            Set(value As String)
                _ContractorsEquipmentCatastropheLimit = value
                qqHelper.ConvertToLimitFormat(_ContractorsEquipmentCatastropheLimit)
            End Set
        End Property
        Public Property ContractorsEquipmentCatastropheQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_ContractorsEquipmentCatastropheQuotedPremium)
            End Get
            Set(value As String)
                _ContractorsEquipmentCatastropheQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_ContractorsEquipmentCatastropheQuotedPremium)
            End Set

        End Property
        Public Property EmployeeTheftLimit As String 'note: cov also has CoverageBasisTypeId 1
            Get
                Return _EmployeeTheftLimit
            End Get
            Set(value As String)
                _EmployeeTheftLimit = value
                qqHelper.ConvertToLimitFormat(_EmployeeTheftLimit)
            End Set
        End Property
        Public Property EmployeeTheftDeductibleId As String 'static data; note: proposal doesn't currently show values that weren't used for VR (less than 500); full list: 0=N/A, 2=100, 4=250, 8=500, 9=1,000, 15=2,500, 17=10,000, 19=25,000, 20=50,000, 21=75,000, 22=100,000
            Get
                Return _EmployeeTheftDeductibleId
            End Get
            Set(value As String)
                _EmployeeTheftDeductibleId = value
            End Set
        End Property
        Public Property EmployeeTheftNumberOfRatableEmployees As String 'CoverageDetail
            Get
                Return _EmployeeTheftNumberOfRatableEmployees
            End Get
            Set(value As String)
                _EmployeeTheftNumberOfRatableEmployees = value
            End Set
        End Property
        Public Property EmployeeTheftNumberOfAdditionalPremises As String 'CoverageDetail
            Get
                Return _EmployeeTheftNumberOfAdditionalPremises
            End Get
            Set(value As String)
                _EmployeeTheftNumberOfAdditionalPremises = value
            End Set
        End Property
        Public Property EmployeeTheftFaithfulPerformanceOfDutyTypeId As String 'CoverageDetail; static data
            Get
                Return _EmployeeTheftFaithfulPerformanceOfDutyTypeId
            End Get
            Set(value As String)
                _EmployeeTheftFaithfulPerformanceOfDutyTypeId = value
            End Set
        End Property
        Public Property EmployeeTheftScheduledEmployeeBenefitPlans As List(Of String)
            Get
                Return _EmployeeTheftScheduledEmployeeBenefitPlans
            End Get
            Set(value As List(Of String))
                _EmployeeTheftScheduledEmployeeBenefitPlans = value
            End Set
        End Property
        Public Property EmployeeTheftIncludedPersonsOrClasses As List(Of String)
            Get
                Return _EmployeeTheftIncludedPersonsOrClasses
            End Get
            Set(value As List(Of String))
                _EmployeeTheftIncludedPersonsOrClasses = value
            End Set
        End Property
        Public Property EmployeeTheftIncludedChairpersonsAndSpecifiedCommitteeMembers As List(Of String)
            Get
                Return _EmployeeTheftIncludedChairpersonsAndSpecifiedCommitteeMembers
            End Get
            Set(value As List(Of String))
                _EmployeeTheftIncludedChairpersonsAndSpecifiedCommitteeMembers = value
            End Set
        End Property
        Public Property EmployeeTheftScheduledPartners As List(Of String)
            Get
                Return _EmployeeTheftScheduledPartners
            End Get
            Set(value As List(Of String))
                _EmployeeTheftScheduledPartners = value
            End Set
        End Property
        Public Property EmployeeTheftScheduledLLCMembers As List(Of String)
            Get
                Return _EmployeeTheftScheduledLLCMembers
            End Get
            Set(value As List(Of String))
                _EmployeeTheftScheduledLLCMembers = value
            End Set
        End Property
        Public Property EmployeeTheftScheduledNonCompensatedOfficers As List(Of String)
            Get
                Return _EmployeeTheftScheduledNonCompensatedOfficers
            End Get
            Set(value As List(Of String))
                _EmployeeTheftScheduledNonCompensatedOfficers = value
            End Set
        End Property
        Public Property EmployeeTheftExcludedPersonsOrClasses As List(Of String)
            Get
                Return _EmployeeTheftExcludedPersonsOrClasses
            End Get
            Set(value As List(Of String))
                _EmployeeTheftExcludedPersonsOrClasses = value
            End Set
        End Property
        Public Property EmployeeTheftQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_EmployeeTheftQuotedPremium)
            End Get
            Set(value As String)
                _EmployeeTheftQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_EmployeeTheftQuotedPremium)
            End Set
        End Property
        Public Property InsidePremisesTheftOfMoneyAndSecuritiesLimit As String 'note: cov also has CoverageBasisTypeId 1
            Get
                Return _InsidePremisesTheftOfMoneyAndSecuritiesLimit
            End Get
            Set(value As String)
                _InsidePremisesTheftOfMoneyAndSecuritiesLimit = value
                qqHelper.ConvertToLimitFormat(_InsidePremisesTheftOfMoneyAndSecuritiesLimit)
            End Set
        End Property
        Public Property InsidePremisesTheftOfMoneyAndSecuritiesDeductibleId As String 'static data
            Get
                Return _InsidePremisesTheftOfMoneyAndSecuritiesDeductibleId
            End Get
            Set(value As String)
                _InsidePremisesTheftOfMoneyAndSecuritiesDeductibleId = value
            End Set
        End Property
        Public Property InsidePremisesTheftOfMoneyAndSecuritiesNumberOfPremises As String 'CoverageDetail
            Get
                Return _InsidePremisesTheftOfMoneyAndSecuritiesNumberOfPremises
            End Get
            Set(value As String)
                _InsidePremisesTheftOfMoneyAndSecuritiesNumberOfPremises = value
            End Set
        End Property
        Public Property InsidePremisesTheftOfMoneyAndSecuritiesIncludeGuestsProperty As Boolean 'CoverageDetail
            Get
                Return _InsidePremisesTheftOfMoneyAndSecuritiesIncludeGuestsProperty
            End Get
            Set(value As Boolean)
                _InsidePremisesTheftOfMoneyAndSecuritiesIncludeGuestsProperty = value
            End Set
        End Property
        Public Property InsidePremisesTheftOfMoneyAndSecuritiesRequireRecordOfChecks As Boolean 'CoverageDetail
            Get
                Return _InsidePremisesTheftOfMoneyAndSecuritiesRequireRecordOfChecks
            End Get
            Set(value As Boolean)
                _InsidePremisesTheftOfMoneyAndSecuritiesRequireRecordOfChecks = value
            End Set
        End Property
        Public Property InsidePremisesTheftOfMoneyAndSecuritiesQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_InsidePremisesTheftOfMoneyAndSecuritiesQuotedPremium)
            End Get
            Set(value As String)
                _InsidePremisesTheftOfMoneyAndSecuritiesQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_InsidePremisesTheftOfMoneyAndSecuritiesQuotedPremium)
            End Set
        End Property
        Public Property OutsideThePremisesLimit As String 'note: cov also has CoverageBasisTypeId 1
            Get
                Return _OutsideThePremisesLimit
            End Get
            Set(value As String)
                _OutsideThePremisesLimit = value
                qqHelper.ConvertToLimitFormat(_OutsideThePremisesLimit)
            End Set
        End Property
        Public Property OutsideThePremisesDeductibleId As String 'static data
            Get
                Return _OutsideThePremisesDeductibleId
            End Get
            Set(value As String)
                _OutsideThePremisesDeductibleId = value
            End Set
        End Property
        Public Property OutsideThePremisesNumberOfPremises As String 'CoverageDetail
            Get
                Return _OutsideThePremisesNumberOfPremises
            End Get
            Set(value As String)
                _OutsideThePremisesNumberOfPremises = value
            End Set
        End Property
        Public Property OutsideThePremisesIncludeSellingPrice As Boolean 'CoverageDetail
            Get
                Return _OutsideThePremisesIncludeSellingPrice
            End Get
            Set(value As Boolean)
                _OutsideThePremisesIncludeSellingPrice = value
            End Set
        End Property
        Public Property OutsideThePremisesLimitToRobberyOnly As Boolean 'CoverageDetail
            Get
                Return _OutsideThePremisesLimitToRobberyOnly
            End Get
            Set(value As Boolean)
                _OutsideThePremisesLimitToRobberyOnly = value
            End Set
        End Property
        Public Property OutsideThePremisesRequireRecordOfChecks As Boolean 'CoverageDetail
            Get
                Return _OutsideThePremisesRequireRecordOfChecks
            End Get
            Set(value As Boolean)
                _OutsideThePremisesRequireRecordOfChecks = value
            End Set
        End Property
        Public Property OutsideThePremisesQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_OutsideThePremisesQuotedPremium)
            End Get
            Set(value As String)
                _OutsideThePremisesQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_OutsideThePremisesQuotedPremium)
            End Set
        End Property
        'added 02/03/2020 for new crime coverages
        Public Property ForgeryAlterationDeductibleId As String = ""
        Public Property ForgeryAlterationLimit As String
            Get
                Return _ForgeryAlterationLimit
            End Get
            Set
                _ForgeryAlterationLimit = Value
                qqHelper.ConvertToLimitFormat(_ForgeryAlterationLimit)
            End Set
        End Property

        Public Property ForgeryAlterationNumberOfRatableEmployees As String = ""
        Public Property ForgeryAlterationAdditionalPremises As String = ""
        Public Property ForgeryAlterationQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_ForgeryAlterationQuotedPremium)
            End Get
            Set
                _ForgeryAlterationQuotedPremium = Value
                qqHelper.ConvertToQuotedPremiumFormat(_ForgeryAlterationQuotedPremium)
            End Set
        End Property


        Public Property ComputerFraudDeductibleId As String = ""
        Public Property ComputerFraudLimit As String
            Get
                Return _ComputerFraudLimit
            End Get
            Set
                _ComputerFraudLimit = Value
                qqHelper.ConvertToLimitFormat(_ComputerFraudLimit)
            End Set
        End Property

        Public Property ComputerFraudNumberOfRatableEmployees As String = ""
        Public Property ComputerFraudAdditionalPremises As String = ""
        Public Property ComputerFraudQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_ComputerFraudQuotedPremium)
            End Get
            Set
                _ComputerFraudQuotedPremium = Value
                qqHelper.ConvertToQuotedPremiumFormat(_ComputerFraudQuotedPremium)
            End Set
        End Property

        Public Property FundsTransferFraudDeductibleId As String = ""
        Public Property FundsTransferFraudLimit As String
            Get
                Return _FundsTransferFraudLimit
            End Get
            Set
                _FundsTransferFraudLimit = Value
                qqHelper.ConvertToLimitFormat(_FundsTransferFraudLimit)
            End Set
        End Property

        Public Property FundsTransferFraudNumberOfRatableEmployees As String = ""
        Public Property FundsTransferFraudAdditionalPremises As String = ""
        Public Property FundsTransferFraudQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_FundsTransferFraudQuotedPremium)
            End Get
            Set
                _FundsTransferFraudQuotedPremium = Value
                qqHelper.ConvertToQuotedPremiumFormat(_FundsTransferFraudQuotedPremium)
            End Set
        End Property
        'Public Property HasContractorsEnhancement As Boolean
        '    Get
        '        Return _HasContractorsEnhancement
        '    End Get
        '    Set(value As Boolean)
        '        _HasContractorsEnhancement = value
        '    End Set
        'End Property
        'Public Property ContractorsEnhancementQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_ContractorsEnhancementQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _ContractorsEnhancementQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_ContractorsEnhancementQuotedPremium)
        '    End Set
        'End Property
        'Public Property CPP_CPR_ContractorsEnhancementQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_CPP_CPR_ContractorsEnhancementQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _CPP_CPR_ContractorsEnhancementQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_CPP_CPR_ContractorsEnhancementQuotedPremium)
        '    End Set
        'End Property
        'Public Property CPP_CGL_ContractorsEnhancementQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_CPP_CGL_ContractorsEnhancementQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _CPP_CGL_ContractorsEnhancementQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_CPP_CGL_ContractorsEnhancementQuotedPremium)
        '    End Set
        'End Property
        'Public Property CPP_CIM_ContractorsEnhancementQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_CPP_CIM_ContractorsEnhancementQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _CPP_CIM_ContractorsEnhancementQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_CPP_CIM_ContractorsEnhancementQuotedPremium)
        '    End Set
        'End Property
        'Public Property HasManufacturersEnhancement As Boolean
        '    Get
        '        Return _HasManufacturersEnhancement
        '    End Get
        '    Set(value As Boolean)
        '        _HasManufacturersEnhancement = value
        '    End Set
        'End Property
        'Public Property ManufacturersEnhancementQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_ManufacturersEnhancementQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _ManufacturersEnhancementQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_ManufacturersEnhancementQuotedPremium)
        '    End Set
        'End Property
        'Public Property CPP_CPR_ManufacturersEnhancementQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_CPP_CPR_ManufacturersEnhancementQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _CPP_CPR_ManufacturersEnhancementQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_CPP_CPR_ManufacturersEnhancementQuotedPremium)
        '    End Set
        'End Property
        'Public Property CPP_CGL_ManufacturersEnhancementQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_CPP_CGL_ManufacturersEnhancementQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _CPP_CGL_ManufacturersEnhancementQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_CPP_CGL_ManufacturersEnhancementQuotedPremium)
        '    End Set
        'End Property
        'Public Property HasAutoPlusEnhancement As Boolean
        '    Get
        '        Return _HasAutoPlusEnhancement
        '    End Get
        '    Set(value As Boolean)
        '        _HasAutoPlusEnhancement = value
        '    End Set
        'End Property
        'Public Property AutoPlusEnhancement_QuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_AutoPlusEnhancement_QuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _AutoPlusEnhancement_QuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_AutoPlusEnhancement_QuotedPremium)
        '    End Set
        'End Property
        Public Property ScheduledGolfCourses As List(Of QuickQuoteScheduledGolfCourse)
            Get
                Return _ScheduledGolfCourses
            End Get
            Set(value As List(Of QuickQuoteScheduledGolfCourse))
                _ScheduledGolfCourses = value
            End Set
        End Property
        Public Property ScheduledGolfCartCourses As List(Of QuickQuoteScheduledGolfCartCourse)
            Get
                Return _ScheduledGolfCartCourses
            End Get
            Set(value As List(Of QuickQuoteScheduledGolfCartCourse))
                _ScheduledGolfCartCourses = value
            End Set
        End Property
        Public Property GolfCourseQuotedPremium As String 'covCodeId 21341
            Get
                Return qqHelper.QuotedPremiumFormat(_GolfCourseQuotedPremium)
            End Get
            Set(value As String)
                _GolfCourseQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_GolfCourseQuotedPremium)
            End Set
        End Property
        Public Property GolfCourseCoverageLimitId As String 'covCodeId 21341
            Get
                Return _GolfCourseCoverageLimitId
            End Get
            Set(value As String)
                _GolfCourseCoverageLimitId = value
            End Set
        End Property
        Public ReadOnly Property GolfCourseCoverageLimit As String 'added 5/8/2017; still needs update to static data values
            Get
                Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.GolfCourseCoverageLimitId, _GolfCourseCoverageLimitId)
            End Get
        End Property
        Public Property GolfCourseDeductibleId As String 'covCodeId 21341
            Get
                Return _GolfCourseDeductibleId
            End Get
            Set(value As String)
                _GolfCourseDeductibleId = value
            End Set
        End Property
        Public ReadOnly Property GolfCourseDeductible As String 'added 5/8/2017; still needs update to static data values
            Get
                Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.GolfCourseDeductibleId, _GolfCourseDeductibleId)
            End Get
        End Property
        Public Property GolfCourseCoinsuranceTypeId As String 'covCodeId 21341
            Get
                Return _GolfCourseCoinsuranceTypeId
            End Get
            Set(value As String)
                _GolfCourseCoinsuranceTypeId = value
            End Set
        End Property
        Public ReadOnly Property GolfCourseCoinsuranceType As String
            Get
                Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.GolfCourseCoinsuranceTypeId, _GolfCourseCoinsuranceTypeId)
            End Get
        End Property
        Public Property GolfCourseRate As String 'covCodeId 21341
            Get
                Return _GolfCourseRate
            End Get
            Set(value As String)
                _GolfCourseRate = value
            End Set
        End Property
        Public Property GolfCartQuotedPremium As String 'covCodeId 50121
            Get
                Return qqHelper.QuotedPremiumFormat(_GolfCartQuotedPremium)
            End Get
            Set(value As String)
                _GolfCartQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_GolfCartQuotedPremium)
            End Set
        End Property
        Public Property GolfCartManualLimitAmount As String 'covCodeId 50121
            Get
                Return _GolfCartManualLimitAmount
            End Get
            Set(value As String)
                _GolfCartManualLimitAmount = value
                qqHelper.ConvertToLimitFormat(_GolfCartManualLimitAmount)
            End Set
        End Property
        Public Property GolfCartDeductibleId As String 'covCodeId 50121
            Get
                Return _GolfCartDeductibleId
            End Get
            Set(value As String)
                _GolfCartDeductibleId = value
            End Set
        End Property
        Public ReadOnly Property GolfCartDeductible As String 'added 5/8/2017; still needs update to static data values
            Get
                Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.GolfCartDeductibleId, _GolfCartDeductibleId)
            End Get
        End Property
        Public Property GolfCartCoinsuranceTypeId As String 'covCodeId 50121
            Get
                Return _GolfCartCoinsuranceTypeId
            End Get
            Set(value As String)
                _GolfCartCoinsuranceTypeId = value
            End Set
        End Property
        Public ReadOnly Property GolfCartCoinsuranceType As String 'added 5/8/2017; still needs update to static data values
            Get
                Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.GolfCartCoinsuranceTypeId, _GolfCartCoinsuranceTypeId)
            End Get
        End Property
        Public Property GolfCartRate As String 'covCodeId 50121
            Get
                Return _GolfCartRate
            End Get
            Set(value As String)
                _GolfCartRate = value
            End Set
        End Property
        Public Property GolfCartCatastropheManualLimitAmount As String 'covCodeId 21343
            Get
                Return _GolfCartCatastropheManualLimitAmount
            End Get
            Set(value As String)
                _GolfCartCatastropheManualLimitAmount = value
                qqHelper.ConvertToLimitFormat(_GolfCartCatastropheManualLimitAmount)
            End Set
        End Property
        Public Property GolfCartDebrisRemovalCoverageLimitId As String 'covCodeId 80223
            Get
                Return _GolfCartDebrisRemovalCoverageLimitId
            End Get
            Set(value As String)
                _GolfCartDebrisRemovalCoverageLimitId = value
            End Set
        End Property
        Public ReadOnly Property GolfCartDebrisRemovalCoverageLimit As String 'added 5/9/2017; still needs update to static data values
            Get
                Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.GolfCartDebrisRemovalCoverageLimitId, _GolfCartDebrisRemovalCoverageLimitId)
            End Get
        End Property

        'added 10/15/2018 - moved from AlliedToIndividualState object
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>specific to Diamond InclusionsExclusions w/ TypeId 14</remarks>
        Public Property HasInclusionOfSoleProprietorsPartnersOfficersAndOthers As Boolean
            Get
                Return _HasInclusionOfSoleProprietorsPartnersOfficersAndOthers
            End Get
            Set(value As Boolean)
                _HasInclusionOfSoleProprietorsPartnersOfficersAndOthers = value
                If QuickQuoteHelperClass.UseComparativeRaterForLob(lob:=_LobType, quoteTransactionType:=QuoteTransactionType) <> QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then 'using <> Yes to also allow on Maybe
                    QuickQuoteHelperClass.UpdateInclusionOfSoleProprietorListFromHasFlag(_InclusionOfSoleProprietorRecords, _HasInclusionOfSoleProprietorsPartnersOfficersAndOthers, inclusionsBackup:=_InclusionOfSoleProprietorRecordsBackup, updateBackupListBeforeRemoving:=True, addInitialItemIfNeeded:=False)
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>specific to Diamond InclusionsExclusions w/ TypeId 15</remarks>
        Public Property HasWaiverOfSubrogation As Boolean
            Get
                Return _HasWaiverOfSubrogation
            End Get
            Set(value As Boolean)
                _HasWaiverOfSubrogation = value
                If QuickQuoteHelperClass.UseComparativeRaterForLob(lob:=_LobType, quoteTransactionType:=QuoteTransactionType) <> QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then 'using <> Yes to also allow on Maybe
                    QuickQuoteHelperClass.UpdateWaiverOfSubrogationRecordListFromHasFlag(_WaiverOfSubrogationRecords, _HasWaiverOfSubrogation, waiversBackup:=_WaiverOfSubrogationRecordsBackup, updateBackupListBeforeRemoving:=True, addInitialItemIfNeeded:=False)
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>specific to Diamond InclusionsExclusions w/ TypeId 15</remarks>
        Public Property WaiverOfSubrogationNumberOfWaivers As Integer
            Get
                If QuickQuoteHelperClass.UseComparativeRaterForLob(lob:=_LobType, quoteTransactionType:=QuoteTransactionType) <> QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then 'using <> Yes to also allow on Maybe
                    If _WaiverOfSubrogationRecords IsNot Nothing AndAlso _WaiverOfSubrogationRecords.Count > 0 Then
                        _WaiverOfSubrogationNumberOfWaivers = _WaiverOfSubrogationRecords.Count
                    Else
                        _WaiverOfSubrogationNumberOfWaivers = 0
                    End If
                End If
                Return _WaiverOfSubrogationNumberOfWaivers
            End Get
            Set(value As Integer)
                _WaiverOfSubrogationNumberOfWaivers = value
                If QuickQuoteHelperClass.UseComparativeRaterForLob(lob:=_LobType, quoteTransactionType:=QuoteTransactionType) <> QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then 'using <> Yes to also allow on Maybe
                    QuickQuoteHelperClass.UpdateWaiverOfSubrogationRecordListFromNumberOfWaivers(_WaiverOfSubrogationRecords, _WaiverOfSubrogationNumberOfWaivers, waiversBackup:=_WaiverOfSubrogationRecordsBackup, updateBackupListBeforeRemoving:=True)
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>specific to Diamond InclusionsExclusions w/ TypeId 15</remarks>
        Public Property WaiverOfSubrogationPremium As String
            Get
                If QuickQuoteHelperClass.UseComparativeRaterForLob(lob:=_LobType, quoteTransactionType:=QuoteTransactionType) <> QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then 'using <> Yes to also allow on Maybe Then
                    If _NeedsToUpdateWaiverOfSubrogationPremiumId = False Then
                        If _WaiverOfSubrogationRecords IsNot Nothing AndAlso _WaiverOfSubrogationRecords.Count > 0 AndAlso WaiverOfSubrogationRecords(_WaiverOfSubrogationRecords.Count - 1) IsNot Nothing Then
                            _WaiverOfSubrogationPremium = _WaiverOfSubrogationRecords(_WaiverOfSubrogationRecords.Count - 1).Premium
                        End If
                    End If
                End If
                Return _WaiverOfSubrogationPremium
            End Get
            Set(value As String)
                _WaiverOfSubrogationPremium = value
                Select Case _WaiverOfSubrogationPremium
                    Case "Not Assigned"
                        _WaiverOfSubrogationPremiumId = "0"
                    Case "0"
                        _WaiverOfSubrogationPremiumId = "1"
                    Case "25"
                        _WaiverOfSubrogationPremiumId = "2"
                    Case "50"
                        _WaiverOfSubrogationPremiumId = "3"
                    Case "75"
                        _WaiverOfSubrogationPremiumId = "4"
                    Case "100"
                        _WaiverOfSubrogationPremiumId = "5"
                    Case "150"
                        _WaiverOfSubrogationPremiumId = "6"
                    Case "200"
                        _WaiverOfSubrogationPremiumId = "7"
                    Case "250"
                        _WaiverOfSubrogationPremiumId = "8"
                    Case "300"
                        _WaiverOfSubrogationPremiumId = "9"
                    Case "400"
                        _WaiverOfSubrogationPremiumId = "10"
                    Case "500"
                        _WaiverOfSubrogationPremiumId = "11"
                    Case Else
                        _WaiverOfSubrogationPremiumId = ""
                End Select
                If QuickQuoteHelperClass.UseComparativeRaterForLob(lob:=_LobType, quoteTransactionType:=QuoteTransactionType) <> QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then 'using <> Yes to also allow on Maybe Then
                    Dim waiversUpdated As Integer = 0
                    QuickQuoteHelperClass.UpdateWaiverOfSubrogationRecordListWithPremiumId(_WaiverOfSubrogationRecords, _WaiverOfSubrogationPremiumId, waiversUpdated:=waiversUpdated)
                    If waiversUpdated > 0 Then
                        _NeedsToUpdateWaiverOfSubrogationPremiumId = False
                    Else
                        _NeedsToUpdateWaiverOfSubrogationPremiumId = True
                    End If
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>specific to Diamond InclusionsExclusions w/ TypeId 15</remarks>
        Public Property WaiverOfSubrogationPremiumId As String
            Get
                If QuickQuoteHelperClass.UseComparativeRaterForLob(lob:=_LobType, quoteTransactionType:=QuoteTransactionType) <> QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then 'using <> Yes to also allow on Maybe Then
                    If _NeedsToUpdateWaiverOfSubrogationPremiumId = False Then
                        If _WaiverOfSubrogationRecords IsNot Nothing AndAlso _WaiverOfSubrogationRecords.Count > 0 AndAlso WaiverOfSubrogationRecords(_WaiverOfSubrogationRecords.Count - 1) IsNot Nothing Then
                            _WaiverOfSubrogationPremiumId = _WaiverOfSubrogationRecords(_WaiverOfSubrogationRecords.Count - 1).PremiumId
                        End If
                    End If
                End If
                Return _WaiverOfSubrogationPremiumId
            End Get
            Set(value As String)
                _WaiverOfSubrogationPremiumId = value
                _WaiverOfSubrogationPremium = ""
                If IsNumeric(_WaiverOfSubrogationPremiumId) = True Then
                    Select Case _WaiverOfSubrogationPremiumId
                        Case "0"
                            _WaiverOfSubrogationPremium = "Not Assigned"
                        Case "1"
                            _WaiverOfSubrogationPremium = "0"
                        Case "2"
                            _WaiverOfSubrogationPremium = "25"
                        Case "3"
                            _WaiverOfSubrogationPremium = "50"
                        Case "4"
                            _WaiverOfSubrogationPremium = "75"
                        Case "5"
                            _WaiverOfSubrogationPremium = "100"
                        Case "6"
                            _WaiverOfSubrogationPremium = "150"
                        Case "7"
                            _WaiverOfSubrogationPremium = "200"
                        Case "8"
                            _WaiverOfSubrogationPremium = "250"
                        Case "9"
                            _WaiverOfSubrogationPremium = "300"
                        Case "10"
                            _WaiverOfSubrogationPremium = "400"
                        Case "11"
                            _WaiverOfSubrogationPremium = "500"
                    End Select
                End If
                If QuickQuoteHelperClass.UseComparativeRaterForLob(lob:=_LobType, quoteTransactionType:=QuoteTransactionType) <> QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then 'using <> Yes to also allow on Maybe Then
                    Dim waiversUpdated As Integer = 0
                    QuickQuoteHelperClass.UpdateWaiverOfSubrogationRecordListWithPremiumId(_WaiverOfSubrogationRecords, _WaiverOfSubrogationPremiumId, waiversUpdated:=waiversUpdated)
                    If waiversUpdated > 0 Then
                        _NeedsToUpdateWaiverOfSubrogationPremiumId = False
                    Else
                        _NeedsToUpdateWaiverOfSubrogationPremiumId = True
                    End If
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>specific to Diamond InclusionsExclusions w/ TypeId 14</remarks>
        Public Property InclusionOfSoleProprietorRecords As Generic.List(Of QuickQuoteInclusionOfSoleProprietorRecord)
            Get
                Return _InclusionOfSoleProprietorRecords
            End Get
            Set(value As Generic.List(Of QuickQuoteInclusionOfSoleProprietorRecord))
                _InclusionOfSoleProprietorRecords = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>specific to Diamond InclusionsExclusions w/ TypeId 15</remarks>
        Public Property WaiverOfSubrogationRecords As Generic.List(Of QuickQuoteWaiverOfSubrogationRecord)
            Get
                Return _WaiverOfSubrogationRecords
            End Get
            Set(value As Generic.List(Of QuickQuoteWaiverOfSubrogationRecord))
                _WaiverOfSubrogationRecords = value
            End Set
        End Property
        Public ReadOnly Property InclusionOfSoleProprietorRecordsBackup As Generic.List(Of QuickQuoteInclusionOfSoleProprietorRecord)
            Get
                Return _InclusionOfSoleProprietorRecordsBackup
            End Get
        End Property
        Public ReadOnly Property WaiverOfSubrogationRecordsBackup As Generic.List(Of QuickQuoteWaiverOfSubrogationRecord)
            Get
                Return _WaiverOfSubrogationRecordsBackup
            End Get
        End Property
        Public Property WCP_WaiverPremium As String 'covCodeId 10124 CovAddInfo w/ "Waiver Premium" in desc
            Get
                Return qqHelper.QuotedPremiumFormat(_WCP_WaiverPremium)
            End Get
            Set(value As String)
                _WCP_WaiverPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_WCP_WaiverPremium)
            End Set
        End Property
        'added 10/15/2018 - moved from AllStates object
        ''' <summary>
        ''' CGL - Blanket Waiver of Subrogation
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property BlanketWaiverOfSubrogation As String
            Get
                Return _BlanketWaiverOfSubrogation
            End Get
            Set(value As String)
                _BlanketWaiverOfSubrogation = value
            End Set
        End Property
        ''' <summary>
        ''' CGL - Blanket Waiver of Subrogation Quoted Premium
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 70017 (BOP) or 80154 (CGL)</remarks>
        Public Property BlanketWaiverOfSubrogationQuotedPremium As String
            Get
                Return _BlanketWaiverOfSubrogationQuotedPremium
            End Get
            Set(value As String)
                _BlanketWaiverOfSubrogationQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_BlanketWaiverOfSubrogationQuotedPremium)
            End Set
        End Property

#Region "Umbrella"
        Public Property UmbrellaSelfInsuredRetentionLimitId As String
        Public Property UmbrellaCoverageLimitId As String
        Public Property UmbrellaUmUimLimitId As String
        Public Property UmbrellaCoverageCalculation() As String
        Public Property UmbrellaUmUimCoverageCalculation As String
        Public Property UmbrellaLimitPremium As String
            Get
                Return _UmbrellaLimitPremium
            End Get
            Set
                _UmbrellaLimitPremium = Value
                qqHelper.ConvertToQuotedPremiumFormat(_UmbrellaLimitPremium)
            End Set
        End Property

        Public Property UmbrellaUmUimPremium As String
            Get
                Return _UmbrellaUmUimPremium
            End Get
            Set
                _UmbrellaUmUimPremium = Value
                qqHelper.ConvertToQuotedPremiumFormat(_UmbrellaUmUimPremium)
            End Set
        End Property
#End Region





        'note: may not be able to Serialize Protected Friend; updated 8/18/2018 to Friend and then Public... may need to come up w/ some solution to prevent these from being used by Devs
        Public Property BasePolicyLevelInfo As QuickQuoteLobPolicyLevelInfo_AppliedToGoverningState
            Get
                If _BasePolicyLevelInfo Is Nothing Then
                    _BasePolicyLevelInfo = New QuickQuoteLobPolicyLevelInfo_AppliedToGoverningState
                End If
                SetObjectsParent(_BasePolicyLevelInfo) 'can we use a constructor?
                Return _BasePolicyLevelInfo
            End Get
            Set(value As QuickQuoteLobPolicyLevelInfo_AppliedToGoverningState)
                _BasePolicyLevelInfo = value
                SetObjectsParent(_BasePolicyLevelInfo)
            End Set
        End Property

        Public Sub New()
            MyBase.New()
            SetDefaults()
        End Sub
        Public Sub New(Parent As Object) 'to replace multiple constructors for different objects
            Me.New()
            Me.SetParent = Parent
        End Sub
        Private Sub SetDefaults()
            _BasePolicyLevelInfo = New QuickQuoteLobPolicyLevelInfo_AppliedToGoverningState

            'PolicyLevel
            '_HasEnhancementEndorsement = False
            '_EnhancementEndorsementQuotedPremium = ""
            '_Has_PackageGL_EnhancementEndorsement = False
            '_PackageGL_EnhancementEndorsementQuotedPremium = ""
            '_Has_PackageCPR_EnhancementEndorsement = False
            '_PackageCPR_EnhancementEndorsementQuotedPremium = ""
            '_AdditionalInsuredsCount = 0
            '_AdditionalInsuredsCheckboxBOP = Nothing
            '_HasAdditionalInsuredsCheckboxBOP = False
            '_AdditionalInsuredsManualCharge = ""
            '_AdditionalInsuredsQuotedPremium = ""
            '_AdditionalInsureds = Nothing
            '_AdditionalInsuredsBackup = Nothing
            '_EmployeeBenefitsLiabilityText = ""
            '_EmployeeBenefitsLiabilityOccurrenceLimit = ""
            '_EmployeeBenefitsLiabilityOccurrenceLimitId = ""
            '_EmployeeBenefitsLiabilityQuotedPremium = ""
            '_EmployeeBenefitsLiabilityRetroactiveDate = ""
            '_EmployeeBenefitsLiabilityAggregateLimit = ""
            '_EmployeeBenefitsLiabilityDeductible = ""
            _ContractorsEquipmentScheduledCoverages = Nothing
            _ContractorsEquipmentScheduleCoinsuranceTypeId = "" 'may need static data placeholder; may be defaulted as there's just one value in dropdown (1 = per 100)
            _ContractorsEquipmentScheduleDeductibleId = "" 'may need static data placeholder
            _ContractorsEquipmentScheduleRate = ""
            _ContractorsEquipmentScheduleQuotedPremium = ""
            _ContractorsEquipmentLeasedRentedFromOthersLimit = ""
            _ContractorsEquipmentLeasedRentedFromOthersCoverageLimitId = ""
            _ContractorsEquipmentLeasedRentedFromOthersRate = ""
            _ContractorsEquipmentLeasedRentedFromOthersQuotedPremium = ""
            _ContractorsEquipmentRentalReimbursementLimit = ""
            _ContractorsEquipmentRentalReimbursementRate = ""
            _ContractorsEquipmentRentalReimbursementQuotedPremium = ""
            _ContractorsEquipmentSmallToolsEndorsementPerOccurrenceLimit = ""
            _ContractorsEquipmentSmallToolsEndorsementPerOccurrenceRate = ""
            _ContractorsEquipmentSmallToolsEndorsementPerOccurrenceDeductibleId = ""
            _ContractorsEquipmentSmallToolsEndorsementPerOccurrenceQuotedPremium = ""
            _ContractorsEquipmentSmallToolsEndorsementPerToolLimit = ""
            _ContractorsEquipmentSmallToolsEndorsementPerToolQuotedPremium = ""
            _SmallToolsLimit = ""
            _SmallToolsRate = ""
            _SmallToolsDeductibleId = ""
            _SmallToolsAdditionalInterests = Nothing
            _SmallToolsCanUseAdditionalInterestNumForAdditionalInterestReconciliation = False
            _SmallToolsQuotedPremium = ""
            _SmallToolsIsEmployeeTools = False 'small tools floater
            _SmallToolsIsToolsLeasedOrRented = False 'small tools floater
            _SmallToolsAnyOneLossCatastropheLimit = ""
            _SmallToolsAnyOneLossCatastropheQuotedPremium = ""
            _InstallationScheduledLocations = Nothing
            _InstallationQuotedPremium = ""
            _InstallationAdditionalInterests = Nothing
            _InstallationCanUseAdditionalInterestNumForAdditionalInterestReconciliation = False
            _InstallationBlanketLimit = ""
            _InstallationBlanketCoinsuranceTypeId = ""
            _InstallationBlanketDeductibleId = ""
            _InstallationBlanketRate = ""
            _InstallationBlanketQuotedPremium = ""
            _InstallationBlanketAnyOneLossCatastropheLimit = ""
            _InstallationBlanketAnyOneLossCatastropheQuotedPremium = ""
            _InstallationAdditionalDebrisRemovalExpenseLimit = ""
            _InstallationAdditionalDebrisRemovalExpenseQuotedPremium = ""
            _InstallationStorageLocationsLimit = ""
            _InstallationStorageLocationsQuotedPremium = ""
            _InstallationTransitLimit = ""
            _InstallationTransitQuotedPremium = ""
            _InstallationTestingLimit = ""
            _InstallationTestingQuotedPremium = ""
            _InstallationSewerBackupLimit = ""
            _InstallationSewerBackupDeductible = ""
            _InstallationSewerBackupQuotedPremium = ""
            _InstallationSewerBackupCatastropheLimit = ""
            _InstallationSewerBackupCatastropheQuotedPremium = ""
            _InstallationEarthquakeLimit = ""
            _InstallationEarthquakeDeductible = ""
            _InstallationEarthquakeQuotedPremium = ""
            _InstallationEarthquakeCatastropheLimit = ""
            _InstallationEarthquakeCatastropheQuotedPremium = ""
            _BusinessPersonalPropertyLimit = "" 'shown in UI Installation Coverage Extensions section, but may not be specific to Installation
            _BusinessPersonalPropertyQuotedPremium = ""
            _ScheduledPropertyItems = Nothing
            _ScheduledPropertyAdditionalInterests = Nothing
            _ScheduledPropertyCanUseAdditionalInterestNumForAdditionalInterestReconciliation = False
            _ScheduledPropertyCoinsuranceTypeId = ""
            _ScheduledPropertyDeductibleId = ""
            _ScheduledPropertyRate = ""
            _ScheduledPropertyNamedPerils = False
            _ScheduledPropertyQuotedPremium = ""
            '_ComputerCoinsuranceTypeId = "" 'cov also has CoverageBasisTypeId set to 1
            '_ComputerExcludeEarthquake = False
            '_ComputerValuationMethodTypeId = ""
            '_ComputerAdditionalInterests = Nothing
            '_ComputerCanUseAdditionalInterestNumForAdditionalInterestReconciliation = False
            '_ComputerQuotedPremium = ""
            '_ComputerAllPerilsDeductibleId = "" 'cov also has CoverageBasisTypeId set to 1
            '_ComputerAllPerilsQuotedPremium = ""
            '_ComputerEarthquakeVolcanicEruptionDeductible = "" 'cov also has CoverageBasisTypeId set to 1; example also has ApplyToWrittenPremiuim set to true
            '_ComputerEarthquakeVolcanicEruptionQuotedPremium = ""
            '_ComputerMechanicalBreakdownDeductible = "" 'cov also has CoverageBasisTypeId set to 1; example also has ApplyToWrittenPremiuim set to true
            '_ComputerMechanicalBreakdownQuotedPremium = ""
            _BuildersRiskDeductibleId = "" 'cov also has CoverageBasisTypeId set to 1
            _BuildersRiskRate = ""
            _BuildersRiskAdditionalInterests = Nothing
            _BuildersRiskCanUseAdditionalInterestNumForAdditionalInterestReconciliation = False
            _BuildersRiskQuotedPremium = ""
            _BuildersRiskScheduledLocations = Nothing
            _BuildersRiskScheduleStorageLocationsLimit = ""
            _BuildersRiskScheduleStorageLocationsQuotedPremium = ""
            _BuildersRiskScheduleTransitLimit = ""
            _BuildersRiskScheduleTransitQuotedPremium = ""
            _BuildersRiskScheduleTestingLimit = ""
            _BuildersRiskScheduleTestingQuotedPremium = ""
            '_FineArtsDeductibleCategoryTypeId = ""
            '_FineArtsRate = ""
            '_FineArtsDeductibleId = ""
            '_FineArtsQuotedPremium = ""
            '_FineArtsAdditionalInterests = Nothing
            '_FineArtsCanUseAdditionalInterestNumForAdditionalInterestReconciliation = False
            '_FineArtsBreakageMarringOrScratching = False 'renamed from _HasFineArtsBreakageMarringOrScratching
            '_FineArtsBreakageMarringOrScratchingQuotedPremium = ""
            _OwnersCargoAnyOneOwnedVehicleLimit = ""
            _OwnersCargoAnyOneOwnedVehicleDeductibleId = "" 'static data
            _OwnersCargoAnyOneOwnedVehicleRate = ""
            _OwnersCargoAnyOneOwnedVehicleDescription = ""
            _OwnersCargoAnyOneOwnedVehicleAdditionalInterests = Nothing
            _OwnersCargoAnyOneOwnedVehicleCanUseAdditionalInterestNumForAdditionalInterestReconciliation = False
            _OwnersCargoAnyOneOwnedVehicleLoadingUnloading = False
            _OwnersCargoAnyOneOwnedVehicleNamedPerils = False
            _OwnersCargoAnyOneOwnedVehicleQuotedPremium = ""
            _OwnersCargoCatastropheLimit = ""
            _OwnersCargoCatastropheQuotedPremium = ""
            _TransportationCatastropheLimit = ""
            _TransportationCatastropheDeductibleId = "" 'static data
            _TransportationCatastropheDescription = ""
            _TransportationCatastropheAdditionalInterests = Nothing
            _TransportationCatastropheCanUseAdditionalInterestNumForAdditionalInterestReconciliation = False
            _TransportationCatastropheLoadingUnloading = False
            _TransportationCatastropheNamedPerils = False
            _TransportationCatastropheQuotedPremium = ""
            _TransportationAnyOneOwnedVehicleLimit = "" 'note: cov also has CoverageBasisTypeId set to 1
            _TransportationAnyOneOwnedVehicleNumberOfVehicles = "" 'CoverageDetail
            _TransportationAnyOneOwnedVehicleRate = ""
            _TransportationAnyOneOwnedVehicleQuotedPremium = ""
            _MotorTruckCargoScheduledVehicles = Nothing
            _MotorTruckCargoScheduledVehicleAdditionalInterests = Nothing
            _MotorTruckCargoScheduledVehicleCanUseAdditionalInterestNumForAdditionalInterestReconciliation = False
            _MotorTruckCargoScheduledVehicleLoadingUnloading = False 'CoverageDetail
            _MotorTruckCargoScheduledVehicleNamedPerils = False 'CoverageDetail
            _MotorTruckCargoScheduledVehicleOperatingRadius = "" 'CoverageDetail
            _MotorTruckCargoScheduledVehicleRate = "" 'CoverageDetail
            _MotorTruckCargoScheduledVehicleDeductibleId = "" 'static data
            _MotorTruckCargoScheduledVehicleDescription = ""
            _MotorTruckCargoScheduledVehicleQuotedPremium = ""
            _MotorTruckCargoScheduledVehicleCatastropheLimit = ""
            _MotorTruckCargoScheduledVehicleCatastropheQuotedPremium = ""
            _MotorTruckCargoUnScheduledVehicleAdditionalInterests = Nothing
            _MotorTruckCargoUnScheduledVehicleCanUseAdditionalInterestNumForAdditionalInterestReconciliation = False
            _MotorTruckCargoUnScheduledVehicleLoadingUnloading = False 'CoverageDetail
            _MotorTruckCargoUnScheduledVehicleNamedPerils = False 'CoverageDetail
            _MotorTruckCargoUnScheduledVehicleOperatingRadius = "" 'CoverageDetail
            _MotorTruckCargoUnScheduledVehicleRate = "" 'CoverageDetail
            _MotorTruckCargoUnScheduledVehicleDeductibleId = "" 'static data
            _MotorTruckCargoUnScheduledVehicleDescription = ""
            _MotorTruckCargoUnScheduledVehicleQuotedPremium = ""
            _MotorTruckCargoUnScheduledVehicleCatastropheLimit = ""
            _MotorTruckCargoUnScheduledAnyVehicleLimit = ""
            _MotorTruckCargoUnScheduledNumberOfVehicles = ""
            _MotorTruckCargoUnScheduledVehicleCatastropheQuotedPremium = ""
            '_SignsAdditionalInterests = Nothing
            '_SignsCanUseAdditionalInterestNumForAdditionalInterestReconciliation = False
            '_SignsMaximumDeductible = "" 'CoverageDetail
            '_SignsMinimumDeductible = "" 'CoverageDetail
            '_SignsValuationMethodTypeId = "" 'CoverageDetail; static data
            '_SignsDeductibleId = "" 'static data
            '_SignsQuotedPremium = ""
            '_SignsAnyOneLossCatastropheLimit = "" 'note: cov also has CoverageBasisTypeId set to 1
            '_SignsAnyOneLossCatastropheQuotedPremium = ""
            _ContractorsEquipmentCatastropheLimit = ""
            _ContractorsEquipmentCatastropheQuotedPremium = ""
            _EmployeeTheftLimit = "" 'note: cov also has CoverageBasisTypeId 1
            _EmployeeTheftDeductibleId = "" 'static data
            _EmployeeTheftNumberOfRatableEmployees = "" 'CoverageDetail
            _EmployeeTheftNumberOfAdditionalPremises = "" 'CoverageDetail
            _EmployeeTheftFaithfulPerformanceOfDutyTypeId = "" 'CoverageDetail; static data
            _EmployeeTheftScheduledEmployeeBenefitPlans = Nothing
            _EmployeeTheftIncludedPersonsOrClasses = Nothing
            _EmployeeTheftIncludedChairpersonsAndSpecifiedCommitteeMembers = Nothing
            _EmployeeTheftScheduledPartners = Nothing
            _EmployeeTheftScheduledLLCMembers = Nothing
            _EmployeeTheftScheduledNonCompensatedOfficers = Nothing
            _EmployeeTheftExcludedPersonsOrClasses = Nothing
            _EmployeeTheftQuotedPremium = ""
            _InsidePremisesTheftOfMoneyAndSecuritiesLimit = "" 'note: cov also has CoverageBasisTypeId 1
            _InsidePremisesTheftOfMoneyAndSecuritiesDeductibleId = "" 'static data
            _InsidePremisesTheftOfMoneyAndSecuritiesNumberOfPremises = "" 'CoverageDetail
            _InsidePremisesTheftOfMoneyAndSecuritiesIncludeGuestsProperty = False 'CoverageDetail
            _InsidePremisesTheftOfMoneyAndSecuritiesRequireRecordOfChecks = False 'CoverageDetail
            _InsidePremisesTheftOfMoneyAndSecuritiesQuotedPremium = ""
            _OutsideThePremisesLimit = "" 'note: cov also has CoverageBasisTypeId 1
            _OutsideThePremisesDeductibleId = "" 'static data
            _OutsideThePremisesNumberOfPremises = "" 'CoverageDetail
            _OutsideThePremisesIncludeSellingPrice = False 'CoverageDetail
            _OutsideThePremisesLimitToRobberyOnly = False 'CoverageDetail
            _OutsideThePremisesRequireRecordOfChecks = False 'CoverageDetail
            _OutsideThePremisesQuotedPremium = ""

            _ForgeryAlterationLimit = ""
            _ForgeryAlterationQuotedPremium = ""
            _ComputerFraudLimit = ""
            _ComputerFraudQuotedPremium = ""
            _FundsTransferFraudLimit = ""
            _FundsTransferFraudQuotedPremium = ""
            '_HasContractorsEnhancement = False
            '_ContractorsEnhancementQuotedPremium = ""
            '_CPP_CPR_ContractorsEnhancementQuotedPremium = ""
            '_CPP_CGL_ContractorsEnhancementQuotedPremium = ""
            '_CPP_CIM_ContractorsEnhancementQuotedPremium = ""
            '_HasManufacturersEnhancement = False
            '_ManufacturersEnhancementQuotedPremium = ""
            '_CPP_CPR_ManufacturersEnhancementQuotedPremium = ""
            '_CPP_CGL_ManufacturersEnhancementQuotedPremium = ""
            '_HasAutoPlusEnhancement = False
            '_AutoPlusEnhancement_QuotedPremium = ""
            _ScheduledGolfCourses = Nothing
            _ScheduledGolfCartCourses = Nothing
            _GolfCourseQuotedPremium = "" 'covCodeId 21341
            _GolfCourseCoverageLimitId = "" 'covCodeId 21341
            _GolfCourseDeductibleId = "" 'covCodeId 21341
            _GolfCourseCoinsuranceTypeId = "" 'covCodeId 21341
            _GolfCourseRate = "" 'covCodeId 21341
            _GolfCartQuotedPremium = "" 'covCodeId 50121
            _GolfCartManualLimitAmount = "" 'covCodeId 50121
            _GolfCartDeductibleId = "" 'covCodeId 50121
            _GolfCartCoinsuranceTypeId = "" 'covCodeId 50121
            _GolfCartRate = "" 'covCodeId 50121
            _GolfCartCatastropheManualLimitAmount = "" 'covCodeId 21343
            _GolfCartDebrisRemovalCoverageLimitId = "" 'covCodeId 80223

            'added 10/15/2018 - moved from AlliedToIndividualState object
            _HasInclusionOfSoleProprietorsPartnersOfficersAndOthers = False
            _HasWaiverOfSubrogation = False
            _WaiverOfSubrogationNumberOfWaivers = 0
            _WaiverOfSubrogationPremium = ""
            _WaiverOfSubrogationPremiumId = ""
            _NeedsToUpdateWaiverOfSubrogationPremiumId = False
            _InclusionOfSoleProprietorRecords = Nothing
            _WaiverOfSubrogationRecords = Nothing
            _InclusionOfSoleProprietorRecordsBackup = Nothing
            _WaiverOfSubrogationRecordsBackup = Nothing
            _WCP_WaiverPremium = "" 'covCodeId 10124 CovAddInfo w/ "Waiver Premium" in desc
            'added 10/15/2018 - moved from AllStates object
            _BlanketWaiverOfSubrogation = 0  ' 0 = None; 1 = CGL1004; 2 = CGL1002; 3/5/2015 note: may need to be updated to empty string; now use this for 3 = CAP and 4 = WCP
            _BlanketWaiverOfSubrogationQuotedPremium = ""

            _QuoteEffectiveDate = ""
            _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.None
            _LobType = QuickQuoteObject.QuickQuoteLobType.None

        End Sub
        'Protected Friend Function Get_AdditionalInsuredsCount_Variable() As Integer
        '    Return _AdditionalInsuredsCount
        'End Function
        'Protected Friend Sub Set_AdditionalInsuredsCount_Variable(ByVal addlInsCount As Integer)
        '    _AdditionalInsuredsCount = addlInsCount
        'End Sub
        'Protected Friend Function Get_AdditionalInsuredsManualCharge_Variable() As String
        '    Return _AdditionalInsuredsManualCharge
        'End Function
        'Protected Friend Sub Set_AdditionalInsuredsManualCharge_Variable(ByVal addlInsManChg As String)
        '    _AdditionalInsuredsManualCharge = addlInsManChg
        'End Sub
        'Protected Friend Sub Set_AdditionalInsuredsBackup_Variable(ByVal addlInsBkp As List(Of QuickQuoteAdditionalInsured))
        '    _AdditionalInsuredsBackup = addlInsBkp
        'End Sub
        'Protected Friend Sub Set_HasInclusionOfSoleProprietorsPartnersOfficersAndOthers_Variable(ByVal hasIt As Boolean)
        '    _HasInclusionOfSoleProprietorsPartnersOfficersAndOthers = hasIt
        'End Sub

        'added 10/15/2018 - moved from AlliedToIndividualState object
        Protected Friend Sub Set_HasInclusionOfSoleProprietorsPartnersOfficersAndOthers_Variable(ByVal hasIt As Boolean)
            _HasInclusionOfSoleProprietorsPartnersOfficersAndOthers = hasIt
        End Sub
        Protected Friend Sub Set_HasWaiverOfSubrogation_Variable(ByVal hasIt As Boolean)
            _HasWaiverOfSubrogation = hasIt
        End Sub
        Protected Friend Sub Set_WaiverOfSubrogationNumberOfWaivers_Variable(ByVal num As Integer)
            _WaiverOfSubrogationNumberOfWaivers = num
        End Sub
        Protected Friend Function Get_WaiverOfSubrogationNumberOfWaivers_Variable() As Integer
            Return _WaiverOfSubrogationNumberOfWaivers
        End Function
        Protected Friend Function Get_NeedsToUpdateWaiverOfSubrogationPremiumId_Variable() As Boolean
            Return _NeedsToUpdateWaiverOfSubrogationPremiumId
        End Function
        Protected Friend Function Get_WaiverOfSubrogationPremiumId_Variable() As String
            Return _WaiverOfSubrogationPremiumId
        End Function
        Protected Friend Sub Set_WaiverOfSubrogationPremiumId_Variable(ByVal premId As String) '7/19/2018 note: same as Property but without logic to set all in list
            _WaiverOfSubrogationPremiumId = premId
            _WaiverOfSubrogationPremium = ""
            If IsNumeric(_WaiverOfSubrogationPremiumId) = True Then
                Select Case _WaiverOfSubrogationPremiumId
                    Case "0"
                        _WaiverOfSubrogationPremium = "Not Assigned"
                    Case "1"
                        _WaiverOfSubrogationPremium = "0"
                    Case "2"
                        _WaiverOfSubrogationPremium = "25"
                    Case "3"
                        _WaiverOfSubrogationPremium = "50"
                    Case "4"
                        _WaiverOfSubrogationPremium = "75"
                    Case "5"
                        _WaiverOfSubrogationPremium = "100"
                    Case "6"
                        _WaiverOfSubrogationPremium = "150"
                    Case "7"
                        _WaiverOfSubrogationPremium = "200"
                    Case "8"
                        _WaiverOfSubrogationPremium = "250"
                    Case "9"
                        _WaiverOfSubrogationPremium = "300"
                    Case "10"
                        _WaiverOfSubrogationPremium = "400"
                    Case "11"
                        _WaiverOfSubrogationPremium = "500"
                End Select
            End If
        End Sub
        Protected Friend Sub Set_InclusionOfSoleProprietorRecordsBackup_Variable(ByVal incs As List(Of QuickQuoteInclusionOfSoleProprietorRecord))
            _InclusionOfSoleProprietorRecordsBackup = incs
        End Sub
        Protected Friend Sub Set_WaiverOfSubrogationRecordsBackup_Variable(ByVal ws As List(Of QuickQuoteWaiverOfSubrogationRecord))
            _WaiverOfSubrogationRecordsBackup = ws
        End Sub

        Public Overrides Function ToString() As String
            Dim str As String = ""
            If Me IsNot Nothing Then


            Else
                str = "Nothing"
            End If
            Return str
        End Function

        'for _EffectiveDate and _QuoteTransactionType variables that were being used by various QuickQuoteObject Properties; could eventually update to pull from Parent Quote
        Public Function QuoteEffectiveDate() As String
            Dim effDate As String = ""

            effDate = _QuoteEffectiveDate

            Return effDate
        End Function
        Public Function QuoteTransactionType() As QuickQuoteObject.QuickQuoteTransactionType
            Dim tranType As QuickQuoteObject.QuickQuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote

            tranType = _QuoteTransactionType

            Return tranType
        End Function
        Public Function LobType() As QuickQuoteObject.QuickQuoteLobType
            Dim lob As QuickQuoteObject.QuickQuoteLobType = QuickQuoteObject.QuickQuoteLobType.None

            lob = _LobType

            Return lob
        End Function
        Protected Friend Sub Set_QuoteTransactionType(ByVal qqTranType As QuickQuoteObject.QuickQuoteTransactionType)
            _QuoteTransactionType = qqTranType
        End Sub
        Protected Friend Sub Set_QuoteEffectiveDate(ByVal effDate As String)
            _QuoteEffectiveDate = effDate
        End Sub
        Protected Friend Sub Set_LobType(ByVal lob As QuickQuoteObject.QuickQuoteLobType)
            _LobType = lob
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls
        Private _ForgeryAlterationLimit As String
        Private _ForgeryAlterationQuotedPremium As String
        Private _ComputerFraudLimit As String
        Private _ComputerFraudQuotedPremium As String
        Private _FundsTransferFraudLimit As String
        Private _FundsTransferFraudQuotedPremium As String
        Private _UmbrellaLimitPremium As String
        Private _UmbrellaUmUimPremium As String


        ' IDisposable
        'Protected Overridable Sub Dispose(disposing As Boolean)
        'updated w/ QuickQuoteBaseObject inheritance
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).


                    'PolicyLevel
                    '_HasEnhancementEndorsement = Nothing
                    'qqHelper.DisposeString(_EnhancementEndorsementQuotedPremium)
                    '_Has_PackageGL_EnhancementEndorsement = Nothing
                    'qqHelper.DisposeString(_PackageGL_EnhancementEndorsementQuotedPremium)
                    '_Has_PackageCPR_EnhancementEndorsement = Nothing
                    'qqHelper.DisposeString(_PackageCPR_EnhancementEndorsementQuotedPremium)
                    '_AdditionalInsuredsCount = Nothing
                    '_AdditionalInsuredsCheckboxBOP = Nothing
                    '_HasAdditionalInsuredsCheckboxBOP = Nothing
                    'qqHelper.DisposeString(_AdditionalInsuredsManualCharge)
                    'qqHelper.DisposeString(_AdditionalInsuredsQuotedPremium)
                    'If _AdditionalInsureds IsNot Nothing Then
                    '    If _AdditionalInsureds.Count > 0 Then
                    '        For Each ai As QuickQuoteAdditionalInsured In _AdditionalInsureds
                    '            ai.Dispose()
                    '            ai = Nothing
                    '        Next
                    '        _AdditionalInsureds.Clear()
                    '    End If
                    '    _AdditionalInsureds = Nothing
                    'End If
                    'If _AdditionalInsuredsBackup IsNot Nothing Then
                    '    If _AdditionalInsuredsBackup.Count > 0 Then
                    '        For Each ai As QuickQuoteAdditionalInsured In _AdditionalInsuredsBackup
                    '            ai.Dispose()
                    '            ai = Nothing
                    '        Next
                    '        _AdditionalInsuredsBackup.Clear()
                    '    End If
                    '    _AdditionalInsuredsBackup = Nothing
                    'End If
                    'qqHelper.DisposeString(_EmployeeBenefitsLiabilityText)
                    'qqHelper.DisposeString(_EmployeeBenefitsLiabilityOccurrenceLimit)
                    'qqHelper.DisposeString(_EmployeeBenefitsLiabilityOccurrenceLimitId)
                    'qqHelper.DisposeString(_EmployeeBenefitsLiabilityQuotedPremium)
                    'qqHelper.DisposeString(_EmployeeBenefitsLiabilityRetroactiveDate)
                    'qqHelper.DisposeString(_EmployeeBenefitsLiabilityAggregateLimit)
                    'qqHelper.DisposeString(_EmployeeBenefitsLiabilityDeductible)
                    If _ContractorsEquipmentScheduledCoverages IsNot Nothing Then
                        If _ContractorsEquipmentScheduledCoverages.Count > 0 Then
                            For Each c As QuickQuoteContractorsEquipmentScheduledCoverage In _ContractorsEquipmentScheduledCoverages
                                c.Dispose()
                                c = Nothing
                            Next
                            _ContractorsEquipmentScheduledCoverages.Clear()
                        End If
                        _ContractorsEquipmentScheduledCoverages = Nothing
                    End If
                    qqHelper.DisposeString(_ContractorsEquipmentScheduleCoinsuranceTypeId) 'may need static data placeholder; may be defaulted as there's just one value in dropdown (1 = per 100)
                    qqHelper.DisposeString(_ContractorsEquipmentScheduleDeductibleId) 'may need static data placeholder
                    qqHelper.DisposeString(_ContractorsEquipmentScheduleRate)
                    qqHelper.DisposeString(_ContractorsEquipmentScheduleQuotedPremium)
                    qqHelper.DisposeString(_ContractorsEquipmentLeasedRentedFromOthersLimit)
                    qqHelper.DisposeString(_ContractorsEquipmentLeasedRentedFromOthersCoverageLimitId)
                    qqHelper.DisposeString(_ContractorsEquipmentLeasedRentedFromOthersRate)
                    qqHelper.DisposeString(_ContractorsEquipmentLeasedRentedFromOthersQuotedPremium)
                    qqHelper.DisposeString(_ContractorsEquipmentRentalReimbursementLimit)
                    qqHelper.DisposeString(_ContractorsEquipmentRentalReimbursementRate)
                    qqHelper.DisposeString(_ContractorsEquipmentRentalReimbursementQuotedPremium)
                    qqHelper.DisposeString(_ContractorsEquipmentSmallToolsEndorsementPerOccurrenceLimit)
                    qqHelper.DisposeString(_ContractorsEquipmentSmallToolsEndorsementPerOccurrenceRate)
                    qqHelper.DisposeString(_ContractorsEquipmentSmallToolsEndorsementPerOccurrenceDeductibleId)
                    qqHelper.DisposeString(_ContractorsEquipmentSmallToolsEndorsementPerOccurrenceQuotedPremium)
                    qqHelper.DisposeString(_ContractorsEquipmentSmallToolsEndorsementPerToolLimit)
                    qqHelper.DisposeString(_ContractorsEquipmentSmallToolsEndorsementPerToolQuotedPremium)
                    qqHelper.DisposeString(_SmallToolsLimit)
                    qqHelper.DisposeString(_SmallToolsRate)
                    qqHelper.DisposeString(_SmallToolsDeductibleId)
                    qqHelper.DisposeAdditionalInterests(_SmallToolsAdditionalInterests)
                    _SmallToolsCanUseAdditionalInterestNumForAdditionalInterestReconciliation = Nothing
                    qqHelper.DisposeString(_SmallToolsQuotedPremium)
                    _SmallToolsIsEmployeeTools = Nothing 'small tools floater
                    _SmallToolsIsToolsLeasedOrRented = Nothing 'small tools floater
                    qqHelper.DisposeString(_SmallToolsAnyOneLossCatastropheLimit)
                    qqHelper.DisposeString(_SmallToolsAnyOneLossCatastropheQuotedPremium)
                    If _InstallationScheduledLocations IsNot Nothing Then
                        If _InstallationScheduledLocations.Count > 0 Then
                            For Each isl As QuickQuoteInstallationScheduledLocation In _InstallationScheduledLocations
                                isl.Dispose()
                                isl = Nothing
                            Next
                            _InstallationScheduledLocations.Clear()
                        End If
                        _InstallationScheduledLocations = Nothing
                    End If
                    qqHelper.DisposeString(_InstallationQuotedPremium)
                    qqHelper.DisposeAdditionalInterests(_InstallationAdditionalInterests)
                    _InstallationCanUseAdditionalInterestNumForAdditionalInterestReconciliation = Nothing
                    qqHelper.DisposeString(_InstallationBlanketLimit)
                    qqHelper.DisposeString(_InstallationBlanketCoinsuranceTypeId)
                    qqHelper.DisposeString(_InstallationBlanketDeductibleId)
                    qqHelper.DisposeString(_InstallationBlanketRate)
                    qqHelper.DisposeString(_InstallationBlanketQuotedPremium)
                    qqHelper.DisposeString(_InstallationBlanketAnyOneLossCatastropheLimit)
                    qqHelper.DisposeString(_InstallationBlanketAnyOneLossCatastropheQuotedPremium)
                    qqHelper.DisposeString(_InstallationAdditionalDebrisRemovalExpenseLimit)
                    qqHelper.DisposeString(_InstallationAdditionalDebrisRemovalExpenseQuotedPremium)
                    qqHelper.DisposeString(_InstallationStorageLocationsLimit)
                    qqHelper.DisposeString(_InstallationStorageLocationsQuotedPremium)
                    qqHelper.DisposeString(_InstallationTransitLimit)
                    qqHelper.DisposeString(_InstallationTransitQuotedPremium)
                    qqHelper.DisposeString(_InstallationTestingLimit)
                    qqHelper.DisposeString(_InstallationTestingQuotedPremium)
                    qqHelper.DisposeString(_InstallationSewerBackupLimit)
                    qqHelper.DisposeString(_InstallationSewerBackupDeductible)
                    qqHelper.DisposeString(_InstallationSewerBackupQuotedPremium)
                    qqHelper.DisposeString(_InstallationSewerBackupCatastropheLimit)
                    qqHelper.DisposeString(_InstallationSewerBackupCatastropheQuotedPremium)
                    qqHelper.DisposeString(_InstallationEarthquakeLimit)
                    qqHelper.DisposeString(_InstallationEarthquakeDeductible)
                    qqHelper.DisposeString(_InstallationEarthquakeQuotedPremium)
                    qqHelper.DisposeString(_InstallationEarthquakeCatastropheLimit)
                    qqHelper.DisposeString(_InstallationEarthquakeCatastropheQuotedPremium)
                    qqHelper.DisposeString(_BusinessPersonalPropertyLimit) 'shown in UI Installation Coverage Extensions section, but may not be specific to Installation
                    qqHelper.DisposeString(_BusinessPersonalPropertyQuotedPremium)
                    If _ScheduledPropertyItems IsNot Nothing Then
                        If _ScheduledPropertyItems.Count > 0 Then
                            For Each sp As QuickQuoteScheduledPropertyItem In _ScheduledPropertyItems
                                sp.Dispose()
                                sp = Nothing
                            Next
                            _ScheduledPropertyItems.Clear()
                        End If
                        _ScheduledPropertyItems = Nothing
                    End If
                    qqHelper.DisposeAdditionalInterests(_ScheduledPropertyAdditionalInterests)
                    _ScheduledPropertyCanUseAdditionalInterestNumForAdditionalInterestReconciliation = Nothing
                    qqHelper.DisposeString(_ScheduledPropertyCoinsuranceTypeId)
                    qqHelper.DisposeString(_ScheduledPropertyDeductibleId)
                    qqHelper.DisposeString(_ScheduledPropertyRate)
                    _ScheduledPropertyNamedPerils = Nothing
                    qqHelper.DisposeString(_ScheduledPropertyQuotedPremium)
                    'qqHelper.DisposeString(_ComputerCoinsuranceTypeId) 'cov also has CoverageBasisTypeId set to 1
                    '_ComputerExcludeEarthquake = Nothing
                    'qqHelper.DisposeString(_ComputerValuationMethodTypeId)
                    'qqHelper.DisposeAdditionalInterests(_ComputerAdditionalInterests)
                    '_ComputerCanUseAdditionalInterestNumForAdditionalInterestReconciliation = Nothing
                    'qqHelper.DisposeString(_ComputerQuotedPremium)
                    'qqHelper.DisposeString(_ComputerAllPerilsDeductibleId) 'cov also has CoverageBasisTypeId set to 1
                    'qqHelper.DisposeString(_ComputerAllPerilsQuotedPremium)
                    'qqHelper.DisposeString(_ComputerEarthquakeVolcanicEruptionDeductible) 'cov also has CoverageBasisTypeId set to 1; example also has ApplyToWrittenPremiuim set to true
                    'qqHelper.DisposeString(_ComputerEarthquakeVolcanicEruptionQuotedPremium)
                    'qqHelper.DisposeString(_ComputerMechanicalBreakdownDeductible) 'cov also has CoverageBasisTypeId set to 1; example also has ApplyToWrittenPremiuim set to true
                    'qqHelper.DisposeString(_ComputerMechanicalBreakdownQuotedPremium)
                    qqHelper.DisposeString(_BuildersRiskDeductibleId) 'cov also has CoverageBasisTypeId set to 1
                    qqHelper.DisposeString(_BuildersRiskRate)
                    qqHelper.DisposeAdditionalInterests(_BuildersRiskAdditionalInterests)
                    _BuildersRiskCanUseAdditionalInterestNumForAdditionalInterestReconciliation = Nothing
                    qqHelper.DisposeString(_BuildersRiskQuotedPremium)
                    If _BuildersRiskScheduledLocations IsNot Nothing Then
                        If _BuildersRiskScheduledLocations.Count > 0 Then
                            For Each sl As QuickQuoteBuildersRiskScheduledLocation In _BuildersRiskScheduledLocations
                                sl.Dispose()
                                sl = Nothing
                            Next
                            _BuildersRiskScheduledLocations.Clear()
                        End If
                        _BuildersRiskScheduledLocations = Nothing
                    End If
                    qqHelper.DisposeString(_BuildersRiskScheduleStorageLocationsLimit)
                    qqHelper.DisposeString(_BuildersRiskScheduleStorageLocationsQuotedPremium)
                    qqHelper.DisposeString(_BuildersRiskScheduleTransitLimit)
                    qqHelper.DisposeString(_BuildersRiskScheduleTransitQuotedPremium)
                    qqHelper.DisposeString(_BuildersRiskScheduleTestingLimit)
                    qqHelper.DisposeString(_BuildersRiskScheduleTestingQuotedPremium)
                    'qqHelper.DisposeString(_FineArtsDeductibleCategoryTypeId)
                    'qqHelper.DisposeString(_FineArtsRate)
                    'qqHelper.DisposeString(_FineArtsDeductibleId)
                    'qqHelper.DisposeString(_FineArtsQuotedPremium)
                    'qqHelper.DisposeAdditionalInterests(_FineArtsAdditionalInterests)
                    '_FineArtsCanUseAdditionalInterestNumForAdditionalInterestReconciliation = Nothing
                    '_FineArtsBreakageMarringOrScratching = Nothing 'renamed from _HasFineArtsBreakageMarringOrScratching
                    'qqHelper.DisposeString(_FineArtsBreakageMarringOrScratchingQuotedPremium)
                    qqHelper.DisposeString(_OwnersCargoAnyOneOwnedVehicleLimit)
                    qqHelper.DisposeString(_OwnersCargoAnyOneOwnedVehicleDeductibleId) 'static data
                    qqHelper.DisposeString(_OwnersCargoAnyOneOwnedVehicleRate)
                    qqHelper.DisposeString(_OwnersCargoAnyOneOwnedVehicleDescription)
                    qqHelper.DisposeAdditionalInterests(_OwnersCargoAnyOneOwnedVehicleAdditionalInterests)
                    _OwnersCargoAnyOneOwnedVehicleCanUseAdditionalInterestNumForAdditionalInterestReconciliation = Nothing
                    _OwnersCargoAnyOneOwnedVehicleLoadingUnloading = Nothing
                    _OwnersCargoAnyOneOwnedVehicleNamedPerils = Nothing
                    qqHelper.DisposeString(_OwnersCargoAnyOneOwnedVehicleQuotedPremium)
                    qqHelper.DisposeString(_OwnersCargoCatastropheLimit)
                    qqHelper.DisposeString(_OwnersCargoCatastropheQuotedPremium)
                    qqHelper.DisposeString(_TransportationCatastropheLimit)
                    qqHelper.DisposeString(_TransportationCatastropheDeductibleId) 'static data
                    qqHelper.DisposeString(_TransportationCatastropheDescription)
                    qqHelper.DisposeAdditionalInterests(_TransportationCatastropheAdditionalInterests)
                    _TransportationCatastropheCanUseAdditionalInterestNumForAdditionalInterestReconciliation = Nothing
                    _TransportationCatastropheLoadingUnloading = Nothing
                    _TransportationCatastropheNamedPerils = Nothing
                    qqHelper.DisposeString(_TransportationCatastropheQuotedPremium)
                    qqHelper.DisposeString(_TransportationAnyOneOwnedVehicleLimit) 'note: cov also has CoverageBasisTypeId set to 1
                    qqHelper.DisposeString(_TransportationAnyOneOwnedVehicleNumberOfVehicles) 'CoverageDetail
                    qqHelper.DisposeString(_TransportationAnyOneOwnedVehicleRate)
                    qqHelper.DisposeString(_TransportationAnyOneOwnedVehicleQuotedPremium)
                    qqHelper.DisposeScheduledVehicles(_MotorTruckCargoScheduledVehicles)
                    qqHelper.DisposeAdditionalInterests(_MotorTruckCargoScheduledVehicleAdditionalInterests)
                    qqHelper.DisposeAdditionalInterests(_MotorTruckCargoUnScheduledVehicleAdditionalInterests)
                    _MotorTruckCargoScheduledVehicleCanUseAdditionalInterestNumForAdditionalInterestReconciliation = Nothing
                    _MotorTruckCargoScheduledVehicleLoadingUnloading = Nothing 'CoverageDetail
                    _MotorTruckCargoScheduledVehicleNamedPerils = Nothing 'CoverageDetail
                    _MotorTruckCargoUnScheduledVehicleCanUseAdditionalInterestNumForAdditionalInterestReconciliation = Nothing
                    _MotorTruckCargoUnScheduledVehicleLoadingUnloading = Nothing 'CoverageDetail
                    _MotorTruckCargoUnScheduledVehicleNamedPerils = Nothing 'CoverageDetail
                    qqHelper.DisposeString(_MotorTruckCargoScheduledVehicleOperatingRadius) 'CoverageDetail
                    qqHelper.DisposeString(_MotorTruckCargoScheduledVehicleRate) 'CoverageDetail
                    qqHelper.DisposeString(_MotorTruckCargoScheduledVehicleDeductibleId) 'static data
                    qqHelper.DisposeString(_MotorTruckCargoScheduledVehicleDescription)
                    qqHelper.DisposeString(_MotorTruckCargoScheduledVehicleQuotedPremium)
                    qqHelper.DisposeString(_MotorTruckCargoScheduledVehicleCatastropheLimit)
                    qqHelper.DisposeString(_MotorTruckCargoScheduledVehicleCatastropheQuotedPremium)

                    qqHelper.DisposeString(_MotorTruckCargoUnScheduledVehicleOperatingRadius) 'CoverageDetail
                    qqHelper.DisposeString(_MotorTruckCargoUnScheduledVehicleRate) 'CoverageDetail
                    qqHelper.DisposeString(_MotorTruckCargoUnScheduledVehicleDeductibleId) 'static data
                    qqHelper.DisposeString(_MotorTruckCargoUnScheduledVehicleDescription)
                    qqHelper.DisposeString(_MotorTruckCargoUnScheduledVehicleQuotedPremium)
                    qqHelper.DisposeString(_MotorTruckCargoUnScheduledVehicleCatastropheLimit)
                    qqHelper.DisposeString(_MotorTruckCargoUnScheduledVehicleCatastropheQuotedPremium)
                    qqHelper.DisposeString(_MotorTruckCargoUnScheduledAnyVehicleLimit)
                    qqHelper.DisposeString(_MotorTruckCargoUnScheduledNumberOfVehicles)

                    'qqHelper.DisposeAdditionalInterests(_SignsAdditionalInterests)
                    '_SignsCanUseAdditionalInterestNumForAdditionalInterestReconciliation = Nothing
                    'qqHelper.DisposeString(_SignsMaximumDeductible) 'CoverageDetail
                    'qqHelper.DisposeString(_SignsMinimumDeductible) 'CoverageDetail
                    'qqHelper.DisposeString(_SignsValuationMethodTypeId) 'CoverageDetail; static data
                    'qqHelper.DisposeString(_SignsDeductibleId) 'static data
                    'qqHelper.DisposeString(_SignsQuotedPremium)
                    'qqHelper.DisposeString(_SignsAnyOneLossCatastropheLimit) 'note: cov also has CoverageBasisTypeId set to 1
                    'qqHelper.DisposeString(_SignsAnyOneLossCatastropheQuotedPremium)
                    qqHelper.DisposeString(_ContractorsEquipmentCatastropheLimit)
                    qqHelper.DisposeString(_ContractorsEquipmentCatastropheQuotedPremium)
                    qqHelper.DisposeString(_EmployeeTheftLimit) 'note: cov also has CoverageBasisTypeId 1
                    qqHelper.DisposeString(_EmployeeTheftDeductibleId) 'static data
                    qqHelper.DisposeString(_EmployeeTheftNumberOfRatableEmployees) 'CoverageDetail
                    qqHelper.DisposeString(_EmployeeTheftNumberOfAdditionalPremises) 'CoverageDetail
                    qqHelper.DisposeString(_EmployeeTheftFaithfulPerformanceOfDutyTypeId) 'CoverageDetail; static data
                    qqHelper.DisposeStrings(_EmployeeTheftScheduledEmployeeBenefitPlans)
                    qqHelper.DisposeStrings(_EmployeeTheftIncludedPersonsOrClasses)
                    qqHelper.DisposeStrings(_EmployeeTheftIncludedChairpersonsAndSpecifiedCommitteeMembers)
                    qqHelper.DisposeStrings(_EmployeeTheftScheduledPartners)
                    qqHelper.DisposeStrings(_EmployeeTheftScheduledLLCMembers)
                    qqHelper.DisposeStrings(_EmployeeTheftScheduledNonCompensatedOfficers)
                    qqHelper.DisposeStrings(_EmployeeTheftExcludedPersonsOrClasses)
                    qqHelper.DisposeString(_EmployeeTheftQuotedPremium)
                    qqHelper.DisposeString(_InsidePremisesTheftOfMoneyAndSecuritiesLimit) 'note: cov also has CoverageBasisTypeId 1
                    qqHelper.DisposeString(_InsidePremisesTheftOfMoneyAndSecuritiesDeductibleId) 'static data
                    qqHelper.DisposeString(_InsidePremisesTheftOfMoneyAndSecuritiesNumberOfPremises) 'CoverageDetail
                    _InsidePremisesTheftOfMoneyAndSecuritiesIncludeGuestsProperty = Nothing 'CoverageDetail
                    _InsidePremisesTheftOfMoneyAndSecuritiesRequireRecordOfChecks = Nothing 'CoverageDetail
                    qqHelper.DisposeString(_InsidePremisesTheftOfMoneyAndSecuritiesQuotedPremium)
                    qqHelper.DisposeString(_OutsideThePremisesLimit) 'note: cov also has CoverageBasisTypeId 1
                    qqHelper.DisposeString(_OutsideThePremisesDeductibleId) 'static data
                    qqHelper.DisposeString(_OutsideThePremisesNumberOfPremises) 'CoverageDetail
                    _OutsideThePremisesIncludeSellingPrice = Nothing 'CoverageDetail
                    _OutsideThePremisesLimitToRobberyOnly = Nothing 'CoverageDetail
                    _OutsideThePremisesRequireRecordOfChecks = Nothing 'CoverageDetail
                    qqHelper.DisposeString(_OutsideThePremisesQuotedPremium)
                    '_HasContractorsEnhancement = Nothing
                    'qqHelper.DisposeString(_ContractorsEnhancementQuotedPremium)
                    'qqHelper.DisposeString(_CPP_CPR_ContractorsEnhancementQuotedPremium)
                    'qqHelper.DisposeString(_CPP_CGL_ContractorsEnhancementQuotedPremium)
                    'qqHelper.DisposeString(_CPP_CIM_ContractorsEnhancementQuotedPremium)
                    '_HasManufacturersEnhancement = Nothing
                    'qqHelper.DisposeString(_ManufacturersEnhancementQuotedPremium)
                    'qqHelper.DisposeString(_CPP_CPR_ManufacturersEnhancementQuotedPremium)
                    'qqHelper.DisposeString(_CPP_CGL_ManufacturersEnhancementQuotedPremium)
                    '_HasAutoPlusEnhancement = Nothing
                    'qqHelper.DisposeString(_AutoPlusEnhancement_QuotedPremium)
                    If _ScheduledGolfCourses IsNot Nothing Then
                        If _ScheduledGolfCourses.Count > 0 Then
                            For Each c As QuickQuoteScheduledGolfCourse In _ScheduledGolfCourses
                                If c IsNot Nothing Then
                                    c.Dispose()
                                    c = Nothing
                                End If
                            Next
                            _ScheduledGolfCourses.Clear()
                        End If
                        _ScheduledGolfCourses = Nothing
                    End If
                    If _ScheduledGolfCartCourses IsNot Nothing Then
                        If _ScheduledGolfCartCourses.Count > 0 Then
                            For Each c As QuickQuoteScheduledGolfCartCourse In _ScheduledGolfCartCourses
                                If c IsNot Nothing Then
                                    c.Dispose()
                                    c = Nothing
                                End If
                            Next
                            _ScheduledGolfCartCourses.Clear()
                        End If
                        _ScheduledGolfCartCourses = Nothing
                    End If
                    qqHelper.DisposeString(_GolfCourseQuotedPremium) 'covCodeId 21341
                    qqHelper.DisposeString(_GolfCourseCoverageLimitId) 'covCodeId 21341
                    qqHelper.DisposeString(_GolfCourseDeductibleId) 'covCodeId 21341
                    qqHelper.DisposeString(_GolfCourseCoinsuranceTypeId) 'covCodeId 21341
                    qqHelper.DisposeString(_GolfCourseRate) 'covCodeId 21341
                    qqHelper.DisposeString(_GolfCartQuotedPremium) 'covCodeId 50121
                    qqHelper.DisposeString(_GolfCartManualLimitAmount) 'covCodeId 50121
                    qqHelper.DisposeString(_GolfCartDeductibleId) 'covCodeId 50121
                    qqHelper.DisposeString(_GolfCartCoinsuranceTypeId) 'covCodeId 50121
                    qqHelper.DisposeString(_GolfCartRate) 'covCodeId 50121
                    qqHelper.DisposeString(_GolfCartCatastropheManualLimitAmount) 'covCodeId 21343
                    qqHelper.DisposeString(_GolfCartDebrisRemovalCoverageLimitId) 'covCodeId 80223

                    'added 10/15/2018 - moved from AlliedToIndividualState object
                    _HasInclusionOfSoleProprietorsPartnersOfficersAndOthers = Nothing
                    _HasWaiverOfSubrogation = Nothing
                    _WaiverOfSubrogationNumberOfWaivers = Nothing
                    qqHelper.DisposeString(_WaiverOfSubrogationPremium)
                    qqHelper.DisposeString(_WaiverOfSubrogationPremiumId)
                    _NeedsToUpdateWaiverOfSubrogationPremiumId = Nothing
                    If _InclusionOfSoleProprietorRecords IsNot Nothing Then
                        If _InclusionOfSoleProprietorRecords.Count > 0 Then
                            For Each sp As QuickQuoteInclusionOfSoleProprietorRecord In _InclusionOfSoleProprietorRecords
                                sp.Dispose()
                                sp = Nothing
                            Next
                            _InclusionOfSoleProprietorRecords.Clear()
                        End If
                        _InclusionOfSoleProprietorRecords = Nothing
                    End If
                    If _WaiverOfSubrogationRecords IsNot Nothing Then
                        If _WaiverOfSubrogationRecords.Count > 0 Then
                            For Each w As QuickQuoteWaiverOfSubrogationRecord In _WaiverOfSubrogationRecords
                                w.Dispose()
                                w = Nothing
                            Next
                            _WaiverOfSubrogationRecords.Clear()
                        End If
                        _WaiverOfSubrogationRecords = Nothing
                    End If
                    If _InclusionOfSoleProprietorRecordsBackup IsNot Nothing Then
                        If _InclusionOfSoleProprietorRecordsBackup.Count > 0 Then
                            For Each sp As QuickQuoteInclusionOfSoleProprietorRecord In _InclusionOfSoleProprietorRecordsBackup
                                sp.Dispose()
                                sp = Nothing
                            Next
                            _InclusionOfSoleProprietorRecordsBackup.Clear()
                        End If
                        _InclusionOfSoleProprietorRecordsBackup = Nothing
                    End If
                    If _WaiverOfSubrogationRecordsBackup IsNot Nothing Then
                        If _WaiverOfSubrogationRecordsBackup.Count > 0 Then
                            For Each w As QuickQuoteWaiverOfSubrogationRecord In _WaiverOfSubrogationRecordsBackup
                                w.Dispose()
                                w = Nothing
                            Next
                            _WaiverOfSubrogationRecordsBackup.Clear()
                        End If
                        _WaiverOfSubrogationRecordsBackup = Nothing
                    End If
                    qqHelper.DisposeString(_WCP_WaiverPremium) 'covCodeId 10124 CovAddInfo w/ "Waiver Premium" in desc
                    'added 10/15/2018 - moved from AllStates object
                    qqHelper.DisposeString(_BlanketWaiverOfSubrogation)
                    qqHelper.DisposeString(_BlanketWaiverOfSubrogationQuotedPremium)

                    qqHelper.DisposeString(_QuoteEffectiveDate)
                    _QuoteTransactionType = Nothing
                    _LobType = Nothing

                    If _BasePolicyLevelInfo IsNot Nothing Then
                        _BasePolicyLevelInfo.Dispose()
                        _BasePolicyLevelInfo = Nothing
                    End If

                    MyBase.Dispose()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        'Public Sub Dispose() Implements IDisposable.Dispose
        'updated  w/ QuickQuoteBaseObject inheritance
        Public Overrides Sub Dispose() 'Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

        '''  <summary>
        '''  </summary>
        '''  <value></value>
        '''  <returns></returns>
        '''  <remarks>corresponds to Diamond coverage w/ coveragecode_id 80557</remarks>
        Public Property StopGapLimitId As String
            Get
                Return _StopGapLimitId
            End Get
            Set(value As String)
                _StopGapLimitId = value
            End Set
        End Property

        '''  <summary>
        '''  </summary>
        '''  <value></value>
        '''  <returns></returns>
        '''  <remarks>corresponds to Diamond coverage w/ coveragecode_id 80557</remarks>
        Public Property StopGapPayroll As String
            Get
                Return _StopGapPayroll
            End Get
            Set(value As String)
                _StopGapPayroll = value
            End Set
        End Property

        '''  <summary>
        '''  </summary>
        '''  <value></value>
        '''  <returns></returns>
        '''  <remarks>corresponds to Diamond coverage w/ coveragecode_id 80557</remarks>
        Public Property StopGapQuotedPremium As String
            Get
                Return _StopGapQuotedPremium
            End Get
            Set(value As String)
                _StopGapQuotedPremium = value
            End Set
        End Property

        Private _StopGapLimitId As String

        Private _StopGapPayroll As String

        Private _StopGapQuotedPremium As String

        '''  <summary>
        '''  </summary>
        '''  <value></value>
        '''  <returns></returns>
        '''  <remarks>corresponds to Diamond coverage w/ coveragecode_id 80557</remarks>
        Public Property PayrollAmount As String
            Get
                Return _PayrollAmount
            End Get
            Set(value As String)
                _PayrollAmount = value
            End Set
        End Property

        Private _PayrollAmount As String
#End Region

    End Class
End Namespace
