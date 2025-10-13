Imports IFM.VR.Common.UWQuestions
Imports QuickQuote.CommonObjects
Imports System.Linq
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.GenericHelper
Imports IFM.ControlFlags
Imports IFM.VR.Flags
Imports IFM.VR.Web.Factory
Imports IFM.VR.Common.Underwriting
Imports IFM.VR.Common.Helpers
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports Diamond.Common.Objects.Billing
Imports IFM.VR.Common.Helpers.CPR
Imports Diamond.Common.Enums
Imports Diamond.Common.Enums.VehicleNonOwned
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Common.Helpers.HOM
Imports IFM.VR.Common.Helpers.DFR
Imports IFM.VR.Common.Helpers.PPA
Imports IFM.VR.Common.Helpers.FARM

Public Class ctlUWQuestionsPopup
    Inherits System.Web.UI.UserControl
    'TODO - See if you can make this use the essentials base control
    ' Right now it causes an additional database pull that otherwise would be cached

    ''' <summary>
    ''' This provides access to several common js scripts as well as the ability to inject js script via code-behind.
    ''' </summary>
    ''' <returns></returns>
    Protected ReadOnly Property VRScript As ctlPageStartupScript
        Get
            Return DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager
        End Get
    End Property

    'Private _validationMessage As String = "The user does not have the authority to bind or issue coverage for this risk. Please refer to your Personal Lines Underwriter."
    ''' <summary>
    ''' Added this property to determine the validation message based on LOB
    ''' </summary>
    ''' <returns></returns>
    Private ReadOnly Property ValidationMessage As String
        Get
            If NewQuoteRequestLOBID > 0 Then
                Select Case NewQuoteRequestLOBID
                    Case 25, 20, 21, 9, 28, 23
                        Return "The user does not have the authority to quote, bind or issue coverage for this risk. Please refer to your Commercial Underwriter."
                    Case Else
                        Return "The user does not have the authority to bind or issue coverage for this risk. Please refer to your Personal Lines Underwriter."
                End Select
            Else
                Return ""
            End If
        End Get
    End Property

    'Private Const inEligibleMsg As String = "This risk is ineligible."
    'Private Const inEligibleMsg As String = "This risk is ineligible, if answered ""Yes"" in error select ""Cancel"".If answered correctly, select ""OK""."
    ''' <summary>
    ''' Added this property to determine the ineligible message based on LOB
    ''' </summary>
    ''' <returns></returns>
    Private ReadOnly Property inEligibleMsg As String
        Get
            If NewQuoteRequestLOBID > 0 Then
                Select Case NewQuoteRequestLOBID
                    Case 25, 20, 21, 9, 28, 23
                        Return "This applicant is not eligible for coverage with Indiana Farmers Mutual Ins Co."
                    Case Else
                        Return "This risk is ineligible, if answered ""Yes"" in error select ""Cancel"".If answered correctly, select ""OK""."
                End Select
            Else
                Return ""
            End If
        End Get
    End Property

    Private ReadOnly Property inEligibleMsgNo As String
        Get
            If NewQuoteRequestLOBID > 0 Then
                Select Case NewQuoteRequestLOBID
                    Case 25, 20, 21, 9, 28, 23
                        Return "This applicant is not eligible for coverage with Indiana Farmers Mutual Ins Co."
                    Case Else
                        Return "This risk is ineligible, if answered ""No"" in error select ""Cancel"".If answered correctly, select ""OK""."
                End Select
            Else
                Return ""
            End If
        End Get
    End Property

    Public ReadOnly Property ValMsg As String
        Get
            If String.IsNullOrWhiteSpace(NewQuoteRequestLOBID) = False Then
                If NewQuoteRequestLOBID = "17" Then
                    Return "The user does not have the authority to bind or issue coverage for this risk. Please refer to your Farm Underwriter."
                End If
            Else
                If (Me.Quote IsNot Nothing AndAlso Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm) Then
                    Return "The user does not have the authority to bind or issue coverage for this risk. Please refer to your Farm Underwriter."
                End If
            End If
            Return ValidationMessage
        End Get
    End Property

    Dim _quote As QuickQuoteObject
    Protected ReadOnly Property Quote As QuickQuote.CommonObjects.QuickQuoteObject
        Get
            Dim errCreateQSO As String = ""
            If _quote Is Nothing Then
                If String.IsNullOrWhiteSpace(ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/15/2019; original logic in ELSE
                    Dim readOnlyLoadError As String = ""
                    '_quote = VR.Common.QuoteSave.QuoteSaveHelpers.GetReadOnlyQuickQuoteObjectForPolicyIdAndImageNum(ReadOnlyPolicyId, ReadOnlyPolicyImageNum, errorMessage:=readOnlyLoadError)
                    'updated 12/17/2019
                    _quote = VR.Common.QuoteSave.QuoteSaveHelpers.GetReadOnlyImageFromAnywhere(ReadOnlyPolicyId, ReadOnlyPolicyImageNum, errorMessage:=readOnlyLoadError)
                ElseIf String.IsNullOrWhiteSpace(EndorsementPolicyIdAndImageNum) = False Then
                    Dim endorsementLoadError As String = ""
                    '_quote = VR.Common.QuoteSave.QuoteSaveHelpers.GetEndorsementQuoteForPolicyIdAndImageNum(EndorsementPolicyId, EndorsementPolicyImageNum, errorMessage:=endorsementLoadError)
                    'updated 12/17/2019
                    _quote = VR.Common.QuoteSave.QuoteSaveHelpers.GetEndorsementQuoteFromAnywhere(EndorsementPolicyId, EndorsementPolicyImageNum, errorMessage:=endorsementLoadError)
                Else
                    '_quote = VR.Common.QuoteSave.QuoteSaveHelpers.GetQuoteById_NOSESSION(Me.QuoteId)
                    'updated 10/4/2019 (12/11/2019 in Sprint 10)
                    _quote = VR.Common.QuoteSave.QuoteSaveHelpers.GetQuoteById(QuickQuoteObject.QuickQuoteLobType.None, Me.QuoteId, errCreateQSO, ignoreLOBType:=True)
                End If
            End If
            Return _quote
        End Get
    End Property

    Protected ReadOnly Property QuoteId As String
        Get
            If Request.QueryString("quoteid") IsNot Nothing Then
                Return Request.QueryString("quoteid")
            End If
            If Page.RouteData.Values("quoteid") IsNot Nothing Then
                Return Page.RouteData.Values("quoteid").ToString()
            End If
            Return ""
        End Get
    End Property

    Dim _lobType As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType = QuickQuoteObject.QuickQuoteLobType.None
    Public ReadOnly Property NewQuoteRequestLOBType As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType
        Get
            If _lobType = QuickQuoteObject.QuickQuoteLobType.None AndAlso NewQuoteRequestLOBID = 0 Then
                Return QuickQuoteObject.QuickQuoteLobType.None
            Else
                Return _lobType
            End If
        End Get
    End Property

    Public Property NewQuoteRequestLOBID As Int32
        Get
            If Me.ViewState("vs_newlobID") Is Nothing Then
                Me.ViewState("vs_newlobID") = 0
                _lobType = QuickQuoteObject.QuickQuoteLobType.None
            Else
                If _lobType = QuickQuoteObject.QuickQuoteLobType.None Then

                    _lobType = qqHelper.ConvertQQLobIdToQQLobType(Me.ViewState("vs_newlobID")?.ToString())
                End If
            End If
            Return CInt(Me.ViewState("vs_newlobID"))
        End Get
        Set(value As Int32)
            Me.ViewState("vs_newlobID") = value
            _lobType = qqHelper.ConvertQQLobIdToQQLobType(Me.ViewState("vs_newlobID")?.ToString())
        End Set
    End Property

    'added 2/15/2019
    Private _EndorsementPolicyId As Integer = 0
    Public ReadOnly Property EndorsementPolicyId As Integer
        Get
            If _EndorsementPolicyId <= 0 Then
                LoadEndorsementPolicyIdAndImageNum()
            End If
            Return _EndorsementPolicyId
        End Get
    End Property
    Private _EndorsementPolicyImageNum As Integer = 0
    Public ReadOnly Property EndorsementPolicyImageNum As Integer
        Get
            If _EndorsementPolicyImageNum <= 0 Then
                LoadEndorsementPolicyIdAndImageNum()
            End If
            Return _EndorsementPolicyImageNum
        End Get
    End Property
    Public ReadOnly Property EndorsementPolicyIdAndImageNum As String
        Get
            Dim strEndorsementPolicyIdAndImageNum As String = ""
            If Request IsNot Nothing AndAlso Request.QueryString IsNot Nothing AndAlso Request.QueryString("EndorsementPolicyIdAndImageNum") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.QueryString("EndorsementPolicyIdAndImageNum").ToString) = False Then
                strEndorsementPolicyIdAndImageNum = Request.QueryString("EndorsementPolicyIdAndImageNum").ToString
            ElseIf Page IsNot Nothing AndAlso Page.RouteData IsNot Nothing AndAlso Page.RouteData.Values IsNot Nothing AndAlso Page.RouteData.Values("EndorsementPolicyIdAndImageNum") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Page.RouteData.Values("EndorsementPolicyIdAndImageNum").ToString) = False Then
                strEndorsementPolicyIdAndImageNum = Page.RouteData.Values("EndorsementPolicyIdAndImageNum").ToString
            End If
            Return strEndorsementPolicyIdAndImageNum
        End Get
    End Property
    Private Sub LoadEndorsementPolicyIdAndImageNum(Optional ByVal reset As Boolean = False)
        If reset = True Then
            _EndorsementPolicyId = 0
            _EndorsementPolicyImageNum = 0
        End If
        Dim strEndorsementPolicyIdAndImageNum As String = EndorsementPolicyIdAndImageNum
        If String.IsNullOrWhiteSpace(strEndorsementPolicyIdAndImageNum) = False Then
            Dim intList As List(Of Integer) = QuickQuoteHelperClass.ListOfIntegerFromString(strEndorsementPolicyIdAndImageNum, delimiter:="|", positiveOnly:=True)
            If intList IsNot Nothing AndAlso intList.Count > 0 Then
                _EndorsementPolicyId = intList(0)
                If intList.Count > 1 Then
                    _EndorsementPolicyImageNum = intList(1)
                End If
            End If
        End If
    End Sub
    Private _ReadOnlyPolicyId As Integer = 0
    Public ReadOnly Property ReadOnlyPolicyId As Integer
        Get
            If _ReadOnlyPolicyId <= 0 Then
                LoadReadOnlyPolicyIdAndImageNum()
            End If
            Return _ReadOnlyPolicyId
        End Get
    End Property
    Private _ReadOnlyPolicyImageNum As Integer = 0
    Public ReadOnly Property ReadOnlyPolicyImageNum As Integer
        Get
            If _ReadOnlyPolicyImageNum <= 0 Then
                LoadReadOnlyPolicyIdAndImageNum()
            End If
            Return _ReadOnlyPolicyImageNum
        End Get
    End Property
    Public ReadOnly Property ReadOnlyPolicyIdAndImageNum As String
        Get
            Dim strReadOnlyPolicyIdAndImageNum As String = ""
            If Request IsNot Nothing AndAlso Request.QueryString IsNot Nothing AndAlso Request.QueryString("ReadOnlyPolicyIdAndImageNum") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.QueryString("ReadOnlyPolicyIdAndImageNum").ToString) = False Then
                strReadOnlyPolicyIdAndImageNum = Request.QueryString("ReadOnlyPolicyIdAndImageNum").ToString
            ElseIf Page IsNot Nothing AndAlso Page.RouteData IsNot Nothing AndAlso Page.RouteData.Values IsNot Nothing AndAlso Page.RouteData.Values("ReadOnlyPolicyIdAndImageNum") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Page.RouteData.Values("ReadOnlyPolicyIdAndImageNum").ToString) = False Then
                strReadOnlyPolicyIdAndImageNum = Page.RouteData.Values("ReadOnlyPolicyIdAndImageNum").ToString
            End If
            Return strReadOnlyPolicyIdAndImageNum
        End Get
    End Property
    Private Sub LoadReadOnlyPolicyIdAndImageNum(Optional ByVal reset As Boolean = False)
        If reset = True Then
            _ReadOnlyPolicyId = 0
            _ReadOnlyPolicyImageNum = 0
        End If
        Dim strReadOnlyPolicyIdAndImageNum As String = ReadOnlyPolicyIdAndImageNum
        If String.IsNullOrWhiteSpace(strReadOnlyPolicyIdAndImageNum) = False Then
            Dim intList As List(Of Integer) = QuickQuoteHelperClass.ListOfIntegerFromString(strReadOnlyPolicyIdAndImageNum, delimiter:="|", positiveOnly:=True)
            If intList IsNot Nothing AndAlso intList.Count > 0 Then
                _ReadOnlyPolicyId = intList(0)
                If intList.Count > 1 Then
                    _ReadOnlyPolicyImageNum = intList(1)
                End If
            End If
        End If
    End Sub


    Private Property doShowEffectiveDatePicker As Boolean = True

    'added 11/22/17 for HOM Upgrade - MLW
    Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
    Dim _chc As New CommonHelperClass

    Private _script As ctlPageStartupScript

    Public Event ToggleUWPopupShown()

