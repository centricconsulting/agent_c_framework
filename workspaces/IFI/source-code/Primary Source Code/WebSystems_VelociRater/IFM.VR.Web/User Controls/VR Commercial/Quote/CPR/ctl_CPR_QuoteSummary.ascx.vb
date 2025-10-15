Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.CPR
Imports IFM.VR.Common.Helpers.MultiState.General
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass

Public Class ctl_CPR_QuoteSummary
    Inherits VRControlBase

    Dim indent As String = "&nbsp;&nbsp;"
    Dim dblindent As String = "&nbsp;&nbsp;&nbsp;&nbsp;"
    Public Overrides Sub AddScriptAlways()

    End Sub
    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion("divCPRQuoteSummary", HiddenField1, "0")
        Me.VRScript.StopEventPropagation(Me.lnkPrint.ClientID, False)
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        If Me.Quote IsNot Nothing Then
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

            Me.lblPremiumMainAccord.Text = String.Format("{0:C2}", Me.Quote.TotalQuotedPremium)

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

            If IsOnAppPage Then
                lblPrintReminder.Visible = True
            End If

            PopulatePolicyLevelCoverages()
            ctlPolicyDiscounts.Populate()
            PopulateLocationInformation()

            If IsOnAppPage AndAlso Me.GoverningStateQuote IsNot Nothing AndAlso Me.GoverningStateQuote.LossHistoryRecords IsNot Nothing AndAlso Me.GoverningStateQuote.LossHistoryRecords.Count > 0 Then
                PopulateLossHistory()
            End If

            Me.ctlQuoteSummaryActions.Populate()
        End If
    End Sub

    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Me.UseRatedQuoteImage = True
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Function Save() As Boolean
        Return True
    End Function

