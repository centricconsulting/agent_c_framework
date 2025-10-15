Imports IFM.PrimativeExtensions
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.IO
Imports System.Globalization
Imports IFM.VR.Common.Helpers.MultiState.General
Imports IFM.VR.Common.Helpers.MultiState.Extensions

Public Class ctl_WCP_QuoteSummary
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


    Private Sub HandleError(ByVal RoutineName As String, ByRef ex As Exception)
        If AppSettings("TestOrProd") IsNot Nothing AndAlso AppSettings("TestOrProd").ToUpper() = "TEST" Then
            lblMsg.Text = RoutineName + ": " & ex.Message
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion("divWCPQuoteSummary", HiddenField1, "0")
        Me.VRScript.StopEventPropagation(Me.lnkPrint.ClientID, False)
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
                If Me.SubQuotesHasAnyState(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana) Then
                    Me.trInSecondInjury.Visible = True
                    Me.lblIndSecondInjurySurcharge.Text = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.SecondInjuryFundQuotedPremium)
                Else
                    Me.trInSecondInjury.Visible = False
                End If

                Me.lblEmployersLiability.Text = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.EmployersLiability)

                If IsOnAppPage Then
                    lblPrintReminder.Visible = True
                End If

                PopulateLocationInformation()

                    PopulateClassCodes()

                    PopulateNamedIndividuals()

                    PopulatePolicyCoverages()

                    If IsOnAppPage AndAlso Me.Quote IsNot Nothing AndAlso GoverningStateQuote() IsNot Nothing AndAlso Me.GoverningStateQuote.LossHistoryRecords IsNot Nothing AndAlso Me.GoverningStateQuote.LossHistoryRecords.Count > 0 Then
                        PopulateLossHistory()
                    End If

                    Me.ctlQuoteSummaryActions.Populate()
                    'clear this label if all populated (before it was keeping old state) CAH 07/28/2017
                    Me.lblMsg.Text = ""
                End If
        Catch ex As Exception
            HandleError("Populate", ex)
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
                If Me.SubQuotesHasAnyState(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana) Then
                    Me.wcp_SecondInjuryFundQuotedPremium_Row.Visible = True
                    Me.wcp_SecondInjuryFundQuotedPremium.InnerText = Me.SubQuotes.SumPropertyValues(Function() Quote.SecondInjuryFundQuotedPremium)
                Else
                    Me.wcp_SecondInjuryFundQuotedPremium_Row.Visible = False
                End If

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

                Dim _CatastropheOtherThanCertifiedActsOfTerrorismQuotedPremium = Me.SubQuotes.SumPropertyValues(Function() Quote.CatastropheOtherThanCertifiedActsOfTerrorismQuotedPremium)
                If _CatastropheOtherThanCertifiedActsOfTerrorismQuotedPremium.TryToGetInt32() > 0 Then
                    wcp_Catastrophe_Terror_Row.Visible = True
                    wcp_Catastrophe_Terror.InnerText = _CatastropheOtherThanCertifiedActsOfTerrorismQuotedPremium
                Else
                    wcp_Catastrophe_Terror_Row.Visible = False
                End If

                ' IL Commission Fund Surcharge
                Dim _IL_WCP_CommissionOperationsFundSurcharge = Me.SubQuotes.SumPropertyValues(Function() Quote.IL_WCP_CommissionOperationsFundSurcharge)
                If _IL_WCP_CommissionOperationsFundSurcharge.TryToGetInt32() > 0 Then
                    IL_WCP_CommissionOperationsFundSurcharge_Row.Visible = True
                    IL_WCP_CommissionOperationsFundSurcharge_Premium.Text = _IL_WCP_CommissionOperationsFundSurcharge

                    trIL_WCP_CommissionOperationsFundSurcharge_Top.Visible = True
                    lblIL_WCP_CommissionOperationsFundSurcharge_Top.Text = _IL_WCP_CommissionOperationsFundSurcharge
                    colWidthSetter.RemoveCssClass("qs_resetWidth")
                    colWidthSetter.AddCssClass("qs_resetWidth-wider")
                Else
                    IL_WCP_CommissionOperationsFundSurcharge_Row.Visible = False
                    trIL_WCP_CommissionOperationsFundSurcharge_Top.Visible = False
                    colWidthSetter.RemoveCssClass("qs_resetWidth-wider")
                    colWidthSetter.AddCssClass("qs_resetWidth")
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
            HandleError("PopulateLocationInformation", ex)
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
                        Dim stateabbrev As String = QuickQuote.CommonMethods.QuickQuoteHelperClass.StateAbbreviationForQuickQuoteState(cls.qqState)

                        html.AppendLine("<tr class=""qs_basic_info_labels_cell"">")
                        WriteCell(html, cls.qqClassification.ClassCode)
                        WriteCell(html, stateabbrev)
                        'Select Case cls.qqState
                        '    Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
                        '        WriteCell(html, "IL")
                        '        Exit Select
                        '    Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
                        '        WriteCell(html, "IN")
                        '        Exit Select
                        '    Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Kentucky
                        '        WriteCell(html, "KY")
                        '        Exit Select
                        'End Select
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
            HandleError("PopulateClassCodes", ex)
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

    Private Sub lnkPrint_Click(sender As Object, e As EventArgs) Handles lnkPrint.Click
        Try
            Fire_BroadcastWorkflowChangeRequestEvent(Common.Workflow.Workflow.WorkflowSection.drivers, "")
        Catch ex As Exception

        End Try
    End Sub

    'added 2/20/2020
    Public Sub CheckForReRateAfterEffDateChange(Optional ByVal qqTranType As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.None, Optional ByVal newEffectiveDate As String = "", Optional ByVal oldEffectiveDate As String = "")
        Me.ctlQuoteSummaryActions.CheckForReRateAfterEffDateChange(qqTranType:=qqTranType, newEffectiveDate:=newEffectiveDate, oldEffectiveDate:=oldEffectiveDate)
    End Sub

End Class