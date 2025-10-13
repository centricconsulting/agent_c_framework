Imports System.Web.SessionState

Public Class Global_asax 'added 12/15/2020
    Inherits System.Web.HttpApplication

    Public Overrides Sub Init()
    End Sub
    Public Shared Sub App_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the application is started
        'added 12/15/2020 for Diamond Product Extensions
        AddHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf Diamond.Common.Utility.ObjectCreator.UIAssemblyResolve
        AddHandler AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve, AddressOf Diamond.Common.Utility.ObjectCreator.UIAssemblyResolve


    End Sub
    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        App_Start(sender, e)
    End Sub

    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the session is started
    End Sub

    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires at the beginning of each request
    End Sub

    Sub Application_AuthenticateRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires upon attempting to authenticate the use
    End Sub

    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when an error occurs
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the session ends
    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the application ends
    End Sub

End Class