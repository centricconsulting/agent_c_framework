Imports QuickQuote.CommonMethods
Imports PopupMessageClass
Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonObjects
Imports IFM.VR.Validation.ObjectValidation
Imports IFM.VR.Common.Helpers
Imports IFM.VR.Common.Helpers.CL


Public Class VelociRater
    Inherits BaseMasterPage

    Const detectStatus As String = "detectStatus"

    ''' <summary>
    ''' Fires when the agency dropdown selection changes. Will be -1 is select all agencies is selected.
    ''' </summary>
    ''' <param name="newAgencyId"></param>
    ''' <remarks></remarks>
    Public Event AgencyIdChanged(newAgencyId As Int32)

    Dim qqHelper As New QuickQuoteHelperClass()
    Private Const staffUserNameIndicationText As String = " (IFM_STAFF)"
    Public Shared ReadOnly connQQ As String = System.Configuration.ConfigurationManager.AppSettings("connQQ")

    Public ReadOnly Property AgencyID As Int32
        Get

            Try
                Return If(String.IsNullOrWhiteSpace(Me.ddAgencies.SelectedValue) = False AndAlso Me.ddAgencies.SelectedValue <> "null", CInt(Me.ddAgencies.SelectedValue), -1)
            Catch ex As Exception
                Return 0
            End Try

        End Get
    End Property

    Public ReadOnly Property AgencyCode As String
        Get
            Try
                Return If(Me.ddAgencies.SelectedItem.Text.Contains("|"), Me.ddAgencies.SelectedItem.Text.Split("|")(0).Trim(), "")
            Catch ex As Exception
                Return 0
            End Try

        End Get
    End Property

    Public ReadOnly Property StartUpScriptManager As ctlPageStartupScript
        Get
            Return Me.ctlPageStartupScript
        End Get
    End Property

    Public ReadOnly Property ValidationSummary As ctlValidationSummary
        Get
            Return Me.ctlValidationSummary
        End Get
    End Property

    Private Function FormatAgentUsername(ByVal username As String, ByVal isStaffUser As String) As String
        If isStaffUser Then
            username = String.Format("{0}{1}", username, staffUserNameIndicationText)
        End If
        Return username
    End Function

    Public Function GetSelectedAgentUserName() As String
        If Me.ddAgent.SelectedValue.Contains(staffUserNameIndicationText) Then
            ' remove
            Return Me.ddAgent.SelectedValue.Replace(staffUserNameIndicationText, "")
        End If
        Return Me.ddAgent.SelectedValue
    End Function

    Private Sub VelociRater_Init(sender As Object, e As EventArgs) Handles Me.Init
        LoadAgencyDropDown()
        Dim changeSearchTypeAccordingToPageCurrentlyOn As Boolean = False
        If IFM.VR.Common.Helpers.GenericHelper.GetAppSettingsValueForBoolean("VR_GlobalSearch_ChangeSearchTypeAccordingToPageCurrentlyOn", changeSearchTypeAccordingToPageCurrentlyOn).ToString() Then
            hdnChangeSearchTypeAccordingToPageCurrentlyOn.Value = changeSearchTypeAccordingToPageCurrentlyOn
        Else
            hdnChangeSearchTypeAccordingToPageCurrentlyOn.Value = False
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim qqHelper As New QuickQuoteHelperClass
        If Not IsPostBack Then
            If Session("DiamondUsername") IsNot Nothing Then
                If IsStaff Then
                    Me.lblWelcomeUser.Text = String.Format("Welcome {0}(IFM_STAFF)", Session("DiamondUsername"))
                Else
                    Me.lblWelcomeUser.Text = String.Format("Welcome {0}", Session("DiamondUsername"))
                End If
                'LoadAgencyDropDown()

                If (TypeOf Me.Page Is MyVelocirater) Then
                    Me.ddAgencies.Enabled = True
                    If Session("dd_agencies_selectedIndex") IsNot Nothing Then
                        Try
                            Me.ddAgencies.SelectedIndex = CInt(Session("dd_agencies_selectedIndex"))
                        Catch ex As Exception
                        End Try
                    End If
                Else
                    ' some page other than myvelocirater page
                    Try
                        ' if a quote is found then use its agencyid
                        If Me.Quote IsNot Nothing Then
                            Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddAgencies, Me.Quote.AgencyId)
                            Me.ddAgencies.Enabled = False
                        End If
                    Catch ex As Exception

                    End Try
                End If

                ' with the recent activity you can't assume that the last url it was at represents the proper status of the quote
                ' so we send this 'detectstatus' in the url and this logic does a lookup to find the right url for the quote in its current status and redirects to that page
                If Request.QueryString(detectStatus) IsNot Nothing Then
                    Dim results = VR.Common.QuoteSearch.QuoteSearch.SearchForQuotes(QuoteId, "", DirectCast(Me.Page.Master, VR.Web.VelociRater).AgencyID, "", "", Nothing, "", "", DirectCast(Me.Page.Master, VR.Web.VelociRater).IsStaff, Not False, 0, True, qqHelper.CanUserAccessEmployeePolicies(), CInt(System.Web.HttpContext.Current.Session("DiamondUserId")))

                    If results.Any() AndAlso results.Count = 1 Then
                        If String.IsNullOrWhiteSpace(results(0).ViewUrl) = False Then
                            Dim currentURL As String = Request.RawUrl.Replace("&" + detectStatus + "=y", "")
                            currentURL = If(currentURL.StartsWith("/"), currentURL.Remove(0, 1), currentURL)
                            If currentURL.ToLower().Trim() <> results(0).ViewUrl.ToLower().Trim() Then ' no need to redirect at all in some cases
                                Response.Redirect(results(0).ViewUrl, True)
                            End If
                        End If
                    End If
                End If
            Else
                Me.lblWelcomeUser.Text = "Welcome Unknown User"
            End If

            'show build date on test environments
            Me.DivVrBuildDate.Visible = Me.IsInTestEnvironment

            Dim MPR As New MasterPageRoutines
            Dim webScrollContent As String = MPR.GetWebScrollContentForMasterPageType(MasterPageRoutines.MasterPageType.VelociRaterMasterPage)
            If webScrollContent <> "" Then
                scrollRow.Visible = True
                Me.WebScrollArea.InnerHtml = webScrollContent
            End If

            ' Start App/Quote Kill LOGIC --------------------------
            If Session("QuoteStopOrKill_ShowMsg") IsNot Nothing AndAlso String.IsNullOrEmpty(Session("QuoteStopOrKill_ShowMsg")) = False AndAlso CBool(Session("QuoteStopOrKill_ShowMsg")) = True Then
                Dim titleMsg = If(String.IsNullOrEmpty(Session("QuoteStopOrKill_titleMsg")) = False, Session("QuoteStopOrKill_titleMsg").ToString, "Important Notice")
                Dim bodyMsg = If(String.IsNullOrEmpty(Session("QuoteStopOrKill_bodyMsg")) = False, Session("QuoteStopOrKill_bodyMsg").ToString, "This quote is not eligible for coverage with Indiana Farmers Mutual Insurance Company.")
                Using popup As New PopupMessageObject(Page, bodyMsg, titleMsg)
                    With popup
                        .isFixedPositionOnScreen = True
                        .ZIndexOfPopup = 2
                        .isModal = True
                        .Image = PopupMessageObject.ImageOptions.None
                        .hideCloseButton = True
                        .AddButton("OK", True)
                        '.AddButton("OK", True, "", System.Configuration.ConfigurationManager.AppSettings("QuickQuote_Personal_HomePage"))
                        ' Foward to home if titlebar "X" is visible
                        '.AddPopupEvent(PopupMessageObject.PopupEventType.close, "window.location.replace('" + System.Configuration.ConfigurationManager.AppSettings("QuickQuote_Personal_HomePage") + "')")
                        .CreateDynamicPopUpWindow()
                    End With
                End Using
            End If
            'Clear these session values
            Session.Remove("QuoteStopOrKill_ShowMsg")
            Session.Remove("QuoteStopOrKill_titleMsg")
            Session.Remove("QuoteStopOrKill_bodyMsg")

            ' End App/Quote Kill LOGIC --------------------------

        End If

        Dim chc As New CommonHelperClass
        For Each IFMVersion As System.Enum In System.Enum.GetValues(GetType(QuickQuoteHelperClass.LOBIFMVersions))
            Dim myHiddenLobVersionDate As New HiddenField
            myHiddenLobVersionDate.ID = "hdnLOBVersionDate_" & IFMVersion.ToString
            myHiddenLobVersionDate.ClientIDMode = ClientIDMode.Static
            myHiddenLobVersionDate.Value = qqHelper.GetIFMLOBVersionDate(IFMVersion)
            pnlLOBVersionFields.Controls.Add(myHiddenLobVersionDate)

            Dim myHiddenLOBVersionAllowedLOBs As New HiddenField
            myHiddenLOBVersionAllowedLOBs.ID = "hdnLOBVersionAllowedLOBs_" & IFMVersion.ToString
            myHiddenLOBVersionAllowedLOBs.ClientIDMode = ClientIDMode.Static
            myHiddenLOBVersionAllowedLOBs.Value = String.Join(",", qqHelper.GetIFMLOBVersionLOBTypeRelationsListByEnum([Enum].Parse(GetType(QuickQuoteHelperClass.LOBIFMVersions), IFMVersion.ToString)).ToArray())
            pnlLOBVersionFields.Controls.Add(myHiddenLOBVersionAllowedLOBs)
        Next

        LoadScriptStaticData() ' loads static data into javascript variables
        LogQuoteActivity() ' Must be at bottom - after the agency drop down is loaded

        If Quote IsNot Nothing Then
            If HotelMotelRemovedRisks.IsHotelMotelRemovedRisksAvailable(Quote) = True Then
                If AllLines.IRPM_ClasscodeCheck.IsUnwantedClassCodePresent(Quote) Then
                    Using popup As New PopupMessageObject(Page, "We are no longer accepting hotel-motel business operations. Please contact your Field Sales Marketing Representative or your Underwriter with questions.", "Hotel-Motel Business Operations")
                        With popup
                            .isFixedPositionOnScreen = True
                            .ZIndexOfPopup = 2 
                            .isModal = True
                            .Image = PopupMessageObject.ImageOptions.None
                            .hideCloseButton = True
                            .AddButton("OK", True, "", System.Configuration.ConfigurationManager.AppSettings("Agent_MyVelociraterPage"))
                            .width = 300
                            .height = 200
                            .CreateDynamicPopUpWindow()
                        End With
                    End Using
                End If
            End If
        End If
    End Sub

    Protected Function GetDiamondSystemDate() As String
        Return IFM.VR.Common.Helpers.GenericHelper.GetDiamondSystemDate().ToShortDateString()
    End Function

    Protected Function GetCompanyId() As String
        If Me.Quote IsNot Nothing AndAlso Not String.IsNullOrEmpty(Quote.CompanyId) Then
            ''Return "VR Company ID: " & Quote.CompanyId & "Diamond CompanyID: " & Quote.Database_DiaCompanyId
            Return String.Format("VR Company ID: {0} / Diamond Company ID: {1}", Quote.CompanyId, Quote.Database_DiaCompanyId)
        End If
        Return ""
    End Function

    Public Function GetRecentHistoryItems() As String
        Dim menuHtml As New StringBuilder()

        If Session("ss_RecentQuoteActivity") IsNot Nothing Then
            Dim sessionActivity = DirectCast(Session("ss_RecentQuoteActivity"), List(Of SessionQuoteHistoryItem))
            If sessionActivity.Any() Then
                sessionActivity.Reverse()
                menuHtml.Append("<ul>")
                For Each i In sessionActivity
                    If i.QuoteId = Me.QuoteId Then
                        menuHtml.Append("<li>Current Quote</li>")
                    Else
                        menuHtml.Append(String.Format("<li><a href=""{0}"">{1} {2}</a></li>", i.LastUrl, i.QuoteNumber, i.ClientName))
                    End If
                Next
                menuHtml.Append("</ul>")
                sessionActivity.Reverse()
            End If
        End If

        Return menuHtml.ToString()
    End Function

    Public Function GetSupportedNewQuoteMenuItem() As String
        Dim html As New StringBuilder()
        Dim personalLines = IFM.VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs(Common.QuoteSearch.QuoteSearch.LobCategory.Personal)
        Dim commLines = IFM.VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs(Common.QuoteSearch.QuoteSearch.LobCategory.Commercial)
        Dim farmLines = IFM.VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs(Common.QuoteSearch.QuoteSearch.LobCategory.Farm)
        Dim umbrellaLines = IFM.VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs(Common.QuoteSearch.QuoteSearch.LobCategory.Umbrella)

        If personalLines.Any() Then
            html.Append("<li>")
            html.Append("<span style=""font-weight:bold;cursor:default;"">Personal Lines</span>")
            html.Append("</li>")
            For Each lob In personalLines
                html.Append(String.Format("<li style=""margin-left:15px"" title=""Start a new {0} quote""><a href=""MyVelocirater.aspx?newQuote={1}"">{0}</a></li>", lob.Key, lob.Value))
            Next

        End If

        If commLines.Any() Then
            'Updated 08/10/2021 for Bug 63958 MLW
            'Filter out new MultiState LOB's from Being options for New Quotes
            commLines = MultistateHelper.FilterOutMultistateLOB(commLines)
            html.Append("<li>")
            html.Append("<span style=""font-weight:bold;cursor:default;"">Commercial Lines</span>")
            html.Append("</li>")
            For Each l In commLines
                html.Append(String.Format("<li style=""margin-left:15px"" title=""Start a new {0} quote""><a href=""MyVelocirater.aspx?newQuote={1}"">{0}</a></li>", l.Key, l.Value))
            Next

        End If

        If farmLines.Any() Then
            'Filter out new MultiState LOB's from Being options for New Quotes
            farmLines = MultistateHelper.FilterOutMultistateLOB(farmLines)
            html.Append("<li>")
            html.Append("<span style=""font-weight:bold;cursor:default;"">Farm Lines</span>")
            html.Append("</li>")
            For Each l In farmLines
                html.Append(String.Format("<li style=""margin-left:15px"" title=""Start a new {0} quote""><a href=""MyVelocirater.aspx?newQuote={1}"">{0}</a></li>", l.Key, l.Value))
            Next
            For Each lob In VR.Common.QuoteSearch.QuoteSearch.GetVrSupportedLobs({20})
                html.Append(String.Format("<li style=""margin-left:15px"" title=""Start a new {0} quote""><a href=""MyVelocirater.aspx?newQuote={1}"">Farm {0}</a></li>", lob.Key, lob.Value))
            Next

        End If

        If umbrellaLines.Any() Then
            html.Append("<li>")
            html.Append("<span style=""font-weight:bold;cursor:default;"">Umbrella Lines</span>")
            html.Append("</li>")
            For Each l In umbrellaLines
                html.Append(String.Format("<li style=""margin-left:15px"" title=""Start a new {0} quote""><a href=""MyVelocirater.aspx?newQuote={1}"">{0}</a></li>", l.Key, l.Value))
            Next

        End If

        Return html.ToString()
    End Function

    Private Sub LogQuoteActivity()
        Dim sessionActivity As List(Of SessionQuoteHistoryItem) = Nothing
        If Session("ss_RecentQuoteActivity") Is Nothing Then
            Session("ss_RecentQuoteActivity") = New List(Of SessionQuoteHistoryItem)()
        End If
        sessionActivity = DirectCast(Session("ss_RecentQuoteActivity"), List(Of SessionQuoteHistoryItem))

        If String.IsNullOrWhiteSpace(Me.QuoteId) = False Then
            'remove current quote from anywhere in the list and later you will move it to the top by adding it again
            Helpers.WebHelper_Personal.RemoveQuoteIdFromSessionHistory(Session, Me.QuoteId)
            'add
            If Me.Quote IsNot Nothing Then
                Dim clientName As String = "No Policyholder"

                If Me.Quote.Policyholder IsNot Nothing AndAlso Me.Quote.Policyholder.Name IsNot Nothing Then
                    If Me.Quote.Policyholder.Name.TypeId = "1" Then
                        'personal
                        clientName = String.Format("{0} {1}", Me.Quote.Policyholder.Name.FirstName, Me.Quote.Policyholder.Name.LastName)
                    Else
                        'business
                        clientName = Me.Quote.Policyholder.Name.CommercialName1
                    End If
                End If

                Dim url As String = Request.RawUrl
                If url.Contains(detectStatus + "=y") = False Then
                    url = url + "&" + detectStatus + "=y"
                End If

                sessionActivity.Add(New SessionQuoteHistoryItem(If(String.IsNullOrWhiteSpace(Me.Quote.QuoteNumber),
                                                                   [Enum].GetName(GetType(QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType), Me.Quote.LobType),
                                                                   Me.Quote.QuoteNumber),
                                                               clientName,
                                                               Me.QuoteId,
                                                               url,
                                                               Me.Quote.AgencyId,
                                                               Me.Quote.AgencyCode))
            Else
                'This should not happen, in fact lets just not log this item if it does - Matt A 11-1-14
                'sessionActivity.Add(New SessionQuoteHistoryItem("unknown", "unknown", Me.QuoteId, Request.RawUrl, "", ""))
            End If
        End If
    End Sub

    Private Sub LoadScriptStaticData()
        Dim lob As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType = QuickQuoteObject.QuickQuoteLobType.None
        If Me.Quote IsNot Nothing Then
            lob = Me.Quote.LobType
        End If
        Dim qqHelper As New QuickQuoteHelperClass
        Dim SD_bodyTypeId_Van As String = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, "Van", lob)
        Dim SD_bodyTypeId_MotorCycle As String = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, "Motorcycle", lob)
        Dim SD_bodyTypeId_MotorHome As String = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, "Motor Home", lob)
        Dim SD_bodyTypeId_PICKUPWCAMPER As String = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, "PICKUP W/CAMPER", lob)
        Dim SD_bodyTypeId_PICKUPWOCamper As String = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, "PICKUP W/O CAMPER", lob)
        Dim SD_bodyTypeId_RecTrailer As String = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, "Rec. Trailer", lob)
        Dim SD_bodyTypeId_ClassicAuto As String = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, "Classic Auto", lob)
        Dim SD_bodyTypeId_OtherTrailer As String = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, "Other Trailer", lob)
        Dim SD_bodyTypeId_AntiqueAuto As String = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, "Antique Auto", lob)
        Dim SD_vehUseTypeId_Farm As String = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.VehicleUseTypeId, "Farm", lob)

        Me.StartUpScriptManager.AddVariableLine("var SD_bodyTypeId_Van = '" + SD_bodyTypeId_Van + "';") '16
        Me.StartUpScriptManager.AddVariableLine("var SD_bodyTypeId_MotorCycle = '" + SD_bodyTypeId_MotorCycle + "';") '42
        Me.StartUpScriptManager.AddVariableLine("var SD_bodyTypeId_MotorHome = '" + SD_bodyTypeId_MotorHome + "';") '18
        Me.StartUpScriptManager.AddVariableLine("var SD_bodyTypeId_PICKUPWCAMPER = '" + SD_bodyTypeId_PICKUPWCAMPER + "';") '39
        Me.StartUpScriptManager.AddVariableLine("var SD_bodyTypeId_PICKUPWOCamper = '" + SD_bodyTypeId_PICKUPWOCamper + "';") '40
        Me.StartUpScriptManager.AddVariableLine("var SD_bodyTypeId_RecTrailer = '" + SD_bodyTypeId_RecTrailer + "';") '19
        Me.StartUpScriptManager.AddVariableLine("var SD_bodyTypeId_ClassicAuto = '" + SD_bodyTypeId_ClassicAuto + "';") '24
        Me.StartUpScriptManager.AddVariableLine("var SD_bodyTypeId_OtherTrailer = '" + SD_bodyTypeId_OtherTrailer + "';") '20
        Me.StartUpScriptManager.AddVariableLine("var SD_bodyTypeId_AntiqueAuto = '" + SD_bodyTypeId_AntiqueAuto + "';") '22

        Dim SD_vehicleUseId_Business As String = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.VehicleUseTypeId, "Business", lob)
        Dim SD_vehicleUseId_Farm As String = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.VehicleUseTypeId, "Farm", lob)

        Me.StartUpScriptManager.AddVariableLine("var SD_vehicleUseId_Business = '" + SD_vehicleUseId_Business + "';") '2
        Me.StartUpScriptManager.AddVariableLine("var SD_vehicleUseId_Farm = '" + SD_vehicleUseId_Farm + "';") '4

        Dim SD_RelationshipTypeId_Policyholder As String = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteDriver, QuickQuoteHelperClass.QuickQuotePropertyName.RelationshipTypeId, "Policyholder", lob)
        Dim SD_RelationshipTypeId_Policyholder2 As String = qqHelper.GetStaticDataValueForText(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteDriver, QuickQuoteHelperClass.QuickQuotePropertyName.RelationshipTypeId, "Policyholder #2", lob)

        Me.StartUpScriptManager.AddVariableLine("var SD_RelationshipTypeId_Policyholder = '" + SD_RelationshipTypeId_Policyholder + "';") '8
        Me.StartUpScriptManager.AddVariableLine("var SD_RelationshipTypeId_Policyholder2 = '" + SD_RelationshipTypeId_Policyholder2 + "';") '5

        Me.StartUpScriptManager.AddVariableLine("var isStaff = " + Me.IsStaff.ToString().ToLower() + ";")
        Me.StartUpScriptManager.AddVariableLine("var master_AgencyId = " + Me.AgencyID.ToString().ToLower() + ";")
        'Me.StartUpScriptManager.AddVariableLine("ifm.vr.currentQuote.agencyId = " + Me.AgencyID.ToString().ToLower() + ";") 'moved below 6/27/2019


        Me.StartUpScriptManager.AddVariableLine("var MasterPage_isLocal = " + Request.IsLocal.ToString().ToLower() + ";")

        Me.StartUpScriptManager.AddVariableLine("var ThemeOrange = '<link id=""ThemeOrange"" href=""" + ResolveClientUrl("~/styles/orange/jquery-ui.css") + """ rel=""stylesheet"" />';")
        ' New Themes CAH
        Me.StartUpScriptManager.AddVariableLine("var ThemeEndorsement = '<link id=""ThemeEndorsement"" href=""" + ResolveClientUrl("~/styles/corpEndorsement/jquery-ui.css") + """ rel=""stylesheet"" />';")
        Me.StartUpScriptManager.AddVariableLine("var ThemeReadOnly = '<link id=""ThemeReadOnly"" href=""" + ResolveClientUrl("~/styles/corpReadOnly/jquery-ui.css") + """ rel=""stylesheet"" />';")
        Me.StartUpScriptManager.AddVariableLine("var ThemePayplanChange = '<link id=""ThemePayplanChange"" href=""" + ResolveClientUrl("~/styles/corpBilling/jquery-ui.css") + """ rel=""stylesheet"" />';")
        Me.StartUpScriptManager.AddVariableLine("var ThemeSearchResults = '<link id=""ThemeSearchResults"" href=""" + ResolveClientUrl("~/styles/corpSearchResults/jquery-ui.css") + """ rel=""stylesheet"" />';")

        'Me.StartUpScriptManager.AddVariableLine("var ThemeChanges = '<link id=""ThemeChanges"" href=""" + ResolveClientUrl("~/styles/changes/jquery-ui.css") + """ rel=""stylesheet"" />';")

        'Me.StartUpScriptManager.AddVariableLine("var ThemeGreen = '<link id=""ThemeGreen"" href=""" + ResolveClientUrl("~/styles/green/jquery-ui.css") + """ rel=""stylesheet"" />';")

        'Me.StartUpScriptManager.AddVariableLine("var ThemeStPatty = '<link id=""ThemeStPatty"" href=""" + ResolveClientUrl("~/styles/stpatty/jquery-ui.css") + """ rel=""stylesheet"" />';")

        'Me.StartUpScriptManager.AddVariableLine("var ThemeOrange_Shine = '<link id=""ThemeOrange_Shine"" href=""" + ResolveClientUrl("~/styles/orange_shine/jquery-ui.css") + """ rel=""stylesheet"" />';")
        'Me.StartUpScriptManager.AddVariableLine("var ThemeVader = '<link id=""ThemeVader"" href=""" + ResolveClientUrl("~/styles/vader/jquery-ui.css") + """ rel=""stylesheet"" />';")
        'Me.StartUpScriptManager.AddVariableLine("var ThemeDarkness = '<link id=""ThemeDarkness""  href=""" + ResolveClientUrl("~/styles/darkness/jquery-ui.css") + """ rel=""stylesheet"" />';")
        'Me.StartUpScriptManager.AddVariableLine("var ThemeCupertino = '<link id=""ThemeCupertino""  href=""" + ResolveClientUrl("~/styles/Cupertino/jquery-ui.css") + """ rel=""stylesheet"" />';")
        'Me.StartUpScriptManager.AddVariableLine("var ThemeDarkHive = '<link id=""ThemeDarkHive""  href=""" + ResolveClientUrl("~/styles/DarkHive/jquery-ui.css") + """ rel=""stylesheet"" />';")
        'Me.StartUpScriptManager.AddVariableLine("var ThemeFlick = '<link id=""ThemeFlick""  href=""" + ResolveClientUrl("~/styles/Flick/jquery-ui.css") + """ rel=""stylesheet"" />';")
        'Me.StartUpScriptManager.AddVariableLine("var ThemeOvercast = '<link id=""ThemeOvercast""  href=""" + ResolveClientUrl("~/styles/Overcast/jquery-ui.css") + """ rel=""stylesheet"" />';")
        'Me.StartUpScriptManager.AddVariableLine("var ThemePepper = '<link id=""ThemePepper""  href=""" + ResolveClientUrl("~/styles/Pepper/jquery-ui.css") + """ rel=""stylesheet"" />';")
        'Me.StartUpScriptManager.AddVariableLine("var ThemeRedmond = '<link id=""ThemeRedmond""  href=""" + ResolveClientUrl("~/styles/Redmond/jquery-ui.css") + """ rel=""stylesheet"" />';")
        'Me.StartUpScriptManager.AddVariableLine("var ThemeSmoothness = '<link id=""ThemeSmoothness""  href=""" + ResolveClientUrl("~/styles/Smoothness/jquery-ui.css") + """ rel=""stylesheet"" />';")

        Me.StartUpScriptManager.AddScriptLine("$(""#mainMenu_VR"").menubar();")

        'updated 4/2/2019; note: can also trigger off Child page if needed; create additional themes as needed; defaults for each themeType are set in vr.core.js DefaultTheme
        Dim themeType As String = ""
        Dim pageViewString As String = ""

        If Request IsNot Nothing AndAlso Request.QueryString("PageView") IsNot Nothing Then
            pageViewString = Request.QueryString("PageView")
        End If

        If String.IsNullOrWhiteSpace(ReadOnlyPolicyIdAndImageNum) = False Then
            themeType = "'ReadOnly'"
        ElseIf String.IsNullOrWhiteSpace(EndorsementPolicyIdAndImageNum) = False OrElse pageViewString.Equals("savedChanges", StringComparison.OrdinalIgnoreCase) Then
            themeType = "'Endorsement'"
        ElseIf String.IsNullOrWhiteSpace(QuoteId) = False OrElse pageViewString.Equals("savedQuotes", StringComparison.OrdinalIgnoreCase) Then
            themeType = "'NewBusinessQuote'"
        ElseIf pageViewString.Equals("billingUpdates", StringComparison.OrdinalIgnoreCase) Then
            themeType = "'PayplanChange'"
        End If

        Me.StartUpScriptManager.AddScriptLine("ifm.vr.theming.LoadThemeFromCookie(" & themeType & ");")

        Me.StartUpScriptManager.AddScriptLine("$(""#jQueryMenu_VR"").show();")

        If IsPostBack Then
            Me.StartUpScriptManager.AddVariableLine("var isPostBack = 1;")
        Else
            Me.StartUpScriptManager.AddVariableLine("var isPostBack = 0;")
        End If

        Me.StartUpScriptManager.AddVariableLine("var QuickQuote_PPA_Input = '" + System.Configuration.ConfigurationManager.AppSettings("QuickQuote_PPA_Input") + "?quoteid=';")
        Me.StartUpScriptManager.AddVariableLine("ifm.vr.workflow.ppa_Input = '" + System.Configuration.ConfigurationManager.AppSettings("QuickQuote_PPA_Input") + "?quoteid=';")

        Me.StartUpScriptManager.AddVariableLine("var QuickQuote_PPA_App = '" + System.Configuration.ConfigurationManager.AppSettings("QuickQuote_PPA_App") + "?quoteid=';")
        Me.StartUpScriptManager.AddVariableLine("ifm.vr.workflow.ppa_App = '" + System.Configuration.ConfigurationManager.AppSettings("QuickQuote_PPA_App") + "?quoteid=';")

        Me.StartUpScriptManager.AddVariableLine("var QuickQuote_HOM_Input = '" + System.Configuration.ConfigurationManager.AppSettings("QuickQuote_HOM_Input") + "?quoteid=';")
        Me.StartUpScriptManager.AddVariableLine("ifm.vr.workflow.hom_Input = '" + System.Configuration.ConfigurationManager.AppSettings("QuickQuote_HOM_Input") + "?quoteid=';")

        Me.StartUpScriptManager.AddVariableLine("var QuickQuote_HOM_App = '" + System.Configuration.ConfigurationManager.AppSettings("QuickQuote_HOM_App") + "?quoteid=';")
        Me.StartUpScriptManager.AddVariableLine("ifm.vr.workflow.hom_App = '" + System.Configuration.ConfigurationManager.AppSettings("QuickQuote_HOM_App") + "?quoteid=';")

        Me.StartUpScriptManager.AddVariableLine("var QuickQuote_FAR_Input = '" + System.Configuration.ConfigurationManager.AppSettings("QuickQuote_FAR_Input") + "?quoteid=';")
        Me.StartUpScriptManager.AddVariableLine("ifm.vr.workflow.far_Input = '" + System.Configuration.ConfigurationManager.AppSettings("QuickQuote_FAR_Input") + "?quoteid=';")

        Me.StartUpScriptManager.AddVariableLine("var QuickQuote_FAR_App = '" + System.Configuration.ConfigurationManager.AppSettings("QuickQuote_FAR_App") + "?quoteid=';")
        Me.StartUpScriptManager.AddVariableLine("ifm.vr.workflow.far_App = '" + System.Configuration.ConfigurationManager.AppSettings("QuickQuote_FAR_App") + "?quoteid=';")

        Me.StartUpScriptManager.AddVariableLine("var QuickQuote_DFR_Input = '" + System.Configuration.ConfigurationManager.AppSettings("QuickQuote_DFR_Input") + "?quoteid=';")
        Me.StartUpScriptManager.AddVariableLine("ifm.vr.workflow.dfr_Input = '" + System.Configuration.ConfigurationManager.AppSettings("QuickQuote_DFR_Input") + "?quoteid=';")

        Me.StartUpScriptManager.AddVariableLine("var QuickQuote_DFR_App = '" + System.Configuration.ConfigurationManager.AppSettings("QuickQuote_DFR_App") + "?quoteid=';")
        Me.StartUpScriptManager.AddVariableLine("ifm.vr.workflow.dfr_App = '" + System.Configuration.ConfigurationManager.AppSettings("QuickQuote_DFR_App") + "?quoteid=';")

        If String.IsNullOrWhiteSpace(Me.QuoteId) = False AndAlso Me.Quote IsNot Nothing Then


            Me.StartUpScriptManager.AddVariableLine("var master_LobId = '" + Me.Quote.LobId + "';")
            Me.StartUpScriptManager.AddVariableLine("ifm.vr.currentQuote.lobId = '" + Me.Quote.LobId + "';")

            Me.StartUpScriptManager.AddVariableLine("var master_quoteID = '" + Me.QuoteId.Trim() + "';")
            Me.StartUpScriptManager.AddVariableLine("ifm.vr.currentQuote.quoteId = '" + Me.QuoteId + "';")

            Me.StartUpScriptManager.AddVariableLine("var master_effectiveDate = '" + Me.Quote.EffectiveDate.Trim() + "';")
            Me.StartUpScriptManager.AddVariableLine("ifm.vr.currentQuote.effectiveDate = '" + Me.Quote.EffectiveDate.Trim() + "';")
        Else
            Me.StartUpScriptManager.AddVariableLine("var master_LobId = '';")
            Me.StartUpScriptManager.AddVariableLine("var master_quoteID = '';")
            Me.StartUpScriptManager.AddVariableLine("var master_effectiveDate = '';")
        End If
        'updated 6/27/2019 for Endorsements
        Dim quoteIdOrPolicyIdAndImageNum As String = ""
        If Me.Quote IsNot Nothing Then
            Me.StartUpScriptManager.AddVariableLine($"ifm.vr.currentQuote.isEndorsement = {(Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote).ToString.ToLower()};")
            If qqHelper.IsPositiveIntegerString(Me.Quote.AgencyId) = True Then
                Me.StartUpScriptManager.AddVariableLine("ifm.vr.currentQuote.agencyId = " + CInt(Me.Quote.AgencyId).ToString() + ";")
            Else
                Me.StartUpScriptManager.AddVariableLine("ifm.vr.currentQuote.agencyId = " + Me.AgencyID.ToString().ToLower() + ";")
            End If

            Me.StartUpScriptManager.AddVariableLine("var master_LobId = '" + Me.Quote.LobId + "';")
            Me.StartUpScriptManager.AddVariableLine("ifm.vr.currentQuote.lobId = '" + Me.Quote.LobId + "';")


            Me.StartUpScriptManager.AddVariableLine("var master_quoteID = '" + Me.QuoteId.Trim() + "';")
            Me.StartUpScriptManager.AddVariableLine("ifm.vr.currentQuote.quoteId = '" + Me.QuoteId + "';")

            Me.StartUpScriptManager.AddVariableLine("var master_effectiveDate = '" + Me.Quote.EffectiveDate.Trim() + "';")
            Me.StartUpScriptManager.AddVariableLine("ifm.vr.currentQuote.effectiveDate = '" + Me.Quote.EffectiveDate.Trim() + "';")

            If Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote OrElse Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                If Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote AndAlso String.IsNullOrWhiteSpace(Me.EndorsementPolicyIdAndImageNum) = False Then
                    quoteIdOrPolicyIdAndImageNum = Me.EndorsementPolicyIdAndImageNum
                ElseIf Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage AndAlso String.IsNullOrWhiteSpace(Me.ReadOnlyPolicyIdAndImageNum) = False Then
                    quoteIdOrPolicyIdAndImageNum = Me.ReadOnlyPolicyIdAndImageNum
                ElseIf qqHelper.IsPositiveIntegerString(Me.Quote.PolicyId) = True Then
                    quoteIdOrPolicyIdAndImageNum = CInt(Me.Quote.PolicyId).ToString & "|" & qqHelper.IntegerForString(Me.Quote.PolicyImageNum).ToString
                End If
            Else
                If String.IsNullOrWhiteSpace(Me.QuoteId) = False Then
                    quoteIdOrPolicyIdAndImageNum = Me.QuoteId
                ElseIf qqHelper.IsPositiveIntegerString(Me.Quote.Database_QuoteId) = True Then
                    quoteIdOrPolicyIdAndImageNum = CInt(Me.Quote.Database_QuoteId).ToString
                End If
            End If
        Else
            Me.StartUpScriptManager.AddVariableLine("ifm.vr.currentQuote.agencyId = " + Me.AgencyID.ToString().ToLower() + ";")
            Me.StartUpScriptManager.AddVariableLine("var master_LobId = '';")
            Me.StartUpScriptManager.AddVariableLine("ifm.vr.currentQuote.lobId = '';")
            Me.StartUpScriptManager.AddVariableLine("var master_quoteID = '';")
            Me.StartUpScriptManager.AddVariableLine("ifm.vr.currentQuote.quoteId = '';")
            Me.StartUpScriptManager.AddVariableLine("var master_effectiveDate = '';")
            Me.StartUpScriptManager.AddVariableLine("ifm.vr.currentQuote.effectiveDate = '';")
            Me.StartUpScriptManager.AddVariableLine($"ifm.vr.currentQuote.isEndorsement = false;")
        End If
        Me.StartUpScriptManager.AddVariableLine("var master_quoteIdOrPolicyIdAndImageNum = '" + quoteIdOrPolicyIdAndImageNum.Trim() + "';")

        Dim gdhjgdhj = IFM.VR.Common.Helpers.GenericHelper.GetDiamondSystemDate.ToShortDateString()
        Me.StartUpScriptManager.AddVariableLine($"var diamondSystemDate = new Date('{IFM.VR.Common.Helpers.GenericHelper.GetDiamondSystemDate.ToShortDateString()}');")

        If Quote IsNot Nothing Then
            'For MINE SUBSIDENCE on BOP, CPR, CPP, and FAR - Added 11/28/18 for multi state MLW
            If Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then
                Me.StartUpScriptManager.AddVariableLine("var multiStateEnabled = true;")
            Else
                Me.StartUpScriptManager.AddVariableLine("var multiStateEnabled = false;")
            End If
        Else
            Me.StartUpScriptManager.AddVariableLine("var multiStateEnabled = false;")
        End If

    End Sub

    Public Sub LoadAgencyDropDown()
        Me.ddAgencies.Items.Clear()
        Try
            Dim showAlltext As String = "Show All"
            'qqHelper.LoadUserAgencyDropDown(Me.ddAgencies)
            'updated 3/8/2021 to use new method (which will determine whether we pull only active agencies or if we also include cancelled agencies that have inforce policies)
            qqHelper.LoadUserAgencyDropDown_AllOptions_Inverted(Me.ddAgencies)

            If IsStaff Then
                ' add 'show all' option
                ' ddAgencies.Items.Add(New ListItem(showAlltext, "null"))
                ' IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(Me.ddAgencies, showAlltext) ' make show all the default
                Dim acode As String = String.Empty
                Dim aid As String = String.Empty
                QuickQuoteHelperClass.SetFullAgencyCodeAndIdFor4DigitAgencyCode(Session("agencyCode"), acode, aid)
                Dim hasVal = False
                For Each item As ListItem In ddAgencies.Items
                    If item.Value.ToLower().Trim() = aid.ToLower().Trim() Then
                        hasVal = True
                        Exit For
                    End If
                Next
                If hasVal = True Then
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddAgencies, aid)
                Else
                    ' add 'show all' option
                    ddAgencies.Items.Add(New ListItem(showAlltext, "null"))
                    IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(Me.ddAgencies, showAlltext) ' make show all the default
                End If
            Else
                Me.ddAgencies.SelectedIndex = 0
                If Me.ddAgencies.Items.Count = 0 Then
                    MessageBoxVRPers.Show("This user has access to no agencies.", Response, ScriptManager.GetCurrent(Me.Page), Me)
                    Me.ctlPageStartupScript.AddScriptLine("DisableMainFormOnSaveRemoves();")
                End If
            End If

            If (String.IsNullOrWhiteSpace(Me.ddAgencies.SelectedValue) OrElse Me.ddAgencies.SelectedValue = "null") AndAlso String.IsNullOrWhiteSpace(Session("dd_agencies_selectedIndex")) = False Then
                If ddAgencies.SelectedIndex.Equals(Session("dd_agencies_selectedIndex")) = False Then
                    Me.ddAgencies.SelectedIndex = Session("dd_agencies_selectedIndex")
                End If
            End If

        Catch ex As Exception
            If IFM.VR.Web.Helpers.WebHelper_Personal.IsTesting Then
                MessageBoxVRPers.Show(ex.Message, Response, ScriptManager.GetCurrent(Me.Page), Me)
            Else
                MessageBoxVRPers.Show("Could not load agency drop down.", Response, ScriptManager.GetCurrent(Me.Page), Me)
                Me.ctlPageStartupScript.AddScriptLine("DisableMainFormOnSaveRemoves();")
            End If
        End Try
    End Sub

    Public Sub LoadUserNameDropDown()

        Try
            Me.ddAgent.Items.Clear()
            Me.ddAgent.Items.Add(New ListItem("", ""))

            Using sql As New SQLselectObject(connQQ)
                sql.queryOrStoredProc = "usp_SavedQuotes_GetAgencyAvailableAgents"

                Dim parms As New ArrayList()
                If IsStaff And AgencyID < 0 Then
                    parms.Add(New System.Data.SqlClient.SqlParameter("@AgencyID", Nothing))
                Else
                    parms.Add(New System.Data.SqlClient.SqlParameter("@AgencyID", AgencyID))
                End If
                sql.parameters = parms

                Using reader As System.Data.SqlClient.SqlDataReader = sql.GetDataReader()
                    If Not sql.hasError Then
                        If reader.HasRows Then
                            While reader.Read()
                                Dim isStaffUser As Boolean = CBool(reader.GetInt32(1))
                                Me.ddAgent.Items.Add(New ListItem(FormatAgentUsername(reader.GetString(0), isStaffUser), reader.GetString(0)))
                            End While
                        End If
                    End If
                End Using
            End Using

            IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(Me.ddAgent, Session("DiamondUsername"))
        Catch ex As Exception
            If IFM.VR.Web.Helpers.WebHelper_Personal.IsTesting() Then
                MessageBoxVRPers.Show(ex.Message, Response, ScriptManager.GetCurrent(Me.Page), Me)
            Else
                MessageBoxVRPers.Show("Could not load username drop down.", Response, ScriptManager.GetCurrent(Me.Page), Me)
            End If
        End Try

    End Sub

    Protected Sub ddAgencies_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddAgencies.SelectedIndexChanged
        Session("dd_agencies_selectedIndex") = Me.ddAgencies.SelectedIndex
        RaiseEvent AgencyIdChanged(If(Me.ddAgencies.SelectedValue <> "null", CInt(Me.ddAgencies.SelectedValue), -1))
    End Sub

    Public Sub AddQuoteStartedThisSession(quoteId As Int32)
        If Me.Session("quotesIdsCreatedThisSession") Is Nothing Then
            Me.Session("quotesIdsCreatedThisSession") = New List(Of Int32)
        End If
        DirectCast(Me.Session("quotesIdsCreatedThisSession"), List(Of Int32)).Add(quoteId)
    End Sub

    Public Function GetQuotesStartedThisSession() As List(Of Int32)
        If Me.Session("quotesIdsCreatedThisSession") Is Nothing Then
            Me.Session("quotesIdsCreatedThisSession") = New List(Of Int32)
        End If
        Return DirectCast(Me.Session("quotesIdsCreatedThisSession"), List(Of Int32))
    End Function
End Class

Public Class SessionQuoteHistoryItem
    Public QuoteNumber As String
    Public ClientName As String
    Public QuoteId As String
    Public LastUrl As String
    Public LastActivity As DateTime = DateTime.Now
    Public AgencyId As String
    Public AgencyCode As String
    Public Sub New(quotenum As String, clientName As String, quoteid As String, url As String, agencyId As String, agencyCode As String)
        Me.QuoteNumber = quotenum
        Me.ClientName = clientName
        Me.QuoteId = quoteid
        Me.LastUrl = url
        Me.AgencyId = agencyId
        Me.AgencyCode = agencyCode
    End Sub
End Class