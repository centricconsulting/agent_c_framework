Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods

Public Class ctlHomePersonalLossList
    Inherits System.Web.UI.UserControl
    Implements IVRUI_P

#Region "Declarations"
    Public Event HomeLostHistoryChanged()
    Public Event SaveRequested(ByVal Index As Integer, ByVal WhichControl As String)

    Private ClassName As String = "ctlHomePersonalLossList"
    Private _quote As QuickQuoteObject
    Protected ReadOnly Property Quote As QuickQuote.CommonObjects.QuickQuoteObject Implements IVRUI_P.Quote
        Get
            Dim errCreateQSO As String = ""
            If _quote Is Nothing Then
                _quote = VR.Common.QuoteSave.QuoteSaveHelpers.GetQuoteById(QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuoteId, errCreateQSO)
            End If
            Return _quote
        End Get
    End Property

    Protected ReadOnly Property QuoteId As String Implements IVRUI_P.QuoteId
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

    Public Property LossHistoryNumber As Int32
        Get
            If ViewState("vs_LossHistoryNum") Is Nothing Then
                ViewState("vs_LossHistoryNum") = -1
            End If
            Return CInt(ViewState("vs_LossHistoryNum"))
        End Get
        Set(value As Int32)
            ViewState("vs_LossHistoryNum") = value
        End Set
    End Property

    Public Property ValidationHelper As ControlValidationHelper Implements IVRUI_P.ValidationHelper
        Get
            If ViewState("vs_valHelp") Is Nothing Then
                ViewState("vs_valHelp") = New ControlValidationHelper
            End If
            Return DirectCast(ViewState("vs_valHelp"), ControlValidationHelper)
        End Get
        Set(value As ControlValidationHelper)
            ViewState("vs_valHelp") = value
        End Set
    End Property

    Public Property ActiveIMPane As String
        Get
            Return Me.hiddenActiveIM.Value
        End Get
        Set(value As String)
            Me.hiddenActiveIM.Value = value
        End Set
    End Property

#End Region

