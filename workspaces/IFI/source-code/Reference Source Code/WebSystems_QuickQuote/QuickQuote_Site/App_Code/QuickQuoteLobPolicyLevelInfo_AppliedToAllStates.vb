Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' object used to store policy-level lob-specific information (that applies to all states) for a quote
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()>
    Public Class QuickQuoteLobPolicyLevelInfo_AppliedToAllStates 'added 8/16/2018
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _PolicyId As String 'PL
        Private _PolicyImageNum As String 'PL

        'PolicyLevel
        Private _AggregateLiabilityIncrementTypeId As String
        Private _AggregateLimit As String 'decimal
        Private _AnniversaryRatingEffectiveDate As String
        Private _AnniversaryRatingExpirationDate As String
        Private _AutoHome As Boolean
        Private _AutoSymbols As Generic.List(Of QuickQuoteAutoSymbol)
        Private _BlanketRatingOption As String
        Private _BlanketRatingOptionId As String
        Private _DeductiblePerTypeId As String
        Private _DeductiblePerType As String
        Private _DrivecamContractEffectiveDate As String '/DateTime; may not be needed... identified in xml but not UI
        Private _EmployeeDiscount As Boolean
        Private _EmployeesFullTime As String
        Private _EmployeesPartTime1To40Days As String
        Private _EmployeesPartTime41To179Days As String
        Private _EntityTypeId As String
        Private _Exclusions As List(Of QuickQuoteExclusion)
        Private _FacultativeReinsurance As Boolean
        Private _FarmIncidentalLimitCoverages As List(Of QuickQuoteCoverage)
        Private _HouseholdMembers As List(Of QuickQuoteHouseholdMember)
        Private _LiabilityOptionId As String
        Private _LimitedPerilsCategoryTypeId As String
        Private _Modifiers As Generic.List(Of QuickQuoteModifier)
        Private _NumberOfEmployees As String 'int
        'Private _OptionalCoverages As List(Of QuickQuoteOptionalCoverage) 'Moved to GoverningState 11/07/2019 bug 33571 MLW
        Private _PackageModificationAssignmentTypeId As String
        Private _PackageModificationAssignmentType As String
        Private _PackageTypeId As String
        Private _PackageType As String
        Private _PolicyTypeId As String
        Private _PolicyType As String
        Private _PolicyUnderwritings As Generic.List(Of QuickQuotePolicyUnderwriting)
        Private _PolicyUnderwritingCodeAndLevelAndTabIds As Generic.List(Of String)
        Private _PriorCarrier As QuickQuotePriorCarrier
        Private _ProgramType As String
        Private _ProgramTypeId As String
        Private _RiskGrade As String
        Private _RiskGradeLookupId As String
        'Private _ScheduledPersonalPropertyCoverages As List(Of QuickQuoteScheduledPersonalPropertyCoverage) 'moved to GoverningState 2/5/2019
        Private _ScheduledRatings As Generic.List(Of QuickQuoteScheduledRating)
        Private _SelectMarketCredit As Boolean
        Private _ThirdPartyData As QuickQuoteThirdPartyData
        Private _TierTypeId As String
        Private _TieringInformation As QuickQuoteTieringInformation
        'Private _UnscheduledPersonalPropertyCoverage As QuickQuoteUnscheduledPersonalPropertyCoverage 'moved to GoverningState 2/5/2019
        Private _ResidenceInfo As QuickQuoteResidenceInfo

        'added 4/20/2020 for PUP/FUP
        Private _FarmSizeTypeId As String 'static data
        Private _FarmTypeId As String 'static data

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
        Public Property AggregateLiabilityIncrementTypeId As String
            Get
                Return _AggregateLiabilityIncrementTypeId
            End Get
            Set(value As String)
                _AggregateLiabilityIncrementTypeId = value
            End Set
        End Property
        Public Property AggregateLimit As String 'decimal
            Get
                Return _AggregateLimit
            End Get
            Set(value As String)
                _AggregateLimit = value
                qqHelper.ConvertToLimitFormat(_AggregateLimit)
            End Set
        End Property
        Public Property AnniversaryRatingEffectiveDate As String
            Get
                Return _AnniversaryRatingEffectiveDate
            End Get
            Set(value As String)
                _AnniversaryRatingEffectiveDate = value
                qqHelper.ConvertToShortDate(_AnniversaryRatingEffectiveDate)
                If IsDate(_AnniversaryRatingEffectiveDate) = True Then
                    AnniversaryRatingExpirationDate = DateAdd(DateInterval.Year, 1, CDate(_AnniversaryRatingEffectiveDate))
                End If
            End Set
        End Property
        Public Property AnniversaryRatingExpirationDate As String
            Get
                Return _AnniversaryRatingExpirationDate
            End Get
            Set(value As String)
                _AnniversaryRatingExpirationDate = value
                qqHelper.ConvertToShortDate(_AnniversaryRatingExpirationDate)
            End Set
        End Property
        Public Property AutoHome As Boolean
            Get
                Return _AutoHome
            End Get
            Set(value As Boolean)
                _AutoHome = value
            End Set
        End Property
        Public Property AutoSymbols As Generic.List(Of QuickQuoteAutoSymbol)
            Get
                Return _AutoSymbols
            End Get
            Set(value As Generic.List(Of QuickQuoteAutoSymbol))
                _AutoSymbols = value
            End Set
        End Property
        Public Property BlanketRatingOption As String
            Get
                Return _BlanketRatingOption
            End Get
            Set(value As String)
                _BlanketRatingOption = value
                Select Case _BlanketRatingOption
                    Case "N/A"
                        _BlanketRatingOptionId = "0"
                    Case "Combined Building and Personal Property"
                        _BlanketRatingOptionId = "1"
                    Case "Building Only"
                        _BlanketRatingOptionId = "2"
                    Case "Personal Property Only"
                        _BlanketRatingOptionId = "3"
                    Case Else
                        _BlanketRatingOptionId = ""
                End Select
            End Set
        End Property
        Public Property BlanketRatingOptionId As String 'verified in database 7/3/2012
            Get
                Return _BlanketRatingOptionId
            End Get
            Set(value As String)
                _BlanketRatingOptionId = value
                '(0=N/A; 1=Combined Building and Personal Property; 2=Building Only; 3=Personal Property Only)
                _BlanketRatingOption = ""
                If IsNumeric(_BlanketRatingOptionId) = True Then
                    Select Case _BlanketRatingOptionId
                        Case "0"
                            _BlanketRatingOption = "N/A"
                        Case "1"
                            _BlanketRatingOption = "Combined Building and Personal Property"
                        Case "2"
                            _BlanketRatingOption = "Building Only"
                        Case "3"
                            _BlanketRatingOption = "Personal Property Only"
                    End Select
                End If
            End Set
        End Property
        Public Property DeductiblePerTypeId As String
            Get
                Return _DeductiblePerTypeId
            End Get
            Set(value As String)
                _DeductiblePerTypeId = value
                '(0-N/A; 1=Per Occurrence; 2=Per Claim)
                _DeductiblePerType = ""
                If IsNumeric(_DeductiblePerTypeId) = True Then
                    Select Case _DeductiblePerTypeId
                        Case "0"
                            _DeductiblePerType = "N/A"
                        Case "1"
                            _DeductiblePerType = "Per Occurrence"
                        Case "2"
                            _DeductiblePerType = "Per Claim"
                    End Select
                End If
            End Set
        End Property
        Public Property DeductiblePerType As String
            Get
                Return _DeductiblePerType
            End Get
            Set(value As String)
                _DeductiblePerType = value
                Select Case _DeductiblePerType
                    Case "N/A"
                        _DeductiblePerTypeId = "0"
                    Case "Per Occurrence"
                        _DeductiblePerTypeId = "1"
                    Case "Per Claim"
                        _DeductiblePerTypeId = "2"
                    Case Else
                        _DeductiblePerTypeId = ""
                End Select
            End Set
        End Property
        Public Property DrivecamContractEffectiveDate As String '/DateTime; may not be needed... identified in xml but not UI
            Get
                Return _DrivecamContractEffectiveDate
            End Get
            Set(value As String)
                _DrivecamContractEffectiveDate = value
                qqHelper.ConvertToShortDate(_DrivecamContractEffectiveDate)
            End Set
        End Property
        Public Property EmployeeDiscount As Boolean
            Get
                Return _EmployeeDiscount
            End Get
            Set(value As Boolean)
                _EmployeeDiscount = value
            End Set
        End Property
        Public Property EmployeesFullTime As String
            Get
                Return _EmployeesFullTime
            End Get
            Set(value As String)
                _EmployeesFullTime = value 'could add numeric formatting... maybe limit formatting (whole #)
            End Set
        End Property
        Public Property EmployeesPartTime1To40Days As String
            Get
                Return _EmployeesPartTime1To40Days
            End Get
            Set(value As String)
                _EmployeesPartTime1To40Days = value 'could add numeric formatting... maybe limit formatting (whole #)
            End Set
        End Property
        Public Property EmployeesPartTime41To179Days As String
            Get
                Return _EmployeesPartTime41To179Days
            End Get
            Set(value As String)
                _EmployeesPartTime41To179Days = value 'could add numeric formatting... maybe limit formatting (whole #)
            End Set
        End Property
        Public Property EntityTypeId As String
            Get
                Return _EntityTypeId
            End Get
            Set(value As String)
                _EntityTypeId = value
            End Set
        End Property
        Public Property Exclusions As List(Of QuickQuoteExclusion)
            Get
                Return _Exclusions
            End Get
            Set(value As List(Of QuickQuoteExclusion))
                _Exclusions = value
            End Set
        End Property
        Public Property FacultativeReinsurance As Boolean
            Get
                Return _FacultativeReinsurance
            End Get
            Set(value As Boolean)
                _FacultativeReinsurance = value
            End Set
        End Property
        Public Property FarmIncidentalLimitCoverages As List(Of QuickQuoteCoverage)
            Get
                Return _FarmIncidentalLimitCoverages
            End Get
            Set(value As List(Of QuickQuoteCoverage))
                _FarmIncidentalLimitCoverages = value
            End Set
        End Property
        Public Property HouseholdMembers As List(Of QuickQuoteHouseholdMember)
            Get
                Return _HouseholdMembers
            End Get
            Set(value As List(Of QuickQuoteHouseholdMember))
                _HouseholdMembers = value
            End Set
        End Property
        Public Property LiabilityOptionId As String
            Get
                Return _LiabilityOptionId
            End Get
            Set(value As String)
                _LiabilityOptionId = value
            End Set
        End Property
        Public Property LimitedPerilsCategoryTypeId As String
            Get
                Return _LimitedPerilsCategoryTypeId
            End Get
            Set(value As String)
                _LimitedPerilsCategoryTypeId = value
            End Set
        End Property
        Public Property Modifiers As Generic.List(Of QuickQuoteModifier)
            Get
                Return _Modifiers
            End Get
            Set(value As Generic.List(Of QuickQuoteModifier))
                _Modifiers = value
            End Set
        End Property
        Public Property NumberOfEmployees As String 'int
            Get
                Return _NumberOfEmployees
            End Get
            Set(value As String)
                _NumberOfEmployees = value
            End Set
        End Property
        'Moved to GoverningState 11/07/2019 bug 33571 MLW
        'Public Property OptionalCoverages As List(Of QuickQuoteOptionalCoverage)
        '    Get
        '        Return _OptionalCoverages
        '    End Get
        '    Set(value As List(Of QuickQuoteOptionalCoverage))
        '        _OptionalCoverages = value
        '    End Set
        'End Property      
        Public Property PackageModificationAssignmentTypeId As String
            Get
                Return _PackageModificationAssignmentTypeId
            End Get
            Set(value As String)
                _PackageModificationAssignmentTypeId = value
                _PackageModificationAssignmentType = ""
                If IsNumeric(_PackageModificationAssignmentTypeId) = True Then
                    Select Case _PackageModificationAssignmentTypeId
                        Case "0"
                            _PackageModificationAssignmentType = "N/A"
                        Case "1"
                            _PackageModificationAssignmentType = "Apartment House Risk"
                        Case "2"
                            _PackageModificationAssignmentType = "Contractors Risk"
                        Case "3"
                            _PackageModificationAssignmentType = "Institutional Risk"
                        Case "4"
                            _PackageModificationAssignmentType = "Industrial and Processing Risk"
                        Case "5"
                            _PackageModificationAssignmentType = "Mercantile Risk"
                        Case "6"
                            _PackageModificationAssignmentType = "Motel/Hotel Risk"
                        Case "7"
                            _PackageModificationAssignmentType = "Office Risk"
                        Case "8"
                            _PackageModificationAssignmentType = "Service Risk"
                    End Select
                End If
            End Set
        End Property
        Public Property PackageModificationAssignmentType As String
            Get
                Return _PackageModificationAssignmentType
            End Get
            Set(value As String)
                _PackageModificationAssignmentType = value
                Select Case _PackageModificationAssignmentType
                    Case "N/A"
                        _PackageModificationAssignmentTypeId = "0"
                    Case "Apartment House Risk"
                        _PackageModificationAssignmentTypeId = "1"
                    Case "Contractors Risk"
                        _PackageModificationAssignmentTypeId = "2"
                    Case "Institutional Risk"
                        _PackageModificationAssignmentTypeId = "3"
                    Case "Industrial and Processing Risk"
                        _PackageModificationAssignmentTypeId = "4"
                    Case "Mercantile Risk"
                        _PackageModificationAssignmentTypeId = "5"
                    Case "Motel/Hotel Risk"
                        _PackageModificationAssignmentTypeId = "6"
                    Case "Office Risk"
                        _PackageModificationAssignmentTypeId = "7"
                    Case "Service Risk"
                        _PackageModificationAssignmentTypeId = "8"
                    Case Else
                        _PackageModificationAssignmentTypeId = ""
                End Select
            End Set
        End Property
        Public Property PackageTypeId As String
            Get
                Return _PackageTypeId
            End Get
            Set(value As String)
                _PackageTypeId = value
                _PackageType = ""
                If IsNumeric(_PackageTypeId) = True Then
                    Select Case _PackageTypeId
                        Case "0"
                            _PackageType = "N/A"
                        Case "1"
                            _PackageType = "CPP"
                        Case "2"
                            _PackageType = "POP"
                    End Select
                End If
            End Set
        End Property
        Public Property PackageType As String
            Get
                Return _PackageType
            End Get
            Set(value As String)
                _PackageType = value
                Select Case _PackageType
                    Case "N/A"
                        _PackageTypeId = "0"
                    Case "CPP"
                        _PackageTypeId = "1"
                    Case "POP"
                        _PackageTypeId = "2"
                    Case Else
                        _PackageTypeId = ""
                End Select
            End Set
        End Property
        Public Property PolicyTypeId As String 'only coded for CPR right now (9/27/2012)
            Get
                Return _PolicyTypeId
            End Get
            Set(value As String)
                _PolicyTypeId = value
                _PolicyType = ""
                If IsNumeric(_PolicyTypeId) = True Then
                    Select Case _PolicyTypeId
                        Case "0"
                            _PolicyType = "N/A"
                        Case "1"
                            _PolicyType = "None"
                        Case "60"
                            _PolicyType = "Standard"
                        Case "61"
                            _PolicyType = "Preferred"
                    End Select
                End If
            End Set
        End Property
        Public Property PolicyType As String 'only coded for CPR right now (9/27/2012)
            Get
                Return _PolicyType
            End Get
            Set(value As String)
                _PolicyType = value
                Select Case _PolicyType
                    Case "N/A"
                        _PolicyTypeId = "0"
                    Case "None"
                        _PolicyTypeId = "1"
                    Case "Standard"
                        _PolicyTypeId = "60"
                    Case "Preferred"
                        _PolicyTypeId = "61"
                    Case Else
                        'won't set _PolicyTypeId = "" for now
                End Select
            End Set
        End Property
        Public Property PolicyUnderwritings As Generic.List(Of QuickQuotePolicyUnderwriting)
            Get
                Return _PolicyUnderwritings
            End Get
            Set(value As Generic.List(Of QuickQuotePolicyUnderwriting))
                _PolicyUnderwritings = value
            End Set
        End Property
        Public Property PolicyUnderwritingCodeAndLevelAndTabIds As Generic.List(Of String)
            Get
                Return _PolicyUnderwritingCodeAndLevelAndTabIds
            End Get
            Set(value As Generic.List(Of String))
                _PolicyUnderwritingCodeAndLevelAndTabIds = value
            End Set
        End Property
        Public Property PriorCarrier As QuickQuotePriorCarrier
            Get
                Return _PriorCarrier
            End Get
            Set(value As QuickQuotePriorCarrier)
                _PriorCarrier = value
            End Set
        End Property
        Public Property ProgramType As String
            Get
                Return _ProgramType
            End Get
            Set(value As String)
                _ProgramType = value
                Select Case _ProgramType
                    Case "Unassigned"
                        _ProgramTypeId = "-1"
                    Case "None"
                        _ProgramTypeId = "0"
                    Case "Homeowners"
                        _ProgramTypeId = "1"
                    Case "Mobile Home"
                        _ProgramTypeId = "2"
                    Case "Dwelling Fire"
                        _ProgramTypeId = "3"
                    Case "Personal Umbrella"
                        _ProgramTypeId = "4"
                    Case "Farm Umbrella"
                        _ProgramTypeId = "5"
                    Case "Farmowners"
                        _ProgramTypeId = "6"
                    Case "Select-O-Matic"
                        _ProgramTypeId = "7"
                    Case "Farm Liability"
                        _ProgramTypeId = "8"
                    Case "N/A"
                        _ProgramTypeId = "9"
                    Case "Coinsurance Only"
                        _ProgramTypeId = "10"
                    Case "Benefits Deductible"
                        _ProgramTypeId = "11"
                    Case "Coinsurance and Deductible"
                        _ProgramTypeId = "12"
                    Case "Dealers"
                        _ProgramTypeId = "13"
                    Case "Non-Dealers"
                        _ProgramTypeId = "14"
                    Case "Commercial Crime"
                        _ProgramTypeId = "48"
                    Case "Government Crime"
                        _ProgramTypeId = "49"
                    Case "Employee Theft & Forgery"
                        _ProgramTypeId = "50"
                    Case "Commercial Umbrella"
                        _ProgramTypeId = "51"
                    Case "BOP - Contractors"
                        _ProgramTypeId = "52"
                    Case "BOP - Other Than Contractors"
                        _ProgramTypeId = "53"
                    Case "CGL - Commercial General Liability - Standard"
                        _ProgramTypeId = "54"
                    Case "CGL - Commercial General Liability - Preferred"
                        _ProgramTypeId = "55"
                    Case "OCP - Owners and Contractors Protective Liability"
                        _ProgramTypeId = "56"
                    Case "Dealers - Unlimited Liability"
                        _ProgramTypeId = "57"
                    Case "Dealers - Limited Liability"
                        _ProgramTypeId = "58"
                    Case "Standard"
                        _ProgramTypeId = "61"
                    Case "Preferred"
                        _ProgramTypeId = "62"
                    Case Else
                        _ProgramTypeId = ""
                End Select
            End Set
        End Property
        Public Property ProgramTypeId As String
            Get
                Return _ProgramTypeId
            End Get
            Set(value As String)
                _ProgramTypeId = value
                _ProgramType = ""
                If IsNumeric(_ProgramTypeId) = True Then
                    Select Case _ProgramTypeId
                        Case "-1"
                            _ProgramType = "Unassigned"
                        Case "0"
                            _ProgramType = "None"
                        Case "1"
                            _ProgramType = "Homeowners"
                        Case "2"
                            _ProgramType = "Mobile Home"
                        Case "3"
                            _ProgramType = "Dwelling Fire"
                        Case "4"
                            _ProgramType = "Personal Umbrella"
                        Case "5"
                            _ProgramType = "Farm Umbrella"
                        Case "6"
                            _ProgramType = "Farmowners"
                        Case "7"
                            _ProgramType = "Select-O-Matic"
                        Case "8"
                            _ProgramType = "Farm Liability"
                        Case "9"
                            _ProgramType = "N/A"
                        Case "10"
                            _ProgramType = "Coinsurance Only"
                        Case "11"
                            _ProgramType = "Benefits Deductible"
                        Case "12"
                            _ProgramType = "Coinsurance and Deductible"
                        Case "13"
                            _ProgramType = "Dealers"
                        Case "14"
                            _ProgramType = "Non-Dealers"
                        Case "48"
                            _ProgramType = "Commercial Crime"
                        Case "49"
                            _ProgramType = "Government Crime"
                        Case "50"
                            _ProgramType = "Employee Theft & Forgery"
                        Case "51"
                            _ProgramType = "Commercial Umbrella"
                        Case "52"
                            _ProgramType = "BOP - Contractors"
                        Case "53"
                            _ProgramType = "BOP - Other Than Contractors"
                        Case "54"
                            _ProgramType = "CGL - Commercial General Liability - Standard"
                        Case "55"
                            _ProgramType = "CGL - Commercial General Liability - Preferred"
                        Case "56"
                            _ProgramType = "OCP - Owners and Contractors Protective Liability"
                        Case "57"
                            _ProgramType = "Dealers - Unlimited Liability"
                        Case "58"
                            _ProgramType = "Dealers - Limited Liability"
                        Case "61"
                            _ProgramType = "Standard"
                        Case "62"
                            _ProgramType = "Preferred"
                    End Select
                End If
            End Set
        End Property
        Public Property RiskGrade As String
            Get
                Return _RiskGrade
            End Get
            Set(value As String)
                _RiskGrade = value
            End Set
        End Property
        Public Property RiskGradeLookupId As String
            Get
                Return _RiskGradeLookupId
            End Get
            Set(value As String)
                _RiskGradeLookupId = value
            End Set
        End Property
        'Public Property ScheduledPersonalPropertyCoverages As List(Of QuickQuoteScheduledPersonalPropertyCoverage) 'moved to GoverningState 2/5/2019
        '    Get
        '        Return _ScheduledPersonalPropertyCoverages
        '    End Get
        '    Set(value As List(Of QuickQuoteScheduledPersonalPropertyCoverage))
        '        _ScheduledPersonalPropertyCoverages = value
        '    End Set
        'End Property
        Public Property ScheduledRatings As Generic.List(Of QuickQuoteScheduledRating)
            Get
                Return _ScheduledRatings
            End Get
            Set(value As Generic.List(Of QuickQuoteScheduledRating))
                _ScheduledRatings = value
            End Set
        End Property
        Public Property SelectMarketCredit As Boolean
            Get
                Return _SelectMarketCredit
            End Get
            Set(value As Boolean)
                _SelectMarketCredit = value
            End Set
        End Property
        Public Property ThirdPartyData As QuickQuoteThirdPartyData
            Get
                Return _ThirdPartyData
            End Get
            Set(value As QuickQuoteThirdPartyData)
                _ThirdPartyData = value
            End Set
        End Property
        Public Property TierTypeId As String 'N/A=-1; None=0; Uniform=1; Variable=2
            Get
                Return _TierTypeId
            End Get
            Set(value As String)
                _TierTypeId = value
            End Set
        End Property
        Public Property TieringInformation As QuickQuoteTieringInformation
            Get
                Return _TieringInformation
            End Get
            Set(value As QuickQuoteTieringInformation)
                _TieringInformation = value
            End Set
        End Property
        'Public Property UnscheduledPersonalPropertyCoverage As QuickQuoteUnscheduledPersonalPropertyCoverage 'moved to GoverningState 2/5/2019
        '    Get
        '        Return _UnscheduledPersonalPropertyCoverage
        '    End Get
        '    Set(value As QuickQuoteUnscheduledPersonalPropertyCoverage)
        '        _UnscheduledPersonalPropertyCoverage = value
        '    End Set
        'End Property
        Public Property ResidenceInfo As QuickQuoteResidenceInfo
            Get
                Return _ResidenceInfo
            End Get
            Set(value As QuickQuoteResidenceInfo)
                _ResidenceInfo = value
            End Set
        End Property

        'added 4/20/2020 for PUP/FUP
        Public Property FarmSizeTypeId As String 'static data
            Get
                Return _FarmSizeTypeId
            End Get
            Set(value As String)
                _FarmSizeTypeId = value
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
        
        'added 5/6/2021 KLJ
        'Need this for FUP/PUP but it is ignored in all other cases
        Public Property ManualAggregateLiabilityLimit As String
        
        Public Sub New()
            MyBase.New()
            SetDefaults()
        End Sub
        'Public Sub New(Parent As QuickQuoteObject) 'added 6/27/2018; could probably just use generic type so one constructor could be used for multiple types; removed 7/27/2018 in lieu of new generic constructor
        '    MyBase.New()
        '    SetDefaults()
        '    Me.SetParent = Parent
        'End Sub
        'Public Sub New(Parent As QuickQuotePackagePart) 'added 6/27/2018; could probably just use generic type so one constructor could be used for multiple types; removed 7/27/2018 in lieu of new generic constructor
        '    MyBase.New()
        '    SetDefaults()
        '    Me.SetParent = Parent
        'End Sub
        Public Sub New(Parent As Object) 'added 7/27/2018 to replace multiple constructors for different objects
            MyBase.New()
            SetDefaults()
            Me.SetParent = Parent
        End Sub
        Private Sub SetDefaults()
            '_FullTermPremium = ""
            _PolicyId = ""
            _PolicyImageNum = ""
            '_AddFormsVersionId = ""
            '_Num = ""
            '_PackagePartTypeId = ""
            '_PackagePartType = ""
            '_RatingVersionId = ""
            '_UnderwritingVersionId = ""
            '_VersionId = ""

            'PolicyLevel
            _AggregateLiabilityIncrementTypeId = ""
            _AggregateLimit = ""
            _AnniversaryRatingEffectiveDate = ""
            _AnniversaryRatingExpirationDate = ""
            _AutoHome = False
            _AutoSymbols = Nothing
            _BlanketRatingOptionId = ""
            _DeductiblePerTypeId = ""
            _DeductiblePerType = ""
            _DrivecamContractEffectiveDate = "" '/DateTime; may not be needed... identified in xml but not UI
            _EmployeeDiscount = False
            _EmployeesFullTime = ""
            _EmployeesPartTime1To40Days = ""
            _EmployeesPartTime41To179Days = ""
            _EntityTypeId = ""
            _Exclusions = Nothing
            _FacultativeReinsurance = False
            _FarmIncidentalLimitCoverages = Nothing
            _HouseholdMembers = Nothing
            _LiabilityOptionId = ""
            _LimitedPerilsCategoryTypeId = ""
            _Modifiers = Nothing
            _NumberOfEmployees = ""
            '_OptionalCoverages = Nothing 'Moved to GoverningState 11/07/2019 bug 33571 MLW
            _PackageModificationAssignmentTypeId = ""
            _PackageModificationAssignmentType = ""
            _PackageTypeId = ""
            _PackageType = ""
            _PolicyTypeId = ""
            _PolicyType = ""
            _PolicyUnderwritings = Nothing
            _PolicyUnderwritingCodeAndLevelAndTabIds = Nothing
            _PriorCarrier = New QuickQuotePriorCarrier
            _ProgramType = ""
            _ProgramTypeId = ""
            _RiskGrade = ""
            _RiskGradeLookupId = ""
            '_ScheduledPersonalPropertyCoverages = Nothing 'moved to GoverningState 2/5/2019
            _ScheduledRatings = Nothing
            _SelectMarketCredit = False
            _ThirdPartyData = New QuickQuoteThirdPartyData
            _TierTypeId = ""
            _TieringInformation = New QuickQuoteTieringInformation
            '_UnscheduledPersonalPropertyCoverage = Nothing 'moved to GoverningState 2/5/2019
            _ResidenceInfo = New QuickQuoteResidenceInfo

            'added 4/20/2020 for PUP/FUP
            _FarmSizeTypeId = "" 'static data
            _FarmTypeId = "" 'static data
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
                    'qqHelper.DisposeString(_FullTermPremium)
                    qqHelper.DisposeString(_PolicyId)
                    qqHelper.DisposeString(_PolicyImageNum)
                    'qqHelper.DisposeString(_AddFormsVersionId)
                    'qqHelper.DisposeString(_Num)
                    'qqHelper.DisposeString(_PackagePartTypeId)
                    'qqHelper.DisposeString(_PackagePartType)
                    'qqHelper.DisposeString(_RatingVersionId)
                    'qqHelper.DisposeString(_UnderwritingVersionId)
                    'qqHelper.DisposeString(_VersionId)

                    'PolicyLevel
                    qqHelper.DisposeString(_AggregateLiabilityIncrementTypeId)
                    qqHelper.DisposeString(_AggregateLimit)
                    qqHelper.DisposeString(_AnniversaryRatingEffectiveDate)
                    qqHelper.DisposeString(_AnniversaryRatingExpirationDate)
                    _AutoHome = Nothing
                    If _AutoSymbols IsNot Nothing Then
                        If _AutoSymbols.Count > 0 Then
                            For Each s As QuickQuoteAutoSymbol In _AutoSymbols
                                s.Dispose()
                                s = Nothing
                            Next
                            _AutoSymbols.Clear()
                        End If
                        _AutoSymbols = Nothing
                    End If
                    qqHelper.DisposeString(_BlanketRatingOptionId)
                    qqHelper.DisposeString(_DeductiblePerTypeId)
                    qqHelper.DisposeString(_DeductiblePerType)
                    qqHelper.DisposeString(_DrivecamContractEffectiveDate) '/DateTime; may not be needed... identified in xml but not UI
                    _EmployeeDiscount = Nothing
                    qqHelper.DisposeString(_EmployeesFullTime)
                    qqHelper.DisposeString(_EmployeesPartTime1To40Days)
                    qqHelper.DisposeString(_EmployeesPartTime41To179Days)
                    qqHelper.DisposeString(_EntityTypeId)
                    If _Exclusions IsNot Nothing Then
                        If _Exclusions.Count > 0 Then
                            For Each ex As QuickQuoteExclusion In _Exclusions
                                ex.Dispose()
                                ex = Nothing
                            Next
                            _Exclusions.Clear()
                        End If
                        _Exclusions = Nothing
                    End If
                    _FacultativeReinsurance = Nothing
                    If _FarmIncidentalLimitCoverages IsNot Nothing Then
                        If _FarmIncidentalLimitCoverages.Count > 0 Then
                            For Each c As QuickQuoteCoverage In _FarmIncidentalLimitCoverages
                                c.Dispose()
                                c = Nothing
                            Next
                            _FarmIncidentalLimitCoverages.Clear()
                        End If
                        _FarmIncidentalLimitCoverages = Nothing
                    End If
                    If _HouseholdMembers IsNot Nothing Then
                        If _HouseholdMembers.Count > 0 Then
                            For Each m As QuickQuoteHouseholdMember In _HouseholdMembers
                                m.Dispose()
                                m = Nothing
                            Next
                            _HouseholdMembers.Clear()
                        End If
                        _HouseholdMembers = Nothing
                    End If
                    qqHelper.DisposeString(_LiabilityOptionId)
                    qqHelper.DisposeString(_LimitedPerilsCategoryTypeId)
                    If _Modifiers IsNot Nothing Then
                        If _Modifiers.Count > 0 Then
                            For Each m As QuickQuoteModifier In _Modifiers
                                m.Dispose()
                                m = Nothing
                            Next
                            _Modifiers.Clear()
                        End If
                        _Modifiers = Nothing
                    End If
                    qqHelper.DisposeString(_NumberOfEmployees)
                    'Moved to GoverningState 11/07/2019 bug 33571 MLW
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
                    qqHelper.DisposeString(_PackageModificationAssignmentTypeId)
                    qqHelper.DisposeString(_PackageModificationAssignmentType)
                    qqHelper.DisposeString(_PackageTypeId)
                    qqHelper.DisposeString(_PackageType)
                    qqHelper.DisposeString(_PolicyTypeId)
                    qqHelper.DisposeString(_PolicyType)
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
                    If _PolicyUnderwritingCodeAndLevelAndTabIds IsNot Nothing Then
                        If _PolicyUnderwritingCodeAndLevelAndTabIds.Count > 0 Then
                            For Each codeId As String In _PolicyUnderwritingCodeAndLevelAndTabIds
                                codeId = Nothing
                            Next
                            _PolicyUnderwritingCodeAndLevelAndTabIds.Clear()
                        End If
                        _PolicyUnderwritingCodeAndLevelAndTabIds = Nothing
                    End If
                    If _PriorCarrier IsNot Nothing Then
                        _PriorCarrier.Dispose()
                        _PriorCarrier = Nothing
                    End If
                    qqHelper.DisposeString(_ProgramType)
                    qqHelper.DisposeString(_ProgramTypeId)
                    qqHelper.DisposeString(_RiskGrade)
                    qqHelper.DisposeString(_RiskGradeLookupId)
                    'If _ScheduledPersonalPropertyCoverages IsNot Nothing Then 'moved to GoverningState 2/5/2019
                    '    If _ScheduledPersonalPropertyCoverages.Count > 0 Then
                    '        For Each sp As QuickQuoteScheduledPersonalPropertyCoverage In _ScheduledPersonalPropertyCoverages
                    '            sp.Dispose()
                    '            sp = Nothing
                    '        Next
                    '        _ScheduledPersonalPropertyCoverages.Clear()
                    '    End If
                    '    _ScheduledPersonalPropertyCoverages = Nothing
                    'End If
                    If _ScheduledRatings IsNot Nothing Then
                        If _ScheduledRatings.Count > 0 Then
                            For Each sr As QuickQuoteScheduledRating In _ScheduledRatings
                                sr.Dispose()
                                sr = Nothing
                            Next
                            _ScheduledRatings.Clear()
                        End If
                        _ScheduledRatings = Nothing
                    End If
                    _SelectMarketCredit = Nothing
                    If _ThirdPartyData IsNot Nothing Then
                        _ThirdPartyData.Dispose()
                        _ThirdPartyData = Nothing
                    End If
                    qqHelper.DisposeString(_TierTypeId)
                    If _TieringInformation IsNot Nothing Then
                        _TieringInformation.Dispose()
                        _TieringInformation = Nothing
                    End If
                    'If _UnscheduledPersonalPropertyCoverage IsNot Nothing Then 'moved to GoverningState 2/5/2019
                    '    _UnscheduledPersonalPropertyCoverage.Dispose()
                    '    _UnscheduledPersonalPropertyCoverage = Nothing
                    'End If
                    If _ResidenceInfo IsNot Nothing Then
                        _ResidenceInfo.Dispose()
                        _ResidenceInfo = Nothing
                    End If

                    'added 4/20/2020 for PUP/FUP
                    qqHelper.DisposeString(_FarmSizeTypeId) 'static data
                    qqHelper.DisposeString(_FarmTypeId) 'static data

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

