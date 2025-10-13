Imports System.Web
Imports System.Web.Services

Public Class NaicsCodeLookup
    Inherits VRGenericHandlerBase

    Overrides Sub ProcessRequest(ByVal context As HttpContext)
        context.Response.Clear()
        context.Response.ContentType = "application/json"

        If context.Request.QueryString("query") IsNot Nothing Then

            If context.Request.QueryString("SearchType") IsNot Nothing AndAlso context.Request.QueryString("SearchText") IsNot Nothing Then

                Dim SearchType As String = context.Request.QueryString("SearchType")
                Dim SearchText As String = context.Request.QueryString("SearchText")


                context.Response.Write(GetjSon(IFM.VR.Common.Helpers.BOP.NaicsCodeHelper.GetNaicsCodes(SearchType, SearchText)))
            End If

        End If

        context.Response.End()
    End Sub

End Class