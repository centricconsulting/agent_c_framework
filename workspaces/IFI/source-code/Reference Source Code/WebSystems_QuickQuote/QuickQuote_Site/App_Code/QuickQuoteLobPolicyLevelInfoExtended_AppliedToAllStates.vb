Imports System.Web
Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' object used to store policy-level lob-specific information (that applies to all states) for a quote; also includes properties that were previously on QuickQuote only
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()>
    Public Class QuickQuoteLobPolicyLevelInfoExtended_AppliedToAllStates 'added 8/17/2018
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        'PolicyLevel
        Private _CPP_CRM_ProgramTypeId As String
        Private _CPP_GAR_ProgramTypeId As String
        Private _RiskGradeLookupId_Original As String
        Private _CPP_CGL_RiskGrade As String
        Private _CPP_CGL_RiskGradeLookupId As String
        Private _CPP_CPR_RiskGrade As String
        Private _CPP_CPR_RiskGradeLookupId As String
        Private _ErrorRiskGradeLookupId As String
        Private _ReplacementRiskGradeLookupId As String
        Private _CPP_CGL_ErrorRiskGradeLookupId As String
        Private _CPP_CGL_ReplacementRiskGradeLookupId As String
        Private _CPP_CPR_ErrorRiskGradeLookupId As String
        Private _CPP_CPR_ReplacementRiskGradeLookupId As String
        Private _CPP_CIM_RiskGrade As String
        Private _CPP_CIM_RiskGradeLookupId As String
        Private _CPP_CIM_ErrorRiskGradeLookupId As String
        Private _CPP_CIM_ReplacementRiskGradeLookupId As String
        Private _CPP_CRM_RiskGrade As String
        Private _CPP_CRM_RiskGradeLookupId As String
        Private _CPP_CRM_ErrorRiskGradeLookupId As String
        Private _CPP_CRM_ReplacementRiskGradeLookupId As String
        Private _OccurrenceLiabilityLimit As String
        Private _OccurrenceLiabilityLimitId As String
        Private _CPP_TargetMarketID As String
        Private _OccurrencyLiabilityQuotedPremium As String
        Private _TenantsFireLiability As String
        Private _TenantsFireLiabilityId As String
        Private _TenantsFireLiabilityQuotedPremium As String
        Private _PropertyDamageLiabilityDeductible As String
        Private _PropertyDamageLiabilityDeductibleId As String
        Private _BlanketRatingQuotedPremium As String
        Private _HasEnhancementEndorsement As Boolean 'BusinessMasterEnhancement in QuickQuoteObject
        Private _EnhancementEndorsementQuotedPremium As String 'BusinessMasterEnhancement in QuickQuoteObject
        Private _Has_PackageGL_EnhancementEndorsement As Boolean
        Private _PackageGL_EnhancementEndorsementQuotedPremium As String
        Private _Has_PackageGL_PlusEnhancementEndorsement As Boolean
        Private _PackageGL_PlusEnhancementEndorsementQuotedPremium As String
        Private _Has_PackageCPR_EnhancementEndorsement As Boolean
        Private _PackageCPR_EnhancementEndorsementQuotedPremium As String
        'Private _AdditionalInsuredsCount As Integer
        'Private _AdditionalInsuredsCheckboxBOP As List(Of QuickQuoteAdditionalInsured)
        'Private _HasAdditionalInsuredsCheckboxBOP As Boolean
        'Private _AdditionalInsuredsManualCharge As String
        'Private _AdditionalInsuredsQuotedPremium As String
        'Private _AdditionalInsureds As Generic.List(Of QuickQuoteAdditionalInsured)
        'Private _AdditionalInsuredsBackup As List(Of QuickQuoteAdditionalInsured)
        Private _EmployeeBenefitsLiabilityText As String
        Private _EmployeeBenefitsLiabilityOccurrenceLimit As String
        Private _EmployeeBenefitsLiabilityOccurrenceLimitId As String
        Private _EmployeeBenefitsLiabilityQuotedPremium As String
        Private _EmployeeBenefitsLiabilityRetroactiveDate As String
        Private _EmployeeBenefitsLiabilityAggregateLimit As String
        Private _EmployeeBenefitsLiabilityDeductible As String
        Private _HasElectronicData As Boolean
        Private _ElectronicDataLimit As String
        Private _ElectronicDataQuotedPremium As String
        Private _ContractorsEquipmentInstallationLimit As String
        Private _ContractorsEquipmentInstallationLimitId As String
        Private _ContractorsEquipmentInstallationLimitQuotedPremium As String
        Private _ContractorsToolsEquipmentBlanket As String
        Private _ContractorsToolsEquipmentBlanketSubLimitId As String
        Private _ContractorsToolsEquipmentBlanketQuotedPremium As String
        Private _ContractorsToolsEquipmentScheduled As String
        Private _ContractorsToolsEquipmentScheduledQuotedPremium As String
        Private _ContractorsToolsEquipmentRented As String
        Private _ContractorsToolsEquipmentRentedQuotedPremium As String
        Private _ContractorsEquipmentScheduledItems As Generic.List(Of QuickQuoteContractorsEquipmentScheduledItem)
        Private _ContractorsEquipmentScheduledItemsBackup As Generic.List(Of QuickQuoteContractorsEquipmentScheduledItem)
        Private _ContractorsEmployeeTools As String
        Private _ContractorsEmployeeToolsQuotedPremium As String
        Private _CrimeEmpDisEmployeeText As String
        Private _CrimeEmpDisLocationText As String
        Private _CrimeEmpDisLimit As String
        Private _CrimeEmpDisLimitId As String
        Private _CrimeEmpDisQuotedPremium As String
        Private _CrimeForgeryLimit As String
        Private _CrimeForgeryLimitId As String
        Private _CrimeForgeryQuotedPremium As String
        Private _HasEarthquake As Boolean
        Private _EarthquakeQuotedPremium As String
        Private _HasHiredAuto As Boolean
        Private _HiredAutoQuotedPremium As String
        Private _HasNonOwnedAuto As Boolean
        Private _NonOwnedAutoWithDelivery As Boolean
        Private _NonOwnedAutoQuotedPremium As String
        Private _PropertyDeductibleId As String
        Private _EmployersLiability As String
        Private _EmployersLiabilityId As String
        Private _EmployersLiabilityQuotedPremium As String
        Private _GeneralAggregateLimit As String
        Private _GeneralAggregateLimitId As String
        Private _GeneralAggregateQuotedPremium As String
        Private _ProductsCompletedOperationsAggregateLimit As String
        Private _ProductsCompletedOperationsAggregateLimitId As String
        Private _ProductsCompletedOperationsAggregateQuotedPremium As String
        Private _PersonalAndAdvertisingInjuryLimit As String
        Private _PersonalAndAdvertisingInjuryLimitId As String
        Private _PersonalAndAdvertisingInjuryQuotedPremium As String
        Private _DamageToPremisesRentedLimit As String
        Private _DamageToPremisesRentedLimitId As String
        Private _DamageToPremisesRentedQuotedPremium As String
        Private _MedicalExpensesLimit As String
        Private _MedicalExpensesLimitId As String
        Private _MedicalExpensesQuotedPremium As String
        'Private _HasExclusionOfAmishWorkers As Boolean
        'Private _HasExclusionOfSoleProprietorsPartnersOfficersAndOthers As Boolean
        'Private _HasInclusionOfSoleProprietorsPartnersOfficersAndOthers As Boolean
        'Private _HasWaiverOfSubrogation As Boolean
        'Private _WaiverOfSubrogationNumberOfWaivers As Integer
        'Private _WaiverOfSubrogationPremium As String
        'Private _WaiverOfSubrogationPremiumId As String
        'Private _NeedsToUpdateWaiverOfSubrogationPremiumId As Boolean
        'Private _ExclusionOfAmishWorkerRecords As Generic.List(Of QuickQuoteExclusionOfAmishWorkerRecord)
        'Private _ExclusionOfSoleProprietorRecords As Generic.List(Of QuickQuoteExclusionOfSoleProprietorRecord)
        'Private _InclusionOfSoleProprietorRecords As Generic.List(Of QuickQuoteInclusionOfSoleProprietorRecord)
        'Private _WaiverOfSubrogationRecords As Generic.List(Of QuickQuoteWaiverOfSubrogationRecord)
        'Private _ExclusionOfAmishWorkerRecordsBackup As Generic.List(Of QuickQuoteExclusionOfAmishWorkerRecord)
        'Private _ExclusionOfSoleProprietorRecordsBackup As Generic.List(Of QuickQuoteExclusionOfSoleProprietorRecord)
        'Private _InclusionOfSoleProprietorRecordsBackup As Generic.List(Of QuickQuoteInclusionOfSoleProprietorRecord)
        'Private _WaiverOfSubrogationRecordsBackup As Generic.List(Of QuickQuoteWaiverOfSubrogationRecord)
        '12/5/2018 - moved professional liability props to AlliedToIndividualState object... to match liquorLiability stuff, which was already moved
        'Private _HasBarbersProfessionalLiability As Boolean
        'Private _BarbersProfessionalLiabiltyQuotedPremium As String
        'Private _BarbersProfessionalLiabilityFullTimeEmpNum As String
        'Private _BarbersProfessionalLiabilityPartTimeEmpNum As String
        'Private _BarbersProfessionalLiabilityDescription As String
        'Private _HasBeauticiansProfessionalLiability As Boolean
        'Private _BeauticiansProfessionalLiabilityQuotedPremium As String
        'Private _BeauticiansProfessionalLiabilityFullTimeEmpNum As String
        'Private _BeauticiansProfessionalLiabilityPartTimeEmpNum As String
        'Private _BeauticiansProfessionalLiabilityDescription As String
        'Private _HasFuneralDirectorsProfessionalLiability As Boolean
        'Private _FuneralDirectorsProfessionalLiabilityQuotedPremium As String
        'Private _FuneralDirectorsProfessionalLiabilityEmpNum As String
        'Private _HasPrintersProfessionalLiability As Boolean
        'Private _PrintersProfessionalLiabilityQuotedPremium As String
        'Private _PrintersProfessionalLiabilityLocNum As String
        'Private _HasSelfStorageFacility As Boolean
        'Private _SelfStorageFacilityQuotedPremium As String
        'Private _SelfStorageFacilityLimit As String
        'Private _HasVeterinariansProfessionalLiability As Boolean
        'Private _VeterinariansProfessionalLiabilityEmpNum As String
        'Private _VeterinariansProfessionalLiabilityQuotedPremium As String
        'Private _HasPharmacistProfessionalLiability As Boolean
        'Private _PharmacistAnnualGrossSales As String
        'Private _PharmacistQuotedPremium As String
        'Private _HasOpticalAndHearingAidProfessionalLiability As Boolean
        'Private _OpticalAndHearingAidProfessionalLiabilityEmpNum As String
        'Private _OpticalAndHearingAidProfessionalLiabilityQuotedPremium As String
        'Private _HasMotelCoverage As Boolean
        'Private _MotelCoveragePerGuestLimitId As String
        'Private _MotelCoveragePerGuestLimit As String
        'Private _MotelCoveragePerGuestQuotedPremium As String
        'Private _MotelCoverageSafeDepositLimitId As String
        'Private _MotelCoverageSafeDepositDeductibleId As String
        'Private _MotelCoverageSafeDepositLimit As String
        'Private _MotelCoverageSafeDepositDeductible As String
        'Private _MotelCoverageQuotedPremium As String
        'Private _MotelCoverageSafeDepositQuotedPremium As String
        'Private _HasPhotographyCoverage As Boolean
        'Private _HasPhotographyCoverageScheduledCoverages As Boolean
        'Private _PhotographyScheduledCoverages As List(Of QuickQuoteCoverage)
        'Private _HasPhotographyMakeupAndHair As Boolean
        'Private _PhotographyMakeupAndHairQuotedPremium As String
        'Private _PhotographyCoverageQuotedPremium As String
        'Private _HasLiquorLiability As Boolean
        'Private _LiquorLiabilityClassCodeTypeId As String '12 = 58161 - Restaurant Includes Package Sales, 13 = 59211 - Package Sales for Consumption Off Premises
        'Private _LiquorLiabilityAnnualGrossPackageSalesReceipts As String
        'Private _LiquorLiabilityAnnualGrossAlcoholSalesReceipts As String
        'Private _HasResidentialCleaning As Boolean
        'Private _ResidentialCleaningQuotedPremium As String
        'Private _LiquorLiabilityOccurrenceLimit As String
        'Private _LiquorLiabilityOccurrenceLimitId As String
        'Private _LiquorLiabilityClassification As String
        'Private _LiquorLiabilityClassificationId As String
        'Private _LiquorSales As String
        'Private _LiquorLiabilityQuotedPremium As String
        'Private _ProfessionalLiabilityCemetaryNumberOfBurials As String
        'Private _ProfessionalLiabilityCemetaryQuotedPremium As String
        'Private _ProfessionalLiabilityFuneralDirectorsNumberOfBodies As String
        'Private _ProfessionalLiabilityPastoralNumberOfClergy As String
        'Private _ProfessionalLiabilityPastoralQuotedPremium As String
        Private _IRPM_ManagementCooperation As String
        Private _IRPM_ManagementCooperationDesc As String
        Private _IRPM_Location As String
        Private _IRPM_LocationDesc As String
        Private _IRPM_BuildingFeatures As String
        Private _IRPM_BuildingFeaturesDesc As String
        Private _IRPM_Premises As String
        Private _IRPM_PremisesDesc As String
        Private _IRPM_Employees As String
        Private _IRPM_EmployeesDesc As String
        Private _IRPM_Protection As String
        Private _IRPM_ProtectionDesc As String
        Private _IRPM_CatostrophicHazards As String
        Private _IRPM_CatostrophicHazardsDesc As String
        Private _IRPM_ManagementExperience As String
        Private _IRPM_ManagementExperienceDesc As String
        Private _IRPM_Equipment As String
        Private _IRPM_EquipmentDesc As String
        Private _IRPM_MedicalFacilities As String
        Private _IRPM_MedicalFacilitiesDesc As String
        Private _IRPM_ClassificationPeculiarities As String
        Private _IRPM_ClassificationPeculiaritiesDesc As String
        Private _IRPM_GL_ManagementCooperation As String
        Private _IRPM_GL_ManagementCooperationDesc As String
        Private _IRPM_GL_Location As String
        Private _IRPM_GL_LocationDesc As String
        Private _IRPM_GL_Premises As String
        Private _IRPM_GL_PremisesDesc As String
        Private _IRPM_GL_Equipment As String
        Private _IRPM_GL_EquipmentDesc As String
        Private _IRPM_GL_Employees As String
        Private _IRPM_GL_EmployeesDesc As String
        Private _IRPM_GL_ClassificationPeculiarities As String
        Private _IRPM_GL_ClassificationPeculiaritiesDesc As String
        Private _IRPM_CAP_Management As String
        Private _IRPM_CAP_ManagementDesc As String
        Private _IRPM_CAP_Employees As String
        Private _IRPM_CAP_EmployeesDesc As String
        Private _IRPM_CAP_Equipment As String
        Private _IRPM_CAP_EquipmentDesc As String
        Private _IRPM_CAP_SafetyOrganization As String
        Private _IRPM_CAP_SafetyOrganizationDesc As String

        Private _IRPM_CAP_Management_Phys_Damage As String
        Private _IRPM_CAP_ManagementDesc_Phys_Damage As String
        Private _IRPM_CAP_Employees_Phys_Damage As String
        Private _IRPM_CAP_EmployeesDesc_Phys_Damage As String
        Private _IRPM_CAP_Equipment_Phys_Damage As String
        Private _IRPM_CAP_EquipmentDesc_Phys_Damage As String
        Private _IRPM_CAP_SafetyOrganization_Phys_Damage As String
        Private _IRPM_CAP_SafetyOrganizationDesc_Phys_Damage As String

        Private _IRPM_CPR_Management As String
        Private _IRPM_CPR_ManagementDesc As String
        Private _IRPM_CPR_PremisesAndEquipment As String
        Private _IRPM_CPR_PremisesAndEquipmentDesc As String
        Private _IRPM_FAR_CareConditionOfEquipPremises As String
        Private _IRPM_FAR_CareConditionOfEquipPremisesDesc As String
        Private _IRPM_FAR_Cooperation As String
        Private _IRPM_FAR_CooperationDesc As String
        Private _IRPM_FAR_DamageSusceptibility As String
        Private _IRPM_FAR_DamageSusceptibilityDesc As String
        Private _IRPM_FAR_DispersionOrConcentration As String
        Private _IRPM_FAR_DispersionOrConcentrationDesc As String
        Private _IRPM_FAR_SuperiorOrInferiorStructureFeatures As String
        Private _IRPM_FAR_SuperiorOrInferiorStructureFeaturesDesc As String
        Private _IRPM_FAR_UseOfRiceHullsOrFlameRetardantBedding As String
        Private _IRPM_FAR_UseOfRiceHullsOrFlameRetardantBeddingDesc As String
        Private _IRPM_FAR_Location As String
        Private _IRPM_FAR_LocationDesc As String
        Private _IRPM_FAR_MiscProtectFeaturesOrHazards As String
        Private _IRPM_FAR_MiscProtectFeaturesOrHazardsDesc As String
        Private _IRPM_FAR_RoofCondition As String
        Private _IRPM_FAR_RoofConditionDesc As String
        Private _IRPM_FAR_StoragePracticesAndHazardousOperations As String
        Private _IRPM_FAR_StoragePracticesAndHazardousOperationsDesc As String
        Private _IRPM_FAR_PastLosses As String
        Private _IRPM_FAR_PastLossesDesc As String
        Private _IRPM_FAR_SupportingBusiness As String
        Private _IRPM_FAR_SupportingBusinessDesc As String
        Private _IRPM_FAR_RegularOnsiteInspections As String
        Private _IRPM_FAR_RegularOnsiteInspectionsDesc As String
        Private _GL_PremisesAndProducts_Deductible As String
        Private _GL_PremisesAndProducts_DeductibleId As String
        Private _GL_PremisesAndProducts_Description As String
        Private _GL_PremisesAndProducts_DeductibleCategoryType As String
        Private _GL_PremisesAndProducts_DeductibleCategoryTypeId As String
        Private _GL_PremisesAndProducts_DeductiblePerType As String
        Private _GL_PremisesAndProducts_DeductiblePerTypeId As String
        Private _Has_GL_PremisesAndProducts As Boolean
        Private _GL_PremisesTotalQuotedPremium As String
        Private _GL_ProductsTotalQuotedPremium As String
        Private _GL_PremisesPolicyLevelQuotedPremium As String
        Private _GL_ProductsPolicyLevelQuotedPremium As String
        Private _GL_PremisesMinimumQuotedPremium As String
        Private _GL_PremisesMinimumPremiumAdjustment As String
        Private _GL_ProductsMinimumQuotedPremium As String
        Private _GL_ProductsMinimumPremiumAdjustment As String
        Private _HasFarmPollutionLiability As Boolean
        Private _FarmPollutionLiabilityQuotedPremium As String
        Private _HasHiredBorrowedNonOwned As Boolean
        Private _HasNonOwnershipLiability As Boolean
        Private _NonOwnershipLiabilityNumberOfEmployees As String
        Private _NonOwnership_ENO_RatingTypeId As String
        Private _NonOwnership_ENO_RatingType As String
        Private _NonOwnershipLiabilityQuotedPremium As String
        Private _HasHiredBorrowedLiability As Boolean
        Private _HiredBorrowedLiabilityQuotedPremium As String
        Private _HasHiredCarPhysicalDamage As Boolean 'in HiredBorrowedLossOfUse section
        Private _HiredBorrowedLossOfUseQuotedPremium As String
        Private _ComprehensiveDeductible As String
        Private _ComprehensiveDeductibleId As String
        Private _ComprehensiveQuotedPremium As String
        Private _CollisionDeductible As String
        Private _CollisionDeductibleId As String
        Private _CollisionQuotedPremium As String
        Private _Liability_UM_UIM_Limit As String
        Private _Liability_UM_UIM_LimitId As String
        Private _Liability_UM_UIM_QuotedPremium As String
        Private _MedicalPaymentsLimit As String
        Private _MedicalPaymentsLimitId As String
        Private _MedicalPaymentsQuotedPremium As String
        Private _QuoteOrIssueBound As QuickQuoteObject.QuickQuoteQuoteOrIssueBound
        Private _IssueBoundEffectiveDate As String
        Private _LiabilityAutoSymbolObject As QuickQuoteDeveloperAutoSymbol
        Private _MedicalPaymentsAutoSymbolObject As QuickQuoteDeveloperAutoSymbol
        Private _UninsuredMotoristAutoSymbolObject As QuickQuoteDeveloperAutoSymbol
        Private _UnderinsuredMotoristAutoSymbolObject As QuickQuoteDeveloperAutoSymbol
        Private _ComprehensiveCoverageAutoSymbolObject As QuickQuoteDeveloperAutoSymbol
        Private _CollisionCoverageAutoSymbolObject As QuickQuoteDeveloperAutoSymbol
        Private _NonOwnershipAutoSymbolObject As QuickQuoteDeveloperAutoSymbol
        Private _HiredBorrowedAutoSymbolObject As QuickQuoteDeveloperAutoSymbol
        Private _UseDeveloperAutoSymbols As Boolean
        Private _TowingAndLaborAutoSymbolObject As QuickQuoteDeveloperAutoSymbol
        Private _CAP_Liability_WouldHaveSymbol8 As Boolean
        Private _CAP_Liability_WouldHaveSymbol9 As Boolean
        Private _CAP_Comprehensive_WouldHaveSymbol8 As Boolean
        Private _CAP_Collision_WouldHaveSymbol8 As Boolean
        Private _HasBlanketBuilding As Boolean
        Private _HasBlanketContents As Boolean
        Private _HasBlanketBuildingAndContents As Boolean
        Private _HasBlanketBusinessIncome As Boolean
        Private _BlanketBuildingQuotedPremium As String
        Private _BlanketContentsQuotedPremium As String
        Private _BlanketBuildingAndContentsQuotedPremium As String
        Private _BlanketBusinessIncomeQuotedPremium As String
        Private _BlanketBuildingCauseOfLossTypeId As String
        Private _BlanketBuildingCauseOfLossType As String
        Private _BlanketContentsCauseOfLossTypeId As String
        Private _BlanketContentsCauseOfLossType As String
        Private _BlanketBuildingAndContentsCauseOfLossTypeId As String
        Private _BlanketBuildingAndContentsCauseOfLossType As String
        Private _BlanketBusinessIncomeCauseOfLossTypeId As String
        Private _BlanketBusinessIncomeCauseOfLossType As String
        Private _BlanketBuildingLimit As String
        Private _BlanketBuildingCoinsuranceTypeId As String
        Private _BlanketBuildingCoinsuranceType As String
        Private _BlanketBuildingValuationId As String
        Private _BlanketBuildingValuation As String
        Private _BlanketBuildingDeductibleID As String
        Private _BlanketContentsLimit As String
        Private _BlanketContentsCoinsuranceTypeId As String
        Private _BlanketContentsCoinsuranceType As String
        Private _BlanketContentsValuationId As String
        Private _BlanketContentsValuation As String
        Private _BlanketContentsDeductibleID As String
        Private _BlanketBuildingAndContentsLimit As String
        Private _BlanketBuildingAndContentsCoinsuranceTypeId As String
        Private _BlanketBuildingAndContentsCoinsuranceType As String
        Private _BlanketBuildingAndContentsValuationId As String
        Private _BlanketBuildingAndContentsValuation As String
        Private _BlanketBuildingAndContentsDeductibleID As String
        Private _BlanketBusinessIncomeLimit As String
        Private _BlanketBusinessIncomeCoinsuranceTypeId As String
        Private _BlanketBusinessIncomeCoinsuranceType As String
        Private _BlanketBusinessIncomeValuationId As String
        Private _BlanketBusinessIncomeValuation As String
        Private _CPR_BlanketCoverages_TotalPremium As String
        Private _BlanketCombinedEarthquake_QuotedPremium As String
        Private _BlanketBuildingIsAgreedValue As Boolean
        Private _BlanketContentsIsAgreedValue As Boolean
        Private _BlanketBuildingAndContentsIsAgreedValue As Boolean
        Private _BlanketBusinessIncomeIsAgreedValue As Boolean
        Private _UseTierOverride As Boolean
        Private _TierAdjustmentTypeId As String
        Private _PersonalLiabilityLimitId As String
        Private _PersonalLiabilityQuotedPremium As String
        Private _HasEPLI As Boolean
        Private _EPLI_Applied As Boolean
        Private _EPLIPremium As String
        Private _EPLICoverageLimitId As String
        Private _EPLIDeductibleId As String
        Private _EPLICoverageTypeId As String

        Private _CyberLiability As Boolean
        Private _CyberLiabilityDeductible As String
        Private _CyberLiabilityDeductibleId As String
        Private _CyberLiabilityLimit As String
        Private _CyberLiabilityLimitId As String
        Private _CyberLiabilityPremium As String
        Private _CyberLiabilityTypeId As String
        Private _CyberLiabilityType As String

        'Private _BlanketWaiverOfSubrogation As String
        'Private _BlanketWaiverOfSubrogationQuotedPremium As String
        Private _HasCondoDandO As Boolean
        Private _CondoDandOAssociatedName As String
        Private _CondoDandODeductibleId As String
        Private _CondoDandOPremium As String
        Private _CondoDandOManualLimit As String
        Private _Farm_F_and_G_DeductibleLimitId As String 'static data
        Private _Farm_F_and_G_DeductibleQuotedPremium As String
        Private _HasFarmEquipmentBreakdown As Boolean
        Private _FarmEquipmentBreakdownQuotedPremium As String
        Private _HasFarmExtender As Boolean
        Private _FarmExtenderQuotedPremium As String
        Private _FarmAllStarLimitId As String 'static data
        Private _FarmAllStarQuotedPremium As String
        Private _HasFarmEmployersLiability As Boolean
        Private _FarmEmployersLiabilityQuotedPremium As String
        Private _FarmFireLegalLiabilityLimitId As String 'static data
        Private _FarmFireLegalLiabilityQuotedPremium As String
        Private _HasFarmPersonalAndAdvertisingInjury As Boolean
        Private _FarmPersonalAndAdvertisingInjuryQuotedPremium As String
        Private _FarmContractGrowersCareCustodyControlLimitId As String 'static data
        Private _FarmContractGrowersCareCustodyControlDescription As String
        Private _FarmContractGrowersCareCustodyControlQuotedPremium As String
        Private _HasFarmExclusionOfProductsCompletedWork As Boolean
        Private _FarmExclusionOfProductsCompletedWorkQuotedPremium As String
        Private _FarmIncidentalLimits As List(Of QuickQuoteFarmIncidentalLimit) 'goes w/ FarmIncidentalLimitCoverages
        Private _HasBusinessIncomeALS As Boolean
        Private _BusinessIncomeALSLimit As String
        Private _BusinessIncomeALSQuotedPremium As String
        Private _HasContractorsEnhancement As Boolean
        Private _ContractorsEnhancementQuotedPremium As String
        Private _CPP_CPR_ContractorsEnhancementQuotedPremium As String
        Private _CPP_CGL_ContractorsEnhancementQuotedPremium As String
        Private _CPP_CIM_ContractorsEnhancementQuotedPremium As String
        Private _HasManufacturersEnhancement As Boolean
        Private _ManufacturersEnhancementQuotedPremium As String
        Private _CPP_CPR_ManufacturersEnhancementQuotedPremium As String
        Private _CPP_CGL_ManufacturersEnhancementQuotedPremium As String
        Private _FarmMachinerySpecialCoverageG_QuotedPremium As String
        Private _HasAutoPlusEnhancement As Boolean
        Private _AutoPlusEnhancement_QuotedPremium As String
        '12/10/2018 - moved professional liability props to AlliedToIndividualState object... to match liquorLiability stuff and others, which were already moved
        'Private _HasApartmentBuildings As Boolean
        'Private _NumberOfLocationsWithApartments As String
        'Private _ApartmentQuotedPremium As String
        'Private _HasRestaurantEndorsement As Boolean
        'Private _RestaurantQuotedPremium As String
        Private _Liability_UM_UIM_AggregateLiabilityIncrementTypeId As String 'covDetail; covCodeId 21552
        Private _Liability_UM_UIM_DeductibleCategoryTypeId As String 'covDetail; covCodeId 21552
        Private _HasUninsuredMotoristPropertyDamage As Boolean 'covCodeId 21539
        Private _UninsuredMotoristPropertyDamageQuotedPremium As String 'covCodeId 21539; may not be populated
        Private _MedicalPaymentsTypeId As String 'covDetail; covCodeId 21540
        Private _HasPhysicalDamageOtherThanCollision As Boolean 'covCodeId 21550
        Private _PhysicalDamageOtherThanCollisionQuotedPremium As String 'covCodeId 21550; may not be populated
        Private _HasPhysicalDamageCollision As Boolean 'covCodeId 21551
        Private _PhysicalDamageCollisionQuotedPremium As String 'covCodeId 21551; may not be populated
        Private _PhysicalDamageCollisionDeductibleId As String 'covCodeId 21551
        Private _HasGarageKeepersOtherThanCollision As Boolean 'covCodeId 21541
        Private _GarageKeepersOtherThanCollisionQuotedPremium As String 'covCodeId 21541
        Private _HasGarageKeepersCollision As Boolean 'covCodeId 21542
        Private _GarageKeepersCollisionQuotedPremium As String 'covCodeId 21542
        Private _MultiLineDiscountValue As String
        Private _HasAdvancedQuoteDiscount As Boolean
        Private _HasFarmIndicator As Boolean
        Private _PriorBodilyInjuryLimitId As String

        'added 9/25/2018 for multi-state
        Private _Liability_UM_UIM_DeductibleId As String 'covCodeId 21552
        Private _UninsuredMotoristPropertyDamageLimitId As String 'covCodeId 21539; note: same prop exists on Vehicle
        Private _UninsuredMotoristPropertyDamageDeductibleId As String 'covCodeId 21539
        Private _UnderinsuredMotoristBodilyInjuryLiabilityLimitId As String 'covCodeId 21548
        Private _UnderinsuredMotoristBodilyInjuryLiabilityQuotedPremium As String 'covCodeId 21548

        'added 7/9/2021
        Private _HasFoodManufacturersEnhancement As Boolean 'covCodeId 100000
        Private _FoodManufacturersEnhancementQuotedPremium As String 'covCodeId 100000
        Private _CPP_CPR_FoodManufacturersEnhancementQuotedPremium As String 'covCodeId 100000
        Private _CPP_CGL_FoodManufacturersEnhancementQuotedPremium As String 'covCodeId 100000

        Private _QuoteEffectiveDate As String
        Private _QuoteTransactionType As QuickQuoteObject.QuickQuoteTransactionType
        Private _LobType As QuickQuoteObject.QuickQuoteLobType

        Private _HasFamilyCyberProtection As Boolean 'covCodeId 80572
        Private _FamilyCyberProtectionQuotedPremium As String

        'Added 6/27/2022 for task 75780 MLW
        Private _Has_PackageCPR_PlusEnhancementEndorsement As Boolean 'covCodeId 100009
        Private _PackageCPR_PlusEnhancementEndorsementQuotedPremium As String

        Private _BasePolicyLevelInfo As QuickQuoteLobPolicyLevelInfo_AppliedToAllStates

        Private _HasFarmAllStar As Boolean 'covCodeId 80125
        Private _FarmAllStarWaterBackupLimitId As String 'covCodeId 144
        Private _FarmAllStarWaterDamageLimitId As String 'covCodeId 80520
        Private _OwnersLesseesorContractorsCompletedOperationsTotalPremium As String 'covCodeId 80446


        'PolicyLevel
        Public Property CPP_CRM_ProgramTypeId As String
            Get
                Return _CPP_CRM_ProgramTypeId
            End Get
            Set(value As String)
                _CPP_CRM_ProgramTypeId = value
            End Set
        End Property
        Public Property CPP_GAR_ProgramTypeId As String
            Get
                Return _CPP_GAR_ProgramTypeId
            End Get
            Set(value As String)
                _CPP_GAR_ProgramTypeId = value
            End Set
        End Property
        Public ReadOnly Property RiskGradeLookupId_Original As String
            Get
                Return _RiskGradeLookupId_Original
            End Get
        End Property
        Protected Friend Sub Set_RiskGradeLookupId_Original(ByVal rgLookupIdOrig As String)
            _RiskGradeLookupId_Original = rgLookupIdOrig
        End Sub
        Public Property CPP_CGL_RiskGrade As String
            Get
                Return _CPP_CGL_RiskGrade
            End Get
            Set(value As String)
                _CPP_CGL_RiskGrade = value
            End Set
        End Property
        Public Property CPP_CGL_RiskGradeLookupId As String
            Get
                Return _CPP_CGL_RiskGradeLookupId
            End Get
            Set(value As String)
                _CPP_CGL_RiskGradeLookupId = value
            End Set
        End Property
        Public Property CPP_CPR_RiskGrade As String
            Get
                Return _CPP_CPR_RiskGrade
            End Get
            Set(value As String)
                _CPP_CPR_RiskGrade = value
            End Set
        End Property
        Public Property CPP_CPR_RiskGradeLookupId As String
            Get
                Return _CPP_CPR_RiskGradeLookupId
            End Get
            Set(value As String)
                _CPP_CPR_RiskGradeLookupId = value
            End Set
        End Property
        Public Property ErrorRiskGradeLookupId As String
            Get
                Return _ErrorRiskGradeLookupId
            End Get
            Set(value As String)
                _ErrorRiskGradeLookupId = value
            End Set
        End Property
        Public Property ReplacementRiskGradeLookupId As String
            Get
                Return _ReplacementRiskGradeLookupId
            End Get
            Set(value As String)
                _ReplacementRiskGradeLookupId = value
            End Set
        End Property
        Public Property CPP_CGL_ErrorRiskGradeLookupId As String
            Get
                Return _CPP_CGL_ErrorRiskGradeLookupId
            End Get
            Set(value As String)
                _CPP_CGL_ErrorRiskGradeLookupId = value
            End Set
        End Property
        Public Property CPP_CGL_ReplacementRiskGradeLookupId As String
            Get
                Return _CPP_CGL_ReplacementRiskGradeLookupId
            End Get
            Set(value As String)
                _CPP_CGL_ReplacementRiskGradeLookupId = value
            End Set
        End Property
        Public Property CPP_CPR_ErrorRiskGradeLookupId As String
            Get
                Return _CPP_CPR_ErrorRiskGradeLookupId
            End Get
            Set(value As String)
                _CPP_CPR_ErrorRiskGradeLookupId = value
            End Set
        End Property
        Public Property CPP_CPR_ReplacementRiskGradeLookupId As String
            Get
                Return _CPP_CPR_ReplacementRiskGradeLookupId
            End Get
            Set(value As String)
                _CPP_CPR_ReplacementRiskGradeLookupId = value
            End Set
        End Property
        Public Property CPP_CIM_RiskGrade As String
            Get
                Return _CPP_CIM_RiskGrade
            End Get
            Set(value As String)
                _CPP_CIM_RiskGrade = value
            End Set
        End Property
        Public Property CPP_CIM_RiskGradeLookupId As String
            Get
                Return _CPP_CIM_RiskGradeLookupId
            End Get
            Set(value As String)
                _CPP_CIM_RiskGradeLookupId = value
            End Set
        End Property
        Public Property CPP_CIM_ErrorRiskGradeLookupId As String
            Get
                Return _CPP_CIM_ErrorRiskGradeLookupId
            End Get
            Set(value As String)
                _CPP_CIM_ErrorRiskGradeLookupId = value
            End Set
        End Property
        Public Property CPP_CIM_ReplacementRiskGradeLookupId As String
            Get
                Return _CPP_CIM_ReplacementRiskGradeLookupId
            End Get
            Set(value As String)
                _CPP_CIM_ReplacementRiskGradeLookupId = value
            End Set
        End Property
        Public Property CPP_CRM_RiskGrade As String
            Get
                Return _CPP_CRM_RiskGrade
            End Get
            Set(value As String)
                _CPP_CRM_RiskGrade = value
            End Set
        End Property
        Public Property CPP_CRM_RiskGradeLookupId As String
            Get
                Return _CPP_CRM_RiskGradeLookupId
            End Get
            Set(value As String)
                _CPP_CRM_RiskGradeLookupId = value
            End Set
        End Property
        Public Property CPP_CRM_ErrorRiskGradeLookupId As String
            Get
                Return _CPP_CRM_ErrorRiskGradeLookupId
            End Get
            Set(value As String)
                _CPP_CRM_ErrorRiskGradeLookupId = value
            End Set
        End Property
        Public Property CPP_CRM_ReplacementRiskGradeLookupId As String
            Get
                Return _CPP_CRM_ReplacementRiskGradeLookupId
            End Get
            Set(value As String)
                _CPP_CRM_ReplacementRiskGradeLookupId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 70017 (BOP) or 80154 (CGL)</remarks>
        Public Property OccurrenceLiabilityLimit As String
            Get
                Return _OccurrenceLiabilityLimit
            End Get
            Set(value As String)
                _OccurrenceLiabilityLimit = value
                Select Case _OccurrenceLiabilityLimit
                    Case "300,000"
                        _OccurrenceLiabilityLimitId = "33"
                    Case "500,000"
                        _OccurrenceLiabilityLimitId = "34"
                    Case "1,000,000"
                        _OccurrenceLiabilityLimitId = "56"
                    Case "25,000"
                        _OccurrenceLiabilityLimitId = "8"
                    Case "50,000"
                        _OccurrenceLiabilityLimitId = "9"
                    Case "100,000"
                        _OccurrenceLiabilityLimitId = "10"
                    Case "200,000"
                        _OccurrenceLiabilityLimitId = "32"
                    Case Else
                        _OccurrenceLiabilityLimitId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Target Market (CPP)</remarks>
        Public Property CPP_TargetMarketID As String
            Get
                Return _CPP_TargetMarketID
            End Get
            Set(value As String)
                _CPP_TargetMarketID = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 70017 (BOP) or 80154 (CGL)</remarks>
        Public Property OccurrenceLiabilityLimitId As String
            Get
                Return _OccurrenceLiabilityLimitId
            End Get
            Set(value As String)
                _OccurrenceLiabilityLimitId = value
                '(33=300,000; 34=500,000; 56=1,000,000)
                _OccurrenceLiabilityLimit = ""
                If IsNumeric(_OccurrenceLiabilityLimitId) = True Then
                    Select Case _OccurrenceLiabilityLimitId
                        Case "33"
                            _OccurrenceLiabilityLimit = "300,000"
                        Case "34"
                            _OccurrenceLiabilityLimit = "500,000"
                        Case "56"
                            _OccurrenceLiabilityLimit = "1,000,000"
                        Case "8"
                            _OccurrenceLiabilityLimit = "25,000"
                        Case "9"
                            _OccurrenceLiabilityLimit = "50,000"
                        Case "10"
                            _OccurrenceLiabilityLimit = "100,000"
                        Case "32"
                            _OccurrenceLiabilityLimit = "200,000"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 70017 (BOP) or 80154 (CGL)</remarks>
        Public Property OccurrencyLiabilityQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_OccurrencyLiabilityQuotedPremium)
            End Get
            Set(value As String)
                _OccurrencyLiabilityQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_OccurrencyLiabilityQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80144</remarks>
        Public Property TenantsFireLiability As String
            Get
                Return _TenantsFireLiability
            End Get
            Set(value As String)
                _TenantsFireLiability = value
                Select Case _TenantsFireLiability
                    Case "50,000"
                        _TenantsFireLiabilityId = "9"
                    Case "100,000"
                        _TenantsFireLiabilityId = "10"
                    Case "250,000"
                        _TenantsFireLiabilityId = "55"
                    Case "500,000"
                        _TenantsFireLiabilityId = "34"
                    Case "1,000,000"
                        _TenantsFireLiabilityId = "56"
                    Case Else
                        _TenantsFireLiabilityId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80144</remarks>
        Public Property TenantsFireLiabilityId As String
            Get
                Return _TenantsFireLiabilityId
            End Get
            Set(value As String)
                _TenantsFireLiabilityId = value
                '(9=50,000; 10=100,000; 55=250,000; 34=500,000; 56=1,000,000)
                _TenantsFireLiability = ""
                If IsNumeric(_TenantsFireLiabilityId) = True Then
                    Select Case _TenantsFireLiabilityId
                        Case "9"
                            _TenantsFireLiability = "50,000"
                        Case "10"
                            _TenantsFireLiability = "100,000"
                        Case "55"
                            _TenantsFireLiability = "250,000"
                        Case "34"
                            _TenantsFireLiability = "500,000"
                        Case "56"
                            _TenantsFireLiability = "1,000,000"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80144</remarks>
        Public Property TenantsFireLiabilityQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_TenantsFireLiabilityQuotedPremium)
            End Get
            Set(value As String)
                _TenantsFireLiabilityQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_TenantsFireLiabilityQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80146</remarks>
        Public Property PropertyDamageLiabilityDeductible As String
            Get
                Return _PropertyDamageLiabilityDeductible
            End Get
            Set(value As String)
                _PropertyDamageLiabilityDeductible = value
                Select Case _PropertyDamageLiabilityDeductible
                    Case "N/A"
                        _PropertyDamageLiabilityDeductibleId = "0"
                    Case "250"
                        _PropertyDamageLiabilityDeductibleId = "21"
                    Case "500"
                        _PropertyDamageLiabilityDeductibleId = "22"
                    Case "1000"
                        _PropertyDamageLiabilityDeductibleId = "24"
                    Case "2500"
                        _PropertyDamageLiabilityDeductibleId = "75"
                    Case Else
                        _PropertyDamageLiabilityDeductibleId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80146</remarks>
        Public Property PropertyDamageLiabilityDeductibleId As String
            Get
                Return _PropertyDamageLiabilityDeductibleId
            End Get
            Set(value As String)
                _PropertyDamageLiabilityDeductibleId = value
                '(0=N/A; 21=250; 22=500; 24=1000; 75=2500)
                _PropertyDamageLiabilityDeductible = ""
                If IsNumeric(_PropertyDamageLiabilityDeductibleId) = True Then
                    Select Case _PropertyDamageLiabilityDeductibleId
                        Case "0"
                            _PropertyDamageLiabilityDeductible = "N/A"
                        Case "21"
                            _PropertyDamageLiabilityDeductible = "250"
                        Case "22"
                            _PropertyDamageLiabilityDeductible = "500"
                        Case "24"
                            _PropertyDamageLiabilityDeductible = "1000"
                        Case "75"
                            _PropertyDamageLiabilityDeductible = "2500"
                    End Select
                End If
            End Set
        End Property
        Public Property BlanketRatingQuotedPremium As String '*not sure where to find (specs show 21085)
            Get
                Return qqHelper.QuotedPremiumFormat(_BlanketRatingQuotedPremium)
            End Get
            Set(value As String)
                _BlanketRatingQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_BlanketRatingQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 286 or 80094 (PPA); property covers Enhancement Endorsement for all LOBs</remarks>
        Public Property HasEnhancementEndorsement As Boolean
            Get
                Return _HasEnhancementEndorsement
            End Get
            Set(value As Boolean)
                _HasEnhancementEndorsement = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 286 or 80094 (PPA); property covers Enhancement Endorsement for all LOBs</remarks>
        Public Property EnhancementEndorsementQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_EnhancementEndorsementQuotedPremium)
            End Get
            Set(value As String)
                _EnhancementEndorsementQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_EnhancementEndorsementQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 286; specific to the GL package part for CPP</remarks>
        Public Property Has_PackageGL_EnhancementEndorsement As Boolean
            Get
                Return _Has_PackageGL_EnhancementEndorsement
            End Get
            Set(value As Boolean)
                _Has_PackageGL_EnhancementEndorsement = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 286; specific to the GL package part for CPP</remarks>
        Public Property PackageGL_EnhancementEndorsementQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_PackageGL_EnhancementEndorsementQuotedPremium)
            End Get
            Set(value As String)
                _PackageGL_EnhancementEndorsementQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_PackageGL_EnhancementEndorsementQuotedPremium)
            End Set
        End Property

        Public Property Has_PackageGL_PlusEnhancementEndorsement As Boolean
            Get
                Return _Has_PackageGL_PlusEnhancementEndorsement
            End Get
            Set(value As Boolean)
                _Has_PackageGL_PlusEnhancementEndorsement = value
            End Set
        End Property
        Public Property PackageGL_PlusEnhancementEndorsementQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_PackageGL_PlusEnhancementEndorsementQuotedPremium)
            End Get
            Set(value As String)
                _PackageGL_PlusEnhancementEndorsementQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_PackageGL_PlusEnhancementEndorsementQuotedPremium)
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 286; specific to the CPR package part for CPP</remarks>
        Public Property Has_PackageCPR_EnhancementEndorsement As Boolean
            Get
                Return _Has_PackageCPR_EnhancementEndorsement
            End Get
            Set(value As Boolean)
                _Has_PackageCPR_EnhancementEndorsement = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 286; specific to the CPR package part for CPP</remarks>
        Public Property PackageCPR_EnhancementEndorsementQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_PackageCPR_EnhancementEndorsementQuotedPremium)
            End Get
            Set(value As String)
                _PackageCPR_EnhancementEndorsementQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_PackageCPR_EnhancementEndorsementQuotedPremium)
            End Set
        End Property
        'Added 6/27/2022 for task 75780 MLW
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 100009; specific to the CPR package part for CPP</remarks>
        Public Property Has_PackageCPR_PlusEnhancementEndorsement As Boolean
            Get
                Return _Has_PackageCPR_PlusEnhancementEndorsement
            End Get
            Set(value As Boolean)
                _Has_PackageCPR_PlusEnhancementEndorsement = value
            End Set
        End Property
        'Added 6/27/2022 for task 75780 MLW
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 100009; specific to the CPR package part for CPP</remarks>
        Public Property PackageCPR_PlusEnhancementEndorsementQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_PackageCPR_PlusEnhancementEndorsementQuotedPremium)
            End Get
            Set(value As String)
                _PackageCPR_PlusEnhancementEndorsementQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_PackageCPR_PlusEnhancementEndorsementQuotedPremium)
            End Set
        End Property
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
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 185 (this property is specific to NumberOfEmployees)</remarks>
        Public Property EmployeeBenefitsLiabilityText As String
            Get
                Return _EmployeeBenefitsLiabilityText
            End Get
            Set(value As String)
                _EmployeeBenefitsLiabilityText = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 185</remarks>
        Public Property EmployeeBenefitsLiabilityOccurrenceLimit As String
            Get
                Return _EmployeeBenefitsLiabilityOccurrenceLimit
            End Get
            Set(value As String)
                _EmployeeBenefitsLiabilityOccurrenceLimit = value
                Select Case _EmployeeBenefitsLiabilityOccurrenceLimit
                    Case "N/A"
                        _EmployeeBenefitsLiabilityOccurrenceLimitId = "0"
                    Case "300,000"
                        _EmployeeBenefitsLiabilityOccurrenceLimitId = "33"
                    Case "500,000"
                        _EmployeeBenefitsLiabilityOccurrenceLimitId = "34"
                    Case "1,000,000"
                        _EmployeeBenefitsLiabilityOccurrenceLimitId = "56"
                    Case Else
                        _EmployeeBenefitsLiabilityOccurrenceLimitId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 185</remarks>
        Public Property EmployeeBenefitsLiabilityOccurrenceLimitId As String
            Get
                Return _EmployeeBenefitsLiabilityOccurrenceLimitId
            End Get
            Set(value As String)
                _EmployeeBenefitsLiabilityOccurrenceLimitId = value
                _EmployeeBenefitsLiabilityOccurrenceLimit = ""
                If IsNumeric(_EmployeeBenefitsLiabilityOccurrenceLimitId) = True Then
                    Select Case _EmployeeBenefitsLiabilityOccurrenceLimitId
                        Case "0"
                            _EmployeeBenefitsLiabilityOccurrenceLimit = "N/A"
                        Case "33"
                            _EmployeeBenefitsLiabilityOccurrenceLimit = "300,000"
                        Case "34"
                            _EmployeeBenefitsLiabilityOccurrenceLimit = "500,000"
                        Case "56"
                            _EmployeeBenefitsLiabilityOccurrenceLimit = "1,000,000"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 185</remarks>
        Public Property EmployeeBenefitsLiabilityQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_EmployeeBenefitsLiabilityQuotedPremium)
            End Get
            Set(value As String)
                _EmployeeBenefitsLiabilityQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_EmployeeBenefitsLiabilityQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 185</remarks>
        Public Property EmployeeBenefitsLiabilityRetroactiveDate As String
            Get
                Return _EmployeeBenefitsLiabilityRetroactiveDate
            End Get
            Set(value As String)
                _EmployeeBenefitsLiabilityRetroactiveDate = value
                qqHelper.ConvertToShortDate(_EmployeeBenefitsLiabilityRetroactiveDate)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 185</remarks>
        Public Property EmployeeBenefitsLiabilityAggregateLimit As String
            Get
                Return _EmployeeBenefitsLiabilityAggregateLimit
            End Get
            Set(value As String)
                _EmployeeBenefitsLiabilityAggregateLimit = value 'might need limit formatting
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 185</remarks>
        Public Property EmployeeBenefitsLiabilityDeductible As String
            Get
                Return _EmployeeBenefitsLiabilityDeductible
            End Get
            Set(value As String)
                _EmployeeBenefitsLiabilityDeductible = value 'might need limit formatting
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 10132</remarks>
        Public Property HasElectronicData As Boolean
            Get
                Return _HasElectronicData
            End Get
            Set(value As Boolean)
                _HasElectronicData = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 10132</remarks>
        Public Property ElectronicDataLimit As String
            Get
                Return _ElectronicDataLimit
            End Get
            Set(value As String)
                _ElectronicDataLimit = value
                qqHelper.ConvertToLimitFormat(_ElectronicDataLimit)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 10132</remarks>
        Public Property ElectronicDataQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_ElectronicDataQuotedPremium)
            End Get
            Set(value As String)
                _ElectronicDataQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_ElectronicDataQuotedPremium)
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21004</remarks>
        Public Property ContractorsEquipmentInstallationLimit As String
            Get
                Return _ContractorsEquipmentInstallationLimit
            End Get
            Set(value As String)
                _ContractorsEquipmentInstallationLimit = value
                Select Case _ContractorsEquipmentInstallationLimit
                    Case "5,000"
                        _ContractorsEquipmentInstallationLimitId = "15"
                    Case "10,000"
                        _ContractorsEquipmentInstallationLimitId = "7"
                    Case "15,000"
                        _ContractorsEquipmentInstallationLimitId = "48"
                    Case "20,000"
                        _ContractorsEquipmentInstallationLimitId = "49"
                    Case "25,000"
                        _ContractorsEquipmentInstallationLimitId = "8"
                    Case Else
                        _ContractorsEquipmentInstallationLimitId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21004</remarks>
        Public Property ContractorsEquipmentInstallationLimitId As String
            Get
                Return _ContractorsEquipmentInstallationLimitId
            End Get
            Set(value As String)
                _ContractorsEquipmentInstallationLimitId = value
                If IsNumeric(_ContractorsEquipmentInstallationLimitId) = True Then
                    Select Case _ContractorsEquipmentInstallationLimitId
                        Case "15"
                            _ContractorsEquipmentInstallationLimit = "5,000"
                        Case "7"
                            _ContractorsEquipmentInstallationLimit = "10,000"
                        Case "48"
                            _ContractorsEquipmentInstallationLimit = "15,000"
                        Case "49"
                            _ContractorsEquipmentInstallationLimit = "20,000"
                        Case "8"
                            _ContractorsEquipmentInstallationLimit = "25,000"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21004</remarks>
        Public Property ContractorsEquipmentInstallationLimitQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_ContractorsEquipmentInstallationLimitQuotedPremium)
            End Get
            Set(value As String)
                _ContractorsEquipmentInstallationLimitQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_ContractorsEquipmentInstallationLimitQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21007</remarks>
        Public Property ContractorsToolsEquipmentBlanket As String
            Get
                Return _ContractorsToolsEquipmentBlanket
            End Get
            Set(value As String)
                _ContractorsToolsEquipmentBlanket = value
                qqHelper.ConvertToLimitFormat(_ContractorsToolsEquipmentBlanket)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21007</remarks>
        Public Property ContractorsToolsEquipmentBlanketSubLimitId As String
            Get
                Return _ContractorsToolsEquipmentBlanketSubLimitId
            End Get
            Set(value As String)
                _ContractorsToolsEquipmentBlanketSubLimitId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21007</remarks>
        Public Property ContractorsToolsEquipmentBlanketQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_ContractorsToolsEquipmentBlanketQuotedPremium)
            End Get
            Set(value As String)
                _ContractorsToolsEquipmentBlanketQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_ContractorsToolsEquipmentBlanketQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21008</remarks>
        Public Property ContractorsToolsEquipmentScheduled As String
            Get
                If QuickQuoteHelperClass.UseComparativeRaterForLob(lob:=_LobType, quoteTransactionType:=QuoteTransactionType) <> QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then 'using <> Yes to also allow on Maybe
                    'note: will just return empty string if 0
                    Dim ceTotal As String = qqHelper.ContractorsEquipmentScheduledItemsTotalLimitFromList(_ContractorsEquipmentScheduledItems, returnInLimitFormat:=True)
                    If qqHelper.IsZeroAmount(ceTotal) = True Then
                        ceTotal = ""
                    End If
                    Return ceTotal
                Else
                    Return _ContractorsToolsEquipmentScheduled
                End If
            End Get
            Set(value As String)
                If QuickQuoteHelperClass.UseComparativeRaterForLob(lob:=_LobType, quoteTransactionType:=QuoteTransactionType) <> QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then 'using <> Yes to also allow on Maybe
                    qqHelper.UpdateContractorsEquipmentScheduledItemsListBasedOnTotalLimit(value, _ContractorsEquipmentScheduledItems, contractorsEquipScheduledItemsBackup:=_ContractorsEquipmentScheduledItemsBackup, updateBackupListBeforeRemoving:=True, maintainOneItemFromOriginalListWhenResetting:=False, maintainFirstOrLastItem:=QuickQuoteHelperClass.FirstOrLast.First, maintainItemsOnUnresolvedDifference:=False, applicableItemToApplyDifferenceTo:=QuickQuoteHelperClass.FirstLastOrAll.All, firstOrLastItemOrderWhenApplyingDifferenceToAll:=QuickQuoteHelperClass.FirstOrLast.First, treatAmountsAsIntegerOverDecimal:=True)
                Else
                    _ContractorsToolsEquipmentScheduled = value
                    qqHelper.ConvertToLimitFormat(_ContractorsToolsEquipmentScheduled)
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21008</remarks>
        Public Property ContractorsToolsEquipmentScheduledQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_ContractorsToolsEquipmentScheduledQuotedPremium)
            End Get
            Set(value As String)
                _ContractorsToolsEquipmentScheduledQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_ContractorsToolsEquipmentScheduledQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21005</remarks> '4/1/2015 note: removed ' (would also use 21421 if present in response xml)'
        Public Property ContractorsToolsEquipmentRented As String
            Get
                Return _ContractorsToolsEquipmentRented
            End Get
            Set(value As String)
                _ContractorsToolsEquipmentRented = value
                qqHelper.ConvertToLimitFormat(_ContractorsToolsEquipmentRented)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21005</remarks> '4/1/2015 note: removed ' (would also use 21421 if present in response xml)'
        Public Property ContractorsToolsEquipmentRentedQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_ContractorsToolsEquipmentRentedQuotedPremium)
            End Get
            Set(value As String)
                _ContractorsToolsEquipmentRentedQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_ContractorsToolsEquipmentRentedQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21008</remarks>
        Public Property ContractorsEquipmentScheduledItems As Generic.List(Of QuickQuoteContractorsEquipmentScheduledItem)
            Get
                Return _ContractorsEquipmentScheduledItems
            End Get
            Set(value As Generic.List(Of QuickQuoteContractorsEquipmentScheduledItem))
                _ContractorsEquipmentScheduledItems = value
            End Set
        End Property
        Public ReadOnly Property ContractorsEquipmentScheduledItemsBackup As Generic.List(Of QuickQuoteContractorsEquipmentScheduledItem)
            Get
                Return _ContractorsEquipmentScheduledItemsBackup
            End Get
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21006</remarks> '4/1/2015 note: removed ' (would also use 80225 if present in response xml)'
        Public Property ContractorsEmployeeTools As String
            Get
                Return _ContractorsEmployeeTools
            End Get
            Set(value As String)
                _ContractorsEmployeeTools = value
                qqHelper.ConvertToLimitFormat(_ContractorsEmployeeTools)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21006</remarks> '4/1/2015 note: removed ' (would also use 80225 if present in response xml)'
        Public Property ContractorsEmployeeToolsQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_ContractorsEmployeeToolsQuotedPremium)
            End Get
            Set(value As String)
                _ContractorsEmployeeToolsQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_ContractorsEmployeeToolsQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21009 (this property is specific to NumberOfEmployees)</remarks>
        Public Property CrimeEmpDisEmployeeText As String
            Get
                Return _CrimeEmpDisEmployeeText
            End Get
            Set(value As String)
                _CrimeEmpDisEmployeeText = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21009 (this property is specific to NumberOfLocations)</remarks>
        Public Property CrimeEmpDisLocationText As String
            Get
                Return _CrimeEmpDisLocationText
            End Get
            Set(value As String)
                _CrimeEmpDisLocationText = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21009</remarks>
        Public Property CrimeEmpDisLimit As String
            Get
                Return _CrimeEmpDisLimit
            End Get
            Set(value As String)
                _CrimeEmpDisLimit = value
                Select Case _CrimeEmpDisLimit
                    Case "5,000"
                        _CrimeEmpDisLimitId = "15"
                    Case "10,000"
                        _CrimeEmpDisLimitId = "7"
                    Case "25,000"
                        _CrimeEmpDisLimitId = "8"
                    Case "50,000"
                        _CrimeEmpDisLimitId = "9"
                    Case "100,000"
                        _CrimeEmpDisLimitId = "10"
                    Case Else
                        _CrimeEmpDisLimitId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21009</remarks>
        Public Property CrimeEmpDisLimitId As String 'verified in database 7/3/2012
            Get
                Return _CrimeEmpDisLimitId
            End Get
            Set(value As String)
                _CrimeEmpDisLimitId = value
                '(15=5,000; 7=10,000; 8=25,000; 9=50,000; 10=100,000)
                _CrimeEmpDisLimit = ""
                If IsNumeric(_CrimeEmpDisLimitId) = True Then
                    Select Case _CrimeEmpDisLimitId
                        Case "15"
                            _CrimeEmpDisLimit = "5,000"
                        Case "7"
                            _CrimeEmpDisLimit = "10,000"
                        Case "8"
                            _CrimeEmpDisLimit = "25,000"
                        Case "9"
                            _CrimeEmpDisLimit = "50,000"
                        Case "10"
                            _CrimeEmpDisLimit = "100,000"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21009</remarks>
        Public Property CrimeEmpDisQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_CrimeEmpDisQuotedPremium)
            End Get
            Set(value As String)
                _CrimeEmpDisQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_CrimeEmpDisQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21010</remarks>
        Public Property CrimeForgeryLimit As String
            Get
                Return _CrimeForgeryLimit
            End Get
            Set(value As String)
                _CrimeForgeryLimit = value
                Select Case _CrimeForgeryLimit
                    Case "5,000"
                        _CrimeForgeryLimitId = "15"
                    Case "10,000"
                        _CrimeForgeryLimitId = "7"
                    Case "25,000"
                        _CrimeForgeryLimitId = "8"
                    Case "50,000"
                        _CrimeForgeryLimitId = "9"
                    Case "100,000"
                        _CrimeForgeryLimitId = "10"
                    Case Else
                        _CrimeForgeryLimitId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21010</remarks>
        Public Property CrimeForgeryLimitId As String
            Get
                Return _CrimeForgeryLimitId
            End Get
            Set(value As String)
                _CrimeForgeryLimitId = value
                '(15=5,000; 7=10,000; 8=25,000; 9=50,000; 10=100,000)
                _CrimeForgeryLimit = ""
                If IsNumeric(_CrimeForgeryLimitId) = True Then
                    Select Case _CrimeForgeryLimitId
                        Case "15"
                            _CrimeForgeryLimit = "5,000"
                        Case "7"
                            _CrimeForgeryLimit = "10,000"
                        Case "8"
                            _CrimeForgeryLimit = "25,000"
                        Case "9"
                            _CrimeForgeryLimit = "50,000"
                        Case "10"
                            _CrimeForgeryLimit = "100,000"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21010</remarks>
        Public Property CrimeForgeryQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_CrimeForgeryQuotedPremium)
            End Get
            Set(value As String)
                _CrimeForgeryQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_CrimeForgeryQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 309</remarks>
        Public Property HasEarthquake As Boolean
            Get
                Return _HasEarthquake
            End Get
            Set(value As Boolean)
                _HasEarthquake = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 309</remarks>
        Public Property EarthquakeQuotedPremium As String
            Get
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
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21029</remarks>
        Public Property HasHiredAuto As Boolean
            Get
                Return _HasHiredAuto
            End Get
            Set(value As Boolean)
                _HasHiredAuto = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21029</remarks>
        Public Property HiredAutoQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_HiredAutoQuotedPremium)
            End Get
            Set(value As String)
                _HiredAutoQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_HiredAutoQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21030</remarks>
        Public Property HasNonOwnedAuto As Boolean
            Get
                Return _HasNonOwnedAuto
            End Get
            Set(value As Boolean)
                _HasNonOwnedAuto = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21030</remarks>
        Public Property NonOwnedAutoWithDelivery As Boolean
            Get
                Return _NonOwnedAutoWithDelivery
            End Get
            Set(value As Boolean)
                _NonOwnedAutoWithDelivery = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21030</remarks>
        Public Property NonOwnedAutoQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_NonOwnedAutoQuotedPremium)
            End Get
            Set(value As String)
                _NonOwnedAutoQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_NonOwnedAutoQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80145; CoverageLimitId</remarks>
        Public Property PropertyDeductibleId As String '*currently being sent/returned in XML here instead of at building level
            Get
                Return _PropertyDeductibleId
            End Get
            Set(value As String)
                _PropertyDeductibleId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 10054 (this property is specific to text for CoverageLimitId)</remarks>
        Public Property EmployersLiability As String
            Get
                Return _EmployersLiability
            End Get
            Set(value As String)
                _EmployersLiability = value
                Select Case _EmployersLiability
                    Case "100/500/100"
                        _EmployersLiabilityId = "311"
                    Case "500/500/500"
                        _EmployersLiabilityId = "313"
                    Case "500/1,000/500"
                        _EmployersLiabilityId = "312"
                    Case "1,000/1,000/1,000"
                        _EmployersLiabilityId = "314"
                    Case "2,000/2,000/2,000"
                        _EmployersLiabilityId = "315"
                    Case "3,000/3,000/3,000"
                        _EmployersLiabilityId = "316"
                    Case "4,000/4,000/4,000"
                        _EmployersLiabilityId = "317"
                    Case "5,000/5,000/5,000"
                        _EmployersLiabilityId = "318"
                    Case "6,000/6,000/6,000"
                        _EmployersLiabilityId = "319"
                    Case "7,000/7,000/7,000"
                        _EmployersLiabilityId = "320"
                    Case "8,000/8,000/8,000"
                        _EmployersLiabilityId = "321"
                    Case "9,000/9,000/9,000"
                        _EmployersLiabilityId = "322"
                    Case "10,000/10,000/10,000"
                        _EmployersLiabilityId = "323"
                    Case Else
                        _EmployersLiabilityId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 10054 (this property is specific to CoverageLimitId)</remarks>
        Public Property EmployersLiabilityId As String
            Get
                Return _EmployersLiabilityId
            End Get
            Set(value As String)
                _EmployersLiabilityId = value
                _EmployersLiability = ""
                If IsNumeric(_EmployersLiabilityId) = True Then
                    Select Case _EmployersLiabilityId
                        Case "311"
                            _EmployersLiability = "100/500/100"
                        Case "313"
                            _EmployersLiability = "500/500/500"
                        Case "312"
                            _EmployersLiability = "500/1,000/500"
                        Case "314"
                            _EmployersLiability = "1,000/1,000/1,000"
                        Case "315"
                            _EmployersLiability = "2,000/2,000/2,000"
                        Case "316"
                            _EmployersLiability = "3,000/3,000/3,000"
                        Case "317"
                            _EmployersLiability = "4,000/4,000/4,000"
                        Case "318"
                            _EmployersLiability = "5,000/5,000/5,000"
                        Case "319"
                            _EmployersLiability = "6,000/6,000/6,000"
                        Case "320"
                            _EmployersLiability = "7,000/7,000/7,000"
                        Case "321"
                            _EmployersLiability = "8,000/8,000/8,000"
                        Case "322"
                            _EmployersLiability = "9,000/9,000/9,000"
                        Case "323"
                            _EmployersLiability = "10,000/10,000/10,000"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 10054</remarks>
        Public Property EmployersLiabilityQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_EmployersLiabilityQuotedPremium)
            End Get
            Set(value As String)
                _EmployersLiabilityQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_EmployersLiabilityQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80155</remarks>
        Public Property GeneralAggregateLimit As String
            Get
                Return _GeneralAggregateLimit
            End Get
            Set(value As String)
                _GeneralAggregateLimit = value
                Select Case _GeneralAggregateLimit
                    Case "50,000"
                        _GeneralAggregateLimitId = "9"
                    Case "100,000"
                        _GeneralAggregateLimitId = "10"
                    Case "200,000"
                        _GeneralAggregateLimitId = "32"
                    Case "300,000"
                        _GeneralAggregateLimitId = "33"
                    Case "500,000"
                        _GeneralAggregateLimitId = "34"
                    Case "600,000"
                        _GeneralAggregateLimitId = "178"
                    Case "1,000,000"
                        _GeneralAggregateLimitId = "56"
                    Case "1,500,000"
                        _GeneralAggregateLimitId = "185"
                    Case "2,000,000"
                        _GeneralAggregateLimitId = "65"
                    Case "3,000,000"
                        _GeneralAggregateLimitId = "167"
                    Case Else
                        _GeneralAggregateLimitId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80155</remarks>
        Public Property GeneralAggregateLimitId As String
            Get
                Return _GeneralAggregateLimitId
            End Get
            Set(value As String)
                _GeneralAggregateLimitId = value
                _GeneralAggregateLimit = ""
                If IsNumeric(_GeneralAggregateLimitId) = True Then
                    Select Case _GeneralAggregateLimitId
                        Case "9"
                            _GeneralAggregateLimit = "50,000"
                        Case "10"
                            _GeneralAggregateLimit = "100,000"
                        Case "32"
                            _GeneralAggregateLimit = "200,000"
                        Case "33"
                            _GeneralAggregateLimit = "300,000"
                        Case "34"
                            _GeneralAggregateLimit = "500,000"
                        Case "178"
                            _GeneralAggregateLimit = "600,000"
                        Case "56"
                            _GeneralAggregateLimit = "1,000,000"
                        Case "185"
                            _GeneralAggregateLimit = "1,500,000"
                        Case "65"
                            _GeneralAggregateLimit = "2,000,000"
                        Case "167"
                            _GeneralAggregateLimit = "3,000,000"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80155</remarks>
        Public Property GeneralAggregateQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_GeneralAggregateQuotedPremium)
            End Get
            Set(value As String)
                _GeneralAggregateQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_GeneralAggregateQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80156</remarks>
        Public Property ProductsCompletedOperationsAggregateLimit As String
            Get
                Return _ProductsCompletedOperationsAggregateLimit
            End Get
            Set(value As String)
                _ProductsCompletedOperationsAggregateLimit = value
                Select Case _ProductsCompletedOperationsAggregateLimit
                    Case "Excluded"
                        _ProductsCompletedOperationsAggregateLimitId = "327"
                    Case "50,000"
                        _ProductsCompletedOperationsAggregateLimitId = "9"
                    Case "100,000"
                        _ProductsCompletedOperationsAggregateLimitId = "10"
                    Case "200,000"
                        _ProductsCompletedOperationsAggregateLimitId = "32"
                    Case "300,000"
                        _ProductsCompletedOperationsAggregateLimitId = "33"
                    Case "500,000"
                        _ProductsCompletedOperationsAggregateLimitId = "34"
                    Case "600,000"
                        _ProductsCompletedOperationsAggregateLimitId = "178"
                    Case "1,000,000"
                        _ProductsCompletedOperationsAggregateLimitId = "56"
                    Case "1,500,000"
                        _ProductsCompletedOperationsAggregateLimitId = "185"
                    Case "2,000,000"
                        _ProductsCompletedOperationsAggregateLimitId = "65"
                    Case "3,000,000"
                        _ProductsCompletedOperationsAggregateLimitId = "167"
                    Case Else
                        _ProductsCompletedOperationsAggregateLimitId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80156</remarks>
        Public Property ProductsCompletedOperationsAggregateLimitId As String
            Get
                Return _ProductsCompletedOperationsAggregateLimitId
            End Get
            Set(value As String)
                _ProductsCompletedOperationsAggregateLimitId = value
                _ProductsCompletedOperationsAggregateLimit = ""
                If IsNumeric(_ProductsCompletedOperationsAggregateLimitId) = True Then
                    Select Case _ProductsCompletedOperationsAggregateLimitId
                        Case "327"
                            _ProductsCompletedOperationsAggregateLimit = "Excluded"
                        Case "9"
                            _ProductsCompletedOperationsAggregateLimit = "50,000"
                        Case "10"
                            _ProductsCompletedOperationsAggregateLimit = "100,000"
                        Case "32"
                            _ProductsCompletedOperationsAggregateLimit = "200,000"
                        Case "33"
                            _ProductsCompletedOperationsAggregateLimit = "300,000"
                        Case "34"
                            _ProductsCompletedOperationsAggregateLimit = "500,000"
                        Case "178"
                            _ProductsCompletedOperationsAggregateLimit = "600,000"
                        Case "56"
                            _ProductsCompletedOperationsAggregateLimit = "1,000,000"
                        Case "185"
                            _ProductsCompletedOperationsAggregateLimit = "1,500,000"
                        Case "65"
                            _ProductsCompletedOperationsAggregateLimit = "2,000,000"
                        Case "167"
                            _ProductsCompletedOperationsAggregateLimit = "3,000,000"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80156</remarks>
        Public Property ProductsCompletedOperationsAggregateQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_ProductsCompletedOperationsAggregateQuotedPremium)
            End Get
            Set(value As String)
                _ProductsCompletedOperationsAggregateQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_ProductsCompletedOperationsAggregateQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80169</remarks>
        Public Property PersonalAndAdvertisingInjuryLimit As String
            Get
                Return _PersonalAndAdvertisingInjuryLimit
            End Get
            Set(value As String)
                _PersonalAndAdvertisingInjuryLimit = value
                Select Case _PersonalAndAdvertisingInjuryLimit
                    Case "Excluded"
                        _PersonalAndAdvertisingInjuryLimitId = "327"
                    Case "25,000"
                        _PersonalAndAdvertisingInjuryLimitId = "8"
                    Case "50,000"
                        _PersonalAndAdvertisingInjuryLimitId = "9"
                    Case "100,000"
                        _PersonalAndAdvertisingInjuryLimitId = "10"
                    Case "200,000"
                        _PersonalAndAdvertisingInjuryLimitId = "32"
                    Case "300,000"
                        _PersonalAndAdvertisingInjuryLimitId = "33"
                    Case "500,000"
                        _PersonalAndAdvertisingInjuryLimitId = "34"
                    Case "1,000,000"
                        _PersonalAndAdvertisingInjuryLimitId = "56"
                    Case Else
                        _PersonalAndAdvertisingInjuryLimitId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80169</remarks>
        Public Property PersonalAndAdvertisingInjuryLimitId As String
            Get
                Return _PersonalAndAdvertisingInjuryLimitId
            End Get
            Set(value As String)
                _PersonalAndAdvertisingInjuryLimitId = value
                _PersonalAndAdvertisingInjuryLimit = ""
                If IsNumeric(_PersonalAndAdvertisingInjuryLimitId) = True Then
                    Select Case _PersonalAndAdvertisingInjuryLimitId
                        Case "327"
                            _PersonalAndAdvertisingInjuryLimit = "Excluded"
                        Case "8"
                            _PersonalAndAdvertisingInjuryLimit = "25,000"
                        Case "9"
                            _PersonalAndAdvertisingInjuryLimit = "50,000"
                        Case "10"
                            _PersonalAndAdvertisingInjuryLimit = "100,000"
                        Case "32"
                            _PersonalAndAdvertisingInjuryLimit = "200,000"
                        Case "33"
                            _PersonalAndAdvertisingInjuryLimit = "300,000"
                        Case "34"
                            _PersonalAndAdvertisingInjuryLimit = "500,000"
                        Case "56"
                            _PersonalAndAdvertisingInjuryLimit = "1,000,000"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80169</remarks>
        Public Property PersonalAndAdvertisingInjuryQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_PersonalAndAdvertisingInjuryQuotedPremium)
            End Get
            Set(value As String)
                _PersonalAndAdvertisingInjuryQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_PersonalAndAdvertisingInjuryQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80178</remarks>
        Public Property DamageToPremisesRentedLimit As String
            Get
                Return _DamageToPremisesRentedLimit
            End Get
            Set(value As String)
                _DamageToPremisesRentedLimit = value
                Select Case _DamageToPremisesRentedLimit
                    Case "Excluded"
                        _DamageToPremisesRentedLimitId = "327"
                    Case "50,000"
                        _DamageToPremisesRentedLimitId = "9"
                    Case "75,000"
                        _DamageToPremisesRentedLimitId = "50"
                    Case "100,000"
                        _DamageToPremisesRentedLimitId = "10"
                    Case "200,000"
                        _DamageToPremisesRentedLimitId = "32"
                    Case "300,000"
                        _DamageToPremisesRentedLimitId = "33"
                    Case "400,000"
                        _DamageToPremisesRentedLimitId = "177"
                    Case Else
                        _DamageToPremisesRentedLimitId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80178</remarks>
        Public Property DamageToPremisesRentedLimitId As String
            Get
                Return _DamageToPremisesRentedLimitId
            End Get
            Set(value As String)
                _DamageToPremisesRentedLimitId = value
                _DamageToPremisesRentedLimit = ""
                If IsNumeric(_DamageToPremisesRentedLimitId) = True Then
                    Select Case _DamageToPremisesRentedLimitId
                        Case "327"
                            _DamageToPremisesRentedLimit = "Excluded"
                        Case "9"
                            _DamageToPremisesRentedLimit = "50,000"
                        Case "50"
                            _DamageToPremisesRentedLimit = "75,000"
                        Case "10"
                            _DamageToPremisesRentedLimit = "100,000"
                        Case "32"
                            _DamageToPremisesRentedLimit = "200,000"
                        Case "33"
                            _DamageToPremisesRentedLimit = "300,000"
                        Case "177"
                            _DamageToPremisesRentedLimit = "400,000"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80178</remarks>
        Public Property DamageToPremisesRentedQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_DamageToPremisesRentedQuotedPremium)
            End Get
            Set(value As String)
                _DamageToPremisesRentedQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_DamageToPremisesRentedQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80170</remarks>
        Public Property MedicalExpensesLimit As String
            Get
                Return _MedicalExpensesLimit
            End Get
            Set(value As String)
                _MedicalExpensesLimit = value
                Select Case _MedicalExpensesLimit
                    Case "Excluded"
                        _MedicalExpensesLimitId = "327"
                    Case "5,000"
                        _MedicalExpensesLimitId = "15"
                    Case "10,000" '*probably shouldn't be an option but it was on the specs
                        _MedicalExpensesLimitId = "7"
                    Case Else
                        _MedicalExpensesLimitId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80170</remarks>
        Public Property MedicalExpensesLimitId As String
            Get
                Return _MedicalExpensesLimitId
            End Get
            Set(value As String)
                _MedicalExpensesLimitId = value
                _MedicalExpensesLimit = ""
                If IsNumeric(_MedicalExpensesLimitId) = True Then
                    Select Case _MedicalExpensesLimitId
                        Case "327"
                            _MedicalExpensesLimit = "Excluded"
                        Case "15"
                            _MedicalExpensesLimit = "5,000"
                        Case "7" '*probably shouldn't be an option but it was on the specs
                            _MedicalExpensesLimit = "10,000"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80170</remarks>
        Public Property MedicalExpensesQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_MedicalExpensesQuotedPremium)
            End Get
            Set(value As String)
                _MedicalExpensesQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_MedicalExpensesQuotedPremium)
            End Set
        End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>specific to Diamond InclusionsExclusions w/ TypeId 12</remarks>
        'Public Property HasExclusionOfAmishWorkers As Boolean
        '    Get
        '        Return _HasExclusionOfAmishWorkers
        '    End Get
        '    Set(value As Boolean)
        '        _HasExclusionOfAmishWorkers = value
        '        If QuickQuoteHelperClass.UseComparativeRaterForLob(lob:=_LobType, quoteTransactionType:=QuoteTransactionType) <> QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then 'using <> Yes to also allow on Maybe
        '            QuickQuoteHelperClass.UpdateExclusionOfAmishWorkerListFromHasFlag(_ExclusionOfAmishWorkerRecords, _HasExclusionOfAmishWorkers, exclusionsBackup:=_ExclusionOfAmishWorkerRecordsBackup, updateBackupListBeforeRemoving:=True, addInitialItemIfNeeded:=False)
        '        End If
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>specific to Diamond InclusionsExclusions w/ TypeId 13</remarks>
        'Public Property HasExclusionOfSoleProprietorsPartnersOfficersAndOthers As Boolean
        '    Get
        '        Return _HasExclusionOfSoleProprietorsPartnersOfficersAndOthers
        '    End Get
        '    Set(value As Boolean)
        '        _HasExclusionOfSoleProprietorsPartnersOfficersAndOthers = value
        '        If QuickQuoteHelperClass.UseComparativeRaterForLob(lob:=_LobType, quoteTransactionType:=QuoteTransactionType) <> QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then 'using <> Yes to also allow on Maybe
        '            QuickQuoteHelperClass.UpdateExclusionOfSoleProprietorListFromHasFlag(_ExclusionOfSoleProprietorRecords, _HasExclusionOfSoleProprietorsPartnersOfficersAndOthers, exclusionsBackup:=_ExclusionOfSoleProprietorRecordsBackup, updateBackupListBeforeRemoving:=True, addInitialItemIfNeeded:=False)
        '        End If
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>specific to Diamond InclusionsExclusions w/ TypeId 14</remarks>
        'Public Property HasInclusionOfSoleProprietorsPartnersOfficersAndOthers As Boolean
        '    Get
        '        Return _HasInclusionOfSoleProprietorsPartnersOfficersAndOthers
        '    End Get
        '    Set(value As Boolean)
        '        _HasInclusionOfSoleProprietorsPartnersOfficersAndOthers = value
        '        If QuickQuoteHelperClass.UseComparativeRaterForLob(lob:=_LobType, quoteTransactionType:=QuoteTransactionType) <> QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then 'using <> Yes to also allow on Maybe
        '            QuickQuoteHelperClass.UpdateInclusionOfSoleProprietorListFromHasFlag(_InclusionOfSoleProprietorRecords, _HasInclusionOfSoleProprietorsPartnersOfficersAndOthers, inclusionsBackup:=_InclusionOfSoleProprietorRecordsBackup, updateBackupListBeforeRemoving:=True, addInitialItemIfNeeded:=False)
        '        End If
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>specific to Diamond InclusionsExclusions w/ TypeId 15</remarks>
        'Public Property HasWaiverOfSubrogation As Boolean
        '    Get
        '        Return _HasWaiverOfSubrogation
        '    End Get
        '    Set(value As Boolean)
        '        _HasWaiverOfSubrogation = value
        '        If QuickQuoteHelperClass.UseComparativeRaterForLob(lob:=_LobType, quoteTransactionType:=QuoteTransactionType) <> QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then 'using <> Yes to also allow on Maybe
        '            QuickQuoteHelperClass.UpdateWaiverOfSubrogationRecordListFromHasFlag(_WaiverOfSubrogationRecords, _HasWaiverOfSubrogation, waiversBackup:=_WaiverOfSubrogationRecordsBackup, updateBackupListBeforeRemoving:=True, addInitialItemIfNeeded:=False)
        '        End If
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>specific to Diamond InclusionsExclusions w/ TypeId 15</remarks>
        'Public Property WaiverOfSubrogationNumberOfWaivers As Integer
        '    Get
        '        If QuickQuoteHelperClass.UseComparativeRaterForLob(lob:=_LobType, quoteTransactionType:=QuoteTransactionType) <> QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then 'using <> Yes to also allow on Maybe
        '            If _WaiverOfSubrogationRecords IsNot Nothing AndAlso _WaiverOfSubrogationRecords.Count > 0 Then
        '                _WaiverOfSubrogationNumberOfWaivers = _WaiverOfSubrogationRecords.Count
        '            Else
        '                _WaiverOfSubrogationNumberOfWaivers = 0
        '            End If
        '        End If
        '        Return _WaiverOfSubrogationNumberOfWaivers
        '    End Get
        '    Set(value As Integer)
        '        _WaiverOfSubrogationNumberOfWaivers = value
        '        If QuickQuoteHelperClass.UseComparativeRaterForLob(lob:=_LobType, quoteTransactionType:=QuoteTransactionType) <> QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then 'using <> Yes to also allow on Maybe
        '            QuickQuoteHelperClass.UpdateWaiverOfSubrogationRecordListFromNumberOfWaivers(_WaiverOfSubrogationRecords, _WaiverOfSubrogationNumberOfWaivers, waiversBackup:=_WaiverOfSubrogationRecordsBackup, updateBackupListBeforeRemoving:=True)
        '        End If
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>specific to Diamond InclusionsExclusions w/ TypeId 15</remarks>
        'Public Property WaiverOfSubrogationPremium As String
        '    Get
        '        If QuickQuoteHelperClass.UseComparativeRaterForLob(lob:=_LobType, quoteTransactionType:=QuoteTransactionType) <> QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then 'using <> Yes to also allow on Maybe Then
        '            If _NeedsToUpdateWaiverOfSubrogationPremiumId = False Then
        '                If _WaiverOfSubrogationRecords IsNot Nothing AndAlso _WaiverOfSubrogationRecords.Count > 0 AndAlso WaiverOfSubrogationRecords(_WaiverOfSubrogationRecords.Count - 1) IsNot Nothing Then
        '                    _WaiverOfSubrogationPremium = _WaiverOfSubrogationRecords(_WaiverOfSubrogationRecords.Count - 1).Premium
        '                End If
        '            End If
        '        End If
        '        Return _WaiverOfSubrogationPremium
        '    End Get
        '    Set(value As String)
        '        _WaiverOfSubrogationPremium = value
        '        Select Case _WaiverOfSubrogationPremium
        '            Case "Not Assigned"
        '                _WaiverOfSubrogationPremiumId = "0"
        '            Case "0"
        '                _WaiverOfSubrogationPremiumId = "1"
        '            Case "25"
        '                _WaiverOfSubrogationPremiumId = "2"
        '            Case "50"
        '                _WaiverOfSubrogationPremiumId = "3"
        '            Case "75"
        '                _WaiverOfSubrogationPremiumId = "4"
        '            Case "100"
        '                _WaiverOfSubrogationPremiumId = "5"
        '            Case "150"
        '                _WaiverOfSubrogationPremiumId = "6"
        '            Case "200"
        '                _WaiverOfSubrogationPremiumId = "7"
        '            Case "250"
        '                _WaiverOfSubrogationPremiumId = "8"
        '            Case "300"
        '                _WaiverOfSubrogationPremiumId = "9"
        '            Case "400"
        '                _WaiverOfSubrogationPremiumId = "10"
        '            Case "500"
        '                _WaiverOfSubrogationPremiumId = "11"
        '            Case Else
        '                _WaiverOfSubrogationPremiumId = ""
        '        End Select
        '        If QuickQuoteHelperClass.UseComparativeRaterForLob(lob:=_LobType, quoteTransactionType:=QuoteTransactionType) <> QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then 'using <> Yes to also allow on Maybe Then
        '            Dim waiversUpdated As Integer = 0
        '            QuickQuoteHelperClass.UpdateWaiverOfSubrogationRecordListWithPremiumId(_WaiverOfSubrogationRecords, _WaiverOfSubrogationPremiumId, waiversUpdated:=waiversUpdated)
        '            If waiversUpdated > 0 Then
        '                _NeedsToUpdateWaiverOfSubrogationPremiumId = False
        '            Else
        '                _NeedsToUpdateWaiverOfSubrogationPremiumId = True
        '            End If
        '        End If
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>specific to Diamond InclusionsExclusions w/ TypeId 15</remarks>
        'Public Property WaiverOfSubrogationPremiumId As String
        '    Get
        '        If QuickQuoteHelperClass.UseComparativeRaterForLob(lob:=_LobType, quoteTransactionType:=QuoteTransactionType) <> QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then 'using <> Yes to also allow on Maybe Then
        '            If _NeedsToUpdateWaiverOfSubrogationPremiumId = False Then
        '                If _WaiverOfSubrogationRecords IsNot Nothing AndAlso _WaiverOfSubrogationRecords.Count > 0 AndAlso WaiverOfSubrogationRecords(_WaiverOfSubrogationRecords.Count - 1) IsNot Nothing Then
        '                    _WaiverOfSubrogationPremiumId = _WaiverOfSubrogationRecords(_WaiverOfSubrogationRecords.Count - 1).PremiumId
        '                End If
        '            End If
        '        End If
        '        Return _WaiverOfSubrogationPremiumId
        '    End Get
        '    Set(value As String)
        '        _WaiverOfSubrogationPremiumId = value
        '        _WaiverOfSubrogationPremium = ""
        '        If IsNumeric(_WaiverOfSubrogationPremiumId) = True Then
        '            Select Case _WaiverOfSubrogationPremiumId
        '                Case "0"
        '                    _WaiverOfSubrogationPremium = "Not Assigned"
        '                Case "1"
        '                    _WaiverOfSubrogationPremium = "0"
        '                Case "2"
        '                    _WaiverOfSubrogationPremium = "25"
        '                Case "3"
        '                    _WaiverOfSubrogationPremium = "50"
        '                Case "4"
        '                    _WaiverOfSubrogationPremium = "75"
        '                Case "5"
        '                    _WaiverOfSubrogationPremium = "100"
        '                Case "6"
        '                    _WaiverOfSubrogationPremium = "150"
        '                Case "7"
        '                    _WaiverOfSubrogationPremium = "200"
        '                Case "8"
        '                    _WaiverOfSubrogationPremium = "250"
        '                Case "9"
        '                    _WaiverOfSubrogationPremium = "300"
        '                Case "10"
        '                    _WaiverOfSubrogationPremium = "400"
        '                Case "11"
        '                    _WaiverOfSubrogationPremium = "500"
        '            End Select
        '        End If
        '        If QuickQuoteHelperClass.UseComparativeRaterForLob(lob:=_LobType, quoteTransactionType:=QuoteTransactionType) <> QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then 'using <> Yes to also allow on Maybe Then
        '            Dim waiversUpdated As Integer = 0
        '            QuickQuoteHelperClass.UpdateWaiverOfSubrogationRecordListWithPremiumId(_WaiverOfSubrogationRecords, _WaiverOfSubrogationPremiumId, waiversUpdated:=waiversUpdated)
        '            If waiversUpdated > 0 Then
        '                _NeedsToUpdateWaiverOfSubrogationPremiumId = False
        '            Else
        '                _NeedsToUpdateWaiverOfSubrogationPremiumId = True
        '            End If
        '        End If
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>specific to Diamond InclusionsExclusions w/ TypeId 12</remarks>
        'Public Property ExclusionOfAmishWorkerRecords As Generic.List(Of QuickQuoteExclusionOfAmishWorkerRecord)
        '    Get
        '        Return _ExclusionOfAmishWorkerRecords
        '    End Get
        '    Set(value As Generic.List(Of QuickQuoteExclusionOfAmishWorkerRecord))
        '        _ExclusionOfAmishWorkerRecords = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>specific to Diamond InclusionsExclusions w/ TypeId 13</remarks>
        'Public Property ExclusionOfSoleProprietorRecords As Generic.List(Of QuickQuoteExclusionOfSoleProprietorRecord)
        '    Get
        '        Return _ExclusionOfSoleProprietorRecords
        '    End Get
        '    Set(value As Generic.List(Of QuickQuoteExclusionOfSoleProprietorRecord))
        '        _ExclusionOfSoleProprietorRecords = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>specific to Diamond InclusionsExclusions w/ TypeId 14</remarks>
        'Public Property InclusionOfSoleProprietorRecords As Generic.List(Of QuickQuoteInclusionOfSoleProprietorRecord)
        '    Get
        '        Return _InclusionOfSoleProprietorRecords
        '    End Get
        '    Set(value As Generic.List(Of QuickQuoteInclusionOfSoleProprietorRecord))
        '        _InclusionOfSoleProprietorRecords = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>specific to Diamond InclusionsExclusions w/ TypeId 15</remarks>
        'Public Property WaiverOfSubrogationRecords As Generic.List(Of QuickQuoteWaiverOfSubrogationRecord)
        '    Get
        '        Return _WaiverOfSubrogationRecords
        '    End Get
        '    Set(value As Generic.List(Of QuickQuoteWaiverOfSubrogationRecord))
        '        _WaiverOfSubrogationRecords = value
        '    End Set
        'End Property
        'Public ReadOnly Property ExclusionOfAmishWorkerRecordsBackup As Generic.List(Of QuickQuoteExclusionOfAmishWorkerRecord)
        '    Get
        '        Return _ExclusionOfAmishWorkerRecordsBackup
        '    End Get
        'End Property
        'Public ReadOnly Property ExclusionOfSoleProprietorRecordsBackup As Generic.List(Of QuickQuoteExclusionOfSoleProprietorRecord)
        '    Get
        '        Return _ExclusionOfSoleProprietorRecordsBackup
        '    End Get
        'End Property
        'Public ReadOnly Property InclusionOfSoleProprietorRecordsBackup As Generic.List(Of QuickQuoteInclusionOfSoleProprietorRecord)
        '    Get
        '        Return _InclusionOfSoleProprietorRecordsBackup
        '    End Get
        'End Property
        'Public ReadOnly Property WaiverOfSubrogationRecordsBackup As Generic.List(Of QuickQuoteWaiverOfSubrogationRecord)
        '    Get
        '        Return _WaiverOfSubrogationRecordsBackup
        '    End Get
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21032</remarks>
        'Public Property HasBarbersProfessionalLiability As Boolean
        '    Get
        '        Return _HasBarbersProfessionalLiability
        '    End Get
        '    Set(value As Boolean)
        '        _HasBarbersProfessionalLiability = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21032</remarks>
        'Public Property BarbersProfessionalLiabiltyQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_BarbersProfessionalLiabiltyQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _BarbersProfessionalLiabiltyQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_BarbersProfessionalLiabiltyQuotedPremium)
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21032</remarks>
        'Public Property BarbersProfessionalLiabilityFullTimeEmpNum As String
        '    Get
        '        Return _BarbersProfessionalLiabilityFullTimeEmpNum
        '    End Get
        '    Set(value As String)
        '        _BarbersProfessionalLiabilityFullTimeEmpNum = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21032</remarks>
        'Public Property BarbersProfessionalLiabilityPartTimeEmpNum As String
        '    Get
        '        Return _BarbersProfessionalLiabilityPartTimeEmpNum
        '    End Get
        '    Set(value As String)
        '        _BarbersProfessionalLiabilityPartTimeEmpNum = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21032</remarks>
        'Public Property BarbersProfessionalLiabilityDescription As String
        '    Get
        '        Return _BarbersProfessionalLiabilityDescription
        '    End Get
        '    Set(value As String)
        '        _BarbersProfessionalLiabilityDescription = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21033</remarks>
        'Public Property HasBeauticiansProfessionalLiability As Boolean
        '    Get
        '        Return _HasBeauticiansProfessionalLiability
        '    End Get
        '    Set(value As Boolean)
        '        _HasBeauticiansProfessionalLiability = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21033</remarks>
        'Public Property BeauticiansProfessionalLiabilityQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_BeauticiansProfessionalLiabilityQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _BeauticiansProfessionalLiabilityQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_BeauticiansProfessionalLiabilityQuotedPremium)
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21033</remarks>
        'Public Property BeauticiansProfessionalLiabilityFullTimeEmpNum As String
        '    Get
        '        Return _BeauticiansProfessionalLiabilityFullTimeEmpNum
        '    End Get
        '    Set(value As String)
        '        _BeauticiansProfessionalLiabilityFullTimeEmpNum = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21033</remarks>
        'Public Property BeauticiansProfessionalLiabilityPartTimeEmpNum As String
        '    Get
        '        Return _BeauticiansProfessionalLiabilityPartTimeEmpNum
        '    End Get
        '    Set(value As String)
        '        _BeauticiansProfessionalLiabilityPartTimeEmpNum = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21033</remarks>
        'Public Property BeauticiansProfessionalLiabilityDescription As String
        '    Get
        '        Return _BeauticiansProfessionalLiabilityDescription
        '    End Get
        '    Set(value As String)
        '        _BeauticiansProfessionalLiabilityDescription = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21034</remarks>
        'Public Property HasFuneralDirectorsProfessionalLiability As Boolean
        '    Get
        '        Return _HasFuneralDirectorsProfessionalLiability
        '    End Get
        '    Set(value As Boolean)
        '        _HasFuneralDirectorsProfessionalLiability = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21034</remarks>
        'Public Property FuneralDirectorsProfessionalLiabilityQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_FuneralDirectorsProfessionalLiabilityQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _FuneralDirectorsProfessionalLiabilityQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_FuneralDirectorsProfessionalLiabilityQuotedPremium)
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21034</remarks>
        'Public Property FuneralDirectorsProfessionalLiabilityEmpNum As String
        '    Get
        '        Return _FuneralDirectorsProfessionalLiabilityEmpNum
        '    End Get
        '    Set(value As String)
        '        _FuneralDirectorsProfessionalLiabilityEmpNum = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21036</remarks>
        'Public Property HasPrintersProfessionalLiability As Boolean
        '    Get
        '        Return _HasPrintersProfessionalLiability
        '    End Get
        '    Set(value As Boolean)
        '        _HasPrintersProfessionalLiability = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21036</remarks>
        'Public Property PrintersProfessionalLiabilityQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_PrintersProfessionalLiabilityQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _PrintersProfessionalLiabilityQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_PrintersProfessionalLiabilityQuotedPremium)
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21036</remarks>
        'Public Property PrintersProfessionalLiabilityLocNum As String
        '    Get
        '        Return _PrintersProfessionalLiabilityLocNum
        '    End Get
        '    Set(value As String)
        '        _PrintersProfessionalLiabilityLocNum = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21058</remarks>
        'Public Property HasSelfStorageFacility As Boolean
        '    Get
        '        Return _HasSelfStorageFacility
        '    End Get
        '    Set(value As Boolean)
        '        _HasSelfStorageFacility = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21058</remarks>
        'Public Property SelfStorageFacilityQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_SelfStorageFacilityQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _SelfStorageFacilityQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_SelfStorageFacilityQuotedPremium)
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21058</remarks>
        'Public Property SelfStorageFacilityLimit As String
        '    Get
        '        Return _SelfStorageFacilityLimit
        '    End Get
        '    Set(value As String)
        '        _SelfStorageFacilityLimit = value
        '        qqHelper.ConvertToLimitFormat(_SelfStorageFacilityLimit)
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 164</remarks>
        'Public Property HasVeterinariansProfessionalLiability As Boolean
        '    Get
        '        Return _HasVeterinariansProfessionalLiability
        '    End Get
        '    Set(value As Boolean)
        '        _HasVeterinariansProfessionalLiability = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 164</remarks>
        'Public Property VeterinariansProfessionalLiabilityEmpNum As String
        '    Get
        '        Return _VeterinariansProfessionalLiabilityEmpNum
        '    End Get
        '    Set(value As String)
        '        _VeterinariansProfessionalLiabilityEmpNum = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 164</remarks>
        'Public Property VeterinariansProfessionalLiabilityQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_VeterinariansProfessionalLiabilityQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _VeterinariansProfessionalLiabilityQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_VeterinariansProfessionalLiabilityQuotedPremium)
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 164</remarks>
        'Public Property HasPharmacistProfessionalLiability As Boolean
        '    Get
        '        Return _HasPharmacistProfessionalLiability
        '    End Get
        '    Set(value As Boolean)
        '        _HasPharmacistProfessionalLiability = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 164</remarks>
        'Public Property PharmacistAnnualGrossSales As String
        '    Get
        '        Return _PharmacistAnnualGrossSales
        '    End Get
        '    Set(value As String)
        '        _PharmacistAnnualGrossSales = value
        '        qqHelper.ConvertToLimitFormat(_PharmacistAnnualGrossSales)
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 164</remarks>
        'Public Property PharmacistQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_PharmacistQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _PharmacistQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_PharmacistQuotedPremium)
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21134</remarks>
        'Public Property HasLiquorLiability As Boolean
        '    Get
        '        Return _HasLiquorLiability
        '    End Get
        '    Set(value As Boolean)
        '        _HasLiquorLiability = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21134</remarks>
        'Public Property LiquorLiabilityAnnualGrossAlcoholSalesReceipts As String
        '    Get
        '        Return _LiquorLiabilityAnnualGrossAlcoholSalesReceipts
        '    End Get
        '    Set(value As String)
        '        _LiquorLiabilityAnnualGrossAlcoholSalesReceipts = value
        '        qqHelper.ConvertToLimitFormat(_LiquorLiabilityAnnualGrossAlcoholSalesReceipts)
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21134</remarks>
        'Public Property LiquorLiabilityAnnualGrossPackageSalesReceipts As String
        '    Get
        '        Return _LiquorLiabilityAnnualGrossPackageSalesReceipts
        '    End Get
        '    Set(value As String)
        '        _LiquorLiabilityAnnualGrossPackageSalesReceipts = value
        '        qqHelper.ConvertToLimitFormat(_LiquorLiabilityAnnualGrossPackageSalesReceipts)
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21134</remarks>
        'Public ReadOnly Property LiquorLiabilityAggregateLimit As String
        '    Get
        '        Dim limit As String = "0"
        '        If _HasLiquorLiability = True Then
        '            If Not String.IsNullOrWhiteSpace(_OccurrenceLiabilityLimit) = True AndAlso IsNumeric(_OccurrenceLiabilityLimit) Then
        '                limit = (CInt(OccurrenceLiabilityLimit) * 2)
        '            End If
        '        End If
        '        qqHelper.ConvertToLimitFormat(limit)
        '        Return limit
        '    End Get
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21134</remarks>
        'Public Property LiquorLiabilityClassCodeTypeId As String
        '    Get
        '        Return _LiquorLiabilityClassCodeTypeId
        '    End Get
        '    Set(value As String)
        '        _LiquorLiabilityClassCodeTypeId = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21035</remarks>
        'Public Property HasOpticalAndHearingAidProfessionalLiability As Boolean
        '    Get
        '        Return _HasOpticalAndHearingAidProfessionalLiability
        '    End Get
        '    Set(value As Boolean)
        '        _HasOpticalAndHearingAidProfessionalLiability = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21035</remarks>
        'Public Property OpticalAndHearingAidProfessionalLiabilityEmpNum As String
        '    Get
        '        Return _OpticalAndHearingAidProfessionalLiabilityEmpNum
        '    End Get
        '    Set(value As String)
        '        _OpticalAndHearingAidProfessionalLiabilityEmpNum = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21035</remarks>
        'Public Property OpticalAndHearingAidProfessionalLiabilityQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_OpticalAndHearingAidProfessionalLiabilityQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _OpticalAndHearingAidProfessionalLiabilityQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_OpticalAndHearingAidProfessionalLiabilityQuotedPremium)
        '    End Set
        'End Property
        'Public ReadOnly Property CustomerAutoLegalQuotedPremium As String 'moved to RiskLevel object 7/23/2018
        '    Get
        '        Dim total As String = ""
        '        If Locations IsNot Nothing AndAlso Locations.Count > 0 Then '7/9/2018 note: using Locations public property instead of _Locations private variable
        '            For Each Loc As QuickQuoteLocation In Locations
        '                If Not String.IsNullOrWhiteSpace(Loc.CustomerAutoLegalLiabilityQuotedPremium) AndAlso IsNumeric(Loc.CustomerAutoLegalLiabilityQuotedPremium) Then
        '                    total = qqHelper.getSum(total, Loc.CustomerAutoLegalLiabilityQuotedPremium)
        '                End If
        '            Next
        '        End If
        '        qqHelper.ConvertToQuotedPremiumFormat(total)
        '        Return total
        '    End Get
        'End Property
        'Public ReadOnly Property TenantAutoLegalQuotedPremium As String 'moved to RiskLevel object 7/23/2018
        '    Get
        '        Dim total As String = ""
        '        If Locations IsNot Nothing AndAlso Locations.Count > 0 Then
        '            For Each Loc As QuickQuoteLocation In Locations
        '                If Not String.IsNullOrWhiteSpace(Loc.TenantAutoLegalLiabilityQuotedPremium) AndAlso IsNumeric(Loc.TenantAutoLegalLiabilityQuotedPremium) Then
        '                    total = qqHelper.getSum(total, Loc.TenantAutoLegalLiabilityQuotedPremium)
        '                End If
        '            Next
        '        End If
        '        qqHelper.ConvertToQuotedPremiumFormat(total)
        '        Return total
        '    End Get
        'End Property
        'Public ReadOnly Property FineArtsLocationQuotedPremium As String 'moved to RiskLevel object 7/23/2018
        '    Get
        '        Dim total As String = ""
        '        If Locations IsNot Nothing AndAlso Locations.Count > 0 Then '7/9/2018 note: using Locations public property instead of _Locations private variable
        '            For Each Loc As QuickQuoteLocation In Locations
        '                If Not String.IsNullOrWhiteSpace(Loc.FineArtsQuotedPremium) AndAlso IsNumeric(Loc.FineArtsQuotedPremium) Then
        '                    total = qqHelper.getSum(total, Loc.FineArtsQuotedPremium)
        '                End If
        '            Next
        '        End If
        '        qqHelper.ConvertToQuotedPremiumFormat(total)
        '        Return total
        '    End Get
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80376 and 80377</remarks>
        'Public Property HasMotelCoverage As Boolean
        '    Get
        '        Return _HasMotelCoverage
        '    End Get
        '    Set(value As Boolean)
        '        _HasMotelCoverage = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80376</remarks>
        'Public Property MotelCoveragePerGuestLimitId As String
        '    Get
        '        Return _MotelCoveragePerGuestLimitId '371 - 1,000/25,000, 372 - 2,000/50,000, 373 - 3,000/75,000, 374 - 4,000/100,000
        '    End Get
        '    Set(value As String)
        '        _MotelCoveragePerGuestLimitId = value
        '        Select Case _MotelCoveragePerGuestLimitId
        '            Case "371"
        '                _MotelCoveragePerGuestLimit = "1,000/25,000"
        '            Case "372"
        '                _MotelCoveragePerGuestLimit = "2,000/50,000"
        '            Case "373"
        '                _MotelCoveragePerGuestLimit = "3,000/75,000"
        '            Case "374"
        '                _MotelCoveragePerGuestLimit = "4,000/100,000"
        '        End Select
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80376</remarks>
        'Public Property MotelCoveragePerGuestLimit As String
        '    Get
        '        Return _MotelCoveragePerGuestLimit '371 - 1,000/25,000, 372 - 2,000/50,000, 373 - 3,000/75,000, 374 - 4,000/100,000
        '    End Get
        '    Set(value As String)
        '        _MotelCoveragePerGuestLimit = value
        '        Select Case _MotelCoveragePerGuestLimit
        '            Case "1000/25000"
        '                _MotelCoveragePerGuestLimitId = "371"
        '            Case "2000/50000"
        '                _MotelCoveragePerGuestLimitId = "372"
        '            Case "3000/75000"
        '                _MotelCoveragePerGuestLimitId = "373"
        '            Case "4000/100000"
        '                _MotelCoveragePerGuestLimitId = "374"
        '        End Select
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80377</remarks>
        'Public Property MotelCoverageSafeDepositLimitId As String
        '    Get
        '        Return _MotelCoverageSafeDepositLimitId '0 - N/A, 8 - 25,000, 9 - 50,000, 10 - 100,000, 55 - 250,000
        '    End Get
        '    Set(value As String)
        '        _MotelCoverageSafeDepositLimitId = value
        '        Select Case _MotelCoverageSafeDepositLimitId
        '            Case "0"
        '                _MotelCoverageSafeDepositLimit = "N/A"
        '            Case "8"
        '                _MotelCoverageSafeDepositLimit = "25,000"
        '            Case "9"
        '                _MotelCoverageSafeDepositLimit = "50,000"
        '            Case "10"
        '                _MotelCoverageSafeDepositLimit = "100,000"
        '            Case "55"
        '                _MotelCoverageSafeDepositLimit = "250,000"
        '        End Select
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80377</remarks>
        'Public Property MotelCoverageSafeDepositLimit As String
        '    Get
        '        Return _MotelCoverageSafeDepositLimit '0 - N/A, 8 - 25,000, 9 - 50,000, 10 - 100,000, 55 - 250,000
        '    End Get
        '    Set(value As String)
        '        _MotelCoverageSafeDepositLimit = value
        '        Select Case _MotelCoverageSafeDepositLimit
        '            Case "N/A"
        '                _MotelCoverageSafeDepositLimitId = "0"
        '            Case "25000"
        '                _MotelCoverageSafeDepositLimitId = "8"
        '            Case "50000"
        '                _MotelCoverageSafeDepositLimitId = "9"
        '            Case "100000"
        '                _MotelCoverageSafeDepositLimitId = "10"
        '            Case "250000"
        '                _MotelCoverageSafeDepositLimitId = "55"
        '        End Select
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80377</remarks>
        'Public Property MotelCoverageSafeDepositDeductibleId As String
        '    Get
        '        Return _MotelCoverageSafeDepositDeductibleId '40 - 0, 4 - 250,8 - 500,9 - 1,000,15 - 2,500
        '    End Get
        '    Set(value As String)
        '        _MotelCoverageSafeDepositDeductibleId = value
        '        Select Case _MotelCoverageSafeDepositDeductibleId
        '            Case "40"
        '                _MotelCoverageSafeDepositDeductible = "0"
        '            Case "4"
        '                _MotelCoverageSafeDepositDeductible = "250"
        '            Case "8"
        '                _MotelCoverageSafeDepositDeductible = "500"
        '            Case "9"
        '                _MotelCoverageSafeDepositDeductible = "1,000"
        '            Case "15"
        '                _MotelCoverageSafeDepositDeductible = "2,500"
        '        End Select
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80377</remarks>
        'Public Property MotelCoverageSafeDepositDeductible As String
        '    Get
        '        Return _MotelCoverageSafeDepositDeductible
        '    End Get
        '    Set(value As String)
        '        _MotelCoverageSafeDepositDeductible = value '40 - 0, 4 - 250,8 - 500,9 - 1,000,15 - 2,500
        '        Select Case _MotelCoverageSafeDepositDeductible
        '            Case "0"
        '                _MotelCoverageSafeDepositDeductibleId = "40"
        '            Case "250"
        '                _MotelCoverageSafeDepositDeductibleId = "4"
        '            Case "500"
        '                _MotelCoverageSafeDepositDeductibleId = "8"
        '            Case "1000"
        '                _MotelCoverageSafeDepositDeductibleId = "9"
        '            Case "2500"
        '                _MotelCoverageSafeDepositDeductibleId = "15"
        '            Case Else
        '                _MotelCoverageSafeDepositDeductibleId = "40"
        '        End Select
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80376</remarks>
        'Public Property MotelCoveragePerGuestQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_MotelCoveragePerGuestQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _MotelCoveragePerGuestQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_MotelCoveragePerGuestQuotedPremium)
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80377</remarks>
        'Public Property MotelCoverageSafeDepositQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_MotelCoverageSafeDepositQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _MotelCoverageSafeDepositQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_MotelCoverageSafeDepositQuotedPremium)
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80377</remarks>
        'Public Property MotelCoverageQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_MotelCoverageQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _MotelCoverageQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_MotelCoverageQuotedPremium)
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80398</remarks>
        'Public Property HasPhotographyCoverage As Boolean
        '    Get
        '        Return _HasPhotographyCoverage
        '    End Get
        '    Set(value As Boolean)
        '        _HasPhotographyCoverage = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80398</remarks>
        'Public Property HasPhotographyCoverageScheduledCoverages As Boolean
        '    Get
        '        Return _HasPhotographyCoverageScheduledCoverages
        '    End Get
        '    Set(value As Boolean)
        '        _HasPhotographyCoverageScheduledCoverages = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80398</remarks>
        'Public Property PhotographyCoverageQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_PhotographyCoverageQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _PhotographyCoverageQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_PhotographyCoverageQuotedPremium)
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80398</remarks>
        'Public ReadOnly Property PhotographyTotalScheduledLimits As String
        '    Get
        '        Dim total As Decimal = 0
        '        If _PhotographyScheduledCoverages IsNot Nothing AndAlso _PhotographyScheduledCoverages.Count > 0 Then
        '            For Each cov As QuickQuoteCoverage In _PhotographyScheduledCoverages
        '                If cov IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(cov.ManualLimitAmount) AndAlso IsNumeric(cov.ManualLimitAmount) Then
        '                    total += CDec(cov.ManualLimitAmount)
        '                End If
        '            Next
        '        End If
        '        Return FormatNumber(total, 0).ToString
        '    End Get
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond Scheduled Item w/ scheduleditem_id 21248</remarks>
        'Public Property PhotographyScheduledCoverages As List(Of QuickQuoteCoverage)
        '    Get
        '        Return _PhotographyScheduledCoverages
        '    End Get
        '    Set(value As List(Of QuickQuoteCoverage))
        '        _PhotographyScheduledCoverages = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80378</remarks>
        'Public Property HasPhotographyMakeupAndHair As Boolean
        '    Get
        '        Return _HasPhotographyMakeupAndHair
        '    End Get
        '    Set(value As Boolean)
        '        _HasPhotographyMakeupAndHair = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80378</remarks>
        'Public Property PhotographyMakeupAndHairQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_PhotographyMakeupAndHairQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _PhotographyMakeupAndHairQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_PhotographyMakeupAndHairQuotedPremium)
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80380</remarks>
        'Public Property HasResidentialCleaning As Boolean
        '    Get
        '        Return _HasResidentialCleaning
        '    End Get
        '    Set(value As Boolean)
        '        _HasResidentialCleaning = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80380</remarks>
        'Public Property ResidentialCleaningQuotedPremium As String
        '    Get
        '        Return _ResidentialCleaningQuotedPremium
        '    End Get
        '    Set(value As String)
        '        _ResidentialCleaningQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_ResidentialCleaningQuotedPremium)
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21134</remarks>
        'Public Property LiquorLiabilityOccurrenceLimit As String
        '    Get
        '        Return _LiquorLiabilityOccurrenceLimit
        '    End Get
        '    Set(value As String)
        '        _LiquorLiabilityOccurrenceLimit = value
        '        Select Case _LiquorLiabilityOccurrenceLimit
        '            Case "N/A"
        '                _LiquorLiabilityOccurrenceLimitId = "0"
        '            Case "300,000"
        '                _LiquorLiabilityOccurrenceLimitId = "33"
        '            Case "500,000"
        '                _LiquorLiabilityOccurrenceLimitId = "34"
        '            Case "1,000,000"
        '                _LiquorLiabilityOccurrenceLimitId = "56"
        '            Case Else
        '                _LiquorLiabilityOccurrenceLimitId = ""
        '        End Select
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21134</remarks>
        'Public Property LiquorLiabilityOccurrenceLimitId As String
        '    Get
        '        Return _LiquorLiabilityOccurrenceLimitId
        '    End Get
        '    Set(value As String)
        '        _LiquorLiabilityOccurrenceLimitId = value
        '        _LiquorLiabilityOccurrenceLimit = ""
        '        If IsNumeric(_LiquorLiabilityOccurrenceLimitId) = True Then
        '            Select Case _LiquorLiabilityOccurrenceLimitId
        '                Case "0"
        '                    _LiquorLiabilityOccurrenceLimit = "N/A"
        '                Case "33"
        '                    _LiquorLiabilityOccurrenceLimit = "300,000"
        '                Case "34"
        '                    _LiquorLiabilityOccurrenceLimit = "500,000"
        '                Case "56"
        '                    _LiquorLiabilityOccurrenceLimit = "1,000,000"
        '            End Select
        '        End If
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21134 (LiquorLiabilityClassificationId 50911 - Manufacturer, Wholesalers &amp; Distributors; LiquorLiabilityClassificationId 58161 - Restaurants or Hotels; LiquorLiabilityClassificationId 59211 - Package Stores; LiquorLiabilityClassificationId 70412 - Clubs</remarks>
        'Public Property LiquorLiabilityClassification As String
        '    Get
        '        Return _LiquorLiabilityClassification
        '    End Get
        '    Set(value As String)
        '        _LiquorLiabilityClassification = value
        '        Select Case _LiquorLiabilityClassification
        '            Case "Manufacturer, Wholesalers & Distributors"
        '                _LiquorLiabilityClassificationId = "50911"
        '            Case "Restaurants or Hotels"
        '                _LiquorLiabilityClassificationId = "58161"
        '            Case "Package Stores"
        '                _LiquorLiabilityClassificationId = "59211"
        '            Case "Clubs"
        '                _LiquorLiabilityClassificationId = "70412"
        '            Case Else
        '                _LiquorLiabilityClassificationId = ""
        '        End Select
        '        'SetLiquorRateAndMinimumPremForClassificationId()
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21134 (LiquorLiabilityClassificationId 50911 - Manufacturer, Wholesalers &amp; Distributors; LiquorLiabilityClassificationId 58161 - Restaurants or Hotels; LiquorLiabilityClassificationId 59211 - Package Stores; LiquorLiabilityClassificationId 70412 - Clubs</remarks>
        'Public Property LiquorLiabilityClassificationId As String
        '    Get
        '        Return _LiquorLiabilityClassificationId
        '    End Get
        '    Set(value As String)
        '        _LiquorLiabilityClassificationId = value
        '        _LiquorLiabilityClassification = ""
        '        If IsNumeric(_LiquorLiabilityClassificationId) = True Then
        '            Select Case _LiquorLiabilityClassificationId
        '                Case "50911"
        '                    _LiquorLiabilityClassification = "Manufacturer, Wholesalers & Distributors"
        '                Case "58161"
        '                    _LiquorLiabilityClassification = "Restaurants or Hotels"
        '                Case "59211"
        '                    _LiquorLiabilityClassification = "Package Stores"
        '                Case "70412"
        '                    _LiquorLiabilityClassification = "Clubs"
        '            End Select
        '        End If
        '        'SetLiquorRateAndMinimumPremForClassificationId()
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21134</remarks>
        'Public Property LiquorSales As String
        '    Get
        '        Return _LiquorSales
        '    End Get
        '    Set(value As String)
        '        _LiquorSales = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21134</remarks>
        'Public Property LiquorLiabilityQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_LiquorLiabilityQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _LiquorLiabilityQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_LiquorLiabilityQuotedPremium)
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21131</remarks>
        'Public Property ProfessionalLiabilityCemetaryNumberOfBurials As String
        '    Get
        '        Return _ProfessionalLiabilityCemetaryNumberOfBurials
        '    End Get
        '    Set(value As String)
        '        _ProfessionalLiabilityCemetaryNumberOfBurials = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21131</remarks>
        'Public Property ProfessionalLiabilityCemetaryQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_ProfessionalLiabilityCemetaryQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _ProfessionalLiabilityCemetaryQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_ProfessionalLiabilityCemetaryQuotedPremium)
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21034</remarks>
        'Public Property ProfessionalLiabilityFuneralDirectorsNumberOfBodies As String
        '    Get
        '        Return _ProfessionalLiabilityFuneralDirectorsNumberOfBodies
        '    End Get
        '    Set(value As String)
        '        _ProfessionalLiabilityFuneralDirectorsNumberOfBodies = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21132</remarks>
        'Public Property ProfessionalLiabilityPastoralNumberOfClergy As String
        '    Get
        '        Return _ProfessionalLiabilityPastoralNumberOfClergy
        '    End Get
        '    Set(value As String)
        '        _ProfessionalLiabilityPastoralNumberOfClergy = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21132</remarks>
        'Public Property ProfessionalLiabilityPastoralQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_ProfessionalLiabilityPastoralQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _ProfessionalLiabilityPastoralQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_ProfessionalLiabilityPastoralQuotedPremium)
        '    End Set
        'End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 4 (IRPM) and RiskCharacteristicTypeId 14</remarks>
        Public Property IRPM_ManagementCooperation As String
            Get
                Return _IRPM_ManagementCooperation
            End Get
            Set(value As String)
                _IRPM_ManagementCooperation = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 4 (IRPM) and RiskCharacteristicTypeId 14</remarks>
        Public Property IRPM_ManagementCooperationDesc As String
            Get
                Return _IRPM_ManagementCooperationDesc
            End Get
            Set(value As String)
                _IRPM_ManagementCooperationDesc = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 4 (IRPM) and RiskCharacteristicTypeId 1</remarks>
        Public Property IRPM_Location As String
            Get
                Return _IRPM_Location
            End Get
            Set(value As String)
                _IRPM_Location = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 4 (IRPM) and RiskCharacteristicTypeId 1</remarks>
        Public Property IRPM_LocationDesc As String
            Get
                Return _IRPM_LocationDesc
            End Get
            Set(value As String)
                _IRPM_LocationDesc = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 4 (IRPM) and RiskCharacteristicTypeId 9</remarks>
        Public Property IRPM_BuildingFeatures As String
            Get
                Return _IRPM_BuildingFeatures
            End Get
            Set(value As String)
                _IRPM_BuildingFeatures = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 4 (IRPM) and RiskCharacteristicTypeId 9</remarks>
        Public Property IRPM_BuildingFeaturesDesc As String
            Get
                Return _IRPM_BuildingFeaturesDesc
            End Get
            Set(value As String)
                _IRPM_BuildingFeaturesDesc = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 4 (IRPM) and RiskCharacteristicTypeId 2</remarks>
        Public Property IRPM_Premises As String
            Get
                Return _IRPM_Premises
            End Get
            Set(value As String)
                _IRPM_Premises = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 4 (IRPM) and RiskCharacteristicTypeId 2</remarks>
        Public Property IRPM_PremisesDesc As String
            Get
                Return _IRPM_PremisesDesc
            End Get
            Set(value As String)
                _IRPM_PremisesDesc = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 4 (IRPM) and RiskCharacteristicTypeId 4</remarks>
        Public Property IRPM_Employees As String
            Get
                Return _IRPM_Employees
            End Get
            Set(value As String)
                _IRPM_Employees = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 4 (IRPM) and RiskCharacteristicTypeId 4</remarks>
        Public Property IRPM_EmployeesDesc As String
            Get
                Return _IRPM_EmployeesDesc
            End Get
            Set(value As String)
                _IRPM_EmployeesDesc = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 4 (IRPM) and RiskCharacteristicTypeId 12</remarks>
        Public Property IRPM_Protection As String
            Get
                Return _IRPM_Protection
            End Get
            Set(value As String)
                _IRPM_Protection = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 4 (IRPM) and RiskCharacteristicTypeId 12</remarks>
        Public Property IRPM_ProtectionDesc As String
            Get
                Return _IRPM_ProtectionDesc
            End Get
            Set(value As String)
                _IRPM_ProtectionDesc = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 4 (IRPM) and RiskCharacteristicTypeId 15</remarks>
        Public Property IRPM_CatostrophicHazards As String
            Get
                Return _IRPM_CatostrophicHazards
            End Get
            Set(value As String)
                _IRPM_CatostrophicHazards = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 4 (IRPM) and RiskCharacteristicTypeId 15</remarks>
        Public Property IRPM_CatostrophicHazardsDesc As String
            Get
                Return _IRPM_CatostrophicHazardsDesc
            End Get
            Set(value As String)
                _IRPM_CatostrophicHazardsDesc = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 4 (IRPM) and RiskCharacteristicTypeId 16</remarks>
        Public Property IRPM_ManagementExperience As String
            Get
                Return _IRPM_ManagementExperience
            End Get
            Set(value As String)
                _IRPM_ManagementExperience = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 4 (IRPM) and RiskCharacteristicTypeId 16</remarks>
        Public Property IRPM_ManagementExperienceDesc As String
            Get
                Return _IRPM_ManagementExperienceDesc
            End Get
            Set(value As String)
                _IRPM_ManagementExperienceDesc = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 4 (IRPM) and RiskCharacteristicTypeId 3</remarks>
        Public Property IRPM_Equipment As String
            Get
                Return _IRPM_Equipment
            End Get
            Set(value As String)
                _IRPM_Equipment = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 4 (IRPM) and RiskCharacteristicTypeId 3</remarks>
        Public Property IRPM_EquipmentDesc As String
            Get
                Return _IRPM_EquipmentDesc
            End Get
            Set(value As String)
                _IRPM_EquipmentDesc = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 4 (IRPM) and RiskCharacteristicTypeId 19</remarks>
        Public Property IRPM_MedicalFacilities As String
            Get
                Return _IRPM_MedicalFacilities
            End Get
            Set(value As String)
                _IRPM_MedicalFacilities = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 4 (IRPM) and RiskCharacteristicTypeId 19</remarks>
        Public Property IRPM_MedicalFacilitiesDesc As String
            Get
                Return _IRPM_MedicalFacilitiesDesc
            End Get
            Set(value As String)
                _IRPM_MedicalFacilitiesDesc = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 4 (IRPM) and RiskCharacteristicTypeId 17</remarks>
        Public Property IRPM_ClassificationPeculiarities As String
            Get
                Return _IRPM_ClassificationPeculiarities
            End Get
            Set(value As String)
                _IRPM_ClassificationPeculiarities = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 4 (IRPM) and RiskCharacteristicTypeId 17</remarks>
        Public Property IRPM_ClassificationPeculiaritiesDesc As String
            Get
                Return _IRPM_ClassificationPeculiaritiesDesc
            End Get
            Set(value As String)
                _IRPM_ClassificationPeculiaritiesDesc = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 5 (Premises) or 6 (Products) and RiskCharacteristicTypeId 14</remarks>
        Public Property IRPM_GL_ManagementCooperation As String
            Get
                Return _IRPM_GL_ManagementCooperation
            End Get
            Set(value As String)
                _IRPM_GL_ManagementCooperation = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 5 (Premises) or 6 (Products) and RiskCharacteristicTypeId 14</remarks>
        Public Property IRPM_GL_ManagementCooperationDesc As String
            Get
                Return _IRPM_GL_ManagementCooperationDesc
            End Get
            Set(value As String)
                _IRPM_GL_ManagementCooperationDesc = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 5 (Premises) or 6 (Products) and RiskCharacteristicTypeId 1</remarks>
        Public Property IRPM_GL_Location As String
            Get
                Return _IRPM_GL_Location
            End Get
            Set(value As String)
                _IRPM_GL_Location = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 5 (Premises) or 6 (Products) and RiskCharacteristicTypeId 1</remarks>
        Public Property IRPM_GL_LocationDesc As String
            Get
                Return _IRPM_GL_LocationDesc
            End Get
            Set(value As String)
                _IRPM_GL_LocationDesc = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 5 (Premises) or 6 (Products) and RiskCharacteristicTypeId 2</remarks>
        Public Property IRPM_GL_Premises As String
            Get
                Return _IRPM_GL_Premises
            End Get
            Set(value As String)
                _IRPM_GL_Premises = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 5 (Premises) or 6 (Products) and RiskCharacteristicTypeId 2</remarks>
        Public Property IRPM_GL_PremisesDesc As String
            Get
                Return _IRPM_GL_PremisesDesc
            End Get
            Set(value As String)
                _IRPM_GL_PremisesDesc = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 5 (Premises) or 6 (Products) and RiskCharacteristicTypeId 3</remarks>
        Public Property IRPM_GL_Equipment As String
            Get
                Return _IRPM_GL_Equipment
            End Get
            Set(value As String)
                _IRPM_GL_Equipment = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 5 (Premises) or 6 (Products) and RiskCharacteristicTypeId 3</remarks>
        Public Property IRPM_GL_EquipmentDesc As String
            Get
                Return _IRPM_GL_EquipmentDesc
            End Get
            Set(value As String)
                _IRPM_GL_EquipmentDesc = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 5 (Premises) or 6 (Products) and RiskCharacteristicTypeId 4</remarks>
        Public Property IRPM_GL_Employees As String
            Get
                Return _IRPM_GL_Employees
            End Get
            Set(value As String)
                _IRPM_GL_Employees = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 5 (Premises) or 6 (Products) and RiskCharacteristicTypeId 4</remarks>
        Public Property IRPM_GL_EmployeesDesc As String
            Get
                Return _IRPM_GL_EmployeesDesc
            End Get
            Set(value As String)
                _IRPM_GL_EmployeesDesc = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 5 (Premises) or 6 (Products) and RiskCharacteristicTypeId 17</remarks>
        Public Property IRPM_GL_ClassificationPeculiarities As String
            Get
                Return _IRPM_GL_ClassificationPeculiarities
            End Get
            Set(value As String)
                _IRPM_GL_ClassificationPeculiarities = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 5 (Premises) or 6 (Products) and RiskCharacteristicTypeId 17</remarks>
        Public Property IRPM_GL_ClassificationPeculiaritiesDesc As String
            Get
                Return _IRPM_GL_ClassificationPeculiaritiesDesc
            End Get
            Set(value As String)
                _IRPM_GL_ClassificationPeculiaritiesDesc = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 1 (Liability) and RiskCharacteristicTypeId 5</remarks>
        Public Property IRPM_CAP_Management As String
            Get
                Return _IRPM_CAP_Management
            End Get
            Set(value As String)
                _IRPM_CAP_Management = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 1 (Liability) and RiskCharacteristicTypeId 5</remarks>
        Public Property IRPM_CAP_ManagementDesc As String
            Get
                Return _IRPM_CAP_ManagementDesc
            End Get
            Set(value As String)
                _IRPM_CAP_ManagementDesc = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 1 (Liability) and RiskCharacteristicTypeId 4</remarks>
        Public Property IRPM_CAP_Employees As String
            Get
                Return _IRPM_CAP_Employees
            End Get
            Set(value As String)
                _IRPM_CAP_Employees = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 1 (Liability) and RiskCharacteristicTypeId 4</remarks>
        Public Property IRPM_CAP_EmployeesDesc As String
            Get
                Return _IRPM_CAP_EmployeesDesc
            End Get
            Set(value As String)
                _IRPM_CAP_EmployeesDesc = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 1 (Liability) and RiskCharacteristicTypeId 3</remarks>
        Public Property IRPM_CAP_Equipment As String
            Get
                Return _IRPM_CAP_Equipment
            End Get
            Set(value As String)
                _IRPM_CAP_Equipment = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 1 (Liability) and RiskCharacteristicTypeId 3</remarks>
        Public Property IRPM_CAP_EquipmentDesc As String
            Get
                Return _IRPM_CAP_EquipmentDesc
            End Get
            Set(value As String)
                _IRPM_CAP_EquipmentDesc = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 1 (Liability) and RiskCharacteristicTypeId 13</remarks>
        Public Property IRPM_CAP_SafetyOrganization As String
            Get
                Return _IRPM_CAP_SafetyOrganization
            End Get
            Set(value As String)
                _IRPM_CAP_SafetyOrganization = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 1 (Liability) and RiskCharacteristicTypeId 13</remarks>
        Public Property IRPM_CAP_SafetyOrganizationDesc As String
            Get
                Return _IRPM_CAP_SafetyOrganizationDesc
            End Get
            Set(value As String)
                _IRPM_CAP_SafetyOrganizationDesc = value
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 2 (Physical Damage) and RiskCharacteristicTypeId 5</remarks>
        Public Property IRPM_CAP_Management_Phys_Damage As String
            Get
                Return _IRPM_CAP_Management_Phys_Damage
            End Get
            Set(value As String)
                _IRPM_CAP_Management_Phys_Damage = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 2 (Physical Damage) and RiskCharacteristicTypeId 5</remarks>
        Public Property IRPM_CAP_ManagementDesc_Phys_Damage As String
            Get
                Return _IRPM_CAP_ManagementDesc_Phys_Damage
            End Get
            Set(value As String)
                _IRPM_CAP_ManagementDesc_Phys_Damage = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 2 (Physical Damage) and RiskCharacteristicTypeId 4</remarks>
        Public Property IRPM_CAP_Employees_Phys_Damage As String
            Get
                Return _IRPM_CAP_Employees_Phys_Damage
            End Get
            Set(value As String)
                _IRPM_CAP_Employees_Phys_Damage = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 2 (Physical Damage) and RiskCharacteristicTypeId 4</remarks>
        Public Property IRPM_CAP_EmployeesDesc_Phys_Damage As String
            Get
                Return _IRPM_CAP_EmployeesDesc_Phys_Damage
            End Get
            Set(value As String)
                _IRPM_CAP_EmployeesDesc_Phys_Damage = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 2 (Physical Damage) and RiskCharacteristicTypeId 3</remarks>
        Public Property IRPM_CAP_Equipment_Phys_Damage As String
            Get
                Return _IRPM_CAP_Equipment_Phys_Damage
            End Get
            Set(value As String)
                _IRPM_CAP_Equipment_Phys_Damage = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 2 (Physical Damage) and RiskCharacteristicTypeId 3</remarks>
        Public Property IRPM_CAP_EquipmentDesc_Phys_Damage As String
            Get
                Return _IRPM_CAP_EquipmentDesc_Phys_Damage
            End Get
            Set(value As String)
                _IRPM_CAP_EquipmentDesc_Phys_Damage = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 2 (Physical Damage) and RiskCharacteristicTypeId 13</remarks>
        Public Property IRPM_CAP_SafetyOrganization_Phys_Damage As String
            Get
                Return _IRPM_CAP_SafetyOrganization_Phys_Damage
            End Get
            Set(value As String)
                _IRPM_CAP_SafetyOrganization_Phys_Damage = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 2 (Physical Damage) and RiskCharacteristicTypeId 13</remarks>
        Public Property IRPM_CAP_SafetyOrganizationDesc_Phys_Damage As String
            Get
                Return _IRPM_CAP_SafetyOrganizationDesc_Phys_Damage
            End Get
            Set(value As String)
                _IRPM_CAP_SafetyOrganizationDesc_Phys_Damage = value
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 4 (IRPM) and RiskCharacteristicTypeId 5</remarks>
        Public Property IRPM_CPR_Management As String
            Get
                Return _IRPM_CPR_Management
            End Get
            Set(value As String)
                _IRPM_CPR_Management = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 4 (IRPM) and RiskCharacteristicTypeId 5</remarks>
        Public Property IRPM_CPR_ManagementDesc As String
            Get
                Return _IRPM_CPR_ManagementDesc
            End Get
            Set(value As String)
                _IRPM_CPR_ManagementDesc = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 4 (IRPM) and RiskCharacteristicTypeId 24</remarks>
        Public Property IRPM_CPR_PremisesAndEquipment As String
            Get
                Return _IRPM_CPR_PremisesAndEquipment
            End Get
            Set(value As String)
                _IRPM_CPR_PremisesAndEquipment = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond ScheduledRating w/ ScheduleRatingTypeId 4 (IRPM) and RiskCharacteristicTypeId 24</remarks>
        Public Property IRPM_CPR_PremisesAndEquipmentDesc As String
            Get
                Return _IRPM_CPR_PremisesAndEquipmentDesc
            End Get
            Set(value As String)
                _IRPM_CPR_PremisesAndEquipmentDesc = value
            End Set
        End Property
        Public Property IRPM_FAR_CareConditionOfEquipPremises As String
            Get
                Return _IRPM_FAR_CareConditionOfEquipPremises
            End Get
            Set(value As String)
                _IRPM_FAR_CareConditionOfEquipPremises = value
            End Set
        End Property
        Public Property IRPM_FAR_CareConditionOfEquipPremisesDesc As String
            Get
                Return _IRPM_FAR_CareConditionOfEquipPremisesDesc
            End Get
            Set(value As String)
                _IRPM_FAR_CareConditionOfEquipPremisesDesc = value
            End Set
        End Property
        Public Property IRPM_FAR_Cooperation As String
            Get
                Return _IRPM_FAR_Cooperation
            End Get
            Set(value As String)
                _IRPM_FAR_Cooperation = value
            End Set
        End Property
        Public Property IRPM_FAR_CooperationDesc As String
            Get
                Return _IRPM_FAR_CooperationDesc
            End Get
            Set(value As String)
                _IRPM_FAR_CooperationDesc = value
            End Set
        End Property
        Public Property IRPM_FAR_DamageSusceptibility As String
            Get
                Return _IRPM_FAR_DamageSusceptibility
            End Get
            Set(value As String)
                _IRPM_FAR_DamageSusceptibility = value
            End Set
        End Property
        Public Property IRPM_FAR_DamageSusceptibilityDesc As String
            Get
                Return _IRPM_FAR_DamageSusceptibilityDesc
            End Get
            Set(value As String)
                _IRPM_FAR_DamageSusceptibilityDesc = value
            End Set
        End Property
        Public Property IRPM_FAR_DispersionOrConcentration As String
            Get
                Return _IRPM_FAR_DispersionOrConcentration
            End Get
            Set(value As String)
                _IRPM_FAR_DispersionOrConcentration = value
            End Set
        End Property
        Public Property IRPM_FAR_DispersionOrConcentrationDesc As String
            Get
                Return _IRPM_FAR_DispersionOrConcentrationDesc
            End Get
            Set(value As String)
                _IRPM_FAR_DispersionOrConcentrationDesc = value
            End Set
        End Property
        Public Property IRPM_FAR_SuperiorOrInferiorStructureFeatures As String
            Get
                Return _IRPM_FAR_SuperiorOrInferiorStructureFeatures
            End Get
            Set(value As String)
                _IRPM_FAR_SuperiorOrInferiorStructureFeatures = value
            End Set
        End Property
        Public Property IRPM_FAR_SuperiorOrInferiorStructureFeaturesDesc As String
            Get
                Return _IRPM_FAR_SuperiorOrInferiorStructureFeaturesDesc
            End Get
            Set(value As String)
                _IRPM_FAR_SuperiorOrInferiorStructureFeaturesDesc = value
            End Set
        End Property
        Public Property IRPM_FAR_UseOfRiceHullsOrFlameRetardantBedding As String
            Get
                Return _IRPM_FAR_UseOfRiceHullsOrFlameRetardantBedding
            End Get
            Set(value As String)
                _IRPM_FAR_UseOfRiceHullsOrFlameRetardantBedding = value
            End Set
        End Property
        Public Property IRPM_FAR_UseOfRiceHullsOrFlameRetardantBeddingDesc As String
            Get
                Return _IRPM_FAR_UseOfRiceHullsOrFlameRetardantBeddingDesc
            End Get
            Set(value As String)
                _IRPM_FAR_UseOfRiceHullsOrFlameRetardantBeddingDesc = value
            End Set
        End Property
        Public Property IRPM_FAR_Location As String
            Get
                Return _IRPM_FAR_Location
            End Get
            Set(value As String)
                _IRPM_FAR_Location = value
            End Set
        End Property
        Public Property IRPM_FAR_LocationDesc As String
            Get
                Return _IRPM_FAR_LocationDesc
            End Get
            Set(value As String)
                _IRPM_FAR_LocationDesc = value
            End Set
        End Property
        Public Property IRPM_FAR_MiscProtectFeaturesOrHazards As String
            Get
                Return _IRPM_FAR_MiscProtectFeaturesOrHazards
            End Get
            Set(value As String)
                _IRPM_FAR_MiscProtectFeaturesOrHazards = value
            End Set
        End Property
        Public Property IRPM_FAR_MiscProtectFeaturesOrHazardsDesc As String
            Get
                Return _IRPM_FAR_MiscProtectFeaturesOrHazardsDesc
            End Get
            Set(value As String)
                _IRPM_FAR_MiscProtectFeaturesOrHazardsDesc = value
            End Set
        End Property
        Public Property IRPM_FAR_RoofCondition As String
            Get
                Return _IRPM_FAR_RoofCondition
            End Get
            Set(value As String)
                _IRPM_FAR_RoofCondition = value
            End Set
        End Property
        Public Property IRPM_FAR_RoofConditionDesc As String
            Get
                Return _IRPM_FAR_RoofConditionDesc
            End Get
            Set(value As String)
                _IRPM_FAR_RoofConditionDesc = value
            End Set
        End Property
        Public Property IRPM_FAR_StoragePracticesAndHazardousOperations As String
            Get
                Return _IRPM_FAR_StoragePracticesAndHazardousOperations
            End Get
            Set(value As String)
                _IRPM_FAR_StoragePracticesAndHazardousOperations = value
            End Set
        End Property
        Public Property IRPM_FAR_StoragePracticesAndHazardousOperationsDesc As String
            Get
                Return _IRPM_FAR_StoragePracticesAndHazardousOperationsDesc
            End Get
            Set(value As String)
                _IRPM_FAR_StoragePracticesAndHazardousOperationsDesc = value
            End Set
        End Property
        Public Property IRPM_FAR_PastLosses As String
            Get
                Return _IRPM_FAR_PastLosses
            End Get
            Set(value As String)
                _IRPM_FAR_PastLosses = value
            End Set
        End Property
        Public Property IRPM_FAR_PastLossesDesc As String
            Get
                Return _IRPM_FAR_PastLossesDesc
            End Get
            Set(value As String)
                _IRPM_FAR_PastLossesDesc = value
            End Set
        End Property
        Public Property IRPM_FAR_SupportingBusiness As String
            Get
                Return _IRPM_FAR_SupportingBusiness
            End Get
            Set(value As String)
                _IRPM_FAR_SupportingBusiness = value
            End Set
        End Property
        Public Property IRPM_FAR_SupportingBusinessDesc As String
            Get
                Return _IRPM_FAR_SupportingBusinessDesc
            End Get
            Set(value As String)
                _IRPM_FAR_SupportingBusinessDesc = value
            End Set
        End Property
        Public Property IRPM_FAR_RegularOnsiteInspections As String
            Get
                Return _IRPM_FAR_RegularOnsiteInspections
            End Get
            Set(value As String)
                _IRPM_FAR_RegularOnsiteInspections = value
            End Set
        End Property
        Public Property IRPM_FAR_RegularOnsiteInspectionsDesc As String
            Get
                Return _IRPM_FAR_RegularOnsiteInspectionsDesc
            End Get
            Set(value As String)
                _IRPM_FAR_RegularOnsiteInspectionsDesc = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverages w/ coveragecode_ids 21153 (Premises) and 21154 (Products)</remarks>
        Public Property GL_PremisesAndProducts_Deductible As String
            Get
                Return _GL_PremisesAndProducts_Deductible
            End Get
            Set(value As String)
                _GL_PremisesAndProducts_Deductible = value
                Select Case _GL_PremisesAndProducts_Deductible
                    Case "N/A"
                        _GL_PremisesAndProducts_DeductibleId = "0"
                    Case "250"
                        _GL_PremisesAndProducts_DeductibleId = "4"
                    Case "500"
                        _GL_PremisesAndProducts_DeductibleId = "8"
                    Case "750"
                        _GL_PremisesAndProducts_DeductibleId = "27"
                    Case "1,000"
                        _GL_PremisesAndProducts_DeductibleId = "9"
                    Case "2,000"
                        _GL_PremisesAndProducts_DeductibleId = "28"
                    Case "3,000"
                        _GL_PremisesAndProducts_DeductibleId = "29"
                    Case "4,000"
                        _GL_PremisesAndProducts_DeductibleId = "30"
                    Case "5,000"
                        _GL_PremisesAndProducts_DeductibleId = "16"
                    Case "10,000"
                        _GL_PremisesAndProducts_DeductibleId = "17"
                    Case "15,000"
                        _GL_PremisesAndProducts_DeductibleId = "31"
                    Case "20,000"
                        _GL_PremisesAndProducts_DeductibleId = "18"
                    Case "25,000"
                        _GL_PremisesAndProducts_DeductibleId = "19"
                    Case "50,000"
                        _GL_PremisesAndProducts_DeductibleId = "20"
                    Case "75,000"
                        _GL_PremisesAndProducts_DeductibleId = "21"
                    Case Else
                        _GL_PremisesAndProducts_DeductibleId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverages w/ coveragecode_ids 21153 (Premises) and 21154 (Products)</remarks>
        Public Property GL_PremisesAndProducts_DeductibleId As String
            Get
                Return _GL_PremisesAndProducts_DeductibleId
            End Get
            Set(value As String)
                _GL_PremisesAndProducts_DeductibleId = value
                _GL_PremisesAndProducts_Deductible = ""
                If IsNumeric(_GL_PremisesAndProducts_DeductibleId) = True Then
                    Select Case _GL_PremisesAndProducts_DeductibleId
                        Case "0"
                            _GL_PremisesAndProducts_Deductible = "N/A"
                        Case "4"
                            _GL_PremisesAndProducts_Deductible = "250"
                        Case "8"
                            _GL_PremisesAndProducts_Deductible = "500"
                        Case "27"
                            _GL_PremisesAndProducts_Deductible = "750"
                        Case "9"
                            _GL_PremisesAndProducts_Deductible = "1,000"
                        Case "28"
                            _GL_PremisesAndProducts_Deductible = "2,000"
                        Case "29"
                            _GL_PremisesAndProducts_Deductible = "3,000"
                        Case "30"
                            _GL_PremisesAndProducts_Deductible = "4,000"
                        Case "16"
                            _GL_PremisesAndProducts_Deductible = "5,000"
                        Case "17"
                            _GL_PremisesAndProducts_Deductible = "10,000"
                        Case "31"
                            _GL_PremisesAndProducts_Deductible = "15,000"
                        Case "18"
                            _GL_PremisesAndProducts_Deductible = "20,000"
                        Case "19"
                            _GL_PremisesAndProducts_Deductible = "25,000"
                        Case "20"
                            _GL_PremisesAndProducts_Deductible = "50,000"
                        Case "21"
                            _GL_PremisesAndProducts_Deductible = "75,000"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverages w/ coveragecode_ids 21153 (Premises) and 21154 (Products)</remarks>
        Public Property GL_PremisesAndProducts_Description As String
            Get
                Return _GL_PremisesAndProducts_Description
            End Get
            Set(value As String)
                _GL_PremisesAndProducts_Description = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverages w/ coveragecode_ids 21153 (Premises) and 21154 (Products)</remarks>
        Public Property GL_PremisesAndProducts_DeductibleCategoryType As String
            Get
                Return _GL_PremisesAndProducts_DeductibleCategoryType
            End Get
            Set(value As String)
                _GL_PremisesAndProducts_DeductibleCategoryType = value
                Select Case _GL_PremisesAndProducts_DeductibleCategoryType
                    Case "N/A"
                        _GL_PremisesAndProducts_DeductibleCategoryTypeId = "0"
                    Case "Bodily Injury"
                        _GL_PremisesAndProducts_DeductibleCategoryTypeId = "5"
                    Case "Property Damage"
                        _GL_PremisesAndProducts_DeductibleCategoryTypeId = "6"
                    Case "Bodily Injury & Property Damage"
                        _GL_PremisesAndProducts_DeductibleCategoryTypeId = "7"
                    Case Else
                        _GL_PremisesAndProducts_DeductibleCategoryTypeId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverages w/ coveragecode_ids 21153 (Premises) and 21154 (Products)</remarks>
        Public Property GL_PremisesAndProducts_DeductibleCategoryTypeId As String
            Get
                Return _GL_PremisesAndProducts_DeductibleCategoryTypeId
            End Get
            Set(value As String)
                _GL_PremisesAndProducts_DeductibleCategoryTypeId = value
                _GL_PremisesAndProducts_DeductibleCategoryType = ""
                If IsNumeric(_GL_PremisesAndProducts_DeductibleCategoryTypeId) = True Then
                    Select Case _GL_PremisesAndProducts_DeductibleCategoryTypeId
                        Case "0"
                            _GL_PremisesAndProducts_DeductibleCategoryType = "N/A"
                        Case "5"
                            _GL_PremisesAndProducts_DeductibleCategoryType = "Bodily Injury"
                        Case "6"
                            _GL_PremisesAndProducts_DeductibleCategoryType = "Property Damage"
                        Case "7"
                            _GL_PremisesAndProducts_DeductibleCategoryType = "Bodily Injury & Property Damage"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverages w/ coveragecode_ids 21153 (Premises) and 21154 (Products)</remarks>
        Public Property GL_PremisesAndProducts_DeductiblePerType As String
            Get
                Return _GL_PremisesAndProducts_DeductiblePerType
            End Get
            Set(value As String)
                _GL_PremisesAndProducts_DeductiblePerType = value
                Select Case _GL_PremisesAndProducts_DeductiblePerType
                    Case "N/A"
                        _GL_PremisesAndProducts_DeductiblePerTypeId = "0"
                    Case "Per Occurrence"
                        _GL_PremisesAndProducts_DeductiblePerTypeId = "1"
                    Case "Per Claim"
                        _GL_PremisesAndProducts_DeductiblePerTypeId = "2"
                    Case Else
                        _GL_PremisesAndProducts_DeductiblePerTypeId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverages w/ coveragecode_ids 21153 (Premises) and 21154 (Products)</remarks>
        Public Property GL_PremisesAndProducts_DeductiblePerTypeId As String
            Get
                Return _GL_PremisesAndProducts_DeductiblePerTypeId
            End Get
            Set(value As String)
                _GL_PremisesAndProducts_DeductiblePerTypeId = value
                '(0-N/A; 1=Per Occurrence; 2=Per Claim)
                _GL_PremisesAndProducts_DeductiblePerType = ""
                If IsNumeric(_GL_PremisesAndProducts_DeductiblePerTypeId) = True Then
                    Select Case _GL_PremisesAndProducts_DeductiblePerTypeId
                        Case "0"
                            _GL_PremisesAndProducts_DeductiblePerType = "N/A"
                        Case "1"
                            _GL_PremisesAndProducts_DeductiblePerType = "Per Occurrence"
                        Case "2"
                            _GL_PremisesAndProducts_DeductiblePerType = "Per Claim"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverages w/ coveragecode_ids 21153 (Premises) and 21154 (Products)</remarks>
        Public Property Has_GL_PremisesAndProducts As Boolean
            Get
                Return _Has_GL_PremisesAndProducts
            End Get
            Set(value As Boolean)
                _Has_GL_PremisesAndProducts = value
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond GLClassifications and coverages w/ coveragecode_id 80150; combines policy level w/ all locations</remarks>
        Public Property GL_PremisesTotalQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_GL_PremisesTotalQuotedPremium)
            End Get
            Set(value As String)
                _GL_PremisesTotalQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_GL_PremisesTotalQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond GLClassifications and coverages w/ coveragecode_id 80152; combines policy level w/ all locations</remarks>
        Public Property GL_ProductsTotalQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_GL_ProductsTotalQuotedPremium)
            End Get
            Set(value As String)
                _GL_ProductsTotalQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_GL_ProductsTotalQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond GLClassifications and coverages w/ coveragecode_id 80150</remarks>
        Public Property GL_PremisesPolicyLevelQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_GL_PremisesPolicyLevelQuotedPremium)
            End Get
            Set(value As String)
                _GL_PremisesPolicyLevelQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_GL_PremisesPolicyLevelQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond GLClassifications and coverages w/ coveragecode_id 80152</remarks>
        Public Property GL_ProductsPolicyLevelQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_GL_ProductsPolicyLevelQuotedPremium)
            End Get
            Set(value As String)
                _GL_ProductsPolicyLevelQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_GL_ProductsPolicyLevelQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80245 (Commercial GL Subline 334 - Policy [premises]); value for CoverageAdditionalInfoRecord w/ description of 'Minimum Premium'</remarks>
        Public Property GL_PremisesMinimumQuotedPremium As String
            Get
                If _GL_PremisesMinimumQuotedPremium = "" Then
                    qqHelper.ConvertToQuotedPremiumFormat(_GL_PremisesMinimumQuotedPremium, QuickQuoteHelperClass.QuickQuoteEmptyStringReplacementType.NonApplicable)
                End If
                Return _GL_PremisesMinimumQuotedPremium
            End Get
            Set(value As String)
                _GL_PremisesMinimumQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_GL_PremisesMinimumQuotedPremium, QuickQuoteHelperClass.QuickQuoteEmptyStringReplacementType.NonApplicable)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80245 (Commercial GL Subline 334 - Policy [premises]); value for CoverageAdditionalInfoRecord w/ description of 'Minimum Premium Adjustment'</remarks>
        Public Property GL_PremisesMinimumPremiumAdjustment As String
            Get
                If _GL_PremisesMinimumPremiumAdjustment = "" Then
                    qqHelper.ConvertToQuotedPremiumFormat(_GL_PremisesMinimumPremiumAdjustment, QuickQuoteHelperClass.QuickQuoteEmptyStringReplacementType.Zero)
                End If
                Return _GL_PremisesMinimumPremiumAdjustment
            End Get
            Set(value As String)
                _GL_PremisesMinimumPremiumAdjustment = value
                qqHelper.ConvertToQuotedPremiumFormat(_GL_PremisesMinimumPremiumAdjustment, QuickQuoteHelperClass.QuickQuoteEmptyStringReplacementType.Zero)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80247 (Commercial GL Subline 336 - Policy [products]); value for CoverageAdditionalInfoRecord w/ description of 'Minimum Premium'</remarks>
        Public Property GL_ProductsMinimumQuotedPremium As String
            Get
                If _GL_ProductsMinimumQuotedPremium = "" Then
                    qqHelper.ConvertToQuotedPremiumFormat(_GL_ProductsMinimumQuotedPremium, QuickQuoteHelperClass.QuickQuoteEmptyStringReplacementType.NonApplicable)
                End If
                Return _GL_ProductsMinimumQuotedPremium
            End Get
            Set(value As String)
                _GL_ProductsMinimumQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_GL_ProductsMinimumQuotedPremium, QuickQuoteHelperClass.QuickQuoteEmptyStringReplacementType.NonApplicable)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80247 (Commercial GL Subline 336 - Policy [products]); value for CoverageAdditionalInfoRecord w/ description of 'Minimum Premium Adjustment'</remarks>
        Public Property GL_ProductsMinimumPremiumAdjustment As String
            Get
                If _GL_ProductsMinimumPremiumAdjustment = "" Then
                    qqHelper.ConvertToQuotedPremiumFormat(_GL_ProductsMinimumPremiumAdjustment, QuickQuoteHelperClass.QuickQuoteEmptyStringReplacementType.Zero)
                End If
                Return _GL_ProductsMinimumPremiumAdjustment
            End Get
            Set(value As String)
                _GL_ProductsMinimumPremiumAdjustment = value
                qqHelper.ConvertToQuotedPremiumFormat(_GL_ProductsMinimumPremiumAdjustment, QuickQuoteHelperClass.QuickQuoteEmptyStringReplacementType.Zero)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 10125 (CAP)</remarks>
        Public Property HasFarmPollutionLiability As Boolean
            Get
                Return _HasFarmPollutionLiability
            End Get
            Set(value As Boolean)
                _HasFarmPollutionLiability = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 10125 (CAP)</remarks>
        Public Property FarmPollutionLiabilityQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_FarmPollutionLiabilityQuotedPremium)
            End Get
            Set(value As String)
                _FarmPollutionLiabilityQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_FarmPollutionLiabilityQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverages w/ coveragecode_ids 10066, 10062, 10063, 10064, or 10065</remarks>
        Public Property HasHiredBorrowedNonOwned As Boolean
            Get
                Return _HasHiredBorrowedNonOwned
            End Get
            Set(value As Boolean)
                _HasHiredBorrowedNonOwned = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 10066</remarks>
        Public Property HasNonOwnershipLiability As Boolean
            Get
                Return _HasNonOwnershipLiability
            End Get
            Set(value As Boolean)
                _HasNonOwnershipLiability = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 10066</remarks>
        Public Property NonOwnershipLiabilityNumberOfEmployees As String
            Get
                Return _NonOwnershipLiabilityNumberOfEmployees
            End Get
            Set(value As String)
                _NonOwnershipLiabilityNumberOfEmployees = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 10066</remarks>
        Public Property NonOwnership_ENO_RatingTypeId As String
            Get
                Return _NonOwnership_ENO_RatingTypeId
            End Get
            Set(value As String)
                _NonOwnership_ENO_RatingTypeId = value
                _NonOwnership_ENO_RatingType = ""
                If IsNumeric(_NonOwnership_ENO_RatingTypeId) = True Then
                    Select Case _NonOwnership_ENO_RatingTypeId
                        Case "0"
                            _NonOwnership_ENO_RatingType = "N/A"
                        Case "1"
                            _NonOwnership_ENO_RatingType = "Employees Only"
                        Case "2"
                            _NonOwnership_ENO_RatingType = "Volunteers Only"
                        Case "3"
                            _NonOwnership_ENO_RatingType = "Employees and Volunteers"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 10066</remarks>
        Public Property NonOwnership_ENO_RatingType As String
            Get
                Return _NonOwnership_ENO_RatingType
            End Get
            Set(value As String)
                _NonOwnership_ENO_RatingType = value
                Select Case _NonOwnership_ENO_RatingType
                    Case "N/A"
                        _NonOwnership_ENO_RatingTypeId = "0"
                    Case "Employees Only"
                        _NonOwnership_ENO_RatingTypeId = "1"
                    Case "Volunteers Only"
                        _NonOwnership_ENO_RatingTypeId = "2"
                    Case "Employees and Volunteers"
                        _NonOwnership_ENO_RatingTypeId = "3"
                    Case Else
                        _NonOwnership_ENO_RatingTypeId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 10066</remarks>
        Public Property NonOwnershipLiabilityQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_NonOwnershipLiabilityQuotedPremium)
            End Get
            Set(value As String)
                _NonOwnershipLiabilityQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_NonOwnershipLiabilityQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 10062; CoverageTypeId = 2 (Excess)</remarks>
        Public Property HasHiredBorrowedLiability As Boolean
            Get
                Return _HasHiredBorrowedLiability
            End Get
            Set(value As Boolean)
                _HasHiredBorrowedLiability = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 10062</remarks>
        Public Property HiredBorrowedLiabilityQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_HiredBorrowedLiabilityQuotedPremium)
            End Get
            Set(value As String)
                _HiredBorrowedLiabilityQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_HiredBorrowedLiabilityQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 10062 (would also use 10065 if present in response xml); IfAnyBasis</remarks>
        Public Property HasHiredCarPhysicalDamage As Boolean
            Get
                Return _HasHiredCarPhysicalDamage
            End Get
            Set(value As Boolean)
                _HasHiredCarPhysicalDamage = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 10065</remarks>
        Public Property HiredBorrowedLossOfUseQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_HiredBorrowedLossOfUseQuotedPremium)
            End Get
            Set(value As String)
                _HiredBorrowedLossOfUseQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_HiredBorrowedLossOfUseQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 10063; OtherThanCollisionTypeId = 3</remarks>
        Public Property ComprehensiveDeductible As String
            Get
                Return _ComprehensiveDeductible
            End Get
            Set(value As String)
                _ComprehensiveDeductible = value
                Select Case _ComprehensiveDeductible
                    Case "N/A"
                        _ComprehensiveDeductibleId = "0"
                    Case "Full"
                        _ComprehensiveDeductibleId = "10"
                    Case "50"
                        _ComprehensiveDeductibleId = "1"
                    Case "100"
                        _ComprehensiveDeductibleId = "2"
                    Case Else
                        _ComprehensiveDeductibleId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 10063; OtherThanCollisionTypeId = 3</remarks>
        Public Property ComprehensiveDeductibleId As String
            Get
                Return _ComprehensiveDeductibleId
            End Get
            Set(value As String)
                _ComprehensiveDeductibleId = value
                _ComprehensiveDeductible = ""
                If IsNumeric(_ComprehensiveDeductibleId) = True Then
                    Select Case _ComprehensiveDeductibleId
                        Case "0"
                            _ComprehensiveDeductible = "N/A"
                        Case "10"
                            _ComprehensiveDeductible = "Full"
                        Case "1"
                            _ComprehensiveDeductible = "50"
                        Case "2"
                            _ComprehensiveDeductible = "100"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 10063; OtherThanCollisionTypeId = 3</remarks>
        Public Property ComprehensiveQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_ComprehensiveQuotedPremium)
            End Get
            Set(value As String)
                _ComprehensiveQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_ComprehensiveQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 10064</remarks>
        Public Property CollisionDeductible As String
            Get
                Return _CollisionDeductible
            End Get
            Set(value As String)
                _CollisionDeductible = value
                Select Case _CollisionDeductible
                    Case "N/A"
                        _CollisionDeductibleId = "0"
                    Case "100"
                        _CollisionDeductibleId = "2"
                    Case "250"
                        _CollisionDeductibleId = "4"
                    Case "500"
                        _CollisionDeductibleId = "8"
                    Case "1,000"
                        _CollisionDeductibleId = "9"
                    Case Else
                        _CollisionDeductibleId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 10064</remarks>
        Public Property CollisionDeductibleId As String
            Get
                Return _CollisionDeductibleId
            End Get
            Set(value As String)
                _CollisionDeductibleId = value
                _CollisionDeductible = ""
                If IsNumeric(_CollisionDeductibleId) = True Then
                    Select Case _CollisionDeductibleId
                        Case "0"
                            _CollisionDeductible = "N/A"
                        Case "2"
                            _CollisionDeductible = "100"
                        Case "4"
                            _CollisionDeductible = "250"
                        Case "8"
                            _CollisionDeductible = "500"
                        Case "9"
                            _CollisionDeductible = "1,000"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 10064</remarks>
        Public Property CollisionQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_CollisionQuotedPremium)
            End Get
            Set(value As String)
                _CollisionQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_CollisionQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21552</remarks>
        Public Property Liability_UM_UIM_Limit As String '5/10/2017 note: added static data values to DiamondStaticData.xml (specific to CAP/GAR)
            Get
                Return _Liability_UM_UIM_Limit
            End Get
            Set(value As String)
                _Liability_UM_UIM_Limit = value
                Select Case _Liability_UM_UIM_Limit
                    Case "N/A"
                        _Liability_UM_UIM_LimitId = "0"
                    Case "60,000"
                        _Liability_UM_UIM_LimitId = "324"
                    Case "100,000"
                        _Liability_UM_UIM_LimitId = "10"
                    Case "250,000"
                        _Liability_UM_UIM_LimitId = "55"
                    Case "300,000"
                        _Liability_UM_UIM_LimitId = "33"
                    Case "350,000"
                        _Liability_UM_UIM_LimitId = "176"
                    Case "500,000"
                        _Liability_UM_UIM_LimitId = "34"
                    Case "750,000"
                        _Liability_UM_UIM_LimitId = "180"
                    Case "1,000,000"
                        _Liability_UM_UIM_LimitId = "56"
                    Case Else
                        _Liability_UM_UIM_LimitId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21552</remarks>
        Public Property Liability_UM_UIM_LimitId As String '5/10/2017 note: added static data values to DiamondStaticData.xml (specific to CAP/GAR)
            Get
                Return _Liability_UM_UIM_LimitId
            End Get
            Set(value As String)
                _Liability_UM_UIM_LimitId = value
                _Liability_UM_UIM_Limit = ""
                If IsNumeric(_Liability_UM_UIM_LimitId) = True Then
                    Select Case _Liability_UM_UIM_LimitId
                        Case "0"
                            _Liability_UM_UIM_Limit = "N/A"
                        Case "324"
                            _Liability_UM_UIM_Limit = "60,000"
                        Case "10"
                            _Liability_UM_UIM_Limit = "100,000"
                        Case "55"
                            _Liability_UM_UIM_Limit = "250,000"
                        Case "33"
                            _Liability_UM_UIM_Limit = "300,000"
                        Case "176"
                            _Liability_UM_UIM_Limit = "350,000"
                        Case "34"
                            _Liability_UM_UIM_Limit = "500,000"
                        Case "180"
                            _Liability_UM_UIM_Limit = "750,000"
                        Case "56"
                            _Liability_UM_UIM_Limit = "1,000,000"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21552</remarks>
        Public Property Liability_UM_UIM_QuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_Liability_UM_UIM_QuotedPremium)
            End Get
            Set(value As String)
                _Liability_UM_UIM_QuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_Liability_UM_UIM_QuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21540 (CAP) or 70072 (HOM and DFR)</remarks>
        Public Property MedicalPaymentsLimit As String '8/11/2014 note: need to update to use static data list... w/ param for lobType; '4/29/2015 note: FAR may now use the MedicalPaymentsLimitId property (unless we decide to create a new prop)... will need to update logic for FAR values and/or update logic to use the static data file; 5/11/2017 note: still need to update to use static data and send LOB
            Get
                Return _MedicalPaymentsLimit
            End Get
            Set(value As String)
                _MedicalPaymentsLimit = value
                Select Case _MedicalPaymentsLimit
                    Case "N/A"
                        _MedicalPaymentsLimitId = "0"
                    Case "1,000"
                        _MedicalPaymentsLimitId = "11"
                    Case "2,000"
                        _MedicalPaymentsLimitId = "12"
                    Case "5,000"
                        _MedicalPaymentsLimitId = "15"
                        'updated 7/30/2013 for HOM
                    Case "500" '5/11/2017 note: also good for DFR
                        _MedicalPaymentsLimitId = "166"
                    Case "1,000" '5/11/2017 note: also good for DFR, FAR
                        _MedicalPaymentsLimitId = "170"
                        'updated 5/11/2017 for GAR
                    Case "500"
                        _MedicalPaymentsLimitId = "113"
                    Case "750"
                        _MedicalPaymentsLimitId = "325"
                    Case "2,000"
                        _MedicalPaymentsLimitId = "326"
                        '5/11/2017 - updates for HOM, DFR, FAR
                    Case "2,000" 'HOM, DFR, FAR
                        _MedicalPaymentsLimitId = "171"
                    Case "3,000" 'HOM, DFR, FAR
                        _MedicalPaymentsLimitId = "13"
                    Case "4,000" 'HOM, DFR, FAR
                        _MedicalPaymentsLimitId = "14"
                    Case "5,000" 'HOM, DFR, FAR
                        _MedicalPaymentsLimitId = "173"
                        '5/11/2017 note: options for FAR that aren't currently shown: 6,000 = 307; 7,000 = 308; 8,000 = 309; 9,000 = 310
                    Case "10,000" 'FAR
                        _MedicalPaymentsLimit = "289"
                        '5/11/2017 note: options for FAR that aren't currently shown: 11,000 = 290; 12,000 = 291; 13,000 = 292; 14,000 = 293; 15,000 = 294; 16,000 = 295; 17,000 = 296; 18,000 = 297; 19,000 = 298; 20,000 = 299; 21,000 = 300; 22,000 = 301; 23,000 = 302; 24,000 = 303; 25,000 = 304
                    Case Else
                        _MedicalPaymentsLimitId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21540 (CAP) or 70072 (HOM and DFR)</remarks>
        Public Property MedicalPaymentsLimitid As String '8/11/2014 note: need to update to use static data list... w/ param for lobType; 4/29/2015 note: FAR may now use the MedicalPaymentsLimitId property (unless we decide to create a new prop)... will need to update logic for FAR values and/or update logic to use the static data file; 5/11/2017 note: still need to update to use static data and send LOB
            Get
                Return _MedicalPaymentsLimitId
            End Get
            Set(value As String)
                _MedicalPaymentsLimitId = value
                _MedicalPaymentsLimit = ""
                If IsNumeric(_MedicalPaymentsLimitId) = True Then
                    Select Case _MedicalPaymentsLimitId
                        Case "0"
                            _MedicalPaymentsLimit = "N/A"
                        Case "11"
                            _MedicalPaymentsLimit = "1,000"
                        Case "12"
                            _MedicalPaymentsLimit = "2,000"
                        Case "15"
                            _MedicalPaymentsLimit = "5,000"
                            'updated 7/30/2013 for HOM
                        Case "166" '5/11/2017 note: also good for DFR
                            _MedicalPaymentsLimit = "500"
                        Case "170" '5/11/2017 note: also good for DFR, FAR
                            _MedicalPaymentsLimit = "1,000"
                            'updated 5/11/2017 for GAR
                        Case "113"
                            _MedicalPaymentsLimit = "500"
                        Case "325"
                            _MedicalPaymentsLimit = "750"
                        Case "326"
                            _MedicalPaymentsLimit = "2,000"
                            '5/11/2017 - updates for HOM, DFR, FAR
                        Case "171" 'HOM, DFR, FAR
                            _MedicalPaymentsLimit = "2,000"
                        Case "13" 'HOM, DFR, FAR
                            _MedicalPaymentsLimit = "3,000"
                        Case "14" 'HOM, DFR, FAR
                            _MedicalPaymentsLimit = "4,000"
                        Case "173" 'HOM, DFR, FAR
                            _MedicalPaymentsLimit = "5,000"
                            '5/11/2017 note: options for FAR that aren't currently shown: 6,000 = 307; 7,000 = 308; 8,000 = 309; 9,000 = 310
                        Case "289" 'FAR
                            _MedicalPaymentsLimit = "10,000"
                            '5/11/2017 note: options for FAR that aren't currently shown: 11,000 = 290; 12,000 = 291; 13,000 = 292; 14,000 = 293; 15,000 = 294; 16,000 = 295; 17,000 = 296; 18,000 = 297; 19,000 = 298; 20,000 = 299; 21,000 = 300; 22,000 = 301; 23,000 = 302; 24,000 = 303; 25,000 = 304
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21540 (CAP) or 70072 (HOM and DFR)</remarks>
        Public Property MedicalPaymentsQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_MedicalPaymentsQuotedPremium)
            End Get
            Set(value As String)
                _MedicalPaymentsQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_MedicalPaymentsQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to modifier w/ Diamond ModifierTypeId 61 (Quote) or 62 (Issue/Bound)</remarks>
        Public Property QuoteOrIssueBound As QuickQuoteObject.QuickQuoteQuoteOrIssueBound
            Get
                Return _QuoteOrIssueBound
            End Get
            Set(value As QuickQuoteObject.QuickQuoteQuoteOrIssueBound)
                _QuoteOrIssueBound = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to modifier w/ Diamond ModifierTypeId 63 (Issue/Bound EffDate; goes w/ ModifierTypeId 62 - Issue/Bound)</remarks>
        Public Property IssueBoundEffectiveDate As String
            Get
                Return _IssueBoundEffectiveDate
            End Get
            Set(value As String)
                _IssueBoundEffectiveDate = value
                qqHelper.ConvertToShortDate(_IssueBoundEffectiveDate)
            End Set
        End Property
        Public Property LiabilityAutoSymbolObject As QuickQuoteDeveloperAutoSymbol
            Get
                Return _LiabilityAutoSymbolObject
            End Get
            Set(value As QuickQuoteDeveloperAutoSymbol)
                _LiabilityAutoSymbolObject = value
            End Set
        End Property
        Public Property MedicalPaymentsAutoSymbolObject As QuickQuoteDeveloperAutoSymbol
            Get
                Return _MedicalPaymentsAutoSymbolObject
            End Get
            Set(value As QuickQuoteDeveloperAutoSymbol)
                _MedicalPaymentsAutoSymbolObject = value
            End Set
        End Property
        Public Property UninsuredMotoristAutoSymbolObject As QuickQuoteDeveloperAutoSymbol
            Get
                Return _UninsuredMotoristAutoSymbolObject
            End Get
            Set(value As QuickQuoteDeveloperAutoSymbol)
                _UninsuredMotoristAutoSymbolObject = value
            End Set
        End Property
        Public Property UnderinsuredMotoristAutoSymbolObject As QuickQuoteDeveloperAutoSymbol
            Get
                Return _UnderinsuredMotoristAutoSymbolObject
            End Get
            Set(value As QuickQuoteDeveloperAutoSymbol)
                _UnderinsuredMotoristAutoSymbolObject = value
            End Set
        End Property
        Public Property ComprehensiveCoverageAutoSymbolObject As QuickQuoteDeveloperAutoSymbol
            Get
                Return _ComprehensiveCoverageAutoSymbolObject
            End Get
            Set(value As QuickQuoteDeveloperAutoSymbol)
                _ComprehensiveCoverageAutoSymbolObject = value
            End Set
        End Property
        Public Property CollisionCoverageAutoSymbolObject As QuickQuoteDeveloperAutoSymbol
            Get
                Return _CollisionCoverageAutoSymbolObject
            End Get
            Set(value As QuickQuoteDeveloperAutoSymbol)
                _CollisionCoverageAutoSymbolObject = value
            End Set
        End Property
        Public Property NonOwnershipAutoSymbolObject As QuickQuoteDeveloperAutoSymbol
            Get
                Return _NonOwnershipAutoSymbolObject
            End Get
            Set(value As QuickQuoteDeveloperAutoSymbol)
                _NonOwnershipAutoSymbolObject = value
            End Set
        End Property
        Public Property HiredBorrowedAutoSymbolObject As QuickQuoteDeveloperAutoSymbol
            Get
                Return _HiredBorrowedAutoSymbolObject
            End Get
            Set(value As QuickQuoteDeveloperAutoSymbol)
                _HiredBorrowedAutoSymbolObject = value
            End Set
        End Property
        Public Property TowingAndLaborAutoSymbolObject As QuickQuoteDeveloperAutoSymbol
            Get
                Return _TowingAndLaborAutoSymbolObject
            End Get
            Set(value As QuickQuoteDeveloperAutoSymbol)
                _TowingAndLaborAutoSymbolObject = value
            End Set
        End Property
        ''' <summary>
        ''' flag used to determine if developer auto symbols should be used instead of QuickQuoteAutoSymbols
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>always set to True when saveType is Quote instead of AppGap</remarks>
        Public Property UseDeveloperAutoSymbols As Boolean
            Get
                Return _UseDeveloperAutoSymbols
            End Get
            Set(value As Boolean)
                _UseDeveloperAutoSymbols = value
            End Set
        End Property
        Public Property CAP_Liability_WouldHaveSymbol8 As Boolean
            Get
                Return _CAP_Liability_WouldHaveSymbol8
            End Get
            Set(value As Boolean)
                _CAP_Liability_WouldHaveSymbol8 = value
            End Set
        End Property
        Public Property CAP_Liability_WouldHaveSymbol9 As Boolean
            Get
                Return _CAP_Liability_WouldHaveSymbol9
            End Get
            Set(value As Boolean)
                _CAP_Liability_WouldHaveSymbol9 = value
            End Set
        End Property
        Public Property CAP_Comprehensive_WouldHaveSymbol8 As Boolean
            Get
                Return _CAP_Comprehensive_WouldHaveSymbol8
            End Get
            Set(value As Boolean)
                _CAP_Comprehensive_WouldHaveSymbol8 = value
            End Set
        End Property
        Public Property CAP_Collision_WouldHaveSymbol8 As Boolean
            Get
                Return _CAP_Collision_WouldHaveSymbol8
            End Get
            Set(value As Boolean)
                _CAP_Collision_WouldHaveSymbol8 = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21082</remarks>
        Public Property HasBlanketBuilding As Boolean
            Get
                Return _HasBlanketBuilding
            End Get
            Set(value As Boolean)
                _HasBlanketBuilding = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21083</remarks>
        Public Property HasBlanketContents As Boolean
            Get
                Return _HasBlanketContents
            End Get
            Set(value As Boolean)
                _HasBlanketContents = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21084</remarks>
        Public Property HasBlanketBuildingAndContents As Boolean
            Get
                Return _HasBlanketBuildingAndContents
            End Get
            Set(value As Boolean)
                _HasBlanketBuildingAndContents = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21085</remarks>
        Public Property HasBlanketBusinessIncome As Boolean
            Get
                Return _HasBlanketBusinessIncome
            End Get
            Set(value As Boolean)
                _HasBlanketBusinessIncome = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21082</remarks>
        Public Property BlanketBuildingQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_BlanketBuildingQuotedPremium)
            End Get
            Set(value As String)
                _BlanketBuildingQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_BlanketBuildingQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21083</remarks>
        Public Property BlanketContentsQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_BlanketContentsQuotedPremium)
            End Get
            Set(value As String)
                _BlanketContentsQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_BlanketContentsQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21084</remarks>
        Public Property BlanketBuildingAndContentsQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_BlanketBuildingAndContentsQuotedPremium)
            End Get
            Set(value As String)
                _BlanketBuildingAndContentsQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_BlanketBuildingAndContentsQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21085</remarks>
        Public Property BlanketBusinessIncomeQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_BlanketBusinessIncomeQuotedPremium)
            End Get
            Set(value As String)
                _BlanketBusinessIncomeQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_BlanketBusinessIncomeQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21082</remarks>
        Public Property BlanketBuildingCauseOfLossTypeId As String
            Get
                Return _BlanketBuildingCauseOfLossTypeId
            End Get
            Set(value As String)
                _BlanketBuildingCauseOfLossTypeId = value
                _BlanketBuildingCauseOfLossType = ""
                If IsNumeric(_BlanketBuildingCauseOfLossTypeId) = True Then
                    Select Case _BlanketBuildingCauseOfLossTypeId
                        Case "0"
                            _BlanketBuildingCauseOfLossType = "N/A"
                        Case "1"
                            _BlanketBuildingCauseOfLossType = "Basic Form"
                        Case "2"
                            _BlanketBuildingCauseOfLossType = "Broad Form"
                        Case "3"
                            _BlanketBuildingCauseOfLossType = "Special Form Including Theft"
                        Case "4"
                            _BlanketBuildingCauseOfLossType = "Special Form Excluding Theft"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21082</remarks>
        Public Property BlanketBuildingCauseOfLossType As String
            Get
                Return _BlanketBuildingCauseOfLossType
            End Get
            Set(value As String)
                _BlanketBuildingCauseOfLossType = value
                Select Case _BlanketBuildingCauseOfLossType
                    Case "N/A"
                        _BlanketBuildingCauseOfLossTypeId = "0"
                    Case "Basic Form"
                        _BlanketBuildingCauseOfLossTypeId = "1"
                    Case "Broad Form"
                        _BlanketBuildingCauseOfLossTypeId = "2"
                    Case "Special Form Including Theft"
                        _BlanketBuildingCauseOfLossTypeId = "3"
                    Case "Special Form Excluding Theft"
                        _BlanketBuildingCauseOfLossTypeId = "4"
                    Case Else
                        _BlanketBuildingCauseOfLossTypeId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21083</remarks>
        Public Property BlanketContentsCauseOfLossTypeId As String
            Get
                Return _BlanketContentsCauseOfLossTypeId
            End Get
            Set(value As String)
                _BlanketContentsCauseOfLossTypeId = value
                _BlanketContentsCauseOfLossType = ""
                If IsNumeric(_BlanketContentsCauseOfLossTypeId) = True Then
                    Select Case _BlanketContentsCauseOfLossTypeId
                        Case "0"
                            _BlanketContentsCauseOfLossType = "N/A"
                        Case "1"
                            _BlanketContentsCauseOfLossType = "Basic Form"
                        Case "2"
                            _BlanketContentsCauseOfLossType = "Broad Form"
                        Case "3"
                            _BlanketContentsCauseOfLossType = "Special Form Including Theft"
                        Case "4"
                            _BlanketContentsCauseOfLossType = "Special Form Excluding Theft"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21083</remarks>
        Public Property BlanketContentsCauseOfLossType As String
            Get
                Return _BlanketContentsCauseOfLossType
            End Get
            Set(value As String)
                _BlanketContentsCauseOfLossType = value
                Select Case _BlanketContentsCauseOfLossType
                    Case "N/A"
                        _BlanketContentsCauseOfLossTypeId = "0"
                    Case "Basic Form"
                        _BlanketContentsCauseOfLossTypeId = "1"
                    Case "Broad Form"
                        _BlanketContentsCauseOfLossTypeId = "2"
                    Case "Special Form Including Theft"
                        _BlanketContentsCauseOfLossTypeId = "3"
                    Case "Special Form Excluding Theft"
                        _BlanketContentsCauseOfLossTypeId = "4"
                    Case Else
                        _BlanketContentsCauseOfLossTypeId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21084</remarks>
        Public Property BlanketBuildingAndContentsCauseOfLossTypeId As String
            Get
                Return _BlanketBuildingAndContentsCauseOfLossTypeId
            End Get
            Set(value As String)
                _BlanketBuildingAndContentsCauseOfLossTypeId = value
                _BlanketBuildingAndContentsCauseOfLossType = ""
                If IsNumeric(_BlanketBuildingAndContentsCauseOfLossTypeId) = True Then
                    Select Case _BlanketBuildingAndContentsCauseOfLossTypeId
                        Case "0"
                            _BlanketBuildingAndContentsCauseOfLossType = "N/A"
                        Case "1"
                            _BlanketBuildingAndContentsCauseOfLossType = "Basic Form"
                        Case "2"
                            _BlanketBuildingAndContentsCauseOfLossType = "Broad Form"
                        Case "3"
                            _BlanketBuildingAndContentsCauseOfLossType = "Special Form Including Theft"
                        Case "4"
                            _BlanketBuildingAndContentsCauseOfLossType = "Special Form Excluding Theft"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21084</remarks>
        Public Property BlanketBuildingAndContentsCauseOfLossType As String
            Get
                Return _BlanketBuildingAndContentsCauseOfLossType
            End Get
            Set(value As String)
                _BlanketBuildingAndContentsCauseOfLossType = value
                Select Case _BlanketBuildingAndContentsCauseOfLossType
                    Case "N/A"
                        _BlanketBuildingAndContentsCauseOfLossTypeId = "0"
                    Case "Basic Form"
                        _BlanketBuildingAndContentsCauseOfLossTypeId = "1"
                    Case "Broad Form"
                        _BlanketBuildingAndContentsCauseOfLossTypeId = "2"
                    Case "Special Form Including Theft"
                        _BlanketBuildingAndContentsCauseOfLossTypeId = "3"
                    Case "Special Form Excluding Theft"
                        _BlanketBuildingAndContentsCauseOfLossTypeId = "4"
                    Case Else
                        _BlanketBuildingAndContentsCauseOfLossTypeId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21085</remarks>
        Public Property BlanketBusinessIncomeCauseOfLossTypeId As String
            Get
                Return _BlanketBusinessIncomeCauseOfLossTypeId
            End Get
            Set(value As String)
                _BlanketBusinessIncomeCauseOfLossTypeId = value
                _BlanketBusinessIncomeCauseOfLossType = ""
                If IsNumeric(_BlanketBusinessIncomeCauseOfLossTypeId) = True Then
                    Select Case _BlanketBusinessIncomeCauseOfLossTypeId
                        Case "0"
                            _BlanketBusinessIncomeCauseOfLossType = "N/A"
                        Case "1"
                            _BlanketBusinessIncomeCauseOfLossType = "Basic Form"
                        Case "2"
                            _BlanketBusinessIncomeCauseOfLossType = "Broad Form"
                        Case "3"
                            _BlanketBusinessIncomeCauseOfLossType = "Special Form Including Theft"
                        Case "4"
                            _BlanketBusinessIncomeCauseOfLossType = "Special Form Excluding Theft"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21085</remarks>
        Public Property BlanketBusinessIncomeCauseOfLossType As String
            Get
                Return _BlanketBusinessIncomeCauseOfLossType
            End Get
            Set(value As String)
                _BlanketBusinessIncomeCauseOfLossType = value
                Select Case _BlanketBusinessIncomeCauseOfLossType
                    Case "N/A"
                        _BlanketBusinessIncomeCauseOfLossTypeId = "0"
                    Case "Basic Form"
                        _BlanketBusinessIncomeCauseOfLossTypeId = "1"
                    Case "Broad Form"
                        _BlanketBusinessIncomeCauseOfLossTypeId = "2"
                    Case "Special Form Including Theft"
                        _BlanketBusinessIncomeCauseOfLossTypeId = "3"
                    Case "Special Form Excluding Theft"
                        _BlanketBusinessIncomeCauseOfLossTypeId = "4"
                    Case Else
                        _BlanketBusinessIncomeCauseOfLossTypeId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21082</remarks>
        Public Property BlanketBuildingLimit As String
            Get
                Return _BlanketBuildingLimit
            End Get
            Set(value As String)
                _BlanketBuildingLimit = value
                qqHelper.ConvertToLimitFormat(_BlanketBuildingLimit)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21082</remarks>
        Public Property BlanketBuildingCoinsuranceTypeId As String
            Get
                Return _BlanketBuildingCoinsuranceTypeId
            End Get
            Set(value As String)
                _BlanketBuildingCoinsuranceTypeId = value
                _BlanketBuildingCoinsuranceType = ""
                If IsNumeric(_BlanketBuildingCoinsuranceTypeId) = True Then
                    Select Case _BlanketBuildingCoinsuranceTypeId
                        Case "0"
                            _BlanketBuildingCoinsuranceType = "N/A"
                        Case "1"
                            _BlanketBuildingCoinsuranceType = "Waived"
                        Case "2"
                            _BlanketBuildingCoinsuranceType = "50%"
                        Case "3"
                            _BlanketBuildingCoinsuranceType = "60%"
                        Case "4"
                            _BlanketBuildingCoinsuranceType = "70%"
                        Case "5"
                            _BlanketBuildingCoinsuranceType = "80%"
                        Case "6"
                            _BlanketBuildingCoinsuranceType = "90%"
                        Case "7"
                            _BlanketBuildingCoinsuranceType = "100%"
                        Case "8"
                            _BlanketBuildingCoinsuranceType = "10%"
                        Case "9"
                            _BlanketBuildingCoinsuranceType = "20%"
                        Case "10"
                            _BlanketBuildingCoinsuranceType = "30%"
                        Case "11"
                            _BlanketBuildingCoinsuranceType = "40%"
                        Case "12"
                            _BlanketBuildingCoinsuranceType = "125%"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21082</remarks>
        Public Property BlanketBuildingCoinsuranceType As String
            Get
                Return _BlanketBuildingCoinsuranceType
            End Get
            Set(value As String)
                _BlanketBuildingCoinsuranceType = value
                Select Case _BlanketBuildingCoinsuranceType
                    Case "N/A"
                        _BlanketBuildingCoinsuranceTypeId = "0"
                    Case "Waived"
                        _BlanketBuildingCoinsuranceTypeId = "1"
                    Case "50%"
                        _BlanketBuildingCoinsuranceTypeId = "2"
                    Case "60%"
                        _BlanketBuildingCoinsuranceTypeId = "3"
                    Case "70%"
                        _BlanketBuildingCoinsuranceTypeId = "4"
                    Case "80%"
                        _BlanketBuildingCoinsuranceTypeId = "5"
                    Case "90%"
                        _BlanketBuildingCoinsuranceTypeId = "6"
                    Case "100%"
                        _BlanketBuildingCoinsuranceTypeId = "7"
                    Case "10%"
                        _BlanketBuildingCoinsuranceTypeId = "8"
                    Case "20%"
                        _BlanketBuildingCoinsuranceTypeId = "9"
                    Case "30%"
                        _BlanketBuildingCoinsuranceTypeId = "10"
                    Case "40%"
                        _BlanketBuildingCoinsuranceTypeId = "11"
                    Case "125%"
                        _BlanketBuildingCoinsuranceTypeId = "12"
                    Case Else
                        _BlanketBuildingCoinsuranceTypeId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21082</remarks>
        Public Property BlanketBuildingValuationId As String
            Get
                Return _BlanketBuildingValuationId
            End Get
            Set(value As String)
                _BlanketBuildingValuationId = value
                '(1=Replacement Cost; 2=Actual Cash Value; 3=Functional Building Valuation)
                _BlanketBuildingValuation = ""
                If IsNumeric(_BlanketBuildingValuationId) = True Then
                    Select Case _BlanketBuildingValuationId
                        Case "-1"
                            _BlanketBuildingValuation = "N/A"
                        Case "1"
                            _BlanketBuildingValuation = "Replacement Cost"
                        Case "2"
                            _BlanketBuildingValuation = "Actual Cash Value"
                        Case "3"
                            _BlanketBuildingValuation = "Functional Building Valuation"
                        Case "7"
                            _BlanketBuildingValuation = "Functional Replacement Cost"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21082</remarks>
        Public Property BlanketBuildingValuation As String
            Get
                Return _BlanketBuildingValuation
            End Get
            Set(value As String)
                _BlanketBuildingValuation = value
                Select Case _BlanketBuildingValuation
                    Case "N/A"
                        _BlanketBuildingValuationId = "-1"
                    Case "Replacement Cost"
                        _BlanketBuildingValuationId = "1"
                    Case "Actual Cash Value"
                        _BlanketBuildingValuationId = "2"
                    Case "Functional Building Valuation"
                        _BlanketBuildingValuationId = "3"
                    Case "Functional Replacement Cost"
                        _BlanketBuildingValuationId = "7"
                    Case Else
                        _BlanketBuildingValuationId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21082</remarks>
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property BlanketBuildingDeductibleID As String
            Get
                Return _BlanketBuildingDeductibleID
            End Get
            Set(value As String)
                _BlanketBuildingDeductibleID = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21083</remarks>
        Public Property BlanketContentsLimit As String
            Get
                Return _BlanketContentsLimit
            End Get
            Set(value As String)
                _BlanketContentsLimit = value
                qqHelper.ConvertToLimitFormat(_BlanketContentsLimit)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21083</remarks>
        Public Property BlanketContentsCoinsuranceTypeId As String
            Get
                Return _BlanketContentsCoinsuranceTypeId
            End Get
            Set(value As String)
                _BlanketContentsCoinsuranceTypeId = value
                _BlanketContentsCoinsuranceType = ""
                If IsNumeric(_BlanketContentsCoinsuranceTypeId) = True Then
                    Select Case _BlanketContentsCoinsuranceTypeId
                        Case "0"
                            _BlanketContentsCoinsuranceType = "N/A"
                        Case "1"
                            _BlanketContentsCoinsuranceType = "Waived"
                        Case "2"
                            _BlanketContentsCoinsuranceType = "50%"
                        Case "3"
                            _BlanketContentsCoinsuranceType = "60%"
                        Case "4"
                            _BlanketContentsCoinsuranceType = "70%"
                        Case "5"
                            _BlanketContentsCoinsuranceType = "80%"
                        Case "6"
                            _BlanketContentsCoinsuranceType = "90%"
                        Case "7"
                            _BlanketContentsCoinsuranceType = "100%"
                        Case "8"
                            _BlanketContentsCoinsuranceType = "10%"
                        Case "9"
                            _BlanketContentsCoinsuranceType = "20%"
                        Case "10"
                            _BlanketContentsCoinsuranceType = "30%"
                        Case "11"
                            _BlanketContentsCoinsuranceType = "40%"
                        Case "12"
                            _BlanketContentsCoinsuranceType = "125%"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21083</remarks>
        Public Property BlanketContentsCoinsuranceType As String
            Get
                Return _BlanketContentsCoinsuranceType
            End Get
            Set(value As String)
                _BlanketContentsCoinsuranceType = value
                Select Case _BlanketContentsCoinsuranceType
                    Case "N/A"
                        _BlanketContentsCoinsuranceTypeId = "0"
                    Case "Waived"
                        _BlanketContentsCoinsuranceTypeId = "1"
                    Case "50%"
                        _BlanketContentsCoinsuranceTypeId = "2"
                    Case "60%"
                        _BlanketContentsCoinsuranceTypeId = "3"
                    Case "70%"
                        _BlanketContentsCoinsuranceTypeId = "4"
                    Case "80%"
                        _BlanketContentsCoinsuranceTypeId = "5"
                    Case "90%"
                        _BlanketContentsCoinsuranceTypeId = "6"
                    Case "100%"
                        _BlanketContentsCoinsuranceTypeId = "7"
                    Case "10%"
                        _BlanketContentsCoinsuranceTypeId = "8"
                    Case "20%"
                        _BlanketContentsCoinsuranceTypeId = "9"
                    Case "30%"
                        _BlanketContentsCoinsuranceTypeId = "10"
                    Case "40%"
                        _BlanketContentsCoinsuranceTypeId = "11"
                    Case "125%"
                        _BlanketContentsCoinsuranceTypeId = "12"
                    Case Else
                        _BlanketContentsCoinsuranceTypeId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21083</remarks>
        Public Property BlanketContentsValuationId As String
            Get
                Return _BlanketContentsValuationId
            End Get
            Set(value As String)
                _BlanketContentsValuationId = value
                '(1=Replacement Cost; 2=Actual Cash Value; 3=Functional Building Valuation)
                _BlanketContentsValuation = ""
                If IsNumeric(_BlanketContentsValuationId) = True Then
                    Select Case _BlanketContentsValuationId
                        Case "-1"
                            _BlanketContentsValuation = "N/A"
                        Case "1"
                            _BlanketContentsValuation = "Replacement Cost"
                        Case "2"
                            _BlanketContentsValuation = "Actual Cash Value"
                        Case "3"
                            _BlanketContentsValuation = "Functional Building Valuation"
                        Case "7"
                            _BlanketContentsValuation = "Functional Replacement Cost"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21083</remarks>
        Public Property BlanketContentsValuation As String
            Get
                Return _BlanketContentsValuation
            End Get
            Set(value As String)
                _BlanketContentsValuation = value
                Select Case _BlanketContentsValuation
                    Case "N/A"
                        _BlanketContentsValuationId = "-1"
                    Case "Replacement Cost"
                        _BlanketContentsValuationId = "1"
                    Case "Actual Cash Value"
                        _BlanketContentsValuationId = "2"
                    Case "Functional Building Valuation"
                        _BlanketContentsValuationId = "3"
                    Case "Functional Replacement Cost"
                        _BlanketContentsValuationId = "7"
                    Case Else
                        _BlanketContentsValuationId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21083</remarks>
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property BlanketContentsDeductibleID As String
            Get
                Return _BlanketContentsDeductibleID
            End Get
            Set(value As String)
                _BlanketContentsDeductibleID = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21084</remarks>
        Public Property BlanketBuildingAndContentsLimit As String
            Get
                Return _BlanketBuildingAndContentsLimit
            End Get
            Set(value As String)
                _BlanketBuildingAndContentsLimit = value
                qqHelper.ConvertToLimitFormat(_BlanketBuildingAndContentsLimit)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21084</remarks>
        Public Property BlanketBuildingAndContentsCoinsuranceTypeId As String
            Get
                Return _BlanketBuildingAndContentsCoinsuranceTypeId
            End Get
            Set(value As String)
                _BlanketBuildingAndContentsCoinsuranceTypeId = value
                _BlanketBuildingAndContentsCoinsuranceType = ""
                If IsNumeric(_BlanketBuildingAndContentsCoinsuranceTypeId) = True Then
                    Select Case _BlanketBuildingAndContentsCoinsuranceTypeId
                        Case "0"
                            _BlanketBuildingAndContentsCoinsuranceType = "N/A"
                        Case "1"
                            _BlanketBuildingAndContentsCoinsuranceType = "Waived"
                        Case "2"
                            _BlanketBuildingAndContentsCoinsuranceType = "50%"
                        Case "3"
                            _BlanketBuildingAndContentsCoinsuranceType = "60%"
                        Case "4"
                            _BlanketBuildingAndContentsCoinsuranceType = "70%"
                        Case "5"
                            _BlanketBuildingAndContentsCoinsuranceType = "80%"
                        Case "6"
                            _BlanketBuildingAndContentsCoinsuranceType = "90%"
                        Case "7"
                            _BlanketBuildingAndContentsCoinsuranceType = "100%"
                        Case "8"
                            _BlanketBuildingAndContentsCoinsuranceType = "10%"
                        Case "9"
                            _BlanketBuildingAndContentsCoinsuranceType = "20%"
                        Case "10"
                            _BlanketBuildingAndContentsCoinsuranceType = "30%"
                        Case "11"
                            _BlanketBuildingAndContentsCoinsuranceType = "40%"
                        Case "12"
                            _BlanketBuildingAndContentsCoinsuranceType = "125%"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21084</remarks>
        Public Property BlanketBuildingAndContentsCoinsuranceType As String
            Get
                Return _BlanketBuildingAndContentsCoinsuranceType
            End Get
            Set(value As String)
                _BlanketBuildingAndContentsCoinsuranceType = value
                Select Case _BlanketBuildingAndContentsCoinsuranceType
                    Case "N/A"
                        _BlanketBuildingAndContentsCoinsuranceTypeId = "0"
                    Case "Waived"
                        _BlanketBuildingAndContentsCoinsuranceTypeId = "1"
                    Case "50%"
                        _BlanketBuildingAndContentsCoinsuranceTypeId = "2"
                    Case "60%"
                        _BlanketBuildingAndContentsCoinsuranceTypeId = "3"
                    Case "70%"
                        _BlanketBuildingAndContentsCoinsuranceTypeId = "4"
                    Case "80%"
                        _BlanketBuildingAndContentsCoinsuranceTypeId = "5"
                    Case "90%"
                        _BlanketBuildingAndContentsCoinsuranceTypeId = "6"
                    Case "100%"
                        _BlanketBuildingAndContentsCoinsuranceTypeId = "7"
                    Case "10%"
                        _BlanketBuildingAndContentsCoinsuranceTypeId = "8"
                    Case "20%"
                        _BlanketBuildingAndContentsCoinsuranceTypeId = "9"
                    Case "30%"
                        _BlanketBuildingAndContentsCoinsuranceTypeId = "10"
                    Case "40%"
                        _BlanketBuildingAndContentsCoinsuranceTypeId = "11"
                    Case "125%"
                        _BlanketBuildingAndContentsCoinsuranceTypeId = "12"
                    Case Else
                        _BlanketBuildingAndContentsCoinsuranceTypeId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21084</remarks>
        Public Property BlanketBuildingAndContentsValuationId As String
            Get
                Return _BlanketBuildingAndContentsValuationId
            End Get
            Set(value As String)
                _BlanketBuildingAndContentsValuationId = value
                '(1=Replacement Cost; 2=Actual Cash Value; 3=Functional Building Valuation)
                _BlanketBuildingAndContentsValuation = ""
                If IsNumeric(_BlanketBuildingAndContentsValuationId) = True Then
                    Select Case _BlanketBuildingAndContentsValuationId
                        Case "-1"
                            _BlanketBuildingAndContentsValuation = "N/A"
                        Case "1"
                            _BlanketBuildingAndContentsValuation = "Replacement Cost"
                        Case "2"
                            _BlanketBuildingAndContentsValuation = "Actual Cash Value"
                        Case "3"
                            _BlanketBuildingAndContentsValuation = "Functional Building Valuation"
                        Case "7"
                            _BlanketBuildingAndContentsValuation = "Functional Replacement Cost"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21084</remarks>
        Public Property BlanketBuildingAndContentsValuation As String
            Get
                Return _BlanketBuildingAndContentsValuation
            End Get
            Set(value As String)
                _BlanketBuildingAndContentsValuation = value
                Select Case _BlanketBuildingAndContentsValuation
                    Case "N/A"
                        _BlanketBuildingAndContentsValuationId = "-1"
                    Case "Replacement Cost"
                        _BlanketBuildingAndContentsValuationId = "1"
                    Case "Actual Cash Value"
                        _BlanketBuildingAndContentsValuationId = "2"
                    Case "Functional Building Valuation"
                        _BlanketBuildingAndContentsValuationId = "3"
                    Case "Functional Replacement Cost"
                        _BlanketBuildingAndContentsValuationId = "7"
                    Case Else
                        _BlanketBuildingAndContentsValuationId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21084</remarks>
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property BlanketBuildingAndContentsDeductibleID As String
            Get
                Return _BlanketBuildingAndContentsDeductibleID
            End Get
            Set(value As String)
                _BlanketBuildingAndContentsDeductibleID = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21085</remarks>
        Public Property BlanketBusinessIncomeLimit As String
            Get
                Return _BlanketBusinessIncomeLimit
            End Get
            Set(value As String)
                _BlanketBusinessIncomeLimit = value
                qqHelper.ConvertToLimitFormat(_BlanketBusinessIncomeLimit)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21085</remarks>
        Public Property BlanketBusinessIncomeCoinsuranceTypeId As String
            Get
                Return _BlanketBusinessIncomeCoinsuranceTypeId
            End Get
            Set(value As String)
                _BlanketBusinessIncomeCoinsuranceTypeId = value
                _BlanketBusinessIncomeCoinsuranceType = ""
                If IsNumeric(_BlanketBusinessIncomeCoinsuranceTypeId) = True Then
                    Select Case _BlanketBusinessIncomeCoinsuranceTypeId
                        Case "0"
                            _BlanketBusinessIncomeCoinsuranceType = "N/A"
                        Case "1"
                            _BlanketBusinessIncomeCoinsuranceType = "Waived"
                        Case "2"
                            _BlanketBusinessIncomeCoinsuranceType = "50%"
                        Case "3"
                            _BlanketBusinessIncomeCoinsuranceType = "60%"
                        Case "4"
                            _BlanketBusinessIncomeCoinsuranceType = "70%"
                        Case "5"
                            _BlanketBusinessIncomeCoinsuranceType = "80%"
                        Case "6"
                            _BlanketBusinessIncomeCoinsuranceType = "90%"
                        Case "7"
                            _BlanketBusinessIncomeCoinsuranceType = "100%"
                        Case "8"
                            _BlanketBusinessIncomeCoinsuranceType = "10%"
                        Case "9"
                            _BlanketBusinessIncomeCoinsuranceType = "20%"
                        Case "10"
                            _BlanketBusinessIncomeCoinsuranceType = "30%"
                        Case "11"
                            _BlanketBusinessIncomeCoinsuranceType = "40%"
                        Case "12"
                            _BlanketBusinessIncomeCoinsuranceType = "125%"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21085</remarks>
        Public Property BlanketBusinessIncomeCoinsuranceType As String
            Get
                Return _BlanketBusinessIncomeCoinsuranceType
            End Get
            Set(value As String)
                _BlanketBusinessIncomeCoinsuranceType = value
                Select Case _BlanketBusinessIncomeCoinsuranceType
                    Case "N/A"
                        _BlanketBusinessIncomeCoinsuranceTypeId = "0"
                    Case "Waived"
                        _BlanketBusinessIncomeCoinsuranceTypeId = "1"
                    Case "50%"
                        _BlanketBusinessIncomeCoinsuranceTypeId = "2"
                    Case "60%"
                        _BlanketBusinessIncomeCoinsuranceTypeId = "3"
                    Case "70%"
                        _BlanketBusinessIncomeCoinsuranceTypeId = "4"
                    Case "80%"
                        _BlanketBusinessIncomeCoinsuranceTypeId = "5"
                    Case "90%"
                        _BlanketBusinessIncomeCoinsuranceTypeId = "6"
                    Case "100%"
                        _BlanketBusinessIncomeCoinsuranceTypeId = "7"
                    Case "10%"
                        _BlanketBusinessIncomeCoinsuranceTypeId = "8"
                    Case "20%"
                        _BlanketBusinessIncomeCoinsuranceTypeId = "9"
                    Case "30%"
                        _BlanketBusinessIncomeCoinsuranceTypeId = "10"
                    Case "40%"
                        _BlanketBusinessIncomeCoinsuranceTypeId = "11"
                    Case "125%"
                        _BlanketBusinessIncomeCoinsuranceTypeId = "12"
                    Case Else
                        _BlanketBusinessIncomeCoinsuranceTypeId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21085</remarks>
        Public Property BlanketBusinessIncomeValuationId As String
            Get
                Return _BlanketBusinessIncomeValuationId
            End Get
            Set(value As String)
                _BlanketBusinessIncomeValuationId = value
                '(1=Replacement Cost; 2=Actual Cash Value; 3=Functional Building Valuation)
                _BlanketBusinessIncomeValuation = ""
                If IsNumeric(_BlanketBusinessIncomeValuationId) = True Then
                    Select Case _BlanketBusinessIncomeValuationId
                        Case "-1"
                            _BlanketBusinessIncomeValuation = "N/A"
                        Case "1"
                            _BlanketBusinessIncomeValuation = "Replacement Cost"
                        Case "2"
                            _BlanketBusinessIncomeValuation = "Actual Cash Value"
                        Case "3"
                            _BlanketBusinessIncomeValuation = "Functional Building Valuation"
                        Case "7"
                            _BlanketBusinessIncomeValuation = "Functional Replacement Cost"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21085</remarks>
        Public Property BlanketBusinessIncomeValuation As String
            Get
                Return _BlanketBusinessIncomeValuation
            End Get
            Set(value As String)
                _BlanketBusinessIncomeValuation = value
                Select Case _BlanketBusinessIncomeValuation
                    Case "N/A"
                        _BlanketBusinessIncomeValuationId = "-1"
                    Case "Replacement Cost"
                        _BlanketBusinessIncomeValuationId = "1"
                    Case "Actual Cash Value"
                        _BlanketBusinessIncomeValuationId = "2"
                    Case "Functional Building Valuation"
                        _BlanketBusinessIncomeValuationId = "3"
                    Case "Functional Replacement Cost"
                        _BlanketBusinessIncomeValuationId = "7"
                    Case Else
                        _BlanketBusinessIncomeValuationId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverages w/ coveragecode_ids 21082 (building), 21083 (contents), 21084 (building and contents), 21085 (business income), and 21122 (combined EQ)</remarks>
        Public Property CPR_BlanketCoverages_TotalPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_CPR_BlanketCoverages_TotalPremium)
            End Get
            Set(value As String)
                _CPR_BlanketCoverages_TotalPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_CPR_BlanketCoverages_TotalPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21122</remarks>
        Public Property BlanketCombinedEarthquake_QuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_BlanketCombinedEarthquake_QuotedPremium)
            End Get
            Set(value As String)
                _BlanketCombinedEarthquake_QuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_BlanketCombinedEarthquake_QuotedPremium)
            End Set
        End Property
        Public Property BlanketBuildingIsAgreedValue As Boolean
            Get
                Return _BlanketBuildingIsAgreedValue
            End Get
            Set(value As Boolean)
                _BlanketBuildingIsAgreedValue = value
            End Set
        End Property
        Public Property BlanketContentsIsAgreedValue As Boolean
            Get
                Return _BlanketContentsIsAgreedValue
            End Get
            Set(value As Boolean)
                _BlanketContentsIsAgreedValue = value
            End Set
        End Property
        Public Property BlanketBuildingAndContentsIsAgreedValue As Boolean
            Get
                Return _BlanketBuildingAndContentsIsAgreedValue
            End Get
            Set(value As Boolean)
                _BlanketBuildingAndContentsIsAgreedValue = value
            End Set
        End Property
        Public Property BlanketBusinessIncomeIsAgreedValue As Boolean
            Get
                Return _BlanketBusinessIncomeIsAgreedValue
            End Get
            Set(value As Boolean)
                _BlanketBusinessIncomeIsAgreedValue = value
            End Set
        End Property
        ''' <summary>
        ''' used for tier override
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>should just be used for testing</remarks>
        Public Property UseTierOverride As Boolean
            Get
                Return _UseTierOverride
            End Get
            Set(value As Boolean)
                _UseTierOverride = value
            End Set
        End Property
        ''' <summary>
        ''' used for tier override
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's TierAdjustmentType table; should just be used for testing</remarks>
        Public Property TierAdjustmentTypeId As String 'TierAdjustmentType table: N/A=0; 1=13; etc.
            Get
                Return _TierAdjustmentTypeId
            End Get
            Set(value As String)
                _TierAdjustmentTypeId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 30007 (HOM and DFR)</remarks>
        Public Property PersonalLiabilityLimitId As String '259=25,000; 262=100,000
            Get
                Return _PersonalLiabilityLimitId
            End Get
            Set(value As String)
                _PersonalLiabilityLimitId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 30007 (HOM and DFR)</remarks>
        Public Property PersonalLiabilityQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_PersonalLiabilityQuotedPremium)
            End Get
            Set(value As String)
                _PersonalLiabilityQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_PersonalLiabilityQuotedPremium)
            End Set
        End Property
        Public Property HasEPLI As Boolean
            Get
                Return _HasEPLI
            End Get
            Set(value As Boolean)
                _HasEPLI = value
            End Set
        End Property
        Public Property EPLI_Applied As Boolean
            Get
                Return _EPLI_Applied
            End Get
            Set(value As Boolean)
                _EPLI_Applied = value
            End Set
        End Property
        Public Property EPLIPremium As String
            Get
                qqHelper.ConvertToQuotedPremiumFormat(_EPLIPremium, QuickQuoteHelperClass.QuickQuoteEmptyStringReplacementType.NonApplicable)
                Return _EPLIPremium
            End Get
            Set(value As String)
                _EPLIPremium = value
            End Set
        End Property
        Public Property EPLICoverageLimitId As String
            Get
                Return _EPLICoverageLimitId
            End Get
            Set(value As String)
                _EPLICoverageLimitId = value
            End Set
        End Property
        Public ReadOnly Property EPLICoverageLimit As String
            Get
                Select Case _EPLICoverageLimitId
                    Case "0"
                        Return "N/A"
                    Case "360"
                        Return "100,000/100,000"
                    Case "361"
                        Return "250,000/250,000"
                    Case "362"
                        Return "500,000/500,000"
                    Case "363"
                        Return "1,000,000/1,000,000"
                    Case Else
                End Select
                Return ""
                'N/A = 0
                '100,000/100,000 = 360
                '250,000/250,000 = 361
                '500,000/500,000 = 362
                '1,000,000/1,000,000 = 363
            End Get
        End Property
        Public Property EPLIDeductibleId As String
            Get
                Return _EPLIDeductibleId
            End Get
            Set(value As String)
                _EPLIDeductibleId = value
            End Set
        End Property
        Public ReadOnly Property EPLIDeductible As String
            Get
                Select Case _EPLIDeductibleId
                    Case "16"
                        Return "5,000"
                    Case "17"
                        Return "10,000"
                    Case "31"
                        Return "15,000"
                    Case "18"
                        Return "20,000"
                    Case "19"
                        Return "25,000"
                    Case Else

                End Select
                Return ""
                '5,000 = 16
                '10,000 = 17
                '15,000 = 31
                '20,000 = 18
                '25,000 = 19
            End Get
        End Property
        Public Property EPLICoverageTypeID As String
            Get
                Return _EPLICoverageTypeId
            End Get
            Set(value As String)
                _EPLICoverageTypeId = value
            End Set
        End Property
        Public ReadOnly Property EPLICoverageType As String
            Get
                Select Case _EPLICoverageTypeId
                    Case "19"
                        Return "Opt-Out"
                    Case "20"
                        Return "Ineligible"
                    Case "21"
                        Return "EPLI (underwritten)"
                    Case "22"
                        Return "EPLI (non-underwritten)"
                    Case Else

                End Select
                Return ""
                'EPLI (non-underwritten) = 22
                'EPLI (underwritten) = 21
                'Opt-Out = 19
                'Ineligible = 20
                Return _EPLICoverageTypeId
            End Get
        End Property

        'added 04/15/2019 for CGG-CPP-BOP Cyber Liability
        Public Property CyberLiability As Boolean
            Get
                Return _CyberLiability
            End Get
            Set(value As Boolean)
                _CyberLiability = value
            End Set
        End Property

        'added 04/15/2019 for CGG-CPP-BOP Cyber Liability
        Public Property CyberLiabilityPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_CyberLiabilityPremium)
            End Get
            Set(value As String)
                _CyberLiabilityPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_CyberLiabilityPremium)
            End Set
        End Property

        'added 04/15/2019 for CGG-CPP-BOP Cyber Liability
        Public Property CyberLiabilityDeductible As String
            Get
                Return _CyberLiabilityDeductible
            End Get
            Set(value As String)
                _CyberLiabilityDeductible = value
                qqHelper.ConvertToLimitFormat(_CyberLiabilityDeductible)
            End Set
        End Property

        'added 04/15/2019 for CGG-CPP-BOP Cyber Liability
        Public Property CyberLiabilityDeductibleId As String
            Get
                Return _CyberLiabilityDeductibleId
            End Get
            Set(value As String)
                _CyberLiabilityDeductibleId = value

                Select Case _CyberLiabilityDeductibleId
                    Case "9"
                        _CyberLiabilityDeductible = "1,000"
                    Case "15"
                        _CyberLiabilityDeductible = "2,500"
                    Case "16"
                        _CyberLiabilityDeductible = "5,000"
                End Select
            End Set
        End Property

        'added 04/15/2019 for CGG-CPP-BOP Cyber Liability
        Public Property CyberLiabilityLimit As String
            Get
                Return _CyberLiabilityLimit
            End Get
            Set(value As String)
                _CyberLiabilityLimit = value
                qqHelper.ConvertToLimitFormat(_CyberLiabilityLimit)
            End Set
        End Property

        'added 04/15/2019 for CGG-CPP-BOP Cyber Liability
        Public Property CyberLiabilityLimitId As String
            Get
                Return _CyberLiabilityLimitId
            End Get
            Set(value As String)
                _CyberLiabilityLimitId = value

                Select Case _CyberLiabilityLimitId
                    Case "261"
                        _CyberLiabilityLimit = "50,000"
                    Case "262"
                        _CyberLiabilityLimit = "100,000"
                    Case "263"
                        _CyberLiabilityLimit = "200,000"
                    Case "423"
                        _CyberLiabilityLimit = "250,000"
                    Case "266"
                        _CyberLiabilityLimit = "500,000"
                    Case "424"
                        _CyberLiabilityLimit = "1,000,000"
                End Select
            End Set
        End Property

        'added 04/15/2019 for CGG-CPP-BOP Cyber Liability
        Public Property CyberLiabilityTypeId As String
            Get
                Return _CyberLiabilityTypeId
            End Get
            Set(value As String)
                _CyberLiabilityTypeId = value
            End Set
        End Property

        'added 04/15/2019 for CGG-CPP-BOP Cyber Liability
        Public Property CyberLiabilityType As String
            Get
                Return _CyberLiabilityType
            End Get
            Set(value As String)
                _CyberLiabilityType = value
            End Set
        End Property

        '''' <summary>
        '''' CGL - Blanket Waiver of Subrogation
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks></remarks>
        'Public Property BlanketWaiverOfSubrogation As String
        '    Get
        '        Return _BlanketWaiverOfSubrogation
        '    End Get
        '    Set(value As String)
        '        _BlanketWaiverOfSubrogation = value
        '    End Set
        'End Property
        '''' <summary>
        '''' CGL - Blanket Waiver of Subrogation Quoted Premium
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 70017 (BOP) or 80154 (CGL)</remarks>
        'Public Property BlanketWaiverOfSubrogationQuotedPremium As String
        '    Get
        '        Return _BlanketWaiverOfSubrogationQuotedPremium
        '    End Get
        '    Set(value As String)
        '        _BlanketWaiverOfSubrogationQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_BlanketWaiverOfSubrogationQuotedPremium)
        '    End Set
        'End Property


        Public Property HasCondoDandO As Boolean
            Get
                Return _HasCondoDandO
            End Get
            Set(value As Boolean)
                _HasCondoDandO = value
            End Set
        End Property

        Public Property CondoDandOAssociatedName As String
            Get
                Return _CondoDandOAssociatedName
            End Get
            Set(value As String)
                _CondoDandOAssociatedName = value
            End Set
        End Property

        Public Property CondoDandODeductibleId As String
            Get
                Return _CondoDandODeductibleId
            End Get
            Set(value As String)
                _CondoDandODeductibleId = value
            End Set
        End Property

        Public ReadOnly Property CondoDandODeductible As String
            Get
                Select Case _CondoDandODeductibleId
                    Case "9"
                        Return "1,000"
                    Case "15"
                        Return "2,500"
                    Case "16"
                        Return "5,000"
                    Case "17"
                        Return "10,000"
                'Case "31"
                '    Return "15,000"
                'Case "18"
                '    Return "20,000"
                    Case "19"
                        Return "25,000"
                    Case Else

                End Select
                Return ""
            End Get

        End Property

        Public Property CondoDandOPremium As String
            Get
                qqHelper.ConvertToQuotedPremiumFormat(_CondoDandOPremium, QuickQuoteHelperClass.QuickQuoteEmptyStringReplacementType.NonApplicable)
                Return _CondoDandOPremium
            End Get
            Set(value As String)
                _CondoDandOPremium = value
            End Set
        End Property

        Public Property CondoDandOManualLimit As String
            Get
                Return _CondoDandOManualLimit
            End Get
            Set(value As String)
                _CondoDandOManualLimit = value
            End Set
        End Property
        Public Property Farm_F_and_G_DeductibleLimitId As String 'static data
            Get
                Return _Farm_F_and_G_DeductibleLimitId
            End Get
            Set(value As String)
                _Farm_F_and_G_DeductibleLimitId = value
            End Set
        End Property
        Public Property Farm_F_and_G_DeductibleQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_Farm_F_and_G_DeductibleQuotedPremium)
            End Get
            Set(value As String)
                _Farm_F_and_G_DeductibleQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_Farm_F_and_G_DeductibleQuotedPremium)
            End Set
        End Property
        Public Property HasFarmEquipmentBreakdown As Boolean
            Get
                Return _HasFarmEquipmentBreakdown
            End Get
            Set(value As Boolean)
                _HasFarmEquipmentBreakdown = value
            End Set
        End Property
        Public Property FarmEquipmentBreakdownQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_FarmEquipmentBreakdownQuotedPremium)
            End Get
            Set(value As String)
                _FarmEquipmentBreakdownQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_FarmEquipmentBreakdownQuotedPremium)
            End Set
        End Property
        Public Property HasFarmExtender As Boolean
            Get
                Return _HasFarmExtender
            End Get
            Set(value As Boolean)
                _HasFarmExtender = value
            End Set
        End Property
        Public Property FarmExtenderQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_FarmExtenderQuotedPremium)
            End Get
            Set(value As String)
                _FarmExtenderQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_FarmExtenderQuotedPremium)
            End Set
        End Property
        Public Property FarmAllStarLimitId As String 'static data
            Get
                Return _FarmAllStarLimitId
            End Get
            Set(value As String)
                _FarmAllStarLimitId = value
            End Set
        End Property
        Public Property FarmAllStarQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_FarmAllStarQuotedPremium)
            End Get
            Set(value As String)
                _FarmAllStarQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_FarmAllStarQuotedPremium)
            End Set
        End Property
        Public Property HasFarmEmployersLiability As Boolean
            Get
                Return _HasFarmEmployersLiability
            End Get
            Set(value As Boolean)
                _HasFarmEmployersLiability = value
            End Set
        End Property
        Public Property FarmEmployersLiabilityQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_FarmEmployersLiabilityQuotedPremium)
            End Get
            Set(value As String)
                _FarmEmployersLiabilityQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_FarmEmployersLiabilityQuotedPremium)
            End Set
        End Property
        Public Property FarmFireLegalLiabilityLimitId As String 'static data
            Get
                Return _FarmFireLegalLiabilityLimitId
            End Get
            Set(value As String)
                _FarmFireLegalLiabilityLimitId = value
            End Set
        End Property
        Public Property FarmFireLegalLiabilityQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_FarmFireLegalLiabilityQuotedPremium)
            End Get
            Set(value As String)
                _FarmFireLegalLiabilityQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_FarmFireLegalLiabilityQuotedPremium)
            End Set
        End Property
        Public Property HasFarmPersonalAndAdvertisingInjury As Boolean
            Get
                Return _HasFarmPersonalAndAdvertisingInjury
            End Get
            Set(value As Boolean)
                _HasFarmPersonalAndAdvertisingInjury = value
            End Set
        End Property
        Public Property FarmPersonalAndAdvertisingInjuryQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_FarmPersonalAndAdvertisingInjuryQuotedPremium)
            End Get
            Set(value As String)
                _FarmPersonalAndAdvertisingInjuryQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_FarmPersonalAndAdvertisingInjuryQuotedPremium)
            End Set
        End Property
#Region "Farm Custom Growers"
        Public Property FarmContractGrowersCareCustodyControlLimitId As String 'static data
            Get
                Return _FarmContractGrowersCareCustodyControlLimitId
            End Get
            Set(value As String)
                _FarmContractGrowersCareCustodyControlLimitId = value
            End Set
        End Property
        Public Property FarmContractGrowersCareCustodyControlDescription As String
            Get
                Return _FarmContractGrowersCareCustodyControlDescription
            End Get
            Set(value As String)
                _FarmContractGrowersCareCustodyControlDescription = value
            End Set
        End Property
        Public Property FarmContractGrowersCareCustodyControlQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_FarmContractGrowersCareCustodyControlQuotedPremium)
            End Get
            Set(value As String)
                _FarmContractGrowersCareCustodyControlQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_FarmContractGrowersCareCustodyControlQuotedPremium)
            End Set
        End Property
#End Region
#Region "Farm Custom Feeding"

        Protected Function farmCustomFeedingLimitAsText(propertyName As QuickQuoteHelperClass.QuickQuotePropertyName, limitIdValue As String) As String
            Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, propertyName, limitIdValue)
        End Function
        'Cattle
        Public Property FarmCustomFeedingCattleLimitId As String 'static data
            Get
                'Return _FarmContractGrowersCareCustodyControlLimitId
                'updated 7/21/2018
                Return _FarmCustomFeedingCattleLimitId
            End Get
            Set(value As String)
                '_FarmContractGrowersCareCustodyControlLimitId = value
                'updated 7/21/2018
                _FarmCustomFeedingCattleLimitId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property FarmCustomFeedingCattleLimit As String
            Get
                Return farmCustomFeedingLimitAsText(QuickQuoteHelperClass.QuickQuotePropertyName.FarmCustomFeedingCattleLimitId, FarmCustomFeedingCattleLimitId)
            End Get
        End Property
        Public Property FarmCustomFeedingCattleDescription As String
            Get
                'Return _FarmContractGrowersCareCustodyControlDescription
                'updated 7/21/2018
                Return _FarmCustomFeedingCattleDescription
            End Get
            Set(value As String)
                '_FarmContractGrowersCareCustodyControlDescription = value
                'updated 7/21/2018
                _FarmCustomFeedingCattleDescription = value
            End Set
        End Property
        Public Property FarmCustomFeedingCattleQuotedPremium As String
            Get
                'Return qqHelper.QuotedPremiumFormat(_FarmContractGrowersCareCustodyControlQuotedPremium)
                'updated 7/21/2018
                Return qqHelper.QuotedPremiumFormat(_FarmCustomFeedingCattleQuotedPremium)
            End Get
            Set(value As String)
                '_FarmContractGrowersCareCustodyControlQuotedPremium = value
                'qqHelper.ConvertToQuotedPremiumFormat(_FarmContractGrowersCareCustodyControlQuotedPremium)
                'updated 7/21/2018
                _FarmCustomFeedingCattleQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_FarmCustomFeedingCattleQuotedPremium)
            End Set
        End Property
        'Equine
        Public Property FarmCustomFeedingEquineLimitId As String 'static data
            Get
                'Return _FarmContractGrowersCareCustodyControlLimitId
                'updated 7/21/2018
                Return _FarmCustomFeedingEquineLimitId
            End Get
            Set(value As String)
                '_FarmContractGrowersCareCustodyControlLimitId = value
                'updated 7/21/2018
                _FarmCustomFeedingEquineLimitId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property FarmCustomFeedingEquineLimit As String
            Get
                Return farmCustomFeedingLimitAsText(QuickQuoteHelperClass.QuickQuotePropertyName.FarmCustomFeedingEquineLimitId, FarmCustomFeedingEquineLimitId)
            End Get
        End Property
        Public Property FarmCustomFeedingEquineDescription As String
            Get
                'Return _FarmContractGrowersCareCustodyControlDescription
                'updated 7/21/2018
                Return _FarmCustomFeedingEquineDescription
            End Get
            Set(value As String)
                '_FarmContractGrowersCareCustodyControlDescription = value
                'updated 7/21/2018
                _FarmCustomFeedingEquineDescription = value
            End Set
        End Property
        Public Property FarmCustomFeedingEquineQuotedPremium As String
            Get
                'Return qqHelper.QuotedPremiumFormat(_FarmContractGrowersCareCustodyControlQuotedPremium)
                'updated 7/21/2018
                Return qqHelper.QuotedPremiumFormat(_FarmCustomFeedingEquineQuotedPremium)
            End Get
            Set(value As String)
                '_FarmContractGrowersCareCustodyControlQuotedPremium = value
                'qqHelper.ConvertToQuotedPremiumFormat(_FarmContractGrowersCareCustodyControlQuotedPremium)
                'updated 7/21/2018
                _FarmCustomFeedingEquineQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_FarmCustomFeedingEquineQuotedPremium)
            End Set
        End Property
        'Poultry
        Public Property FarmCustomFeedingPoultryLimitId As String 'static data
            Get
                'Return _FarmContractGrowersCareCustodyControlLimitId
                'updated 7/21/2018
                Return _FarmCustomFeedingPoultryLimitId
            End Get
            Set(value As String)
                '_FarmContractGrowersCareCustodyControlLimitId = value
                'updated 7/21/2018
                _FarmCustomFeedingPoultryLimitId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property FarmCustomFeedingPoultryLimit As String
            Get
                Return farmCustomFeedingLimitAsText(QuickQuoteHelperClass.QuickQuotePropertyName.FarmCustomFeedingPoultryLimitId, FarmCustomFeedingPoultryLimitId)
            End Get
        End Property
        Public Property FarmCustomFeedingPoultryDescription As String
            Get
                'Return _FarmContractGrowersCareCustodyControlDescription
                'updated 7/21/2018
                Return _FarmCustomFeedingPoultryDescription
            End Get
            Set(value As String)
                '_FarmContractGrowersCareCustodyControlDescription = value
                'updated 7/21/2018
                _FarmCustomFeedingPoultryDescription = value
            End Set
        End Property
        Public Property FarmCustomFeedingPoultryQuotedPremium As String
            Get
                'Return qqHelper.QuotedPremiumFormat(_FarmContractGrowersCareCustodyControlQuotedPremium)
                'updated 7/21/2018
                Return qqHelper.QuotedPremiumFormat(_FarmCustomFeedingPoultryQuotedPremium)
            End Get
            Set(value As String)
                '_FarmContractGrowersCareCustodyControlQuotedPremium = value
                'qqHelper.ConvertToQuotedPremiumFormat(_FarmContractGrowersCareCustodyControlQuotedPremium)
                'updated 7/21/2018
                _FarmCustomFeedingPoultryQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_FarmCustomFeedingPoultryQuotedPremium)
            End Set
        End Property
        'Swine
        Public Property FarmCustomFeedingSwineLimitId As String 'static data
            Get
                'Return _FarmContractGrowersCareCustodyControlLimitId
                'updated 7/21/2018
                Return _FarmCustomFeedingSwineLimitId
            End Get
            Set(value As String)
                '_FarmContractGrowersCareCustodyControlLimitId = value
                'updated 7/21/2018
                _FarmCustomFeedingSwineLimitId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public ReadOnly Property FarmCustomFeedingSwineLimit As String
            Get
                Return farmCustomFeedingLimitAsText(QuickQuoteHelperClass.QuickQuotePropertyName.FarmCustomFeedingSwineLimitId, FarmCustomFeedingSwineLimitId)
            End Get
        End Property
        Public Property FarmCustomFeedingSwineDescription As String
            Get
                'Return _FarmContractGrowersCareCustodyControlDescription
                'updated 7/21/2018
                Return _FarmCustomFeedingSwineDescription
            End Get
            Set(value As String)
                '_FarmContractGrowersCareCustodyControlDescription = value
                'updated 7/21/2018
                _FarmCustomFeedingSwineDescription = value
            End Set
        End Property
        Public Property FarmCustomFeedingSwineQuotedPremium As String
            Get
                'Return qqHelper.QuotedPremiumFormat(_FarmContractGrowersCareCustodyControlQuotedPremium)
                'updated 7/21/2018
                Return qqHelper.QuotedPremiumFormat(_FarmCustomFeedingSwineQuotedPremium)
            End Get
            Set(value As String)
                '_FarmContractGrowersCareCustodyControlQuotedPremium = value
                'qqHelper.ConvertToQuotedPremiumFormat(_FarmContractGrowersCareCustodyControlQuotedPremium)
                'updated 7/21/2018
                _FarmCustomFeedingSwineQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_FarmCustomFeedingSwineQuotedPremium)
            End Set
        End Property
#End Region
        Public Property HasFarmExclusionOfProductsCompletedWork As Boolean
            Get
                Return _HasFarmExclusionOfProductsCompletedWork
            End Get
            Set(value As Boolean)
                _HasFarmExclusionOfProductsCompletedWork = value
            End Set
        End Property
        Public Property FarmExclusionOfProductsCompletedWorkQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_FarmExclusionOfProductsCompletedWorkQuotedPremium)
            End Get
            Set(value As String)
                _FarmExclusionOfProductsCompletedWorkQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_FarmExclusionOfProductsCompletedWorkQuotedPremium)
            End Set
        End Property
        Public Property FarmIncidentalLimits As List(Of QuickQuoteFarmIncidentalLimit) 'goes w/ FarmIncidentalLimitCoverages
            Get
                Return _FarmIncidentalLimits
            End Get
            Set(value As List(Of QuickQuoteFarmIncidentalLimit))
                _FarmIncidentalLimits = value
            End Set
        End Property
        Public Property HasBusinessIncomeALS As Boolean
            Get
                Return _HasBusinessIncomeALS
            End Get
            Set(value As Boolean)
                _HasBusinessIncomeALS = value
            End Set
        End Property
        Public Property BusinessIncomeALSLimit As String
            Get
                Return _BusinessIncomeALSLimit
            End Get
            Set(value As String)
                _BusinessIncomeALSLimit = value
                qqHelper.ConvertToLimitFormat(_BusinessIncomeALSLimit)
            End Set
        End Property
        Public Property BusinessIncomeALSQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_BusinessIncomeALSQuotedPremium)
            End Get
            Set(value As String)
                _BusinessIncomeALSQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_BusinessIncomeALSQuotedPremium)
            End Set
        End Property
        Public Property HasContractorsEnhancement As Boolean
            Get
                Return _HasContractorsEnhancement
            End Get
            Set(value As Boolean)
                _HasContractorsEnhancement = value
            End Set
        End Property
        Public Property ContractorsEnhancementQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_ContractorsEnhancementQuotedPremium)
            End Get
            Set(value As String)
                _ContractorsEnhancementQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_ContractorsEnhancementQuotedPremium)
            End Set
        End Property
        Public Property CPP_CPR_ContractorsEnhancementQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_CPP_CPR_ContractorsEnhancementQuotedPremium)
            End Get
            Set(value As String)
                _CPP_CPR_ContractorsEnhancementQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_CPP_CPR_ContractorsEnhancementQuotedPremium)
            End Set
        End Property
        Public Property CPP_CGL_ContractorsEnhancementQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_CPP_CGL_ContractorsEnhancementQuotedPremium)
            End Get
            Set(value As String)
                _CPP_CGL_ContractorsEnhancementQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_CPP_CGL_ContractorsEnhancementQuotedPremium)
            End Set
        End Property
        Public Property CPP_CIM_ContractorsEnhancementQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_CPP_CIM_ContractorsEnhancementQuotedPremium)
            End Get
            Set(value As String)
                _CPP_CIM_ContractorsEnhancementQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_CPP_CIM_ContractorsEnhancementQuotedPremium)
            End Set
        End Property
        Public Property HasManufacturersEnhancement As Boolean
            Get
                Return _HasManufacturersEnhancement
            End Get
            Set(value As Boolean)
                _HasManufacturersEnhancement = value
            End Set
        End Property
        Public Property ManufacturersEnhancementQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_ManufacturersEnhancementQuotedPremium)
            End Get
            Set(value As String)
                _ManufacturersEnhancementQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_ManufacturersEnhancementQuotedPremium)
            End Set
        End Property
        Public Property CPP_CPR_ManufacturersEnhancementQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_CPP_CPR_ManufacturersEnhancementQuotedPremium)
            End Get
            Set(value As String)
                _CPP_CPR_ManufacturersEnhancementQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_CPP_CPR_ManufacturersEnhancementQuotedPremium)
            End Set
        End Property
        Public Property CPP_CGL_ManufacturersEnhancementQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_CPP_CGL_ManufacturersEnhancementQuotedPremium)
            End Get
            Set(value As String)
                _CPP_CGL_ManufacturersEnhancementQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_CPP_CGL_ManufacturersEnhancementQuotedPremium)
            End Set
        End Property
        Public Property FarmMachinerySpecialCoverageG_QuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_FarmMachinerySpecialCoverageG_QuotedPremium)
            End Get
            Set(value As String)
                _FarmMachinerySpecialCoverageG_QuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_FarmMachinerySpecialCoverageG_QuotedPremium)
            End Set
        End Property
        Public Property HasAutoPlusEnhancement As Boolean
            Get
                Return _HasAutoPlusEnhancement
            End Get
            Set(value As Boolean)
                _HasAutoPlusEnhancement = value
            End Set
        End Property
        Public Property AutoPlusEnhancement_QuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_AutoPlusEnhancement_QuotedPremium)
            End Get
            Set(value As String)
                _AutoPlusEnhancement_QuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_AutoPlusEnhancement_QuotedPremium)
            End Set
        End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80387; stored in xml at policy level</remarks>
        'Public Property HasApartmentBuildings As Boolean
        '    Get
        '        Return _HasApartmentBuildings
        '    End Get
        '    Set(value As Boolean)
        '        _HasApartmentBuildings = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80387; stored in xml at policy level</remarks>
        'Public Property NumberOfLocationsWithApartments As String
        '    Get
        '        Return _NumberOfLocationsWithApartments
        '    End Get
        '    Set(value As String)
        '        _NumberOfLocationsWithApartments = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80387; stored in xml at policy level</remarks>
        'Public Property ApartmentQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_ApartmentQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _ApartmentQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_ApartmentQuotedPremium)
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80387; stored in xml at policy level</remarks>
        'Public Property HasRestaurantEndorsement As Boolean
        '    Get
        '        Return _HasRestaurantEndorsement
        '    End Get
        '    Set(value As Boolean)
        '        _HasRestaurantEndorsement = value
        '    End Set
        'End Property
        '''' <summary>
        '''' 
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80387; stored in xml at policy level</remarks>
        'Public Property RestaurantQuotedPremium As String
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_RestaurantQuotedPremium)
        '    End Get
        '    Set(value As String)
        '        _RestaurantQuotedPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_RestaurantQuotedPremium)
        '    End Set
        'End Property
        Public Property Liability_UM_UIM_AggregateLiabilityIncrementTypeId As String 'covDetail; covCodeId 21552
            Get
                Return _Liability_UM_UIM_AggregateLiabilityIncrementTypeId
            End Get
            Set(value As String)
                _Liability_UM_UIM_AggregateLiabilityIncrementTypeId = value
            End Set
        End Property
        Public ReadOnly Property Liability_UM_UIM_AggregateLiabilityIncrementType As String
            Get
                Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.Liability_UM_UIM_AggregateLiabilityIncrementTypeId, _Liability_UM_UIM_AggregateLiabilityIncrementTypeId)
            End Get
        End Property
        Public Property Liability_UM_UIM_DeductibleCategoryTypeId As String 'covDetail; covCodeId 21552
            Get
                Return _Liability_UM_UIM_DeductibleCategoryTypeId
            End Get
            Set(value As String)
                _Liability_UM_UIM_DeductibleCategoryTypeId = value
            End Set
        End Property
        Public ReadOnly Property Liability_UM_UIM_DeductibleCategoryType As String
            Get
                Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.Liability_UM_UIM_DeductibleCategoryTypeId, _Liability_UM_UIM_DeductibleCategoryTypeId)
            End Get
        End Property
        Public Property HasUninsuredMotoristPropertyDamage As Boolean 'covCodeId 21539
            Get
                Return _HasUninsuredMotoristPropertyDamage
            End Get
            Set(value As Boolean)
                _HasUninsuredMotoristPropertyDamage = value
            End Set
        End Property
        Public Property UninsuredMotoristPropertyDamageQuotedPremium As String 'covCodeId 21539; may not be populated
            Get
                Return qqHelper.QuotedPremiumFormat(_UninsuredMotoristPropertyDamageQuotedPremium)
            End Get
            Set(value As String)
                _UninsuredMotoristPropertyDamageQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_UninsuredMotoristPropertyDamageQuotedPremium)
            End Set
        End Property
        Public Property MedicalPaymentsTypeId As String 'covDetail; covCodeId 21540
            Get
                Return _MedicalPaymentsTypeId
            End Get
            Set(value As String)
                _MedicalPaymentsTypeId = value
            End Set
        End Property
        Public ReadOnly Property MedicalPaymentsType As String
            Get
                Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.MedicalPaymentsTypeId, _MedicalPaymentsTypeId)
            End Get
        End Property
        Public Property HasPhysicalDamageOtherThanCollision As Boolean 'covCodeId 21550
            Get
                Return _HasPhysicalDamageOtherThanCollision
            End Get
            Set(value As Boolean)
                _HasPhysicalDamageOtherThanCollision = value
            End Set
        End Property
        Public Property PhysicalDamageOtherThanCollisionQuotedPremium As String 'covCodeId 21550; may not be populated
            Get
                Return qqHelper.QuotedPremiumFormat(_PhysicalDamageOtherThanCollisionQuotedPremium)
            End Get
            Set(value As String)
                _PhysicalDamageOtherThanCollisionQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_PhysicalDamageOtherThanCollisionQuotedPremium)
            End Set
        End Property
        Public Property HasPhysicalDamageCollision As Boolean 'covCodeId 21551
            Get
                Return _HasPhysicalDamageCollision
            End Get
            Set(value As Boolean)
                _HasPhysicalDamageCollision = value
            End Set
        End Property
        Public Property PhysicalDamageCollisionQuotedPremium As String 'covCodeId 21551; may not be populated
            Get
                Return qqHelper.QuotedPremiumFormat(_PhysicalDamageCollisionQuotedPremium)
            End Get
            Set(value As String)
                _PhysicalDamageCollisionQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_PhysicalDamageCollisionQuotedPremium)
            End Set
        End Property
        Public Property PhysicalDamageCollisionDeductibleId As String 'covCodeId 21551
            Get
                Return _PhysicalDamageCollisionDeductibleId
            End Get
            Set(value As String)
                _PhysicalDamageCollisionDeductibleId = value
            End Set
        End Property
        Public ReadOnly Property PhysicalDamageCollisionDeductible As String 'added 5/9/2017; still needs update to static data values
            Get
                Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.PhysicalDamageCollisionDeductibleId, _PhysicalDamageCollisionDeductibleId)
            End Get
        End Property
        Public Property HasGarageKeepersOtherThanCollision As Boolean 'covCodeId 21541
            Get
                Return _HasGarageKeepersOtherThanCollision
            End Get
            Set(value As Boolean)
                _HasGarageKeepersOtherThanCollision = value
            End Set
        End Property
        Public Property GarageKeepersOtherThanCollisionQuotedPremium As String 'covCodeId 21541
            Get
                Return qqHelper.QuotedPremiumFormat(_GarageKeepersOtherThanCollisionQuotedPremium)
            End Get
            Set(value As String)
                _GarageKeepersOtherThanCollisionQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_GarageKeepersOtherThanCollisionQuotedPremium)
            End Set
        End Property
        'Public Property GarageKeepersOtherThanCollisionManualLimitAmount As String 'covCodeId 21541; will stay w/ PolicyLevel and RiskLevel stuff
        '    Get
        '        Dim _garageKeepersOtcTotalLimitFromLocs As String = qqHelper.GarageKeepersOtherThanCollisionTotalLimitFromLocations(Locations, returnInLimitFormat:=True) '7/13/2018 note: was using _Locations
        '        If qqHelper.IsPositiveDecimalString(_garageKeepersOtcTotalLimitFromLocs) = True Then
        '            _GarageKeepersOtherThanCollisionManualLimitAmount = _garageKeepersOtcTotalLimitFromLocs
        '        End If
        '        Return _GarageKeepersOtherThanCollisionManualLimitAmount
        '    End Get
        '    Set(value As String)
        '        _GarageKeepersOtherThanCollisionManualLimitAmount = value
        '        qqHelper.ConvertToLimitFormat(_GarageKeepersOtherThanCollisionManualLimitAmount)
        '        qqHelper.UpdateGarageKeepersOtherThanCollisionAtLocationLevelBasedOnTotalLimit(value, Locations) '7/13/2018 note: was using _Locations
        '    End Set
        'End Property
        'Public Property GarageKeepersOtherThanCollisionBasisTypeId As String 'covDetail; covCodeId 21541; will stay w/ PolicyLevel and RiskLevel stuff
        '    Get
        '        Return _GarageKeepersOtherThanCollisionBasisTypeId
        '    End Get
        '    Set(value As String)
        '        _GarageKeepersOtherThanCollisionBasisTypeId = value
        '        If qqHelper.LocationCount(Locations) > 0 Then '7/13/2018 note: was using _Locations
        '            For Each l As QuickQuoteLocation In Locations
        '                If l IsNot Nothing Then
        '                    l.Set_GarageKeepersOtherThanCollisionBasisTypeId(_GarageKeepersOtherThanCollisionBasisTypeId)
        '                End If
        '            Next
        '        End If
        '    End Set
        'End Property
        'Public ReadOnly Property GarageKeepersOtherThanCollisionBasisType As String 'will stay w/ PolicyLevel and RiskLevel stuff
        '    Get
        '        Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.GarageKeepersOtherThanCollisionBasisTypeId, _GarageKeepersOtherThanCollisionBasisTypeId)
        '    End Get
        'End Property
        'Public Property GarageKeepersOtherThanCollisionDeductibleCategoryTypeId As String 'covDetail; covCodeId 21541; will stay w/ PolicyLevel and RiskLevel stuff
        '    Get
        '        Return _GarageKeepersOtherThanCollisionDeductibleCategoryTypeId
        '    End Get
        '    Set(value As String)
        '        _GarageKeepersOtherThanCollisionDeductibleCategoryTypeId = value
        '        If qqHelper.LocationCount(Locations) > 0 Then '7/13/2018 note: was using _Locations
        '            For Each l As QuickQuoteLocation In Locations
        '                If l IsNot Nothing Then
        '                    l.Set_GarageKeepersOtherThanCollisionDeductibleCategoryTypeId(_GarageKeepersOtherThanCollisionDeductibleCategoryTypeId)
        '                End If
        '            Next
        '        End If
        '    End Set
        'End Property
        'Public ReadOnly Property GarageKeepersOtherThanCollisionDeductibleCategoryType As String 'will stay w/ PolicyLevel and RiskLevel stuff
        '    Get
        '        Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.GarageKeepersOtherThanCollisionDeductibleCategoryTypeId, _GarageKeepersOtherThanCollisionDeductibleCategoryTypeId)
        '    End Get
        'End Property
        'Public Property GarageKeepersOtherThanCollisionTypeId As String 'covDetail; covCodeId 21541; will stay w/ PolicyLevel and RiskLevel stuff
        '    Get
        '        Return _GarageKeepersOtherThanCollisionTypeId
        '    End Get
        '    Set(value As String)
        '        _GarageKeepersOtherThanCollisionTypeId = value
        '        If qqHelper.LocationCount(Locations) > 0 Then '7/13/2018 note: was using _Locations
        '            For Each l As QuickQuoteLocation In Locations
        '                If l IsNot Nothing Then
        '                    l.Set_GarageKeepersOtherThanCollisionTypeId(_GarageKeepersOtherThanCollisionTypeId)
        '                End If
        '            Next
        '        End If
        '    End Set
        'End Property
        'Public ReadOnly Property GarageKeepersOtherThanCollisionType As String 'will stay w/ PolicyLevel and RiskLevel stuff
        '    Get
        '        Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.GarageKeepersOtherThanCollisionTypeId, _GarageKeepersOtherThanCollisionTypeId)
        '    End Get
        'End Property
        'Public Property GarageKeepersOtherThanCollisionDeductibleId As String 'covCodeId 21541; will stay w/ PolicyLevel and RiskLevel stuff
        '    Get
        '        Return _GarageKeepersOtherThanCollisionDeductibleId
        '    End Get
        '    Set(value As String)
        '        _GarageKeepersOtherThanCollisionDeductibleId = value
        '        If qqHelper.LocationCount(Locations) > 0 Then '7/13/2018 note: was using _Locations
        '            For Each l As QuickQuoteLocation In Locations
        '                If l IsNot Nothing Then
        '                    l.Set_GarageKeepersOtherThanCollisionDeductibleId(_GarageKeepersOtherThanCollisionDeductibleId)
        '                End If
        '            Next
        '        End If
        '    End Set
        'End Property
        'Public ReadOnly Property GarageKeepersOtherThanCollisionDeductible As String 'added 5/9/2017; still needs update to static data values; will stay w/ PolicyLevel and RiskLevel stuff
        '    Get
        '        Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.GarageKeepersOtherThanCollisionDeductibleId, _GarageKeepersOtherThanCollisionDeductibleId)
        '    End Get
        'End Property
        Public Property HasGarageKeepersCollision As Boolean 'covCodeId 21542
            Get
                Return _HasGarageKeepersCollision
            End Get
            Set(value As Boolean)
                _HasGarageKeepersCollision = value
            End Set
        End Property
        Public Property GarageKeepersCollisionQuotedPremium As String 'covCodeId 21542
            Get
                Return qqHelper.QuotedPremiumFormat(_GarageKeepersCollisionQuotedPremium)
            End Get
            Set(value As String)
                _GarageKeepersCollisionQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_GarageKeepersCollisionQuotedPremium)
            End Set
        End Property
        'Public Property GarageKeepersCollisionManualLimitAmount As String 'covCodeId 21542; will stay w/ PolicyLevel and RiskLevel stuff
        '    Get
        '        Dim _garageKeepersCollTotalLimitFromLocs As String = qqHelper.GarageKeepersCollisionTotalLimitFromLocations(Locations, returnInLimitFormat:=True) '7/13/2018 note: was using _Locations
        '        If qqHelper.IsPositiveDecimalString(_garageKeepersCollTotalLimitFromLocs) = True Then
        '            _GarageKeepersCollisionManualLimitAmount = _garageKeepersCollTotalLimitFromLocs
        '        End If
        '        Return _GarageKeepersCollisionManualLimitAmount
        '    End Get
        '    Set(value As String)
        '        _GarageKeepersCollisionManualLimitAmount = value
        '        qqHelper.ConvertToLimitFormat(_GarageKeepersCollisionManualLimitAmount)
        '        qqHelper.UpdateGarageKeepersCollisionAtLocationLevelBasedOnTotalLimit(value, Locations) '7/13/2018 note: was using _Locations
        '    End Set
        'End Property
        'Public Property GarageKeepersCollisionBasisTypeId As String 'covDetail; covCodeId 21542; will stay w/ PolicyLevel and RiskLevel stuff
        '    Get
        '        Return _GarageKeepersCollisionBasisTypeId
        '    End Get
        '    Set(value As String)
        '        _GarageKeepersCollisionBasisTypeId = value
        '        If qqHelper.LocationCount(Locations) > 0 Then '7/13/2018 note: was using _Locations
        '            For Each l As QuickQuoteLocation In Locations
        '                If l IsNot Nothing Then
        '                    l.Set_GarageKeepersCollisionBasisTypeId(_GarageKeepersCollisionBasisTypeId)
        '                End If
        '            Next
        '        End If
        '    End Set
        'End Property
        'Public ReadOnly Property GarageKeepersCollisionBasisType As String 'will stay w/ PolicyLevel and RiskLevel stuff
        '    Get
        '        Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.GarageKeepersCollisionBasisTypeId, _GarageKeepersCollisionBasisTypeId)
        '    End Get
        'End Property
        'Public Property GarageKeepersCollisionDeductibleId As String 'covCodeId 21542; will stay w/ PolicyLevel and RiskLevel stuff
        '    Get
        '        Return _GarageKeepersCollisionDeductibleId
        '    End Get
        '    Set(value As String)
        '        _GarageKeepersCollisionDeductibleId = value
        '        If qqHelper.LocationCount(Locations) > 0 Then '7/13/2018 note: was using _Locations
        '            For Each l As QuickQuoteLocation In Locations
        '                If l IsNot Nothing Then
        '                    l.Set_GarageKeepersCollisionDeductibleId(_GarageKeepersCollisionDeductibleId)
        '                End If
        '            Next
        '        End If
        '    End Set
        'End Property
        'Public ReadOnly Property GarageKeepersCollisionDeductible As String 'added 5/9/2017; still needs update to static data values; will stay w/ PolicyLevel and RiskLevel stuff
        '    Get
        '        Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.GarageKeepersCollisionDeductibleId, _GarageKeepersCollisionDeductibleId)
        '    End Get
        'End Property
        'Public Property GarageKeepersBasisTypeId As String 'covDetail; covCodeIds 21541 (OtherThanColl) and 21542 (Coll); will stay w/ PolicyLevel and RiskLevel stuff
        '    Get
        '        Dim gkBasisTypeId As String = ""

        '        If qqHelper.IsPositiveIntegerString(_GarageKeepersOtherThanCollisionBasisTypeId) = True Then
        '            gkBasisTypeId = _GarageKeepersOtherThanCollisionBasisTypeId
        '        ElseIf qqHelper.IsPositiveIntegerString(_GarageKeepersCollisionBasisTypeId) = True Then
        '            gkBasisTypeId = _GarageKeepersCollisionBasisTypeId
        '        ElseIf String.IsNullOrWhiteSpace(_GarageKeepersOtherThanCollisionBasisTypeId) = False Then
        '            gkBasisTypeId = _GarageKeepersOtherThanCollisionBasisTypeId
        '        ElseIf String.IsNullOrWhiteSpace(_GarageKeepersCollisionBasisTypeId) = False Then
        '            gkBasisTypeId = _GarageKeepersCollisionBasisTypeId
        '        End If

        '        Return gkBasisTypeId
        '    End Get
        '    Set(value As String)
        '        GarageKeepersOtherThanCollisionBasisTypeId = value
        '        GarageKeepersCollisionBasisTypeId = value
        '    End Set
        'End Property
        'Public ReadOnly Property GarageKeepersBasisType As String 'will stay w/ PolicyLevel and RiskLevel stuff
        '    Get
        '        Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteCoverage, QuickQuoteHelperClass.QuickQuotePropertyName.BasisTypeId, GarageKeepersBasisTypeId)
        '    End Get
        'End Property
        Public Property MultiLineDiscount As String
            Get
                Return _MultiLineDiscountValue
            End Get
            Set(value As String)
                _MultiLineDiscountValue = value
            End Set
        End Property
        'added 4/1/2019 - Bug 30754 - DJG
        Public Property HasAdvancedQuoteDiscount As Boolean
            Get
                Return _HasAdvancedQuoteDiscount
            End Get
            Set(value As Boolean)
                _HasAdvancedQuoteDiscount = value
            End Set
        End Property
        Public Property HasFarmIndicator As Boolean
            Get
                Return _HasFarmIndicator
            End Get
            Set(value As Boolean)
                _HasFarmIndicator = value
            End Set
        End Property
        Public Property PriorBodilyInjuryLimitId As String
            Get
                Return _PriorBodilyInjuryLimitId
            End Get
            Set(value As String)
                _PriorBodilyInjuryLimitId = value
            End Set
        End Property

        'added 9/25/2018 for multi-state
        Public Property Liability_UM_UIM_DeductibleId As String 'covCodeId 21552
            Get
                Return _Liability_UM_UIM_DeductibleId
            End Get
            Set(value As String)
                _Liability_UM_UIM_DeductibleId = value
            End Set
        End Property
        Public Property UninsuredMotoristPropertyDamageLimitId As String 'covCodeId 21539; note: same prop exists on Vehicle
            Get
                Return _UninsuredMotoristPropertyDamageLimitId
            End Get
            Set(value As String)
                _UninsuredMotoristPropertyDamageLimitId = value
            End Set
        End Property
        Public Property UninsuredMotoristPropertyDamageDeductibleId As String 'covCodeId 21539
            Get
                Return _UninsuredMotoristPropertyDamageDeductibleId
            End Get
            Set(value As String)
                _UninsuredMotoristPropertyDamageDeductibleId = value
            End Set
        End Property
        Public Property UnderinsuredMotoristBodilyInjuryLiabilityLimitId As String 'covCodeId 21548
            Get
                Return _UnderinsuredMotoristBodilyInjuryLiabilityLimitId
            End Get
            Set(value As String)
                _UnderinsuredMotoristBodilyInjuryLiabilityLimitId = value
            End Set
        End Property
        Public Property UnderinsuredMotoristBodilyInjuryLiabilityQuotedPremium As String 'covCodeId 21548
            Get
                Return qqHelper.QuotedPremiumFormat(_UnderinsuredMotoristBodilyInjuryLiabilityQuotedPremium)
            End Get
            Set(value As String)
                _UnderinsuredMotoristBodilyInjuryLiabilityQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_UnderinsuredMotoristBodilyInjuryLiabilityQuotedPremium)
            End Set
        End Property

        'added 7/9/2021
        Public Property HasFoodManufacturersEnhancement As Boolean 'covCodeId 100000
            Get
                Return _HasFoodManufacturersEnhancement
            End Get
            Set(value As Boolean)
                _HasFoodManufacturersEnhancement = value
            End Set
        End Property
        Public Property FoodManufacturersEnhancementQuotedPremium As String 'covCodeId 100000
            Get
                Return qqHelper.QuotedPremiumFormat(_FoodManufacturersEnhancementQuotedPremium)
            End Get
            Set(value As String)
                _FoodManufacturersEnhancementQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_FoodManufacturersEnhancementQuotedPremium)
            End Set
        End Property
        Public Property CPP_CPR_FoodManufacturersEnhancementQuotedPremium As String 'covCodeId 100000
            Get
                Return qqHelper.QuotedPremiumFormat(_CPP_CPR_FoodManufacturersEnhancementQuotedPremium)
            End Get
            Set(value As String)
                _CPP_CPR_FoodManufacturersEnhancementQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_CPP_CPR_FoodManufacturersEnhancementQuotedPremium)
            End Set
        End Property
        Public Property CPP_CGL_FoodManufacturersEnhancementQuotedPremium As String 'covCodeId 100000
            Get
                Return qqHelper.QuotedPremiumFormat(_CPP_CGL_FoodManufacturersEnhancementQuotedPremium)
            End Get
            Set(value As String)
                _CPP_CGL_FoodManufacturersEnhancementQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_CPP_CGL_FoodManufacturersEnhancementQuotedPremium)
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80572 (HOM)</remarks>
        Public Property HasFamilyCyberProtection As Boolean
            Get
                Return _HasFamilyCyberProtection
            End Get
            Set(value As Boolean)
                _HasFamilyCyberProtection = value
            End Set
        End Property
        Public Property FamilyCyberProtectionQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_FamilyCyberProtectionQuotedPremium)
            End Get
            Set(value As String)
                _FamilyCyberProtectionQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_FamilyCyberProtectionQuotedPremium)
            End Set
        End Property

        'note: may not be able to Serialize Protected Friend; updated 8/18/2018 to Friend and then Public... may need to come up w/ some solution to prevent these from being used by Devs
        Public Property BasePolicyLevelInfo As QuickQuoteLobPolicyLevelInfo_AppliedToAllStates
            Get
                If _BasePolicyLevelInfo Is Nothing Then
                    _BasePolicyLevelInfo = New QuickQuoteLobPolicyLevelInfo_AppliedToAllStates
                End If
                SetObjectsParent(_BasePolicyLevelInfo)
                Return _BasePolicyLevelInfo
            End Get
            Set(value As QuickQuoteLobPolicyLevelInfo_AppliedToAllStates)
                _BasePolicyLevelInfo = value
                SetObjectsParent(_BasePolicyLevelInfo)
            End Set
        End Property
      
        'added 4/20/2020 for PUP/FUP
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property FarmSizeTypeId As String 'static data
            Get
                Return _BasePolicyLevelInfo.FarmSizeTypeId
            End Get
            Set(value As String)
                _BasePolicyLevelInfo.FarmSizeTypeId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property FarmTypeId As String 'static data
            Get
                Return _BasePolicyLevelInfo.FarmTypeId
            End Get
            Set(value As String)
                _BasePolicyLevelInfo.FarmTypeId = value
            End Set
        End Property
                
        'added 5/6/2021 KLJ
        'Need this for Umbrella but it is ignored in all other cases
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property ManualAggregateLiabilityLimit() As String
            Get
                return _BasePolicyLevelInfo.ManualAggregateLiabilityLimit
            End Get
            Set(value As String)
                _BasePolicyLevelInfo.ManualAggregateLiabilityLimit = qqHelper.DiamondAmountFormat(value)
            End Set
        end property

        Public Property HasFarmAllStar As Boolean
            Get
                Return _HasFarmAllStar
            End Get
            Set(value As Boolean)
                _HasFarmAllStar = value
            End Set
        End Property

        Public Property FarmAllStarWaterBackupLimitId As String 'static data
            Get
                Return _FarmAllStarWaterBackupLimitId
            End Get
            Set(value As String)
                _FarmAllStarWaterBackupLimitId = value
            End Set
        End Property

        Public Property FarmAllStarWaterDamageLimitId As String 'static data
            Get
                Return _FarmAllStarWaterDamageLimitId
            End Get
            Set(value As String)
                _FarmAllStarWaterDamageLimitId = value
            End Set
        End Property

        ''' <remarks>CovCodeId 80446</remarks>
        Public Property OwnersLesseesorContractorsCompletedOperationsTotalPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_OwnersLesseesorContractorsCompletedOperationsTotalPremium)
            End Get
            Set(value As String)
                _OwnersLesseesorContractorsCompletedOperationsTotalPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_OwnersLesseesorContractorsCompletedOperationsTotalPremium)
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
            _BasePolicyLevelInfo = New QuickQuoteLobPolicyLevelInfo_AppliedToAllStates

            'PolicyLevel
            _CPP_CRM_ProgramTypeId = ""
            _CPP_GAR_ProgramTypeId = ""
            _RiskGradeLookupId_Original = ""
            _CPP_CGL_RiskGrade = ""
            _CPP_CGL_RiskGradeLookupId = ""
            _CPP_CPR_RiskGrade = ""
            _CPP_CPR_RiskGradeLookupId = ""
            _ErrorRiskGradeLookupId = ""
            _ReplacementRiskGradeLookupId = ""
            _CPP_CGL_ErrorRiskGradeLookupId = ""
            _CPP_CGL_ReplacementRiskGradeLookupId = ""
            _CPP_CPR_ErrorRiskGradeLookupId = ""
            _CPP_CPR_ReplacementRiskGradeLookupId = ""
            _CPP_CIM_RiskGrade = ""
            _CPP_CIM_RiskGradeLookupId = ""
            _CPP_CIM_ErrorRiskGradeLookupId = ""
            _CPP_CIM_ReplacementRiskGradeLookupId = ""
            _CPP_CRM_RiskGrade = ""
            _CPP_CRM_RiskGradeLookupId = ""
            _CPP_CRM_ErrorRiskGradeLookupId = ""
            _CPP_CRM_ReplacementRiskGradeLookupId = ""
            _OccurrenceLiabilityLimit = ""
            _OccurrenceLiabilityLimitId = ""
            _OccurrencyLiabilityQuotedPremium = ""
            _TenantsFireLiability = ""
            _TenantsFireLiabilityId = ""
            _TenantsFireLiabilityQuotedPremium = ""
            _PropertyDamageLiabilityDeductible = ""
            _PropertyDamageLiabilityDeductibleId = ""
            _BlanketRatingQuotedPremium = ""
            _HasEnhancementEndorsement = False
            _EnhancementEndorsementQuotedPremium = ""
            _Has_PackageGL_EnhancementEndorsement = False
            _PackageGL_EnhancementEndorsementQuotedPremium = ""
            _Has_PackageGL_PlusEnhancementEndorsement = False
            _PackageGL_EnhancementEndorsementQuotedPremium = ""
            _Has_PackageCPR_EnhancementEndorsement = False
            _PackageCPR_EnhancementEndorsementQuotedPremium = ""
            '_AdditionalInsuredsCount = 0
            '_AdditionalInsuredsCheckboxBOP = Nothing
            '_HasAdditionalInsuredsCheckboxBOP = False
            '_AdditionalInsuredsManualCharge = ""
            '_AdditionalInsuredsQuotedPremium = ""
            '_AdditionalInsureds = Nothing
            '_AdditionalInsuredsBackup = Nothing
            _EmployeeBenefitsLiabilityText = ""
            _EmployeeBenefitsLiabilityOccurrenceLimit = ""
            _EmployeeBenefitsLiabilityOccurrenceLimitId = ""
            _EmployeeBenefitsLiabilityQuotedPremium = ""
            _EmployeeBenefitsLiabilityRetroactiveDate = ""
            _EmployeeBenefitsLiabilityAggregateLimit = ""
            _EmployeeBenefitsLiabilityDeductible = ""
            _ContractorsEquipmentInstallationLimit = ""
            _ContractorsEquipmentInstallationLimitId = ""
            _ContractorsEquipmentInstallationLimitQuotedPremium = ""
            _ContractorsToolsEquipmentBlanket = ""
            _ContractorsToolsEquipmentBlanketSubLimitId = ""
            _ContractorsToolsEquipmentBlanketQuotedPremium = ""
            _ContractorsToolsEquipmentScheduled = ""
            _ContractorsToolsEquipmentScheduledQuotedPremium = ""
            _ContractorsToolsEquipmentRented = ""
            _ContractorsToolsEquipmentRentedQuotedPremium = ""
            _ContractorsEquipmentScheduledItems = Nothing
            _ContractorsEquipmentScheduledItemsBackup = Nothing
            _ContractorsEmployeeTools = ""
            _ContractorsEmployeeToolsQuotedPremium = ""
            _CrimeEmpDisEmployeeText = ""
            _CrimeEmpDisLocationText = ""
            _CrimeEmpDisLimit = ""
            _CrimeEmpDisLimitId = ""
            _CrimeEmpDisQuotedPremium = ""
            _CrimeForgeryLimit = ""
            _CrimeForgeryLimitId = ""
            _CrimeForgeryQuotedPremium = ""
            _HasEarthquake = False
            _EarthquakeQuotedPremium = ""
            _HasHiredAuto = False
            _HiredAutoQuotedPremium = ""
            _HasNonOwnedAuto = False
            _NonOwnedAutoWithDelivery = False
            _NonOwnedAutoQuotedPremium = ""
            _PropertyDeductibleId = ""
            _EmployersLiability = ""
            _EmployersLiabilityId = ""
            _EmployersLiabilityQuotedPremium = ""
            _GeneralAggregateLimit = ""
            _GeneralAggregateLimitId = ""
            _GeneralAggregateQuotedPremium = ""
            _ProductsCompletedOperationsAggregateLimit = ""
            _ProductsCompletedOperationsAggregateLimitId = ""
            _ProductsCompletedOperationsAggregateQuotedPremium = ""
            _PersonalAndAdvertisingInjuryLimit = ""
            _PersonalAndAdvertisingInjuryLimitId = ""
            _PersonalAndAdvertisingInjuryQuotedPremium = ""
            _DamageToPremisesRentedLimit = ""
            _DamageToPremisesRentedLimitId = ""
            _DamageToPremisesRentedQuotedPremium = ""
            _MedicalExpensesLimit = ""
            _MedicalExpensesLimitId = ""
            _MedicalExpensesQuotedPremium = ""
            '_HasExclusionOfAmishWorkers = False
            '_HasExclusionOfSoleProprietorsPartnersOfficersAndOthers = False
            '_HasInclusionOfSoleProprietorsPartnersOfficersAndOthers = False
            '_HasWaiverOfSubrogation = False
            '_WaiverOfSubrogationNumberOfWaivers = 0
            '_WaiverOfSubrogationPremium = ""
            '_WaiverOfSubrogationPremiumId = ""
            '_NeedsToUpdateWaiverOfSubrogationPremiumId = False
            '_ExclusionOfAmishWorkerRecords = Nothing
            '_ExclusionOfSoleProprietorRecords = Nothing
            '_InclusionOfSoleProprietorRecords = Nothing
            '_WaiverOfSubrogationRecords = Nothing
            '_ExclusionOfAmishWorkerRecordsBackup = Nothing
            '_ExclusionOfSoleProprietorRecordsBackup = Nothing
            '_InclusionOfSoleProprietorRecordsBackup = Nothing
            '_WaiverOfSubrogationRecordsBackup = Nothing
            '_HasBarbersProfessionalLiability = False
            '_BarbersProfessionalLiabiltyQuotedPremium = ""
            '_BarbersProfessionalLiabilityFullTimeEmpNum = ""
            '_BarbersProfessionalLiabilityPartTimeEmpNum = ""
            '_BarbersProfessionalLiabilityDescription = ""
            '_HasBeauticiansProfessionalLiability = False
            '_BeauticiansProfessionalLiabilityQuotedPremium = ""
            '_BeauticiansProfessionalLiabilityFullTimeEmpNum = ""
            '_BeauticiansProfessionalLiabilityPartTimeEmpNum = ""
            '_BeauticiansProfessionalLiabilityDescription = ""
            '_HasFuneralDirectorsProfessionalLiability = False
            '_FuneralDirectorsProfessionalLiabilityQuotedPremium = ""
            '_FuneralDirectorsProfessionalLiabilityEmpNum = ""
            '_HasPrintersProfessionalLiability = False
            '_PrintersProfessionalLiabilityQuotedPremium = ""
            '_PrintersProfessionalLiabilityLocNum = ""
            '_HasSelfStorageFacility = False
            '_SelfStorageFacilityQuotedPremium = ""
            '_SelfStorageFacilityLimit = ""
            '_HasVeterinariansProfessionalLiability = False
            '_VeterinariansProfessionalLiabilityEmpNum = ""
            '_VeterinariansProfessionalLiabilityQuotedPremium = ""
            '_HasPharmacistProfessionalLiability = False
            '_PharmacistAnnualGrossSales = ""
            '_PharmacistQuotedPremium = ""
            '_HasOpticalAndHearingAidProfessionalLiability = False
            '_OpticalAndHearingAidProfessionalLiabilityEmpNum = ""
            '_OpticalAndHearingAidProfessionalLiabilityQuotedPremium = ""
            '_HasMotelCoverage = False
            '_MotelCoveragePerGuestLimitId = ""
            '_MotelCoveragePerGuestLimit = ""
            '_MotelCoveragePerGuestQuotedPremium = ""
            '_MotelCoverageSafeDepositLimitId = ""
            '_MotelCoverageSafeDepositDeductibleId = ""
            '_MotelCoverageSafeDepositLimit = ""
            '_MotelCoverageSafeDepositDeductible = ""
            '_MotelCoverageQuotedPremium = ""
            '_MotelCoverageSafeDepositQuotedPremium = ""
            '_HasPhotographyCoverage = False
            '_HasPhotographyCoverageScheduledCoverages = False
            '_PhotographyScheduledCoverages = Nothing
            '_HasPhotographyMakeupAndHair = False
            '_PhotographyMakeupAndHairQuotedPremium = ""
            '_PhotographyCoverageQuotedPremium = ""
            '_HasLiquorLiability = False
            '_LiquorLiabilityClassCodeTypeId = "" '12 = 58161 - Restaurant Includes Package Sales, 13 = 59211 - Package Sales for Consumption Off Premises
            '_LiquorLiabilityAnnualGrossPackageSalesReceipts = ""
            '_LiquorLiabilityAnnualGrossAlcoholSalesReceipts = ""
            '_HasResidentialCleaning = False
            '_ResidentialCleaningQuotedPremium = ""
            '_LiquorLiabilityOccurrenceLimit = ""
            '_LiquorLiabilityOccurrenceLimitId = ""
            '_LiquorLiabilityClassification = ""
            '_LiquorLiabilityClassificationId = ""
            '_LiquorSales = ""
            '_LiquorLiabilityQuotedPremium = ""
            '_ProfessionalLiabilityCemetaryNumberOfBurials = ""
            '_ProfessionalLiabilityCemetaryQuotedPremium = ""
            '_ProfessionalLiabilityFuneralDirectorsNumberOfBodies = ""
            '_ProfessionalLiabilityPastoralNumberOfClergy = ""
            '_ProfessionalLiabilityPastoralQuotedPremium = ""
            Reset_IRPM_Values()
            _GL_PremisesAndProducts_Deductible = ""
            _GL_PremisesAndProducts_DeductibleId = ""
            _GL_PremisesAndProducts_Description = ""
            _GL_PremisesAndProducts_DeductibleCategoryType = ""
            _GL_PremisesAndProducts_DeductibleCategoryTypeId = ""
            _GL_PremisesAndProducts_DeductiblePerType = ""
            _GL_PremisesAndProducts_DeductiblePerTypeId = ""
            _Has_GL_PremisesAndProducts = False
            _GL_PremisesTotalQuotedPremium = ""
            _GL_ProductsTotalQuotedPremium = ""
            _GL_PremisesPolicyLevelQuotedPremium = ""
            _GL_ProductsPolicyLevelQuotedPremium = ""
            _GL_PremisesMinimumQuotedPremium = ""
            _GL_PremisesMinimumPremiumAdjustment = ""
            _GL_ProductsMinimumQuotedPremium = ""
            _GL_ProductsMinimumPremiumAdjustment = ""
            _HasFarmPollutionLiability = False
            _FarmPollutionLiabilityQuotedPremium = ""
            _HasHiredBorrowedNonOwned = False
            _HasNonOwnershipLiability = False
            _NonOwnershipLiabilityNumberOfEmployees = ""
            _NonOwnership_ENO_RatingTypeId = ""
            _NonOwnership_ENO_RatingType = ""
            _NonOwnershipLiabilityQuotedPremium = ""
            _HasHiredBorrowedLiability = False
            _HiredBorrowedLiabilityQuotedPremium = ""
            _HasHiredCarPhysicalDamage = False
            _HiredBorrowedLossOfUseQuotedPremium = ""
            _ComprehensiveDeductible = ""
            _ComprehensiveDeductibleId = ""
            _ComprehensiveQuotedPremium = ""
            _CollisionDeductible = ""
            _CollisionDeductibleId = ""
            _CollisionQuotedPremium = ""
            _Liability_UM_UIM_Limit = ""
            _Liability_UM_UIM_LimitId = ""
            _Liability_UM_UIM_QuotedPremium = ""
            _MedicalPaymentsLimit = ""
            _MedicalPaymentsLimitId = ""
            _MedicalPaymentsQuotedPremium = ""
            _QuoteOrIssueBound = QuickQuoteObject.QuickQuoteQuoteOrIssueBound.Quote
            _IssueBoundEffectiveDate = ""
            _LiabilityAutoSymbolObject = New QuickQuoteDeveloperAutoSymbol
            _MedicalPaymentsAutoSymbolObject = New QuickQuoteDeveloperAutoSymbol
            _UninsuredMotoristAutoSymbolObject = New QuickQuoteDeveloperAutoSymbol
            _UnderinsuredMotoristAutoSymbolObject = New QuickQuoteDeveloperAutoSymbol
            _ComprehensiveCoverageAutoSymbolObject = New QuickQuoteDeveloperAutoSymbol
            _CollisionCoverageAutoSymbolObject = New QuickQuoteDeveloperAutoSymbol
            _NonOwnershipAutoSymbolObject = New QuickQuoteDeveloperAutoSymbol
            _HiredBorrowedAutoSymbolObject = New QuickQuoteDeveloperAutoSymbol
            _TowingAndLaborAutoSymbolObject = New QuickQuoteDeveloperAutoSymbol
            _UseDeveloperAutoSymbols = False
            _CAP_Liability_WouldHaveSymbol8 = False
            _CAP_Liability_WouldHaveSymbol9 = False
            _CAP_Comprehensive_WouldHaveSymbol8 = False
            _CAP_Collision_WouldHaveSymbol8 = False
            _HasBlanketBuilding = False
            _HasBlanketContents = False
            _HasBlanketBuildingAndContents = False
            _HasBlanketBusinessIncome = False
            _BlanketBuildingQuotedPremium = ""
            _BlanketContentsQuotedPremium = ""
            _BlanketBuildingAndContentsQuotedPremium = ""
            _BlanketBusinessIncomeQuotedPremium = ""
            _BlanketBuildingCauseOfLossTypeId = ""
            _BlanketBuildingCauseOfLossType = ""
            _BlanketContentsCauseOfLossTypeId = ""
            _BlanketContentsCauseOfLossType = ""
            _BlanketBuildingAndContentsCauseOfLossTypeId = ""
            _BlanketBuildingAndContentsCauseOfLossType = ""
            _BlanketBusinessIncomeCauseOfLossTypeId = ""
            _BlanketBusinessIncomeCauseOfLossType = ""
            _BlanketBuildingLimit = ""
            _BlanketBuildingCoinsuranceTypeId = ""
            _BlanketBuildingCoinsuranceType = ""
            _BlanketBuildingValuationId = ""
            _BlanketBuildingValuation = ""
            _BlanketContentsLimit = ""
            _BlanketContentsCoinsuranceTypeId = ""
            _BlanketContentsCoinsuranceType = ""
            _BlanketContentsValuationId = ""
            _BlanketContentsValuation = ""
            _BlanketBuildingAndContentsLimit = ""
            _BlanketBuildingAndContentsCoinsuranceTypeId = ""
            _BlanketBuildingAndContentsCoinsuranceType = ""
            _BlanketBuildingAndContentsValuationId = ""
            _BlanketBuildingAndContentsValuation = ""
            _BlanketBusinessIncomeLimit = ""
            _BlanketBusinessIncomeCoinsuranceTypeId = ""
            _BlanketBusinessIncomeCoinsuranceType = ""
            _BlanketBusinessIncomeValuationId = ""
            _BlanketBusinessIncomeValuation = ""
            _CPR_BlanketCoverages_TotalPremium = ""
            _BlanketCombinedEarthquake_QuotedPremium = ""
            _BlanketBuildingIsAgreedValue = False
            _BlanketContentsIsAgreedValue = False
            _BlanketBuildingAndContentsIsAgreedValue = False
            _BlanketBusinessIncomeIsAgreedValue = False
            _UseTierOverride = False
            _TierAdjustmentTypeId = ""
            _PersonalLiabilityLimitId = ""
            _PersonalLiabilityQuotedPremium = ""
            _HasEPLI = False
            _EPLI_Applied = False
            _EPLIPremium = ""
            _EPLICoverageLimitId = ""
            _EPLIDeductibleId = ""
            _EPLICoverageTypeId = ""
            '_BlanketWaiverOfSubrogation = 0  ' 0 = None; 1 = CGL1004; 2 = CGL1002; 3/5/2015 note: may need to be updated to empty string; now use this for 3 = CAP and 4 = WCP
            '_BlanketWaiverOfSubrogationQuotedPremium = ""
            _HasCondoDandO = False
            _CondoDandOAssociatedName = ""
            _CondoDandODeductibleId = ""
            _CondoDandOPremium = ""
            _CondoDandOManualLimit = ""
            _Farm_F_and_G_DeductibleLimitId = "" 'static data
            _Farm_F_and_G_DeductibleQuotedPremium = ""
            _HasFarmEquipmentBreakdown = False
            _FarmEquipmentBreakdownQuotedPremium = ""
            _HasFarmExtender = False
            _FarmExtenderQuotedPremium = ""
            _FarmAllStarLimitId = ""
            _FarmAllStarQuotedPremium = ""
            _HasFarmEmployersLiability = False
            _FarmEmployersLiabilityQuotedPremium = ""
            _FarmFireLegalLiabilityLimitId = ""
            _FarmFireLegalLiabilityQuotedPremium = ""
            _HasFarmPersonalAndAdvertisingInjury = False
            _FarmPersonalAndAdvertisingInjuryQuotedPremium = ""
            _FarmContractGrowersCareCustodyControlLimitId = ""
            _FarmContractGrowersCareCustodyControlDescription = ""
            _FarmContractGrowersCareCustodyControlQuotedPremium = ""
            _HasFarmExclusionOfProductsCompletedWork = False
            _FarmExclusionOfProductsCompletedWorkQuotedPremium = ""
            FarmIncidentalLimits = Nothing 'goes w/ FarmIncidentalLimitCoverages
            _HasBusinessIncomeALS = False
            _BusinessIncomeALSLimit = ""
            _BusinessIncomeALSQuotedPremium = ""
            _HasContractorsEnhancement = False
            _ContractorsEnhancementQuotedPremium = ""
            _CPP_CPR_ContractorsEnhancementQuotedPremium = ""
            _CPP_CGL_ContractorsEnhancementQuotedPremium = ""
            _CPP_CIM_ContractorsEnhancementQuotedPremium = ""
            _HasManufacturersEnhancement = False
            _ManufacturersEnhancementQuotedPremium = ""
            _CPP_CPR_ManufacturersEnhancementQuotedPremium = ""
            _CPP_CGL_ManufacturersEnhancementQuotedPremium = ""
            _FarmMachinerySpecialCoverageG_QuotedPremium = ""
            _HasAutoPlusEnhancement = False
            _AutoPlusEnhancement_QuotedPremium = ""
            '_HasApartmentBuildings = False
            '_NumberOfLocationsWithApartments = ""
            '_ApartmentQuotedPremium = ""
            '_HasRestaurantEndorsement = False
            '_RestaurantQuotedPremium = ""
            _Liability_UM_UIM_AggregateLiabilityIncrementTypeId = "" 'covDetail; covCodeId 21552
            _Liability_UM_UIM_DeductibleCategoryTypeId = "" 'covDetail; covCodeId 21552
            _HasUninsuredMotoristPropertyDamage = False 'covCodeId 21539
            _UninsuredMotoristPropertyDamageQuotedPremium = "" 'covCodeId 21539; may not be populated
            _MedicalPaymentsTypeId = "" 'covDetail; covCodeId 21540
            _HasPhysicalDamageOtherThanCollision = False 'covCodeId 21550; may not be populated
            _PhysicalDamageOtherThanCollisionQuotedPremium = "" 'covCodeId 21550
            _HasPhysicalDamageCollision = False 'covCodeId 21551
            _PhysicalDamageCollisionQuotedPremium = "" 'covCodeId 21551; may not be populated
            _PhysicalDamageCollisionDeductibleId = "" 'covCodeId 21551
            _HasGarageKeepersOtherThanCollision = False 'covCodeId 21541
            _GarageKeepersOtherThanCollisionQuotedPremium = "" 'covCodeId 21541
            '_GarageKeepersOtherThanCollisionManualLimitAmount = "" 'covCodeId 21541; will stay w/ PolicyLevel and RiskLevel stuff
            '_GarageKeepersOtherThanCollisionBasisTypeId = "" 'covDetail; covCodeId 21541; will stay w/ PolicyLevel and RiskLevel stuff
            '_GarageKeepersOtherThanCollisionDeductibleCategoryTypeId = "" 'covDetail; covCodeId 21541; will stay w/ PolicyLevel and RiskLevel stuff
            '_GarageKeepersOtherThanCollisionTypeId = "" 'covDetail; covCodeId 21541; will stay w/ PolicyLevel and RiskLevel stuff
            '_GarageKeepersOtherThanCollisionDeductibleId = "" 'covCodeId 21541; will stay w/ PolicyLevel and RiskLevel stuff
            _HasGarageKeepersCollision = False 'covCodeId 21542
            _GarageKeepersCollisionQuotedPremium = "" 'covCodeId 21542
            '_GarageKeepersCollisionManualLimitAmount = "" 'covCodeId 21542; will stay w/ PolicyLevel and RiskLevel stuff
            '_GarageKeepersCollisionBasisTypeId = "" 'covDetail; covCodeId 21542; will stay w/ PolicyLevel and RiskLevel stuff
            '_GarageKeepersCollisionDeductibleId = "" 'covCodeId 21542; will stay w/ PolicyLevel and RiskLevel stuff
            _MultiLineDiscountValue = ""
            _PriorBodilyInjuryLimitId = ""

            'added 9/25/2018 for multi-state
            _Liability_UM_UIM_DeductibleId = "" 'covCodeId 21552
            _UninsuredMotoristPropertyDamageLimitId = "" 'covCodeId 21539; note: same prop exists on Vehicle
            _UninsuredMotoristPropertyDamageDeductibleId = "" 'covCodeId 21539
            _UnderinsuredMotoristBodilyInjuryLiabilityLimitId = "" 'covCodeId 21548
            _UnderinsuredMotoristBodilyInjuryLiabilityQuotedPremium = "" 'covCodeId 21548

            'added 7/9/2021
            _HasFoodManufacturersEnhancement = False 'covCodeId 100000
            _FoodManufacturersEnhancementQuotedPremium = "" 'covCodeId 100000
            _CPP_CPR_FoodManufacturersEnhancementQuotedPremium = "" 'covCodeId 100000
            _CPP_CGL_FoodManufacturersEnhancementQuotedPremium = "" 'covCodeId 100000

            'Added 6/27/2022 for task 75780 MLW
            _Has_PackageCPR_PlusEnhancementEndorsement = False
            _PackageCPR_PlusEnhancementEndorsementQuotedPremium = ""

            'Added 08/21/2023 for task WS-855
            _HasFarmIndicator = False

            _QuoteEffectiveDate = ""
            _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.None
            _LobType = QuickQuoteObject.QuickQuoteLobType.None

            _HasFarmAllStar = False
            _FarmAllStarWaterBackupLimitId = ""
            _FarmAllStarWaterDamageLimitId = ""
            _OwnersLesseesorContractorsCompletedOperationsTotalPremium = ""

        End Sub
        Public Sub Reset_IRPM_Values() 'added 7/9/2018
            _IRPM_ManagementCooperation = "1.000"
            _IRPM_ManagementCooperationDesc = ""
            _IRPM_Location = "1.000"
            _IRPM_LocationDesc = ""
            _IRPM_BuildingFeatures = "1.000"
            _IRPM_BuildingFeaturesDesc = ""
            _IRPM_Premises = "1.000"
            _IRPM_PremisesDesc = ""
            _IRPM_Employees = "1.000"
            _IRPM_EmployeesDesc = ""
            _IRPM_Protection = "1.000"
            _IRPM_ProtectionDesc = ""
            _IRPM_CatostrophicHazards = "1.000"
            _IRPM_CatostrophicHazardsDesc = ""
            _IRPM_ManagementExperience = "1.000"
            _IRPM_ManagementExperienceDesc = ""
            _IRPM_Equipment = "1.000"
            _IRPM_EquipmentDesc = ""
            _IRPM_MedicalFacilities = "1.000"
            _IRPM_MedicalFacilitiesDesc = ""
            _IRPM_ClassificationPeculiarities = "1.000"
            _IRPM_ClassificationPeculiaritiesDesc = ""
            _IRPM_GL_ManagementCooperation = "1.000"
            _IRPM_GL_ManagementCooperationDesc = ""
            _IRPM_GL_Location = "1.000"
            _IRPM_GL_LocationDesc = ""
            _IRPM_GL_Premises = "1.000"
            _IRPM_GL_PremisesDesc = ""
            _IRPM_GL_Equipment = "1.000"
            _IRPM_GL_EquipmentDesc = ""
            _IRPM_GL_Employees = "1.000"
            _IRPM_GL_EmployeesDesc = ""
            _IRPM_GL_ClassificationPeculiarities = "1.000"
            _IRPM_GL_ClassificationPeculiaritiesDesc = ""
            _IRPM_CAP_Management = "1.000"
            _IRPM_CAP_ManagementDesc = ""
            _IRPM_CAP_Employees = "1.000"
            _IRPM_CAP_EmployeesDesc = ""
            _IRPM_CAP_Equipment = "1.000"
            _IRPM_CAP_EquipmentDesc = ""
            _IRPM_CAP_SafetyOrganization = "1.000"
            _IRPM_CAP_SafetyOrganizationDesc = ""
            _IRPM_CAP_Management_Phys_Damage = "1.000"
            _IRPM_CAP_ManagementDesc_Phys_Damage = ""
            _IRPM_CAP_Employees_Phys_Damage = "1.000"
            _IRPM_CAP_EmployeesDesc_Phys_Damage = ""
            _IRPM_CAP_Equipment_Phys_Damage = "1.000"
            _IRPM_CAP_EquipmentDesc_Phys_Damage = ""
            _IRPM_CAP_SafetyOrganization_Phys_Damage = "1.000"
            _IRPM_CAP_SafetyOrganizationDesc_Phys_Damage = ""
            _IRPM_CPR_Management = "1.000"
            _IRPM_CPR_ManagementDesc = ""
            _IRPM_CPR_PremisesAndEquipment = "1.000"
            _IRPM_CPR_PremisesAndEquipmentDesc = ""
            _IRPM_FAR_CareConditionOfEquipPremises = "1.000"
            _IRPM_FAR_CareConditionOfEquipPremisesDesc = ""
            _IRPM_FAR_Cooperation = "1.000"
            _IRPM_FAR_CooperationDesc = ""
            _IRPM_FAR_DamageSusceptibility = "1.000"
            _IRPM_FAR_DamageSusceptibilityDesc = ""
            _IRPM_FAR_DispersionOrConcentration = "1.000"
            _IRPM_FAR_DispersionOrConcentrationDesc = ""
            _IRPM_FAR_SuperiorOrInferiorStructureFeatures = "1.000"
            _IRPM_FAR_SuperiorOrInferiorStructureFeaturesDesc = ""
            _IRPM_FAR_UseOfRiceHullsOrFlameRetardantBedding = "1.000"
            _IRPM_FAR_UseOfRiceHullsOrFlameRetardantBeddingDesc = ""
            _IRPM_FAR_Location = "1.000"
            _IRPM_FAR_LocationDesc = ""
            _IRPM_FAR_MiscProtectFeaturesOrHazards = "1.000"
            _IRPM_FAR_MiscProtectFeaturesOrHazardsDesc = ""
            _IRPM_FAR_RoofCondition = "1.000"
            _IRPM_FAR_RoofConditionDesc = ""
            _IRPM_FAR_StoragePracticesAndHazardousOperations = "1.000"
            _IRPM_FAR_StoragePracticesAndHazardousOperationsDesc = ""
            _IRPM_FAR_PastLosses = "1.000"
            _IRPM_FAR_PastLossesDesc = ""
            _IRPM_FAR_SupportingBusiness = "1.000"
            _IRPM_FAR_SupportingBusinessDesc = ""
            _IRPM_FAR_RegularOnsiteInspections = "1.000"
            _IRPM_FAR_RegularOnsiteInspectionsDesc = ""
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
        Protected Friend Function Get_ContractorsToolsEquipmentScheduled_Variable() As String
            Return _ContractorsToolsEquipmentScheduled
        End Function
        Protected Friend Sub Set_ContractorsToolsEquipmentScheduled_Variable(ByVal contToolsEquipSch As String, Optional ByVal convertToLimitFormat As Boolean = False)
            _ContractorsToolsEquipmentScheduled = contToolsEquipSch
            If convertToLimitFormat = True Then
                qqHelper.ConvertToLimitFormat(_ContractorsToolsEquipmentScheduled)
            End If
        End Sub
        'Protected Friend Sub Set_AdditionalInsuredsBackup_Variable(ByVal addlInsBkp As List(Of QuickQuoteAdditionalInsured))
        '    _AdditionalInsuredsBackup = addlInsBkp
        'End Sub
        'Protected Friend Sub Set_HasInclusionOfSoleProprietorsPartnersOfficersAndOthers_Variable(ByVal hasIt As Boolean)
        '    _HasInclusionOfSoleProprietorsPartnersOfficersAndOthers = hasIt
        'End Sub
        'Protected Friend Sub Set_HasWaiverOfSubrogation_Variable(ByVal hasIt As Boolean)
        '    _HasWaiverOfSubrogation = hasIt
        'End Sub
        'Protected Friend Sub Set_HasExclusionOfAmishWorkers_Variable(ByVal hasIt As Boolean)
        '    _HasExclusionOfAmishWorkers = hasIt
        'End Sub
        'Protected Friend Sub Set_HasExclusionOfSoleProprietorsPartnersOfficersAndOthers_Variable(ByVal hasIt As Boolean)
        '    _HasExclusionOfSoleProprietorsPartnersOfficersAndOthers = hasIt
        'End Sub
        'Protected Friend Sub Set_WaiverOfSubrogationNumberOfWaivers_Variable(ByVal num As Integer)
        '    _WaiverOfSubrogationNumberOfWaivers = num
        'End Sub
        'Protected Friend Function Get_WaiverOfSubrogationNumberOfWaivers_Variable() As Integer
        '    Return _WaiverOfSubrogationNumberOfWaivers
        'End Function
        'Protected Friend Function Get_NeedsToUpdateWaiverOfSubrogationPremiumId_Variable() As Boolean
        '    Return _NeedsToUpdateWaiverOfSubrogationPremiumId
        'End Function
        'Protected Friend Function Get_WaiverOfSubrogationPremiumId_Variable() As String
        '    Return _WaiverOfSubrogationPremiumId
        'End Function
        'Protected Friend Sub Set_WaiverOfSubrogationPremiumId_Variable(ByVal premId As String) '7/19/2018 note: same as Property but without logic to set all in list
        '    _WaiverOfSubrogationPremiumId = premId
        '    _WaiverOfSubrogationPremium = ""
        '    If IsNumeric(_WaiverOfSubrogationPremiumId) = True Then
        '        Select Case _WaiverOfSubrogationPremiumId
        '            Case "0"
        '                _WaiverOfSubrogationPremium = "Not Assigned"
        '            Case "1"
        '                _WaiverOfSubrogationPremium = "0"
        '            Case "2"
        '                _WaiverOfSubrogationPremium = "25"
        '            Case "3"
        '                _WaiverOfSubrogationPremium = "50"
        '            Case "4"
        '                _WaiverOfSubrogationPremium = "75"
        '            Case "5"
        '                _WaiverOfSubrogationPremium = "100"
        '            Case "6"
        '                _WaiverOfSubrogationPremium = "150"
        '            Case "7"
        '                _WaiverOfSubrogationPremium = "200"
        '            Case "8"
        '                _WaiverOfSubrogationPremium = "250"
        '            Case "9"
        '                _WaiverOfSubrogationPremium = "300"
        '            Case "10"
        '                _WaiverOfSubrogationPremium = "400"
        '            Case "11"
        '                _WaiverOfSubrogationPremium = "500"
        '        End Select
        '    End If
        'End Sub
        Protected Friend Sub Set_ContractorsEquipmentScheduledItemsBackup_variable(ByVal contEqipSchItems As List(Of QuickQuoteContractorsEquipmentScheduledItem))
            _ContractorsEquipmentScheduledItemsBackup = contEqipSchItems
        End Sub
        'Protected Friend Sub Set_ExclusionOfAmishWorkerRecordsBackup_Variable(ByVal exs As List(Of QuickQuoteExclusionOfAmishWorkerRecord))
        '    _ExclusionOfAmishWorkerRecordsBackup = exs
        'End Sub
        'Protected Friend Sub Set_ExclusionOfSoleProprietorRecordsBackup_Variable(ByVal exs As List(Of QuickQuoteExclusionOfSoleProprietorRecord))
        '    _ExclusionOfSoleProprietorRecordsBackup = exs
        'End Sub
        'Protected Friend Sub Set_InclusionOfSoleProprietorRecordsBackup_Variable(ByVal incs As List(Of QuickQuoteInclusionOfSoleProprietorRecord))
        '    _InclusionOfSoleProprietorRecordsBackup = incs
        'End Sub
        'Protected Friend Sub Set_WaiverOfSubrogationRecordsBackup_Variable(ByVal ws As List(Of QuickQuoteWaiverOfSubrogationRecord))
        '    _WaiverOfSubrogationRecordsBackup = ws
        'End Sub
        'Protected Friend Sub Set_GarageKeepersCollisionManualLimitAmount_Variable(ByVal lmt As String) 'will stay w/ PolicyLevel and RiskLevel stuff
        '    _GarageKeepersCollisionManualLimitAmount = lmt
        'End Sub
        'Protected Friend Sub Set_GarageKeepersOtherThanCollisionManualLimitAmount_Variable(ByVal lmt As String) 'will stay w/ PolicyLevel and RiskLevel stuff
        '    _GarageKeepersOtherThanCollisionManualLimitAmount = lmt
        'End Sub
        'Protected Friend Function Get_GarageKeepersCollisionManualLimitAmount_Variable() As String 'will stay w/ PolicyLevel and RiskLevel stuff
        '    Return _GarageKeepersCollisionManualLimitAmount
        'End Function
        'Protected Friend Function Get_GarageKeepersOtherThanCollisionManualLimitAmount_Variable() As String 'will stay w/ PolicyLevel and RiskLevel stuff
        '    Return _GarageKeepersOtherThanCollisionManualLimitAmount
        'End Function

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
        Private _FarmCustomFeedingCattleLimitId As String
        Private _FarmCustomFeedingCattleDescription As String
        Private _FarmCustomFeedingCattleQuotedPremium As String
        Private _FarmCustomFeedingEquineLimitId As String
        Private _FarmCustomFeedingEquineDescription As String
        Private _FarmCustomFeedingEquineQuotedPremium As String
        Private _FarmCustomFeedingPoultryLimitId As String
        Private _FarmCustomFeedingPoultryDescription As String
        Private _FarmCustomFeedingPoultryQuotedPremium As String
        Private _FarmCustomFeedingSwineLimitId As String
        Private _FarmCustomFeedingSwineDescription As String
        Private _FarmCustomFeedingSwineQuotedPremium As String

        ' IDisposable
        'Protected Overridable Sub Dispose(disposing As Boolean)
        'updated w/ QuickQuoteBaseObject inheritance
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).


                    'PolicyLevel
                    qqHelper.DisposeString(_CPP_CRM_ProgramTypeId)
                    qqHelper.DisposeString(_CPP_GAR_ProgramTypeId)
                    qqHelper.DisposeString(_RiskGradeLookupId_Original)
                    qqHelper.DisposeString(_CPP_CGL_RiskGrade)
                    qqHelper.DisposeString(_CPP_CGL_RiskGradeLookupId)
                    qqHelper.DisposeString(_CPP_CPR_RiskGrade)
                    qqHelper.DisposeString(_CPP_CPR_RiskGradeLookupId)
                    qqHelper.DisposeString(_ErrorRiskGradeLookupId)
                    qqHelper.DisposeString(_ReplacementRiskGradeLookupId)
                    qqHelper.DisposeString(_CPP_CGL_ErrorRiskGradeLookupId)
                    qqHelper.DisposeString(_CPP_CGL_ReplacementRiskGradeLookupId)
                    qqHelper.DisposeString(_CPP_CPR_ErrorRiskGradeLookupId)
                    qqHelper.DisposeString(_CPP_CPR_ReplacementRiskGradeLookupId)
                    qqHelper.DisposeString(_CPP_CIM_RiskGrade)
                    qqHelper.DisposeString(_CPP_CIM_RiskGradeLookupId)
                    qqHelper.DisposeString(_CPP_CIM_ErrorRiskGradeLookupId)
                    qqHelper.DisposeString(_CPP_CIM_ReplacementRiskGradeLookupId)
                    qqHelper.DisposeString(_CPP_CRM_RiskGrade)
                    qqHelper.DisposeString(_CPP_CRM_RiskGradeLookupId)
                    qqHelper.DisposeString(_CPP_CRM_ErrorRiskGradeLookupId)
                    qqHelper.DisposeString(_CPP_CRM_ReplacementRiskGradeLookupId)
                    qqHelper.DisposeString(_OccurrenceLiabilityLimit)
                    qqHelper.DisposeString(_OccurrenceLiabilityLimitId)
                    qqHelper.DisposeString(_OccurrencyLiabilityQuotedPremium)
                    qqHelper.DisposeString(_TenantsFireLiability)
                    qqHelper.DisposeString(_TenantsFireLiabilityId)
                    qqHelper.DisposeString(_TenantsFireLiabilityQuotedPremium)
                    qqHelper.DisposeString(_PropertyDamageLiabilityDeductible)
                    qqHelper.DisposeString(_PropertyDamageLiabilityDeductibleId)
                    qqHelper.DisposeString(_BlanketRatingQuotedPremium)
                    _HasEnhancementEndorsement = Nothing
                    qqHelper.DisposeString(_EnhancementEndorsementQuotedPremium)
                    _Has_PackageGL_EnhancementEndorsement = Nothing
                    qqHelper.DisposeString(_PackageGL_EnhancementEndorsementQuotedPremium)
                    _Has_PackageGL_PlusEnhancementEndorsement = Nothing
                    qqHelper.DisposeString(_PackageGL_PlusEnhancementEndorsementQuotedPremium)
                    _Has_PackageCPR_EnhancementEndorsement = Nothing
                    qqHelper.DisposeString(_PackageCPR_EnhancementEndorsementQuotedPremium)
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
                    qqHelper.DisposeString(_EmployeeBenefitsLiabilityText)
                    qqHelper.DisposeString(_EmployeeBenefitsLiabilityOccurrenceLimit)
                    qqHelper.DisposeString(_EmployeeBenefitsLiabilityOccurrenceLimitId)
                    qqHelper.DisposeString(_EmployeeBenefitsLiabilityQuotedPremium)
                    qqHelper.DisposeString(_EmployeeBenefitsLiabilityRetroactiveDate)
                    qqHelper.DisposeString(_EmployeeBenefitsLiabilityAggregateLimit)
                    qqHelper.DisposeString(_EmployeeBenefitsLiabilityDeductible)
                    qqHelper.DisposeString(_ContractorsEquipmentInstallationLimit)
                    qqHelper.DisposeString(_ContractorsEquipmentInstallationLimitId)
                    qqHelper.DisposeString(_ContractorsEquipmentInstallationLimitQuotedPremium)
                    qqHelper.DisposeString(_ContractorsToolsEquipmentBlanket)
                    qqHelper.DisposeString(_ContractorsToolsEquipmentBlanketSubLimitId)
                    qqHelper.DisposeString(_ContractorsToolsEquipmentBlanketQuotedPremium)
                    qqHelper.DisposeString(_ContractorsToolsEquipmentScheduled)
                    qqHelper.DisposeString(_ContractorsToolsEquipmentScheduledQuotedPremium)
                    qqHelper.DisposeString(_ContractorsToolsEquipmentRented)
                    qqHelper.DisposeString(_ContractorsToolsEquipmentRentedQuotedPremium)
                    If _ContractorsEquipmentScheduledItems IsNot Nothing Then
                        If _ContractorsEquipmentScheduledItems.Count > 0 Then
                            For Each si As QuickQuoteContractorsEquipmentScheduledItem In _ContractorsEquipmentScheduledItems
                                si.Dispose()
                                si = Nothing
                            Next
                            _ContractorsEquipmentScheduledItems.Clear()
                        End If
                        _ContractorsEquipmentScheduledItems = Nothing
                    End If
                    If _ContractorsEquipmentScheduledItemsBackup IsNot Nothing Then
                        If _ContractorsEquipmentScheduledItemsBackup.Count > 0 Then
                            For Each si As QuickQuoteContractorsEquipmentScheduledItem In _ContractorsEquipmentScheduledItemsBackup
                                si.Dispose()
                                si = Nothing
                            Next
                            _ContractorsEquipmentScheduledItemsBackup.Clear()
                        End If
                        _ContractorsEquipmentScheduledItemsBackup = Nothing
                    End If
                    qqHelper.DisposeString(_ContractorsEmployeeTools)
                    qqHelper.DisposeString(_ContractorsEmployeeToolsQuotedPremium)
                    qqHelper.DisposeString(_CrimeEmpDisEmployeeText)
                    qqHelper.DisposeString(_CrimeEmpDisLocationText)
                    qqHelper.DisposeString(_CrimeEmpDisLimit)
                    qqHelper.DisposeString(_CrimeEmpDisLimitId)
                    qqHelper.DisposeString(_CrimeEmpDisQuotedPremium)
                    qqHelper.DisposeString(_CrimeForgeryLimit)
                    qqHelper.DisposeString(_CrimeForgeryLimitId)
                    qqHelper.DisposeString(_CrimeForgeryQuotedPremium)
                    _HasEarthquake = Nothing
                    qqHelper.DisposeString(_EarthquakeQuotedPremium)
                    _HasHiredAuto = Nothing
                    qqHelper.DisposeString(_HiredAutoQuotedPremium)
                    _HasNonOwnedAuto = Nothing
                    _NonOwnedAutoWithDelivery = Nothing
                    qqHelper.DisposeString(_NonOwnedAutoQuotedPremium)
                    qqHelper.DisposeString(_PropertyDeductibleId)
                    qqHelper.DisposeString(_EmployersLiability)
                    qqHelper.DisposeString(_EmployersLiabilityId)
                    qqHelper.DisposeString(_EmployersLiabilityQuotedPremium)
                    qqHelper.DisposeString(_GeneralAggregateLimit)
                    qqHelper.DisposeString(_GeneralAggregateLimitId)
                    qqHelper.DisposeString(_GeneralAggregateQuotedPremium)
                    qqHelper.DisposeString(_ProductsCompletedOperationsAggregateLimit)
                    qqHelper.DisposeString(_ProductsCompletedOperationsAggregateLimitId)
                    qqHelper.DisposeString(_ProductsCompletedOperationsAggregateQuotedPremium)
                    qqHelper.DisposeString(_PersonalAndAdvertisingInjuryLimit)
                    qqHelper.DisposeString(_PersonalAndAdvertisingInjuryLimitId)
                    qqHelper.DisposeString(_PersonalAndAdvertisingInjuryQuotedPremium)
                    qqHelper.DisposeString(_DamageToPremisesRentedLimit)
                    qqHelper.DisposeString(_DamageToPremisesRentedLimitId)
                    qqHelper.DisposeString(_DamageToPremisesRentedQuotedPremium)
                    qqHelper.DisposeString(_MedicalExpensesLimit)
                    qqHelper.DisposeString(_MedicalExpensesLimitId)
                    qqHelper.DisposeString(_MedicalExpensesQuotedPremium)
                    '_HasExclusionOfAmishWorkers = Nothing
                    '_HasExclusionOfSoleProprietorsPartnersOfficersAndOthers = Nothing
                    '_HasInclusionOfSoleProprietorsPartnersOfficersAndOthers = Nothing
                    '_HasWaiverOfSubrogation = Nothing
                    '_WaiverOfSubrogationNumberOfWaivers = Nothing
                    'qqHelper.DisposeString(_WaiverOfSubrogationPremium)
                    'qqHelper.DisposeString(_WaiverOfSubrogationPremiumId)
                    '_NeedsToUpdateWaiverOfSubrogationPremiumId = Nothing
                    'If _ExclusionOfAmishWorkerRecords IsNot Nothing Then
                    '    If _ExclusionOfAmishWorkerRecords.Count > 0 Then
                    '        For Each aw As QuickQuoteExclusionOfAmishWorkerRecord In _ExclusionOfAmishWorkerRecords
                    '            aw.Dispose()
                    '            aw = Nothing
                    '        Next
                    '        _ExclusionOfAmishWorkerRecords.Clear()
                    '    End If
                    '    _ExclusionOfAmishWorkerRecords = Nothing
                    'End If
                    'If _ExclusionOfSoleProprietorRecords IsNot Nothing Then
                    '    If _ExclusionOfSoleProprietorRecords.Count > 0 Then
                    '        For Each sp As QuickQuoteExclusionOfSoleProprietorRecord In _ExclusionOfSoleProprietorRecords
                    '            sp.Dispose()
                    '            sp = Nothing
                    '        Next
                    '        _ExclusionOfSoleProprietorRecords.Clear()
                    '    End If
                    '    _ExclusionOfSoleProprietorRecords = Nothing
                    'End If
                    'If _InclusionOfSoleProprietorRecords IsNot Nothing Then
                    '    If _InclusionOfSoleProprietorRecords.Count > 0 Then
                    '        For Each sp As QuickQuoteInclusionOfSoleProprietorRecord In _InclusionOfSoleProprietorRecords
                    '            sp.Dispose()
                    '            sp = Nothing
                    '        Next
                    '        _InclusionOfSoleProprietorRecords.Clear()
                    '    End If
                    '    _InclusionOfSoleProprietorRecords = Nothing
                    'End If
                    'If _WaiverOfSubrogationRecords IsNot Nothing Then
                    '    If _WaiverOfSubrogationRecords.Count > 0 Then
                    '        For Each w As QuickQuoteWaiverOfSubrogationRecord In _WaiverOfSubrogationRecords
                    '            w.Dispose()
                    '            w = Nothing
                    '        Next
                    '        _WaiverOfSubrogationRecords.Clear()
                    '    End If
                    '    _WaiverOfSubrogationRecords = Nothing
                    'End If
                    'If _ExclusionOfAmishWorkerRecordsBackup IsNot Nothing Then
                    '    If _ExclusionOfAmishWorkerRecordsBackup.Count > 0 Then
                    '        For Each aw As QuickQuoteExclusionOfAmishWorkerRecord In _ExclusionOfAmishWorkerRecordsBackup
                    '            aw.Dispose()
                    '            aw = Nothing
                    '        Next
                    '        _ExclusionOfAmishWorkerRecordsBackup.Clear()
                    '    End If
                    '    _ExclusionOfAmishWorkerRecordsBackup = Nothing
                    'End If
                    'If _ExclusionOfSoleProprietorRecordsBackup IsNot Nothing Then
                    '    If _ExclusionOfSoleProprietorRecordsBackup.Count > 0 Then
                    '        For Each sp As QuickQuoteExclusionOfSoleProprietorRecord In _ExclusionOfSoleProprietorRecordsBackup
                    '            sp.Dispose()
                    '            sp = Nothing
                    '        Next
                    '        _ExclusionOfSoleProprietorRecordsBackup.Clear()
                    '    End If
                    '    _ExclusionOfSoleProprietorRecordsBackup = Nothing
                    'End If
                    'If _InclusionOfSoleProprietorRecordsBackup IsNot Nothing Then
                    '    If _InclusionOfSoleProprietorRecordsBackup.Count > 0 Then
                    '        For Each sp As QuickQuoteInclusionOfSoleProprietorRecord In _InclusionOfSoleProprietorRecordsBackup
                    '            sp.Dispose()
                    '            sp = Nothing
                    '        Next
                    '        _InclusionOfSoleProprietorRecordsBackup.Clear()
                    '    End If
                    '    _InclusionOfSoleProprietorRecordsBackup = Nothing
                    'End If
                    'If _WaiverOfSubrogationRecordsBackup IsNot Nothing Then
                    '    If _WaiverOfSubrogationRecordsBackup.Count > 0 Then
                    '        For Each w As QuickQuoteWaiverOfSubrogationRecord In _WaiverOfSubrogationRecordsBackup
                    '            w.Dispose()
                    '            w = Nothing
                    '        Next
                    '        _WaiverOfSubrogationRecordsBackup.Clear()
                    '    End If
                    '    _WaiverOfSubrogationRecordsBackup = Nothing
                    'End If
                    '_HasBarbersProfessionalLiability = False
                    'qqHelper.DisposeString(_BarbersProfessionalLiabiltyQuotedPremium)
                    'qqHelper.DisposeString(_BarbersProfessionalLiabilityFullTimeEmpNum)
                    'qqHelper.DisposeString(_BarbersProfessionalLiabilityPartTimeEmpNum)
                    'qqHelper.DisposeString(_BarbersProfessionalLiabilityDescription)
                    '_HasBeauticiansProfessionalLiability = False
                    'qqHelper.DisposeString(_BeauticiansProfessionalLiabilityQuotedPremium)
                    'qqHelper.DisposeString(_BeauticiansProfessionalLiabilityFullTimeEmpNum)
                    'qqHelper.DisposeString(_BeauticiansProfessionalLiabilityPartTimeEmpNum)
                    'qqHelper.DisposeString(_BeauticiansProfessionalLiabilityDescription)
                    '_HasFuneralDirectorsProfessionalLiability = False
                    'qqHelper.DisposeString(_FuneralDirectorsProfessionalLiabilityQuotedPremium)
                    'qqHelper.DisposeString(_FuneralDirectorsProfessionalLiabilityEmpNum)
                    '_HasPrintersProfessionalLiability = False
                    'qqHelper.DisposeString(_PrintersProfessionalLiabilityQuotedPremium)
                    'qqHelper.DisposeString(_PrintersProfessionalLiabilityLocNum)
                    '_HasSelfStorageFacility = False
                    'qqHelper.DisposeString(_SelfStorageFacilityQuotedPremium)
                    'qqHelper.DisposeString(_SelfStorageFacilityLimit)
                    '_HasVeterinariansProfessionalLiability = False
                    'qqHelper.DisposeString(_VeterinariansProfessionalLiabilityEmpNum)
                    'qqHelper.DisposeString(_VeterinariansProfessionalLiabilityQuotedPremium)
                    '_HasPharmacistProfessionalLiability = False
                    'qqHelper.DisposeString(_PharmacistAnnualGrossSales)
                    'qqHelper.DisposeString(_PharmacistQuotedPremium)
                    '_HasOpticalAndHearingAidProfessionalLiability = False
                    'qqHelper.DisposeString(_OpticalAndHearingAidProfessionalLiabilityEmpNum)
                    'qqHelper.DisposeString(_OpticalAndHearingAidProfessionalLiabilityQuotedPremium)
                    '_HasMotelCoverage = False
                    'qqHelper.DisposeString(_MotelCoveragePerGuestLimitId)
                    'qqHelper.DisposeString(_MotelCoveragePerGuestLimit)
                    'qqHelper.DisposeString(_MotelCoveragePerGuestQuotedPremium)
                    'qqHelper.DisposeString(_MotelCoverageSafeDepositLimitId)
                    'qqHelper.DisposeString(_MotelCoverageSafeDepositDeductibleId)
                    'qqHelper.DisposeString(_MotelCoverageSafeDepositLimit)
                    'qqHelper.DisposeString(_MotelCoverageSafeDepositDeductible)
                    'qqHelper.DisposeString(_MotelCoverageQuotedPremium)
                    'qqHelper.DisposeString(_MotelCoverageSafeDepositQuotedPremium)
                    '_HasPhotographyCoverage = False
                    '_HasPhotographyCoverageScheduledCoverages = False
                    'If _PhotographyScheduledCoverages IsNot Nothing Then
                    '    If _PhotographyScheduledCoverages.Count > 0 Then
                    '        For Each qqc As QuickQuoteCoverage In _PhotographyScheduledCoverages
                    '            qqc.Dispose()
                    '            qqc = Nothing
                    '        Next
                    '        _PhotographyScheduledCoverages.Clear()
                    '    End If
                    '    _PhotographyScheduledCoverages = Nothing
                    'End If
                    '_HasPhotographyMakeupAndHair = False
                    'qqHelper.DisposeString(_PhotographyMakeupAndHairQuotedPremium)
                    'qqHelper.DisposeString(_PhotographyCoverageQuotedPremium)
                    '_HasLiquorLiability = False
                    'qqHelper.DisposeString(_LiquorLiabilityClassCodeTypeId) '12 = 58161 - Restaurant Includes Package Sales, 13 = 59211 - Package Sales for Consumption Off Premises
                    'qqHelper.DisposeString(_LiquorLiabilityAnnualGrossPackageSalesReceipts)
                    'qqHelper.DisposeString(_LiquorLiabilityAnnualGrossAlcoholSalesReceipts)
                    '_HasResidentialCleaning = False
                    'qqHelper.DisposeString(_ResidentialCleaningQuotedPremium)
                    'qqHelper.DisposeString(_LiquorLiabilityOccurrenceLimit)
                    'qqHelper.DisposeString(_LiquorLiabilityOccurrenceLimitId)
                    'qqHelper.DisposeString(_LiquorLiabilityClassification)
                    'qqHelper.DisposeString(_LiquorLiabilityClassificationId)
                    'qqHelper.DisposeString(_LiquorSales)
                    'qqHelper.DisposeString(_LiquorLiabilityQuotedPremium)
                    'qqHelper.DisposeString(_ProfessionalLiabilityCemetaryNumberOfBurials)
                    'qqHelper.DisposeString(_ProfessionalLiabilityCemetaryQuotedPremium)
                    'qqHelper.DisposeString(_ProfessionalLiabilityFuneralDirectorsNumberOfBodies)
                    'qqHelper.DisposeString(_ProfessionalLiabilityPastoralNumberOfClergy)
                    'qqHelper.DisposeString(_ProfessionalLiabilityPastoralQuotedPremium)
                    qqHelper.DisposeString(_IRPM_ManagementCooperation)
                    qqHelper.DisposeString(_IRPM_ManagementCooperationDesc)
                    qqHelper.DisposeString(_IRPM_Location)
                    qqHelper.DisposeString(_IRPM_LocationDesc)
                    qqHelper.DisposeString(_IRPM_BuildingFeatures)
                    qqHelper.DisposeString(_IRPM_BuildingFeaturesDesc)
                    qqHelper.DisposeString(_IRPM_Premises)
                    qqHelper.DisposeString(_IRPM_PremisesDesc)
                    qqHelper.DisposeString(_IRPM_Employees)
                    qqHelper.DisposeString(_IRPM_EmployeesDesc)
                    qqHelper.DisposeString(_IRPM_Protection)
                    qqHelper.DisposeString(_IRPM_ProtectionDesc)
                    qqHelper.DisposeString(_IRPM_CatostrophicHazards)
                    qqHelper.DisposeString(_IRPM_CatostrophicHazardsDesc)
                    qqHelper.DisposeString(_IRPM_ManagementExperience)
                    qqHelper.DisposeString(_IRPM_ManagementExperienceDesc)
                    qqHelper.DisposeString(_IRPM_Equipment)
                    qqHelper.DisposeString(_IRPM_EquipmentDesc)
                    qqHelper.DisposeString(_IRPM_MedicalFacilities)
                    qqHelper.DisposeString(_IRPM_MedicalFacilitiesDesc)
                    qqHelper.DisposeString(_IRPM_ClassificationPeculiarities)
                    qqHelper.DisposeString(_IRPM_ClassificationPeculiaritiesDesc)
                    qqHelper.DisposeString(_IRPM_GL_ManagementCooperation)
                    qqHelper.DisposeString(_IRPM_GL_ManagementCooperationDesc)
                    qqHelper.DisposeString(_IRPM_GL_Location)
                    qqHelper.DisposeString(_IRPM_GL_LocationDesc)
                    qqHelper.DisposeString(_IRPM_GL_Premises)
                    qqHelper.DisposeString(_IRPM_GL_PremisesDesc)
                    qqHelper.DisposeString(_IRPM_GL_Equipment)
                    qqHelper.DisposeString(_IRPM_GL_EquipmentDesc)
                    qqHelper.DisposeString(_IRPM_GL_Employees)
                    qqHelper.DisposeString(_IRPM_GL_EmployeesDesc)
                    qqHelper.DisposeString(_IRPM_GL_ClassificationPeculiarities)
                    qqHelper.DisposeString(_IRPM_GL_ClassificationPeculiaritiesDesc)
                    qqHelper.DisposeString(_IRPM_CAP_Management)
                    qqHelper.DisposeString(_IRPM_CAP_ManagementDesc)
                    qqHelper.DisposeString(_IRPM_CAP_Employees)
                    qqHelper.DisposeString(_IRPM_CAP_EmployeesDesc)
                    qqHelper.DisposeString(_IRPM_CAP_Equipment)
                    qqHelper.DisposeString(_IRPM_CAP_EquipmentDesc)
                    qqHelper.DisposeString(_IRPM_CAP_SafetyOrganization)
                    qqHelper.DisposeString(_IRPM_CAP_SafetyOrganizationDesc)
                    qqHelper.DisposeString(_IRPM_CAP_Management_Phys_Damage)
                    qqHelper.DisposeString(_IRPM_CAP_ManagementDesc_Phys_Damage)
                    qqHelper.DisposeString(_IRPM_CAP_Employees_Phys_Damage)
                    qqHelper.DisposeString(_IRPM_CAP_EmployeesDesc_Phys_Damage)
                    qqHelper.DisposeString(_IRPM_CAP_Equipment_Phys_Damage)
                    qqHelper.DisposeString(_IRPM_CAP_EquipmentDesc_Phys_Damage)
                    qqHelper.DisposeString(_IRPM_CAP_SafetyOrganization_Phys_Damage)
                    qqHelper.DisposeString(_IRPM_CAP_SafetyOrganizationDesc_Phys_Damage)
                    qqHelper.DisposeString(_IRPM_CPR_Management)
                    qqHelper.DisposeString(_IRPM_CPR_ManagementDesc)
                    qqHelper.DisposeString(_IRPM_CPR_PremisesAndEquipment)
                    qqHelper.DisposeString(_IRPM_CPR_PremisesAndEquipmentDesc)
                    qqHelper.DisposeString(_IRPM_FAR_CareConditionOfEquipPremises)
                    qqHelper.DisposeString(_IRPM_FAR_CareConditionOfEquipPremisesDesc)
                    qqHelper.DisposeString(_IRPM_FAR_Cooperation)
                    qqHelper.DisposeString(_IRPM_FAR_CooperationDesc)
                    qqHelper.DisposeString(_IRPM_FAR_DamageSusceptibility)
                    qqHelper.DisposeString(_IRPM_FAR_DamageSusceptibilityDesc)
                    qqHelper.DisposeString(_IRPM_FAR_DispersionOrConcentration)
                    qqHelper.DisposeString(_IRPM_FAR_DispersionOrConcentrationDesc)
                    qqHelper.DisposeString(_IRPM_FAR_SuperiorOrInferiorStructureFeatures)
                    qqHelper.DisposeString(_IRPM_FAR_SuperiorOrInferiorStructureFeaturesDesc)
                    qqHelper.DisposeString(_IRPM_FAR_UseOfRiceHullsOrFlameRetardantBedding)
                    qqHelper.DisposeString(_IRPM_FAR_UseOfRiceHullsOrFlameRetardantBeddingDesc)
                    qqHelper.DisposeString(_IRPM_FAR_Location)
                    qqHelper.DisposeString(_IRPM_FAR_LocationDesc)
                    qqHelper.DisposeString(_IRPM_FAR_MiscProtectFeaturesOrHazards)
                    qqHelper.DisposeString(_IRPM_FAR_MiscProtectFeaturesOrHazardsDesc)
                    qqHelper.DisposeString(_IRPM_FAR_RoofCondition)
                    qqHelper.DisposeString(_IRPM_FAR_RoofConditionDesc)
                    qqHelper.DisposeString(_IRPM_FAR_StoragePracticesAndHazardousOperations)
                    qqHelper.DisposeString(_IRPM_FAR_StoragePracticesAndHazardousOperationsDesc)
                    qqHelper.DisposeString(_IRPM_FAR_PastLosses)
                    qqHelper.DisposeString(_IRPM_FAR_PastLossesDesc)
                    qqHelper.DisposeString(_IRPM_FAR_SupportingBusiness)
                    qqHelper.DisposeString(_IRPM_FAR_SupportingBusinessDesc)
                    qqHelper.DisposeString(_IRPM_FAR_RegularOnsiteInspections)
                    qqHelper.DisposeString(_IRPM_FAR_RegularOnsiteInspectionsDesc)
                    qqHelper.DisposeString(_GL_PremisesAndProducts_Deductible)
                    qqHelper.DisposeString(_GL_PremisesAndProducts_DeductibleId)
                    qqHelper.DisposeString(_GL_PremisesAndProducts_Description)
                    qqHelper.DisposeString(_GL_PremisesAndProducts_DeductibleCategoryType)
                    qqHelper.DisposeString(_GL_PremisesAndProducts_DeductibleCategoryTypeId)
                    qqHelper.DisposeString(_GL_PremisesAndProducts_DeductiblePerType)
                    qqHelper.DisposeString(_GL_PremisesAndProducts_DeductiblePerTypeId)
                    _Has_GL_PremisesAndProducts = Nothing
                    qqHelper.DisposeString(_GL_PremisesTotalQuotedPremium)
                    qqHelper.DisposeString(_GL_ProductsTotalQuotedPremium)
                    qqHelper.DisposeString(_GL_PremisesPolicyLevelQuotedPremium)
                    qqHelper.DisposeString(_GL_ProductsPolicyLevelQuotedPremium)
                    qqHelper.DisposeString(_GL_PremisesMinimumQuotedPremium)
                    qqHelper.DisposeString(_GL_PremisesMinimumPremiumAdjustment)
                    qqHelper.DisposeString(_GL_ProductsMinimumQuotedPremium)
                    qqHelper.DisposeString(_GL_ProductsMinimumPremiumAdjustment)
                    _HasFarmPollutionLiability = Nothing
                    qqHelper.DisposeString(_FarmPollutionLiabilityQuotedPremium)
                    _HasHiredBorrowedNonOwned = Nothing
                    _HasNonOwnershipLiability = Nothing
                    qqHelper.DisposeString(_NonOwnershipLiabilityNumberOfEmployees)
                    qqHelper.DisposeString(_NonOwnership_ENO_RatingTypeId)
                    qqHelper.DisposeString(_NonOwnership_ENO_RatingType)
                    qqHelper.DisposeString(_NonOwnershipLiabilityQuotedPremium)
                    _HasHiredBorrowedLiability = Nothing
                    qqHelper.DisposeString(_HiredBorrowedLiabilityQuotedPremium)
                    _HasHiredCarPhysicalDamage = Nothing
                    qqHelper.DisposeString(_HiredBorrowedLossOfUseQuotedPremium)
                    qqHelper.DisposeString(_ComprehensiveDeductible)
                    qqHelper.DisposeString(_ComprehensiveDeductibleId)
                    qqHelper.DisposeString(_ComprehensiveQuotedPremium)
                    qqHelper.DisposeString(_CollisionDeductible)
                    qqHelper.DisposeString(_CollisionDeductibleId)
                    qqHelper.DisposeString(_CollisionQuotedPremium)
                    qqHelper.DisposeString(_Liability_UM_UIM_Limit)
                    qqHelper.DisposeString(_Liability_UM_UIM_LimitId)
                    qqHelper.DisposeString(_Liability_UM_UIM_QuotedPremium)
                    qqHelper.DisposeString(_MedicalPaymentsLimit)
                    qqHelper.DisposeString(_MedicalPaymentsLimitId)
                    qqHelper.DisposeString(_MedicalPaymentsQuotedPremium)
                    _QuoteOrIssueBound = Nothing
                    qqHelper.DisposeString(_IssueBoundEffectiveDate)
                    If _LiabilityAutoSymbolObject IsNot Nothing Then
                        _LiabilityAutoSymbolObject.Dispose()
                        _LiabilityAutoSymbolObject = Nothing
                    End If
                    If _MedicalPaymentsAutoSymbolObject IsNot Nothing Then
                        _MedicalPaymentsAutoSymbolObject.Dispose()
                        _MedicalPaymentsAutoSymbolObject = Nothing
                    End If
                    If _UninsuredMotoristAutoSymbolObject IsNot Nothing Then
                        _UninsuredMotoristAutoSymbolObject.Dispose()
                        _UninsuredMotoristAutoSymbolObject = Nothing
                    End If
                    If _UnderinsuredMotoristAutoSymbolObject IsNot Nothing Then
                        _UnderinsuredMotoristAutoSymbolObject.Dispose()
                        _UnderinsuredMotoristAutoSymbolObject = Nothing
                    End If
                    If _ComprehensiveCoverageAutoSymbolObject IsNot Nothing Then
                        _ComprehensiveCoverageAutoSymbolObject.Dispose()
                        _ComprehensiveCoverageAutoSymbolObject = Nothing
                    End If
                    If _CollisionCoverageAutoSymbolObject IsNot Nothing Then
                        _CollisionCoverageAutoSymbolObject.Dispose()
                        _CollisionCoverageAutoSymbolObject = Nothing
                    End If
                    If _NonOwnershipAutoSymbolObject IsNot Nothing Then
                        _NonOwnershipAutoSymbolObject.Dispose()
                        _NonOwnershipAutoSymbolObject = Nothing
                    End If
                    If _HiredBorrowedAutoSymbolObject IsNot Nothing Then
                        _HiredBorrowedAutoSymbolObject.Dispose()
                        _HiredBorrowedAutoSymbolObject = Nothing
                    End If
                    If _TowingAndLaborAutoSymbolObject IsNot Nothing Then
                        _TowingAndLaborAutoSymbolObject.Dispose()
                        _TowingAndLaborAutoSymbolObject = Nothing
                    End If
                    _UseDeveloperAutoSymbols = Nothing
                    _CAP_Liability_WouldHaveSymbol8 = Nothing
                    _CAP_Liability_WouldHaveSymbol9 = Nothing
                    _CAP_Comprehensive_WouldHaveSymbol8 = Nothing
                    _CAP_Collision_WouldHaveSymbol8 = Nothing
                    _HasBlanketBuilding = Nothing
                    _HasBlanketContents = Nothing
                    _HasBlanketBuildingAndContents = Nothing
                    _HasBlanketBusinessIncome = Nothing
                    qqHelper.DisposeString(_BlanketBuildingQuotedPremium)
                    qqHelper.DisposeString(_BlanketContentsQuotedPremium)
                    qqHelper.DisposeString(_BlanketBuildingAndContentsQuotedPremium)
                    qqHelper.DisposeString(_BlanketBusinessIncomeQuotedPremium)
                    qqHelper.DisposeString(_BlanketBuildingCauseOfLossTypeId)
                    qqHelper.DisposeString(_BlanketBuildingCauseOfLossType)
                    qqHelper.DisposeString(_BlanketContentsCauseOfLossTypeId)
                    qqHelper.DisposeString(_BlanketContentsCauseOfLossType)
                    qqHelper.DisposeString(_BlanketBuildingAndContentsCauseOfLossTypeId)
                    qqHelper.DisposeString(_BlanketBuildingAndContentsCauseOfLossType)
                    qqHelper.DisposeString(_BlanketBusinessIncomeCauseOfLossTypeId)
                    qqHelper.DisposeString(_BlanketBusinessIncomeCauseOfLossType)
                    qqHelper.DisposeString(_BlanketBuildingLimit)
                    qqHelper.DisposeString(_BlanketBuildingCoinsuranceTypeId)
                    qqHelper.DisposeString(_BlanketBuildingCoinsuranceType)
                    qqHelper.DisposeString(_BlanketBuildingValuationId)
                    qqHelper.DisposeString(_BlanketBuildingValuation)
                    qqHelper.DisposeString(_BlanketContentsLimit)
                    qqHelper.DisposeString(_BlanketContentsCoinsuranceTypeId)
                    qqHelper.DisposeString(_BlanketContentsCoinsuranceType)
                    qqHelper.DisposeString(_BlanketContentsValuationId)
                    qqHelper.DisposeString(_BlanketContentsValuation)
                    qqHelper.DisposeString(_BlanketBuildingAndContentsLimit)
                    qqHelper.DisposeString(_BlanketBuildingAndContentsCoinsuranceTypeId)
                    qqHelper.DisposeString(_BlanketBuildingAndContentsCoinsuranceType)
                    qqHelper.DisposeString(_BlanketBuildingAndContentsValuationId)
                    qqHelper.DisposeString(_BlanketBuildingAndContentsValuation)
                    qqHelper.DisposeString(_BlanketBusinessIncomeLimit)
                    qqHelper.DisposeString(_BlanketBusinessIncomeCoinsuranceTypeId)
                    qqHelper.DisposeString(_BlanketBusinessIncomeCoinsuranceType)
                    qqHelper.DisposeString(_BlanketBusinessIncomeValuationId)
                    qqHelper.DisposeString(_BlanketBusinessIncomeValuation)
                    qqHelper.DisposeString(_CPR_BlanketCoverages_TotalPremium)
                    qqHelper.DisposeString(_BlanketCombinedEarthquake_QuotedPremium)
                    _BlanketBuildingIsAgreedValue = Nothing
                    _BlanketContentsIsAgreedValue = Nothing
                    _BlanketBuildingAndContentsIsAgreedValue = Nothing
                    _BlanketBusinessIncomeIsAgreedValue = Nothing
                    _UseTierOverride = Nothing
                    qqHelper.DisposeString(_TierAdjustmentTypeId)
                    qqHelper.DisposeString(_PersonalLiabilityLimitId)
                    qqHelper.DisposeString(_PersonalLiabilityQuotedPremium)
                    _HasEPLI = Nothing
                    _EPLI_Applied = Nothing
                    qqHelper.DisposeString(_EPLIPremium)
                    qqHelper.DisposeString(_EPLICoverageLimitId)
                    qqHelper.DisposeString(_EPLIDeductibleId)
                    qqHelper.DisposeString(_EPLICoverageTypeId)
                    'qqHelper.DisposeString(_BlanketWaiverOfSubrogation)
                    'qqHelper.DisposeString(_BlanketWaiverOfSubrogationQuotedPremium)
                    _HasCondoDandO = Nothing
                    qqHelper.DisposeString(_CondoDandOAssociatedName)
                    qqHelper.DisposeString(_CondoDandODeductibleId)
                    qqHelper.DisposeString(_CondoDandOPremium)
                    qqHelper.DisposeString(_CondoDandOManualLimit)
                    qqHelper.DisposeString(_Farm_F_and_G_DeductibleLimitId) 'static data
                    qqHelper.DisposeString(_Farm_F_and_G_DeductibleQuotedPremium)
                    _HasFarmEquipmentBreakdown = Nothing
                    qqHelper.DisposeString(_FarmEquipmentBreakdownQuotedPremium)
                    _HasFarmExtender = Nothing
                    qqHelper.DisposeString(_FarmExtenderQuotedPremium)
                    qqHelper.DisposeString(_FarmAllStarLimitId)
                    qqHelper.DisposeString(_FarmAllStarQuotedPremium)
                    _HasFarmEmployersLiability = Nothing
                    qqHelper.DisposeString(_FarmEmployersLiabilityQuotedPremium)
                    qqHelper.DisposeString(_FarmFireLegalLiabilityLimitId)
                    qqHelper.DisposeString(_FarmFireLegalLiabilityQuotedPremium)
                    _HasFarmPersonalAndAdvertisingInjury = Nothing
                    qqHelper.DisposeString(_FarmPersonalAndAdvertisingInjuryQuotedPremium)
                    qqHelper.DisposeString(_FarmContractGrowersCareCustodyControlLimitId)
                    qqHelper.DisposeString(_FarmContractGrowersCareCustodyControlDescription)
                    qqHelper.DisposeString(_FarmContractGrowersCareCustodyControlQuotedPremium)

                    qqHelper.DisposeString(_FarmCustomFeedingCattleLimitId)
                    qqHelper.DisposeString(_FarmCustomFeedingCattleDescription)
                    qqHelper.DisposeString(_FarmCustomFeedingCattleQuotedPremium)

                    qqHelper.DisposeString(_FarmCustomFeedingEquineLimitId)
                    qqHelper.DisposeString(_FarmCustomFeedingEquineDescription)
                    qqHelper.DisposeString(_FarmCustomFeedingEquineQuotedPremium)

                    qqHelper.DisposeString(_FarmCustomFeedingPoultryLimitId)
                    qqHelper.DisposeString(_FarmCustomFeedingPoultryDescription)
                    qqHelper.DisposeString(_FarmCustomFeedingPoultryQuotedPremium)

                    qqHelper.DisposeString(_FarmCustomFeedingSwineLimitId)
                    qqHelper.DisposeString(_FarmCustomFeedingSwineDescription)
                    qqHelper.DisposeString(_FarmCustomFeedingSwineQuotedPremium)

                    _HasFarmExclusionOfProductsCompletedWork = Nothing
                    qqHelper.DisposeString(_FarmExclusionOfProductsCompletedWorkQuotedPremium)
                    If _FarmIncidentalLimits IsNot Nothing Then 'goes w/ FarmIncidentalLimitCoverages
                        If _FarmIncidentalLimits.Count > 0 Then
                            For Each fil As QuickQuoteFarmIncidentalLimit In _FarmIncidentalLimits
                                fil.Dispose()
                                fil = Nothing
                            Next
                            _FarmIncidentalLimits.Clear()
                        End If
                        _FarmIncidentalLimits = Nothing
                    End If
                    _HasBusinessIncomeALS = Nothing
                    qqHelper.DisposeString(_BusinessIncomeALSLimit)
                    qqHelper.DisposeString(_BusinessIncomeALSQuotedPremium)
                    _HasContractorsEnhancement = Nothing
                    qqHelper.DisposeString(_ContractorsEnhancementQuotedPremium)
                    qqHelper.DisposeString(_CPP_CPR_ContractorsEnhancementQuotedPremium)
                    qqHelper.DisposeString(_CPP_CGL_ContractorsEnhancementQuotedPremium)
                    qqHelper.DisposeString(_CPP_CIM_ContractorsEnhancementQuotedPremium)
                    _HasManufacturersEnhancement = Nothing
                    qqHelper.DisposeString(_ManufacturersEnhancementQuotedPremium)
                    qqHelper.DisposeString(_CPP_CPR_ManufacturersEnhancementQuotedPremium)
                    qqHelper.DisposeString(_CPP_CGL_ManufacturersEnhancementQuotedPremium)
                    qqHelper.DisposeString(_FarmMachinerySpecialCoverageG_QuotedPremium)
                    _HasAutoPlusEnhancement = Nothing
                    qqHelper.DisposeString(_AutoPlusEnhancement_QuotedPremium)
                    '_HasApartmentBuildings = Nothing
                    'qqHelper.DisposeString(_NumberOfLocationsWithApartments)
                    'qqHelper.DisposeString(_ApartmentQuotedPremium)
                    '_HasRestaurantEndorsement = Nothing
                    'qqHelper.DisposeString(_RestaurantQuotedPremium)
                    qqHelper.DisposeString(_Liability_UM_UIM_AggregateLiabilityIncrementTypeId) 'covDetail; covCodeId 21552
                    qqHelper.DisposeString(_Liability_UM_UIM_DeductibleCategoryTypeId) 'covDetail; covCodeId 21552
                    _HasUninsuredMotoristPropertyDamage = Nothing 'covCodeId 21539
                    qqHelper.DisposeString(_UninsuredMotoristPropertyDamageQuotedPremium) 'covCodeId 21539; may not be populated
                    qqHelper.DisposeString(_MedicalPaymentsTypeId) 'covDetail; covCodeId 21540
                    _HasPhysicalDamageOtherThanCollision = Nothing 'covCodeId 21550
                    qqHelper.DisposeString(_PhysicalDamageOtherThanCollisionQuotedPremium) 'covCodeId 21550; may not be populated
                    _HasPhysicalDamageCollision = Nothing 'covCodeId 21551
                    qqHelper.DisposeString(_PhysicalDamageCollisionQuotedPremium) 'covCodeId 21551; may not be populated
                    qqHelper.DisposeString(_PhysicalDamageCollisionDeductibleId) 'covCodeId 21551
                    _HasGarageKeepersOtherThanCollision = Nothing 'covCodeId 21541
                    qqHelper.DisposeString(_GarageKeepersOtherThanCollisionQuotedPremium) 'covCodeId 21541
                    'qqHelper.DisposeString(_GarageKeepersOtherThanCollisionManualLimitAmount) 'covCodeId 21541; will stay w/ PolicyLevel and RiskLevel stuff
                    'qqHelper.DisposeString(_GarageKeepersOtherThanCollisionBasisTypeId) 'covDetail; covCodeId 21541; will stay w/ PolicyLevel and RiskLevel stuff
                    'qqHelper.DisposeString(_GarageKeepersOtherThanCollisionDeductibleCategoryTypeId) 'covDetail; covCodeId 21541; will stay w/ PolicyLevel and RiskLevel stuff
                    'qqHelper.DisposeString(_GarageKeepersOtherThanCollisionTypeId) 'covDetail; covCodeId 21541; will stay w/ PolicyLevel and RiskLevel stuff
                    'qqHelper.DisposeString(_GarageKeepersOtherThanCollisionDeductibleId) 'covCodeId 21541; will stay w/ PolicyLevel and RiskLevel stuff
                    _HasGarageKeepersCollision = Nothing 'covCodeId 21542
                    qqHelper.DisposeString(_GarageKeepersCollisionQuotedPremium) 'covCodeId 21542
                    'qqHelper.DisposeString(_GarageKeepersCollisionManualLimitAmount) 'covCodeId 21542; will stay w/ PolicyLevel and RiskLevel stuff
                    'qqHelper.DisposeString(_GarageKeepersCollisionBasisTypeId) 'covDetail; covCodeId 21542; will stay w/ PolicyLevel and RiskLevel stuff
                    'qqHelper.DisposeString(_GarageKeepersCollisionDeductibleId) 'covCodeId 21542; will stay w/ PolicyLevel and RiskLevel stuff
                    qqHelper.DisposeString(_MultiLineDiscountValue)
                    qqHelper.DisposeString(_PriorBodilyInjuryLimitId)

                    'added 9/25/2018 for multi-state
                    qqHelper.DisposeString(_Liability_UM_UIM_DeductibleId) 'covCodeId 21552
                    qqHelper.DisposeString(_UninsuredMotoristPropertyDamageLimitId) 'covCodeId 21539; note: same prop exists on Vehicle
                    qqHelper.DisposeString(_UninsuredMotoristPropertyDamageDeductibleId) 'covCodeId 21539
                    qqHelper.DisposeString(_UnderinsuredMotoristBodilyInjuryLiabilityLimitId) 'covCodeId 21548
                    qqHelper.DisposeString(_UnderinsuredMotoristBodilyInjuryLiabilityQuotedPremium) 'covCodeId 21548

                    'added 7/9/2021
                    _HasFoodManufacturersEnhancement = Nothing 'covCodeId 100000
                    qqHelper.DisposeString(_FoodManufacturersEnhancementQuotedPremium) 'covCodeId 100000
                    qqHelper.DisposeString(_CPP_CPR_FoodManufacturersEnhancementQuotedPremium) 'covCodeId 100000
                    qqHelper.DisposeString(_CPP_CGL_FoodManufacturersEnhancementQuotedPremium) 'covCodeId 100000

                    'Added 6/27/2022 for task 75780 MLW
                    _Has_PackageCPR_PlusEnhancementEndorsement = Nothing
                    qqHelper.DisposeString(_PackageCPR_PlusEnhancementEndorsementQuotedPremium)

                    'Added 8/21/2023 for task WS-855 BD
                    _HasFarmIndicator = Nothing
                    
                    qqHelper.DisposeString(_QuoteEffectiveDate)
                    _QuoteTransactionType = Nothing
                    _LobType = Nothing

                    If _BasePolicyLevelInfo IsNot Nothing Then
                        _BasePolicyLevelInfo.Dispose()
                        _BasePolicyLevelInfo = Nothing
                    End If

                    _HasFarmAllStar = Nothing
                    qqHelper.DisposeString(_FarmAllStarWaterBackupLimitId)
                    qqHelper.DisposeString(_FarmAllStarWaterDamageLimitId)
                    qqHelper.DisposeString(_OwnersLesseesorContractorsCompletedOperationsTotalPremium)

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
#End Region

    End Class
End Namespace
