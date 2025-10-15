Imports Microsoft.VisualBasic
Imports System.Web
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store package part information for a CPP quote
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()>
    Public Class QuickQuotePackagePart
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        '8/6/2018 note: added script tags to old properties for Serialization since it will already be in another object

        Dim qqHelper As New QuickQuoteHelperClass

        Private _FullTermPremium As String
        Private _PolicyId As String
        Private _PolicyImageNum As String
        'Private _AddFormsVersionId As String 'removed 7/23/2018
        Private _Num As String
        Private _PackagePartTypeId As String
        Private _PackagePartType As String
        'Private _RatingVersionId As String 'removed 7/23/2018
        'Private _UnderwritingVersionId As String 'removed 7/23/2018
        'Private _VersionId As String 'removed 7/23/2018

        'Private _PackageModificationAssignmentTypeId As String
        'Private _PackageTypeId As String
        'Private _PolicyTypeId As String

        'removed 7/23/2018
        ''added more 10/29/2012
        ''PolicyLevel
        'Private _AnniversaryRatingEffectiveDate As String
        'Private _AnniversaryRatingExpirationDate As String
        'Private _AutoSymbols As Generic.List(Of QuickQuoteAutoSymbol)
        'Private _BlanketRatingOptionId As String
        'Private _Coverages As Generic.List(Of QuickQuoteCoverage)
        'Private _DeductiblePerTypeId As String
        'Private _GLClassifications As Generic.List(Of QuickQuoteGLClassification)
        'Private _InclusionsExclusions As Generic.List(Of QuickQuoteInclusionExclusion)
        'Private _LossHistoryRecords As Generic.List(Of QuickQuoteLossHistoryRecord)
        'Private _Modifiers As Generic.List(Of QuickQuoteModifier)
        'Private _PackageModificationAssignmentTypeId As String
        'Private _PackageTypeId As String
        'Private _PolicyTypeId As String
        'Private _PolicyUnderwritings As Generic.List(Of QuickQuotePolicyUnderwriting)
        ''Private _PolicyUnderwritingCodeIds As Generic.List(Of String) 'added 11/26/2012 for CPP (to keep track of these so they aren't duplicated)
        ''updated 12/24/2012 since same code id can be used w/ different level or tab id
        'Private _PolicyUnderwritingCodeAndLevelAndTabIds As Generic.List(Of String)
        'Private _PriorCarrier As QuickQuotePriorCarrier
        'Private _ProgramTypeId As String
        'Private _RiskGrade As String
        'Private _RiskGradeLookupId As String 'added 10/31/2012
        'Private _ScheduledRatings As Generic.List(Of QuickQuoteScheduledRating)

        'removed 7/23/2018
        ''RiskLevel
        'Private _Applicants As Generic.List(Of QuickQuoteApplicant)
        'Private _Drivers As Generic.List(Of QuickQuoteDriver)
        'Private _Locations As Generic.List(Of QuickQuoteLocation)
        'Private _Vehicles As Generic.List(Of QuickQuoteVehicle)
        'Private _Operators As List(Of QuickQuoteOperator) 'added 4/9/2015

        'removed 7/23/2018
        ''added 1/26/2015 for CIM (PolicyLevel)
        'Private _ScheduledCoverages As List(Of QuickQuoteScheduledCoverage)

        'removed 7/23/2018
        ''added 4/9/2015 for Crime
        'Private _ClassificationCodes As List(Of QuickQuoteClassificationCode)
        'Private _AggregateLimit As String 'decimal
        'Private _NumberOfEmployees As String 'int

        'added 6/12/2018; Diamond doesn't have a spot in the UI to see CPP AIs at the policy level, but this is our workaround to have a central list that other spots can pull from
        'Private _AdditionalInterests As List(Of QuickQuoteAdditionalInterest) 'removed 8/6/2018

        'added 6/25/2018; updated 6/27/2018 to use VersionAndLobInfo object instead of LobInfo
        Private _VersionAndLobInfo As QuickQuoteVersionAndLobInfo

        'added 7/31/2018
        Private _PackagePartTypeMasterStateId As Integer
        Private _PackagePartTypeMasterLobId As Integer
        Private _PackagePartTypeStateId As Integer
        Private _PackagePartTypeLobId As Integer
        Private _NeedsToSetMasterAndPartStateAndLobCombosForPackagePartTypeId As Boolean
        Private _CaughtErrorWhenSettingMasterAndPartStateAndLobCombosForPackagePartTypeId As Boolean
        Private _OriginalPackagePartTypeId As String

        Private _DetailStatusCode As String 'added 5/15/2019

        Public Property FullTermPremium As String
            Get
                'Return _FullTermPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_FullTermPremium)
            End Get
            Set(value As String)
                _FullTermPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_FullTermPremium)
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
        'added 8/6/2018 to ignore for Serialization since it will already be in VersionAndLobInfo
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property AddFormsVersionId As String
            Get
                'Return _AddFormsVersionId
                'updated 7/23/2018
                Return VersionAndLobInfo.AddFormsVersionId
            End Get
            Set(value As String)
                '_AddFormsVersionId = value
                'updated 7/23/2018
                VersionAndLobInfo.AddFormsVersionId = value
            End Set
        End Property
        Public Property Num As String
            Get
                Return _Num
            End Get
            Set(value As String)
                _Num = value
            End Set
        End Property
        Public Property PackagePartTypeId As String
            Get
                Return _PackagePartTypeId
            End Get
            Set(value As String)
                Dim previousPackagePartTypeId As String = _PackagePartTypeId 'added 7/31/2018

                If String.IsNullOrWhiteSpace(_OriginalPackagePartTypeId) = True Then 'added 7/31/2018
                    _OriginalPackagePartTypeId = value
                End If

                _PackagePartTypeId = value
                _PackagePartType = ""
                If IsNumeric(_PackagePartTypeId) = True Then
                    Select Case _PackagePartTypeId
                        Case "0"
                            _PackagePartType = "N/A"
                        Case "1"
                            _PackagePartType = "Property"
                        Case "2"
                            _PackagePartType = "General Liability"
                        Case "3"
                            _PackagePartType = "Inland Marine"
                        Case "4"
                            _PackagePartType = "Crime"
                        Case "5"
                            _PackagePartType = "Garage"
                        Case "6"
                            _PackagePartType = "Package"
                    End Select
                End If

                'added 7/31/2018
                If QuickQuoteHelperClass.isTextMatch(previousPackagePartTypeId, _PackagePartTypeId, matchType:=QuickQuoteHelperClass.TextMatchType.IntegerOrText_IgnoreCasing) = False Then
                    _NeedsToSetMasterAndPartStateAndLobCombosForPackagePartTypeId = True
                End If
            End Set
        End Property
        Public ReadOnly Property PackagePartType As String
            Get
                Return _PackagePartType
            End Get
        End Property
        'added 8/6/2018 to ignore for Serialization since it will already be in VersionAndLobInfo
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property RatingVersionId As String
            Get
                'Return _RatingVersionId
                'updated 7/23/2018
                Return VersionAndLobInfo.RatingVersionId
            End Get
            Set(value As String)
                '_RatingVersionId = value
                'updated 7/23/2018
                VersionAndLobInfo.RatingVersionId = value
            End Set
        End Property
        'added 8/6/2018 to ignore for Serialization since it will already be in VersionAndLobInfo
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property UnderwritingVersionId As String
            Get
                'Return _UnderwritingVersionId
                'updated 7/23/2018
                Return VersionAndLobInfo.UnderwritingVersionId
            End Get
            Set(value As String)
                '_UnderwritingVersionId = value
                'updated 7/23/2018
                VersionAndLobInfo.UnderwritingVersionId = value
            End Set
        End Property
        'added 8/6/2018 to ignore for Serialization since it will already be in VersionAndLobInfo
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property VersionId As String
            Get
                'Return _VersionId
                'updated 7/23/2018
                Return VersionAndLobInfo.VersionId
            End Get
            Set(value As String)
                '_VersionId = value
                'updated 7/23/2018
                VersionAndLobInfo.VersionId = value
            End Set
        End Property

        'added 8/6/2018 to ignore for Serialization since it will already be in VersionAndLobInfo
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property AnniversaryRatingEffectiveDate As String
            Get
                'Return _AnniversaryRatingEffectiveDate
                'updated 7/23/2018
                Return VersionAndLobInfo.AnniversaryRatingEffectiveDate
            End Get
            Set(value As String)
                '_AnniversaryRatingEffectiveDate = value
                'qqHelper.ConvertToShortDate(_AnniversaryRatingEffectiveDate)
                'If IsDate(_AnniversaryRatingEffectiveDate) = True Then
                '    AnniversaryRatingExpirationDate = DateAdd(DateInterval.Year, 1, CDate(_AnniversaryRatingEffectiveDate))
                'End If
                'updated 7/23/2018
                VersionAndLobInfo.AnniversaryRatingEffectiveDate = value
            End Set
        End Property
        'added 8/6/2018 to ignore for Serialization since it will already be in VersionAndLobInfo
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property AnniversaryRatingExpirationDate As String
            Get
                'Return _AnniversaryRatingExpirationDate
                'updated 7/23/2018
                Return VersionAndLobInfo.AnniversaryRatingExpirationDate
            End Get
            Set(value As String)
                '_AnniversaryRatingExpirationDate = value
                'qqHelper.ConvertToShortDate(_AnniversaryRatingExpirationDate)
                'updated 7/23/2018
                VersionAndLobInfo.AnniversaryRatingExpirationDate = value
            End Set
        End Property
        'added 8/6/2018 to ignore for Serialization since it will already be in VersionAndLobInfo
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property AutoSymbols As Generic.List(Of QuickQuoteAutoSymbol)
            Get
                'SetParentOfListItems(_AutoSymbols, "{663B7C7B-F2AC-4BF6-965A-D30F41A05403}")
                'Return _AutoSymbols
                'updated 7/23/2018
                SetParentOfListItems(VersionAndLobInfo.AutoSymbols, "{663B7C7B-F2AC-4BF6-965A-D30F41A05403}")
                Return VersionAndLobInfo.AutoSymbols
            End Get
            Set(value As Generic.List(Of QuickQuoteAutoSymbol))
                '_AutoSymbols = value
                'SetParentOfListItems(_AutoSymbols, "{663B7C7B-F2AC-4BF6-965A-D30F41A05403}")
                'updated 7/23/2018
                VersionAndLobInfo.AutoSymbols = value
                SetParentOfListItems(VersionAndLobInfo.AutoSymbols, "{663B7C7B-F2AC-4BF6-965A-D30F41A05403}")
            End Set
        End Property
        'added 8/6/2018 to ignore for Serialization since it will already be in VersionAndLobInfo
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property BlanketRatingOptionId As String 'verified in database 7/3/2012
            Get
                'Return _BlanketRatingOptionId
                'updated 7/23/2018
                Return VersionAndLobInfo.BlanketRatingOptionId
            End Get
            Set(value As String)
                '_BlanketRatingOptionId = value
                ''(0=N/A; 1=Combined Building and Personal Property; 2=Building Only; 3=Personal Property Only)
                ''_BlanketRatingOption = ""
                ''If IsNumeric(_BlanketRatingOptionId) = True Then
                ''    Select Case _BlanketRatingOptionId
                ''        Case "0"
                ''            _BlanketRatingOption = "N/A"
                ''        Case "1"
                ''            _BlanketRatingOption = "Combined Building and Personal Property"
                ''        Case "2"
                ''            _BlanketRatingOption = "Building Only"
                ''        Case "3"
                ''            _BlanketRatingOption = "Personal Property Only"
                ''    End Select
                ''End If
                'updated 7/23/2018
                VersionAndLobInfo.BlanketRatingOptionId = value
            End Set
        End Property
        'added 8/6/2018 to ignore for Serialization since it will already be in VersionAndLobInfo
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Coverages As Generic.List(Of QuickQuoteCoverage)
            Get
                'SetParentOfListItems(_Coverages, "{663B7C7B-F2AC-4BF6-965A-D30F41A05404}")
                'Return _Coverages
                'updated 7/23/2018
                SetParentOfListItems(VersionAndLobInfo.Coverages, "{663B7C7B-F2AC-4BF6-965A-D30F41A05404}")
                Return VersionAndLobInfo.Coverages
            End Get
            Set(value As Generic.List(Of QuickQuoteCoverage))
                '_Coverages = value
                'SetParentOfListItems(_Coverages, "{663B7C7B-F2AC-4BF6-965A-D30F41A05404}")
                'updated 7/23/2018
                VersionAndLobInfo.Coverages = value
                SetParentOfListItems(VersionAndLobInfo.Coverages, "{663B7C7B-F2AC-4BF6-965A-D30F41A05404}")
            End Set
        End Property
        'added 8/6/2018 to ignore for Serialization since it will already be in VersionAndLobInfo
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property DeductiblePerTypeId As String
            Get
                'Return _DeductiblePerTypeId
                'updated 7/23/2018
                Return VersionAndLobInfo.DeductiblePerTypeId
            End Get
            Set(value As String)
                '_DeductiblePerTypeId = value
                ''(0-N/A; 1=Per Occurrence; 2=Per Claim)
                ''_PropertyDamageLiabilityDeductibleOption = ""
                ''If IsNumeric(_PropertyDamageLiabilityDeductibleOptionId) = True Then
                ''    Select Case _PropertyDamageLiabilityDeductibleOptionId
                ''        Case "0"
                ''            _PropertyDamageLiabilityDeductibleOption = "N/A"
                ''        Case "1"
                ''            _PropertyDamageLiabilityDeductibleOption = "Per Occurrence"
                ''        Case "2"
                ''            _PropertyDamageLiabilityDeductibleOption = "Per Claim"
                ''    End Select
                ''End If
                'updated 7/23/2018
                VersionAndLobInfo.DeductiblePerTypeId = value
            End Set
        End Property
        'added 8/6/2018 to ignore for Serialization since it will already be in VersionAndLobInfo
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property GLClassifications As Generic.List(Of QuickQuoteGLClassification)
            Get
                'SetParentOfListItems(_GLClassifications, "{663B7C7B-F2AC-4BF6-965A-D30F41A05405}")
                'Return _GLClassifications
                'updated 7/23/2018
                SetParentOfListItems(VersionAndLobInfo.GLClassifications, "{663B7C7B-F2AC-4BF6-965A-D30F41A05405}")
                Return VersionAndLobInfo.GLClassifications
            End Get
            Set(value As Generic.List(Of QuickQuoteGLClassification))
                '_GLClassifications = value
                'SetParentOfListItems(_GLClassifications, "{663B7C7B-F2AC-4BF6-965A-D30F41A05405}")
                'updated 7/23/2018
                VersionAndLobInfo.GLClassifications = value
                SetParentOfListItems(VersionAndLobInfo.GLClassifications, "{663B7C7B-F2AC-4BF6-965A-D30F41A05405}")
            End Set
        End Property
        'added 8/6/2018 to ignore for Serialization since it will already be in VersionAndLobInfo
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property InclusionsExclusions As Generic.List(Of QuickQuoteInclusionExclusion)
            Get
                'SetParentOfListItems(_InclusionsExclusions, "{663B7C7B-F2AC-4BF6-965A-D30F41A05406}")
                'Return _InclusionsExclusions
                'updated 7/23/2018
                SetParentOfListItems(VersionAndLobInfo.InclusionsExclusions, "{663B7C7B-F2AC-4BF6-965A-D30F41A05406}")
                Return VersionAndLobInfo.InclusionsExclusions
            End Get
            Set(value As Generic.List(Of QuickQuoteInclusionExclusion))
                '_InclusionsExclusions = value
                'SetParentOfListItems(_InclusionsExclusions, "{663B7C7B-F2AC-4BF6-965A-D30F41A05406}")
                'updated 7/23/2018
                VersionAndLobInfo.InclusionsExclusions = value
                SetParentOfListItems(VersionAndLobInfo.InclusionsExclusions, "{663B7C7B-F2AC-4BF6-965A-D30F41A05406}")
            End Set
        End Property
        'added 8/6/2018 to ignore for Serialization since it will already be in VersionAndLobInfo
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property LossHistoryRecords As Generic.List(Of QuickQuoteLossHistoryRecord)
            Get
                'SetParentOfListItems(_LossHistoryRecords, "{663B7C7B-F2AC-4BF6-965A-D30F41A05407}")
                'Return _LossHistoryRecords
                'updated 7/23/2018
                SetParentOfListItems(VersionAndLobInfo.LossHistoryRecords, "{663B7C7B-F2AC-4BF6-965A-D30F41A05407}")
                Return VersionAndLobInfo.LossHistoryRecords
            End Get
            Set(value As Generic.List(Of QuickQuoteLossHistoryRecord))
                '_LossHistoryRecords = value
                'SetParentOfListItems(_LossHistoryRecords, "{663B7C7B-F2AC-4BF6-965A-D30F41A05407}")
                'updated 7/23/2018
                VersionAndLobInfo.LossHistoryRecords = value
                SetParentOfListItems(VersionAndLobInfo.LossHistoryRecords, "{663B7C7B-F2AC-4BF6-965A-D30F41A05407}")
            End Set
        End Property
        'added 8/6/2018 to ignore for Serialization since it will already be in VersionAndLobInfo
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Modifiers As Generic.List(Of QuickQuoteModifier)
            Get
                'SetParentOfListItems(_Modifiers, "{663B7C7B-F2AC-4BF6-965A-D30F41A05408}")
                'Return _Modifiers
                'updated 7/23/2018
                SetParentOfListItems(VersionAndLobInfo.Modifiers, "{663B7C7B-F2AC-4BF6-965A-D30F41A05408}")
                Return VersionAndLobInfo.Modifiers
            End Get
            Set(value As Generic.List(Of QuickQuoteModifier))
                '_Modifiers = value
                'SetParentOfListItems(_Modifiers, "{663B7C7B-F2AC-4BF6-965A-D30F41A05408}")
                'updated 7/23/2018
                VersionAndLobInfo.Modifiers = value
                SetParentOfListItems(VersionAndLobInfo.Modifiers, "{663B7C7B-F2AC-4BF6-965A-D30F41A05408}")
            End Set
        End Property
        'added 8/6/2018 to ignore for Serialization since it will already be in VersionAndLobInfo
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PackageModificationAssignmentTypeId As String
            Get
                'Return _PackageModificationAssignmentTypeId
                'updated 7/23/2018
                Return VersionAndLobInfo.PackageModificationAssignmentTypeId
            End Get
            Set(value As String)
                '_PackageModificationAssignmentTypeId = value
                ''_PackageModificationAssignmentType = ""
                ''If IsNumeric(_PackageModificationAssignmentTypeId) = True Then
                ''    Select Case _PackageModificationAssignmentTypeId
                ''        Case "0"
                ''            _PackageModificationAssignmentType = "N/A"
                ''        Case "1"
                ''            _PackageModificationAssignmentType = "Apartment House Risk"
                ''        Case "2"
                ''            _PackageModificationAssignmentType = "Contractors Risk"
                ''        Case "3"
                ''            _PackageModificationAssignmentType = "Institutional Risk"
                ''        Case "4"
                ''            _PackageModificationAssignmentType = "Industrial and Processing Risk"
                ''        Case "5"
                ''            _PackageModificationAssignmentType = "Mercantile Risk"
                ''        Case "6"
                ''            _PackageModificationAssignmentType = "Motel/Hotel Risk"
                ''        Case "7"
                ''            _PackageModificationAssignmentType = "Office Risk"
                ''        Case "8"
                ''            _PackageModificationAssignmentType = "Service Risk"
                ''    End Select
                ''End If
                'updated 7/23/2018
                VersionAndLobInfo.PackageModificationAssignmentTypeId = value
            End Set
        End Property
        'added 8/6/2018 to ignore for Serialization since it will already be in VersionAndLobInfo
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PackageTypeId As String
            Get
                'Return _PackageTypeId
                'updated 7/23/2018
                Return VersionAndLobInfo.PackageTypeId
            End Get
            Set(value As String)
                '_PackageTypeId = value
                ''_PackageType = ""
                ''If IsNumeric(_PackageTypeId) = True Then
                ''    Select Case _PackageTypeId
                ''        Case "0"
                ''            _PackageType = "N/A"
                ''        Case "1"
                ''            _PackageType = "CPP"
                ''        Case "2"
                ''            _PackageType = "POP"
                ''    End Select
                ''End If
                'updated 7/23/2018
                VersionAndLobInfo.PackageTypeId = value
            End Set
        End Property
        'added 8/6/2018 to ignore for Serialization since it will already be in VersionAndLobInfo
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PolicyTypeId As String 'only coded for CPR right now (9/27/2012)
            Get
                'Return _PolicyTypeId
                'updated 7/23/2018
                Return VersionAndLobInfo.PolicyTypeId
            End Get
            Set(value As String)
                '_PolicyTypeId = value
                ''_PolicyType = ""
                ''If IsNumeric(_PolicyTypeId) = True Then
                ''    Select Case _PolicyTypeId
                ''        Case "0"
                ''            _PolicyType = "N/A"
                ''        Case "1"
                ''            _PolicyType = "None"
                ''        Case "60"
                ''            _PolicyType = "Standard"
                ''        Case "61"
                ''            _PolicyType = "Preferred"
                ''    End Select
                ''End If
                'updated 7/23/2018
                VersionAndLobInfo.PolicyTypeId = value
            End Set
        End Property
        'added 8/6/2018 to ignore for Serialization since it will already be in VersionAndLobInfo
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PolicyUnderwritings As Generic.List(Of QuickQuotePolicyUnderwriting)
            Get
                'SetParentOfListItems(_LossHistoryRecords, "{663B7C7B-F2AC-4BF6-965A-D30F41A05409}")
                'Return _PolicyUnderwritings
                'updated 7/23/2018
                SetParentOfListItems(VersionAndLobInfo.PolicyUnderwritings, "{663B7C7B-F2AC-4BF6-965A-D30F41A05409}")
                Return VersionAndLobInfo.PolicyUnderwritings
            End Get
            Set(value As Generic.List(Of QuickQuotePolicyUnderwriting))
                '_PolicyUnderwritings = value
                'SetParentOfListItems(_LossHistoryRecords, "{663B7C7B-F2AC-4BF6-965A-D30F41A05409}")
                'updated 7/23/2018
                VersionAndLobInfo.PolicyUnderwritings = value
                SetParentOfListItems(VersionAndLobInfo.PolicyUnderwritings, "{663B7C7B-F2AC-4BF6-965A-D30F41A05409}")
            End Set
        End Property
        'added 8/6/2018 to ignore for Serialization since it will already be in VersionAndLobInfo
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PolicyUnderwritingCodeAndLevelAndTabIds As Generic.List(Of String)
            Get
                'Return _PolicyUnderwritingCodeAndLevelAndTabIds
                'updated 7/23/2018
                Return VersionAndLobInfo.PolicyUnderwritingCodeAndLevelAndTabIds
            End Get
            Set(value As Generic.List(Of String))
                '_PolicyUnderwritingCodeAndLevelAndTabIds = value
                'updated 7/23/2018
                VersionAndLobInfo.PolicyUnderwritingCodeAndLevelAndTabIds = value
            End Set
        End Property
        'added 8/6/2018 to ignore for Serialization since it will already be in VersionAndLobInfo
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property PriorCarrier As QuickQuotePriorCarrier
            Get
                'SetObjectsParent(_PriorCarrier)
                'Return _PriorCarrier
                'updated 7/23/2018
                SetObjectsParent(VersionAndLobInfo.PriorCarrier)
                Return VersionAndLobInfo.PriorCarrier
            End Get
            Set(value As QuickQuotePriorCarrier)
                '_PriorCarrier = value
                'SetObjectsParent(_PriorCarrier)
                'updated 7/23/2018
                VersionAndLobInfo.PriorCarrier = value
                SetObjectsParent(VersionAndLobInfo.PriorCarrier)
            End Set
        End Property
        'added 8/6/2018 to ignore for Serialization since it will already be in VersionAndLobInfo
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property ProgramTypeId As String
            Get
                'Return _ProgramTypeId
                'updated 7/23/2018
                Return VersionAndLobInfo.ProgramTypeId
            End Get
            Set(value As String)
                '_ProgramTypeId = value
                ''_ProgramType = ""
                ''If IsNumeric(_ProgramTypeId) = True Then
                ''    Select Case _ProgramTypeId
                ''        Case "-1"
                ''            _ProgramType = "Unassigned"
                ''        Case "0"
                ''            _ProgramType = "None"
                ''        Case "1"
                ''            _ProgramType = "Homeowners"
                ''        Case "2"
                ''            _ProgramType = "Mobile Home"
                ''        Case "3"
                ''            _ProgramType = "Dwelling Fire"
                ''        Case "4"
                ''            _ProgramType = "Personal Umbrella"
                ''        Case "5"
                ''            _ProgramType = "Farm Umbrella"
                ''        Case "6"
                ''            _ProgramType = "Farmowners"
                ''        Case "7"
                ''            _ProgramType = "Select-O-Matic"
                ''        Case "8"
                ''            _ProgramType = "Farm Liability"
                ''        Case "9"
                ''            _ProgramType = "N/A"
                ''        Case "10"
                ''            _ProgramType = "Coinsurance Only"
                ''        Case "11"
                ''            _ProgramType = "Benefits Deductible"
                ''        Case "12"
                ''            _ProgramType = "Coinsurance and Deductible"
                ''        Case "13"
                ''            _ProgramType = "Dealers"
                ''        Case "14"
                ''            _ProgramType = "Non-Dealers"
                ''        Case "48"
                ''            _ProgramType = "Commercial Crime"
                ''        Case "49"
                ''            _ProgramType = "Government Crime"
                ''        Case "50"
                ''            _ProgramType = "Employee Theft & Forgery"
                ''        Case "51"
                ''            _ProgramType = "Commercial Umbrella"
                ''        Case "52"
                ''            _ProgramType = "BOP - Contractors"
                ''        Case "53"
                ''            _ProgramType = "BOP - Other Than Contractors"
                ''        Case "54"
                ''            _ProgramType = "CGL - Commercial General Liability - Standard"
                ''        Case "55"
                ''            _ProgramType = "CGL - Commercial General Liability - Preferred"
                ''        Case "56"
                ''            _ProgramType = "OCP - Owners and Contractors Protective Liability"
                ''        Case "57"
                ''            _ProgramType = "Dealers - Unlimited Liability"
                ''        Case "58"
                ''            _ProgramType = "Dealers - Limited Liability"
                ''    End Select
                ''End If
                'updated 7/23/2018
                VersionAndLobInfo.ProgramTypeId = value
            End Set
        End Property
        'added 8/6/2018 to ignore for Serialization since it will already be in VersionAndLobInfo
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property RiskGrade As String
            Get
                'Return _RiskGrade
                'updated 7/23/2018
                Return VersionAndLobInfo.RiskGrade
            End Get
            Set(value As String)
                '_RiskGrade = value
                'updated 7/23/2018
                VersionAndLobInfo.RiskGrade = value
            End Set
        End Property
        'added 8/6/2018 to ignore for Serialization since it will already be in VersionAndLobInfo
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property RiskGradeLookupId As String
            Get
                'Return _RiskGradeLookupId
                'updated 7/23/2018
                Return VersionAndLobInfo.RiskGradeLookupId
            End Get
            Set(value As String)
                '_RiskGradeLookupId = value
                'updated 7/23/2018
                VersionAndLobInfo.RiskGradeLookupId = value
            End Set
        End Property
        'added 8/6/2018 to ignore for Serialization since it will already be in VersionAndLobInfo
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property ScheduledRatings As Generic.List(Of QuickQuoteScheduledRating)
            Get
                'SetParentOfListItems(_ScheduledRatings, "{663B7C7B-F2AC-4BF6-965A-D30F41A05410}")
                'Return _ScheduledRatings
                'updated 7/23/2018
                SetParentOfListItems(VersionAndLobInfo.ScheduledRatings, "{663B7C7B-F2AC-4BF6-965A-D30F41A05410}")
                Return VersionAndLobInfo.ScheduledRatings
            End Get
            Set(value As Generic.List(Of QuickQuoteScheduledRating))
                '_ScheduledRatings = value
                'SetParentOfListItems(_ScheduledRatings, "{663B7C7B-F2AC-4BF6-965A-D30F41A05410}")
                'updated 7/23/2018
                VersionAndLobInfo.ScheduledRatings = value
                SetParentOfListItems(VersionAndLobInfo.ScheduledRatings, "{663B7C7B-F2AC-4BF6-965A-D30F41A05410}")
            End Set
        End Property

        'added 8/6/2018 to ignore for Serialization since it will already be in VersionAndLobInfo
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Applicants As Generic.List(Of QuickQuoteApplicant)
            Get
                'SetParentOfListItems(_Applicants, "{663B7C7B-F2AC-4BF6-965A-D30F41A05411}")
                'Return _Applicants
                'updated 7/23/2018
                SetParentOfListItems(VersionAndLobInfo.Applicants, "{663B7C7B-F2AC-4BF6-965A-D30F41A05411}")
                Return VersionAndLobInfo.Applicants
            End Get
            Set(value As Generic.List(Of QuickQuoteApplicant))
                '_Applicants = value
                'SetParentOfListItems(_Applicants, "{663B7C7B-F2AC-4BF6-965A-D30F41A05411}")
                'updated 7/23/2018
                VersionAndLobInfo.Applicants = value
                SetParentOfListItems(VersionAndLobInfo.Applicants, "{663B7C7B-F2AC-4BF6-965A-D30F41A05411}")
            End Set
        End Property
        'added 8/6/2018 to ignore for Serialization since it will already be in VersionAndLobInfo
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Drivers As Generic.List(Of QuickQuoteDriver)
            Get
                'SetParentOfListItems(_Drivers, "{663B7C7B-F2AC-4BF6-965A-D30F41A05412}")
                'Return _Drivers
                'updated 7/23/2018
                SetParentOfListItems(VersionAndLobInfo.Drivers, "{663B7C7B-F2AC-4BF6-965A-D30F41A05412}")
                Return VersionAndLobInfo.Drivers
            End Get
            Set(value As Generic.List(Of QuickQuoteDriver))
                '_Drivers = value
                'SetParentOfListItems(_Drivers, "{663B7C7B-F2AC-4BF6-965A-D30F41A05412}")
                'updated 7/23/2018
                VersionAndLobInfo.Drivers = value
                SetParentOfListItems(VersionAndLobInfo.Drivers, "{663B7C7B-F2AC-4BF6-965A-D30F41A05412}")
            End Set
        End Property
        'added 8/6/2018 to ignore for Serialization since it will already be in VersionAndLobInfo
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Locations As Generic.List(Of QuickQuoteLocation)
            Get
                'SetParentOfListItems(_Locations, "{663B7C7B-F2AC-4BF6-965A-D30F41A05413}")
                'Return _Locations
                'updated 7/23/2018
                SetParentOfListItems(VersionAndLobInfo.Locations, "{663B7C7B-F2AC-4BF6-965A-D30F41A05413}")
                Return VersionAndLobInfo.Locations
            End Get
            Set(value As Generic.List(Of QuickQuoteLocation))
                '_Locations = value
                'SetParentOfListItems(_Locations, "{663B7C7B-F2AC-4BF6-965A-D30F41A05413}")
                'updated 7/23/2018
                VersionAndLobInfo.Locations = value
                SetParentOfListItems(VersionAndLobInfo.Locations, "{663B7C7B-F2AC-4BF6-965A-D30F41A05413}")
            End Set
        End Property
        'added 8/6/2018 to ignore for Serialization since it will already be in VersionAndLobInfo
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Vehicles As Generic.List(Of QuickQuoteVehicle)
            Get
                'SetParentOfListItems(_Vehicles, "{663B7C7B-F2AC-4BF6-965A-D30F41A05414}")
                'Return _Vehicles
                'updated 7/23/2018
                SetParentOfListItems(VersionAndLobInfo.Vehicles, "{663B7C7B-F2AC-4BF6-965A-D30F41A05414}")
                Return VersionAndLobInfo.Vehicles
            End Get
            Set(value As Generic.List(Of QuickQuoteVehicle))
                '_Vehicles = value
                'SetParentOfListItems(_Vehicles, "{663B7C7B-F2AC-4BF6-965A-D30F41A05414}")
                'updated 7/23/2018
                VersionAndLobInfo.Vehicles = value
                SetParentOfListItems(VersionAndLobInfo.Vehicles, "{663B7C7B-F2AC-4BF6-965A-D30F41A05414}")
            End Set
        End Property
        'added 8/6/2018 to ignore for Serialization since it will already be in VersionAndLobInfo
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property Operators As List(Of QuickQuoteOperator) 'added 4/9/2015
            Get
                'SetParentOfListItems(_Operators, "{663B7C7B-F2AC-4BF6-965A-D30F41A05415}")
                'Return _Operators
                'updated 7/23/2018
                SetParentOfListItems(VersionAndLobInfo.Operators, "{663B7C7B-F2AC-4BF6-965A-D30F41A05415}")
                Return VersionAndLobInfo.Operators
            End Get
            Set(value As List(Of QuickQuoteOperator))
                '_Operators = value
                'SetParentOfListItems(_Operators, "{663B7C7B-F2AC-4BF6-965A-D30F41A05415}")
                'updated 7/23/2018
                VersionAndLobInfo.Operators = value
                SetParentOfListItems(VersionAndLobInfo.Operators, "{663B7C7B-F2AC-4BF6-965A-D30F41A05415}")
            End Set
        End Property

        'added 1/26/2015 for CIM
        'added 8/6/2018 to ignore for Serialization since it will already be in VersionAndLobInfo
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property ScheduledCoverages As List(Of QuickQuoteScheduledCoverage)
            Get
                'SetParentOfListItems(_ScheduledCoverages, "{663B7C7B-F2AC-4BF6-965A-D30F41A05416}")
                'Return _ScheduledCoverages
                'updated 7/23/2018
                SetParentOfListItems(VersionAndLobInfo.ScheduledCoverages, "{663B7C7B-F2AC-4BF6-965A-D30F41A05416}")
                Return VersionAndLobInfo.ScheduledCoverages
            End Get
            Set(value As List(Of QuickQuoteScheduledCoverage))
                '_ScheduledCoverages = value
                'SetParentOfListItems(_ScheduledCoverages, "{663B7C7B-F2AC-4BF6-965A-D30F41A05416}")
                'updated 7/23/2018
                VersionAndLobInfo.ScheduledCoverages = value
                SetParentOfListItems(VersionAndLobInfo.ScheduledCoverages, "{663B7C7B-F2AC-4BF6-965A-D30F41A05416}")
            End Set
        End Property

        'added 4/9/2015 for Crime
        'added 8/6/2018 to ignore for Serialization since it will already be in VersionAndLobInfo
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property ClassificationCodes As List(Of QuickQuoteClassificationCode)
            Get
                'SetParentOfListItems(_ClassificationCodes, "{663B7C7B-F2AC-4BF6-965A-D30F41A05417}")
                'Return _ClassificationCodes
                'updated 7/23/2018
                SetParentOfListItems(VersionAndLobInfo.ClassificationCodes, "{663B7C7B-F2AC-4BF6-965A-D30F41A05417}")
                Return VersionAndLobInfo.ClassificationCodes
            End Get
            Set(value As List(Of QuickQuoteClassificationCode))
                '_ClassificationCodes = value
                'SetParentOfListItems(_ClassificationCodes, "{663B7C7B-F2AC-4BF6-965A-D30F41A05417}")
                'updated 7/23/2018
                VersionAndLobInfo.ClassificationCodes = value
                SetParentOfListItems(VersionAndLobInfo.ClassificationCodes, "{663B7C7B-F2AC-4BF6-965A-D30F41A05417}")
            End Set
        End Property
        'added 8/6/2018 to ignore for Serialization since it will already be in VersionAndLobInfo
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property AggregateLimit As String 'decimal
            Get
                'Return _AggregateLimit
                'updated 7/23/2018
                Return VersionAndLobInfo.AggregateLimit
            End Get
            Set(value As String)
                '_AggregateLimit = value
                'qqHelper.ConvertToLimitFormat(_AggregateLimit)
                'updated 7/23/2018
                VersionAndLobInfo.AggregateLimit = value
            End Set
        End Property
        'added 8/6/2018 to ignore for Serialization since it will already be in VersionAndLobInfo
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property NumberOfEmployees As String 'int
            Get
                'Return _NumberOfEmployees
                'updated 7/23/2018
                Return VersionAndLobInfo.NumberOfEmployees
            End Get
            Set(value As String)
                '_NumberOfEmployees = value
                'updated 7/23/2018
                VersionAndLobInfo.NumberOfEmployees = value
            End Set
        End Property

        'added 6/12/2018; Diamond doesn't have a spot in the UI to see CPP AIs at the policy level, but this is our workaround to have a central list that other spots can pull from
        'added 8/6/2018 to ignore for Serialization since it will already be in VersionAndLobInfo
        <Script.Serialization.ScriptIgnore>
        <System.Xml.Serialization.XmlIgnore()>
        Public Property AdditionalInterests As List(Of QuickQuoteAdditionalInterest)
            Get
                'SetParentOfListItems(_AdditionalInterests, "{663B7C7B-F2AC-4BF6-965A-D30F41A05418}")
                'Return _AdditionalInterests
                'updated 8/6/2018
                SetParentOfListItems(VersionAndLobInfo.AdditionalInterests, "{663B7C7B-F2AC-4BF6-965A-D30F41A05418}")
                Return VersionAndLobInfo.AdditionalInterests
            End Get
            Set(value As List(Of QuickQuoteAdditionalInterest))
                '_AdditionalInterests = value
                'SetParentOfListItems(_AdditionalInterests, "{663B7C7B-F2AC-4BF6-965A-D30F41A05418}")
                'updated 8/6/2018
                VersionAndLobInfo.AdditionalInterests = value
                SetParentOfListItems(VersionAndLobInfo.AdditionalInterests, "{663B7C7B-F2AC-4BF6-965A-D30F41A05418}")
            End Set
        End Property

        'added 6/25/2018; updated 6/27/2018 to use VersionAndLobInfo object instead of LobInfo
        Public Property VersionAndLobInfo As QuickQuoteVersionAndLobInfo
            Get
                If _VersionAndLobInfo Is Nothing Then
                    _VersionAndLobInfo = New QuickQuoteVersionAndLobInfo
                End If
                '_VersionAndLobInfo.SetParent = Me
                'updated 7/25/2018
                SetObjectsParent(_VersionAndLobInfo)
                Return _VersionAndLobInfo
            End Get
            Set(value As QuickQuoteVersionAndLobInfo)
                _VersionAndLobInfo = value
                SetObjectsParent(_VersionAndLobInfo) 'added 7/25/2018
            End Set
        End Property

        'added 7/31/2018
        Public ReadOnly Property PackagePartTypeMasterStateId As Integer
            Get
                If _NeedsToSetMasterAndPartStateAndLobCombosForPackagePartTypeId = True Then
                    SetMasterAndPartStateAndLobCombosForPackagePartTypeId()
                End If
                Return _PackagePartTypeMasterStateId
            End Get
        End Property
        Public ReadOnly Property PackagePartTypeMasterState As QuickQuoteHelperClass.QuickQuoteState
            Get
                Return QuickQuoteHelperClass.QuickQuoteStateForDiamondStateId(PackagePartTypeMasterStateId, defaultToIndiana:=False)
            End Get
        End Property
        Public ReadOnly Property PackagePartTypeMasterLobId As Integer
            Get
                If _NeedsToSetMasterAndPartStateAndLobCombosForPackagePartTypeId = True Then
                    SetMasterAndPartStateAndLobCombosForPackagePartTypeId()
                End If
                Return _PackagePartTypeMasterLobId
            End Get
        End Property
        Public ReadOnly Property PackagePartTypeMasterLob As QuickQuoteObject.QuickQuoteLobType
            Get
                Return QuickQuoteHelperClass.LobTypeForMasterLobId(PackagePartTypeMasterLobId)
            End Get
        End Property
        Public ReadOnly Property PackagePartTypeStateId As Integer
            Get
                If _NeedsToSetMasterAndPartStateAndLobCombosForPackagePartTypeId = True Then
                    SetMasterAndPartStateAndLobCombosForPackagePartTypeId()
                End If
                Return _PackagePartTypeStateId
            End Get
        End Property
        Public ReadOnly Property PackagePartTypeState As QuickQuoteHelperClass.QuickQuoteState
            Get
                Return QuickQuoteHelperClass.QuickQuoteStateForDiamondStateId(PackagePartTypeStateId, defaultToIndiana:=False)
            End Get
        End Property
        Public ReadOnly Property PackagePartTypeLobId As Integer
            Get
                If _NeedsToSetMasterAndPartStateAndLobCombosForPackagePartTypeId = True Then
                    SetMasterAndPartStateAndLobCombosForPackagePartTypeId()
                End If
                Return _PackagePartTypeLobId
            End Get
        End Property
        Public ReadOnly Property PackagePartTypeLob As QuickQuoteObject.QuickQuoteLobType
            Get
                Return QuickQuoteHelperClass.LobTypeForLobId(PackagePartTypeLobId)
            End Get
        End Property
        Public ReadOnly Property IsMasterPackagePart As Boolean
            Get
                Dim isMaster As Boolean = False
                If PackagePartTypeLobId > 0 AndAlso _CaughtErrorWhenSettingMasterAndPartStateAndLobCombosForPackagePartTypeId = False Then
                    'okay to use PackagePartType properties
                    If (PackagePartTypeMasterStateId = 0 AndAlso PackagePartTypeMasterLobId = 0) OrElse QuickQuoteHelperClass.IsMasterLobId(PackagePartTypeLobId) = True Then
                        isMaster = True
                    End If
                Else
                    'If QuickQuoteHelperClass.IsMasterLobId(VersionAndLobInfo.ActualLobId) = True Then
                    'updated 8/5/2018
                    If QuickQuoteHelperClass.IsMasterLobId(qqHelper.IntegerForString(VersionAndLobInfo.ActualLobId)) = True Then
                        isMaster = True
                    End If
                End If
                Return isMaster
            End Get
        End Property
        Public ReadOnly Property OriginalPackagePartTypeId As String
            Get
                Return _OriginalPackagePartTypeId
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

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub

        Public Sub New(Parent As QuickQuoteObject)
            MyBase.New()
            SetDefaults()
            Me.SetParent = Parent
        End Sub
        Private Sub SetDefaults()
            'added 6/25/2018; updated 6/27/2018 to use VersionAndLobInfo object instead of LobInfo
            _VersionAndLobInfo = New QuickQuoteVersionAndLobInfo

            _FullTermPremium = ""
            _PolicyId = ""
            _PolicyImageNum = ""
            '_AddFormsVersionId = "" 'removed 7/23/2018
            _Num = ""
            _PackagePartTypeId = ""
            _PackagePartType = ""
            '_RatingVersionId = "" 'removed 7/23/2018
            '_UnderwritingVersionId = "" 'removed 7/23/2018
            '_VersionId = "" 'removed 7/23/2018

            'removed 7/23/2018
            '_AnniversaryRatingEffectiveDate = ""
            '_AnniversaryRatingExpirationDate = ""
            ''_AutoSymbols = New Generic.List(Of QuickQuoteAutoSymbol)
            '_AutoSymbols = Nothing 'added 8/4/2014
            '_BlanketRatingOptionId = ""
            ''_Coverages = New Generic.List(Of QuickQuoteCoverage)
            '_Coverages = Nothing 'added 8/4/2014
            '_DeductiblePerTypeId = ""
            ''_GLClassifications = New Generic.List(Of QuickQuoteGLClassification)
            '_GLClassifications = Nothing 'added 8/4/2014
            ''_InclusionsExclusions = New Generic.List(Of QuickQuoteInclusionExclusion)
            '_InclusionsExclusions = Nothing 'added 8/4/2014
            ''_LossHistoryRecords = New Generic.List(Of QuickQuoteLossHistoryRecord)
            '_LossHistoryRecords = Nothing 'added 8/4/2014
            ''_Modifiers = New Generic.List(Of QuickQuoteModifier)
            '_Modifiers = Nothing 'added 8/4/2014
            '_PackageModificationAssignmentTypeId = ""
            '_PackageTypeId = ""
            '_PolicyTypeId = ""
            ''_PolicyUnderwritings = New Generic.List(Of QuickQuotePolicyUnderwriting)
            '_PolicyUnderwritings = Nothing 'added 8/4/2014
            ''_PolicyUnderwritingCodeIds = New Generic.List(Of String)
            ''_PolicyUnderwritingCodeAndLevelAndTabIds = New Generic.List(Of String)
            '_PolicyUnderwritingCodeAndLevelAndTabIds = Nothing 'added 8/4/2014
            '_PriorCarrier = New QuickQuotePriorCarrier
            '_ProgramTypeId = ""
            '_RiskGrade = ""
            '_RiskGradeLookupId = ""
            ''_ScheduledRatings = New Generic.List(Of QuickQuoteScheduledRating)
            '_ScheduledRatings = Nothing 'added 8/4/2014

            'removed 7/23/2018
            ''_Applicants = New Generic.List(Of QuickQuoteApplicant)
            '_Applicants = Nothing 'added 8/4/2014
            ''_Drivers = New Generic.List(Of QuickQuoteDriver)
            '_Drivers = Nothing 'added 8/4/2014
            ''_Locations = New Generic.List(Of QuickQuoteLocation)
            '_Locations = Nothing 'added 8/4/2014
            ''_Vehicles = New Generic.List(Of QuickQuoteVehicle)
            '_Vehicles = Nothing 'added 8/4/2014
            ''_Operators = New List(Of QuickQuoteOperator) 'added 4/9/2015
            '_Operators = Nothing

            'removed 7/23/2018
            ''added 1/26/2015 for CIM
            ''_ScheduledCoverages = New List(Of QuickQuoteScheduledCoverage)
            '_ScheduledCoverages = Nothing

            'removed 7/23/2018
            ''added 4/9/2015 for Crime
            ''_ClassificationCodes = New List(Of QuickQuoteClassificationCode)
            '_ClassificationCodes = Nothing
            '_AggregateLimit = ""
            '_NumberOfEmployees = ""

            'added 6/12/2018; Diamond doesn't have a spot in the UI to see CPP AIs at the policy level, but this is our workaround to have a central list that other spots can pull from
            '_AdditionalInterests = Nothing 'removed 8/6/2018

            'added 7/31/2018
            _PackagePartTypeMasterStateId = 0
            _PackagePartTypeMasterLobId = 0
            _PackagePartTypeStateId = 0
            _PackagePartTypeLobId = 0
            _NeedsToSetMasterAndPartStateAndLobCombosForPackagePartTypeId = False
            _CaughtErrorWhenSettingMasterAndPartStateAndLobCombosForPackagePartTypeId = False
            _OriginalPackagePartTypeId = ""

            _DetailStatusCode = "" 'added 5/15/2019
        End Sub
        Public Overrides Function ToString() As String 'added 6/30/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.PackagePartTypeId <> "" Then
                    Dim t As String = ""
                    t = "PackagePartTypeId: " & Me.PackagePartTypeId
                    If Me.PackagePartType <> "" Then
                        t &= " (" & Me.PackagePartType & ")"
                    End If
                    str = qqHelper.appendText(str, t, vbCrLf)
                End If

                'added 9/6/2018
                Dim strVersionInfo As String = "VersionId: " & Me.VersionId
                strVersionInfo &= "; LobId: " & Me.VersionAndLobInfo.LobId
                If Me.VersionAndLobInfo.LobType <> QuickQuoteObject.QuickQuoteLobType.None Then
                    strVersionInfo &= " (" & System.Enum.GetName(GetType(QuickQuoteObject.QuickQuoteLobType), Me.VersionAndLobInfo.LobType) & ")"
                End If
                If String.IsNullOrWhiteSpace(Me.VersionAndLobInfo.ActualLobId) = False AndAlso QuickQuoteHelperClass.isTextMatch(Me.VersionAndLobInfo.LobId, Me.VersionAndLobInfo.ActualLobId, matchType:=QuickQuoteHelperClass.TextMatchType.IntegerOrText_IgnoreCasing) = False Then
                    strVersionInfo &= "; ActualLobId: " & Me.VersionAndLobInfo.ActualLobId
                End If
                strVersionInfo &= "; StateId: " & Me.VersionAndLobInfo.StateId
                If Me.VersionAndLobInfo.State <> QuickQuoteHelperClass.QuickQuoteState.None Then
                    strVersionInfo &= " (" & System.Enum.GetName(GetType(QuickQuoteHelperClass.QuickQuoteState), Me.VersionAndLobInfo.State) & ")"
                End If
                str = qqHelper.appendText(str, strVersionInfo, vbCrLf)

                If Me.FullTermPremium <> "" Then
                    str = qqHelper.appendText(str, "FullTermPremium: " & Me.FullTermPremium, vbCrLf)
                End If
            Else
                str = "Nothing"
            End If
            Return str
        End Function

        'added 7/31/2018
        Private Sub SetMasterAndPartStateAndLobCombosForPackagePartTypeId()
            QuickQuoteHelperClass.SetMasterAndPartStateAndLobCombosForPackagePartTypeId(qqHelper.IntegerForString(_PackagePartTypeId), _PackagePartTypeMasterStateId, _PackagePartTypeMasterLobId, _PackagePartTypeStateId, _PackagePartTypeLobId, errorOnLookup:=_CaughtErrorWhenSettingMasterAndPartStateAndLobCombosForPackagePartTypeId)
            If _CaughtErrorWhenSettingMasterAndPartStateAndLobCombosForPackagePartTypeId = False Then
                _NeedsToSetMasterAndPartStateAndLobCombosForPackagePartTypeId = False
            End If
        End Sub
        Protected Friend Sub Set_PackagePartTypeId_Variable(ByVal ppTypeId As String, Optional ByVal setPackagePartType As Boolean = True)
            _PackagePartTypeId = ppTypeId

            If setPackagePartType = True Then
                _PackagePartType = ""
                If IsNumeric(_PackagePartTypeId) = True Then
                    Select Case _PackagePartTypeId
                        Case "0"
                            _PackagePartType = "N/A"
                        Case "1"
                            _PackagePartType = "Property"
                        Case "2"
                            _PackagePartType = "General Liability"
                        Case "3"
                            _PackagePartType = "Inland Marine"
                        Case "4"
                            _PackagePartType = "Crime"
                        Case "5"
                            _PackagePartType = "Garage"
                        Case "6"
                            _PackagePartType = "Package"
                    End Select
                End If
            End If
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        'Protected Overridable Sub Dispose(disposing As Boolean)
        'updated 8/4/2014 w/ QuickQuoteBaseObject inheritance
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    If _FullTermPremium IsNot Nothing Then
                        _FullTermPremium = Nothing
                    End If
                    If _PolicyId IsNot Nothing Then
                        _PolicyId = Nothing
                    End If
                    If _PolicyImageNum IsNot Nothing Then
                        _PolicyImageNum = Nothing
                    End If
                    'If _AddFormsVersionId IsNot Nothing Then 'removed 7/23/2018
                    '    _AddFormsVersionId = Nothing
                    'End If
                    If _Num IsNot Nothing Then
                        _Num = Nothing
                    End If
                    If _PackagePartTypeId IsNot Nothing Then
                        _PackagePartTypeId = Nothing
                    End If
                    If _PackagePartType IsNot Nothing Then
                        _PackagePartType = Nothing
                    End If
                    'If _RatingVersionId IsNot Nothing Then 'removed 7/23/2018
                    '    _RatingVersionId = Nothing
                    'End If
                    'If _UnderwritingVersionId IsNot Nothing Then 'removed 7/23/2018
                    '    _UnderwritingVersionId = Nothing
                    'End If
                    'If _VersionId IsNot Nothing Then 'removed 7/23/2018
                    '    _VersionId = Nothing
                    'End If

                    'removed 7/23/2018
                    'If _AnniversaryRatingEffectiveDate IsNot Nothing Then
                    '    _AnniversaryRatingEffectiveDate = Nothing
                    'End If
                    'If _AnniversaryRatingExpirationDate IsNot Nothing Then
                    '    _AnniversaryRatingExpirationDate = Nothing
                    'End If
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
                    'If _BlanketRatingOptionId IsNot Nothing Then
                    '    _BlanketRatingOptionId = Nothing
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
                    'If _DeductiblePerTypeId IsNot Nothing Then
                    '    _DeductiblePerTypeId = Nothing
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
                    'If _PackageModificationAssignmentTypeId IsNot Nothing Then
                    '    _PackageModificationAssignmentTypeId = Nothing
                    'End If
                    'If _PackageTypeId IsNot Nothing Then
                    '    _PackageTypeId = Nothing
                    'End If
                    'If _PolicyTypeId IsNot Nothing Then
                    '    _PolicyTypeId = Nothing
                    'End If
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
                    'If _ProgramTypeId IsNot Nothing Then
                    '    _ProgramTypeId = Nothing
                    'End If
                    'If _RiskGrade IsNot Nothing Then
                    '    _RiskGrade = Nothing
                    'End If
                    'If _RiskGradeLookupId IsNot Nothing Then
                    '    _RiskGradeLookupId = Nothing
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

                    'removed 7/23/2018
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
                    'If _Operators IsNot Nothing Then 'added 4/9/2015
                    '    If _Operators.Count > 0 Then
                    '        For Each o As QuickQuoteOperator In _Operators
                    '            o.Dispose()
                    '            o = Nothing
                    '        Next
                    '        _Operators.Clear()
                    '    End If
                    '    _Operators = Nothing
                    'End If

                    'removed 7/23/2018
                    ''added 1/26/2015 for CIM
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

                    'removed 7/23/2018
                    ''added 4/9/2015 for Crime
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
                    'qqHelper.DisposeString(_AggregateLimit)
                    'qqHelper.DisposeString(_NumberOfEmployees)

                    'added 6/12/2018; Diamond doesn't have a spot in the UI to see CPP AIs at the policy level, but this is our workaround to have a central list that other spots can pull from
                    'qqHelper.DisposeAdditionalInterests(_AdditionalInterests) 'removed 8/6/2018

                    'added 6/25/2018; updated 6/27/2018 to use VersionAndLobInfo object instead of LobInfo
                    If _VersionAndLobInfo IsNot Nothing Then
                        _VersionAndLobInfo.Dispose()
                        _VersionAndLobInfo = Nothing
                    End If

                    'added 7/31/2018
                    _PackagePartTypeMasterStateId = Nothing
                    _PackagePartTypeMasterLobId = Nothing
                    _PackagePartTypeStateId = Nothing
                    _PackagePartTypeLobId = Nothing
                    _NeedsToSetMasterAndPartStateAndLobCombosForPackagePartTypeId = Nothing
                    _CaughtErrorWhenSettingMasterAndPartStateAndLobCombosForPackagePartTypeId = Nothing
                    qqHelper.DisposeString(_OriginalPackagePartTypeId)

                    qqHelper.DisposeString(_DetailStatusCode) 'added 5/15/2019

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
