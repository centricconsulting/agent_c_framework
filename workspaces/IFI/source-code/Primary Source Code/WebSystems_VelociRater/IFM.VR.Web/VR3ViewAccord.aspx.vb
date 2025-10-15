Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods

Public Class VR3ViewAccord
    Inherits System.Web.UI.Page

    Dim _quote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing
    Protected ReadOnly Property Quote As QuickQuote.CommonObjects.QuickQuoteObject
        Get
            If _quote Is Nothing Then
                Dim errCreateQSO As String = ""
                If String.IsNullOrWhiteSpace(ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/18/2019; original logic in ELSE
                    _quote = VR.Common.QuoteSave.QuoteSaveHelpers.GetReadOnlyQuickQuoteObjectForPolicyIdAndImageNum(Me.ReadOnlyPolicyId, Me.ReadOnlyPolicyImageNum, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.AppGap, ratedView:=True, errorMessage:=errCreateQSO)
                ElseIf String.IsNullOrWhiteSpace(EndorsementPolicyIdAndImageNum) = False Then
                    _quote = VR.Common.QuoteSave.QuoteSaveHelpers.GetEndorsementQuoteForPolicyIdAndImageNum(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.AppGap, ratedView:=True, errorMessage:=errCreateQSO)
                Else
                    _quote = Common.QuoteSave.QuoteSaveHelpers.GetRatedQuoteById_NOSESSION(Me.QuoteId, errCreateQSO, QuickQuoteObject.QuickQuoteLobType.None, QuickQuoteXML.QuickQuoteSaveType.AppGap)
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

    'added 2/18/2019
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

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.Clear()
        If Me.Quote IsNot Nothing Then
            If IsNumeric(Me.Quote.PolicyId) Then
                Dim loginName As String = ""
                Dim loginPassword As String = ""

                If System.Web.HttpContext.Current.Session("DiamondUsername") IsNot Nothing AndAlso System.Web.HttpContext.Current.Session("DiamondUsername").ToString <> "" Then
                    loginName = System.Web.HttpContext.Current.Session("DiamondUsername").ToString
                ElseIf ConfigurationManager.AppSettings("TestOrProd") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("TestOrProd").ToString) = "TEST" AndAlso ConfigurationManager.AppSettings("QuickQuote_UseTestVariables") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_UseTestVariables").ToString) = "YES" Then
                    loginName = ConfigurationManager.AppSettings("QuickQuoteTestUsername").ToString
                End If
                If System.Web.HttpContext.Current.Session("DiamondPassword") IsNot Nothing AndAlso System.Web.HttpContext.Current.Session("DiamondPassword").ToString <> "" Then
                    loginPassword = System.Web.HttpContext.Current.Session("DiamondPassword").ToString
                ElseIf ConfigurationManager.AppSettings("TestOrProd") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("TestOrProd").ToString) = "TEST" AndAlso ConfigurationManager.AppSettings("QuickQuote_UseTestVariables") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_UseTestVariables").ToString) = "YES" Then
                    loginPassword = ConfigurationManager.AppSettings("QuickQuoteTestPassword").ToString
                End If

                Dim bytes As Byte() = Common.PrintHistories.PrintHistories.GetPrintHistories(Me.Quote.PolicyId,
                                                                                             loginName,
                                                                                             loginPassword,
                                                                                             Common.PrintHistories.PrintHistories.PrintType.JustApplication,
                                                                                             "",
                                                                                             Common.PrintHistories.PrintHistories.PrintFormDescriptionEvaluationType.OnlyUniqueFormDescriptions)
                If bytes IsNot Nothing Then

                    Response.ContentType = "application/pdf"
                    'Response.AddHeader("content-disposition", "attachment; filename=" + String.Format("ACCORD{0}.pdf", Me.Quote.QuoteNumber))
                    Response.BinaryWrite(bytes)
                Else
                    Response.Write("No document located or access is denied.")
                End If
            Else
                Response.Write("No document located or access is denied.")

            End If
        Else
            Response.Write("No document located or access is denied.")
        End If
        Response.Flush()
    End Sub

End Class