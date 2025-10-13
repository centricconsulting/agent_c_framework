
Partial Class controls_Proposal_VRProposal_Terrorism_notice
    Inherits System.Web.UI.UserControl

    Public Property Logo As String
        Get
            Return Me.lblLogo.Text
        End Get
        Set(value As String)
            Me.lblLogo.Text = value
        End Set
    End Property

    Protected Sub controls_Proposal_VRProposal_Terrorism_notice_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Page.IsPostBack = False Then
            If ConfigurationManager.AppSettings("QuickQuote_Proposal_MainPageLogo") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_Proposal_MainPageLogo").ToString <> "" Then
                Me.MainPageLogo.Src = ConfigurationManager.AppSettings("QuickQuote_Proposal_MainPageLogo").ToString
                Me.MainPageLogo.Width = 400
                Me.MainPageLogo.Align = "left"
            End If
                      
        End If
    End Sub
End Class
