Imports QuickQuote.CommonObjects

Public Class ctlMiniClientSearch
    Inherits System.Web.UI.UserControl

    Public ReadOnly Property AgencyId As Int32
        Get
            Return DirectCast(Me.Page.Master, VelociRater).AgencyID
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

End Class