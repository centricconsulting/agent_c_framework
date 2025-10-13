Imports System.Web
Imports System.Web.Services
Imports QuickQuote.CommonMethods
Imports System.Web.Script.Serialization

Public Class ApplicantLookup
    Inherits VRGenericHandlerBase



    Overrides Sub ProcessRequest(ByVal context As HttpContext)
        context.Response.Clear()
        context.Response.ContentType = "application/json"
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache)

        If context.Request.QueryString("agencyId") IsNot Nothing Then
            If UserHasAgencyIdAccess(context.Request.QueryString("agencyId")) Then ' this protects the searches from agency to agency

                Dim agencyId As String = context.Request.QueryString("agencyId")

                Dim firstName As String = ""
                If context.Request.QueryString("firstName") IsNot Nothing Then
                    firstName = context.Request.QueryString("firstName")
                End If

                Dim lastName As String = ""
                If context.Request.QueryString("lastName") IsNot Nothing Then
                    lastName = context.Request.QueryString("lastName")
                End If

                Dim zip As String = ""
                If context.Request.QueryString("zip") IsNot Nothing Then
                    zip = context.Request.QueryString("zip")
                End If

                Dim ssn As String = ""
                If context.Request.QueryString("ssn") IsNot Nothing Then
                    ssn = context.Request.QueryString("ssn")
                End If

                context.Response.Write(GetjSon(IFM.VR.Common.ApplicantSearch.ApplicantSearch.SearchApplicants(agencyId, firstName, lastName, zip, ssn)))

            End If
        End If

        context.Response.Write("")

    End Sub



End Class