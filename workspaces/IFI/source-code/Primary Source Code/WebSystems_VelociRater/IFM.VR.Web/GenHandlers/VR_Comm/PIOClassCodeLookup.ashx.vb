Imports System.Web
Imports System.Web.Services

Public Class PIOClassCodeLookup
    Inherits VRGenericHandlerBase

    Overrides Sub ProcessRequest(ByVal context As HttpContext)
        context.Response.Clear()
        context.Response.ContentType = "application/json"

        If context.Request.QueryString("query") IsNot Nothing Then

            If context.Request.QueryString("ClassCode") IsNot Nothing AndAlso context.Request.QueryString("SearchText") IsNot Nothing AndAlso context.Request("StateId") IsNot Nothing AndAlso context.Request("EffectiveDate") IsNot Nothing Then

                Dim ClassCode As String = context.Request.QueryString("ClassCode")
                Dim SearchText As String = context.Request.QueryString("SearchText")
                Dim StateId As String = context.Request.QueryString("StateId")
                Dim EffectiveDate As String = context.Request.QueryString("EffectiveDate")

                Dim CompanyId As String = "1"
                If context.Request.QueryString("CompanyId") IsNot Nothing
                    CompanyId = context.Request.QueryString("CompanyId")
                End If


                context.Response.Write(GetjSon(IFM.VR.Common.Helpers.CPR.PIOClassCodeHelper.GetClassCodes(ClassCode, SearchText, StateId, EffectiveDate, CompanyId)))
            End If

        End If

        context.Response.End()
    End Sub

End Class