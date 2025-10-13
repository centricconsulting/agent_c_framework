Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonObjects

Public Class ctl_Farm_Structures_App
    Inherits VRControlBase

    Public Property MyLocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_locationIndex")
        End Get
        Set(value As Int32)
            ViewState("vs_locationIndex") = value
        End Set
    End Property

    Public ReadOnly Property MyLocation As QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing Then
                Return Me.Quote.Locations.GetItemAtIndex(MyLocationIndex)
            End If
            Return Nothing
        End Get
    End Property

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.MainAccordionDivId, Me.hddnAccord, "0")
        Me.VRScript.StopEventPropagation(Me.lnkBtnSave.ClientID)
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        Me.Visible = Me.MyLocation IsNot Nothing AndAlso Me.MyLocation.Buildings IsNot Nothing AndAlso Me.MyLocation.Buildings.Any()
        Me.lblHeader.Text = String.Format("Structures for Location #{0} - Description", MyLocationIndex + 1)
        Me.Repeater1.DataSource = Me.MyLocation.Buildings
        Me.Repeater1.DataBind()
        Me.PopulateChildControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.MainAccordionDivId = Me.divMain.ClientID
    End Sub

    Public Overrides Function Save() As Boolean
        For Each item As RepeaterItem In Me.Repeater1.Items
            Dim txtDescription As TextBox = item.FindControl("txtDescription")

            Me.MyLocation.Buildings(item.ItemIndex).Description = txtDescription.Text.Trim()
        Next
        Me.SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = String.Format("Location #{0} - Structures", (Me.MyLocationIndex + 1).ToString())
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        For Each item As RepeaterItem In Me.Repeater1.Items
            Dim txtDescription As TextBox = item.FindControl("txtDescription")
            If (String.IsNullOrWhiteSpace(txtDescription.Text)) Then
                Me.ValidationHelper.AddError(txtDescription, "Missing Description", accordList)
            End If

        Next

        Me.ValidateChildControls(valArgs)
    End Sub

    Protected Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkBtnSave.Click
        Me.Save_FireSaveEvent()
    End Sub

    Private Sub Repeater1_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles Repeater1.ItemDataBound
        If e.Item.ItemIndex >= 0 Then
            Dim lblLine As Label = e.Item.FindControl("lblLineNumber")
            Dim txtDescription As TextBox = e.Item.FindControl("txtDescription")
            lblLine.Text = (e.Item.ItemIndex + 1).ToString()
            txtDescription.Text = Me.MyLocation.Buildings(e.Item.ItemIndex).Description
        End If

    End Sub

End Class