Imports System.Web
Imports System.Web.Services

Public Class CPPCrimeClassCodeLookup
    Inherits VRGenericHandlerBase

    Overrides Sub ProcessRequest(ByVal context As HttpContext)
        context.Response.Clear()
        context.Response.ContentType = "application/json"
        If context.Request.QueryString("query") IsNot Nothing Then
            If context.Request.QueryString("ClassCode") IsNot Nothing AndAlso context.Request.QueryString("SearchText") IsNot Nothing Then
                Dim ClassCode As String = context.Request.QueryString("ClassCode")
                Dim SearchText As String = context.Request.QueryString("SearchText")

                ' CF = Property
                ' CR = Crime
                context.Response.Write(GetjSon(IFM.VR.Common.Helpers.CPP.CPPCrimeClassCodeHelper.GetClassCodes(ClassCode, SearchText, "CR", "0")))
            End If
        End If
        context.Response.End()
    End Sub

End Class