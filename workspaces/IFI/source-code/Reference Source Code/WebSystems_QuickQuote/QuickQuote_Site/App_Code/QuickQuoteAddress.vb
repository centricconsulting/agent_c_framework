Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store address information
    ''' </summary>
    ''' <remarks>used w/ several different objects... policyholders, clients, locations, etc.</remarks>
    <Serializable()> _
    Public Class QuickQuoteAddress
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _PolicyId As String
        Private _PolicyImageNum As String
        Private _AddressId As String
        Private _AddressNum As String
        Private _ApartmentNumber As String
        Private _HouseNum As String
        Private _StreetName As String
        Private _City As String
        Private _State As String
        Private _Zip As String
        Private _County As String
        Private _DisplayAddress As String
        Private _DisplayAddressForWeb As String 'added 5/1/2013 so it's always recalculated and line feeds are present; regular displayAddress already has PO Box text
        Private _Other As String
        Private _POBox As String
        Private _StateId As String
        Private _OriginalStateId As String 'added 8/6/2018

        Private _AttemptedVerify As Boolean
        Private _Township As String
        Private _Verified As Boolean
        Private _territorycode As String

        Private _DisplayAddressWasManuallySet As Boolean

        Private _StateWasManuallySet As Boolean 'added 12/29/2018

        Private _DetailStatusCode As String 'added 5/15/2019

        'added 8/9/2012 to help prevent SQL address error (specific to PriorCarrier section)
        'removed HasData private variables 5/7/2014
        'Private _HasData As Boolean
        'Private _HasDataWasManuallySet As Boolean

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
        Public Property AddressId As String
            Get
                Return _AddressId
            End Get
            Set(value As String)
                _AddressId = value
                'If _AddressId <> "" Then
                '    CheckHasData()
                'End If
            End Set
        End Property
        Public Property AddressNum As String
            Get
                Return _AddressNum
            End Get
            Set(value As String)
                _AddressNum = value
                'If _AddressNum <> "" Then
                '    CheckHasData()
                'End If
            End Set
        End Property
        Public Property ApartmentNumber As String
            Get
                Return _ApartmentNumber
            End Get
            Set(value As String)
                _ApartmentNumber = value
                _DisplayAddressWasManuallySet = False 'added 12/24/2012 to make sure display address is updated whenever something changes
                'If _ApartmentNumber <> "" Then
                '    CheckHasData()
                'End If
            End Set
        End Property
        Public Property HouseNum As String
            Get
                Return _HouseNum
            End Get
            Set(value As String)
                _HouseNum = value
                _DisplayAddressWasManuallySet = False 'added 12/24/2012 to make sure display address is updated whenever something changes
                'If _HouseNum <> "" Then
                '    CheckHasData()
                'End If
            End Set
        End Property
        Public Property StreetName As String
            Get
                Return _StreetName
            End Get
            Set(value As String)
                _StreetName = value
                _DisplayAddressWasManuallySet = False 'added 12/24/2012 to make sure display address is updated whenever something changes
                'If _StreetName <> "" Then
                '    CheckHasData()
                'End If
            End Set
        End Property
        Public Property City As String
            Get
                Return _City
            End Get
            Set(value As String)
                _City = value
                _DisplayAddressWasManuallySet = False 'added 12/24/2012 to make sure display address is updated whenever something changes
                'If _City <> "" Then
                '    CheckHasData()
                'End If
            End Set
        End Property
        Public Property State As String
            Get
                Return _State
            End Get
            Set(value As String)
                _State = value
                _StateWasManuallySet = True 'added 12/29/2018
                _DisplayAddressWasManuallySet = False 'added 12/24/2012 to make sure display address is updated whenever something changes
                'If _State <> "" Then
                '    CheckHasData()
                'End If
                'added backwards/forwards lookup 10/25/2012
                'Select Case _State
                '    Case "NA" 'Not Assigned
                '        _StateId = "0"
                '    Case "AK" 'Alaska
                '        _StateId = "1"
                '    Case "AL" 'Alabama
                '        _StateId = "2"
                '    Case "AR" 'Arkansas
                '        _StateId = "3"
                '    Case "AZ" 'Arizona
                '        _StateId = "4"
                '    Case "CA" 'California
                '        _StateId = "5"
                '    Case "CO" 'Colorado
                '        _StateId = "6"
                '    Case "CT" 'Connecticut
                '        _StateId = "7"
                '    Case "DC" 'District of Columbia
                '        _StateId = "8"
                '    Case "DE" 'Delaware
                '        _StateId = "9"
                '    Case "FL" 'Florida
                '        _StateId = "10"
                '    Case "GA" 'Georgia
                '        _StateId = "11"
                '    Case "HI" 'Hawaii
                '        _StateId = "12"
                '    Case "IA" 'Iowa
                '        _StateId = "13"
                '    Case "ID" 'Idaho
                '        _StateId = "14"
                '    Case "IL" 'Illinois
                '        _StateId = "15"
                '    Case "IN" 'Indiana
                '        _StateId = "16"
                '    Case "KS" 'Kansas
                '        _StateId = "17"
                '    Case "KY" 'Kentucky
                '        _StateId = "18"
                '    Case "LA" 'Louisiana
                '        _StateId = "19"
                '    Case "MA" 'Massachusetts
                '        _StateId = "20"
                '    Case "MD" 'Maryland
                '        _StateId = "21"
                '    Case "ME" 'Maine
                '        _StateId = "22"
                '    Case "MI" 'Michigan
                '        _StateId = "23"
                '    Case "MN" 'Minnesota
                '        _StateId = "24"
                '    Case "MO" 'Missouri
                '        _StateId = "25"
                '    Case "MS" 'Mississippi
                '        _StateId = "26"
                '    Case "MT" 'Montana
                '        _StateId = "27"
                '    Case "NC" 'North Carolina
                '        _StateId = "28"
                '    Case "ND" 'North Dakota
                '        _StateId = "29"
                '    Case "NE" 'Nebraska
                '        _StateId = "30"
                '    Case "NH" 'New Hampshire
                '        _StateId = "31"
                '    Case "NJ" 'New Jersey
                '        _StateId = "32"
                '    Case "NM" 'New Mexico
                '        _StateId = "33"
                '    Case "NV" 'Nevada
                '        _StateId = "34"
                '    Case "NY" 'New York
                '        _StateId = "35"
                '    Case "OH" 'Ohio
                '        _StateId = "36"
                '    Case "OK" 'Oklahoma
                '        _StateId = "37"
                '    Case "OR" 'Oregon
                '        _StateId = "38"
                '    Case "PA" 'Pennsylvania
                '        _StateId = "39"
                '    Case "RI" 'Rhode Island
                '        _StateId = "40"
                '    Case "SC" 'South Carolina
                '        _StateId = "41"
                '    Case "SD" 'South Dakota
                '        _StateId = "42"
                '    Case "TN" 'Tennessee
                '        _StateId = "43"
                '    Case "TX" 'Texas
                '        _StateId = "44"
                '    Case "UT" 'Utah
                '        _StateId = "45"
                '    Case "VA" 'Virginia
                '        _StateId = "46"
                '    Case "VT" 'Vermont
                '        _StateId = "47"
                '    Case "WA" 'Washington
                '        _StateId = "48"
                '    Case "WI" 'Wisconsin
                '        _StateId = "49"
                '    Case "WV" 'West Virginia
                '        _StateId = "50"
                '    Case "WY" 'Wyoming
                '        _StateId = "51"
                '    Case "AB" 'Alberta
                '        _StateId = "52"
                '    Case "BC" 'British Columbia
                '        _StateId = "53"
                '    Case "NB" 'New Brunswick
                '        _StateId = "54"
                '    Case "NL" 'Newfoundland
                '        _StateId = "55"
                '    Case "NT" 'Northwest Territories/Nunavut
                '        _StateId = "56"
                '    Case "NS" 'Nova Scotia
                '        _StateId = "57"
                '    Case "ON" 'Ontario
                '        _StateId = "58"
                '    Case "PE" 'Prince Edward Island
                '        _StateId = "59"
                '    Case "QC" 'Quebec
                '        _StateId = "60"
                '    Case "SK" 'Saskatchewan
                '        _StateId = "61"
                '    Case "YT" 'Yukon Territory
                '        _StateId = "62"
                '    Case "MB" 'Manitoba
                '        _StateId = "67"
                '    Case "NN" 'Nunavut
                '        _StateId = "68"
                '    Case "ACT" 'Australian Capital Territory
                '        _StateId = "69"
                '    Case "NSW" 'New South Wales
                '        _StateId = "70"
                '    Case "NT" 'Northern Territory
                '        _StateId = "71"
                '    Case "QLD" 'Queensland
                '        _StateId = "72"
                '    Case "SA" 'South Australia
                '        _StateId = "73"
                '    Case "TAS" 'Tasmania
                '        _StateId = "74"
                '    Case "VIC" 'Victoria
                '        _StateId = "75"
                '    Case "WA" 'Western Australia
                '        _StateId = "76"
                '    Case "GB" 'England
                '        _StateId = "77"
                '    Case "SL" 'Scotland
                '        _StateId = "78"
                '    Case "WL" 'Wales
                '        _StateId = "79"
                '    Case "NI" 'Northern Ireland
                '        _StateId = "80"
                '    Case "PR" 'Puerto Rico
                '        _StateId = "285"
                '    Case "AS" 'American Samoa
                '        _StateId = "291"
                '    Case "FM" 'Federal States of Micronesia
                '        _StateId = "292"
                '    Case "GU" 'Guam
                '        _StateId = "293"
                '    Case "MH" 'Marshall Islands
                '        _StateId = "294"
                '    Case "MP" 'Northern Mariana Island
                '        _StateId = "295"
                '    Case "PW" 'Palau
                '        _StateId = "296"
                '    Case "VI" 'US Virgin Islands
                '        _StateId = "297"
                '    Case "AE" 'Military Europe
                '        _StateId = "298"
                '    Case "AP" 'Military Pacific
                '        _StateId = "299"
                '    Case "AA" 'Armed Forces Americas
                '        _StateId = "300"
                '    Case "Non-US" 'Non-US
                '        _StateId = "306"
                '    Case Else
                '        _StateId = ""
                'End Select
                'updated 12/11/2013
                '_StateId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteAddress, QuickQuoteHelperClass.QuickQuotePropertyName.StateId, _State)
                ''added 10/22/2016 for backup logic
                'If qqHelper.IsPositiveIntegerString(_StateId) = False AndAlso String.IsNullOrWhiteSpace(_State) = False Then
                '    If UCase(_State) = "IN" Then
                '        _StateId = "16"
                '    End If
                'End If
                'updated 2/3/2019
                Dim intStateId As Integer = QuickQuoteHelperClass.DiamondStateIdForStateAbbreviation_OptionalClassNameForStaticData(_State, qqClassName:=QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteAddress, defaultToIndiana:=False)
                If intStateId > 0 Then
                    _StateId = intStateId.ToString
                Else
                    _StateId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteAddress, QuickQuoteHelperClass.QuickQuotePropertyName.StateId, _State)
                    If qqHelper.IsPositiveIntegerString(_StateId) = False AndAlso String.IsNullOrWhiteSpace(_State) = False Then
                        If UCase(_State) = "IN" Then
                            _StateId = "16"
                        End If
                    End If
                End If

                If String.IsNullOrWhiteSpace(_OriginalStateId) = True AndAlso String.IsNullOrWhiteSpace(_StateId) = False Then 'added 8/6/2018
                    _OriginalStateId = _StateId
                End If
            End Set
        End Property
        Public Property Zip As String
            Get
                Return _Zip
            End Get
            Set(value As String)
                _Zip = value
                _DisplayAddressWasManuallySet = False 'added 12/24/2012 to make sure display address is updated whenever something changes
                'If _Zip <> "" AndAlso Len(_Zip) = 5 Then
                '    _Zip &= "-0000"
                'End If
                'updated 7/24/2013 for additional formatting
                If _Zip <> "" Then
                    If Len(_Zip) = 5 Then
                        _Zip &= "-0000"
                    ElseIf Len(_Zip) = 9 Then 'may also check to make sure it doesn't already have a hyphen
                        _Zip = Left(_Zip, 5) & "-" & Right(_Zip, 4)
                    End If
                End If
                'If _Zip <> "" Then
                '    CheckHasData()
                'End If
            End Set
        End Property
        Public Property County As String
            Get
                Return _County
            End Get
            Set(value As String)
                _County = value
                'If _County <> "" Then
                '    CheckHasData()
                'End If
            End Set
        End Property

        Public Property IsChanged As Boolean
        Public Property IsNew As Boolean

        ''' <summary>
        ''' used for display address in Diamond
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>automatically calculated when something changes; doesn't need to be set by developer code</remarks>
        Public Property DisplayAddress As String
            Get
                If _DisplayAddressWasManuallySet = False Then
                    SetDisplayAddress()
                End If
                Return _DisplayAddress
            End Get
            Set(value As String)
                _DisplayAddress = value
                '_DisplayAddressWasManuallySet = True
                If _DisplayAddress <> "" Then
                    _DisplayAddressWasManuallySet = True '4/22/2013 - moved inside conditional (to match update made for Name 10/13/2012)
                    'CheckHasData()
                End If
            End Set
        End Property
        ''' <summary>
        ''' similar to display address in Diamond but always contains line feeds
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>automatically calculated whenever used; doesn't need to be set by developer code</remarks>
        Public Property DisplayAddressForWeb As String
            Get
                SetDisplayAddressForWeb()
                Return _DisplayAddressForWeb
            End Get
            Set(value As String)
                _DisplayAddressForWeb = value
            End Set
        End Property
        Public Property Other As String
            Get
                Return _Other
            End Get
            Set(value As String)
                _Other = value
                _DisplayAddressWasManuallySet = False 'added 12/24/2012 to make sure display address is updated whenever something changes
                'If _Other <> "" Then
                '    CheckHasData()
                'End If
            End Set
        End Property
        Public Property POBox As String
            Get
                Return _POBox
            End Get
            Set(value As String)
                _POBox = value
                _DisplayAddressWasManuallySet = False 'added 12/24/2012 to make sure display address is updated whenever something changes
                'If _POBox <> "" Then
                '    CheckHasData()
                'End If
            End Set
        End Property
        Public Property StateId As String
            Get
                Return _StateId
            End Get
            Set(value As String)
                _StateId = value
                _StateWasManuallySet = True 'added 12/29/2018
                If String.IsNullOrWhiteSpace(_OriginalStateId) = True Then 'added 8/6/2018
                    _OriginalStateId = value
                End If
                _DisplayAddressWasManuallySet = False 'added 12/24/2012 to make sure display address is updated whenever something changes
                'If _StateId <> "" Then
                '    CheckHasData()
                'End If
                'added backwards/forwards lookup 10/25/2012
                '_State = ""
                'If IsNumeric(_StateId) = True Then
                '    Select Case _StateId
                '        Case "0"
                '            _State = "NA" 'Not Assigned
                '        Case "1"
                '            _State = "AK" 'Alaska
                '        Case "2"
                '            _State = "AL" 'Alabama
                '        Case "3"
                '            _State = "AR" 'Arkansas
                '        Case "4"
                '            _State = "AZ" 'Arizona
                '        Case "5"
                '            _State = "CA" 'California
                '        Case "6"
                '            _State = "CO" 'Colorado
                '        Case "7"
                '            _State = "CT" 'Connecticut
                '        Case "8"
                '            _State = "DC" 'District of Columbia
                '        Case "9"
                '            _State = "DE" 'Delaware
                '        Case "10"
                '            _State = "FL" 'Florida
                '        Case "11"
                '            _State = "GA" 'Georgia
                '        Case "12"
                '            _State = "HI" 'Hawaii
                '        Case "13"
                '            _State = "IA" 'Iowa
                '        Case "14"
                '            _State = "ID" 'Idaho
                '        Case "15"
                '            _State = "IL" 'Illinois
                '        Case "16"
                '            _State = "IN" 'Indiana
                '        Case "17"
                '            _State = "KS" 'Kansas
                '        Case "18"
                '            _State = "KY" 'Kentucky
                '        Case "19"
                '            _State = "LA" 'Louisiana
                '        Case "20"
                '            _State = "MA" 'Massachusetts
                '        Case "21"
                '            _State = "MD" 'Maryland
                '        Case "22"
                '            _State = "ME" 'Maine
                '        Case "23"
                '            _State = "MI" 'Michigan
                '        Case "24"
                '            _State = "MN" 'Minnesota
                '        Case "25"
                '            _State = "MO" 'Missouri
                '        Case "26"
                '            _State = "MS" 'Mississippi
                '        Case "27"
                '            _State = "MT" 'Montana
                '        Case "28"
                '            _State = "NC" 'North Carolina
                '        Case "29"
                '            _State = "ND" 'North Dakota
                '        Case "30"
                '            _State = "NE" 'Nebraska
                '        Case "31"
                '            _State = "NH" 'New Hampshire
                '        Case "32"
                '            _State = "NJ" 'New Jersey
                '        Case "33"
                '            _State = "NM" 'New Mexico
                '        Case "34"
                '            _State = "NV" 'Nevada
                '        Case "35"
                '            _State = "NY" 'New York
                '        Case "36"
                '            _State = "OH" 'Ohio
                '        Case "37"
                '            _State = "OK" 'Oklahoma
                '        Case "38"
                '            _State = "OR" 'Oregon
                '        Case "39"
                '            _State = "PA" 'Pennsylvania
                '        Case "40"
                '            _State = "RI" 'Rhode Island
                '        Case "41"
                '            _State = "SC" 'South Carolina
                '        Case "42"
                '            _State = "SD" 'South Dakota
                '        Case "43"
                '            _State = "TN" 'Tennessee
                '        Case "44"
                '            _State = "TX" 'Texas
                '        Case "45"
                '            _State = "UT" 'Utah
                '        Case "46"
                '            _State = "VA" 'Virginia
                '        Case "47"
                '            _State = "VT" 'Vermont
                '        Case "48"
                '            _State = "WA" 'Washington
                '        Case "49"
                '            _State = "WI" 'Wisconsin
                '        Case "50"
                '            _State = "WV" 'West Virginia
                '        Case "51"
                '            _State = "WY" 'Wyoming
                '        Case "52"
                '            _State = "AB" 'Alberta
                '        Case "53"
                '            _State = "BC" 'British Columbia
                '        Case "54"
                '            _State = "NB" 'New Brunswick
                '        Case "55"
                '            _State = "NL" 'Newfoundland
                '        Case "56"
                '            _State = "NT" 'Northwest Territories/Nunavut
                '        Case "57"
                '            _State = "NS" 'Nova Scotia
                '        Case "58"
                '            _State = "ON" 'Ontario
                '        Case "59"
                '            _State = "PE" 'Prince Edward Island
                '        Case "60"
                '            _State = "QC" 'Quebec
                '        Case "61"
                '            _State = "SK" 'Saskatchewan
                '        Case "62"
                '            _State = "YT" 'Yukon Territory
                '        Case "67"
                '            _State = "MB" 'Manitoba
                '        Case "68"
                '            _State = "NN" 'Nunavut
                '        Case "69"
                '            _State = "ACT" 'Australian Capital Territory
                '        Case "70"
                '            _State = "NSW" 'New South Wales
                '        Case "71"
                '            _State = "NT" 'Northern Territory
                '        Case "72"
                '            _State = "QLD" 'Queensland
                '        Case "73"
                '            _State = "SA" 'South Australia
                '        Case "74"
                '            _State = "TAS" 'Tasmania
                '        Case "75"
                '            _State = "VIC" 'Victoria
                '        Case "76"
                '            _State = "WA" 'Western Australia
                '        Case "77"
                '            _State = "GB" 'England
                '        Case "78"
                '            _State = "SL" 'Scotland
                '        Case "79"
                '            _State = "WL" 'Wales
                '        Case "80"
                '            _State = "NI" 'Northern Ireland
                '        Case "285"
                '            _State = "PR" 'Puerto Rico
                '        Case "291"
                '            _State = "AS" 'American Samoa
                '        Case "292"
                '            _State = "FM" 'Federal States of Micronesia
                '        Case "293"
                '            _State = "GU" 'Guam
                '        Case "294"
                '            _State = "MH" 'Marshall Islands
                '        Case "295"
                '            _State = "MP" 'Northern Mariana Island
                '        Case "296"
                '            _State = "PW" 'Palau
                '        Case "297"
                '            _State = "VI" 'US Virgin Islands
                '        Case "298"
                '            _State = "AE" 'Military Europe
                '        Case "299"
                '            _State = "AP" 'Military Pacific
                '        Case "300"
                '            _State = "AA" 'Armed Forces Americas
                '        Case "306"
                '            _State = "Non-US" 'Non-US
                '    End Select
                'End If
                'updated 12/11/2013
                If IsNumeric(_StateId) = True Then 'added IF 2/3/2019; original logic in ELSE
                    _State = QuickQuoteHelperClass.StateAbbreviationForDiamondStateId_OptionalClassNameForStaticData(CInt(_StateId), qqClassName:=QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteAddress, defaultToIndiana:=False)
                Else
                    _State = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteAddress, QuickQuoteHelperClass.QuickQuotePropertyName.StateId, _StateId)
                    'added 10/22/2016 for backup logic
                    If qqHelper.IsPositiveIntegerString(_StateId) = True AndAlso String.IsNullOrWhiteSpace(_State) = True Then
                        If _StateId = "16" Then
                            _State = "IN"
                        End If
                    End If
                End If
            End Set
        End Property
        Public Property QuickQuoteState As QuickQuoteHelperClass.QuickQuoteState 'added 7/30/2018
            Get
                Return QuickQuoteHelperClass.QuickQuoteStateForDiamondStateId(qqHelper.IntegerForString(_StateId), defaultToIndiana:=False)
            End Get
            Set(value As QuickQuoteHelperClass.QuickQuoteState)
                Dim intStateID As Integer = QuickQuoteHelperClass.DiamondStateIdForQuickQuoteState(value, defaultToIndiana:=False)
                If intStateID > 0 Then
                    StateId = intStateID
                End If
            End Set
        End Property
        Public ReadOnly Property OriginalStateId As String 'added 8/6/2018
            Get
                Return _OriginalStateId
            End Get
        End Property
        Public ReadOnly Property OriginalQuickQuoteState As String 'added 8/7/2018
            Get
                Return QuickQuoteHelperClass.QuickQuoteStateForDiamondStateId(qqHelper.IntegerForString(_OriginalStateId), defaultToIndiana:=False)
            End Get
        End Property

        Public Property AttemptedVerify As Boolean
            Get
                Return _AttemptedVerify
            End Get
            Set(value As Boolean)
                _AttemptedVerify = value
                'If _AttemptedVerify <> Nothing Then
                '    CheckHasData()
                'End If
            End Set
        End Property
        Public Property Township As String
            Get
                Return _Township
            End Get
            Set(value As String)
                _Township = value
                'If _Township <> "" Then
                '    CheckHasData()
                'End If
            End Set
        End Property
        Public Property Verified As Boolean
            Get
                Return _Verified
            End Get
            Set(value As Boolean)
                _Verified = value
                'If _Verified <> Nothing Then
                '    CheckHasData()
                'End If
            End Set
        End Property
        Public Property territorycode As String
            Get
                Return _territorycode
            End Get
            Set(value As String)
                _territorycode = value
                'If _territorycode <> "" Then
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
                'If _PolicyId <> "" OrElse _PolicyImageNum <> "" OrElse _AddressId <> "" OrElse _AddressNum <> "" OrElse _ApartmentNumber <> "" OrElse _HouseNum <> "" OrElse _StreetName <> "" OrElse _City <> "" OrElse _State <> "" OrElse _Zip <> "" OrElse _County <> "" OrElse _DisplayAddress <> "" OrElse _Other <> "" OrElse _POBox <> "" OrElse _StateId <> "" OrElse _Township <> "" OrElse _territorycode <> "" Then
                'now just checking props that are used... no _PolicyId, _PolicyImageNum, _AddressId, _AddressNum, _State, _DisplayAddress
                'might also eventually check IsNumeric for integers and IsDate for dates... may also need to check for integers > 0 since we're now parsing rated xml, which could have default values of 0 when nothing was ever set; might also remove _StateId since it's being defaulted... updated to only use if it's different than IN (16)
                'If _ApartmentNumber <> "" OrElse _HouseNum <> "" OrElse _StreetName <> "" OrElse _City <> "" OrElse _Zip <> "" OrElse _County <> "" OrElse _Other <> "" OrElse _POBox <> "" OrElse (_StateId <> "" AndAlso _State <> "" AndAlso UCase(_State) <> "IN") OrElse _Township <> "" OrElse _territorycode <> "" Then
                'updated 3/8/2017 to handle for blank zip; also territoryCode on 3/9/2017
                'If _ApartmentNumber <> "" OrElse _HouseNum <> "" OrElse _StreetName <> "" OrElse _City <> "" OrElse Replace(Replace(_Zip, "0", ""), "-", "") <> "" OrElse _County <> "" OrElse _Other <> "" OrElse _POBox <> "" OrElse (_StateId <> "" AndAlso _State <> "" AndAlso UCase(_State) <> "IN") OrElse _Township <> "" OrElse Replace(_territorycode, "0", "") <> "" Then
                'updated 12/29/2018 to use new helper Properties
                If HasValidNonIndianaState = True OrElse HasAnyDataOtherThanState = True Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property

        'added 10/9/2016
        Public ReadOnly Property HasFullStreetAddress As Boolean
            Get
                If String.IsNullOrWhiteSpace(_HouseNum) = False AndAlso String.IsNullOrWhiteSpace(_StreetName) = False AndAlso String.IsNullOrWhiteSpace(_City) = False AndAlso String.IsNullOrWhiteSpace(_State) = False AndAlso String.IsNullOrWhiteSpace(_Zip) = False Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property
        Public ReadOnly Property HasAnyStreetAddressInfo As Boolean
            Get
                'If String.IsNullOrWhiteSpace(_HouseNum) = False OrElse String.IsNullOrWhiteSpace(_StreetName) = False OrElse String.IsNullOrWhiteSpace(_City) = False OrElse String.IsNullOrWhiteSpace(_State) = False OrElse String.IsNullOrWhiteSpace(_Zip) = False Then
                'updated 10/13/2016 to also check for _ApartmentNumber
                If String.IsNullOrWhiteSpace(_HouseNum) = False OrElse String.IsNullOrWhiteSpace(_StreetName) = False OrElse String.IsNullOrWhiteSpace(_ApartmentNumber) = False OrElse String.IsNullOrWhiteSpace(_City) = False OrElse String.IsNullOrWhiteSpace(_State) = False OrElse String.IsNullOrWhiteSpace(_Zip) = False Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property

        'added 10/13/2016
        Public ReadOnly Property OneLineStreetAddress(Optional ByVal useApartmentNumber As Boolean = True) As String 'updated 10/22/2016 w/ optional param to use aptNum... appears that IsoTransmission for Verisk Report doesn't use it
            Get
                Dim _OneLineStreetAddress As String = ""

                If String.IsNullOrWhiteSpace(_HouseNum) = False OrElse String.IsNullOrWhiteSpace(_StreetName) = False Then
                    _OneLineStreetAddress = qqHelper.appendText(_HouseNum, _StreetName, " ")
                    If useApartmentNumber = True Then 'added IF 10/22/2016
                        _OneLineStreetAddress = qqHelper.appendText(_OneLineStreetAddress, _ApartmentNumber, " ")
                    End If
                ElseIf String.IsNullOrWhiteSpace(_POBox) = False Then
                    _OneLineStreetAddress = "PO BOX " & _POBox
                End If
                _OneLineStreetAddress = qqHelper.appendText(_OneLineStreetAddress, _City, " ")
                'added 10/22/2016 for backup logic
                If qqHelper.IsPositiveIntegerString(_StateId) = True AndAlso String.IsNullOrWhiteSpace(_State) = True Then
                    If _StateId = "16" Then
                        _State = "IN"
                    End If
                End If
                _OneLineStreetAddress = qqHelper.appendText(_OneLineStreetAddress, _State, " ")
                _OneLineStreetAddress = qqHelper.appendText(_OneLineStreetAddress, _Zip, " ")

                Return _OneLineStreetAddress
            End Get
        End Property

        Public ReadOnly Property StateWasManuallySet As Boolean 'added 12/29/2018
            Get
                Return _StateWasManuallySet
            End Get
        End Property
        Public ReadOnly Property HasValidQuickQuoteState As Boolean 'added 12/29/2018
            Get
                Dim hasValid As Boolean = False
                Dim qqState As QuickQuoteHelperClass.QuickQuoteState = QuickQuoteState
                If System.Enum.IsDefined(GetType(QuickQuoteHelperClass.QuickQuoteState), qqState) = True AndAlso qqState <> QuickQuoteHelperClass.QuickQuoteState.None Then
                    hasValid = True
                End If
                Return hasValid
            End Get
        End Property
        Public ReadOnly Property HasValidState As Boolean 'added 12/29/2018
            Get
                Dim hasValid As Boolean = False
                If qqHelper.IsPositiveIntegerString(_StateId) = True AndAlso String.IsNullOrWhiteSpace(_State) = False Then
                    hasValid = True
                End If
                Return hasValid
            End Get
        End Property
        Public ReadOnly Property HasValidNonIndianaState As Boolean 'added 12/29/2018
            Get
                Dim hasValid As Boolean = False
                If HasValidState = True AndAlso QuickQuoteState <> QuickQuoteHelperClass.QuickQuoteState.Indiana Then
                    hasValid = True
                End If
                Return hasValid
            End Get
        End Property
        Public ReadOnly Property HasAnyDataOtherThanState As Boolean 'added 12/29/2018
            Get
                If String.IsNullOrWhiteSpace(_ApartmentNumber) = False OrElse String.IsNullOrWhiteSpace(_HouseNum) = False OrElse String.IsNullOrWhiteSpace(_StreetName) = False OrElse String.IsNullOrWhiteSpace(_City) = False OrElse (String.IsNullOrWhiteSpace(_Zip) = False AndAlso String.IsNullOrWhiteSpace(Replace(Replace(_Zip, "0", ""), "-", "")) = False) OrElse String.IsNullOrWhiteSpace(_County) = False OrElse String.IsNullOrWhiteSpace(_Other) = False OrElse String.IsNullOrWhiteSpace(_POBox) = False OrElse String.IsNullOrWhiteSpace(_Township) = False OrElse (String.IsNullOrWhiteSpace(_territorycode) = False AndAlso String.IsNullOrWhiteSpace(Replace(_territorycode, "0", "")) = False) Then
                    Return True
                Else
                    Return False
                End If
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
        Private Sub SetDefaults()
            _PolicyId = ""
            _PolicyImageNum = ""
            _AddressId = ""
            _AddressNum = ""
            _ApartmentNumber = ""
            _HouseNum = ""
            _StreetName = ""
            _City = ""
            _State = "IN" '*defaulting
            _Zip = ""
            _County = ""
            _DisplayAddress = ""
            _DisplayAddressForWeb = ""
            _Other = ""
            _POBox = ""
            _StateId = "16" '*defaulting to IN
            '5/7/2014 - may change to use static data like what's already being done on QuickQuoteObject.StateId
            '_StateId = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteAddress, QuickQuoteHelperClass.QuickQuotePropertyName.StateId, _State)
            _OriginalStateId = "" 'added 8/6/2018

            _AttemptedVerify = False
            _Township = ""
            _Verified = False
            _territorycode = ""

            _DisplayAddressWasManuallySet = False

            _StateWasManuallySet = False 'added 12/29/2018

            _DetailStatusCode = "" 'added 5/15/2019

            IsChanged = False
            IsNew = False

            'removed HasData private variables 5/7/2014
            '_HasData = False
            '_HasDataWasManuallySet = False
        End Sub
        Private Sub SetDisplayAddress()
            If _HouseNum <> "" AndAlso _StreetName <> "" Then
                _DisplayAddress = _HouseNum & " " & _StreetName
                _DisplayAddress = qqHelper.appendText(_DisplayAddress, _ApartmentNumber, " ")
            ElseIf _POBox <> "" Then
                _DisplayAddress = "PO BOX " & _POBox
            Else
                _DisplayAddress = ""
            End If
            Dim cityStateZip As String = qqHelper.appendText(_City, _State, ", ")
            cityStateZip = qqHelper.appendText(cityStateZip, _Zip, " ")
            _DisplayAddress = qqHelper.appendText(_Other, _DisplayAddress, vbCrLf)
            _DisplayAddress = qqHelper.appendText(_DisplayAddress, cityStateZip, vbCrLf)
        End Sub
        Private Sub SetDisplayAddressForWeb()
            If _HouseNum <> "" AndAlso _StreetName <> "" Then
                _DisplayAddressForWeb = _HouseNum & " " & _StreetName
                _DisplayAddressForWeb = qqHelper.appendText(_DisplayAddressForWeb, _ApartmentNumber, " ")
            ElseIf _POBox <> "" Then
                _DisplayAddressForWeb = "PO BOX " & _POBox
            Else
                _DisplayAddressForWeb = ""
            End If
            Dim cityStateZip As String = qqHelper.appendText(_City, _State, ", ")
            cityStateZip = qqHelper.appendText(cityStateZip, _Zip, " ")
            _DisplayAddressForWeb = qqHelper.appendText(_Other, _DisplayAddressForWeb, vbCrLf)
            _DisplayAddressForWeb = qqHelper.appendText(_DisplayAddressForWeb, cityStateZip, vbCrLf)
        End Sub
        Public Overrides Function ToString() As String 'added 6/29/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                str = qqHelper.appendText(str, "DisplayAddress: " & Me.DisplayAddress, vbCrLf)
            Else
                str = "Nothing"
            End If
            Return str
        End Function

        'removed HasData private variables 5/7/2014
        'Private Sub CheckHasData()
        '    If _HasData = False AndAlso _HasDataWasManuallySet = False Then
        '        _HasData = True
        '    End If
        'End Sub

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
                    If _AddressId IsNot Nothing Then
                        _AddressId = Nothing
                    End If
                    If _AddressNum IsNot Nothing Then
                        _AddressNum = Nothing
                    End If
                    If _ApartmentNumber IsNot Nothing Then
                        _ApartmentNumber = Nothing
                    End If
                    If _HouseNum IsNot Nothing Then
                        _HouseNum = Nothing
                    End If
                    If _StreetName IsNot Nothing Then
                        _StreetName = Nothing
                    End If
                    If _City IsNot Nothing Then
                        _City = Nothing
                    End If
                    If _State IsNot Nothing Then
                        _State = Nothing
                    End If
                    If _Zip IsNot Nothing Then
                        _Zip = Nothing
                    End If
                    If _County IsNot Nothing Then
                        _County = Nothing
                    End If
                    If _DisplayAddress IsNot Nothing Then
                        _DisplayAddress = Nothing
                    End If
                    If _DisplayAddressForWeb IsNot Nothing Then
                        _DisplayAddressForWeb = Nothing
                    End If
                    If _Other IsNot Nothing Then
                        _Other = Nothing
                    End If
                    If _POBox IsNot Nothing Then
                        _POBox = Nothing
                    End If
                    If _StateId IsNot Nothing Then
                        _StateId = Nothing
                    End If
                    qqHelper.DisposeString(_OriginalStateId) 'added 8/6/2018

                    If _AttemptedVerify <> Nothing Then
                        _AttemptedVerify = Nothing
                    End If
                    If _Township IsNot Nothing Then
                        _Township = Nothing
                    End If
                    If _Verified <> Nothing Then
                        _Verified = Nothing
                    End If
                    If _territorycode IsNot Nothing Then
                        _territorycode = Nothing
                    End If

                    If _DisplayAddressWasManuallySet <> Nothing Then
                        _DisplayAddressWasManuallySet = Nothing
                    End If

                    _StateWasManuallySet = Nothing 'added 12/29/2018

                    qqHelper.DisposeString(_DetailStatusCode) 'added 5/15/2019

                    'removed HasData private variables 5/7/2014
                    'If _HasData <> Nothing Then
                    '    _HasData = Nothing
                    'End If
                    'If _HasDataWasManuallySet <> Nothing Then
                    '    _HasDataWasManuallySet = Nothing
                    'End If

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
