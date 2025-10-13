Imports IFM.VR.Common.Helpers.CPP
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects

Public Class cov_MotorTruckCargo
    Inherits VRControlBase

    Private Property selectedOption As Boolean
    Public Overrides Sub LoadStaticData()
        If UnScheduledMotorTruckCargoHelper.IsUnScheduledMotorTruckCargoAvailable(Quote) Then
            QQHelper.LoadStaticDataOptionsDropDown(Me.mtUnScheduledDeductible, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.MotorTruckCargoUnScheduledVehicleDeductibleId, QuickQuoteStaticDataOption.SortBy.TextAscending, Me.Quote.LobType)
        Else
            QQHelper.LoadStaticDataOptionsDropDown(Me.mtDeductible, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.MotorTruckCargoScheduledVehicleDeductibleId, QuickQuoteStaticDataOption.SortBy.TextAscending, Me.Quote.LobType)
        End If
    End Sub

    Public Overrides Sub Populate()
        LoadStaticData()
        mtSubGroup.Attributes.Add("style", "display:none;")
        mtUnSubGroup.Attributes.Add("style", "display:none;")

        If UnScheduledMotorTruckCargoHelper.IsUnScheduledMotorTruckCargoAvailable(Quote) Then
            mtUnSubGroup.Attributes.Add("style", "display:'';")
            If String.IsNullOrWhiteSpace(GoverningStateQuote.MotorTruckCargoUnScheduledVehicleDeductibleId) = False AndAlso String.IsNullOrWhiteSpace(GoverningStateQuote.MotorTruckCargoUnScheduledVehicleCatastropheLimit) = False OrElse String.IsNullOrWhiteSpace(GoverningStateQuote.MotorTruckCargoUnScheduledAnyVehicleLimit) = False OrElse String.IsNullOrWhiteSpace(GoverningStateQuote.MotorTruckCargoScheduledVehicleDescription) = False Then
                Me.chkUnscheduledTruckCargo.Checked = True
                If String.IsNullOrWhiteSpace(GoverningStateQuote.MotorTruckCargoUnScheduledVehicleDeductibleId) = False Then

                    If IsQuoteReadOnly() Then
                        WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.mtUnScheduledDeductible, GoverningStateQuote.MotorTruckCargoUnScheduledVehicleDeductibleId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.MotorTruckCargoUnScheduledVehicleDeductibleId)
                    Else
                        WebHelper_Personal.SetdropDownFromValue(Me.mtUnScheduledDeductible, GoverningStateQuote.MotorTruckCargoUnScheduledVehicleDeductibleId)
                    End If
                End If
                Me.txtUnScheduledDescription.Text = GoverningStateQuote.MotorTruckCargoUnScheduledVehicleDescription
                Me.txtUnScheduledLimitPerVehicle.Text = GoverningStateQuote.MotorTruckCargoUnScheduledAnyVehicleLimit
                Me.txtUnScheduledNumberOfVehicles.Text = GoverningStateQuote.MotorTruckCargoUnScheduledNumberOfVehicles
                Try
                    Dim numVehicles As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(GoverningStateQuote.MotorTruckCargoUnScheduledVehicleCatastropheLimit) / IFM.Common.InputValidation.InputHelpers.TryToGetDouble(GoverningStateQuote.MotorTruckCargoUnScheduledAnyVehicleLimit)
                    Me.txtUnScheduledNumberOfVehicles.Text = If(Double.IsNaN(numVehicles), "", Math.Ceiling(numVehicles).ToString())
                Catch ex As Exception ' just fail silently - could divide by zero
                End Try

                Me.txtUnScheduledCatLimit.Text = GoverningStateQuote.MotorTruckCargoUnScheduledVehicleCatastropheLimit
            Else
                Me.chkUnscheduledTruckCargo.Checked = False
            End If

            If selectedOption Then
                chkUnscheduledTruckCargo.Checked = selectedOption
            End If
        Else
            mtSubGroup.Attributes.Add("style", "display:'';")
            If String.IsNullOrWhiteSpace(GoverningStateQuote.MotorTruckCargoScheduledVehicleDeductibleId) = False AndAlso (GoverningStateQuote.MotorTruckCargoScheduledVehicles IsNot Nothing AndAlso GoverningStateQuote.MotorTruckCargoScheduledVehicles.Any()) Or String.IsNullOrWhiteSpace(GoverningStateQuote.MotorTruckCargoScheduledVehicleDescription) = False Then
                Me.chkTruckCargo.Checked = True
                If String.IsNullOrWhiteSpace(GoverningStateQuote.MotorTruckCargoScheduledVehicleDeductibleId) = False Then
                    'Updated 12/15/2021 for CPP Endorsements Task 66800 MLW
                    If IsQuoteReadOnly() Then
                        WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.mtDeductible, GoverningStateQuote.MotorTruckCargoScheduledVehicleDeductibleId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.MotorTruckCargoScheduledVehicleDeductibleId)
                    Else
                        WebHelper_Personal.SetdropDownFromValue(Me.mtDeductible, GoverningStateQuote.MotorTruckCargoScheduledVehicleDeductibleId)
                    End If
                End If

                Me.txtDescription.Text = GoverningStateQuote.MotorTruckCargoScheduledVehicleDescription
                Me.txtCatLimit.Text = GoverningStateQuote.MotorTruckCargoScheduledVehicleCatastropheLimit
            Else
                Me.chkTruckCargo.Checked = False
            End If
        End If
        Me.PopulateChildControls()
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Public Overrides Function Save() As Boolean
        If UnScheduledMotorTruckCargoHelper.IsUnScheduledMotorTruckCargoAvailable(Quote) Then
            selectedOption = False
            If Me.chkUnscheduledTruckCargo.Checked Then
                If GoverningStateQuote() IsNot Nothing Then
                    selectedOption = True
                    GoverningStateQuote.MotorTruckCargoUnScheduledVehicleDeductibleId = mtUnScheduledDeductible.SelectedValue
                    GoverningStateQuote.MotorTruckCargoUnScheduledVehicleDescription = Me.txtUnScheduledDescription.Text.Trim()
                    GoverningStateQuote.MotorTruckCargoUnScheduledAnyVehicleLimit = Me.txtUnScheduledLimitPerVehicle.Text.Trim()
                    GoverningStateQuote.MotorTruckCargoUnScheduledNumberOfVehicles = Me.txtUnScheduledNumberOfVehicles.Text.Trim()

                    Dim catLimit As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtUnScheduledLimitPerVehicle.Text.Trim()) * IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtUnScheduledNumberOfVehicles.Text)
                    GoverningStateQuote.MotorTruckCargoUnScheduledVehicleCatastropheLimit = If(catLimit > 0, catLimit.ToString(), "")

                    GoverningStateQuote.MotorTruckCargoUnScheduledVehicleRate = CIMHelper.MotorTruckCargoRateTable.GetRateForLimit(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(GoverningStateQuote.MotorTruckCargoUnScheduledVehicleCatastropheLimit))
                End If
            Else
                ClearControl()
            End If
        Else
            If chkTruckCargo.Checked Then
                GoverningStateQuote.MotorTruckCargoScheduledVehicleDeductibleId = mtDeductible.SelectedValue
                GoverningStateQuote.MotorTruckCargoScheduledVehicleDescription = Me.txtDescription.Text.Trim()
                Me.SaveChildControls()
            Else
                ClearControl()
            End If
        End If
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        If UnScheduledMotorTruckCargoHelper.IsUnScheduledMotorTruckCargoAvailable(Quote) Then
            If Me.chkUnscheduledTruckCargo.Checked Then
                Me.ValidationHelper.GroupName = "Motor Truck Cargo"

                Dim deductibleAmount = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.mtUnScheduledDeductible.SelectedItem.ToString)
                Dim limitPerVehicle = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtUnScheduledLimitPerVehicle.Text)
                Dim numVehicles = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtUnScheduledNumberOfVehicles.Text)

                If String.IsNullOrEmpty(mtUnScheduledDeductible.SelectedValue) Then
                    Me.ValidationHelper.AddError("Missing Deductible", mtUnScheduledDeductible.ClientID)
                End If

                If String.IsNullOrEmpty(txtUnScheduledDescription.Text) Then
                    Me.ValidationHelper.AddError("Missing Cargo description", txtUnScheduledDescription.ClientID)
                End If

                If String.IsNullOrEmpty(txtUnScheduledLimitPerVehicle.Text) Then
                    Me.ValidationHelper.AddError("Missing Limit Per Vehicles", txtUnScheduledLimitPerVehicle.ClientID)
                End If

                If String.IsNullOrEmpty(txtUnScheduledNumberOfVehicles.Text) Then
                    Me.ValidationHelper.AddError("Missing Number of Vehicles", txtUnScheduledNumberOfVehicles.ClientID)
                End If

                If deductibleAmount >= limitPerVehicle Then
                    Me.ValidationHelper.AddError("Deductible amount selected is equal or greater than the Limit. Please adjust either value.", txtUnScheduledLimitPerVehicle.ClientID)
                End If

                If limitPerVehicle > 100000 Then
                    Me.ValidationHelper.AddError("You have selected a limit that exceeds your authority, please contact your underwriter for higher limits.", txtUnScheduledLimitPerVehicle.ClientID)
                End If

                If (limitPerVehicle * numVehicles) > 100000 Then
                    Me.ValidationHelper.AddError("You have selected a limit that exceeds your authority, please contact your underwriter for higher limits.", txtUnScheduledLimitPerVehicle.ClientID)
                End If

                Me.ValidateChildControls(valArgs)
            End If
        Else
            If Me.chkTruckCargo.Checked Then
                Me.ValidationHelper.GroupName = "Motor Truck Cargo – Scheduled"

                If String.IsNullOrEmpty(mtDeductible.SelectedValue) Then
                    Me.ValidationHelper.AddError("Missing Deductible", mtDeductible.ClientID)
                End If

                If String.IsNullOrEmpty(txtDescription.Text) Then
                    Me.ValidationHelper.AddError("Missing Cargo description", txtDescription.ClientID)
                End If

                '3.8.125
                If IFM.Common.InputValidation.InputHelpers.TryToGetDouble(GoverningStateQuote.MotorTruckCargoScheduledVehicleCatastropheLimit) > 100000 Then
                    Me.ValidationHelper.AddError("You have selected a limit that exceeds your authority, please contact your underwriter for higher limits.", txtCatLimit.ClientID)
                End If

                Me.ValidateChildControls(valArgs)
            End If
        End If
    End Sub

    Public Overrides Sub ClearControl()
        If UnScheduledMotorTruckCargoHelper.IsUnScheduledMotorTruckCargoAvailable(Quote) Then
            GoverningStateQuote.MotorTruckCargoUnScheduledVehicleDeductibleId = ""
            GoverningStateQuote.MotorTruckCargoUnScheduledVehicleDescription = ""
            GoverningStateQuote.MotorTruckCargoUnScheduledVehicleCatastropheLimit = ""
            GoverningStateQuote.MotorTruckCargoUnScheduledAnyVehicleLimit = ""
            GoverningStateQuote.MotorTruckCargoUnScheduledVehicleRate = ""
            GoverningStateQuote.MotorTruckCargoUnScheduledNumberOfVehicles = ""
            txtUnScheduledDescription.Text = String.Empty
            txtUnScheduledNumberOfVehicles.Text = String.Empty
            txtUnScheduledLimitPerVehicle.Text = String.Empty
            txtUnScheduledCatLimit.Text = String.Empty
        Else
            GoverningStateQuote.MotorTruckCargoScheduledVehicleDeductibleId = ""
            GoverningStateQuote.MotorTruckCargoScheduledVehicleDescription = ""
            GoverningStateQuote.MotorTruckCargoScheduledVehicleCatastropheLimit = ""
        End If
        ClearChildControls()
    End Sub

    Public Overrides Function hasSetting() As Boolean
        If UnScheduledMotorTruckCargoHelper.IsUnScheduledMotorTruckCargoAvailable(Quote) Then
            Return Me.chkUnscheduledTruckCargo.Checked
        Else
            Return Me.chkTruckCargo.Checked
        End If
    End Function

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles clearButton.Click
        If UnScheduledMotorTruckCargoHelper.IsUnScheduledMotorTruckCargoAvailable(Quote) Then
            If chkUnscheduledTruckCargo.Checked = False Then
                ClearControl()
                Me.Save_FireSaveEvent(False)
                Populate()
            End If
        Else
            If chkTruckCargo.Checked = False Then
                ClearControl()
                Me.Save_FireSaveEvent(False)
                Populate()
            End If
        End If
    End Sub
End Class