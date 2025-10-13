'added 3/24/2017
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods

Partial Class controls_Proposal_VRProposal_Summary
    Inherits System.Web.UI.UserControl

    Dim qqHelper As New QuickQuoteHelperClass
    Dim proposalHelper As New ProposalHelperClass 'added 6/10/2013

    Private _Proposal As QuickQuoteProposalObject
    Public Property Proposal As QuickQuoteProposalObject
        Get
            Return _Proposal
        End Get
        Set(value As QuickQuoteProposalObject)
            _Proposal = value
            LoadSummaryObjects()
        End Set
    End Property

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Page.IsPostBack = False Then
            If ConfigurationManager.AppSettings("QuickQuote_Proposal_HeaderLogo") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_Proposal_HeaderLogo").ToString <> "" Then
                Me.HeaderLogo.Src = ConfigurationManager.AppSettings("QuickQuote_Proposal_HeaderLogo").ToString
            Else
                Me.HeaderLogo.Src = "https://www.indianafarmers.com/agentsonly/images/Icon Row Member Portal.jpg"
            End If
        End If
    End Sub
    Private Sub LoadSummaryObjects()
        If _Proposal IsNot Nothing AndAlso _Proposal.QuickQuoteObjects.Count > 0 Then
            With _Proposal
                Me.VRProposal_ClientAndAgencyInfo1.ClientInfo = qqHelper.appendText(.Client.Name.DisplayNameForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), .Client.Address.DisplayAddressForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), "<br />")
                Me.VRProposal_ClientAndAgencyInfo1.ClientInfo = qqHelper.appendText(Me.VRProposal_ClientAndAgencyInfo1.ClientInfo, .Client.PrimaryPhone, "<br />")
                'Me.VRProposal_ClientAndAgencyInfo1.AgencyInfo = qqHelper.appendText(.Agency.Name.DisplayNameForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), .Agency.Address.DisplayAddressForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), "<br />")
                'updated 6/10/2013 to use DBA name or IRS name if DBA isn't available (instead of using both)
                Me.VRProposal_ClientAndAgencyInfo1.AgencyInfo = qqHelper.appendText(proposalHelper.GetAgencyName(.Agency.Name).Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), .Agency.Address.DisplayAddressForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), "<br />")
                Me.VRProposal_ClientAndAgencyInfo1.AgencyInfo = qqHelper.appendText(Me.VRProposal_ClientAndAgencyInfo1.AgencyInfo, .Agency.PrimaryPhone, "<br />")

                Me.lblCombinedPremium.Text = .CombinedPremium

                'added 6/28/2013
                Dim LinesOnCurrentPage As Integer = 0
                ResetPageLineCount(LinesOnCurrentPage, True)
                Dim pageNumber As Integer = 1
                Dim controlCount As Integer = 0 'current logic will keep track of controls per page so it won't perform break w/o any controls
                Dim qqCounter As Integer = 0
                Dim isLastControl As Boolean = False

                '6/27/2013 note: may add logic to determine where page breaks should be added... could allow 2 LOB summaries on 1st page (since it also has the logo and agency info at top) and 3 on each subsequent page; could also determine based on line counts (Matt's logic assumes page is around 60 lines)
                For Each qq As QuickQuoteObject In .QuickQuoteObjects
                    qqCounter += 1
                    If qqCounter = .QuickQuoteObjects.Count Then
                        isLastControl = True
                    End If
                    Select Case qq.LobType
                        Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                            'AddPageBreakToPlaceholder()

                            Dim BOP_Summary As controls_Proposal_VRProposal_BOP_Summary = LoadControl("VRProposal_BOP_Summary.ascx")
                            BOP_Summary.QuickQuote = qq
                            'added 6/28/2013
                            DetermineIfPageBreakIsNeeded(pageNumber, LinesOnCurrentPage, controlCount, BOP_Summary.LinesInControl, isLastControl)
                            controlCount += 1
                            Me.phControls.Controls.Add(BOP_Summary)
                        Case QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
                            'AddPageBreakToPlaceholder()

                            Dim CGL_Summary As controls_Proposal_VRProposal_CGL_Summary = LoadControl("VRProposal_CGL_Summary.ascx")
                            CGL_Summary.QuickQuote = qq
                            'added 6/28/2013
                            DetermineIfPageBreakIsNeeded(pageNumber, LinesOnCurrentPage, controlCount, CGL_Summary.LinesInControl, isLastControl)
                            controlCount += 1
                            Me.phControls.Controls.Add(CGL_Summary)
                        Case QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                            'AddPageBreakToPlaceholder()

                            Dim WCP_Summary As controls_Proposal_VRProposal_WCP_Summary = LoadControl("VRProposal_WCP_Summary.ascx")
                            WCP_Summary.QuickQuote = qq
                            'added 6/28/2013
                            DetermineIfPageBreakIsNeeded(pageNumber, LinesOnCurrentPage, controlCount, WCP_Summary.LinesInControl, isLastControl)
                            controlCount += 1
                            Me.phControls.Controls.Add(WCP_Summary)
                        Case QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                            'AddPageBreakToPlaceholder()

                            Dim CPR_Summary As controls_Proposal_VRProposal_CPR_Summary = LoadControl("VRProposal_CPR_Summary.ascx")
                            CPR_Summary.QuickQuote = qq
                            'added 6/28/2013
                            DetermineIfPageBreakIsNeeded(pageNumber, LinesOnCurrentPage, controlCount, CPR_Summary.LinesInControl, isLastControl)
                            controlCount += 1
                            Me.phControls.Controls.Add(CPR_Summary)
                        Case QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                            'AddPageBreakToPlaceholder()

                            Dim CPP_Summary As controls_Proposal_VRProposal_CPP_Summary = LoadControl("VRProposal_CPP_Summary.ascx")
                            CPP_Summary.QuickQuote = qq
                            'added 6/28/2013
                            'DetermineIfPageBreakIsNeeded(pageNumber, LinesOnCurrentPage, controlCount, CPP_Summary.LinesInControl, isLastControl)
                            'updated 8/20/2015 to use new method specific to CPP
                            DetermineIfPageBreaksAreNeededForCPP(CPP_Summary, pageNumber, LinesOnCurrentPage, controlCount, CPP_Summary.LinesInControl, isLastControl)
                            controlCount += 1
                            Me.phControls.Controls.Add(CPP_Summary)
                        Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                            'AddPageBreakToPlaceholder()

                            Dim CAP_Summary As controls_Proposal_VRProposal_CAP_Summary = LoadControl("VRProposal_CAP_Summary.ascx")
                            CAP_Summary.QuickQuote = qq
                            'added 6/28/2013
                            DetermineIfPageBreakIsNeeded(pageNumber, LinesOnCurrentPage, controlCount, CAP_Summary.LinesInControl, isLastControl)
                            controlCount += 1
                            Me.phControls.Controls.Add(CAP_Summary)
                        Case QuickQuoteObject.QuickQuoteLobType.CommercialGarage 'added 4/22/2017
                            'AddPageBreakToPlaceholder()

                            Dim GAR_Summary As controls_Proposal_VRProposal_GAR_Summary = LoadControl("VRProposal_GAR_Summary.ascx")
                            GAR_Summary.QuickQuote = qq
                            DetermineIfPageBreakIsNeeded(pageNumber, LinesOnCurrentPage, controlCount, GAR_Summary.LinesInControl, isLastControl)
                            controlCount += 1
                            Me.phControls.Controls.Add(GAR_Summary)
                        Case QuickQuoteObject.QuickQuoteLobType.CommercialInlandMarine 'added 5/15/2017
                            'AddPageBreakToPlaceholder()

                            Dim CIM_Summary As controls_Proposal_VRProposal_CIM_Summary = LoadControl("VRProposal_CIM_Summary.ascx")
                            CIM_Summary.QuickQuote = qq
                            DetermineIfPageBreakIsNeeded(pageNumber, LinesOnCurrentPage, controlCount, CIM_Summary.LinesInControl, isLastControl)
                            controlCount += 1
                            Me.phControls.Controls.Add(CIM_Summary)
                        Case QuickQuoteObject.QuickQuoteLobType.CommercialCrime 'added 5/15/2017
                            'AddPageBreakToPlaceholder()

                            Dim CRM_Summary As controls_Proposal_VRProposal_CRM_Summary = LoadControl("VRProposal_CRM_Summary.ascx")
                            CRM_Summary.QuickQuote = qq
                            DetermineIfPageBreakIsNeeded(pageNumber, LinesOnCurrentPage, controlCount, CRM_Summary.LinesInControl, isLastControl)
                            controlCount += 1
                            Me.phControls.Controls.Add(CRM_Summary)
                        Case Else
                            'nothing available for this lob
                            'errMsg = qqHelper.appendText(errMsg, "Nothing is currently available for quote number " & qq.QuoteNumber & ".", "<br>", True) 'True is to append error prefix w/ its own break
                            'added 4/20/2017
                            Dim LOB_Generic_Summary As controls_Proposal_VRProposal_LOB_Generic_Summary = LoadControl("VRProposal_LOB_Generic_Summary.ascx")
                            LOB_Generic_Summary.Quote = qq
                            'added 6/28/2013
                            DetermineIfPageBreakIsNeeded(pageNumber, LinesOnCurrentPage, controlCount, LOB_Generic_Summary.LinesInControl, isLastControl)
                            controlCount += 1
                            Me.phControls.Controls.Add(LOB_Generic_Summary)
                    End Select
                Next
            End With
        End If
    End Sub

    'added 6/28/2013
    Private Sub ResetPageLineCount(ByRef currLines As Integer, Optional ByVal isFirstPage As Boolean = False)
        If isFirstPage = True Then
            currLines = 18 'everything before adding LOB summaries (assuming 8 lines for ClientAndAgencyInfo control; everything else alloted to logo, headers, and breaks)
        Else
            currLines = 0
        End If

        'also add in lines after LOB summaries (combined premium, breaks, and disclaimer) since they'll always be on the last page w/ the 
        'currLines += 5 'breaks and combined premium
        'currLines += 8 'disclaimer control
        'updated to only add into total if there are no remaining controls left
    End Sub
    Private Sub AddPageBreakToPlaceholder(ByRef pageNum As Integer, ByRef controlCount As Integer, Optional ByRef currLines As Integer = Nothing, Optional ByVal controlLines As Integer = Nothing)
        proposalHelper.AddPageBreakToPlaceholder(Me.phControls)
        pageNum += 1
        controlCount = 0
        'If currLines <> Nothing Then 'removed IF 8/20/2015... shouldn't be needed since 0 should be okay
        ResetPageLineCount(currLines)
        If controlLines <> Nothing AndAlso controlLines > 0 Then 'add lines back in for control that caused break (since it will be on the next page)
            currLines += controlLines
        End If
        'End If
    End Sub
    Private Sub DetermineIfPageBreakIsNeeded(ByRef pageNum As Integer, ByRef currLines As Integer, ByRef controlCount As Integer, ByVal controlLines As Integer, ByVal isLastControl As Boolean)
        currLines += controlLines
        If isLastControl = True Then
            'currLines += 13 '5 for breaks and combined prem and 8 for disclaimer
            'updated 8/20/2015 to use new method
            AddFooterLinesToRunningCount(currLines)
        End If
        If controlCount <> Nothing AndAlso controlCount > 0 AndAlso currLines > proposalHelper.SummaryLinesInPage Then 'updated 7/13/2017 from LinesInPage
            AddPageBreakToPlaceholder(pageNum, controlCount, currLines, controlLines)
        End If
    End Sub
    'added 8/20/2015
    'Private Sub DetermineIfPageBreaksAreNeededForCPP(ByRef CPP_Summary As controls_Proposal_VRProposal_CPP_Summary, ByRef pageNum As Integer, ByRef currLines As Integer, ByRef controlCount As Integer, ByVal controlLines As Integer, ByVal isLastControl As Boolean)
    '    'note: this method assumes that the order of the CPP packagePart controls is CPR, CGL, CIM, and then CRM; would need to update logic if order changes
    '    If CPP_Summary Is Nothing Then
    '        DetermineIfPageBreakIsNeeded(pageNum, currLines, controlCount, controlLines, isLastControl)
    '    Else
    '        If isLastControl = True Then
    '            'currLines += 13 '5 for breaks and combined prem and 8 for disclaimer
    '            'updated 8/20/2015 to use new method
    '            AddFooterLinesToRunningCount(currLines)
    '        End If
    '        If proposalHelper.Sum(currLines, CPP_Summary.LinesInControl) <= proposalHelper.SummaryLinesInPage Then 'updated 7/13/2017 from LinesInPage
    '            'everything will fit; no page breaks needed here
    '            'now add control lines to running count
    '            currLines += CPP_Summary.LinesInControl
    '        Else
    '            'should need a page break somewhere
    '            If proposalHelper.Sum(currLines, CPP_Summary.Lines_start_thru_CPR) > proposalHelper.SummaryLinesInPage Then 'updated 7/13/2017 from LinesInPage
    '                '1st CPP packagePart control won't fit on page; need to do page break now
    '                'AddPageBreakToPlaceholder(pageNum, controlCount, currLines, CPP_Summary.Lines_start_thru_CPR)
    '                ''new page after CPR; now add footer lines back on if needed and check CGL and so-on
    '                'If isLastControl = True Then
    '                '    AddFooterLinesToRunningCount(currLines)
    '                'End If
    '                ''now check CGL and so-on
    '                'removed previous logic so we can just re-call this method
    '                'new page before CPP control; send 0 for controlLines so it will restart count at 0
    '                AddPageBreakToPlaceholder(pageNum, controlCount, currLines, 0)
    '                DetermineIfPageBreaksAreNeededForCPP(CPP_Summary, pageNum, currLines, controlCount, controlLines, isLastControl)
    '                Exit Sub
    '            Else
    '                'at least 1st CPP packagePart control will fit on page, but there should be a break at some point
    '                'remove footer lines from count if needed since the next packagePart control is not the last
    '                If isLastControl = True Then
    '                    SubtractFooterLinesFromRunningCount(currLines)
    '                End If
    '                If proposalHelper.Sum(currLines, CPP_Summary.Lines_start_thru_CIM) <= proposalHelper.SummaryLinesInPage Then 'updated 7/13/2017 from LinesInPage
    '                    '1st CPP packagePart control thru CIM control will fit on page; should need break between CIM and CRM
    '                    CPP_Summary.EnableOrDisablePageBreak_between_CIM_and_CRM(True)
    '                    'break added; now reset currLines
    '                    pageNum += 1
    '                    ResetPageLineCount(currLines)
    '                    If isLastControl = True Then
    '                        AddFooterLinesToRunningCount(currLines)
    '                    End If
    '                    'now add applicable control lines to running count
    '                    currLines += CPP_Summary.Lines_CRM_to_end
    '                    'nothing else needed since we just started a new page and CRM is the last control
    '                ElseIf proposalHelper.Sum(currLines, CPP_Summary.Lines_start_thru_CGL) <= proposalHelper.SummaryLinesInPage Then 'updated 7/13/2017 from LinesInPage
    '                    '1st CPP packagePart control thru CGL control will fit on page; should need break between CGL and CIM; will also need to make sure another break isn't needed between CIM and CRM
    '                    CPP_Summary.EnableOrDisablePageBreak_between_CGL_and_CIM(True)
    '                    'break added; now reset currLines
    '                    pageNum += 1
    '                    ResetPageLineCount(currLines)
    '                    If isLastControl = True Then
    '                        AddFooterLinesToRunningCount(currLines)
    '                    End If
    '                    'now see if break is needed between CIM and CRM
    '                    If proposalHelper.Sum(currLines, CPP_Summary.Lines_CIM_to_end) <= proposalHelper.SummaryLinesInPage Then 'updated 7/13/2017 from LinesInPage
    '                        'okay; enough room on last page for CIM and CRM
    '                        'now add applicable control lines to running count
    '                        currLines += CPP_Summary.Lines_CIM_to_end
    '                    Else
    '                        'needs break between CIM and CRM
    '                        CPP_Summary.EnableOrDisablePageBreak_between_CIM_and_CRM(True)
    '                        'break added; now reset currLines
    '                        pageNum += 1
    '                        ResetPageLineCount(currLines)
    '                        If isLastControl = True Then
    '                            AddFooterLinesToRunningCount(currLines)
    '                        End If
    '                        'now add applicable control lines to running count
    '                        currLines += CPP_Summary.Lines_CRM_to_end
    '                        'nothing else needed since we just started a new page and CRM is the last control
    '                    End If
    '                Else
    '                    'shouldn't get here
    '                End If
    '            End If
    '        End If
    '    End If
    'End Sub
    'updated 4/22/2017 for GAR
    'Private Sub DetermineIfPageBreaksAreNeededForCPP(ByRef CPP_Summary As controls_Proposal_VRProposal_CPP_Summary, ByRef pageNum As Integer, ByRef currLines As Integer, ByRef controlCount As Integer, ByVal controlLines As Integer, ByVal isLastControl As Boolean)
    '    'note: this method assumes that the order of the CPP packagePart controls is CPR, CGL, CIM, CRM, and then GAR; would need to update logic if order changes
    '    If CPP_Summary Is Nothing Then
    '        DetermineIfPageBreakIsNeeded(pageNum, currLines, controlCount, controlLines, isLastControl)
    '    Else
    '        If isLastControl = True Then
    '            'currLines += 13 '5 for breaks and combined prem and 8 for disclaimer
    '            'updated 8/20/2015 to use new method
    '            AddFooterLinesToRunningCount(currLines)
    '        End If
    '        If proposalHelper.Sum(currLines, CPP_Summary.LinesInControl) <= proposalHelper.SummaryLinesInPage Then 'updated 7/13/2017 from LinesInPage
    '            'everything will fit; no page breaks needed here
    '            'now add control lines to running count
    '            currLines += CPP_Summary.LinesInControl
    '        Else
    '            'should need a page break somewhere
    '            If proposalHelper.Sum(currLines, CPP_Summary.Lines_start_thru_CPR) > proposalHelper.SummaryLinesInPage Then 'updated 7/13/2017 from LinesInPage
    '                '1st CPP packagePart control won't fit on page; need to do page break now
    '                'AddPageBreakToPlaceholder(pageNum, controlCount, currLines, CPP_Summary.Lines_start_thru_CPR)
    '                ''new page after CPR; now add footer lines back on if needed and check CGL and so-on
    '                'If isLastControl = True Then
    '                '    AddFooterLinesToRunningCount(currLines)
    '                'End If
    '                ''now check CGL and so-on
    '                'removed previous logic so we can just re-call this method
    '                'new page before CPP control; send 0 for controlLines so it will restart count at 0
    '                AddPageBreakToPlaceholder(pageNum, controlCount, currLines, 0)
    '                DetermineIfPageBreaksAreNeededForCPP(CPP_Summary, pageNum, currLines, controlCount, controlLines, isLastControl)
    '                Exit Sub
    '            Else
    '                'at least 1st CPP packagePart control will fit on page, but there should be a break at some point
    '                'remove footer lines from count if needed since the next packagePart control is not the last
    '                If isLastControl = True Then
    '                    SubtractFooterLinesFromRunningCount(currLines)
    '                End If
    '                If proposalHelper.Sum(currLines, CPP_Summary.Lines_start_thru_CRM) <= proposalHelper.SummaryLinesInPage Then 'updated 7/13/2017 from LinesInPage
    '                    '1st CPP packagePart control thru CRM control will fit on page; should need break between CRM and GAR
    '                    CPP_Summary.EnableOrDisablePageBreak_between_CRM_and_GAR(True)
    '                    'break added; now reset currLines
    '                    pageNum += 1
    '                    ResetPageLineCount(currLines)
    '                    If isLastControl = True Then
    '                        AddFooterLinesToRunningCount(currLines)
    '                    End If
    '                    'now add applicable control lines to running count
    '                    currLines += CPP_Summary.Lines_GAR_to_end
    '                    'nothing else needed since we just started a new page and GAR is the last control
    '                ElseIf proposalHelper.Sum(currLines, CPP_Summary.Lines_start_thru_CIM) <= proposalHelper.SummaryLinesInPage Then 'updated 7/13/2017 from LinesInPage
    '                    '1st CPP packagePart control thru CIM control will fit on page; should need break between CIM and CRM; will also need to make sure another break isn't needed between CRM and GAR
    '                    CPP_Summary.EnableOrDisablePageBreak_between_CIM_and_CRM(True)
    '                    'break added; now reset currLines
    '                    pageNum += 1
    '                    ResetPageLineCount(currLines)
    '                    If isLastControl = True Then
    '                        AddFooterLinesToRunningCount(currLines)
    '                    End If
    '                    'now see if break is needed between CRM and GAR
    '                    If proposalHelper.Sum(currLines, CPP_Summary.Lines_CRM_to_end) <= proposalHelper.SummaryLinesInPage Then 'updated 7/13/2017 from LinesInPage
    '                        'okay; enough room on last page for CRM and GAR
    '                        'now add applicable control lines to running count
    '                        currLines += CPP_Summary.Lines_CRM_to_end
    '                    Else
    '                        'needs break between CRM and GAR
    '                        CPP_Summary.EnableOrDisablePageBreak_between_CRM_and_GAR(True)
    '                        'break added; now reset currLines
    '                        pageNum += 1
    '                        ResetPageLineCount(currLines)
    '                        If isLastControl = True Then
    '                            AddFooterLinesToRunningCount(currLines)
    '                        End If
    '                        'now add applicable control lines to running count
    '                        currLines += CPP_Summary.Lines_GAR_to_end
    '                        'nothing else needed since we just started a new page and GAR is the last control
    '                    End If
    '                ElseIf proposalHelper.Sum(currLines, CPP_Summary.Lines_start_thru_CGL) <= proposalHelper.SummaryLinesInPage Then 'updated 7/13/2017 from LinesInPage
    '                    '1st CPP packagePart control thru CGL control will fit on page; should need break between CGL and CIM; will also need to make sure additional breaks aren't needed between CIM and CRM or CRM and GAR
    '                    CPP_Summary.EnableOrDisablePageBreak_between_CGL_and_CIM(True)
    '                    'break added; now reset currLines
    '                    pageNum += 1
    '                    ResetPageLineCount(currLines)
    '                    If isLastControl = True Then
    '                        AddFooterLinesToRunningCount(currLines)
    '                    End If
    '                    'now see if breaks are needed between CIM and CRM or CRM and GAR
    '                    If proposalHelper.Sum(currLines, CPP_Summary.Lines_CIM_to_end) <= proposalHelper.SummaryLinesInPage Then 'updated 7/13/2017 from LinesInPage
    '                        'okay; enough room on last page for CIM, CRM, and GAR
    '                        'now add applicable control lines to running count
    '                        currLines += CPP_Summary.Lines_CIM_to_end
    '                    Else
    '                        'needs break between CIM and CRM and/or CRM and GAR
    '                        '1st see if CIM thru CRM will fit on 1 page
    '                        If proposalHelper.Sum(currLines, CPP_Summary.Lines_CIM_thru_CRM) <= proposalHelper.SummaryLinesInPage Then 'updated 7/13/2017 from LinesInPage
    '                            'CIM thru CRM fits; needs break between CRM and GAR
    '                            CPP_Summary.EnableOrDisablePageBreak_between_CRM_and_GAR(True)
    '                            'break added; now reset currLines
    '                            pageNum += 1
    '                            ResetPageLineCount(currLines)
    '                            If isLastControl = True Then
    '                                AddFooterLinesToRunningCount(currLines)
    '                            End If
    '                            'now add applicable control lines to running count
    '                            currLines += CPP_Summary.Lines_GAR_to_end
    '                            'nothing else needed since we just started a new page and GAR is the last control
    '                        Else
    '                            'needs break between CIM and CRM
    '                            CPP_Summary.EnableOrDisablePageBreak_between_CIM_and_CRM(True)
    '                            'break added; now reset currLines
    '                            pageNum += 1
    '                            ResetPageLineCount(currLines)
    '                            If isLastControl = True Then
    '                                AddFooterLinesToRunningCount(currLines)
    '                            End If
    '                            'now see if break is needed between CRM and GAR
    '                            If proposalHelper.Sum(currLines, CPP_Summary.Lines_CRM_to_end) <= proposalHelper.SummaryLinesInPage Then 'updated 7/13/2017 from LinesInPage
    '                                'okay; enough room on last page for CRM and GAR
    '                                'now add applicable control lines to running count
    '                                currLines += CPP_Summary.Lines_CRM_to_end
    '                            Else
    '                                'needs break between CRM and GAR
    '                                CPP_Summary.EnableOrDisablePageBreak_between_CRM_and_GAR(True)
    '                                'break added; now reset currLines
    '                                pageNum += 1
    '                                ResetPageLineCount(currLines)
    '                                If isLastControl = True Then
    '                                    AddFooterLinesToRunningCount(currLines)
    '                                End If
    '                                'now add applicable control lines to running count
    '                                currLines += CPP_Summary.Lines_GAR_to_end
    '                                'nothing else needed since we just started a new page and GAR is the last control
    '                            End If
    '                        End If
    '                    End If
    '                Else
    '                    'shouldn't get here
    '                End If
    '            End If
    '        End If
    '    End If
    'End Sub
    'updated again 5/13/2017 to move GAR from last to in between CGL and CIM... new order is CPR, CGL, GAR, CIM, and CRM
    Private Sub DetermineIfPageBreaksAreNeededForCPP(ByRef CPP_Summary As controls_Proposal_VRProposal_CPP_Summary, ByRef pageNum As Integer, ByRef currLines As Integer, ByRef controlCount As Integer, ByVal controlLines As Integer, ByVal isLastControl As Boolean)
        'note: this method assumes that the order of the CPP packagePart controls is CPR, CGL, CIM, CRM, and then GAR; would need to update logic if order changes
        If CPP_Summary Is Nothing Then
            DetermineIfPageBreakIsNeeded(pageNum, currLines, controlCount, controlLines, isLastControl)
        Else
            If isLastControl = True Then
                'currLines += 13 '5 for breaks and combined prem and 8 for disclaimer
                'updated 8/20/2015 to use new method
                AddFooterLinesToRunningCount(currLines)
            End If
            If proposalHelper.Sum(currLines, CPP_Summary.LinesInControl) <= proposalHelper.SummaryLinesInPage Then 'updated 7/13/2017 from LinesInPage
                'everything will fit; no page breaks needed here
                'now add control lines to running count
                currLines += CPP_Summary.LinesInControl
            Else
                'should need a page break somewhere; see which control shows up first (order: CPR, CGL, GAR, CIM, CRM)
                If CPP_Summary.CPR_IsVisible = True Then
                    'CPR is visible; might have all packageParts (CPR, CGL, GAR, CIM, and CRM)
                    If proposalHelper.Sum(currLines, CPP_Summary.Lines_start_thru_CPR) > proposalHelper.SummaryLinesInPage Then 'updated 7/13/2017 from LinesInPage
                        '1st CPP packagePart control (CPR) won't fit on page; need to do page break now
                        'AddPageBreakToPlaceholder(pageNum, controlCount, currLines, CPP_Summary.Lines_start_thru_CPR)
                        ''new page after CPR; now add footer lines back on if needed and check CGL and so-on
                        'If isLastControl = True Then
                        '    AddFooterLinesToRunningCount(currLines)
                        'End If
                        ''now check CGL and so-on
                        'removed previous logic so we can just re-call this method
                        'new page before CPP control; send 0 for controlLines so it will restart count at 0
                        AddPageBreakToPlaceholder(pageNum, controlCount, currLines, 0)
                        DetermineIfPageBreaksAreNeededForCPP(CPP_Summary, pageNum, currLines, controlCount, controlLines, isLastControl)
                        Exit Sub
                    Else
                        'at least 1st CPP packagePart control (CPR) will fit on page, but there should be a break at some point
                        'remove footer lines from count if needed since the next packagePart control is not the last
                        If isLastControl = True Then
                            SubtractFooterLinesFromRunningCount(currLines)
                        End If
                        If proposalHelper.Sum(currLines, CPP_Summary.Lines_start_thru_CIM) <= proposalHelper.SummaryLinesInPage Then 'updated 7/13/2017 from LinesInPage
                            '1st CPP packagePart control thru CIM control will fit on page; should need break between CIM and CRM
                            CPP_Summary.EnableOrDisablePageBreak_between_CIM_and_CRM(True)
                            'break added; now reset currLines
                            pageNum += 1
                            ResetPageLineCount(currLines)
                            If isLastControl = True Then
                                AddFooterLinesToRunningCount(currLines)
                            End If
                            'now add applicable control lines to running count
                            currLines += CPP_Summary.Lines_CRM_to_end
                            'nothing else needed since we just started a new page and CRM is the last control
                        ElseIf proposalHelper.Sum(currLines, CPP_Summary.Lines_start_thru_GAR) <= proposalHelper.SummaryLinesInPage Then 'updated 7/13/2017 from LinesInPage
                            '1st CPP packagePart control thru GAR control will fit on page; should need break between GAR and CIM; will also need to make sure another break isn't needed between CIM and CRM
                            CPP_Summary.EnableOrDisablePageBreak_between_GAR_and_CIM(True)
                            'break added; now reset currLines
                            pageNum += 1
                            ResetPageLineCount(currLines)
                            If isLastControl = True Then
                                AddFooterLinesToRunningCount(currLines)
                            End If
                            'now see if break is needed between CIM and CRM
                            If proposalHelper.Sum(currLines, CPP_Summary.Lines_CIM_to_end) <= proposalHelper.SummaryLinesInPage Then 'updated 7/13/2017 from LinesInPage
                                'okay; enough room on last page for CIM and CRM
                                'now add applicable control lines to running count
                                currLines += CPP_Summary.Lines_CIM_to_end
                            Else
                                'needs break between CIM and CRM
                                CPP_Summary.EnableOrDisablePageBreak_between_CIM_and_CRM(True)
                                'break added; now reset currLines
                                pageNum += 1
                                ResetPageLineCount(currLines)
                                If isLastControl = True Then
                                    AddFooterLinesToRunningCount(currLines)
                                End If
                                'now add applicable control lines to running count
                                currLines += CPP_Summary.Lines_CRM_to_end
                                'nothing else needed since we just started a new page and CRM is the last control
                            End If
                        ElseIf proposalHelper.Sum(currLines, CPP_Summary.Lines_start_thru_CGL) <= proposalHelper.SummaryLinesInPage Then 'updated 7/13/2017 from LinesInPage
                            '1st CPP packagePart control thru CGL control will fit on page; should need break between CGL and GAR; will also need to make sure additional breaks aren't needed between GAR and CIM or CIM and CRM
                            CPP_Summary.EnableOrDisablePageBreak_between_CGL_and_GAR(True)
                            'break added; now reset currLines
                            pageNum += 1
                            ResetPageLineCount(currLines)
                            If isLastControl = True Then
                                AddFooterLinesToRunningCount(currLines)
                            End If
                            'now see if breaks are needed between GAR and CIM or CIM and CRM
                            If proposalHelper.Sum(currLines, CPP_Summary.Lines_GAR_to_end) <= proposalHelper.SummaryLinesInPage Then 'updated 7/13/2017 from LinesInPage
                                'okay; enough room on last page for GAR, CIM, and CRM
                                'now add applicable control lines to running count
                                currLines += CPP_Summary.Lines_GAR_to_end
                            Else
                                'needs break between GAR and CIM and/or CIM and CRM
                                '1st see if GAR thru CIM will fit on 1 page
                                If proposalHelper.Sum(currLines, CPP_Summary.Lines_GAR_thru_CIM) <= proposalHelper.SummaryLinesInPage Then 'updated 7/13/2017 from LinesInPage
                                    'GAR thru CIM fits; needs break between CIM and CRM
                                    CPP_Summary.EnableOrDisablePageBreak_between_CIM_and_CRM(True)
                                    'break added; now reset currLines
                                    pageNum += 1
                                    ResetPageLineCount(currLines)
                                    If isLastControl = True Then
                                        AddFooterLinesToRunningCount(currLines)
                                    End If
                                    'now add applicable control lines to running count
                                    currLines += CPP_Summary.Lines_CRM_to_end
                                    'nothing else needed since we just started a new page and CRM is the last control
                                Else
                                    'needs break between GAR and CIM
                                    CPP_Summary.EnableOrDisablePageBreak_between_GAR_and_CIM(True)
                                    'break added; now reset currLines
                                    pageNum += 1
                                    ResetPageLineCount(currLines)
                                    If isLastControl = True Then
                                        AddFooterLinesToRunningCount(currLines)
                                    End If
                                    'now see if break is needed between CIM and CRM
                                    If proposalHelper.Sum(currLines, CPP_Summary.Lines_CIM_to_end) <= proposalHelper.SummaryLinesInPage Then 'updated 7/13/2017 from LinesInPage
                                        'okay; enough room on last page for CIM and CRM
                                        'now add applicable control lines to running count
                                        currLines += CPP_Summary.Lines_CIM_to_end
                                    Else
                                        'needs break between CIM and CRM
                                        CPP_Summary.EnableOrDisablePageBreak_between_CIM_and_CRM(True)
                                        'break added; now reset currLines
                                        pageNum += 1
                                        ResetPageLineCount(currLines)
                                        If isLastControl = True Then
                                            AddFooterLinesToRunningCount(currLines)
                                        End If
                                        'now add applicable control lines to running count
                                        currLines += CPP_Summary.Lines_CRM_to_end
                                        'nothing else needed since we just started a new page and CRM is the last control
                                    End If
                                End If
                            End If
                        Else
                            'shouldn't get here
                        End If
                    End If
                ElseIf CPP_Summary.CGL_IsVisible = True Then
                    'CGL is 1st visible packagePart; doesn't have CPR; might have CGL, GAR, CIM, and CRM
                    If proposalHelper.Sum(currLines, CPP_Summary.Lines_start_thru_CGL) > proposalHelper.SummaryLinesInPage Then 'updated 7/13/2017 from LinesInPage
                        '1st CPP packagePart control (CGL) won't fit on page; need to do page break now
                        'new page before CPP control; send 0 for controlLines so it will restart count at 0
                        AddPageBreakToPlaceholder(pageNum, controlCount, currLines, 0)
                        DetermineIfPageBreaksAreNeededForCPP(CPP_Summary, pageNum, currLines, controlCount, controlLines, isLastControl)
                        Exit Sub
                    Else
                        'at least 1st CPP packagePart control (CGL) will fit on page, but there should be a break at some point
                        'remove footer lines from count if needed since the next packagePart control is not the last
                        If isLastControl = True Then
                            SubtractFooterLinesFromRunningCount(currLines)
                        End If
                        If proposalHelper.Sum(currLines, CPP_Summary.Lines_start_thru_CIM) <= proposalHelper.SummaryLinesInPage Then 'updated 7/13/2017 from LinesInPage
                            '1st CPP packagePart control thru CIM control will fit on page; should need break between CIM and CRM
                            CPP_Summary.EnableOrDisablePageBreak_between_CIM_and_CRM(True)
                            'break added; now reset currLines
                            pageNum += 1
                            ResetPageLineCount(currLines)
                            If isLastControl = True Then
                                AddFooterLinesToRunningCount(currLines)
                            End If
                            'now add applicable control lines to running count
                            currLines += CPP_Summary.Lines_CRM_to_end
                            'nothing else needed since we just started a new page and CRM is the last control
                        ElseIf proposalHelper.Sum(currLines, CPP_Summary.Lines_start_thru_GAR) <= proposalHelper.SummaryLinesInPage Then 'updated 7/13/2017 from LinesInPage
                            '1st CPP packagePart control thru GAR control will fit on page; should need break between GAR and CIM; will also need to make sure another break isn't needed between CIM and CRM
                            CPP_Summary.EnableOrDisablePageBreak_between_GAR_and_CIM(True)
                            'break added; now reset currLines
                            pageNum += 1
                            ResetPageLineCount(currLines)
                            If isLastControl = True Then
                                AddFooterLinesToRunningCount(currLines)
                            End If
                            'now see if break is needed between CIM and CRM
                            If proposalHelper.Sum(currLines, CPP_Summary.Lines_CIM_to_end) <= proposalHelper.SummaryLinesInPage Then 'updated 7/13/2017 from LinesInPage
                                'okay; enough room on last page for CIM and CRM
                                'now add applicable control lines to running count
                                currLines += CPP_Summary.Lines_CIM_to_end
                            Else
                                'needs break between CIM and CRM
                                CPP_Summary.EnableOrDisablePageBreak_between_CIM_and_CRM(True)
                                'break added; now reset currLines
                                pageNum += 1
                                ResetPageLineCount(currLines)
                                If isLastControl = True Then
                                    AddFooterLinesToRunningCount(currLines)
                                End If
                                'now add applicable control lines to running count
                                currLines += CPP_Summary.Lines_CRM_to_end
                                'nothing else needed since we just started a new page and CRM is the last control
                            End If
                        Else
                            'shouldn't get here
                        End If
                    End If
                ElseIf CPP_Summary.GAR_IsVisible = True Then
                    'GAR is 1st visible packagePart; doesn't have CPR or GAR; might have GAR, CIM, and CRM
                    If proposalHelper.Sum(currLines, CPP_Summary.Lines_start_thru_GAR) > proposalHelper.SummaryLinesInPage Then 'updated 7/13/2017 from LinesInPage
                        '1st CPP packagePart control (GAR) won't fit on page; need to do page break now
                        'new page before CPP control; send 0 for controlLines so it will restart count at 0
                        AddPageBreakToPlaceholder(pageNum, controlCount, currLines, 0)
                        DetermineIfPageBreaksAreNeededForCPP(CPP_Summary, pageNum, currLines, controlCount, controlLines, isLastControl)
                        Exit Sub
                    Else
                        'at least 1st CPP packagePart control (GAR) will fit on page, but there should be a break at some point
                        'remove footer lines from count if needed since the next packagePart control is not the last
                        If isLastControl = True Then
                            SubtractFooterLinesFromRunningCount(currLines)
                        End If
                        If proposalHelper.Sum(currLines, CPP_Summary.Lines_start_thru_CIM) <= proposalHelper.SummaryLinesInPage Then 'updated 7/13/2017 from LinesInPage
                            '1st CPP packagePart control thru CIM control will fit on page; should need break between CIM and CRM
                            CPP_Summary.EnableOrDisablePageBreak_between_CIM_and_CRM(True)
                            'break added; now reset currLines
                            pageNum += 1
                            ResetPageLineCount(currLines)
                            If isLastControl = True Then
                                AddFooterLinesToRunningCount(currLines)
                            End If
                            'now add applicable control lines to running count
                            currLines += CPP_Summary.Lines_CRM_to_end
                            'nothing else needed since we just started a new page and CRM is the last control
                        Else
                            'shouldn't get here
                        End If
                    End If
                ElseIf CPP_Summary.CIM_IsVisible = True Then
                    'CIM is 1st visible packagePart; doesn't have CPR, CGL, or GAR; might have CIM and CRM
                    If proposalHelper.Sum(currLines, CPP_Summary.Lines_start_thru_CIM) > proposalHelper.SummaryLinesInPage Then 'updated 7/13/2017 from LinesInPage
                        '1st CPP packagePart control (CIM) won't fit on page; need to do page break now
                        'new page before CPP control; send 0 for controlLines so it will restart count at 0
                        AddPageBreakToPlaceholder(pageNum, controlCount, currLines, 0)
                        DetermineIfPageBreaksAreNeededForCPP(CPP_Summary, pageNum, currLines, controlCount, controlLines, isLastControl)
                        Exit Sub
                    Else
                        'at least 1st CPP packagePart control (CIM) will fit on page, but there should be a break at some point... must be between CIM and CRM since CRM is the last control
                        'remove footer lines from count if needed since the next packagePart control is not the last
                        If isLastControl = True Then
                            SubtractFooterLinesFromRunningCount(currLines)
                        End If
                        'header thru CIM control will fit on page; should need break between CIM and CRM
                        CPP_Summary.EnableOrDisablePageBreak_between_CIM_and_CRM(True)
                        'break added; now reset currLines
                        pageNum += 1
                        ResetPageLineCount(currLines)
                        If isLastControl = True Then
                            AddFooterLinesToRunningCount(currLines)
                        End If
                        'now add applicable control lines to running count
                        currLines += CPP_Summary.Lines_CRM_to_end
                        'nothing else needed since we just started a new page and CRM is the last control
                    End If
                ElseIf CPP_Summary.CRM_IsVisible = True Then
                    'CRM is only visible packagePart; doesn't have CPR, CGL, GAR, or CIM; need to do pageBreak now
                    'new page before CPP control; send 0 for controlLines so it will restart count at 0
                    AddPageBreakToPlaceholder(pageNum, controlCount, currLines, 0)
                    DetermineIfPageBreaksAreNeededForCPP(CPP_Summary, pageNum, currLines, controlCount, controlLines, isLastControl)
                    Exit Sub
                Else
                    'no packageParts are visible; need to do pageBreak now since header and footer lines won't fit
                    'new page before CPP control; send 0 for controlLines so it will restart count at 0
                    AddPageBreakToPlaceholder(pageNum, controlCount, currLines, 0)
                    DetermineIfPageBreaksAreNeededForCPP(CPP_Summary, pageNum, currLines, controlCount, controlLines, isLastControl)
                    Exit Sub
                End If
            End If
        End If
    End Sub
    'added 8/20/2015
    Public Sub AddFooterLinesToRunningCount(ByRef currLines As Integer)
        currLines += FooterLinesCount()
    End Sub
    Public Sub SubtractFooterLinesFromRunningCount(ByRef currLines As Integer)
        currLines -= FooterLinesCount()
    End Sub
    Public Function FooterLinesCount() As Integer
        Return 13 '5 for breaks and combined prem and 8 for disclaimer
    End Function
End Class
