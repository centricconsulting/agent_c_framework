Imports System.Web
Imports System.Web.Services

Public Class RACAHandler
    Inherits VRGenericHandlerBase

    Overrides Sub ProcessRequest(ByVal context As HttpContext)
        context.Response.Clear()
        context.Response.ContentType = "application/json"

        Dim Vin As String = ""

        If context.Request.QueryString("Vin") IsNot Nothing Then
            Vin = context.Request.QueryString("Vin").ToUpper()
            'uses IntegrationHelper from IFM.VR.Common via IntegrationHelper from IFM.VR.Web
            Dim ih As New IntegrationHelper
            context.Response.Write(GetjSon(ih.GetRACASymbols(Vin)))
        End If

        context.Response.End()
    End Sub
End Class