Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports System.Data
Partial Class EndorsementTestingApp
    Inherits System.Web.UI.Page

    Dim qqXml As New QuickQuoteXML
    Dim qqHelper As New QuickQuoteHelperClass

    Private Sub EndorsementTesting_Load(sender As Object, e As EventArgs) Handles Me.Load
        Page.MaintainScrollPositionOnPostBack = True

        If Page.IsPostBack = False Then
            'added agents link 11/11/2016
            Dim agentsLinkHref As String = QuickQuoteHelperClass.configAppSettingValueAsString("AgentsLink")
            If String.IsNullOrWhiteSpace(agentsLinkHref) = False Then
                Me.AgentsLink.HRef = agentsLinkHref
                Me.AgentsLinkSection.Visible = True
            Else
                Me.AgentsLinkSection.Visible = False
            End If

            Me.btnPolicySearch.Attributes.Add("onclick", "btnSubmit_Click(this, 'Searching...');") 'for disable button and server-side logic
            Me.btnEndorsementSearch.Attributes.Add("onclick", "btnSubmit_Click(this, 'Searching...');") 'for disable button and server-side logic

            Me.btnNewEndorsementCreate.Attributes.Add("onclick", "btnSubmit_Click(this, 'Creating...');") 'for disable button and server-side logic

            Me.btnExistingEndorsementSave.Attributes.Add("onclick", "btnSubmit_Click(this, 'Saving...');") 'for disable button and server-side logic
            Me.btnExistingEndorsementRate.Attributes.Add("onclick", "btnSubmit_Click(this, 'Rating...');") 'for disable button and server-side logic
            'Me.btnExistingEndorsementDelete.Attributes.Add("onclick", "btnSubmit_Click(this, 'Deleting...');") 'for disable button and server-side logic
            Me.btnExistingEndorsementDelete.Attributes.Add("onclick", "return SubmitConfirm(this, 'Are you sure you want to delete this endorsement?', 'Deleting...');") 'for confirm box, disable button, and call server-side logic

            Me.btnPolicyInfoNewEndorsement.Attributes.Add("onclick", "btnSubmit_Click(this, 'Loading...');") 'for disable button and server-side logic

            'added 11/11/2016
            Me.btnExistingEndorsementIssuance.Attributes.Add("onclick", "return SubmitConfirm(this, 'Are you sure you are ready to issue this endorsement?', 'Redirecting...');") 'for confirm box, disable button, and call server-side logic

            'added 5/15/2019
            Me.btnViewReadOnly.Attributes.Add("onclick", "btnSubmit_Click(this, 'Loading...');") 'for disable button and server-side logic

            'added 11/11/2016
            Dim agencyCount As Integer = 0
            qqHelper.LoadUserAgencyDropDown(Me.ddlEndorsementSearchAgencyCodeSelection)
            If Me.ddlEndorsementSearchAgencyCodeSelection.Items IsNot Nothing AndAlso Me.ddlEndorsementSearchAgencyCodeSelection.Items.Count > 0 Then
                agencyCount = Me.ddlEndorsementSearchAgencyCodeSelection.Items.Count
                If agencyCount > 1 Then 'added 11/11/2016
                    Dim liAll As New ListItem
                    With liAll
                        .Text = "All My Agencies"
                        .Value = "0"
                    End With
                    Me.ddlEndorsementSearchAgencyCodeSelection.Items.Insert(0, liAll)
                End If
                For Each li As ListItem In Me.ddlEndorsementSearchAgencyCodeSelection.Items
                    Me.ddlPolicySearchAgencyCodeSelection.Items.Add(li)
                Next
            End If
            Dim isStaff As Boolean = qqHelper.IsHomeOfficeStaffUser
            Dim canAccessEmployeePolicies As Boolean = qqHelper.CanUserAccessEmployeePolicies
            'If isStaff = True AndAlso agencyCount > 0 Then
            '    Me.EndorsementSearchAgencyCodeRow.Visible = False
            '    Me.EndorsementSearchAgencyCodeSelectionRow.Visible = True
            '    Me.PolicySearchAgencyCodeRow.Visible = False
            '    Me.PolicySearchAgencyCodeSelectionRow.Visible = True
            'Else
            '    Me.EndorsementSearchAgencyCodeRow.Visible = True
            '    Me.EndorsementSearchAgencyCodeSelectionRow.Visible = False
            '    Me.PolicySearchAgencyCodeRow.Visible = True
            '    Me.PolicySearchAgencyCodeSelectionRow.Visible = False
            'End If
            'updated 11/11/2016
            If agencyCount > 0 Then
                Me.EndorsementSearchAgencyCodeRow.Visible = False
                Me.EndorsementSearchAgencyCodeSelectionRow.Visible = True
                Me.PolicySearchAgencyCodeRow.Visible = False
                Me.PolicySearchAgencyCodeSelectionRow.Visible = True
            Else
                Me.EndorsementSearchAgencyCodeRow.Visible = True
                Me.EndorsementSearchAgencyCodeSelectionRow.Visible = False
                Me.PolicySearchAgencyCodeRow.Visible = True
                Me.PolicySearchAgencyCodeSelectionRow.Visible = False
            End If
            If isStaff = True Then
                Me.EndorsementSearchAgencyCodeWarningRow.Visible = True
                Me.PolicySearchAgencyCodeWarningRow.Visible = True
            Else
                Me.EndorsementSearchAgencyCodeWarningRow.Visible = False
                Me.PolicySearchAgencyCodeWarningRow.Visible = False
            End If
        End If

    End Sub

    Private Sub ShowError(ByVal message As String, Optional ByVal redirect As Boolean = False, Optional ByVal redirectPage As String = "")
        message = Replace(message, "\", "\\")
        message = Replace(message, "<br>", "\n")
        message = Replace(message, "<br />", "\n")
        message = Replace(message, vbCrLf, "\n")

        Dim strScript As String = "<script language=JavaScript>"
        strScript &= "alert(""" & message & """);"
        If redirect = True Then
            If redirectPage = "" Then
                redirectPage = "MyVelociRater.aspx" 'use config key if available
            End If
            strScript &= " window.location.href='" & redirectPage & "';"
        End If
        strScript &= "</script>"

        Page.RegisterStartupScript("clientScript", strScript)

    End Sub

    Protected Sub ddlAction_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlAction.SelectedIndexChanged
        HideAllPanels()
        Select Case UCase(Me.ddlAction.SelectedValue)
            Case "ENDORSEMENT SEARCH"
                Me.pnlEndorsementSearch.Visible = True
            Case "POLICY SEARCH"
                Me.pnlPolicySearch.Visible = True
            Case Else

        End Select
    End Sub
    Private Sub HideAllPanels()
        Me.pnlEndorsementSearch.Visible = False
        Me.pnlPolicySearch.Visible = False
        Me.pnlPolicyInfo.Visible = False
        Me.pnlSearchResults.Visible = False
        Me.pnlNewEndorsement.Visible = False
        Me.pnlExistingEndorsement.Visible = False
        Me.tblSelectedInfo.Visible = False
        Me.tblCurrentInfo.Visible = False
        Me.btnPolicyInfoNewEndorsement.Visible = False
        Me.btnExistingEndorsementSave.Enabled = False
        Me.btnExistingEndorsementRate.Enabled = False
        Me.btnExistingEndorsementDelete.Enabled = False
        Me.ExistingEndorsementIssuanceSection.Visible = False 'added 11/11/2016
        Me.ReadOnlyButtonSection.Visible = False 'added 5/15/2019
    End Sub
    Private Sub PrepareForSearch() 'added 11/6/2016
        Me.pnlSearchResults.Visible = False
        Me.pnlPolicyInfo.Visible = False
        Me.tblSelectedInfo.Visible = False
        Me.tblCurrentInfo.Visible = False
        Me.pnlNewEndorsement.Visible = False
        Me.btnPolicyInfoNewEndorsement.Visible = False
        Me.pnlExistingEndorsement.Visible = False
        Me.btnExistingEndorsementSave.Enabled = False
        Me.btnExistingEndorsementRate.Enabled = False
        Me.btnExistingEndorsementDelete.Enabled = False
        Me.ExistingEndorsementIssuanceSection.Visible = False 'added 11/11/2016
        Me.ReadOnlyButtonSection.Visible = False 'added 5/15/2019
    End Sub

    Private Sub dgrdSearchResults_ItemCommand(source As Object, e As DataGridCommandEventArgs) Handles dgrdSearchResults.ItemCommand
        'added 11/11/2016
        Dim dtFromViewstateOrSession As New DataTable
        Dim sortFromViewstateOrSession As String = ""
        Dim isSession As Boolean = False
        If ViewState.Item("dt") IsNot Nothing Then
            dtFromViewstateOrSession = ViewState.Item("dt")
            sortFromViewstateOrSession = ViewState.Item("sort")
        Else
            isSession = True
            dtFromViewstateOrSession = Session.Item("dt")
            sortFromViewstateOrSession = Session.Item("sort")
        End If

        Select Case e.CommandName

            Case "Sort"
                'retrieve datatable and sort from view state
                Dim sort As String = e.CommandArgument 'e.SortExpression
                'If ViewState.Item("sort") = sort Then
                'updated 11/11/2016
                If sortFromViewstateOrSession = sort Then

                    'handle for multiple sort values
                    If InStr(sort, ",") Then
                        Dim sortArray As Array = Split(sort, ",")

                        sort = ""
                        For Each sortItem As String In sortArray
                            sortItem = Trim(sortItem)

                            If InStr(UCase(sortItem), " DESC") Then
                                sortItem = Left(sortItem, Len(sortItem) - 5)
                            ElseIf InStr(UCase(sortItem), " ASC") Then
                                sortItem = Left(sortItem, Len(sortItem) - 4) & " desc"
                            Else
                                sortItem = sortItem & " desc"
                            End If

                            If sort = "" Then
                                sort = sortItem
                            Else
                                sort &= ", " & sortItem
                            End If
                        Next
                    Else
                        'only one sort value
                        sort = sort & " desc"
                    End If
                End If

                Dim dt As New DataTable
                'dt = ViewState.Item("dt")
                'updated 11/11/2016
                dt = dtFromViewstateOrSession
                dt.DefaultView.Sort = sort
                'ViewState.Item("sort") = sort
                'updated 11/11/2016
                If isSession = True Then
                    Session.Item("sort") = sort
                Else
                    ViewState.Item("sort") = sort
                End If

                Me.dgrdSearchResults.DataSource = dt

                Me.dgrdSearchResults.CurrentPageIndex = 0
                Me.lblPage.Text = "Page " & Me.dgrdSearchResults.CurrentPageIndex + 1 & " of " & Me.dgrdSearchResults.PageCount

                Me.dgrdSearchResults.SelectedIndex = -1

                Me.dgrdSearchResults.DataBind()

            Case "Page"
                If UCase(e.CommandArgument.ToString) = "NEXT" Then
                    Me.dgrdSearchResults.CurrentPageIndex += 1
                ElseIf UCase(e.CommandArgument.ToString) = "PREV" Then
                    Me.dgrdSearchResults.CurrentPageIndex -= 1
                Else 'numeric
                    Me.dgrdSearchResults.CurrentPageIndex = e.CommandArgument - 1
                End If

                Dim dt As New DataTable
                'dt = ViewState.Item("dt")
                'dt.DefaultView.Sort = ViewState.Item("sort")
                'updated 11/11/2016
                dt = dtFromViewstateOrSession
                dt.DefaultView.Sort = sortFromViewstateOrSession

                Me.dgrdSearchResults.DataSource = dt
                Me.dgrdSearchResults.DataBind()


                Me.lblPage.Text = "Page " & Me.dgrdSearchResults.CurrentPageIndex + 1 & " of " & Me.dgrdSearchResults.PageCount
            Case "Select"
                System.Threading.Thread.Sleep(500) 'delay 1/2 second

                Dim polNum As String = ItemCellText(e.Item.Cells(1).Text)
                Dim quoteNum As String = ItemCellText(e.Item.Cells(2).Text)
                Dim polId As String = ItemCellText(e.Item.Cells(3).Text)
                Dim polImgNum As String = ItemCellText(e.Item.Cells(4).Text)
                Dim transType As String = ItemCellText(e.Item.Cells(5).Text)
                Dim polStatusCode As String = ItemCellText(e.Item.Cells(6).Text)
                Dim agCode As String = ItemCellText(e.Item.Cells(7).Text)
                Dim effDate As String = ItemCellText(e.Item.Cells(8).Text)
                Dim expDate As String = ItemCellText(e.Item.Cells(9).Text)
                Dim teffDate As String = ItemCellText(e.Item.Cells(10).Text)
                Dim texpDate As String = ItemCellText(e.Item.Cells(11).Text)

                Me.pnlPolicyInfo.Visible = True

                Dim getCurrentInfo As Boolean = False
                Select Case UCase(Me.ddlAction.SelectedValue)
                    Case "ENDORSEMENT SEARCH"
                        Select Case UCase(Me.ddlEndorsementSearchByPolicyOrImage.SelectedValue)
                            Case "BY IMAGE"
                                'ByImage
                                getCurrentInfo = True
                            Case Else
                                'ByPolicy; should already be showing current
                        End Select
                    Case "POLICY SEARCH"
                        Select Case UCase(Me.ddlPolicySearchByPolicyOrImage.SelectedValue)
                            Case "BY IMAGE"
                                'ByImage
                                getCurrentInfo = True
                            Case Else
                                'ByPolicy; should already be showing current
                        End Select
                    Case Else

                End Select

                Me.tblSelectedInfo.Visible = True
                Me.lblPolicyNumber_selected.Text = polNum
                Me.lblQuoteNumber_selected.Text = quoteNum
                Me.lblPolicyId_selected.Text = polId
                Me.lblPolicyImageNum_selected.Text = polImgNum
                Me.lblTransType_selected.Text = transType
                Me.lblPolStatusCode_selected.Text = polStatusCode
                Me.lblAgencyCode_selected.Text = agCode
                Me.lblTermDates_selected.Text = effDate & " - " & expDate
                Me.lblTranDates_selected.Text = teffDate & " - " & texpDate

                Me.pnlNewEndorsement.Visible = False
                Me.pnlExistingEndorsement.Visible = False
                Me.btnPolicyInfoNewEndorsement.Enabled = True 'added 11/6/2016
                Me.btnPolicyInfoNewEndorsement.ToolTip = "" 'added 11/6/2016

                Dim isEndorsement As Boolean = False 'added 5/15/2019

                'If UCase(Me.lblTransType_selected.Text) = "ENDORSEMENT" AndAlso UCase(Me.lblPolStatusCode_selected.Text) = "PENDING" Then
                'updated 10/10/2019
                If UCase(Me.lblTransType_selected.Text) = "ENDORSEMENT" AndAlso (UCase(Me.lblPolStatusCode_selected.Text) = "PENDING" OrElse (QuickQuoteHelperClass.ConsiderEndorsementQuoteStatusAsPending() = True AndAlso UCase(Me.lblPolStatusCode_selected.Text) = "QUOTE")) Then
                    Me.btnPolicyInfoNewEndorsement.Visible = False
                    'load existing endorsement if possible

                    'If qqHelper.IsPositiveIntegerString(Me.lblPolicyId_selected.Text) = True AndAlso qqHelper.IsPositiveIntegerString(Me.lblPolicyImageNum_selected.Text) = True Then
                    '    Me.pnlExistingEndorsement.Visible = True

                    '    Dim qqE As QuickQuoteObject = Nothing
                    '    Dim errorMessage As String = ""
                    '    qqE = qqXml.QuickQuoteEndorsementForPolicyIdAndImageNum(CInt(Me.lblPolicyId_selected.Text), CInt(Me.lblPolicyImageNum_selected.Text), errorMessage:=errorMessage)
                    '    If qqE IsNot Nothing Then
                    '        If ViewState.Item("QuickQuoteEndorsement") IsNot Nothing Then
                    '            ViewState.Item("QuickQuoteEndorsement") = qqE
                    '        Else
                    '            ViewState.Add("QuickQuoteEndorsement", qqE)
                    '        End If
                    '        Me.lblExistingEndorsementTranEffDate.Text = qqE.TransactionEffectiveDate
                    '        Me.txtExistingEndorsementRemarks.Text = qqE.TransactionRemark 'probably need new QQO property; added 11/4/2016
                    '        If Me.ddlExistingEndorsementTransReason.Items.FindByValue(qqE.TransactionReasonId) IsNot Nothing Then
                    '            Me.ddlExistingEndorsementTransReason.SelectedValue = qqE.TransactionReasonId
                    '        End If
                    '        'added policy info label 11/6/2016; could use to verify against the selected policy info provided above
                    '        Me.lblExistingEndorsementPolicyNum.Text = qqE.PolicyNumber
                    '        Me.lblExistingEndorsementPolicyId.Text = qqE.PolicyId
                    '        Me.lblExistingEndorsementPolicyImageNum.Text = qqE.PolicyImageNum

                    '        Me.btnExistingEndorsementSave.Enabled = True
                    '        Me.btnExistingEndorsementRate.Enabled = True
                    '        Me.btnExistingEndorsementDelete.Enabled = True
                    '    Else
                    '        Me.btnExistingEndorsementSave.Enabled = False
                    '        Me.btnExistingEndorsementRate.Enabled = False
                    '        Me.btnExistingEndorsementDelete.Enabled = False

                    '        If String.IsNullOrWhiteSpace(errorMessage) = False Then
                    '            ShowError(errorMessage)
                    '        Else
                    '            ShowError("problem loading QuickQuoteEndorsement")
                    '        End If
                    '    End If
                    'Else
                    '    ShowError("unable to obtain policyId and policyImageNum to load Endorsement info")
                    'End If
                    'updated 11/8/2016 to use new common method
                    LoadExistingEndorsement(Me.lblPolicyId_selected.Text, Me.lblPolicyImageNum_selected.Text)
                    isEndorsement = True 'added 5/15/2019
                Else
                    Me.btnPolicyInfoNewEndorsement.Visible = True
                    '11/6/2016 note: maybe only show this if status if Inforce or Future; backend logic will still handle if not from here
                    If getCurrentInfo = False Then 'added 11/6/2016
                        'selected info is current
                        Select Case UCase(Me.lblPolStatusCode_selected.Text)
                            Case "IN-FORCE", "FUTURE"
                                Me.btnPolicyInfoNewEndorsement.Enabled = True
                                Me.btnPolicyInfoNewEndorsement.ToolTip = ""
                            Case Else
                                Me.btnPolicyInfoNewEndorsement.Enabled = False
                                Me.btnPolicyInfoNewEndorsement.ToolTip = "Endorsements can only be submitted on policies with a status of in-force or future"
                        End Select
                    End If
                End If

                If getCurrentInfo = True Then
                    Me.tblCurrentInfo.Visible = True
                    'Dim policyLookupInfo As New QuickQuotePolicyLookupInfo
                    'With policyLookupInfo
                    '    .PolicyNumber = Me.lblPolicyNumber_selected.Text
                    '    .PolicyLookupType = QuickQuotePolicyLookupInfo.LookupType.ByPolicy
                    'End With
                    'Dim policyResult As QuickQuotePolicyLookupInfo = Nothing
                    'Dim caughtDatabaseError As Boolean = False
                    'policyResult = QuickQuoteHelperClass.PolicyResultForLookupInfo(policyLookupInfo, caughtDatabaseError:=caughtDatabaseError)
                    'updated 5/31/2019
                    Dim caughtDatabaseError As Boolean = False
                    Dim policyResult As QuickQuotePolicyLookupInfo = QuickQuoteHelperClass.CurrentPolicyResultForPolicyNumber(Me.lblPolicyNumber_selected.Text, caughtDatabaseError:=caughtDatabaseError)
                    If policyResult IsNot Nothing Then
                        With policyResult
                            Me.lblPolicyNumber.Text = .PolicyNumber
                            Me.lblQuoteNumber.Text = .QuoteNumber
                            Me.lblPolicyId.Text = .PolicyId
                            Me.lblPolicyImageNum.Text = .PolicyImageNum
                            Select Case .TransTypeId
                                Case 2
                                    Me.lblTransType.Text = "New Business"
                                Case 3
                                    Me.lblTransType.Text = "Endorsement"
                                Case Else
                                    Me.lblTransType.Text = "TransTypeId " & .TransTypeId.ToString
                            End Select
                            Select Case .PolicyStatusCodeId
                                Case 1
                                    Me.lblPolStatusCode.Text = "In-Force"
                                Case 2
                                    Me.lblPolStatusCode.Text = "Future"
                                Case 3
                                    Me.lblPolStatusCode.Text = "History"
                                Case 4
                                    Me.lblPolStatusCode.Text = "Pending"
                                Case 12 'added 10/10/2019
                                    Me.lblPolStatusCode.Text = "Quote"
                                Case Else
                                    Me.lblPolStatusCode.Text = "PolicyStatusCodeId " & .PolicyStatusCodeId.ToString
                            End Select
                            Me.lblAgencyCode.Text = .AgencyCode
                            Me.lblTermDates.Text = .EffectiveDate & " - " & .ExpirationDate
                            Me.lblTranDates.Text = .TransactionEffectiveDate & " - " & .TransactionExpirationDate
                        End With
                        'added 11/6/2016
                        Select Case UCase(Me.lblPolStatusCode.Text)
                            Case "IN-FORCE", "FUTURE"
                                Me.btnPolicyInfoNewEndorsement.Enabled = True
                                Me.btnPolicyInfoNewEndorsement.ToolTip = ""
                            Case Else
                                Me.btnPolicyInfoNewEndorsement.Enabled = False
                                Me.btnPolicyInfoNewEndorsement.ToolTip = "Endorsements can only be submitted on policies with a status of in-force or future"
                        End Select
                    Else
                        Me.tblCurrentInfo.Visible = False
                        If caughtDatabaseError = True Then
                            ShowError("database error encountered during search for current info")
                        Else
                            ShowError("no results found for current info")
                        End If
                    End If
                End If

                'added 11/6/2016
                If Me.btnPolicyInfoNewEndorsement.Visible = True AndAlso Me.btnPolicyInfoNewEndorsement.Enabled = True Then
                    Dim polIdToCheck As Integer = 0
                    If getCurrentInfo = True AndAlso qqHelper.IsPositiveIntegerString(Me.lblPolicyId.Text) = True Then
                        polIdToCheck = CInt(Me.lblPolicyId.Text)
                    ElseIf qqHelper.IsPositiveIntegerString(Me.lblPolicyId_selected.Text) = True Then
                        polIdToCheck = CInt(Me.lblPolicyId_selected.Text)
                    End If
                    'note: could also just check for pending endorsement image for policy... a policy can have multiple policyIds... not sure if it's valid to enter endorsement on one policyId if there's already a pending endorsement on a different policyId
                    If polIdToCheck > 0 Then
                        If QuickQuoteHelperClass.HasPendingEndorsementImage(policyId:=polIdToCheck) = True Then 'could pass in policyResult byref if we want to get the info for the pending endorsement image if it's found
                            Me.btnPolicyInfoNewEndorsement.Enabled = False
                            Me.btnPolicyInfoNewEndorsement.ToolTip = "This policy record already has a pending endorsement"
                        End If
                    End If
                End If

                'added 5/15/2019
                Dim readOnlyViewPageUrl As String = QuickQuoteHelperClass.configAppSettingValueAsString("VR_ReadOnlyViewPageUrl")
                If String.IsNullOrWhiteSpace(readOnlyViewPageUrl) = False Then
                    Me.ReadOnlyButtonSection.Visible = True
                    If isEndorsement = False Then
                        Dim doReadOnlyRedirect As Boolean = False
                        Dim strRedirectToReadOnlyViewPage As String = QuickQuoteHelperClass.configAppSettingValueAsString("VR_RedirectToReadOnlyViewPage")
                        If String.IsNullOrWhiteSpace(strRedirectToReadOnlyViewPage) = False Then
                            If UCase(strRedirectToReadOnlyViewPage) = "YES" OrElse qqHelper.BitToBoolean(strRedirectToReadOnlyViewPage) = True Then 'key would just need any text value that doesn't equate to True to return False
                                doReadOnlyRedirect = True
                            End If
                        End If
                        If doReadOnlyRedirect = True Then
                            Dim polIdToUse As Integer = 0
                            Dim polImgNumToUse As Integer = 0
                            If Me.tblSelectedInfo.Visible = True AndAlso qqHelper.IsPositiveIntegerString(Me.lblPolicyId_selected.Text) = True AndAlso qqHelper.IsPositiveIntegerString(Me.lblPolicyImageNum_selected.Text) = True Then
                                polIdToUse = CInt(Me.lblPolicyId_selected.Text)
                                polImgNumToUse = CInt(Me.lblPolicyImageNum_selected.Text)
                            ElseIf Me.tblCurrentInfo.Visible = True AndAlso qqHelper.IsPositiveIntegerString(Me.lblPolicyId.Text) = True AndAlso qqHelper.IsPositiveIntegerString(Me.lblPolicyImageNum.Text) = True Then
                                polIdToUse = CInt(Me.lblPolicyId.Text)
                                polImgNumToUse = CInt(Me.lblPolicyImageNum.Text)
                            End If
                            If polIdToUse > 0 AndAlso polImgNumToUse > 0 Then
                                Dim appendQuestionMark As Boolean = False
                                Dim appendAmpersand As Boolean = False
                                Dim appendReadOnlyParam As Boolean = False
                                If readOnlyViewPageUrl.Contains("?") = True Then
                                    If UCase(readOnlyViewPageUrl).Contains(UCase("ReadOnlyPolicyIdAndImageNum=")) = False Then
                                        appendAmpersand = True
                                        appendReadOnlyParam = True
                                    End If
                                Else
                                    appendQuestionMark = True
                                    appendReadOnlyParam = True
                                End If
                                If appendQuestionMark = True Then
                                    readOnlyViewPageUrl &= "?"
                                End If
                                If appendAmpersand = True Then
                                    readOnlyViewPageUrl &= "&"
                                End If
                                If appendReadOnlyParam = True Then
                                    readOnlyViewPageUrl &= "ReadOnlyPolicyIdAndImageNum="
                                End If
                                readOnlyViewPageUrl &= polIdToUse.ToString & "|" & polImgNumToUse.ToString
                                Response.Redirect(readOnlyViewPageUrl)
                            End If
                        End If
                    End If
                End If

            Case "Delete" 'added 5/14/2019
                System.Threading.Thread.Sleep(500) 'delay 1/2 second

                Dim polNum As String = ItemCellText(e.Item.Cells(1).Text)
                Dim quoteNum As String = ItemCellText(e.Item.Cells(2).Text)
                Dim polId As String = ItemCellText(e.Item.Cells(3).Text)
                Dim polImgNum As String = ItemCellText(e.Item.Cells(4).Text)
                Dim transType As String = ItemCellText(e.Item.Cells(5).Text)
                Dim polStatusCode As String = ItemCellText(e.Item.Cells(6).Text)
                Dim agCode As String = ItemCellText(e.Item.Cells(7).Text)
                Dim effDate As String = ItemCellText(e.Item.Cells(8).Text)
                Dim expDate As String = ItemCellText(e.Item.Cells(9).Text)
                Dim teffDate As String = ItemCellText(e.Item.Cells(10).Text)
                Dim texpDate As String = ItemCellText(e.Item.Cells(11).Text)
                Dim agId As String = ItemCellText(e.Item.Cells(13).Text) 'note: phName1 is at index 12

                Dim strCurrentEndorsementSuccessfullyRated As String = ""
                If qqHelper.IsPositiveIntegerString(polId) = True AndAlso qqHelper.IsPositiveIntegerString(polImgNum) = True Then
                    strCurrentEndorsementSuccessfullyRated = "EndorsementSuccessfullyRated_" & polId & "_" & polImgNum

                    Dim errorMessage As String = ""
                    Dim success As Boolean = qqXml.SuccessfullyDeletedPendingEndorsementImageInDiamond(CInt(polId), policyImageNum:=CInt(polImgNum), errorMessage:=errorMessage)
                    Dim msgToShow As String = ""
                    If success = True Then
                        If String.IsNullOrWhiteSpace(strCurrentEndorsementSuccessfullyRated) = False AndAlso ViewState.Item(strCurrentEndorsementSuccessfullyRated) IsNot Nothing Then
                            ViewState.Item(strCurrentEndorsementSuccessfullyRated) = Nothing
                        End If

                        msgToShow = "your pending endorsement has now been deleted in Diamond"

                        'now load images for policyId
                        Me.ddlAction.SelectedValue = "Policy Search"
                        ddlAction_SelectedIndexChanged(source, e)
                        Me.ddlPolicySearchBy.SelectedValue = "Policy Id"
                        Me.txtPolicySearchFor.Text = polId
                        Me.ddlPolicySearchByPolicyOrImage.SelectedValue = "By Image"
                        Me.ddlPolicySearchByLobId.SelectedValue = "-1"
                        Me.ddlPolicySearchAgenciesToUse.SelectedValue = "Just my primary and secondary codes" 'other value is "Just my primary agency code"; could check against ddlPolicySearchAgenciesToUse (or possibly ddlEndorsementSearchAgenciesToUse) and try to set to same value unless it's "All Codes (for testing validation)"
                        'If String.IsNullOrWhiteSpace(agCode) = False AndAlso Me.ddlPolicySearchAgencyCodeSelection.Items.FindByText(agCode) IsNot Nothing Then 'won't find anything
                        '    Me.ddlPolicySearchAgencyCodeSelection.SelectedValue = Me.ddlPolicySearchAgencyCodeSelection.Items.FindByText(agCode).Value
                        'End If
                        If qqHelper.IsPositiveIntegerString(agId) = True AndAlso Me.ddlPolicySearchAgencyCodeSelection.Items.FindByValue(agId) IsNot Nothing Then
                            Me.ddlPolicySearchAgencyCodeSelection.SelectedValue = agId
                        End If
                        btnPolicySearch_Click(source, e)
                    Else
                        If String.IsNullOrWhiteSpace(errorMessage) = False Then
                            msgToShow = errorMessage
                        Else
                            msgToShow = "problem deleting your pending endorsement in Diamond"
                        End If
                    End If
                    ShowError(msgToShow)
                Else
                    ShowError("invalid policyId and/or imageNum; please reload results")
                End If

        End Select
    End Sub
    Private Function ItemCellText(ByVal input As String) As String
        Dim output As String = ""

        If String.IsNullOrEmpty(input) = False AndAlso UCase(input) <> "&NBSP;" Then 'need to ignore single space when cell is empty
            output = input
        End If

        Return output
    End Function

    Private Sub btnPolicySearch_Click(sender As Object, e As EventArgs) Handles btnPolicySearch.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        'Me.pnlSearchResults.Visible = False
        'Me.pnlPolicyInfo.Visible = False
        'Me.tblSelectedInfo.Visible = False
        'Me.tblCurrentInfo.Visible = False
        'Me.pnlNewEndorsement.Visible = False
        'Me.btnPolicyInfoNewEndorsement.Visible = False
        'Me.pnlExistingEndorsement.Visible = False
        'Me.btnExistingEndorsementSave.Enabled = False
        'Me.btnExistingEndorsementRate.Enabled = False
        'Me.btnExistingEndorsementDelete.Enabled = False
        'updated 11/6/2016 to use new common method
        PrepareForSearch()

        If Me.txtPolicySearchFor.Text = "" Then
            ShowError("please enter search text")
        Else
            'Dim policyLookupInfo As QuickQuotePolicyLookupInfo = Nothing
            'Select Case UCase(Me.ddlPolicySearchBy.SelectedValue)
            '    Case "POLICY NUMBER"
            '        policyLookupInfo = New QuickQuotePolicyLookupInfo
            '        With policyLookupInfo
            '            .PolicyNumber = Me.txtPolicySearchFor.Text
            '            Select Case UCase(Me.ddlPolicySearchAgenciesToUse.SelectedValue)
            '                Case UCase("Just my primary agency code")
            '                    .AgencyId = qqHelper.IntegerForString(QuickQuoteHelperClass.DiamondAgencyId)
            '                Case UCase("Just my primary and secondary codes")
            '                    .AgencyIds = QuickQuoteHelperClass.DiamondAgencyIds
            '            End Select
            '            Select Case UCase(Me.ddlPolicySearchByPolicyOrImage.SelectedValue)
            '                Case "BY IMAGE"
            '                    .PolicyLookupType = QuickQuotePolicyLookupInfo.LookupType.ByImage
            '                Case Else
            '                    .PolicyLookupType = QuickQuotePolicyLookupInfo.LookupType.ByPolicy
            '            End Select
            '        End With
            '    Case "QUOTE NUMBER"
            '        policyLookupInfo = New QuickQuotePolicyLookupInfo
            '        With policyLookupInfo
            '            .QuoteNumber = Me.txtPolicySearchFor.Text
            '            Select Case UCase(Me.ddlPolicySearchAgenciesToUse.SelectedValue)
            '                Case UCase("Just my primary agency code")
            '                    .AgencyId = qqHelper.IntegerForString(QuickQuoteHelperClass.DiamondAgencyId)
            '                Case UCase("Just my primary and secondary codes")
            '                    .AgencyIds = QuickQuoteHelperClass.DiamondAgencyIds
            '            End Select
            '            Select Case UCase(Me.ddlPolicySearchByPolicyOrImage.SelectedValue)
            '                Case "BY IMAGE"
            '                    .PolicyLookupType = QuickQuotePolicyLookupInfo.LookupType.ByImage
            '                Case Else
            '                    .PolicyLookupType = QuickQuotePolicyLookupInfo.LookupType.ByPolicy
            '            End Select
            '        End With
            '    Case "POLICY ID"
            '        policyLookupInfo = New QuickQuotePolicyLookupInfo
            '        With policyLookupInfo
            '            .PolicyId = qqHelper.IntegerForString(Me.txtPolicySearchFor.Text)
            '            Select Case UCase(Me.ddlPolicySearchAgenciesToUse.SelectedValue)
            '                Case UCase("Just my primary agency code")
            '                    .AgencyId = qqHelper.IntegerForString(QuickQuoteHelperClass.DiamondAgencyId)
            '                Case UCase("Just my primary and secondary codes")
            '                    .AgencyIds = QuickQuoteHelperClass.DiamondAgencyIds
            '            End Select
            '            Select Case UCase(Me.ddlPolicySearchByPolicyOrImage.SelectedValue)
            '                Case "BY IMAGE"
            '                    .PolicyLookupType = QuickQuotePolicyLookupInfo.LookupType.ByImage
            '                Case Else
            '                    .PolicyLookupType = QuickQuotePolicyLookupInfo.LookupType.ByPolicy
            '            End Select
            '        End With

            'End Select
            'updated 11/5/2016
            Dim policyLookupInfo As New QuickQuotePolicyLookupInfo
            With policyLookupInfo
                Select Case UCase(Me.ddlPolicySearchBy.SelectedValue)
                    Case "POLICY NUMBER"
                        .PolicyNumber = Me.txtPolicySearchFor.Text
                    Case "QUOTE NUMBER"
                        .QuoteNumber = Me.txtPolicySearchFor.Text
                    Case "POLICY ID"
                        .PolicyId = qqHelper.IntegerForString(Me.txtPolicySearchFor.Text)

                        'added Name functionality 11/5/2016
                    Case UCase("Display Name (Exact Match)"), UCase("Display Name (Match Beginning)"), UCase("Display Name (Match End)"), UCase("Display Name (Match Middle)")
                        .Policyholder1NameToFind = Me.txtPolicySearchFor.Text
                        .Policyholder1NameLookupField = QuickQuotePolicyLookupInfo.NameLookupField.DisplayName
                        Select Case UCase(Me.ddlPolicySearchBy.SelectedValue)
                            Case UCase("Display Name (Match Beginning)")
                                .Policyholder1NameLookupMatchType = QuickQuotePolicyLookupInfo.NameLookupMatchType.MatchBeginning
                            Case UCase("Display Name (Match End)")
                                .Policyholder1NameLookupMatchType = QuickQuotePolicyLookupInfo.NameLookupMatchType.MatchEnd
                            Case UCase("Display Name (Match Middle)")
                                .Policyholder1NameLookupMatchType = QuickQuotePolicyLookupInfo.NameLookupMatchType.MatchMiddle
                            Case Else
                                .Policyholder1NameLookupMatchType = QuickQuotePolicyLookupInfo.NameLookupMatchType.ExactMatch
                        End Select
                    Case UCase("Last Name (Exact Match)"), UCase("Last Name (Match Beginning)"), UCase("Last Name (Match End)"), UCase("Last Name (Match Middle)")
                        .Policyholder1NameToFind = Me.txtPolicySearchFor.Text
                        .Policyholder1NameLookupField = QuickQuotePolicyLookupInfo.NameLookupField.LastName
                        Select Case UCase(Me.ddlPolicySearchBy.SelectedValue)
                            Case UCase("Last Name (Match Beginning)")
                                .Policyholder1NameLookupMatchType = QuickQuotePolicyLookupInfo.NameLookupMatchType.MatchBeginning
                            Case UCase("Last Name (Match End)")
                                .Policyholder1NameLookupMatchType = QuickQuotePolicyLookupInfo.NameLookupMatchType.MatchEnd
                            Case UCase("Last Name (Match Middle)")
                                .Policyholder1NameLookupMatchType = QuickQuotePolicyLookupInfo.NameLookupMatchType.MatchMiddle
                            Case Else
                                .Policyholder1NameLookupMatchType = QuickQuotePolicyLookupInfo.NameLookupMatchType.ExactMatch
                        End Select
                    Case UCase("Commercial Name 1 (Exact Match)"), UCase("Commercial Name 1 (Match Beginning)"), UCase("Commercial Name 1 (Match End)"), UCase("Commercial Name 1 (Match Middle)")
                        .Policyholder1NameToFind = Me.txtPolicySearchFor.Text
                        .Policyholder1NameLookupField = QuickQuotePolicyLookupInfo.NameLookupField.CommercialName1
                        Select Case UCase(Me.ddlPolicySearchBy.SelectedValue)
                            Case UCase("Commercial Name 1 (Match Beginning)")
                                .Policyholder1NameLookupMatchType = QuickQuotePolicyLookupInfo.NameLookupMatchType.MatchBeginning
                            Case UCase("Commercial Name 1 (Match End)")
                                .Policyholder1NameLookupMatchType = QuickQuotePolicyLookupInfo.NameLookupMatchType.MatchEnd
                            Case UCase("Commercial Name 1 (Match Middle)")
                                .Policyholder1NameLookupMatchType = QuickQuotePolicyLookupInfo.NameLookupMatchType.MatchMiddle
                            Case Else
                                .Policyholder1NameLookupMatchType = QuickQuotePolicyLookupInfo.NameLookupMatchType.ExactMatch
                        End Select
                End Select
                If Me.PolicySearchAgencyCodeSelectionRow.Visible = True Then 'added 11/11/2016; original logic in ELSE
                    If qqHelper.IsPositiveIntegerString(Me.ddlPolicySearchAgencyCodeSelection.SelectedValue) = True Then
                        .AgencyId = qqHelper.IntegerForString(Me.ddlPolicySearchAgencyCodeSelection.SelectedValue)
                    Else
                        .AgencyIds = QuickQuoteHelperClass.DiamondAgencyIds
                    End If
                Else
                    Select Case UCase(Me.ddlPolicySearchAgenciesToUse.SelectedValue)
                        Case UCase("Just my primary agency code")
                            .AgencyId = qqHelper.IntegerForString(QuickQuoteHelperClass.DiamondAgencyId)
                        Case UCase("Just my primary and secondary codes")
                            .AgencyIds = QuickQuoteHelperClass.DiamondAgencyIds
                    End Select
                End If
                Select Case UCase(Me.ddlPolicySearchByPolicyOrImage.SelectedValue)
                    Case "BY IMAGE"
                        .PolicyLookupType = QuickQuotePolicyLookupInfo.LookupType.ByImage
                    Case Else
                        .PolicyLookupType = QuickQuotePolicyLookupInfo.LookupType.ByPolicy
                End Select
                If qqHelper.IsPositiveIntegerString(Me.ddlPolicySearchByLobId.SelectedValue) = True Then 'added 11/5/2016
                    .LobId = CInt(Me.ddlPolicySearchByLobId.SelectedValue)
                End If
                .ForcePolicyholder1NameReturn = Me.cbPolicySearchReturnPH1Name.Checked 'added 11/5/2016
            End With

            'If policyLookupInfo IsNot Nothing AndAlso policyLookupInfo.HasAnyDistinguishableInfo = True Then 'updated 11/5/2016 to check HasAnyDistinguishableInfo to make sure search will be attempted... so I can go ahead and instantiate policyLookupInfo
            '    Dim policyResults As List(Of QuickQuotePolicyLookupInfo) = Nothing
            '    Dim caughtDatabaseError As Boolean = False
            '    policyResults = QuickQuoteHelperClass.PolicyResultsForLookupInfo(policyLookupInfo, caughtDatabaseError:=caughtDatabaseError)
            '    If policyResults IsNot Nothing AndAlso policyResults.Count > 0 Then
            '        Me.pnlSearchResults.Visible = True
            '        Dim dt As New DataTable
            '        Dim sort As String = ""
            '        dt.Columns.Add("polNum", System.Type.GetType("System.String"))
            '        dt.Columns.Add("quoteNum", System.Type.GetType("System.String"))
            '        dt.Columns.Add("polId", System.Type.GetType("System.Int32"))
            '        dt.Columns.Add("polImgNum", System.Type.GetType("System.Int32"))
            '        dt.Columns.Add("transType", System.Type.GetType("System.String"))
            '        dt.Columns.Add("polStatusCode", System.Type.GetType("System.String"))
            '        dt.Columns.Add("agCode", System.Type.GetType("System.String"))
            '        dt.Columns.Add("effDate", System.Type.GetType("System.DateTime"))
            '        dt.Columns.Add("expDate", System.Type.GetType("System.DateTime"))
            '        dt.Columns.Add("teffDate", System.Type.GetType("System.DateTime"))
            '        dt.Columns.Add("texpDate", System.Type.GetType("System.DateTime"))
            '        dt.Columns.Add("ph1Name", System.Type.GetType("System.DateTime"))
            '        dt.Columns.Add("ph1SortName", System.Type.GetType("System.DateTime"))

            '        For Each polResult As QuickQuotePolicyLookupInfo In policyResults
            '            Dim newRow As DataRow = dt.NewRow
            '            newRow.Item("polNum") = polResult.PolicyNumber
            '            newRow.Item("quoteNum") = polResult.QuoteNumber
            '            newRow.Item("polId") = polResult.PolicyId
            '            newRow.Item("polImgNum") = polResult.PolicyImageNum
            '            Select Case polResult.TransTypeId
            '                Case 2
            '                    newRow.Item("transType") = "New Business"
            '                Case 3
            '                    newRow.Item("transType") = "Endorsement"
            '                Case Else
            '                    newRow.Item("transType") = "TransTypeId " & polResult.TransTypeId.ToString
            '            End Select
            '            Select Case polResult.PolicyStatusCodeId
            '                Case 1
            '                    newRow.Item("polStatusCode") = "In-Force"
            '                Case 2
            '                    newRow.Item("polStatusCode") = "Future"
            '                Case 3
            '                    newRow.Item("polStatusCode") = "History"
            '                Case 4
            '                    newRow.Item("polStatusCode") = "Pending"
            '                Case Else
            '                    newRow.Item("polStatusCode") = "PolicyStatusCodeId " & polResult.PolicyStatusCodeId.ToString
            '            End Select
            '            newRow.Item("agCode") = polResult.AgencyCode
            '            If qqHelper.IsDateString(polResult.EffectiveDate) = True Then
            '                newRow.Item("effDate") = CDate(polResult.EffectiveDate)
            '            Else
            '                newRow.Item("effDate") = Nothing
            '            End If
            '            If qqHelper.IsDateString(polResult.ExpirationDate) = True Then
            '                newRow.Item("expDate") = CDate(polResult.ExpirationDate)
            '            Else
            '                newRow.Item("expDate") = Nothing
            '            End If
            '            If qqHelper.IsDateString(polResult.TransactionEffectiveDate) = True Then
            '                newRow.Item("teffDate") = CDate(polResult.TransactionEffectiveDate)
            '            Else
            '                newRow.Item("teffDate") = Nothing
            '            End If
            '            If qqHelper.IsDateString(polResult.TransactionExpirationDate) = True Then
            '                newRow.Item("texpDate") = CDate(polResult.TransactionExpirationDate)
            '            Else
            '                newRow.Item("texpDate") = Nothing
            '            End If
            '            newRow.Item("ph1Name") = polResult.Policyholder1Name 'added 11/5/2016
            '            newRow.Item("ph1SortName") = polResult.Policyholder1SortName 'added 11/5/2016
            '            dt.Rows.Add(newRow)
            '        Next

            '        sort = "polNum, polId, polImgNum"
            '        dt.DefaultView.Sort = sort
            '        If ViewState.Item("dt") Is Nothing Then
            '            ViewState.Add("dt", dt)
            '            ViewState.Add("sort", sort)
            '        Else
            '            ViewState.Item("dt") = dt
            '            ViewState.Item("sort") = sort
            '        End If

            '        Me.dgrdSearchResults.DataSource = dt
            '        Me.dgrdSearchResults.CurrentPageIndex = 0
            '        Me.dgrdSearchResults.SelectedIndex = -1
            '        Me.dgrdSearchResults.DataBind()

            '        Me.lblData.Text = "Policy Results (count = " & dt.Rows.Count & ")"
            '        Me.lblPage.Text = "Page " & Me.dgrdSearchResults.CurrentPageIndex + 1 & " of " & Me.dgrdSearchResults.PageCount

            '    Else
            '        If caughtDatabaseError = True Then
            '            ShowError("database error encountered during search")
            '        Else
            '            ShowError("no results found")
            '        End If
            '    End If
            'Else
            '    ShowError("invalid policy search")
            'End If
            'updated 11/5/2016 to use new method
            ProcessPolicyLookupInfoAndDisplayResults(policyLookupInfo, policyOrEndorsementSearch:=SearchType.Policy)
        End If
    End Sub

    Private Sub btnNewEndorsementCreate_Click(sender As Object, e As EventArgs) Handles btnNewEndorsementCreate.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        '11/6/2016 note: can now use new labels for info below instead of checking pnlPolicyInfo and tblCurrentInfo/tblSelectedInfo
        If Me.pnlPolicyInfo.Visible = True Then
            Dim polIdToUse As Integer = 0
            If Me.tblCurrentInfo.Visible = True AndAlso qqHelper.IsPositiveIntegerString(Me.lblPolicyId.Text) = True Then
                polIdToUse = CInt(Me.lblPolicyId.Text)
            ElseIf Me.tblSelectedInfo.Visible = True AndAlso qqHelper.IsPositiveIntegerString(Me.lblPolicyId_selected.Text) = True Then
                polIdToUse = CInt(Me.lblPolicyId_selected.Text)
            End If
            If polIdToUse > 0 Then
                If qqHelper.IsDateString(Me.txtNewEndorsementTranEffDate.Text) = True Then
                    If Me.txtNewEndorsementRemarks.Text <> "" Then
                        Dim qqe As QuickQuoteObject = Nothing
                        Dim newPolicyImageNum As Integer = 0
                        Dim latestPendingEndorsementImageNum As Integer = 0
                        Dim latestPendingEndorsementImageTranEffDate As String = ""
                        Dim errorMessage As String = ""
                        qqe = qqXml.NewQuickQuoteEndorsementForPolicyIdAndTransactionDate(polIdToUse, Me.txtNewEndorsementTranEffDate.Text, endorsementRemarks:=Me.txtNewEndorsementRemarks.Text, newPolicyImageNum:=newPolicyImageNum, latestPendingEndorsementImageNum:=latestPendingEndorsementImageNum, latestPendingEndorsementImageTranEffDate:=latestPendingEndorsementImageTranEffDate, validateTransactionDate:=True, daysBack:=15, daysForward:=15, returnExistingPendingQuickQuoteEndorsement:=False, onlyReturnPendingQuickQuoteEndorsementWhenDateMatches:=True, errorMessage:=errorMessage)
                        If newPolicyImageNum > 0 Then
                            ShowError("a new Endorsement image has now been created for policyId " & polIdToUse.ToString & "; new image # = " & newPolicyImageNum.ToString)
                            'now perform Endorsement lookup for policyId and load Existing Endorsement screen

                            'added 11/8/2016
                            Me.ddlAction.SelectedValue = "Endorsement Search"
                            ddlAction_SelectedIndexChanged(sender, e)
                            Me.ddlEndorsementSearchBy.SelectedValue = "Policy Id"
                            Me.txtEndorsementSearchFor.Text = polIdToUse.ToString
                            Me.ddlEndorsementSearchByPolicyOrImage.SelectedValue = "By Image"
                            Me.ddlEndorsementSearchByLobId.SelectedValue = "-1"
                            Me.ddlEndorsementSearchAgenciesToUse.SelectedValue = "Just my primary and secondary codes" 'other value is "Just my primary agency code"; could check against ddlPolicySearchAgenciesToUse and try to set to same value unless it's "All Codes (for testing validation)"
                            If qqHelper.IsPositiveIntegerString(qqe.AgencyId) = True AndAlso Me.ddlEndorsementSearchAgencyCodeSelection.Items.FindByValue(qqe.AgencyId) IsNot Nothing Then 'added 11/11/2016
                                Me.ddlEndorsementSearchAgencyCodeSelection.SelectedValue = qqe.AgencyId
                            End If
                            btnEndorsementSearch_Click(sender, e)
                            SelectFirstRecordInGrid() 'added 11/11/2016

                        Else
                            If String.IsNullOrWhiteSpace(errorMessage) = False Then
                                ShowError(errorMessage)
                            Else
                                ShowError("problem creating new endorsement")
                            End If
                            'If latestPendingEndorsementImageNum > 0 OrElse qqHelper.IsDateString(latestPendingEndorsementImageTranEffDate) = True Then

                            'End If
                        End If
                    Else
                        ShowError("please enter Endorsement remarks")
                    End If
                Else
                    ShowError("Invalid Transaction Effective Date")
                End If
            Else
                ShowError("unable to obtain policyId for policy info")
            End If
        Else
            ShowError("no policy info currently being shown; please go back and find a policy")
        End If

    End Sub

    Private Sub btnPolicyInfoNewEndorsement_Click(sender As Object, e As EventArgs) Handles btnPolicyInfoNewEndorsement.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        Me.pnlNewEndorsement.Visible = True

        'added 11/6/2016
        If Me.pnlPolicyInfo.Visible = True Then
            If Me.tblCurrentInfo.Visible = True AndAlso qqHelper.IsPositiveIntegerString(Me.lblPolicyId.Text) = True Then
                Me.lblNewEndorsementPolicyNum.Text = Me.lblPolicyNumber.Text
                Me.lblNewEndorsementPolicyId.Text = Me.lblPolicyId.Text
            ElseIf Me.tblSelectedInfo.Visible = True AndAlso qqHelper.IsPositiveIntegerString(Me.lblPolicyId_selected.Text) = True Then
                Me.lblNewEndorsementPolicyNum.Text = Me.lblPolicyNumber_selected.Text
                Me.lblNewEndorsementPolicyId.Text = Me.lblPolicyId_selected.Text
            End If
        End If

        'added 5/14/2019
        Dim startNewEndorsementPageUrl As String = QuickQuoteHelperClass.configAppSettingValueAsString("VR_StartNewEndorsementPageUrl")
        If String.IsNullOrWhiteSpace(startNewEndorsementPageUrl) = False Then
            Dim polIdToUse As Integer = 0
            Dim polImgNumToUse As Integer = 0
            If Me.tblCurrentInfo.Visible = True AndAlso qqHelper.IsPositiveIntegerString(Me.lblPolicyId.Text) = True AndAlso qqHelper.IsPositiveIntegerString(Me.lblPolicyImageNum.Text) = True Then
                polIdToUse = CInt(Me.lblPolicyId.Text)
                polImgNumToUse = CInt(Me.lblPolicyImageNum.Text)
            ElseIf Me.tblSelectedInfo.Visible = True AndAlso qqHelper.IsPositiveIntegerString(Me.lblPolicyId_selected.Text) = True AndAlso qqHelper.IsPositiveIntegerString(Me.lblPolicyImageNum_selected.Text) = True Then
                polIdToUse = CInt(Me.lblPolicyId_selected.Text)
                polImgNumToUse = CInt(Me.lblPolicyImageNum_selected.Text)
            End If
            If polIdToUse > 0 AndAlso polImgNumToUse > 0 Then
                Dim appendQuestionMark As Boolean = False
                Dim appendAmpersand As Boolean = False
                Dim appendReadOnlyParam As Boolean = False
                If startNewEndorsementPageUrl.Contains("?") = True Then
                    If UCase(startNewEndorsementPageUrl).Contains(UCase("ReadOnlyPolicyIdAndImageNum=")) = False Then
                        appendAmpersand = True
                        appendReadOnlyParam = True
                    End If
                Else
                    appendQuestionMark = True
                    appendReadOnlyParam = True
                End If
                If appendQuestionMark = True Then
                    startNewEndorsementPageUrl &= "?"
                End If
                If appendAmpersand = True Then
                    startNewEndorsementPageUrl &= "&"
                End If
                If appendReadOnlyParam = True Then
                    startNewEndorsementPageUrl &= "ReadOnlyPolicyIdAndImageNum="
                End If
                startNewEndorsementPageUrl &= polIdToUse.ToString & "|" & polImgNumToUse.ToString
                Response.Redirect(startNewEndorsementPageUrl)
            End If
        End If

    End Sub

    Private Sub btnExistingEndorsementSave_Click(sender As Object, e As EventArgs) Handles btnExistingEndorsementSave.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        'added 11/11/2016
        Dim strCurrentEndorsementSuccessfullyRated As String = ""
        If qqHelper.IsPositiveIntegerString(Me.lblExistingEndorsementPolicyId.Text) = True AndAlso qqHelper.IsPositiveIntegerString(Me.lblExistingEndorsementPolicyImageNum.Text) = True Then
            strCurrentEndorsementSuccessfullyRated = "EndorsementSuccessfullyRated_" & Me.lblExistingEndorsementPolicyId.Text & "_" & Me.lblExistingEndorsementPolicyImageNum.Text
        End If

        Dim validationErrorMessage As String = ""
        If String.IsNullOrWhiteSpace(Me.txtExistingEndorsementRemarks.Text) = True Then
            validationErrorMessage = "please enter Endorsement remarks"
        End If
        Dim updateTeffDate As Boolean = False
        If Me.txtExistingEndorsementTranEffDate.Visible = True AndAlso QuickQuoteHelperClass.Endorsements_AllowTransactionEffectiveDateChange() = True Then
            updateTeffDate = True
            If qqHelper.IsValidDateString(Me.txtExistingEndorsementTranEffDate.Text, mustBeGreaterThanDefaultDate:=True) = False Then
                validationErrorMessage = qqHelper.appendText(validationErrorMessage, "please enter a valid transaction effective date", splitter:="; ")
            End If
            If String.IsNullOrWhiteSpace(validationErrorMessage) = True Then
                If qqHelper.IsDateString(Me.txtExistingEndorsementTranEffDate.Text) = True AndAlso qqHelper.IsDateString(Me.lblExistingEndorsementTranEffDate.Text) = True AndAlso CDate(Me.txtExistingEndorsementTranEffDate.Text) <> CDate(Me.lblExistingEndorsementTranEffDate.Text) AndAlso qqHelper.IsPositiveIntegerString(Me.lblExistingEndorsementPolicyId.Text) = True AndAlso qqHelper.IsPositiveIntegerString(Me.lblExistingEndorsementPolicyImageNum.Text) = True Then
                    If qqXml.Diamond_IsNewTransactionEffectiveDateOkay(qqHelper.IntegerForString(Me.lblExistingEndorsementPolicyId.Text), qqHelper.IntegerForString(Me.lblExistingEndorsementPolicyImageNum.Text), Me.txtExistingEndorsementTranEffDate.Text) = False Then
                        validationErrorMessage = "change effective date must stay within current policy term and version; reverted back to " & Me.lblExistingEndorsementTranEffDate.Text
                        Me.txtExistingEndorsementTranEffDate.Text = Me.lblExistingEndorsementTranEffDate.Text
                    End If
                End If
            End If
        End If

        'If Me.txtExistingEndorsementRemarks.Text <> "" Then
        If String.IsNullOrWhiteSpace(validationErrorMessage) = True Then
            Dim qqE As QuickQuoteObject = Nothing
            If ViewState.Item("QuickQuoteEndorsement") IsNot Nothing Then
                qqE = DirectCast(ViewState.Item("QuickQuoteEndorsement"), QuickQuoteObject)
            End If
            If qqE IsNot Nothing Then
                qqE.TransactionRemark = Me.txtExistingEndorsementRemarks.Text
                qqE.TransactionReasonId = Me.ddlExistingEndorsementTransReason.SelectedValue
                If updateTeffDate = True Then
                    qqE.TransactionEffectiveDate = Me.txtExistingEndorsementTranEffDate.Text
                End If

                'added 11/9/2016
                If Me.PPA_HasTestDriverAndVehicleRow.Visible = True Then
                    Dim alreadyHadTestDriverAndVehicle As Boolean = qqHelper.BitToBoolean(Me.lblExistingEndorsementHasTestDriverAndVehicle.Text)
                    If Me.cbExistingEndorsementUseTestDriverAndVehicle.Checked = True Then
                        If alreadyHadTestDriverAndVehicle = True Then
                            'maintain
                        Else
                            'need to add
                            Dim added As Boolean = False
                            AddTestDriverAndVehicle(qqE, added:=added)
                        End If
                    Else
                        If alreadyHadTestDriverAndVehicle = True Then
                            'need to remove
                            Dim removed As Boolean = False
                            RemoveTestDriverAndVehicle(qqE, removed:=removed)
                        Else
                            'never had it
                        End If
                    End If

                    'new test 9/12/2019
                    'If qqHelper.IntegerForString(qqE.PolicyId) = 1519 AndAlso qqHelper.IntegerForString(qqE.PolicyImageNum) = 42 Then 'PPA1000637
                    '    If qqE.Vehicles IsNot Nothing AndAlso qqE.Vehicles.Count > 0 AndAlso qqE.Vehicles(qqE.Vehicles.Count - 1) IsNot Nothing Then
                    '        With qqE.Vehicles(qqE.Vehicles.Count - 1)
                    '            If .Make = "JEEP" AndAlso .Model = "WRANGLER" AndAlso .Year = "2015" Then
                    '                .Make = ""
                    '                .Model = ""
                    '                .Year = ""
                    '            End If
                    '        End With
                    '    End If
                    'End If
                Else 'added 7/30/2019 for testing HOM
                    'If qqHelper.IntegerForString(qqE.PolicyId) = 1540328 AndAlso qqHelper.IntegerForString(qqE.PolicyImageNum) = 4 Then 'HOM2122616
                    '    If qqE.Locations IsNot Nothing AndAlso qqE.Locations.Count > 0 AndAlso qqE.Locations(0) IsNot Nothing Then
                    '        Dim canineSectionCovs As List(Of QuickQuoteSectionIICoverage) = qqHelper.QuickQuoteSectionIICoveragesForType(qqE.Locations(0).SectionIICoverages, QuickQuoteSectionIICoverage.SectionIICoverageType.CanineLiabilityExclusion)
                    '        If canineSectionCovs IsNot Nothing AndAlso canineSectionCovs.Count > 0 Then
                    '            If canineSectionCovs.Count = 1 Then
                    '                Dim newCanineSectionCov As New QuickQuoteSectionIICoverage
                    '                With newCanineSectionCov
                    '                    .HOM_CoverageType = QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.CanineLiabilityExclusion
                    '                End With
                    '                qqE.Locations(0).SectionIICoverages.Add(newCanineSectionCov)
                    '            ElseIf canineSectionCovs.Count = 2 Then
                    '                qqE.Locations(0).SectionIICoverages.Remove(canineSectionCovs(1))
                    '            End If
                    '        End If
                    '    End If
                    'End If

                    'added 2/4/2020 for testing HOM IM
                    'If qqHelper.IntegerForString(qqE.PolicyId) = 93755 AndAlso qqHelper.IntegerForString(qqE.PolicyImageNum) = 12 Then 'HOM2026263 (Patch; previously NewBuildTest)
                    '    If qqE.Locations IsNot Nothing AndAlso qqE.Locations.Count > 0 AndAlso qqE.Locations(0) IsNot Nothing Then
                    '        Dim jewelryIM As QuickQuoteInlandMarine = qqHelper.QuickQuoteInlandMarineForType(qqE.Locations(0).InlandMarines, QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Jewelry)
                    '        'If jewelryIM Is Nothing Then
                    '        '    jewelryIM = New QuickQuoteInlandMarine
                    '        '    jewelryIM.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Jewelry

                    '        '    If qqE.Locations(0).InlandMarines Is Nothing Then
                    '        '        qqE.Locations(0).InlandMarines = New List(Of QuickQuoteInlandMarine)
                    '        '    End If
                    '        '    qqE.Locations(0).InlandMarines.Add(jewelryIM)
                    '        'End If
                    '        'note: QuickQuoteInlandMarineForType has optional param to add item if not already there
                    '        If jewelryIM IsNot Nothing Then
                    '            If qqHelper.IsPositiveDecimalString(jewelryIM.IncreasedLimit) = False Then
                    '                jewelryIM.IncreasedLimit = "1.00"
                    '            End If
                    '        End If
                    '    End If
                    'End If
                    'If qqHelper.IntegerForString(qqE.PolicyId) = 93755 AndAlso qqHelper.IntegerForString(qqE.PolicyImageNum) = 12 Then 'HOM2026263 (Patch)
                    '    If qqE.Locations IsNot Nothing AndAlso qqE.Locations.Count > 0 AndAlso qqE.Locations(0) IsNot Nothing Then
                    '        Dim jewelryIM As QuickQuoteInlandMarine = qqHelper.QuickQuoteInlandMarineForType(qqE.Locations(0).InlandMarines, QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Jewelry, returnNewIfNothing:=False)
                    '        If jewelryIM IsNot Nothing Then
                    '            'verify limit and description; delete for re-run
                    '            qqHelper.RemoveQuickQuoteInlandMarinesForType(qqE.Locations(0).InlandMarines, QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Jewelry)
                    '        Else
                    '            jewelryIM = New QuickQuoteInlandMarine
                    '            jewelryIM.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Jewelry
                    '            If qqE.Locations(0).InlandMarines Is Nothing Then
                    '                qqE.Locations(0).InlandMarines = New List(Of QuickQuoteInlandMarine)
                    '            End If
                    '            qqE.Locations(0).InlandMarines.Add(jewelryIM)
                    '        End If
                    '    End If
                    'End If

                    'added 3/20/2020 for testing HOM RvWatercraft Operator assignment
                    'If qqHelper.IntegerForString(qqE.PolicyId) = 581326 AndAlso qqHelper.IntegerForString(qqE.PolicyImageNum) = 12 Then 'HOM2081773 (Patch)
                    '    If qqE.Locations IsNot Nothing AndAlso qqE.Locations.Count > 0 AndAlso qqE.Locations(0) IsNot Nothing AndAlso qqE.Locations(0).RvWatercrafts IsNot Nothing AndAlso qqE.Locations(0).RvWatercrafts.Count > 0 AndAlso qqE.Locations(0).RvWatercrafts(0) IsNot Nothing Then
                    '        If qqE.Operators IsNot Nothing AndAlso qqE.Operators.Count > 0 AndAlso (qqE.Locations(0).RvWatercrafts(0).AssignedOperatorNums Is Nothing OrElse qqE.Locations(0).RvWatercrafts(0).AssignedOperatorNums.Count = 0) Then
                    '            QuickQuoteHelperClass.AddIntegerToIntegerList(qqE.Operators.Count, qqE.Locations(0).RvWatercrafts(0).AssignedOperatorNums)
                    '        End If
                    '    End If
                    'End If

                    'added 12/8/2020 for testing CAP
                    'If qqHelper.IntegerForString(qqE.PolicyId) = 2017669 AndAlso qqHelper.IntegerForString(qqE.PolicyImageNum) = 2 Then 'CAP1011454 (NewBuildTest)
                    '    'qqE.TransactionReasonId = "10002" 'Endorsement
                    '    qqE.TransactionReasonId = "10169" 'Endorsement Change Dec and Full Revised Dec
                    '    'qqE.TransactionReasonId = "10168" 'Endorsement Change Dec Only
                    'End If
                End If

                Dim successfullySaved As Boolean = False
                Dim replacedObjectPassedIn As Boolean = False
                Dim saveErrorMessage As String = ""
                Dim qqeResults As QuickQuoteObject = Nothing

                successfullySaved = qqXml.SuccessfullySavedQuickQuoteEndorsementInDiamond_ReplaceObjectPassedIn(qqE, qqeResults:=qqeResults, replacedObjectPassedIn:=replacedObjectPassedIn, errorMessage:=saveErrorMessage)
                If replacedObjectPassedIn = True Then
                    ViewState.Item("QuickQuoteEndorsement") = qqE
                End If
                Dim msgToShow As String = "" 'added 11/6/2016
                If successfullySaved = True Then
                    'added 11/11/2016
                    If String.IsNullOrWhiteSpace(strCurrentEndorsementSuccessfullyRated) = False AndAlso ViewState.Item(strCurrentEndorsementSuccessfullyRated) IsNot Nothing Then
                        ViewState.Item(strCurrentEndorsementSuccessfullyRated) = Nothing
                    End If

                    'ShowError("your endorsement changes have been saved in Diamond")
                    'updated 11/6/2016 to use msgToShow
                    msgToShow = "your endorsement changes have been saved in Diamond"

                    'added 11/8/2016
                    msgToShow &= "; the updated image will now be reloaded"
                    LoadExistingEndorsement(qqE.PolicyId, qqE.PolicyImageNum) 'could also use Me.lblExistingEndorsementPolicyId.Text and Me.lblExistingEndorsementPolicyImageNum.Text
                    'note: could also just reload ExistingEndorsement panel w/ latest from qqE (if replacedObjectPassedIn = True)
                Else
                    If String.IsNullOrWhiteSpace(saveErrorMessage) = False Then
                        'ShowError(saveErrorMessage)
                        'updated 11/6/2016 to use msgToShow
                        msgToShow = saveErrorMessage
                    Else
                        'ShowError("problem saving your endorsement changes in Diamond")
                        'note: could use qqeResults and check for validation items
                        'updated 11/6/2016 to use msgToShow
                        msgToShow = "problem saving your endorsement changes in Diamond"
                    End If
                End If
                'updated 11/6/2016 to use msgToShow and use valItems
                If qqeResults IsNot Nothing Then
                    Dim valItems As String = QuickQuoteValidationItemsAsString(qqeResults)
                    msgToShow = qqHelper.appendText(msgToShow, valItems, "<br /><br />")
                End If
                ShowError(msgToShow)
            Else
                ShowError("unable to load QuickQuoteEndorsement from ViewState")
            End If
        Else
            'ShowError("please enter Endorsement remarks")
            ShowError(validationErrorMessage)
        End If
    End Sub

    Private Sub btnExistingEndorsementRate_Click(sender As Object, e As EventArgs) Handles btnExistingEndorsementRate.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        'added 11/11/2016
        Dim strCurrentEndorsementSuccessfullyRated As String = ""
        If qqHelper.IsPositiveIntegerString(Me.lblExistingEndorsementPolicyId.Text) = True AndAlso qqHelper.IsPositiveIntegerString(Me.lblExistingEndorsementPolicyImageNum.Text) = True Then
            strCurrentEndorsementSuccessfullyRated = "EndorsementSuccessfullyRated_" & Me.lblExistingEndorsementPolicyId.Text & "_" & Me.lblExistingEndorsementPolicyImageNum.Text
        End If

        Dim validationErrorMessage As String = ""
        If String.IsNullOrWhiteSpace(Me.txtExistingEndorsementRemarks.Text) = True Then
            validationErrorMessage = "please enter Endorsement remarks"
        End If
        Dim updateTeffDate As Boolean = False
        If Me.txtExistingEndorsementTranEffDate.Visible = True AndAlso QuickQuoteHelperClass.Endorsements_AllowTransactionEffectiveDateChange() = True Then
            updateTeffDate = True
            If qqHelper.IsValidDateString(Me.txtExistingEndorsementTranEffDate.Text, mustBeGreaterThanDefaultDate:=True) = False Then
                validationErrorMessage = qqHelper.appendText(validationErrorMessage, "please enter a valid transaction effective date", splitter:="; ")
            End If
            If String.IsNullOrWhiteSpace(validationErrorMessage) = True Then
                If qqHelper.IsDateString(Me.txtExistingEndorsementTranEffDate.Text) = True AndAlso qqHelper.IsDateString(Me.lblExistingEndorsementTranEffDate.Text) = True AndAlso CDate(Me.txtExistingEndorsementTranEffDate.Text) <> CDate(Me.lblExistingEndorsementTranEffDate.Text) AndAlso qqHelper.IsPositiveIntegerString(Me.lblExistingEndorsementPolicyId.Text) = True AndAlso qqHelper.IsPositiveIntegerString(Me.lblExistingEndorsementPolicyImageNum.Text) = True Then
                    If qqXml.Diamond_IsNewTransactionEffectiveDateOkay(qqHelper.IntegerForString(Me.lblExistingEndorsementPolicyId.Text), qqHelper.IntegerForString(Me.lblExistingEndorsementPolicyImageNum.Text), Me.txtExistingEndorsementTranEffDate.Text) = False Then
                        validationErrorMessage = "change effective date must stay within current policy term and version; reverted back to " & Me.lblExistingEndorsementTranEffDate.Text
                        Me.txtExistingEndorsementTranEffDate.Text = Me.lblExistingEndorsementTranEffDate.Text
                    End If
                End If
            End If
        End If

        'If Me.txtExistingEndorsementRemarks.Text <> "" Then
        If String.IsNullOrWhiteSpace(validationErrorMessage) = True Then
            Dim qqE As QuickQuoteObject = Nothing
            If ViewState.Item("QuickQuoteEndorsement") IsNot Nothing Then
                qqE = DirectCast(ViewState.Item("QuickQuoteEndorsement"), QuickQuoteObject)
            End If
            If qqE IsNot Nothing Then
                qqE.TransactionRemark = Me.txtExistingEndorsementRemarks.Text
                qqE.TransactionReasonId = Me.ddlExistingEndorsementTransReason.SelectedValue
                If updateTeffDate = True Then
                    qqE.TransactionEffectiveDate = Me.txtExistingEndorsementTranEffDate.Text
                End If

                'added 11/9/2016
                If Me.PPA_HasTestDriverAndVehicleRow.Visible = True Then
                    Dim alreadyHadTestDriverAndVehicle As Boolean = qqHelper.BitToBoolean(Me.lblExistingEndorsementHasTestDriverAndVehicle.Text)
                    If Me.cbExistingEndorsementUseTestDriverAndVehicle.Checked = True Then
                        If alreadyHadTestDriverAndVehicle = True Then
                            'maintain
                        Else
                            'need to add
                            Dim added As Boolean = False
                            AddTestDriverAndVehicle(qqE, added:=added)
                        End If
                    Else
                        If alreadyHadTestDriverAndVehicle = True Then
                            'need to remove
                            Dim removed As Boolean = False
                            RemoveTestDriverAndVehicle(qqE, removed:=removed)
                        Else
                            'never had it
                        End If
                    End If
                Else 'added 7/30/2019 for testing HOM
                    'If qqHelper.IntegerForString(qqE.PolicyId) = 1540328 AndAlso qqHelper.IntegerForString(qqE.PolicyImageNum) = 4 Then 'HOM2122616
                    '    If qqE.Locations IsNot Nothing AndAlso qqE.Locations.Count > 0 AndAlso qqE.Locations(0) IsNot Nothing Then
                    '        Dim canineSectionCovs As List(Of QuickQuoteSectionIICoverage) = qqHelper.QuickQuoteSectionIICoveragesForType(qqE.Locations(0).SectionIICoverages, QuickQuoteSectionIICoverage.SectionIICoverageType.CanineLiabilityExclusion)
                    '        If canineSectionCovs IsNot Nothing AndAlso canineSectionCovs.Count > 0 Then
                    '            If canineSectionCovs.Count = 1 Then
                    '                Dim newCanineSectionCov As New QuickQuoteSectionIICoverage
                    '                With newCanineSectionCov
                    '                    .HOM_CoverageType = QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.CanineLiabilityExclusion
                    '                End With
                    '                qqE.Locations(0).SectionIICoverages.Add(newCanineSectionCov)
                    '            ElseIf canineSectionCovs.Count = 2 Then
                    '                qqE.Locations(0).SectionIICoverages.Remove(canineSectionCovs(1))
                    '            End If
                    '        End If
                    '    End If

                    'End If

                    'added 3/20/2020 for testing HOM RvWatercraft Operator assignment
                    'If qqHelper.IntegerForString(qqE.PolicyId) = 581326 AndAlso qqHelper.IntegerForString(qqE.PolicyImageNum) = 12 Then 'HOM2081773 (Patch)
                    '    If qqE.Locations IsNot Nothing AndAlso qqE.Locations.Count > 0 AndAlso qqE.Locations(0) IsNot Nothing AndAlso qqE.Locations(0).RvWatercrafts IsNot Nothing AndAlso qqE.Locations(0).RvWatercrafts.Count > 0 AndAlso qqE.Locations(0).RvWatercrafts(0) IsNot Nothing Then
                    '        If qqE.Operators IsNot Nothing AndAlso qqE.Operators.Count > 0 AndAlso (qqE.Locations(0).RvWatercrafts(0).AssignedOperatorNums Is Nothing OrElse qqE.Locations(0).RvWatercrafts(0).AssignedOperatorNums.Count = 0) Then
                    '            QuickQuoteHelperClass.AddIntegerToIntegerList(qqE.Operators.Count, qqE.Locations(0).RvWatercrafts(0).AssignedOperatorNums)
                    '        End If
                    '    End If
                    'End If
                End If

                Dim successfullySavedAndRated As Boolean = False
                Dim successfullySaved As Boolean = False
                Dim successfullyRated As Boolean = False
                Dim replacedObjectPassedIn As Boolean = False
                Dim saveRateErrorMessage As String = ""
                Dim qqeResults As QuickQuoteObject = Nothing

                'this method calls Diamond's SaveRate service
                successfullySavedAndRated = qqXml.SuccessfullySavedAndRatedQuickQuoteEndorsementInDiamond_ReplaceObjectPassedIn(qqE, qqeResults:=qqeResults, saveSuccessful:=successfullySaved, rateSuccessful:=successfullyRated, replacedObjectPassedIn:=replacedObjectPassedIn, errorMessage:=saveRateErrorMessage)

                'this method calls Diamond's Save service 1st and then the Rate service
                'successfullySavedAndRated = qqXml.SuccessfullySavedAndRatedQuickQuoteEndorsementInDiamondWithSeparateServiceCalls_ReplaceObjectPassedIn(qqE, qqeResults:=qqeResults, saveSuccessful:=successfullySaved, rateSuccessful:=successfullyRated, replacedObjectPassedIn:=replacedObjectPassedIn, errorMessage:=saveRateErrorMessage)

                If replacedObjectPassedIn = True Then
                    ViewState.Item("QuickQuoteEndorsement") = qqE
                End If
                Dim msgToShow As String = "" 'added 11/6/2016
                If successfullySavedAndRated = True Then
                    'added 11/11/2016
                    If String.IsNullOrWhiteSpace(strCurrentEndorsementSuccessfullyRated) = False Then
                        If ViewState.Item(strCurrentEndorsementSuccessfullyRated) IsNot Nothing Then
                            ViewState.Item(strCurrentEndorsementSuccessfullyRated) = "True"
                        Else
                            ViewState.Add(strCurrentEndorsementSuccessfullyRated, "True")
                        End If
                    End If

                    Dim successMessage As String = "your endorsement changes have been saved/rated in Diamond"
                    'If qqeResults IsNot Nothing AndAlso qqHelper.IsPositiveDecimalString(qqeResults.TotalQuotedPremium) = True Then
                    'updated 11/10/2016 to use new prem properties
                    If qqeResults IsNot Nothing AndAlso qqHelper.IsPositiveDecimalString(qqeResults.FullTermPremium) = True Then
                        'successMessage &= "; your new premium is " & qqeResults.TotalQuotedPremium
                        'updated 11/10/2016 to use new premium properties; TotalQuotedPremium was for NewBusiness Quotes... uses FullTermPremium
                        successMessage &= "; Full Term Premium = " & qqeResults.FullTermPremium & "; Change in Full Term Premium = " & qqeResults.ChangeInFullTermPremium & "; Written Premium = " & qqeResults.WrittenPremium & "; Change in Written Premium = " & qqeResults.ChangeInWrittenPremium

                        'test logic to see if ViewState will maintain ReadOnly props... CloneObject function updated to handle some ReadOnly props on QuickQuoteObject (QuoteTransactionType and premiums)
                        'note: this appears to work fine... ReadOnly props are still present
                        'Dim testQQE As QuickQuoteObject = Nothing
                        'If ViewState.Item("TestQQE") Is Nothing Then
                        '    ViewState.Add("TestQQE", qqeResults)
                        'Else
                        '    ViewState.Item("TestQQE") = qqeResults
                        'End If
                        'If ViewState.Item("TestQQE") IsNot Nothing Then
                        '    'qqeResults = CType(ViewState.Item("TestQQE"), QuickQuoteObject)
                        '    'qqeResults = DirectCast(ViewState.Item("TestQQE"), QuickQuoteObject)
                        '    'testQQE = CType(ViewState.Item("TestQQE"), QuickQuoteObject)
                        '    testQQE = DirectCast(ViewState.Item("TestQQE"), QuickQuoteObject)
                        'End If
                    End If
                    'ShowError(successMessage)
                    'updated 11/6/2016 to use msgToShow
                    msgToShow = successMessage
                Else
                    'added 11/11/2016
                    If successfullySaved = True AndAlso String.IsNullOrWhiteSpace(strCurrentEndorsementSuccessfullyRated) = False AndAlso ViewState.Item(strCurrentEndorsementSuccessfullyRated) IsNot Nothing Then
                        ViewState.Item(strCurrentEndorsementSuccessfullyRated) = Nothing
                    End If

                    If String.IsNullOrWhiteSpace(saveRateErrorMessage) = False Then
                        'ShowError(saveRateErrorMessage)
                        'updated 11/6/2016 to use msgToShow
                        msgToShow = saveRateErrorMessage
                    Else
                        If successfullySaved = True AndAlso successfullyRated = False Then
                            'ShowError("your endorsement changes were successfully saved, but rating failed")
                            'updated 11/6/2016 to use msgToShow
                            msgToShow = "your endorsement changes were successfully saved, but rating failed"
                        Else
                            'ShowError("problem saving/rating your endorsement changes in Diamond")
                            'note: could use qqeResults and check for validation items
                            'updated 11/6/2016 to use msgToShow
                            msgToShow = "problem saving/rating your endorsement changes in Diamond"
                        End If
                    End If
                End If

                'added 11/8/2016
                If successfullySavedAndRated = True OrElse successfullySaved = True Then
                    msgToShow &= "; the updated image will now be reloaded"
                    LoadExistingEndorsement(qqE.PolicyId, qqE.PolicyImageNum) 'could also use Me.lblExistingEndorsementPolicyId.Text and Me.lblExistingEndorsementPolicyImageNum.Text
                    'note: could also just reload ExistingEndorsement panel w/ latest from qqE (if replacedObjectPassedIn = True)
                End If

                'updated 11/6/2016 to use msgToShow and use valItems
                If qqeResults IsNot Nothing Then
                    Dim valItems As String = QuickQuoteValidationItemsAsString(qqeResults)
                    msgToShow = qqHelper.appendText(msgToShow, valItems, "<br /><br />")
                End If
                ShowError(msgToShow)
            Else
                ShowError("unable to load QuickQuoteEndorsement from ViewState")
            End If
        Else
            'ShowError("please enter Endorsement remarks")
            ShowError(validationErrorMessage)
        End If
    End Sub

    Private Sub btnEndorsementSearch_Click(sender As Object, e As EventArgs) Handles btnEndorsementSearch.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        'Me.pnlSearchResults.Visible = False
        'Me.pnlPolicyInfo.Visible = False
        'Me.tblSelectedInfo.Visible = False
        'Me.tblCurrentInfo.Visible = False
        'Me.pnlNewEndorsement.Visible = False
        'Me.btnPolicyInfoNewEndorsement.Visible = False
        'Me.pnlExistingEndorsement.Visible = False
        'Me.btnExistingEndorsementSave.Enabled = False
        'Me.btnExistingEndorsementRate.Enabled = False
        'Me.btnExistingEndorsementDelete.Enabled = False
        'updated 11/6/2016 to use new common method
        PrepareForSearch()

        If Me.txtEndorsementSearchFor.Text = "" AndAlso String.IsNullOrWhiteSpace(Me.ddlEndorsementSearchBy.SelectedValue) = False Then
            ShowError("please enter search text")
        Else
            Dim policyLookupInfo As New QuickQuotePolicyLookupInfo
            With policyLookupInfo
                .TransTypeId = 3 'Endorsement
                If QuickQuoteHelperClass.ConsiderEndorsementQuoteStatusAsPending() = True Then 'added IF 10/10/2019; original logic in ELSE
                    .PolicyStatusCodeIds = QuickQuoteHelperClass.PendingDiamondPolicyStatusCodeIds(considerQuoteStatusAsPending:=True)
                Else
                    '.PolicyStatusCodeId = 4 'Pending
                    .PolicyStatusCodeId = CInt(QuickQuotePolicyLookupInfo.DiamondPolicyStatusCode.Pending)
                End If
                Select Case UCase(Me.ddlEndorsementSearchBy.SelectedValue)
                    Case "POLICY NUMBER"
                        .PolicyNumber = Me.txtEndorsementSearchFor.Text
                    Case "QUOTE NUMBER"
                        .QuoteNumber = Me.txtEndorsementSearchFor.Text
                    Case "POLICY ID"
                        .PolicyId = qqHelper.IntegerForString(Me.txtEndorsementSearchFor.Text)

                        'added Name functionality 11/5/2016
                    Case UCase("Display Name (Exact Match)"), UCase("Display Name (Match Beginning)"), UCase("Display Name (Match End)"), UCase("Display Name (Match Middle)")
                        .Policyholder1NameToFind = Me.txtEndorsementSearchFor.Text
                        .Policyholder1NameLookupField = QuickQuotePolicyLookupInfo.NameLookupField.DisplayName
                        Select Case UCase(Me.ddlEndorsementSearchBy.SelectedValue)
                            Case UCase("Display Name (Match Beginning)")
                                .Policyholder1NameLookupMatchType = QuickQuotePolicyLookupInfo.NameLookupMatchType.MatchBeginning
                            Case UCase("Display Name (Match End)")
                                .Policyholder1NameLookupMatchType = QuickQuotePolicyLookupInfo.NameLookupMatchType.MatchEnd
                            Case UCase("Display Name (Match Middle)")
                                .Policyholder1NameLookupMatchType = QuickQuotePolicyLookupInfo.NameLookupMatchType.MatchMiddle
                            Case Else
                                .Policyholder1NameLookupMatchType = QuickQuotePolicyLookupInfo.NameLookupMatchType.ExactMatch
                        End Select
                    Case UCase("Last Name (Exact Match)"), UCase("Last Name (Match Beginning)"), UCase("Last Name (Match End)"), UCase("Last Name (Match Middle)")
                        .Policyholder1NameToFind = Me.txtEndorsementSearchFor.Text
                        .Policyholder1NameLookupField = QuickQuotePolicyLookupInfo.NameLookupField.LastName
                        Select Case UCase(Me.ddlEndorsementSearchBy.SelectedValue)
                            Case UCase("Last Name (Match Beginning)")
                                .Policyholder1NameLookupMatchType = QuickQuotePolicyLookupInfo.NameLookupMatchType.MatchBeginning
                            Case UCase("Last Name (Match End)")
                                .Policyholder1NameLookupMatchType = QuickQuotePolicyLookupInfo.NameLookupMatchType.MatchEnd
                            Case UCase("Last Name (Match Middle)")
                                .Policyholder1NameLookupMatchType = QuickQuotePolicyLookupInfo.NameLookupMatchType.MatchMiddle
                            Case Else
                                .Policyholder1NameLookupMatchType = QuickQuotePolicyLookupInfo.NameLookupMatchType.ExactMatch
                        End Select
                    Case UCase("Commercial Name 1 (Exact Match)"), UCase("Commercial Name 1 (Match Beginning)"), UCase("Commercial Name 1 (Match End)"), UCase("Commercial Name 1 (Match Middle)")
                        .Policyholder1NameToFind = Me.txtEndorsementSearchFor.Text
                        .Policyholder1NameLookupField = QuickQuotePolicyLookupInfo.NameLookupField.CommercialName1
                        Select Case UCase(Me.ddlEndorsementSearchBy.SelectedValue)
                            Case UCase("Commercial Name 1 (Match Beginning)")
                                .Policyholder1NameLookupMatchType = QuickQuotePolicyLookupInfo.NameLookupMatchType.MatchBeginning
                            Case UCase("Commercial Name 1 (Match End)")
                                .Policyholder1NameLookupMatchType = QuickQuotePolicyLookupInfo.NameLookupMatchType.MatchEnd
                            Case UCase("Commercial Name 1 (Match Middle)")
                                .Policyholder1NameLookupMatchType = QuickQuotePolicyLookupInfo.NameLookupMatchType.MatchMiddle
                            Case Else
                                .Policyholder1NameLookupMatchType = QuickQuotePolicyLookupInfo.NameLookupMatchType.ExactMatch
                        End Select
                End Select
                If Me.EndorsementSearchAgencyCodeSelectionRow.Visible = True Then 'added 11/11/2016; original logic in ELSE
                    If qqHelper.IsPositiveIntegerString(Me.ddlEndorsementSearchAgencyCodeSelection.SelectedValue) = True Then
                        .AgencyId = qqHelper.IntegerForString(Me.ddlEndorsementSearchAgencyCodeSelection.SelectedValue)
                    Else
                        .AgencyIds = QuickQuoteHelperClass.DiamondAgencyIds
                    End If
                Else
                    Select Case UCase(Me.ddlEndorsementSearchAgenciesToUse.SelectedValue)
                        Case UCase("Just my primary agency code")
                            .AgencyId = qqHelper.IntegerForString(QuickQuoteHelperClass.DiamondAgencyId)
                        Case UCase("Just my primary and secondary codes")
                            .AgencyIds = QuickQuoteHelperClass.DiamondAgencyIds
                    End Select
                End If
                Select Case UCase(Me.ddlEndorsementSearchByPolicyOrImage.SelectedValue)
                    Case "BY IMAGE"
                        .PolicyLookupType = QuickQuotePolicyLookupInfo.LookupType.ByImage
                    Case Else
                        .PolicyLookupType = QuickQuotePolicyLookupInfo.LookupType.ByPolicy
                End Select
                If qqHelper.IsPositiveIntegerString(Me.ddlEndorsementSearchByLobId.SelectedValue) = True Then 'added 11/5/2016
                    .LobId = CInt(Me.ddlEndorsementSearchByLobId.SelectedValue)
                End If
                .ForcePolicyholder1NameReturn = Me.cbEndorsementSearchReturnPH1Name.Checked 'added 11/5/2016
            End With

            'If policyLookupInfo IsNot Nothing AndAlso policyLookupInfo.HasAnyDistinguishableInfo = True Then 'updated 11/5/2016 to check HasAnyDistinguishableInfo to make sure search will be attempted
            '    Dim policyResults As List(Of QuickQuotePolicyLookupInfo) = Nothing
            '    Dim caughtDatabaseError As Boolean = False
            '    policyResults = QuickQuoteHelperClass.PolicyResultsForLookupInfo(policyLookupInfo, caughtDatabaseError:=caughtDatabaseError)
            '    If policyResults IsNot Nothing AndAlso policyResults.Count > 0 Then
            '        Me.pnlSearchResults.Visible = True
            '        Dim dt As New DataTable
            '        Dim sort As String = ""
            '        dt.Columns.Add("polNum", System.Type.GetType("System.String"))
            '        dt.Columns.Add("quoteNum", System.Type.GetType("System.String"))
            '        dt.Columns.Add("polId", System.Type.GetType("System.Int32"))
            '        dt.Columns.Add("polImgNum", System.Type.GetType("System.Int32"))
            '        dt.Columns.Add("transType", System.Type.GetType("System.String"))
            '        dt.Columns.Add("polStatusCode", System.Type.GetType("System.String"))
            '        dt.Columns.Add("agCode", System.Type.GetType("System.String"))
            '        dt.Columns.Add("effDate", System.Type.GetType("System.DateTime"))
            '        dt.Columns.Add("expDate", System.Type.GetType("System.DateTime"))
            '        dt.Columns.Add("teffDate", System.Type.GetType("System.DateTime"))
            '        dt.Columns.Add("texpDate", System.Type.GetType("System.DateTime"))

            '        For Each polResult As QuickQuotePolicyLookupInfo In policyResults
            '            Dim newRow As DataRow = dt.NewRow
            '            newRow.Item("polNum") = polResult.PolicyNumber
            '            newRow.Item("quoteNum") = polResult.QuoteNumber
            '            newRow.Item("polId") = polResult.PolicyId
            '            newRow.Item("polImgNum") = polResult.PolicyImageNum
            '            Select Case polResult.TransTypeId
            '                Case 2
            '                    newRow.Item("transType") = "New Business"
            '                Case 3
            '                    newRow.Item("transType") = "Endorsement"
            '                Case Else
            '                    newRow.Item("transType") = "TransTypeId " & polResult.TransTypeId.ToString
            '            End Select
            '            Select Case polResult.PolicyStatusCodeId
            '                Case 1
            '                    newRow.Item("polStatusCode") = "In-Force"
            '                Case 2
            '                    newRow.Item("polStatusCode") = "Future"
            '                Case 3
            '                    newRow.Item("polStatusCode") = "History"
            '                Case 4
            '                    newRow.Item("polStatusCode") = "Pending"
            '                Case Else
            '                    newRow.Item("polStatusCode") = "PolicyStatusCodeId " & polResult.PolicyStatusCodeId.ToString
            '            End Select
            '            newRow.Item("agCode") = polResult.AgencyCode
            '            If qqHelper.IsDateString(polResult.EffectiveDate) = True Then
            '                newRow.Item("effDate") = CDate(polResult.EffectiveDate)
            '            Else
            '                newRow.Item("effDate") = Nothing
            '            End If
            '            If qqHelper.IsDateString(polResult.ExpirationDate) = True Then
            '                newRow.Item("expDate") = CDate(polResult.ExpirationDate)
            '            Else
            '                newRow.Item("expDate") = Nothing
            '            End If
            '            If qqHelper.IsDateString(polResult.TransactionEffectiveDate) = True Then
            '                newRow.Item("teffDate") = CDate(polResult.TransactionEffectiveDate)
            '            Else
            '                newRow.Item("teffDate") = Nothing
            '            End If
            '            If qqHelper.IsDateString(polResult.TransactionExpirationDate) = True Then
            '                newRow.Item("texpDate") = CDate(polResult.TransactionExpirationDate)
            '            Else
            '                newRow.Item("texpDate") = Nothing
            '            End If
            '            dt.Rows.Add(newRow)
            '        Next

            '        sort = "polNum, polId, polImgNum"
            '        dt.DefaultView.Sort = sort
            '        If ViewState.Item("dt") Is Nothing Then
            '            ViewState.Add("dt", dt)
            '            ViewState.Add("sort", sort)
            '        Else
            '            ViewState.Item("dt") = dt
            '            ViewState.Item("sort") = sort
            '        End If

            '        Me.dgrdSearchResults.DataSource = dt
            '        Me.dgrdSearchResults.CurrentPageIndex = 0
            '        Me.dgrdSearchResults.SelectedIndex = -1
            '        Me.dgrdSearchResults.DataBind()

            '        Me.lblData.Text = "Policy Results (count = " & dt.Rows.Count & ")"
            '        Me.lblPage.Text = "Page " & Me.dgrdSearchResults.CurrentPageIndex + 1 & " of " & Me.dgrdSearchResults.PageCount

            '    Else
            '        If caughtDatabaseError = True Then
            '            ShowError("database error encountered during search")
            '        Else
            '            ShowError("no results found")
            '        End If
            '    End If
            'Else
            '    ShowError("invalid endorsement search")
            'End If
            'updated 11/5/2016 to use new method
            ProcessPolicyLookupInfoAndDisplayResults(policyLookupInfo, policyOrEndorsementSearch:=SearchType.Endorsement)
        End If
    End Sub

    'added 11/5/2016 to keep logic in one spot
    Enum SearchType
        None = 0
        Policy = 1
        Endorsement = 2
    End Enum
    Private Sub ProcessPolicyLookupInfoAndDisplayResults(ByVal policyLookupInfo As QuickQuotePolicyLookupInfo, Optional ByVal policyOrEndorsementSearch As SearchType = SearchType.Policy)
        If policyLookupInfo IsNot Nothing AndAlso policyLookupInfo.HasAnyDistinguishableInfo = True Then 'updated 11/5/2016 to check HasAnyDistinguishableInfo to make sure search will be attempted... so I can go ahead and instantiate policyLookupInfo
            Dim policyResults As List(Of QuickQuotePolicyLookupInfo) = Nothing
            Dim caughtDatabaseError As Boolean = False
            policyResults = QuickQuoteHelperClass.PolicyResultsForLookupInfo(policyLookupInfo, caughtDatabaseError:=caughtDatabaseError)
            If policyResults IsNot Nothing AndAlso policyResults.Count > 0 Then
                Me.pnlSearchResults.Visible = True
                Dim dt As New DataTable
                Dim sort As String = ""
                dt.Columns.Add("polNum", System.Type.GetType("System.String"))
                dt.Columns.Add("quoteNum", System.Type.GetType("System.String"))
                dt.Columns.Add("polId", System.Type.GetType("System.Int32"))
                dt.Columns.Add("polImgNum", System.Type.GetType("System.Int32"))
                dt.Columns.Add("transType", System.Type.GetType("System.String"))
                dt.Columns.Add("polStatusCode", System.Type.GetType("System.String"))
                dt.Columns.Add("agCode", System.Type.GetType("System.String"))
                dt.Columns.Add("effDate", System.Type.GetType("System.DateTime"))
                dt.Columns.Add("expDate", System.Type.GetType("System.DateTime"))
                dt.Columns.Add("teffDate", System.Type.GetType("System.DateTime"))
                dt.Columns.Add("texpDate", System.Type.GetType("System.DateTime"))
                dt.Columns.Add("ph1Name", System.Type.GetType("System.String"))
                dt.Columns.Add("ph1SortName", System.Type.GetType("System.String"))
                dt.Columns.Add("agId", System.Type.GetType("System.Int32")) 'added 5/14/2019

                'note: this would need to be updated if columns change
                If policyLookupInfo.SetToReturnPolicyholder1Name = True Then
                    Me.dgrdSearchResults.Columns.Item(12).Visible = True
                Else
                    Me.dgrdSearchResults.Columns.Item(12).Visible = False
                End If

                For Each polResult As QuickQuotePolicyLookupInfo In policyResults
                    Dim newRow As DataRow = dt.NewRow
                    newRow.Item("polNum") = polResult.PolicyNumber
                    newRow.Item("quoteNum") = polResult.QuoteNumber
                    newRow.Item("polId") = polResult.PolicyId
                    newRow.Item("polImgNum") = polResult.PolicyImageNum
                    Select Case polResult.TransTypeId
                        Case 2
                            newRow.Item("transType") = "New Business"
                        Case 3
                            newRow.Item("transType") = "Endorsement"
                        Case Else
                            newRow.Item("transType") = "TransTypeId " & polResult.TransTypeId.ToString
                    End Select
                    Select Case polResult.PolicyStatusCodeId
                        Case 1
                            newRow.Item("polStatusCode") = "In-Force"
                        Case 2
                            newRow.Item("polStatusCode") = "Future"
                        Case 3
                            newRow.Item("polStatusCode") = "History"
                        Case 4
                            newRow.Item("polStatusCode") = "Pending"
                        Case 12 'added 10/10/2019
                            newRow.Item("polStatusCode") = "Quote"
                        Case Else
                            newRow.Item("polStatusCode") = "PolicyStatusCodeId " & polResult.PolicyStatusCodeId.ToString
                    End Select
                    newRow.Item("agCode") = polResult.AgencyCode
                    If qqHelper.IsDateString(polResult.EffectiveDate) = True Then
                        newRow.Item("effDate") = CDate(polResult.EffectiveDate)
                    Else
                        newRow.Item("effDate") = Nothing
                    End If
                    If qqHelper.IsDateString(polResult.ExpirationDate) = True Then
                        newRow.Item("expDate") = CDate(polResult.ExpirationDate)
                    Else
                        newRow.Item("expDate") = Nothing
                    End If
                    If qqHelper.IsDateString(polResult.TransactionEffectiveDate) = True Then
                        newRow.Item("teffDate") = CDate(polResult.TransactionEffectiveDate)
                    Else
                        newRow.Item("teffDate") = Nothing
                    End If
                    If qqHelper.IsDateString(polResult.TransactionExpirationDate) = True Then
                        newRow.Item("texpDate") = CDate(polResult.TransactionExpirationDate)
                    Else
                        newRow.Item("texpDate") = Nothing
                    End If
                    newRow.Item("ph1Name") = polResult.Policyholder1Name 'added 11/5/2016
                    newRow.Item("ph1SortName") = polResult.Policyholder1SortName 'added 11/5/2016
                    newRow.Item("agId") = polResult.AgencyId
                    dt.Rows.Add(newRow)
                Next

                sort = "polNum, polId, polImgNum"
                dt.DefaultView.Sort = sort
                'If ViewState.Item("dt") Is Nothing Then
                '    ViewState.Add("dt", dt)
                '    ViewState.Add("sort", sort)
                'Else
                '    ViewState.Item("dt") = dt
                '    ViewState.Item("sort") = sort
                'End If
                'updated 11/11/2016 to handle large collections in session
                If policyResults.Count > 2000 Then
                    If Session.Item("dt") Is Nothing Then
                        Session.Add("dt", dt)
                        Session.Add("sort", sort)
                    Else
                        Session.Item("dt") = dt
                        Session.Item("sort") = sort
                    End If
                    If ViewState.Item("dt") IsNot Nothing Then
                        ViewState.Item("dt") = Nothing
                        ViewState.Item("sort") = Nothing
                    End If
                Else
                    If ViewState.Item("dt") Is Nothing Then
                        ViewState.Add("dt", dt)
                        ViewState.Add("sort", sort)
                    Else
                        ViewState.Item("dt") = dt
                        ViewState.Item("sort") = sort
                    End If
                    If Session.Item("dt") IsNot Nothing Then
                        Session.Item("dt") = Nothing
                        Session.Item("sort") = Nothing
                    End If
                End If

                Me.dgrdSearchResults.DataSource = dt
                Me.dgrdSearchResults.CurrentPageIndex = 0
                Me.dgrdSearchResults.SelectedIndex = -1
                Me.dgrdSearchResults.DataBind()

                Me.lblData.Text = "Policy Results (count = " & dt.Rows.Count & ")"
                Me.lblPage.Text = "Page " & Me.dgrdSearchResults.CurrentPageIndex + 1 & " of " & Me.dgrdSearchResults.PageCount

            Else
                If caughtDatabaseError = True Then
                    ShowError("database error encountered during search")
                Else
                    ShowError("no results found")
                End If
            End If
        Else
            If policyOrEndorsementSearch = SearchType.Endorsement Then
                ShowError("invalid endorsement search")
            Else
                ShowError("invalid policy search")
            End If
        End If
    End Sub
    Private Function QuickQuoteValidationItemsAsString(ByVal qqo As QuickQuoteObject) As String
        Dim strValItems As String = ""

        If qqo IsNot Nothing AndAlso qqo.ValidationItems IsNot Nothing AndAlso qqo.ValidationItems.Count > 0 Then
            strValItems = "Validation Items:"
            Dim valItemCounter As Integer = 0 'added 11/10/2016
            For Each vi As QuickQuoteValidationItem In qqo.ValidationItems
                valItemCounter += 1 'added 11/10/2016
                'strValItems &= "<br />" & vi.ValidationSeverityType & " - " & vi.Message
                'updated 11/10/2016 since it was just showing Integer for Enum; could also use GetName to get text for Enum
                Dim strValItem As String = ""
                Dim valSeverityType As String = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteValidationItem, QuickQuoteHelperClass.QuickQuotePropertyName.ValidationSeverityTypeId, vi.ValidationSeverityTypeId.ToString)
                If String.IsNullOrWhiteSpace(valSeverityType) = False Then
                    strValItem = valSeverityType
                Else
                    strValItem = "# " & valItemCounter.ToString
                End If
                strValItem &= " - " & vi.Message
                strValItems &= "<br />" & strValItem
            Next
        End If

        Return strValItems
    End Function

    Private Sub btnExistingEndorsementDelete_Click(sender As Object, e As EventArgs) Handles btnExistingEndorsementDelete.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        'added 11/11/2016
        Dim strCurrentEndorsementSuccessfullyRated As String = ""
        If qqHelper.IsPositiveIntegerString(Me.lblExistingEndorsementPolicyId.Text) = True AndAlso qqHelper.IsPositiveIntegerString(Me.lblExistingEndorsementPolicyImageNum.Text) = True Then
            strCurrentEndorsementSuccessfullyRated = "EndorsementSuccessfullyRated_" & Me.lblExistingEndorsementPolicyId.Text & "_" & Me.lblExistingEndorsementPolicyImageNum.Text
        End If

        'ShowError("this functionality is not yet available")
        'try Diamond's demote or delete service
        'Diamond.Common.Services.Messages.PolicyService.DeletePendingImage 'this is probably the right one
        'Diamond.Common.Services.Messages.PolicyService.DemotePendingToQuote 'this probably wouldn't make sense
        'Diamond.Common.Services.Messages.PolicyService.DeleteQuote 'probably just for quotes

        'added functionality 11/9/2016
        Dim qqE As QuickQuoteObject = Nothing
        If ViewState.Item("QuickQuoteEndorsement") IsNot Nothing Then
            qqE = DirectCast(ViewState.Item("QuickQuoteEndorsement"), QuickQuoteObject)
        End If
        If qqE IsNot Nothing Then
            Dim errorMessage As String = ""
            Dim success As Boolean = qqXml.SuccessfullyDeletedPendingQuickQuoteEndorsementInDiamond(qqE, errorMessage:=errorMessage)
            Dim msgToShow As String = ""
            If success = True Then
                'added 11/11/2016
                If String.IsNullOrWhiteSpace(strCurrentEndorsementSuccessfullyRated) = False AndAlso ViewState.Item(strCurrentEndorsementSuccessfullyRated) IsNot Nothing Then
                    ViewState.Item(strCurrentEndorsementSuccessfullyRated) = Nothing
                End If

                msgToShow = "your pending endorsement has now been deleted in Diamond"

                'now load images for policyId
                Me.ddlAction.SelectedValue = "Policy Search"
                ddlAction_SelectedIndexChanged(sender, e)
                Me.ddlPolicySearchBy.SelectedValue = "Policy Id"
                Me.txtPolicySearchFor.Text = qqE.PolicyId
                Me.ddlPolicySearchByPolicyOrImage.SelectedValue = "By Image"
                Me.ddlPolicySearchByLobId.SelectedValue = "-1"
                Me.ddlPolicySearchAgenciesToUse.SelectedValue = "Just my primary and secondary codes" 'other value is "Just my primary agency code"; could check against ddlPolicySearchAgenciesToUse (or possibly ddlEndorsementSearchAgenciesToUse) and try to set to same value unless it's "All Codes (for testing validation)"
                If qqHelper.IsPositiveIntegerString(qqE.AgencyId) = True AndAlso Me.ddlPolicySearchAgencyCodeSelection.Items.FindByValue(qqE.AgencyId) IsNot Nothing Then 'added 11/11/2016
                    Me.ddlPolicySearchAgencyCodeSelection.SelectedValue = qqE.AgencyId
                End If
                btnPolicySearch_Click(sender, e)
            Else
                If String.IsNullOrWhiteSpace(errorMessage) = False Then
                    msgToShow = errorMessage
                Else
                    msgToShow = "problem deleting your pending endorsement in Diamond"
                End If
            End If
            ShowError(msgToShow)
        Else
            ShowError("unable to load QuickQuoteEndorsement from ViewState")
        End If

    End Sub

    'added 11/8/2016
    Private Sub LoadExistingEndorsement(ByVal policyId As String, ByVal policyImageNum As String)
        Me.pnlExistingEndorsement.Visible = False
        Me.PPA_HasTestDriverAndVehicleRow.Visible = False 'added 11/9/2016
        Me.ExistingEndorsementIssuanceSection.Visible = False 'added 11/11/2016
        If qqHelper.IsPositiveIntegerString(policyId) = True AndAlso qqHelper.IsPositiveIntegerString(policyImageNum) = True Then
            'added 5/14/2019
            Dim endorsementEditPageUrl As String = QuickQuoteHelperClass.configAppSettingValueAsString("VR_EndorsementEditPageUrl")
            If String.IsNullOrWhiteSpace(endorsementEditPageUrl) = False Then
                Dim appendQuestionMark As Boolean = False
                Dim appendAmpersand As Boolean = False
                Dim appendEndorsementParam As Boolean = False
                If endorsementEditPageUrl.Contains("?") = True Then
                    If UCase(endorsementEditPageUrl).Contains(UCase("EndorsementPolicyIdAndImageNum=")) = False Then
                        appendAmpersand = True
                        appendEndorsementParam = True
                    End If
                Else
                    appendQuestionMark = True
                    appendEndorsementParam = True
                End If
                If appendQuestionMark = True Then
                    endorsementEditPageUrl &= "?"
                End If
                If appendAmpersand = True Then
                    endorsementEditPageUrl &= "&"
                End If
                If appendEndorsementParam = True Then
                    endorsementEditPageUrl &= "EndorsementPolicyIdAndImageNum="
                End If
                endorsementEditPageUrl &= CInt(policyId).ToString & "|" & CInt(policyImageNum).ToString
                Response.Redirect(endorsementEditPageUrl)
            End If

            Me.pnlExistingEndorsement.Visible = True

            'added 11/11/2016
            Dim strCurrentEndorsementSuccessfullyRated As String = "EndorsementSuccessfullyRated_" & CInt(policyId).ToString & "_" & CInt(policyImageNum).ToString

            Dim qqE As QuickQuoteObject = Nothing
            Dim errorMessage As String = ""
            qqE = qqXml.QuickQuoteEndorsementForPolicyIdAndImageNum(CInt(policyId), CInt(policyImageNum), errorMessage:=errorMessage)
            If qqE IsNot Nothing Then
                If ViewState.Item("QuickQuoteEndorsement") IsNot Nothing Then
                    ViewState.Item("QuickQuoteEndorsement") = qqE
                Else
                    ViewState.Add("QuickQuoteEndorsement", qqE)
                End If
                Me.lblExistingEndorsementTranEffDate.Text = qqE.TransactionEffectiveDate
                Me.txtExistingEndorsementTranEffDate.Text = qqE.TransactionEffectiveDate
                If QuickQuoteHelperClass.Endorsements_AllowTransactionEffectiveDateChange() = True Then
                    Me.lblExistingEndorsementTranEffDate.Visible = False
                    Me.txtExistingEndorsementTranEffDate.Visible = True
                Else
                    Me.lblExistingEndorsementTranEffDate.Visible = True
                    Me.txtExistingEndorsementTranEffDate.Visible = False
                End If
                Me.txtExistingEndorsementRemarks.Text = qqE.TransactionRemark 'probably need new QQO property; added 11/4/2016
                If Me.ddlExistingEndorsementTransReason.Items.FindByValue(qqE.TransactionReasonId) IsNot Nothing Then
                    Me.ddlExistingEndorsementTransReason.SelectedValue = qqE.TransactionReasonId
                End If
                'added policy info label 11/6/2016; could use to verify against the selected policy info provided above
                Me.lblExistingEndorsementPolicyNum.Text = qqE.PolicyNumber
                Me.lblExistingEndorsementPolicyId.Text = qqE.PolicyId
                Me.lblExistingEndorsementPolicyImageNum.Text = qqE.PolicyImageNum

                Me.btnExistingEndorsementSave.Enabled = True
                Me.btnExistingEndorsementRate.Enabled = True
                Me.btnExistingEndorsementDelete.Enabled = True

                'added 11/9/2016
                If qqE.LobType = QuickQuoteObject.QuickQuoteLobType.AutoPersonal Then
                    Me.PPA_HasTestDriverAndVehicleRow.Visible = True

                    Me.cbExistingEndorsementUseTestDriverAndVehicle.Checked = HasTestDriverAndVehicle(qqE)
                    Me.lblExistingEndorsementHasTestDriverAndVehicle.Text = Me.cbExistingEndorsementUseTestDriverAndVehicle.Checked.ToString

                End If

                'added 11/11/2016
                If String.IsNullOrWhiteSpace(strCurrentEndorsementSuccessfullyRated) = False AndAlso ViewState.Item(strCurrentEndorsementSuccessfullyRated) IsNot Nothing AndAlso String.IsNullOrWhiteSpace(ViewState.Item(strCurrentEndorsementSuccessfullyRated).ToString) = False Then
                    If qqHelper.BitToBoolean(ViewState.Item(strCurrentEndorsementSuccessfullyRated).ToString) = True Then
                        Me.ExistingEndorsementIssuanceSection.Visible = True
                    End If
                End If
            Else
                Me.btnExistingEndorsementSave.Enabled = False
                Me.btnExistingEndorsementRate.Enabled = False
                Me.btnExistingEndorsementDelete.Enabled = False

                If String.IsNullOrWhiteSpace(errorMessage) = False Then
                    ShowError(errorMessage)
                Else
                    ShowError("problem loading QuickQuoteEndorsement")
                End If
            End If
        Else
            ShowError("unable to obtain policyId and policyImageNum to load Endorsement info")
        End If
    End Sub

    'Private Function HasTestDriverAndVehicle(ByVal qqo As QuickQuoteObject) As Boolean
    'updated 11/9/2016 w/ optional byref params
    Private Function HasTestDriverAndVehicle(ByVal qqo As QuickQuoteObject, Optional ByRef testDriverNum As Integer = 0, Optional ByRef testVehicleNum As Integer = 0, Optional ByRef vehicleIsLinkedToDriver As Boolean = False) As Boolean
        Dim hasIt As Boolean = False
        '11/9/2016 - defaulting new optional byref params
        testDriverNum = 0
        testVehicleNum = 0
        vehicleIsLinkedToDriver = False

        If qqo IsNot Nothing AndAlso qqo.Drivers IsNot Nothing AndAlso qqo.Drivers.Count > 0 AndAlso qqo.Vehicles IsNot Nothing AndAlso qqo.Vehicles.Count > 0 Then
            '1st find driver
            'Dim testDriverNum As Integer = 0 'updated 11/9/2016 to optional byref param
            Dim driverCount As Integer = 0
            For Each d As QuickQuoteDriver In qqo.Drivers
                driverCount += 1
                If d.Name IsNot Nothing Then
                    If UCase(d.Name.FirstName) = "TEST" AndAlso UCase(d.Name.LastName) = "DRIVER" Then
                        testDriverNum = driverCount.ToString
                        Exit For
                    End If
                End If
            Next
            If testDriverNum > 0 Then
                'now find vehicle
                'Dim testVehicleNum As Integer = 0 'updated 11/9/2016 to optional byref param
                Dim vehicleCount As Integer = 0
                'Dim vehicleIsLinkedToDriver As Boolean = False 'updated 11/9/2016 to optional byref param
                For Each v As QuickQuoteVehicle In qqo.Vehicles
                    vehicleCount += 1
                    If UCase(v.Make) = "CHEVY" AndAlso UCase(v.Model) = "CORVETTE" AndAlso v.Year = "1973" Then
                        testVehicleNum = vehicleCount
                        If v.PrincipalDriverNum = testDriverNum.ToString Then
                            vehicleIsLinkedToDriver = True
                        End If
                        Exit For
                    End If
                Next
                If testVehicleNum > 0 Then
                    If vehicleIsLinkedToDriver = True Then
                        hasIt = True
                    End If
                End If
            End If
        End If

        Return hasIt
    End Function
    Private Sub AddTestDriverAndVehicle(ByRef qqo As QuickQuoteObject, Optional ByRef added As Boolean = False)
        added = False

        If qqo IsNot Nothing Then
            If qqo.Drivers Is Nothing Then
                qqo.Drivers = New List(Of QuickQuoteDriver)
            End If
            If qqo.Vehicles Is Nothing Then
                qqo.Vehicles = New List(Of QuickQuoteVehicle)
            End If
            Dim newDriver As New QuickQuoteDriver
            With newDriver
                If .Name Is Nothing Then
                    .Name = New QuickQuoteName
                End If
                With .Name
                    .FirstName = "Test"
                    .LastName = "Driver"
                    .SexId = "1"
                    .TypeId = "1"
                    .TaxNumber = "123456789"
                    .TaxTypeId = "1"
                    .EntityTypeId = "1" 'Individual
                    .BirthDate = "1/1/1982"
                    .DriversLicenseDate = "1/1/1998"
                    .DriversLicenseNumber = "1234567890" 'remove "-"; was 1234-56-7890
                    .DriversLicenseStateId = "16" 'added 4/17/2014; needed for MVR
                    .MaritalStatusId = "1" '1=Single, 2=Married
                End With
                If .Address Is Nothing Then
                    .Address = New QuickQuoteAddress
                End If
                With .Address
                    .HouseNum = "123"
                    .StreetName = "Auto Street"
                    .City = "Indianapolis"
                    .StateId = "16" 'should be default
                    .Zip = "46227"
                    .County = "Marion"
                End With
                .LicenseStatusId = "2" 'Valid
                .DriverExcludeTypeId = "1" 'Rated
                .RelationshipTypeId = "11" 'Not Related to Policyholder
            End With
            qqo.Drivers.Add(newDriver)
            Dim newVehicle As New QuickQuoteVehicle
            With newVehicle
                .Make = "CHEVY"
                .Model = "CORVETTE"
                .Year = "1973"
                .ClassCode = "7398"
                .Vin = "1Z37J354324831111"
                .CostNew = "12000"
                .VehicleRatingTypeId = "1" 'Private Passenger Type
                '.UseCodeTypeId = "20" 'Business (for commercial only)
                .VehicleUseTypeId = "2" 'Business
                .OperatorTypeId = "0"
                .OperatorUseTypeId = "0"
                .RadiusTypeId = "0" 'N/A
                .SecondaryClassTypeId = "0" 'N/A
                .SecondaryClassUsageTypeId = "0" 'N/A
                .SizeTypeId = "0" 'N/A
                .PerformanceTypeId = "2" 'High
                .BodyTypeId = "14" 'Car
                .AntiLockTypeId = "2" 'All-Wheel Anti-Lock Brakes
                .RegisteredStateId = "16" 'IN
                .RestraintTypeId = "1" 'Passive Seat Belts
                .AntiTheftTypeId = "1" 'Alarm Only
                .VehicleValueId = "2" 'Used
                .GrossVehicleWeight = "2650"
                .VehicleTypeId = "" 'for motorcycles
                .OdometerReading = "87569"
                .AnnualMiles = "8300"
                .MilesOneWay = "34"
                .DaysPerWeek = "5"
                .ActualCashValue = "10800" 'problem field... int in database; had to remove money formatting from property
                .StatedAmount = "10500"
                .PurchasedDate = "6/1/1988"
                .CubicCentimeters = "350"
                .CustomEquipmentAmount = "13500"
                .MultiCar = True
                .DriverOutOfStateSurcharge = False
                .DamageYesNoId = "2" 'No
                .DamageRemarks = ""
                .NonOwnedNamed = False 'Named Non-Owned Non-Specific Vehicle
                .NonOwned = False 'Extended Non-Owned

                'new PPA props
                .Liability_UM_UIM_LimitId = "0" 'N/A; 100,000=10
                .MedicalPaymentsLimitId = "0" 'N/A; 1,000=170
                .ComprehensiveDeductibleLimitId = "18" '100
                .CollisionDeductibleLimitId = "20" '200
                .TowingAndLaborDeductibleLimitId = "41" '50; 25=27
                .UninsuredMotoristLiabilityLimitId = "0" 'N/A; 100/300=4
                .BodilyInjuryLiabilityLimitId = "0" 'N/A; 100/200=135
                .PropertyDamageLimitId = "0" 'N/A; 25,000=8
                .UninsuredCombinedSingleLimitId = "0" 'N/A; 100,000=10
                .UninsuredMotoristPropertyDamageLimitId = "0" 'N/A; 25,000=8
                .UninsuredMotoristPropertyDamageDeductibleLimitId = "0" 'N/A; 300=155
                .HasPollutionLiabilityBroadenedCoverage = False
                .TransportationExpenseLimitId = "30" '20/600; 30/900=80
                .HasAutoLoanOrLease = False
                .TapesAndRecordsLimitId = "219" '400; 200=212; 4/15/2014 note: cannot be higher than 200 (in some cases) unless sound equipment is at least 1500
                .SoundEquipmentLimit = "2000" 'changed 4/1/2014 for testing (from 50) since rating doesn't start until after $1000
                .ElectronicEquipmentLimit = "75"
                .TripInterruptionLimitId = "0" 'N/A; 300=25

                With .GaragingAddress
                    .WithinCity = True
                    .Address = qqHelper.CloneObject(qqo.Drivers(qqo.Drivers.Count - 1).Address)
                    .GaragedInside = True
                End With

                .PrincipalDriverNum = qqo.Drivers.Count 'set to last driver added
            End With
            qqo.Vehicles.Add(newVehicle)
            added = True
        End If
    End Sub
    'added 11/9/2016
    Private Sub RemoveTestDriverAndVehicle(ByRef qqo As QuickQuoteObject, Optional ByRef removed As Boolean = False)
        removed = False

        'could also store driver# and vehicle# in labels when loading ExistingEndorsment panel... they are optional byref params of HasTestDriverAndVehicle
        'will just manually recall HasTestDriverAndVehicle and use driver/vehicle #s from that
        Dim testDriverNum As Integer = 0
        Dim testVehicleNum As Integer = 0
        Dim vehicleIsLinkedToDriver As Boolean = False
        If HasTestDriverAndVehicle(qqo, testDriverNum:=testDriverNum, testVehicleNum:=testVehicleNum, vehicleIsLinkedToDriver:=vehicleIsLinkedToDriver) = True Then
            Dim removedDriver As Boolean = False
            Dim removedVehicle As Boolean = False
            RemoveDriverAndOrVehicle(qqo, driverNumToRemove:=testDriverNum, vehicleNumToRemove:=testVehicleNum, removedDriver:=removedDriver, removedVehicle:=removedVehicle)
            If removedDriver = True AndAlso removedVehicle = True Then
                removed = True
            End If
        End If
    End Sub
    Private Sub RemoveDriverAndOrVehicle(ByRef qqo As QuickQuoteObject, Optional ByVal driverNumToRemove As Integer = 0, Optional ByVal vehicleNumToRemove As Integer = 0, Optional ByRef removedDriver As Boolean = False, Optional ByRef removedVehicle As Boolean = False)
        removedDriver = False
        removedVehicle = False

        If qqo IsNot Nothing AndAlso (driverNumToRemove > 0 OrElse vehicleNumToRemove > 0) Then
            If vehicleNumToRemove > 0 Then
                qqHelper.RemoveQuickQuoteVehicleByIndex(qqo.Vehicles, vehicleNumToRemove - 1, removed:=removedVehicle)
            End If
            If driverNumToRemove > 0 Then
                '1st see if any vehicles are still tied to driver
                'new methods to remove vehicle driver assignments, but needs to be verified and tested (see qqHelper.RemoveQuickQuoteVehicleDriverAssignmentsForDriverNumber and qqHelper.MoveUpVehicleDrivers... MoveUpVehicleDrivers could be called from RemoveQuickQuoteVehicleDriverAssignmentsForDriverNumber)
                'should be able to leave off for now since this is currently only used for TestDriverAndVehicle
                qqHelper.RemoveQuickQuoteDriverByIndex(qqo.Drivers, driverNumToRemove - 1, removed:=removedDriver)
            End If
        End If

    End Sub

    Private Sub dgrdSearchResults_ItemDataBound(sender As Object, e As DataGridItemEventArgs) Handles dgrdSearchResults.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            'Dim btnControl As Control = e.Item.Cells(0).Controls(0)
            'If btnControl IsNot Nothing AndAlso TypeOf btnControl Is Button Then
            '    CType(btnControl, Button).Attributes.Add("onclick", "btnSubmit_Click(this, 'Selecting...');") 'for disable button and server-side logic
            'End If
            Dim btnSelect As Button = e.Item.FindControl("btnSelect")
            If btnSelect IsNot Nothing Then
                btnSelect.Attributes.Add("onclick", "btnSubmit_Click(this, 'Selecting...');") 'for disable button and server-side logic
            End If

            'added 5/14/2019
            Dim DeleteImageSection As HtmlGenericControl = e.Item.FindControl("DeleteImageSection")
            If DeleteImageSection IsNot Nothing Then
                DeleteImageSection.Visible = False
                Select Case UCase(Me.ddlAction.SelectedValue)
                    Case "ENDORSEMENT SEARCH"
                        DeleteImageSection.Visible = True

                    Case "POLICY SEARCH"

                    Case Else

                End Select
                If DeleteImageSection.Visible = True Then
                    Dim btnDelete As Button = DeleteImageSection.FindControl("btnDelete")
                    If btnDelete IsNot Nothing Then
                        'btnDelete.Attributes.Add("onclick", "btnSubmit_Click(this, 'Deleting...');") 'for disable button and server-side logic
                        btnDelete.Attributes.Add("onclick", "return SubmitConfirm(this, 'Are you sure you want to delete this endorsement?', 'Deleting...');") 'for confirm box, disable button, and call server-side logic
                    End If
                End If
            End If
        End If
    End Sub

    'added 11/11/2016
    Private Sub SelectFirstRecordInGrid()
        If Me.dgrdSearchResults.Items IsNot Nothing AndAlso Me.dgrdSearchResults.Items.Count > 0 Then
            For Each dgItem As DataGridItem In Me.dgrdSearchResults.Items
                Dim btnSelect As Button = dgItem.FindControl("btnSelect")
                If btnSelect IsNot Nothing Then
                    Dim cmdEventArgs As New CommandEventArgs("Select", Nothing)
                    Dim dgCmdEventArgs As New DataGridCommandEventArgs(dgItem, btnSelect, cmdEventArgs)
                    dgrdSearchResults_ItemCommand(Me.dgrdSearchResults, dgCmdEventArgs)
                    Exit For
                End If
            Next
        End If
    End Sub

    Private Sub btnExistingEndorsementIssuance_Click(sender As Object, e As EventArgs) Handles btnExistingEndorsementIssuance.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        If String.IsNullOrWhiteSpace(Me.lblExistingEndorsementPolicyNum.Text) = False Then
            'Response.Redirect("VelociRaterAppIssuanceRelay.aspx?polNumToIssue=" & Me.lblExistingEndorsementPolicyNum.Text)
            '11/11/2016 - changed this page name to include "App" and updated code to go directly to MakeAPayment... since it wasn't seeing VelociRaterAppIssuanceRelay as the UrlReferrer since the original Response.Redirect came from here
            'Dim makeAPaymentLink As String = QuickQuoteHelperClass.configAppSettingValueAsString("MakeAPaymentLink")
            'If String.IsNullOrWhiteSpace(makeAPaymentLink) = True Then
            '    Dim agentsLink As String = QuickQuoteHelperClass.configAppSettingValueAsString("AgentsLink")
            '    If String.IsNullOrWhiteSpace(agentsLink) = False AndAlso UCase(agentsLink).Contains("AGENTS.ASPX") AndAlso Len(agentsLink) > 11 Then 'has full url
            '        makeAPaymentLink = UCase(agentsLink).Replace("AGENTS.ASPX", "MakeAPayment.aspx?polNum=")
            '    Else
            '        makeAPaymentLink = "MakeAPayment.aspx?polNum="
            '    End If
            'End If
            'makeAPaymentLink &= Me.lblExistingEndorsementPolicyNum.Text & "&Quote=Yes"
            'Response.Redirect(makeAPaymentLink)
            'updated 10/11/2019
            Dim qqE As QuickQuoteObject = Nothing
            If ViewState.Item("QuickQuoteEndorsement") IsNot Nothing Then
                qqE = DirectCast(ViewState.Item("QuickQuoteEndorsement"), QuickQuoteObject)
            End If
            If qqE IsNot Nothing Then
                Dim finalizeErrorMsg As String = ""
                Dim successfullyFinalized As Boolean = False
                Dim attemptedPromote As Boolean = False
                Dim successfullyPromoted As Boolean = False
                qqXml.FinalizeEndorsement(qqE, onlyFinalizeWhenNoFailureOnPromotion:=True, errorMsg:=finalizeErrorMsg, successfullyFinalized:=successfullyFinalized, attemptedToPromote:=attemptedPromote, successfullyPromoted:=successfullyPromoted)
                If successfullyFinalized = True OrElse attemptedPromote = False OrElse successfullyPromoted = True Then 'we won't worry if it fails to update our table w/ the Finalized status; we just don't want to send the user to MakeAPayment if it needs to be Promoted and that has failed
                    Dim makeAPaymentLink As String = QuickQuoteHelperClass.configAppSettingValueAsString("MakeAPaymentLink")
                    If String.IsNullOrWhiteSpace(makeAPaymentLink) = True Then
                        Dim agentsLink As String = QuickQuoteHelperClass.configAppSettingValueAsString("AgentsLink")
                        If String.IsNullOrWhiteSpace(agentsLink) = False AndAlso UCase(agentsLink).Contains("AGENTS.ASPX") AndAlso Len(agentsLink) > 11 Then 'has full url
                            makeAPaymentLink = UCase(agentsLink).Replace("AGENTS.ASPX", "MakeAPayment.aspx?polNum=")
                        Else
                            makeAPaymentLink = "MakeAPayment.aspx?polNum="
                        End If
                    End If
                    makeAPaymentLink &= Me.lblExistingEndorsementPolicyNum.Text & "&Quote=Yes"
                    makeAPaymentLink &= "&VrQuoteId=" & qqHelper.IntegerForString(qqE.PolicyId) & "|" & qqHelper.IntegerForString(qqE.PolicyImageNum)
                    Response.Redirect(makeAPaymentLink)
                Else
                    Dim friendlyFinalizeErrorMsg As String = ""
                    If attemptedPromote = True AndAlso successfullyPromoted = False Then
                        friendlyFinalizeErrorMsg = "Problem promoting Endorsement Quote to Pending Endorsement"
                    Else
                        friendlyFinalizeErrorMsg = "Problem finalizing Endorsement"
                    End If
                    friendlyFinalizeErrorMsg &= "; please try again later."

                    ShowError(friendlyFinalizeErrorMsg)
                End If
            Else
                ShowError("unable to load QuickQuoteEndorsement from ViewState")
            End If
        Else
            ShowError("Unable to load information needed for issuance.")
        End If
    End Sub

    Private Sub btnViewReadOnly_Click(sender As Object, e As EventArgs) Handles btnViewReadOnly.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        Dim readOnlyViewPageUrl As String = QuickQuoteHelperClass.configAppSettingValueAsString("VR_ReadOnlyViewPageUrl")
        If String.IsNullOrWhiteSpace(readOnlyViewPageUrl) = False Then
            Dim polIdToUse As Integer = 0
            Dim polImgNumToUse As Integer = 0
            If Me.tblSelectedInfo.Visible = True AndAlso qqHelper.IsPositiveIntegerString(Me.lblPolicyId_selected.Text) = True AndAlso qqHelper.IsPositiveIntegerString(Me.lblPolicyImageNum_selected.Text) = True Then
                polIdToUse = CInt(Me.lblPolicyId_selected.Text)
                polImgNumToUse = CInt(Me.lblPolicyImageNum_selected.Text)
            ElseIf Me.tblCurrentInfo.Visible = True AndAlso qqHelper.IsPositiveIntegerString(Me.lblPolicyId.Text) = True AndAlso qqHelper.IsPositiveIntegerString(Me.lblPolicyImageNum.Text) = True Then
                polIdToUse = CInt(Me.lblPolicyId.Text)
                polImgNumToUse = CInt(Me.lblPolicyImageNum.Text)
            End If
            If polIdToUse > 0 AndAlso polImgNumToUse > 0 Then
                Dim appendQuestionMark As Boolean = False
                Dim appendAmpersand As Boolean = False
                Dim appendReadOnlyParam As Boolean = False
                If readOnlyViewPageUrl.Contains("?") = True Then
                    If UCase(readOnlyViewPageUrl).Contains(UCase("ReadOnlyPolicyIdAndImageNum=")) = False Then
                        appendAmpersand = True
                        appendReadOnlyParam = True
                    End If
                Else
                    appendQuestionMark = True
                    appendReadOnlyParam = True
                End If
                If appendQuestionMark = True Then
                    readOnlyViewPageUrl &= "?"
                End If
                If appendAmpersand = True Then
                    readOnlyViewPageUrl &= "&"
                End If
                If appendReadOnlyParam = True Then
                    readOnlyViewPageUrl &= "ReadOnlyPolicyIdAndImageNum="
                End If
                readOnlyViewPageUrl &= polIdToUse.ToString & "|" & polImgNumToUse.ToString
                Response.Redirect(readOnlyViewPageUrl)
            Else
                ShowError("unable to determine policyId and/or policyImageNum")
            End If
        Else
            ShowError("unable to determine ReadOnly URL")
        End If
    End Sub
End Class
