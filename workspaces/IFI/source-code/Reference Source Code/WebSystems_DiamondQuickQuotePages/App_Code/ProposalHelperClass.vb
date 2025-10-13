Imports Microsoft.VisualBasic

'added 3/24/2017
Imports QuickQuote.CommonObjects

Public Class ProposalHelperClass 'added 6/10/2013

    Public ReadOnly Property LinesInPage As Integer 'added 6/28/2013
        Get
            'Return 60
            'updated 4/20/2017; didn't work for Patch test (1st https://www.ifmig.net/agentsonly/DiamondQuoteProposalLoader.aspx?diamondProposalBinaryId=6 and then https://www.ifmig.net/agentsonly/DiamondQuoteProposal.aspx?printerfriendlydiamondpolicyidsandpolicyimagenums=1142055**1||1142046**1||1142294**1, which produced https://www.ifmig.net/agentsonly/DiamondQuoteProposalLoader.aspx?diamondProposalBinaryId=48)
            'Return 59
            'updated 7/1/2015 to use config key
            'If ConfigurationManager.AppSettings("VRProposalLinesPerPage") IsNot Nothing AndAlso ConfigurationManager.AppSettings("VRProposalLinesPerPage").ToString <> "" AndAlso IsNumeric(ConfigurationManager.AppSettings("VRProposalLinesPerPage").ToString) = True Then
            '    Return CInt(ConfigurationManager.AppSettings("VRProposalLinesPerPage").ToString)
            'Else
            '    Return 60
            'End If
            'updated 7/13/2017; was previously using hard-coded 60
            Dim _linesInPage As Integer = 0
            Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
            _linesInPage = qqHelper.IntegerForString(QuickQuote.CommonMethods.QuickQuoteHelperClass.configAppSettingValueAsString("VRProposalLinesPerPage"))
            If _linesInPage <= 0 Then
                _linesInPage = 60
            End If
            Return _linesInPage
        End Get
    End Property
    Public ReadOnly Property SummaryLinesInPage As Integer 'added 7/13/2017 so # used by Summary can change without affecting anything else
        Get
            Dim _linesInPage As Integer = 0
            Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
            _linesInPage = qqHelper.IntegerForString(QuickQuote.CommonMethods.QuickQuoteHelperClass.configAppSettingValueAsString("VRProposalSummaryLinesPerPage"))
            If _linesInPage <= 0 Then
                _linesInPage = LinesInPage
                If _linesInPage <= 0 Then
                    _linesInPage = 60
                End If
            End If
            Return _linesInPage
        End Get
    End Property

    Public Function GetAgencyName(ByVal name As QuickQuoteName) As String 'added 6/10/2013
        Dim agName As String = ""

        If name IsNot Nothing Then
            If name.CommercialDBAname <> "" Then
                agName = name.CommercialDBAname
            ElseIf name.DoingBusinessAsName <> "" Then 'this probably won't ever have a value
                agName = name.DoingBusinessAsName
            ElseIf name.CommercialIRSname <> "" Then
                agName = name.CommercialIRSname
            Else
                agName = name.DisplayNameForWeb 'just in case nothing is found in any of the above, which should mean that this one is blank too
            End If
        End If

        Return agName
    End Function

    Public Sub AddPageBreakToPlaceholder(ByRef ph As PlaceHolder) 'added 6/28/2013 (taken from code on DiamondQuoteProposal.aspx.vb)
        Dim pageBreak As New Panel
        pageBreak.CssClass = "page-break"
        Dim pageBreakSection As New Panel
        pageBreakSection.CssClass = "page-break-section"
        'pageBreak.Controls.Add(pageBreakSection)'would only do this if other div needs to wrap around it
        ph.Controls.Add(pageBreak)
        ph.Controls.Add(pageBreakSection)
    End Sub

    'added 8/20/2015
    Public Function Sum(ByVal num1 As Integer, ByVal num2 As Integer) As Integer
        Dim amt As Integer = 0

        amt = num1 + num2

        Return amt
    End Function

    'added 5/13/2017 to centralize logic and be able to do multiple things from same method
    Public Sub CheckForProposalLobControlAndAddToPlaceholder(ByVal qq As QuickQuoteObject, Optional ByVal lobToCheckFor As QuickQuoteObject.QuickQuoteLobType = QuickQuoteObject.QuickQuoteLobType.None, Optional ByRef hasLobControl As Boolean = False, Optional ByVal addToPlaceholder As Boolean = True, Optional ByRef webPage As System.Web.UI.Page = Nothing, Optional ByRef ph As PlaceHolder = Nothing)
        hasLobControl = False
        If qq IsNot Nothing Then
            If lobToCheckFor = Nothing OrElse lobToCheckFor = QuickQuoteObject.QuickQuoteLobType.None Then
                lobToCheckFor = qq.LobType
            End If
            If addToPlaceholder = True Then
                If webPage Is Nothing Then
                    webPage = New System.Web.UI.Page
                End If
                If ph Is Nothing Then
                    ph = New PlaceHolder
                End If
            End If

            Select Case lobToCheckFor
                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                    hasLobControl = True

                    If addToPlaceholder = True Then
                        AddPageBreakToPlaceholder(ph)

                        If ConfigurationManager.AppSettings("QuickQuote_BOP_ProposalControlPath") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_BOP_ProposalControlPath").ToString <> "" Then
                            Dim BOP As uc_VRProposal_BOP = webPage.LoadControl(ConfigurationManager.AppSettings("QuickQuote_BOP_ProposalControlPath").ToString)
                            BOP.QO = qq
                            ph.Controls.Add(BOP)
                        Else
                            Dim l As New Label
                            l.Text = "<br /><b>BOP Quote:  </b>" & qq.PolicyNumber 'use PolicyNumber instead of QuoteNumber
                            l.Text &= "<br />Premium:  " & qq.TotalQuotedPremium
                            ph.Controls.Add(l)
                        End If
                    End If
                Case QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
                    hasLobControl = True

                    If addToPlaceholder = True Then
                        AddPageBreakToPlaceholder(ph)

                        If ConfigurationManager.AppSettings("QuickQuote_CGL_ProposalControlPath") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_CGL_ProposalControlPath").ToString <> "" Then
                            Dim CGL As Controls_Proposal_CGL_Velocirater_Proposal_CGL_Control = webPage.LoadControl(ConfigurationManager.AppSettings("QuickQuote_CGL_ProposalControlPath").ToString)
                            CGL.QO = qq
                            ph.Controls.Add(CGL)
                        Else
                            Dim l As New Label
                            l.Text = "<br /><b>CGL Quote:  </b>" & qq.PolicyNumber 'use PolicyNumber instead of QuoteNumber
                            l.Text &= "<br />Premium:  " & qq.TotalQuotedPremium
                            ph.Controls.Add(l)
                        End If
                    End If
                Case QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                    hasLobControl = True

                    If addToPlaceholder = True Then
                        AddPageBreakToPlaceholder(ph)

                        If ConfigurationManager.AppSettings("QuickQuote_WCP_ProposalControlPath") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_WCP_ProposalControlPath").ToString <> "" Then
                            Dim WCP As uc_VRProposal_WCP = webPage.LoadControl(ConfigurationManager.AppSettings("QuickQuote_WCP_ProposalControlPath").ToString)
                            WCP.QO = qq
                            ph.Controls.Add(WCP)
                        Else
                            Dim l As New Label
                            l.Text = "<br /><b>WCP Quote:  </b>" & qq.PolicyNumber 'use PolicyNumber instead of QuoteNumber
                            'l.Text &= "<br />Premium:  " & qq.TotalQuotedPremium
                            'updated 5/6/2013 to use different property for WCP premium
                            l.Text &= "<br />Premium:  " & qq.Dec_WC_TotalPremiumDue
                            ph.Controls.Add(l)
                        End If
                    End If
                Case QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                    hasLobControl = True

                    If addToPlaceholder = True Then
                        AddPageBreakToPlaceholder(ph)

                        If ConfigurationManager.AppSettings("QuickQuote_CPR_ProposalControlPath") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_CPR_ProposalControlPath").ToString <> "" Then
                            Dim CPR As Controls_Proposal_CPR_Velocirater_Proposal_CPR_Control = webPage.LoadControl(ConfigurationManager.AppSettings("QuickQuote_CPR_ProposalControlPath").ToString)
                            CPR.QO = qq
                            ph.Controls.Add(CPR)
                        Else
                            Dim l As New Label
                            l.Text = "<br /><b>CPR Quote:  </b>" & qq.PolicyNumber 'use PolicyNumber instead Of QuoteNumber
                            l.Text &= "<br />Premium:  " & qq.TotalQuotedPremium
                            ph.Controls.Add(l)
                        End If
                    End If
                Case QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                    hasLobControl = True

                    If addToPlaceholder = True Then
                        AddPageBreakToPlaceholder(ph)

                        If ConfigurationManager.AppSettings("QuickQuote_CPP_ProposalControlPath") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_CPP_ProposalControlPath").ToString <> "" Then
                            Dim CPP As Controls_Proposal_CPP_Velocirater_Proposal_CPP_Control = webPage.LoadControl(ConfigurationManager.AppSettings("QuickQuote_CPP_ProposalControlPath").ToString)
                            CPP.QO = qq
                            ph.Controls.Add(CPP)
                        Else
                            Dim l As New Label
                            l.Text = "<br /><b>CPP Quote:  </b>" & qq.PolicyNumber 'use PolicyNumber instead of QuoteNumber
                            l.Text &= "<br />Premium:  " & qq.TotalQuotedPremium
                            ph.Controls.Add(l)
                        End If
                    End If
                Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                    hasLobControl = True

                    If addToPlaceholder = True Then
                        AddPageBreakToPlaceholder(ph)

                        If ConfigurationManager.AppSettings("QuickQuote_CAP_ProposalControlPath") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_CAP_ProposalControlPath").ToString <> "" Then
                            Dim CAP As uc_VRProposal_CAP = webPage.LoadControl(ConfigurationManager.AppSettings("QuickQuote_CAP_ProposalControlPath").ToString)
                            CAP.QO = qq
                            ph.Controls.Add(CAP)
                        Else
                            Dim l As New Label
                            l.Text = "<br /><b>CAP Quote:  </b>" & qq.PolicyNumber 'use PolicyNumber instead of QuoteNumber
                            l.Text &= "<br />Premium:  " & qq.TotalQuotedPremium
                            ph.Controls.Add(l)
                        End If
                    End If
                Case QuickQuoteObject.QuickQuoteLobType.CommercialGarage 'un-commented 5/15/2017
                    hasLobControl = True

                    If addToPlaceholder = True Then
                        AddPageBreakToPlaceholder(ph)

                        If ConfigurationManager.AppSettings("QuickQuote_GAR_ProposalControlPath") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_GAR_ProposalControlPath").ToString <> "" Then
                            'Dim GAR As Controls_Proposal_GAR_Velocirater_Proposal_GAR_Control = webPage.LoadControl(ConfigurationManager.AppSettings("QuickQuote_GAR_ProposalControlPath").ToString)
                            'updated 5/15/2017 for correct control name
                            Dim GAR As uc_VRProposal_GAR = webPage.LoadControl(ConfigurationManager.AppSettings("QuickQuote_GAR_ProposalControlPath").ToString)
                            GAR.QO = qq
                            ph.Controls.Add(GAR)
                        Else
                            Dim l As New Label
                            l.Text = "<br /><b>GAR Quote:  </b>" & qq.PolicyNumber 'use PolicyNumber instead of QuoteNumber
                            l.Text &= "<br />Premium:  " & qq.TotalQuotedPremium
                            ph.Controls.Add(l)
                        End If
                    End If
                Case QuickQuoteObject.QuickQuoteLobType.CommercialInlandMarine 'added 5/15/2017
                    hasLobControl = True

                    If addToPlaceholder = True Then
                        AddPageBreakToPlaceholder(ph)

                        If ConfigurationManager.AppSettings("QuickQuote_CIM_ProposalControlPath") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_CIM_ProposalControlPath").ToString <> "" Then
                            Dim CIM As Controls_Proposal_CIM_Velocirater_Proposal_CIM_Control = webPage.LoadControl(ConfigurationManager.AppSettings("QuickQuote_CIM_ProposalControlPath").ToString)
                            CIM.QO = qq
                            ph.Controls.Add(CIM)
                        Else
                            Dim l As New Label
                            l.Text = "<br /><b>CIM Quote:  </b>" & qq.PolicyNumber 'use PolicyNumber instead of QuoteNumber
                            l.Text &= "<br />Premium:  " & qq.TotalQuotedPremium
                            ph.Controls.Add(l)
                        End If
                    End If
                Case QuickQuoteObject.QuickQuoteLobType.CommercialCrime 'added 5/15/2017
                    hasLobControl = True

                    If addToPlaceholder = True Then
                        AddPageBreakToPlaceholder(ph)

                        If ConfigurationManager.AppSettings("QuickQuote_CRM_ProposalControlPath") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_CRM_ProposalControlPath").ToString <> "" Then
                            Dim CRM As Controls_Proposal_CRM_Velocirater_Proposal_CRM_Control = webPage.LoadControl(ConfigurationManager.AppSettings("QuickQuote_CRM_ProposalControlPath").ToString)
                            CRM.QO = qq
                            ph.Controls.Add(CRM)
                        Else
                            Dim l As New Label
                            l.Text = "<br /><b>CRM Quote:  </b>" & qq.PolicyNumber 'use PolicyNumber instead of QuoteNumber
                            l.Text &= "<br />Premium:  " & qq.TotalQuotedPremium
                            ph.Controls.Add(l)
                        End If
                    End If
                Case Else
                    'nothing available for this lob
                    hasLobControl = False 'redundant
            End Select
        End If
    End Sub

    'added 7/11/2017
    Public Function OkayToSkipProposalLobDetailsForTesting() As Boolean
        Dim isOkay As Boolean = False

        Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
        If QuickQuote.CommonMethods.QuickQuoteHelperClass.IsTestEnvironment = True Then
            isOkay = qqHelper.BitToBoolean(QuickQuote.CommonMethods.QuickQuoteHelperClass.configAppSettingValueAsString("QuickQuote_Proposal_OkayToSkipProposalLobDetailsForTesting"))
        End If

        Return isOkay
    End Function
    Public Function ShowGenericPaymentOptions() As Boolean
        Dim showIt As Boolean = False

        Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
        showIt = qqHelper.BitToBoolean(QuickQuote.CommonMethods.QuickQuoteHelperClass.configAppSettingValueAsString("QuickQuote_Proposal_ShowGenericPaymentOptions"))

        Return showIt
    End Function
    Public Function ShowClientAndAgencyInfoForGenericPaymentOptions() As Boolean
        Dim showIt As Boolean = False

        Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
        showIt = qqHelper.BitToBoolean(QuickQuote.CommonMethods.QuickQuoteHelperClass.configAppSettingValueAsString("QuickQuote_Proposal_ShowClientAndAgencyInfoForGenericPaymentOptions"))

        Return showIt
    End Function
    'added 9/16/2017
    Public Function TryActualPaymentOptionsFirst() As Boolean
        Dim tryIt As Boolean = False

        Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
        tryIt = qqHelper.BitToBoolean(QuickQuote.CommonMethods.QuickQuoteHelperClass.configAppSettingValueAsString("QuickQuote_Proposal_TryActualPaymentOptionsFirst"))

        Return tryIt
    End Function
    'added 10/2/2017
    Public Function ActualPaymentOptions_DescriptionTextToIgnore() As List(Of String)
        Dim listOfTxtToIgnore As List(Of String) = Nothing

        Dim strTxtToIgnore As String = QuickQuote.CommonMethods.QuickQuoteHelperClass.configAppSettingValueAsString("QuickQuote_Proposal_ActualPaymentOptions_DescriptionTextToIgnore")
        If String.IsNullOrWhiteSpace(strTxtToIgnore) = False Then
            listOfTxtToIgnore = QuickQuote.CommonMethods.QuickQuoteHelperClass.ListOfStringFromString(strTxtToIgnore, delimiter:="|", returnPairForEachDelimiter:=False)
        End If

        Return listOfTxtToIgnore
    End Function
    Public Function ActualPaymentOptions_SortBy() As QuickQuotePaymentOption.SortBy
        Dim sortBy As QuickQuotePaymentOption.SortBy = QuickQuotePaymentOption.SortBy.None

        Dim strSortBy As String = QuickQuote.CommonMethods.QuickQuoteHelperClass.configAppSettingValueAsString("QuickQuote_Proposal_ActualPaymentOptions_SortBy")

        Return sortBy
    End Function
    Public Function ActualPaymentOptions_FilteredAndSorted(ByRef pmtOptions As List(Of QuickQuotePaymentOption)) As List(Of QuickQuotePaymentOption)
        Dim filteredAndSortPmtOptions As List(Of QuickQuotePaymentOption) = Nothing

        If pmtOptions IsNot Nothing AndAlso pmtOptions.Count > 0 Then
            filteredAndSortPmtOptions = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePaymentOptionsWithoutMatchingTextInDescription(pmtOptions, ActualPaymentOptions_DescriptionTextToIgnore())
            If filteredAndSortPmtOptions IsNot Nothing AndAlso filteredAndSortPmtOptions.Count > 0 Then
                Dim sortBy As QuickQuotePaymentOption.SortBy = ActualPaymentOptions_SortBy()
                If sortBy <> Nothing AndAlso sortBy <> QuickQuotePaymentOption.SortBy.None Then
                    Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
                    qqHelper.SortPaymentOptions(filteredAndSortPmtOptions, sortBy:=sortBy, backupListPositionSortBy:=QuickQuotePaymentOption.ListPositionSortBy.Ascending)
                End If
            End If
        End If

        Return filteredAndSortPmtOptions
    End Function

    Public Shared Function Find_First_PolicyLevelCoverage(quoteList As List(Of QuickQuoteObject), coverageCodeId As String) As QuickQuoteCoverage
        'check parameters
        If quoteList Is Nothing Then Throw New ArgumentNullException("quoteList")
        If String.IsNullOrWhiteSpace(coverageCodeId) OrElse Integer.TryParse(coverageCodeId, 0) = False Then
            Throw New ArgumentException("The supplied coverage code id is not a valid value or is an empty string", "coverageCodeId")
        End If

        Dim found As Boolean = False
        Dim retval As QuickQuoteCoverage = Nothing

        '' this is faster than foreach or equivalent LINQ expression because it doesn't incur the overhead of enumerators
        '' the Find function predates linq and uses a regular for loop internally - it is used for readability and conciseness here
        If quoteList.Any() Then
            Dim indx = 0
            Do
                If quoteList(indx).PolicyCoverages IsNot Nothing Then
                    retval = quoteList(indx).PolicyCoverages.Find(Function(c As QuickQuoteCoverage) c.CoverageCodeId = coverageCodeId)
                End If
                indx += 1
            Loop While retval Is Nothing AndAlso indx < quoteList.Count
        End If
        Return retval
    End Function
    Public Shared Function Find_First_PackageLevelCoverage(packagePart As QuickQuotePackagePart, coverageCodeId As String) As QuickQuoteCoverage
        'check parameters
        If packagePart Is Nothing Then Throw New ArgumentNullException("packagePart")
        If String.IsNullOrWhiteSpace(coverageCodeId) OrElse Integer.TryParse(coverageCodeId, 0) = False Then
            Throw New ArgumentException("The supplied coverage code id is not a valid value or is an empty string", "coverageCodeId")
        End If

        Dim found As Boolean = False
        Dim retval As QuickQuoteCoverage = Nothing

        '' this is faster than foreach or equivalent LINQ expression because it doesn't incur the overhead of enumerators
        '' the Find function predates linq and uses a regular for loop internally - it is used for readability and conciseness here
        If packagePart.Coverages IsNot Nothing Then
            retval = packagePart.Coverages.Find(Function(c As QuickQuoteCoverage) c.CoverageCodeId = coverageCodeId)
        End If

        Return retval
    End Function

    ''' <summary>
    ''' This method works similarly to TryToGetDouble except that it is parsing a Decimal value
    ''' instead..
    ''' </summary>
    ''' <param name="inputText">The input text to process.</param>
    ''' <param name="setToZeroIfNegative">If true this method will return 0 if the parsed value is negative.</param>
    ''' <returns>A parsed integer value if possible.</returns>
    Public Shared Function TryToGetDec(ByVal inputText As String, Optional setToZeroIfNegative As Boolean = False) As Decimal
        Try
            inputText = inputText.Replace("$", "")
            inputText = inputText.Replace(",", "")
            Dim val As Decimal = CDec(inputText)
            If setToZeroIfNegative AndAlso val < 0.0 Then
                Return 0
            End If
            Return val
        Catch ex As Exception
        End Try
        Return 0
    End Function
    Public Shared Function UpdateRCCTextDate() As DateTime
        Dim cutoverDate As String = ConfigurationManager.AppSettings("Remove_RccBillingPayPlanCalendarDate")
        If cutoverDate <> "" Then
            Dim dateResult As Date
            If Date.TryParse(cutoverDate, dateResult) Then
                Return dateResult
            End If
        End If
        Return Date.MaxValue
    End Function

    Public Shared Function IsRccPolicy(quote As QuickQuoteObject) As Boolean
        Dim rccPayPlan As Boolean = False
            If quote IsNot Nothing Then
                If quote.CurrentPayplanId = "18" OrElse quote.BillingPayPlanId = "18" Then
                    rccPayPlan = True
                End If
            End If
            Return rccPayPlan
    End Function

End Class
