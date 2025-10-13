Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods

Public Class ctlQuoteSummary_DFR
    Inherits VRControlBase

    Dim indentStyle As String = "padding-left:20px;"

    Public ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
                Return Me.Quote.Locations(0)
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property CurrentForm As String
        Get
            Return QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, Quote.Locations(0).FormTypeId).Substring(0, 4)
        End Get
    End Property

    Private Sub Page_Load1(sender As Object, e As EventArgs) Handles Me.Load
        MyBase.ValidateControl(New VRValidationArgs())

    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion("divHomeQuoteSummary", HiddenField1, "0")
        Me.VRScript.StopEventPropagation(Me.lnkPrint.ClientID, False)
        Me.Page.MaintainScrollPositionOnPostBack = False
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Me.UseRatedQuoteImage = True
    End Sub

    Public Overrides Sub Populate()
        If Me.Quote IsNot Nothing Then
            If Me.MyLocation IsNot Nothing Then

                'If IsOnAppPage Then
                '    Me.lblMainAccord.Text = String.Format("App Summary - Effective Date: {0} - 12 month", Me.Quote.EffectiveDate)
                '    Session("SumType") = "App"
                'Else
                '    Me.lblMainAccord.Text = String.Format("Quote Summary - Effective Date: {0} - 12 month", Me.Quote.EffectiveDate)
                '    Session("SumType") = ""
                'End If
                'updated 5/9/2019; logic taken from updates for PPA; DFR now matches HOM exactly
                If IsOnAppPage Then
                    Session("SumType") = "App"
                Else
                    Session("SumType") = ""
                End If
                Select Case Me.Quote.QuoteTransactionType
                    Case QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote, QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage
                        Me.lblMainAccord.Text = If(Me.Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote, "Change", "Image") & " Summary - Updated " & QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.PolicyTermId, Me.Quote.PolicyTermId) & " Premium"
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
                        Me.lblMainAccord.Text = String.Format("{2} - Effective Date: {0} - {1}", Me.Quote.EffectiveDate, QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.PolicyTermId, Me.Quote.PolicyTermId), If(IsOnAppPage, "App", "Quote") & " Summary")
                        Me.ImageDateAndPremChangeLine.Visible = False
                End Select

                Me.lblPremiumMainAccord.Text = String.Format("{0:C2}", Me.Quote.TotalQuotedPremium)

                Dim ph1Name As String = String.Format("{0} {1} {2} {3}", Me.Quote.Policyholder.Name.FirstName, Me.Quote.Policyholder.Name.MiddleName, Me.Quote.Policyholder.Name.LastName, Me.Quote.Policyholder.Name.SuffixName).Replace("  ", " ").Trim()
                'Dim ph2Name As String = String.Format("{0} {1} {2} {3}", Me.Quote.Policyholder2.Name.FirstName, Me.Quote.Policyholder2.Name.MiddleName, Me.Quote.Policyholder2.Name.LastName, Me.Quote.Policyholder2.Name.SuffixName).Replace("  ", " ").Trim()

                Me.lblPhName.Text = ph1Name
                'Me.lblPhName.Text = String.Format(If(String.IsNullOrWhiteSpace(ph2Name), "{0}", "{0} & {1}"), ph1Name, ph2Name).Replace("  ", " ").Trim()

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

                PopulateApplicants()
                PopulateLocations()

                PopulateCoverageSummary()

                'The only Credit is for Deductible in DFR.  Surchareges not needed
                PopulateCredits()

                PopulateIncludedCoverages()
                PopulateOptionalCoverages()

                Me.ctlQuoteSummaryActions.Populate()
            Else
                Me.ctlQuoteSummaryActions.Visible = False
            End If
        End If
    End Sub

    Private Sub WriteCell(sb As StringBuilder, cellText As String)
        sb.AppendLine("<td>")
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

    Private Sub PopulateApplicants()
        Dim html As New StringBuilder()

        html.AppendLine("<table class=""hom_qa_table_shades"">")
        html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")
        WriteCell(html, "Name")
        WriteCell(html, "SSN")

        WriteCell(html, "Birth Date")
        WriteCell(html, "Relationship")

        html.AppendLine("</tr>")

        Dim opCovList = {(New With {.Name = "", .SSN = "", .MaritalStatus = "", .BirthDate = "", .Occupation = "", .Employer = "", .Relationship = ""})}.Take(0).ToList()

        'Updated 9/4/18 for multi state MLW - reverted. No change needed for this on multi state
        If Me.Quote IsNot Nothing AndAlso Me.Quote.Applicants IsNot Nothing Then
            For Each applicant In Me.Quote.Applicants
                'If Me.Quote IsNot Nothing AndAlso Me.GoverningStateQuote IsNot Nothing AndAlso Me.GoverningStateQuote.Applicants IsNot Nothing Then
                '    For Each applicant In Me.GoverningStateQuote.Applicants
                Dim Name As String = String.Format("{0} {1} {2} {3}", applicant.Name.FirstName, applicant.Name.MiddleName, applicant.Name.LastName, applicant.Name.SuffixName).Replace("  ", " ").Trim()
                Dim maritalStatus As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteName, QuickQuoteHelperClass.QuickQuotePropertyName.MaritalStatusId, applicant.Name.MaritalStatusId)
                Dim occupation As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteApplicant, QuickQuoteHelperClass.QuickQuotePropertyName.OccupationTypeId, applicant.OccupationTypeId)
                Dim relationship As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteApplicant, QuickQuoteHelperClass.QuickQuotePropertyName.RelationshipTypeId, applicant.RelationshipTypeId)

                opCovList.Add(New With {.Name = Name, .SSN = applicant.Name.TaxNumber_Hyphens, .MaritalStatus = maritalStatus,
                                        .BirthDate = applicant.Name.BirthDate, .Occupation = occupation, .Employer = applicant.Employer, .Relationship = relationship})
            Next

            For Each c In opCovList
                html.AppendLine("<tr>")

                WriteCell(html, c.Name)
                WriteCell(html, c.SSN)
                WriteCell(html, c.BirthDate)
                WriteCell(html, c.Relationship)

                html.AppendLine("</tr>")
            Next

        End If

        html.AppendLine("</table>")

        Me.tblApplicants.Text = html.ToString()
    End Sub

    Private Sub PopulateLocations()
        Dim html As New StringBuilder()

        html.AppendLine("<table class=""hom_qa_table_shades"">")
        html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")

        WriteCell(html, "Address", "width: 120px;")
        WriteCell(html, "Const", "width: 60px;")
        WriteCell(html, "Year")
        WriteCell(html, "Sq Ft")
        WriteCell(html, "Structure", "width: 60px;")
        WriteCell(html, "Terr")
        WriteCell(html, "P.C.")
        WriteCell(html, "Premium", "", "qs_Grid_cell_premium")

        html.AppendLine("</tr>")

        Dim opCovList = {(New With {.Address = "", .Construction = "", .Year = "", .SqrFeet = "", .Structure = "", .Territory = "", .ProtectionClass = "", .Premium = ""})}.Take(0).ToList()

        If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing Then
            For Each l In Me.Quote.Locations
                Dim zip As String = l.Address.Zip
                If zip.Length > 5 Then
                    zip = zip.Substring(0, 5)
                End If
                Dim address As String = String.Format("{0} {1} {2} {3}</br>{4} {5} {6}", l.Address.HouseNum, l.Address.StreetName, If(String.IsNullOrWhiteSpace(l.Address.ApartmentNumber) = False, "Apt# " + l.Address.ApartmentNumber, ""), l.Address.POBox, l.Address.City, l.Address.State, zip).Replace("  ", " ").Trim()

                Dim construction As String
                If (l.ConstructionTypeId = "2") Then
                    construction = "Masonary"
                Else
                    construction = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.ConstructionTypeId, l.ConstructionTypeId)
                End If

                Dim structureType As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.StructureTypeId, l.StructureTypeId)

                opCovList.Add(New With {.Address = address, .Construction = construction, .Year = l.YearBuilt, .SqrFeet = l.SquareFeet,
                                .Structure = structureType, .Territory = l.TerritoryNumber, .ProtectionClass = l.ProtectionClass, .Premium = l.PremiumFullterm})

            Next

            For Each c In opCovList
                html.AppendLine("<tr>")

                WriteCell(html, c.Address)
                WriteCell(html, c.Construction)
                WriteCell(html, c.Year)
                WriteCell(html, c.SqrFeet)
                WriteCell(html, c.Structure)
                WriteCell(html, c.Territory)
                WriteCell(html, c.ProtectionClass)
                WriteCell(html, c.Premium)

                html.AppendLine("</tr>")
            Next
        End If

        html.AppendLine("</table>")

        Me.tblLocations.Text = html.ToString()
    End Sub

    Private Sub PopulateCoverageSummary()
        Dim html As New StringBuilder()

        Dim opCovList = {(New With {.Name = "", .Limit = "", .Premium = ""})}.Take(0).ToList()

        html.AppendLine("<table class=""hom_qa_table_shades"">")
        html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")

        WriteCell(html, "Deductible")
        WriteCell(html, "Limit", "", "qs_Grid_cell_premium")
        WriteCell(html, "        ", "", "qs_Grid_cell_premium")
        html.AppendLine("</tr>")

        Dim deductibleLimit As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleLimitId, MyLocation.DeductibleLimitId)
        opCovList.Add(New With {.Name = "Deductible", .Limit = deductibleLimit, .Premium = ""})

        For Each c In opCovList
            html.AppendLine("<tr>")

            WriteCell(html, c.Name)
            WriteCell(html, c.Limit)
            WriteCell(html, c.Premium)

            html.AppendLine("</tr>")
        Next

        html.AppendLine("</table>")

        opCovList.Clear()
        html.AppendLine("<table class=""hom_qa_table_shades"">")
        html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")

        WriteCell(html, "Coverage")
        WriteCell(html, "Limit", "", "qs_Grid_cell_premium")
        WriteCell(html, "Premium", "", "qs_Grid_cell_premium")

        html.AppendLine("</tr>")

        ' do not show on HO-4 and ML-4
        If Me.MyLocation.FormTypeId <> "4" And Me.MyLocation.FormTypeId <> "7" Then
            Dim A_included As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.A_Dwelling_LimitIncluded)
            Dim A_increased As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.A_Dwelling_LimitIncreased)
            If (Me.MyLocation.FormTypeId = "11" Or Me.MyLocation.FormTypeId = "12") Then
                Dim AECPrem As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.A_Dwelling_EC_QuotedPremium)
                Dim AVMMPrem As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.A_Dwelling_VMM_QuotedPremium)
                Dim APrem As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.A_Dwelling_QuotedPremium)
                opCovList.Add(New With {.Name = "A. Dwelling", .Limit = String.Format("{0:N0} <sup>1</sup>", A_included + A_increased), .Premium = If(APrem + AECPrem + AVMMPrem > 0, (APrem + AECPrem + AVMMPrem).TryToFormatAsCurreny(), "Included")})
            Else
                opCovList.Add(New With {.Name = "A. Dwelling", .Limit = String.Format("{0:N0} <sup>1</sup>", A_included + A_increased), .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.MyLocation.A_Dwelling_QuotedPremium) > 0, Me.MyLocation.A_Dwelling_QuotedPremium, "Included")})
            End If

        End If

        ' do not show on HO-4 and ML-4 and Ho-6
        If Me.MyLocation.FormTypeId <> "4" And Me.MyLocation.FormTypeId <> "7" And Me.MyLocation.FormTypeId <> "5" Then
            Dim B_included As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.B_OtherStructures_LimitIncluded)
            Dim B_increased As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.B_OtherStructures_LimitIncreased)
            If (Me.MyLocation.FormTypeId = "11" Or Me.MyLocation.FormTypeId = "12") Then
                Dim BECPrem As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.B_OtherStructures_EC_QuotedPremium)
                Dim BVMMPrem As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.B_OtherStructures_VMM_QuotedPremium)
                Dim BPrem As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.B_OtherStructures_QuotedPremium)
                opCovList.Add(New With {.Name = "B. Other Structures", .Limit = String.Format("{0:N0}", B_included + B_increased), .Premium = If((B_increased + BPrem + BECPrem + BVMMPrem) <= 0, "Included", (BPrem + BECPrem + BVMMPrem).TryToFormatAsCurreny())})
            Else
                opCovList.Add(New With {.Name = "B. Other Structures", .Limit = String.Format("{0:N0}", B_included + B_increased), .Premium = If(B_increased <= 0, "Included", Me.MyLocation.B_OtherStructures_QuotedPremium)})
            End If

        End If

        Dim C_included As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.C_PersonalProperty_LimitIncluded)
        Dim C_increased As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.C_PersonalProperty_LimitIncreased)

        If MyLocation.FormTypeId <> "4" Then
            If (Me.MyLocation.FormTypeId = "11" Or Me.MyLocation.FormTypeId = "12") Then
                Dim CECPrem As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.C_Contents_EC_QuotedPremium)
                Dim CVMMPrem As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.C_Contents_VMM_QuotedPremium)
                Dim CPrem As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.C_PersonalProperty_QuotedPremium)
                opCovList.Add(New With {.Name = "C. Personal Property", .Limit = String.Format("{0:N0}", C_included + C_increased), .Premium = If((C_increased + CPrem + CECPrem + CVMMPrem) <= 0, "Included", (CPrem + CECPrem + CVMMPrem).TryToFormatAsCurreny())})
            Else
                opCovList.Add(New With {.Name = "C. Personal Property", .Limit = String.Format("{0:N0}", C_included + C_increased), .Premium = If(C_increased <= 0, "Included", Me.MyLocation.C_PersonalProperty_QuotedPremium)})
            End If

        Else
            opCovList.Add(New With {.Name = "C. Personal Property", .Limit = String.Format("{0:N0} <sup>1</sup>", C_included + C_increased), .Premium = If(C_increased <= 0, "Included", Me.MyLocation.C_PersonalProperty_QuotedPremium)})
        End If

        Dim D_included As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.D_LossOfUse_LimitIncluded)
        Dim D_increased As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.D_LossOfUse_LimitIncreased)

        Select Case Me.MyLocation.FormTypeId
            Case "6" ' ml-2
                opCovList.Add(New With {.Name = "D. Additional Living Cost/Fair Rental Value", .Limit = String.Format("{0:N0}", D_included + D_increased), .Premium = If(D_increased <= 0, "Included", Me.MyLocation.D_LossOfUse_QuotedPremium)})
            Case "7" 'ml-4
                opCovList.Add(New With {.Name = "D. Additional Living Cost/Fair Rental Value", .Limit = String.Format("{0:N0}", D_included + D_increased), .Premium = If(D_increased <= 0, "Included", Me.MyLocation.D_LossOfUse_QuotedPremium)})
            Case Else
                If (Me.MyLocation.FormTypeId = "11" Or Me.MyLocation.FormTypeId = "12") Then
                    Dim DECPrem As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.D_and_E_EC_QuotedPremium)
                    Dim DVMMPrem As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.D_and_E_VMM_QuotedPremium)
                    Dim DPrem As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.D_LossOfUse_QuotedPremium)
                    opCovList.Add(New With {.Name = "D. Additional Living Cost/Fair Rental Value", .Limit = String.Format("{0:N0}", D_included + D_increased), .Premium = If((D_increased + DPrem + DECPrem + DVMMPrem) <= 0, "Included", (DPrem + DECPrem + DVMMPrem).TryToFormatAsCurreny())})
                Else
                    opCovList.Add(New With {.Name = "D. Additional Living Cost/Fair Rental Value", .Limit = String.Format("{0:N0}", D_included + D_increased), .Premium = If(D_increased <= 0, "Included", Me.MyLocation.D_LossOfUse_QuotedPremium)})
                End If

        End Select

        'Updated 9/4/18 for multi state MLW - reverted. No change needed for this on multi state
        'If Me.SubQuoteFirst IsNot Nothing Then
        'Updated 9/4/18 for multi state MLW - reverted. No change needed for this on multi state
        Dim E_Personal As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.PersonalLiabilityLimitId, Me.Quote.PersonalLiabilityLimitId, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal)
            opCovList.Add(New With {.Name = "L. Personal Liability", .Limit = E_Personal, .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.Quote.PersonalLiabilityQuotedPremium) > 0, Me.Quote.PersonalLiabilityQuotedPremium, "Included")})
            'Dim E_Personal As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.PersonalLiabilityLimitId, Me.SubQuoteFirst.PersonalLiabilityLimitId, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal)
            'opCovList.Add(New With {.Name = "L. Personal Liability", .Limit = E_Personal, .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.SubQuoteFirst.PersonalLiabilityQuotedPremium) > 0, Me.SubQuoteFirst.PersonalLiabilityQuotedPremium, "Included")})

            'Updated 9/4/18 for multi state MLW - reverted. No change needed for this on multi state
            Dim F_Medical As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.MedicalPaymentsLimitId, Me.Quote.MedicalPaymentsLimitid, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal)
            opCovList.Add(New With {.Name = "M. Medical Payment", .Limit = F_Medical, .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.Quote.MedicalPaymentsQuotedPremium) > 0, Me.Quote.MedicalPaymentsQuotedPremium, "Included")})
        'Dim F_Medical As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.MedicalPaymentsLimitId, Me.SubQuoteFirst.MedicalPaymentsLimitid, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal)
        'opCovList.Add(New With {.Name = "M. Medical Payment", .Limit = F_Medical, .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.SubQuoteFirst.MedicalPaymentsQuotedPremium) > 0, Me.SubQuoteFirst.MedicalPaymentsQuotedPremium, "Included")})
        'End If

        For Each c In opCovList
            If String.IsNullOrWhiteSpace(c.Name) Then
                html.AppendLine("<tr style=""height: 20px;"">")
            Else
                html.AppendLine("<tr>")
            End If

            WriteCell(html, c.Name)
            WriteCell(html, c.Limit)
            WriteCell(html, c.Premium)

            html.AppendLine("</tr>")
        Next

        html.AppendLine("</table>")
        Me.tblCoverageSummary.Text = html.ToString()
    End Sub

    Private Sub PopulateCredits()
        Dim html As New StringBuilder()
        html.AppendLine("<table class=""hom_qa_table_shades"">")
        html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header "">")
        WriteCell(html, "Credit")
        WriteCell(html, "Percent", "", "qs_Grid_cell_premium")
        html.AppendLine("</tr>")

        Dim opCovList = {(New With {.Name = "", .Percent = ""})}.Take(0).ToList()

        For Each c In IFM.VR.Common.Helpers.HOM.HOMCreditFactors.GetPolicyDiscountsAsListOfPercents(Me.Quote, False)
            opCovList.Add(New With {.Name = c.Key, .Percent = c.Value})
        Next

        For Each c In From o In opCovList Order By IFM.Common.InputValidation.InputHelpers.TryToGetDouble(o.Percent) Descending Select o
            html.AppendLine("<tr>")
            WriteCell(html, c.Name)
            WriteCell(html, c.Percent)
            html.AppendLine("</tr>")
        Next

        html.AppendLine("</table>")
        Me.tblDiscounts.Text = html.ToString()
    End Sub



    Private Sub PopulateIncludedCoverages()
        Dim html As New StringBuilder()

        If (Me.MyLocation.FormTypeId = "11" Or Me.MyLocation.FormTypeId = "12") Then
            formTypeCoveragesSection.Visible = False
        Else

            html.AppendLine("<table class=""hom_qa_table_shades"">")
            html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")

            WriteCell(html, "Coverage")
            WriteCell(html, "Limit", "", "qs_Grid_cell_premium")
            WriteCell(html, "Premium", "", "qs_Grid_cell_premium")

            html.AppendLine("</tr>")

            Dim opCovList = {(New With {.Name = "", .Limit = "", .Premium = "", .Style = ""})}.Take(0).ToList()
            Dim A_limit As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.A_Dwelling_Limit)
            Dim B_limit As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.B_OtherStructures_Limit)
            Dim C_limit As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.C_PersonalProperty_Limit)
            Dim D_limit As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.D_LossOfUse_Limit)

            'FormType/Included Coverages
            'DP 00 01 Fire, EC
            If Me.MyLocation.FormTypeId = "9" Then
                If (IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.MyLocation.A_Dwelling_Limit) > 0) Then
                    opCovList.Add(New With {.Name = "Cov. A - Dwelling EC Base Fire Premium", .Limit = String.Format("{0:N0} <sup>1</sup>", A_limit), .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.MyLocation.A_Dwelling_EC_QuotedPremium) > 0, Me.MyLocation.A_Dwelling_EC_QuotedPremium, "Included"), .Style = ""})
                End If
                If (IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.MyLocation.B_OtherStructures_Limit) > 0) Then
                    opCovList.Add(New With {.Name = "Cov. B -  Other Structures EC ", .Limit = String.Format("{0:N0} ", B_limit), .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.MyLocation.B_OtherStructures_EC_QuotedPremium) > 0, Me.MyLocation.B_OtherStructures_EC_QuotedPremium, "Included"), .Style = ""})
                End If
                If (IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.MyLocation.C_PersonalProperty_Limit) > 0) Then
                    opCovList.Add(New With {.Name = "Cov. C - Personal Property EC", .Limit = String.Format("{0:N0} ", C_limit), .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.MyLocation.C_Contents_EC_QuotedPremium) > 0, Me.MyLocation.C_Contents_EC_QuotedPremium, "Included"), .Style = ""})
                End If
                If (IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.MyLocation.D_LossOfUse_Limit) > 0) Then
                    opCovList.Add(New With {.Name = "Cov. D - Additional Living Cost/Fair Rental Value EC", .Limit = String.Format("{0:N0} ", D_limit), .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.MyLocation.D_and_E_EC_QuotedPremium) > 0, Me.MyLocation.D_and_E_EC_QuotedPremium, "Included"), .Style = ""})
                End If
            End If
            'DP 00 01 Fire, EC, VMM and DP 00 02 Broad, DP 00 03 Special
            If Me.MyLocation.FormTypeId = "10" Then
                'Per bugtraq 6068 We need to display these values if they have a premium attached or not.
                If (IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.MyLocation.A_Dwelling_Limit) > 0) Then
                    opCovList.Add(New With {.Name = "Cov. A - Dwelling EC Base Fire Premium", .Limit = String.Format("{0:N0} <sup>1</sup>", A_limit), .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.MyLocation.A_Dwelling_EC_QuotedPremium) > 0, Me.MyLocation.A_Dwelling_EC_QuotedPremium, "Included"), .Style = ""})
                End If
                If (IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.MyLocation.A_Dwelling_Limit) > 0) Then
                    opCovList.Add(New With {.Name = "Cov. A - Dwelling V&MM Base Premium", .Limit = String.Format("{0:N0} <sup>1</sup>", A_limit), .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.MyLocation.A_Dwelling_VMM_QuotedPremium) > 0, Me.MyLocation.A_Dwelling_VMM_QuotedPremium, "Included"), .Style = ""})
                End If
                If (IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.MyLocation.B_OtherStructures_Limit) > 0) Then
                    opCovList.Add(New With {.Name = "Cov. B -  Other Structures EC", .Limit = String.Format("{0:N0} ", B_limit), .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.MyLocation.B_OtherStructures_EC_QuotedPremium) > 0, Me.MyLocation.B_OtherStructures_EC_QuotedPremium, "Included"), .Style = ""})
                End If
                If (IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.MyLocation.B_OtherStructures_Limit) > 0) Then
                    opCovList.Add(New With {.Name = "Cov. B - Other Structures V&MM", .Limit = String.Format("{0:N0} ", B_limit), .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.MyLocation.B_OtherStructures_VMM_QuotedPremium) > 0, Me.MyLocation.B_OtherStructures_VMM_QuotedPremium, "Included"), .Style = ""})
                End If
                If (IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.MyLocation.C_PersonalProperty_Limit) > 0) Then
                    opCovList.Add(New With {.Name = "Cov. C - Personal Property EC", .Limit = String.Format("{0:N0} ", C_limit), .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.MyLocation.C_Contents_EC_QuotedPremium) > 0, Me.MyLocation.C_Contents_EC_QuotedPremium, "Included"), .Style = ""})
                End If
                If (IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.MyLocation.C_PersonalProperty_Limit) > 0) Then
                    opCovList.Add(New With {.Name = "Cov. C - Personal Property V&MM", .Limit = String.Format("{0:N0} ", C_limit), .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.MyLocation.C_Contents_VMM_QuotedPremium) > 0, Me.MyLocation.C_Contents_VMM_QuotedPremium, "Included"), .Style = ""})
                End If
                If (IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.MyLocation.D_LossOfUse_Limit) > 0) Then
                    opCovList.Add(New With {.Name = "Cov. D - Additional Living Cost/Fair Rental Value EC", .Limit = String.Format("{0:N0} ", D_limit), .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.MyLocation.D_and_E_EC_QuotedPremium) > 0, Me.MyLocation.D_and_E_EC_QuotedPremium, "Included"), .Style = ""})
                End If
                If (IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.MyLocation.D_LossOfUse_Limit) > 0) Then
                    opCovList.Add(New With {.Name = "Cov. D - Additional Living Cost/Fair Rental Value V&MM", .Limit = String.Format("{0:N0} ", D_limit), .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.MyLocation.D_and_E_VMM_QuotedPremium) > 0, Me.MyLocation.D_and_E_VMM_QuotedPremium, "Included"), .Style = ""})
                End If

            End If
            'If formtype = DP 00 01 Fire then Diplay N/A instead of this section
            If Me.MyLocation.FormTypeId = "8" Then
                opCovList.Add(New With {.Name = "N/A", .Limit = "", .Premium = "", .Style = ""})
            End If

            'Display Optional Coverages
            For Each c In opCovList
                html.AppendLine("<tr>")
                WriteCell(html, c.Name, c.Style)
                WriteCell(html, c.Limit)
                WriteCell(html, c.Premium)

                html.AppendLine("</tr>")
            Next

            html.AppendLine("</table>")

            Me.tblIncludedCoverages.Text = html.ToString()
        End If
    End Sub

    Private Sub PopulateOptionalCoverages()
        Dim html As New StringBuilder()

        html.AppendLine("<table class=""hom_qa_table_shades"">")
        html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")

        WriteCell(html, "Coverage")
        WriteCell(html, "", "", "qs_Grid_cell_premium")
        WriteCell(html, "Premium", "", "qs_Grid_cell_premium")

        html.AppendLine("</tr>")

        Dim opCovList = {(New With {.Name = "", .Limit = "", .Premium = "", .Index = 0, .Style = ""})}.Take(0).ToList()

        If Me.MyLocation.SectionICoverages IsNot Nothing Then
            For Each cov In Me.MyLocation.SectionICoverages
                Dim prem As String = cov.Premium
                If IFM.Common.InputValidation.InputHelpers.TryToGetDouble(cov.Premium) = 0.0 Then
                    prem = "Included"
                End If
                Dim A_included As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.A_Dwelling_LimitIncluded)
                Dim A_increased As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.A_Dwelling_LimitIncreased)
                Dim CovALimit As Double = A_included + A_increased
                Select Case cov.DFR_CoverageType
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.DFR_SectionICoverageType.ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing
                        'Not Available on any DP 00 01 Form Type per DFR Requirements
                        If Me.MyLocation.FormTypeId = "11" Or Me.MyLocation.FormTypeId = "12" Then
                            opCovList.Add(New With {.Name = "Actual Cash Value Loss Settlement/Windstorm or Hail Losses to Roof Surfacing", .Limit = "", .Premium = prem, .Index = 0, .Style = ""})
                        End If
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.DFR_SectionICoverageType.Earthquake
                        Dim deductible As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleLimitId, cov.DeductibleLimitId)
                        opCovList.Add(New With {.Name = String.Format("Earthquake - (Deductible {0})", deductible), .Limit = "", .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(prem) > 0, prem, "Included"), .Index = 9, .Style = ""})
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.DFR_SectionICoverageType.FunctionalReplacementCostLossAssessment
                        'Not Available on any DP 00 01 Form Type per DFR Requirements
                        If Me.MyLocation.FormTypeId = "11" Or Me.MyLocation.FormTypeId = "12" Then
                            opCovList.Add(New With {.Name = "Functional Replacement Cost", .Limit = "N/A", .Premium = prem, .Index = 2, .Style = ""})
                        End If
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.DFR_SectionICoverageType.MineSubsidenceCovA
                        ' limit - lesser of Cov A or $200,000
                        Dim Limit As String = ""
                        If CovALimit > 200000 Then
                            Limit = "$200,000"
                        Else
                            Limit = String.Format("{0:C0}", CovALimit)
                        End If
                        opCovList.Add(New With {.Name = "Mine Subsidence Cov A", .Limit = "", .Premium = prem, .Index = 11, .Style = ""})
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.DFR_SectionICoverageType.MineSubsidenceCovAAndB
                        ' limit - lesser of Cov A or $200,000
                        Dim Limit As String = ""
                        If CovALimit > 200000 Then
                            Limit = "$200,000"
                        Else
                            Limit = String.Format("{0:C0}", CovALimit)
                        End If
                        opCovList.Add(New With {.Name = "Mine Subsidence Cov A & B", .Limit = "", .Premium = prem, .Index = 10, .Style = ""})
                    Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.DFR_SectionICoverageType.SinkholeCollapse
                        opCovList.Add(New With {.Name = "Sinkhole Collapse", .Limit = "", .Premium = prem, .Index = 13, .Style = ""})
                End Select
            Next

        Else
            opCovList.Add(New With {.Name = "N/A", .Limit = "", .Premium = "", .Index = 0, .Style = ""})
        End If

        If Me.MyLocation.SectionIAndIICoverages IsNot Nothing Then
            For Each cov In Me.MyLocation.SectionIAndIICoverages
                Select Case cov.MainCoverageType
                End Select
            Next
        End If

        If Me.MyLocation.SectionIICoverages IsNot Nothing Then
            For Each cov In Me.MyLocation.SectionIICoverages
                Dim prem As String = cov.Premium
                If IFM.Common.InputValidation.InputHelpers.TryToGetDouble(cov.Premium) = 0.0 Then
                    prem = "Included"
                End If
                Select Case cov.DFR_CoverageType
                    Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.DFR_SectionIICoverageType.Non_OwnerOccupiedDwelling
                        opCovList.Add(New With {.Name = "Non-Owner Occupied Dwelling", .Limit = "", .Premium = prem, .Index = 12, .Style = ""})
                End Select
            Next
        End If

        For Each c In (From a In opCovList Order By a.Index Select a)
            html.AppendLine("<tr>")

            WriteCell(html, c.Name, c.Style)
            WriteCell(html, "") ' Convert $0 to N/A
            WriteCell(html, c.Premium)

            html.AppendLine("</tr>")
        Next

        html.AppendLine("</table>")
        html.AppendLine("<br/><sup>1</sup>Limit values are rounded to the next highest $1000.")

        Me.tblOptionalCoverages.Text = html.ToString()
    End Sub

    Public Overrides Function Save() As Boolean
        ' no saving needed
        Return True
    End Function

    Protected Sub lnkPrint_Click(sender As Object, e As EventArgs) Handles lnkPrint.Click
        'Response.Redirect(String.Format("~/Reports/DFR/PFQuoteSummary_DFR.aspx?quoteid={0}&summarytype={1}", Request.QueryString("QuoteId").ToString, Session("SumType")))
        'updated 5/8/2019
        Dim quoteOrPolicyInfo As String = ""
        If String.IsNullOrWhiteSpace(ReadOnlyPolicyIdAndImageNum) = False Then
            quoteOrPolicyInfo = "ReadOnlyPolicyIdAndImageNum=" & Me.ReadOnlyPolicyId.ToString & "|" & Me.ReadOnlyPolicyImageNum.ToString
        ElseIf String.IsNullOrWhiteSpace(EndorsementPolicyIdAndImageNum) = False Then
            quoteOrPolicyInfo = "EndorsementPolicyIdAndImageNum=" & Me.EndorsementPolicyId.ToString & "|" & Me.EndorsementPolicyImageNum.ToString
        ElseIf String.IsNullOrWhiteSpace(Me.QuoteId) = False Then
            quoteOrPolicyInfo = "quoteid=" & Me.QuoteId
        End If
        If String.IsNullOrWhiteSpace(quoteOrPolicyInfo) = False Then
            Dim sumType As String = ""
            If Session("SumType") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(Session("SumType").ToString) = False AndAlso UCase(Session("SumType").ToString) = "APP" Then
                sumType = "App"
            End If
            Response.Redirect(String.Format("~/Reports/DFR/PFQuoteSummary_DFR.aspx?{0}&summarytype={1}", quoteOrPolicyInfo, sumType))
        End If
    End Sub

    'added 2/18/2020
    Public Sub CheckForReRateAfterEffDateChange(Optional ByVal qqTranType As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.None, Optional ByVal newEffectiveDate As String = "", Optional ByVal oldEffectiveDate As String = "")
        Me.ctlQuoteSummaryActions.CheckForReRateAfterEffDateChange(qqTranType:=qqTranType, newEffectiveDate:=newEffectiveDate, oldEffectiveDate:=oldEffectiveDate)
    End Sub

End Class