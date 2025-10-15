Imports System.IO
Imports System.Xml.Serialization
Imports QuickQuote.CommonMethods
Imports System.Web.Script.Serialization
Imports IFM.VR.Common.Helpers
Imports IFM.PrimativeExtensions
Imports System.Web
Imports System.Data.SqlClient
Imports System.Configuration.ConfigurationManager
Imports Diamond.Business.ThirdParty.ChoicePoint.ThirdParty
Imports Diamond.Business.ThirdParty.tuxml
Imports System.Security.Cryptography

Namespace IFM.VR.Common.QuoteSearch

    Public Class QuoteSearch
        Private Shared qqConn As String = System.Configuration.ConfigurationManager.AppSettings("ConnQQ") ' "Server=ifmdiasqlQA;UID=ifmdsn;PWD=ifmdsn;Initial Catalog=QuickQuote;Max Pool Size=400;"
        Private Shared supportedLobIds As String = System.Configuration.ConfigurationManager.AppSettings("supportedLobIds") '"1,2,9,20,21,23,25,28"

        Public Enum SearchType
            NA = 0
            All = 1
            Quotes = 2
            Changes = 3
            BillingUpdates = 4
            Policies = 5
        End Enum

        Public Enum ItemType
            NA = 0
            Quote = 1
            Policy = 2
            Change = 3
            BillingUpdate = 4
        End Enum

        Public Shared Function UserHasAccessToQuoteId(quoteId As Int32) As Boolean
            ' do database lookup on quoteid
            Dim agencyId As Int32 = 0
            Using conn As New System.Data.SqlClient.SqlConnection(qqConn)
                conn.Open()
                Using cmd As New System.Data.SqlClient.SqlCommand()
                    cmd.Connection = conn
                    cmd.CommandText = "select agencyId from [QuickQuote].[dbo].[Quote] where quoteid = @quoteId;"
                    cmd.Parameters.AddWithValue("@quoteid", quoteId)
                    Using reader As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader()
                        If reader.HasRows Then
                            reader.Read()
                            ' get the agencyId from the lookup
                            agencyId = reader.GetInt32(0)
                        End If
                    End Using
                End Using
            End Using


            Return If(agencyId > 0, QuickQuoteObjectHelper.UserHasAgencyIdAccess(agencyId), False)
        End Function

        Private Shared ReadOnly Property MyAgencyList As List(Of QuickQuote.CommonObjects.QuickQuoteUserAgency)
            Get
                Dim qqHelper As New QuickQuoteHelperClass()
                Return qqHelper.GetUserAgencies(QuickQuoteHelperClass.DiamondUserId()) ' should use this
            End Get
        End Property

        Public Shared Function SearchBySearchParameters(searchParameters As QQSearchParameters) As List(Of QQSearchResult)
            Return SearchCore(searchParameters)
        End Function

        Private Shared Function SearchCore(searchParameters As QQSearchParameters) As List(Of QQSearchResult)
#If DEBUG Then
            'Dim startTime As DateTime = DateTime.Now
            Dim sw As New Stopwatch
            sw.Start()
#End If


            Dim results As New List(Of QQSearchResult)

            If searchParameters.AgencyID <> 0 Then
                Using conn As New System.Data.SqlClient.SqlConnection(qqConn)
                    conn.Open()
                    If System.Configuration.ConfigurationManager.AppSettings("ARITHABORT_ON") = "yes" Then
                        Using arithAbortCmd As New SqlCommand("SET ARITHABORT ON", conn)
                            arithAbortCmd.ExecuteNonQuery()
                        End Using
                    End If

                    Using cmd As New System.Data.SqlClient.SqlCommand()
                        cmd.Connection = conn

                        cmd.CommandText = searchParameters.StoredProcedureName
                        cmd.CommandTimeout = 250
                        cmd.CommandType = CommandType.StoredProcedure

                        BuildParameterList(cmd.Parameters, searchParameters)

                        Using reader As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader()
                            If reader.HasRows Then

                                While reader.Read()

                                    Dim result As New QQSearchResult()

                                    result = GetResult(reader)

                                    results.Add(result)

                                End While
                            End If
                        End Using

                    End Using
                End Using
            End If
#If DEBUG Then
            sw.Stop()
            Debug.WriteLine(String.Format("{0} Lookup Query Time: {1}   LOB IDs: {2}   Results: {3}", searchParameters.SearchType.ToString(), sw.ElapsedMilliseconds, searchParameters.LobIDs, results.Count))
#End If
            Return results
        End Function

        Public Shared Function SearchForAll(searchParameters As Common.QuoteSearch.QQSearchParameters) As List(Of QQSearchResult)
            Return SearchCore(searchParameters)
        End Function

        Public Shared Function SearchForAll(quoteOrPolicyID As Int32, agentUserName As String, agencyId As Int32, quoteNumber As String, clientname As String, statusIDs As String,
                                               lobIDs As String, lobIDsExclude As String, IsStaff As Boolean, neverShowArchived As Boolean,
                                               showNDays As Int32, allowResultsNotCurrentlyInVr As Boolean, HasEmployeeAccess As Boolean, CurrentUserID As Integer, Optional clientId As Int32 = 0,
                                               Optional TopEntriesToDisplay As Integer = 0, Optional agentUserID As Integer = 0,
                                               Optional ExcludedLobIDsForEndorsements As List(Of Integer) = Nothing) As List(Of QQSearchResult)

            Dim searchParameters As New Common.QuoteSearch.QQSearchParameters

            searchParameters.SearchType = SearchType.All
            searchParameters.quoteOrPolicyID = quoteOrPolicyID
            searchParameters.AgentUserName = agentUserName
            searchParameters.AgencyID = agencyId
            searchParameters.QuoteOrPolicyNumber = quoteNumber
            searchParameters.ClientName = clientname
            searchParameters.StatusIDs = statusIDs
            searchParameters.LobIDs = lobIDs
            searchParameters.ExcludedLobIds = lobIDsExclude
            searchParameters.IsStaff = IsStaff
            searchParameters.ShowArchived = Not neverShowArchived
            searchParameters.TimeFrame = showNDays
            searchParameters.AllowResultsNotCurrentlyInVR = allowResultsNotCurrentlyInVr
            searchParameters.HasEmployeeAccess = HasEmployeeAccess
            searchParameters.CurrentDiamondUserID = CurrentUserID
            searchParameters.ClientID = clientId
            searchParameters.NumberOfResultsToDisplay = TopEntriesToDisplay
            searchParameters.AgentUserID = agentUserID
            searchParameters.NonEndorsementReadyLobIds = ExcludedLobIDsForEndorsements

            Return SearchCore(searchParameters)
        End Function

        Public Shared Function SearchForQuotes(searchParameters As Common.QuoteSearch.QQSearchParameters) As List(Of QQSearchResult)
            Return SearchBySearchParameters(searchParameters)
        End Function

        Public Shared Function SearchForQuotes(quoteOrPolicyID As Int32, agentUserName As String, agencyId As Int32, quoteNumber As String, clientname As String, statusIDs As String,
                                               lobIDs As String, lobIDsExclude As String, IsStaff As Boolean, neverShowArchived As Boolean,
                                               showNDays As Int32, allowResultsNotCurrentlyInVr As Boolean, HasEmployeeAccess As Boolean, CurrentUserID As Integer, Optional clientId As Int32 = 0, Optional TopEntriesToDisplay As Integer = 0, Optional agentUserID As Integer = 0, Optional isGlobalSearch As Boolean = False) As List(Of QQSearchResult)

            Dim searchParameters As New Common.QuoteSearch.QQSearchParameters

            searchParameters.SearchType = SearchType.Quotes
            searchParameters.quoteOrPolicyID = quoteOrPolicyID
            searchParameters.AgentUserName = agentUserName
            searchParameters.AgencyID = agencyId
            searchParameters.QuoteOrPolicyNumber = quoteNumber
            searchParameters.ClientName = clientname
            searchParameters.StatusIDs = statusIDs
            searchParameters.LobIDs = lobIDs
            searchParameters.ExcludedLobIds = lobIDsExclude
            searchParameters.IsStaff = IsStaff
            searchParameters.ShowArchived = Not neverShowArchived
            searchParameters.TimeFrame = showNDays
            searchParameters.AllowResultsNotCurrentlyInVR = allowResultsNotCurrentlyInVr
            searchParameters.HasEmployeeAccess = HasEmployeeAccess
            searchParameters.CurrentDiamondUserID = CurrentUserID
            searchParameters.ClientID = clientId
            searchParameters.NumberOfResultsToDisplay = TopEntriesToDisplay
            searchParameters.AgentUserID = agentUserID
            searchParameters.isGlobalSearch = isGlobalSearch

            Return SearchCore(searchParameters)
        End Function

        Public Shared Function SearchForChanges(searchParameters As Common.QuoteSearch.QQSearchParameters) As List(Of QQSearchResult)
            Return SearchCore(searchParameters)
        End Function

        Public Shared Function SearchForChanges(quoteOrPolicyID As Int32, agentUserName As String, agencyId As Int32, quoteNumber As String, clientname As String, statusIDs As String,
                                               lobIDs As String, lobIDsExclude As String, IsStaff As Boolean, neverShowArchived As Boolean,
                                               showNDays As Int32, allowResultsNotCurrentlyInVr As Boolean, HasEmployeeAccess As Boolean, CurrentUserID As Integer, Optional clientId As Int32 = 0, Optional TopEntriesToDisplay As Integer = 0, Optional agentUserID As Integer = 0) As List(Of QQSearchResult)

            Dim searchParameters As New Common.QuoteSearch.QQSearchParameters

            searchParameters.SearchType = SearchType.Changes
            searchParameters.quoteOrPolicyID = quoteOrPolicyID
            searchParameters.AgentUserName = agentUserName
            searchParameters.AgencyID = agencyId
            searchParameters.QuoteOrPolicyNumber = quoteNumber
            searchParameters.ClientName = clientname
            searchParameters.StatusIDs = statusIDs
            searchParameters.LobIDs = lobIDs
            searchParameters.ExcludedLobIds = lobIDsExclude
            searchParameters.IsStaff = IsStaff
            searchParameters.ShowArchived = Not neverShowArchived
            searchParameters.TimeFrame = showNDays
            searchParameters.AllowResultsNotCurrentlyInVR = allowResultsNotCurrentlyInVr
            searchParameters.HasEmployeeAccess = HasEmployeeAccess
            searchParameters.CurrentDiamondUserID = CurrentUserID
            searchParameters.ClientID = clientId
            searchParameters.NumberOfResultsToDisplay = TopEntriesToDisplay
            searchParameters.AgentUserID = agentUserID

            Return SearchCore(searchParameters)

        End Function

        Public Shared Function SearchForBillingUpdates(searchParameters As Common.QuoteSearch.QQSearchParameters) As List(Of QQSearchResult)
            Return SearchCore(searchParameters)
        End Function

        Public Shared Function SearchForBillingUpdates(quoteOrPolicyID As Int32, agentUserName As String, agencyId As Int32, quoteNumber As String, clientname As String, statusIDs As String,
                                               lobIDs As String, lobIDsExclude As String, IsStaff As Boolean, neverShowArchived As Boolean,
                                               showNDays As Int32, allowResultsNotCurrentlyInVr As Boolean, HasEmployeeAccess As Boolean, CurrentUserID As Integer, Optional clientId As Int32 = 0, Optional TopEntriesToDisplay As Integer = 0, Optional agentUserID As Integer = 0) As List(Of QQSearchResult)

            Dim searchParameters As New Common.QuoteSearch.QQSearchParameters

            searchParameters.SearchType = SearchType.BillingUpdates
            searchParameters.quoteOrPolicyID = quoteOrPolicyID
            searchParameters.AgentUserName = agentUserName
            searchParameters.AgencyID = agencyId
            searchParameters.QuoteOrPolicyNumber = quoteNumber
            searchParameters.ClientName = clientname
            searchParameters.StatusIDs = statusIDs
            searchParameters.LobIDs = lobIDs
            searchParameters.ExcludedLobIds = lobIDsExclude
            searchParameters.IsStaff = IsStaff
            searchParameters.ShowArchived = Not neverShowArchived
            searchParameters.TimeFrame = showNDays
            searchParameters.AllowResultsNotCurrentlyInVR = allowResultsNotCurrentlyInVr
            searchParameters.HasEmployeeAccess = HasEmployeeAccess
            searchParameters.CurrentDiamondUserID = CurrentUserID
            searchParameters.ClientID = clientId
            searchParameters.NumberOfResultsToDisplay = TopEntriesToDisplay
            searchParameters.AgentUserID = agentUserID

            Return SearchCore(searchParameters)

        End Function

        Public Shared Function SearchForPolicies(searchParameters As Common.QuoteSearch.QQSearchParameters) As List(Of QQSearchResult)
            Return SearchCore(searchParameters)
        End Function

        Public Shared Function SearchForPolicies(quoteOrPolicyID As Int32, agentUserName As String, agencyId As Int32, quoteNumber As String, clientname As String, statusIDs As String,
                                               lobIDs As String, lobIDsExclude As String, IsStaff As Boolean, neverShowArchived As Boolean,
                                               showNDays As Int32, allowResultsNotCurrentlyInVr As Boolean, HasEmployeeAccess As Boolean, CurrentUserID As Integer, Optional clientId As Int32 = 0, Optional TopEntriesToDisplay As Integer = 0, Optional agentUserID As Integer = 0) As List(Of QQSearchResult)

            Dim searchParameters As New Common.QuoteSearch.QQSearchParameters

            searchParameters.SearchType = SearchType.Policies
            searchParameters.quoteOrPolicyID = quoteOrPolicyID
            searchParameters.AgentUserName = agentUserName
            searchParameters.AgencyID = agencyId
            searchParameters.QuoteOrPolicyNumber = quoteNumber
            searchParameters.ClientName = clientname
            searchParameters.StatusIDs = statusIDs
            searchParameters.LobIDs = lobIDs
            searchParameters.ExcludedLobIds = lobIDsExclude
            searchParameters.IsStaff = IsStaff
            searchParameters.ShowArchived = Not neverShowArchived
            searchParameters.TimeFrame = showNDays
            searchParameters.AllowResultsNotCurrentlyInVR = allowResultsNotCurrentlyInVr
            searchParameters.HasEmployeeAccess = HasEmployeeAccess
            searchParameters.CurrentDiamondUserID = CurrentUserID
            searchParameters.ClientID = clientId
            searchParameters.NumberOfResultsToDisplay = TopEntriesToDisplay
            searchParameters.AgentUserID = agentUserID

            Return SearchCore(searchParameters)

        End Function

        Private Shared Sub BuildParameterListCore(ByRef params As SqlParameterCollection, searchParameters As QQSearchParameters)
            If params IsNot Nothing Then
                If searchParameters.AgencyID > 0 Then
                    params.AddWithValue("@AgencyID", searchParameters.AgencyID) 'CHECK
                End If

                If String.IsNullOrWhiteSpace(searchParameters.QuoteOrPolicyNumber) = False Then
                    params.AddWithValue("@quoteOrPolicyNumberSearchTerm", searchParameters.QuoteOrPolicyNumber) 'CHECK
                End If

                If String.IsNullOrWhiteSpace(searchParameters.ClientName) = False Then
                    params.AddWithValue("@displayNameSearchTerm", searchParameters.ClientName) 'CHECK
                End If

                If searchParameters.AgentUserID > 0 Then
                    params.AddWithValue("@AgentUserID", searchParameters.AgentUserID)
                Else
                    If String.IsNullOrEmpty(searchParameters.AgentUserName.Trim()) = False Then
                        params.AddWithValue("@AgentUsername_initial", searchParameters.AgentUserName)
                        params.AddWithValue("@AgentUsername_lastchanged", searchParameters.AgentUserName)
                    End If
                End If

                If searchParameters.ClientID > 0 Then
                    params.AddWithValue("@clientID", searchParameters.ClientID) 'CHECK
                End If

                If String.IsNullOrWhiteSpace(searchParameters.LobIDs) = False Then
                    params.AddWithValue("@LOBID_List", searchParameters.LobIDs) ' lobId 'CHECK
                End If

                If String.IsNullOrWhiteSpace(searchParameters.ExcludedLobIds) = False Then
                    If searchParameters.SearchType = SearchType.Quotes Then
                        params.AddWithValue("@LOBID_Exclude_List", searchParameters.ExcludedLobIds) ' lobId 'CHECK
                    End If
                End If

                If searchParameters.NonEndorsementReadyLobIds IsNot Nothing AndAlso searchParameters.NonEndorsementReadyLobIds.Count > 0 Then
                    params.AddWithValue("@LOBID_Exclude_List_EndorsementFunctionality", searchParameters.NonEndorsementReadyLobIds.ListToCSV())
                End If

                params.AddWithValue("@IsStaff", searchParameters.IsStaff)

                params.AddWithValue("@HasEmployeeAccess", searchParameters.HasEmployeeAccess) 'CHECK

                If searchParameters.TimeFrame > 0 Then
                    params.AddWithValue("@ShowNDays", searchParameters.TimeFrame) 'CHECK
                End If

                Dim useSingleSQLQuery As Boolean = False
                If IFM.VR.Common.Helpers.GenericHelper.GetAppSettingsValueForBoolean("VR_GlobalSearch_UseSingleSQLQuery", useSingleSQLQuery) AndAlso useSingleSQLQuery = True Then
                    'Get All Results
                    params.AddWithValue("@TopEntriesToDisplay", 0)
                Else
                    If searchParameters.NumberOfResultsToDisplay > 0 Then
                        params.AddWithValue("@TopEntriesToDisplay", searchParameters.NumberOfResultsToDisplay)
                    End If
                End If

                If searchParameters.quoteOrPolicyID > 0 Then
                    params.AddWithValue("@quoteOrPolicyID", searchParameters.quoteOrPolicyID)
                End If

                If String.IsNullOrWhiteSpace(searchParameters.StatusIDs) = False Then
                    Dim DiamondIDs As New List(Of String)
                    Dim QQIDs As New List(Of String)

                    ConvertStatusDropdownValuesToQQandDiamondStatusIDs(searchParameters.StatusIDs, QQIDs, DiamondIDs)

                    If (QQIDs IsNot Nothing AndAlso QQIDs.Count > 0) OrElse (DiamondIDs IsNot Nothing AndAlso DiamondIDs.Count > 0) Then
                        If QQIDs IsNot Nothing AndAlso QQIDs.Count > 0 Then
                            params.AddWithValue("@QuickQuoteStatusID", QQIDs.ListToCSV)
                        Else
                            params.AddWithValue("@QuickQuoteStatusID", 999) 'This should force no results to return for anything QuickQuote related, since we are wanting something Diamond related
                        End If

                        If DiamondIDs IsNot Nothing AndAlso DiamondIDs.Count > 0 Then
                            params.AddWithValue("@DiamondStatusID", DiamondIDs.ListToCSV)
                        Else
                            params.AddWithValue("@DiamondStatusID", 999) 'This should force no results to return for anything Diamond related, since we are wanting something QuickQuote related
                        End If
                    End If
                End If

                Try
                    'params.AddWithValue("@currentUserId", CInt(System.Web.HttpContext.Current.Session("DiamondUserId"))) ' 5-16-2016 Matt A
                    params.AddWithValue("@currentUserId", searchParameters.CurrentDiamondUserID)
                Catch ex As Exception
