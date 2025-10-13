Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods
Imports helper = QuickQuote.CommonMethods.QuickQuoteHelperClass 'added 8/26/2014

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store location information
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()>
    Public Class QuickQuoteLocation
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass
        Dim pvHelper As New QuickQuotePropertyValuationHelperClass 'added 7/28/2015 for e2Value; moving logic from QuickQuoteLocation to QuickQuotePropertyValuationHelperClass

        Private _Description As String 'might be able to set using one of these nodes: DescriptionBusiness, DescriptionOccupancy, or LegalDescription
        'Private _StreetNum As String
        'Private _StreetName As String
        'Private _City As String
        'Private _State As String
        'Private _Zip As String
        'Private _County As String
        Private _Address As QuickQuoteAddress
        Private _ProtectionClass As String
        Private _ProtectionClassId As String
        Private _NumberOfPools As String

        Private _PropertyDeductibleId As String

        '3/9/2017 - BOP stuff
        Private _NumberOfAmusementAreas As String
        Private _NumberOfPlaygrounds As String
        Private _PoolsQuotedPremium As String
        Private _AmusementsQuotedPremium As String
        Private _PlaygroundsQuotedPremium As String

        Private _EquipmentBreakdownDeductible As String
        Private _EquipmentBreakdownDeductibleId As String
        Private _EquipmentBreakdownDeductibleQuotedPremium As String
        Private _EquipmentBreakdownDeductibleMinimumRequiredByClassCode As String '3/9/2017 - BOP stuff
        Private _MoneySecuritiesOnPremises As String
        Private _MoneySecuritiesOffPremises As String
        Private _MoneySecuritiesQuotedPremium As String
        Private _MoneySecuritiesQuotedPremium_OnPremises As String
        Private _MoneySecuritiesQuotedPremium_OffPremises As String
        Private _OutdoorSignsLimit As String
        Private _OutdoorSignsQuotedPremium As String

        Private _Buildings As Generic.List(Of QuickQuoteBuilding)
        Private _LocationCoverages As Generic.List(Of QuickQuoteCoverage)

        Private _GLClassifications As Generic.List(Of QuickQuoteGLClassification)
        Private _Classifications As Generic.List(Of QuickQuoteClassification)

        'added name 7/10/2012 (for WC)
        Private _Name As QuickQuoteName

        'added 8/28/2012 for GL dec section
        Private _GL_PremisesQuotedPremium As String
        Private _GL_ProductsQuotedPremium As String

        'added 9/10/2012 PM for validation purposes
        Private _HasBuilding As Boolean
        Private _HasClassification As Boolean

        'added 9/17/2012 for CPR
        Private _CauseOfLossTypeId As String 'example 3 = Special Form Including Theft (CauseOfLossType table)
        Private _CauseOfLossType As String
        Private _DeductibleId As String 'example 8 = 500
        Private _Deductible As String
        Private _CoinsuranceTypeId As String 'example 5 = 80% (CoinsuranceType table)
        Private _CoinsuranceType As String
        Private _ValuationMethodTypeId As String 'example 1 = Replacement Cost
        Private _ValuationMethodType As String

        'added 9/27/2012 for CPR
        Private _EquipmentBreakdownOccupancyTypeId As String
        Private _ClassificationCode As QuickQuoteClassificationCode

        'added 1/29/2013 for Protection Class Lookup stuff
        Private _FeetToFireHydrant As String
        Private _MilesToFireDepartment As String

        Private _ScheduledCoverages As Generic.List(Of QuickQuoteScheduledCoverage) 'added 3/19/2013 for CPR (Property In The Open)
        Private _PropertyInTheOpenRecords As Generic.List(Of QuickQuotePropertyInTheOpenRecord) 'added 3/19/2013 for CPR

        '*added for 3/27/2013 for CPR defaults (stored in xml but not valid; no spot in Diamond)
        Private _WindstormOrHailPercentageDeductibleId As String
        Private _WindstormOrHailPercentageDeductible As String
        Private _WindstormOrHailMinimumDollarDeductibleId As String
        Private _WindstormOrHailMinimumDollarDeductible As String
        Private _EarthquakeApplies As Boolean

        'added 4/17/2013 for CPR to total up Property in the Open coverage premiums
        Private _PropertyInTheOpenRecordsTotal_QuotedPremium As String
        Private _PropertyInTheOpenRecordsTotal_EQ_Premium As String
        Private _PropertyInTheOpenRecordsTotal_QuotedPremium_With_EQ As String

        'added 7/26/2013 for HOM
        Private _Acreage As String
        Private _CondoRentedTypeId As String '0=N/A; 1=Yes; 2=No
        Private _ConstructionTypeId As String '-1=N/A; 0=None; 1=Frame; etc. (may need matching ConstructionType variable/property)
        Private _DeductibleLimitId As String 'may need matching Limit variable/property
        Private _WindHailDeductibleLimitId As String 'may need matching Limit variable/property
        Private _DayEmployees As Boolean
        Private _DaytimeOccupancy As Boolean
        Private _FamilyUnitsId As String 'may need matching FamilyUnits variable/property
        Private _FireDepartmentDistanceId As String 'may need matching FireDepartmentDistance variable/property
        Private _FireHydrantDistanceId As String 'may need matching FireHydrantDistance variable/property
        Private _FormTypeId As String 'may need matching FormType variable/property
        Private _FoundationTypeId As String 'may need matching FoundationType variable/property
        Private _LastCostEstimatorDate As String
        Private _MarketValue As String
        Private _NumberOfFamiliesId As String 'may need matching NumberOfFamilies variable/property
        Private _OccupancyCodeId As String 'may need matching OccupancyCode variable/property
        Private _PrimaryResidence As Boolean
        Private _ProgramTypeId As String 'may need matching ProgramType variable/property
        'added 7/30/2013 for HOM
        Private _NumberOfApartments As String
        Private _NumberOfSolidFuelBurningUnits As String
        Private _RebuildCost As String
        Private _Remarks As String
        Private _SquareFeet As String
        Private _StructureTypeId As String 'may need matching StructureType variable/property
        Private _YearBuilt As String
        Private _A_Dwelling_Limit As String
        Private _A_Dwelling_LimitIncreased As String
        Private _A_Dwelling_LimitIncluded As String
        Private _A_Dwelling_Calc As String 'added 12/4/2014
        Private _B_OtherStructures_Limit As String
        Private _B_OtherStructures_LimitIncreased As String
        Private _B_OtherStructures_LimitIncluded As String
        Private _B_OtherStructures_Calc As String 'added 12/10/2014
        Private _C_PersonalProperty_Limit As String
        Private _C_PersonalProperty_LimitIncreased As String
        Private _C_PersonalProperty_LimitIncluded As String
        Private _C_PersonalProperty_Calc As String 'added 12/10/2014
        Private _D_LossOfUse_Limit As String
        Private _D_LossOfUse_LimitIncreased As String
        Private _D_LossOfUse_LimitIncluded As String
        Private _D_LossOfUse_Calc As String 'added 12/10/2014
        Private _AdditionalInterests As Generic.List(Of QuickQuoteAdditionalInterest)
        'added 7/31/2013 for HOM
        Private _Updates As QuickQuoteUpdatesRecord 'same object is now part of Building so it can be used instead of multiple properties
        Private _Modifiers As List(Of QuickQuoteModifier)
        Private _MultiPolicyDiscount As Boolean
        Private _MatureHomeownerDiscount As Boolean
        Private _NewHomeDiscount As Boolean
        Private _SelectMarketCredit As Boolean
        Private _BurglarAlarm_LocalAlarmSystem As Boolean
        Private _BurglarAlarm_CentralStationAlarmSystem As Boolean
        Private _FireSmokeAlarm_LocalAlarmSystem As Boolean
        Private _FireSmokeAlarm_CentralStationAlarmSystem As Boolean
        Private _FireSmokeAlarm_SmokeAlarm As Boolean
        Private _SprinklerSystem_AllExcept As Boolean
        Private _SprinklerSystem_AllIncluding As Boolean
        Private _TrampolineSurcharge As Boolean
        Private _WoodOrFuelBurningApplianceSurcharge As Boolean
        Private _SwimmingPoolHotTubSurcharge As Boolean 'added 7/28/2014
        'added 8/1/2013 for HOM
        Private _SectionCoverages As List(Of QuickQuoteSectionCoverage)
        Private _Exclusions As List(Of QuickQuoteExclusion)
        Private _InlandMarines As List(Of QuickQuoteInlandMarine)
        Private _RvWatercrafts As List(Of QuickQuoteRvWatercraft)
        'added 8/14/2013 for HOM
        Private _SectionICoverages As List(Of QuickQuoteSectionICoverage)
        Private _SectionIICoverages As List(Of QuickQuoteSectionIICoverage)
        Private _SectionIAndIICoverages As List(Of QuickQuoteSectionIAndIICoverage)
        'added 8/15/2013 for HOM
        Private _PolicyUnderwritings As Generic.List(Of QuickQuotePolicyUnderwriting)
        'added 8/15/2013 for DFR
        Private _NumberOfDaysRented As String
        Private _NumberOfUnitsId As String
        Private _UsageTypeId As String 'may need matching UsageType variable/property
        Private _VacancyFromDate As String
        Private _VacancyToDate As String

        'added 2/18/2014
        Private _HasConvertedCoverages As Boolean
        Private _HasConvertedModifiers As Boolean
        Private _HasConvertedScheduledCoverages As Boolean
        Private _HasConvertedSectionCoverages As Boolean

        'added 4/2/2014
        Private _PremiumFullterm As String
        Private _BuildingsTotal_PremiumFullTerm As String

        'added 4/23/2014 for reconciliation
        Private _LocationNum As String
        Private _HasLocationAddressChanged As Boolean
        Private _CanUseFarmBarnBuildingNumForBuildingReconciliation As Boolean
        Private _CanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean 'added 4/29/2014
        'added 10/17/2018 for multi-state
        Private _LocationNum_MasterPart As String
        Private _LocationNum_CGLPart As String
        Private _LocationNum_CPRPart As String
        Private _LocationNum_CIMPart As String
        Private _LocationNum_CRMPart As String
        Private _LocationNum_GARPart As String
        'added 10/18/2018 for multi-state
        Private _CanUseFarmBarnBuildingNumForMasterPartBuildingReconciliation As Boolean
        Private _CanUseFarmBarnBuildingNumForCGLPartBuildingReconciliation As Boolean
        Private _CanUseFarmBarnBuildingNumForCPRPartBuildingReconciliation As Boolean
        Private _CanUseFarmBarnBuildingNumForCIMPartBuildingReconciliation As Boolean
        Private _CanUseFarmBarnBuildingNumForCRMPartBuildingReconciliation As Boolean
        Private _CanUseFarmBarnBuildingNumForGARPartBuildingReconciliation As Boolean

        'added 7/29/2014 for HOM (specific to mobile home formTypes... FormTypeId 6 = ML-2 - Mobile Home Owner Occupied; 7 = ML-4 - Mobile Home Tenant Occupied
        Private _TheftDeductibleLimitId As String 'int; corresponds to CoverageCodeId 20034; static data list
        Private _HomeInPark As Boolean
        Private _MobileHomeConsecutiveMonthsOccupied As String 'int
        Private _MobileHomeCostNew As String 'dec
        Private _MobileHomeLength As String 'int
        Private _MobileHomeMake As String
        Private _MobileHomeModel As String
        Private _MobileHomeParkName As String
        Private _MobileHomePurchasePrice As String 'dec
        Private _MobileHomeSkirtTypeId As String 'int; static data list
        Private _MobileHomeTieDownTypeId As String 'int; static data list
        Private _MobileHomeVIN As String
        Private _MobileHomeWidth As String 'int
        Private _PermanentFoundation As Boolean

        Private _PropertyValuation As QuickQuotePropertyValuation 'added 8/5/2014 for e2Value
        Private _ArchitecturalStyle As String 'added 8/14/2014 for e2Value; corresponds to prop w/ the same name in QuickQuotePropertyValuationRequest

        'added 10/13/2014 for surcharge premiums... added as modifiers, but prem comes back in coverage... may also need to set Checkbox to True on Coverage
        Dim _SwimmingPoolHotTubSurchargeQuotePremium As String
        Dim _TrampolineSurchargeQuotePremium As String
        Dim _WoodOrFuelBurningApplianceSurchargeQuotePremium As String
        'added 10/14/2014 for other HOM coverage premiums
        Private _DeductibleQuotedPremium As String
        Private _WindHailDeductibleQuotedPremium As String
        Private _A_Dwelling_QuotedPremium As String
        Private _B_OtherStructures_QuotedPremium As String
        Private _C_PersonalProperty_QuotedPremium As String
        Private _D_LossOfUse_QuotedPremium As String
        Private _TheftDeductibleQuotedPremium As String

        'added 10/14/2014 for reconciliation
        Private _CanUseExclusionNumForExclusionReconciliation As Boolean
        Private _CanUseInlandMarineNumForInlandMarineReconciliation As Boolean
        Private _CanUsePolicyUnderwritingNumForPolicyUnderwritingReconciliation As Boolean
        Private _CanUseRvWatercraftNumForRvWatercraftReconciliation As Boolean
        'Private _CanUseSectionCoverageNumForSectionCoverageReconciliation As Boolean 'removed 10/29/2018
        Private _CanUseDiamondNumForSectionCoverageReconciliationGroup As QuickQuoteCanUseDiamondNumFlagGroup 'added 10/29/2018

        'added 10/29/2014 for HOM
        Private _TerritoryNumber As String

        'added 11/17/2014 for HOM
        Private _InlandMarinesTotal_Premium As String
        Private _InlandMarinesTotal_CoveragePremium As String
        Private _RvWatercraftsTotal_Premium As String
        Private _RvWatercraftsTotal_CoveragesPremium As String

        Private _CanUseScheduledCoverageNumForScheduledCoverageReconciliation As Boolean 'added 1/22/2015

        'added 2/25/2015 for Farm
        Private _IncidentalDwellingCoverages As List(Of QuickQuoteCoverage)
        Private _HasConvertedIncidentalDwellingCoverages As Boolean
        'added 2/26/2015
        Private _Acreages As List(Of QuickQuoteAcreage)
        Private _CanUseAcreageNumForAcreageReconciliation As Boolean
        Private _IncomeLosses As List(Of QuickQuoteIncomeLoss)
        Private _CanUseLossOfIncomeNumForIncomeLossReconciliation As Boolean
        Private _HasConvertedIncomeLosses As Boolean
        Private _ResidentNames As List(Of QuickQuoteResidentName)
        Private _CanUseResidentNumForResidentNameReconciliation As Boolean
        'added 3/4/2015
        Private _DwellingTypeId As String 'static data; example had 22 (Type 1)
        Private _FarmTypeBees As Boolean
        Private _FarmTypeDairy As Boolean
        Private _FarmTypeFeedLot As Boolean
        Private _FarmTypeFieldCrops As Boolean
        Private _FarmTypeFlowers As Boolean
        Private _FarmTypeFruits As Boolean
        Private _FarmTypeFurbearingAnimals As Boolean
        Private _FarmTypeGreenhouses As Boolean
        Private _FarmTypeHobby As Boolean
        Private _FarmTypeHorse As Boolean
        Private _FarmTypeLivestock As Boolean
        Private _FarmTypeMushrooms As Boolean
        Private _FarmTypeNurseryStock As Boolean
        Private _FarmTypeNuts As Boolean
        Private _FarmTypeOtherDescription As String
        Private _FarmTypePoultry As Boolean
        Private _FarmTypeSod As Boolean
        Private _FarmTypeSwine As Boolean
        Private _FarmTypeTobacco As Boolean
        Private _FarmTypeTurkey As Boolean
        Private _FarmTypeVegetables As Boolean
        Private _FarmTypeVineyards As Boolean
        Private _FarmTypeWorms As Boolean
        Private _LegalDescription As String
        Private _Owns As Boolean
        Private _RoofExclusion As Boolean
        'added 5/20/2015
        Private _AcreageOnly As Boolean
        'added 6/11/2015
        Private _HobbyFarmCredit As Boolean
        'added 6/12/2015
        Private _FireDepartmentAlarm As Boolean
        Private _PoliceDepartmentTheftAlarm As Boolean
        Private _BurglarAlarm_CentralAlarmSystem As Boolean
        Private _FireSmokeAlarm_CentralAlarmSystem As Boolean
        'added 9/23/2015
        Private _Farm_L_Liability_QuotedPremium As String
        Private _Farm_M_Medical_Payments_QuotedPremium As String

        'added 11/2/2015 for DFR
        Private _A_Dwelling_EC_QuotedPremium As String '70223
        Private _C_Contents_EC_QuotedPremium As String '70224
        Private _D_and_E_EC_QuotedPremium As String '80106
        Private _B_OtherStructures_EC_QuotedPremium As String '70218
        Private _A_Dwelling_VMM_QuotedPremium As String '70225
        Private _C_Contents_VMM_QuotedPremium As String '70226
        Private _D_and_E_VMM_QuotedPremium As String '80107
        Private _B_OtherStructures_VMM_QuotedPremium As String '70220

        'added 9/19/2016 for Verisk Protection Class
        Private _FireDistrictName As String
        Private _ProtectionClassSystemGeneratedId As String '9/24/2016 note: need to update static data
        'added 9/24/2016 for Verisk Protection Class
        Private _PPCMatchTypeId As String 'need to add to static data in case we need to look it up

        'added 10/7/2016 to compare before/after address
        Private _OriginalAddress As QuickQuoteAddress

        'added 10/14/2016 for Verisk Protection Class
        Private _FireStationDistance As String

        'added 2/20/2017
        Private _CanUseClassificationNumForClassificationReconciliation As Boolean

        '3/9/2017 - BOP stuff
        Private _HasTenantAutoLegalLiability As Boolean
        Private _TenantAutoLegalLiabilityLimitOfLiabilityId As String
        Private _TenantAutoLegalLiabilityDeductibleId As String
        Private _TenantAutoLegalLiabilityLimitOfLiability As String
        Private _TenantAutoLegalLiabilityDeductible As String
        Private _TenantAutoLegalLiabilityQuotedPremium As String
        Private _HasCustomerAutoLegalLiability As Boolean
        Private _CustomerAutoLegalLiabilityLimitOfLiabilityId As String
        Private _CustomerAutoLegalLiabilityDeductibleId As String
        Private _CustomerAutoLegalLiabilityLimitOfLiability As String
        Private _CustomerAutoLegalLiabilityDeductible As String
        Private _CustomerAutoLegalLiabilityQuotedPremium As String
        Private _hasFineArts As Boolean
        Private _FineArtsQuotedPremium As String

        'added 5/8/2017 for GAR
        Private _LiabilityQuotedPremium As String 'covCodeId 10111
        Private _LiabilityAggregateLiabilityIncrementTypeId As String 'covDetail; covCodeId 10111
        Private _LiabilityCoverageLimitId As String 'covCodeId 10111
        Private _MedicalPaymentsQuotedPremium As String 'covCodeId 10112
        Private _MedicalPaymentsCoverageLimitId As String 'covCodeId 10112
        Private _HasUninsuredUnderinsuredMotoristBIandPD As Boolean 'covCodeId 10113
        Private _UninsuredUnderinsuredMotoristBIandPDQuotedPremium As String 'covCodeId 10113; may not be populated
        Private _UninsuredUnderinsuredMotoristBIandPDCoverageLimitId As String 'covCodeId 10113
        Private _UninsuredUnderinsuredMotoristBIandPDDeductibleId As String 'covCodeId 10113
        Private _PhysicalDamageOtherThanCollisionStandardOpenLotsQuotedPremium As String 'covCodeId 10116
        Private _PhysicalDamageOtherThanCollisionStandardOpenLotsManualLimitAmount As String 'covCodeId 10116; added 5/12/2017
        'Private _PhysicalDamageOtherThanCollisionStandardOpenLotsDeductibleCategoryTypeId As String 'covDetail; covCodeId 10116; removed 5/15/2017 since it's the same for all PD OtherThanCollision covs; will use 1 new field
        'Private _PhysicalDamageOtherThanCollisionStandardOpenLotsOtherThanCollisionTypeId As String 'covDetail; covCodeId 10116; removed 5/15/2017 since it's the same for all PD OtherThanCollision covs; will use 1 new field
        'Private _PhysicalDamageOtherThanCollisionStandardOpenLotsDeductibleId As String 'covCodeId 10116; removed 5/15/2017 since it's the same for all PD OtherThanCollision covs; will use 1 new field
        Private _HasDealersBlanketCollision As Boolean 'covCodeId 10120
        Private _DealersBlanketCollisionQuotedPremium As String 'covCodeId 10120
        Private _DealersBlanketCollisionDeductibleId As String 'covCodeId 10120
        Private _GarageKeepersOtherThanCollisionQuotedPremium As String 'covCodeId 10086; may not be populated
        Private _GarageKeepersOtherThanCollisionManualLimitAmount As String 'covCodeId 10086
        Private _GarageKeepersOtherThanCollisionDeductibleCategoryTypeId As String 'covCodeId 10086
        Private _GarageKeepersOtherThanCollisionTypeId As String 'covCodeId 10086
        Private _GarageKeepersOtherThanCollisionDeductibleId As String 'covCodeId 10086
        Private _GarageKeepersOtherThanCollisionBasisTypeId As String 'added 7/14/2017; covDetail; covCodeId 10086
        Private _GarageKeepersCollisionQuotedPremium As String 'covCodeId 10087; may not be populated
        Private _GarageKeepersCollisionManualLimitAmount As String 'covCodeId 10087
        Private _GarageKeepersCollisionDeductibleId As String 'covCodeId 10087
        Private _GarageKeepersCollisionBasisTypeId As String 'added 7/15/2017; covDetail; covCodeId 10087
        Private _HasGarageKeepersCoverageExtensions As Boolean 'covCodeId 10126
        Private _GarageKeepersCoverageExtensionsQuotedPremium As String 'covCodeId 10126; may not be populated
        'added 5/11/2017 for GAR
        Private _ClassIIEmployees25AndOlder As String
        Private _ClassIIEmployeesUnderAge25 As String
        Private _ClassIOtherEmployees As String
        Private _ClassIRegularEmployees As String
        Private _ClassificationTypeId As String
        Private _NumberOfEmployees As String
        Private _Payroll As String
        Private _UninsuredUnderinsuredMotoristBIandPDNumberOfPlates As String 'covCodeId 10113; covDetail
        'added 5/15/2017 for GAR
        Private _PhysicalDamageOtherThanCollisionBuildingQuotedPremium As String 'covCodeId 10115
        Private _PhysicalDamageOtherThanCollisionBuildingManualLimitAmount As String 'covCodeId 10115
        Private _PhysicalDamageOtherThanCollisionNonStandardOpenLotsQuotedPremium As String 'covCodeId 10117
        Private _PhysicalDamageOtherThanCollisionNonStandardOpenLotsManualLimitAmount As String 'covCodeId 10117
        Private _PhysicalDamageOtherThanCollisionMiscellaneousBuildingsQuotedPremium As String 'covCodeId 10118
        Private _PhysicalDamageOtherThanCollisionMiscellaneousBuildingsManualLimitAmount As String 'covCodeId 10118
        Private _PhysicalDamageOtherThanCollisionMiscellaneousOpenLotsQuotedPremium As String 'covCodeId 10119
        Private _PhysicalDamageOtherThanCollisionMiscellaneousOpenLotsManualLimitAmount As String 'covCodeId 10119
        Private _PhysicalDamageOtherThanCollisionTotalQuotedPremium As String 'SUM of covCodeIds 10115, 10116, 10117, 10118, and 10119
        Private _PhysicalDamageOtherThanCollisionTotalManualLimitAmount As String 'SUM of covCodeIds 10115, 10116, 10117, 10118, and 10119
        Private _PhysicalDamageOtherThanCollisionDeductibleCategoryTypeId As String 'covDetail; covCodeIds 10115, 10116, 10117, 10118, and 10119
        Private _PhysicalDamageOtherThanCollisionTypeId As String 'covDetail; covCodeIds 10115, 10116, 10117, 10118, and 10119
        Private _PhysicalDamageOtherThanCollisionDeductibleId As String 'covCodeIds 10115, 10116, 10117, 10118, and 10119

        'added 11/21/2017 for HOM 2018 Upgrade
        '---------------------------------
        Private _NumberOfUnitsInFireDivision As String
        Private _PrimaryPolicyNumber As String
        Private _sprinklerSystem As Boolean
        '-------------------------------------

        'added 10/27/2017 for MBR Equipment Breakdown
        Private _EquipmentBreakdown_MBR_PollutantCleanupRemovalCoverageLimitId As String 'covCodeId 80513; has CoverageBasisTypeId 1
        Private _EquipmentBreakdown_MBR_PollutantCleanupRemovalDeductibleId As String 'covCodeId 80513; has CoverageBasisTypeId 1
        Private _EquipmentBreakdown_MBR_PollutantCleanupRemovalQuotedPremium As String 'covCodeId 80513; has CoverageBasisTypeId 1
        Private _EquipmentBreakdown_MBR_RefrigerantContaminationCoverageLimitId As String 'covCodeId 80514; has CoverageBasisTypeId 1
        Private _EquipmentBreakdown_MBR_RefrigerantContaminationDeductibleId As String 'covCodeId 80514; has CoverageBasisTypeId 1
        Private _EquipmentBreakdown_MBR_RefrigerantContaminationQuotedPremium As String 'covCodeId 80514; has CoverageBasisTypeId 1
        Private _EquipmentBreakdown_MBR_SpoilageCoverageLimitId As String 'covCodeId 80515; has CoverageBasisTypeId 1
        Private _EquipmentBreakdown_MBR_SpoilageDeductibleId As String 'covCodeId 80515; has CoverageBasisTypeId 1
        Private _EquipmentBreakdown_MBR_SpoilageQuotedPremium As String 'covCodeId 80515; has CoverageBasisTypeId 1
        Private _EquipmentBreakdown_MBR_UnderwrittenRateQuotedPremium As String 'covCodeId 80521
        Private _EquipmentBreakdown_MBR_TotalQuotedPremium As String 'SUM of covCodeIds 21042, 80513, 80514, 80515, 80521
        'added 10/30/2017
        Private _EquipmentBreakdown_AdjustmentFactor As String 'covCodeId 21042; 11/14/2017 note: covDetail; decimal
        'added 11/1/2017
        Private _EquipmentBreakdownDeductibleIdBackup As String 'covCodeId 21042

        'added 8/2/2018
        Private _QuoteStateTakenFrom As helper.QuickQuoteState

        'added 1/16/2019
        Private _DisplayNum As Integer
        Private _OriginalDisplayNum As Integer
        Private _OkayToUseDisplayNum As QuickQuoteHelperClass.WhenToSetType

        'Private _parent As QuickQuoteObject

        Private _DetailStatusCode As String 'added 5/15/2019

        Private _AddedDate As String
        Private _EffectiveDate As String
        Private _LastModifiedDate As String
        Private _PCAdded_Date As String
        Private _AddedImageNum As String

        Public Property Description As String
            Get
                Return _Description
            End Get
            Set(value As String)
                _Description = value
            End Set
        End Property
        'Public Property StreetNum As String
        '    Get
        '        Return _StreetNum
        '    End Get
        '    Set(value As String)
        '        _StreetNum = value
        '    End Set
        'End Property
        'Public Property StreetName As String
        '    Get
        '        Return _StreetName
        '    End Get
        '    Set(value As String)
        '        _StreetName = value
        '    End Set
        'End Property
        'Public Property City As String
        '    Get
        '        Return _City
        '    End Get
        '    Set(value As String)
        '        _City = value
        '    End Set
        'End Property
        'Public Property State As String
        '    Get
        '        Return _State
        '    End Get
        '    Set(value As String)
        '        _State = value
        '    End Set
        'End Property
        'Public Property Zip As String
        '    Get
        '        Return _Zip
        '    End Get
        '    Set(value As String)
        '        _Zip = value
        '    End Set
        'End Property
        'Public Property County As String
        '    Get
        '        Return _County
        '    End Get
        '    Set(value As String)
        '        _County = value
        '    End Set
        'End Property
        Public Property Address As QuickQuoteAddress
            Get
                SetObjectsParent(_Address)
                Return _Address
            End Get
            Set(value As QuickQuoteAddress)
                _Address = value
                SetObjectsParent(_Address)
            End Set
        End Property
        Public Property ProtectionClass As String '*currently being sent/returned in XML at Building level
            Get
                'Return _ProtectionClass
                'updated 9/26/2016
                If qqHelper.IsNumericString(_ProtectionClassId) = True AndAlso String.IsNullOrWhiteSpace(_ProtectionClass) = True Then
                    Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.ProtectionClassSystemGeneratedId, _ProtectionClassId)
                Else
                    Return _ProtectionClass
                End If
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
                '        'updated 7/30/2013 for HOM
                '    Case "1"
                '        _ProtectionClassId = "1"
                '    Case "2"
                '        _ProtectionClassId = "2"
                '    Case "3"
                '        _ProtectionClassId = "3"
                '    Case "4"
                '        _ProtectionClassId = "4"
                '    Case "5"
                '        _ProtectionClassId = "5"
                '    Case "6"
                '        _ProtectionClassId = "6"
                '    Case "7"
                '        _ProtectionClassId = "7"
                '    Case "8"
                '        _ProtectionClassId = "8"
                '    Case "9"
                '        _ProtectionClassId = "9"
                '    Case "10" 'already above for comm
                '        _ProtectionClassId = "10"
                '    Case "11"
                '        _ProtectionClassId = "11"
                '    Case Else 'added 5/2/2013
                '        _ProtectionClassId = ""
                'End Select
                'updated 12/12/2013
                _ProtectionClassId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.ProtectionClassId, _ProtectionClass)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's ProtectionClass table (-1=N/A, 0=None, 1=1, 2=2, 3=3, 4=4, 5=5, 6=6, 7=7, 8=8, 9=9, 10=10, 11=11, 12=01, 13=02, 14=03, 15=04, 16=05, 17=06, 18=07, 19=08, 20=8B, 21=09, 22=10); Personal uses ids 1-11, Commercial uses ids 12-22</remarks>
        Public Property ProtectionClassId As String 'verified in database 7/3/2012; 7/30/2013: -1=N/A; 0=None
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
                '            'updated 7/30/2013 for HOM
                '        Case "1"
                '            _ProtectionClass = "1"
                '        Case "2"
                '            _ProtectionClass = "2"
                '        Case "3"
                '            _ProtectionClass = "3"
                '        Case "4"
                '            _ProtectionClass = "4"
                '        Case "5"
                '            _ProtectionClass = "5"
                '        Case "6"
                '            _ProtectionClass = "6"
                '        Case "7"
                '            _ProtectionClass = "7"
                '        Case "8"
                '            _ProtectionClass = "8"
                '        Case "9"
                '            _ProtectionClass = "9"
                '        Case "10"
                '            _ProtectionClass = "10"
                '        Case "11"
                '            _ProtectionClass = "11"
                '    End Select
                'End If
                'updated 12/12/2013
                _ProtectionClass = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.ProtectionClassId, _ProtectionClassId)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21059</remarks>
        Public Property NumberOfPools As String
            Get
                Return _NumberOfPools
            End Get
            Set(value As String)
                _NumberOfPools = value
            End Set
        End Property
        ''' <summary>
        ''' su
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80399</remarks>
        Public Property PropertyDeductibleId As String
            Get
                Return _PropertyDeductibleId
            End Get
            Set(value As String)
                _PropertyDeductibleId = value
            End Set
        End Property

        '3/9/2017 - BOP stuff
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21059</remarks>
        Public Property PoolsQuotedPremium As String
            Get
                'Return _PoolsQuotedPremium
                Return qqHelper.QuotedPremiumFormat(_PoolsQuotedPremium)
            End Get
            Set(value As String)
                _PoolsQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_PoolsQuotedPremium)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21060</remarks>
        Public Property NumberOfAmusementAreas As String
            Get
                Return _NumberOfAmusementAreas
            End Get
            Set(value As String)
                _NumberOfAmusementAreas = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21060</remarks>
        Public Property AmusementAreasQuotedPremium As String
            Get
                'Return _AmusementsQuotedPremium
                Return qqHelper.QuotedPremiumFormat(_AmusementsQuotedPremium)
            End Get
            Set(value As String)
                _AmusementsQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_AmusementsQuotedPremium)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80386</remarks>
        Public Property NumberOfPlaygrounds As String
            Get
                Return _NumberOfPlaygrounds
            End Get
            Set(value As String)
                _NumberOfPlaygrounds = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80386</remarks>
        Public Property PlaygroundsQuotedPremium As String
            Get
                'Return _PlaygroundsQuotedPremium
                Return qqHelper.QuotedPremiumFormat(_PlaygroundsQuotedPremium)
            End Get
            Set(value As String)
                _PlaygroundsQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_PlaygroundsQuotedPremium)
            End Set
        End Property

        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21042</remarks>
        Public Property EquipmentBreakdownDeductible As String
            Get
                Return _EquipmentBreakdownDeductible
            End Get
            Set(value As String)
                _EquipmentBreakdownDeductible = value
                'Select Case _EquipmentBreakdownDeductible
                '    Case "250"
                '        '_EquipmentBreakdownDeductibleId = "21"
                '        _EquipmentBreakdownDeductibleId = "4"
                '    Case "500"
                '        '_EquipmentBreakdownDeductibleId = "22"
                '        _EquipmentBreakdownDeductibleId = "8"
                '    Case "1,000"
                '        '_EquipmentBreakdownDeductibleId = "11"
                '        _EquipmentBreakdownDeductibleId = "9"
                '    Case "2,500"
                '        '_EquipmentBreakdownDeductibleId = "134"
                '        _EquipmentBreakdownDeductibleId = "15"
                '    Case "5,000"
                '        '_EquipmentBreakdownDeductibleId = "15"
                '        _EquipmentBreakdownDeductibleId = "16"
                '    Case "10,000"
                '        '_EquipmentBreakdownDeductibleId = "288"
                '        _EquipmentBreakdownDeductibleId = "17"
                '    Case Else
                '        _EquipmentBreakdownDeductibleId = ""
                'End Select
                'updated 12/12/2013
                _EquipmentBreakdownDeductibleId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.EquipmentBreakdownDeductibleId, _EquipmentBreakdownDeductible)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21042</remarks>
        Public Property EquipmentBreakdownDeductibleId As String '*appears to be wrong (correct for limits but not deductibles); checked database 7/3/2012
            Get
                Return _EquipmentBreakdownDeductibleId
            End Get
            Set(value As String)
                _EquipmentBreakdownDeductibleId = value
                '(21=250; 22=500; 11=1,000; 134=2,500; 15=5,000; 288=10,000)
                '_EquipmentBreakdownDeductible = ""
                'If IsNumeric(_EquipmentBreakdownDeductibleId) = True Then
                '    Select Case _EquipmentBreakdownDeductibleId
                '        'Case "21"
                '        '    _EquipmentBreakdownDeductible = "250"
                '        'Case "22"
                '        '    _EquipmentBreakdownDeductible = "500"
                '        'Case "11"
                '        '    _EquipmentBreakdownDeductible = "1,000"
                '        'Case "134"
                '        '    _EquipmentBreakdownDeductible = "2,500"
                '        'Case "15"
                '        '    _EquipmentBreakdownDeductible = "5,000"
                '        'Case "288"
                '        '    _EquipmentBreakdownDeductible = "10,000"

                '        'started correcting 7/3/2012
                '        Case "4"
                '            _EquipmentBreakdownDeductible = "250"
                '        Case "8"
                '            _EquipmentBreakdownDeductible = "500"
                '        Case "9"
                '            _EquipmentBreakdownDeductible = "1,000"
                '        Case "15"
                '            _EquipmentBreakdownDeductible = "2,500"
                '        Case "16"
                '            _EquipmentBreakdownDeductible = "5,000"
                '        Case "17"
                '            _EquipmentBreakdownDeductible = "10,000"
                '    End Select
                'End If
                'updated 12/12/2013
                _EquipmentBreakdownDeductible = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.EquipmentBreakdownDeductibleId, _EquipmentBreakdownDeductibleId)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21042</remarks>
        Public Property EquipmentBreakdownDeductibleQuotedPremium As String
            Get
                'Return _EquipmentBreakdownDeductibleQuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_EquipmentBreakdownDeductibleQuotedPremium)
            End Get
            Set(value As String)
                _EquipmentBreakdownDeductibleQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_EquipmentBreakdownDeductibleQuotedPremium)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21040</remarks>
        Public Property MoneySecuritiesOnPremises As String
            Get
                Return _MoneySecuritiesOnPremises
            End Get
            Set(value As String)
                _MoneySecuritiesOnPremises = value
                qqHelper.ConvertToLimitFormat(_MoneySecuritiesOnPremises)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21041</remarks>
        Public Property MoneySecuritiesOffPremises As String
            Get
                Return _MoneySecuritiesOffPremises
            End Get
            Set(value As String)
                _MoneySecuritiesOffPremises = value
                qqHelper.ConvertToLimitFormat(_MoneySecuritiesOffPremises)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverages w/ coveragecode_ids 21040 and 21041</remarks>
        Public ReadOnly Property MoneySecuritiesQuotedPremium As String
            Get
                'Return _MoneySecuritiesQuotedPremium
                'updated 8/25/2014
                'Return qqHelper.QuotedPremiumFormat(_MoneySecuritiesQuotedPremium)
                Return qqHelper.QuotedPremiumFormat(qqHelper.getSum(_MoneySecuritiesQuotedPremium_OnPremises, _MoneySecuritiesQuotedPremium_OffPremises))
            End Get
            'Set(value As String)
            '    _MoneySecuritiesQuotedPremium = value
            '    qqHelper.ConvertToQuotedPremiumFormat(_MoneySecuritiesQuotedPremium)
            'End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverages w/ coveragecode_ids 21040</remarks>
        Public Property MoneySecuritiesQuotedPremium_OnPremises As String
            Get
                'Return _MoneySecuritiesQuotedPremium_OnPremises
                'updated 8/28/2022
                Return qqHelper.QuotedPremiumFormat(_MoneySecuritiesQuotedPremium_OnPremises)
            End Get
            Set(value As String)
                _MoneySecuritiesQuotedPremium_OnPremises = value
                qqHelper.ConvertToQuotedPremiumFormat(_MoneySecuritiesQuotedPremium_OnPremises)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverages w/ coveragecode_ids 21041</remarks>
        Public Property MoneySecuritiesQuotedPremium_OffPremises As String
            Get
                'Return _MoneySecuritiesQuotedPremium_OffPremises
                'updated 8/28/2022
                Return qqHelper.QuotedPremiumFormat(_MoneySecuritiesQuotedPremium_OffPremises)
            End Get
            Set(value As String)
                _MoneySecuritiesQuotedPremium_OffPremises = value
                qqHelper.ConvertToQuotedPremiumFormat(_MoneySecuritiesQuotedPremium_OffPremises)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 149</remarks>
        Public Property OutdoorSignsLimit As String
            Get
                Return _OutdoorSignsLimit
            End Get
            Set(value As String)
                _OutdoorSignsLimit = value
                qqHelper.ConvertToLimitFormat(_OutdoorSignsLimit)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 149</remarks>
        Public Property OutdoorSignsQuotedPremium As String
            Get
                'Return _OutdoorSignsQuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_OutdoorSignsQuotedPremium)
            End Get
            Set(value As String)
                _OutdoorSignsQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_OutdoorSignsQuotedPremium)
            End Set
        End Property

        Public Property Buildings As Generic.List(Of QuickQuoteBuilding)
            Get
                SetParentOfListItems(_Buildings, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5E5}")
                Return _Buildings
            End Get
            Set(value As Generic.List(Of QuickQuoteBuilding))
                _Buildings = value
                SetParentOfListItems(_Buildings, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5E5}")
            End Set
        End Property
        Public Property LocationCoverages As Generic.List(Of QuickQuoteCoverage)
            Get
                SetParentOfListItems(_LocationCoverages, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5E6}")
                Return _LocationCoverages
            End Get
            Set(value As Generic.List(Of QuickQuoteCoverage))
                _LocationCoverages = value
                SetParentOfListItems(_LocationCoverages, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5E6}")
            End Set
        End Property

        Public Property GLClassifications As Generic.List(Of QuickQuoteGLClassification)
            Get
                SetParentOfListItems(_GLClassifications, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5E7}")
                Return _GLClassifications
            End Get
            Set(value As Generic.List(Of QuickQuoteGLClassification))
                _GLClassifications = value
                SetParentOfListItems(_GLClassifications, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5E7}")
            End Set
        End Property
        Public Property Classifications As Generic.List(Of QuickQuoteClassification)
            Get
                SetParentOfListItems(_Classifications, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5E8}")
                Return _Classifications
            End Get
            Set(value As Generic.List(Of QuickQuoteClassification))
                _Classifications = value
                SetParentOfListItems(_Classifications, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5E8}")
            End Set
        End Property

        Public Property Name As QuickQuoteName
            Get
                SetObjectsParent(_Name)
                Return _Name
            End Get
            Set(value As QuickQuoteName)
                _Name = value
                SetObjectsParent(_Name)
            End Set
        End Property

        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond GLClassifications and coverages w/ coveragecode_id 80150</remarks>
        Public Property GL_PremisesQuotedPremium As String
            Get
                'Return _GL_PremisesQuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_GL_PremisesQuotedPremium)
            End Get
            Set(value As String)
                _GL_PremisesQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_GL_PremisesQuotedPremium)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond GLClassifications and coverages w/ coveragecode_id 80152</remarks>
        Public Property GL_ProductsQuotedPremium As String
            Get
                'Return _GL_ProductsQuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_GL_ProductsQuotedPremium)
            End Get
            Set(value As String)
                _GL_ProductsQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_GL_ProductsQuotedPremium)
            End Set
        End Property

        Public Property HasBuilding As Boolean
            Get
                Return _HasBuilding
            End Get
            Set(value As Boolean)
                _HasBuilding = value
            End Set
        End Property
        Public Property HasClassification As Boolean
            Get
                Return _HasClassification
            End Get
            Set(value As Boolean)
                _HasClassification = value
            End Set
        End Property

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
                _CauseOfLossType = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.CauseOfLossTypeId, _CauseOfLossTypeId)
            End Set
        End Property
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
                _CauseOfLossTypeId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.CauseOfLossTypeId, _CauseOfLossType)
            End Set
        End Property
        Public Property DeductibleId As String
            Get
                Return _DeductibleId
            End Get
            Set(value As String)
                _DeductibleId = value
                '_Deductible = ""
                'If IsNumeric(_DeductibleId) = True Then
                '    Select Case _DeductibleId
                '        Case "0"
                '            _Deductible = "N/A"
                '        Case "4"
                '            _Deductible = "250"
                '        Case "8"
                '            _Deductible = "500"
                '        Case "9"
                '            _Deductible = "1,000"
                '        Case "15"
                '            _Deductible = "2,500"
                '        Case "16"
                '            _Deductible = "5,000"
                '        Case "17"
                '            _Deductible = "10,000"
                '        Case "19"
                '            _Deductible = "25,000"
                '        Case "20"
                '            _Deductible = "50,000"
                '        Case "21"
                '            _Deductible = "75,000"
                '        Case "32"
                '            _Deductible = "1%"
                '        Case "33"
                '            _Deductible = "2%"
                '        Case "34"
                '            _Deductible = "5%"
                '        Case "42"
                '            _Deductible = "Same"
                '        Case "36"
                '            _Deductible = "10%"
                '    End Select
                'End If
                'updated 12/13/2013
                _Deductible = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleId, _DeductibleId)
            End Set
        End Property
        Public Property Deductible As String
            Get
                Return _Deductible
            End Get
            Set(value As String)
                _Deductible = value
                'Select Case _Deductible
                '    Case "N/A"
                '        _DeductibleId = "0"
                '    Case "250"
                '        _DeductibleId = "4"
                '    Case "500"
                '        _DeductibleId = "8"
                '    Case "1,000"
                '        _DeductibleId = "9"
                '    Case "2,500"
                '        _DeductibleId = "15"
                '    Case "5,000"
                '        _DeductibleId = "16"
                '    Case "10,000"
                '        _DeductibleId = "17"
                '    Case "25,000"
                '        _DeductibleId = "19"
                '    Case "50,000"
                '        _DeductibleId = "20"
                '    Case "75,000"
                '        _DeductibleId = "21"
                '    Case "1%"
                '        _DeductibleId = "32"
                '    Case "2%"
                '        _DeductibleId = "33"
                '    Case "5%"
                '        _DeductibleId = "34"
                '    Case "Same"
                '        _DeductibleId = "42"
                '    Case "10%"
                '        _DeductibleId = "36"
                '    Case Else
                '        _DeductibleId = ""
                'End Select
                'updated 12/13/2013
                _DeductibleId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleId, _Deductible)
            End Set
        End Property
        Public Property CoinsuranceTypeId As String 'only N/A, 80%, 90%, 100% are used for CPR locations
            Get
                Return _CoinsuranceTypeId
            End Get
            Set(value As String)
                _CoinsuranceTypeId = value
                '_CoinsuranceType = ""
                'If IsNumeric(_CoinsuranceTypeId) = True Then
                '    Select Case _CoinsuranceTypeId
                '        Case "0"
                '            _CoinsuranceType = "N/A"
                '        Case "1"
                '            _CoinsuranceType = "Waived"
                '        Case "2"
                '            _CoinsuranceType = "50%"
                '        Case "3"
                '            _CoinsuranceType = "60%"
                '        Case "4"
                '            _CoinsuranceType = "70%"
                '        Case "5"
                '            _CoinsuranceType = "80%"
                '        Case "6"
                '            _CoinsuranceType = "90%"
                '        Case "7"
                '            _CoinsuranceType = "100%"
                '        Case "8"
                '            _CoinsuranceType = "10%"
                '        Case "9"
                '            _CoinsuranceType = "20%"
                '        Case "10"
                '            _CoinsuranceType = "30%"
                '        Case "11"
                '            _CoinsuranceType = "40%"
                '        Case "12"
                '            _CoinsuranceType = "125%"
                '    End Select
                'End If
                'updated 12/13/2013
                _CoinsuranceType = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.CoinsuranceTypeId, _CoinsuranceTypeId)
            End Set
        End Property
        Public Property CoinsuranceType As String 'only N/A, 80%, 90%, 100% are used for CPR locations
            Get
                Return _CoinsuranceType
            End Get
            Set(value As String)
                _CoinsuranceType = value
                'Select Case _CoinsuranceType
                '    Case "N/A"
                '        _CoinsuranceTypeId = "0"
                '    Case "Waived"
                '        _CoinsuranceTypeId = "1"
                '    Case "50%"
                '        _CoinsuranceTypeId = "2"
                '    Case "60%"
                '        _CoinsuranceTypeId = "3"
                '    Case "70%"
                '        _CoinsuranceTypeId = "4"
                '    Case "80%"
                '        _CoinsuranceTypeId = "5"
                '    Case "90%"
                '        _CoinsuranceTypeId = "6"
                '    Case "100%"
                '        _CoinsuranceTypeId = "7"
                '    Case "10%"
                '        _CoinsuranceTypeId = "8"
                '    Case "20%"
                '        _CoinsuranceTypeId = "9"
                '    Case "30%"
                '        _CoinsuranceTypeId = "10"
                '    Case "40%"
                '        _CoinsuranceTypeId = "11"
                '    Case "125%"
                '        _CoinsuranceTypeId = "12"
                '    Case Else
                '        _CoinsuranceTypeId = ""
                'End Select
                'updated 12/13/2013
                _CoinsuranceTypeId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.CoinsuranceTypeId, _CoinsuranceType)
            End Set
        End Property
        Public Property ValuationMethodTypeId As String
            Get
                Return _ValuationMethodTypeId
            End Get
            Set(value As String)
                _ValuationMethodTypeId = value
                '(1=Replacement Cost; 2=Actual Cash Value; 3=Functional Building Valuation)
                '_ValuationMethodType = ""
                'If IsNumeric(_ValuationMethodTypeId) = True Then
                '    Select Case _ValuationMethodTypeId
                '        Case "-1" 'added 10/19/2012 for CPR to match specs; 12/13/2013 note: this should be 0
                '            _ValuationMethodType = "N/A"
                '        Case "1"
                '            _ValuationMethodType = "Replacement Cost"
                '        Case "2"
                '            _ValuationMethodType = "Actual Cash Value"
                '        Case "3"
                '            _ValuationMethodType = "Functional Building Valuation"
                '        Case "7" 'added 10/18/2012 for CPR
                '            _ValuationMethodType = "Functional Replacement Cost"
                '    End Select
                'End If
                'updated 12/13/2013
                _ValuationMethodType = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.ValuationMethodTypeId, _ValuationMethodTypeId)
            End Set
        End Property
        Public Property ValuationMethodType As String
            Get
                Return _ValuationMethodType
            End Get
            Set(value As String)
                _ValuationMethodType = value
                'Select Case _ValuationMethodType
                '    Case "N/A" 'added 10/19/2012 for CPR to match specs; 12/13/2013 note: this should be 0
                '        _ValuationMethodTypeId = "-1"
                '    Case "Replacement Cost"
                '        _ValuationMethodTypeId = "1"
                '    Case "Actual Cash Value"
                '        _ValuationMethodTypeId = "2"
                '    Case "Functional Building Valuation"
                '        _ValuationMethodTypeId = "3"
                '    Case "Functional Replacement Cost" 'added 10/18/2012 for CPR
                '        _ValuationMethodTypeId = "7"
                '    Case Else
                '        _ValuationMethodTypeId = ""
                'End Select
                'updated 12/13/2013
                _ValuationMethodTypeId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.ValuationMethodTypeId, _ValuationMethodType)
            End Set
        End Property

        Public Property EquipmentBreakdownOccupancyTypeId As String '12/18/2013 note: not in xml yet; 167 rows
            Get
                Return _EquipmentBreakdownOccupancyTypeId
            End Get
            Set(value As String)
                _EquipmentBreakdownOccupancyTypeId = value
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

        Public Property ScheduledCoverages As Generic.List(Of QuickQuoteScheduledCoverage)
            Get
                SetParentOfListItems(_ScheduledCoverages, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5E9}")
                Return _ScheduledCoverages
            End Get
            Set(value As Generic.List(Of QuickQuoteScheduledCoverage))
                _ScheduledCoverages = value
                SetParentOfListItems(_ScheduledCoverages, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5E9}")
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Diamond ScheduledCoverages w/ UICoverageScheduledCoverageParentTypeId 91</remarks>
        Public Property PropertyInTheOpenRecords As Generic.List(Of QuickQuotePropertyInTheOpenRecord)
            Get
                SetParentOfListItems(_PropertyInTheOpenRecords, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5F0}")
                Return _PropertyInTheOpenRecords
            End Get
            Set(value As Generic.List(Of QuickQuotePropertyInTheOpenRecord))
                _PropertyInTheOpenRecords = value
                SetParentOfListItems(_PropertyInTheOpenRecords, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5F0}")
            End Set
        End Property

        'added 3/27/2013 for CPR defaults (stored in xml but not valid; no spot in Diamond)
        Public Property WindstormOrHailPercentageDeductibleId As String
            Get
                Return _WindstormOrHailPercentageDeductibleId
            End Get
            Set(value As String)
                _WindstormOrHailPercentageDeductibleId = value
                '_WindstormOrHailPercentageDeductible = ""
                'If IsNumeric(_WindstormOrHailPercentageDeductibleId) = True Then
                '    Select Case _WindstormOrHailPercentageDeductibleId
                '        Case "0"
                '            _WindstormOrHailPercentageDeductible = "N/A"
                '        Case "4"
                '            _WindstormOrHailPercentageDeductible = "250"
                '        Case "8"
                '            _WindstormOrHailPercentageDeductible = "500"
                '        Case "9"
                '            _WindstormOrHailPercentageDeductible = "1,000"
                '        Case "15"
                '            _WindstormOrHailPercentageDeductible = "2,500"
                '        Case "16"
                '            _WindstormOrHailPercentageDeductible = "5,000"
                '        Case "17"
                '            _WindstormOrHailPercentageDeductible = "10,000"
                '        Case "19"
                '            _WindstormOrHailPercentageDeductible = "25,000"
                '        Case "20"
                '            _WindstormOrHailPercentageDeductible = "50,000"
                '        Case "21"
                '            _WindstormOrHailPercentageDeductible = "75,000"
                '        Case "32"
                '            _WindstormOrHailPercentageDeductible = "1%"
                '        Case "33"
                '            _WindstormOrHailPercentageDeductible = "2%"
                '        Case "34"
                '            _WindstormOrHailPercentageDeductible = "5%"
                '        Case "42"
                '            _WindstormOrHailPercentageDeductible = "Same"
                '        Case "36"
                '            _WindstormOrHailPercentageDeductible = "10%"
                '    End Select
                'End If
                'updated 12/18/2013
                _WindstormOrHailPercentageDeductible = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.WindstormOrHailPercentageDeductibleId, _WindstormOrHailPercentageDeductibleId)
            End Set
        End Property
        Public Property WindstormOrHailPercentageDeductible As String
            Get
                Return _WindstormOrHailPercentageDeductible
            End Get
            Set(value As String)
                _WindstormOrHailPercentageDeductible = value
                'Select Case _WindstormOrHailPercentageDeductible
                '    Case "N/A"
                '        _WindstormOrHailPercentageDeductibleId = "0"
                '    Case "250"
                '        _WindstormOrHailPercentageDeductibleId = "4"
                '    Case "500"
                '        _WindstormOrHailPercentageDeductibleId = "8"
                '    Case "1,000"
                '        _WindstormOrHailPercentageDeductibleId = "9"
                '    Case "2,500"
                '        _WindstormOrHailPercentageDeductibleId = "15"
                '    Case "5,000"
                '        _WindstormOrHailPercentageDeductibleId = "16"
                '    Case "10,000"
                '        _WindstormOrHailPercentageDeductibleId = "17"
                '    Case "25,000"
                '        _WindstormOrHailPercentageDeductibleId = "19"
                '    Case "50,000"
                '        _WindstormOrHailPercentageDeductibleId = "20"
                '    Case "75,000"
                '        _WindstormOrHailPercentageDeductibleId = "21"
                '    Case "1%"
                '        _WindstormOrHailPercentageDeductibleId = "32"
                '    Case "2%"
                '        _WindstormOrHailPercentageDeductibleId = "33"
                '    Case "5%"
                '        _WindstormOrHailPercentageDeductibleId = "34"
                '    Case "Same"
                '        _WindstormOrHailPercentageDeductibleId = "42"
                '    Case "10%"
                '        _WindstormOrHailPercentageDeductibleId = "36"
                '    Case Else
                '        _WindstormOrHailPercentageDeductibleId = ""
                'End Select
                'updated 12/18/2013
                _WindstormOrHailPercentageDeductibleId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.WindstormOrHailPercentageDeductibleId, _WindstormOrHailPercentageDeductible)
            End Set
        End Property
        Public Property WindstormOrHailMinimumDollarDeductibleId As String
            Get
                Return _WindstormOrHailMinimumDollarDeductibleId
            End Get
            Set(value As String)
                _WindstormOrHailMinimumDollarDeductibleId = value
                '_WindstormOrHailMinimumDollarDeductible = ""
                'If IsNumeric(_WindstormOrHailMinimumDollarDeductibleId) = True Then
                '    Select Case _WindstormOrHailMinimumDollarDeductibleId
                '        Case "0"
                '            _WindstormOrHailMinimumDollarDeductible = "N/A"
                '        Case "4"
                '            _WindstormOrHailMinimumDollarDeductible = "250"
                '        Case "8"
                '            _WindstormOrHailMinimumDollarDeductible = "500"
                '        Case "9"
                '            _WindstormOrHailMinimumDollarDeductible = "1,000"
                '        Case "15"
                '            _WindstormOrHailMinimumDollarDeductible = "2,500"
                '        Case "16"
                '            _WindstormOrHailMinimumDollarDeductible = "5,000"
                '        Case "17"
                '            _WindstormOrHailMinimumDollarDeductible = "10,000"
                '        Case "19"
                '            _WindstormOrHailMinimumDollarDeductible = "25,000"
                '        Case "20"
                '            _WindstormOrHailMinimumDollarDeductible = "50,000"
                '        Case "21"
                '            _WindstormOrHailMinimumDollarDeductible = "75,000"
                '        Case "32"
                '            _WindstormOrHailMinimumDollarDeductible = "1%"
                '        Case "33"
                '            _WindstormOrHailMinimumDollarDeductible = "2%"
                '        Case "34"
                '            _WindstormOrHailMinimumDollarDeductible = "5%"
                '        Case "42"
                '            _WindstormOrHailMinimumDollarDeductible = "Same"
                '        Case "36"
                '            _WindstormOrHailMinimumDollarDeductible = "10%"
                '    End Select
                'End If
                'updated 12/18/2013
                _WindstormOrHailMinimumDollarDeductible = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.WindstormOrHailMinimumDollarDeductibleId, _WindstormOrHailMinimumDollarDeductibleId)
            End Set
        End Property
        Public Property WindstormOrHailMinimumDollarDeductible As String
            Get
                Return _WindstormOrHailMinimumDollarDeductible
            End Get
            Set(value As String)
                _WindstormOrHailMinimumDollarDeductible = value
                'Select Case _WindstormOrHailMinimumDollarDeductible
                '    Case "N/A"
                '        _WindstormOrHailMinimumDollarDeductibleId = "0"
                '    Case "250"
                '        _WindstormOrHailMinimumDollarDeductibleId = "4"
                '    Case "500"
                '        _WindstormOrHailMinimumDollarDeductibleId = "8"
                '    Case "1,000"
                '        _WindstormOrHailMinimumDollarDeductibleId = "9"
                '    Case "2,500"
                '        _WindstormOrHailMinimumDollarDeductibleId = "15"
                '    Case "5,000"
                '        _WindstormOrHailMinimumDollarDeductibleId = "16"
                '    Case "10,000"
                '        _WindstormOrHailMinimumDollarDeductibleId = "17"
                '    Case "25,000"
                '        _WindstormOrHailMinimumDollarDeductibleId = "19"
                '    Case "50,000"
                '        _WindstormOrHailMinimumDollarDeductibleId = "20"
                '    Case "75,000"
                '        _WindstormOrHailMinimumDollarDeductibleId = "21"
                '    Case "1%"
                '        _WindstormOrHailMinimumDollarDeductibleId = "32"
                '    Case "2%"
                '        _WindstormOrHailMinimumDollarDeductibleId = "33"
                '    Case "5%"
                '        _WindstormOrHailMinimumDollarDeductibleId = "34"
                '    Case "Same"
                '        _WindstormOrHailMinimumDollarDeductibleId = "42"
                '    Case "10%"
                '        _WindstormOrHailMinimumDollarDeductibleId = "36"
                '    Case Else
                '        _WindstormOrHailMinimumDollarDeductibleId = ""
                'End Select
                'updated 12/18/2013
                _WindstormOrHailMinimumDollarDeductibleId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.WindstormOrHailMinimumDollarDeductibleId, _WindstormOrHailMinimumDollarDeductible)
            End Set
        End Property
        Public Property EarthquakeApplies As Boolean
            Get
                Return _EarthquakeApplies
            End Get
            Set(value As Boolean)
                _EarthquakeApplies = value
            End Set
        End Property

        'added 4/17/2013 for CPR to total up Property in the Open coverage premiums
        Public Property PropertyInTheOpenRecordsTotal_QuotedPremium As String
            Get
                'Return _PropertyInTheOpenRecordsTotal_QuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_PropertyInTheOpenRecordsTotal_QuotedPremium)
            End Get
            Set(value As String)
                _PropertyInTheOpenRecordsTotal_QuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_PropertyInTheOpenRecordsTotal_QuotedPremium)
            End Set
        End Property
        Public Property PropertyInTheOpenRecordsTotal_EQ_Premium As String
            Get
                'Return _PropertyInTheOpenRecordsTotal_EQ_Premium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_PropertyInTheOpenRecordsTotal_EQ_Premium)
            End Get
            Set(value As String)
                _PropertyInTheOpenRecordsTotal_EQ_Premium = value
                qqHelper.ConvertToQuotedPremiumFormat(_PropertyInTheOpenRecordsTotal_EQ_Premium)
            End Set
        End Property
        Public Property PropertyInTheOpenRecordsTotal_QuotedPremium_With_EQ As String
            Get
                'Return _PropertyInTheOpenRecordsTotal_QuotedPremium_With_EQ
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_PropertyInTheOpenRecordsTotal_QuotedPremium_With_EQ)
            End Get
            Set(value As String)
                _PropertyInTheOpenRecordsTotal_QuotedPremium_With_EQ = value
                qqHelper.ConvertToQuotedPremiumFormat(_PropertyInTheOpenRecordsTotal_QuotedPremium_With_EQ)
            End Set
        End Property

        Public Property Acreage As String
            Get
                Return _Acreage
            End Get
            Set(value As String)
                _Acreage = value
            End Set
        End Property
        Public Property CondoRentedTypeId As String '0=N/A; 1=Yes; 2=No
            Get
                Return _CondoRentedTypeId
            End Get
            Set(value As String)
                _CondoRentedTypeId = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's ConstructionType table</remarks>
        Public Property ConstructionTypeId As String '-1=N/A; 0=None; 1=Frame; etc.
            Get
                Return _ConstructionTypeId
            End Get
            Set(value As String)
                _ConstructionTypeId = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 70025 (HOM and DFR)</remarks>
        Public Property DeductibleLimitId As String '22=500
            Get
                Return _DeductibleLimitId
            End Get
            Set(value As String)
                _DeductibleLimitId = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 70216 (HOM)</remarks>
        Public Property WindHailDeductibleLimitId As String '24=1000
            Get
                Return _WindHailDeductibleLimitId
            End Get
            Set(value As String)
                _WindHailDeductibleLimitId = value
            End Set
        End Property
        Public Property DayEmployees As Boolean
            Get
                Return _DayEmployees
            End Get
            Set(value As Boolean)
                _DayEmployees = value
            End Set
        End Property
        Public Property DaytimeOccupancy As Boolean
            Get
                Return _DaytimeOccupancy
            End Get
            Set(value As Boolean)
                _DaytimeOccupancy = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's FamilyUnits table (0=None, 1=1, 2=2, 3=3, 4=4, 5=5 or More)</remarks>
        Public Property FamilyUnitsId As String '0=None; 1=1; 2=2; 3=3; 4=4; 5=5 or More
            Get
                Return _FamilyUnitsId
            End Get
            Set(value As String)
                _FamilyUnitsId = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's FireDepartmentDistance table (0=N/A, 1=More than 5 Miles, 2=5 Miles or Less)</remarks>
        Public Property FireDepartmentDistanceId As String '0=N/A; 1=More than 5 Miles; 2=5 Miles or Less
            Get
                Return _FireDepartmentDistanceId
            End Get
            Set(value As String)
                _FireDepartmentDistanceId = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's FireHydrantDistance table (-1=Unknown; 0=N/A; 1=Over 1,000 Feet; 2=1000+ feet; 3=1,000 Feet or Less; 4=Within 1,000 feet)</remarks>
        Public Property FireHydrantDistanceId As String '-1=Unknown; 0=N/A; 1=Over 1,000 Feet; 2=1000+ feet; 3=1,000 Feet or Less; 4=Within 1,000 feet
            Get
                Return _FireHydrantDistanceId
            End Get
            Set(value As String)
                _FireHydrantDistanceId = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's FormType table (HOM and DFR values: -1=Unassigned; 0=None; 1=HO-2 - Homeowners Broad Form; 2=HO-3 - Homeowners Special Form; 3=HO-3 with HO-15; 4=HO-4 - Homeowners Contents Broad Form; 5=HO-6 - Homeowners Unit Owners Form; 6=ML-2 - Mobile Home Owner Occupied; 7=ML-4 - Mobile Home Tenant Occupied; 8=DP 00 01 - Fire; 9=DP 00 01 - Fire and EC; 10=DP 00 01 - Fire, EC and V&amp;MM; 11=DP 00 02 - Broad; 12=DP 00 03 - Special)</remarks>
        Public Property FormTypeId As String '-1=Unassigned; 0=None; 1=HO-2 - Homeowners Broad Form; 2=HO-3 - Homeowners Special Form; 3=HO-3 with HO-15; 4=HO-4 - Homeowners Contents Broad Form; 5=HO-6 - Homeowners Unit Owners Form; 6=ML-2 - Mobile Home Owner Occupied; 7=ML-4 - Mobile Home Tenant Occupied; 8/15/2013 - more for DFR: 8=DP 00 01 - Fire; 9=DP 00 01 - Fire and EC; 10=DP 00 01 - Fire, EC and V&MM; 11=DP 00 02 - Broad; 12=DP 00 03 - Special
            Get
                Return _FormTypeId
            End Get
            Set(value As String)
                _FormTypeId = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's FoundationType table (0=N/A, 1=Open, 2=Closed, 3=Other, 4=No Foundation)</remarks>
        Public Property FoundationTypeId As String '0=N/A; 1=Open; 2=Closed; 3=Other; 4=No Foundation
            Get
                Return _FoundationTypeId
            End Get
            Set(value As String)
                _FoundationTypeId = value
            End Set
        End Property
        Public Property LastCostEstimatorDate As String
            Get
                Return _LastCostEstimatorDate
            End Get
            Set(value As String)
                _LastCostEstimatorDate = value
                qqHelper.ConvertToShortDate(_LastCostEstimatorDate)
            End Set
        End Property
        Public Property MarketValue As String
            Get
                Return _MarketValue
                'updated 8/25/2014; not using for now
                'Return qqHelper.QuotedPremiumFormat(_MarketValue)
            End Get
            Set(value As String)
                _MarketValue = value
                qqHelper.ConvertToQuotedPremiumFormat(_MarketValue) 'may not use
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's NumberOfFamilies table (0=None, 1=1, 2=2, 3=3, 4=4, 5=5 or more)</remarks>
        Public Property NumberOfFamiliesId As String '0=None; 1=1; 2=2; 3=3; 4=4; 5=5 or more
            Get
                Return _NumberOfFamiliesId
            End Get
            Set(value As String)
                _NumberOfFamiliesId = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's OccupancyCode table (HOM and DFR values: -1=N/A, 0=None, 1=Owner, 4=Seasonal, 5=Secondary, 6=Vacant, 7=Under Construction, 9=Tenant, 14=Owner Occupied, 15=Tenant Occupied)</remarks>
        Public Property OccupancyCodeId As String '-1=N/A; 0=None; 1=Owner; 4=Seasonal; 5=Secondary; 6=Vacant; 7=Under Construction; 9=Tenant; etc.; 8/15/2013 - more for DFR: 14=Owner Occupied; 15=Tenant Occupied
            Get
                Return _OccupancyCodeId
            End Get
            Set(value As String)
                _OccupancyCodeId = value
            End Set
        End Property
        Public Property PrimaryResidence As Boolean
            Get
                Return _PrimaryResidence
            End Get
            Set(value As Boolean)
                _PrimaryResidence = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's ProgramType table (HOM and DFR values: -1=Unassigned, 0=None, 1=Homeowners, 2=Mobile Home, 3=Dwelling Fire)</remarks>
        Public Property ProgramTypeId As String '-1=Unassigned; 0=None; 1=Homeowners; 2=Mobile Home; 3=Dwelling Fire; 4=Personal Umbrella; etc.
            Get
                Return _ProgramTypeId
            End Get
            Set(value As String)
                _ProgramTypeId = value
            End Set
        End Property
        Public ReadOnly Property ProgramType As String 'added 5/8/2017; could also use new QuickQuoteHelperClass.ProgramTypeForId function
            Get
                Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.ProgramTypeId, _ProgramTypeId)
            End Get
        End Property
        Public Property NumberOfApartments As String
            Get
                Return _NumberOfApartments
            End Get
            Set(value As String)
                _NumberOfApartments = value
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
        Public Property RebuildCost As String
            Get
                Return _RebuildCost
                'updated 8/25/2014; not using for now
                'Return qqHelper.QuotedPremiumFormat(_RebuildCost)
            End Get
            Set(value As String)
                _RebuildCost = value
                qqHelper.ConvertToQuotedPremiumFormat(_RebuildCost) 'may not use
            End Set
        End Property
        Public Property Remarks As String
            Get
                Return _Remarks
            End Get
            Set(value As String)
                _Remarks = value
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
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's StructureType table</remarks>
        Public Property StructureTypeId As String '0=None; 1=Residential Dwelling; 2=Mobile Home; 3=Trailer Home; 4=Townhouse; 6=Dwelling Under Construction; 7=Standard Dwelling; 13=Conventionally Built; etc.
            Get
                Return _StructureTypeId
            End Get
            Set(value As String)
                _StructureTypeId = value
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
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 70021 (HOM and DFR)</remarks>
        Public Property A_Dwelling_Limit As String
            Get
                Return _A_Dwelling_Limit
            End Get
            Set(value As String)
                _A_Dwelling_Limit = value
                qqHelper.ConvertToLimitFormat(_A_Dwelling_Limit)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 70021 (HOM and DFR)</remarks>
        Public Property A_Dwelling_LimitIncreased As String
            Get
                Return _A_Dwelling_LimitIncreased
            End Get
            Set(value As String)
                _A_Dwelling_LimitIncreased = value
                qqHelper.ConvertToLimitFormat(_A_Dwelling_LimitIncreased)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 70021 (HOM and DFR)</remarks>
        Public Property A_Dwelling_LimitIncluded As String
            Get
                Return _A_Dwelling_LimitIncluded
            End Get
            Set(value As String)
                _A_Dwelling_LimitIncluded = value
                qqHelper.ConvertToLimitFormat(_A_Dwelling_LimitIncluded)
            End Set
        End Property
        Public Property A_Dwelling_Calc As String 'added 12/4/2014
            Get
                Return _A_Dwelling_Calc
            End Get
            Set(value As String)
                _A_Dwelling_Calc = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 70022 (HOM and DFR)</remarks>
        Public Property B_OtherStructures_Limit As String
            Get
                Return _B_OtherStructures_Limit
            End Get
            Set(value As String)
                _B_OtherStructures_Limit = value
                qqHelper.ConvertToLimitFormat(_B_OtherStructures_Limit)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 70022 (HOM and DFR)</remarks>
        Public Property B_OtherStructures_LimitIncreased As String
            Get
                Return _B_OtherStructures_LimitIncreased
            End Get
            Set(value As String)
                _B_OtherStructures_LimitIncreased = value
                qqHelper.ConvertToLimitFormat(_B_OtherStructures_LimitIncreased)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 70022 (HOM and DFR)</remarks>
        Public Property B_OtherStructures_LimitIncluded As String
            Get
                Return _B_OtherStructures_LimitIncluded
            End Get
            Set(value As String)
                _B_OtherStructures_LimitIncluded = value
                qqHelper.ConvertToLimitFormat(_B_OtherStructures_LimitIncluded)
            End Set
        End Property
        Public Property B_OtherStructures_Calc As String 'added 12/10/2014
            Get
                Return _B_OtherStructures_Calc
            End Get
            Set(value As String)
                _B_OtherStructures_Calc = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 70023 (HOM and DFR)</remarks>
        Public Property C_PersonalProperty_Limit As String
            Get
                Return _C_PersonalProperty_Limit
            End Get
            Set(value As String)
                _C_PersonalProperty_Limit = value
                qqHelper.ConvertToLimitFormat(_C_PersonalProperty_Limit)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 70023 (HOM and DFR)</remarks>
        Public Property C_PersonalProperty_LimitIncreased As String
            Get
                Return _C_PersonalProperty_LimitIncreased
            End Get
            Set(value As String)
                _C_PersonalProperty_LimitIncreased = value
                qqHelper.ConvertToLimitFormat(_C_PersonalProperty_LimitIncreased)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 70023 (HOM and DFR)</remarks>
        Public Property C_PersonalProperty_LimitIncluded As String
            Get
                Return _C_PersonalProperty_LimitIncluded
            End Get
            Set(value As String)
                _C_PersonalProperty_LimitIncluded = value
                qqHelper.ConvertToLimitFormat(_C_PersonalProperty_LimitIncluded)
            End Set
        End Property
        Public Property C_PersonalProperty_Calc As String 'added 12/10/2014
            Get
                Return _C_PersonalProperty_Calc
            End Get
            Set(value As String)
                _C_PersonalProperty_Calc = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 70024 (HOM and DFR)</remarks>
        Public Property D_LossOfUse_Limit As String
            Get
                Return _D_LossOfUse_Limit
            End Get
            Set(value As String)
                _D_LossOfUse_Limit = value
                qqHelper.ConvertToLimitFormat(_D_LossOfUse_Limit)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 70024 (HOM and DFR)</remarks>
        Public Property D_LossOfUse_LimitIncreased As String
            Get
                Return _D_LossOfUse_LimitIncreased
            End Get
            Set(value As String)
                _D_LossOfUse_LimitIncreased = value
                qqHelper.ConvertToLimitFormat(_D_LossOfUse_LimitIncreased)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 70024 (HOM and DFR)</remarks>
        Public Property D_LossOfUse_LimitIncluded As String
            Get
                Return _D_LossOfUse_LimitIncluded
            End Get
            Set(value As String)
                _D_LossOfUse_LimitIncluded = value
                qqHelper.ConvertToLimitFormat(_D_LossOfUse_LimitIncluded)
            End Set
        End Property
        Public Property D_LossOfUse_Calc As String 'added 12/10/2014
            Get
                Return _D_LossOfUse_Calc
            End Get
            Set(value As String)
                _D_LossOfUse_Calc = value
            End Set
        End Property
        Public Property AdditionalInterests As Generic.List(Of QuickQuoteAdditionalInterest)
            Get
                SetParentOfListItems(_AdditionalInterests, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5F1}")
                Return _AdditionalInterests
            End Get
            Set(value As Generic.List(Of QuickQuoteAdditionalInterest))
                _AdditionalInterests = value
                SetParentOfListItems(_AdditionalInterests, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5F1}")
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
        Public Property Modifiers As List(Of QuickQuoteModifier) 'added 7/31/2013
            Get
                SetParentOfListItems(_Modifiers, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5F2}")
                Return _Modifiers
            End Get
            Set(value As List(Of QuickQuoteModifier))
                SetParentOfListItems(_Modifiers, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5F2}")
                _Modifiers = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>modifier w/ Diamond ModifierTypeId 1 (HOM)</remarks>
        Public Property MultiPolicyDiscount As Boolean
            Get
                Return _MultiPolicyDiscount
            End Get
            Set(value As Boolean)
                _MultiPolicyDiscount = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>modifier w/ Diamond ModifierTypeId 3 (HOM)</remarks>
        Public Property MatureHomeownerDiscount As Boolean
            Get
                Return _MatureHomeownerDiscount
            End Get
            Set(value As Boolean)
                _MatureHomeownerDiscount = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>modifier w/ Diamond ModifierTypeId 5 (HOM)</remarks>
        Public Property NewHomeDiscount As Boolean
            Get
                Return _NewHomeDiscount
            End Get
            Set(value As Boolean)
                _NewHomeDiscount = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>modifier w/ Diamond ModifierTypeId 36 (HOM)</remarks>
        Public Property SelectMarketCredit As Boolean
            Get
                Return _SelectMarketCredit
            End Get
            Set(value As Boolean)
                _SelectMarketCredit = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>modifier w/ Diamond ModifierTypeId 10 and ParentModifierTypeId 26 (HOM)</remarks>
        Public Property BurglarAlarm_LocalAlarmSystem As Boolean
            Get
                Return _BurglarAlarm_LocalAlarmSystem
            End Get
            Set(value As Boolean)
                _BurglarAlarm_LocalAlarmSystem = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>modifier w/ Diamond ModifierTypeId 9 and ParentModifierTypeId 26 (HOM)</remarks>
        Public Property BurglarAlarm_CentralStationAlarmSystem As Boolean
            Get
                Return _BurglarAlarm_CentralStationAlarmSystem
            End Get
            Set(value As Boolean)
                _BurglarAlarm_CentralStationAlarmSystem = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>modifier w/ Diamond ModifierTypeId 10 and ParentModifierTypeId 27 (HOM)</remarks>
        Public Property FireSmokeAlarm_LocalAlarmSystem As Boolean
            Get
                Return _FireSmokeAlarm_LocalAlarmSystem
            End Get
            Set(value As Boolean)
                _FireSmokeAlarm_LocalAlarmSystem = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>modifier w/ Diamond ModifierTypeId 9 and ParentModifierTypeId 27 (HOM)</remarks>
        Public Property FireSmokeAlarm_CentralStationAlarmSystem As Boolean
            Get
                Return _FireSmokeAlarm_CentralStationAlarmSystem
            End Get
            Set(value As Boolean)
                _FireSmokeAlarm_CentralStationAlarmSystem = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>modifier w/ Diamond ModifierTypeId 53 (HOM)</remarks>
        Public Property FireSmokeAlarm_SmokeAlarm As Boolean
            Get
                Return _FireSmokeAlarm_SmokeAlarm
            End Get
            Set(value As Boolean)
                _FireSmokeAlarm_SmokeAlarm = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>modifier w/ Diamond ModifierTypeId 39 (HOM)</remarks>
        Public Property SprinklerSystem_AllExcept As Boolean
            Get
                Return _SprinklerSystem_AllExcept
            End Get
            Set(value As Boolean)
                _SprinklerSystem_AllExcept = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>modifier w/ Diamond ModifierTypeId 40 (HOM)</remarks>
        Public Property SprinklerSystem_AllIncluding As Boolean
            Get
                Return _SprinklerSystem_AllIncluding
            End Get
            Set(value As Boolean)
                _SprinklerSystem_AllIncluding = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>modifier w/ Diamond ModifierTypeId 37 (HOM) or ModifierTypeId 112 (FAR) post 9/1/2022</remarks>
        Public Property TrampolineSurcharge As Boolean
            Get
                Return _TrampolineSurcharge
            End Get
            Set(value As Boolean)
                _TrampolineSurcharge = value
            End Set
        End Property
        ''' <summary>
        ''' Number Of Units for which the surcharge is applied
        ''' </summary>
        ''' <returns></returns>
        Public Property TrampolineSurcharge_NumberOfUnits As Integer

        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>modifier w/ Diamond ModifierTypeId 17 (HOM) or ModifierTypeId 112 (FAR) post 9/1/2022</remarks>
        Public Property WoodOrFuelBurningApplianceSurcharge As Boolean
            Get
                Return _WoodOrFuelBurningApplianceSurcharge
            End Get
            Set(value As Boolean)
                _WoodOrFuelBurningApplianceSurcharge = value
            End Set
        End Property
        ''' <summary>
        ''' Number Of Units for which the surcharge is applied
        ''' </summary>
        ''' <returns></returns>
        Public Property WoodOrFuelBurningApplianceSurcharge_NumberOfUnits As Integer

        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>modifier w/ Diamond ModifierTypeId 65 (HOM) or ModifierTypeId 112 (FAR) post 9/1/2022</remarks>
        Public Property SwimmingPoolHotTubSurcharge As Boolean 'added 7/28/2014
            Get
                Return _SwimmingPoolHotTubSurcharge
            End Get
            Set(value As Boolean)
                _SwimmingPoolHotTubSurcharge = value
            End Set
        End Property
        ''' <summary>
        ''' Number Of Units for which the surcharge is applied
        ''' </summary>
        ''' <returns></returns>
        Public Property SwimmingPoolHotTubSurcharge_NumberOfUnits As Integer

        Public Property SectionCoverages As List(Of QuickQuoteSectionCoverage)
            Get
                SetParentOfListItems(Of QuickQuoteSectionCoverage)(_SectionCoverages, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5D2}")
                Return _SectionCoverages
            End Get
            Set(value As List(Of QuickQuoteSectionCoverage))
                _SectionCoverages = value
                SetParentOfListItems(Of QuickQuoteSectionCoverage)(_SectionCoverages, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5D2}")
            End Set
        End Property

        Public Property Exclusions As List(Of QuickQuoteExclusion)
            Get
                SetParentOfListItems(Of QuickQuoteExclusion)(_Exclusions, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5D3}")
                Return _Exclusions
            End Get
            Set(value As List(Of QuickQuoteExclusion))
                _Exclusions = value
                SetParentOfListItems(Of QuickQuoteExclusion)(_Exclusions, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5D3}")
            End Set
        End Property

        Public Property InlandMarines As List(Of QuickQuoteInlandMarine)
            Get
                SetParentOfListItems(Of QuickQuoteInlandMarine)(_InlandMarines, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5D4}")
                Return _InlandMarines
            End Get
            Set(value As List(Of QuickQuoteInlandMarine))
                _InlandMarines = value
                SetParentOfListItems(Of QuickQuoteInlandMarine)(_InlandMarines, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5D4}")
            End Set
        End Property

        Public Property RvWatercrafts As List(Of QuickQuoteRvWatercraft)
            Get
                SetParentOfListItems(Of QuickQuoteRvWatercraft)(_RvWatercrafts, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5D5}")
                Return _RvWatercrafts
            End Get
            Set(value As List(Of QuickQuoteRvWatercraft))
                _RvWatercrafts = value
                SetParentOfListItems(Of QuickQuoteRvWatercraft)(_RvWatercrafts, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5D5}")
            End Set
        End Property

        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Diamond SectionCoverages w/ CoverageExposureId 6 (HOM and DFR)</remarks>
        Public Property SectionICoverages As List(Of QuickQuoteSectionICoverage)
            Get
                SetParentOfListItems(Of QuickQuote.CommonObjects.QuickQuoteSectionICoverage)(_SectionICoverages, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5D6}")
                Return _SectionICoverages
            End Get
            Set(value As List(Of QuickQuoteSectionICoverage))
                _SectionICoverages = value
                SetParentOfListItems(Of QuickQuote.CommonObjects.QuickQuoteSectionICoverage)(_SectionICoverages, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5D6}")
            End Set
        End Property

        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Diamond SectionCoverages w/ CoverageExposureId 7 (HOM and DFR)</remarks>
        Public Property SectionIICoverages As List(Of QuickQuoteSectionIICoverage)
            Get
                SetParentOfListItems(Of QuickQuote.CommonObjects.QuickQuoteSectionIICoverage)(_SectionIICoverages, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5D7}")
                Return _SectionIICoverages
            End Get
            Set(value As List(Of QuickQuoteSectionIICoverage))
                _SectionIICoverages = value
                SetParentOfListItems(Of QuickQuote.CommonObjects.QuickQuoteSectionIICoverage)(_SectionIICoverages, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5D7}")
            End Set
        End Property

        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Diamond SectionCoverages w/ CoverageExposureId 118 (HOM and DFR)</remarks>
        Public Property SectionIAndIICoverages As List(Of QuickQuoteSectionIAndIICoverage)
            Get
                SetParentOfListItems(Of QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage)(_SectionIAndIICoverages, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5D8}")
                Return _SectionIAndIICoverages
            End Get
            Set(value As List(Of QuickQuoteSectionIAndIICoverage))
                _SectionIAndIICoverages = value
                SetParentOfListItems(Of QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage)(_SectionIAndIICoverages, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5D8}")
            End Set
        End Property

        Public Property PolicyUnderwritings As Generic.List(Of QuickQuotePolicyUnderwriting)
            Get
                SetParentOfListItems(Of QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting)(_PolicyUnderwritings, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5D9}")
                Return _PolicyUnderwritings
            End Get
            Set(value As Generic.List(Of QuickQuotePolicyUnderwriting))
                _PolicyUnderwritings = value
                SetParentOfListItems(Of QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting)(_PolicyUnderwritings, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5D9}")
            End Set
        End Property
        Public Property NumberOfDaysRented As String
            Get
                Return _NumberOfDaysRented
            End Get
            Set(value As String)
                _NumberOfDaysRented = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's NumberOfUnits table (-1=N/A, 0=None, 1=1, 2=2, 3=3, 4=4, 5=5, 6=6, 7=7, 8=8, 9=9, 10=10, 11=0</remarks>
        Public Property NumberOfUnitsId As String '-1=N/A; 0=None; 1=1; 2=2; etc.; 11=0
            Get
                Return _NumberOfUnitsId
            End Get
            Set(value As String)
                _NumberOfUnitsId = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's UsageType table (-1=N/A, 0=None, 1=Seasonal, 2=Non-Seasonal)</remarks>
        Public Property UsageTypeId As String '-1=N/A; 0=None; 1=Seasonal; 2=Non-Seasonal
            Get
                Return _UsageTypeId
            End Get
            Set(value As String)
                _UsageTypeId = value
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

        'added 2/18/2014
        Public Property HasConvertedCoverages As Boolean
            Get
                Return _HasConvertedCoverages
            End Get
            Set(value As Boolean)
                _HasConvertedCoverages = value
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
        Public Property HasConvertedScheduledCoverages As Boolean
            Get
                Return _HasConvertedScheduledCoverages
            End Get
            Set(value As Boolean)
                _HasConvertedScheduledCoverages = value
            End Set
        End Property
        Public Property HasConvertedSectionCoverages As Boolean
            Get
                Return _HasConvertedSectionCoverages
            End Get
            Set(value As Boolean)
                _HasConvertedSectionCoverages = value
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
        Public Property BuildingsTotal_PremiumFullterm As String 'added 4/2/2014
            Get
                'Return _BuildingsTotal_PremiumFullTerm
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_BuildingsTotal_PremiumFullTerm)
            End Get
            Set(value As String)
                _BuildingsTotal_PremiumFullTerm = value
                qqHelper.ConvertToQuotedPremiumFormat(_BuildingsTotal_PremiumFullTerm)
            End Set
        End Property

        'added 4/23/2014 for reconciliation
        Public Property LocationNum As String
            Get
                Return _LocationNum
            End Get
            Set(value As String)
                _LocationNum = value
            End Set
        End Property
        Public Property HasLocationAddressChanged As Boolean
            Get
                Return _HasLocationAddressChanged
            End Get
            Set(value As Boolean)
                _HasLocationAddressChanged = value
            End Set
        End Property
        Public Property CanUseFarmBarnBuildingNumForBuildingReconciliation As Boolean
            Get
                Return _CanUseFarmBarnBuildingNumForBuildingReconciliation
            End Get
            Set(value As Boolean)
                _CanUseFarmBarnBuildingNumForBuildingReconciliation = value
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
        'added 10/17/2018 for multi-state
        Public Property LocationNum_MasterPart As String
            Get
                Return _LocationNum_MasterPart
            End Get
            Set(value As String)
                _LocationNum_MasterPart = value
            End Set
        End Property
        Public Property LocationNum_CGLPart As String
            Get
                Return _LocationNum_CGLPart
            End Get
            Set(value As String)
                _LocationNum_CGLPart = value
            End Set
        End Property
        Public Property LocationNum_CPRPart As String
            Get
                Return _LocationNum_CPRPart
            End Get
            Set(value As String)
                _LocationNum_CPRPart = value
            End Set
        End Property
        Public Property LocationNum_CIMPart As String
            Get
                Return _LocationNum_CIMPart
            End Get
            Set(value As String)
                _LocationNum_CIMPart = value
            End Set
        End Property
        Public Property LocationNum_CRMPart As String
            Get
                Return _LocationNum_CRMPart
            End Get
            Set(value As String)
                _LocationNum_CRMPart = value
            End Set
        End Property
        Public Property LocationNum_GARPart As String
            Get
                Return _LocationNum_GARPart
            End Get
            Set(value As String)
                _LocationNum_GARPart = value
            End Set
        End Property
        'added 10/18/2018 for multi-state
        Public Property CanUseFarmBarnBuildingNumForMasterPartBuildingReconciliation As Boolean
            Get
                Return _CanUseFarmBarnBuildingNumForMasterPartBuildingReconciliation
            End Get
            Set(value As Boolean)
                _CanUseFarmBarnBuildingNumForMasterPartBuildingReconciliation = value
            End Set
        End Property
        Public Property CanUseFarmBarnBuildingNumForCGLPartBuildingReconciliation As Boolean
            Get
                Return _CanUseFarmBarnBuildingNumForCGLPartBuildingReconciliation
            End Get
            Set(value As Boolean)
                _CanUseFarmBarnBuildingNumForCGLPartBuildingReconciliation = value
            End Set
        End Property
        Public Property CanUseFarmBarnBuildingNumForCPRPartBuildingReconciliation As Boolean
            Get
                Return _CanUseFarmBarnBuildingNumForCPRPartBuildingReconciliation
            End Get
            Set(value As Boolean)
                _CanUseFarmBarnBuildingNumForCPRPartBuildingReconciliation = value
            End Set
        End Property
        Public Property CanUseFarmBarnBuildingNumForCIMPartBuildingReconciliation As Boolean
            Get
                Return _CanUseFarmBarnBuildingNumForCIMPartBuildingReconciliation
            End Get
            Set(value As Boolean)
                _CanUseFarmBarnBuildingNumForCIMPartBuildingReconciliation = value
            End Set
        End Property
        Public Property CanUseFarmBarnBuildingNumForCRMPartBuildingReconciliation As Boolean
            Get
                Return _CanUseFarmBarnBuildingNumForCRMPartBuildingReconciliation
            End Get
            Set(value As Boolean)
                _CanUseFarmBarnBuildingNumForCRMPartBuildingReconciliation = value
            End Set
        End Property
        Public Property CanUseFarmBarnBuildingNumForGARPartBuildingReconciliation As Boolean
            Get
                Return _CanUseFarmBarnBuildingNumForGARPartBuildingReconciliation
            End Get
            Set(value As Boolean)
                _CanUseFarmBarnBuildingNumForGARPartBuildingReconciliation = value
            End Set
        End Property

        'added 7/29/2014 for HOM (specific to mobile home formTypes... FormTypeId 6 = ML-2 - Mobile Home Owner Occupied; 7 = ML-4 - Mobile Home Tenant Occupied
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 20034</remarks>
        Public Property TheftDeductibleLimitId As String 'int; corresponds to CoverageCodeId 20034; static data list
            Get
                Return _TheftDeductibleLimitId
            End Get
            Set(value As String)
                _TheftDeductibleLimitId = value
            End Set
        End Property
        Public Property HomeInPark As Boolean
            Get
                Return _HomeInPark
            End Get
            Set(value As Boolean)
                _HomeInPark = value
            End Set
        End Property
        Public Property MobileHomeConsecutiveMonthsOccupied As String 'int
            Get
                Return _MobileHomeConsecutiveMonthsOccupied
            End Get
            Set(value As String)
                _MobileHomeConsecutiveMonthsOccupied = value
            End Set
        End Property
        Public Property MobileHomeCostNew As String 'dec
            Get
                Return _MobileHomeCostNew
            End Get
            Set(value As String)
                _MobileHomeCostNew = value
            End Set
        End Property
        Public Property MobileHomeLength As String 'int
            Get
                Return _MobileHomeLength
            End Get
            Set(value As String)
                _MobileHomeLength = value
            End Set
        End Property
        Public Property MobileHomeMake As String
            Get
                Return _MobileHomeMake
            End Get
            Set(value As String)
                _MobileHomeMake = value
            End Set
        End Property
        Public Property MobileHomeModel As String
            Get
                Return _MobileHomeModel
            End Get
            Set(value As String)
                _MobileHomeModel = value
            End Set
        End Property
        Public Property MobileHomeParkName As String
            Get
                Return _MobileHomeParkName
            End Get
            Set(value As String)
                _MobileHomeParkName = value
            End Set
        End Property
        Public Property MobileHomePurchasePrice As String 'dec
            Get
                Return _MobileHomePurchasePrice
            End Get
            Set(value As String)
                _MobileHomePurchasePrice = value
            End Set
        End Property
        Public Property MobileHomeSkirtTypeId As String 'int; static data list
            Get
                Return _MobileHomeSkirtTypeId
            End Get
            Set(value As String)
                _MobileHomeSkirtTypeId = value
            End Set
        End Property
        Public Property MobileHomeTieDownTypeId As String 'int; static data list
            Get
                Return _MobileHomeTieDownTypeId
            End Get
            Set(value As String)
                _MobileHomeTieDownTypeId = value
            End Set
        End Property
        Public Property MobileHomeVIN As String
            Get
                Return _MobileHomeVIN
            End Get
            Set(value As String)
                _MobileHomeVIN = value
            End Set
        End Property
        Public Property MobileHomeWidth As String 'int
            Get
                Return _MobileHomeWidth
            End Get
            Set(value As String)
                _MobileHomeWidth = value
            End Set
        End Property
        Public Property PermanentFoundation As Boolean
            Get
                Return _PermanentFoundation
            End Get
            Set(value As Boolean)
                _PermanentFoundation = value
            End Set
        End Property

        Public Property PropertyValuation As QuickQuotePropertyValuation 'added 8/5/2014 for e2Value
            Get
                SetObjectsParent(_PropertyValuation)
                Return _PropertyValuation
            End Get
            Set(value As QuickQuotePropertyValuation)
                _PropertyValuation = value
                SetObjectsParent(_PropertyValuation)
            End Set
        End Property
        Public Property ArchitecturalStyle As String 'added 8/14/2014 for e2Value; corresponds to prop w/ the same name in QuickQuotePropertyValuationRequest
            Get
                Return _ArchitecturalStyle
            End Get
            Set(value As String)
                _ArchitecturalStyle = value
            End Set
        End Property

        'added 10/13/2014 for surcharge premiums... added as modifiers, but prem comes back in coverage... may also need to set Checkbox to True on Coverage
        Public Property SwimmingPoolHotTubSurchargeQuotePremium As String
            Get
                Return _SwimmingPoolHotTubSurchargeQuotePremium
                'Return qqHelper.QuotedPremiumFormat(_SwimmingPoolHotTubSurchargeQuotePremium) '6/15/2015 note: looks like this was missed when premium props were updated to use new function on Get... should probably be updated but don't want to cause any inadvertent issues (since empty string could now come back as $0.00)
            End Get
            Set(value As String)
                _SwimmingPoolHotTubSurchargeQuotePremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_SwimmingPoolHotTubSurchargeQuotePremium)
            End Set
        End Property
        Public Property TrampolineSurchargeQuotePremium As String
            Get
                Return _TrampolineSurchargeQuotePremium
                'Return qqHelper.QuotedPremiumFormat(_TrampolineSurchargeQuotePremium) '6/15/2015 note: looks like this was missed when premium props were updated to use new function on Get... should probably be updated but don't want to cause any inadvertent issues (since empty string could now come back as $0.00)
            End Get
            Set(value As String)
                _TrampolineSurchargeQuotePremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_TrampolineSurchargeQuotePremium)
            End Set
        End Property
        Public Property WoodOrFuelBurningApplianceSurchargeQuotePremium As String
            Get
                Return _WoodOrFuelBurningApplianceSurchargeQuotePremium
                'Return qqHelper.QuotedPremiumFormat(_WoodOrFuelBurningApplianceSurchargeQuotePremium) '6/15/2015 note: looks like this was missed when premium props were updated to use new function on Get... should probably be updated but don't want to cause any inadvertent issues (since empty string could now come back as $0.00)
            End Get
            Set(value As String)
                _WoodOrFuelBurningApplianceSurchargeQuotePremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_WoodOrFuelBurningApplianceSurchargeQuotePremium)
            End Set
        End Property
        'added 10/14/2014 for other HOM coverage premiums
        Public Property DeductibleQuotedPremium As String
            Get
                Return _DeductibleQuotedPremium
                'Return qqHelper.QuotedPremiumFormat(_DeductibleQuotedPremium) '6/15/2015 note: looks like this was missed when premium props were updated to use new function on Get... should probably be updated but don't want to cause any inadvertent issues (since empty string could now come back as $0.00)
            End Get
            Set(value As String)
                _DeductibleQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_DeductibleQuotedPremium)
            End Set
        End Property
        Public Property WindHailDeductibleQuotedPremium As String
            Get
                Return _WindHailDeductibleQuotedPremium
                'Return qqHelper.QuotedPremiumFormat(_WindHailDeductibleQuotedPremium) '6/15/2015 note: looks like this was missed when premium props were updated to use new function on Get... should probably be updated but don't want to cause any inadvertent issues (since empty string could now come back as $0.00)
            End Get
            Set(value As String)
                _WindHailDeductibleQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_WindHailDeductibleQuotedPremium)
            End Set
        End Property
        Public Property A_Dwelling_QuotedPremium As String
            Get
                Return _A_Dwelling_QuotedPremium
                'Return qqHelper.QuotedPremiumFormat(_A_Dwelling_QuotedPremium) '6/15/2015 note: looks like this was missed when premium props were updated to use new function on Get... should probably be updated but don't want to cause any inadvertent issues (since empty string could now come back as $0.00)
            End Get
            Set(value As String)
                _A_Dwelling_QuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_A_Dwelling_QuotedPremium)
            End Set
        End Property
        Public Property B_OtherStructures_QuotedPremium As String
            Get
                Return _B_OtherStructures_QuotedPremium
                'Return qqHelper.QuotedPremiumFormat(_B_OtherStructures_QuotedPremium) '6/15/2015 note: looks like this was missed when premium props were updated to use new function on Get... should probably be updated but don't want to cause any inadvertent issues (since empty string could now come back as $0.00)
            End Get
            Set(value As String)
                _B_OtherStructures_QuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_B_OtherStructures_QuotedPremium)
            End Set
        End Property
        Public Property C_PersonalProperty_QuotedPremium As String
            Get
                Return _C_PersonalProperty_QuotedPremium
                'Return qqHelper.QuotedPremiumFormat(_C_PersonalProperty_QuotedPremium) '6/15/2015 note: looks like this was missed when premium props were updated to use new function on Get... should probably be updated but don't want to cause any inadvertent issues (since empty string could now come back as $0.00)
            End Get
            Set(value As String)
                _C_PersonalProperty_QuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_C_PersonalProperty_QuotedPremium)
            End Set
        End Property
        Public Property D_LossOfUse_QuotedPremium As String
            Get
                Return _D_LossOfUse_QuotedPremium
                'Return qqHelper.QuotedPremiumFormat(_D_LossOfUse_QuotedPremium) '6/15/2015 note: looks like this was missed when premium props were updated to use new function on Get... should probably be updated but don't want to cause any inadvertent issues (since empty string could now come back as $0.00)
            End Get
            Set(value As String)
                _D_LossOfUse_QuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_D_LossOfUse_QuotedPremium)
            End Set
        End Property
        Public Property TheftDeductibleQuotedPremium As String
            Get
                Return _TheftDeductibleQuotedPremium
                'Return qqHelper.QuotedPremiumFormat(_TheftDeductibleQuotedPremium) '6/15/2015 note: looks like this was missed when premium props were updated to use new function on Get... should probably be updated but don't want to cause any inadvertent issues (since empty string could now come back as $0.00)
            End Get
            Set(value As String)
                _TheftDeductibleQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_TheftDeductibleQuotedPremium)
            End Set
        End Property

        'added 10/14/2014 for reconciliation
        Public Property CanUseExclusionNumForExclusionReconciliation As Boolean
            Get
                Return _CanUseExclusionNumForExclusionReconciliation
            End Get
            Set(value As Boolean)
                _CanUseExclusionNumForExclusionReconciliation = value
            End Set
        End Property
        Public Property CanUseInlandMarineNumForInlandMarineReconciliation As Boolean
            Get
                Return _CanUseInlandMarineNumForInlandMarineReconciliation
            End Get
            Set(value As Boolean)
                _CanUseInlandMarineNumForInlandMarineReconciliation = value
            End Set
        End Property
        Public Property CanUsePolicyUnderwritingNumForPolicyUnderwritingReconciliation As Boolean
            Get
                Return _CanUsePolicyUnderwritingNumForPolicyUnderwritingReconciliation
            End Get
            Set(value As Boolean)
                _CanUsePolicyUnderwritingNumForPolicyUnderwritingReconciliation = value
            End Set
        End Property
        Public Property CanUseRvWatercraftNumForRvWatercraftReconciliation As Boolean
            Get
                Return _CanUseRvWatercraftNumForRvWatercraftReconciliation
            End Get
            Set(value As Boolean)
                _CanUseRvWatercraftNumForRvWatercraftReconciliation = value
            End Set
        End Property
        Public Property CanUseSectionCoverageNumForSectionCoverageReconciliation As Boolean
            Get
                'Return _CanUseSectionCoverageNumForSectionCoverageReconciliation
                'updated 10/29/2018
                Return CanUseDiamondNumForSectionCoverageReconciliationGroup.CanUseDiamondNumForReconciliation
            End Get
            Set(value As Boolean)
                '_CanUseSectionCoverageNumForSectionCoverageReconciliation = value
                'updated 10/29/2018
                CanUseDiamondNumForSectionCoverageReconciliationGroup.CanUseDiamondNumForReconciliation = value
            End Set
        End Property
        Public Property CanUseDiamondNumForSectionCoverageReconciliationGroup As QuickQuoteCanUseDiamondNumFlagGroup 'added 10/29/2018
            Get
                If _CanUseDiamondNumForSectionCoverageReconciliationGroup Is Nothing Then
                    _CanUseDiamondNumForSectionCoverageReconciliationGroup = New QuickQuoteCanUseDiamondNumFlagGroup(Me)
                Else
                    SetObjectsParent(_CanUseDiamondNumForSectionCoverageReconciliationGroup)
                End If
                Return _CanUseDiamondNumForSectionCoverageReconciliationGroup
            End Get
            Set(value As QuickQuoteCanUseDiamondNumFlagGroup)
                _CanUseDiamondNumForSectionCoverageReconciliationGroup = value
                SetObjectsParent(_CanUseDiamondNumForSectionCoverageReconciliationGroup)
            End Set
        End Property

        'added 10/29/2014 for HOM
        Public Property TerritoryNumber As String
            Get
                Return _TerritoryNumber
            End Get
            Set(value As String)
                _TerritoryNumber = value
            End Set
        End Property

        'added 11/17/2014 for HOM
        Public Property InlandMarinesTotal_Premium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_InlandMarinesTotal_Premium)
            End Get
            Set(value As String)
                _InlandMarinesTotal_Premium = value
                qqHelper.ConvertToQuotedPremiumFormat(_InlandMarinesTotal_Premium)
            End Set
        End Property
        Public Property InlandMarinesTotal_CoveragePremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_InlandMarinesTotal_CoveragePremium)
            End Get
            Set(value As String)
                _InlandMarinesTotal_CoveragePremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_InlandMarinesTotal_CoveragePremium)
            End Set
        End Property
        Public Property RvWatercraftsTotal_Premium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_RvWatercraftsTotal_Premium)
            End Get
            Set(value As String)
                _RvWatercraftsTotal_Premium = value
                qqHelper.ConvertToQuotedPremiumFormat(_RvWatercraftsTotal_Premium)
            End Set
        End Property
        Public Property RvWatercraftsTotal_CoveragesPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_RvWatercraftsTotal_CoveragesPremium)
            End Get
            Set(value As String)
                _RvWatercraftsTotal_CoveragesPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_RvWatercraftsTotal_CoveragesPremium)
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

        'added 2/25/2015 for Farm
        Public Property IncidentalDwellingCoverages As List(Of QuickQuoteCoverage)
            Get
                SetParentOfListItems(Of QuickQuoteCoverage)(_IncidentalDwellingCoverages, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5E1}")
                Return _IncidentalDwellingCoverages
            End Get
            Set(value As List(Of QuickQuoteCoverage))
                _IncidentalDwellingCoverages = value
                SetParentOfListItems(Of QuickQuoteCoverage)(_IncidentalDwellingCoverages, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5E1}")
            End Set
        End Property
        Public Property HasConvertedIncidentalDwellingCoverages As Boolean
            Get
                Return _HasConvertedIncidentalDwellingCoverages
            End Get
            Set(value As Boolean)
                _HasConvertedIncidentalDwellingCoverages = value
            End Set
        End Property

        'added 2/26/2015
        Public Property Acreages As List(Of QuickQuoteAcreage)
            Get
                SetParentOfListItems(Of QuickQuoteAcreage)(_Acreages, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5E2}")
                Return _Acreages
            End Get
            Set(value As List(Of QuickQuoteAcreage))
                _Acreages = value
                SetParentOfListItems(Of QuickQuoteAcreage)(_Acreages, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5E2}")
            End Set
        End Property
        Public Property CanUseAcreageNumForAcreageReconciliation As Boolean
            Get
                Return _CanUseAcreageNumForAcreageReconciliation
            End Get
            Set(value As Boolean)
                _CanUseAcreageNumForAcreageReconciliation = value
            End Set
        End Property

        Public Property IncomeLosses As List(Of QuickQuoteIncomeLoss)
            Get
                SetParentOfListItems(Of QuickQuoteIncomeLoss)(_IncomeLosses, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5E3}")
                Return _IncomeLosses
            End Get
            Set(value As List(Of QuickQuoteIncomeLoss))
                _IncomeLosses = value
                SetParentOfListItems(Of QuickQuoteIncomeLoss)(_IncomeLosses, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5E3}")
            End Set
        End Property
        Public Property CanUseLossOfIncomeNumForIncomeLossReconciliation As Boolean
            Get
                Return _CanUseLossOfIncomeNumForIncomeLossReconciliation
            End Get
            Set(value As Boolean)
                _CanUseLossOfIncomeNumForIncomeLossReconciliation = value
            End Set
        End Property
        Public Property HasConvertedIncomeLosses As Boolean
            Get
                Return _HasConvertedIncomeLosses
            End Get
            Set(value As Boolean)
                _HasConvertedIncomeLosses = value
            End Set
        End Property

        Public Property ResidentNames As List(Of QuickQuoteResidentName)
            Get
                SetParentOfListItems(Of QuickQuoteResidentName)(_ResidentNames, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5E4}")
                Return _ResidentNames
            End Get
            Set(value As List(Of QuickQuoteResidentName))
                _ResidentNames = value
                SetParentOfListItems(Of QuickQuoteResidentName)(_ResidentNames, "{26E6E852-9AAD-4D51-A11E-13A1E2A5B5E4}")
            End Set
        End Property
        Public Property CanUseResidentNumForResidentNameReconciliation As Boolean
            Get
                Return _CanUseResidentNumForResidentNameReconciliation
            End Get
            Set(value As Boolean)
                _CanUseResidentNumForResidentNameReconciliation = value
            End Set
        End Property
        'added 3/4/2015
        Public Property DwellingTypeId As String 'static data; example had 22 (Type 1)
            Get
                Return _DwellingTypeId
            End Get
            Set(value As String)
                _DwellingTypeId = value
            End Set
        End Property
        Public Property FarmTypeBees As Boolean
            Get
                Return _FarmTypeBees
            End Get
            Set(value As Boolean)
                _FarmTypeBees = value
            End Set
        End Property
        Public Property FarmTypeDairy As Boolean
            Get
                Return _FarmTypeDairy
            End Get
            Set(value As Boolean)
                _FarmTypeDairy = value
            End Set
        End Property
        Public Property FarmTypeFeedLot As Boolean
            Get
                Return _FarmTypeFeedLot
            End Get
            Set(value As Boolean)
                _FarmTypeFeedLot = value
            End Set
        End Property
        Public Property FarmTypeFieldCrops As Boolean
            Get
                Return _FarmTypeFieldCrops
            End Get
            Set(value As Boolean)
                _FarmTypeFieldCrops = value
            End Set
        End Property
        Public Property FarmTypeFlowers As Boolean
            Get
                Return _FarmTypeFlowers
            End Get
            Set(value As Boolean)
                _FarmTypeFlowers = value
            End Set
        End Property
        Public Property FarmTypeFruits As Boolean
            Get
                Return _FarmTypeFruits
            End Get
            Set(value As Boolean)
                _FarmTypeFruits = value
            End Set
        End Property
        Public Property FarmTypeFurbearingAnimals As Boolean
            Get
                Return _FarmTypeFurbearingAnimals
            End Get
            Set(value As Boolean)
                _FarmTypeFurbearingAnimals = value
            End Set
        End Property
        Public Property FarmTypeGreenhouses As Boolean
            Get
                Return _FarmTypeGreenhouses
            End Get
            Set(value As Boolean)
                _FarmTypeGreenhouses = value
            End Set
        End Property
        Public Property FarmTypeHobby As Boolean
            Get
                Return _FarmTypeHobby
            End Get
            Set(value As Boolean)
                _FarmTypeHobby = value
            End Set
        End Property
        Public Property FarmTypeHorse As Boolean
            Get
                Return _FarmTypeHorse
            End Get
            Set(value As Boolean)
                _FarmTypeHorse = value
            End Set
        End Property
        Public Property FarmTypeLivestock As Boolean
            Get
                Return _FarmTypeLivestock
            End Get
            Set(value As Boolean)
                _FarmTypeLivestock = value
            End Set
        End Property
        Public Property FarmTypeMushrooms As Boolean
            Get
                Return _FarmTypeMushrooms
            End Get
            Set(value As Boolean)
                _FarmTypeMushrooms = value
            End Set
        End Property
        Public Property FarmTypeNurseryStock As Boolean
            Get
                Return _FarmTypeNurseryStock
            End Get
            Set(value As Boolean)
                _FarmTypeNurseryStock = value
            End Set
        End Property
        Public Property FarmTypeNuts As Boolean
            Get
                Return _FarmTypeNuts
            End Get
            Set(value As Boolean)
                _FarmTypeNuts = value
            End Set
        End Property
        Public Property FarmTypeOtherDescription As String
            Get
                Return _FarmTypeOtherDescription
            End Get
            Set(value As String)
                _FarmTypeOtherDescription = value
            End Set
        End Property
        Public Property FarmTypePoultry As Boolean
            Get
                Return _FarmTypePoultry
            End Get
            Set(value As Boolean)
                _FarmTypePoultry = value
            End Set
        End Property
        Public Property FarmTypeSod As Boolean
            Get
                Return _FarmTypeSod
            End Get
            Set(value As Boolean)
                _FarmTypeSod = value
            End Set
        End Property
        Public Property FarmTypeSwine As Boolean
            Get
                Return _FarmTypeSwine
            End Get
            Set(value As Boolean)
                _FarmTypeSwine = value
            End Set
        End Property
        Public Property FarmTypeTobacco As Boolean
            Get
                Return _FarmTypeTobacco
            End Get
            Set(value As Boolean)
                _FarmTypeTobacco = value
            End Set
        End Property
        Public Property FarmTypeTurkey As Boolean
            Get
                Return _FarmTypeTurkey
            End Get
            Set(value As Boolean)
                _FarmTypeTurkey = value
            End Set
        End Property
        Public Property FarmTypeVegetables As Boolean
            Get
                Return _FarmTypeVegetables
            End Get
            Set(value As Boolean)
                _FarmTypeVegetables = value
            End Set
        End Property
        Public Property FarmTypeVineyards As Boolean
            Get
                Return _FarmTypeVineyards
            End Get
            Set(value As Boolean)
                _FarmTypeVineyards = value
            End Set
        End Property
        Public Property FarmTypeWorms As Boolean
            Get
                Return _FarmTypeWorms
            End Get
            Set(value As Boolean)
                _FarmTypeWorms = value
            End Set
        End Property
        Public Property LegalDescription As String
            Get
                Return _LegalDescription
            End Get
            Set(value As String)
                _LegalDescription = value
            End Set
        End Property
        Public Property Owns As Boolean
            Get
                Return _Owns
            End Get
            Set(value As Boolean)
                _Owns = value
            End Set
        End Property
        Public Property RoofExclusion As Boolean
            Get
                Return _RoofExclusion
            End Get
            Set(value As Boolean)
                _RoofExclusion = value
            End Set
        End Property
        'added 5/20/2015
        Public Property AcreageOnly As Boolean
            Get
                Return _AcreageOnly
            End Get
            Set(value As Boolean)
                _AcreageOnly = value
            End Set
        End Property
        'added 6/11/2015
        Public Property HobbyFarmCredit As Boolean
            Get
                Return _HobbyFarmCredit
            End Get
            Set(value As Boolean)
                _HobbyFarmCredit = value
            End Set
        End Property
        'added 6/12/2015
        Public Property FireDepartmentAlarm As Boolean
            Get
                Return _FireDepartmentAlarm
            End Get
            Set(value As Boolean)
                _FireDepartmentAlarm = value
            End Set
        End Property
        Public Property PoliceDepartmentTheftAlarm As Boolean
            Get
                Return _PoliceDepartmentTheftAlarm
            End Get
            Set(value As Boolean)
                _PoliceDepartmentTheftAlarm = value
            End Set
        End Property
        Public Property BurglarAlarm_CentralAlarmSystem As Boolean
            Get
                Return _BurglarAlarm_CentralAlarmSystem
            End Get
            Set(value As Boolean)
                _BurglarAlarm_CentralAlarmSystem = value
            End Set
        End Property
        Public Property FireSmokeAlarm_CentralAlarmSystem As Boolean
            Get
                Return _FireSmokeAlarm_CentralAlarmSystem
            End Get
            Set(value As Boolean)
                _FireSmokeAlarm_CentralAlarmSystem = value
            End Set
        End Property
        'added 9/23/2015
        Public Property Farm_L_Liability_QuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_Farm_L_Liability_QuotedPremium)
            End Get
            Set(value As String)
                _Farm_L_Liability_QuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_Farm_L_Liability_QuotedPremium)
            End Set
        End Property
        Public Property Farm_M_Medical_Payments_QuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_Farm_M_Medical_Payments_QuotedPremium)
            End Get
            Set(value As String)
                _Farm_M_Medical_Payments_QuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_Farm_M_Medical_Payments_QuotedPremium)
            End Set
        End Property

        'added 11/2/2015 for DFR
        Public Property A_Dwelling_EC_QuotedPremium As String '70223
            Get
                Return qqHelper.QuotedPremiumFormat(_A_Dwelling_EC_QuotedPremium)
            End Get
            Set(value As String)
                _A_Dwelling_EC_QuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_A_Dwelling_EC_QuotedPremium)
            End Set
        End Property
        Public Property C_Contents_EC_QuotedPremium As String '70224
            Get
                Return qqHelper.QuotedPremiumFormat(_C_Contents_EC_QuotedPremium)
            End Get
            Set(value As String)
                _C_Contents_EC_QuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_C_Contents_EC_QuotedPremium)
            End Set
        End Property
        Public Property D_and_E_EC_QuotedPremium As String '80106
            Get
                Return qqHelper.QuotedPremiumFormat(_D_and_E_EC_QuotedPremium)
            End Get
            Set(value As String)
                _D_and_E_EC_QuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_D_and_E_EC_QuotedPremium)
            End Set
        End Property
        Public Property B_OtherStructures_EC_QuotedPremium As String '70218
            Get
                Return qqHelper.QuotedPremiumFormat(_B_OtherStructures_EC_QuotedPremium)
            End Get
            Set(value As String)
                _B_OtherStructures_EC_QuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_B_OtherStructures_EC_QuotedPremium)
            End Set
        End Property
        Public Property A_Dwelling_VMM_QuotedPremium As String '70225
            Get
                Return qqHelper.QuotedPremiumFormat(_A_Dwelling_VMM_QuotedPremium)
            End Get
            Set(value As String)
                _A_Dwelling_VMM_QuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_A_Dwelling_VMM_QuotedPremium)
            End Set
        End Property
        Public Property C_Contents_VMM_QuotedPremium As String '70226
            Get
                Return qqHelper.QuotedPremiumFormat(_C_Contents_VMM_QuotedPremium)
            End Get
            Set(value As String)
                _C_Contents_VMM_QuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_C_Contents_VMM_QuotedPremium)
            End Set
        End Property
        Public Property D_and_E_VMM_QuotedPremium As String '80107
            Get
                Return qqHelper.QuotedPremiumFormat(_D_and_E_VMM_QuotedPremium)
            End Get
            Set(value As String)
                _D_and_E_VMM_QuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_D_and_E_VMM_QuotedPremium)
            End Set
        End Property
        Public Property B_OtherStructures_VMM_QuotedPremium As String '70220
            Get
                Return qqHelper.QuotedPremiumFormat(_B_OtherStructures_VMM_QuotedPremium)
            End Get
            Set(value As String)
                _B_OtherStructures_VMM_QuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_B_OtherStructures_VMM_QuotedPremium)
            End Set
        End Property

        'added 9/20/2016 for Verisk Protection Class
        Public Property FireDistrictName As String '10/14/2016 note: may need Readonly Prop w/ Protected Friend Sub to Set
            Get
                Return _FireDistrictName
            End Get
            Set(value As String)
                _FireDistrictName = value
            End Set
        End Property
        Public ReadOnly Property ProtectionClassSystemGeneratedId As String
            Get
                Return _ProtectionClassSystemGeneratedId
            End Get
        End Property
        'added 9/24/2016 for Verisk Protection Class
        Public ReadOnly Property ProtectionClassSystemGenerated As String
            Get
                Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.ProtectionClassSystemGeneratedId, _ProtectionClassSystemGeneratedId)
            End Get
        End Property
        Public ReadOnly Property PPCMatchTypeId As String
            Get
                Return _PPCMatchTypeId
            End Get
        End Property
        Public ReadOnly Property PPCMatchType As String
            Get
                Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.PPCMatchTypeId, _PPCMatchTypeId)
            End Get
        End Property

        'added 10/7/2016 to compare before/after address
        Public ReadOnly Property OriginalAddress As QuickQuoteAddress
            Get
                SetObjectsParent(_OriginalAddress)
                Return _OriginalAddress
            End Get
        End Property

        'added 10/14/2016 for Verisk Protection Class
        Public ReadOnly Property FireStationDistance As String
            Get
                Return _FireStationDistance
            End Get
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
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80385; stored in xml at location level</remarks>
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
                If IsNumeric(_TenantAutoLegalLiabilityLimitOfLiability) Then
                    qqHelper.ConvertToLimitFormat(_TenantAutoLegalLiabilityLimitOfLiability)
                End If
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
                Select Case _TenantAutoLegalLiabilityLimitOfLiability.Replace("$", "").Replace(",", "")
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
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80385; stored in xml at location level</remarks>
        Public Property TenantAutoLegalLiabilityQuotedPremium As String
            Get
                'Return _TenantAutoLegalLiabilityQuotedPremium
                Return qqHelper.QuotedPremiumFormat(_TenantAutoLegalLiabilityQuotedPremium)
            End Get
            Set(value As String)
                _TenantAutoLegalLiabilityQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_TenantAutoLegalLiabilityQuotedPremium)
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
                If IsNumeric(_CustomerAutoLegalLiabilityLimitOfLiability) Then
                    qqHelper.ConvertToLimitFormat(_CustomerAutoLegalLiabilityLimitOfLiability)
                End If
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
                Select Case _CustomerAutoLegalLiabilityLimitOfLiability.Replace("$", "").Replace(",", "")
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
                    Case "440"
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
        Public Property CustomerAutoLegalLiabilityQuotedPremium As String
            Get
                'Return _CustomerAutoLegalLiabilityQuotedPremium
                Return qqHelper.QuotedPremiumFormat(_CustomerAutoLegalLiabilityQuotedPremium)
            End Get
            Set(value As String)
                _CustomerAutoLegalLiabilityQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_CustomerAutoLegalLiabilityQuotedPremium)
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
                Return _hasFineArts
            End Get
            Set(value As Boolean)
                _hasFineArts = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80383; stored in xml at location level</remarks>
        Public Property FineArtsQuotedPremium As String
            Get
                'Return _FineArtsQuotedPremium
                Return qqHelper.QuotedPremiumFormat(_FineArtsQuotedPremium)
            End Get
            Set(value As String)
                _FineArtsQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_FineArtsQuotedPremium)
            End Set
        End Property

        'added 5/8/2017 for GAR
        Public Property LiabilityQuotedPremium As String 'covCodeId 10111
            Get
                'Return _LiabilityQuotedPremium
                Return qqHelper.QuotedPremiumFormat(_LiabilityQuotedPremium)
            End Get
            Set(value As String)
                _LiabilityQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_LiabilityQuotedPremium)
            End Set
        End Property
        Public Property LiabilityAggregateLiabilityIncrementTypeId As String 'covDetail; covCodeId 10111
            Get
                Return _LiabilityAggregateLiabilityIncrementTypeId
            End Get
            Set(value As String)
                _LiabilityAggregateLiabilityIncrementTypeId = value
            End Set
        End Property
        Public ReadOnly Property LiabilityAggregateLiabilityIncrementType As String 'added 5/9/2017
            Get
                Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.LiabilityAggregateLiabilityIncrementTypeId, _LiabilityAggregateLiabilityIncrementTypeId)
            End Get
        End Property
        Public Property LiabilityCoverageLimitId As String 'covCodeId 10111
            Get
                Return _LiabilityCoverageLimitId
            End Get
            Set(value As String)
                _LiabilityCoverageLimitId = value
            End Set
        End Property
        Public ReadOnly Property LiabilityCoverageLimit As String 'added 5/9/2017; still needs update to static data values
            Get
                Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.LiabilityCoverageLimitId, _LiabilityCoverageLimitId)
            End Get
        End Property
        Public Property MedicalPaymentsQuotedPremium As String 'covCodeId 10112
            Get
                'Return _MedicalPaymentsQuotedPremium
                Return qqHelper.QuotedPremiumFormat(_MedicalPaymentsQuotedPremium)
            End Get
            Set(value As String)
                _MedicalPaymentsQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_MedicalPaymentsQuotedPremium)
            End Set
        End Property
        Public Property MedicalPaymentsCoverageLimitId As String 'covCodeId 10112
            Get
                Return _MedicalPaymentsCoverageLimitId
            End Get
            Set(value As String)
                _MedicalPaymentsCoverageLimitId = value
            End Set
        End Property
        Public ReadOnly Property MedicalPaymentsCoverageLimit As String 'added 5/9/2017; still needs update to static data values
            Get
                Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.MedicalPaymentsCoverageLimitId, _MedicalPaymentsCoverageLimitId)
            End Get
        End Property
        Public Property HasUninsuredUnderinsuredMotoristBIandPD As Boolean 'covCodeId 10113
            Get
                Return _HasUninsuredUnderinsuredMotoristBIandPD
            End Get
            Set(value As Boolean)
                _HasUninsuredUnderinsuredMotoristBIandPD = value
            End Set
        End Property
        Public Property UninsuredUnderinsuredMotoristBIandPDQuotedPremium As String 'covCodeId 10113; may not be populated
            Get
                'Return _UninsuredUnderinsuredMotoristBIandPDQuotedPremium
                Return qqHelper.QuotedPremiumFormat(_UninsuredUnderinsuredMotoristBIandPDQuotedPremium)
            End Get
            Set(value As String)
                _UninsuredUnderinsuredMotoristBIandPDQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_UninsuredUnderinsuredMotoristBIandPDQuotedPremium)
            End Set
        End Property
        Public Property UninsuredUnderinsuredMotoristBIandPDCoverageLimitId As String 'covCodeId 10113
            Get
                Return _UninsuredUnderinsuredMotoristBIandPDCoverageLimitId
            End Get
            Set(value As String)
                _UninsuredUnderinsuredMotoristBIandPDCoverageLimitId = value
            End Set
        End Property
        Public ReadOnly Property UninsuredUnderinsuredMotoristBIandPDCoverageLimit As String 'added 5/9/2017; still needs update to static data values
            Get
                Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredUnderinsuredMotoristBIandPDCoverageLimitId, _UninsuredUnderinsuredMotoristBIandPDCoverageLimitId)
            End Get
        End Property
        Public Property UninsuredUnderinsuredMotoristBIandPDDeductibleId As String 'covCodeId 10113
            Get
                Return _UninsuredUnderinsuredMotoristBIandPDDeductibleId
            End Get
            Set(value As String)
                _UninsuredUnderinsuredMotoristBIandPDDeductibleId = value
            End Set
        End Property
        Public ReadOnly Property UninsuredUnderinsuredMotoristBIandPDDeductible As String 'added 5/9/2017; still needs update to static data values; 5/11/2017 note: currently just uses policy level deductible for cov 21539 (no prop on QuickQuoteObject; UninsuredMotoristPropertyDamageQuotedPremium is prem prop... should probably add QuickQuoteObject.UninsuredMotoristPropertyDamageDeductibleId)
            Get
                Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredUnderinsuredMotoristBIandPDDeductibleId, _UninsuredUnderinsuredMotoristBIandPDDeductibleId)
            End Get
        End Property
        Public Property PhysicalDamageOtherThanCollisionStandardOpenLotsQuotedPremium As String 'covCodeId 10116
            Get
                'Return _PhysicalDamageOtherThanCollisionStandardOpenLotsQuotedPremium
                Return qqHelper.QuotedPremiumFormat(_PhysicalDamageOtherThanCollisionStandardOpenLotsQuotedPremium)
            End Get
            Set(value As String)
                _PhysicalDamageOtherThanCollisionStandardOpenLotsQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_PhysicalDamageOtherThanCollisionStandardOpenLotsQuotedPremium)
            End Set
        End Property
        Public Property PhysicalDamageOtherThanCollisionStandardOpenLotsManualLimitAmount As String 'covCodeId 10116; added 5/12/2017
            Get
                Return _PhysicalDamageOtherThanCollisionStandardOpenLotsManualLimitAmount
            End Get
            Set(value As String)
                _PhysicalDamageOtherThanCollisionStandardOpenLotsManualLimitAmount = value
                qqHelper.ConvertToLimitFormat(_PhysicalDamageOtherThanCollisionStandardOpenLotsManualLimitAmount)
            End Set
        End Property
        'Public Property PhysicalDamageOtherThanCollisionStandardOpenLotsDeductibleCategoryTypeId As String 'covDetail; covCodeId 10116; removed 5/15/2017 since it's the same for all PD OtherThanCollision covs; will use 1 new field
        '    Get
        '        Return _PhysicalDamageOtherThanCollisionStandardOpenLotsDeductibleCategoryTypeId
        '    End Get
        '    Set(value As String)
        '        _PhysicalDamageOtherThanCollisionStandardOpenLotsDeductibleCategoryTypeId = value
        '    End Set
        'End Property
        'Public ReadOnly Property PhysicalDamageOtherThanCollisionStandardOpenLotsDeductibleCategoryType As String 'added 5/9/2017; removed 5/15/2017 since it's the same for all PD OtherThanCollision covs; will use 1 new field
        '    Get
        '        Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.PhysicalDamageOtherThanCollisionStandardOpenLotsDeductibleCategoryTypeId, _PhysicalDamageOtherThanCollisionStandardOpenLotsDeductibleCategoryTypeId)
        '    End Get
        'End Property
        'Public Property PhysicalDamageOtherThanCollisionStandardOpenLotsOtherThanCollisionTypeId As String 'covDetail; covCodeId 10116; removed 5/15/2017 since it's the same for all PD OtherThanCollision covs; will use 1 new field
        '    Get
        '        Return _PhysicalDamageOtherThanCollisionStandardOpenLotsOtherThanCollisionTypeId
        '    End Get
        '    Set(value As String)
        '        _PhysicalDamageOtherThanCollisionStandardOpenLotsOtherThanCollisionTypeId = value
        '    End Set
        'End Property
        'Public ReadOnly Property PhysicalDamageOtherThanCollisionStandardOpenLotsOtherThanCollisionType As String 'added 5/9/2017
        '    Get
        '        Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.PhysicalDamageOtherThanCollisionStandardOpenLotsOtherThanCollisionTypeId, _PhysicalDamageOtherThanCollisionStandardOpenLotsOtherThanCollisionTypeId)
        '    End Get
        'End Property
        'Public Property PhysicalDamageOtherThanCollisionStandardOpenLotsDeductibleId As String 'covCodeId 10116; removed 5/15/2017 since it's the same for all PD OtherThanCollision covs; will use 1 new field
        '    Get
        '        Return _PhysicalDamageOtherThanCollisionStandardOpenLotsDeductibleId
        '    End Get
        '    Set(value As String)
        '        _PhysicalDamageOtherThanCollisionStandardOpenLotsDeductibleId = value
        '    End Set
        'End Property
        'Public ReadOnly Property PhysicalDamageOtherThanCollisionStandardOpenLotsDeductible As String 'added 5/9/2017; still needs update to static data values; removed 5/15/2017 since it's the same for all PD OtherThanCollision covs; will use 1 new field
        '    Get
        '        Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.PhysicalDamageOtherThanCollisionStandardOpenLotsDeductibleId, _PhysicalDamageOtherThanCollisionStandardOpenLotsDeductibleId)
        '    End Get
        'End Property
        Public Property HasDealersBlanketCollision As Boolean 'covCodeId 10120
            Get
                Return _HasDealersBlanketCollision
            End Get
            Set(value As Boolean)
                _HasDealersBlanketCollision = value
            End Set
        End Property
        Public Property DealersBlanketCollisionQuotedPremium As String 'covCodeId 10120
            Get
                'Return _DealersBlanketCollisionQuotedPremium
                Return qqHelper.QuotedPremiumFormat(_DealersBlanketCollisionQuotedPremium)
            End Get
            Set(value As String)
                _DealersBlanketCollisionQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_DealersBlanketCollisionQuotedPremium)
            End Set
        End Property
        Public Property DealersBlanketCollisionDeductibleId As String 'covCodeId 10120
            Get
                Return _DealersBlanketCollisionDeductibleId
            End Get
            Set(value As String)
                _DealersBlanketCollisionDeductibleId = value
            End Set
        End Property
        Public ReadOnly Property DealersBlanketCollisionDeductible As String 'added 5/9/2017; still needs update to static data values
            Get
                Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.DealersBlanketCollisionDeductibleId, _DealersBlanketCollisionDeductibleId)
            End Get
        End Property
        Public Property GarageKeepersOtherThanCollisionQuotedPremium As String 'covCodeId 10086; may not be populated
            Get
                'Return _GarageKeepersOtherThanCollisionQuotedPremium
                Return qqHelper.QuotedPremiumFormat(_GarageKeepersOtherThanCollisionQuotedPremium)
            End Get
            Set(value As String)
                _GarageKeepersOtherThanCollisionQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_GarageKeepersOtherThanCollisionQuotedPremium)
            End Set
        End Property
        Public Property GarageKeepersOtherThanCollisionManualLimitAmount As String 'covCodeId 10086
            Get
                Return _GarageKeepersOtherThanCollisionManualLimitAmount
            End Get
            Set(value As String)
                _GarageKeepersOtherThanCollisionManualLimitAmount = value
                qqHelper.ConvertToLimitFormat(_GarageKeepersOtherThanCollisionManualLimitAmount)
            End Set
        End Property
        Public ReadOnly Property GarageKeepersOtherThanCollisionDeductibleCategoryTypeId As String 'covCodeId 10086; updated 7/15/2017 to ReadOnly
            Get
                Return _GarageKeepersOtherThanCollisionDeductibleCategoryTypeId
            End Get
            'Set(value As String)
            '    _GarageKeepersOtherThanCollisionDeductibleCategoryTypeId = value
            'End Set
        End Property
        Public ReadOnly Property GarageKeepersOtherThanCollisionDeductibleCategoryType As String 'added 5/9/2017
            Get
                Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.GarageKeepersOtherThanCollisionDeductibleCategoryTypeId, _GarageKeepersOtherThanCollisionDeductibleCategoryTypeId)
            End Get
        End Property
        Public ReadOnly Property GarageKeepersOtherThanCollisionTypeId As String 'covCodeId 10086; updated 7/15/2017 to ReadOnly
            Get
                Return _GarageKeepersOtherThanCollisionTypeId
            End Get
            'Set(value As String)
            '    _GarageKeepersOtherThanCollisionTypeId = value
            'End Set
        End Property
        Public ReadOnly Property GarageKeepersOtherThanCollisionType As String 'added 5/9/2017
            Get
                Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.GarageKeepersOtherThanCollisionTypeId, _GarageKeepersOtherThanCollisionTypeId)
            End Get
        End Property
        Public ReadOnly Property GarageKeepersOtherThanCollisionDeductibleId As String 'covCodeId 10086; updated 7/15/2017 to ReadOnly
            Get
                Return _GarageKeepersOtherThanCollisionDeductibleId
            End Get
            'Set(value As String)
            '    _GarageKeepersOtherThanCollisionDeductibleId = value
            'End Set
        End Property
        Public ReadOnly Property GarageKeepersOtherThanCollisionDeductible As String 'added 5/9/2017; still needs update to static data values
            Get
                Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.GarageKeepersOtherThanCollisionDeductibleId, _GarageKeepersOtherThanCollisionDeductibleId)
            End Get
        End Property
        Public ReadOnly Property GarageKeepersOtherThanCollisionBasisTypeId As String 'added 7/14/2017; covDetail; covCodeId 10086; updated 7/15/2017 to ReadOnly
            Get
                Return _GarageKeepersOtherThanCollisionBasisTypeId
            End Get
            'Set(value As String)
            '    _GarageKeepersOtherThanCollisionBasisTypeId = value
            'End Set
        End Property
        Public Property GarageKeepersCollisionQuotedPremium As String 'covCodeId 10087; may not be populated
            Get
                'Return _GarageKeepersCollisionQuotedPremium
                Return qqHelper.QuotedPremiumFormat(_GarageKeepersCollisionQuotedPremium)
            End Get
            Set(value As String)
                _GarageKeepersCollisionQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_GarageKeepersCollisionQuotedPremium)
            End Set
        End Property
        Public Property GarageKeepersCollisionManualLimitAmount As String 'covCodeId 10087
            Get
                Return _GarageKeepersCollisionManualLimitAmount
            End Get
            Set(value As String)
                _GarageKeepersCollisionManualLimitAmount = value
                qqHelper.ConvertToLimitFormat(_GarageKeepersCollisionManualLimitAmount)
            End Set
        End Property
        Public ReadOnly Property GarageKeepersCollisionDeductibleId As String 'covCodeId 10087; updated 7/15/2017 to ReadOnly
            Get
                Return _GarageKeepersCollisionDeductibleId
            End Get
            'Set(value As String)
            '    _GarageKeepersCollisionDeductibleId = value
            'End Set
        End Property
        Public ReadOnly Property GarageKeepersCollisionDeductible As String 'added 5/9/2017; still needs update to static data values
            Get
                Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.GarageKeepersCollisionDeductibleId, _GarageKeepersCollisionDeductibleId)
            End Get
        End Property
        Public ReadOnly Property GarageKeepersCollisionBasisTypeId As String 'added 7/15/2017; covDetail; covCodeId 10087
            Get
                Return _GarageKeepersCollisionBasisTypeId
            End Get
            'Set(value As String)
            '    _GarageKeepersCollisionBasisTypeId = value
            'End Set
        End Property
        Public Property HasGarageKeepersCoverageExtensions As Boolean 'covCodeId 10126
            Get
                Return _HasGarageKeepersCoverageExtensions
            End Get
            Set(value As Boolean)
                _HasGarageKeepersCoverageExtensions = value
            End Set
        End Property
        Public Property GarageKeepersCoverageExtensionsQuotedPremium As String 'covCodeId 10126; may not be populated
            Get
                'Return _GarageKeepersCoverageExtensionsQuotedPremium
                Return qqHelper.QuotedPremiumFormat(_GarageKeepersCoverageExtensionsQuotedPremium)
            End Get
            Set(value As String)
                _GarageKeepersCoverageExtensionsQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_GarageKeepersCoverageExtensionsQuotedPremium)
            End Set
        End Property
        'added 5/11/2017 for GAR
        Public Property ClassIIEmployees25AndOlder As String
            Get
                Return _ClassIIEmployees25AndOlder
            End Get
            Set(value As String)
                _ClassIIEmployees25AndOlder = value
                qqHelper.ConvertToLimitFormat(_ClassIIEmployees25AndOlder)
            End Set
        End Property
        Public Property ClassIIEmployeesUnderAge25 As String
            Get
                Return _ClassIIEmployeesUnderAge25
            End Get
            Set(value As String)
                _ClassIIEmployeesUnderAge25 = value
                qqHelper.ConvertToLimitFormat(_ClassIIEmployeesUnderAge25)
            End Set
        End Property
        Public Property ClassIOtherEmployees As String
            Get
                Return _ClassIOtherEmployees
            End Get
            Set(value As String)
                _ClassIOtherEmployees = value
                qqHelper.ConvertToLimitFormat(_ClassIOtherEmployees)
            End Set
        End Property
        Public Property ClassIRegularEmployees As String
            Get
                Return _ClassIRegularEmployees
            End Get
            Set(value As String)
                _ClassIRegularEmployees = value
                qqHelper.ConvertToLimitFormat(_ClassIRegularEmployees)
            End Set
        End Property
        Public Property ClassificationTypeId As String
            Get
                Return _ClassificationTypeId
            End Get
            Set(value As String)
                _ClassificationTypeId = value
            End Set
        End Property
        Public Property NumberOfEmployees As String
            Get
                Return _NumberOfEmployees
            End Get
            Set(value As String)
                _NumberOfEmployees = value
                qqHelper.ConvertToLimitFormat(_NumberOfEmployees)
            End Set
        End Property
        Public Property Payroll As String
            Get
                Return _Payroll
            End Get
            Set(value As String)
                _Payroll = value
                qqHelper.ConvertToLimitFormat(_Payroll)
            End Set
        End Property
        Public Property UninsuredUnderinsuredMotoristBIandPDNumberOfPlates As String 'covCodeId 10113; covDetail
            Get
                Return _UninsuredUnderinsuredMotoristBIandPDNumberOfPlates
            End Get
            Set(value As String)
                _UninsuredUnderinsuredMotoristBIandPDNumberOfPlates = value
                qqHelper.ConvertToLimitFormat(_UninsuredUnderinsuredMotoristBIandPDNumberOfPlates)
            End Set
        End Property
        'added 5/15/2017 for GAR
        Public Property PhysicalDamageOtherThanCollisionBuildingQuotedPremium As String 'covCodeId 10115
            Get
                'Return _PhysicalDamageOtherThanCollisionBuildingQuotedPremium
                Return qqHelper.QuotedPremiumFormat(_PhysicalDamageOtherThanCollisionBuildingQuotedPremium)
            End Get
            Set(value As String)
                _PhysicalDamageOtherThanCollisionBuildingQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_PhysicalDamageOtherThanCollisionBuildingQuotedPremium)
            End Set
        End Property
        Public Property PhysicalDamageOtherThanCollisionBuildingManualLimitAmount As String 'covCodeId 10115
            Get
                Return _PhysicalDamageOtherThanCollisionBuildingManualLimitAmount
            End Get
            Set(value As String)
                _PhysicalDamageOtherThanCollisionBuildingManualLimitAmount = value
                qqHelper.ConvertToLimitFormat(_PhysicalDamageOtherThanCollisionBuildingManualLimitAmount)
            End Set
        End Property
        Public Property PhysicalDamageOtherThanCollisionNonStandardOpenLotsQuotedPremium As String 'covCodeId 10117
            Get
                'Return _PhysicalDamageOtherThanCollisionNonStandardOpenLotsQuotedPremium
                Return qqHelper.QuotedPremiumFormat(_PhysicalDamageOtherThanCollisionNonStandardOpenLotsQuotedPremium)
            End Get
            Set(value As String)
                _PhysicalDamageOtherThanCollisionNonStandardOpenLotsQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_PhysicalDamageOtherThanCollisionNonStandardOpenLotsQuotedPremium)
            End Set
        End Property
        Public Property PhysicalDamageOtherThanCollisionNonStandardOpenLotsManualLimitAmount As String 'covCodeId 10117
            Get
                Return _PhysicalDamageOtherThanCollisionNonStandardOpenLotsManualLimitAmount
            End Get
            Set(value As String)
                _PhysicalDamageOtherThanCollisionNonStandardOpenLotsManualLimitAmount = value
                qqHelper.ConvertToLimitFormat(_PhysicalDamageOtherThanCollisionNonStandardOpenLotsManualLimitAmount)
            End Set
        End Property
        Public Property PhysicalDamageOtherThanCollisionMiscellaneousBuildingsQuotedPremium As String 'covCodeId 10118
            Get
                'Return _PhysicalDamageOtherThanCollisionMiscellaneousBuildingsQuotedPremium
                Return qqHelper.QuotedPremiumFormat(_PhysicalDamageOtherThanCollisionMiscellaneousBuildingsQuotedPremium)
            End Get
            Set(value As String)
                _PhysicalDamageOtherThanCollisionMiscellaneousBuildingsQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_PhysicalDamageOtherThanCollisionMiscellaneousBuildingsQuotedPremium)
            End Set
        End Property
        Public Property PhysicalDamageOtherThanCollisionMiscellaneousBuildingsManualLimitAmount As String 'covCodeId 10118
            Get
                Return _PhysicalDamageOtherThanCollisionMiscellaneousBuildingsManualLimitAmount
            End Get
            Set(value As String)
                _PhysicalDamageOtherThanCollisionMiscellaneousBuildingsManualLimitAmount = value
                qqHelper.ConvertToLimitFormat(_PhysicalDamageOtherThanCollisionMiscellaneousBuildingsManualLimitAmount)
            End Set
        End Property
        Public Property PhysicalDamageOtherThanCollisionMiscellaneousOpenLotsQuotedPremium As String 'covCodeId 10119
            Get
                'Return _PhysicalDamageOtherThanCollisionMiscellaneousOpenLotsQuotedPremium
                Return qqHelper.QuotedPremiumFormat(_PhysicalDamageOtherThanCollisionMiscellaneousOpenLotsQuotedPremium)
            End Get
            Set(value As String)
                _PhysicalDamageOtherThanCollisionMiscellaneousOpenLotsQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_PhysicalDamageOtherThanCollisionMiscellaneousOpenLotsQuotedPremium)
            End Set
        End Property
        Public Property PhysicalDamageOtherThanCollisionMiscellaneousOpenLotsManualLimitAmount As String 'covCodeId 10119
            Get
                Return _PhysicalDamageOtherThanCollisionMiscellaneousOpenLotsManualLimitAmount
            End Get
            Set(value As String)
                _PhysicalDamageOtherThanCollisionMiscellaneousOpenLotsManualLimitAmount = value
                qqHelper.ConvertToLimitFormat(_PhysicalDamageOtherThanCollisionMiscellaneousOpenLotsManualLimitAmount)
            End Set
        End Property
        Public Property PhysicalDamageOtherThanCollisionTotalQuotedPremium As String 'SUM of covCodeIds 10115, 10116, 10117, 10118, and 10119
            Get
                'Return _PhysicalDamageOtherThanCollisionTotalQuotedPremium
                Return qqHelper.QuotedPremiumFormat(_PhysicalDamageOtherThanCollisionTotalQuotedPremium)
            End Get
            Set(value As String)
                _PhysicalDamageOtherThanCollisionTotalQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_PhysicalDamageOtherThanCollisionTotalQuotedPremium)
            End Set
        End Property
        Public Property PhysicalDamageOtherThanCollisionTotalManualLimitAmount As String 'SUM of covCodeIds 10115, 10116, 10117, 10118, and 10119
            Get
                Return _PhysicalDamageOtherThanCollisionTotalManualLimitAmount
            End Get
            Set(value As String)
                _PhysicalDamageOtherThanCollisionTotalManualLimitAmount = value
                qqHelper.ConvertToLimitFormat(_PhysicalDamageOtherThanCollisionTotalManualLimitAmount)
            End Set
        End Property
        Public Property PhysicalDamageOtherThanCollisionDeductibleCategoryTypeId As String 'covDetail; covCodeIds 10115, 10116, 10117, 10118, and 10119
            Get
                Return _PhysicalDamageOtherThanCollisionDeductibleCategoryTypeId
            End Get
            Set(value As String)
                _PhysicalDamageOtherThanCollisionDeductibleCategoryTypeId = value
            End Set
        End Property
        Public ReadOnly Property PhysicalDamageOtherThanCollisionDeductibleCategoryType As String
            Get
                Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.PhysicalDamageOtherThanCollisionDeductibleCategoryTypeId, _PhysicalDamageOtherThanCollisionDeductibleCategoryTypeId)
            End Get
        End Property
        Public Property PhysicalDamageOtherThanCollisionTypeId As String 'covDetail; covCodeIds 10115, 10116, 10117, 10118, and 10119
            Get
                Return _PhysicalDamageOtherThanCollisionTypeId
            End Get
            Set(value As String)
                _PhysicalDamageOtherThanCollisionTypeId = value
            End Set
        End Property
        Public ReadOnly Property PhysicalDamageOtherThanCollisionType As String
            Get
                Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.PhysicalDamageOtherThanCollisionTypeId, _PhysicalDamageOtherThanCollisionTypeId)
            End Get
        End Property
        Public Property PhysicalDamageOtherThanCollisionDeductibleId As String 'covCodeIds 10115, 10116, 10117, 10118, and 10119
            Get
                Return _PhysicalDamageOtherThanCollisionDeductibleId
            End Get
            Set(value As String)
                _PhysicalDamageOtherThanCollisionDeductibleId = value
            End Set
        End Property
        Public ReadOnly Property PhysicalDamageOtherThanCollisionDeductible As String
            Get
                Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.PhysicalDamageOtherThanCollisionDeductibleId, _PhysicalDamageOtherThanCollisionDeductibleId)
            End Get
        End Property

        'added 10/27/2017 for MBR Equipment Breakdown
        Public Property EquipmentBreakdown_MBR_PollutantCleanupRemovalCoverageLimitId As String 'covCodeId 80513; has CoverageBasisTypeId 1
            Get
                Return _EquipmentBreakdown_MBR_PollutantCleanupRemovalCoverageLimitId
            End Get
            Set(value As String)
                _EquipmentBreakdown_MBR_PollutantCleanupRemovalCoverageLimitId = value
            End Set
        End Property
        Public ReadOnly Property EquipmentBreakdown_MBR_PollutantCleanupRemovalCoverageLimit As String 'added 10/30/2017
            Get
                Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.EquipmentBreakdown_MBR_PollutantCleanupRemovalCoverageLimitId, _EquipmentBreakdown_MBR_PollutantCleanupRemovalCoverageLimitId)
            End Get
        End Property
        Public Property EquipmentBreakdown_MBR_PollutantCleanupRemovalDeductibleId As String 'covCodeId 80513; has CoverageBasisTypeId 1
            Get
                Return _EquipmentBreakdown_MBR_PollutantCleanupRemovalDeductibleId
            End Get
            Set(value As String)
                _EquipmentBreakdown_MBR_PollutantCleanupRemovalDeductibleId = value
            End Set
        End Property
        Public ReadOnly Property EquipmentBreakdown_MBR_PollutantCleanupRemovalDeductible As String 'added 10/30/2017
            Get
                Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.EquipmentBreakdown_MBR_PollutantCleanupRemovalDeductibleId, _EquipmentBreakdown_MBR_PollutantCleanupRemovalDeductibleId)
            End Get
        End Property
        Public Property EquipmentBreakdown_MBR_PollutantCleanupRemovalQuotedPremium As String 'covCodeId 80513; has CoverageBasisTypeId 1
            Get
                'Return _EquipmentBreakdown_MBR_PollutantCleanupRemovalQuotedPremium
                Return qqHelper.QuotedPremiumFormat(_EquipmentBreakdown_MBR_PollutantCleanupRemovalQuotedPremium)
            End Get
            Set(value As String)
                _EquipmentBreakdown_MBR_PollutantCleanupRemovalQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_EquipmentBreakdown_MBR_PollutantCleanupRemovalQuotedPremium)
            End Set
        End Property
        Public Property EquipmentBreakdown_MBR_RefrigerantContaminationCoverageLimitId As String 'covCodeId 80514; has CoverageBasisTypeId 1
            Get
                Return _EquipmentBreakdown_MBR_RefrigerantContaminationCoverageLimitId
            End Get
            Set(value As String)
                _EquipmentBreakdown_MBR_RefrigerantContaminationCoverageLimitId = value
            End Set
        End Property
        Public ReadOnly Property EquipmentBreakdown_MBR_RefrigerantContaminationCoverageLimit As String 'added 10/30/2017
            Get
                Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.EquipmentBreakdown_MBR_RefrigerantContaminationCoverageLimitId, _EquipmentBreakdown_MBR_RefrigerantContaminationCoverageLimitId)
            End Get
        End Property
        Public Property EquipmentBreakdown_MBR_RefrigerantContaminationDeductibleId As String 'covCodeId 80514; has CoverageBasisTypeId 1
            Get
                Return _EquipmentBreakdown_MBR_RefrigerantContaminationDeductibleId
            End Get
            Set(value As String)
                _EquipmentBreakdown_MBR_RefrigerantContaminationDeductibleId = value
            End Set
        End Property
        Public ReadOnly Property EquipmentBreakdown_MBR_RefrigerantContaminationDeductible As String 'added 10/30/2017
            Get
                Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.EquipmentBreakdown_MBR_RefrigerantContaminationDeductibleId, _EquipmentBreakdown_MBR_RefrigerantContaminationDeductibleId)
            End Get
        End Property
        Public Property EquipmentBreakdown_MBR_RefrigerantContaminationQuotedPremium As String 'covCodeId 80514; has CoverageBasisTypeId 1
            Get
                'Return _EquipmentBreakdown_MBR_RefrigerantContaminationQuotedPremium
                Return qqHelper.QuotedPremiumFormat(_EquipmentBreakdown_MBR_RefrigerantContaminationQuotedPremium)
            End Get
            Set(value As String)
                _EquipmentBreakdown_MBR_RefrigerantContaminationQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_EquipmentBreakdown_MBR_RefrigerantContaminationQuotedPremium)
            End Set
        End Property
        Public Property EquipmentBreakdown_MBR_SpoilageCoverageLimitId As String 'covCodeId 80515; has CoverageBasisTypeId 1
            Get
                Return _EquipmentBreakdown_MBR_SpoilageCoverageLimitId
            End Get
            Set(value As String)
                _EquipmentBreakdown_MBR_SpoilageCoverageLimitId = value
            End Set
        End Property
        Public ReadOnly Property EquipmentBreakdown_MBR_SpoilageCoverageLimit As String 'added 10/30/2017
            Get
                Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.EquipmentBreakdown_MBR_SpoilageCoverageLimitId, _EquipmentBreakdown_MBR_SpoilageCoverageLimitId)
            End Get
        End Property
        Public Property EquipmentBreakdown_MBR_SpoilageDeductibleId As String 'covCodeId 80515; has CoverageBasisTypeId 1
            Get
                Return _EquipmentBreakdown_MBR_SpoilageDeductibleId
            End Get
            Set(value As String)
                _EquipmentBreakdown_MBR_SpoilageDeductibleId = value
            End Set
        End Property
        Public ReadOnly Property EquipmentBreakdown_MBR_SpoilageDeductible As String 'added 10/30/2017
            Get
                Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.EquipmentBreakdown_MBR_SpoilageDeductibleId, _EquipmentBreakdown_MBR_SpoilageDeductibleId)
            End Get
        End Property
        Public Property EquipmentBreakdown_MBR_SpoilageQuotedPremium As String 'covCodeId 80515; has CoverageBasisTypeId 1
            Get
                'Return _EquipmentBreakdown_MBR_SpoilageQuotedPremium
                Return qqHelper.QuotedPremiumFormat(_EquipmentBreakdown_MBR_SpoilageQuotedPremium)
            End Get
            Set(value As String)
                _EquipmentBreakdown_MBR_SpoilageQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_EquipmentBreakdown_MBR_SpoilageQuotedPremium)
            End Set
        End Property
        Public Property EquipmentBreakdown_MBR_UnderwrittenRateQuotedPremium As String 'covCodeId 80521
            Get
                'Return _EquipmentBreakdown_MBR_UnderwrittenRateQuotedPremium
                Return qqHelper.QuotedPremiumFormat(_EquipmentBreakdown_MBR_UnderwrittenRateQuotedPremium)
            End Get
            Set(value As String)
                _EquipmentBreakdown_MBR_UnderwrittenRateQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_EquipmentBreakdown_MBR_UnderwrittenRateQuotedPremium)
            End Set
        End Property
        Public Property EquipmentBreakdown_MBR_TotalQuotedPremium As String 'SUM of covCodeIds 21042, 80513, 80514, 80515, 80521
            Get
                'Return _EquipmentBreakdown_MBR_TotalQuotedPremium
                Return qqHelper.QuotedPremiumFormat(_EquipmentBreakdown_MBR_TotalQuotedPremium)
            End Get
            Set(value As String)
                _EquipmentBreakdown_MBR_TotalQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_EquipmentBreakdown_MBR_TotalQuotedPremium)
            End Set
        End Property
        'added 10/30/2017
        Public Property EquipmentBreakdown_AdjustmentFactor As String 'covCodeId 21042
            Get
                Return _EquipmentBreakdown_AdjustmentFactor
            End Get
            Set(value As String)
                _EquipmentBreakdown_AdjustmentFactor = value
            End Set
        End Property
        'added 11/1/2017
        Public ReadOnly Property EquipmentBreakdownDeductibleIdBackup As String 'covCodeId 21042
            Get
                Return _EquipmentBreakdownDeductibleIdBackup
            End Get
        End Property
        'added 11/21/2017 for HOM 2018 Upgrade
        '--------------------------------------
        Public Property NumberOfUnitsInFireDivision As String
            Get
                Return _NumberOfUnitsInFireDivision
            End Get
            Set(value As String)
                _NumberOfUnitsInFireDivision = value
            End Set
        End Property
        Public Property PrimaryPolicyNumber As String
            Get
                Return _PrimaryPolicyNumber
            End Get
            Set(value As String)
                _PrimaryPolicyNumber = value
            End Set
        End Property

        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>modifier w/ Diamond ModifierTypeId 28 (HOM)</remarks>
        Public Property SprinklerSystem As Boolean
            Get
                Return _sprinklerSystem
            End Get
            Set(value As Boolean)
                _sprinklerSystem = value
            End Set
        End Property

        'added 8/2/2018
        Public ReadOnly Property QuoteStateTakenFrom As helper.QuickQuoteState
            Get
                Return _QuoteStateTakenFrom
            End Get
        End Property

        'added 1/16/2019
        Public ReadOnly Property DisplayNum As Integer
            Get
                Return _DisplayNum
            End Get
        End Property
        Public ReadOnly Property OriginalDisplayNum As Integer
            Get
                Return _OriginalDisplayNum
            End Get
        End Property

        Public Property DetailStatusCode As String 'added 5/15/2019
            Get
                Return _DetailStatusCode
            End Get
            Set(value As String)
                _DetailStatusCode = value
            End Set
        End Property

        Public Property AddedDate As String
            Get
                Return _AddedDate
            End Get
            Set(value As String)
                _AddedDate = value
            End Set
        End Property
        Public Property EffectiveDate As String
            Get
                Return _EffectiveDate
            End Get
            Set(value As String)
                _EffectiveDate = value
            End Set
        End Property
        Public Property LastModifiedDate As String
            Get
                Return _LastModifiedDate
            End Get
            Set(value As String)
                _LastModifiedDate = value
            End Set
        End Property
        Public Property PCAdded_Date As String
            Get
                Return _PCAdded_Date
            End Get
            Set(value As String)
                _PCAdded_Date = value
            End Set
        End Property
        Public Property AddedImageNum As String
            Get
                Return _AddedImageNum
            End Get
            Set(value As String)
                _AddedImageNum = value
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub

        Public Sub New(Parent As QuickQuoteObject)
            MyBase.New()
            SetDefaults()
            SetParent = Parent
        End Sub
        Private Sub SetDefaults()
            _Description = ""
            '_StreetNum = ""
            '_StreetName = ""
            '_City = ""
            '_State = "IN"
            '_Zip = ""
            '_County = ""
            _Address = New QuickQuoteAddress
            _ProtectionClass = ""
            _ProtectionClassId = ""
            _NumberOfPools = ""

            _PropertyDeductibleId = ""

            '3/9/2017 - BOP stuff
            _NumberOfAmusementAreas = ""
            _NumberOfPlaygrounds = ""
            _PoolsQuotedPremium = ""
            _AmusementsQuotedPremium = ""
            _PlaygroundsQuotedPremium = ""

            _EquipmentBreakdownDeductible = ""
            _EquipmentBreakdownDeductibleId = ""
            _EquipmentBreakdownDeductibleQuotedPremium = ""
            _EquipmentBreakdownDeductibleMinimumRequiredByClassCode = "" '3/9/2017 - BOP stuff
            _MoneySecuritiesOnPremises = ""
            _MoneySecuritiesOffPremises = ""
            _MoneySecuritiesQuotedPremium = ""
            _MoneySecuritiesQuotedPremium_OnPremises = ""
            _MoneySecuritiesQuotedPremium_OffPremises = ""
            _OutdoorSignsLimit = ""
            _OutdoorSignsQuotedPremium = ""

            '_Buildings = New Generic.List(Of QuickQuoteBuilding)
            _Buildings = Nothing 'added 8/4/2014
            '_LocationCoverages = New Generic.List(Of QuickQuoteCoverage)
            _LocationCoverages = Nothing 'added 8/4/2014

            '_GLClassifications = New Generic.List(Of QuickQuoteGLClassification)
            _GLClassifications = Nothing 'added 8/4/2014

            '_Classifications = New Generic.List(Of QuickQuoteClassification)
            _Classifications = Nothing 'added 8/4/2014

            _Name = New QuickQuoteName
            _Name.NameAddressSourceId = "13" 'Location

            _GL_PremisesQuotedPremium = ""
            _GL_ProductsQuotedPremium = ""

            _HasBuilding = False
            _HasClassification = False

            _CauseOfLossTypeId = ""
            _CauseOfLossType = ""
            _DeductibleId = ""
            _Deductible = ""
            _CoinsuranceTypeId = ""
            _CoinsuranceType = ""
            _ValuationMethodTypeId = ""
            _ValuationMethodType = ""

            _EquipmentBreakdownOccupancyTypeId = ""
            _ClassificationCode = New QuickQuoteClassificationCode

            _FeetToFireHydrant = ""
            _MilesToFireDepartment = ""

            '_ScheduledCoverages = New Generic.List(Of QuickQuoteScheduledCoverage)
            _ScheduledCoverages = Nothing 'added 8/4/2014
            '_PropertyInTheOpenRecords = New Generic.List(Of QuickQuotePropertyInTheOpenRecord)
            _PropertyInTheOpenRecords = Nothing 'added 8/4/2014

            _WindstormOrHailPercentageDeductibleId = ""
            _WindstormOrHailPercentageDeductible = ""
            _WindstormOrHailMinimumDollarDeductibleId = ""
            _WindstormOrHailMinimumDollarDeductible = ""
            _EarthquakeApplies = False

            'added 4/17/2013 for CPR to total up Property in the Open coverage premiums
            _PropertyInTheOpenRecordsTotal_QuotedPremium = ""
            _PropertyInTheOpenRecordsTotal_EQ_Premium = ""
            _PropertyInTheOpenRecordsTotal_QuotedPremium_With_EQ = ""

            _Acreage = ""
            _CondoRentedTypeId = ""
            _ConstructionTypeId = ""
            _DeductibleLimitId = ""
            _WindHailDeductibleLimitId = ""
            _DayEmployees = False
            _DaytimeOccupancy = False
            _FamilyUnitsId = ""
            _FireDepartmentDistanceId = ""
            _FireHydrantDistanceId = ""
            _FormTypeId = ""
            _FoundationTypeId = ""
            _LastCostEstimatorDate = ""
            _MarketValue = ""
            _NumberOfFamiliesId = ""
            _OccupancyCodeId = ""
            _PrimaryResidence = False
            _ProgramTypeId = ""
            _NumberOfApartments = ""
            _NumberOfSolidFuelBurningUnits = ""
            _RebuildCost = ""
            _Remarks = ""
            _SquareFeet = ""
            _StructureTypeId = ""
            _YearBuilt = ""
            _A_Dwelling_Limit = ""
            _A_Dwelling_LimitIncreased = ""
            _A_Dwelling_LimitIncluded = ""
            _A_Dwelling_Calc = "" 'added 12/4/2014
            _B_OtherStructures_Limit = ""
            _B_OtherStructures_LimitIncreased = ""
            _B_OtherStructures_LimitIncluded = ""
            _B_OtherStructures_Calc = "" 'added 12/10/2014
            _C_PersonalProperty_Limit = ""
            _C_PersonalProperty_LimitIncreased = ""
            _C_PersonalProperty_LimitIncluded = ""
            _C_PersonalProperty_Calc = "" 'added 12/10/2014
            _D_LossOfUse_Limit = ""
            _D_LossOfUse_LimitIncreased = ""
            _D_LossOfUse_LimitIncluded = ""
            _D_LossOfUse_Calc = "" 'added 12/10/2014
            '_AdditionalInterests = New Generic.List(Of QuickQuoteAdditionalInterest)
            _AdditionalInterests = Nothing 'added 8/4/2014
            _Updates = New QuickQuoteUpdatesRecord 'added 7/31/2013
            '_Modifiers = New List(Of QuickQuoteModifier) 'added 7/31/2013
            _Modifiers = Nothing 'added 8/4/2014
            _MultiPolicyDiscount = False
            _MatureHomeownerDiscount = False
            _NewHomeDiscount = False
            _SelectMarketCredit = False
            _BurglarAlarm_LocalAlarmSystem = False
            _BurglarAlarm_CentralStationAlarmSystem = False
            _FireSmokeAlarm_LocalAlarmSystem = False
            _FireSmokeAlarm_CentralStationAlarmSystem = False
            _FireSmokeAlarm_SmokeAlarm = False
            _SprinklerSystem_AllExcept = False
            _SprinklerSystem_AllIncluding = False
            _TrampolineSurcharge = False
            _WoodOrFuelBurningApplianceSurcharge = False
            _SwimmingPoolHotTubSurcharge = False 'added 7/28/2014
            '_SectionCoverages = New List(Of QuickQuoteSectionCoverage)
            _SectionCoverages = Nothing 'added 8/4/2014
            '_Exclusions = New List(Of QuickQuoteExclusion)
            _Exclusions = Nothing 'added 8/4/2014
            '_InlandMarines = New List(Of QuickQuoteInlandMarine)
            _InlandMarines = Nothing 'added 8/4/2014
            '_RvWatercrafts = New List(Of QuickQuoteRvWatercraft)
            _RvWatercrafts = Nothing 'added 8/4/2014
            '_SectionICoverages = New List(Of QuickQuoteSectionICoverage)
            _SectionICoverages = Nothing 'added 8/4/2014
            '_SectionIICoverages = New List(Of QuickQuoteSectionIICoverage)
            _SectionIICoverages = Nothing 'added 8/4/2014
            '_SectionIAndIICoverages = New List(Of QuickQuoteSectionIAndIICoverage)
            _SectionIAndIICoverages = Nothing 'added 8/4/2014
            '_PolicyUnderwritings = New Generic.List(Of QuickQuotePolicyUnderwriting)
            _PolicyUnderwritings = Nothing 'added 8/4/2014
            _NumberOfDaysRented = ""
            _NumberOfUnitsId = ""
            _UsageTypeId = ""
            _VacancyFromDate = ""
            _VacancyToDate = ""

            'added 2/18/2014
            _HasConvertedCoverages = False
            _HasConvertedModifiers = False
            _HasConvertedScheduledCoverages = False
            _HasConvertedSectionCoverages = False

            _PremiumFullterm = "" 'added 4/2/2014
            _BuildingsTotal_PremiumFullTerm = "" 'added 4/2/2014

            'added 4/23/2014 for reconciliation
            _LocationNum = ""
            _HasLocationAddressChanged = False
            _CanUseFarmBarnBuildingNumForBuildingReconciliation = False
            _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = False 'added 4/29/2014
            'added 10/17/2018 for multi-state
            _LocationNum_MasterPart = ""
            _LocationNum_CGLPart = ""
            _LocationNum_CPRPart = ""
            _LocationNum_CIMPart = ""
            _LocationNum_CRMPart = ""
            _LocationNum_GARPart = ""
            'added 10/18/2018 for multi-state
            _CanUseFarmBarnBuildingNumForMasterPartBuildingReconciliation = False
            _CanUseFarmBarnBuildingNumForCGLPartBuildingReconciliation = False
            _CanUseFarmBarnBuildingNumForCPRPartBuildingReconciliation = False
            _CanUseFarmBarnBuildingNumForCIMPartBuildingReconciliation = False
            _CanUseFarmBarnBuildingNumForCRMPartBuildingReconciliation = False
            _CanUseFarmBarnBuildingNumForGARPartBuildingReconciliation = False

            'added 7/29/2014 for HOM (specific to mobile home formTypes... FormTypeId 6 = ML-2 - Mobile Home Owner Occupied; 7 = ML-4 - Mobile Home Tenant Occupied
            _TheftDeductibleLimitId = "" 'int; corresponds to CoverageCodeId 20034; static data list
            _HomeInPark = False
            _MobileHomeConsecutiveMonthsOccupied = "" 'int
            _MobileHomeCostNew = "" 'dec
            _MobileHomeLength = "" 'int
            _MobileHomeMake = ""
            _MobileHomeModel = ""
            _MobileHomeParkName = ""
            _MobileHomePurchasePrice = "" 'dec
            _MobileHomeSkirtTypeId = "" 'int; static data list
            _MobileHomeTieDownTypeId = "" 'int; static data list
            _MobileHomeVIN = ""
            _MobileHomeWidth = "" 'int
            _PermanentFoundation = False

            _PropertyValuation = New QuickQuotePropertyValuation 'added 8/5/2014 for e2Value
            _ArchitecturalStyle = "" 'added 8/14/2014 for e2Value; corresponds to prop w/ the same name in QuickQuotePropertyValuationRequest

            'added 10/13/2014 for surcharge premiums... added as modifiers, but prem comes back in coverage... may also need to set Checkbox to True on Coverage
            _SwimmingPoolHotTubSurchargeQuotePremium = ""
            _TrampolineSurchargeQuotePremium = ""
            _WoodOrFuelBurningApplianceSurchargeQuotePremium = ""
            'added 10/14/2014 for other HOM coverage premiums
            _DeductibleQuotedPremium = ""
            _WindHailDeductibleQuotedPremium = ""
            _A_Dwelling_QuotedPremium = ""
            _B_OtherStructures_QuotedPremium = ""
            _C_PersonalProperty_QuotedPremium = ""
            _D_LossOfUse_QuotedPremium = ""
            _TheftDeductibleQuotedPremium = ""

            'added 10/14/2014 for reconciliation
            _CanUseExclusionNumForExclusionReconciliation = False
            _CanUseInlandMarineNumForInlandMarineReconciliation = False
            _CanUsePolicyUnderwritingNumForPolicyUnderwritingReconciliation = False
            _CanUseRvWatercraftNumForRvWatercraftReconciliation = False
            '_CanUseSectionCoverageNumForSectionCoverageReconciliation = False'removed 10/29/2018
            _CanUseDiamondNumForSectionCoverageReconciliationGroup = New QuickQuoteCanUseDiamondNumFlagGroup(Me)

            'added 10/29/2014 for HOM
            _TerritoryNumber = ""

            'added 11/17/2014 for HOM
            _InlandMarinesTotal_Premium = ""
            _InlandMarinesTotal_CoveragePremium = ""
            _RvWatercraftsTotal_Premium = ""
            _RvWatercraftsTotal_CoveragesPremium = ""

            _CanUseScheduledCoverageNumForScheduledCoverageReconciliation = False 'added 1/22/2015

            'added 2/25/2015 for Farm
            '_IncidentalDwellingCoverages = New List(Of QuickQuoteCoverage)
            _IncidentalDwellingCoverages = Nothing
            _HasConvertedIncidentalDwellingCoverages = False
            'added 2/26/2015
            '_Acreages = New List(Of QuickQuoteAcreage)
            _Acreages = Nothing
            _CanUseAcreageNumForAcreageReconciliation = False
            '_IncomeLosses = New List(Of QuickQuoteIncomeLoss)
            _IncomeLosses = Nothing
            _CanUseLossOfIncomeNumForIncomeLossReconciliation = False
            _HasConvertedIncomeLosses = False
            '_ResidentNames = New List(Of QuickQuoteResidentName)
            _ResidentNames = Nothing
            _CanUseResidentNumForResidentNameReconciliation = False
            'added 3/4/2015
            _DwellingTypeId = "" 'static data; example had 22 (Type 1)
            _FarmTypeBees = False
            _FarmTypeDairy = False
            _FarmTypeFeedLot = False
            _FarmTypeFieldCrops = False
            _FarmTypeFlowers = False
            _FarmTypeFruits = False
            _FarmTypeFurbearingAnimals = False
            _FarmTypeGreenhouses = False
            _FarmTypeHobby = False
            _FarmTypeHorse = False
            _FarmTypeLivestock = False
            _FarmTypeMushrooms = False
            _FarmTypeNurseryStock = False
            _FarmTypeNuts = False
            _FarmTypeOtherDescription = ""
            _FarmTypePoultry = False
            _FarmTypeSod = False
            _FarmTypeSwine = False
            _FarmTypeTobacco = False
            _FarmTypeTurkey = False
            _FarmTypeVegetables = False
            _FarmTypeVineyards = False
            _FarmTypeWorms = False
            _LegalDescription = ""
            _Owns = False
            _RoofExclusion = False
            'added 5/20/2015
            _AcreageOnly = False
            'added 6/11/2015
            _HobbyFarmCredit = False
            'added 6/12/2015
            _FireDepartmentAlarm = False
            _PoliceDepartmentTheftAlarm = False
            _BurglarAlarm_CentralAlarmSystem = False
            _FireSmokeAlarm_CentralAlarmSystem = False
            'added 9/23/2015
            _Farm_L_Liability_QuotedPremium = ""
            _Farm_M_Medical_Payments_QuotedPremium = ""

            'added 11/2/2015 for DFR
            _A_Dwelling_EC_QuotedPremium = "" '70223
            _C_Contents_EC_QuotedPremium = "" '70224
            _D_and_E_EC_QuotedPremium = "" '80106
            _B_OtherStructures_EC_QuotedPremium = "" '70218
            _A_Dwelling_VMM_QuotedPremium = "" '70225
            _C_Contents_VMM_QuotedPremium = "" '70226
            _D_and_E_VMM_QuotedPremium = "" '80107
            _B_OtherStructures_VMM_QuotedPremium = "" '70220

            'added 9/20/2016 for Verisk Protection Class
            _FireDistrictName = ""
            _ProtectionClassSystemGeneratedId = ""
            'added 9/24/2016 for Verisk Protection Class
            _PPCMatchTypeId = ""

            'added 10/7/2016 to compare before/after address
            _OriginalAddress = Nothing

            'added 10/14/2016 for Verisk Protection Class
            _FireStationDistance = ""

            'added 2/20/2017
            _CanUseClassificationNumForClassificationReconciliation = False

            '3/9/2017 - BOP stuff
            _HasTenantAutoLegalLiability = False
            _TenantAutoLegalLiabilityDeductibleId = ""
            _TenantAutoLegalLiabilityLimitOfLiabilityId = ""
            _TenantAutoLegalLiabilityDeductible = ""
            _TenantAutoLegalLiabilityLimitOfLiability = ""
            _TenantAutoLegalLiabilityQuotedPremium = ""
            _HasCustomerAutoLegalLiability = False
            _CustomerAutoLegalLiabilityDeductibleId = ""
            _CustomerAutoLegalLiabilityLimitOfLiabilityId = ""
            _CustomerAutoLegalLiabilityDeductible = ""
            _CustomerAutoLegalLiabilityLimitOfLiability = ""
            _CustomerAutoLegalLiabilityQuotedPremium = ""

            _hasFineArts = False
            _FineArtsQuotedPremium = ""

            'added 5/8/2017 for GAR
            _LiabilityQuotedPremium = "" 'covCodeId 10111
            _LiabilityAggregateLiabilityIncrementTypeId = "" 'covDetail; covCodeId 10111
            _LiabilityCoverageLimitId = "" 'covCodeId 10111
            _MedicalPaymentsQuotedPremium = "" 'covCodeId 10112
            _MedicalPaymentsCoverageLimitId = "" 'covCodeId 10112
            _HasUninsuredUnderinsuredMotoristBIandPD = False 'covCodeId 10113
            _UninsuredUnderinsuredMotoristBIandPDQuotedPremium = "" 'covCodeId 10113; may not be populated
            _UninsuredUnderinsuredMotoristBIandPDCoverageLimitId = "" 'covCodeId 10113
            _UninsuredUnderinsuredMotoristBIandPDDeductibleId = "" 'covCodeId 10113
            _PhysicalDamageOtherThanCollisionStandardOpenLotsQuotedPremium = "" 'covCodeId 10116
            _PhysicalDamageOtherThanCollisionStandardOpenLotsManualLimitAmount = "" 'covCodeId 10116; added 5/12/2017
            '_PhysicalDamageOtherThanCollisionStandardOpenLotsDeductibleCategoryTypeId = "" 'covDetail; covCodeId 10116; removed 5/15/2017 since it's the same for all PD OtherThanCollision covs; will use 1 new field
            '_PhysicalDamageOtherThanCollisionStandardOpenLotsOtherThanCollisionTypeId = "" 'covDetail; covCodeId 10116; removed 5/15/2017 since it's the same for all PD OtherThanCollision covs; will use 1 new field
            '_PhysicalDamageOtherThanCollisionStandardOpenLotsDeductibleId = "" 'covCodeId 10116; removed 5/15/2017 since it's the same for all PD OtherThanCollision covs; will use 1 new field
            _HasDealersBlanketCollision = False 'covCodeId 10120
            _DealersBlanketCollisionQuotedPremium = "" 'covCodeId 10120
            _DealersBlanketCollisionDeductibleId = "" 'covCodeId 10120
            _GarageKeepersOtherThanCollisionQuotedPremium = "" 'covCodeId 10086; may not be populated
            _GarageKeepersOtherThanCollisionManualLimitAmount = "" 'covCodeId 10086
            _GarageKeepersOtherThanCollisionDeductibleCategoryTypeId = "" 'covCodeId 10086
            _GarageKeepersOtherThanCollisionTypeId = "" 'covCodeId 10086
            _GarageKeepersOtherThanCollisionDeductibleId = "" 'covCodeId 10086
            _GarageKeepersOtherThanCollisionBasisTypeId = "" 'added 7/14/2017; covDetail; covCodeId 10086
            _GarageKeepersCollisionQuotedPremium = "" 'covCodeId 10087; may not be populated
            _GarageKeepersCollisionManualLimitAmount = "" 'covCodeId 10087
            _GarageKeepersCollisionDeductibleId = "" 'covCodeId 10087
            _GarageKeepersCollisionBasisTypeId = "" 'added 7/15/2017; covDetail; covCodeId 10087
            _HasGarageKeepersCoverageExtensions = False 'covCodeId 10126
            _GarageKeepersCoverageExtensionsQuotedPremium = "" 'covCodeId 10126; may not be populated
            'added 5/11/2017 for GAR
            _ClassIIEmployees25AndOlder = ""
            _ClassIIEmployeesUnderAge25 = ""
            _ClassIOtherEmployees = ""
            _ClassIRegularEmployees = ""
            _ClassificationTypeId = ""
            _NumberOfEmployees = ""
            _Payroll = ""
            _UninsuredUnderinsuredMotoristBIandPDNumberOfPlates = "" 'covCodeId 10113; covDetail
            'added 5/15/2017 for GAR
            _PhysicalDamageOtherThanCollisionBuildingQuotedPremium = "" 'covCodeId 10115
            _PhysicalDamageOtherThanCollisionBuildingManualLimitAmount = "" 'covCodeId 10115
            _PhysicalDamageOtherThanCollisionNonStandardOpenLotsQuotedPremium = "" 'covCodeId 10117
            _PhysicalDamageOtherThanCollisionNonStandardOpenLotsManualLimitAmount = "" 'covCodeId 10117
            _PhysicalDamageOtherThanCollisionMiscellaneousBuildingsQuotedPremium = "" 'covCodeId 10118
            _PhysicalDamageOtherThanCollisionMiscellaneousBuildingsManualLimitAmount = "" 'covCodeId 10118
            _PhysicalDamageOtherThanCollisionMiscellaneousOpenLotsQuotedPremium = "" 'covCodeId 10119
            _PhysicalDamageOtherThanCollisionMiscellaneousOpenLotsManualLimitAmount = "" 'covCodeId 10119
            _PhysicalDamageOtherThanCollisionTotalQuotedPremium = "" 'SUM of covCodeIds 10115, 10116, 10117, 10118, and 10119
            _PhysicalDamageOtherThanCollisionTotalManualLimitAmount = "" 'SUM of covCodeIds 10115, 10116, 10117, 10118, and 10119
            _PhysicalDamageOtherThanCollisionDeductibleCategoryTypeId = "" 'covDetail; covCodeIds 10115, 10116, 10117, 10118, and 10119
            _PhysicalDamageOtherThanCollisionTypeId = "" 'covDetail; covCodeIds 10115, 10116, 10117, 10118, and 10119
            _PhysicalDamageOtherThanCollisionDeductibleId = "" 'covCodeIds 10115, 10116, 10117, 10118, and 10119

            'added 11/21/2017 for HOM 2018 Upgrade
            '--------------------------------------
            _NumberOfUnitsInFireDivision = ""
            _PrimaryPolicyNumber = ""
            _sprinklerSystem = False
            '--------------------------------------

            'added 10/27/2017 for MBR Equipment Breakdown
            _EquipmentBreakdown_MBR_PollutantCleanupRemovalCoverageLimitId = "" 'covCodeId 80513; has CoverageBasisTypeId 1
            _EquipmentBreakdown_MBR_PollutantCleanupRemovalDeductibleId = "" 'covCodeId 80513; has CoverageBasisTypeId 1
            _EquipmentBreakdown_MBR_PollutantCleanupRemovalQuotedPremium = "" 'covCodeId 80513; has CoverageBasisTypeId 1
            _EquipmentBreakdown_MBR_RefrigerantContaminationCoverageLimitId = "" 'covCodeId 80514; has CoverageBasisTypeId 1
            _EquipmentBreakdown_MBR_RefrigerantContaminationDeductibleId = "" 'covCodeId 80514; has CoverageBasisTypeId 1
            _EquipmentBreakdown_MBR_RefrigerantContaminationQuotedPremium = "" 'covCodeId 80514; has CoverageBasisTypeId 1
            _EquipmentBreakdown_MBR_SpoilageCoverageLimitId = "" 'covCodeId 80515; has CoverageBasisTypeId 1
            _EquipmentBreakdown_MBR_SpoilageDeductibleId = "" 'covCodeId 80515; has CoverageBasisTypeId 1
            _EquipmentBreakdown_MBR_SpoilageQuotedPremium = "" 'covCodeId 80515; has CoverageBasisTypeId 1
            _EquipmentBreakdown_MBR_UnderwrittenRateQuotedPremium = "" 'covCodeId 80521
            _EquipmentBreakdown_MBR_TotalQuotedPremium = "" 'SUM of covCodeIds 21042, 80513, 80514, 80515, 80521
            'added 10/30/2017
            _EquipmentBreakdown_AdjustmentFactor = "" 'covCodeId 21042
            'added 11/1/2017
            _EquipmentBreakdownDeductibleIdBackup = "" 'covCodeId 21042

            'added 8/2/2018
            _QuoteStateTakenFrom = helper.QuickQuoteState.None

            'added 1/16/2019
            _DisplayNum = 0
            _OriginalDisplayNum = 0
            _OkayToUseDisplayNum = QuickQuoteHelperClass.WhenToSetType.None

            _DetailStatusCode = "" 'added 5/15/2019

            _AddedDate = ""
            _EffectiveDate = ""
            _LastModifiedDate = ""
            _PCAdded_Date = ""
            _AddedImageNum = ""

        End Sub

        ''' <summary>
        ''' used to parse thru coverages and set different properties
        ''' </summary>
        ''' <remarks>always executed when xml is parsed; doesn't specifically needed to be called by developer code</remarks>
        Public Sub ParseThruCoverages() 'added 4/8/2015
            ParseThruCoverages(_LocationCoverages)
        End Sub
        ''' <summary>
        ''' used to parse thru coverages and set different properties
        ''' </summary>
        ''' <remarks>always executed when xml is parsed; doesn't specifically needed to be called by developer code</remarks>
        ''' 'Public Sub ParseThruCoverages()
        Public Sub ParseThruCoverages(ByVal covs As List(Of QuickQuoteCoverage), Optional ByVal packagePartType As QuickQuoteXML.QuickQuotePackagePartType = QuickQuoteXML.QuickQuotePackagePartType.None) 'added new params 4/8/2015 for CPP
            'If _LocationCoverages IsNot Nothing AndAlso _LocationCoverages.Count > 0 Then
            'updated 4/8/2015 to use param
            If covs IsNot Nothing AndAlso covs.Count > 0 Then
                'For Each cov As QuickQuoteCoverage In _LocationCoverages
                'updated 4/8/2015 to use param
                For Each cov As QuickQuoteCoverage In covs
                    Select Case cov.CoverageCodeId
                        Case "21059" 'Checkbox; 3/20/2013 note: Swimming Pools
                            If cov.Checkbox = True Then
                                _NumberOfPools = cov.NumberOfSwimmingPools
                                PoolsQuotedPremium = cov.FullTermPremium 'added 6/8/2017; must've been missed when converting BOP upgrade logic from DiamondQuickQuote to QuickQuote
                            End If
                        Case "21060" 'Checkbox; 7/7/2016 note: Amusements; 3/9/2017 - BOP stuff
                            If cov.Checkbox = True Then
                                _NumberOfAmusementAreas = cov.NumberOfAmusementAreas
                                AmusementAreasQuotedPremium = cov.FullTermPremium
                            End If
                        Case "80386" 'Checkbox; 7/7/2016 note: Playgrounds; 3/9/2017 - BOP stuff
                            If cov.Checkbox = True Then
                                _NumberOfPlaygrounds = cov.NumberOfLocations
                                PlaygroundsQuotedPremium = cov.FullTermPremium
                            End If
                        Case "21042" 'Checkbox; 3/20/2013 note: Equipment Breakdown; 11/2/2017 note: BOP, CPR only
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty Then 'added IF 4/10/2015; may not be needed
                                If cov.Checkbox = True Then
                                    EquipmentBreakdownDeductibleId = cov.DeductibleId
                                    EquipmentBreakdownDeductibleQuotedPremium = cov.FullTermPremium
                                    EquipmentBreakdownOccupancyTypeId = cov.CommercialOccupancyTypeId 'added 9/27/2012 for CPR
                                    'added 10/27/2017 for MBR Equipment Breakdown
                                    EquipmentBreakdown_MBR_TotalQuotedPremium = qqHelper.getSum(_EquipmentBreakdown_MBR_TotalQuotedPremium, cov.FullTermPremium) 'SUM of covCodeIds 21042, 80513, 80514, 80515, 80521
                                    'added 10/30/2017
                                    EquipmentBreakdown_AdjustmentFactor = cov.AdjustmentFactor
                                End If
                            End If
                        Case "21040" 'Edit; 3/20/2013 note: Money & Securities: On Premises
                            MoneySecuritiesOnPremises = cov.ManualLimitAmount
                            MoneySecuritiesQuotedPremium_OnPremises = cov.FullTermPremium
                        Case "21041" 'Edit; 3/20/2013 note: Money & Securities: Off Premises
                            MoneySecuritiesOffPremises = cov.ManualLimitAmount
                            MoneySecuritiesQuotedPremium_OffPremises = cov.FullTermPremium
                        Case "149" 'Edit; 3/20/2013 note: Outdoor Signs
                            OutdoorSignsLimit = cov.ManualLimitAmount
                            OutdoorSignsQuotedPremium = cov.FullTermPremium

                            'added 7/26/2013 for HOM
                        Case "70021" 'Edit: Location_A_Dwelling
                            A_Dwelling_Limit = cov.ManualLimitAmount
                            A_Dwelling_LimitIncluded = cov.ManualLimitIncluded
                            A_Dwelling_LimitIncreased = cov.ManualLimitIncreased
                            A_Dwelling_QuotedPremium = cov.FullTermPremium 'added 10/14/2014
                            A_Dwelling_Calc = cov.Calc 'added 12/4/2014
                        Case "70022" 'Edit: Location_B_Other_Structures
                            B_OtherStructures_Limit = cov.ManualLimitAmount
                            B_OtherStructures_LimitIncluded = cov.ManualLimitIncluded
                            B_OtherStructures_LimitIncreased = cov.ManualLimitIncreased
                            B_OtherStructures_QuotedPremium = cov.FullTermPremium 'added 10/14/2014
                            B_OtherStructures_Calc = cov.Calc 'added 12/10/2014
                        Case "70023" 'Edit: Location_C_Personal_Property
                            C_PersonalProperty_Limit = cov.ManualLimitAmount
                            C_PersonalProperty_LimitIncluded = cov.ManualLimitIncluded
                            C_PersonalProperty_LimitIncreased = cov.ManualLimitIncreased
                            C_PersonalProperty_QuotedPremium = cov.FullTermPremium 'added 10/14/2014
                            C_PersonalProperty_Calc = cov.Calc 'added 12/10/2014
                        Case "70024" 'Edit: Location_D_Loss_of_Use
                            D_LossOfUse_Limit = cov.ManualLimitAmount
                            D_LossOfUse_LimitIncluded = cov.ManualLimitIncluded
                            D_LossOfUse_LimitIncreased = cov.ManualLimitIncreased
                            D_LossOfUse_QuotedPremium = cov.FullTermPremium 'added 10/14/2014
                            D_LossOfUse_Calc = cov.Calc 'added 12/10/2014
                        Case "70025" 'Combo: Location_A_B_C_D_Deductible
                            DeductibleLimitId = cov.CoverageLimitId
                            DeductibleQuotedPremium = cov.FullTermPremium 'added 10/14/2014
                        Case "70216" 'Combo: Wind/Hail Deductible
                            WindHailDeductibleLimitId = cov.CoverageLimitId
                            WindHailDeductibleQuotedPremium = cov.FullTermPremium 'added 10/14/2014
                        Case "80399" '11/2/2017 note: BOP only
                            PropertyDeductibleId = cov.CoverageLimitId
                        Case "80108" 'CheckBox: Trampoline Surcharge (need to use checkbox prop since coverage is always there); 7/29/2014 note: not sure if this needs to be added w/ Modifier... only Mod was used when saving un-acquired quote in UI... haven't tried acquiring and then saving/rating... may just be needed to get surcharge premium
                            TrampolineSurchargeQuotePremium = cov.FullTermPremium 'added 10/13/2014
                        Case "80109" 'CheckBox: Wood or Fuel Burning Appliance Surcharge (need to use checkbox prop since coverage is always there); 7/29/2014 note: not sure if this needs to be added w/ Modifier... only Mod was used when saving un-acquired quote in UI... haven't tried acquiring and then saving/rating... may just be needed to get surcharge premium
                            WoodOrFuelBurningApplianceSurchargeQuotePremium = cov.FullTermPremium 'added 10/13/2014
                        Case "20034" 'Combo: Theft Deductible; 7/29/2014 note: appears to be specific to mobile home formtypes... CoverageLimitId 21 = 250
                            TheftDeductibleLimitId = cov.CoverageLimitId 'added code 7/29/2014
                            TheftDeductibleQuotedPremium = cov.FullTermPremium 'added 10/14/2014
                        Case "80319" 'CheckBox: Swimming Pool/Hot Tub Surcharge (need to use checkbox prop since coverage is always there... after certain effDate); added here 7/29/2014 (wasn't in previous HOM versions); 7/29/2014 note: not sure if this needs to be added w/ Modifier... only Mod was used when saving un-acquired quote in UI... haven't tried acquiring and then saving/rating... may just be needed to get surcharge premium
                            SwimmingPoolHotTubSurchargeQuotePremium = cov.FullTermPremium 'added 10/13/2014

                            'added 3/4/2015 for Farm
                            'Case "70021" 'Edit: Location_A_Dwelling; already added above for HOM; uses ManualLimitAmount, ManualLimitIncreased

                            'Case "70022" 'Edit: Location_B_Other_Structures; already added above for HOM; uses ManualLimitAmount, ManualLimitIncluded, ManualLimitIncreased

                            'Case "70023" 'Edit: Location_C_Personal_Property; already added above for HOM; uses ManualLimitAmount, ManualLimitIncluded, ManualLimitIncreased

                            'Case "70024" 'Edit: Location_D_Loss_of_Use; already added above for HOM; uses ManualLimitAmount, ManualLimitIncluded

                            'Case "70025" 'Combo: Location_A_B_C_D_Deductible; already added above for HOM; uses CoverageLimitId

                        Case "80249" 'CheckBox: RVWatercraft - Minimum Premium Adjustment; example doesn't have anything... cov must not be there

                        Case "40078" 'CheckBox: Location_Solid_Fuel_Burning_Units; uses Checkbox true; 6/12/2015 note: this appears to be the checkbox on the Location Supplemental Surcharges screen... UI has field for #, which appears to get passed to Location.NumberOfSolidFuelBurningUnits

                        Case "40079" 'CheckBox: Location_Amendment_of_Vacancy_or_Unoccupancy; example doesn't have anything... cov must not be there; 6/12/2015 note: this appears to be the checkbox on the Location Supplemental Surcharges screen... UI has fields for dates, which appear to get passed to Location.VacancyFromDate and Location.VacancyToDate

                        Case "40124" 'CheckBox: Location - Wood Roof RC Settlement Terms Surcharge; example doesn't have anything... cov must not be there; 6/12/2015 note: this appears to be the checkbox on the Location Supplemental Surcharges screen

                        Case "80135" 'CheckBox: Farm_L_Liability_Location; uses Checkbox true
                            'updated w/ logic 9/23/2015
                            If cov.Checkbox = True Then
                                Farm_L_Liability_QuotedPremium = cov.FullTermPremium
                            End If
                        Case "80136" 'CheckBox: Farm_M_Medical_Payments_Location; uses Checkbox true
                            If cov.Checkbox = True Then
                                Farm_M_Medical_Payments_QuotedPremium = cov.FullTermPremium
                            End If
                            'Case "80109" 'CheckBox: Wood or Fuel Burning Appliance Surcharge; already added above for HOM; uses Checkbox true

                            'Case "80108" 'CheckBox: Trampoline Surcharge; already added above for HOM; example doesn't have anything... cov must not be there


                            'added 11/2/2015 for DFR
                        Case "70223" 'Edit: Dwelling EC Base Premium; FullTermPremium
                            A_Dwelling_EC_QuotedPremium = cov.FullTermPremium
                        Case "70224" 'Edit: Contents EC Base Premium; FullTermPremium
                            C_Contents_EC_QuotedPremium = cov.FullTermPremium
                        Case "80106" 'Edit: Cov. D & E EC; FullTermPremium
                            D_and_E_EC_QuotedPremium = cov.FullTermPremium
                        Case "70218" 'Edit: Specific Other Structures Dwelling EC; FullTermPremium
                            B_OtherStructures_EC_QuotedPremium = cov.FullTermPremium
                        Case "70225" 'Edit: Dwelling VMM Base Premium; no prem or not on sample quote
                            A_Dwelling_VMM_QuotedPremium = cov.FullTermPremium
                        Case "70226" 'Edit: Contents VMM Base Premium; no prem or not on sample quote
                            C_Contents_VMM_QuotedPremium = cov.FullTermPremium
                        Case "80107" 'Edit: Cov. D & E V&MM; no prem or not on sample quote
                            D_and_E_VMM_QuotedPremium = cov.FullTermPremium
                        Case "70220" 'Edit: Specific Other Structures Dwelling VMM; no prem or not on sample quote
                            B_OtherStructures_VMM_QuotedPremium = cov.FullTermPremium

                            '3/9/2017 - BOP stuff
                        Case "80383"
                            _HasTenantAutoLegalLiability = True
                            TenantAutoLegalLiabilityDeductibleId = cov.DeductibleId
                            TenantAutoLegalLiabilityLimitOfLiabilityId = cov.CoverageLimitId
                            TenantAutoLegalLiabilityQuotedPremium = cov.FullTermPremium
                        Case "80385"
                            _HasCustomerAutoLegalLiability = True
                            CustomerAutoLegalLiabilityDeductibleId = cov.DeductibleId
                            CustomerAutoLegalLiabilityLimitOfLiabilityId = cov.CoverageLimitId
                            CustomerAutoLegalLiabilityQuotedPremium = cov.FullTermPremium
                        Case "170"
                            _hasFineArts = cov.Checkbox
                            FineArtsQuotedPremium = cov.FullTermPremium

                            'added 5/5/2017 for GAR
                        Case "10111" 'Combo: Liability - Location (GAR only)
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.Garage Then
                                'GAR example values (file:///C:/Users/domin/Documents/GAR1000098(1143275-1)GKLL.xml)
                                'cov.FullTermPremium = "5438.00"
                                'cov.Checkbox = True
                                'cov.AggregateLiabilityIncrementTypeId = "8" '3 x Limit
                                'cov.CoverageBasisTypeId = "1"
                                'cov.ShouldSyncWithMasterCoverage = True
                                'cov.CoverageLimitId = "56" '1,000,000
                                'added 5/8/2017
                                LiabilityQuotedPremium = cov.FullTermPremium
                                LiabilityAggregateLiabilityIncrementTypeId = cov.AggregateLiabilityIncrementTypeId
                                LiabilityCoverageLimitId = cov.CoverageLimitId
                                '5/11/2017 note: currently just uses policy level deductible for cov 21552 (no prop on QuickQuoteObject; Liability_UM_UIM_LimitId is limit prop... should probably add QuickQuoteObject.Liability_UM_UIM_DeductibleId if this is ever needed)
                            End If
                        Case "10112" 'Combo: Medical Payments - Location (GAR only)
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.Garage Then
                                'GAR example values (file:///C:/Users/domin/Documents/GAR1000098(1143275-1)GKLL.xml)
                                'cov.FullTermPremium = "49.00"
                                'cov.Checkbox = True
                                'cov.CoverageLimitId = "15" '5,000
                                'added 5/8/2017
                                MedicalPaymentsQuotedPremium = cov.FullTermPremium
                                MedicalPaymentsCoverageLimitId = cov.CoverageLimitId
                            End If
                        Case "10113" 'CheckBox: Uninsured / Underinsured Motorist BI and PD - Location (GAR only)
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.Garage Then
                                'GAR example values (file:///C:/Users/domin/Documents/GAR1000098(1143275-1)GKLL.xml)
                                'no cov.FullTermPremium
                                'cov.Checkbox = True
                                'cov.CoverageLimitId = "56"
                                'cov.DeductibleId = "11" 'No Deductible
                                '5/8/2017 note for GAR: premium doesn't appear to be at Policy level either - Case "21541" 'CheckBox: Garagekeepers Other than Collision (CAP, GAR)
                                '5/9/2017 note: CoverageDetail.NumberOfPlates = Number of Dealer Plates textbox
                                'added 5/8/2017
                                If cov.Checkbox = True Then
                                    _HasUninsuredUnderinsuredMotoristBIandPD = True
                                    UninsuredUnderinsuredMotoristBIandPDQuotedPremium = cov.FullTermPremium
                                    UninsuredUnderinsuredMotoristBIandPDCoverageLimitId = cov.CoverageLimitId
                                    UninsuredUnderinsuredMotoristBIandPDDeductibleId = cov.DeductibleId
                                    UninsuredUnderinsuredMotoristBIandPDNumberOfPlates = cov.NumberOfPlates 'added 5/11/2017
                                End If
                            End If
                        Case "10115" 'Edit: Physical Damage Other Than Collision Building (GAR only)
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.Garage Then
                                'added 5/15/2017 for GAR
                                PhysicalDamageOtherThanCollisionBuildingQuotedPremium = cov.FullTermPremium
                                PhysicalDamageOtherThanCollisionTotalQuotedPremium = qqHelper.getSum(_PhysicalDamageOtherThanCollisionTotalQuotedPremium, cov.FullTermPremium) 'SUM of covCodeIds 10115, 10116, 10117, 10118, and 10119
                                PhysicalDamageOtherThanCollisionBuildingManualLimitAmount = cov.ManualLimitAmount
                                PhysicalDamageOtherThanCollisionTotalManualLimitAmount = qqHelper.getSum(_PhysicalDamageOtherThanCollisionTotalManualLimitAmount, cov.ManualLimitAmount) 'SUM of covCodeIds 10115, 10116, 10117, 10118, and 10119
                                If qqHelper.IsPositiveIntegerString(_PhysicalDamageOtherThanCollisionDeductibleCategoryTypeId) = False Then 'good for all PD OtherThanCollision covs
                                    PhysicalDamageOtherThanCollisionDeductibleCategoryTypeId = cov.DeductibleCategoryTypeId
                                End If
                                If qqHelper.IsPositiveIntegerString(_PhysicalDamageOtherThanCollisionTypeId) = False Then 'good for all PD OtherThanCollision covs
                                    PhysicalDamageOtherThanCollisionTypeId = cov.OtherThanCollisionTypeId
                                End If
                                If qqHelper.IsPositiveIntegerString(_PhysicalDamageOtherThanCollisionDeductibleId) = False Then 'good for all PD OtherThanCollision covs
                                    PhysicalDamageOtherThanCollisionDeductibleId = cov.DeductibleId
                                End If
                            End If
                        Case "10116" 'Edit: Physical Damage Other Than Collision Standard Open Lots (GAR only)
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.Garage Then
                                'GAR example values (file:///C:/Users/domin/Documents/GAR1000098(1143275-1)GKLL.xml)
                                'cov.FullTermPremium = "1833.00"
                                'no cov.Checkbox
                                'cov.ManualLimitAmount = 250000.00 'added 5/12/2017
                                'cov.DeductibleCategoryTypeId = "3"
                                'cov.OtherThanCollisionTypeId = "3"
                                'cov.DeductibleId = "7"
                                'added 5/8/2017
                                PhysicalDamageOtherThanCollisionStandardOpenLotsQuotedPremium = cov.FullTermPremium
                                PhysicalDamageOtherThanCollisionStandardOpenLotsManualLimitAmount = cov.ManualLimitAmount 'covCodeId 10116; added 5/12/2017
                                'PhysicalDamageOtherThanCollisionStandardOpenLotsDeductibleCategoryTypeId = cov.DeductibleCategoryTypeId ' removed 5/15/2017 since it's the same for all PD OtherThanCollision covs; will use 1 new field
                                'PhysicalDamageOtherThanCollisionStandardOpenLotsOtherThanCollisionTypeId = cov.OtherThanCollisionTypeId 'removed 5/15/2017 since it's the same for all PD OtherThanCollision covs; will use 1 new field
                                'PhysicalDamageOtherThanCollisionStandardOpenLotsDeductibleId = cov.DeductibleId 'removed 5/15/2017 since it's the same for all PD OtherThanCollision covs; will use 1 new field
                                'added 5/15/2017 for GAR
                                PhysicalDamageOtherThanCollisionTotalQuotedPremium = qqHelper.getSum(_PhysicalDamageOtherThanCollisionTotalQuotedPremium, cov.FullTermPremium) 'SUM of covCodeIds 10115, 10116, 10117, 10118, and 10119
                                PhysicalDamageOtherThanCollisionTotalManualLimitAmount = qqHelper.getSum(_PhysicalDamageOtherThanCollisionTotalManualLimitAmount, cov.ManualLimitAmount) 'SUM of covCodeIds 10115, 10116, 10117, 10118, and 10119
                                If qqHelper.IsPositiveIntegerString(_PhysicalDamageOtherThanCollisionDeductibleCategoryTypeId) = False Then 'good for all PD OtherThanCollision covs
                                    PhysicalDamageOtherThanCollisionDeductibleCategoryTypeId = cov.DeductibleCategoryTypeId
                                End If
                                If qqHelper.IsPositiveIntegerString(_PhysicalDamageOtherThanCollisionTypeId) = False Then 'good for all PD OtherThanCollision covs
                                    PhysicalDamageOtherThanCollisionTypeId = cov.OtherThanCollisionTypeId
                                End If
                                If qqHelper.IsPositiveIntegerString(_PhysicalDamageOtherThanCollisionDeductibleId) = False Then 'good for all PD OtherThanCollision covs
                                    PhysicalDamageOtherThanCollisionDeductibleId = cov.DeductibleId
                                End If
                            End If
                        Case "10117" 'Physical Damage Other Than Collision Non-Standard Open Lots (GAR only); added 5/15/2017
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.Garage Then
                                PhysicalDamageOtherThanCollisionNonStandardOpenLotsQuotedPremium = cov.FullTermPremium
                                PhysicalDamageOtherThanCollisionTotalQuotedPremium = qqHelper.getSum(_PhysicalDamageOtherThanCollisionTotalQuotedPremium, cov.FullTermPremium) 'SUM of covCodeIds 10115, 10116, 10117, 10118, and 10119
                                PhysicalDamageOtherThanCollisionNonStandardOpenLotsManualLimitAmount = cov.ManualLimitAmount
                                PhysicalDamageOtherThanCollisionTotalManualLimitAmount = qqHelper.getSum(_PhysicalDamageOtherThanCollisionTotalManualLimitAmount, cov.ManualLimitAmount) 'SUM of covCodeIds 10115, 10116, 10117, 10118, and 10119
                                If qqHelper.IsPositiveIntegerString(_PhysicalDamageOtherThanCollisionDeductibleCategoryTypeId) = False Then 'good for all PD OtherThanCollision covs
                                    PhysicalDamageOtherThanCollisionDeductibleCategoryTypeId = cov.DeductibleCategoryTypeId
                                End If
                                If qqHelper.IsPositiveIntegerString(_PhysicalDamageOtherThanCollisionTypeId) = False Then 'good for all PD OtherThanCollision covs
                                    PhysicalDamageOtherThanCollisionTypeId = cov.OtherThanCollisionTypeId
                                End If
                                If qqHelper.IsPositiveIntegerString(_PhysicalDamageOtherThanCollisionDeductibleId) = False Then 'good for all PD OtherThanCollision covs
                                    PhysicalDamageOtherThanCollisionDeductibleId = cov.DeductibleId
                                End If
                            End If
                        Case "10118" 'Physical Damage Other Than Collision Miscellaneous Buildings (GAR only); added 5/15/2017
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.Garage Then
                                PhysicalDamageOtherThanCollisionMiscellaneousBuildingsQuotedPremium = cov.FullTermPremium
                                PhysicalDamageOtherThanCollisionTotalQuotedPremium = qqHelper.getSum(_PhysicalDamageOtherThanCollisionTotalQuotedPremium, cov.FullTermPremium) 'SUM of covCodeIds 10115, 10116, 10117, 10118, and 10119
                                PhysicalDamageOtherThanCollisionMiscellaneousBuildingsManualLimitAmount = cov.ManualLimitAmount
                                PhysicalDamageOtherThanCollisionTotalManualLimitAmount = qqHelper.getSum(_PhysicalDamageOtherThanCollisionTotalManualLimitAmount, cov.ManualLimitAmount) 'SUM of covCodeIds 10115, 10116, 10117, 10118, and 10119
                                If qqHelper.IsPositiveIntegerString(_PhysicalDamageOtherThanCollisionDeductibleCategoryTypeId) = False Then 'good for all PD OtherThanCollision covs
                                    PhysicalDamageOtherThanCollisionDeductibleCategoryTypeId = cov.DeductibleCategoryTypeId
                                End If
                                If qqHelper.IsPositiveIntegerString(_PhysicalDamageOtherThanCollisionTypeId) = False Then 'good for all PD OtherThanCollision covs
                                    PhysicalDamageOtherThanCollisionTypeId = cov.OtherThanCollisionTypeId
                                End If
                                If qqHelper.IsPositiveIntegerString(_PhysicalDamageOtherThanCollisionDeductibleId) = False Then 'good for all PD OtherThanCollision covs
                                    PhysicalDamageOtherThanCollisionDeductibleId = cov.DeductibleId
                                End If
                            End If
                        Case "10119" 'Physical Damage Other Than Collision Miscellaneous Open Lots (GAR only); added 5/15/2017
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.Garage Then
                                PhysicalDamageOtherThanCollisionMiscellaneousOpenLotsQuotedPremium = cov.FullTermPremium
                                PhysicalDamageOtherThanCollisionTotalQuotedPremium = qqHelper.getSum(_PhysicalDamageOtherThanCollisionTotalQuotedPremium, cov.FullTermPremium) 'SUM of covCodeIds 10115, 10116, 10117, 10118, and 10119
                                PhysicalDamageOtherThanCollisionMiscellaneousOpenLotsManualLimitAmount = cov.ManualLimitAmount
                                PhysicalDamageOtherThanCollisionTotalManualLimitAmount = qqHelper.getSum(_PhysicalDamageOtherThanCollisionTotalManualLimitAmount, cov.ManualLimitAmount) 'SUM of covCodeIds 10115, 10116, 10117, 10118, and 10119
                                If qqHelper.IsPositiveIntegerString(_PhysicalDamageOtherThanCollisionDeductibleCategoryTypeId) = False Then 'good for all PD OtherThanCollision covs
                                    PhysicalDamageOtherThanCollisionDeductibleCategoryTypeId = cov.DeductibleCategoryTypeId
                                End If
                                If qqHelper.IsPositiveIntegerString(_PhysicalDamageOtherThanCollisionTypeId) = False Then 'good for all PD OtherThanCollision covs
                                    PhysicalDamageOtherThanCollisionTypeId = cov.OtherThanCollisionTypeId
                                End If
                                If qqHelper.IsPositiveIntegerString(_PhysicalDamageOtherThanCollisionDeductibleId) = False Then 'good for all PD OtherThanCollision covs
                                    PhysicalDamageOtherThanCollisionDeductibleId = cov.DeductibleId
                                End If
                            End If
                        Case "10120" 'CheckBox: Dealer's Blanket Collision (GAR only)
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.Garage Then
                                'GAR example values (file:///C:/Users/domin/Documents/GAR1000098(1143275-1)GKLL.xml)
                                'cov.FullTermPremium = "1125.00"
                                'cov.Checkbox = True
                                'cov.DeductibleId = "8"
                                'added 5/8/2017
                                If cov.Checkbox = True Then
                                    _HasDealersBlanketCollision = True
                                    DealersBlanketCollisionQuotedPremium = cov.FullTermPremium
                                    DealersBlanketCollisionDeductibleId = cov.DeductibleId
                                End If
                            End If
                        Case "10086" 'Edit: Garagekeepers Other than Collision (CAP, GAR)
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.Garage Then
                                'GAR example values (file:///C:/Users/domin/Documents/GAR1000098(1143275-1)GKLL.xml)
                                'no cov.FullTermPremium
                                'no cov.Checkbox
                                'cov.ManualLimitAmount = "100000.00"
                                'cov.DeductibleCategoryTypeId = "3"
                                'cov.OtherThanCollisionTypeId = "3"
                                'cov.DeductibleId = "7"
                                '5/8/2017 note: also had BasisTypeId; same as 21541 at Policy level
                                '5/8/2017 note for GAR: premium at Policy level - Case "21541" 'CheckBox: Garagekeepers Other than Collision (CAP, GAR)
                                'added 5/8/2017
                                GarageKeepersOtherThanCollisionQuotedPremium = cov.FullTermPremium
                                GarageKeepersOtherThanCollisionManualLimitAmount = cov.ManualLimitAmount
                                'GarageKeepersOtherThanCollisionDeductibleCategoryTypeId = cov.DeductibleCategoryTypeId
                                'GarageKeepersOtherThanCollisionTypeId = cov.OtherThanCollisionTypeId
                                'GarageKeepersOtherThanCollisionDeductibleId = cov.DeductibleId
                                'GarageKeepersOtherThanCollisionBasisTypeId = cov.BasisTypeId 'added 7/14/2017
                                'updated 7/17/2017 after changing properties to ReadOnly
                                Set_GarageKeepersOtherThanCollisionDeductibleCategoryTypeId(cov.DeductibleCategoryTypeId)
                                Set_GarageKeepersOtherThanCollisionTypeId(cov.OtherThanCollisionTypeId)
                                Set_GarageKeepersOtherThanCollisionDeductibleId(cov.DeductibleId)
                                Set_GarageKeepersOtherThanCollisionBasisTypeId(cov.BasisTypeId)
                            End If
                        Case "10087" 'Edit: Garagekeepers Collision (CAP, GAR)
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.Garage Then
                                'GAR example values (file:///C:/Users/domin/Documents/GAR1000098(1143275-1)GKLL.xml)
                                'no cov.FullTermPremium
                                'no cov.Checkbox
                                'cov.ManualLimitAmount = "100000.00"
                                'cov.DeductibleId = "8"
                                '7/14/2017 note: didn't see BasisTypeId here like on 21542 at Policy level
                                '5/8/2017 note for GAR: premium at Policy level - Case "21542" 'CheckBox: Garagekeepers Collision (CAP, GAR)
                                'added 5/8/2017
                                GarageKeepersCollisionQuotedPremium = cov.FullTermPremium
                                GarageKeepersCollisionManualLimitAmount = cov.ManualLimitAmount
                                'GarageKeepersCollisionDeductibleId = cov.DeductibleId
                                'GarageKeepersCollisionBasisTypeId = cov.BasisTypeId 'added 7/15/2017; covDetail; covCodeId 10087
                                'updated 7/17/2017 after changing properties to ReadOnly
                                Set_GarageKeepersCollisionDeductibleId(cov.DeductibleId)
                                Set_GarageKeepersCollisionBasisTypeId(cov.BasisTypeId)
                            End If
                        Case "10126" 'CheckBox: Garagekeepers Coverage Extensions (CAP, GAR)
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.Garage Then
                                'GAR example values (file:///C:/Users/domin/Documents/GAR1000098(1143275-1)GKLL.xml)
                                'no cov.FullTermPremium
                                'cov.Checkbox = True
                                'added 5/8/2017
                                If cov.Checkbox = True Then
                                    _HasGarageKeepersCoverageExtensions = True
                                    GarageKeepersCoverageExtensionsQuotedPremium = cov.FullTermPremium
                                End If
                            End If

                            'added 10/27/2017 for MBR Equipment Breakdown
                        Case "80513" 'Combo: Pollutant Cleanup and Removal (BOP, CPR)
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty Then
                                EquipmentBreakdown_MBR_PollutantCleanupRemovalCoverageLimitId = cov.CoverageLimitId
                                EquipmentBreakdown_MBR_PollutantCleanupRemovalDeductibleId = cov.DeductibleId
                                EquipmentBreakdown_MBR_PollutantCleanupRemovalQuotedPremium = cov.FullTermPremium
                                EquipmentBreakdown_MBR_TotalQuotedPremium = qqHelper.getSum(_EquipmentBreakdown_MBR_TotalQuotedPremium, cov.FullTermPremium) 'SUM of covCodeIds 21042, 80513, 80514, 80515, 80521
                            End If
                        Case "80514" 'Combo: Refrigerant Contamination (BOP, CPR)
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty Then
                                EquipmentBreakdown_MBR_RefrigerantContaminationCoverageLimitId = cov.CoverageLimitId
                                EquipmentBreakdown_MBR_RefrigerantContaminationDeductibleId = cov.DeductibleId
                                EquipmentBreakdown_MBR_RefrigerantContaminationQuotedPremium = cov.FullTermPremium
                                EquipmentBreakdown_MBR_TotalQuotedPremium = qqHelper.getSum(_EquipmentBreakdown_MBR_TotalQuotedPremium, cov.FullTermPremium) 'SUM of covCodeIds 21042, 80513, 80514, 80515, 80521
                            End If
                        Case "80515" 'Combo: Spoilage (BOP, CPR)
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty Then
                                EquipmentBreakdown_MBR_SpoilageCoverageLimitId = cov.CoverageLimitId
                                EquipmentBreakdown_MBR_SpoilageDeductibleId = cov.DeductibleId
                                EquipmentBreakdown_MBR_SpoilageQuotedPremium = cov.FullTermPremium
                                EquipmentBreakdown_MBR_TotalQuotedPremium = qqHelper.getSum(_EquipmentBreakdown_MBR_TotalQuotedPremium, cov.FullTermPremium) 'SUM of covCodeIds 21042, 80513, 80514, 80515, 80521
                            End If
                        Case "80521" 'CheckBox: MBR Underwritten Rate (BOP, CPR)
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty Then
                                If cov.Checkbox = True Then
                                    EquipmentBreakdown_MBR_UnderwrittenRateQuotedPremium = cov.FullTermPremium
                                    EquipmentBreakdown_MBR_TotalQuotedPremium = qqHelper.getSum(_EquipmentBreakdown_MBR_TotalQuotedPremium, cov.FullTermPremium) 'SUM of covCodeIds 21042, 80513, 80514, 80515, 80521
                                End If
                            End If

                    End Select
                Next
            End If
        End Sub
        'added 8/20/2012
        ''' <summary>
        ''' used to parse thru classifications and set different properties
        ''' </summary>
        ''' <remarks>always executed when xml is parsed; doesn't specifically needed to be called by developer code</remarks>
        Public Sub ParseThruClassifications()
            If _Classifications IsNot Nothing AndAlso _Classifications.Count > 0 Then
                _HasClassification = True 'added 9/10/2012 PM for validation purposes
                For Each cls As QuickQuoteClassification In _Classifications
                    cls.ParseThruCoverage() 'sets classification premium to that of coverage
                    If _CanUseClassificationNumForClassificationReconciliation = False Then 'added 2/20/2017 for reconciliation
                        If cls.HasValidClassificationNum = True Then
                            _CanUseClassificationNumForClassificationReconciliation = True
                        End If
                    End If
                Next
            End If
        End Sub
        'added 8/22/2012 for GL
        ''' <summary>
        ''' used to parse thru GL classifications and set different properties
        ''' </summary>
        ''' <remarks>always executed when xml is parsed; doesn't specifically needed to be called by developer code</remarks>
        Public Sub ParseThruGLClassifications()
            'added 8/28/2012
            GL_PremisesQuotedPremium = ""
            GL_ProductsQuotedPremium = ""

            If _GLClassifications IsNot Nothing AndAlso _GLClassifications.Count > 0 Then
                For Each gl As QuickQuoteGLClassification In _GLClassifications
                    gl.ParseThruCoverages()
                    'added 8/28/2012
                    GL_PremisesQuotedPremium = qqHelper.getSum(_GL_PremisesQuotedPremium, gl.PremisesQuotedPremium)
                    GL_ProductsQuotedPremium = qqHelper.getSum(_GL_ProductsQuotedPremium, gl.ProductsQuotedPremium)
                Next
            End If
        End Sub
        ''' <summary>
        ''' used to parse thru scheduled coverages and set different properties
        ''' </summary>
        ''' <remarks>always executed when xml is parsed; doesn't specifically needed to be called by developer code</remarks>
        Public Sub ParseThruScheduledCoverages() 'added 4/8/2015
            ParseThruScheduledCoverages(_ScheduledCoverages)
        End Sub
        ''' <summary>
        ''' used to parse thru scheduled coverages and set different properties
        ''' </summary>
        ''' <remarks>always executed when xml is parsed; doesn't specifically needed to be called by developer code</remarks>
        ''' 'Public Sub ParseThruScheduledCoverages() 'added 3/19/2013 for CPR
        Public Sub ParseThruScheduledCoverages(ByVal schCovs As List(Of QuickQuoteScheduledCoverage), Optional ByVal packagePartType As QuickQuoteXML.QuickQuotePackagePartType = QuickQuoteXML.QuickQuotePackagePartType.None) 'added new params 4/8/2015 for CPP
            'If _ScheduledCoverages IsNot Nothing AndAlso _ScheduledCoverages.Count > 0 Then
            'updated 4/8/2015 to use param
            If schCovs IsNot Nothing AndAlso schCovs.Count > 0 Then
                'For Each sc As QuickQuoteScheduledCoverage In _ScheduledCoverages
                'updated 4/8/2015 to use param
                For Each sc As QuickQuoteScheduledCoverage In schCovs
                    'added 1/22/2015; note: should only happen when parsing xml (FinalizeQuickQuote method)... shouldn't happen in FinalizeQuickQuoteLight method
                    If _CanUseScheduledCoverageNumForScheduledCoverageReconciliation = False Then
                        If sc.HasValidScheduledCoverageNum = True Then
                            _CanUseScheduledCoverageNumForScheduledCoverageReconciliation = True
                        End If
                    End If
                    Select Case sc.UICoverageScheduledCoverageParentTypeId
                        Case "91" 'Property in the Open; primary_coveragecode_id = 21107
                            If packagePartType = Nothing OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.None OrElse packagePartType = QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty Then 'added IF 4/13/2015
                                If _PropertyInTheOpenRecords Is Nothing Then
                                    _PropertyInTheOpenRecords = New Generic.List(Of QuickQuotePropertyInTheOpenRecord)
                                End If
                                Dim p As New QuickQuotePropertyInTheOpenRecord
                                p.ScheduledCoverageNum = sc.ScheduledCoverageNum 'added 1/22/2015
                                If sc.Coverages IsNot Nothing AndAlso sc.Coverages.Count > 0 Then
                                    For Each c As QuickQuoteCoverage In sc.Coverages
                                        Select Case c.CoverageCodeId
                                            Case "21107" 'Edit:  Property in the Open
                                                p.Limit = c.ManualLimitAmount
                                                p.IncludedInBlanketCoverage = c.IsIncludedInBlanketRating
                                                p.DeductibleId = c.DeductibleId
                                                p.CoinsuranceTypeId = c.CoinsuranceTypeId
                                                p.ValuationId = c.ValuationMethodTypeId
                                                p.ConstructionTypeId = c.ConstructionTypeId
                                                If p.EarthquakeApplies = False Then 'condition to make sure it doesn't overwrite 21520
                                                    p.EarthquakeApplies = c.IsEarthquakeApplies
                                                    p.EarthquakeDeductibleId = c.DeductibleId
                                                End If
                                                p.RatingTypeId = c.RatingTypeId
                                                p.CauseOfLossTypeId = c.CauseOfLossTypeId
                                                p.QuotedPremium = c.FullTermPremium
                                                'p.ClassificationCode = c.ClassificationCode
                                                p.FeetToFireHydrant = c.FeetToFireHydrant
                                                p.MilesToFireDepartment = c.MilesToFireDepartment
                                                p.Description = c.Description
                                                p.SpecialClassCodeTypeId = c.SpecialClassCodeTypeId
                                                p.InflationGuardTypeId = c.InflationGuardTypeId
                                                p.ProtectionClassId = c.ProtectionClassId
                                                p.IsAgreedValue = c.IsAgreedValue 'added 1/11/2018 (3/29/2018 in new branch)
                                            Case "21520" 'CheckBox:  Property in the Open - Earthquake
                                                p.EarthquakeApplies = True
                                                p.EarthquakeQuotedPremium = c.FullTermPremium
                                                p.EarthquakeDeductibleId = c.DeductibleId
                                            Case "21518" 'CheckBox:  Property in the Open - Optional Theft
                                                p.OptionalTheftDeductibleId = c.DeductibleId
                                                p.OptionalTheftQuotedPremium = c.FullTermPremium
                                            Case "21519" 'CheckBox:  Property in the Open - Windstorm or Hail
                                                p.OptionalWindstormOrHailDeductibleId = c.DeductibleId
                                                p.OptionalWindstormOrHailQuotedPremium = c.FullTermPremium
                                        End Select
                                    Next
                                End If
                                p.CalculatePremium_With_EQ_Included() 'added 4/17/2013
                                PropertyInTheOpenRecordsTotal_QuotedPremium = qqHelper.getSum(_PropertyInTheOpenRecordsTotal_QuotedPremium, p.QuotedPremium) 'added 4/17/2013
                                PropertyInTheOpenRecordsTotal_EQ_Premium = qqHelper.getSum(_PropertyInTheOpenRecordsTotal_EQ_Premium, p.EarthquakeQuotedPremium) 'added 4/17/2013
                                PropertyInTheOpenRecordsTotal_QuotedPremium_With_EQ = qqHelper.getSum(_PropertyInTheOpenRecordsTotal_QuotedPremium_With_EQ, p.QuotedPremium_With_EQ) 'added 4/17/2013
                                _PropertyInTheOpenRecords.Add(p)
                            End If
                    End Select
                Next
            End If
        End Sub
        ''' <summary>
        ''' used to parse thru modifiers and set different properties
        ''' </summary>
        ''' <remarks>always executed when xml is parsed; doesn't specifically need to be called by developer code</remarks>
        Public Sub ParseThruModifiers() 'added 7/31/2013 for HOM (specific to credits and surcharges)
            If _Modifiers IsNot Nothing AndAlso _Modifiers.Count > 0 Then
                For Each m As QuickQuoteModifier In _Modifiers
                    If m.ModifierTypeId <> "" Then '10/16/2014 note: could now use ModifierType and ParentModifierType props instead of hard-coded ids
                        Select Case m.ModifierTypeId 'all have ModifierLevelId 8 (Location)
                            Case "1" 'Multi Policy Discount; same ParentModifierTypeId; ModifierGroupId = 1 (Credits)
                                _MultiPolicyDiscount = m.CheckboxSelected
                            Case "27" 'Fire/Smoke Alarm; same ParentModifierTypeId; ModifierGroupId = 1 (Credits)
                                'at least 1 (10 or 9 or 53); 8/1/2013 - CheckboxSelected shows false
                            Case "3" 'Mature Homeowner Discount; same ParentModifierTypeId; ModifierGroupId = 1 (Credits)
                                _MatureHomeownerDiscount = m.CheckboxSelected
                                'Case "10" 'Local Alarm System; ParentModifierTypeId = 27; ModifierGroupId = 1 (Credits)
                                '    _FireSmokeAlarm_LocalAlarmSystem = True
                            Case "10" 'Local Alarm System
                                Select Case m.ParentModifierTypeId
                                    Case "27" 'Fire/Smoke Alarm
                                        _FireSmokeAlarm_LocalAlarmSystem = m.CheckboxSelected
                                    Case "26" 'Burglar Alarm
                                        _BurglarAlarm_LocalAlarmSystem = m.CheckboxSelected
                                End Select
                            Case "5" 'New Home Discount; same ParentModifierTypeId; ModifierGroupId = 1 (Credits)
                                _NewHomeDiscount = m.CheckboxSelected
                                'Case "9" 'Central Station Alarm System; ParentModifierTypeId = 27; ModifierGroupId = 1 (Credits)
                                '    _FireSmokeAlarm_CentralStationAlarmSystem = True
                            Case "9" 'Central Station Alarm System
                                Select Case m.ParentModifierTypeId
                                    Case "27" 'Fire/Smoke Alarm
                                        _FireSmokeAlarm_CentralStationAlarmSystem = m.CheckboxSelected
                                    Case "26" 'Burglar Alarm
                                        _BurglarAlarm_CentralStationAlarmSystem = m.CheckboxSelected
                                End Select
                            Case "36" 'Select Market Credit; same ParentModifierTypeId; ModifierGroupId = 1 (Credits)
                                _SelectMarketCredit = m.CheckboxSelected
                            Case "53" 'Smoke Alarm; ParentModifierTypeId = 27; ModifierGroupId = 1 (Credits)
                                _FireSmokeAlarm_SmokeAlarm = m.CheckboxSelected
                            Case "26" 'Burglar Alarm; same ParentModifierTypeId; ModifierGroupId = 1 (Credits)
                                'at least 1 (10 or 9); 8/1/2013 - CheckboxSelected shows false
                            Case "28" 'Sprinkler System; same ParentModifierTypeId; ModifierGroupId = 1 (Credits)
                                _sprinklerSystem = m.CheckboxSelected 'HOM2018 Upgrade
                                'at least 1 (39 or 40); 8/1/2013 - CheckboxSelected shows false
                                'Case "10" 'Local Alarm System; ParentModifierTypeId = 26; ModifierGroupId = 1 (Credits)
                                '    _BurglarAlarm_LocalAlarmSystem = True
                            Case "39" 'All areas except attics, bathrooms, closets, and attached structures; ParentModifierTypeId = 28; ModifierGroupId = 1 (Credits)
                                _SprinklerSystem_AllExcept = m.CheckboxSelected
                                'Case "9" 'Central Station Alarm System; ParentModifierTypeId = 26; ModifierGroupId = 1 (Credits)
                                '    _BurglarAlarm_CentralStationAlarmSystem = True
                            Case "40" 'All areas including attics, bathrooms, closets, and attached structures; ParentModifierTypeId = 28; ModifierGroupId = 1 (Credits)
                                _SprinklerSystem_AllIncluding = m.CheckboxSelected
                            Case "37" 'Trampoline Surcharge; same ParentModifierTypeId; ModifierGroupId = 2 (Surcharges/Fees)
                                _TrampolineSurcharge = m.CheckboxSelected
                            Case "17" 'Wood or Fuel Burning Appliance Surcharge; same ParentModifierTypeId; ModifierGroupId = 2 (Surcharges/Fees)
                                _WoodOrFuelBurningApplianceSurcharge = m.CheckboxSelected
                            Case "65" 'Swimming Pool/Hot Tub Surcharge; same ParentModifierTypeId; ModifierGroupId = 2 (Surcharges/Fees)
                                'added 7/28/2014
                                _SwimmingPoolHotTubSurcharge = m.CheckboxSelected
                            Case "67" 'added 8/21/2014 for e2Value... will need to update w/ correct id when available; added 8/25/2014
                                Dim pvModifierText As String = m.ModifierOptionDescription
                                Dim pvId As String = ""
                                Dim pvArchStyle As String = ""
                                Dim foundPvId As Boolean = False
                                Dim foundPvArchStyle As Boolean = False
                                'SplitPropertyValuationModifierText(pvModifierText, pvId, pvArchStyle, foundPvId, foundPvArchStyle)
                                'updated 7/28/2015 to use method in QuickQuotePropertyValuationHelperClass
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
                                    If _PropertyValuation IsNot Nothing AndAlso _PropertyValuation.Request IsNot Nothing AndAlso _PropertyValuation.Request.ArchitecturalStyle <> "" Then
                                        '8/21/2014 note: may need to convert from property valuation value to location value
                                        '_ArchitecturalStyle = _PropertyValuation.Request.ArchitecturalStyle
                                        'updated 8/25/2014
                                        Dim optionAttributes As New List(Of QuickQuoteStaticDataAttribute)
                                        If _PropertyValuation.Vendor <> QuickQuotePropertyValuation.ValuationVendor.None OrElse _PropertyValuation.VendorEstimatorType <> QuickQuotePropertyValuation.ValuationVendorEstimatorType.None Then
                                            If _PropertyValuation.Vendor <> QuickQuotePropertyValuation.ValuationVendor.None Then
                                                Dim a1 As New QuickQuoteStaticDataAttribute
                                                a1.nvp_propertyName = QuickQuoteHelperClass.QuickQuotePropertyName.Vendor
                                                a1.nvp_value = System.Enum.GetName(GetType(QuickQuotePropertyValuation.ValuationVendor), _PropertyValuation.Vendor)
                                                optionAttributes.Add(a1)
                                            End If
                                            If _PropertyValuation.VendorEstimatorType <> QuickQuotePropertyValuation.ValuationVendorEstimatorType.None Then
                                                Dim a2 As New QuickQuoteStaticDataAttribute
                                                a2.nvp_propertyName = QuickQuoteHelperClass.QuickQuotePropertyName.VendorEstimatorType
                                                a2.nvp_value = System.Enum.GetName(GetType(QuickQuotePropertyValuation.ValuationVendorEstimatorType), _PropertyValuation.VendorEstimatorType)
                                                optionAttributes.Add(a2)
                                            End If
                                        End If
                                        _ArchitecturalStyle = qqHelper.GetRelatedStaticDataValueForOptionValue_MatchingOptionAttributes(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuotePropertyValuationRequest, QuickQuoteHelperClass.QuickQuotePropertyName.ArchitecturalStyle, optionAttributes, _PropertyValuation.Request.ArchitecturalStyle, QuickQuoteHelperClass.QuickQuotePropertyName.ArchitecturalStyle)
                                    End If
                                    'added 8/26/2014 to reset if originated from a different environment
                                    If _PropertyValuation IsNot Nothing AndAlso UCase(_PropertyValuation.db_environment) <> UCase(helper.Environment) Then
                                        _PropertyValuation.Dispose()
                                        _PropertyValuation = New QuickQuotePropertyValuation
                                    End If
                                End If
                                'If pvArchStyle <> "" Then
                                'updated 8/21/2014 to always use it if it's there... even if it's blank; shouldn't be there if not different than what's on PV
                                If foundPvArchStyle = True Then
                                    '8/21/2014 note: may need to convert from property valuation value to location value
                                    _ArchitecturalStyle = pvArchStyle
                                End If

                                'added 3/4/2015 for Farm
                            Case "48" 'Hobby Farm Credit; same ParentModifierTypeId; ModifierGroupId = 1 (Credits)
                                'updated w/ logic 6/11/2015
                                _HobbyFarmCredit = m.CheckboxSelected
                                'added 6/11/2015 for Farm
                            Case "49" 'Central Alarm System; ParentModifierTypeId = 27 (Fire/Smoke Alarm); ModifierGroupId = 1 (Credits)... also mod w/ modifierTypeId 49 and ParentModifierTypeId 26 (Burglar Alarm)
                                'updated 6/12/2015 for Farm
                                Select Case m.ParentModifierTypeId
                                    Case "27" 'Fire/Smoke Alarm
                                        _FireSmokeAlarm_CentralAlarmSystem = m.CheckboxSelected
                                    Case "26" 'Burglar Alarm
                                        _BurglarAlarm_CentralAlarmSystem = m.CheckboxSelected
                                End Select
                            Case "55" 'Fire Department Alarm; ParentModifierTypeId = 27 (Fire/Smoke Alarm); ModifierGroupId = 1 (Credits)
                                'updated w/ logic 6/12/2015
                                _FireDepartmentAlarm = m.CheckboxSelected
                            Case "54" 'Police Department Theft Alarm; ParentModifierTypeId = 26 (Burglar Alarm); ModifierGroupId = 1 (Credits)
                                'updated w/ logic 6/12/2015
                                _PoliceDepartmentTheftAlarm = m.CheckboxSelected
                            Case "110" 'Wood or Fuel Burining Surcharge; post 9/1 effective date
                                _WoodOrFuelBurningApplianceSurcharge = m.CheckboxSelected
                            Case "111" 'Wood or Fuel Burining Surcharge - Number of Units; post 9/1 effective date
                                Integer.TryParse(m.ModifierOptionDescription, WoodOrFuelBurningApplianceSurcharge_NumberOfUnits)
                            Case "112" 'Trampoline Surcharge; post 9/1 effective date
                                _TrampolineSurcharge = m.CheckboxSelected
                            Case "113" 'Trampoline Surcharge - Number of Units; post 9/1 effective date
                                Integer.TryParse(m.ModifierOptionDescription, TrampolineSurcharge_NumberOfUnits)
                            Case "114" 'Swimming Pool or Hot Tub Surcharge; post 9/1 effective date
                                _SwimmingPoolHotTubSurcharge = m.CheckboxSelected
                            Case "115" 'Swimming Pool or Hot Tub Surcharge - Number of Units; post 9/1 effective date
                                Integer.TryParse(m.ModifierOptionDescription, SwimmingPoolHotTubSurcharge_NumberOfUnits)
                        End Select
                    End If
                Next
            End If
        End Sub
        'removed 7/28/2015; now in QuickQuotePropertyValuationHelperClass
        'Private Sub SplitPropertyValuationModifierText(ByVal propertyValuationModifierText As String, ByRef propertyValuationId As String, ByRef propertyValuationArchitecturalStyle As String, Optional ByRef foundPropertyValuationId As Boolean = False, Optional ByRef foundPropertyValuationArchitecturalStyle As Boolean = False)
        '    'propertyValuationId==12345||architecturalStyle==TestStyle
        '    propertyValuationId = ""
        '    propertyValuationArchitecturalStyle = ""
        '    foundPropertyValuationId = False
        '    foundPropertyValuationArchitecturalStyle = False
        '    If propertyValuationModifierText <> "" AndAlso propertyValuationModifierText.Contains("==") = True Then
        '        Dim arNameValuePair As Array
        '        If propertyValuationModifierText.Contains("||") = True Then
        '            'multiple values
        '            Dim arPvModString As String()
        '            arPvModString = Split(propertyValuationModifierText, "||")
        '            For Each nameValuePair As String In arPvModString
        '                If nameValuePair.Contains("==") = True Then
        '                    arNameValuePair = Split(nameValuePair, "==")
        '                    Select Case UCase(arNameValuePair(0).ToString.Trim)
        '                        Case "PROPERTYVALUATIONID", "PROPERTYVALID", "PROPVALID", "PVID"
        '                            propertyValuationId = arNameValuePair(1).ToString.Trim
        '                            foundPropertyValuationId = True
        '                        Case "ARCHITECTURALSTYLE", "PROPERTYVALUATIONARCHITECTURALSTYLE", "PVARCHITECTURALSTYLE", "PVARCHSTYLE"
        '                            propertyValuationArchitecturalStyle = arNameValuePair(1).ToString.Trim
        '                            foundPropertyValuationArchitecturalStyle = True
        '                    End Select
        '                End If
        '            Next
        '        ElseIf propertyValuationId = "" AndAlso (UCase(propertyValuationModifierText).Contains("PROPERTYVALUATIONID") = True OrElse UCase(propertyValuationModifierText).Contains("PROPERTYVALID") = True OrElse UCase(propertyValuationModifierText).Contains("PROPVALID") = True OrElse UCase(propertyValuationModifierText).Contains("PVID") = True) Then
        '            arNameValuePair = Split(propertyValuationModifierText, "==")
        '            propertyValuationId = arNameValuePair(1).ToString.Trim
        '            foundPropertyValuationId = True

        '            'added extra ElseIf 8/27/2014 to pick up architecturalStyle if it's by itself
        '        ElseIf propertyValuationArchitecturalStyle = "" AndAlso (UCase(propertyValuationModifierText).Contains("ARCHITECTURALSTYLE") = True OrElse UCase(propertyValuationModifierText).Contains("PROPERTYVALUATIONARCHITECTURALSTYLE") = True OrElse UCase(propertyValuationModifierText).Contains("PVARCHITECTURALSTYLE") = True OrElse UCase(propertyValuationModifierText).Contains("PVARCHSTYLE") = True) Then
        '            arNameValuePair = Split(propertyValuationModifierText, "==")
        '            propertyValuationArchitecturalStyle = arNameValuePair(1).ToString.Trim
        '            foundPropertyValuationArchitecturalStyle = True
        '        End If
        '    End If
        'End Sub
        ''' <summary>
        ''' used to parse thru section coverages and set different properties
        ''' </summary>
        ''' <remarks>always executed when xml is parsed; doesn't specifically needed to be called by developer code</remarks>
        Public Sub ParseThruSectionCoverages() 'added 8/1/2013 for HOM
            If _SectionCoverages IsNot Nothing AndAlso _SectionCoverages.Count > 0 Then
                For Each sc As QuickQuoteSectionCoverage In _SectionCoverages 'mostly appears to include only 1 Coverage for each SectionCoverage; found 3 for CoverageExposureId 118 test example
                    sc.ParseThruAdditionalInterests() 'updated to use this instead of only calling it on sectionI, sectionII, and sectionIAndII coverages
                    'added 10/15/2014; note: should only happen when parsing xml (FinalizeQuickQuote method)... shouldn't happen in FinalizeQuickQuoteLight method
                    If CanUseSectionCoverageNumForSectionCoverageReconciliation = False Then 'updated 10/29/2018 to use Public Property instead of Private Variable
                        If sc.HasValidSectionCoverageNum = True Then
                            CanUseSectionCoverageNumForSectionCoverageReconciliation = True 'updated 10/29/2018 to use Public Property instead of Private Variable
                        End If
                    End If
                    Select Case sc.CoverageExposureId
                        Case "6" 'Section I Coverages; added logic 8/14/2013; 8/15/2013 note: or Optional Property Coverages in DFR
                            If _SectionICoverages Is Nothing Then
                                _SectionICoverages = New List(Of QuickQuoteSectionICoverage)
                            End If
                            Dim sIc As New QuickQuoteSectionICoverage(Me)
                            With sIc
                                '.CoverageType = SectionICoverageType.None
                                '.CoverageCodeId = ""
                                '.Premium = ""
                                '.IncreasedLimitId = ""
                                '.IncreasedLimit = ""
                                .Description = sc.Description
                                .Address = qqHelper.CloneObject(sc.Address) 'updated 10/15/2014 to clone
                                .EffectiveDate = sc.EffectiveDate
                                .ConstructionTypeId = sc.ConstructionTypeId
                                .DescribedLocation = sc.DescribedLocation
                                .TheftExtension = sc.TheftExtension
                                .AdditionalInterests = qqHelper.CloneObject(sc.AdditionalInterests) 'updated 10/15/2014 to clone
                                .CanUseAdditionalInterestNumForAdditionalInterestReconciliation = sc.CanUseAdditionalInterestNumForAdditionalInterestReconciliation 'copying over since Parse method is now being called on each sectionCoverage instead of just for sectionI, sectionII, and sectionIAndII coverages
                                'added 8/16/2013 for DFR
                                .OccupancyCodeId = sc.OccupancyCodeId
                                .ProtectionClassId = sc.ProtectionClassId
                                .UsageTypeId = sc.UsageTypeId
                                .NumberOfDaysVacant = sc.NumberOfDaysVacant 'added 6/8/2015 for Farm
                                .NumberOfFamilies = sc.NumberOfFamilies
                                .NumberOfWells = sc.NumberOfWells 'added 6/8/2015 for Farm
                                '.SectionCoverageNum = sc.SectionCoverageNum 'added 10/15/2014 for reconciliation
                                'updated 10/29/2018
                                .SectionCoverageNumGroup = qqHelper.CloneObject(sc.SectionCoverageNumGroup)
                                .EventEffDate = sc.EventEffDate
                                .EventExpDate = sc.EventExpDate
                                .VegetatedRoof = sc.VegetatedRoof
                                .IncreasedCostOfLossId = sc.IncreasedCostofLossId
                                .RelatedExpenseLimit = sc.RelatedExpenseLimit
                                .ExteriorDoorWindowSurfacing = sc.ExteriorDoorWindowSurfacing
                                .ExteriorWallSurfacing = sc.ExteriorWallSurfacing
                                .RoofSurfacing = sc.RoofSurfacing
                                If sc.Coverages IsNot Nothing AndAlso sc.Coverages.Count > 0 Then
                                    For Each c As QuickQuoteCoverage In sc.Coverages
                                        If c.CoverageCodeId <> "" AndAlso IsNumeric(c.CoverageCodeId) = True Then
                                            .CoverageCodeId = c.CoverageCodeId
                                            .Premium = c.FullTermPremium
                                            .IncreasedLimitId = c.CoverageLimitId
                                            .IncreasedLimit = c.ManualLimitIncreased
                                            .IncludedLimit = c.ManualLimitIncluded 'added 9/10/2014
                                            .TotalLimit = c.ManualLimitAmount 'added 9/10/2014
                                        End If

                                        'Select Case c.CoverageCodeId
                                        '    'automatically added
                                        '    Case "20000" 'Combo: Inflation_Guard
                                        '        'CoverageLimitId example value 73
                                        '    Case "20098" 'Combo: Business Property Increased
                                        '        'ManualLimitAmount example value 2500.0000
                                        '        'ManualLimitIncluded example value 2500.0000
                                        '        'ManualLimitIncreased example value 0.0000
                                        '    Case "20126" 'Combo: Credit Card/Fund Trans/Forgery/Etc
                                        '        'ManualLimitAmount example value 2500.0000
                                        '        'ManualLimitIncluded example value 2500.0000
                                        '        'ManualLimitIncreased example value 0.0000
                                        '    Case "40117" 'Edit: Money
                                        '        'ManualLimitAmount example value 200.0000
                                        '        'ManualLimitIncluded example value 200.0000
                                        '        'ManualLimitIncreased example value 0.0000
                                        '    Case "40118" 'Edit: Securities
                                        '        'ManualLimitAmount example value 1000.0000
                                        '        'ManualLimitIncluded example value 1000.0000
                                        '        'ManualLimitIncreased example value 0.0000
                                        '    Case "70304" 'Edit: Personal Property at Other Residence Increase Limit
                                        '        'ManualLimitAmount example value 4230.0000
                                        '        'ManualLimitIncluded example value 4230.0000
                                        '        'ManualLimitIncreased example value 0.0000
                                        '    Case "20116" 'Edit: Jewelry, Watches & Furs
                                        '        'ManualLimitAmount example value 1000.0000
                                        '        'ManualLimitIncluded example value 1000.0000
                                        '        'ManualLimitIncreased example value 0.0000
                                        '    Case "20119" 'Edit: Silverware, Goldware, Pewterware
                                        '        'ManualLimitAmount example value 2500.0000
                                        '        'ManualLimitIncluded example value 2500.0000
                                        '        'ManualLimitIncreased example value 0.0000
                                        '    Case "20115" 'Edit: Firearms
                                        '        'ManualLimitAmount example value 2000.0000
                                        '        'ManualLimitIncluded example value 2000.0000
                                        '        'ManualLimitIncreased example value 0.0000
                                        '    Case "40116" 'Edit: Debris Removal
                                        '        'ManualLimitAmount example value 250.0000
                                        '        'ManualLimitIncluded example value 250.0000
                                        '        'ManualLimitIncreased example value 0.0000

                                        '        'manually added
                                        '    Case "70259" 'Combo: Loss Assessment
                                        '        'ManualLimitAmount example value 5000
                                        '        'ManualLimitIncluded example value 1000
                                        '        'ManualLimitIncreased example value 0
                                        '        'CoverageLimitId example value 221 (4,000)
                                        '    Case "70260" 'Edit: Loss Assessment - Earthquake
                                        '        'ManualLimitAmount example value 3200
                                        '        'ManualLimitIncluded example value 0
                                        '        'ManualLimitIncreased example value 3200.00

                                        'End Select
                                    Next
                                End If
                                '.ParseThruAdditionalInterests() 'added 4/29/2014 for reconciliation; could also call ParseThruAdditionalInterests method for sectionCoverage and then copy the canUse prop from it... done
                            End With
                            _SectionICoverages.Add(sIc)
                        Case "7" 'Section II Coverages; added logic 8/14/2013; 8/15/2013 note: or Optional Liability Coverages in DFR
                            If _SectionIICoverages Is Nothing Then
                                _SectionIICoverages = New List(Of QuickQuoteSectionIICoverage)
                            End If
                            Dim sIIc As New QuickQuoteSectionIICoverage
                            With sIIc
                                '.CoverageType = SectionIICoverageType.None
                                '.CoverageCodeId = ""
                                '.Premium = ""
                                .Description = sc.Description
                                .Name = qqHelper.CloneObject(sc.Name) 'updated 10/15/2014 to clone
                                .Address = qqHelper.CloneObject(sc.Address) 'updated 10/15/2014 to clone
                                .NumberOfDomesticEmployees = sc.NumberOfDomesticEmployees 'added 6/8/2015 for Farm
                                .NumberOfEvents = sc.NumberOfEvents 'added 6/8/2015 for Farm
                                .NumberOfPersonsReceivingCare = sc.NumberOfPersonsReceivingCare
                                .NumberOfFamilies = sc.NumberOfFamilies
                                .NumberOfFullTimeEmployees_180plus_days = sc.EmployeesFulltime
                                .NumberOfPartTimeEmployees_41_to_180_days = sc.EmployeesParttime41To179Days
                                .NumberOfPartTimeEmployees_40_or_less_days = sc.EmployeesParttimeOneTo40Days
                                .NumberOfStalls = sc.NumberOfStalls 'added 6/8/2015 for Farm
                                .EstimatedNumberOfHead = sc.NumberOfLivestock
                                .EstimatedReceipts = sc.EstimatedReceipts 'added 6/8/2015 for Farm
                                .BusinessPursuitTypeId = sc.BusinessPursuitTypeId 'added 6/12/2015 for Farm
                                .BusinessType = sc.BusinessType
                                .InitialFarmPremises = sc.InitialFarmPremises
                                .EventFrom = sc.EventEffDate
                                .EventTo = sc.EventExpDate
                                .BusinessName = sc.BusinessName
                                .AdditionalInterests = qqHelper.CloneObject(sc.AdditionalInterests) 'updated 10/15/2014 to clone
                                .CanUseAdditionalInterestNumForAdditionalInterestReconciliation = sc.CanUseAdditionalInterestNumForAdditionalInterestReconciliation 'copying over since Parse method is now being called on each sectionCoverage instead of just for sectionI, sectionII, and sectionIAndII coverages
                                'added 8/16/2013 for DFR
                                .NavigationPeriodEffDate = sc.NavigationPeriodEffDate
                                .NavigationPeriodExpDate = sc.NavigationPeriodExpDate
                                .NumberOfEmployees = sc.NumberOfEmployees
                                '.SectionCoverageNum = sc.SectionCoverageNum 'added 10/15/2014 for reconciliation
                                'updated 10/29/2018
                                .SectionCoverageNumGroup = qqHelper.CloneObject(sc.SectionCoverageNumGroup)
                                '.EventEffDate = sc.EventEffDate
                                '.EventExpDate = sc.EventExpDate
                                '.VegetatedRoof = sc.VegetatedRoof
                                '.IncreasedCostofLossId = sc.IncreasedCostofLossId
                                .AddedDate = sc.AddedDate
                                .PCAdded_Date = sc.PCAdded_Date
                                .LastModifiedDate = sc.LastModifiedDate
                                .AddedImageNum = sc.AddedImageNum

                                If sc.Coverages IsNot Nothing AndAlso sc.Coverages.Count > 0 Then
                                    For Each c As QuickQuoteCoverage In sc.Coverages
                                        If c.CoverageCodeId <> "" AndAlso IsNumeric(c.CoverageCodeId) = True Then
                                            .CoverageCodeId = c.CoverageCodeId
                                            .Premium = c.FullTermPremium
                                            .IncreasedLimit = c.ManualLimitIncreased 'added 8/16/2013 for DFR
                                            .IncludedLimit = c.ManualLimitIncluded 'added 9/10/2014
                                            .TotalLimit = c.ManualLimitAmount 'added 9/10/2014
                                            .IncreasedLimitId = c.CoverageLimitId 'added 10/8/2015 for Farm
                                        End If

                                        'Select Case c.CoverageCodeId
                                        '    'manually added
                                        '    Case "20169" 'CheckBox: Home Day Care Liability
                                        '        'Checkbox example value true
                                        '    Case "20049" 'CheckBox: Business Pursuits - Clerical
                                        '        'Checkbox example value true

                                        'End Select
                                    Next
                                End If
                                '.ParseThruAdditionalInterests() 'added 4/29/2014 for reconciliation; could also call ParseThruAdditionalInterests method for sectionCoverage and then copy the canUse prop from it... done
                            End With
                            _SectionIICoverages.Add(sIIc)
                        Case "118" 'Section I & II Coverages; added logic 8/14/2013
                            If _SectionIAndIICoverages Is Nothing Then
                                _SectionIAndIICoverages = New List(Of QuickQuoteSectionIAndIICoverage)
                            End If
                            Dim sIAndIIc As New QuickQuoteSectionIAndIICoverage
                            With sIAndIIc
                                '.MainCoverageType = SectionIAndIICoverageType.None
                                '.MainCoverageCodeId = ""
                                '.PropertyCoverageType = SectionIAndIIPropertyCoverageType.None
                                '.PropertyCoverageCodeId = ""
                                '.LiabilityCoverageType = SectionIAndIILiabilityCoverageType.None
                                '.LiabilityCoverageCodeId = ""
                                '.Premium = ""
                                '.PropertyIncreasedLimit = ""
                                .Description = sc.Description
                                .Name = qqHelper.CloneObject(sc.Name) 'updated 10/15/2014 to clone
                                .Address = qqHelper.CloneObject(sc.Address) 'updated 10/15/2014 to clone
                                .NumberOfFamilies = sc.NumberOfFamilies
                                .AdditionalInterests = qqHelper.CloneObject(sc.AdditionalInterests) 'updated 10/15/2014 to clone
                                .CanUseAdditionalInterestNumForAdditionalInterestReconciliation = sc.CanUseAdditionalInterestNumForAdditionalInterestReconciliation 'copying over since Parse method is now being called on each sectionCoverage instead of just for sectionI, sectionII, and sectionIAndII coverages
                                '.SectionCoverageNum = sc.SectionCoverageNum 'added 10/15/2014 for reconciliation
                                'updated 10/29/2018
                                .SectionCoverageNumGroup = qqHelper.CloneObject(sc.SectionCoverageNumGroup)
                                .EventEffDate = sc.EventEffDate
                                .EventExpDate = sc.EventExpDate
                                '.VegetatedRoof = sc.VegetatedRoof
                                '.IncreasedCostofLossId = sc.IncreasedCostofLossId
                                If sc.Coverages IsNot Nothing AndAlso sc.Coverages.Count > 0 Then
                                    For Each c As QuickQuoteCoverage In sc.Coverages
                                        If c.CoverageCodeId <> "" AndAlso IsNumeric(c.CoverageCodeId) = True Then
                                            Dim isKnownCov As Boolean = False 'added 1/31/2020
                                            'If System.Enum.IsDefined(GetType(QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType), CInt(c.CoverageCodeId)) = True Then
                                            '    .MainCoverageType = CInt(c.CoverageCodeId)
                                            '    .Premium = qqHelper.getSum(.Premium, c.FullTermPremium)
                                            'End If
                                            'updated 12/4/2013; IsDefined will look for match on Enum text or value
                                            If System.Enum.IsDefined(GetType(QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIAndIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.MainCoverageCodeId, c.CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.MainCoverageType)) = True Then
                                                .MainCoverageType = System.Enum.Parse(GetType(QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIAndIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.MainCoverageCodeId, c.CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.MainCoverageType))
                                                .Premium = qqHelper.getSum(.Premium, c.FullTermPremium)
                                                isKnownCov = True 'added 1/31/2020
                                            End If
                                            'If System.Enum.IsDefined(GetType(QuickQuoteSectionIAndIICoverage.SectionIAndIIPropertyCoverageType), CInt(c.CoverageCodeId)) = True Then
                                            '    .PropertyCoverageType = CInt(c.CoverageCodeId)
                                            '    .Premium = qqHelper.getSum(.Premium, c.FullTermPremium)
                                            '    .PropertyIncreasedLimit = c.ManualLimitIncreased
                                            'End If
                                            'updated 12/4/2013; IsDefined will look for match on Enum text or value
                                            If System.Enum.IsDefined(GetType(QuickQuoteSectionIAndIICoverage.SectionIAndIIPropertyCoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIAndIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.PropertyCoverageCodeId, c.CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.PropertyCoverageType)) = True Then
                                                .PropertyCoverageType = System.Enum.Parse(GetType(QuickQuoteSectionIAndIICoverage.SectionIAndIIPropertyCoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIAndIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.PropertyCoverageCodeId, c.CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.PropertyCoverageType))
                                                .Premium = qqHelper.getSum(.Premium, c.FullTermPremium)
                                                .PropertyIncreasedLimit = c.ManualLimitIncreased
                                                .PropertyIncludedLimit = c.ManualLimitIncluded 'added 9/10/2014
                                                .PropertyTotalLimit = c.ManualLimitAmount 'added 9/10/2014
                                                isKnownCov = True 'added 1/31/2020
                                            End If
                                            'If System.Enum.IsDefined(GetType(QuickQuoteSectionIAndIICoverage.SectionIAndIILiabilityCoverageType), CInt(c.CoverageCodeId)) = True Then
                                            '    .LiabilityCoverageType = CInt(c.CoverageCodeId)
                                            '    .Premium = qqHelper.getSum(.Premium, c.FullTermPremium)
                                            'End If
                                            'updated 12/4/2013; IsDefined will look for match on Enum text or value
                                            If System.Enum.IsDefined(GetType(QuickQuoteSectionIAndIICoverage.SectionIAndIILiabilityCoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIAndIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.LiabilityCoverageCodeId, c.CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.LiabilityCoverageType)) = True Then
                                                .LiabilityCoverageType = System.Enum.Parse(GetType(QuickQuoteSectionIAndIICoverage.SectionIAndIILiabilityCoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIAndIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.LiabilityCoverageCodeId, c.CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.LiabilityCoverageType))
                                                .Premium = qqHelper.getSum(.Premium, c.FullTermPremium)
                                                .LiabilityIncreasedLimit = c.ManualLimitIncreased
                                                .LiabilityIncludedLimit = c.ManualLimitIncluded
                                                .LiabilityTotalLimit = c.ManualLimitAmount
                                                isKnownCov = True 'added 1/31/2020
                                            End If

                                            'added 1/31/2020
                                            If isKnownCov = False Then
                                                If .UnknownCoverages Is Nothing Then
                                                    .UnknownCoverages = New List(Of QuickQuoteCoverage)
                                                End If
                                                .UnknownCoverages.Add(qqHelper.CloneObject(c))
                                            End If
                                        End If

                                        'Select Case c.CoverageCodeId
                                        '    'manually added
                                        '    Case "20067" 'CheckBox: Permitted Incidental Occupancies Residence Premises - Other Structures
                                        '        'Checkbox example value true
                                        '    Case "80095" 'Edit: Permitted Incidental Occupancies Residence Premises - Other Structures - Property
                                        '        'ManualLimitAmount example value 1000
                                        '        'ManualLimitIncluded example value 0
                                        '        'ManualLimitIncreased example value 1000.00
                                        '    Case "80096" 'CheckBox: Permitted Incidental Occupancies Residence Premises - Other Structures - Liability
                                        '        'Checkbox example value true

                                        'End Select
                                    Next
                                End If
                                '.ParseThruAdditionalInterests() 'added 4/29/2014 for reconciliation; could also call ParseThruAdditionalInterests method for sectionCoverage and then copy the canUse prop from it... done
                            End With
                            _SectionIAndIICoverages.Add(sIAndIIc)
                    End Select
                Next
            End If
        End Sub
        'added 8/6/2013
        Public Sub ParseThruInlandMarines()
            If _InlandMarines IsNot Nothing AndAlso _InlandMarines.Count > 0 Then
                For Each im As QuickQuoteInlandMarine In _InlandMarines
                    im.CheckCoverage()
                    im.ParseThruAdditionalInterests() 'added 4/29/2014 for reconciliation

                    'added 10/15/2014; note: should only happen when parsing xml (FinalizeQuickQuote method)... shouldn't happen in FinalizeQuickQuoteLight method
                    If _CanUseInlandMarineNumForInlandMarineReconciliation = False Then
                        If im.HasValidInlandMarineNum = True Then
                            _CanUseInlandMarineNumForInlandMarineReconciliation = True
                        End If
                    End If

                    'added 11/17/2014
                    InlandMarinesTotal_Premium = qqHelper.getSum(_InlandMarinesTotal_Premium, im.Premium)
                    InlandMarinesTotal_CoveragePremium = qqHelper.getSum(_InlandMarinesTotal_CoveragePremium, im.CoveragePremium)
                Next
            End If
        End Sub
        'Public Sub ParseThruRvWatercrafts()
        'updated 10/30/2014
        Public Sub ParseThruRvWatercrafts(Optional ByVal policyLevelOperators As List(Of QuickQuoteOperator) = Nothing)
            If _RvWatercrafts IsNot Nothing AndAlso _RvWatercrafts.Count > 0 Then
                For Each rv As QuickQuoteRvWatercraft In _RvWatercrafts
                    rv.HasConvertedCoverages = False 'added 2/20/2014 so this is also reset on the 1st rate; was previously only being done in FinalizeQuickQuoteLight
                    rv.ParseThruCoverages()
                    rv.ParseThruAdditionalInterests() 'added 4/29/2014 for reconciliation
                    rv.HasConvertedAssignedOperators = False 'added 10/29/2014
                    rv.ParseThruOperators(policyLevelOperators) 'added 10/29/2014 for reconciliation; updated 10/30/2014 for policyLevelOperators

                    'added 10/15/2014; note: should only happen when parsing xml (FinalizeQuickQuote method)... shouldn't happen in FinalizeQuickQuoteLight method
                    If _CanUseRvWatercraftNumForRvWatercraftReconciliation = False Then
                        If rv.HasValidRvWatercraftNum = True Then
                            _CanUseRvWatercraftNumForRvWatercraftReconciliation = True
                        End If
                    End If
                    rv.ParseThruRvWatercraftMotors()

                    'added 11/17/2014
                    RvWatercraftsTotal_Premium = qqHelper.getSum(_RvWatercraftsTotal_Premium, rv.Premium)
                    RvWatercraftsTotal_CoveragesPremium = qqHelper.getSum(_RvWatercraftsTotal_CoveragesPremium, rv.CoveragesPremium)
                Next
            End If
        End Sub
        Public Function HasValidLocationNum() As Boolean 'added 4/23/2014 for reconciliation purposes
            'If _LocationNum <> "" AndAlso IsNumeric(_LocationNum) = True AndAlso CInt(_LocationNum) > 0 Then
            '    Return True
            'Else
            '    Return False
            'End If
            'updated 4/27/2014 to use common method
            'Return qqHelper.IsValidQuickQuoteIdOrNum(_LocationNum)
            'updated 10/17/2018 to use new method
            Return HasValidLocationNum(QuickQuoteXML.QuickQuotePackagePartType.None)
        End Function
        'added 10/17/2018
        Public Function LocationNumForPackagePartType(ByVal packagePartType As QuickQuoteXML.QuickQuotePackagePartType) As String
            Select Case packagePartType
                Case QuickQuoteXML.QuickQuotePackagePartType.Package
                    Return _LocationNum_MasterPart
                Case QuickQuoteXML.QuickQuotePackagePartType.GeneralLiability
                    Return _LocationNum_CGLPart
                Case QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty
                    Return _LocationNum_CPRPart
                Case QuickQuoteXML.QuickQuotePackagePartType.InlandMarine
                    Return _LocationNum_CIMPart
                Case QuickQuoteXML.QuickQuotePackagePartType.Crime
                    Return _LocationNum_CRMPart
                Case QuickQuoteXML.QuickQuotePackagePartType.Garage
                    Return _LocationNum_GARPart
                Case Else
                    Return _LocationNum
            End Select
        End Function
        Public Function HasValidLocationNum(ByVal packagePartType As QuickQuoteXML.QuickQuotePackagePartType) As Boolean
            Return qqHelper.IsValidQuickQuoteIdOrNum(LocationNumForPackagePartType(packagePartType))
        End Function
        Public Sub SetLocationNumForPackagePartType(ByVal locNum As String, ByVal packagePartType As QuickQuoteXML.QuickQuotePackagePartType)
            Select Case packagePartType
                Case QuickQuoteXML.QuickQuotePackagePartType.Package
                    _LocationNum_MasterPart = locNum
                Case QuickQuoteXML.QuickQuotePackagePartType.GeneralLiability
                    _LocationNum_CGLPart = locNum
                Case QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty
                    _LocationNum_CPRPart = locNum
                Case QuickQuoteXML.QuickQuotePackagePartType.InlandMarine
                    _LocationNum_CIMPart = locNum
                Case QuickQuoteXML.QuickQuotePackagePartType.Crime
                    _LocationNum_CRMPart = locNum
                Case QuickQuoteXML.QuickQuotePackagePartType.Garage
                    _LocationNum_GARPart = locNum
                Case Else
                    _LocationNum = locNum
            End Select
        End Sub
        'added 10/18/2018
        Public Function CanUseFarmBarnBuildingNumFlagForPackagePartType(ByVal packagePartType As QuickQuoteXML.QuickQuotePackagePartType) As Boolean
            Select Case packagePartType
                Case QuickQuoteXML.QuickQuotePackagePartType.Package
                    Return CanUseFarmBarnBuildingNumForMasterPartBuildingReconciliation
                Case QuickQuoteXML.QuickQuotePackagePartType.GeneralLiability
                    Return CanUseFarmBarnBuildingNumForCGLPartBuildingReconciliation
                Case QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty
                    Return CanUseFarmBarnBuildingNumForCPRPartBuildingReconciliation
                Case QuickQuoteXML.QuickQuotePackagePartType.InlandMarine
                    Return CanUseFarmBarnBuildingNumForCIMPartBuildingReconciliation
                Case QuickQuoteXML.QuickQuotePackagePartType.Crime
                    Return CanUseFarmBarnBuildingNumForCRMPartBuildingReconciliation
                Case QuickQuoteXML.QuickQuotePackagePartType.Garage
                    Return CanUseFarmBarnBuildingNumForGARPartBuildingReconciliation
                Case Else
                    Return CanUseFarmBarnBuildingNumForBuildingReconciliation
            End Select
        End Function
        Public Sub SetCanUseFarmBarnBuildingNumFlagForPackagePartType(ByVal canUse As Boolean, ByVal packagePartType As QuickQuoteXML.QuickQuotePackagePartType)
            Select Case packagePartType
                Case QuickQuoteXML.QuickQuotePackagePartType.Package
                    CanUseFarmBarnBuildingNumForMasterPartBuildingReconciliation = canUse
                Case QuickQuoteXML.QuickQuotePackagePartType.GeneralLiability
                    CanUseFarmBarnBuildingNumForCGLPartBuildingReconciliation = canUse
                Case QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty
                    CanUseFarmBarnBuildingNumForCPRPartBuildingReconciliation = canUse
                Case QuickQuoteXML.QuickQuotePackagePartType.InlandMarine
                    CanUseFarmBarnBuildingNumForCIMPartBuildingReconciliation = canUse
                Case QuickQuoteXML.QuickQuotePackagePartType.Crime
                    CanUseFarmBarnBuildingNumForCRMPartBuildingReconciliation = canUse
                Case QuickQuoteXML.QuickQuotePackagePartType.Garage
                    CanUseFarmBarnBuildingNumForGARPartBuildingReconciliation = canUse
                Case Else
                    CanUseFarmBarnBuildingNumForBuildingReconciliation = canUse
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
        'added 10/14/2014 for reconciliation
        Public Sub ParseThruExclusions()
            If _Exclusions IsNot Nothing AndAlso _Exclusions.Count > 0 Then
                For Each e As QuickQuoteExclusion In _Exclusions
                    'note: should only happen when parsing xml (FinalizeQuickQuote method)... shouldn't happen in FinalizeQuickQuoteLight method
                    If _CanUseExclusionNumForExclusionReconciliation = False Then
                        If e.HasValidExclusionNum = True Then
                            _CanUseExclusionNumForExclusionReconciliation = True
                            Exit For
                        End If
                    End If
                Next
            End If
        End Sub
        'added 10/15/2014 for reconciliation
        Public Sub ParseThruPolicyUnderwritings()
            If _PolicyUnderwritings IsNot Nothing AndAlso _PolicyUnderwritings.Count > 0 Then
                For Each pu As QuickQuotePolicyUnderwriting In _PolicyUnderwritings
                    'note: should only happen when parsing xml (FinalizeQuickQuote method)... shouldn't happen in FinalizeQuickQuoteLight method
                    If _CanUsePolicyUnderwritingNumForPolicyUnderwritingReconciliation = False Then
                        If pu.HasValidPolicyUnderwritingNum = True Then
                            _CanUsePolicyUnderwritingNumForPolicyUnderwritingReconciliation = True
                            Exit For
                        End If
                    End If
                Next
            End If
        End Sub
        'added 2/25/2015
        Public Sub ParseThruIncidentalDwellingCoverages()
            If _IncidentalDwellingCoverages IsNot Nothing AndAlso _IncidentalDwellingCoverages.Count > 0 Then
                For Each c As QuickQuoteCoverage In _IncidentalDwellingCoverages
                    Select Case c.CoverageCodeId
                        Case "80139" 'Edit: Location_Farm_Fire_Department_Service_Charge; example has ManualLimitAmount, ManualLimitIncluded, and ManualLimitIncreased

                        Case "70143" 'Edit: Farm_Outdoor_Antenna_Satellite_Dish; example has ManualLimitAmount and ManualLimitIncluded

                        Case "70144" 'Edit: Farm_Well_Pumps; example has ManualLimitAmount and ManualLimitIncluded

                        Case "70145" 'Edit: Farm_Private_Power_Light_Poles; example has ManualLimitAmount and ManualLimitIncluded

                        Case "70147" 'Combo: Farm_Credit_Card_Forgery_and_Counterfeit_Money; example has ManualLimitAmount and ManualLimitIncluded

                        Case "70148" 'Edit: Farm_Refrigerated_Food_Spoilage; example has ManualLimitAmount and ManualLimitIncluded

                    End Select
                Next
            End If
        End Sub
        'added 2/26/2015
        Public Sub ParseThruAcreages()
            If _Acreages IsNot Nothing AndAlso _Acreages.Count > 0 Then
                For Each a As QuickQuoteAcreage In _Acreages
                    'note: should only happen when parsing xml (FinalizeQuickQuote method)... shouldn't happen in FinalizeQuickQuoteLight method
                    If _CanUseAcreageNumForAcreageReconciliation = False Then
                        If a.HasValidAcreageNum = True Then
                            _CanUseAcreageNumForAcreageReconciliation = True
                            Exit For
                        End If
                    End If
                Next
            End If
        End Sub
        Public Sub ParseThruIncomeLosses()
            If _IncomeLosses IsNot Nothing AndAlso _IncomeLosses.Count > 0 Then
                For Each il As QuickQuoteIncomeLoss In _IncomeLosses
                    il.CheckCoverage() 'added call 3/4/2015; must've been inadvertently left out
                    'note: should only happen when parsing xml (FinalizeQuickQuote method)... shouldn't happen in FinalizeQuickQuoteLight method
                    If _CanUseLossOfIncomeNumForIncomeLossReconciliation = False Then
                        If il.HasValidLossOfIncomeNum = True Then
                            _CanUseLossOfIncomeNumForIncomeLossReconciliation = True
                            'Exit For 'commented 12/21/2015 so CheckCoverage can be called for every IncomeLoss and not just the 1st one
                        End If
                    End If
                Next
            End If
        End Sub
        Public Sub ParseThruResidentNames()
            If _ResidentNames IsNot Nothing AndAlso _ResidentNames.Count > 0 Then
                For Each rn As QuickQuoteResidentName In _ResidentNames
                    'note: should only happen when parsing xml (FinalizeQuickQuote method)... shouldn't happen in FinalizeQuickQuoteLight method
                    If _CanUseResidentNumForResidentNameReconciliation = False Then
                        If rn.HasValidResidentNum = True Then
                            _CanUseResidentNumForResidentNameReconciliation = True
                            Exit For
                        End If
                    End If
                Next
            End If
        End Sub
        Public Overrides Function ToString() As String 'added 6/29/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.Address IsNot Nothing Then
                    str = qqHelper.appendText(str, "DisplayAddress: " & Me.Address.DisplayAddress, vbCrLf)
                End If
                If Me.Name IsNot Nothing Then
                    str = qqHelper.appendText(str, "DisplayName: " & Me.Name.DisplayName, vbCrLf)
                End If
                If Me.Description <> "" Then
                    str = qqHelper.appendText(str, "Description: " & Me.Description, vbCrLf)
                End If
                If Me.YearBuilt <> "" Then
                    str = qqHelper.appendText(str, "YearBuilt: " & Me.YearBuilt, vbCrLf)
                End If
                If Me.ProtectionClassId <> "" Then
                    Dim p As String = ""
                    p = "ProtectionClassId: " & Me.ProtectionClassId
                    Dim pClass As String = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.ProtectionClassId, Me.ProtectionClassId)
                    If pClass <> "" Then
                        p &= " (" & pClass & ")"
                    End If
                    str = qqHelper.appendText(str, p, vbCrLf)
                End If
                If Me.ProgramTypeId <> "" Then
                    Dim p As String = ""
                    p = "ProgramTypeId: " & Me.ProgramTypeId
                    Dim pType As String = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.ProgramTypeId, Me.ProgramTypeId)
                    If pType <> "" Then
                        p &= " (" & pType & ")"
                    End If
                    str = qqHelper.appendText(str, p, vbCrLf)
                End If
                If Me.FormTypeId <> "" Then
                    Dim f As String = ""
                    f = "FormTypeId: " & Me.FormTypeId
                    Dim fType As String = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, Me.FormTypeId)
                    If fType <> "" Then
                        f &= " (" & fType & ")"
                    End If
                    str = qqHelper.appendText(str, f, vbCrLf)
                End If
                If Me.Buildings IsNot Nothing AndAlso Me.Buildings.Count > 0 Then
                    str = qqHelper.appendText(str, Me.Buildings.Count.ToString & " Buildings", vbCrLf)
                End If
                If Me.LocationCoverages IsNot Nothing AndAlso Me.LocationCoverages.Count > 0 Then
                    str = qqHelper.appendText(str, Me.LocationCoverages.Count.ToString & " Coverages", vbCrLf)
                End If
                If Me.InlandMarines IsNot Nothing AndAlso Me.InlandMarines.Count > 0 Then
                    str = qqHelper.appendText(str, Me.InlandMarines.Count.ToString & " InlandMarines", vbCrLf)
                End If
                If Me.RvWatercrafts IsNot Nothing AndAlso Me.RvWatercrafts.Count > 0 Then
                    str = qqHelper.appendText(str, Me.RvWatercrafts.Count.ToString & " RvWatercrafts", vbCrLf)
                End If
                If Me.Modifiers IsNot Nothing AndAlso Me.Modifiers.Count > 0 Then 'added 6/30/2015
                    str = qqHelper.appendText(str, Me.Modifiers.Count.ToString & " Modifiers", vbCrLf)
                End If
            Else
                str = "Nothing"
            End If
            Return str
        End Function

        'added 9/20/2016 for Verisk Protection Class
        Protected Friend Sub Set_ProtectionClassSystemGeneratedId(ByVal protClassSysGenId As String)
            _ProtectionClassSystemGeneratedId = protClassSysGenId
        End Sub
        'added 9/24/2016 for Verisk Protection Class
        Protected Friend Sub Set_PPCMatchTypeId(ByVal matchTypeId As String)
            _PPCMatchTypeId = matchTypeId
        End Sub

        'added 10/7/2016 to compare before/after address
        Protected Friend Sub Set_OriginalAddress(ByVal origAdd As QuickQuoteAddress)
            _OriginalAddress = origAdd
        End Sub

        'added 10/14/2016 for Verisk Protection Class
        Protected Friend Sub Set_FireStationDistance(ByVal fireStationDist As String)
            _FireStationDistance = fireStationDist
        End Sub

        'added 7/15/2017 after making Properties ReadOnly (since they should be set at Policy level)
        Protected Friend Sub Set_GarageKeepersOtherThanCollisionDeductibleCategoryTypeId(ByVal otcDedCatTypeId As String)
            _GarageKeepersOtherThanCollisionDeductibleCategoryTypeId = otcDedCatTypeId
        End Sub
        Protected Friend Sub Set_GarageKeepersOtherThanCollisionTypeId(ByVal otcTypeId As String)
            _GarageKeepersOtherThanCollisionTypeId = otcTypeId
        End Sub
        Protected Friend Sub Set_GarageKeepersOtherThanCollisionDeductibleId(ByVal otcDedId As String)
            _GarageKeepersOtherThanCollisionDeductibleId = otcDedId
        End Sub
        Protected Friend Sub Set_GarageKeepersOtherThanCollisionBasisTypeId(ByVal otcBasisTypeId As String)
            _GarageKeepersOtherThanCollisionBasisTypeId = otcBasisTypeId
        End Sub
        Protected Friend Sub Set_GarageKeepersCollisionDeductibleId(ByVal collDedId As String)
            _GarageKeepersCollisionDeductibleId = collDedId
        End Sub
        Protected Friend Sub Set_GarageKeepersCollisionBasisTypeId(ByVal collBasisTypeId As String)
            _GarageKeepersCollisionBasisTypeId = collBasisTypeId
        End Sub
        'added 11/1/2017; covCodeId 21042
        Protected Friend Sub Set_EquipmentBreakdownDeductibleIdBackup(ByVal ebDeductId As String)
            _EquipmentBreakdownDeductibleIdBackup = ebDeductId
        End Sub

        'added 8/2/2018
        Protected Friend Sub Set_QuoteStateTakenFrom(ByVal qqState As helper.QuickQuoteState)
            _QuoteStateTakenFrom = qqState
        End Sub

        'added 1/16/2019
        Protected Friend Sub Set_DisplayNum(ByVal dNum As Integer)
            _DisplayNum = dNum
            If _OriginalDisplayNum <= 0 Then
                _OriginalDisplayNum = _DisplayNum
            End If
        End Sub
        Protected Friend Sub Set_OkayToUseDisplayNum(ByVal setType As QuickQuoteHelperClass.WhenToSetType)
            _OkayToUseDisplayNum = setType
        End Sub
        Public Function OkayToUseDisplayNum(Optional ByVal packagePartType As QuickQuoteXML.QuickQuotePackagePartType = QuickQuoteXML.QuickQuotePackagePartType.None) As Boolean
            Return QuickQuoteHelperClass.OkayToUseInteger(_OkayToUseDisplayNum, _DisplayNum, packagePartType:=packagePartType)
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
                    If _Description IsNot Nothing Then
                        _Description = Nothing
                    End If
                    'If _StreetNum IsNot Nothing Then
                    '    _StreetNum = Nothing
                    'End If
                    'If _StreetName IsNot Nothing Then
                    '    _StreetName = Nothing
                    'End If
                    'If _City IsNot Nothing Then
                    '    _City = Nothing
                    'End If
                    'If _State IsNot Nothing Then
                    '    _State = Nothing
                    'End If
                    'If _Zip IsNot Nothing Then
                    '    _Zip = Nothing
                    'End If
                    'If _County IsNot Nothing Then
                    '    _County = Nothing
                    'End If
                    If _Address IsNot Nothing Then
                        _Address.Dispose()
                        _Address = Nothing
                    End If
                    If _ProtectionClass IsNot Nothing Then
                        _ProtectionClass = Nothing
                    End If
                    If _ProtectionClassId IsNot Nothing Then
                        _ProtectionClassId = Nothing
                    End If
                    If _NumberOfPools IsNot Nothing Then
                        _NumberOfPools = Nothing
                    End If

                    If _PropertyDeductibleId IsNot Nothing Then
                        _PropertyDeductibleId = Nothing
                    End If

                    '3/9/2017 - BOP stuff
                    If _NumberOfAmusementAreas IsNot Nothing Then
                        _NumberOfAmusementAreas = Nothing
                    End If
                    If _NumberOfPlaygrounds IsNot Nothing Then
                        _NumberOfPlaygrounds = Nothing
                    End If

                    If _EquipmentBreakdownDeductible IsNot Nothing Then
                        _EquipmentBreakdownDeductible = Nothing
                    End If
                    If _EquipmentBreakdownDeductibleId IsNot Nothing Then
                        _EquipmentBreakdownDeductibleId = Nothing
                    End If
                    If _EquipmentBreakdownDeductibleQuotedPremium IsNot Nothing Then
                        _EquipmentBreakdownDeductibleQuotedPremium = Nothing
                    End If
                    If _EquipmentBreakdownDeductibleMinimumRequiredByClassCode IsNot Nothing Then '3/9/2017 - BOP stuff
                        _EquipmentBreakdownDeductibleMinimumRequiredByClassCode = Nothing
                    End If
                    If _MoneySecuritiesOnPremises IsNot Nothing Then
                        _MoneySecuritiesOnPremises = Nothing
                    End If
                    If _MoneySecuritiesOffPremises IsNot Nothing Then
                        _MoneySecuritiesOffPremises = Nothing
                    End If
                    If _MoneySecuritiesQuotedPremium IsNot Nothing Then
                        _MoneySecuritiesQuotedPremium = Nothing
                    End If
                     If _MoneySecuritiesQuotedPremium_OnPremises IsNot Nothing Then
                        _MoneySecuritiesQuotedPremium_OnPremises = Nothing
                    End If
                     If _MoneySecuritiesQuotedPremium_OffPremises IsNot Nothing Then
                        _MoneySecuritiesQuotedPremium_OffPremises = Nothing
                    End If
                    If _OutdoorSignsLimit IsNot Nothing Then
                        _OutdoorSignsLimit = Nothing
                    End If
                    If _OutdoorSignsQuotedPremium IsNot Nothing Then
                        _OutdoorSignsQuotedPremium = Nothing
                    End If

                    If _Buildings IsNot Nothing Then
                        If _Buildings.Count > 0 Then
                            For Each build As QuickQuoteBuilding In _Buildings
                                build.Dispose()
                                build = Nothing
                            Next
                            _Buildings.Clear()
                        End If
                        _Buildings = Nothing
                    End If
                    If _LocationCoverages IsNot Nothing Then
                        If _LocationCoverages.Count > 0 Then
                            For Each cov As QuickQuoteCoverage In _LocationCoverages
                                cov.Dispose()
                                cov = Nothing
                            Next
                            _LocationCoverages.Clear()
                        End If
                        _LocationCoverages = Nothing
                    End If

                    If _GLClassifications IsNot Nothing Then
                        If _GLClassifications.Count > 0 Then
                            For Each gl As QuickQuoteGLClassification In _GLClassifications
                                gl.Dispose()
                                gl = Nothing
                            Next
                            _GLClassifications.Clear()
                        End If
                        _GLClassifications = Nothing
                    End If
                    If _Classifications IsNot Nothing Then
                        If _Classifications.Count > 0 Then
                            For Each cls As QuickQuoteClassification In _Classifications
                                cls.Dispose()
                                cls = Nothing
                            Next
                            _Classifications.Clear()
                        End If
                        _Classifications = Nothing
                    End If

                    If _Name IsNot Nothing Then
                        _Name.Dispose()
                        _Name = Nothing
                    End If

                    If _GL_PremisesQuotedPremium IsNot Nothing Then
                        _GL_PremisesQuotedPremium = Nothing
                    End If
                    If _GL_ProductsQuotedPremium IsNot Nothing Then
                        _GL_ProductsQuotedPremium = Nothing
                    End If

                    If _HasBuilding <> Nothing Then
                        _HasBuilding = Nothing
                    End If
                    If _HasClassification <> Nothing Then
                        _HasClassification = Nothing
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
                    If _ValuationMethodTypeId IsNot Nothing Then
                        _ValuationMethodTypeId = Nothing
                    End If
                    If _ValuationMethodType IsNot Nothing Then
                        _ValuationMethodType = Nothing
                    End If

                    If _EquipmentBreakdownOccupancyTypeId IsNot Nothing Then
                        _EquipmentBreakdownOccupancyTypeId = Nothing
                    End If
                    If _ClassificationCode IsNot Nothing Then
                        _ClassificationCode.Dispose()
                        _ClassificationCode = Nothing
                    End If

                    If _FeetToFireHydrant IsNot Nothing Then
                        _FeetToFireHydrant = Nothing
                    End If
                    If _MilesToFireDepartment IsNot Nothing Then
                        _MilesToFireDepartment = Nothing
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
                    If _PropertyInTheOpenRecords IsNot Nothing Then
                        If _PropertyInTheOpenRecords.Count > 0 Then
                            For Each p As QuickQuotePropertyInTheOpenRecord In _PropertyInTheOpenRecords
                                p.Dispose()
                                p = Nothing
                            Next
                            _PropertyInTheOpenRecords.Clear()
                        End If
                        _PropertyInTheOpenRecords = Nothing
                    End If

                    If _WindstormOrHailPercentageDeductibleId IsNot Nothing Then
                        _WindstormOrHailPercentageDeductibleId = Nothing
                    End If
                    If _WindstormOrHailPercentageDeductible IsNot Nothing Then
                        _WindstormOrHailPercentageDeductible = Nothing
                    End If
                    If _WindstormOrHailMinimumDollarDeductibleId IsNot Nothing Then
                        _WindstormOrHailMinimumDollarDeductibleId = Nothing
                    End If
                    If _WindstormOrHailMinimumDollarDeductible IsNot Nothing Then
                        _WindstormOrHailMinimumDollarDeductible = Nothing
                    End If
                    If _EarthquakeApplies <> Nothing Then
                        _EarthquakeApplies = Nothing
                    End If

                    'added 4/17/2013 for CPR to total up Property in the Open coverage premiums
                    If _PropertyInTheOpenRecordsTotal_QuotedPremium IsNot Nothing Then
                        _PropertyInTheOpenRecordsTotal_QuotedPremium = Nothing
                    End If
                    If _PropertyInTheOpenRecordsTotal_EQ_Premium IsNot Nothing Then
                        _PropertyInTheOpenRecordsTotal_EQ_Premium = Nothing
                    End If
                    If _PropertyInTheOpenRecordsTotal_QuotedPremium_With_EQ IsNot Nothing Then
                        _PropertyInTheOpenRecordsTotal_QuotedPremium_With_EQ = Nothing
                    End If

                    If _Acreage IsNot Nothing Then
                        _Acreage = Nothing
                    End If
                    If _CondoRentedTypeId IsNot Nothing Then
                        _CondoRentedTypeId = Nothing
                    End If
                    If _ConstructionTypeId IsNot Nothing Then
                        _ConstructionTypeId = Nothing
                    End If
                    If _DeductibleLimitId IsNot Nothing Then
                        _DeductibleLimitId = Nothing
                    End If
                    If _WindHailDeductibleLimitId IsNot Nothing Then
                        _WindHailDeductibleLimitId = Nothing
                    End If
                    If _DayEmployees <> Nothing Then
                        _DayEmployees = Nothing
                    End If
                    If _DaytimeOccupancy <> Nothing Then
                        _DaytimeOccupancy = Nothing
                    End If
                    If _FamilyUnitsId IsNot Nothing Then
                        _FamilyUnitsId = Nothing
                    End If
                    If _FireDepartmentDistanceId IsNot Nothing Then
                        _FireDepartmentDistanceId = Nothing
                    End If
                    If _FireHydrantDistanceId IsNot Nothing Then
                        _FireHydrantDistanceId = Nothing
                    End If
                    If _FormTypeId IsNot Nothing Then
                        _FormTypeId = Nothing
                    End If
                    If _FoundationTypeId IsNot Nothing Then
                        _FoundationTypeId = Nothing
                    End If
                    If _LastCostEstimatorDate IsNot Nothing Then
                        _LastCostEstimatorDate = Nothing
                    End If
                    If _MarketValue IsNot Nothing Then
                        _MarketValue = Nothing
                    End If
                    If _NumberOfFamiliesId IsNot Nothing Then
                        _NumberOfFamiliesId = Nothing
                    End If
                    If _OccupancyCodeId IsNot Nothing Then
                        _OccupancyCodeId = Nothing
                    End If
                    If _PrimaryResidence <> Nothing Then
                        _PrimaryResidence = Nothing
                    End If
                    If _ProgramTypeId IsNot Nothing Then
                        _ProgramTypeId = Nothing
                    End If
                    If _NumberOfApartments IsNot Nothing Then
                        _NumberOfApartments = Nothing
                    End If
                    If _NumberOfSolidFuelBurningUnits IsNot Nothing Then
                        _NumberOfSolidFuelBurningUnits = Nothing
                    End If
                    If _RebuildCost IsNot Nothing Then
                        _RebuildCost = Nothing
                    End If
                    If _Remarks IsNot Nothing Then
                        _Remarks = Nothing
                    End If
                    If _SquareFeet IsNot Nothing Then
                        _SquareFeet = Nothing
                    End If
                    If _StructureTypeId IsNot Nothing Then
                        _StructureTypeId = Nothing
                    End If
                    If _YearBuilt IsNot Nothing Then
                        _YearBuilt = Nothing
                    End If
                    If _A_Dwelling_Limit IsNot Nothing Then
                        _A_Dwelling_Limit = Nothing
                    End If
                    If _A_Dwelling_LimitIncreased IsNot Nothing Then
                        _A_Dwelling_LimitIncreased = Nothing
                    End If
                    If _A_Dwelling_LimitIncluded IsNot Nothing Then
                        _A_Dwelling_LimitIncluded = Nothing
                    End If
                    If _A_Dwelling_Calc IsNot Nothing Then 'added 12/4/2014
                        _A_Dwelling_Calc = Nothing
                    End If
                    If _B_OtherStructures_Limit IsNot Nothing Then
                        _B_OtherStructures_Limit = Nothing
                    End If
                    If _B_OtherStructures_LimitIncreased IsNot Nothing Then
                        _B_OtherStructures_LimitIncreased = Nothing
                    End If
                    If _B_OtherStructures_LimitIncluded IsNot Nothing Then
                        _B_OtherStructures_LimitIncluded = Nothing
                    End If
                    If _B_OtherStructures_Calc IsNot Nothing Then 'added 12/10/2014
                        _B_OtherStructures_Calc = Nothing
                    End If
                    If _C_PersonalProperty_Limit IsNot Nothing Then
                        _C_PersonalProperty_Limit = Nothing
                    End If
                    If _C_PersonalProperty_LimitIncreased IsNot Nothing Then
                        _C_PersonalProperty_LimitIncreased = Nothing
                    End If
                    If _C_PersonalProperty_LimitIncluded IsNot Nothing Then
                        _C_PersonalProperty_LimitIncluded = Nothing
                    End If
                    If _C_PersonalProperty_Calc IsNot Nothing Then 'added 12/10/2014
                        _C_PersonalProperty_Calc = Nothing
                    End If
                    If _D_LossOfUse_Limit IsNot Nothing Then
                        _D_LossOfUse_Limit = Nothing
                    End If
                    If _D_LossOfUse_LimitIncreased IsNot Nothing Then
                        _D_LossOfUse_LimitIncreased = Nothing
                    End If
                    If _D_LossOfUse_LimitIncluded IsNot Nothing Then
                        _D_LossOfUse_LimitIncluded = Nothing
                    End If
                    If _D_LossOfUse_Calc IsNot Nothing Then 'added 12/10/2014
                        _D_LossOfUse_Calc = Nothing
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
                    If _Updates IsNot Nothing Then 'added 7/31/2013
                        _Updates.Dispose()
                        _Updates = Nothing
                    End If
                    If _Modifiers IsNot Nothing Then 'added 7/31/2013
                        If _Modifiers.Count > 0 Then
                            For Each m As QuickQuoteModifier In _Modifiers
                                m.Dispose()
                                m = Nothing
                            Next
                            _Modifiers.Clear()
                        End If
                        _Modifiers = Nothing
                    End If
                    If _MultiPolicyDiscount <> Nothing Then
                        _MultiPolicyDiscount = Nothing
                    End If
                    If _MatureHomeownerDiscount <> Nothing Then
                        _MatureHomeownerDiscount = Nothing
                    End If
                    If _NewHomeDiscount <> Nothing Then
                        _NewHomeDiscount = Nothing
                    End If
                    If _SelectMarketCredit <> Nothing Then
                        _SelectMarketCredit = Nothing
                    End If
                    If _BurglarAlarm_LocalAlarmSystem <> Nothing Then
                        _BurglarAlarm_LocalAlarmSystem = Nothing
                    End If
                    If _BurglarAlarm_CentralStationAlarmSystem <> Nothing Then
                        _BurglarAlarm_CentralStationAlarmSystem = Nothing
                    End If
                    If _FireSmokeAlarm_LocalAlarmSystem <> Nothing Then
                        _FireSmokeAlarm_LocalAlarmSystem = Nothing
                    End If
                    If _FireSmokeAlarm_CentralStationAlarmSystem <> Nothing Then
                        _FireSmokeAlarm_CentralStationAlarmSystem = Nothing
                    End If
                    If _FireSmokeAlarm_SmokeAlarm <> Nothing Then
                        _FireSmokeAlarm_SmokeAlarm = Nothing
                    End If
                    If _SprinklerSystem_AllExcept <> Nothing Then
                        _SprinklerSystem_AllExcept = Nothing
                    End If
                    If _SprinklerSystem_AllIncluding <> Nothing Then
                        _SprinklerSystem_AllIncluding = Nothing
                    End If
                    If _sprinklerSystem <> Nothing Then 'HOM2018 Upgrade
                        _sprinklerSystem = Nothing
                    End If
                    If _TrampolineSurcharge <> Nothing Then
                        _TrampolineSurcharge = Nothing
                    End If
                    If _WoodOrFuelBurningApplianceSurcharge <> Nothing Then
                        _WoodOrFuelBurningApplianceSurcharge = Nothing
                    End If
                    If _SwimmingPoolHotTubSurcharge <> Nothing Then 'added 7/28/2014
                        _SwimmingPoolHotTubSurcharge = Nothing
                    End If
                    If _SectionCoverages IsNot Nothing Then
                        If _SectionCoverages.Count > 0 Then
                            For Each sc As QuickQuoteSectionCoverage In _SectionCoverages
                                sc.Dispose()
                                sc = Nothing
                            Next
                            _SectionCoverages.Clear()
                        End If
                        _SectionCoverages = Nothing
                    End If
                    If _Exclusions IsNot Nothing Then
                        If _Exclusions.Count > 0 Then
                            For Each e As QuickQuoteExclusion In _Exclusions
                                e.Dispose()
                                e = Nothing
                            Next
                            _Exclusions.Clear()
                        End If
                        _Exclusions = Nothing
                    End If
                    If _InlandMarines IsNot Nothing Then
                        If _InlandMarines.Count > 0 Then
                            For Each im As QuickQuoteInlandMarine In _InlandMarines
                                im.Dispose()
                                im = Nothing
                            Next
                            _InlandMarines.Clear()
                        End If
                        _InlandMarines = Nothing
                    End If
                    If _RvWatercrafts IsNot Nothing Then
                        If _RvWatercrafts.Count > 0 Then
                            For Each rv As QuickQuoteRvWatercraft In _RvWatercrafts
                                rv.Dispose()
                                rv = Nothing
                            Next
                            _RvWatercrafts.Clear()
                        End If
                        _RvWatercrafts = Nothing
                    End If
                    If _SectionICoverages IsNot Nothing Then
                        If _SectionICoverages.Count > 0 Then
                            For Each sIc As QuickQuoteSectionICoverage In _SectionICoverages
                                sIc.Dispose()
                                sIc = Nothing
                            Next
                            _SectionICoverages.Clear()
                        End If
                        _SectionICoverages = Nothing
                    End If
                    If _SectionIICoverages IsNot Nothing Then
                        If _SectionIICoverages.Count > 0 Then
                            For Each sIIc As QuickQuoteSectionIICoverage In _SectionIICoverages
                                sIIc.Dispose()
                                sIIc = Nothing
                            Next
                            _SectionIICoverages.Clear()
                        End If
                        _SectionIICoverages = Nothing
                    End If
                    If _SectionIAndIICoverages IsNot Nothing Then
                        If _SectionIAndIICoverages.Count > 0 Then
                            For Each sIAndIIc As QuickQuoteSectionIAndIICoverage In _SectionIAndIICoverages
                                sIAndIIc.Dispose()
                                sIAndIIc = Nothing
                            Next
                            _SectionIAndIICoverages.Clear()
                        End If
                        _SectionIAndIICoverages = Nothing
                    End If
                    If _PolicyUnderwritings IsNot Nothing Then
                        If _PolicyUnderwritings.Count > 0 Then
                            For Each uw As QuickQuotePolicyUnderwriting In _PolicyUnderwritings
                                uw.Dispose()
                                uw = Nothing
                            Next
                            _PolicyUnderwritings.Clear()
                        End If
                        _PolicyUnderwritings = Nothing
                    End If
                    If _NumberOfDaysRented IsNot Nothing Then
                        _NumberOfDaysRented = Nothing
                    End If
                    If _NumberOfUnitsId IsNot Nothing Then
                        _NumberOfUnitsId = Nothing
                    End If
                    If _UsageTypeId IsNot Nothing Then
                        _UsageTypeId = Nothing
                    End If
                    If _VacancyFromDate IsNot Nothing Then
                        _VacancyFromDate = Nothing
                    End If
                    If _VacancyToDate IsNot Nothing Then
                        _VacancyToDate = Nothing
                    End If

                    'added 2/18/2014
                    If _HasConvertedCoverages <> Nothing Then
                        _HasConvertedCoverages = Nothing
                    End If
                    If _HasConvertedModifiers <> Nothing Then
                        _HasConvertedModifiers = Nothing
                    End If
                    If _HasConvertedScheduledCoverages <> Nothing Then
                        _HasConvertedScheduledCoverages = Nothing
                    End If
                    If _HasConvertedSectionCoverages <> Nothing Then
                        _HasConvertedSectionCoverages = Nothing
                    End If

                    If _PremiumFullterm IsNot Nothing Then 'added 4/2/2014
                        _PremiumFullterm = Nothing
                    End If
                    If _BuildingsTotal_PremiumFullTerm IsNot Nothing Then 'added 4/2/2014
                        _BuildingsTotal_PremiumFullTerm = Nothing
                    End If

                    'added 4/23/2014 for reconciliation
                    If _LocationNum IsNot Nothing Then
                        _LocationNum = Nothing
                    End If
                    If _HasLocationAddressChanged <> Nothing Then
                        _HasLocationAddressChanged = Nothing
                    End If
                    If _CanUseFarmBarnBuildingNumForBuildingReconciliation <> Nothing Then
                        _CanUseFarmBarnBuildingNumForBuildingReconciliation = Nothing
                    End If
                    If _CanUseAdditionalInterestNumForAdditionalInterestReconciliation <> Nothing Then 'added 4/29/2014
                        _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = Nothing
                    End If
                    'added 10/17/2018 for multi-state
                    qqHelper.DisposeString(_LocationNum_MasterPart)
                    qqHelper.DisposeString(_LocationNum_CGLPart)
                    qqHelper.DisposeString(_LocationNum_CPRPart)
                    qqHelper.DisposeString(_LocationNum_CIMPart)
                    qqHelper.DisposeString(_LocationNum_CRMPart)
                    qqHelper.DisposeString(_LocationNum_GARPart)
                    'added 10/18/2018 for multi-state
                    _CanUseFarmBarnBuildingNumForMasterPartBuildingReconciliation = Nothing
                    _CanUseFarmBarnBuildingNumForCGLPartBuildingReconciliation = Nothing
                    _CanUseFarmBarnBuildingNumForCPRPartBuildingReconciliation = Nothing
                    _CanUseFarmBarnBuildingNumForCIMPartBuildingReconciliation = Nothing
                    _CanUseFarmBarnBuildingNumForCRMPartBuildingReconciliation = Nothing
                    _CanUseFarmBarnBuildingNumForGARPartBuildingReconciliation = Nothing

                    'added 7/29/2014 for HOM (specific to mobile home formTypes... FormTypeId 6 = ML-2 - Mobile Home Owner Occupied; 7 = ML-4 - Mobile Home Tenant Occupied
                    If _TheftDeductibleLimitId IsNot Nothing Then 'int; corresponds to CoverageCodeId 20034; static data list
                        _TheftDeductibleLimitId = Nothing
                    End If
                    _HomeInPark = Nothing
                    If _MobileHomeConsecutiveMonthsOccupied IsNot Nothing Then 'int
                        _MobileHomeConsecutiveMonthsOccupied = Nothing
                    End If
                    If _MobileHomeCostNew IsNot Nothing Then 'dec
                        _MobileHomeCostNew = Nothing
                    End If
                    If _MobileHomeLength IsNot Nothing Then 'int
                        _MobileHomeLength = Nothing
                    End If
                    If _MobileHomeMake IsNot Nothing Then
                        _MobileHomeMake = Nothing
                    End If
                    If _MobileHomeModel IsNot Nothing Then
                        _MobileHomeModel = Nothing
                    End If
                    If _MobileHomeParkName IsNot Nothing Then
                        _MobileHomeParkName = Nothing
                    End If
                    If _MobileHomePurchasePrice IsNot Nothing Then 'dec
                        _MobileHomePurchasePrice = Nothing
                    End If
                    If _MobileHomeSkirtTypeId IsNot Nothing Then 'int; static data list
                        _MobileHomeSkirtTypeId = Nothing
                    End If
                    If _MobileHomeTieDownTypeId IsNot Nothing Then 'int; static data list
                        _MobileHomeTieDownTypeId = Nothing
                    End If
                    If _MobileHomeVIN IsNot Nothing Then
                        _MobileHomeVIN = Nothing
                    End If
                    If _MobileHomeWidth IsNot Nothing Then 'int
                        _MobileHomeWidth = Nothing
                    End If
                    _PermanentFoundation = Nothing

                    If _PropertyValuation IsNot Nothing Then 'added 8/5/2014 for e2Value
                        _PropertyValuation.Dispose()
                        _PropertyValuation = Nothing
                    End If
                    If _ArchitecturalStyle IsNot Nothing Then 'added 8/14/2014 for e2Value; corresponds to prop w/ the same name in QuickQuotePropertyValuationRequest
                        _ArchitecturalStyle = Nothing
                    End If

                    'added 10/13/2014 for surcharge premiums... added as modifiers, but prem comes back in coverage... may also need to set Checkbox to True on Coverage
                    If _SwimmingPoolHotTubSurchargeQuotePremium IsNot Nothing Then
                        _SwimmingPoolHotTubSurchargeQuotePremium = Nothing
                    End If
                    If _TrampolineSurchargeQuotePremium IsNot Nothing Then
                        _TrampolineSurchargeQuotePremium = Nothing
                    End If
                    If _WoodOrFuelBurningApplianceSurchargeQuotePremium IsNot Nothing Then
                        _WoodOrFuelBurningApplianceSurchargeQuotePremium = Nothing
                    End If
                    'added 10/14/2014 for other HOM coverage premiums
                    If _DeductibleQuotedPremium IsNot Nothing Then
                        _DeductibleQuotedPremium = Nothing
                    End If
                    If _WindHailDeductibleQuotedPremium IsNot Nothing Then
                        _WindHailDeductibleQuotedPremium = Nothing
                    End If
                    If _A_Dwelling_QuotedPremium IsNot Nothing Then
                        _A_Dwelling_QuotedPremium = Nothing
                    End If
                    If _B_OtherStructures_QuotedPremium IsNot Nothing Then
                        _B_OtherStructures_QuotedPremium = Nothing
                    End If
                    If _C_PersonalProperty_QuotedPremium IsNot Nothing Then
                        _C_PersonalProperty_QuotedPremium = Nothing
                    End If
                    If _D_LossOfUse_QuotedPremium IsNot Nothing Then
                        _D_LossOfUse_QuotedPremium = Nothing
                    End If
                    If _TheftDeductibleQuotedPremium IsNot Nothing Then
                        _TheftDeductibleQuotedPremium = Nothing
                    End If

                    'added 10/14/2014 for reconciliation
                    _CanUseExclusionNumForExclusionReconciliation = Nothing
                    _CanUseInlandMarineNumForInlandMarineReconciliation = Nothing
                    _CanUsePolicyUnderwritingNumForPolicyUnderwritingReconciliation = Nothing
                    _CanUseRvWatercraftNumForRvWatercraftReconciliation = Nothing
                    '_CanUseSectionCoverageNumForSectionCoverageReconciliation = Nothing 'removed 10/29/2018
                    qqHelper.DisposeQuickQuoteCanUseDiamondNumFlagGroup(_CanUseDiamondNumForSectionCoverageReconciliationGroup)

                    'added 10/29/2014 for HOM
                    If _TerritoryNumber IsNot Nothing Then
                        _TerritoryNumber = Nothing
                    End If

                    'added 11/17/2014 for HOM
                    If _InlandMarinesTotal_Premium IsNot Nothing Then
                        _InlandMarinesTotal_Premium = Nothing
                    End If
                    If _InlandMarinesTotal_CoveragePremium IsNot Nothing Then
                        _InlandMarinesTotal_CoveragePremium = Nothing
                    End If
                    If _RvWatercraftsTotal_Premium IsNot Nothing Then
                        _RvWatercraftsTotal_Premium = Nothing
                    End If
                    If _RvWatercraftsTotal_CoveragesPremium IsNot Nothing Then
                        _RvWatercraftsTotal_CoveragesPremium = Nothing
                    End If

                    _CanUseScheduledCoverageNumForScheduledCoverageReconciliation = Nothing 'added 1/22/2015

                    'added 2/25/2015 for Farm
                    qqHelper.DisposeCoverages(_IncidentalDwellingCoverages)
                    _HasConvertedIncidentalDwellingCoverages = Nothing
                    'added 2/26/2015
                    If _Acreages IsNot Nothing Then
                        If _Acreages.Count > 0 Then
                            For Each a As QuickQuoteAcreage In _Acreages
                                a.Dispose()
                                a = Nothing
                            Next
                            _Acreages.Clear()
                        End If
                        _Acreages = Nothing
                    End If
                    _CanUseAcreageNumForAcreageReconciliation = Nothing
                    If _IncomeLosses IsNot Nothing Then
                        If _IncomeLosses.Count > 0 Then
                            For Each il As QuickQuoteIncomeLoss In _IncomeLosses
                                il.Dispose()
                                il = Nothing
                            Next
                            _IncomeLosses.Clear()
                        End If
                        _IncomeLosses = Nothing
                    End If
                    _CanUseLossOfIncomeNumForIncomeLossReconciliation = Nothing
                    _HasConvertedIncomeLosses = Nothing
                    If _ResidentNames IsNot Nothing Then
                        If _ResidentNames.Count > 0 Then
                            For Each rn As QuickQuoteResidentName In _ResidentNames
                                rn.Dispose()
                                rn = Nothing
                            Next
                            _ResidentNames.Clear()
                        End If
                        _ResidentNames = Nothing
                    End If
                    _CanUseResidentNumForResidentNameReconciliation = Nothing
                    'added 3/4/2015
                    qqHelper.DisposeString(_DwellingTypeId) 'static data; example had 22 (Type 1)
                    _FarmTypeBees = Nothing
                    _FarmTypeDairy = Nothing
                    _FarmTypeFeedLot = Nothing
                    _FarmTypeFieldCrops = Nothing
                    _FarmTypeFlowers = Nothing
                    _FarmTypeFruits = Nothing
                    _FarmTypeFurbearingAnimals = Nothing
                    _FarmTypeGreenhouses = Nothing
                    _FarmTypeHobby = Nothing
                    _FarmTypeHorse = Nothing
                    _FarmTypeLivestock = Nothing
                    _FarmTypeMushrooms = Nothing
                    _FarmTypeNurseryStock = Nothing
                    _FarmTypeNuts = Nothing
                    qqHelper.DisposeString(_FarmTypeOtherDescription)
                    _FarmTypePoultry = Nothing
                    _FarmTypeSod = Nothing
                    _FarmTypeSwine = Nothing
                    _FarmTypeTobacco = Nothing
                    _FarmTypeTurkey = Nothing
                    _FarmTypeVegetables = Nothing
                    _FarmTypeVineyards = Nothing
                    _FarmTypeWorms = Nothing
                    qqHelper.DisposeString(_LegalDescription)
                    _Owns = Nothing
                    _RoofExclusion = Nothing
                    'added 5/20/2015
                    _AcreageOnly = Nothing
                    'added 6/11/2015
                    _HobbyFarmCredit = Nothing
                    'added 6/12/2015
                    _FireDepartmentAlarm = Nothing
                    _PoliceDepartmentTheftAlarm = Nothing
                    _BurglarAlarm_CentralAlarmSystem = Nothing
                    _FireSmokeAlarm_CentralAlarmSystem = Nothing
                    'added 9/23/2015
                    qqHelper.DisposeString(_Farm_L_Liability_QuotedPremium)
                    qqHelper.DisposeString(_Farm_M_Medical_Payments_QuotedPremium)

                    'added 11/2/2015 for DFR
                    qqHelper.DisposeString(_A_Dwelling_EC_QuotedPremium) '70223
                    qqHelper.DisposeString(_C_Contents_EC_QuotedPremium) '70224
                    qqHelper.DisposeString(_D_and_E_EC_QuotedPremium) '80106
                    qqHelper.DisposeString(_B_OtherStructures_EC_QuotedPremium) '70218
                    qqHelper.DisposeString(_A_Dwelling_VMM_QuotedPremium) '70225
                    qqHelper.DisposeString(_C_Contents_VMM_QuotedPremium) '70226
                    qqHelper.DisposeString(_D_and_E_VMM_QuotedPremium) '80107
                    qqHelper.DisposeString(_B_OtherStructures_VMM_QuotedPremium) '70220

                    'added 9/20/2016 for Verisk Protection Class
                    qqHelper.DisposeString(_FireDistrictName)
                    qqHelper.DisposeString(_ProtectionClassSystemGeneratedId)
                    'added 9/24/2016 for Verisk Protection Class
                    qqHelper.DisposeString(_PPCMatchTypeId)

                    'added 10/7/2016 to compare before/after address
                    If _OriginalAddress IsNot Nothing Then
                        _OriginalAddress.Dispose()
                        _OriginalAddress = Nothing
                    End If

                    'added 10/14/2016 for Verisk Protection Class
                    qqHelper.DisposeString(_FireStationDistance)

                    'added 2/20/2017
                    _CanUseClassificationNumForClassificationReconciliation = Nothing

                    '3/9/2017 - BOP stuff
                    If _HasTenantAutoLegalLiability <> Nothing Then
                        _HasTenantAutoLegalLiability = Nothing
                    End If
                    If _TenantAutoLegalLiabilityDeductibleId IsNot Nothing Then
                        _TenantAutoLegalLiabilityDeductibleId = Nothing
                    End If
                    If _TenantAutoLegalLiabilityLimitOfLiabilityId IsNot Nothing Then
                        _TenantAutoLegalLiabilityLimitOfLiabilityId = Nothing
                    End If
                    If _TenantAutoLegalLiabilityDeductible IsNot Nothing Then
                        _TenantAutoLegalLiabilityDeductible = Nothing
                    End If
                    If _TenantAutoLegalLiabilityLimitOfLiability IsNot Nothing Then
                        _TenantAutoLegalLiabilityLimitOfLiability = Nothing
                    End If
                    If _TenantAutoLegalLiabilityQuotedPremium IsNot Nothing Then
                        _TenantAutoLegalLiabilityQuotedPremium = Nothing
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
                    If _CustomerAutoLegalLiabilityDeductible IsNot Nothing Then
                        _CustomerAutoLegalLiabilityDeductible = Nothing
                    End If
                    If _CustomerAutoLegalLiabilityLimitOfLiability IsNot Nothing Then
                        _CustomerAutoLegalLiabilityLimitOfLiability = Nothing
                    End If
                    If _CustomerAutoLegalLiabilityQuotedPremium IsNot Nothing Then
                        _CustomerAutoLegalLiabilityQuotedPremium = Nothing
                    End If
                    If _hasFineArts <> Nothing Then
                        _hasFineArts = Nothing
                    End If
                    If _FineArtsQuotedPremium IsNot Nothing Then
                        _FineArtsQuotedPremium = Nothing
                    End If

                    'added 5/8/2017 for GAR
                    qqHelper.DisposeString(_LiabilityQuotedPremium) 'covCodeId 10111
                    qqHelper.DisposeString(_LiabilityAggregateLiabilityIncrementTypeId) 'covDetail; covCodeId 10111
                    qqHelper.DisposeString(_LiabilityCoverageLimitId) 'covCodeId 10111
                    qqHelper.DisposeString(_MedicalPaymentsQuotedPremium) 'covCodeId 10112
                    qqHelper.DisposeString(_MedicalPaymentsCoverageLimitId) 'covCodeId 10112
                    _HasUninsuredUnderinsuredMotoristBIandPD = Nothing 'covCodeId 10113
                    qqHelper.DisposeString(_UninsuredUnderinsuredMotoristBIandPDQuotedPremium) 'covCodeId 10113; may not be populated
                    qqHelper.DisposeString(_UninsuredUnderinsuredMotoristBIandPDCoverageLimitId) 'covCodeId 10113
                    qqHelper.DisposeString(_UninsuredUnderinsuredMotoristBIandPDDeductibleId) 'covCodeId 10113
                    qqHelper.DisposeString(_PhysicalDamageOtherThanCollisionStandardOpenLotsQuotedPremium) 'covCodeId 10116
                    qqHelper.DisposeString(_PhysicalDamageOtherThanCollisionStandardOpenLotsManualLimitAmount) 'covCodeId 10116; added 5/12/2017
                    'qqHelper.DisposeString(_PhysicalDamageOtherThanCollisionStandardOpenLotsDeductibleCategoryTypeId) 'covDetail; covCodeId 10116; removed 5/15/2017 since it's the same for all PD OtherThanCollision covs; will use 1 new field
                    'qqHelper.DisposeString(_PhysicalDamageOtherThanCollisionStandardOpenLotsOtherThanCollisionTypeId) 'covDetail; covCodeId 10116; removed 5/15/2017 since it's the same for all PD OtherThanCollision covs; will use 1 new field
                    'qqHelper.DisposeString(_PhysicalDamageOtherThanCollisionStandardOpenLotsDeductibleId) 'covCodeId 10116; removed 5/15/2017 since it's the same for all PD OtherThanCollision covs; will use 1 new field
                    _HasDealersBlanketCollision = Nothing 'covCodeId 10120
                    qqHelper.DisposeString(_DealersBlanketCollisionQuotedPremium) 'covCodeId 10120
                    qqHelper.DisposeString(_DealersBlanketCollisionDeductibleId) 'covCodeId 10120
                    qqHelper.DisposeString(_GarageKeepersOtherThanCollisionQuotedPremium) 'covCodeId 10086; may not be populated
                    qqHelper.DisposeString(_GarageKeepersOtherThanCollisionManualLimitAmount) 'covCodeId 10086
                    qqHelper.DisposeString(_GarageKeepersOtherThanCollisionDeductibleCategoryTypeId) 'covCodeId 10086
                    qqHelper.DisposeString(_GarageKeepersOtherThanCollisionTypeId) 'covCodeId 10086
                    qqHelper.DisposeString(_GarageKeepersOtherThanCollisionDeductibleId) 'covCodeId 10086
                    qqHelper.DisposeString(_GarageKeepersOtherThanCollisionBasisTypeId) 'added 7/14/2017; covDetail; covCodeId 10086
                    qqHelper.DisposeString(_GarageKeepersCollisionQuotedPremium) 'covCodeId 10087; may not be populated
                    qqHelper.DisposeString(_GarageKeepersCollisionManualLimitAmount) 'covCodeId 10087
                    qqHelper.DisposeString(_GarageKeepersCollisionDeductibleId) 'covCodeId 10087
                    qqHelper.DisposeString(_GarageKeepersCollisionBasisTypeId) 'added 7/15/2017; covDetail; covCodeId 10087
                    _HasGarageKeepersCoverageExtensions = Nothing 'covCodeId 10126
                    qqHelper.DisposeString(_GarageKeepersCoverageExtensionsQuotedPremium) 'covCodeId 10126; may not be populated
                    'added 5/11/2017 for GAR
                    qqHelper.DisposeString(_ClassIIEmployees25AndOlder)
                    qqHelper.DisposeString(_ClassIIEmployeesUnderAge25)
                    qqHelper.DisposeString(_ClassIOtherEmployees)
                    qqHelper.DisposeString(_ClassIRegularEmployees)
                    qqHelper.DisposeString(_ClassificationTypeId)
                    qqHelper.DisposeString(_NumberOfEmployees)
                    qqHelper.DisposeString(_Payroll)
                    qqHelper.DisposeString(_UninsuredUnderinsuredMotoristBIandPDNumberOfPlates) 'covCodeId 10113; covDetail
                    'added 5/15/2017 for GAR
                    qqHelper.DisposeString(_PhysicalDamageOtherThanCollisionBuildingQuotedPremium) 'covCodeId 10115
                    qqHelper.DisposeString(_PhysicalDamageOtherThanCollisionBuildingManualLimitAmount) 'covCodeId 10115
                    qqHelper.DisposeString(_PhysicalDamageOtherThanCollisionNonStandardOpenLotsQuotedPremium) 'covCodeId 10117
                    qqHelper.DisposeString(_PhysicalDamageOtherThanCollisionNonStandardOpenLotsManualLimitAmount) 'covCodeId 10117
                    qqHelper.DisposeString(_PhysicalDamageOtherThanCollisionMiscellaneousBuildingsQuotedPremium) 'covCodeId 10118
                    qqHelper.DisposeString(_PhysicalDamageOtherThanCollisionMiscellaneousBuildingsManualLimitAmount) 'covCodeId 10118
                    qqHelper.DisposeString(_PhysicalDamageOtherThanCollisionMiscellaneousOpenLotsQuotedPremium) 'covCodeId 10119
                    qqHelper.DisposeString(_PhysicalDamageOtherThanCollisionMiscellaneousOpenLotsManualLimitAmount) 'covCodeId 10119
                    qqHelper.DisposeString(_PhysicalDamageOtherThanCollisionTotalQuotedPremium) 'SUM of covCodeIds 10115, 10116, 10117, 10118, and 10119
                    qqHelper.DisposeString(_PhysicalDamageOtherThanCollisionTotalManualLimitAmount) 'SUM of covCodeIds 10115, 10116, 10117, 10118, and 10119
                    qqHelper.DisposeString(_PhysicalDamageOtherThanCollisionDeductibleCategoryTypeId) 'covDetail; covCodeIds 10115, 10116, 10117, 10118, and 10119
                    qqHelper.DisposeString(_PhysicalDamageOtherThanCollisionTypeId) 'covDetail; covCodeIds 10115, 10116, 10117, 10118, and 10119
                    qqHelper.DisposeString(_PhysicalDamageOtherThanCollisionDeductibleId) 'covCodeIds 10115, 10116, 10117, 10118, and 10119

                    'added 11/21/2017 for HOM 2018 Upgrade
                    '--------------------------------------
                    qqHelper.DisposeString(_NumberOfUnitsInFireDivision)
                    qqHelper.DisposeString(_PrimaryPolicyNumber)
                    '--------------------------------------

                    'added 10/27/2017 for MBR Equipment Breakdown
                    qqHelper.DisposeString(_EquipmentBreakdown_MBR_PollutantCleanupRemovalCoverageLimitId) 'covCodeId 80513; has CoverageBasisTypeId 1
                    qqHelper.DisposeString(_EquipmentBreakdown_MBR_PollutantCleanupRemovalDeductibleId) 'covCodeId 80513; has CoverageBasisTypeId 1
                    qqHelper.DisposeString(_EquipmentBreakdown_MBR_PollutantCleanupRemovalQuotedPremium) 'covCodeId 80513; has CoverageBasisTypeId 1
                    qqHelper.DisposeString(_EquipmentBreakdown_MBR_RefrigerantContaminationCoverageLimitId) 'covCodeId 80514; has CoverageBasisTypeId 1
                    qqHelper.DisposeString(_EquipmentBreakdown_MBR_RefrigerantContaminationDeductibleId) 'covCodeId 80514; has CoverageBasisTypeId 1
                    qqHelper.DisposeString(_EquipmentBreakdown_MBR_RefrigerantContaminationQuotedPremium) 'covCodeId 80514; has CoverageBasisTypeId 1
                    qqHelper.DisposeString(_EquipmentBreakdown_MBR_SpoilageCoverageLimitId) 'covCodeId 80515; has CoverageBasisTypeId 1
                    qqHelper.DisposeString(_EquipmentBreakdown_MBR_SpoilageDeductibleId) 'covCodeId 80515; has CoverageBasisTypeId 1
                    qqHelper.DisposeString(_EquipmentBreakdown_MBR_SpoilageQuotedPremium) 'covCodeId 80515; has CoverageBasisTypeId 1
                    qqHelper.DisposeString(_EquipmentBreakdown_MBR_UnderwrittenRateQuotedPremium) 'covCodeId 80521
                    qqHelper.DisposeString(_EquipmentBreakdown_MBR_TotalQuotedPremium) 'SUM of covCodeIds 21042, 80513, 80514, 80515, 80521
                    'added 10/30/2017
                    qqHelper.DisposeString(_EquipmentBreakdown_AdjustmentFactor) 'covCodeId 21042
                    'added 11/1/2017
                    qqHelper.DisposeString(_EquipmentBreakdownDeductibleIdBackup) 'covCodeId 21042

                    'added 8/2/2018
                    _QuoteStateTakenFrom = Nothing

                    'added 1/16/2019
                    _DisplayNum = Nothing
                    _OriginalDisplayNum = Nothing
                    _OkayToUseDisplayNum = Nothing

                    qqHelper.DisposeString(_DetailStatusCode) 'added 5/15/2019

                    qqHelper.DisposeString(_AddedDate)
                    qqHelper.DisposeString(_EffectiveDate)
                    qqHelper.DisposeString(_LastModifiedDate)
                    qqHelper.DisposeString(_PCAdded_Date)
                    qqHelper.DisposeString(_AddedImageNum)

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
