Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonObjects

Public Class ctlRecentlyManufacturedVehicle_APP
    Inherits VRControlBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
    End Sub

    Public Overrides Function Save() As Boolean
        Return True
    End Function

    Protected Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        ResetAndHidePopup()
    End Sub

    Public Sub CheckForRecentModels(valArgs As VRValidationArgs)

    End Sub

    Public Sub ShowPopup()
        If (Not IsQuoteEndorsement()) AndAlso Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal Then
            Me.Visible = True
            Me.VRScript.CreatePopupForm(Me.divNewVehicalPopup_APP.ClientID, "Loan Lease Gap coverage added", 400, 125, True, True, False, btnOK.ClientID, "")
        End If
    End Sub

    Private Sub ResetAndHidePopup()
        Me.Visible = False
    End Sub

End Class