#If DEBUG Then
                    Debugger.Break()
#End If
                End Try

                If searchParameters.isGlobalSearch Then
                    params.AddWithValue("@isGlobalSearch", searchParameters.isGlobalSearch)
                End If

                params.AddWithValue("@NeverShowArchived", searchParameters.ShowArchived = False)

                params.AddWithValue("@isBillingUpdate", searchParameters.SearchType = SearchType.BillingUpdates)

                If searchParameters.SearchType = SearchType.All AndAlso searchParameters.excludeSavedQuotes = True Then 'added 3/9/2021
                    params.AddWithValue("@excludeQuoteSearch", searchParameters.excludeSavedQuotes)
                End If
            End If
        End Sub

        Private Shared Sub BuildParameterList(ByRef params As SqlParameterCollection, searchParameters As QQSearchParameters)
            BuildParameterListCore(params, searchParameters)
        End Sub

        Private Shared Sub BuildParameterList(ByRef params As SqlParameterCollection, quoteOrPolicyID As Int32, agentUserName As String, agencyId As Int32, quoteNumber As String, clientname As String, statusIDs As String,
                                               lobIDs As String, lobIDsExclude As String, IsStaff As Boolean, neverShowArchived As Boolean,
                                               showNDays As Int32, allowResultsNotCurrentlyInVr As Boolean, currentUserID As Integer, HasEmployeeAccess As Boolean, Optional clientId As Int32 = 0, Optional TopEntriesToDisplay As Integer = 0, Optional agentUserID As Integer = 0, Optional isBillingUpdate As Boolean = False, Optional searchType As SearchType = SearchType.NA, Optional ExcludedLobIDsForEndorsements As List(Of Integer) = Nothing, Optional isGlobalSearch As Boolean = False)
            If params IsNot Nothing Then
                Dim searchParameters As New QQSearchParameters
                searchParameters.SearchType = searchType

                If agencyId > 0 Then
                    searchParameters.AgencyID = agencyId
                End If

                If String.IsNullOrWhiteSpace(quoteNumber) = False Then
                    searchParameters.quoteOrPolicyID = quoteNumber
                End If

                If String.IsNullOrWhiteSpace(clientname) = False Then
                    searchParameters.ClientName = clientname
                End If

                If agentUserID > 0 Then
                    searchParameters.AgentUserID = agentUserID
                Else
                    If String.IsNullOrEmpty(agentUserName.Trim()) = False Then
                        searchParameters.AgentUserName = agentUserName
                    End If
                End If

                If clientId > 0 Then
                    searchParameters.ClientID = clientId
                End If

                If String.IsNullOrWhiteSpace(lobIDs) = False Then
                    searchParameters.LobIDs = lobIDs
                End If

                If String.IsNullOrWhiteSpace(lobIDsExclude) = False Then
                    If searchType = SearchType.Quotes Then
                        searchParameters.ExcludedLobIds = lobIDsExclude
                    End If
                End If

                If ExcludedLobIDsForEndorsements IsNot Nothing AndAlso ExcludedLobIDsForEndorsements.Count > 0 Then
                    searchParameters.NonEndorsementReadyLobIds = ExcludedLobIDsForEndorsements
                End If

                searchParameters.IsStaff = IsStaff

                searchParameters.HasEmployeeAccess = HasEmployeeAccess

                If showNDays > 0 Then
                    searchParameters.TimeFrame = showNDays
                End If

                If TopEntriesToDisplay > 0 Then
                    searchParameters.NumberOfResultsToDisplay = TopEntriesToDisplay
                End If

                If quoteOrPolicyID > 0 Then
                    searchParameters.quoteOrPolicyID = quoteOrPolicyID
                End If

                If String.IsNullOrWhiteSpace(statusIDs) = False Then
                    searchParameters.StatusIDs = statusIDs
                End If

                Try
                    searchParameters.CurrentDiamondUserID = currentUserID
                Catch ex As Exception
#If DEBUG Then
                    Debugger.Break()
