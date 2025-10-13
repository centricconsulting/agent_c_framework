Imports IFM.VR.Web.Helpers
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports System.Data
Imports System.Xml
Imports System.Diagnostics
Imports System.IO
Imports System.Threading
Imports Common.Helpers.HOM
Imports IFM.VR.Common.Helpers.HOM
Imports IFM.PrimativeExtensions

Public Class PFQuoteSummary_HOM
    Inherits System.Web.UI.Page

    Protected ReadOnly Property HomeVersion As String
        Get
            Dim effectiveDate As DateTime
            If Me.quickQuote IsNot Nothing Then
                If Me.quickQuote.EffectiveDate IsNot Nothing AndAlso Me.quickQuote.EffectiveDate <> String.Empty Then
                    effectiveDate = Me.quickQuote.EffectiveDate
                Else
                    effectiveDate = Now()
                End If
            Else
                effectiveDate = Now()
            End If
            If qqHelper.doUseNewVersionOfLOB(quickQuote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                Return "After20180701"
            Else
                Return "Before20180701"
            End If
        End Get
    End Property

    Dim quickQuote As QuickQuote.CommonObjects.QuickQuoteObject
    Dim QQxml As New QuickQuote.CommonMethods.QuickQuoteXML
    Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
    Dim summaryHelper As New SummaryHelperClass
    Dim fileName As String = ""
    Dim quoteIds As List(Of String)
    Dim noSurchargesExist As Boolean = True

    Public Property RowNumber As Int32
        Get
            If ViewState("vs_rowNumber") Is Nothing Then
                ViewState("vs_rowNumber") = 0
            End If
            Return CInt(ViewState("vs_rowNumber"))
        End Get
        Set(value As Int32)
            ViewState("vs_rowNumber") = value
        End Set
    End Property

    Public ReadOnly Property CurrentForm As String
        Get
            ''11/17/17 added for HOM Upgrade MLW, was just else statement
            'If Session("homeVersion") = "After20180701" Then
            '    Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, quickQuote.Locations(0).FormTypeId).Substring(0, 7)
            'Else
            '    Return qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, quickQuote.Locations(0).FormTypeId).Substring(0, 4)
            'End If

            'Updated 11/27/17 for HOM Upgrade MLW
            Try
                Return qqHelper.GetShortFormName(quickQuote)
            Catch ex As Exception
                Return ""
            End Try

        End Get
    End Property

    'Added 7/30/2019 for Home Endorsements Task 38934 MLW
    Public ReadOnly Property IsBillingUpdate() As Boolean
        Get
            Dim result As Boolean = False
            If Request IsNot Nothing AndAlso Request.QueryString IsNot Nothing AndAlso Request.QueryString("isBillingUpdate") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.QueryString("isBillingUpdate").ToString) = False Then
                result = CBool(Request.QueryString("isBillingUpdate").ToString)
            ElseIf Page IsNot Nothing AndAlso Page.RouteData IsNot Nothing AndAlso Page.RouteData.Values IsNot Nothing AndAlso Page.RouteData.Values("isBillingUpdate") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Page.RouteData.Values("isBillingUpdate").ToString) = False Then
                result = CBool(Page.RouteData.Values("isBillingUpdate").ToString)
            Else
                If quickQuote IsNot Nothing AndAlso quickQuote.IsBillingEndorsement = True Then
                    result = True
                End If
            End If
            Return result
        End Get

    End Property

    'added 2/21/2019
    Public ReadOnly Property QuoteId As String
        Get
            If Request.QueryString("quoteid") IsNot Nothing Then
                Return Request.QueryString("quoteid")
            End If
            If Page.RouteData.Values("quoteid") IsNot Nothing Then
                Return Page.RouteData.Values("quoteid").ToString()
            End If
            Return ""
        End Get
    End Property
    Private _EndorsementPolicyId As Integer = 0
    Public ReadOnly Property EndorsementPolicyId As Integer
        Get
            If _EndorsementPolicyId <= 0 Then
                LoadEndorsementPolicyIdAndImageNum()
            End If
            Return _EndorsementPolicyId
        End Get
    End Property
    Private _EndorsementPolicyImageNum As Integer = 0
    Public ReadOnly Property EndorsementPolicyImageNum As Integer
        Get
            If _EndorsementPolicyImageNum <= 0 Then
                LoadEndorsementPolicyIdAndImageNum()
            End If
            Return _EndorsementPolicyImageNum
        End Get
    End Property
    Public ReadOnly Property EndorsementPolicyIdAndImageNum As String
        Get
            Dim strEndorsementPolicyIdAndImageNum As String = ""
            If Request IsNot Nothing AndAlso Request.QueryString IsNot Nothing AndAlso Request.QueryString("EndorsementPolicyIdAndImageNum") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.QueryString("EndorsementPolicyIdAndImageNum").ToString) = False Then
                strEndorsementPolicyIdAndImageNum = Request.QueryString("EndorsementPolicyIdAndImageNum").ToString
            ElseIf Page IsNot Nothing AndAlso Page.RouteData IsNot Nothing AndAlso Page.RouteData.Values IsNot Nothing AndAlso Page.RouteData.Values("EndorsementPolicyIdAndImageNum") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Page.RouteData.Values("EndorsementPolicyIdAndImageNum").ToString) = False Then
                strEndorsementPolicyIdAndImageNum = Page.RouteData.Values("EndorsementPolicyIdAndImageNum").ToString
            End If
            Return strEndorsementPolicyIdAndImageNum
        End Get
    End Property
    Private Sub LoadEndorsementPolicyIdAndImageNum(Optional ByVal reset As Boolean = False)
        If reset = True Then
            _EndorsementPolicyId = 0
            _EndorsementPolicyImageNum = 0
        End If
        Dim strEndorsementPolicyIdAndImageNum As String = EndorsementPolicyIdAndImageNum
        If String.IsNullOrWhiteSpace(strEndorsementPolicyIdAndImageNum) = False Then
            Dim intList As List(Of Integer) = QuickQuoteHelperClass.ListOfIntegerFromString(strEndorsementPolicyIdAndImageNum, delimiter:="|", positiveOnly:=True)
            If intList IsNot Nothing AndAlso intList.Count > 0 Then
                _EndorsementPolicyId = intList(0)
                If intList.Count > 1 Then
                    _EndorsementPolicyImageNum = intList(1)
                End If
            End If
        End If
    End Sub
    Private _ReadOnlyPolicyId As Integer = 0
    Public ReadOnly Property ReadOnlyPolicyId As Integer
        Get
            If _ReadOnlyPolicyId <= 0 Then
                LoadReadOnlyPolicyIdAndImageNum()
            End If
            Return _ReadOnlyPolicyId
        End Get
    End Property
    Private _ReadOnlyPolicyImageNum As Integer = 0
    Public ReadOnly Property ReadOnlyPolicyImageNum As Integer
        Get
            If _ReadOnlyPolicyImageNum <= 0 Then
                LoadReadOnlyPolicyIdAndImageNum()
            End If
            Return _ReadOnlyPolicyImageNum
        End Get
    End Property
    Public ReadOnly Property ReadOnlyPolicyIdAndImageNum As String
        Get
            Dim strReadOnlyPolicyIdAndImageNum As String = ""
            If Request IsNot Nothing AndAlso Request.QueryString IsNot Nothing AndAlso Request.QueryString("ReadOnlyPolicyIdAndImageNum") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Request.QueryString("ReadOnlyPolicyIdAndImageNum").ToString) = False Then
                strReadOnlyPolicyIdAndImageNum = Request.QueryString("ReadOnlyPolicyIdAndImageNum").ToString
            ElseIf Page IsNot Nothing AndAlso Page.RouteData IsNot Nothing AndAlso Page.RouteData.Values IsNot Nothing AndAlso Page.RouteData.Values("ReadOnlyPolicyIdAndImageNum") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Page.RouteData.Values("ReadOnlyPolicyIdAndImageNum").ToString) = False Then
                strReadOnlyPolicyIdAndImageNum = Page.RouteData.Values("ReadOnlyPolicyIdAndImageNum").ToString
            End If
            Return strReadOnlyPolicyIdAndImageNum
        End Get
    End Property
    Private Sub LoadReadOnlyPolicyIdAndImageNum(Optional ByVal reset As Boolean = False)
        If reset = True Then
            _ReadOnlyPolicyId = 0
            _ReadOnlyPolicyImageNum = 0
        End If
        Dim strReadOnlyPolicyIdAndImageNum As String = ReadOnlyPolicyIdAndImageNum
        If String.IsNullOrWhiteSpace(strReadOnlyPolicyIdAndImageNum) = False Then
            Dim intList As List(Of Integer) = QuickQuoteHelperClass.ListOfIntegerFromString(strReadOnlyPolicyIdAndImageNum, delimiter:="|", positiveOnly:=True)
            If intList IsNot Nothing AndAlso intList.Count > 0 Then
                _ReadOnlyPolicyId = intList(0)
                If intList.Count > 1 Then
                    _ReadOnlyPolicyImageNum = intList(1)
                End If
            End If
        End If
    End Sub
    Dim _RatedQuote As QuickQuoteObject = Nothing
    Protected ReadOnly Property RatedQuote As QuickQuote.CommonObjects.QuickQuoteObject
        Get
            If _RatedQuote Is Nothing Then
                Dim errCreateQSO As String = ""
                If String.IsNullOrWhiteSpace(ReadOnlyPolicyIdAndImageNum) = False Then
                    _RatedQuote = VR.Common.QuoteSave.QuoteSaveHelpers.GetReadOnlyQuickQuoteObjectForPolicyIdAndImageNum(Me.ReadOnlyPolicyId, Me.ReadOnlyPolicyImageNum, ratedView:=True, errorMessage:=errCreateQSO)
                ElseIf String.IsNullOrWhiteSpace(EndorsementPolicyIdAndImageNum) = False Then
                    _RatedQuote = VR.Common.QuoteSave.QuoteSaveHelpers.GetEndorsementQuoteForPolicyIdAndImageNum(Me.EndorsementPolicyId, Me.EndorsementPolicyImageNum, ratedView:=True, errorMessage:=errCreateQSO)
                Else
                    _RatedQuote = VR.Common.QuoteSave.QuoteSaveHelpers.GetRatedQuoteById_NOSESSION(Me.QuoteId, errCreateQSO, QuickQuoteObject.QuickQuoteLobType.None, Nothing)
                End If
            End If
            Return _RatedQuote
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            'Updated 10/30/2019 for code review 39343 MLW
            'Dim summaryType As String = Request.QueryString("summarytype").ToString
            Dim summaryType As String = If(Request.QueryString("summarytype") IsNot Nothing, Request.QueryString("summarytype").ToString, "")
            'Updated 7/15/2019 for Home Endorsements Project Task 38921 MLW
            If (RatedQuote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote) Then
                EndorsementPrintSection.Visible = True
                QuoteAppPrintSection.Visible = False
            Else
                EndorsementPrintSection.Visible = False
                QuoteAppPrintSection.Visible = True

                'Me.lblQuoteId.Text = Request.QueryString("QuoteId").ToString
                'updated 5/10/2019
                Me.lblQuoteId.Text = If(Request.QueryString("QuoteId") IsNot Nothing, Request.QueryString("QuoteId").ToString, "")
                Dim reportHeader As StringBuilder = New StringBuilder
                reportHeader.Append("Indiana Farmers Mutual Insurance Group ")

                If summaryType = "App" Then
                    reportHeader.Append("Application Summary")
                Else
                    reportHeader.Append("Quote Summary")
                End If

                lblHeader.Text = reportHeader.ToString
                ctlClientAndAgencyInfo.ToggleProducer(False)
            End If
            GetQuoteFromDb(summaryType)
            LoadSummaryObjects()
        End If
    End Sub

    Private Sub LoadSummaryObjects()
        If quickQuote IsNot Nothing Then 'added IF 2/21/2019
            If quickQuote.Applicants IsNot Nothing Then
                Dim applicantStr As StringBuilder = New StringBuilder() 
                Dim Name As String = String.Format("{0} {1} {2} {3}", quickQuote.Policyholder.Name.FirstName, quickQuote.Policyholder.Name.MiddleName, quickQuote.Policyholder.Name.LastName, quickQuote.Policyholder.Name.SuffixName).Replace("  ", " ").Trim()
                applicantStr.Append(Name + "<br />")
                If quickQuote?.Policyholder2?.Name?.DisplayNameForWeb IsNot Nothing AndAlso quickQuote.Policyholder2.Name.DisplayNameForWeb.IsNullEmptyorWhitespace() = False Then
                    Dim Name2 As String = String.Format("{0}", quickQuote.Policyholder2.Name.DisplayNameForWeb).Replace("  ", " ").Trim()
                    applicantStr.Append(Name2 + "<br />")
                End If
                Me.ctlClientAndAgencyInfo.ClientInfo = qqHelper.appendText(applicantStr.ToString().Substring(0, applicantStr.Length - 6), quickQuote.Policyholder.Address.DisplayAddressForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), "<br />")
                Me.ctlClientAndAgencyInfo.ClientInfo = qqHelper.appendText(Me.ctlClientAndAgencyInfo.ClientInfo, quickQuote.Policyholder.PrimaryPhone, "<br />")
            End If

            Me.ctlClientAndAgencyInfo.AgencyInfo = qqHelper.appendText(summaryHelper.GetAgencyName(quickQuote.Agency.Name).Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), quickQuote.Agency.Address.DisplayAddressForWeb.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />"), "<br />")

            'Dim trimPrimaryPhone As String = ""
            'Try
            '    trimPrimaryPhone = quickQuote.Agency.PrimaryPhone.Trim.Remove(quickQuote.Agency.PrimaryPhone.Length - 2)
            'Catch ex As Exception
            '    trimPrimaryPhone = ""
            'End Try

            'Me.ctlClientAndAgencyInfo.AgencyInfo = qqHelper.appendText(Me.ctlClientAndAgencyInfo.AgencyInfo, trimPrimaryPhone, "<br />")
            'updated 9/19/2017
            Me.ctlClientAndAgencyInfo.AgencyInfo = qqHelper.appendText(Me.ctlClientAndAgencyInfo.AgencyInfo, quickQuote.Agency.PrimaryPhone, "<br />")
            Me.ctlClientAndAgencyInfo.ProducerCode = quickQuote.AgencyProducerCode

        End If
    End Sub

    Private Sub ShowCoverageA()
        Dim PremiumInt As Integer = 0
        Dim A_included As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).A_Dwelling_LimitIncluded)
        Dim A_increased As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).A_Dwelling_LimitIncreased)
        lblCovALimit.Text = (A_included + A_increased).ToString("N0")

        PremiumInt = quickQuote.Locations(0).A_Dwelling_QuotedPremium
        If PremiumInt <> 0 Then
            lblCovAPrem.Text = quickQuote.Locations(0).A_Dwelling_QuotedPremium
        Else
            lblCovAPrem.Text = "Included"
        End If
    End Sub

    Private Sub HideCoverageA()
        pnlCovADwelling.Visible = False
    End Sub

    Private Sub ShowCoverageB()
        Dim PremiumInt As Integer = 0
        Dim B_included As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).B_OtherStructures_LimitIncluded)
        Dim B_increased As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).B_OtherStructures_LimitIncreased)
        lblCovBLimit.Text = (B_included + B_increased).ToString("N0")

        PremiumInt = quickQuote.Locations(0).B_OtherStructures_QuotedPremium
        If PremiumInt <> 0 Then
            lblCovBPrem.Text = quickQuote.Locations(0).B_OtherStructures_QuotedPremium
        Else
            lblCovBPrem.Text = "Included"
        End If
    End Sub

    Private Sub HideCoverageB()
        pnlCovBStruct.Visible = False
    End Sub

    Private Sub ShowCoverageC()
        Dim PremiumInt As Integer = 0
        Dim C_included As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).C_PersonalProperty_LimitIncluded)
        Dim C_increased As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).C_PersonalProperty_LimitIncreased)
        lblCovCLimit.Text = (C_included + C_increased).ToString("N0")

        PremiumInt = quickQuote.Locations(0).C_PersonalProperty_QuotedPremium
        If PremiumInt <> 0 Then
            lblCovCPrem.Text = quickQuote.Locations(0).C_PersonalProperty_QuotedPremium
        Else
            lblCovCPrem.Text = "Included"
        End If
    End Sub

    Private Sub ShowCoverageD(formNum As String)
        If HomeVersion <> "After20180701" Then ' Matt A 5-4-18
            Dim PremiumInt As Integer = 0
            Dim D_included As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).D_LossOfUse_LimitIncluded)
            Dim D_increased As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(quickQuote.Locations(0).D_LossOfUse_LimitIncreased)

            If formNum <> "ML-2" And formNum <> "ML-4" Then
                lblCovDLimit.Text = "See <sup>2</sup>"
                lblCovDPrem.Text = "Included"
            Else
                lblCovDLimit.Text = (D_included + D_increased).ToString("N0")

                PremiumInt = quickQuote.Locations(0).D_LossOfUse_QuotedPremium
                If PremiumInt <> 0 Then
                    lblCovDPrem.Text = quickQuote.Locations(0).D_LossOfUse_QuotedPremium
                Else
                    lblCovDPrem.Text = "Included"
                End If
            End If
        Else
            lblCovDLimit.Text = "See <sup>2</sup>"
            lblCovDPrem.Text = "Included"
        End If

    End Sub

    Private Sub FillLiteral(literal As Literal, list As List(Of SummaryCoverageLineItem))
        Dim sb As New StringBuilder()
        sb.AppendLine("<table class=""table"" style=""width:100%"">")
        For Each covItem In list
            sb.AppendLine("<tr>")
            sb.AppendLine("<td style=""width:70%"">")
            sb.AppendLine(If(covItem.NameIsBold, "<b>", "") + If(covItem.IsSubItem, "<div style=""margin-left:15px;"">" + covItem.Name + "</div>", covItem.Name) + If(covItem.NameIsBold, "</b>", ""))
            sb.AppendLine("</td>")

            sb.AppendLine("<td>")

            sb.AppendLine("<table style=""width: 100%"" class=""table"">")
            sb.AppendLine("<tr>")
            sb.AppendLine("<td style=""width: 41.5%;text-align: right; vertical-align:top;"">")
            sb.AppendLine(covItem.Limit)
            sb.AppendLine("</td>")
            sb.AppendLine("<td style=""text-align: right; vertical-align:top;"">")
            sb.AppendLine(covItem.Premium)
            sb.AppendLine("</td>")
            sb.AppendLine("</tr>")
            sb.AppendLine("</table>")

            sb.AppendLine("</td>")
            sb.AppendLine("</tr>")
        Next
        sb.AppendLine("</table>")
        literal.Text = sb.ToString()
    End Sub

    Private Sub AppendCoverageWithIndent(ByRef coverageList As List(Of SummaryCoverageLineItem), ByVal coverageName As String, ByVal limit As String, ByVal premium As String)
        Dim newCoverage As New SummaryCoverageLineItem With {
        .Name = coverageName,
        .Limit = limit,
        .Premium = premium,
        .IsSubItem = True ' Indicating this is a sub-item/child
    }
        coverageList.Add(newCoverage)
    End Sub


    Public Sub GetQuoteFromDb(summaryType As String)
        Dim valItemsMsg As String = "" 'added 4/11/2013 to use w/ Route To UW
        Dim val_item_counter As Integer = 0 'added 4/11/2013
        Dim noPolicyDiscounts As Boolean = True
        Dim policyCount As Integer = 25
        Dim inclCount As Integer = 2
        Dim propCount As Integer = 2
        Dim liabCount As Integer = 4
        Dim imCount As Integer = 4
        Dim rvwCount As Integer = 5
        Dim surCount As Integer = 5
        Dim creditCount As Integer = 0

        'If IsNumeric(Me.lblQuoteId.Text) = True Then 'removed IF 2/21/2019
        'Dim errorMsg As String = ""
        'Dim rateType As QuickQuoteXML.QuickQuoteSaveType = Nothing
        'QQxml.GetRatedQuote(Me.lblQuoteId.Text, quickQuote, rateType, errorMsg)
        'updated 2/21/2019; note: rateType should be set by method, though none of these have the param as ByRef
        quickQuote = RatedQuote
        'If quickQuote.Locations IsNot Nothing Then
        'updated 2/21/2019
        If quickQuote IsNot Nothing AndAlso quickQuote.Locations IsNot Nothing AndAlso quickQuote.Locations.Count > 0 Then
            With quickQuote
                'Policy Information
                Me.lblQuoteNumber.Text = .QuoteNumber
                Me.lblDate.Text = DateTime.Now.ToShortDateString()
                Me.lblEffectiveDate.Text = .EffectiveDate
                Me.lblTier.Text = .TieringInformation.RatedTier
                Me.lblPremium.Text = .TotalQuotedPremium

                'Added 7/16/2019 for Home Endorsements Project Task 38921 MLW
                Dim loadEndorsementReadOnlyHeader As Boolean = False
                Select Case .QuoteTransactionType
                    Case QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage
                        If String.IsNullOrWhiteSpace(.PolicyNumber) = False AndAlso Left(UCase(.PolicyNumber), 1) = "Q" Then
                            Me.lblQuoteNumber.Text = .QuoteNumber
                        Else
                            Me.lblQuoteNumber.Text = .PolicyNumber
                            If String.IsNullOrWhiteSpace(Me.lblQuoteNo.Text) = False AndAlso Len(Me.lblQuoteNo.Text) >= 5 AndAlso Left(Me.lblQuoteNo.Text, 5) = "Quote" Then
                                Me.lblQuoteNo.Text = Me.lblQuoteNo.Text.Replace("Quote", "Policy")
                            End If
                        End If
                        Me.lblEffectiveDate.Text = .TransactionEffectiveDate
                        If String.IsNullOrWhiteSpace(Me.lblEffective.Text) = False AndAlso Len(Me.lblEffective.Text) >= 8 AndAlso Left(Me.lblEffective.Text, 8) = "Proposed" Then
                            Me.lblEffective.Text = Me.lblEffective.Text.Replace("Proposed", "Image")
                        End If
                        loadEndorsementReadOnlyHeader = True
                    Case QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote
                        Me.lblQuoteNumber.Text = .PolicyNumber
                        If String.IsNullOrWhiteSpace(Me.lblQuoteNo.Text) = False AndAlso Len(Me.lblQuoteNo.Text) >= 5 AndAlso Left(Me.lblQuoteNo.Text, 5) = "Quote" Then
                            Me.lblQuoteNo.Text = Me.lblQuoteNo.Text.Replace("Quote", "Policy")
                        End If
                        Me.lblEffectiveDate.Text = .TransactionEffectiveDate
                        If String.IsNullOrWhiteSpace(Me.lblEffective.Text) = False AndAlso Len(Me.lblEffective.Text) >= 8 AndAlso Left(Me.lblEffective.Text, 8) = "Proposed" Then
                            Me.lblEffective.Text = Me.lblEffective.Text.Replace("Proposed", "Change")
                        End If
                        loadEndorsementReadOnlyHeader = True
                    Case Else
                        Me.lblQuoteNumber.Text = .QuoteNumber
                        Me.lblEffectiveDate.Text = .EffectiveDate
                End Select
                If loadEndorsementReadOnlyHeader = True Then
                    'Me.OldHeaderSection.Visible = False
                    'Me.OldQuoteNumberSection.Visible = False
                    'Me.OldClientAndAgencySection.Visible = False
                    'Me.OldPremSection.Visible = False
                    'Me.OldEffDateSection.Visible = False
                    Me.ctlEndorsementOrChangeHeader.Visible = True
                    Me.ctlEndorsementOrChangeHeader.Quote = quickQuote
                End If

                'Updated 7/30/2019 for Home Endorsements Task 38934 MLW
                If IsBillingUpdate() Then
                    quoteSummaryDetailsContent.Visible = False
                Else


                    If .Locations(0) IsNot Nothing Then
                        'Policy Coverages
                        lblResidenceData.Text = String.Format("{0} {1}", .Locations(0).Address.HouseNum, .Locations(0).Address.StreetName)
                        'Updated 12/5/17 for HOM Upgrade MLW
                        If ((.Locations(0).FormTypeId = "22" Or .Locations(0).FormTypeId = "25") And .Locations(0).StructureTypeId = "2") Then
                            Dim optionAttributes As New List(Of QuickQuoteStaticDataAttribute)
                            Dim a1 As New QuickQuoteStaticDataAttribute
                            a1.nvp_name = "StructureTypeId" 'only way to determine mobile types on the new form types is by having StructureTypeId set to 2. Therefore, only use this to get the formType description/name for mobile on new forms. MLW
                            a1.nvp_value = 2
                            optionAttributes.Add(a1)
                            lblFormData.Text = qqHelper.GetStaticDataTextForValue_MatchingOptionAttributes(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, optionAttributes, .Locations(0).FormTypeId)
                        Else
                            lblFormData.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, .Locations(0).FormTypeId)
                        End If
                        If qqHelper.doUseNewVersionOfLOB(quickQuote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                            lblOccupancyCodeData.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.OccupancyCodeId, .Locations(0).OccupancyCodeId)
                        Else
                            trOccupancyCode.Style.Add("display", "none")
                        End If
                        lblDeductibleData.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleLimitId, .Locations(0).DeductibleLimitId)

                        Dim windHail As String = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.WindHailDeductibleLimitId, .Locations(0).WindHailDeductibleLimitId)

                        If CurrentForm = "HO-2" Or CurrentForm = "HO-3" Then
                            policyCount += 1
                            If windHail = "N/A" Then
                                windHail = lblDeductibleData.Text
                            End If
                        Else
                            lblWindHail.Visible = False
                            lblWindHailData.Visible = False
                        End If

                        lblWindHailData.Text = windHail

                        'Added 10/24/2019 for bug 39802 MLW
                        Dim constructionType As String = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.ConstructionTypeId, .Locations(0).ConstructionTypeId)
                        lblConstructionData.Text = constructionType

                        'Base Coverages
                        'Select Case lblFormData.Text.Substring(0, 4)
                        Select Case CurrentForm
                            Case "HO-4"
                                HideCoverageA()
                                HideCoverageB()
                                ShowCoverageC()
                                lblCovCSup.Visible = True
                                ShowCoverageD(CurrentForm)
                                policyCount += 2
                            Case "HO-6"
                                ShowCoverageA()
                                HideCoverageB()
                                ShowCoverageC()
                                ShowCoverageD(CurrentForm)
                                policyCount += 3
                            Case "ML-2"
                                ShowCoverageA()
                                ShowCoverageB()
                                ShowCoverageC()
                                ShowCoverageD(CurrentForm)
                                policyCount += 4
                            Case "ML-4"
                                HideCoverageA()
                                HideCoverageB()
                                ShowCoverageC()
                                ShowCoverageD(CurrentForm)
                                policyCount += 2
                            Case Else
                                ShowCoverageA()
                                ShowCoverageB()
                                ShowCoverageC()
                                ShowCoverageD(CurrentForm)
                                policyCount += 4
                        End Select

                        'Added 4/10/18 for HOM Uprade MLW 'updated 4/30/18 MLW
                        If qqHelper.doUseNewVersionOfLOB(quickQuote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                            If quickQuote.MinimumPremiumAdjustment IsNot Nothing Then
                                Dim minPremAdjPrem As Integer = quickQuote.MinimumPremiumAdjustment
                                If minPremAdjPrem <> 0 AndAlso quickQuote.MinimumPremiumAdjustment <> "" Then
                                    pnlMinPremAdj.Visible = True
                                    lblMinPremAdjLimit.Text = "N/A"
                                    lblMinPremAdjPrem.Text = quickQuote.MinimumPremiumAdjustment
                                    policyCount += 1
                                Else
                                    pnlMinPremAdj.Visible = False
                                End If
                            Else
                                pnlMinPremAdj.Visible = False
                            End If
                        Else
                            pnlMinPremAdj.Visible = False
                        End If

                        FillLiteral(Me.litIncludedCoverages, IFM.VR.Common.Helpers.HOM.HOMSummaryHelper.GetIncludedCoverageList_Print(quickQuote))
                        inclCount += IFM.VR.Common.Helpers.HOM.HOMSummaryHelper.GetIncludedCoverageList_Print(quickQuote).Count


                        If FamilyCyberProtectionHelper.IsFamilyCyberProtectionAvailable(quickQuote) AndAlso quickQuote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "26") Then

                            Dim otherPropertyCoverages As List(Of SummaryCoverageLineItem) = IFM.VR.Common.Helpers.HOM.HOMSummaryHelper.GetOtherPropertyList_Print(quickQuote)
                            Dim combinedNewOtherPropertyCoverages As List(Of SummaryCoverageLineItem) = New List(Of SummaryCoverageLineItem)
                            
                            'Dim familyCyberProtectionCoverages As New List(Of SummaryCoverageLineItem)
                            For Each cov As SummaryCoverageLineItem In otherPropertyCoverages
                                 combinedNewOtherPropertyCoverages.Add(cov)
                                
                                ' Check if the current coverage is "Family Cyber Protection (HOM 1018)"
                                If cov.Name.Contains("Family Cyber Protection") Then

                                    ' Append each additional coverage under it
                                    AppendCoverageWithIndent(combinedNewOtherPropertyCoverages, "Social Engineering Coverage", "", "Included")
                                    AppendCoverageWithIndent(combinedNewOtherPropertyCoverages, "Cyber Bullying Coverage", "", "Included")
                                    AppendCoverageWithIndent(combinedNewOtherPropertyCoverages, "Identity Theft Coverage", "", "Included")
                                    AppendCoverageWithIndent(combinedNewOtherPropertyCoverages, "Internet Clean Up Coverage", "", "Included")
                                    AppendCoverageWithIndent(combinedNewOtherPropertyCoverages, "Breach Costs Coverage", "", "Included")
                                    AppendCoverageWithIndent(combinedNewOtherPropertyCoverages, "Online Extortion Coverage & Systems Compromise Coverage (combined)", "2,500", "Included")
                                End If
                            Next

                            FillLiteral(Me.litOtherPropertyCoverages, combinedNewOtherPropertyCoverages)
                            propCount += combinedNewOtherPropertyCoverages.Count
                        Else
                            FillLiteral(Me.litOtherPropertyCoverages, IFM.VR.Common.Helpers.HOM.HOMSummaryHelper.GetOtherPropertyList_Print(quickQuote))
                            propCount += IFM.VR.Common.Helpers.HOM.HOMSummaryHelper.GetOtherPropertyList_Print(quickQuote).Count
                        End If

                        FillLiteral(Me.litOtherLiabilityCoverages, IFM.VR.Common.Helpers.HOM.HOMSummaryHelper.GetLiabilityList_Print(quickQuote))
                        liabCount += IFM.VR.Common.Helpers.HOM.HOMSummaryHelper.GetLiabilityList_Print(quickQuote).Count

                        lblCovELimit.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.PersonalLiabilityLimitId, .PersonalLiabilityLimitId)

                        Dim covEPrem As Integer = .PersonalLiabilityQuotedPremium
                        If covEPrem <> 0 Then
                            lblCovEPrem.Text = .PersonalLiabilityQuotedPremium
                        Else
                            lblCovEPrem.Text = "Included"
                        End If

                        liabCount += 1

                        lblCovFLimit.Text = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.MedicalPaymentsLimitId, .MedicalPaymentsLimitid, quickQuote.LobType)

                        Dim covFPrem As Integer = .MedicalPaymentsQuotedPremium
                        If covFPrem <> 0 Then
                            lblCovFPrem.Text = .MedicalPaymentsQuotedPremium
                        Else
                            lblCovFPrem.Text = "Included"
                        End If

                        liabCount += 1

                    End If

                    '
                    ' Inland Marine
                    '
                    Dim totalIMPremium As String = "0"
                    Dim dtInlandMarine As New DataTable
                    dtInlandMarine.Columns.Add("Coverage", System.Type.GetType("System.String"))
                    dtInlandMarine.Columns.Add("Description", System.Type.GetType("System.String"))
                    dtInlandMarine.Columns.Add("Deductible", System.Type.GetType("System.String"))
                    dtInlandMarine.Columns.Add("Limits", System.Type.GetType("System.String"))
                    dtInlandMarine.Columns.Add("Premium", System.Type.GetType("System.String"))

                    ' IM Jewelry
                    InlandMarineData(quickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Jewelry), dtInlandMarine, totalIMPremium, imCount)

                    ' Jewelry in Vault
                    InlandMarineData(quickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.JewelryInVault), dtInlandMarine, totalIMPremium, imCount)

                    'Bicycles
                    InlandMarineData(quickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Bicycles), dtInlandMarine, totalIMPremium, imCount)

                    ' Cameras
                    InlandMarineData(quickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Cameras), dtInlandMarine, totalIMPremium, imCount)

                    ' Coins
                    InlandMarineData(quickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Coins), dtInlandMarine, totalIMPremium, imCount)

                    ' Collector's Items With Breakage
                    InlandMarineData(quickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.CollectorsItemsWithBreakage), dtInlandMarine, totalIMPremium, imCount)

                    ' Collector's Items W/O Breakage
                    InlandMarineData(quickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.CollectorsItemsWithoutBreakage), dtInlandMarine, totalIMPremium, imCount)

                    ' Computers
                    InlandMarineData(quickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Computer), dtInlandMarine, totalIMPremium, imCount)

                    ' Farm Machinery - Scheduled
                    InlandMarineData(quickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.FarmMachineryScheduled), dtInlandMarine, totalIMPremium, imCount)

                    ' Fine Arts - with breakage coverage
                    InlandMarineData(quickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_With_Breakage), dtInlandMarine, totalIMPremium, imCount)

                    ' Fine Arts - without breakage coverage
                    InlandMarineData(quickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_Without_Breakage), dtInlandMarine, totalIMPremium, imCount)

                    ' Furs
                    InlandMarineData(quickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Furs), dtInlandMarine, totalIMPremium, imCount)

                    ' Garden Tractors
                    InlandMarineData(quickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.GardenTractors), dtInlandMarine, totalIMPremium, imCount)

                    ' Golfers Equipment
                    InlandMarineData(quickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Golf), dtInlandMarine, totalIMPremium, imCount)

                    ' Grave Markers
                    InlandMarineData(quickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.GraveMarkers), dtInlandMarine, totalIMPremium, imCount)

                    ' Guns
                    InlandMarineData(quickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Guns), dtInlandMarine, totalIMPremium, imCount)

                    ' Hearing Aids
                    InlandMarineData(quickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.HearingAids), dtInlandMarine, totalIMPremium, imCount)

                    ' Medical Items and Equipment
                    InlandMarineData(quickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.MedicalItemsAndEquipment), dtInlandMarine, totalIMPremium, imCount)

                    ' Musical Instrument (Non-Professional)
                    InlandMarineData(quickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Musical_Instruments_Non_Professional), dtInlandMarine, totalIMPremium, imCount)

                    ' Silverware
                    InlandMarineData(quickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Miscellaneous_Class_I), dtInlandMarine, totalIMPremium, imCount)

                    ' Sports Equipment
                    InlandMarineData(quickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.InlandMarineSportsEquipment), dtInlandMarine, totalIMPremium, imCount)

                    ' Telephone - Car or Mobile
                    InlandMarineData(quickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.TelephonesCarOrMobile), dtInlandMarine, totalIMPremium, imCount)

                    ' Tools and Equipment
                    InlandMarineData(quickQuote.Locations(0).InlandMarines.FindAll(Function(p) p.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.InlandMarineToolsAndEquipment), dtInlandMarine, totalIMPremium, imCount)


                    If imCount > 4 Then
                        lblNoIMExist.Visible = False
                        pnlInlandMarine.Visible = True
                    Else
                        imCount += 1
                    End If

                    lblTotalIMPremData.Text = "$" + totalIMPremium

                    '
                    ' RV/Watercraft
                    '
                    Dim totalRVWPremium As String = "0"
                    Dim dtRVWatercraft As New DataTable
                    dtRVWatercraft.Columns.Add("Coverage", System.Type.GetType("System.String"))
                    dtRVWatercraft.Columns.Add("CoverageOpt", System.Type.GetType("System.String"))
                    dtRVWatercraft.Columns.Add("Deductible", System.Type.GetType("System.String"))
                    dtRVWatercraft.Columns.Add("Limits", System.Type.GetType("System.String"))
                    dtRVWatercraft.Columns.Add("Premium", System.Type.GetType("System.String"))

                    rvwCount += 3

                    RVWatercraftData(quickQuote.Locations(0).RvWatercrafts, dtRVWatercraft, totalRVWPremium, rvwCount)
                    lblTotalRVWatercraftPremData.Text = "$" + totalRVWPremium

                    If rvwCount > 8 Then
                        lblNoRVWaterExist.Visible = False
                        pnlRVWatercraft.Visible = True
                    Else
                        rvwCount += 1
                    End If

                    ' Determine Additional Amount needed to meet $25.00 minimum premium
                    If (Double.Parse(totalIMPremium) + Double.Parse(totalRVWPremium)) <> 0 Then
                        If (Double.Parse(totalIMPremium) + Double.Parse(totalRVWPremium)) < 25.0 Then
                            lblNoIMExist.Visible = False
                            pnlInlandMarine.Visible = True
                            pnlAdditionalPrem.Visible = True
                            lblMeetMinPrem.Text = "$" + (25.0 - (Double.Parse(totalIMPremium) + Double.Parse(totalRVWPremium))).ToString("N2")
                            If Double.Parse(totalIMPremium) <> 0.0 Then
                                lblTotalIMPremData.Text = (Double.Parse(lblMeetMinPrem.Text.Replace("$", "")) + Double.Parse(totalIMPremium)).ToString("N2")
                            Else
                                dgInlandMarine.Visible = False
                                lblTotalIMPremData.Text = lblMeetMinPrem.Text
                            End If

                            imCount += 1
                        End If
                    Else
                        lblNoIMExist.Visible = True
                        pnlInlandMarine.Visible = False
                        pnlAdditionalPrem.Visible = False
                    End If

                    '
                    ' Applied Credits
                    '
                    Dim dtCreditList As New DataTable
                    dtCreditList.Columns.Add("Credit", System.Type.GetType("System.String"))
                    dtCreditList.Columns.Add("CreditPercent", System.Type.GetType("System.String"))
                    creditCount = 4

                    Dim creditList = IFM.VR.Common.Helpers.HOM.HOMCreditFactors.GetPolicyDiscountsAsListOfPercents(quickQuote, True)

                    If creditList.Count <= 0 Then
                        lblNoCredits.Visible = True
                        creditCount += 1
                    Else
                        creditCount += creditList.Count
                    End If

                    For inx As Integer = 0 To creditList.Count - 1
                        Dim newCreditRow As DataRow = dtCreditList.NewRow

                        newCreditRow.Item("Credit") = creditList(inx).Key
                        newCreditRow.Item("CreditPercent") = creditList(inx).Value
                        dtCreditList.Rows.Add(newCreditRow)
                    Next

                    dgCreditList.DataSource = dtCreditList
                    dgCreditList.DataBind()

                    '
                    ' Applied Surcharges
                    '
                    Dim dtSurchargeList As New DataTable
                    dtSurchargeList.Columns.Add("Surcharge", System.Type.GetType("System.String"))
                    dtSurchargeList.Columns.Add("SurchargePercent", System.Type.GetType("System.String"))
                    surCount = 4

                    Dim surchargeList = IFM.VR.Common.Helpers.HOM.LossHistoryHelper_HOM.GetAllHOMSurcharges(quickQuote)

                    If surchargeList.Count <= 0 Then
                        lblNoSurchargeExist.Visible = True
                        surCount += 1
                    Else
                        surCount += surchargeList.Count
                    End If

                    For inx As Integer = 0 To surchargeList.Count - 1
                        Dim newSurchargeRow As DataRow = dtSurchargeList.NewRow

                        newSurchargeRow.Item("Surcharge") = surchargeList(inx).Key
                        newSurchargeRow.Item("SurchargePercent") = surchargeList(inx).Value
                        dtSurchargeList.Rows.Add(newSurchargeRow)
                    Next

                    dgSurchargeList.DataSource = dtSurchargeList
                    dgSurchargeList.DataBind()

                    '
                    ' Payment Plan
                    '
                    Dim totalPrem As String = .TotalQuotedPremium

                    If totalPrem <> "" AndAlso IsNumeric(totalPrem) = True Then
                        Me.ctlPaymentOptions.DirectAnnualDown = totalPrem
                        Me.ctlPaymentOptions.DirectMonthlyInstallFee = "3"
                        Me.ctlPaymentOptions.DirectSemiAnnualDown = qqHelper.getDivisionQuotient(totalPrem, "2")
                        qqHelper.ConvertToQuotedPremiumFormat(Me.ctlPaymentOptions.DirectSemiAnnualDown)
                        Me.ctlPaymentOptions.DirectQuarterlyDown = qqHelper.getDivisionQuotient(totalPrem, "4")
                        qqHelper.ConvertToQuotedPremiumFormat(Me.ctlPaymentOptions.DirectQuarterlyDown)
                        Me.ctlPaymentOptions.DirectMonthlyDown = qqHelper.getSum(qqHelper.getDivisionQuotient(totalPrem, "12"), Me.ctlPaymentOptions.DirectMonthlyInstallFee) 'may also need to include installment fee
                        qqHelper.ConvertToQuotedPremiumFormat(Me.ctlPaymentOptions.DirectMonthlyDown)
                        Me.ctlPaymentOptions.EFTMonthlyDown = qqHelper.getDivisionQuotient(totalPrem, "12")
                        qqHelper.ConvertToQuotedPremiumFormat(Me.ctlPaymentOptions.EFTMonthlyDown)
                        Me.ctlPaymentOptions.CreditDown = Me.ctlPaymentOptions.EFTMonthlyDown
                        Me.ctlPaymentOptions.RenewalCreditDown = Me.ctlPaymentOptions.EFTMonthlyDown
                        Me.ctlPaymentOptions.RenewalEftDown = Me.ctlPaymentOptions.EFTMonthlyDown
                        Me.ctlPaymentOptions.AnnualMtgDown = Me.ctlPaymentOptions.AnnualMtgDown

                        Me.ctlPaymentOptions.DirectAnnualRemainInstall = "N/A"
                        Me.ctlPaymentOptions.DirectSemiAnnualRemainInstall = "1"
                        Me.ctlPaymentOptions.DirectQuarterlyRemainInstall = "3"
                        Me.ctlPaymentOptions.DirectMonthlyRemainInstall = "11"
                        Me.ctlPaymentOptions.EFTMonthlyRemainInstall = Me.ctlPaymentOptions.DirectMonthlyRemainInstall
                        Me.ctlPaymentOptions.CreditRemainInstall = Me.ctlPaymentOptions.DirectMonthlyRemainInstall
                        Me.ctlPaymentOptions.RenewalCreditRemainInstall = Me.ctlPaymentOptions.DirectMonthlyRemainInstall
                        Me.ctlPaymentOptions.RenewalEftRemainInstall = Me.ctlPaymentOptions.DirectMonthlyRemainInstall
                        Me.ctlPaymentOptions.AnnualMtgRemainInstall = Me.ctlPaymentOptions.AnnualMtgRemainInstall

                        Me.ctlPaymentOptions.DirectAnnualBasicInstall = "N/A"
                        Me.ctlPaymentOptions.DirectSemiAnnualBasicInstall = Me.ctlPaymentOptions.DirectSemiAnnualDown
                        Me.ctlPaymentOptions.DirectQuarterlyBasicInstall = Me.ctlPaymentOptions.DirectQuarterlyDown
                        Me.ctlPaymentOptions.DirectMonthlyBasicInstall = Me.ctlPaymentOptions.DirectMonthlyDown 'may also need to include installment fee
                        qqHelper.ConvertToQuotedPremiumFormat(Me.ctlPaymentOptions.DirectMonthlyBasicInstall)
                        Me.ctlPaymentOptions.EFTMonthlyBasicInstall = Me.ctlPaymentOptions.EFTMonthlyDown
                        Me.ctlPaymentOptions.CreditBasicInstall = Me.ctlPaymentOptions.CreditDown
                        Me.ctlPaymentOptions.RenewalCreditBasicInstall = Me.ctlPaymentOptions.RenewalCreditDown
                        Me.ctlPaymentOptions.RenewalEftBasicInstall = Me.ctlPaymentOptions.RenewalEftDown
                        Me.ctlPaymentOptions.AnnualMtgInstallAmt = Me.ctlPaymentOptions.AnnualMtgInstallAmt
                    End If

                    PageBreaks(lblFormData.Text.Substring(0, 4), policyCount, inclCount, propCount, liabCount, imCount, rvwCount, creditCount, surCount)
                End If
            End With
        End If
        'End If

        'added 4/11/2013
        If valItemsMsg <> "" Then
            valItemsMsg = "Validation Item" & If(val_item_counter = 1, "", "s") & ":  " & valItemsMsg
            If ViewState.Item("valItemsMsg") Is Nothing Then
                ViewState.Add("valItemsMsg", valItemsMsg)
            Else
                ViewState.Item("valItemsMsg") = valItemsMsg
            End If
        End If
    End Sub

    Private Sub PageBreaks(form As String, policyCoverage As Integer, includedCoverage As Integer, otherPropCoverage As Integer, liabCoverage As Integer, imCoverage As Integer, rvwCoverage As Integer, creditCount As Integer, surCount As Integer)
        Dim linesOnPage As Integer = 54


        Dim formTotal As Integer = policyCoverage + includedCoverage + otherPropCoverage

        If formTotal > linesOnPage Then
            PropBreak.Visible = True
            PropLine.Visible = False
            formTotal = 0
        End If

        If formTotal = linesOnPage Then
            LiabBreak.Visible = True
            formTotal = 0
        End If

        formTotal += liabCoverage
        If formTotal >= linesOnPage Then
            LiabBreak.Visible = True
            formTotal = liabCoverage
        End If

        If formTotal = linesOnPage Then
            IMBreak.Visible = True
            formTotal = 0
        End If

        formTotal += imCoverage
        If quickQuote.Locations(0).InlandMarines.Count > 0 Then
            If formTotal > linesOnPage Then
                IMBreak.Visible = True
                imCoverage = formTotal - linesOnPage
                formTotal = imCoverage
            End If
        Else
            If (formTotal > linesOnPage) Or ((formTotal - linesOnPage) > imCoverage) Then
                IMBreak.Visible = True
                formTotal = 0
            End If
        End If

        If formTotal = linesOnPage Then
            RVWBreak.Visible = True
            formTotal = 0
        End If

        formTotal += rvwCoverage
        If quickQuote.Locations(0).RvWatercrafts.Count > 0 Then
            If formTotal > linesOnPage Then
                rvwCoverage = formTotal - linesOnPage

                If (rvwCoverage - 8) <= 0 And (rvwCoverage - 8) > -2 Then
                    RVWBreak.Visible = True
                End If

                formTotal = rvwCoverage
            End If

            If formTotal = linesOnPage Then
                AdjBreak.Visible = True
                formTotal = 0
            End If
        Else
            If (formTotal > linesOnPage) Or ((formTotal - linesOnPage) > rvwCoverage) Then
                RVWBreak.Visible = True
            End If
            formTotal = 0
        End If

        If surCount > creditCount Then
            formTotal += surCount
        Else
            formTotal += creditCount
        End If

        If formTotal > linesOnPage Then
            AdjBreak.Visible = True

            If surCount > creditCount Then
                formTotal = surCount
            Else
                formTotal = creditCount
            End If
        End If

        If formTotal = linesOnPage Then
            AdjBreak.Visible = True
            formTotal = 0
        End If

        formTotal += 3
        If formTotal > linesOnPage Then
            AdjBreak.Visible = True
        End If
    End Sub

    Private Sub RVWatercraftData(rvwDataSource As List(Of QuickQuoteRvWatercraft), ByRef dtRVWatercraft As DataTable, ByRef totalRVWPrem As String, ByRef lineCnt As Integer)

        Dim source = From RVItem In rvwDataSource
                     Select RVItem
                     Order By qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraft, QuickQuoteHelperClass.QuickQuotePropertyName.RvWatercraftTypeId, RVItem.RvWatercraftTypeId)

        For Each rvwItem As QuickQuoteRvWatercraft In source
            Dim newRVWRow As DataRow = dtRVWatercraft.NewRow
            Dim rvwCoverageOpt As New StringBuilder

            If rvwItem.HasLiability And Not rvwItem.HasLiabilityOnly Then
                rvwCoverageOpt.Append("Physical Damage,Liability")
            End If
            If rvwItem.HasLiability And rvwItem.HasLiabilityOnly Then
                rvwCoverageOpt.Append("Liability")
            End If
            If Not rvwItem.HasLiability And Not rvwItem.HasLiabilityOnly Then
                rvwCoverageOpt.Append("Physical Damage")
            End If

            If rvwItem.UninsuredMotoristBodilyInjuryLimitId <> "" And rvwItem.UninsuredMotoristBodilyInjuryLimitId <> "0" Then
                rvwCoverageOpt.Append(",UWBI")
            End If

            newRVWRow.Item("Coverage") = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraft, QuickQuoteHelperClass.QuickQuotePropertyName.RvWatercraftTypeId, rvwItem.RvWatercraftTypeId)
            newRVWRow.Item("CoverageOpt") = rvwCoverageOpt.ToString()
            newRVWRow.Item("Deductible") = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraft, QuickQuoteHelperClass.QuickQuotePropertyName.PropertyDeductibleLimitId, rvwItem.PropertyDeductibleLimitId)
            If rvwItem.RvWatercraftTypeId <> "3" Then
                newRVWRow.Item("Limits") = rvwItem.CostNew.Substring(0, rvwItem.CostNew.Length - 3).Replace("$", "")
            Else
                newRVWRow.Item("Limits") = rvwItem.RvWatercraftMotors.FirstOrDefault.CostNew.Substring(0, rvwItem.RvWatercraftMotors.FirstOrDefault.CostNew.Length - 3).Replace("$", "")
            End If
            newRVWRow.Item("Premium") = rvwItem.CoveragesPremium
            dtRVWatercraft.Rows.Add(newRVWRow)
            pnlRVWatercraft.Visible = True

            totalRVWPrem = (Decimal.Parse(totalRVWPrem.Replace(",", "")) + Decimal.Parse(rvwItem.CoveragesPremium.ToString().Replace("$", "").Replace(",", ""))).ToString()
            lineCnt += 1
        Next

        dgRVWatercraft.DataSource = dtRVWatercraft
        dgRVWatercraft.DataBind()
    End Sub

    Private Sub InlandMarineData(imDataSource As List(Of QuickQuoteInlandMarine), ByRef dtInlandMarine As DataTable, ByRef totalIMPrem As String, ByRef lineCnt As Integer)
        For Each imItem As QuickQuoteInlandMarine In imDataSource
            Dim newIMRow As DataRow = dtInlandMarine.NewRow

            Dim inlandType As String = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteInlandMarine, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, imItem.CoverageCodeId)
            inlandType = inlandType.Replace("_", " ").Replace("Inland Marine", "")

            Select Case inlandType
                Case " Miscellaneous Class I"
                    inlandType = "Silverware"
                Case "Medical Items & Equipment"
                    inlandType = "Medical Items and Equipment"
            End Select

            ''added 4/4/18 for HOM 2011 Upgrade MLW
            'If qqHelper.doUseNewVersionOfLOB(Quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
            '    If inlandType = " Golf" Then
            '        inlandType = " Golf (Excl Golf Carts)"
            '    End If
            'End If
            newIMRow.Item("Coverage") = inlandType
            newIMRow.Item("Description") = imItem.Description
            newIMRow.Item("Deductible") = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteInlandMarine, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleLimitId, imItem.DeductibleLimitId)
            newIMRow.Item("Limits") = imItem.IncreasedLimit
            newIMRow.Item("Premium") = imItem.CoveragePremium
            dtInlandMarine.Rows.Add(newIMRow)
            pnlInlandMarine.Visible = True

            totalIMPrem = (Decimal.Parse(totalIMPrem.Replace(",", "")) + Decimal.Parse(imItem.CoveragePremium.ToString().Replace("$", "").Replace(",", ""))).ToString()
            If imItem.Description.Length > 26 Then
                lineCnt += 2
            Else
                lineCnt += 1
            End If
        Next

        dgInlandMarine.DataSource = dtInlandMarine
        dgInlandMarine.DataBind()
    End Sub

    Protected Overrides Sub Render(writer As HtmlTextWriter)
        Dim sb As New StringBuilder()
        Using sw As New StringWriter(sb)
            Using htw As New HtmlTextWriter(sw)
                MyBase.Render(htw)
                writer.Write(sb.ToString())

                Dim htmlBytes As Byte() = Encoding.UTF8.GetBytes(sw.ToString)
                If htmlBytes IsNot Nothing Then
                    fileName = String.Format("SUMMARY{0}.pdf", Me.quickQuote.QuoteNumber)
                    Dim filePath As String = Server.MapPath(Request.ApplicationPath) & "\Reports\" & fileName & ".htm"
                    Dim fs As New FileStream(filePath, FileMode.Create)
                    fs.Write(htmlBytes, 0, htmlBytes.Length)
                    fs.Close()

                    If File.Exists(filePath) = True Then 'enclosed block in IF statement to make sure file exists
                        Dim status As String = ""
                        Dim pdfPath As String = Server.MapPath(Request.ApplicationPath) & "\Reports\" & fileName & ".pdf"

                        Try
                            RunExecutable(Server.MapPath(Request.ApplicationPath) & "\Reports\wkhtmltopdf\wkhtmltopdf.exe", """" & filePath & """ """ & pdfPath & """", status)

                            System.IO.File.Delete(filePath)
                            If File.Exists(pdfPath) = True Then
                                Dim fs_pdf As New FileStream(pdfPath, FileMode.Open, FileAccess.Read)
                                Dim pdfBytes As Byte() = New Byte(fs_pdf.Length - 1) {}
                                fs_pdf.Read(pdfBytes, 0, System.Convert.ToInt32(fs_pdf.Length))
                                fs_pdf.Close()
                                If pdfBytes IsNot Nothing Then

                                    Dim proposalId As String = ""
                                    Dim errorMsg As String = ""
                                    Dim successfulInsert As Boolean = False

                                    If errorMsg = "" Then
                                        Response.Clear()
                                        Response.ContentType = "application/pdf"
                                        Response.AddHeader("content-disposition", "attachment; filename=" + String.Format("SUMMARY{0}.pdf", Me.quickQuote.QuoteNumber))
                                        Response.BinaryWrite(pdfBytes)
                                        System.IO.File.Delete(pdfPath)
                                    End If
                                End If
                            End If
                        Catch ex As Exception

                        End Try
                    End If
                End If
            End Using
        End Using
    End Sub
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

        Dim process As Process = New Process()
        process.StartInfo = starter

        Dim compareTime As DateTime = DateAdd(DateInterval.Second, -5, Date.Now)

        process.Start()
        'updated to use variable
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
            status &= "Error<br><u>Output</u> - " & strOutput & "<br><u>Error</u> - " & strError

            'added 5/28/2013 for debugging on ifmwebtest (since it always works locally); seems to work okay after changing WaitForExit from 4000 to 10000 (4 seconds to 10 seconds)
            If ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_SendErrorEmail") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_SendErrorEmail").ToString <> "" AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_Proposal_PdfConverter_SendErrorEmail").ToString) = "YES" Then
                Dim eMsg As String = ""
                eMsg &= "<b>executable:</b>  " & executable
                eMsg &= "<br /><br />"
                eMsg &= "<b>arguments:</b>  " & arguments
                eMsg &= "<br /><br />"
                eMsg &= "<b>status:</b>  " & status
                qqHelper.SendEmail("ProposalPdfConverter@indianafarmers.com", "tbirkey@indianafarmers.com", "Error Converting Summary to PDF", eMsg)
            End If
        Else
            'ShowError("Success")
            status &= "Success<br><u>Output</u> - " & strOutput & "<br><u>Error</u> - " & strError
        End If

        process.Close()
        process.Dispose()
        process = Nothing
        starter = Nothing
    End Sub
End Class