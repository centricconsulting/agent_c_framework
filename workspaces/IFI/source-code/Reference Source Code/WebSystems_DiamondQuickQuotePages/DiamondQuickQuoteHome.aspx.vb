
Partial Class DiamondQuickQuoteHome_QQ
    Inherits System.Web.UI.Page

    Protected Sub Page_PreInit(sender As Object, e As System.EventArgs) Handles Me.PreInit
        Me.MasterPageFile = ConfigurationManager.AppSettings("DiamondQuickQuoteMaster")
    End Sub
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Page.IsPostBack = False Then
            
        End If
    End Sub
End Class
