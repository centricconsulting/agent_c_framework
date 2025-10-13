Imports System.Web
Imports System.Web.Services
Imports System.Web.Script.Serialization
Imports QuickQuote.CommonMethods

Public Class ClientNameAutoComplete
    Inherits VRGenericHandlerBase


    Class AutoCompleteEntry
        Public Property id As Int32
        Public Property label As String
        Public Property value As String

        Public Sub New()

        End Sub

        Public Sub New(id As String, label As String, value As String)
            Me.id = id
            Me.label = label
            Me.value = value
        End Sub
    End Class

    Overrides Sub ProcessRequest(ByVal context As HttpContext)
        context.Response.Clear()
        context.Response.ContentType = "application/json"
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache)

        Dim searchParm As String = ""
        If context.Request.QueryString("term") IsNot Nothing Then
            searchParm = context.Request.QueryString("term")
        End If

        Dim autoCompleteList As List(Of AutoCompleteEntry) = Nothing
        If context.Request.QueryString("agencyId") IsNot Nothing Then
            If UserHasAgencyIdAccess(context.Request.QueryString("agencyId")) Then ' this protects the searches from agency to agency
                If context.Request.QueryString("searchQuotes") IsNot Nothing Then
                    ' looking for quotes that start with 'term'
                    autoCompleteList = GetQuoteNumResults(searchParm, context.Request.QueryString("agencyId"))
                    If autoCompleteList.Count > 20 Then
                        autoCompleteList = autoCompleteList.GetRange(0, 20)
                        autoCompleteList.Add(New AutoCompleteEntry("0", "[Limited to first 20 results]", ""))
                    End If
                    context.Response.Write(GetjSon(autoCompleteList))
                Else
                    ' looking for full client info
                    If context.Request.QueryString("miniClientSearch") IsNot Nothing Then
                        Dim json As String = ""
                        If context.Request.QueryString("businessname") Is Nothing Then
                            Dim results = VR.Common.QuoteSearch.ClientSearch.SearchClients(Common.QuoteSearch.ClientSearch.ClientSearchType.personal, context.Request.QueryString("agencyId"), "", context.Request.QueryString("FirstName"), context.Request.QueryString("LastName"), context.Request.QueryString("City"), context.Request.QueryString("Zip"), context.Request.QueryString("SSN"))

                            If results.Count > 10 Then
                                ' limited to 10 results
                                json = GetjSon(results.GetRange(0, 10))
                            Else
                                json = GetjSon(results)
                            End If
                        Else
                            Dim results = VR.Common.QuoteSearch.ClientSearch.SearchClients(Common.QuoteSearch.ClientSearch.ClientSearchType.commercial, context.Request.QueryString("agencyId"), "", context.Request.QueryString("businessname"), "", context.Request.QueryString("City"), context.Request.QueryString("Zip"), context.Request.QueryString("SSN"))

                            If results.Count > 10 Then
                                ' limited to 10 results
                                json = GetjSon(results.GetRange(0, 10))
                            Else
                                json = GetjSon(results)
                            End If
                        End If

                        context.Response.Write(json)
                    Else
                        ' simple client name search - in this case you only want them it quote exists
                        autoCompleteList = GetExistingPolicyClientsResults(searchParm, context.Request.QueryString("agencyId"))
                        If autoCompleteList.Count > 15 Then
                            autoCompleteList = autoCompleteList.GetRange(0, 15)
                            autoCompleteList.Add(New AutoCompleteEntry("0", "[Limited to first 15 results]", ""))
                        End If
                        context.Response.Write(GetjSon(autoCompleteList))
                    End If

                End If
            End If
        End If

        context.Response.End()
    End Sub


    Private Function GetExistingPolicyClientsResults(term As String, agencyId As Int32) As List(Of AutoCompleteEntry)
        Dim autoEntries As New List(Of AutoCompleteEntry)
        Dim qqHelper As New QuickQuoteHelperClass
        If Me.UserHasAgencyIdAccess(agencyId) Then
            Dim lobsExclude As String = ""

            ' The first would get more results but it would give you names that no existing quote exists for in VR
            'Dim results = (From i As QuickQuote.CommonObjects.QuickQuoteClient In VR.Common.QuoteSearch.ClientSearch.SearchClients(agencyId, term, "", "", "", "", "") Select i.Name.DisplayName).Distinct()
            ' this one you only get results from current Vr quotes
            Dim results = (From i In VR.Common.QuoteSearch.QuoteSearch.SearchForQuotes(-1, "", agencyId, "", term, Nothing, Nothing, Nothing, Me.IsStaff, False, 0, True, qqHelper.CanUserAccessEmployeePolicies(), CInt(System.Web.HttpContext.Current.Session("DiamondUserId"))) Select i.ClientName.ToUpper()).Distinct()

            Dim index As Int32 = 1
            For Each result As String In results
                autoEntries.Add(New AutoCompleteEntry(index.ToString(), result, ""))
                index += 1
            Next
        End If
        Return autoEntries
    End Function

    Private Function GetQuoteNumResults(term As String, agencyId As Int32) As List(Of AutoCompleteEntry)
        Dim qqHelper As New QuickQuoteHelperClass
        Dim autoEntries As New List(Of AutoCompleteEntry)
        If Me.UserHasAgencyIdAccess(agencyId) Then
            Dim isStaff As Boolean = False
            Dim lobsExclude As String = ""

            Dim results = (From i In VR.Common.QuoteSearch.QuoteSearch.SearchForQuotes(-1, "", agencyId, term + "%", "", Nothing, Nothing, Nothing, isStaff, False, 0, True, qqHelper.CanUserAccessEmployeePolicies(), CInt(System.Web.HttpContext.Current.Session("DiamondUserId"))) Select i.QuoteNumber.ToUpper()).Distinct()

            Dim index As Int32 = 1
            For Each result As String In results
                autoEntries.Add(New AutoCompleteEntry(index.ToString(), result, ""))
                index += 1
            Next
        End If
        Return autoEntries
    End Function


End Class