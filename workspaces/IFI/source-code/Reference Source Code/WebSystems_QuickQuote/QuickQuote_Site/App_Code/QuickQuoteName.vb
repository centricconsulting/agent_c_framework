Imports System.Configuration
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store name information
    ''' </summary>
    ''' <remarks>used w/ several different objects... policyholders, clients, locations, etc.</remarks>
    <Serializable()> _
    Public Class QuickQuoteName
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _PolicyId As String
        Private _PolicyImageNum As String
        Private _BirthDate As String
        Private _CommercialDBAname As String 'name1
        Private _CommercialIRSname As String 'name2
        Private _DriversLicenseDate As String
        Private _DriversLicenseNumber As String
        Private _DriversLicenseStateId As String
        Private _DisplayName As String
        Private _DisplayNameForWeb As String 'added 9/5/2012 to append DBA text when needed
        Private _FirstName As String
        Private _LastName As String
        Private _MaritalStatusId As String
        Private _MiddleName As String
        Private _NameAddressSourceId As String
        Private _NameId As String
        Private _NameNum As String
        Private _PositionTitle As String
        Private _PrefixName As String
        Private _SexId As String
        Private _SortName As String
        Private _SuffixName As String

        Private _TaxNumber As String
        Private _TaxTypeId As String
        Private _ThirdPartyEntityId As String
        Private _ThirdPartyGroupId As String
        Private _TypeId As String

        'removed HasData private variables 5/7/2014
        'Private _HasData As Boolean
        'Private _HasDataWasManuallySet As Boolean

        Private _DoingBusinessAsName As String

        Private _DisplayNameWasManuallySet As Boolean

        'added 7/17/2012 for WC
        Private _EntityTypeId As String
        Private _EntityType As String

        'added 7/18/2012 for Client
        Private _DescriptionOfOperations As String

        Private _SortNameWasManuallySet As Boolean 'added 10/15/2012 to make names show in workflow queue

        'Private _TestNameField As String '*added 3/5/2013 for testing arbitrary fields in xml

        Private _TaxNumber_NoHyphens As String 'added 4/10/2013 since Diamond can no longer handle hyphens
        Private _TaxNumber_Hyphens As String 'added 4/11/2013
        Private _TaxNumber_Entered As String 'added 4/11/2013; *(stored in xml but not valid; no spot in Diamond)
        'added 4/11/2013 for same information that uses different fields for formatting (like TaxNumber props)
        Private _OnlyUsePropertyToSetFieldWithSameName As Boolean
        Private _TaxTypeId_Entered As String 'added 4/11/2013; *(stored in xml but not valid; no spot in Diamond)

        Private _Salutation As String 'added 7/21/2015 for Farm; used to store ResidentName relationship (free-flow text field)

        'added 7/23/2015 for Commercial
        Private _DateBusinessStarted As String 'date
        Private _YearsOfExperience As String 'int

        Private _DetailStatusCode As String 'added 5/15/2019

        Private _OtherLegalEntityDescription As String 'Added 2/15/2022 for bug 63511 MLW

        Private _NAICS As String

        Public Property PolicyId As String
            Get
                Return _PolicyId
            End Get
            Set(value As String)
                _PolicyId = value
                'If _PolicyId <> "" Then '9/6/2012 - added conditional to make sure something was there first
                '    CheckHasData()
                'End If
            End Set
        End Property
        Public Property PolicyImageNum As String
            Get
                Return _PolicyImageNum
            End Get
            Set(value As String)
                _PolicyImageNum = value
                'If _PolicyImageNum <> "" Then
                '    CheckHasData()
                'End If
            End Set
        End Property
        Public Property BirthDate As String
            Get
                Return _BirthDate
            End Get
            Set(value As String)
                _BirthDate = value
                qqHelper.ConvertToShortDate(_BirthDate)
                'If _BirthDate <> "" Then
                '    CheckHasData()
                'End If
            End Set
        End Property
        Public Property CommercialDBAname As String
            Get
                Return _CommercialDBAname
            End Get
            Set(value As String)
                _CommercialDBAname = value
                _DisplayNameWasManuallySet = False 'added 12/24/2012 to make sure display name is updated whenever something changes
                _SortNameWasManuallySet = False 'added 12/24/2012 to make sure sort name is updated whenever something changes
                'If _CommercialDBAname <> "" Then
                '    CheckHasData()
                'End If
            End Set
        End Property
        Public Property CommercialIRSname As String
            Get
                Return _CommercialIRSname
            End Get
            Set(value As String)
                _CommercialIRSname = value
                _DisplayNameWasManuallySet = False 'added 12/24/2012 to make sure display name is updated whenever something changes
                _SortNameWasManuallySet = False 'added 12/24/2012 to make sure sort name is updated whenever something changes
                'If _CommercialIRSname <> "" Then
                '    CheckHasData()
                'End If
            End Set
        End Property
        '8/16/2012 - added duplicate properties for CommercialName1 and CommercialName2
        Public Property CommercialName1 As String
            Get
                Return _CommercialDBAname
            End Get
            Set(value As String)
                _CommercialDBAname = value
                _DisplayNameWasManuallySet = False 'added 12/24/2012 to make sure display name is updated whenever something changes
                _SortNameWasManuallySet = False 'added 12/24/2012 to make sure sort name is updated whenever something changes
                'If _CommercialDBAname <> "" Then
                '    CheckHasData()
                'End If
            End Set
        End Property
        Public Property CommercialName2 As String
            Get
                Return _CommercialIRSname
            End Get
            Set(value As String)
                _CommercialIRSname = value
                _DisplayNameWasManuallySet = False 'added 12/24/2012 to make sure display name is updated whenever something changes
                _SortNameWasManuallySet = False 'added 12/24/2012 to make sure sort name is updated whenever something changes
                'If _CommercialIRSname <> "" Then
                '    CheckHasData()
                'End If
            End Set
        End Property
        Public Property DriversLicenseDate As String
            Get
                Return _DriversLicenseDate
            End Get
            Set(value As String)
                _DriversLicenseDate = value
                qqHelper.ConvertToShortDate(_DriversLicenseDate)
                'If _DriversLicenseDate <> "" Then
                '    CheckHasData()
                'End If
            End Set
        End Property
        Public Property DriversLicenseNumber As String
            Get
                Return _DriversLicenseNumber
            End Get
            Set(value As String)
                _DriversLicenseNumber = value
                'If _DriversLicenseNumber <> "" Then
                '    CheckHasData()
                'End If
            End Set
        End Property
        Public Property DriversLicenseStateId As String
            Get
                Return _DriversLicenseStateId
            End Get
            Set(value As String)
                _DriversLicenseStateId = value
                'If _DriversLicenseStateId <> "" Then
                '    CheckHasData()
                'End If
            End Set
        End Property
        ''' <summary>
        ''' used for display name in Diamond
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>automatically calculated when something changes; doesn't need to be set by developer code</remarks>
        Public Property DisplayName As String
            Get
                'If _DisplayNameWasManuallySet = False Then
                'updated 4/22/2013 to recalculate display name if it incorrectly includes/excludes doing_business_as (to handle for existing/copied quotes)
                If _DisplayNameWasManuallySet = False OrElse (_DoingBusinessAsName <> "" AndAlso ConfigurationManager.AppSettings("QuickQuote_DisplayNames_Exclude_DoingBusinessAs") IsNot Nothing AndAlso ((UCase(ConfigurationManager.AppSettings("QuickQuote_DisplayNames_Exclude_DoingBusinessAs").ToString) = "YES" AndAlso UCase(_DisplayName).Contains(UCase(_DoingBusinessAsName)) = True) OrElse (UCase(ConfigurationManager.AppSettings("QuickQuote_DisplayNames_Exclude_DoingBusinessAs").ToString) = "NO" AndAlso UCase(_DisplayName).Contains(UCase(_DoingBusinessAsName)) = False))) Then
                    SetDisplayName()
                End If
                Return _DisplayName
            End Get
            Set(value As String)
                _DisplayName = value
                '_DisplayNameWasManuallySet = True
                If _DisplayName <> "" Then
                    _DisplayNameWasManuallySet = True '10/15/2012 - moved inside conditional
                    'CheckHasData()
                End If
            End Set
        End Property
        ''' <summary>
        ''' similar to display address in Diamond but always contains line feeds and doing business as name when applicable
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>automatically calculated whenever used; doesn't need to be set by developer code</remarks>
        Public Property DisplayNameForWeb As String
            Get
                SetDisplayNameForWeb()
                Return _DisplayNameForWeb
            End Get
            Set(value As String)
                _DisplayNameForWeb = value
            End Set
        End Property
        Public Property FirstName As String
            Get
                Return _FirstName
            End Get
            Set(value As String)
                _FirstName = value
                _DisplayNameWasManuallySet = False 'added 12/24/2012 to make sure display name is updated whenever something changes
                _SortNameWasManuallySet = False 'added 12/24/2012 to make sure sort name is updated whenever something changes
                'If _FirstName <> "" Then
                '    CheckHasData()
                'End If
            End Set
        End Property
        Public Property LastName As String
            Get
                Return _LastName
            End Get
            Set(value As String)
                _LastName = value
                _DisplayNameWasManuallySet = False 'added 12/24/2012 to make sure display name is updated whenever something changes
                _SortNameWasManuallySet = False 'added 12/24/2012 to make sure sort name is updated whenever something changes
                'If _LastName <> "" Then
                '    CheckHasData()
                'End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's MaritalStatus table (1=Single, 2=Married, 3=Divorced, 4=Widowed)</remarks>
        Public Property MaritalStatusId As String '7/24/2013: 1=Single, 2=Married, 3=Divorced, 4=Widowed
            Get
                Return _MaritalStatusId
            End Get
            Set(value As String)
                _MaritalStatusId = value
                'If _MaritalStatusId <> "" Then
                '    CheckHasData()
                'End If
            End Set
        End Property
        Public Property MiddleName As String
            Get
                Return _MiddleName
            End Get
            Set(value As String)
                _MiddleName = value
                _DisplayNameWasManuallySet = False 'added 12/24/2012 to make sure display name is updated whenever something changes
                _SortNameWasManuallySet = False 'added 12/24/2012 to make sure sort name is updated whenever something changes
                'If _MiddleName <> "" Then
                '    CheckHasData()
                'End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's NameAddressSource table</remarks>
        Public Property NameAddressSourceId As String
            Get
                Return _NameAddressSourceId
            End Get
            Set(value As String)
                _NameAddressSourceId = value
                'CheckHasData()
            End Set
        End Property
        Public Property NameId As String
            Get
                Return _NameId
            End Get
            Set(value As String)
                _NameId = value
                'If _NameId <> "" Then
                '    CheckHasData()
                'End If
            End Set
        End Property
        Public Property NameNum As String
            Get
                Return _NameNum
            End Get
            Set(value As String)
                _NameNum = value
                'If _NameNum <> "" Then
                '    CheckHasData()
                'End If
            End Set
        End Property
        Public Property PositionTitle As String
            Get
                Return _PositionTitle
            End Get
            Set(value As String)
                _PositionTitle = value
                'If _PositionTitle <> "" Then
                '    CheckHasData()
                'End If
            End Set
        End Property
        Public Property PrefixName As String
            Get
                Return _PrefixName
            End Get
            Set(value As String)
                _PrefixName = value
                _DisplayNameWasManuallySet = False 'added 12/24/2012 to make sure display name is updated whenever something changes
                _SortNameWasManuallySet = False 'added 12/24/2012 to make sure sort name is updated whenever something changes
                'If _PrefixName <> "" Then
                '    CheckHasData()
                'End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's Sex table (1=Male, 2=Female)</remarks>
        Public Property SexId As String
            Get
                Return _SexId
            End Get
            Set(value As String)
                _SexId = value
                'If _SexId <> "" Then
                '    CheckHasData()
                'End If
            End Set
        End Property
        Public Property SortName As String
            Get
                'If _SortNameWasManuallySet = False Then 'added 10/15/2012
                'updated 4/22/2013 to recalculate display name if it incorrectly includes/excludes doing_business_as (to handle for existing/copied quotes)
                If _SortNameWasManuallySet = False OrElse (_DoingBusinessAsName <> "" AndAlso ConfigurationManager.AppSettings("QuickQuote_SortNames_Exclude_DoingBusinessAs") IsNot Nothing AndAlso ((UCase(ConfigurationManager.AppSettings("QuickQuote_SortNames_Exclude_DoingBusinessAs").ToString) = "YES" AndAlso UCase(_SortName).Contains(UCase(_DoingBusinessAsName)) = True) OrElse (UCase(ConfigurationManager.AppSettings("QuickQuote_SortNames_Exclude_DoingBusinessAs").ToString) = "NO" AndAlso UCase(_SortName).Contains(UCase(_DoingBusinessAsName)) = False))) Then
                    SetSortName()
                End If
                Return _SortName
            End Get
            Set(value As String)
                _SortName = value
                '_SortNameWasManuallySet = True 'added 10/15/2012
                If _SortName <> "" Then
                    _SortNameWasManuallySet = True '10/15/2012 - moved inside conditional
                    'CheckHasData()
                End If
            End Set
        End Property
        Public Property SuffixName As String
            Get
                Return _SuffixName
            End Get
            Set(value As String)
                _SuffixName = value
                _DisplayNameWasManuallySet = False 'added 12/24/2012 to make sure display name is updated whenever something changes
                _SortNameWasManuallySet = False 'added 12/24/2012 to make sure sort name is updated whenever something changes
                'If _SuffixName <> "" Then
                '    CheckHasData()
                'End If
            End Set
        End Property

        Public Property TaxNumber As String
            Get
                'Return _TaxNumber
                'updated 4/11/2013 to always return to developer code w/ correct hyphens
                Return TaxNumber_Hyphens
            End Get
            Set(value As String)
                _TaxNumber = value
                'updated 4/11/2013 (sets both when set by developer; only sets 1 when read from xml)
                If _OnlyUsePropertyToSetFieldWithSameName = False Then
                    _TaxNumber_Entered = value
                Else 'don't set other value; set flag back to False after
                    _OnlyUsePropertyToSetFieldWithSameName = False
                End If
                _TaxNumber_NoHyphens = _TaxNumber.Replace("-", "") 'added 4/10/2013 since Diamond can no longer handle hyphens

                ''added 4/11/2013
                '_TaxNumber_Hyphens = _TaxNumber
                'If _TaxNumber_Hyphens.Contains("-") = True Then
                '    'don't do any formatting
                '    If _TaxTypeId = "" AndAlso IsNumeric(_TaxNumber_Hyphens.Replace("-", "")) = True Then
                '        Dim r As New Regex("-")
                '        Dim i As Integer = r.Matches(_TaxNumber_Hyphens).Count
                '        If Len(_TaxNumber_Hyphens) = 11 AndAlso i = 2 AndAlso _TaxNumber_Hyphens.IndexOf("-") = 3 AndAlso _TaxNumber_Hyphens.LastIndexOf("-") = 6 Then
                '            _TaxTypeId = "1"
                '        ElseIf Len(_TaxNumber_Hyphens) = 10 AndAlso i = 1 AndAlso _TaxNumber_Hyphens.IndexOf("-") = 2 Then
                '            _TaxTypeId = "2"
                '        End If
                '    End If
                'Else
                '    'no hyphens; format if possible
                '    If Len(_TaxNumber_Hyphens) = 9 AndAlso IsNumeric(_TaxTypeId) = True Then
                '        If _TaxTypeId = "1" Then
                '            _TaxNumber_Hyphens = Left(_TaxNumber_Hyphens, 3) & "-" & Mid(_TaxNumber_Hyphens, 4, 2) & "-" & Right(_TaxNumber_Hyphens, 4)
                '        ElseIf _TaxTypeId = "2" Then
                '            _TaxNumber_Hyphens = Left(_TaxNumber_Hyphens, 2) & "-" & Right(_TaxNumber_Hyphens, 7)
                '        End If
                '    End If
                'End If

                'If _TaxNumber <> "" Then
                '    CheckHasData()
                'End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's TaxType table (0=Not Assigned, 1=SSN, 2=FEIN, 3=SIN, 4=BN)</remarks>
        Public Property TaxTypeId As String 'note 4/11/2013: 0=Not Assigned, 1=SSN, 2=FEIN, 3=SIN, 4=BN
            Get
                'added 4/11/2013
                If _TaxTypeId = "" AndAlso _TaxNumber.Contains("-") = True AndAlso IsNumeric(_TaxNumber.Replace("-", "")) = True Then
                    Dim r As New Regex("-")
                    Dim i As Integer = r.Matches(_TaxNumber).Count
                    If Len(_TaxNumber) = 11 AndAlso i = 2 AndAlso _TaxNumber.IndexOf("-") = 3 AndAlso _TaxNumber.LastIndexOf("-") = 6 Then
                        _TaxTypeId = "1"
                    ElseIf Len(_TaxNumber) = 10 AndAlso i = 1 AndAlso _TaxNumber.IndexOf("-") = 2 Then
                        _TaxTypeId = "2"
                    End If
                End If

                Return _TaxTypeId
            End Get
            Set(value As String)
                _TaxTypeId = value
                'updated 4/11/2013 (sets both when set by developer; only sets 1 when read from xml)
                If _OnlyUsePropertyToSetFieldWithSameName = False Then
                    _TaxTypeId_Entered = value
                Else 'don't set other value; set flag back to False after
                    _OnlyUsePropertyToSetFieldWithSameName = False
                End If

                'If _TaxTypeId <> "" Then
                '    CheckHasData()
                'End If
            End Set
        End Property
        Public Property ThirdPartyEntityId As String '12/11/2013 note: maybe Diamond's ThirdParty table; also ThirdPartyType for functionality types (i.e. Motor Vehicle Records, Auto Data Prefill, etc.)
            Get
                Return _ThirdPartyEntityId
            End Get
            Set(value As String)
                _ThirdPartyEntityId = value
                'If _ThirdPartyEntityId <> "" Then
                '    CheckHasData()
                'End If
            End Set
        End Property
        Public Property ThirdPartyGroupId As String
            Get
                Return _ThirdPartyGroupId
            End Get
            Set(value As String)
                _ThirdPartyGroupId = value
                'If _ThirdPartyGroupId <> "" Then
                '    CheckHasData()
                'End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's NameType table (0=Not Assigned, 1=Personal Name, 2=Commercial)</remarks>
        Public Property TypeId As String
            Get
                Return _TypeId
            End Get
            Set(value As String)
                _TypeId = value
                'If _TypeId <> "" Then
                '    CheckHasData()
                'End If
            End Set
        End Property

        'Public Property HasData As Boolean
        '    Get
        '        Return _HasData
        '    End Get
        '    Set(value As Boolean)
        '        _HasData = value
        '        _HasDataWasManuallySet = True
        '    End Set
        'End Property
        'replaced HasData property 5/7/2014
        Public ReadOnly Property HasData As Boolean
            Get
                'If _PolicyId <> "" OrElse _PolicyImageNum <> "" OrElse _BirthDate <> "" OrElse _CommercialDBAname <> "" OrElse _CommercialIRSname <> "" OrElse _DriversLicenseDate <> "" OrElse _DriversLicenseNumber <> "" OrElse _DriversLicenseStateId <> "" OrElse _DisplayName <> "" OrElse _FirstName <> "" OrElse _LastName <> "" OrElse _MaritalStatusId <> "" OrElse _MiddleName <> "" OrElse _NameId <> "" OrElse _NameNum <> "" OrElse _PositionTitle <> "" OrElse _PrefixName <> "" OrElse _SexId <> "" OrElse _SortName <> "" OrElse _SuffixName <> "" OrElse _TaxNumber <> "" OrElse _TaxTypeId <> "" OrElse _ThirdPartyEntityId <> "" OrElse _ThirdPartyGroupId <> "" OrElse _TypeId <> "" OrElse _DoingBusinessAsName <> "" OrElse _EntityTypeId <> "" OrElse _EntityType <> "" OrElse _DescriptionOfOperations <> "" Then
                'now just checking props that are used... no _PolicyId, _PolicyImageNum, _DisplayName, _NameId, _NameNum, _SortName, _EntityType
                'might also eventually check IsNumeric for integers and IsDate for dates... may also need to check for integers > 0 since we're now parsing rated xml, which could have default values of 0 when nothing was ever set
                'If _BirthDate <> "" OrElse _CommercialDBAname <> "" OrElse _CommercialIRSname <> "" OrElse _DriversLicenseDate <> "" OrElse _DriversLicenseNumber <> "" OrElse _DriversLicenseStateId <> "" OrElse _FirstName <> "" OrElse _LastName <> "" OrElse _MaritalStatusId <> "" OrElse _MiddleName <> "" OrElse _PositionTitle <> "" OrElse _PrefixName <> "" OrElse _SexId <> "" OrElse _SuffixName <> "" OrElse _TaxNumber <> "" OrElse _TaxTypeId <> "" OrElse _ThirdPartyEntityId <> "" OrElse _ThirdPartyGroupId <> "" OrElse _TypeId <> "" OrElse _DoingBusinessAsName <> "" OrElse _EntityTypeId <> "" OrElse _DescriptionOfOperations <> "" Then
                'updated 6/3/2014 to check IsNumeric and IsDate where needed... also > 0 on ints
                'If (_BirthDate <> "" AndAlso IsDate(_BirthDate) = True) OrElse _CommercialDBAname <> "" OrElse _CommercialIRSname <> "" OrElse (_DriversLicenseDate <> "" AndAlso IsDate(_DriversLicenseDate) = True) OrElse _DriversLicenseNumber <> "" OrElse qqHelper.IsValidQuickQuoteIdOrNum(_DriversLicenseStateId) = True OrElse _FirstName <> "" OrElse _LastName <> "" OrElse qqHelper.IsValidQuickQuoteIdOrNum(_MaritalStatusId) = True OrElse _MiddleName <> "" OrElse _PositionTitle <> "" OrElse _PrefixName <> "" OrElse qqHelper.IsValidQuickQuoteIdOrNum(_SexId) = True OrElse _SuffixName <> "" OrElse _TaxNumber <> "" OrElse qqHelper.IsValidQuickQuoteIdOrNum(_TaxTypeId) = True OrElse qqHelper.IsValidQuickQuoteIdOrNum(_ThirdPartyEntityId) = True OrElse qqHelper.IsValidQuickQuoteIdOrNum(_ThirdPartyGroupId) = True OrElse qqHelper.IsValidQuickQuoteIdOrNum(_TypeId) = True OrElse _DoingBusinessAsName <> "" OrElse qqHelper.IsValidQuickQuoteIdOrNum(_EntityTypeId) = True OrElse _DescriptionOfOperations <> "" Then
                'updated 7/21/2015 for salutation
                'If (_BirthDate <> "" AndAlso IsDate(_BirthDate) = True) OrElse _CommercialDBAname <> "" OrElse _CommercialIRSname <> "" OrElse (_DriversLicenseDate <> "" AndAlso IsDate(_DriversLicenseDate) = True) OrElse _DriversLicenseNumber <> "" OrElse qqHelper.IsValidQuickQuoteIdOrNum(_DriversLicenseStateId) = True OrElse _FirstName <> "" OrElse _LastName <> "" OrElse qqHelper.IsValidQuickQuoteIdOrNum(_MaritalStatusId) = True OrElse _MiddleName <> "" OrElse _PositionTitle <> "" OrElse _PrefixName <> "" OrElse qqHelper.IsValidQuickQuoteIdOrNum(_SexId) = True OrElse _SuffixName <> "" OrElse _TaxNumber <> "" OrElse qqHelper.IsValidQuickQuoteIdOrNum(_TaxTypeId) = True OrElse qqHelper.IsValidQuickQuoteIdOrNum(_ThirdPartyEntityId) = True OrElse qqHelper.IsValidQuickQuoteIdOrNum(_ThirdPartyGroupId) = True OrElse qqHelper.IsValidQuickQuoteIdOrNum(_TypeId) = True OrElse _DoingBusinessAsName <> "" OrElse qqHelper.IsValidQuickQuoteIdOrNum(_EntityTypeId) = True OrElse _DescriptionOfOperations <> "" OrElse _Salutation <> "" Then
                'updated 7/23/2015 for DateBusinessStarted and YearsOfExperience
                'If (_BirthDate <> "" AndAlso IsDate(_BirthDate) = True) OrElse _CommercialDBAname <> "" OrElse _CommercialIRSname <> "" OrElse (_DriversLicenseDate <> "" AndAlso IsDate(_DriversLicenseDate) = True) OrElse _DriversLicenseNumber <> "" OrElse qqHelper.IsValidQuickQuoteIdOrNum(_DriversLicenseStateId) = True OrElse _FirstName <> "" OrElse _LastName <> "" OrElse qqHelper.IsValidQuickQuoteIdOrNum(_MaritalStatusId) = True OrElse _MiddleName <> "" OrElse _PositionTitle <> "" OrElse _PrefixName <> "" OrElse qqHelper.IsValidQuickQuoteIdOrNum(_SexId) = True OrElse _SuffixName <> "" OrElse _TaxNumber <> "" OrElse qqHelper.IsValidQuickQuoteIdOrNum(_TaxTypeId) = True OrElse qqHelper.IsValidQuickQuoteIdOrNum(_ThirdPartyEntityId) = True OrElse qqHelper.IsValidQuickQuoteIdOrNum(_ThirdPartyGroupId) = True OrElse qqHelper.IsValidQuickQuoteIdOrNum(_TypeId) = True OrElse _DoingBusinessAsName <> "" OrElse qqHelper.IsValidQuickQuoteIdOrNum(_EntityTypeId) = True OrElse _DescriptionOfOperations <> "" OrElse _Salutation <> "" OrElse qqHelper.IsDateString(_DateBusinessStarted) = True OrElse qqHelper.IsValidQuickQuoteIdOrNum(_YearsOfExperience) = True Then
                'updated date checks on 12/20/2018
                If qqHelper.IsValidDateString(_BirthDate, mustBeGreaterThanDefaultDate:=True) = True OrElse _CommercialDBAname <> "" OrElse _CommercialIRSname <> "" OrElse qqHelper.IsValidDateString(_DriversLicenseDate, mustBeGreaterThanDefaultDate:=True) = True OrElse (String.IsNullOrWhiteSpace(_DriversLicenseNumber) = False AndAlso String.IsNullOrWhiteSpace(Replace(Replace(_DriversLicenseNumber, "0", ""), "-", "")) = False) OrElse qqHelper.IsValidQuickQuoteIdOrNum(_DriversLicenseStateId) = True OrElse _FirstName <> "" OrElse _LastName <> "" OrElse qqHelper.IsValidQuickQuoteIdOrNum(_MaritalStatusId) = True OrElse _MiddleName <> "" OrElse _PositionTitle <> "" OrElse _PrefixName <> "" OrElse qqHelper.IsValidQuickQuoteIdOrNum(_SexId) = True OrElse _SuffixName <> "" OrElse (String.IsNullOrWhiteSpace(_TaxNumber) = False AndAlso String.IsNullOrWhiteSpace(Replace(Replace(_TaxNumber, "0", ""), "-", "")) = False) OrElse qqHelper.IsValidQuickQuoteIdOrNum(_TaxTypeId) = True OrElse qqHelper.IsValidQuickQuoteIdOrNum(_ThirdPartyEntityId) = True OrElse qqHelper.IsValidQuickQuoteIdOrNum(_ThirdPartyGroupId) = True OrElse qqHelper.IsValidQuickQuoteIdOrNum(_TypeId) = True OrElse _DoingBusinessAsName <> "" OrElse qqHelper.IsValidQuickQuoteIdOrNum(_EntityTypeId) = True OrElse _DescriptionOfOperations <> "" OrElse _Salutation <> "" OrElse qqHelper.IsValidDateString(_DateBusinessStarted, mustBeGreaterThanDefaultDate:=True) = True OrElse qqHelper.IsValidQuickQuoteIdOrNum(_YearsOfExperience) = True Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property

        Public Property DoingBusinessAsName As String
            Get
                Return _DoingBusinessAsName
            End Get
            Set(value As String)
                _DoingBusinessAsName = value
                _DisplayNameWasManuallySet = False 'added 12/24/2012 to make sure display name is updated whenever something changes
                _SortNameWasManuallySet = False 'added 12/24/2012 to make sure sort name is updated whenever something changes
                'If _DoingBusinessAsName <> "" Then
                '    CheckHasData()
                'End If
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's EntityType table</remarks>
        Public Property EntityTypeId As String
            Get
                Return _EntityTypeId
            End Get
            Set(value As String)
                _EntityTypeId = value
                'If _EntityTypeId <> "" Then
                '    CheckHasData()
                'End If
                '_EntityType = ""
                'If IsNumeric(_EntityTypeId) = True Then
                '    Select Case _EntityTypeId
                '        Case "-1"
                '            _EntityType = "N/A"
                '        Case "0"
                '            _EntityType = "None"
                '        Case "1"
                '            _EntityType = "Individual"
                '        Case "2"
                '            _EntityType = "Partnership"
                '        Case "3"
                '            _EntityType = "Corporation"
                '        Case "4"
                '            _EntityType = "Joint Venture"
                '        Case "5"
                '            _EntityType = "Other"
                '        Case "6"
                '            _EntityType = "Estate or Trust"
                '        Case "7"
                '            _EntityType = "Family Corporation"
                '        Case "8"
                '            _EntityType = "LLC"
                '        Case "9"
                '            _EntityType = "LLP"
                '    End Select
                'End If
                'updated 12/11/2013
                _EntityType = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteName, QuickQuoteHelperClass.QuickQuotePropertyName.EntityTypeId, _EntityTypeId)
            End Set
        End Property
        Public Property EntityType As String
            Get
                Return _EntityType
            End Get
            Set(value As String)
                _EntityType = value
                'If _EntityType <> "" Then
                '    CheckHasData()
                'End If
                'Select Case _EntityType
                '    Case "N/A"
                '        _EntityTypeId = "-1"
                '    Case "None"
                '        _EntityTypeId = "0"
                '    Case "Individual"
                '        _EntityTypeId = "1"
                '    Case "Partnership"
                '        _EntityTypeId = "2"
                '    Case "Corporation"
                '        _EntityTypeId = "3"
                '    Case "Joint Venture"
                '        _EntityTypeId = "4"
                '    Case "Other"
                '        _EntityTypeId = "5"
                '    Case "Estate or Trust"
                '        _EntityTypeId = "6"
                '    Case "Family Corporation"
                '        _EntityTypeId = "7"
                '    Case "LLC"
                '        _EntityTypeId = "8"
                '    Case "LLP"
                '        _EntityTypeId = "9"
                '    Case Else
                '        _EntityTypeId = ""
                'End Select
                'updated 12/11/2013
                _EntityTypeId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteName, QuickQuoteHelperClass.QuickQuotePropertyName.EntityTypeId, _EntityType)
            End Set
        End Property

        Public Property DescriptionOfOperations As String
            Get
                Return _DescriptionOfOperations
            End Get
            Set(value As String)
                _DescriptionOfOperations = value
                'If _DescriptionOfOperations <> "" Then 'added 1/29/2013
                '    CheckHasData()
                'End If
            End Set
        End Property

        'Public Property TestNameField As String
        '    Get
        '        Return _TestNameField
        '    End Get
        '    Set(value As String)
        '        _TestNameField = value
        '    End Set
        'End Property

        Public ReadOnly Property TaxNumber_NoHyphens As String
            Get
                Return _TaxNumber_NoHyphens
            End Get
        End Property
        Public ReadOnly Property TaxNumber_Hyphens As String
            Get
                'added 4/11/2013
                _TaxNumber_Hyphens = _TaxNumber
                If _TaxNumber_Hyphens.Contains("-") = True Then
                    'don't do any formatting
                    'If _TaxTypeId = "" AndAlso IsNumeric(_TaxNumber_Hyphens.Replace("-", "")) = True Then
                    '    Dim r As New Regex("-")
                    '    Dim i As Integer = r.Matches(_TaxNumber_Hyphens).Count
                    '    If Len(_TaxNumber_Hyphens) = 11 AndAlso i = 2 AndAlso _TaxNumber_Hyphens.IndexOf("-") = 3 AndAlso _TaxNumber_Hyphens.LastIndexOf("-") = 6 Then
                    '        _TaxTypeId = "1"
                    '    ElseIf Len(_TaxNumber_Hyphens) = 10 AndAlso i = 1 AndAlso _TaxNumber_Hyphens.IndexOf("-") = 2 Then
                    '        _TaxTypeId = "2"
                    '    End If
                    'End If
                Else
                    'no hyphens; format if possible
                    If Len(_TaxNumber_Hyphens) = 9 AndAlso IsNumeric(_TaxTypeId) = True Then
                        If _TaxTypeId = "1" Then
                            _TaxNumber_Hyphens = Left(_TaxNumber_Hyphens, 3) & "-" & Mid(_TaxNumber_Hyphens, 4, 2) & "-" & Right(_TaxNumber_Hyphens, 4)
                        ElseIf _TaxTypeId = "2" Then
                            _TaxNumber_Hyphens = Left(_TaxNumber_Hyphens, 2) & "-" & Right(_TaxNumber_Hyphens, 7)
                        End If
                    End If
                End If

                Return _TaxNumber_Hyphens
            End Get
        End Property
        Public Property TaxNumber_Entered As String
            Get
                Return _TaxNumber_Entered
            End Get
            Set(value As String)
                _TaxNumber_Entered = value
            End Set
        End Property
        Public Property OnlyUsePropertyToSetFieldWithSameName As Boolean
            Get
                Return _OnlyUsePropertyToSetFieldWithSameName
            End Get
            Set(value As Boolean)
                _OnlyUsePropertyToSetFieldWithSameName = value
            End Set
        End Property
        Public Property TaxTypeId_Entered As String
            Get
                Return _TaxTypeId_Entered
            End Get
            Set(value As String)
                _TaxTypeId_Entered = value
            End Set
        End Property

        Public Property Salutation As String 'added 7/21/2015 for Farm; used to store ResidentName relationship (free-flow text field)
            Get
                Return _Salutation
            End Get
            Set(value As String)
                _Salutation = value
            End Set
        End Property

        'added 7/23/2015 for Commercial
        Public Property DateBusinessStarted As String 'date
            Get
                Return _DateBusinessStarted
            End Get
            Set(value As String)
                _DateBusinessStarted = value
                qqHelper.ConvertToShortDate(_DateBusinessStarted)
            End Set
        End Property
        Public Property YearsOfExperience As String 'int
            Get
                Return _YearsOfExperience
            End Get
            Set(value As String)
                _YearsOfExperience = value
            End Set
        End Property

        Public Property DetailStatusCode As String 'added 5/15/2019
            Get
                Return _DetailStatusCode
            End Get
            Set(value As String)
                _DetailStatusCode = value
            End Set
        End Property

        Public Property OtherLegalEntityDescription As String 'Added 2/15/2022 for bug 63511 MLW
            Get
                Return _OtherLegalEntityDescription
            End Get
            Set(value As String)
                _OtherLegalEntityDescription = value
            End Set
        End Property

        Public Property NAICS As String
            Get
                Return _NAICS
            End Get
            Set(value As String)
                _NAICS = value
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _PolicyId = ""
            _PolicyImageNum = ""
            _BirthDate = ""
            _CommercialDBAname = ""
            _CommercialIRSname = ""
            _DriversLicenseDate = ""
            _DriversLicenseNumber = ""
            _DriversLicenseStateId = ""
            _DisplayName = ""
            _DisplayNameForWeb = ""
            _FirstName = ""
            _LastName = ""
            _MaritalStatusId = ""
            _MiddleName = ""
            _NameAddressSourceId = ""
            _NameId = ""
            _NameNum = ""
            _PositionTitle = ""
            _PrefixName = ""
            _SexId = ""
            _SortName = ""
            _SuffixName = ""

            _TaxNumber = ""
            _TaxTypeId = ""
            _ThirdPartyEntityId = ""
            _ThirdPartyGroupId = ""
            _TypeId = "" '*might default to 0 (Not Assigned; other values are 1 for Personal Name and 2 for Commercial)

            'removed HasData private variables 5/7/2014
            '_HasData = False
            '_HasDataWasManuallySet = False

            _DoingBusinessAsName = ""

            _DisplayNameWasManuallySet = False

            _EntityTypeId = ""
            _EntityType = ""

            _DescriptionOfOperations = ""

            _SortNameWasManuallySet = False

            '_TestNameField = ""

            _TaxNumber_NoHyphens = ""
            _TaxNumber_Hyphens = ""
            _TaxNumber_Entered = ""
            _OnlyUsePropertyToSetFieldWithSameName = False
            _TaxTypeId_Entered = ""

            _Salutation = "" 'added 7/21/2015 for Farm; used to store ResidentName relationship (free-flow text field)

            'added 7/23/2015 for Commercial
            _DateBusinessStarted = "" 'date
            _YearsOfExperience = "" 'int

            _DetailStatusCode = "" 'added 5/15/2019

            _OtherLegalEntityDescription = "" 'Added 2/15/2022 for bug 63511 MLW

            _NAICS = ""
        End Sub
        'removed HasData private variables 5/7/2014
        'Private Sub CheckHasData()
        '    If _HasData = False AndAlso _HasDataWasManuallySet = False Then
        '        _HasData = True
        '    End If
        'End Sub
        Private Sub SetDisplayName()

            Dim isCommercial As Boolean = False
            If qqHelper.IntegerForString(_TypeId) = 2 AndAlso (String.IsNullOrWhiteSpace(_CommercialDBAname) = False OrElse String.IsNullOrWhiteSpace(_CommercialIRSname) = False) Then
                isCommercial = True
            End If

            If isCommercial = False AndAlso _FirstName <> "" AndAlso _LastName <> "" Then
                _DisplayName = qqHelper.appendText(_PrefixName, _FirstName, " ")
                _DisplayName = qqHelper.appendText(_DisplayName, _MiddleName, " ")
                _DisplayName = qqHelper.appendText(_DisplayName, _LastName, " ")
                _DisplayName = qqHelper.appendText(_DisplayName, _SuffixName, " ")
            ElseIf _CommercialDBAname <> "" OrElse _CommercialIRSname <> "" OrElse _DoingBusinessAsName <> "" Then
                _DisplayName = qqHelper.appendText(_CommercialIRSname, _CommercialDBAname, vbCrLf)
                'updated 4/22/2013 to optionally exclude doing_business_as from display_name
                If ConfigurationManager.AppSettings("QuickQuote_DisplayNames_Exclude_DoingBusinessAs") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_DisplayNames_Exclude_DoingBusinessAs").ToString) = "YES" Then
                    'exclude
                Else
                    _DisplayName = qqHelper.appendText(_DisplayName, _DoingBusinessAsName, vbCrLf)
                End If
            Else
                _DisplayName = ""
            End If
        End Sub
        Private Sub SetDisplayNameForWeb()
            Dim isCommercial As Boolean = False
            If qqHelper.IntegerForString(_TypeId) = 2 AndAlso (String.IsNullOrWhiteSpace(_CommercialDBAname) = False OrElse String.IsNullOrWhiteSpace(_CommercialIRSname) = False) Then
                isCommercial = True
            End If

            If isCommercial = False AndAlso _FirstName <> "" AndAlso _LastName <> "" Then
                _DisplayNameForWeb = qqHelper.appendText(_PrefixName, _FirstName, " ")
                _DisplayNameForWeb = qqHelper.appendText(_DisplayNameForWeb, _MiddleName, " ")
                _DisplayNameForWeb = qqHelper.appendText(_DisplayNameForWeb, _LastName, " ")
                _DisplayNameForWeb = qqHelper.appendText(_DisplayNameForWeb, _SuffixName, " ")
            ElseIf _CommercialDBAname <> "" OrElse _CommercialIRSname <> "" OrElse _DoingBusinessAsName <> "" Then
                _DisplayNameForWeb = qqHelper.appendText(_CommercialIRSname, _CommercialDBAname, vbCrLf)
                'updated 9/5/2012 to append DBA text when needed
                Dim dba_withPrefix As String = ""
                If _DoingBusinessAsName <> "" Then
                    dba_withPrefix = qqHelper.appendText("DBA", _DoingBusinessAsName, " ")
                    '_DisplayNameForWeb = qqHelper.appendText(_DisplayNameForWeb, _DoingBusinessAsName, vbCrLf)
                    _DisplayNameForWeb = qqHelper.appendText(_DisplayNameForWeb, dba_withPrefix, vbCrLf)
                End If
            Else
                _DisplayNameForWeb = ""
            End If
        End Sub
        Private Sub SetSortName() 'added 10/15/2012 to make names show up in workflow queues
            If _FirstName <> "" AndAlso _LastName <> "" Then
                '_SortName = qqHelper.appendText(_PrefixName, _FirstName, " ")
                '_SortName = qqHelper.appendText(_SortName, _MiddleName, " ")
                '_SortName = qqHelper.appendText(_SortName, _LastName, " ")
                '_SortName = qqHelper.appendText(_SortName, _SuffixName, " ")
                _SortName = qqHelper.appendText(_LastName, _FirstName, "; ")
                _SortName = qqHelper.appendText(_SortName, _MiddleName, " ")
            ElseIf _CommercialDBAname <> "" OrElse _CommercialIRSname <> "" OrElse _DoingBusinessAsName <> "" Then
                _SortName = qqHelper.appendText(_CommercialIRSname, _CommercialDBAname, vbCrLf)
                'updated 4/22/2013 to optionally exclude doing_business_as from sort_name
                If ConfigurationManager.AppSettings("QuickQuote_SortNames_Exclude_DoingBusinessAs") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_SortNames_Exclude_DoingBusinessAs").ToString) = "YES" Then
                    'exclude
                Else
                    _SortName = qqHelper.appendText(_SortName, _DoingBusinessAsName, vbCrLf)
                End If
            Else
                _SortName = ""
            End If
        End Sub
        Public Overrides Function ToString() As String 'added 6/29/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                str = qqHelper.appendText(str, "DisplayName: " & Me.DisplayName, vbCrLf)
                '6/29/2015 note: can use either way... maybe leave as-is so dev can tell if something is blank... since nothing would be after DisplayName: 
                'Dim dn As String = Me.DisplayName
                'If dn <> "" Then
                '    str = qqHelper.appendText(str, "DisplayName: " & dn, vbCrLf)
                'End If
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
                    If _BirthDate IsNot Nothing Then
                        _BirthDate = Nothing
                    End If
                    If _CommercialDBAname IsNot Nothing Then
                        _CommercialDBAname = Nothing
                    End If
                    If _CommercialIRSname IsNot Nothing Then
                        _CommercialIRSname = Nothing
                    End If
                    If _DriversLicenseDate IsNot Nothing Then
                        _DriversLicenseDate = Nothing
                    End If
                    If _DriversLicenseNumber IsNot Nothing Then
                        _DriversLicenseNumber = Nothing
                    End If
                    If _DriversLicenseStateId IsNot Nothing Then
                        _DriversLicenseStateId = Nothing
                    End If
                    If _DisplayName IsNot Nothing Then
                        _DisplayName = Nothing
                    End If
                    If _DisplayNameForWeb IsNot Nothing Then
                        _DisplayNameForWeb = Nothing
                    End If
                    If _FirstName IsNot Nothing Then
                        _FirstName = Nothing
                    End If
                    If _LastName IsNot Nothing Then
                        _LastName = Nothing
                    End If
                    If _MaritalStatusId IsNot Nothing Then
                        _MaritalStatusId = Nothing
                    End If
                    If _MiddleName IsNot Nothing Then
                        _MiddleName = Nothing
                    End If
                    If _NameAddressSourceId IsNot Nothing Then
                        _NameAddressSourceId = Nothing
                    End If
                    If _NameId IsNot Nothing Then
                        _NameId = Nothing
                    End If
                    If _NameNum IsNot Nothing Then
                        _NameNum = Nothing
                    End If
                    If _PositionTitle IsNot Nothing Then
                        _PositionTitle = Nothing
                    End If
                    If _PrefixName IsNot Nothing Then
                        _PrefixName = Nothing
                    End If
                    If _SexId IsNot Nothing Then
                        _SexId = Nothing
                    End If
                    If _SortName IsNot Nothing Then
                        _SortName = Nothing
                    End If
                    If _SuffixName IsNot Nothing Then
                        _SuffixName = Nothing
                    End If

                    If _TaxNumber IsNot Nothing Then
                        _TaxNumber = Nothing
                    End If
                    If _TaxTypeId IsNot Nothing Then
                        _TaxTypeId = Nothing
                    End If
                    If _ThirdPartyEntityId IsNot Nothing Then
                        _ThirdPartyEntityId = Nothing
                    End If
                    If _ThirdPartyGroupId IsNot Nothing Then
                        _ThirdPartyGroupId = Nothing
                    End If
                    If _TypeId IsNot Nothing Then
                        _TypeId = Nothing
                    End If

                    'removed HasData private variables 5/7/2014
                    'If _HasData <> Nothing Then
                    '    _HasData = Nothing
                    'End If
                    'If _HasDataWasManuallySet <> Nothing Then
                    '    _HasDataWasManuallySet = Nothing
                    'End If

                    If _DoingBusinessAsName IsNot Nothing Then
                        _DoingBusinessAsName = Nothing
                    End If

                    If _DisplayNameWasManuallySet <> Nothing Then
                        _DisplayNameWasManuallySet = Nothing
                    End If

                    If _EntityTypeId IsNot Nothing Then
                        _EntityTypeId = Nothing
                    End If
                    If _EntityType IsNot Nothing Then
                        _EntityType = Nothing
                    End If

                    If _DescriptionOfOperations IsNot Nothing Then
                        _DescriptionOfOperations = Nothing
                    End If

                    'Added 2/15/2022 for bug 63511 MLW
                    If _OtherLegalEntityDescription IsNot Nothing Then
                        _OtherLegalEntityDescription = Nothing
                    End If
                    If _OtherLegalEntityDescription IsNot Nothing Then
                        _OtherLegalEntityDescription = Nothing
                    End If

                    If _NAICS IsNot Nothing Then
                        _NAICS = Nothing
                    End If

                    If _SortNameWasManuallySet <> Nothing Then
                        _SortNameWasManuallySet = Nothing
                    End If

                    'If _TestNameField IsNot Nothing Then
                    '    _TestNameField = Nothing
                    'End If

                    If _TaxNumber_NoHyphens IsNot Nothing Then
                        _TaxNumber_NoHyphens = Nothing
                    End If
                    If _TaxNumber_Hyphens IsNot Nothing Then
                        _TaxNumber_Hyphens = Nothing
                    End If
                    If _TaxNumber_Entered IsNot Nothing Then
                        _TaxNumber_Entered = Nothing
                    End If
                    If _OnlyUsePropertyToSetFieldWithSameName <> Nothing Then
                        _OnlyUsePropertyToSetFieldWithSameName = Nothing
                    End If
                    If _TaxTypeId_Entered IsNot Nothing Then
                        _TaxTypeId_Entered = Nothing
                    End If

                    qqHelper.DisposeString(_Salutation) 'added 7/21/2015 for Farm; used to store ResidentName relationship (free-flow text field)

                    'added 7/23/2015 for Commercial
                    qqHelper.DisposeString(_DateBusinessStarted) 'date
                    qqHelper.DisposeString(_YearsOfExperience) 'int

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
