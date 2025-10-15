Imports IFM.PrimativeExtensions

Public Class NewFlagItem
    ''' <summary>
    ''' Default Unset or Unconvertable Date Format
    ''' </summary>
    ''' <returns>"01/01/1800" as DateTime</returns>
    Public ReadOnly Property UnsetDate() As DateTime
        Get
            Return "01/01/1800".ToDateTime
        End Get
    End Property

    Private _EnabledFlag As Boolean
    ''' <summary>
    ''' Returns the Enabled Status
    ''' </summary>
    ''' <returns>Boolean</returns>
    Public ReadOnly Property EnabledFlag() As Boolean
        Get
            Return _EnabledFlag
        End Get
    End Property

    Private _StartDate As DateTime
    ''' <summary>
    ''' The Date a flagged item becomes Available (start date, effective date, etc)
    ''' </summary>
    ''' <returns>DateTime</returns>
    Public ReadOnly Property StartDate() As DateTime
        Get
            Return _StartDate
        End Get
    End Property

    Private _VersionNumber As Integer
    ''' <summary>
    ''' The VersionID or RatedVersionID that the flagged item becomes available.
    ''' Mostly used for Endorsements.
    ''' </summary>
    ''' <returns>version number as an Integer</returns>
    Public ReadOnly Property VersionNumber() As Integer
        Get
            Return _VersionNumber
        End Get
    End Property

    Private _RenewalDate As DateTime
    ''' <summary>
    ''' The Date a flagged item Renews
    ''' </summary>
    ''' <returns>Boolean</returns>
    Public ReadOnly Property RenewalDate() As DateTime
        Get
            Return _RenewalDate
        End Get
    End Property

    Private _OtherQualifiers As Boolean
    ''' <summary>
    ''' A list of other qualifiers for testing if a flag is enabled.  
    ''' Example: (IsCorrectLob AndAlso IsCorrectFormType) both of which are boolean variables.
    ''' These are often added after creation and dependent on the item being tested for enabled status.
    ''' </summary>
    ''' <returns>Boolean</returns>
    Public Property OtherQualifiers() As Boolean
        Get
            Return _OtherQualifiers
        End Get
        Set(ByVal value As Boolean)
            _OtherQualifiers = value
        End Set
    End Property

    ''' <summary>
    ''' Does this object have a proper Renewal Date
    ''' </summary>
    ''' <returns>Boolean</returns>
    Public ReadOnly Property HasRenewalDate() As Boolean
        Get
            Return _RenewalDate <> UnsetDate
        End Get
    End Property

    ''' <summary>
    ''' Does this object have a proper Version Number
    ''' </summary>
    ''' <returns>Boolean</returns>
    Public ReadOnly Property HasVersionNumber() As Boolean
        Get
            Return _VersionNumber.HasValue
        End Get
    End Property

    Public Sub New()

    End Sub

    ''' <summary>
    ''' Contructor
    ''' </summary>
    ''' <param name="value">Value of the web.config key as string</param>
    Public Sub New(Key As String)
        ' Example of a web.config key:  <add key = "VR_Far_Canine_Settings" value="True,9/1/2022,219"/>
        Dim chc = New CommonHelperClass
        Dim SettingArray As String() = {}
        Dim value As String = chc.ConfigurationAppSettingValueAsString(Key)
        If value.IsNullEmptyorWhitespace = False Then
            SettingArray = value.Split(",")

            _EnabledFlag = chc.BooleanForString(SettingArray(0))

            Dim OutDate As DateTime
            Dim Converted = DateTime.TryParse(SettingArray(1), OutDate)
            If Converted Then
                _StartDate = OutDate
            Else
                _StartDate = UnsetDate
            End If

            _VersionNumber = SettingArray(2).Int32Value

            _RenewalDate = UnsetDate
            If HasVersionNumber = False Then
                Dim OutDateRenew As DateTime
                Dim ConvertedRenew = DateTime.TryParse(SettingArray(2), OutDateRenew)
                If ConvertedRenew Then
                    _RenewalDate = OutDateRenew
                End If
            End If

            'Default this to True, so we only stop them if there is a false value
            _OtherQualifiers = True
        Else
            'No Key or No Value for the given key.
            _EnabledFlag = False
            _StartDate = UnsetDate
            _VersionNumber = 0
            _RenewalDate = UnsetDate
            _OtherQualifiers = True
        End If
    End Sub

    ''' <summary>
    ''' Gets StartDate if it is valid, if not it attempts to convert the Param into a date
    ''' and returns that value instead.
    ''' </summary>
    ''' <param name="DateString">String containing a date "mm/dd/yyyy"</param>
    ''' <returns>StartDate if it is valid or DateString as a date</returns>
    Public Function GetStartDateOrDefault(DateString As String) As DateTime
        If StartDate <> UnsetDate Then
            Return StartDate
        Else
            Dim OutDate As DateTime
            Dim Converted = DateTime.TryParse(DateString, OutDate)
            If Converted Then
                Return OutDate
            Else
                Return UnsetDate
            End If
        End If

    End Function

    'Example of Use:
    'If quote IsNot Nothing Then
    '   Dim qqh As New QuickQuoteHelperClass
    '   Dim SubQuoteFirst = qqh.MultiStateQuickQuoteObjects(quote).GetItemAtIndex(0)

    '   Dim IsCorrectLOB As Boolean = quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm
    '   Dim IsCorrectProgramType As Boolean = SubQuoteFirst?.ProgramTypeId = "6" 'FO only

    '   Dim WoodburningSettings As NewFlagItem = New NewFlagItem("VR_Far_WoodburningUnits_Settings")
    '   WoodburningSettings.OtherQualifiers = IsCorrectProgramType AndAlso IsCorrectLOB
    '   Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, WoodburningSettings)
    'End If
    'Return False
End Class
