Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' object used to store lob-specific information for a quote
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()>
    Public Class QuickQuoteLobInfo 'added 6/1/2018 (from original multi-state branch)
        Inherits QuickQuoteBaseGenericObject(Of Object) 'updated 6/27/2018 from QuickQuoteBaseObject
        Implements IDisposable

        '8/21/2018 note: this class stopped being used once the PolicyLevelInfoExtended and RiskLevelInfoExtended classes came around; VersionAndLobInfo originally inherited from this class, but it now has separate properties for the PolicyLevel and RiskLevel extended classes

        Dim qqHelper As New QuickQuoteHelperClass

        'removed 7/24/2018
        ''Private _FullTermPremium As String
        'Private _PolicyId As String 'PL
        'Private _PolicyImageNum As String 'PL
        ''Private _AddFormsVersionId As String
        ''Private _Num As String
        ''Private _PackagePartTypeId As String
        ''Private _PackagePartType As String
        ''Private _RatingVersionId As String
        ''Private _UnderwritingVersionId As String
        ''Private _VersionId As String

        'removed 7/24/2018
        ''PolicyLevel
        'Private _AdditionalInterestListLinks As List(Of QuickQuoteAdditionalInterestListLink)
        'Private _AdditionalInterests As List(Of QuickQuoteAdditionalInterest)
        'Private _AggregateLiabilityIncrementTypeId As String
        'Private _AggregateLimit As String 'decimal
        'Private _AnniversaryRatingEffectiveDate As String
        'Private _AnniversaryRatingExpirationDate As String
        'Private _AutoHome As Boolean
        'Private _AutoSymbols As Generic.List(Of QuickQuoteAutoSymbol)
        'Private _BlanketRatingOption As String
        'Private _BlanketRatingOptionId As String
        'Private _ClassificationCodes As List(Of QuickQuoteClassificationCode)
        'Private _Coverages As Generic.List(Of QuickQuoteCoverage)
        'Private _DeductiblePerTypeId As String
        'Private _DeductiblePerType As String
        'Private _DrivecamContractEffectiveDate As String '/DateTime; may not be needed... identified in xml but not UI
        'Private _EmployeeDiscount As Boolean
        'Private _EmployeesFullTime As String
        'Private _EmployeesPartTime1To40Days As String
        'Private _EmployeesPartTime41To179Days As String
        'Private _EntityTypeId As String
        'Private _Exclusions As List(Of QuickQuoteExclusion)
        'Private _FacultativeReinsurance As Boolean
        'Private _FarmIncidentalLimitCoverages As List(Of QuickQuoteCoverage)
        'Private _GLClassifications As Generic.List(Of QuickQuoteGLClassification)
        'Private _HouseholdMembers As List(Of QuickQuoteHouseholdMember)
        'Private _InclusionsExclusions As Generic.List(Of QuickQuoteInclusionExclusion)
        'Private _LiabilityOptionId As String
        'Private _LimitedPerilsCategoryTypeId As String
        'Private _LossHistoryRecords As Generic.List(Of QuickQuoteLossHistoryRecord)
        'Private _Modifiers As Generic.List(Of QuickQuoteModifier)
        'Private _NumberOfEmployees As String 'int
        'Private _OptionalCoverages As List(Of QuickQuoteOptionalCoverage)
        'Private _PackageModificationAssignmentTypeId As String
        'Private _PackageModificationAssignmentType As String
        'Private _PackageTypeId As String
        'Private _PackageType As String
        'Private _PolicyTypeId As String
        'Private _PolicyType As String
        'Private _PolicyUnderwritings As Generic.List(Of QuickQuotePolicyUnderwriting)
        'Private _PolicyUnderwritingCodeAndLevelAndTabIds As Generic.List(Of String)
        'Private _PriorCarrier As QuickQuotePriorCarrier
        'Private _ProgramType As String
        'Private _ProgramTypeId As String
        'Private _RiskGrade As String
        'Private _RiskGradeLookupId As String
        'Private _ScheduledCoverages As List(Of QuickQuoteScheduledCoverage)
        'Private _ScheduledPersonalPropertyCoverages As List(Of QuickQuoteScheduledPersonalPropertyCoverage)
        'Private _ScheduledRatings As Generic.List(Of QuickQuoteScheduledRating)
        'Private _SelectMarketCredit As Boolean
        'Private _ThirdPartyData As QuickQuoteThirdPartyData
        'Private _TierTypeId As String
        'Private _TieringInformation As QuickQuoteTieringInformation
        'Private _UnscheduledPersonalPropertyCoverage As QuickQuoteUnscheduledPersonalPropertyCoverage
        ''added 7/11/2018
        'Private _ResidenceInfo As QuickQuoteResidenceInfo

        'removed 7/24/2018
        ''RiskLevel
        'Private _Applicants As Generic.List(Of QuickQuoteApplicant)
        'Private _Drivers As Generic.List(Of QuickQuoteDriver)
        'Private _Locations As Generic.List(Of QuickQuoteLocation)
        'Private _Vehicles As Generic.List(Of QuickQuoteVehicle)
        'Private _Operators As List(Of QuickQuoteOperator)

        'added 7/24/2018
        Private _PolicyLevelInfo As QuickQuoteLobPolicyLevelInfo
        Private _RiskLevelInfo As QuickQuoteLobRiskLevelInfo

        'Public Property FullTermPremium As String
        '    Get
        '        'Return _FullTermPremium
        '        Return qqHelper.QuotedPremiumFormat(_FullTermPremium)
        '    End Get
        '    Set(value As String)
        '        _FullTermPremium = value
        '        qqHelper.ConvertToQuotedPremiumFormat(_FullTermPremium)
        '    End Set
        'End Property
        Public Property PolicyId As String
            Get
                Return PolicyLevelInfo.PolicyId
            End Get
            Set(value As String)
                PolicyLevelInfo.PolicyId = value
            End Set
        End Property
        Public Property PolicyImageNum As String
            Get
                Return PolicyLevelInfo.PolicyImageNum
            End Get
            Set(value As String)
                PolicyLevelInfo.PolicyImageNum = value
            End Set
        End Property
        'Public Property AddFormsVersionId As String
        '    Get
        '        Return _AddFormsVersionId
        '    End Get
        '    Set(value As String)
        '        _AddFormsVersionId = value
        '    End Set
        'End Property
        'Public Property Num As String
        '    Get
        '        Return _Num
        '    End Get
        '    Set(value As String)
        '        _Num = value
        '    End Set
        'End Property
        'Public Property PackagePartTypeId As String
        '    Get
        '        Return _PackagePartTypeId
        '    End Get
        '    Set(value As String)
        '        _PackagePartTypeId = value
        '        _PackagePartType = ""
        '        If IsNumeric(_PackagePartTypeId) = True Then
        '            Select Case _PackagePartTypeId
        '                Case "0"
        '                    _PackagePartType = "N/A"
        '                Case "1"
        '                    _PackagePartType = "Property"
        '                Case "2"
        '                    _PackagePartType = "General Liability"
        '                Case "3"
        '                    _PackagePartType = "Inland Marine"
        '                Case "4"
        '                    _PackagePartType = "Crime"
        '                Case "5"
        '                    _PackagePartType = "Garage"
        '                Case "6"
        '                    _PackagePartType = "Package"
        '            End Select
        '        End If
        '    End Set
        'End Property
        'Public ReadOnly Property PackagePartType As String
        '    Get
        '        Return _PackagePartType
        '    End Get
        'End Property
        'Public Property RatingVersionId As String
        '    Get
        '        Return _RatingVersionId
        '    End Get
        '    Set(value As String)
        '        _RatingVersionId = value
        '    End Set
        'End Property
        'Public Property UnderwritingVersionId As String
        '    Get
        '        Return _UnderwritingVersionId
        '    End Get
        '    Set(value As String)
        '        _UnderwritingVersionId = value
        '    End Set
        'End Property
        'Public Property VersionId As String
        '    Get
        '        Return _VersionId
        '    End Get
        '    Set(value As String)
        '        _VersionId = value
        '    End Set
        'End Property

        'PolicyLevel
        Public Property AdditionalInterestListLinks As List(Of QuickQuoteAdditionalInterestListLink)
            Get
                Return PolicyLevelInfo.AdditionalInterestListLinks
            End Get
            Set(value As List(Of QuickQuoteAdditionalInterestListLink))
                PolicyLevelInfo.AdditionalInterestListLinks = value
            End Set
        End Property
        Public Property AdditionalInterests As List(Of QuickQuoteAdditionalInterest)
            Get
                Return PolicyLevelInfo.AdditionalInterests
            End Get
            Set(value As List(Of QuickQuoteAdditionalInterest))
                PolicyLevelInfo.AdditionalInterests = value
            End Set
        End Property
        Public Property AggregateLiabilityIncrementTypeId As String
            Get
                Return PolicyLevelInfo.AggregateLiabilityIncrementTypeId
            End Get
            Set(value As String)
                PolicyLevelInfo.AggregateLiabilityIncrementTypeId = value
            End Set
        End Property
        Public Property AggregateLimit As String 'decimal
            Get
                Return PolicyLevelInfo.AggregateLimit
            End Get
            Set(value As String)
                PolicyLevelInfo.AggregateLimit = value
            End Set
        End Property
        Public Property AnniversaryRatingEffectiveDate As String
            Get
                Return PolicyLevelInfo.AnniversaryRatingEffectiveDate
            End Get
            Set(value As String)
                PolicyLevelInfo.AnniversaryRatingEffectiveDate = value
            End Set
        End Property
        Public Property AnniversaryRatingExpirationDate As String
            Get
                Return PolicyLevelInfo.AnniversaryRatingExpirationDate
            End Get
            Set(value As String)
                PolicyLevelInfo.AnniversaryRatingExpirationDate = value
            End Set
        End Property
        Public Property AutoHome As Boolean
            Get
                Return PolicyLevelInfo.AutoHome
            End Get
            Set(value As Boolean)
                PolicyLevelInfo.AutoHome = value
            End Set
        End Property
        Public Property AutoSymbols As Generic.List(Of QuickQuoteAutoSymbol)
            Get
                Return PolicyLevelInfo.AutoSymbols
            End Get
            Set(value As Generic.List(Of QuickQuoteAutoSymbol))
                PolicyLevelInfo.AutoSymbols = value
            End Set
        End Property
        Public Property BlanketRatingOption As String
            Get
                Return PolicyLevelInfo.BlanketRatingOption
            End Get
            Set(value As String)
                PolicyLevelInfo.BlanketRatingOption = value
            End Set
        End Property
        Public Property BlanketRatingOptionId As String 'verified in database 7/3/2012
            Get
                Return PolicyLevelInfo.BlanketRatingOptionId
            End Get
            Set(value As String)
                PolicyLevelInfo.BlanketRatingOptionId = value
            End Set
        End Property
        Public Property ClassificationCodes As List(Of QuickQuoteClassificationCode)
            Get
                Return PolicyLevelInfo.ClassificationCodes
            End Get
            Set(value As List(Of QuickQuoteClassificationCode))
                PolicyLevelInfo.ClassificationCodes = value
            End Set
        End Property
        Public Property Coverages As Generic.List(Of QuickQuoteCoverage)
            Get
                Return PolicyLevelInfo.Coverages
            End Get
            Set(value As Generic.List(Of QuickQuoteCoverage))
                PolicyLevelInfo.Coverages = value
            End Set
        End Property
        Public Property DeductiblePerTypeId As String
            Get
                Return PolicyLevelInfo.DeductiblePerTypeId
            End Get
            Set(value As String)
                PolicyLevelInfo.DeductiblePerTypeId = value
            End Set
        End Property
        Public Property DeductiblePerType As String
            Get
                Return PolicyLevelInfo.DeductiblePerType
            End Get
            Set(value As String)
                PolicyLevelInfo.DeductiblePerType = value
            End Set
        End Property
        Public Property DrivecamContractEffectiveDate As String '/DateTime; may not be needed... identified in xml but not UI
            Get
                Return PolicyLevelInfo.DrivecamContractEffectiveDate
            End Get
            Set(value As String)
                PolicyLevelInfo.DrivecamContractEffectiveDate = value
            End Set
        End Property
        Public Property EmployeeDiscount As Boolean
            Get
                Return PolicyLevelInfo.EmployeeDiscount
            End Get
            Set(value As Boolean)
                PolicyLevelInfo.EmployeeDiscount = value
            End Set
        End Property
        Public Property EmployeesFullTime As String
            Get
                Return PolicyLevelInfo.EmployeesFullTime
            End Get
            Set(value As String)
                PolicyLevelInfo.EmployeesFullTime = value 'could add numeric formatting... maybe limit formatting (whole #)
            End Set
        End Property
        Public Property EmployeesPartTime1To40Days As String
            Get
                Return PolicyLevelInfo.EmployeesPartTime1To40Days
            End Get
            Set(value As String)
                PolicyLevelInfo.EmployeesPartTime1To40Days = value 'could add numeric formatting... maybe limit formatting (whole #)
            End Set
        End Property
        Public Property EmployeesPartTime41To179Days As String
            Get
                Return PolicyLevelInfo.EmployeesPartTime41To179Days
            End Get
            Set(value As String)
                PolicyLevelInfo.EmployeesPartTime41To179Days = value 'could add numeric formatting... maybe limit formatting (whole #)
            End Set
        End Property
        Public Property EntityTypeId As String
            Get
                Return PolicyLevelInfo.EntityTypeId
            End Get
            Set(value As String)
                PolicyLevelInfo.EntityTypeId = value
            End Set
        End Property
        Public Property Exclusions As List(Of QuickQuoteExclusion)
            Get
                Return PolicyLevelInfo.Exclusions
            End Get
            Set(value As List(Of QuickQuoteExclusion))
                PolicyLevelInfo.Exclusions = value
            End Set
        End Property
        Public Property FacultativeReinsurance As Boolean
            Get
                Return PolicyLevelInfo.FacultativeReinsurance
            End Get
            Set(value As Boolean)
                PolicyLevelInfo.FacultativeReinsurance = value
            End Set
        End Property
        Public Property FarmIncidentalLimitCoverages As List(Of QuickQuoteCoverage)
            Get
                Return PolicyLevelInfo.FarmIncidentalLimitCoverages
            End Get
            Set(value As List(Of QuickQuoteCoverage))
                PolicyLevelInfo.FarmIncidentalLimitCoverages = value
            End Set
        End Property
        Public Property GLClassifications As Generic.List(Of QuickQuoteGLClassification)
            Get
                Return PolicyLevelInfo.GLClassifications
            End Get
            Set(value As Generic.List(Of QuickQuoteGLClassification))
                PolicyLevelInfo.GLClassifications = value
            End Set
        End Property
        Public Property HouseholdMembers As List(Of QuickQuoteHouseholdMember)
            Get
                Return PolicyLevelInfo.HouseholdMembers
            End Get
            Set(value As List(Of QuickQuoteHouseholdMember))
                PolicyLevelInfo.HouseholdMembers = value
            End Set
        End Property
        Public Property InclusionsExclusions As Generic.List(Of QuickQuoteInclusionExclusion)
            Get
                Return PolicyLevelInfo.InclusionsExclusions
            End Get
            Set(value As Generic.List(Of QuickQuoteInclusionExclusion))
                PolicyLevelInfo.InclusionsExclusions = value
            End Set
        End Property
        Public Property LiabilityOptionId As String
            Get
                Return PolicyLevelInfo.LiabilityOptionId
            End Get
            Set(value As String)
                PolicyLevelInfo.LiabilityOptionId = value
            End Set
        End Property
        Public Property LimitedPerilsCategoryTypeId As String
            Get
                Return PolicyLevelInfo.LimitedPerilsCategoryTypeId
            End Get
            Set(value As String)
                PolicyLevelInfo.LimitedPerilsCategoryTypeId = value
            End Set
        End Property
        Public Property LossHistoryRecords As Generic.List(Of QuickQuoteLossHistoryRecord)
            Get
                Return PolicyLevelInfo.LossHistoryRecords
            End Get
            Set(value As Generic.List(Of QuickQuoteLossHistoryRecord))
                PolicyLevelInfo.LossHistoryRecords = value
            End Set
        End Property
        Public Property Modifiers As Generic.List(Of QuickQuoteModifier)
            Get
                Return PolicyLevelInfo.Modifiers
            End Get
            Set(value As Generic.List(Of QuickQuoteModifier))
                PolicyLevelInfo.Modifiers = value
            End Set
        End Property
        Public Property NumberOfEmployees As String 'int
            Get
                Return PolicyLevelInfo.NumberOfEmployees
            End Get
            Set(value As String)
                PolicyLevelInfo.NumberOfEmployees = value
            End Set
        End Property
        Public Property OptionalCoverages As List(Of QuickQuoteOptionalCoverage)
            Get
                Return PolicyLevelInfo.OptionalCoverages
            End Get
            Set(value As List(Of QuickQuoteOptionalCoverage))
                PolicyLevelInfo.OptionalCoverages = value
            End Set
        End Property
        Public Property PackageModificationAssignmentTypeId As String
            Get
                Return PolicyLevelInfo.PackageModificationAssignmentTypeId
            End Get
            Set(value As String)
                PolicyLevelInfo.PackageModificationAssignmentTypeId = value
            End Set
        End Property
        Public Property PackageModificationAssignmentType As String
            Get
                Return PolicyLevelInfo.PackageModificationAssignmentType
            End Get
            Set(value As String)
                PolicyLevelInfo.PackageModificationAssignmentType = value
            End Set
        End Property
        Public Property PackageTypeId As String
            Get
                Return PolicyLevelInfo.PackageTypeId
            End Get
            Set(value As String)
                PolicyLevelInfo.PackageTypeId = value
            End Set
        End Property
        Public Property PackageType As String
            Get
                Return PolicyLevelInfo.PackageType
            End Get
            Set(value As String)
                PolicyLevelInfo.PackageType = value
            End Set
        End Property
        Public Property PolicyTypeId As String 'only coded for CPR right now (9/27/2012)
            Get
                Return PolicyLevelInfo.PolicyTypeId
            End Get
            Set(value As String)
                PolicyLevelInfo.PolicyTypeId = value
            End Set
        End Property
        Public Property PolicyType As String 'only coded for CPR right now (9/27/2012)
            Get
                Return PolicyLevelInfo.PolicyType
            End Get
            Set(value As String)
                PolicyLevelInfo.PolicyType = value
            End Set
        End Property
        Public Property PolicyUnderwritings As Generic.List(Of QuickQuotePolicyUnderwriting)
            Get
                Return PolicyLevelInfo.PolicyUnderwritings
            End Get
            Set(value As Generic.List(Of QuickQuotePolicyUnderwriting))
                PolicyLevelInfo.PolicyUnderwritings = value
            End Set
        End Property
        Public Property PolicyUnderwritingCodeAndLevelAndTabIds As Generic.List(Of String)
            Get
                Return PolicyLevelInfo.PolicyUnderwritingCodeAndLevelAndTabIds
            End Get
            Set(value As Generic.List(Of String))
                PolicyLevelInfo.PolicyUnderwritingCodeAndLevelAndTabIds = value
            End Set
        End Property
        Public Property PriorCarrier As QuickQuotePriorCarrier
            Get
                Return PolicyLevelInfo.PriorCarrier
            End Get
            Set(value As QuickQuotePriorCarrier)
                PolicyLevelInfo.PriorCarrier = value
            End Set
        End Property
        Public Property ProgramType As String
            Get
                Return PolicyLevelInfo.ProgramType
            End Get
            Set(value As String)
                PolicyLevelInfo.ProgramType = value
            End Set
        End Property
        Public Property ProgramTypeId As String
            Get
                Return PolicyLevelInfo.ProgramTypeId
            End Get
            Set(value As String)
                PolicyLevelInfo.ProgramTypeId = value
            End Set
        End Property
        Public Property RiskGrade As String
            Get
                Return PolicyLevelInfo.RiskGrade
            End Get
            Set(value As String)
                PolicyLevelInfo.RiskGrade = value
            End Set
        End Property
        Public Property RiskGradeLookupId As String
            Get
                Return PolicyLevelInfo.RiskGradeLookupId
            End Get
            Set(value As String)
                PolicyLevelInfo.RiskGradeLookupId = value
            End Set
        End Property
        Public Property ScheduledCoverages As List(Of QuickQuoteScheduledCoverage)
            Get
                Return PolicyLevelInfo.ScheduledCoverages
            End Get
            Set(value As List(Of QuickQuoteScheduledCoverage))
                PolicyLevelInfo.ScheduledCoverages = value
            End Set
        End Property
        Public Property ScheduledPersonalPropertyCoverages As List(Of QuickQuoteScheduledPersonalPropertyCoverage)
            Get
                Return PolicyLevelInfo.ScheduledPersonalPropertyCoverages
            End Get
            Set(value As List(Of QuickQuoteScheduledPersonalPropertyCoverage))
                PolicyLevelInfo.ScheduledPersonalPropertyCoverages = value
            End Set
        End Property
        Public Property ScheduledRatings As Generic.List(Of QuickQuoteScheduledRating)
            Get
                Return PolicyLevelInfo.ScheduledRatings
            End Get
            Set(value As Generic.List(Of QuickQuoteScheduledRating))
                PolicyLevelInfo.ScheduledRatings = value
            End Set
        End Property
        Public Property SelectMarketCredit As Boolean
            Get
                Return PolicyLevelInfo.SelectMarketCredit
            End Get
            Set(value As Boolean)
                PolicyLevelInfo.SelectMarketCredit = value
            End Set
        End Property
        Public Property ThirdPartyData As QuickQuoteThirdPartyData
            Get
                Return PolicyLevelInfo.ThirdPartyData
            End Get
            Set(value As QuickQuoteThirdPartyData)
                PolicyLevelInfo.ThirdPartyData = value
            End Set
        End Property
        Public Property TierTypeId As String 'N/A=-1; None=0; Uniform=1; Variable=2
            Get
                Return PolicyLevelInfo.TierTypeId
            End Get
            Set(value As String)
                PolicyLevelInfo.TierTypeId = value
            End Set
        End Property
        Public Property TieringInformation As QuickQuoteTieringInformation
            Get
                Return PolicyLevelInfo.TieringInformation
            End Get
            Set(value As QuickQuoteTieringInformation)
                PolicyLevelInfo.TieringInformation = value
            End Set
        End Property
        Public Property UnscheduledPersonalPropertyCoverage As QuickQuoteUnscheduledPersonalPropertyCoverage
            Get
                Return PolicyLevelInfo.UnscheduledPersonalPropertyCoverage
            End Get
            Set(value As QuickQuoteUnscheduledPersonalPropertyCoverage)
                PolicyLevelInfo.UnscheduledPersonalPropertyCoverage = value
            End Set
        End Property
        'added 7/11/2018
        Public Property ResidenceInfo As QuickQuoteResidenceInfo
            Get
                Return PolicyLevelInfo.ResidenceInfo
            End Get
            Set(value As QuickQuoteResidenceInfo)
                PolicyLevelInfo.ResidenceInfo = value
            End Set
        End Property

        'RiskLevel
        Public Property Applicants As Generic.List(Of QuickQuoteApplicant)
            Get
                Return RiskLevelInfo.Applicants
            End Get
            Set(value As Generic.List(Of QuickQuoteApplicant))
                RiskLevelInfo.Applicants = value
            End Set
        End Property
        Public Property Drivers As Generic.List(Of QuickQuoteDriver)
            Get
                Return RiskLevelInfo.Drivers
            End Get
            Set(value As Generic.List(Of QuickQuoteDriver))
                RiskLevelInfo.Drivers = value
            End Set
        End Property
        Public Property Locations As Generic.List(Of QuickQuoteLocation)
            Get
                Return RiskLevelInfo.Locations
            End Get
            Set(value As Generic.List(Of QuickQuoteLocation))
                RiskLevelInfo.Locations = value
            End Set
        End Property
        Public Property Vehicles As Generic.List(Of QuickQuoteVehicle)
            Get
                Return RiskLevelInfo.Vehicles
            End Get
            Set(value As Generic.List(Of QuickQuoteVehicle))
                RiskLevelInfo.Vehicles = value
            End Set
        End Property
        Public Property Operators As List(Of QuickQuoteOperator)
            Get
                Return RiskLevelInfo.Operators
            End Get
            Set(value As List(Of QuickQuoteOperator))
                RiskLevelInfo.Operators = value
            End Set
        End Property

        'added 7/24/2018
        Public Property PolicyLevelInfo As QuickQuoteLobPolicyLevelInfo
            Get
                If _PolicyLevelInfo Is Nothing Then
                    _PolicyLevelInfo = New QuickQuoteLobPolicyLevelInfo
                End If
                _PolicyLevelInfo.SetParent = Me
                Return _PolicyLevelInfo
            End Get
            Set(value As QuickQuoteLobPolicyLevelInfo)
                _PolicyLevelInfo = value
            End Set
        End Property
        Public Property RiskLevelInfo As QuickQuoteLobRiskLevelInfo
            Get
                If _RiskLevelInfo Is Nothing Then
                    _RiskLevelInfo = New QuickQuoteLobRiskLevelInfo
                End If
                _RiskLevelInfo.SetParent = Me
                Return _RiskLevelInfo
            End Get
            Set(value As QuickQuoteLobRiskLevelInfo)
                _RiskLevelInfo = value
            End Set
        End Property

        Public Sub New()
            MyBase.New()
            SetDefaults()
        End Sub
        Public Sub New(Parent As QuickQuoteObject) 'added 6/27/2018; could probably just use generic type so one constructor could be used for multiple types
            MyBase.New()
            SetDefaults()
            Me.SetParent = Parent
        End Sub
        Public Sub New(Parent As QuickQuotePackagePart) 'added 6/27/2018; could probably just use generic type so one constructor could be used for multiple types
            MyBase.New()
            SetDefaults()
            Me.SetParent = Parent
        End Sub
        Private Sub SetDefaults()
            'added 7/24/2018
            _PolicyLevelInfo = New QuickQuoteLobPolicyLevelInfo
            _RiskLevelInfo = New QuickQuoteLobRiskLevelInfo

            'removed 7/24/2018
            ''_FullTermPremium = ""
            '_PolicyId = ""
            '_PolicyImageNum = ""
            ''_AddFormsVersionId = ""
            ''_Num = ""
            ''_PackagePartTypeId = ""
            ''_PackagePartType = ""
            ''_RatingVersionId = ""
            ''_UnderwritingVersionId = ""
            ''_VersionId = ""

            'removed 7/24/2018
            ''PolicyLevel
            '_AdditionalInterestListLinks = Nothing
            '_AdditionalInterests = Nothing
            '_AggregateLiabilityIncrementTypeId = ""
            '_AggregateLimit = ""
            '_AnniversaryRatingEffectiveDate = ""
            '_AnniversaryRatingExpirationDate = ""
            '_AutoHome = False
            '_AutoSymbols = Nothing
            '_BlanketRatingOptionId = ""
            '_ClassificationCodes = Nothing
            '_Coverages = Nothing
            '_DeductiblePerTypeId = ""
            '_DeductiblePerType = ""
            '_DrivecamContractEffectiveDate = "" '/DateTime; may not be needed... identified in xml but not UI
            '_EmployeeDiscount = False
            '_EmployeesFullTime = ""
            '_EmployeesPartTime1To40Days = ""
            '_EmployeesPartTime41To179Days = ""
            '_EntityTypeId = ""
            '_Exclusions = Nothing
            '_FacultativeReinsurance = False
            '_FarmIncidentalLimitCoverages = Nothing
            '_GLClassifications = Nothing
            '_HouseholdMembers = Nothing
            '_InclusionsExclusions = Nothing
            '_LiabilityOptionId = ""
            '_LimitedPerilsCategoryTypeId = ""
            '_LossHistoryRecords = Nothing
            '_Modifiers = Nothing
            '_NumberOfEmployees = ""
            '_OptionalCoverages = Nothing
            '_PackageModificationAssignmentTypeId = ""
            '_PackageModificationAssignmentType = ""
            '_PackageTypeId = ""
            '_PackageType = ""
            '_PolicyTypeId = ""
            '_PolicyType = ""
            '_PolicyUnderwritings = Nothing
            '_PolicyUnderwritingCodeAndLevelAndTabIds = Nothing
            '_PriorCarrier = New QuickQuotePriorCarrier
            '_ProgramType = ""
            '_ProgramTypeId = ""
            '_RiskGrade = ""
            '_RiskGradeLookupId = ""
            '_ScheduledCoverages = Nothing
            '_ScheduledPersonalPropertyCoverages = Nothing
            '_ScheduledRatings = Nothing
            '_SelectMarketCredit = False
            '_ThirdPartyData = New QuickQuoteThirdPartyData
            '_TierTypeId = ""
            '_TieringInformation = New QuickQuoteTieringInformation
            '_UnscheduledPersonalPropertyCoverage = Nothing
            ''added 7/11/2018
            '_ResidenceInfo = New QuickQuoteResidenceInfo

            'removed 7/24/2018
            ''RiskLevel
            '_Applicants = Nothing
            '_Drivers = Nothing
            '_Locations = Nothing
            '_Vehicles = Nothing
            '_Operators = Nothing
        End Sub
        Public Overrides Function ToString() As String
            Dim str As String = ""
            If Me IsNot Nothing Then
                'If Me.PackagePartTypeId <> "" Then
                '    Dim t As String = ""
                '    t = "PackagePartTypeId: " & Me.PackagePartTypeId
                '    If Me.PackagePartType <> "" Then
                '        t &= " (" & Me.PackagePartType & ")"
                '    End If
                '    str = qqHelper.appendText(str, t, vbCrLf)
                'End If
                'If Me.FullTermPremium <> "" Then
                '    str = qqHelper.appendText(str, "FullTermPremium: " & Me.FullTermPremium, vbCrLf)
                'End If
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
                If Me.Coverages IsNot Nothing AndAlso Me.Coverages.Count > 0 Then
                    str = qqHelper.appendText(str, Me.Coverages.Count.ToString & " Coverages", vbCrLf)
                End If
                If Me.Modifiers IsNot Nothing AndAlso Me.Modifiers.Count > 0 Then
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
        'updated w/ QuickQuoteBaseObject inheritance
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    'removed 7/24/2018
                    ''qqHelper.DisposeString(_FullTermPremium)
                    'qqHelper.DisposeString(_PolicyId)
                    'qqHelper.DisposeString(_PolicyImageNum)
                    ''qqHelper.DisposeString(_AddFormsVersionId)
                    ''qqHelper.DisposeString(_Num)
                    ''qqHelper.DisposeString(_PackagePartTypeId)
                    ''qqHelper.DisposeString(_PackagePartType)
                    ''qqHelper.DisposeString(_RatingVersionId)
                    ''qqHelper.DisposeString(_UnderwritingVersionId)
                    ''qqHelper.DisposeString(_VersionId)

                    'removed 7/24/2018
                    ''PolicyLevel
                    'If _AdditionalInterestListLinks IsNot Nothing Then
                    '    If _AdditionalInterestListLinks.Count > 0 Then
                    '        For Each ll As QuickQuoteAdditionalInterestListLink In _AdditionalInterestListLinks
                    '            If ll Is Nothing Then
                    '                ll.Dispose()
                    '                ll = Nothing
                    '            End If
                    '        Next
                    '        _AdditionalInterestListLinks.Clear()
                    '    End If
                    '    _AdditionalInterestListLinks = Nothing
                    'End If
                    'qqHelper.DisposeAdditionalInterests(_AdditionalInterests)
                    'qqHelper.DisposeString(_AggregateLiabilityIncrementTypeId)
                    'qqHelper.DisposeString(_AggregateLimit)
                    'qqHelper.DisposeString(_AnniversaryRatingEffectiveDate)
                    'qqHelper.DisposeString(_AnniversaryRatingExpirationDate)
                    '_AutoHome = Nothing
                    'If _AutoSymbols IsNot Nothing Then
                    '    If _AutoSymbols.Count > 0 Then
                    '        For Each s As QuickQuoteAutoSymbol In _AutoSymbols
                    '            s.Dispose()
                    '            s = Nothing
                    '        Next
                    '        _AutoSymbols.Clear()
                    '    End If
                    '    _AutoSymbols = Nothing
                    'End If
                    'qqHelper.DisposeString(_BlanketRatingOptionId)
                    'If _ClassificationCodes IsNot Nothing Then
                    '    If _ClassificationCodes.Count > 0 Then
                    '        For Each c As QuickQuoteClassificationCode In _ClassificationCodes
                    '            c.Dispose()
                    '            c = Nothing
                    '        Next
                    '        _ClassificationCodes.Clear()
                    '    End If
                    '    _ClassificationCodes = Nothing
                    'End If
                    'If _Coverages IsNot Nothing Then
                    '    If _Coverages.Count > 0 Then
                    '        For Each cov As QuickQuoteCoverage In _Coverages
                    '            cov.Dispose()
                    '            cov = Nothing
                    '        Next
                    '        _Coverages.Clear()
                    '    End If
                    '    _Coverages = Nothing
                    'End If
                    'qqHelper.DisposeString(_DeductiblePerTypeId)
                    'qqHelper.DisposeString(_DeductiblePerType)
                    'qqHelper.DisposeString(_DrivecamContractEffectiveDate) '/DateTime; may not be needed... identified in xml but not UI
                    '_EmployeeDiscount = Nothing
                    'qqHelper.DisposeString(_EmployeesFullTime)
                    'qqHelper.DisposeString(_EmployeesPartTime1To40Days)
                    'qqHelper.DisposeString(_EmployeesPartTime41To179Days)
                    'qqHelper.DisposeString(_EntityTypeId)
                    'If _Exclusions IsNot Nothing Then
                    '    If _Exclusions.Count > 0 Then
                    '        For Each ex As QuickQuoteExclusion In _Exclusions
                    '            ex.Dispose()
                    '            ex = Nothing
                    '        Next
                    '        _Exclusions.Clear()
                    '    End If
                    '    _Exclusions = Nothing
                    'End If
                    '_FacultativeReinsurance = Nothing
                    'If _FarmIncidentalLimitCoverages IsNot Nothing Then
                    '    If _FarmIncidentalLimitCoverages.Count > 0 Then
                    '        For Each c As QuickQuoteCoverage In _FarmIncidentalLimitCoverages
                    '            c.Dispose()
                    '            c = Nothing
                    '        Next
                    '        _FarmIncidentalLimitCoverages.Clear()
                    '    End If
                    '    _FarmIncidentalLimitCoverages = Nothing
                    'End If
                    'If _GLClassifications IsNot Nothing Then
                    '    If _GLClassifications.Count > 0 Then
                    '        For Each gl As QuickQuoteGLClassification In _GLClassifications
                    '            gl.Dispose()
                    '            gl = Nothing
                    '        Next
                    '        _GLClassifications.Clear()
                    '    End If
                    '    _GLClassifications = Nothing
                    'End If
                    'If _HouseholdMembers IsNot Nothing Then
                    '    If _HouseholdMembers.Count > 0 Then
                    '        For Each m As QuickQuoteHouseholdMember In _HouseholdMembers
                    '            m.Dispose()
                    '            m = Nothing
                    '        Next
                    '        _HouseholdMembers.Clear()
                    '    End If
                    '    _HouseholdMembers = Nothing
                    'End If
                    'If _InclusionsExclusions IsNot Nothing Then
                    '    If _InclusionsExclusions.Count > 0 Then
                    '        For Each ie As QuickQuoteInclusionExclusion In _InclusionsExclusions
                    '            ie.Dispose()
                    '            ie = Nothing
                    '        Next
                    '        _InclusionsExclusions.Clear()
                    '    End If
                    '    _InclusionsExclusions = Nothing
                    'End If
                    'qqHelper.DisposeString(_LiabilityOptionId)
                    'qqHelper.DisposeString(_LimitedPerilsCategoryTypeId)
                    'If _LossHistoryRecords IsNot Nothing Then
                    '    If _LossHistoryRecords.Count > 0 Then
                    '        For Each lh As QuickQuoteLossHistoryRecord In _LossHistoryRecords
                    '            lh.Dispose()
                    '            lh = Nothing
                    '        Next
                    '        _LossHistoryRecords.Clear()
                    '    End If
                    '    _LossHistoryRecords = Nothing
                    'End If
                    'If _Modifiers IsNot Nothing Then
                    '    If _Modifiers.Count > 0 Then
                    '        For Each m As QuickQuoteModifier In _Modifiers
                    '            m.Dispose()
                    '            m = Nothing
                    '        Next
                    '        _Modifiers.Clear()
                    '    End If
                    '    _Modifiers = Nothing
                    'End If
                    'qqHelper.DisposeString(_NumberOfEmployees)
                    'If _OptionalCoverages IsNot Nothing Then
                    '    If _OptionalCoverages.Count > 0 Then
                    '        For Each oc As QuickQuoteOptionalCoverage In _OptionalCoverages
                    '            oc.Dispose()
                    '            oc = Nothing
                    '        Next
                    '        _OptionalCoverages.Clear()
                    '    End If
                    '    _OptionalCoverages = Nothing
                    'End If
                    'qqHelper.DisposeString(_PackageModificationAssignmentTypeId)
                    'qqHelper.DisposeString(_PackageModificationAssignmentType)
                    'qqHelper.DisposeString(_PackageTypeId)
                    'qqHelper.DisposeString(_PackageType)
                    'qqHelper.DisposeString(_PolicyTypeId)
                    'qqHelper.DisposeString(_PolicyType)
                    'If _PolicyUnderwritings IsNot Nothing Then
                    '    If _PolicyUnderwritings.Count > 0 Then
                    '        For Each uw As QuickQuotePolicyUnderwriting In _PolicyUnderwritings
                    '            uw.Dispose()
                    '            uw = Nothing
                    '        Next
                    '        _PolicyUnderwritings.Clear()
                    '    End If
                    '    _PolicyUnderwritings = Nothing
                    'End If
                    'If _PolicyUnderwritingCodeAndLevelAndTabIds IsNot Nothing Then
                    '    If _PolicyUnderwritingCodeAndLevelAndTabIds.Count > 0 Then
                    '        For Each codeId As String In _PolicyUnderwritingCodeAndLevelAndTabIds
                    '            codeId = Nothing
                    '        Next
                    '        _PolicyUnderwritingCodeAndLevelAndTabIds.Clear()
                    '    End If
                    '    _PolicyUnderwritingCodeAndLevelAndTabIds = Nothing
                    'End If
                    'If _PriorCarrier IsNot Nothing Then
                    '    _PriorCarrier.Dispose()
                    '    _PriorCarrier = Nothing
                    'End If
                    'qqHelper.DisposeString(_ProgramType)
                    'qqHelper.DisposeString(_ProgramTypeId)
                    'qqHelper.DisposeString(_RiskGrade)
                    'qqHelper.DisposeString(_RiskGradeLookupId)
                    'If _ScheduledCoverages IsNot Nothing Then
                    '    If _ScheduledCoverages.Count > 0 Then
                    '        For Each c As QuickQuoteScheduledCoverage In _ScheduledCoverages
                    '            c.Dispose()
                    '            c = Nothing
                    '        Next
                    '        _ScheduledCoverages.Clear()
                    '    End If
                    '    _ScheduledCoverages = Nothing
                    'End If
                    'If _ScheduledPersonalPropertyCoverages IsNot Nothing Then
                    '    If _ScheduledPersonalPropertyCoverages.Count > 0 Then
                    '        For Each sp As QuickQuoteScheduledPersonalPropertyCoverage In _ScheduledPersonalPropertyCoverages
                    '            sp.Dispose()
                    '            sp = Nothing
                    '        Next
                    '        _ScheduledPersonalPropertyCoverages.Clear()
                    '    End If
                    '    _ScheduledPersonalPropertyCoverages = Nothing
                    'End If
                    'If _ScheduledRatings IsNot Nothing Then
                    '    If _ScheduledRatings.Count > 0 Then
                    '        For Each sr As QuickQuoteScheduledRating In _ScheduledRatings
                    '            sr.Dispose()
                    '            sr = Nothing
                    '        Next
                    '        _ScheduledRatings.Clear()
                    '    End If
                    '    _ScheduledRatings = Nothing
                    'End If
                    '_SelectMarketCredit = Nothing
                    'If _ThirdPartyData IsNot Nothing Then
                    '    _ThirdPartyData.Dispose()
                    '    _ThirdPartyData = Nothing
                    'End If
                    'qqHelper.DisposeString(_TierTypeId)
                    'If _TieringInformation IsNot Nothing Then
                    '    _TieringInformation.Dispose()
                    '    _TieringInformation = Nothing
                    'End If
                    'If _UnscheduledPersonalPropertyCoverage IsNot Nothing Then
                    '    _UnscheduledPersonalPropertyCoverage.Dispose()
                    '    _UnscheduledPersonalPropertyCoverage = Nothing
                    'End If
                    ''added 7/11/2018
                    'If _ResidenceInfo IsNot Nothing Then
                    '    _ResidenceInfo.Dispose()
                    '    _ResidenceInfo = Nothing
                    'End If

                    'removed 7/24/2018
                    ''RiskLevel
                    'If _Applicants IsNot Nothing Then
                    '    If _Applicants.Count > 0 Then
                    '        For Each a As QuickQuoteApplicant In _Applicants
                    '            a.Dispose()
                    '            a = Nothing
                    '        Next
                    '        _Applicants.Clear()
                    '    End If
                    '    _Applicants = Nothing
                    'End If
                    'If _Drivers IsNot Nothing Then
                    '    If _Drivers.Count > 0 Then
                    '        For Each d As QuickQuoteDriver In _Drivers
                    '            d.Dispose()
                    '            d = Nothing
                    '        Next
                    '        _Drivers.Clear()
                    '    End If
                    '    _Drivers = Nothing
                    'End If
                    'If _Locations IsNot Nothing Then
                    '    If _Locations.Count > 0 Then
                    '        For Each Loc As QuickQuoteLocation In _Locations
                    '            Loc.Dispose()
                    '            Loc = Nothing
                    '        Next
                    '        _Locations.Clear()
                    '    End If
                    '    _Locations = Nothing
                    'End If
                    'If _Vehicles IsNot Nothing Then
                    '    If _Vehicles.Count > 0 Then
                    '        For Each v As QuickQuoteVehicle In _Vehicles
                    '            v.Dispose()
                    '            v = Nothing
                    '        Next
                    '        _Vehicles.Clear()
                    '    End If
                    '    _Vehicles = Nothing
                    'End If
                    'If _Operators IsNot Nothing Then
                    '    If _Operators.Count > 0 Then
                    '        For Each o As QuickQuoteOperator In _Operators
                    '            o.Dispose()
                    '            o = Nothing
                    '        Next
                    '        _Operators.Clear()
                    '    End If
                    '    _Operators = Nothing
                    'End If

                    'added 7/24/2018
                    If _PolicyLevelInfo IsNot Nothing Then
                        _PolicyLevelInfo.Dispose()
                        _PolicyLevelInfo = Nothing
                    End If
                    If _RiskLevelInfo IsNot Nothing Then
                        _RiskLevelInfo.Dispose()
                        _RiskLevelInfo = Nothing
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
