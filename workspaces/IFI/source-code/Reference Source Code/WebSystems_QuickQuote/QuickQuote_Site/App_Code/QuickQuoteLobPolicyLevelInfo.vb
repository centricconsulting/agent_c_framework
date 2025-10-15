Imports System.Web
Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    ''' <summary>
    ''' object used to store policy-level lob-specific information for a quote
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()>
    Public Class QuickQuoteLobPolicyLevelInfo 'added 7/23/2018
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        '8/18/2018 note: this class is no longer being used since it was broken up into smaller objects, which are now used from the smaller extended objects

        Dim qqHelper As New QuickQuoteHelperClass

        'Private _FullTermPremium As String
        'Private _PolicyId As String 'PL; removed 8/16/2018; updated Properties to point to sub objects instead of private variables
        'Private _PolicyImageNum As String 'PL; removed 8/16/2018; updated Properties to point to sub objects instead of private variables
        'Private _AddFormsVersionId As String
        'Private _Num As String
        'Private _PackagePartTypeId As String
        'Private _PackagePartType As String
        'Private _RatingVersionId As String
        'Private _UnderwritingVersionId As String
        'Private _VersionId As String

        'PolicyLevel; removed 8/16/2018; updated Properties to point to sub objects instead of private variables
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
        'Private _ResidenceInfo As QuickQuoteResidenceInfo

        'added 8/16/2018
        Private _AppliedToGoverningState As QuickQuoteLobPolicyLevelInfo_AppliedToGoverningState
        Private _AppliedToAllStates As QuickQuoteLobPolicyLevelInfo_AppliedToAllStates
        Private _AppliedToIndividualState As QuickQuoteLobPolicyLevelInfo_AppliedToIndividualState

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
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PolicyId As String
            Get
                Return AppliedToAllStates.PolicyId
            End Get
            Set(value As String)
                AppliedToAllStates.PolicyId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PolicyImageNum As String
            Get
                Return AppliedToAllStates.PolicyImageNum
            End Get
            Set(value As String)
                AppliedToAllStates.PolicyImageNum = value
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
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property AdditionalInterestListLinks As List(Of QuickQuoteAdditionalInterestListLink)
            Get
                Return AppliedToGoverningState.AdditionalInterestListLinks
            End Get
            Set(value As List(Of QuickQuoteAdditionalInterestListLink))
                AppliedToGoverningState.AdditionalInterestListLinks = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property AdditionalInterests As List(Of QuickQuoteAdditionalInterest)
            Get
                Return AppliedToGoverningState.AdditionalInterests
            End Get
            Set(value As List(Of QuickQuoteAdditionalInterest))
                AppliedToGoverningState.AdditionalInterests = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property AggregateLiabilityIncrementTypeId As String
            Get
                Return AppliedToAllStates.AggregateLiabilityIncrementTypeId
            End Get
            Set(value As String)
                AppliedToAllStates.AggregateLiabilityIncrementTypeId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property AggregateLimit As String 'decimal
            Get
                Return AppliedToAllStates.AggregateLimit
            End Get
            Set(value As String)
                AppliedToAllStates.AggregateLimit = value
                'qqHelper.ConvertToLimitFormat(_AggregateLimit)
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property AnniversaryRatingEffectiveDate As String
            Get
                Return AppliedToAllStates.AnniversaryRatingEffectiveDate
            End Get
            Set(value As String)
                AppliedToAllStates.AnniversaryRatingEffectiveDate = value
                'qqHelper.ConvertToShortDate(_AnniversaryRatingEffectiveDate)
                'If IsDate(_AnniversaryRatingEffectiveDate) = True Then
                'AnniversaryRatingExpirationDate = DateAdd(DateInterval.Year, 1, CDate(_AnniversaryRatingEffectiveDate))
                'End If
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property AnniversaryRatingExpirationDate As String
            Get
                Return AppliedToAllStates.AnniversaryRatingExpirationDate
            End Get
            Set(value As String)
                AppliedToAllStates.AnniversaryRatingExpirationDate = value
                'qqHelper.ConvertToShortDate(_AnniversaryRatingExpirationDate)
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property AutoHome As Boolean
            Get
                Return AppliedToAllStates.AutoHome
            End Get
            Set(value As Boolean)
                AppliedToAllStates.AutoHome = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property AutoSymbols As Generic.List(Of QuickQuoteAutoSymbol)
            Get
                Return AppliedToAllStates.AutoSymbols
            End Get
            Set(value As Generic.List(Of QuickQuoteAutoSymbol))
                AppliedToAllStates.AutoSymbols = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property BlanketRatingOption As String
            Get
                Return AppliedToAllStates.BlanketRatingOption
            End Get
            Set(value As String)
                AppliedToAllStates.BlanketRatingOption = value
                'Select Case _BlanketRatingOption
                'Case "N/A"
                '_BlanketRatingOptionId = "0"
                'Case "Combined Building and Personal Property"
                '_BlanketRatingOptionId = "1"
                'Case "Building Only"
                '_BlanketRatingOptionId = "2"
                'Case "Personal Property Only"
                '_BlanketRatingOptionId = "3"
                'Case Else
                '_BlanketRatingOptionId = ""
                'End Select
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property BlanketRatingOptionId As String 'verified in database 7/3/2012
            Get
                Return AppliedToAllStates.BlanketRatingOptionId
            End Get
            Set(value As String)
                AppliedToAllStates.BlanketRatingOptionId = value
                '(0=N/A; 1=Combined Building and Personal Property; 2=Building Only; 3=Personal Property Only)
                '_BlanketRatingOption = ""
                'If IsNumeric(_BlanketRatingOptionId) = True Then
                'Select Case _BlanketRatingOptionId
                'Case "0"
                '_BlanketRatingOption = "N/A"
                'Case "1"
                '_BlanketRatingOption = "Combined Building and Personal Property"
                'Case "2"
                '_BlanketRatingOption = "Building Only"
                'Case "3"
                '_BlanketRatingOption = "Personal Property Only"
                'End Select
                'End If
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property ClassificationCodes As List(Of QuickQuoteClassificationCode)
            Get
                Return AppliedToGoverningState.ClassificationCodes '8/19/2018 - moved from IndividualState to GoverningState because it's CRM; originally thought it was for WCP Classes, which are at Location.Classifications
            End Get
            Set(value As List(Of QuickQuoteClassificationCode))
                AppliedToGoverningState.ClassificationCodes = value '8/19/2018 - moved from IndividualState to GoverningState because it's CRM; originally thought it was for WCP Classes, which are at Location.Classifications
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Coverages As Generic.List(Of QuickQuoteCoverage)
            Get
                Return AppliedToIndividualState.Coverages
            End Get
            Set(value As Generic.List(Of QuickQuoteCoverage))
                AppliedToIndividualState.Coverages = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property DeductiblePerTypeId As String
            Get
                Return AppliedToAllStates.DeductiblePerTypeId
            End Get
            Set(value As String)
                AppliedToAllStates.DeductiblePerTypeId = value
                '(0-N/A; 1=Per Occurrence; 2=Per Claim)
                '_DeductiblePerType = ""
                'If IsNumeric(_DeductiblePerTypeId) = True Then
                'Select Case _DeductiblePerTypeId
                'Case "0"
                '_DeductiblePerType = "N/A"
                'Case "1"
                '_DeductiblePerType = "Per Occurrence"
                'Case "2"
                '_DeductiblePerType = "Per Claim"
                'End Select
                'End If
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property DeductiblePerType As String
            Get
                Return AppliedToAllStates.DeductiblePerType
            End Get
            Set(value As String)
                AppliedToAllStates.DeductiblePerType = value
                'Select Case _DeductiblePerType
                'Case "N/A"
                '_DeductiblePerTypeId = "0"
                'Case "Per Occurrence"
                '_DeductiblePerTypeId = "1"
                'Case "Per Claim"
                '_DeductiblePerTypeId = "2"
                'Case Else
                '_DeductiblePerTypeId = ""
                'End Select
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property DrivecamContractEffectiveDate As String '/DateTime; may not be needed... identified in xml but not UI
            Get
                Return AppliedToAllStates.DrivecamContractEffectiveDate
            End Get
            Set(value As String)
                AppliedToAllStates.DrivecamContractEffectiveDate = value
                'qqHelper.ConvertToShortDate(_DrivecamContractEffectiveDate)
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property EmployeeDiscount As Boolean
            Get
                Return AppliedToAllStates.EmployeeDiscount
            End Get
            Set(value As Boolean)
                AppliedToAllStates.EmployeeDiscount = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property EmployeesFullTime As String
            Get
                Return AppliedToAllStates.EmployeesFullTime
            End Get
            Set(value As String)
                AppliedToAllStates.EmployeesFullTime = value 'could add numeric formatting... maybe limit formatting (whole #)
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property EmployeesPartTime1To40Days As String
            Get
                Return AppliedToAllStates.EmployeesPartTime1To40Days
            End Get
            Set(value As String)
                AppliedToAllStates.EmployeesPartTime1To40Days = value 'could add numeric formatting... maybe limit formatting (whole #)
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property EmployeesPartTime41To179Days As String
            Get
                Return AppliedToAllStates.EmployeesPartTime41To179Days
            End Get
            Set(value As String)
                AppliedToAllStates.EmployeesPartTime41To179Days = value 'could add numeric formatting... maybe limit formatting (whole #)
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property EntityTypeId As String
            Get
                Return AppliedToAllStates.EntityTypeId
            End Get
            Set(value As String)
                AppliedToAllStates.EntityTypeId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Exclusions As List(Of QuickQuoteExclusion)
            Get
                Return AppliedToAllStates.Exclusions
            End Get
            Set(value As List(Of QuickQuoteExclusion))
                AppliedToAllStates.Exclusions = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property FacultativeReinsurance As Boolean
            Get
                Return AppliedToAllStates.FacultativeReinsurance
            End Get
            Set(value As Boolean)
                AppliedToAllStates.FacultativeReinsurance = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property FarmIncidentalLimitCoverages As List(Of QuickQuoteCoverage)
            Get
                Return AppliedToAllStates.FarmIncidentalLimitCoverages
            End Get
            Set(value As List(Of QuickQuoteCoverage))
                AppliedToAllStates.FarmIncidentalLimitCoverages = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property GLClassifications As Generic.List(Of QuickQuoteGLClassification)
            Get
                Return AppliedToIndividualState.GLClassifications
            End Get
            Set(value As Generic.List(Of QuickQuoteGLClassification))
                AppliedToIndividualState.GLClassifications = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property HouseholdMembers As List(Of QuickQuoteHouseholdMember)
            Get
                Return AppliedToAllStates.HouseholdMembers
            End Get
            Set(value As List(Of QuickQuoteHouseholdMember))
                AppliedToAllStates.HouseholdMembers = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property InclusionsExclusions As Generic.List(Of QuickQuoteInclusionExclusion)
            Get
                Return AppliedToIndividualState.InclusionsExclusions
            End Get
            Set(value As Generic.List(Of QuickQuoteInclusionExclusion))
                AppliedToIndividualState.InclusionsExclusions = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property LiabilityOptionId As String
            Get
                Return AppliedToAllStates.LiabilityOptionId
            End Get
            Set(value As String)
                AppliedToAllStates.LiabilityOptionId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property LimitedPerilsCategoryTypeId As String
            Get
                Return AppliedToAllStates.LimitedPerilsCategoryTypeId
            End Get
            Set(value As String)
                AppliedToAllStates.LimitedPerilsCategoryTypeId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property LossHistoryRecords As Generic.List(Of QuickQuoteLossHistoryRecord)
            Get
                Return AppliedToGoverningState.LossHistoryRecords
            End Get
            Set(value As Generic.List(Of QuickQuoteLossHistoryRecord))
                AppliedToGoverningState.LossHistoryRecords = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Modifiers As Generic.List(Of QuickQuoteModifier)
            Get
                Return AppliedToAllStates.Modifiers
            End Get
            Set(value As Generic.List(Of QuickQuoteModifier))
                AppliedToAllStates.Modifiers = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property NumberOfEmployees As String 'int
            Get
                Return AppliedToAllStates.NumberOfEmployees
            End Get
            Set(value As String)
                AppliedToAllStates.NumberOfEmployees = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property OptionalCoverages As List(Of QuickQuoteOptionalCoverage)
            Get
                'Moved to GoverningState 11/07/2019 bug 33571 MLW
                'Return AppliedToAllStates.OptionalCoverages
                Return AppliedToGoverningState.OptionalCoverages
            End Get
            Set(value As List(Of QuickQuoteOptionalCoverage))
                'Moved to GoverningState 11/07/2019 bug 33571 MLW
                'AppliedToAllStates.OptionalCoverages = value
                AppliedToGoverningState.OptionalCoverages = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PackageModificationAssignmentTypeId As String
            Get
                Return AppliedToAllStates.PackageModificationAssignmentTypeId
            End Get
            Set(value As String)
                AppliedToAllStates.PackageModificationAssignmentTypeId = value
                '_PackageModificationAssignmentType = ""
                'If IsNumeric(_PackageModificationAssignmentTypeId) = True Then
                'Select Case _PackageModificationAssignmentTypeId
                'Case "0"
                '_PackageModificationAssignmentType = "N/A"
                'Case "1"
                '_PackageModificationAssignmentType = "Apartment House Risk"
                'Case "2"
                '_PackageModificationAssignmentType = "Contractors Risk"
                'Case "3"
                '_PackageModificationAssignmentType = "Institutional Risk"
                'Case "4"
                '_PackageModificationAssignmentType = "Industrial and Processing Risk"
                'Case "5"
                '_PackageModificationAssignmentType = "Mercantile Risk"
                'Case "6"
                '_PackageModificationAssignmentType = "Motel/Hotel Risk"
                'Case "7"
                '_PackageModificationAssignmentType = "Office Risk"
                'Case "8"
                '_PackageModificationAssignmentType = "Service Risk"
                'End Select
                'End If
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PackageModificationAssignmentType As String
            Get
                Return AppliedToAllStates.PackageModificationAssignmentType
            End Get
            Set(value As String)
                AppliedToAllStates.PackageModificationAssignmentType = value
                'Select Case _PackageModificationAssignmentType
                'Case "N/A"
                '_PackageModificationAssignmentTypeId = "0"
                'Case "Apartment House Risk"
                '_PackageModificationAssignmentTypeId = "1"
                'Case "Contractors Risk"
                '_PackageModificationAssignmentTypeId = "2"
                'Case "Institutional Risk"
                '_PackageModificationAssignmentTypeId = "3"
                'Case "Industrial and Processing Risk"
                '_PackageModificationAssignmentTypeId = "4"
                'Case "Mercantile Risk"
                '_PackageModificationAssignmentTypeId = "5"
                'Case "Motel/Hotel Risk"
                '_PackageModificationAssignmentTypeId = "6"
                'Case "Office Risk"
                '_PackageModificationAssignmentTypeId = "7"
                'Case "Service Risk"
                '_PackageModificationAssignmentTypeId = "8"
                'Case Else
                '_PackageModificationAssignmentTypeId = ""
                'End Select
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PackageTypeId As String
            Get
                Return AppliedToAllStates.PackageTypeId
            End Get
            Set(value As String)
                AppliedToAllStates.PackageTypeId = value
                '_PackageType = ""
                'If IsNumeric(_PackageTypeId) = True Then
                'Select Case _PackageTypeId
                'Case "0"
                '_PackageType = "N/A"
                'Case "1"
                '_PackageType = "CPP"
                'Case "2"
                '_PackageType = "POP"
                'End Select
                'End If
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PackageType As String
            Get
                Return AppliedToAllStates.PackageType
            End Get
            Set(value As String)
                AppliedToAllStates.PackageType = value
                'Select Case _PackageType
                'Case "N/A"
                '_PackageTypeId = "0"
                'Case "CPP"
                '_PackageTypeId = "1"
                'Case "POP"
                '_PackageTypeId = "2"
                'Case Else
                '_PackageTypeId = ""
                'End Select
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PolicyTypeId As String 'only coded for CPR right now (9/27/2012)
            Get
                Return AppliedToAllStates.PolicyTypeId
            End Get
            Set(value As String)
                AppliedToAllStates.PolicyTypeId = value
                '_PolicyType = ""
                'If IsNumeric(_PolicyTypeId) = True Then
                'Select Case _PolicyTypeId
                'Case "0"
                '_PolicyType = "N/A"
                'Case "1"
                '_PolicyType = "None"
                'Case "60"
                '_PolicyType = "Standard"
                'Case "61"
                '_PolicyType = "Preferred"
                'End Select
                'End If
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PolicyType As String 'only coded for CPR right now (9/27/2012)
            Get
                Return AppliedToAllStates.PolicyType
            End Get
            Set(value As String)
                AppliedToAllStates.PolicyType = value
                'Select Case _PolicyType
                'Case "N/A"
                '_PolicyTypeId = "0"
                'Case "None"
                '_PolicyTypeId = "1"
                'Case "Standard"
                '_PolicyTypeId = "60"
                'Case "Preferred"
                '_PolicyTypeId = "61"
                'Case Else
                ''won't set _PolicyTypeId = "" for now
                'End Select
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PolicyUnderwritings As Generic.List(Of QuickQuotePolicyUnderwriting)
            Get
                Return AppliedToAllStates.PolicyUnderwritings
            End Get
            Set(value As Generic.List(Of QuickQuotePolicyUnderwriting))
                AppliedToAllStates.PolicyUnderwritings = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PolicyUnderwritingCodeAndLevelAndTabIds As Generic.List(Of String)
            Get
                Return AppliedToAllStates.PolicyUnderwritingCodeAndLevelAndTabIds
            End Get
            Set(value As Generic.List(Of String))
                AppliedToAllStates.PolicyUnderwritingCodeAndLevelAndTabIds = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PriorCarrier As QuickQuotePriorCarrier
            Get
                Return AppliedToAllStates.PriorCarrier
            End Get
            Set(value As QuickQuotePriorCarrier)
                AppliedToAllStates.PriorCarrier = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property ProgramType As String
            Get
                Return AppliedToAllStates.ProgramType
            End Get
            Set(value As String)
                AppliedToAllStates.ProgramType = value
                'Select Case _ProgramType
                '    Case "Unassigned"
                '        _ProgramTypeId = "-1"
                '    Case "None"
                '        _ProgramTypeId = "0"
                '    Case "Homeowners"
                '        _ProgramTypeId = "1"
                '    Case "Mobile Home"
                '        _ProgramTypeId = "2"
                '    Case "Dwelling Fire"
                '        _ProgramTypeId = "3"
                '    Case "Personal Umbrella"
                '        _ProgramTypeId = "4"
                '    Case "Farm Umbrella"
                '        _ProgramTypeId = "5"
                '    Case "Farmowners"
                '        _ProgramTypeId = "6"
                '    Case "Select-O-Matic"
                '        _ProgramTypeId = "7"
                '    Case "Farm Liability"
                '        _ProgramTypeId = "8"
                '    Case "N/A"
                '        _ProgramTypeId = "9"
                '    Case "Coinsurance Only"
                '        _ProgramTypeId = "10"
                '    Case "Benefits Deductible"
                '        _ProgramTypeId = "11"
                '    Case "Coinsurance and Deductible"
                '        _ProgramTypeId = "12"
                '    Case "Dealers"
                '        _ProgramTypeId = "13"
                '    Case "Non-Dealers"
                '        _ProgramTypeId = "14"
                '    Case "Commercial Crime"
                '        _ProgramTypeId = "48"
                '    Case "Government Crime"
                '        _ProgramTypeId = "49"
                '    Case "Employee Theft & Forgery"
                '        _ProgramTypeId = "50"
                '    Case "Commercial Umbrella"
                '        _ProgramTypeId = "51"
                '    Case "BOP - Contractors"
                '        _ProgramTypeId = "52"
                '    Case "BOP - Other Than Contractors"
                '        _ProgramTypeId = "53"
                '    Case "CGL - Commercial General Liability - Standard"
                '        _ProgramTypeId = "54"
                '    Case "CGL - Commercial General Liability - Preferred"
                '        _ProgramTypeId = "55"
                '    Case "OCP - Owners and Contractors Protective Liability"
                '        _ProgramTypeId = "56"
                '    Case "Dealers - Unlimited Liability"
                '        _ProgramTypeId = "57"
                '    Case "Dealers - Limited Liability"
                '        _ProgramTypeId = "58"
                '    Case Else
                '        _ProgramTypeId = ""
                'End Select
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property ProgramTypeId As String
            Get
                Return AppliedToAllStates.ProgramTypeId
            End Get
            Set(value As String)
                AppliedToAllStates.ProgramTypeId = value
                '_ProgramType = ""
                'If IsNumeric(_ProgramTypeId) = True Then
                '    Select Case _ProgramTypeId
                '        Case "-1"
                '            _ProgramType = "Unassigned"
                '        Case "0"
                '            _ProgramType = "None"
                '        Case "1"
                '            _ProgramType = "Homeowners"
                '        Case "2"
                '            _ProgramType = "Mobile Home"
                '        Case "3"
                '            _ProgramType = "Dwelling Fire"
                '        Case "4"
                '            _ProgramType = "Personal Umbrella"
                '        Case "5"
                '            _ProgramType = "Farm Umbrella"
                '        Case "6"
                '            _ProgramType = "Farmowners"
                '        Case "7"
                '            _ProgramType = "Select-O-Matic"
                '        Case "8"
                '            _ProgramType = "Farm Liability"
                '        Case "9"
                '            _ProgramType = "N/A"
                '        Case "10"
                '            _ProgramType = "Coinsurance Only"
                '        Case "11"
                '            _ProgramType = "Benefits Deductible"
                '        Case "12"
                '            _ProgramType = "Coinsurance and Deductible"
                '        Case "13"
                '            _ProgramType = "Dealers"
                '        Case "14"
                '            _ProgramType = "Non-Dealers"
                '        Case "48"
                '            _ProgramType = "Commercial Crime"
                '        Case "49"
                '            _ProgramType = "Government Crime"
                '        Case "50"
                '            _ProgramType = "Employee Theft & Forgery"
                '        Case "51"
                '            _ProgramType = "Commercial Umbrella"
                '        Case "52"
                '            _ProgramType = "BOP - Contractors"
                '        Case "53"
                '            _ProgramType = "BOP - Other Than Contractors"
                '        Case "54"
                '            _ProgramType = "CGL - Commercial General Liability - Standard"
                '        Case "55"
                '            _ProgramType = "CGL - Commercial General Liability - Preferred"
                '        Case "56"
                '            _ProgramType = "OCP - Owners and Contractors Protective Liability"
                '        Case "57"
                '            _ProgramType = "Dealers - Unlimited Liability"
                '        Case "58"
                '            _ProgramType = "Dealers - Limited Liability"
                '    End Select
                'End If
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property RiskGrade As String
            Get
                Return AppliedToAllStates.RiskGrade
            End Get
            Set(value As String)
                AppliedToAllStates.RiskGrade = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property RiskGradeLookupId As String
            Get
                Return AppliedToAllStates.RiskGradeLookupId
            End Get
            Set(value As String)
                AppliedToAllStates.RiskGradeLookupId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property ScheduledCoverages As List(Of QuickQuoteScheduledCoverage)
            Get
                Return AppliedToIndividualState.ScheduledCoverages
            End Get
            Set(value As List(Of QuickQuoteScheduledCoverage))
                AppliedToIndividualState.ScheduledCoverages = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property ScheduledPersonalPropertyCoverages As List(Of QuickQuoteScheduledPersonalPropertyCoverage)
            Get
                Return AppliedToGoverningState.ScheduledPersonalPropertyCoverages
            End Get
            Set(value As List(Of QuickQuoteScheduledPersonalPropertyCoverage))
                AppliedToGoverningState.ScheduledPersonalPropertyCoverages = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property ScheduledRatings As Generic.List(Of QuickQuoteScheduledRating)
            Get
                Return AppliedToAllStates.ScheduledRatings
            End Get
            Set(value As Generic.List(Of QuickQuoteScheduledRating))
                AppliedToAllStates.ScheduledRatings = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property SelectMarketCredit As Boolean
            Get
                Return AppliedToAllStates.SelectMarketCredit
            End Get
            Set(value As Boolean)
                AppliedToAllStates.SelectMarketCredit = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property ThirdPartyData As QuickQuoteThirdPartyData
            Get
                Return AppliedToAllStates.ThirdPartyData
            End Get
            Set(value As QuickQuoteThirdPartyData)
                AppliedToAllStates.ThirdPartyData = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property TierTypeId As String 'N/A=-1; None=0; Uniform=1; Variable=2
            Get
                Return AppliedToAllStates.TierTypeId
            End Get
            Set(value As String)
                AppliedToAllStates.TierTypeId = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property TieringInformation As QuickQuoteTieringInformation
            Get
                Return AppliedToAllStates.TieringInformation
            End Get
            Set(value As QuickQuoteTieringInformation)
                AppliedToAllStates.TieringInformation = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property UnscheduledPersonalPropertyCoverage As QuickQuoteUnscheduledPersonalPropertyCoverage
            Get
                Return AppliedToGoverningState.UnscheduledPersonalPropertyCoverage
            End Get
            Set(value As QuickQuoteUnscheduledPersonalPropertyCoverage)
                AppliedToGoverningState.UnscheduledPersonalPropertyCoverage = value
            End Set
        End Property
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property ResidenceInfo As QuickQuoteResidenceInfo
            Get
                Return AppliedToAllStates.ResidenceInfo
            End Get
            Set(value As QuickQuoteResidenceInfo)
                AppliedToAllStates.ResidenceInfo = value
            End Set
        End Property

        'added 8/16/2018
        Public Property AppliedToGoverningState As QuickQuoteLobPolicyLevelInfo_AppliedToGoverningState
            Get
                If _AppliedToGoverningState Is Nothing Then
                    _AppliedToGoverningState = New QuickQuoteLobPolicyLevelInfo_AppliedToGoverningState
                End If
                SetObjectsParent(_AppliedToGoverningState)
                Return _AppliedToGoverningState
            End Get
            Set(value As QuickQuoteLobPolicyLevelInfo_AppliedToGoverningState)
                _AppliedToGoverningState = value
                SetObjectsParent(_AppliedToGoverningState)
            End Set
        End Property
        Public Property AppliedToAllStates As QuickQuoteLobPolicyLevelInfo_AppliedToAllStates
            Get
                If _AppliedToAllStates Is Nothing Then
                    _AppliedToAllStates = New QuickQuoteLobPolicyLevelInfo_AppliedToAllStates
                End If
                SetObjectsParent(_AppliedToAllStates)
                Return _AppliedToAllStates
            End Get
            Set(value As QuickQuoteLobPolicyLevelInfo_AppliedToAllStates)
                _AppliedToAllStates = value
                SetObjectsParent(_AppliedToAllStates)
            End Set
        End Property
        Public Property AppliedToIndividualState As QuickQuoteLobPolicyLevelInfo_AppliedToIndividualState
            Get
                If _AppliedToIndividualState Is Nothing Then
                    _AppliedToIndividualState = New QuickQuoteLobPolicyLevelInfo_AppliedToIndividualState
                End If
                SetObjectsParent(_AppliedToIndividualState)
                Return _AppliedToIndividualState
            End Get
            Set(value As QuickQuoteLobPolicyLevelInfo_AppliedToIndividualState)
                _AppliedToIndividualState = value
                SetObjectsParent(_AppliedToIndividualState)
            End Set
        End Property

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
            'added 8/16/2018
            _AppliedToGoverningState = New QuickQuoteLobPolicyLevelInfo_AppliedToGoverningState
            _AppliedToAllStates = New QuickQuoteLobPolicyLevelInfo_AppliedToAllStates
            _AppliedToIndividualState = New QuickQuoteLobPolicyLevelInfo_AppliedToIndividualState

            '_FullTermPremium = ""
            '_PolicyId = "" 'removed 8/16/2018; updated Properties to point to sub objects instead of private variables
            '_PolicyImageNum = "" 'removed 8/16/2018; updated Properties to point to sub objects instead of private variables
            '_AddFormsVersionId = ""
            '_Num = ""
            '_PackagePartTypeId = ""
            '_PackagePartType = ""
            '_RatingVersionId = ""
            '_UnderwritingVersionId = ""
            '_VersionId = ""

            'PolicyLevel; removed 8/16/2018; updated Properties to point to sub objects instead of private variables
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
            '_ResidenceInfo = New QuickQuoteResidenceInfo
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
                    'qqHelper.DisposeString(_FullTermPremium)
                    'qqHelper.DisposeString(_PolicyId) 'removed 8/16/2018; updated Properties to point to sub objects instead of private variables
                    'qqHelper.DisposeString(_PolicyImageNum) 'removed 8/16/2018; updated Properties to point to sub objects instead of private variables
                    'qqHelper.DisposeString(_AddFormsVersionId)
                    'qqHelper.DisposeString(_Num)
                    'qqHelper.DisposeString(_PackagePartTypeId)
                    'qqHelper.DisposeString(_PackagePartType)
                    'qqHelper.DisposeString(_RatingVersionId)
                    'qqHelper.DisposeString(_UnderwritingVersionId)
                    'qqHelper.DisposeString(_VersionId)

                    'PolicyLevel; removed 8/16/2018; updated Properties to point to sub objects instead of private variables
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
                    'If _ResidenceInfo IsNot Nothing Then
                    '    _ResidenceInfo.Dispose()
                    '    _ResidenceInfo = Nothing
                    'End If

                    'added 8/16/2018
                    If _AppliedToGoverningState IsNot Nothing Then
                        _AppliedToGoverningState.Dispose()
                        _AppliedToGoverningState = Nothing
                    End If
                    If _AppliedToAllStates IsNot Nothing Then
                        _AppliedToAllStates.Dispose()
                        _AppliedToAllStates = Nothing
                    End If
                    If _AppliedToIndividualState IsNot Nothing Then
                        _AppliedToIndividualState.Dispose()
                        _AppliedToIndividualState = Nothing
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
