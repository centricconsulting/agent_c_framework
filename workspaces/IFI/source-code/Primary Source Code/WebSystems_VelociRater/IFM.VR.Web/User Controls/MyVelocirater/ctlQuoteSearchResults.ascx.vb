Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.QuoteSearch

Public Class ctlQuoteSearchResults
    Inherits System.Web.UI.UserControl

    Public Enum ResultDisplayType As Integer
        Undefined = 0
        Auto = 1
        Home = 2
        Comm = 3
        Farm = 4
        Umbrella = 5
    End Enum

    Private Property SearchParameterObjectPassed As Boolean = False
    Public Property NonEndorsementReadyLobIds As New List(Of Integer)
    Private Property EndorsementReadyLobIds As New List(Of Integer)
    Public ReadOnly Property SearchableLobIds As List(Of Integer)
        Get
            'NonEndorsementReadyLobIds = New List(Of Integer)
            'EndorsementReadyLobIds = New List(Of Integer)

            Dim lobids As New List(Of Integer)
            Dim doCheck As Boolean = True
            Dim isEndorsementFunctionality As Boolean = False

            If Me.QuoteNumber.NoneAreNullEmptyorWhitespace AndAlso QuoteNumberMatchesLOBType() = False Then
                doCheck = False
            End If

            'Check if we are doing an endorsement related search, determine if this LOB is OK
            Select Case Me.SearchType
                Case Common.QuoteSearch.QuoteSearch.SearchType.Quotes
                    isEndorsementFunctionality = False
                Case Common.QuoteSearch.QuoteSearch.SearchType.Policies, Common.QuoteSearch.QuoteSearch.SearchType.Changes, Common.QuoteSearch.QuoteSearch.SearchType.BillingUpdates, Common.QuoteSearch.QuoteSearch.SearchType.All
                    isEndorsementFunctionality = True
            End Select

            If doCheck Then
                Select Case Me.ResultType
                    Case ResultDisplayType.Auto
                        If VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs().Values.Contains(1) Then
                            If LobID = 0 Or LobID = IFM.VR.Common.QuoteSearch.QuoteSearch.LobCategory.Personal Then
                                ' searching by category not a real LOBid
                                lobids.Add(1)
                            Else
                                'searching for a specific lobID
                                If VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs().Values.Contains(LobID) Then
                                    ' is it a specific lobid that this section cares about ??
                                    If {1}.Contains(LobID) Then
                                        lobids.Add(LobID)
                                    End If
                                End If
                            End If
                        End If
                    Case ResultDisplayType.Comm
                        If LobID = 0 Or LobID = IFM.VR.Common.QuoteSearch.QuoteSearch.LobCategory.Commercial Then
                            ' searching by category not a real LOBid
                            lobids = IFM.VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs(Common.QuoteSearch.QuoteSearch.LobCategory.Commercial).Values.ToList()
                        Else
                            'searching for a specific lobID
                            If VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs().Values.Contains(LobID) Then
                                ' is it a specific lobid that this section cares about ??
                                If IFM.VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs(Common.QuoteSearch.QuoteSearch.LobCategory.Commercial).Values.ToArray().Contains(LobID) Then
                                    lobids.Add(LobID)
                                    'Added 08/25/2021 for Bug 64324 MLW
                                    Dim multiStateLobId As Integer = QuickQuoteHelperClass.MasterLobIdForLobId(LobID)
                                    If multiStateLobId > 0 AndAlso multiStateLobId <> LobID AndAlso lobids.Contains(multiStateLobId) = False Then
                                        lobids.Add(multiStateLobId)
                                    End If
                                End If
                            End If
                        End If

                    Case ResultDisplayType.Farm
                        If VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs().Values.Contains(17) Then
                            If LobID = 0 Or LobID = IFM.VR.Common.QuoteSearch.QuoteSearch.LobCategory.Farm Then
                                ' searching by category not a real LOBid
                                ' lobids.Add(17)

                                lobids = IFM.VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs(Common.QuoteSearch.QuoteSearch.LobCategory.Farm).Values.ToList()
                            Else
                                'searching for a specific lobID
                                If VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs().Values.Contains(LobID) Then
                                    ' is it a specific lobid that this section cares about ??
                                    If IFM.VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs(Common.QuoteSearch.QuoteSearch.LobCategory.Farm).Values.ToArray().Contains(LobID) Then
                                        lobids.Add(LobID)
                                        'Added 08/25/2021 for Bug 64324 MLW
                                        Dim multiStateLobId As Integer = QuickQuoteHelperClass.MasterLobIdForLobId(LobID)
                                        If multiStateLobId > 0 AndAlso multiStateLobId <> LobID AndAlso lobids.Contains(multiStateLobId) = False Then
                                            lobids.Add(multiStateLobId)
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    Case ResultDisplayType.Home
                        If LobID = 0 Or LobID = IFM.VR.Common.QuoteSearch.QuoteSearch.LobCategory.Personal Or LobID = Common.QuoteSearch.QuoteSearch.LobCategory.PersonalPropertyandLiability Then
                            ' searching by category not a real LOBid
                            lobids = IFM.VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs(Common.QuoteSearch.QuoteSearch.LobCategory.PersonalPropertyandLiability).Values.ToList()
                        Else
                            'searching for a specific lobID
                            If VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs().Values.Contains(LobID) Then
                                ' is it a specific lobid that this section cares about ??
                                If IFM.VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs(Common.QuoteSearch.QuoteSearch.LobCategory.PersonalPropertyandLiability).Values.ToArray().Contains(LobID) Then
                                    lobids.Add(LobID)
                                End If
                            End If
                        End If
                    Case ResultDisplayType.Umbrella
                        If VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs().Values.Contains(14) Then
                            If LobID = 0 Or LobID = IFM.VR.Common.QuoteSearch.QuoteSearch.LobCategory.Umbrella Then
                                ' searching by category not a real LOBid
                                lobids.Add(14)
                            Else
                                'searching for a specific lobID
                                If VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs().Values.Contains(LobID) Then
                                    ' is it a specific lobid that this section cares about ??
                                    If IFM.VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs(Common.QuoteSearch.QuoteSearch.LobCategory.Umbrella).Values.ToArray().Contains(LobID) Then
                                        lobids.Add(LobID)
                                    End If
                                End If
                            End If
                        End If
                End Select
            End If

            If isEndorsementFunctionality Then
                If lobids IsNot Nothing AndAlso lobids.Count > 0 Then
                    EndorsementReadyLobIds = New List(Of Integer)
                    NonEndorsementReadyLobIds = New List(Of Integer)
                    For Each thisLobID As Integer In lobids
                        If IFM.VR.Common.Helpers.LOBHelper.LOBIsInCSVConfigKey(thisLobID, "VR_LOBsReadyForEndorsements") Then
                            EndorsementReadyLobIds.Add(thisLobID)
                        Else
                            NonEndorsementReadyLobIds.Add(thisLobID)
                        End If
                    Next
                End If
                SearchParameters.LobIDs = EndorsementReadyLobIds.ListToCSV()
                SearchParameters.NonEndorsementReadyLobIds = NonEndorsementReadyLobIds
                Return EndorsementReadyLobIds
            Else
                SearchParameters.LobIDs = lobids.ListToCSV()
                Return lobids
            End If

        End Get
    End Property

    Public ReadOnly Property SectionName As String
        Get
            Dim sectionTitle As String = ""
            Dim OHStartDate As DateTime = IFM.VR.Common.Helpers.GenericHelper.GetOhioEffectiveDate()
            Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
            Select Case Me.ResultType
                Case ResultDisplayType.Auto
                    sectionTitle = IFM.VR.Common.QuoteSearch.QuoteSearch.GetLobNameFromId(1)
                    If (IFM.VR.Common.Helpers.MultiState.General.VRMultiStateSupportEnabled()) Then
                        If qqHelper.BitToBoolean(System.Configuration.ConfigurationManager.AppSettings("VR_PPA_OH_Enabled")) Then
                            Me.lblLobNameAdditional.Text = "(IN only, IL and OH if customer has an existing Farm Policy)"
                        Else
                            Me.lblLobNameAdditional.Text = "(IN only, IL if customer has an existing Farm Policy)"
                        End If
                    End If
                Case ResultDisplayType.Comm
                    sectionTitle = "Commercial"
                    If (IFM.VR.Common.Helpers.MultiState.General.VRMultiStateSupportEnabled()) Then
                        'Updated 3/14/2022 for KY WCP Task 73875 MLW
                        Dim chc As New CommonHelperClass
                        If DateTime.Today >= OHStartDate Then
                            If IFM.VR.Common.Helpers.MultiState.General.KentuckyWCPGovStateEnabled = True Then
                                If Request IsNot Nothing AndAlso Request.QueryString("PageView") IsNot Nothing AndAlso chc.StringsAreEqual(Me.Request("PageView"), "savedQuotes") Then
                                    Me.lblLobNameAdditional.Text = "(IN, IL, OH and KY-WC Only)"
                                Else
                                    Me.lblLobNameAdditional.Text = "(IN, IL and OH)"
                                End If
                            Else
                                Me.lblLobNameAdditional.Text = "(IN, IL and OH)"
                            End If
                        Else
                            If IFM.VR.Common.Helpers.MultiState.General.KentuckyWCPGovStateEnabled = True Then
                                If Request IsNot Nothing AndAlso Request.QueryString("PageView") IsNot Nothing AndAlso chc.StringsAreEqual(Me.Request("PageView"), "savedQuotes") Then
                                    Me.lblLobNameAdditional.Text = "(IN, IL and KY-WC Only)"
                                Else
                                    Me.lblLobNameAdditional.Text = "(IN and IL)"
                                End If
                            Else
                                Me.lblLobNameAdditional.Text = "(IN and IL)"
                            End If
                        End If
                        'If DateTime.Today >= OHStartDate Then
                        '    Me.lblLobNameAdditional.Text = "(IN, IL and OH)"
                        'Else
                        '    Me.lblLobNameAdditional.Text = "(IN and IL)"
                        'End If
                    End If
                Case ResultDisplayType.Farm
                    sectionTitle = IFM.VR.Common.QuoteSearch.QuoteSearch.GetLobNameFromId(17)
                    If (IFM.VR.Common.Helpers.MultiState.General.VRMultiStateSupportEnabled()) Then
                        If DateTime.Today >= OHStartDate Then
                            Me.lblLobNameAdditional.Text = "(IN, IL and OH)"
                        Else
                            Me.lblLobNameAdditional.Text = "(IN and IL)"
                        End If
                    End If
                Case ResultDisplayType.Home
                    sectionTitle = "Personal Property and Liability"
                    If (IFM.VR.Common.Helpers.MultiState.General.VRMultiStateSupportEnabled()) Then
                        Me.lblLobNameAdditional.Text = "(IN only)"
                    End If
                Case ResultDisplayType.Umbrella
                    sectionTitle = IFM.VR.Common.QuoteSearch.QuoteSearch.GetLobNameFromId(14)
                    'If (IFM.VR.Common.Helpers.MultiState.General.VRMultiStateSupportEnabled()) Then
                    '    Me.lblLobNameAdditional.Text = "(IN and IL)"
                    'End If
            End Select
            Return sectionTitle
        End Get
    End Property

    Public ReadOnly Property SectionImageUrl As String
        Get
            Dim imageUrl As String = ""
            Select Case Me.ResultType
                Case ResultDisplayType.Auto
                    imageUrl = "../../images/dark_auto.png"
                Case ResultDisplayType.Comm
                    imageUrl = "../../images/policy_comm.png"
                Case ResultDisplayType.Farm
                    imageUrl = "../../images/policy_farm.png"
                Case ResultDisplayType.Home
                    imageUrl = "../../images/dark_home.png"
            End Select
            Return imageUrl
        End Get
    End Property

    Public ReadOnly Property SectionImageClass As String
        Get
            Dim imageClass As String = ""
            Select Case Me.ResultType
                Case ResultDisplayType.Auto
                    imageClass = "fas fa-car"
                Case ResultDisplayType.Comm
                    imageClass = "apc-icon-open-sign"
                Case ResultDisplayType.Farm
                    imageClass = "apc-icon-barn"
                Case ResultDisplayType.Home
                    imageClass = "fas fa-home"
                Case ResultDisplayType.Umbrella
                    imageClass = "fas fa-umbrella"
            End Select
            Return imageClass
        End Get
    End Property

    Public Property ResultType As ResultDisplayType

    Private _SearchParameters As Common.QuoteSearch.QQSearchParameters = Nothing
    Public Property SearchParameters As Common.QuoteSearch.QQSearchParameters
        Get
            If _SearchParameters Is Nothing Then
                If ViewState("vs_SearchParameters") IsNot Nothing Then
                    _SearchParameters = ViewState("vs_SearchParameters")
                End If
            End If

            Return _SearchParameters
        End Get
        Set(value As Common.QuoteSearch.QQSearchParameters)
            _SearchParameters = value
            QuoteNumber = _SearchParameters.QuoteOrPolicyNumber
            SearchForQuoteId = _SearchParameters.quoteOrPolicyID
            PolicyNumber = _SearchParameters.QuoteOrPolicyNumber
            ClientName = _SearchParameters.ClientName
            LobID = _SearchParameters.LobID
            StatusIDs = _SearchParameters.StatusIDs
            AgentUserName = _SearchParameters.AgentUserName
            AgentUserID = _SearchParameters.AgentUserID
            ShowArchived = _SearchParameters.ShowArchived
            TimeFrameDays = _SearchParameters.TimeFrame
            ShowAllOnOnePage = _SearchParameters.ShowAllOnPage
            IsSavedPage = _SearchParameters.isSavedPage
            SearchType = _SearchParameters.SearchType
            HasEmployeeAccess = _SearchParameters.HasEmployeeAccess
            IsStaff = _SearchParameters.IsStaff
            CurrentUserDiamondID = _SearchParameters.CurrentDiamondUserID
            _SearchParameters.AllowResultsNotCurrentlyInVR = True
            _SearchParameters.LobIDsList = SearchableLobIds
            _SearchParameters.NonEndorsementReadyLobIds = NonEndorsementReadyLobIds
            SearchParameterObjectPassed = True
            ViewState("vs_SearchParameters") = _SearchParameters
        End Set
    End Property

    Public Property PageSize As Int32
        Get
            Return Me.gridQuoteResults.PageSize
        End Get
        Set(value As Int32)
            Me.gridQuoteResults.PageSize = value
        End Set
    End Property

    Public Property SearchForQuoteId As Int32
        Get
            If ViewState("vs_quoteID") Is Nothing Then
                ViewState("vs_quoteID") = -1
            End If
            Return ViewState("vs_quoteID")
        End Get
        Set(value As Int32)
            ViewState("vs_quoteID") = value
        End Set
    End Property

    Public Property QuoteNumber As String
        Get
            If ViewState("vs_quoteNum") Is Nothing Then
                ViewState("vs_quoteNum") = ""
            End If
            Return ViewState("vs_quoteNum")
        End Get
        Set(value As String)
            ViewState("vs_quoteNum") = value
        End Set
    End Property

    Public Property PolicyNumber As String
        Get
            If ViewState("vs_policyNum") Is Nothing Then
                ViewState("vs_policyNum") = ""
            End If
            Return ViewState("vs_policyNum")
        End Get
        Set(value As String)
            ViewState("vs_policyNum") = value
        End Set
    End Property

    Public Property ClientName As String
        Get
            If ViewState("vsClientName") Is Nothing Then
                ViewState("vsClientName") = ""
            End If
            Return ViewState("vsClientName")
        End Get
        Set(value As String)
            ViewState("vsClientName") = value
        End Set
    End Property

    Public Property LobID As Int32
        Get
            If ViewState("vsLobID") Is Nothing Then
                ViewState("vsLobID") = 0
            End If
            Return ViewState("vsLobID")
        End Get
        Set(value As Int32)
            ViewState("vsLobID") = value
        End Set
    End Property

    Public Property StatusIDs As String
        Get
            If ViewState("vs_statusID") Is Nothing Then
                ViewState("vs_statusID") = 0
            End If
            Return ViewState("vs_statusID")
        End Get
        Set(value As String)
            ViewState("vs_statusID") = value
        End Set
    End Property

    Public Property AgentUserName As String
        Get
            If ViewState("vs_AgentUserName") Is Nothing Then
                ViewState("vs_AgentUserName") = String.Empty
            End If
            Return ViewState("vs_AgentUserName")
        End Get
        Set(value As String)
            ViewState("vs_AgentUserName") = value
        End Set
    End Property

    Public Property AgentUserID As Integer
        Get
            If ViewState("vs_AgentUserID") Is Nothing Then
                ViewState("vs_AgentUserID") = -2
            End If
            Return ViewState("vs_AgentUserID")
        End Get
        Set(value As Integer)
            ViewState("vs_AgentUserID") = value
        End Set
    End Property

    Public Property ShowArchived As Boolean
        Get
            If ViewState("vs_ShowArchived") Is Nothing Then
                ViewState("vs_ShowArchived") = False
            End If
            Return CBool(ViewState("vs_ShowArchived"))
        End Get
        Set(value As Boolean)
            ViewState("vs_ShowArchived") = value
        End Set
    End Property

    Public Property TimeFrameDays As Int32
        Get
            If ViewState("vsTimeFrameDays") Is Nothing Then
                ViewState("vsTimeFrameDays") = 0
            End If
            Return ViewState("vsTimeFrameDays")
        End Get
        Set(value As Int32)
            ViewState("vsTimeFrameDays") = value
        End Set
    End Property

    Public Property ShowAllOnOnePage As Boolean
        Get
            If ViewState("vs_ShowAllOnOnePage") Is Nothing Then
                ViewState("vs_ShowAllOnOnePage") = False
            End If
            Return CBool(ViewState("vs_ShowAllOnOnePage"))
        End Get
        Set(value As Boolean)
            ViewState("vs_ShowAllOnOnePage") = value
        End Set
    End Property

    Public Property IsSavedPage As Boolean
        Get
            If ViewState("vs_IsSavedPage") Is Nothing Then
                ViewState("vs_IsSavedPage") = False
            End If
            Return CBool(ViewState("vs_IsSavedPage"))
        End Get
        Set(value As Boolean)
            ViewState("vs_IsSavedPage") = value
        End Set
    End Property
    Public Property SearchType As IFM.VR.Common.QuoteSearch.QuoteSearch.SearchType
        Get
            If ViewState("vs_SearchType") Is Nothing Then
                ViewState("vs_SearchType") = IFM.VR.Common.QuoteSearch.QuoteSearch.SearchType.NA
            End If
            Return ViewState("vs_SearchType")
        End Get
        Set(value As IFM.VR.Common.QuoteSearch.QuoteSearch.SearchType)
            ViewState("vs_SearchType") = value
        End Set
    End Property

    Public Property PageResults As List(Of Common.QuoteSearch.QQSearchResult)
        Get
            If ViewState("vs_PageResults") Is Nothing Then
                ViewState("vs_PageResults") = Nothing
            End If
            Return ViewState("vs_PageResults")
        End Get
        Set(value As List(Of Common.QuoteSearch.QQSearchResult))
            ViewState("vs_PageResults") = value
        End Set
    End Property
    Public Property HasEmployeeAccess As Boolean
    Public Property CurrentUserDiamondID As Integer
    Public Property IsStaff As Boolean

    Public Property SearchResults As List(Of Common.QuoteSearch.QQSearchResult)

    Private Property SelectedGridRowDataItem As Common.QuoteSearch.QQSearchResult

    Private ReadOnly Property NewLobButtonText As String
        Get
            Dim newButtonText As String = ""
            Select Case Me.ResultType
                Case ResultDisplayType.Auto
                    newButtonText = "New {0} Quote".FormatIFM(IFM.VR.Common.QuoteSearch.QuoteSearch.GetLobNameFromId(1))
                Case ResultDisplayType.Comm
                    newButtonText = "New {0} Quote".FormatIFM("Commercial")
                Case ResultDisplayType.Farm
                    newButtonText = "New {0} Quote".FormatIFM(IFM.VR.Common.QuoteSearch.QuoteSearch.GetLobNameFromId(17))
                Case ResultDisplayType.Home
                    If VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs({2, 3}).Count > 1 Then
                        newButtonText = "New {0} Quote".FormatIFM("Property/Liability")
                    Else
                        newButtonText = "New {0} Quote".FormatIFM("Personal Home")
                    End If
                Case ResultDisplayType.Umbrella
                    newButtonText = "New {0} Quote".FormatIFM("Umbrella")
            End Select
            Return newButtonText
        End Get

    End Property

    Public ReadOnly Property GetSplitButtonHtml As String
        Get
            Dim ulText As String = ""
            ulText += "<div style=""float:right"">"
            ulText += "<div>"
            If Me.ResultType = ResultDisplayType.Auto Then
                ulText += "<input type=""button"" style=""width:200px;height:25px;"" class=""StandardButton"" onclick=""document.location = 'MyVelocirater.aspx?newQuote=1'; return false;"" value=""" + NewLobButtonText + """/>"
            End If

            If Me.ResultType = ResultDisplayType.Home Then
                If VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs({2, 3}).Count > 1 Then
                    If VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs.ContainsValue(3) Then
                        ' create split button - home and dfr
                        ulText += "<input type=""button"" style=""width:200px;height:25px;"" class=""splitbutton StandardButton"" value=""" + NewLobButtonText + """/>"
                    Else
                        'just home
                        ulText += "<input type=""button"" style=""width:200px;height:25px;"" class=""StandardButton"" onclick=""document.location = 'MyVelocirater.aspx?newQuote=2'; return false;"" value=""" + NewLobButtonText + """/>"
                    End If
                Else
                    'just Home
                    ulText += "<input type=""button"" style=""width:200px;height:25px;"" class=""StandardButton"" onclick=""document.location = 'MyVelocirater.aspx?newQuote=2'; return false;"" value=""" + NewLobButtonText + """/>"
                End If


            End If

            If Me.ResultType = ResultDisplayType.Farm Then
                'ulText += "<input type=""button"" style=""width:200px;height:25px;"" class=""StandardButton"" onclick=""document.location = 'MyVelocirater.aspx?newQuote=17'; return false;"" value=""" + NewLobButtonText + """/>"
                ulText += "<input type=""button"" style=""width:200px;height:25px;"" class=""splitbutton StandardButton"" value=""" + NewLobButtonText + """/>"
            End If

            If Me.ResultType = ResultDisplayType.Comm Then
                ulText += "<input type=""button"" style=""width:200px;height:25px;"" class=""splitbutton StandardButton"" value=""" + NewLobButtonText + """/>"
            End If

            ulText += "</div>"

            If Me.ResultType = ResultDisplayType.Home Then
                If VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs({2, 3}).Count > 1 Then
                    ' create split button options more than one LOB
                    ulText += "<ul style=""z-index: 10000;"">"
                    For Each lob In VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs({2, 3}).Reverse()
                        ulText += "<li><a href=""MyVelocirater.aspx?newQuote=" + lob.Value.ToString() + """>" + lob.Key + "</a></li>"
                    Next
                    ulText += "</ul>"
                End If

            End If

            If Me.ResultType = ResultDisplayType.Farm Then
                ulText += "<ul style=""z-index: 10000;"">"
                For Each lob In VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs({17})
                    ulText += "<li><a href=""MyVelocirater.aspx?newQuote=" + lob.Value.ToString() + """>" + lob.Key + "</a></li>"
                Next
                For Each lob In VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs({20})
                    ulText += "<li><a href=""MyVelocirater.aspx?newQuote=" + lob.Value.ToString() + """>Farm " + lob.Key + "</a></li>"
                Next
                ulText += "</ul>"
            End If

            If Me.ResultType = ResultDisplayType.Comm Then
                ulText += "<ul style=""z-index: 10000;"">"
                'Updated 08/10/2021 for Bug 63958 MLW
                Dim commLines = Helpers.MultistateHelper.FilterOutMultistateLOB(IFM.VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs(Common.QuoteSearch.QuoteSearch.LobCategory.Commercial))
                For Each lob In commLines
                    ulText += "<li><a href=""MyVelocirater.aspx?newQuote=" + lob.Value.ToString() + """>" + lob.Key + "</a></li>"
                    'ulText += "<li><a href=""" + System.Configuration.ConfigurationManager.AppSettings("QuickQuote_NewQuote").ToString() + "?newQuote=" + lob.Value.ToString() + """>" + lob.Key + "</a></li>"
                Next
                'For Each lob In VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs(Common.QuoteSearch.QuoteSearch.LobCategory.Commercial)
                '    ulText += "<li><a href=""MyVelocirater.aspx?newQuote=" + lob.Value.ToString() + """>" + lob.Key + "</a></li>"
                '    'ulText += "<li><a href=""" + System.Configuration.ConfigurationManager.AppSettings("QuickQuote_NewQuote").ToString() + "?newQuote=" + lob.Value.ToString() + """>" + lob.Key + "</a></li>"
                'Next
                ulText += "</ul>"
            End If

            If Me.ResultType = ResultDisplayType.Umbrella Then
                ulText += "<input type=""button"" style=""width:200px;height:25px;"" class=""StandardButton"" onclick=""document.location = 'MyVelocirater.aspx?newQuote=14'; return false;"" value=""" + NewLobButtonText + """/>"
                ulText += "</div>"
            End If

            ulText += "</div>"
            Return ulText
        End Get
    End Property

    Dim qqConn As String = ConfigurationManager.AppSettings("ConnQQ")

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim target As String = Request("__EVENTTARGET")
        Dim parameter As String = Request("__EVENTARGUMENT")
        Dim num As String = ""
        Dim rownum As Integer = 0

        If Request("__EVENTTARGET") IsNot Nothing AndAlso Request("__EVENTTARGET").NoneAreNullEmptyorWhitespace() Then
            num = Request("__EVENTTARGET").Split("_").ToList().Last() 'Get the selected GridRowIndex
            target = Request("__EVENTTARGET").Replace("_" & num, "") 'Get the clientID of the grid that had the selected item
        End If

        If String.IsNullOrWhiteSpace(target) = False AndAlso target.Equals(Me.ClientID, StringComparison.OrdinalIgnoreCase) AndAlso String.IsNullOrWhiteSpace(parameter) = False AndAlso parameter.Contains("_") Then
            Try ' should remove try being lazy
                If PageResults IsNot Nothing AndAlso PageResults.Count > 0 AndAlso num.IsNumeric() AndAlso Integer.TryParse(num, rownum) Then
                    Me.SelectedGridRowDataItem = PageResults.Item(rownum)
                End If
                Dim action As String = parameter.Split("_")(0)
                Dim quoteid As String = parameter.Split("_")(1) ' could bomb out
                DoAction(action)
            Catch ex As Exception

            End Try
        End If
    End Sub

    Protected Function AcquireFromStaffToCurrentDiamondUserId(policyId As Int32) As Boolean
        If policyId > 0 Then
            If (Not DirectCast(Me.Page.Master, VR.Web.VelociRater).IsStaff) Then
                Dim diamondUserId = 0
                Integer.TryParse(Session("DiamondUserId"), diamondUserId)
                If (diamondUserId > 0) Then
                    Using proxy As New IFM.JsonProxyClient.ProxyClient(ConfigurationManager.AppSettings("IFMDataServices_EndPointBaseUrl"))
                        Dim result = proxy.Get($"VR/QuoteTransfer/AquireByUserId/{policyId}/{diamondUserId}")
                        Return result.IsSuccessStatusCode
                    End Using
                End If
            Else
                Return True
            End If
        Else
            Return True
        End If
        Return False
    End Function

    Protected Sub DoAction(action As String)
        'find the action
        ' based on status allow various actions to be performed
        Dim errorMessage As String = ""
        Dim result As Common.QuoteSearch.QQSearchResult = Me.SelectedGridRowDataItem

        Select Case action
            Case "Acquire Quote"
                ' Transfer from staff user to agency user
                Dim qq = IFM.VR.Common.QuoteSave.QuoteSaveHelpers.GetQuoteById_NOSESSION(result.QuoteId)
                If (qq IsNot Nothing) Then
                    If (qq.PolicyOriginTypeId.EqualsAny("3", "4", "6")) Then
                        ' set active to false
                        Dim consumerGuid As String = PublicQuotingLib.BusinessLogic.ServiceHelper.ConsumerQuotingGuidFromQuoteId(result.QuoteId)
                        Dim h = PublicQuotingLib.BusinessLogic.ServiceHelper.DeactivateConsumerGuidStatus(consumerGuid) ' might already be but not always
                        ' to load quote at beginning of quote - will need it change it's status to 2
                        Dim QQxml As New QuickQuote.CommonMethods.QuickQuoteXML()
                        Dim err As String = Nothing
                        QQxml.UpdateQuoteStatus(result.QuoteId, QuickQuoteXML.QuickQuoteStatusType.QuoteStarted, err)
                    End If

                    If (Not AcquireFromStaffToCurrentDiamondUserId(qq.PolicyId.TryToGetInt32())) Then
                        'alert
                        DirectCast(Me.Page.Master, VelociRater).StartUpScriptManager.AddScriptLine("alert('eCommerce quote could not be acquired from a staff user. Contact your underwriter.')")
                        Return
                    End If
                End If

                ' call back in now that you have deactivated it
                DoAction("View/Edit")


            Case "Move to VelociRater" ' quoteidOrQuoteNumber will be a quotenumber and not a quoteid
                Dim QQxml As New QuickQuote.CommonMethods.QuickQuoteXML()
                Dim successfullyLoadedIntoVR As Boolean = False
                Dim quoteNum As String = result.QuoteNumber
                Dim newOrExistingQuoteId As Integer = 0
                Dim loadIntoVrErrorMsg As String = ""

                Dim newQuote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing

                successfullyLoadedIntoVR = QQxml.SuccessfullyLoadedDiamondQuoteIntoVelociRater(quoteNum, newOrExistingQuoteId, newQuote, loadIntoVrErrorMsg)
                If successfullyLoadedIntoVR = True AndAlso String.IsNullOrEmpty(loadIntoVrErrorMsg) = True AndAlso newOrExistingQuoteId > 0 AndAlso newQuote IsNot Nothing Then
                    'success; should be able to just check for success True and quoteId > 0
                    'Response.Redirect(IFM.VR.Common.QuoteSearch.QQSearchResult.GetViewUrl(newQuote.LobId, newQuote.Database_QuoteStatusId, newQuote.Database_QuoteId) + "&showNowInVr=true", True)
                    IFM.VR.Web.Helpers.WebHelper_Personal.RedirectToQuotePage(tranType:=QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote, lobType:=IFM.VR.Common.Helpers.LOBHelper.LOBIdToType(newQuote.LobId), quoteId:=newQuote.Database_QuoteId, quoteStatus:=newQuote.Database_QuoteStatusId, goToApp:=False, workflowQueryString:="", isBillingUpdate:=False, AdditionalQueryStringParams:="&showNowInVr=true")
                Else
#If DEBUG Then
                    Debugger.Break()
#End If
                End If

            Case "Update from Diamond" ' quoteidOrQuoteNumber will be a quotenumber and not a quoteid
                Dim QQxml As New QuickQuote.CommonMethods.QuickQuoteXML()
                Dim successfullyLoadedIntoVR As Boolean = False
                Dim newOrExistingQuoteId As Integer = 0
                Dim loadIntoVrErrorMsg As String = ""

                Dim newQuote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing


                successfullyLoadedIntoVR = QQxml.SuccessfullyLoadedDiamondQuoteIntoVelociRater(result.QuoteNumber, newOrExistingQuoteId, newQuote, loadIntoVrErrorMsg)
                If successfullyLoadedIntoVR = True AndAlso String.IsNullOrEmpty(loadIntoVrErrorMsg) = True AndAlso newOrExistingQuoteId > 0 AndAlso newQuote IsNot Nothing Then
                    'success; should be able to just check for success True and quoteId > 0
                    'Response.Redirect(IFM.VR.Common.QuoteSearch.QQSearchResult.GetViewUrl(newQuote.LobId, newQuote.Database_XmlStatusId, newQuote.Database_QuoteId) + "&UpdatedFromDiamond=true", True)
                    IFM.VR.Web.Helpers.WebHelper_Personal.RedirectToQuotePage(tranType:=QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote, lobType:=IFM.VR.Common.Helpers.LOBHelper.LOBIdToType(newQuote.LobId), quoteId:=newQuote.Database_QuoteId, quoteStatus:=newQuote.Database_QuoteStatusId, goToApp:=False, workflowQueryString:="", isBillingUpdate:=False, AdditionalQueryStringParams:="&UpdatedFromDiamond=true")
                Else
#If DEBUG Then
                    Debugger.Break()
#End If
                End If

            Case Else 'Reworked so that we pull in the information from the previous data pull. No longer need an extra database call here. -actions that require a quoteid to work    -   quoteidOrQuoteNumber will be the quoteid and not a quotenumber
                If result IsNot Nothing Then
                    Select Case action
                        Case "Portal"
                            Response.Redirect(result.ViewUrl, False)
                        Case "View/Edit", "Quote Summary", "Application", "App Summary", "Edit Change", "Edit Quote"  ' Added "Edit Change" and "Edit Quote" per task 43589 MGB 4/22/2020
                            If result.ItemType <> Common.QuoteSearch.QuoteSearch.ItemType.NA Then
                                Dim goToApp As Boolean = False
                                Dim resultStatus As Integer = 0
                                'If action.Equals("Application", StringComparison.OrdinalIgnoreCase) OrElse action.Equals("", StringComparison.OrdinalIgnoreCase) Then
                                '    goToApp = True
                                'End If
                                If UCase(action).Contains("APP") = True Then
                                    goToApp = True
                                End If

                                If result.VR_LastModified <= Date.Now.AddDays(-Helpers.EffectiveDateHelper.DaysAgoForPolicyToRestartAtPolicyHolder) Then
                                    resultStatus = 0
                                Else
                                    resultStatus = result.StatusId
                                End If

                                Select Case result.ItemType
                                    'Case Common.QuoteSearch.QuoteSearch.ItemType.Change, Common.QuoteSearch.QuoteSearch.ItemType.BillingUpdate, Common.QuoteSearch.QuoteSearch.ItemType.Quote
                                    Case Common.QuoteSearch.QuoteSearch.ItemType.Change, Common.QuoteSearch.QuoteSearch.ItemType.BillingUpdate
                                        'IFM.VR.Web.Helpers.WebHelper_Personal.RedirectToQuotePage(tranType:=result.QuickQuoteTransactionType, lobType:=IFM.VR.Common.Helpers.LOBHelper.LOBIdToType(result.LobId), policyId:=result.PolicyId, policyImageNum:=result.PolicyImageNum, quoteStatus:=result.StatusId, goToApp:=goToApp, workflowQueryString:="", isBillingUpdate:=result.IsBillingUpdate)
                                        IFM.VR.Web.Helpers.WebHelper_Personal.RedirectToQuotePage(tranType:=QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote, lobType:=IFM.VR.Common.Helpers.LOBHelper.LOBIdToType(result.LobId), policyId:=result.PolicyId, policyImageNum:=result.PolicyImageNum, quoteStatus:=result.StatusId, goToApp:=goToApp, workflowQueryString:="", isBillingUpdate:=result.IsBillingUpdate)
                                    Case Common.QuoteSearch.QuoteSearch.ItemType.Quote
                                        'IFM.VR.Web.Helpers.WebHelper_Personal.RedirectToQuotePage(tranType:=result.QuickQuoteTransactionType, lobType:=IFM.VR.Common.Helpers.LOBHelper.LOBIdToType(result.LobId), quoteId:=result.QuoteId, quoteStatus:=result.StatusId, goToApp:=goToApp, workflowQueryString:="", isBillingUpdate:=result.IsBillingUpdate)
                                        IFM.VR.Web.Helpers.WebHelper_Personal.RedirectToQuotePage(tranType:=QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote, lobType:=IFM.VR.Common.Helpers.LOBHelper.LOBIdToType(result.LobId), quoteId:=result.QuoteId, quoteStatus:=resultStatus, goToApp:=goToApp, workflowQueryString:="", isBillingUpdate:=False)
                                End Select
                            End If
                        Case "View Policy"
                            'IFM.VR.Web.Helpers.WebHelper_Personal.RedirectToQuotePage(tranType:=result.QuickQuoteTransactionType, lobType:=IFM.VR.Common.Helpers.LOBHelper.LOBIdToType(result.LobId), policyId:=result.PolicyId, policyImageNum:=result.CurrentPolicyImageNumForPolicy, quoteStatus:=Nothing, goToApp:=False, workflowQueryString:="", isBillingUpdate:=False)
                            IFM.VR.Web.Helpers.WebHelper_Personal.RedirectToQuotePage(tranType:=QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage, lobType:=IFM.VR.Common.Helpers.LOBHelper.LOBIdToType(result.LobId), policyId:=result.PolicyId, policyImageNum:=result.CurrentPolicyImageNumForPolicy, quoteStatus:=Nothing, goToApp:=False, workflowQueryString:="", isBillingUpdate:=False)
                        Case "Delete"
                            If IFM.VR.Common.QuoteSave.QuoteSaveHelpers.SuccessfullyDeletedEndorsementQuote(result.PolicyId, policyImageNum:=result.PolicyImageNum, errorMessage:=errorMessage) Then
                                'If the searchtype is all or policies, we should load the policy back into the search results after deleting the change. 
                                'Instead of doing a full search and refresh the full list, it should be faster to search for that single policy And re-add this ViewState list.
                                'For the saved changes, billing updates pages... We don't need to do this.
                                If SearchType = Common.QuoteSearch.QuoteSearch.SearchType.All OrElse SearchType = Common.QuoteSearch.QuoteSearch.SearchType.Policies Then
                                    AddNewRecordToSearchResultsList(result, action)
                                Else
                                    PageResults.Remove(result)
                                End If
                                gridQuoteResults.DataSource = PageResults
                                gridQuoteResults.DataBind()
                            Else
                                VR.Web.Helpers.MessageBoxVRPers.Show(errorMessage, Response, ScriptManager.GetCurrent(Me.Page), Me)
                            End If
                        Case "Process Change"
                            IFM.VR.Web.Helpers.WebHelper_Personal.RedirectToStartEndorsementPage(result.PolicyId, result.PolicyImageNum, False)
                        Case "Billing Change"
                            IFM.VR.Web.Helpers.WebHelper_Personal.RedirectToStartEndorsementPage(result.PolicyId, result.PolicyImageNum, True)
                        Case "Copy"
                            Dim qqx As New QuickQuoteXML()
                            Dim newQuoteID As String = Nothing
                            Dim errMsg As String = Nothing

                            If IFM.VR.Common.QuoteSearch.QuoteSearch.CommericialLobs.Contains(result.LobId) Then
                                'comm
                                qqx.CopyQuote(result.QuoteId, newQuoteID, errMsg)
                            Else
                                'personal
                                qqx.CopyQuote(result.QuoteId, True, newQuoteID, errMsg) '11-7-14 new remove UW questions overload
                            End If

                            Dim newQuoteIDInt As Integer = 0

                            If String.IsNullOrWhiteSpace(errMsg) AndAlso newQuoteID.IsNullEmptyorWhitespace = False AndAlso Integer.TryParse(newQuoteID, newQuoteIDInt) AndAlso newQuoteID > 0 Then
                                AddNewRecordToSearchResultsList(result, action, newQuoteID)
                                Dim newResult As Common.QuoteSearch.QQSearchResult = PageResults.Find(Function(x) x.QuoteId = newQuoteIDInt)
                                If newResult IsNot Nothing Then
                                    IFM.VR.Web.Helpers.WebHelper_Personal.RedirectToQuotePage(tranType:=QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote, lobType:=IFM.VR.Common.Helpers.LOBHelper.LOBIdToType(newResult.LobId), quoteId:=newResult.QuoteId, quoteStatus:=newResult.StatusId, goToApp:=False, workflowQueryString:="", isBillingUpdate:=False)
                                Else
                                    errMsg = "Unable to open copied quote at this time. Please try again later."
                                    VR.Web.Helpers.MessageBoxVRPers.Show(errMsg, Response, ScriptManager.GetCurrent(Me.Page), Me)
                                End If
                            Else
                                VR.Web.Helpers.MessageBoxVRPers.Show(errMsg, Response, ScriptManager.GetCurrent(Me.Page), Me)
                            End If

                        Case "Undelete"
                            Dim qqx As New QuickQuoteXML()
                            Dim errMsg As String = Nothing
                            qqx.ArchiveOrUnarchiveQuote(result.QuoteId, QuickQuoteXML.QuickQuoteArchiveType.UnArchive, errMsg)
                            If String.IsNullOrWhiteSpace(errMsg) Then
                                AddNewRecordToSearchResultsList(result, action)
                                gridQuoteResults.DataSource = PageResults
                                gridQuoteResults.DataBind()
                            Else
                                VR.Web.Helpers.MessageBoxVRPers.Show(errMsg, Response, ScriptManager.GetCurrent(Me.Page), Me)
                            End If
                        Case "Archive"
                            ' remove this quote from recent quote log
                            Helpers.WebHelper_Personal.RemoveQuoteIdFromSessionHistory(Session, result.QuoteId)
                            Dim qqx As New QuickQuoteXML()
                            Dim errMsg As String = Nothing
                            qqx.ArchiveOrUnarchiveQuote(result.QuoteId, QuickQuoteXML.QuickQuoteArchiveType.Archive, errMsg)
                            If errMsg.IsNullEmptyorWhitespace() Then
                                If ShowArchived = True Then
                                    ChangeResultItemStatusID(result, 12)
                                Else
                                    PageResults.Remove(result)
                                End If
                                gridQuoteResults.DataSource = PageResults
                                gridQuoteResults.DataBind()
                            Else
                                VR.Web.Helpers.MessageBoxVRPers.Show(errMsg, Response, ScriptManager.GetCurrent(Me.Page), Me)
                            End If
                        Case "Proposal"
                            Response.Redirect(result.ProposalUrl)
                        Case "Quote XML"
                            Response.Redirect(System.Configuration.ConfigurationManager.AppSettings("QuickQuote_XmlSelecter") + "?quoteid=" + result.QuoteId.ToString())
                    End Select
                End If
        End Select
    End Sub

    Private Sub AddNewRecordToSearchResultsList(result As Common.QuoteSearch.QQSearchResult, action As String, Optional newQuoteOrPolicyID As Integer = 0)
        If result IsNot Nothing AndAlso String.IsNullOrWhiteSpace(action) = False Then
            Dim newResultList As List(Of Common.QuoteSearch.QQSearchResult) = Nothing
            Dim searchParams As New Common.QuoteSearch.QQSearchParameters

            If SearchParameters IsNot Nothing Then
                With searchParams
                    .AgencyID = SearchParameters.AgencyID
                    If newQuoteOrPolicyID > 0 Then
                        .quoteOrPolicyID = newQuoteOrPolicyID
                    Else
                        If result.ItemType = Common.QuoteSearch.QuoteSearch.ItemType.Quote Then
                            .quoteOrPolicyID = result.QuoteId
                        Else
                            .quoteOrPolicyID = result.PolicyId
                        End If
                    End If
                    .IsStaff = SearchParameters.IsStaff
                    .ShowArchived = False
                    .AllowResultsNotCurrentlyInVR = True
                    .HasEmployeeAccess = HasEmployeeAccess
                    .CurrentDiamondUserID = CurrentUserDiamondID
                    .SearchType = SearchParameters.SearchType
                End With

                newResultList = Common.QuoteSearch.QuoteSearch.SearchBySearchParameters(searchParams)

                If newResultList IsNot Nothing AndAlso newResultList.Count = 1 Then
                    'Using a new list to keep the current sorting intact
                    Dim sortedResultList As New List(Of Common.QuoteSearch.QQSearchResult)

                    Select Case action
                        Case "Delete", "Undelete"
                            For Each thisResult As Common.QuoteSearch.QQSearchResult In PageResults
                                If thisResult.Equals(result) = False Then
                                    sortedResultList.Add(thisResult)
                                Else
                                    sortedResultList.Add(newResultList(0))
                                End If
                            Next
                            PageResults = sortedResultList
                        Case "Copy"
                            sortedResultList.Add(newResultList(0))
                            sortedResultList.AddRange(PageResults)
                            PageResults = sortedResultList
                    End Select
                End If
            End If
        End If
    End Sub

    Private Sub ChangeResultItemStatusID(result As Common.QuoteSearch.QQSearchResult, StatusId As Integer)
        Dim updateThisResult As Common.QuoteSearch.QQSearchResult = PageResults.Find(Function(x) x.QuoteId = result.QuoteId)
        If updateThisResult IsNot Nothing Then
            updateThisResult.StatusId = 12
        End If
    End Sub

    Private Function GetData(searchParams As Common.QuoteSearch.QQSearchParameters) As List(Of VR.Common.QuoteSearch.QQSearchResult)
        Dim results As List(Of VR.Common.QuoteSearch.QQSearchResult) = Nothing

        If PageResults Is Nothing Then
            If searchParams IsNot Nothing Then
                results = Common.QuoteSearch.QuoteSearch.SearchBySearchParameters(searchParams)
            End If
        End If

        Return results
    End Function

    Public Sub Populate(Optional dontpulldata As Boolean = False, Optional ByVal forceEmptyLoad As Boolean = False) 'added optional forceEmptyLoad param 3/9/2021
        Dim useSingleSQLQuery As Boolean = False
        Me.Visible = False

        If ViewState("vs_DefaultPageSize") Is Nothing Then
            ViewState("vs_DefaultPageSize") = Me.gridQuoteResults.PageSize
        End If

        Me.gridQuoteResults.PageIndex = 0
        Me.gridQuoteResults.PageSize = If(Me.ShowAllOnOnePage, 1000, CInt(ViewState("vs_DefaultPageSize")))
        ViewState.Item("lastSortColumn") = ""
        ViewState.Item("SortAcs") = "0"

        If IFM.VR.Common.Helpers.GenericHelper.GetAppSettingsValueForBoolean("VR_GlobalSearch_UseSingleSQLQuery", useSingleSQLQuery) AndAlso useSingleSQLQuery = True Then
            PopulateWithData(forceEmptyLoad:=forceEmptyLoad) 'updated 3/9/2021 for forceEmptyLoad
        Else
            InitialDataLoad(forceEmptyLoad:=forceEmptyLoad) 'updated 3/9/2021 for forceEmptyLoad
        End If

        Me.lblNoResults.Visible = Not (PageResults IsNot Nothing AndAlso PageResults.Any())
        If Me.lblNoResults.Visible Then
            If dontpulldata Then
                Me.lblNoResults.Text = "Staff user detected no attempt to perform search until specifically requested."
            Else
                Me.lblNoResults.Text = "No search results found. Try changing your search criteria and search again."
            End If
            Me.hdnSearchResults.Value = "1"
        End If

    End Sub

    Private Sub gridQuoteResults_DataBound(sender As Object, e As EventArgs) Handles gridQuoteResults.DataBound
        Dim isStaff As Boolean = DirectCast(Me.Page.Master, VelociRater).IsStaff

        gridQuoteResults.Columns(IFM.VR.Common.QuoteSearch.QuoteSearch.SearchResultColumns.AgencyCode).Visible = isStaff
        gridQuoteResults.Columns(IFM.VR.Common.QuoteSearch.QuoteSearch.SearchResultColumns.LastModified).Visible = False
        gridQuoteResults.Columns(IFM.VR.Common.QuoteSearch.QuoteSearch.SearchResultColumns.QuoteID).Visible = False
        gridQuoteResults.Columns(IFM.VR.Common.QuoteSearch.QuoteSearch.SearchResultColumns.ItemType).Visible = False
        gridQuoteResults.Columns(IFM.VR.Common.QuoteSearch.QuoteSearch.SearchResultColumns.PolicyTerm).Visible = False

        If SearchType = Common.QuoteSearch.QuoteSearch.SearchType.Quotes Then
            gridQuoteResults.Columns(IFM.VR.Common.QuoteSearch.QuoteSearch.SearchResultColumns.LastModified).Visible = isStaff
            gridQuoteResults.Columns(IFM.VR.Common.QuoteSearch.QuoteSearch.SearchResultColumns.QuoteID).Visible = isStaff

            Select Case Me.ResultType
                Case ResultDisplayType.Comm, ResultDisplayType.Umbrella
                    gridQuoteResults.Columns(IFM.VR.Common.QuoteSearch.QuoteSearch.SearchResultColumns.LOBName).Visible = True

                Case ResultDisplayType.Home AndAlso IFM.VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs().Values.Contains(3)
                    gridQuoteResults.Columns(IFM.VR.Common.QuoteSearch.QuoteSearch.SearchResultColumns.LOBName).Visible = True

                Case Else
                    gridQuoteResults.Columns(IFM.VR.Common.QuoteSearch.QuoteSearch.SearchResultColumns.LOBName).Visible = False
            End Select

            'gridQuoteResults.Columns(IFM.VR.Common.QuoteSearch.QuoteSearch.SearchResultColumns.LOBName).Visible = Me.ResultType = ResultDisplayType.Comm Or Me.ResultType = ResultDisplayType.Home '  isStaff
        Else
            gridQuoteResults.Columns(IFM.VR.Common.QuoteSearch.QuoteSearch.SearchResultColumns.PolicyTerm).Visible = True
            gridQuoteResults.Columns(IFM.VR.Common.QuoteSearch.QuoteSearch.SearchResultColumns.LOBName).Visible = True
        End If
    End Sub

    Private Sub gridQuoteResults_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles gridQuoteResults.PageIndexChanging
        Me.gridQuoteResults.PageIndex = e.NewPageIndex

        Me.gridQuoteResults.DataSource = Nothing
        Me.gridQuoteResults.DataBind()
        SortData()
    End Sub

    Private Sub gridQuoteResults_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles gridQuoteResults.RowDataBound
        'ddActionList
        Dim isStaff As Boolean = DirectCast(Me.Page.Master, VelociRater).IsStaff

        Select Case e.Row.RowType
            Case DataControlRowType.Header
                If SearchType = Common.QuoteSearch.QuoteSearch.SearchType.Quotes Then
                    e.Row.Cells(IFM.VR.Common.QuoteSearch.QuoteSearch.SearchResultColumns.QuoteNumAndDescription).Text = "Quote #"
                Else
                    e.Row.Cells(IFM.VR.Common.QuoteSearch.QuoteSearch.SearchResultColumns.QuoteNumAndDescription).Text = "Quote/Policy #"
                End If
            Case DataControlRowType.DataRow
                If Request("__EVENTTARGET") IsNot Nothing Then
                    Dim rowNum As Integer = -1
                    Dim num As String = Request("__EVENTTARGET").Split("_").ToList().Last() 'Get the selected GridRowIndex
                    Dim target As String = Request("__EVENTTARGET").Replace("_" & num, "") 'Get the clientID of the grid that had the selected item
                    If target.Equals(Me.ClientID, StringComparison.OrdinalIgnoreCase) AndAlso IsNumeric(num) AndAlso Integer.TryParse(num, rowNum) AndAlso rowNum >= 0 AndAlso e.Row.RowIndex = rowNum Then
                        Me.SelectedGridRowDataItem = e.Row.DataItem
                    End If
                End If

                Dim ddList As DropDownList = e.Row.FindControl("ddActionList")
                If ddList IsNot Nothing Then
                    Dim result As VR.Common.QuoteSearch.QQSearchResult = e.Row.DataItem
                    If result IsNot Nothing Then
                        ddList.Enabled = result.AvilableActions.Count > 1
                        For Each i In result.AvilableActions
                            ddList.Items.Add(New ListItem(i.Key, i.Value))
                        Next
                        If isStaff And result.IsCurrentlyInVR Then
                            ddList.Enabled = True

                            'added 3/17/2021; note: QuoteSearcher.GetViewURLForQuote doesn't return anything for statusId 13, but it shouldn't be used on the action redirect anyway, so it shouldn't matter
                            If result.StatusId = 13 AndAlso (String.IsNullOrWhiteSpace(result.PolicyNumber) = True OrElse Left(result.PolicyNumber, 1) = "Q") Then
                                ddList.Items.Add(New ListItem("Edit Quote", result.QuoteId))
                            End If

                            ddList.Items.Add(New ListItem("Quote XML", result.QuoteId))
                        End If
                        ddList.Attributes.Add("onchange", "if ($(this).children(""option"").filter("":selected"").text() == ""Archive""){if (confirm(""Archive this quote? "")) {DisableFormOnSaveRemoves();Post(this, '" & Me.ClientID & "_" & e.Row.DataItemIndex & "');} else {$(this).removeAttr(""selected"").find(""option:first"").attr(""selected"", ""selected"");}return true;} else if ($(this).children(""option"").filter("":selected"").text() == ""Delete""){if (confirm(""Delete this change? "")) {DisableFormOnSaveRemoves();Post(this, '" & Me.ClientID & "_" & e.Row.DataItemIndex & "');} else {$(this).removeAttr(""selected"").find(""option:first"").attr(""selected"", ""selected"");}}else {DisableFormOnSaveRemoves(); Post(this, '" & Me.ClientID & "_" & e.Row.DataItemIndex & "');}")
                        'If result.EffectiveDate.NoneAreNullEmptyorWhitespace AndAlso IsDate(result.EffectiveDate) Then
                        'If result.EffectiveDate.NoneAreNullEmptyorWhitespace AndAlso IsDate(result.EffectiveDate) AndAlso result.EffectiveDate.Equals("01/01/0001", StringComparison.Ordinal) = False Then
                        If result.TEffectiveDate.NoneAreNullEmptyorWhitespace AndAlso IsDate(result.TEffectiveDate) AndAlso result.TEffectiveDate.Equals("01/01/0001", StringComparison.Ordinal) = False Then
                            e.Row.ToolTip = String.Format("Last Modified: {0}" & Environment.NewLine & "Effective Date: {1}", result.LastModified.ToString(), result.TEffectiveDate)
                        Else
                            e.Row.ToolTip = String.Format("Last Modified: {0}", result.LastModified.ToString())
                        End If

                        If result.EffectiveDate.Equals("01/01/0001", StringComparison.Ordinal) = False AndAlso result.ExpirationDate.Equals("01/01/0001", StringComparison.OrdinalIgnoreCase) = False Then
                            e.Row.Cells(IFM.VR.Common.QuoteSearch.QuoteSearch.SearchResultColumns.PolicyTerm).Text = result.EffectiveDate & "-<br />" & result.ExpirationDate
                        Else
                            e.Row.Cells(IFM.VR.Common.QuoteSearch.QuoteSearch.SearchResultColumns.PolicyTerm).Text = ""
                        End If
                    End If
                End If

                Dim customerName As String = e.Row.Cells(IFM.VR.Common.QuoteSearch.QuoteSearch.SearchResultColumns.ClientName).Text
                customerName = customerName.Replace("  ", " ")
                If customerName.Contains(" ") Then
                    Dim newCustomerName As String = ""
                    Dim names = customerName.Split(" ")
                    For Each name In names
                        If name.Length > 30 Then
                            newCustomerName += name.Substring(0, 30) + " " + name.Substring(30, name.Length - 30)
                        Else
                            newCustomerName += name + " "
                        End If
                    Next
                    customerName = newCustomerName
                End If
                If customerName.Length > 150 Then
                    customerName = customerName.Substring(0, 150) + "..."
                End If

                e.Row.Cells(IFM.VR.Common.QuoteSearch.QuoteSearch.SearchResultColumns.ClientName).Text = customerName
                If IsDate(e.Row.Cells(IFM.VR.Common.QuoteSearch.QuoteSearch.SearchResultColumns.LastModified).Text) Then
                    Dim lastModifieddate As DateTime = CDate(e.Row.Cells(IFM.VR.Common.QuoteSearch.QuoteSearch.SearchResultColumns.LastModified).Text)
                    If lastModifieddate.ToShortDateString() = DateTime.Now.ToShortDateString() Then
                        e.Row.Cells(IFM.VR.Common.QuoteSearch.QuoteSearch.SearchResultColumns.LastModified).Text = lastModifieddate.ToShortTimeString()
                    Else
                        If (CDate(DateTime.Now.ToShortDateString()) - CDate(lastModifieddate.ToShortDateString())).Days < 7 Then
                            e.Row.Cells(IFM.VR.Common.QuoteSearch.QuoteSearch.SearchResultColumns.LastModified).Text = lastModifieddate.DayOfWeek.ToString() + "<br/> " + lastModifieddate.ToShortTimeString()
                        Else
                            'do nothing just show full date
                        End If
                    End If
                End If

                ' Adjust LobName for Umbrella Items
                If Me.ResultType = ResultDisplayType.Umbrella Then
                    Dim type = e.Row.Cells(IFM.VR.Common.QuoteSearch.QuoteSearch.SearchResultColumns.LOBName).Text.Split(" ")
                    'e.Row.Cells(IFM.VR.Common.QuoteSearch.QuoteSearch.SearchResultColumns.LOBName).Text = type(1) + " " + type(0)
                    'updated 4/22/2021
                    Dim persOrFarmText As String = type(1)
                    Dim result As VR.Common.QuoteSearch.QQSearchResult = e.Row.DataItem
                    If result IsNot Nothing AndAlso result.ProgramTypeId = 5 Then
                        persOrFarmText = "Farm"
                    End If
                    e.Row.Cells(IFM.VR.Common.QuoteSearch.QuoteSearch.SearchResultColumns.LOBName).Text = persOrFarmText + " " + type(0)
                End If
        End Select
    End Sub

    Private Sub gridQuoteResults_Sorting(sender As Object, e As GridViewSortEventArgs) Handles gridQuoteResults.Sorting
        Dim sort = e.SortExpression
        Me.gridQuoteResults.PageIndex = 0
        If String.IsNullOrWhiteSpace(ViewState.Item("lastSortColumn")) = False Then

            If ViewState.Item("lastSortColumn") = e.SortExpression Then
                ViewState.Item("SortAcs") = If(ViewState.Item("SortAcs") = "0", "1", "0")
            Else
                ViewState.Item("SortAcs") = "1"
            End If
        Else
            ViewState.Item("SortAcs") = "0"
        End If

        'always set this
        ViewState.Item("lastSortColumn") = e.SortExpression
        SortData()
    End Sub

    Private Sub InitialDataLoad(Optional ByVal forceEmptyLoad As Boolean = False) 'added optional forceEmptyLoad param 3/9/2021
        'Private Function InitialDataLoad())
        Dim results As List(Of VR.Common.QuoteSearch.QQSearchResult) = Nothing

        Me.Visible = False
        If SearchParameterObjectPassed Then
            If SearchParameters.LobIDsList.IsLoaded() OrElse (SearchParameters.NonEndorsementReadyLobIds.IsLoaded() AndAlso SearchType = Common.QuoteSearch.QuoteSearch.SearchType.All) Then
                Me.Visible = True
                spnImage.AddCssClass(SectionImageClass)
                Me.lblLobName.Text = SectionName
                Me.hdnSearchResults.Value = 1 'If(Page.Request("PageView") Is Nothing, Me.hdnSearchResults.Value, "1")
                If forceEmptyLoad = True Then 'added IF 3/9/2021; original logic in ELSE
                    PageResults = New List(Of Common.QuoteSearch.QQSearchResult)
                Else
                    PageResults = GetData(SearchParameters)
                End If
            End If
        End If

        SortData()
    End Sub

    Private Sub PopulateWithData(Optional ByVal forceEmptyLoad As Boolean = False) 'added optional forceEmptyLoad param 3/9/2021
        Dim useSingleSQLQuery As Boolean = False

        Me.Visible = False
        If SearchParameterObjectPassed Then
            If SearchParameters.LobIDsList.IsLoaded() OrElse (SearchParameters.NonEndorsementReadyLobIds.IsLoaded() AndAlso SearchType = Common.QuoteSearch.QuoteSearch.SearchType.All) Then
                Me.Visible = True
                spnImage.AddCssClass(SectionImageClass)
                Me.lblLobName.Text = SectionName
                Me.hdnSearchResults.Value = 1 'If(Page.Request("PageView") Is Nothing, Me.hdnSearchResults.Value, "1")
                If IFM.VR.Common.Helpers.GenericHelper.GetAppSettingsValueForBoolean("VR_GlobalSearch_UseSingleSQLQuery", useSingleSQLQuery) = False OrElse useSingleSQLQuery = False Then
                    If forceEmptyLoad = True Then 'added IF 3/9/2021; original logic in ELSE
                        PageResults = New List(Of Common.QuoteSearch.QQSearchResult)
                    Else
                        PageResults = GetData(SearchParameters)
                    End If
                End If
            End If
        End If

        SortData()
    End Sub

    Private Sub SortData()
        Dim results As List(Of VR.Common.QuoteSearch.QQSearchResult) = Nothing

        If PageResults IsNot Nothing Then
            results = PageResults
        End If

        If results IsNot Nothing Then
            If ViewState.Item("SortAcs") = "1" Then
                Select Case ViewState.Item("lastSortColumn")
                    Case "PolicyNumber"
                        results = (From r In results Select r Order By r.PolicyNumber, r.QuoteNumber Ascending).ToList()
                    Case "QuoteNumAndDescription"
                        results = (From r In results Select r Order By r.QuoteNumAndDescription Ascending).ToList()
                    Case "ClientName"
                        results = (From r In results Select r Order By r.ClientName Ascending).ToList()
                    Case "Premium"
                        results = (From r In results Select r Order By r.Premium Ascending).ToList()
                    Case "FriendlyStatus"
                        results = (From r In results Select r Order By r.FriendlyStatus Ascending).ToList()
                    Case "LastModified"
                        results = (From r In results Select r Order By r.LastModified Ascending).ToList()
                    Case "AgencyCode"
                        results = (From r In results Select r Order By r.AgencyCode Ascending).ToList()
                    Case "Lob"
                        results = (From r In results Select r Order By r.LobName Ascending).ToList()
                    Case Else
                        If SearchType = Common.QuoteSearch.QuoteSearch.SearchType.Quotes OrElse SearchType = Common.QuoteSearch.QuoteSearch.SearchType.Changes OrElse SearchType = Common.QuoteSearch.QuoteSearch.SearchType.BillingUpdates Then
                            results = (From r In results Select r Order By r.LastModified Ascending).ToList()
                        Else
                            results = (From r In results Select r Order By r.PolicyNumber, r.QuoteNumber Ascending).ToList()
                        End If
                End Select
            Else
                Select Case ViewState.Item("lastSortColumn")
                    Case "PolicyNumber"
                        results = (From r In results Select r Order By r.PolicyNumber, r.QuoteNumber Descending).ToList()
                    Case "QuoteNumAndDescription"
                        results = (From r In results Select r Order By r.QuoteNumAndDescription Descending).ToList()
                    Case "ClientName"
                        results = (From r In results Select r Order By r.ClientName Descending).ToList()
                    Case "Premium"
                        results = (From r In results Select r Order By r.Premium Descending).ToList()
                    Case "FriendlyStatus"
                        results = (From r In results Select r Order By r.FriendlyStatus Descending).ToList()
                    Case "LastModified"
                        results = (From r In results Select r Order By r.LastModified Descending).ToList()
                    Case "AgencyCode"
                        results = (From r In results Select r Order By r.AgencyCode Descending).ToList()
                    Case "Lob"
                        results = (From r In results Select r Order By r.LobName Descending).ToList()
                    Case Else
                        If SearchType = Common.QuoteSearch.QuoteSearch.SearchType.Quotes OrElse SearchType = Common.QuoteSearch.QuoteSearch.SearchType.Changes OrElse SearchType = Common.QuoteSearch.QuoteSearch.SearchType.BillingUpdates Then
                            results = (From r In results Select r Order By r.LastModified Descending).ToList()
                        Else
                            results = (From r In results Select r Order By r.PolicyNumber, r.QuoteNumber Descending).ToList()
                        End If
                End Select

            End If
        End If

        PageResults = results

        Me.gridQuoteResults.DataSource = results
        Me.gridQuoteResults.DataBind()
    End Sub

    Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        If Me.hdnSearchResults.Value = "0" Then
            Me.divSubResults.Attributes.Add("style", "margin-left: 15px;display:none;")
        Else
            Me.divSubResults.Attributes.Add("style", "margin-left: 15px;")
        End If
    End Sub

    Private Function QuoteNumberMatchesLOBType() As Boolean
        Dim returnVar As Boolean = False
        Dim lobPrefix As String = ""

        If Me.QuoteNumber.Substring(0, 1).Equals("Q", StringComparison.OrdinalIgnoreCase) Then
            lobPrefix = Me.QuoteNumber.Substring(1, 3)
        Else
            lobPrefix = Me.QuoteNumber.Substring(0, 3)
        End If

        Select Case lobPrefix.ToUpper()
            Case "PPA"
                If Me.ResultType = ResultDisplayType.Auto Then
                    returnVar = True
                End If
            Case "HOM", "DFR"
                If Me.ResultType = ResultDisplayType.Home Then
                    returnVar = True
                End If
            Case "CAP", "BOP", "CGL", "CPP", "CPR", "WCP"
                If Me.ResultType = ResultDisplayType.Comm Then
                    returnVar = True
                End If
            Case "FAR"
                If Me.ResultType = ResultDisplayType.Farm Then
                    returnVar = True
                End If
            Case "PUP", "FUP"
                If Me.ResultType = ResultDisplayType.Umbrella Then
                    returnVar = True
                End If
        End Select

        If lobPrefix.Equals("PPA", StringComparison.OrdinalIgnoreCase) Then

        End If

        Return returnVar
    End Function

    Private Sub ctlQuoteSearchResults_Unload(sender As Object, e As EventArgs) Handles Me.Unload

    End Sub
End Class