Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store vehicle information
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class QuickQuoteVehicle 'added 8/29/2012
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _PolicyId As String
        Private _PolicyImageNum As String
        Private _AnnualMiles As String
        Private _ClassCode As String
        Private _CostNew As String
        Private _CostUsed As String
        Private _Coverages As Generic.List(Of QuickQuoteCoverage)
        Private _DaysPerWeek As String
        Private _Make As String
        Private _MilesOneWay As String
        Private _Model As String
        Private _OdometerReading As String
        Private _Vin As String
        Private _Year As String
        Private _GaragingAddress As QuickQuoteGaragingAddress
        'added 8/30/2012
        Private _HasLiability_UM_UIM As Boolean
        Private _Liability_UM_UIM_QuotedPremium As String
        Private _Liability_UM_UIM_LimitId As String 'added 7/24/2013 for PPA; may need matching Limit variable/property
        Private _HasMedicalPayments As Boolean
        Private _MedicalPaymentsQuotedPremium As String
        Private _MedicalPaymentsLimitId As String 'added 7/24/2013 for PPA; may need matching Limit variable/property
        Private _HasComprehensive As Boolean
        Private _ComprehensiveDeductible As String
        Private _ComprehensiveDeductibleId As String
        Private _ComprehensiveQuotedPremium As String
        Private _ComprehensiveDeductibleLimitId As String 'added 7/24/2013 for PPA; may need matching Limit variable/property
        Private _HasCollision As Boolean
        Private _CollisionDeductible As String
        Private _CollisionDeductibleId As String
        Private _CollisionQuotedPremium As String
        Private _CollisionDeductibleLimitId As String 'added 7/25/2013 for PPA; may need matching Limit variable/property
        'added 8/31/2012
        Private _HasTowingAndLabor As Boolean
        Private _TowingAndLaborQuotedPremium As String
        Private _TowingAndLaborDeductibleLimitId As String 'added 7/25/2013 for PPA; may need matching Limit variable/property
        Private _HasRentalReimbursement As Boolean
        Private _RentalReimbursementDailyReimbursement As String
        Private _RentalReimbursementNumberOfDays As String
        Private _RentalReimbursementQuotedPremium As String

        'added 9/24/2012
        Private _PremiumFullTerm As String
        Private _TerritoryNum As String

        'added 9/25/2012
        Private _VehicleRatingTypeId As String
        Private _VehicleUseTypeId As String
        Private _VehicleUsageTypeId As String
        Private _FarmUseCodeTypeId As String
        Private _UseCodeTypeId As String
        Private _OperatorTypeId As String
        Private _OperatorUseTypeId As String
        Private _RadiusTypeId As String
        Private _SizeTypeId As String
        Private _SecondaryClassTypeId As String
        Private _SecondaryClassUsageTypeId As String
        Private _UsedInDumping As Boolean 'added 9/26/2012

        Private _AdditionalInterests As Generic.List(Of QuickQuoteAdditionalInterest) 'added 9/26/2012

        'added 10/3/2012 for declarations section
        Private _UninsuredMotoristLiabilityQuotedPremium As String
        Private _UninsuredMotoristLiabilityLimitId As String 'added 7/24/2013 for PPA; may need matching Limit variable/property
        Private _UnderinsuredMotoristBodilyInjuryLiabilityQuotedPremium As String

        'added 10/9/2012 for CAP (more class code lookup stuff)
        Private _TrailerTypeId As String
        'added 10/11/2012 for CAP (class code lookup again)
        Private _StatedAmount As String

        'added 7/24/2013 for PPA
        Private _PerformanceTypeId As String
        Private _BodyTypeId As String
        Private _PrincipalDriverNum As String
        Private _OccasionalDriver1Num As String
        Private _OccasionalDriver2Num As String
        Private _OccasionalDriver3Num As String
        Private _BodilyInjuryLiabilityLimitId As String 'may need matching Limit variable/property
        Private _BodilyInjuryLiabilityQuotedPremium As String '7/25/2013
        Private _PropertyDamageLimitId As String 'may need matching Limit variable/property
        Private _PropertyDamageQuotedPremium As String '7/25/2013
        Private _UninsuredCombinedSingleLimitId As String 'may need matching Limit variable/property
        Private _UninsuredCombinedSingleQuotedPremium As String '7/25/2013
        Private _UninsuredMotoristPropertyDamageLimitId As String 'may need matching Limit variable/property
        Private _UninsuredMotoristPropertyDamageQuotedPremium As String '7/25/2013
        Private _HasUninsuredMotoristPropertyDamage As Boolean 'added 10/30/2018 for CAP IL; covCodeId 9; CheckBox
        Private _UninsuredMotoristPropertyDamageDeductibleLimitId As String 'may need matching Limit variable/property
        Private _UninsuredMotoristPropertyDamageDeductibleQuotedPremium As String '7/25/2013
        'added 7/25/2013 for PPA
        Private _HasPollutionLiabilityBroadenedCoverage As Boolean
        Private _PollutionLiabilityBroadenedCoverageQuotedPremium As String
        Private _TransportationExpenseLimitId As String 'may need matching Limit variable/property
        Private _TransportationExpenseQuotedPremium As String
        Private _HasAutoLoanOrLease As Boolean
        Private _AutoLoanOrLeaseQuotedPremium As String
        Private _TapesAndRecordsLimitId As String 'may need matching Limit variable/property
        Private _TapesAndRecordsQuotedPremium As String
        Private _SoundEquipmentLimit As String
        Private _SoundEquipmentQuotedPremium As String
        Private _ElectronicEquipmentLimit As String
        Private _ElectronicEquipmentQuotedPremium As String
        Private _TripInterruptionLimitId As String 'may need matching Limit variable/property
        Private _TripInterruptionQuotedPremium As String
        'added 7/26/2013 for PPA
        Private _ScheduledItems As List(Of QuickQuoteScheduledItem)
        'added 7/30/2013 for PPA
        Private _AntiLockTypeId As String 'may need matching AntiLockType variable/property
        'added 8/7/2013 for PPA
        Private _ActualCashValue As String
        Private _AntiTheftTypeId As String 'may need matching AntiTheftType variable/property
        Private _CubicCentimeters As String 'Horsepower/CC's in UI
        Private _CustomEquipmentAmount As String
        Private _DamageRemarks As String
        Private _DamageYesNoId As String 'may need matching DamageYesNo variable/property
        Private _DriverOutOfStateSurcharge As Boolean
        Private _GrossVehicleWeight As String 'GVW in UI
        Private _MultiCar As Boolean
        Private _NonOwned As Boolean 'Extended Non-Owned in UI
        Private _NonOwnedNamed As Boolean 'Named Non-Owned Non-Specific Vehicle in UI
        Private _PurchasedDate As String
        Private _RegisteredStateId As String 'may need matching RegisteredState variable/property
        Private _RestraintTypeId As String 'may need matching RestraintType variable/property
        'maybe add VehicleSymbols
        Private _VehicleTypeId As String 'Motorcycle Type in UI; may need matching VehicleType variable/property
        Private _VehicleValueId As String 'uses VehicleValueType table; may need matching VehicleValue variable/property

        'added 2/18/2014
        Private _HasConvertedCoverages As Boolean

        'added 3/26/2014
        Private _ComprehensiveCoverageOnly As Boolean

        'added 4/15/2014
        Private _VehicleSymbols As List(Of QuickQuoteVehicleSymbol)

        Private _VehicleNum As String 'added 4/21/2014 for reconciliation purposes
        Private _HasVehicleMakeModelYearChanged As Boolean 'added 4/21/2014
        Private _CanUseVehicleSymbolNumForVehicleSymbolReconciliation As Boolean 'added 4/24/2014
        Private _CanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean 'added 4/29/2014
        Private _CanUseScheduledItemsNumForScheduledItemReconciliation As Boolean 'added 5/14/2014
        'added 10/17/2018 for multi-state
        Private _VehicleNum_MasterPart As String
        Private _VehicleNum_CGLPart As String
        Private _VehicleNum_CPRPart As String
        Private _VehicleNum_CIMPart As String
        Private _VehicleNum_CRMPart As String
        Private _VehicleNum_GARPart As String

        Private _Modifiers As List(Of QuickQuoteModifier) 'added 10/16/2014

        'added 5/26/2017
        Private _TotalCoveragesPremium As String
        Private _CAP_GAR_TotalCoveragesPremium As String

        'added 8/2/2018
        Private _QuoteStateTakenFrom As QuickQuoteHelperClass.QuickQuoteState

        'added 9/28/2018
        Private _UnderinsuredCombinedSingleLimitId As String 'covCodeId 296; PPA IL only
        Private _UnderinsuredCombinedSingleLimitQuotedPremium As String 'covCodeId 296; PPA IL only
        Private _UninsuredBodilyInjuryLimitId As String 'covCodeId 294; PPA IL, HOM IN, DFR IN, FAR IN/IL
        Private _UninsuredBodilyInjuryQuotedPremium As String 'covCodeId 294; PPA IL, HOM IN, DFR IN, FAR IN/IL
        Private _UnderinsuredBodilyInjuryLimitId As String 'covCodeId 295; PPA IL only
        Private _UnderinsuredBodilyInjuryQuotedPremium As String 'covCodeId 295; PPA IL only

        'added 1/16/2019
        Private _DisplayNum As Integer
        Private _OriginalDisplayNum As Integer
        Private _OkayToUseDisplayNum As QuickQuoteHelperClass.WhenToSetType

        Private _DetailStatusCode As String 'added 5/15/2019

        'added 5/22/2019
        Private _AddedDate As String
        Private _EffectiveDate As String
        Private _LastModifiedDate As String
        Private _PCAdded_Date As String
        Private _AddedImageNum As String 'added 7/31/2019

        Private _UnderinsuredMotoristBodilyInjuryLiabilityLimitId As String

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
        Public Property AnnualMiles As String
            Get
                Return _AnnualMiles
            End Get
            Set(value As String)
                _AnnualMiles = value
            End Set
        End Property
        Public Property ClassCode As String
            Get
                Return _ClassCode
            End Get
            Set(value As String)
                _ClassCode = value
            End Set
        End Property
        Public Property CostNew As String
            Get
                Return _CostNew
                'updated 8/26/2014; won't use for now
                'Return qqHelper.QuotedPremiumFormat(_CostNew)
            End Get
            Set(value As String)
                _CostNew = value
                qqHelper.ConvertToQuotedPremiumFormat(_CostNew)
            End Set
        End Property
        Public Property CostUsed As String
            Get
                Return _CostUsed
                'updated 8/26/2014; won't use for now
                'Return qqHelper.QuotedPremiumFormat(_CostUsed)
            End Get
            Set(value As String)
                _CostUsed = value
                qqHelper.ConvertToQuotedPremiumFormat(_CostUsed)
            End Set
        End Property
        Public Property Coverages As Generic.List(Of QuickQuoteCoverage)
            Get
                SetParentOfListItems(_Coverages, "{663B7C7B-F2AC-4BF6-965A-D30F41A05660}")
                Return _Coverages
            End Get
            Set(value As Generic.List(Of QuickQuoteCoverage))
                _Coverages = value
                SetParentOfListItems(_Coverages, "{663B7C7B-F2AC-4BF6-965A-D30F41A05660}")
            End Set
        End Property
        Public Property DaysPerWeek As String
            Get
                Return _DaysPerWeek
            End Get
            Set(value As String)
                _DaysPerWeek = value
            End Set
        End Property
        Public Property Make As String
            Get
                Return _Make
            End Get
            Set(value As String)
                _Make = value
            End Set
        End Property
        Public Property MilesOneWay As String
            Get
                Return _MilesOneWay
            End Get
            Set(value As String)
                _MilesOneWay = value
            End Set
        End Property
        Public Property Model As String
            Get
                Return _Model
            End Get
            Set(value As String)
                _Model = value
            End Set
        End Property
        Public Property OdometerReading As String
            Get
                Return _OdometerReading
            End Get
            Set(value As String)
                _OdometerReading = value
            End Set
        End Property
        Public Property Vin As String
            Get
                Return _Vin
            End Get
            Set(value As String)
                _Vin = value
            End Set
        End Property
        Public Property Year As String
            Get
                Return _Year
            End Get
            Set(value As String)
                _Year = value
            End Set
        End Property
        Public Property GaragingAddress As QuickQuoteGaragingAddress
            Get
                SetObjectsParent(_GaragingAddress)
                Return _GaragingAddress
            End Get
            Set(value As QuickQuoteGaragingAddress)
                _GaragingAddress = value
                SetObjectsParent(_GaragingAddress)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 2</remarks>
        Public Property HasLiability_UM_UIM As Boolean
            Get
                Return _HasLiability_UM_UIM
            End Get
            Set(value As Boolean)
                _HasLiability_UM_UIM = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 2</remarks>
        Public Property Liability_UM_UIM_QuotedPremium As String
            Get
                'Return _Liability_UM_UIM_QuotedPremium
                'updated 8/26/2014
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
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 2</remarks>
        Public Property Liability_UM_UIM_LimitId As String 'added 7/24/2013: CoverageLimit table... 100,000=10; N/A=0
            Get
                Return _Liability_UM_UIM_LimitId
            End Get
            Set(value As String)
                _Liability_UM_UIM_LimitId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 60006 (CAP) or 6 (PPA)</remarks>
        Public Property HasMedicalPayments As Boolean
            Get
                Return _HasMedicalPayments
            End Get
            Set(value As Boolean)
                _HasMedicalPayments = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 60006 (CAP) or 6 (PPA)</remarks>
        Public Property MedicalPaymentsQuotedPremium As String
            Get
                'Return _MedicalPaymentsQuotedPremium
                'updated 8/26/2014
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
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 60006 (CAP) or 6 (PPA)</remarks>
        Public Property MedicalPaymentsLimitId As String 'added 7/24/2013: CoverageLimit table... 1,000=170; N/A=0
            Get
                Return _MedicalPaymentsLimitId
            End Get
            Set(value As String)
                _MedicalPaymentsLimitId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 3</remarks>
        Public Property HasComprehensive As Boolean
            Get
                Return _HasComprehensive
            End Get
            Set(value As Boolean)
                _HasComprehensive = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 3</remarks>
        Public Property ComprehensiveDeductible As String 'currently matching Deductible table (8/30/2012)
            Get
                Return _ComprehensiveDeductible
            End Get
            Set(value As String)
                _ComprehensiveDeductible = value
                Select Case _ComprehensiveDeductible
                    Case "N/A"
                        _ComprehensiveDeductibleId = "0"
                    Case "0"
                        _ComprehensiveDeductibleId = "40"
                    Case "50"
                        _ComprehensiveDeductibleId = "1"
                    Case "100"
                        _ComprehensiveDeductibleId = "2"
                    Case "250"
                        _ComprehensiveDeductibleId = "4"
                    Case "500"
                        _ComprehensiveDeductibleId = "8"
                    Case "1,000"
                        _ComprehensiveDeductibleId = "9"
                    Case "2,000"
                        _ComprehensiveDeductibleId = "28"
                    Case "3,000"
                        _ComprehensiveDeductibleId = "29"
                    Case "No Deductible" 'added 6/20/2017 for Diamond Proposals
                        _ComprehensiveDeductibleId = "11"
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
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 3</remarks>
        Public Property ComprehensiveDeductibleId As String 'currently matching Deductible table (8/30/2012)
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
                        Case "40"
                            _ComprehensiveDeductible = "0"
                        Case "1"
                            _ComprehensiveDeductible = "50"
                        Case "2"
                            _ComprehensiveDeductible = "100"
                        Case "4"
                            _ComprehensiveDeductible = "250"
                        Case "8"
                            _ComprehensiveDeductible = "500"
                        Case "9"
                            _ComprehensiveDeductible = "1,000"
                        Case "28"
                            _ComprehensiveDeductible = "2,000"
                        Case "29"
                            _ComprehensiveDeductible = "3,000"
                        Case "11" 'added 6/20/2017 for Diamond Proposals
                            _ComprehensiveDeductible = "No Deductible"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 3</remarks>
        Public Property ComprehensiveQuotedPremium As String
            Get
                'Return _ComprehensiveQuotedPremium
                'updated 8/26/2014
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
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 3</remarks>
        Public Property ComprehensiveDeductibleLimitId As String 'added 7/24/2013: CoverageLimit table... 100=18
            Get
                Return _ComprehensiveDeductibleLimitId
            End Get
            Set(value As String)
                _ComprehensiveDeductibleLimitId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 5</remarks>
        Public Property HasCollision As Boolean
            Get
                Return _HasCollision
            End Get
            Set(value As Boolean)
                _HasCollision = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 5</remarks>
        Public Property CollisionDeductible As String 'currently matching Deductible table (8/30/2012)
            Get
                Return _CollisionDeductible
            End Get
            Set(value As String)
                _CollisionDeductible = value
                Select Case _CollisionDeductible
                    Case "N/A"
                        _CollisionDeductibleId = "0"
                    Case "50"
                        _CollisionDeductibleId = "1"
                    Case "100"
                        _CollisionDeductibleId = "2"
                    Case "200"
                        _CollisionDeductibleId = "3"
                    Case "250"
                        _CollisionDeductibleId = "4"
                    Case "500"
                        _CollisionDeductibleId = "8"
                    Case "1,000"
                        _CollisionDeductibleId = "9"
                    Case "2,000"
                        _CollisionDeductibleId = "28"
                    Case "3,000"
                        _CollisionDeductibleId = "29"
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
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 5</remarks>
        Public Property CollisionDeductibleId As String 'currently matching Deductible table (8/30/2012)
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
                        Case "1"
                            _CollisionDeductible = "50"
                        Case "2"
                            _CollisionDeductible = "100"
                        Case "3"
                            _CollisionDeductible = "200"
                        Case "4"
                            _CollisionDeductible = "250"
                        Case "8"
                            _CollisionDeductible = "500"
                        Case "9"
                            _CollisionDeductible = "1,000"
                        Case "28"
                            _CollisionDeductible = "2,000"
                        Case "29"
                            _CollisionDeductible = "3,000"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 5</remarks>
        Public Property CollisionQuotedPremium As String
            Get
                'Return _CollisionQuotedPremium
                'updated 8/26/2014
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
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 5</remarks>
        Public Property CollisionDeductibleLimitId As String 'added 7/25/2013: CoverageLimit table... 200=20
            Get
                Return _CollisionDeductibleLimitId
            End Get
            Set(value As String)
                _CollisionDeductibleLimitId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 60008</remarks>
        Public Property HasTowingAndLabor As Boolean
            Get
                Return _HasTowingAndLabor
            End Get
            Set(value As Boolean)
                _HasTowingAndLabor = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 60008</remarks>
        Public Property TowingAndLaborQuotedPremium As String
            Get
                'Return _TowingAndLaborQuotedPremium
                'updated 8/26/2014
                Return qqHelper.QuotedPremiumFormat(_TowingAndLaborQuotedPremium)
            End Get
            Set(value As String)
                _TowingAndLaborQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_TowingAndLaborQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 60008</remarks>
        Public Property TowingAndLaborDeductibleLimitId As String 'added 7/25/2013: CoverageLimit table... 25=27; 50=41
            Get
                Return _TowingAndLaborDeductibleLimitId
            End Get
            Set(value As String)
                _TowingAndLaborDeductibleLimitId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverages w/ coveragecode_ids 10094 (Comp) and 10095 (Coll)</remarks>
        Public Property HasRentalReimbursement As Boolean
            Get
                Return _HasRentalReimbursement
            End Get
            Set(value As Boolean)
                _HasRentalReimbursement = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverages w/ coveragecode_ids 10094 (Comp) and 10095 (Coll)</remarks>
        Public Property RentalReimbursementDailyReimbursement As String
            Get
                Return _RentalReimbursementDailyReimbursement
            End Get
            Set(value As String)
                _RentalReimbursementDailyReimbursement = value
                'qqHelper.ConvertToQuotedPremiumFormat(_RentalReimbursementDailyReimbursement)'not used yet
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverages w/ coveragecode_ids 10094 (Comp) and 10095 (Coll)</remarks>
        Public Property RentalReimbursementNumberOfDays As String
            Get
                Return _RentalReimbursementNumberOfDays
            End Get
            Set(value As String)
                _RentalReimbursementNumberOfDays = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverages w/ coveragecode_ids 10094 (Comp) and 10095 (Coll)</remarks>
        Public Property RentalReimbursementQuotedPremium As String
            Get
                'Return _RentalReimbursementQuotedPremium
                'updated 8/26/2014
                Return qqHelper.QuotedPremiumFormat(_RentalReimbursementQuotedPremium)
            End Get
            Set(value As String)
                _RentalReimbursementQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_RentalReimbursementQuotedPremium)
            End Set
        End Property

        Public Property PremiumFullTerm As String
            Get
                'Return _PremiumFullTerm
                'updated 8/26/2014
                Return qqHelper.QuotedPremiumFormat(_PremiumFullTerm)
            End Get
            Set(value As String)
                _PremiumFullTerm = value
                qqHelper.ConvertToQuotedPremiumFormat(_PremiumFullTerm)
            End Set
        End Property
        Public Property TerritoryNum As String
            Get
                Return _TerritoryNum
            End Get
            Set(value As String)
                _TerritoryNum = value
            End Set
        End Property

        Public Property VehicleRatingTypeId As String '0=N/A; 1=Private Passenger Type; etc. (VehicleRatingType table)
            Get
                Return _VehicleRatingTypeId
            End Get
            Set(value As String)
                _VehicleRatingTypeId = value
            End Set
        End Property
        Public Property VehicleUseTypeId As String '-1=N/A; 0=None; 1=Pleasure; 2=Business; 3=Work; 4=Farm; 5=School; 6=Personal; 7=Service; 8=Retail; 9=Commercial; 10=Seasonal Farm Truck
            Get
                Return _VehicleUseTypeId
            End Get
            Set(value As String)
                _VehicleUseTypeId = value
            End Set
        End Property
        Public Property VehicleUsageTypeId As String '0=None
            Get
                Return _VehicleUsageTypeId
            End Get
            Set(value As String)
                _VehicleUsageTypeId = value
            End Set
        End Property
        Public Property FarmUseCodeTypeId As String '0=N/A; 1=Non-Farm; 2=Farm; 3=Seasonal Farm
            Get
                Return _FarmUseCodeTypeId
            End Get
            Set(value As String)
                _FarmUseCodeTypeId = value
            End Set
        End Property
        Public Property UseCodeTypeId As String '0=N/A; etc.; 20=Business; 21=Personal (UseCodeType table)
            Get
                Return _UseCodeTypeId
            End Get
            Set(value As String)
                _UseCodeTypeId = value
            End Set
        End Property
        Public Property OperatorTypeId As String '0=N/A; 1=No Operator Licensed less than 5 years; 2=Licensed less than 5 years, not owner or principal operator; 3=Owner or principal operator licensed less than 5 years
            Get
                Return _OperatorTypeId
            End Get
            Set(value As String)
                _OperatorTypeId = value
            End Set
        End Property
        Public Property OperatorUseTypeId As String '0=N/A; 1=Not driven to work or school; 2=To or from work less than 15 miles; 3=To or from work 15 miles or more
            Get
                Return _OperatorUseTypeId
            End Get
            Set(value As String)
                _OperatorUseTypeId = value
            End Set
        End Property
        Public Property RadiusTypeId As String '0=N/A; 1=Local, up to 50 miles; 2=Intermediate, 51 to 200 miles; 3=Long Distance, Over 200 miles
            Get
                Return _RadiusTypeId
            End Get
            Set(value As String)
                _RadiusTypeId = value
            End Set
        End Property
        Public Property SizeTypeId As String '0=N/A; 1=Private Passenger; etc. (SizeType table)
            Get
                Return _SizeTypeId
            End Get
            Set(value As String)
                _SizeTypeId = value
            End Set
        End Property
        Public Property SecondaryClassTypeId As String '0=N/A; 1=Truckers; etc. (SecondaryClassType table)
            Get
                Return _SecondaryClassTypeId
            End Get
            Set(value As String)
                _SecondaryClassTypeId = value
            End Set
        End Property
        Public Property SecondaryClassUsageTypeId As String '0=N/A; 1=Common Carrier; etc. (SecondaryClassUsageType table)
            Get
                Return _SecondaryClassUsageTypeId
            End Get
            Set(value As String)
                _SecondaryClassUsageTypeId = value
            End Set
        End Property
        Public Property UsedInDumping As Boolean
            Get
                Return _UsedInDumping
            End Get
            Set(value As Boolean)
                _UsedInDumping = value
            End Set
        End Property

        Public Property AdditionalInterests As Generic.List(Of QuickQuoteAdditionalInterest)
            Get
                SetParentOfListItems(_AdditionalInterests, "{663B7C7B-F2AC-4BF6-965A-D30F41A05661}")
                Return _AdditionalInterests
            End Get
            Set(value As Generic.List(Of QuickQuoteAdditionalInterest))
                _AdditionalInterests = value
                SetParentOfListItems(_AdditionalInterests, "{663B7C7B-F2AC-4BF6-965A-D30F41A05661}")
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 8</remarks>
        Public Property UninsuredMotoristLiabilityQuotedPremium As String
            Get
                'Return _UninsuredMotoristLiabilityQuotedPremium
                'updated 8/26/2014
                Return qqHelper.QuotedPremiumFormat(_UninsuredMotoristLiabilityQuotedPremium)
            End Get
            Set(value As String)
                _UninsuredMotoristLiabilityQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_UninsuredMotoristLiabilityQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 8</remarks>
        Public Property UninsuredMotoristLiabilityLimitId As String 'added 7/24/2013: CoverageLimit table... 100/300=4; N/A=0
            Get
                Return _UninsuredMotoristLiabilityLimitId
            End Get
            Set(value As String)
                _UninsuredMotoristLiabilityLimitId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 30013</remarks>
        Public Property UnderinsuredMotoristBodilyInjuryLiabilityQuotedPremium As String
            Get
                'Return _UnderinsuredMotoristBodilyInjuryLiabilityQuotedPremium
                'updated 8/26/2014
                Return qqHelper.QuotedPremiumFormat(_UnderinsuredMotoristBodilyInjuryLiabilityQuotedPremium)
            End Get
            Set(value As String)
                _UnderinsuredMotoristBodilyInjuryLiabilityQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_UnderinsuredMotoristBodilyInjuryLiabilityQuotedPremium)
            End Set
        End Property

        Public Property TrailerTypeId As String '0=N/A; 2=Semitrailer; 3=Trailer; 4=Service or Utility Trailer < 2,001 load capacity
            Get
                Return _TrailerTypeId
            End Get
            Set(value As String)
                _TrailerTypeId = value
            End Set
        End Property
        Public Property StatedAmount As String
            Get
                Return _StatedAmount
                'updated 8/26/2014; won't use for now
                ' Return qqHelper.QuotedPremiumFormat(_StatedAmount)
            End Get
            Set(value As String)
                _StatedAmount = value
                qqHelper.ConvertToQuotedPremiumFormat(_StatedAmount)
            End Set
        End Property

        'added 7/24/2013 for PPA
        Public Property PerformanceTypeId As String 'N/A=0, Standard=1, High=2, Sports=3, Intermediate=4, Sports Premium=5
            Get
                Return _PerformanceTypeId
            End Get
            Set(value As String)
                _PerformanceTypeId = value
            End Set
        End Property
        Public Property BodyTypeId As String 'None=0, Car=14, Van=16, Pickup w/o Camper=40, etc. BodyType table
            Get
                Return _BodyTypeId
            End Get
            Set(value As String)
                _BodyTypeId = value
            End Set
        End Property
        Public Property PrincipalDriverNum As String
            Get
                Return _PrincipalDriverNum
            End Get
            Set(value As String)
                _PrincipalDriverNum = value
            End Set
        End Property
        Public Property OccasionalDriver1Num As String
            Get
                Return _OccasionalDriver1Num
            End Get
            Set(value As String)
                _OccasionalDriver1Num = value
            End Set
        End Property
        Public Property OccasionalDriver2Num As String
            Get
                Return _OccasionalDriver2Num
            End Get
            Set(value As String)
                _OccasionalDriver2Num = value
            End Set
        End Property
        Public Property OccasionalDriver3Num As String
            Get
                Return _OccasionalDriver3Num
            End Get
            Set(value As String)
                _OccasionalDriver3Num = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 1</remarks>
        Public Property BodilyInjuryLiabilityLimitId As String 'added 7/24/2013: CoverageLimit table... 100/200=135; N/A=0
            Get
                Return _BodilyInjuryLiabilityLimitId
            End Get
            Set(value As String)
                _BodilyInjuryLiabilityLimitId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 1</remarks>
        Public Property BodilyInjuryLiabilityQuotedPremium As String '7/25/2013
            Get
                'Return _BodilyInjuryLiabilityQuotedPremium
                'updated 8/26/2014
                Return qqHelper.QuotedPremiumFormat(_BodilyInjuryLiabilityQuotedPremium)
            End Get
            Set(value As String)
                _BodilyInjuryLiabilityQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_BodilyInjuryLiabilityQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 4</remarks>
        Public Property PropertyDamageLimitId As String 'added 7/24/2013: CoverageLimit table... 25,000=8; N/A=0
            Get
                Return _PropertyDamageLimitId
            End Get
            Set(value As String)
                _PropertyDamageLimitId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 4</remarks>
        Public Property PropertyDamageQuotedPremium As String '7/25/2013
            Get
                'Return _PropertyDamageQuotedPremium
                'updated 8/26/2014
                Return qqHelper.QuotedPremiumFormat(_PropertyDamageQuotedPremium)
            End Get
            Set(value As String)
                _PropertyDamageQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_PropertyDamageQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 10007</remarks>
        Public Property UninsuredCombinedSingleLimitId As String 'added 7/24/2013: CoverageLimit table... 100,000=10; N/A=0
            Get
                Return _UninsuredCombinedSingleLimitId
            End Get
            Set(value As String)
                _UninsuredCombinedSingleLimitId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 10007</remarks>
        Public Property UninsuredCombinedSingleQuotedPremium As String '7/25/2013
            Get
                'Return _UninsuredCombinedSingleQuotedPremium
                'updated 8/26/2014
                Return qqHelper.QuotedPremiumFormat(_UninsuredCombinedSingleQuotedPremium)
            End Get
            Set(value As String)
                _UninsuredCombinedSingleQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_UninsuredCombinedSingleQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 9</remarks>
        Public Property UninsuredMotoristPropertyDamageLimitId As String 'added 7/24/2013: CoverageLimit table... 25,000=8; N/A=0
            Get
                Return _UninsuredMotoristPropertyDamageLimitId
            End Get
            Set(value As String)
                _UninsuredMotoristPropertyDamageLimitId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 9</remarks>
        Public Property UninsuredMotoristPropertyDamageQuotedPremium As String '7/25/2013
            Get
                'Return _UninsuredMotoristPropertyDamageQuotedPremium
                'updated 8/26/2014
                Return qqHelper.QuotedPremiumFormat(_UninsuredMotoristPropertyDamageQuotedPremium)
            End Get
            Set(value As String)
                _UninsuredMotoristPropertyDamageQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_UninsuredMotoristPropertyDamageQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 9; this is for CAP IL only</remarks>
        Public Property HasUninsuredMotoristPropertyDamage As Boolean 'added 10/30/2018 for CAP IL; covCodeId 9; CheckBox
            Get
                Return _HasUninsuredMotoristPropertyDamage
            End Get
            Set(value As Boolean)
                _HasUninsuredMotoristPropertyDamage = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 293</remarks>
        Public Property UninsuredMotoristPropertyDamageDeductibleLimitId As String 'added 7/24/2013: CoverageLimit table... 300=155; N/A=0
            Get
                Return _UninsuredMotoristPropertyDamageDeductibleLimitId
            End Get
            Set(value As String)
                _UninsuredMotoristPropertyDamageDeductibleLimitId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 293</remarks>
        Public Property UninsuredMotoristPropertyDamageDeductibleQuotedPremium As String '7/25/2013
            Get
                'Return _UninsuredMotoristPropertyDamageDeductibleQuotedPremium
                'updated 8/26/2014
                Return qqHelper.QuotedPremiumFormat(_UninsuredMotoristPropertyDamageDeductibleQuotedPremium)
            End Get
            Set(value As String)
                _UninsuredMotoristPropertyDamageDeductibleQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_UninsuredMotoristPropertyDamageDeductibleQuotedPremium)
            End Set
        End Property
        'added 7/25/2013 for PPA
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80056</remarks>
        Public Property HasPollutionLiabilityBroadenedCoverage As Boolean
            Get
                Return _HasPollutionLiabilityBroadenedCoverage
            End Get
            Set(value As Boolean)
                _HasPollutionLiabilityBroadenedCoverage = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80056</remarks>
        Public Property PollutionLiabilityBroadenedCoverageQuotedPremium As String
            Get
                'Return _PollutionLiabilityBroadenedCoverageQuotedPremium
                'updated 8/26/2014
                Return qqHelper.QuotedPremiumFormat(_PollutionLiabilityBroadenedCoverageQuotedPremium)
            End Get
            Set(value As String)
                _PollutionLiabilityBroadenedCoverageQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_PollutionLiabilityBroadenedCoverageQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 66</remarks>
        Public Property TransportationExpenseLimitId As String 'added 7/25/2013: CoverageLimit table... 30/900=80; 20/600=30
            Get
                Return _TransportationExpenseLimitId
            End Get
            Set(value As String)
                _TransportationExpenseLimitId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 66</remarks>
        Public Property TransportationExpenseQuotedPremium As String
            Get
                'Return _TransportationExpenseQuotedPremium
                'updated 8/26/2014
                Return qqHelper.QuotedPremiumFormat(_TransportationExpenseQuotedPremium)
            End Get
            Set(value As String)
                _TransportationExpenseQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_TransportationExpenseQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 10044</remarks>
        Public Property HasAutoLoanOrLease As Boolean
            Get
                Return _HasAutoLoanOrLease
            End Get
            Set(value As Boolean)
                _HasAutoLoanOrLease = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 10044</remarks>
        Public Property AutoLoanOrLeaseQuotedPremium As String
            Get
                'Return _AutoLoanOrLeaseQuotedPremium
                'updated 8/26/2014
                Return qqHelper.QuotedPremiumFormat(_AutoLoanOrLeaseQuotedPremium)
            End Get
            Set(value As String)
                _AutoLoanOrLeaseQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_AutoLoanOrLeaseQuotedPremium) 'added 8/26/2014
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 16</remarks>
        Public Property TapesAndRecordsLimitId As String 'added 7/25/2013: CoverageLimit table... 200=212; 400=219
            Get
                Return _TapesAndRecordsLimitId
            End Get
            Set(value As String)
                _TapesAndRecordsLimitId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 16</remarks>
        Public Property TapesAndRecordsQuotedPremium As String
            Get
                'Return _TapesAndRecordsQuotedPremium
                'updated 8/26/2014
                Return qqHelper.QuotedPremiumFormat(_TapesAndRecordsQuotedPremium)
            End Get
            Set(value As String)
                _TapesAndRecordsQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_TapesAndRecordsQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 15</remarks>
        Public Property SoundEquipmentLimit As String
            Get
                Return _SoundEquipmentLimit
            End Get
            Set(value As String)
                _SoundEquipmentLimit = value
                qqHelper.ConvertToLimitFormat(_SoundEquipmentLimit)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 15</remarks>
        Public Property SoundEquipmentQuotedPremium As String
            Get
                'Return _SoundEquipmentQuotedPremium
                'updated 8/26/2014
                Return qqHelper.QuotedPremiumFormat(_SoundEquipmentQuotedPremium)
            End Get
            Set(value As String)
                _SoundEquipmentQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_SoundEquipmentQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 57</remarks>
        Public Property ElectronicEquipmentLimit As String
            Get
                Return _ElectronicEquipmentLimit
            End Get
            Set(value As String)
                _ElectronicEquipmentLimit = value
                qqHelper.ConvertToLimitFormat(_ElectronicEquipmentLimit)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 57</remarks>
        Public Property ElectronicEquipmentQuotedPremium As String
            Get
                'Return _ElectronicEquipmentQuotedPremium
                'updated 8/26/2014
                Return qqHelper.QuotedPremiumFormat(_ElectronicEquipmentQuotedPremium)
            End Get
            Set(value As String)
                _ElectronicEquipmentQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_ElectronicEquipmentQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80031</remarks>
        Public Property TripInterruptionLimitId As String 'added 7/25/2013: CoverageLimit table... 300=25; N/A=0
            Get
                Return _TripInterruptionLimitId
            End Get
            Set(value As String)
                _TripInterruptionLimitId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 80031</remarks>
        Public Property TripInterruptionQuotedPremium As String
            Get
                'Return _TripInterruptionQuotedPremium
                'updated 8/26/2014
                Return qqHelper.QuotedPremiumFormat(_TripInterruptionQuotedPremium)
            End Get
            Set(value As String)
                _TripInterruptionQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_TripInterruptionQuotedPremium)
            End Set
        End Property
        Public Property ScheduledItems As List(Of QuickQuoteScheduledItem)
            Get
                SetParentOfListItems(_ScheduledItems, "{09860584-30E8-475E-A428-409826D39D30}")
                Return _ScheduledItems
            End Get
            Set(value As List(Of QuickQuoteScheduledItem))
                _ScheduledItems = value
                SetParentOfListItems(_ScheduledItems, "{09860584-30E8-475E-A428-409826D39D30}")
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's AntiLockType table (0=None, 1=Rear-Wheel Anti-Lock Brakes, 2=All-Wheel Anti-Lock Brakes, 3=None)</remarks>
        Public Property AntiLockTypeId As String 'added 7/30/2013 for PPA: 0=None; 1=Rear-Wheel Anti-Lock Brakes; 2=All-Wheel Anti-Lock Brakes; 3=None
            Get
                Return _AntiLockTypeId
            End Get
            Set(value As String)
                _AntiLockTypeId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>potential problem field... int in database</remarks>
        Public Property ActualCashValue As String
            Get
                Return _ActualCashValue
            End Get
            Set(value As String)
                _ActualCashValue = value
                'qqHelper.ConvertToQuotedPremiumFormat(_ActualCashValue) '8/8/2013 - int in database; had to remove money formatting (caused 'Unable to successfully translate source data to policy image.' error)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's AntiTheftType table (0=None, 1=Alarm Only, 2=Passive Disabling Device, 3=None, 4=Alarm Only, 5=Passive Disabling Device)</remarks>
        Public Property AntiTheftTypeId As String '0=None, 1=Alarm Only, 2=Passive Disabling Device, 3=None, 4=Alarm Only, 5=Passive Disabling Device
            Get
                Return _AntiTheftTypeId
            End Get
            Set(value As String)
                _AntiTheftTypeId = value
            End Set
        End Property
        Public Property CubicCentimeters As String 'Horsepower/CC's in UI; may need to format
            Get
                Return _CubicCentimeters
            End Get
            Set(value As String)
                _CubicCentimeters = value
            End Set
        End Property
        Public Property CustomEquipmentAmount As String
            Get
                Return _CustomEquipmentAmount
                'updated 8/26/2014; won't use for now
                'Return qqHelper.QuotedPremiumFormat(_CustomEquipmentAmount)
            End Get
            Set(value As String)
                _CustomEquipmentAmount = value
                qqHelper.ConvertToQuotedPremiumFormat(_CustomEquipmentAmount)
            End Set
        End Property
        Public Property DamageRemarks As String
            Get
                Return _DamageRemarks
            End Get
            Set(value As String)
                _DamageRemarks = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's YesNo table (-1=[blank or empty string], 0=[blank or empty string], 1=Yes, 2=No)</remarks>
        Public Property DamageYesNoId As String 'YesNo table; -1=[blank or empty string], 0=[blank or empty string], 1=Yes, 2=No
            Get
                Return _DamageYesNoId
            End Get
            Set(value As String)
                _DamageYesNoId = value
            End Set
        End Property
        Public Property DriverOutOfStateSurcharge As Boolean
            Get
                Return _DriverOutOfStateSurcharge
            End Get
            Set(value As Boolean)
                _DriverOutOfStateSurcharge = value
            End Set
        End Property
        Public Property GrossVehicleWeight As String 'GVW in UI; may need to format
            Get
                Return _GrossVehicleWeight
            End Get
            Set(value As String)
                _GrossVehicleWeight = value
            End Set
        End Property
        Public Property MultiCar As Boolean
            Get
                Return _MultiCar
            End Get
            Set(value As Boolean)
                _MultiCar = value
            End Set
        End Property
        Public Property NonOwned As Boolean 'Extended Non-Owned in UI
            Get
                Return _NonOwned
            End Get
            Set(value As Boolean)
                _NonOwned = value
            End Set
        End Property
        Public Property NonOwnedNamed As Boolean 'Named Non-Owned Non-Specific Vehicle in UI
            Get
                Return _NonOwnedNamed
            End Get
            Set(value As Boolean)
                _NonOwnedNamed = value
            End Set
        End Property
        Public Property PurchasedDate As String
            Get
                Return _PurchasedDate
            End Get
            Set(value As String)
                _PurchasedDate = value
                qqHelper.ConvertToShortDate(_PurchasedDate)
            End Set
        End Property
        Public Property RegisteredStateId As String '16=IN
            Get
                Return _RegisteredStateId
            End Get
            Set(value As String)
                _RegisteredStateId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's RestraintType table (0=None, 1=Passive Seat Belts, 2=Driver Side Airbags, 3=Driver and Passenger Airbags, 4=Side Airbags)</remarks>
        Public Property RestraintTypeId As String '0=None, 1=Passive Seat Belts, 2=Driver Side Airbags, 3=Driver and Passenger Airbags, 4=Side Airbags
            Get
                Return _RestraintTypeId
            End Get
            Set(value As String)
                _RestraintTypeId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's VehicleType table; Motorcycle Type in UI screen for PPA vehicle; -1=[blank or empty string], 0=N/A, 1=Cruiser, 2=Touring, 3=Street Bike/Sport Bike, 4=Trike (3 or 4 Wheel)</remarks>
        Public Property VehicleTypeId As String 'Motorcycle Type in UI; -1=[blank or empty string], 0=N/A, 1=Cruiser, 2=Touring, 3=Street Bike/Sport Bike, 4=Trike (3 or 4 Wheel), etc.
            Get
                Return _VehicleTypeId
            End Get
            Set(value As String)
                _VehicleTypeId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's VehicleValueType table (0=None, 1=New, 2=Used); New or Used in UI screen for PPA vehicle</remarks>
        Public Property VehicleValueId As String 'uses VehicleValueType table; 0=None, 1=New, 2=Used
            Get
                Return _VehicleValueId
            End Get
            Set(value As String)
                _VehicleValueId = value
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

        'added 3/26/2014
        Public Property ComprehensiveCoverageOnly As Boolean
            Get
                Return _ComprehensiveCoverageOnly
            End Get
            Set(value As Boolean)
                _ComprehensiveCoverageOnly = value
            End Set
        End Property

        'added 4/15/2014
        Public Property VehicleSymbols As List(Of QuickQuoteVehicleSymbol)
            Get
                SetParentOfListItems(_VehicleSymbols, "{09860584-30E8-475E-A428-409826D39D31}")
                Return _VehicleSymbols
            End Get
            Set(value As List(Of QuickQuoteVehicleSymbol))
                _VehicleSymbols = value
                SetParentOfListItems(_VehicleSymbols, "{09860584-30E8-475E-A428-409826D39D31}")
            End Set
        End Property

        Public Property VehicleNum As String 'added 4/21/2014 for reconciliation purposes
            Get
                Return _VehicleNum
            End Get
            Set(value As String)
                _VehicleNum = value
            End Set
        End Property
        Public Property HasVehicleMakeModelYearChanged As Boolean 'added 4/21/2014
            Get
                Return _HasVehicleMakeModelYearChanged
            End Get
            Set(value As Boolean)
                _HasVehicleMakeModelYearChanged = value
            End Set
        End Property
        Public Property CanUseVehicleSymbolNumForVehicleSymbolReconciliation As Boolean 'added 4/24/2014
            Get
                Return _CanUseVehicleSymbolNumForVehicleSymbolReconciliation
            End Get
            Set(value As Boolean)
                _CanUseVehicleSymbolNumForVehicleSymbolReconciliation = value
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
        Public Property CanUseScheduledItemsNumForScheduledItemReconciliation As Boolean 'added 5/14/2014
            Get
                Return _CanUseScheduledItemsNumForScheduledItemReconciliation
            End Get
            Set(value As Boolean)
                _CanUseScheduledItemsNumForScheduledItemReconciliation = value
            End Set
        End Property
        'added 10/17/2018 for multi-state
        Public Property VehicleNum_MasterPart As String
            Get
                Return _VehicleNum_MasterPart
            End Get
            Set(value As String)
                _VehicleNum_MasterPart = value
            End Set
        End Property
        Public Property VehicleNum_CGLPart As String
            Get
                Return _VehicleNum_CGLPart
            End Get
            Set(value As String)
                _VehicleNum_CGLPart = value
            End Set
        End Property
        Public Property VehicleNum_CPRPart As String
            Get
                Return _VehicleNum_CPRPart
            End Get
            Set(value As String)
                _VehicleNum_CPRPart = value
            End Set
        End Property
        Public Property VehicleNum_CIMPart As String
            Get
                Return _VehicleNum_CIMPart
            End Get
            Set(value As String)
                _VehicleNum_CIMPart = value
            End Set
        End Property
        Public Property VehicleNum_CRMPart As String
            Get
                Return _VehicleNum_CRMPart
            End Get
            Set(value As String)
                _VehicleNum_CRMPart = value
            End Set
        End Property
        Public Property VehicleNum_GARPart As String
            Get
                Return _VehicleNum_GARPart
            End Get
            Set(value As String)
                _VehicleNum_GARPart = value
            End Set
        End Property

        Public Property Modifiers As List(Of QuickQuoteModifier) 'added 10/16/2014
            Get
                SetParentOfListItems(_Modifiers, "{09860584-30E8-475E-A428-409826D39D32}")
                Return _Modifiers
            End Get
            Set(value As List(Of QuickQuoteModifier))
                _Modifiers = value
                SetParentOfListItems(_Modifiers, "{09860584-30E8-475E-A428-409826D39D32}")
            End Set
        End Property

        'added 5/26/2017
        Public Property TotalCoveragesPremium As String
            Get
                'Return _TotalCoveragesPremium
                Return qqHelper.QuotedPremiumFormat(_TotalCoveragesPremium)
            End Get
            Set(value As String)
                _TotalCoveragesPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_TotalCoveragesPremium)
            End Set
        End Property
        Public Property CAP_GAR_TotalCoveragesPremium As String
            Get
                'Return _CAP_GAR_TotalCoveragesPremium
                Return qqHelper.QuotedPremiumFormat(_CAP_GAR_TotalCoveragesPremium)
            End Get
            Set(value As String)
                _CAP_GAR_TotalCoveragesPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_CAP_GAR_TotalCoveragesPremium)
            End Set
        End Property

        'added 8/2/2018
        Public ReadOnly Property QuoteStateTakenFrom As QuickQuoteHelperClass.QuickQuoteState
            Get
                Return _QuoteStateTakenFrom
            End Get
        End Property

        'added 9/17/2018
        Public ReadOnly Property OneLineYearMakeAndModel As String
            Get
                Dim strOneLine As String = _Year
                strOneLine = qqHelper.appendText(strOneLine, _Make, " ")
                strOneLine = qqHelper.appendText(strOneLine, _Model, " ")

                Return strOneLine
            End Get
        End Property

        'added 9/28/2018
        Public Property UnderinsuredCombinedSingleLimitId As String 'covCodeId 296; PPA IL only
            Get
                Return _UnderinsuredCombinedSingleLimitId
            End Get
            Set(value As String)
                _UnderinsuredCombinedSingleLimitId = value
            End Set
        End Property

        Public Property UnderinsuredCombinedSingleLimitQuotedPremium As String 'covCodeId 296; PPA IL only
            Get
                Return qqHelper.QuotedPremiumFormat(_UnderinsuredCombinedSingleLimitQuotedPremium)
            End Get
            Set(value As String)
                _UnderinsuredCombinedSingleLimitQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_UnderinsuredCombinedSingleLimitQuotedPremium)
            End Set
        End Property
        Public Property UninsuredBodilyInjuryLimitId As String 'covCodeId 294; PPA IL, HOM IN, DFR IN, FAR IN/IL
            Get
                Return _UninsuredBodilyInjuryLimitId
            End Get
            Set(value As String)
                _UninsuredBodilyInjuryLimitId = value
            End Set
        End Property
        Public Property UninsuredBodilyInjuryQuotedPremium As String 'covCodeId 294; PPA IL, HOM IN, DFR IN, FAR IN/IL
            Get
                Return qqHelper.QuotedPremiumFormat(_UninsuredBodilyInjuryQuotedPremium)
            End Get
            Set(value As String)
                _UninsuredBodilyInjuryQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_UninsuredBodilyInjuryQuotedPremium)
            End Set
        End Property
        Public Property UnderinsuredBodilyInjuryLimitId As String 'covCodeId 295; PPA IL only
            Get
                Return _UnderinsuredBodilyInjuryLimitId
            End Get
            Set(value As String)
                _UnderinsuredBodilyInjuryLimitId = value
            End Set
        End Property
        Public Property UnderinsuredBodilyInjuryQuotedPremium As String 'covCodeId 295; PPA IL only
            Get
                Return qqHelper.QuotedPremiumFormat(_UnderinsuredBodilyInjuryQuotedPremium)
            End Get
            Set(value As String)
                _UnderinsuredBodilyInjuryQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_UnderinsuredBodilyInjuryQuotedPremium)
            End Set
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

        'added 5/22/2019
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
        Public Property AddedImageNum As String 'added 7/31/2019
            Get
                Return _AddedImageNum
            End Get
            Set(value As String)
                _AddedImageNum = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 30013</remarks>
        Public Property UnderinsuredMotoristBodilyInjuryLiabilityLimitId As String 'added 7/24/2013: CoverageLimit table... 100/300=4; N/A=0
            Get
                Return _UnderinsuredMotoristBodilyInjuryLiabilityLimitId
            End Get
            Set(value As String)
                _UnderinsuredMotoristBodilyInjuryLiabilityLimitId = value
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _PolicyId = ""
            _PolicyImageNum = ""
            _AnnualMiles = ""
            _ClassCode = ""
            _CostNew = ""
            _CostUsed = ""
            '_Coverages = New Generic.List(Of QuickQuoteCoverage)
            _Coverages = Nothing 'added 8/4/2014
            _DaysPerWeek = ""
            _Make = ""
            _MilesOneWay = ""
            _Model = ""
            _OdometerReading = ""
            _Vin = ""
            _Year = ""
            _GaragingAddress = New QuickQuoteGaragingAddress
            _HasLiability_UM_UIM = False
            _Liability_UM_UIM_QuotedPremium = ""
            _Liability_UM_UIM_LimitId = ""
            _HasMedicalPayments = False
            _MedicalPaymentsQuotedPremium = ""
            _MedicalPaymentsLimitId = ""
            _HasComprehensive = False
            _ComprehensiveDeductible = ""
            _ComprehensiveDeductibleId = ""
            _ComprehensiveQuotedPremium = ""
            _ComprehensiveDeductibleLimitId = ""
            _HasCollision = False
            _CollisionDeductible = ""
            _CollisionDeductibleId = ""
            _CollisionQuotedPremium = ""
            _CollisionDeductibleLimitId = ""
            _HasTowingAndLabor = False
            _TowingAndLaborQuotedPremium = ""
            _TowingAndLaborDeductibleLimitId = ""
            _HasRentalReimbursement = False
            _RentalReimbursementDailyReimbursement = ""
            _RentalReimbursementNumberOfDays = ""
            _RentalReimbursementQuotedPremium = ""

            _PremiumFullTerm = ""
            _TerritoryNum = ""

            _VehicleRatingTypeId = ""
            _VehicleUseTypeId = ""
            _VehicleUsageTypeId = ""
            _FarmUseCodeTypeId = ""
            _UseCodeTypeId = ""
            _OperatorTypeId = ""
            _OperatorUseTypeId = ""
            _RadiusTypeId = ""
            _SizeTypeId = ""
            _SecondaryClassTypeId = ""
            _SecondaryClassUsageTypeId = ""
            _UsedInDumping = False

            '_AdditionalInterests = New Generic.List(Of QuickQuoteAdditionalInterest)
            _AdditionalInterests = Nothing 'added 8/4/2014

            _UninsuredMotoristLiabilityQuotedPremium = ""
            _UninsuredMotoristLiabilityLimitId = ""
            _UnderinsuredMotoristBodilyInjuryLiabilityQuotedPremium = ""

            _TrailerTypeId = ""
            _StatedAmount = ""

            _PerformanceTypeId = ""
            _BodyTypeId = ""
            _PrincipalDriverNum = ""
            _OccasionalDriver1Num = ""
            _OccasionalDriver2Num = ""
            _OccasionalDriver3Num = ""
            _BodilyInjuryLiabilityLimitId = ""
            _BodilyInjuryLiabilityQuotedPremium = ""
            _PropertyDamageLimitId = ""
            _PropertyDamageQuotedPremium = ""
            _UninsuredCombinedSingleLimitId = ""
            _UninsuredCombinedSingleQuotedPremium = ""
            _UninsuredMotoristPropertyDamageLimitId = ""
            _UninsuredMotoristPropertyDamageQuotedPremium = ""
            _HasUninsuredMotoristPropertyDamage = False 'added 10/30/2018 for CAP IL; covCodeId 9; CheckBox
            _UninsuredMotoristPropertyDamageDeductibleLimitId = ""
            _UninsuredMotoristPropertyDamageDeductibleQuotedPremium = ""
            _HasPollutionLiabilityBroadenedCoverage = False
            _PollutionLiabilityBroadenedCoverageQuotedPremium = ""
            _TransportationExpenseLimitId = ""
            _TransportationExpenseQuotedPremium = ""
            _HasAutoLoanOrLease = False
            _AutoLoanOrLeaseQuotedPremium = ""
            _TapesAndRecordsLimitId = ""
            _TapesAndRecordsQuotedPremium = ""
            _SoundEquipmentLimit = ""
            _SoundEquipmentQuotedPremium = ""
            _ElectronicEquipmentLimit = ""
            _ElectronicEquipmentQuotedPremium = ""
            _TripInterruptionLimitId = ""
            _TripInterruptionQuotedPremium = ""
            '_ScheduledItems = New List(Of QuickQuoteScheduledItem)
            _ScheduledItems = Nothing 'added 8/4/2014
            _AntiLockTypeId = ""
            _ActualCashValue = ""
            _AntiTheftTypeId = ""
            _CubicCentimeters = ""
            _CustomEquipmentAmount = ""
            _DamageRemarks = ""
            _DamageYesNoId = ""
            _DriverOutOfStateSurcharge = False
            _GrossVehicleWeight = ""
            _MultiCar = False
            _NonOwnedNamed = False
            _PurchasedDate = ""
            _RegisteredStateId = ""
            _RestraintTypeId = ""
            _VehicleTypeId = ""
            _VehicleValueId = ""
            _NonOwned = False

            _HasConvertedCoverages = False 'added 2/18/2014

            _ComprehensiveCoverageOnly = False 'added 3/26/2014

            '_VehicleSymbols = New List(Of QuickQuoteVehicleSymbol) 'added 4/15/2014
            _VehicleSymbols = Nothing 'added 8/4/2014

            _VehicleNum = "" 'added 4/21/2014 for reconciliation purposes
            _HasVehicleMakeModelYearChanged = False 'added 4/21/2014
            _CanUseVehicleSymbolNumForVehicleSymbolReconciliation = False 'added 4/24/2014
            _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = False 'added 4/29/2014
            _CanUseScheduledItemsNumForScheduledItemReconciliation = False 'added 5/14/2014
            'added 10/17/2018 for multi-state
            _VehicleNum_MasterPart = ""
            _VehicleNum_CGLPart = ""
            _VehicleNum_CPRPart = ""
            _VehicleNum_CIMPart = ""
            _VehicleNum_CRMPart = ""
            _VehicleNum_GARPart = ""

            'added 10/16/2014
            '_Modifiers = New List(Of QuickQuoteModifier)
            _Modifiers = Nothing

            'added 5/26/2017
            _TotalCoveragesPremium = ""
            _CAP_GAR_TotalCoveragesPremium = ""

            'added 8/2/2018
            _QuoteStateTakenFrom = QuickQuoteHelperClass.QuickQuoteState.None

            'added 9/28/2018
            _UnderinsuredCombinedSingleLimitId = "" 'covCodeId 296; PPA IL only
            _UnderinsuredCombinedSingleLimitQuotedPremium = "" 'covCodeId 296; PPA IL only
            _UninsuredBodilyInjuryLimitId = "" 'covCodeId 294; PPA IL, HOM IN, DFR IN, FAR IN/IL
            _UninsuredBodilyInjuryQuotedPremium = "" 'covCodeId 294; PPA IL, HOM IN, DFR IN, FAR IN/IL
            _UnderinsuredBodilyInjuryLimitId = "" 'covCodeId 295; PPA IL only
            _UnderinsuredBodilyInjuryQuotedPremium = "" 'covCodeId 295; PPA IL only

            'added 1/16/2019
            _DisplayNum = 0
            _OriginalDisplayNum = 0
            _OkayToUseDisplayNum = QuickQuoteHelperClass.WhenToSetType.None

            _DetailStatusCode = "" 'added 5/15/2019

            'added 5/22/2019
            _AddedDate = ""
            _EffectiveDate = ""
            _LastModifiedDate = ""
            _PCAdded_Date = ""
            _AddedImageNum = "" 'added 7/31/2019

            _UnderinsuredMotoristBodilyInjuryLiabilityLimitId = ""

        End Sub
        ''' <summary>
        ''' used to parse thru coverages and set different properties
        ''' </summary>
        ''' <remarks>always executed when xml is parsed; doesn't specifically needed to be called by developer code</remarks>
        Public Sub ParseThruCoverages()
            If _Coverages IsNot Nothing AndAlso _Coverages.Count > 0 Then
                For Each cov As QuickQuoteCoverage In _Coverages
                    TotalCoveragesPremium = qqHelper.getSum(_TotalCoveragesPremium, cov.FullTermPremium) 'added 5/26/2017
                    Select Case cov.CoverageCodeId
                        Case "2" 'Combo:  Combined Single Limit Liability; 5/5/2017 note: PPA, CAP, GAR
                            _HasLiability_UM_UIM = True
                            Liability_UM_UIM_QuotedPremium = cov.FullTermPremium
                            Liability_UM_UIM_LimitId = cov.CoverageLimitId 'added 7/24/2013 for PPA; ManualLimitAmount was also populated
                            '5/9/2017 note for GAR: has FullTermPremium (396.00), Checkbox True, CoverageLimitId 56 (1,000,000) (file:///C:/Users/domin/Documents/GAR1000098(1143275-1)GKLL.xml)
                            CAP_GAR_TotalCoveragesPremium = qqHelper.getSum(_CAP_GAR_TotalCoveragesPremium, cov.FullTermPremium) 'added 5/26/2017
                        Case "60006", "6" 'Combo:  Medical Payments; updated 7/24/2013 for PPA (6); 5/5/2017 note: 60006 valid for CAP, GAR
                            _HasMedicalPayments = True
                            MedicalPaymentsQuotedPremium = cov.FullTermPremium
                            MedicalPaymentsLimitId = cov.CoverageLimitId 'added 7/24/2013 for PPA; ManualLimitAmount was also populated
                            '5/9/2017 note for GAR (60006): has FullTermPremium (17.00), Checkbox True, MedicalPaymentsTypeId 3 (carried over from Policy), CoverageLimitId 15 (5,000) (file:///C:/Users/domin/Documents/GAR1000098(1143275-1)GKLL.xml)
                            CAP_GAR_TotalCoveragesPremium = qqHelper.getSum(_CAP_GAR_TotalCoveragesPremium, cov.FullTermPremium) 'added 5/26/2017
                        Case "3" 'CheckBox:  Comprehensive Coverage; OtherThanCollisionSubTypeId; updated 7/24/2013 for PPA - Combo; 5/5/2017 note: PPA (Combo), CAP (Checkbox), GAR (Checkbox)
                            'can check Checkbox property
                            _HasComprehensive = True
                            ComprehensiveDeductibleId = cov.DeductibleId
                            ComprehensiveQuotedPremium = cov.FullTermPremium
                            ComprehensiveDeductibleLimitId = cov.CoverageLimitId 'added 7/24/2013 for PPA; ManualLimitAmount was also populated
                            '5/9/2017 note for GAR: no FullTermPremium, Checkbox True, OtherThanCollisionSubTypeId 6 (Comprehensive) (file:///C:/Users/domin/Documents/GAR1000098(1143275-1)GKLL.xml)
                            CAP_GAR_TotalCoveragesPremium = qqHelper.getSum(_CAP_GAR_TotalCoveragesPremium, cov.FullTermPremium) 'added 5/26/2017
                        Case "5" 'CheckBox:  Collision Coverage; updated 7/25/2013 for PPA - Combo; 5/5/2017 note: PPA (Combo), CAP (Checkbox), GAR (Checkbox)
                            'can check Checkbox property
                            _HasCollision = True
                            CollisionDeductibleId = cov.DeductibleId
                            CollisionQuotedPremium = cov.FullTermPremium
                            CollisionDeductibleLimitId = cov.CoverageLimitId 'added 7/25/2013 for PPA; ManualLimitAmount was also populated
                            '5/9/2017 note for GAR: has FullTermPremium (418.00), Checkbox True, DeductibleId 4 (250) (file:///C:/Users/domin/Documents/GAR1000098(1143275-1)GKLL.xml)
                            CAP_GAR_TotalCoveragesPremium = qqHelper.getSum(_CAP_GAR_TotalCoveragesPremium, cov.FullTermPremium) 'added 5/26/2017
                        Case "60008" 'CheckBox:  Towing and Labor; updated 7/25/2013 for PPA - Combo; 5/10/2017 note: PPA/CAP/GAR, Combo for PPA and Checkbox for CAP/GAR
                            'can check Checkbox property
                            _HasTowingAndLabor = True
                            TowingAndLaborQuotedPremium = cov.FullTermPremium
                            TowingAndLaborDeductibleLimitId = cov.CoverageLimitId 'added 7/25/2013 for PPA; ManualLimitAmount was also populated
                            CAP_GAR_TotalCoveragesPremium = qqHelper.getSum(_CAP_GAR_TotalCoveragesPremium, cov.FullTermPremium) 'added 5/26/2017
                        Case "10094" 'CheckBox/Edit:  Rental Reimbursement Other Than Collision (not using yet); updated 3/28/2013 to use w/ 10095 (needs to combine premiums); 5/10/2017 note: CAP/GAR, Checkbox for both
                            'can check Checkbox property
                            _HasRentalReimbursement = True
                            RentalReimbursementDailyReimbursement = cov.DailyReimbursement
                            _RentalReimbursementNumberOfDays = cov.NumberOfDays
                            RentalReimbursementQuotedPremium = qqHelper.getSum(_RentalReimbursementQuotedPremium, cov.FullTermPremium)
                            CAP_GAR_TotalCoveragesPremium = qqHelper.getSum(_CAP_GAR_TotalCoveragesPremium, cov.FullTermPremium) 'added 5/26/2017
                        Case "10095" 'CheckBox/Edit:  Rental Reimbursement Collision; 5/10/2017 note: CAP/GAR, Checkbox for both
                            'can check Checkbox property
                            _HasRentalReimbursement = True
                            RentalReimbursementDailyReimbursement = cov.DailyReimbursement
                            _RentalReimbursementNumberOfDays = cov.NumberOfDays
                            'RentalReimbursementQuotedPremium = cov.FullTermPremium
                            'updated 3/28/2013 to use w/ 10094 (needs to combine premiums)
                            RentalReimbursementQuotedPremium = qqHelper.getSum(_RentalReimbursementQuotedPremium, cov.FullTermPremium)
                            CAP_GAR_TotalCoveragesPremium = qqHelper.getSum(_CAP_GAR_TotalCoveragesPremium, cov.FullTermPremium) 'added 5/26/2017
                        Case "8" 'CheckBox:  Uninsured Motorist Liability (added 10/3/2012 for declarations section)'no xml writing logic since coverage is automatically added; updated 7/24/2013 for PPA - Combo; 5/5/2017 note: PPA (Combo), CAP (Checkbox), GAR (Checkbox)
                            'get premium and maybe limit; all vehicle prems are later summed
                            UninsuredMotoristLiabilityQuotedPremium = cov.FullTermPremium
                            UninsuredMotoristLiabilityLimitId = cov.CoverageLimitId 'added 7/24/2013 for PPA
                            '5/9/2017 note for GAR: has FullTermPremium (20.00), Checkbox True, CoverageLimitId 56, DeductibleId 11 (file:///C:/Users/domin/Documents/GAR1000098(1143275-1)GKLL.xml)
                            CAP_GAR_TotalCoveragesPremium = qqHelper.getSum(_CAP_GAR_TotalCoveragesPremium, cov.FullTermPremium) 'added 5/26/2017
                        Case "30013" 'Combo:  Underinsured Motorist Bodily Injury Liability (added 10/3/2012 for declarations section)'no xml writing logic since coverage is automatically added; 5/5/2017 note: CAP, GAR
                            UnderinsuredMotoristBodilyInjuryLiabilityLimitId = cov.CoverageLimitId
                            'get premium and maybe limit; all vehicle prems are later summed
                            UnderinsuredMotoristBodilyInjuryLiabilityQuotedPremium = cov.FullTermPremium
                            '5/9/2017 note for GAR: has FullTermPremium (29.00), Checkbox True, CoverageLimitId 56 (1,000,000) (file:///C:/Users/domin/Documents/GAR1000098(1143275-1)GKLL.xml)
                            CAP_GAR_TotalCoveragesPremium = qqHelper.getSum(_CAP_GAR_TotalCoveragesPremium, cov.FullTermPremium) 'added 5/26/2017

                            'added 7/24/2013 for PPA
                        Case "1" 'Combo: Bodily Injury Liability; 5/10/2017 note: PPA only
                            BodilyInjuryLiabilityLimitId = cov.CoverageLimitId
                            BodilyInjuryLiabilityQuotedPremium = cov.FullTermPremium
                        Case "4" 'Combo: Property Damage-Single Limit; 5/10/2017 note: PPA only
                            PropertyDamageLimitId = cov.CoverageLimitId 'ManualLimitAmount was also populated
                            PropertyDamageQuotedPremium = cov.FullTermPremium
                            'Case "6" 'Combo: Medical Payments; use existing CAP property above
                        Case "10007", "7" 'Combo: Uninsured Combined Single Limit; 5/10/2017 note: PPA only; updated 9/28/2018 for IL (7-PPA IL only)
                            UninsuredCombinedSingleLimitId = cov.CoverageLimitId 'ManualLimitAmount was also populated
                            UninsuredCombinedSingleQuotedPremium = cov.FullTermPremium
                        Case "9" 'Combo: Uninsured Motorist Property Damage; 5/10/2017 note: PPA only; 10/30/2018 - now CAP IL also - CheckBox
                            UninsuredMotoristPropertyDamageLimitId = cov.CoverageLimitId 'ManualLimitAmount was also populated
                            UninsuredMotoristPropertyDamageQuotedPremium = cov.FullTermPremium
                            _HasUninsuredMotoristPropertyDamage = cov.Checkbox 'added 10/30/2018 for CAP IL; covCodeId 9; CheckBox
                        Case "293" 'Combo: UM Deductible; 5/10/2017 note: PPA only
                            UninsuredMotoristPropertyDamageDeductibleLimitId = cov.CoverageLimitId 'ManualLimitAmount was also populated
                            UninsuredMotoristPropertyDamageDeductibleQuotedPremium = cov.FullTermPremium
                            'added 7/25/2013 for PPA
                        Case "80056" 'CheckBox: Pollution Liability Broadened Coverage; 5/10/2017 note: PPA only
                            _HasPollutionLiabilityBroadenedCoverage = cov.Checkbox 'need to look at checkbox since coverage always seems to be there
                            PollutionLiabilityBroadenedCoverageQuotedPremium = cov.FullTermPremium
                        Case "66" 'Combo: Transportation Expense; 5/10/2017 note: PPA (Combo) and GAR (Checkbox)
                            TransportationExpenseLimitId = cov.CoverageLimitId
                            TransportationExpenseQuotedPremium = cov.FullTermPremium
                        Case "10044" 'CheckBox: Auto Loan/Lease; 5/10/2017 note: PPA only
                            _HasAutoLoanOrLease = cov.Checkbox 'need to look at checkbox since coverage always seems to be there
                            AutoLoanOrLeaseQuotedPremium = cov.FullTermPremium
                        Case "16" 'Combo: Tapes and Records; 5/10/2017 note: PPA (Combo), CAP (Checkbox), and GAR (Checkbox)
                            TapesAndRecordsLimitId = cov.CoverageLimitId 'ManualLimitAmount was also populated
                            TapesAndRecordsQuotedPremium = cov.FullTermPremium
                        Case "15" 'Edit: Sound Receiving and Transmitting; 5/10/2017 note: PPA only
                            SoundEquipmentLimit = cov.ManualLimitAmount
                            SoundEquipmentQuotedPremium = cov.FullTermPremium
                        Case "57" 'Edit: Electronic Equipment, Audio - Visual; 5/10/2017 note: PPA only
                            ElectronicEquipmentLimit = cov.ManualLimitAmount
                            ElectronicEquipmentQuotedPremium = cov.FullTermPremium
                        Case "80031" 'Combo: Trip Interruption Coverage; 5/10/2017 note: PPA only
                            TripInterruptionLimitId = cov.CoverageLimitId 'ManualLimitAmount was also populated
                            TripInterruptionQuotedPremium = cov.FullTermPremium
                        Case "313" 'Button: Scheduled Items #1; 5/10/2017 note: PPA only
                            'cov.ScheduledItems (no current property) will give the # of scheduled items (have to look at vehicles ScheduledItems xml to see what they are)

                            'added 5/5/2017 for GAR
                        Case "30012" 'Combo: Uninsured Motorist Bodily Injury Liability (CAP, GAR)
                            '5/9/2017 note for GAR: no FullTermPremium, CoverageLimitId 56 (file:///C:/Users/domin/Documents/GAR1000098(1143275-1)GKLL.xml)
                            'CAP_GAR_TotalCoveragesPremium = qqHelper.getSum(_CAP_GAR_TotalCoveragesPremium, cov.FullTermPremium) 'added 5/26/2017

                        Case "296" 'Combo: Underinsured Combined Single Limit; added 9/28/2018 (PPA IL only)
                            UnderinsuredCombinedSingleLimitId = cov.CoverageLimitId
                            UnderinsuredCombinedSingleLimitQuotedPremium = cov.FullTermPremium
                        Case "294" 'Combo: Uninsured Bodily Injury; added 9/28/2018 (PPA IL, HOM IN, DFR IN, FAR IN/IL)
                            UninsuredBodilyInjuryLimitId = cov.CoverageLimitId
                            UninsuredBodilyInjuryQuotedPremium = cov.FullTermPremium
                        Case "295" 'Combo: Underinsured Bodily Injury; added 9/28/2018 (PPA IL only)
                            UnderinsuredBodilyInjuryLimitId = cov.CoverageLimitId
                            UnderinsuredBodilyInjuryQuotedPremium = cov.FullTermPremium

                    End Select
                Next
            End If
        End Sub

        Public Sub MoveUpVehicleDrivers() 'added 1/20/2014 for PPA; was previously just being done in treeview
            If _OccasionalDriver2Num = "" AndAlso _OccasionalDriver3Num <> "" Then
                _OccasionalDriver2Num = _OccasionalDriver3Num
                _OccasionalDriver3Num = ""
            End If
            If _OccasionalDriver1Num = "" AndAlso _OccasionalDriver2Num <> "" Then
                _OccasionalDriver1Num = _OccasionalDriver2Num
                _OccasionalDriver2Num = ""
            End If
            'this is optional (to replace principal w/ occasional)
            'If _PrincipalDriverNum = "" AndAlso _OccasionalDriver1Num <> "" Then
            '    _PrincipalDriverNum = _OccasionalDriver1Num
            '    _OccasionalDriver1Num = ""
            '    If _OccasionalDriver2Num <> "" Then
            '        _OccasionalDriver1Num = _OccasionalDriver2Num
            '        _OccasionalDriver2Num = ""
            '        If _OccasionalDriver3Num <> "" Then
            '            _OccasionalDriver2Num = _OccasionalDriver3Num
            '            _OccasionalDriver3Num = ""
            '        End If
            '    End If
            'End If
        End Sub
        Public Function HasValidVehicleNum() As Boolean 'added 4/21/2014 for reconciliation purposes
            'If _VehicleNum <> "" AndAlso IsNumeric(_VehicleNum) = True AndAlso CInt(_VehicleNum) > 0 Then
            '    Return True
            'Else
            '    Return False
            'End If
            'updated 4/27/2014 to use common method
            'Return qqHelper.IsValidQuickQuoteIdOrNum(_VehicleNum)
            'updated 10/17/2018 to use new method
            Return HasValidVehicleNum(QuickQuoteXML.QuickQuotePackagePartType.None)
        End Function
        'added 10/17/2018
        Public Function VehicleNumForPackagePartType(ByVal packagePartType As QuickQuoteXML.QuickQuotePackagePartType) As String
            Select Case packagePartType
                Case QuickQuoteXML.QuickQuotePackagePartType.Package
                    Return _VehicleNum_MasterPart
                Case QuickQuoteXML.QuickQuotePackagePartType.GeneralLiability
                    Return _VehicleNum_CGLPart
                Case QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty
                    Return _VehicleNum_CPRPart
                Case QuickQuoteXML.QuickQuotePackagePartType.InlandMarine
                    Return _VehicleNum_CIMPart
                Case QuickQuoteXML.QuickQuotePackagePartType.Crime
                    Return _VehicleNum_CRMPart
                Case QuickQuoteXML.QuickQuotePackagePartType.Garage
                    Return _VehicleNum_GARPart
                Case Else
                    Return _VehicleNum
            End Select
        End Function
        Public Function HasValidVehicleNum(ByVal packagePartType As QuickQuoteXML.QuickQuotePackagePartType) As Boolean
            Return qqHelper.IsValidQuickQuoteIdOrNum(VehicleNumForPackagePartType(packagePartType))
        End Function
        Public Sub SetVehicleNumForPackagePartType(ByVal vehNum As String, ByVal packagePartType As QuickQuoteXML.QuickQuotePackagePartType)
            Select Case packagePartType
                Case QuickQuoteXML.QuickQuotePackagePartType.Package
                    _VehicleNum_MasterPart = vehNum
                Case QuickQuoteXML.QuickQuotePackagePartType.GeneralLiability
                    _VehicleNum_CGLPart = vehNum
                Case QuickQuoteXML.QuickQuotePackagePartType.CommercialProperty
                    _VehicleNum_CPRPart = vehNum
                Case QuickQuoteXML.QuickQuotePackagePartType.InlandMarine
                    _VehicleNum_CIMPart = vehNum
                Case QuickQuoteXML.QuickQuotePackagePartType.Crime
                    _VehicleNum_CRMPart = vehNum
                Case QuickQuoteXML.QuickQuotePackagePartType.Garage
                    _VehicleNum_GARPart = vehNum
                Case Else
                    _VehicleNum = vehNum
            End Select
        End Sub
        'added 4/24/2014 for vehSym reconciliation
        Public Sub ParseThruVehicleSymbols()
            If _VehicleSymbols IsNot Nothing AndAlso _VehicleSymbols.Count > 0 Then
                For Each s As QuickQuoteVehicleSymbol In _VehicleSymbols
                    '4/24/2014 note: should only happen when parsing xml (FinalizeQuickQuote method)... shouldn't happen in FinalizeQuickQuoteLight method
                    If _CanUseVehicleSymbolNumForVehicleSymbolReconciliation = False Then
                        If s.HasValidVehicleSymbolNum = True Then
                            _CanUseVehicleSymbolNumForVehicleSymbolReconciliation = True
                            Exit For
                        End If
                    End If
                Next
            End If
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
        'added 4/29/2014
        Public Sub ParseThruScheduledItems()
            If _ScheduledItems IsNot Nothing AndAlso _ScheduledItems.Count > 0 Then
                For Each si As QuickQuoteScheduledItem In _ScheduledItems
                    'note: should only happen when parsing xml (FinalizeQuickQuote method)... shouldn't happen in FinalizeQuickQuoteLight method
                    'updated 5/14/2014 w/ logic for CanUseScheduledItemsNumForScheduledItemReconciliation
                    If _CanUseScheduledItemsNumForScheduledItemReconciliation = False Then
                        If si.HasValidScheduledItemsNum = True Then
                            _CanUseScheduledItemsNumForScheduledItemReconciliation = True
                        End If
                    End If
                    si.ParseThruAdditionalInterests()
                Next
            End If
        End Sub

        'added 8/2/2018
        Protected Friend Sub Set_QuoteStateTakenFrom(ByVal qqState As QuickQuoteHelperClass.QuickQuoteState)
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

        Public Overrides Function ToString() As String 'added 6/29/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.Year <> "" OrElse Me.Make <> "" OrElse Me.Model <> "" Then
                    Dim yearMakeModel As String = ""
                    If Me.Year <> "" Then
                        yearMakeModel = qqHelper.appendText(yearMakeModel, Me.Year, " ")
                    End If
                    If Me.Make <> "" Then
                        yearMakeModel = qqHelper.appendText(yearMakeModel, Me.Make, " ")
                    End If
                    If Me.Model <> "" Then
                        yearMakeModel = qqHelper.appendText(yearMakeModel, Me.Model, " ")
                    End If
                    str = qqHelper.appendText(str, "yearMakeModel: " & yearMakeModel, vbCrLf)
                End If
                If Me.Vin <> "" Then
                    str = qqHelper.appendText(str, "VIN: " & Me.Vin, vbCrLf)
                End If
                If Me.Coverages IsNot Nothing AndAlso Me.Coverages.Count > 0 Then
                    str = qqHelper.appendText(str, Me.Coverages.Count.ToString & " Coverages", vbCrLf)
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
                    If _AnnualMiles IsNot Nothing Then
                        _AnnualMiles = Nothing
                    End If
                    If _ClassCode IsNot Nothing Then
                        _ClassCode = Nothing
                    End If
                    If _CostNew IsNot Nothing Then
                        _CostNew = Nothing
                    End If
                    If _CostUsed IsNot Nothing Then
                        _CostUsed = Nothing
                    End If
                    If _Coverages IsNot Nothing Then
                        If _Coverages.Count > 0 Then
                            For Each cov As QuickQuoteCoverage In _Coverages
                                cov.Dispose()
                                cov = Nothing
                            Next
                            _Coverages.Clear()
                        End If
                        _Coverages = Nothing
                    End If
                    If _DaysPerWeek IsNot Nothing Then
                        _DaysPerWeek = Nothing
                    End If
                    If _Make IsNot Nothing Then
                        _Make = Nothing
                    End If
                    If _MilesOneWay IsNot Nothing Then
                        _MilesOneWay = Nothing
                    End If
                    If _Model IsNot Nothing Then
                        _Model = Nothing
                    End If
                    If _OdometerReading IsNot Nothing Then
                        _OdometerReading = Nothing
                    End If
                    If _Vin IsNot Nothing Then
                        _Vin = Nothing
                    End If
                    If _Year IsNot Nothing Then
                        _Year = Nothing
                    End If
                    If _GaragingAddress IsNot Nothing Then
                        _GaragingAddress.Dispose()
                        _GaragingAddress = Nothing
                    End If
                    If _HasLiability_UM_UIM <> Nothing Then
                        _HasLiability_UM_UIM = Nothing
                    End If
                    If _Liability_UM_UIM_QuotedPremium IsNot Nothing Then
                        _Liability_UM_UIM_QuotedPremium = Nothing
                    End If
                    If _Liability_UM_UIM_LimitId IsNot Nothing Then
                        _Liability_UM_UIM_LimitId = Nothing
                    End If
                    If _HasMedicalPayments <> Nothing Then
                        _HasMedicalPayments = Nothing
                    End If
                    If _MedicalPaymentsQuotedPremium IsNot Nothing Then
                        _MedicalPaymentsQuotedPremium = Nothing
                    End If
                    If _MedicalPaymentsLimitId IsNot Nothing Then
                        _MedicalPaymentsLimitId = Nothing
                    End If
                    If _HasComprehensive <> Nothing Then
                        _HasComprehensive = Nothing
                    End If
                    If _ComprehensiveDeductible IsNot Nothing Then
                        _ComprehensiveDeductible = Nothing
                    End If
                    If _ComprehensiveDeductibleId IsNot Nothing Then
                        _ComprehensiveDeductibleId = Nothing
                    End If
                    If _ComprehensiveQuotedPremium IsNot Nothing Then
                        _ComprehensiveQuotedPremium = Nothing
                    End If
                    If _ComprehensiveDeductibleLimitId IsNot Nothing Then
                        _ComprehensiveDeductibleLimitId = Nothing
                    End If
                    If _HasCollision <> Nothing Then
                        _HasCollision = Nothing
                    End If
                    If _CollisionDeductible IsNot Nothing Then
                        _CollisionDeductible = Nothing
                    End If
                    If _CollisionDeductibleId IsNot Nothing Then
                        _CollisionDeductibleId = Nothing
                    End If
                    If _CollisionQuotedPremium IsNot Nothing Then
                        _CollisionQuotedPremium = Nothing
                    End If
                    If _CollisionDeductibleLimitId IsNot Nothing Then
                        _CollisionDeductibleLimitId = Nothing
                    End If
                    If _HasTowingAndLabor <> Nothing Then
                        _HasTowingAndLabor = Nothing
                    End If
                    If _TowingAndLaborQuotedPremium IsNot Nothing Then
                        _TowingAndLaborQuotedPremium = Nothing
                    End If
                    If _TowingAndLaborDeductibleLimitId IsNot Nothing Then
                        _TowingAndLaborDeductibleLimitId = Nothing
                    End If
                    If _HasRentalReimbursement <> Nothing Then
                        _HasRentalReimbursement = Nothing
                    End If
                    If _RentalReimbursementDailyReimbursement IsNot Nothing Then
                        _RentalReimbursementDailyReimbursement = Nothing
                    End If
                    If _RentalReimbursementNumberOfDays IsNot Nothing Then
                        _RentalReimbursementNumberOfDays = Nothing
                    End If
                    If _RentalReimbursementQuotedPremium IsNot Nothing Then
                        _RentalReimbursementQuotedPremium = Nothing
                    End If

                    If _PremiumFullTerm IsNot Nothing Then
                        _PremiumFullTerm = Nothing
                    End If
                    If _TerritoryNum IsNot Nothing Then
                        _TerritoryNum = Nothing
                    End If

                    If _VehicleRatingTypeId IsNot Nothing Then
                        _VehicleRatingTypeId = Nothing
                    End If
                    If _VehicleUseTypeId IsNot Nothing Then
                        _VehicleUseTypeId = Nothing
                    End If
                    If _VehicleUsageTypeId IsNot Nothing Then
                        _VehicleUsageTypeId = Nothing
                    End If
                    If _FarmUseCodeTypeId IsNot Nothing Then
                        _FarmUseCodeTypeId = Nothing
                    End If
                    If _UseCodeTypeId IsNot Nothing Then
                        _UseCodeTypeId = Nothing
                    End If
                    If _OperatorTypeId IsNot Nothing Then
                        _OperatorTypeId = Nothing
                    End If
                    If _OperatorUseTypeId IsNot Nothing Then
                        _OperatorUseTypeId = Nothing
                    End If
                    If _RadiusTypeId IsNot Nothing Then
                        _RadiusTypeId = Nothing
                    End If
                    If _SizeTypeId IsNot Nothing Then
                        _SizeTypeId = Nothing
                    End If
                    If _SecondaryClassTypeId IsNot Nothing Then
                        _SecondaryClassTypeId = Nothing
                    End If
                    If _SecondaryClassUsageTypeId IsNot Nothing Then
                        _SecondaryClassUsageTypeId = Nothing
                    End If
                    If _UsedInDumping <> Nothing Then
                        _UsedInDumping = Nothing
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

                    If _UninsuredMotoristLiabilityQuotedPremium IsNot Nothing Then
                        _UninsuredMotoristLiabilityQuotedPremium = Nothing
                    End If
                    If _UninsuredMotoristLiabilityLimitId IsNot Nothing Then
                        _UninsuredMotoristLiabilityLimitId = Nothing
                    End If
                    If _UnderinsuredMotoristBodilyInjuryLiabilityQuotedPremium IsNot Nothing Then
                        _UnderinsuredMotoristBodilyInjuryLiabilityQuotedPremium = Nothing
                    End If

                    If _TrailerTypeId IsNot Nothing Then
                        _TrailerTypeId = Nothing
                    End If
                    If _StatedAmount IsNot Nothing Then
                        _StatedAmount = Nothing
                    End If

                    If _PerformanceTypeId IsNot Nothing Then
                        _PerformanceTypeId = Nothing
                    End If
                    If _BodyTypeId IsNot Nothing Then
                        _BodyTypeId = Nothing
                    End If
                    If _PrincipalDriverNum IsNot Nothing Then
                        _PrincipalDriverNum = Nothing
                    End If
                    If _OccasionalDriver1Num IsNot Nothing Then
                        _OccasionalDriver1Num = Nothing
                    End If
                    If _OccasionalDriver2Num IsNot Nothing Then
                        _OccasionalDriver2Num = Nothing
                    End If
                    If _OccasionalDriver3Num IsNot Nothing Then
                        _OccasionalDriver3Num = Nothing
                    End If
                    If _BodilyInjuryLiabilityLimitId IsNot Nothing Then
                        _BodilyInjuryLiabilityLimitId = Nothing
                    End If
                    If _BodilyInjuryLiabilityQuotedPremium IsNot Nothing Then
                        _BodilyInjuryLiabilityQuotedPremium = Nothing
                    End If
                    If _PropertyDamageLimitId IsNot Nothing Then
                        _PropertyDamageLimitId = Nothing
                    End If
                    If _PropertyDamageQuotedPremium IsNot Nothing Then
                        _PropertyDamageQuotedPremium = Nothing
                    End If
                    If _UninsuredCombinedSingleLimitId IsNot Nothing Then
                        _UninsuredCombinedSingleLimitId = Nothing
                    End If
                    If _UninsuredCombinedSingleQuotedPremium IsNot Nothing Then
                        _UninsuredCombinedSingleQuotedPremium = Nothing
                    End If
                    If _UninsuredMotoristPropertyDamageLimitId IsNot Nothing Then
                        _UninsuredMotoristPropertyDamageLimitId = Nothing
                    End If
                    If _UninsuredMotoristPropertyDamageQuotedPremium IsNot Nothing Then
                        _UninsuredMotoristPropertyDamageQuotedPremium = Nothing
                    End If
                    _HasUninsuredMotoristPropertyDamage = Nothing 'added 10/30/2018 for CAP IL; covCodeId 9; CheckBox
                    If _UninsuredMotoristPropertyDamageDeductibleLimitId IsNot Nothing Then
                        _UninsuredMotoristPropertyDamageDeductibleLimitId = Nothing
                    End If
                    If _UninsuredMotoristPropertyDamageDeductibleQuotedPremium IsNot Nothing Then
                        _UninsuredMotoristPropertyDamageDeductibleQuotedPremium = Nothing
                    End If
                    If _HasPollutionLiabilityBroadenedCoverage <> Nothing Then
                        _HasPollutionLiabilityBroadenedCoverage = Nothing
                    End If
                    If _PollutionLiabilityBroadenedCoverageQuotedPremium IsNot Nothing Then
                        _PollutionLiabilityBroadenedCoverageQuotedPremium = Nothing
                    End If
                    If _TransportationExpenseLimitId IsNot Nothing Then
                        _TransportationExpenseLimitId = Nothing
                    End If
                    If _TransportationExpenseQuotedPremium IsNot Nothing Then
                        _TransportationExpenseQuotedPremium = Nothing
                    End If
                    If _HasAutoLoanOrLease <> Nothing Then
                        _HasAutoLoanOrLease = Nothing
                    End If
                    If _AutoLoanOrLeaseQuotedPremium IsNot Nothing Then
                        _AutoLoanOrLeaseQuotedPremium = Nothing
                    End If
                    If _TapesAndRecordsLimitId IsNot Nothing Then
                        _TapesAndRecordsLimitId = Nothing
                    End If
                    If _TapesAndRecordsQuotedPremium IsNot Nothing Then
                        _TapesAndRecordsQuotedPremium = Nothing
                    End If
                    If _SoundEquipmentLimit IsNot Nothing Then
                        _SoundEquipmentLimit = Nothing
                    End If
                    If _SoundEquipmentQuotedPremium IsNot Nothing Then
                        _SoundEquipmentQuotedPremium = Nothing
                    End If
                    If _ElectronicEquipmentLimit IsNot Nothing Then
                        _ElectronicEquipmentLimit = Nothing
                    End If
                    If _ElectronicEquipmentQuotedPremium IsNot Nothing Then
                        _ElectronicEquipmentQuotedPremium = Nothing
                    End If
                    If _TripInterruptionLimitId IsNot Nothing Then
                        _TripInterruptionLimitId = Nothing
                    End If
                    If _TripInterruptionQuotedPremium IsNot Nothing Then
                        _TripInterruptionQuotedPremium = Nothing
                    End If
                    If _ScheduledItems IsNot Nothing Then
                        If _ScheduledItems.Count > 0 Then
                            For Each si As QuickQuoteScheduledItem In _ScheduledItems
                                si.Dispose()
                                si = Nothing
                            Next
                            _ScheduledItems.Clear()
                        End If
                        _ScheduledItems = Nothing
                    End If
                    If _AntiLockTypeId IsNot Nothing Then
                        _AntiLockTypeId = Nothing
                    End If
                    If _ActualCashValue IsNot Nothing Then
                        _ActualCashValue = Nothing
                    End If
                    If _AntiTheftTypeId IsNot Nothing Then
                        _AntiTheftTypeId = Nothing
                    End If
                    If _CubicCentimeters IsNot Nothing Then
                        _CubicCentimeters = Nothing
                    End If
                    If _CustomEquipmentAmount IsNot Nothing Then
                        _CustomEquipmentAmount = Nothing
                    End If
                    If _DamageRemarks IsNot Nothing Then
                        _DamageRemarks = Nothing
                    End If
                    If _DamageYesNoId IsNot Nothing Then
                        _DamageYesNoId = Nothing
                    End If
                    If _DriverOutOfStateSurcharge <> Nothing Then
                        _DriverOutOfStateSurcharge = Nothing
                    End If
                    If _GrossVehicleWeight IsNot Nothing Then
                        _GrossVehicleWeight = Nothing
                    End If
                    If _MultiCar <> Nothing Then
                        _MultiCar = Nothing
                    End If
                    If _NonOwnedNamed <> Nothing Then
                        _NonOwnedNamed = Nothing
                    End If
                    If _PurchasedDate IsNot Nothing Then
                        _PurchasedDate = Nothing
                    End If
                    If _RegisteredStateId IsNot Nothing Then
                        _RegisteredStateId = Nothing
                    End If
                    If _RestraintTypeId IsNot Nothing Then
                        _RestraintTypeId = Nothing
                    End If
                    If _VehicleTypeId IsNot Nothing Then
                        _VehicleTypeId = Nothing
                    End If
                    If _VehicleValueId IsNot Nothing Then
                        _VehicleValueId = Nothing
                    End If
                    If _NonOwned <> Nothing Then
                        _NonOwned = Nothing
                    End If

                    'added 2/18/2014
                    If _HasConvertedCoverages <> Nothing Then
                        _HasConvertedCoverages = Nothing
                    End If

                    'added 3/26/2014
                    If _ComprehensiveCoverageOnly <> Nothing Then
                        _ComprehensiveCoverageOnly = Nothing
                    End If

                    'added 4/15/2014
                    If _VehicleSymbols IsNot Nothing Then
                        If _VehicleSymbols.Count > 0 Then
                            For Each vs As QuickQuoteVehicleSymbol In _VehicleSymbols
                                vs.Dispose()
                                vs = Nothing
                            Next
                            _VehicleSymbols.Clear()
                        End If
                        _VehicleSymbols = Nothing
                    End If

                    If _VehicleNum IsNot Nothing Then 'added 4/21/2014 for reconciliation purposes
                        _VehicleNum = Nothing
                    End If
                    If _HasVehicleMakeModelYearChanged <> Nothing Then 'added 4/21/2014
                        _HasVehicleMakeModelYearChanged = Nothing
                    End If
                    If _CanUseVehicleSymbolNumForVehicleSymbolReconciliation <> Nothing Then 'added 4/24/2014
                        _CanUseVehicleSymbolNumForVehicleSymbolReconciliation = Nothing
                    End If
                    If _CanUseAdditionalInterestNumForAdditionalInterestReconciliation <> Nothing Then 'added 4/29/2014
                        _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = Nothing
                    End If
                    If _CanUseScheduledItemsNumForScheduledItemReconciliation <> Nothing Then 'added 5/14/2014
                        _CanUseScheduledItemsNumForScheduledItemReconciliation = Nothing
                    End If
                    'added 10/17/2018 for multi-state
                    qqHelper.DisposeString(_VehicleNum_MasterPart)
                    qqHelper.DisposeString(_VehicleNum_CGLPart)
                    qqHelper.DisposeString(_VehicleNum_CPRPart)
                    qqHelper.DisposeString(_VehicleNum_CIMPart)
                    qqHelper.DisposeString(_VehicleNum_CRMPart)
                    qqHelper.DisposeString(_VehicleNum_GARPart)

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

                    'added 5/26/2017
                    qqHelper.DisposeString(_TotalCoveragesPremium)
                    qqHelper.DisposeString(_CAP_GAR_TotalCoveragesPremium)

                    'added 8/2/2018
                    _QuoteStateTakenFrom = Nothing

                    'added 9/28/2018
                    qqHelper.DisposeString(_UnderinsuredCombinedSingleLimitId) 'covCodeId 296; PPA IL only
                    qqHelper.DisposeString(_UnderinsuredCombinedSingleLimitQuotedPremium) 'covCodeId 296; PPA IL only
                    qqHelper.DisposeString(_UninsuredBodilyInjuryLimitId) 'covCodeId 294; PPA IL, HOM IN, DFR IN, FAR IN/IL
                    qqHelper.DisposeString(_UninsuredBodilyInjuryQuotedPremium) 'covCodeId 294; PPA IL, HOM IN, DFR IN, FAR IN/IL
                    qqHelper.DisposeString(_UnderinsuredBodilyInjuryLimitId) 'covCodeId 295; PPA IL only
                    qqHelper.DisposeString(_UnderinsuredBodilyInjuryQuotedPremium) 'covCodeId 295; PPA IL only

                    'added 1/16/2019
                    _DisplayNum = Nothing
                    _OriginalDisplayNum = Nothing
                    _OkayToUseDisplayNum = Nothing

                    qqHelper.DisposeString(_DetailStatusCode) 'added 5/15/2019

                    'added 5/22/2019
                    qqHelper.DisposeString(_AddedDate)
                    qqHelper.DisposeString(_EffectiveDate)
                    qqHelper.DisposeString(_LastModifiedDate)
                    qqHelper.DisposeString(_PCAdded_Date)
                    qqHelper.DisposeString(_AddedImageNum) 'added 7/31/2019

                    qqHelper.DisposeString(_UnderinsuredMotoristBodilyInjuryLiabilityLimitId)

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