#Region "Cell Creation Functions"

    Private Sub WriteCell(sb As StringBuilder, cellText As String)
        sb.AppendLine("<td>")
        sb.AppendLine(cellText)
        sb.AppendLine("</td>")
    End Sub

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

    Private Sub WriteCell(sb As StringBuilder, cellText As String, styleText As String, cssclass As String)
        sb.AppendLine("<td style=""" + styleText + """ class=""" + cssclass + """>")
        sb.AppendLine(cellText)
        sb.AppendLine("</td>")
    End Sub

#End Region

    Private Sub PopulatePolicyLevelCoverages()
        If Quote IsNot Nothing AndAlso SubQuoteFirst IsNot Nothing Then
            'Intialize
            rowEnhEndo.Visible = False
            rowBlanket.Visible = False
            rowMenPremAdj.Visible = False
            rowPropPlusEnhEndo.Visible = False

            'Static Elements
            'none

            'Dynamic Elements
            If SubQuoteFirst.HasBusinessMasterEnhancement = True Then
                rowEnhEndo.Visible = True
                tdEnhEndoPremium.InnerText = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.BusinessMasterEnhancementQuotedPremium)
            End If

            'Property Plus Enhancement
            If SubQuoteFirst.Has_PackageCPR_PlusEnhancementEndorsement Then
                rowPropPlusEnhEndo.Visible = True
                tdPropertyPlusEnhancementEndorsementPremium.InnerText = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.PackageCPR_PlusEnhancementEndorsementQuotedPremium)
            End If

            If SubQuoteFirst.HasBlanketBuildingAndContents OrElse SubQuoteFirst.HasBlanketBuilding OrElse SubQuoteFirst.HasBlanketContents Then
                Me.BlanketType.InnerText = "N/A"
                If SubQuoteFirst.HasBlanketBuildingAndContents = True Then
                    Me.BlanketType.InnerText = "Combined Building and Personal Property"
                ElseIf SubQuoteFirst.HasBlanketBuilding = True Then
                    Me.BlanketType.InnerText = "Building Only"
                ElseIf SubQuoteFirst.HasBlanketContents = True Then
                    Me.BlanketType.InnerText = "Personal Property Only"
                End If
                Me.tdBlanketQuotedPremium.InnerText = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.CPR_BlanketCoverages_TotalPremium)
                rowBlanket.Visible = True
            End If

            If IFM.Common.InputValidation.CommonValidations.IsPositiveNumber(Quote.MinimumPremiumAdjustment) = True Then
                rowMenPremAdj.Visible = True
                tdMenPremAdjQuotedPremium.InnerText = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.MinimumPremiumAdjustment)
            End If
        End If
    End Sub

    Private Sub PopulateLocationInformation()
        Dim html As New StringBuilder()

        If Quote IsNot Nothing And Quote.Locations IsNot Nothing Then
            For Each LOC As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                ' LOCATIONS
                ' Section Header
                html.AppendLine("<span Class='qs_section_headers'>LOCATION INFORMATION</span>")
                html.AppendLine("<div Class='qs_Sub_Sections'>")

                'html.AppendLine("<div class=""qs_Sub_Sections"">")
                html.AppendLine("<table class=""qa_table_shades"">")
                html.AppendLine("<tr class=""qs_section_grid_headers ui-widget-header"">")

                ' Header Row
                WriteCell(html, "Address")
                WriteCell(html, "Protection Class", "", "qs_rightJustify qs_padRight")
                html.AppendLine("</tr>")

                ' Data Rows
                html.AppendLine("<tr class=""qs_basic_info_labels_cell"">")
                WriteCell(html, LOC.Address.DisplayAddress)
                WriteCell(html, LOC.ProtectionClass, "", "qs_rightJustify qs_padRight")
                html.AppendLine("</tr>")

                ' Close table
                html.AppendLine("</table>")

                PopulateOptionalLocationCoverages(html, LOC)
                PopulateBuildings(html, LOC, Quote)
                PopulatePropertyInTheOpen(html, LOC)

                ' Close div
                html.AppendLine("</div>")

                ' Write page
                tblLocations.Text = html.ToString()
            Next
        End If


        Exit Sub
    End Sub

    Private Sub PopulateOptionalLocationCoverages(ByRef html As StringBuilder, ByVal LOC As QuickQuote.CommonObjects.QuickQuoteLocation)
        Try
            html.AppendLine("<div Class='qs_Main_Sections'>")
            html.AppendLine("<span Class='qs_section_headers'>Optional Location Coverages</span>")
            html.AppendLine("<div class=""qs_Sub_Sections"">")
            html.AppendLine("<table class=""qa_table_shades"">")

            ' 1st orange column header
            html.AppendLine("<tr class=""qs_section_grid_headers ui-widget-header"">")
            WriteCell(html, "Coverage")
            WriteCell(html, "Premium", "", "qs_rightJustify qs_padRight")
            html.AppendLine("</tr>")

            ' DATA ROWS
            ' Equipment Breakdown  - Will Change
            If LOC.EquipmentBreakdownDeductible.Trim <> "" Then
                html.AppendLine("<tr class=""qs_basic_info_labels_cell qs_grid_4_columns"">")
                WriteCell(html, "Equipment Breakdown")
                WriteCell(html, LOC.EquipmentBreakdownDeductibleQuotedPremium, "", "qs_rightJustify qs_padRight")
                html.AppendLine("</tr>")
            End If

            ' Spacer line
            'html.AppendLine("<tr><td colspan=4>&nbsp;</td></tr>")

            ' Close the table and the divs
            html.AppendLine("</table>")
            html.AppendLine("</div>")
            html.AppendLine("</div>")

            Exit Sub
        Catch ex As Exception
            Exit Sub
        End Try
    End Sub

    Private Sub PopulateBuildings(ByRef html As StringBuilder, ByVal LOC As QuickQuote.CommonObjects.QuickQuoteLocation, ByVal quote As QuickQuote.CommonObjects.QuickQuoteObject)
        Try
            Dim ShowBlanketHelp = False
            ' WindHail is set at the location level and applies to all buildings
            Dim windHailPercent = QQHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteBuilding, QuickQuotePropertyName.WindHailDeductibleLimitId, LOC.WindHailDeductibleLimitId)

            For Each BLD As QuickQuote.CommonObjects.QuickQuoteBuilding In LOC.Buildings

                Dim has_CPR_building_buildingCov As Boolean = False 'added 11/27/2012
                If BLD.Limit <> "" OrElse (BLD.ValuationId <> "" AndAlso IsNumeric(BLD.ValuationId) = True) OrElse (BLD.ClassificationCode IsNot Nothing AndAlso BLD.ClassificationCode.ClassCode <> "") OrElse BLD.EarthquakeApplies = True OrElse (BLD.CauseOfLossTypeId <> "" AndAlso IsNumeric(BLD.CauseOfLossTypeId) = True) OrElse (BLD.CoinsuranceTypeId <> "" AndAlso IsNumeric(BLD.CoinsuranceTypeId) = True) OrElse (BLD.DeductibleId <> "" AndAlso IsNumeric(BLD.DeductibleId) = True) OrElse (BLD.RatingTypeId <> "" AndAlso IsNumeric(BLD.RatingTypeId) = True) OrElse (BLD.InflationGuardTypeId <> "" AndAlso IsNumeric(BLD.InflationGuardTypeId) = True) Then
                    has_CPR_building_buildingCov = True
                End If

                Dim has_CPR_building_busIncomeCov As Boolean = False 'added 11/27/2012
                If (BLD.BusinessIncomeCov_Limit <> "" AndAlso BLD.BusinessIncomeCov_Limit <> "0") OrElse (BLD.BusinessIncomeCov_CoinsuranceTypeId <> "" AndAlso QQHelper.IsPositiveIntegerString(BLD.BusinessIncomeCov_CoinsuranceTypeId) = True) OrElse (BLD.BusinessIncomeCov_MonthlyPeriodTypeId <> "" AndAlso QQHelper.IsPositiveIntegerString(BLD.BusinessIncomeCov_MonthlyPeriodTypeId) = True) OrElse (BLD.BusinessIncomeCov_BusinessIncomeTypeId <> "" AndAlso QQHelper.IsPositiveIntegerString(BLD.BusinessIncomeCov_BusinessIncomeTypeId) = True) OrElse (BLD.BusinessIncomeCov_RiskTypeId <> "" AndAlso QQHelper.IsPositiveIntegerString(BLD.BusinessIncomeCov_RiskTypeId) = True) OrElse (BLD.BusinessIncomeCov_RatingTypeId <> "" AndAlso QQHelper.IsPositiveIntegerString(BLD.BusinessIncomeCov_RatingTypeId) = True) OrElse (BLD.BusinessIncomeCov_CauseOfLossTypeId <> "" AndAlso QQHelper.IsPositiveIntegerString(BLD.BusinessIncomeCov_CauseOfLossTypeId) = True) OrElse (BLD.BusinessIncomeCov_ClassificationCode IsNot Nothing AndAlso BLD.BusinessIncomeCov_ClassificationCode.ClassCode <> "") OrElse BLD.BusinessIncomeCov_EarthquakeApplies = True Then
                    has_CPR_building_busIncomeCov = True
                End If

                Dim has_CPR_building_persPropCov As Boolean = False 'added 11/27/2012
                If BLD.PersPropCov_PersonalPropertyLimit <> "" OrElse (BLD.PersPropCov_PropertyTypeId <> "" AndAlso IsNumeric(BLD.PersPropCov_PropertyTypeId) = True) OrElse (BLD.PersPropCov_RiskTypeId <> "" AndAlso IsNumeric(BLD.PersPropCov_RiskTypeId) = True) OrElse BLD.PersPropCov_EarthquakeApplies = True OrElse (BLD.PersPropCov_RatingTypeId <> "" AndAlso IsNumeric(BLD.PersPropCov_RatingTypeId) = True) OrElse (BLD.PersPropCov_CauseOfLossTypeId <> "" AndAlso IsNumeric(BLD.PersPropCov_CauseOfLossTypeId) = True) OrElse (BLD.PersPropCov_DeductibleId <> "" AndAlso IsNumeric(BLD.PersPropCov_DeductibleId) = True) OrElse (BLD.PersPropCov_CoinsuranceTypeId <> "" AndAlso IsNumeric(BLD.PersPropCov_CoinsuranceTypeId) = True) OrElse (BLD.PersPropCov_ValuationId <> "" AndAlso IsNumeric(BLD.PersPropCov_ValuationId) = True) OrElse (BLD.PersPropCov_ClassificationCode IsNot Nothing AndAlso BLD.PersPropCov_ClassificationCode.ClassCode <> "") Then
                    has_CPR_building_persPropCov = True
                End If

                Dim has_CPR_building_persPropOfOthers As Boolean = False 'added 11/27/2012
                If BLD.PersPropOfOthers_PersonalPropertyLimit <> "" OrElse (BLD.PersPropOfOthers_RiskTypeId <> "" AndAlso IsNumeric(BLD.PersPropOfOthers_RiskTypeId) = True) OrElse BLD.PersPropOfOthers_EarthquakeApplies = True OrElse (BLD.PersPropOfOthers_RatingTypeId <> "" AndAlso IsNumeric(BLD.PersPropOfOthers_RatingTypeId) = True) OrElse (BLD.PersPropOfOthers_CauseOfLossTypeId <> "" AndAlso IsNumeric(BLD.PersPropOfOthers_CauseOfLossTypeId) = True) OrElse (BLD.PersPropOfOthers_DeductibleId <> "" AndAlso IsNumeric(BLD.PersPropOfOthers_DeductibleId) = True) OrElse (BLD.PersPropOfOthers_CoinsuranceTypeId <> "" AndAlso IsNumeric(BLD.PersPropOfOthers_CoinsuranceTypeId) = True) OrElse (BLD.PersPropOfOthers_ValuationId <> "" AndAlso IsNumeric(BLD.PersPropOfOthers_ValuationId) = True) OrElse (BLD.PersPropOfOthers_ClassificationCode IsNot Nothing AndAlso BLD.PersPropOfOthers_ClassificationCode.ClassCode <> "") Then
                    has_CPR_building_persPropOfOthers = True
                End If
                html.AppendLine("<div Class='qs_Main_Sections'>")
                html.AppendLine("<span Class='qs_section_headers'>Buildings</span>")
                html.AppendLine("<div class=""qs_Sub_Sections"">")
                html.AppendLine("<table class=""qa_table_shades"">")

                ' 1st orange column header
                html.AppendLine("<tr class=""qs_section_grid_headers ui-widget-header"">")
                WriteCell(html, indent & "Building Information", 3)
                WriteCell(html, "Premium", "", "qs_rightJustify qs_padRight")
                html.AppendLine("</tr>")

                ' 2nd orange column header
                html.AppendLine("<tr class=""qs_section_grid_headers ui-widget-header qs_grid_4_columns"">")
                WriteCell(html, "Description")
                WriteCell(html, "")
                WriteCell(html, "Class Code")
                WriteCell(html, "")
                html.AppendLine("</tr>")

                ' DATA ROWS
                ' Building Classifications
                html.AppendLine("<tr class=""qs_basic_info_labels_cell qs_grid_4_columns"">")
                WriteCell(html, BLD.ClassificationCode.ClassDescription, 2)
                WriteCell(html, BLD.ClassificationCode.ClassCode, 2)
                html.AppendLine("</tr>")

                ' Construction
                html.AppendLine("<tr class=""qs_basic_info_labels_cell"">")
                WriteCell(html, "Construction: " & BLD.Construction, 4)
                html.AppendLine("</tr>")

                '=================================================
                ' Spacer line
                html.AppendLine("<tr><td colspan=4>&nbsp;</td></tr>")

                If has_CPR_building_buildingCov Then
                    ' Building Limit
                    Dim blanketTxt As String = Nothing
                    Dim agreedTxt As String = Nothing
                    Dim asterisk As String = String.Empty
                    If BLD.IsBuildingValIncludedInBlanketRating Then
                        blanketTxt = "/Incl in Blanket"
                        asterisk = "*"
                        ShowBlanketHelp = True
                    End If
                    If BLD.IsAgreedValue Then
                        agreedTxt = "/Agreed Amount"
                    End If
                    html.AppendLine("<tr class=""qs_basic_info_labels_cell qs_grid_4_columns"">")
                    WriteCell(html, "Building Limit: " & BLD.Limit & " (" & BLD.Valuation & blanketTxt & agreedTxt & ")", 3)
                    WriteCell(html, asterisk + BLD.LimitQuotedPremium, "text-align:right;")
                    html.AppendLine("</tr>")

                    'Deductible / Wind/ Hail
                    html.AppendLine("<tr class=""qs_basic_info_labels_cell qs_grid_4_columns"">")
                    If LocationWindHailHelper.IsLocationWindHailAvailable(quote) AndAlso LOC.WindHailDeductibleLimitId <> BLD.OptionalWindstormOrHailDeductibleId Then
                        WriteCell(html, "Deductible: " & BLD.Deductible, 4)
                    Else
                        WriteCell(html, "Deductible: " & BLD.Deductible, 2)
                        WriteCell(html, "Wind/Hail Deductible: " & windHailPercent, 2)
                    End If
                    html.AppendLine("</tr>")

                    'Cause of Loss
                    html.AppendLine("<tr class=""qs_basic_info_labels_cell qs_grid_4_columns"">")
                    WriteCell(html, "Cause of Loss: " & BLD.CauseOfLossType, 4)
                    html.AppendLine("</tr>")

                    'Co-Insurance
                    html.AppendLine("<tr class=""qs_basic_info_labels_cell qs_grid_4_columns"">")
                    WriteCell(html, "Co-Insurance: " & BLD.CoinsuranceType, 4)
                    html.AppendLine("</tr>")
                End If

                'Earthquake
                If BLD.EarthquakeApplies Then
                    html.AppendLine("<tr class=""qs_basic_info_labels_cell qs_grid_4_columns"">")
                    WriteCell(html, "Earthquake", 3)
                    WriteCell(html, BLD.EarthquakeQuotedPremium, "text-align:right;")
                    html.AppendLine("</tr>")
                End If

                'Mine Subsidence - Added 11/8/18 for multi state MLW               
                ' Ohio logic 1/12/21 MGB
                If IsMultistateCapableEffectiveDate(Me.Quote.EffectiveDate) Then
                    If BLD.HasMineSubsidence Then
                        html.AppendLine("<tr class=""qs_basic_info_labels_cell qs_grid_4_columns"">")
                        If LOC.Address.QuickQuoteState = QuickQuoteState.Ohio Then
                            Dim lim As String = "N/A"
                            If BLD.Limit IsNot Nothing AndAlso IsNumeric(BLD.Limit) Then
                                If CDec(BLD.Limit) < 300000 Then
                                    lim = BLD.Limit
                                Else
                                    lim = "300,000"
                                End If
                            End If
                            WriteCell(html, "Mine Subsidence" & "  Limit: " & lim, 3)
                        Else
                            WriteCell(html, "Mine Subsidence", 3)
                        End If
                        WriteCell(html, BLD.MineSubsidenceQuotedPremium, "text-align:right;")
                        html.AppendLine("</tr>")
                    End If
                End If

                '=================================================
                ' Spacer line


                If has_CPR_building_busIncomeCov Then
                    html.AppendLine("<tr><td colspan=4>&nbsp;</td></tr>")
                    'Updated 12/20/18 for multi state MLW
                    'If quote.HasBusinessIncomeALS Then
                    If SubQuoteFirst.HasBusinessIncomeALS Then
                        'ALS
                        html.AppendLine("<tr class=""qs_basic_info_labels_cell  qs_grid_4_columns"">")
                        WriteCell(html, "Business Income ALS", 3)
                        WriteCell(html, BLD.BusinessIncomeCov_QuotedPremium, "text-align:right;")
                        html.AppendLine("</tr>")
                    Else
                        'Business Income Limit
                        html.AppendLine("<tr class=""qs_basic_info_labels_cell  qs_grid_4_columns"">")
                        WriteCell(html, "Business Income Limit: " & BLD.BusinessIncomeCov_Limit, 3)
                        WriteCell(html, BLD.CPR_BusinessIncomeCov_With_EQ_QuotedPremium, "text-align:right;")
                        html.AppendLine("</tr>")
                    End If

                    'Cause of Loss
                    html.AppendLine("<tr class=""qs_basic_info_labels_cell  qs_grid_4_columns"">")
                    WriteCell(html, "Cause of Loss: " & BLD.BusinessIncomeCov_CauseOfLossType, 4)
                    html.AppendLine("</tr>")


                    'Earthquake
                    If BLD.BusinessIncomeCov_EarthquakeApplies Then
                        html.AppendLine("<tr class=""qs_basic_info_labels_cell qs_grid_4_columns"">")
                        WriteCell(html, "Earthquake", 3)
                        WriteCell(html, BLD.BusinessIncomeCov_EarthquakeQuotedPremium, "text-align:right;")
                        html.AppendLine("</tr>")
                    End If
                End If

                '=================================================
                ' Spacer line


                If has_CPR_building_persPropCov Then
                    html.AppendLine("<tr><td colspan=4>&nbsp;</td></tr>")
                    'Personal Property Limit
                    Dim blanketTxt As String = Nothing
                    Dim agreedTxt As String = Nothing
                    Dim asterisk As String = String.Empty
                    If BLD.PersPropCov_IncludedInBlanketCoverage Then
                        blanketTxt = "/Incl in Blanket"
                        asterisk = "*"
                        ShowBlanketHelp = True
                    End If
                    If BLD.PersPropCov_IsAgreedValue Then
                        agreedTxt = "/Agreed Amount"
                    End If
                    html.AppendLine("<tr class=""qs_basic_info_labels_cell qs_grid_4_columns "">")
                    WriteCell(html, "Personal Property Limit: " & BLD.PersPropCov_PersonalPropertyLimit & " (" & BLD.PersPropCov_Valuation & blanketTxt & agreedTxt & ")", 3)
                    WriteCell(html, asterisk + BLD.PersPropCov_QuotedPremium, "text-align:right;")
                    html.AppendLine("</tr>")

                    'Deductible / Wind/ Hail
                    html.AppendLine("<tr class=""qs_basic_info_labels_cell qs_grid_4_columns"">")
                    If LocationWindHailHelper.IsLocationWindHailAvailable(quote) AndAlso LOC.WindHailDeductibleLimitId <> BLD.PersPropCov_OptionalWindstormOrHailDeductibleId Then
                        WriteCell(html, "Deductible: " & BLD.PersPropCov_Deductible, 4)
                    Else
                        WriteCell(html, "Deductible: " & BLD.PersPropCov_Deductible, 2)
                        WriteCell(html, "Wind/Hail Deductible: " & windHailPercent, 2)
                    End If
                    html.AppendLine("</tr>")

                    'Cause of Loss
                    html.AppendLine("<tr class=""qs_basic_info_labels_cell qs_grid_4_columns"">")
                    WriteCell(html, "Cause of Loss: " & BLD.PersPropCov_CauseOfLossType, 4)
                    html.AppendLine("</tr>")

                    'Co-Insurance
                    html.AppendLine("<tr class=""qs_basic_info_labels_cell qs_grid_4_columns"">")
                    WriteCell(html, "Co-Insurance: " & BLD.PersPropCov_CoinsuranceType, 4)
                    html.AppendLine("</tr>")
                End If

                'Earthquake
                If BLD.PersPropCov_EarthquakeApplies Then
                    html.AppendLine("<tr class=""qs_basic_info_labels_cell qs_grid_4_columns"">")
                    WriteCell(html, "Earthquake", 3)
                    WriteCell(html, BLD.PersPropCov_EarthquakeQuotedPremium, "text-align:right;")
                    html.AppendLine("</tr>")
                End If

                '=================================================
                ' Spacer line

                If has_CPR_building_persPropOfOthers Then
                    html.AppendLine("<tr><td colspan=4>&nbsp;</td></tr>")
                    'Personal Property of Others Limit
                    Dim blanketTxt As String = Nothing
                    Dim agreedTxt As String = Nothing
                    Dim asterisk As String = String.Empty
                    If BLD.PersPropOfOthers_IncludedInBlanketCoverage Then
                        blanketTxt = "/Incl in Blanket"
                        asterisk = "*"
                        ShowBlanketHelp = True
                    End If
                    html.AppendLine("<tr class=""qs_basic_info_labels_cell qs_grid_4_columns"">")
                    WriteCell(html, "Personal Property Of Others Limit: " & BLD.PersPropOfOthers_PersonalPropertyLimit & " (" & BLD.PersPropOfOthers_Valuation & blanketTxt & ")", 3)
                    WriteCell(html, asterisk + BLD.PersPropOfOthers_QuotedPremium, "text-align:right;")
                    html.AppendLine("</tr>")

                    'Deductible / Wind/ Hail
                    html.AppendLine("<tr class=""qs_basic_info_labels_cell qs_grid_4_columns"">")
                    If LocationWindHailHelper.IsLocationWindHailAvailable(quote) AndAlso LOC.WindHailDeductibleLimitId <> BLD.PersPropOfOthers_OptionalWindstormOrHailDeductibleId Then
                        WriteCell(html, "Deductible: " & BLD.PersPropOfOthers_Deductible, 4)
                    Else
                        WriteCell(html, "Deductible: " & BLD.PersPropOfOthers_Deductible, 2)
                        WriteCell(html, "Wind/Hail Deductible: " & windHailPercent, 2)
                    End If
                    html.AppendLine("</tr>")

                    'Cause of Loss
                    html.AppendLine("<tr class=""qs_basic_info_labels_cell qs_grid_4_columns"">")
                    WriteCell(html, "Cause of Loss: " & BLD.PersPropOfOthers_CauseOfLossType, 4)
                    html.AppendLine("</tr>")

                    'Co-Insurance
                    html.AppendLine("<tr class=""qs_basic_info_labels_cell qs_grid_4_columns"">")
                    WriteCell(html, "Co-Insurance: " & BLD.PersPropOfOthers_CoinsuranceType, 4)
                    html.AppendLine("</tr>")
                End If

                'Earthquake
                If BLD.PersPropOfOthers_EarthquakeApplies Then
                    html.AppendLine("<tr class=""qs_basic_info_labels_cell qs_grid_4_columns"">")
                    WriteCell(html, "Earthquake", 3)
                    WriteCell(html, BLD.PersPropOfOthers_EarthquakeQuotedPremium, "text-align:right;")
                    html.AppendLine("</tr>")
                End If

                'Blanket Help Text
                If ShowBlanketHelp = True Then
                    ' Spacer line
                    html.AppendLine("<tr><td colspan=4>&nbsp;</td></tr>")
                    html.AppendLine("<tr class=""qs_basic_info_labels_cell qs_grid_4_columns"">")
                    WriteCell(html, "*Individual premiums are included in the blanket total. ", 4)
                    html.AppendLine("</tr>")
                End If

                ' Close the table and the divs
                html.AppendLine("</table>")
                html.AppendLine("</div>")
                html.AppendLine("</div>")
            Next
            Exit Sub
        Catch ex As Exception
            Exit Sub
        End Try
    End Sub
    Private Sub PopulatePropertyInTheOpen(ByRef html As StringBuilder, ByVal LOC As QuickQuote.CommonObjects.QuickQuoteLocation)
        Try
            ' 4 Columns:
            ' Header/Data Line 1:   |Building Information                     |Premium      |
            ' Header/Data Line 2:   |Description           |Classcode                       |

            For Each pito As QuickQuote.CommonObjects.QuickQuotePropertyInTheOpenRecord In LOC.PropertyInTheOpenRecords
                html.AppendLine("<div Class='qs_Main_Sections'>")
                html.AppendLine("<span Class='qs_section_headers'>Property In The Open</span>")

                html.AppendLine("<div class=""qs_Sub_Sections"">")
                html.AppendLine("<table class=""qa_table_shades"">")

                ' 1st orange column header
                html.AppendLine("<tr class=""qs_section_grid_headers ui-widget-header qs_grid_4_columns"">")
                WriteCell(html, "Description", 2)
                'WriteCell(html, "")
                WriteCell(html, "Class Code")
                WriteCell(html, "Premium", "", "qs_rightJustify qs_padRight")
                html.AppendLine("</tr>")

                ' DATA ROWS
                ' Description
                If pito.Description IsNot Nothing Then
                    ' Line 1 of the class code - program, class code
                    html.AppendLine("<tr class=""qs_basic_info_labels_cell qs_grid_4_columns"">")
                    WriteCell(html, pito.Description, 2)
                    WriteCell(html, pito.SpecialClassCode & "-" & pito.SpecialClassCodeType, 2)
                    html.AppendLine("</tr>")
                End If

                ' Limit
                Dim blanketTxt As String = Nothing
                Dim agreedTxt As String = Nothing
                Dim asterisk As String = String.Empty
                If pito.IncludedInBlanketCoverage Then
                    blanketTxt = "/Incl in Blanket"
                    asterisk = "*"
                End If
                If pito.IsAgreedValue Then
                    agreedTxt = "/Agreed Amount"
                End If
                html.AppendLine("<tr class=""qs_basic_info_labels_cell qs_grid_4_columns"">")
                WriteCell(html, "Building Limit: " & pito.Limit & " (" & pito.Valuation & blanketTxt & agreedTxt & ")", 3)
                WriteCell(html, asterisk + pito.QuotedPremium, "text-align:right;")
                html.AppendLine("</tr>")

                ' Deductible
                html.AppendLine("<tr class=""qs_basic_info_labels_cell qs_grid_4_columns"">")
                If LocationWindHailHelper.IsLocationWindHailAvailable(Quote) AndAlso LOC.WindHailDeductibleLimitId = pito.OptionalWindstormOrHailDeductibleId Then
                    Dim windHailPercent = QQHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteBuilding, QuickQuotePropertyName.WindHailDeductibleLimitId, LOC.WindHailDeductibleLimitId)
                    WriteCell(html, "Deductible: " & pito.Deductible, 2)
                    WriteCell(html, "Wind/Hail Deductible: " & windHailPercent, 2)
                Else
                    WriteCell(html, "Deductible: " & pito.Deductible, 4)
                End If
                html.AppendLine("</tr>")

                'Cause of Loss
                If String.IsNullOrEmpty(pito.CauseOfLossType.Trim) = False Then
                    html.AppendLine("<tr class=""qs_basic_info_labels_cell qs_grid_4_columns"">")
                    WriteCell(html, "Cause of Loss: " & pito.CauseOfLossType, 4)
                    html.AppendLine("</tr>")
                End If

                'Co-Insurance
                If String.IsNullOrEmpty(pito.CoinsuranceType.Trim) = False Then
                    html.AppendLine("<tr class=""qs_basic_info_labels_cell qs_grid_4_columns"">")
                    WriteCell(html, "Co-Insurance: " & pito.CoinsuranceType, 4)
                    html.AppendLine("</tr>")
                End If

                'Earthquake
                If pito.EarthquakeApplies Then
                    html.AppendLine("<tr class=""qs_basic_info_labels_cell qs_grid_4_columns"">")
                    WriteCell(html, "Earthquake", 3)
                    WriteCell(html, pito.EarthquakeQuotedPremium, "text-align:right;")
                    html.AppendLine("</tr>")
                End If

                ' Spacer line
                html.AppendLine("<tr><td colspan=4>&nbsp;</td></tr>")

                ' Close the table and the divs
                html.AppendLine("</table>")
                html.AppendLine("</div>")
                html.AppendLine("</div>")

            Next

            Exit Sub
        Catch ex As Exception
            Exit Sub
        End Try
    End Sub

    Private Sub PopulateLossHistory()
        Dim indent As String = "&nbsp;&nbsp;"
        Dim html As New StringBuilder()

        ' Start Loss History Section
        html.AppendLine("<div Class='qs_Main_Sections'>")
        'html.AppendLine("<span Class='qs_section_headers'>Buildings</span>")

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
        'PopulateLossHistoryDetail(html, Quote.LossHistoryRecords)
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