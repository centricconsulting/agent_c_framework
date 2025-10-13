Imports System.Web.Services
Imports IFM.VR.Web.Helpers
Imports IFM.PrimativeExtensions
Imports PopupMessageClass

Public Class ctlQuoteSearch
    Inherits System.Web.UI.UserControl

    Public Event SearchRequested(searchParameters As Common.QuoteSearch.QQSearchParameters)

    Private Const staffUserNameIndicationText As String = " (IFM_STAFF)"
    Public Shared ReadOnly connQQ As String = System.Configuration.ConfigurationManager.AppSettings("connQQ")

    Private Property AgencySwitched As Boolean = False

    Private _PreviousSearchParams As Common.QuoteSearch.QQSearchParameters
    Private Property PreviousSearchParams As Common.QuoteSearch.QQSearchParameters
        Get
            If _PreviousSearchParams Is Nothing AndAlso ViewState IsNot Nothing AndAlso ViewState("VR_LandingPage_PreviousSearchParams") IsNot Nothing Then
                _PreviousSearchParams = ViewState("VR_LandingPage_PreviousSearchParams")
            End If
            Return _PreviousSearchParams
        End Get
        Set(value As Common.QuoteSearch.QQSearchParameters)
            If ViewState IsNot Nothing Then
                ViewState("VR_LandingPage_PreviousSearchParams") = value
            End If
            _PreviousSearchParams = value
        End Set
    End Property

    Public Function GetSelectedAgentUserName() As String
        If Me.ddAgent.SelectedItem.Text.Contains(staffUserNameIndicationText) Then
            ' remove
            Return Me.ddAgent.SelectedItem.Text.Replace(staffUserNameIndicationText, "")
        End If
        Return Me.ddAgent.SelectedItem.Text
    End Function

    Public Function GetSelectedAgentUserID() As Integer
        Dim userID As Integer = -2
        If ddAgent.SelectedValue.NoneAreNullEmptyorWhitespace AndAlso ddAgent.SelectedValue.IsNumeric() AndAlso Integer.TryParse(ddAgent.SelectedValue, userID) Then
            'The tryparse in the If Statement will load up the userID variable if possible, no code really needs to go in here.
        End If
        Return userID
    End Function

    Public Function GetAgencyId() As Int32
        Return DirectCast(Me.Page.Master, VelociRater).AgencyID
    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim masterPage As VelociRater = DirectCast(Me.Page.Master, VR.Web.VelociRater)
        AddHandler masterPage.AgencyIdChanged, AddressOf AgencyChanged

        If Not IsPostBack Then
            Me.divShowArchived.Visible = False
            Me.divSearchByQuoteId.Visible = False

            If DirectCast(Me.Page.Master, VelociRater).IsStaff Then
                Me.divShowArchived.Visible = True
                Me.divSearchByQuoteId.Visible = True
            End If
            InitME()
        End If
    End Sub

    Sub InitME()
        LoadUserNameDropDown()
        LoadStatusDropDown()
        LoadLobDropDown()
    End Sub

    Sub AgencyChanged()
        AgencySwitched = True
        InitME()
        If DirectCast(Me.Page.Master, VelociRater).IsStaff Then
            'ddTimeFrame.SetFromValue("30")
            If Request.QueryString("noSearch") Is Nothing Then
                btnSearch_Click(Nothing, Nothing)
            End If
        Else
            btnSearch_Click(Nothing, Nothing)
        End If
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        Dim searchParams As New Common.QuoteSearch.QQSearchParameters
        Dim lobid As Int32 = 0
        Dim searchType As IFM.VR.Common.QuoteSearch.QuoteSearch.SearchType
        Dim hasValidationError As Boolean = False
        Dim validationMessages As New List(Of String)

        If AgencySwitched = True Then
            If PreviousSearchParams IsNot Nothing Then
                searchParams = PreviousSearchParams
                searchParams.searchInitiatedByAgencySwitch = True
            Else
                Dim chc As New CommonHelperClass

                searchParams.searchInitiatedByAgencySwitch = True
                If Request IsNot Nothing AndAlso Request.QueryString("PageView") IsNot Nothing Then
                    Select Case True
                        Case chc.StringsAreEqual(Me.Request("PageView"), "savedQuotes")
                            searchParams.SearchType = Common.QuoteSearch.QuoteSearch.SearchType.Quotes
                            searchParams.isSavedPage = True
                            searchParams.TimeFrame = 90

                            'added 3/9/2021
                            Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
                            Dim agencyId As Integer = DirectCast(Me.Page.Master, VelociRater).AgencyID
                            If agencyId > 0 AndAlso qqHelper.IsAgencyIdInCancelledSessionList(agencyId.ToString) = True Then
                                Dim cancelledAgencyText As String = QuickQuote.CommonMethods.QuickQuoteHelperClass.CancelledAgencyBaseText()
                                hasValidationError = True
                                validationMessages.Add("New Business Quoting is not permitted on " & cancelledAgencyText & " agencies.")
                            End If
                        Case chc.StringsAreEqual(Me.Request("PageView"), "savedChanges")
                            searchParams.SearchType = Common.QuoteSearch.QuoteSearch.SearchType.Changes
                            searchParams.isSavedPage = True
                        Case chc.StringsAreEqual(Me.Request("PageView"), "billingUpdates")
                            searchParams.SearchType = Common.QuoteSearch.QuoteSearch.SearchType.BillingUpdates
                            searchParams.isSavedPage = True
                    End Select
                End If
            End If
        Else
            Dim statusIds As String = Me.ddStatus.SelectedValue
            Dim agentUserName As String = GetSelectedAgentUserName()
            Dim agentUserNameID As Integer = GetSelectedAgentUserID()
            Dim quoteID As Integer = 0

            searchType = Me.ddType.SelectedValue

            If txtClientName.Text.NoneAreNullEmptyorWhitespace() AndAlso txtClientName.Text.Length < 4 Then
                hasValidationError = True
                validationMessages.Add("Customer Name must be 4 or more characters.")
            End If

            Select Case searchType
                Case Common.QuoteSearch.QuoteSearch.SearchType.All, Common.QuoteSearch.QuoteSearch.SearchType.Policies
                    If txtClientName.Text.IsNullEmptyorWhitespace AndAlso txtQuoteNum.Text.IsNullEmptyorWhitespace Then
                        hasValidationError = True
                        validationMessages.Add("When searching with the type All or Policies, you must enter a Policy/Quote Number or Customer Name to search for.")
                    End If
                Case Common.QuoteSearch.QuoteSearch.SearchType.Quotes 'added 3/9/2021
                    Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
                    Dim agencyId As Integer = DirectCast(Me.Page.Master, VelociRater).AgencyID
                    If agencyId > 0 AndAlso qqHelper.IsAgencyIdInCancelledSessionList(agencyId.ToString) = True Then
                        Dim cancelledAgencyText As String = QuickQuote.CommonMethods.QuickQuoteHelperClass.CancelledAgencyBaseText()
                        hasValidationError = True
                        validationMessages.Add("New Business Quoting is not permitted on " & cancelledAgencyText & " agencies.")
                    End If
            End Select

            If IsNumeric(Me.ddLob.SelectedValue) Then
                lobid = Me.ddLob.SelectedValue
            End If

            If agentUserName = "All Users" Then
                agentUserName = ""
            End If

            If txtQuoteId.Text.NoneAreNullEmptyorWhitespace() AndAlso txtQuoteId.Text.IsNumeric() AndAlso Integer.TryParse(txtQuoteId.Text, quoteID) Then
                searchParams.quoteOrPolicyID = quoteID
            End If

            searchParams.ClientName = Me.txtClientName.Text
            searchParams.QuoteOrPolicyNumber = Me.txtQuoteNum.Text
            searchParams.LobID = lobid
            searchParams.StatusIDs = statusIds
            searchParams.AgentUserName = agentUserName
            searchParams.ShowArchived = Me.chkShowArchived.Checked
            searchParams.TimeFrame = Me.ddTimeFrame.SelectedValue
            searchParams.ShowAllOnPage = Me.chkShowAll.Checked
            searchParams.SearchType = searchType
            searchParams.AgentUserID = agentUserNameID
            searchParams.isSavedPage = False
            searchParams.searchInitiatedByAgencySwitch = False
        End If

        AgencySwitched = False

        If hasValidationError = False Then
            PreviousSearchParams = searchParams
            RaiseEvent SearchRequested(searchParams)
        Else
            Dim displayedErrorMessage As String = "<ul><li class='informationalTextRed'>"
            displayedErrorMessage &= String.Join("</li><li class='informationalTextRed'>", validationMessages)
            displayedErrorMessage &= "</li></ul>"

            Using popup As New PopupMessageObject(Page, displayedErrorMessage, "Search Validation")
                With popup
                    .isFixedPositionOnScreen = True
                    .ZIndexOfPopup = 2 'un-commented 3/9/2021; seems to only work when setting ZIndexOfPopup; setting ZIndexOfOverlay to one less didn't work
                    .isModal = True
                    .Image = PopupMessageObject.ImageOptions.None
                    .hideCloseButton = True
                    .AddButton("OK", True)
                    .CreateDynamicPopUpWindow()
                End With
            End Using
        End If
    End Sub

    Private Function FormatAgentUsername(ByVal username As String, ByVal isStaffUser As String) As String
        If isStaffUser Then
            username = String.Format("{0}{1}", username, staffUserNameIndicationText)
        End If
        Return username
    End Function

    Public Sub LoadUserNameDropDown()
        Try
            Dim startingSelectedVal As String = Me.ddAgent.SelectedValue

            If startingSelectedVal = "-1" And DirectCast(Me.Page.Master, VelociRater).IsStaff = False Then
                startingSelectedVal = Session("DiamondUsername")
            End If

            Me.ddAgent.Items.Clear()
            Me.ddAgent.Items.Add(New ListItem("", ""))

            Using sql As New SQLselectObject(connQQ)
                sql.queryOrStoredProc = "usp_SavedQuotes_GetAgencyAvailableAgents"

                Dim masterPage As VelociRater = DirectCast(Me.Page.Master, VR.Web.VelociRater)
                Dim parms As New ArrayList()
                If masterPage.IsStaff And masterPage.AgencyID < 0 Then
                    parms.Add(New System.Data.SqlClient.SqlParameter("@AgencyID", Nothing))
                Else
                    parms.Add(New System.Data.SqlClient.SqlParameter("@AgencyID", masterPage.AgencyID))
                End If
                Dim supportedLobList_Text As String = ""
                For Each Pair In Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs()
                    supportedLobList_Text += Pair.Value.ToString() + ","
                Next
                supportedLobList_Text = supportedLobList_Text.Trim().TrimEnd(",")

                parms.Add(New System.Data.SqlClient.SqlParameter("@lobList", supportedLobList_Text.Trim(",")))
                sql.parameters = parms

                Using reader As System.Data.SqlClient.SqlDataReader = sql.GetDataReader()
                    If Not sql.hasError Then
                        If reader.HasRows Then
                            While reader.Read()
                                Dim isStaffUser As Boolean = CBool(reader.GetInt32(1))
                                Dim userName As String = If(reader.IsDBNull(0), "", reader.GetString(0))
                                Dim userID As Integer = If(reader.IsDBNull(0), 0, reader.GetInt32(2))
                                'Me.ddAgent.Items.Add(New ListItem(FormatAgentUsername(userName, isStaffUser), userName))
                                Me.ddAgent.Items.Add(New ListItem(FormatAgentUsername(userName, isStaffUser), userID))
                            End While
                        End If
                    End If
                End Using
            End Using

            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddAgent, startingSelectedVal)
        Catch ex As Exception
