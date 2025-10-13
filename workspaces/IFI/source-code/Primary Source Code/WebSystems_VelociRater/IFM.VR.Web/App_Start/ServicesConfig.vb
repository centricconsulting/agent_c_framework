Imports System.Runtime.CompilerServices
Imports IFM.ControlFlags
Imports IFM.VR.Common.Underwriting
Imports IFM.VR.Flags
Imports Microsoft.Extensions.Caching.Memory
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.FileProviders

'<Assembly: PreApplicationStartMethod(GetType(ServicesConfig), NameOf(ServicesConfig.Init))>
Public Module ServicesConfig
    Public Sub RegisterServices(serviceCollection As IServiceCollection)
        serviceCollection.AddMemoryCache() _
                         .AddLocalFileProvider() _
                         .AddSingleton(Of IFeatureFlag, LOB.PPA) _
                         .AddSingleton(Of IUnderwritingQuestionsService, UnderwritingQuestionsService) _
                         .AddTransient(Of QuickQuote.CommonMethods.QuickQuoteHelperClass)(Function(sp) New QuickQuote.CommonMethods.QuickQuoteHelperClass())
    End Sub

    'extension methods to configure and add custom services or dependencies
    <Extension()>
    Public Function AddLocalFileProvider(serviceCollection As IServiceCollection) As IServiceCollection

        serviceCollection.AddSingleton(Of IFileProvider)(New PhysicalFileProvider(AppDomain.CurrentDomain.BaseDirectory()))

        Return serviceCollection
    End Function


    Public Sub Init()
        'This is needed for how VR runs with AgentPort
        HttpApplication.RegisterModule(GetType(InterceptModule))
    End Sub
End Module

Public Class InterceptModule
    Implements IHttpModule

    Private _application As HttpApplication

    Public Sub New()
    End Sub

    Public Sub Init(context As HttpApplication) Implements IHttpModule.Init
        _application = context

        If New IFM.VR.Flags.App.DependencyInjection().UseMEDI Then
            RegisterServices(_application.AddServiceCollection())
        End If

    End Sub

    Public Sub Dispose() Implements IHttpModule.Dispose

    End Sub

    Public Sub RegisterServices(serviceCollection As IServiceCollection)
        serviceCollection.AddMemoryCache() _
                             .AddSingleton(Of IFeatureFlag, LOB.PPA) _
                             .AddSingleton(Of IUnderwritingQuestionsService, UnderwritingQuestionsService) _
                             .AddTransient(Of QuickQuote.CommonMethods.QuickQuoteHelperClass)(Function(sp) New QuickQuote.CommonMethods.QuickQuoteHelperClass())
        AddLocalFileProvider(serviceCollection)
    End Sub

    Public Function AddLocalFileProvider(serviceCollection As IServiceCollection) As IServiceCollection

        serviceCollection.AddSingleton(Of IFileProvider)(New PhysicalFileProvider(AppDomain.CurrentDomain.BaseDirectory()))

        Return serviceCollection
    End Function

End Class