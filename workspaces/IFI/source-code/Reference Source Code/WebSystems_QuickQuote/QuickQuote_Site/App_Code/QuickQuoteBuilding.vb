Imports System
Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods
Imports helper = QuickQuote.CommonMethods.QuickQuoteHelperClass 'added 7/28/2015

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store building information
    ''' </summary>
    ''' <remarks>typically found under QuickQuoteLocation object (<see cref="QuickQuoteLocation"/>) as a list</remarks>
    <Serializable()>
    Public Class QuickQuoteBuilding
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass
        Dim pvHelper As New QuickQuotePropertyValuationHelperClass 'added 7/28/2015 for e2Value

        Private _effectiveDate As String '3/9/2017 - BOP stuff
        Private _PolicyId As String
        Private _PolicyImageNum As String
        Private _Description As String 'can probably use Description node
        Private _Program As String
        Private _ProgramAbbreviation As String
        Private _Classification As String
        Private _ClassCode As String
        Private _ClassificationTypeId As String
        Private _Occupancy As String
        Private _OccupancyId As String
        Private _Construction As String
        Private _ConstructionId As String
        Private _AutoIncrease As String
        Private _AutoIncreaseId As String
        Private _AutoIncreasePremium As String '3/9/2017 - BOP stuff
        Private _HasAutoIncreasePremium As Boolean '3/9/2017 - BOP stuff
        Private _PropertyDeductible As String
        Private _PropertyDeductibleId As String
        Private _Limit As String
        Private _LimitQuotedPremium As String
        Private _Valuation As String '*note 3/26/2013:  for Building cov; CoverageCodeID 165
        Private _ValuationId As String '*note 3/26/2013:  for Building cov; CoverageCodeID 165
        Private _IsAgreedValue As Boolean ' added 2/1/2016 for bug 4845 Matt A; 3/9/2017 - included in this library w/ BOP updates
        Private _IsBuildingValIncludedInBlanketRating As Boolean '*note 3/26/2013:  for Building cov; CoverageCodeID 165
        Private _HasACVRoofing As Boolean '3/9/2017 - BOP stuff
        Private _ACVRoofingQuotedPremium As String '3/9/2017 - BOP stuff
        Private _HasMineSubsidence As Boolean
        Private _MineSubsidenceQuotedPremium As String
        Private _MineSubsidence_IsDwellingStructure As Boolean 'added 11/8/2018 for CPR mine subsidence; covCodeId 20027; CheckBox: Mine Subsidence (BOP IN/IL, CPR IN/IL)
        Private _HasSprinklered As Boolean
        Private _PersonalPropertyLimit As String
        Private _PersonalPropertyLimitQuotedPremium As String
        Private _ValuationMethod As String '*note 3/26/2013:  for PersProperty cov; CoverageCodeID 21037
        Private _ValuationMethodId As String '*note 3/26/2013:  for PersProperty cov; CoverageCodeID 21037
        Private _IsValMethodIncludedInBlanketRating As Boolean '*note 3/26/2013:  for PersProperty cov; CoverageCodeID 21037

        Private _AccountsReceivableOnPremises As String
        Private _AccountsReceivableOffPremises As String
        Private _AccountsReceivableQuotedPremium As String
        Private _ValuablePapersOnPremises As String
        Private _ValuablePapersOffPremises As String
        Private _ValuablePapersQuotedPremium As String
        Private _CondoCommercialUnitOwnersLimit As String
        Private _CondoCommercialUnitOwnersLimitId As String
        Private _CondoCommercialUnitOwnersLimitQuotedPremium As String
        Private _HasOrdinanceOrLaw As Boolean
        Private _HasOrdOrLawUndamagedPortion As Boolean
        Private _OrdOrLawUndamagedPortionQuotedPremium As String
        Private _OrdOrLawDemoCostLimit As String
        Private _OrdOrLawDemoCostLimitQuotedPremium As String
        Private _OrdOrLawIncreasedCostLimit As String
        Private _OrdOrLawIncreaseCostLimitQuotedPremium As String
        Private _OrdOrLawDemoAndIncreasedCostLimit As String
        Private _OrdOrLawDemoAndIncreasedCostLimitQuotedPremium As String
        Private _HasSpoilage As Boolean
        Private _SpoilageQuotedPremium As String
        Private _SpoilagePropertyClassification As String
        Private _SpoilagePropertyClassificationId As String
        Private _SpoilageTotalLimit As String
        Private _IsSpoilageRefrigerationMaintenanceAgreement As Boolean
        Private _IsSpoilageBreakdownOrContamination As Boolean
        Private _IsSpoilagePowerOutage As Boolean

        Private _EquipmentBreakdownDeductibleMinimumRequiredByClassCode As String '3/9/2017 - BOP stuff

        Private _BuildingCoverages As Generic.List(Of QuickQuoteCoverage)

        Private _BuildingClassifications As Generic.List(Of QuickQuoteClassification)

        Private _HasBusinessMasterEnhancement As Boolean

        Private _ProtectionClassId As String
        Private _ProtectionClass As String 'added 8/30/2012 after moving input to building

        Private _AccountsReceivableOnPremisesExcessLimit As String
        Private _ValuablePapersOnPremisesExcessLimit As String

        Private _YearBuilt As String

        'Private _NumberOfSoleProprietors As String
        'Private _NumberOfCorporateOfficers As String
        'Private _NumberOfPartners As String
        Private _NumberOfOfficersAndPartnersAndInsureds As String 'combined into 1 on 7/5/2012
        Private _EmployeePayroll As String
        Private _AnnualReceipts As String

        'more stuff for App Gap 7/19/2012
        Private _SquareFeet As String

        'and more 7/20/2012; commented out 7/31/2013 to use Updates variable/property instead
        'Private _CentralHeatElectric As Boolean
        'Private _CentralHeatGas As Boolean
        'Private _CentralHeatOil As Boolean
        'Private _CentralHeatOther As Boolean
        'Private _CentralHeatOtherDescription As String
        'Private _CentralHeatUpdateTypeId As String
        'Private _CentralHeatUpdateYear As String
        'Private _ImprovementsDescription As String
        'Private _Electric100Amp As Boolean
        'Private _Electric120Amp As Boolean
        'Private _Electric200Amp As Boolean
        'Private _Electric60Amp As Boolean
        'Private _ElectricBurningUnit As Boolean
        'Private _ElectricCircuitBreaker As Boolean
        'Private _ElectricFuses As Boolean
        'Private _ElectricSpaceHeater As Boolean
        'Private _ElectricUpdateTypeId As String
        'Private _ElectricUpdateYear As String
        'Private _PlumbingCopper As Boolean
        'Private _PlumbingGalvanized As Boolean
        'Private _PlumbingPlastic As Boolean
        'Private _PlumbingUpdateTypeId As String
        'Private _PlumbingUpdateYear As String
        'Private _RoofAsphaltShingle As Boolean
        'Private _RoofMetal As Boolean
        'Private _RoofOther As Boolean
        'Private _RoofOtherDescription As String
        'Private _RoofSlate As Boolean
        'Private _RoofUpdateTypeId As String
        'Private _RoofUpdateYear As String
        'Private _RoofWood As Boolean
        'Private _SupplementalHeatBurningUnit As Boolean
        'Private _SupplementalHeatFireplace As Boolean
        'Private _SupplementalHeatFireplaceInsert As Boolean
        'Private _SupplementalHeatNA As Boolean
        'Private _SupplementalHeatSolidFuel As Boolean
        'Private _SupplementalHeatSpaceHeater As Boolean
        'Private _SupplementalHeatUpdateTypeId As String
        'Private _SupplementalHeatUpdateYear As String
        'Private _WindowsUpdateTypeId As String
        'Private _WindowsUpdateYear As String
        Private _Updates As QuickQuoteUpdatesRecord 'added 7/31/2013 to be used instead of multiple properties (created object after finding same thing on Location node)

        '8/24/2012 - updated logic (on other properties) to default updateTypeIds when needed and not provided; not using just yet

        'added 8/1/2012 for App Gap
        Private _AdditionalInterests As Generic.List(Of QuickQuoteAdditionalInterest)

        'still have to do Additional Optional Coverages (maybe; still stored in Diamond at policy level)
        'added 8/6/2012
        Private _HasBarbersProfessionalLiability As Boolean
        Private _BarbersProfessionalLiabilityFullTimeEmpNum As String
        Private _BarbersProfessionalLiabilityPartTimeEmpNum As String
        Private _HasBeauticiansProfessionalLiability As Boolean
        Private _BeauticiansProfessionalLiabilityFullTimeEmpNum As String
        Private _BeauticiansProfessionalLiabilityPartTimeEmpNum As String
        Private _HasFuneralDirectorsProfessionalLiability As Boolean
        Private _FuneralDirectorsProfessionalLiabilityEmpNum As String
        Private _HasPrintersProfessionalLiability As Boolean
        Private _PrintersProfessionalLiabilityLocNum As String
        Private _HasSelfStorageFacility As Boolean
        Private _SelfStorageFacilityLimit As String
        Private _HasVeterinariansProfessionalLiability As Boolean
        Private _VeterinariansProfessionalLiabilityEmpNum As String
        Private _HasOpticalAndHearingAidProfessionalLiability As Boolean
        Private _OpticalAndHearingAidProfessionalLiabilityEmpNum As String

        '3/9/2017 - BOP stuff
        Private _HasMotelCoverage As Boolean
        Private _MotelCoveragePerGuestLimitId As String
        Private _MotelCoveragePerGuestLimit As String
        Private _MotelCoverageSafeDepositLimitId As String
        Private _MotelCoverageSafeDepositLimit As String
        Private _MotelCoverageSafeDepositDeductibleId As String
        Private _MotelCoverageSafeDepositDeductible As String
        Private _HasPhotographyCoverage As Boolean
        Private _HasPhotographyCoverageScheduledCoverages As Boolean
        Private _PhotographyScheduledCoverages As List(Of QuickQuoteCoverage)
        Private _HasPhotographyMakeupAndHair As Boolean
        Private _HasPharmacistProfessionalLiability As Boolean
        Private _PharmacistAnnualGrossSales As String
        Private _HasApartmentBuildings As Boolean
        Private _NumberOfLocationsWithApartments As String
        Private _HasTenantAutoLegalLiability As Boolean
        Private _TenantAutoLegalLiabilityLimitOfLiabilityId As String
        Private _TenantAutoLegalLiabilityLimitOfLiability As String
        Private _TenantAutoLegalLiabilityDeductibleId As String
        Private _TenantAutoLegalLiabilityDeductible As String
        Private _HasCustomerAutoLegalLiability As Boolean
        Private _CustomerAutoLegalLiabilityLimitOfLiabilityId As String
        Private _CustomerAutoLegalLiabilityLimitOfLiability As String
        Private _CustomerAutoLegalLiabilityDeductibleId As String
        Private _CustomerAutoLegalLiabilityDeductible As String
        Private _HasFineArts As Boolean
        Private _HasResidentialCleaning As Boolean
        Private _HasLiquorLiability As Boolean
        Private _LiquorLiabilityClassCodeTypeId As String '12 = 58161 - Restaurant Includes Package Sales, 13 = 59211 - Package Sales for Consumption Off Premises
        Private _LiquorLiabilityAnnualGrossPackageSalesReceipts As String
        Private _LiquorLiabilityAnnualGrossAlcoholSalesReceipts As String
        Private _LiquorLiabilityAggregateLimit As String

        'still have to do IRPM (maybe)

        Private _ClassificationCode As QuickQuoteClassificationCode 'added 9/27/2012 for CPR
        Private _EarthquakeBuildingClassificationTypeId As String 'added 9/27/2012 for CPR

        'added 9/27/2012 for CPR
        Private _EarthquakeApplies As Boolean
        Private _CauseOfLossTypeId As String 'example 3 = Special Form Including Theft (CauseOfLossType table)
        Private _CauseOfLossType As String
        Private _DeductibleId As String 'example 8 = 500
        Private _Deductible As String
        Private _CoinsuranceTypeId As String 'example 5 = 80% (CoinsuranceType table)
        Private _CoinsuranceType As String
        'Private _ValuationMethodTypeId As String 'example 1 = Replacement Cost (commented since already has equivalent variable/property)
        'Private _ValuationMethodType As String
        Private _RatingTypeId As String 'RatingType table
        Private _RatingType As String
        Private _InflationGuardTypeId As String 'InflationGuardType table
        Private _InflationGuardType As String

        Private _ScheduledCoverages As Generic.List(Of QuickQuoteScheduledCoverage) 'added 9/27/2012 for CPR
        Private _PersPropCov_PersonalPropertyLimit As String
        Private _PersPropCov_PropertyTypeId As String
        Private _PersPropCov_PropertyType As String
        Private _PersPropCov_RiskTypeId As String
        Private _PersPropCov_RiskType As String
        Private _PersPropCov_EarthquakeApplies As Boolean
        Private _PersPropCov_RatingTypeId As String
        Private _PersPropCov_RatingType As String
        Private _PersPropCov_CauseOfLossTypeId As String
        Private _PersPropCov_CauseOfLossType As String
        Private _PersPropCov_DeductibleId As String
        Private _PersPropCov_Deductible As String
        Private _PersPropCov_CoinsuranceTypeId As String
        Private _PersPropCov_CoinsuranceType As String
        Private _PersPropCov_ValuationId As String
        Private _PersPropCov_Valuation As String
        Private _PersPropCov_QuotedPremium As String
        Private _PersPropCov_ClassificationCode As QuickQuoteClassificationCode
        Private _PersPropCov_IsAgreedValue As Boolean '3/9/2017 - included in this library w/ BOP updates
        'added 10/8/2012 for CPR
        Private _PersPropOfOthers_PersonalPropertyLimit As String
        'Private _PersPropOfOthers_PropertyTypeId As String'defaulting
        'Private _PersPropOfOthers_PropertyType As String
        Private _PersPropOfOthers_RiskTypeId As String
        Private _PersPropOfOthers_RiskType As String
        Private _PersPropOfOthers_EarthquakeApplies As Boolean
        Private _PersPropOfOthers_RatingTypeId As String
        Private _PersPropOfOthers_RatingType As String
        Private _PersPropOfOthers_CauseOfLossTypeId As String
        Private _PersPropOfOthers_CauseOfLossType As String
        Private _PersPropOfOthers_DeductibleId As String
        Private _PersPropOfOthers_Deductible As String
        Private _PersPropOfOthers_CoinsuranceTypeId As String
        Private _PersPropOfOthers_CoinsuranceType As String
        Private _PersPropOfOthers_ValuationId As String
        Private _PersPropOfOthers_Valuation As String
        Private _PersPropOfOthers_QuotedPremium As String
        Private _PersPropOfOthers_ClassificationCode As QuickQuoteClassificationCode
        'added 10/8/2012 (finished 10/9/2012) for CPR
        Private _BusinessIncomeCov_Limit As String
        Private _BusinessIncomeCov_CoinsuranceTypeId As String
        Private _BusinessIncomeCov_CoinsuranceType As String
        Private _BusinessIncomeCov_MonthlyPeriodTypeId As String
        Private _BusinessIncomeCov_MonthlyPeriodType As String
        Private _BusinessIncomeCov_BusinessIncomeTypeId As String
        Private _BusinessIncomeCov_BusinessIncomeType As String
        Private _BusinessIncomeCov_RiskTypeId As String
        Private _BusinessIncomeCov_RiskType As String
        Private _BusinessIncomeCov_EarthquakeApplies As Boolean
        Private _BusinessIncomeCov_RatingTypeId As String
        Private _BusinessIncomeCov_RatingType As String
        Private _BusinessIncomeCov_CauseOfLossTypeId As String
        Private _BusinessIncomeCov_CauseOfLossType As String
        Private _BusinessIncomeCov_QuotedPremium As String
        Private _BusinessIncomeCov_ClassificationCode As QuickQuoteClassificationCode
        'added 10/10/2012 for CPR (required for rating)
        Private _BusinessIncomeCov_WaitingPeriodTypeId As String
        Private _BusinessIncomeCov_WaitingPeriodType As String

        'added 10/9/2012 for CPR
        Private _Building_BusinessIncome_Group1_Rate As String
        Private _Building_BusinessIncome_Group2_Rate As String
        Private _PersonalProperty_Group1_Rate As String
        Private _PersonalProperty_Group2_Rate As String
        'added 10/10/2012 for CPR
        Private _Building_BusinessIncome_Group1_LossCost As String
        Private _Building_BusinessIncome_Group2_LossCost As String
        Private _PersonalProperty_Group1_LossCost As String
        Private _PersonalProperty_Group2_LossCost As String

        'added 10/10/2012 for CPR (required)
        Private _CoverageFormTypeId As String
        Private _CoverageFormType As String

        'added 10/16/2012 for CPR
        Private _CPR_Covs_TotalBuildingPremium As String

        'added 10/23/2012 for CPR
        'Private _PersPropCov_EarthquakeRateGradeTypeId As String
        'Private _PersPropOfOthers_EarthquakeRateGradeTypeId As String
        Private _PersonalProperty_EarthquakeRateGradeTypeId As String 'Diamond's PersonalPropertyRateGradeType table

        'added 11/13/2012 for CPR (appears to be required for business income EQ)
        Private _NumberOfStories As String

        'added 11/13/2012 for CPR (EQ stuff)
        Private _EarthquakeQuotedPremium As String
        Private _PersPropCov_EarthquakeQuotedPremium As String
        Private _PersPropOfOthers_EarthquakeQuotedPremium As String
        Private _BusinessIncomeCov_EarthquakeQuotedPremium As String
        'added 11/15/2012 for CPR (EQ stuff)
        Private _CPR_Covs_TotalBuilding_EQ_Premium As String
        'added 11/26/2012 for CPR
        Private _CPR_Covs_TotalBuilding_With_EQ_Premium As String
        Private _CPR_BuildingLimit_With_EQ_QuotedPremium As String
        Private _CPR_PersPropCov_With_EQ_QuotedPremium As String
        Private _CPR_PersPropOfOthers_With_EQ_QuotedPremium As String
        Private _CPR_BusinessIncomeCov_With_EQ_QuotedPremium As String
        'added 11/28/2012 for CPR
        Private _CPR_BuildingLimit_EarthquakeBuildingClassificationPercentage As String
        Private _CPR_PersPropCov_EarthquakeBuildingClassificationPercentage As String
        Private _CPR_PersPropOfOthers_EarthquakeBuildingClassificationPercentage As String
        Private _CPR_BusinessIncomeCov_EarthquakeBuildingClassificationPercentage As String 'this one may not be valid since I didn't see it in the xml
        'added 11/29/2012 for CPR
        Private _OptionalTheftDeductibleId As String
        Private _OptionalTheftDeductible As String
        Private _OptionalWindstormOrHailDeductibleId As String
        Private _OptionalWindstormOrHailDeductible As String
        Private _PersPropCov_OptionalTheftDeductibleId As String
        Private _PersPropCov_OptionalTheftDeductible As String
        Private _PersPropCov_OptionalWindstormOrHailDeductibleId As String
        Private _PersPropCov_OptionalWindstormOrHailDeductible As String
        Private _PersPropOfOthers_OptionalTheftDeductibleId As String
        Private _PersPropOfOthers_OptionalTheftDeductible As String
        Private _PersPropOfOthers_OptionalWindstormOrHailDeductibleId As String
        Private _PersPropOfOthers_OptionalWindstormOrHailDeductible As String
        'added 1/2/2013 for CPR
        Private _PersPropCov_DoesYardRateApplyTypeId As String
        Private _PersPropOfOthers_DoesYardRateApplyTypeId As String

        'added 1/29/2013 for Protection Class Lookup stuff
        Private _FeetToFireHydrant As String
        Private _MilesToFireDepartment As String

        'added 3/26/2013 for CPR (Blanket)
        Private _PersPropCov_IncludedInBlanketCoverage As Boolean
        Private _PersPropOfOthers_IncludedInBlanketCoverage As Boolean
        Private _BusinessIncomeCov_IncludedInBlanketCoverage As Boolean

        'added 2/18/2014
        Private _HasConvertedClassifications As Boolean
        Private _HasConvertedCoverages As Boolean
        Private _HasConvertedScheduledCoverages As Boolean

        'added 4/2/2014
        Private _PremiumFullterm As String

        Private _FarmBarnBuildingNum As String 'added 4/23/2014 for reconciliation
        Private _CanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean 'added 4/29/2014
        'added 10/18/2018 for multi-state
        Private _FarmBarnBuildingNum_MasterPart As String
        Private _FarmBarnBuildingNum_CGLPart As String
        Private _FarmBarnBuildingNum_CPRPart As String
        Private _FarmBarnBuildingNum_CIMPart As String
        Private _FarmBarnBuildingNum_CRMPart As String
        Private _FarmBarnBuildingNum_GARPart As String

        Private _Modifiers As List(Of QuickQuoteModifier) 'added 10/16/2014

        Private _CanUseScheduledCoverageNumForScheduledCoverageReconciliation As Boolean 'added 1/22/2015
        'added 1/22/2015
        Private _PersPropCov_ScheduledCoverageNum As String
        Private _PersPropOfOthers_ScheduledCoverageNum As String

        'added 2/9/2015 for CIM
        Private _ComputerHardwareLimit As String 'cov also has CoverageBasisTypeId set to 1
        Private _ComputerHardwareRate As String
        Private _ComputerHardwareQuotedPremium As String
        Private _ComputerProgramsApplicationsAndMediaLimit As String 'cov also has CoverageBasisTypeId set to 1
        Private _ComputerProgramsApplicationsAndMediaRate As String
        Private _ComputerProgramsApplicationsAndMediaQuotedPremium As String
        Private _ComputerBusinessIncomeLimit As String 'cov also has CoverageBasisTypeId set to 1
        Private _ComputerBusinessIncomeRate As String
        Private _ComputerBusinessIncomeQuotedPremium As String
        'added 3/16/2015
        Private _FineArtsScheduledItems As List(Of QuickQuoteFineArtsScheduledItem)
        'added 3/26/2015
        Private _ScheduledSigns As List(Of QuickQuoteScheduledSign)
        Private _UnscheduledSignsLimit As String 'cov also has CoverageBasisTypeId set to 1
        Private _UnscheduledSignsQuotedPremium As String

        'added 6/15/2015 for Farm
        Private _Dimensions As String
        Private _FarmStructureTypeId As String 'static data
        Private _FarmTypeId As String 'static data
        Private _NumberOfSolidFuelBurningUnits As String
        Private _VacancyFromDate As String
        Private _VacancyToDate As String
        Private _HasConvertedModifiers As Boolean
        Private _SprinklerSystem_AllExcept As Boolean
        Private _SprinklerSystem_AllIncluding As Boolean
        Private _HeatedBuildingSurchargeGasElectric As Boolean
        Private _HeatedBuildingSurchargeOther As Boolean
        Private _ExposedInsulationSurcharge As Boolean
        Private _E_Farm_Limit As String
        Private _E_Farm_DeductibleLimitId As String 'static data
        Private _E_Farm_QuotedPremium As String
        'added 6/16/2015 for Farm
        Private _HouseholdContentsLimit As String
        Private _HouseholdContentsQuotedPremium As String
        'added 6/24/2015 for Farm
        Private _OptionalCoverageEs As List(Of QuickQuoteOptionalCoverageE)
        Private _CanUseFarmBarnBuildingOptionalCoverageNumForOptionalCoverageEReconciliation As Boolean
        Private _HasConvertedOptionalCoverageEs As Boolean
        'added 7/28/2015 for Farm e2Value
        Private _PropertyValuation As QuickQuotePropertyValuation

        'added 2/17/2017
        Private _UseBuildingClassificationPropertiesToCreateOneItemInList As Boolean
        'added 2/20/2017
        Private _CanUseClassificationNumForClassificationReconciliation As Boolean

        Private _HasRestaurantEndorsement As Boolean '3/9/2017 - BOP stuff

        'added 7/8/2017
        Private _BuildingPersonalProperties As List(Of QuickQuoteBuildingPersonalProperty)
        Private _TotalPersonalPropertyNormalQuotedPremium As String
        Private _TotalPersonalPropertyOfOthersQuotedPremium As String
        Private _TotalPersonalPropertyCombinedQuotedPremium As String
        Private _TotalPersonalPropertyNormalLimit As String
        Private _TotalPersonalPropertyOfOthersLimit As String
        Private _TotalPersonalPropertyNormalEarthquakeQuotedPremium As String
        Private _TotalPersonalPropertyOfOthersEarthquakeQuotedPremium As String
        Private _TotalPersonalPropertyCombinedEarthquakeQuotedPremium As String
        Private _TotalPersonalPropertyNormalWithEarthquakeQuotedPremium As String
        Private _TotalPersonalPropertyOfOthersWithEarthquakeQuotedPremium As String
        Private _TotalPersonalPropertyCombinedWithEarthquakeQuotedPremium As String
        Private _TotalPersonalPropertyNormalCount As Integer
        Private _TotalPersonalPropertyOfOthersCount As Integer

        'added 10/26/2018
        Private _HasWindHailACVSettlement As Boolean 'covCodeId 20040; BOP IN/IL
        Private _WindHailACVSettlementQuotedPremium As String 'covCodeId 20040; BOP IN/IL
        Private _HasLimitationsOnRoofSurfacing As Boolean 'covCodeId 80542; BOP IL only
        Private _LimitationsOnRoofSurfacingQuotedPremium As String 'covCodeId 80542; BOP IL only
        Private _HasACVRoofSurfacing As Boolean 'covCodeId 80543; BOP IL only
        Private _ACVRoofSurfacingQuotedPremium As String 'covCodeId 80543; BOP IL only
        Private _ExcludeCosmeticDamage As Boolean 'covCodeId 80544; BOP IL only
        Private _ExcludeCosmeticDamageQuotedPremium As String 'covCodeId 80544; BOP IL only

        Private _DetailStatusCode As String 'added 5/15/2019

        Private _NumberOfUnitsPerBuilding As String 'added 8/25/2020 - DJG - Ohio BOP - Needed for Class Code 69145

        Private _EarthquakeDeductibleId As String
        Private _EarthquakeDeductible As String
        Private _BuildingCov_EarthquakeDeductibleId As String
        Private _BuildingCov_EarthquakeDeductible As String
        'Private _BusinessIncomeCov_EarthquakeDeductibleId As String 'not applying earthquake deductible to Business Income per requirements
        Private _BusinessIncomeCov_EarthquakeDeductible As String
        Private _PersPropCov_EarthquakeDeductibleId As String
        Private _PersPropCov_EarthquakeDeductible As String
        Private _PersPropOfOthers_EarthquakeDeductibleId As String
        Private _PersPropOfOthers_EarthquakeDeductible As String
        Private _PersPropCov_InflationGuardTypeId As String
        Private _PersPropCov_InflationGuardType As String
        Private _PersPropOfOthers_InflationGuardTypeId As String
        Private _PersPropOfOthers_InflationGuardType As String

        Private _OwnerOccupiedPercentageId As String
        Private _OwnerOccupiedPercentage As String

        Public Property EffectiveDate As String '3/9/2017 - BOP stuff
            Get
                Return _effectiveDate
            End Get
            Set(value As String)
                _effectiveDate = value
            End Set
        End Property
        Public Property PolicyId As String
            Get
                Return _PolicyId
            End Get
            Set(value As String)
                _PolicyId = value
            End Set
        End Property
        Public Property PolicyImageNum As String
            Get
                Return _PolicyImageNum
            End Get
            Set(value As String)
                _PolicyImageNum = value
            End Set
        End Property
        Public Property Description As String
            Get
                Return _Description
            End Get
            Set(value As String)
                _Description = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond building classification; contains empty coverage w/ coveragecode_id 10058</remarks>
        Public Property Program As String
            Get
                Return _Program
            End Get
            Set(value As String)
                _Program = value
                If _Classification <> "" AndAlso _ClassCode <> "" AndAlso (_ProgramAbbreviation <> "" OrElse _Program <> "") Then
                    'If _UseBuildingClassificationPropertiesToCreateOneItemInList = True Then 'added 2/20/2017; previously happening every time; may not use, but shouldn't matter since properties should no longer be set
                    '_ClassificationTypeId = qqHelper.GetBuildingClassificationTypeId(_Classification, _ClassCode, _ProgramAbbreviation, _Program)
                    '3/9/2017 - BOP stuff
                    If qqHelper.IsDateString(_effectiveDate) = True Then
                        _ClassificationTypeId = qqHelper.GetBuildingClassificationTypeId(_Classification, _ClassCode, _ProgramAbbreviation, _Program, _effectiveDate)
                    Else
                        _ClassificationTypeId = qqHelper.GetBuildingClassificationTypeId(_Classification, _ClassCode, _ProgramAbbreviation, _Program)
                    End If
                    'End If
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond building classification; contains empty coverage w/ coveragecode_id 10058</remarks>
        Public Property ProgramAbbreviation As String
            Get
                Return _ProgramAbbreviation
            End Get
            Set(value As String)
                _ProgramAbbreviation = value
                If _Classification <> "" AndAlso _ClassCode <> "" AndAlso (_ProgramAbbreviation <> "" OrElse _Program <> "") Then
                    'If _UseBuildingClassificationPropertiesToCreateOneItemInList = True Then 'added 2/20/2017; previously happening every time; may not use, but shouldn't matter since properties should no longer be set
                    '_ClassificationTypeId = qqHelper.GetBuildingClassificationTypeId(_Classification, _ClassCode, _ProgramAbbreviation, _Program)
                    '3/9/2017 - BOP stuff
                    If qqHelper.IsDateString(_effectiveDate) = True Then
                        _ClassificationTypeId = qqHelper.GetBuildingClassificationTypeId(_Classification, _ClassCode, _ProgramAbbreviation, _Program, _effectiveDate)
                    Else
                        _ClassificationTypeId = qqHelper.GetBuildingClassificationTypeId(_Classification, _ClassCode, _ProgramAbbreviation, _Program)
                    End If
                    'End If
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond building classification; contains empty coverage w/ coveragecode_id 10058</remarks>
        Public Property Classification As String
            Get
                Return _Classification
            End Get
            Set(value As String)
                _Classification = value
                If _Classification <> "" AndAlso _ClassCode <> "" AndAlso (_ProgramAbbreviation <> "" OrElse _Program <> "") Then
                    'If _UseBuildingClassificationPropertiesToCreateOneItemInList = True Then 'added 2/20/2017; previously happening every time; may not use, but shouldn't matter since properties should no longer be set
                    '_ClassificationTypeId = qqHelper.GetBuildingClassificationTypeId(_Classification, _ClassCode, _ProgramAbbreviation, _Program)
                    '3/9/2017 - BOP stuff
                    If qqHelper.IsDateString(_effectiveDate) = True Then
                        _ClassificationTypeId = qqHelper.GetBuildingClassificationTypeId(_Classification, _ClassCode, _ProgramAbbreviation, _Program, _effectiveDate)
                    Else
                        _ClassificationTypeId = qqHelper.GetBuildingClassificationTypeId(_Classification, _ClassCode, _ProgramAbbreviation, _Program)
                    End If
                    'End If
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond building classification; contains empty coverage w/ coveragecode_id 10058</remarks>
        Public Property ClassCode As String
            Get
                Return _ClassCode
            End Get
            Set(value As String)
                _ClassCode = value
                If _Classification <> "" AndAlso _ClassCode <> "" AndAlso (_ProgramAbbreviation <> "" OrElse _Program <> "") Then
                    'If _UseBuildingClassificationPropertiesToCreateOneItemInList = True Then 'added 2/20/2017; previously happening every time; may not use, but shouldn't matter since properties should no longer be set
                    '_ClassificationTypeId = qqHelper.GetBuildingClassificationTypeId(_Classification, _ClassCode, _ProgramAbbreviation, _Program)
                    '3/9/2017 - BOP stuff
                    If qqHelper.IsDateString(_effectiveDate) = True Then
                        _ClassificationTypeId = qqHelper.GetBuildingClassificationTypeId(_Classification, _ClassCode, _ProgramAbbreviation, _Program, _effectiveDate)
                    Else
                        _ClassificationTypeId = qqHelper.GetBuildingClassificationTypeId(_Classification, _ClassCode, _ProgramAbbreviation, _Program)
                    End If
                    'End If
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond building classification; contains empty coverage w/ coveragecode_id 10058</remarks>
        Public Property ClassificationTypeId As String
            Get
                Return _ClassificationTypeId
            End Get
            Set(value As String)
                _ClassificationTypeId = value
                'If _UseBuildingClassificationPropertiesToCreateOneItemInList = True Then 'added 2/20/2017; previously happening every time; will leave for now since ClassificationTypeId would be set when retrieving quote (and building), which is before new building property could be set
                'qqHelper.SetBuildingClassificationType(_ClassificationTypeId, _Program, _Classification, _ClassCode, _ProgramAbbreviation) 'commented 9/2/2017 to prevent duplicate call w/ below code
                '3/9/2017 - BOP stuff
                If qqHelper.IsDateString(_effectiveDate) = True Then
                    qqHelper.SetBuildingClassificationType(_ClassificationTypeId, _Program, _Classification, _ClassCode, _ProgramAbbreviation, _effectiveDate)
                Else
                    qqHelper.SetBuildingClassificationType(_ClassificationTypeId, _Program, _Classification, _ClassCode, _ProgramAbbreviation)
                End If
                'End If
            End Set
        End Property
        Public Property Occupancy As String
            Get
                Return _Occupancy
            End Get
            Set(value As String)
                _Occupancy = value
                'Select Case _Occupancy
                '    Case "Non-Owner Occupied Bldg / Lessor's"
                '        _OccupancyId = "16"
                '    Case "Owner Occupied Bldg 75% or Less / Lessor's"
                '        _OccupancyId = "17"
                '    Case "Owner Occupied Bldg 76% or More / Occupant"
                '        _OccupancyId = "18"
                '    Case "Tenant / Occupant"
                '        _OccupancyId = "19"
                '    Case "Owner Occupied Bldg 10% Or Less / Lessor's" '3/9/2017 - BOP stuff
                '        _OccupancyId = "20"
                '    Case "Owner Occupied Bldg More than 10% / Occupant" '3/9/2017 - BOP stuff
                '        _OccupancyId = "21"
                '    Case Else
                '        _OccupancyId = ""
                'End Select
                'updated 12/12/2013
                _OccupancyId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuoteHelperClass.QuickQuotePropertyName.OccupancyId, _Occupancy)
            End Set
        End Property
        Public Property OccupancyId As String 'verified in database 7/3/2012; 12/12/2013 note: uses Diamond's OccupancyCode table (also used w/ QuickQuoteLocation, QuickQuoteSectionCoverage, QuickQuoteSectionICoverage); using separate xml for Building than other spots
            Get
                Return _OccupancyId
            End Get
            Set(value As String)
                _OccupancyId = value
                '(16=Non-Owner Occupied Bldg / Lessor's; 17=Owner Occupied Bldg 75% or Less / Lessor's; 18=Owner Occupied Bldg 76% or More / Occupant; 19=Tenant / Occupant)
                '_Occupancy = ""
                'If IsNumeric(_OccupancyId) = True Then
                '    Select Case _OccupancyId
                '        Case "16"
                '            _Occupancy = "Non-Owner Occupied Bldg / Lessor's"
                '        Case "17"
                '            _Occupancy = "Owner Occupied Bldg 75% or Less / Lessor's"
                '        Case "18"
                '            _Occupancy = "Owner Occupied Bldg 76% or More / Occupant"
                '        Case "19"
                '            _Occupancy = "Tenant / Occupant"
                '        Case "20" '3/9/2017 - BOP stuff
                '            _Occupancy = "Owner Occupied Bldg 10% Or Less / Lessor's"
                '        Case "21" '3/9/2017 - BOP stuff
                '            _Occupancy = "Owner Occupied Bldg More than 10% / Occupant"
                '    End Select
                'End If
                'updated 12/12/2013
                _Occupancy = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuoteHelperClass.QuickQuotePropertyName.OccupancyId, _OccupancyId)
            End Set
        End Property
        Public Property Construction As String '11/25/2013 note: uses Diamond's FarmConstructionType table
            Get
                Return _Construction
            End Get
            Set(value As String)
                _Construction = value
                'Select Case _Construction
                '    Case "Frame"
                '        _ConstructionId = "7" 'could also be 2 (10/18/2012); 11/25/2013 note: looks like 2 should be Masonry unless it's just something hokey for CPR that's not reflected in the database (should be okay since this uses FarmConstructionType table)
                '    Case "Joisted Masonry" '11/26/2012 - corrected spelling from "Jointed Masonry"
                '        _ConstructionId = "12"
                '    Case "Non-Combustible"
                '        _ConstructionId = "13"
                '    Case "Masonry Non-Combustible"
                '        _ConstructionId = "14"
                '    Case "Modified Fire Resistive"
                '        _ConstructionId = "15"
                '    Case "Fire Resistive"
                '        _ConstructionId = "16"
                '    Case Else
                '        _ConstructionId = ""
                'End Select
                'updated 12/12/2013
                _ConstructionId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuoteHelperClass.QuickQuotePropertyName.ConstructionId, _Construction)
            End Set
        End Property
        Public Property ConstructionId As String 'verified in database 7/3/2012; updated 10/18/2012 w/ Frame = 2; 11/25/2013 note: uses Diamond's FarmConstructionType table
            Get
                Return _ConstructionId
            End Get
            Set(value As String)
                _ConstructionId = value
                '(7=Frame; 12=Joisted Masonry; 13=Non-Combustible; 14=Masonry Non-Combustible; 15=Modified Fire Resistive; 16=Fire Resistive)'11/26/2012 - corrected spelling from 12=Jointed Masonry
                '_Construction = ""
                'If IsNumeric(_ConstructionId) = True Then
                '    Select Case _ConstructionId
                '        Case "2" 'added 10/18/2012; 11/25/2013 note: looks like 2 should be Masonry unless it's just something hokey for CPR that's not reflected in the database (should be okay since this uses FarmConstructionType table)
                '            _Construction = "Frame"
                '        Case "7"
                '            _Construction = "Frame"
                '        Case "12"
                '            _Construction = "Joisted Masonry" '11/26/2012 - corrected spelling from "Jointed Masonry"
                '        Case "13"
                '            _Construction = "Non-Combustible"
                '        Case "14"
                '            _Construction = "Masonry Non-Combustible"
                '        Case "15"
                '            _Construction = "Modified Fire Resistive"
                '        Case "16"
                '            _Construction = "Fire Resistive"
                '    End Select
                'End If
                'updated 12/12/2013
                _Construction = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuoteHelperClass.QuickQuotePropertyName.ConstructionId, _ConstructionId)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 165</remarks>
        Public Property AutoIncrease As String
            Get
                Return _AutoIncrease
            End Get
            Set(value As String)
                _AutoIncrease = value
                Select Case AutoIncrease
                    Case "2", "2%"
                        _AutoIncreaseId = "1"
                    Case "4", "4%"
                        _AutoIncreaseId = "2"
                    Case "6", "6%"
                        _AutoIncreaseId = "3"
                    Case "8", "8%"
                        _AutoIncreaseId = "4"
                    Case "10", "10%"
                        _AutoIncreaseId = "5"
                    Case "12", "12%"
                        _AutoIncreaseId = "6"
                    Case "14", "14%"
                        _AutoIncreaseId = "7"
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 165</remarks>
        Public Property AutoIncreaseId As String 'verified in database 7/3/2012
            Get
                Return _AutoIncreaseId
            End Get
            Set(value As String)
                _AutoIncreaseId = value
                '(1=2; 2=4; 3=6; 4=8; 5=10; 6=12; 7=14; 8=16)
                _AutoIncrease = ""
                If IsNumeric(_AutoIncreaseId) = True Then
                    Select Case _AutoIncreaseId
                        Case "1"
                            _AutoIncrease = "2%"
                        Case "2"
                            _AutoIncrease = "4%"
                        Case "3"
                            _AutoIncrease = "6%"
                        Case "4"
                            _AutoIncrease = "8%"
                        Case "5"
                            _AutoIncrease = "10%"
                        Case "6"
                            _AutoIncrease = "12%"
                        Case "7"
                            _AutoIncrease = "14%"
                        Case "8"
                            _AutoIncrease = "16%"
                    End Select
                End If
            End Set
        End Property

        '3/9/2017 - BOP stuff
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80400</remarks>
        Public Property AutoIncreasePremium As String
            Get
                'Return _AutoIncreasePremium
                Return qqHelper.QuotedPremiumFormat(_AutoIncreasePremium)
            End Get
            Set(value As String)
                _AutoIncreasePremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_AutoIncreasePremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80400</remarks>
        Public Property HasAutoIncreasePremium As Boolean
            Get
                Return _HasAutoIncreasePremium
            End Get
            Set(value As Boolean)
                _HasAutoIncreasePremium = value
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80145; stored in xml at policy level; CoverageLimitId</remarks>
        Public Property PropertyDeductible As String '*currently being sent/returned w/ PolicyCoverages
            Get
                Return _PropertyDeductible
            End Get
            Set(value As String)
                _PropertyDeductible = value
                Select Case _PropertyDeductible
                    Case "250"
                        _PropertyDeductibleId = "21"
                    Case "500"
                        _PropertyDeductibleId = "22"
                    Case "1000"
                        _PropertyDeductibleId = "24"
                    Case "2500"
                        _PropertyDeductibleId = "75"
                    Case "5000" 'added 5/10/2017
                        _PropertyDeductibleId = "76"
                    Case "7500" 'added 5/10/2017
                        _PropertyDeductibleId = "333"
                    Case "10000" 'added 5/10/2017
                        _PropertyDeductibleId = "157"

                    Case Else
                        _PropertyDeductibleId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80145; stored in xml at policy level; CoverageLimitId</remarks>
        Public Property PropertyDeductibleId As String 'verified in database 7/3/2012 (matches CovererageLimit values)
            Get
                Return _PropertyDeductibleId
            End Get
            Set(value As String)
                _PropertyDeductibleId = value
                '(21=250; 22=500; 24=1000; 75=2500)
                _PropertyDeductible = ""
                If IsNumeric(_PropertyDeductibleId) = True Then
                    Select Case _PropertyDeductibleId
                        Case "21"
                            _PropertyDeductible = "250"
                        Case "22"
                            _PropertyDeductible = "500"
                        Case "24"
                            _PropertyDeductible = "1000"
                        Case "75"
                            _PropertyDeductible = "2500"
                        Case "76" 'added 5/10/2017
                            _PropertyDeductible = "5000"
                        Case "333" 'added 5/10/2017
                            _PropertyDeductible = "7500"
                        Case "157" 'added 5/10/2017
                            _PropertyDeductible = "10000"

                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 165</remarks>
        Public Property Limit As String
            Get
                Return _Limit
            End Get
            Set(value As String)
                _Limit = value
                qqHelper.ConvertToLimitFormat(_Limit)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 165</remarks>
        Public Property LimitQuotedPremium As String
            Get
                'Return _LimitQuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_LimitQuotedPremium)
            End Get
            Set(value As String)
                _LimitQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_LimitQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 165</remarks>
        Public Property Valuation As String
            Get
                Return _Valuation
            End Get
            Set(value As String)
                _Valuation = value
                Select Case _Valuation
                    Case "N/A" 'added 10/19/2012 for CPR to match specs
                        _ValuationId = "-1"
                    Case "Replacement Cost"
                        _ValuationId = "1"
                    Case "Actual Cash Value"
                        _ValuationId = "2"
                    Case "Functional Building Valuation"
                        _ValuationId = "3"
                    Case "Functional Replacement Cost" 'added 10/18/2012 for CPR
                        _ValuationId = "7"
                    Case Else
                        _ValuationId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' specific to building coverage (coveragecode_id 165)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>coveragecode_id 165</remarks>
        Public Property ValuationId As String 'verified in database 7/3/2012
            Get
                Return _ValuationId
            End Get
            Set(value As String)
                _ValuationId = value
                '(1=Replacement Cost; 2=Actual Cash Value; 3=Functional Building Valuation)
                _Valuation = ""
                If IsNumeric(_ValuationId) = True Then
                    Select Case _ValuationId
                        Case "-1" 'added 10/19/2012 for CPR to match specs
                            _Valuation = "N/A"
                        Case "1"
                            _Valuation = "Replacement Cost"
                        Case "2"
                            _Valuation = "Actual Cash Value"
                        Case "3"
                            _Valuation = "Functional Building Valuation"
                        Case "7" 'added 10/18/2012 for CPR
                            _Valuation = "Functional Replacement Cost"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' specific to building coverage (coveragecode_id 165)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>coveragecode_id 165</remarks>
        Public Property IsBuildingValIncludedInBlanketRating As Boolean
            Get
                Return _IsBuildingValIncludedInBlanketRating
            End Get
            Set(value As Boolean)
                _IsBuildingValIncludedInBlanketRating = value
            End Set
        End Property

        '3/9/2017 - BOP stuff
        ''' <summary>
        ''' specific to personal property coverage (coveragecode_id 80395)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>coveragecode_id 21037</remarks>
        Public Property HasACVRoofing As Boolean
            Get
                Return _HasACVRoofing
            End Get
            Set(value As Boolean)
                _HasACVRoofing = value
            End Set
        End Property

        Public Property ACVRoofingQuotedPremium As String
            Get
                'Return _ACVRoofingQuotedPremium
                Return qqHelper.QuotedPremiumFormat(_ACVRoofingQuotedPremium)
            End Get
            Set(value As String)
                _ACVRoofingQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_ACVRoofingQuotedPremium)
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 165</remarks>
        Public Property HasMineSubsidence As Boolean
            Get
                Return _HasMineSubsidence
            End Get
            Set(value As Boolean)
                _HasMineSubsidence = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 165; found in value for CoverageAdditionalInfoRecord w/ description = 'Mine Subsidence Premium'</remarks>
        Public Property MineSubsidenceQuotedPremium As String '*not sure where to find; maybe used w/ coveragecode_id 20027
            Get
                'Return _MineSubsidenceQuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_MineSubsidenceQuotedPremium)
            End Get
            Set(value As String)
                _MineSubsidenceQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_MineSubsidenceQuotedPremium)
            End Set
        End Property
        Public Property MineSubsidence_IsDwellingStructure As Boolean 'added 11/8/2018 for CPR mine subsidence; covCodeId 20027; CheckBox: Mine Subsidence (BOP IN/IL, CPR IN/IL)
            Get
                Return _MineSubsidence_IsDwellingStructure
            End Get
            Set(value As Boolean)
                _MineSubsidence_IsDwellingStructure = value
            End Set
        End Property
        Public Property HasSprinklered As Boolean
            Get
                Return _HasSprinklered
            End Get
            Set(value As Boolean)
                _HasSprinklered = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21037</remarks>
        Public Property PersonalPropertyLimit As String
            Get
                Return _PersonalPropertyLimit
            End Get
            Set(value As String)
                _PersonalPropertyLimit = value
                qqHelper.ConvertToLimitFormat(_PersonalPropertyLimit)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21037</remarks>
        Public Property PersonalPropertyLimitQuotedPremium As String
            Get
                'Return _PersonalPropertyLimitQuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_PersonalPropertyLimitQuotedPremium)
            End Get
            Set(value As String)
                _PersonalPropertyLimitQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_PersonalPropertyLimitQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21037</remarks>
        Public Property ValuationMethod As String
            Get
                Return _ValuationMethod
            End Get
            Set(value As String)
                _ValuationMethod = value
                Select Case _ValuationMethod
                    Case "N/A" 'added 10/19/2012 for CPR to match specs
                        _ValuationMethodId = "-1"
                    Case "Replacement Cost"
                        _ValuationMethodId = "1"
                    Case "Actual Cash Value"
                        _ValuationMethodId = "2"
                    Case "Functional Building Valuation"
                        _ValuationMethodId = "3"
                    Case "Functional Replacement Cost" 'added 10/18/2012 for CPR
                        _ValuationMethodId = "7"
                    Case Else
                        _ValuationMethodId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' specific to personal property coverage (coveragecode_id 21037)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>coveragecode_id 21037</remarks>
        Public Property ValuationMethodId As String 'verified in database 7/3/2012
            Get
                Return _ValuationMethodId
            End Get
            Set(value As String)
                _ValuationMethodId = value
                '(1=Replacement Cost; 2=Actual Cash Value; 3=Functional Building Valuation)
                _ValuationMethod = ""
                If IsNumeric(_ValuationMethodId) = True Then
                    Select Case _ValuationMethodId
                        Case "-1" 'added 10/19/2012 for CPR to match specs
                            _ValuationMethod = "N/A"
                        Case "1"
                            _ValuationMethod = "Replacement Cost"
                        Case "2"
                            _ValuationMethod = "Actual Cash Value"
                        Case "3"
                            _ValuationMethod = "Functional Building Valuation"
                        Case "7" 'added 10/18/2012 for CPR
                            _ValuationMethod = "Functional Replacement Cost"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' specific to personal property coverage (coveragecode_id 21037)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>coveragecode_id 21037</remarks>
        Public Property IsValMethodIncludedInBlanketRating As Boolean
            Get
                Return _IsValMethodIncludedInBlanketRating
            End Get
            Set(value As Boolean)
                _IsValMethodIncludedInBlanketRating = value
            End Set
        End Property

        Public Property IsAgreedValue As Boolean ' added 2/1/2016 Bug 4845 Matt A; 3/9/2017 - included in this library w/ BOP updates
            Get
                Return _IsAgreedValue
            End Get
            Set(value As Boolean)
                _IsAgreedValue = value
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 150; should use <see cref="AccountsReceivableOnPremisesExcessLimit"/></remarks>
        Public Property AccountsReceivableOnPremises As String
            Get
                Return _AccountsReceivableOnPremises
            End Get
            Set(value As String)
                _AccountsReceivableOnPremises = value
                qqHelper.ConvertToLimitFormat(_AccountsReceivableOnPremises)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 150; defaulted</remarks>
        Public Property AccountsReceivableOffPremises As String
            Get
                Return _AccountsReceivableOffPremises
            End Get
            Set(value As String)
                _AccountsReceivableOffPremises = value
                qqHelper.ConvertToLimitFormat(_AccountsReceivableOffPremises)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 150</remarks>
        Public Property AccountsReceivableQuotedPremium As String
            Get
                'Return _AccountsReceivableQuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_AccountsReceivableQuotedPremium)
            End Get
            Set(value As String)
                _AccountsReceivableQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_AccountsReceivableQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 151; should use <see cref="ValuablePapersOnPremisesExcessLimit"/></remarks>
        Public Property ValuablePapersOnPremises As String
            Get
                Return _ValuablePapersOnPremises
            End Get
            Set(value As String)
                _ValuablePapersOnPremises = value
                qqHelper.ConvertToLimitFormat(_ValuablePapersOnPremises)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 151; defaulted</remarks>
        Public Property ValuablePapersOffPremises As String
            Get
                Return _ValuablePapersOffPremises
            End Get
            Set(value As String)
                _ValuablePapersOffPremises = value
                qqHelper.ConvertToLimitFormat(_ValuablePapersOffPremises)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 151</remarks>
        Public Property ValuablePapersQuotedPremium As String
            Get
                'Return _ValuablePapersQuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_ValuablePapersQuotedPremium)
            End Get
            Set(value As String)
                _ValuablePapersQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_ValuablePapersQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21038</remarks>
        Public Property CondoCommercialUnitOwnersLimit As String
            Get
                Return _CondoCommercialUnitOwnersLimit
            End Get
            Set(value As String)
                _CondoCommercialUnitOwnersLimit = value
                Select Case _CondoCommercialUnitOwnersLimit
                    Case "1,000"
                        _CondoCommercialUnitOwnersLimitId = "11"
                    Case "5,000"
                        _CondoCommercialUnitOwnersLimitId = "15"
                    Case "10,000"
                        _CondoCommercialUnitOwnersLimitId = "7"
                    Case "15,000"
                        _CondoCommercialUnitOwnersLimitId = "48"
                    Case "20,000"
                        _CondoCommercialUnitOwnersLimitId = "49"
                    Case "25,000"
                        _CondoCommercialUnitOwnersLimitId = "8"
                    Case Else
                        _CondoCommercialUnitOwnersLimitId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21038</remarks>
        Public Property CondoCommercialUnitOwnersLimitId As String 'verified in database 7/3/2012
            Get
                Return _CondoCommercialUnitOwnersLimitId
            End Get
            Set(value As String)
                _CondoCommercialUnitOwnersLimitId = value
                '(11=1,000; 15=5,000; 7=10,000; 48=15,000; 49=20,000; 8=25,000)
                _CondoCommercialUnitOwnersLimit = ""
                If IsNumeric(_CondoCommercialUnitOwnersLimitId) = True Then
                    Select Case _CondoCommercialUnitOwnersLimitId
                        Case "11"
                            _CondoCommercialUnitOwnersLimit = "1,000"
                        Case "15"
                            _CondoCommercialUnitOwnersLimit = "5,000"
                        Case "7"
                            _CondoCommercialUnitOwnersLimit = "10,000"
                        Case "48"
                            _CondoCommercialUnitOwnersLimit = "15,000"
                        Case "49"
                            _CondoCommercialUnitOwnersLimit = "20,000"
                        Case "8"
                            _CondoCommercialUnitOwnersLimit = "25,000"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21038</remarks>
        Public Property CondoCommercialUnitOwnersLimitQuotedPremium As String
            Get
                'Return _CondoCommercialUnitOwnersLimitQuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_CondoCommercialUnitOwnersLimitQuotedPremium)
            End Get
            Set(value As String)
                _CondoCommercialUnitOwnersLimitQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_CondoCommercialUnitOwnersLimitQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverages w/ coveragecode_ids 173 (undamaged), 161 (demo), 167 (increased), or 21045 (demo and increased)</remarks>
        Public Property HasOrdinanceOrLaw As Boolean
            Get
                Return _HasOrdinanceOrLaw
            End Get
            Set(value As Boolean)
                _HasOrdinanceOrLaw = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 173</remarks>
        Public Property HasOrdOrLawUndamagedPortion As Boolean
            Get
                Return _HasOrdOrLawUndamagedPortion
            End Get
            Set(value As Boolean)
                _HasOrdOrLawUndamagedPortion = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 173</remarks>
        Public Property OrdOrLawUndamagedPortionQuotedPremium As String
            Get
                'Return _OrdOrLawUndamagedPortionQuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_OrdOrLawUndamagedPortionQuotedPremium)
            End Get
            Set(value As String)
                _OrdOrLawUndamagedPortionQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_OrdOrLawUndamagedPortionQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 161</remarks>
        Public Property OrdOrLawDemoCostLimit As String
            Get
                Return _OrdOrLawDemoCostLimit
            End Get
            Set(value As String)
                _OrdOrLawDemoCostLimit = value
                qqHelper.ConvertToLimitFormat(_OrdOrLawDemoCostLimit)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 161</remarks>
        Public Property OrdOrLawDemoCostLimitQuotedPremium As String
            Get
                'Return _OrdOrLawDemoCostLimitQuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_OrdOrLawDemoCostLimitQuotedPremium)
            End Get
            Set(value As String)
                _OrdOrLawDemoCostLimitQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_OrdOrLawDemoCostLimitQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 167</remarks>
        Public Property OrdOrLawIncreasedCostLimit As String
            Get
                Return _OrdOrLawIncreasedCostLimit
            End Get
            Set(value As String)
                _OrdOrLawIncreasedCostLimit = value
                qqHelper.ConvertToLimitFormat(_OrdOrLawIncreasedCostLimit)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 167</remarks>
        Public Property OrdOrLawIncreaseCostLimitQuotedPremium As String
            Get
                'Return _OrdOrLawIncreaseCostLimitQuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_OrdOrLawIncreaseCostLimitQuotedPremium)
            End Get
            Set(value As String)
                _OrdOrLawIncreaseCostLimitQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_OrdOrLawIncreaseCostLimitQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21045</remarks>
        Public Property OrdOrLawDemoAndIncreasedCostLimit As String
            Get
                Return _OrdOrLawDemoAndIncreasedCostLimit
            End Get
            Set(value As String)
                _OrdOrLawDemoAndIncreasedCostLimit = value
                qqHelper.ConvertToLimitFormat(_OrdOrLawDemoAndIncreasedCostLimit)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21045</remarks>
        Public Property OrdOrLawDemoAndIncreasedCostLimitQuotedPremium As String
            Get
                'Return _OrdOrLawDemoAndIncreasedCostLimitQuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_OrdOrLawDemoAndIncreasedCostLimitQuotedPremium)
            End Get
            Set(value As String)
                _OrdOrLawDemoAndIncreasedCostLimitQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_OrdOrLawDemoAndIncreasedCostLimitQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 70066</remarks>
        Public Property HasSpoilage As Boolean
            Get
                Return _HasSpoilage
            End Get
            Set(value As Boolean)
                _HasSpoilage = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 70066</remarks>
        Public Property SpoilageQuotedPremium As String
            Get
                'Return _SpoilageQuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_SpoilageQuotedPremium)
            End Get
            Set(value As String)
                _SpoilageQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_SpoilageQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 70066</remarks>
        Public Property SpoilagePropertyClassification As String
            Get
                Return _SpoilagePropertyClassification
            End Get
            Set(value As String)
                _SpoilagePropertyClassification = value
                'Select Case _SpoilagePropertyClassificationId = _SpoilagePropertyClassification
                'corrected 4/14/2015 after getting error "Conversion from string "Bakery Goods" to type 'Boolean' is not valid."; 4/16/2015 note: caused by calling qqHelper.CloneObject on a building w/ SpoilagePropertyClassification = ""
                Select Case _SpoilagePropertyClassification
                    Case "Bakery Goods"
                        _SpoilagePropertyClassificationId = "1"
                    Case "Cheese Goods"
                        _SpoilagePropertyClassificationId = "2"
                    Case "Convenience Food Stores"
                        _SpoilagePropertyClassificationId = "3"
                    Case "Dairy Products, excluding Ice Cream"
                        _SpoilagePropertyClassificationId = "4"
                    Case "Dairy Products, including Ice Cream"
                        _SpoilagePropertyClassificationId = "5"
                    Case "Delicatessens"
                        _SpoilagePropertyClassificationId = "6"
                    Case "Florists"
                        _SpoilagePropertyClassificationId = "7"
                    Case "Fruits and Vegetables"
                        _SpoilagePropertyClassificationId = "8"
                    Case "Grocery Stores"
                        _SpoilagePropertyClassificationId = "9"
                    Case "Meat and Poultry Markets"
                        _SpoilagePropertyClassificationId = "10"
                    Case "Other"
                        _SpoilagePropertyClassificationId = "11"
                    Case "Pharmaceuticals"
                        _SpoilagePropertyClassificationId = "12"
                    Case "Restaurants (limited cooking only)"
                        _SpoilagePropertyClassificationId = "13"
                    Case "Seafood"
                        _SpoilagePropertyClassificationId = "14"
                    Case "Supermarkets"
                        _SpoilagePropertyClassificationId = "15"
                    Case Else
                        _SpoilagePropertyClassificationId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 70066</remarks>
        Public Property SpoilagePropertyClassificationId As String 'verified in database 7/3/2012
            Get
                Return _SpoilagePropertyClassificationId
            End Get
            Set(value As String)
                _SpoilagePropertyClassificationId = value
                '(1=Bakery Goods; 2=Cheese Goods; 3=Convenience Food Stores; 4=Dairy Products, excluding Ice Cream; 5=Dairy Products, including Ice Cream; 6=Delicatessens; 7=Florists; 8=Fruits and Vegetables; 9=Grocery Stores; 10=Meat and Poultry Markets; 11=Other; 12=Pharmaceuticals; 13=Restaurants (limited cooking only); 14=Seafood; 15=Supermarkets)
                _SpoilagePropertyClassification = ""
                If IsNumeric(_SpoilagePropertyClassificationId) = True Then
                    Select Case _SpoilagePropertyClassificationId
                        Case "1"
                            _SpoilagePropertyClassification = "Bakery Goods"
                        Case "2"
                            _SpoilagePropertyClassification = "Cheese Goods"
                        Case "3"
                            _SpoilagePropertyClassification = "Convenience Food Stores"
                        Case "4"
                            _SpoilagePropertyClassification = "Dairy Products, excluding Ice Cream"
                        Case "5"
                            _SpoilagePropertyClassification = "Dairy Products, including Ice Cream"
                        Case "6"
                            _SpoilagePropertyClassification = "Delicatessens"
                        Case "7"
                            _SpoilagePropertyClassification = "Florists"
                        Case "8"
                            _SpoilagePropertyClassification = "Fruits and Vegetables"
                        Case "9"
                            _SpoilagePropertyClassification = "Grocery Stores"
                        Case "10"
                            _SpoilagePropertyClassification = "Meat and Poultry Markets"
                        Case "11"
                            _SpoilagePropertyClassification = "Other"
                        Case "12"
                            _SpoilagePropertyClassification = "Pharmaceuticals"
                        Case "13"
                            _SpoilagePropertyClassification = "Restaurants (limited cooking only)"
                        Case "14"
                            _SpoilagePropertyClassification = "Seafood"
                        Case "15"
                            _SpoilagePropertyClassification = "Supermarkets"
                            'Case Else
                            '    _SpoilagePropertyClassification = ""
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 70066</remarks>
        Public Property SpoilageTotalLimit As String
            Get
                Return _SpoilageTotalLimit
            End Get
            Set(value As String)
                _SpoilageTotalLimit = value
                qqHelper.ConvertToLimitFormat(_SpoilageTotalLimit)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 70066</remarks>
        Public Property IsSpoilageRefrigerationMaintenanceAgreement As Boolean
            Get
                Return _IsSpoilageRefrigerationMaintenanceAgreement
            End Get
            Set(value As Boolean)
                _IsSpoilageRefrigerationMaintenanceAgreement = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 70066</remarks>
        Public Property IsSpoilageBreakdownOrContamination As Boolean
            Get
                Return _IsSpoilageBreakdownOrContamination
            End Get
            Set(value As Boolean)
                _IsSpoilageBreakdownOrContamination = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 70066</remarks>
        Public Property IsSpoilagePowerOutage As Boolean
            Get
                Return _IsSpoilagePowerOutage
            End Get
            Set(value As Boolean)
                _IsSpoilagePowerOutage = value
            End Set
        End Property

        Public Property BuildingCoverages As Generic.List(Of QuickQuoteCoverage)
            Get
                Return _BuildingCoverages
            End Get
            Set(value As Generic.List(Of QuickQuoteCoverage))
                _BuildingCoverages = value
            End Set
        End Property

        Public Property BuildingClassifications As Generic.List(Of QuickQuoteClassification)
            Get
                Return _BuildingClassifications
            End Get
            Set(value As Generic.List(Of QuickQuoteClassification))
                _BuildingClassifications = value
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 286; stored in xml at policy level</remarks>
        Public Property HasBusinessMasterEnhancement As Boolean
            Get
                Return _HasBusinessMasterEnhancement
            End Get
            Set(value As Boolean)
                _HasBusinessMasterEnhancement = value
            End Set
        End Property

        'Public Property ProtectionClassId As String
        '    Get
        '        Return _ProtectionClassId
        '    End Get
        '    Set(value As String)
        '        _ProtectionClassId = value
        '    End Set
        'End Property
        Public Property ProtectionClassId As String '11/25/2013 note: uses Diamond's FarmProtectionClassType table
            Get
                Return _ProtectionClassId
            End Get
            Set(value As String)
                _ProtectionClassId = value
                '_ProtectionClass = ""
                'If IsNumeric(_ProtectionClassId) = True Then
                '    Select Case _ProtectionClassId
                '        Case "12"
                '            _ProtectionClass = "01"
                '        Case "13"
                '            _ProtectionClass = "02"
                '        Case "14"
                '            _ProtectionClass = "03"
                '        Case "15"
                '            _ProtectionClass = "04"
                '        Case "16"
                '            _ProtectionClass = "05"
                '        Case "17"
                '            _ProtectionClass = "06"
                '        Case "18"
                '            _ProtectionClass = "07"
                '        Case "19"
                '            _ProtectionClass = "08"
                '        Case "20"
                '            _ProtectionClass = "8B"
                '        Case "21"
                '            _ProtectionClass = "09"
                '        Case "22"
                '            _ProtectionClass = "10"
                '    End Select
                'End If
                'updated 12/12/2013
                _ProtectionClass = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuoteHelperClass.QuickQuotePropertyName.ProtectionClassId, _ProtectionClassId)
            End Set
        End Property
        Public Property ProtectionClass As String '11/25/2013 note: uses Diamond's FarmProtectionClassType table
            Get
                Return _ProtectionClass
            End Get
            Set(value As String)
                _ProtectionClass = value
                'Select Case _ProtectionClass
                '    Case "01"
                '        _ProtectionClassId = "12"
                '    Case "02"
                '        _ProtectionClassId = "13"
                '    Case "03"
                '        _ProtectionClassId = "14"
                '    Case "04"
                '        _ProtectionClassId = "15"
                '    Case "05"
                '        _ProtectionClassId = "16"
                '    Case "06"
                '        _ProtectionClassId = "17"
                '    Case "07"
                '        _ProtectionClassId = "18"
                '    Case "08"
                '        _ProtectionClassId = "19"
                '    Case "8B"
                '        _ProtectionClassId = "20"
                '    Case "09"
                '        _ProtectionClassId = "21"
                '    Case "10"
                '        _ProtectionClassId = "22"
                '    Case Else 'added 5/2/2013
                '        _ProtectionClassId = ""
                'End Select
                'updated 12/12/2013
                _ProtectionClassId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuoteHelperClass.QuickQuotePropertyName.ProtectionClassId, _ProtectionClass)
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 150</remarks>
        Public Property AccountsReceivableOnPremisesExcessLimit As String
            Get
                Return _AccountsReceivableOnPremisesExcessLimit
            End Get
            Set(value As String)
                _AccountsReceivableOnPremisesExcessLimit = value
                qqHelper.ConvertToLimitFormat(_AccountsReceivableOnPremisesExcessLimit)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 151</remarks>
        Public Property ValuablePapersOnPremisesExcessLimit As String
            Get
                Return _ValuablePapersOnPremisesExcessLimit
            End Get
            Set(value As String)
                _ValuablePapersOnPremisesExcessLimit = value
                qqHelper.ConvertToLimitFormat(_ValuablePapersOnPremisesExcessLimit)
            End Set
        End Property

        Public Property YearBuilt As String
            Get
                Return _YearBuilt
            End Get
            Set(value As String)
                _YearBuilt = value
            End Set
        End Property

        'Public Property NumberOfSoleProprietors As String
        '    Get
        '        Return _NumberOfSoleProprietors
        '    End Get
        '    Set(value As String)
        '        _NumberOfSoleProprietors = value
        '    End Set
        'End Property
        'Public Property NumberOfCorporateOfficers As String
        '    Get
        '        Return _NumberOfCorporateOfficers
        '    End Get
        '    Set(value As String)
        '        _NumberOfCorporateOfficers = value
        '    End Set
        'End Property
        'Public Property NumberOfPartners As String
        '    Get
        '        Return _NumberOfPartners
        '    End Get
        '    Set(value As String)
        '        _NumberOfPartners = value
        '    End Set
        'End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond building classification; contains empty coverage w/ coveragecode_id 10058</remarks>
        Public Property NumberOfOfficersAndPartnersAndInsureds As String
            Get
                Return _NumberOfOfficersAndPartnersAndInsureds
            End Get
            Set(value As String)
                _NumberOfOfficersAndPartnersAndInsureds = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond building classification; contains empty coverage w/ coveragecode_id 10058</remarks>
        Public Property EmployeePayroll As String
            Get
                Return _EmployeePayroll
            End Get
            Set(value As String)
                _EmployeePayroll = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond building classification; contains empty coverage w/ coveragecode_id 10058</remarks>
        Public Property AnnualReceipts As String
            Get
                Return _AnnualReceipts
            End Get
            Set(value As String)
                _AnnualReceipts = value
            End Set
        End Property

        Public Property SquareFeet As String
            Get
                Return _SquareFeet
            End Get
            Set(value As String)
                _SquareFeet = value
            End Set
        End Property

        Public Property CentralHeatElectric As Boolean
            Get
                'Return _CentralHeatElectric
                'updated 7/31/2013
                Return _Updates.CentralHeatElectric
            End Get
            Set(value As Boolean)
                '_CentralHeatElectric = value
                'updated 7/31/2013
                _Updates.CentralHeatElectric = value
                'If _CentralHeatUpdateTypeId = "" Then
                '    _CentralHeatUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property CentralHeatGas As Boolean
            Get
                'Return _CentralHeatGas
                'updated 7/31/2013
                Return _Updates.CentralHeatGas
            End Get
            Set(value As Boolean)
                '_CentralHeatGas = value
                'updated 7/31/2013
                _Updates.CentralHeatGas = value
                'If _CentralHeatUpdateTypeId = "" Then
                '    _CentralHeatUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property CentralHeatOil As Boolean
            Get
                'Return _CentralHeatOil
                'updated 7/31/2013
                Return _Updates.CentralHeatOil
            End Get
            Set(value As Boolean)
                '_CentralHeatOil = value
                'updated 7/31/2013
                _Updates.CentralHeatOil = value
                'If _CentralHeatUpdateTypeId = "" Then
                '    _CentralHeatUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property CentralHeatOther As Boolean
            Get
                'Return _CentralHeatOther
                'updated 7/31/2013
                Return _Updates.CentralHeatOther
            End Get
            Set(value As Boolean)
                '_CentralHeatOther = value
                'updated 7/31/2013
                _Updates.CentralHeatOther = value
                'If _CentralHeatUpdateTypeId = "" Then
                '    _CentralHeatUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property CentralHeatOtherDescription As String
            Get
                'Return _CentralHeatOtherDescription
                'updated 7/31/2013
                Return _Updates.CentralHeatOtherDescription
            End Get
            Set(value As String)
                '_CentralHeatOtherDescription = value
                'updated 7/31/2013
                _Updates.CentralHeatOtherDescription = value
                'If _CentralHeatUpdateTypeId = "" Then
                '    _CentralHeatUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property CentralHeatUpdateTypeId As String
            Get
                'Return _CentralHeatUpdateTypeId
                'updated 7/31/2013
                Return _Updates.CentralHeatUpdateTypeId
            End Get
            Set(value As String)
                '_CentralHeatUpdateTypeId = value
                'updated 7/31/2013
                _Updates.CentralHeatUpdateTypeId = value
            End Set
        End Property
        Public Property CentralHeatUpdateYear As String
            Get
                'Return _CentralHeatUpdateYear
                'updated 7/31/2013
                Return _Updates.CentralHeatUpdateYear
            End Get
            Set(value As String)
                '_CentralHeatUpdateYear = value
                'updated 7/31/2013
                _Updates.CentralHeatUpdateYear = value
                'If _CentralHeatUpdateTypeId = "" Then
                '    _CentralHeatUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property ImprovementsDescription As String
            Get
                'Return _ImprovementsDescription
                'updated 7/31/2013
                Return _Updates.ImprovementsDescription
            End Get
            Set(value As String)
                '_ImprovementsDescription = value
                'updated 7/31/2013
                _Updates.ImprovementsDescription = value
            End Set
        End Property
        Public Property Electric100Amp As Boolean
            Get
                'Return _Electric100Amp
                'updated 7/31/2013
                Return _Updates.Electric100Amp
            End Get
            Set(value As Boolean)
                '_Electric100Amp = value
                'updated 7/31/2013
                _Updates.Electric100Amp = value
                'If _ElectricUpdateTypeId = "" Then
                '    _ElectricUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property Electric120Amp As Boolean
            Get
                'Return _Electric120Amp
                'updated 7/31/2013
                Return _Updates.Electric120Amp
            End Get
            Set(value As Boolean)
                '_Electric120Amp = value
                'updated 7/31/2013
                _Updates.Electric120Amp = value
                'If _ElectricUpdateTypeId = "" Then
                '    _ElectricUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property Electric200Amp As Boolean
            Get
                'Return _Electric200Amp
                'updated 7/31/2013
                Return _Updates.Electric200Amp
            End Get
            Set(value As Boolean)
                '_Electric200Amp = value
                'updated 7/31/2013
                _Updates.Electric200Amp = value
                'If _ElectricUpdateTypeId = "" Then
                '    _ElectricUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property Electric60Amp As Boolean
            Get
                'Return _Electric60Amp
                'updated 7/31/2013
                Return _Updates.Electric60Amp
            End Get
            Set(value As Boolean)
                '_Electric60Amp = value
                'updated 7/31/2013
                _Updates.Electric60Amp = value
                'If _ElectricUpdateTypeId = "" Then
                '    _ElectricUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property ElectricBurningUnit As Boolean
            Get
                'Return _ElectricBurningUnit
                'updated 7/31/2013
                Return _Updates.ElectricBurningUnit
            End Get
            Set(value As Boolean)
                '_ElectricBurningUnit = value
                'updated 7/31/2013
                _Updates.ElectricBurningUnit = value
                'If _ElectricUpdateTypeId = "" Then
                '    _ElectricUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property ElectricCircuitBreaker As Boolean
            Get
                'Return _ElectricCircuitBreaker
                'updated 7/31/2013
                Return _Updates.ElectricCircuitBreaker
            End Get
            Set(value As Boolean)
                '_ElectricCircuitBreaker = value
                'updated 7/31/2013
                _Updates.ElectricCircuitBreaker = value
                'If _ElectricUpdateTypeId = "" Then
                '    _ElectricUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property ElectricFuses As Boolean
            Get
                'Return _ElectricFuses
                'updated 7/31/2013
                Return _Updates.ElectricFuses
            End Get
            Set(value As Boolean)
                '_ElectricFuses = value
                'updated 7/31/2013
                _Updates.ElectricFuses = value
                'If _ElectricUpdateTypeId = "" Then
                '    _ElectricUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property ElectricSpaceHeater As Boolean
            Get
                'Return _ElectricSpaceHeater
                'updated 7/31/2013
                Return _Updates.ElectricSpaceHeater
            End Get
            Set(value As Boolean)
                '_ElectricSpaceHeater = value
                'updated 7/31/2013
                _Updates.ElectricSpaceHeater = value
                'If _ElectricUpdateTypeId = "" Then
                '    _ElectricUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property ElectricUpdateTypeId As String
            Get
                'Return _ElectricUpdateTypeId
                'updated 7/31/2013
                Return _Updates.ElectricUpdateTypeId
            End Get
            Set(value As String)
                '_ElectricUpdateTypeId = value
                'updated 7/31/2013
                _Updates.ElectricUpdateTypeId = value
            End Set
        End Property
        Public Property ElectricUpdateYear As String
            Get
                'Return _ElectricUpdateYear
                'updated 7/31/2013
                Return _Updates.ElectricUpdateYear
            End Get
            Set(value As String)
                '_ElectricUpdateYear = value
                'updated 7/31/2013
                _Updates.ElectricUpdateYear = value
                'If _ElectricUpdateTypeId = "" Then
                '    _ElectricUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property PlumbingCopper As Boolean
            Get
                'Return _PlumbingCopper
                'updated 7/31/2013
                Return _Updates.PlumbingCopper
            End Get
            Set(value As Boolean)
                '_PlumbingCopper = value
                'updated 7/31/2013
                _Updates.PlumbingCopper = value
                'If _PlumbingUpdateTypeId = "" Then
                '    _PlumbingUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property PlumbingGalvanized As Boolean
            Get
                'Return _PlumbingGalvanized
                'updated 7/31/2013
                Return _Updates.PlumbingGalvanized
            End Get
            Set(value As Boolean)
                '_PlumbingGalvanized = value
                'updated 7/31/2013
                _Updates.PlumbingGalvanized = value
                'If _PlumbingUpdateTypeId = "" Then
                '    _PlumbingUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property PlumbingPlastic As Boolean
            Get
                'Return _PlumbingPlastic
                'updated 7/31/2013
                Return _Updates.PlumbingPlastic
            End Get
            Set(value As Boolean)
                '_PlumbingPlastic = value
                'updated 7/31/2013
                _Updates.PlumbingPlastic = value
                'If _PlumbingUpdateTypeId = "" Then
                '    _PlumbingUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property PlumbingUpdateTypeId As String
            Get
                'Return _PlumbingUpdateTypeId
                'updated 7/31/2013
                Return _Updates.PlumbingUpdateTypeId
            End Get
            Set(value As String)
                '_PlumbingUpdateTypeId = value
                'updated 7/31/2013
                _Updates.PlumbingUpdateTypeId = value
            End Set
        End Property
        Public Property PlumbingUpdateYear As String
            Get
                'Return _PlumbingUpdateYear
                'updated 7/31/2013
                Return _Updates.PlumbingUpdateYear
            End Get
            Set(value As String)
                '_PlumbingUpdateYear = value
                'updated 7/31/2013
                _Updates.PlumbingUpdateYear = value
                'If _PlumbingUpdateTypeId = "" Then
                '    _PlumbingUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property RoofAsphaltShingle As Boolean
            Get
                'Return _RoofAsphaltShingle
                'updated 7/31/2013
                Return _Updates.RoofAsphaltShingle
            End Get
            Set(value As Boolean)
                '_RoofAsphaltShingle = value
                'updated 7/31/2013
                _Updates.RoofAsphaltShingle = value
                'If _RoofUpdateTypeId = "" Then
                '    _RoofUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property RoofMetal As Boolean
            Get
                'Return _RoofMetal
                'updated 7/31/2013
                Return _Updates.RoofMetal
            End Get
            Set(value As Boolean)
                '_RoofMetal = value
                'updated 7/31/2013
                _Updates.RoofMetal = value
                'If _RoofUpdateTypeId = "" Then
                '    _RoofUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property RoofOther As Boolean
            Get
                'Return _RoofOther
                'updated 7/31/2013
                Return _Updates.RoofOther
            End Get
            Set(value As Boolean)
                '_RoofOther = value
                'updated 7/31/2013
                _Updates.RoofOther = value
                'If _RoofUpdateTypeId = "" Then
                '    _RoofUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property RoofOtherDescription As String
            Get
                'Return _RoofOtherDescription
                'updated 7/31/2013
                Return _Updates.RoofOtherDescription
            End Get
            Set(value As String)
                '_RoofOtherDescription = value
                'updated 7/31/2013
                _Updates.RoofOtherDescription = value
                'If _RoofUpdateTypeId = "" Then
                '    _RoofUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property RoofSlate As Boolean
            Get
                'Return _RoofSlate
                'updated 7/31/2013
                Return _Updates.RoofSlate
            End Get
            Set(value As Boolean)
                '_RoofSlate = value
                'updated 7/31/2013
                _Updates.RoofSlate = value
                'If _RoofUpdateTypeId = "" Then
                '    _RoofUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property RoofUpdateTypeId As String
            Get
                'Return _RoofUpdateTypeId
                'updated 7/31/2013
                Return _Updates.RoofUpdateTypeId
            End Get
            Set(value As String)
                '_RoofUpdateTypeId = value
                'updated 7/31/2013
                _Updates.RoofUpdateTypeId = value
            End Set
        End Property
        Public Property RoofUpdateYear As String
            Get
                'Return _RoofUpdateYear
                'updated 7/31/2013
                Return _Updates.RoofUpdateYear
            End Get
            Set(value As String)
                '_RoofUpdateYear = value
                'updated 7/31/2013
                _Updates.RoofUpdateYear = value
                'If _RoofUpdateTypeId = "" Then
                '    _RoofUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property RoofWood As Boolean
            Get
                'Return _RoofWood
                'updated 7/31/2013
                Return _Updates.RoofWood
            End Get
            Set(value As Boolean)
                '_RoofWood = value
                'updated 7/31/2013
                _Updates.RoofWood = value
                'If _RoofUpdateTypeId = "" Then
                '    _RoofUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property SupplementalHeatBurningUnit As Boolean
            Get
                'Return _SupplementalHeatBurningUnit
                'updated 7/31/2013
                Return _Updates.SupplementalHeatBurningUnit
            End Get
            Set(value As Boolean)
                '_SupplementalHeatBurningUnit = value
                'updated 7/31/2013
                _Updates.SupplementalHeatBurningUnit = value
                'If _SupplementalHeatUpdateTypeId = "" Then
                '    _SupplementalHeatUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property SupplementalHeatFireplace As Boolean
            Get
                'Return _SupplementalHeatFireplace
                'updated 7/31/2013
                Return _Updates.SupplementalHeatFireplace
            End Get
            Set(value As Boolean)
                '_SupplementalHeatFireplace = value
                'updated 7/31/2013
                _Updates.SupplementalHeatFireplace = value
                'If _SupplementalHeatUpdateTypeId = "" Then
                '    _SupplementalHeatUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property SupplementalHeatFireplaceInsert As Boolean
            Get
                'Return _SupplementalHeatFireplaceInsert
                'updated 7/31/2013
                Return _Updates.SupplementalHeatFireplaceInsert
            End Get
            Set(value As Boolean)
                '_SupplementalHeatFireplaceInsert = value
                'updated 7/31/2013
                _Updates.SupplementalHeatFireplaceInsert = value
                'If _SupplementalHeatUpdateTypeId = "" Then
                '    _SupplementalHeatUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property SupplementalHeatNA As Boolean
            Get
                'Return _SupplementalHeatNA
                'updated 7/31/2013
                Return _Updates.SupplementalHeatNA
            End Get
            Set(value As Boolean)
                '_SupplementalHeatNA = value
                'updated 7/31/2013
                _Updates.SupplementalHeatNA = value
                'If _SupplementalHeatUpdateTypeId = "" Then
                '    _SupplementalHeatUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property SupplementalHeatSolidFuel As Boolean
            Get
                'Return _SupplementalHeatSolidFuel
                'updated 7/31/2013
                Return _Updates.SupplementalHeatSolidFuel
            End Get
            Set(value As Boolean)
                '_SupplementalHeatSolidFuel = value
                'updated 7/31/2013
                _Updates.SupplementalHeatSolidFuel = value
                'If _SupplementalHeatUpdateTypeId = "" Then
                '    _SupplementalHeatUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property SupplementalHeatSpaceHeater As Boolean
            Get
                'Return _SupplementalHeatSpaceHeater
                'updated 7/31/2013
                Return _Updates.SupplementalHeatSpaceHeater
            End Get
            Set(value As Boolean)
                '_SupplementalHeatSpaceHeater = value
                'updated 7/31/2013
                _Updates.SupplementalHeatSpaceHeater = value
                'If _SupplementalHeatUpdateTypeId = "" Then
                '    _SupplementalHeatUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property SupplementalHeatUpdateTypeId As String
            Get
                'Return _SupplementalHeatUpdateTypeId
                'updated 7/31/2013
                Return _Updates.SupplementalHeatUpdateTypeId
            End Get
            Set(value As String)
                '_SupplementalHeatUpdateTypeId = value
                'updated 7/31/2013
                _Updates.SupplementalHeatUpdateTypeId = value
            End Set
        End Property
        Public Property SupplementalHeatUpdateYear As String
            Get
                'Return _SupplementalHeatUpdateYear
                'updated 7/31/2013
                Return _Updates.SupplementalHeatUpdateYear
            End Get
            Set(value As String)
                '_SupplementalHeatUpdateYear = value
                'updated 7/31/2013
                _Updates.SupplementalHeatUpdateYear = value
                'If _SupplementalHeatUpdateTypeId = "" Then
                '    _SupplementalHeatUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property WindowsUpdateTypeId As String
            Get
                'Return _WindowsUpdateTypeId
                'updated 7/31/2013
                Return _Updates.WindowsUpdateTypeId
            End Get
            Set(value As String)
                '_WindowsUpdateTypeId = value
                'updated 7/31/2013
                _Updates.WindowsUpdateTypeId = value
            End Set
        End Property
        Public Property WindowsUpdateYear As String
            Get
                'Return _WindowsUpdateYear
                'updated 7/31/2013
                Return _Updates.WindowsUpdateYear
            End Get
            Set(value As String)
                '_WindowsUpdateYear = value
                'updated 7/31/2013
                _Updates.WindowsUpdateYear = value
                'If _WindowsUpdateTypeId = "" Then
                '    _WindowsUpdateTypeId = "2" 'Complete; 1=Partial; 0=N/A
                'End If
            End Set
        End Property
        Public Property Updates As QuickQuoteUpdatesRecord 'added 7/31/2013
            Get
                SetObjectsParent(_Updates)
                Return _Updates
            End Get
            Set(value As QuickQuoteUpdatesRecord)
                _Updates = value
                SetObjectsParent(_Updates)
            End Set
        End Property

        Public Property AdditionalInterests As Generic.List(Of QuickQuoteAdditionalInterest)
            Get
                Return _AdditionalInterests
            End Get
            Set(value As Generic.List(Of QuickQuoteAdditionalInterest))
                _AdditionalInterests = value
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21032; stored in xml at policy level</remarks>
        Public Property HasBarbersProfessionalLiability As Boolean
            Get
                Return _HasBarbersProfessionalLiability
            End Get
            Set(value As Boolean)
                _HasBarbersProfessionalLiability = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21032; stored in xml at policy level</remarks>
        Public Property BarbersProfessionalLiabilityFullTimeEmpNum As String
            Get
                Return _BarbersProfessionalLiabilityFullTimeEmpNum
            End Get
            Set(value As String)
                _BarbersProfessionalLiabilityFullTimeEmpNum = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21032; stored in xml at policy level</remarks>
        Public Property BarbersProfessionalLiabilityPartTimeEmpNum As String
            Get
                Return _BarbersProfessionalLiabilityPartTimeEmpNum
            End Get
            Set(value As String)
                _BarbersProfessionalLiabilityPartTimeEmpNum = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21033; stored in xml at policy level</remarks>
        Public Property HasBeauticiansProfessionalLiability As Boolean
            Get
                Return _HasBeauticiansProfessionalLiability
            End Get
            Set(value As Boolean)
                _HasBeauticiansProfessionalLiability = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21033; stored in xml at policy level</remarks>
        Public Property BeauticiansProfessionalLiabilityFullTimeEmpNum As String
            Get
                Return _BeauticiansProfessionalLiabilityFullTimeEmpNum
            End Get
            Set(value As String)
                _BeauticiansProfessionalLiabilityFullTimeEmpNum = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21033; stored in xml at policy level</remarks>
        Public Property BeauticiansProfessionalLiabilityPartTimeEmpNum As String
            Get
                Return _BeauticiansProfessionalLiabilityPartTimeEmpNum
            End Get
            Set(value As String)
                _BeauticiansProfessionalLiabilityPartTimeEmpNum = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21034; stored in xml at policy level</remarks>
        Public Property HasFuneralDirectorsProfessionalLiability As Boolean
            Get
                Return _HasFuneralDirectorsProfessionalLiability
            End Get
            Set(value As Boolean)
                _HasFuneralDirectorsProfessionalLiability = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21034; stored in xml at policy level</remarks>
        Public Property FuneralDirectorsProfessionalLiabilityEmpNum As String
            Get
                Return _FuneralDirectorsProfessionalLiabilityEmpNum
            End Get
            Set(value As String)
                _FuneralDirectorsProfessionalLiabilityEmpNum = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21036; stored in xml at policy level</remarks>
        Public Property HasPrintersProfessionalLiability As Boolean
            Get
                Return _HasPrintersProfessionalLiability
            End Get
            Set(value As Boolean)
                _HasPrintersProfessionalLiability = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21036; stored in xml at policy level</remarks>
        Public Property PrintersProfessionalLiabilityLocNum As String
            Get
                Return _PrintersProfessionalLiabilityLocNum
            End Get
            Set(value As String)
                _PrintersProfessionalLiabilityLocNum = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21058; stored in xml at policy level</remarks>
        Public Property HasSelfStorageFacility As Boolean
            Get
                Return _HasSelfStorageFacility
            End Get
            Set(value As Boolean)
                _HasSelfStorageFacility = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21058; stored in xml at policy level</remarks>
        Public Property SelfStorageFacilityLimit As String
            Get
                Return _SelfStorageFacilityLimit
            End Get
            Set(value As String)
                _SelfStorageFacilityLimit = value
                qqHelper.ConvertToLimitFormat(_SelfStorageFacilityLimit)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 164; stored in xml at policy level</remarks>
        Public Property HasVeterinariansProfessionalLiability As Boolean
            Get
                Return _HasVeterinariansProfessionalLiability
            End Get
            Set(value As Boolean)
                _HasVeterinariansProfessionalLiability = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 164; stored in xml at policy level</remarks>
        Public Property VeterinariansProfessionalLiabilityEmpNum As String
            Get
                Return _VeterinariansProfessionalLiabilityEmpNum
            End Get
            Set(value As String)
                _VeterinariansProfessionalLiabilityEmpNum = value
            End Set
        End Property

        '3/9/2017 - BOP stuff
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 164; stored in xml at policy level</remarks>
        Public Property HasPharmacistProfessionalLiability As Boolean
            Get
                Return _HasPharmacistProfessionalLiability
            End Get
            Set(value As Boolean)
                _HasPharmacistProfessionalLiability = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 164; stored in xml at policy level</remarks>
        Public Property PharmacistAnnualGrossSales As String
            Get
                Return _PharmacistAnnualGrossSales
            End Get
            Set(value As String)
                _PharmacistAnnualGrossSales = value
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21035; stored in xml at policy level</remarks>
        Public Property HasOpticalAndHearingAidProfessionalLiability As Boolean
            Get
                Return _HasOpticalAndHearingAidProfessionalLiability
            End Get
            Set(value As Boolean)
                _HasOpticalAndHearingAidProfessionalLiability = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21035; stored in xml at policy level</remarks>
        Public Property OpticalAndHearingAidProfessionalLiabilityEmpNum As String
            Get
                Return _OpticalAndHearingAidProfessionalLiabilityEmpNum
            End Get
            Set(value As String)
                _OpticalAndHearingAidProfessionalLiabilityEmpNum = value
            End Set
        End Property

        '3/9/2017 - BOP stuff (motel and photography in this block)
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80376</remarks>
        Public Property HasMotelCoverage As Boolean
            Get
                Return _HasMotelCoverage
            End Get
            Set(value As Boolean)
                _HasMotelCoverage = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80376</remarks>
        Public Property MotelCoveragePerGuestLimitId As String
            Get
                Return _MotelCoveragePerGuestLimitId '371 - 1,000/25,000, 372 - 2,000/50,000, 373 - 3,000/75,000, 374 - 4,000/100,000
            End Get
            Set(value As String)
                _MotelCoveragePerGuestLimitId = value
                Select Case _MotelCoveragePerGuestLimitId
                    Case "371"
                        _MotelCoveragePerGuestLimit = "1,000/25,000"
                    Case "372"
                        _MotelCoveragePerGuestLimit = "2,000/50,000"
                    Case "373"
                        _MotelCoveragePerGuestLimit = "3,000/75,000"
                    Case "374"
                        _MotelCoveragePerGuestLimit = "4,000/100,000"
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80376</remarks>
        Public Property MotelCoveragePerGuestLimit As String
            Get
                Return _MotelCoveragePerGuestLimit '371 - 1,000/25,000, 372 - 2,000/50,000, 373 - 3,000/75,000, 374 - 4,000/100,000
            End Get
            Set(value As String)
                _MotelCoveragePerGuestLimit = value
                Select Case _MotelCoveragePerGuestLimit
                    Case "1000/25000"
                        _MotelCoveragePerGuestLimitId = "371"
                    Case "2000/50000"
                        _MotelCoveragePerGuestLimitId = "372"
                    Case "3000/75000"
                        _MotelCoveragePerGuestLimitId = "373"
                    Case "4000/100000"
                        _MotelCoveragePerGuestLimitId = "374"
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80377</remarks>
        Public Property MotelCoverageSafeDepositLimitId As String
            Get
                Return _MotelCoverageSafeDepositLimitId '0 - N/A, 8 - 25,000, 9 - 50,000, 10 - 100,000, 55 - 250,000
            End Get
            Set(value As String)
                _MotelCoverageSafeDepositLimitId = value
                Select Case _MotelCoverageSafeDepositLimitId
                    Case "0"
                        _MotelCoverageSafeDepositLimit = "N/A"
                    Case "8"
                        _MotelCoverageSafeDepositLimit = "25,000"
                    Case "9"
                        _MotelCoverageSafeDepositLimit = "50,000"
                    Case "10"
                        _MotelCoverageSafeDepositLimit = "100,000"
                    Case "55"
                        _MotelCoverageSafeDepositLimit = "250,000"
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80377</remarks>
        Public Property MotelCoverageSafeDepositLimit As String
            Get
                Return _MotelCoverageSafeDepositLimit '0 - N/A, 8 - 25,000, 9 - 50,000, 10 - 100,000, 55 - 250,000
            End Get
            Set(value As String)
                _MotelCoverageSafeDepositLimit = value
                Select Case _MotelCoverageSafeDepositLimit
                    Case "N/A"
                        _MotelCoverageSafeDepositLimitId = "0"
                    Case "25000"
                        _MotelCoverageSafeDepositLimitId = "8"
                    Case "50000"
                        _MotelCoverageSafeDepositLimitId = "9"
                    Case "100000"
                        _MotelCoverageSafeDepositLimitId = "10"
                    Case "250000"
                        _MotelCoverageSafeDepositLimitId = "55"
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80377</remarks>
        Public Property MotelCoverageSafeDepositDeductibleId As String
            Get
                Return _MotelCoverageSafeDepositDeductibleId '40 - 0, 4 - 250,8 - 500,9 - 1,000,15 - 2,500
            End Get
            Set(value As String)
                _MotelCoverageSafeDepositDeductibleId = value
                Select Case _MotelCoverageSafeDepositDeductibleId
                    Case "40"
                        _MotelCoverageSafeDepositDeductible = "0"
                    Case "4"
                        _MotelCoverageSafeDepositDeductible = "250"
                    Case "8"
                        _MotelCoverageSafeDepositDeductible = "500"
                    Case "9"
                        _MotelCoverageSafeDepositDeductible = "1,000"
                    Case "15"
                        _MotelCoverageSafeDepositDeductible = "2,500"
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80377</remarks>
        Public Property MotelCoverageSafeDepositDeductible As String
            Get
                Return _MotelCoverageSafeDepositDeductible
            End Get
            Set(value As String)
                _MotelCoverageSafeDepositDeductible = value '40 - 0, 4 - 250,8 - 500,9 - 1,000,15 - 2,500
                Select Case _MotelCoverageSafeDepositDeductible
                    Case "0"
                        _MotelCoverageSafeDepositDeductibleId = "40"
                    Case "250"
                        _MotelCoverageSafeDepositDeductibleId = "4"
                    Case "500"
                        _MotelCoverageSafeDepositDeductibleId = "8"
                    Case "1000"
                        _MotelCoverageSafeDepositDeductibleId = "9"
                    Case "2500"
                        _MotelCoverageSafeDepositDeductibleId = "15"
                    Case Else
                        _MotelCoverageSafeDepositDeductibleId = "40"
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80398</remarks>
        Public Property HasPhotographyCoverage As Boolean
            Get
                Return _HasPhotographyCoverage
            End Get
            Set(value As Boolean)
                _HasPhotographyCoverage = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80398</remarks>
        Public Property HasPhotographyCoverageScheduledCoverages As Boolean
            Get
                Return _HasPhotographyCoverageScheduledCoverages
            End Get
            Set(value As Boolean)
                _HasPhotographyCoverageScheduledCoverages = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80398</remarks>
        Public ReadOnly Property PhotographyTotalScheduledLimits As String
            Get
                Dim total As Decimal = 0
                If _PhotographyScheduledCoverages IsNot Nothing AndAlso _PhotographyScheduledCoverages.Count > 0 Then
                    For Each cov As QuickQuoteCoverage In _PhotographyScheduledCoverages
                        If cov IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(cov.ManualLimitAmount) AndAlso IsNumeric(cov.ManualLimitAmount) Then
                            total += CDec(cov.ManualLimitAmount)
                        End If
                    Next
                End If
                Return total.ToString()
            End Get
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond Scheduled Item w/ scheduleditem_id 21248</remarks>
        Public Property PhotographyScheduledCoverages As List(Of QuickQuoteCoverage)
            Get
                SetParentOfListItems(_PhotographyScheduledCoverages, "{09860584-30E8-475E-A428-409826D39D24}")
                Return _PhotographyScheduledCoverages
            End Get
            Set(value As List(Of QuickQuoteCoverage))
                _PhotographyScheduledCoverages = value
                SetParentOfListItems(_PhotographyScheduledCoverages, "{09860584-30E8-475E-A428-409826D39D24}")
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80378</remarks>
        Public Property HasPhotographyMakeupAndHair As Boolean
            Get
                Return _HasPhotographyMakeupAndHair
            End Get
            Set(value As Boolean)
                _HasPhotographyMakeupAndHair = value
            End Set
        End Property

        Public Property ClassificationCode As QuickQuoteClassificationCode
            Get
                SetObjectsParent(_ClassificationCode)
                Return _ClassificationCode
            End Get
            Set(value As QuickQuoteClassificationCode)
                _ClassificationCode = value
                SetObjectsParent(_ClassificationCode)
            End Set
        End Property
        Public Property EarthquakeBuildingClassificationTypeId As String 'comes from Diamond's EarthquakeBuildingClassificationType table
            Get
                Return _EarthquakeBuildingClassificationTypeId
            End Get
            Set(value As String)
                _EarthquakeBuildingClassificationTypeId = value
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21155; also sets flag on building coverage (165)</remarks>
        Public Property EarthquakeApplies As Boolean
            Get
                Return _EarthquakeApplies
            End Get
            Set(value As Boolean)
                _EarthquakeApplies = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 165</remarks>
        Public Property CauseOfLossTypeId As String
            Get
                Return _CauseOfLossTypeId
            End Get
            Set(value As String)
                _CauseOfLossTypeId = value
                '_CauseOfLossType = ""
                'If IsNumeric(_CauseOfLossTypeId) = True Then
                '    Select Case _CauseOfLossTypeId
                '        Case "0"
                '            _CauseOfLossType = "N/A"
                '        Case "1"
                '            _CauseOfLossType = "Basic Form"
                '        Case "2"
                '            _CauseOfLossType = "Broad Form"
                '        Case "3"
                '            _CauseOfLossType = "Special Form Including Theft"
                '        Case "4"
                '            _CauseOfLossType = "Special Form Excluding Theft"
                '    End Select
                'End If
                'updated 12/12/2013
                _CauseOfLossType = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuoteHelperClass.QuickQuotePropertyName.CauseOfLossTypeId, _CauseOfLossTypeId)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 165</remarks>
        Public Property CauseOfLossType As String
            Get
                Return _CauseOfLossType
            End Get
            Set(value As String)
                _CauseOfLossType = value
                'Select Case _CauseOfLossType
                '    Case "N/A"
                '        _CauseOfLossTypeId = "0"
                '    Case "Basic Form"
                '        _CauseOfLossTypeId = "1"
                '    Case "Broad Form"
                '        _CauseOfLossTypeId = "2"
                '    Case "Special Form Including Theft"
                '        _CauseOfLossTypeId = "3"
                '    Case "Special Form Excluding Theft"
                '        _CauseOfLossTypeId = "4"
                '    Case Else
                '        _CauseOfLossTypeId = ""
                'End Select
                'updated 12/12/2013
                _CauseOfLossTypeId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuoteHelperClass.QuickQuotePropertyName.CauseOfLossTypeId, _CauseOfLossType)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 165</remarks>
        Public Property DeductibleId As String
            Get
                Return _DeductibleId
            End Get
            Set(value As String)
                _DeductibleId = value
                _Deductible = ""
                If IsNumeric(_DeductibleId) = True Then
                    Select Case _DeductibleId
                        Case "0"
                            _Deductible = "N/A"
                        Case "4"
                            _Deductible = "250"
                        Case "8"
                            _Deductible = "500"
                        Case "9"
                            _Deductible = "1,000"
                        Case "15"
                            _Deductible = "2,500"
                        Case "16"
                            _Deductible = "5,000"
                        Case "17"
                            _Deductible = "10,000"
                        Case "19"
                            _Deductible = "25,000"
                        Case "20"
                            _Deductible = "50,000"
                        Case "21"
                            _Deductible = "75,000"
                        Case "32"
                            _Deductible = "1%"
                        Case "33"
                            _Deductible = "2%"
                        Case "34"
                            _Deductible = "5%"
                        Case "42"
                            _Deductible = "Same"
                        Case "36"
                            _Deductible = "10%"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 165</remarks>
        Public Property Deductible As String
            Get
                Return _Deductible
            End Get
            Set(value As String)
                _Deductible = value
                Select Case _Deductible
                    Case "N/A"
                        _DeductibleId = "0"
                    Case "250"
                        _DeductibleId = "4"
                    Case "500"
                        _DeductibleId = "8"
                    Case "1,000"
                        _DeductibleId = "9"
                    Case "2,500"
                        _DeductibleId = "15"
                    Case "5,000"
                        _DeductibleId = "16"
                    Case "10,000"
                        _DeductibleId = "17"
                    Case "25,000"
                        _DeductibleId = "19"
                    Case "50,000"
                        _DeductibleId = "20"
                    Case "75,000"
                        _DeductibleId = "21"
                    Case "1%"
                        _DeductibleId = "32"
                    Case "2%"
                        _DeductibleId = "33"
                    Case "5%"
                        _DeductibleId = "34"
                    Case "Same"
                        _DeductibleId = "42"
                    Case "10%"
                        _DeductibleId = "36"
                    Case Else
                        _DeductibleId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 165</remarks>
        Public Property CoinsuranceTypeId As String 'only N/A, 80%, 90%, 100% are used for CPR locations
            Get
                Return _CoinsuranceTypeId
            End Get
            Set(value As String)
                _CoinsuranceTypeId = value
                _CoinsuranceType = ""
                If IsNumeric(_CoinsuranceTypeId) = True Then
                    Select Case _CoinsuranceTypeId
                        Case "0"
                            _CoinsuranceType = "N/A"
                        Case "1"
                            _CoinsuranceType = "Waived"
                        Case "2"
                            _CoinsuranceType = "50%"
                        Case "3"
                            _CoinsuranceType = "60%"
                        Case "4"
                            _CoinsuranceType = "70%"
                        Case "5"
                            _CoinsuranceType = "80%"
                        Case "6"
                            _CoinsuranceType = "90%"
                        Case "7"
                            _CoinsuranceType = "100%"
                        Case "8"
                            _CoinsuranceType = "10%"
                        Case "9"
                            _CoinsuranceType = "20%"
                        Case "10"
                            _CoinsuranceType = "30%"
                        Case "11"
                            _CoinsuranceType = "40%"
                        Case "12"
                            _CoinsuranceType = "125%"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 165</remarks>
        Public Property CoinsuranceType As String 'only N/A, 80%, 90%, 100% are used for CPR locations
            Get
                Return _CoinsuranceType
            End Get
            Set(value As String)
                _CoinsuranceType = value
                Select Case _CoinsuranceType
                    Case "N/A"
                        _CoinsuranceTypeId = "0"
                    Case "Waived"
                        _CoinsuranceTypeId = "1"
                    Case "50%"
                        _CoinsuranceTypeId = "2"
                    Case "60%"
                        _CoinsuranceTypeId = "3"
                    Case "70%"
                        _CoinsuranceTypeId = "4"
                    Case "80%"
                        _CoinsuranceTypeId = "5"
                    Case "90%"
                        _CoinsuranceTypeId = "6"
                    Case "100%"
                        _CoinsuranceTypeId = "7"
                    Case "10%"
                        _CoinsuranceTypeId = "8"
                    Case "20%"
                        _CoinsuranceTypeId = "9"
                    Case "30%"
                        _CoinsuranceTypeId = "10"
                    Case "40%"
                        _CoinsuranceTypeId = "11"
                    Case "125%"
                        _CoinsuranceTypeId = "12"
                    Case Else
                        _CoinsuranceTypeId = ""
                End Select
            End Set
        End Property
        'Public Property ValuationMethodTypeId As String
        '    Get
        '        Return _ValuationMethodTypeId
        '    End Get
        '    Set(value As String)
        '        _ValuationMethodTypeId = value
        '        '(1=Replacement Cost; 2=Actual Cash Value; 3=Functional Building Valuation)
        '        _ValuationMethodType = ""
        '        If IsNumeric(_ValuationMethodTypeId) = True Then
        '            Select Case _ValuationMethodTypeId
        '                Case "1"
        '                    _ValuationMethodType = "Replacement Cost"
        '                Case "2"
        '                    _ValuationMethodType = "Actual Cash Value"
        '                Case "3"
        '                    _ValuationMethodType = "Functional Building Valuation"
        '            End Select
        '        End If
        '    End Set
        'End Property
        'Public Property ValuationMethodType As String
        '    Get
        '        Return _ValuationMethodType
        '    End Get
        '    Set(value As String)
        '        _ValuationMethodType = value
        '        Select Case _ValuationMethodType
        '            Case "Replacement Cost"
        '                _ValuationMethodTypeId = "1"
        '            Case "Actual Cash Value"
        '                _ValuationMethodTypeId = "2"
        '            Case "Functional Building Valuation"
        '                _ValuationMethodTypeId = "3"
        '            Case Else
        '                _ValuationMethodTypeId = ""
        '        End Select
        '    End Set
        'End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 165</remarks>
        Public Property RatingTypeId As String
            Get
                Return _RatingTypeId
            End Get
            Set(value As String)
                _RatingTypeId = value
                _RatingType = ""
                If IsNumeric(_RatingTypeId) = True Then
                    Select Case _RatingTypeId
                        Case "0"
                            _RatingType = "None"
                        Case "1"
                            _RatingType = "Class Rated"
                        Case "2"
                            _RatingType = "Specific Rated"
                        Case "3"
                            _RatingType = "Special Class Rate"
                        Case "4"
                            _RatingType = "Symbol"
                        Case "5"
                            _RatingType = "Specific"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 165</remarks>
        Public Property RatingType As String
            Get
                Return _RatingType
            End Get
            Set(value As String)
                _RatingType = value
                Select Case _RatingType
                    Case "None"
                        _RatingTypeId = "0"
                    Case "Class Rated"
                        _RatingTypeId = "1"
                    Case "Specific Rated"
                        _RatingTypeId = "2"
                    Case "Special Class Rate"
                        _RatingTypeId = "3"
                    Case "Symbol"
                        _RatingTypeId = "4"
                    Case "Specific"
                        _RatingTypeId = "5"
                    Case Else
                        _RatingTypeId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 165</remarks>
        Public Property InflationGuardTypeId As String
            Get
                Return _InflationGuardTypeId
            End Get
            Set(value As String)
                _InflationGuardTypeId = value
                _InflationGuardType = ""
                If IsNumeric(_InflationGuardTypeId) = True Then
                    Select Case _InflationGuardTypeId
                        Case "0"
                            _InflationGuardType = "N/A"
                        Case "1"
                            _InflationGuardType = "2"
                        Case "2"
                            _InflationGuardType = "4"
                        Case "3"
                            _InflationGuardType = "6"
                        Case "4"
                            _InflationGuardType = "8"
                        Case "5"
                            _InflationGuardType = "10"
                        Case "6"
                            _InflationGuardType = "12"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 165</remarks>
        Public Property InflationGuardType As String
            Get
                Return _InflationGuardType
            End Get
            Set(value As String)
                _InflationGuardType = value
                Select Case _InflationGuardType
                    Case "N/A"
                        _InflationGuardTypeId = "0"
                    Case "2"
                        _InflationGuardTypeId = "1"
                    Case "4"
                        _InflationGuardTypeId = "2"
                    Case "6"
                        _InflationGuardTypeId = "3"
                    Case "8"
                        _InflationGuardTypeId = "4"
                    Case "10"
                        _InflationGuardTypeId = "5"
                    Case "12"
                        _InflationGuardTypeId = "6"
                    Case Else
                        _InflationGuardTypeId = ""
                End Select
            End Set
        End Property

        Public Property ScheduledCoverages As Generic.List(Of QuickQuoteScheduledCoverage)
            Get
                Return _ScheduledCoverages
            End Get
            Set(value As Generic.List(Of QuickQuoteScheduledCoverage))
                _ScheduledCoverages = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropCov_PersonalPropertyLimit As String
            Get
                Return _PersPropCov_PersonalPropertyLimit
            End Get
            Set(value As String)
                _PersPropCov_PersonalPropertyLimit = value
                qqHelper.ConvertToLimitFormat(_PersPropCov_PersonalPropertyLimit)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropCov_PropertyTypeId As String
            Get
                Return _PersPropCov_PropertyTypeId
            End Get
            Set(value As String)
                _PersPropCov_PropertyTypeId = value
                _PersPropCov_PropertyType = ""
                If IsNumeric(_PersPropCov_PropertyTypeId) = True Then
                    Select Case _PersPropCov_PropertyTypeId
                        Case "0"
                            _PersPropCov_PropertyType = "N/A"
                        Case "1"
                            _PersPropCov_PropertyType = "Personal Property - Stock Only"
                        Case "2"
                            _PersPropCov_PropertyType = "Personal Property - Excluding Stock"
                        Case "3"
                            _PersPropCov_PropertyType = "Machinery and Equipment"
                        Case "4"
                            _PersPropCov_PropertyType = "Furniture"
                        Case "5"
                            _PersPropCov_PropertyType = "Fixtures"
                        Case "6"
                            _PersPropCov_PropertyType = "Tenants Improvements and Betterments"
                        Case "7"
                            _PersPropCov_PropertyType = "Personal Property - Including Stock"
                        Case "8"
                            _PersPropCov_PropertyType = "Personal Property of Others"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropCov_PropertyType As String
            Get
                Return _PersPropCov_PropertyType
            End Get
            Set(value As String)
                _PersPropCov_PropertyType = value
                Select Case _PersPropCov_PropertyType
                    Case "N/A"
                        _PersPropCov_PropertyTypeId = "0"
                    Case "Personal Property - Stock Only"
                        _PersPropCov_PropertyTypeId = "1"
                    Case "Personal Property - Excluding Stock"
                        _PersPropCov_PropertyTypeId = "2"
                    Case "Machinery and Equipment"
                        _PersPropCov_PropertyTypeId = "3"
                    Case "Furniture"
                        _PersPropCov_PropertyTypeId = "4"
                    Case "Fixtures"
                        _PersPropCov_PropertyTypeId = "5"
                    Case "Tenants Improvements and Betterments"
                        _PersPropCov_PropertyTypeId = "6"
                    Case "Personal Property - Including Stock"
                        _PersPropCov_PropertyTypeId = "7"
                    Case "Personal Property of Others"
                        _PersPropCov_PropertyTypeId = "8"
                    Case Else
                        _PersPropCov_PropertyTypeId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropCov_RiskTypeId As String
            Get
                Return _PersPropCov_RiskTypeId
            End Get
            Set(value As String)
                _PersPropCov_RiskTypeId = value
                _PersPropCov_RiskType = ""
                If IsNumeric(_PersPropCov_RiskTypeId) = True Then
                    Select Case _PersPropCov_RiskTypeId
                        Case "0"
                            _PersPropCov_RiskType = "N/A"
                        Case "1"
                            _PersPropCov_RiskType = "Not in Towing Business"
                        Case "2"
                            _PersPropCov_RiskType = "Tow Truck Operator"
                        Case "3"
                            _PersPropCov_RiskType = "Type 1 - Apartments and Condominiums - Residential Use Only"
                        Case "4"
                            _PersPropCov_RiskType = "Type 2 - Offices"
                        Case "5"
                            _PersPropCov_RiskType = "Type 3 - All Other Personal Property"
                        Case "6"
                            _PersPropCov_RiskType = "Mercantile or Non-Manufacturing"
                        Case "7"
                            _PersPropCov_RiskType = "Manufacturing"
                        Case "8"
                            _PersPropCov_RiskType = "Mining"
                        Case "9"
                            _PersPropCov_RiskType = "Rental Properties"
                        Case "10"
                            _PersPropCov_RiskType = "Combined Manufacturing And Mercantile"
                        Case "11"
                            _PersPropCov_RiskType = "Combined Manufacturing and Rental"
                        Case "12"
                            _PersPropCov_RiskType = "Combined Mercantile and Rental"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropCov_RiskType As String
            Get
                Return _PersPropCov_RiskType
            End Get
            Set(value As String)
                _PersPropCov_RiskType = value
                Select Case _PersPropCov_RiskType
                    Case "N/A"
                        _PersPropCov_RiskTypeId = "0"
                    Case "Not in Towing Business"
                        _PersPropCov_RiskTypeId = "1"
                    Case "Tow Truck Operator"
                        _PersPropCov_RiskTypeId = "2"
                    Case "Type 1 - Apartments and Condominiums - Residential Use Only"
                        _PersPropCov_RiskTypeId = "3"
                    Case "Type 2 - Offices"
                        _PersPropCov_RiskTypeId = "4"
                    Case "Type 3 - All Other Personal Property"
                        _PersPropCov_RiskTypeId = "5"
                    Case "Mercantile or Non-Manufacturing"
                        _PersPropCov_RiskTypeId = "6"
                    Case "Manufacturing"
                        _PersPropCov_RiskTypeId = "7"
                    Case "Mining"
                        _PersPropCov_RiskTypeId = "8"
                    Case "Rental Properties"
                        _PersPropCov_RiskTypeId = "9"
                    Case "Combined Manufacturing And Mercantile"
                        _PersPropCov_RiskTypeId = "10"
                    Case "Combined Manufacturing and Rental"
                        _PersPropCov_RiskTypeId = "11"
                    Case "Combined Mercantile and Rental"
                        _PersPropCov_RiskTypeId = "12"
                    Case Else
                        _PersPropCov_RiskTypeId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21160; also sets flag on coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropCov_EarthquakeApplies As Boolean
            Get
                Return _PersPropCov_EarthquakeApplies
            End Get
            Set(value As Boolean)
                _PersPropCov_EarthquakeApplies = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropCov_RatingTypeId As String
            Get
                Return _PersPropCov_RatingTypeId
            End Get
            Set(value As String)
                _PersPropCov_RatingTypeId = value
                _PersPropCov_RatingType = ""
                If IsNumeric(_PersPropCov_RatingTypeId) = True Then
                    Select Case _PersPropCov_RatingTypeId
                        Case "0"
                            _PersPropCov_RatingType = "None"
                        Case "1"
                            _PersPropCov_RatingType = "Class Rated"
                        Case "2"
                            _PersPropCov_RatingType = "Specific Rated"
                        Case "3"
                            _PersPropCov_RatingType = "Special Class Rate"
                        Case "4"
                            _PersPropCov_RatingType = "Symbol"
                        Case "5"
                            _PersPropCov_RatingType = "Specific"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropCov_RatingType As String
            Get
                Return _PersPropCov_RatingType
            End Get
            Set(value As String)
                _PersPropCov_RatingType = value
                Select Case _PersPropCov_RatingType
                    Case "None"
                        _PersPropCov_RatingTypeId = "0"
                    Case "Class Rated"
                        _PersPropCov_RatingTypeId = "1"
                    Case "Specific Rated"
                        _PersPropCov_RatingTypeId = "2"
                    Case "Special Class Rate"
                        _PersPropCov_RatingTypeId = "3"
                    Case "Symbol"
                        _PersPropCov_RatingTypeId = "4"
                    Case "Specific"
                        _PersPropCov_RatingTypeId = "5"
                    Case Else
                        _PersPropCov_RatingTypeId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropCov_CauseOfLossTypeId As String
            Get
                Return _PersPropCov_CauseOfLossTypeId
            End Get
            Set(value As String)
                _PersPropCov_CauseOfLossTypeId = value
                _PersPropCov_CauseOfLossType = ""
                If IsNumeric(_PersPropCov_CauseOfLossTypeId) = True Then
                    Select Case _PersPropCov_CauseOfLossTypeId
                        Case "0"
                            _PersPropCov_CauseOfLossType = "N/A"
                        Case "1"
                            _PersPropCov_CauseOfLossType = "Basic Form"
                        Case "2"
                            _PersPropCov_CauseOfLossType = "Broad Form"
                        Case "3"
                            _PersPropCov_CauseOfLossType = "Special Form Including Theft"
                        Case "4"
                            _PersPropCov_CauseOfLossType = "Special Form Excluding Theft"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropCov_CauseOfLossType As String
            Get
                Return _PersPropCov_CauseOfLossType
            End Get
            Set(value As String)
                _PersPropCov_CauseOfLossType = value
                Select Case _PersPropCov_CauseOfLossType
                    Case "N/A"
                        _PersPropCov_CauseOfLossTypeId = "0"
                    Case "Basic Form"
                        _PersPropCov_CauseOfLossTypeId = "1"
                    Case "Broad Form"
                        _PersPropCov_CauseOfLossTypeId = "2"
                    Case "Special Form Including Theft"
                        _PersPropCov_CauseOfLossTypeId = "3"
                    Case "Special Form Excluding Theft"
                        _PersPropCov_CauseOfLossTypeId = "4"
                    Case Else
                        _PersPropCov_CauseOfLossTypeId = ""
                End Select
            End Set
        End Property

        Public Property PersPropCov_IsAgreedValue As Boolean ' added 2/1/2016 for bug 4845 Matt A; 3/9/2017 - included in this library w/ BOP updates
            Get
                Return _PersPropCov_IsAgreedValue
            End Get
            Set(value As Boolean)
                _PersPropCov_IsAgreedValue = value
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropCov_DeductibleId As String
            Get
                Return _PersPropCov_DeductibleId
            End Get
            Set(value As String)
                _PersPropCov_DeductibleId = value
                _PersPropCov_Deductible = ""
                If IsNumeric(_PersPropCov_DeductibleId) = True Then
                    Select Case _PersPropCov_DeductibleId
                        Case "0"
                            _PersPropCov_Deductible = "N/A"
                        Case "4"
                            _PersPropCov_Deductible = "250"
                        Case "8"
                            _PersPropCov_Deductible = "500"
                        Case "9"
                            _PersPropCov_Deductible = "1,000"
                        Case "15"
                            _PersPropCov_Deductible = "2,500"
                        Case "16"
                            _PersPropCov_Deductible = "5,000"
                        Case "17"
                            _PersPropCov_Deductible = "10,000"
                        Case "19"
                            _PersPropCov_Deductible = "25,000"
                        Case "20"
                            _PersPropCov_Deductible = "50,000"
                        Case "21"
                            _PersPropCov_Deductible = "75,000"
                        Case "32"
                            _PersPropCov_Deductible = "1%"
                        Case "33"
                            _PersPropCov_Deductible = "2%"
                        Case "34"
                            _PersPropCov_Deductible = "5%"
                        Case "42"
                            _PersPropCov_Deductible = "Same"
                        Case "36"
                            _PersPropCov_Deductible = "10%"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropCov_Deductible As String
            Get
                Return _PersPropCov_Deductible
            End Get
            Set(value As String)
                _PersPropCov_Deductible = value
                Select Case _PersPropCov_Deductible
                    Case "N/A"
                        _PersPropCov_DeductibleId = "0"
                    Case "250"
                        _PersPropCov_DeductibleId = "4"
                    Case "500"
                        _PersPropCov_DeductibleId = "8"
                    Case "1,000"
                        _PersPropCov_DeductibleId = "9"
                    Case "2,500"
                        _PersPropCov_DeductibleId = "15"
                    Case "5,000"
                        _PersPropCov_DeductibleId = "16"
                    Case "10,000"
                        _PersPropCov_DeductibleId = "17"
                    Case "25,000"
                        _PersPropCov_DeductibleId = "19"
                    Case "50,000"
                        _PersPropCov_DeductibleId = "20"
                    Case "75,000"
                        _PersPropCov_DeductibleId = "21"
                    Case "1%"
                        _PersPropCov_DeductibleId = "32"
                    Case "2%"
                        _PersPropCov_DeductibleId = "33"
                    Case "5%"
                        _PersPropCov_DeductibleId = "34"
                    Case "Same"
                        _PersPropCov_DeductibleId = "42"
                    Case "10%"
                        _PersPropCov_DeductibleId = "36"
                    Case Else
                        _PersPropCov_DeductibleId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropCov_CoinsuranceTypeId As String
            Get
                Return _PersPropCov_CoinsuranceTypeId
            End Get
            Set(value As String)
                _PersPropCov_CoinsuranceTypeId = value
                _PersPropCov_CoinsuranceType = ""
                If IsNumeric(_PersPropCov_CoinsuranceTypeId) = True Then
                    Select Case _PersPropCov_CoinsuranceTypeId
                        Case "0"
                            _PersPropCov_CoinsuranceType = "N/A"
                        Case "1"
                            _PersPropCov_CoinsuranceType = "Waived"
                        Case "2"
                            _PersPropCov_CoinsuranceType = "50%"
                        Case "3"
                            _PersPropCov_CoinsuranceType = "60%"
                        Case "4"
                            _PersPropCov_CoinsuranceType = "70%"
                        Case "5"
                            _PersPropCov_CoinsuranceType = "80%"
                        Case "6"
                            _PersPropCov_CoinsuranceType = "90%"
                        Case "7"
                            _PersPropCov_CoinsuranceType = "100%"
                        Case "8"
                            _PersPropCov_CoinsuranceType = "10%"
                        Case "9"
                            _PersPropCov_CoinsuranceType = "20%"
                        Case "10"
                            _PersPropCov_CoinsuranceType = "30%"
                        Case "11"
                            _PersPropCov_CoinsuranceType = "40%"
                        Case "12"
                            _PersPropCov_CoinsuranceType = "125%"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropCov_CoinsuranceType As String
            Get
                Return _PersPropCov_CoinsuranceType
            End Get
            Set(value As String)
                _PersPropCov_CoinsuranceType = value
                Select Case _PersPropCov_CoinsuranceType
                    Case "N/A"
                        _PersPropCov_CoinsuranceTypeId = "0"
                    Case "Waived"
                        _PersPropCov_CoinsuranceTypeId = "1"
                    Case "50%"
                        _PersPropCov_CoinsuranceTypeId = "2"
                    Case "60%"
                        _PersPropCov_CoinsuranceTypeId = "3"
                    Case "70%"
                        _PersPropCov_CoinsuranceTypeId = "4"
                    Case "80%"
                        _PersPropCov_CoinsuranceTypeId = "5"
                    Case "90%"
                        _PersPropCov_CoinsuranceTypeId = "6"
                    Case "100%"
                        _PersPropCov_CoinsuranceTypeId = "7"
                    Case "10%"
                        _PersPropCov_CoinsuranceTypeId = "8"
                    Case "20%"
                        _PersPropCov_CoinsuranceTypeId = "9"
                    Case "30%"
                        _PersPropCov_CoinsuranceTypeId = "10"
                    Case "40%"
                        _PersPropCov_CoinsuranceTypeId = "11"
                    Case "125%"
                        _PersPropCov_CoinsuranceTypeId = "12"
                    Case Else
                        _PersPropCov_CoinsuranceTypeId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropCov_ValuationId As String
            Get
                Return _PersPropCov_ValuationId
            End Get
            Set(value As String)
                _PersPropCov_ValuationId = value
                '(1=Replacement Cost; 2=Actual Cash Value; 3=Functional Building Valuation)
                _PersPropCov_Valuation = ""
                If IsNumeric(_PersPropCov_ValuationId) = True Then
                    Select Case _PersPropCov_ValuationId
                        Case "-1" 'added 10/19/2012 for CPR to match specs
                            _PersPropCov_Valuation = "N/A"
                        Case "1"
                            _PersPropCov_Valuation = "Replacement Cost"
                        Case "2"
                            _PersPropCov_Valuation = "Actual Cash Value"
                        Case "3"
                            _PersPropCov_Valuation = "Functional Building Valuation"
                        Case "7" 'added 10/18/2012 for CPR
                            _PersPropCov_Valuation = "Functional Replacement Cost"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropCov_Valuation As String
            Get
                Return _PersPropCov_Valuation
            End Get
            Set(value As String)
                _PersPropCov_Valuation = value
                Select Case _PersPropCov_Valuation
                    Case "N/A" 'added 10/19/2012 for CPR to match specs
                        _PersPropCov_ValuationId = "-1"
                    Case "Replacement Cost"
                        _PersPropCov_ValuationId = "1"
                    Case "Actual Cash Value"
                        _PersPropCov_ValuationId = "2"
                    Case "Functional Building Valuation"
                        _PersPropCov_ValuationId = "3"
                    Case "Functional Replacement Cost" 'added 10/18/2012 for CPR
                        _PersPropCov_ValuationId = "7"
                    Case Else
                        _PersPropCov_ValuationId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropCov_QuotedPremium As String
            Get
                'Return _PersPropCov_QuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_PersPropCov_QuotedPremium)
            End Get
            Set(value As String)
                _PersPropCov_QuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_PersPropCov_QuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropCov_ClassificationCode As QuickQuoteClassificationCode
            Get
                SetObjectsParent(_PersPropCov_ClassificationCode)
                Return _PersPropCov_ClassificationCode
            End Get
            Set(value As QuickQuoteClassificationCode)
                _PersPropCov_ClassificationCode = value
                SetObjectsParent(_PersPropCov_ClassificationCode)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId = 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropOfOthers_PersonalPropertyLimit As String
            Get
                Return _PersPropOfOthers_PersonalPropertyLimit
            End Get
            Set(value As String)
                _PersPropOfOthers_PersonalPropertyLimit = value
                qqHelper.ConvertToLimitFormat(_PersPropOfOthers_PersonalPropertyLimit)
            End Set
        End Property
        'Public Property PersPropOfOthers_PropertyTypeId As String'defaulting
        '    Get
        '        Return _PersPropOfOthers_PropertyTypeId
        '    End Get
        '    Set(value As String)
        '        _PersPropOfOthers_PropertyTypeId = value
        '        _PersPropOfOthers_PropertyType = ""
        '        If IsNumeric(_PersPropOfOthers_PropertyTypeId) = True Then
        '            Select Case _PersPropOfOthers_PropertyTypeId
        '                Case "0"
        '                    _PersPropOfOthers_PropertyType = "N/A"
        '                Case "1"
        '                    _PersPropOfOthers_PropertyType = "Personal Property - Stock Only"
        '                Case "2"
        '                    _PersPropOfOthers_PropertyType = "Personal Property - Excluding Stock"
        '                Case "3"
        '                    _PersPropOfOthers_PropertyType = "Machinery and Equipment"
        '                Case "4"
        '                    _PersPropOfOthers_PropertyType = "Furniture"
        '                Case "5"
        '                    _PersPropOfOthers_PropertyType = "Fixtures"
        '                Case "6"
        '                    _PersPropOfOthers_PropertyType = "Tenants Improvements and Betterments"
        '                Case "7"
        '                    _PersPropOfOthers_PropertyType = "Personal Property - Including Stock"
        '                Case "8"
        '                    _PersPropOfOthers_PropertyType = "Personal Property of Others"
        '            End Select
        '        End If
        '    End Set
        'End Property
        'Public Property PersPropOfOthers_PropertyType As String
        '    Get
        '        Return _PersPropOfOthers_PropertyType
        '    End Get
        '    Set(value As String)
        '        _PersPropOfOthers_PropertyType = value
        '        Select Case _PersPropOfOthers_PropertyType
        '            Case "N/A"
        '                _PersPropOfOthers_PropertyTypeId = "0"
        '            Case "Personal Property - Stock Only"
        '                _PersPropOfOthers_PropertyTypeId = "1"
        '            Case "Personal Property - Excluding Stock"
        '                _PersPropOfOthers_PropertyTypeId = "2"
        '            Case "Machinery and Equipment"
        '                _PersPropOfOthers_PropertyTypeId = "3"
        '            Case "Furniture"
        '                _PersPropOfOthers_PropertyTypeId = "4"
        '            Case "Fixtures"
        '                _PersPropOfOthers_PropertyTypeId = "5"
        '            Case "Tenants Improvements and Betterments"
        '                _PersPropOfOthers_PropertyTypeId = "6"
        '            Case "Personal Property - Including Stock"
        '                _PersPropOfOthers_PropertyTypeId = "7"
        '            Case "Personal Property of Others"
        '                _PersPropOfOthers_PropertyTypeId = "8"
        '            Case Else
        '                _PersPropOfOthers_PropertyTypeId = ""
        '        End Select
        '    End Set
        'End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId = 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropOfOthers_RiskTypeId As String
            Get
                Return _PersPropOfOthers_RiskTypeId
            End Get
            Set(value As String)
                _PersPropOfOthers_RiskTypeId = value
                _PersPropOfOthers_RiskType = ""
                If IsNumeric(_PersPropOfOthers_RiskTypeId) = True Then
                    Select Case _PersPropOfOthers_RiskTypeId
                        Case "0"
                            _PersPropOfOthers_RiskType = "N/A"
                        Case "1"
                            _PersPropOfOthers_RiskType = "Not in Towing Business"
                        Case "2"
                            _PersPropOfOthers_RiskType = "Tow Truck Operator"
                        Case "3"
                            _PersPropOfOthers_RiskType = "Type 1 - Apartments and Condominiums - Residential Use Only"
                        Case "4"
                            _PersPropOfOthers_RiskType = "Type 2 - Offices"
                        Case "5"
                            _PersPropOfOthers_RiskType = "Type 3 - All Other Personal Property"
                        Case "6"
                            _PersPropOfOthers_RiskType = "Mercantile or Non-Manufacturing"
                        Case "7"
                            _PersPropOfOthers_RiskType = "Manufacturing"
                        Case "8"
                            _PersPropOfOthers_RiskType = "Mining"
                        Case "9"
                            _PersPropOfOthers_RiskType = "Rental Properties"
                        Case "10"
                            _PersPropOfOthers_RiskType = "Combined Manufacturing And Mercantile"
                        Case "11"
                            _PersPropOfOthers_RiskType = "Combined Manufacturing and Rental"
                        Case "12"
                            _PersPropOfOthers_RiskType = "Combined Mercantile and Rental"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId = 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropOfOthers_RiskType As String
            Get
                Return _PersPropOfOthers_RiskType
            End Get
            Set(value As String)
                _PersPropOfOthers_RiskType = value
                Select Case _PersPropOfOthers_RiskType
                    Case "N/A"
                        _PersPropOfOthers_RiskTypeId = "0"
                    Case "Not in Towing Business"
                        _PersPropOfOthers_RiskTypeId = "1"
                    Case "Tow Truck Operator"
                        _PersPropOfOthers_RiskTypeId = "2"
                    Case "Type 1 - Apartments and Condominiums - Residential Use Only"
                        _PersPropOfOthers_RiskTypeId = "3"
                    Case "Type 2 - Offices"
                        _PersPropOfOthers_RiskTypeId = "4"
                    Case "Type 3 - All Other Personal Property"
                        _PersPropOfOthers_RiskTypeId = "5"
                    Case "Mercantile or Non-Manufacturing"
                        _PersPropOfOthers_RiskTypeId = "6"
                    Case "Manufacturing"
                        _PersPropOfOthers_RiskTypeId = "7"
                    Case "Mining"
                        _PersPropOfOthers_RiskTypeId = "8"
                    Case "Rental Properties"
                        _PersPropOfOthers_RiskTypeId = "9"
                    Case "Combined Manufacturing And Mercantile"
                        _PersPropOfOthers_RiskTypeId = "10"
                    Case "Combined Manufacturing and Rental"
                        _PersPropOfOthers_RiskTypeId = "11"
                    Case "Combined Mercantile and Rental"
                        _PersPropOfOthers_RiskTypeId = "12"
                    Case Else
                        _PersPropOfOthers_RiskTypeId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21160; also sets flag on coveragecode_id 21090 (when BusinessPropertyTypeId = 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropOfOthers_EarthquakeApplies As Boolean
            Get
                Return _PersPropOfOthers_EarthquakeApplies
            End Get
            Set(value As Boolean)
                _PersPropOfOthers_EarthquakeApplies = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId = 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropOfOthers_RatingTypeId As String
            Get
                Return _PersPropOfOthers_RatingTypeId
            End Get
            Set(value As String)
                _PersPropOfOthers_RatingTypeId = value
                _PersPropOfOthers_RatingType = ""
                If IsNumeric(_PersPropOfOthers_RatingTypeId) = True Then
                    Select Case _PersPropOfOthers_RatingTypeId
                        Case "0"
                            _PersPropOfOthers_RatingType = "None"
                        Case "1"
                            _PersPropOfOthers_RatingType = "Class Rated"
                        Case "2"
                            _PersPropOfOthers_RatingType = "Specific Rated"
                        Case "3"
                            _PersPropOfOthers_RatingType = "Special Class Rate"
                        Case "4"
                            _PersPropOfOthers_RatingType = "Symbol"
                        Case "5"
                            _PersPropOfOthers_RatingType = "Specific"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId = 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropOfOthers_RatingType As String
            Get
                Return _PersPropOfOthers_RatingType
            End Get
            Set(value As String)
                _PersPropOfOthers_RatingType = value
                Select Case _PersPropOfOthers_RatingType
                    Case "None"
                        _PersPropOfOthers_RatingTypeId = "0"
                    Case "Class Rated"
                        _PersPropOfOthers_RatingTypeId = "1"
                    Case "Specific Rated"
                        _PersPropOfOthers_RatingTypeId = "2"
                    Case "Special Class Rate"
                        _PersPropOfOthers_RatingTypeId = "3"
                    Case "Symbol"
                        _PersPropOfOthers_RatingTypeId = "4"
                    Case "Specific"
                        _PersPropOfOthers_RatingTypeId = "5"
                    Case Else
                        _PersPropOfOthers_RatingTypeId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId = 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropOfOthers_CauseOfLossTypeId As String
            Get
                Return _PersPropOfOthers_CauseOfLossTypeId
            End Get
            Set(value As String)
                _PersPropOfOthers_CauseOfLossTypeId = value
                _PersPropOfOthers_CauseOfLossType = ""
                If IsNumeric(_PersPropOfOthers_CauseOfLossTypeId) = True Then
                    Select Case _PersPropOfOthers_CauseOfLossTypeId
                        Case "0"
                            _PersPropOfOthers_CauseOfLossType = "N/A"
                        Case "1"
                            _PersPropOfOthers_CauseOfLossType = "Basic Form"
                        Case "2"
                            _PersPropOfOthers_CauseOfLossType = "Broad Form"
                        Case "3"
                            _PersPropOfOthers_CauseOfLossType = "Special Form Including Theft"
                        Case "4"
                            _PersPropOfOthers_CauseOfLossType = "Special Form Excluding Theft"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId = 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropOfOthers_CauseOfLossType As String
            Get
                Return _PersPropOfOthers_CauseOfLossType
            End Get
            Set(value As String)
                _PersPropOfOthers_CauseOfLossType = value
                Select Case _PersPropOfOthers_CauseOfLossType
                    Case "N/A"
                        _PersPropOfOthers_CauseOfLossTypeId = "0"
                    Case "Basic Form"
                        _PersPropOfOthers_CauseOfLossTypeId = "1"
                    Case "Broad Form"
                        _PersPropOfOthers_CauseOfLossTypeId = "2"
                    Case "Special Form Including Theft"
                        _PersPropOfOthers_CauseOfLossTypeId = "3"
                    Case "Special Form Excluding Theft"
                        _PersPropOfOthers_CauseOfLossTypeId = "4"
                    Case Else
                        _PersPropOfOthers_CauseOfLossTypeId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId = 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropOfOthers_DeductibleId As String
            Get
                Return _PersPropOfOthers_DeductibleId
            End Get
            Set(value As String)
                _PersPropOfOthers_DeductibleId = value
                _PersPropOfOthers_Deductible = ""
                If IsNumeric(_PersPropOfOthers_DeductibleId) = True Then
                    Select Case _PersPropOfOthers_DeductibleId
                        Case "0"
                            _PersPropOfOthers_Deductible = "N/A"
                        Case "4"
                            _PersPropOfOthers_Deductible = "250"
                        Case "8"
                            _PersPropOfOthers_Deductible = "500"
                        Case "9"
                            _PersPropOfOthers_Deductible = "1,000"
                        Case "15"
                            _PersPropOfOthers_Deductible = "2,500"
                        Case "16"
                            _PersPropOfOthers_Deductible = "5,000"
                        Case "17"
                            _PersPropOfOthers_Deductible = "10,000"
                        Case "19"
                            _PersPropOfOthers_Deductible = "25,000"
                        Case "20"
                            _PersPropOfOthers_Deductible = "50,000"
                        Case "21"
                            _PersPropOfOthers_Deductible = "75,000"
                        Case "32"
                            _PersPropOfOthers_Deductible = "1%"
                        Case "33"
                            _PersPropOfOthers_Deductible = "2%"
                        Case "34"
                            _PersPropOfOthers_Deductible = "5%"
                        Case "42"
                            _PersPropOfOthers_Deductible = "Same"
                        Case "36"
                            _PersPropOfOthers_Deductible = "10%"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId = 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropOfOthers_Deductible As String
            Get
                Return _PersPropOfOthers_Deductible
            End Get
            Set(value As String)
                _PersPropOfOthers_Deductible = value
                Select Case _PersPropOfOthers_Deductible
                    Case "N/A"
                        _PersPropOfOthers_DeductibleId = "0"
                    Case "250"
                        _PersPropOfOthers_DeductibleId = "4"
                    Case "500"
                        _PersPropOfOthers_DeductibleId = "8"
                    Case "1,000"
                        _PersPropOfOthers_DeductibleId = "9"
                    Case "2,500"
                        _PersPropOfOthers_DeductibleId = "15"
                    Case "5,000"
                        _PersPropOfOthers_DeductibleId = "16"
                    Case "10,000"
                        _PersPropOfOthers_DeductibleId = "17"
                    Case "25,000"
                        _PersPropOfOthers_DeductibleId = "19"
                    Case "50,000"
                        _PersPropOfOthers_DeductibleId = "20"
                    Case "75,000"
                        _PersPropOfOthers_DeductibleId = "21"
                    Case "1%"
                        _PersPropOfOthers_DeductibleId = "32"
                    Case "2%"
                        _PersPropOfOthers_DeductibleId = "33"
                    Case "5%"
                        _PersPropOfOthers_DeductibleId = "34"
                    Case "Same"
                        _PersPropOfOthers_DeductibleId = "42"
                    Case "10%"
                        _PersPropOfOthers_DeductibleId = "36"
                    Case Else
                        _PersPropOfOthers_DeductibleId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId = 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropOfOthers_CoinsuranceTypeId As String
            Get
                Return _PersPropOfOthers_CoinsuranceTypeId
            End Get
            Set(value As String)
                _PersPropOfOthers_CoinsuranceTypeId = value
                _PersPropOfOthers_CoinsuranceType = ""
                If IsNumeric(_PersPropOfOthers_CoinsuranceTypeId) = True Then
                    Select Case _PersPropOfOthers_CoinsuranceTypeId
                        Case "0"
                            _PersPropOfOthers_CoinsuranceType = "N/A"
                        Case "1"
                            _PersPropOfOthers_CoinsuranceType = "Waived"
                        Case "2"
                            _PersPropOfOthers_CoinsuranceType = "50%"
                        Case "3"
                            _PersPropOfOthers_CoinsuranceType = "60%"
                        Case "4"
                            _PersPropOfOthers_CoinsuranceType = "70%"
                        Case "5"
                            _PersPropOfOthers_CoinsuranceType = "80%"
                        Case "6"
                            _PersPropOfOthers_CoinsuranceType = "90%"
                        Case "7"
                            _PersPropOfOthers_CoinsuranceType = "100%"
                        Case "8"
                            _PersPropOfOthers_CoinsuranceType = "10%"
                        Case "9"
                            _PersPropOfOthers_CoinsuranceType = "20%"
                        Case "10"
                            _PersPropOfOthers_CoinsuranceType = "30%"
                        Case "11"
                            _PersPropOfOthers_CoinsuranceType = "40%"
                        Case "12"
                            _PersPropOfOthers_CoinsuranceType = "125%"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId = 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropOfOthers_CoinsuranceType As String
            Get
                Return _PersPropOfOthers_CoinsuranceType
            End Get
            Set(value As String)
                _PersPropOfOthers_CoinsuranceType = value
                Select Case _PersPropOfOthers_CoinsuranceType
                    Case "N/A"
                        _PersPropOfOthers_CoinsuranceTypeId = "0"
                    Case "Waived"
                        _PersPropOfOthers_CoinsuranceTypeId = "1"
                    Case "50%"
                        _PersPropOfOthers_CoinsuranceTypeId = "2"
                    Case "60%"
                        _PersPropOfOthers_CoinsuranceTypeId = "3"
                    Case "70%"
                        _PersPropOfOthers_CoinsuranceTypeId = "4"
                    Case "80%"
                        _PersPropOfOthers_CoinsuranceTypeId = "5"
                    Case "90%"
                        _PersPropOfOthers_CoinsuranceTypeId = "6"
                    Case "100%"
                        _PersPropOfOthers_CoinsuranceTypeId = "7"
                    Case "10%"
                        _PersPropOfOthers_CoinsuranceTypeId = "8"
                    Case "20%"
                        _PersPropOfOthers_CoinsuranceTypeId = "9"
                    Case "30%"
                        _PersPropOfOthers_CoinsuranceTypeId = "10"
                    Case "40%"
                        _PersPropOfOthers_CoinsuranceTypeId = "11"
                    Case "125%"
                        _PersPropOfOthers_CoinsuranceTypeId = "12"
                    Case Else
                        _PersPropOfOthers_CoinsuranceTypeId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId = 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropOfOthers_ValuationId As String
            Get
                Return _PersPropOfOthers_ValuationId
            End Get
            Set(value As String)
                _PersPropOfOthers_ValuationId = value
                '(1=Replacement Cost; 2=Actual Cash Value; 3=Functional Building Valuation)
                _PersPropOfOthers_Valuation = ""
                If IsNumeric(_PersPropOfOthers_ValuationId) = True Then
                    Select Case _PersPropOfOthers_ValuationId
                        Case "-1" 'added 10/19/2012 for CPR to match specs
                            _PersPropOfOthers_Valuation = "N/A"
                        Case "1"
                            _PersPropOfOthers_Valuation = "Replacement Cost"
                        Case "2"
                            _PersPropOfOthers_Valuation = "Actual Cash Value"
                        Case "3"
                            _PersPropOfOthers_Valuation = "Functional Building Valuation"
                        Case "7" 'added 10/18/2012 for CPR
                            _PersPropOfOthers_Valuation = "Functional Replacement Cost"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId = 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropOfOthers_Valuation As String
            Get
                Return _PersPropOfOthers_Valuation
            End Get
            Set(value As String)
                _PersPropOfOthers_Valuation = value
                Select Case _PersPropOfOthers_Valuation
                    Case "N/A" 'added 10/19/2012 for CPR to match specs
                        _PersPropOfOthers_ValuationId = "-1"
                    Case "Replacement Cost"
                        _PersPropOfOthers_ValuationId = "1"
                    Case "Actual Cash Value"
                        _PersPropOfOthers_ValuationId = "2"
                    Case "Functional Building Valuation"
                        _PersPropOfOthers_ValuationId = "3"
                    Case "Functional Replacement Cost" 'added 10/18/2012 for CPR
                        _PersPropOfOthers_ValuationId = "7"
                    Case Else
                        _PersPropOfOthers_ValuationId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId = 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropOfOthers_QuotedPremium As String
            Get
                'Return _PersPropOfOthers_QuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_PersPropOfOthers_QuotedPremium)
            End Get
            Set(value As String)
                _PersPropOfOthers_QuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_PersPropOfOthers_QuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId = 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropOfOthers_ClassificationCode As QuickQuoteClassificationCode
            Get
                SetObjectsParent(_PersPropOfOthers_ClassificationCode)
                Return _PersPropOfOthers_ClassificationCode
            End Get
            Set(value As QuickQuoteClassificationCode)
                _PersPropOfOthers_ClassificationCode = value
                SetObjectsParent(_PersPropOfOthers_ClassificationCode)
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21095</remarks>
        Public Property BusinessIncomeCov_Limit As String
            Get
                Return _BusinessIncomeCov_Limit
            End Get
            Set(value As String)
                _BusinessIncomeCov_Limit = value
                qqHelper.ConvertToLimitFormat(_BusinessIncomeCov_Limit)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21095</remarks>
        Public Property BusinessIncomeCov_CoinsuranceTypeId As String
            Get
                Return _BusinessIncomeCov_CoinsuranceTypeId
            End Get
            Set(value As String)
                _BusinessIncomeCov_CoinsuranceTypeId = value
                _BusinessIncomeCov_CoinsuranceType = ""
                If IsNumeric(_BusinessIncomeCov_CoinsuranceTypeId) = True Then
                    Select Case _BusinessIncomeCov_CoinsuranceTypeId
                        Case "0"
                            _BusinessIncomeCov_CoinsuranceType = "N/A"
                        Case "1"
                            _BusinessIncomeCov_CoinsuranceType = "Waived"
                        Case "2"
                            _BusinessIncomeCov_CoinsuranceType = "50%"
                        Case "3"
                            _BusinessIncomeCov_CoinsuranceType = "60%"
                        Case "4"
                            _BusinessIncomeCov_CoinsuranceType = "70%"
                        Case "5"
                            _BusinessIncomeCov_CoinsuranceType = "80%"
                        Case "6"
                            _BusinessIncomeCov_CoinsuranceType = "90%"
                        Case "7"
                            _BusinessIncomeCov_CoinsuranceType = "100%"
                        Case "8"
                            _BusinessIncomeCov_CoinsuranceType = "10%"
                        Case "9"
                            _BusinessIncomeCov_CoinsuranceType = "20%"
                        Case "10"
                            _BusinessIncomeCov_CoinsuranceType = "30%"
                        Case "11"
                            _BusinessIncomeCov_CoinsuranceType = "40%"
                        Case "12"
                            _BusinessIncomeCov_CoinsuranceType = "125%"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21095</remarks>
        Public Property BusinessIncomeCov_CoinsuranceType As String
            Get
                Return _BusinessIncomeCov_CoinsuranceType
            End Get
            Set(value As String)
                _BusinessIncomeCov_CoinsuranceType = value
                Select Case _BusinessIncomeCov_CoinsuranceType
                    Case "N/A"
                        _BusinessIncomeCov_CoinsuranceTypeId = "0"
                    Case "Waived"
                        _BusinessIncomeCov_CoinsuranceTypeId = "1"
                    Case "50%"
                        _BusinessIncomeCov_CoinsuranceTypeId = "2"
                    Case "60%"
                        _BusinessIncomeCov_CoinsuranceTypeId = "3"
                    Case "70%"
                        _BusinessIncomeCov_CoinsuranceTypeId = "4"
                    Case "80%"
                        _BusinessIncomeCov_CoinsuranceTypeId = "5"
                    Case "90%"
                        _BusinessIncomeCov_CoinsuranceTypeId = "6"
                    Case "100%"
                        _BusinessIncomeCov_CoinsuranceTypeId = "7"
                    Case "10%"
                        _BusinessIncomeCov_CoinsuranceTypeId = "8"
                    Case "20%"
                        _BusinessIncomeCov_CoinsuranceTypeId = "9"
                    Case "30%"
                        _BusinessIncomeCov_CoinsuranceTypeId = "10"
                    Case "40%"
                        _BusinessIncomeCov_CoinsuranceTypeId = "11"
                    Case "125%"
                        _BusinessIncomeCov_CoinsuranceTypeId = "12"
                    Case Else
                        _BusinessIncomeCov_CoinsuranceTypeId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21095</remarks>
        Public Property BusinessIncomeCov_MonthlyPeriodTypeId As String
            Get
                Return _BusinessIncomeCov_MonthlyPeriodTypeId
            End Get
            Set(value As String)
                _BusinessIncomeCov_MonthlyPeriodTypeId = value
                _BusinessIncomeCov_MonthlyPeriodType = ""
                If IsNumeric(_BusinessIncomeCov_MonthlyPeriodTypeId) = True Then
                    Select Case _BusinessIncomeCov_MonthlyPeriodTypeId
                        Case "0"
                            _BusinessIncomeCov_MonthlyPeriodType = "N/A"
                        Case "1"
                            _BusinessIncomeCov_MonthlyPeriodType = "1/3"
                        Case "2"
                            _BusinessIncomeCov_MonthlyPeriodType = "1/4"
                        Case "3"
                            _BusinessIncomeCov_MonthlyPeriodType = "1/6"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21095</remarks>
        Public Property BusinessIncomeCov_MonthlyPeriodType As String
            Get
                Return _BusinessIncomeCov_MonthlyPeriodType
            End Get
            Set(value As String)
                _BusinessIncomeCov_MonthlyPeriodType = value
                Select Case _BusinessIncomeCov_MonthlyPeriodType
                    Case "N/A"
                        _BusinessIncomeCov_MonthlyPeriodTypeId = "0"
                    Case "1/3"
                        _BusinessIncomeCov_MonthlyPeriodTypeId = "1"
                    Case "1/4"
                        _BusinessIncomeCov_MonthlyPeriodTypeId = "2"
                    Case "1/6"
                        _BusinessIncomeCov_MonthlyPeriodTypeId = "3"
                    Case Else
                        _BusinessIncomeCov_MonthlyPeriodTypeId = "4"
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21095</remarks>
        Public Property BusinessIncomeCov_BusinessIncomeTypeId As String
            Get
                Return _BusinessIncomeCov_BusinessIncomeTypeId
            End Get
            Set(value As String)
                _BusinessIncomeCov_BusinessIncomeTypeId = value
                _BusinessIncomeCov_BusinessIncomeType = ""
                If IsNumeric(_BusinessIncomeCov_BusinessIncomeTypeId) = True Then
                    Select Case _BusinessIncomeCov_BusinessIncomeTypeId
                        Case "0"
                            _BusinessIncomeCov_BusinessIncomeType = "N/A"
                        Case "1"
                            _BusinessIncomeCov_BusinessIncomeType = "Business Income  Including Rental Value With Extra Expense"
                        Case "2"
                            _BusinessIncomeCov_BusinessIncomeType = "Business Income  Including Rental Value Without Extra Expense"
                        Case "3"
                            _BusinessIncomeCov_BusinessIncomeType = "Business Income Other than Rental Value With Extra Expense"
                        Case "4"
                            _BusinessIncomeCov_BusinessIncomeType = "Business Income Other than Rental Value Without Extra Expense"
                        Case "5"
                            _BusinessIncomeCov_BusinessIncomeType = "Rental Value With Extra Expense"
                        Case "6"
                            _BusinessIncomeCov_BusinessIncomeType = "Rental Value Without Extra Expense"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21095</remarks>
        Public Property BusinessIncomeCov_BusinessIncomeType As String
            Get
                Return _BusinessIncomeCov_BusinessIncomeType
            End Get
            Set(value As String)
                _BusinessIncomeCov_BusinessIncomeType = value
                Select Case _BusinessIncomeCov_BusinessIncomeType
                    Case "N/A"
                        _BusinessIncomeCov_BusinessIncomeTypeId = "0"
                    Case "Business Income  Including Rental Value With Extra Expense"
                        _BusinessIncomeCov_BusinessIncomeTypeId = "1"
                    Case "Business Income  Including Rental Value Without Extra Expense"
                        _BusinessIncomeCov_BusinessIncomeTypeId = "2"
                    Case "Business Income Other than Rental Value With Extra Expense"
                        _BusinessIncomeCov_BusinessIncomeTypeId = "3"
                    Case "Business Income Other than Rental Value Without Extra Expense"
                        _BusinessIncomeCov_BusinessIncomeTypeId = "4"
                    Case "Rental Value With Extra Expense"
                        _BusinessIncomeCov_BusinessIncomeTypeId = "5"
                    Case "Rental Value Without Extra Expense"
                        _BusinessIncomeCov_BusinessIncomeTypeId = "6"
                    Case Else
                        _BusinessIncomeCov_BusinessIncomeTypeId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21095</remarks>
        Public Property BusinessIncomeCov_RiskTypeId As String
            Get
                Return _BusinessIncomeCov_RiskTypeId
            End Get
            Set(value As String)
                _BusinessIncomeCov_RiskTypeId = value
                _BusinessIncomeCov_RiskType = ""
                If IsNumeric(_BusinessIncomeCov_RiskTypeId) = True Then
                    Select Case _BusinessIncomeCov_RiskTypeId
                        Case "0"
                            _BusinessIncomeCov_RiskType = "N/A"
                        Case "1"
                            _BusinessIncomeCov_RiskType = "Not in Towing Business"
                        Case "2"
                            _BusinessIncomeCov_RiskType = "Tow Truck Operator"
                        Case "3"
                            _BusinessIncomeCov_RiskType = "Type 1 - Apartments and Condominiums - Residential Use Only"
                        Case "4"
                            _BusinessIncomeCov_RiskType = "Type 2 - Offices"
                        Case "5"
                            _BusinessIncomeCov_RiskType = "Type 3 - All Other Personal Property"
                        Case "6"
                            _BusinessIncomeCov_RiskType = "Mercantile or Non-Manufacturing"
                        Case "7"
                            _BusinessIncomeCov_RiskType = "Manufacturing"
                        Case "8"
                            _BusinessIncomeCov_RiskType = "Mining"
                        Case "9"
                            _BusinessIncomeCov_RiskType = "Rental Properties"
                        Case "10"
                            _BusinessIncomeCov_RiskType = "Combined Manufacturing And Mercantile"
                        Case "11"
                            _BusinessIncomeCov_RiskType = "Combined Manufacturing and Rental"
                        Case "12"
                            _BusinessIncomeCov_RiskType = "Combined Mercantile and Rental"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21095</remarks>
        Public Property BusinessIncomeCov_RiskType As String
            Get
                Return _BusinessIncomeCov_RiskType
            End Get
            Set(value As String)
                _BusinessIncomeCov_RiskType = value
                Select Case _BusinessIncomeCov_RiskType
                    Case "N/A"
                        _BusinessIncomeCov_RiskTypeId = "0"
                    Case "Not in Towing Business"
                        _BusinessIncomeCov_RiskTypeId = "1"
                    Case "Tow Truck Operator"
                        _BusinessIncomeCov_RiskTypeId = "2"
                    Case "Type 1 - Apartments and Condominiums - Residential Use Only"
                        _BusinessIncomeCov_RiskTypeId = "3"
                    Case "Type 2 - Offices"
                        _BusinessIncomeCov_RiskTypeId = "4"
                    Case "Type 3 - All Other Personal Property"
                        _BusinessIncomeCov_RiskTypeId = "5"
                    Case "Mercantile or Non-Manufacturing"
                        _BusinessIncomeCov_RiskTypeId = "6"
                    Case "Manufacturing"
                        _BusinessIncomeCov_RiskTypeId = "7"
                    Case "Mining"
                        _BusinessIncomeCov_RiskTypeId = "8"
                    Case "Rental Properties"
                        _BusinessIncomeCov_RiskTypeId = "9"
                    Case "Combined Manufacturing And Mercantile"
                        _BusinessIncomeCov_RiskTypeId = "10"
                    Case "Combined Manufacturing and Rental"
                        _BusinessIncomeCov_RiskTypeId = "11"
                    Case "Combined Mercantile and Rental"
                        _BusinessIncomeCov_RiskTypeId = "12"
                    Case Else
                        _BusinessIncomeCov_RiskTypeId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21163; also sets flag on coveragecode_id = 21095</remarks>
        Public Property BusinessIncomeCov_EarthquakeApplies As Boolean
            Get
                Return _BusinessIncomeCov_EarthquakeApplies
            End Get
            Set(value As Boolean)
                _BusinessIncomeCov_EarthquakeApplies = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21095</remarks>
        Public Property BusinessIncomeCov_RatingTypeId As String
            Get
                Return _BusinessIncomeCov_RatingTypeId
            End Get
            Set(value As String)
                _BusinessIncomeCov_RatingTypeId = value
                _BusinessIncomeCov_RatingType = ""
                If IsNumeric(_BusinessIncomeCov_RatingTypeId) = True Then
                    Select Case _BusinessIncomeCov_RatingTypeId
                        Case "0"
                            _BusinessIncomeCov_RatingType = "None"
                        Case "1"
                            _BusinessIncomeCov_RatingType = "Class Rated"
                        Case "2"
                            _BusinessIncomeCov_RatingType = "Specific Rated"
                        Case "3"
                            _BusinessIncomeCov_RatingType = "Special Class Rate"
                        Case "4"
                            _BusinessIncomeCov_RatingType = "Symbol"
                        Case "5"
                            _BusinessIncomeCov_RatingType = "Specific"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21095</remarks>
        Public Property BusinessIncomeCov_RatingType As String
            Get
                Return _BusinessIncomeCov_RatingType
            End Get
            Set(value As String)
                _BusinessIncomeCov_RatingType = value
                Select Case _BusinessIncomeCov_RatingType
                    Case "None"
                        _BusinessIncomeCov_RatingTypeId = "0"
                    Case "Class Rated"
                        _BusinessIncomeCov_RatingTypeId = "1"
                    Case "Specific Rated"
                        _BusinessIncomeCov_RatingTypeId = "2"
                    Case "Special Class Rate"
                        _BusinessIncomeCov_RatingTypeId = "3"
                    Case "Symbol"
                        _BusinessIncomeCov_RatingTypeId = "4"
                    Case "Specific"
                        _BusinessIncomeCov_RatingTypeId = "5"
                    Case Else
                        _BusinessIncomeCov_RatingTypeId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21095</remarks>
        Public Property BusinessIncomeCov_CauseOfLossTypeId As String
            Get
                Return _BusinessIncomeCov_CauseOfLossTypeId
            End Get
            Set(value As String)
                _BusinessIncomeCov_CauseOfLossTypeId = value
                _BusinessIncomeCov_CauseOfLossType = ""
                If IsNumeric(_BusinessIncomeCov_CauseOfLossTypeId) = True Then
                    Select Case _BusinessIncomeCov_CauseOfLossTypeId
                        Case "0"
                            _BusinessIncomeCov_CauseOfLossType = "N/A"
                        Case "1"
                            _BusinessIncomeCov_CauseOfLossType = "Basic Form"
                        Case "2"
                            _BusinessIncomeCov_CauseOfLossType = "Broad Form"
                        Case "3"
                            _BusinessIncomeCov_CauseOfLossType = "Special Form Including Theft"
                        Case "4"
                            _BusinessIncomeCov_CauseOfLossType = "Special Form Excluding Theft"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21095</remarks>
        Public Property BusinessIncomeCov_CauseOfLossType As String
            Get
                Return _BusinessIncomeCov_CauseOfLossType
            End Get
            Set(value As String)
                _BusinessIncomeCov_CauseOfLossType = value
                Select Case _BusinessIncomeCov_CauseOfLossType
                    Case "N/A"
                        _BusinessIncomeCov_CauseOfLossTypeId = "0"
                    Case "Basic Form"
                        _BusinessIncomeCov_CauseOfLossTypeId = "1"
                    Case "Broad Form"
                        _BusinessIncomeCov_CauseOfLossTypeId = "2"
                    Case "Special Form Including Theft"
                        _BusinessIncomeCov_CauseOfLossTypeId = "3"
                    Case "Special Form Excluding Theft"
                        _BusinessIncomeCov_CauseOfLossTypeId = "4"
                    Case Else
                        _BusinessIncomeCov_CauseOfLossTypeId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21095</remarks>
        Public Property BusinessIncomeCov_QuotedPremium As String
            Get
                'Return _BusinessIncomeCov_QuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_BusinessIncomeCov_QuotedPremium)
            End Get
            Set(value As String)
                _BusinessIncomeCov_QuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_BusinessIncomeCov_QuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21095</remarks>
        Public Property BusinessIncomeCov_ClassificationCode As QuickQuoteClassificationCode
            Get
                SetObjectsParent(_BusinessIncomeCov_ClassificationCode)
                Return _BusinessIncomeCov_ClassificationCode
            End Get
            Set(value As QuickQuoteClassificationCode)
                _BusinessIncomeCov_ClassificationCode = value
                SetObjectsParent(_BusinessIncomeCov_ClassificationCode)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21095</remarks>
        Public Property BusinessIncomeCov_WaitingPeriodTypeId As String 'added 10/10/2012 for CPR building Business Income Cov; only N/A, No Waiting, 24, and 72 are shown in Diamond UI
            Get
                Return _BusinessIncomeCov_WaitingPeriodTypeId
            End Get
            Set(value As String)
                _BusinessIncomeCov_WaitingPeriodTypeId = value
                _BusinessIncomeCov_WaitingPeriodType = ""
                If IsNumeric(_BusinessIncomeCov_WaitingPeriodTypeId) = True Then
                    Select Case _BusinessIncomeCov_WaitingPeriodTypeId
                        Case "0"
                            _BusinessIncomeCov_WaitingPeriodType = "N/A"
                        Case "1"
                            _BusinessIncomeCov_WaitingPeriodType = "No Waiting"
                        Case "2"
                            _BusinessIncomeCov_WaitingPeriodType = "24"
                        Case "3"
                            _BusinessIncomeCov_WaitingPeriodType = "72"
                        Case "4"
                            _BusinessIncomeCov_WaitingPeriodType = "0"
                        Case "5"
                            _BusinessIncomeCov_WaitingPeriodType = "1"
                        Case "6"
                            _BusinessIncomeCov_WaitingPeriodType = "2"
                        Case "7"
                            _BusinessIncomeCov_WaitingPeriodType = "3"
                        Case "8"
                            _BusinessIncomeCov_WaitingPeriodType = "72 hours"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21095</remarks>
        Public Property BusinessIncomeCov_WaitingPeriodType As String
            Get
                Return _BusinessIncomeCov_WaitingPeriodType
            End Get
            Set(value As String)
                _BusinessIncomeCov_WaitingPeriodType = value
                Select Case _BusinessIncomeCov_WaitingPeriodType
                    Case "N/A"
                        _BusinessIncomeCov_WaitingPeriodTypeId = "0"
                    Case "No Waiting"
                        _BusinessIncomeCov_WaitingPeriodTypeId = "1"
                    Case "24"
                        _BusinessIncomeCov_WaitingPeriodTypeId = "2"
                    Case "72"
                        _BusinessIncomeCov_WaitingPeriodTypeId = "3"
                    Case "0"
                        _BusinessIncomeCov_WaitingPeriodTypeId = "4"
                    Case "1"
                        _BusinessIncomeCov_WaitingPeriodTypeId = "5"
                    Case "2"
                        _BusinessIncomeCov_WaitingPeriodTypeId = "6"
                    Case "3"
                        _BusinessIncomeCov_WaitingPeriodTypeId = "7"
                    Case "72 hours"
                        _BusinessIncomeCov_WaitingPeriodTypeId = "8"
                    Case Else
                        _BusinessIncomeCov_WaitingPeriodTypeId = ""
                End Select
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21175</remarks>
        Public Property Building_BusinessIncome_Group1_Rate As String
            Get
                Return _Building_BusinessIncome_Group1_Rate
            End Get
            Set(value As String)
                _Building_BusinessIncome_Group1_Rate = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21180</remarks>
        Public Property Building_BusinessIncome_Group2_Rate As String
            Get
                Return _Building_BusinessIncome_Group2_Rate
            End Get
            Set(value As String)
                _Building_BusinessIncome_Group2_Rate = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21176 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others] for coverage w/ coveragecode_id = 21090); found inside ScheduledCoverage</remarks>
        Public Property PersonalProperty_Group1_Rate As String
            Get
                Return _PersonalProperty_Group1_Rate
            End Get
            Set(value As String)
                _PersonalProperty_Group1_Rate = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21181 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others] for coverage w/ coveragecode_id = 21090); found inside ScheduledCoverage</remarks>
        Public Property PersonalProperty_Group2_Rate As String
            Get
                Return _PersonalProperty_Group2_Rate
            End Get
            Set(value As String)
                _PersonalProperty_Group2_Rate = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21175</remarks>
        Public Property Building_BusinessIncome_Group1_LossCost As String
            Get
                Return _Building_BusinessIncome_Group1_LossCost
            End Get
            Set(value As String)
                _Building_BusinessIncome_Group1_LossCost = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21180</remarks>
        Public Property Building_BusinessIncome_Group2_LossCost As String
            Get
                Return _Building_BusinessIncome_Group2_LossCost
            End Get
            Set(value As String)
                _Building_BusinessIncome_Group2_LossCost = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21176 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others] for coverage w/ coveragecode_id = 21090); found inside ScheduledCoverage</remarks>
        Public Property PersonalProperty_Group1_LossCost As String
            Get
                Return _PersonalProperty_Group1_LossCost
            End Get
            Set(value As String)
                _PersonalProperty_Group1_LossCost = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21181 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others] for coverage w/ coveragecode_id = 21090); found inside ScheduledCoverage</remarks>
        Public Property PersonalProperty_Group2_LossCost As String
            Get
                Return _PersonalProperty_Group2_LossCost
            End Get
            Set(value As String)
                _PersonalProperty_Group2_LossCost = value
            End Set
        End Property

        Public Property CoverageFormTypeId As String
            Get
                Return _CoverageFormTypeId
            End Get
            Set(value As String)
                _CoverageFormTypeId = value
                _CoverageFormType = ""
                If IsNumeric(_CoverageFormTypeId) = True Then
                    Select Case _CoverageFormTypeId
                        Case "0"
                            _CoverageFormType = "N/A"
                        Case "1"
                            _CoverageFormType = "CP 00 10 Building and Personal Property Coverage Form"
                        Case "2"
                            _CoverageFormType = "CP 00 17 Condominium Association Coverage Form"
                        Case "3"
                            _CoverageFormType = "CP 00 18 Condominium Commercial Unit-Owners Coverage Form"
                        Case "4"
                            _CoverageFormType = "CP 00 20 Builders Risk Coverage Form"
                        Case "5"
                            _CoverageFormType = "CP 00 40 Legal Liability Coverage Form"
                    End Select
                End If
            End Set
        End Property
        Public Property CoverageFormType As String
            Get
                Return _CoverageFormType
            End Get
            Set(value As String)
                _CoverageFormType = value
                Select Case _CoverageFormType
                    Case "N/A"
                        _CoverageFormTypeId = "0"
                    Case "CP 00 10 Building and Personal Property Coverage Form"
                        _CoverageFormTypeId = "1"
                    Case "CP 00 17 Condominium Association Coverage Form"
                        _CoverageFormTypeId = "2"
                    Case "CP 00 18 Condominium Commercial Unit-Owners Coverage Form"
                        _CoverageFormTypeId = "3"
                    Case "CP 00 20 Builders Risk Coverage Form"
                        _CoverageFormTypeId = "4"
                    Case "CP 00 40 Legal Liability Coverage Form"
                        _CoverageFormTypeId = "5"
                    Case Else
                        _CoverageFormTypeId = ""
                End Select
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks><see cref="LimitQuotedPremium"/> + <see cref="PersPropCov_QuotedPremium"/> + <see cref="PersPropOfOthers_QuotedPremium"/> + <see cref="BusinessIncomeCov_QuotedPremium"/></remarks>
        Public Property CPR_Covs_TotalBuildingPremium As String
            Get
                'Return _CPR_Covs_TotalBuildingPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_CPR_Covs_TotalBuildingPremium)
            End Get
            Set(value As String)
                _CPR_Covs_TotalBuildingPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_CPR_Covs_TotalBuildingPremium)
            End Set
        End Property

        'Public Property PersPropCov_EarthquakeRateGradeTypeId As String
        '    Get
        '        Return _PersPropCov_EarthquakeRateGradeTypeId
        '    End Get
        '    Set(value As String)
        '        _PersPropCov_EarthquakeRateGradeTypeId = value
        '    End Set
        'End Property
        'Public Property PersPropOfOthers_EarthquakeRateGradeTypeId As String
        '    Get
        '        Return _PersPropOfOthers_EarthquakeRateGradeTypeId
        '    End Get
        '    Set(value As String)
        '        _PersPropOfOthers_EarthquakeRateGradeTypeId = value
        '    End Set
        'End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21160; found inside ScheduledCoverage</remarks>
        Public Property PersonalProperty_EarthquakeRateGradeTypeId As String
            Get
                Return _PersonalProperty_EarthquakeRateGradeTypeId
            End Get
            Set(value As String)
                _PersonalProperty_EarthquakeRateGradeTypeId = value
            End Set
        End Property

        Public Property NumberOfStories As String
            Get
                Return _NumberOfStories
            End Get
            Set(value As String)
                _NumberOfStories = value
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21155</remarks>
        Public Property EarthquakeQuotedPremium As String
            Get
                'Return _EarthquakeQuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_EarthquakeQuotedPremium)
            End Get
            Set(value As String)
                _EarthquakeQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_EarthquakeQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21160 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others] for coverage w/ coveragecode_id = 21090); found inside ScheduledCoverage</remarks>
        Public Property PersPropCov_EarthquakeQuotedPremium As String
            Get
                'Return _PersPropCov_EarthquakeQuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_PersPropCov_EarthquakeQuotedPremium)
            End Get
            Set(value As String)
                _PersPropCov_EarthquakeQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_PersPropCov_EarthquakeQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21160 (when BusinessPropertyTypeId = 8 [Personal Property of Others] for coverage w/ coveragecode_id = 21090); found inside ScheduledCoverage</remarks>
        Public Property PersPropOfOthers_EarthquakeQuotedPremium As String
            Get
                'Return _PersPropOfOthers_EarthquakeQuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_PersPropOfOthers_EarthquakeQuotedPremium)
            End Get
            Set(value As String)
                _PersPropOfOthers_EarthquakeQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_PersPropOfOthers_EarthquakeQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21163</remarks>
        Public Property BusinessIncomeCov_EarthquakeQuotedPremium As String
            Get
                'Return _BusinessIncomeCov_EarthquakeQuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_BusinessIncomeCov_EarthquakeQuotedPremium)
            End Get
            Set(value As String)
                _BusinessIncomeCov_EarthquakeQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_BusinessIncomeCov_EarthquakeQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks><see cref="EarthquakeQuotedPremium"/> + <see cref="PersPropCov_EarthquakeQuotedPremium"/> + <see cref="PersPropOfOthers_EarthquakeQuotedPremium"/> + <see cref="BusinessIncomeCov_EarthquakeQuotedPremium"/></remarks>
        Public Property CPR_Covs_TotalBuilding_EQ_Premium As String
            Get
                'Return _CPR_Covs_TotalBuilding_EQ_Premium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_CPR_Covs_TotalBuilding_EQ_Premium)
            End Get
            Set(value As String)
                _CPR_Covs_TotalBuilding_EQ_Premium = value
                qqHelper.ConvertToQuotedPremiumFormat(_CPR_Covs_TotalBuilding_EQ_Premium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks><see cref="CPR_Covs_TotalBuildingPremium"/> + <see cref="CPR_Covs_TotalBuilding_EQ_Premium"/></remarks>
        Public Property CPR_Covs_TotalBuilding_With_EQ_Premium As String
            Get
                'Return _CPR_Covs_TotalBuilding_With_EQ_Premium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_CPR_Covs_TotalBuilding_With_EQ_Premium)
            End Get
            Set(value As String)
                _CPR_Covs_TotalBuilding_With_EQ_Premium = value
                qqHelper.ConvertToQuotedPremiumFormat(_CPR_Covs_TotalBuilding_With_EQ_Premium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks><see cref="LimitQuotedPremium"/> + <see cref="EarthquakeQuotedPremium"/></remarks>
        Public Property CPR_BuildingLimit_With_EQ_QuotedPremium As String
            Get
                'Return _CPR_BuildingLimit_With_EQ_QuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_CPR_BuildingLimit_With_EQ_QuotedPremium)
            End Get
            Set(value As String)
                _CPR_BuildingLimit_With_EQ_QuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_CPR_BuildingLimit_With_EQ_QuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks><see cref="PersPropCov_QuotedPremium"/> + <see cref="PersPropCov_EarthquakeQuotedPremium"/></remarks>
        Public Property CPR_PersPropCov_With_EQ_QuotedPremium As String
            Get
                'Return _CPR_PersPropCov_With_EQ_QuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_CPR_PersPropCov_With_EQ_QuotedPremium)
            End Get
            Set(value As String)
                _CPR_PersPropCov_With_EQ_QuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_CPR_PersPropCov_With_EQ_QuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks><see cref="PersPropOfOthers_QuotedPremium"/> + <see cref="PersPropOfOthers_EarthquakeQuotedPremium"/></remarks>
        Public Property CPR_PersPropOfOthers_With_EQ_QuotedPremium As String
            Get
                'Return _CPR_PersPropOfOthers_With_EQ_QuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_CPR_PersPropOfOthers_With_EQ_QuotedPremium)
            End Get
            Set(value As String)
                _CPR_PersPropOfOthers_With_EQ_QuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_CPR_PersPropOfOthers_With_EQ_QuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks><see cref="BusinessIncomeCov_QuotedPremium"/> + <see cref="BusinessIncomeCov_EarthquakeQuotedPremium"/></remarks>
        Public Property CPR_BusinessIncomeCov_With_EQ_QuotedPremium As String
            Get
                'Return _CPR_BusinessIncomeCov_With_EQ_QuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_CPR_BusinessIncomeCov_With_EQ_QuotedPremium)
            End Get
            Set(value As String)
                _CPR_BusinessIncomeCov_With_EQ_QuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_CPR_BusinessIncomeCov_With_EQ_QuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21155; value for CoverageAdditionalInfoRecord w/ description = 'EarthquakeBuildingClassificationPercentage'</remarks>
        Public Property CPR_BuildingLimit_EarthquakeBuildingClassificationPercentage As String
            Get
                Return _CPR_BuildingLimit_EarthquakeBuildingClassificationPercentage
            End Get
            Set(value As String)
                _CPR_BuildingLimit_EarthquakeBuildingClassificationPercentage = value
                'qqHelper.ConvertToQuotedPremiumFormat(_CPR_BuildingLimit_EarthquakeBuildingClassificationPercentage)'needs to be % instead
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21160 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others] for coverage w/ coveragecode_id = 21090); value for CoverageAdditionalInfoRecord w/ description = 'EarthquakeBuildingClassificationPercentage'; found inside ScheduledCoverage</remarks>
        Public Property CPR_PersPropCov_EarthquakeBuildingClassificationPercentage As String
            Get
                Return _CPR_PersPropCov_EarthquakeBuildingClassificationPercentage
            End Get
            Set(value As String)
                _CPR_PersPropCov_EarthquakeBuildingClassificationPercentage = value
                'qqHelper.ConvertToQuotedPremiumFormat(_CPR_PersPropCov_EarthquakeBuildingClassificationPercentage)'needs to be % instead
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21160 (when BusinessPropertyTypeId = 8 [Personal Property of Others] for coverage w/ coveragecode_id = 21090); value for CoverageAdditionalInfoRecord w/ description = 'EarthquakeBuildingClassificationPercentage'; found inside ScheduledCoverage</remarks>
        Public Property CPR_PersPropOfOthers_EarthquakeBuildingClassificationPercentage As String
            Get
                Return _CPR_PersPropOfOthers_EarthquakeBuildingClassificationPercentage
            End Get
            Set(value As String)
                _CPR_PersPropOfOthers_EarthquakeBuildingClassificationPercentage = value
                'qqHelper.ConvertToQuotedPremiumFormat(_CPR_PersPropOfOthers_EarthquakeBuildingClassificationPercentage)'needs to be % instead
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21163; value for CoverageAdditionalInfoRecord w/ description = 'EarthquakeBuildingClassificationPercentage'</remarks>
        Public Property CPR_BusinessIncomeCov_EarthquakeBuildingClassificationPercentage As String
            Get
                Return _CPR_BusinessIncomeCov_EarthquakeBuildingClassificationPercentage
            End Get
            Set(value As String)
                _CPR_BusinessIncomeCov_EarthquakeBuildingClassificationPercentage = value
                'qqHelper.ConvertToQuotedPremiumFormat(_CPR_BusinessIncomeCov_EarthquakeBuildingClassificationPercentage)'needs to be % instead
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21167</remarks>
        Public Property OptionalTheftDeductibleId As String 'UI doesn't have the % values
            Get
                Return _OptionalTheftDeductibleId
            End Get
            Set(value As String)
                _OptionalTheftDeductibleId = value
                _OptionalTheftDeductible = ""
                If IsNumeric(_OptionalTheftDeductibleId) = True Then
                    Select Case _OptionalTheftDeductibleId
                        Case "0"
                            _OptionalTheftDeductible = "N/A"
                        Case "4"
                            _OptionalTheftDeductible = "250"
                        Case "8"
                            _OptionalTheftDeductible = "500"
                        Case "9"
                            _OptionalTheftDeductible = "1,000"
                        Case "15"
                            _OptionalTheftDeductible = "2,500"
                        Case "16"
                            _OptionalTheftDeductible = "5,000"
                        Case "17"
                            _OptionalTheftDeductible = "10,000"
                        Case "19"
                            _OptionalTheftDeductible = "25,000"
                        Case "20"
                            _OptionalTheftDeductible = "50,000"
                        Case "21"
                            _OptionalTheftDeductible = "75,000"
                        Case "32"
                            _OptionalTheftDeductible = "1%"
                        Case "33"
                            _OptionalTheftDeductible = "2%"
                        Case "34"
                            _OptionalTheftDeductible = "5%"
                        Case "42"
                            _OptionalTheftDeductible = "Same"
                        Case "36"
                            _OptionalTheftDeductible = "10%"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21167</remarks>
        Public Property OptionalTheftDeductible As String
            Get
                Return _OptionalTheftDeductible
            End Get
            Set(value As String)
                _OptionalTheftDeductible = value
                Select Case _OptionalTheftDeductible
                    Case "N/A"
                        _OptionalTheftDeductibleId = "0"
                    Case "250"
                        _OptionalTheftDeductibleId = "4"
                    Case "500"
                        _OptionalTheftDeductibleId = "8"
                    Case "1,000"
                        _OptionalTheftDeductibleId = "9"
                    Case "2,500"
                        _OptionalTheftDeductibleId = "15"
                    Case "5,000"
                        _OptionalTheftDeductibleId = "16"
                    Case "10,000"
                        _OptionalTheftDeductibleId = "17"
                    Case "25,000"
                        _OptionalTheftDeductibleId = "19"
                    Case "50,000"
                        _OptionalTheftDeductibleId = "20"
                    Case "75,000"
                        _OptionalTheftDeductibleId = "21"
                    Case "1%"
                        _OptionalTheftDeductibleId = "32"
                    Case "2%"
                        _OptionalTheftDeductibleId = "33"
                    Case "5%"
                        _OptionalTheftDeductibleId = "34"
                    Case "Same"
                        _OptionalTheftDeductibleId = "42"
                    Case "10%"
                        _OptionalTheftDeductibleId = "36"
                    Case Else
                        _OptionalTheftDeductibleId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21170</remarks>
        Public Property OptionalWindstormOrHailDeductibleId As String
            Get
                Return _OptionalWindstormOrHailDeductibleId
            End Get
            Set(value As String)
                _OptionalWindstormOrHailDeductibleId = value
                _OptionalWindstormOrHailDeductible = ""
                If IsNumeric(_OptionalWindstormOrHailDeductibleId) = True Then
                    Select Case _OptionalWindstormOrHailDeductibleId
                        Case "0"
                            _OptionalWindstormOrHailDeductible = "N/A"
                        Case "4"
                            _OptionalWindstormOrHailDeductible = "250"
                        Case "8"
                            _OptionalWindstormOrHailDeductible = "500"
                        Case "9"
                            _OptionalWindstormOrHailDeductible = "1,000"
                        Case "15"
                            _OptionalWindstormOrHailDeductible = "2,500"
                        Case "16"
                            _OptionalWindstormOrHailDeductible = "5,000"
                        Case "17"
                            _OptionalWindstormOrHailDeductible = "10,000"
                        Case "19"
                            _OptionalWindstormOrHailDeductible = "25,000"
                        Case "20"
                            _OptionalWindstormOrHailDeductible = "50,000"
                        Case "21"
                            _OptionalWindstormOrHailDeductible = "75,000"
                        Case "32"
                            _OptionalWindstormOrHailDeductible = "1%"
                        Case "33"
                            _OptionalWindstormOrHailDeductible = "2%"
                        Case "34"
                            _OptionalWindstormOrHailDeductible = "5%"
                        Case "42"
                            _OptionalWindstormOrHailDeductible = "Same"
                        Case "36"
                            _OptionalWindstormOrHailDeductible = "10%"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21170</remarks>
        Public Property OptionalWindstormOrHailDeductible As String
            Get
                Return _OptionalWindstormOrHailDeductible
            End Get
            Set(value As String)
                _OptionalWindstormOrHailDeductible = value
                Select Case _OptionalWindstormOrHailDeductible
                    Case "N/A"
                        _OptionalWindstormOrHailDeductibleId = "0"
                    Case "250"
                        _OptionalWindstormOrHailDeductibleId = "4"
                    Case "500"
                        _OptionalWindstormOrHailDeductibleId = "8"
                    Case "1,000"
                        _OptionalWindstormOrHailDeductibleId = "9"
                    Case "2,500"
                        _OptionalWindstormOrHailDeductibleId = "15"
                    Case "5,000"
                        _OptionalWindstormOrHailDeductibleId = "16"
                    Case "10,000"
                        _OptionalWindstormOrHailDeductibleId = "17"
                    Case "25,000"
                        _OptionalWindstormOrHailDeductibleId = "19"
                    Case "50,000"
                        _OptionalWindstormOrHailDeductibleId = "20"
                    Case "75,000"
                        _OptionalWindstormOrHailDeductibleId = "21"
                    Case "1%"
                        _OptionalWindstormOrHailDeductibleId = "32"
                    Case "2%"
                        _OptionalWindstormOrHailDeductibleId = "33"
                    Case "5%"
                        _OptionalWindstormOrHailDeductibleId = "34"
                    Case "Same"
                        _OptionalWindstormOrHailDeductibleId = "42"
                    Case "10%"
                        _OptionalWindstormOrHailDeductibleId = "36"
                    Case Else
                        _OptionalWindstormOrHailDeductibleId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21168 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others] for coverage w/ coveragecode_id = 21090); found inside ScheduledCoverage</remarks>
        Public Property PersPropCov_OptionalTheftDeductibleId As String 'UI doesn't have the % values
            Get
                Return _PersPropCov_OptionalTheftDeductibleId
            End Get
            Set(value As String)
                _PersPropCov_OptionalTheftDeductibleId = value
                _PersPropCov_OptionalTheftDeductible = ""
                If IsNumeric(_PersPropCov_OptionalTheftDeductibleId) = True Then
                    Select Case _PersPropCov_OptionalTheftDeductibleId
                        Case "0"
                            _PersPropCov_OptionalTheftDeductible = "N/A"
                        Case "4"
                            _PersPropCov_OptionalTheftDeductible = "250"
                        Case "8"
                            _PersPropCov_OptionalTheftDeductible = "500"
                        Case "9"
                            _PersPropCov_OptionalTheftDeductible = "1,000"
                        Case "15"
                            _PersPropCov_OptionalTheftDeductible = "2,500"
                        Case "16"
                            _PersPropCov_OptionalTheftDeductible = "5,000"
                        Case "17"
                            _PersPropCov_OptionalTheftDeductible = "10,000"
                        Case "19"
                            _PersPropCov_OptionalTheftDeductible = "25,000"
                        Case "20"
                            _PersPropCov_OptionalTheftDeductible = "50,000"
                        Case "21"
                            _PersPropCov_OptionalTheftDeductible = "75,000"
                        Case "32"
                            _PersPropCov_OptionalTheftDeductible = "1%"
                        Case "33"
                            _PersPropCov_OptionalTheftDeductible = "2%"
                        Case "34"
                            _PersPropCov_OptionalTheftDeductible = "5%"
                        Case "42"
                            _PersPropCov_OptionalTheftDeductible = "Same"
                        Case "36"
                            _PersPropCov_OptionalTheftDeductible = "10%"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21168 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others] for coverage w/ coveragecode_id = 21090); found inside ScheduledCoverage</remarks>
        Public Property PersPropCov_OptionalTheftDeductible As String
            Get
                Return _PersPropCov_OptionalTheftDeductible
            End Get
            Set(value As String)
                _PersPropCov_OptionalTheftDeductible = value
                Select Case _PersPropCov_OptionalTheftDeductible
                    Case "N/A"
                        _PersPropCov_OptionalTheftDeductibleId = "0"
                    Case "250"
                        _PersPropCov_OptionalTheftDeductibleId = "4"
                    Case "500"
                        _PersPropCov_OptionalTheftDeductibleId = "8"
                    Case "1,000"
                        _PersPropCov_OptionalTheftDeductibleId = "9"
                    Case "2,500"
                        _PersPropCov_OptionalTheftDeductibleId = "15"
                    Case "5,000"
                        _PersPropCov_OptionalTheftDeductibleId = "16"
                    Case "10,000"
                        _PersPropCov_OptionalTheftDeductibleId = "17"
                    Case "25,000"
                        _PersPropCov_OptionalTheftDeductibleId = "19"
                    Case "50,000"
                        _PersPropCov_OptionalTheftDeductibleId = "20"
                    Case "75,000"
                        _PersPropCov_OptionalTheftDeductibleId = "21"
                    Case "1%"
                        _PersPropCov_OptionalTheftDeductibleId = "32"
                    Case "2%"
                        _PersPropCov_OptionalTheftDeductibleId = "33"
                    Case "5%"
                        _PersPropCov_OptionalTheftDeductibleId = "34"
                    Case "Same"
                        _PersPropCov_OptionalTheftDeductibleId = "42"
                    Case "10%"
                        _PersPropCov_OptionalTheftDeductibleId = "36"
                    Case Else
                        _PersPropCov_OptionalTheftDeductibleId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21171 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others] for coverage w/ coveragecode_id = 21090); found inside ScheduledCoverage</remarks>
        Public Property PersPropCov_OptionalWindstormOrHailDeductibleId As String
            Get
                Return _PersPropCov_OptionalWindstormOrHailDeductibleId
            End Get
            Set(value As String)
                _PersPropCov_OptionalWindstormOrHailDeductibleId = value
                _PersPropCov_OptionalWindstormOrHailDeductible = ""
                If IsNumeric(_PersPropCov_OptionalWindstormOrHailDeductibleId) = True Then
                    Select Case _PersPropCov_OptionalWindstormOrHailDeductibleId
                        Case "0"
                            _PersPropCov_OptionalWindstormOrHailDeductible = "N/A"
                        Case "4"
                            _PersPropCov_OptionalWindstormOrHailDeductible = "250"
                        Case "8"
                            _PersPropCov_OptionalWindstormOrHailDeductible = "500"
                        Case "9"
                            _PersPropCov_OptionalWindstormOrHailDeductible = "1,000"
                        Case "15"
                            _PersPropCov_OptionalWindstormOrHailDeductible = "2,500"
                        Case "16"
                            _PersPropCov_OptionalWindstormOrHailDeductible = "5,000"
                        Case "17"
                            _PersPropCov_OptionalWindstormOrHailDeductible = "10,000"
                        Case "19"
                            _PersPropCov_OptionalWindstormOrHailDeductible = "25,000"
                        Case "20"
                            _PersPropCov_OptionalWindstormOrHailDeductible = "50,000"
                        Case "21"
                            _PersPropCov_OptionalWindstormOrHailDeductible = "75,000"
                        Case "32"
                            _PersPropCov_OptionalWindstormOrHailDeductible = "1%"
                        Case "33"
                            _PersPropCov_OptionalWindstormOrHailDeductible = "2%"
                        Case "34"
                            _PersPropCov_OptionalWindstormOrHailDeductible = "5%"
                        Case "42"
                            _PersPropCov_OptionalWindstormOrHailDeductible = "Same"
                        Case "36"
                            _PersPropCov_OptionalWindstormOrHailDeductible = "10%"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21171 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others] for coverage w/ coveragecode_id = 21090); found inside ScheduledCoverage</remarks>
        Public Property PersPropCov_OptionalWindstormOrHailDeductible As String
            Get
                Return _PersPropCov_OptionalWindstormOrHailDeductible
            End Get
            Set(value As String)
                _PersPropCov_OptionalWindstormOrHailDeductible = value
                Select Case _PersPropCov_OptionalWindstormOrHailDeductible
                    Case "N/A"
                        _PersPropCov_OptionalWindstormOrHailDeductibleId = "0"
                    Case "250"
                        _PersPropCov_OptionalWindstormOrHailDeductibleId = "4"
                    Case "500"
                        _PersPropCov_OptionalWindstormOrHailDeductibleId = "8"
                    Case "1,000"
                        _PersPropCov_OptionalWindstormOrHailDeductibleId = "9"
                    Case "2,500"
                        _PersPropCov_OptionalWindstormOrHailDeductibleId = "15"
                    Case "5,000"
                        _PersPropCov_OptionalWindstormOrHailDeductibleId = "16"
                    Case "10,000"
                        _PersPropCov_OptionalWindstormOrHailDeductibleId = "17"
                    Case "25,000"
                        _PersPropCov_OptionalWindstormOrHailDeductibleId = "19"
                    Case "50,000"
                        _PersPropCov_OptionalWindstormOrHailDeductibleId = "20"
                    Case "75,000"
                        _PersPropCov_OptionalWindstormOrHailDeductibleId = "21"
                    Case "1%"
                        _PersPropCov_OptionalWindstormOrHailDeductibleId = "32"
                    Case "2%"
                        _PersPropCov_OptionalWindstormOrHailDeductibleId = "33"
                    Case "5%"
                        _PersPropCov_OptionalWindstormOrHailDeductibleId = "34"
                    Case "Same"
                        _PersPropCov_OptionalWindstormOrHailDeductibleId = "42"
                    Case "10%"
                        _PersPropCov_OptionalWindstormOrHailDeductibleId = "36"
                    Case Else
                        _PersPropCov_OptionalWindstormOrHailDeductibleId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21168 (when BusinessPropertyTypeId = 8 [Personal Property of Others] for coverage w/ coveragecode_id = 21090); found inside ScheduledCoverage</remarks>
        Public Property PersPropOfOthers_OptionalTheftDeductibleId As String 'UI doesn't have the % values
            Get
                Return _PersPropOfOthers_OptionalTheftDeductibleId
            End Get
            Set(value As String)
                _PersPropOfOthers_OptionalTheftDeductibleId = value
                _PersPropOfOthers_OptionalTheftDeductible = ""
                If IsNumeric(_PersPropOfOthers_OptionalTheftDeductibleId) = True Then
                    Select Case _PersPropOfOthers_OptionalTheftDeductibleId
                        Case "0"
                            _PersPropOfOthers_OptionalTheftDeductible = "N/A"
                        Case "4"
                            _PersPropOfOthers_OptionalTheftDeductible = "250"
                        Case "8"
                            _PersPropOfOthers_OptionalTheftDeductible = "500"
                        Case "9"
                            _PersPropOfOthers_OptionalTheftDeductible = "1,000"
                        Case "15"
                            _PersPropOfOthers_OptionalTheftDeductible = "2,500"
                        Case "16"
                            _PersPropOfOthers_OptionalTheftDeductible = "5,000"
                        Case "17"
                            _PersPropOfOthers_OptionalTheftDeductible = "10,000"
                        Case "19"
                            _PersPropOfOthers_OptionalTheftDeductible = "25,000"
                        Case "20"
                            _PersPropOfOthers_OptionalTheftDeductible = "50,000"
                        Case "21"
                            _PersPropOfOthers_OptionalTheftDeductible = "75,000"
                        Case "32"
                            _PersPropOfOthers_OptionalTheftDeductible = "1%"
                        Case "33"
                            _PersPropOfOthers_OptionalTheftDeductible = "2%"
                        Case "34"
                            _PersPropOfOthers_OptionalTheftDeductible = "5%"
                        Case "42"
                            _PersPropOfOthers_OptionalTheftDeductible = "Same"
                        Case "36"
                            _PersPropOfOthers_OptionalTheftDeductible = "10%"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21168 (when BusinessPropertyTypeId = 8 [Personal Property of Others] for coverage w/ coveragecode_id = 21090); found inside ScheduledCoverage</remarks>
        Public Property PersPropOfOthers_OptionalTheftDeductible As String
            Get
                Return _PersPropOfOthers_OptionalTheftDeductible
            End Get
            Set(value As String)
                _PersPropOfOthers_OptionalTheftDeductible = value
                Select Case _PersPropOfOthers_OptionalTheftDeductible
                    Case "N/A"
                        _PersPropOfOthers_OptionalTheftDeductibleId = "0"
                    Case "250"
                        _PersPropOfOthers_OptionalTheftDeductibleId = "4"
                    Case "500"
                        _PersPropOfOthers_OptionalTheftDeductibleId = "8"
                    Case "1,000"
                        _PersPropOfOthers_OptionalTheftDeductibleId = "9"
                    Case "2,500"
                        _PersPropOfOthers_OptionalTheftDeductibleId = "15"
                    Case "5,000"
                        _PersPropOfOthers_OptionalTheftDeductibleId = "16"
                    Case "10,000"
                        _PersPropOfOthers_OptionalTheftDeductibleId = "17"
                    Case "25,000"
                        _PersPropOfOthers_OptionalTheftDeductibleId = "19"
                    Case "50,000"
                        _PersPropOfOthers_OptionalTheftDeductibleId = "20"
                    Case "75,000"
                        _PersPropOfOthers_OptionalTheftDeductibleId = "21"
                    Case "1%"
                        _PersPropOfOthers_OptionalTheftDeductibleId = "32"
                    Case "2%"
                        _PersPropOfOthers_OptionalTheftDeductibleId = "33"
                    Case "5%"
                        _PersPropOfOthers_OptionalTheftDeductibleId = "34"
                    Case "Same"
                        _PersPropOfOthers_OptionalTheftDeductibleId = "42"
                    Case "10%"
                        _PersPropOfOthers_OptionalTheftDeductibleId = "36"
                    Case Else
                        _PersPropOfOthers_OptionalTheftDeductibleId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21171 (when BusinessPropertyTypeId = 8 [Personal Property of Others] for coverage w/ coveragecode_id = 21090); found inside ScheduledCoverage</remarks>
        Public Property PersPropOfOthers_OptionalWindstormOrHailDeductibleId As String
            Get
                Return _PersPropOfOthers_OptionalWindstormOrHailDeductibleId
            End Get
            Set(value As String)
                _PersPropOfOthers_OptionalWindstormOrHailDeductibleId = value
                _PersPropOfOthers_OptionalWindstormOrHailDeductible = ""
                If IsNumeric(_PersPropOfOthers_OptionalWindstormOrHailDeductibleId) = True Then
                    Select Case _PersPropOfOthers_OptionalWindstormOrHailDeductibleId
                        Case "0"
                            _PersPropOfOthers_OptionalWindstormOrHailDeductible = "N/A"
                        Case "4"
                            _PersPropOfOthers_OptionalWindstormOrHailDeductible = "250"
                        Case "8"
                            _PersPropOfOthers_OptionalWindstormOrHailDeductible = "500"
                        Case "9"
                            _PersPropOfOthers_OptionalWindstormOrHailDeductible = "1,000"
                        Case "15"
                            _PersPropOfOthers_OptionalWindstormOrHailDeductible = "2,500"
                        Case "16"
                            _PersPropOfOthers_OptionalWindstormOrHailDeductible = "5,000"
                        Case "17"
                            _PersPropOfOthers_OptionalWindstormOrHailDeductible = "10,000"
                        Case "19"
                            _PersPropOfOthers_OptionalWindstormOrHailDeductible = "25,000"
                        Case "20"
                            _PersPropOfOthers_OptionalWindstormOrHailDeductible = "50,000"
                        Case "21"
                            _PersPropOfOthers_OptionalWindstormOrHailDeductible = "75,000"
                        Case "32"
                            _PersPropOfOthers_OptionalWindstormOrHailDeductible = "1%"
                        Case "33"
                            _PersPropOfOthers_OptionalWindstormOrHailDeductible = "2%"
                        Case "34"
                            _PersPropOfOthers_OptionalWindstormOrHailDeductible = "5%"
                        Case "42"
                            _PersPropOfOthers_OptionalWindstormOrHailDeductible = "Same"
                        Case "36"
                            _PersPropOfOthers_OptionalWindstormOrHailDeductible = "10%"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21171 (when BusinessPropertyTypeId = 8 [Personal Property of Others] for coverage w/ coveragecode_id = 21090); found inside ScheduledCoverage</remarks>
        Public Property PersPropOfOthers_OptionalWindstormOrHailDeductible As String
            Get
                Return _PersPropOfOthers_OptionalWindstormOrHailDeductible
            End Get
            Set(value As String)
                _PersPropOfOthers_OptionalWindstormOrHailDeductible = value
                Select Case _PersPropOfOthers_OptionalWindstormOrHailDeductible
                    Case "N/A"
                        _PersPropOfOthers_OptionalWindstormOrHailDeductibleId = "0"
                    Case "250"
                        _PersPropOfOthers_OptionalWindstormOrHailDeductibleId = "4"
                    Case "500"
                        _PersPropOfOthers_OptionalWindstormOrHailDeductibleId = "8"
                    Case "1,000"
                        _PersPropOfOthers_OptionalWindstormOrHailDeductibleId = "9"
                    Case "2,500"
                        _PersPropOfOthers_OptionalWindstormOrHailDeductibleId = "15"
                    Case "5,000"
                        _PersPropOfOthers_OptionalWindstormOrHailDeductibleId = "16"
                    Case "10,000"
                        _PersPropOfOthers_OptionalWindstormOrHailDeductibleId = "17"
                    Case "25,000"
                        _PersPropOfOthers_OptionalWindstormOrHailDeductibleId = "19"
                    Case "50,000"
                        _PersPropOfOthers_OptionalWindstormOrHailDeductibleId = "20"
                    Case "75,000"
                        _PersPropOfOthers_OptionalWindstormOrHailDeductibleId = "21"
                    Case "1%"
                        _PersPropOfOthers_OptionalWindstormOrHailDeductibleId = "32"
                    Case "2%"
                        _PersPropOfOthers_OptionalWindstormOrHailDeductibleId = "33"
                    Case "5%"
                        _PersPropOfOthers_OptionalWindstormOrHailDeductibleId = "34"
                    Case "Same"
                        _PersPropOfOthers_OptionalWindstormOrHailDeductibleId = "42"
                    Case "10%"
                        _PersPropOfOthers_OptionalWindstormOrHailDeductibleId = "36"
                    Case Else
                        _PersPropOfOthers_OptionalWindstormOrHailDeductibleId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropCov_DoesYardRateApplyTypeId As String '0=N/A; 1=Yes; 2=No
            Get
                Return _PersPropCov_DoesYardRateApplyTypeId
            End Get
            Set(value As String)
                _PersPropCov_DoesYardRateApplyTypeId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId = 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropOfOthers_DoesYardRateApplyTypeId As String '0=N/A; 1=Yes; 2=No
            Get
                Return _PersPropOfOthers_DoesYardRateApplyTypeId
            End Get
            Set(value As String)
                _PersPropOfOthers_DoesYardRateApplyTypeId = value
            End Set
        End Property

        Public Property FeetToFireHydrant As String
            Get
                Return _FeetToFireHydrant
            End Get
            Set(value As String)
                _FeetToFireHydrant = value
            End Set
        End Property
        Public Property MilesToFireDepartment As String
            Get
                Return _MilesToFireDepartment
            End Get
            Set(value As String)
                _MilesToFireDepartment = value
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropCov_IncludedInBlanketCoverage As Boolean
            Get
                Return _PersPropCov_IncludedInBlanketCoverage
            End Get
            Set(value As Boolean)
                _PersPropCov_IncludedInBlanketCoverage = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId = 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersPropOfOthers_IncludedInBlanketCoverage As Boolean
            Get
                Return _PersPropOfOthers_IncludedInBlanketCoverage
            End Get
            Set(value As Boolean)
                _PersPropOfOthers_IncludedInBlanketCoverage = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21095</remarks>
        Public Property BusinessIncomeCov_IncludedInBlanketCoverage As Boolean
            Get
                Return _BusinessIncomeCov_IncludedInBlanketCoverage
            End Get
            Set(value As Boolean)
                _BusinessIncomeCov_IncludedInBlanketCoverage = value
            End Set
        End Property

        'added 2/18/2014
        Public Property HasConvertedClassifications As Boolean
            Get
                Return _HasConvertedClassifications
            End Get
            Set(value As Boolean)
                _HasConvertedClassifications = value
            End Set
        End Property
        Public Property HasConvertedCoverages As Boolean
            Get
                Return _HasConvertedCoverages
            End Get
            Set(value As Boolean)
                _HasConvertedCoverages = value
            End Set
        End Property
        Public Property HasConvertedScheduledCoverages As Boolean
            Get
                Return _HasConvertedScheduledCoverages
            End Get
            Set(value As Boolean)
                _HasConvertedScheduledCoverages = value
            End Set
        End Property

        Public Property PremiumFullterm As String 'added 4/2/2014
            Get
                'Return _PremiumFullterm
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_PremiumFullterm)
            End Get
            Set(value As String)
                _PremiumFullterm = value
                qqHelper.ConvertToQuotedPremiumFormat(_PremiumFullterm)
            End Set
        End Property

        Public Property FarmBarnBuildingNum As String 'added 4/23/2014 for reconciliation
            Get
                Return _FarmBarnBuildingNum
            End Get
            Set(value As String)
                _FarmBarnBuildingNum = value
            End Set
        End Property
        Public Property CanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean 'added 4/29/2014
            Get
                Return _CanUseAdditionalInterestNumForAdditionalInterestReconciliation
            End Get
            Set(value As Boolean)
                _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = value
            End Set
        End Property
        'added 10/18/2018 for multi-state
        Public Property FarmBarnBuildingNum_MasterPart As String
            Get
                Return _FarmBarnBuildingNum_MasterPart
            End Get
            Set(value As String)
                _FarmBarnBuildingNum_MasterPart = value
            End Set
        End Property
        Public Property FarmBarnBuildingNum_CGLPart As String
            Get
                Return _FarmBarnBuildingNum_CGLPart
            End Get
            Set(value As String)
                _FarmBarnBuildingNum_CGLPart = value
            End Set
        End Property
        Public Property FarmBarnBuildingNum_CPRPart As String
            Get
                Return _FarmBarnBuildingNum_CPRPart
            End Get
            Set(value As String)
                _FarmBarnBuildingNum_CPRPart = value
            End Set
        End Property
        Public Property FarmBarnBuildingNum_CIMPart As String
            Get
                Return _FarmBarnBuildingNum_CIMPart
            End Get
            Set(value As String)
                _FarmBarnBuildingNum_CIMPart = value
            End Set
        End Property
        Public Property FarmBarnBuildingNum_CRMPart As String
            Get
                Return _FarmBarnBuildingNum_CRMPart
            End Get
            Set(value As String)
                _FarmBarnBuildingNum_CRMPart = value
            End Set
        End Property
        Public Property FarmBarnBuildingNum_GARPart As String
            Get
                Return _FarmBarnBuildingNum_GARPart
            End Get
            Set(value As String)
                _FarmBarnBuildingNum_GARPart = value
            End Set
        End Property

        Public Property Modifiers As List(Of QuickQuoteModifier) 'added 10/16/2014
            Get
                SetParentOfListItems(_Modifiers, "{09860584-30E8-475E-A428-409826D39D25}")
                Return _Modifiers
            End Get
            Set(value As List(Of QuickQuoteModifier))
                _Modifiers = value
                SetParentOfListItems(_Modifiers, "{09860584-30E8-475E-A428-409826D39D25}")
            End Set
        End Property

        Public Property CanUseScheduledCoverageNumForScheduledCoverageReconciliation As Boolean 'added 1/22/2015
            Get
                Return _CanUseScheduledCoverageNumForScheduledCoverageReconciliation
            End Get
            Set(value As Boolean)
                _CanUseScheduledCoverageNumForScheduledCoverageReconciliation = value
            End Set
        End Property
        'added 1/22/2015
        Public Property PersPropCov_ScheduledCoverageNum As String
            Get
                Return _PersPropCov_ScheduledCoverageNum
            End Get
            Set(value As String)
                _PersPropCov_ScheduledCoverageNum = value
            End Set
        End Property
        Public Property PersPropOfOthers_ScheduledCoverageNum As String
            Get
                Return _PersPropOfOthers_ScheduledCoverageNum
            End Get
            Set(value As String)
                _PersPropOfOthers_ScheduledCoverageNum = value
            End Set
        End Property

        'added 2/9/2014 for CIM
        Public Property ComputerHardwareLimit As String 'cov also has CoverageBasisTypeId set to 1
            Get
                Return _ComputerHardwareLimit
            End Get
            Set(value As String)
                _ComputerHardwareLimit = value
                qqHelper.ConvertToLimitFormat(_ComputerHardwareLimit)
            End Set
        End Property
        Public Property ComputerHardwareRate As String
            Get
                Return _ComputerHardwareRate
            End Get
            Set(value As String)
                _ComputerHardwareRate = value
            End Set
        End Property
        Public Property ComputerHardwareQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_ComputerHardwareQuotedPremium)
            End Get
            Set(value As String)
                _ComputerHardwareQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_ComputerHardwareQuotedPremium)
            End Set
        End Property
        Public Property ComputerProgramsApplicationsAndMediaLimit As String 'cov also has CoverageBasisTypeId set to 1
            Get
                Return _ComputerProgramsApplicationsAndMediaLimit
            End Get
            Set(value As String)
                _ComputerProgramsApplicationsAndMediaLimit = value
                qqHelper.ConvertToLimitFormat(_ComputerProgramsApplicationsAndMediaLimit)
            End Set
        End Property
        Public Property ComputerProgramsApplicationsAndMediaRate As String
            Get
                Return _ComputerProgramsApplicationsAndMediaRate
            End Get
            Set(value As String)
                _ComputerProgramsApplicationsAndMediaRate = value
            End Set
        End Property
        Public Property ComputerProgramsApplicationsAndMediaQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_ComputerProgramsApplicationsAndMediaQuotedPremium)
            End Get
            Set(value As String)
                _ComputerProgramsApplicationsAndMediaQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_ComputerProgramsApplicationsAndMediaQuotedPremium)
            End Set
        End Property
        Public Property ComputerBusinessIncomeLimit As String 'cov also has CoverageBasisTypeId set to 1
            Get
                Return _ComputerBusinessIncomeLimit
            End Get
            Set(value As String)
                _ComputerBusinessIncomeLimit = value
                qqHelper.ConvertToLimitFormat(_ComputerBusinessIncomeLimit)
            End Set
        End Property
        Public Property ComputerBusinessIncomeRate As String
            Get
                Return _ComputerBusinessIncomeRate
            End Get
            Set(value As String)
                _ComputerBusinessIncomeRate = value
            End Set
        End Property
        Public Property ComputerBusinessIncomeQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_ComputerBusinessIncomeQuotedPremium)
            End Get
            Set(value As String)
                _ComputerBusinessIncomeQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_ComputerBusinessIncomeQuotedPremium)
            End Set
        End Property
        'added 3/16/2015
        Public Property FineArtsScheduledItems As List(Of QuickQuoteFineArtsScheduledItem)
            Get
                SetParentOfListItems(_FineArtsScheduledItems, "{09860584-30E8-475E-A428-409826D39D26}")
                Return _FineArtsScheduledItems
            End Get
            Set(value As List(Of QuickQuoteFineArtsScheduledItem))
                _FineArtsScheduledItems = value
                SetParentOfListItems(_FineArtsScheduledItems, "{09860584-30E8-475E-A428-409826D39D26}")
            End Set
        End Property
        'added 3/26/2015
        Public Property ScheduledSigns As List(Of QuickQuoteScheduledSign)
            Get
                SetParentOfListItems(_ScheduledSigns, "{09860584-30E8-475E-A428-409826D39D27}")
                Return _ScheduledSigns
            End Get
            Set(value As List(Of QuickQuoteScheduledSign))
                _ScheduledSigns = value
                SetParentOfListItems(_ScheduledSigns, "{09860584-30E8-475E-A428-409826D39D27}")
            End Set
        End Property
        Public Property UnscheduledSignsLimit As String 'cov also has CoverageBasisTypeId set to 1
            Get
                Return _UnscheduledSignsLimit
            End Get
            Set(value As String)
                _UnscheduledSignsLimit = value
                qqHelper.ConvertToLimitFormat(_UnscheduledSignsLimit)
            End Set
        End Property
        Public Property UnscheduledSignsQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_UnscheduledSignsQuotedPremium)
            End Get
            Set(value As String)
                _UnscheduledSignsQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_UnscheduledSignsQuotedPremium)
            End Set
        End Property

        'added 6/15/2015 for Farm
        Public Property Dimensions As String
            Get
                Return _Dimensions
            End Get
            Set(value As String)
                _Dimensions = value
            End Set
        End Property
        Public Property FarmStructureTypeId As String 'static data
            Get
                Return _FarmStructureTypeId
            End Get
            Set(value As String)
                _FarmStructureTypeId = value
            End Set
        End Property
        Public Property FarmTypeId As String 'static data
            Get
                Return _FarmTypeId
            End Get
            Set(value As String)
                _FarmTypeId = value
            End Set
        End Property
        Public Property NumberOfSolidFuelBurningUnits As String
            Get
                Return _NumberOfSolidFuelBurningUnits
            End Get
            Set(value As String)
                _NumberOfSolidFuelBurningUnits = value
            End Set
        End Property
        Public Property VacancyFromDate As String
            Get
                Return _VacancyFromDate
            End Get
            Set(value As String)
                _VacancyFromDate = value
                qqHelper.ConvertToShortDate(_VacancyFromDate)
            End Set
        End Property
        Public Property VacancyToDate As String
            Get
                Return _VacancyToDate
            End Get
            Set(value As String)
                _VacancyToDate = value
                qqHelper.ConvertToShortDate(_VacancyToDate)
            End Set
        End Property
        Public Property HasConvertedModifiers As Boolean
            Get
                Return _HasConvertedModifiers
            End Get
            Set(value As Boolean)
                _HasConvertedModifiers = value
            End Set
        End Property
        Public Property SprinklerSystem_AllExcept As Boolean
            Get
                Return _SprinklerSystem_AllExcept
            End Get
            Set(value As Boolean)
                _SprinklerSystem_AllExcept = value
            End Set
        End Property
        Public Property SprinklerSystem_AllIncluding As Boolean
            Get
                Return _SprinklerSystem_AllIncluding
            End Get
            Set(value As Boolean)
                _SprinklerSystem_AllIncluding = value
            End Set
        End Property
        Public Property HeatedBuildingSurchargeGasElectric As Boolean
            Get
                Return _HeatedBuildingSurchargeGasElectric
            End Get
            Set(value As Boolean)
                _HeatedBuildingSurchargeGasElectric = value
            End Set
        End Property
        Public Property HeatedBuildingSurchargeOther As Boolean
            Get
                Return _HeatedBuildingSurchargeOther
            End Get
            Set(value As Boolean)
                _HeatedBuildingSurchargeOther = value
            End Set
        End Property
        Public Property ExposedInsulationSurcharge As Boolean
            Get
                Return _ExposedInsulationSurcharge
            End Get
            Set(value As Boolean)
                _ExposedInsulationSurcharge = value
            End Set
        End Property
        Public Property E_Farm_Limit As String
            Get
                Return _E_Farm_Limit
            End Get
            Set(value As String)
                _E_Farm_Limit = value
                qqHelper.ConvertToLimitFormat(_E_Farm_Limit)
            End Set
        End Property
        Public Property E_Farm_DeductibleLimitId As String 'static data
            Get
                Return _E_Farm_DeductibleLimitId
            End Get
            Set(value As String)
                _E_Farm_DeductibleLimitId = value
            End Set
        End Property
        Public Property E_Farm_QuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_E_Farm_QuotedPremium)
            End Get
            Set(value As String)
                _E_Farm_QuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_E_Farm_QuotedPremium)
            End Set
        End Property
        'added 6/16/2015 for Farm
        Public Property HouseholdContentsLimit As String
            Get
                Return _HouseholdContentsLimit
            End Get
            Set(value As String)
                _HouseholdContentsLimit = value
                qqHelper.ConvertToLimitFormat(_HouseholdContentsLimit)
            End Set
        End Property
        Public Property HouseholdContentsQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_HouseholdContentsQuotedPremium)
            End Get
            Set(value As String)
                _HouseholdContentsQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_HouseholdContentsQuotedPremium)
            End Set
        End Property
        'added 6/24/2015 for Farm
        Public Property OptionalCoverageEs As List(Of QuickQuoteOptionalCoverageE)
            Get
                SetParentOfListItems(_OptionalCoverageEs, "{09860584-30E8-475E-A428-409826D39D28}")
                Return _OptionalCoverageEs
            End Get
            Set(value As List(Of QuickQuoteOptionalCoverageE))
                _OptionalCoverageEs = value
                SetParentOfListItems(_OptionalCoverageEs, "{09860584-30E8-475E-A428-409826D39D28}")
            End Set
        End Property
        Public Property CanUseFarmBarnBuildingOptionalCoverageNumForOptionalCoverageEReconciliation As Boolean
            Get
                Return _CanUseFarmBarnBuildingOptionalCoverageNumForOptionalCoverageEReconciliation
            End Get
            Set(value As Boolean)
                _CanUseFarmBarnBuildingOptionalCoverageNumForOptionalCoverageEReconciliation = value
            End Set
        End Property
        Public Property HasConvertedOptionalCoverageEs As Boolean
            Get
                Return _HasConvertedOptionalCoverageEs
            End Get
            Set(value As Boolean)
                _HasConvertedOptionalCoverageEs = value
            End Set
        End Property
        'added 7/28/2015 for Farm e2Value
        Public Property PropertyValuation As QuickQuotePropertyValuation
            Get
                SetObjectsParent(_PropertyValuation)
                Return _PropertyValuation
            End Get
            Set(value As QuickQuotePropertyValuation)
                _PropertyValuation = value
                SetObjectsParent(_PropertyValuation)
            End Set
        End Property

        'added 2/17/2017
        Public Property UseBuildingClassificationPropertiesToCreateOneItemInList As Boolean
            Get
                Return _UseBuildingClassificationPropertiesToCreateOneItemInList
            End Get
            Set(value As Boolean)
                _UseBuildingClassificationPropertiesToCreateOneItemInList = value
            End Set
        End Property
        'added 2/20/2017
        Public Property CanUseClassificationNumForClassificationReconciliation As Boolean
            Get
                Return _CanUseClassificationNumForClassificationReconciliation
            End Get
            Set(value As Boolean)
                _CanUseClassificationNumForClassificationReconciliation = value
            End Set
        End Property

        '3/9/2017 - BOP stuff
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80383; stored in xml at location level</remarks>
        Public Property HasApartmentBuildings As Boolean
            Get
                Return _HasApartmentBuildings
            End Get
            Set(value As Boolean)
                _HasApartmentBuildings = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80383; stored in xml at location level</remarks>
        Public Property NumberOfLocationsWithApartments As String
            Get
                Return _NumberOfLocationsWithApartments
            End Get
            Set(value As String)
                _NumberOfLocationsWithApartments = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80383; stored in xml at location level</remarks>
        Public Property HasTenantAutoLegalLiability As Boolean
            Get
                Return _HasTenantAutoLegalLiability
            End Get
            Set(value As Boolean)
                _HasTenantAutoLegalLiability = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80385; stored in xml at location level</remarks>
        Public Property TenantAutoLegalLiabilityLimitOfLiabilityId As String
            Get
                Return _TenantAutoLegalLiabilityLimitOfLiabilityId
            End Get
            Set(value As String)
                _TenantAutoLegalLiabilityLimitOfLiabilityId = value
                Select Case _TenantAutoLegalLiabilityLimitOfLiabilityId
                    Case "0"
                        _TenantAutoLegalLiabilityLimitOfLiability = "N/A"
                    Case "161"
                        _TenantAutoLegalLiabilityLimitOfLiability = "6,000"
                    Case "6"
                        _TenantAutoLegalLiabilityLimitOfLiability = "7,500"
                    Case "268"
                        _TenantAutoLegalLiabilityLimitOfLiability = "9,000"
                    Case "375"
                        _TenantAutoLegalLiabilityLimitOfLiability = "12,000"
                    Case "48"
                        _TenantAutoLegalLiabilityLimitOfLiability = "15,000"
                    Case "376"
                        _TenantAutoLegalLiabilityLimitOfLiability = "18,000"
                    Case "377"
                        _TenantAutoLegalLiabilityLimitOfLiability = "22,500"
                    Case "62"
                        _TenantAutoLegalLiabilityLimitOfLiability = "30,000"
                    Case "378"
                        _TenantAutoLegalLiabilityLimitOfLiability = "37,500"
                    Case "282"
                        _TenantAutoLegalLiabilityLimitOfLiability = "45,000"
                    Case "324"
                        _TenantAutoLegalLiabilityLimitOfLiability = "60,000"
                    Case "50"
                        _TenantAutoLegalLiabilityLimitOfLiability = "75,000"
                    Case "379"
                        _TenantAutoLegalLiabilityLimitOfLiability = "90,000"
                    Case "380"
                        _TenantAutoLegalLiabilityLimitOfLiability = "120,000"
                    Case "52"
                        _TenantAutoLegalLiabilityLimitOfLiability = "150,000"
                    Case "381"
                        _TenantAutoLegalLiabilityLimitOfLiability = "180,000"
                    Case "54"
                        _TenantAutoLegalLiabilityLimitOfLiability = "225,000"
                    Case "33"
                        _TenantAutoLegalLiabilityLimitOfLiability = "300,000"
                    Case "382"
                        _TenantAutoLegalLiabilityLimitOfLiability = "375,000"
                    Case "383"
                        _TenantAutoLegalLiabilityLimitOfLiability = "450,000"
                    Case "178"
                        _TenantAutoLegalLiabilityLimitOfLiability = "600,000"
                    Case "180"
                        _TenantAutoLegalLiabilityLimitOfLiability = "750,000"
                    Case "182"
                        _TenantAutoLegalLiabilityLimitOfLiability = "900,000"
                    Case "183"
                        _TenantAutoLegalLiabilityLimitOfLiability = "1,200,000"
                    Case "185"
                        _TenantAutoLegalLiabilityLimitOfLiability = "1,500,000"
                    Case Else
                        _TenantAutoLegalLiabilityLimitOfLiability = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80385; stored in xml at location level</remarks>
        Public Property TenantAutoLegalLiabilityLimitOfLiability As String
            Get
                Return _TenantAutoLegalLiabilityLimitOfLiability
            End Get
            Set(value As String)
                _TenantAutoLegalLiabilityLimitOfLiability = value
                Select Case _TenantAutoLegalLiabilityLimitOfLiability.Replace(",", "")
                    Case "N/A"
                        _TenantAutoLegalLiabilityLimitOfLiabilityId = "0"
                    Case "6000"
                        _TenantAutoLegalLiabilityLimitOfLiabilityId = "161"
                    Case "7500"
                        _TenantAutoLegalLiabilityLimitOfLiabilityId = "6"
                    Case "9000"
                        _TenantAutoLegalLiabilityLimitOfLiabilityId = "268"
                    Case "12000"
                        _TenantAutoLegalLiabilityLimitOfLiabilityId = "375"
                    Case "15000"
                        _TenantAutoLegalLiabilityLimitOfLiabilityId = "48"
                    Case "18000"
                        _TenantAutoLegalLiabilityLimitOfLiabilityId = "376"
                    Case "22500"
                        _TenantAutoLegalLiabilityLimitOfLiabilityId = "377"
                    Case "30000"
                        _TenantAutoLegalLiabilityLimitOfLiabilityId = "62"
                    Case "37500"
                        _TenantAutoLegalLiabilityLimitOfLiabilityId = "378"
                    Case "45000"
                        _TenantAutoLegalLiabilityLimitOfLiabilityId = "282"
                    Case "60000"
                        _TenantAutoLegalLiabilityLimitOfLiabilityId = "324"
                    Case "75000"
                        _TenantAutoLegalLiabilityLimitOfLiabilityId = "50"
                    Case "90000"
                        _TenantAutoLegalLiabilityLimitOfLiabilityId = "379"
                    Case "120000"
                        _TenantAutoLegalLiabilityLimitOfLiabilityId = "380"
                    Case "150000"
                        _TenantAutoLegalLiabilityLimitOfLiabilityId = "52"
                    Case "180000"
                        _TenantAutoLegalLiabilityLimitOfLiabilityId = "381"
                    Case "225000"
                        _TenantAutoLegalLiabilityLimitOfLiabilityId = "54"
                    Case "300000"
                        _TenantAutoLegalLiabilityLimitOfLiabilityId = "33"
                    Case "375000"
                        _TenantAutoLegalLiabilityLimitOfLiabilityId = "382"
                    Case "450000"
                        _TenantAutoLegalLiabilityLimitOfLiabilityId = "383"
                    Case "600000"
                        _TenantAutoLegalLiabilityLimitOfLiabilityId = "178"
                    Case "750000"
                        _TenantAutoLegalLiabilityLimitOfLiabilityId = "180"
                    Case "900000"
                        _TenantAutoLegalLiabilityLimitOfLiabilityId = "182"
                    Case "1200000"
                        _TenantAutoLegalLiabilityLimitOfLiabilityId = "183"
                    Case "1500000"
                        _TenantAutoLegalLiabilityLimitOfLiabilityId = "185"
                    Case Else
                        _TenantAutoLegalLiabilityLimitOfLiabilityId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80385; stored in xml at location level</remarks>
        Public Property TenantAutoLegalLiabilityDeductibleId As String
            Get
                Return _TenantAutoLegalLiabilityDeductibleId
            End Get
            Set(value As String)
                _TenantAutoLegalLiabilityDeductibleId = value
                Select Case _TenantAutoLegalLiabilityDeductibleId
                    Case "0"
                        _TenantAutoLegalLiabilityDeductible = "N/A"
                    Case "43"
                        _TenantAutoLegalLiabilityDeductible = "250/500/500"
                    Case "44"
                        _TenantAutoLegalLiabilityDeductible = "250/1,000/500"
                    Case "45"
                        _TenantAutoLegalLiabilityDeductible = "500/2,500/500"
                    Case "46"
                        _TenantAutoLegalLiabilityDeductible = "500/2,500/1,000"
                    Case Else
                        _TenantAutoLegalLiabilityDeductible = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80385; stored in xml at location level</remarks>
        Public Property TenantAutoLegalLiabilityDeductible As String
            Get
                Return _TenantAutoLegalLiabilityDeductible
            End Get
            Set(value As String)
                _TenantAutoLegalLiabilityDeductible = value
                Select Case _TenantAutoLegalLiabilityDeductible.Replace(",", "")
                    Case "N/A"
                        _TenantAutoLegalLiabilityDeductibleId = "0"
                    Case "250/500/500"
                        _TenantAutoLegalLiabilityDeductibleId = "43"
                    Case "250/1000/500"
                        _TenantAutoLegalLiabilityDeductibleId = "44"
                    Case "500/2500/500"
                        _TenantAutoLegalLiabilityDeductibleId = "45"
                    Case "500/2500/1000"
                        _TenantAutoLegalLiabilityDeductibleId = "46"
                    Case Else
                        _TenantAutoLegalLiabilityDeductibleId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80383; stored in xml at location level</remarks>
        Public Property HasCustomerAutoLegalLiability As Boolean
            Get
                Return _HasCustomerAutoLegalLiability
            End Get
            Set(value As Boolean)
                _HasCustomerAutoLegalLiability = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80383; stored in xml at location level</remarks>
        Public Property CustomerAutoLegalLiabilityLimitOfLiabilityId As String
            Get
                Return _CustomerAutoLegalLiabilityLimitOfLiabilityId
            End Get
            Set(value As String)
                _CustomerAutoLegalLiabilityLimitOfLiabilityId = value
                Select Case _CustomerAutoLegalLiabilityLimitOfLiabilityId
                    Case "0"
                        _CustomerAutoLegalLiabilityLimitOfLiability = "N/A"
                    Case "161"
                        _CustomerAutoLegalLiabilityLimitOfLiability = "6,000"
                    Case "6"
                        _CustomerAutoLegalLiabilityLimitOfLiability = "7,500"
                    Case "268"
                        _CustomerAutoLegalLiabilityLimitOfLiability = "9,000"
                    Case "375"
                        _CustomerAutoLegalLiabilityLimitOfLiability = "12,000"
                    Case "48"
                        _CustomerAutoLegalLiabilityLimitOfLiability = "15,000"
                    Case "376"
                        _CustomerAutoLegalLiabilityLimitOfLiability = "18,000"
                    Case "377"
                        _CustomerAutoLegalLiabilityLimitOfLiability = "22,500"
                    Case "62"
                        _CustomerAutoLegalLiabilityLimitOfLiability = "30,000"
                    Case "378"
                        _CustomerAutoLegalLiabilityLimitOfLiability = "37,500"
                    Case "282"
                        _CustomerAutoLegalLiabilityLimitOfLiability = "45,000"
                    Case "324"
                        _CustomerAutoLegalLiabilityLimitOfLiability = "60,000"
                    Case "50"
                        _CustomerAutoLegalLiabilityLimitOfLiability = "75,000"
                    Case "379"
                        _CustomerAutoLegalLiabilityLimitOfLiability = "90,000"
                    Case "380"
                        _CustomerAutoLegalLiabilityLimitOfLiability = "120,000"
                    Case "52"
                        _CustomerAutoLegalLiabilityLimitOfLiability = "150,000"
                    Case "381"
                        _CustomerAutoLegalLiabilityLimitOfLiability = "180,000"
                    Case "54"
                        _CustomerAutoLegalLiabilityLimitOfLiability = "225,000"
                    Case "33"
                        _CustomerAutoLegalLiabilityLimitOfLiability = "300,000"
                    Case "382"
                        _CustomerAutoLegalLiabilityLimitOfLiability = "375,000"
                    Case "383"
                        _CustomerAutoLegalLiabilityLimitOfLiability = "450,000"
                    Case "178"
                        _CustomerAutoLegalLiabilityLimitOfLiability = "600,000"
                    Case "180"
                        _CustomerAutoLegalLiabilityLimitOfLiability = "750,000"
                    Case "182"
                        _CustomerAutoLegalLiabilityLimitOfLiability = "900,000"
                    Case "183"
                        _CustomerAutoLegalLiabilityLimitOfLiability = "1,200,000"
                    Case "185"
                        _CustomerAutoLegalLiabilityLimitOfLiability = "1,500,000"
                    Case Else
                        _CustomerAutoLegalLiabilityLimitOfLiability = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80383; stored in xml at location level</remarks>
        Public Property CustomerAutoLegalLiabilityLimitOfLiability As String
            Get
                Return _CustomerAutoLegalLiabilityLimitOfLiability
            End Get
            Set(value As String)
                _CustomerAutoLegalLiabilityLimitOfLiability = value
                Select Case _CustomerAutoLegalLiabilityLimitOfLiability.Replace(",", "")
                    Case "N/A"
                        _CustomerAutoLegalLiabilityLimitOfLiabilityId = "0"
                    Case "6000"
                        _CustomerAutoLegalLiabilityLimitOfLiabilityId = "161"
                    Case "7500"
                        _CustomerAutoLegalLiabilityLimitOfLiabilityId = "6"
                    Case "9000"
                        _CustomerAutoLegalLiabilityLimitOfLiabilityId = "268"
                    Case "12000"
                        _CustomerAutoLegalLiabilityLimitOfLiabilityId = "375"
                    Case "15000"
                        _CustomerAutoLegalLiabilityLimitOfLiabilityId = "48"
                    Case "18000"
                        _CustomerAutoLegalLiabilityLimitOfLiabilityId = "376"
                    Case "22500"
                        _CustomerAutoLegalLiabilityLimitOfLiabilityId = "377"
                    Case "30000"
                        _CustomerAutoLegalLiabilityLimitOfLiabilityId = "62"
                    Case "37500"
                        _CustomerAutoLegalLiabilityLimitOfLiabilityId = "378"
                    Case "45000"
                        _CustomerAutoLegalLiabilityLimitOfLiabilityId = "282"
                    Case "60000"
                        _CustomerAutoLegalLiabilityLimitOfLiabilityId = "324"
                    Case "75000"
                        _CustomerAutoLegalLiabilityLimitOfLiabilityId = "50"
                    Case "90000"
                        _CustomerAutoLegalLiabilityLimitOfLiabilityId = "379"
                    Case "120000"
                        _CustomerAutoLegalLiabilityLimitOfLiabilityId = "380"
                    Case "150000"
                        _CustomerAutoLegalLiabilityLimitOfLiabilityId = "52"
                    Case "180000"
                        _CustomerAutoLegalLiabilityLimitOfLiabilityId = "381"
                    Case "225000"
                        _CustomerAutoLegalLiabilityLimitOfLiabilityId = "54"
                    Case "300000"
                        _CustomerAutoLegalLiabilityLimitOfLiabilityId = "33"
                    Case "375000"
                        _CustomerAutoLegalLiabilityLimitOfLiabilityId = "382"
                    Case "450000"
                        _CustomerAutoLegalLiabilityLimitOfLiabilityId = "383"
                    Case "600000"
                        _CustomerAutoLegalLiabilityLimitOfLiabilityId = "178"
                    Case "750000"
                        _CustomerAutoLegalLiabilityLimitOfLiabilityId = "180"
                    Case "900000"
                        _CustomerAutoLegalLiabilityLimitOfLiabilityId = "182"
                    Case "1200000"
                        _CustomerAutoLegalLiabilityLimitOfLiabilityId = "183"
                    Case "1500000"
                        _CustomerAutoLegalLiabilityLimitOfLiabilityId = "185"
                    Case Else
                        _CustomerAutoLegalLiabilityLimitOfLiabilityId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80383; stored in xml at location level</remarks>
        Public Property CustomerAutoLegalLiabilityDeductibleId As String
            Get
                Return _CustomerAutoLegalLiabilityDeductibleId
            End Get
            Set(value As String)
                _CustomerAutoLegalLiabilityDeductibleId = value
                Select Case _CustomerAutoLegalLiabilityDeductibleId
                    Case "0"
                        _CustomerAutoLegalLiabilityDeductible = "N/A"
                    Case "43"
                        _CustomerAutoLegalLiabilityDeductible = "250/500/500"
                    Case "44"
                        _CustomerAutoLegalLiabilityDeductible = "250/1,000/500"
                    Case "45"
                        _CustomerAutoLegalLiabilityDeductible = "500/2,500/500"
                    Case "46"
                        _CustomerAutoLegalLiabilityDeductible = "500/2,500/1,000"
                    Case Else
                        _CustomerAutoLegalLiabilityDeductible = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80383; stored in xml at location level</remarks>
        Public Property CustomerAutoLegalLiabilityDeductible As String
            Get
                Return _CustomerAutoLegalLiabilityDeductible
            End Get
            Set(value As String)
                _CustomerAutoLegalLiabilityDeductible = value
                Select Case _CustomerAutoLegalLiabilityDeductible.Replace(",", "")
                    Case "N/A"
                        _CustomerAutoLegalLiabilityDeductibleId = "0"
                    Case "250/500/500"
                        _CustomerAutoLegalLiabilityDeductibleId = "43"
                    Case "250/1000/500"
                        _CustomerAutoLegalLiabilityDeductibleId = "44"
                    Case "500/2500/500"
                        _CustomerAutoLegalLiabilityDeductibleId = "45"
                    Case "500/2500/1000"
                        _CustomerAutoLegalLiabilityDeductibleId = "46"
                    Case Else
                        _CustomerAutoLegalLiabilityDeductibleId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80383; stored in xml at location level</remarks>
        Public Property HasFineArts As Boolean
            Get
                Return _HasFineArts
            End Get
            Set(value As Boolean)
                _HasFineArts = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80380</remarks>
        Public Property HasResidentialCleaning As Boolean
            Get
                Return _HasResidentialCleaning
            End Get
            Set(value As Boolean)
                _HasResidentialCleaning = value
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21134</remarks>
        Public Property HasLiquorLiability As Boolean
            Get
                Return _HasLiquorLiability
            End Get
            Set(value As Boolean)
                _HasLiquorLiability = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21134</remarks>
        Public Property LiquorLiabilityAnnualGrossAlcoholSalesReceipts As String
            Get
                Return _LiquorLiabilityAnnualGrossAlcoholSalesReceipts
            End Get
            Set(value As String)
                _LiquorLiabilityAnnualGrossAlcoholSalesReceipts = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21134</remarks>
        Public Property LiquorLiabilityAnnualGrossPackageSalesReceipts As String
            Get
                Return _LiquorLiabilityAnnualGrossPackageSalesReceipts
            End Get
            Set(value As String)
                _LiquorLiabilityAnnualGrossPackageSalesReceipts = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21134</remarks>
        Public Property LiquorLiabilityAggregateLimit As String
            Get
                Return _LiquorLiabilityAggregateLimit
            End Get
            Set(value As String)
                _LiquorLiabilityAggregateLimit = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21134</remarks>
        Public Property LiquorLiabilityClassCodeTypeId As String
            Get
                Return _LiquorLiabilityClassCodeTypeId
            End Get
            Set(value As String)
                _LiquorLiabilityClassCodeTypeId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80381</remarks>
        Public Property HasRestaurantEndorsement As Boolean
            Get
                Return _HasRestaurantEndorsement
            End Get
            Set(value As Boolean)
                _HasRestaurantEndorsement = value
            End Set
        End Property

        'added 7/8/2017
        Public Property BuildingPersonalProperties As List(Of QuickQuoteBuildingPersonalProperty)
            Get
                SetParentOfListItems(_BuildingPersonalProperties, "{09860584-30E8-475E-A428-409826D39D29}")
                Return _BuildingPersonalProperties
            End Get
            Set(value As List(Of QuickQuoteBuildingPersonalProperty))
                _BuildingPersonalProperties = value
                SetParentOfListItems(_BuildingPersonalProperties, "{09860584-30E8-475E-A428-409826D39D29}")
            End Set
        End Property
        Public Property TotalPersonalPropertyNormalQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_TotalPersonalPropertyNormalQuotedPremium)
            End Get
            Set(value As String)
                _TotalPersonalPropertyNormalQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_TotalPersonalPropertyNormalQuotedPremium)
            End Set
        End Property
        Public Property TotalPersonalPropertyOfOthersQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_TotalPersonalPropertyOfOthersQuotedPremium)
            End Get
            Set(value As String)
                _TotalPersonalPropertyOfOthersQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_TotalPersonalPropertyOfOthersQuotedPremium)
            End Set
        End Property
        Public Property TotalPersonalPropertyCombinedQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_TotalPersonalPropertyCombinedQuotedPremium)
            End Get
            Set(value As String)
                _TotalPersonalPropertyCombinedQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_TotalPersonalPropertyCombinedQuotedPremium)
            End Set
        End Property
        Public Property TotalPersonalPropertyNormalLimit As String
            Get
                Return _TotalPersonalPropertyNormalLimit
            End Get
            Set(value As String)
                _TotalPersonalPropertyNormalLimit = value
                qqHelper.ConvertToLimitFormat(_TotalPersonalPropertyNormalLimit)
            End Set
        End Property
        Public Property TotalPersonalPropertyOfOthersLimit As String
            Get
                Return _TotalPersonalPropertyOfOthersLimit
            End Get
            Set(value As String)
                _TotalPersonalPropertyOfOthersLimit = value
                qqHelper.ConvertToLimitFormat(_TotalPersonalPropertyOfOthersLimit)
            End Set
        End Property
        Public Property TotalPersonalPropertyNormalEarthquakeQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_TotalPersonalPropertyNormalEarthquakeQuotedPremium)
            End Get
            Set(value As String)
                _TotalPersonalPropertyNormalEarthquakeQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_TotalPersonalPropertyNormalEarthquakeQuotedPremium)
            End Set
        End Property
        Public Property TotalPersonalPropertyOfOthersEarthquakeQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_TotalPersonalPropertyOfOthersEarthquakeQuotedPremium)
            End Get
            Set(value As String)
                _TotalPersonalPropertyOfOthersEarthquakeQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_TotalPersonalPropertyOfOthersEarthquakeQuotedPremium)
            End Set
        End Property
        Public Property TotalPersonalPropertyCombinedEarthquakeQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_TotalPersonalPropertyCombinedEarthquakeQuotedPremium)
            End Get
            Set(value As String)
                _TotalPersonalPropertyCombinedEarthquakeQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_TotalPersonalPropertyCombinedEarthquakeQuotedPremium)
            End Set
        End Property
        Public Property TotalPersonalPropertyNormalWithEarthquakeQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_TotalPersonalPropertyNormalWithEarthquakeQuotedPremium)
            End Get
            Set(value As String)
                _TotalPersonalPropertyNormalWithEarthquakeQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_TotalPersonalPropertyNormalWithEarthquakeQuotedPremium)
            End Set
        End Property
        Public Property TotalPersonalPropertyOfOthersWithEarthquakeQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_TotalPersonalPropertyOfOthersWithEarthquakeQuotedPremium)
            End Get
            Set(value As String)
                _TotalPersonalPropertyOfOthersWithEarthquakeQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_TotalPersonalPropertyOfOthersWithEarthquakeQuotedPremium)
            End Set
        End Property
        Public Property TotalPersonalPropertyCombinedWithEarthquakeQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_TotalPersonalPropertyCombinedWithEarthquakeQuotedPremium)
            End Get
            Set(value As String)
                _TotalPersonalPropertyCombinedWithEarthquakeQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_TotalPersonalPropertyCombinedWithEarthquakeQuotedPremium)
            End Set
        End Property
        Public Property TotalPersonalPropertyNormalCount As Integer
            Get
                Return _TotalPersonalPropertyNormalCount
            End Get
            Set(value As Integer)
                _TotalPersonalPropertyNormalCount = value
            End Set
        End Property
        Public Property TotalPersonalPropertyOfOthersCount As Integer
            Get
                Return _TotalPersonalPropertyOfOthersCount
            End Get
            Set(value As Integer)
                _TotalPersonalPropertyOfOthersCount = value
            End Set
        End Property

        'added 10/26/2018
        Public Property HasWindHailACVSettlement As Boolean 'covCodeId 20040; BOP IN/IL
            Get
                Return _HasWindHailACVSettlement
            End Get
            Set(value As Boolean)
                _HasWindHailACVSettlement = value
            End Set
        End Property
        Public Property WindHailACVSettlementQuotedPremium As String 'covCodeId 20040; BOP IN/IL
            Get
                Return qqHelper.QuotedPremiumFormat(_WindHailACVSettlementQuotedPremium)
            End Get
            Set(value As String)
                _WindHailACVSettlementQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_WindHailACVSettlementQuotedPremium)
            End Set
        End Property
        Public Property HasLimitationsOnRoofSurfacing As Boolean 'covCodeId 80542; BOP IL only
            Get
                Return _HasLimitationsOnRoofSurfacing
            End Get
            Set(value As Boolean)
                _HasLimitationsOnRoofSurfacing = value
            End Set
        End Property
        Public Property LimitationsOnRoofSurfacingQuotedPremium As String 'covCodeId 80542; BOP IL only
            Get
                Return qqHelper.QuotedPremiumFormat(_LimitationsOnRoofSurfacingQuotedPremium)
            End Get
            Set(value As String)
                _LimitationsOnRoofSurfacingQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_LimitationsOnRoofSurfacingQuotedPremium)
            End Set
        End Property
        Public Property HasACVRoofSurfacing As Boolean 'covCodeId 80543; BOP IL only
            Get
                Return _HasACVRoofSurfacing
            End Get
            Set(value As Boolean)
                _HasACVRoofSurfacing = value
            End Set
        End Property
        Public Property ACVRoofSurfacingQuotedPremium As String 'covCodeId 80543; BOP IL only
            Get
                Return qqHelper.QuotedPremiumFormat(_ACVRoofSurfacingQuotedPremium)
            End Get
            Set(value As String)
                _ACVRoofSurfacingQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_ACVRoofSurfacingQuotedPremium)
            End Set
        End Property
        Public Property ExcludeCosmeticDamage As Boolean 'covCodeId 80544; BOP IL only
            Get
                Return _ExcludeCosmeticDamage
            End Get
            Set(value As Boolean)
                _ExcludeCosmeticDamage = value
            End Set
        End Property
        Public Property ExcludeCosmeticDamageQuotedPremium As String 'covCodeId 80544; BOP IL only
            Get
                Return qqHelper.QuotedPremiumFormat(_ExcludeCosmeticDamageQuotedPremium)
            End Get
            Set(value As String)
                _ExcludeCosmeticDamageQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_ExcludeCosmeticDamageQuotedPremium)
            End Set
        End Property

        Public Property DetailStatusCode As String 'added 5/15/2019
            Get
                Return _DetailStatusCode
            End Get
            Set(value As String)
                _DetailStatusCode = value
            End Set
        End Property

        Public Property NumberOfUnitsPerBuilding As String 'added 8/25/2020 - DJG - Added for Ohio BOP - Specifically for Class Code 69145
            Get
                Return _NumberOfUnitsPerBuilding
            End Get
            Set(value As String)
                _NumberOfUnitsPerBuilding = value
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 100010</remarks>
        Public Property EarthquakeDeductibleId As String
            Get
                Return _EarthquakeDeductibleId
            End Get
            Set(value As String)
                _EarthquakeDeductibleId = value
                _EarthquakeDeductible = ""
                If IsNumeric(_EarthquakeDeductibleId) = True Then
                    Select Case _EarthquakeDeductibleId
                        Case "34"
                            _EarthquakeDeductible = "5%"
                        Case "36"
                            _EarthquakeDeductible = "10%"
                        Case "37"
                            _EarthquakeDeductible = "15%"
                        Case "38"
                            _EarthquakeDeductible = "20%"
                        Case "39"
                            _EarthquakeDeductible = "25%"
                        Case "48"
                            _EarthquakeDeductible = "30%"
                        Case "49"
                            _EarthquakeDeductible = "35%"
                        Case "50"
                            _EarthquakeDeductible = "40%"
                        Case Else
                            _EarthquakeDeductible = ""
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 100010</remarks>
        Public Property EarthquakeDeductible As String
            Get
                Return _EarthquakeDeductible
            End Get
            Set(value As String)
                _EarthquakeDeductible = value
                Select Case _EarthquakeDeductible
                    Case "5%"
                        _EarthquakeDeductibleId = "34"
                    Case "10%"
                        _EarthquakeDeductibleId = "36"
                    Case "15%"
                        _EarthquakeDeductibleId = "37"
                    Case "20%"
                        _EarthquakeDeductibleId = "38"
                    Case "25%"
                        _EarthquakeDeductibleId = "39"
                    Case "30%"
                        _EarthquakeDeductibleId = "48"
                    Case "35%"
                        _EarthquakeDeductibleId = "49"
                    Case "40%"
                        _EarthquakeDeductibleId = "50"
                    Case Else
                        _EarthquakeDeductibleId = ""
                End Select
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21155</remarks>
        Public Property BuildingCov_EarthquakeDeductibleId As String
            Get
                Return _BuildingCov_EarthquakeDeductibleId
            End Get
            Set(value As String)
                _BuildingCov_EarthquakeDeductibleId = value
                _BuildingCov_EarthquakeDeductible = ""
                If IsNumeric(_BuildingCov_EarthquakeDeductibleId) = True Then
                    Select Case _BuildingCov_EarthquakeDeductibleId
                        Case "34"
                            _BuildingCov_EarthquakeDeductible = "5%"
                        Case "36"
                            _BuildingCov_EarthquakeDeductible = "10%"
                        Case "37"
                            _BuildingCov_EarthquakeDeductible = "15%"
                        Case "38"
                            _BuildingCov_EarthquakeDeductible = "20%"
                        Case "39"
                            _BuildingCov_EarthquakeDeductible = "25%"
                        Case "48"
                            _BuildingCov_EarthquakeDeductible = "30%"
                        Case "49"
                            _BuildingCov_EarthquakeDeductible = "35%"
                        Case "50"
                            _BuildingCov_EarthquakeDeductible = "40%"
                        Case Else
                            _BuildingCov_EarthquakeDeductible = ""
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21155</remarks>
        Public Property BuildingCov_EarthquakeDeductible As String
            Get
                Return _BuildingCov_EarthquakeDeductible
            End Get
            Set(value As String)
                _BuildingCov_EarthquakeDeductible = value
                Select Case _BuildingCov_EarthquakeDeductible
                    Case "5%"
                        _BuildingCov_EarthquakeDeductibleId = "34"
                    Case "10%"
                        _BuildingCov_EarthquakeDeductibleId = "36"
                    Case "15%"
                        _BuildingCov_EarthquakeDeductibleId = "37"
                    Case "20%"
                        _BuildingCov_EarthquakeDeductibleId = "38"
                    Case "25%"
                        _BuildingCov_EarthquakeDeductibleId = "39"
                    Case "30%"
                        _BuildingCov_EarthquakeDeductibleId = "48"
                    Case "35%"
                        _BuildingCov_EarthquakeDeductibleId = "49"
                    Case "40%"
                        _BuildingCov_EarthquakeDeductibleId = "50"
                    Case Else
                        _BuildingCov_EarthquakeDeductibleId = ""
                End Select
            End Set
        End Property

        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21163</remarks>
        'Public Property BusinessIncomeCov_EarthquakeDeductibleId As String
        '    Get
        '        Return _BusinessIncomeCov_EarthquakeDeductibleId
        '    End Get
        '    Set(value As String)
        '        _BusinessIncomeCov_EarthquakeDeductibleId = value
        '        _BusinessIncomeCov_EarthquakeDeductible = ""
        '        If IsNumeric(_BusinessIncomeCov_EarthquakeDeductibleId) = True Then
        '            Select Case _BusinessIncomeCov_EarthquakeDeductibleId
        '                Case "34"
        '                    _BusinessIncomeCov_EarthquakeDeductible = "5%"
        '                Case "36"
        '                    _BusinessIncomeCov_EarthquakeDeductible = "10%"
        '                Case "37"
        '                    _BusinessIncomeCov_EarthquakeDeductible = "15%"
        '                Case "38"
        '                    _BusinessIncomeCov_EarthquakeDeductible = "20%"
        '                Case "39"
        '                    _BusinessIncomeCov_EarthquakeDeductible = "25%"
        '                Case "48"
        '                    _BusinessIncomeCov_EarthquakeDeductible = "30%"
        '                Case "49"
        '                    _BusinessIncomeCov_EarthquakeDeductible = "35%"
        '                Case "50"
        '                    _BusinessIncomeCov_EarthquakeDeductible = "40%"
        '                Case Else
        '                    _BusinessIncomeCov_EarthquakeDeductible = ""
        '            End Select
        '        End If
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21155</remarks>
        'Public Property BusinessIncomeCov_EarthquakeDeductible As String
        '    Get
        '        Return _BusinessIncomeCov_EarthquakeDeductible
        '    End Get
        '    Set(value As String)
        '        _BusinessIncomeCov_EarthquakeDeductible = value
        '        Select Case _BusinessIncomeCov_EarthquakeDeductible
        '            Case "5%"
        '                _BusinessIncomeCov_EarthquakeDeductibleId = "34"
        '            Case "10%"
        '                _BusinessIncomeCov_EarthquakeDeductibleId = "36"
        '            Case "15%"
        '                _BusinessIncomeCov_EarthquakeDeductibleId = "37"
        '            Case "20%"
        '                _BusinessIncomeCov_EarthquakeDeductibleId = "38"
        '            Case "25%"
        '                _BusinessIncomeCov_EarthquakeDeductibleId = "39"
        '            Case "30%"
        '                _BusinessIncomeCov_EarthquakeDeductibleId = "48"
        '            Case "35%"
        '                _BusinessIncomeCov_EarthquakeDeductibleId = "49"
        '            Case "40%"
        '                _BusinessIncomeCov_EarthquakeDeductibleId = "50"
        '            Case Else
        '                _BusinessIncomeCov_EarthquakeDeductibleId = ""
        '        End Select
        '    End Set
        'End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21160</remarks>
        Public Property PersPropCov_EarthquakeDeductibleId As String
            Get
                Return _PersPropCov_EarthquakeDeductibleId
            End Get
            Set(value As String)
                _PersPropCov_EarthquakeDeductibleId = value
                _PersPropCov_EarthquakeDeductible = ""
                If IsNumeric(_PersPropCov_EarthquakeDeductibleId) = True Then
                    Select Case _PersPropCov_EarthquakeDeductibleId
                        Case "34"
                            _PersPropCov_EarthquakeDeductible = "5%"
                        Case "36"
                            _PersPropCov_EarthquakeDeductible = "10%"
                        Case "37"
                            _PersPropCov_EarthquakeDeductible = "15%"
                        Case "38"
                            _PersPropCov_EarthquakeDeductible = "20%"
                        Case "39"
                            _PersPropCov_EarthquakeDeductible = "25%"
                        Case "48"
                            _PersPropCov_EarthquakeDeductible = "30%"
                        Case "49"
                            _PersPropCov_EarthquakeDeductible = "35%"
                        Case "50"
                            _PersPropCov_EarthquakeDeductible = "40%"
                        Case Else
                            _PersPropCov_EarthquakeDeductible = ""
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21160</remarks>
        Public Property PersPropCov_EarthquakeDeductible As String
            Get
                Return _PersPropCov_EarthquakeDeductible
            End Get
            Set(value As String)
                _PersPropCov_EarthquakeDeductible = value
                Select Case _PersPropCov_EarthquakeDeductible
                    Case "5%"
                        _PersPropCov_EarthquakeDeductibleId = "34"
                    Case "10%"
                        _PersPropCov_EarthquakeDeductibleId = "36"
                    Case "15%"
                        _PersPropCov_EarthquakeDeductibleId = "37"
                    Case "20%"
                        _PersPropCov_EarthquakeDeductibleId = "38"
                    Case "25%"
                        _PersPropCov_EarthquakeDeductibleId = "39"
                    Case "30%"
                        _PersPropCov_EarthquakeDeductibleId = "48"
                    Case "35%"
                        _PersPropCov_EarthquakeDeductibleId = "49"
                    Case "40%"
                        _PersPropCov_EarthquakeDeductibleId = "50"
                    Case Else
                        _PersPropCov_EarthquakeDeductibleId = ""
                End Select
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21160</remarks>
        Public Property PersPropOfOthers_EarthquakeDeductibleId As String
            Get
                Return _PersPropOfOthers_EarthquakeDeductibleId
            End Get
            Set(value As String)
                _PersPropOfOthers_EarthquakeDeductibleId = value
                _PersPropOfOthers_EarthquakeDeductible = ""
                If IsNumeric(_PersPropOfOthers_EarthquakeDeductibleId) = True Then
                    Select Case _PersPropOfOthers_EarthquakeDeductibleId
                        Case "34"
                            _PersPropOfOthers_EarthquakeDeductible = "5%"
                        Case "36"
                            _PersPropOfOthers_EarthquakeDeductible = "10%"
                        Case "37"
                            _PersPropOfOthers_EarthquakeDeductible = "15%"
                        Case "38"
                            _PersPropOfOthers_EarthquakeDeductible = "20%"
                        Case "39"
                            _PersPropOfOthers_EarthquakeDeductible = "25%"
                        Case "48"
                            _PersPropOfOthers_EarthquakeDeductible = "30%"
                        Case "49"
                            _PersPropOfOthers_EarthquakeDeductible = "35%"
                        Case "50"
                            _PersPropOfOthers_EarthquakeDeductible = "40%"
                        Case Else
                            _PersPropOfOthers_EarthquakeDeductible = ""
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21160</remarks>
        Public Property PersPropOfOthers_EarthquakeDeductible As String
            Get
                Return _PersPropOfOthers_EarthquakeDeductible
            End Get
            Set(value As String)
                _PersPropOfOthers_EarthquakeDeductible = value
                Select Case _PersPropOfOthers_EarthquakeDeductible
                    Case "5%"
                        _PersPropOfOthers_EarthquakeDeductibleId = "34"
                    Case "10%"
                        _PersPropOfOthers_EarthquakeDeductibleId = "36"
                    Case "15%"
                        _PersPropOfOthers_EarthquakeDeductibleId = "37"
                    Case "20%"
                        _PersPropOfOthers_EarthquakeDeductibleId = "38"
                    Case "25%"
                        _PersPropOfOthers_EarthquakeDeductibleId = "39"
                    Case "30%"
                        _PersPropOfOthers_EarthquakeDeductibleId = "48"
                    Case "35%"
                        _PersPropOfOthers_EarthquakeDeductibleId = "49"
                    Case "40%"
                        _PersPropOfOthers_EarthquakeDeductibleId = "50"
                    Case Else
                        _PersPropOfOthers_EarthquakeDeductibleId = ""
                End Select
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090</remarks>
        Public Property PersPropCov_InflationGuardTypeId As String
            Get
                Return _PersPropCov_InflationGuardTypeId
            End Get
            Set(value As String)
                _PersPropCov_InflationGuardTypeId = value
                _PersPropCov_InflationGuardType = ""
                If IsNumeric(_PersPropCov_InflationGuardTypeId) = True Then
                    _PersPropCov_InflationGuardType = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuoteHelperClass.QuickQuotePropertyName.InflationGuardTypeId, _PersPropCov_InflationGuardType)
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090</remarks>
        Public Property PersPropCov_InflationGuardType As String
            Get
                Return _PersPropCov_InflationGuardType
            End Get
            Set(value As String)
                _PersPropCov_InflationGuardType = value
                Select Case _PersPropCov_InflationGuardType
                    Case "N/A"
                        _PersPropCov_InflationGuardTypeId = "0"
                    Case "2"
                        _PersPropCov_InflationGuardTypeId = "1"
                    Case "4"
                        _PersPropCov_InflationGuardTypeId = "2"
                    Case "6"
                        _PersPropCov_InflationGuardTypeId = "3"
                    Case "8"
                        _PersPropCov_InflationGuardTypeId = "4"
                    Case "10"
                        _PersPropCov_InflationGuardTypeId = "5"
                    Case "12"
                        _PersPropCov_InflationGuardTypeId = "6"
                    Case Else
                        _PersPropCov_InflationGuardTypeId = ""
                End Select
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090</remarks>
        Public Property PersPropOfOthers_InflationGuardTypeId As String
            Get
                Return _PersPropOfOthers_InflationGuardTypeId
            End Get
            Set(value As String)
                _PersPropOfOthers_InflationGuardTypeId = value
                _PersPropOfOthers_InflationGuardType = ""
                If IsNumeric(_PersPropOfOthers_InflationGuardTypeId) = True Then
                    _PersPropOfOthers_InflationGuardType = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuoteHelperClass.QuickQuotePropertyName.InflationGuardTypeId, _PersPropOfOthers_InflationGuardType)
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090</remarks>
        Public Property PersPropOfOthers_InflationGuardType As String
            Get
                Return _PersPropOfOthers_InflationGuardType
            End Get
            Set(value As String)
                _PersPropOfOthers_InflationGuardType = value
                Select Case _PersPropOfOthers_InflationGuardType
                    Case "N/A"
                        _PersPropOfOthers_InflationGuardTypeId = "0"
                    Case "2"
                        _PersPropOfOthers_InflationGuardTypeId = "1"
                    Case "4"
                        _PersPropOfOthers_InflationGuardTypeId = "2"
                    Case "6"
                        _PersPropOfOthers_InflationGuardTypeId = "3"
                    Case "8"
                        _PersPropOfOthers_InflationGuardTypeId = "4"
                    Case "10"
                        _PersPropOfOthers_InflationGuardTypeId = "5"
                    Case "12"
                        _PersPropOfOthers_InflationGuardTypeId = "6"
                    Case Else
                        _PersPropOfOthers_InflationGuardTypeId = ""
                End Select
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 165</remarks>
        Public Property OwnerOccupiedPercentage As String
            Get
                Return _OwnerOccupiedPercentage
            End Get
            Set(value As String)
                _OwnerOccupiedPercentage = value
                Select Case _OwnerOccupiedPercentage
                    Case "None"
                        _OwnerOccupiedPercentageId = "30"
                    Case "1% - 10% owner occupied"
                        _OwnerOccupiedPercentageId = "31"
                    Case "11% - 100% owner occupied"
                        _OwnerOccupiedPercentageId = "32"
                    Case Else
                        _OwnerOccupiedPercentageId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' specific to building coverage (coveragecode_id 165)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>coveragecode_id 165</remarks>
        Public Property OwnerOccupiedPercentageId As String
            Get
                Return _OwnerOccupiedPercentageId
            End Get
            Set(value As String)
                _OwnerOccupiedPercentageId = value
                '(30=None; 31=1% - 10% owner occupied; 32=11% - 100% owner occupied)
                _OwnerOccupiedPercentage = ""
                If IsNumeric(_OwnerOccupiedPercentageId) = True Then
                    Select Case _OwnerOccupiedPercentageId
                        Case "-1"
                            _OwnerOccupiedPercentage = ""
                        Case "30"
                            _OwnerOccupiedPercentage = "None"
                        Case "31"
                            _OwnerOccupiedPercentage = "1% - 10% owner occupied"
                        Case "32"
                            _OwnerOccupiedPercentage = "11% - 100% owner occupied"
                    End Select
                End If
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _effectiveDate = "" '3/9/2017 - BOP stuff
            _PolicyId = ""
            _PolicyImageNum = ""
            _Description = ""
            _Program = ""
            _ProgramAbbreviation = ""
            _Classification = ""
            _ClassCode = ""
            _ClassificationTypeId = ""
            _Occupancy = ""
            _OccupancyId = ""
            _Construction = ""
            _ConstructionId = ""
            _AutoIncrease = ""
            _AutoIncreaseId = ""
            _AutoIncreasePremium = "" '3/9/2017 - BOP stuff
            _HasAutoIncreasePremium = False '3/9/2017 - BOP stuff
            _PropertyDeductible = ""
            _PropertyDeductibleId = ""
            _Limit = ""
            _LimitQuotedPremium = ""
            _Valuation = ""
            _ValuationId = ""
            _IsBuildingValIncludedInBlanketRating = False
            _IsAgreedValue = False ' added 2/1/2016 for bug 4845 Matt A; 3/9/2017 - included in this library w/ BOP updates
            _HasACVRoofing = False '3/9/2017 - BOP stuff
            _ACVRoofingQuotedPremium = "" '3/9/2017 - BOP stuff
            _HasMineSubsidence = False
            _MineSubsidenceQuotedPremium = ""
            _MineSubsidence_IsDwellingStructure = False 'added 11/8/2018 for CPR mine subsidence; covCodeId 20027; CheckBox: Mine Subsidence (BOP IN/IL, CPR IN/IL)
            _HasSprinklered = False
            _PersonalPropertyLimit = ""
            _PersonalPropertyLimitQuotedPremium = ""
            _ValuationMethod = ""
            _ValuationMethodId = ""
            _IsValMethodIncludedInBlanketRating = False

            _AccountsReceivableOnPremises = ""
            _AccountsReceivableOffPremises = ""
            _AccountsReceivableQuotedPremium = ""
            _ValuablePapersOnPremises = ""
            _ValuablePapersOffPremises = ""
            _ValuablePapersQuotedPremium = ""
            _CondoCommercialUnitOwnersLimit = ""
            _CondoCommercialUnitOwnersLimitId = ""
            _CondoCommercialUnitOwnersLimitQuotedPremium = ""
            _HasOrdinanceOrLaw = False
            _HasOrdOrLawUndamagedPortion = False
            _OrdOrLawUndamagedPortionQuotedPremium = ""
            _OrdOrLawDemoCostLimit = ""
            _OrdOrLawDemoCostLimitQuotedPremium = ""
            _OrdOrLawIncreasedCostLimit = ""
            _OrdOrLawIncreaseCostLimitQuotedPremium = ""
            _OrdOrLawDemoAndIncreasedCostLimit = ""
            _OrdOrLawDemoAndIncreasedCostLimitQuotedPremium = ""
            _HasSpoilage = False
            _SpoilageQuotedPremium = ""
            _SpoilagePropertyClassification = ""
            _SpoilagePropertyClassificationId = ""
            _SpoilageTotalLimit = ""
            _IsSpoilageRefrigerationMaintenanceAgreement = False
            _IsSpoilageBreakdownOrContamination = False
            _IsSpoilagePowerOutage = False

            '_BuildingCoverages = New Generic.List(Of QuickQuoteCoverage)
            _BuildingCoverages = Nothing 'added 8/4/2014

            '_BuildingClassifications = New Generic.List(Of QuickQuoteClassification)
            _BuildingClassifications = Nothing 'added 8/4/2014

            _HasBusinessMasterEnhancement = False

            _ProtectionClassId = ""
            _ProtectionClass = ""

            _AccountsReceivableOnPremisesExcessLimit = ""
            _ValuablePapersOnPremisesExcessLimit = ""

            _YearBuilt = ""

            '_NumberOfSoleProprietors = ""
            '_NumberOfCorporateOfficers = ""
            '_NumberOfPartners = ""
            _NumberOfOfficersAndPartnersAndInsureds = ""
            _EmployeePayroll = ""
            _AnnualReceipts = ""

            _SquareFeet = ""

            '_CentralHeatElectric = False
            '_CentralHeatGas = False
            '_CentralHeatOil = False
            '_CentralHeatOther = False
            '_CentralHeatOtherDescription = ""
            '_CentralHeatUpdateTypeId = ""
            '_CentralHeatUpdateYear = ""
            '_ImprovementsDescription = ""
            '_Electric100Amp = False
            '_Electric120Amp = False
            '_Electric200Amp = False
            '_Electric60Amp = False
            '_ElectricBurningUnit = False
            '_ElectricCircuitBreaker = False
            '_ElectricFuses = False
            '_ElectricSpaceHeater = False
            '_ElectricUpdateTypeId = ""
            '_ElectricUpdateYear = ""
            '_PlumbingCopper = False
            '_PlumbingGalvanized = False
            '_PlumbingPlastic = False
            '_PlumbingUpdateTypeId = ""
            '_PlumbingUpdateYear = ""
            '_RoofAsphaltShingle = False
            '_RoofMetal = False
            '_RoofOther = False
            '_RoofOtherDescription = ""
            '_RoofSlate = False
            '_RoofUpdateTypeId = ""
            '_RoofUpdateYear = ""
            '_RoofWood = False
            '_SupplementalHeatBurningUnit = False
            '_SupplementalHeatFireplace = False
            '_SupplementalHeatFireplaceInsert = False
            '_SupplementalHeatNA = False
            '_SupplementalHeatSolidFuel = False
            '_SupplementalHeatSpaceHeater = False
            '_SupplementalHeatUpdateTypeId = ""
            '_SupplementalHeatUpdateYear = ""
            '_WindowsUpdateTypeId = ""
            '_WindowsUpdateYear = ""
            _Updates = New QuickQuoteUpdatesRecord 'added 7/31/2013

            '_AdditionalInterests = New Generic.List(Of QuickQuoteAdditionalInterest)
            _AdditionalInterests = Nothing 'added 8/4/2014

            _HasBarbersProfessionalLiability = False
            _BarbersProfessionalLiabilityFullTimeEmpNum = ""
            _BarbersProfessionalLiabilityPartTimeEmpNum = ""
            _HasBeauticiansProfessionalLiability = False
            _BeauticiansProfessionalLiabilityFullTimeEmpNum = ""
            _BeauticiansProfessionalLiabilityPartTimeEmpNum = ""
            _HasFuneralDirectorsProfessionalLiability = False
            _FuneralDirectorsProfessionalLiabilityEmpNum = ""
            _HasPrintersProfessionalLiability = False
            _PrintersProfessionalLiabilityLocNum = ""
            _HasSelfStorageFacility = False
            _SelfStorageFacilityLimit = ""
            _HasVeterinariansProfessionalLiability = False
            _VeterinariansProfessionalLiabilityEmpNum = ""
            _HasOpticalAndHearingAidProfessionalLiability = False
            _OpticalAndHearingAidProfessionalLiabilityEmpNum = ""

            '3/9/2017 - BOP stuff
            _HasMotelCoverage = False
            _MotelCoveragePerGuestLimitId = ""
            _MotelCoverageSafeDepositDeductibleId = ""
            _MotelCoverageSafeDepositLimitId = ""
            _HasPharmacistProfessionalLiability = False
            _PharmacistAnnualGrossSales = ""
            _HasApartmentBuildings = False
            _NumberOfLocationsWithApartments = ""
            _HasTenantAutoLegalLiability = False
            _TenantAutoLegalLiabilityDeductibleId = ""
            _TenantAutoLegalLiabilityLimitOfLiabilityId = ""
            _HasCustomerAutoLegalLiability = False
            _CustomerAutoLegalLiabilityDeductibleId = ""
            _CustomerAutoLegalLiabilityLimitOfLiabilityId = ""
            _LiquorLiabilityAggregateLimit = ""
            _LiquorLiabilityAnnualGrossAlcoholSalesReceipts = ""
            _LiquorLiabilityAnnualGrossPackageSalesReceipts = ""
            _LiquorLiabilityClassCodeTypeId = ""
            _HasLiquorLiability = False
            _HasFineArts = False
            _HasPhotographyMakeupAndHair = False
            _HasPhotographyCoverage = False
            _HasPhotographyCoverageScheduledCoverages = False
            _PhotographyScheduledCoverages = Nothing
            _HasRestaurantEndorsement = False

            _ClassificationCode = New QuickQuoteClassificationCode
            _EarthquakeBuildingClassificationTypeId = ""

            _EarthquakeApplies = False
            _CauseOfLossTypeId = ""
            _CauseOfLossType = ""
            _DeductibleId = ""
            _Deductible = ""
            _CoinsuranceTypeId = ""
            _CoinsuranceType = ""
            '_ValuationMethodTypeId = ""
            '_ValuationMethodType = ""
            _RatingTypeId = ""
            _RatingType = ""
            _InflationGuardTypeId = ""
            _InflationGuardType = ""

            '_ScheduledCoverages = New Generic.List(Of QuickQuoteScheduledCoverage)
            _ScheduledCoverages = Nothing 'added 8/4/2014
            _PersPropCov_PersonalPropertyLimit = ""
            _PersPropCov_PropertyTypeId = ""
            _PersPropCov_PropertyType = ""
            _PersPropCov_RiskTypeId = ""
            _PersPropCov_RiskType = ""
            _PersPropCov_EarthquakeApplies = False
            _PersPropCov_IsAgreedValue = False ' added 2/1/2016 for bug 4845 Matt A; 3/9/2017 - included in this library w/ BOP updates
            _PersPropCov_RatingTypeId = ""
            _PersPropCov_RatingType = ""
            _PersPropCov_CauseOfLossTypeId = ""
            _PersPropCov_CauseOfLossType = ""
            _PersPropCov_DeductibleId = ""
            _PersPropCov_Deductible = ""
            _PersPropCov_CoinsuranceTypeId = ""
            _PersPropCov_CoinsuranceType = ""
            _PersPropCov_ValuationId = ""
            _PersPropCov_Valuation = ""
            _PersPropCov_QuotedPremium = ""
            _PersPropCov_ClassificationCode = New QuickQuoteClassificationCode
            _PersPropOfOthers_PersonalPropertyLimit = ""
            '_PersPropOfOthers_PropertyTypeId = ""'defaulting
            '_PersPropOfOthers_PropertyType = ""
            _PersPropOfOthers_RiskTypeId = ""
            _PersPropOfOthers_RiskType = ""
            _PersPropOfOthers_EarthquakeApplies = False
            _PersPropOfOthers_RatingTypeId = ""
            _PersPropOfOthers_RatingType = ""
            _PersPropOfOthers_CauseOfLossTypeId = ""
            _PersPropOfOthers_CauseOfLossType = ""
            _PersPropOfOthers_DeductibleId = ""
            _PersPropOfOthers_Deductible = ""
            _PersPropOfOthers_CoinsuranceTypeId = ""
            _PersPropOfOthers_CoinsuranceType = ""
            _PersPropOfOthers_ValuationId = ""
            _PersPropOfOthers_Valuation = ""
            _PersPropOfOthers_QuotedPremium = ""
            _PersPropOfOthers_ClassificationCode = New QuickQuoteClassificationCode

            _BusinessIncomeCov_Limit = ""
            _BusinessIncomeCov_CoinsuranceTypeId = ""
            _BusinessIncomeCov_CoinsuranceType = ""
            _BusinessIncomeCov_MonthlyPeriodTypeId = ""
            _BusinessIncomeCov_MonthlyPeriodType = ""
            _BusinessIncomeCov_BusinessIncomeTypeId = ""
            _BusinessIncomeCov_BusinessIncomeType = ""
            _BusinessIncomeCov_RiskTypeId = ""
            _BusinessIncomeCov_RiskType = ""
            _BusinessIncomeCov_EarthquakeApplies = False
            _BusinessIncomeCov_RatingTypeId = ""
            _BusinessIncomeCov_RatingType = ""
            _BusinessIncomeCov_CauseOfLossTypeId = ""
            _BusinessIncomeCov_CauseOfLossType = ""
            _BusinessIncomeCov_QuotedPremium = ""
            _BusinessIncomeCov_ClassificationCode = New QuickQuoteClassificationCode
            _BusinessIncomeCov_WaitingPeriodTypeId = ""
            _BusinessIncomeCov_WaitingPeriodType = ""

            _Building_BusinessIncome_Group1_Rate = ""
            _Building_BusinessIncome_Group2_Rate = ""
            _PersonalProperty_Group1_Rate = ""
            _PersonalProperty_Group2_Rate = ""
            _Building_BusinessIncome_Group1_LossCost = ""
            _Building_BusinessIncome_Group2_LossCost = ""
            _PersonalProperty_Group1_LossCost = ""
            _PersonalProperty_Group2_LossCost = ""

            _CoverageFormTypeId = ""
            _CoverageFormType = ""

            _CPR_Covs_TotalBuildingPremium = ""

            '_PersPropCov_EarthquakeRateGradeTypeId = ""
            '_PersPropOfOthers_EarthquakeRateGradeTypeId = ""
            _PersonalProperty_EarthquakeRateGradeTypeId = ""

            _NumberOfStories = ""

            _EarthquakeQuotedPremium = ""
            _PersPropCov_EarthquakeQuotedPremium = ""
            _PersPropOfOthers_EarthquakeQuotedPremium = ""
            _BusinessIncomeCov_EarthquakeQuotedPremium = ""
            _CPR_Covs_TotalBuilding_EQ_Premium = ""
            _CPR_Covs_TotalBuilding_With_EQ_Premium = ""
            _CPR_BuildingLimit_With_EQ_QuotedPremium = ""
            _CPR_PersPropCov_With_EQ_QuotedPremium = ""
            _CPR_PersPropOfOthers_With_EQ_QuotedPremium = ""
            _CPR_BusinessIncomeCov_With_EQ_QuotedPremium = ""
            _CPR_BuildingLimit_EarthquakeBuildingClassificationPercentage = ""
            _CPR_PersPropCov_EarthquakeBuildingClassificationPercentage = ""
            _CPR_PersPropOfOthers_EarthquakeBuildingClassificationPercentage = ""
            _CPR_BusinessIncomeCov_EarthquakeBuildingClassificationPercentage = ""
            _OptionalTheftDeductibleId = ""
            _OptionalTheftDeductible = ""
            _OptionalWindstormOrHailDeductibleId = ""
            _OptionalWindstormOrHailDeductible = ""
            _PersPropCov_OptionalTheftDeductibleId = ""
            _PersPropCov_OptionalTheftDeductible = ""
            _PersPropCov_OptionalWindstormOrHailDeductibleId = ""
            _PersPropCov_OptionalWindstormOrHailDeductible = ""
            _PersPropOfOthers_OptionalTheftDeductibleId = ""
            _PersPropOfOthers_OptionalTheftDeductible = ""
            _PersPropOfOthers_OptionalWindstormOrHailDeductibleId = ""
            _PersPropOfOthers_OptionalWindstormOrHailDeductible = ""
            _PersPropCov_DoesYardRateApplyTypeId = ""
            _PersPropOfOthers_DoesYardRateApplyTypeId = ""

            _FeetToFireHydrant = ""
            _MilesToFireDepartment = ""

            _PersPropCov_IncludedInBlanketCoverage = False
            _PersPropOfOthers_IncludedInBlanketCoverage = False
            _BusinessIncomeCov_IncludedInBlanketCoverage = False

            'added 2/18/2014
            _HasConvertedClassifications = False
            _HasConvertedCoverages = False
            _HasConvertedScheduledCoverages = False

            _PremiumFullterm = "" 'added 4/2/2014

            _FarmBarnBuildingNum = "" 'added 4/23/2014 for reconciliation
            _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = False 'added 4/29/2014
            'added 10/18/2018 for multi-state
            _FarmBarnBuildingNum_MasterPart = ""
            _FarmBarnBuildingNum_CGLPart = ""
            _FarmBarnBuildingNum_CPRPart = ""
            _FarmBarnBuildingNum_CIMPart = ""
            _FarmBarnBuildingNum_CRMPart = ""
            _FarmBarnBuildingNum_GARPart = ""

            'added 10/16/2014
            '_Modifiers = New List(Of QuickQuoteModifier)
            _Modifiers = Nothing

            _CanUseScheduledCoverageNumForScheduledCoverageReconciliation = False 'added 1/22/2015
            'added 1/22/2015
            _PersPropCov_ScheduledCoverageNum = ""
            _PersPropOfOthers_ScheduledCoverageNum = ""

            'added 2/9/2014 for CIM
            _ComputerHardwareLimit = "" 'cov also has CoverageBasisTypeId set to 1
            _ComputerHardwareRate = ""
            _ComputerHardwareQuotedPremium = ""
            _ComputerProgramsApplicationsAndMediaLimit = "" 'cov also has CoverageBasisTypeId set to 1
            _ComputerProgramsApplicationsAndMediaRate = ""
            _ComputerProgramsApplicationsAndMediaQuotedPremium = ""
            _ComputerBusinessIncomeLimit = "" 'cov also has CoverageBasisTypeId set to 1
            _ComputerBusinessIncomeRate = ""
            _ComputerBusinessIncomeQuotedPremium = ""
            'added 3/16/2015
            '_FineArtsScheduledItems = New List(Of QuickQuoteFineArtsScheduledItem)
            _FineArtsScheduledItems = Nothing
            'added 3/26/2015
            '_ScheduledSigns = New List(Of QuickQuoteScheduledSign)
            _ScheduledSigns = Nothing
            _UnscheduledSignsLimit = "" 'cov also has CoverageBasisTypeId set to 1
            _UnscheduledSignsQuotedPremium = ""

            'added 6/15/2015 for Farm
            _Dimensions = ""
            _FarmStructureTypeId = "" 'static data
            _FarmTypeId = "" 'static data
            _NumberOfSolidFuelBurningUnits = ""
            _VacancyFromDate = ""
            _VacancyToDate = ""
            _HasConvertedModifiers = False
            _SprinklerSystem_AllExcept = False
            _SprinklerSystem_AllIncluding = False
            _HeatedBuildingSurchargeGasElectric = False
            _HeatedBuildingSurchargeOther = False
            _ExposedInsulationSurcharge = False
            _E_Farm_Limit = ""
            _E_Farm_DeductibleLimitId = "" 'static data
            _E_Farm_QuotedPremium = ""
            'added 6/16/2015 for Farm
            _HouseholdContentsLimit = ""
            _HouseholdContentsQuotedPremium = ""
            'added 6/24/2015 for Farm
            '_OptionalCoverageEs = New List(Of QuickQuoteOptionalCoverageE)
            _OptionalCoverageEs = Nothing
            _CanUseFarmBarnBuildingOptionalCoverageNumForOptionalCoverageEReconciliation = False
            _HasConvertedOptionalCoverageEs = False
            'added 7/28/2015 for Farm e2Value
            _PropertyValuation = New QuickQuotePropertyValuation

            'added 2/17/2017
            '_UseBuildingClassificationPropertiesToCreateOneItemInList = False
            'updated 3/2/2017 to default based on whether or not the compRater services are being used... old functionality (1 classification) for compRater services and new functionality (multiple classifications) for Diamond services
            If QuickQuoteHelperClass.UseComparativeRaterForLob(lob:=QuickQuoteObject.QuickQuoteLobType.CommercialBOP) = helper.QuickQuoteYesNoMaybeType.Yes Then
                _UseBuildingClassificationPropertiesToCreateOneItemInList = True
            Else
                _UseBuildingClassificationPropertiesToCreateOneItemInList = False
            End If
            'added 2/20/2017
            _CanUseClassificationNumForClassificationReconciliation = False

            'added 7/8/2017
            _BuildingPersonalProperties = Nothing
            _TotalPersonalPropertyNormalQuotedPremium = ""
            _TotalPersonalPropertyOfOthersQuotedPremium = ""
            _TotalPersonalPropertyCombinedQuotedPremium = ""
            _TotalPersonalPropertyNormalLimit = ""
            _TotalPersonalPropertyOfOthersLimit = ""
            _TotalPersonalPropertyNormalEarthquakeQuotedPremium = ""
            _TotalPersonalPropertyOfOthersEarthquakeQuotedPremium = ""
            _TotalPersonalPropertyCombinedEarthquakeQuotedPremium = ""
            _TotalPersonalPropertyNormalWithEarthquakeQuotedPremium = ""
            _TotalPersonalPropertyOfOthersWithEarthquakeQuotedPremium = ""
            _TotalPersonalPropertyCombinedWithEarthquakeQuotedPremium = ""
            _TotalPersonalPropertyNormalCount = 0
            _TotalPersonalPropertyOfOthersCount = 0

            'added 10/26/2018
            _HasWindHailACVSettlement = False 'covCodeId 20040; BOP IN/IL
            _WindHailACVSettlementQuotedPremium = "" 'covCodeId 20040; BOP IN/IL
            _HasLimitationsOnRoofSurfacing = False 'covCodeId 80542; BOP IL only
            _LimitationsOnRoofSurfacingQuotedPremium = "" 'covCodeId 80542; BOP IL only
            _HasACVRoofSurfacing = False 'covCodeId 80543; BOP IL only
            _ACVRoofSurfacingQuotedPremium = "" 'covCodeId 80543; BOP IL only
            _ExcludeCosmeticDamage = False 'covCodeId 80544; BOP IL only
            _ExcludeCosmeticDamageQuotedPremium = "" 'covCodeId 80544; BOP IL only

            _DetailStatusCode = "" 'added 5/15/2019

            _EarthquakeDeductibleId = ""
            _EarthquakeDeductible = ""
            _BuildingCov_EarthquakeDeductibleId = ""
            _BuildingCov_EarthquakeDeductible = ""
            '_BusinessIncomeCov_EarthquakeDeductibleId = ""
            _BusinessIncomeCov_EarthquakeDeductible = ""
            _PersPropCov_EarthquakeDeductibleId = ""
            _PersPropCov_EarthquakeDeductible = ""
            _PersPropOfOthers_EarthquakeDeductibleId = ""
            _PersPropOfOthers_EarthquakeDeductible = ""
            _PersPropCov_InflationGuardTypeId = ""
            _PersPropCov_InflationGuardType = ""
            _PersPropOfOthers_InflationGuardTypeId = ""
            _PersPropOfOthers_InflationGuardType = ""
            _OwnerOccupiedPercentageId = ""
            _OwnerOccupiedPercentage = ""

        End Sub

        ''' <summary>
        ''' used to parse thru coverages and set different properties
        ''' </summary>
        ''' <remarks>always executed when xml is parsed; doesn't specifically needed to be called by developer code</remarks>
        Public Sub ParseThruCoverages() 'added 4/8/2015
            ParseThruCoverages(_BuildingCoverages)
        End Sub
        ''' <summary>
        ''' used to parse thru coverages and set different properties
        ''' </summary>
        ''' <remarks>always executed when xml is parsed; doesn't specifically needed to be called by developer code</remarks>
        ''' 'Public Sub ParseThruCoverages()
        Public Sub ParseThruCoverages(ByVal covs As List(Of QuickQuoteCoverage), Optional ByVal packagePartType As QuickQuoteXML.QuickQuotePackagePartType = QuickQuoteXML.QuickQuotePackagePartType.None) 'added new params 4/8/2015 for CPP
            'If _BuildingCoverages IsNot Nothing AndAlso _BuildingCoverages.Count > 0 Then
            'updated 4/8/2015 to use param
            If covs IsNot Nothing AndAlso covs.Count > 0 Then
                'For Each cov As QuickQuoteCoverage In _BuildingCoverages
                'updated 4/8/2015 to use param
                For Each cov As QuickQuoteCoverage In covs
                    Select Case cov.CoverageCodeId
                        Case "80400" '3/9/2017 - BOP stuff
                            _HasAutoIncreasePremium = cov.Checkbox
                            _AutoIncreasePremium = cov.FullTermPremium
                        Case "80395" '3/9/2017 - BOP stuff
                            _HasACVRoofing = cov.Checkbox
                            ACVRoofingQuotedPremium = cov.FullTermPremium
                        Case "165" 'Edit; 3/20/2013 note: Building
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty Then 'added IF 4/10/2015; may not be needed
                                AutoIncreaseId = cov.AutomaticIncreasePercentTypeId
                                Limit = cov.ManualLimitAmount
                                LimitQuotedPremium = cov.FullTermPremium
                                ValuationId = cov.ValuationMethodTypeId
                                OwnerOccupiedPercentageId = cov.RiskTypeId
                                _IsBuildingValIncludedInBlanketRating = cov.IsIncludedInBlanketRating
                                _IsAgreedValue = cov.IsAgreedValue ' added 2/1/2016 bug 4845 Matt A; 3/9/2017 - included in this library w/ BOP updates
                                If _HasMineSubsidence = False Then 'added IF 11/8/2018 since covCodeId 20027 will now be the driving force behind this premium; 11/9/2018 note: this appears to work for BOP w/ old logic (on single-state IN before multi-state); we apparently never set mineSub for CPR, which would require 20027
                                    _HasMineSubsidence = cov.IsMineSubsidence
                                End If
                                If _HasMineSubsidence = True AndAlso cov.CoverageAdditionalInfoRecords IsNot Nothing AndAlso cov.CoverageAdditionalInfoRecords.Count > 0 Then
                                    '_MineSubsidenceQuotedPremium = cov.FullTermPremium
                                    'updated 6/15/2012 per IS (and verified 6/27/2012)
                                    For Each add As QuickQuoteCoverageAdditionalInfoRecord In cov.CoverageAdditionalInfoRecords
                                        If UCase(add.Description) = "MINE SUBSIDENCE PREMIUM" Then
                                            If qqHelper.IsPositiveDecimalString(MineSubsidenceQuotedPremium) = False Then 'added IF 11/8/2018 since covCodeId 20027 will now be the driving force behind this premium; 11/9/2018 note: even when flag is set on building cov for mineSub w/ old logic, premium appears to come back in covCodeId 20027... also, CoverageAdditionalInfoRecord for premium only shows up on that cov and not this one
                                                MineSubsidenceQuotedPremium = add.Value
                                            End If
                                            Exit For
                                        End If
                                    Next
                                End If
                                'added 9/27/2012 for CPR
                                _ClassificationCode = qqHelper.CloneObject(cov.ClassificationCode) 'updated 10/15/2014 to clone
                                If _EarthquakeApplies = False Then 'added conditional 11/6/2012 to make sure this won't overwrite 21155
                                    _EarthquakeApplies = cov.IsEarthquakeApplies
                                End If
                                CauseOfLossTypeId = cov.CauseOfLossTypeId
                                CoinsuranceTypeId = cov.CoinsuranceTypeId
                                DeductibleId = cov.DeductibleId
                                RatingTypeId = cov.RatingTypeId
                                InflationGuardTypeId = cov.InflationGuardTypeId
                            End If
                        Case "80145" 'Combo; *should probably be at Policy level; 3/20/2013 note: Property Deductible
                            PropertyDeductibleId = cov.CoverageLimitId
                        Case "21037" 'Edit; 3/20/2013 note: Personal Property
                            PersonalPropertyLimit = cov.ManualLimitAmount
                            PersonalPropertyLimitQuotedPremium = cov.FullTermPremium
                            ValuationMethodId = cov.ValuationMethodTypeId
                            _IsValMethodIncludedInBlanketRating = cov.IsIncludedInBlanketRating
                        Case "150" 'Edit; 3/20/2013 note: Accounts Receivable
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.InlandMarine Then 'added IF 4/13/2015; may not be needed
                                '_AccountsReceivableOnPremises = cov.ManualLimitAmount
                                '_AccountsReceivableQuotedPremium = qqHelper.getSum(_AccountsReceivableQuotedPremium, cov.FullTermPremium)
                                'If _HasBusinessMasterEnhancement = True Then 'updated 8/13/2012
                                AccountsReceivableOnPremisesExcessLimit = cov.ManualLimitAmount
                                'AccountsReceivableOnPremises = qqHelper.getSum("50000", cov.ManualLimitAmount)
                                'AccountsReceivableOnPremises = qqHelper.getSum("50000", _AccountsReceivableOnPremisesExcessLimit) 'commented 8/13/2012
                                AccountsReceivableOnPremises = _AccountsReceivableOnPremisesExcessLimit 'added 8/13/2012
                                If _HasBusinessMasterEnhancement = True Then
                                    AccountsReceivableOffPremises = "25000" 'moved inside if statement 8/13/2012
                                ElseIf _AccountsReceivableOnPremisesExcessLimit <> "" Then 'added 8/13/2012
                                    AccountsReceivableOffPremises = "5000"
                                End If
                                AccountsReceivableQuotedPremium = cov.FullTermPremium
                                'End If
                            End If
                        Case "151" 'Edit; 3/20/2013 note: Valuable Papers and Records
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.InlandMarine Then 'added IF 4/13/2015; may not be needed
                                'If _HasBusinessMasterEnhancement = True Then 'updated 8/13/2012
                                ValuablePapersOnPremisesExcessLimit = cov.ManualLimitAmount
                                'ValuablePapersOnPremises = qqHelper.getSum("25000", cov.ManualLimitAmount)
                                'ValuablePapersOnPremises = qqHelper.getSum("25000", _ValuablePapersOnPremisesExcessLimit) 'commented 8/13/2012
                                ValuablePapersOnPremises = _ValuablePapersOnPremisesExcessLimit 'added 8/13/2012
                                If _HasBusinessMasterEnhancement = True Then
                                    ValuablePapersOffPremises = "10000" 'moved inside if statement 8/13/2012
                                ElseIf _ValuablePapersOnPremisesExcessLimit <> "" Then 'added 8/13/2012
                                    ValuablePapersOffPremises = "5000"
                                End If
                                ValuablePapersQuotedPremium = cov.FullTermPremium
                                'End If
                            End If
                        Case "21038" 'Combo; specs also say 21039, but I didn't see it returned in 6/27/2012 test; 3/20/2013 note: Condominium Commercial Unit Owners: Loss Assessment
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty Then 'added IF 4/13/2015; may not be needed
                                CondoCommercialUnitOwnersLimitId = cov.CoverageLimitId
                                CondoCommercialUnitOwnersLimitQuotedPremium = cov.FullTermPremium
                            End If
                        Case "173" 'CheckBox; 3/20/2013 note: Ordinance or Law: Undamaged Part
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty Then 'added IF 4/13/2015; may not be needed
                                _HasOrdinanceOrLaw = True
                                _HasOrdOrLawUndamagedPortion = cov.Checkbox
                                OrdOrLawUndamagedPortionQuotedPremium = cov.FullTermPremium
                            End If
                        Case "161" 'Edit; 3/20/2013 note: Ordinance or Law: Demolition Cost
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty Then 'added IF 4/13/2015; may not be needed
                                _HasOrdinanceOrLaw = True
                                OrdOrLawDemoCostLimit = cov.ManualLimitAmount
                                OrdOrLawDemoCostLimitQuotedPremium = cov.FullTermPremium
                            End If
                        Case "167" 'Edit; 3/20/2013 note: Ordinace or Law: Increased Cost
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty Then 'added IF 4/13/2015; may not be needed
                                _HasOrdinanceOrLaw = True
                                OrdOrLawIncreasedCostLimit = cov.ManualLimitAmount
                                OrdOrLawIncreaseCostLimitQuotedPremium = cov.FullTermPremium
                            End If
                        Case "21045" 'Edit; 3/20/2013 note: Ordinance or Law: Demolition and Increased Cost Combined
                            _HasOrdinanceOrLaw = True
                            OrdOrLawDemoAndIncreasedCostLimit = cov.ManualLimitAmount
                            OrdOrLawDemoAndIncreasedCostLimitQuotedPremium = cov.FullTermPremium
                        Case "70066" 'Edit; 3/20/2013 note: Spoilage
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty Then 'added IF 4/13/2015; may not be needed
                                _HasSpoilage = True 'maybe need to see if checkbox is true or false
                                SpoilagePropertyClassificationId = cov.PropertyClassificationTypeId
                                SpoilageTotalLimit = cov.ManualLimitAmount
                                SpoilageQuotedPremium = cov.FullTermPremium
                                _IsSpoilageRefrigerationMaintenanceAgreement = cov.IsRefrigerationMaintenanceAgreement
                                _IsSpoilageBreakdownOrContamination = cov.IsBreakdownOrContamination
                                _IsSpoilagePowerOutage = cov.IsPowerOutage
                            End If
                            'Case "20027" 'Checkbox; *might need for mine subsidence eventhough hasMineSubsidence is under 165 (verified that it's not needed 6/27/2012)
                        Case "20027" 'CheckBox: Mine Subsidence (BOP IN/IL, CPR IN/IL); started using 11/8/2018; 11/9/2018 note: BOP was previously working w/ flag on covCodeId 165 only, but premium appears that it would've always come out of this cov; this cov would definitely be required for CPR since it doesn't have the flag on 165; Martin's no longer getting BOP IL mineSub errors after updates, so maybe adding this cov allows it to pass new validation that wasn't previously required
                            If cov.Checkbox = True Then
                                _HasMineSubsidence = True
                                If qqHelper.IsPositiveDecimalString(MineSubsidenceQuotedPremium) = False Then 'using IF in case premium will still be found under covCodeId 165 and CoverageAdditionalInfoRecord w/ description "MINE SUBSIDENCE PREMIUM"
                                    MineSubsidenceQuotedPremium = cov.FullTermPremium
                                End If
                                _MineSubsidence_IsDwellingStructure = cov.IsDwellingStructure 'added 11/8/2018 for CPR mine subsidence; covCodeId 20027; CheckBox: Mine Subsidence (BOP IN/IL, CPR IN/IL)
                            End If

                            'Case "21090" 'Edit:  Building - Personal Property (this one's inside the ScheduledCoverages node instead of the normal Coverages node); added 9/27/2012

                        Case "21095" 'Edit:  Building - Business Income (added 10/8/2012 for CPR)
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty Then 'added IF 4/13/2015; may not be needed
                                BusinessIncomeCov_Limit = cov.ManualLimitAmount
                                BusinessIncomeCov_CoinsuranceTypeId = cov.CoinsuranceTypeId
                                BusinessIncomeCov_MonthlyPeriodTypeId = cov.MonthlyPeriodTypeId
                                BusinessIncomeCov_BusinessIncomeTypeId = cov.BusinessIncomeTypeId
                                BusinessIncomeCov_RiskTypeId = cov.RiskTypeId
                                BusinessIncomeCov_RatingTypeId = cov.RatingTypeId
                                BusinessIncomeCov_CauseOfLossTypeId = cov.CauseOfLossTypeId
                                BusinessIncomeCov_QuotedPremium = cov.FullTermPremium
                                _BusinessIncomeCov_ClassificationCode = qqHelper.CloneObject(cov.ClassificationCode) 'updated 10/15/2014 to clone
                                BusinessIncomeCov_WaitingPeriodTypeId = cov.WaitingPeriodTypeId 'added 10/10/2012 for CPR

                                If _BusinessIncomeCov_EarthquakeApplies = False Then 'added 11/6/2012 to work w/ 21163 (works either way)
                                    _BusinessIncomeCov_EarthquakeApplies = cov.IsEarthquakeApplies
                                End If
                                _BusinessIncomeCov_IncludedInBlanketCoverage = cov.IsIncludedInBlanketRating 'added 3/26/2013 for CPR
                            End If
                        Case "21163" 'CheckBox:  Business Income - Earthquake (added 10/8/2012 for CPR)
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty Then 'added IF 4/13/2015; may not be needed
                                _BusinessIncomeCov_EarthquakeApplies = True 'might need to use CheckBox
                                BusinessIncomeCov_EarthquakeQuotedPremium = cov.FullTermPremium 'added 11/13/2012 for CPR
                                'BusinessIncomeCov_EarthquakeDeductibleId = cov.DeductibleId 'not applying earthquake deductible for business income per requirements

                                'updated 11/28/2012 for CPR
                                If cov.CoverageAdditionalInfoRecords IsNot Nothing AndAlso cov.CoverageAdditionalInfoRecords.Count > 0 Then
                                    For Each add As QuickQuoteCoverageAdditionalInfoRecord In cov.CoverageAdditionalInfoRecords
                                        If UCase(add.Description) = "EARTHQUAKEBUILDINGCLASSIFICATIONPERCENTAGE" Then
                                            CPR_BusinessIncomeCov_EarthquakeBuildingClassificationPercentage = add.Value
                                            Exit For
                                        End If
                                    Next
                                End If
                            End If

                        Case "100010" 'Drop Down:  Earthquake Deductible
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty Then
                                EarthquakeDeductibleId = cov.DeductibleId
                            End If

                        Case "21155" 'CheckBox:  Building - Earthquake (added 11/6/2012 for CPR; was previously just using IsEarthQuakeApplies value for 165)
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty Then 'added IF 4/10/2015; may not be needed
                                _EarthquakeApplies = True 'might need to use CheckBox
                                EarthquakeQuotedPremium = cov.FullTermPremium 'added 11/13/2012 for CPR
                                BuildingCov_EarthquakeDeductibleId = cov.DeductibleId

                                'updated 11/28/2012 for CPR
                                If cov.CoverageAdditionalInfoRecords IsNot Nothing AndAlso cov.CoverageAdditionalInfoRecords.Count > 0 Then
                                    For Each add As QuickQuoteCoverageAdditionalInfoRecord In cov.CoverageAdditionalInfoRecords
                                        If UCase(add.Description) = "EARTHQUAKEBUILDINGCLASSIFICATIONPERCENTAGE" Then
                                            CPR_BuildingLimit_EarthquakeBuildingClassificationPercentage = add.Value
                                            Exit For
                                        End If
                                    Next
                                End If
                            End If

                            'added 10/9/2012 for CPR (specific rates)
                        Case "21175" 'CheckBox:  Building - Group I
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty Then 'added IF 4/10/2015; may not be needed
                                If _Building_BusinessIncome_Group1_Rate = "" AndAlso cov.Rate <> "" Then
                                    _Building_BusinessIncome_Group1_Rate = cov.Rate
                                End If
                                If _Building_BusinessIncome_Group1_LossCost = "" AndAlso cov.LossCost <> "" Then
                                    _Building_BusinessIncome_Group1_LossCost = cov.LossCost
                                End If
                            End If
                        Case "21180" 'CheckBox:  Building - Group II
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty Then 'added IF 4/10/2015; may not be needed
                                If _Building_BusinessIncome_Group2_Rate = "" AndAlso cov.Rate <> "" Then
                                    _Building_BusinessIncome_Group2_Rate = cov.Rate
                                End If
                                If _Building_BusinessIncome_Group2_LossCost = "" AndAlso cov.LossCost <> "" Then
                                    _Building_BusinessIncome_Group2_LossCost = cov.LossCost
                                End If
                            End If
                        Case "21176" 'CheckBox:  Personal Property - Group I (should be under ScheduledCoverages)
                            If _PersonalProperty_Group1_LossCost = "" AndAlso cov.LossCost <> "" Then
                                _PersonalProperty_Group1_LossCost = cov.LossCost
                            End If
                            If _PersonalProperty_Group1_Rate = "" AndAlso cov.Rate <> "" Then
                                _PersonalProperty_Group1_Rate = cov.Rate
                            End If
                        Case "21181" 'CheckBox:  Personal Property - Group II (should be under ScheduledCoverages)
                            If _PersonalProperty_Group2_Rate = "" AndAlso cov.Rate <> "" Then
                                _PersonalProperty_Group2_Rate = cov.Rate
                            End If
                            If _PersonalProperty_Group2_LossCost = "" AndAlso cov.LossCost <> "" Then
                                _PersonalProperty_Group2_LossCost = cov.LossCost
                            End If
                        Case "21178" 'CheckBox:  Building Business Income - Group I
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty Then 'added IF 4/10/2015; may not be needed
                                If _Building_BusinessIncome_Group1_Rate = "" AndAlso cov.Rate <> "" Then
                                    _Building_BusinessIncome_Group1_Rate = cov.Rate
                                End If
                                If _Building_BusinessIncome_Group1_LossCost = "" AndAlso cov.LossCost <> "" Then
                                    _Building_BusinessIncome_Group1_LossCost = cov.LossCost
                                End If
                            End If
                        Case "21183" 'CheckBox:  Building Business Income - Group II
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty Then 'added IF 4/10/2015; may not be needed
                                If _Building_BusinessIncome_Group2_Rate = "" AndAlso cov.Rate <> "" Then
                                    _Building_BusinessIncome_Group2_Rate = cov.Rate
                                End If
                                If _Building_BusinessIncome_Group2_LossCost = "" AndAlso cov.LossCost <> "" Then
                                    _Building_BusinessIncome_Group2_LossCost = cov.LossCost
                                End If
                            End If

                            'added 10/17/2012 for CPR
                        Case "21534" 'CheckBox:  Building - Broad
                        Case "21535" 'CheckBox:  Building - Special

                            'added 11/29/2012 for CPR
                        Case "21167" 'CheckBox:  Building - Optional Theft
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty Then 'added IF 4/10/2015; may not be needed
                                OptionalTheftDeductibleId = cov.DeductibleId
                            End If
                        Case "21170" 'CheckBox:  Building - Windstorm or Hail
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty Then 'added IF 4/13/2015; may not be needed
                                OptionalWindstormOrHailDeductibleId = cov.DeductibleId
                            End If

                            'added 1/19/2015 for CIM (Commercial Inland Marine)
                        Case "21321" 'Edit: Building - Fine Arts - Schedule; corresponds w/ 50026 (Fine Arts) on the QuickQuoteObject; 3/11/2015 note: can have multiple; examples have ManualLimitAmount, CoverageBasisTypeId 1, Description; 4/1/2015 note: example has ShouldSyncWithMasterCoverage true... now updated to use in xml
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.InlandMarine Then 'added IF 4/8/2015
                                'updated w/ logic 3/16/2015
                                If _FineArtsScheduledItems Is Nothing Then
                                    _FineArtsScheduledItems = New List(Of QuickQuoteFineArtsScheduledItem)
                                End If
                                Dim fa As New QuickQuoteFineArtsScheduledItem
                                With fa
                                    .Limit = cov.ManualLimitAmount
                                    .Description = cov.Description
                                    .QuotedPremium = cov.FullTermPremium
                                End With
                                _FineArtsScheduledItems.Add(fa)
                            End If
                        Case "21317" 'Edit: Building - Fine Arts Dealer - Schedule; added 3/11/2015; corresponds w/ 21310 (Fine Arts Dealer) on the QuickQuoteObject; probably can have multiple but Diamond UI just allows 1; example has ManualLimitAmount, CoverageBasisTypeId 1
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.InlandMarine Then 'added IF 4/8/2015

                            End If
                        Case "21499" 'Edit: Building - Computer - Hardware; 4/1/2015 note: example has ShouldSyncWithMasterCoverage true... now updated to use in xml
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.InlandMarine Then 'added IF 4/8/2015
                                'updated w/ logic 2/9/2015
                                ComputerHardwareLimit = cov.ManualLimitAmount 'cov also has CoverageBasisTypeId set to 1
                                ComputerHardwareRate = cov.Rate
                                ComputerHardwareQuotedPremium = cov.FullTermPremium
                            End If
                        Case "21500" 'Edit: Building - Computer - Programs and Applications and Media; 4/1/2015 note: example has ShouldSyncWithMasterCoverage true... now updated to use in xml
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.InlandMarine Then 'added IF 4/8/2015
                                'updated w/ logic 2/9/2015
                                ComputerProgramsApplicationsAndMediaLimit = cov.ManualLimitAmount 'cov also has CoverageBasisTypeId set to 1
                                ComputerProgramsApplicationsAndMediaRate = cov.Rate
                                ComputerProgramsApplicationsAndMediaQuotedPremium = cov.FullTermPremium
                            End If
                        Case "21501" 'Edit: Building - Computer - Business Income; 4/1/2015 note: example has ShouldSyncWithMasterCoverage true... now updated to use in xml
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.InlandMarine Then 'added IF 4/8/2015
                                'updated w/ logic 2/9/2015
                                ComputerBusinessIncomeLimit = cov.ManualLimitAmount 'cov also has CoverageBasisTypeId set to 1
                                ComputerBusinessIncomeRate = cov.Rate
                                ComputerBusinessIncomeQuotedPremium = cov.FullTermPremium
                            End If
                            'added 1/20/2015 for CIM
                        Case "21259" 'Edit: Building - Signs - Schedule; corresponds w/ 21256 (Signs) on the QuickQuoteObject; 3/24/2015 note: latest example has ManualLimitAmount, CoverageBasisTypeId, IsIndoor (CoverageDetail; boolean), Description; can have multiple; 3/26/2015 note: may need ShouldSyncWithMasterCoverage true in CoverageDetail... confirmed 4/1/2015
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.InlandMarine Then 'added IF 4/8/2015
                                'updated w/ logic 3/26/2015
                                If _ScheduledSigns Is Nothing Then
                                    _ScheduledSigns = New List(Of QuickQuoteScheduledSign)
                                End If
                                Dim ss As New QuickQuoteScheduledSign
                                With ss
                                    .Limit = cov.ManualLimitAmount
                                    .IsIndoor = cov.IsIndoor
                                    .Description = cov.Description
                                    .QuotedPremium = cov.FullTermPremium
                                End With
                                _ScheduledSigns.Add(ss)
                            End If
                        Case "21258" 'Edit: Building - Signs - Unscheduled; added 3/24/2015; has tie back to 21256 (Signs) on the QuickQuoteObject; latest example has ManualLimitAmount, CoverageBasisTypeId 1; 3/26/2015 note: may need ShouldSyncWithMasterCoverage true in CoverageDetail... confirmed 4/1/2015
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.InlandMarine Then 'added IF 4/8/2015
                                'updated w/ logic 3/26/2015
                                UnscheduledSignsLimit = cov.ManualLimitAmount
                                UnscheduledSignsQuotedPremium = cov.FullTermPremium
                            End If

                            'added 6/15/2015 for Farm
                        Case "40004" 'Edit: Location E. Farm Barns and Buildings; ManualLimitAmount 20000, ManualLimitIncreased 20000 CoverageLimitId 24 (1000 deductible)
                            E_Farm_Limit = cov.ManualLimitAmount
                            E_Farm_DeductibleLimitId = cov.CoverageLimitId 'static data
                            E_Farm_QuotedPremium = cov.FullTermPremium
                        Case "40145" 'Combo: Policy Wind/Hail Deductible; note: 40144 is for policy level; CoverageLimitId - 1

                        Case "80130" 'Edit: Household Contents; ManualLimitAmount 1200 ManualLimitIncreased 1200
                            'added logic 6/16/2015 for Farm
                            HouseholdContentsLimit = cov.ManualLimitAmount
                            HouseholdContentsQuotedPremium = cov.FullTermPremium
                        Case "70180" 'CheckBox: Barn_Solid_Fuel_Burning_Units; Checkbox true

                        Case "70181" 'CheckBox: Barn_Amendment_of_Vacancy_or_Unoccupancy; Checkbox true

                        Case "70204" 'CheckBox: Barn_Wood_Roof_RC_Settlement_Terms_Surcharge; Checkbox true

                        Case "80381" '3/9/2017 - BOP stuff
                            _HasRestaurantEndorsement = cov.Checkbox

                            'added 10/26/2018 w/ multi-state project
                        Case "20040" 'CheckBox: Actual Cash Value Loss Settlement / Windstorm or Hail Losses to Roof Surfacing; BOP IN/IL
                            If cov.Checkbox = True Then
                                _HasWindHailACVSettlement = True
                                WindHailACVSettlementQuotedPremium = cov.FullTermPremium
                            End If
                        Case "80542" 'CheckBox: Limitations on Roof Surfacing; BOP IL only
                            If cov.Checkbox = True Then
                                _HasLimitationsOnRoofSurfacing = True
                                LimitationsOnRoofSurfacingQuotedPremium = cov.FullTermPremium
                            End If
                        Case "80543" 'CheckBox: ACV Roof Surfacing; BOP IL only
                            If cov.Checkbox = True Then
                                _HasACVRoofSurfacing = True
                                ACVRoofSurfacingQuotedPremium = cov.FullTermPremium
                            End If
                        Case "80544" 'CheckBox: Exclude Cosmetic Damage; BOP IL only
                            If cov.Checkbox = True Then
                                _ExcludeCosmeticDamage = True
                                ExcludeCosmeticDamageQuotedPremium = cov.FullTermPremium
                            End If

                    End Select
                Next
            End If
        End Sub
        ''' <summary>
        ''' used to parse thru classifications and set different properties
        ''' </summary>
        ''' <remarks>always executed when xml is parsed; doesn't specifically needed to be called by developer code</remarks>
        Public Sub ParseThruClassifications()
            If _BuildingClassifications IsNot Nothing AndAlso _BuildingClassifications.Count > 0 Then
                For Each cls As QuickQuoteClassification In _BuildingClassifications
                    If cls.ClassificationTypeId <> "" Then
                        ClassificationTypeId = cls.ClassificationTypeId
                        _AnnualReceipts = cls.AnnualSalesReceipts
                        _NumberOfOfficersAndPartnersAndInsureds = cls.NumberOfExecutiveOfficers
                        _EmployeePayroll = cls.Payroll
                    End If
                    If _CanUseClassificationNumForClassificationReconciliation = False Then 'added 2/20/2017 for reconciliation
                        If cls.HasValidClassificationNum = True Then
                            _CanUseClassificationNumForClassificationReconciliation = True
                        End If
                    End If
                Next
            End If
        End Sub
        ''' <summary>
        ''' used to parse thru scheduled coverages and set different properties
        ''' </summary>
        ''' <remarks>always executed when xml is parsed; doesn't specifically needed to be called by developer code</remarks>
        Public Sub ParseThruScheduledCoverages()
            ParseThruScheduledCoverages(_ScheduledCoverages)
        End Sub
        ''' <summary>
        ''' used to parse thru scheduled coverages and set different properties
        ''' </summary>
        ''' <remarks>always executed when xml is parsed; doesn't specifically needed to be called by developer code</remarks>
        ''' 'Public Sub ParseThruScheduledCoverages() 'added 9/27/2012 for CPR
        Public Sub ParseThruScheduledCoverages(ByVal schCovs As List(Of QuickQuoteScheduledCoverage), Optional ByVal packagePartType As QuickQuoteXML.QuickQuotePackagePartType = QuickQuoteXML.QuickQuotePackagePartType.None) 'added new params 4/8/2015 for CPP
            'If _ScheduledCoverages IsNot Nothing AndAlso _ScheduledCoverages.Count > 0 Then
            'updated 4/8/2015 to use param
            If schCovs IsNot Nothing AndAlso schCovs.Count > 0 Then
                Dim isPersPropOfOthers As Boolean = False 'added 11/6/2012 for CPR; 9/26/2013 note: should probably be initialized directly below the 'for each scheduledcoverage line' (so each one starts as false instead of being determined when coveragecodeid 21090 is found)
                'For Each sc As QuickQuoteScheduledCoverage In _ScheduledCoverages
                'updated 4/8/2015 to use param
                For Each sc As QuickQuoteScheduledCoverage In schCovs
                    'added 1/22/2015; note: should only happen when parsing xml (FinalizeQuickQuote method)... shouldn't happen in FinalizeQuickQuoteLight method
                    If _CanUseScheduledCoverageNumForScheduledCoverageReconciliation = False Then
                        If sc.HasValidScheduledCoverageNum = True Then
                            _CanUseScheduledCoverageNumForScheduledCoverageReconciliation = True
                        End If
                    End If
                    '1/19/2015 note: should probably check UICoverageScheduledCoverageParentTypeId property; 92 = 21090	Personal Property
                    Select Case sc.UICoverageScheduledCoverageParentTypeId 'started using 4/14/2015; original logic is in 92 CASE
                        Case "92" '21090	Personal Property
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty Then 'added IF 4/14/2015
                                'added 7/8/2017
                                If _BuildingPersonalProperties Is Nothing Then
                                    _BuildingPersonalProperties = New List(Of QuickQuoteBuildingPersonalProperty)
                                End If
                                Dim bpp As New QuickQuoteBuildingPersonalProperty
                                bpp.ScheduledCoverageNum = sc.ScheduledCoverageNum

                                If sc.Coverages IsNot Nothing AndAlso sc.Coverages.Count > 0 Then
                                    For Each c As QuickQuoteCoverage In sc.Coverages
                                        Select Case c.CoverageCodeId
                                            Case "21090" 'Edit:  Building - Personal Property
                                                'updated 10/8/2012 for PersPropOfOthers
                                                If c.BusinessPropertyTypeId = "8" AndAlso Has_CPR_PersonalPropertyOfOthers() = False Then 'Personal Property of Others
                                                    isPersPropOfOthers = True
                                                    PersPropOfOthers_PersonalPropertyLimit = c.ManualLimitAmount
                                                    PersPropOfOthers_RiskTypeId = c.RiskTypeId
                                                    If _PersPropOfOthers_EarthquakeApplies = False Then 'added conditional 11/6/2012 to make sure this won't overwrite 21160
                                                        _PersPropOfOthers_EarthquakeApplies = c.IsEarthquakeApplies
                                                    End If
                                                    PersPropOfOthers_RatingTypeId = c.RatingTypeId
                                                    PersPropOfOthers_CauseOfLossTypeId = c.CauseOfLossTypeId
                                                    PersPropOfOthers_DeductibleId = c.DeductibleId
                                                    PersPropOfOthers_CoinsuranceTypeId = c.CoinsuranceTypeId
                                                    PersPropOfOthers_ValuationId = c.ValuationMethodTypeId
                                                    PersPropOfOthers_QuotedPremium = c.FullTermPremium
                                                    _PersPropOfOthers_ClassificationCode = qqHelper.CloneObject(c.ClassificationCode) 'updated 10/15/2014 to clone
                                                    PersPropOfOthers_DoesYardRateApplyTypeId = c.DoesYardRateApplyTypeId 'added 1/2/2013 for CPR
                                                    _PersPropOfOthers_IncludedInBlanketCoverage = c.IsIncludedInBlanketRating 'added 3/26/2013 for CPR
                                                    'added 1/22/2015
                                                    _PersPropOfOthers_ScheduledCoverageNum = sc.ScheduledCoverageNum
                                                    _PersPropOfOthers_InflationGuardTypeId = c.InflationGuardTypeId
                                                Else
                                                    isPersPropOfOthers = False
                                                    PersPropCov_PersonalPropertyLimit = c.ManualLimitAmount
                                                    PersPropCov_PropertyTypeId = c.BusinessPropertyTypeId
                                                    PersPropCov_RiskTypeId = c.RiskTypeId
                                                    If _PersPropCov_EarthquakeApplies = False Then 'added conditional 11/6/2012 to make sure this won't overwrite 21160
                                                        _PersPropCov_EarthquakeApplies = c.IsEarthquakeApplies
                                                    End If
                                                    PersPropCov_RatingTypeId = c.RatingTypeId
                                                    PersPropCov_CauseOfLossTypeId = c.CauseOfLossTypeId
                                                    PersPropCov_DeductibleId = c.DeductibleId
                                                    PersPropCov_CoinsuranceTypeId = c.CoinsuranceTypeId
                                                    PersPropCov_ValuationId = c.ValuationMethodTypeId
                                                    PersPropCov_QuotedPremium = c.FullTermPremium
                                                    _PersPropCov_ClassificationCode = qqHelper.CloneObject(c.ClassificationCode) 'updated 10/15/2014 to clone
                                                    PersPropCov_DoesYardRateApplyTypeId = c.DoesYardRateApplyTypeId 'added 1/2/2013 for CPR
                                                    _PersPropCov_IncludedInBlanketCoverage = c.IsIncludedInBlanketRating 'added 3/26/2013 for CPR
                                                    _PersPropCov_IsAgreedValue = c.IsAgreedValue ' added 2/1/2016 bug 4845 Matt A; 3/9/2017 - included in this library w/ BOP updates
                                                    'added 1/22/2015
                                                    _PersPropCov_ScheduledCoverageNum = sc.ScheduledCoverageNum
                                                    _PersPropCov_InflationGuardTypeId = c.InflationGuardTypeId
                                                End If

                                                'added 7/8/2017
                                                With bpp
                                                    .PersonalPropertyLimit = c.ManualLimitAmount
                                                    .PropertyTypeId = c.BusinessPropertyTypeId
                                                    .RiskTypeId = c.RiskTypeId
                                                    If .EarthquakeApplies = False Then
                                                        .EarthquakeApplies = c.IsEarthquakeApplies
                                                    End If
                                                    .IsAgreedValue = c.IsAgreedValue
                                                    .RatingTypeId = c.RatingTypeId
                                                    .CauseOfLossTypeId = c.CauseOfLossTypeId
                                                    .DeductibleId = c.DeductibleId
                                                    .CoinsuranceTypeId = c.CoinsuranceTypeId
                                                    .ValuationId = c.ValuationMethodTypeId
                                                    .ClassificationCode = qqHelper.CloneObject(c.ClassificationCode)
                                                    .DoesYardRateApplyTypeId = c.DoesYardRateApplyTypeId
                                                    .IncludedInBlanketCoverage = c.IsIncludedInBlanketRating

                                                    .BuildingPersonalPropertyQuotedPremium = c.FullTermPremium
                                                    .InflationGuardTypeId = c.InflationGuardTypeId
                                                End With

                                                'added 10/9/2012 for CPR (specific rates); copied from Coverages above on 10/17/2012
                                            Case "21175" 'CheckBox:  Building - Group I (should be under Coverages)
                                                If _Building_BusinessIncome_Group1_Rate = "" AndAlso c.Rate <> "" Then
                                                    _Building_BusinessIncome_Group1_Rate = c.Rate
                                                End If
                                                If _Building_BusinessIncome_Group1_LossCost = "" AndAlso c.LossCost <> "" Then
                                                    _Building_BusinessIncome_Group1_LossCost = c.LossCost
                                                End If
                                            Case "21180" 'CheckBox:  Building - Group II (should be under Coverages)
                                                If _Building_BusinessIncome_Group2_Rate = "" AndAlso c.Rate <> "" Then
                                                    _Building_BusinessIncome_Group2_Rate = c.Rate
                                                End If
                                                If _Building_BusinessIncome_Group2_LossCost = "" AndAlso c.LossCost <> "" Then
                                                    _Building_BusinessIncome_Group2_LossCost = c.LossCost
                                                End If
                                            Case "21176" 'CheckBox:  Personal Property - Group I
                                                If _PersonalProperty_Group1_LossCost = "" AndAlso c.LossCost <> "" Then
                                                    _PersonalProperty_Group1_LossCost = c.LossCost
                                                End If
                                                If _PersonalProperty_Group1_Rate = "" AndAlso c.Rate <> "" Then
                                                    _PersonalProperty_Group1_Rate = c.Rate
                                                End If

                                                'added 7/8/2017
                                                With bpp
                                                    .Group1_LossCost = c.LossCost
                                                    .Group1_Rate = c.Rate
                                                End With
                                            Case "21181" 'CheckBox:  Personal Property - Group II
                                                If _PersonalProperty_Group2_Rate = "" AndAlso c.Rate <> "" Then
                                                    _PersonalProperty_Group2_Rate = c.Rate
                                                End If
                                                If _PersonalProperty_Group2_LossCost = "" AndAlso c.LossCost <> "" Then
                                                    _PersonalProperty_Group2_LossCost = c.LossCost
                                                End If

                                                'added 7/8/2017
                                                With bpp
                                                    .Group2_LossCost = c.LossCost
                                                    .Group2_Rate = c.Rate
                                                End With
                                            Case "21178" 'CheckBox:  Building Business Income - Group I (should be under Coverages)
                                                If _Building_BusinessIncome_Group1_Rate = "" AndAlso c.Rate <> "" Then
                                                    _Building_BusinessIncome_Group1_Rate = c.Rate
                                                End If
                                                If _Building_BusinessIncome_Group1_LossCost = "" AndAlso c.LossCost <> "" Then
                                                    _Building_BusinessIncome_Group1_LossCost = c.LossCost
                                                End If
                                            Case "21183" 'CheckBox:  Building Business Income - Group II (should be under Coverages)
                                                If _Building_BusinessIncome_Group2_Rate = "" AndAlso c.Rate <> "" Then
                                                    _Building_BusinessIncome_Group2_Rate = c.Rate
                                                End If
                                                If _Building_BusinessIncome_Group2_LossCost = "" AndAlso c.LossCost <> "" Then
                                                    _Building_BusinessIncome_Group2_LossCost = c.LossCost
                                                End If
                                            Case "21160" 'CheckBox:  Personal Property - Earthquake (added 10/23/2012)
                                                If _PersonalProperty_EarthquakeRateGradeTypeId = "" Then
                                                    _PersonalProperty_EarthquakeRateGradeTypeId = c.PersonalPropertyRateGradeTypeId
                                                End If
                                                If isPersPropOfOthers = True Then 'added 11/6/2012 for CPR
                                                    _PersPropOfOthers_EarthquakeApplies = True
                                                    PersPropOfOthers_EarthquakeQuotedPremium = c.FullTermPremium 'added 11/13/2012 for CPR
                                                    PersPropOfOthers_EarthquakeDeductibleId = c.DeductibleId

                                                    'updated 11/28/2012 for CPR
                                                    If c.CoverageAdditionalInfoRecords IsNot Nothing AndAlso c.CoverageAdditionalInfoRecords.Count > 0 Then
                                                        For Each add As QuickQuoteCoverageAdditionalInfoRecord In c.CoverageAdditionalInfoRecords
                                                            If UCase(add.Description) = "EARTHQUAKEBUILDINGCLASSIFICATIONPERCENTAGE" Then
                                                                CPR_PersPropOfOthers_EarthquakeBuildingClassificationPercentage = add.Value
                                                                Exit For
                                                            End If
                                                        Next
                                                    End If
                                                Else
                                                    _PersPropCov_EarthquakeApplies = True
                                                    PersPropCov_EarthquakeQuotedPremium = c.FullTermPremium 'added 11/13/2012 for CPR
                                                    PersPropCov_EarthquakeDeductibleId = c.DeductibleId

                                                    'updated 11/28/2012 for CPR
                                                    If c.CoverageAdditionalInfoRecords IsNot Nothing AndAlso c.CoverageAdditionalInfoRecords.Count > 0 Then
                                                        For Each add As QuickQuoteCoverageAdditionalInfoRecord In c.CoverageAdditionalInfoRecords
                                                            If UCase(add.Description) = "EARTHQUAKEBUILDINGCLASSIFICATIONPERCENTAGE" Then
                                                                CPR_PersPropCov_EarthquakeBuildingClassificationPercentage = add.Value
                                                                Exit For
                                                            End If
                                                        Next
                                                    End If
                                                End If

                                                'added 7/8/2017
                                                With bpp
                                                    .EarthquakeApplies = True
                                                    .EarthquakeRateGradeTypeId = c.PersonalPropertyRateGradeTypeId

                                                    If c.CoverageAdditionalInfoRecords IsNot Nothing AndAlso c.CoverageAdditionalInfoRecords.Count > 0 Then
                                                        For Each add As QuickQuoteCoverageAdditionalInfoRecord In c.CoverageAdditionalInfoRecords
                                                            If UCase(add.Description) = "EARTHQUAKEBUILDINGCLASSIFICATIONPERCENTAGE" Then
                                                                .EarthquakeBuildingClassificationPercentage = add.Value
                                                                Exit For
                                                            End If
                                                        Next
                                                    End If

                                                    .PersonalPropertyEarthquakeQuotedPremium = c.FullTermPremium
                                                End With

                                                'added 11/29/2012 for CPR
                                            Case "21168" 'CheckBox:  Personal Property - Optional Theft
                                                If isPersPropOfOthers = True Then
                                                    PersPropOfOthers_OptionalTheftDeductibleId = c.DeductibleId
                                                Else
                                                    PersPropCov_OptionalTheftDeductibleId = c.DeductibleId
                                                End If

                                                'added 7/8/2017
                                                With bpp
                                                    .OptionalTheftDeductibleId = c.DeductibleId
                                                End With
                                            Case "21171" 'CheckBox:  Personal Property - Windstorm or Hail
                                                If isPersPropOfOthers = True Then
                                                    PersPropOfOthers_OptionalWindstormOrHailDeductibleId = c.DeductibleId
                                                Else
                                                    PersPropCov_OptionalWindstormOrHailDeductibleId = c.DeductibleId
                                                End If

                                                'added 7/8/2017
                                                With bpp
                                                    .OptionalWindstormOrHailDeductibleId = c.DeductibleId
                                                End With
                                        End Select
                                    Next
                                End If

                                'added 7/8/2017
                                With bpp
                                    .BuildingPersonalPropertyWithEarthquakeQuotedPremium = qqHelper.getSum(.BuildingPersonalPropertyQuotedPremium, .PersonalPropertyEarthquakeQuotedPremium)

                                    TotalPersonalPropertyCombinedQuotedPremium = qqHelper.getSum(_TotalPersonalPropertyCombinedQuotedPremium, .BuildingPersonalPropertyQuotedPremium)
                                    TotalPersonalPropertyCombinedEarthquakeQuotedPremium = qqHelper.getSum(_TotalPersonalPropertyCombinedEarthquakeQuotedPremium, .PersonalPropertyEarthquakeQuotedPremium)
                                    TotalPersonalPropertyCombinedWithEarthquakeQuotedPremium = qqHelper.getSum(_TotalPersonalPropertyCombinedWithEarthquakeQuotedPremium, .BuildingPersonalPropertyWithEarthquakeQuotedPremium)

                                    If .PropertyTypeId = "8" Then 'Personal Property of Others
                                        TotalPersonalPropertyOfOthersQuotedPremium = qqHelper.getSum(_TotalPersonalPropertyOfOthersQuotedPremium, .BuildingPersonalPropertyQuotedPremium)
                                        TotalPersonalPropertyOfOthersLimit = qqHelper.getSum(_TotalPersonalPropertyOfOthersLimit, .PersonalPropertyLimit)
                                        TotalPersonalPropertyOfOthersEarthquakeQuotedPremium = qqHelper.getSum(_TotalPersonalPropertyOfOthersEarthquakeQuotedPremium, .PersonalPropertyEarthquakeQuotedPremium)
                                        TotalPersonalPropertyOfOthersWithEarthquakeQuotedPremium = qqHelper.getSum(_TotalPersonalPropertyOfOthersWithEarthquakeQuotedPremium, .BuildingPersonalPropertyWithEarthquakeQuotedPremium)
                                        _TotalPersonalPropertyOfOthersCount += 1
                                    Else
                                        TotalPersonalPropertyNormalQuotedPremium = qqHelper.getSum(_TotalPersonalPropertyNormalQuotedPremium, .BuildingPersonalPropertyQuotedPremium)
                                        TotalPersonalPropertyNormalLimit = qqHelper.getSum(_TotalPersonalPropertyNormalLimit, .PersonalPropertyLimit)
                                        TotalPersonalPropertyNormalEarthquakeQuotedPremium = qqHelper.getSum(_TotalPersonalPropertyNormalEarthquakeQuotedPremium, .PersonalPropertyEarthquakeQuotedPremium)
                                        TotalPersonalPropertyNormalWithEarthquakeQuotedPremium = qqHelper.getSum(_TotalPersonalPropertyNormalWithEarthquakeQuotedPremium, .BuildingPersonalPropertyWithEarthquakeQuotedPremium)
                                        _TotalPersonalPropertyNormalCount += 1
                                    End If
                                End With
                                _BuildingPersonalProperties.Add(bpp)
                            End If
                    End Select
                Next

                'added 7/8/2017
                PersPropOfOthers_QuotedPremium = _TotalPersonalPropertyOfOthersQuotedPremium 'still being set old way above, but this will overwrite
                PersPropCov_QuotedPremium = _TotalPersonalPropertyNormalQuotedPremium 'still being set old way above, but this will overwrite
                PersPropOfOthers_EarthquakeQuotedPremium = TotalPersonalPropertyOfOthersEarthquakeQuotedPremium 'still being set old way above, but this will overwrite
                PersPropCov_EarthquakeQuotedPremium = _TotalPersonalPropertyNormalEarthquakeQuotedPremium 'still being set old way above, but this will overwrite
                'PersPropOfOthers_PersonalPropertyLimit = _TotalPersonalPropertyOfOthersLimit 'still being set old way above, but this will overwrite
                'PersPropCov_PersonalPropertyLimit = _TotalPersonalPropertyNormalLimit 'still being set old way above, but this will overwrite
                'updated limit properties to only overwrite if positive decimal or if already numeric... so the coverages don't get added when they shouldn't
                If IsNumeric(PersPropOfOthers_PersonalPropertyLimit) = True OrElse qqHelper.IsPositiveDecimalString(_TotalPersonalPropertyOfOthersLimit) = True Then
                    PersPropOfOthers_PersonalPropertyLimit = _TotalPersonalPropertyOfOthersLimit 'still being set old way above, but this will overwrite
                End If
                If IsNumeric(PersPropCov_PersonalPropertyLimit) = True OrElse qqHelper.IsPositiveDecimalString(_TotalPersonalPropertyNormalLimit) = True Then
                    PersPropCov_PersonalPropertyLimit = _TotalPersonalPropertyNormalLimit 'still being set old way above, but this will overwrite
                End If

            End If
        End Sub
        Public Function Has_CPR_PersonalPropertyOfOthers() As Boolean
            'If _PersPropOfOthers_PersonalPropertyLimit <> "" OrElse _PersPropOfOthers_RiskTypeId <> "" OrElse _PersPropOfOthers_EarthquakeApplies = True OrElse _PersPropOfOthers_RatingTypeId <> "" OrElse _PersPropOfOthers_CauseOfLossTypeId <> "" OrElse _PersPropOfOthers_DeductibleId <> "" OrElse _PersPropOfOthers_CoinsuranceTypeId <> "" OrElse _PersPropOfOthers_ValuationId <> "" OrElse _PersPropOfOthers_QuotedPremium <> "" Then
            'updated 7/7/2017
            If qqHelper.IsPositiveDecimalString(_PersPropOfOthers_PersonalPropertyLimit) = True OrElse qqHelper.IsPositiveIntegerString(_PersPropOfOthers_RiskTypeId) = True OrElse _PersPropOfOthers_EarthquakeApplies = True OrElse qqHelper.IsPositiveIntegerString(_PersPropOfOthers_RatingTypeId) = True OrElse qqHelper.IsPositiveIntegerString(_PersPropOfOthers_CauseOfLossTypeId) = True OrElse qqHelper.IsPositiveIntegerString(_PersPropOfOthers_DeductibleId) = True OrElse qqHelper.IsPositiveIntegerString(_PersPropOfOthers_CoinsuranceTypeId) = True OrElse qqHelper.IsPositiveIntegerString(_PersPropOfOthers_ValuationId) = True OrElse qqHelper.IsZeroPremium(_PersPropOfOthers_QuotedPremium) = False Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Sub Calculate_CPR_Covs_TotalBuildingPremium() 'added 10/16/2012 for CPR
            'CPR_Covs_TotalBuildingPremium = qqHelper.getSum(_LimitQuotedPremium, _PersPropCov_QuotedPremium)
            'CPR_Covs_TotalBuildingPremium = qqHelper.getSum(_CPR_Covs_TotalBuildingPremium, _PersPropOfOthers_QuotedPremium)
            'updated 7/8/2017 for Diamond Proposals... since you can have multiple in Diamond whereas VR was coded w/ the assumption that there could only be one
            CPR_Covs_TotalBuildingPremium = qqHelper.getSum(_LimitQuotedPremium, _TotalPersonalPropertyCombinedQuotedPremium)
            CPR_Covs_TotalBuildingPremium = qqHelper.getSum(_CPR_Covs_TotalBuildingPremium, _BusinessIncomeCov_QuotedPremium)
            'added 11/26/2012 for CPR
            CPR_Covs_TotalBuilding_With_EQ_Premium = qqHelper.getSum(_CPR_Covs_TotalBuildingPremium, _CPR_Covs_TotalBuilding_EQ_Premium)
        End Sub
        Public Sub Calculate_CPR_Covs_TotalBuilding_EQ_Premium() 'added 11/15/2012 for CPR
            'CPR_Covs_TotalBuilding_EQ_Premium = qqHelper.getSum(_EarthquakeQuotedPremium, _PersPropCov_EarthquakeQuotedPremium)
            'CPR_Covs_TotalBuilding_EQ_Premium = qqHelper.getSum(_CPR_Covs_TotalBuilding_EQ_Premium, _PersPropOfOthers_EarthquakeQuotedPremium)
            'updated 7/8/2017 for Diamond Proposals... since you can have multiple in Diamond whereas VR was coded w/ the assumption that there could only be one
            CPR_Covs_TotalBuilding_EQ_Premium = qqHelper.getSum(_EarthquakeQuotedPremium, _TotalPersonalPropertyCombinedEarthquakeQuotedPremium)
            CPR_Covs_TotalBuilding_EQ_Premium = qqHelper.getSum(_CPR_Covs_TotalBuilding_EQ_Premium, _BusinessIncomeCov_EarthquakeQuotedPremium)
            'added 11/26/2012 for CPR
            CPR_Covs_TotalBuilding_With_EQ_Premium = qqHelper.getSum(_CPR_Covs_TotalBuildingPremium, _CPR_Covs_TotalBuilding_EQ_Premium)
        End Sub
        Public Sub Calculate_CPR_Covs_With_EQ_Premium() 'added 11/26/2012 for CPR
            CPR_BuildingLimit_With_EQ_QuotedPremium = qqHelper.getSum(_LimitQuotedPremium, _EarthquakeQuotedPremium)
            'CPR_PersPropCov_With_EQ_QuotedPremium = qqHelper.getSum(_PersPropCov_QuotedPremium, _PersPropCov_EarthquakeQuotedPremium)
            'CPR_PersPropOfOthers_With_EQ_QuotedPremium = qqHelper.getSum(_PersPropOfOthers_QuotedPremium, _PersPropOfOthers_EarthquakeQuotedPremium)
            'updated 7/8/2017 for Diamond Proposals... since you can have multiple in Diamond whereas VR was coded w/ the assumption that there could only be one
            CPR_PersPropCov_With_EQ_QuotedPremium = _TotalPersonalPropertyNormalWithEarthquakeQuotedPremium
            CPR_PersPropOfOthers_With_EQ_QuotedPremium = _TotalPersonalPropertyOfOthersWithEarthquakeQuotedPremium
            CPR_BusinessIncomeCov_With_EQ_QuotedPremium = qqHelper.getSum(_BusinessIncomeCov_QuotedPremium, _BusinessIncomeCov_EarthquakeQuotedPremium)
        End Sub
        Public Function HasValidFarmBarnBuildingNum() As Boolean 'added 4/23/2014 for reconciliation purposes
            'If _FarmBarnBuildingNum <> "" AndAlso IsNumeric(_FarmBarnBuildingNum) = True AndAlso CInt(_FarmBarnBuildingNum) > 0 Then
            '    Return True
            'Else
            '    Return False
            'End If
            'updated 4/27/2014 to use common method
            'Return qqHelper.IsValidQuickQuoteIdOrNum(_FarmBarnBuildingNum)
            'updated 10/18/2018 to use new method
            Return HasValidFarmBarnBuildingNum(QuickQuoteXML.QuickQuotePackagePartType.None)
        End Function
        'added 10/18/2018
        Public Function FarmBarnBuildingNumForPackagePartType(ByVal packagePartType As QuickQuoteXML.QuickQuotePackagePartType) As String
            Select Case packagePartType
                Case QuickQuoteXML.QuickQuotePackagePartType.Package
                    Return _FarmBarnBuildingNum_MasterPart
                Case QuickQuoteXML.QuickQuotePackagePartType.GeneralLiability
                    Return _FarmBarnBuildingNum_CGLPart
                Case QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty
                    Return _FarmBarnBuildingNum_CPRPart
                Case QuickQuoteXML.QuickQuotePackagePartType.InlandMarine
                    Return _FarmBarnBuildingNum_CIMPart
                Case QuickQuoteXML.QuickQuotePackagePartType.Crime
                    Return _FarmBarnBuildingNum_CRMPart
                Case QuickQuoteXML.QuickQuotePackagePartType.Garage
                    Return _FarmBarnBuildingNum_GARPart
                Case Else
                    Return _FarmBarnBuildingNum
            End Select
        End Function
        Public Function HasValidFarmBarnBuildingNum(ByVal packagePartType As QuickQuoteXML.QuickQuotePackagePartType) As Boolean
            Return qqHelper.IsValidQuickQuoteIdOrNum(FarmBarnBuildingNumForPackagePartType(packagePartType))
        End Function
        Public Sub SetFarmBarnBuildingNumForPackagePartType(ByVal locNum As String, ByVal packagePartType As QuickQuoteXML.QuickQuotePackagePartType)
            Select Case packagePartType
                Case QuickQuoteXML.QuickQuotePackagePartType.Package
                    _FarmBarnBuildingNum_MasterPart = locNum
                Case QuickQuoteXML.QuickQuotePackagePartType.GeneralLiability
                    _FarmBarnBuildingNum_CGLPart = locNum
                Case QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty
                    _FarmBarnBuildingNum_CPRPart = locNum
                Case QuickQuoteXML.QuickQuotePackagePartType.InlandMarine
                    _FarmBarnBuildingNum_CIMPart = locNum
                Case QuickQuoteXML.QuickQuotePackagePartType.Crime
                    _FarmBarnBuildingNum_CRMPart = locNum
                Case QuickQuoteXML.QuickQuotePackagePartType.Garage
                    _FarmBarnBuildingNum_GARPart = locNum
                Case Else
                    _FarmBarnBuildingNum = locNum
            End Select
        End Sub
        'added 4/29/2014 for additionalInterests reconciliation
        Public Sub ParseThruAdditionalInterests()
            If _AdditionalInterests IsNot Nothing AndAlso _AdditionalInterests.Count > 0 Then
                For Each ai As QuickQuoteAdditionalInterest In _AdditionalInterests
                    'note: should only happen when parsing xml (FinalizeQuickQuote method)... shouldn't happen in FinalizeQuickQuoteLight method
                    If _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = False Then
                        If ai.HasValidAdditionalInterestNum = True Then
                            _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = True
                            Exit For
                        End If
                    End If
                Next
            End If
        End Sub
        Public Function HasValidScheduledCoverageNum_PersPropCov() As Boolean 'added 1/22/2015 for reconciliation purposes
            Return qqHelper.IsValidQuickQuoteIdOrNum(_PersPropCov_ScheduledCoverageNum)
        End Function
        Public Function HasValidScheduledCoverageNum_PersPropOfOthers() As Boolean 'added 1/22/2015 for reconciliation purposes
            Return qqHelper.IsValidQuickQuoteIdOrNum(_PersPropOfOthers_ScheduledCoverageNum)
        End Function

        ''' <summary>
        ''' used to parse thru modifiers and set different properties
        ''' </summary>
        ''' <remarks>always executed when xml is parsed; doesn't specifically need to be called by developer code</remarks>
        Public Sub ParseThruModifiers() 'added 6/15/2015 for FAR (specific to credits and surcharges)
            If _Modifiers IsNot Nothing AndAlso _Modifiers.Count > 0 Then
                For Each m As QuickQuoteModifier In _Modifiers
                    If m.ModifierTypeId <> "" Then 'note: could now use ModifierType and ParentModifierType props instead of hard-coded ids
                        Select Case m.ModifierTypeId 'all have ModifierLevelId 9 (Farm Barns and Buildings)
                            Case "28" 'Sprinkler System; ParentModifierTypeId 28, ModifierGroupId 1 (Credits)

                            Case "39" 'All areas except attics, bathrooms, closets, and attached structures; ParentModifierTypeId 28, ModifierGroupId 1 (Credits), CheckboxSelected true
                                _SprinklerSystem_AllExcept = m.CheckboxSelected
                            Case "40" 'All areas including attics, bathrooms, closets, and attached structures; ParentModifierTypeId 28, ModifierGroupId 1 (Credits), CheckboxSelected true
                                _SprinklerSystem_AllIncluding = m.CheckboxSelected
                            Case "50" 'Heated Building Surcharge Gas or Electric; ParentModifierTypeId 50, ModifierGroupId = 2 (Surcharges/Fees), CheckboxSelected true
                                _HeatedBuildingSurchargeGasElectric = m.CheckboxSelected
                            Case "51" 'Heated Building Surcharge Other; ParentModifierTypeId 51, ModifierGroupId = 2 (Surcharges/Fees), CheckboxSelected true
                                _HeatedBuildingSurchargeOther = m.CheckboxSelected
                            Case "52" 'Exposed Insulation Surcharge; ParentModifierTypeId 52, ModifierGroupId = 2 (Surcharges/Fees), CheckboxSelected true
                                _ExposedInsulationSurcharge = m.CheckboxSelected

                            Case "67" 'added 7/28/2015 for Farm e2Value; logic originally came from QuickQuoteLocation.ParseThruModifiers
                                Dim pvModifierText As String = m.ModifierOptionDescription
                                Dim pvId As String = ""
                                Dim pvArchStyle As String = ""
                                Dim foundPvId As Boolean = False
                                Dim foundPvArchStyle As Boolean = False
                                pvHelper.SplitPropertyValuationModifierText(pvModifierText, pvId, pvArchStyle, foundPvId, foundPvArchStyle)
                                If pvId IsNot Nothing AndAlso IsNumeric(pvId) = True Then
                                    Dim pvhc As New QuickQuotePropertyValuationHelperClass
                                    Dim pvLoadErrorMsg As String = ""
                                    pvhc.LoadPropertyValuationFromDatabase(pvId, _PropertyValuation, pvLoadErrorMsg) 'may also need to check environment and maybe other things to make sure it's the correct record
                                    If pvLoadErrorMsg <> "" Then
                                        'may not use
                                    Else 'added 8/26/2014 to reset if originated from a different environment; moving logic below so ArchitecturalStyle can still be pulled
                                        'If _PropertyValuation IsNot Nothing AndAlso UCase(_PropertyValuation.db_environment) <> UCase(helper.Environment) Then
                                        '    _PropertyValuation.Dispose()
                                        '    _PropertyValuation = New QuickQuotePropertyValuation
                                        'End If
                                    End If
                                    '7/28/2015 note: commented out following code; will need to un-comment if ArchitecturalStyle is added as Building property
                                    'If _PropertyValuation IsNot Nothing AndAlso _PropertyValuation.Request IsNot Nothing AndAlso _PropertyValuation.Request.ArchitecturalStyle <> "" Then
                                    '    '8/21/2014 note: may need to convert from property valuation value to location value
                                    '    '_ArchitecturalStyle = _PropertyValuation.Request.ArchitecturalStyle
                                    '    'updated 8/25/2014
                                    '    Dim optionAttributes As New List(Of QuickQuoteStaticDataAttribute)
                                    '    If _PropertyValuation.Vendor <> QuickQuotePropertyValuation.ValuationVendor.None OrElse _PropertyValuation.VendorEstimatorType <> QuickQuotePropertyValuation.ValuationVendorEstimatorType.None Then
                                    '        If _PropertyValuation.Vendor <> QuickQuotePropertyValuation.ValuationVendor.None Then
                                    '            Dim a1 As New QuickQuoteStaticDataAttribute
                                    '            a1.nvp_propertyName = QuickQuoteHelperClass.QuickQuotePropertyName.Vendor
                                    '            a1.nvp_value = System.Enum.GetName(GetType(QuickQuotePropertyValuation.ValuationVendor), _PropertyValuation.Vendor)
                                    '            optionAttributes.Add(a1)
                                    '        End If
                                    '        If _PropertyValuation.VendorEstimatorType <> QuickQuotePropertyValuation.ValuationVendorEstimatorType.None Then
                                    '            Dim a2 As New QuickQuoteStaticDataAttribute
                                    '            a2.nvp_propertyName = QuickQuoteHelperClass.QuickQuotePropertyName.VendorEstimatorType
                                    '            a2.nvp_value = System.Enum.GetName(GetType(QuickQuotePropertyValuation.ValuationVendorEstimatorType), _PropertyValuation.VendorEstimatorType)
                                    '            optionAttributes.Add(a2)
                                    '        End If
                                    '    End If
                                    '    _ArchitecturalStyle = qqHelper.GetRelatedStaticDataValueForOptionValue_MatchingOptionAttributes(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuotePropertyValuationRequest, QuickQuoteHelperClass.QuickQuotePropertyName.ArchitecturalStyle, optionAttributes, _PropertyValuation.Request.ArchitecturalStyle, QuickQuoteHelperClass.QuickQuotePropertyName.ArchitecturalStyle)
                                    'End If
                                    'added 8/26/2014 to reset if originated from a different environment
                                    If _PropertyValuation IsNot Nothing AndAlso UCase(_PropertyValuation.db_environment) <> UCase(helper.Environment) Then
                                        _PropertyValuation.Dispose()
                                        _PropertyValuation = New QuickQuotePropertyValuation
                                    End If
                                End If
                                '7/28/2015 note: commented out following code; will need to un-comment if ArchitecturalStyle is added as Building property
                                ''If pvArchStyle <> "" Then
                                ''updated 8/21/2014 to always use it if it's there... even if it's blank; shouldn't be there if not different than what's on PV
                                'If foundPvArchStyle = True Then
                                '    '8/21/2014 note: may need to convert from property valuation value to location value
                                '    _ArchitecturalStyle = pvArchStyle
                                'End If
                        End Select
                    End If
                Next
            End If
        End Sub
        Public Sub ParseThruOptionalCoverageEs() 'added 6/24/2015 for Farm
            If _OptionalCoverageEs IsNot Nothing AndAlso _OptionalCoverageEs.Count > 0 Then
                For Each e As QuickQuoteOptionalCoverageE In _OptionalCoverageEs
                    If _CanUseFarmBarnBuildingOptionalCoverageNumForOptionalCoverageEReconciliation = False Then
                        If e.HasValidFarmBarnBuildingOptionalCoverageNum = True Then
                            _CanUseFarmBarnBuildingOptionalCoverageNumForOptionalCoverageEReconciliation = True
                        End If
                    End If
                    e.CheckCoverage()
                Next
            End If
        End Sub
        Public Overrides Function ToString() As String 'added 6/29/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.Description <> "" Then
                    str = qqHelper.appendText(str, "Description: " & Me.Description, vbCrLf)
                End If
                If Me.YearBuilt <> "" Then
                    str = qqHelper.appendText(str, "YearBuilt: " & Me.YearBuilt, vbCrLf)
                End If
                If Me.FarmStructureTypeId <> "" Then
                    Dim fs As String = ""
                    fs = "FarmStructureTypeId: " & Me.FarmStructureTypeId
                    Dim fsType As String = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuoteHelperClass.QuickQuotePropertyName.FarmStructureTypeId, Me.FarmStructureTypeId)
                    If fsType <> "" Then
                        fs &= " (" & fsType & ")"
                    End If
                    str = qqHelper.appendText(str, fs, vbCrLf)
                End If
                If Me.BuildingCoverages IsNot Nothing AndAlso Me.BuildingCoverages.Count > 0 Then
                    str = qqHelper.appendText(str, Me.BuildingCoverages.Count.ToString & " Coverages", vbCrLf)
                End If
                If Me.Modifiers IsNot Nothing AndAlso Me.Modifiers.Count > 0 Then 'added 6/30/2015
                    str = qqHelper.appendText(str, Me.Modifiers.Count.ToString & " Modifiers", vbCrLf)
                End If
            Else
                str = "Nothing"
            End If
            Return str
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        'Protected Overridable Sub Dispose(disposing As Boolean)
        'updated 8/4/2014 w/ QuickQuoteBaseObject inheritance
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    If _PolicyId IsNot Nothing Then
                        _PolicyId = Nothing
                    End If
                    If _PolicyImageNum IsNot Nothing Then
                        _PolicyImageNum = Nothing
                    End If
                    If _Description IsNot Nothing Then
                        _Description = Nothing
                    End If
                    If _Program IsNot Nothing Then
                        _Program = Nothing
                    End If
                    If _ProgramAbbreviation IsNot Nothing Then
                        _ProgramAbbreviation = Nothing
                    End If
                    If _Classification IsNot Nothing Then
                        _Classification = Nothing
                    End If
                    If _ClassCode IsNot Nothing Then
                        _ClassCode = Nothing
                    End If
                    If _ClassificationTypeId IsNot Nothing Then
                        _ClassificationTypeId = Nothing
                    End If
                    If _Occupancy IsNot Nothing Then
                        _Occupancy = Nothing
                    End If
                    If _OccupancyId IsNot Nothing Then
                        _OccupancyId = Nothing
                    End If
                    If _Construction IsNot Nothing Then
                        _Construction = Nothing
                    End If
                    If _ConstructionId IsNot Nothing Then
                        _ConstructionId = Nothing
                    End If
                    If _AutoIncrease IsNot Nothing Then
                        _AutoIncrease = Nothing
                    End If
                    If _AutoIncreaseId IsNot Nothing Then
                        _AutoIncreaseId = Nothing
                    End If
                    If _PropertyDeductible IsNot Nothing Then
                        _PropertyDeductible = Nothing
                    End If
                    If _PropertyDeductibleId IsNot Nothing Then
                        _PropertyDeductibleId = Nothing
                    End If
                    If _Limit IsNot Nothing Then
                        _Limit = Nothing
                    End If
                    If _LimitQuotedPremium IsNot Nothing Then
                        _LimitQuotedPremium = Nothing
                    End If
                    If _Valuation IsNot Nothing Then
                        _Valuation = Nothing
                    End If
                    If _ValuationId IsNot Nothing Then
                        _ValuationId = Nothing
                    End If
                    If _IsBuildingValIncludedInBlanketRating <> Nothing Then
                        _IsBuildingValIncludedInBlanketRating = Nothing
                    End If
                    If _HasACVRoofing <> Nothing Then '3/9/2017 - BOP stuff
                        _HasACVRoofing = Nothing
                    End If
                    If _ACVRoofingQuotedPremium IsNot Nothing Then '3/9/2017 - BOP stuff
                        _ACVRoofingQuotedPremium = Nothing
                    End If
                    If _HasMineSubsidence <> Nothing Then
                        _HasMineSubsidence = Nothing
                    End If
                    If _MineSubsidenceQuotedPremium IsNot Nothing Then
                        _MineSubsidenceQuotedPremium = Nothing
                    End If
                    _MineSubsidence_IsDwellingStructure = Nothing 'added 11/8/2018 for CPR mine subsidence; covCodeId 20027; CheckBox: Mine Subsidence (BOP IN/IL, CPR IN/IL)
                    If _HasSprinklered <> Nothing Then
                        _HasSprinklered = Nothing
                    End If
                    If _PersonalPropertyLimit IsNot Nothing Then
                        _PersonalPropertyLimit = Nothing
                    End If
                    If _PersonalPropertyLimitQuotedPremium IsNot Nothing Then
                        _PersonalPropertyLimitQuotedPremium = Nothing
                    End If
                    If _ValuationMethod IsNot Nothing Then
                        _ValuationMethod = Nothing
                    End If
                    If _ValuationMethodId IsNot Nothing Then
                        _ValuationMethodId = Nothing
                    End If
                    If _IsValMethodIncludedInBlanketRating <> Nothing Then
                        _IsValMethodIncludedInBlanketRating = Nothing
                    End If

                    If _AccountsReceivableOnPremises IsNot Nothing Then
                        _AccountsReceivableOnPremises = Nothing
                    End If
                    If _AccountsReceivableOffPremises IsNot Nothing Then
                        _AccountsReceivableOffPremises = Nothing
                    End If
                    If _AccountsReceivableQuotedPremium IsNot Nothing Then
                        _AccountsReceivableQuotedPremium = Nothing
                    End If
                    If _ValuablePapersOnPremises IsNot Nothing Then
                        _ValuablePapersOnPremises = Nothing
                    End If
                    If _ValuablePapersOffPremises IsNot Nothing Then
                        _ValuablePapersOffPremises = Nothing
                    End If
                    If _ValuablePapersQuotedPremium IsNot Nothing Then
                        _ValuablePapersQuotedPremium = Nothing
                    End If
                    If _CondoCommercialUnitOwnersLimit IsNot Nothing Then
                        _CondoCommercialUnitOwnersLimit = Nothing
                    End If
                    If _CondoCommercialUnitOwnersLimitId IsNot Nothing Then
                        _CondoCommercialUnitOwnersLimitId = Nothing
                    End If
                    If _CondoCommercialUnitOwnersLimitQuotedPremium IsNot Nothing Then
                        _CondoCommercialUnitOwnersLimitQuotedPremium = Nothing
                    End If
                    If _HasOrdinanceOrLaw <> Nothing Then
                        _HasOrdinanceOrLaw = Nothing
                    End If
                    If _HasOrdOrLawUndamagedPortion <> Nothing Then
                        _HasOrdOrLawUndamagedPortion = Nothing
                    End If
                    If _OrdOrLawUndamagedPortionQuotedPremium IsNot Nothing Then
                        _OrdOrLawUndamagedPortionQuotedPremium = Nothing
                    End If
                    If _OrdOrLawDemoCostLimit IsNot Nothing Then
                        _OrdOrLawDemoCostLimit = Nothing
                    End If
                    If _OrdOrLawDemoCostLimitQuotedPremium IsNot Nothing Then
                        _OrdOrLawDemoCostLimitQuotedPremium = Nothing
                    End If
                    If _OrdOrLawIncreasedCostLimit IsNot Nothing Then
                        _OrdOrLawIncreasedCostLimit = Nothing
                    End If
                    If _OrdOrLawIncreaseCostLimitQuotedPremium IsNot Nothing Then
                        _OrdOrLawIncreaseCostLimitQuotedPremium = Nothing
                    End If
                    If _OrdOrLawDemoAndIncreasedCostLimit IsNot Nothing Then
                        _OrdOrLawDemoAndIncreasedCostLimit = Nothing
                    End If
                    If _OrdOrLawDemoAndIncreasedCostLimitQuotedPremium IsNot Nothing Then
                        _OrdOrLawDemoAndIncreasedCostLimitQuotedPremium = Nothing
                    End If
                    If _HasSpoilage <> Nothing Then
                        _HasSpoilage = Nothing
                    End If
                    If _SpoilageQuotedPremium IsNot Nothing Then
                        _SpoilageQuotedPremium = Nothing
                    End If
                    If _SpoilagePropertyClassification IsNot Nothing Then
                        _SpoilagePropertyClassification = Nothing
                    End If
                    If _SpoilagePropertyClassificationId IsNot Nothing Then
                        _SpoilagePropertyClassificationId = Nothing
                    End If
                    If _SpoilageTotalLimit IsNot Nothing Then
                        _SpoilageTotalLimit = Nothing
                    End If
                    If _IsSpoilageRefrigerationMaintenanceAgreement <> Nothing Then
                        _IsSpoilageRefrigerationMaintenanceAgreement = Nothing
                    End If
                    If _IsSpoilageBreakdownOrContamination <> Nothing Then
                        _IsSpoilageBreakdownOrContamination = Nothing
                    End If
                    If _IsSpoilagePowerOutage <> Nothing Then
                        _IsSpoilagePowerOutage = Nothing
                    End If

                    If _BuildingCoverages IsNot Nothing Then
                        If _BuildingCoverages.Count > 0 Then
                            For Each cov As QuickQuoteCoverage In _BuildingCoverages
                                cov.Dispose()
                                cov = Nothing
                            Next
                            _BuildingCoverages.Clear()
                        End If
                        _BuildingCoverages = Nothing
                    End If

                    If _BuildingClassifications IsNot Nothing Then
                        If _BuildingClassifications.Count > 0 Then
                            For Each cls As QuickQuoteClassification In _BuildingClassifications
                                cls.Dispose()
                                cls = Nothing
                            Next
                            _BuildingClassifications.Clear()
                        End If
                        _BuildingClassifications = Nothing
                    End If

                    If _HasBusinessMasterEnhancement <> Nothing Then
                        _HasBusinessMasterEnhancement = Nothing
                    End If

                    If _ProtectionClassId IsNot Nothing Then
                        _ProtectionClassId = Nothing
                    End If
                    If _ProtectionClass IsNot Nothing Then
                        _ProtectionClass = Nothing
                    End If

                    If _YearBuilt IsNot Nothing Then
                        _YearBuilt = Nothing
                    End If


                    'If _NumberOfSoleProprietors IsNot Nothing Then
                    '    _NumberOfSoleProprietors = Nothing
                    'End If
                    'If _NumberOfCorporateOfficers IsNot Nothing Then
                    '    _NumberOfCorporateOfficers = Nothing
                    'End If
                    'If _NumberOfPartners IsNot Nothing Then
                    '    _NumberOfPartners = Nothing
                    'End If
                    If _NumberOfOfficersAndPartnersAndInsureds IsNot Nothing Then
                        _NumberOfOfficersAndPartnersAndInsureds = Nothing
                    End If
                    If _EmployeePayroll IsNot Nothing Then
                        _EmployeePayroll = Nothing
                    End If
                    If _AnnualReceipts IsNot Nothing Then
                        _AnnualReceipts = Nothing
                    End If

                    If _SquareFeet IsNot Nothing Then
                        _SquareFeet = Nothing
                    End If

                    'If _CentralHeatElectric <> Nothing Then
                    '    _CentralHeatElectric = Nothing
                    'End If
                    'If _CentralHeatGas <> Nothing Then
                    '    _CentralHeatGas = Nothing
                    'End If
                    'If _CentralHeatOil <> Nothing Then
                    '    _CentralHeatOil = Nothing
                    'End If
                    'If _CentralHeatOther <> Nothing Then
                    '    _CentralHeatOther = Nothing
                    'End If
                    'If _CentralHeatOtherDescription IsNot Nothing Then
                    '    _CentralHeatOtherDescription = Nothing
                    'End If
                    'If _CentralHeatUpdateTypeId IsNot Nothing Then
                    '    _CentralHeatUpdateTypeId = Nothing
                    'End If
                    'If _CentralHeatUpdateYear IsNot Nothing Then
                    '    _CentralHeatUpdateYear = Nothing
                    'End If
                    'If _ImprovementsDescription IsNot Nothing Then
                    '    _ImprovementsDescription = Nothing
                    'End If
                    'If _Electric100Amp <> Nothing Then
                    '    _Electric100Amp = Nothing
                    'End If
                    'If _Electric120Amp <> Nothing Then
                    '    _Electric120Amp = Nothing
                    'End If
                    'If _Electric200Amp <> Nothing Then
                    '    _Electric200Amp = Nothing
                    'End If
                    'If _Electric60Amp <> Nothing Then
                    '    _Electric60Amp = Nothing
                    'End If
                    'If _ElectricBurningUnit <> Nothing Then
                    '    _ElectricBurningUnit = Nothing
                    'End If
                    'If _ElectricCircuitBreaker <> Nothing Then
                    '    _ElectricCircuitBreaker = Nothing
                    'End If
                    'If _ElectricFuses <> Nothing Then
                    '    _ElectricFuses = Nothing
                    'End If
                    'If _ElectricSpaceHeater <> Nothing Then
                    '    _ElectricSpaceHeater = Nothing
                    'End If
                    'If _ElectricUpdateTypeId IsNot Nothing Then
                    '    _ElectricUpdateTypeId = Nothing
                    'End If
                    'If _ElectricUpdateYear IsNot Nothing Then
                    '    _ElectricUpdateYear = Nothing
                    'End If
                    'If _PlumbingCopper <> Nothing Then
                    '    _PlumbingCopper = Nothing
                    'End If
                    'If _PlumbingGalvanized <> Nothing Then
                    '    _PlumbingGalvanized = Nothing
                    'End If
                    'If _PlumbingPlastic <> Nothing Then
                    '    _PlumbingPlastic = Nothing
                    'End If
                    'If _PlumbingUpdateTypeId IsNot Nothing Then
                    '    _PlumbingUpdateTypeId = Nothing
                    'End If
                    'If _PlumbingUpdateYear IsNot Nothing Then
                    '    _PlumbingUpdateYear = Nothing
                    'End If
                    'If _RoofAsphaltShingle <> Nothing Then
                    '    _RoofAsphaltShingle = Nothing
                    'End If
                    'If _RoofMetal <> Nothing Then
                    '    _RoofMetal = Nothing
                    'End If
                    'If _RoofOther <> Nothing Then
                    '    _RoofOther = Nothing
                    'End If
                    'If _RoofOtherDescription IsNot Nothing Then
                    '    _RoofOtherDescription = Nothing
                    'End If
                    'If _RoofSlate <> Nothing Then
                    '    _RoofSlate = Nothing
                    'End If
                    'If _RoofUpdateTypeId IsNot Nothing Then
                    '    _RoofUpdateTypeId = Nothing
                    'End If
                    'If _RoofUpdateYear IsNot Nothing Then
                    '    _RoofUpdateYear = Nothing
                    'End If
                    'If _RoofWood <> Nothing Then
                    '    _RoofWood = Nothing
                    'End If
                    'If _SupplementalHeatBurningUnit <> Nothing Then
                    '    _SupplementalHeatBurningUnit = Nothing
                    'End If
                    'If _SupplementalHeatFireplace <> Nothing Then
                    '    _SupplementalHeatFireplace = Nothing
                    'End If
                    'If _SupplementalHeatFireplaceInsert <> Nothing Then
                    '    _SupplementalHeatFireplaceInsert = Nothing
                    'End If
                    'If _SupplementalHeatNA <> Nothing Then
                    '    _SupplementalHeatNA = Nothing
                    'End If
                    'If _SupplementalHeatSolidFuel <> Nothing Then
                    '    _SupplementalHeatSolidFuel = Nothing
                    'End If
                    'If _SupplementalHeatSpaceHeater <> Nothing Then
                    '    _SupplementalHeatSpaceHeater = Nothing
                    'End If
                    'If _SupplementalHeatUpdateTypeId IsNot Nothing Then
                    '    _SupplementalHeatUpdateTypeId = Nothing
                    'End If
                    'If _SupplementalHeatUpdateYear IsNot Nothing Then
                    '    _SupplementalHeatUpdateYear = Nothing
                    'End If
                    'If _WindowsUpdateTypeId IsNot Nothing Then
                    '    _WindowsUpdateTypeId = Nothing
                    'End If
                    'If _WindowsUpdateYear IsNot Nothing Then
                    '    _WindowsUpdateYear = Nothing
                    'End If
                    If _Updates IsNot Nothing Then 'added 7/31/2013
                        _Updates.Dispose()
                        _Updates = Nothing
                    End If

                    If _AdditionalInterests IsNot Nothing Then
                        If _AdditionalInterests.Count > 0 Then
                            For Each ai As QuickQuoteAdditionalInterest In _AdditionalInterests
                                ai.Dispose()
                                ai = Nothing
                            Next
                            _AdditionalInterests.Clear()
                        End If
                        _AdditionalInterests = Nothing
                    End If

                    If _HasBarbersProfessionalLiability <> Nothing Then
                        _HasBarbersProfessionalLiability = Nothing
                    End If
                    If _BarbersProfessionalLiabilityFullTimeEmpNum IsNot Nothing Then
                        _BarbersProfessionalLiabilityFullTimeEmpNum = Nothing
                    End If
                    If _BarbersProfessionalLiabilityPartTimeEmpNum IsNot Nothing Then
                        _BarbersProfessionalLiabilityPartTimeEmpNum = Nothing
                    End If
                    If _HasBeauticiansProfessionalLiability <> Nothing Then
                        _HasBeauticiansProfessionalLiability = Nothing
                    End If
                    If _BeauticiansProfessionalLiabilityFullTimeEmpNum IsNot Nothing Then
                        _BeauticiansProfessionalLiabilityFullTimeEmpNum = Nothing
                    End If
                    If _BeauticiansProfessionalLiabilityPartTimeEmpNum IsNot Nothing Then
                        _BeauticiansProfessionalLiabilityPartTimeEmpNum = Nothing
                    End If
                    If _HasFuneralDirectorsProfessionalLiability <> Nothing Then
                        _HasFuneralDirectorsProfessionalLiability = Nothing
                    End If
                    If _FuneralDirectorsProfessionalLiabilityEmpNum IsNot Nothing Then
                        _FuneralDirectorsProfessionalLiabilityEmpNum = Nothing
                    End If
                    If _HasPrintersProfessionalLiability <> Nothing Then
                        _HasPrintersProfessionalLiability = Nothing
                    End If
                    If _PrintersProfessionalLiabilityLocNum IsNot Nothing Then
                        _PrintersProfessionalLiabilityLocNum = Nothing
                    End If
                    If _HasSelfStorageFacility <> Nothing Then
                        _HasSelfStorageFacility = Nothing
                    End If
                    If _SelfStorageFacilityLimit IsNot Nothing Then
                        _SelfStorageFacilityLimit = Nothing
                    End If
                    If _HasVeterinariansProfessionalLiability <> Nothing Then
                        _HasVeterinariansProfessionalLiability = Nothing
                    End If
                    If _VeterinariansProfessionalLiabilityEmpNum IsNot Nothing Then
                        _VeterinariansProfessionalLiabilityEmpNum = Nothing
                    End If
                    If _HasPharmacistProfessionalLiability <> Nothing Then '3/9/2017 - BOP stuff
                        _HasPharmacistProfessionalLiability = Nothing
                    End If
                    If _PharmacistAnnualGrossSales IsNot Nothing Then '3/9/2017 - BOP stuff
                        _PharmacistAnnualGrossSales = Nothing
                    End If
                    If _HasOpticalAndHearingAidProfessionalLiability <> Nothing Then
                        _HasOpticalAndHearingAidProfessionalLiability = Nothing
                    End If
                    If _OpticalAndHearingAidProfessionalLiabilityEmpNum IsNot Nothing Then
                        _OpticalAndHearingAidProfessionalLiabilityEmpNum = Nothing
                    End If
                    If _HasMotelCoverage <> Nothing Then '3/9/2017 - BOP stuff
                        _HasMotelCoverage = Nothing
                    End If
                    If _MotelCoveragePerGuestLimitId IsNot Nothing Then '3/9/2017 - BOP stuff
                        _MotelCoveragePerGuestLimitId = Nothing
                    End If
                    If _MotelCoverageSafeDepositDeductibleId IsNot Nothing Then '3/9/2017 - BOP stuff
                        _MotelCoverageSafeDepositDeductibleId = Nothing
                    End If
                    If _MotelCoverageSafeDepositLimitId IsNot Nothing Then '3/9/2017 - BOP stuff
                        _MotelCoverageSafeDepositLimitId = Nothing
                    End If

                    If _ClassificationCode IsNot Nothing Then
                        _ClassificationCode.Dispose()
                        _ClassificationCode = Nothing
                    End If
                    If _EarthquakeBuildingClassificationTypeId IsNot Nothing Then
                        _EarthquakeBuildingClassificationTypeId = Nothing
                    End If

                    If _EarthquakeApplies <> Nothing Then
                        _EarthquakeApplies = Nothing
                    End If
                    If _CauseOfLossTypeId IsNot Nothing Then
                        _CauseOfLossTypeId = Nothing
                    End If
                    If _CauseOfLossType IsNot Nothing Then
                        _CauseOfLossType = Nothing
                    End If
                    If _DeductibleId IsNot Nothing Then
                        _DeductibleId = Nothing
                    End If
                    If _Deductible IsNot Nothing Then
                        _Deductible = Nothing
                    End If
                    If _CoinsuranceTypeId IsNot Nothing Then
                        _CoinsuranceTypeId = Nothing
                    End If
                    If _CoinsuranceType IsNot Nothing Then
                        _CoinsuranceType = Nothing
                    End If
                    'If _ValuationMethodTypeId IsNot Nothing Then
                    '    _ValuationMethodTypeId = Nothing
                    'End If
                    'If _ValuationMethodType IsNot Nothing Then
                    '    _ValuationMethodType = Nothing
                    'End If
                    If _RatingTypeId IsNot Nothing Then
                        _RatingTypeId = Nothing
                    End If
                    If _RatingType IsNot Nothing Then
                        _RatingType = Nothing
                    End If
                    If _InflationGuardTypeId IsNot Nothing Then
                        _InflationGuardTypeId = Nothing
                    End If
                    If _InflationGuardType IsNot Nothing Then
                        _InflationGuardType = Nothing
                    End If

                    If _ScheduledCoverages IsNot Nothing Then
                        If _ScheduledCoverages.Count > 0 Then
                            For Each c As QuickQuoteScheduledCoverage In _ScheduledCoverages
                                c.Dispose()
                                c = Nothing
                            Next
                            _ScheduledCoverages.Clear()
                        End If
                        _ScheduledCoverages = Nothing
                    End If
                    If _PersPropCov_PersonalPropertyLimit IsNot Nothing Then
                        _PersPropCov_PersonalPropertyLimit = Nothing
                    End If
                    If _PersPropCov_PropertyTypeId IsNot Nothing Then
                        _PersPropCov_PropertyTypeId = Nothing
                    End If
                    If _PersPropCov_PropertyType IsNot Nothing Then
                        _PersPropCov_PropertyType = Nothing
                    End If
                    If _PersPropCov_RiskTypeId IsNot Nothing Then
                        _PersPropCov_RiskTypeId = Nothing
                    End If
                    If _PersPropCov_RiskType IsNot Nothing Then
                        _PersPropCov_RiskType = Nothing
                    End If
                    If _PersPropCov_EarthquakeApplies <> Nothing Then
                        _PersPropCov_EarthquakeApplies = Nothing
                    End If
                    If _PersPropCov_RatingTypeId IsNot Nothing Then
                        _PersPropCov_RatingTypeId = Nothing
                    End If
                    If _PersPropCov_RatingType IsNot Nothing Then
                        _PersPropCov_RatingType = Nothing
                    End If
                    If _PersPropCov_CauseOfLossTypeId IsNot Nothing Then
                        _PersPropCov_CauseOfLossTypeId = Nothing
                    End If
                    If _PersPropCov_CauseOfLossType IsNot Nothing Then
                        _PersPropCov_CauseOfLossType = Nothing
                    End If
                    If _PersPropCov_DeductibleId IsNot Nothing Then
                        _PersPropCov_DeductibleId = Nothing
                    End If
                    If _PersPropCov_Deductible IsNot Nothing Then
                        _PersPropCov_Deductible = Nothing
                    End If
                    If _PersPropCov_CoinsuranceTypeId IsNot Nothing Then
                        _PersPropCov_CoinsuranceTypeId = Nothing
                    End If
                    If _PersPropCov_CoinsuranceType IsNot Nothing Then
                        _PersPropCov_CoinsuranceType = Nothing
                    End If
                    If _PersPropCov_ValuationId IsNot Nothing Then
                        _PersPropCov_ValuationId = Nothing
                    End If
                    If _PersPropCov_Valuation IsNot Nothing Then
                        _PersPropCov_Valuation = Nothing
                    End If
                    If _PersPropCov_QuotedPremium IsNot Nothing Then
                        _PersPropCov_QuotedPremium = Nothing
                    End If
                    If _PersPropCov_ClassificationCode IsNot Nothing Then
                        _PersPropCov_ClassificationCode.Dispose()
                        _PersPropCov_ClassificationCode = Nothing
                    End If
                    If _PersPropOfOthers_PersonalPropertyLimit IsNot Nothing Then
                        _PersPropOfOthers_PersonalPropertyLimit = Nothing
                    End If
                    'If _PersPropOfOthers_PropertyTypeId IsNot Nothing Then'defaulting
                    '    _PersPropOfOthers_PropertyTypeId = Nothing
                    'End If
                    'If _PersPropOfOthers_PropertyType IsNot Nothing Then
                    '    _PersPropOfOthers_PropertyType = Nothing
                    'End If
                    If _PersPropOfOthers_RiskTypeId IsNot Nothing Then
                        _PersPropOfOthers_RiskTypeId = Nothing
                    End If
                    If _PersPropOfOthers_RiskType IsNot Nothing Then
                        _PersPropOfOthers_RiskType = Nothing
                    End If
                    If _PersPropOfOthers_EarthquakeApplies <> Nothing Then
                        _PersPropOfOthers_EarthquakeApplies = Nothing
                    End If
                    If _PersPropOfOthers_RatingTypeId IsNot Nothing Then
                        _PersPropOfOthers_RatingTypeId = Nothing
                    End If
                    If _PersPropOfOthers_RatingType IsNot Nothing Then
                        _PersPropOfOthers_RatingType = Nothing
                    End If
                    If _PersPropOfOthers_CauseOfLossTypeId IsNot Nothing Then
                        _PersPropOfOthers_CauseOfLossTypeId = Nothing
                    End If
                    If _PersPropOfOthers_CauseOfLossType IsNot Nothing Then
                        _PersPropOfOthers_CauseOfLossType = Nothing
                    End If
                    If _PersPropOfOthers_DeductibleId IsNot Nothing Then
                        _PersPropOfOthers_DeductibleId = Nothing
                    End If
                    If _PersPropOfOthers_Deductible IsNot Nothing Then
                        _PersPropOfOthers_Deductible = Nothing
                    End If
                    If _PersPropOfOthers_CoinsuranceTypeId IsNot Nothing Then
                        _PersPropOfOthers_CoinsuranceTypeId = Nothing
                    End If
                    If _PersPropOfOthers_CoinsuranceType IsNot Nothing Then
                        _PersPropOfOthers_CoinsuranceType = Nothing
                    End If
                    If _PersPropOfOthers_ValuationId IsNot Nothing Then
                        _PersPropOfOthers_ValuationId = Nothing
                    End If
                    If _PersPropOfOthers_Valuation IsNot Nothing Then
                        _PersPropOfOthers_Valuation = Nothing
                    End If
                    If _PersPropOfOthers_QuotedPremium IsNot Nothing Then
                        _PersPropOfOthers_QuotedPremium = Nothing
                    End If
                    If _PersPropOfOthers_ClassificationCode IsNot Nothing Then
                        _PersPropOfOthers_ClassificationCode.Dispose()
                        _PersPropOfOthers_ClassificationCode = Nothing
                    End If

                    If _BusinessIncomeCov_Limit IsNot Nothing Then
                        _BusinessIncomeCov_Limit = Nothing
                    End If
                    If _BusinessIncomeCov_CoinsuranceTypeId IsNot Nothing Then
                        _BusinessIncomeCov_CoinsuranceTypeId = Nothing
                    End If
                    If _BusinessIncomeCov_CoinsuranceType IsNot Nothing Then
                        _BusinessIncomeCov_CoinsuranceType = Nothing
                    End If
                    If _BusinessIncomeCov_MonthlyPeriodTypeId IsNot Nothing Then
                        _BusinessIncomeCov_MonthlyPeriodTypeId = Nothing
                    End If
                    If _BusinessIncomeCov_MonthlyPeriodType IsNot Nothing Then
                        _BusinessIncomeCov_MonthlyPeriodType = Nothing
                    End If
                    If _BusinessIncomeCov_BusinessIncomeTypeId IsNot Nothing Then
                        _BusinessIncomeCov_BusinessIncomeTypeId = Nothing
                    End If
                    If _BusinessIncomeCov_BusinessIncomeType IsNot Nothing Then
                        _BusinessIncomeCov_BusinessIncomeType = Nothing
                    End If
                    If _BusinessIncomeCov_RiskTypeId IsNot Nothing Then
                        _BusinessIncomeCov_RiskTypeId = Nothing
                    End If
                    If _BusinessIncomeCov_RiskType IsNot Nothing Then
                        _BusinessIncomeCov_RiskType = Nothing
                    End If
                    If _BusinessIncomeCov_EarthquakeApplies <> Nothing Then
                        _BusinessIncomeCov_EarthquakeApplies = Nothing
                    End If
                    If _BusinessIncomeCov_RatingTypeId IsNot Nothing Then
                        _BusinessIncomeCov_RatingTypeId = Nothing
                    End If
                    If _BusinessIncomeCov_RatingType IsNot Nothing Then
                        _BusinessIncomeCov_RatingType = Nothing
                    End If
                    If _BusinessIncomeCov_CauseOfLossTypeId IsNot Nothing Then
                        _BusinessIncomeCov_CauseOfLossTypeId = Nothing
                    End If
                    If _BusinessIncomeCov_CauseOfLossType IsNot Nothing Then
                        _BusinessIncomeCov_CauseOfLossType = Nothing
                    End If
                    If _BusinessIncomeCov_QuotedPremium IsNot Nothing Then
                        _BusinessIncomeCov_QuotedPremium = Nothing
                    End If
                    If _BusinessIncomeCov_ClassificationCode IsNot Nothing Then
                        _BusinessIncomeCov_ClassificationCode.Dispose()
                        _BusinessIncomeCov_ClassificationCode = Nothing
                    End If
                    If _BusinessIncomeCov_WaitingPeriodTypeId IsNot Nothing Then
                        _BusinessIncomeCov_WaitingPeriodTypeId = Nothing
                    End If
                    If _BusinessIncomeCov_WaitingPeriodType IsNot Nothing Then
                        _BusinessIncomeCov_WaitingPeriodType = Nothing
                    End If

                    If _Building_BusinessIncome_Group1_Rate IsNot Nothing Then
                        _Building_BusinessIncome_Group1_Rate = Nothing
                    End If
                    If _Building_BusinessIncome_Group2_Rate IsNot Nothing Then
                        _Building_BusinessIncome_Group2_Rate = Nothing
                    End If
                    If _PersonalProperty_Group1_Rate IsNot Nothing Then
                        _PersonalProperty_Group1_Rate = Nothing
                    End If
                    If _PersonalProperty_Group2_Rate IsNot Nothing Then
                        _PersonalProperty_Group2_Rate = Nothing
                    End If
                    If _Building_BusinessIncome_Group1_LossCost IsNot Nothing Then
                        _Building_BusinessIncome_Group1_LossCost = Nothing
                    End If
                    If _Building_BusinessIncome_Group2_LossCost IsNot Nothing Then
                        _Building_BusinessIncome_Group2_LossCost = Nothing
                    End If
                    If _PersonalProperty_Group1_LossCost IsNot Nothing Then
                        _PersonalProperty_Group1_LossCost = Nothing
                    End If
                    If _PersonalProperty_Group2_LossCost IsNot Nothing Then
                        _PersonalProperty_Group2_LossCost = Nothing
                    End If

                    If _CoverageFormTypeId IsNot Nothing Then
                        _CoverageFormTypeId = Nothing
                    End If
                    If _CoverageFormType IsNot Nothing Then
                        _CoverageFormType = Nothing
                    End If

                    If _CPR_Covs_TotalBuildingPremium IsNot Nothing Then
                        _CPR_Covs_TotalBuildingPremium = Nothing
                    End If

                    'If _PersPropCov_EarthquakeRateGradeTypeId IsNot Nothing Then
                    '    _PersPropCov_EarthquakeRateGradeTypeId = Nothing
                    'End If
                    'If _PersPropOfOthers_EarthquakeRateGradeTypeId IsNot Nothing Then
                    '    _PersPropOfOthers_EarthquakeRateGradeTypeId = Nothing
                    'End If
                    If _PersonalProperty_EarthquakeRateGradeTypeId IsNot Nothing Then
                        _PersonalProperty_EarthquakeRateGradeTypeId = Nothing
                    End If

                    If _NumberOfStories IsNot Nothing Then
                        _NumberOfStories = Nothing
                    End If

                    If _EarthquakeQuotedPremium IsNot Nothing Then
                        _EarthquakeQuotedPremium = Nothing
                    End If
                    If _PersPropCov_EarthquakeQuotedPremium IsNot Nothing Then
                        _PersPropCov_EarthquakeQuotedPremium = Nothing
                    End If
                    If _PersPropOfOthers_EarthquakeQuotedPremium IsNot Nothing Then
                        _PersPropOfOthers_EarthquakeQuotedPremium = Nothing
                    End If
                    If _BusinessIncomeCov_EarthquakeQuotedPremium IsNot Nothing Then
                        _BusinessIncomeCov_EarthquakeQuotedPremium = Nothing
                    End If
                    If _CPR_Covs_TotalBuilding_EQ_Premium IsNot Nothing Then
                        _CPR_Covs_TotalBuilding_EQ_Premium = Nothing
                    End If
                    If _CPR_Covs_TotalBuilding_With_EQ_Premium IsNot Nothing Then
                        _CPR_Covs_TotalBuilding_With_EQ_Premium = Nothing
                    End If
                    If _CPR_BuildingLimit_With_EQ_QuotedPremium IsNot Nothing Then
                        _CPR_BuildingLimit_With_EQ_QuotedPremium = Nothing
                    End If
                    If _CPR_PersPropCov_With_EQ_QuotedPremium IsNot Nothing Then
                        _CPR_PersPropCov_With_EQ_QuotedPremium = Nothing
                    End If
                    If _CPR_PersPropOfOthers_With_EQ_QuotedPremium IsNot Nothing Then
                        _CPR_PersPropOfOthers_With_EQ_QuotedPremium = Nothing
                    End If
                    If _CPR_BusinessIncomeCov_With_EQ_QuotedPremium IsNot Nothing Then
                        _CPR_BusinessIncomeCov_With_EQ_QuotedPremium = Nothing
                    End If
                    If _CPR_BuildingLimit_EarthquakeBuildingClassificationPercentage IsNot Nothing Then
                        _CPR_BuildingLimit_EarthquakeBuildingClassificationPercentage = Nothing
                    End If
                    If _CPR_PersPropCov_EarthquakeBuildingClassificationPercentage IsNot Nothing Then
                        _CPR_PersPropCov_EarthquakeBuildingClassificationPercentage = Nothing
                    End If
                    If _CPR_PersPropOfOthers_EarthquakeBuildingClassificationPercentage IsNot Nothing Then
                        _CPR_PersPropOfOthers_EarthquakeBuildingClassificationPercentage = Nothing
                    End If
                    If _CPR_BusinessIncomeCov_EarthquakeBuildingClassificationPercentage IsNot Nothing Then
                        _CPR_BusinessIncomeCov_EarthquakeBuildingClassificationPercentage = Nothing
                    End If
                    If _OptionalTheftDeductibleId IsNot Nothing Then
                        _OptionalTheftDeductibleId = Nothing
                    End If
                    If _OptionalTheftDeductible IsNot Nothing Then
                        _OptionalTheftDeductible = Nothing
                    End If
                    If _OptionalWindstormOrHailDeductibleId IsNot Nothing Then
                        _OptionalWindstormOrHailDeductibleId = Nothing
                    End If
                    If _OptionalWindstormOrHailDeductible IsNot Nothing Then
                        _OptionalWindstormOrHailDeductible = Nothing
                    End If
                    If _PersPropCov_OptionalTheftDeductibleId IsNot Nothing Then
                        _PersPropCov_OptionalTheftDeductibleId = Nothing
                    End If
                    If _PersPropCov_OptionalTheftDeductible IsNot Nothing Then
                        _PersPropCov_OptionalTheftDeductible = Nothing
                    End If
                    If _PersPropCov_OptionalWindstormOrHailDeductibleId IsNot Nothing Then
                        _PersPropCov_OptionalWindstormOrHailDeductibleId = Nothing
                    End If
                    If _PersPropCov_OptionalWindstormOrHailDeductible IsNot Nothing Then
                        _PersPropCov_OptionalWindstormOrHailDeductible = Nothing
                    End If
                    If _PersPropOfOthers_OptionalTheftDeductibleId IsNot Nothing Then
                        _PersPropOfOthers_OptionalTheftDeductibleId = Nothing
                    End If
                    If _PersPropOfOthers_OptionalTheftDeductible IsNot Nothing Then
                        _PersPropOfOthers_OptionalTheftDeductible = Nothing
                    End If
                    If _PersPropOfOthers_OptionalWindstormOrHailDeductibleId IsNot Nothing Then
                        _PersPropOfOthers_OptionalWindstormOrHailDeductibleId = Nothing
                    End If
                    If _PersPropOfOthers_OptionalWindstormOrHailDeductible IsNot Nothing Then
                        _PersPropOfOthers_OptionalWindstormOrHailDeductible = Nothing
                    End If
                    If _PersPropCov_DoesYardRateApplyTypeId IsNot Nothing Then
                        _PersPropCov_DoesYardRateApplyTypeId = Nothing
                    End If
                    If _PersPropOfOthers_DoesYardRateApplyTypeId IsNot Nothing Then
                        _PersPropOfOthers_DoesYardRateApplyTypeId = Nothing
                    End If

                    If _FeetToFireHydrant IsNot Nothing Then
                        _FeetToFireHydrant = Nothing
                    End If
                    If _MilesToFireDepartment IsNot Nothing Then
                        _MilesToFireDepartment = Nothing
                    End If

                    If _PersPropCov_IncludedInBlanketCoverage <> Nothing Then
                        _PersPropCov_IncludedInBlanketCoverage = Nothing
                    End If
                    If _PersPropOfOthers_IncludedInBlanketCoverage <> Nothing Then
                        _PersPropOfOthers_IncludedInBlanketCoverage = Nothing
                    End If
                    If _BusinessIncomeCov_IncludedInBlanketCoverage <> Nothing Then
                        _BusinessIncomeCov_IncludedInBlanketCoverage = Nothing
                    End If

                    '3/9/2017 - BOP stuff
                    If _HasRestaurantEndorsement <> Nothing Then
                        _HasRestaurantEndorsement = Nothing
                    End If
                    If _HasApartmentBuildings <> Nothing Then
                        _HasApartmentBuildings = Nothing
                    End If
                    If _NumberOfLocationsWithApartments IsNot Nothing Then
                        _NumberOfLocationsWithApartments = Nothing
                    End If
                    If _HasTenantAutoLegalLiability <> Nothing Then
                        _HasTenantAutoLegalLiability = Nothing
                    End If
                    If _TenantAutoLegalLiabilityDeductibleId IsNot Nothing Then
                        _TenantAutoLegalLiabilityDeductibleId = Nothing
                    End If
                    If _TenantAutoLegalLiabilityLimitOfLiabilityId IsNot Nothing Then
                        _TenantAutoLegalLiabilityLimitOfLiabilityId = Nothing
                    End If
                    If _HasCustomerAutoLegalLiability <> Nothing Then
                        _HasCustomerAutoLegalLiability = Nothing
                    End If
                    If _CustomerAutoLegalLiabilityDeductibleId IsNot Nothing Then
                        _CustomerAutoLegalLiabilityDeductibleId = Nothing
                    End If
                    If _CustomerAutoLegalLiabilityLimitOfLiabilityId IsNot Nothing Then
                        _CustomerAutoLegalLiabilityLimitOfLiabilityId = Nothing
                    End If
                    If _HasFineArts <> Nothing Then
                        _HasFineArts = Nothing
                    End If
                    If _LiquorLiabilityClassCodeTypeId IsNot Nothing Then
                        _LiquorLiabilityClassCodeTypeId = Nothing
                    End If
                    If _LiquorLiabilityAnnualGrossPackageSalesReceipts IsNot Nothing Then
                        _LiquorLiabilityAnnualGrossPackageSalesReceipts = Nothing
                    End If
                    If _LiquorLiabilityAnnualGrossAlcoholSalesReceipts IsNot Nothing Then
                        _LiquorLiabilityAnnualGrossAlcoholSalesReceipts = Nothing
                    End If
                    If _LiquorLiabilityAggregateLimit IsNot Nothing Then
                        _LiquorLiabilityAggregateLimit = Nothing
                    End If
                    If _HasLiquorLiability <> Nothing Then
                        _HasLiquorLiability = Nothing
                    End If

                    'added 2/18/2014
                    If _HasConvertedClassifications <> Nothing Then
                        _HasConvertedClassifications = Nothing
                    End If
                    If _HasConvertedCoverages <> Nothing Then
                        _HasConvertedCoverages = Nothing
                    End If
                    If _HasConvertedScheduledCoverages <> Nothing Then
                        _HasConvertedScheduledCoverages = Nothing
                    End If

                    If _PremiumFullterm IsNot Nothing Then 'added 4/2/2014
                        _PremiumFullterm = Nothing
                    End If

                    If _FarmBarnBuildingNum IsNot Nothing Then 'added 4/23/2014 for reconciliation
                        _FarmBarnBuildingNum = Nothing
                    End If
                    If _CanUseAdditionalInterestNumForAdditionalInterestReconciliation <> Nothing Then 'added 4/29/2014
                        _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = Nothing
                    End If
                    'added 10/18/2018 for multi-state
                    qqHelper.DisposeString(_FarmBarnBuildingNum_MasterPart)
                    qqHelper.DisposeString(_FarmBarnBuildingNum_CGLPart)
                    qqHelper.DisposeString(_FarmBarnBuildingNum_CPRPart)
                    qqHelper.DisposeString(_FarmBarnBuildingNum_CIMPart)
                    qqHelper.DisposeString(_FarmBarnBuildingNum_CRMPart)
                    qqHelper.DisposeString(_FarmBarnBuildingNum_GARPart)

                    If _Modifiers IsNot Nothing Then 'added 10/16/2014
                        If _Modifiers.Count > 0 Then
                            For Each m As QuickQuoteModifier In _Modifiers
                                m.Dispose()
                                m = Nothing
                            Next
                            _Modifiers.Clear()
                        End If
                        _Modifiers = Nothing
                    End If

                    _CanUseScheduledCoverageNumForScheduledCoverageReconciliation = Nothing 'added 1/22/2015
                    'added 1/22/2015
                    If _PersPropCov_ScheduledCoverageNum IsNot Nothing Then
                        _PersPropCov_ScheduledCoverageNum = Nothing
                    End If
                    If _PersPropOfOthers_ScheduledCoverageNum IsNot Nothing Then
                        _PersPropOfOthers_ScheduledCoverageNum = Nothing
                    End If

                    'added 2/9/2014 for CIM
                    qqHelper.DisposeString(_ComputerHardwareLimit) 'cov also has CoverageBasisTypeId set to 1
                    qqHelper.DisposeString(_ComputerHardwareRate)
                    qqHelper.DisposeString(_ComputerHardwareQuotedPremium)
                    qqHelper.DisposeString(_ComputerProgramsApplicationsAndMediaLimit) 'cov also has CoverageBasisTypeId set to 1
                    qqHelper.DisposeString(_ComputerProgramsApplicationsAndMediaRate)
                    qqHelper.DisposeString(_ComputerProgramsApplicationsAndMediaQuotedPremium)
                    qqHelper.DisposeString(_ComputerBusinessIncomeLimit) 'cov also has CoverageBasisTypeId set to 1
                    qqHelper.DisposeString(_ComputerBusinessIncomeRate)
                    qqHelper.DisposeString(_ComputerBusinessIncomeQuotedPremium)
                    'added 3/16/2015
                    If _FineArtsScheduledItems IsNot Nothing Then
                        If _FineArtsScheduledItems.Count > 0 Then
                            For Each fa As QuickQuoteFineArtsScheduledItem In _FineArtsScheduledItems
                                fa.Dispose()
                                fa = Nothing
                            Next
                            _FineArtsScheduledItems.Clear()
                        End If
                        _FineArtsScheduledItems = Nothing
                    End If
                    'added 3/26/2015
                    If _ScheduledSigns IsNot Nothing Then
                        If _ScheduledSigns.Count > 0 Then
                            For Each ss As QuickQuoteScheduledSign In _ScheduledSigns
                                ss.Dispose()
                                ss = Nothing
                            Next
                            _ScheduledSigns.Clear()
                        End If
                        _ScheduledSigns = Nothing
                    End If
                    qqHelper.DisposeString(_UnscheduledSignsLimit)
                    qqHelper.DisposeString(_UnscheduledSignsQuotedPremium)

                    'added 6/15/2015 for Farm
                    qqHelper.DisposeString(_Dimensions)
                    qqHelper.DisposeString(_FarmStructureTypeId) 'static data
                    qqHelper.DisposeString(_FarmTypeId) 'static data
                    qqHelper.DisposeString(_NumberOfSolidFuelBurningUnits)
                    qqHelper.DisposeString(_VacancyFromDate)
                    qqHelper.DisposeString(_VacancyToDate)
                    _HasConvertedModifiers = Nothing
                    _SprinklerSystem_AllExcept = Nothing
                    _SprinklerSystem_AllIncluding = Nothing
                    _HeatedBuildingSurchargeGasElectric = Nothing
                    _HeatedBuildingSurchargeOther = Nothing
                    _ExposedInsulationSurcharge = Nothing
                    qqHelper.DisposeString(_E_Farm_Limit)
                    qqHelper.DisposeString(_E_Farm_DeductibleLimitId) 'static data
                    qqHelper.DisposeString(_E_Farm_QuotedPremium)
                    'added 6/16/2015 for Farm
                    qqHelper.DisposeString(_HouseholdContentsLimit)
                    qqHelper.DisposeString(_HouseholdContentsQuotedPremium)
                    'added 6/24/2015 for Farm
                    If _OptionalCoverageEs IsNot Nothing Then
                        If _OptionalCoverageEs.Count > 0 Then
                            For Each e As QuickQuoteOptionalCoverageE In _OptionalCoverageEs
                                e.Dispose()
                                e = Nothing
                            Next
                            _OptionalCoverageEs.Clear()
                        End If
                        _OptionalCoverageEs = Nothing
                    End If
                    _CanUseFarmBarnBuildingOptionalCoverageNumForOptionalCoverageEReconciliation = Nothing
                    _HasConvertedOptionalCoverageEs = Nothing
                    'added 7/28/2015 for Farm e2Value
                    If _PropertyValuation IsNot Nothing Then
                        _PropertyValuation.Dispose()
                        _PropertyValuation = Nothing
                    End If

                    'added 2/17/2017
                    _UseBuildingClassificationPropertiesToCreateOneItemInList = Nothing
                    'added 2/20/2017
                    _CanUseClassificationNumForClassificationReconciliation = Nothing

                    'added 7/8/2017
                    If _BuildingPersonalProperties IsNot Nothing Then
                        If _BuildingPersonalProperties.Count > 0 Then
                            For Each bpp As QuickQuoteBuildingPersonalProperty In _BuildingPersonalProperties
                                If bpp IsNot Nothing Then
                                    bpp.Dispose()
                                    bpp = Nothing
                                End If
                            Next
                            _BuildingPersonalProperties.Clear()
                        End If
                        _BuildingPersonalProperties = Nothing
                    End If
                    qqHelper.DisposeString(_TotalPersonalPropertyNormalQuotedPremium)
                    qqHelper.DisposeString(_TotalPersonalPropertyOfOthersQuotedPremium)
                    qqHelper.DisposeString(_TotalPersonalPropertyCombinedQuotedPremium)
                    qqHelper.DisposeString(_TotalPersonalPropertyNormalLimit)
                    qqHelper.DisposeString(_TotalPersonalPropertyOfOthersLimit)
                    qqHelper.DisposeString(_TotalPersonalPropertyNormalEarthquakeQuotedPremium)
                    qqHelper.DisposeString(_TotalPersonalPropertyOfOthersEarthquakeQuotedPremium)
                    qqHelper.DisposeString(_TotalPersonalPropertyCombinedEarthquakeQuotedPremium)
                    qqHelper.DisposeString(_TotalPersonalPropertyNormalWithEarthquakeQuotedPremium)
                    qqHelper.DisposeString(_TotalPersonalPropertyOfOthersWithEarthquakeQuotedPremium)
                    qqHelper.DisposeString(_TotalPersonalPropertyCombinedWithEarthquakeQuotedPremium)
                    _TotalPersonalPropertyNormalCount = Nothing
                    _TotalPersonalPropertyOfOthersCount = Nothing

                    'added 10/26/2018
                    _HasWindHailACVSettlement = Nothing 'covCodeId 20040; BOP IN/IL
                    qqHelper.DisposeString(_WindHailACVSettlementQuotedPremium) 'covCodeId 20040; BOP IN/IL
                    _HasLimitationsOnRoofSurfacing = Nothing 'covCodeId 80542; BOP IL only
                    qqHelper.DisposeString(_LimitationsOnRoofSurfacingQuotedPremium) 'covCodeId 80542; BOP IL only
                    _HasACVRoofSurfacing = Nothing 'covCodeId 80543; BOP IL only
                    qqHelper.DisposeString(_ACVRoofSurfacingQuotedPremium) 'covCodeId 80543; BOP IL only
                    _ExcludeCosmeticDamage = Nothing 'covCodeId 80544; BOP IL only
                    qqHelper.DisposeString(_ExcludeCosmeticDamageQuotedPremium) 'covCodeId 80544; BOP IL only

                    qqHelper.DisposeString(_DetailStatusCode) 'added 5/15/2019

                    qqHelper.DisposeString(_EarthquakeDeductibleId)
                    qqHelper.DisposeString(_EarthquakeDeductible)
                    qqHelper.DisposeString(_BuildingCov_EarthquakeDeductibleId)
                    qqHelper.DisposeString(_BuildingCov_EarthquakeDeductible)
                    'qqHelper.DisposeString(_BusinessIncomeCov_EarthquakeDeductibleId)
                    qqHelper.DisposeString(_BusinessIncomeCov_EarthquakeDeductible)
                    qqHelper.DisposeString(_PersPropCov_EarthquakeDeductibleId)
                    qqHelper.DisposeString(_PersPropCov_EarthquakeDeductible)
                    qqHelper.DisposeString(_PersPropOfOthers_EarthquakeDeductibleId)
                    qqHelper.DisposeString(_PersPropOfOthers_EarthquakeDeductible)
                    qqHelper.DisposeString(_PersPropCov_InflationGuardTypeId)
                    qqHelper.DisposeString(_PersPropCov_InflationGuardType)
                    qqHelper.DisposeString(_PersPropOfOthers_InflationGuardTypeId)
                    qqHelper.DisposeString(_PersPropOfOthers_InflationGuardType)
                    qqHelper.DisposeString(_OwnerOccupiedPercentageId)
                    qqHelper.DisposeString(_OwnerOccupiedPercentage)

                    MyBase.Dispose() 'added 8/4/2014

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
        'updated 8/4/2014 w/ QuickQuoteBaseObject inheritance
        Public Overrides Sub Dispose() 'Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
