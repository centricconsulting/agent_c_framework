Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store coverage information
    ''' </summary>
    ''' <remarks>can be found under several different objects... policy (QuickQuoteObject), locations, buildings, vehicles, etc.</remarks>
    <Serializable()> _
    Public Class QuickQuoteCoverage
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _AnnualPremium As String
        Private _EstimatedPremium As String
        Private _FullTermPremium As String
        Private _PolicyId As String
        Private _PolicyImageNum As String
        Private _WrittenPremium As String
        Private _Checkbox As Boolean
        Private _Deductible As String
        Private _Manual As Boolean
        Private _ManualLimitAmount As String
        Private _ManualLimitIncluded As String
        Private _ManualLimitIncreased As String 'added 7/30/2013 for HOM loc coverages
        'Private _ClassificationCode As String'replaced w/ new object variable 9/27/2012
        Private _CoverageCode As String
        Private _CoverageCodeId As String
        Private _CoverageLimitId As String
        Private _DeductibleTypeId As String '2/13/2014 note: not in the xml... wasn't ever trying to use it anywhere
        Private _Description As String
        Private _DescriptionInformation As String '3/9/2017 - BOP stuff
        Private _EffectiveDate As String
        Private _ExpirationDate As String
        Private _SubCoverageCodeId As String

        'CoverageDetail fields
        Private _NumberOfEmployees As String
        Private _NumberOfLocations As String
        Private _AutomaticIncreasePercentTypeId As String
        Private _ValuationMethodTypeId As String
        Private _IsIncludedInBlanketRating As Boolean
        Private _IsMineSubsidence As Boolean
        Private _IsCondoCommercialUnitOwnersCoverage As Boolean
        Private _PropertyClassificationTypeId As String
        Private _IsRefrigerationMaintenanceAgreement As Boolean
        Private _IsBreakdownOrContamination As Boolean
        Private _IsPowerOutage As Boolean

        Private _DeductiblePerTypeId As String

        Private _NumberOfSwimmingPools As String
        Private _NumberOfAmusementAreas As String '3/9/2017 - BOP stuff

        Private _DeductibleId As String

        Private _CoverageAdditionalInfoRecords As Generic.List(Of QuickQuoteCoverageAdditionalInfoRecord)

        'Private _NumberOfFullTimeEmployees As String'same as NumberOfEmployees
        Private _NumberOfPartTimeEmployees As String

        'added 7/12/2012 for GL
        Private _ManuallyRated As Boolean
        Private _ManualPremium As String
        'added 7/13/2012 for GL
        Private _NumberOfBurials As String
        Private _NumberOfBodies As String
        Private _NumberOfPastoralCounselors As String

        'added 7/18/2012 for GL liquor liability
        Private _SalesClubs As String
        Private _SalesManufacturers As String
        Private _SalesPackageStores As String
        Private _SalesRestaurants As String

        'added 7/19/2012 for App Gap
        Private _Comments As String
        Private _NameInformation As String
        Private _HasWaiverOfSubrogation As Boolean

        'added 8/22/2012 for GL
        Private _DeductibleCategoryTypeId As String

        'added 8/29/2012 for employee benefits liability
        Private _RetroactiveDate As String 'inside CoverageDetail

        'added 8/31/2012 for vehicles and CAP in general (CoverageDetail)
        Private _DailyReimbursement As String
        Private _NumberOfDays As String
        Private _NamedInsuredsBusinessTypeId As String
        Private _ExtendNonOwnershipRatingTypeId As String
        Private _CoverageTypeId As String
        Private _OtherThanCollisionSubTypeId As String
        Private _OtherThanCollisionTypeId As String
        Private _IfAnyBasis As Boolean

        'added 9/12/2012 for App Gap (Additional Insured designation of premises; CoverageDetail)
        Private _AddressInformation As String

        'added 9/27/2012 for CPR
        Private _CommercialOccupancyTypeId As String '(CoverageDetail)
        Private _ClassificationCode As QuickQuoteClassificationCode 'added 9/27/2012 for CPR; replaced previous string variable
        Private _IsEarthquakeApplies As Boolean '(CoverageDetail)
        Private _CauseOfLossTypeId As String 'example 3 = Special Form Including Theft (CauseOfLossType table); CoverageDetail
        Private _CoinsuranceTypeId As String 'example 5 = 80% (CoinsuranceType table); CoverageDetail
        Private _RatingTypeId As String 'CoverageDetail
        Private _InflationGuardTypeId As String 'InflationGuardType table; CoverageDetail
        Private _BusinessPropertyTypeId As String 'BusinessPropertyType table; CoverageDetail
        Private _RiskTypeId As String 'RiskType table; CoverageDetail
        'added 10/9/2012 for CPR
        Private _MonthlyPeriodTypeId As String
        Private _BusinessIncomeTypeId As String
        'more 10/9/2012 for CPR (specific rates)
        Private _LossCost As String
        Private _Rate As String
        Private _WaitingPeriodTypeId As String 'added 10/10/2012 for CPR (building Business Income Cov)

        'added 10/19/2012 for GL
        Private _ProductDescription As String

        'added 10/23/2012 for CPR (building Personal Property - CoverageDetail)
        Private _PersonalPropertyRateGradeTypeId As String 'Diamond's PersonalPropertyRateGradeType table

        'added 11/5/2012 for GL (ELPa Rates - CoverageDetail)
        Private _ManualElpaRate As String
        'added 11/26/2012 for GL
        Private _ExcludeProductsCompletedOperations As Boolean
        'added 12/3/2012 for GL
        Private _AggregateLimit As String

        'added 1/2/2013 for CPR
        Private _DoesYardRateApplyTypeId As String 'CoverageDetail

        'added 3/20/2013 for CPR
        Private _ConstructionTypeId As String 'CoverageDetail
        Private _FeetToFireHydrant As String 'CoverageDetail
        Private _MilesToFireDepartment As String 'CoverageDetail
        Private _SpecialClassCodeTypeId As String 'CoverageDetail
        Private _ProtectionClassId As String 'CoverageDetail

        'added 2/13/2014 for determining when to include CoverageDetail
        Private _IncludeCoverageDetail As QuickQuoteHelperClass.QuickQuoteYesNoMaybeType

        'added 2/17/2014... CoverageDetail
        Private _CoverageBasisTypeId As String

        'added 12/4/2014
        Private _Calc As String

        'added 1/20/2015 (Comm IM / Crime; Farm); note: will need to update HasCoverageDetail method to include the new CoverageDetail nodes below if any are sent to Diamond
        Private _ASLId As String
        Private _MinimumPremiumAdjustment As String 'CoverageDetail
        Private _NumberOfContractors As String 'CoverageDetail
        Private _NumberOfExcludedEmployees As String 'CoverageDetail
        Private _NumberOfIndependentContractors As String 'CoverageDetail
        Private _NumberOfRatableEmployees As String 'CoverageDetail
        Private _NumberOfPremises As String 'CoverageDetail
        Private _NumberOfTemporaryWorkers As String 'CoverageDetail
        Private _PercentOfWorkers As String 'CoverageDetail
        Private _ShouldSyncWithMasterCoverage As Boolean 'CoverageDetail
        Private _AdditionalInterests As List(Of QuickQuoteAdditionalInterest) 'note: currently just have parsing logic and Diamond service logic... no comparative rater logic (xml writing) since there currently aren't Write methods for Coverages or ScheduledCoverages (just being done from parent node)
        Private _CanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean
        Private _Make As String 'CoverageDetail
        Private _ManufacturerName As String 'CoverageDetail
        Private _Model As String 'CoverageDetail
        Private _SerialNumber As String 'CoverageDetail
        Private _VIN As String 'CoverageDetail
        Private _Year As String 'CoverageDetail
        'added 2/9/2015
        Private _IsNamedPerils As Boolean 'CoverageDetail
        'added 2/10/2015
        Private _ExcludeEarthquake As Boolean 'CoverageDetail
        'added 3/3/2015 for Farm
        Private _DescriptionOfOperations As String 'CoverageDetail
        Private _DesignatedJobSite As String 'CoverageDetail
        'added 3/17/2015 for CIM
        Private _HasLoadingUnloading As Boolean 'CoverageDetail
        Private _NumberOfVehicles As String 'CoverageDetail
        Private _NumberOfShipments As String 'CoverageDetail
        'added 3/18/2015 for CIM
        Private _OperatingRadius As String 'CoverageDetail
        'added 3/23/2015 for CIM
        Private _City As String 'CoverageDetail
        Private _ExcludedEggs As Boolean 'CoverageDetail
        Private _ExcludedFursOrFurTrimmedGarments As Boolean 'CoverageDetail
        Private _ExcludedLiquor As Boolean 'CoverageDetail
        Private _ExcludedLivestockOrPoultry As Boolean 'CoverageDetail
        Private _ExcludedTobacco As Boolean 'CoverageDetail
        'Private _ProductDescription As String 'CoverageDetail; already coded above
        Private _StateId As String 'CoverageDetail; static data
        Private _WithinMiles As String 'CoverageDetail
        'added 3/26/2015 for CIM
        Private _MaximumDeductible As String 'CoverageDetail
        Private _MinimumDeductible As String 'CoverageDetail
        Private _IsIndoor As Boolean 'CoverageDetail
        'added 3/30/2015 for CRM
        Private _ScheduledTextCollection As List(Of QuickQuoteScheduledText)
        Private _CanUseScheduledTextNumForScheduledTextReconciliation As Boolean
        Private _FaithfulPerformanceOfDutyTypeId As String 'CoverageDetail; static data
        Private _NumberOfAdditionalPremises As String 'CoverageDetail
        Private _LimitTypeId As String 'CoverageDetail; static data
        Private _EmployeeTheftScheduleTypeId As String 'CoverageDetail; static data
        'added 3/31/2015 for CRM
        Private _NumberOfCardHolders As String 'CoverageDetail
        Private _PremiumChargeTypeId As String 'CoverageDetail; static data
        Private _OverrideFullyEarned As Boolean
        Private _BasisTypeId As String 'CoverageDetail; static data
        Private _IncludeSellingPrice As Boolean 'CoverageDetail
        Private _IsIncludeGuestsProperty As Boolean 'CoverageDetail
        Private _IsLimitToRobberyOnly As Boolean 'CoverageDetail
        Private _IsRequireRecordOfChecks As Boolean 'CoverageDetail
        Private _FromDate As String 'CoverageDetail; /DateTime
        Private _ToDate As String 'CoverageDetail; /DateTime

        'added 5/11/2015 (specific to peak season coverages; found under scheduled personal properties for Farm); we're currently not setting these Coverage properties but will if flag is True
        Private _UseEffectiveDateAndExpirationDate As Boolean

        'added 5/12/2015 for Farm (F & G Optional Covs)
        Private _OriginalCost As String

        'added 5/14/2015 for Farm (may need to set these for some of the different covs... testing policy-level now)
        Private _ApplyToWrittenPremium As Boolean
        Private _Exposure As String

        'added 6/8/2015 for testing latest CPP enhancements
        Private _CoverageDescription As String 'CoverageDetail

        'added 7/15/2015 for missed CIM requirement (on small tools floater)
        Private _IsEmployeeTools As Boolean 'CoverageDetail
        Private _IsToolsLeasedOrRented As Boolean 'CoverageDetail

        'added 5/3/2016 for Farm Machinery coverage option (under UnscheduledPersonalProperties)
        Private _IsLimitedPerilsExtendedCoverage As Boolean 'CoverageDetail

        Private _IsAgreedValue As Boolean 'Bug 4845 Matt A 2/1/2016; 3/9/2017 - included in this library w/ BOP updates
        Private _Receipts As String '3/9/2017 - BOP stuff

        Private _PriorGrossReceipts As String '3/9/2017 - BOP stuff
        Private _LiquorLiabilityClassCodeTypeId As String '3/9/2017 - BOP stuff

        'added 5/4/2017 for CIM (Golf)
        Private _CoveredHolesFairways As String 'covDetail; int (actually string)
        Private _CoveredHolesGreens As String 'covDetail; int (actually string)
        Private _CoveredHolesTees As String 'covDetail; int (actually string)
        Private _CoveredHolesTrees As String 'covDetail; int (actually string)
        Private _IsFairways As Boolean 'covDetail
        Private _IsGreens As Boolean 'covDetail
        Private _IsTees As Boolean 'covDetail
        Private _IsTrees As Boolean 'covDetail
        Private _NumberOfCarts As String 'covDetail; int

        'added 5/5/2017 for GAR
        Private _AggregateLiabilityIncrementTypeId As String 'covDetail; int
        Private _MedicalPaymentsTypeId As String 'covDetail; int
        'added 5/11/2017 for GAR
        Private _NumberOfPlates As String 'covDetail; int

        'added 10/30/2017
        Private _AdjustmentFactor As String 'covDetail; decimal; won't update HasCoverageDetail to use it until code is updating in in QuickQuoteXML routines

        Private _DetailStatusCode As String 'added 3/30/2018

        Private _OverwriteExposure As Boolean 'added 4/10/2018 for CPP Contractors and Manufacturers enhancements

        Private _IsDwellingStructure As Boolean 'added 11/8/2018 for CPR mine subsidence; covDetail

        Private _Payroll As String

        Private _OwnerOccupiedPercentageId As String 'covdetail


        Public Property AnnualPremium As String
            Get
                Return _AnnualPremium
            End Get
            Set(value As String)
                _AnnualPremium = value
            End Set
        End Property
        Public Property EstimatedPremium As String
            Get
                Return _EstimatedPremium
            End Get
            Set(value As String)
                _EstimatedPremium = value
            End Set
        End Property
        Public Property FullTermPremium As String
            Get
                Return _FullTermPremium
            End Get
            Set(value As String)
                _FullTermPremium = value
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
        Public Property WrittenPremium As String
            Get
                Return _WrittenPremium
            End Get
            Set(value As String)
                _WrittenPremium = value
            End Set
        End Property
        Public Property Checkbox As Boolean
            Get
                Return _Checkbox
            End Get
            Set(value As Boolean)
                _Checkbox = value
            End Set
        End Property
        Public Property Deductible As String
            Get
                Return _Deductible
            End Get
            Set(value As String)
                _Deductible = value
            End Set
        End Property
        Public Property Manual As Boolean
            Get
                Return _Manual
            End Get
            Set(value As Boolean)
                _Manual = value
            End Set
        End Property
        Public Property ManualLimitAmount As String
            Get
                Return _ManualLimitAmount
            End Get
            Set(value As String)
                _ManualLimitAmount = value
            End Set
        End Property
        Public Property ManualLimitIncluded As String
            Get
                Return _ManualLimitIncluded
            End Get
            Set(value As String)
                _ManualLimitIncluded = value
            End Set
        End Property
        Public Property ManualLimitIncreased As String
            Get
                Return _ManualLimitIncreased
            End Get
            Set(value As String)
                _ManualLimitIncreased = value
            End Set
        End Property
        'Public Property ClassificationCode As String
        '    Get
        '        Return _ClassificationCode
        '    End Get
        '    Set(value As String)
        '        _ClassificationCode = value
        '    End Set
        'End Property
        Public Property CoverageCode As String
            Get
                Return _CoverageCode
            End Get
            Set(value As String)
                _CoverageCode = value
            End Set
        End Property
        Public Property CoverageCodeId As String
            Get
                Return _CoverageCodeId
            End Get
            Set(value As String)
                _CoverageCodeId = value
            End Set
        End Property
        Public Property CoverageLimitId As String
            Get
                Return _CoverageLimitId
            End Get
            Set(value As String)
                _CoverageLimitId = value
            End Set
        End Property
        Public Property DeductibleTypeId As String '2/13/2014 note: not in the xml... wasn't ever trying to use it anywhere
            Get
                Return _DeductibleTypeId
            End Get
            Set(value As String)
                _DeductibleTypeId = value
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
        Public Property DescriptionInformation As String '3/9/2017 - BOP stuff
            Get
                Return _DescriptionInformation
            End Get
            Set(value As String)
                _DescriptionInformation = value
            End Set
        End Property
        Public Property EffectiveDate As String
            Get
                Return _EffectiveDate
            End Get
            Set(value As String)
                _EffectiveDate = value
                qqHelper.ConvertToShortDate(_EffectiveDate)
            End Set
        End Property
        Public Property ExpirationDate As String
            Get
                Return _ExpirationDate
            End Get
            Set(value As String)
                _ExpirationDate = value
                qqHelper.ConvertToShortDate(_ExpirationDate)
            End Set
        End Property
        Public Property SubCoverageCodeId As String
            Get
                Return _SubCoverageCodeId
            End Get
            Set(value As String)
                _SubCoverageCodeId = value
            End Set
        End Property

        Public Property NumberOfEmployees As String
            Get
                Return _NumberOfEmployees
            End Get
            Set(value As String)
                _NumberOfEmployees = value
            End Set
        End Property
        Public Property NumberOfLocations As String
            Get
                Return _NumberOfLocations
            End Get
            Set(value As String)
                _NumberOfLocations = value
            End Set
        End Property
        Public Property AutomaticIncreasePercentTypeId As String
            Get
                Return _AutomaticIncreasePercentTypeId
            End Get
            Set(value As String)
                _AutomaticIncreasePercentTypeId = value
            End Set
        End Property
        Public Property ValuationMethodTypeId As String
            Get
                Return _ValuationMethodTypeId
            End Get
            Set(value As String)
                _ValuationMethodTypeId = value
            End Set
        End Property
        Public Property IsIncludedInBlanketRating As Boolean
            Get
                Return _IsIncludedInBlanketRating
            End Get
            Set(value As Boolean)
                _IsIncludedInBlanketRating = value
            End Set
        End Property
        Public Property IsMineSubsidence As Boolean
            Get
                Return _IsMineSubsidence
            End Get
            Set(value As Boolean)
                _IsMineSubsidence = value
            End Set
        End Property
        Public Property IsCondoCommercialUnitOwnersCoverage As Boolean
            Get
                Return _IsCondoCommercialUnitOwnersCoverage
            End Get
            Set(value As Boolean)
                _IsCondoCommercialUnitOwnersCoverage = value
            End Set
        End Property
        Public Property PropertyClassificationTypeId As String
            Get
                Return _PropertyClassificationTypeId
            End Get
            Set(value As String)
                _PropertyClassificationTypeId = value
            End Set
        End Property
        Public Property IsRefrigerationMaintenanceAgreement As Boolean
            Get
                Return _IsRefrigerationMaintenanceAgreement
            End Get
            Set(value As Boolean)
                _IsRefrigerationMaintenanceAgreement = value
            End Set
        End Property
        Public Property IsBreakdownOrContamination As Boolean
            Get
                Return _IsBreakdownOrContamination
            End Get
            Set(value As Boolean)
                _IsBreakdownOrContamination = value
            End Set
        End Property
        Public Property IsPowerOutage As Boolean
            Get
                Return _IsPowerOutage
            End Get
            Set(value As Boolean)
                _IsPowerOutage = value
            End Set
        End Property

        Public Property DeductiblePerTypeId As String
            Get
                Return _DeductiblePerTypeId
            End Get
            Set(value As String)
                _DeductiblePerTypeId = value
            End Set
        End Property

        Public Property NumberOfSwimmingPools As String
            Get
                Return _NumberOfSwimmingPools
            End Get
            Set(value As String)
                _NumberOfSwimmingPools = value
            End Set
        End Property
        Public Property NumberOfAmusementAreas As String '3/9/2017 - BOP stuff
            Get
                Return _NumberOfAmusementAreas
            End Get
            Set(value As String)
                _NumberOfAmusementAreas = value
            End Set
        End Property

        Public Property DeductibleId As String
            Get
                Return _DeductibleId
            End Get
            Set(value As String)
                _DeductibleId = value
            End Set
        End Property

        Public Property CoverageAdditionalInfoRecords As Generic.List(Of QuickQuoteCoverageAdditionalInfoRecord)
            Get
                SetParentOfListItems(_CoverageAdditionalInfoRecords, "{663B7C7B-F2AC-4BF6-965A-D30F41A04193}")
                Return _CoverageAdditionalInfoRecords
            End Get
            Set(value As Generic.List(Of QuickQuoteCoverageAdditionalInfoRecord))
                _CoverageAdditionalInfoRecords = value
                SetParentOfListItems(_CoverageAdditionalInfoRecords, "{663B7C7B-F2AC-4BF6-965A-D30F41A04193}")
            End Set
        End Property

        'Public Property NumberOfFullTimeEmployees As String
        '    Get
        '        Return _NumberOfFullTimeEmployees
        '    End Get
        '    Set(value As String)
        '        _NumberOfFullTimeEmployees = value
        '    End Set
        'End Property
        Public Property NumberOfPartTimeEmployees As String
            Get
                Return _NumberOfPartTimeEmployees
            End Get
            Set(value As String)
                _NumberOfPartTimeEmployees = value
            End Set
        End Property

        Public Property ManuallyRated As Boolean
            Get
                Return _ManuallyRated
            End Get
            Set(value As Boolean)
                _ManuallyRated = value
            End Set
        End Property
        Public Property ManualPremium As String
            Get
                Return _ManualPremium
            End Get
            Set(value As String)
                _ManualPremium = value
            End Set
        End Property
        Public Property NumberOfBurials As String
            Get
                Return _NumberOfBurials
            End Get
            Set(value As String)
                _NumberOfBurials = value
            End Set
        End Property
        Public Property NumberOfBodies As String
            Get
                Return _NumberOfBodies
            End Get
            Set(value As String)
                _NumberOfBodies = value
            End Set
        End Property
        Public Property NumberOfPastoralCounselors As String
            Get
                Return _NumberOfPastoralCounselors
            End Get
            Set(value As String)
                _NumberOfPastoralCounselors = value
            End Set
        End Property
        Public Property SalesClubs As String
            Get
                Return _SalesClubs
            End Get
            Set(value As String)
                _SalesClubs = value
            End Set
        End Property
        Public Property SalesManufacturers As String
            Get
                Return _SalesManufacturers
            End Get
            Set(value As String)
                _SalesManufacturers = value
            End Set
        End Property
        Public Property SalesPackageStores As String
            Get
                Return _SalesPackageStores
            End Get
            Set(value As String)
                _SalesPackageStores = value
            End Set
        End Property
        Public Property SalesRestaurants As String
            Get
                Return _SalesRestaurants
            End Get
            Set(value As String)
                _SalesRestaurants = value
            End Set
        End Property

        Public Property Comments As String
            Get
                Return _Comments
            End Get
            Set(value As String)
                _Comments = value
            End Set
        End Property
        Public Property NameInformation As String
            Get
                Return _NameInformation
            End Get
            Set(value As String)
                _NameInformation = value
            End Set
        End Property
        Public Property HasWaiverOfSubrogation As Boolean
            Get
                Return _HasWaiverOfSubrogation
            End Get
            Set(value As Boolean)
                _HasWaiverOfSubrogation = value
            End Set
        End Property

        Public Property DeductibleCategoryTypeId As String
            Get
                Return _DeductibleCategoryTypeId
            End Get
            Set(value As String)
                _DeductibleCategoryTypeId = value
            End Set
        End Property

        Public Property RetroactiveDate As String
            Get
                Return _RetroactiveDate
            End Get
            Set(value As String)
                _RetroactiveDate = value
                qqHelper.ConvertToShortDate(_RetroactiveDate)
            End Set
        End Property

        Public Property DailyReimbursement As String
            Get
                Return _DailyReimbursement
            End Get
            Set(value As String)
                _DailyReimbursement = value
            End Set
        End Property
        Public Property NumberOfDays As String
            Get
                Return _NumberOfDays
            End Get
            Set(value As String)
                _NumberOfDays = value
            End Set
        End Property
        Public Property NamedInsuredsBusinessTypeId As String
            Get
                Return _NamedInsuredsBusinessTypeId
            End Get
            Set(value As String)
                _NamedInsuredsBusinessTypeId = value
            End Set
        End Property
        Public Property ExtendNonOwnershipRatingTypeId As String
            Get
                Return _ExtendNonOwnershipRatingTypeId
            End Get
            Set(value As String)
                _ExtendNonOwnershipRatingTypeId = value
            End Set
        End Property
        Public Property CoverageTypeId As String
            Get
                Return _CoverageTypeId
            End Get
            Set(value As String)
                _CoverageTypeId = value
            End Set
        End Property
        Public Property OtherThanCollisionSubTypeId As String
            Get
                Return _OtherThanCollisionSubTypeId
            End Get
            Set(value As String)
                _OtherThanCollisionSubTypeId = value
            End Set
        End Property
        Public Property OtherThanCollisionTypeId As String
            Get
                Return _OtherThanCollisionTypeId
            End Get
            Set(value As String)
                _OtherThanCollisionTypeId = value
            End Set
        End Property
        Public Property IfAnyBasis As Boolean
            Get
                Return _IfAnyBasis
            End Get
            Set(value As Boolean)
                _IfAnyBasis = value
            End Set
        End Property

        Public Property AddressInformation As String
            Get
                Return _AddressInformation
            End Get
            Set(value As String)
                _AddressInformation = value
            End Set
        End Property

        Public Property CommercialOccupancyTypeId As String
            Get
                Return _CommercialOccupancyTypeId
            End Get
            Set(value As String)
                _CommercialOccupancyTypeId = value
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
        Public Property IsEarthquakeApplies As Boolean
            Get
                Return _IsEarthquakeApplies
            End Get
            Set(value As Boolean)
                _IsEarthquakeApplies = value
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
            End Set
        End Property
        Public Property RatingTypeId As String
            Get
                Return _RatingTypeId
            End Get
            Set(value As String)
                _RatingTypeId = value
            End Set
        End Property
        Public Property InflationGuardTypeId As String
            Get
                Return _InflationGuardTypeId
            End Get
            Set(value As String)
                _InflationGuardTypeId = value
            End Set
        End Property
        Public Property BusinessPropertyTypeId As String
            Get
                Return _BusinessPropertyTypeId
            End Get
            Set(value As String)
                _BusinessPropertyTypeId = value
            End Set
        End Property
        Public Property RiskTypeId As String
            Get
                Return _RiskTypeId
            End Get
            Set(value As String)
                _RiskTypeId = value
            End Set
        End Property
        Public Property MonthlyPeriodTypeId As String
            Get
                Return _MonthlyPeriodTypeId
            End Get
            Set(value As String)
                _MonthlyPeriodTypeId = value
            End Set
        End Property
        Public Property BusinessIncomeTypeId As String
            Get
                Return _BusinessIncomeTypeId
            End Get
            Set(value As String)
                _BusinessIncomeTypeId = value
            End Set
        End Property
        Public Property LossCost As String
            Get
                Return _LossCost
            End Get
            Set(value As String)
                _LossCost = value
            End Set
        End Property
        Public Property Rate As String
            Get
                Return _Rate
            End Get
            Set(value As String)
                _Rate = value
            End Set
        End Property
        Public Property WaitingPeriodTypeId As String
            Get
                Return _WaitingPeriodTypeId
            End Get
            Set(value As String)
                _WaitingPeriodTypeId = value
            End Set
        End Property

        Public Property ProductDescription As String
            Get
                Return _ProductDescription
            End Get
            Set(value As String)
                _ProductDescription = value
            End Set
        End Property

        Public Property PersonalPropertyRateGradeTypeId As String
            Get
                Return _PersonalPropertyRateGradeTypeId
            End Get
            Set(value As String)
                _PersonalPropertyRateGradeTypeId = value
            End Set
        End Property

        Public Property ManualElpaRate As String
            Get
                Return _ManualElpaRate
            End Get
            Set(value As String)
                _ManualElpaRate = value
            End Set
        End Property
        Public Property ExcludeProductsCompletedOperations As Boolean
            Get
                Return _ExcludeProductsCompletedOperations
            End Get
            Set(value As Boolean)
                _ExcludeProductsCompletedOperations = value
            End Set
        End Property
        Public Property AggregateLimit As String
            Get
                Return _AggregateLimit
            End Get
            Set(value As String)
                _AggregateLimit = value
            End Set
        End Property

        Public Property DoesYardRateApplyTypeId As String 'added 1/2/2013 for CPR (CoverageDetail:  0=N/A; 1=Yes; 2=No)
            Get
                Return _DoesYardRateApplyTypeId
            End Get
            Set(value As String)
                _DoesYardRateApplyTypeId = value
            End Set
        End Property

        Public Property ConstructionTypeId As String
            Get
                Return _ConstructionTypeId
            End Get
            Set(value As String)
                _ConstructionTypeId = value
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
        Public Property SpecialClassCodeTypeId As String
            Get
                Return _SpecialClassCodeTypeId
            End Get
            Set(value As String)
                _SpecialClassCodeTypeId = value
            End Set
        End Property
        Public Property ProtectionClassId As String
            Get
                Return _ProtectionClassId
            End Get
            Set(value As String)
                _ProtectionClassId = value
            End Set
        End Property

        Public Property IncludeCoverageDetail As QuickQuoteHelperClass.QuickQuoteYesNoMaybeType 'added 2/13/2014
            Get
                Return _IncludeCoverageDetail
            End Get
            Set(value As QuickQuoteHelperClass.QuickQuoteYesNoMaybeType)
                _IncludeCoverageDetail = value
            End Set
        End Property

        Public Property CoverageBasisTypeId As String 'added 2/17/2014
            Get
                Return _CoverageBasisTypeId
            End Get
            Set(value As String)
                _CoverageBasisTypeId = value
            End Set
        End Property

        'added 12/4/2014
        Public Property Calc As String
            Get
                Return _Calc
            End Get
            Set(value As String)
                _Calc = value
            End Set
        End Property

        'added 1/20/2015 (Comm IM / Crime; Farm)
        Public Property ASLId As String
            Get
                Return _ASLId
            End Get
            Set(value As String)
                _ASLId = value
            End Set
        End Property
        Public Property MinimumPremiumAdjustment As String 'CoverageDetail
            Get
                Return _MinimumPremiumAdjustment
            End Get
            Set(value As String)
                _MinimumPremiumAdjustment = value
            End Set
        End Property
        Public Property NumberOfContractors As String 'CoverageDetail
            Get
                Return _NumberOfContractors
            End Get
            Set(value As String)
                _NumberOfContractors = value
            End Set
        End Property
        Public Property NumberOfExcludedEmployees As String 'CoverageDetail
            Get
                Return _NumberOfExcludedEmployees
            End Get
            Set(value As String)
                _NumberOfExcludedEmployees = value
            End Set
        End Property
        Public Property NumberOfIndependentContractors As String 'CoverageDetail
            Get
                Return _NumberOfIndependentContractors
            End Get
            Set(value As String)
                _NumberOfIndependentContractors = value
            End Set
        End Property
        Public Property NumberOfRatableEmployees As String 'CoverageDetail
            Get
                Return _NumberOfRatableEmployees
            End Get
            Set(value As String)
                _NumberOfRatableEmployees = value
            End Set
        End Property
        Public Property NumberOfPremises As String 'CoverageDetail
            Get
                Return _NumberOfPremises
            End Get
            Set(value As String)
                _NumberOfPremises = value
            End Set
        End Property
        Public Property NumberOfTemporaryWorkers As String 'CoverageDetail
            Get
                Return _NumberOfTemporaryWorkers
            End Get
            Set(value As String)
                _NumberOfTemporaryWorkers = value
            End Set
        End Property
        Public Property PercentOfWorkers As String 'CoverageDetail
            Get
                Return _PercentOfWorkers
            End Get
            Set(value As String)
                _PercentOfWorkers = value
            End Set
        End Property
        Public Property ShouldSyncWithMasterCoverage As Boolean 'CoverageDetail
            Get
                Return _ShouldSyncWithMasterCoverage
            End Get
            Set(value As Boolean)
                _ShouldSyncWithMasterCoverage = value
            End Set
        End Property
        Public Property AdditionalInterests As List(Of QuickQuoteAdditionalInterest)
            Get
                SetParentOfListItems(_AdditionalInterests, "{663B7C7B-F2AC-4BF6-965A-D30F41A04194}")
                Return _AdditionalInterests
            End Get
            Set(value As List(Of QuickQuoteAdditionalInterest))
                _AdditionalInterests = value
                SetParentOfListItems(_AdditionalInterests, "{663B7C7B-F2AC-4BF6-965A-D30F41A04194}")
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
        Public Property Make As String 'CoverageDetail
            Get
                Return _Make
            End Get
            Set(value As String)
                _Make = value
            End Set
        End Property
        Public Property ManufacturerName As String 'CoverageDetail
            Get
                Return _ManufacturerName
            End Get
            Set(value As String)
                _ManufacturerName = value
            End Set
        End Property
        Public Property Model As String 'CoverageDetail
            Get
                Return _Model
            End Get
            Set(value As String)
                _Model = value
            End Set
        End Property
        Public Property SerialNumber As String 'CoverageDetail
            Get
                Return _SerialNumber
            End Get
            Set(value As String)
                _SerialNumber = value
            End Set
        End Property
        Public Property VIN As String 'CoverageDetail
            Get
                Return _VIN
            End Get
            Set(value As String)
                _VIN = value
            End Set
        End Property
        Public Property Year As String 'CoverageDetail
            Get
                Return _Year
            End Get
            Set(value As String)
                _Year = value
            End Set
        End Property
        'added 2/9/2015
        Public Property IsNamedPerils As Boolean 'CoverageDetail
            Get
                Return _IsNamedPerils
            End Get
            Set(value As Boolean)
                _IsNamedPerils = value
            End Set
        End Property
        'added 2/10/2015
        Public Property ExcludeEarthquake As Boolean 'CoverageDetail
            Get
                Return _ExcludeEarthquake
            End Get
            Set(value As Boolean)
                _ExcludeEarthquake = value
            End Set
        End Property
        'added 3/3/2015 for Farm
        Public Property DescriptionOfOperations As String 'CoverageDetail
            Get
                Return _DescriptionOfOperations
            End Get
            Set(value As String)
                _DescriptionOfOperations = value
            End Set
        End Property
        Public Property DesignatedJobSite As String 'CoverageDetail
            Get
                Return _DesignatedJobSite
            End Get
            Set(value As String)
                _DesignatedJobSite = value
            End Set
        End Property
        'added 3/17/2015 for CIM
        Public Property HasLoadingUnloading As Boolean 'CoverageDetail
            Get
                Return _HasLoadingUnloading
            End Get
            Set(value As Boolean)
                _HasLoadingUnloading = value
            End Set
        End Property
        Public Property NumberOfVehicles As String 'CoverageDetail
            Get
                Return _NumberOfVehicles
            End Get
            Set(value As String)
                _NumberOfVehicles = value
            End Set
        End Property
        Public Property NumberOfShipments As String 'CoverageDetail
            Get
                Return _NumberOfShipments
            End Get
            Set(value As String)
                _NumberOfShipments = value
            End Set
        End Property
        'added 3/18/2015 for CIM
        Public Property OperatingRadius As String 'CoverageDetail
            Get
                Return _OperatingRadius
            End Get
            Set(value As String)
                _OperatingRadius = value
            End Set
        End Property
        'added 3/23/2015 for CIM
        Public Property City As String 'CoverageDetail
            Get
                Return _City
            End Get
            Set(value As String)
                _City = value
            End Set
        End Property
        Public Property ExcludedEggs As Boolean 'CoverageDetail
            Get
                Return _ExcludedEggs
            End Get
            Set(value As Boolean)
                _ExcludedEggs = value
            End Set
        End Property
        Public Property ExcludedFursOrFurTrimmedGarments As Boolean 'CoverageDetail
            Get
                Return _ExcludedFursOrFurTrimmedGarments
            End Get
            Set(value As Boolean)
                _ExcludedFursOrFurTrimmedGarments = value
            End Set
        End Property
        Public Property ExcludedLiquor As Boolean 'CoverageDetail
            Get
                Return _ExcludedLiquor
            End Get
            Set(value As Boolean)
                _ExcludedLiquor = value
            End Set
        End Property
        Public Property ExcludedLivestockOrPoultry As Boolean 'CoverageDetail
            Get
                Return _ExcludedLivestockOrPoultry
            End Get
            Set(value As Boolean)
                _ExcludedLivestockOrPoultry = value
            End Set
        End Property
        Public Property ExcludedTobacco As Boolean 'CoverageDetail
            Get
                Return _ExcludedTobacco
            End Get
            Set(value As Boolean)
                _ExcludedTobacco = value
            End Set
        End Property
        'Public Property ProductDescription As String 'CoverageDetail; already coded above
        '    Get
        '        Return _ProductDescription
        '    End Get
        '    Set(value As String)
        '        _ProductDescription = value
        '    End Set
        'End Property
        Public Property StateId As String 'CoverageDetail; static data
            Get
                Return _StateId
            End Get
            Set(value As String)
                _StateId = value
            End Set
        End Property
        Public Property WithinMiles As String 'CoverageDetail
            Get
                Return _WithinMiles
            End Get
            Set(value As String)
                _WithinMiles = value
            End Set
        End Property
        'added 3/26/2015 for CIM
        Public Property MaximumDeductible As String 'CoverageDetail
            Get
                Return _MaximumDeductible
            End Get
            Set(value As String)
                _MaximumDeductible = value
            End Set
        End Property
        Public Property MinimumDeductible As String 'CoverageDetail
            Get
                Return _MinimumDeductible
            End Get
            Set(value As String)
                _MinimumDeductible = value
            End Set
        End Property
        Public Property IsIndoor As Boolean 'CoverageDetail
            Get
                Return _IsIndoor
            End Get
            Set(value As Boolean)
                _IsIndoor = value
            End Set
        End Property
        'added 3/30/2015 for CRM
        Public Property ScheduledTextCollection As List(Of QuickQuoteScheduledText)
            Get
                SetParentOfListItems(_ScheduledTextCollection, "{663B7C7B-F2AC-4BF6-965A-D30F41A04195}")
                Return _ScheduledTextCollection
            End Get
            Set(value As List(Of QuickQuoteScheduledText))
                _ScheduledTextCollection = value
                SetParentOfListItems(_ScheduledTextCollection, "{663B7C7B-F2AC-4BF6-965A-D30F41A04195}")
            End Set
        End Property
        Public Property CanUseScheduledTextNumForScheduledTextReconciliation As Boolean
            Get
                Return _CanUseScheduledTextNumForScheduledTextReconciliation
            End Get
            Set(value As Boolean)
                _CanUseScheduledTextNumForScheduledTextReconciliation = value
            End Set
        End Property
        Public Property FaithfulPerformanceOfDutyTypeId As String 'CoverageDetail; static data
            Get
                Return _FaithfulPerformanceOfDutyTypeId
            End Get
            Set(value As String)
                _FaithfulPerformanceOfDutyTypeId = value
            End Set
        End Property
        Public Property NumberOfAdditionalPremises As String 'CoverageDetail
            Get
                Return _NumberOfAdditionalPremises
            End Get
            Set(value As String)
                _NumberOfAdditionalPremises = value
            End Set
        End Property
        Public Property LimitTypeId As String 'CoverageDetail; static data
            Get
                Return _LimitTypeId
            End Get
            Set(value As String)
                _LimitTypeId = value
            End Set
        End Property
        Public Property EmployeeTheftScheduleTypeId As String 'CoverageDetail; static data
            Get
                Return _EmployeeTheftScheduleTypeId
            End Get
            Set(value As String)
                _EmployeeTheftScheduleTypeId = value
            End Set
        End Property
        'added 3/31/2015 for CRM
        Public Property NumberOfCardHolders As String 'CoverageDetail
            Get
                Return _NumberOfCardHolders
            End Get
            Set(value As String)
                _NumberOfCardHolders = value
            End Set
        End Property
        Public Property PremiumChargeTypeId As String 'CoverageDetail; static data
            Get
                Return _PremiumChargeTypeId
            End Get
            Set(value As String)
                _PremiumChargeTypeId = value
            End Set
        End Property
        Public Property OverrideFullyEarned As Boolean
            Get
                Return _OverrideFullyEarned
            End Get
            Set(value As Boolean)
                _OverrideFullyEarned = value
            End Set
        End Property
        Public Property BasisTypeId As String 'CoverageDetail; static data
            Get
                Return _BasisTypeId
            End Get
            Set(value As String)
                _BasisTypeId = value
            End Set
        End Property
        Public Property IncludeSellingPrice As Boolean 'CoverageDetail
            Get
                Return _IncludeSellingPrice
            End Get
            Set(value As Boolean)
                _IncludeSellingPrice = value
            End Set
        End Property
        Public Property IsIncludeGuestsProperty As Boolean 'CoverageDetail
            Get
                Return _IsIncludeGuestsProperty
            End Get
            Set(value As Boolean)
                _IsIncludeGuestsProperty = value
            End Set
        End Property
        Public Property IsLimitToRobberyOnly As Boolean 'CoverageDetail
            Get
                Return _IsLimitToRobberyOnly
            End Get
            Set(value As Boolean)
                _IsLimitToRobberyOnly = value
            End Set
        End Property
        Public Property IsRequireRecordOfChecks As Boolean 'CoverageDetail
            Get
                Return _IsRequireRecordOfChecks
            End Get
            Set(value As Boolean)
                _IsRequireRecordOfChecks = value
            End Set
        End Property
        Public Property FromDate As String 'CoverageDetail; /DateTime
            Get
                Return _FromDate
            End Get
            Set(value As String)
                _FromDate = value
                qqHelper.ConvertToShortDate(_FromDate)
            End Set
        End Property
        Public Property ToDate As String 'CoverageDetail; /DateTime
            Get
                Return _ToDate
            End Get
            Set(value As String)
                _ToDate = value
                qqHelper.ConvertToShortDate(_ToDate)
            End Set
        End Property

        'added 5/11/2015 (specific to peak season coverages; found under scheduled personal properties for Farm); we're currently not setting these Coverage properties but will if flag is True
        Public Property UseEffectiveDateAndExpirationDate As Boolean
            Get
                Return _UseEffectiveDateAndExpirationDate
            End Get
            Set(value As Boolean)
                _UseEffectiveDateAndExpirationDate = value
            End Set
        End Property

        'added 5/12/2015 for Farm (F & G Optional Covs)
        Public Property OriginalCost As String
            Get
                Return _OriginalCost
            End Get
            Set(value As String)
                _OriginalCost = value
            End Set
        End Property

        'added 5/14/2015 for Farm (may need to set these for some of the different covs... testing policy-level now)
        Public Property ApplyToWrittenPremium As Boolean
            Get
                Return _ApplyToWrittenPremium
            End Get
            Set(value As Boolean)
                _ApplyToWrittenPremium = value
            End Set
        End Property
        Public Property Exposure As String
            Get
                Return _Exposure
            End Get
            Set(value As String)
                _Exposure = value
            End Set
        End Property

        'added 6/8/2015 for testing latest CPP enhancements
        Public Property CoverageDescription As String 'CoverageDetail
            Get
                Return _CoverageDescription
            End Get
            Set(value As String)
                _CoverageDescription = value
            End Set
        End Property

        'added 7/15/2015 for missed CIM requirement (on small tools floater)
        Public Property IsEmployeeTools As Boolean 'CoverageDetail
            Get
                Return _IsEmployeeTools
            End Get
            Set(value As Boolean)
                _IsEmployeeTools = value
            End Set
        End Property
        Public Property IsToolsLeasedOrRented As Boolean 'CoverageDetail
            Get
                Return _IsToolsLeasedOrRented
            End Get
            Set(value As Boolean)
                _IsToolsLeasedOrRented = value
            End Set
        End Property

        'added 5/3/2016 for Farm Machinery coverage option (under UnscheduledPersonalProperties)
        Public Property IsLimitedPerilsExtendedCoverage As Boolean 'CoverageDetail
            Get
                Return _IsLimitedPerilsExtendedCoverage
            End Get
            Set(value As Boolean)
                _IsLimitedPerilsExtendedCoverage = value
            End Set
        End Property

        Public Property IsAgreedValue As Boolean 'Bug 4845 Matt A 2-1-2016; 3/9/2017 - included in this library w/ BOP updates
            Get
                Return _IsAgreedValue
            End Get
            Set(value As Boolean)
                _IsAgreedValue = value
            End Set
        End Property
        Public Property Receipts As String '3/9/2017 - BOP stuff
            Get
                Return _Receipts
            End Get
            Set(value As String)
                _Receipts = value
            End Set
        End Property
        Public Property PriorGrossReceipts As String '3/9/2017 - BOP stuff
            Get
                Return _PriorGrossReceipts
            End Get
            Set(value As String)
                _PriorGrossReceipts = value
            End Set
        End Property
        Public Property LiquorLiabilityClassCodeTypeId As String '3/9/2017 - BOP stuff
            Get
                Return _LiquorLiabilityClassCodeTypeId
            End Get
            Set(value As String)
                _LiquorLiabilityClassCodeTypeId = value
            End Set
        End Property

        'added 5/4/2017 for CIM (Golf)
        Public Property CoveredHolesFairways As String 'covDetail; int (actually string)
            Get
                Return _CoveredHolesFairways
            End Get
            Set(value As String)
                _CoveredHolesFairways = value
            End Set
        End Property
        Public Property CoveredHolesGreens As String 'covDetail; int (actually string)
            Get
                Return _CoveredHolesGreens
            End Get
            Set(value As String)
                _CoveredHolesGreens = value
            End Set
        End Property
        Public Property CoveredHolesTees As String 'covDetail; int (actually string)
            Get
                Return _CoveredHolesTees
            End Get
            Set(value As String)
                _CoveredHolesTees = value
            End Set
        End Property
        Public Property CoveredHolesTrees As String 'covDetail; int (actually string)
            Get
                Return _CoveredHolesTrees
            End Get
            Set(value As String)
                _CoveredHolesTrees = value
            End Set
        End Property
        Public Property IsFairways As Boolean 'covDetail
            Get
                Return _IsFairways
            End Get
            Set(value As Boolean)
                _IsFairways = value
            End Set
        End Property
        Public Property IsGreens As Boolean 'covDetail
            Get
                Return _IsGreens
            End Get
            Set(value As Boolean)
                _IsGreens = value
            End Set
        End Property
        Public Property IsTees As Boolean 'covDetail
            Get
                Return _IsTees
            End Get
            Set(value As Boolean)
                _IsTees = value
            End Set
        End Property
        Public Property IsTrees As Boolean 'covDetail
            Get
                Return _IsTrees
            End Get
            Set(value As Boolean)
                _IsTrees = value
            End Set
        End Property
        Public Property NumberOfCarts As String 'covDetail; int
            Get
                Return _NumberOfCarts
            End Get
            Set(value As String)
                _NumberOfCarts = value
            End Set
        End Property

        'added 5/5/2017 for GAR
        Public Property AggregateLiabilityIncrementTypeId As String 'covDetail; int
            Get
                Return _AggregateLiabilityIncrementTypeId
            End Get
            Set(value As String)
                _AggregateLiabilityIncrementTypeId = value
            End Set
        End Property
        Public Property MedicalPaymentsTypeId As String 'covDetail; int
            Get
                Return _MedicalPaymentsTypeId
            End Get
            Set(value As String)
                _MedicalPaymentsTypeId = value
            End Set
        End Property
        'added 5/11/2017 for GAR
        Public Property NumberOfPlates As String 'covDetail; int
            Get
                Return _NumberOfPlates
            End Get
            Set(value As String)
                _NumberOfPlates = value
            End Set
        End Property

        'added 10/30/2017
        Public Property AdjustmentFactor As String 'covDetail; decimal; won't update HasCoverageDetail to use it until code is updating in in QuickQuoteXML routines
            Get
                Return _AdjustmentFactor
            End Get
            Set(value As String)
                _AdjustmentFactor = value
            End Set
        End Property

        Public Property DetailStatusCode As String 'added 3/30/2018
            Get
                Return _DetailStatusCode
            End Get
            Set(value As String)
                _DetailStatusCode = value
            End Set
        End Property

        Protected Friend Property OverwriteExposure As Boolean 'added 4/10/2018 for CPP Contractors and Manufacturers enhancements
            Get
                Return _OverwriteExposure
            End Get
            Set(value As Boolean)
                _OverwriteExposure = value
            End Set
        End Property

        Public Property IsDwellingStructure As Boolean 'added 11/8/2018 for CPR mine subsidence; covDetail
            Get
                Return _IsDwellingStructure
            End Get
            Set(value As Boolean)
                _IsDwellingStructure = value
            End Set
        End Property

        Public Property Payroll As String 'added 8/27/2020 - DJG - for Ohio StopGap; covdetail
            Get
                Return _Payroll
            End Get
            Set(value As String)
                _Payroll = value
            End Set
        End Property

        Public Property OwnerOccupiedPercentageId As String 'added 12/26/2024 -  covdetail
            Get
                Return _OwnerOccupiedPercentageId
            End Get
            Set(value As String)
                _OwnerOccupiedPercentageId = value
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _AnnualPremium = ""
            _EstimatedPremium = ""
            _FullTermPremium = ""
            _PolicyId = ""
            _PolicyImageNum = ""
            _WrittenPremium = ""
            _Checkbox = False
            _Deductible = ""
            _Manual = False
            _ManualLimitAmount = ""
            _ManualLimitIncluded = ""
            _ManualLimitIncreased = ""
            '_ClassificationCode = ""
            _CoverageCode = ""
            _CoverageCodeId = ""
            _CoverageLimitId = ""
            _DeductibleTypeId = ""
            _Description = ""
            _DescriptionInformation = "" '3/9/2017 - BOP stuff
            _EffectiveDate = ""
            _ExpirationDate = ""
            _SubCoverageCodeId = ""

            _NumberOfEmployees = ""
            _NumberOfLocations = ""
            _AutomaticIncreasePercentTypeId = ""
            _ValuationMethodTypeId = ""
            _IsIncludedInBlanketRating = False
            _IsMineSubsidence = False
            _IsCondoCommercialUnitOwnersCoverage = False
            _PropertyClassificationTypeId = ""
            _IsRefrigerationMaintenanceAgreement = False
            _IsBreakdownOrContamination = False
            _IsPowerOutage = False

            _DeductiblePerTypeId = ""

            _NumberOfSwimmingPools = ""
            _NumberOfAmusementAreas = "" '3/9/2017 - BOP stuff

            _DeductibleId = ""

            '_CoverageAdditionalInfoRecords = New Generic.List(Of QuickQuoteCoverageAdditionalInfoRecord)
            _CoverageAdditionalInfoRecords = Nothing 'added 8/4/2014

            '_NumberOfFullTimeEmployees = ""
            _NumberOfPartTimeEmployees = ""

            _ManuallyRated = False
            _ManualPremium = ""
            _NumberOfBurials = ""
            _NumberOfBodies = ""
            _NumberOfPastoralCounselors = ""

            _SalesClubs = ""
            _SalesManufacturers = ""
            _SalesPackageStores = ""
            _SalesRestaurants = ""

            _Comments = ""
            _NameInformation = ""
            _HasWaiverOfSubrogation = False

            _DeductibleCategoryTypeId = ""

            _RetroactiveDate = ""

            _DailyReimbursement = ""
            _NumberOfDays = ""
            _NamedInsuredsBusinessTypeId = ""
            _ExtendNonOwnershipRatingTypeId = ""
            _CoverageTypeId = ""
            _OtherThanCollisionSubTypeId = ""
            _OtherThanCollisionTypeId = ""
            _IfAnyBasis = False

            _AddressInformation = ""

            _CommercialOccupancyTypeId = ""
            _ClassificationCode = New QuickQuoteClassificationCode
            _IsEarthquakeApplies = False
            _CauseOfLossTypeId = ""
            _CoinsuranceTypeId = ""
            _RatingTypeId = ""
            _InflationGuardTypeId = ""
            _BusinessPropertyTypeId = ""
            _RiskTypeId = ""
            _MonthlyPeriodTypeId = ""
            _BusinessIncomeTypeId = ""
            _LossCost = ""
            _Rate = ""
            _WaitingPeriodTypeId = ""

            _ProductDescription = ""

            _PersonalPropertyRateGradeTypeId = ""

            _ManualElpaRate = ""
            _ExcludeProductsCompletedOperations = False
            _AggregateLimit = ""

            _DoesYardRateApplyTypeId = ""

            _ConstructionTypeId = ""
            _FeetToFireHydrant = ""
            _MilesToFireDepartment = ""
            _SpecialClassCodeTypeId = ""
            _ProtectionClassId = ""

            _IncludeCoverageDetail = QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Maybe 'added 2/13/2014

            _CoverageBasisTypeId = ""

            'added 12/4/2014
            _Calc = ""

            'added 1/20/2015 (Comm IM / Crime; Farm)
            _ASLId = ""
            _MinimumPremiumAdjustment = "" 'CoverageDetail
            _NumberOfContractors = "" 'CoverageDetail
            _NumberOfExcludedEmployees = "" 'CoverageDetail
            _NumberOfIndependentContractors = "" 'CoverageDetail
            _NumberOfRatableEmployees = "" 'CoverageDetail
            _NumberOfPremises = "" 'CoverageDetail
            _NumberOfTemporaryWorkers = "" 'CoverageDetail
            _PercentOfWorkers = "" 'CoverageDetail
            _ShouldSyncWithMasterCoverage = False 'CoverageDetail
            '_AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
            _AdditionalInterests = Nothing
            _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = False
            _Make = "" 'CoverageDetail
            _ManufacturerName = "" 'CoverageDetail
            _Model = "" 'CoverageDetail
            _SerialNumber = "" 'CoverageDetail
            _VIN = "" 'CoverageDetail
            _Year = "" 'CoverageDetail
            'added 2/9/2015
            _IsNamedPerils = False 'CoverageDetail
            'added 2/10/2015
            _ExcludeEarthquake = False 'CoverageDetail
            'added 3/3/2015 for Farm
            _DescriptionOfOperations = "" 'CoverageDetail
            _DesignatedJobSite = "" 'CoverageDetail
            'added 3/17/2015 for CIM
            _HasLoadingUnloading = False 'CoverageDetail
            _NumberOfVehicles = "" 'CoverageDetail
            _NumberOfShipments = "" 'CoverageDetail
            'added 3/18/2015 for CIM
            _OperatingRadius = "" 'CoverageDetail
            'added 3/23/2015 for CIM
            _City = "" 'CoverageDetail
            _ExcludedEggs = False 'CoverageDetail
            _ExcludedFursOrFurTrimmedGarments = False 'CoverageDetail
            _ExcludedLiquor = False 'CoverageDetail
            _ExcludedLivestockOrPoultry = False 'CoverageDetail
            _ExcludedTobacco = False 'CoverageDetail
            '_ProductDescription = "" 'CoverageDetail; already coded above
            _StateId = "" 'CoverageDetail; static data
            _WithinMiles = "" 'CoverageDetail
            'added 3/26/2015 for CIM
            _MaximumDeductible = "" 'CoverageDetail
            _MinimumDeductible = "" 'CoverageDetail
            _IsIndoor = False 'CoverageDetail
            'added 3/30/2015 for CRM
            '_ScheduledTextCollection = New List(Of QuickQuoteScheduledText)
            _ScheduledTextCollection = Nothing
            _CanUseScheduledTextNumForScheduledTextReconciliation = False
            _FaithfulPerformanceOfDutyTypeId = "" 'CoverageDetail; static data
            _NumberOfAdditionalPremises = "" 'CoverageDetail
            _LimitTypeId = "" 'CoverageDetail; static data
            _EmployeeTheftScheduleTypeId = "" 'CoverageDetail; static data
            'added 3/31/2015 for CRM
            _NumberOfCardHolders = "" 'CoverageDetail
            _PremiumChargeTypeId = "" 'CoverageDetail; static data
            _OverrideFullyEarned = False
            _BasisTypeId = "" 'CoverageDetail; static data
            _IncludeSellingPrice = False 'CoverageDetail
            _IsIncludeGuestsProperty = False 'CoverageDetail
            _IsLimitToRobberyOnly = False 'CoverageDetail
            _IsRequireRecordOfChecks = False 'CoverageDetail
            _FromDate = "" 'CoverageDetail; /DateTime
            _ToDate = "" 'CoverageDetail; /DateTime

            'added 5/11/2015 (specific to peak season coverages; found under scheduled personal properties for Farm); we're currently not setting these Coverage properties but will if flag is True
            _UseEffectiveDateAndExpirationDate = False

            'added 5/12/2015 for Farm (F & G Optional Covs)
            _OriginalCost = ""

            'added 5/14/2015 for Farm (may need to set these for some of the different covs... testing policy-level now)
            _ApplyToWrittenPremium = False
            _Exposure = ""

            'added 6/8/2015 for testing latest CPP enhancements
            _CoverageDescription = "" 'CoverageDetail

            'added 7/15/2015 for missed CIM requirement (on small tools floater)
            _IsEmployeeTools = False 'CoverageDetail
            _IsToolsLeasedOrRented = False 'CoverageDetail

            'added 5/3/2016 for Farm Machinery coverage option (under UnscheduledPersonalProperties)
            _IsLimitedPerilsExtendedCoverage = False 'CoverageDetail

            '3/9/2017 - BOP stuff
            _IsAgreedValue = False 'CoverageDetail Bug 4845; 3/9/2017 - included in this library w/ BOP updates
            _Receipts = ""
            _PriorGrossReceipts = ""
            _LiquorLiabilityClassCodeTypeId = ""

            'added 5/4/2017 for CIM (Golf)
            _CoveredHolesFairways = "" 'covDetail; int (actually string)
            _CoveredHolesGreens = "" 'covDetail; int (actually string)
            _CoveredHolesTees = "" 'covDetail; int (actually string)
            _CoveredHolesTrees = "" 'covDetail; int (actually string)
            _IsFairways = False 'covDetail
            _IsGreens = False 'covDetail
            _IsTees = False 'covDetail
            _IsTrees = False 'covDetail
            _NumberOfCarts = False 'covDetail; int

            'added 5/5/2017 for GAR
            _AggregateLiabilityIncrementTypeId = "" 'covDetail; int
            _MedicalPaymentsTypeId = "" 'covDetail; int
            'added 5/11/2017 for GAR
            _NumberOfPlates = "" 'covDetail; int

            'added 10/30/2017
            _AdjustmentFactor = "" 'covDetail; decimal; won't update HasCoverageDetail to use it until code is updating in in QuickQuoteXML routines

            _DetailStatusCode = "" 'added 3/30/2018

            _OverwriteExposure = False 'added 4/10/2018 for CPP Contractors and Manufacturers enhancements

            _IsDwellingStructure = False 'added 11/8/2018 for CPR mine subsidence; covDetail
            _OwnerOccupiedPercentageId = "" ' added 12/26/2024 for CPR/CPP covDetail
        End Sub
        'added 2/13/2014
        Public Function HasCoverageDetail() As Boolean
            Dim hasCD As Boolean = False

            If _IncludeCoverageDetail <> Nothing AndAlso (_IncludeCoverageDetail = QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes OrElse _IncludeCoverageDetail = QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.No) Then
                If _IncludeCoverageDetail = QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Yes Then
                    hasCD = True
                Else
                    hasCD = False
                End If
            Else
                'If _AddressInformation <> "" OrElse (_AggregateLimit <> "" AndAlso IsNumeric(_AggregateLimit) = True) OrElse (_AutomaticIncreasePercentTypeId <> "" AndAlso IsNumeric(_AutomaticIncreasePercentTypeId) = True) OrElse (_BusinessIncomeTypeId <> "" AndAlso IsNumeric(_BusinessIncomeTypeId) = True) OrElse (_BusinessPropertyTypeId <> "" AndAlso IsNumeric(_BusinessPropertyTypeId) = True) OrElse (_CauseOfLossTypeId <> "" AndAlso IsNumeric(_CauseOfLossTypeId) = True) OrElse (_CoinsuranceTypeId <> "" AndAlso IsNumeric(_CoinsuranceTypeId) = True) OrElse _Comments <> "" OrElse (_CommercialOccupancyTypeId <> "" AndAlso IsNumeric(_CommercialOccupancyTypeId) = True) OrElse (_ConstructionTypeId <> "" AndAlso IsNumeric(_ConstructionTypeId) = True) OrElse (_CoverageTypeId <> "" AndAlso IsNumeric(_CoverageTypeId) = True) OrElse (_DailyReimbursement <> "" AndAlso IsNumeric(_DailyReimbursement) = True) OrElse (_DeductibleCategoryTypeId <> "" AndAlso IsNumeric(_DeductibleCategoryTypeId) = True) OrElse (_DeductiblePerTypeId <> "" AndAlso IsNumeric(_DeductiblePerTypeId) = True) OrElse (_DoesYardRateApplyTypeId <> "" AndAlso IsNumeric(_DoesYardRateApplyTypeId) = True) OrElse _ExcludeProductsCompletedOperations = True OrElse (_ExtendNonOwnershipRatingTypeId <> "" AndAlso IsNumeric(_ExtendNonOwnershipRatingTypeId) = True) OrElse (_FeetToFireHydrant <> "" AndAlso IsNumeric(_FeetToFireHydrant) = True) OrElse _HasWaiverOfSubrogation = True OrElse _IfAnyBasis = True OrElse (_InflationGuardTypeId <> "" AndAlso IsNumeric(_InflationGuardTypeId) = True) OrElse _IsBreakdownOrContamination = True OrElse _IsCondoCommercialUnitOwnersCoverage = True OrElse _IsEarthquakeApplies = True OrElse _IsIncludedInBlanketRating = True OrElse _IsMineSubsidence = True OrElse _IsPowerOutage = True OrElse _IsRefrigerationMaintenanceAgreement = True OrElse (_LossCost <> "" AndAlso IsNumeric(_LossCost) = True) OrElse (_ManualElpaRate <> "" AndAlso IsNumeric(_ManualElpaRate) = True) OrElse _ManuallyRated = True OrElse (_ManualPremium <> "" AndAlso IsNumeric(_ManualPremium) = True) OrElse (_MilesToFireDepartment <> "" AndAlso IsNumeric(_MilesToFireDepartment) = True) OrElse (_MonthlyPeriodTypeId <> "" AndAlso IsNumeric(_MonthlyPeriodTypeId) = True) OrElse (_NamedInsuredsBusinessTypeId <> "" AndAlso IsNumeric(_NamedInsuredsBusinessTypeId) = True) OrElse _NameInformation <> "" OrElse (_NumberOfBodies <> "" AndAlso IsNumeric(_NumberOfBodies) = True) OrElse (_NumberOfBurials <> "" AndAlso IsNumeric(_NumberOfBurials) = True) OrElse (_NumberOfDays <> "" AndAlso IsNumeric(_NumberOfDays) = True) OrElse (_NumberOfEmployees <> "" AndAlso IsNumeric(_NumberOfEmployees) = True) OrElse (_NumberOfLocations <> "" AndAlso IsNumeric(_NumberOfLocations) = True) OrElse (_NumberOfPartTimeEmployees <> "" AndAlso IsNumeric(_NumberOfPartTimeEmployees) = True) OrElse (_NumberOfPastoralCounselors <> "" AndAlso IsNumeric(_NumberOfPastoralCounselors) = True) OrElse (_NumberOfSwimmingPools <> "" AndAlso IsNumeric(_NumberOfSwimmingPools) = True) OrElse (_OtherThanCollisionSubTypeId <> "" AndAlso IsNumeric(_OtherThanCollisionSubTypeId) = True) OrElse (_OtherThanCollisionTypeId <> "" AndAlso IsNumeric(_OtherThanCollisionTypeId) = True) OrElse (_PersonalPropertyRateGradeTypeId <> "" AndAlso IsNumeric(_PersonalPropertyRateGradeTypeId) = True) OrElse _ProductDescription <> "" OrElse (_PropertyClassificationTypeId <> "" AndAlso IsNumeric(_PropertyClassificationTypeId) = True) OrElse (_ProtectionClassId <> "" AndAlso IsNumeric(_ProtectionClassId) = True) OrElse (_Rate <> "" AndAlso IsNumeric(_Rate) = True) OrElse (_RatingTypeId <> "" AndAlso IsNumeric(_RatingTypeId) = True) OrElse (_RetroactiveDate <> "" AndAlso IsDate(_RetroactiveDate) = True) OrElse (_RiskTypeId <> "" AndAlso IsNumeric(_RiskTypeId) = True) OrElse (_SalesClubs <> "" AndAlso IsNumeric(_SalesClubs) = True) OrElse (_SalesManufacturers <> "" AndAlso IsNumeric(_SalesManufacturers) = True) OrElse (_SalesPackageStores <> "" AndAlso IsNumeric(_SalesPackageStores) = True) OrElse (_SalesRestaurants <> "" AndAlso IsNumeric(_SalesRestaurants) = True) OrElse (_SpecialClassCodeTypeId <> "" AndAlso IsNumeric(_SpecialClassCodeTypeId) = True) OrElse (_ValuationMethodTypeId <> "" AndAlso IsNumeric(_ValuationMethodTypeId) = True) OrElse (_WaitingPeriodTypeId <> "" AndAlso IsNumeric(_WaitingPeriodTypeId) = True) Then
                'updated 2/17/2014 for CoverageBasisTypeId
                'If _AddressInformation <> "" OrElse (_AggregateLimit <> "" AndAlso IsNumeric(_AggregateLimit) = True) OrElse (_AutomaticIncreasePercentTypeId <> "" AndAlso IsNumeric(_AutomaticIncreasePercentTypeId) = True) OrElse (_BusinessIncomeTypeId <> "" AndAlso IsNumeric(_BusinessIncomeTypeId) = True) OrElse (_BusinessPropertyTypeId <> "" AndAlso IsNumeric(_BusinessPropertyTypeId) = True) OrElse (_CauseOfLossTypeId <> "" AndAlso IsNumeric(_CauseOfLossTypeId) = True) OrElse (_CoinsuranceTypeId <> "" AndAlso IsNumeric(_CoinsuranceTypeId) = True) OrElse _Comments <> "" OrElse (_CommercialOccupancyTypeId <> "" AndAlso IsNumeric(_CommercialOccupancyTypeId) = True) OrElse (_ConstructionTypeId <> "" AndAlso IsNumeric(_ConstructionTypeId) = True) OrElse (_CoverageTypeId <> "" AndAlso IsNumeric(_CoverageTypeId) = True) OrElse (_DailyReimbursement <> "" AndAlso IsNumeric(_DailyReimbursement) = True) OrElse (_DeductibleCategoryTypeId <> "" AndAlso IsNumeric(_DeductibleCategoryTypeId) = True) OrElse (_DeductiblePerTypeId <> "" AndAlso IsNumeric(_DeductiblePerTypeId) = True) OrElse (_DoesYardRateApplyTypeId <> "" AndAlso IsNumeric(_DoesYardRateApplyTypeId) = True) OrElse _ExcludeProductsCompletedOperations = True OrElse (_ExtendNonOwnershipRatingTypeId <> "" AndAlso IsNumeric(_ExtendNonOwnershipRatingTypeId) = True) OrElse (_FeetToFireHydrant <> "" AndAlso IsNumeric(_FeetToFireHydrant) = True) OrElse _HasWaiverOfSubrogation = True OrElse _IfAnyBasis = True OrElse (_InflationGuardTypeId <> "" AndAlso IsNumeric(_InflationGuardTypeId) = True) OrElse _IsBreakdownOrContamination = True OrElse _IsCondoCommercialUnitOwnersCoverage = True OrElse _IsEarthquakeApplies = True OrElse _IsIncludedInBlanketRating = True OrElse _IsMineSubsidence = True OrElse _IsPowerOutage = True OrElse _IsRefrigerationMaintenanceAgreement = True OrElse (_LossCost <> "" AndAlso IsNumeric(_LossCost) = True) OrElse (_ManualElpaRate <> "" AndAlso IsNumeric(_ManualElpaRate) = True) OrElse _ManuallyRated = True OrElse (_ManualPremium <> "" AndAlso IsNumeric(_ManualPremium) = True) OrElse (_MilesToFireDepartment <> "" AndAlso IsNumeric(_MilesToFireDepartment) = True) OrElse (_MonthlyPeriodTypeId <> "" AndAlso IsNumeric(_MonthlyPeriodTypeId) = True) OrElse (_NamedInsuredsBusinessTypeId <> "" AndAlso IsNumeric(_NamedInsuredsBusinessTypeId) = True) OrElse _NameInformation <> "" OrElse (_NumberOfBodies <> "" AndAlso IsNumeric(_NumberOfBodies) = True) OrElse (_NumberOfBurials <> "" AndAlso IsNumeric(_NumberOfBurials) = True) OrElse (_NumberOfDays <> "" AndAlso IsNumeric(_NumberOfDays) = True) OrElse (_NumberOfEmployees <> "" AndAlso IsNumeric(_NumberOfEmployees) = True) OrElse (_NumberOfLocations <> "" AndAlso IsNumeric(_NumberOfLocations) = True) OrElse (_NumberOfPartTimeEmployees <> "" AndAlso IsNumeric(_NumberOfPartTimeEmployees) = True) OrElse (_NumberOfPastoralCounselors <> "" AndAlso IsNumeric(_NumberOfPastoralCounselors) = True) OrElse (_NumberOfSwimmingPools <> "" AndAlso IsNumeric(_NumberOfSwimmingPools) = True) OrElse (_OtherThanCollisionSubTypeId <> "" AndAlso IsNumeric(_OtherThanCollisionSubTypeId) = True) OrElse (_OtherThanCollisionTypeId <> "" AndAlso IsNumeric(_OtherThanCollisionTypeId) = True) OrElse (_PersonalPropertyRateGradeTypeId <> "" AndAlso IsNumeric(_PersonalPropertyRateGradeTypeId) = True) OrElse _ProductDescription <> "" OrElse (_PropertyClassificationTypeId <> "" AndAlso IsNumeric(_PropertyClassificationTypeId) = True) OrElse (_ProtectionClassId <> "" AndAlso IsNumeric(_ProtectionClassId) = True) OrElse (_Rate <> "" AndAlso IsNumeric(_Rate) = True) OrElse (_RatingTypeId <> "" AndAlso IsNumeric(_RatingTypeId) = True) OrElse (_RetroactiveDate <> "" AndAlso IsDate(_RetroactiveDate) = True) OrElse (_RiskTypeId <> "" AndAlso IsNumeric(_RiskTypeId) = True) OrElse (_SalesClubs <> "" AndAlso IsNumeric(_SalesClubs) = True) OrElse (_SalesManufacturers <> "" AndAlso IsNumeric(_SalesManufacturers) = True) OrElse (_SalesPackageStores <> "" AndAlso IsNumeric(_SalesPackageStores) = True) OrElse (_SalesRestaurants <> "" AndAlso IsNumeric(_SalesRestaurants) = True) OrElse (_SpecialClassCodeTypeId <> "" AndAlso IsNumeric(_SpecialClassCodeTypeId) = True) OrElse (_ValuationMethodTypeId <> "" AndAlso IsNumeric(_ValuationMethodTypeId) = True) OrElse (_WaitingPeriodTypeId <> "" AndAlso IsNumeric(_WaitingPeriodTypeId) = True) OrElse (_CoverageBasisTypeId <> "" AndAlso IsNumeric(_CoverageBasisTypeId) = True) Then
                'updated 1/21/2015 for new fields; also moved CoverageBasisTypeId part to be in order
                'If _AddressInformation <> "" OrElse (_AggregateLimit <> "" AndAlso IsNumeric(_AggregateLimit) = True) OrElse (_AutomaticIncreasePercentTypeId <> "" AndAlso IsNumeric(_AutomaticIncreasePercentTypeId) = True) OrElse (_BusinessIncomeTypeId <> "" AndAlso IsNumeric(_BusinessIncomeTypeId) = True) OrElse (_BusinessPropertyTypeId <> "" AndAlso IsNumeric(_BusinessPropertyTypeId) = True) OrElse (_CauseOfLossTypeId <> "" AndAlso IsNumeric(_CauseOfLossTypeId) = True) OrElse (_CoinsuranceTypeId <> "" AndAlso IsNumeric(_CoinsuranceTypeId) = True) OrElse _Comments <> "" OrElse (_CommercialOccupancyTypeId <> "" AndAlso IsNumeric(_CommercialOccupancyTypeId) = True) OrElse (_ConstructionTypeId <> "" AndAlso IsNumeric(_ConstructionTypeId) = True) OrElse (_CoverageBasisTypeId <> "" AndAlso IsNumeric(_CoverageBasisTypeId) = True) OrElse (_CoverageTypeId <> "" AndAlso IsNumeric(_CoverageTypeId) = True) OrElse (_DailyReimbursement <> "" AndAlso IsNumeric(_DailyReimbursement) = True) OrElse (_DeductibleCategoryTypeId <> "" AndAlso IsNumeric(_DeductibleCategoryTypeId) = True) OrElse (_DeductiblePerTypeId <> "" AndAlso IsNumeric(_DeductiblePerTypeId) = True) OrElse (_DoesYardRateApplyTypeId <> "" AndAlso IsNumeric(_DoesYardRateApplyTypeId) = True) OrElse _ExcludeProductsCompletedOperations = True OrElse (_ExtendNonOwnershipRatingTypeId <> "" AndAlso IsNumeric(_ExtendNonOwnershipRatingTypeId) = True) OrElse (_FeetToFireHydrant <> "" AndAlso IsNumeric(_FeetToFireHydrant) = True) OrElse _HasWaiverOfSubrogation = True OrElse _IfAnyBasis = True OrElse (_InflationGuardTypeId <> "" AndAlso IsNumeric(_InflationGuardTypeId) = True) OrElse _IsBreakdownOrContamination = True OrElse _IsCondoCommercialUnitOwnersCoverage = True OrElse _IsEarthquakeApplies = True OrElse _IsIncludedInBlanketRating = True OrElse _IsMineSubsidence = True OrElse _IsPowerOutage = True OrElse _IsRefrigerationMaintenanceAgreement = True OrElse (_LossCost <> "" AndAlso IsNumeric(_LossCost) = True) OrElse _Make <> "" OrElse (_ManualElpaRate <> "" AndAlso IsNumeric(_ManualElpaRate) = True) OrElse _ManuallyRated = True OrElse (_ManualPremium <> "" AndAlso IsNumeric(_ManualPremium) = True) OrElse _ManufacturerName <> "" OrElse (_MilesToFireDepartment <> "" AndAlso IsNumeric(_MilesToFireDepartment) = True) OrElse _Model <> "" OrElse (_MonthlyPeriodTypeId <> "" AndAlso IsNumeric(_MonthlyPeriodTypeId) = True) OrElse (_NamedInsuredsBusinessTypeId <> "" AndAlso IsNumeric(_NamedInsuredsBusinessTypeId) = True) OrElse _NameInformation <> "" OrElse (_NumberOfBodies <> "" AndAlso IsNumeric(_NumberOfBodies) = True) OrElse (_NumberOfBurials <> "" AndAlso IsNumeric(_NumberOfBurials) = True) OrElse (_NumberOfContractors <> "" AndAlso IsNumeric(_NumberOfContractors) = True) OrElse (_NumberOfDays <> "" AndAlso IsNumeric(_NumberOfDays) = True) OrElse (_NumberOfEmployees <> "" AndAlso IsNumeric(_NumberOfEmployees) = True) OrElse (_NumberOfExcludedEmployees <> "" AndAlso IsNumeric(_NumberOfExcludedEmployees) = True) OrElse (_NumberOfIndependentContractors <> "" AndAlso IsNumeric(_NumberOfIndependentContractors) = True) OrElse (_NumberOfLocations <> "" AndAlso IsNumeric(_NumberOfLocations) = True) OrElse (_NumberOfPartTimeEmployees <> "" AndAlso IsNumeric(_NumberOfPartTimeEmployees) = True) OrElse (_NumberOfPastoralCounselors <> "" AndAlso IsNumeric(_NumberOfPastoralCounselors) = True) OrElse (_NumberOfPremises <> "" AndAlso IsNumeric(_NumberOfPremises) = True) OrElse (_NumberOfRatableEmployees <> "" AndAlso IsNumeric(_NumberOfRatableEmployees) = True) OrElse (_NumberOfSwimmingPools <> "" AndAlso IsNumeric(_NumberOfSwimmingPools) = True) OrElse (_NumberOfTemporaryWorkers <> "" AndAlso IsNumeric(_NumberOfTemporaryWorkers) = True) OrElse (_OtherThanCollisionSubTypeId <> "" AndAlso IsNumeric(_OtherThanCollisionSubTypeId) = True) OrElse (_OtherThanCollisionTypeId <> "" AndAlso IsNumeric(_OtherThanCollisionTypeId) = True) OrElse (_PercentOfWorkers <> "" AndAlso IsNumeric(_PercentOfWorkers) = True) OrElse (_PersonalPropertyRateGradeTypeId <> "" AndAlso IsNumeric(_PersonalPropertyRateGradeTypeId) = True) OrElse _ProductDescription <> "" OrElse (_PropertyClassificationTypeId <> "" AndAlso IsNumeric(_PropertyClassificationTypeId) = True) OrElse (_ProtectionClassId <> "" AndAlso IsNumeric(_ProtectionClassId) = True) OrElse (_Rate <> "" AndAlso IsNumeric(_Rate) = True) OrElse (_RatingTypeId <> "" AndAlso IsNumeric(_RatingTypeId) = True) OrElse (_RetroactiveDate <> "" AndAlso IsDate(_RetroactiveDate) = True) OrElse (_RiskTypeId <> "" AndAlso IsNumeric(_RiskTypeId) = True) OrElse (_SalesClubs <> "" AndAlso IsNumeric(_SalesClubs) = True) OrElse (_SalesManufacturers <> "" AndAlso IsNumeric(_SalesManufacturers) = True) OrElse (_SalesPackageStores <> "" AndAlso IsNumeric(_SalesPackageStores) = True) OrElse (_SalesRestaurants <> "" AndAlso IsNumeric(_SalesRestaurants) = True) OrElse _SerialNumber <> "" OrElse _ShouldSyncWithMasterCoverage = True OrElse (_SpecialClassCodeTypeId <> "" AndAlso IsNumeric(_SpecialClassCodeTypeId) = True) OrElse _VIN <> "" OrElse (_ValuationMethodTypeId <> "" AndAlso IsNumeric(_ValuationMethodTypeId) = True) OrElse (_WaitingPeriodTypeId <> "" AndAlso IsNumeric(_WaitingPeriodTypeId) = True) OrElse (_Year <> "" AndAlso IsNumeric(_Year) = True) Then 'may not use _ShouldSyncWithMasterCoverage
                'updated 2/9/2015 for IsNamedPerils
                'If _AddressInformation <> "" OrElse (_AggregateLimit <> "" AndAlso IsNumeric(_AggregateLimit) = True) OrElse (_AutomaticIncreasePercentTypeId <> "" AndAlso IsNumeric(_AutomaticIncreasePercentTypeId) = True) OrElse (_BusinessIncomeTypeId <> "" AndAlso IsNumeric(_BusinessIncomeTypeId) = True) OrElse (_BusinessPropertyTypeId <> "" AndAlso IsNumeric(_BusinessPropertyTypeId) = True) OrElse (_CauseOfLossTypeId <> "" AndAlso IsNumeric(_CauseOfLossTypeId) = True) OrElse (_CoinsuranceTypeId <> "" AndAlso IsNumeric(_CoinsuranceTypeId) = True) OrElse _Comments <> "" OrElse (_CommercialOccupancyTypeId <> "" AndAlso IsNumeric(_CommercialOccupancyTypeId) = True) OrElse (_ConstructionTypeId <> "" AndAlso IsNumeric(_ConstructionTypeId) = True) OrElse (_CoverageBasisTypeId <> "" AndAlso IsNumeric(_CoverageBasisTypeId) = True) OrElse (_CoverageTypeId <> "" AndAlso IsNumeric(_CoverageTypeId) = True) OrElse (_DailyReimbursement <> "" AndAlso IsNumeric(_DailyReimbursement) = True) OrElse (_DeductibleCategoryTypeId <> "" AndAlso IsNumeric(_DeductibleCategoryTypeId) = True) OrElse (_DeductiblePerTypeId <> "" AndAlso IsNumeric(_DeductiblePerTypeId) = True) OrElse (_DoesYardRateApplyTypeId <> "" AndAlso IsNumeric(_DoesYardRateApplyTypeId) = True) OrElse _ExcludeProductsCompletedOperations = True OrElse (_ExtendNonOwnershipRatingTypeId <> "" AndAlso IsNumeric(_ExtendNonOwnershipRatingTypeId) = True) OrElse (_FeetToFireHydrant <> "" AndAlso IsNumeric(_FeetToFireHydrant) = True) OrElse _HasWaiverOfSubrogation = True OrElse _IfAnyBasis = True OrElse (_InflationGuardTypeId <> "" AndAlso IsNumeric(_InflationGuardTypeId) = True) OrElse _IsBreakdownOrContamination = True OrElse _IsCondoCommercialUnitOwnersCoverage = True OrElse _IsEarthquakeApplies = True OrElse _IsIncludedInBlanketRating = True OrElse _IsMineSubsidence = True OrElse _IsPowerOutage = True OrElse _IsRefrigerationMaintenanceAgreement = True OrElse (_LossCost <> "" AndAlso IsNumeric(_LossCost) = True) OrElse _Make <> "" OrElse (_ManualElpaRate <> "" AndAlso IsNumeric(_ManualElpaRate) = True) OrElse _ManuallyRated = True OrElse (_ManualPremium <> "" AndAlso IsNumeric(_ManualPremium) = True) OrElse _ManufacturerName <> "" OrElse (_MilesToFireDepartment <> "" AndAlso IsNumeric(_MilesToFireDepartment) = True) OrElse _Model <> "" OrElse (_MonthlyPeriodTypeId <> "" AndAlso IsNumeric(_MonthlyPeriodTypeId) = True) OrElse (_NamedInsuredsBusinessTypeId <> "" AndAlso IsNumeric(_NamedInsuredsBusinessTypeId) = True) OrElse _NameInformation <> "" OrElse (_NumberOfBodies <> "" AndAlso IsNumeric(_NumberOfBodies) = True) OrElse (_NumberOfBurials <> "" AndAlso IsNumeric(_NumberOfBurials) = True) OrElse (_NumberOfContractors <> "" AndAlso IsNumeric(_NumberOfContractors) = True) OrElse (_NumberOfDays <> "" AndAlso IsNumeric(_NumberOfDays) = True) OrElse (_NumberOfEmployees <> "" AndAlso IsNumeric(_NumberOfEmployees) = True) OrElse (_NumberOfExcludedEmployees <> "" AndAlso IsNumeric(_NumberOfExcludedEmployees) = True) OrElse (_NumberOfIndependentContractors <> "" AndAlso IsNumeric(_NumberOfIndependentContractors) = True) OrElse (_NumberOfLocations <> "" AndAlso IsNumeric(_NumberOfLocations) = True) OrElse (_NumberOfPartTimeEmployees <> "" AndAlso IsNumeric(_NumberOfPartTimeEmployees) = True) OrElse (_NumberOfPastoralCounselors <> "" AndAlso IsNumeric(_NumberOfPastoralCounselors) = True) OrElse (_NumberOfPremises <> "" AndAlso IsNumeric(_NumberOfPremises) = True) OrElse (_NumberOfRatableEmployees <> "" AndAlso IsNumeric(_NumberOfRatableEmployees) = True) OrElse (_NumberOfSwimmingPools <> "" AndAlso IsNumeric(_NumberOfSwimmingPools) = True) OrElse (_NumberOfTemporaryWorkers <> "" AndAlso IsNumeric(_NumberOfTemporaryWorkers) = True) OrElse (_OtherThanCollisionSubTypeId <> "" AndAlso IsNumeric(_OtherThanCollisionSubTypeId) = True) OrElse (_OtherThanCollisionTypeId <> "" AndAlso IsNumeric(_OtherThanCollisionTypeId) = True) OrElse (_PercentOfWorkers <> "" AndAlso IsNumeric(_PercentOfWorkers) = True) OrElse (_PersonalPropertyRateGradeTypeId <> "" AndAlso IsNumeric(_PersonalPropertyRateGradeTypeId) = True) OrElse _ProductDescription <> "" OrElse (_PropertyClassificationTypeId <> "" AndAlso IsNumeric(_PropertyClassificationTypeId) = True) OrElse (_ProtectionClassId <> "" AndAlso IsNumeric(_ProtectionClassId) = True) OrElse (_Rate <> "" AndAlso IsNumeric(_Rate) = True) OrElse (_RatingTypeId <> "" AndAlso IsNumeric(_RatingTypeId) = True) OrElse (_RetroactiveDate <> "" AndAlso IsDate(_RetroactiveDate) = True) OrElse (_RiskTypeId <> "" AndAlso IsNumeric(_RiskTypeId) = True) OrElse (_SalesClubs <> "" AndAlso IsNumeric(_SalesClubs) = True) OrElse (_SalesManufacturers <> "" AndAlso IsNumeric(_SalesManufacturers) = True) OrElse (_SalesPackageStores <> "" AndAlso IsNumeric(_SalesPackageStores) = True) OrElse (_SalesRestaurants <> "" AndAlso IsNumeric(_SalesRestaurants) = True) OrElse _SerialNumber <> "" OrElse _ShouldSyncWithMasterCoverage = True OrElse (_SpecialClassCodeTypeId <> "" AndAlso IsNumeric(_SpecialClassCodeTypeId) = True) OrElse _VIN <> "" OrElse (_ValuationMethodTypeId <> "" AndAlso IsNumeric(_ValuationMethodTypeId) = True) OrElse (_WaitingPeriodTypeId <> "" AndAlso IsNumeric(_WaitingPeriodTypeId) = True) OrElse (_Year <> "" AndAlso IsNumeric(_Year) = True) OrElse _IsNamedPerils = True Then 'may not use _ShouldSyncWithMasterCoverage
                'updated 2/10/2015 for ExcludeEarthquake
                'If _AddressInformation <> "" OrElse (_AggregateLimit <> "" AndAlso IsNumeric(_AggregateLimit) = True) OrElse (_AutomaticIncreasePercentTypeId <> "" AndAlso IsNumeric(_AutomaticIncreasePercentTypeId) = True) OrElse (_BusinessIncomeTypeId <> "" AndAlso IsNumeric(_BusinessIncomeTypeId) = True) OrElse (_BusinessPropertyTypeId <> "" AndAlso IsNumeric(_BusinessPropertyTypeId) = True) OrElse (_CauseOfLossTypeId <> "" AndAlso IsNumeric(_CauseOfLossTypeId) = True) OrElse (_CoinsuranceTypeId <> "" AndAlso IsNumeric(_CoinsuranceTypeId) = True) OrElse _Comments <> "" OrElse (_CommercialOccupancyTypeId <> "" AndAlso IsNumeric(_CommercialOccupancyTypeId) = True) OrElse (_ConstructionTypeId <> "" AndAlso IsNumeric(_ConstructionTypeId) = True) OrElse (_CoverageBasisTypeId <> "" AndAlso IsNumeric(_CoverageBasisTypeId) = True) OrElse (_CoverageTypeId <> "" AndAlso IsNumeric(_CoverageTypeId) = True) OrElse (_DailyReimbursement <> "" AndAlso IsNumeric(_DailyReimbursement) = True) OrElse (_DeductibleCategoryTypeId <> "" AndAlso IsNumeric(_DeductibleCategoryTypeId) = True) OrElse (_DeductiblePerTypeId <> "" AndAlso IsNumeric(_DeductiblePerTypeId) = True) OrElse (_DoesYardRateApplyTypeId <> "" AndAlso IsNumeric(_DoesYardRateApplyTypeId) = True) OrElse _ExcludeProductsCompletedOperations = True OrElse (_ExtendNonOwnershipRatingTypeId <> "" AndAlso IsNumeric(_ExtendNonOwnershipRatingTypeId) = True) OrElse (_FeetToFireHydrant <> "" AndAlso IsNumeric(_FeetToFireHydrant) = True) OrElse _HasWaiverOfSubrogation = True OrElse _IfAnyBasis = True OrElse (_InflationGuardTypeId <> "" AndAlso IsNumeric(_InflationGuardTypeId) = True) OrElse _IsBreakdownOrContamination = True OrElse _IsCondoCommercialUnitOwnersCoverage = True OrElse _IsEarthquakeApplies = True OrElse _IsIncludedInBlanketRating = True OrElse _IsMineSubsidence = True OrElse _IsPowerOutage = True OrElse _IsRefrigerationMaintenanceAgreement = True OrElse (_LossCost <> "" AndAlso IsNumeric(_LossCost) = True) OrElse _Make <> "" OrElse (_ManualElpaRate <> "" AndAlso IsNumeric(_ManualElpaRate) = True) OrElse _ManuallyRated = True OrElse (_ManualPremium <> "" AndAlso IsNumeric(_ManualPremium) = True) OrElse _ManufacturerName <> "" OrElse (_MilesToFireDepartment <> "" AndAlso IsNumeric(_MilesToFireDepartment) = True) OrElse _Model <> "" OrElse (_MonthlyPeriodTypeId <> "" AndAlso IsNumeric(_MonthlyPeriodTypeId) = True) OrElse (_NamedInsuredsBusinessTypeId <> "" AndAlso IsNumeric(_NamedInsuredsBusinessTypeId) = True) OrElse _NameInformation <> "" OrElse (_NumberOfBodies <> "" AndAlso IsNumeric(_NumberOfBodies) = True) OrElse (_NumberOfBurials <> "" AndAlso IsNumeric(_NumberOfBurials) = True) OrElse (_NumberOfContractors <> "" AndAlso IsNumeric(_NumberOfContractors) = True) OrElse (_NumberOfDays <> "" AndAlso IsNumeric(_NumberOfDays) = True) OrElse (_NumberOfEmployees <> "" AndAlso IsNumeric(_NumberOfEmployees) = True) OrElse (_NumberOfExcludedEmployees <> "" AndAlso IsNumeric(_NumberOfExcludedEmployees) = True) OrElse (_NumberOfIndependentContractors <> "" AndAlso IsNumeric(_NumberOfIndependentContractors) = True) OrElse (_NumberOfLocations <> "" AndAlso IsNumeric(_NumberOfLocations) = True) OrElse (_NumberOfPartTimeEmployees <> "" AndAlso IsNumeric(_NumberOfPartTimeEmployees) = True) OrElse (_NumberOfPastoralCounselors <> "" AndAlso IsNumeric(_NumberOfPastoralCounselors) = True) OrElse (_NumberOfPremises <> "" AndAlso IsNumeric(_NumberOfPremises) = True) OrElse (_NumberOfRatableEmployees <> "" AndAlso IsNumeric(_NumberOfRatableEmployees) = True) OrElse (_NumberOfSwimmingPools <> "" AndAlso IsNumeric(_NumberOfSwimmingPools) = True) OrElse (_NumberOfTemporaryWorkers <> "" AndAlso IsNumeric(_NumberOfTemporaryWorkers) = True) OrElse (_OtherThanCollisionSubTypeId <> "" AndAlso IsNumeric(_OtherThanCollisionSubTypeId) = True) OrElse (_OtherThanCollisionTypeId <> "" AndAlso IsNumeric(_OtherThanCollisionTypeId) = True) OrElse (_PercentOfWorkers <> "" AndAlso IsNumeric(_PercentOfWorkers) = True) OrElse (_PersonalPropertyRateGradeTypeId <> "" AndAlso IsNumeric(_PersonalPropertyRateGradeTypeId) = True) OrElse _ProductDescription <> "" OrElse (_PropertyClassificationTypeId <> "" AndAlso IsNumeric(_PropertyClassificationTypeId) = True) OrElse (_ProtectionClassId <> "" AndAlso IsNumeric(_ProtectionClassId) = True) OrElse (_Rate <> "" AndAlso IsNumeric(_Rate) = True) OrElse (_RatingTypeId <> "" AndAlso IsNumeric(_RatingTypeId) = True) OrElse (_RetroactiveDate <> "" AndAlso IsDate(_RetroactiveDate) = True) OrElse (_RiskTypeId <> "" AndAlso IsNumeric(_RiskTypeId) = True) OrElse (_SalesClubs <> "" AndAlso IsNumeric(_SalesClubs) = True) OrElse (_SalesManufacturers <> "" AndAlso IsNumeric(_SalesManufacturers) = True) OrElse (_SalesPackageStores <> "" AndAlso IsNumeric(_SalesPackageStores) = True) OrElse (_SalesRestaurants <> "" AndAlso IsNumeric(_SalesRestaurants) = True) OrElse _SerialNumber <> "" OrElse _ShouldSyncWithMasterCoverage = True OrElse (_SpecialClassCodeTypeId <> "" AndAlso IsNumeric(_SpecialClassCodeTypeId) = True) OrElse _VIN <> "" OrElse (_ValuationMethodTypeId <> "" AndAlso IsNumeric(_ValuationMethodTypeId) = True) OrElse (_WaitingPeriodTypeId <> "" AndAlso IsNumeric(_WaitingPeriodTypeId) = True) OrElse (_Year <> "" AndAlso IsNumeric(_Year) = True) OrElse _IsNamedPerils = True OrElse _ExcludeEarthquake = True Then 'may not use _ShouldSyncWithMasterCoverage
                'updated 3/3/2015 for DescriptionOfOperations and DesignatedJobSite
                'If _AddressInformation <> "" OrElse (_AggregateLimit <> "" AndAlso IsNumeric(_AggregateLimit) = True) OrElse (_AutomaticIncreasePercentTypeId <> "" AndAlso IsNumeric(_AutomaticIncreasePercentTypeId) = True) OrElse (_BusinessIncomeTypeId <> "" AndAlso IsNumeric(_BusinessIncomeTypeId) = True) OrElse (_BusinessPropertyTypeId <> "" AndAlso IsNumeric(_BusinessPropertyTypeId) = True) OrElse (_CauseOfLossTypeId <> "" AndAlso IsNumeric(_CauseOfLossTypeId) = True) OrElse (_CoinsuranceTypeId <> "" AndAlso IsNumeric(_CoinsuranceTypeId) = True) OrElse _Comments <> "" OrElse (_CommercialOccupancyTypeId <> "" AndAlso IsNumeric(_CommercialOccupancyTypeId) = True) OrElse (_ConstructionTypeId <> "" AndAlso IsNumeric(_ConstructionTypeId) = True) OrElse (_CoverageBasisTypeId <> "" AndAlso IsNumeric(_CoverageBasisTypeId) = True) OrElse (_CoverageTypeId <> "" AndAlso IsNumeric(_CoverageTypeId) = True) OrElse (_DailyReimbursement <> "" AndAlso IsNumeric(_DailyReimbursement) = True) OrElse (_DeductibleCategoryTypeId <> "" AndAlso IsNumeric(_DeductibleCategoryTypeId) = True) OrElse (_DeductiblePerTypeId <> "" AndAlso IsNumeric(_DeductiblePerTypeId) = True) OrElse (_DoesYardRateApplyTypeId <> "" AndAlso IsNumeric(_DoesYardRateApplyTypeId) = True) OrElse _ExcludeProductsCompletedOperations = True OrElse (_ExtendNonOwnershipRatingTypeId <> "" AndAlso IsNumeric(_ExtendNonOwnershipRatingTypeId) = True) OrElse (_FeetToFireHydrant <> "" AndAlso IsNumeric(_FeetToFireHydrant) = True) OrElse _HasWaiverOfSubrogation = True OrElse _IfAnyBasis = True OrElse (_InflationGuardTypeId <> "" AndAlso IsNumeric(_InflationGuardTypeId) = True) OrElse _IsBreakdownOrContamination = True OrElse _IsCondoCommercialUnitOwnersCoverage = True OrElse _IsEarthquakeApplies = True OrElse _IsIncludedInBlanketRating = True OrElse _IsMineSubsidence = True OrElse _IsPowerOutage = True OrElse _IsRefrigerationMaintenanceAgreement = True OrElse (_LossCost <> "" AndAlso IsNumeric(_LossCost) = True) OrElse _Make <> "" OrElse (_ManualElpaRate <> "" AndAlso IsNumeric(_ManualElpaRate) = True) OrElse _ManuallyRated = True OrElse (_ManualPremium <> "" AndAlso IsNumeric(_ManualPremium) = True) OrElse _ManufacturerName <> "" OrElse (_MilesToFireDepartment <> "" AndAlso IsNumeric(_MilesToFireDepartment) = True) OrElse _Model <> "" OrElse (_MonthlyPeriodTypeId <> "" AndAlso IsNumeric(_MonthlyPeriodTypeId) = True) OrElse (_NamedInsuredsBusinessTypeId <> "" AndAlso IsNumeric(_NamedInsuredsBusinessTypeId) = True) OrElse _NameInformation <> "" OrElse (_NumberOfBodies <> "" AndAlso IsNumeric(_NumberOfBodies) = True) OrElse (_NumberOfBurials <> "" AndAlso IsNumeric(_NumberOfBurials) = True) OrElse (_NumberOfContractors <> "" AndAlso IsNumeric(_NumberOfContractors) = True) OrElse (_NumberOfDays <> "" AndAlso IsNumeric(_NumberOfDays) = True) OrElse (_NumberOfEmployees <> "" AndAlso IsNumeric(_NumberOfEmployees) = True) OrElse (_NumberOfExcludedEmployees <> "" AndAlso IsNumeric(_NumberOfExcludedEmployees) = True) OrElse (_NumberOfIndependentContractors <> "" AndAlso IsNumeric(_NumberOfIndependentContractors) = True) OrElse (_NumberOfLocations <> "" AndAlso IsNumeric(_NumberOfLocations) = True) OrElse (_NumberOfPartTimeEmployees <> "" AndAlso IsNumeric(_NumberOfPartTimeEmployees) = True) OrElse (_NumberOfPastoralCounselors <> "" AndAlso IsNumeric(_NumberOfPastoralCounselors) = True) OrElse (_NumberOfPremises <> "" AndAlso IsNumeric(_NumberOfPremises) = True) OrElse (_NumberOfRatableEmployees <> "" AndAlso IsNumeric(_NumberOfRatableEmployees) = True) OrElse (_NumberOfSwimmingPools <> "" AndAlso IsNumeric(_NumberOfSwimmingPools) = True) OrElse (_NumberOfTemporaryWorkers <> "" AndAlso IsNumeric(_NumberOfTemporaryWorkers) = True) OrElse (_OtherThanCollisionSubTypeId <> "" AndAlso IsNumeric(_OtherThanCollisionSubTypeId) = True) OrElse (_OtherThanCollisionTypeId <> "" AndAlso IsNumeric(_OtherThanCollisionTypeId) = True) OrElse (_PercentOfWorkers <> "" AndAlso IsNumeric(_PercentOfWorkers) = True) OrElse (_PersonalPropertyRateGradeTypeId <> "" AndAlso IsNumeric(_PersonalPropertyRateGradeTypeId) = True) OrElse _ProductDescription <> "" OrElse (_PropertyClassificationTypeId <> "" AndAlso IsNumeric(_PropertyClassificationTypeId) = True) OrElse (_ProtectionClassId <> "" AndAlso IsNumeric(_ProtectionClassId) = True) OrElse (_Rate <> "" AndAlso IsNumeric(_Rate) = True) OrElse (_RatingTypeId <> "" AndAlso IsNumeric(_RatingTypeId) = True) OrElse (_RetroactiveDate <> "" AndAlso IsDate(_RetroactiveDate) = True) OrElse (_RiskTypeId <> "" AndAlso IsNumeric(_RiskTypeId) = True) OrElse (_SalesClubs <> "" AndAlso IsNumeric(_SalesClubs) = True) OrElse (_SalesManufacturers <> "" AndAlso IsNumeric(_SalesManufacturers) = True) OrElse (_SalesPackageStores <> "" AndAlso IsNumeric(_SalesPackageStores) = True) OrElse (_SalesRestaurants <> "" AndAlso IsNumeric(_SalesRestaurants) = True) OrElse _SerialNumber <> "" OrElse _ShouldSyncWithMasterCoverage = True OrElse (_SpecialClassCodeTypeId <> "" AndAlso IsNumeric(_SpecialClassCodeTypeId) = True) OrElse _VIN <> "" OrElse (_ValuationMethodTypeId <> "" AndAlso IsNumeric(_ValuationMethodTypeId) = True) OrElse (_WaitingPeriodTypeId <> "" AndAlso IsNumeric(_WaitingPeriodTypeId) = True) OrElse (_Year <> "" AndAlso IsNumeric(_Year) = True) OrElse _IsNamedPerils = True OrElse _ExcludeEarthquake = True OrElse _DescriptionOfOperations <> "" OrElse _DesignatedJobSite <> "" Then 'may not use _ShouldSyncWithMasterCoverage
                'updated 3/17/2015 for HasLoadingUnloading, NumberOfVehicles, and NumberOfShipments; 3/18/2015 for OperatingRadius (note: all of the number fields are currently just looking for numeric whether it's a diamond id or a count of some kind... count variables may need to be updated to look for numeric > 0)
                'If _AddressInformation <> "" OrElse (_AggregateLimit <> "" AndAlso IsNumeric(_AggregateLimit) = True) OrElse (_AutomaticIncreasePercentTypeId <> "" AndAlso IsNumeric(_AutomaticIncreasePercentTypeId) = True) OrElse (_BusinessIncomeTypeId <> "" AndAlso IsNumeric(_BusinessIncomeTypeId) = True) OrElse (_BusinessPropertyTypeId <> "" AndAlso IsNumeric(_BusinessPropertyTypeId) = True) OrElse (_CauseOfLossTypeId <> "" AndAlso IsNumeric(_CauseOfLossTypeId) = True) OrElse (_CoinsuranceTypeId <> "" AndAlso IsNumeric(_CoinsuranceTypeId) = True) OrElse _Comments <> "" OrElse (_CommercialOccupancyTypeId <> "" AndAlso IsNumeric(_CommercialOccupancyTypeId) = True) OrElse (_ConstructionTypeId <> "" AndAlso IsNumeric(_ConstructionTypeId) = True) OrElse (_CoverageBasisTypeId <> "" AndAlso IsNumeric(_CoverageBasisTypeId) = True) OrElse (_CoverageTypeId <> "" AndAlso IsNumeric(_CoverageTypeId) = True) OrElse (_DailyReimbursement <> "" AndAlso IsNumeric(_DailyReimbursement) = True) OrElse (_DeductibleCategoryTypeId <> "" AndAlso IsNumeric(_DeductibleCategoryTypeId) = True) OrElse (_DeductiblePerTypeId <> "" AndAlso IsNumeric(_DeductiblePerTypeId) = True) OrElse (_DoesYardRateApplyTypeId <> "" AndAlso IsNumeric(_DoesYardRateApplyTypeId) = True) OrElse _ExcludeProductsCompletedOperations = True OrElse (_ExtendNonOwnershipRatingTypeId <> "" AndAlso IsNumeric(_ExtendNonOwnershipRatingTypeId) = True) OrElse (_FeetToFireHydrant <> "" AndAlso IsNumeric(_FeetToFireHydrant) = True) OrElse _HasWaiverOfSubrogation = True OrElse _IfAnyBasis = True OrElse (_InflationGuardTypeId <> "" AndAlso IsNumeric(_InflationGuardTypeId) = True) OrElse _IsBreakdownOrContamination = True OrElse _IsCondoCommercialUnitOwnersCoverage = True OrElse _IsEarthquakeApplies = True OrElse _IsIncludedInBlanketRating = True OrElse _IsMineSubsidence = True OrElse _IsPowerOutage = True OrElse _IsRefrigerationMaintenanceAgreement = True OrElse (_LossCost <> "" AndAlso IsNumeric(_LossCost) = True) OrElse _Make <> "" OrElse (_ManualElpaRate <> "" AndAlso IsNumeric(_ManualElpaRate) = True) OrElse _ManuallyRated = True OrElse (_ManualPremium <> "" AndAlso IsNumeric(_ManualPremium) = True) OrElse _ManufacturerName <> "" OrElse (_MilesToFireDepartment <> "" AndAlso IsNumeric(_MilesToFireDepartment) = True) OrElse _Model <> "" OrElse (_MonthlyPeriodTypeId <> "" AndAlso IsNumeric(_MonthlyPeriodTypeId) = True) OrElse (_NamedInsuredsBusinessTypeId <> "" AndAlso IsNumeric(_NamedInsuredsBusinessTypeId) = True) OrElse _NameInformation <> "" OrElse (_NumberOfBodies <> "" AndAlso IsNumeric(_NumberOfBodies) = True) OrElse (_NumberOfBurials <> "" AndAlso IsNumeric(_NumberOfBurials) = True) OrElse (_NumberOfContractors <> "" AndAlso IsNumeric(_NumberOfContractors) = True) OrElse (_NumberOfDays <> "" AndAlso IsNumeric(_NumberOfDays) = True) OrElse (_NumberOfEmployees <> "" AndAlso IsNumeric(_NumberOfEmployees) = True) OrElse (_NumberOfExcludedEmployees <> "" AndAlso IsNumeric(_NumberOfExcludedEmployees) = True) OrElse (_NumberOfIndependentContractors <> "" AndAlso IsNumeric(_NumberOfIndependentContractors) = True) OrElse (_NumberOfLocations <> "" AndAlso IsNumeric(_NumberOfLocations) = True) OrElse (_NumberOfPartTimeEmployees <> "" AndAlso IsNumeric(_NumberOfPartTimeEmployees) = True) OrElse (_NumberOfPastoralCounselors <> "" AndAlso IsNumeric(_NumberOfPastoralCounselors) = True) OrElse (_NumberOfPremises <> "" AndAlso IsNumeric(_NumberOfPremises) = True) OrElse (_NumberOfRatableEmployees <> "" AndAlso IsNumeric(_NumberOfRatableEmployees) = True) OrElse (_NumberOfSwimmingPools <> "" AndAlso IsNumeric(_NumberOfSwimmingPools) = True) OrElse (_NumberOfTemporaryWorkers <> "" AndAlso IsNumeric(_NumberOfTemporaryWorkers) = True) OrElse (_OtherThanCollisionSubTypeId <> "" AndAlso IsNumeric(_OtherThanCollisionSubTypeId) = True) OrElse (_OtherThanCollisionTypeId <> "" AndAlso IsNumeric(_OtherThanCollisionTypeId) = True) OrElse (_PercentOfWorkers <> "" AndAlso IsNumeric(_PercentOfWorkers) = True) OrElse (_PersonalPropertyRateGradeTypeId <> "" AndAlso IsNumeric(_PersonalPropertyRateGradeTypeId) = True) OrElse _ProductDescription <> "" OrElse (_PropertyClassificationTypeId <> "" AndAlso IsNumeric(_PropertyClassificationTypeId) = True) OrElse (_ProtectionClassId <> "" AndAlso IsNumeric(_ProtectionClassId) = True) OrElse (_Rate <> "" AndAlso IsNumeric(_Rate) = True) OrElse (_RatingTypeId <> "" AndAlso IsNumeric(_RatingTypeId) = True) OrElse (_RetroactiveDate <> "" AndAlso IsDate(_RetroactiveDate) = True) OrElse (_RiskTypeId <> "" AndAlso IsNumeric(_RiskTypeId) = True) OrElse (_SalesClubs <> "" AndAlso IsNumeric(_SalesClubs) = True) OrElse (_SalesManufacturers <> "" AndAlso IsNumeric(_SalesManufacturers) = True) OrElse (_SalesPackageStores <> "" AndAlso IsNumeric(_SalesPackageStores) = True) OrElse (_SalesRestaurants <> "" AndAlso IsNumeric(_SalesRestaurants) = True) OrElse _SerialNumber <> "" OrElse _ShouldSyncWithMasterCoverage = True OrElse (_SpecialClassCodeTypeId <> "" AndAlso IsNumeric(_SpecialClassCodeTypeId) = True) OrElse _VIN <> "" OrElse (_ValuationMethodTypeId <> "" AndAlso IsNumeric(_ValuationMethodTypeId) = True) OrElse (_WaitingPeriodTypeId <> "" AndAlso IsNumeric(_WaitingPeriodTypeId) = True) OrElse (_Year <> "" AndAlso IsNumeric(_Year) = True) OrElse _IsNamedPerils = True OrElse _ExcludeEarthquake = True OrElse _DescriptionOfOperations <> "" OrElse _DesignatedJobSite <> "" OrElse _HasLoadingUnloading = True OrElse qqHelper.IsNumericString(_NumberOfVehicles) = True OrElse qqHelper.IsNumericString(_NumberOfShipments) = True OrElse qqHelper.IsNumericString(_OperatingRadius) = True Then 'may not use _ShouldSyncWithMasterCoverage
                'updated 3/23/2015 for new fields; note: new int fields currently just looking for numeric, but may need to check for > 0 (_StateId, _WithinMiles); also removed extra _ProductDescription; updated 3/26/2015 for Max and Min Deductible fields... may need to check for zeroPremium false instead of just numeric; also IsIndoor 3/26/2015
                'If _AddressInformation <> "" OrElse (_AggregateLimit <> "" AndAlso IsNumeric(_AggregateLimit) = True) OrElse (_AutomaticIncreasePercentTypeId <> "" AndAlso IsNumeric(_AutomaticIncreasePercentTypeId) = True) OrElse (_BusinessIncomeTypeId <> "" AndAlso IsNumeric(_BusinessIncomeTypeId) = True) OrElse (_BusinessPropertyTypeId <> "" AndAlso IsNumeric(_BusinessPropertyTypeId) = True) OrElse (_CauseOfLossTypeId <> "" AndAlso IsNumeric(_CauseOfLossTypeId) = True) OrElse (_CoinsuranceTypeId <> "" AndAlso IsNumeric(_CoinsuranceTypeId) = True) OrElse _Comments <> "" OrElse (_CommercialOccupancyTypeId <> "" AndAlso IsNumeric(_CommercialOccupancyTypeId) = True) OrElse (_ConstructionTypeId <> "" AndAlso IsNumeric(_ConstructionTypeId) = True) OrElse (_CoverageBasisTypeId <> "" AndAlso IsNumeric(_CoverageBasisTypeId) = True) OrElse (_CoverageTypeId <> "" AndAlso IsNumeric(_CoverageTypeId) = True) OrElse (_DailyReimbursement <> "" AndAlso IsNumeric(_DailyReimbursement) = True) OrElse (_DeductibleCategoryTypeId <> "" AndAlso IsNumeric(_DeductibleCategoryTypeId) = True) OrElse (_DeductiblePerTypeId <> "" AndAlso IsNumeric(_DeductiblePerTypeId) = True) OrElse (_DoesYardRateApplyTypeId <> "" AndAlso IsNumeric(_DoesYardRateApplyTypeId) = True) OrElse _ExcludeProductsCompletedOperations = True OrElse (_ExtendNonOwnershipRatingTypeId <> "" AndAlso IsNumeric(_ExtendNonOwnershipRatingTypeId) = True) OrElse (_FeetToFireHydrant <> "" AndAlso IsNumeric(_FeetToFireHydrant) = True) OrElse _HasWaiverOfSubrogation = True OrElse _IfAnyBasis = True OrElse (_InflationGuardTypeId <> "" AndAlso IsNumeric(_InflationGuardTypeId) = True) OrElse _IsBreakdownOrContamination = True OrElse _IsCondoCommercialUnitOwnersCoverage = True OrElse _IsEarthquakeApplies = True OrElse _IsIncludedInBlanketRating = True OrElse _IsMineSubsidence = True OrElse _IsPowerOutage = True OrElse _IsRefrigerationMaintenanceAgreement = True OrElse (_LossCost <> "" AndAlso IsNumeric(_LossCost) = True) OrElse _Make <> "" OrElse (_ManualElpaRate <> "" AndAlso IsNumeric(_ManualElpaRate) = True) OrElse _ManuallyRated = True OrElse (_ManualPremium <> "" AndAlso IsNumeric(_ManualPremium) = True) OrElse _ManufacturerName <> "" OrElse (_MilesToFireDepartment <> "" AndAlso IsNumeric(_MilesToFireDepartment) = True) OrElse _Model <> "" OrElse (_MonthlyPeriodTypeId <> "" AndAlso IsNumeric(_MonthlyPeriodTypeId) = True) OrElse (_NamedInsuredsBusinessTypeId <> "" AndAlso IsNumeric(_NamedInsuredsBusinessTypeId) = True) OrElse _NameInformation <> "" OrElse (_NumberOfBodies <> "" AndAlso IsNumeric(_NumberOfBodies) = True) OrElse (_NumberOfBurials <> "" AndAlso IsNumeric(_NumberOfBurials) = True) OrElse (_NumberOfContractors <> "" AndAlso IsNumeric(_NumberOfContractors) = True) OrElse (_NumberOfDays <> "" AndAlso IsNumeric(_NumberOfDays) = True) OrElse (_NumberOfEmployees <> "" AndAlso IsNumeric(_NumberOfEmployees) = True) OrElse (_NumberOfExcludedEmployees <> "" AndAlso IsNumeric(_NumberOfExcludedEmployees) = True) OrElse (_NumberOfIndependentContractors <> "" AndAlso IsNumeric(_NumberOfIndependentContractors) = True) OrElse (_NumberOfLocations <> "" AndAlso IsNumeric(_NumberOfLocations) = True) OrElse (_NumberOfPartTimeEmployees <> "" AndAlso IsNumeric(_NumberOfPartTimeEmployees) = True) OrElse (_NumberOfPastoralCounselors <> "" AndAlso IsNumeric(_NumberOfPastoralCounselors) = True) OrElse (_NumberOfPremises <> "" AndAlso IsNumeric(_NumberOfPremises) = True) OrElse (_NumberOfRatableEmployees <> "" AndAlso IsNumeric(_NumberOfRatableEmployees) = True) OrElse (_NumberOfSwimmingPools <> "" AndAlso IsNumeric(_NumberOfSwimmingPools) = True) OrElse (_NumberOfTemporaryWorkers <> "" AndAlso IsNumeric(_NumberOfTemporaryWorkers) = True) OrElse (_OtherThanCollisionSubTypeId <> "" AndAlso IsNumeric(_OtherThanCollisionSubTypeId) = True) OrElse (_OtherThanCollisionTypeId <> "" AndAlso IsNumeric(_OtherThanCollisionTypeId) = True) OrElse (_PercentOfWorkers <> "" AndAlso IsNumeric(_PercentOfWorkers) = True) OrElse (_PersonalPropertyRateGradeTypeId <> "" AndAlso IsNumeric(_PersonalPropertyRateGradeTypeId) = True) OrElse _ProductDescription <> "" OrElse (_PropertyClassificationTypeId <> "" AndAlso IsNumeric(_PropertyClassificationTypeId) = True) OrElse (_ProtectionClassId <> "" AndAlso IsNumeric(_ProtectionClassId) = True) OrElse (_Rate <> "" AndAlso IsNumeric(_Rate) = True) OrElse (_RatingTypeId <> "" AndAlso IsNumeric(_RatingTypeId) = True) OrElse (_RetroactiveDate <> "" AndAlso IsDate(_RetroactiveDate) = True) OrElse (_RiskTypeId <> "" AndAlso IsNumeric(_RiskTypeId) = True) OrElse (_SalesClubs <> "" AndAlso IsNumeric(_SalesClubs) = True) OrElse (_SalesManufacturers <> "" AndAlso IsNumeric(_SalesManufacturers) = True) OrElse (_SalesPackageStores <> "" AndAlso IsNumeric(_SalesPackageStores) = True) OrElse (_SalesRestaurants <> "" AndAlso IsNumeric(_SalesRestaurants) = True) OrElse _SerialNumber <> "" OrElse _ShouldSyncWithMasterCoverage = True OrElse (_SpecialClassCodeTypeId <> "" AndAlso IsNumeric(_SpecialClassCodeTypeId) = True) OrElse _VIN <> "" OrElse (_ValuationMethodTypeId <> "" AndAlso IsNumeric(_ValuationMethodTypeId) = True) OrElse (_WaitingPeriodTypeId <> "" AndAlso IsNumeric(_WaitingPeriodTypeId) = True) OrElse (_Year <> "" AndAlso IsNumeric(_Year) = True) OrElse _IsNamedPerils = True OrElse _ExcludeEarthquake = True OrElse _DescriptionOfOperations <> "" OrElse _DesignatedJobSite <> "" OrElse _HasLoadingUnloading = True OrElse qqHelper.IsNumericString(_NumberOfVehicles) = True OrElse qqHelper.IsNumericString(_NumberOfShipments) = True OrElse qqHelper.IsNumericString(_OperatingRadius) = True OrElse _City <> "" OrElse _ExcludedEggs = True OrElse _ExcludedFursOrFurTrimmedGarments = True OrElse _ExcludedLiquor = True OrElse _ExcludedLivestockOrPoultry = True OrElse _ExcludedTobacco = True OrElse qqHelper.IsNumericString(_StateId) = True OrElse qqHelper.IsNumericString(_WithinMiles) = True OrElse qqHelper.IsNumericString(_MaximumDeductible) = True OrElse qqHelper.IsNumericString(_MinimumDeductible) = True OrElse _IsIndoor = True Then 'may not use _ShouldSyncWithMasterCoverage
                'updated 3/30/2015 for FaithfulPerformanceOfDutyTypeId, NumberOfAdditionalPremises, LimitTypeId, and EmployeeTheftScheduleTypeId; just using numeric, but may need > 0; same thing for NumberOfCardHolders, PremiumChargeTypeId, and BasisTypeId added 3/31/2015; also updated for more fields 3/31/2015: IncludeSellingPrice, IsIncludeGuestsProperty, IsLimitToRobberyOnly, IsRequireRecordOfChecks, FromDate, ToDate... date fields currently just looking for valid date... may need to verify that it's greater than default date (1/1/1800)
                'If _AddressInformation <> "" OrElse (_AggregateLimit <> "" AndAlso IsNumeric(_AggregateLimit) = True) OrElse (_AutomaticIncreasePercentTypeId <> "" AndAlso IsNumeric(_AutomaticIncreasePercentTypeId) = True) OrElse (_BusinessIncomeTypeId <> "" AndAlso IsNumeric(_BusinessIncomeTypeId) = True) OrElse (_BusinessPropertyTypeId <> "" AndAlso IsNumeric(_BusinessPropertyTypeId) = True) OrElse (_CauseOfLossTypeId <> "" AndAlso IsNumeric(_CauseOfLossTypeId) = True) OrElse (_CoinsuranceTypeId <> "" AndAlso IsNumeric(_CoinsuranceTypeId) = True) OrElse _Comments <> "" OrElse (_CommercialOccupancyTypeId <> "" AndAlso IsNumeric(_CommercialOccupancyTypeId) = True) OrElse (_ConstructionTypeId <> "" AndAlso IsNumeric(_ConstructionTypeId) = True) OrElse (_CoverageBasisTypeId <> "" AndAlso IsNumeric(_CoverageBasisTypeId) = True) OrElse (_CoverageTypeId <> "" AndAlso IsNumeric(_CoverageTypeId) = True) OrElse (_DailyReimbursement <> "" AndAlso IsNumeric(_DailyReimbursement) = True) OrElse (_DeductibleCategoryTypeId <> "" AndAlso IsNumeric(_DeductibleCategoryTypeId) = True) OrElse (_DeductiblePerTypeId <> "" AndAlso IsNumeric(_DeductiblePerTypeId) = True) OrElse (_DoesYardRateApplyTypeId <> "" AndAlso IsNumeric(_DoesYardRateApplyTypeId) = True) OrElse _ExcludeProductsCompletedOperations = True OrElse (_ExtendNonOwnershipRatingTypeId <> "" AndAlso IsNumeric(_ExtendNonOwnershipRatingTypeId) = True) OrElse (_FeetToFireHydrant <> "" AndAlso IsNumeric(_FeetToFireHydrant) = True) OrElse _HasWaiverOfSubrogation = True OrElse _IfAnyBasis = True OrElse (_InflationGuardTypeId <> "" AndAlso IsNumeric(_InflationGuardTypeId) = True) OrElse _IsBreakdownOrContamination = True OrElse _IsCondoCommercialUnitOwnersCoverage = True OrElse _IsEarthquakeApplies = True OrElse _IsIncludedInBlanketRating = True OrElse _IsMineSubsidence = True OrElse _IsPowerOutage = True OrElse _IsRefrigerationMaintenanceAgreement = True OrElse (_LossCost <> "" AndAlso IsNumeric(_LossCost) = True) OrElse _Make <> "" OrElse (_ManualElpaRate <> "" AndAlso IsNumeric(_ManualElpaRate) = True) OrElse _ManuallyRated = True OrElse (_ManualPremium <> "" AndAlso IsNumeric(_ManualPremium) = True) OrElse _ManufacturerName <> "" OrElse (_MilesToFireDepartment <> "" AndAlso IsNumeric(_MilesToFireDepartment) = True) OrElse _Model <> "" OrElse (_MonthlyPeriodTypeId <> "" AndAlso IsNumeric(_MonthlyPeriodTypeId) = True) OrElse (_NamedInsuredsBusinessTypeId <> "" AndAlso IsNumeric(_NamedInsuredsBusinessTypeId) = True) OrElse _NameInformation <> "" OrElse (_NumberOfBodies <> "" AndAlso IsNumeric(_NumberOfBodies) = True) OrElse (_NumberOfBurials <> "" AndAlso IsNumeric(_NumberOfBurials) = True) OrElse (_NumberOfContractors <> "" AndAlso IsNumeric(_NumberOfContractors) = True) OrElse (_NumberOfDays <> "" AndAlso IsNumeric(_NumberOfDays) = True) OrElse (_NumberOfEmployees <> "" AndAlso IsNumeric(_NumberOfEmployees) = True) OrElse (_NumberOfExcludedEmployees <> "" AndAlso IsNumeric(_NumberOfExcludedEmployees) = True) OrElse (_NumberOfIndependentContractors <> "" AndAlso IsNumeric(_NumberOfIndependentContractors) = True) OrElse (_NumberOfLocations <> "" AndAlso IsNumeric(_NumberOfLocations) = True) OrElse (_NumberOfPartTimeEmployees <> "" AndAlso IsNumeric(_NumberOfPartTimeEmployees) = True) OrElse (_NumberOfPastoralCounselors <> "" AndAlso IsNumeric(_NumberOfPastoralCounselors) = True) OrElse (_NumberOfPremises <> "" AndAlso IsNumeric(_NumberOfPremises) = True) OrElse (_NumberOfRatableEmployees <> "" AndAlso IsNumeric(_NumberOfRatableEmployees) = True) OrElse (_NumberOfSwimmingPools <> "" AndAlso IsNumeric(_NumberOfSwimmingPools) = True) OrElse (_NumberOfTemporaryWorkers <> "" AndAlso IsNumeric(_NumberOfTemporaryWorkers) = True) OrElse qqHelper.IsNumericString(_NumberOfAdditionalPremises) = True OrElse (_OtherThanCollisionSubTypeId <> "" AndAlso IsNumeric(_OtherThanCollisionSubTypeId) = True) OrElse (_OtherThanCollisionTypeId <> "" AndAlso IsNumeric(_OtherThanCollisionTypeId) = True) OrElse (_PercentOfWorkers <> "" AndAlso IsNumeric(_PercentOfWorkers) = True) OrElse (_PersonalPropertyRateGradeTypeId <> "" AndAlso IsNumeric(_PersonalPropertyRateGradeTypeId) = True) OrElse _ProductDescription <> "" OrElse (_PropertyClassificationTypeId <> "" AndAlso IsNumeric(_PropertyClassificationTypeId) = True) OrElse (_ProtectionClassId <> "" AndAlso IsNumeric(_ProtectionClassId) = True) OrElse (_Rate <> "" AndAlso IsNumeric(_Rate) = True) OrElse (_RatingTypeId <> "" AndAlso IsNumeric(_RatingTypeId) = True) OrElse (_RetroactiveDate <> "" AndAlso IsDate(_RetroactiveDate) = True) OrElse (_RiskTypeId <> "" AndAlso IsNumeric(_RiskTypeId) = True) OrElse (_SalesClubs <> "" AndAlso IsNumeric(_SalesClubs) = True) OrElse (_SalesManufacturers <> "" AndAlso IsNumeric(_SalesManufacturers) = True) OrElse (_SalesPackageStores <> "" AndAlso IsNumeric(_SalesPackageStores) = True) OrElse (_SalesRestaurants <> "" AndAlso IsNumeric(_SalesRestaurants) = True) OrElse _SerialNumber <> "" OrElse _ShouldSyncWithMasterCoverage = True OrElse (_SpecialClassCodeTypeId <> "" AndAlso IsNumeric(_SpecialClassCodeTypeId) = True) OrElse _VIN <> "" OrElse (_ValuationMethodTypeId <> "" AndAlso IsNumeric(_ValuationMethodTypeId) = True) OrElse (_WaitingPeriodTypeId <> "" AndAlso IsNumeric(_WaitingPeriodTypeId) = True) OrElse (_Year <> "" AndAlso IsNumeric(_Year) = True) OrElse _IsNamedPerils = True OrElse _ExcludeEarthquake = True OrElse _DescriptionOfOperations <> "" OrElse _DesignatedJobSite <> "" OrElse _HasLoadingUnloading = True OrElse qqHelper.IsNumericString(_NumberOfVehicles) = True OrElse qqHelper.IsNumericString(_NumberOfShipments) = True OrElse qqHelper.IsNumericString(_OperatingRadius) = True OrElse _City <> "" OrElse _ExcludedEggs = True OrElse _ExcludedFursOrFurTrimmedGarments = True OrElse _ExcludedLiquor = True OrElse _ExcludedLivestockOrPoultry = True OrElse _ExcludedTobacco = True OrElse qqHelper.IsNumericString(_StateId) = True OrElse qqHelper.IsNumericString(_WithinMiles) = True OrElse qqHelper.IsNumericString(_MaximumDeductible) = True OrElse qqHelper.IsNumericString(_MinimumDeductible) = True OrElse _IsIndoor = True OrElse qqHelper.IsNumericString(_FaithfulPerformanceOfDutyTypeId) = True OrElse qqHelper.IsNumericString(_LimitTypeId) = True OrElse qqHelper.IsNumericString(_EmployeeTheftScheduleTypeId) = True OrElse qqHelper.IsNumericString(_NumberOfCardHolders) = True OrElse qqHelper.IsNumericString(_PremiumChargeTypeId) = True OrElse qqHelper.IsNumericString(_BasisTypeId) = True OrElse _IncludeSellingPrice = True OrElse _IsIncludeGuestsProperty = True OrElse _IsLimitToRobberyOnly = True OrElse _IsRequireRecordOfChecks = True OrElse qqHelper.IsDateString(_FromDate) = True OrElse qqHelper.IsDateString(_ToDate) = True Then 'may not use _ShouldSyncWithMasterCoverage
                'added 6/8/2015 for testing latest CPP enhancements; CoverageDescription
                'If _AddressInformation <> "" OrElse (_AggregateLimit <> "" AndAlso IsNumeric(_AggregateLimit) = True) OrElse (_AutomaticIncreasePercentTypeId <> "" AndAlso IsNumeric(_AutomaticIncreasePercentTypeId) = True) OrElse (_BusinessIncomeTypeId <> "" AndAlso IsNumeric(_BusinessIncomeTypeId) = True) OrElse (_BusinessPropertyTypeId <> "" AndAlso IsNumeric(_BusinessPropertyTypeId) = True) OrElse (_CauseOfLossTypeId <> "" AndAlso IsNumeric(_CauseOfLossTypeId) = True) OrElse (_CoinsuranceTypeId <> "" AndAlso IsNumeric(_CoinsuranceTypeId) = True) OrElse _Comments <> "" OrElse (_CommercialOccupancyTypeId <> "" AndAlso IsNumeric(_CommercialOccupancyTypeId) = True) OrElse (_ConstructionTypeId <> "" AndAlso IsNumeric(_ConstructionTypeId) = True) OrElse (_CoverageBasisTypeId <> "" AndAlso IsNumeric(_CoverageBasisTypeId) = True) OrElse (_CoverageTypeId <> "" AndAlso IsNumeric(_CoverageTypeId) = True) OrElse (_DailyReimbursement <> "" AndAlso IsNumeric(_DailyReimbursement) = True) OrElse (_DeductibleCategoryTypeId <> "" AndAlso IsNumeric(_DeductibleCategoryTypeId) = True) OrElse (_DeductiblePerTypeId <> "" AndAlso IsNumeric(_DeductiblePerTypeId) = True) OrElse (_DoesYardRateApplyTypeId <> "" AndAlso IsNumeric(_DoesYardRateApplyTypeId) = True) OrElse _ExcludeProductsCompletedOperations = True OrElse (_ExtendNonOwnershipRatingTypeId <> "" AndAlso IsNumeric(_ExtendNonOwnershipRatingTypeId) = True) OrElse (_FeetToFireHydrant <> "" AndAlso IsNumeric(_FeetToFireHydrant) = True) OrElse _HasWaiverOfSubrogation = True OrElse _IfAnyBasis = True OrElse (_InflationGuardTypeId <> "" AndAlso IsNumeric(_InflationGuardTypeId) = True) OrElse _IsBreakdownOrContamination = True OrElse _IsCondoCommercialUnitOwnersCoverage = True OrElse _IsEarthquakeApplies = True OrElse _IsIncludedInBlanketRating = True OrElse _IsMineSubsidence = True OrElse _IsPowerOutage = True OrElse _IsRefrigerationMaintenanceAgreement = True OrElse (_LossCost <> "" AndAlso IsNumeric(_LossCost) = True) OrElse _Make <> "" OrElse (_ManualElpaRate <> "" AndAlso IsNumeric(_ManualElpaRate) = True) OrElse _ManuallyRated = True OrElse (_ManualPremium <> "" AndAlso IsNumeric(_ManualPremium) = True) OrElse _ManufacturerName <> "" OrElse (_MilesToFireDepartment <> "" AndAlso IsNumeric(_MilesToFireDepartment) = True) OrElse _Model <> "" OrElse (_MonthlyPeriodTypeId <> "" AndAlso IsNumeric(_MonthlyPeriodTypeId) = True) OrElse (_NamedInsuredsBusinessTypeId <> "" AndAlso IsNumeric(_NamedInsuredsBusinessTypeId) = True) OrElse _NameInformation <> "" OrElse (_NumberOfBodies <> "" AndAlso IsNumeric(_NumberOfBodies) = True) OrElse (_NumberOfBurials <> "" AndAlso IsNumeric(_NumberOfBurials) = True) OrElse (_NumberOfContractors <> "" AndAlso IsNumeric(_NumberOfContractors) = True) OrElse (_NumberOfDays <> "" AndAlso IsNumeric(_NumberOfDays) = True) OrElse (_NumberOfEmployees <> "" AndAlso IsNumeric(_NumberOfEmployees) = True) OrElse (_NumberOfExcludedEmployees <> "" AndAlso IsNumeric(_NumberOfExcludedEmployees) = True) OrElse (_NumberOfIndependentContractors <> "" AndAlso IsNumeric(_NumberOfIndependentContractors) = True) OrElse (_NumberOfLocations <> "" AndAlso IsNumeric(_NumberOfLocations) = True) OrElse (_NumberOfPartTimeEmployees <> "" AndAlso IsNumeric(_NumberOfPartTimeEmployees) = True) OrElse (_NumberOfPastoralCounselors <> "" AndAlso IsNumeric(_NumberOfPastoralCounselors) = True) OrElse (_NumberOfPremises <> "" AndAlso IsNumeric(_NumberOfPremises) = True) OrElse (_NumberOfRatableEmployees <> "" AndAlso IsNumeric(_NumberOfRatableEmployees) = True) OrElse (_NumberOfSwimmingPools <> "" AndAlso IsNumeric(_NumberOfSwimmingPools) = True) OrElse (_NumberOfTemporaryWorkers <> "" AndAlso IsNumeric(_NumberOfTemporaryWorkers) = True) OrElse qqHelper.IsNumericString(_NumberOfAdditionalPremises) = True OrElse (_OtherThanCollisionSubTypeId <> "" AndAlso IsNumeric(_OtherThanCollisionSubTypeId) = True) OrElse (_OtherThanCollisionTypeId <> "" AndAlso IsNumeric(_OtherThanCollisionTypeId) = True) OrElse (_PercentOfWorkers <> "" AndAlso IsNumeric(_PercentOfWorkers) = True) OrElse (_PersonalPropertyRateGradeTypeId <> "" AndAlso IsNumeric(_PersonalPropertyRateGradeTypeId) = True) OrElse _ProductDescription <> "" OrElse (_PropertyClassificationTypeId <> "" AndAlso IsNumeric(_PropertyClassificationTypeId) = True) OrElse (_ProtectionClassId <> "" AndAlso IsNumeric(_ProtectionClassId) = True) OrElse (_Rate <> "" AndAlso IsNumeric(_Rate) = True) OrElse (_RatingTypeId <> "" AndAlso IsNumeric(_RatingTypeId) = True) OrElse (_RetroactiveDate <> "" AndAlso IsDate(_RetroactiveDate) = True) OrElse (_RiskTypeId <> "" AndAlso IsNumeric(_RiskTypeId) = True) OrElse (_SalesClubs <> "" AndAlso IsNumeric(_SalesClubs) = True) OrElse (_SalesManufacturers <> "" AndAlso IsNumeric(_SalesManufacturers) = True) OrElse (_SalesPackageStores <> "" AndAlso IsNumeric(_SalesPackageStores) = True) OrElse (_SalesRestaurants <> "" AndAlso IsNumeric(_SalesRestaurants) = True) OrElse _SerialNumber <> "" OrElse _ShouldSyncWithMasterCoverage = True OrElse (_SpecialClassCodeTypeId <> "" AndAlso IsNumeric(_SpecialClassCodeTypeId) = True) OrElse _VIN <> "" OrElse (_ValuationMethodTypeId <> "" AndAlso IsNumeric(_ValuationMethodTypeId) = True) OrElse (_WaitingPeriodTypeId <> "" AndAlso IsNumeric(_WaitingPeriodTypeId) = True) OrElse (_Year <> "" AndAlso IsNumeric(_Year) = True) OrElse _IsNamedPerils = True OrElse _ExcludeEarthquake = True OrElse _DescriptionOfOperations <> "" OrElse _DesignatedJobSite <> "" OrElse _HasLoadingUnloading = True OrElse qqHelper.IsNumericString(_NumberOfVehicles) = True OrElse qqHelper.IsNumericString(_NumberOfShipments) = True OrElse qqHelper.IsNumericString(_OperatingRadius) = True OrElse _City <> "" OrElse _ExcludedEggs = True OrElse _ExcludedFursOrFurTrimmedGarments = True OrElse _ExcludedLiquor = True OrElse _ExcludedLivestockOrPoultry = True OrElse _ExcludedTobacco = True OrElse qqHelper.IsNumericString(_StateId) = True OrElse qqHelper.IsNumericString(_WithinMiles) = True OrElse qqHelper.IsNumericString(_MaximumDeductible) = True OrElse qqHelper.IsNumericString(_MinimumDeductible) = True OrElse _IsIndoor = True OrElse qqHelper.IsNumericString(_FaithfulPerformanceOfDutyTypeId) = True OrElse qqHelper.IsNumericString(_LimitTypeId) = True OrElse qqHelper.IsNumericString(_EmployeeTheftScheduleTypeId) = True OrElse qqHelper.IsNumericString(_NumberOfCardHolders) = True OrElse qqHelper.IsNumericString(_PremiumChargeTypeId) = True OrElse qqHelper.IsNumericString(_BasisTypeId) = True OrElse _IncludeSellingPrice = True OrElse _IsIncludeGuestsProperty = True OrElse _IsLimitToRobberyOnly = True OrElse _IsRequireRecordOfChecks = True OrElse qqHelper.IsDateString(_FromDate) = True OrElse qqHelper.IsDateString(_ToDate) = True OrElse _CoverageDescription <> "" Then 'may not use _ShouldSyncWithMasterCoverage
                'updated 7/15/2015 for IsEmployeeTools and IsToolsLeasedOrRented
                'If _AddressInformation <> "" OrElse (_AggregateLimit <> "" AndAlso IsNumeric(_AggregateLimit) = True) OrElse (_AutomaticIncreasePercentTypeId <> "" AndAlso IsNumeric(_AutomaticIncreasePercentTypeId) = True) OrElse (_BusinessIncomeTypeId <> "" AndAlso IsNumeric(_BusinessIncomeTypeId) = True) OrElse (_BusinessPropertyTypeId <> "" AndAlso IsNumeric(_BusinessPropertyTypeId) = True) OrElse (_CauseOfLossTypeId <> "" AndAlso IsNumeric(_CauseOfLossTypeId) = True) OrElse (_CoinsuranceTypeId <> "" AndAlso IsNumeric(_CoinsuranceTypeId) = True) OrElse _Comments <> "" OrElse (_CommercialOccupancyTypeId <> "" AndAlso IsNumeric(_CommercialOccupancyTypeId) = True) OrElse (_ConstructionTypeId <> "" AndAlso IsNumeric(_ConstructionTypeId) = True) OrElse (_CoverageBasisTypeId <> "" AndAlso IsNumeric(_CoverageBasisTypeId) = True) OrElse (_CoverageTypeId <> "" AndAlso IsNumeric(_CoverageTypeId) = True) OrElse (_DailyReimbursement <> "" AndAlso IsNumeric(_DailyReimbursement) = True) OrElse (_DeductibleCategoryTypeId <> "" AndAlso IsNumeric(_DeductibleCategoryTypeId) = True) OrElse (_DeductiblePerTypeId <> "" AndAlso IsNumeric(_DeductiblePerTypeId) = True) OrElse (_DoesYardRateApplyTypeId <> "" AndAlso IsNumeric(_DoesYardRateApplyTypeId) = True) OrElse _ExcludeProductsCompletedOperations = True OrElse (_ExtendNonOwnershipRatingTypeId <> "" AndAlso IsNumeric(_ExtendNonOwnershipRatingTypeId) = True) OrElse (_FeetToFireHydrant <> "" AndAlso IsNumeric(_FeetToFireHydrant) = True) OrElse _HasWaiverOfSubrogation = True OrElse _IfAnyBasis = True OrElse (_InflationGuardTypeId <> "" AndAlso IsNumeric(_InflationGuardTypeId) = True) OrElse _IsBreakdownOrContamination = True OrElse _IsCondoCommercialUnitOwnersCoverage = True OrElse _IsEarthquakeApplies = True OrElse _IsIncludedInBlanketRating = True OrElse _IsMineSubsidence = True OrElse _IsPowerOutage = True OrElse _IsRefrigerationMaintenanceAgreement = True OrElse (_LossCost <> "" AndAlso IsNumeric(_LossCost) = True) OrElse _Make <> "" OrElse (_ManualElpaRate <> "" AndAlso IsNumeric(_ManualElpaRate) = True) OrElse _ManuallyRated = True OrElse (_ManualPremium <> "" AndAlso IsNumeric(_ManualPremium) = True) OrElse _ManufacturerName <> "" OrElse (_MilesToFireDepartment <> "" AndAlso IsNumeric(_MilesToFireDepartment) = True) OrElse _Model <> "" OrElse (_MonthlyPeriodTypeId <> "" AndAlso IsNumeric(_MonthlyPeriodTypeId) = True) OrElse (_NamedInsuredsBusinessTypeId <> "" AndAlso IsNumeric(_NamedInsuredsBusinessTypeId) = True) OrElse _NameInformation <> "" OrElse (_NumberOfBodies <> "" AndAlso IsNumeric(_NumberOfBodies) = True) OrElse (_NumberOfBurials <> "" AndAlso IsNumeric(_NumberOfBurials) = True) OrElse (_NumberOfContractors <> "" AndAlso IsNumeric(_NumberOfContractors) = True) OrElse (_NumberOfDays <> "" AndAlso IsNumeric(_NumberOfDays) = True) OrElse (_NumberOfEmployees <> "" AndAlso IsNumeric(_NumberOfEmployees) = True) OrElse (_NumberOfExcludedEmployees <> "" AndAlso IsNumeric(_NumberOfExcludedEmployees) = True) OrElse (_NumberOfIndependentContractors <> "" AndAlso IsNumeric(_NumberOfIndependentContractors) = True) OrElse (_NumberOfLocations <> "" AndAlso IsNumeric(_NumberOfLocations) = True) OrElse (_NumberOfPartTimeEmployees <> "" AndAlso IsNumeric(_NumberOfPartTimeEmployees) = True) OrElse (_NumberOfPastoralCounselors <> "" AndAlso IsNumeric(_NumberOfPastoralCounselors) = True) OrElse (_NumberOfPremises <> "" AndAlso IsNumeric(_NumberOfPremises) = True) OrElse (_NumberOfRatableEmployees <> "" AndAlso IsNumeric(_NumberOfRatableEmployees) = True) OrElse (_NumberOfSwimmingPools <> "" AndAlso IsNumeric(_NumberOfSwimmingPools) = True) OrElse (_NumberOfTemporaryWorkers <> "" AndAlso IsNumeric(_NumberOfTemporaryWorkers) = True) OrElse qqHelper.IsNumericString(_NumberOfAdditionalPremises) = True OrElse (_OtherThanCollisionSubTypeId <> "" AndAlso IsNumeric(_OtherThanCollisionSubTypeId) = True) OrElse (_OtherThanCollisionTypeId <> "" AndAlso IsNumeric(_OtherThanCollisionTypeId) = True) OrElse (_PercentOfWorkers <> "" AndAlso IsNumeric(_PercentOfWorkers) = True) OrElse (_PersonalPropertyRateGradeTypeId <> "" AndAlso IsNumeric(_PersonalPropertyRateGradeTypeId) = True) OrElse _ProductDescription <> "" OrElse (_PropertyClassificationTypeId <> "" AndAlso IsNumeric(_PropertyClassificationTypeId) = True) OrElse (_ProtectionClassId <> "" AndAlso IsNumeric(_ProtectionClassId) = True) OrElse (_Rate <> "" AndAlso IsNumeric(_Rate) = True) OrElse (_RatingTypeId <> "" AndAlso IsNumeric(_RatingTypeId) = True) OrElse (_RetroactiveDate <> "" AndAlso IsDate(_RetroactiveDate) = True) OrElse (_RiskTypeId <> "" AndAlso IsNumeric(_RiskTypeId) = True) OrElse (_SalesClubs <> "" AndAlso IsNumeric(_SalesClubs) = True) OrElse (_SalesManufacturers <> "" AndAlso IsNumeric(_SalesManufacturers) = True) OrElse (_SalesPackageStores <> "" AndAlso IsNumeric(_SalesPackageStores) = True) OrElse (_SalesRestaurants <> "" AndAlso IsNumeric(_SalesRestaurants) = True) OrElse _SerialNumber <> "" OrElse _ShouldSyncWithMasterCoverage = True OrElse (_SpecialClassCodeTypeId <> "" AndAlso IsNumeric(_SpecialClassCodeTypeId) = True) OrElse _VIN <> "" OrElse (_ValuationMethodTypeId <> "" AndAlso IsNumeric(_ValuationMethodTypeId) = True) OrElse (_WaitingPeriodTypeId <> "" AndAlso IsNumeric(_WaitingPeriodTypeId) = True) OrElse (_Year <> "" AndAlso IsNumeric(_Year) = True) OrElse _IsNamedPerils = True OrElse _ExcludeEarthquake = True OrElse _DescriptionOfOperations <> "" OrElse _DesignatedJobSite <> "" OrElse _HasLoadingUnloading = True OrElse qqHelper.IsNumericString(_NumberOfVehicles) = True OrElse qqHelper.IsNumericString(_NumberOfShipments) = True OrElse qqHelper.IsNumericString(_OperatingRadius) = True OrElse _City <> "" OrElse _ExcludedEggs = True OrElse _ExcludedFursOrFurTrimmedGarments = True OrElse _ExcludedLiquor = True OrElse _ExcludedLivestockOrPoultry = True OrElse _ExcludedTobacco = True OrElse qqHelper.IsNumericString(_StateId) = True OrElse qqHelper.IsNumericString(_WithinMiles) = True OrElse qqHelper.IsNumericString(_MaximumDeductible) = True OrElse qqHelper.IsNumericString(_MinimumDeductible) = True OrElse _IsIndoor = True OrElse qqHelper.IsNumericString(_FaithfulPerformanceOfDutyTypeId) = True OrElse qqHelper.IsNumericString(_LimitTypeId) = True OrElse qqHelper.IsNumericString(_EmployeeTheftScheduleTypeId) = True OrElse qqHelper.IsNumericString(_NumberOfCardHolders) = True OrElse qqHelper.IsNumericString(_PremiumChargeTypeId) = True OrElse qqHelper.IsNumericString(_BasisTypeId) = True OrElse _IncludeSellingPrice = True OrElse _IsIncludeGuestsProperty = True OrElse _IsLimitToRobberyOnly = True OrElse _IsRequireRecordOfChecks = True OrElse qqHelper.IsDateString(_FromDate) = True OrElse qqHelper.IsDateString(_ToDate) = True OrElse _CoverageDescription <> "" OrElse _IsEmployeeTools = True OrElse _IsToolsLeasedOrRented = True Then 'may not use _ShouldSyncWithMasterCoverage
                'updated 5/3/2016 for IsLimitedPerilsExtendedCoverage
                'If _AddressInformation <> "" OrElse (_AggregateLimit <> "" AndAlso IsNumeric(_AggregateLimit) = True) OrElse (_AutomaticIncreasePercentTypeId <> "" AndAlso IsNumeric(_AutomaticIncreasePercentTypeId) = True) OrElse (_BusinessIncomeTypeId <> "" AndAlso IsNumeric(_BusinessIncomeTypeId) = True) OrElse (_BusinessPropertyTypeId <> "" AndAlso IsNumeric(_BusinessPropertyTypeId) = True) OrElse (_CauseOfLossTypeId <> "" AndAlso IsNumeric(_CauseOfLossTypeId) = True) OrElse (_CoinsuranceTypeId <> "" AndAlso IsNumeric(_CoinsuranceTypeId) = True) OrElse _Comments <> "" OrElse (_CommercialOccupancyTypeId <> "" AndAlso IsNumeric(_CommercialOccupancyTypeId) = True) OrElse (_ConstructionTypeId <> "" AndAlso IsNumeric(_ConstructionTypeId) = True) OrElse (_CoverageBasisTypeId <> "" AndAlso IsNumeric(_CoverageBasisTypeId) = True) OrElse (_CoverageTypeId <> "" AndAlso IsNumeric(_CoverageTypeId) = True) OrElse (_DailyReimbursement <> "" AndAlso IsNumeric(_DailyReimbursement) = True) OrElse (_DeductibleCategoryTypeId <> "" AndAlso IsNumeric(_DeductibleCategoryTypeId) = True) OrElse (_DeductiblePerTypeId <> "" AndAlso IsNumeric(_DeductiblePerTypeId) = True) OrElse (_DoesYardRateApplyTypeId <> "" AndAlso IsNumeric(_DoesYardRateApplyTypeId) = True) OrElse _ExcludeProductsCompletedOperations = True OrElse (_ExtendNonOwnershipRatingTypeId <> "" AndAlso IsNumeric(_ExtendNonOwnershipRatingTypeId) = True) OrElse (_FeetToFireHydrant <> "" AndAlso IsNumeric(_FeetToFireHydrant) = True) OrElse _HasWaiverOfSubrogation = True OrElse _IfAnyBasis = True OrElse (_InflationGuardTypeId <> "" AndAlso IsNumeric(_InflationGuardTypeId) = True) OrElse _IsBreakdownOrContamination = True OrElse _IsCondoCommercialUnitOwnersCoverage = True OrElse _IsEarthquakeApplies = True OrElse _IsIncludedInBlanketRating = True OrElse _IsMineSubsidence = True OrElse _IsPowerOutage = True OrElse _IsRefrigerationMaintenanceAgreement = True OrElse (_LossCost <> "" AndAlso IsNumeric(_LossCost) = True) OrElse _Make <> "" OrElse (_ManualElpaRate <> "" AndAlso IsNumeric(_ManualElpaRate) = True) OrElse _ManuallyRated = True OrElse (_ManualPremium <> "" AndAlso IsNumeric(_ManualPremium) = True) OrElse _ManufacturerName <> "" OrElse (_MilesToFireDepartment <> "" AndAlso IsNumeric(_MilesToFireDepartment) = True) OrElse _Model <> "" OrElse (_MonthlyPeriodTypeId <> "" AndAlso IsNumeric(_MonthlyPeriodTypeId) = True) OrElse (_NamedInsuredsBusinessTypeId <> "" AndAlso IsNumeric(_NamedInsuredsBusinessTypeId) = True) OrElse _NameInformation <> "" OrElse (_NumberOfBodies <> "" AndAlso IsNumeric(_NumberOfBodies) = True) OrElse (_NumberOfBurials <> "" AndAlso IsNumeric(_NumberOfBurials) = True) OrElse (_NumberOfContractors <> "" AndAlso IsNumeric(_NumberOfContractors) = True) OrElse (_NumberOfDays <> "" AndAlso IsNumeric(_NumberOfDays) = True) OrElse (_NumberOfEmployees <> "" AndAlso IsNumeric(_NumberOfEmployees) = True) OrElse (_NumberOfExcludedEmployees <> "" AndAlso IsNumeric(_NumberOfExcludedEmployees) = True) OrElse (_NumberOfIndependentContractors <> "" AndAlso IsNumeric(_NumberOfIndependentContractors) = True) OrElse (_NumberOfLocations <> "" AndAlso IsNumeric(_NumberOfLocations) = True) OrElse (_NumberOfPartTimeEmployees <> "" AndAlso IsNumeric(_NumberOfPartTimeEmployees) = True) OrElse (_NumberOfPastoralCounselors <> "" AndAlso IsNumeric(_NumberOfPastoralCounselors) = True) OrElse (_NumberOfPremises <> "" AndAlso IsNumeric(_NumberOfPremises) = True) OrElse (_NumberOfRatableEmployees <> "" AndAlso IsNumeric(_NumberOfRatableEmployees) = True) OrElse (_NumberOfSwimmingPools <> "" AndAlso IsNumeric(_NumberOfSwimmingPools) = True) OrElse (_NumberOfTemporaryWorkers <> "" AndAlso IsNumeric(_NumberOfTemporaryWorkers) = True) OrElse qqHelper.IsNumericString(_NumberOfAdditionalPremises) = True OrElse (_OtherThanCollisionSubTypeId <> "" AndAlso IsNumeric(_OtherThanCollisionSubTypeId) = True) OrElse (_OtherThanCollisionTypeId <> "" AndAlso IsNumeric(_OtherThanCollisionTypeId) = True) OrElse (_PercentOfWorkers <> "" AndAlso IsNumeric(_PercentOfWorkers) = True) OrElse (_PersonalPropertyRateGradeTypeId <> "" AndAlso IsNumeric(_PersonalPropertyRateGradeTypeId) = True) OrElse _ProductDescription <> "" OrElse (_PropertyClassificationTypeId <> "" AndAlso IsNumeric(_PropertyClassificationTypeId) = True) OrElse (_ProtectionClassId <> "" AndAlso IsNumeric(_ProtectionClassId) = True) OrElse (_Rate <> "" AndAlso IsNumeric(_Rate) = True) OrElse (_RatingTypeId <> "" AndAlso IsNumeric(_RatingTypeId) = True) OrElse (_RetroactiveDate <> "" AndAlso IsDate(_RetroactiveDate) = True) OrElse (_RiskTypeId <> "" AndAlso IsNumeric(_RiskTypeId) = True) OrElse (_SalesClubs <> "" AndAlso IsNumeric(_SalesClubs) = True) OrElse (_SalesManufacturers <> "" AndAlso IsNumeric(_SalesManufacturers) = True) OrElse (_SalesPackageStores <> "" AndAlso IsNumeric(_SalesPackageStores) = True) OrElse (_SalesRestaurants <> "" AndAlso IsNumeric(_SalesRestaurants) = True) OrElse _SerialNumber <> "" OrElse _ShouldSyncWithMasterCoverage = True OrElse (_SpecialClassCodeTypeId <> "" AndAlso IsNumeric(_SpecialClassCodeTypeId) = True) OrElse _VIN <> "" OrElse (_ValuationMethodTypeId <> "" AndAlso IsNumeric(_ValuationMethodTypeId) = True) OrElse (_WaitingPeriodTypeId <> "" AndAlso IsNumeric(_WaitingPeriodTypeId) = True) OrElse (_Year <> "" AndAlso IsNumeric(_Year) = True) OrElse _IsNamedPerils = True OrElse _ExcludeEarthquake = True OrElse _DescriptionOfOperations <> "" OrElse _DesignatedJobSite <> "" OrElse _HasLoadingUnloading = True OrElse qqHelper.IsNumericString(_NumberOfVehicles) = True OrElse qqHelper.IsNumericString(_NumberOfShipments) = True OrElse qqHelper.IsNumericString(_OperatingRadius) = True OrElse _City <> "" OrElse _ExcludedEggs = True OrElse _ExcludedFursOrFurTrimmedGarments = True OrElse _ExcludedLiquor = True OrElse _ExcludedLivestockOrPoultry = True OrElse _ExcludedTobacco = True OrElse qqHelper.IsNumericString(_StateId) = True OrElse qqHelper.IsNumericString(_WithinMiles) = True OrElse qqHelper.IsNumericString(_MaximumDeductible) = True OrElse qqHelper.IsNumericString(_MinimumDeductible) = True OrElse _IsIndoor = True OrElse qqHelper.IsNumericString(_FaithfulPerformanceOfDutyTypeId) = True OrElse qqHelper.IsNumericString(_LimitTypeId) = True OrElse qqHelper.IsNumericString(_EmployeeTheftScheduleTypeId) = True OrElse qqHelper.IsNumericString(_NumberOfCardHolders) = True OrElse qqHelper.IsNumericString(_PremiumChargeTypeId) = True OrElse qqHelper.IsNumericString(_BasisTypeId) = True OrElse _IncludeSellingPrice = True OrElse _IsIncludeGuestsProperty = True OrElse _IsLimitToRobberyOnly = True OrElse _IsRequireRecordOfChecks = True OrElse qqHelper.IsDateString(_FromDate) = True OrElse qqHelper.IsDateString(_ToDate) = True OrElse _CoverageDescription <> "" OrElse _IsEmployeeTools = True OrElse _IsToolsLeasedOrRented = True OrElse _IsLimitedPerilsExtendedCoverage = True Then 'may not use _ShouldSyncWithMasterCoverage
                '3/9/2017 - BOP stuff; note: Receipts and PriorGrossReceipts are int in Diamond and not dec
                'If _AddressInformation <> "" OrElse (_AggregateLimit <> "" AndAlso IsNumeric(_AggregateLimit) = True) OrElse (_AutomaticIncreasePercentTypeId <> "" AndAlso IsNumeric(_AutomaticIncreasePercentTypeId) = True) OrElse (_BusinessIncomeTypeId <> "" AndAlso IsNumeric(_BusinessIncomeTypeId) = True) OrElse (_BusinessPropertyTypeId <> "" AndAlso IsNumeric(_BusinessPropertyTypeId) = True) OrElse (_CauseOfLossTypeId <> "" AndAlso IsNumeric(_CauseOfLossTypeId) = True) OrElse (_CoinsuranceTypeId <> "" AndAlso IsNumeric(_CoinsuranceTypeId) = True) OrElse _Comments <> "" OrElse (_CommercialOccupancyTypeId <> "" AndAlso IsNumeric(_CommercialOccupancyTypeId) = True) OrElse (_ConstructionTypeId <> "" AndAlso IsNumeric(_ConstructionTypeId) = True) OrElse (_CoverageBasisTypeId <> "" AndAlso IsNumeric(_CoverageBasisTypeId) = True) OrElse (_CoverageTypeId <> "" AndAlso IsNumeric(_CoverageTypeId) = True) OrElse (_DailyReimbursement <> "" AndAlso IsNumeric(_DailyReimbursement) = True) OrElse (_DeductibleCategoryTypeId <> "" AndAlso IsNumeric(_DeductibleCategoryTypeId) = True) OrElse (_DeductiblePerTypeId <> "" AndAlso IsNumeric(_DeductiblePerTypeId) = True) OrElse (_DoesYardRateApplyTypeId <> "" AndAlso IsNumeric(_DoesYardRateApplyTypeId) = True) OrElse _ExcludeProductsCompletedOperations = True OrElse (_ExtendNonOwnershipRatingTypeId <> "" AndAlso IsNumeric(_ExtendNonOwnershipRatingTypeId) = True) OrElse (_FeetToFireHydrant <> "" AndAlso IsNumeric(_FeetToFireHydrant) = True) OrElse _HasWaiverOfSubrogation = True OrElse _IfAnyBasis = True OrElse (_InflationGuardTypeId <> "" AndAlso IsNumeric(_InflationGuardTypeId) = True) OrElse _IsBreakdownOrContamination = True OrElse _IsCondoCommercialUnitOwnersCoverage = True OrElse _IsEarthquakeApplies = True OrElse _IsIncludedInBlanketRating = True OrElse _IsMineSubsidence = True OrElse _IsPowerOutage = True OrElse _IsRefrigerationMaintenanceAgreement = True OrElse (_LossCost <> "" AndAlso IsNumeric(_LossCost) = True) OrElse _Make <> "" OrElse (_ManualElpaRate <> "" AndAlso IsNumeric(_ManualElpaRate) = True) OrElse _ManuallyRated = True OrElse (_ManualPremium <> "" AndAlso IsNumeric(_ManualPremium) = True) OrElse _ManufacturerName <> "" OrElse (_MilesToFireDepartment <> "" AndAlso IsNumeric(_MilesToFireDepartment) = True) OrElse _Model <> "" OrElse (_MonthlyPeriodTypeId <> "" AndAlso IsNumeric(_MonthlyPeriodTypeId) = True) OrElse (_NamedInsuredsBusinessTypeId <> "" AndAlso IsNumeric(_NamedInsuredsBusinessTypeId) = True) OrElse _NameInformation <> "" OrElse (_NumberOfBodies <> "" AndAlso IsNumeric(_NumberOfBodies) = True) OrElse (_NumberOfBurials <> "" AndAlso IsNumeric(_NumberOfBurials) = True) OrElse (_NumberOfContractors <> "" AndAlso IsNumeric(_NumberOfContractors) = True) OrElse (_NumberOfDays <> "" AndAlso IsNumeric(_NumberOfDays) = True) OrElse (_NumberOfEmployees <> "" AndAlso IsNumeric(_NumberOfEmployees) = True) OrElse (_NumberOfExcludedEmployees <> "" AndAlso IsNumeric(_NumberOfExcludedEmployees) = True) OrElse (_NumberOfIndependentContractors <> "" AndAlso IsNumeric(_NumberOfIndependentContractors) = True) OrElse (_NumberOfLocations <> "" AndAlso IsNumeric(_NumberOfLocations) = True) OrElse (_NumberOfPartTimeEmployees <> "" AndAlso IsNumeric(_NumberOfPartTimeEmployees) = True) OrElse (_NumberOfPastoralCounselors <> "" AndAlso IsNumeric(_NumberOfPastoralCounselors) = True) OrElse (_NumberOfPremises <> "" AndAlso IsNumeric(_NumberOfPremises) = True) OrElse (_NumberOfRatableEmployees <> "" AndAlso IsNumeric(_NumberOfRatableEmployees) = True) OrElse (_NumberOfSwimmingPools <> "" AndAlso IsNumeric(_NumberOfSwimmingPools) = True) OrElse (_NumberOfTemporaryWorkers <> "" AndAlso IsNumeric(_NumberOfTemporaryWorkers) = True) OrElse qqHelper.IsNumericString(_NumberOfAdditionalPremises) = True OrElse (_OtherThanCollisionSubTypeId <> "" AndAlso IsNumeric(_OtherThanCollisionSubTypeId) = True) OrElse (_OtherThanCollisionTypeId <> "" AndAlso IsNumeric(_OtherThanCollisionTypeId) = True) OrElse (_PercentOfWorkers <> "" AndAlso IsNumeric(_PercentOfWorkers) = True) OrElse (_PersonalPropertyRateGradeTypeId <> "" AndAlso IsNumeric(_PersonalPropertyRateGradeTypeId) = True) OrElse _ProductDescription <> "" OrElse (_PropertyClassificationTypeId <> "" AndAlso IsNumeric(_PropertyClassificationTypeId) = True) OrElse (_ProtectionClassId <> "" AndAlso IsNumeric(_ProtectionClassId) = True) OrElse (_Rate <> "" AndAlso IsNumeric(_Rate) = True) OrElse (_RatingTypeId <> "" AndAlso IsNumeric(_RatingTypeId) = True) OrElse (_RetroactiveDate <> "" AndAlso IsDate(_RetroactiveDate) = True) OrElse (_RiskTypeId <> "" AndAlso IsNumeric(_RiskTypeId) = True) OrElse (_SalesClubs <> "" AndAlso IsNumeric(_SalesClubs) = True) OrElse (_SalesManufacturers <> "" AndAlso IsNumeric(_SalesManufacturers) = True) OrElse (_SalesPackageStores <> "" AndAlso IsNumeric(_SalesPackageStores) = True) OrElse (_SalesRestaurants <> "" AndAlso IsNumeric(_SalesRestaurants) = True) OrElse _SerialNumber <> "" OrElse _ShouldSyncWithMasterCoverage = True OrElse (_SpecialClassCodeTypeId <> "" AndAlso IsNumeric(_SpecialClassCodeTypeId) = True) OrElse _VIN <> "" OrElse (_ValuationMethodTypeId <> "" AndAlso IsNumeric(_ValuationMethodTypeId) = True) OrElse (_WaitingPeriodTypeId <> "" AndAlso IsNumeric(_WaitingPeriodTypeId) = True) OrElse (_Year <> "" AndAlso IsNumeric(_Year) = True) OrElse _IsNamedPerils = True OrElse _ExcludeEarthquake = True OrElse _DescriptionOfOperations <> "" OrElse _DesignatedJobSite <> "" OrElse _HasLoadingUnloading = True OrElse qqHelper.IsNumericString(_NumberOfVehicles) = True OrElse qqHelper.IsNumericString(_NumberOfShipments) = True OrElse qqHelper.IsNumericString(_OperatingRadius) = True OrElse _City <> "" OrElse _ExcludedEggs = True OrElse _ExcludedFursOrFurTrimmedGarments = True OrElse _ExcludedLiquor = True OrElse _ExcludedLivestockOrPoultry = True OrElse _ExcludedTobacco = True OrElse qqHelper.IsNumericString(_StateId) = True OrElse qqHelper.IsNumericString(_WithinMiles) = True OrElse qqHelper.IsNumericString(_MaximumDeductible) = True OrElse qqHelper.IsNumericString(_MinimumDeductible) = True OrElse _IsIndoor = True OrElse qqHelper.IsNumericString(_FaithfulPerformanceOfDutyTypeId) = True OrElse qqHelper.IsNumericString(_LimitTypeId) = True OrElse qqHelper.IsNumericString(_EmployeeTheftScheduleTypeId) = True OrElse qqHelper.IsNumericString(_NumberOfCardHolders) = True OrElse qqHelper.IsNumericString(_PremiumChargeTypeId) = True OrElse qqHelper.IsNumericString(_BasisTypeId) = True OrElse _IncludeSellingPrice = True OrElse _IsIncludeGuestsProperty = True OrElse _IsLimitToRobberyOnly = True OrElse _IsRequireRecordOfChecks = True OrElse qqHelper.IsDateString(_FromDate) = True OrElse qqHelper.IsDateString(_ToDate) = True OrElse _CoverageDescription <> "" OrElse _IsEmployeeTools = True OrElse _IsToolsLeasedOrRented = True OrElse _IsLimitedPerilsExtendedCoverage = True OrElse _IsAgreedValue = True OrElse String.IsNullOrEmpty(_DescriptionInformation) = False OrElse qqHelper.IsPositiveIntegerString(_LiquorLiabilityClassCodeTypeId) = True OrElse qqHelper.IsPositiveIntegerString(_NumberOfAmusementAreas) = True OrElse qqHelper.IsPositiveIntegerString(_Receipts) = True OrElse qqHelper.IsPositiveIntegerString(_PriorGrossReceipts) = True Then 'may not use _ShouldSyncWithMasterCoverage
                'updated 5/4/2017 for CIM Golf properties
                'If _AddressInformation <> "" OrElse (_AggregateLimit <> "" AndAlso IsNumeric(_AggregateLimit) = True) OrElse (_AutomaticIncreasePercentTypeId <> "" AndAlso IsNumeric(_AutomaticIncreasePercentTypeId) = True) OrElse (_BusinessIncomeTypeId <> "" AndAlso IsNumeric(_BusinessIncomeTypeId) = True) OrElse (_BusinessPropertyTypeId <> "" AndAlso IsNumeric(_BusinessPropertyTypeId) = True) OrElse (_CauseOfLossTypeId <> "" AndAlso IsNumeric(_CauseOfLossTypeId) = True) OrElse (_CoinsuranceTypeId <> "" AndAlso IsNumeric(_CoinsuranceTypeId) = True) OrElse _Comments <> "" OrElse (_CommercialOccupancyTypeId <> "" AndAlso IsNumeric(_CommercialOccupancyTypeId) = True) OrElse (_ConstructionTypeId <> "" AndAlso IsNumeric(_ConstructionTypeId) = True) OrElse (_CoverageBasisTypeId <> "" AndAlso IsNumeric(_CoverageBasisTypeId) = True) OrElse (_CoverageTypeId <> "" AndAlso IsNumeric(_CoverageTypeId) = True) OrElse (_DailyReimbursement <> "" AndAlso IsNumeric(_DailyReimbursement) = True) OrElse (_DeductibleCategoryTypeId <> "" AndAlso IsNumeric(_DeductibleCategoryTypeId) = True) OrElse (_DeductiblePerTypeId <> "" AndAlso IsNumeric(_DeductiblePerTypeId) = True) OrElse (_DoesYardRateApplyTypeId <> "" AndAlso IsNumeric(_DoesYardRateApplyTypeId) = True) OrElse _ExcludeProductsCompletedOperations = True OrElse (_ExtendNonOwnershipRatingTypeId <> "" AndAlso IsNumeric(_ExtendNonOwnershipRatingTypeId) = True) OrElse (_FeetToFireHydrant <> "" AndAlso IsNumeric(_FeetToFireHydrant) = True) OrElse _HasWaiverOfSubrogation = True OrElse _IfAnyBasis = True OrElse (_InflationGuardTypeId <> "" AndAlso IsNumeric(_InflationGuardTypeId) = True) OrElse _IsBreakdownOrContamination = True OrElse _IsCondoCommercialUnitOwnersCoverage = True OrElse _IsEarthquakeApplies = True OrElse _IsIncludedInBlanketRating = True OrElse _IsMineSubsidence = True OrElse _IsPowerOutage = True OrElse _IsRefrigerationMaintenanceAgreement = True OrElse (_LossCost <> "" AndAlso IsNumeric(_LossCost) = True) OrElse _Make <> "" OrElse (_ManualElpaRate <> "" AndAlso IsNumeric(_ManualElpaRate) = True) OrElse _ManuallyRated = True OrElse (_ManualPremium <> "" AndAlso IsNumeric(_ManualPremium) = True) OrElse _ManufacturerName <> "" OrElse (_MilesToFireDepartment <> "" AndAlso IsNumeric(_MilesToFireDepartment) = True) OrElse _Model <> "" OrElse (_MonthlyPeriodTypeId <> "" AndAlso IsNumeric(_MonthlyPeriodTypeId) = True) OrElse (_NamedInsuredsBusinessTypeId <> "" AndAlso IsNumeric(_NamedInsuredsBusinessTypeId) = True) OrElse _NameInformation <> "" OrElse (_NumberOfBodies <> "" AndAlso IsNumeric(_NumberOfBodies) = True) OrElse (_NumberOfBurials <> "" AndAlso IsNumeric(_NumberOfBurials) = True) OrElse (_NumberOfContractors <> "" AndAlso IsNumeric(_NumberOfContractors) = True) OrElse (_NumberOfDays <> "" AndAlso IsNumeric(_NumberOfDays) = True) OrElse (_NumberOfEmployees <> "" AndAlso IsNumeric(_NumberOfEmployees) = True) OrElse (_NumberOfExcludedEmployees <> "" AndAlso IsNumeric(_NumberOfExcludedEmployees) = True) OrElse (_NumberOfIndependentContractors <> "" AndAlso IsNumeric(_NumberOfIndependentContractors) = True) OrElse (_NumberOfLocations <> "" AndAlso IsNumeric(_NumberOfLocations) = True) OrElse (_NumberOfPartTimeEmployees <> "" AndAlso IsNumeric(_NumberOfPartTimeEmployees) = True) OrElse (_NumberOfPastoralCounselors <> "" AndAlso IsNumeric(_NumberOfPastoralCounselors) = True) OrElse (_NumberOfPremises <> "" AndAlso IsNumeric(_NumberOfPremises) = True) OrElse (_NumberOfRatableEmployees <> "" AndAlso IsNumeric(_NumberOfRatableEmployees) = True) OrElse (_NumberOfSwimmingPools <> "" AndAlso IsNumeric(_NumberOfSwimmingPools) = True) OrElse (_NumberOfTemporaryWorkers <> "" AndAlso IsNumeric(_NumberOfTemporaryWorkers) = True) OrElse qqHelper.IsNumericString(_NumberOfAdditionalPremises) = True OrElse (_OtherThanCollisionSubTypeId <> "" AndAlso IsNumeric(_OtherThanCollisionSubTypeId) = True) OrElse (_OtherThanCollisionTypeId <> "" AndAlso IsNumeric(_OtherThanCollisionTypeId) = True) OrElse (_PercentOfWorkers <> "" AndAlso IsNumeric(_PercentOfWorkers) = True) OrElse (_PersonalPropertyRateGradeTypeId <> "" AndAlso IsNumeric(_PersonalPropertyRateGradeTypeId) = True) OrElse _ProductDescription <> "" OrElse (_PropertyClassificationTypeId <> "" AndAlso IsNumeric(_PropertyClassificationTypeId) = True) OrElse (_ProtectionClassId <> "" AndAlso IsNumeric(_ProtectionClassId) = True) OrElse (_Rate <> "" AndAlso IsNumeric(_Rate) = True) OrElse (_RatingTypeId <> "" AndAlso IsNumeric(_RatingTypeId) = True) OrElse (_RetroactiveDate <> "" AndAlso IsDate(_RetroactiveDate) = True) OrElse (_RiskTypeId <> "" AndAlso IsNumeric(_RiskTypeId) = True) OrElse (_SalesClubs <> "" AndAlso IsNumeric(_SalesClubs) = True) OrElse (_SalesManufacturers <> "" AndAlso IsNumeric(_SalesManufacturers) = True) OrElse (_SalesPackageStores <> "" AndAlso IsNumeric(_SalesPackageStores) = True) OrElse (_SalesRestaurants <> "" AndAlso IsNumeric(_SalesRestaurants) = True) OrElse _SerialNumber <> "" OrElse _ShouldSyncWithMasterCoverage = True OrElse (_SpecialClassCodeTypeId <> "" AndAlso IsNumeric(_SpecialClassCodeTypeId) = True) OrElse _VIN <> "" OrElse (_ValuationMethodTypeId <> "" AndAlso IsNumeric(_ValuationMethodTypeId) = True) OrElse (_WaitingPeriodTypeId <> "" AndAlso IsNumeric(_WaitingPeriodTypeId) = True) OrElse (_Year <> "" AndAlso IsNumeric(_Year) = True) OrElse _IsNamedPerils = True OrElse _ExcludeEarthquake = True OrElse _DescriptionOfOperations <> "" OrElse _DesignatedJobSite <> "" OrElse _HasLoadingUnloading = True OrElse qqHelper.IsNumericString(_NumberOfVehicles) = True OrElse qqHelper.IsNumericString(_NumberOfShipments) = True OrElse qqHelper.IsNumericString(_OperatingRadius) = True OrElse _City <> "" OrElse _ExcludedEggs = True OrElse _ExcludedFursOrFurTrimmedGarments = True OrElse _ExcludedLiquor = True OrElse _ExcludedLivestockOrPoultry = True OrElse _ExcludedTobacco = True OrElse qqHelper.IsNumericString(_StateId) = True OrElse qqHelper.IsNumericString(_WithinMiles) = True OrElse qqHelper.IsNumericString(_MaximumDeductible) = True OrElse qqHelper.IsNumericString(_MinimumDeductible) = True OrElse _IsIndoor = True OrElse qqHelper.IsNumericString(_FaithfulPerformanceOfDutyTypeId) = True OrElse qqHelper.IsNumericString(_LimitTypeId) = True OrElse qqHelper.IsNumericString(_EmployeeTheftScheduleTypeId) = True OrElse qqHelper.IsNumericString(_NumberOfCardHolders) = True OrElse qqHelper.IsNumericString(_PremiumChargeTypeId) = True OrElse qqHelper.IsNumericString(_BasisTypeId) = True OrElse _IncludeSellingPrice = True OrElse _IsIncludeGuestsProperty = True OrElse _IsLimitToRobberyOnly = True OrElse _IsRequireRecordOfChecks = True OrElse qqHelper.IsDateString(_FromDate) = True OrElse qqHelper.IsDateString(_ToDate) = True OrElse _CoverageDescription <> "" OrElse _IsEmployeeTools = True OrElse _IsToolsLeasedOrRented = True OrElse _IsLimitedPerilsExtendedCoverage = True OrElse _IsAgreedValue = True OrElse String.IsNullOrEmpty(_DescriptionInformation) = False OrElse qqHelper.IsPositiveIntegerString(_LiquorLiabilityClassCodeTypeId) = True OrElse qqHelper.IsPositiveIntegerString(_NumberOfAmusementAreas) = True OrElse qqHelper.IsPositiveIntegerString(_Receipts) = True OrElse qqHelper.IsPositiveIntegerString(_PriorGrossReceipts) = True OrElse qqHelper.IsPositiveIntegerString(_CoveredHolesFairways) = True OrElse qqHelper.IsPositiveIntegerString(_CoveredHolesGreens) = True OrElse qqHelper.IsPositiveIntegerString(_CoveredHolesTees) = True OrElse qqHelper.IsPositiveIntegerString(_CoveredHolesTrees) = True OrElse _IsFairways = True OrElse _IsGreens = True OrElse _IsTees = True OrElse _IsTrees = True OrElse qqHelper.IsPositiveIntegerString(_NumberOfCarts) = True Then 'may not use _ShouldSyncWithMasterCoverage
                'updated 5/4/2017 again since CoveredHoles props are actually strings instead of int (CIM Golf)
                'If _AddressInformation <> "" OrElse (_AggregateLimit <> "" AndAlso IsNumeric(_AggregateLimit) = True) OrElse (_AutomaticIncreasePercentTypeId <> "" AndAlso IsNumeric(_AutomaticIncreasePercentTypeId) = True) OrElse (_BusinessIncomeTypeId <> "" AndAlso IsNumeric(_BusinessIncomeTypeId) = True) OrElse (_BusinessPropertyTypeId <> "" AndAlso IsNumeric(_BusinessPropertyTypeId) = True) OrElse (_CauseOfLossTypeId <> "" AndAlso IsNumeric(_CauseOfLossTypeId) = True) OrElse (_CoinsuranceTypeId <> "" AndAlso IsNumeric(_CoinsuranceTypeId) = True) OrElse _Comments <> "" OrElse (_CommercialOccupancyTypeId <> "" AndAlso IsNumeric(_CommercialOccupancyTypeId) = True) OrElse (_ConstructionTypeId <> "" AndAlso IsNumeric(_ConstructionTypeId) = True) OrElse (_CoverageBasisTypeId <> "" AndAlso IsNumeric(_CoverageBasisTypeId) = True) OrElse (_CoverageTypeId <> "" AndAlso IsNumeric(_CoverageTypeId) = True) OrElse (_DailyReimbursement <> "" AndAlso IsNumeric(_DailyReimbursement) = True) OrElse (_DeductibleCategoryTypeId <> "" AndAlso IsNumeric(_DeductibleCategoryTypeId) = True) OrElse (_DeductiblePerTypeId <> "" AndAlso IsNumeric(_DeductiblePerTypeId) = True) OrElse (_DoesYardRateApplyTypeId <> "" AndAlso IsNumeric(_DoesYardRateApplyTypeId) = True) OrElse _ExcludeProductsCompletedOperations = True OrElse (_ExtendNonOwnershipRatingTypeId <> "" AndAlso IsNumeric(_ExtendNonOwnershipRatingTypeId) = True) OrElse (_FeetToFireHydrant <> "" AndAlso IsNumeric(_FeetToFireHydrant) = True) OrElse _HasWaiverOfSubrogation = True OrElse _IfAnyBasis = True OrElse (_InflationGuardTypeId <> "" AndAlso IsNumeric(_InflationGuardTypeId) = True) OrElse _IsBreakdownOrContamination = True OrElse _IsCondoCommercialUnitOwnersCoverage = True OrElse _IsEarthquakeApplies = True OrElse _IsIncludedInBlanketRating = True OrElse _IsMineSubsidence = True OrElse _IsPowerOutage = True OrElse _IsRefrigerationMaintenanceAgreement = True OrElse (_LossCost <> "" AndAlso IsNumeric(_LossCost) = True) OrElse _Make <> "" OrElse (_ManualElpaRate <> "" AndAlso IsNumeric(_ManualElpaRate) = True) OrElse _ManuallyRated = True OrElse (_ManualPremium <> "" AndAlso IsNumeric(_ManualPremium) = True) OrElse _ManufacturerName <> "" OrElse (_MilesToFireDepartment <> "" AndAlso IsNumeric(_MilesToFireDepartment) = True) OrElse _Model <> "" OrElse (_MonthlyPeriodTypeId <> "" AndAlso IsNumeric(_MonthlyPeriodTypeId) = True) OrElse (_NamedInsuredsBusinessTypeId <> "" AndAlso IsNumeric(_NamedInsuredsBusinessTypeId) = True) OrElse _NameInformation <> "" OrElse (_NumberOfBodies <> "" AndAlso IsNumeric(_NumberOfBodies) = True) OrElse (_NumberOfBurials <> "" AndAlso IsNumeric(_NumberOfBurials) = True) OrElse (_NumberOfContractors <> "" AndAlso IsNumeric(_NumberOfContractors) = True) OrElse (_NumberOfDays <> "" AndAlso IsNumeric(_NumberOfDays) = True) OrElse (_NumberOfEmployees <> "" AndAlso IsNumeric(_NumberOfEmployees) = True) OrElse (_NumberOfExcludedEmployees <> "" AndAlso IsNumeric(_NumberOfExcludedEmployees) = True) OrElse (_NumberOfIndependentContractors <> "" AndAlso IsNumeric(_NumberOfIndependentContractors) = True) OrElse (_NumberOfLocations <> "" AndAlso IsNumeric(_NumberOfLocations) = True) OrElse (_NumberOfPartTimeEmployees <> "" AndAlso IsNumeric(_NumberOfPartTimeEmployees) = True) OrElse (_NumberOfPastoralCounselors <> "" AndAlso IsNumeric(_NumberOfPastoralCounselors) = True) OrElse (_NumberOfPremises <> "" AndAlso IsNumeric(_NumberOfPremises) = True) OrElse (_NumberOfRatableEmployees <> "" AndAlso IsNumeric(_NumberOfRatableEmployees) = True) OrElse (_NumberOfSwimmingPools <> "" AndAlso IsNumeric(_NumberOfSwimmingPools) = True) OrElse (_NumberOfTemporaryWorkers <> "" AndAlso IsNumeric(_NumberOfTemporaryWorkers) = True) OrElse qqHelper.IsNumericString(_NumberOfAdditionalPremises) = True OrElse (_OtherThanCollisionSubTypeId <> "" AndAlso IsNumeric(_OtherThanCollisionSubTypeId) = True) OrElse (_OtherThanCollisionTypeId <> "" AndAlso IsNumeric(_OtherThanCollisionTypeId) = True) OrElse (_PercentOfWorkers <> "" AndAlso IsNumeric(_PercentOfWorkers) = True) OrElse (_PersonalPropertyRateGradeTypeId <> "" AndAlso IsNumeric(_PersonalPropertyRateGradeTypeId) = True) OrElse _ProductDescription <> "" OrElse (_PropertyClassificationTypeId <> "" AndAlso IsNumeric(_PropertyClassificationTypeId) = True) OrElse (_ProtectionClassId <> "" AndAlso IsNumeric(_ProtectionClassId) = True) OrElse (_Rate <> "" AndAlso IsNumeric(_Rate) = True) OrElse (_RatingTypeId <> "" AndAlso IsNumeric(_RatingTypeId) = True) OrElse (_RetroactiveDate <> "" AndAlso IsDate(_RetroactiveDate) = True) OrElse (_RiskTypeId <> "" AndAlso IsNumeric(_RiskTypeId) = True) OrElse (_SalesClubs <> "" AndAlso IsNumeric(_SalesClubs) = True) OrElse (_SalesManufacturers <> "" AndAlso IsNumeric(_SalesManufacturers) = True) OrElse (_SalesPackageStores <> "" AndAlso IsNumeric(_SalesPackageStores) = True) OrElse (_SalesRestaurants <> "" AndAlso IsNumeric(_SalesRestaurants) = True) OrElse _SerialNumber <> "" OrElse _ShouldSyncWithMasterCoverage = True OrElse (_SpecialClassCodeTypeId <> "" AndAlso IsNumeric(_SpecialClassCodeTypeId) = True) OrElse _VIN <> "" OrElse (_ValuationMethodTypeId <> "" AndAlso IsNumeric(_ValuationMethodTypeId) = True) OrElse (_WaitingPeriodTypeId <> "" AndAlso IsNumeric(_WaitingPeriodTypeId) = True) OrElse (_Year <> "" AndAlso IsNumeric(_Year) = True) OrElse _IsNamedPerils = True OrElse _ExcludeEarthquake = True OrElse _DescriptionOfOperations <> "" OrElse _DesignatedJobSite <> "" OrElse _HasLoadingUnloading = True OrElse qqHelper.IsNumericString(_NumberOfVehicles) = True OrElse qqHelper.IsNumericString(_NumberOfShipments) = True OrElse qqHelper.IsNumericString(_OperatingRadius) = True OrElse _City <> "" OrElse _ExcludedEggs = True OrElse _ExcludedFursOrFurTrimmedGarments = True OrElse _ExcludedLiquor = True OrElse _ExcludedLivestockOrPoultry = True OrElse _ExcludedTobacco = True OrElse qqHelper.IsNumericString(_StateId) = True OrElse qqHelper.IsNumericString(_WithinMiles) = True OrElse qqHelper.IsNumericString(_MaximumDeductible) = True OrElse qqHelper.IsNumericString(_MinimumDeductible) = True OrElse _IsIndoor = True OrElse qqHelper.IsNumericString(_FaithfulPerformanceOfDutyTypeId) = True OrElse qqHelper.IsNumericString(_LimitTypeId) = True OrElse qqHelper.IsNumericString(_EmployeeTheftScheduleTypeId) = True OrElse qqHelper.IsNumericString(_NumberOfCardHolders) = True OrElse qqHelper.IsNumericString(_PremiumChargeTypeId) = True OrElse qqHelper.IsNumericString(_BasisTypeId) = True OrElse _IncludeSellingPrice = True OrElse _IsIncludeGuestsProperty = True OrElse _IsLimitToRobberyOnly = True OrElse _IsRequireRecordOfChecks = True OrElse qqHelper.IsDateString(_FromDate) = True OrElse qqHelper.IsDateString(_ToDate) = True OrElse _CoverageDescription <> "" OrElse _IsEmployeeTools = True OrElse _IsToolsLeasedOrRented = True OrElse _IsLimitedPerilsExtendedCoverage = True OrElse _IsAgreedValue = True OrElse String.IsNullOrEmpty(_DescriptionInformation) = False OrElse qqHelper.IsPositiveIntegerString(_LiquorLiabilityClassCodeTypeId) = True OrElse qqHelper.IsPositiveIntegerString(_NumberOfAmusementAreas) = True OrElse qqHelper.IsPositiveIntegerString(_Receipts) = True OrElse qqHelper.IsPositiveIntegerString(_PriorGrossReceipts) = True OrElse String.IsNullOrWhiteSpace(_CoveredHolesFairways) = False OrElse String.IsNullOrWhiteSpace(_CoveredHolesGreens) = False OrElse String.IsNullOrWhiteSpace(_CoveredHolesTees) = False OrElse String.IsNullOrWhiteSpace(_CoveredHolesTrees) = False OrElse _IsFairways = True OrElse _IsGreens = True OrElse _IsTees = True OrElse _IsTrees = True OrElse qqHelper.IsPositiveIntegerString(_NumberOfCarts) = True Then 'may not use _ShouldSyncWithMasterCoverage
                'updated for new GAR props
                'If _AddressInformation <> "" OrElse (_AggregateLimit <> "" AndAlso IsNumeric(_AggregateLimit) = True) OrElse (_AutomaticIncreasePercentTypeId <> "" AndAlso IsNumeric(_AutomaticIncreasePercentTypeId) = True) OrElse (_BusinessIncomeTypeId <> "" AndAlso IsNumeric(_BusinessIncomeTypeId) = True) OrElse (_BusinessPropertyTypeId <> "" AndAlso IsNumeric(_BusinessPropertyTypeId) = True) OrElse (_CauseOfLossTypeId <> "" AndAlso IsNumeric(_CauseOfLossTypeId) = True) OrElse (_CoinsuranceTypeId <> "" AndAlso IsNumeric(_CoinsuranceTypeId) = True) OrElse _Comments <> "" OrElse (_CommercialOccupancyTypeId <> "" AndAlso IsNumeric(_CommercialOccupancyTypeId) = True) OrElse (_ConstructionTypeId <> "" AndAlso IsNumeric(_ConstructionTypeId) = True) OrElse (_CoverageBasisTypeId <> "" AndAlso IsNumeric(_CoverageBasisTypeId) = True) OrElse (_CoverageTypeId <> "" AndAlso IsNumeric(_CoverageTypeId) = True) OrElse (_DailyReimbursement <> "" AndAlso IsNumeric(_DailyReimbursement) = True) OrElse (_DeductibleCategoryTypeId <> "" AndAlso IsNumeric(_DeductibleCategoryTypeId) = True) OrElse (_DeductiblePerTypeId <> "" AndAlso IsNumeric(_DeductiblePerTypeId) = True) OrElse (_DoesYardRateApplyTypeId <> "" AndAlso IsNumeric(_DoesYardRateApplyTypeId) = True) OrElse _ExcludeProductsCompletedOperations = True OrElse (_ExtendNonOwnershipRatingTypeId <> "" AndAlso IsNumeric(_ExtendNonOwnershipRatingTypeId) = True) OrElse (_FeetToFireHydrant <> "" AndAlso IsNumeric(_FeetToFireHydrant) = True) OrElse _HasWaiverOfSubrogation = True OrElse _IfAnyBasis = True OrElse (_InflationGuardTypeId <> "" AndAlso IsNumeric(_InflationGuardTypeId) = True) OrElse _IsBreakdownOrContamination = True OrElse _IsCondoCommercialUnitOwnersCoverage = True OrElse _IsEarthquakeApplies = True OrElse _IsIncludedInBlanketRating = True OrElse _IsMineSubsidence = True OrElse _IsPowerOutage = True OrElse _IsRefrigerationMaintenanceAgreement = True OrElse (_LossCost <> "" AndAlso IsNumeric(_LossCost) = True) OrElse _Make <> "" OrElse (_ManualElpaRate <> "" AndAlso IsNumeric(_ManualElpaRate) = True) OrElse _ManuallyRated = True OrElse (_ManualPremium <> "" AndAlso IsNumeric(_ManualPremium) = True) OrElse _ManufacturerName <> "" OrElse (_MilesToFireDepartment <> "" AndAlso IsNumeric(_MilesToFireDepartment) = True) OrElse _Model <> "" OrElse (_MonthlyPeriodTypeId <> "" AndAlso IsNumeric(_MonthlyPeriodTypeId) = True) OrElse (_NamedInsuredsBusinessTypeId <> "" AndAlso IsNumeric(_NamedInsuredsBusinessTypeId) = True) OrElse _NameInformation <> "" OrElse (_NumberOfBodies <> "" AndAlso IsNumeric(_NumberOfBodies) = True) OrElse (_NumberOfBurials <> "" AndAlso IsNumeric(_NumberOfBurials) = True) OrElse (_NumberOfContractors <> "" AndAlso IsNumeric(_NumberOfContractors) = True) OrElse (_NumberOfDays <> "" AndAlso IsNumeric(_NumberOfDays) = True) OrElse (_NumberOfEmployees <> "" AndAlso IsNumeric(_NumberOfEmployees) = True) OrElse (_NumberOfExcludedEmployees <> "" AndAlso IsNumeric(_NumberOfExcludedEmployees) = True) OrElse (_NumberOfIndependentContractors <> "" AndAlso IsNumeric(_NumberOfIndependentContractors) = True) OrElse (_NumberOfLocations <> "" AndAlso IsNumeric(_NumberOfLocations) = True) OrElse (_NumberOfPartTimeEmployees <> "" AndAlso IsNumeric(_NumberOfPartTimeEmployees) = True) OrElse (_NumberOfPastoralCounselors <> "" AndAlso IsNumeric(_NumberOfPastoralCounselors) = True) OrElse (_NumberOfPremises <> "" AndAlso IsNumeric(_NumberOfPremises) = True) OrElse (_NumberOfRatableEmployees <> "" AndAlso IsNumeric(_NumberOfRatableEmployees) = True) OrElse (_NumberOfSwimmingPools <> "" AndAlso IsNumeric(_NumberOfSwimmingPools) = True) OrElse (_NumberOfTemporaryWorkers <> "" AndAlso IsNumeric(_NumberOfTemporaryWorkers) = True) OrElse qqHelper.IsNumericString(_NumberOfAdditionalPremises) = True OrElse (_OtherThanCollisionSubTypeId <> "" AndAlso IsNumeric(_OtherThanCollisionSubTypeId) = True) OrElse (_OtherThanCollisionTypeId <> "" AndAlso IsNumeric(_OtherThanCollisionTypeId) = True) OrElse (_PercentOfWorkers <> "" AndAlso IsNumeric(_PercentOfWorkers) = True) OrElse (_PersonalPropertyRateGradeTypeId <> "" AndAlso IsNumeric(_PersonalPropertyRateGradeTypeId) = True) OrElse _ProductDescription <> "" OrElse (_PropertyClassificationTypeId <> "" AndAlso IsNumeric(_PropertyClassificationTypeId) = True) OrElse (_ProtectionClassId <> "" AndAlso IsNumeric(_ProtectionClassId) = True) OrElse (_Rate <> "" AndAlso IsNumeric(_Rate) = True) OrElse (_RatingTypeId <> "" AndAlso IsNumeric(_RatingTypeId) = True) OrElse (_RetroactiveDate <> "" AndAlso IsDate(_RetroactiveDate) = True) OrElse (_RiskTypeId <> "" AndAlso IsNumeric(_RiskTypeId) = True) OrElse (_SalesClubs <> "" AndAlso IsNumeric(_SalesClubs) = True) OrElse (_SalesManufacturers <> "" AndAlso IsNumeric(_SalesManufacturers) = True) OrElse (_SalesPackageStores <> "" AndAlso IsNumeric(_SalesPackageStores) = True) OrElse (_SalesRestaurants <> "" AndAlso IsNumeric(_SalesRestaurants) = True) OrElse _SerialNumber <> "" OrElse _ShouldSyncWithMasterCoverage = True OrElse (_SpecialClassCodeTypeId <> "" AndAlso IsNumeric(_SpecialClassCodeTypeId) = True) OrElse _VIN <> "" OrElse (_ValuationMethodTypeId <> "" AndAlso IsNumeric(_ValuationMethodTypeId) = True) OrElse (_WaitingPeriodTypeId <> "" AndAlso IsNumeric(_WaitingPeriodTypeId) = True) OrElse (_Year <> "" AndAlso IsNumeric(_Year) = True) OrElse _IsNamedPerils = True OrElse _ExcludeEarthquake = True OrElse _DescriptionOfOperations <> "" OrElse _DesignatedJobSite <> "" OrElse _HasLoadingUnloading = True OrElse qqHelper.IsNumericString(_NumberOfVehicles) = True OrElse qqHelper.IsNumericString(_NumberOfShipments) = True OrElse qqHelper.IsNumericString(_OperatingRadius) = True OrElse _City <> "" OrElse _ExcludedEggs = True OrElse _ExcludedFursOrFurTrimmedGarments = True OrElse _ExcludedLiquor = True OrElse _ExcludedLivestockOrPoultry = True OrElse _ExcludedTobacco = True OrElse qqHelper.IsNumericString(_StateId) = True OrElse qqHelper.IsNumericString(_WithinMiles) = True OrElse qqHelper.IsNumericString(_MaximumDeductible) = True OrElse qqHelper.IsNumericString(_MinimumDeductible) = True OrElse _IsIndoor = True OrElse qqHelper.IsNumericString(_FaithfulPerformanceOfDutyTypeId) = True OrElse qqHelper.IsNumericString(_LimitTypeId) = True OrElse qqHelper.IsNumericString(_EmployeeTheftScheduleTypeId) = True OrElse qqHelper.IsNumericString(_NumberOfCardHolders) = True OrElse qqHelper.IsNumericString(_PremiumChargeTypeId) = True OrElse qqHelper.IsNumericString(_BasisTypeId) = True OrElse _IncludeSellingPrice = True OrElse _IsIncludeGuestsProperty = True OrElse _IsLimitToRobberyOnly = True OrElse _IsRequireRecordOfChecks = True OrElse qqHelper.IsDateString(_FromDate) = True OrElse qqHelper.IsDateString(_ToDate) = True OrElse _CoverageDescription <> "" OrElse _IsEmployeeTools = True OrElse _IsToolsLeasedOrRented = True OrElse _IsLimitedPerilsExtendedCoverage = True OrElse _IsAgreedValue = True OrElse String.IsNullOrEmpty(_DescriptionInformation) = False OrElse qqHelper.IsPositiveIntegerString(_LiquorLiabilityClassCodeTypeId) = True OrElse qqHelper.IsPositiveIntegerString(_NumberOfAmusementAreas) = True OrElse qqHelper.IsPositiveIntegerString(_Receipts) = True OrElse qqHelper.IsPositiveIntegerString(_PriorGrossReceipts) = True OrElse String.IsNullOrWhiteSpace(_CoveredHolesFairways) = False OrElse String.IsNullOrWhiteSpace(_CoveredHolesGreens) = False OrElse String.IsNullOrWhiteSpace(_CoveredHolesTees) = False OrElse String.IsNullOrWhiteSpace(_CoveredHolesTrees) = False OrElse _IsFairways = True OrElse _IsGreens = True OrElse _IsTees = True OrElse _IsTrees = True OrElse qqHelper.IsPositiveIntegerString(_NumberOfCarts) = True OrElse qqHelper.IsPositiveIntegerString(_AggregateLiabilityIncrementTypeId) = True OrElse qqHelper.IsPositiveIntegerString(_MedicalPaymentsTypeId) = True Then 'may not use _ShouldSyncWithMasterCoverage
                'updated 5/11/2017 for NumberOfPlates (GAR)
                'If _AddressInformation <> "" OrElse (_AggregateLimit <> "" AndAlso IsNumeric(_AggregateLimit) = True) OrElse (_AutomaticIncreasePercentTypeId <> "" AndAlso IsNumeric(_AutomaticIncreasePercentTypeId) = True) OrElse (_BusinessIncomeTypeId <> "" AndAlso IsNumeric(_BusinessIncomeTypeId) = True) OrElse (_BusinessPropertyTypeId <> "" AndAlso IsNumeric(_BusinessPropertyTypeId) = True) OrElse (_CauseOfLossTypeId <> "" AndAlso IsNumeric(_CauseOfLossTypeId) = True) OrElse (_CoinsuranceTypeId <> "" AndAlso IsNumeric(_CoinsuranceTypeId) = True) OrElse _Comments <> "" OrElse (_CommercialOccupancyTypeId <> "" AndAlso IsNumeric(_CommercialOccupancyTypeId) = True) OrElse (_ConstructionTypeId <> "" AndAlso IsNumeric(_ConstructionTypeId) = True) OrElse (_CoverageBasisTypeId <> "" AndAlso IsNumeric(_CoverageBasisTypeId) = True) OrElse (_CoverageTypeId <> "" AndAlso IsNumeric(_CoverageTypeId) = True) OrElse (_DailyReimbursement <> "" AndAlso IsNumeric(_DailyReimbursement) = True) OrElse (_DeductibleCategoryTypeId <> "" AndAlso IsNumeric(_DeductibleCategoryTypeId) = True) OrElse (_DeductiblePerTypeId <> "" AndAlso IsNumeric(_DeductiblePerTypeId) = True) OrElse (_DoesYardRateApplyTypeId <> "" AndAlso IsNumeric(_DoesYardRateApplyTypeId) = True) OrElse _ExcludeProductsCompletedOperations = True OrElse (_ExtendNonOwnershipRatingTypeId <> "" AndAlso IsNumeric(_ExtendNonOwnershipRatingTypeId) = True) OrElse (_FeetToFireHydrant <> "" AndAlso IsNumeric(_FeetToFireHydrant) = True) OrElse _HasWaiverOfSubrogation = True OrElse _IfAnyBasis = True OrElse (_InflationGuardTypeId <> "" AndAlso IsNumeric(_InflationGuardTypeId) = True) OrElse _IsBreakdownOrContamination = True OrElse _IsCondoCommercialUnitOwnersCoverage = True OrElse _IsEarthquakeApplies = True OrElse _IsIncludedInBlanketRating = True OrElse _IsMineSubsidence = True OrElse _IsPowerOutage = True OrElse _IsRefrigerationMaintenanceAgreement = True OrElse (_LossCost <> "" AndAlso IsNumeric(_LossCost) = True) OrElse _Make <> "" OrElse (_ManualElpaRate <> "" AndAlso IsNumeric(_ManualElpaRate) = True) OrElse _ManuallyRated = True OrElse (_ManualPremium <> "" AndAlso IsNumeric(_ManualPremium) = True) OrElse _ManufacturerName <> "" OrElse (_MilesToFireDepartment <> "" AndAlso IsNumeric(_MilesToFireDepartment) = True) OrElse _Model <> "" OrElse (_MonthlyPeriodTypeId <> "" AndAlso IsNumeric(_MonthlyPeriodTypeId) = True) OrElse (_NamedInsuredsBusinessTypeId <> "" AndAlso IsNumeric(_NamedInsuredsBusinessTypeId) = True) OrElse _NameInformation <> "" OrElse (_NumberOfBodies <> "" AndAlso IsNumeric(_NumberOfBodies) = True) OrElse (_NumberOfBurials <> "" AndAlso IsNumeric(_NumberOfBurials) = True) OrElse (_NumberOfContractors <> "" AndAlso IsNumeric(_NumberOfContractors) = True) OrElse (_NumberOfDays <> "" AndAlso IsNumeric(_NumberOfDays) = True) OrElse (_NumberOfEmployees <> "" AndAlso IsNumeric(_NumberOfEmployees) = True) OrElse (_NumberOfExcludedEmployees <> "" AndAlso IsNumeric(_NumberOfExcludedEmployees) = True) OrElse (_NumberOfIndependentContractors <> "" AndAlso IsNumeric(_NumberOfIndependentContractors) = True) OrElse (_NumberOfLocations <> "" AndAlso IsNumeric(_NumberOfLocations) = True) OrElse (_NumberOfPartTimeEmployees <> "" AndAlso IsNumeric(_NumberOfPartTimeEmployees) = True) OrElse (_NumberOfPastoralCounselors <> "" AndAlso IsNumeric(_NumberOfPastoralCounselors) = True) OrElse (_NumberOfPremises <> "" AndAlso IsNumeric(_NumberOfPremises) = True) OrElse (_NumberOfRatableEmployees <> "" AndAlso IsNumeric(_NumberOfRatableEmployees) = True) OrElse (_NumberOfSwimmingPools <> "" AndAlso IsNumeric(_NumberOfSwimmingPools) = True) OrElse (_NumberOfTemporaryWorkers <> "" AndAlso IsNumeric(_NumberOfTemporaryWorkers) = True) OrElse qqHelper.IsNumericString(_NumberOfAdditionalPremises) = True OrElse (_OtherThanCollisionSubTypeId <> "" AndAlso IsNumeric(_OtherThanCollisionSubTypeId) = True) OrElse (_OtherThanCollisionTypeId <> "" AndAlso IsNumeric(_OtherThanCollisionTypeId) = True) OrElse (_PercentOfWorkers <> "" AndAlso IsNumeric(_PercentOfWorkers) = True) OrElse (_PersonalPropertyRateGradeTypeId <> "" AndAlso IsNumeric(_PersonalPropertyRateGradeTypeId) = True) OrElse _ProductDescription <> "" OrElse (_PropertyClassificationTypeId <> "" AndAlso IsNumeric(_PropertyClassificationTypeId) = True) OrElse (_ProtectionClassId <> "" AndAlso IsNumeric(_ProtectionClassId) = True) OrElse (_Rate <> "" AndAlso IsNumeric(_Rate) = True) OrElse (_RatingTypeId <> "" AndAlso IsNumeric(_RatingTypeId) = True) OrElse (_RetroactiveDate <> "" AndAlso IsDate(_RetroactiveDate) = True) OrElse (_RiskTypeId <> "" AndAlso IsNumeric(_RiskTypeId) = True) OrElse (_SalesClubs <> "" AndAlso IsNumeric(_SalesClubs) = True) OrElse (_SalesManufacturers <> "" AndAlso IsNumeric(_SalesManufacturers) = True) OrElse (_SalesPackageStores <> "" AndAlso IsNumeric(_SalesPackageStores) = True) OrElse (_SalesRestaurants <> "" AndAlso IsNumeric(_SalesRestaurants) = True) OrElse _SerialNumber <> "" OrElse _ShouldSyncWithMasterCoverage = True OrElse (_SpecialClassCodeTypeId <> "" AndAlso IsNumeric(_SpecialClassCodeTypeId) = True) OrElse _VIN <> "" OrElse (_ValuationMethodTypeId <> "" AndAlso IsNumeric(_ValuationMethodTypeId) = True) OrElse (_WaitingPeriodTypeId <> "" AndAlso IsNumeric(_WaitingPeriodTypeId) = True) OrElse (_Year <> "" AndAlso IsNumeric(_Year) = True) OrElse _IsNamedPerils = True OrElse _ExcludeEarthquake = True OrElse _DescriptionOfOperations <> "" OrElse _DesignatedJobSite <> "" OrElse _HasLoadingUnloading = True OrElse qqHelper.IsNumericString(_NumberOfVehicles) = True OrElse qqHelper.IsNumericString(_NumberOfShipments) = True OrElse qqHelper.IsNumericString(_OperatingRadius) = True OrElse _City <> "" OrElse _ExcludedEggs = True OrElse _ExcludedFursOrFurTrimmedGarments = True OrElse _ExcludedLiquor = True OrElse _ExcludedLivestockOrPoultry = True OrElse _ExcludedTobacco = True OrElse qqHelper.IsNumericString(_StateId) = True OrElse qqHelper.IsNumericString(_WithinMiles) = True OrElse qqHelper.IsNumericString(_MaximumDeductible) = True OrElse qqHelper.IsNumericString(_MinimumDeductible) = True OrElse _IsIndoor = True OrElse qqHelper.IsNumericString(_FaithfulPerformanceOfDutyTypeId) = True OrElse qqHelper.IsNumericString(_LimitTypeId) = True OrElse qqHelper.IsNumericString(_EmployeeTheftScheduleTypeId) = True OrElse qqHelper.IsNumericString(_NumberOfCardHolders) = True OrElse qqHelper.IsNumericString(_PremiumChargeTypeId) = True OrElse qqHelper.IsNumericString(_BasisTypeId) = True OrElse _IncludeSellingPrice = True OrElse _IsIncludeGuestsProperty = True OrElse _IsLimitToRobberyOnly = True OrElse _IsRequireRecordOfChecks = True OrElse qqHelper.IsDateString(_FromDate) = True OrElse qqHelper.IsDateString(_ToDate) = True OrElse _CoverageDescription <> "" OrElse _IsEmployeeTools = True OrElse _IsToolsLeasedOrRented = True OrElse _IsLimitedPerilsExtendedCoverage = True OrElse _IsAgreedValue = True OrElse String.IsNullOrEmpty(_DescriptionInformation) = False OrElse qqHelper.IsPositiveIntegerString(_LiquorLiabilityClassCodeTypeId) = True OrElse qqHelper.IsPositiveIntegerString(_NumberOfAmusementAreas) = True OrElse qqHelper.IsPositiveIntegerString(_Receipts) = True OrElse qqHelper.IsPositiveIntegerString(_PriorGrossReceipts) = True OrElse String.IsNullOrWhiteSpace(_CoveredHolesFairways) = False OrElse String.IsNullOrWhiteSpace(_CoveredHolesGreens) = False OrElse String.IsNullOrWhiteSpace(_CoveredHolesTees) = False OrElse String.IsNullOrWhiteSpace(_CoveredHolesTrees) = False OrElse _IsFairways = True OrElse _IsGreens = True OrElse _IsTees = True OrElse _IsTrees = True OrElse qqHelper.IsPositiveIntegerString(_NumberOfCarts) = True OrElse qqHelper.IsPositiveIntegerString(_AggregateLiabilityIncrementTypeId) = True OrElse qqHelper.IsPositiveIntegerString(_MedicalPaymentsTypeId) = True OrElse qqHelper.IsPositiveIntegerString(_NumberOfPlates) = True Then 'may not use _ShouldSyncWithMasterCoverage
                'updated 11/8/2018 for IsDwellingStructure
                If _AddressInformation <> "" OrElse (_AggregateLimit <> "" AndAlso IsNumeric(_AggregateLimit) = True) OrElse (_AutomaticIncreasePercentTypeId <> "" AndAlso IsNumeric(_AutomaticIncreasePercentTypeId) = True) OrElse (_BusinessIncomeTypeId <> "" AndAlso IsNumeric(_BusinessIncomeTypeId) = True) OrElse (_BusinessPropertyTypeId <> "" AndAlso IsNumeric(_BusinessPropertyTypeId) = True) OrElse (_CauseOfLossTypeId <> "" AndAlso IsNumeric(_CauseOfLossTypeId) = True) OrElse (_CoinsuranceTypeId <> "" AndAlso IsNumeric(_CoinsuranceTypeId) = True) OrElse _Comments <> "" OrElse (_CommercialOccupancyTypeId <> "" AndAlso IsNumeric(_CommercialOccupancyTypeId) = True) OrElse (_ConstructionTypeId <> "" AndAlso IsNumeric(_ConstructionTypeId) = True) OrElse (_CoverageBasisTypeId <> "" AndAlso IsNumeric(_CoverageBasisTypeId) = True) OrElse (_CoverageTypeId <> "" AndAlso IsNumeric(_CoverageTypeId) = True) OrElse (_DailyReimbursement <> "" AndAlso IsNumeric(_DailyReimbursement) = True) OrElse (_DeductibleCategoryTypeId <> "" AndAlso IsNumeric(_DeductibleCategoryTypeId) = True) OrElse (_DeductiblePerTypeId <> "" AndAlso IsNumeric(_DeductiblePerTypeId) = True) OrElse (_DoesYardRateApplyTypeId <> "" AndAlso IsNumeric(_DoesYardRateApplyTypeId) = True) OrElse _ExcludeProductsCompletedOperations = True OrElse (_ExtendNonOwnershipRatingTypeId <> "" AndAlso IsNumeric(_ExtendNonOwnershipRatingTypeId) = True) OrElse (_FeetToFireHydrant <> "" AndAlso IsNumeric(_FeetToFireHydrant) = True) OrElse _HasWaiverOfSubrogation = True OrElse _IfAnyBasis = True OrElse (_InflationGuardTypeId <> "" AndAlso IsNumeric(_InflationGuardTypeId) = True) OrElse _IsBreakdownOrContamination = True OrElse _IsCondoCommercialUnitOwnersCoverage = True OrElse _IsEarthquakeApplies = True OrElse _IsIncludedInBlanketRating = True OrElse _IsMineSubsidence = True OrElse _IsPowerOutage = True OrElse _IsRefrigerationMaintenanceAgreement = True OrElse (_LossCost <> "" AndAlso IsNumeric(_LossCost) = True) OrElse _Make <> "" OrElse (_ManualElpaRate <> "" AndAlso IsNumeric(_ManualElpaRate) = True) OrElse _ManuallyRated = True OrElse (_ManualPremium <> "" AndAlso IsNumeric(_ManualPremium) = True) OrElse _ManufacturerName <> "" OrElse (_MilesToFireDepartment <> "" AndAlso IsNumeric(_MilesToFireDepartment) = True) OrElse _Model <> "" OrElse (_MonthlyPeriodTypeId <> "" AndAlso IsNumeric(_MonthlyPeriodTypeId) = True) OrElse (_NamedInsuredsBusinessTypeId <> "" AndAlso IsNumeric(_NamedInsuredsBusinessTypeId) = True) OrElse _NameInformation <> "" OrElse (_NumberOfBodies <> "" AndAlso IsNumeric(_NumberOfBodies) = True) OrElse (_NumberOfBurials <> "" AndAlso IsNumeric(_NumberOfBurials) = True) OrElse (_NumberOfContractors <> "" AndAlso IsNumeric(_NumberOfContractors) = True) OrElse (_NumberOfDays <> "" AndAlso IsNumeric(_NumberOfDays) = True) OrElse (_NumberOfEmployees <> "" AndAlso IsNumeric(_NumberOfEmployees) = True) OrElse (_NumberOfExcludedEmployees <> "" AndAlso IsNumeric(_NumberOfExcludedEmployees) = True) OrElse (_NumberOfIndependentContractors <> "" AndAlso IsNumeric(_NumberOfIndependentContractors) = True) OrElse (_NumberOfLocations <> "" AndAlso IsNumeric(_NumberOfLocations) = True) OrElse (_NumberOfPartTimeEmployees <> "" AndAlso IsNumeric(_NumberOfPartTimeEmployees) = True) OrElse (_NumberOfPastoralCounselors <> "" AndAlso IsNumeric(_NumberOfPastoralCounselors) = True) OrElse (_NumberOfPremises <> "" AndAlso IsNumeric(_NumberOfPremises) = True) OrElse (_NumberOfRatableEmployees <> "" AndAlso IsNumeric(_NumberOfRatableEmployees) = True) OrElse (_NumberOfSwimmingPools <> "" AndAlso IsNumeric(_NumberOfSwimmingPools) = True) OrElse (_NumberOfTemporaryWorkers <> "" AndAlso IsNumeric(_NumberOfTemporaryWorkers) = True) OrElse qqHelper.IsNumericString(_NumberOfAdditionalPremises) = True OrElse (_OtherThanCollisionSubTypeId <> "" AndAlso IsNumeric(_OtherThanCollisionSubTypeId) = True) OrElse (_OtherThanCollisionTypeId <> "" AndAlso IsNumeric(_OtherThanCollisionTypeId) = True) OrElse (_PercentOfWorkers <> "" AndAlso IsNumeric(_PercentOfWorkers) = True) OrElse (_PersonalPropertyRateGradeTypeId <> "" AndAlso IsNumeric(_PersonalPropertyRateGradeTypeId) = True) OrElse _ProductDescription <> "" OrElse (_PropertyClassificationTypeId <> "" AndAlso IsNumeric(_PropertyClassificationTypeId) = True) OrElse (_ProtectionClassId <> "" AndAlso IsNumeric(_ProtectionClassId) = True) OrElse (_Rate <> "" AndAlso IsNumeric(_Rate) = True) OrElse (_RatingTypeId <> "" AndAlso IsNumeric(_RatingTypeId) = True) OrElse (_RetroactiveDate <> "" AndAlso IsDate(_RetroactiveDate) = True) OrElse (_RiskTypeId <> "" AndAlso IsNumeric(_RiskTypeId) = True) OrElse (_SalesClubs <> "" AndAlso IsNumeric(_SalesClubs) = True) OrElse (_SalesManufacturers <> "" AndAlso IsNumeric(_SalesManufacturers) = True) OrElse (_SalesPackageStores <> "" AndAlso IsNumeric(_SalesPackageStores) = True) OrElse (_SalesRestaurants <> "" AndAlso IsNumeric(_SalesRestaurants) = True) OrElse _SerialNumber <> "" OrElse _ShouldSyncWithMasterCoverage = True OrElse (_SpecialClassCodeTypeId <> "" AndAlso IsNumeric(_SpecialClassCodeTypeId) = True) OrElse _VIN <> "" OrElse (_ValuationMethodTypeId <> "" AndAlso IsNumeric(_ValuationMethodTypeId) = True) OrElse (_WaitingPeriodTypeId <> "" AndAlso IsNumeric(_WaitingPeriodTypeId) = True) OrElse (_Year <> "" AndAlso IsNumeric(_Year) = True) OrElse _IsNamedPerils = True OrElse _ExcludeEarthquake = True OrElse _DescriptionOfOperations <> "" OrElse _DesignatedJobSite <> "" OrElse _HasLoadingUnloading = True OrElse qqHelper.IsNumericString(_NumberOfVehicles) = True OrElse qqHelper.IsNumericString(_NumberOfShipments) = True OrElse qqHelper.IsNumericString(_OperatingRadius) = True OrElse _City <> "" OrElse _ExcludedEggs = True OrElse _ExcludedFursOrFurTrimmedGarments = True OrElse _ExcludedLiquor = True OrElse _ExcludedLivestockOrPoultry = True OrElse _ExcludedTobacco = True OrElse qqHelper.IsNumericString(_StateId) = True OrElse qqHelper.IsNumericString(_WithinMiles) = True OrElse qqHelper.IsNumericString(_MaximumDeductible) = True OrElse qqHelper.IsNumericString(_MinimumDeductible) = True OrElse _IsIndoor = True OrElse qqHelper.IsNumericString(_FaithfulPerformanceOfDutyTypeId) = True OrElse qqHelper.IsNumericString(_LimitTypeId) = True OrElse qqHelper.IsNumericString(_EmployeeTheftScheduleTypeId) = True OrElse qqHelper.IsNumericString(_NumberOfCardHolders) = True OrElse qqHelper.IsNumericString(_PremiumChargeTypeId) = True OrElse qqHelper.IsNumericString(_BasisTypeId) = True OrElse _IncludeSellingPrice = True OrElse _IsIncludeGuestsProperty = True OrElse _IsLimitToRobberyOnly = True OrElse _IsRequireRecordOfChecks = True OrElse qqHelper.IsDateString(_FromDate) = True OrElse qqHelper.IsDateString(_ToDate) = True OrElse _CoverageDescription <> "" OrElse _IsEmployeeTools = True OrElse _IsToolsLeasedOrRented = True OrElse _IsLimitedPerilsExtendedCoverage = True OrElse _IsAgreedValue = True OrElse String.IsNullOrEmpty(_DescriptionInformation) = False OrElse qqHelper.IsPositiveIntegerString(_LiquorLiabilityClassCodeTypeId) = True OrElse qqHelper.IsPositiveIntegerString(_NumberOfAmusementAreas) = True OrElse qqHelper.IsPositiveIntegerString(_Receipts) = True OrElse qqHelper.IsPositiveIntegerString(_PriorGrossReceipts) = True OrElse String.IsNullOrWhiteSpace(_CoveredHolesFairways) = False OrElse String.IsNullOrWhiteSpace(_CoveredHolesGreens) = False OrElse String.IsNullOrWhiteSpace(_CoveredHolesTees) = False OrElse String.IsNullOrWhiteSpace(_CoveredHolesTrees) = False OrElse _IsFairways = True OrElse _IsGreens = True OrElse _IsTees = True OrElse _IsTrees = True OrElse qqHelper.IsPositiveIntegerString(_NumberOfCarts) = True OrElse qqHelper.IsPositiveIntegerString(_AggregateLiabilityIncrementTypeId) = True OrElse qqHelper.IsPositiveIntegerString(_MedicalPaymentsTypeId) = True OrElse qqHelper.IsPositiveIntegerString(_NumberOfPlates) = True OrElse _IsDwellingStructure = True Then 'may not use _ShouldSyncWithMasterCoverage
                    hasCD = True
                End If
            End If

            Return hasCD
        End Function
        'added 1/20/2015 for reconciliation
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
        'added 3/30/2015 for reconciliation
        Public Sub ParseThruScheduledTextCollection()
            If _ScheduledTextCollection IsNot Nothing AndAlso _ScheduledTextCollection.Count > 0 Then
                For Each st As QuickQuoteScheduledText In _ScheduledTextCollection
                    'note: should only happen when parsing xml (FinalizeQuickQuote method)... shouldn't happen in FinalizeQuickQuoteLight method
                    If _CanUseScheduledTextNumForScheduledTextReconciliation = False Then
                        If st.HasValidScheduledTextNum = True Then
                            _CanUseScheduledTextNumForScheduledTextReconciliation = True
                            Exit For
                        End If
                    End If
                Next
            End If
        End Sub
        Public Overrides Function ToString() As String 'added 6/29/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.CoverageCodeId <> "" Then
                    str = qqHelper.appendText(str, "CoverageCodeId: " & Me.CoverageCodeId, vbCrLf)
                End If

                If Me.FullTermPremium <> "" Then
                    str = qqHelper.appendText(str, "FullTermPremium: " & Me.FullTermPremium, vbCrLf)
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
                    If _AnnualPremium IsNot Nothing Then
                        _AnnualPremium = Nothing
                    End If
                    If _EstimatedPremium IsNot Nothing Then
                        _EstimatedPremium = Nothing
                    End If
                    If _FullTermPremium IsNot Nothing Then
                        _FullTermPremium = Nothing
                    End If
                    If _PolicyId IsNot Nothing Then
                        _PolicyId = Nothing
                    End If
                    If _PolicyImageNum IsNot Nothing Then
                        _PolicyImageNum = Nothing
                    End If
                    If _WrittenPremium IsNot Nothing Then
                        _WrittenPremium = Nothing
                    End If
                    If _Checkbox <> Nothing Then
                        _Checkbox = Nothing
                    End If
                    If _Deductible IsNot Nothing Then
                        _Deductible = Nothing
                    End If
                    If _Manual <> Nothing Then
                        _Manual = Nothing
                    End If
                    If _ManualLimitAmount IsNot Nothing Then
                        _ManualLimitAmount = Nothing
                    End If
                    If _ManualLimitIncluded IsNot Nothing Then
                        _ManualLimitIncluded = Nothing
                    End If
                    If _ManualLimitIncreased IsNot Nothing Then
                        _ManualLimitIncreased = Nothing
                    End If
                    'If _ClassificationCode IsNot Nothing Then
                    '    _ClassificationCode = Nothing
                    'End If
                    If _CoverageCode IsNot Nothing Then
                        _CoverageCode = Nothing
                    End If
                    If _CoverageCodeId IsNot Nothing Then
                        _CoverageCodeId = Nothing
                    End If
                    If _CoverageLimitId IsNot Nothing Then
                        _CoverageLimitId = Nothing
                    End If
                    If _DeductibleTypeId IsNot Nothing Then
                        _DeductibleTypeId = Nothing
                    End If
                    If _Description IsNot Nothing Then
                        _Description = Nothing
                    End If
                    If _DescriptionInformation IsNot Nothing Then '3/9/2017 - BOP stuff
                        _DescriptionInformation = Nothing
                    End If
                    If _EffectiveDate IsNot Nothing Then
                        _EffectiveDate = Nothing
                    End If
                    If _ExpirationDate IsNot Nothing Then
                        _ExpirationDate = Nothing
                    End If
                    If _SubCoverageCodeId IsNot Nothing Then
                        _SubCoverageCodeId = Nothing
                    End If

                    If _NumberOfEmployees IsNot Nothing Then
                        _NumberOfEmployees = Nothing
                    End If
                    If _NumberOfLocations IsNot Nothing Then
                        _NumberOfLocations = Nothing
                    End If
                    If _AutomaticIncreasePercentTypeId IsNot Nothing Then
                        _AutomaticIncreasePercentTypeId = Nothing
                    End If
                    If _ValuationMethodTypeId IsNot Nothing Then
                        _ValuationMethodTypeId = Nothing
                    End If
                    If _IsIncludedInBlanketRating <> Nothing Then
                        _IsIncludedInBlanketRating = Nothing
                    End If
                    If _IsMineSubsidence <> Nothing Then
                        _IsMineSubsidence = Nothing
                    End If
                    If _IsCondoCommercialUnitOwnersCoverage <> Nothing Then
                        _IsCondoCommercialUnitOwnersCoverage = Nothing
                    End If
                    If _PropertyClassificationTypeId IsNot Nothing Then
                        _PropertyClassificationTypeId = Nothing
                    End If
                    If _IsRefrigerationMaintenanceAgreement <> Nothing Then
                        _IsRefrigerationMaintenanceAgreement = Nothing
                    End If
                    If _IsBreakdownOrContamination <> Nothing Then
                        _IsBreakdownOrContamination = Nothing
                    End If
                    If _IsPowerOutage <> Nothing Then
                        _IsPowerOutage = Nothing
                    End If

                    If _DeductiblePerTypeId IsNot Nothing Then
                        _DeductiblePerTypeId = Nothing
                    End If

                    If _NumberOfSwimmingPools IsNot Nothing Then
                        _NumberOfSwimmingPools = Nothing
                    End If

                    If _DeductibleId IsNot Nothing Then
                        _DeductibleId = Nothing
                    End If

                    If _CoverageAdditionalInfoRecords IsNot Nothing Then
                        If _CoverageAdditionalInfoRecords.Count > 0 Then
                            For Each add As QuickQuoteCoverageAdditionalInfoRecord In _CoverageAdditionalInfoRecords
                                add.Dispose()
                                add = Nothing
                            Next
                            _CoverageAdditionalInfoRecords.Clear()
                        End If
                        _CoverageAdditionalInfoRecords = Nothing
                    End If

                    'If _NumberOfFullTimeEmployees IsNot Nothing Then
                    '    _NumberOfFullTimeEmployees = Nothing
                    'End If
                    If _NumberOfPartTimeEmployees IsNot Nothing Then
                        _NumberOfPartTimeEmployees = Nothing
                    End If

                    If _ManuallyRated <> Nothing Then
                        _ManuallyRated = Nothing
                    End If
                    If _ManualPremium IsNot Nothing Then
                        _ManualPremium = Nothing
                    End If
                    If _NumberOfBurials IsNot Nothing Then
                        _NumberOfBurials = Nothing
                    End If
                    If _NumberOfBodies IsNot Nothing Then
                        _NumberOfBodies = Nothing
                    End If
                    If _NumberOfPastoralCounselors IsNot Nothing Then
                        _NumberOfPastoralCounselors = Nothing
                    End If

                    If _SalesClubs IsNot Nothing Then
                        _SalesClubs = Nothing
                    End If
                    If _SalesManufacturers IsNot Nothing Then
                        _SalesManufacturers = Nothing
                    End If
                    If _SalesPackageStores IsNot Nothing Then
                        _SalesPackageStores = Nothing
                    End If
                    If _SalesRestaurants IsNot Nothing Then
                        _SalesRestaurants = Nothing
                    End If

                    If _Comments IsNot Nothing Then
                        _Comments = Nothing
                    End If
                    If _NameInformation IsNot Nothing Then
                        _NameInformation = Nothing
                    End If
                    If _HasWaiverOfSubrogation <> Nothing Then
                        _HasWaiverOfSubrogation = Nothing
                    End If

                    If _DeductibleCategoryTypeId IsNot Nothing Then
                        _DeductibleCategoryTypeId = Nothing
                    End If

                    If _RetroactiveDate IsNot Nothing Then
                        _RetroactiveDate = Nothing
                    End If

                    If _DailyReimbursement IsNot Nothing Then
                        _DailyReimbursement = Nothing
                    End If
                    If _NumberOfDays IsNot Nothing Then
                        _NumberOfDays = Nothing
                    End If
                    If _NamedInsuredsBusinessTypeId IsNot Nothing Then
                        _NamedInsuredsBusinessTypeId = Nothing
                    End If
                    If _ExtendNonOwnershipRatingTypeId IsNot Nothing Then
                        _ExtendNonOwnershipRatingTypeId = Nothing
                    End If
                    If _CoverageTypeId IsNot Nothing Then
                        _CoverageTypeId = Nothing
                    End If
                    If _OtherThanCollisionSubTypeId IsNot Nothing Then
                        _OtherThanCollisionSubTypeId = Nothing
                    End If
                    If _OtherThanCollisionTypeId IsNot Nothing Then
                        _OtherThanCollisionTypeId = Nothing
                    End If
                    If _IfAnyBasis <> Nothing Then
                        _IfAnyBasis = Nothing
                    End If

                    If _AddressInformation IsNot Nothing Then
                        _AddressInformation = Nothing
                    End If

                    If _CommercialOccupancyTypeId IsNot Nothing Then
                        _CommercialOccupancyTypeId = Nothing
                    End If
                    If _ClassificationCode IsNot Nothing Then
                        _ClassificationCode = Nothing
                    End If
                    If _IsEarthquakeApplies <> Nothing Then
                        _IsEarthquakeApplies = Nothing
                    End If
                    If _CauseOfLossTypeId IsNot Nothing Then
                        _CauseOfLossTypeId = Nothing
                    End If
                    If _CoinsuranceTypeId IsNot Nothing Then
                        _CoinsuranceTypeId = Nothing
                    End If
                    If _RatingTypeId IsNot Nothing Then
                        _RatingTypeId = Nothing
                    End If
                    If _InflationGuardTypeId IsNot Nothing Then
                        _InflationGuardTypeId = Nothing
                    End If
                    If _BusinessPropertyTypeId IsNot Nothing Then
                        _BusinessPropertyTypeId = Nothing
                    End If
                    If _RiskTypeId IsNot Nothing Then
                        _RiskTypeId = Nothing
                    End If
                    If _MonthlyPeriodTypeId IsNot Nothing Then
                        _MonthlyPeriodTypeId = Nothing
                    End If
                    If _BusinessIncomeTypeId IsNot Nothing Then
                        _BusinessIncomeTypeId = Nothing
                    End If
                    If _LossCost IsNot Nothing Then
                        _LossCost = Nothing
                    End If
                    If _Rate IsNot Nothing Then
                        _Rate = Nothing
                    End If
                    If _WaitingPeriodTypeId IsNot Nothing Then
                        _WaitingPeriodTypeId = Nothing
                    End If

                    If _ProductDescription IsNot Nothing Then
                        _ProductDescription = Nothing
                    End If

                    If _PersonalPropertyRateGradeTypeId IsNot Nothing Then
                        _PersonalPropertyRateGradeTypeId = Nothing
                    End If

                    If _ManualElpaRate IsNot Nothing Then
                        _ManualElpaRate = Nothing
                    End If
                    If _ExcludeProductsCompletedOperations <> Nothing Then
                        _ExcludeProductsCompletedOperations = Nothing
                    End If
                    If _AggregateLimit IsNot Nothing Then
                        _AggregateLimit = Nothing
                    End If

                    If _DoesYardRateApplyTypeId IsNot Nothing Then
                        _DoesYardRateApplyTypeId = Nothing
                    End If

                    If _ConstructionTypeId IsNot Nothing Then
                        _ConstructionTypeId = Nothing
                    End If
                    If _FeetToFireHydrant IsNot Nothing Then
                        _FeetToFireHydrant = Nothing
                    End If
                    If _MilesToFireDepartment IsNot Nothing Then
                        _MilesToFireDepartment = Nothing
                    End If
                    If _SpecialClassCodeTypeId IsNot Nothing Then
                        _SpecialClassCodeTypeId = Nothing
                    End If
                    If _ProtectionClassId IsNot Nothing Then
                        _ProtectionClassId = Nothing
                    End If

                    If _IncludeCoverageDetail <> Nothing Then
                        _IncludeCoverageDetail = Nothing
                    End If

                    If _CoverageBasisTypeId IsNot Nothing Then
                        _CoverageBasisTypeId = Nothing
                    End If

                    'added 12/4/2014
                    If _Calc IsNot Nothing Then
                        _Calc = Nothing
                    End If

                    'added 1/20/2015 (Comm IM / Crime; Farm)
                    If _ASLId IsNot Nothing Then
                        _ASLId = Nothing
                    End If
                    If _MinimumPremiumAdjustment IsNot Nothing Then
                        _MinimumPremiumAdjustment = Nothing 'CoverageDetail
                    End If
                    If _NumberOfContractors IsNot Nothing Then
                        _NumberOfContractors = Nothing 'CoverageDetail
                    End If
                    If _NumberOfExcludedEmployees IsNot Nothing Then
                        _NumberOfExcludedEmployees = Nothing 'CoverageDetail
                    End If
                    If _NumberOfIndependentContractors IsNot Nothing Then
                        _NumberOfIndependentContractors = Nothing 'CoverageDetail
                    End If
                    If _NumberOfRatableEmployees IsNot Nothing Then
                        _NumberOfRatableEmployees = Nothing 'CoverageDetail
                    End If
                    If _NumberOfPremises IsNot Nothing Then
                        _NumberOfPremises = Nothing 'CoverageDetail
                    End If
                    If _NumberOfTemporaryWorkers IsNot Nothing Then
                        _NumberOfTemporaryWorkers = Nothing 'CoverageDetail
                    End If
                    If _PercentOfWorkers IsNot Nothing Then
                        _PercentOfWorkers = Nothing 'CoverageDetail
                    End If
                    _ShouldSyncWithMasterCoverage = Nothing 'CoverageDetail
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
                    _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = Nothing
                    If _Make IsNot Nothing Then
                        _Make = Nothing 'CoverageDetail
                    End If
                    If _ManufacturerName IsNot Nothing Then
                        _ManufacturerName = Nothing 'CoverageDetail
                    End If
                    If _Model IsNot Nothing Then
                        _Model = Nothing 'CoverageDetail
                    End If
                    If _SerialNumber IsNot Nothing Then
                        _SerialNumber = Nothing 'CoverageDetail
                    End If
                    If _VIN IsNot Nothing Then
                        _VIN = Nothing 'CoverageDetail
                    End If
                    If _Year IsNot Nothing Then
                        _Year = Nothing 'CoverageDetail
                    End If
                    'added 2/9/2015
                    _IsNamedPerils = Nothing 'CoverageDetail
                    'added 2/10/2015
                    _ExcludeEarthquake = Nothing 'CoverageDetail
                    'added 3/3/2015 for Farm
                    qqHelper.DisposeString(_DescriptionOfOperations) 'CoverageDetail
                    qqHelper.DisposeString(_DesignatedJobSite) 'CoverageDetail
                    'added 3/17/2015 for CIM
                    _HasLoadingUnloading = Nothing 'CoverageDetail
                    qqHelper.DisposeString(_NumberOfVehicles) 'CoverageDetail
                    qqHelper.DisposeString(_NumberOfShipments) 'CoverageDetail
                    'added 3/18/2015 for CIM
                    qqHelper.DisposeString(_OperatingRadius) 'CoverageDetail
                    'added 3/23/2015 for CIM
                    qqHelper.DisposeString(_City) 'CoverageDetail
                    _ExcludedEggs = Nothing 'CoverageDetail
                    _ExcludedFursOrFurTrimmedGarments = Nothing 'CoverageDetail
                    _ExcludedLiquor = Nothing 'CoverageDetail
                    _ExcludedLivestockOrPoultry = Nothing 'CoverageDetail
                    _ExcludedTobacco = Nothing 'CoverageDetail
                    'qqHelper.DisposeString(_ProductDescription) 'CoverageDetail; already coded above
                    qqHelper.DisposeString(_StateId) 'CoverageDetail; static data
                    qqHelper.DisposeString(_WithinMiles) 'CoverageDetail
                    'added 3/26/2015 for CIM
                    qqHelper.DisposeString(_MaximumDeductible) 'CoverageDetail
                    qqHelper.DisposeString(_MinimumDeductible) 'CoverageDetail
                    _IsIndoor = Nothing 'CoverageDetail
                    'added 3/30/2015 for CRM
                    If _ScheduledTextCollection IsNot Nothing Then
                        If _ScheduledTextCollection.Count > 0 Then
                            For Each st As QuickQuoteScheduledText In _ScheduledTextCollection
                                st.Dispose()
                                st = Nothing
                            Next
                            _ScheduledTextCollection.Clear()
                        End If
                        _ScheduledTextCollection = Nothing
                    End If
                    _CanUseScheduledTextNumForScheduledTextReconciliation = Nothing
                    qqHelper.DisposeString(_FaithfulPerformanceOfDutyTypeId) 'CoverageDetail; static data
                    qqHelper.DisposeString(_NumberOfAdditionalPremises) 'CoverageDetail
                    qqHelper.DisposeString(_LimitTypeId) 'CoverageDetail; static data
                    qqHelper.DisposeString(_EmployeeTheftScheduleTypeId) 'CoverageDetail; static data
                    qqHelper.DisposeString(_NumberOfCardHolders) 'CoverageDetail
                    qqHelper.DisposeString(_PremiumChargeTypeId) 'CoverageDetail; static data
                    _OverrideFullyEarned = Nothing
                    qqHelper.DisposeString(_BasisTypeId) 'CoverageDetail; static data
                    _IncludeSellingPrice = Nothing 'CoverageDetail
                    _IsIncludeGuestsProperty = Nothing 'CoverageDetail
                    _IsLimitToRobberyOnly = Nothing 'CoverageDetail
                    _IsRequireRecordOfChecks = Nothing 'CoverageDetail
                    qqHelper.DisposeString(_FromDate) 'CoverageDetail; /DateTime
                    qqHelper.DisposeString(_ToDate) 'CoverageDetail; /DateTime

                    'added 5/11/2015 (specific to peak season coverages; found under scheduled personal properties for Farm); we're currently not setting these Coverage properties but will if flag is True
                    _UseEffectiveDateAndExpirationDate = Nothing

                    'added 5/12/2015 for Farm (F & G Optional Covs)
                    qqHelper.DisposeString(_OriginalCost)

                    'added 5/14/2015 for Farm (may need to set these for some of the different covs... testing policy-level now)
                    _ApplyToWrittenPremium = Nothing
                    qqHelper.DisposeString(_Exposure)

                    'added 6/8/2015 for testing latest CPP enhancements
                    qqHelper.DisposeString(_CoverageDescription) 'CoverageDetail

                    'added 7/15/2015 for missed CIM requirement (on small tools floater)
                    _IsEmployeeTools = Nothing
                    _IsToolsLeasedOrRented = Nothing

                    'added 5/3/2016 for Farm Machinery coverage option (under UnscheduledPersonalProperties)
                    _IsLimitedPerilsExtendedCoverage = Nothing 'CoverageDetail

                    '3/9/2017 - BOP stuff
                    If _Receipts IsNot Nothing Then
                        _Receipts = Nothing
                    End If
                    If _PriorGrossReceipts IsNot Nothing Then
                        _PriorGrossReceipts = Nothing
                    End If
                    If _LiquorLiabilityClassCodeTypeId IsNot Nothing Then
                        _LiquorLiabilityClassCodeTypeId = Nothing
                    End If

                    'added 5/4/2017 for CIM (Golf)
                    qqHelper.DisposeString(_CoveredHolesFairways) 'covDetail; int (actually string)
                    qqHelper.DisposeString(_CoveredHolesGreens) 'covDetail; int (actually string)
                    qqHelper.DisposeString(_CoveredHolesTees) 'covDetail; int (actually string)
                    qqHelper.DisposeString(_CoveredHolesTrees) 'covDetail; int (actually string)
                    _IsFairways = Nothing 'covDetail
                    _IsGreens = Nothing 'covDetail
                    _IsTees = Nothing 'covDetail
                    _IsTrees = Nothing 'covDetail
                    _NumberOfCarts = Nothing 'covDetail; int

                    'added 5/5/2017 for GAR
                    qqHelper.DisposeString(_AggregateLiabilityIncrementTypeId) 'covDetail; int
                    qqHelper.DisposeString(_MedicalPaymentsTypeId) 'covDetail; int
                    'added 5/11/2017 for GAR
                    qqHelper.DisposeString(_NumberOfPlates) 'covDetail; int

                    'added 10/30/2017
                    qqHelper.DisposeString(_AdjustmentFactor) 'covDetail; decimal; won't update HasCoverageDetail to use it until code is updating in in QuickQuoteXML routines

                    qqHelper.DisposeString(_DetailStatusCode) 'added 3/30/2018

                    _OverwriteExposure = Nothing 'added 4/10/2018 for CPP Contractors and Manufacturers enhancements

                    _IsDwellingStructure = Nothing 'added 11/8/2018 for CPR mine subsidence; covDetail

                    qqHelper.DisposeString(_OwnerOccupiedPercentageId) 'added 12/26/2024; covDetail


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
