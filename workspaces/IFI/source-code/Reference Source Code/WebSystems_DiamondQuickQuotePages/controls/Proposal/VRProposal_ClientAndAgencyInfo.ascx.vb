
Partial Class controls_Proposal_VRProposal_ClientAndAgencyInfo
    Inherits System.Web.UI.UserControl

    Public Property ClientInfo As String
        Get
            Return Me.lblClientInfo.Text
        End Get
        Set(value As String)
            Me.lblClientInfo.Text = value
        End Set
    End Property
    Public Property AgencyInfo As String
        Get
            Return Me.lblAgencyInfo.Text
        End Get
        Set(value As String)
            Me.lblAgencyInfo.Text = value
        End Set
    End Property


    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Page.IsPostBack = False Then

        End If
    End Sub
End Class
