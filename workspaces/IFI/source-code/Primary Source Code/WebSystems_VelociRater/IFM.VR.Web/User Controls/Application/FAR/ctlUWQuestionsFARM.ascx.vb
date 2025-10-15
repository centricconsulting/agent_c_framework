Imports IFM
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration.ConfigurationManager
Imports System.Web.Query
Imports Diamond.UI.Utility.DataManager
Imports Diamond.Common.StaticDataManager.Objects.VersionData

Public Class ctlUWQuestionsFARM
    Inherits System.Web.UI.UserControl

#Region "Declarations"

    Private Const ClassName As String = "ctlUWQuestionsFARM"
    Private RowList As List(Of HtmlTableRow) = Nothing
    
    ' Set these constants to the correct values for farm, they are used throughout this control
    Private Const puwAnswerTypeID As String = "1"
    Private Const puwUWTabID As String = "1"
    Private Const puwUWLevelID As String = "1"
    Private Const puwYES As String = "1"
    Private Const puwNO As String = "-1"

    'added for Task 62085 11/8/2022 KLJ
    Private Const VIEWSTATE_HOBBY_FLAG_KEY = "HOBBY-FARM-FLAG"
    Private Property HasHobbyFarm As Boolean
        Get
            Dim hobbyFlag = Convert.ToBoolean(If(ViewState(VIEWSTATE_HOBBY_FLAG_KEY), "false"))
            Return hobbyFlag
        End Get
        Set(value As Boolean)
            ViewState(VIEWSTATE_HOBBY_FLAG_KEY) = value
        End Set
    End Property

    Public Event SaveRequested(ByVal index As Integer, ByVal WhichControl As String)
    Public Event RequestNavigationToApplication(ByVal sender As Object, ByVal QuoteID As String)

    ''' <summary>
    ''' The QuoteId is read from the url parameter 'quoteid' or the routing value 'quoteid'
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property QuoteId As String
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

    Private _QuoteObj As QuickQuoteObject = Nothing

    ' UNCOMMENT THE FOLLOWING PROPERTY ONCE WE'RE READY TO USE A REAL QUOTE OBJECT
    ''' <summary>
    ''' QuoteObj is loaded with the quote based on the QuoteId property
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property QuoteObj As QuickQuoteObject
        Get
            Dim qqxml As New QuickQuoteXML
            Dim errCreateQSO As String = ""
            Dim err As String = ""

            Try
                'Return VR.Common.QuoteSave.QuoteSaveHelpers.GetRatedQuoteById_NOSESSION(QuoteId, errCreateQSO, QuickQuoteObject.QuickQuoteLobType.Farm)
                If String.IsNullOrWhiteSpace(ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/18/2019; original logic in ELSE
                    Return VR.Common.QuoteSave.QuoteSaveHelpers.GetReadOnlyImageFromAnywhere(ReadOnlyPolicyId, ReadOnlyPolicyImageNum, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.AppGap, errorMessage:=errCreateQSO)
                ElseIf String.IsNullOrWhiteSpace(EndorsementPolicyIdAndImageNum) = False Then
                    Return VR.Common.QuoteSave.QuoteSaveHelpers.GetEndorsementQuoteFromAnywhere(EndorsementPolicyId, EndorsementPolicyImageNum, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.AppGap, errorMessage:=errCreateQSO)
                Else
                    Return VR.Common.QuoteSave.QuoteSaveHelpers.GetQuoteById(QuickQuoteObject.QuickQuoteLobType.None, QuoteId, errCreateQSO, True, QuickQuoteXML.QuickQuoteSaveType.AppGap) ' Matt A 8-12-15
                End If
                'Return VR.Common.QuoteSave.QuoteSaveHelpers.GetQuoteById(QuickQuoteObject.QuickQuoteLobType.None, QuoteId, errCreateQSO, True, QuickQuoteXML.QuickQuoteSaveType.Quote)
            Catch ex As Exception
                HandleError("QuoteObj GET", ex)
                Return _QuoteObj
            Finally

            End Try
        End Get
    End Property

    'Public Property QuoteObj As QuickQuoteObject
    '    Get
    '        Return _QuoteObj
    '    End Get
    '    Set(value As QuickQuoteObject)
    '        _QuoteObj = value
    '    End Set
    'End Property

    Private _FocusFieldName As String
    Private Property FocusFieldName As String
        Get
            Return _FocusFieldName
        End Get
        Set(value As String)
            _FocusFieldName = value
        End Set
    End Property

    Dim _qqHelper As QuickQuoteHelperClass = Nothing
    Protected ReadOnly Property QQHelper As QuickQuoteHelperClass
        Get
            If _qqHelper Is Nothing Then
                _qqHelper = New QuickQuoteHelperClass
            End If
            Return _qqHelper
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

#End Region

#Region "Methods and Functions"

    ''' <summary>
    ''' Calls the markup function 'ShowAlert' to display an javascript alert dialog with the passed text
    ''' </summary>
    ''' <param name="msg"></param>
    ''' <remarks></remarks>
    Private Sub Alert(ByVal msg As String)
        Try
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "Alert", "ShowAlert('" & msg & "');", True)
            Exit Sub
        Catch ex As Exception
            HandleError("Alert", ex)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Load any static data
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadStaticData()
        Try
            ' Previous Carrier
            QQHelper.LoadStaticDataOptionsDropDown(ddlQ2PreviousCarrier, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuotePriorCarrier, QuickQuoteHelperClass.QuickQuotePropertyName.PreviousInsurerTypeId, QuickQuoteStaticDataOption.SortBy.None, Me.QuoteObj.LobType)
            ddlQ2PreviousCarrier.Items.Add(New ListItem("OTHER (Not Listed)", "OTHER"))
            If Me.QuoteObj.PriorCarrier IsNot Nothing Then
                If Not (Me.QuoteObj.LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal Or Me.QuoteObj.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal) Or Me.QuoteObj.PriorCarrier.PreviousInsurerTypeId <> "85" Then ' BUG 7793
                    Dim NotFoundItem = (From i As ListItem In Me.ddlQ2PreviousCarrier.Items Where i.Value = "85" Or i.Text.ToLower().Trim() = "not found" Select i).FirstOrDefault() ' 'Not Found' is only for PAA and HOM - because of Comp Raters
                    If NotFoundItem IsNot Nothing Then
                        Me.ddlQ2PreviousCarrier.Items.Remove(NotFoundItem)
                    End If
                End If
            End If
            Exit Sub
        Catch ex As Exception
            HandleError("LoadStaticData", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub ShowMsg(ByVal msg As String)
        lblMsg.Text = msg
    End Sub

    Private Sub HideMsg()
        lblMsg.Text = "&nbsp;"
    End Sub

    ''' <summary>
    ''' Standard error handler
    ''' </summary>
    ''' <param name="RoutineName"></param>
    ''' <param name="Exc"></param>
    ''' <remarks></remarks>
    Private Sub HandleError(ByVal RoutineName As String, ByVal Exc As Exception)
        Dim rec As New IFM.ErrLog_Parameters_Structure()
        Dim msg As String = "Error Detected in ctlUWQuestionsFARM(" & RoutineName & "): " & Exc.Message
        Dim err As String = Nothing

        lblMsg.Text = msg

        rec.ApplicationName = "Velocirater Personal"
        rec.ClassName = ClassName
        rec.ErrorMessage = Exc.Message
        rec.LogDate = DateTime.Now
        rec.RoutineName = RoutineName
        rec.StackTrace = Exc.StackTrace

        WriteErrorLogRecord(rec, err)

        Exit Sub
    End Sub

    ''' <summary>
    ''' Initialize the questions and their controls at form load
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitializeFormQuestions()
        Dim uwqs As List(Of IFM.VR.Common.UWQuestions.VRUWQuestion) = Nothing

        Try    
            ' *******************************
            ' Disable all text input
            ' *******************************
            'txtQ1AddlInfo.Enabled = True
            'txtQ4AddlInfo.Enabled = False
            'txtQ5AddlInfo.Enabled = True
            'txtQ6AddlInfo.Enabled = False
            'txtQ7AddlInfo.Enabled = False
            'txtQ8AddlInfo.Enabled = False
            'txtQ9AddlInfo.Enabled = False
            'txtQ10aAddlInfo.Enabled = False
            'txtQ10AddlInfo.Enabled = False
            'txtQ11AddlInfo.Enabled = False
            'txtQ12aAddlInfo.Enabled = False
            'txtQ12cAddlInfo.Enabled = False
            'ddlQ12dSlideOrDivingBoard.Enabled = False
            'txtQ12eAddlInfo.Enabled = False
            'txtQ14aAddlInfo.Enabled = False
            'txtQ14bAddlInfo.Enabled = False
            'txtQ17AddlInfo.Enabled = False
            'txtQ19bAddlInfo.Enabled = False
            'txtQ20AddlInfo.Enabled = False

            ' *******************************
            ' Hide the expandable responses to questions that may not be answered yet
            ' *******************************
            trQuestion2AddlInfoRow_TEXTBOX.Attributes.Add("style", "display:none;")
            trQuestion4AddlInfoRow.Attributes.Add("style", "display:none;")
            trQuestion6AddlInfoRow.Attributes.Add("style", "display:none;")
            trQuestion7AddlInfoRow.Attributes.Add("style", "display:none;")
            trQuestion8AddlInfoRow.Attributes.Add("style", "display:none;")
            trQuestion9AddlInfoRow.Attributes.Add("style", "display:none;")
            trQuestion10AddlInfoRow.Attributes.Add("style", "display:none;")
            trQuestion11AddlInfoRow.Attributes.Add("style", "display:none;")
            trQuestion17AddlInfoRow.Attributes.Add("style", "display:none;")
            trQuestion20AddlInfoRow.Attributes.Add("style", "display:none;")

            ' *******************************
            ' Hide the expandable questions
            ' *******************************
            trQuestion10a.Attributes.Add("style", "display:none;")
            trQuestion10aAddlInfoRow.Attributes.Add("style", "display:none;")
            trQuestion10b.Attributes.Add("style", "display:none;")
            trQuestion10c.Attributes.Add("style", "display:none;")

            trQuestion12a.Attributes.Add("style", "display:none;")
            trQuestion12aAddlInfoRow.Attributes.Add("style", "display:none;")
            trQuestion12b.Attributes.Add("style", "display:none;")
            trQuestion12bAddlInfoRow.Attributes.Add("style", "display:none;")
            trQuestion12c.Attributes.Add("style", "display:none;")
            trQuestion12cAddlInfoRow.Attributes.Add("style", "display:none;")
            trQuestion12d.Attributes.Add("style", "display:none;")
            trQuestion12dAddlInfoRow.Attributes.Add("style", "display:none;")
            trQuestion12e.Attributes.Add("style", "display:none;")
            trQuestion12eAddlInfoRow.Attributes.Add("style", "display:none;")

            trQuestion13a.Attributes.Add("style", "display:none;")

            trQuestion14a.Attributes.Add("style", "display:none;")
            trQuestion14aAddlInfoRow.Attributes.Add("style", "display:none;")
            trQuestion14b.Attributes.Add("style", "display:none;")
            trQuestion14bAddlInfoRow.Attributes.Add("style", "display:none;")

            trQuestion15a.Attributes.Add("style", "display:none;")

            trQuestion19a.Attributes.Add("style", "display:none;")
            trQuestion19b.Attributes.Add("style", "display:none;")
            trQuestion19bAddlInfoRow.Attributes.Add("style", "display:none;")

            ' *******************************
            ' Set the question text values
            ' *******************************
            uwqs = VR.Common.UWQuestions.UWQuestions.GetFarmUnderwritingQuestions()

            If uwqs Is Nothing OrElse uwqs.Count <= 0 Then Throw New Exception("No Underwriting Questions Returned!!")
            For Each q As VR.Common.UWQuestions.VRUWQuestion In uwqs
                Select Case q.QuestionNumber
                    Case 1
                        lblQ1Text.Text = q.Description
                        SetAttributes(trQuestion1, q.PolicyUnderwritingCodeId)
                        SetAttributes(trQuestion1AddlInfoRow, q.PolicyUnderwritingCodeId, True)
                        Exit Select
                    Case 2
                        lblQ2Text.Text = q.Description
                        SetAttributes(trQuestion2, q.PolicyUnderwritingCodeId)
                        SetAttributes(trQuestion2AddlInfoRow_DDL, q.PolicyUnderwritingCodeId, True)
                        SetAttributes(trQuestion2AddlInfoRow_TEXTBOX, q.PolicyUnderwritingCodeId, True)
                        Exit Select
                    Case 3
                        lblQ3Text.Text = q.Description
                        SetAttributes(trQuestion3, q.PolicyUnderwritingCodeId)
                        SetAttributes(trQuestion3AddlInfoRow, q.PolicyUnderwritingCodeId, True)
                        Exit Select
                    Case 4
                        lblQ4Text.Text = q.Description
                        SetAttributes(trQuestion4, q.PolicyUnderwritingCodeId)
                        SetAttributes(trQuestion4AddlInfoRow, q.PolicyUnderwritingCodeId, True)
                        SetRadioBtnAttribute(rbQ4No, q.PolicyUnderwritingCodeId)
                        SetRadioBtnAttribute(rbQ4Yes, q.PolicyUnderwritingCodeId)
                        Exit Select
                    Case 5
                        lblQ5Text.Text = q.Description
                        SetAttributes(trQuestion5, q.PolicyUnderwritingCodeId,)
                        SetAttributes(trQuestion5AddlInfoRow, q.PolicyUnderwritingCodeId, True)
                        Exit Select
                    Case 6
                        lblQ6Text.Text = q.Description
                        SetAttributes(trQuestion6, q.PolicyUnderwritingCodeId)
                        SetAttributes(trQuestion6AddlInfoRow, q.PolicyUnderwritingCodeId, True)
                        SetRadioBtnAttribute(rbQ6No, q.PolicyUnderwritingCodeId)
                        SetRadioBtnAttribute(rbQ6Yes, q.PolicyUnderwritingCodeId)
                        Exit Select
                    Case 7
                        lblQ7Text.Text = q.Description
                        SetAttributes(trQuestion7, q.PolicyUnderwritingCodeId)
                        SetAttributes(trQuestion7AddlInfoRow, q.PolicyUnderwritingCodeId, True)
                        SetRadioBtnAttribute(rbQ7No, q.PolicyUnderwritingCodeId)
                        SetRadioBtnAttribute(rbQ7Yes, q.PolicyUnderwritingCodeId)
                        Exit Select
                    Case 8
                        lblQ8Text.Text = q.Description
                        SetAttributes(trQuestion8, q.PolicyUnderwritingCodeId)
                        SetAttributes(trQuestion8AddlInfoRow, q.PolicyUnderwritingCodeId, True)
                        SetRadioBtnAttribute(rbQ8No, q.PolicyUnderwritingCodeId)
                        SetRadioBtnAttribute(rbQ8Yes, q.PolicyUnderwritingCodeId)
                        Exit Select
                    Case 9
                        lblQ9Text.Text = q.Description
                        SetAttributes(trQuestion9, q.PolicyUnderwritingCodeId)
                        SetAttributes(trQuestion9AddlInfoRow, q.PolicyUnderwritingCodeId, True)
                        SetRadioBtnAttribute(rbQ9No, q.PolicyUnderwritingCodeId)
                        SetRadioBtnAttribute(rbQ9Yes, q.PolicyUnderwritingCodeId)
                        Exit Select
                    Case 10
                        lblQ10Text.Text = q.Description
                        SetAttributes(trQuestion10, q.PolicyUnderwritingCodeId)
                        SetAttributes(trQuestion10AddlInfoRow, q.PolicyUnderwritingCodeId, True)
                        SetRadioBtnAttribute(rbQ10No, q.PolicyUnderwritingCodeId)
                        SetRadioBtnAttribute(rbQ10Yes, q.PolicyUnderwritingCodeId)
                        Exit Select
                    Case 11
                        lblQ10aText.Text = q.Description
                        SetAttributes(trQuestion10a, q.PolicyUnderwritingCodeId)
                        SetAttributes(trQuestion10aAddlInfoRow, q.PolicyUnderwritingCodeId, True)
                        Exit Select
                    Case 12
                        lblQ10bText.Text = q.Description
                        SetAttributes(trQuestion10b, q.PolicyUnderwritingCodeId)
                        SetRadioBtnAttribute(rbQ10bNo, q.PolicyUnderwritingCodeId)
                        SetRadioBtnAttribute(rbQ10bYes, q.PolicyUnderwritingCodeId)
                        Exit Select
                    Case 13
                        lblQ10cText.Text = q.Description
                        SetAttributes(trQuestion10c, q.PolicyUnderwritingCodeId)
                        SetRadioBtnAttribute(rbQ10cNo, q.PolicyUnderwritingCodeId)
                        SetRadioBtnAttribute(rbQ10cYes, q.PolicyUnderwritingCodeId)
                        Exit Select
                    Case 14
                        lblQ11Text.Text = q.Description
                        SetAttributes(trQuestion11, q.PolicyUnderwritingCodeId)
                        SetAttributes(trQuestion11AddlInfoRow, q.PolicyUnderwritingCodeId, True)
                        SetRadioBtnAttribute(rbQ11No, q.PolicyUnderwritingCodeId)
                        SetRadioBtnAttribute(rbQ11Yes, q.PolicyUnderwritingCodeId)
                        Exit Select
                    Case 15
                        lblQ12Text.Text = q.Description
                        SetAttributes(trQuestion12, q.PolicyUnderwritingCodeId)
                        SetRadioBtnAttribute(rbQ12No, q.PolicyUnderwritingCodeId)
                        SetRadioBtnAttribute(rbQ12Yes, q.PolicyUnderwritingCodeId)
                        Exit Select
                    Case 16
                        lblQ12aText.Text = q.Description
                        SetAttributes(trQuestion12a, q.PolicyUnderwritingCodeId)
                        SetAttributes(trQuestion12aAddlInfoRow, q.PolicyUnderwritingCodeId, True)
                        Exit Select
                    Case 17
                        lblQ12bText.Text = q.Description
                        SetAttributes(trQuestion12b, q.PolicyUnderwritingCodeId)
                        SetAttributes(trQuestion12bAddlInfoRow, q.PolicyUnderwritingCodeId, True)
                        Exit Select
                    Case 18
                        lblQ12cText.Text = q.Description
                        SetAttributes(trQuestion12c, q.PolicyUnderwritingCodeId)
                        SetAttributes(trQuestion12cAddlInfoRow, q.PolicyUnderwritingCodeId, True)
                        Exit Select
                    Case 19
                        lblQ12dText.Text = q.Description
                        SetAttributes(trQuestion12d, q.PolicyUnderwritingCodeId)
                        SetAttributes(trQuestion12dAddlInfoRow, q.PolicyUnderwritingCodeId, True)
                        SetRadioBtnAttribute(rbQ12dNo, q.PolicyUnderwritingCodeId)
                        SetRadioBtnAttribute(rbQ12dYes, q.PolicyUnderwritingCodeId)
                        Exit Select
                    Case 20
                        lblQ12eText.Text = q.Description
                        SetAttributes(trQuestion12e, q.PolicyUnderwritingCodeId)
                        SetAttributes(trQuestion12eAddlInfoRow, q.PolicyUnderwritingCodeId, True)
                        SetRadioBtnAttribute(rbQ12eNo, q.PolicyUnderwritingCodeId)
                        SetRadioBtnAttribute(rbQ12eYes, q.PolicyUnderwritingCodeId)
                        Exit Select
                    Case 21
                        lblQ13Text.Text = q.Description
                        SetAttributes(trQuestion13, q.PolicyUnderwritingCodeId)
                        SetRadioBtnAttribute(rbQ13No, q.PolicyUnderwritingCodeId)
                        SetRadioBtnAttribute(rbQ13Yes, q.PolicyUnderwritingCodeId)
                        Exit Select
                    Case 22
                        lblQ13aText.Text = q.Description
                        SetAttributes(trQuestion13a, q.PolicyUnderwritingCodeId)
                        SetRadioBtnAttribute(rbQ13aNo, q.PolicyUnderwritingCodeId)
                        SetRadioBtnAttribute(rbQ13aYes, q.PolicyUnderwritingCodeId)
                        Exit Select
                    Case 23
                        lblQ14Text.Text = q.Description
                        SetAttributes(trQuestion14, q.PolicyUnderwritingCodeId)
                        SetRadioBtnAttribute(rbQ14No, q.PolicyUnderwritingCodeId)
                        SetRadioBtnAttribute(rbQ14Yes, q.PolicyUnderwritingCodeId)
                        Exit Select
                    Case 24
                        lblQ14aText.Text = q.Description
                        SetAttributes(trQuestion14a, q.PolicyUnderwritingCodeId)
                        SetAttributes(trQuestion14aAddlInfoRow, q.PolicyUnderwritingCodeId, True)
                        Exit Select
                    Case 25
                        lblQ14bText.Text = q.Description
                        SetAttributes(trQuestion14b, q.PolicyUnderwritingCodeId)
                        SetAttributes(trQuestion14bAddlInfoRow, q.PolicyUnderwritingCodeId, True)
                        SetRadioBtnAttribute(rbQ14bNo, q.PolicyUnderwritingCodeId)
                        SetRadioBtnAttribute(rbQ14bYes, q.PolicyUnderwritingCodeId)
                        Exit Select
                    Case 26
                        lblQ15Text.Text = q.Description
                        SetAttributes(trQuestion15, q.PolicyUnderwritingCodeId)
                        SetRadioBtnAttribute(rbQ15No, q.PolicyUnderwritingCodeId)
                        SetRadioBtnAttribute(rbQ15Yes, q.PolicyUnderwritingCodeId)
                        Exit Select
                    Case 27
                        lblQ15aText.Text = q.Description
                        SetAttributes(trQuestion15a, q.PolicyUnderwritingCodeId)
                        SetRadioBtnAttribute(rbQ15aNo, q.PolicyUnderwritingCodeId)
                        SetRadioBtnAttribute(rbQ15aYes, q.PolicyUnderwritingCodeId)
                        Exit Select
                    Case 28
                        lblQ16Text.Text = q.Description
                        SetAttributes(trQuestion16, q.PolicyUnderwritingCodeId)
                        SetRadioBtnAttribute(rbQ16No, q.PolicyUnderwritingCodeId)
                        SetRadioBtnAttribute(rbQ16Yes, q.PolicyUnderwritingCodeId)
                        Exit Select
                    Case 29
                        lblQ17Text.Text = q.Description
                        SetAttributes(trQuestion17, q.PolicyUnderwritingCodeId)
                        SetAttributes(trQuestion17AddlInfoRow, q.PolicyUnderwritingCodeId, True)
                        SetRadioBtnAttribute(rbQ17No, q.PolicyUnderwritingCodeId)
                        SetRadioBtnAttribute(rbQ17Yes, q.PolicyUnderwritingCodeId)
                        Exit Select
                    Case 30
                        lblQ18Text.Text = q.Description
                        SetAttributes(trQuestion18, q.PolicyUnderwritingCodeId)
                        SetRadioBtnAttribute(rbQ18No, q.PolicyUnderwritingCodeId)
                        SetRadioBtnAttribute(rbQ18Yes, q.PolicyUnderwritingCodeId)
                        Exit Select
                    Case 31
                        lblQ19Text.Text = q.Description
                        SetAttributes(trQuestion19, q.PolicyUnderwritingCodeId)
                        SetRadioBtnAttribute(rbQ19No, q.PolicyUnderwritingCodeId)
                        SetRadioBtnAttribute(rbQ19Yes, q.PolicyUnderwritingCodeId)
                        Exit Select
                    Case 32
                        lblQ19aText.Text = q.Description
                        SetAttributes(trQuestion19a, q.PolicyUnderwritingCodeId)
                        SetRadioBtnAttribute(rbQ19aNo, q.PolicyUnderwritingCodeId)
                        SetRadioBtnAttribute(rbQ19aYes, q.PolicyUnderwritingCodeId)
                        Exit Select
                    Case 33
                        lblQ19bText.Text = q.Description
                        SetAttributes(trQuestion19b, q.PolicyUnderwritingCodeId)
                        SetAttributes(trQuestion19bAddlInfoRow, q.PolicyUnderwritingCodeId, True)
                        Exit Select
                    Case 34
                        lblQ20Text.Text = q.Description
                        SetAttributes(trQuestion20, q.PolicyUnderwritingCodeId)
                        SetAttributes(trQuestion20AddlInfoRow, q.PolicyUnderwritingCodeId, True)
                        SetRadioBtnAttribute(rbQ20No, q.PolicyUnderwritingCodeId)
                        SetRadioBtnAttribute(rbQ20Yes, q.PolicyUnderwritingCodeId)
                        Exit Select
                    Case Else
                        Throw New Exception("Unknown Question Number: " & q.QuestionNumber.ToString())
                End Select
            Next

            Exit Sub
        Catch ex As Exception
            HandleError("InitializeFormQuestions", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub SetAttributes(element As HtmlControl, diamondcode As String, Optional isDescription As Boolean = False)
        Dim class_type As String = "questionRow " 'space on purpose

        If isDescription Then
            class_type = "descriptionRow " 'space on purpose
        End If

        element.Attributes.Remove("class")
        element.Attributes.Add("class", class_type & diamondcode)
        element.Attributes.Add("data-diamondcode", diamondcode)
    End Sub

    Private Sub SetRadioBtnAttribute(element As RadioButton, diamondcode As String)
        element.Attributes.Add("data-diamondcode", diamondcode)
    End Sub

    ''' <summary>
    ''' Displays the additional info row for the passed question number
    ''' </summary>
    ''' <param name="QuestionNumber"></param>
    ''' <remarks></remarks>
    Private Sub ShowAddlInfoRow(QuestionNumber As String)
        Try
            Select Case QuestionNumber.ToUpper()
                Case "3" 'added for Task 62085 11/07/2022 KLJ
                    trQuestion3AddlInfoRow.Attributes.Add("style", "display:'';")
                    txtQ3AddlInfo.Attributes.Remove("disabled")
                    Exit Select
                Case "4"
                    trQuestion4AddlInfoRow.Attributes.Add("style", "display:'';")
                    txtQ4AddlInfo.Attributes.Remove("disabled")
                    Exit Select
                Case "5"
                    trQuestion5AddlInfoRow.Attributes.Add("style", "display:'';")
                    txtQ5AddlInfo.Attributes.Remove("disabled")
                    Exit Select
                Case "6"
                    trQuestion6AddlInfoRow.Attributes.Add("style", "display:'';")
                    txtQ6AddlInfo.Attributes.Remove("disabled")
                    Exit Select
                Case "7"
                    trQuestion7AddlInfoRow.Attributes.Add("style", "display:'';")
                    txtQ7AddlInfo.Attributes.Remove("disabled")
                    Exit Select
                Case "8"
                    trQuestion8AddlInfoRow.Attributes.Add("style", "display:'';")
                    txtQ8AddlInfo.Attributes.Remove("disabled")
                    Exit Select
                Case "9"
                    trQuestion9AddlInfoRow.Attributes.Add("style", "display:'';")
                    txtQ9AddlInfo.Attributes.Remove("disabled")
                    Exit Select
                Case "10"
                    trQuestion10AddlInfoRow.Attributes.Add("style", "display:'';")
                    txtQ10AddlInfo.Attributes.Remove("disabled")
                    Exit Select
                Case "10A"
                    trQuestion10aAddlInfoRow.Attributes.Add("style", "display:'';")
                    txtQ10aAddlInfo.Attributes.Remove("disabled")
                    Exit Select
                Case "11"
                    trQuestion11AddlInfoRow.Attributes.Add("style", "display:'';")
                    txtQ11AddlInfo.Attributes.Remove("disabled")
                    Exit Select
                Case "12D"
                    trQuestion12dAddlInfoRow.Attributes.Add("style", "display:'';")
                    ddlQ12dSlideOrDivingBoard.Attributes.Remove("disabled")
                    Exit Select
                Case "12E"
                    trQuestion12eAddlInfoRow.Attributes.Add("style", "display:'';")
                    txtQ12eAddlInfo.Attributes.Remove("disabled")
                    Exit Select
                Case "14A"
                    trQuestion14aAddlInfoRow.Attributes.Add("style", "display:'';")
                    txtQ14aAddlInfo.Attributes.Remove("disabled")
                    Exit Select
                Case "14B"
                    trQuestion14bAddlInfoRow.Attributes.Add("style", "display:'';")
                    txtQ14bAddlInfo.Attributes.Remove("disabled")
                    Exit Select
                Case "17"
                    trQuestion17AddlInfoRow.Attributes.Add("style", "display:'';")
                    txtQ17AddlInfo.Attributes.Remove("disabled")
                    Exit Select
                Case "19B"
                    trQuestion19bAddlInfoRow.Attributes.Add("style", "display:'';")
                    txtQ19bAddlInfo.Attributes.Remove("disabled")
                    Exit Select
                Case "20"
                    trQuestion20AddlInfoRow.Attributes.Add("style", "display:'';")
                    txtQ20AddlInfo.Attributes.Remove("disabled")
                    Exit Select
                Case Else
                    Throw New Exception("Row " & QuestionNumber & " not defined")
            End Select

            Exit Sub
        Catch ex As Exception
            HandleError("ShowAddlInfoRow", ex)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Hides the additional info row for the passed question number
    ''' </summary>
    ''' <param name="QuestionNumber"></param>
    ''' <remarks></remarks>
    Private Sub HideAddlInfoRow(QuestionNumber As String)
        Try
            Select Case QuestionNumber.ToUpper()
                Case "3"
                    trQuestion3AddlInfoRow.Attributes.Add("style", "display:none;")
                    txtQ3AddlInfo.Attributes.Add("disabled", "True")
                    Exit Select
                Case "4"
                    trQuestion4AddlInfoRow.Attributes.Add("style", "display:none;")
                    txtQ4AddlInfo.Attributes.Add("disabled", "True")
                    Exit Select
                Case "5"
                    trQuestion5AddlInfoRow.Attributes.Add("style", "display:none;")
                    txtQ5AddlInfo.Attributes.Add("disabled", "True")
                    Exit Select
                Case "6"
                    trQuestion6AddlInfoRow.Attributes.Add("style", "display:none;")
                    txtQ6AddlInfo.Attributes.Add("disabled", "True")
                    Exit Select
                Case "7"
                    trQuestion7AddlInfoRow.Attributes.Add("style", "display:none;")
                    txtQ7AddlInfo.Attributes.Add("disabled", "True")
                    Exit Select
                Case "8"
                    trQuestion8AddlInfoRow.Attributes.Add("style", "display:none;")
                    txtQ8AddlInfo.Attributes.Add("disabled", "True")
                    Exit Select
                Case "9"
                    trQuestion9AddlInfoRow.Attributes.Add("style", "display:none;")
                    txtQ9AddlInfo.Attributes.Add("disabled", "True")
                    Exit Select
                Case "10"
                    trQuestion10AddlInfoRow.Attributes.Add("style", "display:none;")
                    txtQ10AddlInfo.Attributes.Add("disabled", "True")
                    Exit Select
                Case "10A"
                    trQuestion10aAddlInfoRow.Attributes.Add("style", "display:none;")
                    txtQ10aAddlInfo.Attributes.Add("disabled", "True")
                    Exit Select
                Case "11"
                    trQuestion11AddlInfoRow.Attributes.Add("style", "display:none;")
                    txtQ11AddlInfo.Attributes.Add("disabled", "True")
                    Exit Select
                Case "12D"
                    trQuestion12dAddlInfoRow.Attributes.Add("style", "display:none;")
                    ddlQ12dSlideOrDivingBoard.Attributes.Add("disabled", "True")
                    Exit Select
                Case "12E"
                    trQuestion12eAddlInfoRow.Attributes.Add("style", "display:none;")
                    txtQ12eAddlInfo.Attributes.Add("disabled", "True")
                    Exit Select
                Case "14A"
                    trQuestion14aAddlInfoRow.Attributes.Add("style", "display:none;")
                    txtQ14aAddlInfo.Attributes.Add("disabled", "True")
                    Exit Select
                Case "14B"
                    trQuestion14bAddlInfoRow.Attributes.Add("style", "display:none;")
                    txtQ14bAddlInfo.Attributes.Add("disabled", "True")
                    Exit Select
                Case "17"
                    trQuestion17AddlInfoRow.Attributes.Add("style", "display:none;")
                    txtQ17AddlInfo.Attributes.Add("disabled", "True")
                    Exit Select
                Case "19B"
                    trQuestion19bAddlInfoRow.Attributes.Add("style", "display:none;")
                    txtQ19bAddlInfo.Attributes.Add("disabled", "True")
                    Exit Select
                Case "20"
                    trQuestion20AddlInfoRow.Attributes.Add("style", "display:none;")
                    txtQ20AddlInfo.Attributes.Add("disabled", "True")
                    Exit Select
                Case Else
                    Throw New Exception("Row " & QuestionNumber & " not defined")
            End Select

            Exit Sub
        Catch ex As Exception
            HandleError("HideAddlInfoRow", ex)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Expands an expandable question (10, 12, 13, 14, 15, 19 and 19A)
    ''' </summary>
    ''' <param name="QuestionNumber"></param>
    ''' <remarks></remarks>
    Private Sub ExpandQuestion(ByVal QuestionNumber As String)
        Try
            ' if you add expandable questions you need to add them to this select statement
            Select Case QuestionNumber.ToUpper()
                Case "10"
                    'show 10a, 10b, 10c
                    trQuestion10a.Attributes.Add("style", "display:'';")
                    txtQ10aAddlInfo.Attributes.Remove("disabled")
                    trQuestion10aAddlInfoRow.Attributes.Add("style", "display:'';")
                    trQuestion10b.Attributes.Add("style", "display:'';")
                    trQuestion10c.Attributes.Add("style", "display:'';")
                    Exit Select
                Case "12"
                    'show 12a, 12b, 12c, 12d, 12e
                    trQuestion12a.Attributes.Add("style", "display:'';")
                    trQuestion12aAddlInfoRow.Attributes.Add("style", "display:'';")
                    trQuestion12b.Attributes.Add("style", "display:'';")
                    trQuestion12bAddlInfoRow.Attributes.Add("style", "display:'';")
                    trQuestion12c.Attributes.Add("style", "display:'';")
                    trQuestion12cAddlInfoRow.Attributes.Add("style", "display:'';")
                    trQuestion12d.Attributes.Add("style", "display:'';")
                    trQuestion12dAddlInfoRow.Attributes.Add("style", "display:none;") 'Q12d Yes shows this
                    trQuestion12e.Attributes.Add("style", "display:'';")
                    trQuestion12eAddlInfoRow.Attributes.Add("style", "display:none;") 'this is currently not in use
                    'enable 12a-12e
                    txtQ12aAddlInfo.Attributes.Remove("disabled")
                    ddlQ12bAboveOrInGroundPool.Attributes.Remove("disabled")
                    txtQ12cAddlInfo.Attributes.Remove("disabled")
                    ddlQ12dSlideOrDivingBoard.Attributes.Add("disabled", "True") 'Q12d Yes enables this
                    txtQ12eAddlInfo.Attributes.Add("disabled", "True") 'this is currently not in use
                    Exit Select
                Case "13"
                    'show 13a
                    trQuestion13a.Attributes.Add("style", "display:'';")
                    Exit Select
                Case "14"
                    'show 14a, 14b
                    trQuestion14a.Attributes.Add("style", "display:'';")
                    trQuestion14aAddlInfoRow.Attributes.Add("style", "display:'';")
                    txtQ14aAddlInfo.Attributes.Remove("disabled")
                    trQuestion14b.Attributes.Add("style", "display:'';")
                    trQuestion14bAddlInfoRow.Attributes.Add("style", "display:none;") 'Q14b Yes shows add'l info
                    Exit Select
                Case "15"
                    'show 15a
                    trQuestion15a.Attributes.Add("style", "display:'';")
                    Exit Select
                Case "19"
                    'show 19a
                    trQuestion19a.Attributes.Add("style", "display:'';")
                    'trQuestion19b.Attributes.Add("style", "display:none;")
                    Exit Select
                Case "19A"
                    'Show 19b when 19a No
                    trQuestion19b.Attributes.Add("style", "display:'';")
                    trQuestion19bAddlInfoRow.Attributes.Add("style", "display:'';")
                    txtQ19bAddlInfo.Attributes.Remove("disabled")
                    Exit Select
                Case Else
                    Exit Sub
            End Select

            Exit Sub
        Catch ex As Exception
            HandleError("ExpandQuestion", ex)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Expands an expandable question (10, 12, 13, 14, 15, 19 and 19A)
    ''' </summary>
    ''' <param name="QuestionNumber"></param>
    ''' <remarks></remarks>
    Private Sub CollapseQuestion(ByVal QuestionNumber As String)
        Try
            ' if you add expandable questions you need to add them to this select statement
            Select Case QuestionNumber.ToUpper()
                Case "10"
                    'hide 10a, 10b, 10c
                    trQuestion10a.Attributes.Add("style", "display:none;")
                    trQuestion10aAddlInfoRow.Attributes.Add("style", "display:none;")
                    trQuestion10b.Attributes.Add("style", "display:none;")
                    trQuestion10c.Attributes.Add("style", "display:none;")
                    'disable 10a
                    txtQ10aAddlInfo.Attributes.Add("disabled", "True")
                    Exit Select
                Case "12"
                    'hide 12a, 12b, 12c, 12d, 12e
                    trQuestion12a.Attributes.Add("style", "display:none;")
                    trQuestion12aAddlInfoRow.Attributes.Add("style", "display:none;")
                    trQuestion12b.Attributes.Add("style", "display:none;")
                    trQuestion12bAddlInfoRow.Attributes.Add("style", "display:none;")
                    trQuestion12c.Attributes.Add("style", "display:none;")
                    trQuestion12cAddlInfoRow.Attributes.Add("style", "display:none;")
                    trQuestion12d.Attributes.Add("style", "display:none;")
                    trQuestion12dAddlInfoRow.Attributes.Add("style", "display:none;")
                    trQuestion12e.Attributes.Add("style", "display:none;")
                    trQuestion12eAddlInfoRow.Attributes.Add("style", "display:none;")
                    'disable 12a, 12b, 12c, 12d, 12e
                    txtQ12aAddlInfo.Attributes.Add("disabled", "True")
                    ddlQ12bAboveOrInGroundPool.Attributes.Add("disabled", "True")
                    txtQ12cAddlInfo.Attributes.Add("disabled", "True")
                    ddlQ12dSlideOrDivingBoard.Attributes.Add("disabled", "True")
                    txtQ12eAddlInfo.Attributes.Add("disabled", "True")
                    Exit Select
                Case "13"
                    'hide 13a
                    trQuestion13a.Attributes.Add("style", "display:none;") 
                    Exit Select
                Case "14"
                    'hide 14a, 14b
                    trQuestion14a.Attributes.Add("style", "display:none;")
                    trQuestion14aAddlInfoRow.Attributes.Add("style", "display:none;")
                    trQuestion14b.Attributes.Add("style", "display:none;")
                    trQuestion14bAddlInfoRow.Attributes.Add("style", "display:none;")
                    'disable 14a
                    txtQ14aAddlInfo.Attributes.Add("disabled", "True")
                    Exit Select
                Case "15"
                    'hide 15a
                    trQuestion15a.Attributes.Add("style", "display:none;")
                    Exit Select
                Case "19"
                    'hide 19a
                    trQuestion19a.Attributes.Add("style", "display:none;")
                    trQuestion19b.Attributes.Add("style", "display:none;")
                    'disable 19b
                    txtQ19bAddlInfo.Attributes.Add("disabled", "True")
                    Exit Select
                Case "19A"
                    'hide 19b
                    trQuestion19b.Attributes.Add("style", "display:none;")
                    trQuestion19bAddlInfoRow.Attributes.Add("style", "display:none;")
                    'disable 19b
                    txtQ19bAddlInfo.Attributes.Add("disabled", "True")
                    Exit Select
                Case Else
                    Exit Sub
            End Select

            Exit Sub
        Catch ex As Exception
            HandleError("ExpandQuestion", ex)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Returns TRUE if the quote object has been iniitalized, FALSE of not
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function QuoteObjectOK() As Boolean
        Try
            If QuoteObj Is Nothing OrElse QuoteObj.Equals(New QuickQuoteObject) Then
                Return False
            Else
                Return True
            End If
        Catch ex As Exception
            HandleError("QuoteObjectOK", ex)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Display any existing UW Question data from the quote object
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DisplayExistingQuoteData()
        Try
            ' Check quote object
            If Not QuoteObjectOK() Then Throw New Exception("Quote object has not been initialized!")

            'added 8/10/2018 for multi-state
            Dim multiStateQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject) = Nothing
            Dim quoteToUse As QuickQuote.CommonObjects.QuickQuoteObject = Nothing
            If QuoteObj IsNot Nothing Then
                multiStateQuotes = QQHelper.MultiStateQuickQuoteObjects(QuoteObj) 'should always return at least QuoteObj in the list
                If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                    quoteToUse = multiStateQuotes.Item(0) 'will just use the 1st stateQuote found (or QuoteObj) since all questions are currently the same for all states
                End If
            End If

            ' If trampoline was indicated on the location, check question 13 and lock it
            '8/10/2018 note: may need to check 1st location for each state when applicable
            If QuoteObj.Locations IsNot Nothing AndAlso QuoteObj.Locations.Count >= 1 Then
                Dim l1 As QuickQuoteLocation = QuoteObj.Locations(0)
                If l1.TrampolineSurcharge Then
                    rbQ13Yes.Checked = True
                    ExpandQuestion("13")
                    rbQ13Yes.Enabled = False
                    rbQ13No.Enabled = False
                End If
                If l1.FarmTypeHobby = True Then
                    txtQ3AddlInfo.Attributes.Add("style", "display:'';")
                    ddlQ3PrincipalTypeOfFarming.Attributes.Add("style", "display:none;")
                Else
                    txtQ3AddlInfo.Attributes.Add("style", "display:none;")
                    ddlQ3PrincipalTypeOfFarming.Attributes.Add("style", "display:'';")
                End If
                ''added for Task 62085 11/7/2022 KLJ
                'txtQ3AddlInfo.Visible = l1.FarmTypeHobby
                'ddlQ3PrincipalTypeOfFarming.Visible = Not l1.FarmTypeHobby
                HasHobbyFarm = l1.FarmTypeHobby

            End If

            'If QuoteObj.PolicyUnderwritings Is Nothing OrElse QuoteObj.PolicyUnderwritings.Count <= 0 Then Exit Sub
            'updated 8/10/2018
            If quoteToUse Is Nothing OrElse quoteToUse.PolicyUnderwritings Is Nothing OrElse quoteToUse.PolicyUnderwritings.Count <= 0 Then Exit Sub

            'For Each pu As QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting In QuoteObj.PolicyUnderwritings
            'updated 8/10/2018
            For Each pu As QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting In quoteToUse.PolicyUnderwritings
                Select Case pu.PolicyUnderwritingCodeId
                    Case "9529"
                        txtQ1AddlInfo.Text = pu.PolicyUnderwritingExtraAnswer
                        Exit Select
                    Case "9530"
                        ddlQ2PreviousCarrier.SelectedIndex = 0
                        trQuestion2AddlInfoRow_TEXTBOX.Attributes.Add("style", "display:none;")
                        For Each li As ListItem In ddlQ2PreviousCarrier.Items
                            If li.Text.ToUpper() = pu.PolicyUnderwritingExtraAnswer.ToUpper() Then
                                ddlQ2PreviousCarrier.SelectedIndex = -1
                                li.Selected = True
                                Exit Select
                            End If
                        Next
                        If ddlQ2PreviousCarrier.SelectedIndex = 0 AndAlso pu.PolicyUnderwritingExtraAnswer.Trim <> String.Empty Then
                            trQuestion2AddlInfoRow_TEXTBOX.Attributes.Add("style", "display:'';")
                            trQuestion2AddlInfoRow_DDL.Attributes.Add("style", "display:none;")
                            txtQ2PreviousCarrier.Text = pu.PolicyUnderwritingExtraAnswer
                        Else
                            trQuestion2AddlInfoRow_TEXTBOX.Attributes.Add("style", "display:none;")
                        End If
                        Exit Select
                    Case "9531"
                        'added for Task 62085 11/7/2022 KLJ
                        If HasHobbyFarm = False Then
                            ddlQ3PrincipalTypeOfFarming.SelectedIndex = 0
                            For Each li As ListItem In ddlQ3PrincipalTypeOfFarming.Items
                                If li.Text.ToUpper = pu.PolicyUnderwritingExtraAnswer.ToUpper Then
                                    ddlQ3PrincipalTypeOfFarming.SelectedIndex = -1
                                    li.Selected = True
                                    Exit Select
                                End If
                            Next
                        Else
                            txtQ3AddlInfo.Text = pu.PolicyUnderwritingExtraAnswer.ToUpper
                        End If

                        Exit Select
                    Case "9532"
                        Select Case pu.PolicyUnderwritingAnswer
                            Case puwYES
                                rbQ4Yes.Checked = True
                                HideAddlInfoRow("4")
                                Exit Select
                            Case puwNO
                                rbQ4No.Checked = True
                                ShowAddlInfoRow("4")
                                txtQ4AddlInfo.Text = pu.PolicyUnderwritingExtraAnswer
                                Exit Select
                            Case Else
                                Throw New Exception("Unknown answer value (UW Code " & pu.PolicyUnderwritingCodeId & ")")
                        End Select
                        Exit Select
                    Case "9533"
                        txtQ5AddlInfo.Text = pu.PolicyUnderwritingExtraAnswer
                        Exit Select
                    Case "9534"
                        Select Case pu.PolicyUnderwritingAnswer
                            Case puwYES
                                rbQ6Yes.Checked = True
                                ShowAddlInfoRow("6")
                                txtQ6AddlInfo.Text = pu.PolicyUnderwritingExtraAnswer
                                Exit Select
                            Case puwNO
                                rbQ6No.Checked = True
                                HideAddlInfoRow("6")
                                Exit Select
                            Case Else
                                Throw New Exception("Unknown answer value (UW Code " & pu.PolicyUnderwritingCodeId & ")")
                        End Select
                        Exit Select
                    Case "9535"  ' KILL QUESTION
                        Select Case pu.PolicyUnderwritingAnswer
                            Case puwYES
                                rbQ7Yes.Checked = True
                                ShowAddlInfoRow("7")
                                txtQ7AddlInfo.Text = pu.PolicyUnderwritingExtraAnswer
                                Exit Select
                            Case puwNO
                                rbQ7No.Checked = True
                                HideAddlInfoRow("7")
                                Exit Select
                            Case Else
                                Throw New Exception("Unknown answer value (UW Code " & pu.PolicyUnderwritingCodeId & ")")
                        End Select
                        Exit Select
                    Case "9536"  ' KILL QUESTION
                        Select Case pu.PolicyUnderwritingAnswer
                            Case puwYES
                                rbQ8Yes.Checked = True
                                ShowAddlInfoRow("8")
                                txtQ8AddlInfo.Text = pu.PolicyUnderwritingExtraAnswer
                                Exit Select
                            Case puwNO
                                rbQ8No.Checked = True
                                HideAddlInfoRow("8")
                                Exit Select
                            Case Else
                                Throw New Exception("Unknown answer value (UW Code " & pu.PolicyUnderwritingCodeId & ")")
                        End Select
                        Exit Select
                    Case "9537"  ' KILL QUESTION
                        Select Case pu.PolicyUnderwritingAnswer
                            Case puwYES
                                rbQ9Yes.Checked = True
                                ShowAddlInfoRow("9")
                                txtQ9AddlInfo.Text = pu.PolicyUnderwritingExtraAnswer
                                Exit Select
                            Case puwNO
                                rbQ9No.Checked = True
                                HideAddlInfoRow("9")
                                Exit Select
                            Case Else
                                Throw New Exception("Unknown answer value (UW Code " & pu.PolicyUnderwritingCodeId & ")")
                        End Select
                        Exit Select
                    Case "9538"
                        Select Case pu.PolicyUnderwritingAnswer
                            Case puwYES
                                rbQ10Yes.Checked = True
                                ShowAddlInfoRow("10")
                                txtQ10AddlInfo.Text = pu.PolicyUnderwritingExtraAnswer
                                ExpandQuestion("10")
                                Exit Select
                            Case puwNO
                                rbQ10No.Checked = True
                                HideAddlInfoRow("10")
                                CollapseQuestion("10")
                                Exit Select
                            Case Else
                                Throw New Exception("Unknown answer value (UW Code " & pu.PolicyUnderwritingCodeId & ")")
                        End Select
                        Exit Select
                    Case "9539"
                        txtQ10aAddlInfo.Text = pu.PolicyUnderwritingExtraAnswer
                        Exit Select
                    Case "9540"
                        Select Case pu.PolicyUnderwritingAnswer
                            Case puwYES
                                rbQ10bYes.Checked = True
                                Exit Select
                            Case puwNO
                                rbQ10bNo.Checked = True
                                Exit Select
                            Case Else
                                Throw New Exception("Unknown answer value (UW Code " & pu.PolicyUnderwritingCodeId & ")")
                        End Select
                        Exit Select
                    Case "9541"
                        Select Case pu.PolicyUnderwritingAnswer
                            Case puwYES
                                rbQ10cYes.Checked = True
                                Exit Select
                            Case puwNO
                                rbQ10cNo.Checked = True
                                Exit Select
                            Case Else
                                Throw New Exception("Unknown answer value (UW Code " & pu.PolicyUnderwritingCodeId & ")")
                        End Select
                        Exit Select
                    Case "9542"  ' KILL QUESTION
                        Select Case pu.PolicyUnderwritingAnswer
                            Case puwYES
                                rbQ11Yes.Checked = True
                                ShowAddlInfoRow("11")
                                txtQ11AddlInfo.Text = pu.PolicyUnderwritingExtraAnswer
                                Exit Select
                            Case puwNO
                                rbQ11No.Checked = True
                                HideAddlInfoRow("11")
                                Exit Select
                            Case Else
                                Throw New Exception("Unknown answer value (UW Code " & pu.PolicyUnderwritingCodeId & ")")
                        End Select
                        Exit Select
                    Case "9543"
                        Select Case pu.PolicyUnderwritingAnswer
                            Case puwYES
                                rbQ12Yes.Checked = True
                                ExpandQuestion("12")
                                Exit Select
                            Case puwNO
                                rbQ12No.Checked = True
                                CollapseQuestion("12")
                                Exit Select
                            Case Else
                                Throw New Exception("Unknown answer value (UW Code " & pu.PolicyUnderwritingCodeId & ")")
                        End Select
                        Exit Select
                    Case "9544"
                        txtQ12aAddlInfo.Text = pu.PolicyUnderwritingExtraAnswer
                        Exit Select
                    Case "9545"
                        For Each li As ListItem In ddlQ12bAboveOrInGroundPool.Items
                            If li.Text.ToUpper() = pu.PolicyUnderwritingExtraAnswer.ToUpper() Then
                                ddlQ12bAboveOrInGroundPool.SelectedIndex = -1
                                li.Selected = True
                                Exit For
                            End If
                        Next
                        Exit Select
                    Case "9546"
                        txtQ12cAddlInfo.Text = pu.PolicyUnderwritingExtraAnswer
                        Exit Select
                    Case "9547"
                        Select Case pu.PolicyUnderwritingAnswer
                            Case puwYES
                                rbQ12dYes.Checked = True
                                If rbQ12Yes.Checked AndAlso rbQ12dYes.Checked Then
                                    ShowAddlInfoRow("12D")
                                Else
                                    HideAddlInfoRow("12D")
                                End If
                                Exit Select
                            Case puwNO
                                rbQ12dNo.Checked = True
                                HideAddlInfoRow("12D")
                                Exit Select
                            Case Else
                                Throw New Exception("Unknown answer value (UW Code " & pu.PolicyUnderwritingCodeId & ")")
                        End Select
                        For Each li As ListItem In ddlQ12dSlideOrDivingBoard.Items
                            If li.Value.ToUpper() = pu.PolicyUnderwritingExtraAnswer.ToUpper() Then
                                ddlQ12dSlideOrDivingBoard.SelectedIndex = -1
                                li.Selected = True
                                Exit For
                            End If
                        Next
                        Exit Select
                    Case "9548"
                        'This currently isn't being used - always hide
                        Select Case pu.PolicyUnderwritingAnswer
                            Case puwYES
                                rbQ12eYes.Checked = True
                                HideAddlInfoRow("12E") 'not currently used
                                Exit Select
                            Case puwNO
                                rbQ12eNo.Checked = True
                                HideAddlInfoRow("12E") 'not currently used
                                txtQ12eAddlInfo.Text = pu.PolicyUnderwritingExtraAnswer
                                Exit Select
                            Case Else
                                Throw New Exception("Unknown answer value (UW Code " & pu.PolicyUnderwritingCodeId & ")")
                        End Select
                        Exit Select
                    Case "9549"
                        Select Case pu.PolicyUnderwritingAnswer
                            Case puwYES
                                rbQ13Yes.Checked = True
                                ExpandQuestion("13")
                                Exit Select
                            Case puwNO
                                rbQ13No.Checked = True
                                CollapseQuestion("13")
                                Exit Select
                            Case Else
                                Throw New Exception("Unknown answer value (UW Code " & pu.PolicyUnderwritingCodeId & ")")
                        End Select
                        Exit Select
                    Case "9550"
                        Select Case pu.PolicyUnderwritingAnswer
                            Case puwYES
                                rbQ13aYes.Checked = True
                                Exit Select
                            Case puwNO
                                rbQ13aNo.Checked = True
                                Exit Select
                            Case Else
                                Throw New Exception("Unknown answer value (UW Code " & pu.PolicyUnderwritingCodeId & ")")
                        End Select
                        Exit Select
                    Case "9551"  ' KILL QUESTION
                        Select Case pu.PolicyUnderwritingAnswer
                            Case puwYES
                                rbQ14Yes.Checked = True
                                ExpandQuestion("14")
                                Exit Select
                            Case puwNO
                                rbQ14No.Checked = True
                                CollapseQuestion("14")
                                Exit Select
                            Case Else
                                Throw New Exception("Unknown answer value (UW Code " & pu.PolicyUnderwritingCodeId & ")")
                        End Select
                        Exit Select
                    Case "9552"
                        txtQ14aAddlInfo.Text = pu.PolicyUnderwritingExtraAnswer
                        Exit Select
                    Case "9553"
                        Select Case pu.PolicyUnderwritingAnswer
                            Case puwYES
                                rbQ14bYes.Checked = True
                                If rbQ14Yes.Checked AndAlso rbQ14bYes.Checked Then
                                    ShowAddlInfoRow("14B")
                                Else
                                    HideAddlInfoRow("14B")
                                End If
                                txtQ14bAddlInfo.Text = pu.PolicyUnderwritingExtraAnswer
                                Exit Select
                            Case puwNO
                                rbQ14bNo.Checked = True
                                HideAddlInfoRow("14B")
                                Exit Select
                            Case Else
                                Throw New Exception("Unknown answer value (UW Code " & pu.PolicyUnderwritingCodeId & ")")
                        End Select
                        Exit Select
                    Case "9554"
                        Select Case pu.PolicyUnderwritingAnswer
                            Case puwYES
                                rbQ15Yes.Checked = True
                                ExpandQuestion("15")
                                Exit Select
                            Case puwNO
                                rbQ15No.Checked = True
                                CollapseQuestion("15")
                                Exit Select
                            Case Else
                                Throw New Exception("Unknown answer value (UW Code " & pu.PolicyUnderwritingCodeId & ")")
                        End Select
                        Exit Select
                    Case "9555"
                        Select Case pu.PolicyUnderwritingAnswer
                            Case puwYES
                                rbQ15aYes.Checked = True
                                Exit Select
                            Case puwNO
                                rbQ15aNo.Checked = True
                                Exit Select
                            Case Else
                                Throw New Exception("Unknown answer value (UW Code " & pu.PolicyUnderwritingCodeId & ")")
                        End Select
                        Exit Select
                    Case "9556"
                        Select Case pu.PolicyUnderwritingAnswer
                            Case puwYES
                                rbQ16Yes.Checked = True
                                Exit Select
                            Case puwNO
                                rbQ16No.Checked = True
                                Exit Select
                            Case Else
                                Throw New Exception("Unknown answer value (UW Code " & pu.PolicyUnderwritingCodeId & ")")
                        End Select
                        Exit Select
                    Case "9557"
                        Select Case pu.PolicyUnderwritingAnswer
                            Case puwYES
                                rbQ17Yes.Checked = True
                                ShowAddlInfoRow("17")
                                txtQ17AddlInfo.Text = pu.PolicyUnderwritingExtraAnswer
                                Exit Select
                            Case puwNO
                                rbQ17No.Checked = True
                                HideAddlInfoRow("17")
                                Exit Select
                            Case Else
                                Throw New Exception("Unknown answer value (UW Code " & pu.PolicyUnderwritingCodeId & ")")
                        End Select
                        Exit Select
                    Case "9558"
                        Select Case pu.PolicyUnderwritingAnswer
                            Case puwYES
                                rbQ18Yes.Checked = True
                                Exit Select
                            Case puwNO
                                rbQ18No.Checked = True
                                Exit Select
                            Case Else
                                Throw New Exception("Unknown answer value (UW Code " & pu.PolicyUnderwritingCodeId & ")")
                        End Select
                        Exit Select
                    Case "9559"
                        Select Case pu.PolicyUnderwritingAnswer
                            Case puwYES
                                rbQ19Yes.Checked = True
                                ExpandQuestion("19")
                                Exit Select
                            Case puwNO
                                rbQ19No.Checked = True
                                CollapseQuestion("19")
                                CollapseQuestion("19A")
                                Exit Select
                            Case Else
                                Throw New Exception("Unknown answer value (UW Code " & pu.PolicyUnderwritingCodeId & ")")
                        End Select
                        Exit Select
                    Case "9560"
                        Select Case pu.PolicyUnderwritingAnswer
                            Case puwYES
                                rbQ19aYes.Checked = True
                                CollapseQuestion("19A")
                                Exit Select
                            Case puwNO
                                rbQ19aNo.Checked = True
                                If rbQ19Yes.Checked Then
                                    ExpandQuestion("19A")
                                Else
                                    CollapseQuestion("19A")
                                End If
                                Exit Select
                            Case Else
                                'Throw New Exception("Unknown answer value (UW Code " & pu.PolicyUnderwritingCodeId & ")")
                        End Select
                        Exit Select
                    Case "9561"
                        txtQ19bAddlInfo.Text = pu.PolicyUnderwritingExtraAnswer
                        Exit Select
                    Case "9562"
                        Select Case pu.PolicyUnderwritingAnswer
                            Case puwYES
                                rbQ20Yes.Checked = True
                                ShowAddlInfoRow("20")
                                txtQ20AddlInfo.Text = pu.PolicyUnderwritingExtraAnswer
                                Exit Select
                            Case puwNO
                                rbQ20No.Checked = True
                                HideAddlInfoRow("20")
                                Exit Select
                            Case Else
                                Throw New Exception("Unknown answer value (UW Code " & pu.PolicyUnderwritingCodeId & ")")
                        End Select
                        Exit Select
                    Case Else
                        Exit Select
                End Select
            Next

            ' Set Question #3: Principal Type of Farming
            'added for Task 62085 11/8/2022 KLJ
            If HasHobbyFarm = False Then
                SetPrincipalTypeOfFarmingValue()
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("DisplayExistingQuoteData", ex)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Set the value of Question 3: Principal Type of Farming based on the values in the Quote Object
    ''' Principal Type Of Farming is set at the beginning of the Quote process
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetPrincipalTypeOfFarmingValue()
        Try
            ddlQ3PrincipalTypeOfFarming.SelectedIndex = 0
            ddlQ3PrincipalTypeOfFarming.Enabled = False

            If Not QuoteObjectOK() Then Exit Sub
            If QuoteObj.Locations Is Nothing OrElse QuoteObj.Locations.Count < 1 Then Exit Sub

            Dim loc As QuickQuoteLocation = QuoteObj.Locations(0)

            ' Set the ddl item based on the Type of Farming on location 0
            '8/10/2018 note: may need to check 1st location for each state when applicable
            If loc.FarmTypeDairy Then
                ddlQ3PrincipalTypeOfFarming.SelectedValue = "Dairy"
            ElseIf loc.FarmTypeFieldCrops Then
                ddlQ3PrincipalTypeOfFarming.SelectedValue = "Field crops"
            ElseIf loc.FarmTypeFruits Then
                ddlQ3PrincipalTypeOfFarming.SelectedValue = "Fruit"
            ElseIf loc.FarmTypeGreenhouses Then
                ddlQ3PrincipalTypeOfFarming.SelectedValue = "Greenhouses"
            ElseIf loc.FarmTypeHorse Then
                ddlQ3PrincipalTypeOfFarming.SelectedValue = "Horses"
            ElseIf loc.FarmTypeLivestock Then
                ddlQ3PrincipalTypeOfFarming.SelectedValue = "Livestock"
            ElseIf loc.FarmTypePoultry Then
                ddlQ3PrincipalTypeOfFarming.SelectedValue = "Poultry"
            ElseIf loc.FarmTypeSwine Then
                ddlQ3PrincipalTypeOfFarming.SelectedValue = "Swine"
            ElseIf loc.FarmTypeVegetables Then
                ddlQ3PrincipalTypeOfFarming.SelectedValue = "Vegetables"
            ElseIf loc.FarmTypeHobby = False AndAlso HasHobbyFarm = False Then
                ' Value is not in list - enable the ddl for seletion
                ddlQ3PrincipalTypeOfFarming.Enabled = True
                ddlQ3PrincipalTypeOfFarming.SelectedIndex = 0
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("SetPrincipalTypeOfFarmingValue", ex)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Build a list of row controls for the background formatting
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub BuildRowList()
        Try
            RowList = New List(Of HtmlTableRow)

            RowList.Add(trQuestion1)
            RowList.Add(trQuestion2)
            RowList.Add(trQuestion3)
            RowList.Add(trQuestion4)
            RowList.Add(trQuestion5)
            RowList.Add(trQuestion6)
            RowList.Add(trQuestion7)
            RowList.Add(trQuestion8)
            RowList.Add(trQuestion9)
            RowList.Add(trQuestion10)
            RowList.Add(trQuestion10a)
            RowList.Add(trQuestion10b)
            RowList.Add(trQuestion10c)
            RowList.Add(trQuestion11)
            RowList.Add(trQuestion12)
            RowList.Add(trQuestion12a)
            RowList.Add(trQuestion12b)
            RowList.Add(trQuestion12c)
            RowList.Add(trQuestion12d)
            RowList.Add(trQuestion12e)
            RowList.Add(trQuestion13)
            RowList.Add(trQuestion13a)
            RowList.Add(trQuestion14)
            RowList.Add(trQuestion14a)
            RowList.Add(trQuestion14b)
            RowList.Add(trQuestion15)
            RowList.Add(trQuestion15a)
            RowList.Add(trQuestion16)
            RowList.Add(trQuestion17)
            RowList.Add(trQuestion18)
            RowList.Add(trQuestion19)
            RowList.Add(trQuestion19a)
            RowList.Add(trQuestion19b)
            RowList.Add(trQuestion20)

            Exit Sub
        Catch ex As Exception
            HandleError("BuildRowList", ex)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Removes the error formatting from all textboxes and dropdownlists in the input tables
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub RemoveErrorFormatting()
        Dim FormatControls As New List(Of Control)

        Try
            ' Hide all of the extra answer warning panels
            pnlQ10aAddlInfo.Visible = False
            pnlQ10AddlInfo.Visible = False
            pnlQ11AddlInfo.Visible = False
            pnlQ12aAddlInfo.Visible = False
            pnlQ12cAddlInfo.Visible = False
            pnlQ12eAddlInfo.Visible = False
            pnlQ14aAddlInfo.Visible = False
            pnlQ14bAddlInfo.Visible = False
            pnlQ17AddlInfo.Visible = False
            pnlQ19bAddlInfo.Visible = False
            pnlQ1AddlInfo.Visible = False
            pnlQ20AddlInfo.Visible = False
            pnlQ3AddlInfo.Visible = False
            pnlQ4AddlInfo.Visible = False
            pnlQ5AddlInfo.Visible = False
            pnlQ6AddlInfo.Visible = False
            pnlQ7AddlInfo.Visible = False
            pnlQ8AddlInfo.Visible = False
            pnlQ9AddlInfo.Visible = False
            pnlQ3AddlInfo.Visible = False

            ' hide all the asterisk labels
            lblQ10Asterisk.Visible = False
            lblQ10bAsterisk.Visible = False
            lblQ10cAsterisk.Visible = False
            lblQ11Asterisk.Visible = False
            lblQ12Asterisk.Visible = False
            lblQ12dAsterisk.Visible = False
            lblQ12eAsterisk.Visible = False
            lblQ13aAsterisk.Visible = False
            lblQ13Asterisk.Visible = False
            lblQ14Asterisk.Visible = False
            lblQ14bAsterisk.Visible = False
            lblQ15aAsterisk.Visible = False
            lblQ15Asterisk.Visible = False
            lblQ16Asterisk.Visible = False
            lblQ17Asterisk.Visible = False
            lblQ18Asterisk.Visible = False
            lblQ19aAsterisk.Visible = False
            lblQ19Asterisk.Visible = False
            lblQ20Asterisk.Visible = False
            lblQ4Asterisk.Visible = False
            lblQ6Asterisk.Visible = False
            lblQ7Asterisk.Visible = False
            lblQ8Asterisk.Visible = False
            lblQ9Asterisk.Visible = False

            ' Build a list of controls to remove formatting from
            FormatControls.Add(txtQ1AddlInfo)
            FormatControls.Add(ddlQ2PreviousCarrier)
            FormatControls.Add(txtQ2PreviousCarrier)
            If Not HasHobbyFarm Then
                FormatControls.Add(ddlQ3PrincipalTypeOfFarming)
            Else
                FormatControls.Add(txtQ3AddlInfo)
            End If
            FormatControls.Add(rbQ4No)
            FormatControls.Add(rbQ4Yes)
            FormatControls.Add(txtQ4AddlInfo)
            FormatControls.Add(txtQ5AddlInfo)
            FormatControls.Add(rbQ6No)
            FormatControls.Add(rbQ6Yes)
            FormatControls.Add(txtQ6AddlInfo)
            FormatControls.Add(rbQ7No)
            FormatControls.Add(rbQ7Yes)
            FormatControls.Add(txtQ7AddlInfo)
            FormatControls.Add(rbQ8No)
            FormatControls.Add(rbQ8Yes)
            FormatControls.Add(txtQ8AddlInfo)
            FormatControls.Add(rbQ9No)
            FormatControls.Add(rbQ9Yes)
            FormatControls.Add(txtQ9AddlInfo)
            FormatControls.Add(rbQ10No)
            FormatControls.Add(rbQ10Yes)
            FormatControls.Add(txtQ10AddlInfo)
            FormatControls.Add(txtQ10aAddlInfo)
            FormatControls.Add(rbQ10bNo)
            FormatControls.Add(rbQ10bYes)
            FormatControls.Add(rbQ10cNo)
            FormatControls.Add(rbQ10cYes)
            FormatControls.Add(rbQ11No)
            FormatControls.Add(rbQ11Yes)
            FormatControls.Add(txtQ11AddlInfo)
            FormatControls.Add(rbQ12No)
            FormatControls.Add(rbQ12Yes)
            FormatControls.Add(txtQ12aAddlInfo)
            FormatControls.Add(ddlQ12bAboveOrInGroundPool)
            FormatControls.Add(txtQ12cAddlInfo)
            FormatControls.Add(rbQ12dNo)
            FormatControls.Add(rbQ12dYes)
            FormatControls.Add(ddlQ12dSlideOrDivingBoard)
            FormatControls.Add(rbQ12eNo)
            FormatControls.Add(rbQ12eYes)
            FormatControls.Add(txtQ12eAddlInfo)
            FormatControls.Add(rbQ13No)
            FormatControls.Add(rbQ13Yes)
            FormatControls.Add(rbQ13aNo)
            FormatControls.Add(rbQ13aYes)
            FormatControls.Add(rbQ14No)
            FormatControls.Add(rbQ14Yes)
            FormatControls.Add(txtQ14aAddlInfo)
            FormatControls.Add(rbQ14bNo)
            FormatControls.Add(rbQ14bYes)
            FormatControls.Add(txtQ14bAddlInfo)
            FormatControls.Add(rbQ15No)
            FormatControls.Add(rbQ15Yes)
            FormatControls.Add(rbQ15aNo)
            FormatControls.Add(rbQ15aYes)
            FormatControls.Add(rbQ16No)
            FormatControls.Add(rbQ16Yes)
            FormatControls.Add(rbQ17No)
            FormatControls.Add(rbQ17Yes)
            FormatControls.Add(txtQ17AddlInfo)
            FormatControls.Add(rbQ18No)
            FormatControls.Add(rbQ18Yes)
            FormatControls.Add(rbQ19No)
            FormatControls.Add(rbQ19Yes)
            FormatControls.Add(rbQ19aNo)
            FormatControls.Add(rbQ19aYes)
            FormatControls.Add(txtQ19bAddlInfo)
            FormatControls.Add(rbQ20No)
            FormatControls.Add(rbQ20Yes)
            FormatControls.Add(txtQ20AddlInfo)

            ' Loop through the list and remove formatting
            For Each c As Control In FormatControls
                Select Case c.GetType()
                    Case GetType(TextBox)
                        Dim txt As TextBox = CType(c, TextBox)
                        txt.BorderStyle = BorderStyle.None
                        txt.BackColor = Nothing
                        Exit Select
                    Case GetType(DropDownList)
                        Dim ddl As DropDownList = CType(c, DropDownList)
                        ddl.BorderStyle = BorderStyle.None
                        ddl.BackColor = Nothing
                        Exit Select
                    Case GetType(RadioButton)
                        Dim rb As RadioButton = CType(c, RadioButton)
                        rb.BorderStyle = BorderStyle.None
                        rb.BackColor = Nothing
                        Exit Select
                    Case Else
                        Exit Select
                End Select
            Next

            Exit Sub
        Catch ex As Exception
            HandleError("RemoveErrorFormatting", ex)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Validates user input
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ValidData() As Boolean
        Dim errctls As New List(Of Control)
        Dim errMsgs As New List(Of String)
        Dim errAsterisks As New List(Of Label)
        Dim errAddlInfo As New List(Of Panel)
        Dim ErrColor As System.Drawing.Color = Drawing.Color.LightCoral
        Dim AlertMsg As String = ""

        Try
            ' Initialize
            rptErrors.DataSource = Nothing
            rptErrors.DataBind()
            pnlErrors.Visible = False
            RemoveErrorFormatting()
            HideMsg()

            ' All questions answered?
            If Not AllQuestionsAnswered() Then
                'ShowMsg("All Questions must be answered")
                errMsgs.Add("Please provide answers for ALL questions")
            End If

            ' QUESTION 1
            If txtQ1AddlInfo.Text = String.Empty Then
                errctls.Add(txtQ1AddlInfo)
                errMsgs.Add("Question1: Please provide an answer")
                errAddlInfo.Add(pnlQ1AddlInfo)
            End If

            '' QUESTION 2
            'If txtQ2PreviousCarrier.Visible Then
            '    ' Other selected, check the textbox value
            '    If txtQ2PreviousCarrier.Text.Trim = String.Empty Then
            '        errctls.Add(txtQ2PreviousCarrier)
            '        errMsgs.Add("Question 2: You must enter previous carrier name")
            '    End If
            'Else
            '    If ddlQ2PreviousCarrier.SelectedIndex <= 0 Then
            '        errctls.Add(ddlQ2PreviousCarrier)
            '        errMsgs.Add("Question 2: Select previous carrier")
            '    End If
            'End If

            ' QUESTION 2
            If ddlQ2PreviousCarrier.SelectedIndex <= 0 AndAlso txtQ2PreviousCarrier.Text.Trim = String.Empty Then
                errctls.Add(ddlQ2PreviousCarrier)
                errMsgs.Add("Question 2: Select previous carrier")
                trQuestion2AddlInfoRow_TEXTBOX.Attributes.Add("style", "display:none;")
                trQuestion2AddlInfoRow_DDL.Attributes.Add("style", "display:'';")
            ElseIf ddlQ2PreviousCarrier.SelectedValue = "OTHER" AndAlso txtQ2PreviousCarrier.Text.Trim = String.Empty Then
                errctls.Add(txtQ2PreviousCarrier)
                errMsgs.Add("Question 2: You must enter previous carrier name")
                trQuestion2AddlInfoRow_TEXTBOX.Attributes.Add("style", "display:'';")
                trQuestion2AddlInfoRow_DDL.Attributes.Add("style", "display:none;")
            End If

            ' QUESTION 3
            'added for Task 62085 11/7/2022 KLJ
            If HasHobbyFarm AndAlso String.IsNullOrWhiteSpace(txtQ3AddlInfo.Text) Then
                errctls.Add(txtQ3AddlInfo)
                errMsgs.Add("Question 3: Enter principal type of farming")
                errAddlInfo.Add(pnlQ3AddlInfo)
            End If
            If HasHobbyFarm = False AndAlso ddlQ3PrincipalTypeOfFarming.SelectedIndex <= 0 Then
                errctls.Add(ddlQ3PrincipalTypeOfFarming)
                errMsgs.Add("Question 3: Select principal type of farming")
            End If

            ' QUESTION 4
            If (Not rbQ4No.Checked) AndAlso (Not rbQ4Yes.Checked) Then
                errctls.Add(rbQ4No)
                errctls.Add(rbQ4Yes)
                errMsgs.Add("Question 4: Please provide an answer")
                errAsterisks.Add(lblQ4Asterisk)
            Else
                If rbQ4No.Checked Then
                    If txtQ4AddlInfo.Text.Trim = String.Empty Then
                        errctls.Add(txtQ4AddlInfo)
                        errMsgs.Add("Question 4: Please describe the premises")
                        errAddlInfo.Add(pnlQ4AddlInfo)
                    End If
                End If
            End If

            ' QUESTION 5
            If txtQ5AddlInfo.Text.Trim = String.Empty Then
                errctls.Add(txtQ5AddlInfo)
                errMsgs.Add("Question 5: Please provide an answer")
                errAddlInfo.Add(pnlQ5AddlInfo)
            End If

            ' QUESTION 6
            If (Not rbQ6No.Checked) AndAlso (Not rbQ6Yes.Checked) Then
                errctls.Add(rbQ6No)
                errctls.Add(rbQ6Yes)
                errMsgs.Add("Question 6: Please provide an answer")
                errAsterisks.Add(lblQ6Asterisk)
            Else
                If rbQ6Yes.Checked Then
                    If txtQ6AddlInfo.Text.Trim = String.Empty Then
                        errctls.Add(txtQ6AddlInfo)
                        errMsgs.Add("Question 6: Please explain other businesses")
                        errAddlInfo.Add(pnlQ6AddlInfo)
                    End If
                End If
            End If

            ' QUESTION 7
            If (Not rbQ7No.Checked) AndAlso (Not rbQ7Yes.Checked) Then
                errctls.Add(rbQ7No)
                errctls.Add(rbQ7Yes)
                errMsgs.Add("Question 7: Please provide an answer")
                errAsterisks.Add(lblQ7Asterisk)
            Else
                If rbQ7Yes.Checked Then
                    If txtQ7AddlInfo.Text.Trim = String.Empty Then
                        errctls.Add(txtQ7AddlInfo)
                        errMsgs.Add("Question 7: Please explain cancelled or non-renewed policies")
                        errAddlInfo.Add(pnlQ7AddlInfo)
                    End If
                End If
            End If

            ' QUESTION 8
            If (Not rbQ8No.Checked) AndAlso (Not rbQ8Yes.Checked) Then
                errctls.Add(rbQ8No)
                errctls.Add(rbQ8Yes)
                errMsgs.Add("Question 8: Please provide an answer")
                errAsterisks.Add(lblQ8Asterisk)
            Else
                If rbQ8Yes.Checked Then
                    If txtQ8AddlInfo.Text.Trim = String.Empty Then
                        errctls.Add(txtQ8AddlInfo)
                        errMsgs.Add("Question 8:  Describe vacant or unoccupied dwellings")
                        errAddlInfo.Add(pnlQ8AddlInfo)
                    End If
                End If
            End If

            ' QUESTION 9
            If (Not rbQ9No.Checked) AndAlso (Not rbQ9Yes.Checked) Then
                errctls.Add(rbQ9No)
                errctls.Add(rbQ9Yes)
                errMsgs.Add("Question 9: Please provide an answer")
                errAsterisks.Add(lblQ9Asterisk)
            Else
                If rbQ9Yes.Checked Then
                    If txtQ9AddlInfo.Text.Trim = String.Empty Then
                        errctls.Add(txtQ9AddlInfo)
                        errMsgs.Add("Question 9:  Please explain farm use or lease for organized recreational use")
                        errAddlInfo.Add(pnlQ8AddlInfo)
                    End If
                End If
            End If

            ' QUESTION 10
            If (Not rbQ10No.Checked) AndAlso (Not rbQ10Yes.Checked) Then
                errctls.Add(rbQ10No)
                errctls.Add(rbQ10Yes)
                errMsgs.Add("Question 10: Please provide an answer")
                errAsterisks.Add(lblQ10Asterisk)
            Else
                If rbQ10Yes.Checked Then
                    If txtQ10AddlInfo.Text.Trim = String.Empty Then
                        errctls.Add(txtQ10AddlInfo)
                        errMsgs.Add("Question 10:  Please provide amount of annual receipts")
                        errAddlInfo.Add(pnlQ10AddlInfo)
                    End If
                    If txtQ10aAddlInfo.Text.Trim = String.Empty Then
                        errctls.Add(txtQ10aAddlInfo)
                        errMsgs.Add("Question 10a: Describe the custom farming activities")
                        errAddlInfo.Add(pnlQ10aAddlInfo)
                    End If
                    If (Not rbQ10bNo.Checked) AndAlso (Not rbQ10bYes.Checked) Then
                        errctls.Add(rbQ10bNo)
                        errctls.Add(rbQ10bYes)
                        errMsgs.Add("Question 10b:  Please provide an answer")
                        errAsterisks.Add(lblQ10bAsterisk)
                    End If
                    If (Not rbQ10cNo.Checked) AndAlso (Not rbQ10cYes.Checked) Then
                        errctls.Add(rbQ10cNo)
                        errctls.Add(rbQ10cYes)
                        errMsgs.Add("Question 10c: Please provide an answer")
                        errAsterisks.Add(lblQ10cAsterisk)
                    End If
                End If
            End If

            ' QUESTION 11
            If (Not rbQ11No.Checked) AndAlso (Not rbQ11Yes.Checked) Then
                errctls.Add(rbQ11No)
                errctls.Add(rbQ11Yes)
                errMsgs.Add("Question 11: Please provide an answer")
                errAsterisks.Add(lblQ11Asterisk)
            Else
                If rbQ11Yes.Checked Then
                    If txtQ11AddlInfo.Text.Trim = String.Empty Then
                        errctls.Add(txtQ11AddlInfo)
                        errMsgs.Add("Question 11:  Describe public activities")
                        errAddlInfo.Add(pnlQ11AddlInfo)
                    End If
                End If
            End If

            ' QUESTION 12
            If (Not rbQ12No.Checked) AndAlso (Not rbQ12Yes.Checked) Then
                errctls.Add(rbQ12No)
                errctls.Add(rbQ12Yes)
                errMsgs.Add("Question 12: Please provide an answer")
                errAsterisks.Add(lblQ12Asterisk)
            Else
                If rbQ12Yes.Checked Then
                    If txtQ12aAddlInfo.Text.Trim = String.Empty Then
                        errctls.Add(txtQ12aAddlInfo)
                        errMsgs.Add("Question 12a:  Please describe swimming pool")
                        errAddlInfo.Add(pnlQ12aAddlInfo)
                    End If
                    If ddlQ12bAboveOrInGroundPool.SelectedIndex = 0 Then
                        errctls.Add(ddlQ12bAboveOrInGroundPool)
                        errMsgs.Add("Question 12b:  Please select above or in-ground pool")
                    End If
                    If txtQ12cAddlInfo.Text.Trim = String.Empty Then
                        errctls.Add(txtQ12cAddlInfo)
                        errMsgs.Add("Question 12c:  Please enter greatest depth of pool")
                        errAddlInfo.Add(pnlQ12cAddlInfo)
                    End If
                    If (Not rbQ12dNo.Checked) AndAlso (Not rbQ12dYes.Checked) Then
                        errctls.Add(rbQ12dNo)
                        errctls.Add(rbQ12dYes)
                        errMsgs.Add("Question 12d:Please provide an answer")
                        errAsterisks.Add(lblQ12dAsterisk)
                    Else
                        If rbQ12dYes.Checked Then
                            If ddlQ12dSlideOrDivingBoard.SelectedIndex <= 0 Then
                                errctls.Add(ddlQ12dSlideOrDivingBoard)
                                errMsgs.Add("Question 12d:  Please indicate whether a slide or diving board is present")
                            End If
                        End If
                    End If
                    If (Not rbQ12eNo.Checked) AndAlso (Not rbQ12eYes.Checked) Then
                        errctls.Add(rbQ12eNo)
                        errctls.Add(rbQ12eYes)
                        errMsgs.Add("Question 12e: Please provide an answer")
                        errAsterisks.Add(lblQ12eAsterisk)
                        'Else
                        '    If rbQ12eNo.Checked Then
                        '        If txtQ12eAddlInfo.Text.Trim = String.Empty Then
                        '            errctls.Add(txtQ12eAddlInfo)
                        '            errMsgs.Add("Question 12e:  Please describe pool fencing and/or pool cover and locking mechanisms")
                        '            errAddlInfo.Add(pnlQ12eAddlInfo)
                        '        End If
                        '    End If
                    End If
                End If
            End If

            ' QUESTION 13
            If (Not rbQ13No.Checked) AndAlso (Not rbQ13Yes.Checked) Then
                errctls.Add(rbQ13No)
                errctls.Add(rbQ13Yes)
                errMsgs.Add("Question 13: Please provide an answer")
                errAsterisks.Add(lblQ13Asterisk)
            Else
                If rbQ13Yes.Checked Then
                    If (Not rbQ13aNo.Checked) AndAlso (Not rbQ13aYes.Checked) Then
                        errctls.Add(rbQ13aNo)
                        errctls.Add(rbQ13aYes)
                        errMsgs.Add("Question 13a:  Please provide an answer")
                        errAsterisks.Add(lblQ13aAsterisk)
                    End If
                End If
            End If

            ' QUESTION 14
            If (Not rbQ14No.Checked) AndAlso (Not rbQ14Yes.Checked) Then
                errctls.Add(rbQ14No)
                errctls.Add(rbQ14Yes)
                errMsgs.Add("Question 14: Please provide an answer")
                errAsterisks.Add(lblQ14Asterisk)
            Else
                If rbQ14Yes.Checked Then
                    If txtQ14aAddlInfo.Text.Trim = String.Empty Then
                        errctls.Add(txtQ14aAddlInfo)
                        errMsgs.Add("Question 14a:  Please enter the number of dogs")
                        errAddlInfo.Add(pnlQ14aAddlInfo)
                    End If
                    If (Not rbQ14bNo.Checked) AndAlso (Not rbQ14bYes.Checked) Then
                        errctls.Add(rbQ14bNo)
                        errctls.Add(rbQ14bYes)
                        errMsgs.Add("Question 14b:  Please provide an answer")
                        errAsterisks.Add(lblQ14bAsterisk)
                    Else
                        If rbQ14bYes.Checked Then
                            If txtQ14bAddlInfo.Text.Trim = String.Empty Then
                                errctls.Add(txtQ14bAddlInfo)
                                errMsgs.Add("Question 14b:  Please describe each dog that meets the breed criteria")
                                errAddlInfo.Add(pnlQ14bAddlInfo)
                            End If
                        End If
                    End If
                End If
            End If

            ' QUESTION 15
            If (Not rbQ15No.Checked) AndAlso (Not rbQ15Yes.Checked) Then
                errctls.Add(rbQ15No)
                errctls.Add(rbQ15Yes)
                errMsgs.Add("Question 15: Please provide an answer")
                errAsterisks.Add(lblQ15Asterisk)
            Else
                If rbQ15Yes.Checked Then
                    If (Not rbQ15aNo.Checked) AndAlso (Not rbQ15aYes.Checked) Then
                        errctls.Add(rbQ15aYes)
                        errctls.Add(rbQ15aNo)
                        errMsgs.Add("Question 15a:  Please provide a yes or no answer")
                        errAsterisks.Add(lblQ15aAsterisk)
                        'Throw New Exception("Question 15a:  Please provide a yes or no answer")
                    End If
                End If
            End If

            ' QUESTION 16
            If (Not rbQ16No.Checked) AndAlso (Not rbQ16Yes.Checked) Then
                errctls.Add(rbQ16No)
                errctls.Add(rbQ16Yes)
                errMsgs.Add("Question 16: Please provide an answer")
                errAsterisks.Add(lblQ16Asterisk)
            End If

            ' QUESTION 17
            If (Not rbQ17No.Checked) AndAlso (Not rbQ17Yes.Checked) Then
                errctls.Add(rbQ17No)
                errctls.Add(rbQ17Yes)
                errMsgs.Add("Question 17: Please provide an answer")
                errAsterisks.Add(lblQ17Asterisk)
            Else
                If rbQ17Yes.Checked Then
                    If txtQ17AddlInfo.Text.Trim = String.Empty Then
                        errctls.Add(txtQ17AddlInfo)
                        errMsgs.Add("Question 17:  Please describe hazard")
                        errAddlInfo.Add(pnlQ17AddlInfo)
                    End If
                End If
            End If

            ' QUESTION 18
            If (Not rbQ18No.Checked) AndAlso (Not rbQ18Yes.Checked) Then
                errctls.Add(rbQ18No)
                errctls.Add(rbQ18Yes)
                errMsgs.Add("Question 18: Please provide an answer")
                errAsterisks.Add(lblQ18Asterisk)
            End If

            ' QUESTION 19
            If (Not rbQ19No.Checked) AndAlso (Not rbQ19Yes.Checked) Then
                errctls.Add(rbQ19No)
                errctls.Add(rbQ19Yes)
                errMsgs.Add("Question 19: Please provide an answer")
                errAsterisks.Add(lblQ19Asterisk)
            Else
                If rbQ19Yes.Checked Then
                    If (Not rbQ19aNo.Checked) AndAlso (Not rbQ19aYes.Checked) Then
                        errctls.Add(rbQ19aYes)
                        errctls.Add(rbQ19aNo)
                        errMsgs.Add("Question 19a:  Please provide a yes or no answer")
                        errAsterisks.Add(lblQ19aAsterisk)
                    Else
                        If rbQ19aNo.Checked Then
                            If txtQ19bAddlInfo.Text.Trim = String.Empty Then
                                errctls.Add(txtQ19bAddlInfo)
                                errMsgs.Add("Question 19b:  Please describe the exposures listed")
                                errAddlInfo.Add(pnlQ19bAddlInfo)
                            End If
                        End If
                    End If
                End If
            End If

            ' QUESTION 20
            If (Not rbQ20No.Checked) AndAlso (Not rbQ20Yes.Checked) Then
                errctls.Add(rbQ20No)
                errctls.Add(rbQ20Yes)
                errMsgs.Add("Question 20: Please provide an answer")
                errAsterisks.Add(lblQ20Asterisk)
            Else
                If rbQ20Yes.Checked Then
                    If txtQ20AddlInfo.Text = String.Empty Then
                        errctls.Add(txtQ20AddlInfo)
                        errMsgs.Add("Question 20:  Please list any other supporting Indiana Farmers Mutual policies")
                        errAddlInfo.Add(pnlQ20AddlInfo)
                    End If
                End If
            End If

            If errctls Is Nothing OrElse errctls.Count <= 0 Then
                ' All good
                ' Don't show this panel! MGB 10/15/15
                'pnlErrors.BackColor = Drawing.Color.PaleGreen
                'pnlErrors.Visible = True
                'lblErrorHeader.Text = "No Errors Found!"
                Return True
            Else
                ' Validation errors found - format the failed controls
                Dim tabindex As Integer = -1
                Dim firstone As Control = Nothing
                Dim tbl As New DataTable()
                Dim nr As DataRow = Nothing
                tbl.Columns.Add("ErrNum")
                tbl.Columns.Add("ErrMsg")

                For Each c As Control In errctls
                    Select Case c.GetType()
                        Case GetType(TextBox)
                            Dim txt As TextBox = CType(c, TextBox)
                            txt.BorderStyle = BorderStyle.Solid
                            txt.BorderWidth = 1
                            txt.BorderColor = ErrColor
                            'txt.BackColor = ErrColor
                            If tabindex = -1 OrElse txt.TabIndex < tabindex Then
                                tabindex = txt.TabIndex
                                firstone = c
                            End If
                            Exit Select
                        Case GetType(DropDownList)
                            Dim ddl As DropDownList = CType(c, DropDownList)
                            ddl.BorderStyle = BorderStyle.Solid
                            ddl.BorderColor = ErrColor
                            'ddl.BackColor = ErrColor
                            ddl.BorderWidth = 1
                            If tabindex = -1 OrElse ddl.TabIndex < tabindex Then
                                tabindex = ddl.TabIndex
                                firstone = c
                            End If
                            Exit Select
                        Case GetType(RadioButton)
                            Dim rb As RadioButton = CType(c, RadioButton)
                            'rb.BorderStyle = BorderStyle.Solid
                            'rb.BorderColor = ErrColor
                            'rb.BackColor = ErrColor
                            'rb.BorderWidth = 1
                            If tabindex = -1 OrElse rb.TabIndex < tabindex Then
                                tabindex = rb.TabIndex
                                firstone = c
                            End If
                            Exit Select
                        Case Else
                            Exit Select
                    End Select
                Next

                For Each l As Label In errAsterisks
                    l.Visible = True
                Next

                For Each p As Panel In errAddlInfo
                    p.Visible = True
                Next

                ' List the errors
                'AlertMsg = "Please correct the following errors: \n"
                'For i As Integer = 0 To errMsgs.Count - 1
                '    AlertMsg = AlertMsg & "* " & errMsgs(i) & "\n"
                '    nr = tbl.NewRow()
                '    nr("ErrNum") = "*"
                '    nr("ErrMsg") = errMsgs(i)
                '    '                    tbl.Rows.Add(nr)
                'Next
                'rptErrors.DataSource = tbl
                'rptErrors.DataBind()
                'pnlErrors.BackColor = ErrColor
                'pnlErrors.Visible = True
                'lblErrorHeader.Text = "Please correct the following errors:"

                ' Set focus to the control in the errorcontrols collection that has the lowest tab index
                If firstone IsNot Nothing Then
                    FocusFieldName = firstone.ClientID
                    hdnFocusField.Value = firstone.ClientID
                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "FocusControl", "FocusControl('" & firstone.ClientID & "');", True)
                End If

                'Alert(AlertMsg)

                Return False
            End If
        Catch ex As Exception
            ShowMsg(ex.Message)
            'HandleError("ValidData", ex)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' If all questions have answers returns true
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function AllQuestionsAnswered() As Boolean
        Try
            If txtQ1AddlInfo.Text.Trim = String.Empty Then Return False
            If ddlQ2PreviousCarrier.SelectedIndex = 0 Then Return False
            If Not HasHobbyFarm AndAlso ddlQ3PrincipalTypeOfFarming.SelectedIndex = 0 Then Return False
            If HasHobbyFarm AndAlso String.IsNullOrWhiteSpace(txtQ3AddlInfo.Text) Then Return False
            If (Not rbQ4No.Checked) AndAlso (Not rbQ4Yes.Checked) Then Return False
            If txtQ5AddlInfo.Text.Trim = String.Empty Then Return False
            If (Not rbQ6No.Checked) AndAlso (Not rbQ6Yes.Checked) Then Return False
            If (Not rbQ7No.Checked) AndAlso (Not rbQ7Yes.Checked) Then Return False
            If (Not rbQ8No.Checked) AndAlso (Not rbQ8Yes.Checked) Then Return False
            If (Not rbQ9No.Checked) AndAlso (Not rbQ9Yes.Checked) Then Return False
            If (Not rbQ10No.Checked) AndAlso (Not rbQ10Yes.Checked) Then Return False
            If (Not rbQ11No.Checked) AndAlso (Not rbQ11Yes.Checked) Then Return False
            If (Not rbQ12No.Checked) AndAlso (Not rbQ12Yes.Checked) Then Return False
            If (Not rbQ13No.Checked) AndAlso (Not rbQ13Yes.Checked) Then Return False
            If (Not rbQ14No.Checked) AndAlso (Not rbQ14Yes.Checked) Then Return False
            If (Not rbQ15No.Checked) AndAlso (Not rbQ15Yes.Checked) Then Return False
            If (Not rbQ16No.Checked) AndAlso (Not rbQ16Yes.Checked) Then Return False
            If (Not rbQ17No.Checked) AndAlso (Not rbQ17Yes.Checked) Then Return False
            If (Not rbQ18No.Checked) AndAlso (Not rbQ18Yes.Checked) Then Return False
            If (Not rbQ19No.Checked) AndAlso (Not rbQ19Yes.Checked) Then Return False
            If (Not rbQ20No.Checked) AndAlso (Not rbQ20Yes.Checked) Then Return False

            Return True
        Catch ex As Exception
            HandleError("AllQuestionsAnswered", ex)
            Return False
        End Try
    End Function

    Private Function AnyWarnings() As Boolean
        Try
            If rbQ4Yes.Checked _
                OrElse rbQ6Yes.Checked _
                OrElse rbQ7Yes.Checked _
                OrElse rbQ8Yes.Checked _
                OrElse rbQ9Yes.Checked _
                OrElse rbQ10Yes.Checked _
                OrElse rbQ10bYes.Checked _
                OrElse rbQ10cYes.Checked _
                OrElse rbQ11Yes.Checked _
                OrElse rbQ12Yes.Checked _
                OrElse rbQ12dYes.Checked _
                OrElse rbQ12eYes.Checked _
                OrElse rbQ13aYes.Checked _
                OrElse rbQ14bYes.Checked _
                OrElse rbQ14Yes.Checked _
                OrElse rbQ15Yes.Checked _
                OrElse rbQ15aYes.Checked _
                OrElse rbQ16Yes.Checked _
                OrElse rbQ17Yes.Checked _
                OrElse rbQ18Yes.Checked _
                OrElse rbQ19Yes.Checked _
                OrElse rbQ19aYes.Checked _
                OrElse rbQ20Yes.Checked Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            HandleError("AnyWarnings", ex)
            Return True
        End Try
    End Function

    ''' <summary>
    ''' Update the policy underwriting questions on the quote object
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SaveAnswersToQuoteObject() As Boolean
        Dim questions As New List(Of VR.Common.UWQuestions.VRUWQuestion)

        Try
            'Return True
            ' Create a new underwriting object for each question
            ' Answers (from policyunderwritinganswertype_id table):
            ' -1 Not Answered
            ' 0 N/A
            ' 1 YES
            ' 2 NO

            ' PolicyUnderwritingExtraAnswerType_id:
            ' 1 = Text
            ' 2 = Date
            ' 3 = Currency

            ' Make sure the quote object has been initialized
            If Not QuoteObjectOK() Then Throw New Exception("Quote object not initialized!")

            'added 8/10/2018 for multi-state
            Dim multiStateQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject) = Nothing
            If QuoteObj IsNot Nothing Then
                multiStateQuotes = QQHelper.MultiStateQuickQuoteObjects(QuoteObj) 'should always return at least QuoteObj in the list
            End If

            ' Remove any existing underwriting values
            'QuoteObj.PolicyUnderwritings = New List(Of QuickQuotePolicyUnderwriting)
            'updated 8/10/2018 for multi-state
            If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                For Each msq As QuickQuoteObject In multiStateQuotes
                    msq.PolicyUnderwritings = New List(Of QuickQuotePolicyUnderwriting)
                Next
            End If

            ' Get list of questions
            questions = IFM.VR.Common.UWQuestions.UWQuestions.GetFarmUnderwritingQuestions()
            If questions Is Nothing OrElse questions.Count <> 34 Then Throw New Exception("something is wrong with the questions list!")

            ' Loop through the row list and set each questions values
            For i As Integer = 0 To RowList.Count - 1
                Dim puw As New QuickQuotePolicyUnderwriting()
                Dim tr As HtmlTableRow = RowList(i)
                Dim q As IFM.VR.Common.UWQuestions.VRUWQuestion = Nothing

                puw.PolicyUnderwritingTabId = puwUWTabID
                puw.PolicyUnderwritingLevelId = puwUWLevelID

                Select Case tr.ID.ToUpper()
                    Case "TRQUESTION1"  ' Text answer only
                        q = questions(0)
                        puw.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId
                        ' Answer
                        puw.PolicyUnderwritingAnswerTypeId = puwAnswerTypeID
                        ' Extra Answer
                        puw.PolicyUnderwritingExtraAnswerTypeId = "1" ' Text
                        puw.PolicyUnderwritingExtraAnswer = txtQ1AddlInfo.Text
                        'QuoteObj.PolicyUnderwritings.Add(puw)
                        'updated 8/10/2018 for multi-state
                        If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                            For Each msq As QuickQuoteObject In multiStateQuotes
                                msq.PolicyUnderwritings.Add(puw)
                            Next
                        End If
                        Exit Select
                    Case "TRQUESTION2"  ' Dropdown answer only
                        q = questions(1)
                        puw.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId
                        ' Answer
                        puw.PolicyUnderwritingAnswerTypeId = puwAnswerTypeID
                        ' Extra Answer
                        puw.PolicyUnderwritingExtraAnswerTypeId = "1" ' Text
                        If txtQ2PreviousCarrier.Text.Trim <> "" Then
                            ' Text answer
                            puw.PolicyUnderwritingExtraAnswer = txtQ2PreviousCarrier.Text
                        Else
                            ' ddl answer
                            ' Use the carrier text instead of id
                            puw.PolicyUnderwritingExtraAnswer = ddlQ2PreviousCarrier.SelectedItem.Text
                        End If
                        'puw.PolicyUnderwritingExtraAnswer = ddlQ2PreviousCarrier.SelectedValue
                        'QuoteObj.PolicyUnderwritings.Add(puw)
                        'updated 8/10/2018 for multi-state
                        If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                            For Each msq As QuickQuoteObject In multiStateQuotes
                                msq.PolicyUnderwritings.Add(puw)
                            Next
                        End If
                        Exit Select
                    Case "TRQUESTION3"  ' Dropdown answer only
                        q = questions(2)
                        puw.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId
                        ' Answer
                        puw.PolicyUnderwritingAnswerTypeId = puwAnswerTypeID
                        ' Extra Answer
                        puw.PolicyUnderwritingExtraAnswerTypeId = "1" ' Text
                        If Not HasHobbyFarm Then
                            puw.PolicyUnderwritingExtraAnswer = ddlQ3PrincipalTypeOfFarming.SelectedItem.Text
                        Else
                            puw.PolicyUnderwritingExtraAnswer = txtQ3AddlInfo.Text
                        End If

                        'QuoteObj.PolicyUnderwritings.Add(puw)
                        'updated 8/10/2018 for multi-state
                        If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                            For Each msq As QuickQuoteObject In multiStateQuotes
                                msq.PolicyUnderwritings.Add(puw)
                            Next
                        End If
                        Exit Select
                    Case "TRQUESTION4"  ' Yes/No with extra answer
                        q = questions(3)
                        puw.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId
                        ' Answer
                        puw.PolicyUnderwritingAnswerTypeId = puwAnswerTypeID
                        If rbQ4Yes.Checked Then
                            puw.PolicyUnderwritingAnswer = puwYES  ' YES
                        Else
                            puw.PolicyUnderwritingAnswer = puwNO  ' NO
                            ' Extra Answer
                            ' Question 4 is the only one where a NO answer has an extra question value,
                            ' the rest have extra question value on YES
                            puw.PolicyUnderwritingExtraAnswerTypeId = "1" ' Text
                            puw.PolicyUnderwritingExtraAnswer = txtQ4AddlInfo.Text
                        End If
                        'QuoteObj.PolicyUnderwritings.Add(puw)
                        'updated 8/10/2018 for multi-state
                        If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                            For Each msq As QuickQuoteObject In multiStateQuotes
                                msq.PolicyUnderwritings.Add(puw)
                            Next
                        End If
                        Exit Select
                    Case "TRQUESTION5"  ' Text answer only
                        q = questions(4)
                        puw.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId
                        ' Answer
                        puw.PolicyUnderwritingAnswerTypeId = puwAnswerTypeID
                        ' Extra Answer
                        puw.PolicyUnderwritingExtraAnswerTypeId = "1" ' Text
                        puw.PolicyUnderwritingExtraAnswer = txtQ5AddlInfo.Text
                        'QuoteObj.PolicyUnderwritings.Add(puw)
                        'updated 8/10/2018 for multi-state
                        If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                            For Each msq As QuickQuoteObject In multiStateQuotes
                                msq.PolicyUnderwritings.Add(puw)
                            Next
                        End If
                        Exit Select
                    Case "TRQUESTION6"  ' Yes/No with extra answer
                        q = questions(5)
                        puw.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId
                        ' Answer
                        puw.PolicyUnderwritingAnswerTypeId = puwAnswerTypeID
                        If rbQ6Yes.Checked Then
                            puw.PolicyUnderwritingAnswer = puwYES  ' YES
                            ' Extra Answer
                            puw.PolicyUnderwritingExtraAnswerTypeId = "1" ' Text
                            puw.PolicyUnderwritingExtraAnswer = txtQ6AddlInfo.Text
                        Else
                            puw.PolicyUnderwritingAnswer = puwNO  ' NO
                        End If
                        'QuoteObj.PolicyUnderwritings.Add(puw)
                        'updated 8/10/2018 for multi-state
                        If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                            For Each msq As QuickQuoteObject In multiStateQuotes
                                msq.PolicyUnderwritings.Add(puw)
                            Next
                        End If
                        Exit Select
                    Case "TRQUESTION7"  ' Yes/No with extra answer
                        q = questions(6)
                        puw.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId
                        ' Answer
                        puw.PolicyUnderwritingAnswerTypeId = puwAnswerTypeID
                        If rbQ7Yes.Checked Then
                            puw.PolicyUnderwritingAnswer = puwYES  ' YES
                            ' Extra Answer
                            puw.PolicyUnderwritingExtraAnswerTypeId = "1" ' Text
                            puw.PolicyUnderwritingExtraAnswer = txtQ7AddlInfo.Text
                        Else
                            puw.PolicyUnderwritingAnswer = puwNO  ' NO
                        End If
                        'QuoteObj.PolicyUnderwritings.Add(puw)
                        'updated 8/10/2018 for multi-state
                        If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                            For Each msq As QuickQuoteObject In multiStateQuotes
                                msq.PolicyUnderwritings.Add(puw)
                            Next
                        End If
                        Exit Select
                    Case "TRQUESTION8"  ' Yes/No with extra answer
                        q = questions(7)
                        puw.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId
                        ' Answer
                        puw.PolicyUnderwritingAnswerTypeId = puwAnswerTypeID
                        If rbQ8Yes.Checked Then
                            puw.PolicyUnderwritingAnswer = puwYES  ' YES
                            ' Extra Answer
                            puw.PolicyUnderwritingExtraAnswerTypeId = "1" ' Text
                            puw.PolicyUnderwritingExtraAnswer = txtQ8AddlInfo.Text
                        Else
                            puw.PolicyUnderwritingAnswer = puwNO  ' NO
                        End If
                        'QuoteObj.PolicyUnderwritings.Add(puw)
                        'updated 8/10/2018 for multi-state
                        If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                            For Each msq As QuickQuoteObject In multiStateQuotes
                                msq.PolicyUnderwritings.Add(puw)
                            Next
                        End If
                        Exit Select
                    Case "TRQUESTION9"  ' Yes/No with extra answer
                        q = questions(8)
                        puw.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId
                        ' Answer
                        puw.PolicyUnderwritingAnswerTypeId = puwAnswerTypeID
                        If rbQ9Yes.Checked Then
                            puw.PolicyUnderwritingAnswer = puwYES  ' YES
                            ' Extra Answer
                            puw.PolicyUnderwritingExtraAnswerTypeId = "1" ' Text
                            puw.PolicyUnderwritingExtraAnswer = txtQ9AddlInfo.Text
                        Else
                            puw.PolicyUnderwritingAnswer = puwNO  ' NO
                        End If
                        'QuoteObj.PolicyUnderwritings.Add(puw)
                        'updated 8/10/2018 for multi-state
                        If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                            For Each msq As QuickQuoteObject In multiStateQuotes
                                msq.PolicyUnderwritings.Add(puw)
                            Next
                        End If
                        Exit Select
                    Case "TRQUESTION10"  ' Yes/No with extra answer
                        q = questions(9)
                        puw.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId
                        ' Answer
                        puw.PolicyUnderwritingAnswerTypeId = puwAnswerTypeID
                        If rbQ10Yes.Checked Then
                            puw.PolicyUnderwritingAnswer = puwYES  ' YES
                            ' Extra Answer
                            puw.PolicyUnderwritingExtraAnswerTypeId = "1" ' Text
                            puw.PolicyUnderwritingExtraAnswer = txtQ10AddlInfo.Text
                        Else
                            puw.PolicyUnderwritingAnswer = puwNO  ' NO
                        End If
                        'QuoteObj.PolicyUnderwritings.Add(puw)
                        'updated 8/10/2018 for multi-state
                        If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                            For Each msq As QuickQuoteObject In multiStateQuotes
                                msq.PolicyUnderwritings.Add(puw)
                            Next
                        End If
                        Exit Select
                    Case "TRQUESTION10A"  ' Text answer only
                        q = questions(10)
                        puw.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId
                        ' Answer
                        puw.PolicyUnderwritingAnswerTypeId = puwAnswerTypeID
                        puw.PolicyUnderwritingAnswer = txtQ10aAddlInfo.Text
                        ' Extra Answer
                        puw.PolicyUnderwritingExtraAnswerTypeId = "1" ' Text
                        puw.PolicyUnderwritingExtraAnswer = txtQ10aAddlInfo.Text
                        'QuoteObj.PolicyUnderwritings.Add(puw)
                        'updated 8/10/2018 for multi-state
                        If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                            For Each msq As QuickQuoteObject In multiStateQuotes
                                msq.PolicyUnderwritings.Add(puw)
                            Next
                        End If
                        Exit Select
                    Case "TRQUESTION10B"  ' Yes/No only
                        q = questions(11)
                        puw.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId
                        ' Answer
                        puw.PolicyUnderwritingAnswerTypeId = puwAnswerTypeID
                        If rbQ10bYes.Checked Then
                            puw.PolicyUnderwritingAnswer = puwYES  ' YES
                        Else
                            puw.PolicyUnderwritingAnswer = puwNO  ' NO
                        End If
                        'QuoteObj.PolicyUnderwritings.Add(puw)
                        'updated 8/10/2018 for multi-state
                        If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                            For Each msq As QuickQuoteObject In multiStateQuotes
                                msq.PolicyUnderwritings.Add(puw)
                            Next
                        End If
                        Exit Select
                    Case "TRQUESTION10C"  ' Yes/No only
                        q = questions(12)
                        puw.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId
                        ' Answer
                        puw.PolicyUnderwritingAnswerTypeId = puwAnswerTypeID
                        If rbQ10cYes.Checked Then
                            puw.PolicyUnderwritingAnswer = puwYES  ' YES
                        Else
                            puw.PolicyUnderwritingAnswer = puwNO  ' NO
                        End If
                        'QuoteObj.PolicyUnderwritings.Add(puw)
                        'updated 8/10/2018 for multi-state
                        If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                            For Each msq As QuickQuoteObject In multiStateQuotes
                                msq.PolicyUnderwritings.Add(puw)
                            Next
                        End If
                        Exit Select
                    Case "TRQUESTION11"  ' Yes/No with extra answer
                        q = questions(13)
                        puw.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId
                        ' Answer
                        puw.PolicyUnderwritingAnswerTypeId = puwAnswerTypeID
                        If rbQ11Yes.Checked Then
                            puw.PolicyUnderwritingAnswer = puwYES  ' YES
                            ' Extra Answer
                            puw.PolicyUnderwritingExtraAnswerTypeId = "1" ' Text
                            puw.PolicyUnderwritingExtraAnswer = txtQ11AddlInfo.Text
                        Else
                            puw.PolicyUnderwritingAnswer = puwNO  ' NO
                        End If
                        'QuoteObj.PolicyUnderwritings.Add(puw)
                        'updated 8/10/2018 for multi-state
                        If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                            For Each msq As QuickQuoteObject In multiStateQuotes
                                msq.PolicyUnderwritings.Add(puw)
                            Next
                        End If
                        Exit Select
                    Case "TRQUESTION12"  ' Yes/No only
                        q = questions(14)
                        puw.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId
                        ' Answer
                        puw.PolicyUnderwritingAnswerTypeId = puwAnswerTypeID
                        If rbQ12Yes.Checked Then
                            puw.PolicyUnderwritingAnswer = puwYES  ' YES
                        Else
                            puw.PolicyUnderwritingAnswer = puwNO  ' NO
                        End If
                        'QuoteObj.PolicyUnderwritings.Add(puw)
                        'updated 8/10/2018 for multi-state
                        If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                            For Each msq As QuickQuoteObject In multiStateQuotes
                                msq.PolicyUnderwritings.Add(puw)
                            Next
                        End If
                        Exit Select
                    Case "TRQUESTION12A"  ' Text answer only
                        q = questions(15)
                        puw.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId
                        ' Answer
                        puw.PolicyUnderwritingAnswerTypeId = puwAnswerTypeID
                        ' Extra Answer
                        puw.PolicyUnderwritingExtraAnswerTypeId = "1" ' Text
                        puw.PolicyUnderwritingExtraAnswer = txtQ12aAddlInfo.Text
                        'QuoteObj.PolicyUnderwritings.Add(puw)
                        'updated 8/10/2018 for multi-state
                        If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                            For Each msq As QuickQuoteObject In multiStateQuotes
                                msq.PolicyUnderwritings.Add(puw)
                            Next
                        End If
                        Exit Select
                    Case "TRQUESTION12B"  ' ddl answer only
                        q = questions(16)
                        puw.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId
                        ' Answer
                        puw.PolicyUnderwritingAnswerTypeId = puwAnswerTypeID
                        ' ddl value
                        puw.PolicyUnderwritingExtraAnswerTypeId = "1" ' Text
                        puw.PolicyUnderwritingExtraAnswer = ddlQ12bAboveOrInGroundPool.SelectedValue
                        'QuoteObj.PolicyUnderwritings.Add(puw)
                        'updated 8/10/2018 for multi-state
                        If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                            For Each msq As QuickQuoteObject In multiStateQuotes
                                msq.PolicyUnderwritings.Add(puw)
                            Next
                        End If
                        Exit Select
                    Case "TRQUESTION12C"  ' Text answer only
                        q = questions(17)
                        puw.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId
                        ' Answer
                        puw.PolicyUnderwritingAnswerTypeId = puwAnswerTypeID
                        ' Extra Answer
                        puw.PolicyUnderwritingExtraAnswerTypeId = "1" ' Text
                        puw.PolicyUnderwritingExtraAnswer = txtQ12cAddlInfo.Text
                        'QuoteObj.PolicyUnderwritings.Add(puw)
                        'updated 8/10/2018 for multi-state
                        If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                            For Each msq As QuickQuoteObject In multiStateQuotes
                                msq.PolicyUnderwritings.Add(puw)
                            Next
                        End If
                        Exit Select
                    Case "TRQUESTION12D"  ' Yes/No and ddl answer
                        q = questions(18)
                        puw.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId
                        ' Answer
                        puw.PolicyUnderwritingAnswerTypeId = puwAnswerTypeID
                        If rbQ12dYes.Checked Then
                            puw.PolicyUnderwritingAnswer = puwYES  ' YES
                        Else
                            puw.PolicyUnderwritingAnswer = puwNO  ' NO
                        End If
                        ' ddl value
                        puw.PolicyUnderwritingExtraAnswerTypeId = "1" ' Text
                        puw.PolicyUnderwritingExtraAnswer = ddlQ12dSlideOrDivingBoard.SelectedValue
                        'QuoteObj.PolicyUnderwritings.Add(puw)
                        'updated 8/10/2018 for multi-state
                        If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                            For Each msq As QuickQuoteObject In multiStateQuotes
                                msq.PolicyUnderwritings.Add(puw)
                            Next
                        End If
                        Exit Select
                    Case "TRQUESTION12E"  ' Yes/No with extra answer
                        q = questions(19)
                        puw.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId
                        ' Answer
                        puw.PolicyUnderwritingAnswerTypeId = puwAnswerTypeID
                        If rbQ12eYes.Checked Then
                            puw.PolicyUnderwritingAnswer = puwYES  ' YES
                        Else
                            puw.PolicyUnderwritingAnswer = puwNO  ' NO
                            ' Extra Answer
                            puw.PolicyUnderwritingExtraAnswerTypeId = "1" ' Text
                            puw.PolicyUnderwritingExtraAnswer = txtQ12eAddlInfo.Text
                        End If
                        'QuoteObj.PolicyUnderwritings.Add(puw)
                        'updated 8/10/2018 for multi-state
                        If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                            For Each msq As QuickQuoteObject In multiStateQuotes
                                msq.PolicyUnderwritings.Add(puw)
                            Next
                        End If
                        Exit Select
                    Case "TRQUESTION13"  ' Yes/No only
                        q = questions(20)
                        puw.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId
                        ' Answer
                        puw.PolicyUnderwritingAnswerTypeId = puwAnswerTypeID
                        If rbQ13Yes.Checked Then
                            puw.PolicyUnderwritingAnswer = puwYES  ' YES
                        Else
                            puw.PolicyUnderwritingAnswer = puwNO  ' NO
                        End If
                        'QuoteObj.PolicyUnderwritings.Add(puw)
                        'updated 8/10/2018 for multi-state
                        If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                            For Each msq As QuickQuoteObject In multiStateQuotes
                                msq.PolicyUnderwritings.Add(puw)
                            Next
                        End If
                        Exit Select
                    Case "TRQUESTION13A"  ' Yes/No only
                        q = questions(21)
                        puw.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId
                        ' Answer
                        puw.PolicyUnderwritingAnswerTypeId = puwAnswerTypeID
                        If rbQ13aYes.Checked Then
                            puw.PolicyUnderwritingAnswer = puwYES  ' YES
                        Else
                            puw.PolicyUnderwritingAnswer = puwNO  ' NO
                        End If
                        'QuoteObj.PolicyUnderwritings.Add(puw)
                        'updated 8/10/2018 for multi-state
                        If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                            For Each msq As QuickQuoteObject In multiStateQuotes
                                msq.PolicyUnderwritings.Add(puw)
                            Next
                        End If
                        Exit Select
                    Case "TRQUESTION14"  ' Yes/No only
                        q = questions(22)
                        puw.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId
                        ' Answer
                        puw.PolicyUnderwritingAnswerTypeId = puwAnswerTypeID
                        If rbQ14Yes.Checked Then
                            puw.PolicyUnderwritingAnswer = puwYES  ' YES
                        Else
                            puw.PolicyUnderwritingAnswer = puwNO  ' NO
                        End If
                        'QuoteObj.PolicyUnderwritings.Add(puw)
                        'updated 8/10/2018 for multi-state
                        If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                            For Each msq As QuickQuoteObject In multiStateQuotes
                                msq.PolicyUnderwritings.Add(puw)
                            Next
                        End If
                        Exit Select
                    Case "TRQUESTION14A"  ' Text answer only
                        q = questions(23)
                        puw.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId
                        ' Answer
                        puw.PolicyUnderwritingAnswerTypeId = puwAnswerTypeID
                        ' Extra Answer
                        puw.PolicyUnderwritingExtraAnswerTypeId = "1" ' Text
                        puw.PolicyUnderwritingExtraAnswer = txtQ14aAddlInfo.Text
                        'QuoteObj.PolicyUnderwritings.Add(puw)
                        'updated 8/10/2018 for multi-state
                        If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                            For Each msq As QuickQuoteObject In multiStateQuotes
                                msq.PolicyUnderwritings.Add(puw)
                            Next
                        End If
                        Exit Select
                    Case "TRQUESTION14B"  ' Yes/No with extra answer
                        q = questions(24)
                        puw.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId
                        ' Answer
                        puw.PolicyUnderwritingAnswerTypeId = puwAnswerTypeID
                        If rbQ14bYes.Checked Then
                            puw.PolicyUnderwritingAnswer = puwYES  ' YES
                            ' Extra Answer
                            puw.PolicyUnderwritingExtraAnswerTypeId = "1" ' Text
                            puw.PolicyUnderwritingExtraAnswer = txtQ14bAddlInfo.Text
                        Else
                            puw.PolicyUnderwritingAnswer = puwNO  ' NO
                        End If
                        'QuoteObj.PolicyUnderwritings.Add(puw)
                        'updated 8/10/2018 for multi-state
                        If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                            For Each msq As QuickQuoteObject In multiStateQuotes
                                msq.PolicyUnderwritings.Add(puw)
                            Next
                        End If
                        Exit Select
                    Case "TRQUESTION15"  ' Yes/No only
                        q = questions(25)
                        puw.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId
                        ' Answer
                        puw.PolicyUnderwritingAnswerTypeId = puwAnswerTypeID
                        If rbQ15Yes.Checked Then
                            puw.PolicyUnderwritingAnswer = puwYES  ' YES
                        Else
                            puw.PolicyUnderwritingAnswer = puwNO  ' NO
                        End If
                        'QuoteObj.PolicyUnderwritings.Add(puw)
                        'updated 8/10/2018 for multi-state
                        If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                            For Each msq As QuickQuoteObject In multiStateQuotes
                                msq.PolicyUnderwritings.Add(puw)
                            Next
                        End If
                        Exit Select
                    Case "TRQUESTION15A"  ' Yes/No only
                        q = questions(26)
                        puw.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId
                        ' Answer
                        puw.PolicyUnderwritingAnswerTypeId = puwAnswerTypeID
                        If rbQ15aYes.Checked Then
                            puw.PolicyUnderwritingAnswer = puwYES  ' YES
                        Else
                            puw.PolicyUnderwritingAnswer = puwNO  ' NO
                        End If
                        'QuoteObj.PolicyUnderwritings.Add(puw)
                        'updated 8/10/2018 for multi-state
                        If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                            For Each msq As QuickQuoteObject In multiStateQuotes
                                msq.PolicyUnderwritings.Add(puw)
                            Next
                        End If
                        Exit Select
                    Case "TRQUESTION16"  ' Yes/No only
                        q = questions(27)
                        puw.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId
                        ' Answer
                        puw.PolicyUnderwritingAnswerTypeId = puwAnswerTypeID
                        If rbQ16Yes.Checked Then
                            puw.PolicyUnderwritingAnswer = puwYES  ' YES
                        Else
                            puw.PolicyUnderwritingAnswer = puwNO  ' NO
                        End If
                        'QuoteObj.PolicyUnderwritings.Add(puw)
                        'updated 8/10/2018 for multi-state
                        If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                            For Each msq As QuickQuoteObject In multiStateQuotes
                                msq.PolicyUnderwritings.Add(puw)
                            Next
                        End If
                        Exit Select
                    Case "TRQUESTION17"  ' Yes/No with extra answer
                        q = questions(28)
                        puw.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId
                        ' Answer
                        puw.PolicyUnderwritingAnswerTypeId = puwAnswerTypeID
                        If rbQ17Yes.Checked Then
                            puw.PolicyUnderwritingAnswer = puwYES  ' YES
                            ' Extra Answer
                            puw.PolicyUnderwritingExtraAnswerTypeId = "1" ' Text
                            puw.PolicyUnderwritingExtraAnswer = txtQ17AddlInfo.Text
                        Else
                            puw.PolicyUnderwritingAnswer = puwNO  ' NO
                        End If
                        'QuoteObj.PolicyUnderwritings.Add(puw)
                        'updated 8/10/2018 for multi-state
                        If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                            For Each msq As QuickQuoteObject In multiStateQuotes
                                msq.PolicyUnderwritings.Add(puw)
                            Next
                        End If
                        Exit Select
                    Case "TRQUESTION18"  ' Yes/No only
                        q = questions(29)
                        puw.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId
                        ' Answer
                        puw.PolicyUnderwritingAnswerTypeId = puwAnswerTypeID
                        If rbQ18Yes.Checked Then
                            puw.PolicyUnderwritingAnswer = puwYES  ' YES
                        Else
                            puw.PolicyUnderwritingAnswer = puwNO  ' NO
                        End If
                        'QuoteObj.PolicyUnderwritings.Add(puw)
                        'updated 8/10/2018 for multi-state
                        If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                            For Each msq As QuickQuoteObject In multiStateQuotes
                                msq.PolicyUnderwritings.Add(puw)
                            Next
                        End If
                        Exit Select
                    Case "TRQUESTION19"  ' Yes/No only
                        q = questions(30)
                        puw.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId
                        ' Answer
                        puw.PolicyUnderwritingAnswerTypeId = puwAnswerTypeID
                        If rbQ19Yes.Checked Then
                            puw.PolicyUnderwritingAnswer = puwYES  ' YES
                        Else
                            puw.PolicyUnderwritingAnswer = puwNO  ' NO
                        End If
                        'QuoteObj.PolicyUnderwritings.Add(puw)
                        'updated 8/10/2018 for multi-state
                        If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                            For Each msq As QuickQuoteObject In multiStateQuotes
                                msq.PolicyUnderwritings.Add(puw)
                            Next
                        End If
                        Exit Select
                    Case "TRQUESTION19A"  ' Yes/No only
                        q = questions(31)
                        puw.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId
                        ' Answer
                        puw.PolicyUnderwritingAnswerTypeId = puwAnswerTypeID
                        If rbQ19aYes.Checked Then
                            puw.PolicyUnderwritingAnswer = puwYES  ' YES
                        Else 'If rbQ19aNo.Checked
                            puw.PolicyUnderwritingAnswer = puwNO  ' NO
                        End If
                        'QuoteObj.PolicyUnderwritings.Add(puw)
                        'updated 8/10/2018 for multi-state
                        If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                            For Each msq As QuickQuoteObject In multiStateQuotes
                                msq.PolicyUnderwritings.Add(puw)
                            Next
                        End If
                        Exit Select
                    Case "TRQUESTION19B"  ' Text answer only
                        q = questions(32)
                        puw.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId
                        ' Answer
                        puw.PolicyUnderwritingAnswerTypeId = puwAnswerTypeID
                        ' Extra Answer
                        puw.PolicyUnderwritingExtraAnswerTypeId = "1" ' Text
                        puw.PolicyUnderwritingExtraAnswer = txtQ19bAddlInfo.Text
                        'QuoteObj.PolicyUnderwritings.Add(puw)
                        'updated 8/10/2018 for multi-state
                        If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                            For Each msq As QuickQuoteObject In multiStateQuotes
                                msq.PolicyUnderwritings.Add(puw)
                            Next
                        End If
                        Exit Select
                    Case "TRQUESTION20"  ' Yes/No with extra answer
                        q = questions(33)
                        puw.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId
                        ' Answer
                        puw.PolicyUnderwritingAnswerTypeId = puwAnswerTypeID
                        If rbQ20Yes.Checked Then
                            puw.PolicyUnderwritingAnswer = puwYES  ' YES
                            ' Extra Answer
                            puw.PolicyUnderwritingExtraAnswerTypeId = "1" ' Text
                            puw.PolicyUnderwritingExtraAnswer = txtQ20AddlInfo.Text
                        Else
                            puw.PolicyUnderwritingAnswer = puwNO  ' NO
                        End If
                        'QuoteObj.PolicyUnderwritings.Add(puw)
                        'updated 8/10/2018 for multi-state
                        If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                            For Each msq As QuickQuoteObject In multiStateQuotes
                                msq.PolicyUnderwritings.Add(puw)
                            Next
                        End If
                        Exit Select
                End Select
            Next

            ' Type of Farming (Question 3)
            '8/10/2018 note: may need to update 1st location for each state
            Dim loc As QuickQuoteLocation = QuoteObj.Locations(0)
            '  If TypeOfFarmingChanged Then
            ' Remove all FarmType values
            loc.FarmTypeBees = False
            loc.FarmTypeDairy = False
            loc.FarmTypeFeedLot = False
            loc.FarmTypeFieldCrops = False
            loc.FarmTypeFlowers = False
            loc.FarmTypeFruits = False
            loc.FarmTypeFurbearingAnimals = False
            loc.FarmTypeGreenhouses = False
            loc.FarmTypeHobby = False
            loc.FarmTypeHorse = False
            loc.FarmTypeLivestock = False
            loc.FarmTypeMushrooms = False
            loc.FarmTypeNurseryStock = False
            loc.FarmTypeNuts = False
            loc.FarmTypeOtherDescription = False
            loc.FarmTypePoultry = False
            loc.FarmTypeSod = False
            loc.FarmTypeSwine = False
            loc.FarmTypeTobacco = False
            loc.FarmTypeTurkey = False
            loc.FarmTypeVegetables = False
            loc.FarmTypeVineyards = False
            loc.FarmTypeWorms = False

            If ddlQ3PrincipalTypeOfFarming.SelectedValue = "Dairy" Then
                loc.FarmTypeDairy = True
            ElseIf ddlQ3PrincipalTypeOfFarming.SelectedValue = "Field crops" Then
                loc.FarmTypeFieldCrops = True
            ElseIf ddlQ3PrincipalTypeOfFarming.SelectedValue = "Fruit" Then
                loc.FarmTypeFruits = True
            ElseIf ddlQ3PrincipalTypeOfFarming.SelectedValue = "Greenhouses" Then
                loc.FarmTypeGreenhouses = True
            ElseIf ddlQ3PrincipalTypeOfFarming.SelectedValue = "Horses" Then
                loc.FarmTypeHorse = True
            ElseIf ddlQ3PrincipalTypeOfFarming.SelectedValue = "Livestock" Then
                loc.FarmTypeLivestock = True
            ElseIf ddlQ3PrincipalTypeOfFarming.SelectedValue = "Poultry" Then
                loc.FarmTypePoultry = True
            ElseIf ddlQ3PrincipalTypeOfFarming.SelectedValue = "Swine" Then
                loc.FarmTypeSwine = True
            ElseIf ddlQ3PrincipalTypeOfFarming.SelectedValue = "Vegetables" Then
                loc.FarmTypeVegetables = True
            ElseIf HasHobbyFarm Then
                ' Must add hobby farm credit
                loc.FarmTypeHobby = True
                loc.HobbyFarmCredit = True
            End If
            'End If

            DisplayExistingQuoteData()

            Return True
        Catch ex As Exception
            HandleError("SaveAnswersToQuoteObject", ex)
            Return False
        End Try
    End Function

