Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports System.Configuration.ConfigurationManager
Imports Diamond.Business.ThirdParty.DocuSignAPI
Imports IFM.VR.Common.Helpers
Imports System.Windows.Interop
Imports System.Data.SqlClient

Public Class ctl_Comm_EmailUW
    Inherits System.Web.UI.UserControl

#Region "Declarations"
    Private Shared chc As New CommonHelperClass

    Dim _quote As QuickQuoteObject
    Protected ReadOnly Property Quote As QuickQuote.CommonObjects.QuickQuoteObject
        Get
            Dim errCreateQSO As String = ""
            If _quote Is Nothing Then
                If String.IsNullOrWhiteSpace(ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/18/2019; original logic in ELSE
                    _quote = VR.Common.QuoteSave.QuoteSaveHelpers.GetReadOnlyQuickQuoteObjectForPolicyIdAndImageNum(Me.ReadOnlyPolicyId, Me.ReadOnlyPolicyImageNum, errorMessage:=errCreateQSO)
                ElseIf String.IsNullOrWhiteSpace(EndorsementPolicyIdAndImageNum) = False Then
                    _quote = VR.Common.QuoteSave.QuoteSaveHelpers.GetEndorsementQuoteForPolicyIdAndImageNum(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum, errorMessage:=errCreateQSO)
                Else
                    _quote = VR.Common.QuoteSave.QuoteSaveHelpers.GetQuoteById_NOSESSION(Me.QuoteId)
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
    Public ReadOnly Property QuoteIdOrPolicyIdPipeImageNumber As String
        Get
            If String.IsNullOrWhiteSpace(ReadOnlyPolicyIdAndImageNum) = False Then
                Return ReadOnlyPolicyIdAndImageNum
            ElseIf String.IsNullOrWhiteSpace(EndorsementPolicyIdAndImageNum) = False Then
                Return EndorsementPolicyIdAndImageNum
            Else
                Return QuoteId
            End If
        End Get
    End Property

    'added 2/19/2019
    Dim _RatedQuote As QuickQuoteObject = Nothing
    Protected ReadOnly Property RatedQuote As QuickQuote.CommonObjects.QuickQuoteObject
        Get
            If _RatedQuote Is Nothing Then
                Dim errCreateQSO As String = ""
                If String.IsNullOrWhiteSpace(ReadOnlyPolicyIdAndImageNum) = False Then
                    _RatedQuote = VR.Common.QuoteSave.QuoteSaveHelpers.GetReadOnlyQuickQuoteObjectForPolicyIdAndImageNum(Me.ReadOnlyPolicyId, Me.ReadOnlyPolicyImageNum, ratedView:=True, errorMessage:=errCreateQSO)
                ElseIf String.IsNullOrWhiteSpace(EndorsementPolicyIdAndImageNum) = False Then
                    _RatedQuote = VR.Common.QuoteSave.QuoteSaveHelpers.GetEndorsementQuoteForPolicyIdAndImageNum(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum, ratedView:=True, errorMessage:=errCreateQSO)
                Else
                    _RatedQuote = VR.Common.QuoteSave.QuoteSaveHelpers.GetRatedQuoteById_NOSESSION(Me.QuoteId, errCreateQSO, QuickQuoteObject.QuickQuoteLobType.None, Nothing)
                End If
            End If
            Return _RatedQuote
        End Get
    End Property
    Public ReadOnly Property IsOnAppPage As Boolean 'added 12/16/2022
        Get
            If Me.Page.GetType() IsNot Nothing AndAlso UCase(Me.Page.GetType().ToString).Contains("APP") = True Then
                Return True
            End If
            Return False
        End Get
    End Property
    Public ReadOnly Property QuoteSummaryActionsValidationHelper As ControlValidationHelper
        Get
            Dim cvh As ControlValidationHelper = Nothing
            If TypeOf Me.Page Is BasePage Then
                cvh = CType(Me.Page, BasePage).QuoteSummaryActionsValidationHelper
            End If
            Return cvh
        End Get
    End Property

#End Region

#Region "Methods and Functions"

    Private Sub HandleError(ByVal RoutineName As String, ByVal ex As Exception)
        If AppSettings("TestOrProd").ToUpper = "TEST" Then
            lblMsg.Text = ex.Message
        Else
            Throw New Exception(ex.Message, ex)
        End If
    End Sub

    Public Function SendUnderwritingEmail(ByRef err As String) As Boolean
        'Dim strBody As String = Nothing
        'Try
        '    Using objMail As New EmailObject()
        '        ' FROM address
        '        objMail.EmailFromAddress = "Velocirater@IndianaFarmers.com"
        '        'objMail.EmailToAddress = "mbutler@indianafarmers.com"
        '        objMail.EmailToAddress = AppSettings("BOP_UnderwritingEmailAddresses")
        '        objMail.EmailSubject = Quote.PolicyNumber & " - Referred to UW from Velocirater Quote Summary"

        '        strBody = "<html><body><table>"
        '        strBody += "<tr><td>Policy Number: " & Quote.PolicyNumber & "</td></tr>"
        '        strBody += "<tr><td>Name: " & txtName.Text & "</td></tr>"
        '        strBody += "<tr><td>Phone Number: " & TxtPhoneNumber.Text & "</td></tr>"
        '        strBody += "<tr><td>Email: " & txtEmail.Text & "</td></tr>"
        '        strBody += "<tr><td>Message entered by agent: " & txtUserMessage.Text & "</td></tr>"
        '        strBody += "<tr><td>"
        '        Dim msgs As List(Of String) = GetRatingMessages()
        '        If msgs IsNot Nothing Then
        '            Dim i As Integer = 0
        '            strBody += "<tr><td>&nbsp;</td></tr>"
        '            strBody += "<b><u>Rating Messages:</u></b> <br />"
        '            For Each msg As String In msgs
        '                i += 1
        '                strBody += "<b>Message " & i.ToString & ":</b><br />"
        '                strBody += msg
        '                strBody += "<tr><td>&nbsp;</td></tr>"
        '            Next
        '        Else
        '            strBody += "-- No Rating Messages -- "
        '        End If

        '        strBody += "</td></tr>"
        '        strBody += "</table></body></html>"

        '        ' Set the body of the email
        '        objMail.EmailBody = strBody

        '        objMail.MailHost = System.Configuration.ConfigurationManager.AppSettings("mailhost")

        '        objMail.SendEmail()

        '        If objMail.hasError Then Throw New Exception("Email transmission to " & objMail.EmailToAddress & " failed: " & objMail.errorMsg)

        '    End Using

        '    Return True
        'Catch ex As Exception
        '    err = ex.Message
        '    HandleError("SendMail", ex)
        '    Return False
        'End Try
        'updated 12/16/2022 to call new method
        Return SendUnderwritingEmail_OptionallyAppendNoteToUW(err)
    End Function
    Public Function SendUnderwritingEmail_OptionallyAppendNoteToUW(ByRef err As String, Optional ByVal noteForUW As String = "") As Boolean 'added 12/16/2022
        Dim strBody As String = Nothing
        Dim qqHelper As New QuickQuoteHelperClass
        Try
            Using objMail As New EmailObject()
                ' FROM address
                objMail.EmailFromAddress = "Velocirater@IndianaFarmers.com"
                'objMail.EmailToAddress = "mbutler@indianafarmers.com"
                objMail.EmailToAddress = AppSettings("BOP_UnderwritingEmailAddresses")
                objMail.EmailSubject = Quote.PolicyNumber & " - Referred to UW from Velocirater Quote Summary"

                strBody = "<html><body><table>"
                strBody += "<tr><td><b>Policy Number:</b> " & Quote.PolicyNumber & "</td></tr>"
                strBody += "<tr><td><b>Name:</b> " & txtName.Text & "</td></tr>"
                strBody += "<tr><td><b>Phone Number:</b> " & TxtPhoneNumber.Text & "</td></tr>"
                strBody += "<tr><td><b>Email:</b> " & txtEmail.Text & "</td></tr>"
                Dim strUnderwriterName = GetCommUnderwriterNameByAgency(Quote)
                strBody += "<tr><td><b>Underwriter:</b> " & strUnderwriterName & "</td></tr>"
                strBody += "<tr><td><b>Message entered by agent:</b> " & txtUserMessage.Text & "</td></tr>"
                If Me.Quote IsNot Nothing AndAlso qqHelper.IsPositiveIntegerString(Me.Quote.PolicyId) AndAlso ((Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote AndAlso QuickQuoteHelperClass.AllowVRToUpdateFromDiamond_Interoperability(Me.Quote.LobType) = True) OrElse Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote) Then
                    'check for attachments then append NoteForUW 
                    'IFM.VR.Common.Helpers.FileUploadHelper.SearchForQuoteFiles(context.Request.QueryString("agencyId"), context.Request.QueryString("quoteId")
                    If IFM.VR.Common.Helpers.FileUploadHelper.SearchForQuoteFiles(Me.Quote.AgencyId, Me.QuoteIdOrPolicyIdPipeImageNumber).Any Then
                        Dim UwText = "There are attachments for review."
                        Dim splitterTxt As String = "<br />"
                        noteForUW = qqHelper.appendText(noteForUW, UwText, splitter:=splitterTxt)
                    End If
                End If
                If String.IsNullOrWhiteSpace(noteForUW) = False Then
                    strBody += "<tr><td><b><u>Note to UW:</u></b> " & noteForUW & "</td></tr>"
                End If
                strBody += "<tr><td>&nbsp;</td></tr>"
                strBody += "<tr><td>"
                Dim msgs As List(Of String) = GetRatingMessages()
                If msgs IsNot Nothing AndAlso msgs.Count > 0 Then
                    Dim i As Integer = 0
                    strBody += "<table>"
                    strBody += "<tr><td><b><u>Rating Messages:</u></b></td></tr>"
                    For Each msg As String In msgs
                        i += 1
                        strBody += "<tr><td><b>Message " & i.ToString & ":</b><br />" & msg & "</td></tr>"
                        strBody += "<tr><td>&nbsp;</td></tr>"
                    Next
                    strBody += "</table>"
                Else
                    Dim valErrMsgs As List(Of String) = GetValidationErrorMessages()
                    If valErrMsgs IsNot Nothing AndAlso valErrMsgs.Count > 0 Then
                        Dim i As Integer = 0
                        strBody += "<table>"
                        strBody += "<tr><td><b><u>Validation Error Messages:</u></b></td></tr>"
                        For Each em As String In valErrMsgs
                            i += 1
                            strBody += "<tr><td><b>Message " & i.ToString & ":</b><br />" & em & "</td></tr>"
                            strBody += "<tr><td>&nbsp;</td></tr>"
                        Next
                        strBody += "</table>"
                    Else
                        strBody += "-- No Rating Messages -- "
                    End If
                End If

                strBody += "</td></tr>"
                strBody += "</table></body></html>"

                ' Set the body of the email
                objMail.EmailBody = strBody

                objMail.MailHost = System.Configuration.ConfigurationManager.AppSettings("mailhost")

                objMail.SendEmail()

                If objMail.hasError Then Throw New Exception("Email transmission to " & objMail.EmailToAddress & " failed: " & objMail.errorMsg)

            End Using

            Return True
        Catch ex As Exception
            err = ex.Message
            HandleError("SendMail", ex)
            Return False
        End Try
    End Function

    Private Function GetCommUnderwriterNameByAgency(quote As QuickQuoteObject) As Object
        Dim strUnderwriterName As String = ""

        If IsNumeric(quote.AgencyId) = True Then
            Using sql As New SQLselectObject(ConfigurationManager.AppSettings("connDiamondReports"))
                sql.queryOrStoredProc = "usp_GetCommercialUnderwriterNameByAgencyId"

                Dim params As New ArrayList()
                params.Add(New System.Data.SqlClient.SqlParameter("@agency_id", CInt(quote.AgencyId)))
                sql.parameters = params

                Using reader As System.Data.SqlClient.SqlDataReader = sql.GetDataReader()
                    If Not sql.hasError Then
                        If reader.HasRows Then
                            While reader.Read()
                                If reader.Item("underwriterName") IsNot DBNull.Value Then
                                    strUnderwriterName = reader.Item("underwriterName").ToString().Trim()
                                End If
                            End While
                        End If
                    End If
                End Using
            End Using
        End If

        Return strUnderwriterName
    End Function

    Private Function GetRatingMessages() As List(Of String)
        Dim QQXML As New QuickQuoteXML()
        Dim qt As QuickQuoteObject = Nothing
        Dim err As String = Nothing
        Dim errList As List(Of String) = Nothing

        Try
            'QQXML.GetRatedQuote(Request("quoteid"), qt, QuickQuoteXML.QuickQuoteSaveType.Quote, err)
            'updated 2/19/2019 to use new Property to get RatedQuote
            qt = RatedQuote
            'If qt.ValidationItems IsNot Nothing And qt.ValidationItems.Count > 0 Then
            'updated 2/19/2019 to also verify that quote is something
            If qt IsNot Nothing AndAlso qt.ValidationItems IsNot Nothing AndAlso qt.ValidationItems.Count > 0 Then
                errList = New List(Of String)
                For Each vitem As QuickQuote.CommonObjects.QuickQuoteValidationItem In qt.ValidationItems
                    If vitem IsNot Nothing AndAlso String.IsNullOrWhiteSpace(vitem.Message) = False Then
                        Select Case vitem.ValidationSeverityType
                            Case QuickQuoteValidationItem.QuickQuoteValidationSeverityType.ValidationWarning
                                errList.Add("(Warning) " & vitem.Message)
                                Exit Select
                            Case QuickQuoteValidationItem.QuickQuoteValidationSeverityType.ValidationError
                                errList.Add("(Error) " & vitem.Message)
                                Exit Select
                            Case QuickQuoteValidationItem.QuickQuoteValidationSeverityType.NonApplicable
                                errList.Add("(Non-Applicable) " & vitem.Message)
                                Exit Select
                            Case QuickQuoteValidationItem.QuickQuoteValidationSeverityType.None
                                errList.Add("(None) " & vitem.Message)
                                Exit Select
                            Case QuickQuoteValidationItem.QuickQuoteValidationSeverityType.Other
                                errList.Add("(Other) " & vitem.Message)
                                Exit Select
                            Case Else
                                Exit Select
                        End Select
                    End If
                Next
            End If

            Return errList
        Catch ex As Exception
            HandleError("GetRatingMessages", ex)
            Return Nothing
        End Try

    End Function
    Private Function GetValidationErrorMessages() As List(Of String)
        Dim errs As List(Of String) = Nothing

        Dim qsaValidationHelper As ControlValidationHelper = Me.QuoteSummaryActionsValidationHelper
        If qsaValidationHelper IsNot Nothing Then
            Dim summaryErrors As List(Of WebValidationItem) = qsaValidationHelper.GetErrors()
            If summaryErrors IsNot Nothing AndAlso summaryErrors.Count > 0 Then
                errs = New List(Of String)
                For Each se As WebValidationItem In summaryErrors
                    If se IsNot Nothing AndAlso String.IsNullOrWhiteSpace(se.Message) = False Then
                        errs.Add(se.Message)
                    End If
                Next
            End If
        End If

        Return errs
    End Function

#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            Exit Sub
        Catch ex As Exception
            HandleError("Page LOAD", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub btnContinue_Click(sender As Object, e As EventArgs) Handles btnContinue.Click
        Dim err As String = Nothing
        Dim msg As String = Nothing
        Try
            'added 12/16/2022
            Dim noteToUW As String = ""
            If Common.Helpers.GenericHelper.SaveToDiamondOnNewBusinessRouteToUnderwriting() = True AndAlso Me.Quote IsNot Nothing AndAlso Me.Quote.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote Then
                Dim quoteId As String = Me.QuoteId
                Dim qqHelper As New QuickQuoteHelperClass
                If qqHelper.IsPositiveIntegerString(quoteId) = False AndAlso Me.Quote IsNot Nothing AndAlso qqHelper.IsPositiveIntegerString(Me.Quote.Database_QuoteId) = True Then
                    quoteId = Me.Quote.Database_QuoteId
                End If
                If Common.QuoteSave.QuoteSaveHelpers.DiamondHasLatest(quoteId) = False Then
                    Dim successfullySavedToDiamond As Boolean = False
                    If Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedAndRatedNewBusinessQuote(Me.Quote, quoteId, successfullySavedToDiamond:=successfullySavedToDiamond, saveType:=If(IsOnAppPage() = True, QuickQuoteXML.QuickQuoteSaveType.AppGap, QuickQuoteXML.QuickQuoteSaveType.Quote)) = False AndAlso successfullySavedToDiamond = False Then
                        noteToUW = "unable to save the latest quote information to Diamond; please verify in VelociRater"
                    End If
                End If
            End If

            ' Send Email
            'If SendUnderwritingEmail(err) Then
            'updated 12/16/2022
            If SendUnderwritingEmail_OptionallyAppendNoteToUW(err, noteForUW:=noteToUW) Then

                'added 12/3/2020 (Interoperability); may not need to tie to Interoperability, but it should be okay for now
                If Me.Quote IsNot Nothing AndAlso Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote AndAlso QuickQuoteHelperClass.AllowVRToUpdateFromDiamond_Interoperability(Me.Quote.LobType) = True Then
                    Dim qqHelper As New QuickQuoteHelperClass

                    Dim qId As Integer = qqHelper.IntegerForString(Me.Quote.Database_QuoteId)
                    Dim policyId As Integer = qqHelper.IntegerForString(Me.Quote.PolicyId)
                    Dim policyImageNum As Integer = qqHelper.IntegerForString(Me.Quote.PolicyImageNum)
                    Dim userId As Integer = qqHelper.IntegerForString(QuickQuoteHelperClass.DiamondUserId())

                    If qId <= 0 AndAlso qqHelper.IsPositiveIntegerString(Me.QuoteId) = True Then
                        qId = CInt(Me.QuoteId)
                    End If
                    If policyImageNum <= 0 Then
                        policyImageNum = 1
                    End If

                    If qId > 0 AndAlso policyId > 0 Then
                        Dim statusUpdateErrorMsg As String = ""
                        Dim qqXml As New QuickQuoteXML
                        qqXml.UpdateQuoteStatus(Me.Quote, QuickQuoteXML.QuickQuoteStatusType.ReferredToUW, statusUpdateErrorMsg)
                        If statusUpdateErrorMsg = "" Then
                            qqXml.GetQuoteHistoryAndSaveNote(qqHelper.IntegerForString(Me.Quote.Database_QuoteId), policyId, policyImageNum, userId)
                        Else
                            'could show message similar to "Problem updating QuickQuote status: " & statusUpdateErrorMsg, but that's probably not necessary
                        End If
                    End If
                ElseIf Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                    'Added ElseIf 08/02/2021 for CAP Endorsements Task 53030 MLW
                    Dim statusUpdateErrorMsg As String = ""
                    Dim qqXml As New QuickQuoteXML
                    qqXml.UpdateQuoteStatus(Me.Quote, QuickQuoteXML.QuickQuoteStatusType.ReferredToUW, statusUpdateErrorMsg)
                End If

                ' Show confirmation alert
                msg = "Thank you for your question, our underwriting team will respond soon."

                'Page.ClientScript.RegisterStartupScript(Me.GetType(), "test", "alert('Thank you for your question, our underwriting team will respond soon.');", True)

                ' Instead of showing the thank you dialog here we're going to redirect to MyVelocirater and show it there since that's where we want to go after sending the email.
                Try
                    Response.Redirect(AppSettings("QuickQuote_Personal_HomePage") & "?eml=1", True)
                Catch ex As Exception

                End Try
            Else
                msg = "There was a problem sending your email, please try again. (" & err & ")"
            End If


            Exit Sub
        Catch ex As Exception
            HandleError("btnContinue_Click", ex)
            Exit Sub
        End Try
    End Sub

#End Region

End Class