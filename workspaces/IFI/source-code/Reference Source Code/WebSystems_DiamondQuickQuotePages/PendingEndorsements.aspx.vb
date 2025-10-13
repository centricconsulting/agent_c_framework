Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports System.Data
Partial Class PendingEndorsements 'added 7/24/2017
    Inherits System.Web.UI.Page

    Dim qqXml As New QuickQuoteXML
    Dim qqHelper As New QuickQuoteHelperClass

    Private Sub PendingEndorsements_Load(sender As Object, e As EventArgs) Handles Me.Load
        Page.MaintainScrollPositionOnPostBack = True

        If Page.IsPostBack = False Then
            Me.btnEndorsementSearch.Attributes.Add("onclick", "btnSubmit_Click(this, 'Searching...');") 'for disable button and server-side logic

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
            End If
            If agencyCount > 0 Then
                Me.EndorsementSearchAgencyCodeRow.Visible = False
                Me.EndorsementSearchAgencyCodeSelectionRow.Visible = True
            Else
                Me.EndorsementSearchAgencyCodeRow.Visible = True
                Me.EndorsementSearchAgencyCodeSelectionRow.Visible = False
            End If
        End If
    End Sub

    Private Sub PendingEndorsements_PreInit(sender As Object, e As EventArgs) Handles Me.PreInit
        Me.MasterPageFile = ConfigurationManager.AppSettings("DiamondQuickQuoteMaster")
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

    Private Sub btnEndorsementSearch_Click(sender As Object, e As EventArgs) Handles btnEndorsementSearch.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        If Me.txtEndorsementSearchFor.Text = "" AndAlso String.IsNullOrWhiteSpace(Me.ddlEndorsementSearchBy.SelectedValue) = False Then
            ShowError("please enter search text")
        Else
            Dim policyLookupInfo As New QuickQuotePolicyLookupInfo
            With policyLookupInfo
                .TransTypeId = 3 'Endorsement
                .PolicyStatusCodeId = 4 'Pending
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
                .PolicyLookupType = QuickQuotePolicyLookupInfo.LookupType.ByImage
                If qqHelper.IsPositiveIntegerString(Me.ddlEndorsementSearchByLobId.SelectedValue) = True Then 'added 11/5/2016
                    .LobId = CInt(Me.ddlEndorsementSearchByLobId.SelectedValue)
                End If
                .ForcePolicyholder1NameReturn = True
            End With

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

                    Me.lblData.Text = "Endorsement Results (count = " & dt.Rows.Count & ")"
                    Me.lblPage.Text = "Page " & Me.dgrdSearchResults.CurrentPageIndex + 1 & " of " & Me.dgrdSearchResults.PageCount

                Else
                    If caughtDatabaseError = True Then
                        ShowError("database error encountered during search")
                    Else
                        ShowError("no results found")
                    End If
                End If
            Else
                ShowError("invalid endorsement search")
            End If
        End If
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



        End Select
    End Sub
    Private Function ItemCellText(ByVal input As String) As String
        Dim output As String = ""

        If String.IsNullOrEmpty(input) = False AndAlso UCase(input) <> "&NBSP;" Then 'need to ignore single space when cell is empty
            output = input
        End If

        Return output
    End Function
End Class