#If DEBUG Then
            Debugger.Break()
#End If
            If IFM.VR.Web.Helpers.WebHelper_Personal.IsTesting() Then
                MessageBoxVRPers.Show(ex.Message, Response, ScriptManager.GetCurrent(Me.Page), Me)
            Else
                MessageBoxVRPers.Show("Could not load username drop down.", Response, ScriptManager.GetCurrent(Me.Page), Me)
            End If
        End Try

    End Sub

    Public Sub LoadStatusDropDown()
        Me.ddStatus.Items.Clear()

        ' The val does not mean anything - we will match on TEXT :(
        Me.ddStatus.Items.Add(New ListItem("All Statuses", ""))
        Dim index As Int32 = 1
        For Each status In VR.Common.QuoteSearch.QuoteSearch.FriendlyStatusesForGlobalSearchDropdown
            Dim isStaff As Boolean = DirectCast(Me.Page.Master, VelociRater).IsStaff
            If isStaff = False And status.Key.ToLower().Trim() = "archive" Then
                Continue For
            End If
            Dim csv As String = ""
            For Each Val As Int32 In status.Value
                csv += Val.ToString() + ","
            Next
            Me.ddStatus.Items.Add(New ListItem(status.Key, csv.Trim(",")))
            index += 1
        Next
    End Sub

    Public Sub LoadLobDropDown()
        Me.ddLob.Items.Clear()
        If VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs().Count() > 1 Then
            Me.ddLob.Items.Add("All LOBs")
        End If

        Dim personalLines = IFM.VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs(Common.QuoteSearch.QuoteSearch.LobCategory.Personal)
        Dim commLines = IFM.VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs(Common.QuoteSearch.QuoteSearch.LobCategory.Commercial)
        Dim farmLines = IFM.VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs(Common.QuoteSearch.QuoteSearch.LobCategory.Farm)
        Dim umbrellaLines = IFM.VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs(Common.QuoteSearch.QuoteSearch.LobCategory.Umbrella)

        If personalLines.Any() Then
            Dim hI As New ListItem("All Personal Lines", IFM.VR.Common.QuoteSearch.QuoteSearch.LobCategory.Personal)
            Me.ddLob.Items.Add(hI)
            For Each lob In personalLines
                Dim i As New ListItem(lob.Key, lob.Value)
                Me.ddLob.Items.Add(i)
            Next
        End If

        If commLines.Any() Then
            'Updated 08/24/2021 for Bug 64324 MLW
            'Filter out new MultiState LOB's from being options
            commLines = MultistateHelper.FilterOutMultistateLOB(commLines)
            Dim hI As New ListItem("All Commercial Lines", IFM.VR.Common.QuoteSearch.QuoteSearch.LobCategory.Commercial)
            Me.ddLob.Items.Add(hI)
            For Each lob In commLines
                Dim i As New ListItem(lob.Key, lob.Value)
                Me.ddLob.Items.Add(i)
            Next
        End If

        If farmLines.Any() Then
            'Updated 08/24/2021 for Bug 64324 MLW
            'Filter out new MultiState LOB's from being options
            farmLines = MultistateHelper.FilterOutMultistateLOB(farmLines)
            Dim hI As New ListItem("All Farm Lines", IFM.VR.Common.QuoteSearch.QuoteSearch.LobCategory.Farm)
            Me.ddLob.Items.Add(hI)
            For Each lob In farmLines
                Dim i As New ListItem(lob.Key, lob.Value)
                Me.ddLob.Items.Add(i)
            Next
        End If

        If umbrellaLines.Any() Then
            Dim hI As New ListItem("Umbrella Lines", IFM.VR.Common.QuoteSearch.QuoteSearch.LobCategory.Umbrella)
            Me.ddLob.Items.Add(hI)
            For Each lob In umbrellaLines
                Dim i As New ListItem(lob.Key, lob.Value)
                Me.ddLob.Items.Add(i)
            Next
        End If

        'For Each lob In VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs()
        '    Me.ddLob.Items.Add(New ListItem(lob.Key, lob.Value))
        'Next
    End Sub
End Class