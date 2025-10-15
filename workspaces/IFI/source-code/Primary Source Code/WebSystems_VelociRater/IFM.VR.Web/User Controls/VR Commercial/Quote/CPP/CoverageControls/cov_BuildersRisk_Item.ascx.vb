Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports IFM.PrimativeExtensions
Public Class cov_BuildersRisk_Item
    Inherits VRControlBase

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()

        If GoverningStateQuote.BuildersRiskScheduledLocations Is Nothing Then
            GoverningStateQuote.BuildersRiskScheduledLocations = New List(Of QuickQuoteBuildersRiskScheduledLocation)()
        End If
        If GoverningStateQuote.BuildersRiskScheduledLocations.Any() = False Then
            GoverningStateQuote.BuildersRiskScheduledLocations.Add(New QuickQuoteBuildersRiskScheduledLocation())
            GoverningStateQuote.BuildersRiskScheduledLocations(0).Limit = ""
            GoverningStateQuote.BuildersRiskScheduledLocations(0).AddressInfo = ""
        End If

        Me.brRepeater.DataSource = GoverningStateQuote.BuildersRiskScheduledLocations
        Me.brRepeater.DataBind()
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)

        Me.ValidationHelper.GroupName = "Builders Risk - Scheduled"
        Dim deductibleControl As DropDownList = Parent.FindControl("brDeductible")
        Dim deductibleAmount = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(deductibleControl.SelectedItem.Text)
        Dim totalLimit As Double = 0.0
        For Each ri As RepeaterItem In brRepeater.Items
            Dim txtLimit As TextBox = ri.FindControl("txtLimit")
            Dim LimitAmount = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(txtLimit.Text)
            Dim txtLocation As TextBox = ri.FindControl("txtLocation")
            ' 3.8.7
            If String.IsNullOrEmpty(txtLimit.Text) Then
                Me.ValidationHelper.AddError("Missing Limit", txtLimit.ClientID)
            End If
            ' 3.8.9
            If String.IsNullOrEmpty(txtLocation.Text) Then
                Me.ValidationHelper.AddError("Missing Jobsite Location Description", txtLocation.ClientID)
            End If
            ' 3.8.112
            If LimitAmount > 500000 Then
                Me.ValidationHelper.AddError("You have selected a limit that exceeds your authority, please contact your underwriter for higher limits.", txtLimit.ClientID)
            End If
            ' 3.8.114
            If deductibleAmount >= LimitAmount Then
                Me.ValidationHelper.AddError("Deductible amount selected is equal or greater than the Limit. Please adjust either value.", txtLimit.ClientID)
            End If
            totalLimit += LimitAmount
            ' Total Limit - 3.8.113
            If totalLimit > 1000000 Then
                Me.ValidationHelper.AddError("You have selected a limit that exceeds your authority, please contact your underwriter for higher limits.", txtLimit.ClientID)
            End If

        Next

        Me.ValidateChildControls(valArgs)
    End Sub

    Public Overrides Function Save() As Boolean

        Dim brList As New List(Of QuickQuoteBuildersRiskScheduledLocation)

        For Each ri As RepeaterItem In brRepeater.Items
            Dim loc As New QuickQuoteBuildersRiskScheduledLocation
            Dim txtLimit As TextBox = ri.FindControl("txtLimit")
            Dim txtDescription As TextBox = ri.FindControl("txtLocation")
            Dim location As LinkButton = ri.FindControl("btnDelete")
            Dim itemIndex = ri.ItemIndex

            'If Not String.IsNullOrEmpty(txtLimit.Text) OrElse Not String.IsNullOrEmpty(txtDescription.Text) Then
            loc.Limit = txtLimit.Text
            loc.AddressInfo = txtDescription.Text
            brList.Add(loc)
            'End If
        Next
        GoverningStateQuote.BuildersRiskScheduledLocations = brList

        GoverningStateQuote.BuildersRiskRate = CIMHelper.BuildersRiskRateTable.GetRateForLimit(Me.GetLimit())

        Return True
    End Function

    Private Sub btnAddClick()
        If Me.GoverningStateQuote.IsNotNull Then
            Me.Save_FireSaveEvent(False)
            Me.GoverningStateQuote.BuildersRiskScheduledLocations.AddNew()
            Populate()
            Me.Save_FireSaveEvent(False)
        End If
    End Sub

    Private Sub btnDeleteClick(index)
        If Me.GoverningStateQuote.IsNotNull Then
            Me.Save_FireSaveEvent(False)
            Me.GoverningStateQuote.BuildersRiskScheduledLocations.RemoveAt(index)
            Populate()
            Me.Save_FireSaveEvent(False)
        End If
    End Sub

    Protected Sub brRepeater_Add(source As Object, e As RepeaterCommandEventArgs) Handles brRepeater.ItemCommand
        If e.CommandName = "lnkAdd" Then
            btnAddClick()
        ElseIf e.CommandName = "lnkDelete" Then
            btnDeleteClick(e.Item.ItemIndex)
        End If

    End Sub

    Protected Function GetLimit() As Double
        Return GoverningStateQuote.BuildersRiskScheduledLocations.Sum(Function(a)
                                                                          Return IFM.Common.InputValidation.InputHelpers.TryToGetDouble(a.Limit)
                                                                      End Function)
    End Function

    Public Overrides Sub ClearControl()
        GoverningStateQuote.BuildersRiskScheduledLocations = New List(Of QuickQuoteBuildersRiskScheduledLocation)()
    End Sub
End Class