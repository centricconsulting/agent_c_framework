Imports IFM.PrimativeExtensions
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.IO
Imports System.Globalization
Imports IFM.VR.Common.Helpers.MultiState.General
Imports IFM.VR.Common.Helpers.MultiState.Extensions

Public Class ctl_WCP_PFSummary
    Inherits VRControlBase

    ''' <summary>
    ''' Returns the KY effective date as a date object
    ''' </summary>
    ''' <returns></returns>
    'Private ReadOnly Property WCKYEffDate As DateTime
    '    Get
    '        If Not IsNothing(System.Configuration.ConfigurationManager.AppSettings("WC_KY_EffectiveDate")) Then
    '            If IsDate(System.Configuration.ConfigurationManager.AppSettings("WC_KY_EffectiveDate")) Then
    '                Return CDate(System.Configuration.ConfigurationManager.AppSettings("WC_KY_EffectiveDate"))
    '            Else
    '                Return DateTime.Now.AddDays(1)
    '            End If
    '        Else
    '            Return DateTime.Now.AddDays(1)
    '        End If
    '    End Get
    'End Property

    Dim indent As String = "&nbsp;&nbsp;"
    Dim dblindent As String = "&nbsp;&nbsp;&nbsp;&nbsp;"

    Private Structure Classification_struct
        Public qqClassification As QuickQuote.CommonObjects.QuickQuoteClassification
        Public qqState As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState
    End Structure

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion("divWCPQuoteSummary", HiddenField1, "0")
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub



    Public Overrides Sub Populate()
        Try
            If Me.Quote IsNot Nothing Then
                '-----------------------------------------------
                ' Temp Code until Rate
                '-----------------------------------------------
                '-----------------------------------------------
                'If IsOnAppPage Then
                '    Me.lblMainAccord.Text = String.Format("App Summary - Effective Date: {0} - 12 month", Me.Quote.EffectiveDate)
                'Else
                '    Me.lblMainAccord.Text = String.Format("Quote Summary - Effective Date: {0} - 12 month", Me.Quote.EffectiveDate)
                'End If
                'updated 5/10/2019; logic taken from updates for PPA
                Select Case Me.Quote.QuoteTransactionType
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage
                        Me.lblMainAccord.Text = If(Me.Quote.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote, "Change", "Image") & " Summary - Updated " & QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.PolicyTermId, Me.Quote.PolicyTermId) & " Premium"
                        Me.ImageDateAndPremChangeLine.Visible = True
                        If QQHelper.IsDateString(Me.Quote.TransactionEffectiveDate) = True Then
                            Me.lblTranEffDate.Text = "Effective Date: " & Me.Quote.TransactionEffectiveDate 'note: should already be in shortdate format
                        Else
                            Me.lblTranEffDate.Text = ""
                        End If
                        If QQHelper.IsNumericString(Me.Quote.ChangeInFullTermPremium) = True Then 'note: was originally looking for positive decimal, but the change in prem could be zero or negative
                            Me.lblAnnualPremChg.Text = "Annual Premium Change: " & Me.Quote.ChangeInFullTermPremium 'note: should already be in money format
                        Else
                            Me.lblAnnualPremChg.Text = ""
                        End If
                    Case Else
                        Me.lblMainAccord.Text = String.Format("{2} - Effective Date: {0} - {1}", Me.Quote.EffectiveDate, QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.PolicyTermId, Me.Quote.PolicyTermId), If(IsOnAppPage, "App", "Quote") & " Summary")
                        Me.ImageDateAndPremChangeLine.Visible = False
                End Select

                'Me.lblPremiumMainAccord.Text = Quote.Dec_WC_TotalPremiumDue
                'updated 11/20/2018 for multiState
                Me.lblPremiumMainAccord.Text = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.Dec_WC_TotalPremiumDue)

                'Dim ph1Name As String = String.Format("{0} {1} {2} {3}", Me.Quote.Policyholder.Name.FirstName, Me.Quote.Policyholder.Name.MiddleName, Me.Quote.Policyholder.Name.LastName, Me.Quote.Policyholder.Name.SuffixName).Replace("  ", " ").Trim()
                Me.lblPhName.Text = Me.Quote.Policyholder.Name.DisplayName
                If String.IsNullOrEmpty(Me.Quote.Policyholder.Name.DoingBusinessAsName) = False Then
                    lblPhName.Text = lblPhName.Text + " DBA " + Me.Quote.Policyholder.Name.DoingBusinessAsName
                End If

                Dim AddressOtherField As AddressOtherField = New AddressOtherField(Me.Quote.Policyholder.Address.Other)
                If AddressOtherField.PrefixType = Helpers.AddressOtherFieldPrefixHelper.OtherFieldPrefix.Other Then
                    Me.lblCareOf.Text = ""
                    Me.trCareOf.Visible = False
                Else
                    Me.lblCareOf.Text = AddressOtherField.NameWithPrefix
                    Me.trCareOf.Visible = True
                End If

                Me.lblQuoteNum.Text = Me.Quote.QuoteNumber

                Dim zip As String = Me.Quote.Policyholder.Address.Zip
                If zip.Length > 5 Then
                    zip = zip.Substring(0, 5)
                End If

                'house num, street, apt, pobox, city, state, zip
                Me.lblPhAddress.Text = String.Format("{0} {1} {2} {3} {4} {5} {6}", Me.Quote.Policyholder.Address.HouseNum, Me.Quote.Policyholder.Address.StreetName, If(String.IsNullOrWhiteSpace(Me.Quote.Policyholder.Address.ApartmentNumber) = False, "Apt# " + Me.Quote.Policyholder.Address.ApartmentNumber, ""), Me.Quote.Policyholder.Address.POBox, Me.Quote.Policyholder.Address.City, Me.Quote.Policyholder.Address.State, zip).Replace("  ", " ").Trim()

                Me.lblEffectiveDate.Text = Me.Quote.EffectiveDate
                Me.lblExpirationDate.Text = Me.Quote.ExpirationDate

                Me.lblFullPremium.Text = String.Format("{0:C2}", Me.Quote.TotalQuotedPremium)
                'Me.lblIndSecondInjurySurcharge.Text = Quote.SecondInjuryFundQuotedPremium
                'Me.lblEmployersLiability.Text = Quote.EmployersLiability
                'updated 11/20/2018 for multiState
                Me.lblIndSecondInjurySurcharge.Text = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.SecondInjuryFundQuotedPremium)
                Me.lblEmployersLiability.Text = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.EmployersLiability)

                PopulateLocationInformation()

                PopulateClassCodes()

                PopulateNamedIndividuals()

                PopulatePolicyCoverages()

                If IsOnAppPage AndAlso Me.Quote IsNot Nothing AndAlso GoverningStateQuote() IsNot Nothing AndAlso Me.GoverningStateQuote.LossHistoryRecords IsNot Nothing AndAlso Me.GoverningStateQuote.LossHistoryRecords.Count > 0 Then
                    PopulateLossHistory()
                End If

                'Me.ctlQuoteSummaryActions.Populate()
                'clear this label if all populated (before it was keeping old state) CAH 07/28/2017
                'Me.lblMsg.Text = ""
            End If
        Catch ex As Exception
            'HandleError("Populate", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Me.UseRatedQuoteImage = True
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Uncomment this line in the printer-friendly quote summary
        Populate()
    End Sub

    Public Overrides Function Save() As Boolean
        Return True
    End Function

    Private Sub WriteCell(sb As StringBuilder, cellText As String, Optional colSpan As Integer = 1)
        If colSpan > 1 Then
            sb.AppendLine("<td colspan='" & colSpan.ToString & "'>")
        Else
            sb.AppendLine("<td>")
        End If
        sb.AppendLine(cellText)
        sb.AppendLine("</td>")
    End Sub

    Private Sub WriteCell(sb As StringBuilder, cellText As String, styleText As String)
        sb.AppendLine("<td style=""" + styleText + """>")
        sb.AppendLine(cellText)
        sb.AppendLine("</td>")
    End Sub

    Private Sub WriteCell(sb As StringBuilder, cellText As String, styleText As String, cssclass As String, Optional ColSpan As Integer = -1)
        If ColSpan > 0 Then
            sb.AppendLine("<td colspan=" & ColSpan.ToString & " style=""" + styleText + """ class=""" + cssclass + """>")
        Else
            sb.AppendLine("<td style=""" + styleText + """ class=""" + cssclass + """>")
        End If
        sb.AppendLine(cellText)
        sb.AppendLine("</td>")
    End Sub


    Private Sub PopulatePolicyCoverages()
        If Quote IsNot Nothing Then
            If Me.SubQuotes IsNot Nothing Then
                Me.wcp_TotalEstimatedPlanPremium.InnerText = Me.SubQuotes.SumPropertyValues(Function() Quote.TotalEstimatedPlanPremium)
                Me.wcp_EmployersLiabilityQuotedPremium.InnerText = Me.SubQuotes.SumPropertyValues(Function() Quote.EmployersLiabilityQuotedPremium)
                Me.wcp_ExpModQuotedPremium.InnerText = Me.SubQuotes.SumPropertyValues(Function() Quote.ExpModQuotedPremium)
                Me.wcp_ScheduleModQuotedPremium.InnerText = Me.SubQuotes.SumPropertyValues(Function() Quote.ScheduleModQuotedPremium)
                Me.wcp_PremDiscountQuotedPremium.InnerText = Me.SubQuotes.SumPropertyValues(Function() Quote.PremDiscountQuotedPremium)
                Me.wcp_Dec_ExpenseConstantPremium.InnerText = Me.SubQuotes.SumPropertyValues(Function() Quote.Dec_ExpenseConstantPremium)
                Me.wcp_TerrorismQuotedPremium.InnerText = Me.SubQuotes.SumPropertyValues(Function() Quote.TerrorismQuotedPremium)
                Me.wcp_MinimumPremiumAdjustment.InnerText = Me.SubQuotes.SumPropertyValues(Function() Quote.MinimumPremiumAdjustment)
                Me.wcp_TotalQuotedPremium.InnerText = Me.SubQuotes.SumPropertyValues(Function() Quote.TotalQuotedPremium)
                Me.wcp_SecondInjuryFundQuotedPremium.InnerText = Me.SubQuotes.SumPropertyValues(Function() Quote.SecondInjuryFundQuotedPremium)

                ' Add Kentucky Special Fund Adjustment & Kentucky Surcharge  MGB 7/16/19
                wcp_KentuckySpecialFundAssessment_Row.Visible = False
                wcp_KentuckySurcharge_Row.Visible = False
                Dim tmp As String = Me.SubQuotes.SumPropertyValues(Function() Quote.KentuckySpecialFundAssessmentQuotedPremium)
                If tmp <> "" AndAlso IsNumeric(tmp) AndAlso CDec(tmp) > 0 Then
                    wcp_KentuckySpecialFundAssessment_Row.Visible = True
                    Me.lblWCPKYSpecialFundAssessmentPremium.Text = tmp
                End If
                tmp = Me.SubQuotes.SumPropertyValues(Function() Quote.WCP_KY_PremSurcharge)
                If tmp <> "" AndAlso IsNumeric(tmp) AndAlso CDec(tmp) > 0 Then
                    wcp_KentuckySurcharge_Row.Visible = True
                    Me.lblWCPKentuckySurchargePremium.Text = tmp
                End If

                Me.wcp_Dec_WC_TotalPremiumDue.InnerText = Me.SubQuotes.SumPropertyValues(Function() Quote.Dec_WC_TotalPremiumDue)
            End If
        End If

        Exit Sub
    End Sub

    Private Sub PopulateNamedIndividuals()
        If Quote IsNot Nothing Then
            If Me.SubQuotes IsNot Nothing Then
                If Me.SubQuotes.HasAnyTruePropertyValues(Function() Quote.HasInclusionOfSoleProprietorsPartnersOfficersAndOthers) Then
                    Me.Inclusion.Visible = True
                Else
                    Me.Inclusion.Visible = False
                End If

                If Me.SubQuotes.HasAnyTruePropertyValues(Function() Quote.HasWaiverOfSubrogation) Then
                    Me.Waiver13.Visible = True
                    Me.numWaivers.InnerText = Me.SubQuotes.SumPropertyValues(Function() Quote.WaiverOfSubrogationNumberOfWaivers)
                    Me.AmtWaivers.InnerText = If(Me.SubQuotes.HasAnyPropertyValuesMatchingString(Function() Quote.BlanketWaiverOfSubrogation, "4"), "0", Me.SubQuotes.SumPropertyValues(Function() Quote.WaiverOfSubrogationPremium))
                Else
                    Me.Waiver13.Visible = False
                    Me.numWaivers.InnerText = "0"
                    Me.AmtWaivers.InnerText = "0"
                End If

                If Me.SubQuotes.HasAnyPropertyValuesMatchingString(Function() Quote.BlanketWaiverOfSubrogation, "4") Then 'Waiver - Written Contract'
                    Me.WaiverContract.Visible = True
                    Me.WiaverContractPremium.InnerText = Me.SubQuotes.SumPropertyValues(Function() Quote.WCP_WaiverPremium)
                Else
                    Me.WaiverContract.Visible = False
                End If

                If Me.SubQuotes.HasAnyTruePropertyValues(Function() Quote.HasExclusionOfAmishWorkers) Then
                    Me.ExclusionAmish.Visible = True
                Else
                    Me.ExclusionAmish.Visible = False
                End If

                If Me.SubQuotes.HasAnyTruePropertyValues(Function() Quote.HasExclusionOfSoleProprietorsPartnersOfficersAndOthers) Then
                    Me.ExclusionOfficer.Visible = True
                Else
                    Me.ExclusionOfficer.Visible = False
                End If

                If IsMultistateCapableEffectiveDate(Quote.EffectiveDate) AndAlso SubQuotesContainsState("IL") Then
                    Dim ILQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState("IL")
                    If ILQuote IsNot Nothing Then
                        trExclusionOfExecutiveOfficerEtc.Visible = ILQuote.HasExclusionOfSoleProprietorsPartnersOfficersAndOthers_IL
                    End If
                End If

                ' Rejection of Coverage Endorsement - Added for KY MGB 4-30-19
                If CDate(Quote.EffectiveDate).Date >= CDate(IFM.VR.Common.Helpers.MultiState.General.KentuckyWCPEffectiveDate).Date AndAlso SubQuotesContainsState("KY") Then
                    Dim KYQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState("KY")
                    If KYQuote IsNot Nothing Then
                        trRejectionOfCoverageEndorsement.Visible = KYQuote.HasKentuckyRejectionOfCoverageEndorsement
                    End If
                End If

            End If
        End If

        Exit Sub
    End Sub

    Private Sub PopulateLocationInformation()
        Dim html As New StringBuilder()
        Dim locNdx As Integer = -1
        Dim bldNdx As Integer = -1

        Try
            If Quote IsNot Nothing Then
                If Quote.Locations IsNot Nothing Then
                    ' LOCATIONS
                    ' Section Header
                    html.AppendLine("<span Class='qs_section_headers'>LOCATION INFORMATION</span>")
                    html.AppendLine("<div class=""qs_Sub_Sections"">")
                    html.AppendLine("<table class=""qa_table_shades"">")
                    html.AppendLine("<tr class=""qs_section_grid_headers ui-widget-header"">")

                    ' Header Row
                    WriteCell(html, "Address")
                    WriteCell(html, "# of Employees", "", "qs_rightJustify qs_padRight")
                    html.AppendLine("</tr>")

                    ' Data Rows

                    ' LOCATIONS - New for multistate 11/7/18 MGB
                    ' Need to show all locations on the quote side now
                    For Each location In Quote.Locations
                        html.AppendLine("<tr class=""qs_basic_info_labels_cell"">")
                        ' Always show the address
                        WriteCell(html, location.Address.DisplayAddress)
                        ' Only show the number of employeees on the first location on each state
                        If WCP_LocationIsFirstOnStateQuote(location) Then
                            If location.Classifications.HasItemAtIndex(0) Then
                                WriteCell(html, location.Classifications(0).NumberOfEmployees, "", "qs_rightJustify qs_padRight")
                            Else
                                WriteCell(html, "")
                            End If
                        Else
                            WriteCell(html, "")
                        End If
                        html.AppendLine("</tr>")
                    Next

                    ' Close table
                    html.AppendLine("</table>")

                    ' Close div
                    html.AppendLine("</div>")

                    ' Write page
                    Me.tblLocationInfo.Text = html.ToString()
                End If
            End If

            Exit Sub
        Catch ex As Exception
            'HandleError("PopulateLocationInformation", ex)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' For WCP Multistate
    ''' 
    ''' Determines if the passed location is the first on it's state quote
    ''' returns true if so, false if not
    ''' </summary>
    ''' <param name="myLoc"></param>
    ''' <returns></returns>
    Private Function WCP_LocationIsFirstOnStateQuote(ByRef myLoc As QuickQuote.CommonObjects.QuickQuoteLocation) As Boolean
        If IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then
            ' Multistate
            Dim sq As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(myLoc.Address.QuickQuoteState)
            If sq IsNot Nothing Then
                If sq.Locations(0).Equals(myLoc) Then Return True
            End If
            Return False
        Else
            ' Not Multistate
            If Quote.Locations(0).Equals(myLoc) Then Return True Else Return False
        End If
    End Function

    Private Function GetClassifications() As List(Of Classification_struct)
        Dim classifications As New List(Of Classification_struct)

        If IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then
            ' MULTISTATE
            For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
                If sq.Locations IsNot Nothing AndAlso sq.Locations.HasItemAtIndex(0) AndAlso sq.Locations(0).Classifications IsNot Nothing Then
                    For Each cls As QuickQuote.CommonObjects.QuickQuoteClassification In sq.Locations(0).Classifications
                        If cls.ClassCode IsNot Nothing AndAlso cls.ClassCode.Trim <> "" Then
                            Dim newclass As New Classification_struct
                            newclass.qqClassification = cls
                            newclass.qqState = sq.Locations(0).Address.QuickQuoteState
                            classifications.Add(newclass)
                        End If
                    Next
                End If
            Next
            Return classifications
        Else
            ' SINGLE STATE
            If Quote IsNot Nothing AndAlso Quote.Locations IsNot Nothing AndAlso Quote.Locations.HasItemAtIndex(0) AndAlso Quote.Locations(0).Classifications IsNot Nothing Then
                For Each cls As QuickQuote.CommonObjects.QuickQuoteClassification In Quote.Locations(0).Classifications
                    If cls.ClassCode IsNot Nothing AndAlso cls.ClassCode.Trim <> "" Then
                        Dim newclass As New Classification_struct
                        newclass.qqClassification = cls
                        newclass.qqState = Quote.Locations(0).Address.QuickQuoteState
                        classifications.Add(newclass)
                    End If
                Next
                Return classifications
            End If
        End If
        Return Nothing  ' If we got here something went wrong
    End Function

    Private Sub PopulateClassCodes()
        Dim html As New StringBuilder()
        Dim locNdx As Integer = -1
        Dim bldNdx As Integer = -1
        Dim classifications As New List(Of Classification_struct)
        Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass

        Try
            If IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then
                ' MULTISTATE CODE (NEW)
                classifications = GetClassifications()
                If classifications IsNot Nothing Then
                    ' Section Header
                    html.AppendLine("<span Class='qs_section_headers'>CLASS CODES</span>")
                    html.AppendLine("<div class=""qs_Sub_Sections"">")
                    html.AppendLine("<table class=""qa_table_shades"">")
                    html.AppendLine("<tr class='qs_section_grid_headers ui-widget-header'>")

                    ' Header Row
                    WriteCell(html, "Class Code", "width:15%;")
                    WriteCell(html, "State", "width:10%;")
                    WriteCell(html, "Description", "width:35%;")
                    WriteCell(html, "Payroll", "width:20%;", "qs_rightJustify")
                    WriteCell(html, "Premium", "width:20%;", "qs_rightJustify")
                    html.AppendLine("</tr>")

                    For Each cls As Classification_struct In classifications
                        html.AppendLine("<tr class=""qs_basic_info_labels_cell"">")
                        WriteCell(html, cls.qqClassification.ClassCode)
                        Dim st As String = QuickQuote.CommonMethods.QuickQuoteHelperClass.StateAbbreviationForQuickQuoteState(cls.qqState)
                        WriteCell(html, st)
                        WriteCell(html, cls.qqClassification.Description)
                        WriteCell(html, CDec(cls.qqClassification.Payroll).ToString("C0"), "", "qs_rightJustify")
                        WriteCell(html, cls.qqClassification.QuotedPremium, "", "qs_rightJustify")
                        html.AppendLine("</tr>")
                    Next

                    ' Close table
                    html.AppendLine("</table>")

                    ' Close div
                    html.AppendLine("</div>")

                    ' Write page
                    Me.tblClassCodes.Text = html.ToString()
                End If
            Else
                ' NON-MULTISTATE CODE - USES OLD LOGIC
                If Quote IsNot Nothing Then
                    If Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count >= 1 Then
                        If Quote.Locations(0).Classifications IsNot Nothing AndAlso Quote.Locations(0).Classifications.Count > 0 Then
                            ' LOCATIONS
                            ' Section Header
                            html.AppendLine("<span Class='qs_section_headers'>CLASS CODES</span>")
                            html.AppendLine("<div class=""qs_Sub_Sections"">")
                            html.AppendLine("<table class=""qa_table_shades"">")
                            html.AppendLine("<tr class=""qs_section_grid_headers ui-widget-header qs_grid_4_columns"">")

                            ' Header Row
                            WriteCell(html, "Class Code")
                            WriteCell(html, "Description")
                            WriteCell(html, "Payroll", "", "qs_rightJustify")
                            WriteCell(html, "Premium", "", "qs_rightJustify")
                            html.AppendLine("</tr>")

                            ' Data Rows
                            For Each classification As QuickQuote.CommonObjects.QuickQuoteClassification In Quote.Locations(0).Classifications
                                html.AppendLine("<tr class=""qs_basic_info_labels_cell qs_grid_4_columns"">")
                                WriteCell(html, classification.ClassCode)
                                WriteCell(html, classification.Description)
                                WriteCell(html, CDec(classification.Payroll).ToString("C0"), "", "qs_rightJustify")
                                WriteCell(html, classification.QuotedPremium, "", "qs_rightJustify")
                                html.AppendLine("</tr>")
                            Next

                            ' Close table
                            html.AppendLine("</table>")

                            ' Close div
                            html.AppendLine("</div>")

                            ' Write page
                            Me.tblClassCodes.Text = html.ToString()
                        End If
                    End If
                End If
            End If

            Exit Sub
        Catch ex As Exception
            'HandleError("PopulateClassCodes", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub PopulateLossHistory()
        Dim html As New StringBuilder()

        ' We don't need to check quote or any of the objects because we checked all of that on the call to this function.  MGB 8/29/2018

        ' Start Loss History Section
        html.AppendLine("<div Class='qs_Main_Sections'>")
        html.AppendLine("<div class=""qs_Sub_Sections"">")
        html.AppendLine("<table class=""qa_table_shades"">")

        ' 1st orange column header
        html.AppendLine("<tr class=""qs_section_grid_headers ui-widget-header  qs_grid_4_columns"">")
        WriteCell(html, indent & "Loss History (" + GoverningStateQuote.LossHistoryRecords.Count.ToString + ")", 3)
        WriteCell(html, "", "")
        html.AppendLine("</tr>")

        ' 2nd orange column header
        html.AppendLine("<tr class=""qs_section_grid_headers ui-widget-header"">")
        WriteCell(html, "Type of Loss", "width:40%")
        WriteCell(html, "Loss Date", "width:24%")
        WriteCell(html, "Loss Amount", "width:23%")
        WriteCell(html, "", "width:13%")
        html.AppendLine("</tr>")

        ' DATA ROWS
        PopulateLossHistoryDetail(html, GoverningStateQuote.LossHistoryRecords)

        ' Extra line & Close table
        html.AppendLine("<tr><td colspan=3>&nbsp;</td></tr>")
        html.AppendLine("</table>") ' Building coverages table
        html.AppendLine("</div>")
        html.AppendLine("</div>")

        ' Write page
        Me.tblLossHistory.Text = html.ToString()

        ' Spacer line
        'html.AppendLine("<tr><td colspan=4>&nbsp;</td></tr>")

        Exit Sub
    End Sub

    Private Sub PopulateLossHistoryDetail(ByRef html As StringBuilder, ByVal Hist As List(Of QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord))

        'Public Function GetStaticDataTextForValue(options As List(Of QuickQuoteStaticDataOption), value As String, ByRef foundValue As Boolean) As String
        For Each loss As QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord In Hist
            Dim LossType = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLossHistoryRecord, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.TypeOfLossId, loss.TypeOfLossId)

            html.AppendLine("<tr class="""">")
            WriteCell(html, LossType)
            WriteCell(html, loss.LossDate)
            'WriteCell(html, FormatCurrency(loss.Amount), 2)
            WriteCell(html, FormatCurrency(If(loss IsNot Nothing AndAlso String.IsNullOrWhiteSpace(loss.Amount) = False, loss.Amount, 0)), 2)
            html.AppendLine("</tr>")
        Next


    End Sub

    'Private Sub lnkPrint_Click(sender As Object, e As EventArgs) Handles lnkPrint.Click
    '    Try
    '        Fire_BroadcastWorkflowChangeRequestEvent(Common.Workflow.Workflow.WorkflowSection.drivers, "")
    '    Catch ex As Exception

    '    End Try
    'End Sub

    Private Function SumMoneyStrings(items As List(Of String)) As String
        Dim sum = 0
        For Each number As String In items
            Dim result = 0
            If Decimal.TryParse(number, NumberStyles.Currency, CultureInfo.CurrentCulture, result) Then
                sum += result
            End If
        Next
        Return sum.ToString("c")
    End Function


    Protected Overrides Sub Render(writer As HtmlTextWriter)
        Dim filename As String
        Dim sb As New StringBuilder()
        Using sw As New StringWriter(sb)
            Using htw As New HtmlTextWriter(sw)
                MyBase.Render(htw)
                writer.Write(sb.ToString())

                Dim htmlBytes As Byte() = Encoding.UTF8.GetBytes(sw.ToString)
                If htmlBytes IsNot Nothing Then
                    filename = String.Format("SUMMARY{0}.pdf", Me.Quote.QuoteNumber)
                    Dim filePath As String = Server.MapPath(Request.ApplicationPath) & "\Reports\" & filename & ".htm"
                    Dim fs As New FileStream(filePath, FileMode.Create)
                    fs.Write(htmlBytes, 0, htmlBytes.Length)
                    fs.Close()

                    If File.Exists(filePath) = True Then 'enclosed block in IF statement to make sure file exists
                        Dim status As String = ""
                        Dim pdfPath As String = Server.MapPath(Request.ApplicationPath) & "\Reports\" & filename & ".pdf"

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
                                        Response.AddHeader("content-disposition", "attachment; filename=" + String.Format("SUMMARY{0}.pdf", Me.Quote.PolicyNumber)) 'updated 6/19/2019 to use PolicyNumber instead of QuoteNumber
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
                QQHelper.SendEmail("ProposalPdfConverter@indianafarmers.com", "tbirkey@indianafarmers.com", "Error Converting Summary to PDF", eMsg)
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