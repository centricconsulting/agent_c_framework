Imports System.Web
Imports System.Web.Services

Public Class CommRiskGrades
    Inherits VRGenericHandlerBase

    Overrides Sub ProcessRequest(ByVal context As HttpContext)
        context.Response.Clear()
        context.Response.ContentType = "application/json"

        If context.Request.QueryString("query") IsNot Nothing Then


            If context.Request.QueryString("riskTypeId") IsNot Nothing AndAlso context.Request.QueryString("searchText") IsNot Nothing AndAlso context.Request.QueryString("versionid") IsNot Nothing Then

                Dim riskTypeId As String = context.Request.QueryString("riskTypeId")
                Dim searchText As String = context.Request.QueryString("searchText")
                Dim versionID As String = context.Request.QueryString("versionid")

                context.Response.Write(GetjSon(IFM.VR.Common.Helpers.CGL.CommRiskGradeHelper.GetRiskCodes(riskTypeId, searchText, versionID)))
            End If

        End If






        context.Response.End()
    End Sub

End Class