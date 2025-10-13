Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Web.Script.Services
Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.DFR

Public Class ctlUWQuestions
    Inherits System.Web.UI.UserControl

    ' ************************************************************************************************
    ' * TO ADD A NEW LOB TO THIS CONTROL:
    ' ************************************************************************************************
    '   1. Update the LoadUWQuestionAnswers routine, add the LOB and create a LoadPersonalXXXXUWQuestionAnswers
    '       routine to load the answers onto the form for the new LOB.
    '   2. Go to the IFM.VR.COMMON.UWQuestions.vb module and add a routine to get the new LOB's UW questions.
    '       See routine GetPersonalAutoUnderwritingQuestions for an example.
    '   3. Update the InitializeQuestions routine.  Add the LOB and call the new routine you added in step 3.
    '   4. Update the GetPolicyUnderwritingCodeID routine to return the appropriate codes for the new LOB.
    '   5. Update the GetQuestion routine.  Create a GetQuestion<LOB> routine to get the appropriate question
    '       object for the new LOB.
    '   6. Update event handler btnSubmit_Click.  Make sure the correct save event is being raised for the
    '       new LOB's calling page.
    '   7. Add the LOB type to Page LOAD where it sets the myLOB variable.
    '   8. Update the rptUWQ.ItemDataBound event to handle the new LOB
    ' ************************************************************************************************
#Region "Declarations"
    Private Const ClassName As String = "ctlPersonalAutoUWQuestionsFull"

    Private msgQuoteObjNothing As String = "Quote Object is Nothing"
    Private msgLOBNotSupported As String = "LOB Not Supported!"

    Private TabIndex As Integer = 0
#End Region

#Region "Public Events"
    Public Event PersonalAutoUWQuestionsSaved(ByVal sender As Object, ByVal QuoteID As String)
    Public Event PersonalAutoUWQuestionsSaveFailed(ByVal sender As Object, ByVal QuoteID As String)
    Public Event PersonalAutoUWQuestionsLoadFailed(ByVal sender As Object, ByVal ErrMsg As String)
    Public Event SaveRequested(ByVal index As Integer, ByVal WhichControl As String)
    Public Event RequestNavigationToApplication(ByVal sender As Object, ByVal QuoteID As String)

    'added 2/28/2019
    Public Event PersonalAutoUWQuestionsSaved_Endorsements(ByVal sender As Object, ByVal PolicyId As Integer, ByVal PolicyImageNum As Integer)
    Public Event PersonalAutoUWQuestionsSaveFailed_Endorsements(ByVal sender As Object, ByVal PolicyId As Integer, ByVal PolicyImageNum As Integer)
    Public Event RequestNavigationToApplication_Endorsements(ByVal sender As Object, ByVal PolicyId As Integer, ByVal PolicyImageNum As Integer)
    Public Event RequestNavigationToApplication_ReadOnly(ByVal sender As Object, ByVal PolicyId As Integer, ByVal PolicyImageNum As Integer)
#End Region