#End If
                End Try

                If isGlobalSearch Then
                    searchParameters.isGlobalSearch = isGlobalSearch
                End If

                searchParameters.ShowArchived = neverShowArchived = True

                If searchType = SearchType.NA AndAlso isBillingUpdate = True Then
                    searchType = SearchType.BillingUpdates
                End If

                BuildParameterListCore(params, searchParameters)
            End If
        End Sub

        Private Shared Sub ConvertStatusDropdownValuesToQQandDiamondStatusIDs(ByVal StatusIDCSV As String, ByRef QQIDs As List(Of String), ByRef DiamondIDs As List(Of String))
            Dim statusList As List(Of String)
            If StatusIDCSV.Contains(",") Then
                statusList = StatusIDCSV.CSVtoList
            Else
                statusList = New List(Of String)
                statusList.Add(StatusIDCSV)
            End If
            If statusList IsNot Nothing AndAlso statusList.Count > 0 Then
                For Each statusID As String In statusList
                    Dim statusIDInt As Integer
                    If statusID.IsNumeric() AndAlso Integer.TryParse(statusID, statusIDInt) Then
                        If statusIDInt > 100 Then
                            If statusIDInt < 500 Then '500 and above is being used for status' like Saved Change, Saved Billing Update
                                DiamondIDs.Add(statusIDInt - 100)
                            End If
                        Else
                            QQIDs.Add(statusIDInt)
                        End If
                    End If
                Next
            End If
        End Sub

        Private Shared Function GetResult(reader As System.Data.SqlClient.SqlDataReader) As QQSearchResult
            Dim result As New QQSearchResult()

            result.QuoteId = CheckDBNull(Of Integer)(reader("QuoteID"))
            result.Description = CheckDBNull(Of String)(reader("QuoteDescription"))
            result.PolicyId = CheckDBNull(Of Integer)(reader("PolicyID"))
            result.PolicyImageNum = CheckDBNull(Of Integer)(reader("PolicyImageNum"))
            result.CurrentPolicyImageNumForPolicy = CheckDBNull(Of Integer)(reader("CurrentPolicyImageNumForPolicy"))
            result.QuoteNumber = CheckDBNull(Of String)(reader("QuoteNumber"))
            result.AgencyID = CheckDBNull(Of Integer)(reader("AgencyID"))
            result.AgencyCode = CheckDBNull(Of String)(reader("Agency Code"))
            result.PolicyNumber = CheckDBNull(Of String)(reader("PolicyNumber"))
            result.PolicyCurrentStatusID = CheckDBNull(Of Integer)(reader("PolicyCurrentStatusID"))
            result.PolicyStatusCodeID = CheckDBNull(Of Integer)(reader("PolicyStatusCodeID"))
            result.TransTypeID = CheckDBNull(Of Integer)(reader("TransTypeID"))
            result.EffectiveDate = CheckDBNull(Of Date)(reader("EffDate")).ToString("MM/dd/yyyy") 'DateTime.Parse(reader("effDate")).ToString("MM/dd/yyyy")
            result.ExpirationDate = CheckDBNull(Of Date)(reader("ExpDate")).ToString("MM/dd/yyyy") 'If(IsDBNull(reader("ExpDate")) = False, CDate(reader("Last Modified In VR")), DateTime.MinValue)
            result.TEffectiveDate = CheckDBNull(Of Date)(reader("TEffDate")).ToString("MM/dd/yyyy") 'If(IsDBNull(reader("TEffDate")) = False, CDate(reader("Last Modified In VR")), DateTime.MinValue)
            result.TExpirationDate = CheckDBNull(Of Date)(reader("TExpDate")).ToString("MM/dd/yyyy") 'If(IsDBNull(reader("TExpDate")) = False, CDate(reader("Last Modified In VR")), DateTime.MinValue)
            result.VersionID = CheckDBNull(Of Integer)(reader("VersionID"))
            result.LobId = CheckDBNull(Of Integer)(reader("LOB ID"))
            result.LobName = CheckDBNull(Of String)(reader("LOB Name"))
            result.UserName = CheckDBNull(Of String)(reader("UserName"))
            result.ClientID = CheckDBNull(Of Integer)(reader("ClientID"))
            result.ClientName = CheckDBNull(Of String)(reader("Client Name"))
            result.Policyholder = CheckDBNull(Of String)(reader("Policyholder"))
            result.Premium = CheckDBNull(Of Double)(reader("QuotedPremium")) '.ToString().Replace("$", "").Replace(",", "")
            result.PremiumChangeFullTerm = CheckDBNull(Of Double)(reader("PremiumChangeFullTerm")) '.ToString().Replace("$", "").Replace(",", "")
            result.PremiumWritten = CheckDBNull(Of Double)(reader("PremiumWritten")) '.ToString().Replace("$", "").Replace(",", "")
            result.PremiumChangeWritten = CheckDBNull(Of Double)(reader("PremiumChangeWritten")) 'ToString().Replace("$", "").Replace(",", "")
            result.CreatedTimestamp = CheckDBNull(Of Date)(reader("Inserted Date"))
            result.LastModified = CheckDBNull(Of Date)(reader("LastModifiedDate"))
            If result.LastModified.ToShortDateString() = DateTime.Now.ToShortDateString() Then
                result.LastModified_FriendlyDate = result.LastModified.ToShortTimeString()
            Else
                If (CDate(DateTime.Now.ToShortDateString()) - CDate(result.LastModified.ToShortDateString())).Days < 7 Then
                    result.LastModified_FriendlyDate = result.LastModified.DayOfWeek.ToString() + "<br/> " + result.LastModified.ToShortTimeString()
                Else
                    'do nothing just show full date
                    result.LastModified_FriendlyDate = result.LastModified.ToString()
                End If
            End If
            result.StatusId = CheckDBNull(Of Integer)(reader("Status ID")) 'moved here from below 8/21/2017
            result.Archived = CheckDBNull(Of Boolean)(reader("Archived")) 'moved here from below 8/21/2017
            result.IsStaffUserName = CheckDBNull(Of Boolean)(reader("isStaffUserName"))
            result.QuoteStatus = CheckDBNull(Of String)(reader("Quote Status"))
            result.UserID = CheckDBNull(Of Integer)(reader("userId"))
            result.CancelDate = CheckDBNull(Of Date)(reader("CancelDate"))
            result.IsBillingUpdate = CheckDBNull(Of Boolean)(reader("isBillingUpdate"))
            Dim itemType As String = CheckDBNull(Of String)(reader("ItemType"))
            result.ItemType = IFM.VR.Common.QuoteSearch.QuoteSearch.ConvertStringToItemType(itemType)
            result.CreateByUserid = CheckDBNull(Of Integer)(reader("Created By UserId"))

            '#If Not DEBUG Then
            result.IsActiveConsumerQuote = CheckDBNull(Of Boolean)(reader("Is Active Consumer Quote")) '0 or 1
            '#End If

            ' Add stateid for KY WC MGB 5-13-19
            result.StateIds = CheckDBNull(Of String)(reader("stateIds"))

            'new with Comp Rater Project
            result.OriginatedInVR = CheckDBNull(Of Boolean)(reader("originatedInVR")) '0 or 1
            result.IsCurrentlyInVR = CheckDBNull(Of Boolean)(reader("currentlyInVR")) '0 or 1
            result.VR_LastModified = CheckDBNull(Of Date)(reader("Last Modified In VR")) 'If(IsDBNull(reader("Last Modified In VR")) = False, CDate(reader("Last Modified In VR")), DateTime.MinValue) 'null or datetime
            result.Diamond_LastModified = CheckDBNull(Of Date)(reader("Last Modified In Diamond")) 'If(IsDBNull(reader("Last Modified In Diamond")) = False, CDate(reader("Last Modified In Diamond")), DateTime.MinValue) 'reader("Last Modified In Diamond") 'null or datetime
            'END new with Comp Rater Project

            result.LastRulesOverrideRecordModifiedDate = CheckDBNull(Of Date)(reader("LastRulesOverrideRecordModifiedDate")) 'added 12/9/2020 (Interoperability)

            result.ProgramTypeId = CheckDBNull(Of Integer)(reader("programTypeId")) 'added 4/22/2021 (Umbrella)

            'added 4/16/2024
            result.TransUserId = CheckDBNull(Of Integer)(reader("TransUserId"))
            result.TransUserLoginDomain = CheckDBNull(Of String)(reader("TransUserLoginDomain"))
            result.TransUserLoginName = CheckDBNull(Of String)(reader("TransUserLoginName"))

            Select Case result.ItemType
                'Case QuoteSearch.ItemType.BillingUpdate
                '    result.StatusId = 502
                'Case QuoteSearch.ItemType.Change
                '    result.StatusId = 501
                Case QuoteSearch.ItemType.Policy
                    result.StatusId = result.PolicyCurrentStatusID
                    If result.StatusId < 100 Then
                        result.StatusId += 100 'Loading DiamondStatusIDs as id + 100 to avoid confusion with our own QuickQuote StatusIds
                    End If
            End Select

            Return result
        End Function

        Private Shared Function CheckDBNull(Of T)(ByVal pReaderVar As Object) As T
            If pReaderVar.Equals(DBNull.Value) Then
                ' Value is null, determine the return type for a default
                If GetType(T).Equals(GetType(String)) Then
                    Return CType(CType("", Object), T)
                ElseIf GetType(T).Equals(GetType(Nullable(Of DateTime))) Then
                    Return CType(CType(New Nullable(Of DateTime), Object), T)
                ElseIf GetType(T).Equals(GetType(Nullable(Of Date))) Then
                    Return CType(CType(New Nullable(Of Date), Object), T)
                Else
                    ' If it's anything else just return nothing
                    Return Nothing
                End If
            Else
                ' Cast the value into the correct return type
                Return CType(pReaderVar, T)
            End If
        End Function

        Private Shared _statuses As New Dictionary(Of String, Int32)
        Friend Shared Function GetStatuses() As Dictionary(Of String, Int32)
            If _statuses.Any() = False Then
                Using conn As New System.Data.SqlClient.SqlConnection(qqConn)
                    conn.Open()
                    Using cmd As New System.Data.SqlClient.SqlCommand()
                        cmd.Connection = conn
                        cmd.CommandText = "SELECT [quoteStatusId],[description]  FROM [QuickQuote].[dbo].[QuoteStatus]"
                        Using reader As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader()
                            If reader.HasRows Then
                                While reader.Read()
                                    _statuses.Add(reader.GetString(1), reader.GetInt32(0))
                                End While

                            End If
                        End Using
                    End Using
                End Using
            End If
            Return _statuses
        End Function

        Public Shared ReadOnly Property FriendlyStatuses As Dictionary(Of String, List(Of Int32))
            Get
                ' this takes a sec longer to create each time but it is safer
                ' could just make a read-only collection but that incurs costs as well
                Dim _friendlyStatuses As New Dictionary(Of String, List(Of Int32))
                If _friendlyStatuses.Any() = False Then
                    _friendlyStatuses.Add("App", New List(Of Int32)(New Int32() {7, 8}))
                    _friendlyStatuses.Add("App Error", New List(Of Int32)(New Int32() {9, 10, 11, 17})) 'updated 8/21/2017 for App Stopped (17)
                    _friendlyStatuses.Add("Archive", New List(Of Int32)(New Int32() {12}))
                    _friendlyStatuses.Add("Finalized", New List(Of Int32)(New Int32() {14}))
                    _friendlyStatuses.Add("In Force", New List(Of Int32)(New Int32() {101}))
                    _friendlyStatuses.Add("Future", New List(Of Int32)(New Int32() {102}))
                    _friendlyStatuses.Add("Canceled", New List(Of Int32)(New Int32() {103}))
                    _friendlyStatuses.Add("Pending", New List(Of Int32)(New Int32() {104}))
                    _friendlyStatuses.Add("Quote", New List(Of Int32)(New Int32() {2, 3}))
                    _friendlyStatuses.Add("Quote Error", New List(Of Int32)(New Int32() {4, 5, 6, 15})) ' might add '1' later; updated 8/21/2017 for Quote Stopped (15)
                    _friendlyStatuses.Add("Referred to Underwriter", New List(Of Int32)(New Int32() {13}))
                    _friendlyStatuses.Add("Saved Change", New List(Of Int32)(New Int32() {501}))
                    _friendlyStatuses.Add("Saved Billing Update", New List(Of Int32)(New Int32() {502}))
                    _friendlyStatuses.Add("Finalized Change", New List(Of Int32)(New Int32() {503}))
                    _friendlyStatuses.Add("Finalized Billing Update", New List(Of Int32)(New Int32() {504}))
                End If

                Return _friendlyStatuses
            End Get
        End Property

        Public Shared ReadOnly Property FriendlyStatusesForGlobalSearchDropdown As Dictionary(Of String, List(Of Int32))
            Get
                Dim _friendlyStatusesForDD As Dictionary(Of String, List(Of Int32)) = FriendlyStatuses
                Dim friendlyStatusesToRemove As New List(Of String)

                friendlyStatusesToRemove.Add("Saved Change")
                friendlyStatusesToRemove.Add("Saved Billing Update")
                friendlyStatusesToRemove.Add("Finalized Change")
                friendlyStatusesToRemove.Add("Finalized Billing Update")
                'friendlyStatusesToRemove.Add("Finalized") 'Finalized dropdown option reimplemented 11/11/2019 DJG

                For Each statusToRemove As String In friendlyStatusesToRemove
                    If _friendlyStatusesForDD.ContainsKey(statusToRemove) Then
                        _friendlyStatusesForDD.Remove(statusToRemove)
                    End If
                Next

                Return _friendlyStatusesForDD
            End Get
        End Property

        'TODO - Matt - Cleanup these LOB related items should probably live in the LOB helper - Minimize use of LOB ids if possible
        Private Shared _lobIds As New Dictionary(Of String, Int32)
        Private Shared Function GetLobs() As Dictionary(Of String, Int32)
            If _lobIds.Any() = False Then
                Using conn As New System.Data.SqlClient.SqlConnection(qqConn)
                    conn.Open()
                    Using cmd As New System.Data.SqlClient.SqlCommand()
                        cmd.Connection = conn
                        cmd.CommandText = "SELECT [lob_id],[lobname] FROM [Diamond].[dbo].[Lob] with (nolock) order by lobname, lob_id"
                        Using reader As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader()
                            If reader.HasRows Then
                                While reader.Read()
                                    Select Case reader.GetString(1).Trim().ToLower()
                                        Case "auto personal" ' used to change LOB names - would rather not do this but BU wants changes
                                            Dim NewLobName As String = "Personal Auto"
                                            If _lobIds.ContainsKey(NewLobName) = False Then
                                                _lobIds.Add(NewLobName, reader.GetInt32(0))
                                            End If
                                        Case "home personal"
                                            Dim NewLobName As String = "Personal Home"
                                            If _lobIds.ContainsKey(NewLobName) = False Then
                                                _lobIds.Add(NewLobName, reader.GetInt32(0))
                                            End If
                                        Case "dwelling fire personal"
                                            Dim NewLobName As String = "Personal Dwelling Fire"
                                            If _lobIds.ContainsKey(NewLobName) = False Then
                                                _lobIds.Add(NewLobName, reader.GetInt32(0))
                                            End If
                                        Case "umbrella personal"
                                            Dim NewLobName As String = "Personal/Farm Umbrella"
                                            If _lobIds.ContainsKey(NewLobName) = False Then
                                                _lobIds.Add(NewLobName, reader.GetInt32(0))
                                            End If
                                        Case Else
                                            If _lobIds.ContainsKey(reader.GetString(1)) = False Then
                                                _lobIds.Add(reader.GetString(1), reader.GetInt32(0))
                                            End If
                                    End Select
                                End While
                            End If
                        End Using
                    End Using
                End Using
            End If
            Return _lobIds
        End Function

        Public Shared Function GetLobNameFromId(id As Int32) As String
            Dim lobs = GetLobs()
            If lobs.Values.Contains(id) Then
                For Each l As KeyValuePair(Of String, Int32) In lobs
                    If l.Value = id Then
                        Return l.Key
                    End If
                Next
            End If
            Return Nothing
        End Function

        Public Shared Function GetVrSupportedLobs() As Dictionary(Of String, Int32)
            Dim ids As List(Of String) = supportedLobIds.Split(",").ToList()

            Do_UAT_Restriction(ids) ' Matt A 8-21-15

            Return (From lob As KeyValuePair(Of String, Int32) In GetLobs() Where ids.Contains(lob.Value.ToString()) Select lob).ToDictionary(Function(c)
                                                                                                                                                  Return c.Key
                                                                                                                                              End Function,
                                                                                                                                              Function(c)
                                                                                                                                                  Return c.Value
                                                                                                                                              End Function)
        End Function

        Public Shared Function GetVrSupportedLobs(getLobsWithTheseIdsIfSupportedList As Int32()) As Dictionary(Of String, Int32)
            Dim ids As List(Of String) = supportedLobIds.Split(",").ToList()

            Do_UAT_Restriction(ids) ' Matt A 8-21-15

            Return (From lob As KeyValuePair(Of String, Int32) In GetLobs() Where ids.Contains(lob.Value.ToString()) AndAlso getLobsWithTheseIdsIfSupportedList.Contains(lob.Value) Select lob).ToDictionary(Function(c)
                                                                                                                                                                                                                 Return c.Key
                                                                                                                                                                                                             End Function,
                                                                                                                                              Function(c)
                                                                                                                                                  Return c.Value
                                                                                                                                              End Function)
        End Function

        Private Shared Sub Do_UAT_Restriction(ids As List(Of String))
            Dim qqHelper As New QuickQuoteHelperClass()

            Dim UATLobAgency_Text As String = If(System.Configuration.ConfigurationManager.AppSettings("UAT_Testing_LOB_Agency_HasAccess") IsNot Nothing, System.Configuration.ConfigurationManager.AppSettings("UAT_Testing_LOB_Agency_HasAccess"), "")
            If String.IsNullOrWhiteSpace(UATLobAgency_Text) = False AndAlso qqHelper.IsHomeOfficeStaffUser() = False Then
                ' In UAT mode
                Dim restrictList = {(New With {.LobId = 0, .AllowedAgencies = New List(Of String)})}.Take(0).ToList()

                Dim pairs_text As String() = Nothing
                If UATLobAgency_Text.Contains(",") Then
                    pairs_text = UATLobAgency_Text.Split(",")
                Else
                    pairs_text = New String() {UATLobAgency_Text}
                End If

                For Each p In pairs_text
                    Dim lob As Int32 = CInt(p.Substring(0, p.IndexOf("(")))
                    Dim agencies As New List(Of String)
                    Dim agencyList_Text As String = p.Substring(p.IndexOf("(") + 1, (p.IndexOf(")") - p.IndexOf("(")) - 1)

                    If agencyList_Text.Contains("|") Then
                        agencies = agencyList_Text.Split("|").ToList()
                    Else
                        agencies.Add(agencyList_Text)
                    End If
                    restrictList.Add(New With {.LobId = lob, .AllowedAgencies = agencies})
                Next

                If restrictList.Any() Then
                    Dim myAgencyNon6000Codes As List(Of String) = (From a In MyAgencyList Select a.ShortAgencyCode).ToList()

                    For Each r In restrictList
                        If ids.Contains(r.LobId) Then
                            Dim allowLob As Boolean = False
                            For Each c In r.AllowedAgencies
                                If myAgencyNon6000Codes.Contains(c) Then
                                    allowLob = True
                                    Exit For
                                End If
                            Next
                            If allowLob = False Then
                                ids.Remove(r.LobId)
                            End If
                        End If
                    Next
                End If

            End If
        End Sub

        Public Shared Function GetVrSupportedLobs_Enums() As List(Of QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType)
            Dim lobs As New List(Of QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType)
            For Each l In GetVrSupportedLobs()
                lobs.Add(CType(l.Value, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType))

            Next
            Return lobs
        End Function

        Public Enum LobCategory ' high number to not conflict with real lobs
            Personal = 1001
            Commercial = 1002
            Farm = 1003
            PersonalPropertyandLiability = 1004
            Umbrella = 1005
            'Dwelling_lobs like HOM, DFR, ect
        End Enum

        Public Enum SearchResultColumns
            Action
            QuoteNumAndDescription
            ClientName
            PolicyTerm
            Premium
            FriendlyStatus
            LastModified
            LOBName
            AgencyCode
            QuoteID
            ItemType
        End Enum

        ' These are all lobs grouped into categories. They are not all supported. (Includes 8/24/2020 MultiState!)
        ' Make sure Supported LOBS (including MultiState!) are added to the webconfig - supportedLobIds
        Public Shared CommericialLobs() As String = {9, 10, 11, 12, 15, 20, 21, 23, 24, 25, 26, 27, 28, 29, 43, 44, 45, 46, 47, 48, 49, 50}
        Public Shared PersonalLobs() As String = {1, 2, 3, 4, 6, 7, 8, 13, 14, 16, 18, 19, 22, 51, 53}
        Public Shared FarmLobs() As String = {17, 52}
        Public Shared PersonalPropertyandLiabilityLobs() As String = {2, 3}
        Public Shared UmbrellaLobs() As String = {14}

        Public Shared Function GetVrSupportedLobs(lobCategory As LobCategory) As Dictionary(Of String, Int32)
            Dim lob As Dictionary(Of String, Int32) = GetVrSupportedLobs()
            Dim lobCategoryList As String() = Nothing
            Select Case lobCategory
                Case QuoteSearch.LobCategory.Personal
                    lobCategoryList = PersonalLobs
                Case QuoteSearch.LobCategory.Commercial
                    lobCategoryList = CommericialLobs
                Case QuoteSearch.LobCategory.Farm
                    lobCategoryList = FarmLobs
                Case QuoteSearch.LobCategory.PersonalPropertyandLiability
                    lobCategoryList = PersonalPropertyandLiabilityLobs
                Case QuoteSearch.LobCategory.Umbrella
                    lobCategoryList = UmbrellaLobs
            End Select

            lob = (From l In lob, _l In lobCategoryList Where l.Value = CInt(_l) Select l).ToDictionary(Function(c)
                                                                                                            Return c.Key
                                                                                                        End Function,
                                                                                                Function(c)
                                                                                                    Return c.Value
                                                                                                End Function)

            ' **********      SORTING     **************
            Select Case lobCategory
                Case QuoteSearch.LobCategory.Personal, QuoteSearch.LobCategory.PersonalPropertyandLiability
                    ' order by lobID
                    lob = (From l In lob Select l Order By l.Value Ascending).ToDictionary(Function(c)
                                                                                               Return c.Key
                                                                                           End Function,
                                                                                                Function(c)
                                                                                                    Return c.Value
                                                                                                End Function)
                Case QuoteSearch.LobCategory.Commercial
                    'just use original order
                Case QuoteSearch.LobCategory.Farm
                    'just use original order
            End Select
            ' **********   END   SORTING    **************

            Return lob
        End Function

        Public Shared Function ConvertStringToItemType(myItemType As String) As ItemType
            Dim itemType As ItemType = ItemType.NA
            If String.IsNullOrWhiteSpace(myItemType) = False Then
                Select Case True
                    Case myItemType.Equals("quote", StringComparison.OrdinalIgnoreCase)
                        itemType = ItemType.Quote
                    Case myItemType.Equals("policy", StringComparison.OrdinalIgnoreCase)
                        itemType = ItemType.Policy
                    Case myItemType.Equals("change", StringComparison.OrdinalIgnoreCase)
                        itemType = ItemType.Change
                    Case myItemType.Equals("billingupdate", StringComparison.OrdinalIgnoreCase)
                        itemType = ItemType.BillingUpdate
                End Select
            End If
            Return itemType
        End Function

    End Class

    <Serializable()>
    Public Class QQSearchResult
        ''' <summary>
        ''' These are the lobs that are supported on the old comparative rating system.
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Function commSystemLobs() As String()
            Dim commSystemLobsList = New List(Of String)()
            Return commSystemLobsList.ToArray()
            'Return If(IFM.VR.Common.VRFeatures.NewLookBOPEnabled AndAlso IFM.VR.Common.VRFeatures.NewLookCGLEnabled,
            '    {"20", "21", "23", "28"},
            '    If(IFM.VR.Common.VRFeatures.NewLookBOPEnabled = False AndAlso IFM.VR.Common.VRFeatures.NewLookCGLEnabled = False,
            '    {"9", "20", "21", "23", "25", "28"},
            '    If(IFM.VR.Common.VRFeatures.NewLookBOPEnabled,
            '    {"9", "20", "21", "23", "28"},
            '    If(IFM.VR.Common.VRFeatures.NewLookCGLEnabled,
            '    {"20", "21", "23", "25", "28"},
            '    {"9", "20", "21", "23", "25", "28"})))) ' this last one should be impossible to hit at this point
        End Function









        'Public Const IFM.VR.Common.Workflow.Workflow.WorkFlowSection_qs As String = "Workflow"

        'Public Enum WorkflowSection
        '    na = 1000
        '    policyHolders = 0
        '    drivers = 1
        '    vehicles = 2
        '    coverages = 3
        '    summary = 4
        '    uwQuestions = 5
        '    app = 6
        '    property_ = 7
        '    location = 8
        '    farmIRPM = 9
        '    farmPP = 10
        '    InlandMarine = 11
        '    buildings = 12
        '    fileUpload = 13
        'End Enum

        Public Overrides Function ToString() As String
            Return Me.QuoteNumAndDescription ' String.Format("GetQuoteObject: {0}", GetQuoteObject)
        End Function

        'ConfigurationManager.AppSettings("QuickQuote_QuoteProposalSelection") + "?quoteid=" + quoteID.ToString()

        Public Property IsActiveConsumerQuote As Boolean = False

        Public ReadOnly Property ViewUrl As String
            Get
                Select Case ItemType
                    Case QuoteSearch.ItemType.Quote
                        Return GetViewURLForQuote()
                    Case QuoteSearch.ItemType.Change

                    Case QuoteSearch.ItemType.BillingUpdate

                    Case QuoteSearch.ItemType.Policy

                End Select

