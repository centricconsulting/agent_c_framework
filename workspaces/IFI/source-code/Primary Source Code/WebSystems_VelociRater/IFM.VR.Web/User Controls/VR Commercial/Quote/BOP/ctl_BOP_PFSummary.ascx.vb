Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.IO
Public Class ctl_BOP_PFSummary
    Inherits VRControlBase
    Const COVERAGE_CODE_ID_LARGE_PREMIUM_DISCOUNT = "80558"

    Dim indent As String = "&nbsp;&nbsp;"
    Dim dblindent As String = "&nbsp;&nbsp;&nbsp;&nbsp;"

    Private Enum PLCoverages_Enum
        Apartments
        Barbers
        Beauticans
        FineArts
        Funeral
        Liquor
        Motel
        Optical
        Pharmacist
        Photography
        MakeupAndHair
        Printers
        ResidentialCleaning
        Restaurants
        SelfStorage
        Veterinarians
    End Enum

    Private Sub HandleError(ByVal RoutineName As String, ByRef ex As Exception)
        If AppSettings("TestOrProd") IsNot Nothing AndAlso AppSettings("TestOrProd").ToUpper() = "TEST" Then
            'lblMsg.Text = RoutineName + ": " & ex.Message
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion("divBOPQuoteSummary", HiddenField1, "0")
        'Me.VRScript.StopEventPropagation(Me.lnkPrint.ClientID, False)
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub



    Public Overrides Sub Populate()
        Try
            If Me.Quote IsNot Nothing Then
                Quote.CopyProfessionalLiabilityCoveragesFromPolicyToBuildings_UseBuildingClassificationList()
                'Updated 09/21/2021 for BOP Endorsements Task 61506 MLW
                Select Case Me.Quote.QuoteTransactionType
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote
                        Me.ctlEndorsementOrChangeHeader.Visible = True
                        Me.ctlEndorsementOrChangeHeader.Quote = Quote
                        Me.EndorsementPrintSection.Visible = True
                        Me.quoteSummaryHeader.Visible = False
                        Me.quoteSummarySection.Visible = False
                    Case Else
                        Me.ctlEndorsementOrChangeHeader.Visible = False
                        Me.EndorsementPrintSection.Visible = False
                        Me.quoteSummaryHeader.Visible = True
                        Me.quoteSummarySection.Visible = True

                        Me.lblMainAccord.Text = String.Format("{2} - Effective Date: {0} - {1}", Me.Quote.EffectiveDate, QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.PolicyTermId, Me.Quote.PolicyTermId), If(IsOnAppPage, "App", "Quote") & " Summary")
                        Me.ImageDateAndPremChangeLine.Visible = False

                        Me.lblPremiumMainAccord.Text = String.Format("{0:C2}", Me.Quote.TotalQuotedPremium)

                        Dim nm As String = Me.Quote.Policyholder.Name.DisplayName
                        If Quote.Policyholder.Name.DoingBusinessAsName IsNot Nothing AndAlso Quote.Policyholder.Name.DoingBusinessAsName.Trim <> "" Then nm += " DBA " & Quote.Policyholder.Name.DoingBusinessAsName
                        Me.lblPhName.Text = nm

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
                End Select

                ''If IsOnAppPage Then
                ''    Me.lblMainAccord.Text = String.Format("App Summary - Effective Date: {0} - 12 month", Me.Quote.EffectiveDate)
                ''Else
                ''    Me.lblMainAccord.Text = String.Format("Quote Summary - Effective Date: {0} - 12 month", Me.Quote.EffectiveDate)
                ''End If
                ''updated 5/10/2019; logic taken from updates for PPA
                'Select Case Me.Quote.QuoteTransactionType
                '    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage
                '        Me.lblMainAccord.Text = If(Me.Quote.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote, "Change", "Image") & " Summary - Updated " & QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.PolicyTermId, Me.Quote.PolicyTermId) & " Premium"
                '        Me.ImageDateAndPremChangeLine.Visible = True
                '        If QQHelper.IsDateString(Me.Quote.TransactionEffectiveDate) = True Then
                '            Me.lblTranEffDate.Text = "Effective Date: " & Me.Quote.TransactionEffectiveDate 'note: should already be in shortdate format
                '        Else
                '            Me.lblTranEffDate.Text = ""
                '        End If
                '        If QQHelper.IsNumericString(Me.Quote.ChangeInFullTermPremium) = True Then 'note: was originally looking for positive decimal, but the change in prem could be zero or negative
                '            Me.lblAnnualPremChg.Text = "Annual Premium Change: " & Me.Quote.ChangeInFullTermPremium 'note: should already be in money format
                '        Else
                '            Me.lblAnnualPremChg.Text = ""
                '        End If
                '    Case Else
                '        Me.lblMainAccord.Text = String.Format("{2} - Effective Date: {0} - {1}", Me.Quote.EffectiveDate, QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.PolicyTermId, Me.Quote.PolicyTermId), If(IsOnAppPage, "App", "Quote") & " Summary")
                '        Me.ImageDateAndPremChangeLine.Visible = False
                'End Select

                'Me.lblPremiumMainAccord.Text = String.Format("{0:C2}", Me.Quote.TotalQuotedPremium)

                'Dim nm As String = Me.Quote.Policyholder.Name.DisplayName
                'If Quote.Policyholder.Name.DoingBusinessAsName IsNot Nothing AndAlso Quote.Policyholder.Name.DoingBusinessAsName.Trim <> "" Then nm += " DBA " & Quote.Policyholder.Name.DoingBusinessAsName
                'Me.lblPhName.Text = nm

                'Me.lblQuoteNum.Text = Me.Quote.QuoteNumber

                'Dim zip As String = Me.Quote.Policyholder.Address.Zip
                'If zip.Length > 5 Then
                '    zip = zip.Substring(0, 5)
                'End If

                ''house num, street, apt, pobox, city, state, zip
                'Me.lblPhAddress.Text = String.Format("{0} {1} {2} {3} {4} {5} {6}", Me.Quote.Policyholder.Address.HouseNum, Me.Quote.Policyholder.Address.StreetName, If(String.IsNullOrWhiteSpace(Me.Quote.Policyholder.Address.ApartmentNumber) = False, "Apt# " + Me.Quote.Policyholder.Address.ApartmentNumber, ""), Me.Quote.Policyholder.Address.POBox, Me.Quote.Policyholder.Address.City, Me.Quote.Policyholder.Address.State, zip).Replace("  ", " ").Trim()

                'Me.lblEffectiveDate.Text = Me.Quote.EffectiveDate
                'Me.lblExpirationDate.Text = Me.Quote.ExpirationDate

                'Me.lblFullPremium.Text = String.Format("{0:C2}", Me.Quote.TotalQuotedPremium)

                PopulateLiabilityInformation()

                PopulatePolicyCoverageInformation()

                PopulatePolicyDiscounts()

                PopulateLocationInformation()

                If IsOnAppPage AndAlso Me.Quote IsNot Nothing AndAlso Me.GoverningStateQuote IsNot Nothing AndAlso Me.GoverningStateQuote.LossHistoryRecords IsNot Nothing AndAlso Me.GoverningStateQuote.LossHistoryRecords.Count > 0 Then
                    PopulateLossHistory()
                End If

                'Me.ctlQuoteSummaryActions.Populate()
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

    Private Sub PopulateLiabilityInformation()
        Dim html As New StringBuilder()

        Try
            If Quote IsNot Nothing Then
                If SubQuoteFirst IsNot Nothing Then
                    html.AppendLine("<div class=""bop_qs_Sub_Sections"">")
                    html.AppendLine("<table class=""bop_qa_table_shades"">")
                    html.AppendLine("<tr class=""qs_bop_section_grid_headers ui-widget-header"">")

                    ' Header Row
                    WriteCell(html, "Coverage")
                    WriteCell(html, "Limit", "text-align:right", "qs_bop_Grid_cell_premium")
                    WriteCell(html, "Premium", "text-align:right;", "qs_bop_Grid_cell_premium")
                    html.AppendLine("</tr>")

                    ' Data Rows
                    ' Occurrence Liability Limit
                    html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                    WriteCell(html, "Occurrence Liability Limit")
                    WriteCell(html, SubQuoteFirst.OccurrenceLiabilityLimit, "text-align:right;")
                    WriteCell(html, QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.OccurrencyLiabilityQuotedPremium), "text-align:right;")
                    html.AppendLine("</tr>")

                    ' Tenants Fire Liability
                    html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                    WriteCell(html, "Tenants Fire Liability")
                    WriteCell(html, SubQuoteFirst.TenantsFireLiability, "text-align:right;")
                    WriteCell(html, QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.TenantsFireLiabilityQuotedPremium), "text-align:right;")
                    html.AppendLine("</tr>")
                    ' Property Damage Liability Deductible
                    html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                    WriteCell(html, "Property Damage Liability Deductible")
                    WriteCell(html, Quote.PropertyDamageLiabilityDeductible, "text-align:right;")
                    WriteCell(html, "")
                    html.AppendLine("</tr>")
                    ' Business Master Enhancement
                    If SubQuoteFirst.HasBusinessMasterEnhancement Then
                        html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                        WriteCell(html, "BusinessMaster Enhancement")
                        WriteCell(html, "")
                        WriteCell(html, QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.BusinessMasterEnhancementQuotedPremium), "text-align:right;")
                        html.AppendLine("</tr>")
                    End If

                    ' IL Contractors Home Repair & Remodeling MGB 10/31/2018
                    If IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(Quote.EffectiveDate) AndAlso SubQuotesContainsState("IL") Then
                        Dim ILQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois)
                        If ILQuote IsNot Nothing AndAlso ILQuote.HasIllinoisContractorsHomeRepairAndRemodeling Then
                            html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                            WriteCell(html, "IL Contractors - Home Repair & Remodeling")
                            WriteCell(html, "10,000", "text-align:right;")
                            Dim prem As String = ILQuote.IllinoisContractorsHomeRepairAndRemodelingQuotedPremium
                            WriteCell(html, prem, "text-align:right;")
                            html.AppendLine("</tr>")
                        End If
                    End If

                    ' OH Stop Gap
                    If IFM.VR.Common.Helpers.MultiState.General.IsOhioEffective(Quote) Then
                        Dim gsQuote As QuickQuoteObject = Me.GoverningStateQuote()
                        If Not String.IsNullOrWhiteSpace(gsQuote.StopGapLimitId) AndAlso gsQuote.StopGapLimitId.IsNumeric Then
                            html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                            WriteCell(html, "Stop Gap (OH)")
                            'Dim lim As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.StopGapLimitId, SubQuoteFirst.StopGapLimitId)
                            'WriteCell(html, lim)
                            WriteCell(html, "")
                            Dim Prem As String = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.StopGapQuotedPremium)
                            If Prem.IsNumeric Then Prem = Format(CDec(Prem), "c")
                            WriteCell(html, Prem, "text-align:right;")
                            html.AppendLine("</tr>")
                        End If
                    End If

                    ' Close table tag
                    html.AppendLine("</table>")

                    ' Write to page
                    Me.tblLiabilityInfo.Text = html.ToString()
                End If
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("PopulateLiabilityInformation", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub PopulatePolicyCoverageInformation()
        Dim html As New StringBuilder()
        Try
            If Quote IsNot Nothing Then
                If SubQuoteFirst IsNot Nothing Then
                    html.AppendLine("<div class=""bop_qs_Sub_Sections"">")
                    html.AppendLine("<table class=""bop_qa_table_shades"">")
                    html.AppendLine("<tr class=""qs_bop_section_grid_headers ui-widget-header"">")

                    ' Header Row
                    WriteCell(html, "Coverage")
                    WriteCell(html, "Premium", "text-align:right;", "qs_bop_Grid_cell_premium")
                    html.AppendLine("</tr>")

                    ' Data Rows
                    ' Additional Insureds
                    If (Quote.AdditionalInsureds IsNot Nothing AndAlso Quote.AdditionalInsureds.Count > 0) OrElse (SubQuoteFirst.AdditionalInsuredsCheckboxBOP IsNot Nothing AndAlso SubQuoteFirst.AdditionalInsuredsCheckboxBOP.Count > 0) Then
                        html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                        Dim cnt As Decimal = 0
                        If Quote.AdditionalInsureds IsNot Nothing AndAlso Quote.AdditionalInsureds.Count > 0 Then cnt += Quote.AdditionalInsureds.Count
                        If SubQuoteFirst.AdditionalInsuredsCheckboxBOP IsNot Nothing AndAlso SubQuoteFirst.AdditionalInsuredsCheckboxBOP.Count > 0 Then cnt += SubQuoteFirst.AdditionalInsuredsCheckboxBOP.Count
                        WriteCell(html, "Additonal Insureds: " & cnt.ToString)
                        Dim tot As Decimal = 0
                        If IsNumeric(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.AdditionalInsuredsQuotedPremium)) Then tot += CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.AdditionalInsuredsQuotedPremium))
                        If IsNumeric(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.AdditionalInsuredsCheckboxBOPPremium)) Then tot += CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.AdditionalInsuredsCheckboxBOPPremium))
                        WriteCell(html, Format(tot, "c"), "text-align:right;")
                        html.AppendLine("</tr>")
                    End If

                    'Owners Lessees Or Contractors Completed Operations Total Premium
                    If SubQuoteFirst.OwnersLesseesorContractorsCompletedOperationsTotalPremium IsNot Nothing AndAlso
                        Not QQHelper.IsZeroPremium(SubQuoteFirst.OwnersLesseesorContractorsCompletedOperationsTotalPremium) Then
                        html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                        WriteCell(html, "Owners, Lessees or Contractors - Completed Operations")
                        WriteCell(html, SubQuoteFirst.OwnersLesseesorContractorsCompletedOperationsTotalPremium, "text-align:right;")
                        html.AppendLine("</tr>")
                    End If

                    ' Electronic Data
                    If SubQuoteFirst.HasElectronicData Then
                        html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                        WriteCell(html, "Electronic Data: " & Quote.ElectronicDataLimit)
                        WriteCell(html, QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.ElectronicDataQuotedPremium), "text-align:right;")
                        html.AppendLine("</tr>")
                    End If

                    ' Contractors
                    If SubQuoteFirst.ContractorsEquipmentInstallationLimit.Trim <> "" OrElse SubQuoteFirst.ContractorsToolsEquipmentBlanket.Trim <> "" OrElse SubQuoteFirst.ContractorsToolsEquipmentScheduled.Trim <> "" OrElse SubQuoteFirst.ContractorsToolsEquipmentRented.Trim <> "" OrElse SubQuoteFirst.ContractorsEmployeeTools.Trim <> "" Then
                        ' Installation Limit
                        If SubQuoteFirst.ContractorsEquipmentInstallationLimit.Trim <> "" Then
                            html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                            WriteCell(html, "Contractors Equipment/Installation: " & Quote.ContractorsEquipmentInstallationLimit & " Contractors Installation Limit")
                            WriteCell(html, QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.ContractorsEquipmentInstallationLimitQuotedPremium), "text-align:right;")
                            html.AppendLine("</tr>")
                        End If
                        ' Tools & Equipment - Blanket
                        If SubQuoteFirst.ContractorsToolsEquipmentBlanket.Trim <> "" Then
                            html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                            WriteCell(html, indent & Quote.ContractorsToolsEquipmentBlanket & " Contractors Tools & Equipment - Blanket ")
                            WriteCell(html, QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.ContractorsToolsEquipmentBlanketQuotedPremium), "text-align:right;")
                            html.AppendLine("</tr>")
                        End If
                        ' Tools & Equipment - Scheduled
                        If SubQuoteFirst.ContractorsToolsEquipmentScheduled.Trim <> "" Then
                            html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                            WriteCell(html, indent & Quote.ContractorsToolsEquipmentScheduled & " Contractors Tools & Equipment - Scheduled")
                            WriteCell(html, QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.ContractorsToolsEquipmentScheduledQuotedPremium), "text-align:right;")
                            html.AppendLine("</tr>")
                        End If
                        ' Rented/Leased Tools & Equipment
                        If SubQuoteFirst.ContractorsToolsEquipmentRented.Trim <> "" Then
                            html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                            WriteCell(html, indent & Quote.ContractorsToolsEquipmentRented & " Rented/Leased Tools & Equipment Limit")
                            WriteCell(html, QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.ContractorsToolsEquipmentRentedQuotedPremium), "text-align:right;")
                            html.AppendLine("</tr>")
                        End If
                        ' Contractors Employees Tools Limit
                        If SubQuoteFirst.ContractorsEmployeeTools.Trim <> "" Then
                            html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                            WriteCell(html, indent & Quote.ContractorsEmployeeTools & " Contractors Employees Tools")
                            WriteCell(html, QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.ContractorsEmployeeToolsQuotedPremium), "text-align:right;")
                            html.AppendLine("</tr>")
                        End If
                    End If

                    ' Crime
                    If SubQuoteFirst.CrimeEmpDisLimit.Trim <> "" Then
                        Dim MsON As Decimal = 0
                        Dim EmpDH As Decimal = 0
                        Dim Forgery As Decimal = 0
                        Dim TotPrem As Decimal = 0

                        ' Get Employee Dishonesty and Forgery premiums from the quote 
                        If IsNumeric(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.CrimeEmpDisQuotedPremium)) Then EmpDH = CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.CrimeEmpDisQuotedPremium))
                        If IsNumeric(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.CrimeForgeryQuotedPremium)) Then Forgery = CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.CrimeForgeryQuotedPremium))

                        ' Get Money & Securities total premium from all locations on the quote
                        'commented out per task WS-621 and WS-1279                        'For Each LOC As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                        '    If IsNumeric(LOC.MoneySecuritiesQuotedPremium) Then MsON += CDec(LOC.MoneySecuritiesQuotedPremium)
                        'Next

                        ' Sum the above premiums to get the total crime premium
                        TotPrem = EmpDH + Forgery + MsON

                        ' Add the line to the quote summary
                        html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                        WriteCell(html, "Crime")
                        WriteCell(html, Format(TotPrem, "c"), "text-align:right;")
                        html.AppendLine("</tr>")
                    End If

                    ' EPLI
                    ' MGB 11/12/2020 Bug 43384 correction
                    If SubQuoteFirst.HasEPLI AndAlso SubQuoteFirst.EPLICoverageTypeID = "22" Then
                        html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                        WriteCell(html, "EPLI: (non-underwritten)")
                        WriteCell(html, QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.EPLIPremium), "text-align:right;")
                        html.AppendLine("</tr>")
                    End If

                    ' CLI
                    If SubQuoteFirst.CyberLiability AndAlso SubQuoteFirst.CyberLiabilityTypeId = "23" Then
                        html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                        If IFM.VR.Common.Helpers.BOP.CyberCoverageHelper.IsCyberCoverageAvailable(Quote) Then
                            WriteCell(html, "Cyber Coverage:")
                        Else
                            WriteCell(html, "Cyber Liability:")
                        End If
                        WriteCell(html, QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.CyberLiabilityPremium), "text-align:right;")
                        html.AppendLine("</tr>")
                    End If

                    ' Employee Benefits Liability
                    If SubQuoteFirst.EmployeeBenefitsLiabilityText.Trim <> "" Then
                        html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                        ' TODO: ASK DON ABOUT THIS
                        WriteCell(html, "Employee Benefits Liability: " & SubQuoteFirst.EmployeeBenefitsLiabilityText & " employees")
                        WriteCell(html, QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.EmployeeBenefitsLiabilityQuotedPremium), "text-align:right;")
                        html.AppendLine("</tr>")
                    End If

                    ' Earthquake
                    If SubQuoteFirst.HasEarthquake Then
                        html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                        WriteCell(html, "Earthquake")
                        WriteCell(html, QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.EarthquakeQuotedPremium), "text-align:right;")
                        html.AppendLine("</tr>")
                    End If

                    ' Hired Auto
                    If SubQuoteFirst.HasHiredAuto Then
                        html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                        WriteCell(html, "Hired Auto")
                        WriteCell(html, QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.HiredAutoQuotedPremium), "text-align:right;")
                        html.AppendLine("</tr>")
                    End If

                    ' Non-Owned
                    If SubQuoteFirst.HasNonOwnedAuto Then
                        html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                        WriteCell(html, "Non-owned")
                        WriteCell(html, QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.NonOwnedAutoQuotedPremium), "text-align:right;")
                        html.AppendLine("</tr>")
                    End If

                    ' Close Table
                    html.AppendLine("</table>")
                    Me.tblPolicyCoverages.Text = html.ToString()
                End If
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("PopulatePolicyCoverageInformation", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub PopulatePolicyDiscounts()
        Try
            If Me.GoverningStateQuote.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio Then

                Dim LPDP_Coverage = SummaryHelperClass.Find_First_PolicyLevelCoverage(SubQuotes, COVERAGE_CODE_ID_LARGE_PREMIUM_DISCOUNT)
                Dim lpdpElement As XElement = Nothing

                If LPDP_Coverage IsNot Nothing Then
                    Dim lpdpValue As Decimal = Decimal.Parse(LPDP_Coverage.FullTermPremium)

                    lpdpElement = <tr class="qs_cap_grid_3_columns">
                                      <td colspan="2">Total Ohio Premium Discount</td>
                                      <td class="qs_rightJustify"><%= $"{lpdpValue:$#.00}" %></td>
                                  </tr>
                End If


                Dim html =
                        <div class="bop_qs_Sub_Sections">
                            <table class="bop_qa_table_shades">
                                <tr class="qs_bop_section_grid_headers ui-widget-header  qs_bop_grid_3_columns">
                                    <td colspan="2">Coverage</td>
                                    <td class="qs_rightJustify">Premium</td>
                                </tr>
                                <%= lpdpElement %>
                            </table>
                        </div>
                Me.tblPolicyDiscounts.Text = html.ToString()
            End If
        Catch ex As Exception
            HandleError("PopulatePolicyDiscounts", ex)
        End Try
    End Sub
    Private Sub PopulateLocationInformation()
        Dim html As New StringBuilder()
        Dim locNdx As Integer = -1
        Dim bldNdx As Integer = -1

        Try
            For Each LOC As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                ' LOCATIONS
                ' Section Header
                html.AppendLine("<span Class='qs_bop_section_headers'>LOCATION INFORMATION</span>")
                html.AppendLine("<div Class='bop_qs_Sub_Sections'>")

                'html.AppendLine("<div class=""bop_qs_Sub_Sections"">")
                html.AppendLine("<table class=""bop_qa_table_shades"">")
                html.AppendLine("<tr class=""qs_bop_section_grid_headers ui-widget-header"">")

                If NewCoProtectionClassHelper.IsNewCoProtectionClassAvailable(Quote) Then
                    ' Header Row
                    WriteCell(html, "Address", "width:50%")
                    WriteCell(html, "PC", "width:10%")
                    WriteCell(html, "Description", "width:40%")
                    html.AppendLine("</tr>")

                    ' Data Rows
                    html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                    WriteCell(html, LOC.Address.DisplayAddress, "width:50%")
                    WriteCell(html, LOC.ProtectionClass, "width:10%")
                    WriteCell(html, LOC.Description, "width:40%")
                    html.AppendLine("</tr>")
                Else
                    ' Header Row
                    WriteCell(html, "Address")
                    WriteCell(html, "Description")
                    html.AppendLine("</tr>")

                    ' Data Rows
                    html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                    WriteCell(html, LOC.Address.DisplayAddress)
                    WriteCell(html, LOC.Description)
                    html.AppendLine("</tr>")

                End If
                '' Header Row
                'WriteCell(html, "Address")
                'WriteCell(html, "Description")
                'html.AppendLine("</tr>")

                '' Data Rows
                'html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                'WriteCell(html, LOC.Address.DisplayAddress)
                'WriteCell(html, LOC.Description)
                'html.AppendLine("</tr>")


                ' Close table
                html.AppendLine("</table>")

                PopulateOptionalLocationCoverages(html, LOC)
                PopulateBuildings(html, LOC)

                ' Close div
                html.AppendLine("</div>")

                ' Write page
                Me.tblLocationInfo.Text = html.ToString()
            Next

            Exit Sub
        Catch ex As Exception
            HandleError("PopulateLocationInformation", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub PopulateLossHistory()
        Dim html As New StringBuilder()

        ' Start Loss History Section
        html.AppendLine("<div Class='bop_qs_Main_Sections'>")
        'html.AppendLine("<span Class='qs_bop_section_headers'>Buildings</span>")

        html.AppendLine("<div class=""bop_qs_Sub_Sections"">")
        html.AppendLine("<table class=""bop_qa_table_shades"">")

        ' 1st orange column header
        html.AppendLine("<tr class=""qs_bop_section_grid_headers ui-widget-header"">")
        WriteCell(html, indent & "Loss History (" + GoverningStateQuote.LossHistoryRecords.Count.ToString + ")", 3)
        WriteCell(html, "", "")
        html.AppendLine("</tr>")

        ' 2nd orange column header
        html.AppendLine("<tr class=""qs_bop_section_grid_headers ui-widget-header"">")
        WriteCell(html, "Type of Loss", "width:40%")
        WriteCell(html, "Loss Date", "width:24%")
        WriteCell(html, "Loss Amount", "width:23%")
        WriteCell(html, "", "width:13%")
        html.AppendLine("</tr>")

        ' DATA ROWS
        Dim gsq As QuickQuote.CommonObjects.QuickQuoteObject = GoverningStateQuote()
        PopulateLossHistoryDetail(html, gsq.LossHistoryRecords)
        'PopulateLossHistoryDetail(html, Quote.LossHistoryRecords)

        ' Extra line & Close table
        html.AppendLine("<tr><td colspan=3>&nbsp;</td></tr>")
        html.AppendLine("</table>") ' Building coverages table
        html.AppendLine("</div>")
        html.AppendLine("</div>")

        ' Write page
        Me.tblLossHistory.Text = html.ToString()

        ' Spacer line
        'html.AppendLine("<tr><td colspan=4>&nbsp;</td></tr>")

    End Sub

    Private Sub PopulateOptionalLocationCoverages(ByRef html As StringBuilder, ByVal LOC As QuickQuote.CommonObjects.QuickQuoteLocation)
        Try
            html.AppendLine("<div class='bop_qs_Main_Sections'>")
            html.AppendLine("<span Class='qs_bop_section_headers'>Optional Location Coverages</span>")

            html.AppendLine("<div class=""bop_qs_Sub_Sections"">")
            html.AppendLine("<table class=""bop_qa_table_shades"">")
            html.AppendLine("<tr class=""qs_bop_section_grid_headers ui-widget-header"">")

            ' Header Row
            WriteCell(html, "Coverage")
            WriteCell(html, "Limit", "text-align:right", "qs_bop_Grid_cell_premium")
            WriteCell(html, "Premium", "text-align:right;", "qs_bop_Grid_cell_premium")
            html.AppendLine("</tr>")

            ' Data
            ' Equipment Breakdown
            If LOC.EquipmentBreakdownDeductible.Trim <> "" Then
                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, "Equipment Breakdown Deductible: " & LOC.EquipmentBreakdownDeductible)
                WriteCell(html, "")
                WriteCell(html, LOC.EquipmentBreakdownDeductibleQuotedPremium, "text-align:right;")
                html.AppendLine("</tr>")
            End If

            ' Money and Securities
            If LOC.MoneySecuritiesOnPremises.Trim <> "" Then
                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, "Money and Securities - On Premises: ")
                WriteCell(html, LOC.MoneySecuritiesOnPremises, "text-align:right;")
                WriteCell(html, LOC.MoneySecuritiesQuotedPremium_OnPremises, "text-align:right;")
                html.AppendLine("</tr>")
            End If

            If LOC.MoneySecuritiesOffPremises.Trim <> "" Then
                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, "Money and Securities - Off Premises: ")
                WriteCell(html, LOC.MoneySecuritiesOffPremises, "text-align:right;")
                WriteCell(html, LOC.MoneySecuritiesQuotedPremium_OffPremises, "text-align:right;")
                html.AppendLine("</tr>")
            End If

            ' Outdoor Signs
            If LOC.OutdoorSignsLimit.Trim <> "" Then
                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, "Outdoor Signs: " & LOC.OutdoorSignsLimit & " Total Limit")
                WriteCell(html, "")
                WriteCell(html, LOC.OutdoorSignsQuotedPremium, "text-align:right;")
                html.AppendLine("</tr>")
            End If

            ' Amusement Areas
            If LOC.NumberOfAmusementAreas.Trim <> "" Then
                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, "Amusement Areas: " & LOC.NumberOfAmusementAreas)
                WriteCell(html, "")
                WriteCell(html, LOC.AmusementAreasQuotedPremium, "text-align:right;")
                html.AppendLine("</tr>")
            End If

            ' Playgrounds
            If LOC.NumberOfPlaygrounds.Trim <> "" Then
                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, "Playgrounds: " & LOC.NumberOfPlaygrounds)
                WriteCell(html, "")
                WriteCell(html, LOC.PlaygroundsQuotedPremium, "text-align:right;")
                html.AppendLine("</tr>")
            End If

            ' Swimming Pools
            If LOC.NumberOfPools.Trim <> "" Then
                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, "Swimming Pools: " & LOC.NumberOfPools)
                WriteCell(html, "")
                WriteCell(html, LOC.PoolsQuotedPremium, "text-align:right;")
                html.AppendLine("</tr>")
            End If

            html.AppendLine("</table>")
            html.AppendLine("</div>")
            html.AppendLine("</div>")

            Exit Sub
        Catch ex As Exception
            HandleError("PopulateOptionalLocationCoverages", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub PopulateBuildings(ByRef html As StringBuilder, ByVal LOC As QuickQuote.CommonObjects.QuickQuoteLocation)
        Dim err As String = Nothing

        Try

            ' WindHail is set at the location level and applies to all buildings
            Dim windHailPercent = QQHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.WindHailDeductibleLimitId, LOC.WindHailDeductibleLimitId)

            ' 4 Columns:
            ' Header/Data Line 1:   |Building Information                     |Premium      |
            ' Header/Data Line 2:   |Description       |Program    |Classcode               |

            For Each BLD As QuickQuote.CommonObjects.QuickQuoteBuilding In LOC.Buildings
                html.AppendLine("<div Class='bop_qs_Main_Sections'>")
                html.AppendLine("<span Class='qs_bop_section_headers'>Buildings</span>")

                html.AppendLine("<div class=""bop_qs_Sub_Sections"">")
                html.AppendLine("<table class=""bop_qa_table_shades"">")

                ' 1st orange column header
                html.AppendLine("<tr class=""qs_bop_section_grid_headers ui-widget-header"">")
                WriteCell(html, indent & "Building Information", 3)
                WriteCell(html, "Premium", "qs_bop_Grid_cell_premium", "text-align:right;")
                html.AppendLine("</tr>")

                ' 2nd orange column header
                html.AppendLine("<tr class=""qs_bop_section_grid_headers ui-widget-header"">")
                WriteCell(html, "Description", "width:40%")
                WriteCell(html, "Program", "width:24%")
                WriteCell(html, "Class Code", "width:23%")
                WriteCell(html, "", "width:13%")
                html.AppendLine("</tr>")

                ' DATA ROWS
                ' Building

                ' Building Classifications
                If BLD.BuildingClassifications IsNot Nothing Then
                    For Each BCls As QuickQuote.CommonObjects.QuickQuoteClassification In BLD.BuildingClassifications
                        ' Line 1 of the class code - program, class code
                        html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                        WriteCell(html, BLD.Description)
                        WriteCell(html, BLD.Program)
                        WriteCell(html, BLD.ClassCode, 2)
                        html.AppendLine("</tr>")
                        ' Line 2 of the class code - description
                        html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                        Dim txt As String = Nothing
                        If BCls.PredominantOccupancy Then
                            txt = "Classification: " & GetClassificationDescription(BCls.ClassificationTypeId, BCls, err) & " (Primary)"
                        Else
                            txt = "Classification: " & GetClassificationDescription(BCls.ClassificationTypeId, BCls, err)
                        End If
                        WriteCell(html, txt, 4)
                        html.AppendLine("</tr>")
                    Next
                End If

                ' Occupancy
                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, indent & "Occupancy: " & BLD.Occupancy, 4)
                html.AppendLine("</tr>")

                ' Construction
                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, indent & "Construction: " & BLD.Construction, 4)
                html.AppendLine("</tr>")

                ' Automatic Increase
                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, indent & "Automatic Increase: " & BLD.AutoIncrease, 3)
                WriteCell(html, BLD.AutoIncreasePremium, "text-align:right;")
                html.AppendLine("</tr>")

                ' Property Deductible
                If BLD.PropertyDeductible.Trim <> "" Then
                    html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                    WriteCell(html, indent & "Property Deductible: " & BLD.PropertyDeductible, 2)
                    If Not String.IsNullOrEmpty(windHailPercent) AndAlso windHailPercent <> "N/A" Then
                        WriteCell(html, "Wind/Hail Deductible: " & windHailPercent, 2)
                    End If
                    html.AppendLine("</tr>")
                End If

                ' Building Limit
                If BLD.Limit.Trim <> "" Then
                    Dim txt As String = Nothing
                    If BLD.IsBuildingValIncludedInBlanketRating Then
                        txt = "Included in Blanket Rating"
                    Else
                        txt = "Not included in Blanket Rating"
                    End If
                    html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                    WriteCell(html, indent & "Building Limit: " & BLD.Limit & " (" & BLD.Valuation & "/" & txt & ")", 3)
                    WriteCell(html, BLD.LimitQuotedPremium, "text-align:right;")
                    html.AppendLine("</tr>")
                End If
                
                'Mine Subsidence
                  If BLD.HasMineSubsidence = True Then
                    Dim maxLimit As String
                    If Loc.Address.State = "OH"
                        maxLimit = "300,000"
                    Else If LOC.Address.State = "IN"
                         maxLimit = "$500,000"
                    Else If Loc.Address.State = "IL"
                         maxLimit = "$750,000"
                    End If
                 html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                    WriteCell(html, indent & "Mine Subsidence: " & "lesser of " & maxLimit & " or the building limit of insurance", 3)
                    WriteCell(html, BLD.MineSubsidenceQuotedPremium, "text-align:right;")
                    html.AppendLine("</tr>")
                End If

                ' Personal Property Limit
                If BLD.PersonalPropertyLimit.Trim <> "" Then
                    Dim txt As String = Nothing
                    If BLD.IsValMethodIncludedInBlanketRating Then
                        txt = "Included in Blanket Rating"
                    Else
                        txt = "Not included in Blanket Rating"
                    End If
                    html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                    WriteCell(html, indent & "Personal Property Limit: " & BLD.PersonalPropertyLimit & " (" & BLD.ValuationMethod & "/" & txt & ")", 3)
                    WriteCell(html, BLD.PersonalPropertyLimitQuotedPremium, "text-align:right;")
                    html.AppendLine("</tr>")
                End If

                ' Additional Interest
                If BLD.PropertyDeductible.Trim <> "" Then
                    Dim txt As String = Nothing
                    If BLD.AdditionalInterests Is Nothing OrElse BLD.AdditionalInterests.Count <= 0 Then txt = "No" Else txt = "Yes"
                    html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                    WriteCell(html, indent & "Additional Interest: " & txt, 4)
                    html.AppendLine("</tr>")
                End If

                ' Spacer line
                html.AppendLine("<tr><td colspan=4>&nbsp;</td></tr>")

                ' Close the table and the divs
                html.AppendLine("</table>")
                html.AppendLine("</div>")
                html.AppendLine("</div>")

                ' Create the coverages table
                html.AppendLine("<table style=float:right;width:95%; class='bop_qa_table_shades'>")
                ' Orange Header
                html.AppendLine("<tr class=""qs_bop_section_grid_headers ui-widget-header"">")
                WriteCell(html, "Optional Building Coverages", "width:44%")
                WriteCell(html, "", "width:43%")
                WriteCell(html, "Premium", "width:13%;text-align:right;")
                html.AppendLine("</tr>")

                ' Coverages data
                PopulateBuildingCoverages(html, BLD, LOC)

                ' Extra line & Close table
                html.AppendLine("<tr><td colspan=3>&nbsp;</td></tr>")
                html.AppendLine("</table>") ' Building coverages table

            Next

            Exit Sub
        Catch ex As Exception
            HandleError("PopulateBuildings", ex)
            Exit Sub
        End Try
    End Sub

    Private Sub PopulateBuildingCoverages(ByRef html As StringBuilder, ByVal BLD As QuickQuote.CommonObjects.QuickQuoteBuilding, ByVal LOC As QuickQuote.CommonObjects.QuickQuoteLocation)
        Try

            ' BUILDING COVERAGES
            ' Accounts Receivable - On premises
            If BLD.AccountsReceivableOnPremises.Trim <> "" Then
                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, "Accounts Receivable:", 3)
                html.AppendLine("</tr>")

                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, indent & BLD.AccountsReceivableOnPremisesExcessLimit & " On Premises", 2)
                WriteCell(html, BLD.AccountsReceivableQuotedPremium, "text-align:right;")
                html.AppendLine("</tr>")

                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, indent & BLD.AccountsReceivableOffPremises & " Off Premises", 2)
                WriteCell(html, "")
                html.AppendLine("</tr>")
            End If

            ' Valuable Papers
            If BLD.ValuablePapersOnPremises.Trim <> "" Then
                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, "Valuable Papers:", 3)
                html.AppendLine("</tr>")

                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, indent & BLD.ValuablePapersOnPremisesExcessLimit & " On Premises", 2)
                WriteCell(html, BLD.ValuablePapersQuotedPremium, "text-align:right;")
                html.AppendLine("</tr>")

                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, indent & BLD.ValuablePapersOffPremises & " Off Premises", 2)
                WriteCell(html, "")
                html.AppendLine("</tr>")
            End If

            ' Ordinance or Law
            If BLD.HasOrdinanceOrLaw Then
                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, "Ordinance or Law:", 3)
                html.AppendLine("</tr>")

                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, indent & "Loss to the Undamaged Portion of the Building - Undamaged", 2)
                WriteCell(html, BLD.OrdOrLawUndamagedPortionQuotedPremium, "text-align:right;")
                html.AppendLine("</tr>")

                If BLD.OrdOrLawDemoCostLimit.Trim <> "" Then
                    html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                    WriteCell(html, indent & BLD.OrdOrLawDemoCostLimit & " Demolition Cost", 2)
                    WriteCell(html, BLD.OrdOrLawDemoCostLimitQuotedPremium, "text-align:right;")
                    html.AppendLine("</tr>")
                End If

                If BLD.OrdOrLawIncreasedCostLimit.Trim <> "" Then
                    html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                    WriteCell(html, indent & BLD.OrdOrLawIncreasedCostLimit & " Increased Cost of Construction", 2)
                    WriteCell(html, BLD.OrdOrLawIncreaseCostLimitQuotedPremium, "text-align:right;")
                    html.AppendLine("</tr>")
                End If

                If BLD.OrdOrLawDemoAndIncreasedCostLimit.Trim <> "" Then
                    html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                    WriteCell(html, indent & BLD.OrdOrLawDemoAndIncreasedCostLimit & " Demolition and Increased Cost Combined", 2)
                    WriteCell(html, BLD.OrdOrLawDemoAndIncreasedCostLimitQuotedPremium, "text-align:right;")
                    html.AppendLine("</tr>")
                End If
            End If

            ' Spoilage
            If BLD.HasSpoilage Then
                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, "Spoilage:", 2)
                WriteCell(html, BLD.SpoilageQuotedPremium, "text-align:right;")
                html.AppendLine("</tr>")

                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, indent & BLD.SpoilagePropertyClassification, 2)
                WriteCell(html, "")
                html.AppendLine("</tr>")

                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, indent & BLD.SpoilageTotalLimit & " Total Limit", 2)
                WriteCell(html, "")
                html.AppendLine("</tr>")

                If BLD.IsSpoilageRefrigerationMaintenanceAgreement Then
                    html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                    WriteCell(html, indent & "Refrigeration Maintenance Agreement", 3)
                    html.AppendLine("</tr>")
                End If

                If BLD.IsSpoilageBreakdownOrContamination Then
                    html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                    WriteCell(html, indent & "Breakdown or Contamination", 3)
                    html.AppendLine("</tr>")
                End If

                If BLD.IsSpoilagePowerOutage Then
                    html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                    WriteCell(html, indent & "Power Outage", 3)
                    html.AppendLine("</tr>")
                End If
            End If

            ' PROFESSIONAL LIABILITY COVERAGES 
            ' NOTE that we only show the premium on the first building that has the coverage
            ' Apartments
            If BLD.HasApartmentBuildings AndAlso BuildingIsFirstOnStateQuoteWithPLCoverage(LOC.Address.QuickQuoteState, BLD, PLCoverages_Enum.Apartments) Then
                Dim StateQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(LOC.Address.QuickQuoteState)
                Dim prem As Integer = 0
                Dim premtext As String = ""
                If StateQuote IsNot Nothing Then
                    If IsNumeric(StateQuote.ApartmentQuotedPremium) Then prem += CDec(StateQuote.ApartmentQuotedPremium)
                    If IsNumeric(StateQuote.TenantAutoLegalQuotedPremium) Then prem += CDec(StateQuote.TenantAutoLegalQuotedPremium)
                End If
                If prem >= 0 Then premtext = Format(prem, "c")
                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, "Apartment:", 2)
                If BuildingIsFirstOnStateQuoteWithPLCoverage(LOC.Address.QuickQuoteState, BLD, PLCoverages_Enum.Apartments) Then
                    'WriteCell(html, Quote.ApartmentQuotedPremium)
                    WriteCell(html, Format(premtext), "text-align:right;")
                Else
                    WriteCell(html, "")
                End If
                html.AppendLine("</tr>")

                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, indent & "Location with Apartments " & BLD.NumberOfLocationsWithApartments, 3)
                html.AppendLine("</tr>")
            End If

            ' Barbers
            If BLD.HasBarbersProfessionalLiability AndAlso BuildingIsFirstOnStateQuoteWithPLCoverage(LOC.Address.QuickQuoteState, BLD, PLCoverages_Enum.Barbers) Then
                Dim StateQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(LOC.Address.QuickQuoteState)
                Dim prem As String = ""
                If StateQuote IsNot Nothing Then prem = StateQuote.BarbersProfessionalLiabiltyQuotedPremium
                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, "Barbers Professional Liability:", 2)
                If BuildingIsFirstOnStateQuoteWithPLCoverage(LOC.Address.QuickQuoteState, BLD, PLCoverages_Enum.Barbers) Then
                    WriteCell(html, prem, "text-align:right;")
                Else
                    WriteCell(html, "")
                End If
                html.AppendLine("</tr>")

                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, indent & "Number of Full Time Employees " & BLD.BarbersProfessionalLiabilityFullTimeEmpNum, 3)
                html.AppendLine("</tr>")

                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, indent & "Number of Part Time Employees " & BLD.BarbersProfessionalLiabilityPartTimeEmpNum, 3)
                html.AppendLine("</tr>")
            End If

            ' Beauticians
            If BLD.HasBeauticiansProfessionalLiability AndAlso BuildingIsFirstOnStateQuoteWithPLCoverage(LOC.Address.QuickQuoteState, BLD, PLCoverages_Enum.Beauticans) Then
                Dim StateQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(LOC.Address.QuickQuoteState)
                Dim prem As String = ""
                If StateQuote IsNot Nothing Then prem = StateQuote.BeauticiansProfessionalLiabilityQuotedPremium
                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, "Beauticians Professional Liability:", 2)
                If BuildingIsFirstOnStateQuoteWithPLCoverage(LOC.Address.QuickQuoteState, BLD, PLCoverages_Enum.Beauticans) Then
                    WriteCell(html, prem, "text-align:right;")
                Else
                    WriteCell(html, "")
                End If
                html.AppendLine("</tr>")

                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, indent & "Number of Full Time Employees " & BLD.BeauticiansProfessionalLiabilityFullTimeEmpNum, 3)
                html.AppendLine("</tr>")

                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, indent & "Number of Part Time Employees " & BLD.BeauticiansProfessionalLiabilityPartTimeEmpNum, 3)
                html.AppendLine("</tr>")
            End If

            ' Fine Arts
            If BLD.HasFineArts AndAlso BuildingIsFirstOnStateQuoteWithPLCoverage(LOC.Address.QuickQuoteState, BLD, PLCoverages_Enum.FineArts) Then
                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, "Fine Arts:", 2)
                If BuildingIsFirstOnStateQuoteWithPLCoverage(LOC.Address.QuickQuoteState, BLD, PLCoverages_Enum.FineArts) Then
                    WriteCell(html, LOC.FineArtsQuotedPremium, "text-align:right;")
                Else
                    WriteCell(html, "")
                End If
                html.AppendLine("</tr>")
            End If

            ' Funeral Directors
            If BLD.HasFuneralDirectorsProfessionalLiability AndAlso BuildingIsFirstOnStateQuoteWithPLCoverage(LOC.Address.QuickQuoteState, BLD, PLCoverages_Enum.Funeral) Then
                Dim StateQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(LOC.Address.QuickQuoteState)
                Dim prem As String = ""
                If StateQuote IsNot Nothing Then prem = StateQuote.FuneralDirectorsProfessionalLiabilityQuotedPremium
                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, "Funeral Directors Professional Liability:", 2)
                If BuildingIsFirstOnStateQuoteWithPLCoverage(LOC.Address.QuickQuoteState, BLD, PLCoverages_Enum.Funeral) Then
                    WriteCell(html, prem, "text-align:right;")
                Else
                    WriteCell(html, "")
                End If
                html.AppendLine("</tr>")

                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, indent & "Number of Employees " & BLD.FuneralDirectorsProfessionalLiabilityEmpNum, 3)
                html.AppendLine("</tr>")
            End If

            ' Liquor Liability
            If IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then
                ' Multistate - get the values from the state quotes
                If BLD.HasLiquorLiability AndAlso BuildingIsFirstOnStateQuoteWithPLCoverage(LOC.Address.QuickQuoteState, BLD, PLCoverages_Enum.Liquor) Then
                    Dim StateQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(LOC.Address.QuickQuoteState)
                    Dim pkg As Decimal = 0
                    Dim alc As Decimal = 0
                    Dim tot As Decimal = 0

                    If StateQuote IsNot Nothing Then
                        If IsNumeric(StateQuote.LiquorLiabilityAnnualGrossAlcoholSalesReceipts) Then alc = CDec(StateQuote.LiquorLiabilityAnnualGrossAlcoholSalesReceipts)
                        If IsNumeric(StateQuote.LiquorLiabilityAnnualGrossPackageSalesReceipts) Then pkg = CDec(StateQuote.LiquorLiabilityAnnualGrossPackageSalesReceipts)
                        tot = pkg + alc

                        html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                        WriteCell(html, "Liquor Liability:", 2)
                        If BuildingIsFirstOnStateQuoteWithPLCoverage(LOC.Address.QuickQuoteState, BLD, PLCoverages_Enum.Liquor) Then
                            WriteCell(html, StateQuote.LiquorLiabilityQuotedPremium, "text-align:right;")
                        Else
                            WriteCell(html, "")
                        End If
                        html.AppendLine("</tr>")

                        html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                        ' note that we used to display quote.LiquorLiabilityOccurrenceLimit but that field is not being set correctly for IL so I have hardcoded the values
                        Select Case LOC.Address.QuickQuoteState
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
                                If CDate(Quote.EffectiveDate) < CDate("4/1/2020") Then
                                    WriteCell(html, indent & "Limit 69/69/85", 3)
                                Else
                                    WriteCell(html, indent & "Limit: REFER TO ILLINOIS STATUTORY LIMITS", 3)
                                End If
                                Exit Select
                            Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
                                WriteCell(html, indent & "Limit $1,000,000", 3)
                                Exit Select
                        End Select
                        'WriteCell(html, indent & "Limit " & Quote.LiquorLiabilityOccurrenceLimit, 3)
                        html.AppendLine("</tr>")

                        html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                        'WriteCell(html, indent & "Annual Gross Alcohol Sales " & Quote.LiquorLiabilityAnnualGrossAlcoholSalesReceipts, 3)
                        WriteCell(html, indent & "Annual Gross Alcohol Sales " & Format(tot, "###,###,###,##0"), 3)
                        html.AppendLine("</tr>")
                    End If

                End If
            Else
                ' Not Multistate - get the values from the quote object
                If BLD.HasLiquorLiability AndAlso BuildingIsFirstOnStateQuoteWithPLCoverage(LOC.Address.QuickQuoteState, BLD, PLCoverages_Enum.Liquor) Then
                    Dim pkg As Decimal = 0
                    Dim alc As Decimal = 0
                    Dim tot As Decimal = 0
                    If IsNumeric(Quote.LiquorLiabilityAnnualGrossAlcoholSalesReceipts) Then alc = CDec(Quote.LiquorLiabilityAnnualGrossAlcoholSalesReceipts)
                    If IsNumeric(Quote.LiquorLiabilityAnnualGrossPackageSalesReceipts) Then pkg = CDec(Quote.LiquorLiabilityAnnualGrossPackageSalesReceipts)
                    tot = pkg + alc

                    html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                    WriteCell(html, "Liquor Liability:", 2)
                    If BuildingIsFirstOnStateQuoteWithPLCoverage(LOC.Address.QuickQuoteState, BLD, PLCoverages_Enum.Liquor) Then
                        WriteCell(html, Quote.LiquorLiabilityQuotedPremium, "text-align:right;")
                    Else
                        WriteCell(html, "")
                    End If
                    html.AppendLine("</tr>")

                    html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                    ' note that we used to display quote.LiquorLiabilityOccurrenceLimit but that field is not being set correctly for IL so I have hardcoded the values
                    Select Case LOC.Address.QuickQuoteState
                        Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois
                            WriteCell(html, indent & "Limit 69/69/85", 3)
                            Exit Select
                        Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
                            WriteCell(html, indent & "Limit $1,000,000", 3)
                            Exit Select
                    End Select
                    'WriteCell(html, indent & "Limit " & Quote.LiquorLiabilityOccurrenceLimit, 3)
                    html.AppendLine("</tr>")

                    html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                    'WriteCell(html, indent & "Annual Gross Alcohol Sales " & Quote.LiquorLiabilityAnnualGrossAlcoholSalesReceipts, 3)
                    WriteCell(html, indent & "Annual Gross Alcohol Sales " & Format(tot, "###,###,###,##0"), 3)
                    html.AppendLine("</tr>")
                End If
            End If

            ' Motels
            If BLD.HasMotelCoverage AndAlso BuildingIsFirstOnStateQuoteWithPLCoverage(LOC.Address.QuickQuoteState, BLD, PLCoverages_Enum.Motel) Then
                Dim StateQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(LOC.Address.QuickQuoteState)
                Dim totPrem As Decimal = 0

                If StateQuote.MotelCoveragePerGuestQuotedPremium IsNot Nothing AndAlso IsNumeric(StateQuote.MotelCoveragePerGuestQuotedPremium) Then
                    totPrem += CDec(StateQuote.MotelCoveragePerGuestQuotedPremium)
                End If
                If StateQuote.MotelCoverageSafeDepositQuotedPremium IsNot Nothing AndAlso IsNumeric(StateQuote.MotelCoverageSafeDepositQuotedPremium) Then
                    totPrem += CDec(StateQuote.MotelCoverageSafeDepositQuotedPremium)
                End If

                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, "Motel:", 2)
                If BuildingIsFirstOnStateQuoteWithPLCoverage(LOC.Address.QuickQuoteState, BLD, PLCoverages_Enum.Motel) Then
                    'WriteCell(html, Quote.MotelCoverageQuotedPremium)
                    WriteCell(html, Format(totPrem, "c"), "text-align:right;")
                Else
                    WriteCell(html, "")
                End If
                html.AppendLine("</tr>")

                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, indent & "Guest Property Limit " & StateQuote.MotelCoveragePerGuestLimit, 3)
                html.AppendLine("</tr>")

                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, indent & "Safe Deposit Box Deductible " & StateQuote.MotelCoverageSafeDepositDeductible, 3)
                html.AppendLine("</tr>")

                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, indent & "Safe Deposit Box Limit " & StateQuote.MotelCoverageSafeDepositLimit, 3)
                html.AppendLine("</tr>")
            End If

            ' Optical & Hearing Aid
            If BLD.HasOpticalAndHearingAidProfessionalLiability AndAlso BuildingIsFirstOnStateQuoteWithPLCoverage(LOC.Address.QuickQuoteState, BLD, PLCoverages_Enum.Optical) Then
                Dim StateQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(LOC.Address.QuickQuoteState)
                Dim prem As String = ""
                If StateQuote IsNot Nothing Then prem = StateQuote.OpticalAndHearingAidProfessionalLiabilityQuotedPremium

                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, "Optical & Hearing Aid Professional Liability:", 2)
                If BuildingIsFirstOnStateQuoteWithPLCoverage(LOC.Address.QuickQuoteState, BLD, PLCoverages_Enum.Optical) Then
                    WriteCell(html, prem, "text-align:right;")
                Else
                    WriteCell(html, "")
                End If
                html.AppendLine("</tr>")

                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, indent & "Number of Employees " & BLD.OpticalAndHearingAidProfessionalLiabilityEmpNum, 3)
                html.AppendLine("</tr>")
            End If

            ' Pharmacists
            If BLD.HasPharmacistProfessionalLiability AndAlso BuildingIsFirstOnStateQuoteWithPLCoverage(LOC.Address.QuickQuoteState, BLD, PLCoverages_Enum.Pharmacist) Then
                Dim StateQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(LOC.Address.QuickQuoteState)
                Dim prem As String = ""
                If StateQuote IsNot Nothing Then prem = StateQuote.PharmacistQuotedPremium

                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, "Pharmacist Professional Liability:", 2)
                If BuildingIsFirstOnStateQuoteWithPLCoverage(LOC.Address.QuickQuoteState, BLD, PLCoverages_Enum.Pharmacist) Then
                    WriteCell(html, prem, "text-align:right;")
                Else
                    WriteCell(html, "")
                End If
                html.AppendLine("</tr>")

                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, indent & "Receipts " & BLD.PharmacistAnnualGrossSales, 3)
                html.AppendLine("</tr>")
            End If

            ' Photography
            If BLD.HasPhotographyCoverage AndAlso BuildingIsFirstOnStateQuoteWithPLCoverage(LOC.Address.QuickQuoteState, BLD, PLCoverages_Enum.Photography) Then
                Dim StateQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(LOC.Address.QuickQuoteState)
                Dim prem As String = ""
                If StateQuote IsNot Nothing Then prem = StateQuote.PhotographyCoverageQuotedPremium

                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, "Photographic Equipment:", 2)
                If BuildingIsFirstOnStateQuoteWithPLCoverage(LOC.Address.QuickQuoteState, BLD, PLCoverages_Enum.Photography) Then
                    WriteCell(html, prem, "text-align:right;")
                Else
                    WriteCell(html, "")
                End If
                html.AppendLine("</tr>")

                If BLD.HasPhotographyCoverageScheduledCoverages Then
                    html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                    WriteCell(html, indent & "Schedule of Equipment " & BLD.PhotographyTotalScheduledLimits, 3)
                    html.AppendLine("</tr>")
                End If

                If BLD.HasPhotographyMakeupAndHair Then
                    html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                    WriteCell(html, indent & "Makeup and Hair", 3)
                    html.AppendLine("</tr>")
                End If
            End If

            ' Printers
            If BLD.HasPrintersProfessionalLiability AndAlso BuildingIsFirstOnStateQuoteWithPLCoverage(LOC.Address.QuickQuoteState, BLD, PLCoverages_Enum.Printers) Then
                Dim StateQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(LOC.Address.QuickQuoteState)
                Dim prem As String = ""
                If StateQuote IsNot Nothing Then prem = StateQuote.PrintersProfessionalLiabilityQuotedPremium

                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, "Printers Errors & Omissions Liability:", 2)
                If BuildingIsFirstOnStateQuoteWithPLCoverage(LOC.Address.QuickQuoteState, BLD, PLCoverages_Enum.Printers) Then
                    WriteCell(html, prem, "text-align:right;")
                Else
                    WriteCell(html, "")
                End If
                html.AppendLine("</tr>")

                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, indent & "Number of Locations " & BLD.PrintersProfessionalLiabilityLocNum, 3)
                html.AppendLine("</tr>")
            End If

            ' Residential Cleaning
            If BLD.HasResidentialCleaning AndAlso BuildingIsFirstOnStateQuoteWithPLCoverage(LOC.Address.QuickQuoteState, BLD, PLCoverages_Enum.ResidentialCleaning) Then
                Dim StateQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(LOC.Address.QuickQuoteState)
                Dim prem As String = ""
                If StateQuote IsNot Nothing Then prem = StateQuote.ResidentialCleaningQuotedPremium

                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, "Residential Cleaning:", 2)
                If BuildingIsFirstOnStateQuoteWithPLCoverage(LOC.Address.QuickQuoteState, BLD, PLCoverages_Enum.ResidentialCleaning) Then
                    WriteCell(html, prem, "text-align:right;")
                Else
                    WriteCell(html, "")
                End If
                html.AppendLine("</tr>")
            End If

            ' Restaurants
            If BLD.HasRestaurantEndorsement AndAlso BuildingIsFirstOnStateQuoteWithPLCoverage(LOC.Address.QuickQuoteState, BLD, PLCoverages_Enum.Restaurants) Then
                Dim StateQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(LOC.Address.QuickQuoteState)
                Dim premiumsToCombine As New List(Of String) From {StateQuote.CustomerAutoLegalQuotedPremium, StateQuote.RestaurantQuotedPremium}
                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, "Restaurant:", 2)
                If BuildingIsFirstOnStateQuoteWithPLCoverage(LOC.Address.QuickQuoteState, BLD, PLCoverages_Enum.Restaurants) Then
                    WriteCell(html, IFM.VR.Common.Helpers.QuickQuoteObjectHelper.SumMoneyStrings(premiumsToCombine), "text-align:right;") 'Quote.RestaurantQuotedPremium + Quote.CustomerAutoLegalQuotedPremium
                Else
                    WriteCell(html, "")
                End If
                html.AppendLine("</tr>")
            End If

            ' Self Storage
            If BLD.HasSelfStorageFacility AndAlso BuildingIsFirstOnStateQuoteWithPLCoverage(LOC.Address.QuickQuoteState, BLD, PLCoverages_Enum.SelfStorage) Then
                Dim StateQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(LOC.Address.QuickQuoteState)
                Dim prem As String = ""
                If StateQuote IsNot Nothing Then prem = StateQuote.SelfStorageFacilityQuotedPremium

                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, "Self-Storage Facility: " & BLD.SelfStorageFacilityLimit, 2)
                If BuildingIsFirstOnStateQuoteWithPLCoverage(LOC.Address.QuickQuoteState, BLD, PLCoverages_Enum.SelfStorage) Then
                    WriteCell(html, prem, "text-align:right;")
                Else
                    WriteCell(html, "")
                End If
                html.AppendLine("</tr>")
            End If

            ' Veterinarians
            If BLD.HasVeterinariansProfessionalLiability AndAlso BuildingIsFirstOnStateQuoteWithPLCoverage(LOC.Address.QuickQuoteState, BLD, PLCoverages_Enum.Veterinarians) Then
                Dim StateQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(LOC.Address.QuickQuoteState)
                Dim prem As String = ""
                If StateQuote IsNot Nothing Then prem = StateQuote.VeterinariansProfessionalLiabilityQuotedPremium

                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, "Veterinarians Professional Liability:", 2)
                If BuildingIsFirstOnStateQuoteWithPLCoverage(LOC.Address.QuickQuoteState, BLD, PLCoverages_Enum.Veterinarians) Then
                    WriteCell(html, prem, "text-align:right;")
                Else
                    WriteCell(html, "")
                End If
                html.AppendLine("</tr>")

                html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
                WriteCell(html, "Number of Employees " & BLD.VeterinariansProfessionalLiabilityEmpNum, 3)
                html.AppendLine("</tr>")
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("PopulateBuildingCoverages", ex)
            Exit Sub
        End Try
    End Sub


    Private Function BuildingIsFirstOnStateQuoteWithPLCoverage(ByVal qqState As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState, ByVal BLD As QuickQuote.CommonObjects.QuickQuoteBuilding, PLCov As PLCoverages_Enum) As Boolean
        Dim LocNdx As Integer = -1
        Dim BldNdx As Integer = -1
        Dim StateQuote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing

        Try
            If Quote IsNot Nothing Then
                StateQuote = SubQuoteForState(qqState)

                If StateQuote IsNot Nothing AndAlso StateQuote.Locations IsNot Nothing Then
                    For Each L As QuickQuote.CommonObjects.QuickQuoteLocation In StateQuote.Locations
                        LocNdx += 1
                        For Each B As QuickQuote.CommonObjects.QuickQuoteBuilding In L.Buildings
                            BldNdx += 1
                            Select Case PLCov
                                Case PLCoverages_Enum.Apartments
                                    If B.HasApartmentBuildings Then
                                        If BLD.Equals(B) Then
                                            Return True
                                        Else
                                            Return False
                                        End If
                                    End If
                                    Exit Select
                                Case PLCoverages_Enum.Barbers
                                    If B.HasBarbersProfessionalLiability Then
                                        If BLD.Equals(B) Then
                                            Return True
                                        Else
                                            Return False
                                        End If
                                    End If
                                    Exit Select
                                Case PLCoverages_Enum.Beauticans
                                    If B.HasBeauticiansProfessionalLiability Then
                                        If BLD.Equals(B) Then
                                            Return True
                                        Else
                                            Return False
                                        End If
                                    End If
                                    Exit Select
                                Case PLCoverages_Enum.FineArts
                                    If B.HasFineArts Then
                                        If BLD.Equals(B) Then
                                            Return True
                                        Else
                                            Return False
                                        End If
                                    End If
                                    Exit Select
                                Case PLCoverages_Enum.Funeral
                                    If B.HasFuneralDirectorsProfessionalLiability Then
                                        If BLD.Equals(B) Then
                                            Return True
                                        Else
                                            Return False
                                        End If
                                    End If
                                    Exit Select
                                Case PLCoverages_Enum.Liquor
                                    If B.HasLiquorLiability Then
                                        If BLD.Equals(B) Then
                                            Return True
                                        Else
                                            Return False
                                        End If
                                    End If
                                    Exit Select
                                Case PLCoverages_Enum.MakeupAndHair
                                    If B.HasPhotographyMakeupAndHair Then
                                        If BLD.Equals(B) Then
                                            Return True
                                        Else
                                            Return False
                                        End If
                                    End If
                                    Exit Select
                                Case PLCoverages_Enum.Motel
                                    If B.HasMotelCoverage Then
                                        If BLD.Equals(B) Then
                                            Return True
                                        Else
                                            Return False
                                        End If
                                    End If
                                    Exit Select
                                Case PLCoverages_Enum.Optical
                                    If B.HasOpticalAndHearingAidProfessionalLiability Then
                                        If BLD.Equals(B) Then
                                            Return True
                                        Else
                                            Return False
                                        End If
                                    End If
                                    Exit Select
                                Case PLCoverages_Enum.Pharmacist
                                    If B.HasPharmacistProfessionalLiability Then
                                        If BLD.Equals(B) Then
                                            Return True
                                        Else
                                            Return False
                                        End If
                                    End If
                                    Exit Select
                                Case PLCoverages_Enum.Photography
                                    If B.HasPhotographyCoverage Then
                                        If BLD.Equals(B) Then
                                            Return True
                                        Else
                                            Return False
                                        End If
                                    End If
                                    Exit Select
                                Case PLCoverages_Enum.Printers
                                    If B.HasPrintersProfessionalLiability Then
                                        If BLD.Equals(B) Then
                                            Return True
                                        Else
                                            Return False
                                        End If
                                    End If
                                    Exit Select
                                Case PLCoverages_Enum.ResidentialCleaning
                                    If B.HasResidentialCleaning Then
                                        If BLD.Equals(B) Then
                                            Return True
                                        Else
                                            Return False
                                        End If
                                    End If
                                    Exit Select
                                Case PLCoverages_Enum.Restaurants
                                    If B.HasRestaurantEndorsement Then
                                        If BLD.Equals(B) Then
                                            Return True
                                        Else
                                            Return False
                                        End If
                                    End If
                                    Exit Select
                                Case PLCoverages_Enum.SelfStorage
                                    If B.HasSelfStorageFacility Then
                                        If BLD.Equals(B) Then
                                            Return True
                                        Else
                                            Return False
                                        End If
                                    End If
                                    Exit Select
                                Case PLCoverages_Enum.Veterinarians
                                    If B.HasVeterinariansProfessionalLiability Then
                                        If BLD.Equals(B) Then
                                            Return True
                                        Else
                                            Return False
                                        End If
                                    End If
                                    Exit Select
                                Case Else
                                    Return False
                            End Select
                        Next
                    Next
                End If
            End If

            Return False
        Catch ex As Exception
            HandleError("BuildingIsFirstOnStateQuoteWithPLCoverage", ex)
            Return False
        End Try
    End Function


    Private Function GetClassificationDescription(ByVal dia_classificationtype_id As String, ByVal BuildingClassification As QuickQuoteClassification, ByRef err As String) As String
        'Dim conn As New SqlConnection()
        'Dim cmd As New SqlCommand()
        'Dim da As New SqlDataAdapter()
        'Dim tbl As New DataTable()

        Try

            Dim spQuery = New IFM.VR.Common.Helpers.BOP.QueryHelper()
            Dim Data = spQuery.GetClassDescFromClassification(QQHelper.IntegerForString(dia_classificationtype_id))
            'If String.IsNullOrWhiteSpace(Data) Then Throw New Exception("No matching classification record found")
            If String.IsNullOrWhiteSpace(Data) Then
                If BuildingClassification IsNot Nothing Then
                    Return BuildingClassification.Description
                Else
                    Return String.Empty
                End If
            Else
                Return Data
            End If

            '    conn = New SqlConnection(AppSettings("connQQ"))
            '    conn.Open()
            '    cmd.Connection = conn
            '    'cmd.CommandType = CommandType.Text
            '    'cmd.CommandText = "SELECT ClassDesc FROM BOPClassNew WHERE dia_classificationtype_id = " & dia_classificationtype_id & " AND UsedInNewBop = 1"
            '    cmd.CommandText = "usp_BOPCLASSNEW_ClassCode_Classification_Search"
            '    cmd.CommandType = CommandType.StoredProcedure
            '    cmd.Parameters.AddWithValue("@dia_classificationtype_id", QQHelper.IntegerForString(dia_classificationtype_id))
            '    cmd.Parameters.AddWithValue("@UsedInNewBOP", 1)
            '    da = New SqlDataAdapter()
            '    da.SelectCommand = cmd
            '    da.Fill(tbl)

            '    If tbl Is Nothing OrElse tbl.Rows.Count < 0 Then Throw New Exception("No matching classification record found")

            '    Return tbl.Rows(0)("ClassDesc").ToString
        Catch ex As Exception
            err = ex.Message
            'HandleError("GetClassificationDescription", ex)
            If BuildingClassification IsNot Nothing Then
                Return BuildingClassification.Description
            Else
                Return String.Empty
            End If
            'Finally
            '    If conn.State = ConnectionState.Open Then conn.Close()
            '    conn.Dispose()
            '    cmd.Dispose()
            '    da.Dispose()
            '    tbl.Dispose()
        End Try
    End Function

    Private Sub PopulateLossHistoryDetail(ByRef html As StringBuilder, ByVal Hist As List(Of QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord))

        'Public Function GetStaticDataTextForValue(options As List(Of QuickQuoteStaticDataOption), value As String, ByRef foundValue As Boolean) As String
        For Each loss As QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord In Hist
            Dim LossType = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLossHistoryRecord, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.TypeOfLossId, loss.TypeOfLossId)

            html.AppendLine("<tr class=""qs_bop_basic_info_labels_cell"">")
            WriteCell(html, LossType)
            WriteCell(html, loss.LossDate)
            WriteCell(html, FormatCurrency(loss.Amount), 2)
            html.AppendLine("</tr>")
        Next


    End Sub

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