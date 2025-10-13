
Partial Class controls_Proposal_VRProposal_Footer
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Page.IsPostBack = False Then
            If ConfigurationManager.AppSettings("QuickQuote_Proposal_FooterLogo") IsNot Nothing AndAlso
                ConfigurationManager.AppSettings("QuickQuote_Proposal_FooterLogo").ToString <> "" Then
                Me.FooterLogo.Src = ConfigurationManager.AppSettings("QuickQuote_Proposal_FooterLogo").ToString
            Else
                Me.FooterLogo.Src = "https://www.indianafarmers.com/agentsonly/images/Color Quote Proposal Logo 11-18.jpg"
            End If
        End If
    End Sub

End Class
