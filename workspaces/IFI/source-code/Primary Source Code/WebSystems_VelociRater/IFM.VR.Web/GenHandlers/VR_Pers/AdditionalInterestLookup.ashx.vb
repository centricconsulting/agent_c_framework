Imports System.Web
Imports System.Web.Services
Imports QuickQuote.CommonMethods
Imports System.Web.Script.Serialization

Public Class AdditionalInterestLookup
    Inherits VRGenericHandlerBase


    Overrides Sub ProcessRequest(ByVal context As HttpContext)

        context.Response.Clear()
        context.Response.ContentType = "application/json"

        Dim commName As String = Nothing
        Dim firstName As String = Nothing
        Dim middleName As String = Nothing
        Dim lastName As String = Nothing

        Dim quoteId As String = Nothing

        If context.Request.QueryString("commercialName") IsNot Nothing Then
            commName = context.Request.QueryString("commercialName")
        End If

        If context.Request.QueryString("firstName") IsNot Nothing Then
            firstName = context.Request.QueryString("firstName")
        End If

        If context.Request.QueryString("middleName") IsNot Nothing Then
            middleName = context.Request.QueryString("middleName")
        End If

        If context.Request.QueryString("lastName") IsNot Nothing Then
            lastName = context.Request.QueryString("lastName")
        End If

        If context.Request.QueryString("quoteId") IsNot Nothing Then '2/16/2021 note: should already be handling for Endorsements - quoteId will be policyId|policyImageNum in that case
            quoteId = context.Request.QueryString("quoteId")
        End If

        If context.Request.QueryString("agencyId") IsNot Nothing Then
            If UserHasAgencyIdAccess(context.Request.QueryString("agencyId")) And Integer.TryParse(context.Request.QueryString("agencyId"), Nothing) Then ' this protects the searches from agency to agency

                ' i need to set the is editable property so the form will know to allow edits or not
                Dim aiCreatedInSession = AdditionalInterstIdsCreatedThisSession(context, quoteId)
                Dim results = VR.Common.Helpers.AdditionalInterest.GetAdditionalInterestByName(commName, firstName, middleName, lastName, CInt(context.Request.QueryString("agencyId")))
                For Each r In results
                    r.IsEditable = aiCreatedInSession.Contains(r.Id)
                Next
                context.Response.Write(GetjSon(results))
            End If
        End If
    End Sub

    Public Function AdditionalInterstIdsCreatedThisSession(ByVal context As HttpContext, quoteId As String) As List(Of Int32) '2/16/2021 note: should already be handling for Endorsements - quoteId will be policyId|policyImageNum in that case
        If context.Session(quoteId + "_AI_Created_List") Is Nothing Then
            context.Session(quoteId + "_AI_Created_List") = New List(Of Int32)
        End If

        Return context.Session(quoteId + "_AI_Created_List")
    End Function



End Class