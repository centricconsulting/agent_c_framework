
Partial Class controls_Proposal_VRProposal_AboutUs
    Inherits System.Web.UI.UserControl


    Protected Sub controls_Proposal_VRProposal_AboutUs_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Page.IsPostBack = False Then
        Me.AboutHeader.Height = 250
        End If
    End Sub
End Class
