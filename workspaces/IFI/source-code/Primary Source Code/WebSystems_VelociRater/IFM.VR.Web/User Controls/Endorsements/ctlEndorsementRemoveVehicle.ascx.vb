Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonObjects

Public Class ctlEndorsementRemoveVehicle
    Inherits VRControlBase

    Public Event RemoveSelectedVehicle(VehicleNumber As Integer)

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        btnCancel.Focus()
        PopulateVehicleDdl()
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        If Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal Then
            Me.VRScript.CreatePopupForm(Me.divEndorsementRemoveVehiclePopup.ClientID, "Remove Vehicle", 400, 200, True, True, False, "", "")
        End If
    End Sub

    Public Overrides Function Save() As Boolean
    End Function

    Protected Sub btnRemove_Click(sender As Object, e As EventArgs) Handles btnRemove.Click
        Dim VehicleNumber = ddlRemoveVehicleSelect.SelectedValue

        ResetAndHidePopup()
        RaiseEvent RemoveSelectedVehicle(VehicleNumber)

    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        ResetAndHidePopup()
    End Sub

    Private Sub PopulateVehicleDdl()
        Dim newListItem As ListItem
        Dim vehicleIndex As Integer = 0

        If Quote?.Vehicles IsNot Nothing Then
            ddlRemoveVehicleSelect.Items.Clear()
            'Add a Blank Line
            newListItem = New ListItem("", "-1")
            ddlRemoveVehicleSelect.Items.Add(newListItem)

            For Each vehicle As QuickQuoteVehicle In Quote.Vehicles
                ' Use Last 4 of Vin if more than 4, else use Vin
                Dim VinResult As String = vehicle.Vin
                If VinResult.Length > 4 Then
                    VinResult = VinResult.Substring(VinResult.Length - 4)
                End If

                Dim Title = String.Format("{0} {1} {2} {3}", vehicle.Year, vehicle.Make, vehicle.Model, VinResult).ToUpper()
                Dim Value = vehicleIndex
                newListItem = New ListItem(Title, Value)
                ddlRemoveVehicleSelect.Items.Add(newListItem)
                vehicleIndex = vehicleIndex + 1
            Next
        End If
    End Sub

    Private Sub ResetAndHidePopup()
        Me.Visible = False
        ddlRemoveVehicleSelect.SelectedIndex = 0
        btnCancel.Focus()
    End Sub


End Class
