Imports System.Web
Imports System.Web.Services

Public Class CPREQClassificationLookup
    Inherits VRGenericHandlerBase

    Overrides Sub ProcessRequest(ByVal context As HttpContext)
        context.Response.Clear()
        context.Response.ContentType = "application/json"
        If context.Request.QueryString("query") IsNot Nothing Then
            If context.Request.QueryString("SearchId") IsNot Nothing AndAlso context.Request.QueryString("SearchText") IsNot Nothing Then
                Dim SearchId As String = context.Request.QueryString("SearchId")
                Dim SearchText As String = context.Request.QueryString("SearchText")

                context.Response.Write(GetjSon(IFM.VR.Common.Helpers.CPR.CPREQClassificationHelper.GetClassCodes(SearchId, SearchText)))
            End If
        End If
        context.Response.End()
    End Sub

End Class