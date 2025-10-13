Imports System.Configuration.ConfigurationManager

Public Class VRTest_UW
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Private Sub VRTest_Farm_PreInit(sender As Object, e As EventArgs) Handles Me.PreInit
        Me.MasterPageFile = "Velocirater.master"
    End Sub
End Class