#Region "Methods and Functions"
    Private Sub HandleError(ByVal RoutineName As String, ByVal exc As Exception)
        ShowMessage(ClassName & ":" & RoutineName & ": " & exc.Message)
    End Sub

    Private Sub ClearErrorMessage()
        lblMsg.Text = "&nbsp;"
    End Sub

    Private Sub ShowMessage(ByVal msg As String)
        lblMsg.Text = msg
    End Sub

    ''' <summary>
    ''' Loops through the repeaters, saves each Loss History Item
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Save() As Boolean Implements IVRUI_P.Save
        Try
            Dim index As Integer = 0
            For Each cntrl As RepeaterItem In rptLossHistories.Items
                Dim LHControl As ctlHomePersonalLossItem = cntrl.FindControl("ctlLossHistoryItem")
                LHControl.Save()
                LHControl.Populate()
                index += 1
            Next
            RaiseEvent HomeLostHistoryChanged()
            Return True
        Catch ex As Exception
            HandleError("Save", ex)
            Return False
        End Try
        Return True
    End Function

    ''' <summary>
    ''' Not used
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub LoadStaticData() Implements IVRUI_P.LoadStaticData
        Exit Sub
    End Sub

    ''' <summary>
    ''' Binds the repeater to it's data
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Populate() Implements IVRUI_P.Populate
        Try
            rptLossHistories.DataSource = Nothing
            rptLossHistories.DataBind()

            'Bind the LossHistories collection directly to the repeater
            If Quote IsNot Nothing AndAlso Quote.LossHistoryRecords IsNot Nothing _
                AndAlso Quote.LossHistoryRecords.Count > 0 Then
                rptLossHistories.DataSource = Quote.LossHistoryRecords
                rptLossHistories.DataBind()
                btnSubmit.Visible = Me.Quote.LossHistoryRecords.Any
            Else
                btnSubmit.Visible = False
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("Populate", ex)
            Exit Sub
        End Try
    End Sub

    Public Sub ValidateForm() Implements IVRUI_P.ValidateForm
        Try
            ValidationHelper.Clear()
            Dim _script = DirectCast(Page.Master, VelociRater).StartUpScriptManager

            For Each cntrl As RepeaterItem In rptLossHistories.Items
                Dim LHControl As ctlHomePersonalLossItem = cntrl.FindControl("ctlLossHistoryItem")
                LHControl.ValidateForm()
            Next

            Dim valSum As ctlValidationSummary = DirectCast(Page.Master, VelociRater).ValidationSummary
            valSum.InsertValidationControl(Me.ValidationHelper)

            Exit Sub
        Catch ex As Exception
            HandleError("ValidateForm", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub ItemSaveDetected()
        RaiseEvent SaveRequested(-1, ClassName)
    End Sub

    Public Sub AttachLHControlEvents()
        Dim index As Integer = 0
        Try
            ' need to wire up the remove events for each control every single time
            For Each cntrl As RepeaterItem In rptLossHistories.Items
                Dim LHControl As ctlHomePersonalLossItem = cntrl.FindControl("ctlLossHistoryItem")
                AddHandler LHControl.ItemRemoveRequest, AddressOf ItemRemoveRequest
                AddHandler LHControl.SaveRequested, AddressOf ItemSaveDetected
                index += 1
            Next

            Exit Sub
        Catch ex As Exception
            HandleError("AttachLHControlEvents", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub ItemRemoveRequest()
        Dim err As String = ""
        Try
            RaiseEvent SaveRequested(-1, ClassName)
            RaiseEvent HomeLostHistoryChanged()
            Populate()
            Dim _script = DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager
            _script.AddScriptLine("ToggleEditMode(true,true);")

            Exit Sub
        Catch ex As Exception
            HandleError("ItemRemoveRequest", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub LHControlNewIMRequested()
        btnAddLossHistory_Click(Me, New EventArgs())
    End Sub

    Private Sub LHControlRemoving(index As Integer)
        Dim activePan As Int32 = 0
        If Int32.TryParse(Me.hiddenActiveIM.Value, activePan) Then
            If activePan >= index Then
                Me.hiddenActiveIM.Value = (activePan - 1).ToString()
            End If
        End If
    End Sub

    Private Sub LHControlRequestedSave(invokeValidations As Boolean)
        Save()
        If invokeValidations Then
            Me.ValidateForm()
        End If
    End Sub

    Private Sub LHControlRequestedListRefresh()
        RaiseEvent HomeLostHistoryChanged()
        Populate()
    End Sub

    Public Sub AddNewLoss()
        Try
            btnAddLossHistory_Click(Me, New EventArgs())
            Exit Sub
        Catch ex As Exception
            HandleError("AddNewLoss", ex)
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
        Try
            If Not IsPostBack Then
                LoadStaticData()
                Populate()
            End If

            AttachLHControlEvents()

            'Dim _script = DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager
            'If Me.Quote IsNot Nothing Then
            '    _script.AddScriptLine("PolicyHolderCount = " + IFM.VR.Common.Helpers.QuickQuoteObjectHelper.PolicyHolderCount(Me.Quote).ToString() + ";")
            '    ' run at all startups to disable or enable policyholder1/2 in relationship to policyholder dd
            '    _script.AddScriptLine("CheckDriverRelationshipToPolicyHolder();", True)
            'End If

            Exit Sub
        Catch ex As Exception
            HandleError("PAGE LOAD", ex)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Repeater ItemDatabound
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rptLossHistories_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptLossHistories.ItemDataBound
        Dim LHctl As ctlHomePersonalLossItem = Nothing
        Try
            LHctl = e.Item.FindControl("ctlLossHistoryItem")
            If LHctl Is Nothing Then Throw New Exception("Loss History Item control not found!")
            LHctl.LossHistoryNumber = e.Item.ItemIndex
            LHctl.Populate()
            Exit Sub
        Catch ex As Exception
            HandleError("rptLossHistories_ItemDataBound", ex)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' ADD button
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub btnAddLossHistory_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAddLossHistory.Click
        Try
            If Me.Quote IsNot Nothing Then
                If Quote.LossHistoryRecords Is Nothing Then
                    Quote.LossHistoryRecords = New List(Of QuickQuoteLossHistoryRecord)()
                End If
                Quote.LossHistoryRecords.Add(New QuickQuoteLossHistoryRecord())
                Save()
                Populate()
                hiddenActiveIM.Value = (Me.Quote.LossHistoryRecords.Count() - 1).ToString()
                RaiseEvent HomeLostHistoryChanged()
            End If
            Exit Sub
        Catch ex As Exception
            HandleError("btnAddLossHistory_Click", ex)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' SAVE Button
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSubmit.Click
        Try
            Save()
            ValidateForm()

            Exit Sub
        Catch ex As Exception
            HandleError("btnSubmit_Click", ex)
            Exit Sub
        End Try
    End Sub

#End Region

End Class