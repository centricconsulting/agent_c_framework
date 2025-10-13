'5/22/2013 - for testing executable call
Imports System.Diagnostics
Imports System.IO
Imports System.Threading

'added 3/24/2017
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods

'5/29/2013 for SQL stuff
Imports System.Data.SqlClient

Partial Class DiamondQuoteProposal
    Inherits System.Web.UI.Page

    Dim qqHelper As New QuickQuoteHelperClass
    'Dim okayToRender As Boolean = True 'added 5/24/2013
    Dim okayToSavePage As Boolean = False 'added 5/24/2013
    Dim fileName As String = "" 'added 5/24/2013; don't include extension
    Dim quoteIds As List(Of String) 'added 5/29/2013 for database stuff
    Dim proposalHelper As New ProposalHelperClass 'added 6/10/2013

    'added 3/28/2017
    Dim diamondProposalId As Integer = 0
    'added 4/5/2017
    Dim qqDiaProposal As QuickQuote.CommonObjects.QuickQuoteDiamondProposal = Nothing
    'added 4/10/2017
    Dim strDiamondPolicyIdsAndPolicyImageNums As String = ""

    Protected Sub Page_PreInit(sender As Object, e As System.EventArgs) Handles Me.PreInit
        'If Request.QueryString("PrinterFriendlyQuoteIds") IsNot Nothing AndAlso UCase(Request.QueryString("PrinterFriendlyQuoteIds").ToString) <> "" Then
        'updated 3/28/2017
        'If (Request.QueryString("PrinterFriendlyQuoteIds") IsNot Nothing AndAlso UCase(Request.QueryString("PrinterFriendlyQuoteIds").ToString) <> "") OrElse (Request.QueryString("PrinterFriendlyDiamondProposalId") IsNot Nothing AndAlso qqHelper.IsPositiveIntegerString(Request.QueryString("PrinterFriendlyDiamondProposalId").ToString) = True) Then
        'updated 4/10/2017
        If (Request.QueryString("PrinterFriendlyQuoteIds") IsNot Nothing AndAlso UCase(Request.QueryString("PrinterFriendlyQuoteIds").ToString) <> "") OrElse (Request.QueryString("PrinterFriendlyDiamondProposalId") IsNot Nothing AndAlso qqHelper.IsPositiveIntegerString(Request.QueryString("PrinterFriendlyDiamondProposalId").ToString) = True) OrElse (Request.QueryString("PrinterFriendlyDiamondPolicyIdsAndPolicyImageNums") IsNot Nothing AndAlso Request.QueryString("PrinterFriendlyDiamondPolicyIdsAndPolicyImageNums").ToString <> "") Then
            Me.MasterPageFile = ConfigurationManager.AppSettings("DiamondQuickQuoteMasterPF")
        Else
            Me.MasterPageFile = ConfigurationManager.AppSettings("DiamondQuickQuoteMaster")
        End If
    End Sub
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Page.MaintainScrollPositionOnPostBack = True
        If Page.IsPostBack = False Then
            Page.Header.DataBind() '5/24/2013 - to get hidden field id for pdf open js
            LoadQuotes()

            'testing 02/21/2022
            'TestConvert()

        End If
    End Sub
    Private Sub LoadQuotes()
        Dim quoteIds As String = ""
        Dim errMsg As String = ""

        If Request.QueryString("QuoteIds") IsNot Nothing AndAlso Request.QueryString("QuoteIds").ToString <> "" Then
            quoteIds = Request.QueryString("QuoteIds").ToString
        ElseIf Request.QueryString("PrinterFriendlyQuoteIds") IsNot Nothing AndAlso Request.QueryString("PrinterFriendlyQuoteIds").ToString <> "" Then
            quoteIds = Request.QueryString("PrinterFriendlyQuoteIds").ToString
        Else
            'errMsg = "The parameters passed to this page were insufficient."
        End If

        'added 3/28/2017
        If Request.QueryString("DiamondProposalId") IsNot Nothing AndAlso qqHelper.IsPositiveIntegerString(Request.QueryString("DiamondProposalId").ToString) = True Then
            diamondProposalId = qqHelper.IntegerForString(Request.QueryString("DiamondProposalId").ToString)
        ElseIf Request.QueryString("PrinterFriendlyDiamondProposalId") IsNot Nothing AndAlso qqHelper.IsPositiveIntegerString(Request.QueryString("PrinterFriendlyDiamondProposalId").ToString) = True Then
            diamondProposalId = qqHelper.IntegerForString(Request.QueryString("PrinterFriendlyDiamondProposalId").ToString)
        End If

        'added 4/10/2017
        If Request.QueryString("DiamondPolicyIdsAndPolicyImageNums") IsNot Nothing AndAlso Request.QueryString("DiamondPolicyIdsAndPolicyImageNums").ToString <> "" Then
            strDiamondPolicyIdsAndPolicyImageNums = Request.QueryString("DiamondPolicyIdsAndPolicyImageNums").ToString
        ElseIf Request.QueryString("PrinterFriendlyDiamondPolicyIdsAndPolicyImageNums") IsNot Nothing AndAlso Request.QueryString("PrinterFriendlyDiamondPolicyIdsAndPolicyImageNums").ToString <> "" Then
            strDiamondPolicyIdsAndPolicyImageNums = Request.QueryString("PrinterFriendlyDiamondPolicyIdsAndPolicyImageNums").ToString
        End If

        If quoteIds <> "" Then

            'updated 5/30/2013 to 1st look for existing proposal in db
            If Request.QueryString("generatePDF") IsNot Nothing AndAlso Request.QueryString("generatePDF").ToString <> "" AndAlso (UCase(Request.QueryString("generatePDF").ToString) = "NO" OrElse UCase(Request.QueryString("generatePDF").ToString) = "FALSE") Then
                'don't try to generate PDF
            Else
                If ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_OpenPdfDirectly") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_OpenPdfDirectly").ToString <> "" AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_OpenPdfDirectly").ToString) = "YES" Then
                    If Request.QueryString("lookForExistingPDF") IsNot Nothing AndAlso Request.QueryString("lookForExistingPDF").ToString <> "" AndAlso (UCase(Request.QueryString("lookForExistingPDF").ToString) = "NO" OrElse UCase(Request.QueryString("lookForExistingPDF").ToString) = "FALSE") Then
                        'don't look for existing PDF
                    Else
                        Dim existingProposalId As String = GetExistingProposalId(quoteIds)
                        If existingProposalId <> "" Then
                            Response.Redirect("DiamondQuoteProposalLoader.aspx?proposalId=" & existingProposalId)
                        End If
                    End If
                End If
            End If

            'fileName = "Proposal_" & Replace(quoteIds, "|", ",") & "_" & Replace(Replace(Date.Now.ToString, "/", ""), ":", "") 'added 5/24/2013; replace / and : in timestamp and | in quoteIds
            Using qqProposal As New QuickQuoteProposalObject
                With qqProposal
                    If quoteIds.Contains("|") = True Then
                        Dim qIds As String()
                        qIds = Split(quoteIds, "|")
                        For Each qId As String In qIds
                            If IsNumeric(qId) = True Then
                                .QuoteIds.Add(qId)
                            End If
                        Next
                    Else
                        If IsNumeric(quoteIds) = True Then
                            .QuoteIds.Add(quoteIds)
                        End If
                    End If
                    If .QuoteIds.Count > 0 Then
                        .LoadQuickQuoteObjects()
                        If .ValidQuoteIds.Count > 0 AndAlso .QuickQuoteObjects.Count > 0 Then
                            LoadProposal(qqProposal, errMsg)
                        Else
                            If .ErrorMessages.Count = 0 Then
                                errMsg = "The parameters passed to this page were insufficient."
                            End If
                        End If
                        If .ErrorMessages.Count > 0 Then
                            For Each eMsg As String In .ErrorMessages
                                errMsg = qqHelper.appendText(errMsg, eMsg, "<br>", True) 'True is to append error prefix w/ its own break
                            Next
                        End If
                    Else
                        errMsg = "The parameters passed to this page were insufficient."
                    End If
                End With
            End Using
        ElseIf diamondProposalId > 0 Then 'added 4/5/2017
            Dim loadErrorMessage As String = ""
            'Dim qqDiaProposal As QuickQuote.CommonObjects.QuickQuoteDiamondProposal = qqHelper.DiamondProposalForLookup(diamondProposalId:=diamondProposalId, errorMessage:=loadErrorMessage)
            'updated to use page-level variable so we can access from other routines
            qqDiaProposal = qqHelper.DiamondProposalForLookup(diamondProposalId:=diamondProposalId, errorMessage:=loadErrorMessage)

            If qqDiaProposal IsNot Nothing Then
                If qqDiaProposal.DiamondProposalBinaryId > 0 Then 'could also check to verify that bytes isnot nothing
                    'take user directly to proposal
                    Response.Redirect("DiamondQuoteProposalLoader.aspx?diamondProposalBinaryId=" & qqDiaProposal.DiamondProposalBinaryId.ToString)
                ElseIf qqDiaProposal.Images IsNot Nothing AndAlso qqDiaProposal.Images.Count > 0 Then
                    Using qqProposal As New QuickQuoteProposalObject
                        With qqProposal
                            'For Each img As QuickQuoteDiamondProposalImage In qqDiaProposal.Images
                            '    If img IsNot Nothing Then
                            '        QuickQuoteHelperClass.AddPolicyInfoToLookupInfoList(.PolicyRecords, policyId:=img.PolicyId, policyImageNum:=img.PolicyImageNum)
                            '    End If
                            'Next
                            'If .PolicyRecords IsNot Nothing AndAlso .PolicyRecords.Count > 0 Then
                            '    .LoadQuickQuoteObjects()
                            '    If .ValidPolicyRecords IsNot Nothing AndAlso .ValidPolicyRecords.Count > 0 AndAlso .QuickQuoteObjects IsNot Nothing AndAlso .QuickQuoteObjects.Count > 0 Then
                            '        qqDiaProposal.TotalQuotedPremium = .CombinedPremium 'added 4/22/2017
                            '        LoadProposal(qqProposal, errMsg)
                            '    Else
                            '        If .ErrorMessages.Count = 0 Then
                            '            errMsg = "The parameters passed to this page were insufficient."
                            '        End If
                            '    End If
                            '    If .ErrorMessages.Count > 0 Then
                            '        For Each eMsg As String In .ErrorMessages
                            '            errMsg = qqHelper.appendText(errMsg, eMsg, "<br>", True) 'True is to append error prefix w/ its own break
                            '        Next
                            '    End If
                            'Else
                            '    errMsg = "The parameters passed to this page were insufficient."
                            'End If
                            'updated 5/1/2017 to pass in Diamond Proposal so comments can be loaded
                            '.DiamondProposal = qqDiaProposal
                            'updated 5/3/2017 to use clone so it wouldn't get cleared out when qqProposal is disposed
                            .DiamondProposal = qqHelper.CloneObject(qqDiaProposal)
                            .LoadQuickQuoteObjects()
                            If .ValidPolicyRecords IsNot Nothing AndAlso .ValidPolicyRecords.Count > 0 AndAlso .QuickQuoteObjects IsNot Nothing AndAlso .QuickQuoteObjects.Count > 0 Then
                                qqDiaProposal.TotalQuotedPremium = .CombinedPremium 'added 4/22/2017
                                LoadProposal(qqProposal, errMsg)
                            Else
                                If .ErrorMessages.Count = 0 Then
                                    errMsg = "The parameters passed to this page were insufficient."
                                End If
                            End If
                            If .ErrorMessages.Count > 0 Then
                                For Each eMsg As String In .ErrorMessages
                                    errMsg = qqHelper.appendText(errMsg, eMsg, "<br>", True) 'True is to append error prefix w/ its own break
                                Next
                            End If
                        End With
                    End Using
                Else
                    errMsg = "There was a problem loading your proposal."
                End If
            Else
                errMsg = "There was a problem loading your proposal."
            End If
        ElseIf String.IsNullOrWhiteSpace(strDiamondPolicyIdsAndPolicyImageNums) = False Then 'added 4/10/2017
            Dim policyIdAndImageNumList As List(Of QuickQuoteGenericObjectWithTwoIntegerProperties) = QuickQuoteHelperClass.ListOfGenericObjectWith2IntegerPropsFromString(strDiamondPolicyIdsAndPolicyImageNums, objectdelimiter:="||", propDelimiter:="**", integerPairRequirement:=QuickQuoteHelperClass.IntegerPairType.BothPositive)
            If policyIdAndImageNumList IsNot Nothing AndAlso policyIdAndImageNumList.Count > 0 Then
                Using qqProposal As New QuickQuoteProposalObject
                    With qqProposal
                        For Each polIdAndImageNum As QuickQuoteGenericObjectWithTwoIntegerProperties In policyIdAndImageNumList
                            If polIdAndImageNum IsNot Nothing AndAlso polIdAndImageNum.Property1 > 0 AndAlso polIdAndImageNum.Property2 > 0 Then
                                QuickQuoteHelperClass.AddPolicyInfoToLookupInfoList(.PolicyRecords, policyId:=polIdAndImageNum.Property1, policyImageNum:=polIdAndImageNum.Property2)
                            End If
                        Next
                        If .PolicyRecords IsNot Nothing AndAlso .PolicyRecords.Count > 0 Then
                            .LoadQuickQuoteObjects()
                            If .ValidPolicyRecords IsNot Nothing AndAlso .ValidPolicyRecords.Count > 0 AndAlso .QuickQuoteObjects IsNot Nothing AndAlso .QuickQuoteObjects.Count > 0 Then
                                'now load up QuickQuoteDiamondProposal object and Save
                                For Each polRecord As QuickQuotePolicyLookupInfo In .ValidPolicyRecords
                                    qqHelper.UpdateDiamondProposalImageInfo(qqDiaProposal, polRecord.PolicyId, polRecord.PolicyImageNum)
                                Next
                                If qqDiaProposal IsNot Nothing Then
                                    Dim userId As Integer = qqHelper.IntegerForString(QuickQuoteHelperClass.DiamondUserId())
                                    qqDiaProposal.InsertUserId = userId
                                    qqDiaProposal.TotalQuotedPremium = .CombinedPremium 'added 4/22/2017; note: this won't get saved until Binary record is saved
                                    qqHelper.SaveDiamondProposal(qqDiaProposal)
                                    If qqDiaProposal IsNot Nothing AndAlso qqDiaProposal.DiamondProposalId > 0 Then
                                        diamondProposalId = qqDiaProposal.DiamondProposalId
                                    End If
                                End If
                                LoadProposal(qqProposal, errMsg)
                            Else
                                If .ErrorMessages.Count = 0 Then
                                    errMsg = "The parameters passed to this page were insufficient."
                                End If
                            End If
                            If .ErrorMessages.Count > 0 Then
                                For Each eMsg As String In .ErrorMessages
                                    errMsg = qqHelper.appendText(errMsg, eMsg, "<br>", True) 'True is to append error prefix w/ its own break
                                Next
                            End If
                        Else
                            errMsg = "The parameters passed to this page were insufficient."
                        End If
                    End With
                End Using
            Else
                errMsg = "The parameters passed to this page were insufficient."
            End If
        Else
            errMsg = "The parameters passed to this page were insufficient."
        End If

        If errMsg <> "" Then
            ShowError(errMsg)
        End If
    End Sub
    Private Sub LoadProposal(ByVal qqProposal As QuickQuoteProposalObject, ByRef errMsg As String)
        If qqProposal IsNot Nothing AndAlso qqProposal.QuickQuoteObjects.Count > 0 Then
            With qqProposal
                'AddPageBreakToPlaceholder() 'testing to see if logo shows in same place (per title page vs summary page) after page break
                Dim AboutUsPage As controls_Proposal_VRProposal_AboutUs = LoadControl("controls/Proposal/VRProposal_AboutUs.ascx")
                Dim PaymentOptionsGeneric As controls_Proposal_VRProposal_NewPayment_Page = LoadControl("controls/Proposal/VRProposal_NewPayment_Page.ascx")
                Dim TerrorismPage As controls_Proposal_VRProposal_Terrorism_notice = LoadControl("controls/Proposal/VRProposal_Terrorism_Notice.ascx")

                '5/6/2013 error: Unable to cast object of type 'ASP.controls_proposal_vrproposal_titlepage_ascx' to type 'controls_Proposal_VRProposal_TitlePage'. (need to clear temporary Internet files... or build interface the each controls implements)
                Dim TitlePage As controls_Proposal_VRProposal_TitlePage = LoadControl("controls/Proposal/VRProposal_TitlePage.ascx")

                TitlePage.ClientInfo = qqHelper.appendText(TitlePage.ClientInfo, "<div class=""proposalTitlePageName"">" & .Client.Name.DisplayNameForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />") & "</div>", "")
                TitlePage.ClientInfo = qqHelper.appendText(TitlePage.ClientInfo, "<div class=""proposalTitlePageAddress"">" & .Client.Address.DisplayAddressForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />") & "</div>", "")
                TitlePage.ClientInfo = qqHelper.appendText(TitlePage.ClientInfo, "<div class=""proposalTitlePagePhoneNumber"">" & .Client.PrimaryPhone & "</div>", "")

                'TitlePage.ClientInfo = qqHelper.appendText(.Client.Name.DisplayNameForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), .Client.Address.DisplayAddressForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), "<br />")
                'TitlePage.ClientInfo = qqHelper.appendText(TitlePage.ClientInfo, .Client.PrimaryPhone, "<br />")

                'TitlePage.AgencyInfo = qqHelper.appendText(.Agency.Name.DisplayNameForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), .Agency.Address.DisplayAddressForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), "<br />")
                'updated 6/10/2013 to use DBA name or IRS name if DBA isn't available (instead of using both)

                TitlePage.AgencyInfo = qqHelper.appendText(TitlePage.AgencyInfo, "<div class=""proposalTitlePageName"">" & proposalHelper.GetAgencyName(.Agency.Name).Replace(vbCrLf, "<br />").Replace(vbLf, "<br />") & "</div>", "")
                TitlePage.AgencyInfo = qqHelper.appendText(TitlePage.AgencyInfo, "<div class=""proposalTitlePageAddress"">" & .Agency.Address.DisplayAddressForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />") & "</div>", "")
                TitlePage.AgencyInfo = qqHelper.appendText(TitlePage.AgencyInfo, "<div class=""proposalTitlePagePhoneNumber"">" & .Agency.PrimaryPhone & "</div>", "")

                'TitlePage.AgencyInfo = qqHelper.appendText(proposalHelper.GetAgencyName(.Agency.Name).Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), .Agency.Address.DisplayAddressForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), "<br />")
                'TitlePage.AgencyInfo = qqHelper.appendText(TitlePage.AgencyInfo, .Agency.PrimaryPhone, "<br />")


                TitlePage.DateText = FormatDateTime(Date.Today, DateFormat.ShortDate)

                'updated 5/8/2013
                If .LogoWebPath <> "" AndAlso .LogoExists = True Then
                    TitlePage.Logo = "<img src=""" & .LogoWebPath & """ class=""proposalTitlePageAgencyLogo"" alt=""Agency Logo""/>"
                    'PaymentOptionsGeneric.Logo = "<img src=""" & .LogoWebPath & """ class=""QuickQuoteProposalAgencyLogoSection"" />"
                    TerrorismPage.Logo = "<img src=""" & .LogoWebPath & """ class=""QuickQuoteProposalAgencyLogoSection"" />"

                    'Else
                    '    'TitlePage.Logo = "<img src=""https://www.indianafarmers.com/agentsonly/images/bg/IFMlogo2.jpg"" />"
                    '    'updated 5/9/2013
                    '    Dim missingAgencyLogoText As String = "<table align=""center"" class=""QuickQuoteProposalAgencyLogoSection"">"
                    '    missingAgencyLogoText &= "<tr><td align=""center"">"
                    '    missingAgencyLogoText &= "<span>Missing Logo for Agency Code " & .Agency.Code & "</span>" '5/10/2013 - updated w/ span (for CSS)
                    '    missingAgencyLogoText &= "</td></tr></table>"
                    '    TitlePage.Logo = missingAgencyLogoText
                End If
                Me.phControls.Controls.Add(TitlePage)
                'AddTestFooter() 'testing 5/13/2013
                AddPageBreakToPlaceholder()

                'Me.phControls.Controls.Add(PaymentPage)
                'AddPageBreakToPlaceholder()

                'Me.phControls.Controls.Add(AboutUsPage)
                'AddPageBreakToPlaceholder()

                'Me.phControls.Controls.Add(TerrorismPage)
                'AddPageBreakToPlaceholder()


                'Dim hasValidQuickQuoteObject As Boolean = False 'might use before showing anything
                'Dim ClientAndAgencyInfo As VRProposal_ClientAndAgencyInfo = LoadControl("controls/Proposal/VRProposal_ClientAndAgencyInfo.ascx")
                'ClientAndAgencyInfo.ClientInfo = qqHelper.appendText(.Client.Name.DisplayNameForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), .Client.Address.DisplayAddressForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), "<br />")
                'ClientAndAgencyInfo.ClientInfo = qqHelper.appendText(ClientAndAgencyInfo.ClientInfo, .Client.PrimaryPhone, "<br />")
                'ClientAndAgencyInfo.AgencyInfo = qqHelper.appendText(.Agency.Name.DisplayNameForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), .Agency.Address.DisplayAddressForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), "<br />")
                'ClientAndAgencyInfo.AgencyInfo = qqHelper.appendText(ClientAndAgencyInfo.AgencyInfo, .Agency.PrimaryPhone, "<br />")
                'Me.phControls.Controls.Add(ClientAndAgencyInfo)
                'LoadClientAndAgencyInfoControl(qqProposal)

                'AddFooterOpenDiv() 'testing 5/13/2013

                'Dim summaryInfo As New Label
                'summaryInfo.Text = "<br /><b>Summary Info</b>"
                'summaryInfo.Text &= "<br />Total Premium:  " & .CombinedPremium
                'Me.phControls.Controls.Add(summaryInfo)
                Dim Summary As controls_Proposal_VRProposal_Summary = LoadControl("controls/Proposal/VRProposal_Summary.ascx")
                Summary.Proposal = qqProposal
                Me.phControls.Controls.Add(Summary)

                'AddFooterCloseDiv() 'testing 5/13/2013

                If proposalHelper.OkayToSkipProposalLobDetailsForTesting = False Then 'added IF 7/11/2017 for local testing
                    For Each qq As QuickQuoteObject In .QuickQuoteObjects
                        'Select Case qq.LobType
                        '    Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                        '        AddPageBreakToPlaceholder()

                        '        '5/8/2013 - add Martin's control
                        '        If ConfigurationManager.AppSettings("QuickQuote_BOP_ProposalControlPath") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_BOP_ProposalControlPath").ToString <> "" Then
                        '            Dim BOP As uc_VRProposal_BOP = LoadControl(ConfigurationManager.AppSettings("QuickQuote_BOP_ProposalControlPath").ToString)
                        '            BOP.QO = qq
                        '            Me.phControls.Controls.Add(BOP)
                        '        Else
                        '            Dim l As New Label
                        '            l.Text = "<br /><b>BOP Quote:  </b>" & qq.PolicyNumber 'updated 5/1/2017 to use PolicyNumber instead of QuoteNumber
                        '            l.Text &= "<br />Premium:  " & qq.TotalQuotedPremium
                        '            Me.phControls.Controls.Add(l)
                        '        End If

                        '        'AddTestFooter() 'testing 5/13/2013
                        '    Case QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
                        '        AddPageBreakToPlaceholder()

                        '        If ConfigurationManager.AppSettings("QuickQuote_CGL_ProposalControlPath") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_CGL_ProposalControlPath").ToString <> "" Then
                        '            Dim CGL As Controls_Proposal_CGL_Velocirater_Proposal_CGL_Control = LoadControl(ConfigurationManager.AppSettings("QuickQuote_CGL_ProposalControlPath").ToString)
                        '            CGL.QO = qq
                        '            'CGL.Populate() 'won't need w/ Matt's next build; no longer needed as-of 5/9/2013
                        '            Me.phControls.Controls.Add(CGL)
                        '        Else
                        '            Dim l As New Label
                        '            l.Text = "<br /><b>CGL Quote:  </b>" & qq.PolicyNumber 'updated 5/1/2017 to use PolicyNumber instead of QuoteNumber
                        '            l.Text &= "<br />Premium:  " & qq.TotalQuotedPremium
                        '            Me.phControls.Controls.Add(l)
                        '        End If

                        '        'AddTestFooter() 'testing 5/13/2013
                        '    Case QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                        '        AddPageBreakToPlaceholder()

                        '        If ConfigurationManager.AppSettings("QuickQuote_WCP_ProposalControlPath") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_WCP_ProposalControlPath").ToString <> "" Then
                        '            Dim WCP As uc_VRProposal_WCP = LoadControl(ConfigurationManager.AppSettings("QuickQuote_WCP_ProposalControlPath").ToString)
                        '            WCP.QO = qq
                        '            Me.phControls.Controls.Add(WCP)
                        '        Else
                        '            Dim l As New Label
                        '            l.Text = "<br /><b>WCP Quote:  </b>" & qq.PolicyNumber 'updated 5/1/2017 to use PolicyNumber instead of QuoteNumber
                        '            'l.Text &= "<br />Premium:  " & qq.TotalQuotedPremium
                        '            'updated 5/6/2013 to use different property for WCP premium
                        '            l.Text &= "<br />Premium:  " & qq.Dec_WC_TotalPremiumDue
                        '            Me.phControls.Controls.Add(l)
                        '        End If

                        '        'AddTestFooter() 'testing 5/13/2013
                        '    Case QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                        '        AddPageBreakToPlaceholder()

                        '        '5/8/2013 - add Matt's control
                        '        If ConfigurationManager.AppSettings("QuickQuote_CPR_ProposalControlPath") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_CPR_ProposalControlPath").ToString <> "" Then
                        '            Dim CPR As Controls_Proposal_CPR_Velocirater_Proposal_CPR_Control = LoadControl(ConfigurationManager.AppSettings("QuickQuote_CPR_ProposalControlPath").ToString)
                        '            CPR.QO = qq
                        '            Me.phControls.Controls.Add(CPR)
                        '        Else
                        '            Dim l As New Label
                        '            l.Text = "<br /><b>CPR Quote:  </b>" & qq.PolicyNumber 'updated 5/1/2017 to use PolicyNumber instead of QuoteNumber
                        '            l.Text &= "<br />Premium:  " & qq.TotalQuotedPremium
                        '            Me.phControls.Controls.Add(l)
                        '        End If

                        '        'AddTestFooter() 'testing 5/13/2013
                        '    Case QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                        '        AddPageBreakToPlaceholder()

                        '        '5/8/2013 - add Matt's control
                        '        If ConfigurationManager.AppSettings("QuickQuote_CPP_ProposalControlPath") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_CPP_ProposalControlPath").ToString <> "" Then
                        '            Dim CPP As Controls_Proposal_CPP_Velocirater_Proposal_CPP_Control = LoadControl(ConfigurationManager.AppSettings("QuickQuote_CPP_ProposalControlPath").ToString)
                        '            CPP.QO = qq
                        '            Me.phControls.Controls.Add(CPP)
                        '        Else
                        '            Dim l As New Label
                        '            l.Text = "<br /><b>CPP Quote:  </b>" & qq.PolicyNumber 'updated 5/1/2017 to use PolicyNumber instead of QuoteNumber
                        '            l.Text &= "<br />Premium:  " & qq.TotalQuotedPremium
                        '            Me.phControls.Controls.Add(l)
                        '        End If

                        '        'AddTestFooter() 'testing 5/13/2013
                        '    Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                        '        AddPageBreakToPlaceholder()

                        '        If ConfigurationManager.AppSettings("QuickQuote_CAP_ProposalControlPath") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_CAP_ProposalControlPath").ToString <> "" Then
                        '            Dim CAP As uc_VRProposal_CAP = LoadControl(ConfigurationManager.AppSettings("QuickQuote_CAP_ProposalControlPath").ToString)
                        '            CAP.QO = qq
                        '            Me.phControls.Controls.Add(CAP)
                        '        Else
                        '            Dim l As New Label
                        '            l.Text = "<br /><b>CAP Quote:  </b>" & qq.PolicyNumber 'updated 5/1/2017 to use PolicyNumber instead of QuoteNumber
                        '            l.Text &= "<br />Premium:  " & qq.TotalQuotedPremium
                        '            Me.phControls.Controls.Add(l)
                        '        End If

                        '        'AddTestFooter() 'testing 5/13/2013
                        '    Case QuickQuoteObject.QuickQuoteLobType.CommercialGarage 'added 5/13/2017
                        '        'AddPageBreakToPlaceholder()

                        '        'If ConfigurationManager.AppSettings("QuickQuote_GAR_ProposalControlPath") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_GAR_ProposalControlPath").ToString <> "" Then
                        '        '    Dim GAR As Controls_Proposal_GAR_Velocirater_Proposal_GAR_Control = LoadControl(ConfigurationManager.AppSettings("QuickQuote_GAR_ProposalControlPath").ToString)
                        '        '    GAR.QO = qq
                        '        '    Me.phControls.Controls.Add(GAR)
                        '        'Else
                        '        '    Dim l As New Label
                        '        '    l.Text = "<br /><b>GAR Quote:  </b>" & qq.PolicyNumber 'use PolicyNumber instead of QuoteNumber
                        '        '    l.Text &= "<br />Premium:  " & qq.TotalQuotedPremium
                        '        '    Me.phControls.Controls.Add(l)
                        '        'End If
                        '    Case Else
                        '        'nothing available for this lob
                        '        'errMsg = qqHelper.appendText(errMsg, "Nothing is currently available for quote number " & qq.QuoteNumber & ".", "<br>", True) 'True is to append error prefix w/ its own break
                        '        'removed errMsg 5/1/2017 since every other LOB should now show up on Summary page w/ LOB_Generic control
                        'End Select
                        'updated 5/13/2017 to use new centralized method that can be used to check for LOB control too (method will be used from summary too)
                        Dim hasLobControl As Boolean = False
                        proposalHelper.CheckForProposalLobControlAndAddToPlaceholder(qq, hasLobControl:=hasLobControl, addToPlaceholder:=True, webPage:=Me.Page, ph:=Me.phControls)
                        If hasLobControl = False Then
                            'no LOB control available; could show message or something, but Summary should now handle

                        End If
                    Next
                End If

                'moved Payment Options page 5/2/2013; was previously above QQ loop that gets LOB controls
                AddPageBreakToPlaceholder()

                'AddFooterOpenDiv() 'testing 5/13/2013

                'Dim ClientAndAgencyInfo2 As VRProposal_ClientAndAgencyInfo = LoadControl("controls/Proposal/VRProposal_ClientAndAgencyInfo.ascx")
                'ClientAndAgencyInfo2.ClientInfo = qqHelper.appendText(.Client.Name.DisplayNameForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), .Client.Address.DisplayAddressForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), "<br />")
                'ClientAndAgencyInfo2.AgencyInfo = qqHelper.appendText(.Agency.Name.DisplayNameForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), .Agency.Address.DisplayAddressForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), "<br />")
                'Me.phControls.Controls.Add(ClientAndAgencyInfo2)

                'added 9/16/2017
                Dim stillNeedsToLoadPaymentOptions As Boolean = True
                If proposalHelper.TryActualPaymentOptionsFirst = True Then
                    Dim PaymentOptionsActual As controls_Proposal_VRProposal_PaymentOptions_Actual = LoadControl("controls/Proposal/VRProposal_PaymentOptions_Actual.ascx")
                    PaymentOptionsActual.PaymentOptions = .CombinedPaymentOptions
                    If PaymentOptionsActual.HasVisiblePaymentOptions = True Then
                        LoadClientAndAgencyInfoControl(qqProposal)
                        Me.phControls.Controls.Add(PaymentOptionsActual)
                        stillNeedsToLoadPaymentOptions = False
                    End If
                End If

                If stillNeedsToLoadPaymentOptions = True Then 'added IF 9/16/2017; original logic inside
                    If proposalHelper.ShowGenericPaymentOptions = True Then 'added IF 7/12/2017; original logic in ELSE
                        'AddPageBreakToPlaceholder()
                        If proposalHelper.ShowClientAndAgencyInfoForGenericPaymentOptions = True Then
                            LoadClientAndAgencyInfoControl(qqProposal)
                        End If
                        Me.phControls.Controls.Add(PaymentOptionsGeneric)
                        PaymentOptionsGeneric.AgencyName = proposalHelper.GetAgencyName(.Agency.Name).Replace(vbCrLf, ",").Replace(vbLf, ",")
                    Else
                        LoadClientAndAgencyInfoControl(qqProposal)

                        Dim PaymentOptions As controls_Proposal_VRProposal_PaymentOptions = LoadControl("controls/Proposal/VRProposal_PaymentOptions.ascx")
                        PaymentOptions.AnnualPaymentOption = .AnnualPaymentOption
                        PaymentOptions.SemiAnnualPaymentOption = .SemiAnnualPaymentOption
                        PaymentOptions.QuarterlyPaymentOption = .QuarterlyPaymentOption
                        PaymentOptions.MonthlyPaymentOption = .MonthlyPaymentOption
                        PaymentOptions.EftMonthlyPaymentOption = .EftMonthlyPaymentOption
                        PaymentOptions.CreditCardMonthlyPaymentOption = .CreditCardMonthlyPaymentOption
                        PaymentOptions.RenewalCreditCardMonthlyPaymentOption = .RenewalCreditCardMonthlyPaymentOption
                        PaymentOptions.RenewalEftMonthlyPaymentOption = .RenewalEftMonthlyPaymentOption
                        PaymentOptions.AnnualMtgPaymentOption = .AnnualMtgPaymentOption
                        PaymentOptions.CalculateBasicPaymentOptions(.CombinedPremium, .QuickQuoteObjects.Count) 'added 5/23/2013 for basic payment options (could also use .ValidQuoteIds.Count for 2nd param); 7/8/2017 note: .QuickQuoteObjects.Count is now only valid count for this since .ValidQuoteIds would only apply to normal VR Proposals (.ValidPolicyRecords.Count would be the equivalent for Diamond Proposals)
                        Me.phControls.Controls.Add(PaymentOptions)
                    End If
                End If

                'AddFooterCloseDiv() 'testing 5/13/2013

                'added 5/24/2013
                If .ValidQuoteIds IsNot Nothing AndAlso .ValidQuoteIds.Count > 0 Then
                    Dim strQuoteIds As String = ""
                    quoteIds = New List(Of String) 'added 5/29/2013
                    For Each qId As String In .ValidQuoteIds
                        strQuoteIds = qqHelper.appendText(strQuoteIds, qId, ",")
                        quoteIds.Add(qId) 'added 5/29/2013
                    Next
                    If strQuoteIds <> "" Then
                        fileName = "Proposal_" & strQuoteIds & "_" & Replace(Replace(Date.Now.ToString, "/", ""), ":", "") 'added 5/24/2013; replace / and : in timestamp
                    End If
                ElseIf diamondProposalId > 0 AndAlso .ValidPolicyRecords IsNot Nothing AndAlso .ValidPolicyRecords.Count > 0 Then 'added 4/5/2017
                    fileName = "Proposal_" & diamondProposalId.ToString & "_" & Replace(Replace(Date.Now.ToString, "/", ""), ":", "") 'added 5/24/2013; replace / and : in timestamp
                End If
                AddPageBreakToPlaceholder()

                'Me.phControls.Controls.Add(AboutUsPage)
                'AddPageBreakToPlaceholder()

                Me.phControls.Controls.Add(TerrorismPage)
            End With
            '5/22/2013 - for iTextSharp testing
            'GeneratePdf()
            '5/22/2013 - more testing; added pdf=yes querystring param to prevent infinite loop whenever executable calls page
            'If Request.QueryString("pdf") Is Nothing OrElse Request.QueryString("pdf").ToString = "" OrElse UCase(Request.QueryString("pdf").ToString) <> "YES" Then
            '    'TestHtmlToPdf()
            '    'Kill_wkhtmltopdf()

            '    '5/30/2013 - moved logic here from test Subs
            '    okayToSavePage = True
            '    OpenNewWindow(Replace(Request.Url.AbsolutePath, "DiamondQuoteProposal.aspx", "") & "Proposals/" & fileName & ".pdf", True) 'added param to signify it's to open a pdf
            'End If
            '5/30/2013 - updated to use a more suitable querystring param
            If Request.QueryString("generatePDF") IsNot Nothing AndAlso Request.QueryString("generatePDF").ToString <> "" AndAlso (UCase(Request.QueryString("generatePDF").ToString) = "NO" OrElse UCase(Request.QueryString("generatePDF").ToString) = "FALSE") Then
                'don't try to generate PDF
            Else
                okayToSavePage = True
                OpenNewWindow(Replace(Request.Url.AbsolutePath, "DiamondQuoteProposal.aspx", "") & "Proposals/" & fileName & ".pdf", True) 'added param to signify it's to open a pdf
            End If
        End If
    End Sub
    'Private Sub AddPageBreakToPlaceholder()
    '    Dim pageBreak As New Label
    '    'pageBreak.Text = "<br />--------------Page Break--------------"
    '    'updated 5/3/2013
    '    pageBreak.Text = "<div class=""page-break""></div>"
    '    pageBreak.Text &= "<div class=""page-break-section""></div>"
    '    Me.phControls.Controls.Add(pageBreak)
    'End Sub
    'updated 5/24/2013 since label is wrapping div inside span
    Private Sub AddPageBreakToPlaceholder()
        'Dim pageBreak As New HtmlGenericControl
        'pageBreak.InnerHtml = "<div class=""page-break""></div>"
        'pageBreak.InnerHtml &= "<div class=""page-break-section""></div>"
        'Me.phControls.Controls.Add(pageBreak)

        'using panel since HtmlGenericControl acts like span
        'Dim pageBreak As New Panel
        'pageBreak.CssClass = "page-break"
        'Dim pageBreakSection As New Panel
        'pageBreakSection.CssClass = "page-break-section"
        ''pageBreak.Controls.Add(pageBreakSection)'would only do this if other div needs to wrap around it
        'Me.phControls.Controls.Add(pageBreak)
        'Me.phControls.Controls.Add(pageBreakSection)
        'updated 5/13/2017 to use common method
        proposalHelper.AddPageBreakToPlaceholder(Me.phControls)
    End Sub
    Private Sub AddTestFooter()
        'Dim footer As New Label
        'footer.Text = "<div class=""DisclaimerHolder""><div class=""QuickQuoteProposalDisclaimer"">test footer</div></div>"
        'Me.phControls.Controls.Add(footer)
    End Sub
    Private Sub AddFooterOpenDiv()
        'Dim footerOpen As New Label
        'footerOpen.Text = "<div class=""FooterPage"">"
        'Me.phControls.Controls.Add(footerOpen)
    End Sub
    Private Sub AddFooterCloseDiv()
        'Dim footerClose As New Label
        'footerClose.Text = "</div>"
        'Me.phControls.Controls.Add(footerClose)
    End Sub
    Private Sub ShowError(ByVal message As String, Optional ByVal redirect As Boolean = False, Optional ByVal redirectPage As String = "")
        message = Replace(message, "\", "\\")
        message = Replace(message, "<br>", "\n")
        message = Replace(message, vbCrLf, "\n")

        Dim strScript As String = "<script language=JavaScript>"
        strScript &= "alert(""" & message & """);"
        If redirect = True AndAlso redirectPage <> "" Then
            strScript &= " window.location.href='" & redirectPage & "';"
        End If
        strScript &= "</script>"

        'Page.RegisterStartupScript("clientScript", strScript)
        Page.RegisterStartupScript("messageClientScript", strScript) 'updated 5/24/2013 to differentiate clientScripts so multiple can execute

    End Sub
    Private Sub LoadClientAndAgencyInfoControl(ByVal qqProposal As QuickQuoteProposalObject)
        If qqProposal IsNot Nothing Then
            With qqProposal
                Dim ClientAndAgencyInfo As controls_Proposal_VRProposal_ClientAndAgencyInfo = LoadControl("controls/Proposal/VRProposal_ClientAndAgencyInfo.ascx")
                ClientAndAgencyInfo.ClientInfo = qqHelper.appendText(.Client.Name.DisplayNameForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), .Client.Address.DisplayAddressForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), "<br />")
                ClientAndAgencyInfo.ClientInfo = qqHelper.appendText(ClientAndAgencyInfo.ClientInfo, .Client.PrimaryPhone, "<br />")
                'ClientAndAgencyInfo.AgencyInfo = qqHelper.appendText(.Agency.Name.DisplayNameForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), .Agency.Address.DisplayAddressForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), "<br />")
                'updated 6/10/2013 to use DBA name or IRS name if DBA isn't available (instead of using both)
                ClientAndAgencyInfo.AgencyInfo = qqHelper.appendText(proposalHelper.GetAgencyName(.Agency.Name).Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), .Agency.Address.DisplayAddressForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), "<br />")
                ClientAndAgencyInfo.AgencyInfo = qqHelper.appendText(ClientAndAgencyInfo.AgencyInfo, .Agency.PrimaryPhone, "<br />")
                Me.phControls.Controls.Add(ClientAndAgencyInfo)
            End With
        End If
    End Sub

    '5/22/2013 - for iTextSharp testing
    'Private Sub GeneratePdf()
    '    Response.ContentType = "application/pdf"
    '    Response.AddHeader("content-disposition", "attachment;filename=TestPage.pdf")
    '    Response.Cache.SetCacheability(HttpCacheability.NoCache)
    '    Dim sw As New StringWriter()
    '    Dim hw As New HtmlTextWriter(sw)
    '    Me.Page.RenderControl(hw)
    '    Dim sr As New StringReader(sw.ToString())
    '    Dim pdfDoc As New Document(PageSize.A4, 10.0F, 10.0F, 100.0F, 0.0F)
    '    Dim htmlparser As New HTMLWorker(pdfDoc)
    '    PdfWriter.GetInstance(pdfDoc, Response.OutputStream)
    '    pdfDoc.Open()
    '    htmlparser.Parse(sr)
    '    pdfDoc.Close()
    '    Response.Write(pdfDoc)
    '    Response.[End]()
    'End Sub

    Private Sub TestHtmlToPdf()
        Dim status As String = ""
        'RunExecutable("C:\Program Files (x86)\wkhtmltopdf\wkhtmltopdf.exe", "http://www.indianafarmers.com/NewPublicSite/NewPublicHome.aspx C:\Program Files (x86)\wkhtmltopdf\conversions\TestHomePage.pdf", status)
        'RunExecutable("C:\Program Files (x86)\wkhtmltopdf\wkhtmltopdf.exe", """http://www.indianafarmers.com/NewPublicSite/NewPublicHome.aspx"" ""C:\Program Files (x86)\wkhtmltopdf\conversions\TestHomePage.pdf""", status)'must not have had access to write to path
        'RunExecutable("C:\Program Files (x86)\wkhtmltopdf\wkhtmltopdf.exe", """http://www.indianafarmers.com/NewPublicSite/NewPublicHome.aspx"" ""C:\Users\domin\Documents\wkhtmltopdf_conversions\TestHomePage.pdf""", status)'worked
        'RunExecutable("C:\Program Files (x86)\wkhtmltopdf\wkhtmltopdf.exe", """http://localhost:51360/DiamondQuickQuotePages/DiamondQuoteProposal.aspx?PrinterFriendlyQuoteIds=380|1368"" ""C:\Users\domin\Documents\wkhtmltopdf_conversions\TestProposal.pdf""", status)'worked eventually but created several processes; bumped wait up from 4 to 30 seconds
        'RunExecutable("C:\Program Files (x86)\wkhtmltopdf\wkhtmltopdf.exe", """" & Server.MapPath(Request.ApplicationPath) & "/DiamondQuoteProposal.aspx?PrinterFriendlyQuoteIds=380|1368"" ""C:\Users\domin\Documents\wkhtmltopdf_conversions\TestProposal.pdf""", status)'for physical path
        'RunExecutable("C:\Program Files (x86)\wkhtmltopdf\wkhtmltopdf.exe", """" & Request.Url.AbsoluteUri & "/DiamondQuoteProposal.aspx?PrinterFriendlyQuoteIds=380|1368"" ""C:\Users\domin\Documents\wkhtmltopdf_conversions\TestProposal.pdf""", status)
        'RunExecutable("C:\Program Files (x86)\wkhtmltopdf\wkhtmltopdf.exe", """" & Request.Url.AbsoluteUri & "&pdf=yes" & """ ""C:\Users\domin\Documents\wkhtmltopdf_conversions\TestProposal.pdf""", status) 'added pdf=yes querystring param to prevent infinite loop whenever executable calls page
        'RunExecutable("C:\TFS\Internet\Velocirater\DiamondQuickQuotePages\Dev\LargePDF_20220216\Proposals\wkhtmltopdf\wkhtmltopdf.exe", """C:\Users\chhaw\OneDrive - Indiana Farmers Mutual Insurance Company\Desktop\PDFConversions\TestProposal.htm"" ""C:\Users\chhaw\OneDrive - Indiana Farmers Mutual Insurance Company\Desktop\PDFConversions\TestProposal.pdf""", status) 'testing local htm file; worked
        'Kill_wkhtmltopdf()
        StoreResponse()
        'RunExecutable("C:\Program Files (x86)\wkhtmltopdf\wkhtmltopdf.exe", """" & Request.Url.AbsoluteUri.Replace("DiamondQuoteProposal", "DiamondQuoteProposalLoader") & """ ""C:\Users\domin\Documents\wkhtmltopdf_conversions\TestProposal.pdf""", status)'not working when executed before render
    End Sub
    'Private Sub StoreResponse()
    '    Dim resp As HttpResponse = Me.Response
    '    'Dim resp As HttpResponse = HttpContext.Current.Response
    '    If resp IsNot Nothing AndAlso resp.OutputStream IsNot Nothing Then
    '        'resp.OutputStream.Close()
    '        'resp.Close()
    '        'resp.Clear()
    '        'resp.End()
    '        'resp.Flush()

    '        'Dim htmlBytes(resp.OutputStream.Length) As Byte
    '        'resp.OutputStream.Read(htmlBytes, 0, resp.OutputStream.Length)
    '        Dim htmlBytes As Byte() = GetStreamAsByteArray(resp.OutputStream)
    '        If htmlBytes IsNot Nothing Then
    '            Session("htmlBytes") = htmlBytes
    '            OpenNewWindow("DiamondQuoteProposalLoader.aspx")
    '        End If
    '    End If
    'End Sub
    Private Sub StoreResponse()
        okayToSavePage = True
        'Dim sw As New StringWriter
        'Dim tw As New HtmlTextWriter(sw)
        'okayToRender = False
        'Me.Page.RenderControl(tw) 'RegisterForEventValidation can only be called during Render(); - need to disable event validation on page
        'tw.Flush()
        'Dim htmlBytes As Byte() = Encoding.UTF8.GetBytes(sw.ToString)
        'If htmlBytes IsNot Nothing Then
        '    Session("htmlBytes") = htmlBytes
        '    OpenNewWindow("DiamondQuoteProposalLoader.aspx")
        'End If
        'Response.Redirect("DiamondQuoteProposalLoader.aspx")
        'Response.End()
        'Response.Clear()
        'Response.ContentType = "text/html"
        'Response.BinaryWrite(htmlBytes)
        'Response.Flush()
        'If htmlBytes IsNot Nothing Then
        '    Session("htmlBytes") = htmlBytes
        '    OpenNewWindow("DiamondQuoteProposalLoader.aspx")
        'End If

        'OpenNewWindow(Server.MapPath(Request.ApplicationPath) & "\Proposals\" & fileName & ".pdf") 'can't open file path
        'OpenNewWindow(Replace(Request.Url.AbsolutePath, "DiamondQuoteProposal.aspx", "") & "Proposals/" & fileName & ".pdf")
        OpenNewWindow(Replace(Request.Url.AbsolutePath, "DiamondQuoteProposal.aspx", "") & "Proposals/" & fileName & ".pdf", True) 'added param to signify it's to open a pdf

    End Sub
    Protected Overrides Sub Render(ByVal writer As HtmlTextWriter)
        'StoreResponse()'causes loop
        'If okayToRender = True Then
        '    MyBase.Render(writer)
        'Else
        '    MyBase.Render(writer)
        '    Response.End()
        '    okayToRender = True
        'End If

        Dim sb As New StringBuilder()
        Using sw As New StringWriter(sb)
            Using htw As New HtmlTextWriter(sw)
                MyBase.Render(htw)
                writer.Write(sb.ToString())

                'If okayToSavePage = True Then
                If okayToSavePage = True AndAlso fileName <> "" Then
                    'Dim htmlBytes As Byte() = Encoding.UTF8.GetBytes(sw.ToString)
                    'replace css for pdf
                    Dim htmlBytes As Byte() = Encoding.UTF8.GetBytes(ReplaceCssForPdf(sw.ToString))
                    If htmlBytes IsNot Nothing Then
                        'Session("htmlBytes") = htmlBytes
                        'OpenNewWindow("DiamondQuoteProposalLoader.aspx") 'won't execute js from here

                        'Dim filePath As String = "C:\Users\domin\Documents\wkhtmltopdf_conversions\TestProposal.htm"
                        'Dim filePath As String = Server.MapPath(Request.ApplicationPath) & "\" & fileName & ".htm"
                        Dim filePath As String = GetApplicationPath() & fileName & ".htm"
                        Dim fs As New FileStream(filePath, FileMode.Create)
                        fs.Write(htmlBytes, 0, htmlBytes.Length)
                        fs.Close()

                        If File.Exists(filePath) = True Then 'enclosed block in IF statement to make sure file exists
                            Dim status As String = ""
                            'RunExecutable("C:\Program Files (x86)\wkhtmltopdf\wkhtmltopdf.exe", """" & filePath & """ ""C:\Users\domin\Documents\wkhtmltopdf_conversions\TestProposal.pdf""", status)
                            'RunExecutable("C:\Program Files (x86)\wkhtmltopdf\wkhtmltopdf.exe", """" & filePath & """ """ & Server.MapPath(Request.ApplicationPath) & "\Proposals\TestProposal.pdf""", status) 'works
                            'RunExecutable(Server.MapPath(Request.ApplicationPath) & "\Proposals\wkhtmltopdf\wkhtmltopdf.exe", """" & filePath & """ """ & Server.MapPath(Request.ApplicationPath) & "\Proposals\" & fileName & ".pdf""", status) 'works
                            'updated 5/29/2013 to set pdf file path
                            'Dim pdfPath As String = Server.MapPath(Request.ApplicationPath) & "\Proposals\" & fileName & ".pdf"
                            Dim pdfPath As String = GetApplicationPath() & "Proposals\" & fileName & ".pdf"
                            RunExecutable(GetApplicationPath() & "Proposals\wkhtmltopdf\wkhtmltopdf.exe", """" & filePath & """ """ & pdfPath & """", status) 'works
                            'System.IO.File.Delete(filePath)'5/28/2013 - commenting for now so I can troubleshoot why agency logo isn't showing on pdf when deployed to AgentsOnly on webtest
                            'added configurable option 5/28/2013
                            If ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_DeleteHtmFile") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_DeleteHtmFile").ToString <> "" AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_DeleteHtmFile").ToString) = "YES" Then
                                System.IO.File.Delete(filePath)
                            End If
                            'added 5/29/2013
                            If File.Exists(pdfPath) = True Then
                                If ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_OpenPdfDirectly") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_OpenPdfDirectly").ToString <> "" AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_OpenPdfDirectly").ToString) = "YES" Then
                                    Dim fs_pdf As New FileStream(pdfPath, FileMode.Open, FileAccess.Read)
                                    Dim pdfBytes As Byte() = New Byte(fs_pdf.Length - 1) {}
                                    fs_pdf.Read(pdfBytes, 0, System.Convert.ToInt32(fs_pdf.Length))
                                    fs_pdf.Close()
                                    If pdfBytes IsNot Nothing Then
                                        'Session("pdfBytes") = pdfBytes

                                        'Dim proposalId As String = ""
                                        'Dim errorMsg As String = ""
                                        'Dim successfulInsert As Boolean = False
                                        ''5/30/2013 - updated to set userId
                                        'Dim userId As Integer = 0
                                        'If System.Web.HttpContext.Current.Session("DiamondUserId") IsNot Nothing AndAlso System.Web.HttpContext.Current.Session("DiamondUserId").ToString <> "" AndAlso IsNumeric(System.Web.HttpContext.Current.Session("DiamondUserId").ToString) = True Then
                                        '    userId = CInt(System.Web.HttpContext.Current.Session("DiamondUserId").ToString)
                                        'ElseIf ConfigurationManager.AppSettings("TestOrProd") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("TestOrProd").ToString) = "TEST" AndAlso ConfigurationManager.AppSettings("QuickQuote_UseTestVariables") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_UseTestVariables").ToString) = "YES" Then
                                        '    userId = CInt(ConfigurationManager.AppSettings("QuickQuoteTestUserId").ToString)
                                        'End If
                                        'InsertProposal(pdfBytes, userId, proposalId, errorMsg)
                                        'If errorMsg = "" Then
                                        '    If quoteIds IsNot Nothing AndAlso quoteIds.Count > 0 Then
                                        '        For Each qId As String In quoteIds
                                        '            Dim proposalQuoteLinkId As String = ""
                                        '            InsertProposalQuoteLink(proposalId, qId, proposalQuoteLinkId, errorMsg)
                                        '            If errorMsg = "" Then
                                        '                successfulInsert = True
                                        '            Else
                                        '                'error inserting proposal quote link
                                        '            End If
                                        '        Next
                                        '    Else
                                        '        'no quote ids stored earlier
                                        '    End If
                                        '    If successfulInsert = True Then
                                        '        'success; only delete if configured and db insert was successful
                                        '        If ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_DeletePdfFile") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_DeletePdfFile").ToString <> "" AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_DeletePdfFile").ToString) = "YES" Then
                                        '            System.IO.File.Delete(pdfPath)
                                        '        End If
                                        '        Response.Redirect("DiamondQuoteProposalLoader.aspx?proposalId=" & proposalId)
                                        '    End If
                                        'Else
                                        '    'error inserting proposal
                                        'End If
                                        'updated 4/5/2017 to accommodate Diamond Proposals (and not just ones for VelociRater quotes)
                                        Dim userId As Integer = qqHelper.IntegerForString(QuickQuoteHelperClass.DiamondUserId())
                                        If qqDiaProposal IsNot Nothing AndAlso (qqDiaProposal.DiamondProposalId > 0 OrElse diamondProposalId > 0) Then
                                            If qqDiaProposal.DiamondProposalId = 0 AndAlso diamondProposalId > 0 Then 'shouldn't happen
                                                qqDiaProposal.DiamondProposalId = diamondProposalId
                                            End If
                                            If qqDiaProposal.DiamondProposalId > 0 Then 'shouldn't even get here w/ latest outer IF logic (for proposal and proposal id on object or variable)
                                                qqDiaProposal.UpdateUserId = userId
                                                qqDiaProposal.DiamondProposalBytes = pdfBytes
                                                qqDiaProposal.DiamondProposalBinaryId = 0 'just to overwrite and force a new save if there was a previous value
                                                Dim errorMessages As List(Of String) = Nothing
                                                qqHelper.SaveDiamondProposal(qqDiaProposal, errorMessages:=errorMessages)
                                                If qqDiaProposal IsNot Nothing AndAlso qqDiaProposal.DiamondProposalBinaryId > 0 Then
                                                    'success; only delete if configured and db insert was successful
                                                    If ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_DeletePdfFile") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_DeletePdfFile").ToString <> "" AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_DeletePdfFile").ToString) = "YES" Then
                                                        System.IO.File.Delete(pdfPath)
                                                    End If
                                                    Response.Redirect("DiamondQuoteProposalLoader.aspx?diamondProposalBinaryId=" & qqDiaProposal.DiamondProposalBinaryId.ToString)
                                                End If
                                            End If
                                        Else
                                            Dim proposalId As String = ""
                                            Dim errorMsg As String = ""
                                            Dim successfulInsert As Boolean = False

                                            InsertProposal(pdfBytes, userId, proposalId, errorMsg)
                                            If errorMsg = "" Then
                                                If quoteIds IsNot Nothing AndAlso quoteIds.Count > 0 Then
                                                    For Each qId As String In quoteIds
                                                        Dim proposalQuoteLinkId As String = ""
                                                        InsertProposalQuoteLink(proposalId, qId, proposalQuoteLinkId, errorMsg)
                                                        If errorMsg = "" Then
                                                            successfulInsert = True
                                                        Else
                                                            'error inserting proposal quote link
                                                        End If
                                                    Next
                                                Else
                                                    'no quote ids stored earlier
                                                End If
                                                If successfulInsert = True Then
                                                    'success; only delete if configured and db insert was successful
                                                    If ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_DeletePdfFile") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_DeletePdfFile").ToString <> "" AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_DeletePdfFile").ToString) = "YES" Then
                                                        System.IO.File.Delete(pdfPath)
                                                    End If
                                                    Response.Redirect("DiamondQuoteProposalLoader.aspx?proposalId=" & proposalId)
                                                End If
                                            Else
                                                'error inserting proposal
                                            End If
                                        End If
                                    End If
                                Else
                                    'don't open pdf directly; can't delete
                                End If
                            End If
                        End If

                        'unregister window script
                        'Page.RegisterStartupScript("windowClientScript", "") 'can't do from here
                        'Me.hdnPdfFilePath.Value = ""
                    End If
                End If
            End Using
        End Using

    End Sub
    Private Function ReplaceCssForPdf(ByVal strInput As String) As String 'added 5/24/2013
        Dim strOutput As String = strInput

        strOutput = Replace(strOutput, "class=""page-break""", "class=""pdf_page-break""")
        strOutput = Replace(strOutput, "class=""page-break-section""", "class=""pdf_page-break-section""")
        strOutput = Replace(strOutput, "class=""DisclaimerHolder""", "class=""pdf_DisclaimerHolder""")
        strOutput = Replace(strOutput, "class=""QuickQuoteProposalDisclaimer""", "class=""pdf_QuickQuoteProposalDisclaimer""")

        Return strOutput
    End Function

    Private Sub SaveCurrentPage()
        Dim sw As New StringWriter
        Dim tw As New HtmlTextWriter(sw)
        Me.Page.RenderControl(tw)
        tw.Flush()
        Dim fs As New FileStream("filePath", FileMode.Create)
        Dim htmlBytes As Byte() = Encoding.UTF8.GetBytes(sw.ToString)
        fs.Write(htmlBytes, 0, htmlBytes.Length)
        fs.Close()
        Response.End()
    End Sub
    Private Function GetStreamAsByteArray(ByVal stream As System.IO.Stream) As Byte()
        If stream.CanRead = True Then
            'attempt 1
            Dim streamLength As Integer = Convert.ToInt32(stream.Length) 'method not supported when stream can't be read

            Dim fileData As Byte() = New Byte(streamLength) {}

            ' Read the file into a byte array
            stream.Read(fileData, 0, streamLength)
            stream.Close()

            Return (fileData)

            'attempt 2
            'Using ms As New MemoryStream
            '    stream.CopyTo(ms) 'Stream does not support reading.
            '    Return ms.ToArray
            'End Using

            'attempt 3
            'Dim sr As New StreamReader(stream) 'stream not readable
            'Dim strStream As String = sr.ReadToEnd()
            'Dim l As Integer = strStream.Length
            'Dim fileData As Byte() = New Byte(l) {}

            '' Read the file into a byte array
            'stream.Read(fileData, 0, l)
            'stream.Close()

            'Return (fileData)
        Else
            Return Nothing
        End If

    End Function


    Private Sub OpenNewWindow(ByVal url As String, Optional ByVal forPdf As Boolean = False) 'added 5/23/2013; updated w/ forPdf 5/24/2013
        If url <> "" Then
            If forPdf = True Then
                Me.hdnPdfFilePath.Value = url
                Dim strScript As String = "<script language=JavaScript>"
                'strScript &= " openPdf();"
                'updated 5/30/2013 to show button or open directly; previous method name now opens pdf in same window; old functionality is now called showButtonToOpenPdfWindow
                If ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_OpenPdfDirectly") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_OpenPdfDirectly").ToString <> "" AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_OpenPdfDirectly").ToString) = "YES" Then
                    strScript &= " openPdf();"
                Else
                    strScript &= " showButtonToOpenPdfWindow();"
                End If
                strScript &= "</script>"

                Page.RegisterStartupScript("pdfWindowClientScript", strScript)
            Else
                Dim strScript As String = "<script language=JavaScript>"
                'strScript &= " window.open('" & url & "');"
                strScript &= " window.open(""" & url & """);"
                strScript &= "</script>"

                'Page.RegisterStartupScript("clientScript", strScript)
                Page.RegisterStartupScript("windowClientScript", strScript) 'updated 5/24/2013 to differentiate clientScripts so multiple can execute
            End If
        End If
    End Sub

    ''' <summary>
    ''' Returns the current application path, makes sure it has a trailing backslash
    ''' </summary>
    ''' <returns></returns>
    Private Function GetApplicationPath() As String
        Dim appPath As String = Server.MapPath(Request.ApplicationPath)
        ' Make sure the trailing backslash is returned, add it if not
        If Right(appPath, 1) <> "\" Then appPath = appPath & "\"
        Return appPath
    End Function

    Public Sub RunExecutable(ByVal executable As String, ByVal arguments As String, ByRef status As String)
        'using same code as originally used in C:\Users\domin\Documents\Visual Studio 2005\WebSites\TestExecutableCall
        status = ""

        '*************** Feature Flags ***************
        Dim MaxWaitForExitMilliseconds As Integer = 1000
        Dim MaxTrials As Integer = 30
        Dim Trials As Integer = 0
        Dim chc = New CommonHelperClass
        If chc.ConfigurationAppSettingValueAsBoolean("Bug60875_Wkhtmltopdf") Then
            Dim WkArgs As String = "-q --javascript-delay 200" 'Required Defaults
            If ConfigurationManager.AppSettings("Wkhtmltopdf_RequiredArguments") IsNot Nothing Then
                WkArgs = chc.ConfigurationAppSettingValueAsString("Wkhtmltopdf_RequiredArguments")
            End If

            arguments = WkArgs & " " & arguments

            If ConfigurationManager.AppSettings("Wkhtmltopdf_WaitForExitMilliseconds_Per_Trial") IsNot Nothing Then
                MaxWaitForExitMilliseconds = chc.ConfigurationAppSettingValueAsInteger("Wkhtmltopdf_WaitForExitMilliseconds_Per_Trial")
            End If

            If ConfigurationManager.AppSettings("Wkhtmltopdf_MaxTrials") IsNot Nothing Then
                MaxTrials = chc.ConfigurationAppSettingValueAsInteger("Wkhtmltopdf_MaxTrials")
            End If

        End If

        Dim starter As ProcessStartInfo = New ProcessStartInfo(executable, arguments)
        starter.CreateNoWindow = True
        starter.RedirectStandardOutput = True
        starter.RedirectStandardError = True
        starter.UseShellExecute = False
        starter.WorkingDirectory = GetApplicationPath() & "Proposals\wkhtmltopdf\"

        Dim process As Process = New Process()
        process.StartInfo = starter

        Dim compareTime As DateTime = DateAdd(DateInterval.Second, -5, Date.Now)

        process.Start()
        If chc.ConfigurationAppSettingValueAsBoolean("Bug60875_Wkhtmltopdf") = False Then
            ' -----   Old Code  -----
            'process.WaitForExit(4000)
            'updated to use variable
            Dim waitForExitMilliseconds As Integer = 20000
            If ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_WaitForExitMilliseconds") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_WaitForExitMilliseconds").ToString <> "" AndAlso IsNumeric(ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_WaitForExitMilliseconds").ToString) = True Then
                waitForExitMilliseconds = CInt(ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_WaitForExitMilliseconds").ToString)
            End If
            process.WaitForExit(waitForExitMilliseconds)
            'process.WaitForExit()'never finished
            'process.WaitForExit(30000) 'updated to see if program can finish converting in that amount of time; wasn't creating file
            'process.WaitForExit(60000) 'trying double

            While process.HasExited = False
                If process.StartTime > compareTime Then
                    process.CloseMainWindow()
                    Try
                        process.Kill()
                    Catch ex As Exception
                        status = "<b>Kill failed-</b>"
                    End Try
                End If
            End While
        Else
            ' -----   New Code  -----
            While process.WaitForExit(MaxWaitForExitMilliseconds) = False
                If Trials = MaxTrials Then
                    process.CloseMainWindow()
                    Try
                        process.Kill()
                    Catch ex As Exception
                        status = "<b>Kill failed-</b>"
                    End Try
                End If
                Trials += 1
            End While
        End If

        Dim strOutput As String = process.StandardOutput.ReadToEnd
        Dim strError As String = process.StandardError.ReadToEnd

        If (process.ExitCode <> 0) Then
            'ShowError("Error")
            status &= "Error<br><u>Output</u> - " & strOutput & "<br><u>Error</u> - " & strError

            'added 5/28/2013 for debugging on ifmwebtest (since it always works locally); seems to work okay after changing WaitForExit from 4000 to 10000 (4 seconds to 10 seconds)
            If ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_SendErrorEmail") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_SendErrorEmail").ToString <> "" AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_SendErrorEmail").ToString) = "YES" Then
                Dim eMsg As String = ""
                eMsg &= "<b>executable:</b>  " & executable
                eMsg &= "<br /><br />"
                eMsg &= "<b>arguments:</b>  " & arguments
                eMsg &= "<br /><br />"
                eMsg &= "<b>status:</b>  " & status
                If chc.ConfigurationAppSettingValueAsBoolean("Bug60875_Wkhtmltopdf") = True Then
                    eMsg &= "<br /><hr /><br />"
                    eMsg &= "<b>Use New Timeout Code:</b>  True"
                    eMsg &= "<br /><br />"
                    eMsg &= "<b>Max Wait per Trial:</b>  " & MaxWaitForExitMilliseconds
                    eMsg &= "<br /><br />"
                    eMsg &= "<b>Max Trials:</b>  " & MaxTrials
                    eMsg &= "<br /><br />"
                    eMsg &= "<b>Trials Used:</b>  " & Trials
                End If
                qqHelper.SendEmail("ProposalPdfConverter@indianafarmers.com", "dmink@indianafarmers.com; chawley@indianafarmers.com", "Error Converting Proposal to PDF", eMsg)
            End If
            Else
            'ShowError("Success")
            status &= "Success<br><u>Output</u> - " & strOutput & "<br><u>Error</u> - " & strError
        End If

        process.Close()
        process.Dispose()
        process = Nothing
        starter = Nothing

        'process.CloseMainWindow()
        'Dim buffer As StringBuilder = New StringBuilder()
        'Using reader As StreamReader = process.StandardOutput

        '    Dim line As String = reader.ReadLine()
        '    While line <> ""
        '        buffer.Append(line)
        '        buffer.Append(Environment.NewLine)
        '        line = reader.ReadLine()
        '        Thread.Sleep(100)
        '    End While
        'End Using

        'If (process.ExitCode <> 0) Then
        '    'ShowError(executable & " exited with ExitCode " & process.ExitCode & ". Output: " & buffer.ToString())
        '    ShowError("There was an error")
        'Else
        '    ShowError("OK") 'buffer.ToString.Trim)
        'End If

        'process.Close()
        'process.Dispose()

    End Sub
    Public Sub Kill_wkhtmltopdf()
        'will automatically end on program END with VB.NET;
        'will need this if using ASP.NET to fire off Excel
        Dim regName As String = ""
        Dim uppName As String = ""

        Dim objProsses As Diagnostics.Process() = Diagnostics.Process.GetProcesses
        Dim intCount As Integer
        For intCount = 0 To objProsses.Length - 1
            regName = objProsses(intCount).ProcessName
            uppName = objProsses(intCount).ProcessName.ToUpper
            '--fullpath = objprosses(intcount).MainModule.FileName
            If objProsses(intCount).ProcessName.ToUpper = "WKHTMLTOPDF.EXE *32" OrElse objProsses(intCount).ProcessName.ToUpper = "WKHTMLTOPDF.EXE" OrElse objProsses(intCount).ProcessName.ToUpper = "WKHTMLTOPDF" Then
                objProsses(intCount).Kill()
            ElseIf objProsses(intCount).ProcessName.ToUpper = "CONHOST.EXE" OrElse objProsses(intCount).ProcessName.ToUpper = "CONHOST" Then
                objProsses(intCount).Kill()
            End If
        Next

    End Sub

    Private Sub TestConvert()
        Dim status As String = ""

        'Testing files can be found - \\ifmwebtest\wwwroot\ITStaff\Chad Hawley\Big Proposals

        'Arguments for WKHTMLTOPDF 
        Dim Args As StringBuilder = New StringBuilder()
        '***** These are Trial switches
        'Args.Append("--enable-local-file-access ")
        'Args.Append("--debug-javascript ")
        'Args.Append("--load-error-handling skip ")
        '***** The below items are required for v. 11.0 rc2
        '***** These are now added in RunExecutable()
        '**Args.Append("--javascript-delay 200 ")
        '**Args.Append("-q ")
        Args.ToString()

        'Html Input files for WKHTMLTOPDF
        'Testing the combination of 2 Large PDFs that used to fail individually
        Dim HtmlInput As StringBuilder = New StringBuilder()
        HtmlInput.Append("""\\ifmwebtest\wwwroot\ITStaff\Chad Hawley\Big Proposals\Test1.htm"" ")
        HtmlInput.Append("""\\ifmwebtest\wwwroot\ITStaff\Chad Hawley\Big Proposals\Test2.htm"" ")

        'PDF Output file for WKHTMLTOPDF
        Dim PdfOutput As String = String.Empty
        PdfOutput = """\\ifmwebtest\wwwroot\ITStaff\Chad Hawley\Big Proposals\Test2.pdf"" "

        'Test WKHTMLTOPDF included with this project
        RunExecutable(GetApplicationPath() & "Proposals\wkhtmltopdf\wkhtmltopdf.exe", Args.ToString() & HtmlInput.ToString & PdfOutput, status)
        Response.Write(status)
    End Sub

    Private Sub InsertProposal(ByVal proposalBytes As Byte(), ByVal proposalUserId As String, ByRef proposalId As String, ByRef errorMsg As String)
        proposalId = ""
        errorMsg = ""

        Using sql As New SQLexecuteObject(ConfigurationManager.AppSettings("connQQ"))
            sql.queryOrStoredProc = "usp_Insert_Proposal"

            sql.inputParameters = New ArrayList

            If proposalBytes IsNot Nothing Then
                sql.inputParameters.Add(New SqlParameter("@proposalBytes", proposalBytes))
            End If
            If proposalUserId <> "" AndAlso IsNumeric(proposalUserId) = True AndAlso CInt(proposalUserId) > 0 Then '5/30/2013 - updated to make sure it's greater than 0 (since that will be the default passed in)
                sql.inputParameters.Add(New SqlParameter("@proposalUserId", CInt(proposalUserId)))
            End If

            sql.outputParameter = New SqlParameter("@proposalId", Data.SqlDbType.Int)

            sql.ExecuteStatement()

            If sql.rowsAffected = 0 OrElse sql.hasError = True Then
                'error
                errorMsg = "error inserting proposal into database"
            Else
                proposalId = sql.outputParameter.Value
            End If
        End Using

    End Sub
    Private Sub InsertProposalQuoteLink(ByVal proposalId As String, ByVal quoteId As String, ByRef proposalQuoteLinkId As String, ByRef errorMsg As String)
        proposalQuoteLinkId = ""
        errorMsg = ""

        If proposalId = "" OrElse IsNumeric(proposalId) = False Then
            errorMsg = qqHelper.appendText(errorMsg, "proposalId is required", "; ")
        End If
        If quoteId = "" OrElse IsNumeric(quoteId) = False Then
            errorMsg = qqHelper.appendText(errorMsg, "quoteId is required", "; ")
        End If
        If errorMsg = "" Then
            Using sql As New SQLexecuteObject(ConfigurationManager.AppSettings("connQQ"))
                sql.queryOrStoredProc = "usp_Insert_ProposalQuoteLink"

                sql.inputParameters = New ArrayList
                sql.inputParameters.Add(New SqlParameter("@proposalId", CInt(proposalId)))
                sql.inputParameters.Add(New SqlParameter("@quoteId", CInt(quoteId)))

                sql.outputParameter = New SqlParameter("@proposalQuoteLinkId", Data.SqlDbType.Int)

                sql.ExecuteStatement()

                If sql.rowsAffected = 0 OrElse sql.hasError = True Then
                    'error
                    errorMsg = "error inserting proposal quote link into database"
                Else
                    proposalQuoteLinkId = sql.outputParameter.Value
                End If
            End Using
        End If

    End Sub
    Public Function GetExistingProposalId(ByVal quoteIds As String) As String 'added 5/30/2013
        Dim existingProposalId As String = ""

        If quoteIds <> "" Then
            Using sql As New SQLselectObject(ConfigurationManager.AppSettings("connQQ"))
                sql.queryOrStoredProc = "usp_Get_Existing_Proposal"
                sql.parameter = New SqlParameter("@quoteIds", quoteIds)

                Dim dr As SqlDataReader = sql.GetDataReader
                If dr IsNot Nothing AndAlso dr.HasRows = True Then
                    dr.Read() 'could have multiple results; this will just read the 1st one, which should be the latest
                    existingProposalId = dr.Item("proposalId").ToString.Trim
                End If
            End Using
        End If

        Return existingProposalId
    End Function
    'Public Function GetAgencyName(ByVal name As QuickQuoteName) As String 'added 6/10/2013; moved to helper class
    '    Dim agName As String = ""

    '    If name IsNot Nothing Then
    '        If name.CommercialDBAname <> "" Then
    '            agName = name.CommercialDBAname
    '        ElseIf name.DoingBusinessAsName <> "" Then 'this probably won't ever have a value
    '            agName = name.DoingBusinessAsName
    '        ElseIf name.CommercialIRSname <> "" Then
    '            agName = name.CommercialIRSname
    '        Else
    '            agName = name.DisplayNameForWeb 'just in case nothing is found in any of the above, which should mean that this one is blank too
    '        End If
    '    End If

    '    Return agName
    'End Function
End Class
