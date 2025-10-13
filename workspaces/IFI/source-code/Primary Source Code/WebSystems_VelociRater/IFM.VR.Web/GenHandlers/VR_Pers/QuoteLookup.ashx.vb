Imports System.Web
Imports System.Web.Services
Imports QuickQuote.CommonMethods
Imports System.Web.Script.Serialization

Public Class QuoteLookup
    Inherits VRGenericHandlerBase



    Overrides Sub ProcessRequest(ByVal context As HttpContext)
        context.Response.Clear()
        context.Response.ContentType = "application/json"

        If context.Request.QueryString("agencyId") IsNot Nothing Then
            If UserHasAgencyIdAccess(context.Request.QueryString("agencyId")) Then ' this protects the searches from agency to agency
                Dim pageIndex As Int32 = 0
                If context.Request.QueryString("pageIndex") IsNot Nothing Then
                    Int32.TryParse(context.Request.QueryString("pageIndex"), pageIndex)
                End If

                Dim perPageCount As Int32 = 0
                If context.Request.QueryString("perPageCount") IsNot Nothing Then
                    Int32.TryParse(context.Request.QueryString("perPageCount"), perPageCount)
                End If

                Dim sortColumn As String = ""
                If context.Request.QueryString("sortColumn") IsNot Nothing Then
                    sortColumn = context.Request.QueryString("sortColumn").Trim()
                End If

                Dim sortOrderDesc As Boolean = False
                If context.Request.QueryString("sortOrderDesc") IsNot Nothing Then
                    sortOrderDesc = context.Request.QueryString("sortOrderDesc").Trim().ToLower() = "true" Or context.Request.QueryString("sortOrderDesc") = "1"
                End If

                Dim quoteId As Int32 = 0
                If context.Request.QueryString("quoteId") IsNot Nothing Then
                    Int32.TryParse(context.Request.QueryString("quoteId"), quoteId)
                End If

                Dim agentUserName As String = ""
                If context.Request.QueryString("agentUserName") IsNot Nothing Then
                    agentUserName = context.Request.QueryString("agentUserName").Trim()
                End If

                Dim agencyId As Int32 = 0
                If context.Request.QueryString("agencyId") IsNot Nothing Then
                    Int32.TryParse(context.Request.QueryString("agencyId"), agencyId)
                End If

                Dim quoteNumber As String = ""
                If context.Request.QueryString("quoteNumber") IsNot Nothing Then
                    quoteNumber = context.Request.QueryString("quoteNumber").Trim()
                End If

                Dim clientName As String = ""
                If context.Request.QueryString("clientName") IsNot Nothing Then
                    clientName = context.Request.QueryString("clientName").Trim()
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

                '7-11-14
                Dim dontShowArchived As Boolean = True
                If context.Request.QueryString("dontShowArchived") IsNot Nothing Then
                    dontShowArchived = context.Request.QueryString("dontShowArchived").Trim()
                End If

                Dim showNDaysofQuotes As Int32 = 0 '10-29-14 Matt A
                If context.Request.QueryString("showNDaysofQuotes") IsNot Nothing Then
                    showNDaysofQuotes = context.Request.QueryString("showNDaysofQuotes").Trim()
                End If

                context.Response.Write(GetjSon(PerformSearch(pageIndex, perPageCount, sortColumn, sortOrderDesc, quoteId, agentUserName, agencyId, quoteNumber, clientName, statusIds, lobIds, excludeLobIds, dontShowArchived, showNDaysofQuotes)))
            End If
        End If

        context.Response.End()

    End Sub



    Private Function PerformSearch(pageIndex As Int32, PerPageCount As Int32, sortByColumn As String, sortDesc As Boolean,
                                   quoteid As Int32, agentUserName As String, agencyId As Int32, quotenumber As String, clientName As String,
                                   statusIds As String, LobIds As String, ExcludeLobIds As String, dontShowArchived As Boolean, showNDaysOfQuotes As Int32) As QuoteResultSet
        Dim resultSet As New QuoteResultSet()

        Dim results As New List(Of QuoteLookupSearch)
        Dim qqHelper As New QuickQuoteHelperClass

        Dim r = VR.Common.QuoteSearch.QuoteSearch.SearchForQuotes(quoteid, agentUserName, agencyId, quotenumber, clientName, statusIds, LobIds, ExcludeLobIds, Me.IsStaff, dontShowArchived, showNDaysOfQuotes, True, qqHelper.CanUserAccessEmployeePolicies(), CInt(System.Web.HttpContext.Current.Session("DiamondUserId")))
        'Dim r = VR.Common.QuoteSearch.QuoteSearch.SearchForQuotes(-1, "", agencyId, "", term, Nothing, Nothing, Nothing, Me.IsStaff)
        If r.Any() Then
            Dim resultCount As Int32 = 0
            'get result stats
            resultSet.PageIndex = pageIndex
            resultCount = r.Count
            resultSet.QueryFullCount = resultCount
            resultSet.QueryRangeStart = pageIndex * PerPageCount
            Dim maxResultNumber As Int32 = (pageIndex + 1) * PerPageCount

            If (maxResultNumber > resultCount) Then
                ' there are fewer than a full page set of results
                resultSet.QueryRangeEnd = resultCount
            Else
                ' there are more than a full page set of results
                resultSet.QueryRangeEnd = maxResultNumber
            End If

            ' do column sort

            'convert result type
            Select Case sortByColumn.ToLower().Trim()
                Case "premium"
                    If sortDesc = False Then
                        r = (From i In r Order By i.Premium Ascending Select i).ToList()
                    Else
                        r = (From i In r Order By i.Premium Descending Select i).ToList()
                    End If
                Case "quote id"
                    If sortDesc = False Then
                        r = (From i In r Order By i.QuoteId Ascending Select i).ToList()
                    Else
                        r = (From i In r Order By i.QuoteId Descending Select i).ToList()
                    End If
                Case "quote #"
                    If sortDesc = False Then
                        r = (From i In r Order By i.QuoteNumber Ascending Select i).ToList()
                    Else
                        r = (From i In r Order By i.QuoteNumber Descending Select i).ToList()
                    End If
                Case "customer"
                    If sortDesc = False Then
                        r = (From i In r Order By i.ClientName Ascending Select i).ToList()
                    Else
                        r = (From i In r Order By i.ClientName Descending Select i).ToList()
                    End If
                Case "status"
                    If sortDesc = False Then
                        r = (From i In r Order By i.FriendlyStatus Ascending Select i).ToList()
                    Else
                        r = (From i In r Order By i.FriendlyStatus Descending Select i).ToList()
                    End If
                Case "last modified"
                    If sortDesc = False Then
                        r = (From i In r Order By i.LastModified Ascending Select i).ToList()
                    Else
                        r = (From i In r Order By i.LastModified Descending Select i).ToList()
                    End If
                Case "last modified user"
                    If sortDesc = False Then
                        r = (From i In r Order By i.LastModifiedByUsername Ascending Select i).ToList()
                    Else
                        r = (From i In r Order By i.LastModifiedByUsername Descending Select i).ToList()
                    End If
                Case "line of business"
                    If sortDesc = False Then
                        r = (From i In r Order By i.LobName Ascending Select i).ToList()
                    Else
                        r = (From i In r Order By i.LobName Descending Select i).ToList()
                    End If
            End Select

            For Each result In r.GetRange(resultSet.QueryRangeStart, resultSet.QueryRangeEnd - resultSet.QueryRangeStart)
                Dim qs As New QuoteLookupSearch()
                resultSet.Results.Add(qs.FromQQSearchResult(result))
            Next

        End If

        Return resultSet
    End Function

