Public Class ctlWarnSessionExpiring
    Inherits System.Web.UI.UserControl

    Public Property AlwaysShowRemainingTime As String
        Get
            If ViewState("vs_autoLogOut_AlwaysShowTimeRemaining") IsNot Nothing Then
                Return ViewState("vs_autoLogOut_AlwaysShowTimeRemaining").ToLower()
            Else
                Return "false"
            End If

        End Get
        Set(ByVal value As String)
            ViewState("vs_autoLogOut_AlwaysShowTimeRemaining") = value
        End Set
    End Property

    Public ReadOnly Property TimeOut As String
        Get
            If Request.QueryString("testlogout") IsNot Nothing Then
                Return "70"
            Else
                Return ((Session.Timeout * 60) - 120).ToString() ' minus 120 seconds so the session isn't already expired when you show the warning 
            End If
        End Get
    End Property

    Public ReadOnly Property IsDebug As Boolean
        Get
#If DEBUG Then
            Return True
#End If
            Return False
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            If Request.QueryString("testlogout") IsNot Nothing Then
                AlwaysShowRemainingTime = "true"
            Else
                AlwaysShowRemainingTime = "false"
            End If
        End If
    End Sub

End Class