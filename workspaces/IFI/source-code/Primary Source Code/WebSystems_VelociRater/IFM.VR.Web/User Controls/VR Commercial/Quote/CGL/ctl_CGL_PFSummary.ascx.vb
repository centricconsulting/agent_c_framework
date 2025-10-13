Imports IFM.PrimativeExtensions
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.IO
Imports System.Globalization
Imports IFM.VR.Common.Helpers.MultiState.General
Imports QuickQuote.CommonObjects

Public Class ctl_CGL_PFSummary
    Inherits VRControlBase
    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion("divCGLQuoteSummary", HiddenField1, "0")
        'Me.VRScript.StopEventPropagation(Me.lnkPrint.ClientID, False)
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

            PopulatePolicyLevelCoverages()
            ctlPolicyDiscounts.Populate()
            PopulateLocationInformation()
            PopulateClassifications()

            If IsOnAppPage AndAlso Me.Quote IsNot Nothing AndAlso GoverningStateQuote() IsNot Nothing AndAlso Me.GoverningStateQuote.LossHistoryRecords IsNot Nothing AndAlso Me.GoverningStateQuote.LossHistoryRecords.Count > 0 Then
                PopulateLossHistory()
            End If


            'Me.ctlQuoteSummaryActions.Populate()
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

    Private Sub PopulatePolicyLevelCoverages()
        Dim IndianaQuote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing
        Dim IllinoisQuote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing
        Dim OhioQuote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing

        If Quote IsNot Nothing Then
            If SubQuoteFirst IsNot Nothing Then
                'Intialize
                rowBusMasterEnhancement.Visible = False
                rowGlPlusEnhancement.Visible = False
                rowBlanketWaiverOfSubro.Visible = False
                pnlGenLibDeductible.Visible = False
                rowAdditionalInsureds.Visible = False
                rowEmployeeBenefitsLiability.Visible = False
                rowEPLI.Visible = False
                rowHiredAuto.Visible = False
                rowNonOwnedAuto.Visible = False
                pnlLiquorLiability_IN.Visible = False
                pnlLiquorLiability_IL.Visible = False
                pnlProfessionalLiability.Visible = False
                rowProfessionalLiability_CP.Visible = False
                rowProfessionalLiability_FP.Visible = False
                rowProfessionalLiability_PP.Visible = False
                rowPremisesAdjustment.Visible = False
                rowProductsAdjustment.Visible = False
                pnlContractorsHomeRepairAndRemodeling.Visible = False
                rowStopGap.Visible = False

                'Static Elements
                tdGenAggLimit.InnerText = SubQuoteFirst.GeneralAggregateLimit
                'tdGenAggPremium.InnerText = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.GeneralAggregateQuotedPremium)
                ProdCompOpsAggLimit.InnerText = SubQuoteFirst.ProductsCompletedOperationsAggregateLimit
                'ProdCompOpsAggLimitQuotedPremium.InnerText = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.ProductsCompletedOperationsAggregateQuotedPremium)
                tdPersAndAdInjuryLimit.InnerText = SubQuoteFirst.PersonalAndAdvertisingInjuryLimit
                'tdPersAndAdInjuryLimitQuotedPremium.InnerText = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.PersonalAndAdvertisingInjuryQuotedPremium)
                tdOccLiabilityLimit.InnerText = SubQuoteFirst.OccurrenceLiabilityLimit
                'tdOccLiabilityLimitQuotedPremium.InnerText = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.OccurrencyLiabilityQuotedPremium)
                tdDamageToPremsRentedLimit.InnerText = SubQuoteFirst.DamageToPremisesRentedLimit
                'tdDamageToPremsRentedLimitQuotedPremium.InnerText = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.DamageToPremisesRentedQuotedPremium)
                tdMedicalExpensesLimit.InnerText = SubQuoteFirst.MedicalExpensesLimit
                'tdMedicalExpensesLimitQuotedPremium.InnerText = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.MedicalExpensesQuotedPremium)

                'Dynamic Elements
                If SubQuoteFirst.HasBusinessMasterEnhancement = True Then
                    rowBusMasterEnhancement.Visible = True
                    tdBusMasterEnhancementQuotedPremium.InnerText = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.BusinessMasterEnhancementQuotedPremium)
                End If
                If SubQuoteFirst.Has_PackageGL_PlusEnhancementEndorsement Then
                    rowGlPlusEnhancement.Visible = True
                    GlPlusEnhancementQuotedPremium.InnerText = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.PackageGL_PlusEnhancementEndorsementQuotedPremium)
                End If
                If SubQuoteFirst.BlanketWaiverOfSubrogation IsNot Nothing AndAlso SubQuoteFirst.BlanketWaiverOfSubrogation <> String.Empty Then
                    'Added 2 / 11 / 15 MGB Bug 4040 - GL CGL1002/CGL1004 coverages
                    Select Case SubQuoteFirst.BlanketWaiverOfSubrogation
                        Case "1"  ' CGL 1004
                            rowBlanketWaiverOfSubro.Visible = True
                            tdBlanketWaiverOfSubroText.InnerText = "Blanket Waiver of Subro"
                            tdBlanketWaiverOfSubroQuotedPremium.InnerText = "$100.00"
                            Exit Select
                        Case "2"  ' CGL 1002
                            rowBlanketWaiverOfSubro.Visible = True
                            tdBlanketWaiverOfSubroText.InnerText = "Blanket Waiver of Subro w/Completed Ops "
                            tdBlanketWaiverOfSubroQuotedPremium.InnerText = "$300.00"
                            Exit Select
                        Case Else
                            rowBlanketWaiverOfSubro.Visible = False
                            Exit Select
                    End Select
                End If
                If SubQuoteFirst.Has_GL_PremisesAndProducts Then
                    pnlGenLibDeductible.Visible = True
                    tdGenLibDeductibleQuotedPremium.InnerText = ""
                    GenLibDeductibleType.InnerText = SubQuoteFirst.GL_PremisesAndProducts_DeductibleCategoryType
                    GenLibDeductibleAmount.InnerText = SubQuoteFirst.GL_PremisesAndProducts_Deductible
                    GenLibDeductibleBasis.InnerText = SubQuoteFirst.GL_PremisesAndProducts_DeductiblePerType
                End If
                If SubQuoteFirst.AdditionalInsuredsCount > 0 Then
                    rowAdditionalInsureds.Visible = True
                    tdAdditionalInsuredsPremium.InnerText = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.AdditionalInsuredsQuotedPremium)
                End If
                If IsNumeric(SubQuoteFirst.EmployeeBenefitsLiabilityText) = True Then
                    rowEmployeeBenefitsLiability.Visible = True
                    EmpBLibNum.InnerText = SubQuoteFirst.EmployeeBenefitsLiabilityText
                    EmpBLibNum.InnerText &= " Employee" & If(CInt(SubQuoteFirst.EmployeeBenefitsLiabilityText) = 1, "", "s")
                    tdEmployeeBenefitsLiabilityQuotedPremium.InnerText = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.EmployeeBenefitsLiabilityQuotedPremium)
                End If
                ' MGB 11/12/2020 Bug 43384 correction
                If SubQuoteFirst.HasEPLI AndAlso SubQuoteFirst.EPLICoverageTypeID = "22" Then
                    rowEPLI.Visible = True
                    tdEPLIQuotedPremium.InnerText = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.EPLIPremium)
                End If
                ' OH Stop Gap
                '''TODO: evaluate how we should remove  this; create task
                If IFM.VR.Common.Helpers.MultiState.General.IsOhioEffective(Quote) Then
                    Dim gsQuote As QuickQuoteObject = Me.GoverningStateQuote()
                    If Not String.IsNullOrWhiteSpace(gsQuote.StopGapLimitId) AndAlso gsQuote.StopGapLimitId.IsNumeric Then
                        rowStopGap.Visible = True
                        Dim prem As String = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.StopGapQuotedPremium)
                        If prem.IsNumeric Then prem = Format(CDec(prem), "c")
                        tdStopGapQuotedPremium.InnerHtml = prem
                    End If
                End If
                If SubQuoteFirst.HasHiredAuto = True Then
                    rowHiredAuto.Visible = True
                    tdHiredAutoQuotedPremium.InnerText = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.HiredAutoQuotedPremium)
                End If
                If SubQuoteFirst.HasNonOwnedAuto Then
                    rowNonOwnedAuto.Visible = True
                    tdNonOwnedAutoQuotedPremium.InnerText = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.NonOwnedAutoQuotedPremium)
                End If
                ' Indiana Liquor Liability
                If SubQuotesContainsState("IN") Then
                    IndianaQuote = SubQuoteForState("IN")
                    If IndianaQuote.LiquorLiabilityOccurrenceLimitId <> "" OrElse IndianaQuote.LiquorLiabilityClassificationId <> "" Then
                        pnlLiquorLiability_IN.Visible = True
                        'tdLiquorLiabilityQuotedPremium_IN.InnerText = QQHelper.GetSumForPropertyValues(SubQuotes, Function() IndianaQuote.LiquorLiabilityQuotedPremium)
                        'updated 11/19/2018 to be state-specific; previous code would have always showed the prem for all states
                        tdLiquorLiabilityQuotedPremium_IN.InnerText = IndianaQuote.LiquorLiabilityQuotedPremium
                        LiquorLiabilityOccLimit_IN.InnerText = IndianaQuote.LiquorLiabilityOccurrenceLimit
                        LiquorLiabilityClass_IN.InnerText = IndianaQuote.LiquorLiabilityClassification
                        LiquorLiabilityLSales_IN.InnerText = QQHelper.SetLiquorSales(IndianaQuote)
                    End If
                End If
                ' Ohio Liquor Liability
                If SubQuotesContainsState("OH") Then
                    OhioQuote = SubQuoteForState("OH")
                    If OhioQuote.LiquorLiabilityOccurrenceLimitId <> "" OrElse OhioQuote.LiquorLiabilityClassificationId <> "" Then
                        pnlLiquorLiability_OH.Visible = True
                        tdLiquorLiabilityQuotedPremium_OH.InnerText = OhioQuote.LiquorLiabilityQuotedPremium
                        LiquorLiabilityOccLimit_OH.InnerText = OhioQuote.LiquorLiabilityOccurrenceLimit
                        LiquorLiabilityClass_OH.InnerText = OhioQuote.LiquorLiabilityClassification
                        LiquorLiabilitySales_OH.InnerText = QQHelper.SetLiquorSales(OhioQuote)
                    End If
                End If
                ' Illinois Liquor Liability
                If IsMultistateCapableEffectiveDate(Quote.EffectiveDate) AndAlso SubQuotesContainsState("IL") Then
                    IllinoisQuote = SubQuoteForState("IL")
                    If IllinoisQuote.LiquorLiabilityOccurrenceLimitId <> "" OrElse IllinoisQuote.LiquorLiabilityClassificationId <> "" Then
                        pnlLiquorLiability_IL.Visible = True
                        'tdLiquorLiabilityQuotedPremium_IL.InnerText = QQHelper.GetSumForPropertyValues(SubQuotes, Function() IllinoisQuote.LiquorLiabilityQuotedPremium)
                        'updated 11/19/2018 to be state-specific; previous code would have always showed the prem for all states
                        tdLiquorLiabilityQuotedPremium_IL.InnerText = IllinoisQuote.LiquorLiabilityQuotedPremium
                        If CDate(IllinoisQuote.EffectiveDate) < CDate("4/1/2020") Then
                            Select Case IllinoisQuote.LiquorLiabilityOccurrenceLimitId
                                Case "396"
                                    LiquorLiabilityOccLimit_IL.InnerText = "69/69/85/600"
                                    Exit Select
                                Case "397"
                                    LiquorLiabilityOccLimit_IL.InnerText = "69/69/85/1000"
                                    Exit Select
                                Case "399"
                                    LiquorLiabilityOccLimit_IL.InnerText = "69/69/85/2000"
                                    Exit Select
                            End Select
                        Else
                            LiquorLiabilityOccLimit_IL.InnerText = "REFER TO ILLINOIS STATUTORY LIMITS"
                        End If
                        LiquorLiabilityClass_IL.InnerText = IllinoisQuote.LiquorLiabilityClassification
                        LiquorLiabilityLSales_IL.InnerText = QQHelper.SetLiquorSales(IllinoisQuote)
                    End If
                End If
                ' IL Contractors Home Repair and Remodeling  (new coverage for multistate)
                If IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then
                    Dim ILQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois)
                    'If ILQuote IsNot Nothing AndAlso (Not IsNullEmptyorWhitespace(ILQuote.IllinoisContractorsHomeRepairAndRemodelingQuotedPremium)) Then
                    'updated 11/19/2018 to make sure the coverage is there; above code would always evaluate if IL is there since the premium would be zero, which is not null or empty space
                    If ILQuote IsNot Nothing AndAlso ILQuote.HasIllinoisContractorsHomeRepairAndRemodeling Then
                        pnlContractorsHomeRepairAndRemodeling.Visible = True
                        tdContractorsHomeRepairAndRemodelingPremium.InnerText = ILQuote.IllinoisContractorsHomeRepairAndRemodelingQuotedPremium
                    End If
                End If
                If IFM.Common.InputValidation.CommonValidations.IsPositiveNumber(SubQuoteFirst.MinimumPremiumAdjustment) = True Then
                    rowMinimumPremiumAdjustment.Visible = True
                    'The below (lblDec_Minimum_Prem) may be removed if they decide it isn't necessary - CAH 11/09/17
                    'Me.lblDec_Minimum_Prem.Text = Quote.MinimumQuotedPremium
                    tdMinimum_Prem.InnerText = SubQuoteFirst.MinimumPremiumAdjustment
                End If
                If IFM.Common.InputValidation.CommonValidations.IsPositiveNumber(SubQuoteFirst.GL_PremisesMinimumPremiumAdjustment) = True Then
                    rowPremisesAdjustment.Visible = True
                    'Me.lblDec_Premises_Minimum_Prem.Text = Quote.GL_PremisesMinimumQuotedPremium
                    tdPremises_MinPremAdj_Prem.InnerText = SubQuoteFirst.GL_PremisesMinimumPremiumAdjustment
                End If
                If IFM.Common.InputValidation.CommonValidations.IsPositiveNumber(SubQuoteFirst.GL_ProductsMinimumPremiumAdjustment) = True Then
                    rowProductsAdjustment.Visible = True
                    'Me.lblDec_Products_Minimum_Prem.Text = Quote.GL_ProductsMinimumQuotedPremium
                    tdProducts_MinPremAdj_Prem.InnerText = SubQuoteFirst.GL_ProductsMinimumPremiumAdjustment
                End If
                'If Quote.ProfessionalLiabilityCemetaryNumberOfBurials <> "" OrElse Quote.ProfessionalLiabilityFuneralDirectorsNumberOfBodies <> "" OrElse Quote.ProfessionalLiabilityPastoralNumberOfClergy <> "" Then
                '    pnlProfessionalLiability.Visible = True
                '    If Quote.ProfessionalLiabilityCemetaryNumberOfBurials <> "" Then
                '        rowProfessionalLiability_CP.Visible = True
                '        PL_CP_Num.InnerText = Quote.ProfessionalLiabilityCemetaryNumberOfBurials
                '        tdPL_CP_Premium.InnerText = Quote.ProfessionalLiabilityCemetaryQuotedPremium
                '    End If
                '    If Quote.ProfessionalLiabilityFuneralDirectorsNumberOfBodies <> "" Then
                '        rowProfessionalLiability_FP.Visible = True
                '        PL_FP_Num.InnerText = Quote.ProfessionalLiabilityFuneralDirectorsNumberOfBodies
                '        tdPL_FP_Premium.InnerText = Quote.FuneralDirectorsProfessionalLiabilityQuotedPremium
                '    End If
                '    If Quote.ProfessionalLiabilityPastoralNumberOfClergy <> "" Then
                '        rowProfessionalLiability_PP.Visible = True
                '        PL_PP_Num.InnerText = Quote.ProfessionalLiabilityPastoralNumberOfClergy
                '        tdPL_PP_Premium.InnerText = Quote.ProfessionalLiabilityPastoralQuotedPremium
                '    End If

                'End If
            End If
        End If
    End Sub

    Private Sub PopulateLocationInformation()
        Dim html As New StringBuilder()

        html.AppendLine("<table class=""qa_table_shades"">")
        html.AppendLine("<tr class=""qs_section_grid_headers ui-widget-header"">")

        ' Header Row
        WriteCell(html, "Address")
        WriteCell(html, "")
        html.AppendLine("</tr>")

        If Quote IsNot Nothing AndAlso Quote.Locations IsNot Nothing Then
            For Each LOC As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                ' Data Rows
                html.AppendLine("<tr class=""qs_basic_info_labels_cell"">")
                WriteCell(html, LOC.Address.DisplayAddress)
                WriteCell(html, "")
                html.AppendLine("</tr>")
            Next
        End If

        ' Close table
        html.AppendLine("</table>")

        'PopulateOptionalLocationCoverages(html, LOC)
        'PopulateBuildings(html, LOC)

        ' Write page
        tblLocations.Text = html.ToString()

        Exit Sub
    End Sub

    Private Sub PopulateClassifications()
        Dim html As New StringBuilder()
        Dim classCodes = IFM.VR.Common.Helpers.CGL.ClassCodeHelper.GetAllPolicyAndLocationClassCodes(Me.Quote)
        If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Count > 0 Then
            For Each LOC As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                If LOC.GLClassifications IsNot Nothing AndAlso LOC.GLClassifications.Count > 0 Then
                    PopulateLocationCodes(html)
                    Exit For
                End If
            Next
        End If
        If Me.Quote IsNot Nothing AndAlso Me.Quote.GLClassifications IsNot Nothing AndAlso Me.Quote.GLClassifications.Count > 0 Then
            PopulatePolicyCodes(html)
        End If
        'PopulateLocationCodes(html)
        'PopulatePolicyCodes(html)
        tblClassifications.Text = html.ToString()
        Exit Sub
    End Sub

    Private Sub PopulateLocationCodes(ByRef html As StringBuilder)
        Dim locCounter = 0

        ' Start Table
        html.AppendLine("<table class=""qa_table_shades"">")

        ' Header Row 1
        html.AppendLine("<tr class=""qs_section_grid_headers ui-widget-header"">")
        WriteCell(html, "Location GL Class Code")
        WriteCell(html, "")
        html.AppendLine("</tr>")

        ' Header Row 2
        html.AppendLine("<tr class=""qs_section_grid_headers ui-widget-header"">")
        WriteCell(html, "Description")
        WriteCell(html, "ClassCode", "", "qs_rightJustify qs_padRight")
        html.AppendLine("</tr>")

        For Each LOC As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
            Dim ccCounter = 0
            ' Data Rows
            If locCounter > 0 Then
                'give me a blank row between locations
                html.AppendLine("<tr class=""qs_basic_info_labels_cell""><td>&nbsp;</td><td></td></tr>")
            End If

            ' Address Row
            html.AppendLine("<tr class=""qs_basic_info_labels_cell"">")
            WriteCell(html, LOC.Address.DisplayAddress)
            WriteCell(html, "")
            html.AppendLine("</tr>")

            ' Classification Rows
            If LOC.GLClassifications IsNot Nothing Then

                For Each cc As QuickQuote.CommonObjects.QuickQuoteGLClassification In LOC.GLClassifications

                    If ccCounter > 0 Then
                        'give me a blank row between classifications
                        html.AppendLine("<tr class=""qs_basic_info_labels_cell""><td>&nbsp;</td><td></td></tr>")
                    End If

                    html.AppendLine("<tr class=""qs_basic_info_labels_cell qs_indent"">")
                    WriteCell(html, cc.ClassDescription)
                    WriteCell(html, cc.ClassCode, "", "qs_rightJustify qs_padRight")
                    html.AppendLine("</tr>")

                    html.AppendLine("<tr class=""qs_basic_info_labels_cell qs_indent"">")
                    WriteCell(html, "Premium Exposure: " + cc.PremiumExposure)
                    WriteCell(html, "")
                    html.AppendLine("</tr>")

                    html.AppendLine("<tr class=""qs_basic_info_labels_cell qs_indent"">")
                    WriteCell(html, "Premium Base: " + cc.PremiumBase)
                    WriteCell(html, "")
                    html.AppendLine("</tr>")

                    html.AppendLine("<tr class=""qs_basic_info_labels_cell qs_indent"">")
                    WriteCell(html, "Premises Premium: " + cc.PremisesQuotedPremium)
                    WriteCell(html, "")
                    html.AppendLine("</tr>")

                    html.AppendLine("<tr class=""qs_basic_info_labels_cell qs_indent"">")
                    WriteCell(html, "Products Premium: " + cc.ProductsQuotedPremium)
                    WriteCell(html, "")
                    html.AppendLine("</tr>")

                    ccCounter = ccCounter + 1
                Next
            End If

            locCounter = locCounter + 1
        Next

        ' Close table
        html.AppendLine("</table>")
    End Sub

    Private Sub PopulatePolicyCodes(ByRef html As StringBuilder)
        ' Start Table
        html.AppendLine("<table class=""qa_table_shades"" style=""margin-top: 10px"">")

        ' Header Row 1
        html.AppendLine("<tr class=""qs_section_grid_headers ui-widget-header"">")
        WriteCell(html, "Policy GL Class Code")
        WriteCell(html, "")
        html.AppendLine("</tr>")

        ' Header Row 2
        html.AppendLine("<tr class=""qs_section_grid_headers ui-widget-header"">")
        WriteCell(html, "Description")
        WriteCell(html, "ClassCode", "", "qs_rightJustify qs_padRight")
        html.AppendLine("</tr>")

        Dim ccCounter = 0
        ' Data Rows

        ' Classification Rows
        For Each cc As QuickQuote.CommonObjects.QuickQuoteGLClassification In Quote.GLClassifications

            If ccCounter > 0 Then
                'give me a blank row between classifications
                html.AppendLine("<tr class=""qs_basic_info_labels_cell""><td>&nbsp;</td><td></td></tr>")
            End If

            html.AppendLine("<tr class=""qs_basic_info_labels_cell"">")
            WriteCell(html, cc.ClassDescription)
            WriteCell(html, cc.ClassCode, "", "qs_rightJustify qs_padRight")
            html.AppendLine("</tr>")

            html.AppendLine("<tr class=""qs_basic_info_labels_cell"">")
            WriteCell(html, "Premium Exposure: " + cc.PremiumExposure)
            WriteCell(html, "")
            html.AppendLine("</tr>")

            html.AppendLine("<tr class=""qs_basic_info_labels_cell"">")
            WriteCell(html, "Premium Base: " + cc.PremiumBase)
            WriteCell(html, "")
            html.AppendLine("</tr>")

            html.AppendLine("<tr class=""qs_basic_info_labels_cell"">")
            WriteCell(html, "Premises Premium: " + cc.PremisesQuotedPremium)
            WriteCell(html, "")
            html.AppendLine("</tr>")

            html.AppendLine("<tr class=""qs_basic_info_labels_cell"">")
            WriteCell(html, "Products Premium: " + cc.ProductsQuotedPremium)
            WriteCell(html, "")
            html.AppendLine("</tr>")

            ccCounter = ccCounter + 1
        Next

        ' Close table
        html.AppendLine("</table>")
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
            WriteCell(html, FormatCurrency(If(loss IsNot Nothing AndAlso String.IsNullOrWhiteSpace(loss.Amount) = False, loss.Amount, "0")), 2)
            html.AppendLine("</tr>")
        Next

    End Sub

    'Private Sub lnkPrint_Click(sender As Object, e As EventArgs) Handles lnkPrint.Click
    '    Try
    '        Fire_BroadcastWorkflowChangeRequestEvent(Common.Workflow.Workflow.WorkflowSection.drivers, "")
    '    Catch ex As Exception

    '    End Try
    'End Sub

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
                                Dim pdfBytes As Byte() = New Byte(CInt(fs_pdf.Length - 1)) {}
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