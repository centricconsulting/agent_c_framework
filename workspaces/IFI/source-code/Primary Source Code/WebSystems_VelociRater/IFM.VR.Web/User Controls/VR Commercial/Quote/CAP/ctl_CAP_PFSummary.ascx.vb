Imports IFM.PrimativeExtensions
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.IO
Imports System.Globalization
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.VR.Common.Helpers.CAP

Public Class ctl_CAP_PFSummary
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
        'If AppSettings("TestOrProd") IsNot Nothing AndAlso AppSettings("TestOrProd").ToUpper() = "TEST" Then
        '    lblMsg.Text = RoutineName + ": " & ex.Message
        'End If
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion("divCAPQuoteSummary", HiddenField1, "0")
        'Me.VRScript.StopEventPropagation(Me.lnkPrint.ClientID, False)
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub



    Public Overrides Sub Populate()
        Try
            If Me.Quote IsNot Nothing Then
                '-----------------------------------------------
                ' Temp Code until Rate
                '-----------------------------------------------
                'Quote.GarageKeepersTotalPremium = "$315.00"
                'Quote.GarageKeepersOtherThanCollisionManualLimitAmount = "$125,000"
                'Quote.GarageKeepersCollisionManualLimitAmount = "$75,000"
                'Quote.BusinessMasterEnhancementQuotedPremium = "$145.00"
                'Quote.WaiverOfSubrogationPremium = "$95.00"
                'Quote.NonOwnedAutoQuotedPremium = "$275.00"
                'Quote.HiredAutoQuotedPremium = "$150.00"
                'Quote.FarmPollutionLiabilityQuotedPremium = "$75.00"

                'Quote.HasGarageKeepersOtherThanCollision = True
                'Quote.HasGarageKeepersCollision = True
                'Quote.HasBusinessMasterEnhancement = True
                'Quote.HasWaiverOfSubrogation = True
                'Quote.HasNonOwnedAuto = True
                'Quote.HasHiredAuto = True
                'Quote.HasFarmPollutionLiability = True


                '-----------------------------------------------
                'If IsOnAppPage Then
                '    Me.lblMainAccord.Text = String.Format("App Summary - Effective Date: {0} - 12 month", Me.Quote.EffectiveDate)
                'Else
                '    Me.lblMainAccord.Text = String.Format("Quote Summary - Effective Date: {0} - 12 month", Me.Quote.EffectiveDate)
                'End If
                'updated 5/10/2019; logic taken from updates for PPA
                Select Case Me.Quote.QuoteTransactionType
                    Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote
                        'Removed 01/13/2021 for CAP Endorsements Task 52976 MLW
                        'Me.lblMainAccord.Text = If(Me.Quote.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote, "Change", "Image") & " Summary - Updated " & QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.PolicyTermId, Me.Quote.PolicyTermId) & " Premium"
                        'Me.ImageDateAndPremChangeLine.Visible = True
                        'If QQHelper.IsDateString(Me.Quote.TransactionEffectiveDate) = True Then
                        '    Me.lblTranEffDate.Text = "Effective Date: " & Me.Quote.TransactionEffectiveDate 'note: should already be in shortdate format
                        'Else
                        '    Me.lblTranEffDate.Text = ""
                        'End If
                        'If QQHelper.IsNumericString(Me.Quote.ChangeInFullTermPremium) = True Then 'note: was originally looking for positive decimal, but the change in prem could be zero or negative
                        '    Me.lblAnnualPremChg.Text = "Annual Premium Change: " & Me.Quote.ChangeInFullTermPremium 'note: should already be in money format
                        'Else
                        '    Me.lblAnnualPremChg.Text = ""
                        'End If
                        'Added 01/13/2021 for CAP Endorsements Task 52976 MLW
                        Me.ctlEndorsementOrChangeHeader.Visible = True
                        Me.ctlEndorsementOrChangeHeader.Quote = Quote
                        Me.EndorsementPrintSection.Visible = True
                        Me.quoteSummaryHeader.Visible = False
                        Me.quoteSummarySection.Visible = False
                    Case Else
                        'Removed 01/13/2021 for CAP Endorsements Task 52976 MLW
                        'Me.lblMainAccord.Text = String.Format("{2} - Effective Date: {0} - {1}", Me.Quote.EffectiveDate, QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.PolicyTermId, Me.Quote.PolicyTermId), If(IsOnAppPage, "App", "Quote") & " Summary")
                        'Me.ImageDateAndPremChangeLine.Visible = False
                        'Added 01/13/2021 for CAP Endorsements Task 52976 MLW
                        Me.ctlEndorsementOrChangeHeader.Visible = False
                        Me.EndorsementPrintSection.Visible = False
                        Me.quoteSummaryHeader.Visible = True
                        Me.quoteSummarySection.Visible = True

                        Me.lblPremiumMainAccord.Text = String.Format("{0:C2}", Me.Quote.TotalQuotedPremium)

                        'Dim ph1Name As String = String.Format("{0} {1} {2} {3}", Me.Quote.Policyholder.Name.FirstName, Me.Quote.Policyholder.Name.MiddleName, Me.Quote.Policyholder.Name.LastName, Me.Quote.Policyholder.Name.SuffixName).Replace("  ", " ").Trim()
                        Me.lblPhName.Text = Me.Quote.Policyholder.Name.DisplayName
                        If String.IsNullOrEmpty(Me.Quote.Policyholder.Name.DoingBusinessAsName) = False Then
                            lblPhName.Text = lblPhName.Text + " DBA " + Me.Quote.Policyholder.Name.DoingBusinessAsName
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

                Dim AddressOtherField As AddressOtherField = New AddressOtherField(Me.Quote.Policyholder.Address.Other)
                If AddressOtherField.PrefixType = Helpers.AddressOtherFieldPrefixHelper.OtherFieldPrefix.Other Then
                    Me.lblCareOf.Text = ""
                    Me.trCareOf.Visible = False
                Else
                    Me.lblCareOf.Text = AddressOtherField.NameWithPrefix
                    Me.trCareOf.Visible = True
                End If

                'Removed 01/13/2021 for CAP Endorsements Task 52976 MLW -----
                'Me.lblPremiumMainAccord.Text = String.Format("{0:C2}", Me.Quote.TotalQuotedPremium)

                ''Dim ph1Name As String = String.Format("{0} {1} {2} {3}", Me.Quote.Policyholder.Name.FirstName, Me.Quote.Policyholder.Name.MiddleName, Me.Quote.Policyholder.Name.LastName, Me.Quote.Policyholder.Name.SuffixName).Replace("  ", " ").Trim()
                'Me.lblPhName.Text = Me.Quote.Policyholder.Name.DisplayName
                'If String.IsNullOrEmpty(Me.Quote.Policyholder.Name.DoingBusinessAsName) = False Then
                '    lblPhName.Text = lblPhName.Text + " DBA " + Me.Quote.Policyholder.Name.DoingBusinessAsName
                'End If

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
                '----- end remove for CAP Endorsements

                PopulatePolicyCoverageInformation()

                PopulateOptionalCoverages()

                PopulatePolicyDiscounts()

                If IsOnAppPage AndAlso Me.GoverningStateQuote IsNot Nothing AndAlso Me.GoverningStateQuote.LossHistoryRecords IsNot Nothing AndAlso Me.GoverningStateQuote.LossHistoryRecords.Count > 0 Then
                    PopulateLossHistory()
                End If

                'If IsOnAppPage And Me.Quote?.LossHistoryRecords?.Count > 0 Then
                '    PopulateLossHistory()
                'End If

                'Me.ctlQuoteSummaryActions.Populate()
                Me.ctl_vehicle_item.Populate()
            End If
        Catch ex As Exception
            HandleError("Populate", ex)
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


    Private Sub PopulatePolicyCoverageInformation()
        Try
            Dim html As New StringBuilder()
            html.AppendLine("<div class=""cap_qs_Sub_Sections"">")
            html.AppendLine("<table class=""cap_qa_table_shades"">")
            html.AppendLine("<tr class=""qs_cap_section_grid_headers ui-widget-header qs_cap_grid_3_columns"">")

            ' Header Row
            WriteCell(html, "Coverage")
            WriteCell(html, "Limit", "", "qs_rightJustify")
            WriteCell(html, "Premium", "", "qs_rightJustify")
            html.AppendLine("</tr>")

            ' Data Rows
            Dim LiabUMUIMLimit As String = SubQuoteFirst.Liability_UM_UIM_Limit
            If UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(Quote) Then
                'Liability
                Dim liabilityPremium As String = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.VehiclesTotal_CombinedSingleLimitLiablityQuotedPremium)
                If IsNumeric(liabilityPremium) Then
                    html.AppendLine("<tr class="" qs_cap_grid_3_columns"">")
                    WriteCell(html, "Liability")
                    WriteCell(html, "$" + LiabUMUIMLimit, "", "qs_rightJustify")
                    WriteCell(html, liabilityPremium, "", "qs_rightJustify")
                    html.AppendLine("</tr>")
                End If
            Else
                'Liability/UM/UMI
                'If IsNumeric(Quote.Liability_UM_UIM_QuotedPremium) Then
                'updated 8/14/2018; if property is used multiple times, it would be best to set a variable
                If IsNumeric(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.Liability_UM_UIM_QuotedPremium)) Then
                    'Dim premiumsToCombine As New List(Of String) From {Quote.VehiclesTotal_CombinedSingleLimitLiablityQuotedPremium, Quote.VehiclesTotal_UM_UIM_CovsQuotedPremium}
                    'updated 8/14/2018
                    Dim premiumsToCombine As New List(Of String) From {QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.VehiclesTotal_CombinedSingleLimitLiablityQuotedPremium), QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.VehiclesTotal_UM_UIM_CovsQuotedPremium)}
                    html.AppendLine("<tr class="" qs_cap_grid_3_columns"">")
                    WriteCell(html, "Liability/UM/UIM")
                    'WriteCell(html, "$" + Quote.Liability_UM_UIM_Limit, "", "qs_rightJustify")
                    'updated 8/14/2018; could also SUM limits but probably not what we want
                    WriteCell(html, "$" + LiabUMUIMLimit, "", "qs_rightJustify")
                    WriteCell(html, SumMoneyStrings(premiumsToCombine), "", "qs_rightJustify")
                    'WriteCell(html, Quote.VehiclesTotal_CombinedSingleLimitLiablityQuotedPremium)
                    'WriteCell(html, Quote.VehiclesTotal_UM_UIM_CovsQuotedPremium)
                    html.AppendLine("</tr>")
                End If
            End If

            'Starting 8/1/2025 UMPD moves to be shown between UM and UIM
            If UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(Quote) = False Then
                'UMPD IL only
                If IFM.VR.Common.Helpers.CAP.CAP_UMPDLimitsHelper.IsUMPDLimitsAvailable(Quote) Then
                    Dim IllinoisQuote As QuickQuoteObject = ILSubQuote
                    If IllinoisQuote IsNot Nothing AndAlso QQHelper.IsPositiveIntegerString(IllinoisQuote.UninsuredMotoristPropertyDamage_IL_LimitId) Then
                        Dim umpdLimit = QQHelper.GetStaticDataTextForValueAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, QuickQuoteHelperClass.QuickQuoteState.Illinois, IllinoisQuote.UninsuredMotoristPropertyDamage_IL_LimitId, QuickQuoteObject.QuickQuoteLobType.CommercialAuto)
                        html.AppendLine("<tr class="" qs_cap_grid_3_columns"">")
                        WriteCell(html, "UMPD")
                        WriteCell(html, "$" + umpdLimit, "", "qs_rightJustify")
                        WriteCell(html, "Included", "", "qs_rightJustify")
                        html.AppendLine("</tr>")
                    End If
                End If
            End If

            'Medical Payments
            'If IsNumeric(Quote.MedicalPaymentsQuotedPremium) Then
            'updated 8/14/2018; if property is used multiple times, it would be best to set a variable
            If IsNumeric(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.MedicalPaymentsQuotedPremium)) Then
                html.AppendLine("<tr class="" qs_cap_grid_3_columns"">")
                WriteCell(html, "Medical Payments")
                'WriteCell(html, "$" + Quote.MedicalPaymentsLimit, "", "qs_rightJustify")
                'updated 8/14/2018; could also SUM limits but probably not what we want
                WriteCell(html, "$" + SubQuoteFirst.MedicalPaymentsLimit, "", "qs_rightJustify")
                'WriteCell(html, Quote.VehiclesTotal_MedicalPaymentsQuotedPremium, "", "qs_rightJustify")
                'updated 8/14/2018
                WriteCell(html, QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.VehiclesTotal_MedicalPaymentsQuotedPremium), "", "qs_rightJustify")
                html.AppendLine("</tr>")
            End If

            If UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(Quote) Then
                'UM
                Dim umUimLimit As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.Liability_UM_UIM_LimitId, SubQuoteFirst.UninsuredMotoristPropertyDamageLimitId)
                Dim umPremium As String = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.VehiclesTotal_UninsuredMotoristLiabilityQuotedPremium)
                If IsNumeric(umPremium) Then
                    html.AppendLine("<tr class="" qs_cap_grid_3_columns"">")
                    WriteCell(html, "UM")
                    WriteCell(html, "$" + umUimLimit, "", "qs_rightJustify")
                    WriteCell(html, umPremium, "", "qs_rightJustify") 'prob have QQ work to get total UM prem
                    html.AppendLine("</tr>")
                End If

                'UMPD - Indiana and Illinois only
                If (Quote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Indiana AndAlso QQHelper.IsPositiveIntegerString(SubQuoteFirst.UninsuredMotoristPropertyDamageDeductibleId) OrElse
                    Quote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Illinois AndAlso QQHelper.IsPositiveIntegerString(SubQuoteFirst.UninsuredMotoristPropertyDamage_IL_LimitId)) Then
                    html.AppendLine("<tr class="" qs_cap_grid_3_columns"">")
                    WriteCell(html, "UMPD")
                    If Quote.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Indiana Then
                        WriteCell(html, "Included", "", "qs_rightJustify")
                    Else
                        Dim umpdLimit = QQHelper.GetStaticDataTextForValueAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, QuickQuoteHelperClass.QuickQuoteState.Illinois, SubQuoteFirst.UninsuredMotoristPropertyDamage_IL_LimitId, QuickQuoteObject.QuickQuoteLobType.CommercialAuto)
                        WriteCell(html, "$" + umpdLimit, "", "qs_rightJustify")
                    End If
                    WriteCell(html, "Included", "", "qs_rightJustify")
                    html.AppendLine("</tr>")
                End If

                'UIM
                Dim uimPremium As String = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.VehiclesTotal_UnderinsuredMotoristBodilyInjuryLiabilityQuotedPremium)
                If IsNumeric(uimPremium) Then
                    html.AppendLine("<tr class="" qs_cap_grid_3_columns"">")
                    WriteCell(html, "UIM")
                    WriteCell(html, "$" + umUimLimit, "", "qs_rightJustify")
                    WriteCell(html, uimPremium, "", "qs_rightJustify")
                    html.AppendLine("</tr>")
                End If
            End If

            ' Close Table
            html.AppendLine("</table>")
            Me.tblPolicyCoverages.Text = html.ToString()
        Catch ex As Exception
            HandleError("PopulatePolicyCoverageInformation", ex)
        End Try
    End Sub

    Private Sub PopulateOptionalCoverages()
        Try
            Dim html As New StringBuilder()
            html.AppendLine("<div class=""cap_qs_Sub_Sections"">")
            html.AppendLine("<table class=""cap_qa_table_shades"">")
            html.AppendLine("<tr class=""qs_cap_section_grid_headers ui-widget-header  qs_cap_grid_3_columns"">")

            ' Header Row
            WriteCell(html, "Coverage")
            'WriteCell(html, "Premium", "", "qs_cap_Grid_cell_premium")
            'html.AppendLine("</tr>")
            WriteCell(html, "Limit", "", "qs_rightJustify")
            WriteCell(html, "Premium", "", "qs_rightJustify")
            html.AppendLine("</tr>")

            ' --- Data Rows ---
            ' Garage Keepers
            'updated 8/14/2018 to use variable so it doesn't go through SUM logic multiple times
            Dim gkTotPrem As String = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.GarageKeepersTotalPremium)
            If IsNumeric(gkTotPrem) And (gkTotPrem > 0) Then
                html.AppendLine("<tr class=""  qs_cap_grid_3_columns"">")
                WriteCell(html, "Garage Keepers")
                WriteCell(html, "")
                WriteCell(html, gkTotPrem, "", "qs_rightJustify")
                html.AppendLine("</tr>")

                ' Comp
                'If (Quote.HasGarageKeepersOtherThanCollision) Then
                'updated 8/14/2018; if property is used multiple times, it would be best to set a variable
                If (QQHelper.HasAnyTruePropertyValues(SubQuotes, Function() Quote.HasGarageKeepersOtherThanCollision)) Then
                    html.AppendLine("<tr class=""  qs_cap_grid_3_columns"">")
                    WriteCell(html, indent & "Comp")
                    'WriteCell(html, "$" + Quote.GarageKeepersOtherThanCollisionManualLimitAmount, "", "qs_rightJustify")
                    'updated 8/14/2018; could also SUM limits but probably not what we want
                    WriteCell(html, "$" + SubQuoteFirst.GarageKeepersOtherThanCollisionManualLimitAmount, "", "qs_rightJustify")
                    'WriteCell(html, Quote.GarageKeepersOtherThanCollisionQuotedPremium)
                    WriteCell(html, "")
                    html.AppendLine("</tr>")
                    ' 6
                End If

                ' Collision
                If (QQHelper.HasAnyTruePropertyValues(SubQuotes, Function() Quote.HasGarageKeepersCollision)) Then
                    html.AppendLine("<tr class="" qs_cap_grid_3_columns"">")
                    WriteCell(html, indent & "Collision")
                    WriteCell(html, "$" + SubQuoteFirst.GarageKeepersCollisionManualLimitAmount, "", "qs_rightJustify")
                    'WriteCell(html, Quote.GarageKeepersCollisionQuotedPremium)
                    WriteCell(html, "")

                    html.AppendLine("</tr>")
                End If

            End If


            ' Enhancement Endorsement
            If (QQHelper.HasAnyTruePropertyValues(SubQuotes, Function() Quote.HasBusinessMasterEnhancement)) Then
                html.AppendLine("<tr class="" qs_cap_grid_3_columns"">")
                WriteCell(html, "Enhancement Endorsement")
                WriteCell(html, "")
                WriteCell(html, QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.BusinessMasterEnhancementQuotedPremium), "", "qs_rightJustify")
                html.AppendLine("</tr>")
            End If

            ' Waiver of Subrogation
            'If (Quote.BlanketWaiverOfSubrogation = "3") Then
            'updated 8/14/2018
            If (QQHelper.HasAnyPropertyValuesMatchingString(SubQuotes, Function() Quote.BlanketWaiverOfSubrogation, "3", matchType:=QuickQuote.CommonMethods.QuickQuoteHelperClass.TextMatchType.IntegerOrText_IgnoreCasing)) Then
                html.AppendLine("<tr class="" qs_cap_grid_3_columns"">")
                WriteCell(html, "Waiver of Subrogation")
                WriteCell(html, "")
                WriteCell(html, QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.BlanketWaiverOfSubrogationQuotedPremium), "", "qs_rightJustify")
                html.AppendLine("</tr>")
            End If

            ' Non-Owned Auto Liability
            If (QQHelper.HasAnyTruePropertyValues(SubQuotes, Function() Quote.HasNonOwnershipLiability)) Then
                html.AppendLine("<tr class="" qs_cap_grid_3_columns"">")
                WriteCell(html, "Non-Owned Auto Liability")
                WriteCell(html, "")
                WriteCell(html, QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.NonOwnershipLiabilityQuotedPremium), "", "qs_rightJustify")
                html.AppendLine("</tr>")
            End If

            ' Hire Auto Liability
            If (QQHelper.HasAnyTruePropertyValues(SubQuotes, Function() Quote.HasHiredBorrowedLiability)) Then
                html.AppendLine("<tr class="" qs_cap_grid_3_columns"">")
                WriteCell(html, "Hired Auto Liability")
                WriteCell(html, "")
                WriteCell(html, QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.HiredBorrowedLiabilityQuotedPremium), "", "qs_rightJustify")
                html.AppendLine("</tr>")
            End If

            ' Farm Pollution Liability
            If (QQHelper.HasAnyTruePropertyValues(SubQuotes, Function() Quote.HasFarmPollutionLiability)) Then
                html.AppendLine("<tr class="" qs_cap_grid_3_columns"">")
                WriteCell(html, "Farm Pollution Liability")
                WriteCell(html, "")
                WriteCell(html, QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.FarmPollutionLiabilityQuotedPremium), "", "qs_rightJustify")
                html.AppendLine("</tr>")
            End If


            ' Close Table
            html.AppendLine("</table>")
            Me.tblOptionalCoverages.Text = html.ToString()

        Catch ex As Exception
            HandleError("PopulateOptionalCoverages", ex)
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
                    <div class="cap_qs_Sub_Sections">
                        <table class="cap_qa_table_shades">
                            <tr class="qs_cap_section_grid_headers ui-widget-header  qs_cap_grid_3_columns">
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

    Private Sub PopulateLossHistory()
        Dim html As New StringBuilder()

        ' Start Loss History Section
        html.AppendLine("<div Class='cap_qs_Main_Sections'>")
        'html.AppendLine("<span Class='qs_cap_section_headers'>Buildings</span>")

        html.AppendLine("<div class=""cap_qs_Sub_Sections"">")
        html.AppendLine("<table class=""cap_qa_table_shades"">")

        ' 1st orange column header
        html.AppendLine("<tr class=""qs_cap_section_grid_headers ui-widget-header  qs_cap_grid_4_columns"">")
        WriteCell(html, indent & "Loss History (" + GoverningStateQuote.LossHistoryRecords.Count.ToString + ")", 3)
        WriteCell(html, "", "")
        html.AppendLine("</tr>")

        ' 2nd orange column header
        html.AppendLine("<tr class=""qs_cap_section_grid_headers ui-widget-header"">")
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
            WriteCell(html, FormatCurrency(If(loss IsNot Nothing AndAlso String.IsNullOrWhiteSpace(loss.Amount) = False, loss.Amount, 0)), 2)
            'WriteCell(html, FormatCurrency(loss.Amount), 2)
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