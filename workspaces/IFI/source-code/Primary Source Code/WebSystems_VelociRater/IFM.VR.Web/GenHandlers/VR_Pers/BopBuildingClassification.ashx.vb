Imports System.Web
Imports System.Web.Services

Public Class BopBuildingClassification
    Inherits VRGenericHandlerBase

    Public Overrides Sub ProcessRequest(context As HttpContext)
        context.Response.Clear()
        context.Response.ContentType = "application/json"
        If context.Request.QueryString("programType") IsNot Nothing Then
            Dim programTypeId As String = context.Request.QueryString("programType")
            context.Response.Write(GetjSon(IFM.VR.Common.Helpers.BOP.ClassificationHelper.GetClassificationsForProgramTypeId(programTypeId)))
        End If

        context.Response.End()
    End Sub


End Class