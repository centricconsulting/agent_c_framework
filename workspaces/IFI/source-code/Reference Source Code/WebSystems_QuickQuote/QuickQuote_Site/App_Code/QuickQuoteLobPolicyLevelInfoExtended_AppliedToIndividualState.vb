Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' object used to store policy-level lob-specific information (that applies to individual states) for a quote; also includes properties that were previously on QuickQuote only
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()>
    Public Class QuickQuoteLobPolicyLevelInfoExtended_AppliedToIndividualState 'added 8/17/2018
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        'PolicyLevel
        Private _AdditionalInsuredsCount As Integer
        Private _AdditionalInsuredsCheckboxBOP As List(Of QuickQuoteAdditionalInsured)
        Private _HasAdditionalInsuredsCheckboxBOP As Boolean
        Private _AdditionalInsuredsManualCharge As String
        Private _AdditionalInsuredsQuotedPremium As String
        Private _AdditionalInsureds As Generic.List(Of QuickQuoteAdditionalInsured)
        Private _AdditionalInsuredsBackup As List(Of QuickQuoteAdditionalInsured)
        Private _HasExclusionOfAmishWorkers As Boolean
        Private _HasExclusionOfSoleProprietorsPartnersOfficersAndOthers As Boolean
        'Private _HasInclusionOfSoleProprietorsPartnersOfficersAndOthers As Boolean
        'Private _HasWaiverOfSubrogation As Boolean
        'Private _WaiverOfSubrogationNumberOfWaivers As Integer
        'Private _WaiverOfSubrogationPremium As String
        'Private _WaiverOfSubrogationPremiumId As String
        'Private _NeedsToUpdateWaiverOfSubrogationPremiumId As Boolean
        Private _ExclusionOfAmishWorkerRecords As Generic.List(Of QuickQuoteExclusionOfAmishWorkerRecord)
        Private _ExclusionOfSoleProprietorRecords As Generic.List(Of QuickQuoteExclusionOfSoleProprietorRecord)
        Private _LegalEntityTypeQuotedPremium As String
        Private _LegalEntityType As TriState
        'Private _InclusionOfSoleProprietorRecords As Generic.List(Of QuickQuoteInclusionOfSoleProprietorRecord)
        'Private _WaiverOfSubrogationRecords As Generic.List(Of QuickQuoteWaiverOfSubrogationRecord)
        Private _ExclusionOfAmishWorkerRecordsBackup As Generic.List(Of QuickQuoteExclusionOfAmishWorkerRecord)
        Private _ExclusionOfSoleProprietorRecordsBackup As Generic.List(Of QuickQuoteExclusionOfSoleProprietorRecord)
        'Private _InclusionOfSoleProprietorRecordsBackup As Generic.List(Of QuickQuoteInclusionOfSoleProprietorRecord)
        'Private _WaiverOfSubrogationRecordsBackup As Generic.List(Of QuickQuoteWaiverOfSubrogationRecord)
        Private _Dec_BOP_OptCovs_Premium As String
        Private _ExpModQuotedPremium As String
        Private _ScheduleModQuotedPremium As String
        Private _TerrorismQuotedPremium As String
        Private _PremDiscountQuotedPremium As String
        Private _MinimumQuotedPremium As String
        Private _MinimumPremiumAdjustment As String
        Private _TotalEstimatedPlanPremium As String
        Private _SecondInjuryFundQuotedPremium As String
        Private _Dec_LossConstantPremium As String
        Private _Dec_ExpenseConstantPremium As String
        Private _Dec_WC_TotalPremiumDue As String
        Private _Dec_GL_OptCovs_Premium As String
        Private _Dec_CAP_OptCovs_Premium As String
        Private _Dec_CAP_OptCovs_Premium_Without_GarageKeepers As String
        Private _HasConvertedCoverages As Boolean
        Private _HasConvertedInclusionsExclusions As Boolean
        Private _HasConvertedModifiers As Boolean
        Private _HasConvertedScheduledRatings As Boolean
        Private _CanUseExclusionNumForExclusionReconciliation As Boolean
        Private _CanUseLossHistoryNumForLossHistoryReconciliation As Boolean
        Private _CanUsePolicyUnderwritingNumForPolicyUnderwritingReconciliation As Boolean
        Private _HasConvertedScheduledCoverages As Boolean
        Private _CanUseScheduledCoverageNumForScheduledCoverageReconciliation As Boolean
        Private _CanUseClassificationCodeNumForClassificationCodeReconciliation As Boolean 'for reconciliation
        Private _HasConvertedFarmIncidentalLimitCoverages As Boolean
        Private _HasConvertedScheduledPersonalPropertyCoverages As Boolean
        Private _HasConvertedUnscheduledPersonalPropertyCoverages As Boolean
        Private _CanUseScheduledFarmPersonalPropertyNumForScheduledPersonalPropertyReconciliation As Boolean
        Private _CanUseUnscheduledFarmPersonalPropertyNumForUnscheduledPersonalPropertyReconciliation As Boolean
        Private _HasConvertedOptionalCoverages As Boolean
        Private _CanUseOptionalCoveragesNumForOptionalCoverageReconciliation As Boolean
        Private _CanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean
        Private _CPP_MinPremAdj_CPR As String 'covCodeId 10121; note: covCodeId good for CGL, PIM (not in VR yet), FAR, CAP, WCP, GAR, BOP, CRM, CPR, CIM
        Private _CPP_MinPremAdj_CGL As String 'covCodeId 10121; note: covCodeId good for CGL, PIM (not in VR yet), FAR, CAP, WCP, GAR, BOP, CRM, CPR, CIM
        Private _CPP_MinPremAdj_CIM As String 'covCodeId 10121; note: covCodeId good for CGL, PIM (not in VR yet), FAR, CAP, WCP, GAR, BOP, CRM, CPR, CIM
        Private _CPP_MinPremAdj_CRM As String 'covCodeId 10121; note: covCodeId good for CGL, PIM (not in VR yet), FAR, CAP, WCP, GAR, BOP, CRM, CPR, CIM
        Private _CPP_MinPremAdj_GAR As String 'covCodeId 10121; note: covCodeId good for CGL, PIM (not in VR yet), FAR, CAP, WCP, GAR, BOP, CRM, CPR, CIM
        Private _CAP_GAR_PolicyLevelCovs_Premium As String
        'Private _WCP_WaiverPremium As String 'covCodeId 10124 CovAddInfo w/ "Waiver Premium" in desc
        'added 10/15/2018 for IL (similar to existing props w/o IL, but different form # and typeId)
        Private _HasExclusionOfSoleProprietorsPartnersOfficersAndOthers_IL As Boolean
        Private _ExclusionOfSoleProprietorRecords_IL As List(Of QuickQuoteExclusionOfSoleProprietorRecord_IL)
        Private _ExclusionOfSoleProprietorRecordsBackup_IL As List(Of QuickQuoteExclusionOfSoleProprietorRecord_IL)
        'added 4/26/2019 for KY
        Private _HasKentuckyRejectionOfCoverageEndorsement As Boolean
        Private _KentuckyRejectionOfCoverageEndorsementRecords As List(Of QuickQuoteKentuckyRejectionOfCoverageEndorsement)
        Private _KentuckyRejectionOfCoverageEndorsementRecordsBackup As List(Of QuickQuoteKentuckyRejectionOfCoverageEndorsement)

        'added 10/19/2018 - moved from AlliedToAllStates object
        Private _HasLiquorLiability As Boolean
        Private _LiquorLiabilityClassCodeTypeId As String '12 = 58161 - Restaurant Includes Package Sales, 13 = 59211 - Package Sales for Consumption Off Premises
        Private _LiquorLiabilityAnnualGrossPackageSalesReceipts As String
        Private _LiquorLiabilityAnnualGrossAlcoholSalesReceipts As String
        Private _LiquorLiabilityOccurrenceLimit As String
        Private _LiquorLiabilityOccurrenceLimitId As String
        Private _LiquorLiabilityClassification As String
        Private _LiquorLiabilityClassificationId As String
        Private _LiquorSales As String
        Private _LiquorLiabilityQuotedPremium As String
        '12/5/2018 - moved the rest of the professional liability props stuff
        Private _HasBarbersProfessionalLiability As Boolean
        Private _BarbersProfessionalLiabiltyQuotedPremium As String
        Private _BarbersProfessionalLiabilityFullTimeEmpNum As String
        Private _BarbersProfessionalLiabilityPartTimeEmpNum As String
        Private _BarbersProfessionalLiabilityDescription As String
        Private _HasBeauticiansProfessionalLiability As Boolean
        Private _BeauticiansProfessionalLiabilityQuotedPremium As String
        Private _BeauticiansProfessionalLiabilityFullTimeEmpNum As String
        Private _BeauticiansProfessionalLiabilityPartTimeEmpNum As String
        Private _BeauticiansProfessionalLiabilityDescription As String
        Private _HasFuneralDirectorsProfessionalLiability As Boolean
        Private _FuneralDirectorsProfessionalLiabilityQuotedPremium As String
        Private _FuneralDirectorsProfessionalLiabilityEmpNum As String
        Private _HasPrintersProfessionalLiability As Boolean
        Private _PrintersProfessionalLiabilityQuotedPremium As String
        Private _PrintersProfessionalLiabilityLocNum As String
        Private _HasSelfStorageFacility As Boolean
        Private _SelfStorageFacilityQuotedPremium As String
        Private _SelfStorageFacilityLimit As String
        Private _HasVeterinariansProfessionalLiability As Boolean
        Private _VeterinariansProfessionalLiabilityEmpNum As String
        Private _VeterinariansProfessionalLiabilityQuotedPremium As String
        Private _HasPharmacistProfessionalLiability As Boolean
        Private _PharmacistAnnualGrossSales As String
        Private _PharmacistQuotedPremium As String
        Private _HasOpticalAndHearingAidProfessionalLiability As Boolean
        Private _OpticalAndHearingAidProfessionalLiabilityEmpNum As String
        Private _OpticalAndHearingAidProfessionalLiabilityQuotedPremium As String
        Private _HasMotelCoverage As Boolean
        Private _MotelCoveragePerGuestLimitId As String
        Private _MotelCoveragePerGuestLimit As String
        Private _MotelCoveragePerGuestQuotedPremium As String
        Private _MotelCoverageSafeDepositLimitId As String
        Private _MotelCoverageSafeDepositDeductibleId As String
        Private _MotelCoverageSafeDepositLimit As String
        Private _MotelCoverageSafeDepositDeductible As String
        Private _MotelCoverageQuotedPremium As String
        Private _MotelCoverageSafeDepositQuotedPremium As String
        Private _HasPhotographyCoverage As Boolean
        Private _HasPhotographyCoverageScheduledCoverages As Boolean
        Private _PhotographyScheduledCoverages As List(Of QuickQuoteCoverage)
        Private _HasPhotographyMakeupAndHair As Boolean
        Private _PhotographyMakeupAndHairQuotedPremium As String
        Private _PhotographyCoverageQuotedPremium As String
        Private _HasResidentialCleaning As Boolean
        Private _ResidentialCleaningQuotedPremium As String
        Private _ProfessionalLiabilityCemetaryNumberOfBurials As String
        Private _ProfessionalLiabilityCemetaryQuotedPremium As String
        Private _ProfessionalLiabilityFuneralDirectorsNumberOfBodies As String
        Private _ProfessionalLiabilityPastoralNumberOfClergy As String
        Private _ProfessionalLiabilityPastoralQuotedPremium As String
        '12/10/2018 - moved the remaining professional liability props that were missed 12/5/2018
        Private _HasApartmentBuildings As Boolean
        Private _NumberOfLocationsWithApartments As String
        Private _ApartmentQuotedPremium As String
        Private _HasRestaurantEndorsement As Boolean
        Private _RestaurantQuotedPremium As String
        'added 12/18/2018 - moved from GoverningState object since they're specific to Location Buildings (and their parent state quotes)
        Private _ComputerCoinsuranceTypeId As String 'cov also has CoverageBasisTypeId set to 1
        Private _ComputerExcludeEarthquake As Boolean
        Private _ComputerValuationMethodTypeId As String 'static data
        Private _ComputerAdditionalInterests As List(Of QuickQuoteAdditionalInterest)
        Private _ComputerCanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean
        Private _ComputerQuotedPremium As String
        Private _ComputerAllPerilsDeductibleId As String 'cov also has CoverageBasisTypeId set to 1; may also need boolean prop for hasCoverage; static data
        Private _ComputerAllPerilsQuotedPremium As String
        Private _ComputerEarthquakeVolcanicEruptionDeductible As String 'cov also has CoverageBasisTypeId set to 1; example also has ApplyToWrittenPremiuim set to true; may also need boolean prop for hasCoverage
        Private _ComputerEarthquakeVolcanicEruptionQuotedPremium As String
        Private _ComputerMechanicalBreakdownDeductible As String 'cov also has CoverageBasisTypeId set to 1; example also has ApplyToWrittenPremiuim set to true; may also need boolean prop for hasCoverage
        Private _ComputerMechanicalBreakdownQuotedPremium As String
        Private _FineArtsDeductibleCategoryTypeId As String 'static data; note: cov also has CoverageBasisTypeId set to 1
        Private _FineArtsRate As String
        Private _FineArtsDeductibleId As String 'static data
        Private _FineArtsQuotedPremium As String
        Private _FineArtsAdditionalInterests As List(Of QuickQuoteAdditionalInterest)
        Private _FineArtsCanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean
        Private _FineArtsBreakageMarringOrScratching As Boolean 'renamed 3/17/2015 from _HasFineArtsBreakageMarringOrScratching; note: cov also has CoverageBasisTypeId set to 1
        Private _FineArtsBreakageMarringOrScratchingQuotedPremium As String
        Private _SignsAdditionalInterests As List(Of QuickQuoteAdditionalInterest) 'note: cov also has CoverageBasisTypeId set to 1
        Private _SignsCanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean
        Private _SignsMaximumDeductible As String 'CoverageDetail
        Private _SignsMinimumDeductible As String 'CoverageDetail
        Private _SignsValuationMethodTypeId As String 'CoverageDetail; static data
        Private _SignsDeductibleId As String 'static data
        Private _SignsQuotedPremium As String
        Private _SignsAnyOneLossCatastropheLimit As String 'note: cov also has CoverageBasisTypeId set to 1
        Private _SignsAnyOneLossCatastropheQuotedPremium As String

        Private _HasHerbicidePersticideApplicator As Boolean

        'added 10/24/2018
        Private _HasIllinoisContractorsHomeRepairAndRemodeling As Boolean
        Private _IllinoisContractorsHomeRepairAndRemodelingManualLimitAmount As String
        Private _IllinoisContractorsHomeRepairAndRemodelingQuotedPremium As String

        'added 11/28/2018 for WCP IL (included in total premium)
        Private _CatastropheOtherThanCertifiedActsOfTerrorismQuotedPremium As String

        'added 7/15/2019 for WCP KY
        Private _KentuckySpecialFundAssessmentQuotedPremium As String
        Private _WCP_KY_PremSurcharge As String

        'added 1/28/2020 for WCP IL Commission Operations Fund Surcharge
        Private _IL_WCP_CommissionOperationsFundSurcharge As String

        Private _QuoteEffectiveDate As String
        Private _QuoteTransactionType As QuickQuoteObject.QuickQuoteTransactionType
        Private _LobType As QuickQuoteObject.QuickQuoteLobType

        Private _BasePolicyLevelInfo As QuickQuoteLobPolicyLevelInfo_AppliedToIndividualState

        'added 8/8/2023 for CAP IL UMPD
        Private _UninsuredMotoristPropertyDamage_IL_QuotedPremium As String 'covCodeId 30015; may not be populated as premium is included in um/uim premium
        Private _UninsuredMotoristPropertyDamage_IL_LimitId As String 'covCodeId 30015
        Private _UninsuredMotoristPropertyDamage_IL_DeductibleId As String 'covCodeId 30015

        Private _LiquorManufacturersSales As String
        Private _LiquorRestaurantsSales As String
        Private _LiquorPackageStoresSales As String
        Private _LiquorClubsSales As String

        'PolicyLevel
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverages w/ coveragecode_ids 926, 21018, 501, 21022, 21019, 21023, 21020, 21053, 21054, 21055, 21024, 21025, 21026, 21016, 21017, or 21021</remarks>
        Public Property AdditionalInsuredsCount As Integer
            Get
                If QuickQuoteHelperClass.UseComparativeRaterForLob(lob:=_LobType, quoteTransactionType:=QuoteTransactionType) <> QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then 'using <> Yes to also allow on Maybe
                    Return qqHelper.AdditionalInsuredsCountFromList(_AdditionalInsureds)
                Else
                    Return _AdditionalInsuredsCount
                End If
            End Get
            Set(value As Integer)
                If QuickQuoteHelperClass.UseComparativeRaterForLob(lob:=_LobType, quoteTransactionType:=QuoteTransactionType) <> QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then 'using <> Yes to also allow on Maybe
                    qqHelper.UpdateAdditionalInsuredsListBasedOnCount(value, _AdditionalInsureds, additionalInsuredsBackup:=_AdditionalInsuredsBackup, updateBackupListBeforeRemoving:=True, effDate:=QuoteEffectiveDate, lobType:=_LobType, isAdditionalInsuredCheckboxBOP:=False)
                Else
                    _AdditionalInsuredsCount = value
                End If
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverages w/ coveragecode_ids 80371</remarks>
        Public Property HasAdditionalInsuredsCheckboxBOP As Boolean
            Get
                Return _HasAdditionalInsuredsCheckboxBOP
            End Get
            Set(value As Boolean)
                _HasAdditionalInsuredsCheckboxBOP = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverages w/ coveragecode_ids 80371</remarks>
        Public Property AdditionalInsuredsCheckboxBOP As List(Of QuickQuoteAdditionalInsured)
            Get
                Return _AdditionalInsuredsCheckboxBOP
            End Get
            Set(value As List(Of QuickQuoteAdditionalInsured))
                _AdditionalInsuredsCheckboxBOP = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverages w/ coveragecode_ids 80371</remarks>
        Public ReadOnly Property AdditionalInsuredsCheckboxBOPPremium As String
            Get
                Dim prem As Decimal = 0
                If AdditionalInsuredsCheckboxBOP IsNot Nothing AndAlso AdditionalInsuredsCheckboxBOP.Count > 0 Then
                    For Each ai As QuickQuoteAdditionalInsured In AdditionalInsuredsCheckboxBOP
                        If ai IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(ai.FullTermPremium) AndAlso IsNumeric(ai.FullTermPremium) AndAlso CDec(ai.FullTermPremium > 0) Then
                            prem += CDec(ai.FullTermPremium)
                        End If
                    Next
                End If
                Return prem.ToString()
            End Get
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverages w/ coveragecode_ids 926, 21018, 501, 21022, 21019, 21023, 21020, 21053, 21054, 21055, 21024, 21025, 21026, 21016, 21017, or 21021</remarks>
        Public Property AdditionalInsuredsManualCharge As String
            Get
                If QuickQuoteHelperClass.UseComparativeRaterForLob(lob:=_LobType, quoteTransactionType:=QuoteTransactionType) <> QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then 'using <> Yes to also allow on Maybe
                    Dim totalAddInsManChrg As String = qqHelper.AdditionalInsuredsTotalManualChargeFromList(_AdditionalInsureds)
                    If qqHelper.IsZeroAmount(totalAddInsManChrg) = True Then
                        totalAddInsManChrg = ""
                    End If
                    Return totalAddInsManChrg
                Else
                    Return _AdditionalInsuredsManualCharge
                End If
            End Get
            Set(value As String)
                If QuickQuoteHelperClass.UseComparativeRaterForLob(lob:=_LobType, quoteTransactionType:=QuoteTransactionType) <> QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then 'using <> Yes to also allow on Maybe
                    qqHelper.UpdateAdditionalInsuredsListBasedOnManualCharge(value, _AdditionalInsureds, additionalInsuredsBackup:=_AdditionalInsuredsBackup, updateBackupListBeforeRemoving:=True, effDate:=QuoteEffectiveDate, lobType:=_LobType, isAdditionalInsuredCheckboxBOP:=False, maintainOneItemFromOriginalListWhenResetting:=False, maintainFirstOrLastItem:=QuickQuoteHelperClass.FirstOrLast.First, maintainItemsOnUnresolvedDifference:=False, applicableItemToApplyDifferenceTo:=QuickQuoteHelperClass.FirstLastOrAll.All, firstOrLastItemOrderWhenApplyingDifferenceToAll:=QuickQuoteHelperClass.FirstOrLast.First, treatAmountsAsIntegerOverDecimal:=False)
                Else
                    _AdditionalInsuredsManualCharge = value
                End If
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverages w/ coveragecode_ids 926, 21018, 501, 21022, 21019, 21023, 21020, 21053, 21054, 21055, 21024, 21025, 21026, 21016, 21017, or 21021</remarks>
        Public Property AdditionalInsuredsQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_AdditionalInsuredsQuotedPremium)
            End Get
            Set(value As String)
                _AdditionalInsuredsQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_AdditionalInsuredsQuotedPremium)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverages w/ coveragecode_ids 926, 21018, 501, 21022, 21019, 21023, 21020, 21053, 21054, 21055, 21024, 21025, 21026, 21016, 21017, or 21021</remarks>
        Public Property AdditionalInsureds As Generic.List(Of QuickQuoteAdditionalInsured)
            Get
                Return _AdditionalInsureds
            End Get
            Set(value As Generic.List(Of QuickQuoteAdditionalInsured))
                _AdditionalInsureds = value
            End Set
        End Property
        Public ReadOnly Property AdditionalInsuredsBackup As List(Of QuickQuoteAdditionalInsured)
            Get
                Return _AdditionalInsuredsBackup
            End Get
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>specific to Diamond InclusionsExclusions w/ TypeId 12</remarks>
        Public Property HasExclusionOfAmishWorkers As Boolean
            Get
                Return _HasExclusionOfAmishWorkers
            End Get
            Set(value As Boolean)
                _HasExclusionOfAmishWorkers = value
                If QuickQuoteHelperClass.UseComparativeRaterForLob(lob:=_LobType, quoteTransactionType:=QuoteTransactionType) <> QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then 'using <> Yes to also allow on Maybe
                    QuickQuoteHelperClass.UpdateExclusionOfAmishWorkerListFromHasFlag(_ExclusionOfAmishWorkerRecords, _HasExclusionOfAmishWorkers, exclusionsBackup:=_ExclusionOfAmishWorkerRecordsBackup, updateBackupListBeforeRemoving:=True, addInitialItemIfNeeded:=False)
                End If
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>specific to Diamond InclusionsExclusions w/ TypeId 13</remarks>
        Public Property HasExclusionOfSoleProprietorsPartnersOfficersAndOthers As Boolean
            Get
                Return _HasExclusionOfSoleProprietorsPartnersOfficersAndOthers
            End Get
            Set(value As Boolean)
                _HasExclusionOfSoleProprietorsPartnersOfficersAndOthers = value
                If QuickQuoteHelperClass.UseComparativeRaterForLob(lob:=_LobType, quoteTransactionType:=QuoteTransactionType) <> QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then 'using <> Yes to also allow on Maybe
                    QuickQuoteHelperClass.UpdateExclusionOfSoleProprietorListFromHasFlag(_ExclusionOfSoleProprietorRecords, _HasExclusionOfSoleProprietorsPartnersOfficersAndOthers, exclusionsBackup:=_ExclusionOfSoleProprietorRecordsBackup, updateBackupListBeforeRemoving:=True, addInitialItemIfNeeded:=False)
                End If
            End Set
        End Property
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
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>specific to Diamond InclusionsExclusions w/ TypeId 12</remarks>
        Public Property ExclusionOfAmishWorkerRecords As Generic.List(Of QuickQuoteExclusionOfAmishWorkerRecord)
            Get
                Return _ExclusionOfAmishWorkerRecords
            End Get
            Set(value As Generic.List(Of QuickQuoteExclusionOfAmishWorkerRecord))
                _ExclusionOfAmishWorkerRecords = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80560 (CAP)</remarks>
        Public Property LegalEntityTypeQuotedPremium As String
            Get
                Return _LegalEntityTypeQuotedPremium
            End Get
            Set(value As String)
                _LegalEntityTypeQuotedPremium = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80560 (CAP)</remarks>
        Public Property LegalEntityType As TriState
            Get
                Return _LegalEntityType
            End Get
            Set(value As TriState)
                _LegalEntityType = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>specific to Diamond InclusionsExclusions w/ TypeId 13</remarks>
        Public Property ExclusionOfSoleProprietorRecords As Generic.List(Of QuickQuoteExclusionOfSoleProprietorRecord)
            Get
                Return _ExclusionOfSoleProprietorRecords
            End Get
            Set(value As Generic.List(Of QuickQuoteExclusionOfSoleProprietorRecord))
                _ExclusionOfSoleProprietorRecords = value
            End Set
        End Property
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
        Public ReadOnly Property ExclusionOfAmishWorkerRecordsBackup As Generic.List(Of QuickQuoteExclusionOfAmishWorkerRecord)
            Get
                Return _ExclusionOfAmishWorkerRecordsBackup
            End Get
        End Property
        Public ReadOnly Property ExclusionOfSoleProprietorRecordsBackup As Generic.List(Of QuickQuoteExclusionOfSoleProprietorRecord)
            Get
                Return _ExclusionOfSoleProprietorRecordsBackup
            End Get
        End Property
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
        Public Property Dec_BOP_OptCovs_Premium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_Dec_BOP_OptCovs_Premium)
            End Get
            Set(value As String)
                _Dec_BOP_OptCovs_Premium = value
                qqHelper.ConvertToQuotedPremiumFormat(_Dec_BOP_OptCovs_Premium)
            End Set
        End Property

        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 10124 (Workers Compensation); value for CoverageAdditionalInfoRecord w/ description containing 'Experience Mod Premium'</remarks>
        Public Property ExpModQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_ExpModQuotedPremium)
            End Get
            Set(value As String)
                _ExpModQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_ExpModQuotedPremium)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 10124 (Workers Compensation); value for CoverageAdditionalInfoRecord w/ description containing 'Scheduled Rating Plan Premium'</remarks>
        Public Property ScheduleModQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_ScheduleModQuotedPremium)
            End Get
            Set(value As String)
                _ScheduleModQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_ScheduleModQuotedPremium)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 10124 (Workers Compensation); value for CoverageAdditionalInfoRecord w/ description containing 'Terrorism Risk Premium'</remarks>
        Public Property TerrorismQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_TerrorismQuotedPremium)
            End Get
            Set(value As String)
                _TerrorismQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_TerrorismQuotedPremium)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 10124 (Workers Compensation); sum of all values for CoverageAdditionalInfoRecords w/ description containing 'Workers Compensation - Premium Discount'</remarks>
        Public Property PremDiscountQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_PremDiscountQuotedPremium)
            End Get
            Set(value As String)
                _PremDiscountQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_PremDiscountQuotedPremium)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 10124 (Workers Compensation); value for CoverageAdditionalInfoRecord w/ description containing 'Minimum Premium'</remarks>
        Public Property MinimumQuotedPremium As String
            Get
                If _MinimumQuotedPremium = "" Then
                    qqHelper.ConvertToQuotedPremiumFormat(_MinimumQuotedPremium, QuickQuoteHelperClass.QuickQuoteEmptyStringReplacementType.NonApplicable)
                End If
                Return _MinimumQuotedPremium
            End Get
            Set(value As String)
                _MinimumQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_MinimumQuotedPremium, QuickQuoteHelperClass.QuickQuoteEmptyStringReplacementType.NonApplicable)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 10124 (Workers Compensation); value for CoverageAdditionalInfoRecord w/ description containing 'Minimum Premium Adjustment'</remarks>
        Public Property MinimumPremiumAdjustment As String
            Get
                If _MinimumPremiumAdjustment = "" Then
                    qqHelper.ConvertToQuotedPremiumFormat(_MinimumPremiumAdjustment, QuickQuoteHelperClass.QuickQuoteEmptyStringReplacementType.Zero)
                End If
                Return _MinimumPremiumAdjustment
            End Get
            Set(value As String)
                _MinimumPremiumAdjustment = value
                qqHelper.ConvertToQuotedPremiumFormat(_MinimumPremiumAdjustment, QuickQuoteHelperClass.QuickQuoteEmptyStringReplacementType.Zero)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 10055 (Subject)</remarks>
        Public Property TotalEstimatedPlanPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_TotalEstimatedPlanPremium)
            End Get
            Set(value As String)
                _TotalEstimatedPlanPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_TotalEstimatedPlanPremium)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 20227</remarks>
        Public Property SecondInjuryFundQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_SecondInjuryFundQuotedPremium)
            End Get
            Set(value As String)
                _SecondInjuryFundQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_SecondInjuryFundQuotedPremium)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80559</remarks>
        Public Property IL_WCP_CommissionOperationsFundSurcharge As String
            Get
                Return qqHelper.QuotedPremiumFormat(_IL_WCP_CommissionOperationsFundSurcharge)
            End Get
            Set(value As String)
                _IL_WCP_CommissionOperationsFundSurcharge = value
                qqHelper.ConvertToQuotedPremiumFormat(_IL_WCP_CommissionOperationsFundSurcharge)
            End Set
        End Property
        Public Property Dec_LossConstantPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_Dec_LossConstantPremium)
            End Get
            Set(value As String)
                _Dec_LossConstantPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_Dec_LossConstantPremium)
            End Set
        End Property
        Public Property Dec_ExpenseConstantPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_Dec_ExpenseConstantPremium)
            End Get
            Set(value As String)
                _Dec_ExpenseConstantPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_Dec_ExpenseConstantPremium)
            End Set
        End Property
        Public Property Dec_WC_TotalPremiumDue As String
            Get
                Return qqHelper.QuotedPremiumFormat(_Dec_WC_TotalPremiumDue)
            End Get
            Set(value As String)
                _Dec_WC_TotalPremiumDue = value
                qqHelper.ConvertToQuotedPremiumFormat(_Dec_WC_TotalPremiumDue)
            End Set
        End Property
        Public Property Dec_GL_OptCovs_Premium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_Dec_GL_OptCovs_Premium)
            End Get
            Set(value As String)
                _Dec_GL_OptCovs_Premium = value
                qqHelper.ConvertToQuotedPremiumFormat(_Dec_GL_OptCovs_Premium)
            End Set
        End Property
        Public Property Dec_CAP_OptCovs_Premium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_Dec_CAP_OptCovs_Premium)
            End Get
            Set(value As String)
                _Dec_CAP_OptCovs_Premium = value
                qqHelper.ConvertToQuotedPremiumFormat(_Dec_CAP_OptCovs_Premium)
            End Set
        End Property
        Public Property Dec_CAP_OptCovs_Premium_Without_GarageKeepers As String
            Get
                Return qqHelper.QuotedPremiumFormat(_Dec_CAP_OptCovs_Premium_Without_GarageKeepers)
            End Get
            Set(value As String)
                _Dec_CAP_OptCovs_Premium_Without_GarageKeepers = value
                qqHelper.ConvertToQuotedPremiumFormat(_Dec_CAP_OptCovs_Premium_Without_GarageKeepers)
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
        Public Property HasConvertedInclusionsExclusions As Boolean
            Get
                Return _HasConvertedInclusionsExclusions
            End Get
            Set(value As Boolean)
                _HasConvertedInclusionsExclusions = value
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
        Public Property HasConvertedScheduledRatings As Boolean
            Get
                Return _HasConvertedScheduledRatings
            End Get
            Set(value As Boolean)
                _HasConvertedScheduledRatings = value
            End Set
        End Property
        Public Property CanUseExclusionNumForExclusionReconciliation As Boolean
            Get
                Return _CanUseExclusionNumForExclusionReconciliation
            End Get
            Set(value As Boolean)
                _CanUseExclusionNumForExclusionReconciliation = value
            End Set
        End Property
        Public Property CanUseLossHistoryNumForLossHistoryReconciliation As Boolean
            Get
                Return _CanUseLossHistoryNumForLossHistoryReconciliation
            End Get
            Set(value As Boolean)
                _CanUseLossHistoryNumForLossHistoryReconciliation = value
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
        Public Property HasConvertedScheduledCoverages As Boolean
            Get
                Return _HasConvertedScheduledCoverages
            End Get
            Set(value As Boolean)
                _HasConvertedScheduledCoverages = value
            End Set
        End Property
        Public Property CanUseScheduledCoverageNumForScheduledCoverageReconciliation As Boolean
            Get
                Return _CanUseScheduledCoverageNumForScheduledCoverageReconciliation
            End Get
            Set(value As Boolean)
                _CanUseScheduledCoverageNumForScheduledCoverageReconciliation = value
            End Set
        End Property
        Public Property CanUseClassificationCodeNumForClassificationCodeReconciliation As Boolean 'for reconciliation
            Get
                Return _CanUseClassificationCodeNumForClassificationCodeReconciliation
            End Get
            Set(value As Boolean)
                _CanUseClassificationCodeNumForClassificationCodeReconciliation = value
            End Set
        End Property
        Public Property HasConvertedFarmIncidentalLimitCoverages As Boolean
            Get
                Return _HasConvertedFarmIncidentalLimitCoverages
            End Get
            Set(value As Boolean)
                _HasConvertedFarmIncidentalLimitCoverages = value
            End Set
        End Property
        Public Property HasConvertedScheduledPersonalPropertyCoverages As Boolean
            Get
                Return _HasConvertedScheduledPersonalPropertyCoverages
            End Get
            Set(value As Boolean)
                _HasConvertedScheduledPersonalPropertyCoverages = value
            End Set
        End Property
        Public Property HasConvertedUnscheduledPersonalPropertyCoverages As Boolean
            Get
                Return _HasConvertedUnscheduledPersonalPropertyCoverages
            End Get
            Set(value As Boolean)
                _HasConvertedUnscheduledPersonalPropertyCoverages = value
            End Set
        End Property
        Public Property CanUseScheduledFarmPersonalPropertyNumForScheduledPersonalPropertyReconciliation As Boolean
            Get
                Return _CanUseScheduledFarmPersonalPropertyNumForScheduledPersonalPropertyReconciliation
            End Get
            Set(value As Boolean)
                _CanUseScheduledFarmPersonalPropertyNumForScheduledPersonalPropertyReconciliation = value
            End Set
        End Property
        Public Property CanUseUnscheduledFarmPersonalPropertyNumForUnscheduledPersonalPropertyReconciliation As Boolean
            Get
                Return _CanUseUnscheduledFarmPersonalPropertyNumForUnscheduledPersonalPropertyReconciliation
            End Get
            Set(value As Boolean)
                _CanUseUnscheduledFarmPersonalPropertyNumForUnscheduledPersonalPropertyReconciliation = value
            End Set
        End Property
        Public Property HasConvertedOptionalCoverages As Boolean
            Get
                Return _HasConvertedOptionalCoverages
            End Get
            Set(value As Boolean)
                _HasConvertedOptionalCoverages = value
            End Set
        End Property
        Public Property CanUseOptionalCoveragesNumForOptionalCoverageReconciliation As Boolean
            Get
                Return _CanUseOptionalCoveragesNumForOptionalCoverageReconciliation
            End Get
            Set(value As Boolean)
                _CanUseOptionalCoveragesNumForOptionalCoverageReconciliation = value
            End Set
        End Property
        Public Property CanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean
            Get
                Return _CanUseAdditionalInterestNumForAdditionalInterestReconciliation
            End Get
            Set(value As Boolean)
                _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = value
            End Set
        End Property
        Public Property CPP_MinPremAdj_CPR As String 'covCodeId 10121; note: covCodeId good for CGL, PIM (not in VR yet), FAR, CAP, WCP, GAR, BOP, CRM, CPR, CIM
            Get
                Return qqHelper.QuotedPremiumFormat(_CPP_MinPremAdj_CPR)
            End Get
            Set(value As String)
                _CPP_MinPremAdj_CPR = value
                qqHelper.ConvertToQuotedPremiumFormat(_CPP_MinPremAdj_CPR)
            End Set
        End Property
        Public Property CPP_MinPremAdj_CGL As String 'covCodeId 10121; note: covCodeId good for CGL, PIM (not in VR yet), FAR, CAP, WCP, GAR, BOP, CRM, CPR, CIM
            Get
                Return qqHelper.QuotedPremiumFormat(_CPP_MinPremAdj_CGL)
            End Get
            Set(value As String)
                _CPP_MinPremAdj_CGL = value
                qqHelper.ConvertToQuotedPremiumFormat(_CPP_MinPremAdj_CGL)
            End Set
        End Property
        Public Property CPP_MinPremAdj_CIM As String 'covCodeId 10121; note: covCodeId good for CGL, PIM (not in VR yet), FAR, CAP, WCP, GAR, BOP, CRM, CPR, CIM
            Get
                Return qqHelper.QuotedPremiumFormat(_CPP_MinPremAdj_CIM)
            End Get
            Set(value As String)
                _CPP_MinPremAdj_CIM = value
                qqHelper.ConvertToQuotedPremiumFormat(_CPP_MinPremAdj_CIM)
            End Set
        End Property
        Public Property CPP_MinPremAdj_CRM As String 'covCodeId 10121; note: covCodeId good for CGL, PIM (not in VR yet), FAR, CAP, WCP, GAR, BOP, CRM, CPR, CIM
            Get
                Return qqHelper.QuotedPremiumFormat(_CPP_MinPremAdj_CRM)
            End Get
            Set(value As String)
                _CPP_MinPremAdj_CRM = value
                qqHelper.ConvertToQuotedPremiumFormat(_CPP_MinPremAdj_CRM)
            End Set
        End Property
        Public Property CPP_MinPremAdj_GAR As String 'covCodeId 10121; note: covCodeId good for CGL, PIM (not in VR yet), FAR, CAP, WCP, GAR, BOP, CRM, CPR, CIM
            Get
                Return qqHelper.QuotedPremiumFormat(_CPP_MinPremAdj_GAR)
            End Get
            Set(value As String)
                _CPP_MinPremAdj_GAR = value
                qqHelper.ConvertToQuotedPremiumFormat(_CPP_MinPremAdj_GAR)
            End Set
        End Property
        Public Property CAP_GAR_PolicyLevelCovs_Premium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_CAP_GAR_PolicyLevelCovs_Premium)
            End Get
            Set(value As String)
                _CAP_GAR_PolicyLevelCovs_Premium = value
                qqHelper.ConvertToQuotedPremiumFormat(_CAP_GAR_PolicyLevelCovs_Premium)
            End Set
        End Property
        'Public Property WCP_WaiverPremium As String 'covCodeId 10124 CovAddInfo w/ "Waiver Premium" in desc
        '    Get
        '        Return qqHelper.QuotedPremiumFormat(_WCP_WaiverPremium)
        '    End Get
        '    Set(value As String)
        '        _WCP_WaiverPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_WCP_WaiverPremium)
        '    End Set
        'End Property
        'added 10/15/2018 for IL (similar to existing props w/o IL, but different form # and typeId)
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>specific to Diamond InclusionsExclusions w/ TypeId 287</remarks>
        Public Property HasExclusionOfSoleProprietorsPartnersOfficersAndOthers_IL As Boolean
            Get
                Return _HasExclusionOfSoleProprietorsPartnersOfficersAndOthers_IL
            End Get
            Set(value As Boolean)
                _HasExclusionOfSoleProprietorsPartnersOfficersAndOthers_IL = value
                If QuickQuoteHelperClass.UseComparativeRaterForLob(lob:=_LobType, quoteTransactionType:=QuoteTransactionType) <> QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then 'using <> Yes to also allow on Maybe
                    QuickQuoteHelperClass.UpdateExclusionOfSoleProprietorListFromHasFlag_IL(_ExclusionOfSoleProprietorRecords_IL, _HasExclusionOfSoleProprietorsPartnersOfficersAndOthers_IL, exclusionsBackup:=_ExclusionOfSoleProprietorRecordsBackup_IL, updateBackupListBeforeRemoving:=True, addInitialItemIfNeeded:=False)
                End If
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>specific to Diamond InclusionsExclusions w/ TypeId 287</remarks>
        Public Property ExclusionOfSoleProprietorRecords_IL As List(Of QuickQuoteExclusionOfSoleProprietorRecord_IL)
            Get
                Return _ExclusionOfSoleProprietorRecords_IL
            End Get
            Set(value As List(Of QuickQuoteExclusionOfSoleProprietorRecord_IL))
                _ExclusionOfSoleProprietorRecords_IL = value
            End Set
        End Property
        Public ReadOnly Property ExclusionOfSoleProprietorRecordsBackup_IL As Generic.List(Of QuickQuoteExclusionOfSoleProprietorRecord_IL)
            Get
                Return _ExclusionOfSoleProprietorRecordsBackup_IL
            End Get
        End Property
        'added 4/26/2019 for KY
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>specific to Diamond InclusionsExclusions w/ TypeId 291</remarks>
        Public Property HasKentuckyRejectionOfCoverageEndorsement As Boolean
            Get
                Return _HasKentuckyRejectionOfCoverageEndorsement
            End Get
            Set(value As Boolean)
                _HasKentuckyRejectionOfCoverageEndorsement = value
                If QuickQuoteHelperClass.UseComparativeRaterForLob(lob:=_LobType, quoteTransactionType:=QuoteTransactionType) <> QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then 'using <> Yes to also allow on Maybe
                    QuickQuoteHelperClass.UpdateKentuckyRejectionOfCoverageEndorsementListFromHasFlag(_KentuckyRejectionOfCoverageEndorsementRecords, _HasKentuckyRejectionOfCoverageEndorsement, krcesBackup:=_KentuckyRejectionOfCoverageEndorsementRecordsBackup, updateBackupListBeforeRemoving:=True, addInitialItemIfNeeded:=False)
                End If
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>specific to Diamond InclusionsExclusions w/ TypeId 291</remarks>
        Public Property KentuckyRejectionOfCoverageEndorsementRecords As List(Of QuickQuoteKentuckyRejectionOfCoverageEndorsement)
            Get
                Return _KentuckyRejectionOfCoverageEndorsementRecords
            End Get
            Set(value As List(Of QuickQuoteKentuckyRejectionOfCoverageEndorsement))
                _KentuckyRejectionOfCoverageEndorsementRecords = value
            End Set
        End Property
        Public ReadOnly Property KentuckyRejectionOfCoverageEndorsementRecordsBackup As List(Of QuickQuoteKentuckyRejectionOfCoverageEndorsement)
            Get
                Return _KentuckyRejectionOfCoverageEndorsementRecordsBackup
            End Get
        End Property

        'added 10/19/2018 - moved from AlliedToAllStates object
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
                qqHelper.ConvertToLimitFormat(_LiquorLiabilityAnnualGrossAlcoholSalesReceipts)
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
                qqHelper.ConvertToLimitFormat(_LiquorLiabilityAnnualGrossPackageSalesReceipts)
            End Set
        End Property
        '''' <summary>
        ''''
        '''' </summary>
        '''' <value></value>
        '''' <returns></returns>
        '''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21134</remarks>
        'Public ReadOnly Property LiquorLiabilityAggregateLimit As String '10/19/2018 note: had to leave on PolicyLevelExtended object since this object doesn't have OccurrenceLiabilityLimit
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
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21134</remarks>
        Public Property LiquorLiabilityOccurrenceLimit As String
            Get
                Return _LiquorLiabilityOccurrenceLimit
            End Get
            Set(value As String)
                _LiquorLiabilityOccurrenceLimit = value
                Select Case _LiquorLiabilityOccurrenceLimit
                    Case "N/A"
                        _LiquorLiabilityOccurrenceLimitId = "0"
                    Case "300,000"
                        _LiquorLiabilityOccurrenceLimitId = "33"
                    Case "500,000"
                        _LiquorLiabilityOccurrenceLimitId = "34"
                    Case "1,000,000"
                        _LiquorLiabilityOccurrenceLimitId = "56"
                    Case Else
                        _LiquorLiabilityOccurrenceLimitId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21134</remarks>
        Public Property LiquorLiabilityOccurrenceLimitId As String
            Get
                Return _LiquorLiabilityOccurrenceLimitId
            End Get
            Set(value As String)
                _LiquorLiabilityOccurrenceLimitId = value
                _LiquorLiabilityOccurrenceLimit = ""
                If IsNumeric(_LiquorLiabilityOccurrenceLimitId) = True Then
                    Select Case _LiquorLiabilityOccurrenceLimitId
                        Case "0"
                            _LiquorLiabilityOccurrenceLimit = "N/A"
                        Case "33"
                            _LiquorLiabilityOccurrenceLimit = "300,000"
                        Case "34"
                            _LiquorLiabilityOccurrenceLimit = "500,000"
                        Case "56"
                            _LiquorLiabilityOccurrenceLimit = "1,000,000"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21134 (LiquorLiabilityClassificationId 50911 - Manufacturer, Wholesalers &amp; Distributors; LiquorLiabilityClassificationId 58161 - Restaurants or Hotels; LiquorLiabilityClassificationId 59211 - Package Stores; LiquorLiabilityClassificationId 70412 - Clubs</remarks>
        Public Property LiquorLiabilityClassification As String
            Get
                Return _LiquorLiabilityClassification
            End Get
            Set(value As String)
                _LiquorLiabilityClassification = value
                Select Case _LiquorLiabilityClassification
                    Case "Manufacturer, Wholesalers & Distributors"
                        _LiquorLiabilityClassificationId = "50911"
                    Case "Restaurants or Hotels"
                        _LiquorLiabilityClassificationId = "58161"
                    Case "Package Stores"
                        _LiquorLiabilityClassificationId = "59211"
                    Case "Clubs"
                        _LiquorLiabilityClassificationId = "70412"
                    Case Else
                        _LiquorLiabilityClassificationId = ""
                End Select
                'SetLiquorRateAndMinimumPremForClassificationId()
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21134 (LiquorLiabilityClassificationId 50911 - Manufacturer, Wholesalers &amp; Distributors; LiquorLiabilityClassificationId 58161 - Restaurants or Hotels; LiquorLiabilityClassificationId 59211 - Package Stores; LiquorLiabilityClassificationId 70412 - Clubs</remarks>
        Public Property LiquorLiabilityClassificationId As String
            Get
                Return _LiquorLiabilityClassificationId
            End Get
            Set(value As String)
                _LiquorLiabilityClassificationId = value
                _LiquorLiabilityClassification = ""
                If IsNumeric(_LiquorLiabilityClassificationId) = True Then
                    Select Case _LiquorLiabilityClassificationId
                        Case "50911"
                            _LiquorLiabilityClassification = "Manufacturer, Wholesalers & Distributors"
                        Case "58161"
                            _LiquorLiabilityClassification = "Restaurants or Hotels"
                        Case "59211"
                            _LiquorLiabilityClassification = "Package Stores"
                        Case "70412"
                            _LiquorLiabilityClassification = "Clubs"
                    End Select
                End If
                'SetLiquorRateAndMinimumPremForClassificationId()
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21134</remarks>
        Public Property LiquorSales As String
            Get
                Return _LiquorSales
            End Get
            Set(value As String)
                _LiquorSales = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks> 50911 - Manufacturers, Wholesalers and Distributors selling alcoholic beverages for consumption off premises CoverageCode_id 21134</remarks>
        Public Property LiquorManufacturersSales As String
            Get
                Return _LiquorManufacturersSales
            End Get
            Set(value As String)
                _LiquorManufacturersSales = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>58161 - Restaurants, Taverns, Hotels, Motels including package sales CoverageCode_id 21134</remarks>
        Public Property LiquorRestaurantsSales As String
            Get
                Return _LiquorRestaurantsSales
            End Get
            Set(value As String)
                _LiquorRestaurantsSales = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>59211 - Package Stores and other retail establishments selling alchoholic beverages for consumption off premises CoverageCode_id 21134</remarks>
        Public Property LiquorPackageStoresSales As String
            Get
                Return _LiquorPackageStoresSales
            End Get
            Set(value As String)
                _LiquorPackageStoresSales = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>70412 - Clubs CoverageCode_id 21134</remarks>
        Public Property LiquorClubsSales As String
            Get
                Return _LiquorClubsSales
            End Get
            Set(value As String)
                _LiquorClubsSales = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21134</remarks>
        Public Property LiquorLiabilityQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_LiquorLiabilityQuotedPremium)
            End Get
            Set(value As String)
                _LiquorLiabilityQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_LiquorLiabilityQuotedPremium)
            End Set
        End Property
        '12/5/2018 - moved the rest of the professional liability props stuff
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21032</remarks>
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
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21032</remarks>
        Public Property BarbersProfessionalLiabiltyQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_BarbersProfessionalLiabiltyQuotedPremium)
            End Get
            Set(value As String)
                _BarbersProfessionalLiabiltyQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_BarbersProfessionalLiabiltyQuotedPremium)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21032</remarks>
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
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21032</remarks>
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
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21032</remarks>
        Public Property BarbersProfessionalLiabilityDescription As String
            Get
                Return _BarbersProfessionalLiabilityDescription
            End Get
            Set(value As String)
                _BarbersProfessionalLiabilityDescription = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21033</remarks>
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
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21033</remarks>
        Public Property BeauticiansProfessionalLiabilityQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_BeauticiansProfessionalLiabilityQuotedPremium)
            End Get
            Set(value As String)
                _BeauticiansProfessionalLiabilityQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_BeauticiansProfessionalLiabilityQuotedPremium)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21033</remarks>
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
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21033</remarks>
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
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21033</remarks>
        Public Property BeauticiansProfessionalLiabilityDescription As String
            Get
                Return _BeauticiansProfessionalLiabilityDescription
            End Get
            Set(value As String)
                _BeauticiansProfessionalLiabilityDescription = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21034</remarks>
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
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21034</remarks>
        Public Property FuneralDirectorsProfessionalLiabilityQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_FuneralDirectorsProfessionalLiabilityQuotedPremium)
            End Get
            Set(value As String)
                _FuneralDirectorsProfessionalLiabilityQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_FuneralDirectorsProfessionalLiabilityQuotedPremium)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21034</remarks>
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
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21036</remarks>
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
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21036</remarks>
        Public Property PrintersProfessionalLiabilityQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_PrintersProfessionalLiabilityQuotedPremium)
            End Get
            Set(value As String)
                _PrintersProfessionalLiabilityQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_PrintersProfessionalLiabilityQuotedPremium)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21036</remarks>
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
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21058</remarks>
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
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21058</remarks>
        Public Property SelfStorageFacilityQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_SelfStorageFacilityQuotedPremium)
            End Get
            Set(value As String)
                _SelfStorageFacilityQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_SelfStorageFacilityQuotedPremium)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21058</remarks>
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
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 164</remarks>
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
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 164</remarks>
        Public Property VeterinariansProfessionalLiabilityEmpNum As String
            Get
                Return _VeterinariansProfessionalLiabilityEmpNum
            End Get
            Set(value As String)
                _VeterinariansProfessionalLiabilityEmpNum = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 164</remarks>
        Public Property VeterinariansProfessionalLiabilityQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_VeterinariansProfessionalLiabilityQuotedPremium)
            End Get
            Set(value As String)
                _VeterinariansProfessionalLiabilityQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_VeterinariansProfessionalLiabilityQuotedPremium)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 164</remarks>
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
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 164</remarks>
        Public Property PharmacistAnnualGrossSales As String
            Get
                Return _PharmacistAnnualGrossSales
            End Get
            Set(value As String)
                _PharmacistAnnualGrossSales = value
                qqHelper.ConvertToLimitFormat(_PharmacistAnnualGrossSales)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 164</remarks>
        Public Property PharmacistQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_PharmacistQuotedPremium)
            End Get
            Set(value As String)
                _PharmacistQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_PharmacistQuotedPremium)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21035</remarks>
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
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21035</remarks>
        Public Property OpticalAndHearingAidProfessionalLiabilityEmpNum As String
            Get
                Return _OpticalAndHearingAidProfessionalLiabilityEmpNum
            End Get
            Set(value As String)
                _OpticalAndHearingAidProfessionalLiabilityEmpNum = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21035</remarks>
        Public Property OpticalAndHearingAidProfessionalLiabilityQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_OpticalAndHearingAidProfessionalLiabilityQuotedPremium)
            End Get
            Set(value As String)
                _OpticalAndHearingAidProfessionalLiabilityQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_OpticalAndHearingAidProfessionalLiabilityQuotedPremium)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80376 and 80377</remarks>
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
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80376</remarks>
        Public Property MotelCoveragePerGuestQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_MotelCoveragePerGuestQuotedPremium)
            End Get
            Set(value As String)
                _MotelCoveragePerGuestQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_MotelCoveragePerGuestQuotedPremium)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80377</remarks>
        Public Property MotelCoverageSafeDepositQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_MotelCoverageSafeDepositQuotedPremium)
            End Get
            Set(value As String)
                _MotelCoverageSafeDepositQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_MotelCoverageSafeDepositQuotedPremium)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80377</remarks>
        <Obsolete("don't use this... premium for 80376 is in MotelCoveragePerGuestQuotedPremium, and premium for 80377 is in MotelCoverageSafeDepositQuotedPremium")>
        Public Property MotelCoverageQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_MotelCoverageQuotedPremium)
            End Get
            Set(value As String)
                _MotelCoverageQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_MotelCoverageQuotedPremium)
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
        Public Property PhotographyCoverageQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_PhotographyCoverageQuotedPremium)
            End Get
            Set(value As String)
                _PhotographyCoverageQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_PhotographyCoverageQuotedPremium)
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
                Return FormatNumber(total, 0).ToString
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
                Return _PhotographyScheduledCoverages
            End Get
            Set(value As List(Of QuickQuoteCoverage))
                _PhotographyScheduledCoverages = value
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
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80378</remarks>
        Public Property PhotographyMakeupAndHairQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_PhotographyMakeupAndHairQuotedPremium)
            End Get
            Set(value As String)
                _PhotographyMakeupAndHairQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_PhotographyMakeupAndHairQuotedPremium)
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
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80380</remarks>
        Public Property ResidentialCleaningQuotedPremium As String
            Get
                Return _ResidentialCleaningQuotedPremium
            End Get
            Set(value As String)
                _ResidentialCleaningQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_ResidentialCleaningQuotedPremium)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21131</remarks>
        Public Property ProfessionalLiabilityCemetaryNumberOfBurials As String
            Get
                Return _ProfessionalLiabilityCemetaryNumberOfBurials
            End Get
            Set(value As String)
                _ProfessionalLiabilityCemetaryNumberOfBurials = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21131</remarks>
        Public Property ProfessionalLiabilityCemetaryQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_ProfessionalLiabilityCemetaryQuotedPremium)
            End Get
            Set(value As String)
                _ProfessionalLiabilityCemetaryQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_ProfessionalLiabilityCemetaryQuotedPremium)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21034</remarks>
        Public Property ProfessionalLiabilityFuneralDirectorsNumberOfBodies As String
            Get
                Return _ProfessionalLiabilityFuneralDirectorsNumberOfBodies
            End Get
            Set(value As String)
                _ProfessionalLiabilityFuneralDirectorsNumberOfBodies = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21132</remarks>
        Public Property ProfessionalLiabilityPastoralNumberOfClergy As String
            Get
                Return _ProfessionalLiabilityPastoralNumberOfClergy
            End Get
            Set(value As String)
                _ProfessionalLiabilityPastoralNumberOfClergy = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21132</remarks>
        Public Property ProfessionalLiabilityPastoralQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_ProfessionalLiabilityPastoralQuotedPremium)
            End Get
            Set(value As String)
                _ProfessionalLiabilityPastoralQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_ProfessionalLiabilityPastoralQuotedPremium)
            End Set
        End Property
        '12/10/2018 - moved the remaining professional liability props that were missed 12/5/2018
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80387; stored in xml at policy level</remarks>
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
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80387; stored in xml at policy level</remarks>
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
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80387; stored in xml at policy level</remarks>
        Public Property ApartmentQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_ApartmentQuotedPremium)
            End Get
            Set(value As String)
                _ApartmentQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_ApartmentQuotedPremium)
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80387; stored in xml at policy level</remarks>
        Public Property HasRestaurantEndorsement As Boolean
            Get
                Return _HasRestaurantEndorsement
            End Get
            Set(value As Boolean)
                _HasRestaurantEndorsement = value
            End Set
        End Property
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80387; stored in xml at policy level</remarks>
        Public Property RestaurantQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_RestaurantQuotedPremium)
            End Get
            Set(value As String)
                _RestaurantQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_RestaurantQuotedPremium)
            End Set
        End Property
        'added 12/18/2018 - moved from GoverningState object since they're specific to Location Buildings (and their parent state quotes)
        Public Property ComputerCoinsuranceTypeId As String 'cov also has CoverageBasisTypeId set to 1
            Get
                Return _ComputerCoinsuranceTypeId
            End Get
            Set(value As String)
                _ComputerCoinsuranceTypeId = value
            End Set
        End Property
        Public Property ComputerExcludeEarthquake As Boolean
            Get
                Return _ComputerExcludeEarthquake
            End Get
            Set(value As Boolean)
                _ComputerExcludeEarthquake = value
            End Set
        End Property
        Public Property ComputerValuationMethodTypeId As String
            Get
                Return _ComputerValuationMethodTypeId
            End Get
            Set(value As String)
                _ComputerValuationMethodTypeId = value
            End Set
        End Property
        Public Property ComputerAdditionalInterests As List(Of QuickQuoteAdditionalInterest)
            Get
                Return _ComputerAdditionalInterests
            End Get
            Set(value As List(Of QuickQuoteAdditionalInterest))
                _ComputerAdditionalInterests = value
            End Set
        End Property
        Public Property ComputerCanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean
            Get
                Return _ComputerCanUseAdditionalInterestNumForAdditionalInterestReconciliation
            End Get
            Set(value As Boolean)
                _ComputerCanUseAdditionalInterestNumForAdditionalInterestReconciliation = value
            End Set
        End Property
        Public Property ComputerQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_ComputerQuotedPremium)
            End Get
            Set(value As String)
                _ComputerQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_ComputerQuotedPremium)
            End Set
        End Property
        Public Property ComputerAllPerilsDeductibleId As String 'cov also has CoverageBasisTypeId set to 1
            Get
                Return _ComputerAllPerilsDeductibleId
            End Get
            Set(value As String)
                _ComputerAllPerilsDeductibleId = value
            End Set
        End Property
        Public Property ComputerAllPerilsQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_ComputerAllPerilsQuotedPremium)
            End Get
            Set(value As String)
                _ComputerAllPerilsQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_ComputerAllPerilsQuotedPremium)
            End Set
        End Property
        Public Property ComputerEarthquakeVolcanicEruptionDeductible As String 'cov also has CoverageBasisTypeId set to 1; example also has ApplyToWrittenPremiuim set to true
            Get
                Return _ComputerEarthquakeVolcanicEruptionDeductible
            End Get
            Set(value As String)
                _ComputerEarthquakeVolcanicEruptionDeductible = value 'might need limit formatting
            End Set
        End Property
        Public Property ComputerEarthquakeVolcanicEruptionQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_ComputerEarthquakeVolcanicEruptionQuotedPremium)
            End Get
            Set(value As String)
                _ComputerEarthquakeVolcanicEruptionQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_ComputerEarthquakeVolcanicEruptionQuotedPremium)
            End Set
        End Property
        Public Property ComputerMechanicalBreakdownDeductible As String 'cov also has CoverageBasisTypeId set to 1; example also has ApplyToWrittenPremiuim set to true
            Get
                Return _ComputerMechanicalBreakdownDeductible
            End Get
            Set(value As String)
                _ComputerMechanicalBreakdownDeductible = value 'might need limit formatting
            End Set
        End Property
        Public Property ComputerMechanicalBreakdownQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_ComputerMechanicalBreakdownQuotedPremium)
            End Get
            Set(value As String)
                _ComputerMechanicalBreakdownQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_ComputerMechanicalBreakdownQuotedPremium)
            End Set
        End Property
        Public Property FineArtsDeductibleCategoryTypeId As String 'static data
            Get
                Return _FineArtsDeductibleCategoryTypeId
            End Get
            Set(value As String)
                _FineArtsDeductibleCategoryTypeId = value
            End Set
        End Property
        Public Property FineArtsRate As String
            Get
                Return _FineArtsRate
            End Get
            Set(value As String)
                _FineArtsRate = value
            End Set
        End Property
        Public Property FineArtsDeductibleId As String 'static data
            Get
                Return _FineArtsDeductibleId
            End Get
            Set(value As String)
                _FineArtsDeductibleId = value
            End Set
        End Property
        Public Property FineArtsQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_FineArtsQuotedPremium)
            End Get
            Set(value As String)
                _FineArtsQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_FineArtsQuotedPremium)
            End Set
        End Property
        Public Property FineArtsAdditionalInterests As List(Of QuickQuoteAdditionalInterest)
            Get
                Return _FineArtsAdditionalInterests
            End Get
            Set(value As List(Of QuickQuoteAdditionalInterest))
                _FineArtsAdditionalInterests = value
            End Set
        End Property
        Public Property FineArtsCanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean
            Get
                Return _FineArtsCanUseAdditionalInterestNumForAdditionalInterestReconciliation
            End Get
            Set(value As Boolean)
                _FineArtsCanUseAdditionalInterestNumForAdditionalInterestReconciliation = value
            End Set
        End Property
        Public Property FineArtsBreakageMarringOrScratching As Boolean 'renamed from HasFineArtsBreakageMarringOrScratching
            Get
                Return _FineArtsBreakageMarringOrScratching
            End Get
            Set(value As Boolean)
                _FineArtsBreakageMarringOrScratching = value
            End Set
        End Property
        Public Property FineArtsBreakageMarringOrScratchingQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_FineArtsBreakageMarringOrScratchingQuotedPremium)
            End Get
            Set(value As String)
                _FineArtsBreakageMarringOrScratchingQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_FineArtsBreakageMarringOrScratchingQuotedPremium)
            End Set
        End Property
        Public Property SignsAdditionalInterests As List(Of QuickQuoteAdditionalInterest) 'note: cov also has CoverageBasisTypeId set to 1
            Get
                Return _SignsAdditionalInterests
            End Get
            Set(value As List(Of QuickQuoteAdditionalInterest))
                _SignsAdditionalInterests = value
            End Set
        End Property
        Public Property SignsCanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean
            Get
                Return _SignsCanUseAdditionalInterestNumForAdditionalInterestReconciliation
            End Get
            Set(value As Boolean)
                _SignsCanUseAdditionalInterestNumForAdditionalInterestReconciliation = value
            End Set
        End Property
        Public Property SignsMaximumDeductible As String 'CoverageDetail; may need limit formatting
            Get
                Return _SignsMaximumDeductible
            End Get
            Set(value As String)
                _SignsMaximumDeductible = value
            End Set
        End Property
        Public Property SignsMinimumDeductible As String 'CoverageDetail; may need limit formatting
            Get
                Return _SignsMinimumDeductible
            End Get
            Set(value As String)
                _SignsMinimumDeductible = value
            End Set
        End Property
        Public Property SignsValuationMethodTypeId As String 'CoverageDetail; static data
            Get
                Return _SignsValuationMethodTypeId
            End Get
            Set(value As String)
                _SignsValuationMethodTypeId = value
            End Set
        End Property
        Public Property SignsDeductibleId As String 'static data
            Get
                Return _SignsDeductibleId
            End Get
            Set(value As String)
                _SignsDeductibleId = value
            End Set
        End Property
        Public Property SignsQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_SignsQuotedPremium)
            End Get
            Set(value As String)
                _SignsQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_SignsQuotedPremium)
            End Set
        End Property
        Public Property SignsAnyOneLossCatastropheLimit As String 'note: cov also has CoverageBasisTypeId set to 1
            Get
                Return _SignsAnyOneLossCatastropheLimit
            End Get
            Set(value As String)
                _SignsAnyOneLossCatastropheLimit = value
                qqHelper.ConvertToLimitFormat(_SignsAnyOneLossCatastropheLimit)
            End Set
        End Property
        Public Property SignsAnyOneLossCatastropheQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_SignsAnyOneLossCatastropheQuotedPremium)
            End Get
            Set(value As String)
                _SignsAnyOneLossCatastropheQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_SignsAnyOneLossCatastropheQuotedPremium)
            End Set
        End Property

        Public Property HasHerbicidePersticideApplicator As Boolean
            Get
                Return _HasHerbicidePersticideApplicator
            End Get
            Set(value As Boolean)
                _HasHerbicidePersticideApplicator = value
            End Set
        End Property

        'added 10/24/2018
        Public Property HasIllinoisContractorsHomeRepairAndRemodeling As Boolean
            Get
                Return _HasIllinoisContractorsHomeRepairAndRemodeling
            End Get
            Set(value As Boolean)
                _HasIllinoisContractorsHomeRepairAndRemodeling = value
            End Set
        End Property
        Public Property IllinoisContractorsHomeRepairAndRemodelingManualLimitAmount As String
            Get
                Return _IllinoisContractorsHomeRepairAndRemodelingManualLimitAmount
            End Get
            Set(value As String)
                _IllinoisContractorsHomeRepairAndRemodelingManualLimitAmount = value
                qqHelper.ConvertToLimitFormat(_IllinoisContractorsHomeRepairAndRemodelingManualLimitAmount)
            End Set
        End Property
        Public Property IllinoisContractorsHomeRepairAndRemodelingQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_IllinoisContractorsHomeRepairAndRemodelingQuotedPremium)
            End Get
            Set(value As String)
                _IllinoisContractorsHomeRepairAndRemodelingQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_IllinoisContractorsHomeRepairAndRemodelingQuotedPremium)
            End Set
        End Property

        'added 11/28/2018 for WCP IL (included in total premium)
        Public Property CatastropheOtherThanCertifiedActsOfTerrorismQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_CatastropheOtherThanCertifiedActsOfTerrorismQuotedPremium)
            End Get
            Set(value As String)
                _CatastropheOtherThanCertifiedActsOfTerrorismQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_CatastropheOtherThanCertifiedActsOfTerrorismQuotedPremium)
            End Set
        End Property

        'added 7/15/2019 for WCP KY
        Public Property KentuckySpecialFundAssessmentQuotedPremium As String
            Get
                Return qqHelper.QuotedPremiumFormat(_KentuckySpecialFundAssessmentQuotedPremium)
            End Get
            Set(value As String)
                _KentuckySpecialFundAssessmentQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_KentuckySpecialFundAssessmentQuotedPremium)
            End Set
        End Property
        Public Property WCP_KY_PremSurcharge As String
            Get
                Return qqHelper.QuotedPremiumFormat(_WCP_KY_PremSurcharge)
            End Get
            Set(value As String)
                _WCP_KY_PremSurcharge = value
                qqHelper.ConvertToQuotedPremiumFormat(_WCP_KY_PremSurcharge)
            End Set
        End Property

        'note: may not be able to Serialize Protected Friend; updated 8/18/2018 to Friend and then Public... may need to come up w/ some solution to prevent these from being used by Devs
        Public Property BasePolicyLevelInfo As QuickQuoteLobPolicyLevelInfo_AppliedToIndividualState
            Get
                If _BasePolicyLevelInfo Is Nothing Then
                    _BasePolicyLevelInfo = New QuickQuoteLobPolicyLevelInfo_AppliedToIndividualState
                End If
                SetObjectsParent(_BasePolicyLevelInfo)
                Return _BasePolicyLevelInfo
            End Get
            Set(value As QuickQuoteLobPolicyLevelInfo_AppliedToIndividualState)
                _BasePolicyLevelInfo = value
                SetObjectsParent(_BasePolicyLevelInfo)
            End Set
        End Property

        Public Property UninsuredMotoristPropertyDamage_IL_QuotedPremium As String 'covCodeId 30015; may not be populated
            Get
                Return qqHelper.QuotedPremiumFormat(_UninsuredMotoristPropertyDamage_IL_QuotedPremium)
            End Get
            Set(value As String)
                _UninsuredMotoristPropertyDamage_IL_QuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_UninsuredMotoristPropertyDamage_IL_QuotedPremium)
            End Set
        End Property
        Public Property UninsuredMotoristPropertyDamage_IL_LimitId As String 'covCodeId 21539; note: same prop exists on Vehicle
            Get
                Return _UninsuredMotoristPropertyDamage_IL_LimitId
            End Get
            Set(value As String)
                _UninsuredMotoristPropertyDamage_IL_LimitId = value
            End Set
        End Property
        Public Property UninsuredMotoristPropertyDamage_IL_DeductibleId As String 'covCodeId 21539
            Get
                Return _UninsuredMotoristPropertyDamage_IL_DeductibleId
            End Get
            Set(value As String)
                _UninsuredMotoristPropertyDamage_IL_DeductibleId = value
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
            _BasePolicyLevelInfo = New QuickQuoteLobPolicyLevelInfo_AppliedToIndividualState

            'PolicyLevel
            _AdditionalInsuredsCount = 0
            _AdditionalInsuredsCheckboxBOP = Nothing
            _HasAdditionalInsuredsCheckboxBOP = False
            _AdditionalInsuredsManualCharge = ""
            _AdditionalInsuredsQuotedPremium = ""
            _AdditionalInsureds = Nothing
            _AdditionalInsuredsBackup = Nothing
            _HasExclusionOfAmishWorkers = False
            _HasExclusionOfSoleProprietorsPartnersOfficersAndOthers = False
            '_HasInclusionOfSoleProprietorsPartnersOfficersAndOthers = False
            '_HasWaiverOfSubrogation = False
            '_WaiverOfSubrogationNumberOfWaivers = 0
            '_WaiverOfSubrogationPremium = ""
            '_WaiverOfSubrogationPremiumId = ""
            '_NeedsToUpdateWaiverOfSubrogationPremiumId = False
            _ExclusionOfAmishWorkerRecords = Nothing
            _ExclusionOfSoleProprietorRecords = Nothing
            '_InclusionOfSoleProprietorRecords = Nothing
            '_WaiverOfSubrogationRecords = Nothing
            _ExclusionOfAmishWorkerRecordsBackup = Nothing
            _ExclusionOfSoleProprietorRecordsBackup = Nothing
            '_InclusionOfSoleProprietorRecordsBackup = Nothing
            '_WaiverOfSubrogationRecordsBackup = Nothing
            _Dec_BOP_OptCovs_Premium = ""
            _ExpModQuotedPremium = ""
            _ScheduleModQuotedPremium = ""
            _TerrorismQuotedPremium = ""
            _PremDiscountQuotedPremium = ""
            _MinimumQuotedPremium = ""
            _MinimumPremiumAdjustment = ""
            _TotalEstimatedPlanPremium = ""
            _SecondInjuryFundQuotedPremium = ""
            _IL_WCP_CommissionOperationsFundSurcharge = ""
            Dec_LossConstantPremium = "0" 'using property to set default so formatting happens
            Dec_ExpenseConstantPremium = "160" 'using property to set default so formatting happens
            _Dec_WC_TotalPremiumDue = ""
            _Dec_GL_OptCovs_Premium = ""
            _Dec_CAP_OptCovs_Premium = ""
            _Dec_CAP_OptCovs_Premium_Without_GarageKeepers = ""
            _HasConvertedCoverages = False
            _HasConvertedInclusionsExclusions = False
            _HasConvertedModifiers = False
            _HasConvertedScheduledRatings = False
            _CanUseExclusionNumForExclusionReconciliation = False
            _CanUseLossHistoryNumForLossHistoryReconciliation = False
            _CanUsePolicyUnderwritingNumForPolicyUnderwritingReconciliation = False
            _HasConvertedScheduledCoverages = False
            _CanUseScheduledCoverageNumForScheduledCoverageReconciliation = False
            _CanUseClassificationCodeNumForClassificationCodeReconciliation = False
            _HasConvertedFarmIncidentalLimitCoverages = False
            _HasConvertedScheduledPersonalPropertyCoverages = False
            _HasConvertedUnscheduledPersonalPropertyCoverages = False
            _CanUseScheduledFarmPersonalPropertyNumForScheduledPersonalPropertyReconciliation = False
            _CanUseUnscheduledFarmPersonalPropertyNumForUnscheduledPersonalPropertyReconciliation = False
            _HasConvertedOptionalCoverages = False
            _CanUseOptionalCoveragesNumForOptionalCoverageReconciliation = False
            _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = False
            _CPP_MinPremAdj_CPR = "" 'covCodeId 10121; note: covCodeId good for CGL, PIM (not in VR yet), FAR, CAP, WCP, GAR, BOP, CRM, CPR, CIM
            _CPP_MinPremAdj_CGL = "" 'covCodeId 10121; note: covCodeId good for CGL, PIM (not in VR yet), FAR, CAP, WCP, GAR, BOP, CRM, CPR, CIM
            _CPP_MinPremAdj_CIM = "" 'covCodeId 10121; note: covCodeId good for CGL, PIM (not in VR yet), FAR, CAP, WCP, GAR, BOP, CRM, CPR, CIM
            _CPP_MinPremAdj_CRM = "" 'covCodeId 10121; note: covCodeId good for CGL, PIM (not in VR yet), FAR, CAP, WCP, GAR, BOP, CRM, CPR, CIM
            _CPP_MinPremAdj_GAR = "" 'covCodeId 10121; note: covCodeId good for CGL, PIM (not in VR yet), FAR, CAP, WCP, GAR, BOP, CRM, CPR, CIM
            _CAP_GAR_PolicyLevelCovs_Premium = ""
            '_WCP_WaiverPremium = "" 'covCodeId 10124 CovAddInfo w/ "Waiver Premium" in desc
            'added 10/15/2018 for IL (similar to existing props w/o IL, but different form # and typeId)
            _HasExclusionOfSoleProprietorsPartnersOfficersAndOthers_IL = False
            _ExclusionOfSoleProprietorRecords_IL = Nothing
            _ExclusionOfSoleProprietorRecordsBackup_IL = Nothing
            _LegalEntityTypeQuotedPremium = ""
            _LegalEntityType = TriState.UseDefault
            'added 4/26/2019 for KY
            _HasKentuckyRejectionOfCoverageEndorsement = False
            _KentuckyRejectionOfCoverageEndorsementRecords = Nothing
            _KentuckyRejectionOfCoverageEndorsementRecordsBackup = Nothing

            'added 10/19/2018 - moved from AlliedToAllStates object
            _HasLiquorLiability = False
            _LiquorLiabilityClassCodeTypeId = "" '12 = 58161 - Restaurant Includes Package Sales, 13 = 59211 - Package Sales for Consumption Off Premises
            _LiquorLiabilityAnnualGrossPackageSalesReceipts = ""
            _LiquorLiabilityAnnualGrossAlcoholSalesReceipts = ""
            _LiquorLiabilityOccurrenceLimit = ""
            _LiquorLiabilityOccurrenceLimitId = ""
            _LiquorLiabilityClassification = ""
            _LiquorLiabilityClassificationId = ""
            _LiquorSales = ""
            _LiquorLiabilityQuotedPremium = ""
            '12/5/2018 - moved the rest of the professional liability props stuff
            _HasBarbersProfessionalLiability = False
            _BarbersProfessionalLiabiltyQuotedPremium = ""
            _BarbersProfessionalLiabilityFullTimeEmpNum = ""
            _BarbersProfessionalLiabilityPartTimeEmpNum = ""
            _BarbersProfessionalLiabilityDescription = ""
            _HasBeauticiansProfessionalLiability = False
            _BeauticiansProfessionalLiabilityQuotedPremium = ""
            _BeauticiansProfessionalLiabilityFullTimeEmpNum = ""
            _BeauticiansProfessionalLiabilityPartTimeEmpNum = ""
            _BeauticiansProfessionalLiabilityDescription = ""
            _HasFuneralDirectorsProfessionalLiability = False
            _FuneralDirectorsProfessionalLiabilityQuotedPremium = ""
            _FuneralDirectorsProfessionalLiabilityEmpNum = ""
            _HasPrintersProfessionalLiability = False
            _PrintersProfessionalLiabilityQuotedPremium = ""
            _PrintersProfessionalLiabilityLocNum = ""
            _HasSelfStorageFacility = False
            _SelfStorageFacilityQuotedPremium = ""
            _SelfStorageFacilityLimit = ""
            _HasVeterinariansProfessionalLiability = False
            _VeterinariansProfessionalLiabilityEmpNum = ""
            _VeterinariansProfessionalLiabilityQuotedPremium = ""
            _HasPharmacistProfessionalLiability = False
            _PharmacistAnnualGrossSales = ""
            _PharmacistQuotedPremium = ""
            _HasOpticalAndHearingAidProfessionalLiability = False
            _OpticalAndHearingAidProfessionalLiabilityEmpNum = ""
            _OpticalAndHearingAidProfessionalLiabilityQuotedPremium = ""
            _HasMotelCoverage = False
            _MotelCoveragePerGuestLimitId = ""
            _MotelCoveragePerGuestLimit = ""
            _MotelCoveragePerGuestQuotedPremium = ""
            _MotelCoverageSafeDepositLimitId = ""
            _MotelCoverageSafeDepositDeductibleId = ""
            _MotelCoverageSafeDepositLimit = ""
            _MotelCoverageSafeDepositDeductible = ""
            _MotelCoverageQuotedPremium = ""
            _MotelCoverageSafeDepositQuotedPremium = ""
            _HasPhotographyCoverage = False
            _HasPhotographyCoverageScheduledCoverages = False
            _PhotographyScheduledCoverages = Nothing
            _HasPhotographyMakeupAndHair = False
            _PhotographyMakeupAndHairQuotedPremium = ""
            _PhotographyCoverageQuotedPremium = ""
            _HasResidentialCleaning = False
            _ResidentialCleaningQuotedPremium = ""
            _ProfessionalLiabilityCemetaryNumberOfBurials = ""
            _ProfessionalLiabilityCemetaryQuotedPremium = ""
            _ProfessionalLiabilityFuneralDirectorsNumberOfBodies = ""
            _ProfessionalLiabilityPastoralNumberOfClergy = ""
            _ProfessionalLiabilityPastoralQuotedPremium = ""
            '12/10/2018 - moved the remaining professional liability props that were missed 12/5/2018
            _HasApartmentBuildings = False
            _NumberOfLocationsWithApartments = ""
            _ApartmentQuotedPremium = ""
            _HasRestaurantEndorsement = False
            _RestaurantQuotedPremium = ""
            'added 12/18/2018 - moved from GoverningState object since they're specific to Location Buildings (and their parent state quotes)
            _ComputerCoinsuranceTypeId = "" 'cov also has CoverageBasisTypeId set to 1
            _ComputerExcludeEarthquake = False
            _ComputerValuationMethodTypeId = ""
            _ComputerAdditionalInterests = Nothing
            _ComputerCanUseAdditionalInterestNumForAdditionalInterestReconciliation = False
            _ComputerQuotedPremium = ""
            _ComputerAllPerilsDeductibleId = "" 'cov also has CoverageBasisTypeId set to 1
            _ComputerAllPerilsQuotedPremium = ""
            _ComputerEarthquakeVolcanicEruptionDeductible = "" 'cov also has CoverageBasisTypeId set to 1; example also has ApplyToWrittenPremiuim set to true
            _ComputerEarthquakeVolcanicEruptionQuotedPremium = ""
            _ComputerMechanicalBreakdownDeductible = "" 'cov also has CoverageBasisTypeId set to 1; example also has ApplyToWrittenPremiuim set to true
            _ComputerMechanicalBreakdownQuotedPremium = ""
            _FineArtsDeductibleCategoryTypeId = ""
            _FineArtsRate = ""
            _FineArtsDeductibleId = ""
            _FineArtsQuotedPremium = ""
            _FineArtsAdditionalInterests = Nothing
            _FineArtsCanUseAdditionalInterestNumForAdditionalInterestReconciliation = False
            _FineArtsBreakageMarringOrScratching = False 'renamed from _HasFineArtsBreakageMarringOrScratching
            _FineArtsBreakageMarringOrScratchingQuotedPremium = ""
            _SignsAdditionalInterests = Nothing
            _SignsCanUseAdditionalInterestNumForAdditionalInterestReconciliation = False
            _SignsMaximumDeductible = "" 'CoverageDetail
            _SignsMinimumDeductible = "" 'CoverageDetail
            _SignsValuationMethodTypeId = "" 'CoverageDetail; static data
            _SignsDeductibleId = "" 'static data
            _SignsQuotedPremium = ""
            _SignsAnyOneLossCatastropheLimit = "" 'note: cov also has CoverageBasisTypeId set to 1
            _SignsAnyOneLossCatastropheQuotedPremium = ""

            'added 10/24/2018
            _HasIllinoisContractorsHomeRepairAndRemodeling = False
            _IllinoisContractorsHomeRepairAndRemodelingManualLimitAmount = ""
            _IllinoisContractorsHomeRepairAndRemodelingQuotedPremium = ""

            'added 11/28/2018 for WCP IL (included in total premium)
            _CatastropheOtherThanCertifiedActsOfTerrorismQuotedPremium = ""

            'added 7/15/2019 for WCP KY
            _KentuckySpecialFundAssessmentQuotedPremium = ""
            _WCP_KY_PremSurcharge = ""

            _QuoteEffectiveDate = ""
            _QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.None
            _LobType = QuickQuoteObject.QuickQuoteLobType.None

            _UninsuredMotoristPropertyDamage_IL_QuotedPremium = ""
            _UninsuredMotoristPropertyDamage_IL_LimitId = ""
            _UninsuredMotoristPropertyDamage_IL_DeductibleId = ""

            _LiquorManufacturersSales = ""
            _LiquorRestaurantsSales = ""
            _LiquorPackageStoresSales = ""
            _LiquorClubsSales = ""

        End Sub
        Protected Friend Function Get_AdditionalInsuredsCount_Variable() As Integer
            Return _AdditionalInsuredsCount
        End Function
        Protected Friend Sub Set_AdditionalInsuredsCount_Variable(ByVal addlInsCount As Integer)
            _AdditionalInsuredsCount = addlInsCount
        End Sub
        Protected Friend Function Get_AdditionalInsuredsManualCharge_Variable() As String
            Return _AdditionalInsuredsManualCharge
        End Function
        Protected Friend Sub Set_AdditionalInsuredsManualCharge_Variable(ByVal addlInsManChg As String)
            _AdditionalInsuredsManualCharge = addlInsManChg
        End Sub
        Protected Friend Sub Set_AdditionalInsuredsBackup_Variable(ByVal addlInsBkp As List(Of QuickQuoteAdditionalInsured))
            _AdditionalInsuredsBackup = addlInsBkp
        End Sub
        'Protected Friend Sub Set_HasInclusionOfSoleProprietorsPartnersOfficersAndOthers_Variable(ByVal hasIt As Boolean)
        '    _HasInclusionOfSoleProprietorsPartnersOfficersAndOthers = hasIt
        'End Sub
        'Protected Friend Sub Set_HasWaiverOfSubrogation_Variable(ByVal hasIt As Boolean)
        '    _HasWaiverOfSubrogation = hasIt
        'End Sub
        Protected Friend Sub Set_HasExclusionOfAmishWorkers_Variable(ByVal hasIt As Boolean)
            _HasExclusionOfAmishWorkers = hasIt
        End Sub
        Protected Friend Sub Set_HasExclusionOfSoleProprietorsPartnersOfficersAndOthers_Variable(ByVal hasIt As Boolean)
            _HasExclusionOfSoleProprietorsPartnersOfficersAndOthers = hasIt
        End Sub
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
        Protected Friend Sub Set_ExclusionOfAmishWorkerRecordsBackup_Variable(ByVal exs As List(Of QuickQuoteExclusionOfAmishWorkerRecord))
            _ExclusionOfAmishWorkerRecordsBackup = exs
        End Sub
        Protected Friend Sub Set_ExclusionOfSoleProprietorRecordsBackup_Variable(ByVal exs As List(Of QuickQuoteExclusionOfSoleProprietorRecord))
            _ExclusionOfSoleProprietorRecordsBackup = exs
        End Sub
        'Protected Friend Sub Set_InclusionOfSoleProprietorRecordsBackup_Variable(ByVal incs As List(Of QuickQuoteInclusionOfSoleProprietorRecord))
        '    _InclusionOfSoleProprietorRecordsBackup = incs
        'End Sub
        'Protected Friend Sub Set_WaiverOfSubrogationRecordsBackup_Variable(ByVal ws As List(Of QuickQuoteWaiverOfSubrogationRecord))
        '    _WaiverOfSubrogationRecordsBackup = ws
        'End Sub
        Protected Friend Function Get_PremDiscountQuotedPremium_Variable() As String
            Return _PremDiscountQuotedPremium
        End Function

        'added 10/15/2018 for IL (similar to existing props w/o IL, but different form # and typeId)
        Protected Friend Sub Set_HasExclusionOfSoleProprietorsPartnersOfficersAndOthers_IL_Variable(ByVal hasIt As Boolean)
            _HasExclusionOfSoleProprietorsPartnersOfficersAndOthers_IL = hasIt
        End Sub
        Protected Friend Sub Set_ExclusionOfSoleProprietorRecordsBackup_IL_Variable(ByVal exs As List(Of QuickQuoteExclusionOfSoleProprietorRecord_IL))
            _ExclusionOfSoleProprietorRecordsBackup_IL = exs
        End Sub
        'added 4/26/2019 for KY
        Protected Friend Sub Set_HasKentuckyRejectionOfCoverageEndorsement_Variable(ByVal hasIt As Boolean)
            _HasKentuckyRejectionOfCoverageEndorsement = hasIt
        End Sub
        Protected Friend Sub Set_KentuckyRejectionOfCoverageEndorsementRecordsBackup_Variable(ByVal krces As List(Of QuickQuoteKentuckyRejectionOfCoverageEndorsement))
            _KentuckyRejectionOfCoverageEndorsementRecordsBackup = krces
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

        ' IDisposable
        'Protected Overridable Sub Dispose(disposing As Boolean)
        'updated w/ QuickQuoteBaseObject inheritance
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).


                    'PolicyLevel
                    _AdditionalInsuredsCount = Nothing
                    _AdditionalInsuredsCheckboxBOP = Nothing
                    _HasAdditionalInsuredsCheckboxBOP = Nothing
                    qqHelper.DisposeString(_AdditionalInsuredsManualCharge)
                    qqHelper.DisposeString(_AdditionalInsuredsQuotedPremium)
                    If _AdditionalInsureds IsNot Nothing Then
                        If _AdditionalInsureds.Count > 0 Then
                            For Each ai As QuickQuoteAdditionalInsured In _AdditionalInsureds
                                ai.Dispose()
                                ai = Nothing
                            Next
                            _AdditionalInsureds.Clear()
                        End If
                        _AdditionalInsureds = Nothing
                    End If
                    If _AdditionalInsuredsBackup IsNot Nothing Then
                        If _AdditionalInsuredsBackup.Count > 0 Then
                            For Each ai As QuickQuoteAdditionalInsured In _AdditionalInsuredsBackup
                                ai.Dispose()
                                ai = Nothing
                            Next
                            _AdditionalInsuredsBackup.Clear()
                        End If
                        _AdditionalInsuredsBackup = Nothing
                    End If
                    _HasExclusionOfAmishWorkers = Nothing
                    _HasExclusionOfSoleProprietorsPartnersOfficersAndOthers = Nothing
                    _LegalEntityTypeQuotedPremium = Nothing
                    _LegalEntityType = Nothing
                    '_HasInclusionOfSoleProprietorsPartnersOfficersAndOthers = Nothing
                    '_HasWaiverOfSubrogation = Nothing
                    '_WaiverOfSubrogationNumberOfWaivers = Nothing
                    'qqHelper.DisposeString(_WaiverOfSubrogationPremium)
                    'qqHelper.DisposeString(_WaiverOfSubrogationPremiumId)
                    '_NeedsToUpdateWaiverOfSubrogationPremiumId = Nothing
                    If _ExclusionOfAmishWorkerRecords IsNot Nothing Then
                        If _ExclusionOfAmishWorkerRecords.Count > 0 Then
                            For Each aw As QuickQuoteExclusionOfAmishWorkerRecord In _ExclusionOfAmishWorkerRecords
                                aw.Dispose()
                                aw = Nothing
                            Next
                            _ExclusionOfAmishWorkerRecords.Clear()
                        End If
                        _ExclusionOfAmishWorkerRecords = Nothing
                    End If
                    If _ExclusionOfSoleProprietorRecords IsNot Nothing Then
                        If _ExclusionOfSoleProprietorRecords.Count > 0 Then
                            For Each sp As QuickQuoteExclusionOfSoleProprietorRecord In _ExclusionOfSoleProprietorRecords
                                sp.Dispose()
                                sp = Nothing
                            Next
                            _ExclusionOfSoleProprietorRecords.Clear()
                        End If
                        _ExclusionOfSoleProprietorRecords = Nothing
                    End If
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
                    If _ExclusionOfAmishWorkerRecordsBackup IsNot Nothing Then
                        If _ExclusionOfAmishWorkerRecordsBackup.Count > 0 Then
                            For Each aw As QuickQuoteExclusionOfAmishWorkerRecord In _ExclusionOfAmishWorkerRecordsBackup
                                aw.Dispose()
                                aw = Nothing
                            Next
                            _ExclusionOfAmishWorkerRecordsBackup.Clear()
                        End If
                        _ExclusionOfAmishWorkerRecordsBackup = Nothing
                    End If
                    If _ExclusionOfSoleProprietorRecordsBackup IsNot Nothing Then
                        If _ExclusionOfSoleProprietorRecordsBackup.Count > 0 Then
                            For Each sp As QuickQuoteExclusionOfSoleProprietorRecord In _ExclusionOfSoleProprietorRecordsBackup
                                sp.Dispose()
                                sp = Nothing
                            Next
                            _ExclusionOfSoleProprietorRecordsBackup.Clear()
                        End If
                        _ExclusionOfSoleProprietorRecordsBackup = Nothing
                    End If
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
                    qqHelper.DisposeString(_Dec_BOP_OptCovs_Premium)
                    qqHelper.DisposeString(_ExpModQuotedPremium)
                    qqHelper.DisposeString(_ScheduleModQuotedPremium)
                    qqHelper.DisposeString(_TerrorismQuotedPremium)
                    qqHelper.DisposeString(_PremDiscountQuotedPremium)
                    qqHelper.DisposeString(_MinimumQuotedPremium)
                    qqHelper.DisposeString(_MinimumPremiumAdjustment)
                    qqHelper.DisposeString(_TotalEstimatedPlanPremium)
                    qqHelper.DisposeString(_SecondInjuryFundQuotedPremium)
                    qqHelper.DisposeString(_IL_WCP_CommissionOperationsFundSurcharge)
                    qqHelper.DisposeString(_Dec_LossConstantPremium)
                    qqHelper.DisposeString(_Dec_ExpenseConstantPremium)
                    qqHelper.DisposeString(_Dec_WC_TotalPremiumDue)
                    qqHelper.DisposeString(_Dec_GL_OptCovs_Premium)
                    qqHelper.DisposeString(_Dec_CAP_OptCovs_Premium)
                    qqHelper.DisposeString(_Dec_CAP_OptCovs_Premium_Without_GarageKeepers)
                    _HasConvertedCoverages = Nothing
                    _HasConvertedInclusionsExclusions = Nothing
                    _HasConvertedModifiers = Nothing
                    _HasConvertedScheduledRatings = Nothing
                    _CanUseExclusionNumForExclusionReconciliation = Nothing
                    _CanUseLossHistoryNumForLossHistoryReconciliation = Nothing
                    _CanUsePolicyUnderwritingNumForPolicyUnderwritingReconciliation = Nothing
                    _HasConvertedScheduledCoverages = Nothing
                    _CanUseScheduledCoverageNumForScheduledCoverageReconciliation = Nothing
                    _CanUseClassificationCodeNumForClassificationCodeReconciliation = Nothing
                    _HasConvertedFarmIncidentalLimitCoverages = Nothing
                    _HasConvertedScheduledPersonalPropertyCoverages = Nothing
                    _HasConvertedUnscheduledPersonalPropertyCoverages = Nothing
                    _CanUseScheduledFarmPersonalPropertyNumForScheduledPersonalPropertyReconciliation = Nothing
                    _CanUseUnscheduledFarmPersonalPropertyNumForUnscheduledPersonalPropertyReconciliation = Nothing
                    _HasConvertedOptionalCoverages = Nothing
                    _CanUseOptionalCoveragesNumForOptionalCoverageReconciliation = Nothing
                    _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = Nothing
                    qqHelper.DisposeString(_CPP_MinPremAdj_CPR) 'covCodeId 10121; note: covCodeId good for CGL, PIM (not in VR yet), FAR, CAP, WCP, GAR, BOP, CRM, CPR, CIM
                    qqHelper.DisposeString(_CPP_MinPremAdj_CGL) 'covCodeId 10121; note: covCodeId good for CGL, PIM (not in VR yet), FAR, CAP, WCP, GAR, BOP, CRM, CPR, CIM
                    qqHelper.DisposeString(_CPP_MinPremAdj_CIM) 'covCodeId 10121; note: covCodeId good for CGL, PIM (not in VR yet), FAR, CAP, WCP, GAR, BOP, CRM, CPR, CIM
                    qqHelper.DisposeString(_CPP_MinPremAdj_CRM) 'covCodeId 10121; note: covCodeId good for CGL, PIM (not in VR yet), FAR, CAP, WCP, GAR, BOP, CRM, CPR, CIM
                    qqHelper.DisposeString(_CPP_MinPremAdj_GAR) 'covCodeId 10121; note: covCodeId good for CGL, PIM (not in VR yet), FAR, CAP, WCP, GAR, BOP, CRM, CPR, CIM
                    qqHelper.DisposeString(_CAP_GAR_PolicyLevelCovs_Premium)
                    'qqHelper.DisposeString(_WCP_WaiverPremium) 'covCodeId 10124 CovAddInfo w/ "Waiver Premium" in desc
                    'added 10/15/2018 for IL (similar to existing props w/o IL, but different form # and typeId)
                    _HasExclusionOfSoleProprietorsPartnersOfficersAndOthers_IL = Nothing
                    If _ExclusionOfSoleProprietorRecords_IL IsNot Nothing Then
                        If _ExclusionOfSoleProprietorRecords_IL.Count > 0 Then
                            For Each sp As QuickQuoteExclusionOfSoleProprietorRecord_IL In _ExclusionOfSoleProprietorRecords_IL
                                sp.Dispose()
                                sp = Nothing
                            Next
                            _ExclusionOfSoleProprietorRecords_IL.Clear()
                        End If
                        _ExclusionOfSoleProprietorRecords_IL = Nothing
                    End If
                    If _ExclusionOfSoleProprietorRecordsBackup_IL IsNot Nothing Then
                        If _ExclusionOfSoleProprietorRecordsBackup_IL.Count > 0 Then
                            For Each sp As QuickQuoteExclusionOfSoleProprietorRecord_IL In _ExclusionOfSoleProprietorRecordsBackup_IL
                                sp.Dispose()
                                sp = Nothing
                            Next
                            _ExclusionOfSoleProprietorRecordsBackup_IL.Clear()
                        End If
                        _ExclusionOfSoleProprietorRecordsBackup_IL = Nothing
                    End If
                    'added 4/26/2019 for KY
                    _HasKentuckyRejectionOfCoverageEndorsement = Nothing
                    If _KentuckyRejectionOfCoverageEndorsementRecords IsNot Nothing Then
                        If _KentuckyRejectionOfCoverageEndorsementRecords.Count > 0 Then
                            For Each krce As QuickQuoteKentuckyRejectionOfCoverageEndorsement In _KentuckyRejectionOfCoverageEndorsementRecords
                                If krce IsNot Nothing Then
                                    krce.Dispose()
                                    krce = Nothing
                                End If
                            Next
                            _KentuckyRejectionOfCoverageEndorsementRecords.Clear()
                        End If
                        _KentuckyRejectionOfCoverageEndorsementRecords = Nothing
                    End If
                    If _KentuckyRejectionOfCoverageEndorsementRecordsBackup IsNot Nothing Then
                        If _KentuckyRejectionOfCoverageEndorsementRecordsBackup.Count > 0 Then
                            For Each krce As QuickQuoteKentuckyRejectionOfCoverageEndorsement In _KentuckyRejectionOfCoverageEndorsementRecordsBackup
                                If krce IsNot Nothing Then
                                    krce.Dispose()
                                    krce = Nothing
                                End If
                            Next
                            _KentuckyRejectionOfCoverageEndorsementRecordsBackup.Clear()
                        End If
                        _KentuckyRejectionOfCoverageEndorsementRecordsBackup = Nothing
                    End If

                    'added 10/19/2018 - moved from AlliedToAllStates object
                    _HasLiquorLiability = False
                    qqHelper.DisposeString(_LiquorLiabilityClassCodeTypeId) '12 = 58161 - Restaurant Includes Package Sales, 13 = 59211 - Package Sales for Consumption Off Premises
                    qqHelper.DisposeString(_LiquorLiabilityAnnualGrossPackageSalesReceipts)
                    qqHelper.DisposeString(_LiquorLiabilityAnnualGrossAlcoholSalesReceipts)
                    qqHelper.DisposeString(_LiquorLiabilityOccurrenceLimit)
                    qqHelper.DisposeString(_LiquorLiabilityOccurrenceLimitId)
                    qqHelper.DisposeString(_LiquorLiabilityClassification)
                    qqHelper.DisposeString(_LiquorLiabilityClassificationId)
                    qqHelper.DisposeString(_LiquorSales)
                    qqHelper.DisposeString(_LiquorLiabilityQuotedPremium)
                    '12/5/2018 - moved the rest of the professional liability props stuff
                    _HasBarbersProfessionalLiability = False
                    qqHelper.DisposeString(_BarbersProfessionalLiabiltyQuotedPremium)
                    qqHelper.DisposeString(_BarbersProfessionalLiabilityFullTimeEmpNum)
                    qqHelper.DisposeString(_BarbersProfessionalLiabilityPartTimeEmpNum)
                    qqHelper.DisposeString(_BarbersProfessionalLiabilityDescription)
                    _HasBeauticiansProfessionalLiability = False
                    qqHelper.DisposeString(_BeauticiansProfessionalLiabilityQuotedPremium)
                    qqHelper.DisposeString(_BeauticiansProfessionalLiabilityFullTimeEmpNum)
                    qqHelper.DisposeString(_BeauticiansProfessionalLiabilityPartTimeEmpNum)
                    qqHelper.DisposeString(_BeauticiansProfessionalLiabilityDescription)
                    _HasFuneralDirectorsProfessionalLiability = False
                    qqHelper.DisposeString(_FuneralDirectorsProfessionalLiabilityQuotedPremium)
                    qqHelper.DisposeString(_FuneralDirectorsProfessionalLiabilityEmpNum)
                    _HasPrintersProfessionalLiability = False
                    qqHelper.DisposeString(_PrintersProfessionalLiabilityQuotedPremium)
                    qqHelper.DisposeString(_PrintersProfessionalLiabilityLocNum)
                    _HasSelfStorageFacility = False
                    qqHelper.DisposeString(_SelfStorageFacilityQuotedPremium)
                    qqHelper.DisposeString(_SelfStorageFacilityLimit)
                    _HasVeterinariansProfessionalLiability = False
                    qqHelper.DisposeString(_VeterinariansProfessionalLiabilityEmpNum)
                    qqHelper.DisposeString(_VeterinariansProfessionalLiabilityQuotedPremium)
                    _HasPharmacistProfessionalLiability = False
                    qqHelper.DisposeString(_PharmacistAnnualGrossSales)
                    qqHelper.DisposeString(_PharmacistQuotedPremium)
                    _HasOpticalAndHearingAidProfessionalLiability = False
                    qqHelper.DisposeString(_OpticalAndHearingAidProfessionalLiabilityEmpNum)
                    qqHelper.DisposeString(_OpticalAndHearingAidProfessionalLiabilityQuotedPremium)
                    _HasMotelCoverage = False
                    qqHelper.DisposeString(_MotelCoveragePerGuestLimitId)
                    qqHelper.DisposeString(_MotelCoveragePerGuestLimit)
                    qqHelper.DisposeString(_MotelCoveragePerGuestQuotedPremium)
                    qqHelper.DisposeString(_MotelCoverageSafeDepositLimitId)
                    qqHelper.DisposeString(_MotelCoverageSafeDepositDeductibleId)
                    qqHelper.DisposeString(_MotelCoverageSafeDepositLimit)
                    qqHelper.DisposeString(_MotelCoverageSafeDepositDeductible)
                    qqHelper.DisposeString(_MotelCoverageQuotedPremium)
                    qqHelper.DisposeString(_MotelCoverageSafeDepositQuotedPremium)
                    _HasPhotographyCoverage = False
                    _HasPhotographyCoverageScheduledCoverages = False
                    If _PhotographyScheduledCoverages IsNot Nothing Then
                        If _PhotographyScheduledCoverages.Count > 0 Then
                            For Each qqc As QuickQuoteCoverage In _PhotographyScheduledCoverages
                                qqc.Dispose()
                                qqc = Nothing
                            Next
                            _PhotographyScheduledCoverages.Clear()
                        End If
                        _PhotographyScheduledCoverages = Nothing
                    End If
                    _HasPhotographyMakeupAndHair = False
                    qqHelper.DisposeString(_PhotographyMakeupAndHairQuotedPremium)
                    qqHelper.DisposeString(_PhotographyCoverageQuotedPremium)
                    _HasResidentialCleaning = False
                    qqHelper.DisposeString(_ResidentialCleaningQuotedPremium)
                    qqHelper.DisposeString(_ProfessionalLiabilityCemetaryNumberOfBurials)
                    qqHelper.DisposeString(_ProfessionalLiabilityCemetaryQuotedPremium)
                    qqHelper.DisposeString(_ProfessionalLiabilityFuneralDirectorsNumberOfBodies)
                    qqHelper.DisposeString(_ProfessionalLiabilityPastoralNumberOfClergy)
                    qqHelper.DisposeString(_ProfessionalLiabilityPastoralQuotedPremium)
                    '12/10/2018 - moved the remaining professional liability props that were missed 12/5/2018
                    _HasApartmentBuildings = Nothing
                    qqHelper.DisposeString(_NumberOfLocationsWithApartments)
                    qqHelper.DisposeString(_ApartmentQuotedPremium)
                    _HasRestaurantEndorsement = Nothing
                    qqHelper.DisposeString(_RestaurantQuotedPremium)
                    'added 12/18/2018 - moved from GoverningState object since they're specific to Location Buildings (and their parent state quotes)
                    qqHelper.DisposeString(_ComputerCoinsuranceTypeId) 'cov also has CoverageBasisTypeId set to 1
                    _ComputerExcludeEarthquake = Nothing
                    qqHelper.DisposeString(_ComputerValuationMethodTypeId)
                    qqHelper.DisposeAdditionalInterests(_ComputerAdditionalInterests)
                    _ComputerCanUseAdditionalInterestNumForAdditionalInterestReconciliation = Nothing
                    qqHelper.DisposeString(_ComputerQuotedPremium)
                    qqHelper.DisposeString(_ComputerAllPerilsDeductibleId) 'cov also has CoverageBasisTypeId set to 1
                    qqHelper.DisposeString(_ComputerAllPerilsQuotedPremium)
                    qqHelper.DisposeString(_ComputerEarthquakeVolcanicEruptionDeductible) 'cov also has CoverageBasisTypeId set to 1; example also has ApplyToWrittenPremiuim set to true
                    qqHelper.DisposeString(_ComputerEarthquakeVolcanicEruptionQuotedPremium)
                    qqHelper.DisposeString(_ComputerMechanicalBreakdownDeductible) 'cov also has CoverageBasisTypeId set to 1; example also has ApplyToWrittenPremiuim set to true
                    qqHelper.DisposeString(_ComputerMechanicalBreakdownQuotedPremium)
                    qqHelper.DisposeString(_FineArtsDeductibleCategoryTypeId)
                    qqHelper.DisposeString(_FineArtsRate)
                    qqHelper.DisposeString(_FineArtsDeductibleId)
                    qqHelper.DisposeString(_FineArtsQuotedPremium)
                    qqHelper.DisposeAdditionalInterests(_FineArtsAdditionalInterests)
                    _FineArtsCanUseAdditionalInterestNumForAdditionalInterestReconciliation = Nothing
                    _FineArtsBreakageMarringOrScratching = Nothing 'renamed from _HasFineArtsBreakageMarringOrScratching
                    qqHelper.DisposeString(_FineArtsBreakageMarringOrScratchingQuotedPremium)
                    qqHelper.DisposeAdditionalInterests(_SignsAdditionalInterests)
                    _SignsCanUseAdditionalInterestNumForAdditionalInterestReconciliation = Nothing
                    qqHelper.DisposeString(_SignsMaximumDeductible) 'CoverageDetail
                    qqHelper.DisposeString(_SignsMinimumDeductible) 'CoverageDetail
                    qqHelper.DisposeString(_SignsValuationMethodTypeId) 'CoverageDetail; static data
                    qqHelper.DisposeString(_SignsDeductibleId) 'static data
                    qqHelper.DisposeString(_SignsQuotedPremium)
                    qqHelper.DisposeString(_SignsAnyOneLossCatastropheLimit) 'note: cov also has CoverageBasisTypeId set to 1
                    qqHelper.DisposeString(_SignsAnyOneLossCatastropheQuotedPremium)

                    'added 10/24/2018
                    _HasIllinoisContractorsHomeRepairAndRemodeling = Nothing
                    qqHelper.DisposeString(_IllinoisContractorsHomeRepairAndRemodelingManualLimitAmount)
                    qqHelper.DisposeString(_IllinoisContractorsHomeRepairAndRemodelingQuotedPremium)

                    'added 11/28/2018 for WCP IL (included in total premium)
                    qqHelper.DisposeString(_CatastropheOtherThanCertifiedActsOfTerrorismQuotedPremium)

                    'added 7/15/2019 for WCP KY
                    qqHelper.DisposeString(_KentuckySpecialFundAssessmentQuotedPremium)
                    qqHelper.DisposeString(_WCP_KY_PremSurcharge)

                    qqHelper.DisposeString(_QuoteEffectiveDate)
                    _QuoteTransactionType = Nothing
                    _LobType = Nothing

                    If _BasePolicyLevelInfo IsNot Nothing Then
                        _BasePolicyLevelInfo.Dispose()
                        _BasePolicyLevelInfo = Nothing
                    End If

                    qqHelper.DisposeString(_UninsuredMotoristPropertyDamage_IL_QuotedPremium)
                    qqHelper.DisposeString(_UninsuredMotoristPropertyDamage_IL_LimitId)
                    qqHelper.DisposeString(_UninsuredMotoristPropertyDamage_IL_DeductibleId)

                    qqHelper.DisposeString(_LiquorManufacturersSales)
                    qqHelper.DisposeString(_LiquorRestaurantsSales)
                    qqHelper.DisposeString(_LiquorPackageStoresSales)
                    qqHelper.DisposeString(_LiquorClubsSales)

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
