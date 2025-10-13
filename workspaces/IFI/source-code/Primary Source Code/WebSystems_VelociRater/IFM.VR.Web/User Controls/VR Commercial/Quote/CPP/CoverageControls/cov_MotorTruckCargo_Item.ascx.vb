Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.CPP
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects
Public Class cov_MotorTruckCargo_Item
    Inherits VRControlBase

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        'If UnScheduledMotorTruckCargoHelper.IsUnScheduledMotorTruckCargoAvailable(Quote) = False Then
        If GoverningStateQuote.MotorTruckCargoScheduledVehicles Is Nothing Then
                GoverningStateQuote.MotorTruckCargoScheduledVehicles = New List(Of QuickQuoteScheduledVehicle)()
            End If
            If GoverningStateQuote.MotorTruckCargoScheduledVehicles.Any() = False Then
                GoverningStateQuote.MotorTruckCargoScheduledVehicles.Add(New QuickQuoteScheduledVehicle())
            End If

            Me.mtRepeater.DataSource = GoverningStateQuote.MotorTruckCargoScheduledVehicles
            Me.mtRepeater.DataBind()
        'End If
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Public Overrides Function Save() As Boolean
        Dim mtList As New List(Of QuickQuoteScheduledVehicle)
        Dim totalLimit As Double = 0.0

            For Each ri As RepeaterItem In mtRepeater.Items
                Dim mtVehicle As New QuickQuoteScheduledVehicle
                Dim txtLimit As TextBox = ri.FindControl("txtLimit")
                Dim txtYear As TextBox = ri.FindControl("txtYear")
                Dim txtMake As TextBox = ri.FindControl("txtMake")
                Dim txtModel As TextBox = ri.FindControl("txtModel")
                Dim txtVin As TextBox = ri.FindControl("txtVin")
                mtVehicle.Limit = txtLimit.Text.Trim()
                mtVehicle.Year = txtYear.Text.Trim()
                mtVehicle.Make = txtMake.Text.Trim()
                mtVehicle.Model = txtModel.Text.Trim()
                mtVehicle.VIN = txtVin.Text.Trim()
                totalLimit += IFM.Common.InputValidation.InputHelpers.TryToGetDouble(mtVehicle.Limit)
                mtList.Add(mtVehicle)
            Next
            GoverningStateQuote.MotorTruckCargoScheduledVehicles = mtList
            GoverningStateQuote.MotorTruckCargoScheduledVehicleRate = CIMHelper.MotorTruckCargoRateTable.GetRateForLimit(totalLimit)
            GoverningStateQuote.MotorTruckCargoScheduledVehicleCatastropheLimit = totalLimit

        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)

        If UnScheduledMotorTruckCargoHelper.IsUnScheduledMotorTruckCargoAvailable(Quote) = False Then
            MyBase.ValidateControl(valArgs)
            Me.ValidationHelper.GroupName = "Motor Truck Cargo – Scheduled"
            Dim deductibleText As String = QQHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.FineArtsDeductibleId, GoverningStateQuote.MotorTruckCargoScheduledVehicleDeductibleId)
            Dim deductibleAmount = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(deductibleText)

            For Each ri As RepeaterItem In mtRepeater.Items
                Dim vehicleLimitControl As TextBox = ri.FindControl("txtLimit")
                Dim vehicleLimitAmount = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(vehicleLimitControl.Text)

                If String.IsNullOrEmpty(vehicleLimitControl.Text) Then
                    Me.ValidationHelper.AddError("Missing Limit", vehicleLimitControl.ClientID)
                End If

                If deductibleAmount >= vehicleLimitAmount Then
                    Me.ValidationHelper.AddError("Deductible amount selected is equal or greater than the Limit. Please adjust either value.", vehicleLimitControl.ClientID)
                End If
                '3.8.126
                If vehicleLimitAmount > 100000 Then
                    Me.ValidationHelper.AddError("You have selected a limit that exceeds your authority, please contact your underwriter for higher limits.", vehicleLimitControl.ClientID)
                End If
            Next

            Me.ValidateChildControls(valArgs)
        End If
    End Sub

    Private Sub btnAddClick()
        If Me.GoverningStateQuote.IsNotNull Then
            Me.Save_FireSaveEvent(False)
            Me.GoverningStateQuote.MotorTruckCargoScheduledVehicles.AddNew()
            Populate()
            Me.Save_FireSaveEvent(False)
        End If
    End Sub

    Private Sub btnDeleteClick(index)
        If Me.GoverningStateQuote.IsNotNull Then
            Me.Save_FireSaveEvent(False)
            Me.GoverningStateQuote.MotorTruckCargoScheduledVehicles.RemoveAt(index)
            Populate()
            Me.Save_FireSaveEvent(False)
        End If
    End Sub

    Protected Sub mtRepeater_Add(source As Object, e As RepeaterCommandEventArgs) Handles mtRepeater.ItemCommand
        If e.CommandName = "lnkAdd" Then
            btnAddClick()
        ElseIf e.CommandName = "lnkDelete" Then
            btnDeleteClick(e.Item.ItemIndex)
        End If
    End Sub

    Private Sub ceRepeater_Add(sender As Object, e As RepeaterItemEventArgs) Handles mtRepeater.ItemDataBound
        Dim tool As Object = e.Item.DataItem
        Dim spacer As HtmlTableRow = e.Item.FindControl("Spacer")

        If GoverningStateQuote.MotorTruckCargoScheduledVehicles.Count > 1 And spacer IsNot Nothing Then
            spacer.Visible = True
        End If
    End Sub
    Public Overrides Sub ClearControl()
        GoverningStateQuote.MotorTruckCargoScheduledVehicleDeductibleId = ""
        GoverningStateQuote.MotorTruckCargoScheduledVehicleDescription = ""
        GoverningStateQuote.MotorTruckCargoScheduledVehicles = Nothing
        GoverningStateQuote.MotorTruckCargoScheduledVehicleRate = ""
        GoverningStateQuote.MotorTruckCargoScheduledVehicleCatastropheLimit = ""
    End Sub
End Class