Imports System.Reflection
Imports Microsoft.Extensions.DependencyInjection

Public Class ServiceProvider
    Implements IServiceProvider

    Private ReadOnly _serviceProvider As IServiceProvider

    Public Sub New(serviceProvider As IServiceProvider)
        _serviceProvider = serviceProvider
    End Sub

    Public Function GetService(serviceType As Type) As Object Implements IServiceProvider.GetService
        Try
            Dim lifetimeScope As IServiceScope
            Dim currentHttpContext = HttpContext.Current
            If currentHttpContext IsNot Nothing Then

                lifetimeScope = CType(currentHttpContext.Items(GetType(IServiceScope)), IServiceScope)
                If lifetimeScope Is Nothing Then

                    Dim CleanScope As EventHandler =
                        Sub(sender As Object, args As EventArgs)
                            If TypeOf sender Is HttpApplication Then
                                Dim application As HttpApplication = sender
                                RemoveHandler application.RequestCompleted, CleanScope
                                lifetimeScope.Dispose()
                            End If
                        End Sub

                    lifetimeScope = _serviceProvider.CreateScope()
                    currentHttpContext.Items.Add(GetType(IServiceScope), lifetimeScope)
                    AddHandler currentHttpContext.ApplicationInstance.RequestCompleted, CleanScope
                End If
            Else
                lifetimeScope = _serviceProvider.CreateScope()
            End If

            Return ActivatorUtilities.GetServiceOrCreateInstance(lifetimeScope.ServiceProvider, serviceType)
        Catch ex As InvalidOperationException

            'No public ctor available, revert to a private/internal one
            Return Activator.CreateInstance(serviceType, BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.CreateInstance, Nothing, Nothing, Nothing)
        End Try
    End Function

End Class
