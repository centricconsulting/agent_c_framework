Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonObjects

Public Class ctlRecentManufacturedVehicle
    Inherits VRControlBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Public Event RecentDateVehicleOK()
    Public Event RecentDateVehicleCancel()

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        If Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal Then
            Me.VRScript.CreatePopupForm(Me.divNewVehicalPopup.ClientID, "Vehicle added with no lienholder", 400, 200, True, True, False, Me.btnOK.ClientID, "")
        End If
    End Sub

    Public Overrides Function Save() As Boolean

    End Function

    Protected Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        ResetAndHidePopup()
        RaiseEvent RecentDateVehicleOK()
    End Sub

    Public Sub CheckForRecentModels(valArgs As VRValidationArgs)

    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        ResetAndHidePopup()
        RaiseEvent RecentDateVehicleCancel()
    End Sub

    Private Sub ResetAndHidePopup()
        Me.Visible = False
    End Sub

End Class