#Region "EffectiveDatePickerCode"
    Public ReadOnly Property EffectiveDate As String
        Get
            If qqHelper.IsDateString(Me.txtUWQuestionsEffectiveDate.Text) = True Then
                Return CDate(Me.txtUWQuestionsEffectiveDate.Text).ToShortDateString()
            Else
                If Quote IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Quote.EffectiveDate) = False AndAlso IsDate(Quote.EffectiveDate) Then
                    Return Quote.EffectiveDate
                Else
                    Return IFM.VR.Common.Helpers.GenericHelper.GetDiamondSystemDate().ToShortDateString()
                End If
            End If
        End Get
    End Property
    'added 9/17/2015 for effDate validation; originally taken from treeview
    Private _minDate As Integer = -1000
    Private _maxDate As Integer = -1000
    Public ReadOnly Property MinimumEffectiveDateDaysFromToday As Integer
        Get
            If _minDate = -1000 Then
                _minDate = QuickQuoteHelperClass.MinimumEffectiveDateDaysFromToday()
            End If



            Return _minDate
        End Get
    End Property
    Public ReadOnly Property MaximumEffectiveDateDaysFromToday As Integer
        Get
            If _maxDate = -1000 Then
                _maxDate = QuickQuoteHelperClass.MaximumEffectiveDateDaysFromToday()
            End If

            Return _maxDate
        End Get
    End Property
    Public Property MinimumEffectiveDate As String
        Get
            If qqHelper.IsDateString(Me.hdnAppMinimumEffectiveDate.Value) = True Then
                Return CDate(Me.hdnAppMinimumEffectiveDate.Value).ToShortDateString
            Else
                Return ""
            End If
        End Get
        Set(value As String)
            If qqHelper.IsDateString(value) = True Then
                Me.hdnAppMinimumEffectiveDate.Value = CDate(value).ToShortDateString
            Else
                Me.hdnAppMinimumEffectiveDate.Value = ""
            End If
        End Set
    End Property
    Public Property MaximumEffectiveDate As String
        Get
            If qqHelper.IsDateString(Me.hdnAppMaximumEffectiveDate.Value) = True Then
                Return CDate(Me.hdnAppMaximumEffectiveDate.Value).ToShortDateString
            Else
                Return ""
            End If
        End Get
        Set(value As String)
            If qqHelper.IsDateString(value) = True Then
                Me.hdnAppMaximumEffectiveDate.Value = CDate(value).ToShortDateString
            Else
                Me.hdnAppMaximumEffectiveDate.Value = ""
            End If
        End Set
    End Property
    Public Property MinimumEffectiveDateAllQuotes As String
        Get
            If qqHelper.IsDateString(Me.hdnAppMinimumEffectiveDateAllQuotes.Value) = True Then
                Return CDate(Me.hdnAppMinimumEffectiveDateAllQuotes.Value).ToShortDateString
            Else
                Return ""
            End If
        End Get
        Set(value As String)
            If qqHelper.IsDateString(value) = True Then
                Me.hdnAppMinimumEffectiveDateAllQuotes.Value = CDate(value).ToShortDateString
            Else
                Me.hdnAppMinimumEffectiveDateAllQuotes.Value = ""
            End If
        End Set
    End Property
    Public Property MaximumEffectiveDateAllQuotes As String
        Get
            If qqHelper.IsDateString(Me.hdnAppMaximumEffectiveDateAllQuotes.Value) = True Then
                Return CDate(Me.hdnAppMaximumEffectiveDateAllQuotes.Value).ToShortDateString
            Else
                Return ""
            End If
        End Get
        Set(value As String)
            If qqHelper.IsDateString(value) = True Then
                Me.hdnAppMaximumEffectiveDateAllQuotes.Value = CDate(value).ToShortDateString
            Else
                Me.hdnAppMaximumEffectiveDateAllQuotes.Value = ""
            End If
        End Set
    End Property
    Public Property QuoteHasMinimumEffectiveDate As Boolean
        Get
            Return qqHelper.BitToBoolean(Me.hdnAppQuoteHasMinimumEffectiveDate.Value)
        End Get
        Set(value As Boolean)
            Me.hdnAppQuoteHasMinimumEffectiveDate.Value = value.ToString
        End Set
    End Property
    Public Property MinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes As Boolean
        Get
            Return qqHelper.BitToBoolean(Me.hdnAppMinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes.Value)
        End Get
        Set(value As Boolean)
            Me.hdnAppMinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes.Value = value.ToString
        End Set
    End Property
    'added 5/16/2023
    Public Property BeforeDateMsg As String
        Get
            Return Me.hdnAppBeforeDateMsg.Value
        End Get
        Set(value As String)
            Me.hdnAppBeforeDateMsg.Value = value
        End Set
    End Property
    Public Property AfterDateMsg As String
        Get
            Return Me.hdnAppAfterDateMsg.Value
        End Get
        Set(value As String)
            Me.hdnAppAfterDateMsg.Value = value
        End Set
    End Property

    Private Property isNewQuote As Boolean
        Get
            Dim returnVar As Boolean = False

            If ViewState("vs_UWQuestionsPopup_IsNewQuote") IsNot Nothing Then
                If Boolean.TryParse(ViewState("vs_UWQuestionsPopup_IsNewQuote"), returnVar) Then

                End If
            End If

            Return returnVar
        End Get
        Set(value As Boolean)
            ViewState("vs_UWQuestionsPopup_IsNewQuote") = value
        End Set
    End Property

    Private Property isCopiedQuote As Boolean
        Get
            Dim returnVar As Boolean = False

            If ViewState("vs_UWQuestionsPopup_IsCopiedQuote") IsNot Nothing Then
                If Boolean.TryParse(ViewState("vs_UWQuestionsPopup_IsCopiedQuote"), returnVar) Then

                End If
            End If

            Return returnVar
        End Get
        Set(value As Boolean)
            ViewState("vs_UWQuestionsPopup_IsCopiedQuote") = value
        End Set
    End Property

    Private ReadOnly Property QuoteNumberDoesNotExistMessage As String
        Get
            Return "Invalid PPA quote or policy number has been entered. Please enter in a valid PPA quote or policy number to proceed."
        End Get
    End Property

    Dim validateEffectiveDate As Boolean = False

    Private ReadOnly _flags As IEnumerable(Of IFeatureFlag)
    Private _questionsInitialized As Boolean = False
    Public Sub New()
        _flags = New List(Of IFeatureFlag) From {New LOB.PPA}
    End Sub

    <Microsoft.Extensions.DependencyInjection.ActivatorUtilitiesConstructor()>
    Public Sub New(flags As IEnumerable(Of IFeatureFlag))
        _flags = flags
    End Sub

    'added 9/17/2015 for effDate validation; originally taken from treeview
    Public Sub SetMinimumAndMaximumEffectiveDates()
        Dim minEffDateAllQuotes As String = DateAdd(DateInterval.Day, MinimumEffectiveDateDaysFromToday, Date.Today).ToShortDateString
        Dim maxEffDateAllQuotes As String = DateAdd(DateInterval.Day, MaximumEffectiveDateDaysFromToday, Date.Today).ToShortDateString

        'added 5/17/2023
        If isNewQuote OrElse isCopiedQuote Then
            Dim lobToUse As QuickQuoteObject.QuickQuoteLobType = QuickQuoteObject.QuickQuoteLobType.None
            If isNewQuote Then
                If System.Enum.IsDefined(GetType(QuickQuoteObject.QuickQuoteLobType), NewQuoteRequestLOBType) = True AndAlso NewQuoteRequestLOBType <> QuickQuoteObject.QuickQuoteLobType.None Then
                    lobToUse = NewQuoteRequestLOBType
                ElseIf Quote IsNot Nothing Then
                    lobToUse = Quote.LobType
                End If
            ElseIf isCopiedQuote Then
                If Quote IsNot Nothing AndAlso System.Enum.IsDefined(GetType(QuickQuoteObject.QuickQuoteLobType), Quote.LobType) = True AndAlso Quote.LobType <> QuickQuoteObject.QuickQuoteLobType.None Then
                    lobToUse = Quote.LobType
                Else
                    lobToUse = NewQuoteRequestLOBType
                End If
            End If
            Dim beforeDtMsg As String = ""
            Dim afterDtMsg As String = ""
            Helpers.WebHelper_Personal.Check_NewBusiness_Min_and_Max_Dates(minEffDateAllQuotes, maxEffDateAllQuotes, lob:=lobToUse, beforeMinDateMsg:=beforeDtMsg, afterMaxDateMsg:=afterDtMsg)
            BeforeDateMsg = beforeDtMsg
            AfterDateMsg = afterDtMsg
        End If

        Dim minEffDate As String = minEffDateAllQuotes
        Dim maxEffDate As String = maxEffDateAllQuotes

        'updated 9/2/2015
        Dim _QuoteHasMinimumEffectiveDate As Boolean = False
        Dim _MinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes As Boolean = False



        'UNCROSSABLE DATE LINE LOGIC - START
        Dim dateToUse As Date
        If isNewQuote OrElse isCopiedQuote Then
            If isNewQuote Then
                dateToUse = IFM.VR.Common.Helpers.GenericHelper.GetDiamondSystemDate()
            ElseIf isCopiedQuote Then
                If String.IsNullOrWhiteSpace(Quote.EffectiveDate) = False AndAlso IsDate(Quote.EffectiveDate) Then
                    dateToUse = CDate(Quote.EffectiveDate).Date
                Else
                    dateToUse = IFM.VR.Common.Helpers.GenericHelper.GetDiamondSystemDate()
                End If
            End If
        End If

        If dateToUse.IsNotNull Then
            If NewQuoteRequestLOBType <> Nothing AndAlso NewQuoteRequestLOBType <> QuickQuoteObject.QuickQuoteLobType.None Then
                Dim uncrossableDateLineDict As New Dictionary(Of QuickQuoteHelperClass.LOBIFMVersions, String)
                If Quote IsNot Nothing Then  ' Needed on new quote
                    If qqHelper.HasIFMLOBVersionUncrossableDateLineWithinRangeOfToday(Nothing, NewQuoteRequestLOBType, uncrossableDateLineDict) Then
                        For Each uncrossableKeyValuePair As KeyValuePair(Of QuickQuoteHelperClass.LOBIFMVersions, String) In uncrossableDateLineDict
                            If IsDate(uncrossableKeyValuePair.Value) Then
                                Dim uncrossableDate As Date = CDate(uncrossableKeyValuePair.Value)
                                If uncrossableDate.IsNotNull Then
                                    If dateToUse >= uncrossableDate Then
                                        If uncrossableDate > CDate(minEffDate) Then
                                            _QuoteHasMinimumEffectiveDate = True
                                            minEffDate = uncrossableDate
                                            _MinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes = True
                                        End If
                                    Else
                                        If isCopiedQuote = True Then 'A new quote is allowed to cross the uncrossable dateline when current date is before that date line. However, a copied quote is not allowed to cross.
                                            If uncrossableDate < CDate(maxEffDate) Then
                                                maxEffDate = uncrossableDate.AddDays(-1) 'We don't want to include the uncrossable date line as that would put us into the new version. So we are going to subract one day.
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        Next
                    End If '
                End If
            End If
        End If
        'UNCROSSABLE DATE LINE LOGIC - END

        'Prevent crossing multistate effective date line on copied quotes or quotes from comp raters - only shows for personal lines because UW questions are not cleared on Com lines so this logic never happens the treeview will stop effective date jumping
        If isCopiedQuote Then
            Dim lobHelper As New IFM.VR.Common.Helpers.LOBHelper(Me.Quote.LobType)
            If lobHelper.IsMultistateCapableLob Then
                If Me.Quote IsNot Nothing AndAlso Me.Quote.EffectiveDate.IsDate() Then
                    If IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(Me.Quote.EffectiveDate) Then
                        If minEffDate < IFM.VR.Common.Helpers.MultiState.General.MultiStateStartDate Then
                            minEffDate = IFM.VR.Common.Helpers.MultiState.General.MultiStateStartDate
                        End If
                    Else
                        If maxEffDate > IFM.VR.Common.Helpers.MultiState.General.MultiStateStartDate Then
                            maxEffDate = IFM.VR.Common.Helpers.MultiState.General.MultiStateStartDate.AddDays(-1)
                        End If
                    End If
                End If
            End If
        End If

        QuoteHasMinimumEffectiveDate = _QuoteHasMinimumEffectiveDate
        MinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes = _MinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes
        MinimumEffectiveDateAllQuotes = minEffDateAllQuotes
        MaximumEffectiveDateAllQuotes = maxEffDateAllQuotes
        MinimumEffectiveDate = minEffDate
        MaximumEffectiveDate = maxEffDate
    End Sub


    Protected Sub txtEffectiveDate_Changed(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtUWQuestionsEffectiveDate.TextChanged
        If IsDate(txtUWQuestionsEffectiveDate.Text) Then
            Dim myEffectiveDate As Date = CDate(txtUWQuestionsEffectiveDate.Text)
            Dim chc As New CommonHelperClass
            Dim hasError As Boolean = False
            Dim ppa = _flags.WithFlags(Of LOB.PPA)

            'adding this to quickly enable LOB/State-based earliest effective date
            If ppa.OhioEnabled AndAlso
               Me.Quote IsNot Nothing AndAlso
               Me.Quote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Ohio AndAlso
               Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal AndAlso
               LOBHelper.IsEffectiveDateAcceptableToLobAndGoverningState(myEffectiveDate, Me.Quote.LobType, Me.Quote.QuickQuoteState) = False Then
                Dim governingState = Me.Quote.QuickQuoteState
                Dim lobDisplayText As String = qqHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.LobId, $"{Me.Quote.LobId}")
                Dim earliestEffectiveDate = LOBHelper.GetEarliesttEffectiveDateForLobAndGoverningState(Me.Quote.LobType, Me.Quote.QuickQuoteState)
                lblEffectiveDateError.Text = $"The effective Date For {lobDisplayText} Quotes In {governingState} must be On Or after {earliestEffectiveDate:MM/dd/yyyy}"

                hasError = True
            ElseIf myEffectiveDate < CDate(MinimumEffectiveDate) OrElse myEffectiveDate > CDate(MaximumEffectiveDate) Then
                hasError = True
                If QuoteHasMinimumEffectiveDate AndAlso myEffectiveDate < CDate(MinimumEffectiveDate) Then
                    'A version conflict is probably the reason here. Tell user they must start a new quote
                    lblEffectiveDateError.Text = "Effective Date prior to " & MinimumEffectiveDate & " is not permitted. Choose a current date or greater to start your quote."
                Else
                    'Outside the normal -30 to 90 date range
                    lblEffectiveDateError.Text = ""
                End If
            End If

            'This logic will show all radio buttons for the UW questions. At one point this was an option Business was thinking about wanting but we took it back out.
            If hasError = False Then
                lblEffectiveDateError.Text = ""
                PopulateQuestions(NewQuoteRequestLOBID.ToString()) 'Reloading in case new questions are needed, functionality related to questions are needed, or new Form selections are needed based on effective date.
                Me.hdnShowDatePickerOnPageLoad.Value = False
                'Dim radioButtonListToShow As List(Of WebControl)
                'radioButtonListToShow = FindControlsOfType_Recursive(Of RadioButton)(Me)
                'If radioButtonListToShow.IsLoaded Then
                '    For Each webctrl As WebControl In radioButtonListToShow
                '        If webctrl IsNot Nothing Then
                '            webctrl.Style.Remove("visibility")
                '        End If
                '    Next
                'End If
            End If
        Else
            lblEffectiveDateError.Text = "Missing Effective Date"
        End If

        ShowHideEnableGoverningStateOptions()
    End Sub