End Class

<Serializable()>
Public Class QuoteResultSet

    Public Property Results As New List(Of QuoteLookupSearch)
    Public Property PageIndex As Int32
    Public Property QueryFullCount As Int32
    Public Property QueryRangeStart As Int32
    Public Property QueryRangeEnd As Int32

End Class

<Serializable()>
Public Class ValuePair_js
    Public Property label As String
    Public Property value As String

    Public Sub New()

    End Sub

    Public Sub New(label As String, value As String)

        Me.label = label
        Me.value = value
    End Sub
End Class

<Serializable()>
Public Class QuoteLookupSearch

    Public Property AvilableActions As New List(Of ValuePair_js)
    Public Property QuoteId As Int32
    Public Property QuoteNumber As String
    Public Property QuoteNumAndDescription As String
    Public Property Description As String
    Public Property ClientName As String
    Public Property AgencyCode As String
    Public Property LastModified As String
    Public Property Premium As Double
    Public Property FormatedPremium As String
    Public Property LobName As String
    Public Property LobId As Int32
    Public Property LastModifiedByUsername As String
    Public Property LastModifiedByaIfmStaffMember As Boolean
    Public Property StatusId As Int32
    Public Property FriendlyStatus As String
    Public Property LastModified_FriendlyDate As String
    Public Property ViewUrl As String
    Public Property IsCurrentlyInVr As Boolean
    Public Property OrginatedInVr As Boolean
    Public Property WasLastModifiedInVR As Boolean

    Public Function FromQQSearchResult(result As Common.QuoteSearch.QQSearchResult) As QuoteLookupSearch
        Dim x As New QuoteLookupSearch()
        x.AgencyCode = result.AgencyCode
        If result.AvilableActions.Any() Then
            For Each vp In result.AvilableActions
                x.AvilableActions.Add(New ValuePair_js(vp.Key, vp.Value))
            Next
        End If

        x.ClientName = result.ClientName
        x.Description = result.Description
        x.FormatedPremium = result.FormatedPremium
        x.FriendlyStatus = result.FriendlyStatus
        x.LastModified = result.LastModified.ToString()
        x.LastModifiedByaIfmStaffMember = result.LastModifiedByaIfmStaffMember
        x.LastModifiedByUsername = result.LastModifiedByUsername
        x.LobId = result.LobId
        x.LobName = result.LobName
        x.Premium = result.Premium
        x.QuoteId = result.QuoteId
        x.QuoteNumAndDescription = result.QuoteNumAndDescription
        x.QuoteNumber = result.QuoteNumber
        x.StatusId = result.StatusId
        x.LastModified_FriendlyDate = result.LastModified_FriendlyDate
        x.ViewUrl = result.ViewUrl
        x.IsCurrentlyInVr = result.IsCurrentlyInVR
        x.OrginatedInVr = result.OriginatedInVR
        x.WasLastModifiedInVR = result.WasLastModifiedInVR
        Return x
    End Function

End Class