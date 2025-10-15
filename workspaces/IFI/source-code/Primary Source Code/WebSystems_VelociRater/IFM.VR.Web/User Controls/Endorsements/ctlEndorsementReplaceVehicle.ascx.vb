Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonObjects

Public Class ctlEndorsementReplaceVehicle
    Inherits VRControlBase
    Public Event ReplaceSelectedVehicle(CoverageRequest As CoverageRequestType, VehicleNumber As Integer)

    Private _replacementIndex As Integer
    Public Property ReplacementIndex As Integer
        Get
            Return _replacementIndex
        End Get
        Set(ByVal value As Integer)
            _replacementIndex = value
        End Set
    End Property
    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        If rdoCoverageList.Items.Count = 0 Then
            rdoCoverageList.Items.Add("Replace vehicle and keep existing coverage")
            rdoCoverageList.Items.Add("Replace vehicle and add different coverage")
        End If
        btnCancel.Focus()
        PopulateVehicleDdl()
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        If Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal Then
            Me.VRScript.CreatePopupForm(Me.divEndorsementReplaceVehiclePopup.ClientID, "Vehicle Replacement", 400, 200, True, True, False, "", "")
        End If
    End Sub

    Public Overrides Function Save() As Boolean
    End Function

    Protected Sub btnReplace_Click(sender As Object, e As EventArgs) Handles btnReplace.Click
        Dim CoverageRequest As CoverageRequestType = rdoCoverageList.SelectedIndex
        Dim VehicleNumber = ddlReplaceVehicleSelect.SelectedValue

        If CoverageRequest < 0 Then
            ' Add Validation
        Else
            ResetAndHidePopup()
            RaiseEvent ReplaceSelectedVehicle(CoverageRequest, VehicleNumber)
        End If
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        ResetAndHidePopup()
    End Sub

    Private Sub PopulateVehicleDdl()
        Dim newListItem As ListItem
        Dim vehicleIndex As Integer = 0

        If Quote?.Vehicles IsNot Nothing Then
            ddlReplaceVehicleSelect.Items.Clear()
            For Each vehicle As QuickQuoteVehicle In Quote.Vehicles
                Dim Title = String.Format("{0} {1} {2}", vehicle.Year, vehicle.Make, vehicle.Model).ToUpper()
                Dim Value = vehicleIndex
                newListItem = New ListItem(Title, Value)
                ddlReplaceVehicleSelect.Items.Add(newListItem)
                vehicleIndex = vehicleIndex + 1
            Next
            If ddlReplaceVehicleSelect.SelectedIndex > -1 Then
                ddlReplaceVehicleSelect.SelectedIndex = ReplacementIndex
            End If
        End If
        ReplacementIndex = -1
    End Sub

    Private Sub ResetAndHidePopup()
        Me.Visible = False
        rdoCoverageList.SelectedIndex = -1
        ddlReplaceVehicleSelect.SelectedIndex = 0
        btnCancel.Focus()
    End Sub


End Class

Public Enum CoverageRequestType
    RetainCurrentCoverages
    RemoveCurrentCoverages
End Enum