#End Region

    Private Function FindControlsOfType_Recursive(Of T)(control As Control) As List(Of WebControl)
        Dim myWebControls As New List(Of WebControl)

        If control.GetType Is GetType(T) Then
            myWebControls.Add(control)
        End If

        If control.HasControls Then
            For Each childControl As Control In control.Controls
                Dim childWebControlsList As New List(Of WebControl)
                childWebControlsList = FindControlsOfType_Recursive(Of T)(childControl)
                If childWebControlsList.IsLoaded Then
                    For Each webctrl As WebControl In childWebControlsList
                        myWebControls.Add(webctrl)
                    Next
                End If
            Next
        End If

        If myWebControls.Count = 0 Then
            Return Nothing
        Else
            Return myWebControls
        End If
    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load



        _script = DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager

        If Not IsPostBack Then

            If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = True AndAlso String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = True Then 'added IF 5/1/2019 to only show Kill Question popup when not ReadOnly or Endorsement
                If Request.QueryString("newQuote") IsNot Nothing Then
                    Dim lobID As Int32 = -1
                    If Int32.TryParse(Request.QueryString("newQuote"), lobID) Then
                        If VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs().Values.Contains(lobID) Then
                            'added 3/8/2021; original logic in ELSE
                            Dim agencyId As Integer = DirectCast(Me.Page.Master, VelociRater).AgencyID
                            If agencyId > 0 AndAlso qqHelper.IsAgencyIdInCancelledSessionList(agencyId.ToString) = True Then
                                Dim cancelledAgencyText As String = QuickQuoteHelperClass.CancelledAgencyBaseText()
                                _script.AddScriptLine("alert('New Business Quoting is not permitted on " & cancelledAgencyText & " agencies.');")
                            Else
                                isNewQuote = True
                                Me.NewQuoteRequestLOBID = lobID
                                PopulateQuestions(lobID.ToString())
                                If IFM.VR.Common.Helpers.GenericHelper.LOBRequiresEffectiveDateAtQuoteStart(NewQuoteRequestLOBType) = True Then
                                    'This flag, when set to true, will force the datepicker to popup on page load. When set to false, the popup will stay hidden until the user clicks into the field.
                                    Me.hdnShowDatePickerOnPageLoad.Value = True
                                End If
                                If IFM.VR.Common.QuoteSearch.QQSearchResult.commSystemLobs.Contains(lobID.ToString()) Then ' 3-23-15 Matt A
                                    Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_ClientSearch") + "?newquote=" + Request.QueryString("newQuote"), True)
                                    Return
                                End If
                            End If
                        Else
                            _script.AddScriptLine("alert('This line of business is not supported.');")
                        End If
                    End If
                Else
                    Me.btnCancel.Visible = False
                    If Me.Quote IsNot Nothing Then
                        If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
                            Session("DoNotValidateVehicles_" & Session.SessionID.ToString) = True
                        End If

                        If Quote.Database_OriginatedInVR Then
                            isCopiedQuote = True
                        End If

                        Me.NewQuoteRequestLOBID = Me.Quote.LobId
                        If IFM.VR.Common.Helpers.GenericHelper.LOBRequiresEffectiveDateAtQuoteStart(NewQuoteRequestLOBType) = True Then
                            Me.hdnShowDatePickerOnPageLoad.Value = True
                        End If
                        If VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs().Values.Contains(NewQuoteRequestLOBID) Then
                            If (((GetPolicyUws(Me.Quote) Is Nothing OrElse GetPolicyUws(Me.Quote).Any() = False) AndAlso GetPolicyUwsIgnoreLOB() = False) OrElse
                                   (Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.UmbrellaPersonal AndAlso Me.Quote.EffectiveDate = "")) Then

                                ' has no uw questions  
                                PopulateQuestions(Me.Quote.LobId)
                                If Me.Quote.Database_OriginatedInVR = False AndAlso Request.QueryString("showNowInVr") IsNot Nothing Then
                                    Me.VRScript.CreatePopUpWindow_jQueryUi_Dialog_CreatesDiv("Welcome to VelociRater!", "<center><span>You are now on the Indiana Farmers Mutual website to continue processing the quote for your client. <br/>Select OK to proceed.</span></center>", 600, 150, True, True, False, "")
                                    txtUWQuestionsEffectiveDate.Text = Me.Quote.EffectiveDate 'We want to load in the effective date used in Comparative Rater.
                                End If
                            Else
                                ' one or more might be answered
                                'check

                                'updated 11/7/17 for HOM Upgrade - was just else statement - MLW
                                '3/14/2018 - DJG - changed function to take effective date to make it more generic for potential future use by other LOBs
                                Dim questions As List(Of VRUWQuestion)
                                Dim needToShowKillQuestions As Boolean = False

                                _flags.WithFlags(Of LOB.PPA) _
                                      .When(Function(ppa) ppa.OhioEnabled AndAlso
                                                          Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal AndAlso
                                                          Me.Quote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Ohio) _
                                      .Do(Sub()
                                              Dim uwqService = UnderwritingQuestionServiceFactory.BuildFor(Me.Quote.LobType)
                                              Dim request As New UnderwritingQuestionRequest With {
                                                .LobType = Me.Quote.LobType,
                                                .TypeFilter = UnderwritingQuestionRequest.QuestionTypeFilter.KillOnly Or UnderwritingQuestionRequest.QuestionTypeFilter.ExcludeUnmapped,
                                                .GoverningState = Me.Quote.OriginalGoverningState,
                                                .Quote = Me.Quote
                                              }
                                              questions = uwqService.GetQuestions(request).ToList()

                                              If questions.All(Function(q_ans) q_ans.HasBeenAnswered) = False Then
                                                  needToShowKillQuestions = True
                                              End If
                                          End Sub) _
                                      .Else(Sub()
                                                questions = VR.Common.UWQuestions.UWQuestions.GetKillQuestions(Me.Quote.LobId, Quote.EffectiveDate) 'Basing the questions shown on whatever the original Quotes effective date was.

                                                For Each q In (From question In questions Where question.IsTrueUwQuestion Select question)
                                                    Dim a = From qq In GetPolicyUws(Me.Quote) Where qq.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId Select qq
                                                    If a.Any() = False Then
                                                        needToShowKillQuestions = True
                                                    End If
                                                Next
                                            End Sub)

                                If needToShowKillQuestions Then
                                    PopulateQuestions(Me.Quote.LobId)
                                    If Me.Quote.Database_OriginatedInVR = False AndAlso Request.QueryString("showNowInVr") IsNot Nothing Then
                                        Me.VRScript.CreatePopUpWindow_jQueryUi_Dialog_CreatesDiv("Welcome to VelociRater!", "<center><span>You are now on the Indiana Farmers Mutual website to continue processing the quote for your client. <br/>Select OK to proceed.</span></center>", 600, 150, True, True, False, "")
                                    End If
                                End If
                            End If

                            If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal Then
                                If Me.Quote.Locations.Any() AndAlso Me.Quote.Locations(0) IsNot Nothing Then
                                    Me.ddHomFormList.SelectedValue = Me.Quote.Locations(0).FormTypeId
                                    Select Case Me.Quote.Locations(0).FormTypeId
                                        Case "1", "2", "3"
                                            Me.ddHomFormList.Enabled = True ' allow changes on HO-2, Ho-3,HO-3/15
                                        Case "22" 'added 11/7/17 for HOM Upgrade MLW
                                            If Me.hiddenSelectedForm.Value.Contains("MOBILE") Then
                                                Me.Quote.Locations(0).StructureTypeId = "2" 'StructureTypeId 2 = Mobile Home
                                            End If
                                            If Me.Quote.Locations(0).StructureTypeId = "2" Then
                                                Me.ddHomFormList.Enabled = False
                                            Else
                                                Me.ddHomFormList.Enabled = True
                                            End If
                                        Case "23", "24" 'added 11/7/17 for HOM Upgrade MLW
                                            Me.ddHomFormList.Enabled = True
                                        Case Else
                                            Me.ddHomFormList.Enabled = False ' do this after above line in case it fails this won't happen
                                    End Select
                                End If
                                'Else
                                '    Me.ddHomFormList.Enabled = False
                            End If
                        Else
                            _script.AddScriptLine("alert('This line of business is not supported.');")
                        End If

                    End If
                End If

                'Show effective date picker for LOBs or if Lob supports Multistate and possibly needs to ask for governing state
                If True Then 'IFM.VR.Common.Helpers.GenericHelper.LOBRequiresEffectiveDateAtQuoteStart(NewQuoteRequestLOBType) OrElse IFM.VR.Common.Helpers.MultiState.General.MultiStateQuoteableLOBs().Contains(NewQuoteRequestLOBID) Then
                    SetMinimumAndMaximumEffectiveDates()
                Else
                    div_EffectiveDate.Style.Add("display", "none")
                    doShowEffectiveDatePicker = False
                End If

                'added 8/8/2018
                ShowHideEnableGoverningStateOptions()
                If Me.Quote IsNot Nothing AndAlso System.Enum.IsDefined(GetType(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState), Me.Quote.QuickQuoteState) = True AndAlso Me.Quote.QuickQuoteState <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None Then
                    Me.ddlGoverningState.SelectedValue = Me.Quote.StateId
                End If
            End If
        End If

        'This logic will hide all radio buttons for the UW questions. At one point this was an option Business was thinking about wanting but we took it back out.
        'If String.IsNullOrWhiteSpace(NewQuoteRequestLOBID) = False Then
        '    If hdnHideDatePicker.Value = False Then
        '        Dim radioButtonListToHide As List(Of WebControl)
        '        radioButtonListToHide = FindControlsOfType_Recursive(Of RadioButton)(Me)
        '        If radioButtonListToHide.IsLoaded Then
        '            For Each webctrl As WebControl In radioButtonListToHide
        '                If webctrl IsNot Nothing Then
        '                    webctrl.Style.Add("visibility", "hidden")
        '                End If
        '            Next
        '        End If
        '    End If
        'End If
    End Sub

    Private Sub ShowHideEnableGoverningStateOptions()
        Dim lobHelper As New IFM.VR.Common.Helpers.LOBHelper(Me.NewQuoteRequestLOBID)
        Dim acceptableGoverningStateIds = lobHelper.AcceptableGoverningStateIds(EffectiveDate)
        Dim currentlySelectedStateId As String = Me.ddlGoverningState.SelectedValue

        If IsDate(EffectiveDate) AndAlso acceptableGoverningStateIds IsNot Nothing AndAlso acceptableGoverningStateIds.Count > 1 = True Then
            Me.GoverningStateSection.Visible = True
            Me.ddlGoverningState.Items.Clear()
            Me.ddlGoverningState.Items.Add(New ListItem("", ""))
            ' Use this case statement to exlcude new states before their effective date
            For Each stateRecord In IFM.VR.Common.Helpers.States.GetStateInfosFromIds(acceptableGoverningStateIds)
                'Select Case stateRecord.Abbreviation
                '    Case "OH"
                '        If CDate(EffectiveDate) >= IFM.VR.Common.Helpers.GenericHelper.GetOhioEffectiveDate() Then
                '            Me.ddlGoverningState.Items.Add(New ListItem(stateRecord.StateName, stateRecord.StateId))
                '        End If
                '        Exit Select
                '    Case Else
                Me.ddlGoverningState.Items.Add(New ListItem(stateRecord.StateName, stateRecord.StateId))
                '        Exit Select
                'End Select
            Next
            Me.ddlGoverningState.Enabled = Not qqHelper.IsPositiveIntegerString(Me.QuoteId) AndAlso qqHelper.IsDateString(txtUWQuestionsEffectiveDate.Text) 'prevents existing quotes like copied quotes or from comp raters from changing the governing s
            If Me.ddlGoverningState.Enabled = False AndAlso Quote.IsNotNull() AndAlso Not Quote.StateId = String.Empty Then
                Dim governingStateItem = Me.ddlGoverningState.Items.FindByValue(Me.Quote.StateId)

                If governingStateItem Is Nothing Then
                    governingStateItem = New ListItem(Me.Quote.State, Me.Quote.StateId)
                    Me.ddlGoverningState.Items.Add(governingStateItem)
                End If
                Me.ddlGoverningState.SelectedValue = Me.Quote.StateId
            End If
            'Start a New Quote (QuoteSummary)  New quotes won't have an effective date, copied will. 12/11/2018 CAH
            If Me.Quote IsNot Nothing AndAlso Not IsDate(Me.Quote.EffectiveDate) Then
                Me.ddlGoverningState.Enabled = True
                If Me.Quote.StateId IsNot Nothing Then
                    Me.ddlGoverningState.SelectedValue = Me.Quote.StateId
                End If
            End If

            If ddlGoverningState.SelectedValue = "" Then
                ddlGoverningState.SelectedValue = currentlySelectedStateId
            End If

        Else
            Me.GoverningStateSection.Visible = False
        End If
    End Sub

    Public Sub PopulateQuestions(LobId As String)
        Dim _script = DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager
        _script.AddVariableLine("var kqLobId = " + LobId + ";")
        Loadquestions(LobId)
        ShowPopup()
    End Sub

    Private Function CreateQuote(lobID As Int32) As Int32
        Dim agencyId As Int32 = (DirectCast(Me.Page.Master, VelociRater)).AgencyID
        Dim agencyCode As String = (DirectCast(Me.Page.Master, VelociRater)).AgencyCode
        Dim newQuoteId As String = ""
        Dim errorMsg As String = ""
        If IFM.VR.Common.QuoteSave.QuoteSaveHelpers.CreateNewQuote(agencyId, agencyCode, lobID, newQuoteId, errorMsg) = False Then
            'error
        End If
        Dim qID As Int32
        Int32.TryParse(newQuoteId, qID)
        Return qID
    End Function

    ' Consider putting this logic in the common lib - Matt A 2-18-2015
    Private Sub RedirectToEditPages(lobid As Int32, newQuoteId As Int32)
        'If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False OrElse String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then 'added IF 2/15/2019; original logic in ELSE
        '    Dim imgQueryString As String = ""
        '    If String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
        '        imgQueryString = "?EndorsementPolicyIdAndImageNum=" & Me.EndorsementPolicyId.ToString & "|" & Me.EndorsementPolicyImageNum.ToString
        '    Else
        '        imgQueryString = "?ReadOnlyPolicyIdAndImageNum=" & Me.ReadOnlyPolicyId.ToString & "|" & Me.ReadOnlyPolicyImageNum.ToString
        '    End If
        '    Select Case lobid
        '        Case 1
        '            Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_PPA_Input") & imgQueryString, True)
        '        Case 2
        '            Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_HOM_Input") & imgQueryString, True)
        '        Case 17
        '            Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_FAR_Input") & imgQueryString, True)
        '        Case 3
        '            Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_DFR_Input") & imgQueryString, True)
        '        Case 9
        '            If IFM.VR.Common.VRFeatures.NewLookCGLEnabled Then
        '                Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CGL_Quote_NewLook") & imgQueryString, True)
        '            End If
        '        Case 25  ' BOP
        '            If IFM.VR.Common.VRFeatures.NewLookBOPEnabled Then
        '                Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_BOP_Quote_NewLook") & imgQueryString, True)
        '            End If
        '        Case 20  ' CAP
        '            If IFM.VR.Common.VRFeatures.NewLookCAPEnabled Then
        '                Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CAP_Quote_NewLook") & imgQueryString, True)
        '            End If
        '        Case 21  ' WCP
        '            If IFM.VR.Common.VRFeatures.NewLookWCPEnabled Then
        '                Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_WCP_Quote_NewLook") & imgQueryString, True)
        '            End If
        '        Case 28  ' CPR
        '            If IFM.VR.Common.VRFeatures.NewLookCPREnabled Then
        '                Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CPR_Quote_NewLook") & imgQueryString, True)
        '            End If
        '        Case 23  ' CPP
        '            If IFM.VR.Common.VRFeatures.NewLookCPPEnabled Then
        '                Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CPP_Quote_NewLook") & imgQueryString, True)
        '            End If
        '    End Select
        'Else
        '    Select Case lobid
        '        Case 1
        '            Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_PPA_Input") + "?quoteid=" + newQuoteId.ToString(), True)
        '        Case 2
        '            Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_HOM_Input") + "?quoteid=" + newQuoteId.ToString(), True)
        '        Case 17
        '            Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_FAR_Input") + "?quoteid=" + newQuoteId.ToString(), True)
        '        Case 3
        '            Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_DFR_Input") + "?quoteid=" + newQuoteId.ToString(), True)
        '        Case 9
        '            If IFM.VR.Common.VRFeatures.NewLookCGLEnabled Then
        '                Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CGL_Quote_NewLook") + "?quoteid=" + newQuoteId.ToString(), True)
        '            End If
        '        Case 25  ' BOP
        '            If IFM.VR.Common.VRFeatures.NewLookBOPEnabled Then
        '                Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_BOP_Quote_NewLook") + "?quoteid=" + newQuoteId.ToString(), True)
        '            End If
        '        Case 20  ' CAP
        '            If IFM.VR.Common.VRFeatures.NewLookCAPEnabled Then
        '                Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CAP_Quote_NewLook") + "?quoteid=" + newQuoteId.ToString(), True)
        '            End If
        '        Case 21  ' WCP
        '            If IFM.VR.Common.VRFeatures.NewLookWCPEnabled Then
        '                Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_WCP_Quote_NewLook") + "?quoteid=" + newQuoteId.ToString(), True)
        '            End If
        '        Case 28  ' CPR
        '            If IFM.VR.Common.VRFeatures.NewLookCPREnabled Then
        '                Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CPR_Quote_NewLook") + "?quoteid=" + newQuoteId.ToString(), True)
        '            End If
        '        Case 23  ' CPp
        '            If IFM.VR.Common.VRFeatures.NewLookCPPEnabled Then
        '                Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CPP_Quote_NewLook") + "?quoteid=" + newQuoteId.ToString(), True)
        '            End If
        '    End Select
        'End If
        'updated 6/11/2019 to use new helper method
        Dim polIdToUse As Integer = 0
        Dim polImgNumToUse As Integer = 0
        Dim quoteIdToUse As Integer = 0
        Dim lobTypeToUse As QuickQuoteObject.QuickQuoteLobType = QuickQuoteObject.QuickQuoteLobType.None
        Dim tranType As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.None
        If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False OrElse String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
            If String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                polIdToUse = Me.EndorsementPolicyId
                polImgNumToUse = Me.EndorsementPolicyImageNum
                tranType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote
            Else
                polIdToUse = Me.ReadOnlyPolicyId
                polImgNumToUse = Me.ReadOnlyPolicyImageNum
                tranType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage
            End If
        Else
            quoteIdToUse = newQuoteId
            tranType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote
        End If
        If quoteIdToUse > 0 OrElse (polIdToUse > 0 AndAlso polImgNumToUse > 0) Then
            lobTypeToUse = QuickQuoteHelperClass.LobTypeForLobId(lobid)
            'IFM.VR.Web.Helpers.WebHelper_Personal.RedirectToQuotePage(tranType, lobTypeToUse, quoteId:=quoteIdToUse, policyId:=polIdToUse, policyImageNum:=polImgNumToUse, goToApp:=False)
            'updated 7/18/2023
            Dim additionalQuerystringParams As String = ""
            If (Me.divCommDataPrefill.Visible = True AndAlso Me.radCommDataPrefillYes.Checked = True) OrElse (Helpers.WebHelper_Personal.UseCommercialDataPrefill() = True AndAlso Helpers.WebHelper_Personal.UseKillQuestionForCommercialDataPrefill() = False AndAlso WebHelper_Personal.CommercialDataPrefill_OkayToAutoLaunchIfNeeded() = False) Then
                Dim commDataPrefillQuerystring = Helpers.WebHelper_Personal.CommercialDataPrefillQuerystringParam()
                If String.IsNullOrWhiteSpace(commDataPrefillQuerystring) = True Then
                    commDataPrefillQuerystring = "CommDataPrefill"
                End If
                additionalQuerystringParams = "&" & commDataPrefillQuerystring & "=Yes"
            End If
            IFM.VR.Web.Helpers.WebHelper_Personal.RedirectToQuotePage(tranType, lobTypeToUse, quoteId:=quoteIdToUse, policyId:=polIdToUse, policyImageNum:=polImgNumToUse, goToApp:=False, AdditionalQueryStringParams:=additionalQuerystringParams)
        End If

    End Sub

    Private Sub Loadquestions(lobid As Int32)
        Dim questions As List(Of VRUWQuestion)
        'Updated 11/7/17 for HOM Upgrade - was just else statement - MLW
        ' No need for questions if Umbrella, don't waste the cycles. - CAH
        If NewQuoteRequestLOBType <> QuickQuoteObject.QuickQuoteLobType.UmbrellaPersonal Then
            '_flags.WithFlags(Of LOB.PPA) _
            '      .When(Function(ppa) ppa.OhioEnabled AndAlso
            '                            (Me.Quote IsNot Nothing AndAlso
            '                             Me.Quote.OriginalGoverningState = QuickQuoteHelperClass.QuickQuoteState.Ohio AndAlso
            '                             Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal)) _
            '      .Do(Sub()
            '              Dim uwqService = UnderwritingQuestionServiceFactory.BuildFor(Me.Quote.LobType)
            '              Dim request As New UwQuestionRequest With {
            '                .LobType = Me.Quote.LobType,
            '                .TypeFilter = UwQuestionRequest.QuestionTypeFilter.KillOnly
            '              }
            '              questions = uwqService.GetQuestions(request).ToList()
            '          End Sub) _
            '      .Else(Sub()
            questions = VR.Common.UWQuestions.UWQuestions.GetKillQuestions(lobid, EffectiveDate)
            '            End Sub)
        End If

        'Switched to use NewQuoteRequestLOBType for developer readability
        Select Case NewQuoteRequestLOBType
            Case QuickQuoteObject.QuickQuoteLobType.Farm
                questions.RemoveAt(questions.Count() - 1) 'remove the last question because it has a weird workflow that you need special logic to handle
                Exit Select
            Case QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                Exit Select
            Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                Session("CAPNewQuote_" & Session.SessionID) = "true"
                Exit Select
        End Select

        SetKillQuestionAnswers(questions)

        Me.Repeater1.DataSource = questions
        Me.Repeater1.DataBind()

        Me.divFormType.Visible = False
        Me.divAddtionalAutoQuestions.Visible = False
        Me.divAdditionalHomeQuestions.Visible = False
        Me.divAdditionalDFRQuestions.Visible = False
        Me.divLossHistoryHOM.Visible = False
        Me.divDwellingAgeDFR.Visible = False
        Me.divFarmDogQuestions.Visible = False
        Me.divAdditionalBOPQuestions.Visible = False 'Added 09/02/2021 for bug 51550 MLW
        Me.divUmbrellaSection.Visible = False
        Me.divCommDataPrefill.Visible = False 'added 7/18/2023

        hdnIsHOMLossHistoryKillQuestionAvailable.Value = "False"
        hdnIsDFRLossHistoryKillQuestionAvailable.Value = "False"

        Select Case NewQuoteRequestLOBType
            Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                Me.divAddtionalAutoQuestions.Visible = True
            Case QuickQuoteObject.QuickQuoteLobType.HomePersonal
                Me.divFormType.Visible = True
                'qqHelper.LoadStaticDataOptionsDropDown(Me.ddHomFormList, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, QuickQuoteStaticDataOption.SortBy.None, QuickQuoteObject.QuickQuoteLobType.HomePersonal)
                'added 11/22/17 for HOM Upgrade - get form list by Version in XML
                Dim selectedHomFormIndex As Integer
                If qqHelper.doUseNewVersionOfLOB(EffectiveDate, NewQuoteRequestLOBType, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                    'If Me.Quote.IsNotNull() AndAlso Me.Quote.Locations.IsLoaded() Then
                    Dim optionAttributes As New List(Of QuickQuoteStaticDataAttribute)
                    Dim a1 As New QuickQuoteStaticDataAttribute
                    a1.nvp_name = "Version"
                    a1.nvp_value = "After20180701"
                    optionAttributes.Add(a1)
                    selectedHomFormIndex = ddHomFormList.SelectedIndex
                    qqHelper.LoadStaticDataOptionsDropDownWithMatchingAttributes(Me.ddHomFormList, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, optionAttributes, QuickQuoteStaticDataOption.SortBy.None, QuickQuoteObject.QuickQuoteLobType.HomePersonal)
                    'End If
                Else
                    Dim optionAttributes As New List(Of QuickQuoteStaticDataAttribute)
                    Dim a1 As New QuickQuoteStaticDataAttribute
                    a1.nvp_name = "Version"
                    a1.nvp_value = "Before20180701"
                    optionAttributes.Add(a1)
                    qqHelper.LoadStaticDataOptionsDropDownWithMatchingAttributes(Me.ddHomFormList, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, optionAttributes, QuickQuoteStaticDataOption.SortBy.None, QuickQuoteObject.QuickQuoteLobType.HomePersonal)
                End If
                If selectedHomFormIndex = -1 Then
                    ddHomFormList.SelectedIndex = 1
                Else
                    ddHomFormList.SelectedIndex = selectedHomFormIndex
                End If
                If Me.Quote IsNot Nothing Then
                    ' this is a copied quote
                    If Me.Quote.Locations.Any() Then
                        Select Case Me.Quote.Locations(0).FormTypeId
                            Case "1", "2", "3"
                                'Updated 12/13/17 for HOM Upgrade MLW
                                If qqHelper.doUseNewVersionOfLOB(EffectiveDate, NewQuoteRequestLOBType, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                                    For i As Int32 = Me.ddHomFormList.Items.Count - 1 To 0 Step -1
                                        If Me.ddHomFormList.Items(i).Value <> "22" And Me.ddHomFormList.Items(i).Value <> "23" And Me.ddHomFormList.Items(i).Value <> "24" Then
                                            Me.ddHomFormList.Items.RemoveAt(i)
                                        Else
                                            If Me.ddHomFormList.Items(i).Value = "22" Then
                                                'else 'if 22 and Mobile
                                                If Me.ddHomFormList.Items(i).Text.Contains("MOBILE") Then
                                                    Me.ddHomFormList.Items.RemoveAt(i)
                                                End If
                                            End If
                                        End If
                                    Next
                                    If Me.Quote.Locations(0).FormTypeId = "1" Then
                                        Me.ddHomFormList.SelectedValue = "22"
                                    End If
                                    If Me.Quote.Locations(0).FormTypeId = "2" Then
                                        Me.ddHomFormList.SelectedValue = "23"
                                    End If
                                    If Me.Quote.Locations(0).FormTypeId = "3" Then
                                        Me.ddHomFormList.SelectedValue = "24"
                                    End If
                                Else
                                    ' limit the selectable dropdown list
                                    For i As Int32 = Me.ddHomFormList.Items.Count - 1 To 0 Step -1
                                        If Me.ddHomFormList.Items(i).Value <> "1" And Me.ddHomFormList.Items(i).Value <> "2" And Me.ddHomFormList.Items(i).Value <> "3" Then
                                            Me.ddHomFormList.Items.RemoveAt(i)
                                        End If
                                    Next
                                    Me.ddHomFormList.SelectedValue = Me.Quote.Locations(0).FormTypeId ' reselect the current form type dropdown as a default
                                End If
                            Case "4" 'added 12/13/17 for HOM Upgrade
                                If qqHelper.doUseNewVersionOfLOB(EffectiveDate, NewQuoteRequestLOBType, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                                    For i As Int32 = Me.ddHomFormList.Items.Count - 1 To 0 Step -1
                                        If Me.ddHomFormList.Items(i).Value <> "25" Then
                                            Me.ddHomFormList.Items.RemoveAt(i)
                                        Else
                                            Me.ddHomFormList.SelectedValue = "25"
                                        End If
                                    Next
                                End If
                            Case "5" 'added 12/13/17 for HOM Upgrade
                                If qqHelper.doUseNewVersionOfLOB(EffectiveDate, NewQuoteRequestLOBType, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                                    For i As Int32 = Me.ddHomFormList.Items.Count - 1 To 0 Step -1
                                        If Me.ddHomFormList.Items(i).Value <> "26" Then
                                            Me.ddHomFormList.Items.RemoveAt(i)
                                        Else
                                            Me.ddHomFormList.SelectedValue = "26"
                                        End If
                                    Next
                                End If
                            Case "6" 'added 12/13/17 for HOM Upgrade
                                If qqHelper.doUseNewVersionOfLOB(EffectiveDate, NewQuoteRequestLOBType, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                                    For i As Int32 = Me.ddHomFormList.Items.Count - 1 To 0 Step -1
                                        If Me.ddHomFormList.Items(i).Value <> "22" Then
                                            Me.ddHomFormList.Items.RemoveAt(i)
                                        Else
                                            If Me.ddHomFormList.Items(i).Text.Contains("MOBILE") Then
                                                Me.ddHomFormList.SelectedValue = "22"
                                            Else
                                                Me.ddHomFormList.Items.RemoveAt(i)
                                            End If
                                        End If
                                    Next
                                End If
                            Case "7" 'added 12/13/17 for HOM Upgrade
                                If qqHelper.doUseNewVersionOfLOB(EffectiveDate, NewQuoteRequestLOBType, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                                    For i As Int32 = Me.ddHomFormList.Items.Count - 1 To 0 Step -1
                                        If Me.ddHomFormList.Items(i).Value <> "25" Then
                                            Me.ddHomFormList.Items.RemoveAt(i)
                                        Else
                                            If Me.ddHomFormList.Items(i).Text.Contains("MOBILE") Then
                                                Me.ddHomFormList.SelectedValue = "25"
                                            Else
                                                Me.ddHomFormList.Items.RemoveAt(i)
                                            End If
                                        End If
                                    Next
                                End If


                            Case "23", "24" 'added 11/6/17 for HOM Upgrade MLW
                                ' limit the selectable dropdown list
                                For i As Int32 = Me.ddHomFormList.Items.Count - 1 To 0 Step -1
                                    If Me.ddHomFormList.Items(i).Value <> "22" And Me.ddHomFormList.Items(i).Value <> "23" And Me.ddHomFormList.Items(i).Value <> "24" Then
                                        Me.ddHomFormList.Items.RemoveAt(i)
                                    Else
                                        If Me.ddHomFormList.Items(i).Value = "22" Then
                                            'else 'if 22 and Mobile
                                            If Me.ddHomFormList.Items(i).Text.Contains("MOBILE") Then
                                                Me.ddHomFormList.Items.RemoveAt(i)
                                            End If
                                        End If
                                    End If
                                Next
                                Me.ddHomFormList.SelectedValue = Me.Quote.Locations(0).FormTypeId ' reselect the current form type dropdown as a default
                            Case "22" 'added 12/13/17 for HOM Upgrade
                                If qqHelper.doUseNewVersionOfLOB(EffectiveDate, NewQuoteRequestLOBType, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                                    For i As Int32 = Me.ddHomFormList.Items.Count - 1 To 0 Step -1
                                        If Me.ddHomFormList.Items(i).Value <> "22" And Me.ddHomFormList.Items(i).Value <> "23" And Me.ddHomFormList.Items(i).Value <> "24" Then
                                            Me.ddHomFormList.Items.RemoveAt(i)
                                        Else
                                            If Me.Quote.Locations(0).StructureTypeId = "2" Then
                                                If Me.ddHomFormList.Items(i).Text.Contains("MOBILE") Then
                                                Else
                                                    Me.ddHomFormList.Items.RemoveAt(i)
                                                End If
                                            Else
                                                If Me.ddHomFormList.Items(i).Text.Contains("MOBILE") Then
                                                    Me.ddHomFormList.Items.RemoveAt(i)
                                                End If
                                            End If
                                            Me.ddHomFormList.SelectedValue = "22"
                                        End If
                                    Next
                                End If
                            Case "25" 'added 12/13/17 for HOM Upgrade
                                If qqHelper.doUseNewVersionOfLOB(EffectiveDate, NewQuoteRequestLOBType, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                                    For i As Int32 = Me.ddHomFormList.Items.Count - 1 To 0 Step -1
                                        If Me.ddHomFormList.Items(i).Value <> "25" Then
                                            Me.ddHomFormList.Items.RemoveAt(i)
                                        Else
                                            If Me.Quote.Locations(0).StructureTypeId = "2" Then
                                                If Me.ddHomFormList.Items(i).Text.Contains("MOBILE") Then
                                                Else
                                                    Me.ddHomFormList.Items.RemoveAt(i)
                                                End If
                                            Else
                                                If Me.ddHomFormList.Items(i).Text.Contains("MOBILE") Then
                                                    Me.ddHomFormList.Items.RemoveAt(i)
                                                End If
                                            End If
                                            Me.ddHomFormList.SelectedValue = "25"
                                        End If
                                    Next
                                End If
                            Case Else
                        End Select
                    End If
                End If

                ' Bug 3521 MGB 9/18/14
                Me.divAdditionalHomeQuestions.Visible = True
                radHOMMultiPolicyYes.Attributes.Add("alwaysRequireAdditionalAnswer", "true")

                Dim myQuote = New QuickQuoteObject
                myQuote.EffectiveDate = EffectiveDate
                myQuote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal

                If HOMExistingAutoHelper.IsHOMExistingAutoAvailable(myQuote) Then
                    radHOMMultiPolicyNo.Attributes.Add("onclick", "if (confirm('" + inEligibleMsgNo + "')){HideUwQuestions(); window.location = '" + System.Configuration.ConfigurationManager.AppSettings("QuickQuote_Personal_HomePage") + "';} else {var x = document.getElementById('" & radHOMMultiPolicyNo.ClientID & "'); if (x != null){x.checked = false;} else{alert('rbyes NOT found'); } }")
                End If

                If LossHistoryKillQuestionHelper.IsLossHistoryKillQuestionAvailable(myQuote) Then
                    divLossHistoryHOM.Visible = True 'HOM and DFR use the same div
                    radLossYes.Attributes.Add("onclick", "if (confirm('" + inEligibleMsg + "')){HideUwQuestions(); window.location = '" + System.Configuration.ConfigurationManager.AppSettings("QuickQuote_Personal_HomePage") + "';} else {var x = document.getElementById('" & radLossYes.ClientID & "'); if (x != null){x.checked = false;} else{alert('rbyes NOT found'); } }")
                    hdnIsHOMLossHistoryKillQuestionAvailable.Value = "True"
                End If
            Case QuickQuoteObject.QuickQuoteLobType.Farm
                divFarmDogQuestions.Visible = True
            Case QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                divAdditionalDFRQuestions.Visible = True
                radDFRYes.Attributes.Add("alwaysRequireAdditionalAnswer", "true")
                Me.divFormType.Visible = True
                Dim selectedHomFormValue = ddHomFormList.SelectedValue
                qqHelper.LoadStaticDataOptionsDropDown(Me.ddHomFormList, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, QuickQuoteStaticDataOption.SortBy.None, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal)
                ddHomFormList.SetFromValue(selectedHomFormValue)
                If Me.Quote.IsNotNull() AndAlso Me.Quote.Locations.IsLoaded() Then

                    Dim optionAttributes As New List(Of QuickQuoteStaticDataAttribute) 'Matt A 10-27-15
                    Dim a1 As New QuickQuoteStaticDataAttribute
                    a1.nvp_name = "FromFormTypeId" 'only shows the formtypes that are valid based on current form type
                    a1.nvp_value = Me.Quote.Locations(0).FormTypeId
                    optionAttributes.Add(a1)
                    qqHelper.LoadStaticDataOptionsDropDownWithMatchingAttributes(ddHomFormList, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, optionAttributes, QuickQuoteStaticDataOption.SortBy.None, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal)

                    Me.ddHomFormList.SetFromValue(Me.Quote.Locations(0).FormTypeId)
                    Me.ddHomFormList.Enabled = True 'Allow changes on copied quotes
                End If
                Dim myQuote = New QuickQuoteObject
                myQuote.EffectiveDate = EffectiveDate
                myQuote.LobType = QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                If DFRLossHistoryKillQuestionHelper.IsDFRLossHistoryKillQuestionAvailable(myQuote) Then
                    divLossHistoryHOM.Visible = True 'HOM and DFR use the same div
                    radLossYes.Attributes.Add("onclick", "if (confirm('" + inEligibleMsg + "')){HideUwQuestions(); window.location = '" + System.Configuration.ConfigurationManager.AppSettings("QuickQuote_Personal_HomePage") + "';} else {var x = document.getElementById('" & radLossYes.ClientID & "'); if (x != null){x.checked = false;} else{alert('rbyes NOT found'); } }")
                    hdnIsDFRLossHistoryKillQuestionAvailable.Value = "True"
                End If
                divDwellingAgeDFR.Visible = True
                radDwellingAgeYes.Attributes.Add("onclick", "if (confirm('" + inEligibleMsg + "')){HideUwQuestions(); window.location = '" + System.Configuration.ConfigurationManager.AppSettings("QuickQuote_Personal_HomePage") + "';} else {var x = document.getElementById('" & radDwellingAgeYes.ClientID & "'); if (x != null){x.checked = false;} else{alert('rbyes NOT found'); } }")
            Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                'Added 09/02/2021 for bug 51550 MLW
                Me.divAdditionalBOPQuestions.Visible = True
                Dim myQuote = New QuickQuoteObject
                myQuote.EffectiveDate = EffectiveDate
                myQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                If BopStpUwQuestionsHelper.IsBopStpUwQuestionsAvailable(myQuote) Then
                    trBOPOver35k.Visible = True
                    trBOPOver3Stories.Visible = True
                    trBOPSales6M.Visible = True
                    radBOPIncOcc.Visible = True
                    tr_BOP_IncidentalOccupancy.Visible = True
                    radBOPIncOccYes.Attributes.Add("onclick", "alert('" & Me.ValidationMessage & "');")
                Else
                    trBOPOver35k.Visible = False
                    trBOPOver3Stories.Visible = False
                    trBOPSales6M.Visible = False
                    radBOPIncOcc.Visible = False
                    tr_BOP_IncidentalOccupancy.Visible = False
                End If
                If Helpers.WebHelper_Personal.UseCommercialDataPrefill() = True AndAlso Helpers.WebHelper_Personal.UseKillQuestionForCommercialDataPrefill() = True Then
                    Me.divCommDataPrefill.Visible = True
                    Helpers.WebHelper_Personal.AddStyleToHtmlControl(Me.CommDataPrefillTableRow, "background-color", "lightgray")
                End If
            Case QuickQuoteObject.QuickQuoteLobType.UmbrellaPersonal

                divUmbrellaSection.Visible = True
            Case QuickQuoteObject.QuickQuoteLobType.CommercialProperty 'added 7/18/2023
                If Helpers.WebHelper_Personal.UseCommercialDataPrefill() = True AndAlso Helpers.WebHelper_Personal.UseKillQuestionForCommercialDataPrefill() = True Then
                    Me.divCommDataPrefill.Visible = True
                    Helpers.WebHelper_Personal.AddStyleToHtmlControl(Me.CommDataPrefillTableRow, "background-color", "lightgray")
                End If
            Case QuickQuoteObject.QuickQuoteLobType.CommercialPackage 'added 7/18/2023
                If Helpers.WebHelper_Personal.UseCommercialDataPrefill() = True AndAlso Helpers.WebHelper_Personal.UseKillQuestionForCommercialDataPrefill() = True Then
                    Me.divCommDataPrefill.Visible = True
                End If
        End Select

    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        DoRepeaterSave()
    End Sub
    Private Sub DoRepeaterSave()
        Dim _script = DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager
        If DirectCast(Me.Page.Master, VelociRater).AgencyID > 0 Then
            Dim qq As QuickQuote.CommonObjects.QuickQuoteObject = Nothing
            Dim qqID As Int32 = 0
            Dim isNewQuote As Boolean = False
            Dim shouldTryToSaveToQuoteObject As Boolean = True 'Must start as true or now new quotes will be created - 7-12-18
            Dim errorMessage As String = ""
            Dim addCondoDAndOCoverage As Boolean = False 'Added 09/01/2021 for bug 51550 MLW

            For Each row As RepeaterItem In Me.Repeater1.Items
                Dim radYes As RadioButton = row.FindControl("radYes")
                Dim radNo As RadioButton = row.FindControl("radNo")
                Dim txtMoreInfo As TextBox = row.FindControl("txtMoreInfo")
                If (radYes.Checked = False And radNo.Checked = False) Or (radYes.Checked And String.IsNullOrWhiteSpace(txtMoreInfo.Text)) Then
                    shouldTryToSaveToQuoteObject = False
                    Exit For
                End If
            Next

            'Added 09/02/2021 for bug 51550 MLW
            'Condo D&O Question for BOP - If Yes save as CPP. If No, save as BOP.
            If NewQuoteRequestLOBID = 25 AndAlso radBOPCondoDandOYes.Checked Then '25 = BOP
                addCondoDAndOCoverage = True
                Me.NewQuoteRequestLOBID = 23 'change to commercial package for condo D&O '23 = CPP
            End If

            If NewQuoteRequestLOBID = 25 AndAlso
                (radBOPOver35kYes.Checked OrElse radBOPOver3StoriesYes.Checked OrElse radBOPSales6MYes.Checked) Then '25 = BOP
                Me.NewQuoteRequestLOBID = 23 'change to commercial package '23 = CPP
            End If



            Dim effDate As DateTime
            If DateTime.TryParse(EffectiveDate, effDate) Then
                If StopOnOrBeforeDate(Me.NewQuoteRequestLOBID) > DateTime.MinValue Then
                    If (EffectiveDate <= StopOnOrBeforeDate(Me.NewQuoteRequestLOBID)) Then
                        'stop
                        Me.VRScript.AddScriptLine($"alert('{StopOnOrBeforeDateMsg(Me.NewQuoteRequestLOBID)}');")
                        Return
                    End If
                End If

                If StopOnOrAfterDate(Me.NewQuoteRequestLOBID) < DateTime.MaxValue Then
                    If StopOnOrAfterDate(Me.NewQuoteRequestLOBID) < DateTime.MaxValue Then
                        If (EffectiveDate >= StopOnOrAfterDate(Me.NewQuoteRequestLOBID)) Then
                            'stop
                            Me.VRScript.AddScriptLine($"alert('{StopOnOrAfterDateMsg(Me.NewQuoteRequestLOBID)}');")
                            Return
                        End If
                    End If

                End If
            End If


            If Me.Quote Is Nothing Then
                If String.IsNullOrWhiteSpace(ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/15/2019 (may not be needed, but will keep placeholder); original logic in ELSE
                    'shouldn't hit here unless nothing was pulled for Quote
                ElseIf String.IsNullOrWhiteSpace(EndorsementPolicyIdAndImageNum) = False Then
                    'shouldn't hit here unless nothing was pulled for Quote
                Else
                    qqID = Me.CreateQuote(Me.NewQuoteRequestLOBID)
                    Dim errCreateQSO As String = ""
                    If shouldTryToSaveToQuoteObject Then
                        'qq = VR.Common.QuoteSave.QuoteSaveHelpers.GetQuoteById_NOSESSION(qqID)
                        'updated 10/4/2019 (12/11/2019 in Sprint 10); could also set to Me.Quote now, but may be able to get from CreateQuote call
                        qq = VR.Common.QuoteSave.QuoteSaveHelpers.GetQuoteById(QuickQuoteObject.QuickQuoteLobType.None, qqID, errCreateQSO, ignoreLOBType:=True)
                    End If
                    isNewQuote = True
                End If
            Else
                qqID = Me.QuoteId
                qq = Me.Quote
            End If


            If isNewQuote = False Then
                If qqHelper.IsEffectiveDateChangeCrossingUncrossableDateLine(Quote, Quote.EffectiveDate, EffectiveDate, errorMessage) Then
                    shouldTryToSaveToQuoteObject = False
                End If

                'When crossing RAPA date line backward, need to remove the 3 new vehicle symbols
                If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal AndAlso NewRAPASymbolsHelper.NewRAPASymbolsEnabled() AndAlso IsDate(Quote.EffectiveDate) AndAlso IsDate(EffectiveDate) Then
                    Dim OrigDate As Date = CDate(Quote.EffectiveDate)
                    Dim NewDate As Date = CDate(EffectiveDate)
                    Dim RapaDateLine As Date = NewRAPASymbolsHelper.NewRAPASymbolsEffDate '1/1/2025 date line
                    If OrigDate >= RapaDateLine AndAlso NewDate < RapaDateLine Then
                        'Date going backwards across date line - OrigDate 2025, NewDate 2024
                        NewRAPASymbolsHelper.RemoveThreeNewRAPASymbols(Quote)
                    End If
                End If

                'When crossing FARM Building date line forward, need to set default values
                If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm AndAlso FarmBuildingtypeforbuildingHelper.FARMBuildingTypeForBuildingsEnabled() AndAlso IsDate(Quote.EffectiveDate) AndAlso IsDate(EffectiveDate) Then
                    Dim OrigDate As Date = CDate(Quote.EffectiveDate)
                    Dim NewDate As Date = CDate(EffectiveDate)
                    Dim FarmTypeDateLine As Date = FarmBuildingtypeforbuildingHelper.FARMBuildingTypeForBuildingsEffDate '04/15/2025 date line
                    If OrigDate < FarmTypeDateLine AndAlso NewDate >= FarmTypeDateLine Then
                        'Date forward date logic
                        FarmBuildingtypeforbuildingHelper.UpdateDefaultBuildingType(Quote)
                    End If
                End If
            End If

            If Me.NewQuoteRequestLOBID = 1 AndAlso shouldTryToSaveToQuoteObject = True Then
                If Me.radMultiPolicyNo.Checked = False And Me.radMultiPolicyYes.Checked = False Then
                    shouldTryToSaveToQuoteObject = False
                End If
                'Added 10/5/18 for multi state MLW
                Dim governingStateIds = IFM.VR.Common.Helpers.LOBHelper.AcceptableGoverningStateIds(QuickQuoteObject.QuickQuoteLobType.AutoPersonal, EffectiveDate)
                If governingStateIds.Contains(IFM.VR.Common.Helpers.States.Abbreviations.IL) OrElse
                   governingStateIds.Contains(IFM.VR.Common.Helpers.States.Abbreviations.OH) Then
                    If Me.radMultiPolicyYes.Checked = True AndAlso String.IsNullOrWhiteSpace(txtMoreInfoRelated.Text) Then
                        shouldTryToSaveToQuoteObject = False
                    End If
                End If

                If HOMExistingAutoHelper.IsHOMExistingAutoAvailable(Me.Quote) AndAlso Me.radHOMMultiPolicyYes.Checked = True AndAlso String.IsNullOrWhiteSpace(txtMoreInfoRelatedAuto.Text) Then
                    shouldTryToSaveToQuoteObject = False
                End If
            End If

            If shouldTryToSaveToQuoteObject Then

                If Me.GoverningStateSection.Visible = True AndAlso Me.ddlGoverningState.Enabled = True AndAlso Me.ddlGoverningState.SelectedValue.TryToGetInt32() < 1 Then
                    Me.VRScript.ShowAlert("Invalid Governing State Selection")
                End If

                'added 8/8/2018 for multi-state; will set for all states when applicable; code updated to use stateQuote instead of qq
                If qq IsNot Nothing Then
                    'MUST Do this very first thing in case other stuff happens based on governing state
                    If Me.GoverningStateSection.Visible = True AndAlso Me.ddlGoverningState.Enabled = True Then
                        qq.StateId = Me.ddlGoverningState.SelectedValue
                    End If

                    '8/8/2018 note: all of the following logic is for the top quote level; nothing state-specific until we get to multiStateQuotes loop

                    Dim forceShowNoPostBack As Boolean = False
                    Dim cannotProceed As Boolean = False

                    'Updated 11/7/17 for HOM Upgrade - was just else statement - MLW
                    Dim questions As List(Of VR.Common.UWQuestions.VRUWQuestion)
                    qq.EffectiveDate = EffectiveDate

                    _flags.WithFlags(Of LOB.PPA) _
                          .When(Function(ppa) ppa.OhioEnabled AndAlso
                                              qq.LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal AndAlso
                                              qq.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Ohio) _
                          .Do(Sub()
                                  Dim uwqService = UnderwritingQuestionServiceFactory.BuildFor(qq.LobType)

                                  Dim request As New UnderwritingQuestionRequest With {
                                                .LobType = qq.LobType,
                                                .TypeFilter = UnderwritingQuestionRequest.QuestionTypeFilter.KillOnly,
                                                .GoverningState = qq.QuickQuoteState,
                                                .Quote = Me.Quote
                                              }
                                  questions = uwqService.GetQuestions(request).ToList()
                              End Sub) _
                          .Else(Sub()
                                    questions = VR.Common.UWQuestions.UWQuestions.GetKillQuestions(qq.LobId, qq.EffectiveDate)
                                End Sub)

                    'Dim questions As List(Of VR.Common.UWQuestions.VRUWQuestion) = VR.Common.UWQuestions.UWQuestions.GetKillQuestions(qq.LobId)

                    Select Case NewQuoteRequestLOBID
                        Case 1
                            If IFM.VR.Common.Helpers.PPA.PPA_General.IsParachuteQuote(qq) Then
                                'Reverted back to 12 annual 2 for task 40608 MLW
                                ''Updated 6/18/2019 for Bug 31360 MLW
                                '9/29/2021 note: would need to be updated if we ever start loading payplans based on effDate (would rely on VR_Default_PayPlanIds and VR_ConvertPayPlanIdsIfNeeded config keys until then)
                                qq.BillingPayPlanId = "12" 'annual 2 '5-21-18 - Parachute
                                'qq.BillingPayPlanId = "14" 'quarterly 2
                                qq.BillMethodId = "2" 'direct bill '5-21-18 - Parachute
                                qq.BillToId = "1" 'insured '5-21-18 - Parachute
                            End If
                        Case 2
                            'set form type
                            If qq IsNot Nothing Then
                                If qq.Locations Is Nothing Then
                                    qq.Locations = New List(Of QuickQuoteLocation)()
                                End If
                                If qq.Locations.Any() = False Then
                                    qq.Locations.Add(New QuickQuote.CommonObjects.QuickQuoteLocation())
                                End If
                                Dim priorFormType As String = qq.Locations(0).FormTypeId

                                If String.IsNullOrWhiteSpace(priorFormType) = False AndAlso priorFormType <> Me.ddHomFormList.SelectedValue Then
                                    ' form type changed - it was not nothing before so it is a real change
                                    If ((qq.Locations(0).FormTypeId = "22" OrElse qq.Locations(0).FormTypeId = "25") AndAlso qq.Locations(0).StructureTypeId = "2" AndAlso qq.LobId = "2") Then
                                        Dim optionAttributes As New List(Of QuickQuoteStaticDataAttribute) 'Matt A 10-27-15
                                        Dim a1 As New QuickQuoteStaticDataAttribute
                                        a1.nvp_name = "StructureTypeId" 'only way to determine mobile types on the new form types is by having StructureTypeId set to 2. Therefore, only use this to get the formType description/name for mobile on new forms. MLW
                                        a1.nvp_value = 2
                                        optionAttributes.Add(a1)
                                        Me.Quote.QuoteDescription = qqHelper.GetStaticDataTextForValue_MatchingOptionAttributes(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, optionAttributes, Me.ddHomFormList.SelectedValue, QuickQuoteObject.QuickQuoteLobType.HomePersonal) + " - " + Me.Quote.QuoteDescription
                                    Else
                                        Me.Quote.QuoteDescription = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, Me.ddHomFormList.SelectedValue, QuickQuoteObject.QuickQuoteLobType.HomePersonal) + " - " + Me.Quote.QuoteDescription
                                    End If
                                End If

                                'T40490 CAH- Show new question, pre-selected (We will select if an AI is present)
                                ' Only pre-select if New not a copied quote (existing priorFormType)
                                If String.IsNullOrWhiteSpace(priorFormType) Then
                                    If Me.ddHomFormList.SelectedValue.EqualsAny("22", "23", "24") AndAlso Me.hiddenSelectedForm.Value.Contains("MOBILE") = False Then
                                        IFM.VR.Common.Helpers.AdditionalInterest.AddFakeAI_HOM(qq)
                                    End If
                                End If

                                qq.Locations(0).FormTypeId = Me.ddHomFormList.SelectedValue
                                ' Set Multi-Policy value
                                ' Bug 3521 MGB 9/18/14
                                qq.Locations(0).MultiPolicyDiscount = radHOMMultiPolicyYes.Checked
                                'qq.AutoHome = radHOMMultiPolicyYes.Checked
                                'added 11/6/17 for HOM Upgrade MLW - was just last else statement for 6, 7
                                If qqHelper.doUseNewVersionOfLOB(qq, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                                    If qq.Database_OriginatedInVR = False AndAlso String.IsNullOrWhiteSpace(qq.Locations(0).DeductibleLimitId) = False AndAlso IFM.VR.Common.Helpers.HOM.PolicyDeductibleNotLessThan1kHelper.IsPolicyDeductibleNotLessThan1kAvailable(qq) Then
                                        'For HOM quotes coming from CompRater, if the deductible is less than 1k, default the deductible to 1k
                                        Select Case qq.Locations(0).DeductibleLimitId
                                            Case "21", "22", "23"
                                                '21 = 250, 22 = 500, 23 = 750
                                                qq.Locations(0).DeductibleLimitId = "24" '1,000
                                        End Select
                                    End If
                                    If qq.Locations(0).FormTypeId <> "22" And qq.Locations(0).FormTypeId <> "25" Then
                                        qq.Locations(0).SelectMarketCredit = True
                                        'Added 8/15/18 for rater Bug 28327 - MLW
                                        If qq.Locations(0).StructureTypeId Is Nothing OrElse qq.Locations(0).StructureTypeId = "" Then
                                            qq.Locations(0).StructureTypeId = 0 'added 11/30/17 for HOM Upgrade, must pass value for StructureTypeId since it determines mobile or not with new form types MLW
                                        End If
                                    Else
                                        If Me.hiddenSelectedForm.Value.Contains("MOBILE") Then 'for initial eval on new quotes, not for existing quotes. Existing quotes will need to check formType description for the word MOBILE. 11/27/17 MLW
                                            qq.Locations(0).StructureTypeId = 2 'added 11/30/17 for HOM Upgrade, must pass value for StructureTypeId since it determines mobile or not with new form types MLW
                                            qq.Locations(0).SelectMarketCredit = True 'Added 7/2/18 for HOM2011 Upgrade post go-live changes MLW - now available on mobile homes
                                        Else
                                            'Added 8/15/18 for rater Bug 28327 - MLW
                                            If qq.Locations(0).StructureTypeId Is Nothing OrElse qq.Locations(0).StructureTypeId = "" Then
                                                qq.Locations(0).StructureTypeId = 0 'added 11/30/17 for HOM Upgrade, must pass value for StructureTypeId since it determines mobile or not with new form types MLW
                                            End If
                                            qq.Locations(0).SelectMarketCredit = True
                                        End If
                                    End If
                                Else
                                    If qq.Locations(0).FormTypeId <> "6" And qq.Locations(0).FormTypeId <> "7" Then
                                        qq.Locations(0).SelectMarketCredit = True '9-23-14 Doug M said give them this by default
                                        qq.Locations(0).StructureTypeId = 0 'added 11/30/17 for HOM Upgrade, must pass value for StructureTypeId since it determines mobile or not with new form types MLW
                                    Else
                                        qq.Locations(0).StructureTypeId = 2 'added 11/30/17 for HOM Upgrade, must pass value for StructureTypeId since it determines mobile or not with new form types MLW
                                    End If
                                End If

                                'Added 10/12/2022 for bug 76006 MLW
                                If isCopiedQuote AndAlso IFM.VR.Common.Helpers.HOM.StructureTypeManufactureHelper.StructureTypeManufacturedEnabled() AndAlso IsDate(qq.EffectiveDate) Then
                                    If CDate(qq.EffectiveDate) >= IFM.VR.Common.Helpers.HOM.StructureTypeManufactureHelper.StructureTypeManufacturedEffDate() Then
                                        'Forward
                                        HOM.StructureTypeManufactureHelper.UpdateStructureType(Quote, Common.Helper.EnumsHelper.CrossDirectionEnum.FORWARD)
                                    Else
                                        'Backward
                                        HOM.StructureTypeManufactureHelper.UpdateStructureType(Quote, Common.Helper.EnumsHelper.CrossDirectionEnum.BACK)
                                    End If
                                End If
                            End If
                        Case 17 'FARM
                            If isCopiedQuote AndAlso (qq.QuickQuoteState = QuickQuoteState.Indiana OrElse qq.QuickQuoteState = QuickQuoteState.Illinois) Then
                                If qq.Locations IsNot Nothing AndAlso qq.Locations.Count > 0 Then
                                    qq.Locations(0).FarmTypeHobby = False
                                    qq.Locations(0).HobbyFarmCredit = False
                                End If
                            End If

                        Case 3 ' DFR
                            If qq.Locations Is Nothing Then
                                qq.Locations = New List(Of QuickQuoteLocation)()
                            End If
                            If qq.Locations.Any() = False Then
                                qq.Locations.Add(New QuickQuote.CommonObjects.QuickQuoteLocation())
                            End If

                            Dim priorFormType As String = qq.Locations(0).FormTypeId

                            If String.IsNullOrWhiteSpace(priorFormType) = False AndAlso priorFormType <> Me.ddHomFormList.SelectedValue Then
                                ' form type changed - it was not nothing before so it is a real change
                                If ((qq.Locations(0).FormTypeId = "22" OrElse qq.Locations(0).FormTypeId = "25") AndAlso qq.Locations(0).StructureTypeId = "2") Then
                                    Dim optionAttribute As New List(Of QuickQuoteStaticDataAttribute)
                                    Dim a2 As New QuickQuoteStaticDataAttribute
                                    a2.nvp_name = "StructureTypeId" 'only way to determine mobile types on the new form types is by having StructureTypeId set to 2. Therefore, only use this to get the formType description/name for mobile on new forms. MLW
                                    a2.nvp_value = 2
                                    optionAttribute.Add(a2)
                                    Me.Quote.QuoteDescription = qqHelper.GetStaticDataTextForValue_MatchingOptionAttributes(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, optionAttribute, Me.ddHomFormList.SelectedValue, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal) + " - " + Me.Quote.QuoteDescription
                                Else
                                    Me.Quote.QuoteDescription = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, Me.ddHomFormList.SelectedValue, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal) + " - " + Me.Quote.QuoteDescription
                                End If
                            End If
                            qq.Locations(0).FormTypeId = Me.ddHomFormList.SelectedValue
                        Case 9 ' CGL
                            ' Setting locations for CGL 'Z Shanks 10/16/17
                            If qq.Locations Is Nothing Then
                                qq.Locations = New List(Of QuickQuoteLocation)()
                            End If
                            If qq.Locations.Any() = False Then
                                qq.Locations.Add(New QuickQuote.CommonObjects.QuickQuoteLocation())
                            End If

                        Case 25 ' BOP
                            If qq.Locations Is Nothing Then
                                qq.Locations = New List(Of QuickQuoteLocation)()
                            End If
                            If qq.Locations.Any() = False Then
                                qq.Locations.Add(New QuickQuote.CommonObjects.QuickQuoteLocation())
                            End If

                        Case 20 ' CAP
                            'First location must be governing state and must work to always ensure that governing state keeps a location
                            If qq.Locations.IsLoaded() = False Then
                                qq.Locations = New List(Of QuickQuoteLocation)()
                                qq.Locations.Add(New QuickQuote.CommonObjects.QuickQuoteLocation())
                                If qq.Locations(0).Address Is Nothing Then
                                    qq.Locations(0).Address = New QuickQuoteAddress()
                                End If
                                qq.Locations(0).Address.StateId = qq.StateId
                            End If

                        Case 21 ' WCP
                            ' No location set for WCP

                        Case 28 ' CPR
                            If qq.Locations Is Nothing Then
                                qq.Locations = New List(Of QuickQuoteLocation)()
                            End If
                            If qq.Locations.Any() = False Then
                                qq.Locations.Add(New QuickQuote.CommonObjects.QuickQuoteLocation())
                                If PropertyAddressProtectionClassHelper.ispaProtectionClassUnitsAvailable(qq) Then
                                    Dim MyLocation = qq.Locations?.LastOrDefault
                                    If MyLocation IsNot Nothing Then
                                        MyLocation.FeetToFireHydrant = "1000"
                                        MyLocation.MilesToFireDepartment = "5"
                                    End If
                                End If
                            End If

                        Case 23 ' CPP
                            ' Anything?
                            If qq.Locations Is Nothing Then
                                qq.Locations = New List(Of QuickQuoteLocation)()
                            End If
                            If qq.Locations.Any() = False Then
                                qq.Locations.Add(New QuickQuote.CommonObjects.QuickQuoteLocation())
                                If PropertyAddressProtectionClassHelper.ispaProtectionClassUnitsAvailable(qq) Then
                                    Dim MyLocation = qq.Locations?.LastOrDefault
                                    If MyLocation IsNot Nothing Then
                                        MyLocation.FeetToFireHydrant = "1000"
                                        MyLocation.MilesToFireDepartment = "5"
                                    End If
                                End If
                            End If

                        Case 14 ' Umbrella

                        Case Else
                            Exit Select
                    End Select

                    ' SET EPLI DEFAULT TO TRUE FOR CPP - MGB 6-27-18
                    If NewQuoteRequestLOBType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                        IFM.VR.Common.Helpers.CGL.EPLIHelper.Toggle_EPLI_Is_Applied(qq, True)
                        IFM.VR.Common.Helpers.CGL.CLIHelper.Toggle_CLI_Is_Applied(qq, True) 'zts 5/15/19
                    End If

                    If NewQuoteRequestLOBType = QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability Then
                        ' CGL get EPLI by default
                        IFM.VR.Common.Helpers.CGL.EPLIHelper.Toggle_EPLI_Is_Applied(qq, True) ' use this so it won't apply it if it is a copied quote and it has classcodes that don't allow EPLI
                    End If

                    ' Set defaults for BOP
                    If NewQuoteRequestLOBType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP Then
                        ' BOP get EPLI by default
                        IFM.VR.Common.Helpers.CGL.EPLIHelper.Toggle_EPLI_Is_Applied(qq, True) ' use this so it won't apply it if it is a copied quote and it has classcodes that don't allow EPLI
                        IFM.VR.Common.Helpers.CGL.CLIHelper.Toggle_CLI_Is_Applied(qq, True) 'zts 5/15/19
                    End If

                    Dim multiStateQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject) = qqHelper.MultiStateQuickQuoteObjects(qq) 'should always return at least qq in the list
                    If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                        For Each stateQuote As QuickQuote.CommonObjects.QuickQuoteObject In multiStateQuotes
                            '8/8/2018 note: all of the logic in this loop is state-specific

                            Dim index As Int32 = 0

                            Select Case NewQuoteRequestLOBID
                                Case 1
                                    SetPolicyUws(stateQuote, New List(Of QuickQuotePolicyUnderwriting)) ' always do
                                    stateQuote.AutoHome = Me.radMultiPolicyYes.Checked
                                Case 2
                                    'set form type
                                    SetPolicyUws(stateQuote, New List(Of QuickQuotePolicyUnderwriting)) ' always do

                                    'added 11/6/17 for HOM Upgrade MLW - was just last else statement for 6, 7
                                    If qqHelper.doUseNewVersionOfLOB(stateQuote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                                        'Added 1/11/18 for HOM Upgrade MLW - to fix issue where Personal Liability and Medical Payments are set to 0 when null later in the quote process.
                                        If qqHelper.IsPositiveIntegerString(stateQuote.PersonalLiabilityLimitId) = False Then 'added IF 3/9/2021; previously happening every time
                                            stateQuote.PersonalLiabilityLimitId = "262" 'id for 100,000
                                        End If
                                        If qqHelper.IsPositiveIntegerString(stateQuote.MedicalPaymentsLimitid) = False Then 'added IF 3/9/2021; previously happening every time
                                            stateQuote.MedicalPaymentsLimitid = "170" 'id for 1,000
                                        End If
                                    Else

                                    End If
                                Case 17 'FARM
                                    SetPolicyUws(stateQuote, New List(Of QuickQuotePolicyUnderwriting)) ' always do
                                Case 3 ' DFR

                                    SetPolicyUws(stateQuote, New List(Of QuickQuotePolicyUnderwriting)) ' always do

                                Case 9 ' CGL
                                    ' Setting locations for CGL 'Z Shanks 10/16/17
                                    SetPolicyUws(stateQuote, New List(Of QuickQuotePolicyUnderwriting)) ' always do
                                Case 25 ' BOP
                                    SetPolicyUws(stateQuote, New List(Of QuickQuotePolicyUnderwriting)) ' always do
                                Case 20 ' CAP
                                    ' No location set for CAP
                                    SetPolicyUws(stateQuote, New List(Of QuickQuotePolicyUnderwriting)) ' always do
                                Case 21 ' WCP
                                    ' No location set for WCP
                                    SetPolicyUws(stateQuote, New List(Of QuickQuotePolicyUnderwriting)) ' always do
                                Case 28 ' CPR
                                    SetPolicyUws(stateQuote, New List(Of QuickQuotePolicyUnderwriting)) ' always do
                                Case 23 ' CPP
                                    ' Anything?
                                    SetPolicyUws(stateQuote, New List(Of QuickQuotePolicyUnderwriting)) ' always do
                                    'Added 09/01/2021 for bug 51550 MLW
                                    If addCondoDAndOCoverage Then
                                        stateQuote.HasCondoDandO = True
                                    End If
                                Case 14 ' Umbrella
                                    'SetPolicyUws(stateQuote, New List(Of QuickQuotePolicyUnderwriting)) ' No Questions for Umb.
                                    If UmbrellaTypeFarm.Checked Then
                                        stateQuote.ProgramTypeId = "5" 'Mark as Farm
                                        If UmbrellaFarmTypeCommercial.Checked Then
                                            stateQuote.UmbrellaSelfInsuredRetentionLimitId = "286"
                                            'removed 10/1/2021; will now default applicants on governingStateQuote if needed (just like all other LOBs; backend can handle Umbrella)
                                            'If stateQuote.Applicants Is Nothing Then
                                            '    stateQuote.Applicants = New List(Of QuickQuoteApplicant)()
                                            'End If
                                            'If stateQuote.Applicants.Any() = False Then
                                            '    stateQuote.Applicants.Add(New QuickQuoteApplicant())
                                            'End If
                                            If UmbrellaFarmCommercialTypeIndividual.Checked Then
                                                stateQuote.FarmTypeId = "6" 'Individual
                                            Else
                                                stateQuote.FarmTypeId = "7" 'Family Farm Corp. or Partnership
                                            End If

                                            'removed 10/1/2021; will now default applicants on governingStateQuote if needed (just like all other LOBs; backend can handle Umbrella)
                                            'If Not qq.Applicants?.Any() Then 'initialize top level applications list to have one item
                                            '    qq.Applicants?.AddNew()
                                            'End If
                                        Else
                                            stateQuote.UmbrellaSelfInsuredRetentionLimitId = "165" 'Personal farm
                                            stateQuote.FarmTypeId = "6" 'Individual
                                        End If

                                        If UmbrellaFarmSizeSmall.Checked Then
                                            stateQuote.FarmSizeTypeId = "1" ' Mark as Small farm
                                        Else
                                            stateQuote.FarmSizeTypeId = "2" ' Mark as Large farm
                                        End If
                                    Else
                                        stateQuote.ProgramTypeId = "4" 'Mark as Personal
                                        stateQuote.FarmTypeId = "5" 'N/A
                                    End If

                                Case Else
                                    Exit Select
                            End Select

                            For Each row As RepeaterItem In Me.Repeater1.Items
                                ' it is possible that this question is an IFM question and not a real UW questions
                                ' only real UW questions increment the index variable
                                Dim question As VR.Common.UWQuestions.VRUWQuestion = questions(index)
                                If question.IsTrueUwQuestion Then
                                    Dim radYes As RadioButton = row.FindControl("radYes")
                                    Dim radNo As RadioButton = row.FindControl("radNo")
                                    Dim txtMoreInfo As TextBox = row.FindControl("txtMoreInfo")

                                    Dim uw As New QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting()
                                    '' if this is an existing quote then you don't want to blast away all current answers
                                    'Dim existingAnswer = From q In PolicyUws Where q.PolicyUnderwritingCodeId = question.PolicyUnderwritingCodeId Select q
                                    'If existingAnswer.Any() Then
                                    '    ' you have found the prior answer to this question just overwrite
                                    '    uw = (From q In PolicyUws Where q.PolicyUnderwritingCodeId = question.PolicyUnderwritingCodeId Select q).FirstOrDefault()
                                    'End If

                                    uw.PolicyUnderwritingCodeId = question.PolicyUnderwritingCodeId
                                    uw.PolicyUnderwritingLevelId = question.PolicyUnderwritingLevelId
                                    uw.PolicyUnderwritingAnswerTypeId = question.PolicyUnderwritingAnswerTypeId
                                    uw.PolicyUnderwritingExtraAnswerTypeId = question.PolicyUnderwritingExtraAnswerTypeId
                                    uw.PolicyUnderwritingTabId = question.PolicyUnderwritingTabId
                                    If radYes.Checked Then
                                        uw.PolicyUnderwritingAnswer = "1"
                                        uw.PolicyUnderwritingAnswerTypeId = "0"
                                        uw.PolicyUnderwritingExtraAnswer = txtMoreInfo.Text.Trim().ToMaxLength(249) ' Matt A - Added max length 12-15-2015
                                        uw.PolicyUnderwritingExtraAnswerTypeId = 1 '1=TEXT, 2=Date, 3=Currency
                                        If String.IsNullOrWhiteSpace(txtMoreInfo.Text) Then
                                            cannotProceed = True
                                        End If
                                    Else
                                        If radNo.Checked Then
                                            uw.PolicyUnderwritingAnswerTypeId = "0"
                                            uw.PolicyUnderwritingAnswer = "-1"

                                            If radYes.Attributes("alwaysRequiresAdditionalAnswer") IsNot Nothing Then 'special logic for Question #5 of DFR
                                                uw.PolicyUnderwritingExtraAnswer = txtMoreInfo.Text.Trim().ToMaxLength(249) ' Matt A - Added max length 12-15-2015
                                                uw.PolicyUnderwritingExtraAnswerTypeId = 1 '1=TEXT, 2=Date, 3=Currency
                                            End If
                                        Else
                                            'neither were chosen
                                            uw.PolicyUnderwritingAnswerTypeId = "0"
                                            uw.PolicyUnderwritingAnswer = ""
                                        End If

                                    End If
                                    GetPolicyUws(stateQuote).Add(uw)
                                    'index += 1 ' only incremented on real UW questions and not on IFM created additional kill questions
                                End If
                                index += 1

                                'LOB Specific items
                                If (stateQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty Or stateQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage) AndAlso question.PolicyUnderwritingCodeId = "9400" Then
                                    Dim uw As New QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting()
                                    Dim radYes As RadioButton = row.FindControl("radYes")
                                    Dim radNo As RadioButton = row.FindControl("radNo")
                                    Dim txtMoreInfo As TextBox = row.FindControl("txtMoreInfo")

                                    uw.PolicyUnderwritingCodeId = "9010"
                                    uw.PolicyUnderwritingLevelId = "1"
                                    uw.PolicyUnderwritingAnswerTypeId = question.PolicyUnderwritingAnswerTypeId
                                    uw.PolicyUnderwritingExtraAnswerTypeId = question.PolicyUnderwritingExtraAnswerTypeId
                                    uw.PolicyUnderwritingTabId = "3"
                                    If radYes.Checked Then
                                        uw.PolicyUnderwritingAnswer = "1"
                                        uw.PolicyUnderwritingAnswerTypeId = "0"
                                        uw.PolicyUnderwritingExtraAnswer = txtMoreInfo.Text.Trim().ToMaxLength(249) ' Matt A - Added max length 12-15-2015
                                        uw.PolicyUnderwritingExtraAnswerTypeId = 1 '1=TEXT, 2=Date, 3=Currency
                                    Else
                                        If radNo.Checked Then
                                            uw.PolicyUnderwritingAnswerTypeId = "0"
                                            uw.PolicyUnderwritingAnswer = "-1"
                                        Else
                                            'neither were chosen
                                            uw.PolicyUnderwritingAnswerTypeId = "0"
                                            uw.PolicyUnderwritingAnswer = ""
                                            forceShowNoPostBack = True
                                        End If
                                    End If
                                    GetPolicyUws(stateQuote).Add(uw)
                                End If

                                If (stateQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage) AndAlso (question.PolicyUnderwritingCodeId = "9006" OrElse question.PolicyUnderwritingCodeId = "9007") Then
                                    '9345 9346  -- 9006 9007
                                    Dim uw As New QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting()
                                    Dim radYes As RadioButton = row.FindControl("radYes")
                                    Dim radNo As RadioButton = row.FindControl("radNo")
                                    Dim txtMoreInfo As TextBox = row.FindControl("txtMoreInfo")
                                    Dim questionCodeID As String
                                    If question.PolicyUnderwritingCodeId = "9006" Then
                                        questionCodeID = "9345"
                                    Else
                                        questionCodeID = "9346"
                                    End If

                                    uw.PolicyUnderwritingCodeId = questionCodeID
                                    uw.PolicyUnderwritingLevelId = "1"
                                    uw.PolicyUnderwritingAnswerTypeId = question.PolicyUnderwritingAnswerTypeId
                                    uw.PolicyUnderwritingExtraAnswerTypeId = question.PolicyUnderwritingExtraAnswerTypeId
                                    uw.PolicyUnderwritingTabId = "2"
                                    If radYes.Checked Then
                                        uw.PolicyUnderwritingAnswer = "1"
                                        uw.PolicyUnderwritingAnswerTypeId = "0"
                                        uw.PolicyUnderwritingExtraAnswer = txtMoreInfo.Text.Trim().ToMaxLength(249) ' Matt A - Added max length 12-15-2015
                                        uw.PolicyUnderwritingExtraAnswerTypeId = 1 '1=TEXT, 2=Date, 3=Currency
                                    Else
                                        If radNo.Checked Then
                                            uw.PolicyUnderwritingAnswerTypeId = "0"
                                            uw.PolicyUnderwritingAnswer = "-1"
                                        Else
                                            'neither were chosen
                                            uw.PolicyUnderwritingAnswerTypeId = "0"
                                            uw.PolicyUnderwritingAnswer = ""
                                            forceShowNoPostBack = True
                                        End If
                                    End If
                                    GetPolicyUws(stateQuote).Add(uw)
                                End If
                            Next

                            'Added 10/9/18 effective date for multi state MLW
                            Dim isStateExpansion As Boolean = False
                            If EffectiveDate IsNot Nothing AndAlso IsDate(EffectiveDate) Then
                                Dim governingStateIds = IFM.VR.Common.Helpers.LOBHelper.AcceptableGoverningStateIds(QuickQuoteObject.QuickQuoteLobType.AutoPersonal, EffectiveDate)
                                isStateExpansion = governingStateIds.Contains(IFM.VR.Common.Helpers.States.Abbreviations.IL) OrElse
                                                   governingStateIds.Contains(Common.Helpers.States.Abbreviations.OH)
                            End If

                            If isStateExpansion Then
                                If stateQuote.LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal Then
                                    Dim uw8 As New QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting()
                                    uw8.PolicyUnderwritingCodeId = IIf(_flags.WithFlags(Of LOB.PPA).OhioEnabled AndAlso qq.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Ohio,
                                                                        "9593", "9290")
                                    If radMultiPolicyYes.Checked Then
                                        uw8.PolicyUnderwritingAnswerTypeId = "0"
                                        uw8.PolicyUnderwritingAnswer = "1"
                                        uw8.PolicyUnderwritingExtraAnswer = Me.txtMoreInfoRelated.Text.Trim().ToMaxLength(249)
                                        uw8.PolicyUnderwritingExtraAnswerTypeId = 1
                                        If String.IsNullOrWhiteSpace(txtMoreInfoRelated.Text) Then
                                            cannotProceed = True
                                        End If
                                    Else
                                        If radMultiPolicyNo.Checked Then
                                            uw8.PolicyUnderwritingAnswerTypeId = "0"
                                            uw8.PolicyUnderwritingAnswer = "-1"
                                        Else
                                            'neither
                                            uw8.PolicyUnderwritingAnswerTypeId = "0"
                                            uw8.PolicyUnderwritingAnswer = ""
                                            'forceShowNoPostBack = True
                                        End If
                                    End If
                                    GetPolicyUws(stateQuote).Add(uw8)
                                End If
                            End If

                            If stateQuote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal Then
                                Dim uw8 As New QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting()
                                uw8.PolicyUnderwritingCodeId = "9302"
                                If radHOMMultiPolicyYes.Checked Then
                                    uw8.PolicyUnderwritingAnswerTypeId = "0"
                                    uw8.PolicyUnderwritingAnswer = "1"
                                    uw8.PolicyUnderwritingExtraAnswer = Me.txtMoreInfoRelatedAuto.Text.Trim().ToMaxLength(249)
                                    uw8.PolicyUnderwritingExtraAnswerTypeId = 1
                                    If String.IsNullOrWhiteSpace(txtMoreInfoRelatedAuto.Text) OrElse Not EnteredValidQuoteOrPolicyNumber(uw8.PolicyUnderwritingExtraAnswer, 1) Then
                                        cannotProceed = True
                                        errorMessage = QuoteNumberDoesNotExistMessage
                                    End If
                                Else
                                    If radHOMMultiPolicyNo.Checked Then
                                        uw8.PolicyUnderwritingAnswerTypeId = "0"
                                        uw8.PolicyUnderwritingAnswer = "-1"
                                    Else
                                        uw8.PolicyUnderwritingAnswerTypeId = "0"
                                        uw8.PolicyUnderwritingAnswer = ""
                                    End If
                                End If
                                GetPolicyUws(stateQuote).Add(uw8)
                            End If

                            If stateQuote.LobType = QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal Then
                                Dim uw8 As New QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting()
                                uw8.PolicyUnderwritingCodeId = "9419"
                                If radDFRYes.Checked Then
                                    uw8.PolicyUnderwritingAnswerTypeId = "0"
                                    uw8.PolicyUnderwritingAnswer = "1"
                                    uw8.PolicyUnderwritingExtraAnswer = Me.txtMoreInfoDFR.Text.Trim().ToMaxLength(249)
                                    uw8.PolicyUnderwritingExtraAnswerTypeId = 1
                                    If String.IsNullOrWhiteSpace(txtMoreInfoDFR.Text) Then
                                        cannotProceed = True
                                    End If
                                Else
                                    If radDFRNo.Checked Then
                                        uw8.PolicyUnderwritingAnswerTypeId = "0"
                                        uw8.PolicyUnderwritingAnswer = "-1"
                                    Else
                                        uw8.PolicyUnderwritingAnswerTypeId = "0"
                                        uw8.PolicyUnderwritingAnswer = ""
                                    End If
                                End If
                                GetPolicyUws(stateQuote).Add(uw8)
                            End If

                            If stateQuote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then

                                Dim uw14 As New QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting()
                                uw14.PolicyUnderwritingCodeId = "9551"
                                If rad_far_Dog14Yes.Checked Then
                                    uw14.PolicyUnderwritingAnswerTypeId = "0"
                                    uw14.PolicyUnderwritingAnswer = "1"

                                    Dim uw14a As New QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting()
                                    uw14a.PolicyUnderwritingCodeId = "9552"
                                    uw14a.PolicyUnderwritingAnswerTypeId = "0"
                                    uw14a.PolicyUnderwritingAnswer = "1" 'always checked yes
                                    uw14a.PolicyUnderwritingExtraAnswer = Me.txtFar_How_Many_Dogs.Text.ToMaxLength(249) ' Matt A - Added max length 12-15-2015
                                    uw14a.PolicyUnderwritingExtraAnswerTypeId = 1
                                    If String.IsNullOrWhiteSpace(Me.txtFar_How_Many_Dogs.Text) Then
                                        cannotProceed = True
                                    End If

                                    Dim uw14b As New QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting()
                                    uw14b.PolicyUnderwritingCodeId = "9553"
                                    If rad_far_Dog14bYes.Checked Then
                                        uw14b.PolicyUnderwritingAnswerTypeId = "0"
                                        uw14b.PolicyUnderwritingAnswer = "1"
                                        uw14b.PolicyUnderwritingExtraAnswer = Me.txt_far_Dog14bMoreInfo.Text.ToMaxLength(249) ' Matt A - Added max length 12-15-2015
                                        uw14b.PolicyUnderwritingExtraAnswerTypeId = 1
                                        If String.IsNullOrWhiteSpace(Me.txt_far_Dog14bMoreInfo.Text) Then
                                            cannotProceed = True
                                        End If
                                    Else
                                        If rad_far_Dog14bNo.Checked Then
                                            uw14b.PolicyUnderwritingAnswerTypeId = "0"
                                            uw14b.PolicyUnderwritingAnswer = "-1"
                                        Else
                                            'neither
                                            uw14b.PolicyUnderwritingAnswerTypeId = "0"
                                            uw14b.PolicyUnderwritingAnswer = ""
                                            forceShowNoPostBack = True
                                        End If
                                    End If
                                    GetPolicyUws(stateQuote).Add(uw14a)
                                    GetPolicyUws(stateQuote).Add(uw14b)

                                Else
                                    If rad_far_Dog14No.Checked Then
                                        uw14.PolicyUnderwritingAnswerTypeId = "0"
                                        uw14.PolicyUnderwritingAnswer = "-1"
                                    Else
                                        'neither were chosen
                                        uw14.PolicyUnderwritingAnswerTypeId = "0"
                                        uw14.PolicyUnderwritingAnswer = ""
                                        forceShowNoPostBack = True
                                    End If
                                End If
                                GetPolicyUws(stateQuote).Add(uw14)
                            End If

                            ' Set BOP
                            If stateQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP Then
                                stateQuote.OccurrenceLiabilityLimitId = "56"
                                stateQuote.HasBusinessMasterEnhancement = True
                                If BopStpUwQuestionsHelper.IsBopStpUwQuestionsAvailable(stateQuote) Then
                                    Dim uwBopIncOcc As New QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting()
                                    uwBopIncOcc.PolicyUnderwritingCodeId = "9607"
                                    uwBopIncOcc.PolicyUnderwritingTabId = "8"
                                    uwBopIncOcc.PolicyUnderwritingLevelId = "1"
                                    If radBOPIncOccYes.Checked Then
                                        uwBopIncOcc.PolicyUnderwritingAnswerTypeId = "0"
                                        uwBopIncOcc.PolicyUnderwritingAnswer = "1"
                                        uwBopIncOcc.PolicyUnderwritingExtraAnswer = Me.txtIncidentalOccupancyRelated.Text.ToMaxLength(249) ' Matt A - Added max length 12-15-2015
                                        uwBopIncOcc.PolicyUnderwritingExtraAnswerTypeId = "1"
                                        If String.IsNullOrWhiteSpace(Me.txtIncidentalOccupancyRelated.Text) Then
                                            cannotProceed = True
                                        End If
                                    Else
                                        If radBOPIncOccNo.Checked Then
                                            uwBopIncOcc.PolicyUnderwritingAnswerTypeId = "0"
                                            uwBopIncOcc.PolicyUnderwritingAnswer = "-1"
                                        Else
                                            'neither
                                            uwBopIncOcc.PolicyUnderwritingAnswerTypeId = "0"
                                            uwBopIncOcc.PolicyUnderwritingAnswer = ""
                                            forceShowNoPostBack = True
                                        End If
                                    End If
                                    GetPolicyUws(stateQuote).Add(uwBopIncOcc)
                                Else
                                    GetPolicyUws(stateQuote).RemoveAll(Function(x) x.PolicyUnderwritingCodeId = "9607")
                                End If


                            End If


                        Next
                    End If

                    '8/8/2018 note: all of the below logic is at the top quote level; nothing state-specific

                    If qq.LobType = QuickQuoteObject.QuickQuoteLobType.Farm Then ' auto set farm effective dates - Matt A 7-31-2015
                        If IsDate(qq.EffectiveDate) = False Then
                            qq.EffectiveDate = Date.Today.ToShortDateString
                        End If
                    End If

                    If qq.LobType = QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal Then
                        'qq.EffectiveDate = Date.Today.ToShortDateString ' defaults the data since we will not be doing credit
                    End If

                    If qq.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability Then
                        ' CGL get EPLI by default
                        'qq.EffectiveDate = Date.Today.ToShortDateString ' defaults the data since we will not be doing credit
                        qq.Policyholder.Name.TypeId = "2" ' always a comm name
                    End If

                    ' Set defaults for BOP
                    If qq.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP Then
                        ' BOP get EPLI by default
                        'qq.EffectiveDate = Date.Today.ToShortDateString ' defaults the data since we will not be doing credit
                        qq.Policyholder.Name.TypeId = "2" ' always a comm name
                    End If

                    ' Set defaults for CAP
                    If qq.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
                        'qq.EffectiveDate = Date.Today.ToShortDateString ' defaults the data since we will not be doing credit
                        qq.Policyholder.Name.TypeId = "2" ' always a comm name
                        'qq.HasBusinessMasterEnhancement = True
                    End If

                    ' Set defaults for WMC
                    If qq.LobType = QuickQuoteObject.QuickQuoteLobType.WorkersCompensation Then
                        'qq.EffectiveDate = Date.Today.ToShortDateString ' defaults the data since we will not be doing credit
                        qq.Policyholder.Name.TypeId = "2" ' always a comm name
                    End If

                    ' Set defaults for CPR
                    If qq.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty Then
                        'qq.EffectiveDate = Date.Today.ToShortDateString ' defaults the data since we will not be doing credit
                        qq.Policyholder.Name.TypeId = "2" ' always a comm name
                    End If

                    ' Set defaults for CPP
                    If qq.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                        'qq.EffectiveDate = Date.Today.ToShortDateString ' defaults the data since we will not be doing credit
                        qq.Policyholder.Name.TypeId = "2" ' always a comm name
                    End If

                    ' Set defaults for Umbrella
                    If qq.LobType = QuickQuoteObject.QuickQuoteLobType.UmbrellaPersonal Then
                        If UmbrellaTypeFarm.Checked AndAlso UmbrellaFarmTypeCommercial.Checked Then
                            qq.Policyholder.Name.TypeId = "2" ' Comm farm
                            qq.UmbrellaSelfInsuredRetentionLimitId = "286"
                        Else
                            qq.UmbrellaSelfInsuredRetentionLimitId = "165" 'Personal farm
                        End If
                        If UmbrellaTypeFarm.Checked Then
                            If UmbrellaFarmSizeSmall.Checked Then
                                qq.FarmSizeTypeId = "1" ' Mark as Small farm
                            Else
                                qq.FarmSizeTypeId = "2" ' Mark as Large farm
                            End If
                        End If

                        'added 10/1/2021; reverting back to using governingStateQuote like all other LOBs use (backend can handle Umbrella)
                        If qq.Policyholder IsNot Nothing AndAlso qq.Policyholder.Name.TypeId = "2" Then
                            Dim govStateQuote As QuickQuoteObject = qqHelper.GoverningStateQuote(qq)
                            If govStateQuote IsNot Nothing Then
                                If govStateQuote.Applicants Is Nothing Then
                                    govStateQuote.Applicants = New List(Of QuickQuoteApplicant)
                                End If
                                If govStateQuote.Applicants.Count = 0 Then
                                    govStateQuote.Applicants.Add(New QuickQuoteApplicant)
                                End If
                            End If
                        End If
                    End If

                    If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/15/2019; original logic in ELSE
                        'note: could also check for Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage
                        'no Save needed
                        'note: could also get policyId/imageNum from qqo; may note need to reload ReadOnly
                        VR.Common.QuoteSave.QuoteSaveHelpers.ForceReadOnlyImageReloadByPolicyIdAndImageNum(Me.ReadOnlyPolicyId, Me.ReadOnlyPolicyImageNum)
                    ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                        'note: could also check for Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote
                        Dim endorsementSaveError As String = ""
                        Dim successfulEndorsementSave As Boolean = VR.Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedEndorsementQuote(Me.Quote, errorMessage:=endorsementSaveError)
                        'note: could also get policyId/imageNum from qqo
                        VR.Common.QuoteSave.QuoteSaveHelpers.ForceEndorsementReloadByPolicyIdAndImageNum(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum)
                    Else
                        'If IFM.VR.Common.Helpers.NewCompanyIdHelper.IsNewCompanyIdAvailable(qq) Then
                        '    qq.Company = QuickQuoteCompany.IndianaFarmersIndemnity
                        'Else
                        '    qq.Company = QuickQuoteCompany.IndianaFarmersMutual
                        'End If
                        VR.Common.QuoteSave.QuoteSaveHelpers.SaveQuote(qqID, qq, Nothing)

                        ' wipe session
                        VR.Common.QuoteSave.QuoteSaveHelpers.ForceQuoteReloadById(Me.QuoteId) ' Matt A - 12-9-14
                    End If

                    If forceShowNoPostBack OrElse cannotProceed Then
                        ShowPopup()
                        If errorMessage.NoneAreNullEmptyorWhitespace() Then
                            _script.AddScriptLine("alert('" & errorMessage & "');")
                        End If
                    Else
                        If Me.isNewQuote Then
                            DirectCast(Me.Page.Master, VelociRater).AddQuoteStartedThisSession(qq.Database_QuoteId)
                        End If

                        RedirectToEditPages(qq.LobId, qqID)
                    End If
                End If
            Else
                ShowPopup()
                If errorMessage.NoneAreNullEmptyorWhitespace() Then
                    _script.AddScriptLine("alert('" & errorMessage & "');")
                End If

            End If
        Else
            _script.AddScriptLine("alert('Choose an agency before creating a new quote.');")
        End If

    End Sub

    Public Sub ShowPopup()
        Dim _script = DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager
        If DirectCast(Me.Page.Master, VelociRater).AgencyID > 0 Then
            _script.AddScriptLine("ShowUwQuestions();")
            If Me.NewQuoteRequestLOBType = QuickQuoteObject.QuickQuoteLobType.UmbrellaPersonal Then
                _script.AddScriptLine("$( '#" + divUwQuestions.ClientID + "' ).dialog('option', 'title', 'Umbrella');")
            Else
                _script.AddScriptLine("$( '#" + divUwQuestions.ClientID + "' ).dialog('option', 'title', 'Underwriting Questions');")
            End If

            RaiseEvent ToggleUWPopupShown()
        Else
            _script.AddScriptLine("alert('Choose an agency before creating a new quote.');")
        End If

        hdnRAPANewSymbolsEnabled.Value = "False"
        hdnIsNewQuote.Value = isNewQuote
        Select Case NewQuoteRequestLOBType
            Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                'Eligibility for date crossing message involving 3 new RAPA symbols
                If NewRAPASymbolsHelper.NewRAPASymbolsEnabled() Then
                    hdnRAPANewSymbolsEnabled.Value = "True"
                End If
                If isNewQuote = "False" Then
                    hdnCopyQuoteEffectiveDate.Value = Quote.EffectiveDate
                End If
            Case QuickQuoteObject.QuickQuoteLobType.HomePersonal
                VRScript.AddScriptLine("$(""#" + Me.txtMoreInfoRelatedAuto.ClientID + """).watermark(""Enter the Indiana Farmers personal auto quote number (QPPA1234567) or policy number (PPA1234567)."");")
            Case QuickQuoteObject.QuickQuoteLobType.Farm
                If Quote IsNot Nothing AndAlso CosDamHiddenHelper.CosmeticDamageHiddenEnabled() AndAlso IsDate(Quote.EffectiveDate) AndAlso IsDate(EffectiveDate) Then
                    Dim OrigDate As Date = CDate(Quote.EffectiveDate)
                    Dim NewDate As Date = CDate(EffectiveDate)
                    Dim CosmeticDamageHiddenDateLine As Date = CosDamHiddenHelper.CosmeticDamageHiddenEffDate '3/1/2025 date line
                    If OrigDate < CosmeticDamageHiddenDateLine AndAlso NewDate >= CosmeticDamageHiddenDateLine Then
                        'Date going forward, crossing date line - OrigDate prior to 3/1/2025, NewDate 3/1/2025 or later
                        Dim NeedsWarningMessage As Boolean = CosDamHiddenHelper.RemoveCosmeticDamageForBuildingTypes(Quote)
                        If NeedsWarningMessage Then
                            _script.AddScriptLine("alert('Effective 03/01/2025 Cosmetic Damage Exclusion is no longer available for the following building types: Grain Legs, Grain Dryer, Well Pumps, Tanks, Private Power & Light Poles, Radio & Television Equipment, Windmill & Chargers and Outbuilding with Living Quarters. Coverage has been removed.');")
                        End If
                    End If
                End If
        End Select

        If qqHelper.IsDateString(Me.txtUWQuestionsEffectiveDate.Text) OrElse (Quote IsNot Nothing AndAlso qqHelper.IsDateString(Quote.EffectiveDate) AndAlso Not isCopiedQuote) Then
            txtUWQuestionsEffectiveDate.Style.Remove("border-color")
            ToggleFieldsEnabled(True)
        Else
            ToggleFieldsEnabled(False)
        End If

    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        ' do nothing
    End Sub

    Private Sub Repeater1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles Repeater1.ItemDataBound
        Dim _script = DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager

        Dim radYes As RadioButton = e.Item.FindControl("radYes")
        Dim radNo As RadioButton = e.Item.FindControl("radNo")
        Dim txtMoreInfo As TextBox = e.Item.FindControl("txtMoreInfo")
        Dim question As VR.Common.UWQuestions.VRUWQuestion = e.Item.DataItem

        radYes.GroupName = "Grp" + question.QuestionNumber.ToString()
        radNo.GroupName = "Grp" + question.QuestionNumber.ToString()

        _script.AddVariableLine("var " + radYes.GroupName + " = false;")
        If question.IsTrueKillQuestion Then
            ' Updated radYes script to uncheck the YES radiobutton if confirmation was cancelled on true kill question MGB 3-21-2017
            'radYes.Attributes.Add("onclick", "if (confirm('" + inEligibleMsg + "')){HideUwQuestions(); window.location = '" + System.Configuration.ConfigurationManager.AppSettings("QuickQuote_Personal_HomePage") + "';}")
            radYes.Attributes.Add("onclick", "if (confirm('" + inEligibleMsg + "')){HideUwQuestions(); window.location = '" + System.Configuration.ConfigurationManager.AppSettings("QuickQuote_Personal_HomePage") + "';} else {var x = document.getElementById('" & radYes.ClientID & "'); if (x != null){x.checked = false;} else{alert('rbyes NOT found'); } }")
            radNo.Attributes.Add("onclick", "$(this).parent().parent().next().hide();")
        Else
            If question.PolicyUnderwritingCodeId = "9419" Then
                'special logic for Question #5 of DFR

                'Updated 7/26/2022 for task 75803 MLW        
                If IFM.VR.Common.Helpers.DFR.DFRStandaloneHelper.isDFRStandaloneAvailable(Quote) Then
                    radYes.Attributes.Add("alwaysRequiresAdditionalAnswer", "true")
                    radYes.Attributes.Add("onclick", "$(this).parent().parent().parent().next().show();")
                    radNo.Attributes.Add("onclick", "$(this).parent().parent().next().hide();")
                Else
                    radYes.Attributes.Add("alwaysRequiresAdditionalAnswer", "true")
                    radYes.Attributes.Add("onclick", "$(this).parent().parent().parent().next().show();")
                    radNo.Attributes.Add("onclick", "$(this).parent().parent().next().show();alert('" + Me.ValMsg + "');")
                End If
                'radYes.Attributes.Add("alwaysRequiresAdditionalAnswer", "true")
                'radYes.Attributes.Add("onclick", "$(this).parent().parent().parent().next().show();")
                'radNo.Attributes.Add("onclick", "$(this).parent().parent().next().show();alert('" + Me.ValMsg + "');")
            Else
                Dim yesScript As String = "$(this).parent().parent().next().show();$(this).parent().parent().parent().parent().parent().height($(this).parent().parent().parent().parent().parent().height() + 55);"
                _script.AddScriptLine("$(""#" + radYes.ClientID + """).change(function(){" + yesScript + " " + radYes.GroupName + " = true;});")

                If Request.QueryString("newQuote") IsNot Nothing Then
                    Dim lobID As Int32 = -1
                    If Int32.TryParse(Request.QueryString("newQuote"), lobID) Then
                        If lobID <> "17" Then
                            radYes.Attributes.Add("onclick", "alert('" + Me.ValMsg + "');")
                        End If
                    End If
                Else
                    If Me.Quote IsNot Nothing AndAlso Me.Quote.LobType <> QuickQuoteObject.QuickQuoteLobType.Farm Then
                        radYes.Attributes.Add("onclick", "alert('" + Me.ValMsg + "');")
                    End If
                End If

                Dim noScript As String = "$(this).parent().parent().parent().parent().parent().height($(this).parent().parent().parent().parent().parent().height() - 55);"
                _script.AddScriptLine("$(""#" + radNo.ClientID + """).change(function(){if (" + radYes.GroupName + "){" + noScript + "}else{" + radYes.GroupName + " = true;}});")
                radNo.Attributes.Add("onclick", "$(this).parent().parent().next().hide();")
            End If

        End If

    End Sub

    Private Sub SetPolicyUws(q As QuickQuote.CommonObjects.QuickQuoteObject, list As List(Of QuickQuotePolicyUnderwriting))
        Select Case NewQuoteRequestLOBID
            Case 1, 17, 9, 25, 20, 21, 28, 23
                q.PolicyUnderwritings = list
            Case 2, 3
                q.Locations(0).PolicyUnderwritings = list
        End Select
    End Sub

    Private Function GetPolicyUws(q As QuickQuote.CommonObjects.QuickQuoteObject) As List(Of QuickQuotePolicyUnderwriting)
        Dim uws As List(Of QuickQuotePolicyUnderwriting) = Nothing
        Select Case NewQuoteRequestLOBID
            Case 1, 17, 9, 25, 20, 21, 28, 23
                'uws = q.PolicyUnderwritings
                'updated 8/17/2018
                Dim subQuotes As List(Of QuickQuoteObject) = qqHelper.MultiStateQuickQuoteObjects(q)
                If subQuotes.IsLoaded = True Then
                    uws = subQuotes.Item(0).PolicyUnderwritings
                End If
            Case 2, 3
                If q.Locations IsNot Nothing AndAlso q.Locations.Any() Then
                    uws = q.Locations(0).PolicyUnderwritings
                End If
        End Select
        Return uws
    End Function

    Private Function GetPolicyUwsIgnoreLOB() As Boolean
        Select Case Quote.LobType
            Case QuickQuoteObject.QuickQuoteLobType.UmbrellaPersonal
                Return True
            Case Else
                Return False
        End Select
    End Function

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    'Added 09/02/2021 for bug 51550 MLW
    Private Sub radBOPCondoDandOYes_PreRender(sender As Object, e As EventArgs) Handles radBOPCondoDandOYes.PreRender
        radBOPCondoDandOYes.Attributes.Add("onclick", "alert('Condominium Directors & Officers coverage is only available with our CPP product. Responding yes to this question continues this quote as a CPP including Condominium Directors & Officers Coverage. Selecting no will continue quoting the BOP line of business without the Directors & Officers coverage.');")
    End Sub

    Private Sub radBOPOver35kYes_PreRender(sender As Object, e As EventArgs) Handles radBOPOver35kYes.PreRender
        radBOPOver35kYes.Attributes.Add("onclick", "alert('Buildings over 35,000 square feet are only available with our CPP product. Responding yes to this question continues this quote as a CPP. Selecting no will continue quoting the BOP line of business.');")
    End Sub

    Private Sub radBOPOver3StoriesYes_PreRender(sender As Object, e As EventArgs) Handles radBOPOver3StoriesYes.PreRender
        radBOPOver3StoriesYes.Attributes.Add("onclick", "alert('Buildings with more than 3 stories are only available with our CPP product. Responding yes to this question continues this quote as a CPP. Selecting no will continue quoting the BOP line of business.');")
    End Sub
    Private Sub radBOPSales6MYes_PreRender(sender As Object, e As EventArgs) Handles radBOPSales6MYes.PreRender
        radBOPSales6MYes.Attributes.Add("onclick", "alert('Gross sales of $6,000,000 or more are only available with our CPP product. Responding yes to this question continues this quote as a CPP. Selecting no will continue quoting the BOP line of business.');")
    End Sub

    ''' <summary>
    ''' CD-7789: Save Kill Question answers back To the questions list before Data Bind. - lSchwieterman - 8/11/2025
    ''' </summary>
    ''' <param name="questions"></param>
    Private Sub SetKillQuestionAnswers(questions As List(Of VRUWQuestion))
        If Repeater1.Items Is Nothing OrElse Repeater1.Items.Count = 0 OrElse questions Is Nothing OrElse questions.Count = 0 Then Exit Sub

        For Each item As RepeaterItem In Repeater1.Items
            Dim hdnCodeId = TryCast(item.FindControl("hdnPolicyUnderwritingCodeId"), HiddenField)
            If hdnCodeId Is Nothing Then Continue For

            Dim question = questions.FirstOrDefault(Function(q) q.PolicyUnderwritingCodeId = hdnCodeId.Value)
            If question Is Nothing Then Continue For

            Dim radYes = TryCast(item.FindControl("radYes"), RadioButton)
            Dim radNo = TryCast(item.FindControl("radNo"), RadioButton)
            Dim txtMoreInfo = TryCast(item.FindControl("txtMoreInfo"), TextBox)

            If radYes IsNot Nothing AndAlso radYes.Checked Then
                question.QuestionAnswerYes = True
                question.QuestionAnswerNo = False
                If txtMoreInfo IsNot Nothing Then
                    question.PolicyUnderwritingExtraAnswer = txtMoreInfo.Text
                End If
            ElseIf radNo IsNot Nothing AndAlso radNo.Checked Then
                question.QuestionAnswerYes = False
                question.QuestionAnswerNo = True
            Else
                question.QuestionAnswerYes = False
                question.QuestionAnswerNo = False
            End If
        Next
    End Sub

    ''' <summary>
    ''' CD-7789 - Recursively toggle the enabled state of all input fields within a control hierarchy, excluding the effective date field and cancel button. - lSchwieterman - 8/11/2025
    ''' </summary>
    ''' <param name="parent"></param>
    ''' <param name="enabled"></param>
    Private Sub ToggleFieldsEnabledRecursive(parent As Control, enabled As Boolean)
        For Each ctrl As Control In parent.Controls
            ' Skip the effective date field
            If ctrl.ID IsNot Nothing AndAlso {btnCancel.ID, txtUWQuestionsEffectiveDate.ID}.Contains(ctrl.ID) Then
                Continue For
            End If

            ' Enable/disable input controls
            If TypeOf ctrl Is WebControl Then
                DirectCast(ctrl, WebControl).Enabled = enabled
            End If

            ' Recurse into child controls
            If ctrl.HasControls() Then
                ToggleFieldsEnabledRecursive(ctrl, enabled)
            End If
        Next
    End Sub

    ''' <summary>
    ''' CD-7789 - Toggles the enabled state of all input fields within the main divUwQuestions control. - lSchwieterman - 8/11/2025
    ''' </summary>
    ''' <param 
    ''' </summary>
    ''' <param name="enabled"></param>
    Private Sub ToggleFieldsEnabled(enabled As Boolean)
        ToggleFieldsEnabledRecursive(divUwQuestions, enabled)
    End Sub

    ''' <summary>
    ''' CD-7936 - Checks to see if the user entered a valid quote or policy number for the related auto question. - lSchwieterman - 9/02/2025
    ''' </summary>
    ''' <param name="quoteOrPolicyNumber"></param>
    ''' <returns>True if the quote or policy number exists.</returns>
    Private Function EnteredValidQuoteOrPolicyNumber(quoteOrPolicyNumber As String, lobId As Integer) As Boolean
        Dim policyLookupQuery As New QuickQuotePolicyLookupInfo
        With policyLookupQuery
            .PolicyLookupType = QuickQuotePolicyLookupInfo.LookupType.ByPolicy
            .PolicyNumber = quoteOrPolicyNumber
            .AgencyIds = DiamondAgencyIds()
            .LobId = lobId
        End With
        Return PolicyResultForLookupInfo(policyLookupQuery) IsNot Nothing
    End Function
End Class
