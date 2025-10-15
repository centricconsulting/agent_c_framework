Imports QuickQuote.CommonObjects

Imports IFM.PrimativeExtensions
Imports IFM.ControlFlags
Imports IFM.VR.Flags
Imports IFM.VR.Common.Underwriting
Imports AnswerList = System.Collections.Generic.Dictionary(Of IFM.VR.Common.UWQuestions.VRUWQuestion, (AnsweredYes As Boolean, ExtraAnswerText As String))
Imports Answer = System.Collections.Generic.KeyValuePair(Of IFM.VR.Common.UWQuestions.VRUWQuestion, (AnsweredYes As Boolean, ExtraAnswerText As String))
Imports IFM.VR.Common.UWQuestions
Imports IFM.VR.Web.Factory

Public Class ctlUWQuestions_PPA
    Inherits VRControlBase

    Private ReadOnly _flags As IEnumerable(Of IFeatureFlag)
    Private ReadOnly _uwQuestionSvc As IUnderwritingQuestionsService
    Private _tabIndex As Integer
    Protected Property Questions As List(Of VRUWQuestion)
        Get
            Return ViewState(NameOf(Questions))
        End Get
        Set(value As List(Of VRUWQuestion))
            ViewState(NameOf(Questions)) = value
        End Set
    End Property


    Public Sub New()
        _flags = New List(Of IFeatureFlag) From {New LOB.PPA}
        _uwQuestionSvc = UnderwritingQuestionServiceFactory.BuildFor(QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
    End Sub
    ''' <summary>
    ''' constructor supporting dependency injection
    ''' </summary>
    ''' <param name="flags"></param>
    ''' <param name="uwQuestionSvc"></param>
    <Microsoft.Extensions.DependencyInjection.ActivatorUtilitiesConstructor()>
    Public Sub New(flags As IEnumerable(Of IFeatureFlag),
                   uwQuestionSvc As IUnderwritingQuestionsService)
        _flags = flags
        _uwQuestionSvc = uwQuestionSvc
    End Sub

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

#Region "UI Support"
    ''' <summary>
    ''' Supports tab order
    ''' </summary>
    ''' <returns></returns>
    Public Function GetTabIndex() As Integer
        _tabIndex += 1
        Return _tabIndex
    End Function

    ''' <summary>
    ''' Standard error handler.  Displays the error in the lblMsg label.
    ''' </summary>
    ''' <param name="RoutineName"></param>
    ''' <param name="exc"></param>
    ''' <remarks></remarks>
    Private Sub HandleError(ByVal RoutineName As String, ByRef exc As Exception, Optional ByVal message As String = "")
        Dim msgSeparator = IIf(String.IsNullOrWhiteSpace(message), ": ", " ")
        ShowMessage($"{message}{msgSeparator}{RoutineName}:{exc.Message}")
    End Sub
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
    ''' Loads questions and responses and binds them to the repeater
    ''' </summary>
    Private Sub InitializeQuestionData()

        Dim request As New UnderwritingQuestionRequest() With {
                            .LobType = Quote.LobType,
                            .GoverningState = Quote.QuickQuoteState,
                            .Quote = Quote,
                            .TypeFilter = UnderwritingQuestionRequest.QuestionTypeFilter.GoverningStateOnly Or UnderwritingQuestionRequest.QuestionTypeFilter.ExcludeUnmapped
                            }

        Try
            Dim result = _uwQuestionSvc.GetQuestions(request)

            If result IsNot Nothing AndAlso result.Any Then
                Questions = result
                rptUWQ.DataSource = Questions
                rptUWQ.DataBind()
            Else
                ShowMessage("Unable to load underwriting questions")
            End If
        Catch ex As Exception
            HandleError("InitializeQuestions", ex)
        End Try
    End Sub
#End Region

#Region "Overrides"
    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        InitializeQuestionData()
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack And Quote IsNot Nothing Then
            InitializeQuestionData()
        End If
    End Sub

    Public Overrides Function Save() As Boolean

    End Function
#End Region

#Region "Guards"

    ''' <summary>
    ''' Minimum verification prior to navigation of readonly quote
    ''' </summary>
    ''' <returns>true if verification passes otherwise an exception is thrown</returns>
    Protected Function ReadOnly_MinimumVerificationPassesOrThrowException() As Boolean
        If ReadOnlyPolicyId <= 0 OrElse ReadOnlyPolicyImageNum <= 0 Then
            Throw New Exception("Invalid PolicyId and/or PolicyImageNum!")
        End If

        Return True
    End Function
    ''' <summary>
    ''' Minimum verification prior to saving the endorsement quote
    ''' </summary>
    ''' <returns>true if verification passes otherwise an exception is thrown</returns>
    Protected Function Endorsement_MinimumVerificationPassesOrThrowException() As Boolean
        If EndorsementPolicyId <= 0 OrElse EndorsementPolicyImageNum <= 0 Then
            Throw New Exception("Invalid PolicyId and/or PolicyImageNum!")
        End If

        Return True
    End Function
    ''' <summary>
    ''' Minimum verification prior to saving the quote
    ''' </summary>
    ''' <returns>true if verification passes otherwise an exception is thrown</returns>
    Protected Function Quote_MinimumVerificationPassesOrThrowException() As Boolean
        If String.IsNullOrWhiteSpace(QuoteId) Then
            Throw New Exception("Quote ID is not set!")
        ElseIf Not IsNumeric(QuoteId) Then
            Throw New Exception("Invalid Quote ID: " & QuoteId)
        End If

        Return True
    End Function

    ''' <summary>
    ''' Saves the answers to the Quote object
    ''' Actual saving Of quote To database will occur elsewhere When the **Saved events are handled
    ''' </summary>
    ''' <returns> whether or not the save was successful </returns>
    Protected Function Attempt_SaveAnswersToQuote() As Boolean
        Dim retval As Boolean

        Try
            Dim saveRequest As New UnderwritingSaveRequest With {
                       .LobType = Quote.LobType,
                       .GoverningState = Quote.QuickQuoteState,
                       .Quote = Quote,
                       .Answers = Questions
                   }

            retval = _uwQuestionSvc?.SaveAnswers(saveRequest)

            If retval = False Then ShowMessage("Save Failed!")
        Catch ex As Exception
            HandleError(NameOf(Attempt_SaveAnswersToQuote), ex, "Unable to save quote")
        End Try

        Return retval
    End Function


#End Region

#Region "Event Handlers"
    Private Const COMMAND_SAVE = "save"
    Private Const COMMAND_APP = "gotoApp"
    ''' <summary>
    ''' Handles click from Save or GotoApp buttons
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub HandleCommmand(sender As Object, e As CommandEventArgs) Handles btnSave.Command, btnGotoApp.Command

        Try
            'Pre-Save Sanity checks -- we could move these to a reusable object/method
            With Quote
                If IsQuoteReadOnly() Then
                    ReadOnly_MinimumVerificationPassesOrThrowException()
                ElseIf IsQuoteEndorsement() Then
                    Endorsement_MinimumVerificationPassesOrThrowException()
                Else
                    Quote_MinimumVerificationPassesOrThrowException()
                End If
                'if any of these failed, execution would have jumped to the catch block
            End With

            'nothing to do is save is requested, but if gotoApp is request, 
            'only signal navigation if save passes
            If IsQuoteReadOnly() Then
                If e.CommandName = COMMAND_APP Then
                    RaiseEvent RequestNavigationToApplication_ReadOnly(Me, ReadOnlyPolicyId, ReadOnlyPolicyImageNum)
                End If

            ElseIf Attempt_SaveAnswersToQuote() = True Then

                If IsQuoteEndorsement() Then
                    RaiseEvent PersonalAutoUWQuestionsSaved_Endorsements(Me, EndorsementPolicyId, EndorsementPolicyImageNum)
                    If e.CommandName = COMMAND_APP Then RaiseEvent RequestNavigationToApplication_Endorsements(Me, EndorsementPolicyId, EndorsementPolicyImageNum)
                Else 'new business
                    RaiseEvent PersonalAutoUWQuestionsSaved(Me, QuoteId)
                    If e.CommandName = COMMAND_APP Then RaiseEvent RequestNavigationToApplication(Me, QuoteId)
                End If

            ElseIf IsQuoteEndorsement() Then
                RaiseEvent PersonalAutoUWQuestionsSaveFailed_Endorsements(Me, EndorsementPolicyId, EndorsementPolicyImageNum)
            Else 'new business
                RaiseEvent PersonalAutoUWQuestionsSaveFailed(Me, QuoteId)
            End If

        Catch ex As Exception
            HandleError($"HandleCommand:{e.CommandName}", ex)
        End Try
    End Sub

    Protected Sub RepeaterRadioButton_CheckedChanged(sender As Object, e As EventArgs)
        Dim rbSelected As RadioButton = CType(sender, RadioButton)

        Dim questionNumber As Integer = rbSelected.Attributes.Item("data-question-number")
        Dim questionValue = rbSelected.Attributes.Item("data-question-option")

        Dim questionsLocal = Questions
        Dim changedQuestion = questionsLocal.FirstOrDefault(Function(q) q.QuestionNumber = questionNumber)

        If changedQuestion IsNot Nothing Then
            Dim txtDetail As TextBox = CType(rbSelected.Parent.FindControl("txtUWQDescription"), TextBox)

            If questionValue = "Yes" Then
                changedQuestion.QuestionAnswerYes = True
                changedQuestion.QuestionAnswerNo = False
                changedQuestion.DetailTextOnQuestionYes = txtDetail.Text
            Else
                txtDetail.Text = String.Empty
                changedQuestion.DetailTextOnQuestionYes = String.Empty
                changedQuestion.QuestionAnswerYes = False
                changedQuestion.QuestionAnswerNo = True
            End If
            Questions = questionsLocal
        End If
    End Sub

    Protected Sub RepeaterTextbox_TextChanged(sender As Object, e As EventArgs)
        Dim txtChanged As TextBox = CType(sender, TextBox)

        'we only need to back stop this when the question answer is Yes - so the text shouldn't be empty
        If String.IsNullOrWhiteSpace(txtChanged.Text) Then Return

        Dim questionNumber As Integer = txtChanged.Attributes.Item("data-question-number")

        Dim questionsLocal = Questions
        Dim changedQuestion = questionsLocal.FirstOrDefault(Function(q) q.QuestionNumber = questionNumber)

        If changedQuestion IsNot Nothing Then
            'this may be redundant with RepeaterRadioButton_CheckedChanged when the question is first set to yes
            changedQuestion.DetailTextOnQuestionYes = txtChanged.Text
            Questions = questionsLocal
        End If
    End Sub
    Protected Sub rptUWQ_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptUWQ.ItemDataBound

    End Sub

    Protected Sub rptUWQ_ItemCreated(sender As Object, e As RepeaterItemEventArgs) Handles rptUWQ.ItemCreated
        Dim rbYes As RadioButton = e.Item.FindControl("rbYes")
        Dim rbNo As RadioButton = e.Item.FindControl("rbNo")
        Dim txtExtra As TextBox = e.Item.FindControl("txtUWQDescription")

        AddHandler rbYes.CheckedChanged, AddressOf RepeaterRadioButton_CheckedChanged
        AddHandler rbNo.CheckedChanged, AddressOf RepeaterRadioButton_CheckedChanged
        AddHandler txtExtra.TextChanged, AddressOf RepeaterTextbox_TextChanged
    End Sub
#End Region
End Class