#Region "Properties"

    ''' <summary>
    ''' The QuoteId is read from the url parameter 'quoteid' or the routing value 'quoteid'
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property QuoteId As String
        Get
            ' TEST CODE ONLY!!
            'Return "25506"

            ' USE THIS CODE FOR REAL
            If Request.QueryString("quoteid") IsNot Nothing Then
                Return Request.QueryString("quoteid")
            End If
            If Page.RouteData.Values("quoteid") IsNot Nothing Then
                Return Page.RouteData.Values("quoteid").ToString()
            End If
            Return ""
        End Get
    End Property

    Public ReadOnly Property IsAppPageMode As Boolean
        Get
            If TypeOf Me.Page Is VR3AutoApp Then
                Return True
            End If
            Return False
        End Get
    End Property

    Private _QuoteObj As QuickQuoteObject = Nothing
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
                ' TEST CODE ONLY - PULLS THIS SPECIFIC QUOTE FOR TESTING
                'Return VR.Common.QuoteSave.QuoteSaveHelpers.GetQuoteById(QuickQuoteObject.QuickQuoteLobType.None, "25506", errCreateQSO, True, QuickQuoteXML.QuickQuoteSaveType.AppGap)

                ' THIS IS THE METHOD YOU WANT TO USE FOR REALS
                If String.IsNullOrWhiteSpace(ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/18/2019; original logic in ELSE
                    Return VR.Common.QuoteSave.QuoteSaveHelpers.GetReadOnlyImageFromAnywhere(ReadOnlyPolicyId, ReadOnlyPolicyImageNum, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.AppGap, errorMessage:=errCreateQSO)
                ElseIf String.IsNullOrWhiteSpace(EndorsementPolicyIdAndImageNum) = False Then
                    Return VR.Common.QuoteSave.QuoteSaveHelpers.GetEndorsementQuoteFromAnywhere(EndorsementPolicyId, EndorsementPolicyImageNum, saveTypeView:=QuickQuoteXML.QuickQuoteSaveType.AppGap, errorMessage:=errCreateQSO)
                Else
                    Return VR.Common.QuoteSave.QuoteSaveHelpers.GetQuoteById(QuickQuoteObject.QuickQuoteLobType.None, QuoteId, errCreateQSO, True, QuickQuoteXML.QuickQuoteSaveType.AppGap)
                End If
            Catch ex As Exception
                HandleError("QuoteObj GET", ex)
                Return _QuoteObj
            Finally

            End Try
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

    Public Function GetTabIndex() As Integer
        TabIndex += 1
        Return TabIndex
    End Function


    'added 2/8/18 for HOM Upgrade - MLW
    Public Function GetHomeVersion(quote As QuickQuoteObject) As String
        Dim qqh As New QuickQuote.CommonMethods.QuickQuoteHelperClass
        Dim effectiveDate As DateTime
        If quote IsNot Nothing Then
            If quote.EffectiveDate IsNot Nothing AndAlso quote.EffectiveDate <> String.Empty Then
                effectiveDate = quote.EffectiveDate
            Else
                effectiveDate = Now()
            End If
        Else
            effectiveDate = Now()
        End If
        If qqh.doUseNewVersionOfLOB(quote, QuickQuote.CommonMethods.QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade) = True Then
            Return "After20180701"
        Else
            Return "Before20180701"
        End If
    End Function

    ''' <summary>
    ''' Checks that the QuoteObj object has been initialized.
    ''' If the object has NOT been initialized, shows the appropriate message and returns FALSE.
    ''' If the object has been initialized, returns TRUE
    ''' </summary>
    ''' <param name="RoutineName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CheckQuoteObject(ByVal RoutineName As String) As Boolean
        Try
            If QuoteObj Is Nothing Then
                ShowMessage(RoutineName & ": " & msgQuoteObjNothing)
                Return False
            Else
                Return True
            End If
        Catch ex As Exception
            HandleError("CheckQuoteObject", ex)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Displays a message in a javascript dialog box
    ''' </summary>
    ''' <param name="MessageText"></param>
    ''' <remarks></remarks>
    Private Sub DisplayMessage(ByVal MessageText As String)
        Dim sc As String = Nothing

        Try
            MessageText = ReplaceSpecialChars(MessageText)
            sc = "<script>alert('" & MessageText & "');</script>"

            ScriptManager.RegisterClientScriptBlock(Me, Me.GetType(), "msg" + DateTime.Now.ToString(), sc, False)

            Exit Sub
        Catch ex As Exception
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Replaces any special characters in the passed string so that the java Alert command can handle it
    ''' </summary>
    ''' <param name="MyString"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ReplaceSpecialChars(ByVal MyString As String) As String
        Dim SpecialChars As String = Nothing
        Dim addstr As String = Nothing
        Dim newstr As String = Nothing

        Try
            newstr = ""
            SpecialChars = "<>\&'" & """"

            For x = 0 To MyString.Length - 1
                If InStr(SpecialChars, MyString.Substring(x, 1)) Then
                    newstr = newstr & "\" & MyString.Substring(x, 1)
                Else
                    newstr = newstr & MyString.Substring(x, 1)
                End If
            Next

            Return newstr
        Catch ex As Exception
            HandleError("ReplaceSpecialChars", ex)
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' Displays the passed message in the lblMsg label
    ''' </summary>
    ''' <param name="Msg"></param>
    ''' <remarks></remarks>
    Private Sub ShowMessage(ByVal Msg As String)
        lblMsg.Text = Msg
    End Sub

    ''' <summary>
    ''' Clears the lblMsg label
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub HideMessage()
        lblMsg.Text = "&nbsp;"
    End Sub

    ''' <summary>
    ''' Standard error handler.  Displays the error in the lblMsg label.
    ''' </summary>
    ''' <param name="RoutineName"></param>
    ''' <param name="exc"></param>
    ''' <remarks></remarks>
    Private Sub HandleError(ByVal RoutineName As String, ByRef exc As Exception)
        ShowMessage(RoutineName & ": " & exc.Message)
        Exit Sub
    End Sub

    ''' <summary>
    ''' Returns the repeater item for the passed question number
    ''' ex: If 1 is passed then the repeater item for Question 1 will be returned.
    ''' </summary>
    ''' <param name="QuestionNumber"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetPolicyUnderwritingQuestionItem(ByVal QuestionNumber As Integer) As RepeaterItem
        Try
            Return rptUWQ.Items(QuestionNumber - 1)
        Catch ex As Exception
            HandleError("GetPolicyUnderwritingQuestionItem", ex)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Loads the question repeater with the text of the UW Questions
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub InitializeQuestions()
        Dim tbl As New DataTable()
        Dim questions As New List(Of VR.Common.UWQuestions.VRUWQuestion)
        Dim EditedQuestion As String = Nothing

        Try
            If Not CheckQuoteObject("InitializeQuestions") Then Exit Sub

            ' Get the list of questions based on the Quote LOB
            Select Case QuoteObj.LobType
                Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                    questions = VR.Common.UWQuestions.UWQuestions.GetPersonalAutoUnderwritingQuestions()
                    Exit Select
                Case QuickQuoteObject.QuickQuoteLobType.HomePersonal
                    questions = VR.Common.UWQuestions.UWQuestions.GetPersonalHomeUnderwritingQuestions(QuoteObj.EffectiveDate, QuoteObj.LobId)
                    Exit Select
                Case QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal  ' Added DFR 10/20/15
                    'Updated 7/27/2022 for task 75803 MLW
                    'questions = VR.Common.UWQuestions.UWQuestions.GetDwellingFireUnderwritingQuestions()                    
                    questions = VR.Common.UWQuestions.UWQuestions.GetDwellingFireUnderwritingQuestions(QuoteObj.EffectiveDate)
                    Exit Select
                Case Else
                    Throw New Exception(msgLOBNotSupported)
            End Select

            ' I build up the 'tbl' table with the question numbers and text, then bind tbl to the
            ' repeater after it's been populated
            If questions IsNot Nothing AndAlso questions.Any Then
                tbl.Clear()
                tbl.Columns.Add("QuestionNumber")
                tbl.Columns.Add("QuestionText")

                For i As Integer = 1 To questions.Count
                    Dim dr As DataRow = Nothing
                    dr = tbl.NewRow()
                    Select Case QuoteObj.LobType
                        Case QuickQuoteObject.QuickQuoteLobType.HomePersonal
                            Select Case i
                                ' DO NOT SHOW QUESTIONS 15,16,17 FOR HOME QUOTES WITH FORM TYPE HO-2, HO-3, HO-3w/15, ML-2
                                ' MGB 11/17/14
                                'added 11/16/17 form types 22, 23, 24 for HOM Upgrade MLW
                                Case 15, 16, 17
                                    If QuoteObj.Locations(0).FormTypeId <> "1" _
                                            AndAlso QuoteObj.Locations(0).FormTypeId <> "2" _
                                            AndAlso QuoteObj.Locations(0).FormTypeId <> "3" _
                                            AndAlso QuoteObj.Locations(0).FormTypeId <> "6" _
                                            AndAlso QuoteObj.Locations(0).FormTypeId <> "22" _
                                            AndAlso QuoteObj.Locations(0).FormTypeId <> "23" _
                                            AndAlso QuoteObj.Locations(0).FormTypeId <> "24" Then
                                        dr("QuestionNumber") = i.ToString()
                                        ' Strip the question numbers from the question descriptions
                                        EditedQuestion = RemoveQuestionNumber(questions(i - 1).Description)
                                        dr("QuestionText") = EditedQuestion
                                        'dr("QuestionText") = questions(i - 1).Description
                                        tbl.Rows.Add(dr)
                                    End If
                                    Exit Select
                                Case 27
                                    'Added 12/12/17 for HOM Upgrade MLW - need to hide this question since it was not originally on the UW question list
                                    Exit Select
                                Case Else
                                    dr("QuestionNumber") = i.ToString()
                                    EditedQuestion = RemoveQuestionNumber(questions(i - 1).Description)
                                    dr("QuestionText") = EditedQuestion
                                    'dr("QuestionText") = questions(i - 1).Description
                                    tbl.Rows.Add(dr)
                                    Exit Select
                            End Select
                            Exit Select
                        Case QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                            ' DON'T SHOW DFR QUESTIONS 15,16,17 IF OCCUPANCY IS OWNER OCCUPIED (id 14)
                            Select Case i
                                Case 15, 16, 17
                                    If QuoteObj.Locations(0).OccupancyCodeId <> 14 Then
                                        ' Strip the question numbers like home
                                        dr("QuestionNumber") = i.ToString()
                                        EditedQuestion = RemoveQuestionNumber(questions(i - 1).Description)
                                        'dr("QuestionText") = questions(i - 1).Description
                                        dr("QuestionText") = EditedQuestion
                                        tbl.Rows.Add(dr)
                                    End If
                                    Exit Select
                                Case Else
                                    ' Strip the question numbers like home
                                    dr("QuestionNumber") = i.ToString()
                                    EditedQuestion = RemoveQuestionNumber(questions(i - 1).Description)
                                    'dr("QuestionText") = questions(i - 1).Description
                                    dr("QuestionText") = EditedQuestion
                                    tbl.Rows.Add(dr)
                                    Exit Select
                            End Select
                            Exit Select
                        Case Else
                            ' The other LOBs show the question numbers
                            dr("QuestionNumber") = i.ToString()
                            dr("QuestionText") = questions(i - 1).Description
                            tbl.Rows.Add(dr)
                            Exit Select
                    End Select
                Next
            End If

            ' Bind the question table to the repeater
            rptUWQ.DataSource = tbl
            rptUWQ.DataBind()

        Catch ex As Exception
            HandleError("InitializeQuestions", ex)
            Exit Sub
        End Try
    End Sub

    Private Function RemoveQuestionNumber(ByVal q As String) As String
        Dim newstr As String = Nothing
        Dim ndx As Integer = -1

        Try
            ndx = q.IndexOf(". ")
            If ndx < 0 Then Return q

            newstr = q.Substring(ndx + 2)

            Return newstr
        Catch ex As Exception
            HandleError("RemoveQuestionNumber", ex)
            Return q
        End Try
    End Function

    ''' <summary>
    ''' Returns the PolicyUnderwritingCodeId for the passed question number
    ''' </summary>
    ''' <param name="QuestionNumber"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetPolicyUnderwritingCodeID(ByVal QuestionNumber As Integer) As String
        Try
            ' Different calculations based on LOB
            Select Case QuoteObj.LobType
                Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                    Return (9283 + (QuestionNumber - 1)).ToString()
                Case QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                    Return (9415 + (QuestionNumber - 1)).ToString()
                Case QuickQuoteObject.QuickQuoteLobType.HomePersonal
                    Select Case QuestionNumber
                        Case 1
                            'Return "9324"
                            Return "9446"  ' New code 11/10/14 MGB
                        Case 9
                            Return "9447"  ' New code 11/10/14 MGB
                        Case Else
                            Return (9297 + QuestionNumber).ToString()
                    End Select
                Case Else
                    Throw New Exception(msgLOBNotSupported)
            End Select
        Catch ex As Exception
            HandleError("GetPolicyUnderwritingCodeID", ex)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Update the policy underwriting questions on the quote object
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SaveAnswersToQuoteObject() As Boolean
        Dim txtDesc As TextBox = Nothing
        Dim rbY As RadioButton = Nothing
        Dim ri As RepeaterItem = Nothing
        Dim questions As New List(Of VR.Common.UWQuestions.VRUWQuestion)
        Dim q3 As New List(Of VR.Common.UWQuestions.VRUWQuestion)
        Dim hasCoverage As Boolean = False
        Dim found As Boolean = False
        Dim SomethingChanged As Boolean = False
        Dim newpuw As QuickQuotePolicyUnderwriting = Nothing

        'added 8/9/2018 for multi-state
        Dim multiStateQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject) = Nothing
        Dim quoteStates As List(Of QuickQuoteHelperClass.QuickQuoteState) = Nothing
        Dim isMultiState As Boolean = False
        Dim qqHelper As New QuickQuoteHelperClass
        If QuoteObj IsNot Nothing Then
            multiStateQuotes = qqHelper.MultiStateQuickQuoteObjects(QuoteObj) 'should always return at least QuoteObj in the list
            quoteStates = QuoteObj.QuoteStates
            If quoteStates IsNot Nothing AndAlso quoteStates.Count > 1 Then
                isMultiState = True 'note: could have used QuoteObj.HasMultipleQuoteStates, which would also use QuoteObj.QuoteStates
            End If
        End If

        Try
            If Not CheckQuoteObject("SaveAnswersToQuoteObject") Then Return False

            ' Create a new underwriting object for each question
            ' Answers (from pilcyunderwritinganswertype_id table):
            ' -1 Not Answered
            ' 0 N/A
            ' 1 YES
            ' 2 NO

            ' PolicyUnderwritingExtraAnswerType_id:
            ' 1 = Text
            ' 2 = Date
            ' 3 = Currency

            ' Remove any existing values
            Select Case QuoteObj.LobType
                Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                    ' Auto UW Questions get stored at the policy level
                    'QuoteObj.PolicyUnderwritings = New List(Of QuickQuotePolicyUnderwriting)
                    'updated 8/9/2018 for multi-state
                    If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                        For Each msq As QuickQuoteObject In multiStateQuotes
                            msq.PolicyUnderwritings = New List(Of QuickQuotePolicyUnderwriting)
                        Next
                    End If
                    Exit Select
                Case QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal, QuickQuoteObject.QuickQuoteLobType.HomePersonal
                    ' Home & Dwelling Fire UW Questions get stored at the location level
                    'QuoteObj.Locations(0).PolicyUnderwritings = New List(Of QuickQuotePolicyUnderwriting)
                    'updated 8/9/2018 for multi-state
                    If isMultiState = True Then
                        'For Each s As QuickQuoteHelperClass.QuickQuoteState In quoteStates
                        '    Dim initialStateLoc As QuickQuoteLocation = qqHelper.LocationForQuickQuoteState(QuoteObj.Locations, s, firstOrLastItem:=QuickQuoteHelperClass.FirstOrLast.First)
                        '    If initialStateLoc IsNot Nothing Then
                        '        initialStateLoc.PolicyUnderwritings = New List(Of QuickQuotePolicyUnderwriting)
                        '    End If
                        'Next
                        'note: above logic is most accurate, but since we should only have 1 loc per state and above code wouldn't pull anything when state is not defined on loc, the code below would be safest
                        If QuoteObj.Locations IsNot Nothing AndAlso QuoteObj.Locations.Count > 0 Then
                            For Each l As QuickQuoteLocation In QuoteObj.Locations
                                l.PolicyUnderwritings = New List(Of QuickQuotePolicyUnderwriting)
                            Next
                        End If
                    Else
                        QuoteObj.Locations(0).PolicyUnderwritings = New List(Of QuickQuotePolicyUnderwriting)
                    End If
                    Exit Select
                Case Else
                    Throw New Exception("Save not coded for LOB")
            End Select

            For i As Integer = 0 To rptUWQ.Items.Count - 1
                Dim puw As New QuickQuotePolicyUnderwriting()
                ri = rptUWQ.Items(i)
                txtDesc = ri.FindControl("txtUWQDescription")
                rbY = ri.FindControl("rbYes")
                Select Case QuoteObj.LobType
                    ' We have to deal with missing home questions 15,16,17 for HOM
                    Case QuickQuoteObject.QuickQuoteLobType.HomePersonal
                        'Updated 11/21/17 for HOM Uprade to include 22, 23, 24 MLW
                        If QuoteObj.Locations(0).FormTypeId = "1" _
                            OrElse QuoteObj.Locations(0).FormTypeId = "2" _
                            OrElse QuoteObj.Locations(0).FormTypeId = "3" _
                            OrElse QuoteObj.Locations(0).FormTypeId = "6" _
                            OrElse QuoteObj.Locations(0).FormTypeId = "22" _
                            OrElse QuoteObj.Locations(0).FormTypeId = "23" _
                            OrElse QuoteObj.Locations(0).FormTypeId = "24" Then
                            If i <= 13 Then
                                puw.PolicyUnderwritingCodeId = GetPolicyUnderwritingCodeID(i + 1)
                            Else
                                puw.PolicyUnderwritingCodeId = GetPolicyUnderwritingCodeID(i + 4)
                            End If
                        Else
                            puw.PolicyUnderwritingCodeId = GetPolicyUnderwritingCodeID(i + 1)
                        End If
                        Exit Select
                    Case QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                        ' We have to deal with missing home questions 15,16,17 for DFR
                        If QuoteObj.Locations(0).OccupancyCodeId = 14 Then
                            If i <= 13 Then
                                puw.PolicyUnderwritingCodeId = GetPolicyUnderwritingCodeID(i + 1)
                            Else
                                puw.PolicyUnderwritingCodeId = GetPolicyUnderwritingCodeID(i + 4)
                            End If
                        Else
                            puw.PolicyUnderwritingCodeId = GetPolicyUnderwritingCodeID(i + 1)
                        End If
                        Exit Select
                    Case Else
                        puw.PolicyUnderwritingCodeId = GetPolicyUnderwritingCodeID(i + 1)
                        Exit Select
                End Select
                puw.PolicyUnderwritingAnswerTypeId = "0"  ' Always 0
                puw.PolicyUnderwritingExtraAnswerTypeId = 0
                If rbY.Checked Then
                    puw.PolicyUnderwritingAnswer = "1" ' Yes
                    ' Some YES questions don't require an answer
                    If txtDesc.Text <> String.Empty Then
                        puw.PolicyUnderwritingExtraAnswerTypeId = "1" ' Text
                        puw.PolicyUnderwritingExtraAnswer = txtDesc.Text
                    End If
                Else
                    ' No is checked
                    ' For DFR #5, addl info is required for a NO answer
                    If QuoteObj.LobType = QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal Then
                        If i = 4 Then
                            puw.PolicyUnderwritingExtraAnswerTypeId = "1" ' Text
                            puw.PolicyUnderwritingExtraAnswer = txtDesc.Text
                        End If
                    End If
                    puw.PolicyUnderwritingAnswer = "-1"  ' No
                End If
                Select Case QuoteObj.LobType
                    Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                        ' Save PPA UW Questions at Policy Level
                        puw.PolicyUnderwritingTabId = "1"
                        puw.PolicyUnderwritingLevelId = "1"
                        'QuoteObj.PolicyUnderwritings.Add(puw)
                        'updated 8/9/2018 for multi-state
                        If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                            For Each msq As QuickQuoteObject In multiStateQuotes
                                msq.PolicyUnderwritings.Add(puw)
                            Next
                        End If
                        Exit Select
                    Case QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                        ' Save DFR UW Questions at Location level
                        puw.PolicyUnderwritingTabId = "1"
                        puw.PolicyUnderwritingLevelId = "3"

                        If i = 14 AndAlso QuoteObj.Locations(0).OccupancyCodeId = 14 Then
                            ' FOR DFR QUOTES WITH OCCUPANCY CODE 14 (owner occupied) DEFAULT THE ANSWERS TO
                            ' QUESTIONS 15,16,17 TO NO
                            If QuoteObj.LobType = QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal AndAlso QuoteObj.Locations(0).OccupancyCodeId = 14 Then
                                'Updated 7/27/2022 for task 75803 MLW
                                'questions = VR.Common.UWQuestions.UWQuestions.GetDwellingFireUnderwritingQuestions()
                                questions = VR.Common.UWQuestions.UWQuestions.GetDwellingFireUnderwritingQuestions(QuoteObj.EffectiveDate)
                                q3.Add(questions(14))
                                q3.Add(questions(15))
                                q3.Add(questions(16))

                                For Each q As VR.Common.UWQuestions.VRUWQuestion In q3
                                    hasCoverage = False
                                    found = False
                                    For Each p As QuickQuotePolicyUnderwriting In QuoteObj.Locations(0).PolicyUnderwritings
                                        If p.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId Then
                                            found = True
                                            If p.PolicyUnderwritingAnswer Is Nothing OrElse (p.PolicyUnderwritingAnswer <> "-1" AndAlso p.PolicyUnderwritingAnswer <> "1") Then
                                                p.PolicyUnderwritingAnswer = "-1" ' NO
                                                SomethingChanged = True
                                            End If
                                            Exit For
                                        End If
                                    Next
                                    ' If Question was not found we need to insert it
                                    If Not found Then
                                        newpuw = New QuickQuotePolicyUnderwriting()
                                        newpuw.PolicyId = q.PolicyId
                                        newpuw.PolicyImageNum = q.PolicyImageNum
                                        newpuw.PolicyUnderwriterDate = q.PolicyUnderwriterDate
                                        newpuw.PolicyUnderwritingAnswer = "-1"  ' DEFAULT ANSWER TO NO
                                        newpuw.PolicyUnderwritingAnswerTypeId = q.PolicyUnderwritingAnswerTypeId
                                        newpuw.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId
                                        newpuw.PolicyUnderwritingExtraAnswer = q.PolicyUnderwritingExtraAnswer
                                        newpuw.PolicyUnderwritingExtraAnswerTypeId = q.PolicyUnderwritingExtraAnswerTypeId
                                        newpuw.PolicyUnderwritingLevelId = "3"
                                        newpuw.PolicyUnderwritingNum = q.PolicyUnderwritingNum
                                        newpuw.PolicyUnderwritingTabId = "1"
                                        'QuoteObj.Locations(0).PolicyUnderwritings.Add(newpuw)
                                        'updated 8/9/2018 for multi-state
                                        If isMultiState = True Then
                                            'For Each s As QuickQuoteHelperClass.QuickQuoteState In quoteStates
                                            '    Dim initialStateLoc As QuickQuoteLocation = qqHelper.LocationForQuickQuoteState(QuoteObj.Locations, s, firstOrLastItem:=QuickQuoteHelperClass.FirstOrLast.First)
                                            '    If initialStateLoc IsNot Nothing Then
                                            '        initialStateLoc.PolicyUnderwritings.Add(newpuw)
                                            '    End If
                                            'Next
                                            'note: above logic is most accurate, but since we should only have 1 loc per state and above code wouldn't pull anything when state is not defined on loc, the code below would be safest
                                            If QuoteObj.Locations IsNot Nothing AndAlso QuoteObj.Locations.Count > 0 Then
                                                For Each l As QuickQuoteLocation In QuoteObj.Locations
                                                    l.PolicyUnderwritings.Add(newpuw)
                                                Next
                                            End If
                                        Else
                                            QuoteObj.Locations(0).PolicyUnderwritings.Add(newpuw)
                                        End If
                                        SomethingChanged = True
                                    End If
                                Next
                            End If
                        End If
                        'QuoteObj.Locations(0).PolicyUnderwritings.Add(puw)
                        'updated 8/9/2018 for multi-state
                        If isMultiState = True Then
                            'For Each s As QuickQuoteHelperClass.QuickQuoteState In quoteStates
                            '    Dim initialStateLoc As QuickQuoteLocation = qqHelper.LocationForQuickQuoteState(QuoteObj.Locations, s, firstOrLastItem:=QuickQuoteHelperClass.FirstOrLast.First)
                            '    If initialStateLoc IsNot Nothing Then
                            '        initialStateLoc.PolicyUnderwritings.Add(puw)
                            '    End If
                            'Next
                            'note: above logic is most accurate, but since we should only have 1 loc per state and above code wouldn't pull anything when state is not defined on loc, the code below would be safest
                            If QuoteObj.Locations IsNot Nothing AndAlso QuoteObj.Locations.Count > 0 Then
                                For Each l As QuickQuoteLocation In QuoteObj.Locations
                                    l.PolicyUnderwritings.Add(puw)
                                Next
                            End If
                        Else
                            QuoteObj.Locations(0).PolicyUnderwritings.Add(puw)
                        End If
                        Exit Select
                    Case QuickQuoteObject.QuickQuoteLobType.HomePersonal
                        ' Save HOM UW Questions at Location level
                        puw.PolicyUnderwritingTabId = "1"
                        puw.PolicyUnderwritingLevelId = "3"
                        'puw.PolicyUnderwritingNum = (i + 4).ToString()
                        'updated 11/28/17 for HOM Upgrade MLW - added 22, 23, 24
                        If i = 14 _
                        AndAlso QuoteObj.Locations(0).FormTypeId = "1" _
                        OrElse QuoteObj.Locations(0).FormTypeId = "2" _
                        OrElse QuoteObj.Locations(0).FormTypeId = "3" _
                        OrElse QuoteObj.Locations(0).FormTypeId = "6" _
                        OrElse QuoteObj.Locations(0).FormTypeId = "22" _
                        OrElse QuoteObj.Locations(0).FormTypeId = "23" _
                        OrElse QuoteObj.Locations(0).FormTypeId = "24" Then
                            ' FOR HOME QUOTES WITH FORM TYPE HO-2, HO-3, HO-3w/15, ML-2 DEFAULT THE ANSWERS TO
                            ' QUESTIONS 15,16,17 TO NO
                            'updated 11/28/17 for HOM Upgrade MLW - added 22, 23, 24
                            If QuoteObj.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal _
                                AndAlso (QuoteObj.Locations(0).FormTypeId = "1" _
                                OrElse QuoteObj.Locations(0).FormTypeId = "2" _
                                OrElse QuoteObj.Locations(0).FormTypeId = "3" _
                                OrElse QuoteObj.Locations(0).FormTypeId = "6" _
                                OrElse QuoteObj.Locations(0).FormTypeId = "22" _
                                OrElse QuoteObj.Locations(0).FormTypeId = "23" _
                                OrElse QuoteObj.Locations(0).FormTypeId = "24") Then
                                questions = VR.Common.UWQuestions.UWQuestions.GetPersonalHomeUnderwritingQuestions(QuoteObj.EffectiveDate, QuoteObj.LobId)
                                q3.Add(questions(14))
                                q3.Add(questions(15))
                                q3.Add(questions(16))

                                For Each q As VR.Common.UWQuestions.VRUWQuestion In q3
                                    hasCoverage = False
                                    found = False
                                    If isMultiState = True Then 'added IF 8/9/2018 for multi-state; original logic in ELSE
                                        'For Each s As QuickQuoteHelperClass.QuickQuoteState In quoteStates
                                        '    Dim initialStateLoc As QuickQuoteLocation = qqHelper.LocationForQuickQuoteState(QuoteObj.Locations, s, firstOrLastItem:=QuickQuoteHelperClass.FirstOrLast.First)
                                        '    If initialStateLoc IsNot Nothing Then
                                        '        For Each p As QuickQuotePolicyUnderwriting In initialStateLoc.PolicyUnderwritings
                                        '            If p.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId Then
                                        '                found = True
                                        '                If p.PolicyUnderwritingAnswer Is Nothing OrElse (p.PolicyUnderwritingAnswer <> "-1" AndAlso p.PolicyUnderwritingAnswer <> "1") Then
                                        '                    p.PolicyUnderwritingAnswer = "-1" ' NO
                                        '                    SomethingChanged = True
                                        '                End If
                                        '                Exit For
                                        '            End If
                                        '        Next
                                        '    End If
                                        'Next
                                        'note: above logic is most accurate, but since we should only have 1 loc per state and above code wouldn't pull anything when state is not defined on loc, the code below would be safest
                                        If QuoteObj.Locations IsNot Nothing AndAlso QuoteObj.Locations.Count > 0 Then
                                            For Each l As QuickQuoteLocation In QuoteObj.Locations
                                                For Each p As QuickQuotePolicyUnderwriting In l.PolicyUnderwritings
                                                    If p.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId Then
                                                        found = True
                                                        If p.PolicyUnderwritingAnswer Is Nothing OrElse (p.PolicyUnderwritingAnswer <> "-1" AndAlso p.PolicyUnderwritingAnswer <> "1") Then
                                                            p.PolicyUnderwritingAnswer = "-1" ' NO
                                                            SomethingChanged = True
                                                        End If
                                                        Exit For
                                                    End If
                                                Next
                                            Next
                                        End If
                                    Else
                                        For Each p As QuickQuotePolicyUnderwriting In QuoteObj.Locations(0).PolicyUnderwritings
                                            If p.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId Then
                                                found = True
                                                If p.PolicyUnderwritingAnswer Is Nothing OrElse (p.PolicyUnderwritingAnswer <> "-1" AndAlso p.PolicyUnderwritingAnswer <> "1") Then
                                                    p.PolicyUnderwritingAnswer = "-1" ' NO
                                                    SomethingChanged = True
                                                End If
                                                Exit For
                                            End If
                                        Next
                                    End If
                                    ' If Question was not found we need to insert it
                                    If Not found Then
                                        newpuw = New QuickQuotePolicyUnderwriting()
                                        newpuw.PolicyId = q.PolicyId
                                        newpuw.PolicyImageNum = q.PolicyImageNum
                                        newpuw.PolicyUnderwriterDate = q.PolicyUnderwriterDate
                                        newpuw.PolicyUnderwritingAnswer = "-1"  ' DEFAULT ANSWER TO NO
                                        newpuw.PolicyUnderwritingAnswerTypeId = q.PolicyUnderwritingAnswerTypeId
                                        newpuw.PolicyUnderwritingCodeId = q.PolicyUnderwritingCodeId
                                        newpuw.PolicyUnderwritingExtraAnswer = q.PolicyUnderwritingExtraAnswer
                                        newpuw.PolicyUnderwritingExtraAnswerTypeId = q.PolicyUnderwritingExtraAnswerTypeId
                                        newpuw.PolicyUnderwritingLevelId = "3"
                                        newpuw.PolicyUnderwritingNum = q.PolicyUnderwritingNum
                                        newpuw.PolicyUnderwritingTabId = "1"
                                        'QuoteObj.Locations(0).PolicyUnderwritings.Add(newpuw)
                                        'updated 8/9/2018 for multi-state
                                        If isMultiState = True Then
                                            'For Each s As QuickQuoteHelperClass.QuickQuoteState In quoteStates
                                            '    Dim initialStateLoc As QuickQuoteLocation = qqHelper.LocationForQuickQuoteState(QuoteObj.Locations, s, firstOrLastItem:=QuickQuoteHelperClass.FirstOrLast.First)
                                            '    If initialStateLoc IsNot Nothing Then
                                            '        initialStateLoc.PolicyUnderwritings.Add(newpuw)
                                            '    End If
                                            'Next
                                            'note: above logic is most accurate, but since we should only have 1 loc per state and above code wouldn't pull anything when state is not defined on loc, the code below would be safest
                                            If QuoteObj.Locations IsNot Nothing AndAlso QuoteObj.Locations.Count > 0 Then
                                                For Each l As QuickQuoteLocation In QuoteObj.Locations
                                                    l.PolicyUnderwritings.Add(newpuw)
                                                Next
                                            End If
                                        Else
                                            QuoteObj.Locations(0).PolicyUnderwritings.Add(newpuw)
                                        End If
                                        SomethingChanged = True
                                    End If
                                Next
                            End If
                        End If
                        'QuoteObj.Locations(0).PolicyUnderwritings.Add(puw)
                        'updated 8/9/2018 for multi-state
                        If isMultiState = True Then
                            'For Each s As QuickQuoteHelperClass.QuickQuoteState In quoteStates
                            '    Dim initialStateLoc As QuickQuoteLocation = qqHelper.LocationForQuickQuoteState(QuoteObj.Locations, s, firstOrLastItem:=QuickQuoteHelperClass.FirstOrLast.First)
                            '    If initialStateLoc IsNot Nothing Then
                            '        initialStateLoc.PolicyUnderwritings.Add(puw)
                            '    End If
                            'Next
                            'note: above logic is most accurate, but since we should only have 1 loc per state and above code wouldn't pull anything when state is not defined on loc, the code below would be safest
                            If QuoteObj.Locations IsNot Nothing AndAlso QuoteObj.Locations.Count > 0 Then
                                For Each l As QuickQuoteLocation In QuoteObj.Locations
                                    l.PolicyUnderwritings.Add(puw)
                                Next
                            End If
                        Else
                            QuoteObj.Locations(0).PolicyUnderwritings.Add(puw)
                        End If
                        Exit Select
                    Case Else
                        Throw New Exception("Save not coded for LOB")
                End Select
            Next

            Return True
        Catch ex As Exception
            HandleError("SaveAnswersToQuoteObject", ex)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Shows or hides the passed additional info table
    ''' </summary>
    ''' <param name="DescTable"></param>
    ''' <param name="ShowOrHide"></param>
    ''' <remarks></remarks>
    Protected Sub ShowHideAdditionalInfo(ByVal DescTable As HtmlTable, ByVal ShowOrHide As String)
        Dim js As String = ""
        Dim MyType As Type = Nothing
        Dim MyKeyId As String = Nothing

        Try
            If ShowOrHide.ToUpper() = "SHOW" Then
                DescTable.Style.Add("display", "block")
            Else
                DescTable.Style.Add("display", "none")
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("ShowHideAdditionalInfo", ex)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Returns a QuickQuotePolicyUnderwriting object loaded with the appropriate PERSONAL AUTO question
    ''' </summary>
    ''' <param name="QuestionNumber"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetQuestionPersonalAuto(ByVal QuestionNumber As Integer) As QuickQuotePolicyUnderwriting
        Try
            If Not CheckQuoteObject("GetQuestionPersonalAuto") Then Return Nothing
            'If QuoteObj.PolicyUnderwritings Is Nothing OrElse QuoteObj.PolicyUnderwritings.Count <= 0 Then Return Nothing
            'updated 8/10/2018
            Dim qqHelper As New QuickQuoteHelperClass
            Dim multiStateQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject) = Nothing
            Dim quoteToUse As QuickQuote.CommonObjects.QuickQuoteObject = Nothing
            If QuoteObj IsNot Nothing Then
                multiStateQuotes = qqHelper.MultiStateQuickQuoteObjects(QuoteObj) 'should always return at least QuoteObj in the list
                If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                    quoteToUse = multiStateQuotes.Item(0) 'will just use the 1st stateQuote found (or QuoteObj) since all questions are currently the same for all states
                End If
            End If
            If quoteToUse Is Nothing OrElse quoteToUse.PolicyUnderwritings Is Nothing OrElse quoteToUse.PolicyUnderwritings.Count <= 0 Then Return Nothing
            If QuestionNumber <= 0 OrElse QuestionNumber > 16 Then Throw New Exception("Invalid question number passed to GetQuestion: " & QuestionNumber.ToString())

            'For Each puw As QuickQuotePolicyUnderwriting In QuoteObj.PolicyUnderwritings
            'updated 8/10/2018
            For Each puw As QuickQuotePolicyUnderwriting In quoteToUse.PolicyUnderwritings
                Select Case puw.PolicyUnderwritingCodeId
                    Case "9283"
                        If QuestionNumber = 1 Then Return puw
                    Case "9284"
                        If QuestionNumber = 2 Then Return puw
                    Case "9285"
                        If QuestionNumber = 3 Then Return puw
                    Case "9286"
                        If QuestionNumber = 4 Then Return puw
                    Case "9287"
                        If QuestionNumber = 5 Then Return puw
                    Case "9288"
                        If QuestionNumber = 6 Then Return puw
                    Case "9289"
                        If QuestionNumber = 7 Then Return puw
                    Case "9290"
                        If QuestionNumber = 8 Then Return puw
                    Case "9291"
                        If QuestionNumber = 9 Then Return puw
                    Case "9292"
                        If QuestionNumber = 10 Then Return puw
                    Case "9293"
                        If QuestionNumber = 11 Then Return puw
                    Case "9294"
                        If QuestionNumber = 12 Then Return puw
                    Case "9295"
                        If QuestionNumber = 13 Then Return puw
                    Case "9296"
                        If QuestionNumber = 14 Then Return puw
                    Case "9297"
                        If QuestionNumber = 15 Then Return puw
                    Case "9298"
                        If QuestionNumber = 16 Then Return puw
                    Case Else
                        Return Nothing
                End Select
            Next
            Return Nothing
        Catch ex As Exception
            HandleError("GetQuestionPersonalAuto", ex)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Returns a QuickQuotePolicyUnderwriting object loaded with the appropriate PERSONAL HOME question
    ''' </summary>
    ''' <param name="QuestionNumber"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetQuestionPersonalHome(ByVal QuestionNumber As Integer) As QuickQuotePolicyUnderwriting
        Try
            If Not CheckQuoteObject("GetQuestionPersonalHome") Then Return Nothing
            If QuoteObj.Locations Is Nothing OrElse QuoteObj.Locations.Count <= 0 Then Return Nothing
            If QuoteObj.Locations(0).PolicyUnderwritings Is Nothing OrElse QuoteObj.Locations(0).PolicyUnderwritings.Count <= 0 Then Return Nothing
            If QuestionNumber <= 0 OrElse QuestionNumber > 27 Then Throw New Exception("Invalid question number passed to GetQuestion: " & QuestionNumber.ToString()) 'updated 12/1/17 to 27 for HOM Upgrade

            For Each puw As QuickQuotePolicyUnderwriting In QuoteObj.Locations(0).PolicyUnderwritings
                Select Case puw.PolicyUnderwritingCodeId
                    'Case "9324"
                    Case "9446"  ' New code 11/10/14  MGB
                        If QuestionNumber = 1 Then Return puw
                    Case "9299"
                        If QuestionNumber = 2 Then Return puw
                    Case "9300"
                        If QuestionNumber = 3 Then Return puw
                    Case "9301"
                        If QuestionNumber = 4 Then Return puw
                    Case "9302"
                        If QuestionNumber = 5 Then Return puw
                    Case "9303"
                        If QuestionNumber = 6 Then Return puw
                    Case "9304"
                        If QuestionNumber = 7 Then Return puw
                    Case "9305"
                        If QuestionNumber = 8 Then Return puw
                    'Case "9306"
                    Case "9447"  ' New code 11/10/14  MGB
                        If QuestionNumber = 9 Then Return puw
                    Case "9307"
                        If QuestionNumber = 10 Then Return puw
                    Case "9308"
                        If QuestionNumber = 11 Then Return puw
                    Case "9309"
                        If QuestionNumber = 12 Then Return puw
                    Case "9310"
                        If QuestionNumber = 13 Then Return puw
                    Case "9311"
                        If QuestionNumber = 14 Then Return puw
                    Case "9312"
                        If QuestionNumber = 15 Then Return puw
                    Case "9313"
                        If QuestionNumber = 16 Then Return puw
                    Case "9314"
                        If QuestionNumber = 17 Then Return puw
                    Case "9315"
                        If QuestionNumber = 18 Then Return puw
                    Case "9316"
                        If QuestionNumber = 19 Then Return puw
                    Case "9317"
                        If QuestionNumber = 20 Then Return puw
                    Case "9318"
                        If QuestionNumber = 21 Then Return puw
                    Case "9319"
                        If QuestionNumber = 22 Then Return puw
                    Case "9320"
                        If QuestionNumber = 23 Then Return puw
                    Case "9321"
                        If QuestionNumber = 24 Then Return puw
                    Case "9322"
                        If QuestionNumber = 25 Then Return puw
                    Case "9323"
                        If QuestionNumber = 26 Then Return puw
                    Case "9297" 'added 12/1/17 for HOM Upgrade MLW
                        If QuestionNumber = 27 Then Return puw
                    Case Else
                        Exit Select
                End Select
            Next
            Return Nothing
        Catch ex As Exception
            HandleError("GetQuestionPersonalHome", ex)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Returns a QuickQuotePolicyUnderwriting object loaded with the appropriate DWELLING FIRE question
    ''' </summary>
    ''' <param name="QuestionNumber"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetQuestionDwellingFire(ByVal QuestionNumber As Integer) As QuickQuotePolicyUnderwriting
        Try
            If Not CheckQuoteObject("GetQuestionDwellingFire") Then Return Nothing
            If QuoteObj.Locations Is Nothing OrElse QuoteObj.Locations.Count <= 0 Then Return Nothing
            If QuoteObj.Locations(0).PolicyUnderwritings Is Nothing OrElse QuoteObj.Locations(0).PolicyUnderwritings.Count <= 0 Then Return Nothing
            If QuestionNumber <= 0 OrElse QuestionNumber > 26 Then Throw New Exception("Invalid question number passed to GetQuestion: " & QuestionNumber.ToString())

            For Each puw As QuickQuotePolicyUnderwriting In QuoteObj.Locations(0).PolicyUnderwritings
                Select Case puw.PolicyUnderwritingCodeId
                    Case "9415"
                        If QuestionNumber = 1 Then Return puw
                    Case "9416"
                        If QuestionNumber = 2 Then Return puw
                    Case "9417"
                        If QuestionNumber = 3 Then Return puw
                    Case "9418"
                        If QuestionNumber = 4 Then Return puw
                    Case "9419"
                        If QuestionNumber = 5 Then Return puw
                    Case "9420"
                        If QuestionNumber = 6 Then Return puw
                    Case "9421"
                        If QuestionNumber = 7 Then Return puw
                    Case "9422"
                        If QuestionNumber = 8 Then Return puw
                    Case "9423"
                        If QuestionNumber = 9 Then Return puw
                    Case "9424"
                        If QuestionNumber = 10 Then Return puw
                    Case "9425"
                        If QuestionNumber = 11 Then Return puw
                    Case "9426"
                        If QuestionNumber = 12 Then Return puw
                    Case "9427"
                        If QuestionNumber = 13 Then Return puw
                    Case "9428"
                        If QuestionNumber = 14 Then Return puw
                    Case "9429"
                        If QuestionNumber = 15 Then Return puw
                    Case "9430"
                        If QuestionNumber = 16 Then Return puw
                    Case "9431"
                        If QuestionNumber = 17 Then Return puw
                    Case "9432"
                        If QuestionNumber = 18 Then Return puw
                    Case "9433"
                        If QuestionNumber = 19 Then Return puw
                    Case "9434"
                        If QuestionNumber = 20 Then Return puw
                    Case "9435"
                        If QuestionNumber = 21 Then Return puw
                    Case "9436"
                        If QuestionNumber = 22 Then Return puw
                    Case "9437"
                        If QuestionNumber = 23 Then Return puw
                    Case "9438"
                        If QuestionNumber = 24 Then Return puw
                    Case "9439"
                        If QuestionNumber = 25 Then Return puw
                    Case "9440"
                        If QuestionNumber = 26 Then Return puw
                    Case Else
                        Exit Select
                End Select
            Next
            Return Nothing
        Catch ex As Exception
            HandleError("GetQuestionDwellingFire", ex)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Returns a PolicyUnderwriting Object from the quote for the passed question number
    ''' </summary>
    ''' <param name="QuestionNumber"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetQuestion(ByVal QuestionNumber As Integer) As QuickQuotePolicyUnderwriting
        Try
            If Not CheckQuoteObject("GetQuestion") Then Return Nothing

            Select Case QuoteObj.LobType
                Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                    Return GetQuestionPersonalAuto(QuestionNumber)
                Case QuickQuoteObject.QuickQuoteLobType.HomePersonal
                    Return GetQuestionPersonalHome(QuestionNumber)
                Case QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                    Return GetQuestionDwellingFire(QuestionNumber)
                Case Else
                    Return Nothing
            End Select
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' On a Homeowners quote, if HO-72 and/or HO-73 are selected then question 1 is to default to "YES"
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CheckHOMQuestion1()
        Dim newpuw As QuickQuotePolicyUnderwriting = Nothing
        Dim q As List(Of VR.Common.UWQuestions.VRUWQuestion) = Nothing
        Dim q1 As VR.Common.UWQuestions.VRUWQuestion = Nothing
        Dim foundit As Boolean = False
        Dim hasCoverage As Boolean = False

        Try
            'added 8/10/2018 for multi-state
            'Dim multiStateQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject) = Nothing
            Dim quoteStates As List(Of QuickQuoteHelperClass.QuickQuoteState) = Nothing
            Dim isMultiState As Boolean = False
            Dim qqHelper As New QuickQuoteHelperClass
            If QuoteObj IsNot Nothing Then
                'multiStateQuotes = qqHelper.MultiStateQuickQuoteObjects(QuoteObj) 'should always return at least QuoteObj in the list
                quoteStates = QuoteObj.QuoteStates
                If quoteStates IsNot Nothing AndAlso quoteStates.Count > 1 Then
                    isMultiState = True 'note: could have used QuoteObj.HasMultipleQuoteStates, which would also use QuoteObj.QuoteStates
                End If
            Else
                Exit Sub
            End If

            ' Only applies to home quotes
            If QuoteObj.LobType <> QuickQuoteObject.QuickQuoteLobType.HomePersonal Then Exit Sub

            ' The coverages are located on the location so if there's no location then the coverages are obviously not there
            If QuoteObj.Locations Is Nothing OrElse QuoteObj.Locations.Count <= 0 Then Exit Sub

            ' THIS ONLY APPLIES TO FORMS HO-72 AND/OR HO-73
            ' Check to see if either one of these coverages is on the location
            If isMultiState = True Then 'added IF 8/10/2018 for multi-state; original logic in ELSE
                'for multiState; looking in each stateQuote, but don't have to drill into multiStateQuotes list since all state locs are pushed up to top level
                Dim exitLocLoop As Boolean = False
                For Each l As QuickQuoteLocation In QuoteObj.Locations
                    If l.SectionIICoverages IsNot Nothing AndAlso l.SectionIICoverages.Count > 0 Then
                        For Each s2c As QuickQuoteSectionIICoverage In l.SectionIICoverages
                            If s2c.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.IncidentalFarmersPersonalLiability _
                                            OrElse s2c.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.FarmOwnedAndOperatedByInsured0_160Acres Then
                                hasCoverage = True
                                exitLocLoop = True
                                Exit For
                            End If
                        Next
                    End If
                    If exitLocLoop = True Then
                        Exit For
                    End If
                Next
            Else
                If QuoteObj.Locations(0).SectionIICoverages IsNot Nothing Then
                    For Each s2c As QuickQuoteSectionIICoverage In QuoteObj.Locations(0).SectionIICoverages
                        If s2c.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.IncidentalFarmersPersonalLiability _
                            OrElse s2c.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.FarmOwnedAndOperatedByInsured0_160Acres Then
                            hasCoverage = True
                            Exit For
                        End If
                    Next
                End If
            End If

            ' If neither of these coverages are selected, don't do anything just exit
            If Not hasCoverage Then Exit Sub

            ' One or both of the coverages has been selected.  Default the answer if there is no value
            ' If no policyunderwritings collection, create one
            If isMultiState = True Then 'added IF 8/10/2018 for multi-state; original logic in ELSE
                'For Each s As QuickQuoteHelperClass.QuickQuoteState In quoteStates
                '    Dim initialStateLoc As QuickQuoteLocation = qqHelper.LocationForQuickQuoteState(QuoteObj.Locations, s, firstOrLastItem:=QuickQuoteHelperClass.FirstOrLast.First)
                '    If initialStateLoc IsNot Nothing AndAlso initialStateLoc.PolicyUnderwritings Is Nothing Then
                '        initialStateLoc.PolicyUnderwritings = New List(Of QuickQuotePolicyUnderwriting)
                '    End If
                'Next
                'note: above logic is most accurate, but since we should only have 1 loc per state and above code wouldn't pull anything when state is not defined on loc, the code below would be safest
                If QuoteObj.Locations IsNot Nothing AndAlso QuoteObj.Locations.Count > 0 Then
                    For Each l As QuickQuoteLocation In QuoteObj.Locations
                        If l.PolicyUnderwritings Is Nothing Then
                            l.PolicyUnderwritings = New List(Of QuickQuotePolicyUnderwriting)
                        End If
                    Next
                End If
            Else
                If QuoteObj.Locations(0).PolicyUnderwritings Is Nothing Then
                    QuoteObj.Locations(0).PolicyUnderwritings = New List(Of QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting)
                End If
            End If
            ' Check to see if there's an existing Question 1 in the policyunderwritings
            newpuw = New QuickQuotePolicyUnderwriting()
            q = VR.Common.UWQuestions.UWQuestions.GetPersonalHomeUnderwritingQuestions(QuoteObj.EffectiveDate, QuoteObj.LobId)
            q1 = q(0)
            For Each uwq As QuickQuotePolicyUnderwriting In QuoteObj.Locations(0).PolicyUnderwritings
                If uwq.PolicyUnderwritingCodeId = q1.PolicyUnderwritingCodeId Then
                    ' If we find the question then there is nothing for us to do here
                    foundit = True
                    Exit Sub
                End If
            Next

            ' We should only get here if Question 1 was not found in the existing location policyunderwritings
            ' There is no existing question 1 in the policyunderwritings; add it and default the answer to YES
            newpuw.PolicyId = q(0).PolicyId
            newpuw.PolicyImageNum = q(0).PolicyImageNum
            newpuw.PolicyUnderwriterDate = q(0).PolicyUnderwriterDate
            newpuw.PolicyUnderwritingAnswer = 1
            newpuw.PolicyUnderwritingAnswerTypeId = q(0).PolicyUnderwritingAnswerTypeId
            newpuw.PolicyUnderwritingCodeId = q(0).PolicyUnderwritingCodeId
            newpuw.PolicyUnderwritingExtraAnswer = q(0).PolicyUnderwritingExtraAnswer
            newpuw.PolicyUnderwritingExtraAnswerTypeId = q(0).PolicyUnderwritingExtraAnswerTypeId
            newpuw.PolicyUnderwritingLevelId = q(0).PolicyUnderwritingLevelId
            newpuw.PolicyUnderwritingNum = q(0).PolicyUnderwritingNum
            newpuw.PolicyUnderwritingTabId = q(0).PolicyUnderwritingTabId
            If isMultiState = True Then 'added IF 8/10/2018 for multi-state; original logic in ELSE
                'For Each s As QuickQuoteHelperClass.QuickQuoteState In quoteStates
                '    Dim initialStateLoc As QuickQuoteLocation = qqHelper.LocationForQuickQuoteState(QuoteObj.Locations, s, firstOrLastItem:=QuickQuoteHelperClass.FirstOrLast.First)
                '    If initialStateLoc IsNot Nothing AndAlso initialStateLoc.PolicyUnderwritings IsNot Nothing Then
                '        initialStateLoc.PolicyUnderwritings.Add(newpuw)
                '    End If
                'Next
                'note: above logic is most accurate, but since we should only have 1 loc per state and above code wouldn't pull anything when state is not defined on loc, the code below would be safest
                If QuoteObj.Locations IsNot Nothing AndAlso QuoteObj.Locations.Count > 0 Then
                    For Each l As QuickQuoteLocation In QuoteObj.Locations
                        If l.PolicyUnderwritings IsNot Nothing Then
                            l.PolicyUnderwritings.Add(newpuw)
                        End If
                    Next
                End If
            Else
                QuoteObj.Locations(0).PolicyUnderwritings.Add(newpuw)
            End If

            ' Save the quote
            RaiseEvent SaveRequested(0, "ctlUWQuestions")

            Exit Sub
        Catch ex As Exception
            HandleError("CheckHOMQuestion1", ex)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' On a Homeowners quote, if HO 2477 Canine Liability Exclusion is selected then question 9 is to default to "YES"
    ''' Added 2/8/18 for HOM Upgrade MLW
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CheckHOMQuestion9()
        Dim HomeVersion = GetHomeVersion(QuoteObj)
        If (QuoteObj.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso QuoteObj.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
            Dim newpuw As QuickQuotePolicyUnderwriting = Nothing
            Dim q As List(Of VR.Common.UWQuestions.VRUWQuestion) = Nothing
            Dim q9 As VR.Common.UWQuestions.VRUWQuestion = Nothing
            Dim found9 As Boolean = False
            Dim hasCoverage As Boolean = False
            Dim SomethingChanged As Boolean = False

            Try
                'added 8/10/2018 for multi-state
                'Dim multiStateQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject) = Nothing
                Dim quoteStates As List(Of QuickQuoteHelperClass.QuickQuoteState) = Nothing
                Dim isMultiState As Boolean = False
                Dim qqHelper As New QuickQuoteHelperClass
                If QuoteObj IsNot Nothing Then
                    'multiStateQuotes = qqHelper.MultiStateQuickQuoteObjects(QuoteObj) 'should always return at least QuoteObj in the list
                    quoteStates = QuoteObj.QuoteStates
                    If quoteStates IsNot Nothing AndAlso quoteStates.Count > 1 Then
                        isMultiState = True 'note: could have used QuoteObj.HasMultipleQuoteStates, which would also use QuoteObj.QuoteStates
                    End If
                Else
                    Exit Sub
                End If

                ' Only applies to home quotes
                If QuoteObj.LobType <> QuickQuoteObject.QuickQuoteLobType.HomePersonal Then Exit Sub

                ' The coverages are located on the location so if there's no location then the coverages are obviously not there
                If QuoteObj.Locations Is Nothing OrElse QuoteObj.Locations.Count <= 0 Then Exit Sub

                ' THIS ONLY APPLIES TO FORMS HO 2477 Canine Liability Exclusion
                ' Check to see if this coverage is on the location
                If isMultiState = True Then 'added IF 8/10/2018 for multi-state; original logic in ELSE
                    'for multiState; looking in each stateQuote, but don't have to drill into multiStateQuotes list since all state locs are pushed up to top level
                    Dim exitLocLoop As Boolean = False
                    For Each l As QuickQuoteLocation In QuoteObj.Locations
                        If l.SectionIICoverages IsNot Nothing AndAlso l.SectionIICoverages.Count > 0 Then
                            For Each s2c As QuickQuoteSectionIICoverage In l.SectionIICoverages
                                If s2c.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.CanineLiabilityExclusion Then
                                    'BusinessPursuits_Clerical - using this for testing until Canine passed through
                                    'CanineLiabilityExclusion
                                    hasCoverage = True
                                    exitLocLoop = True
                                    Exit For
                                End If
                            Next
                        End If
                        If exitLocLoop = True Then
                            Exit For
                        End If
                    Next
                Else
                    If QuoteObj.Locations(0).SectionIICoverages IsNot Nothing Then
                        For Each s2c As QuickQuoteSectionIICoverage In QuoteObj.Locations(0).SectionIICoverages
                            If s2c.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.CanineLiabilityExclusion Then
                                'BusinessPursuits_Clerical - using this for testing until Canine passed through
                                'CanineLiabilityExclusion
                                hasCoverage = True
                                Exit For
                            End If
                        Next
                    End If
                End If

                ' If neither of these coverages are selected, don't do anything just exit
                If Not hasCoverage Then Exit Sub

                ' The coverage has been selected.  Default the answer if there is no value
                ' If no policyunderwritings collection, create one
                If isMultiState = True Then 'added IF 8/10/2018 for multi-state; original logic in ELSE
                    'For Each s As QuickQuoteHelperClass.QuickQuoteState In quoteStates
                    '    Dim initialStateLoc As QuickQuoteLocation = qqHelper.LocationForQuickQuoteState(QuoteObj.Locations, s, firstOrLastItem:=QuickQuoteHelperClass.FirstOrLast.First)
                    '    If initialStateLoc IsNot Nothing AndAlso initialStateLoc.PolicyUnderwritings Is Nothing Then
                    '        initialStateLoc.PolicyUnderwritings = New List(Of QuickQuotePolicyUnderwriting)
                    '    End If
                    'Next
                    'note: above logic is most accurate, but since we should only have 1 loc per state and above code wouldn't pull anything when state is not defined on loc, the code below would be safest
                    If QuoteObj.Locations IsNot Nothing AndAlso QuoteObj.Locations.Count > 0 Then
                        For Each l As QuickQuoteLocation In QuoteObj.Locations
                            If l.PolicyUnderwritings Is Nothing Then
                                l.PolicyUnderwritings = New List(Of QuickQuotePolicyUnderwriting)
                            End If
                        Next
                    End If
                Else
                    If QuoteObj.Locations(0).PolicyUnderwritings Is Nothing Then
                        QuoteObj.Locations(0).PolicyUnderwritings = New List(Of QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting)
                    End If
                End If
                ' Check to see if there's an existing Question 9 in the policyunderwritings (dangerous breed question)
                newpuw = New QuickQuotePolicyUnderwriting()
                q = VR.Common.UWQuestions.UWQuestions.GetPersonalHomeUnderwritingQuestions(QuoteObj.EffectiveDate, QuoteObj.LobId)
                q9 = q(8)
                hasCoverage = False
                If isMultiState = True Then 'added IF 8/10/2018 for multi-state; original logic in ELSE
                    'for multiState; looking in each stateQuote, but don't have to drill into multiStateQuotes list since all state locs are pushed up to top level
                    For Each l As QuickQuoteLocation In QuoteObj.Locations
                        If l.PolicyUnderwritings IsNot Nothing AndAlso l.PolicyUnderwritings.Count > 0 Then
                            For Each p As QuickQuotePolicyUnderwriting In l.PolicyUnderwritings
                                If p.PolicyUnderwritingCodeId = q9.PolicyUnderwritingCodeId Then
                                    found9 = True
                                    If p.PolicyUnderwritingAnswer Is Nothing OrElse (p.PolicyUnderwritingAnswer <> "1") Then
                                        p.PolicyUnderwritingAnswer = "1" ' YES
                                        SomethingChanged = True
                                    End If
                                    Exit For
                                End If
                            Next
                        End If
                    Next
                Else
                    For Each p As QuickQuotePolicyUnderwriting In QuoteObj.Locations(0).PolicyUnderwritings
                        If p.PolicyUnderwritingCodeId = q9.PolicyUnderwritingCodeId Then
                            found9 = True
                            If p.PolicyUnderwritingAnswer Is Nothing OrElse (p.PolicyUnderwritingAnswer <> "1") Then
                                p.PolicyUnderwritingAnswer = "1" ' YES
                                SomethingChanged = True
                            End If
                            Exit For
                        End If
                    Next
                End If
                ' If Question 9 was not found we need to insert it
                If Not found9 Then
                    newpuw = New QuickQuotePolicyUnderwriting()
                    newpuw.PolicyId = q9.PolicyId
                    newpuw.PolicyImageNum = q9.PolicyImageNum
                    newpuw.PolicyUnderwriterDate = q9.PolicyUnderwriterDate
                    newpuw.PolicyUnderwritingAnswer = "1"  ' YES
                    newpuw.PolicyUnderwritingAnswerTypeId = q9.PolicyUnderwritingAnswerTypeId
                    newpuw.PolicyUnderwritingCodeId = q9.PolicyUnderwritingCodeId
                    newpuw.PolicyUnderwritingExtraAnswer = q9.PolicyUnderwritingExtraAnswer
                    newpuw.PolicyUnderwritingExtraAnswerTypeId = q9.PolicyUnderwritingExtraAnswerTypeId
                    newpuw.PolicyUnderwritingLevelId = "3"
                    newpuw.PolicyUnderwritingNum = q9.PolicyUnderwritingNum
                    newpuw.PolicyUnderwritingTabId = "1"
                    If isMultiState = True Then 'added IF 8/10/2018 for multi-state; original logic in ELSE
                        'For Each s As QuickQuoteHelperClass.QuickQuoteState In quoteStates
                        '    Dim initialStateLoc As QuickQuoteLocation = qqHelper.LocationForQuickQuoteState(QuoteObj.Locations, s, firstOrLastItem:=QuickQuoteHelperClass.FirstOrLast.First)
                        '    If initialStateLoc IsNot Nothing AndAlso initialStateLoc.PolicyUnderwritings IsNot Nothing Then
                        '        initialStateLoc.PolicyUnderwritings.Add(newpuw)
                        '    End If
                        'Next
                        'note: above logic is most accurate, but since we should only have 1 loc per state and above code wouldn't pull anything when state is not defined on loc, the code below would be safest
                        If QuoteObj.Locations IsNot Nothing AndAlso QuoteObj.Locations.Count > 0 Then
                            For Each l As QuickQuoteLocation In QuoteObj.Locations
                                If l.PolicyUnderwritings IsNot Nothing Then
                                    l.PolicyUnderwritings.Add(newpuw)
                                End If
                            Next
                        End If
                    Else
                        QuoteObj.Locations(0).PolicyUnderwritings.Add(newpuw)
                    End If
                    SomethingChanged = True
                End If

                ' Save the quote
                If SomethingChanged Then RaiseEvent SaveRequested(8, "ctlUWQuestions")

                Exit Sub
            Catch ex As Exception
                HandleError("CheckHOMQuestion9", ex)
                Exit Sub
            End Try
        End If
    End Sub

    ''' <summary>
    ''' HOM Questions 10 and 13 should default to "NO"
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CheckHOMQuestions10_13()
        Dim newpuw As QuickQuotePolicyUnderwriting = Nothing
        Dim q As List(Of VR.Common.UWQuestions.VRUWQuestion) = Nothing
        Dim q10 As VR.Common.UWQuestions.VRUWQuestion = Nothing
        Dim q13 As VR.Common.UWQuestions.VRUWQuestion = Nothing
        Dim found10 As Boolean = False
        Dim found13 As Boolean = False
        Dim hasCoverage As Boolean = False
        Dim SomethingChanged As Boolean = False

        Try
            'added 8/10/2018 for multi-state
            'Dim multiStateQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject) = Nothing
            Dim quoteStates As List(Of QuickQuoteHelperClass.QuickQuoteState) = Nothing
            Dim isMultiState As Boolean = False
            Dim qqHelper As New QuickQuoteHelperClass
            If QuoteObj IsNot Nothing Then
                'multiStateQuotes = qqHelper.MultiStateQuickQuoteObjects(QuoteObj) 'should always return at least QuoteObj in the list
                quoteStates = QuoteObj.QuoteStates
                If quoteStates IsNot Nothing AndAlso quoteStates.Count > 1 Then
                    isMultiState = True 'note: could have used QuoteObj.HasMultipleQuoteStates, which would also use QuoteObj.QuoteStates
                End If
            Else
                Exit Sub
            End If

            ' Only applies to home quotes
            If QuoteObj.LobType <> QuickQuoteObject.QuickQuoteLobType.HomePersonal Then Exit Sub

            ' If no policyunderwritings collection, create one
            If isMultiState = True Then 'added IF 8/10/2018 for multi-state; original logic in ELSE
                'For Each s As QuickQuoteHelperClass.QuickQuoteState In quoteStates
                '    Dim initialStateLoc As QuickQuoteLocation = qqHelper.LocationForQuickQuoteState(QuoteObj.Locations, s, firstOrLastItem:=QuickQuoteHelperClass.FirstOrLast.First)
                '    If initialStateLoc IsNot Nothing AndAlso initialStateLoc.PolicyUnderwritings Is Nothing Then
                '        initialStateLoc.PolicyUnderwritings = New List(Of QuickQuotePolicyUnderwriting)
                '    End If
                'Next
                'note: above logic is most accurate, but since we should only have 1 loc per state and above code wouldn't pull anything when state is not defined on loc, the code below would be safest
                If QuoteObj.Locations IsNot Nothing AndAlso QuoteObj.Locations.Count > 0 Then
                    For Each l As QuickQuoteLocation In QuoteObj.Locations
                        If l.PolicyUnderwritings Is Nothing Then
                            l.PolicyUnderwritings = New List(Of QuickQuotePolicyUnderwriting)
                        End If
                    Next
                End If
            Else
                If QuoteObj.Locations(0).PolicyUnderwritings Is Nothing Then
                    QuoteObj.Locations(0).PolicyUnderwritings = New List(Of QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting)
                End If
            End If

            ' Get the list of HOM underwriting questions
            q = VR.Common.UWQuestions.UWQuestions.GetPersonalHomeUnderwritingQuestions(QuoteObj.EffectiveDate, QuoteObj.LobId)
            q10 = q(9)
            q13 = q(12)

            ' ******************
            ' QUESTION 10
            ' ******************
            ' Find question 10  ' 9307 - Distance to Tidal Water
            hasCoverage = False
            If isMultiState = True Then 'added IF 8/10/2018 for multi-state; original logic in ELSE
                'for multiState; looking in each stateQuote, but don't have to drill into multiStateQuotes list since all state locs are pushed up to top level
                For Each l As QuickQuoteLocation In QuoteObj.Locations
                    If l.PolicyUnderwritings IsNot Nothing AndAlso l.PolicyUnderwritings.Count > 0 Then
                        For Each p As QuickQuotePolicyUnderwriting In l.PolicyUnderwritings
                            If p.PolicyUnderwritingCodeId = q10.PolicyUnderwritingCodeId Then
                                found10 = True
                                If p.PolicyUnderwritingAnswer Is Nothing OrElse (p.PolicyUnderwritingAnswer <> "-1" AndAlso p.PolicyUnderwritingAnswer <> "1") Then
                                    p.PolicyUnderwritingAnswer = "-1" ' NO
                                    SomethingChanged = True
                                End If
                                Exit For
                            End If
                        Next
                    End If
                Next
            Else
                For Each p As QuickQuotePolicyUnderwriting In QuoteObj.Locations(0).PolicyUnderwritings
                    If p.PolicyUnderwritingCodeId = q10.PolicyUnderwritingCodeId Then
                        found10 = True
                        If p.PolicyUnderwritingAnswer Is Nothing OrElse (p.PolicyUnderwritingAnswer <> "-1" AndAlso p.PolicyUnderwritingAnswer <> "1") Then
                            p.PolicyUnderwritingAnswer = "-1" ' NO
                            SomethingChanged = True
                        End If
                        Exit For
                    End If
                Next
            End If
            ' If Question 10 was not found we need to insert it
            If Not found10 Then
                newpuw = New QuickQuotePolicyUnderwriting()
                newpuw.PolicyId = q10.PolicyId
                newpuw.PolicyImageNum = q10.PolicyImageNum
                newpuw.PolicyUnderwriterDate = q10.PolicyUnderwriterDate
                newpuw.PolicyUnderwritingAnswer = "-1"  ' DEFAULT ANSWER TO NO
                newpuw.PolicyUnderwritingAnswerTypeId = q10.PolicyUnderwritingAnswerTypeId
                newpuw.PolicyUnderwritingCodeId = q10.PolicyUnderwritingCodeId
                newpuw.PolicyUnderwritingExtraAnswer = q10.PolicyUnderwritingExtraAnswer
                newpuw.PolicyUnderwritingExtraAnswerTypeId = q10.PolicyUnderwritingExtraAnswerTypeId
                newpuw.PolicyUnderwritingLevelId = "3"
                newpuw.PolicyUnderwritingNum = q10.PolicyUnderwritingNum
                newpuw.PolicyUnderwritingTabId = "1"
                If isMultiState = True Then 'added IF 8/10/2018 for multi-state; original logic in ELSE
                    'For Each s As QuickQuoteHelperClass.QuickQuoteState In quoteStates
                    '    Dim initialStateLoc As QuickQuoteLocation = qqHelper.LocationForQuickQuoteState(QuoteObj.Locations, s, firstOrLastItem:=QuickQuoteHelperClass.FirstOrLast.First)
                    '    If initialStateLoc IsNot Nothing AndAlso initialStateLoc.PolicyUnderwritings IsNot Nothing Then
                    '        initialStateLoc.PolicyUnderwritings.Add(newpuw)
                    '    End If
                    'Next
                    'note: above logic is most accurate, but since we should only have 1 loc per state and above code wouldn't pull anything when state is not defined on loc, the code below would be safest
                    If QuoteObj.Locations IsNot Nothing AndAlso QuoteObj.Locations.Count > 0 Then
                        For Each l As QuickQuoteLocation In QuoteObj.Locations
                            If l.PolicyUnderwritings IsNot Nothing Then
                                l.PolicyUnderwritings.Add(newpuw)
                            End If
                        Next
                    End If
                Else
                    QuoteObj.Locations(0).PolicyUnderwritings.Add(newpuw)
                End If
                SomethingChanged = True
            End If

            ' ******************
            ' QUESTION 13
            ' ******************
            ' Find question 13  ' 9310 - Is the building retrofitted for earthquake?
            hasCoverage = False
            If isMultiState = True Then 'added IF 8/10/2018 for multi-state; original logic in ELSE
                'for multiState; looking in each stateQuote, but don't have to drill into multiStateQuotes list since all state locs are pushed up to top level
                For Each l As QuickQuoteLocation In QuoteObj.Locations
                    If l.PolicyUnderwritings IsNot Nothing AndAlso l.PolicyUnderwritings.Count > 0 Then
                        For Each p As QuickQuotePolicyUnderwriting In l.PolicyUnderwritings
                            If p.PolicyUnderwritingCodeId = q13.PolicyUnderwritingCodeId Then
                                found13 = True
                                If p.PolicyUnderwritingAnswer Is Nothing OrElse (p.PolicyUnderwritingAnswer <> "-1" AndAlso p.PolicyUnderwritingAnswer <> "1") Then
                                    p.PolicyUnderwritingAnswer = "-1"
                                    SomethingChanged = True
                                End If
                                Exit For
                            End If
                        Next
                    End If
                Next
            Else
                For Each p As QuickQuotePolicyUnderwriting In QuoteObj.Locations(0).PolicyUnderwritings
                    If p.PolicyUnderwritingCodeId = q13.PolicyUnderwritingCodeId Then
                        found13 = True
                        If p.PolicyUnderwritingAnswer Is Nothing OrElse (p.PolicyUnderwritingAnswer <> "-1" AndAlso p.PolicyUnderwritingAnswer <> "1") Then
                            p.PolicyUnderwritingAnswer = "-1"
                            SomethingChanged = True
                        End If
                        Exit For
                    End If
                Next
            End If
            ' If Question 13 was not found we need to insert it
            If Not found13 Then
                newpuw = New QuickQuotePolicyUnderwriting()
                newpuw.PolicyId = q13.PolicyId
                newpuw.PolicyImageNum = q13.PolicyImageNum
                newpuw.PolicyUnderwriterDate = q13.PolicyUnderwriterDate
                newpuw.PolicyUnderwritingAnswer = "-1"  ' DEFAULT ANSWER TO NO
                newpuw.PolicyUnderwritingAnswerTypeId = q13.PolicyUnderwritingAnswerTypeId
                newpuw.PolicyUnderwritingCodeId = q13.PolicyUnderwritingCodeId
                newpuw.PolicyUnderwritingExtraAnswer = q13.PolicyUnderwritingExtraAnswer
                newpuw.PolicyUnderwritingExtraAnswerTypeId = q13.PolicyUnderwritingExtraAnswerTypeId
                newpuw.PolicyUnderwritingLevelId = "3"
                newpuw.PolicyUnderwritingNum = q13.PolicyUnderwritingNum
                newpuw.PolicyUnderwritingTabId = "1"
                If isMultiState = True Then 'added IF 8/10/2018 for multi-state; original logic in ELSE
                    'For Each s As QuickQuoteHelperClass.QuickQuoteState In quoteStates
                    '    Dim initialStateLoc As QuickQuoteLocation = qqHelper.LocationForQuickQuoteState(QuoteObj.Locations, s, firstOrLastItem:=QuickQuoteHelperClass.FirstOrLast.First)
                    '    If initialStateLoc IsNot Nothing AndAlso initialStateLoc.PolicyUnderwritings IsNot Nothing Then
                    '        initialStateLoc.PolicyUnderwritings.Add(newpuw)
                    '    End If
                    'Next
                    'note: above logic is most accurate, but since we should only have 1 loc per state and above code wouldn't pull anything when state is not defined on loc, the code below would be safest
                    If QuoteObj.Locations IsNot Nothing AndAlso QuoteObj.Locations.Count > 0 Then
                        For Each l As QuickQuoteLocation In QuoteObj.Locations
                            If l.PolicyUnderwritings IsNot Nothing Then
                                l.PolicyUnderwritings.Add(newpuw)
                            End If
                        Next
                    End If
                Else
                    QuoteObj.Locations(0).PolicyUnderwritings.Add(newpuw)
                End If
                SomethingChanged = True
            End If

            ' Save the quote
            If SomethingChanged Then RaiseEvent SaveRequested(0, "ctlUWQuestions")

            Exit Sub
        Catch ex As Exception
            HandleError("CheckHOMQuestions10_13", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub CheckHOMQuestion21()
        Dim newpuw As QuickQuotePolicyUnderwriting = Nothing
        Dim q As List(Of VR.Common.UWQuestions.VRUWQuestion) = Nothing
        Dim q21 As VR.Common.UWQuestions.VRUWQuestion = Nothing
        Dim foundit As Boolean = False
        Dim hasCoverage As Boolean = False

        Try
            'added 8/10/2018 for multi-state
            'Dim multiStateQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject) = Nothing
            Dim quoteStates As List(Of QuickQuoteHelperClass.QuickQuoteState) = Nothing
            Dim isMultiState As Boolean = False
            Dim qqHelper As New QuickQuoteHelperClass
            If QuoteObj IsNot Nothing Then
                'multiStateQuotes = qqHelper.MultiStateQuickQuoteObjects(QuoteObj) 'should always return at least QuoteObj in the list
                quoteStates = QuoteObj.QuoteStates
                If quoteStates IsNot Nothing AndAlso quoteStates.Count > 1 Then
                    isMultiState = True 'note: could have used QuoteObj.HasMultipleQuoteStates, which would also use QuoteObj.QuoteStates
                End If
            Else
                Exit Sub
            End If

            ' Only applies to home quotes
            If QuoteObj.LobType <> QuickQuoteObject.QuickQuoteLobType.HomePersonal Then Exit Sub

            ' The coverages are located on the location so if there's no location then the coverages are obviously not there
            If QuoteObj.Locations Is Nothing OrElse QuoteObj.Locations.Count <= 0 Then Exit Sub

            ' Check to see if the trampoline surcharge is on the quote
            ' If it's not there do nothing
            If Not QuoteObj.Locations(0).TrampolineSurcharge Then Exit Sub

            ' If the trampoline surcharge exists...
            ' Check to see if there's an existing Question 21 in the policyunderwritings
            newpuw = New QuickQuotePolicyUnderwriting()
            q = VR.Common.UWQuestions.UWQuestions.GetPersonalHomeUnderwritingQuestions(QuoteObj.EffectiveDate, QuoteObj.LobId)
            q21 = q(20)
            For Each uwq As QuickQuotePolicyUnderwriting In QuoteObj.Locations(0).PolicyUnderwritings
                If uwq.PolicyUnderwritingCodeId = q21.PolicyUnderwritingCodeId Then
                    ' If we find the question then there is nothing for us to do here
                    foundit = True
                    Exit Sub
                End If
            Next

            ' We should only get here if Question 21 was not found in the existing location policyunderwritings
            ' There is no existing question 21 in the policyunderwritings; add it and default the answer to YES
            newpuw.PolicyId = q(20).PolicyId
            newpuw.PolicyImageNum = q(20).PolicyImageNum
            newpuw.PolicyUnderwriterDate = q(20).PolicyUnderwriterDate
            newpuw.PolicyUnderwritingAnswer = 1
            newpuw.PolicyUnderwritingAnswerTypeId = q(20).PolicyUnderwritingAnswerTypeId
            newpuw.PolicyUnderwritingCodeId = q(20).PolicyUnderwritingCodeId
            newpuw.PolicyUnderwritingExtraAnswer = q(20).PolicyUnderwritingExtraAnswer
            newpuw.PolicyUnderwritingExtraAnswerTypeId = q(20).PolicyUnderwritingExtraAnswerTypeId
            newpuw.PolicyUnderwritingLevelId = q(20).PolicyUnderwritingLevelId
            newpuw.PolicyUnderwritingNum = q(20).PolicyUnderwritingNum
            newpuw.PolicyUnderwritingTabId = q(20).PolicyUnderwritingTabId
            If isMultiState = True Then 'added IF 8/10/2018 for multi-state; original logic in ELSE
                'For Each s As QuickQuoteHelperClass.QuickQuoteState In quoteStates
                '    Dim initialStateLoc As QuickQuoteLocation = qqHelper.LocationForQuickQuoteState(QuoteObj.Locations, s, firstOrLastItem:=QuickQuoteHelperClass.FirstOrLast.First)
                '    If initialStateLoc IsNot Nothing AndAlso initialStateLoc.PolicyUnderwritings IsNot Nothing Then
                '        initialStateLoc.PolicyUnderwritings.Add(newpuw)
                '    End If
                'Next
                'note: above logic is most accurate, but since we should only have 1 loc per state and above code wouldn't pull anything when state is not defined on loc, the code below would be safest
                If QuoteObj.Locations IsNot Nothing AndAlso QuoteObj.Locations.Count > 0 Then
                    For Each l As QuickQuoteLocation In QuoteObj.Locations
                        If l.PolicyUnderwritings IsNot Nothing Then
                            l.PolicyUnderwritings.Add(newpuw)
                        End If
                    Next
                End If
            Else
                QuoteObj.Locations(0).PolicyUnderwritings.Add(newpuw)
            End If

            ' Save the quote
            RaiseEvent SaveRequested(0, "ctlUWQuestions")

            Exit Sub
        Catch ex As Exception
            HandleError("CheckHOMQuestion21", ex)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' DFR Questions 15, 16 and 17 should default to "NO"
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CheckDFRQuestions15_17()
        Dim newpuw As QuickQuotePolicyUnderwriting = Nothing
        Dim q As List(Of VR.Common.UWQuestions.VRUWQuestion) = Nothing
        Dim q15 As VR.Common.UWQuestions.VRUWQuestion = Nothing
        Dim q16 As VR.Common.UWQuestions.VRUWQuestion = Nothing
        Dim q17 As VR.Common.UWQuestions.VRUWQuestion = Nothing
        Dim found15 As Boolean = False
        Dim found16 As Boolean = False
        Dim found17 As Boolean = False
        Dim hasCoverage As Boolean = False
        Dim SomethingChanged As Boolean = False

        Try
            'added 8/10/2018 for multi-state
            'Dim multiStateQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject) = Nothing
            Dim quoteStates As List(Of QuickQuoteHelperClass.QuickQuoteState) = Nothing
            Dim isMultiState As Boolean = False
            Dim qqHelper As New QuickQuoteHelperClass
            If QuoteObj IsNot Nothing Then
                'multiStateQuotes = qqHelper.MultiStateQuickQuoteObjects(QuoteObj) 'should always return at least QuoteObj in the list
                quoteStates = QuoteObj.QuoteStates
                If quoteStates IsNot Nothing AndAlso quoteStates.Count > 1 Then
                    isMultiState = True 'note: could have used QuoteObj.HasMultipleQuoteStates, which would also use QuoteObj.QuoteStates
                End If
            Else
                Exit Sub
            End If

            ' Only applies to DFR quotes
            If QuoteObj.LobType <> QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal Then Exit Sub

            ' If no policyunderwritings collection, create one
            If isMultiState = True Then 'added IF 8/10/2018 for multi-state; original logic in ELSE
                'For Each s As QuickQuoteHelperClass.QuickQuoteState In quoteStates
                '    Dim initialStateLoc As QuickQuoteLocation = qqHelper.LocationForQuickQuoteState(QuoteObj.Locations, s, firstOrLastItem:=QuickQuoteHelperClass.FirstOrLast.First)
                '    If initialStateLoc IsNot Nothing AndAlso initialStateLoc.PolicyUnderwritings Is Nothing Then
                '        initialStateLoc.PolicyUnderwritings = New List(Of QuickQuotePolicyUnderwriting)
                '    End If
                'Next
                'note: above logic is most accurate, but since we should only have 1 loc per state and above code wouldn't pull anything when state is not defined on loc, the code below would be safest
                If QuoteObj.Locations IsNot Nothing AndAlso QuoteObj.Locations.Count > 0 Then
                    For Each l As QuickQuoteLocation In QuoteObj.Locations
                        If l.PolicyUnderwritings Is Nothing Then
                            l.PolicyUnderwritings = New List(Of QuickQuotePolicyUnderwriting)
                        End If
                    Next
                End If
            Else
                If QuoteObj.Locations(0).PolicyUnderwritings Is Nothing Then
                    QuoteObj.Locations(0).PolicyUnderwritings = New List(Of QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting)
                End If
            End If

            ' Get the list of DFR underwriting questions
            'Updated 7/27/2022 for task 75803 MLW
            'q = VR.Common.UWQuestions.UWQuestions.GetDwellingFireUnderwritingQuestions()
            q = VR.Common.UWQuestions.UWQuestions.GetDwellingFireUnderwritingQuestions(QuoteObj.EffectiveDate)
            q15 = q(14)
            q16 = q(15)
            q17 = q(16)

            ' ******************
            ' QUESTION 15
            ' ******************
            ' Find question 15 
            hasCoverage = False
            If isMultiState = True Then 'added IF 8/10/2018 for multi-state; original logic in ELSE
                'for multiState; looking in each stateQuote, but don't have to drill into multiStateQuotes list since all state locs are pushed up to top level
                For Each l As QuickQuoteLocation In QuoteObj.Locations
                    If l.PolicyUnderwritings IsNot Nothing AndAlso l.PolicyUnderwritings.Count > 0 Then
                        For Each p As QuickQuotePolicyUnderwriting In l.PolicyUnderwritings
                            If p.PolicyUnderwritingCodeId = q15.PolicyUnderwritingCodeId Then
                                found15 = True
                                If p.PolicyUnderwritingAnswer Is Nothing OrElse (p.PolicyUnderwritingAnswer <> "-1" AndAlso p.PolicyUnderwritingAnswer <> "1") Then
                                    p.PolicyUnderwritingAnswer = "-1" ' NO
                                    SomethingChanged = True
                                End If
                                Exit For
                            End If
                        Next
                    End If
                Next
            Else
                For Each p As QuickQuotePolicyUnderwriting In QuoteObj.Locations(0).PolicyUnderwritings
                    If p.PolicyUnderwritingCodeId = q15.PolicyUnderwritingCodeId Then
                        found15 = True
                        If p.PolicyUnderwritingAnswer Is Nothing OrElse (p.PolicyUnderwritingAnswer <> "-1" AndAlso p.PolicyUnderwritingAnswer <> "1") Then
                            p.PolicyUnderwritingAnswer = "-1" ' NO
                            SomethingChanged = True
                        End If
                        Exit For
                    End If
                Next
            End If
            ' If Question 15 was not found we need to insert it
            If Not found15 Then
                newpuw = New QuickQuotePolicyUnderwriting()
                newpuw.PolicyId = q15.PolicyId
                newpuw.PolicyImageNum = q15.PolicyImageNum
                newpuw.PolicyUnderwriterDate = q15.PolicyUnderwriterDate
                newpuw.PolicyUnderwritingAnswer = "-1"  ' DEFAULT ANSWER TO NO
                newpuw.PolicyUnderwritingAnswerTypeId = q15.PolicyUnderwritingAnswerTypeId
                newpuw.PolicyUnderwritingCodeId = q15.PolicyUnderwritingCodeId
                newpuw.PolicyUnderwritingExtraAnswer = q15.PolicyUnderwritingExtraAnswer
                newpuw.PolicyUnderwritingExtraAnswerTypeId = q15.PolicyUnderwritingExtraAnswerTypeId
                newpuw.PolicyUnderwritingLevelId = "3"
                newpuw.PolicyUnderwritingNum = q15.PolicyUnderwritingNum
                newpuw.PolicyUnderwritingTabId = "1"
                If isMultiState = True Then 'added IF 8/10/2018 for multi-state; original logic in ELSE
                    'For Each s As QuickQuoteHelperClass.QuickQuoteState In quoteStates
                    '    Dim initialStateLoc As QuickQuoteLocation = qqHelper.LocationForQuickQuoteState(QuoteObj.Locations, s, firstOrLastItem:=QuickQuoteHelperClass.FirstOrLast.First)
                    '    If initialStateLoc IsNot Nothing AndAlso initialStateLoc.PolicyUnderwritings IsNot Nothing Then
                    '        initialStateLoc.PolicyUnderwritings.Add(newpuw)
                    '    End If
                    'Next
                    'note: above logic is most accurate, but since we should only have 1 loc per state and above code wouldn't pull anything when state is not defined on loc, the code below would be safest
                    If QuoteObj.Locations IsNot Nothing AndAlso QuoteObj.Locations.Count > 0 Then
                        For Each l As QuickQuoteLocation In QuoteObj.Locations
                            If l.PolicyUnderwritings IsNot Nothing Then
                                l.PolicyUnderwritings.Add(newpuw)
                            End If
                        Next
                    End If
                Else
                    QuoteObj.Locations(0).PolicyUnderwritings.Add(newpuw)
                End If
                SomethingChanged = True
            End If

            ' ******************
            ' QUESTION 16
            ' ******************
            ' Find question 16 
            hasCoverage = False
            If isMultiState = True Then 'added IF 8/10/2018 for multi-state; original logic in ELSE
                'for multiState; looking in each stateQuote, but don't have to drill into multiStateQuotes list since all state locs are pushed up to top level
                For Each l As QuickQuoteLocation In QuoteObj.Locations
                    If l.PolicyUnderwritings IsNot Nothing AndAlso l.PolicyUnderwritings.Count > 0 Then
                        For Each p As QuickQuotePolicyUnderwriting In l.PolicyUnderwritings
                            If p.PolicyUnderwritingCodeId = q16.PolicyUnderwritingCodeId Then
                                found16 = True
                                If p.PolicyUnderwritingAnswer Is Nothing OrElse (p.PolicyUnderwritingAnswer <> "-1" AndAlso p.PolicyUnderwritingAnswer <> "1") Then
                                    p.PolicyUnderwritingAnswer = "-1" ' NO
                                    SomethingChanged = True
                                End If
                                Exit For
                            End If
                        Next
                    End If
                Next
            Else
                For Each p As QuickQuotePolicyUnderwriting In QuoteObj.Locations(0).PolicyUnderwritings
                    If p.PolicyUnderwritingCodeId = q16.PolicyUnderwritingCodeId Then
                        found16 = True
                        If p.PolicyUnderwritingAnswer Is Nothing OrElse (p.PolicyUnderwritingAnswer <> "-1" AndAlso p.PolicyUnderwritingAnswer <> "1") Then
                            p.PolicyUnderwritingAnswer = "-1" ' NO
                            SomethingChanged = True
                        End If
                        Exit For
                    End If
                Next
            End If
            ' If Question 16 was not found we need to insert it
            If Not found16 Then
                newpuw = New QuickQuotePolicyUnderwriting()
                newpuw.PolicyId = q16.PolicyId
                newpuw.PolicyImageNum = q16.PolicyImageNum
                newpuw.PolicyUnderwriterDate = q16.PolicyUnderwriterDate
                newpuw.PolicyUnderwritingAnswer = "-1"  ' DEFAULT ANSWER TO NO
                newpuw.PolicyUnderwritingAnswerTypeId = q16.PolicyUnderwritingAnswerTypeId
                newpuw.PolicyUnderwritingCodeId = q16.PolicyUnderwritingCodeId
                newpuw.PolicyUnderwritingExtraAnswer = q16.PolicyUnderwritingExtraAnswer
                newpuw.PolicyUnderwritingExtraAnswerTypeId = q16.PolicyUnderwritingExtraAnswerTypeId
                newpuw.PolicyUnderwritingLevelId = "3"
                newpuw.PolicyUnderwritingNum = q16.PolicyUnderwritingNum
                newpuw.PolicyUnderwritingTabId = "1"
                If isMultiState = True Then 'added IF 8/10/2018 for multi-state; original logic in ELSE
                    'For Each s As QuickQuoteHelperClass.QuickQuoteState In quoteStates
                    '    Dim initialStateLoc As QuickQuoteLocation = qqHelper.LocationForQuickQuoteState(QuoteObj.Locations, s, firstOrLastItem:=QuickQuoteHelperClass.FirstOrLast.First)
                    '    If initialStateLoc IsNot Nothing AndAlso initialStateLoc.PolicyUnderwritings IsNot Nothing Then
                    '        initialStateLoc.PolicyUnderwritings.Add(newpuw)
                    '    End If
                    'Next
                    'note: above logic is most accurate, but since we should only have 1 loc per state and above code wouldn't pull anything when state is not defined on loc, the code below would be safest
                    If QuoteObj.Locations IsNot Nothing AndAlso QuoteObj.Locations.Count > 0 Then
                        For Each l As QuickQuoteLocation In QuoteObj.Locations
                            If l.PolicyUnderwritings IsNot Nothing Then
                                l.PolicyUnderwritings.Add(newpuw)
                            End If
                        Next
                    End If
                Else
                    QuoteObj.Locations(0).PolicyUnderwritings.Add(newpuw)
                End If
                SomethingChanged = True
            End If

            ' ******************
            ' QUESTION 17
            ' ******************
            ' Find question 17 
            hasCoverage = False
            If isMultiState = True Then 'added IF 8/10/2018 for multi-state; original logic in ELSE
                'for multiState; looking in each stateQuote, but don't have to drill into multiStateQuotes list since all state locs are pushed up to top level
                For Each l As QuickQuoteLocation In QuoteObj.Locations
                    If l.PolicyUnderwritings IsNot Nothing AndAlso l.PolicyUnderwritings.Count > 0 Then
                        For Each p As QuickQuotePolicyUnderwriting In l.PolicyUnderwritings
                            If p.PolicyUnderwritingCodeId = q17.PolicyUnderwritingCodeId Then
                                found17 = True
                                If p.PolicyUnderwritingAnswer Is Nothing OrElse (p.PolicyUnderwritingAnswer <> "-1" AndAlso p.PolicyUnderwritingAnswer <> "1") Then
                                    p.PolicyUnderwritingAnswer = "-1" ' NO
                                    SomethingChanged = True
                                End If
                                Exit For
                            End If
                        Next
                    End If
                Next
            Else
                For Each p As QuickQuotePolicyUnderwriting In QuoteObj.Locations(0).PolicyUnderwritings
                    If p.PolicyUnderwritingCodeId = q17.PolicyUnderwritingCodeId Then
                        found17 = True
                        If p.PolicyUnderwritingAnswer Is Nothing OrElse (p.PolicyUnderwritingAnswer <> "-1" AndAlso p.PolicyUnderwritingAnswer <> "1") Then
                            p.PolicyUnderwritingAnswer = "-1" ' NO
                            SomethingChanged = True
                        End If
                        Exit For
                    End If
                Next
            End If
            ' If Question 17 was not found we need to insert it
            If Not found17 Then
                newpuw = New QuickQuotePolicyUnderwriting()
                newpuw.PolicyId = q17.PolicyId
                newpuw.PolicyImageNum = q17.PolicyImageNum
                newpuw.PolicyUnderwriterDate = q17.PolicyUnderwriterDate
                newpuw.PolicyUnderwritingAnswer = "-1"  ' DEFAULT ANSWER TO NO
                newpuw.PolicyUnderwritingAnswerTypeId = q17.PolicyUnderwritingAnswerTypeId
                newpuw.PolicyUnderwritingCodeId = q17.PolicyUnderwritingCodeId
                newpuw.PolicyUnderwritingExtraAnswer = q17.PolicyUnderwritingExtraAnswer
                newpuw.PolicyUnderwritingExtraAnswerTypeId = q17.PolicyUnderwritingExtraAnswerTypeId
                newpuw.PolicyUnderwritingLevelId = "3"
                newpuw.PolicyUnderwritingNum = q17.PolicyUnderwritingNum
                newpuw.PolicyUnderwritingTabId = "1"
                If isMultiState = True Then 'added IF 8/10/2018 for multi-state; original logic in ELSE
                    'For Each s As QuickQuoteHelperClass.QuickQuoteState In quoteStates
                    '    Dim initialStateLoc As QuickQuoteLocation = qqHelper.LocationForQuickQuoteState(QuoteObj.Locations, s, firstOrLastItem:=QuickQuoteHelperClass.FirstOrLast.First)
                    '    If initialStateLoc IsNot Nothing AndAlso initialStateLoc.PolicyUnderwritings IsNot Nothing Then
                    '        initialStateLoc.PolicyUnderwritings.Add(newpuw)
                    '    End If
                    'Next
                    'note: above logic is most accurate, but since we should only have 1 loc per state and above code wouldn't pull anything when state is not defined on loc, the code below would be safest
                    If QuoteObj.Locations IsNot Nothing AndAlso QuoteObj.Locations.Count > 0 Then
                        For Each l As QuickQuoteLocation In QuoteObj.Locations
                            If l.PolicyUnderwritings IsNot Nothing Then
                                l.PolicyUnderwritings.Add(newpuw)
                            End If
                        Next
                    End If
                Else
                    QuoteObj.Locations(0).PolicyUnderwritings.Add(newpuw)
                End If
                SomethingChanged = True
            End If

            ' Save the quote
            If SomethingChanged Then RaiseEvent SaveRequested(0, "ctlUWQuestions")

            Exit Sub
        Catch ex As Exception
            HandleError("CheckDFRQuestions15_17", ex)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' DFR Questions 10 and 13 should default to "NO"
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CheckDFRQuestions10_13()
        Dim newpuw As QuickQuotePolicyUnderwriting = Nothing
        Dim q As List(Of VR.Common.UWQuestions.VRUWQuestion) = Nothing
        Dim q10 As VR.Common.UWQuestions.VRUWQuestion = Nothing
        Dim q13 As VR.Common.UWQuestions.VRUWQuestion = Nothing
        Dim found10 As Boolean = False
        Dim found13 As Boolean = False
        Dim hasCoverage As Boolean = False
        Dim SomethingChanged As Boolean = False

        Try
            'added 8/10/2018 for multi-state
            'Dim multiStateQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject) = Nothing
            Dim quoteStates As List(Of QuickQuoteHelperClass.QuickQuoteState) = Nothing
            Dim isMultiState As Boolean = False
            Dim qqHelper As New QuickQuoteHelperClass
            If QuoteObj IsNot Nothing Then
                'multiStateQuotes = qqHelper.MultiStateQuickQuoteObjects(QuoteObj) 'should always return at least QuoteObj in the list
                quoteStates = QuoteObj.QuoteStates
                If quoteStates IsNot Nothing AndAlso quoteStates.Count > 1 Then
                    isMultiState = True 'note: could have used QuoteObj.HasMultipleQuoteStates, which would also use QuoteObj.QuoteStates
                End If
            Else
                Exit Sub
            End If

            ' Only applies to DFR quotes
            If QuoteObj.LobType <> QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal Then Exit Sub

            ' If no policyunderwritings collection, create one
            If isMultiState = True Then 'added IF 8/10/2018 for multi-state; original logic in ELSE
                'For Each s As QuickQuoteHelperClass.QuickQuoteState In quoteStates
                '    Dim initialStateLoc As QuickQuoteLocation = qqHelper.LocationForQuickQuoteState(QuoteObj.Locations, s, firstOrLastItem:=QuickQuoteHelperClass.FirstOrLast.First)
                '    If initialStateLoc IsNot Nothing AndAlso initialStateLoc.PolicyUnderwritings Is Nothing Then
                '        initialStateLoc.PolicyUnderwritings = New List(Of QuickQuotePolicyUnderwriting)
                '    End If
                'Next
                'note: above logic is most accurate, but since we should only have 1 loc per state and above code wouldn't pull anything when state is not defined on loc, the code below would be safest
                If QuoteObj.Locations IsNot Nothing AndAlso QuoteObj.Locations.Count > 0 Then
                    For Each l As QuickQuoteLocation In QuoteObj.Locations
                        If l.PolicyUnderwritings Is Nothing Then
                            l.PolicyUnderwritings = New List(Of QuickQuotePolicyUnderwriting)
                        End If
                    Next
                End If
            Else
                If QuoteObj.Locations(0).PolicyUnderwritings Is Nothing Then
                    QuoteObj.Locations(0).PolicyUnderwritings = New List(Of QuickQuote.CommonObjects.QuickQuotePolicyUnderwriting)
                End If
            End If

            ' Get the list of DFR underwriting questions
            'Updated 7/27/2022 for task 75803 MLW
            'q = VR.Common.UWQuestions.UWQuestions.GetDwellingFireUnderwritingQuestions()
            q = VR.Common.UWQuestions.UWQuestions.GetDwellingFireUnderwritingQuestions(QuoteObj.EffectiveDate)
            q10 = q(9)
            q13 = q(12)

            ' ******************
            ' QUESTION 10
            ' ******************
            ' Find question 10
            hasCoverage = False
            If isMultiState = True Then 'added IF 8/10/2018 for multi-state; original logic in ELSE
                'for multiState; looking in each stateQuote, but don't have to drill into multiStateQuotes list since all state locs are pushed up to top level
                For Each l As QuickQuoteLocation In QuoteObj.Locations
                    If l.PolicyUnderwritings IsNot Nothing AndAlso l.PolicyUnderwritings.Count > 0 Then
                        For Each p As QuickQuotePolicyUnderwriting In l.PolicyUnderwritings
                            If p.PolicyUnderwritingCodeId = q10.PolicyUnderwritingCodeId Then
                                found10 = True
                                If p.PolicyUnderwritingAnswer Is Nothing OrElse (p.PolicyUnderwritingAnswer <> "-1" AndAlso p.PolicyUnderwritingAnswer <> "1") Then
                                    p.PolicyUnderwritingAnswer = "-1" ' NO
                                    SomethingChanged = True
                                End If
                                Exit For
                            End If
                        Next
                    End If
                Next
            Else
                For Each p As QuickQuotePolicyUnderwriting In QuoteObj.Locations(0).PolicyUnderwritings
                    If p.PolicyUnderwritingCodeId = q10.PolicyUnderwritingCodeId Then
                        found10 = True
                        If p.PolicyUnderwritingAnswer Is Nothing OrElse (p.PolicyUnderwritingAnswer <> "-1" AndAlso p.PolicyUnderwritingAnswer <> "1") Then
                            p.PolicyUnderwritingAnswer = "-1" ' NO
                            SomethingChanged = True
                        End If
                        Exit For
                    End If
                Next
            End If
            ' If Question 10 was not found we need to insert it
            If Not found10 Then
                newpuw = New QuickQuotePolicyUnderwriting()
                newpuw.PolicyId = q10.PolicyId
                newpuw.PolicyImageNum = q10.PolicyImageNum
                newpuw.PolicyUnderwriterDate = q10.PolicyUnderwriterDate
                newpuw.PolicyUnderwritingAnswer = "-1"  ' DEFAULT ANSWER TO NO
                newpuw.PolicyUnderwritingAnswerTypeId = q10.PolicyUnderwritingAnswerTypeId
                newpuw.PolicyUnderwritingCodeId = q10.PolicyUnderwritingCodeId
                newpuw.PolicyUnderwritingExtraAnswer = q10.PolicyUnderwritingExtraAnswer
                newpuw.PolicyUnderwritingExtraAnswerTypeId = q10.PolicyUnderwritingExtraAnswerTypeId
                newpuw.PolicyUnderwritingLevelId = "3"
                newpuw.PolicyUnderwritingNum = q10.PolicyUnderwritingNum
                newpuw.PolicyUnderwritingTabId = "1"
                If isMultiState = True Then 'added IF 8/10/2018 for multi-state; original logic in ELSE
                    'For Each s As QuickQuoteHelperClass.QuickQuoteState In quoteStates
                    '    Dim initialStateLoc As QuickQuoteLocation = qqHelper.LocationForQuickQuoteState(QuoteObj.Locations, s, firstOrLastItem:=QuickQuoteHelperClass.FirstOrLast.First)
                    '    If initialStateLoc IsNot Nothing AndAlso initialStateLoc.PolicyUnderwritings IsNot Nothing Then
                    '        initialStateLoc.PolicyUnderwritings.Add(newpuw)
                    '    End If
                    'Next
                    'note: above logic is most accurate, but since we should only have 1 loc per state and above code wouldn't pull anything when state is not defined on loc, the code below would be safest
                    If QuoteObj.Locations IsNot Nothing AndAlso QuoteObj.Locations.Count > 0 Then
                        For Each l As QuickQuoteLocation In QuoteObj.Locations
                            If l.PolicyUnderwritings IsNot Nothing Then
                                l.PolicyUnderwritings.Add(newpuw)
                            End If
                        Next
                    End If
                Else
                    QuoteObj.Locations(0).PolicyUnderwritings.Add(newpuw)
                End If
                SomethingChanged = True
            End If

            ' ******************
            ' QUESTION 13
            ' ******************
            ' Find question 13
            hasCoverage = False
            If isMultiState = True Then 'added IF 8/10/2018 for multi-state; original logic in ELSE
                'for multiState; looking in each stateQuote, but don't have to drill into multiStateQuotes list since all state locs are pushed up to top level
                For Each l As QuickQuoteLocation In QuoteObj.Locations
                    If l.PolicyUnderwritings IsNot Nothing AndAlso l.PolicyUnderwritings.Count > 0 Then
                        For Each p As QuickQuotePolicyUnderwriting In l.PolicyUnderwritings
                            If p.PolicyUnderwritingCodeId = q13.PolicyUnderwritingCodeId Then
                                found13 = True
                                If p.PolicyUnderwritingAnswer Is Nothing OrElse (p.PolicyUnderwritingAnswer <> "-1" AndAlso p.PolicyUnderwritingAnswer <> "1") Then
                                    p.PolicyUnderwritingAnswer = "-1" ' NO
                                    SomethingChanged = True
                                End If
                                Exit For
                            End If
                        Next
                    End If
                Next
            Else
                For Each p As QuickQuotePolicyUnderwriting In QuoteObj.Locations(0).PolicyUnderwritings
                    If p.PolicyUnderwritingCodeId = q13.PolicyUnderwritingCodeId Then
                        found13 = True
                        If p.PolicyUnderwritingAnswer Is Nothing OrElse (p.PolicyUnderwritingAnswer <> "-1" AndAlso p.PolicyUnderwritingAnswer <> "1") Then
                            p.PolicyUnderwritingAnswer = "-1" ' NO
                            SomethingChanged = True
                        End If
                        Exit For
                    End If
                Next
            End If
            ' If Question 16 was not found we need to insert it
            If Not found13 Then
                newpuw = New QuickQuotePolicyUnderwriting()
                newpuw.PolicyId = q13.PolicyId
                newpuw.PolicyImageNum = q13.PolicyImageNum
                newpuw.PolicyUnderwriterDate = q13.PolicyUnderwriterDate
                newpuw.PolicyUnderwritingAnswer = "-1"  ' DEFAULT ANSWER TO NO
                newpuw.PolicyUnderwritingAnswerTypeId = q13.PolicyUnderwritingAnswerTypeId
                newpuw.PolicyUnderwritingCodeId = q13.PolicyUnderwritingCodeId
                newpuw.PolicyUnderwritingExtraAnswer = q13.PolicyUnderwritingExtraAnswer
                newpuw.PolicyUnderwritingExtraAnswerTypeId = q13.PolicyUnderwritingExtraAnswerTypeId
                newpuw.PolicyUnderwritingLevelId = "3"
                newpuw.PolicyUnderwritingNum = q13.PolicyUnderwritingNum
                newpuw.PolicyUnderwritingTabId = "1"
                If isMultiState = True Then 'added IF 8/10/2018 for multi-state; original logic in ELSE
                    'For Each s As QuickQuoteHelperClass.QuickQuoteState In quoteStates
                    '    Dim initialStateLoc As QuickQuoteLocation = qqHelper.LocationForQuickQuoteState(QuoteObj.Locations, s, firstOrLastItem:=QuickQuoteHelperClass.FirstOrLast.First)
                    '    If initialStateLoc IsNot Nothing AndAlso initialStateLoc.PolicyUnderwritings IsNot Nothing Then
                    '        initialStateLoc.PolicyUnderwritings.Add(newpuw)
                    '    End If
                    'Next
                    'note: above logic is most accurate, but since we should only have 1 loc per state and above code wouldn't pull anything when state is not defined on loc, the code below would be safest
                    If QuoteObj.Locations IsNot Nothing AndAlso QuoteObj.Locations.Count > 0 Then
                        For Each l As QuickQuoteLocation In QuoteObj.Locations
                            If l.PolicyUnderwritings IsNot Nothing Then
                                l.PolicyUnderwritings.Add(newpuw)
                            End If
                        Next
                    End If
                Else
                    QuoteObj.Locations(0).PolicyUnderwritings.Add(newpuw)
                End If
                SomethingChanged = True
            End If


            ' Save the quote
            If SomethingChanged Then RaiseEvent SaveRequested(0, "ctlUWQuestions")

            Exit Sub
        Catch ex As Exception
            HandleError("CheckDFRQuestions10_13", ex)
            Exit Sub
        End Try
    End Sub

#End Region

#Region "Events"

    ''' <summary>
    ''' PAGE LOAD
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim tblDesc As HtmlTable = Nothing
        Dim rbY As RadioButton = Nothing
        Dim rbN As RadioButton = Nothing
        Dim myLOB As String = ""
        Dim QN As Integer = 0
        Dim HomeVersion As String = ""
        'Added 2/8/18 for HOM Upgrade MLW
        'Dim HomeVersion As String = ""
        If QuoteObj IsNot Nothing Then
            HomeVersion = GetHomeVersion(QuoteObj)
            If QuoteObj.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso QuoteObj.Locations.IsLoaded Then
                hdnFormType.Value = QuoteObj.Locations(0).FormTypeId 'needed for JS evals on HOM Upgrade
            End If
        End If

        Try
            Dim _script = DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager
            _script.AddScriptLine("$(""#UWQuestionsDiv"").accordion({collapsible: false});")

            If Not IsPostBack Then
                ' PERSONAL HOME EXCEPTIONS
                ' This will default question 1's answer on HOM quotes if coverages HO-72 and/or HO-73 are selected
                CheckHOMQuestion1()
                If (QuoteObj.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso QuoteObj.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                    ' Question 9 is the Dangerous Breed question - sets answer to Yes if Canine Liab Excl selected
                    CheckHOMQuestion9() 'added 2/8/18 for HOM Upgrade MLW
                End If
                ' Questions 10 and 13 default to NO
                CheckHOMQuestions10_13()
                ' Question 21 is the trampoline question
                CheckHOMQuestion21()

                If Not DFRStandaloneHelper.isDFRStandaloneAvailable(QuoteObj) Then
                    ' Check DFR Questions 15,16,17 (rental questions)
                    CheckDFRQuestions15_17()

                    ' Check DFR Questions 10 & 13 
                    CheckDFRQuestions10_13()
                End If

                ' Load the questions repeater
                InitializeQuestions()
            End If


            Select Case QuoteObj.LobType
                Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                    myLOB = "PPA"
                    Exit Select
                Case QuickQuoteObject.QuickQuoteLobType.HomePersonal
                    myLOB = "HOM"
                    Exit Select
                Case QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                    myLOB = "DFR"
                    Exit Select
                Case Else
                    Exit Select
            End Select
            hdnLOB.Value = myLOB

            ' Collapse/expand all of the description panels
            For Each ri As RepeaterItem In rptUWQ.Items
                QN += 1
                tblDesc = ri.FindControl("tblUWQDesc")
                rbY = ri.FindControl("rbYes")
                rbN = ri.FindControl("rbNo")
                If tblDesc Is Nothing Then Throw New Exception("Description Table not found!")
                If rbY Is Nothing Then Throw New Exception("rbYes not found!")
                If rbN Is Nothing Then Throw New Exception("rbNo not found!")
                Select Case myLOB
                    Case "DFR"
                        ' For DFR question 5, if there's any answer (Y or N), show the addl info box
                        If QN = 5 Then
                            'Updated 7/26/2022 for task 75803 MLW
                            If IFM.VR.Common.Helpers.DFR.DFRStandaloneHelper.isDFRStandaloneAvailable(QuoteObj) Then
                                If rbY.Checked Then
                                    ShowHideAdditionalInfo(tblDesc, "show")
                                Else
                                    ShowHideAdditionalInfo(tblDesc, "hide")
                                End If
                                'If rbY.Checked OrElse rbN.Checked Then
                                '    ShowHideAdditionalInfo(tblDesc, "show")
                                'Else
                                '    ShowHideAdditionalInfo(tblDesc, "hide")
                                'End If
                            Else
                                If rbY.Checked OrElse rbN.Checked Then
                                    ShowHideAdditionalInfo(tblDesc, "show")
                                Else
                                    ShowHideAdditionalInfo(tblDesc, "hide")
                                End If
                            End If
                            'If rbY.Checked OrElse rbN.Checked Then
                            '    ShowHideAdditionalInfo(tblDesc, "show")
                            'Else
                            '    ShowHideAdditionalInfo(tblDesc, "hide")
                            'End If
                        Else
                            ' All DFR questions other than #5 get show the addl info box on Yes only
                            If rbY.Checked Then
                                ShowHideAdditionalInfo(tblDesc, "show")
                            Else
                                ShowHideAdditionalInfo(tblDesc, "hide")
                            End If
                        End If
                        Exit Select
                    Case "PPA"
                        If rbY.Checked Then
                            ' PPA Question 16 does not require an additional info textbox
                            If QN = 16 Then
                                ShowHideAdditionalInfo(tblDesc, "hide")
                            Else
                                ShowHideAdditionalInfo(tblDesc, "show")
                            End If
                        Else
                            ShowHideAdditionalInfo(tblDesc, "hide")
                        End If
                        Exit Select
                    Case "HOM"
                        If rptUWQ.Items.Count = 27 Then 'updated 12/1/17 for HOM Upgrade MLW - changed to 27
                            ' Full complement of HOM questions
                            If rbY.Checked Then
                                ' HOM Questions 6, 14, 15, 16, 17 and 21 do not require an additional info textbox
                                Select Case QN
                                    Case 6, 14, 15, 16, 17, 21, 27 'updated 12/1/17 for HOM Upgrade MLW - added 27 'updated 2/8/18 for HOM Upgrade removed 9 Dangerous Breed
                                        ShowHideAdditionalInfo(tblDesc, "hide")
                                        Exit Select
                                    Case 9
                                        'Updated 2/8/18 for HOM Upgrade - removed 9 Dangerous Breed
                                        If (QuoteObj.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso QuoteObj.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                            ShowHideAdditionalInfo(tblDesc, "hide")
                                            Exit Select
                                        Else
                                            ShowHideAdditionalInfo(tblDesc, "show")
                                            Exit Select
                                        End If
                                    Case Else
                                        ShowHideAdditionalInfo(tblDesc, "show")
                                        Exit Select
                                End Select
                            Else
                                ShowHideAdditionalInfo(tblDesc, "hide")
                            End If
                        Else
                            ' HOM Questions 15,16,17 are hidden because of the policy form selected
                            If rbY.Checked Then
                                ' HOM Questions 6, 9, 14, and 21 (number 18 here because of the missing questions) do not require an additional info textbox
                                Select Case QN
                                    Case 6, 14, 18 'Updated 2/8/18 for HOM Upgrade - removed 9 Dangerous Breed
                                        ShowHideAdditionalInfo(tblDesc, "hide")
                                        Exit Select
                                    Case 9
                                        'Updated 2/8/18 for HOM Upgrade - removed 9 Dangerous Breed
                                        If (QuoteObj.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso QuoteObj.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                            ShowHideAdditionalInfo(tblDesc, "show")
                                        Else
                                            ShowHideAdditionalInfo(tblDesc, "hide")
                                        End If
                                        Exit Select
                                    Case Else
                                        ShowHideAdditionalInfo(tblDesc, "show")
                                        Exit Select
                                End Select
                            Else
                                ShowHideAdditionalInfo(tblDesc, "hide")
                            End If
                        End If
                End Select
            Next
            Exit Sub
        Catch ex As Exception
            HandleError("Page LOAD", ex)
            RaiseEvent PersonalAutoUWQuestionsLoadFailed(Me, "Page Load")
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' SUBMIT Button
    ''' This is the only postback on this control
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        Try
            If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/28/2019; original logic in ELSE
                If ReadOnlyPolicyId <= 0 OrElse ReadOnlyPolicyImageNum <= 0 Then
                    Throw New Exception("Invalid PolicyId and/or PolicyImageNum!")
                End If
            ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                If EndorsementPolicyId <= 0 OrElse EndorsementPolicyImageNum <= 0 Then
                    Throw New Exception("Invalid PolicyId and/or PolicyImageNum!")
                End If

                ' !! Validation is done in the markup !!

                ' Save - NOTE THAT THE SAVE IS ACTUALLY DONE WHEN THE SUCCESS EVENT (below) IS RAISED
                If Not SaveAnswersToQuoteObject() Then
                    If IFM.Common.InputValidation.InputHelpers.StringHasAnyValue(lblMsg.Text) Then
                        ShowMessage("Unable to save quote: " & lblMsg.Text)
                    Else
                        ShowMessage("Save Failed!")
                    End If
                    RaiseEvent PersonalAutoUWQuestionsSaveFailed_Endorsements(Me, EndorsementPolicyId, EndorsementPolicyImageNum)
                Else
                    'ShowMessage("Saved")
                    ' Save is performed outside of this control when the following event is raised
                    Select Case QuoteObj.LobType
                        Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                            ' Matt's ctl_app_master_edit page listens for the following event
                            RaiseEvent PersonalAutoUWQuestionsSaved_Endorsements(Me, EndorsementPolicyId, EndorsementPolicyImageNum)
                            Exit Select
                        Case QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                            ' Matt's ctl_app_master_edit page listens for the following event
                            RaiseEvent SaveRequested(-1, ClassName)
                            Exit Select
                    End Select
                    ' redirect to next page or whatever comes next
                End If
            Else
                If QuoteId Is Nothing OrElse QuoteId = "" Then
                    Throw New Exception("Quote ID is not set!")
                Else
                    If Not IsNumeric(QuoteId) Then Throw New Exception("Invalid Quote ID: " & QuoteId)
                End If

                ' !! Validation is done in the markup !!

                ' Save - NOTE THAT THE SAVE IS ACTUALLY DONE WHEN THE SUCCESS EVENT (below) IS RAISED
                If Not SaveAnswersToQuoteObject() Then
                    If IFM.Common.InputValidation.InputHelpers.StringHasAnyValue(lblMsg.Text) Then
                        ShowMessage("Unable to save quote: " & lblMsg.Text)
                    Else
                        ShowMessage("Save Failed!")
                    End If
                    RaiseEvent PersonalAutoUWQuestionsSaveFailed(Me, QuoteId)
                Else
                    'ShowMessage("Saved")
                    ' Save is performed outside of this control when the following event is raised
                    Select Case QuoteObj.LobType
                        Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                            ' Matt's ctl_app_master_edit page listens for the following event
                            RaiseEvent PersonalAutoUWQuestionsSaved(Me, QuoteId)
                            Exit Select
                        Case QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                            ' Matt's ctl_app_master_edit page listens for the following event
                            RaiseEvent SaveRequested(-1, ClassName)
                            Exit Select
                    End Select
                    ' redirect to next page or whatever comes next
                End If
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("btnSubmit_Click", ex)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Repeater ItemDatabound
    ''' Set up and populate each repeater item and it's data
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub rptUWQ_ItemDatabound(sender As Object, e As RepeaterItemEventArgs) Handles rptUWQ.ItemDataBound
        Dim rbYes As RadioButton = e.Item.FindControl("rbYes")
        Dim rbNo As RadioButton = e.Item.FindControl("rbNo")
        Dim lbl As Label = e.Item.FindControl("lblUWQDescriptionTitle")
        Dim lblQuestion = e.Item.FindControl("lblQuestionText")
        Dim DescTextBox As TextBox = e.Item.FindControl("txtUWQDescription")
        Dim DescTable As HtmlTable = e.Item.FindControl("tblUWQDesc")
        Dim QTable As HtmlTable = e.Item.FindControl("tblUWQ")
        Dim MyQ As QuickQuotePolicyUnderwriting = Nothing
        Dim myLOB As String = ""
        Dim qn As Integer = -1
        Dim HideRentersQuestions As Boolean = False
        Dim odd As Boolean = False
        Dim QuestionIndex As Integer = 0
        Dim HOM6 As IFM.VR.Common.UWQuestions.VRUWQuestion = Nothing
        Dim HOM9 As IFM.VR.Common.UWQuestions.VRUWQuestion = Nothing
        Dim HOM14 As IFM.VR.Common.UWQuestions.VRUWQuestion = Nothing
        Dim HOM21 As IFM.VR.Common.UWQuestions.VRUWQuestion = Nothing
        Dim PPA16 As IFM.VR.Common.UWQuestions.VRUWQuestion = Nothing
        Dim DFR5 As IFM.VR.Common.UWQuestions.VRUWQuestion = Nothing
        Dim HOMQs As List(Of IFM.VR.Common.UWQuestions.VRUWQuestion) = IFM.VR.Common.UWQuestions.UWQuestions.GetPersonalHomeUnderwritingQuestions(QuoteObj.EffectiveDate, QuoteObj.LobId)
        Dim PPAQs As List(Of IFM.VR.Common.UWQuestions.VRUWQuestion) = IFM.VR.Common.UWQuestions.UWQuestions.GetPersonalAutoUnderwritingQuestions()
        'Updated 7/27/2022 for task 75803 MLW
        'Dim DFRQs As List(Of IFM.VR.Common.UWQuestions.VRUWQuestion) = IFM.VR.Common.UWQuestions.UWQuestions.GetDwellingFireUnderwritingQuestions()
        Dim DFRQs As List(Of IFM.VR.Common.UWQuestions.VRUWQuestion) = IFM.VR.Common.UWQuestions.UWQuestions.GetDwellingFireUnderwritingQuestions(QuoteObj.EffectiveDate)
        'Added 2/9/18 for HOM Upgrade MLW
        Dim HomeVersion As String = GetHomeVersion(QuoteObj)

        'added 8/9/2018 for multi-state; note: this Sub should only be used for Populate purposes, so using the 1st stateQuote found should suffice
        Dim qqHelper As New QuickQuoteHelperClass
        Dim multiStateQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject) = Nothing
        Dim quoteToUse As QuickQuote.CommonObjects.QuickQuoteObject = Nothing
        If QuoteObj IsNot Nothing Then
            multiStateQuotes = qqHelper.MultiStateQuickQuoteObjects(QuoteObj) 'should always return at least QuoteObj in the list
            If multiStateQuotes IsNot Nothing AndAlso multiStateQuotes.Count > 0 Then
                quoteToUse = multiStateQuotes.Item(0) 'will just use the 1st stateQuote found (or QuoteObj) since all questions are currently the same for all states
            End If
        End If

        Try
            HOM6 = HOMQs(5)
            HOM9 = HOMQs(8)
            HOM14 = HOMQs(13)
            HOM21 = HOMQs(20)
            PPA16 = PPAQs(15)
            DFR5 = DFRQs(4)

            Select Case QuoteObj.LobType
                Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                    myLOB = "PPA"
                    Exit Select
                Case QuickQuoteObject.QuickQuoteLobType.HomePersonal
                    myLOB = "HOM"
                    Exit Select
                Case QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                    myLOB = "DFR"
                    Exit Select
                Case Else
                    Exit Select
            End Select

            Dim d As Decimal = e.Item.ItemIndex / 2
            Dim i As Integer = e.Item.ItemIndex / 2

            If d <> i Then odd = True
            If e.Item.ItemIndex = 0 Then odd = False

            If odd Then
                QTable.BgColor = "LightGray"
                DescTable.BgColor = "LightGray"
            Else
                QTable.BgColor = ""
                DescTable.BgColor = ""
            End If

            ' Bind the radio buttons to the ShowHideDescription logic
            'rbYes.Attributes.Add("onclick", "ShowHideDescriptionPanel(" & DescTable.ClientID & ",'show');") 'actually sends control instead of just id
            'rbNo.Attributes.Add("onclick", "ShowHideDescriptionPanel(" & DescTable.ClientID & ",'hide');") 'actually sends control instead of just id

            'rbYes.Attributes.Add("onclick", "HandleRadioButtonClicks('" & myLOB & "', " & e.Item.ItemIndex & ");")
            'rbNo.Attributes.Add("onclick", "HandleRadioButtonClicks('" & myLOB & "', " & e.Item.ItemIndex & ");")

            ' If the quote is nothing we have a big problem
            If Not CheckQuoteObject("rptUWQ_ItemDatabound") Then Exit Sub

            If QuoteObj.Locations IsNot Nothing AndAlso QuoteObj.Locations.Count > 0 Then
                'updated 11/28/17 for HOM Upgrade MLW - added 22, 23, 24
                If QuoteObj.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal _
                    AndAlso QuoteObj.Locations(0).FormTypeId = "1" _
                    OrElse QuoteObj.Locations(0).FormTypeId = "2" _
                    OrElse QuoteObj.Locations(0).FormTypeId = "3" _
                    OrElse QuoteObj.Locations(0).FormTypeId = "6" _
                    OrElse QuoteObj.Locations(0).FormTypeId = "22" _
                    OrElse QuoteObj.Locations(0).FormTypeId = "23" _
                    OrElse QuoteObj.Locations(0).FormTypeId = "24" Then
                    HideRentersQuestions = True
                End If
            End If

            ' Update the label
            If QuoteObj.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HideRentersQuestions Then
                If e.Item.ItemIndex <= 13 Then
                    qn = e.Item.ItemIndex + 1
                Else
                    qn = e.Item.ItemIndex + 4
                End If
            Else
                qn = e.Item.ItemIndex + 1
            End If

            If myLOB = "HOM" OrElse myLOB = "DFR" Then
                lbl.Text = "Additional Information"
            Else
                lbl.Text = "Question " & qn.ToString() & " Additional Information"
            End If

            ' If there are no policyunderwriting questions then none of the checkboxes should be checked.
            Select Case myLOB
                Case "PPA"
                    ' Note that there always should be answers for #10, 12, 14 1nd 15 - the kill questions
                    'If QuoteObj.PolicyUnderwritings Is Nothing OrElse QuoteObj.PolicyUnderwritings.Count <= 0 Then
                    'updated 8/9/2018 for multi-state
                    If quoteToUse Is Nothing OrElse quoteToUse.PolicyUnderwritings Is Nothing OrElse quoteToUse.PolicyUnderwritings.Count <= 0 Then
                        ShowMessage("No Policy Underwritings Found!")
                        rbYes.Checked = False
                        rbNo.Checked = False
                        Exit Sub
                    End If
                    Exit Select
                Case "HOM", "DFR"
                    '8/9/2018 note: this should be okay for multi-state since Locations will be moved up to the top level and the 1st one should have UW Questions
                    If QuoteObj.Locations(0).PolicyUnderwritings Is Nothing OrElse QuoteObj.Locations(0).PolicyUnderwritings.Count <= 0 Then
                        ShowMessage("No Policy Underwritings Found!")
                        rbYes.Checked = False
                        rbNo.Checked = False
                        Exit Sub
                    End If
                    Exit Select
                Case Else
                    Throw New Exception("LOB Not Supported yet!")
            End Select

            ' Find the UWQuestion and answer in the quote
            If QuoteObj.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HideRentersQuestions Then
                If e.Item.ItemIndex <= 13 Then
                    MyQ = GetQuestion(e.Item.ItemIndex + 1)
                Else
                    MyQ = GetQuestion(e.Item.ItemIndex + 4)
                End If
            Else
                MyQ = GetQuestion(e.Item.ItemIndex + 1)
            End If
            rbNo.Checked = False
            rbYes.Checked = False

            ShowHideAdditionalInfo(DescTable, "hide")

            ' Populate the current question (THIS IS WHERE WE SET THE ANSWERS ON THE FORM)
            If MyQ IsNot Nothing Then
                ' #16 on AUTO is a special case, it does not require any additional info even on YES
                ' So there will never be an additional information textbox
                If myLOB = "PPA" And MyQ.PolicyUnderwritingCodeId = PPA16.PolicyUnderwritingCodeId Then
                    ' 9298 = "16. Has agent inspected vehicle?"
                    Select Case MyQ.PolicyUnderwritingAnswer
                        Case "1"
                            rbYes.Checked = True
                            Exit Select
                        Case "-1"
                            rbNo.Checked = True
                            Exit Select
                    End Select
                    ShowHideAdditionalInfo(DescTable, "hide")
                ElseIf myLOB = "HOM" And (MyQ.PolicyUnderwritingCodeId = HOM6.PolicyUnderwritingCodeId OrElse MyQ.PolicyUnderwritingCodeId = HOM9.PolicyUnderwritingCodeId OrElse MyQ.PolicyUnderwritingCodeId = HOM14.PolicyUnderwritingCodeId OrElse MyQ.PolicyUnderwritingCodeId = HOM21.PolicyUnderwritingCodeId) Then
                    ' HOM questions #6, #9, and #14 also do not require any additional information
                    'Updated 2/9/18 for HOM Upgrade MLW
                    If (QuoteObj.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso QuoteObj.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                        '#9 now has additional info in HOM Upgrade
                        If MyQ.PolicyUnderwritingCodeId = HOM9.PolicyUnderwritingCodeId Then
                            Select Case MyQ.PolicyUnderwritingAnswer
                                Case "1"  ' 1 = Yes
                                    rbYes.Checked = True
                                    ShowHideAdditionalInfo(DescTable, "show")
                                    If IFM.Common.InputValidation.InputHelpers.StringHasAnyValue(MyQ.PolicyUnderwritingExtraAnswer) Then
                                        DescTextBox.Text = MyQ.PolicyUnderwritingExtraAnswer
                                    Else
                                        DescTextBox.Text = ""
                                    End If
                                    Exit Select
                                Case "-1" ' -1 = No
                                    rbNo.Checked = True
                                    Exit Select
                            End Select
                        Else
                            Select Case MyQ.PolicyUnderwritingAnswer
                                Case "1"
                                    rbYes.Checked = True
                                    Exit Select
                                Case "-1"
                                    rbNo.Checked = True
                                    Exit Select
                            End Select
                            ShowHideAdditionalInfo(DescTable, "hide")
                        End If
                    Else
                        Select Case MyQ.PolicyUnderwritingAnswer
                            Case "1"
                                rbYes.Checked = True
                                Exit Select
                            Case "-1"
                                rbNo.Checked = True
                                Exit Select
                        End Select
                        ShowHideAdditionalInfo(DescTable, "hide")
                    End If

                ElseIf myLOB = "DFR" And (MyQ.PolicyUnderwritingCodeId = DFR5.PolicyUnderwritingCodeId) Then
                    ' DFR Question 5 has addl info on yes or no MGB 12/10/15 Bug 5283
                    Select Case MyQ.PolicyUnderwritingAnswer
                        Case "1"
                            rbYes.Checked = True
                            Exit Select
                        Case "-1"
                            rbNo.Checked = True
                            Exit Select
                    End Select
                    'Updated 7/26/2022 for task 75803 MLW
                    If IFM.VR.Common.Helpers.DFR.DFRStandaloneHelper.isDFRStandaloneAvailable(QuoteObj) Then
                        If rbYes.Checked Then
                            ShowHideAdditionalInfo(DescTable, "show")
                        Else
                            ShowHideAdditionalInfo(DescTable, "hide")
                        End If
                    Else
                        ShowHideAdditionalInfo(DescTable, "show")
                    End If
                    'ShowHideAdditionalInfo(DescTable, "show")
                    If IFM.Common.InputValidation.InputHelpers.StringHasAnyValue(MyQ.PolicyUnderwritingExtraAnswer) Then
                        DescTextBox.Text = MyQ.PolicyUnderwritingExtraAnswer
                    Else
                        DescTextBox.Text = ""
                    End If
                Else
                    ' ALL OTHER QUESTIONS (besides #16 PPA) ARE HANDLED WITH THIS CODE
                    Select Case MyQ.PolicyUnderwritingAnswer
                        Case "1"  ' 1 = Yes
                            rbYes.Checked = True
                            ShowHideAdditionalInfo(DescTable, "show")
                            If IFM.Common.InputValidation.InputHelpers.StringHasAnyValue(MyQ.PolicyUnderwritingExtraAnswer) Then
                                DescTextBox.Text = MyQ.PolicyUnderwritingExtraAnswer
                            Else
                                DescTextBox.Text = ""
                            End If
                            Exit Select
                        Case "-1" ' -1 = No
                            rbNo.Checked = True
                            Exit Select
                    End Select
                End If
            End If

            ' Bind the Yes/No radio buttons to the appropriate handler for the current LOB
            Select Case myLOB
                Case "PPA"
                    rbYes.Attributes.Add("onclick", "HandleRadioButtonClicksPPA(" & e.Item.ItemIndex & ");")
                    rbNo.Attributes.Add("onclick", "HandleRadioButtonClicksPPA(" & e.Item.ItemIndex & ");")
                    Exit Select
                Case "HOM"
                    rbYes.Attributes.Add("onclick", "HandleRadioButtonClicksHOM(" & e.Item.ItemIndex & ", '" + HideRentersQuestions.ToString.ToUpper() & "');")
                    rbNo.Attributes.Add("onclick", "HandleRadioButtonClicksHOM(" & e.Item.ItemIndex & ", '" + HideRentersQuestions.ToString.ToUpper() & "');")
                    Exit Select
                Case "DFR"
                    'Updated 7/26/2022 for task 75803 MLW
                    Dim DFRStandaloneAvailable = IFM.VR.Common.Helpers.DFR.DFRStandaloneHelper.isDFRStandaloneAvailable(QuoteObj)
                    rbYes.Attributes.Add("onclick", "HandleRadioButtonClicksDFR(" & e.Item.ItemIndex & ", '" + HideRentersQuestions.ToString.ToUpper() & "', '" & DFRStandaloneAvailable.ToString.ToUpper() & "');")
                    rbNo.Attributes.Add("onclick", "HandleRadioButtonClicksDFR(" & e.Item.ItemIndex & ", '" + HideRentersQuestions.ToString.ToUpper() & "', '" & DFRStandaloneAvailable.ToString.ToUpper() & "');")
                    'rbYes.Attributes.Add("onclick", "HandleRadioButtonClicksDFR(" & e.Item.ItemIndex & ", '" + HideRentersQuestions.ToString.ToUpper() & "');")
                    'rbNo.Attributes.Add("onclick", "HandleRadioButtonClicksDFR(" & e.Item.ItemIndex & ", '" + HideRentersQuestions.ToString.ToUpper() & "');")
                Case Else
                    Throw New Exception("Invalid value")
            End Select

            'rbYes.Attributes.Add("onclick", "HandleRadioButtonClicks('" & myLOB & "', " & e.Item.ItemIndex & ", '" & HideRentersQuestions.ToString().ToUpper() & "');")
            'rbNo.Attributes.Add("onclick", "HandleRadioButtonClicks('" & myLOB & "', " & e.Item.ItemIndex & ", '" & HideRentersQuestions.ToString().ToUpper() & "');")

            Exit Sub
        Catch ex As Exception
            HandleError("rptUWQ_ItemDatabound", ex)
            Exit Sub
        End Try
    End Sub

    Protected Sub btnGoToApp_Click(sender As Object, e As EventArgs) Handles btnGoToApp.Click
        Try
            If String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then 'added IF 2/28/2019; original logic in ELSE
                If ReadOnlyPolicyId <= 0 OrElse ReadOnlyPolicyImageNum <= 0 Then
                    Throw New Exception("Invalid PolicyId and/or PolicyImageNum!")
                End If

                Select Case QuoteObj.LobType
                    Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                        RaiseEvent RequestNavigationToApplication_ReadOnly(Me, ReadOnlyPolicyId, ReadOnlyPolicyImageNum)
                        Exit Select
                    Case QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                        RaiseEvent RequestNavigationToApplication_ReadOnly(Me, ReadOnlyPolicyId, ReadOnlyPolicyImageNum)
                        Exit Select
                End Select
            ElseIf String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                If EndorsementPolicyId <= 0 OrElse EndorsementPolicyImageNum <= 0 Then
                    Throw New Exception("Invalid PolicyId and/or PolicyImageNum!")
                End If

                ' !! Validation is done in the markup !!

                ' Save - NOTE THAT THE SAVE IS ACTUALLY DONE WHEN THE SUCCESS EVENT (below) IS RAISED
                If Not SaveAnswersToQuoteObject() Then
                    If IFM.Common.InputValidation.InputHelpers.StringHasAnyValue(lblMsg.Text) Then
                        ShowMessage("Unable to save quote: " & lblMsg.Text)
                    Else
                        ShowMessage("Save Failed!")
                    End If
                    RaiseEvent PersonalAutoUWQuestionsSaveFailed_Endorsements(Me, EndorsementPolicyId, EndorsementPolicyImageNum)
                Else
                    ' Save is performed outside of this control when the following event is raised
                    Select Case QuoteObj.LobType
                        Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                            ' Matt's auto page listens for the following evet

                            RaiseEvent PersonalAutoUWQuestionsSaved_Endorsements(Me, EndorsementPolicyId, EndorsementPolicyImageNum)
                            RaiseEvent RequestNavigationToApplication_Endorsements(Me, EndorsementPolicyId, EndorsementPolicyImageNum)
                            Exit Select
                        Case QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                            ' My home page listens for the following event
                            'RaiseEvent PersonalAutoUWQuestionsSaved(Me, QuoteId)
                            RaiseEvent SaveRequested(-1, ClassName)
                            RaiseEvent RequestNavigationToApplication_Endorsements(Me, EndorsementPolicyId, EndorsementPolicyImageNum)
                            Exit Select
                    End Select
                    ' redirect to next page or whatever comes next
                End If
            Else
                If QuoteId Is Nothing OrElse QuoteId = "" Then
                    Throw New Exception("Quote ID is not set!")
                Else
                    If Not IsNumeric(QuoteId) Then Throw New Exception("Invalid Quote ID: " & QuoteId)
                End If

                ' !! Validation is done in the markup !!

                ' Save - NOTE THAT THE SAVE IS ACTUALLY DONE WHEN THE SUCCESS EVENT (below) IS RAISED
                If Not SaveAnswersToQuoteObject() Then
                    If IFM.Common.InputValidation.InputHelpers.StringHasAnyValue(lblMsg.Text) Then
                        ShowMessage("Unable to save quote: " & lblMsg.Text)
                    Else
                        ShowMessage("Save Failed!")
                    End If
                    RaiseEvent PersonalAutoUWQuestionsSaveFailed(Me, QuoteId)
                Else
                    ' Save is performed outside of this control when the following event is raised
                    Select Case QuoteObj.LobType
                        Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                            ' Matt's auto page listens for the following evet

                            RaiseEvent PersonalAutoUWQuestionsSaved(Me, QuoteId)
                            RaiseEvent RequestNavigationToApplication(Me, QuoteId)
                            Exit Select
                        Case QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                            ' My home page listens for the following event
                            'RaiseEvent PersonalAutoUWQuestionsSaved(Me, QuoteId)
                            RaiseEvent SaveRequested(-1, ClassName)
                            RaiseEvent RequestNavigationToApplication(Me, QuoteId)
                            Exit Select
                    End Select
                    ' redirect to next page or whatever comes next
                End If
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("btnSubmit_Click", ex)
            Exit Sub
        End Try
    End Sub

#End Region

#Region "Old Code"
    ''' <summary>
    ''' Loads answers for PERSONAL AUTO
    ''' </summary>
    ''' <remarks></remarks>
    'Private Sub LoadPersonalAutoQuestionAnswers()
    '    Dim ritem As RepeaterItem = Nothing
    '    Dim rbN As RadioButton = Nothing
    '    Dim rbY As RadioButton = Nothing
    '    Dim pnlDesc As Panel = Nothing
    '    Dim txtDesc As TextBox = Nothing
    '    Dim tblDesc As HtmlTable = Nothing

    '    Try
    '        ' All of the kill questions should be populated if they have values
    '        ' Note Answer 1 = YES; Answer -1 = NO
    '        For Each uw As QuickQuotePolicyUnderwriting In QuoteObj.PolicyUnderwritings
    '            Select Case uw.PolicyUnderwritingCodeId
    '                Case "9283" ' #1
    '                    ritem = GetPolicyUnderwritingQuestionItem(1)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    pnlDesc = ritem.FindControl("pnlUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9284" ' #2
    '                    ritem = GetPolicyUnderwritingQuestionItem(2)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    pnlDesc = ritem.FindControl("pnlUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9285" ' #3
    '                    ritem = GetPolicyUnderwritingQuestionItem(3)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    pnlDesc = ritem.FindControl("pnlUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9286" ' #4
    '                    ritem = GetPolicyUnderwritingQuestionItem(4)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    pnlDesc = ritem.FindControl("pnlUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9287" ' #5
    '                    ritem = GetPolicyUnderwritingQuestionItem(5)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    pnlDesc = ritem.FindControl("pnlUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9288" ' #6
    '                    ritem = GetPolicyUnderwritingQuestionItem(6)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    pnlDesc = ritem.FindControl("pnlUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9289" ' #7
    '                    ritem = GetPolicyUnderwritingQuestionItem(7)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    pnlDesc = ritem.FindControl("pnlUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9290" ' #8
    '                    ritem = GetPolicyUnderwritingQuestionItem(8)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    pnlDesc = ritem.FindControl("pnlUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9291" ' #9
    '                    ritem = GetPolicyUnderwritingQuestionItem(9)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    pnlDesc = ritem.FindControl("pnlUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9292" ' #10 Any driver’s license been suspended/revoked?
    '                    ritem = GetPolicyUnderwritingQuestionItem(10)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    pnlDesc = ritem.FindControl("pnlUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9293" ' #11
    '                    ritem = GetPolicyUnderwritingQuestionItem(11)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    pnlDesc = ritem.FindControl("pnlUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9294" ' #12
    '                    ritem = GetPolicyUnderwritingQuestionItem(12)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    pnlDesc = ritem.FindControl("pnlUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9295" ' #13
    '                    ritem = GetPolicyUnderwritingQuestionItem(13)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    pnlDesc = ritem.FindControl("pnlUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9296" ' #14
    '                    ritem = GetPolicyUnderwritingQuestionItem(14)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    pnlDesc = ritem.FindControl("pnlUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9297" ' #15
    '                    ritem = GetPolicyUnderwritingQuestionItem(15)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    pnlDesc = ritem.FindControl("pnlUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9298" ' #16
    '                    ritem = GetPolicyUnderwritingQuestionItem(16)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    pnlDesc = ritem.FindControl("pnlUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '            End Select
    '        Next

    '        Exit Sub
    '    Catch ex As Exception
    '        HandleError("LoadPersonalAutoQuestionAnswers", ex)
    '        Exit Sub
    '    End Try
    'End Sub

    ' ''' <summary>
    ' ''' Loads answers for PERSONAL HOME
    ' ''' </summary>
    ' ''' <remarks></remarks>
    'Private Sub LoadPersonalHomeUWQuestionAnswers()
    '    Dim ritem As RepeaterItem = Nothing
    '    Dim rbN As RadioButton = Nothing
    '    Dim rbY As RadioButton = Nothing
    '    Dim pnlDesc As Panel = Nothing
    '    Dim txtDesc As TextBox = Nothing
    '    Dim tblDesc As HtmlTable = Nothing

    '    Try
    '        ' All of the kill questions should be populated if they have values
    '        ' Note Answer 1 = YES; Answer -1 = NO
    '        For Each uw As QuickQuotePolicyUnderwriting In QuoteObj.Locations(0).PolicyUnderwritings
    '            Select Case uw.PolicyUnderwritingCodeId
    '                Case "9324" ' #1
    '                    ritem = GetPolicyUnderwritingQuestionItem(1)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    tblDesc = ritem.FindControl("tblUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9299" ' #2
    '                    ritem = GetPolicyUnderwritingQuestionItem(2)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    tblDesc = ritem.FindControl("tblUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9300" ' #3
    '                    ritem = GetPolicyUnderwritingQuestionItem(3)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    tblDesc = ritem.FindControl("tblUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9301" ' #4
    '                    ritem = GetPolicyUnderwritingQuestionItem(4)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    tblDesc = ritem.FindControl("tblUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9302" ' #5
    '                    ritem = GetPolicyUnderwritingQuestionItem(5)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    tblDesc = ritem.FindControl("tblUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9303" ' #6
    '                    ritem = GetPolicyUnderwritingQuestionItem(6)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    tblDesc = ritem.FindControl("tblUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9304" ' #7
    '                    ritem = GetPolicyUnderwritingQuestionItem(7)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    tblDesc = ritem.FindControl("tblUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9305" ' #8
    '                    ritem = GetPolicyUnderwritingQuestionItem(8)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    tblDesc = ritem.FindControl("tblUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9306" ' #9
    '                    ritem = GetPolicyUnderwritingQuestionItem(9)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    tblDesc = ritem.FindControl("tblUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9307" ' #10 Any driver’s license been suspended/revoked?
    '                    ritem = GetPolicyUnderwritingQuestionItem(10)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    tblDesc = ritem.FindControl("tblUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9308" ' #11
    '                    ritem = GetPolicyUnderwritingQuestionItem(11)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    tblDesc = ritem.FindControl("tblUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9309" ' #12
    '                    ritem = GetPolicyUnderwritingQuestionItem(12)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    tblDesc = ritem.FindControl("tblUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9310" ' #13
    '                    ritem = GetPolicyUnderwritingQuestionItem(13)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    tblDesc = ritem.FindControl("tblUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9311" ' #14
    '                    ritem = GetPolicyUnderwritingQuestionItem(14)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    tblDesc = ritem.FindControl("tblUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9312" ' #15
    '                    ritem = GetPolicyUnderwritingQuestionItem(15)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    tblDesc = ritem.FindControl("tblUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9313" ' #16
    '                    ritem = GetPolicyUnderwritingQuestionItem(16)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    tblDesc = ritem.FindControl("tblUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9314" ' #17
    '                    ritem = GetPolicyUnderwritingQuestionItem(17)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    tblDesc = ritem.FindControl("tblUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9315" ' #18
    '                    ritem = GetPolicyUnderwritingQuestionItem(18)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    tblDesc = ritem.FindControl("tblUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9316" ' #19
    '                    ritem = GetPolicyUnderwritingQuestionItem(19)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    tblDesc = ritem.FindControl("tblUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9317" ' #20
    '                    ritem = GetPolicyUnderwritingQuestionItem(20)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    tblDesc = ritem.FindControl("tblUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9318" ' #21
    '                    ritem = GetPolicyUnderwritingQuestionItem(21)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    tblDesc = ritem.FindControl("tblUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9319" ' #22
    '                    ritem = GetPolicyUnderwritingQuestionItem(22)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    tblDesc = ritem.FindControl("tblUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9320" ' #23
    '                    ritem = GetPolicyUnderwritingQuestionItem(23)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    tblDesc = ritem.FindControl("tblUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9321" ' #24
    '                    ritem = GetPolicyUnderwritingQuestionItem(24)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    tblDesc = ritem.FindControl("tblUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9322" ' #25
    '                    ritem = GetPolicyUnderwritingQuestionItem(25)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    tblDesc = ritem.FindControl("tblUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '                Case "9323" ' #26
    '                    ritem = GetPolicyUnderwritingQuestionItem(26)
    '                    rbN = ritem.FindControl("rbNo")
    '                    rbY = ritem.FindControl("rbYes")
    '                    rbY.Checked = False
    '                    rbN.Checked = False
    '                    tblDesc = ritem.FindControl("tblUWQDesc")
    '                    ShowHideAdditionalInfo(tblDesc, "hide")
    '                    txtDesc = ritem.FindControl("txtUWQDescription")
    '                    txtDesc.Text = ""
    '                    Select Case uw.PolicyUnderwritingAnswer
    '                        Case "1"
    '                            rbY.Checked = True
    '                            ' Add'l Info
    '                            If uw.PolicyUnderwritingExtraAnswer IsNot Nothing AndAlso uw.PolicyUnderwritingExtraAnswer <> "" Then
    '                                txtDesc.Text = uw.PolicyUnderwritingExtraAnswer
    '                            End If
    '                            ShowHideAdditionalInfo(tblDesc, "show")
    '                            Exit Select
    '                        Case "-1"
    '                            rbN.Checked = True
    '                            Exit Select
    '                    End Select
    '            End Select
    '        Next

    '        Exit Sub
    '    Catch ex As Exception
    '        HandleError("LoadPersonalHomeUWQuestionAnswers", ex)
    '    End Try
    'End Sub

    ' ''' <summary>
    ' ''' Directs the loading of answers to the appropriate LOB load function
    ' ''' </summary>
    ' ''' <remarks></remarks>
    'Public Sub LoadUWQuestionAnswers()
    '    Try
    '        ' At this point the quote object MUST be set.  If it's not we're going to throw an error.
    '        If Not CheckQuoteObject("LoadUWQuestionAnswers") Then
    '            RaiseEvent PersonalAutoUWQuestionsLoadFailed(Me, "The Quote Object has not been set")
    '            Exit Sub
    '        End If

    '        ' Load the questions based on the LOB of the quote
    '        Select Case QuoteObj.LobType
    '            Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
    '                LoadPersonalAutoQuestionAnswers()
    '                Exit Select
    '            Case QuickQuoteObject.QuickQuoteLobType.HomePersonal
    '                LoadPersonalHomeUWQuestionAnswers()
    '                Exit Select
    '        End Select

    '        Exit Sub
    '    Catch ex As Exception
    '        HandleError("LoadUWQuestionAnswers", ex)
    '        RaiseEvent PersonalAutoUWQuestionsLoadFailed(Me, QuoteId)
    '    End Try
    'End Sub

#End Region
End Class