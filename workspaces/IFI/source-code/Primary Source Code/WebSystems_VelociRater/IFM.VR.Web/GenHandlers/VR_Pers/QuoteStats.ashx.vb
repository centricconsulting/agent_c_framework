Imports System.Web
Imports System.Web.Services
Imports QuickQuote.CommonMethods
Imports System.Web.Script.Serialization

Public Class QuoteStats
    Inherits VRGenericHandlerBase



    Overrides Sub ProcessRequest(ByVal context As HttpContext)
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache)
        context.Response.Clear()
        context.Response.ContentType = "application/json"

        If context.Request.QueryString("agencyId") IsNot Nothing Then
            If UserHasAgencyIdAccess(context.Request.QueryString("agencyId")) Then ' this protects the searches from agency to agency

                Dim startDate As String = ""
                If context.Request.QueryString("startDate") IsNot Nothing Then
                    startDate = context.Request.QueryString("startDate").Trim()
                End If

                Dim endDate As String = ""
                If context.Request.QueryString("endDate") IsNot Nothing Then
                    endDate = context.Request.QueryString("endDate").Trim()
                End If

                Dim agencyId As Int32 = 0
                If context.Request.QueryString("agencyId") IsNot Nothing Then
                    Int32.TryParse(context.Request.QueryString("agencyId"), agencyId)
                End If

                Dim statusIds As String = ""
                If context.Request.QueryString("statusIds") IsNot Nothing Then
                    statusIds = context.Request.QueryString("statusIds").Trim()
                End If

                Dim lobIds As String = ""
                If context.Request.QueryString("lobIds") IsNot Nothing Then
                    lobIds = context.Request.QueryString("lobIds").Trim()
                End If

                Dim excludeLobIds As String = ""
                If context.Request.QueryString("excludeLobIds") IsNot Nothing Then
                    excludeLobIds = context.Request.QueryString("excludeLobIds").Trim()
                End If

                Dim results = IFM.VR.Common.QuoteSearch.QuotingStats.GetStats(CDate(startDate), CDate(endDate), agencyId, lobIds, IsStaff)
                context.Response.Write(GetjSon(results))
            End If
        End If

        context.Response.End()
    End Sub



End Class