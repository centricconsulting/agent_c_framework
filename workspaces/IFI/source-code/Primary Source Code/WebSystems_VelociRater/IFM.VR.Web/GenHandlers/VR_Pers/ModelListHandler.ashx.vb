Imports System.Web
Imports System.Web.Services

Public Class ModelListHandler
    Inherits VRGenericHandlerBase

    Overrides Sub ProcessRequest(ByVal context As HttpContext)
        context.Response.Clear()
        context.Response.ContentType = "application/json"

        Dim Year As String = ""
        Dim Make As String = ""
       
        If context.Request.QueryString("Make") IsNot Nothing Then
            Make = context.Request.QueryString("Make").Trim()
        End If

        If context.Request.QueryString("Year") IsNot Nothing Then
            Year = context.Request.QueryString("Year")
        End If

        If Int32.TryParse(Year, Nothing) = False Then
            Year = "0"
        End If

        context.Response.Write(GetjSon(IFM.VR.Common.Helpers.PPA.YMMLookup.GetModelListFromYearMake(Year, Make)))
        context.Response.End()
    End Sub

End Class