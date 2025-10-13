Imports System.Web
Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' object used to store risk-level  lob-specific information for a quote; also includes properties that were previously on QuickQuote only
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()>
    Public Class QuickQuoteLobRiskLevelInfoExtended 'added 7/23/2018
        Inherits QuickQuoteBaseGenericObject(Of Object) 'updated 8/16/2018 from Inherits QuickQuoteLobRiskLevelInfo
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        'RiskLevel
        Private _Dec_BuildingLimit_All_Premium As String
        Private _Dec_BuildingPersPropLimit_All_Premium As String
        Private _HasLocation As Boolean
        Private _HasLocationWithBuilding As Boolean
        Private _HasLocationWithClassification As Boolean
        Private _VehiclesTotal_CombinedSingleLimitLiablityQuotedPremium As String
        Private _VehiclesTotal_MedicalPaymentsQuotedPremium As String
        Private _VehiclesTotal_UninsuredMotoristLiabilityQuotedPremium As String
        Private _VehiclesTotal_UnderinsuredMotoristBodilyInjuryLiabilityQuotedPremium As String
        Private _VehiclesTotal_UM_UIM_CovsQuotedPremium As String 'combines _VehiclesTotal_UninsuredMotoristLiabilityQuotedPremium and _VehiclesTotal_UnderinsuredMotoristBodilyInjuryLiabilityQuotedPremium
        Private _VehiclesTotal_ComprehensiveCoverageQuotedPremium As String
        Private _VehiclesTotal_CollisionCoverageQuotedPremium As String
        Private _VehiclesTotal_TowingAndLaborQuotedPremium As String
        Private _VehiclesTotal_RentalReimbursementQuotedPremium As String
        Private _CPR_BuildingsTotal_BuildingCovQuotedPremium As String
        Private _CPR_BuildingsTotal_PersPropCoverageQuotedPremium As String
        Private _CPR_BuildingsTotal_PersPropOfOthersQuotedPremium As String
        Private _CPR_BuildingsTotal_BusinessIncomeCovQuotedPremium As String
        Private _CPR_BuildingsTotal_EQ_QuotedPremium As String
        Private _LocationsTotal_EquipmentBreakdownQuotedPremium As String
        Private _LocationsTotal_PropertyInTheOpenRecords_QuotedPremium As String
        Private _LocationsTotal_PropertyInTheOpenRecords_EQ_Premium As String
        Private _LocationsTotal_PitoRecords_And_BuildingsTotal_Combined_EQ_Premium As String
        Private _VehiclesTotal_PremiumFullTerm As String
        Private _LocationsTotal_PremiumFullTerm As String
        Private _Locations_BuildingsTotal_PremiumFullTerm As String
        Private _CanUseDriverNumForDriverReconciliation As Boolean
        Private _CanUseVehicleNumForVehicleReconciliation As Boolean
        Private _CanUseLocationNumForLocationReconciliation As Boolean
        Private _CanUseApplicantNumForApplicantReconciliation As Boolean
        Private _VehiclesTotal_BodilyInjuryLiabilityQuotedPremium As String
        Private _VehiclesTotal_PropertyDamageQuotedPremium As String
        Private _VehiclesTotal_UninsuredCombinedSingleQuotedPremium As String
        Private _VehiclesTotal_UninsuredMotoristPropertyDamageQuotedPremium As String
        Private _VehiclesTotal_UninsuredMotoristPropertyDamageDeductibleQuotedPremium As String
        Private _VehiclesTotal_PollutionLiabilityBroadenedCoverageQuotedPremium As String
        Private _VehiclesTotal_TransportationExpenseQuotedPremium As String
        Private _VehiclesTotal_AutoLoanOrLeaseQuotedPremium As String
        Private _VehiclesTotal_TapesAndRecordsQuotedPremium As String
        Private _VehiclesTotal_SoundEquipmentQuotedPremium As String
        Private _VehiclesTotal_ElectronicEquipmentQuotedPremium As String
        Private _VehiclesTotal_TripInterruptionQuotedPremium As String
        Private _CanUseOperatorNumForOperatorReconciliation As Boolean
        Private _Locations_InlandMarinesTotal_Premium As String
        Private _Locations_InlandMarinesTotal_CoveragePremium As String
        Private _Locations_RvWatercraftsTotal_Premium As String
        Private _Locations_RvWatercraftsTotal_CoveragesPremium As String
        Private _Locations_Farm_L_Liability_QuotedPremium As String
        Private _Locations_Farm_M_Medical_Payments_QuotedPremium As String
        Private _LocationsTotal_LiabilityQuotedPremium As String 'loc covCodeId 10111
        Private _LocationsTotal_MedicalPaymentsQuotedPremium As String 'loc covCodeId 10112
        Private _LocationsTotal_PhysicalDamageOtherThanCollisionStandardOpenLotsQuotedPremium As String 'loc covCodeId 10116
        Private _LocationsTotal_ClassIIEmployees25AndOlder As String
        Private _LocationsTotal_ClassIIEmployeesUnderAge25 As String
        Private _LocationsTotal_ClassIOtherEmployees As String
        Private _LocationsTotal_ClassIRegularEmployees As String
        Private _LocationsTotal_NumberOfEmployees As String
        Private _LocationsTotal_Payroll As String
        Private _LocationsTotal_UninsuredUnderinsuredMotoristBIandPDNumberOfPlates As String 'covCodeId 10113; covDetail
        Private _LocationsTotal_ClassIEmployees As String
        Private _LocationsTotal_ClassIIEmployees As String
        Private _LocationsTotal_ClassIandIIEmployees As String
        Private _LocationsTotal_DealersBlanketCollisionQuotedPremium As String 'loc covCodeId 10120
        Private _LocationsTotal_PhysicalDamageOtherThanCollisionBuildingQuotedPremium As String 'loc covCodeId 10115
        Private _LocationsTotal_PhysicalDamageOtherThanCollisionBuildingManualLimitAmount As String 'loc covCodeId 10115
        Private _LocationsTotal_PhysicalDamageOtherThanCollisionNonStandardOpenLotsQuotedPremium As String 'loc covCodeId 10117
        Private _LocationsTotal_PhysicalDamageOtherThanCollisionNonStandardOpenLotsManualLimitAmount As String 'loc covCodeId 10117
        Private _LocationsTotal_PhysicalDamageOtherThanCollisionMiscellaneousBuildingsQuotedPremium As String 'loc covCodeId 10118
        Private _LocationsTotal_PhysicalDamageOtherThanCollisionMiscellaneousBuildingsManualLimitAmount As String 'loc covCodeId 10118
        Private _LocationsTotal_PhysicalDamageOtherThanCollisionMiscellaneousOpenLotsQuotedPremium As String 'loc covCodeId 10119
        Private _LocationsTotal_PhysicalDamageOtherThanCollisionMiscellaneousOpenLotsManualLimitAmount As String 'loc covCodeId 10119
        Private _LocationsTotal_PhysicalDamageOtherThanCollisionTotalQuotedPremium As String 'SUM of loc covCodeIds 10115, 10116, 10117, 10118, and 10119
        Private _LocationsTotal_PhysicalDamageOtherThanCollisionTotalManualLimitAmount As String 'SUM of loc covCodeIds 10115, 10116, 10117, 10118, and 10119
        Private _Locations_PhysicalDamageOtherThanCollisionDeductibleCategoryTypeId As String 'covDetail; loc covCodeIds 10115, 10116, 10117, 10118, and 10119
        Private _Locations_PhysicalDamageOtherThanCollisionTypeId As String 'covDetail; loc covCodeIds 10115, 10116, 10117, 10118, and 10119
        Private _Locations_PhysicalDamageOtherThanCollisionDeductibleId As String 'loc covCodeIds 10115, 10116, 10117, 10118, and 10119
        Private _CAP_GAR_LocationLevelCovs_Premium As String
        Private _CAP_GAR_VehicleLevelCovs_Premium As String
        Private _LocationsTotal_UninsuredUnderinsuredMotoristBIandPDQuotedPremium As String 'loc covCodeId 10113
        Private _LocationsTotal_GarageKeepersOtherThanCollisionQuotedPremium As String 'loc covCodeId 10086
        Private _LocationsTotal_GarageKeepersCollisionQuotedPremium As String 'loc covCodeId 10087
        Private _LocationsTotal_GarageKeepersCoverageExtensionsQuotedPremium As String 'loc covCodeId 10126
        Private _VehiclesTotal_CAP_GAR_TotalCoveragesPremium As String 'should essentially match CAP_GAR_VehicleLevelCovs_Premium
        Private _VehiclesTotal_TotalCoveragesPremium As String
        Private _DriversTotal_TotalCoveragesPremium As String
        'added 10/12/2018
        Private _VehiclesTotal_UnderinsuredCombinedSingleLimitQuotedPremium As String 'covCodeId 296 (PPA IL only)
        Private _VehiclesTotal_UninsuredBodilyInjuryQuotedPremium As String 'covCodeId 294 (PPA IL, HOM IN, DFR IN, FAR IN/IL)
        Private _VehiclesTotal_UnderinsuredBodilyInjuryQuotedPremium As String 'covCodeId 295 (PPA IL only)
        Private _VehiclesTotal_UM_UIM_CombinedSingleLimitQuotedPremium As String 'VehiclesTotal_UninsuredCombinedSingleQuotedPremium: covCodeId 10007 (PPA IN only) or covCodeId 7 (PPA IL only) + VehiclesTotal_UnderinsuredCombinedSingleLimitQuotedPremium: covCodeId 296 (PPA IL only)
        Private _VehiclesTotal_UM_UIM_BodilyInjuryLiabilityQuotedPremium As String 'VehiclesTotal_UninsuredMotoristLiabilityQuotedPremium: covCodeId 8 (PPA IN/IL, CAP IN/IL, GAR IN) + VehiclesTotal_UnderinsuredMotoristBodilyInjuryLiabilityQuotedPremium: covCodeId 30013 (CAP IN/IL, GAR IN) + VehiclesTotal_UninsuredBodilyInjuryQuotedPremium: covCodeId 294 (PPA IL, HOM IN, DFR IN, FAR IN/IL) + VehiclesTotal_UnderinsuredBodilyInjuryQuotedPremium: covCodeId 295 (PPA IL only)

        Private _QuoteEffectiveDate As String
        Private _QuoteTransactionType As QuickQuoteObject.QuickQuoteTransactionType
        Private _LobType As QuickQuoteObject.QuickQuoteLobType

        'added 8/16/2018
        Private _BaseRiskLevelInfo As QuickQuoteLobRiskLevelInfo


        'RiskLevel
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>combined prem for Diamond coverage w/ coveragecode_id 165 on all buildings</remarks>
        Public Property Dec_BuildingLimit_All_Premium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_Dec_BuildingLimit_All_Premium)
            End Get
            Set(value As String)
                _Dec_BuildingLimit_All_Premium = value
                qqHelper.ConvertToQuotedPremiumFormat(_Dec_BuildingLimit_All_Premium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>combined prem for Diamond coverage w/ coveragecode_id 21037 on all buildings</remarks>
        Public Property Dec_BuildingPersPropLimit_All_Premium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_Dec_BuildingPersPropLimit_All_Premium)
            End Get
            Set(value As String)
                _Dec_BuildingPersPropLimit_All_Premium = value
                qqHelper.ConvertToQuotedPremiumFormat(_Dec_BuildingPersPropLimit_All_Premium)
            End Set
        End Property
        Public Property HasLocation As Boolean
            Get
                Return _HasLocation
            End Get
            Set(value As Boolean)
                _HasLocation = value
            End Set
        End Property
        Public Property HasLocationWithBuilding As Boolean
            Get
                Return _HasLocationWithBuilding
            End Get
            Set(value As Boolean)
                _HasLocationWithBuilding = value
            End Set
        End Property
        Public Property HasLocationWithClassification As Boolean
            Get
                Return _HasLocationWithClassification
            End Get
            Set(value As Boolean)
                _HasLocationWithClassification = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond vehicle coverage w/ coveragecode_id 2; sum of all vehicles</remarks>
        Public Property VehiclesTotal_CombinedSingleLimitLiablityQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_VehiclesTotal_CombinedSingleLimitLiablityQuotedPremium)
            End Get
            Set(value As String)
                _VehiclesTotal_CombinedSingleLimitLiablityQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_VehiclesTotal_CombinedSingleLimitLiablityQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond vehicle coverage w/ coveragecode_id 60006 (CAP) or 6 (PPA); sum of all vehicles</remarks>
        Public Property VehiclesTotal_MedicalPaymentsQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_VehiclesTotal_MedicalPaymentsQuotedPremium)
            End Get
            Set(value As String)
                _VehiclesTotal_MedicalPaymentsQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_VehiclesTotal_MedicalPaymentsQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond vehicle coverage w/ coveragecode_id 8; sum of all vehicles</remarks>
        Public Property VehiclesTotal_UninsuredMotoristLiabilityQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_VehiclesTotal_UninsuredMotoristLiabilityQuotedPremium)
            End Get
            Set(value As String)
                _VehiclesTotal_UninsuredMotoristLiabilityQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_VehiclesTotal_UninsuredMotoristLiabilityQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond vehicle coverage w/ coveragecode_id 30013; sum of all vehicles</remarks>
        Public Property VehiclesTotal_UnderinsuredMotoristBodilyInjuryLiabilityQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_VehiclesTotal_UnderinsuredMotoristBodilyInjuryLiabilityQuotedPremium)
            End Get
            Set(value As String)
                _VehiclesTotal_UnderinsuredMotoristBodilyInjuryLiabilityQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_VehiclesTotal_UnderinsuredMotoristBodilyInjuryLiabilityQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond vehicle coverages w/ coveragecode_ids 8 and 30013; sum of all vehicles</remarks>
        Public Property VehiclesTotal_UM_UIM_CovsQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_VehiclesTotal_UM_UIM_CovsQuotedPremium)
            End Get
            Set(value As String)
                _VehiclesTotal_UM_UIM_CovsQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_VehiclesTotal_UM_UIM_CovsQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond vehicle coverage w/ coveragecode_id 3; sum of all vehicles</remarks>
        Public Property VehiclesTotal_ComprehensiveCoverageQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_VehiclesTotal_ComprehensiveCoverageQuotedPremium)
            End Get
            Set(value As String)
                _VehiclesTotal_ComprehensiveCoverageQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_VehiclesTotal_ComprehensiveCoverageQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond vehicle coverage w/ coveragecode_id 5; sum of all vehicles</remarks>
        Public Property VehiclesTotal_CollisionCoverageQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_VehiclesTotal_CollisionCoverageQuotedPremium)
            End Get
            Set(value As String)
                _VehiclesTotal_CollisionCoverageQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_VehiclesTotal_CollisionCoverageQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond vehicle coverage w/ coveragecode_id 60008; sum of all vehicles</remarks>
        Public Property VehiclesTotal_TowingAndLaborQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_VehiclesTotal_TowingAndLaborQuotedPremium)
            End Get
            Set(value As String)
                _VehiclesTotal_TowingAndLaborQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_VehiclesTotal_TowingAndLaborQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond vehicle coverages w/ coveragecode_ids 10094 (Comp) and 10095 (Coll); sum of all vehicles</remarks>
        Public Property VehiclesTotal_RentalReimbursementQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_VehiclesTotal_RentalReimbursementQuotedPremium)
            End Get
            Set(value As String)
                _VehiclesTotal_RentalReimbursementQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_VehiclesTotal_RentalReimbursementQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>combined prem for Diamond coverage w/ coveragecode_id 165 on all buildings</remarks>
        Public Property CPR_BuildingsTotal_BuildingCovQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_CPR_BuildingsTotal_BuildingCovQuotedPremium)
            End Get
            Set(value As String)
                _CPR_BuildingsTotal_BuildingCovQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_CPR_BuildingsTotal_BuildingCovQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>combined prem for Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]) on all buildings; found inside building ScheduledCoverage</remarks>
        Public Property CPR_BuildingsTotal_PersPropCoverageQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_CPR_BuildingsTotal_PersPropCoverageQuotedPremium)
            End Get
            Set(value As String)
                _CPR_BuildingsTotal_PersPropCoverageQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_CPR_BuildingsTotal_PersPropCoverageQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>combined prem for Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId = 8 [Personal Property of Others]) on all buildings; found inside building ScheduledCoverage</remarks>
        Public Property CPR_BuildingsTotal_PersPropOfOthersQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_CPR_BuildingsTotal_PersPropOfOthersQuotedPremium)
            End Get
            Set(value As String)
                _CPR_BuildingsTotal_PersPropOfOthersQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_CPR_BuildingsTotal_PersPropOfOthersQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>combined prem for Diamond coverage w/ coveragecode_id 21095 on all buildings</remarks>
        Public Property CPR_BuildingsTotal_BusinessIncomeCovQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_CPR_BuildingsTotal_BusinessIncomeCovQuotedPremium)
            End Get
            Set(value As String)
                _CPR_BuildingsTotal_BusinessIncomeCovQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_CPR_BuildingsTotal_BusinessIncomeCovQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>combined prem for Diamond coverages w/ coveragecode_ids 21155 (building EQ), 21163 (business income EQ), and 21160 (persProp and persPropOfOthers EQ; found inside building ScheduledCoverage) on all buildings</remarks>
        Public Property CPR_BuildingsTotal_EQ_QuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_CPR_BuildingsTotal_EQ_QuotedPremium)
            End Get
            Set(value As String)
                _CPR_BuildingsTotal_EQ_QuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_CPR_BuildingsTotal_EQ_QuotedPremium)
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>combined prem for Diamond coverage w/ coveragecode_id 21042 on all locations</remarks>
        Public Property LocationsTotal_EquipmentBreakdownQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_LocationsTotal_EquipmentBreakdownQuotedPremium)
            End Get
            Set(value As String)
                _LocationsTotal_EquipmentBreakdownQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_LocationsTotal_EquipmentBreakdownQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>combined prem for Diamond coverage w/ coveragecode_id 21107 on all locations; found inside location ScheduledCoverage where UICoverageScheduledCoverageParentTypeId = 91 (Property in the Open)</remarks>
        Public Property LocationsTotal_PropertyInTheOpenRecords_QuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_LocationsTotal_PropertyInTheOpenRecords_QuotedPremium)
            End Get
            Set(value As String)
                _LocationsTotal_PropertyInTheOpenRecords_QuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_LocationsTotal_PropertyInTheOpenRecords_QuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>combined prem for Diamond coverage w/ coveragecode_id 21520 on all locations; found inside location ScheduledCoverage where UICoverageScheduledCoverageParentTypeId = 91 (Property in the Open)</remarks>
        Public Property LocationsTotal_PropertyInTheOpenRecords_EQ_Premium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_LocationsTotal_PropertyInTheOpenRecords_EQ_Premium)
            End Get
            Set(value As String)
                _LocationsTotal_PropertyInTheOpenRecords_EQ_Premium = value
                qqHelper.ConvertToQuotedPremiumFormat(_LocationsTotal_PropertyInTheOpenRecords_EQ_Premium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks><see cref="LocationsTotal_PropertyInTheOpenRecords_EQ_Premium"/> + <see cref="CPR_BuildingsTotal_EQ_QuotedPremium"/></remarks>
        Public Property LocationsTotal_PitoRecords_And_BuildingsTotal_Combined_EQ_Premium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_LocationsTotal_PitoRecords_And_BuildingsTotal_Combined_EQ_Premium)
            End Get
            Set(value As String)
                _LocationsTotal_PitoRecords_And_BuildingsTotal_Combined_EQ_Premium = value
                qqHelper.ConvertToQuotedPremiumFormat(_LocationsTotal_PitoRecords_And_BuildingsTotal_Combined_EQ_Premium)
            End Set
        End Property
        Public Property VehiclesTotal_PremiumFullTerm As String
            Get
                Return qqHelper.QuotedPremiumFormat(_VehiclesTotal_PremiumFullTerm)
            End Get
            Set(value As String)
                _VehiclesTotal_PremiumFullTerm = value
                qqHelper.ConvertToQuotedPremiumFormat(_VehiclesTotal_PremiumFullTerm)
            End Set
        End Property
        Public Property LocationsTotal_PremiumFullTerm As String
            Get
                Return qqHelper.QuotedPremiumFormat(_LocationsTotal_PremiumFullTerm)
            End Get
            Set(value As String)
                _LocationsTotal_PremiumFullTerm = value
                qqHelper.ConvertToQuotedPremiumFormat(_LocationsTotal_PremiumFullTerm)
            End Set
        End Property
        Public Property Locations_BuildingsTotal_PremiumFullTerm As String
            Get
                Return qqHelper.QuotedPremiumFormat(_Locations_BuildingsTotal_PremiumFullTerm)
            End Get
            Set(value As String)
                _Locations_BuildingsTotal_PremiumFullTerm = value
                qqHelper.ConvertToQuotedPremiumFormat(_Locations_BuildingsTotal_PremiumFullTerm)
            End Set
        End Property
        Public Property CanUseDriverNumForDriverReconciliation As Boolean
            Get
                Return _CanUseDriverNumForDriverReconciliation
            End Get
            Set(value As Boolean)
                _CanUseDriverNumForDriverReconciliation = value
            End Set
        End Property
        Public Property CanUseVehicleNumForVehicleReconciliation As Boolean
            Get
                Return _CanUseVehicleNumForVehicleReconciliation
            End Get
            Set(value As Boolean)
                _CanUseVehicleNumForVehicleReconciliation = value
            End Set
        End Property
        Public Property CanUseLocationNumForLocationReconciliation As Boolean
            Get
                Return _CanUseLocationNumForLocationReconciliation
            End Get
            Set(value As Boolean)
                _CanUseLocationNumForLocationReconciliation = value
            End Set
        End Property
        Public Property CanUseApplicantNumForApplicantReconciliation As Boolean
            Get
                Return _CanUseApplicantNumForApplicantReconciliation
            End Get
            Set(value As Boolean)
                _CanUseApplicantNumForApplicantReconciliation = value
            End Set
        End Property
        Public Property VehiclesTotal_BodilyInjuryLiabilityQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_VehiclesTotal_BodilyInjuryLiabilityQuotedPremium)
            End Get
            Set(value As String)
                _VehiclesTotal_BodilyInjuryLiabilityQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_VehiclesTotal_BodilyInjuryLiabilityQuotedPremium)
            End Set
        End Property
        Public Property VehiclesTotal_PropertyDamageQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_VehiclesTotal_PropertyDamageQuotedPremium)
            End Get
            Set(value As String)
                _VehiclesTotal_PropertyDamageQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_VehiclesTotal_PropertyDamageQuotedPremium)
            End Set
        End Property
        Public Property VehiclesTotal_UninsuredCombinedSingleQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_VehiclesTotal_UninsuredCombinedSingleQuotedPremium)
            End Get
            Set(value As String)
                _VehiclesTotal_UninsuredCombinedSingleQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_VehiclesTotal_UninsuredCombinedSingleQuotedPremium)
            End Set
        End Property
        Public Property VehiclesTotal_UninsuredMotoristPropertyDamageQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_VehiclesTotal_UninsuredMotoristPropertyDamageQuotedPremium)
            End Get
            Set(value As String)
                _VehiclesTotal_UninsuredMotoristPropertyDamageQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_VehiclesTotal_UninsuredMotoristPropertyDamageQuotedPremium)
            End Set
        End Property
        Public Property VehiclesTotal_UninsuredMotoristPropertyDamageDeductibleQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_VehiclesTotal_UninsuredMotoristPropertyDamageDeductibleQuotedPremium)
            End Get
            Set(value As String)
                _VehiclesTotal_UninsuredMotoristPropertyDamageDeductibleQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_VehiclesTotal_UninsuredMotoristPropertyDamageDeductibleQuotedPremium)
            End Set
        End Property
        Public Property VehiclesTotal_PollutionLiabilityBroadenedCoverageQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_VehiclesTotal_PollutionLiabilityBroadenedCoverageQuotedPremium)
            End Get
            Set(value As String)
                _VehiclesTotal_PollutionLiabilityBroadenedCoverageQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_VehiclesTotal_PollutionLiabilityBroadenedCoverageQuotedPremium)
            End Set
        End Property
        Public Property VehiclesTotal_TransportationExpenseQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_VehiclesTotal_TransportationExpenseQuotedPremium)
            End Get
            Set(value As String)
                _VehiclesTotal_TransportationExpenseQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_VehiclesTotal_TransportationExpenseQuotedPremium)
            End Set
        End Property
        Public Property VehiclesTotal_AutoLoanOrLeaseQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_VehiclesTotal_AutoLoanOrLeaseQuotedPremium)
            End Get
            Set(value As String)
                _VehiclesTotal_AutoLoanOrLeaseQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_VehiclesTotal_AutoLoanOrLeaseQuotedPremium)
            End Set
        End Property
        Public Property VehiclesTotal_TapesAndRecordsQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_VehiclesTotal_TapesAndRecordsQuotedPremium)
            End Get
            Set(value As String)
                _VehiclesTotal_TapesAndRecordsQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_VehiclesTotal_TapesAndRecordsQuotedPremium)
            End Set
        End Property
        Public Property VehiclesTotal_SoundEquipmentQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_VehiclesTotal_SoundEquipmentQuotedPremium)
            End Get
            Set(value As String)
                _VehiclesTotal_SoundEquipmentQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_VehiclesTotal_SoundEquipmentQuotedPremium)
            End Set
        End Property
        Public Property VehiclesTotal_ElectronicEquipmentQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_VehiclesTotal_ElectronicEquipmentQuotedPremium)
            End Get
            Set(value As String)
                _VehiclesTotal_ElectronicEquipmentQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_VehiclesTotal_ElectronicEquipmentQuotedPremium)
            End Set
        End Property
        Public Property VehiclesTotal_TripInterruptionQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_VehiclesTotal_TripInterruptionQuotedPremium)
            End Get
            Set(value As String)
                _VehiclesTotal_TripInterruptionQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_VehiclesTotal_TripInterruptionQuotedPremium)
            End Set
        End Property
        Public Property CanUseOperatorNumForOperatorReconciliation As Boolean
            Get
                Return _CanUseOperatorNumForOperatorReconciliation
            End Get
            Set(value As Boolean)
                _CanUseOperatorNumForOperatorReconciliation = value
            End Set
        End Property
        Public Property Locations_InlandMarinesTotal_Premium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_Locations_InlandMarinesTotal_Premium)
            End Get
            Set(value As String)
                _Locations_InlandMarinesTotal_Premium = value
                qqHelper.ConvertToQuotedPremiumFormat(_Locations_InlandMarinesTotal_Premium)
            End Set
        End Property
        Public Property Locations_InlandMarinesTotal_CoveragePremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_Locations_InlandMarinesTotal_CoveragePremium)
            End Get
            Set(value As String)
                _Locations_InlandMarinesTotal_CoveragePremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_Locations_InlandMarinesTotal_CoveragePremium)
            End Set
        End Property
        Public Property Locations_RvWatercraftsTotal_Premium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_Locations_RvWatercraftsTotal_Premium)
            End Get
            Set(value As String)
                _Locations_RvWatercraftsTotal_Premium = value
                qqHelper.ConvertToQuotedPremiumFormat(_Locations_RvWatercraftsTotal_Premium)
            End Set
        End Property
        Public Property Locations_RvWatercraftsTotal_CoveragesPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_Locations_RvWatercraftsTotal_CoveragesPremium)
            End Get
            Set(value As String)
                _Locations_RvWatercraftsTotal_CoveragesPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_Locations_RvWatercraftsTotal_CoveragesPremium)
            End Set
        End Property
        Public Property Locations_Farm_L_Liability_QuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_Locations_Farm_L_Liability_QuotedPremium)
            End Get
            Set(value As String)
                _Locations_Farm_L_Liability_QuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_Locations_Farm_L_Liability_QuotedPremium)
            End Set
        End Property
        Public Property Locations_Farm_M_Medical_Payments_QuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_Locations_Farm_M_Medical_Payments_QuotedPremium)
            End Get
            Set(value As String)
                _Locations_Farm_M_Medical_Payments_QuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_Locations_Farm_M_Medical_Payments_QuotedPremium)
            End Set
        End Property
        Public Property LocationsTotal_LiabilityQuotedPremium As String 'loc covCodeId 10111
            Get
                Return qqHelper.QuotedPremiumFormat(_LocationsTotal_LiabilityQuotedPremium)
            End Get
            Set(value As String)
                _LocationsTotal_LiabilityQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_LocationsTotal_LiabilityQuotedPremium)
            End Set
        End Property
        Public Property LocationsTotal_MedicalPaymentsQuotedPremium As String 'loc covCodeId 10112
            Get
                Return qqHelper.QuotedPremiumFormat(_LocationsTotal_MedicalPaymentsQuotedPremium)
            End Get
            Set(value As String)
                _LocationsTotal_MedicalPaymentsQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_LocationsTotal_MedicalPaymentsQuotedPremium)
            End Set
        End Property
        Public Property LocationsTotal_PhysicalDamageOtherThanCollisionStandardOpenLotsQuotedPremium As String 'loc covCodeId 10116
            Get
                Return qqHelper.QuotedPremiumFormat(_LocationsTotal_PhysicalDamageOtherThanCollisionStandardOpenLotsQuotedPremium)
            End Get
            Set(value As String)
                _LocationsTotal_PhysicalDamageOtherThanCollisionStandardOpenLotsQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_LocationsTotal_PhysicalDamageOtherThanCollisionStandardOpenLotsQuotedPremium)
            End Set
        End Property
        Public Property LocationsTotal_ClassIIEmployees25AndOlder As String
            Get
                Return _LocationsTotal_ClassIIEmployees25AndOlder
            End Get
            Set(value As String)
                _LocationsTotal_ClassIIEmployees25AndOlder = value
                qqHelper.ConvertToLimitFormat(_LocationsTotal_ClassIIEmployees25AndOlder)
            End Set
        End Property
        Public Property LocationsTotal_ClassIIEmployeesUnderAge25 As String
            Get
                Return _LocationsTotal_ClassIIEmployeesUnderAge25
            End Get
            Set(value As String)
                _LocationsTotal_ClassIIEmployeesUnderAge25 = value
                qqHelper.ConvertToLimitFormat(_LocationsTotal_ClassIIEmployeesUnderAge25)
            End Set
        End Property
        Public Property LocationsTotal_ClassIOtherEmployees As String
            Get
                Return _LocationsTotal_ClassIOtherEmployees
            End Get
            Set(value As String)
                _LocationsTotal_ClassIOtherEmployees = value
                qqHelper.ConvertToLimitFormat(_LocationsTotal_ClassIOtherEmployees)
            End Set
        End Property
        Public Property LocationsTotal_ClassIRegularEmployees As String
            Get
                Return _LocationsTotal_ClassIRegularEmployees
            End Get
            Set(value As String)
                _LocationsTotal_ClassIRegularEmployees = value
                qqHelper.ConvertToLimitFormat(_LocationsTotal_ClassIRegularEmployees)
            End Set
        End Property
        Public Property LocationsTotal_NumberOfEmployees As String
            Get
                Return _LocationsTotal_NumberOfEmployees
            End Get
            Set(value As String)
                _LocationsTotal_NumberOfEmployees = value
                qqHelper.ConvertToLimitFormat(_LocationsTotal_NumberOfEmployees)
            End Set
        End Property
        Public Property LocationsTotal_Payroll As String
            Get
                Return _LocationsTotal_Payroll
            End Get
            Set(value As String)
                _LocationsTotal_Payroll = value
                qqHelper.ConvertToLimitFormat(_LocationsTotal_Payroll)
            End Set
        End Property
        Public Property LocationsTotal_UninsuredUnderinsuredMotoristBIandPDNumberOfPlates As String 'covCodeId 10113; covDetail
            Get
                Return _LocationsTotal_UninsuredUnderinsuredMotoristBIandPDNumberOfPlates
            End Get
            Set(value As String)
                _LocationsTotal_UninsuredUnderinsuredMotoristBIandPDNumberOfPlates = value
                qqHelper.ConvertToLimitFormat(_LocationsTotal_UninsuredUnderinsuredMotoristBIandPDNumberOfPlates)
            End Set
        End Property
        Public Property LocationsTotal_ClassIEmployees As String
            Get
                Return _LocationsTotal_ClassIEmployees
            End Get
            Set(value As String)
                _LocationsTotal_ClassIEmployees = value
                qqHelper.ConvertToLimitFormat(_LocationsTotal_ClassIEmployees)
            End Set
        End Property
        Public Property LocationsTotal_ClassIIEmployees As String
            Get
                Return _LocationsTotal_ClassIIEmployees
            End Get
            Set(value As String)
                _LocationsTotal_ClassIIEmployees = value
                qqHelper.ConvertToLimitFormat(_LocationsTotal_ClassIIEmployees)
            End Set
        End Property
        Public Property LocationsTotal_ClassIandIIEmployees As String
            Get
                Return _LocationsTotal_ClassIandIIEmployees
            End Get
            Set(value As String)
                _LocationsTotal_ClassIandIIEmployees = value
                qqHelper.ConvertToLimitFormat(_LocationsTotal_ClassIandIIEmployees)
            End Set
        End Property
        Public Property LocationsTotal_DealersBlanketCollisionQuotedPremium As String 'loc covCodeId 10120
            Get
                Return qqHelper.QuotedPremiumFormat(_LocationsTotal_DealersBlanketCollisionQuotedPremium)
            End Get
            Set(value As String)
                _LocationsTotal_DealersBlanketCollisionQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_LocationsTotal_DealersBlanketCollisionQuotedPremium)
            End Set
        End Property
        Public Property LocationsTotal_PhysicalDamageOtherThanCollisionBuildingQuotedPremium As String 'loc covCodeId 10115
            Get
                Return qqHelper.QuotedPremiumFormat(_LocationsTotal_PhysicalDamageOtherThanCollisionBuildingQuotedPremium)
            End Get
            Set(value As String)
                _LocationsTotal_PhysicalDamageOtherThanCollisionBuildingQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_LocationsTotal_PhysicalDamageOtherThanCollisionBuildingQuotedPremium)
            End Set
        End Property
        Public Property LocationsTotal_PhysicalDamageOtherThanCollisionBuildingManualLimitAmount As String 'loc covCodeId 10115
            Get
                Return _LocationsTotal_PhysicalDamageOtherThanCollisionBuildingManualLimitAmount
            End Get
            Set(value As String)
                _LocationsTotal_PhysicalDamageOtherThanCollisionBuildingManualLimitAmount = value
                qqHelper.ConvertToLimitFormat(_LocationsTotal_PhysicalDamageOtherThanCollisionBuildingManualLimitAmount)
            End Set
        End Property
        Public Property LocationsTotal_PhysicalDamageOtherThanCollisionNonStandardOpenLotsQuotedPremium As String 'loc covCodeId 10117
            Get
                Return qqHelper.QuotedPremiumFormat(_LocationsTotal_PhysicalDamageOtherThanCollisionNonStandardOpenLotsQuotedPremium)
            End Get
            Set(value As String)
                _LocationsTotal_PhysicalDamageOtherThanCollisionNonStandardOpenLotsQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_LocationsTotal_PhysicalDamageOtherThanCollisionNonStandardOpenLotsQuotedPremium)
            End Set
        End Property
        Public Property LocationsTotal_PhysicalDamageOtherThanCollisionNonStandardOpenLotsManualLimitAmount As String 'loc covCodeId 10117
            Get
                Return _LocationsTotal_PhysicalDamageOtherThanCollisionNonStandardOpenLotsManualLimitAmount
            End Get
            Set(value As String)
                _LocationsTotal_PhysicalDamageOtherThanCollisionNonStandardOpenLotsManualLimitAmount = value
                qqHelper.ConvertToLimitFormat(_LocationsTotal_PhysicalDamageOtherThanCollisionNonStandardOpenLotsManualLimitAmount)
            End Set
        End Property
        Public Property LocationsTotal_PhysicalDamageOtherThanCollisionMiscellaneousBuildingsQuotedPremium As String 'loc covCodeId 10118
            Get
                Return qqHelper.QuotedPremiumFormat(_LocationsTotal_PhysicalDamageOtherThanCollisionMiscellaneousBuildingsQuotedPremium)
            End Get
            Set(value As String)
                _LocationsTotal_PhysicalDamageOtherThanCollisionMiscellaneousBuildingsQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_LocationsTotal_PhysicalDamageOtherThanCollisionMiscellaneousBuildingsQuotedPremium)
            End Set
        End Property
        Public Property LocationsTotal_PhysicalDamageOtherThanCollisionMiscellaneousBuildingsManualLimitAmount As String 'loc covCodeId 10118
            Get
                Return _LocationsTotal_PhysicalDamageOtherThanCollisionMiscellaneousBuildingsManualLimitAmount
            End Get
            Set(value As String)
                _LocationsTotal_PhysicalDamageOtherThanCollisionMiscellaneousBuildingsManualLimitAmount = value
                qqHelper.ConvertToLimitFormat(_LocationsTotal_PhysicalDamageOtherThanCollisionMiscellaneousBuildingsManualLimitAmount)
            End Set
        End Property
        Public Property LocationsTotal_PhysicalDamageOtherThanCollisionMiscellaneousOpenLotsQuotedPremium As String 'loc covCodeId 10119
            Get
                Return qqHelper.QuotedPremiumFormat(_LocationsTotal_PhysicalDamageOtherThanCollisionMiscellaneousOpenLotsQuotedPremium)
            End Get
            Set(value As String)
                _LocationsTotal_PhysicalDamageOtherThanCollisionMiscellaneousOpenLotsQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_LocationsTotal_PhysicalDamageOtherThanCollisionMiscellaneousOpenLotsQuotedPremium)
            End Set
        End Property
        Public Property LocationsTotal_PhysicalDamageOtherThanCollisionMiscellaneousOpenLotsManualLimitAmount As String 'loc covCodeId 10119
            Get
                Return _LocationsTotal_PhysicalDamageOtherThanCollisionMiscellaneousOpenLotsManualLimitAmount
            End Get
            Set(value As String)
                _LocationsTotal_PhysicalDamageOtherThanCollisionMiscellaneousOpenLotsManualLimitAmount = value
                qqHelper.ConvertToLimitFormat(_LocationsTotal_PhysicalDamageOtherThanCollisionMiscellaneousOpenLotsManualLimitAmount)
            End Set
        End Property
        Public Property LocationsTotal_PhysicalDamageOtherThanCollisionTotalQuotedPremium As String 'SUM of loc covCodeIds 10115, 10116, 10117, 10118, and 10119
            Get
                Return qqHelper.QuotedPremiumFormat(_LocationsTotal_PhysicalDamageOtherThanCollisionTotalQuotedPremium)
            End Get
            Set(value As String)
                _LocationsTotal_PhysicalDamageOtherThanCollisionTotalQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_LocationsTotal_PhysicalDamageOtherThanCollisionTotalQuotedPremium)
            End Set
        End Property
        Public Property LocationsTotal_PhysicalDamageOtherThanCollisionTotalManualLimitAmount As String 'SUM of loc covCodeIds 10115, 10116, 10117, 10118, and 10119
            Get
                Return _LocationsTotal_PhysicalDamageOtherThanCollisionTotalManualLimitAmount
            End Get
            Set(value As String)
                _LocationsTotal_PhysicalDamageOtherThanCollisionTotalManualLimitAmount = value
                qqHelper.ConvertToLimitFormat(_LocationsTotal_PhysicalDamageOtherThanCollisionTotalManualLimitAmount)
            End Set
        End Property
        Public Property Locations_PhysicalDamageOtherThanCollisionDeductibleCategoryTypeId As String 'covDetail; loc covCodeIds 10115, 10116, 10117, 10118, and 10119
            Get
                Return _Locations_PhysicalDamageOtherThanCollisionDeductibleCategoryTypeId
            End Get
            Set(value As String)
                _Locations_PhysicalDamageOtherThanCollisionDeductibleCategoryTypeId = value
            End Set
        End Property
        Public ReadOnly Property Locations_PhysicalDamageOtherThanCollisionDeductibleCategoryType As String
            Get
                Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.Locations_PhysicalDamageOtherThanCollisionDeductibleCategoryTypeId, _Locations_PhysicalDamageOtherThanCollisionDeductibleCategoryTypeId)
            End Get
        End Property
        Public Property Locations_PhysicalDamageOtherThanCollisionTypeId As String 'covDetail; loc covCodeIds 10115, 10116, 10117, 10118, and 10119
            Get
                Return _Locations_PhysicalDamageOtherThanCollisionTypeId
            End Get
            Set(value As String)
                _Locations_PhysicalDamageOtherThanCollisionTypeId = value
            End Set
        End Property
        Public ReadOnly Property Locations_PhysicalDamageOtherThanCollisionType As String
            Get
                Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.Locations_PhysicalDamageOtherThanCollisionTypeId, _Locations_PhysicalDamageOtherThanCollisionTypeId)
            End Get
        End Property
        Public Property Locations_PhysicalDamageOtherThanCollisionDeductibleId As String 'loc covCodeIds 10115, 10116, 10117, 10118, and 10119
            Get
                Return _Locations_PhysicalDamageOtherThanCollisionDeductibleId
            End Get
            Set(value As String)
                _Locations_PhysicalDamageOtherThanCollisionDeductibleId = value
            End Set
        End Property
        Public ReadOnly Property Locations_PhysicalDamageOtherThanCollisionDeductible As String
            Get
                Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.Locations_PhysicalDamageOtherThanCollisionDeductibleId, _Locations_PhysicalDamageOtherThanCollisionDeductibleId)
            End Get
        End Property
        Public Property CAP_GAR_LocationLevelCovs_Premium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_CAP_GAR_LocationLevelCovs_Premium)
            End Get
            Set(value As String)
                _CAP_GAR_LocationLevelCovs_Premium = value
                qqHelper.ConvertToQuotedPremiumFormat(_CAP_GAR_LocationLevelCovs_Premium)
            End Set
        End Property
        Public Property CAP_GAR_VehicleLevelCovs_Premium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_CAP_GAR_VehicleLevelCovs_Premium)
            End Get
            Set(value As String)
                _CAP_GAR_VehicleLevelCovs_Premium = value
                qqHelper.ConvertToQuotedPremiumFormat(_CAP_GAR_VehicleLevelCovs_Premium)
            End Set
        End Property
        Public Property LocationsTotal_UninsuredUnderinsuredMotoristBIandPDQuotedPremium As String 'loc covCodeId 10113
            Get
                Return qqHelper.QuotedPremiumFormat(_LocationsTotal_UninsuredUnderinsuredMotoristBIandPDQuotedPremium)
            End Get
            Set(value As String)
                _LocationsTotal_UninsuredUnderinsuredMotoristBIandPDQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_LocationsTotal_UninsuredUnderinsuredMotoristBIandPDQuotedPremium)
            End Set
        End Property
        Public Property LocationsTotal_GarageKeepersOtherThanCollisionQuotedPremium As String 'loc covCodeId 10086
            Get
                Return qqHelper.QuotedPremiumFormat(_LocationsTotal_GarageKeepersOtherThanCollisionQuotedPremium)
            End Get
            Set(value As String)
                _LocationsTotal_GarageKeepersOtherThanCollisionQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_LocationsTotal_GarageKeepersOtherThanCollisionQuotedPremium)
            End Set
        End Property
        Public Property LocationsTotal_GarageKeepersCollisionQuotedPremium As String 'loc covCodeId 10087
            Get
                Return qqHelper.QuotedPremiumFormat(_LocationsTotal_GarageKeepersCollisionQuotedPremium)
            End Get
            Set(value As String)
                _LocationsTotal_GarageKeepersCollisionQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_LocationsTotal_GarageKeepersCollisionQuotedPremium)
            End Set
        End Property
        Public Property LocationsTotal_GarageKeepersCoverageExtensionsQuotedPremium As String 'loc covCodeId 10126
            Get
                Return qqHelper.QuotedPremiumFormat(_LocationsTotal_GarageKeepersCoverageExtensionsQuotedPremium)
            End Get
            Set(value As String)
                _LocationsTotal_GarageKeepersCoverageExtensionsQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_LocationsTotal_GarageKeepersCoverageExtensionsQuotedPremium)
            End Set
        End Property
        Public Property VehiclesTotal_CAP_GAR_TotalCoveragesPremium As String 'should essentially match CAP_GAR_VehicleLevelCovs_Premium
            Get
                Return qqHelper.QuotedPremiumFormat(_VehiclesTotal_CAP_GAR_TotalCoveragesPremium)
            End Get
            Set(value As String)
                _VehiclesTotal_CAP_GAR_TotalCoveragesPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_VehiclesTotal_CAP_GAR_TotalCoveragesPremium)
            End Set
        End Property
        Public Property VehiclesTotal_TotalCoveragesPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_VehiclesTotal_TotalCoveragesPremium)
            End Get
            Set(value As String)
                _VehiclesTotal_TotalCoveragesPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_VehiclesTotal_TotalCoveragesPremium)
            End Set
        End Property
        Public Property DriversTotal_TotalCoveragesPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_DriversTotal_TotalCoveragesPremium)
            End Get
            Set(value As String)
                _DriversTotal_TotalCoveragesPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_DriversTotal_TotalCoveragesPremium)
            End Set
        End Property
        'added 10/12/2018
        Public Property VehiclesTotal_UnderinsuredCombinedSingleLimitQuotedPremium As String 'covCodeId 296 (PPA IL only)
            Get
                Return qqHelper.QuotedPremiumFormat(_VehiclesTotal_UnderinsuredCombinedSingleLimitQuotedPremium)
            End Get
            Set(value As String)
                _VehiclesTotal_UnderinsuredCombinedSingleLimitQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_VehiclesTotal_UnderinsuredCombinedSingleLimitQuotedPremium)
            End Set
        End Property
        Public Property VehiclesTotal_UninsuredBodilyInjuryQuotedPremium As String 'covCodeId 294 (PPA IL, HOM IN, DFR IN, FAR IN/IL)
            Get
                Return qqHelper.QuotedPremiumFormat(_VehiclesTotal_UninsuredBodilyInjuryQuotedPremium)
            End Get
            Set(value As String)
                _VehiclesTotal_UninsuredBodilyInjuryQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_VehiclesTotal_UninsuredBodilyInjuryQuotedPremium)
            End Set
        End Property
        Public Property VehiclesTotal_UnderinsuredBodilyInjuryQuotedPremium As String 'covCodeId 295 (PPA IL only)
            Get
                Return qqHelper.QuotedPremiumFormat(_VehiclesTotal_UnderinsuredBodilyInjuryQuotedPremium)
            End Get
            Set(value As String)
                _VehiclesTotal_UnderinsuredBodilyInjuryQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_VehiclesTotal_UnderinsuredBodilyInjuryQuotedPremium)
            End Set
        End Property
        Public Property VehiclesTotal_UM_UIM_CombinedSingleLimitQuotedPremium As String 'VehiclesTotal_UninsuredCombinedSingleQuotedPremium: covCodeId 10007 (PPA IN only) or covCodeId 7 (PPA IL only) + VehiclesTotal_UnderinsuredCombinedSingleLimitQuotedPremium: covCodeId 296 (PPA IL only)
            Get
                Return qqHelper.QuotedPremiumFormat(_VehiclesTotal_UM_UIM_CombinedSingleLimitQuotedPremium)
            End Get
            Set(value As String)
                _VehiclesTotal_UM_UIM_CombinedSingleLimitQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_VehiclesTotal_UM_UIM_CombinedSingleLimitQuotedPremium)
            End Set
        End Property
        Public Property VehiclesTotal_UM_UIM_BodilyInjuryLiabilityQuotedPremium As String 'VehiclesTotal_UninsuredMotoristLiabilityQuotedPremium: covCodeId 8 (PPA IN/IL, CAP IN/IL, GAR IN) + VehiclesTotal_UnderinsuredMotoristBodilyInjuryLiabilityQuotedPremium: covCodeId 30013 (CAP IN/IL, GAR IN) + VehiclesTotal_UninsuredBodilyInjuryQuotedPremium: covCodeId 294 (PPA IL, HOM IN, DFR IN, FAR IN/IL) + VehiclesTotal_UnderinsuredBodilyInjuryQuotedPremium: covCodeId 295 (PPA IL only)
            Get
                Return qqHelper.QuotedPremiumFormat(_VehiclesTotal_UM_UIM_BodilyInjuryLiabilityQuotedPremium)
            End Get
            Set(value As String)
                _VehiclesTotal_UM_UIM_BodilyInjuryLiabilityQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_VehiclesTotal_UM_UIM_BodilyInjuryLiabilityQuotedPremium)
            End Set
        End Property
        'moved from PolicyLevel object 7/23/2018
        Public ReadOnly Property CustomerAutoLegalQuotedPremium As String
            Get
                Dim total As String = ""
                If Locations IsNot Nothing AndAlso Locations.Count > 0 Then '7/9/2018 note: using Locations public property instead of _Locations private variable
                    For Each Loc As QuickQuoteLocation In Locations
                        If Not String.IsNullOrWhiteSpace(Loc.CustomerAutoLegalLiabilityQuotedPremium) AndAlso IsNumeric(Loc.CustomerAutoLegalLiabilityQuotedPremium) Then
                            total = qqHelper.getSum(total, Loc.CustomerAutoLegalLiabilityQuotedPremium)
                        End If
                    Next
                End If
                qqHelper.ConvertToQuotedPremiumFormat(total)
                Return total
            End Get
        End Property
        Public ReadOnly Property TenantAutoLegalQuotedPremium As String
            Get
                Dim total As String = ""
                If Locations IsNot Nothing AndAlso Locations.Count > 0 Then
                    For Each Loc As QuickQuoteLocation In Locations
                        If Not String.IsNullOrWhiteSpace(Loc.TenantAutoLegalLiabilityQuotedPremium) AndAlso IsNumeric(Loc.TenantAutoLegalLiabilityQuotedPremium) Then
                            total = qqHelper.getSum(total, Loc.TenantAutoLegalLiabilityQuotedPremium)
                        End If
                    Next
                End If
                qqHelper.ConvertToQuotedPremiumFormat(total)
                Return total
            End Get
        End Property
        Public ReadOnly Property FineArtsLocationQuotedPremium As String
            Get
                Dim total As String = ""
                If Locations IsNot Nothing AndAlso Locations.Count > 0 Then '7/9/2018 note: using Locations public property instead of _Locations private variable
                    For Each Loc As QuickQuoteLocation In Locations
                        If Not String.IsNullOrWhiteSpace(Loc.FineArtsQuotedPremium) AndAlso IsNumeric(Loc.FineArtsQuotedPremium) Then
                            total = qqHelper.getSum(total, Loc.FineArtsQuotedPremium)
                        End If
                    Next
                End If
                qqHelper.ConvertToQuotedPremiumFormat(total)
                Return total
            End Get
        End Property
        Public ReadOnly Property ComputerBuildingsTotalLimit As String 'won't use private variable; just ReadOnly prop... should equal sum of Coverage.ManualLimitAmount for applicable buildings covs
            Get
                '10/15/2018 note: shouldn't need to call ResetStateLevelLocationsIfNeeded (like is being done from VersionAndLobInfo object) since this should only be used in spots where lists should already be set (i.e. Saving or Summary/Proposal)
                Dim tot As String = "0"
                If Locations IsNot Nothing AndAlso Locations.Count > 0 Then 'updated 7/21/2018 from private variable to public property
                    For Each l As QuickQuoteLocation In Locations
                        If l.Buildings IsNot Nothing AndAlso l.Buildings.Count > 0 Then
                            For Each b As QuickQuoteBuilding In l.Buildings
                                tot = qqHelper.getSum(tot, b.ComputerHardwareLimit)
                                tot = qqHelper.getSum(tot, b.ComputerProgramsApplicationsAndMediaLimit)
                                tot = qqHelper.getSum(tot, b.ComputerBusinessIncomeLimit)
                            Next
                        End If
                    Next
                End If
                qqHelper.ConvertToLimitFormat(tot)

                Return tot
            End Get
        End Property
        Public ReadOnly Property ComputerBuildingsTotalQuotedPremium As String 'won't use private variable; just ReadOnly prop... should equal sum of Coverage.FullTermPremium for applicable buildings covs
            Get
                '10/15/2018 note: shouldn't need to call ResetStateLevelLocationsIfNeeded (like is being done from VersionAndLobInfo object) since this should only be used in spots where lists should already be set (i.e. Saving or Summary/Proposal)
                Dim tot As String = "0"
                If Locations IsNot Nothing AndAlso Locations.Count > 0 Then 'updated 7/21/2018 from private variable to public property
                    For Each l As QuickQuoteLocation In Locations
                        If l.Buildings IsNot Nothing AndAlso l.Buildings.Count > 0 Then
                            For Each b As QuickQuoteBuilding In l.Buildings
                                tot = qqHelper.getSum(tot, b.ComputerHardwareQuotedPremium)
                                tot = qqHelper.getSum(tot, b.ComputerProgramsApplicationsAndMediaQuotedPremium)
                                tot = qqHelper.getSum(tot, b.ComputerBusinessIncomeQuotedPremium)
                            Next
                        End If
                    Next
                End If
                qqHelper.ConvertToQuotedPremiumFormat(tot)

                Return tot
            End Get
        End Property
        Public ReadOnly Property FineArtsBuildingsTotalLimit As String 'won't use private variable; just ReadOnly prop... should equal sum of Coverage.ManualLimitAmount for applicable buildings covs
            Get
                '10/15/2018 note: shouldn't need to call ResetStateLevelLocationsIfNeeded (like is being done from VersionAndLobInfo object) since this should only be used in spots where lists should already be set (i.e. Saving or Summary/Proposal)
                Dim tot As String = "0"
                If Locations IsNot Nothing AndAlso Locations.Count > 0 Then 'updated 7/21/2018 from private variable to public property
                    For Each l As QuickQuoteLocation In Locations
                        If l.Buildings IsNot Nothing AndAlso l.Buildings.Count > 0 Then
                            For Each b As QuickQuoteBuilding In l.Buildings
                                If b.FineArtsScheduledItems IsNot Nothing AndAlso b.FineArtsScheduledItems.Count > 0 Then
                                    For Each fa As QuickQuoteFineArtsScheduledItem In b.FineArtsScheduledItems
                                        tot = qqHelper.getSum(tot, fa.Limit)
                                    Next
                                End If
                            Next
                        End If
                    Next
                End If
                qqHelper.ConvertToLimitFormat(tot)

                Return tot
            End Get
        End Property
        Public ReadOnly Property FineArtsBuildingsTotalQuotedPremium As String 'won't use private variable; just ReadOnly prop... should equal sum of Coverage.FullTermPremium for applicable buildings covs
            Get
                '10/15/2018 note: shouldn't need to call ResetStateLevelLocationsIfNeeded (like is being done from VersionAndLobInfo object) since this should only be used in spots where lists should already be set (i.e. Saving or Summary/Proposal)
                Dim tot As String = "0"
                If Locations IsNot Nothing AndAlso Locations.Count > 0 Then 'updated 7/21/2018 from private variable to public property
                    For Each l As QuickQuoteLocation In Locations
                        If l.Buildings IsNot Nothing AndAlso l.Buildings.Count > 0 Then
                            For Each b As QuickQuoteBuilding In l.Buildings
                                If b.FineArtsScheduledItems IsNot Nothing AndAlso b.FineArtsScheduledItems.Count > 0 Then
                                    For Each fa As QuickQuoteFineArtsScheduledItem In b.FineArtsScheduledItems
                                        tot = qqHelper.getSum(tot, fa.QuotedPremium)
                                    Next
                                End If
                            Next
                        End If
                    Next
                End If
                qqHelper.ConvertToQuotedPremiumFormat(tot)

                Return tot
            End Get
        End Property
        Public ReadOnly Property SignsBuildingTotalLimit As String 'won't use private variable; just ReadOnly prop... should equal sum of Coverage.ManualLimitAmount for building scheduled/unscheduled signs
            Get
                '10/15/2018 note: shouldn't need to call ResetStateLevelLocationsIfNeeded (like is being done from VersionAndLobInfo object) since this should only be used in spots where lists should already be set (i.e. Saving or Summary/Proposal)
                Dim tot As String = "0"
                If Locations IsNot Nothing AndAlso Locations.Count > 0 Then 'updated 7/19/2018 from private variable to public property
                    For Each l As QuickQuoteLocation In Locations
                        If l.Buildings IsNot Nothing AndAlso l.Buildings.Count > 0 Then
                            For Each b As QuickQuoteBuilding In l.Buildings
                                If b.ScheduledSigns IsNot Nothing AndAlso b.ScheduledSigns.Count > 0 Then
                                    For Each ss As QuickQuoteScheduledSign In b.ScheduledSigns
                                        tot = qqHelper.getSum(tot, ss.Limit)
                                    Next
                                End If
                                tot = qqHelper.getSum(tot, b.UnscheduledSignsLimit)
                            Next
                        End If
                    Next
                End If
                qqHelper.ConvertToLimitFormat(tot)

                Return tot
            End Get
        End Property
        Public ReadOnly Property SignsBuildingTotalQuotedPremium As String 'won't use private variable; just ReadOnly prop... should equal sum of Coverage.FullTermPremium for building scheduled/unscheduled signs
            Get
                '10/15/2018 note: shouldn't need to call ResetStateLevelLocationsIfNeeded (like is being done from VersionAndLobInfo object) since this should only be used in spots where lists should already be set (i.e. Saving or Summary/Proposal)
                Dim tot As String = "0"
                If Locations IsNot Nothing AndAlso Locations.Count > 0 Then 'updated 7/19/2018 from private variable to public property
                    For Each l As QuickQuoteLocation In Locations
                        If l.Buildings IsNot Nothing AndAlso l.Buildings.Count > 0 Then
                            For Each b As QuickQuoteBuilding In l.Buildings
                                If b.ScheduledSigns IsNot Nothing AndAlso b.ScheduledSigns.Count > 0 Then
                                    For Each ss As QuickQuoteScheduledSign In b.ScheduledSigns
                                        tot = qqHelper.getSum(tot, ss.QuotedPremium)
                                    Next
                                End If
                                tot = qqHelper.getSum(tot, b.UnscheduledSignsQuotedPremium)
                            Next
                        End If
                    Next
                End If
                qqHelper.ConvertToQuotedPremiumFormat(tot)

                Return tot
            End Get
        End Property

        'added 8/16/2018
        Public Property BaseRiskLevelInfo As QuickQuoteLobRiskLevelInfo
            Get
                If _BaseRiskLevelInfo Is Nothing Then
                    _BaseRiskLevelInfo = New QuickQuoteLobRiskLevelInfo
                End If
                SetObjectsParent(_BaseRiskLevelInfo)
                Return _BaseRiskLevelInfo
            End Get
            Set(value As QuickQuoteLobRiskLevelInfo)
                _BaseRiskLevelInfo = value
                SetObjectsParent(_BaseRiskLevelInfo)
            End Set
        End Property

        'properties that were previously inherited from QuickQuoteLobRiskLevelInfo
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Applicants As Generic.List(Of QuickQuoteApplicant)
            Get
                Return BaseRiskLevelInfo.Applicants
            End Get
            Set(value As Generic.List(Of QuickQuoteApplicant))
                BaseRiskLevelInfo.Applicants = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Drivers As Generic.List(Of QuickQuoteDriver)
            Get
                Return BaseRiskLevelInfo.Drivers
            End Get
            Set(value As Generic.List(Of QuickQuoteDriver))
                BaseRiskLevelInfo.Drivers = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Locations As Generic.List(Of QuickQuoteLocation)
            Get
                Return BaseRiskLevelInfo.Locations
            End Get
            Set(value As Generic.List(Of QuickQuoteLocation))
                BaseRiskLevelInfo.Locations = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Vehicles As Generic.List(Of QuickQuoteVehicle)
            Get
                Return BaseRiskLevelInfo.Vehicles
            End Get
            Set(value As Generic.List(Of QuickQuoteVehicle))
                BaseRiskLevelInfo.Vehicles = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Operators As List(Of QuickQuoteOperator)
            Get
                Return BaseRiskLevelInfo.Operators
            End Get
            Set(value As List(Of QuickQuoteOperator))
                BaseRiskLevelInfo.Operators = value
            End Set
        End Property

        Public Sub New()
            MyBase.New()
            SetDefaults()
        End Sub
        'Public Sub New(Parent As QuickQuoteObject) 'added 6/27/2018; could probably just use generic type so one constructor could be used for multiple types; removed 7/27/2018 in lieu of new generic constructor... Parent will likely be VersionAndLobInfo instead of QuickQuoteObject or PackagePart anyway
        '    MyBase.New()
        '    SetDefaults()
        '    Me.SetParent = Parent
        'End Sub
        'Public Sub New(Parent As QuickQuotePackagePart) 'added 6/27/2018; could probably just use generic type so one constructor could be used for multiple types; removed 7/27/2018 in lieu of new generic constructor... Parent will likely be VersionAndLobInfo instead of QuickQuoteObject or PackagePart anyway
        '    MyBase.New()
        '    SetDefaults()
        '    Me.SetParent = Parent
        'End Sub
        Public Sub New(Parent As Object) 'added 7/27/2018 to replace multiple constructors for different objects
            'MyBase.New(Parent)
            'SetDefaults()
            'updated 8/16/2018
            Me.New()
            Me.SetParent = Parent 'already being done in Base constructor call; added back 8/16/2018 after removing inheritance from QuickQuoteLobRiskLevelInfo
        End Sub
        Private Sub SetDefaults()
            'added 8/16/2018
            _BaseRiskLevelInfo = New QuickQuoteLobRiskLevelInfo

            'RiskLevel
            _Dec_BuildingLimit_All_Premium = ""
            _Dec_BuildingPersPropLimit_All_Premium = ""
            _HasLocation = False
            _HasLocationWithBuilding = False
            _HasLocationWithClassification = False
            _VehiclesTotal_CombinedSingleLimitLiablityQuotedPremium = ""
            _VehiclesTotal_MedicalPaymentsQuotedPremium = ""
            _VehiclesTotal_UninsuredMotoristLiabilityQuotedPremium = ""
            _VehiclesTotal_UnderinsuredMotoristBodilyInjuryLiabilityQuotedPremium = ""
            _VehiclesTotal_UM_UIM_CovsQuotedPremium = ""
            _VehiclesTotal_ComprehensiveCoverageQuotedPremium = ""
            _VehiclesTotal_CollisionCoverageQuotedPremium = ""
            _VehiclesTotal_TowingAndLaborQuotedPremium = ""
            _VehiclesTotal_RentalReimbursementQuotedPremium = ""
            _CPR_BuildingsTotal_BuildingCovQuotedPremium = ""
            _CPR_BuildingsTotal_PersPropCoverageQuotedPremium = ""
            _CPR_BuildingsTotal_PersPropOfOthersQuotedPremium = ""
            _CPR_BuildingsTotal_BusinessIncomeCovQuotedPremium = ""
            _CPR_BuildingsTotal_EQ_QuotedPremium = ""
            _LocationsTotal_EquipmentBreakdownQuotedPremium = ""
            _LocationsTotal_PropertyInTheOpenRecords_QuotedPremium = ""
            _LocationsTotal_PropertyInTheOpenRecords_EQ_Premium = ""
            _LocationsTotal_PitoRecords_And_BuildingsTotal_Combined_EQ_Premium = ""
            _VehiclesTotal_PremiumFullTerm = ""
            _LocationsTotal_PremiumFullTerm = ""
            _Locations_BuildingsTotal_PremiumFullTerm = ""
            _CanUseDriverNumForDriverReconciliation = False
            _CanUseVehicleNumForVehicleReconciliation = False
            _CanUseLocationNumForLocationReconciliation = False
            _CanUseApplicantNumForApplicantReconciliation = False
            _VehiclesTotal_BodilyInjuryLiabilityQuotedPremium = ""
            _VehiclesTotal_PropertyDamageQuotedPremium = ""
            _VehiclesTotal_UninsuredCombinedSingleQuotedPremium = ""
            _VehiclesTotal_UninsuredMotoristPropertyDamageQuotedPremium = ""
            _VehiclesTotal_UninsuredMotoristPropertyDamageDeductibleQuotedPremium = ""
            _VehiclesTotal_PollutionLiabilityBroadenedCoverageQuotedPremium = ""
            _VehiclesTotal_TransportationExpenseQuotedPremium = ""
            _VehiclesTotal_AutoLoanOrLeaseQuotedPremium = ""
            _VehiclesTotal_TapesAndRecordsQuotedPremium = ""
            _VehiclesTotal_SoundEquipmentQuotedPremium = ""
            _VehiclesTotal_ElectronicEquipmentQuotedPremium = ""
            _VehiclesTotal_TripInterruptionQuotedPremium = ""
            _CanUseOperatorNumForOperatorReconciliation = False
            _Locations_InlandMarinesTotal_Premium = ""
            _Locations_InlandMarinesTotal_CoveragePremium = ""
            _Locations_RvWatercraftsTotal_Premium = ""
            _Locations_RvWatercraftsTotal_CoveragesPremium = ""
            _Locations_Farm_L_Liability_QuotedPremium = ""
            _Locations_Farm_M_Medical_Payments_QuotedPremium = ""
            _LocationsTotal_LiabilityQuotedPremium = "" 'loc covCodeId 10111
            _LocationsTotal_MedicalPaymentsQuotedPremium = "" 'loc covCodeId 10112
            _LocationsTotal_PhysicalDamageOtherThanCollisionStandardOpenLotsQuotedPremium = "" 'loc covCodeId 10116
            _LocationsTotal_ClassIIEmployees25AndOlder = ""
            _LocationsTotal_ClassIIEmployeesUnderAge25 = ""
            _LocationsTotal_ClassIOtherEmployees = ""
            _LocationsTotal_ClassIRegularEmployees = ""
            _LocationsTotal_NumberOfEmployees = ""
            _LocationsTotal_Payroll = ""
            _LocationsTotal_UninsuredUnderinsuredMotoristBIandPDNumberOfPlates = "" 'covCodeId 10113; covDetail
            _LocationsTotal_ClassIEmployees = ""
            _LocationsTotal_ClassIIEmployees = ""
            _LocationsTotal_ClassIandIIEmployees = ""
            _LocationsTotal_DealersBlanketCollisionQuotedPremium = "" 'loc covCodeId 10120
            _LocationsTotal_PhysicalDamageOtherThanCollisionBuildingQuotedPremium = "" 'loc covCodeId 10115
            _LocationsTotal_PhysicalDamageOtherThanCollisionBuildingManualLimitAmount = "" 'loc covCodeId 10115
            _LocationsTotal_PhysicalDamageOtherThanCollisionNonStandardOpenLotsQuotedPremium = "" 'loc covCodeId 10117
            _LocationsTotal_PhysicalDamageOtherThanCollisionNonStandardOpenLotsManualLimitAmount = "" 'loc covCodeId 10117
            _LocationsTotal_PhysicalDamageOtherThanCollisionMiscellaneousBuildingsQuotedPremium = "" 'loc covCodeId 10118
            _LocationsTotal_PhysicalDamageOtherThanCollisionMiscellaneousBuildingsManualLimitAmount = "" 'loc covCodeId 10118
            _LocationsTotal_PhysicalDamageOtherThanCollisionMiscellaneousOpenLotsQuotedPremium = "" 'loc covCodeId 10119
            _LocationsTotal_PhysicalDamageOtherThanCollisionMiscellaneousOpenLotsManualLimitAmount = "" 'loc covCodeId 10119
            _LocationsTotal_PhysicalDamageOtherThanCollisionTotalQuotedPremium = "" 'SUM of loc covCodeIds 10115, 10116, 10117, 10118, and 10119
            _LocationsTotal_PhysicalDamageOtherThanCollisionTotalManualLimitAmount = "" 'SUM of loc covCodeIds 10115, 10116, 10117, 10118, and 10119
            _Locations_PhysicalDamageOtherThanCollisionDeductibleCategoryTypeId = "" 'covDetail; loc covCodeIds 10115, 10116, 10117, 10118, and 10119
            _Locations_PhysicalDamageOtherThanCollisionTypeId = "" 'covDetail; loc covCodeIds 10115, 10116, 10117, 10118, and 10119
            _Locations_PhysicalDamageOtherThanCollisionDeductibleId = "" 'loc covCodeIds 10115, 10116, 10117, 10118, and 10119
            _CAP_GAR_LocationLevelCovs_Premium = ""
            _CAP_GAR_VehicleLevelCovs_Premium = ""
            _LocationsTotal_UninsuredUnderinsuredMotoristBIandPDQuotedPremium = "" 'loc covCodeId 10113
            _LocationsTotal_GarageKeepersOtherThanCollisionQuotedPremium = "" 'loc covCodeId 10086
            _LocationsTotal_GarageKeepersCollisionQuotedPremium = "" 'loc covCodeId 10087
            _LocationsTotal_GarageKeepersCoverageExtensionsQuotedPremium = "" 'loc covCodeId 10126
            _VehiclesTotal_CAP_GAR_TotalCoveragesPremium = "" 'should essentially match CAP_GAR_VehicleLevelCovs_Premium
            _VehiclesTotal_TotalCoveragesPremium = ""
            _DriversTotal_TotalCoveragesPremium = ""
            'added 10/12/2018
            _VehiclesTotal_UnderinsuredCombinedSingleLimitQuotedPremium = "" 'covCodeId 296 (PPA IL only)
            _VehiclesTotal_UninsuredBodilyInjuryQuotedPremium = "" 'covCodeId 294 (PPA IL, HOM IN, DFR IN, FAR IN/IL)
            _VehiclesTotal_UnderinsuredBodilyInjuryQuotedPremium = "" 'covCodeId 295 (PPA IL only)
            _VehiclesTotal_UM_UIM_CombinedSingleLimitQuotedPremium = "" 'VehiclesTotal_UninsuredCombinedSingleQuotedPremium: covCodeId 10007 (PPA IN only) or covCodeId 7 (PPA IL only) + VehiclesTotal_UnderinsuredCombinedSingleLimitQuotedPremium: covCodeId 296 (PPA IL only)
            _VehiclesTotal_UM_UIM_BodilyInjuryLiabilityQuotedPremium = "" 'VehiclesTotal_UninsuredMotoristLiabilityQuotedPremium: covCodeId 8 (PPA IN/IL, CAP IN/IL, GAR IN) + VehiclesTotal_UnderinsuredMotoristBodilyInjuryLiabilityQuotedPremium: covCodeId 30013 (CAP IN/IL, GAR IN) + VehiclesTotal_UninsuredBodilyInjuryQuotedPremium: covCodeId 294 (PPA IL, HOM IN, DFR IN, FAR IN/IL) + VehiclesTotal_UnderinsuredBodilyInjuryQuotedPremium: covCodeId 295 (PPA IL only)

            _QuoteEffectiveDate = ""
            _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.None
            _LobType = QuickQuoteObject.QuickQuoteLobType.None

        End Sub

        Public Overrides Function ToString() As String
            Dim str As String = ""
            If Me IsNot Nothing Then

                If Me.Applicants IsNot Nothing AndAlso Me.Applicants.Count > 0 Then
                    str = qqHelper.appendText(str, Me.Applicants.Count.ToString & " Applicants", vbCrLf)
                End If
                If Me.Drivers IsNot Nothing AndAlso Me.Drivers.Count > 0 Then
                    str = qqHelper.appendText(str, Me.Drivers.Count.ToString & " Drivers", vbCrLf)
                End If
                If Me.Locations IsNot Nothing AndAlso Me.Locations.Count > 0 Then
                    str = qqHelper.appendText(str, Me.Locations.Count.ToString & " Locations", vbCrLf)
                End If
                If Me.Vehicles IsNot Nothing AndAlso Me.Vehicles.Count > 0 Then
                    str = qqHelper.appendText(str, Me.Vehicles.Count.ToString & " Vehicles", vbCrLf)
                End If
                If Me.Operators IsNot Nothing AndAlso Me.Operators.Count > 0 Then
                    str = qqHelper.appendText(str, Me.Operators.Count.ToString & " Operators", vbCrLf)
                End If
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
        Protected Friend Sub Set_LobType(ByVal lob As QuickQuoteObject.QuickQuoteLobType) 'renamed 8/16/2018 from just LobType
            _LobType = lob
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        'Protected Overridable Sub Dispose(disposing As Boolean)
        'updated w/ QuickQuoteBaseObject inheritance
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).



                    'RiskLevel
                    qqHelper.DisposeString(_Dec_BuildingLimit_All_Premium)
                    qqHelper.DisposeString(_Dec_BuildingPersPropLimit_All_Premium)
                    _HasLocation = Nothing
                    _HasLocationWithBuilding = Nothing
                    _HasLocationWithClassification = Nothing
                    qqHelper.DisposeString(_VehiclesTotal_CombinedSingleLimitLiablityQuotedPremium)
                    qqHelper.DisposeString(_VehiclesTotal_MedicalPaymentsQuotedPremium)
                    qqHelper.DisposeString(_VehiclesTotal_UninsuredMotoristLiabilityQuotedPremium)
                    qqHelper.DisposeString(_VehiclesTotal_UnderinsuredMotoristBodilyInjuryLiabilityQuotedPremium)
                    qqHelper.DisposeString(_VehiclesTotal_UM_UIM_CovsQuotedPremium)
                    qqHelper.DisposeString(_VehiclesTotal_ComprehensiveCoverageQuotedPremium)
                    qqHelper.DisposeString(_VehiclesTotal_CollisionCoverageQuotedPremium)
                    qqHelper.DisposeString(_VehiclesTotal_TowingAndLaborQuotedPremium)
                    qqHelper.DisposeString(_VehiclesTotal_RentalReimbursementQuotedPremium)
                    qqHelper.DisposeString(_CPR_BuildingsTotal_BuildingCovQuotedPremium)
                    qqHelper.DisposeString(_CPR_BuildingsTotal_PersPropCoverageQuotedPremium)
                    qqHelper.DisposeString(_CPR_BuildingsTotal_PersPropOfOthersQuotedPremium)
                    qqHelper.DisposeString(_CPR_BuildingsTotal_BusinessIncomeCovQuotedPremium)
                    qqHelper.DisposeString(_CPR_BuildingsTotal_EQ_QuotedPremium)
                    qqHelper.DisposeString(_LocationsTotal_EquipmentBreakdownQuotedPremium)
                    qqHelper.DisposeString(_LocationsTotal_PropertyInTheOpenRecords_QuotedPremium)
                    qqHelper.DisposeString(_LocationsTotal_PropertyInTheOpenRecords_EQ_Premium)
                    qqHelper.DisposeString(_LocationsTotal_PitoRecords_And_BuildingsTotal_Combined_EQ_Premium)
                    qqHelper.DisposeString(_VehiclesTotal_PremiumFullTerm)
                    qqHelper.DisposeString(_LocationsTotal_PremiumFullTerm)
                    qqHelper.DisposeString(_Locations_BuildingsTotal_PremiumFullTerm)
                    _CanUseDriverNumForDriverReconciliation = Nothing
                    _CanUseVehicleNumForVehicleReconciliation = Nothing
                    _CanUseLocationNumForLocationReconciliation = Nothing
                    _CanUseApplicantNumForApplicantReconciliation = Nothing
                    qqHelper.DisposeString(_VehiclesTotal_BodilyInjuryLiabilityQuotedPremium)
                    qqHelper.DisposeString(_VehiclesTotal_PropertyDamageQuotedPremium)
                    qqHelper.DisposeString(_VehiclesTotal_UninsuredCombinedSingleQuotedPremium)
                    qqHelper.DisposeString(_VehiclesTotal_UninsuredMotoristPropertyDamageQuotedPremium)
                    qqHelper.DisposeString(_VehiclesTotal_UninsuredMotoristPropertyDamageDeductibleQuotedPremium)
                    qqHelper.DisposeString(_VehiclesTotal_PollutionLiabilityBroadenedCoverageQuotedPremium)
                    qqHelper.DisposeString(_VehiclesTotal_TransportationExpenseQuotedPremium)
                    qqHelper.DisposeString(_VehiclesTotal_AutoLoanOrLeaseQuotedPremium)
                    qqHelper.DisposeString(_VehiclesTotal_TapesAndRecordsQuotedPremium)
                    qqHelper.DisposeString(_VehiclesTotal_SoundEquipmentQuotedPremium)
                    qqHelper.DisposeString(_VehiclesTotal_ElectronicEquipmentQuotedPremium)
                    qqHelper.DisposeString(_VehiclesTotal_TripInterruptionQuotedPremium)
                    _CanUseOperatorNumForOperatorReconciliation = Nothing
                    qqHelper.DisposeString(_Locations_InlandMarinesTotal_Premium)
                    qqHelper.DisposeString(_Locations_InlandMarinesTotal_CoveragePremium)
                    qqHelper.DisposeString(_Locations_RvWatercraftsTotal_Premium)
                    qqHelper.DisposeString(_Locations_RvWatercraftsTotal_CoveragesPremium)
                    qqHelper.DisposeString(_Locations_Farm_L_Liability_QuotedPremium)
                    qqHelper.DisposeString(_Locations_Farm_M_Medical_Payments_QuotedPremium)
                    qqHelper.DisposeString(_LocationsTotal_LiabilityQuotedPremium) 'loc covCodeId 10111
                    qqHelper.DisposeString(_LocationsTotal_MedicalPaymentsQuotedPremium) 'loc covCodeId 10112
                    qqHelper.DisposeString(_LocationsTotal_PhysicalDamageOtherThanCollisionStandardOpenLotsQuotedPremium) 'loc covCodeId 10116
                    qqHelper.DisposeString(_LocationsTotal_ClassIIEmployees25AndOlder)
                    qqHelper.DisposeString(_LocationsTotal_ClassIIEmployeesUnderAge25)
                    qqHelper.DisposeString(_LocationsTotal_ClassIOtherEmployees)
                    qqHelper.DisposeString(_LocationsTotal_ClassIRegularEmployees)
                    qqHelper.DisposeString(_LocationsTotal_NumberOfEmployees)
                    qqHelper.DisposeString(_LocationsTotal_Payroll)
                    qqHelper.DisposeString(_LocationsTotal_UninsuredUnderinsuredMotoristBIandPDNumberOfPlates) 'covCodeId 10113; covDetail
                    qqHelper.DisposeString(_LocationsTotal_ClassIEmployees)
                    qqHelper.DisposeString(_LocationsTotal_ClassIIEmployees)
                    qqHelper.DisposeString(_LocationsTotal_ClassIandIIEmployees)
                    qqHelper.DisposeString(_LocationsTotal_DealersBlanketCollisionQuotedPremium) 'loc covCodeId 10120
                    qqHelper.DisposeString(_LocationsTotal_PhysicalDamageOtherThanCollisionBuildingQuotedPremium) 'loc covCodeId 10115
                    qqHelper.DisposeString(_LocationsTotal_PhysicalDamageOtherThanCollisionBuildingManualLimitAmount) 'loc covCodeId 10115
                    qqHelper.DisposeString(_LocationsTotal_PhysicalDamageOtherThanCollisionNonStandardOpenLotsQuotedPremium) 'loc covCodeId 10117
                    qqHelper.DisposeString(_LocationsTotal_PhysicalDamageOtherThanCollisionNonStandardOpenLotsManualLimitAmount) 'loc covCodeId 10117
                    qqHelper.DisposeString(_LocationsTotal_PhysicalDamageOtherThanCollisionMiscellaneousBuildingsQuotedPremium) 'loc covCodeId 10118
                    qqHelper.DisposeString(_LocationsTotal_PhysicalDamageOtherThanCollisionMiscellaneousBuildingsManualLimitAmount) 'loc covCodeId 10118
                    qqHelper.DisposeString(_LocationsTotal_PhysicalDamageOtherThanCollisionMiscellaneousOpenLotsQuotedPremium) 'loc covCodeId 10119
                    qqHelper.DisposeString(_LocationsTotal_PhysicalDamageOtherThanCollisionMiscellaneousOpenLotsManualLimitAmount) 'loc covCodeId 10119
                    qqHelper.DisposeString(_LocationsTotal_PhysicalDamageOtherThanCollisionTotalQuotedPremium) 'SUM of loc covCodeIds 10115, 10116, 10117, 10118, and 10119
                    qqHelper.DisposeString(_LocationsTotal_PhysicalDamageOtherThanCollisionTotalManualLimitAmount) 'SUM of loc covCodeIds 10115, 10116, 10117, 10118, and 10119
                    qqHelper.DisposeString(_Locations_PhysicalDamageOtherThanCollisionDeductibleCategoryTypeId) 'covDetail; loc covCodeIds 10115, 10116, 10117, 10118, and 10119
                    qqHelper.DisposeString(_Locations_PhysicalDamageOtherThanCollisionTypeId) 'covDetail; loc covCodeIds 10115, 10116, 10117, 10118, and 10119
                    qqHelper.DisposeString(_Locations_PhysicalDamageOtherThanCollisionDeductibleId) 'loc covCodeIds 10115, 10116, 10117, 10118, and 10119
                    qqHelper.DisposeString(_CAP_GAR_LocationLevelCovs_Premium)
                    qqHelper.DisposeString(_CAP_GAR_VehicleLevelCovs_Premium)
                    qqHelper.DisposeString(_LocationsTotal_UninsuredUnderinsuredMotoristBIandPDQuotedPremium) 'loc covCodeId 10113
                    qqHelper.DisposeString(_LocationsTotal_GarageKeepersOtherThanCollisionQuotedPremium) 'loc covCodeId 10086
                    qqHelper.DisposeString(_LocationsTotal_GarageKeepersCollisionQuotedPremium) 'loc covCodeId 10087
                    qqHelper.DisposeString(_LocationsTotal_GarageKeepersCoverageExtensionsQuotedPremium) 'loc covCodeId 10126
                    qqHelper.DisposeString(_VehiclesTotal_CAP_GAR_TotalCoveragesPremium) 'should essentially match CAP_GAR_VehicleLevelCovs_Premium
                    qqHelper.DisposeString(_VehiclesTotal_TotalCoveragesPremium)
                    qqHelper.DisposeString(_DriversTotal_TotalCoveragesPremium)
                    'added 10/12/2018
                    qqHelper.DisposeString(_VehiclesTotal_UnderinsuredCombinedSingleLimitQuotedPremium) 'covCodeId 296 (PPA IL only)
                    qqHelper.DisposeString(_VehiclesTotal_UninsuredBodilyInjuryQuotedPremium) 'covCodeId 294 (PPA IL, HOM IN, DFR IN, FAR IN/IL)
                    qqHelper.DisposeString(_VehiclesTotal_UnderinsuredBodilyInjuryQuotedPremium) 'covCodeId 295 (PPA IL only)
                    qqHelper.DisposeString(_VehiclesTotal_UM_UIM_CombinedSingleLimitQuotedPremium) 'VehiclesTotal_UninsuredCombinedSingleQuotedPremium: covCodeId 10007 (PPA IN only) or covCodeId 7 (PPA IL only) + VehiclesTotal_UnderinsuredCombinedSingleLimitQuotedPremium: covCodeId 296 (PPA IL only)
                    qqHelper.DisposeString(_VehiclesTotal_UM_UIM_BodilyInjuryLiabilityQuotedPremium) 'VehiclesTotal_UninsuredMotoristLiabilityQuotedPremium: covCodeId 8 (PPA IN/IL, CAP IN/IL, GAR IN) + VehiclesTotal_UnderinsuredMotoristBodilyInjuryLiabilityQuotedPremium: covCodeId 30013 (CAP IN/IL, GAR IN) + VehiclesTotal_UninsuredBodilyInjuryQuotedPremium: covCodeId 294 (PPA IL, HOM IN, DFR IN, FAR IN/IL) + VehiclesTotal_UnderinsuredBodilyInjuryQuotedPremium: covCodeId 295 (PPA IL only)

                    'added 8/16/2018
                    qqHelper.DisposeString(_QuoteEffectiveDate)
                    _QuoteTransactionType = Nothing
                    _LobType = Nothing

                    'added 8/16/2018
                    If _BaseRiskLevelInfo IsNot Nothing Then
                        _BaseRiskLevelInfo.Dispose()
                        _BaseRiskLevelInfo = Nothing
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
#End Region

    End Class
End Namespace

