Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods

Public Class ctlRVWList
    Inherits System.Web.UI.UserControl
    Implements IVRUI_P

    Public Event HomeRVWatercraftChanged()
    Public Event SaveRequested(ByVal index As Integer, ByVal WhichControl As String)

    Private ClassName As String = "ctlRVWList"
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

    Public Property RVWatercraftNumber As Int32
        Get
            If ViewState("vs_RVatercraftNum") Is Nothing Then
                ViewState("vs_RVWatercraftNum") = -1
            End If
            Return CInt(ViewState("vs_RVWatercraftNum"))
        End Get
        Set(value As Int32)
            ViewState("vs_RVWatercraftNum") = value
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
            Return Me.hiddenActiveRVW.Value
        End Get
        Set(value As String)
            Me.hiddenActiveRVW.Value = value
        End Set
    End Property

    'Private Sub HandleError(ByVal RoutineName As String, ByVal exc As Exception)
    '    ShowMessage(ClassName & ":" & RoutineName & ": " & exc.Message)
    'End Sub

    'Private Sub ClearErrorMessage()
    '    lblMsg.Text = "&nbsp;"
    'End Sub

    'Private Sub ShowMessage(ByVal msg As String)
    '    lblMsg.Text = msg
    'End Sub

    ''' <summary>
    ''' Loops through the repeaters, saves each item
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Save() As Boolean Implements IVRUI_P.Save
        Try
            Dim index As Integer = 0
            For Each cntrl As RepeaterItem In rptRVW.Items
                Dim RVWControl As ctlRVWItem = cntrl.FindControl("ctlRVWItem")
                RVWControl.Save()
                RVWControl.Populate()
                index += 1
            Next
            RaiseEvent SaveRequested(-1, ClassName)
            RaiseEvent HomeRVWatercraftChanged()
            Return True
        Catch ex As Exception
            'HandleError("Save", ex)
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
            rptRVW.DataSource = Nothing
            rptRVW.DataBind()

            If Quote Is Nothing OrElse Quote.Locations Is Nothing OrElse Quote.Locations.Count <= 0 _
                OrElse Quote.Locations(0) Is Nothing OrElse Quote.Locations(0).RvWatercrafts Is Nothing _
                OrElse Quote.Locations(0).RvWatercrafts.Count <= 0 Then
                ' btnSubmit.Visible = False
            Else
                rptRVW.DataSource = Quote.Locations(0).RvWatercrafts
                rptRVW.DataBind()
                'btnSubmit.Visible = Me.Quote.Locations(0).RvWatercrafts.Any
            End If

            Exit Sub
        Catch ex As Exception
            'HandleError("Populate", ex)
            Exit Sub
        End Try
    End Sub

    Public Sub ValidateForm() Implements IVRUI_P.ValidateForm
        Try
            ValidationHelper.Clear()
            Dim _script = DirectCast(Page.Master, VelociRater).StartUpScriptManager

            For Each cntrl As RepeaterItem In rptRVW.Items
                Dim RVWControl As ctlRVWItem = cntrl.FindControl("ctlRVWItem")
                RVWControl.ValidateForm()
            Next

            Dim valSum As ctlValidationSummary = DirectCast(Page.Master, VelociRater).ValidationSummary
            valSum.InsertValidationControl(Me.ValidationHelper)

            Exit Sub
        Catch ex As Exception
            'HandleError("ValidateForm", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub ItemSaveDetected()
        RaiseEvent SaveRequested(-1, ClassName)
    End Sub

    Public Sub AttachRVWControlEvents()
        Dim index As Integer = 0
        Try
            ' need to wire up the remove events for each control every single time
            For Each cntrl As RepeaterItem In rptRVW.Items
                Dim RVWControl As ctlRVWItem = cntrl.FindControl("ctlRVWItem")
                AddHandler RVWControl.ItemRemoveRequest, AddressOf ItemRemoveRequest
                AddHandler RVWControl.SaveRequested, AddressOf ItemSaveDetected
                index += 1
            Next

            Exit Sub
        Catch ex As Exception
            'HandleError("AttachRVWControlEvents", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub ItemRemoveRequest()
        Dim err As String = ""
        Try
            RaiseEvent SaveRequested(-1, ClassName)
            RaiseEvent HomeRVWatercraftChanged()
            Populate()
            Dim _script = DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager
            _script.AddScriptLine("ifm.vr.ui.LockTree_Freeze(); ")

            Exit Sub
        Catch ex As Exception
            'HandleError("ItemRemoveRequest", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub RVWControlNewRVWRequested()
        'btnAdd_Click(Me, New EventArgs())
    End Sub

    Private Sub RVWControlRemoving(index As Integer)
        Dim activePan As Int32 = 0
        If Int32.TryParse(Me.hiddenActiveRVW.Value, activePan) Then
            If activePan >= index Then
                Me.hiddenActiveRVW.Value = (activePan - 1).ToString()
            End If
        End If
    End Sub

    Private Sub IMControlRequestedSave(invokeValidations As Boolean)
        Save()
        If invokeValidations Then
            Me.ValidateForm()
        End If
    End Sub

    Private Sub IMControlRequestedListRefresh()
        RaiseEvent HomeRVWatercraftChanged()
        Populate()
    End Sub

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

            AttachRVWControlEvents()

            Exit Sub
        Catch ex As Exception
            'HandleError("PAGE LOAD", ex)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' ADD NEW linkbutton
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    'Protected Sub lnkAddRV_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkAddRV.Click
    '    Try
    '        'btnAdd_Click(Me, New EventArgs())
    '        Exit Sub
    '    Catch ex As Exception
    '        HandleError("lnkAddRV_Click", ex)
    '        Exit Sub
    '    End Try
    'End Sub

    ''' <summary>
    ''' Repeater ItemDatabound
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub rptRVW_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptRVW.ItemDataBound
        Dim ItemCtl As ctlRVWItem = Nothing
        Try
            ItemCtl = e.Item.FindControl("ctlRVWItem")
            If ItemCtl Is Nothing Then Throw New Exception("Item control not found!")
            ItemCtl.RVWatercraftNumber = e.Item.ItemIndex
            ItemCtl.Populate()
            Exit Sub
        Catch ex As Exception
            'HandleError("rptRVW_ItemDataBound", ex)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' SAVE Button
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    'Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSubmit.Click
    '    Try
    '        ValidateForm()
    '        Save()

    '        Exit Sub
    '    Catch ex As Exception
    '        HandleError("btnSubmit_Click", ex)
    '        Exit Sub
    '    End Try
    'End Sub

    ''' <summary>
    ''' ADD button
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ' ''' <remarks></remarks>
    'Protected Sub btnAdd_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnAdd.Click
    '    Dim ri As RepeaterItem = Nothing
    '    Dim ctl As ctlRVWItem = Nothing

    '    Try
    '        If Me.Quote IsNot Nothing AndAlso Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0 Then
    '            If Quote.Locations(0).RvWatercrafts Is Nothing Then
    '                Quote.Locations(0).RvWatercrafts = New List(Of QuickQuoteRvWatercraft)()
    '            End If
    '            Quote.Locations(0).RvWatercrafts.Add(New QuickQuoteRvWatercraft())
    '            Save()
    '            Populate()
    '            hiddenActiveRVW.Value = (Me.Quote.Locations(0).RvWatercrafts.Count() - 1).ToString()
    '            RaiseEvent HomeRVWatercraftChanged()
    '        End If
    '        Exit Sub
    '    Catch ex As Exception
    '        HandleError("btnAdd_Click", ex)
    '        Exit Sub
    '    End Try
    'End Sub
End Class