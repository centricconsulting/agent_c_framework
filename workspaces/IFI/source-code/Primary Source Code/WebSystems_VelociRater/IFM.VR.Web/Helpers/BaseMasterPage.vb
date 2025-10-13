Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods

Public MustInherit Class BaseMasterPage
    Inherits System.Web.UI.MasterPage
    Dim qqHelper As New QuickQuoteHelperClass()

    Public ReadOnly Property AssemblyLastUpdatedDate As String
        Get
            Return System.IO.File.GetLastWriteTime(System.Reflection.Assembly.GetExecutingAssembly.Location).ToString()
        End Get
    End Property

    Public ReadOnly Property IsInTestEnvironment As Boolean
        Get
            Return System.Configuration.ConfigurationManager.AppSettings("TestOrProd") IsNot Nothing AndAlso System.Configuration.ConfigurationManager.AppSettings("TestOrProd").ToLower() = "test"
        End Get
    End Property

    Public ReadOnly Property ScriptDT As String
        Get
            'Return System.Configuration.ConfigurationManager.AppSettings("scriptDating_VrPers")
            Return System.IO.File.GetLastWriteTime(System.Reflection.Assembly.GetExecutingAssembly.Location).ToShortDateString().Replace("/", "-") + "_" + System.Configuration.ConfigurationManager.AppSettings("scriptDating_VrPers")
        End Get
    End Property

    Public ReadOnly Property PageName As String
        Get
            Return System.Web.VirtualPathUtility.GetFileName(Request.ServerVariables("SCRIPT_NAME")).Split("."c)(0)
        End Get
    End Property

    Private _hasIsStaffBeenSet As Boolean = False
    Private _isStaff As Boolean
    Public ReadOnly Property IsStaff As Boolean
        Get
            If _hasIsStaffBeenSet = False Then
                _isStaff = qqHelper.IsHomeOfficeStaffUser()
                _hasIsStaffBeenSet = True
            End If

            Return _isStaff
        End Get
    End Property

    Public ReadOnly Property IsOnAppPage As Boolean
        Get
            If Me.Page.GetType() IsNot Nothing AndAlso UCase(Me.Page.GetType().ToString).Contains("APP") = True Then
                Return True
            End If
            Return False
        End Get
    End Property

    Public ReadOnly Property Quote As QuickQuote.CommonObjects.QuickQuoteObject
        Get
            If String.IsNullOrWhiteSpace(ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/14/2019; original logic in ELSE
                Dim readOnlyLoadError As String = ""
                Return VR.Common.QuoteSave.QuoteSaveHelpers.GetReadOnlyImageFromAnywhere(ReadOnlyPolicyId, ReadOnlyPolicyImageNum, saveTypeView:=If(IsOnAppPage = True, QuickQuoteXML.QuickQuoteSaveType.AppGap, QuickQuoteXML.QuickQuoteSaveType.Quote), errorMessage:=readOnlyLoadError)
            ElseIf String.IsNullOrWhiteSpace(EndorsementPolicyIdAndImageNum) = False Then
                Dim endorsementLoadError As String = ""
                Return VR.Common.QuoteSave.QuoteSaveHelpers.GetEndorsementQuoteFromAnywhere(EndorsementPolicyId, EndorsementPolicyImageNum, saveTypeView:=If(IsOnAppPage = True, QuickQuoteXML.QuickQuoteSaveType.AppGap, QuickQuoteXML.QuickQuoteSaveType.Quote), errorMessage:=endorsementLoadError)
            Else
                Dim errCreateQSO As String = ""
                If IsOnAppPage Then
                    Return VR.Common.QuoteSave.QuoteSaveHelpers.GetQuoteById(QuickQuoteObject.QuickQuoteLobType.None, QuoteId, errCreateQSO, True, QuickQuoteXML.QuickQuoteSaveType.AppGap)
                End If
                Return VR.Common.QuoteSave.QuoteSaveHelpers.GetQuoteById(QuickQuoteObject.QuickQuoteLobType.None, QuoteId, errCreateQSO, True, QuickQuoteXML.QuickQuoteSaveType.Quote)
            End If
        End Get
    End Property

    Public ReadOnly Property QuoteId As String
        Get
            'a universal place to get this - not all pages use the quote id but most do
            If Request.QueryString("quoteid") IsNot Nothing Then
                Return Request.QueryString("quoteid")
            End If
            If Page.RouteData.Values("quoteid") IsNot Nothing Then
                Return Page.RouteData.Values("quoteid").ToString()
            End If
            Return ""
        End Get
    End Property

    'added 2/14/2019
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

    '6-3-14 - Matt
    Dim _ratedQuote As QuickQuoteObject = Nothing
    Dim _attemptedRatedQuoteFetch As Boolean = False
    Public Function GetRatedQuotefromCache(Optional ForceRefresh As Boolean = False, Optional ratedQuote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing, Optional ByRef loadErrorMsg As String = "") As QuickQuote.CommonObjects.QuickQuoteObject 'updated 10/3/2019 (12/11/2019 in Sprint 10) w/ optional byref param for loadErrorMsg
        Dim errCreateQSO As String = ""
        If ratedQuote IsNot Nothing Then
            _ratedQuote = ratedQuote
        Else
            If (_ratedQuote Is Nothing And _attemptedRatedQuoteFetch = False) Or ForceRefresh Then
                _attemptedRatedQuoteFetch = True
                If String.IsNullOrWhiteSpace(ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/14/2019; original logic in ELSE
                    _ratedQuote = VR.Common.QuoteSave.QuoteSaveHelpers.GetReadOnlyQuickQuoteObjectForPolicyIdAndImageNum(ReadOnlyPolicyId, ReadOnlyPolicyImageNum, saveTypeView:=If(IsOnAppPage = True, QuickQuoteXML.QuickQuoteSaveType.AppGap, QuickQuoteXML.QuickQuoteSaveType.Quote), ratedView:=True, errorMessage:=errCreateQSO)
                ElseIf String.IsNullOrWhiteSpace(EndorsementPolicyIdAndImageNum) = False Then
                    _ratedQuote = VR.Common.QuoteSave.QuoteSaveHelpers.GetEndorsementQuoteForPolicyIdAndImageNum(EndorsementPolicyId, EndorsementPolicyImageNum, saveTypeView:=If(IsOnAppPage = True, QuickQuoteXML.QuickQuoteSaveType.AppGap, QuickQuoteXML.QuickQuoteSaveType.Quote), ratedView:=True, errorMessage:=errCreateQSO)
                Else
                    If IsOnAppPage Then
                        _ratedQuote = Common.QuoteSave.QuoteSaveHelpers.GetRatedQuoteById_NOSESSION(Me.QuoteId, errCreateQSO, QuickQuoteObject.QuickQuoteLobType.None, QuickQuoteXML.QuickQuoteSaveType.AppGap)
                    Else
                        _ratedQuote = Common.QuoteSave.QuoteSaveHelpers.GetRatedQuoteById_NOSESSION(Me.QuoteId, errCreateQSO, QuickQuoteObject.QuickQuoteLobType.None)
                    End If
                End If
            End If
        End If
        loadErrorMsg = errCreateQSO 'added 10/3/2019 (12/11/2019 in Sprint 10)
        Return _ratedQuote
    End Function

    Private Sub BaseMasterPage_Load(sender As Object, e As EventArgs) Handles Me.Load
#If DEBUG Then
        If ConfigurationManager.AppSettings("QuickQuote_UseTestVariables") = "Yes" Then
            Session("DiamondUsername") = ConfigurationManager.AppSettings("QuickQuoteTestUsername")
            Session("DiamondUserId") = ConfigurationManager.AppSettings("QuickQuoteTestUserId")
        End If
#End If
    End Sub
End Class