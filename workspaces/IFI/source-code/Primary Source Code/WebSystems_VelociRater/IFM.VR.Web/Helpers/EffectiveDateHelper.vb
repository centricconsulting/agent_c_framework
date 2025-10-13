Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports System.Runtime.CompilerServices
Imports IFM.VR.Common.Helpers.HOM
Imports IFM.VR.Common.Helpers.FARM
Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers
Imports IFM.VR.Common.Helpers.DFR
Imports IFM.VR.Common.Helpers.PPA
Imports IFM.VR.Common.Helpers.CAP
Imports PublicQuotingLib.Models
Imports IFM.VR.Common.Helpers.CPR
Imports IFM.VR.Common.Helpers.BOP
Imports IFM.VR.Common.Helpers.CPP
Imports IFM.VR.Common.Helpers.CGL

Namespace Helpers
    Public Class EffectiveDateHelper

        Public Shared ReadOnly Property DaysAgoForPolicyToRestartAtPolicyHolder As Integer
            Get
                Dim chc As CommonHelperClass = New CommonHelperClass
                Dim configDays As Integer = 0
                Dim defaultDays As Integer = 61
                configDays = chc.ConfigurationAppSettingValueAsInteger("VR_NewCo_DaysAgoForPolicyToRestartAtPolicyHolder")
                If configDays > 0 Then
                    Return configDays
                Else
                    Return defaultDays
                End If
            End Get
        End Property

        'Public Shared Function isQuoteEffectiveDatePastDate(QuoteEffectiveDate As String, EffectiveDateToCheckAgainst As String) As Boolean
        '    If CDate(QuoteEffectiveDate) >= CDate(EffectiveDateToCheckAgainst) Then
        '        Return True
        '    Else
        '        Return False
        '    End If
        'End Function

        Public Shared Function isQuoteEffectiveDatePastDate(QuoteEffectiveDate As String, EffectiveDateToCheckAgainst As String) As Boolean
            Dim ConvertedQuoteEffDate As DateTime
            Dim ConvertedCheckDate As DateTime
            Dim ValidQuoteEffDate As Boolean
            Dim ValidCheckDate As Boolean

            ValidQuoteEffDate = DateTime.TryParse(QuoteEffectiveDate, ConvertedQuoteEffDate)
            ValidCheckDate = DateTime.TryParse(EffectiveDateToCheckAgainst, ConvertedCheckDate)

            If ValidQuoteEffDate AndAlso ValidCheckDate Then
                If ConvertedQuoteEffDate >= ConvertedCheckDate Then
                    Return True
                End If
            End If
            Return False
        End Function

        Public Shared Function isQuoteEffectiveDateBeforeDate(QuoteEffectiveDate As String, EffectiveDateToCheckAgainst As String) As Boolean
            Dim ConvertedQuoteEffDate As DateTime
            Dim ConvertedCheckDate As DateTime
            Dim ValidQuoteEffDate As Boolean
            Dim ValidCheckDate As Boolean

            ValidQuoteEffDate = DateTime.TryParse(QuoteEffectiveDate, ConvertedQuoteEffDate)
            ValidCheckDate = DateTime.TryParse(EffectiveDateToCheckAgainst, ConvertedCheckDate)

            If ValidQuoteEffDate AndAlso ValidCheckDate Then
                If ConvertedQuoteEffDate < ConvertedCheckDate Then
                    Return True
                End If
            End If
            Return False
        End Function

        Public Shared Function isQuoteVersionGreaterThanOrEqualVersion(QuoteVersion As String, VersionToCheck As String) As Boolean
            If QuoteVersion.TryToGetInt32 >= VersionToCheck.TryToGetInt32 Then
                Return True
            End If
            Return False
        End Function

        Public Shared Function isQuoteVersionLessThanVersion(QuoteVersion As String, VersionToCheck As String) As Boolean
            If QuoteVersion.TryToGetInt32 < VersionToCheck.TryToGetInt32 Then
                Return True
            End If
            Return False
        End Function

        Public Shared Function isQuoteEffectiveDatePastDate(QuoteEffectiveDate As DateTime, EffectiveDateToCheckAgainst As DateTime) As Boolean
            If QuoteEffectiveDate >= EffectiveDateToCheckAgainst Then
                Return True
            Else
                Return False
            End If
        End Function

        'Added 6/6/2022 for task 74147 MLW
        Public Shared Function DateLineCrossed(KeyStartDate As Date, NewEffectiveDate As Date, OldEffectiveDate As Date) As String
            Dim CrossDirection As Common.Helper.EnumsHelper.CrossDirectionEnum = Common.Helper.EnumsHelper.CrossDirectionEnum.NONE
            If NewEffectiveDate = OldEffectiveDate Then
                CrossDirection = Common.Helper.EnumsHelper.CrossDirectionEnum.NONE
            ElseIf OldEffectiveDate >= KeyStartDate AndAlso NewEffectiveDate < KeyStartDate Then
                CrossDirection = Common.Helper.EnumsHelper.CrossDirectionEnum.BACK
            ElseIf OldEffectiveDate < KeyStartDate AndAlso NewEffectiveDate >= KeyStartDate Then
                CrossDirection = Common.Helper.EnumsHelper.CrossDirectionEnum.FORWARD
            End If
            Return CrossDirection
        End Function


        'Added 6/7/2022 for task 74147 MLW
        Public Shared Function DateCrossedCheck(ByRef KeyStartDate As Date, ByRef CrossDirection As Common.Helper.EnumsHelper.CrossDirectionEnum, ByRef NewEffectiveDate As Date, ByRef OldEffectiveDate As Date) As Boolean
            CrossDirection = DateLineCrossed(KeyStartDate, NewEffectiveDate, OldEffectiveDate)
            If CrossDirection <> Common.Helper.EnumsHelper.CrossDirectionEnum.NONE Then
                Return True
            Else
                Return False
            End If
        End Function

        'Added 6/6/2022 for task 74147 MLW
        Public Shared Sub CheckDateCrossing(ByRef Quote As QuickQuoteObject, NewEffectiveDate As String, OldEffectiveDate As String, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing, Optional ByRef ReturnValues As DateCrossingReturnValues = Nothing)
            ' 06/13/2022 CAH - New Business uses EffectiveDate, Endorsements use Version ID
            ReturnValues = New DateCrossingReturnValues
            Dim chc = New CommonHelperClass
            Dim CrossDirection As Common.Helper.EnumsHelper.CrossDirectionEnum = Common.Helper.EnumsHelper.CrossDirectionEnum.NONE
            Dim QQHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
            If Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote Then
                If QQHelper.IsValidDateString(NewEffectiveDate) AndAlso QQHelper.IsValidDateString(OldEffectiveDate) Then
                    Dim ConvertedNewDate As Date
                    Dim ConvertedOldDate As Date
                    Dim ValidNewEffDate = Date.TryParse(NewEffectiveDate, ConvertedNewDate)
                    Dim ValidOldEffDate = Date.TryParse(OldEffectiveDate, ConvertedOldDate)
                    If Quote IsNot Nothing AndAlso ValidNewEffDate AndAlso ValidOldEffDate Then
                        Select Case Quote.LobType
                            Case QuickQuoteObject.QuickQuoteLobType.HomePersonal
                                'Home Plus Enhancement Endorsement 1020, Water Backup
                                If HOM_General.HPEEWaterBUEnabled() Then
                                    If DateCrossedCheck(HOM_General.HPEEWaterBUEffDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then WaterBackupDateCrossing.UpdateHPEE1020WaterBackup(Quote, CrossDirection)
                                End If
                                'Structure Type Manufactured
                                Dim structureTypeManufactured As NewFlagItem = New NewFlagItem("VR_HOM_StructureTypeManufactured_Settings")
                                If structureTypeManufactured.EnabledFlag AndAlso DateCrossedCheck(structureTypeManufactured.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then HOM.StructureTypeManufactureHelper.UpdateStructureType(Quote, CrossDirection, ValidationErrors)

                                If PolicyDeductibleNotLessThan1kHelper.PolicyDeductibleNotLessThan1kEnabled() AndAlso DateCrossedCheck(PolicyDeductibleNotLessThan1kHelper.PolicyDeductibleNotLessThan1kSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then PolicyDeductibleNotLessThan1kHelper.UpdatePolicyDeductibleNotLessThan1k(Quote, CrossDirection, ValidationErrors)

                            Case QuickQuoteObject.QuickQuoteLobType.Farm
                                'Start - Canine Exclusion
                                Dim CanineSettings As NewFlagItem = New NewFlagItem("VR_Far_Canine_Settings")
                                If CanineSettings.EnabledFlag AndAlso DateCrossedCheck(CanineSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then CanineExclusionHelper.UpdateCanineExclusion(Quote, CrossDirection, ValidationErrors)
                                'End - Canine Exlusion

                                'Added 6/14/2022 for task 72947 MLW

                                Dim qqh As New QuickQuoteHelperClass
                                Dim SubQuoteFirst = qqh.MultiStateQuickQuoteObjects(Quote).GetItemAtIndex(0)
                                If SubQuoteFirst.ProgramTypeId = "6" Then
                                    'Start - Woodburning Stove Num of Units
                                    Dim WoodburningSettings As NewFlagItem = New NewFlagItem("VR_Far_WoodburningUnits_Settings")
                                    If WoodburningSettings.EnabledFlag AndAlso DateCrossedCheck(WoodburningStoveHelper.WoodburningNumOfUnitsEffDate(), CrossDirection, ConvertedNewDate, ConvertedOldDate) Then WoodburningStoveHelper.UpdateWoodburningStove(Quote, CrossDirection, ValidationErrors)
                                    'End - Woodburning Stove Num of Units

                                    'Start - Swimming Pool Num of Units
                                    Dim SwimmingPoolSettings As NewFlagItem = New NewFlagItem("VR_Far_SwimmingPoolUnits_Settings")
                                    If SwimmingPoolSettings.EnabledFlag AndAlso DateCrossedCheck(SwimmingPoolSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then SwimmingPoolUnitsHelper.UpdateSwimmingPoolUnits(Quote, CrossDirection, ValidationErrors)
                                    'End  - Swimming Pool Num of Units

                                    'Start - Trampoline Num of Units
                                    Dim TrampolineSettings As NewFlagItem = New NewFlagItem("VR_Far_TrampolineUnits_Settings")
                                    If TrampolineSettings.EnabledFlag AndAlso DateCrossedCheck(TrampolineSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then TrampolineUnitsHelper.UpdateTrampolineUnits(Quote, CrossDirection, ValidationErrors)
                                    'End  - Trampoline Num of Units

                                    'Start - Underground Num of Units
                                    Dim UndergroundServiceLineSettings As NewFlagItem = New NewFlagItem("VR_Far_UndergroundServiceLine_Settings")
                                    If UndergroundServiceLineSettings.EnabledFlag AndAlso DateCrossedCheck(UndergroundServiceLineSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then UndergroundServiceLineHelper.UpdateUndergroundServiceLine(Quote, CrossDirection, ValidationErrors)
                                    'End  - Underground  Num of Units
                                End If

                                'Added 6/21/2022 for task 71215 MLW
                                If PollutionLiability1MHelper.PollutionLiability1MEnabled() Then
                                    If DateCrossedCheck(PollutionLiability1MHelper.PollutionLiability1MEffDate(), CrossDirection, ConvertedNewDate, ConvertedOldDate) Then PollutionLiability1MHelper.UpdatePollutionLiability1M(Quote, CrossDirection, ValidationErrors)
                                End If

                                'Added 8/9/2022 for task 76033 MLW
                                If LiabilityEnhancement1MHelper.LiabilityEnhancement1MEnabled() Then
                                    If DateCrossedCheck(LiabilityEnhancement1MHelper.LiabilityEnhancement1MEffDate(), CrossDirection, ConvertedNewDate, ConvertedOldDate) Then LiabilityEnhancement1MHelper.UpdateLiabilityEnhancement1M(Quote, CrossDirection, ValidationErrors)
                                End If

                                If FarmExtenderHelper.FarmExtenderEnabled() AndAlso DateCrossedCheck(FarmExtenderHelper.FarmExtenderSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then FarmExtenderHelper.UpdateFarmExtender(Quote, CrossDirection, ValidationErrors)

                                If FarmAllStarHelper.FarmAllStarEnabled() AndAlso DateCrossedCheck(FarmAllStarHelper.FarmAllStarSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then FarmAllStarHelper.UpdateFarmAllStar(Quote, CrossDirection, ValidationErrors)

                                If CosmeticDamageExHelper.CosmeticDamageExEnabled() AndAlso DateCrossedCheck(CosmeticDamageExHelper.CosmeticDamageExSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then CosmeticDamageExHelper.UpdateCosmeticDamageEx(Quote, CrossDirection, ValidationErrors)

                                If CosDamHiddenHelper.CosmeticDamageHiddenEnabled() AndAlso DateCrossedCheck(CosDamHiddenHelper.CosmeticDamageHiddenSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then CosDamHiddenHelper.UpdateCosmeticDamageHidden(Quote, CrossDirection, ValidationErrors)

                                If FarmBuildingtypeforbuildingHelper.FARMBuildingTypeForBuildingsEnabled() AndAlso DateCrossedCheck(FarmBuildingtypeforbuildingHelper.FARMBuildingTypeForBuildingsSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then FarmBuildingtypeforbuildingHelper.UpdateFARMBuildingTypeForBuildings(Quote, CrossDirection, ValidationErrors)


                            Case QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                                'Added 6/29/2022 for task 75037 MLW
                                Dim PlusEnhancementSettings As NewFlagItem = New NewFlagItem("VR_CPR_CPP_PlusEnhancementEndorsement_Settings")
                                If PlusEnhancementSettings.EnabledFlag AndAlso DateCrossedCheck(CPR.CPR_PropertyPlusEnhancementEndorsement.PropPlusEnhancementEffDate(), CrossDirection, ConvertedNewDate, ConvertedOldDate) Then CPR.CPR_PropertyPlusEnhancementEndorsement.UpdatePropPlusEnhancement(Quote, CrossDirection, ValidationErrors)
                                'Added 10/20/2022 for task 77527 MLW
                                Dim InflationGuardSettings As NewFlagItem = New NewFlagItem("VR_CPR_CPP_InflationGuardNo2_Settings")
                                If InflationGuardSettings.EnabledFlag AndAlso DateCrossedCheck(InflationGuardSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then CPR.CPR_InflationGuardHelper.UpdateInflationGuard(Quote, CrossDirection, ValidationErrors)

                                Dim CprNewCoId As NewFlagItem = New NewFlagItem("VR_NewCo_NewCompanyId_Settings")
                                Dim CprEarliestDate As Date = NewCompanyIdHelper.GetEarliestEffectiveDatePossible(Quote)
                                If CprNewCoId.EnabledFlag AndAlso DateCrossedCheck(CprEarliestDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then NewCompanyIdHelper.UpdateNewCoId(Quote, CrossDirection, ValidationErrors)

                                If CPRRemovePropDedBelow1k.RemovePropDedBelow1kEnabled() AndAlso DateCrossedCheck(CPRRemovePropDedBelow1k.RemovePropDedBelow1kSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then CPRRemovePropDedBelow1k.UpdatePropertyDeductible(Quote, CrossDirection, ValidationErrors)

                                If FunctionalReplacementCostHelper.FunctionalReplacementCostEnabled() AndAlso DateCrossedCheck(FunctionalReplacementCostHelper.FunctionalReplacementCostSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then FunctionalReplacementCostHelper.UpdateFunctionalReplacementCost(Quote, CrossDirection, ValidationErrors)

                                If ValuationACVHelper.ValuationACVEnabled() AndAlso DateCrossedCheck(ValuationACVHelper.ValuationACVSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then ValuationACVHelper.UpdateValuationACV(Quote, CrossDirection, ValidationErrors)

                                If OwnerOccupiedPercentageFieldHelper.OwnerOccupiedPercentageFieldEnabled() AndAlso DateCrossedCheck(OwnerOccupiedPercentageFieldHelper.OwnerOccupiedPercentageFieldSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then OwnerOccupiedPercentageFieldHelper.UpdateOwnerOccupiedPercentageField(Quote, CrossDirection, ValidationErrors)

                                If RemoveBuildingLevelDeductibleHelper.RemoveBuildingLevelDeductibleEnabled() AndAlso DateCrossedCheck(RemoveBuildingLevelDeductibleHelper.RemoveBuildingLevelDeductibleSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then RemoveBuildingLevelDeductibleHelper.UpdateRemoveBuildingLevelDeductible(Quote, CrossDirection, ValidationErrors)

                                If LocationWindHailHelper.LocationWindHailEnabled() AndAlso DateCrossedCheck(LocationWindHailHelper.LocationWindHailSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then LocationWindHailHelper.UpdateLocationWindHail(Quote, CrossDirection, ValidationErrors)

                                If WindHailDefaultingHelper.WindHailDefaultingEnabled() AndAlso DateCrossedCheck(WindHailDefaultingHelper.WindHailDefaultingSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then WindHailDefaultingHelper.UpdateWindHailDefaulting(Quote, CrossDirection, ValidationErrors)

                            Case QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
                                Dim ThreeMillOption As NewFlagItem = New NewFlagItem("VR_CGL_CPP_3M_Option_Settings")
                                If ThreeMillOption.EnabledFlag AndAlso DateCrossedCheck(ThreeMillOption.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then CGL.GenAggProducts3MHelper.UpdateThreeMillOptionStove(Quote, CrossDirection, ValidationErrors)

                                Dim GlPlusEnhancementEndorsement As NewFlagItem = New NewFlagItem("VR_CGL_GenLiabPlusEnhancementEndorsement_Settings")
                                If GlPlusEnhancementEndorsement.EnabledFlag AndAlso DateCrossedCheck(GlPlusEnhancementEndorsement.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then CGL.CGL_GenLiabPlusEnhancementEndorsement.UpdateGlPlusEnhancement(Quote, CrossDirection, ValidationErrors)

                                Dim CglNewCoId As NewFlagItem = New NewFlagItem("VR_NewCo_NewCompanyId_Settings")
                                Dim CglEarliestDate As Date = NewCompanyIdHelper.GetEarliestEffectiveDatePossible(Quote)
                                If CglNewCoId.EnabledFlag AndAlso DateCrossedCheck(CglEarliestDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then NewCompanyIdHelper.UpdateNewCoId(Quote, CrossDirection, ValidationErrors)

                                If ClassCodeAssignmentHelper.ClassCodeAssignmentEnabled() AndAlso DateCrossedCheck(ClassCodeAssignmentHelper.ClassCodeAssignmentSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then ClassCodeAssignmentHelper.UpdateClassCodeAssignment(Quote, CrossDirection, ValidationErrors)

                            Case QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                                Dim ThreeMillOption As NewFlagItem = New NewFlagItem("VR_CGL_CPP_3M_Option_Settings")
                                If ThreeMillOption.EnabledFlag AndAlso DateCrossedCheck(ThreeMillOption.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then CGL.GenAggProducts3MHelper.UpdateThreeMillOptionStove(Quote, CrossDirection, ValidationErrors)

                                Dim PlusEnhancementSettings As NewFlagItem = New NewFlagItem("VR_CPR_CPP_PlusEnhancementEndorsement_Settings")
                                If PlusEnhancementSettings.EnabledFlag AndAlso DateCrossedCheck(CPR.CPR_PropertyPlusEnhancementEndorsement.PropPlusEnhancementEffDate(), CrossDirection, ConvertedNewDate, ConvertedOldDate) Then CPR.CPR_PropertyPlusEnhancementEndorsement.UpdatePropPlusEnhancement(Quote, CrossDirection, ValidationErrors)

                                Dim GlPlusEnhancementEndorsement As NewFlagItem = New NewFlagItem("VR_CGL_GenLiabPlusEnhancementEndorsement_Settings")
                                If GlPlusEnhancementEndorsement.EnabledFlag AndAlso DateCrossedCheck(GlPlusEnhancementEndorsement.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then CGL.CGL_GenLiabPlusEnhancementEndorsement.UpdateGlPlusEnhancement(Quote, CrossDirection, ValidationErrors)

                                Dim CppNewCoId As NewFlagItem = New NewFlagItem("VR_NewCo_NewCompanyId_Settings")
                                Dim CppEarliestDate As Date = NewCompanyIdHelper.GetEarliestEffectiveDatePossible(Quote)
                                If CppNewCoId.EnabledFlag AndAlso DateCrossedCheck(CppEarliestDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then NewCompanyIdHelper.UpdateNewCoId(Quote, CrossDirection, ValidationErrors)


                                'Added 10/20/2022 for task 77527 MLW
                                Dim InflationGuardSettings As NewFlagItem = New NewFlagItem("VR_CPR_CPP_InflationGuardNo2_Settings")
                                If InflationGuardSettings.EnabledFlag AndAlso DateCrossedCheck(InflationGuardSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then CPR.CPR_InflationGuardHelper.UpdateInflationGuard(Quote, CrossDirection, ValidationErrors)

                                If CPRRemovePropDedBelow1k.RemovePropDedBelow1kEnabled() AndAlso DateCrossedCheck(CPRRemovePropDedBelow1k.RemovePropDedBelow1kSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then CPRRemovePropDedBelow1k.UpdatePropertyDeductible(Quote, CrossDirection, ValidationErrors)

                                If FunctionalReplacementCostHelper.FunctionalReplacementCostEnabled() AndAlso DateCrossedCheck(FunctionalReplacementCostHelper.FunctionalReplacementCostSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then FunctionalReplacementCostHelper.UpdateFunctionalReplacementCost(Quote, CrossDirection, ValidationErrors)

                                If OwnerOccupiedPercentageFieldHelper.OwnerOccupiedPercentageFieldEnabled() AndAlso DateCrossedCheck(OwnerOccupiedPercentageFieldHelper.OwnerOccupiedPercentageFieldSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then OwnerOccupiedPercentageFieldHelper.UpdateOwnerOccupiedPercentageField(Quote, CrossDirection, ValidationErrors)

                                If ClassCodeAssignmentHelper.ClassCodeAssignmentEnabled() AndAlso DateCrossedCheck(ClassCodeAssignmentHelper.ClassCodeAssignmentSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then ClassCodeAssignmentHelper.UpdateClassCodeAssignment(Quote, CrossDirection, ValidationErrors)

                                If ValuationACVHelper.ValuationACVEnabled() AndAlso DateCrossedCheck(ValuationACVHelper.ValuationACVSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then ValuationACVHelper.UpdateValuationACV(Quote, CrossDirection, ValidationErrors)

                                If RemoveBuildingLevelDeductibleHelper.RemoveBuildingLevelDeductibleEnabled() AndAlso DateCrossedCheck(RemoveBuildingLevelDeductibleHelper.RemoveBuildingLevelDeductibleSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then RemoveBuildingLevelDeductibleHelper.UpdateRemoveBuildingLevelDeductible(Quote, CrossDirection, ValidationErrors)

                                If LocationWindHailHelper.LocationWindHailEnabled() AndAlso DateCrossedCheck(LocationWindHailHelper.LocationWindHailSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then LocationWindHailHelper.UpdateLocationWindHail(Quote, CrossDirection, ValidationErrors)

                                If WindHailDefaultingHelper.WindHailDefaultingEnabled() AndAlso DateCrossedCheck(WindHailDefaultingHelper.WindHailDefaultingSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then WindHailDefaultingHelper.UpdateWindHailDefaulting(Quote, CrossDirection, ValidationErrors)

                                If UnScheduledMotorTruckCargoHelper.UnScheduledMotorTruckCargoEnabled() AndAlso DateCrossedCheck(UnScheduledMotorTruckCargoHelper.UnScheduledMotorTruckCargoSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then UnScheduledMotorTruckCargoHelper.UpdateUnScheduledMotorTruckCargo(Quote, CrossDirection, ValidationErrors)

                            Case QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                                'Added 7/26/2022 for task 75911 MLW
                                Dim DFRStandaloneSettings As NewFlagItem = New NewFlagItem("VR_DFR_Standalone_Settings")
                                If DFRStandaloneSettings.EnabledFlag AndAlso DateCrossedCheck(DFRStandaloneSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then
                                    DFRStandaloneHelper.UpdateDFRStandaloneQuestion(Quote, CrossDirection, ValidationErrors)
                                    ReturnValues.DFRStandaloneDateCrossed = True
                                End If

                                If PolicyDeductibleNotLessThan1k.PolicyDeductibleNotLessThan1kEnabled() AndAlso DateCrossedCheck(PolicyDeductibleNotLessThan1k.PolicyDeductibleNotLessThan1kSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then PolicyDeductibleNotLessThan1k.UpdatePolicyDeductibleNotLessThan1k(Quote, CrossDirection, ValidationErrors)

                            Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                                Dim ILCollisionAndUMPDSettings As NewFlagItem = New NewFlagItem("VR_PPA_ILCollisionAndUMPD_Settings")
                                If ILCollisionAndUMPDSettings.EnabledFlag AndAlso DateCrossedCheck(ILCollisionAndUMPDSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then PPA.CollisionAndUMPD.UpdateILCollisionAndUMPD(Quote, CrossDirection, ValidationErrors)

                                If UMPDLimitsHelper.UMPDLimitsEnabled() AndAlso DateCrossedCheck(UMPDLimitsHelper.UMPDLimitsSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then UMPDLimitsHelper.UpdateUMPDLimits(Quote, CrossDirection, ValidationErrors)

                                Dim TransportationExpenseSettings As NewFlagItem = New NewFlagItem("VR_PPA_TransportationExpense_Settings")
                                If TransportationExpenseSettings.EnabledFlag AndAlso DateCrossedCheck(TransportationExpenseHelper.TransportationExpenseEffDate(), CrossDirection, ConvertedNewDate, ConvertedOldDate) Then TransportationExpenseHelper.UpdateTransportationExpense(Quote, CrossDirection, ValidationErrors)

                                If NewRAPASymbolsHelper.NewRAPASymbolsEnabled() AndAlso DateCrossedCheck(NewRAPASymbolsHelper.NewRAPASymbolsSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then NewRAPASymbolsHelper.UpdateNewRAPASymbols(Quote, CrossDirection, ValidationErrors)
                            Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                                If CAP_UMPDLimitsHelper.UMPDLimitsEnabled() AndAlso DateCrossedCheck(CAP_UMPDLimitsHelper.UMPDLimitsSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then CAP_UMPDLimitsHelper.UpdateUMPDLimits(Quote, CrossDirection, ValidationErrors)
                                If UM_UIM_UMPDHelper.UM_UIM_UMPDEnabled() AndAlso DateCrossedCheck(UM_UIM_UMPDHelper.UM_UIM_UMPDSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then
                                    UM_UIM_UMPDHelper.UpdateUM_UIM_UMPD(Quote, CrossDirection, ValidationErrors)
                                    ReturnValues.CAPUMUIMUMPDDateCrossed = True
                                End If
                                If CompCollDeductibleHelper.CompCollDeductibleEnabled() AndAlso DateCrossedCheck(CompCollDeductibleHelper.CompCollDeductibleSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then CompCollDeductibleHelper.UpdateCompCollDeductible(Quote, CrossDirection, ValidationErrors)
                                If RACASymbolHelper.RACASymbolsEnabled() AndAlso DateCrossedCheck(RACASymbolHelper.RACASymbolsSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then
                                    RACASymbolHelper.UpdateRACASymbols(Quote, CrossDirection, ValidationErrors)
                                    ReturnValues.RACASymbolsDateCrossed = True
                                End If
                            Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                                Dim BopSTP As NewFlagItem = New NewFlagItem("VR_BOP_BopStpUwQuestions_Settings")
                                If BopSTP.EnabledFlag AndAlso DateCrossedCheck(BopSTP.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then BopStpUwQuestionsHelper.UpdateBopStpUwQuestions(Quote, CrossDirection, ValidationErrors)
                                Dim BopNewCoId As NewFlagItem = New NewFlagItem("VR_NewCo_NewCompanyId_Settings")
                                Dim BopEarliestDate As Date = NewCompanyIdHelper.GetEarliestEffectiveDatePossible(Quote)
                                If BopNewCoId.EnabledFlag AndAlso DateCrossedCheck(BopEarliestDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then NewCompanyIdHelper.UpdateNewCoId(Quote, CrossDirection, ValidationErrors)
                                If RemovePropDedBelow1k.RemovePropDedBelow1kEnabled() AndAlso DateCrossedCheck(RemovePropDedBelow1k.RemovePropDedBelow1kSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then RemovePropDedBelow1k.UpdatePropertyDeductible(Quote, CrossDirection, ValidationErrors)
                                If OnePctWindHailLessorsRiskHelper.OnePctWindHailLessorsRiskEnabled() AndAlso DateCrossedCheck(OnePctWindHailLessorsRiskHelper.OnePctWindHailLessorsRiskSettings.StartDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then OnePctWindHailLessorsRiskHelper.UpdateOnePctWindHailLessorsRisk(Quote, CrossDirection, ValidationErrors)
                            Case Else
                                'do nothing
                        End Select

                        '------------------- All Lines -------------------
                        'Start New Company ID
                        'Dim NewCoId As NewFlagItem = New NewFlagItem("VR_NewCo_NewCompanyId_Settings")
                        'Dim EarliestDate As Date = NewCompanyIdHelper.GetEarliestEffectiveDatePossible(Quote)
                        'If NewCompanyIdHelper.IsNewCompanyIdAvailable(Quote) AndAlso DateCrossedCheck(EarliestDate, CrossDirection, ConvertedNewDate, ConvertedOldDate) Then NewCompanyIdHelper.UpdateNewCoId(Quote, CrossDirection, ValidationErrors)
                        'End New Company Id
                        ' ---- Is Available will not work because it will use the Quote Effective date and not the intended
                        ' ---- New effective date
                    End If
                End If
            End If
        End Sub

        Public Shared Function doesRequireAPolicyHolderRestart(Quote As QuickQuoteObject) As Boolean
            If Not String.IsNullOrWhiteSpace(Quote?.Database_QuickQuote_Updated) Then
                Dim convertedDate As Date = Date.MaxValue
                If IsDate(Quote?.Database_QuickQuote_Updated) Then
                    convertedDate = CDate(Quote?.Database_QuickQuote_Updated)
                Else
                    Return False
                End If
                If convertedDate < Date.Now.AddDays(-DaysAgoForPolicyToRestartAtPolicyHolder) Then
                    Return True
                End If
            End If

            Return False
        End Function
    End Class

    'Added 8/2/2022 for task 75911 MLW
    Public Class DateCrossingReturnValues
        Public Property DFRStandaloneDateCrossed As Boolean = False
        Public Property CAPUMUIMUMPDDateCrossed As Boolean = False
        Public Property RACASymbolsDateCrossed As Boolean = False
    End Class

    'Public Class EffectiveDateHelper_FarmPollutionAndFarmEnhancement

    '    Public Shared Function GetStartDate() As String
    '        Return ConfigurationManager.AppSettings("VR_FarmPollutionLiabilityUpdate_EffectiveDate")
    '    End Function

    '    Public Shared Sub CheckUpdatedFarmPollutionAndFarmEnhancement(Quote As QuickQuoteObject, NewQuoteEffectiveDate As String, Page As ctlFarmPolicyCoverages)
    '        If EffectiveDateHelper.isQuoteEffectiveDatePastDate(NewQuoteEffectiveDate, GetStartDate()) = False Then
    '            Dim LocationNumList As New List(Of Integer)
    '            Dim AlreadyShowingMessage As Boolean = False
    '            If WebHelper_Personal.QuoteHasSectionIICoverages(Quote, LocationNumList) Then
    '                For Each locNum As Integer In LocationNumList
    '                    Dim LocIndex As Integer = locNum - 1
    '                    Dim farmCoverage As QuickQuoteSectionIICoverage
    '                    If Quote.Policyholder.Name.TypeId = "2" Then
    '                        'Commercial
    '                        farmCoverage = Quote.Locations(LocIndex).SectionIICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.EnhancementEndorsement)
    '                    Else
    '                        'Personal
    '                        farmCoverage = Quote.Locations(LocIndex).SectionIICoverages.Find(Function(p) p.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.OptionalLiability_FarmPollutionLiability)
    '                    End If
    '                    If farmCoverage IsNot Nothing AndAlso (farmCoverage.IncreasedLimitId = "54" OrElse farmCoverage.IncreasedLimitId = "390") Then
    '                        If AlreadyShowingMessage = False Then
    '                            Dim message As String = ""
    '                            If Quote.LiabilityOptionId = 1 Then
    '                                message += "Limited Farm Pollution - "
    '                            Else
    '                                message += "Liability Enhancement Endorsement - "
    '                            End If
    '                            message += "The values '250,000' and '500,000' can only be selected with an effective date on or after " + CDate(GetStartDate()).ToShortDateString() + ". The value has been switched to '100,000'."
    '                            'Dim valHelper As VRControlBase
    '                            Page.ValidationHelper.AddWarning(message)
    '                        End If

    '                        Dim newFarmPollutionDropdown As DropDownList = Page.DDLFarmPollutionLimit
    '                        EffectiveDateHelper_FarmPollutionAndFarmEnhancement.RemoveValuesFromDropDown(newFarmPollutionDropdown)
    '                        newFarmPollutionDropdown.SelectedIndex = (newFarmPollutionDropdown.Items.Count - 1)

    '                        Dim QQHelper As New QuickQuoteHelperClass
    '                        Dim increasedLimit = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.IncreasedLimitId, newFarmPollutionDropdown.SelectedValue, Quote.LobType)
    '                        farmCoverage.IncreasedLimitId = newFarmPollutionDropdown.SelectedValue
    '                        farmCoverage.TotalLimit = Integer.Parse(increasedLimit.Replace(",", "")) 'Since we show the total amount in the dropdown, lets just use that
    '                    End If
    '                Next
    '            End If
    '        End If
    '    End Sub

    '    Public Shared Sub RemoveValuesFromDropDown(ddl As DropDownList)
    '        Dim ItemsToRemove As New List(Of ListItem)
    '        For Each item As ListItem In ddl.Items
    '            If item.Value = "54" OrElse item.Value = "390" Then
    '                ItemsToRemove.Add(item)
    '            End If
    '        Next
    '        If ItemsToRemove IsNot Nothing AndAlso ItemsToRemove.Count > 0 Then
    '            For Each item As ListItem In ItemsToRemove
    '                ddl.Items.Remove(item)
    '            Next
    '        End If
    '    End Sub

    'End Class
End Namespace