#If DEBUG Then
                Debugger.Break()
#End If
                Return ""
            End Get
        End Property

        Public ReadOnly Property ProposalUrl As String
            Get
                If commSystemLobs.Contains(Me.LobId) Then
                    Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_QuoteProposalSelection") + "?quoteid=" + Me.QuoteId.ToString()
                End If
                Return ""
            End Get
        End Property

        Public ReadOnly Property AvilableActions As Dictionary(Of String, String)
            Get
                Dim actions As New Dictionary(Of String, String)

                Select Case ItemType
                    Case QuoteSearch.ItemType.Quote
                        'actions = GetAvailableActionsForQuote(IsStaffUserName)
                        '1/20/2023 note: code should probably be using the following... IsStaffUserName refers to whether or not staff was the last one to touch this quote, but GetAvailableActionsForQuote should be more concerned w/ whether or not the current user is Staff
                        '1/20/2023 note continued: may be better to only check qqHelper.IsHomeOfficeStaffUser() once per user load and not for each quote, which is what having the code directly below would do, but it doesn't look like the IsHomeOfficeStaffUser check is very heavy (just checks session), so not a big deal
                        'finally updated 5/17/2023
                        Dim qqHelper As New QuickQuoteHelperClass()
                        actions = GetAvailableActionsForQuote(qqHelper.IsHomeOfficeStaffUser())
                    Case QuoteSearch.ItemType.Policy
                        actions = GetAvailableActionsForPolicy()
                    Case QuoteSearch.ItemType.Change
                        actions = GetAvailableActionsForChange()
                    Case QuoteSearch.ItemType.BillingUpdate
                        actions = GetAvailableActionsForBillingUpdate()
                End Select

                Return actions
            End Get
        End Property

        Private Shared ReadOnly Property DaysAgoForPolicyToRestartAtPolicyHolder As Integer
            Get
                Dim configDays As Integer = 0
                Dim defaultDays As Integer = 61
                configDays = System.Configuration.ConfigurationManager.AppSettings("VR_NewCo_DaysAgoForPolicyToRestartAtPolicyHolder")
                If configDays > 0 Then
                    Return configDays
                Else
                    Return defaultDays
                End If
            End Get
        End Property

        Private Shared ReadOnly Property DaysAgoForForcedQuoteArchivingOrCopy As Integer
            Get
                Dim configDays As Integer = 0
                Dim defaultDays As Integer = 91
                configDays = System.Configuration.ConfigurationManager.AppSettings("VR_NewCo_DaysAgoForForcedQuoteArchivingOrCopy")
                If configDays > 0 Then
                    Return configDays
                Else
                    Return defaultDays
                End If
            End Get
        End Property

        Private ReadOnly Property isCommercialQuote As Boolean
            Get
                Dim CommercialLobs() As String = {9, 10, 11, 12, 15, 20, 21, 23, 24, 25, 26, 27, 28, 29, 43, 44, 45, 46, 47, 48, 49, 50}
                'configDays = System.Configuration.ConfigurationManager.AppSettings("VR_NewCo_DaysAgoForPolicyToRestartAtPolicyHolder")
                If CommercialLobs.Contains(LobId) Then
                    Return True
                End If
                Return False
            End Get
        End Property

        Public Property PolicyImageNum As Int32
        Public Property CurrentPolicyImageNumForPolicy As Int32
        Public Property QuoteId As Int32
        Public Property PolicyId As Int32
        Public Property PolicyCurrentStatusID As Int32
        Public Property PolicyStatusCodeID As Int32
        Public Property QuoteNumber As String
        Public Property PolicyNumber As String

        Public Property TransTypeID As Int32
        Public Property ExpirationDate As String
        Public Property TEffectiveDate As String
        Public Property TExpirationDate As String
        Public Property VersionID As Int32
        Public Property UserName As String
        Public Property ClientID As Int32
        Public Property Policyholder As String
        Public Property PremiumChangeFullTerm As Double
        Public Property PremiumWritten As Double
        Public Property PremiumChangeWritten As Double
        Public Property InsertedDate As DateTime
        Public Property Archived As Boolean
        Public Property IsStaffUserName As Boolean
        Public Property QuoteStatus As String
        Public Property UserID As Int32
        Public Property CancelDate As Date
        Public Property ItemType As IFM.VR.Common.QuoteSearch.QuoteSearch.ItemType = QuoteSearch.ItemType.NA

        Public Property IsBillingUpdate As Boolean = False
        Public ReadOnly Property QuoteNumAndDescription
            Get
                If String.IsNullOrWhiteSpace(Me.PolicyNumber) = False Then
                    If String.IsNullOrWhiteSpace(Me.ShortDescription) Then
                        Return PolicyNumber
                    Else
                        Return String.Format("{0} - {1}", PolicyNumber, ShortDescription)
                    End If
                Else
                    If String.IsNullOrWhiteSpace(Me.QuoteNumber) = False Then
                        If String.IsNullOrWhiteSpace(Me.ShortDescription) Then
                            Return QuoteNumber
                        Else
                            Return String.Format("{0} - {1}", QuoteNumber, ShortDescription)
                        End If
                    End If
                End If

                Return ShortDescription
            End Get
        End Property

        Public Property Description As String
        Private ReadOnly Property ShortDescription As String
            Get
                If String.IsNullOrWhiteSpace(Me.Description) = False Then
                    If Me.Description.Length > 50 Then
                        Return Me.Description.Substring(0, 50) + "..."
                    Else
                        Return Me.Description
                    End If
                End If
                Return ""
            End Get
        End Property

        Public Property StateIds As String   ' Added 5/13/19 MGB for KY WC
        Public Property ClientName As String
        Public Property AgencyID As Int32
        Public Property AgencyCode As String
        Public Property LastModified As DateTime
        Public Property CreatedTimestamp As DateTime
        Public Property CreateByUserid As Int32
        Public Property Premium As Double

        Public ReadOnly Property FormatedPremium As String
            Get
                Return String.Format("{0:C2}", Me.Premium)
            End Get
        End Property

        Public Property LobName As String
        Public Property LobId As Int32
        Public Property LastModifiedByUsername As String
        Public Property LastModifiedByaIfmStaffMember As Boolean
        Public Property StatusId As Int32
        Private Property _isInCancelWarning As Boolean = False
        Public ReadOnly Property IsInCancelWarning As Boolean
            Get
                Return _isInCancelWarning
            End Get
        End Property
        Private Property _isInFinalCancelWarning As Boolean = False
        Public ReadOnly Property IsInFinalCancelWarning As Boolean
            Get
                Return _isInFinalCancelWarning
            End Get
        End Property
        Public Property EffectiveDate As String
        Public Property LastModified_FriendlyDate As String
        Public Property HasPendingTranaction As Boolean

        Public ReadOnly Property FriendlyStatus As String
            Get

                Dim stat As String = "Unknown Status"

                Dim statusIdToUse As Integer = Me.StatusId
                Select Case Me.ItemType
                    Case QuoteSearch.ItemType.Change
                        statusIdToUse = 501
                        Select Case Me.StatusId
                            Case 13 'Referred to Underwriting
                                statusIdToUse = 13
                            Case 14 'Change is in finalized state
                                statusIdToUse = 503
                        End Select
                    Case QuoteSearch.ItemType.BillingUpdate
                        statusIdToUse = 502
                        Select Case Me.StatusId
                            Case 13 'Referred to Underwriting
                                statusIdToUse = 13
                            Case 14 'Billing Change is in finalized state
                                statusIdToUse = 504
                        End Select
                End Select
                For Each Val As KeyValuePair(Of String, List(Of Int32)) In QuoteSearch.FriendlyStatuses
                    'If Val.Value.Contains(Me.StatusId) Then
                    If Val.Value.Contains(statusIdToUse) Then
                        stat = Val.Key
                        Exit For
                    End If
                Next

                If Me.StatusId = 101 OrElse Me.StatusId = 102 OrElse Me.StatusId = 103 Then
                    Dim diamondStatusId As Integer = Me.StatusId - 100
                    Dim ActualStatus As New QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo.DiamondPolicyCurrentStatus
                    Dim qqHelper As New QuickQuoteHelperClass

                    If IsPolicyInforceFuture(ActualStatus) Then
                        _isInCancelWarning = qqHelper.HasAdvancedCancelWarning(Me.PolicyNumber, Me.PolicyId, Me.AgencyCode, hasFinalCancelWarning:=_isInFinalCancelWarning)
                    End If

                    If IsInCancelWarning OrElse IsInFinalCancelWarning Then
                        If IsInFinalCancelWarning Then
                            stat = "Final Cancel Warning"
                        Else
                            stat = "Cancel Warning"
                        End If
                    Else
                        If ActualStatus <> diamondStatusId Then
                            Me.StatusId = ActualStatus + 100
                            For Each Val As KeyValuePair(Of String, List(Of Int32)) In QuoteSearch.FriendlyStatuses
                                If Val.Value.Contains(Me.StatusId) Then
                                    stat = Val.Key
                                    Exit For
                                End If
                            Next
                        End If
                    End If
                End If

                Return stat
            End Get
        End Property

        Public Property OriginatedInVR As Boolean
        Public Property IsCurrentlyInVR As Boolean

        Public Property VR_LastModified As DateTime

        Public Property Diamond_LastModified As DateTime

        Public ReadOnly Property QuickQuoteTransactionType As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType
            Get
                Select Case ItemType
                    Case QuoteSearch.ItemType.Quote
                        Return QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote
                    Case QuoteSearch.ItemType.Policy
                        Return QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage
                    Case QuoteSearch.ItemType.Change
                        Return QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote
                    Case QuoteSearch.ItemType.BillingUpdate
                        Return QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote
                    Case Else
                        Return QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.None
                End Select
            End Get
        End Property

        Public ReadOnly Property WasLastModifiedInVR As Boolean
            Get
                'Return VR_LastModified > Diamond_LastModified
                'updated 12/9/2020 (Interoperability)
                Dim diaDateToEvaluate As DateTime = Diamond_LastModified
                If LastRulesOverrideRecordModifiedDate <> Nothing AndAlso (Diamond_LastModified = Nothing OrElse LastRulesOverrideRecordModifiedDate > Diamond_LastModified) Then
                    diaDateToEvaluate = LastRulesOverrideRecordModifiedDate
                End If
                Return VR_LastModified > diaDateToEvaluate
            End Get
        End Property

        Public ReadOnly Property GetQuoteObject As QuickQuote.CommonObjects.QuickQuoteObject
            Get
                If String.IsNullOrWhiteSpace(Me.PolicyNumber) = False AndAlso Me.StatusId <> 13 Then '  Me.QuoteNumber.Contains("(")
                    'Portal
                    Return Nothing
                Else
                    Select Case Me.StatusId
                        Case 2, 4, 5
                            'edit page
                            Return IFM.VR.Common.QuoteSave.QuoteSaveHelpers.GetQuoteById_NOSESSION(QuoteId.ToString())
                        Case 3, 6 ',8, 11  '5-21-14
                            ' quote summary
                            Return IFM.VR.Common.QuoteSave.QuoteSaveHelpers.GetRatedQuoteById_NOSESSION(QuoteId.ToString(), "", QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.None, QuickQuoteXML.QuickQuoteSaveType.Quote)

                        Case 7, 9, 10 ', 8, 11 ' added ,8, 11 on 5-21
                            ' app gap
                            Return IFM.VR.Common.QuoteSave.QuoteSaveHelpers.GetQuoteById_NOSESSION(QuoteId.ToString())
                        Case 8, 11 ' on 5-21
                            ' app gap summary
                            Return IFM.VR.Common.QuoteSave.QuoteSaveHelpers.GetRatedQuoteById_NOSESSION(QuoteId.ToString(), "", QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.None, QuickQuoteXML.QuickQuoteSaveType.AppGap)

                        Case 14
                            ' portal for staff
                            'actions.Add("Portal", Me.QuoteId.ToString())
                        Case Else

                    End Select

                End If
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property GetQuoteObjectAsJSON As String
            Get
                Dim q = GetQuoteObject
                If q IsNot Nothing Then
                    Dim jsSerializer As New JavaScriptSerializer()
                    Return jsSerializer.Serialize(q)
                End If
                Return ""

            End Get
        End Property

        Public ReadOnly Property GetQuoteObjectAsXML As String
            Get
                Dim q = GetQuoteObject
                If q IsNot Nothing Then
                    Dim stringWriter As New StringWriter()
                    Dim jsSerializer As New XmlSerializer(q.GetType)
                    jsSerializer.Serialize(stringWriter, q)
                    Return stringWriter.ToString()
                End If
                Return ""

            End Get
        End Property

        Public Property LastRulesOverrideRecordModifiedDate As DateTime 'added 12/9/2020 (Interoperability)

        Public Property ProgramTypeId As Integer 'added 4/22/2021 (Umbrella)

        'added 4/16/2024
        Public Property TransUserId As Integer
        Public Property TransUserLoginDomain As String
        Public Property TransUserLoginName As String
        Public ReadOnly Property HasStaffTransUser As Boolean
            Get
                If String.IsNullOrWhiteSpace(_TransUserLoginDomain) = False AndAlso UCase(_TransUserLoginDomain) = "IFM.IFMIC" Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property

        ''' <summary>
        ''' Converts ths 
        ''' </summary>
        ''' <returns></returns>
        Private Function StateIdsToListOfString() As List(Of String)
            ' Parse the result into a list of string
            ' Note that no list or empty list means Indiana (16) only
            'Dim strList As New List(Of String)
            'If Me.StateIds Is Nothing Then
            '    strList.Add("16")
            '    Return strList
            'End If
            'Dim strParts() As String = Me.StateIds.Split(" ")
            'For Each s As String In strParts
            '    If s.Trim <> String.Empty Then
            '        strList.Add(s)
            '    End If
            'Next

            Dim strList As List(Of String) = QuickQuoteHelperClass.ListOfStringFromString(Me.StateIds, delimiter:=" ")
            If strList Is Nothing Then
                strList = New List(Of String)
            End If
            If strList.Count = 0 Then
                strList.Add("16")
            End If

            Return strList
        End Function

        Public Shared Function GetFromQQObject(q As QuickQuote.CommonObjects.QuickQuoteObject, agencyId As Int32, isStaff As Boolean) As QQSearchResult
            If q IsNot Nothing Then
                Dim qqHelper As New QuickQuoteHelperClass
                Return QuoteSearch.SearchForQuotes(q.Database_QuoteId, "", agencyId, "", "", "", "", "", isStaff, False, 0, False, qqHelper.CanUserAccessEmployeePolicies(), CInt(System.Web.HttpContext.Current.Session("DiamondUserId"))).FirstOrDefault()
            End If
            Return Nothing
        End Function

        Public Shared Function GetViewUrl(lobId As Int32, statusId As Int32, quoteId As Int32) As String
            Dim fakeResult As New QQSearchResult()
            fakeResult.LobId = lobId
            fakeResult.StatusId = statusId
            fakeResult.QuoteId = quoteId
            Return fakeResult.ViewUrl
        End Function

        Private Function IsPolicyInforceFuture(ByRef ActualStatus As QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo.DiamondPolicyCurrentStatus) As Boolean
            Dim isPolicyInforceOrFuture As Boolean = False
            Dim polStatus As QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo.DiamondPolicyCurrentStatus = QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo.DiamondPolicyCurrentStatus.None

            Select Case Me.PolicyCurrentStatusID
                Case QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo.DiamondPolicyCurrentStatus.InForce, QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo.DiamondPolicyCurrentStatus.Future, QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo.DiamondPolicyCurrentStatus.Cancelled
                    ActualStatus = ActualPolicyStatus()
                Case Else
                    ActualStatus = Me.PolicyCurrentStatusID
            End Select

            Select Case ActualStatus
                Case QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo.DiamondPolicyCurrentStatus.InForce, QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo.DiamondPolicyCurrentStatus.Future
                    isPolicyInforceOrFuture = True
            End Select

            Return isPolicyInforceOrFuture
        End Function

        Private Function ActualPolicyStatus() As QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo.DiamondPolicyCurrentStatus
            Dim polStatus As QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo.DiamondPolicyCurrentStatus = QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo.DiamondPolicyCurrentStatus.None

            If IsInForce() = True Then
                polStatus = QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo.DiamondPolicyCurrentStatus.InForce
            ElseIf IsCancelled() = True Then
                polStatus = QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo.DiamondPolicyCurrentStatus.Cancelled
            ElseIf IsExpired() = True Then
                polStatus = QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo.DiamondPolicyCurrentStatus.Expired
            Else
                polStatus = Me.PolicyCurrentStatusID
            End If

            Return polStatus
        End Function

        Private Function IsInForce() As Boolean
            Dim isIt As Boolean = False
            Dim qqHelper As New QuickQuoteHelperClass
            Dim polStatus As QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo.DiamondPolicyCurrentStatus = Me.PolicyCurrentStatusID
            Select Case polStatus
                Case QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo.DiamondPolicyCurrentStatus.InForce
                    If UsingExpirationDateToSeeIfReallyInforce() = True Then
                        If qqHelper.IsValidDateString(ExpirationDate, mustBeGreaterThanDefaultDate:=True) = True Then
                            If CDate(ExpirationDate) >= Date.Today Then
                                isIt = True
                            End If
                        Else
                            isIt = True
                        End If
                    Else
                        isIt = True
                    End If
                Case QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo.DiamondPolicyCurrentStatus.Cancelled
                    If UsingCancelDateToSeeIfReallyCancelled() = True Then
                        If qqHelper.IsValidDateString(CancelDate, mustBeGreaterThanDefaultDate:=True) = True AndAlso CDate(CancelDate) >= Date.Today Then
                            isIt = True
                        End If
                    End If
            End Select

            Return isIt
        End Function
        Private Function IsCancelled() As Boolean
            Dim isIt As Boolean = False
            Dim qqhelper As New QuickQuoteHelperClass
            Dim polStatus As QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo.DiamondPolicyCurrentStatus = Me.PolicyCurrentStatusID
            Select Case polStatus
                Case QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo.DiamondPolicyCurrentStatus.Cancelled
                    If UsingCancelDateToSeeIfReallyCancelled() = True Then
                        If qqhelper.IsValidDateString(CancelDate, mustBeGreaterThanDefaultDate:=True) = True Then
                            If CDate(CancelDate) < Date.Today Then
                                isIt = True
                            End If
                        Else
                            isIt = True
                        End If
                    Else
                        isIt = True
                    End If
            End Select

            Return isIt
        End Function

        Private Function IsExpired() As Boolean
            Dim isIt As Boolean = False
            Dim qqhelper As New QuickQuoteHelperClass
            Dim polStatus As QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo.DiamondPolicyCurrentStatus = Me.PolicyCurrentStatusID
            Select Case polStatus
                Case QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo.DiamondPolicyCurrentStatus.Expired
                    isIt = True
                Case QuickQuote.CommonObjects.QuickQuotePolicyLookupInfo.DiamondPolicyCurrentStatus.InForce
                    If UsingExpirationDateToSeeIfReallyInforce() = True Then
                        If qqhelper.IsValidDateString(ExpirationDate, mustBeGreaterThanDefaultDate:=True) = True AndAlso CDate(ExpirationDate) < Date.Today Then
                            isIt = True
                        End If
                    End If
            End Select

            Return isIt
        End Function
        Public Function UsingExpirationDateToSeeIfReallyInforce() As Boolean
            Dim usingIt As Boolean = False

            Dim strUseExpirationDateToSeeIfReallyInforce As String = QuickQuote.CommonMethods.QuickQuoteHelperClass.configAppSettingValueAsString("PolicyNumberObject_DiamondPolicyStatus_UseExpirationDateToSeeIfReallyInforce")
            If String.IsNullOrWhiteSpace(strUseExpirationDateToSeeIfReallyInforce) = False Then
                If UCase(strUseExpirationDateToSeeIfReallyInforce) = "YES" Then
                    usingIt = True
                Else
                    Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
                    If qqHelper.BitToBoolean(strUseExpirationDateToSeeIfReallyInforce) = True Then
                        usingIt = True
                    End If
                End If
            End If

            Return usingIt
        End Function
        Public Function UsingCancelDateToSeeIfReallyCancelled() As Boolean
            Dim usingIt As Boolean = False

            Dim strUseCancelDateToSeeIfReallyCancelled As String = QuickQuote.CommonMethods.QuickQuoteHelperClass.configAppSettingValueAsString("PolicyNumberObject_DiamondPolicyStatus_UseCancelDateToSeeIfReallyCancelled")
            If String.IsNullOrWhiteSpace(strUseCancelDateToSeeIfReallyCancelled) = False Then
                If UCase(strUseCancelDateToSeeIfReallyCancelled) = "YES" Then
                    usingIt = True
                Else
                    Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
                    If qqHelper.BitToBoolean(strUseCancelDateToSeeIfReallyCancelled) = True Then
                        usingIt = True
                    End If
                End If
            End If

            Return usingIt
        End Function

        Private Function LOBHasPortal() As Boolean
            If Not HideDiamondPortalLink() Then
                If AppSettings("VR_NoPortal_LOBs") IsNot Nothing Then
                    Dim lobabbrev As String = GetLobAbbrev()
                    If AppSettings("VR_NoPortal_LOBs").ToUpper.Contains(lobabbrev) Then
                        Return False
                    Else
                        Return True
                    End If
                Else
                    If Me.LobId = "17" Then ' Farm does not exist in Portal
                        Return False
                    End If
                    Return True
                End If
            End If
            Return False
        End Function

        Private Function HideDiamondPortalLink() As Boolean
            Dim hideIt As Boolean = False

            Dim qqHelper As New QuickQuoteHelperClass
            Dim strRetirementDt As String = QuickQuoteHelperClass.configAppSettingValueAsString("DiamondPortalRetirementDate")
            If qqHelper.IsValidDateString(strRetirementDt, mustBeGreaterThanDefaultDate:=True) = True AndAlso Date.Today >= CDate(strRetirementDt) Then
                hideIt = True
            End If

            Return hideIt
        End Function

        Private Function GetLobAbbrev() As String
            Select Case Me.LobName.ToUpper()
                Case "PERSONAL AUTO", "AUTO PERSONAL"
                    Return "PPA"
                Case "PERSONAL HOME", "HOME PERSONAL"
                    Return "HOM"
                Case "PERSONAL DWELLING FIRE", "DWELLING FIRE PERSONAL"
                    Return "DFR"
                Case "COMMERCIAL AUTO"
                    Return "CAP"
                Case "COMMERCIAL BOP"
                    Return "BOP"
                Case "FARM"
                    Return "FAR"
                Case "WORKERS COMP"
                    Return "WCP"
                Case "INLAND MARINE PERSONAL"
                    Return "PIM"
                Case "COMMERCIAL GENERAL LIABILITY"
                    Return "CGL"
                Case "COMMERCIAL PACKAGE"
                    Return "CPP"
                Case "COMMERCIAL PROPERTY"
                    Return "CPR"
                Case Else
                    Return ""
            End Select
        End Function

        Private Function GetAvailableActionsForQuote(isStaff As Boolean) As Dictionary(Of String, String)
            Dim qqHelper As New QuickQuoteHelperClass()
            'Dim IsStaff As Boolean = qqHelper.IsHomeOfficeStaffUser()
            ' VIEW ACTIONS
            Dim actions As New Dictionary(Of String, String)
            ' based on the status id allow these items
            ' you will use these items in a dropdown list

            '*************** CAUTION **************************
            ' *** Do Not change the action text there are *****
            ' *** case statement that rely on their exact *****
            ' *** current text to work                    *****
            '*************** CAUTION **************************

            actions.Add("-- Select --", Me.QuoteId.ToString())

            If Me.IsActiveConsumerQuote Then ' Matt A 9-30-2016
                actions.Add("Acquire Quote", Me.QuoteId.ToString())
                Return actions
            End If

            Dim LOBType As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType = QuickQuote.CommonMethods.QuickQuoteHelperClass.LobTypeForLobId(LobId) 'moved here 11/20/2020 (Interoperability) from below

            If Me.IsCurrentlyInVR = False Then
                actions.Add("Move to VelociRater", Me.QuoteNumber.ToString())
            Else
                If String.IsNullOrWhiteSpace(Me.PolicyNumber) = False AndAlso Me.PolicyNumber Like "Q*" = False AndAlso Me.StatusId <> 13 Then '  Me.QuoteNumber.Contains("(")
                    If LOBHasPortal() Then actions.Add("Portal", Me.QuoteId.ToString())
                    'If Me.LobId <> "17" Then ' Farm does not exist in Portal
                    '    actions.Add("Portal", Me.QuoteId.ToString())
                    'End If

                    actions.Add("Copy", Me.QuoteId.ToString())
                    'DELETE
                    Select Case Me.StatusId
                        Case 12
                            If isStaff Then
                                actions.Add("Undelete", Me.QuoteId.ToString())
                            End If
                        Case Else
                            actions.Add("Archive", Me.QuoteId.ToString())
                    End Select
                Else
                    If Not isDiamondUpdate(LOBType) Then
                        Dim statusList As New List(Of Integer) From {3, 6, 7, 9, 10, 17, 8, 11}
                        If isCommercialQuote _
                            AndAlso Me.VR_LastModified <= Date.Now.AddDays(-DaysAgoForPolicyToRestartAtPolicyHolder) _
                            AndAlso Me.ItemType = QuoteSearch.ItemType.Quote _
                            AndAlso statusList.Contains(Me.StatusId) Then
                            'This only qualifies for Edit Quote. Ignore lower options.
                            If Me.CreatedTimestamp > Date.Now.AddDays(-DaysAgoForForcedQuoteArchivingOrCopy) Then
                                actions.Add("Edit Quote", Me.QuoteId.ToString())
                            End If
                        ElseIf ((LOBType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.UmbrellaPersonal _
                            OrElse LOBType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal _
                            OrElse LOBType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal _
                            OrElse LOBType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal _
                            OrElse LOBType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm) OrElse isCommercialQuote()) _
                            AndAlso Me.CreatedTimestamp <= Date.Now.AddDays(-DaysAgoForForcedQuoteArchivingOrCopy) _
                            AndAlso Me.ItemType = QuoteSearch.ItemType.Quote Then
                            'Do not add edit option. This does not qualify for the lower options
                        Else
                            Select Case Me.StatusId
                                Case 2, 4, 5, 15 'updated 8/17/2017 for Quote Stopped (15)
                                    'edit page
                                    'actions.Add("View/Edit", Me.QuoteId.ToString())
                                    actions.Add("Edit Quote", Me.QuoteId.ToString())   ' Task 43589 MGB 4/22/2020
                                Case 3, 6 ',8, 11  '5-21-14
                                    ' quote summary
                                    actions.Add("Quote Summary", Me.QuoteId.ToString())
                                    If Me.StatusId = 3 Then
                                        If commSystemLobs.Contains(Me.LobId) Then
                                            actions.Add("Proposal", Me.QuoteId.ToString())
                                        End If
                                    End If
                                Case 7, 9, 10, 17 ', 8, 11 ' added ,8, 11 on 5-21; updated 8/17/2017 for App Stopped (17)
                                    ' app gap
                                    actions.Add("Application", Me.QuoteId.ToString())
                                Case 8, 11 ' on 5-21
                                    ' app gap
                                    actions.Add("App Summary", Me.QuoteId.ToString())
                                Case 14
                                    ' portal for staff
                                    If LOBHasPortal() Then actions.Add("Portal", Me.QuoteId.ToString())
                                    'If Me.LobId <> "17" Then ' Farm does not exist in Portal
                                    '    actions.Add("Portal", Me.QuoteId.ToString())
                                    'End If
                                Case Else

                            End Select
                        End If

                    End If

                    'COPY
                    Select Case Me.StatusId
                        Case 1, 13
                            If Me.StatusId = 13 AndAlso (isStaff OrElse (String.IsNullOrWhiteSpace(Me.PolicyNumber) = False AndAlso Me.PolicyNumber Like "Q*" = False)) Then '1 = error, 13 = referred to underwriting
                                actions.Add("Copy", Me.QuoteId.ToString())
                            End If
                        ' if 13 but not staff then no copy
                        Case 12
                            'deleted
                            ' do nothing
                        Case Else
                            actions.Add("Copy", Me.QuoteId.ToString())
                    End Select

                    'DELETE
                    Select Case Me.StatusId
                        Case 1, 13, 14
                            ' no archive
                        Case 12
                            actions.Add("Undelete", Me.QuoteId.ToString())
                        Case Else
                            actions.Add("Archive", Me.QuoteId.ToString())
                    End Select

                    'Proposal
                    Select Case Me.StatusId
                        Case 3
                            'actions.Add("Proposal", Me.QuoteId.ToString())
                        Case Else

                    End Select

                    If isDiamondUpdate(LOBType) Then
                        'actions.Add("Update from Diamond", Me.QuoteNumber.ToString())
                        'updated 4/16/2024 to not allow agents to take back routed quotes while staff is still working on them
                        Dim isOkayToUpdate As Boolean = True
                        If Me.StatusId = 13 AndAlso Me.HasStaffTransUser = True AndAlso isStaff = False Then '13 = referred to underwriting; currently acquired by staff; current user is not staff
                            isOkayToUpdate = False
                        End If
                        If isOkayToUpdate = True Then
                            actions.Add("Update from Diamond", Me.QuoteNumber.ToString())
                        End If
                    End If
                    'If String.IsNullOrWhiteSpace(Me.PolicyNumber) = False AndAlso Left(Me.PolicyNumber, 1) = "Q" Then 'added IF 3/17/2021 to make sure there's a quote # and that it hasn't already been promoted to a policy
                    '        'If VRFeatures.AllowVRToUpdateFromDiamond_Interoperability Then 'added IF 11/10/2020 (Interoperability); original logic in ELSE
                    '        'updated 11/20/2020 to use QQ method that takes lobType
                    '        If QuickQuoteHelperClass.AllowVRToUpdateFromDiamond_Interoperability(lobType:=LOBType) = True Then 'added IF 11/10/2020 (Interoperability); original logic in ELSE
                    '            If Me.WasLastModifiedInVR = False AndAlso Me.StatusId <> 12 Then
                    '                actions.Add("Update from Diamond", Me.QuoteNumber.ToString())
                    '            End If
                    '        Else
                    '            If VRFeatures.AllowVrToUpdateFromDiamond Then
                    '                'If Me.WasLastModifiedInVR = False And Me.StatusId <> 12 And LobId.EqualsAny(1, 2) Then
                    '                'updated 11/10/2020 (Interoperability); to make this specific to CompRater, which should've been the original intent
                    '                If Me.OriginatedInVR = False AndAlso Me.WasLastModifiedInVR = False AndAlso Me.StatusId <> 12 AndAlso LobId.EqualsAny(1, 2) Then
                    '                    actions.Add("Update from Diamond", Me.QuoteNumber.ToString())
                    '               End If
                    '            End If
                    '       End If
                    'End If
                End If
            End If

            'Dim LOBType As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType = QuickQuote.CommonMethods.QuickQuoteHelperClass.LobTypeForLobId(LobId) 'moved up above 11/20/2020 (Interoperability) so other functionality can use it
            Dim myDict As New Dictionary(Of QuickQuote.CommonMethods.QuickQuoteHelperClass.LOBIFMVersions, String)
            If qqHelper.HasIFMLOBVersionUncrossableDateLineBeforeEffectiveDateAndIsOutsideAcceptableEffectiveDateRange(StateIdsToListOfString(), LOBType, Me.EffectiveDate, myDict) Then
                ' This was intended for HOM2011 but was affecting PPA - PPA needed the no cross logic for a time but should be fine now
                'If qqHelper.ConvertQQLobIdToQQLobType(LobId) <> QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal Then
                'updated 11/20/2020 (Interoperability) to use the existing variable
                If LOBType <> QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal Then
                    actions.Remove("Copy")
                End If
            End If

            Return actions
        End Function

        Private Function isDiamondUpdate(LOBType As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType) As Boolean
            If String.IsNullOrWhiteSpace(Me.PolicyNumber) = False AndAlso Left(Me.PolicyNumber, 1) = "Q" Then 'added IF 3/17/2021 to make sure there's a quote # and that it hasn't already been promoted to a policy
                If QuickQuoteHelperClass.AllowVRToUpdateFromDiamond_Interoperability(lobType:=LOBType) = True Then 'added IF 11/10/2020 (Interoperability); original logic in ELSE
                    If Me.WasLastModifiedInVR = False AndAlso Me.StatusId <> 12 Then
                        Return True
                    End If
                Else
                    If VRFeatures.AllowVrToUpdateFromDiamond Then
                        If Me.OriginatedInVR = False AndAlso Me.WasLastModifiedInVR = False AndAlso Me.StatusId <> 12 AndAlso LobId.EqualsAny(1, 2) Then
                            Return True
                        End If
                    End If
                End If
            End If
            Return False
        End Function

        Private Function GetAvailableActionsForPolicy() As Dictionary(Of String, String)
            ' VIEW ACTIONS
            Dim actions As New Dictionary(Of String, String)

            actions.Add("-- Select --", Me.QuoteId.ToString())

            actions.Add("View Policy", Me.PolicyId)

            Select Case Me.PolicyCurrentStatusID
                Case 1, 2
                    If IsInCancelWarning = False AndAlso IsInFinalCancelWarning = False Then
                        actions.Add("Process Change", Me.PolicyId)
                        'Updated 09/24/2021 for BOP Endorsements task 61263 MLW
                        'Updated 11/13/2020 for CAP Endorsements task 52968 MLW
                        'If (Me.LobId <> 20 AndAlso Me.LobId <> 44) Then '20 is CAP single state, 44 is CAP multi-state
                        If AllowBillingChange(Me.LobId) Then
                            actions.Add("Billing Change", Me.PolicyId)
                        End If
                    End If
            End Select

            Return actions
        End Function

        Private Function GetAvailableActionsForChange() As Dictionary(Of String, String)
            ' VIEW ACTIONS
            Dim actions As New Dictionary(Of String, String)

            actions.Add("-- Select --", Me.QuoteId.ToString())
            actions.Add("View Policy", Me.PolicyId)

            Dim NotAllowedToViewEdit As New List(Of String)
            NotAllowedToViewEdit.Add("Referred to Underwriter")
            NotAllowedToViewEdit.Add("Finalized Change")

            If NotAllowedToViewEdit.Contains(Me.FriendlyStatus) = False Then
                'actions.Add("View/Edit", Me.PolicyId.ToString())
                'Updated 09/24/2021 for BOP Endorsements Task 61263 MLW
                If AllowEditChangeAndDelete(LobId) Then
                    actions.Add("Edit Change", Me.PolicyId.ToString()) ' Task 43589 MGB 4/22/2020
                End If
                ''Updated 01/15/2021 for CAP Endorsements Task 52968 MLW
                'If (LobId <> 20 AndAlso LobId <> 44) OrElse (LobId = 20 AndAlso StatusId > 0) OrElse (LobId = 44 AndAlso StatusId > 0) Then '20 CAP single state, 44 CAP multi state
                '    actions.Add("Edit Change", Me.PolicyId.ToString()) ' Task 43589 MGB 4/22/2020
                'End If
                ''actions.Add("Edit Change", Me.PolicyId.ToString())  ' Task 43589 MGB 4/22/2020
            End If

            Dim NotAllowedToDelete As New List(Of String)
            NotAllowedToDelete.Add("Finalized Change")

            If NotAllowedToDelete.Contains(Me.FriendlyStatus) = False Then
                'Updated 09/24/2021 for BOP Endorsements Task 61263 MLW
                If AllowEditChangeAndDelete(LobId) Then
                    actions.Add("Delete", Me.PolicyId)
                End If
                ''Updated 06/15/2021 for CAP Endorsements Bug 62580 MLW - NOTE: This will be updated in a later task for all LOBs, probably remove LobId and just use StatusId > 0 for the Diamond origin check (if StatusId > 0 then)
                'If (LobId <> 20 AndAlso LobId <> 44) OrElse (LobId = 20 AndAlso StatusId > 0) OrElse (LobId = 44 AndAlso StatusId > 0) Then '20 CAP single state, 44 CAP multi state
                '    actions.Add("Delete", Me.PolicyId)
                'End If
                ''actions.Add("Delete", Me.PolicyId)
            End If

            Return actions
        End Function

        Private Function GetAvailableActionsForBillingUpdate() As Dictionary(Of String, String)
            ' VIEW ACTIONS
            Dim actions As New Dictionary(Of String, String)

            actions.Add("-- Select --", Me.QuoteId.ToString())
            actions.Add("View Policy", Me.PolicyId)

            Dim NotAllowedToViewEdit As New List(Of String)
            NotAllowedToViewEdit.Add("Referred to Underwriter")
            NotAllowedToViewEdit.Add("Finalized Billing Update")

            If NotAllowedToViewEdit.Contains(Me.FriendlyStatus) = False Then
                'actions.Add("View/Edit", Me.PolicyId.ToString())
                actions.Add("Edit Change", Me.PolicyId.ToString())  ' T46340 CAH

            End If

            actions.Add("Delete", Me.PolicyId)

            Return actions
        End Function

        Private Function GetViewURLForQuote() As String
            If String.IsNullOrWhiteSpace(Me.PolicyNumber) = False AndAlso Me.StatusId <> 13 AndAlso Me.PolicyNumber Like "Q*" = False Then '  Me.QuoteNumber.Contains("(")
                'Portal
                Select Case Me.LobId
                    Case "1"
                        'auto
                        Return System.Configuration.ConfigurationManager.AppSettings("DiamondPortalMainPageLink") + "?QuickQuoteNumber=" + Me.PolicyNumber
                    Case "2"
                        'home
                        Return System.Configuration.ConfigurationManager.AppSettings("DiamondPortalMainPageLink") + "?QuickQuoteNumber=" + Me.PolicyNumber
                    Case "3"
                        'DFR Personal
                        Return System.Configuration.ConfigurationManager.AppSettings("DiamondPortalMainPageLink") + "?QuickQuoteNumber=" + Me.PolicyNumber
                    Case "17"
                        'Farm - No Portal
                        Return ""
                    Case Else
                        Return System.Configuration.ConfigurationManager.AppSettings("DiamondPortalMainPageLink") + "?QuickQuoteNumber=" + Me.PolicyNumber
                End Select
            Else
                Select Case Me.StatusId
                    Case 2, 4, 5, 15 'updated 8/17/2017 for Quote Stopped (15)
                        'edit page
                        Select Case Me.LobId
                            Case "1"
                                'auto
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_PPA_Input") + "?quoteid=" + QuoteId.ToString()
                            Case "2"
                                'home
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_HOM_Input") + "?quoteid=" + QuoteId.ToString()
                            Case "3"
                                'DFR Personal
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_DFR_Input") + "?quoteid=" + QuoteId.ToString()
                            Case "17"
                                'Farm
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_FAR_Input") + "?quoteid=" + QuoteId.ToString()
                            Case "9"
                                ''CGL
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CGL_Quote_NewLook") + "?quoteid=" + QuoteId.ToString()
                            Case "20"
                                'CAP
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CAP_Quote_NewLook") + "?quoteid=" + QuoteId.ToString()
                            Case "21"
                                'WCP
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_WCP_Quote_NewLook") + "?quoteid=" + QuoteId.ToString()
                            Case "23"
                                'CPP
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CPP_Quote_NewLook") + "?quoteid=" + QuoteId.ToString()
                            Case "25"
                                'BOP
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_BOP_Quote_NewLook") + "?quoteid=" + QuoteId.ToString()
                            Case "28"
                                'CPR
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CPR_Quote_NewLook") + "?quoteid=" + QuoteId.ToString()
                            Case "14"
                                'FUP/PUP
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_FUPPUP_Input") + "?quoteid=" + QuoteId.ToString()
                            Case Else

                        End Select
                    Case 3, 6 ',8, 11  '5-21-14
                        ' quote summary
                        Select Case Me.LobId
                            Case "1"
                                'auto
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_PPA_Input") + "?quoteid=" + QuoteId.ToString() + "&" + IFM.VR.Common.Workflow.Workflow.WorkFlowSection_qs + "=" + IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary.ToString()                                    '
                            Case "2"
                                'home
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_HOM_Input") + "?quoteid=" + QuoteId.ToString() + "&" + IFM.VR.Common.Workflow.Workflow.WorkFlowSection_qs + "=" + IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary.ToString()
                            Case "3"
                                'DFR Personal
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_DFR_Input") + "?quoteid=" + QuoteId.ToString() + "&" + IFM.VR.Common.Workflow.Workflow.WorkFlowSection_qs + "=" + IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary.ToString()
                            Case "17"
                                'Farm
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_FAR_Input") + "?quoteid=" + QuoteId.ToString() + "&" + IFM.VR.Common.Workflow.Workflow.WorkFlowSection_qs + "=" + IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary.ToString()
                            Case "9"
                                ''CGL
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CGL_Quote_NewLook") + "?quoteid=" + QuoteId.ToString() + "&" + IFM.VR.Common.Workflow.Workflow.WorkFlowSection_qs + "=" + IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary.ToString()
                            Case "25"
                                'BOP
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_BOP_Quote_NewLook") + "?quoteid=" + QuoteId.ToString() + "&" + IFM.VR.Common.Workflow.Workflow.WorkFlowSection_qs + "=" + IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary.ToString()
                            Case "20"
                                ' CAP
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CAP_Quote_NewLook") + "?quoteid=" + QuoteId.ToString() + "&" + IFM.VR.Common.Workflow.Workflow.WorkFlowSection_qs + "=" + IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary.ToString()
                            Case "21"
                                ' WCP
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_WCP_Quote_NewLook") + "?quoteid=" + QuoteId.ToString() + "&" + IFM.VR.Common.Workflow.Workflow.WorkFlowSection_qs + "=" + IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary.ToString()
                            Case "28"
                                'CPR
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CPR_Quote_NewLook") + "?quoteid=" + QuoteId.ToString() + "&" + IFM.VR.Common.Workflow.Workflow.WorkFlowSection_qs + "=" + IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary.ToString()
                            Case "23"
                                'CPP
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CPP_Quote_NewLook") + "?quoteid=" + QuoteId.ToString() + "&" + IFM.VR.Common.Workflow.Workflow.WorkFlowSection_qs + "=" + IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary.ToString()
                            Case "14"
                                'FUP/PUP
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_FUPPUP_Input") + "?quoteid=" + QuoteId.ToString() + "&" + IFM.VR.Common.Workflow.Workflow.WorkFlowSection_qs + "=" + IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary.ToString()
                            Case Else
                                ' All other Com LOBS
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_QuoteSummary") + "?quoteid=" + QuoteId.ToString()
                        End Select
                    Case 7, 9, 10, 17 ', 8, 11 ' added ,8, 11 on 5-21; updated 8/17/2017 for App Stopped (17)
                        ' app gap
                        Select Case Me.LobId
                            Case "1"
                                'auto
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_PPA_App") + "?quoteid=" + QuoteId.ToString()
                            Case "2"
                                'home
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_HOM_App") + "?quoteid=" + QuoteId.ToString()
                            Case "3"
                                'DFR Personal
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_DFR_App") + "?quoteid=" + QuoteId.ToString()
                            Case "17"
                                'Farm
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_FAR_APP") + "?quoteid=" + QuoteId.ToString()
                            Case "9"
                                ''CGL
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CGL_App_NewLook") + "?quoteid=" + QuoteId.ToString()
                            Case "20"
                                'CAP
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CAP_App_NewLook") + "?quoteid=" + QuoteId.ToString() + "&" + IFM.VR.Common.Workflow.Workflow.WorkFlowSection_qs + "=" + IFM.VR.Common.Workflow.Workflow.WorkflowSection.app.ToString()
                            Case "21"
                                'WCP
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_WCP_App_NewLook") + "?quoteid=" + QuoteId.ToString()
                            Case "23"
                                'CPP
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CPP_App_NewLook") + "?quoteid=" + QuoteId.ToString()
                            Case "25"
                                'BOP
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_BOP_App_NewLook") + "?quoteid=" + QuoteId.ToString()
                            Case "28"
                                'CPR
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CPR_App_NewLook") + "?quoteid=" + QuoteId.ToString()
                            Case "14"
                                'FUP/PUP
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_FUPPUP_App") + "?quoteid=" + QuoteId.ToString()
                            Case Else

                        End Select
                    Case 8, 11 ' on 5-21
                        ' app gap summary
                        Select Case Me.LobId
                            Case "1"
                                'auto
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_PPA_App") + "?quoteid=" + QuoteId.ToString() + "&" + IFM.VR.Common.Workflow.Workflow.WorkFlowSection_qs + "=" + IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary.ToString()
                            Case "2"
                                'home
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_HOM_App") + "?quoteid=" + QuoteId.ToString() + "&" + IFM.VR.Common.Workflow.Workflow.WorkFlowSection_qs + "=" + IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary.ToString()
                            Case "3"
                                'DFR Personal
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_DFR_App") + "?quoteid=" + QuoteId.ToString() + "&" + IFM.VR.Common.Workflow.Workflow.WorkFlowSection_qs + "=" + IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary.ToString()
                            Case "17"
                                'Farm
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_FAR_APP") + "?quoteid=" + QuoteId.ToString() + "&" + IFM.VR.Common.Workflow.Workflow.WorkFlowSection_qs + "=" + IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary.ToString()

                            Case "9"
                                ''CGL
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CGL_App_NewLook") + "?quoteid=" + QuoteId.ToString() + "&" + IFM.VR.Common.Workflow.Workflow.WorkFlowSection_qs + "=" + IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary.ToString()
                            Case "25"
                                'BOP
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_BOP_App_NewLook") + "?quoteid=" + QuoteId.ToString() + "&" + IFM.VR.Common.Workflow.Workflow.WorkFlowSection_qs + "=" + IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary.ToString()
                            Case "20"
                                ''CAP
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CAP_App_NewLook") + "?quoteid=" + QuoteId.ToString() + "&" + IFM.VR.Common.Workflow.Workflow.WorkFlowSection_qs + "=" + IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary.ToString()
                            Case "21"
                                'WCP
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_WCP_App_NewLook") + "?quoteid=" + QuoteId.ToString() + "&" + IFM.VR.Common.Workflow.Workflow.WorkFlowSection_qs + "=" + IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary.ToString()
                            Case "28"
                                'CPR
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CPR_App_NewLook") + "?quoteid=" + QuoteId.ToString() + "&" + IFM.VR.Common.Workflow.Workflow.WorkFlowSection_qs + "=" + IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary.ToString()
                            Case "23"
                                'CPP
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_CPP_App_NewLook") + "?quoteid=" + QuoteId.ToString() + "&" + IFM.VR.Common.Workflow.Workflow.WorkFlowSection_qs + "=" + IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary.ToString()
                            Case "14"
                                'FUP/PUP
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_FUPPUP_App") + "?quoteid=" + QuoteId.ToString() + "&" + IFM.VR.Common.Workflow.Workflow.WorkFlowSection_qs + "=" + IFM.VR.Common.Workflow.Workflow.WorkflowSection.summary.ToString()
                            Case Else '5-29-15 Matt A
                                ' All other Com LOBS
                                Return System.Configuration.ConfigurationManager.AppSettings("QuickQuote_QuoteSummary") + "?quoteid=" + QuoteId.ToString()

                        End Select

                    Case 14
                        ' portal for staff
                        'actions.Add("Portal", Me.QuoteId.ToString())
                        Return System.Configuration.ConfigurationManager.AppSettings("DiamondPortalMainPageLink") + "?QuickQuoteNumber=" + If(String.IsNullOrWhiteSpace(Me.PolicyNumber) = False, Me.PolicyNumber, Me.QuoteNumber)
                    Case Else
                End Select
            End If

            Return ""
        End Function

        'Added 09/24/2021 for BOP Endorsements task 61263 MLW
        Private Function AllowBillingChange(lobId As Integer) As Boolean
            Select Case lobId
                Case 20, 23, 44, 25, 48 '20 CAP single state, 44 CAP multi-state, 25 BOP single state, 48 BOP multi-state
                    Return False
                Case Else
                    Return True
            End Select
        End Function
        'Added 09/24/2021 for BOP Endorsements task 61263 MLW
        Private Function AllowEditChangeAndDelete(lobId As Integer) As Boolean
            Select Case lobId
                Case 20, 23, 44, 25, 48 '20 CAP single state, 44 CAP multi state, 25 BOP single state, 48 BOP multi-state
                    If StatusId > 0 Then
                        Return True
                    Else
                        Return False
                    End If
                Case Else
                    Return True
            End Select
        End Function
    End Class

    <Serializable>
    Public Class QQSearchParameters
        Implements ICloneable
        Public Property AgencyID As Integer = 0
        Public Property quoteOrPolicyID As Integer = 0
        Public Property QuoteOrPolicyNumber As String = ""
        Public Property ClientName As String = ""
        Public Property LobID As Integer = 0
        Public Property LobIDs As String = ""
        Private _LobIdsList As List(Of Integer)
        Public Property LobIDsList As List(Of Integer)
            Get
                Return _LobIdsList
            End Get
            Set(value As List(Of Integer))
                _LobIdsList = value
                If _LobIdsList IsNot Nothing AndAlso _LobIdsList.Count > 0 Then
                    If _LobIdsList.Count > 1 Then
                        LobIDs = _LobIdsList.ListToCSV()
                    Else
                        LobIDs = _LobIdsList(0)
                    End If
                End If
            End Set
        End Property
        Public Property ExcludedLobIds As String = ""
        Public Property IsStaff As Boolean = False
        Public Property AllowResultsNotCurrentlyInVR As Boolean = False
        Public Property StatusIDs As String = ""
        Public Property AgentUserName As String = ""
        Public Property ShowArchived As Boolean = False
        Public Property TimeFrame As Integer = 0
        Public Property ShowAllOnPage As Boolean = False
        Public Property SearchType As IFM.VR.Common.QuoteSearch.QuoteSearch.SearchType = QuoteSearch.SearchType.NA
        Public Property AgentUserID As Integer = 0
        Public Property ClientID As Integer = 0
        Public Property CurrentDiamondUserID As Integer = 0
        Public Property HasEmployeeAccess As Boolean = False
        Public Property NumberOfResultsToDisplay As Integer = 0
        Public Property NonEndorsementReadyLobIds As List(Of Integer)
        Public Property searchInitiatedByAgencySwitch As Boolean = False
        Public Property isGlobalSearch As Boolean = False
        Public Property isSavedPage As Boolean = False
        Public Property excludeSavedQuotes As Boolean = False 'added 3/9/2021
        Public ReadOnly Property StoredProcedureName As String
            Get
                Select Case SearchType
                    Case QuoteSearch.SearchType.All
                        Return "usp_MyVelocirater_Search_Global"
                    Case QuoteSearch.SearchType.BillingUpdates, QuoteSearch.SearchType.Changes
                        Return "usp_MyVelocirater_Search_Changes"
                    Case QuoteSearch.SearchType.Policies
                        Return "usp_MyVelocirater_Search_PoliciesAndChangesCombined"
                    Case QuoteSearch.SearchType.Quotes
                        Return "usp_MyVelocirater_Search_Quotes"
                    Case Else
                        Return ""
                End Select
            End Get
        End Property

        Public Function Clone() As Object Implements ICloneable.Clone
            Dim returnObj As New QQSearchParameters

            If Me IsNot Nothing Then
                returnObj = Me.MemberwiseClone
            End If
            'Throw New NotImplementedException()
            Return returnObj
        End Function
    End Class

End Namespace