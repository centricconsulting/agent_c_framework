Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers

Public Class cov_OwnersCargo
    Inherits VRControlBase

    Private Property selectedOption As Boolean

    Public Overrides Sub LoadStaticData()
        QQHelper.LoadStaticDataOptionsDropDown(Me.ocDeductible, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.OwnersCargoAnyOneOwnedVehicleDeductibleId, QuickQuoteStaticDataOption.SortBy.TextAscending, Me.Quote.LobType)
    End Sub

    Public Overrides Sub Populate()
        LoadStaticData()
        If String.IsNullOrWhiteSpace(GoverningStateQuote.OwnersCargoAnyOneOwnedVehicleDeductibleId) = False AndAlso String.IsNullOrWhiteSpace(GoverningStateQuote.OwnersCargoCatastropheLimit) = False Or String.IsNullOrWhiteSpace(GoverningStateQuote.OwnersCargoAnyOneOwnedVehicleLimit) = False Or String.IsNullOrWhiteSpace(GoverningStateQuote.OwnersCargoAnyOneOwnedVehicleDescription) = False Then
            Me.chkOwnersCargo.Checked = True
            If String.IsNullOrWhiteSpace(GoverningStateQuote.OwnersCargoAnyOneOwnedVehicleDeductibleId) = False Then
                'Updated 12/15/2021 for CPP Endorsements Task 66800 MLW
                If IsQuoteReadOnly() Then
                    WebHelper_Personal.SetDropDownValue_ForceDiamondValue(Me.ocDeductible, GoverningStateQuote.OwnersCargoAnyOneOwnedVehicleDeductibleId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.OwnersCargoAnyOneOwnedVehicleDeductibleId)
                Else
                    WebHelper_Personal.SetdropDownFromValue(Me.ocDeductible, GoverningStateQuote.OwnersCargoAnyOneOwnedVehicleDeductibleId)
                End If
            End If
            Me.txtDescription.Text = GoverningStateQuote.OwnersCargoAnyOneOwnedVehicleDescription
            Me.txtLimitPerVehicle.Text = GoverningStateQuote.OwnersCargoAnyOneOwnedVehicleLimit
            Try
                Dim numVehicles As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(GoverningStateQuote.OwnersCargoCatastropheLimit) / IFM.Common.InputValidation.InputHelpers.TryToGetDouble(GoverningStateQuote.OwnersCargoAnyOneOwnedVehicleLimit)
                Me.txtNumOfVehicles.Text = If(Double.IsNaN(numVehicles), "", Math.Ceiling(numVehicles).ToString())
            Catch ex As Exception ' just fail silently - could divide by zero
            End Try

            Me.txtCatLimit.Text = GoverningStateQuote.OwnersCargoCatastropheLimit
        Else
            Me.chkOwnersCargo.Checked = False
        End If
        If selectedOption Then
            chkOwnersCargo.Checked = selectedOption
        End If

    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Public Overrides Function Save() As Boolean
        selectedOption = False
        If Me.chkOwnersCargo.Checked Then
            If GoverningStateQuote() IsNot Nothing Then
                selectedOption = True
                GoverningStateQuote.OwnersCargoAnyOneOwnedVehicleDeductibleId = Me.ocDeductible.SelectedValue
                GoverningStateQuote.OwnersCargoAnyOneOwnedVehicleDescription = Me.txtDescription.Text.Trim()
                GoverningStateQuote.OwnersCargoAnyOneOwnedVehicleLimit = Me.txtLimitPerVehicle.Text.Trim()

                Dim catLimit As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtLimitPerVehicle.Text.Trim()) * IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtNumOfVehicles.Text)
                GoverningStateQuote.OwnersCargoCatastropheLimit = If(catLimit > 0, catLimit.ToString(), "")

                GoverningStateQuote.OwnersCargoAnyOneOwnedVehicleRate = CIMHelper.OwnerCargoRateTable.GetRateForLimit(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(GoverningStateQuote.OwnersCargoCatastropheLimit))
            End If
        Else
            ClearControl()
        End If

        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        If chkOwnersCargo.Checked Then
            Me.ValidationHelper.GroupName = "Owners Cargo"
            Dim deductibleAmount = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.ocDeductible.SelectedItem.ToString)

            Dim limitPerVehicle = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtLimitPerVehicle.Text)

            Dim numVehicles = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.txtNumOfVehicles.Text)

            If String.IsNullOrEmpty(ocDeductible.SelectedValue) Then
                Me.ValidationHelper.AddError("Missing Deductible", ocDeductible.ClientID)
            End If

            If String.IsNullOrEmpty(Me.txtDescription.Text) Then
                Me.ValidationHelper.AddError("Missing Cargo description", txtDescription.ClientID)
            End If

            If String.IsNullOrEmpty(Me.txtNumOfVehicles.Text) Then
                Me.ValidationHelper.AddError("Missing Number of Vehicles", txtNumOfVehicles.ClientID)
            End If

            If String.IsNullOrEmpty(Me.txtLimitPerVehicle.Text) Then
                Me.ValidationHelper.AddError("Missing Limit Per Vehicles", txtLimitPerVehicle.ClientID)
            End If

            If deductibleAmount >= limitPerVehicle Then
                Me.ValidationHelper.AddError("Deductible amount selected is equal or greater than the Limit. Please adjust either value.", txtLimitPerVehicle.ClientID)
            End If
            '3.8.127
            If limitPerVehicle > 100000 Then
                Me.ValidationHelper.AddError("You have selected a limit that exceeds your authority, please contact your underwriter for higher limits.", txtLimitPerVehicle.ClientID)
            End If
            '3.8.128
            If (limitPerVehicle * numVehicles) > 100000 Then
                Me.ValidationHelper.AddError("You have selected a limit that exceeds your authority, please contact your underwriter for higher limits.", txtLimitPerVehicle.ClientID)
            End If

            Me.ValidateChildControls(valArgs)
        End If
    End Sub

    Public Overrides Sub ClearControl()
        GoverningStateQuote.OwnersCargoAnyOneOwnedVehicleDeductibleId = ""
        GoverningStateQuote.OwnersCargoAnyOneOwnedVehicleDescription = ""
        GoverningStateQuote.OwnersCargoAnyOneOwnedVehicleLimit = ""
        GoverningStateQuote.OwnersCargoCatastropheLimit = ""
        GoverningStateQuote.OwnersCargoAnyOneOwnedVehicleRate = ""
        txtDescription.Text = String.Empty
        txtNumOfVehicles.Text = String.Empty
        txtLimitPerVehicle.Text = String.Empty
        txtCatLimit.Text = String.Empty
    End Sub

    Public Overrides Function hasSetting() As Boolean
        Return Me.chkOwnersCargo.Checked
    End Function

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles clearButton.Click
        If chkOwnersCargo.Checked = False Then
            ClearControl()
            Me.Save_FireSaveEvent(False)
            Populate()
        End If
    End Sub
End Class