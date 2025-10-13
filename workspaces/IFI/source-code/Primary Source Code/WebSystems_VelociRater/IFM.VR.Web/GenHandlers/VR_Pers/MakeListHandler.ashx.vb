Imports System.Web
Imports System.Web.Services

Public Class MakeListHandler
    Inherits VRGenericHandlerBase

    Overrides Sub ProcessRequest(ByVal context As HttpContext)
        context.Response.Clear()
        context.Response.ContentType = "application/json"

        Dim Year As String = ""

        If context.Request.QueryString("Year") IsNot Nothing Then
            Year = context.Request.QueryString("Year")
        End If

        If Int32.TryParse(Year, Nothing) = False Then
            Year = "0"
        End If

        context.Response.Write(GetjSon(IFM.VR.Common.Helpers.PPA.YMMLookup.GetMakeListFromYear(Year)))
        context.Response.End()
    End Sub
End Class