Imports System.Web
Imports System.Web.Services

Public Class ReverseVehicleClassCodeLookup
    Inherits VRGenericHandlerBase

    Overrides Sub ProcessRequest(ByVal context As HttpContext)
        context.Response.Clear()
        context.Response.ContentType = "application/json"
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache)

        ' Class Code
        Dim ClassCode As String = ""
        If context.Request.QueryString("ClassCode") IsNot Nothing Then
            ClassCode = context.Request.QueryString("ClassCode").Trim
        End If

        context.Response.Write(GetjSon(IFM.VR.Common.Helpers.VehicleClassCodeLookup.ReverseClassCodeLookup(ClassCode)))

        context.Response.End()

    End Sub

End Class