#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim _script = DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager
        Dim WarningMessage As String = "The user does not have the authority to bind coverage for this risk.  Please refer to your Farm Underwriter."

        Try

            _script.AddScriptLine("$(""#UWQuestionsDiv"").accordion({collapsible: false});")

            BuildRowList()

            If Not IsPostBack Then
                InitializeFormQuestions()
                LoadStaticData()
                DisplayExistingQuoteData()
            End If

            RemoveErrorFormatting()

            Exit Sub
        Catch ex As Exception
            HandleError("Page LOAD", ex)
            Exit Sub
        End Try

    End Sub

    ''' <summary>
    ''' Cancel button
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Try
            If Not ValidData()
                DisplayAdditionalInfoAndQuestions()
                Exit Sub
            End If

            'If Not ValidData() Then Exit Sub
            ''If AnyYESAnswers() Then
            ''    Dim str As String = "alert('The user does not have the authority to bind coverage for this risk.  Please refer to your Farm Underwriter.');"
            ''    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "msg" & DateTime.Now.ToShortDateString(), str, True)
            ''End If

            ' Save answers to the local object
            ' The called routine below handles it's own errors
            If Not SaveAnswersToQuoteObject() Then Exit Sub

            ' Raise the save event

            RaiseEvent SaveRequested(0, "ctlUWQuestionsFARM")
            Exit Sub
        Catch ex As Exception
            HandleError("btnCancel_Click", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub DisplayAdditionalInfoAndQuestions()
        If rbQ4No.Checked Then
            ShowAddlInfoRow("4")
        Else
            HideAddlInfoRow("4")
        End If
        If rbQ6Yes.Checked Then
            ShowAddlInfoRow("6")
        Else
            HideAddlInfoRow("6")
        End If
        If rbQ7Yes.Checked Then
            ShowAddlInfoRow("7")
        Else
            HideAddlInfoRow("7")
        End If
        If rbQ8Yes.Checked Then
            ShowAddlInfoRow("8")
        Else
            HideAddlInfoRow("8")
        End If
        If rbQ9Yes.Checked Then
            ShowAddlInfoRow("9")
        Else
            HideAddlInfoRow("9")
        End If
        If rbQ10Yes.Checked Then
            ShowAddlInfoRow("10")
            ExpandQuestion("10")
        Else
            HideAddlInfoRow("10")
            CollapseQuestion("10")
        End If
        If rbQ11Yes.Checked Then
            ShowAddlInfoRow("11")
        Else
            HideAddlInfoRow("11")
        End If
        If rbQ12Yes.Checked Then
            ExpandQuestion("12")
            If rbQ12DYes.Checked Then
                ShowAddlInfoRow("12D")
            Else
                HideAddlInfoRow("12D")
            End If
        Else
            CollapseQuestion("12")
        End If
        If rbQ13Yes.Checked Then
            ExpandQuestion("13")
        Else
            CollapseQuestion("13")
        End If
        If rbQ14Yes.Checked Then
            ExpandQuestion("14")
            If rbQ14bYes.Checked Then
                ShowAddlInfoRow("14B")
            Else
                HideAddlInfoRow("14B")
            End If
        Else
            CollapseQuestion("14")
        End If
        If rbQ15Yes.Checked Then
            ExpandQuestion("15")
        Else
            CollapseQuestion("15")
        End If
        If rbQ17Yes.Checked Then
            ShowAddlInfoRow("17")
        Else
            HideAddlInfoRow("17")
        End If
        If rbQ19aNo.Checked Then
            ExpandQuestion("19A")
        Else
            CollapseQuestion("19A")
        End If
        If rbQ19Yes.Checked Then
            ExpandQuestion("19")
        Else
            CollapseQuestion("19")
            CollapseQuestion("19A")
        End If
        If rbQ20Yes.Checked Then
            ShowAddlInfoRow("20")
        Else
            HideAddlInfoRow("20")
        End If
    End Sub

    ''' <summary>
    ''' Continue button
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnContinue_Click(sender As Object, e As EventArgs) Handles btnContinue.Click
        Try
            If Not ValidData() Then
                DisplayAdditionalInfoAndQuestions()
                Exit Sub
            End If
            'If Not ValidData() Then Exit Sub
            ''If AnyYESAnswers() Then
            ''    Dim str As String = "alert('The user does not have the authority to bind coverage for this risk.  Please refer to your Farm Underwriter.');"
            ''    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "msg" & DateTime.Now.ToShortDateString(), str, True)
            ''End If

            ' Save answers to the local object
            ' The called routine below handles it's own errors
            If Not SaveAnswersToQuoteObject() Then Exit Sub

            ' Raise the save event

            RaiseEvent SaveRequested(0, "ctlUWQuestionsFARM")
            RaiseEvent RequestNavigationToApplication(Nothing, Nothing)

            Exit Sub
        Catch ex As Exception
            HandleError("btnContinue_Click", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub rptErrors_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptErrors.ItemDataBound
        Dim lblNum As Label = Nothing
        Dim lblMsg As Label = Nothing

        Try
            lblNum = e.Item.FindControl("lblErrorNumber")
            If lblNum Is Nothing Then Throw New Exception("Number label not found")
            lblMsg = e.Item.FindControl("lblErrorMessage")
            If lblNum Is Nothing Then Throw New Exception("Message label not found")

            lblNum.Text = e.Item.DataItem("ErrNum").ToString()
            lblMsg.Text = e.Item.DataItem("ErrMsg").ToString()

            Exit Sub
        Catch ex As Exception
            HandleError("rptErrors_ItemDataBound", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub ddlQ3PrincipalTypeOfFarming_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlQ3PrincipalTypeOfFarming.SelectedIndexChanged
        Try
            If ddlQ3PrincipalTypeOfFarming.SelectedIndex <= 0 Then Exit Sub

            Exit Sub
        Catch ex As Exception
            HandleError("ddlQ3PrincipalTypeOfFarming_SelectedIndexChanged", ex)
            Exit Sub
        End Try
    End Sub

#End Region

End Class