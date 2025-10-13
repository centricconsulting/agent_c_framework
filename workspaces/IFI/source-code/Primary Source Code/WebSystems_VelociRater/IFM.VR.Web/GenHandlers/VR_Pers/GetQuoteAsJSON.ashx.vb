Imports System.Web
Imports System.Web.Services
Imports QuickQuote.CommonMethods
Imports System.Web.Script.Serialization

Public Class GetQuoteAsJSON
    Inherits VRGenericHandlerBase


    Overrides Sub ProcessRequest(ByVal context As HttpContext)
        Dim qqHelper As New QuickQuoteHelperClass
        context.Response.Clear()
        context.Response.ContentType = "application/json"

        If context.Request.QueryString("agencyId") IsNot Nothing Then
            If UserHasAgencyIdAccess(context.Request.QueryString("agencyId")) Then ' this protects the searches from agency to agency
                If context.Request.QueryString("quoteId") IsNot Nothing Then
                    Dim result = VR.Common.QuoteSearch.QuoteSearch.SearchForQuotes(context.Request.QueryString("quoteId"), "", context.Request.QueryString("agencyId"), "", "", "", "", "", Me.IsStaff, True, 0, False, qqHelper.CanUserAccessEmployeePolicies(), CInt(System.Web.HttpContext.Current.Session("DiamondUserId"))).FirstOrDefault()
                    If result IsNot Nothing Then
                        context.Response.Write(result.GetQuoteObjectAsJSON)
                    End If
                End If
            End If
        End If
        context.Response.End()

    End Sub



End Class