Imports QuickQuote.CommonMethods
Imports System.Globalization
Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions.IFMExtensions
Imports IFM.VR.Common.Helpers.HOM

Public Class ctlQuoteSummary_HOM
    Inherits VRControlBase

    'This control is only used for HOM, so no multi state changes are needed 8/23/18 MLW

    Dim HasIncedentalFarmOrFarmOwnder = False
    Dim indentStyle As String = "padding-left:20px;"

    Protected ReadOnly Property HomeVersion As String
        Get
            Dim effectiveDate As DateTime
            If Me.Quote IsNot Nothing Then
                If Me.Quote.EffectiveDate IsNot Nothing AndAlso Me.Quote.EffectiveDate <> String.Empty Then
                    effectiveDate = Me.Quote.EffectiveDate
                Else
                    effectiveDate = Now()
                End If
            Else
                effectiveDate = Now()
            End If
            If QQHelper.doUseNewVersionOfLOB(Quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                Return "After20180701"
            Else
                Return "Before20180701"
            End If
        End Get
    End Property

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
            ''11/16/17 added for HOM Upgrade MLW, was just else statement
            'If Session("homeVersion") = "After20180701" Then
            '    Return QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, Quote.Locations(0).FormTypeId).Substring(0, 7)
            'Else
            '    Return QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.FormTypeId, Quote.Locations(0).FormTypeId).Substring(0, 4)
            'End If

            'Updated 11/27/17 for HOM Upgrade MLW
            Try
                Return QQHelper.GetShortFormName(Quote)
            Catch ex As Exception
                Return ""
            End Try
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
                'Else
                '    Me.lblMainAccord.Text = String.Format("Quote Summary - Effective Date: {0} - 12 month", Me.Quote.EffectiveDate)
                'End If
                'updated 5/9/2019; logic taken from updates for PPA; had to also make sure SumType Session variable was being set since it wasn't previously
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

                Me.lblRatedTier.Text = If(Me.Quote.TieringInformation IsNot Nothing, Me.Quote.TieringInformation.RatedTier, "")

                PopulateApplicants()
                PopulateLocations()

                PopulateCoverageSummary()

                PopulateCredits()
                PopulateSurcharges()

                PopulateIncludedCoverages()
                PopulateOptionalCoverages()

                PopulateInlandMarine()

                PopulateRvWaterCraft()

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
        'If IsOnAppPage Then
        '    WriteCell(html, "Marital Status")
        '    WriteCell(html, "Occupation")
        '    WriteCell(html, "Employer")
        'End If
        WriteCell(html, "Relationship")

        html.AppendLine("</tr>")

        Dim opCovList = {(New With {.Name = "", .SSN = "", .MaritalStatus = "", .BirthDate = "", .Occupation = "", .Employer = "", .Relationship = ""})}.Take(0).ToList()

        If Me.Quote IsNot Nothing AndAlso Me.Quote.Applicants IsNot Nothing Then
            For Each applicant In Me.Quote.Applicants
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
                Dim construction As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.ConstructionTypeId, l.ConstructionTypeId)
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

        html.AppendLine("<table class=""hom_qa_table_shades"" style=""width: 465px; margin-bottom:20px;"">")
        html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")

        WriteCell(html, "Deductible")
        WriteCell(html, "Limit", "", "qs_Grid_cell_premium")
        'WriteCell(html, "Premium", "width: 70px;")

        html.AppendLine("</tr>")

        Dim deductibleLimit As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleLimitId, MyLocation.DeductibleLimitId)
        opCovList.Add(New With {.Name = "Deductible", .Limit = deductibleLimit, .Premium = ""})

        Dim windHailDeductibleLimit As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.WindHailDeductibleLimitId, MyLocation.WindHailDeductibleLimitId)

        'Updated 4/10/18 for HOM Upgrade Bug 26124 Wind/Hail Deductible N/A and removed HO-6 MLW
        If QQHelper.doUseNewVersionOfLOB(Quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
            If CurrentForm = "HO-2" OrElse CurrentForm = "HO-3" Then
                If MyLocation.WindHailDeductibleLimitId <> "0" Then
                    opCovList.Add(New With {.Name = "Wind/Hail Deductible", .Limit = windHailDeductibleLimit, .Premium = ""})
                Else
                    ' when n/a wind/hail then just show policy level deductible
                    opCovList.Add(New With {.Name = "Wind/Hail Deductible", .Limit = deductibleLimit, .Premium = ""})
                End If
            End If
        Else
            If CurrentForm = "HO-2" And CurrentForm <> "ML-4" And CurrentForm <> "HO-4" And CurrentForm <> "HO-6" Then
                If MyLocation.WindHailDeductibleLimitId <> "0" Then
                    opCovList.Add(New With {.Name = "Wind/Hail Deductible", .Limit = windHailDeductibleLimit, .Premium = ""})
                Else
                    ' when n/a wind/hail then just show policy level deductible
                    opCovList.Add(New With {.Name = "Wind/Hail Deductible", .Limit = deductibleLimit, .Premium = ""})
                End If
            End If
        End If


        For Each c In opCovList
            html.AppendLine("<tr>")

            WriteCell(html, c.Name)
            WriteCell(html, c.Limit)
            'WriteCell(html, c.Premium)

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

        ' do not show on HO-4 and ML-4     'updated 11/15/17 for HOM Upgrade - included 25, HO 0004 home/mobile - MLW
        'Updated 12/4/17 for HOM Upgrade MLW
        'If Me.MyLocation.FormTypeId <> "4" And Me.MyLocation.FormTypeId <> "7" And Me.MyLocation.FormTypeId <> "25" Then
        If CurrentForm <> "HO-4" And CurrentForm <> "ML-4" Then
            Dim A_included As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.A_Dwelling_LimitIncluded)
            Dim A_increased As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.A_Dwelling_LimitIncreased)
            opCovList.Add(New With {.Name = "A. Dwelling", .Limit = String.Format("{0:N0} <sup>1</sup>", A_included + A_increased), .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.MyLocation.A_Dwelling_QuotedPremium) > 0, Me.MyLocation.A_Dwelling_QuotedPremium, "Included")})
        End If

        ' do not show on HO-4 and ML-4 and Ho-6      'updated 11/15/17 for HOM Upgrade - included 25 and 26, HO 0004 home/mobile and HO 0006 - MLW
        'Updated 12/4/17 for HOM Upgrade MLW
        'If Me.MyLocation.FormTypeId <> "4" And Me.MyLocation.FormTypeId <> "7" And Me.MyLocation.FormTypeId <> "5" And Me.MyLocation.FormTypeId <> "25" And Me.MyLocation.FormTypeId <> "26" Then
        If CurrentForm <> "HO-4" And CurrentForm <> "ML-4" And CurrentForm <> "HO-6" Then
            Dim B_included As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.B_OtherStructures_LimitIncluded)
            Dim B_increased As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.B_OtherStructures_LimitIncreased)
            opCovList.Add(New With {.Name = "B. Other Structures", .Limit = String.Format("{0:N0}", B_included + B_increased), .Premium = If(B_increased <= 0, "Included", Me.MyLocation.B_OtherStructures_QuotedPremium)})
        End If

        Dim C_included As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.C_PersonalProperty_LimitIncluded)
        Dim C_increased As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.C_PersonalProperty_LimitIncreased)

        'Updated 12/4/17 for HOM Upgrade MLW
        'If MyLocation.FormTypeId <> "4" And MyLocation.FormTypeId <> "25" Then
        If CurrentForm <> "HO-4" Then
            opCovList.Add(New With {.Name = "C. Personal Property", .Limit = String.Format("{0:N0}", C_included + C_increased), .Premium = If(C_increased <= 0, "Included", Me.MyLocation.C_PersonalProperty_QuotedPremium)})
        Else
            opCovList.Add(New With {.Name = "C. Personal Property", .Limit = String.Format("{0:N0} <sup>1</sup>", C_included + C_increased), .Premium = If(C_increased <= 0, "Included", Me.MyLocation.C_PersonalProperty_QuotedPremium)})
        End If

        Dim D_included As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.D_LossOfUse_LimitIncluded)
        Dim D_increased As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.D_LossOfUse_LimitIncreased)

        If HomeVersion <> "After20180701" Then ' Matt A - 5-4-18
            'Updated 12/4/17 for HOM Upgrade MLW
            Select Case CurrentForm
                Case "ML-2"
                    opCovList.Add(New With {.Name = "D. Loss of Use", .Limit = String.Format("{0:N0}", D_included + D_increased), .Premium = If(D_increased <= 0, "Included", Me.MyLocation.D_LossOfUse_QuotedPremium)})
                Case "ML-4"
                    opCovList.Add(New With {.Name = "D. Loss of Use", .Limit = String.Format("{0:N0}", D_included + D_increased), .Premium = If(D_increased <= 0, "Included", Me.MyLocation.D_LossOfUse_QuotedPremium)})
                Case Else
                    opCovList.Add(New With {.Name = "D. Loss of Use", .Limit = "(see <sup>2</sup>)", .Premium = If(D_increased <= 0, "Included", Me.MyLocation.D_LossOfUse_QuotedPremium)})
            End Select
        Else
            opCovList.Add(New With {.Name = "D. Loss of Use", .Limit = "(see <sup>2</sup>)", .Premium = If(D_increased <= 0, "Included", Me.MyLocation.D_LossOfUse_QuotedPremium)})
        End If

        'Select Case Me.MyLocation.FormTypeId
        '    Case "6" ' ml-2
        '        opCovList.Add(New With {.Name = "D. Loss of Use", .Limit = String.Format("{0:N0}", D_included + D_increased), .Premium = If(D_increased <= 0, "Included", Me.MyLocation.D_LossOfUse_QuotedPremium)})
        '    Case "7" 'ml-4
        '        opCovList.Add(New With {.Name = "D. Loss of Use", .Limit = String.Format("{0:N0}", D_included + D_increased), .Premium = If(D_increased <= 0, "Included", Me.MyLocation.D_LossOfUse_QuotedPremium)})
        '    Case "22" 'HO 0002
        '        If Me.MyLocation.StructureTypeId = "2" Then
        '            opCovList.Add(New With {.Name = "D. Loss of Use", .Limit = String.Format("{0:N0}", D_included + D_increased), .Premium = If(D_increased <= 0, "Included", Me.MyLocation.D_LossOfUse_QuotedPremium)})
        '        Else
        '            opCovList.Add(New With {.Name = "D. Loss of Use", .Limit = "(see <sup>2</sup>)", .Premium = If(D_increased <= 0, "Included", Me.MyLocation.D_LossOfUse_QuotedPremium)})
        '        End If
        '    Case "25" 'HO 0004
        '        If Me.MyLocation.StructureTypeId = "2" Then
        '            opCovList.Add(New With {.Name = "D. Loss of Use", .Limit = String.Format("{0:N0}", D_included + D_increased), .Premium = If(D_increased <= 0, "Included", Me.MyLocation.D_LossOfUse_QuotedPremium)})
        '        Else
        '            opCovList.Add(New With {.Name = "D. Loss of Use", .Limit = "(see <sup>2</sup>)", .Premium = If(D_increased <= 0, "Included", Me.MyLocation.D_LossOfUse_QuotedPremium)})
        '        End If
        '    Case Else
        '        opCovList.Add(New With {.Name = "D. Loss of Use", .Limit = "(see <sup>2</sup>)", .Premium = If(D_increased <= 0, "Included", Me.MyLocation.D_LossOfUse_QuotedPremium)})
        'End Select

        Dim E_Personal As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.PersonalLiabilityLimitId, Me.Quote.PersonalLiabilityLimitId)
        opCovList.Add(New With {.Name = "E. Personal Liability", .Limit = E_Personal, .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.Quote.PersonalLiabilityQuotedPremium) > 0, Me.Quote.PersonalLiabilityQuotedPremium, "Included")})

        Dim F_Medical As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.MedicalPaymentsLimitId, Me.Quote.MedicalPaymentsLimitid, Me.Quote.LobType)
        opCovList.Add(New With {.Name = "F. Medical Payment", .Limit = F_Medical, .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(Me.Quote.MedicalPaymentsQuotedPremium) > 0, Me.Quote.MedicalPaymentsQuotedPremium, "Included")})

        'Added 4/10/18 for HOM Upgrade MLW 'Updated 4/30/18 MLW
        If QQHelper.doUseNewVersionOfLOB(Quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
            If Me.Quote.MinimumPremiumAdjustment IsNot Nothing Then
                Dim minPremAdjPrem As Integer = Me.Quote.MinimumPremiumAdjustment
                'Dim mqp As String = Me.Quote.MinimumQuotedPremium
                'Dim tqp As String = Me.Quote.TotalQuotedPremium
                'Dim wp As String = Me.Quote.WrittenPremium
                If minPremAdjPrem <> 0 AndAlso Me.Quote.MinimumPremiumAdjustment <> "" Then
                    opCovList.Add(New With {.Name = "Minimum Premium Adjustment", .Limit = "N/A", .Premium = Me.Quote.MinimumPremiumAdjustment})
                End If
            End If
            ''Dim premiumFullTerm As String = "0"
            'If Quote.Locations IsNot Nothing Then
            '    Dim premiumFullTerm As String = Me.Quote.Locations(0).PremiumFullterm
            '    If Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24") AndAlso premiumFullTerm < "250" Then
            '        If Me.Quote.MinimumPremiumAdjustment <> "$0.00" AndAlso Me.Quote.MinimumPremiumAdjustment <> "0" AndAlso Me.Quote.MinimumPremiumAdjustment <> "" Then
            '            opCovList.Add(New With {.Name = "Minimum Premium Adjustment", .Limit = "N/A", .Premium = Me.Quote.MinimumPremiumAdjustment})
            '        End If
            '    ElseIf Quote.Locations(0).FormTypeId = "25" AndAlso premiumFullTerm < "50" Then
            '        If Me.Quote.MinimumPremiumAdjustment <> "$0.00" AndAlso Me.Quote.MinimumPremiumAdjustment <> "0" AndAlso Me.Quote.MinimumPremiumAdjustment <> "" Then
            '            opCovList.Add(New With {.Name = "Minimum Premium Adjustment", .Limit = "N/A", .Premium = Me.Quote.MinimumPremiumAdjustment})
            '        End If
            '    ElseIf Quote.Locations(0).FormTypeId = "26" AndAlso premiumFullTerm < "100" Then
            '        If Me.Quote.MinimumPremiumAdjustment <> "$0.00" AndAlso Me.Quote.MinimumPremiumAdjustment <> "0" AndAlso Me.Quote.MinimumPremiumAdjustment <> "" Then
            '            opCovList.Add(New With {.Name = "Minimum Premium Adjustment", .Limit = "N/A", .Premium = Me.Quote.MinimumPremiumAdjustment})
            '        End If
            '    End If
            'End If
        End If

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
        html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header qs_section_header_double_hieght"">")
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

    Private Sub PopulateSurcharges()
        Dim html As New StringBuilder()
        html.AppendLine("<table class=""hom_qa_table_shades"">")
        html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header qs_section_header_double_hieght"">")

        WriteCell(html, "Surcharge")
        WriteCell(html, "Premium/ Percent", "", "qs_Grid_cell_premium")

        html.AppendLine("</tr>")

        Dim opCovList = {(New With {.Name = "", .Premium = ""})}.Take(0).ToList()

        For Each item In IFM.VR.Common.Helpers.HOM.LossHistoryHelper_HOM.GetAllHOMSurcharges(Me.Quote)
            opCovList.Add(New With {.Name = item.Key, .Premium = item.Value})
        Next

        For Each c In opCovList
            html.AppendLine("<tr>")

            WriteCell(html, c.Name)
            WriteCell(html, c.Premium)

            html.AppendLine("</tr>")
        Next

        html.AppendLine("</table>")
        Me.tblSurcharges.Text = html.ToString()
    End Sub

    Private Function GetHOMLosses() As List(Of QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord)
        If Me.Quote IsNot Nothing Then
            If Me.Quote.LobId = "2" Then
                Dim lossList As New List(Of QuickQuote.CommonObjects.QuickQuoteLossHistoryRecord)()
                If Me.Quote.LossHistoryRecords IsNot Nothing Then
                    lossList.AddRange(Me.Quote.LossHistoryRecords)
                End If
                If Me.Quote.Applicants IsNot Nothing Then
                    For Each app In Me.Quote.Applicants
                        If app.LossHistoryRecords IsNot Nothing Then
                            lossList.AddRange(app.LossHistoryRecords)
                        End If
                    Next
                End If

                Return If(lossList.Any(), lossList, Nothing)
            End If
        End If
        Return Nothing
    End Function

    Private Sub PopulateIncludedCoverages()
        Dim html As New StringBuilder()

        html.AppendLine("<table class=""hom_qa_table_shades"">")
        html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")

        WriteCell(html, "Coverage")
        WriteCell(html, "Limit", "", "qs_Grid_cell_premium")
        WriteCell(html, "Premium", "", "qs_Grid_cell_premium")

        html.AppendLine("</tr>")

        For Each c In IFM.VR.Common.Helpers.HOM.HOMSummaryHelper.GetIncludedCoverageList(Me.Quote)
            html.AppendLine("<tr>")
            WriteCell(html, c.Name, If(c.IsSubItem, indentStyle, ""))
            WriteCell(html, c.Limit)
            WriteCell(html, c.Premium)
            html.AppendLine("</tr>")
        Next

        html.AppendLine("</table>")

        Me.tblIncludedCoverages.Text = html.ToString()
    End Sub

    Private Sub PopulateOptionalCoverages()
        Dim html As New StringBuilder()

        html.AppendLine("<table class=""hom_qa_table_shades"">")
        html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")

        WriteCell(html, "Coverage")
        WriteCell(html, "Limit", "", "qs_Grid_cell_premium")
        WriteCell(html, "Premium", "", "qs_Grid_cell_premium")

        html.AppendLine("</tr>")

        For Each c In IFM.VR.Common.Helpers.HOM.HOMSummaryHelper.GetOptionalCoverageList(Me.Quote)
            html.AppendLine("<tr>")
            WriteCell(html, c.Name, If(c.IsSubItem, indentStyle, ""))
            WriteCell(html, If(c.Limit = "0", "N/A", c.Limit)) ' Convert $0 to N/A
            WriteCell(html, c.Premium)
            html.AppendLine("</tr>")
            If FamilyCyberProtectionHelper.IsFamilyCyberProtectionAvailable(Quote) AndAlso Quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "26") Then
                If c.Name.Contains("Family Cyber Protection") Then
                    Dim indent As String = indentStyle & "&nbsp;&nbsp;"
                    ' Append each additional coverage with indentation
                    AppendCoverage(html, "Social Engineering Coverage", indent)
                    AppendCoverage(html, "Cyber Bullying Coverage", indent)
                    AppendCoverage(html, "Identity Theft Coverage", indent)
                    AppendCoverage(html, "Internet Clean Up Coverage", indent)
                    AppendCoverage(html, "Breach Costs Coverage", indent)
                    AppendCoverage(html, "Online Extortion Coverage & Systems Compromise Coverage (combined)", indent)
                End If
            End If
        Next
        html.AppendLine("</table>")

        Me.tblOptionalCoverages.Text = html.ToString()
    End Sub

    Private Sub AppendCoverage(ByRef html As StringBuilder, ByVal coverageName As String, ByVal indent As String)
        html.AppendLine("<tr>")
        WriteCell(html, coverageName, indent) ' Indented name
        If coverageName.Contains("Online Extortion Coverage") Then
            WriteCell(html, "2,500")
        Else
            WriteCell(html, "")
        End If
        WriteCell(html, "Included") 
        html.AppendLine("</tr>")
End Sub

    Private Sub PopulateInlandMarine()

        Dim opCovList = {(New With {.Name = "", .Description = "", .Deductible = "", .Limit = "", .Premium = ""})}.Take(0).ToList()

        Dim sortOrder As New Dictionary(Of Int32, Int32)
        Dim sortIndex As Int32 = 0
        sortOrder.Add(sortIndex.IncrementByOne(), QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Jewelry)
        sortOrder.Add(sortIndex.IncrementByOne(), QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.JewelryInVault)
        sortOrder.Add(sortIndex.IncrementByOne(), QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Bicycles)
        sortOrder.Add(sortIndex.IncrementByOne(), QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Cameras)
        sortOrder.Add(sortIndex.IncrementByOne(), QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Coins)
        sortOrder.Add(sortIndex.IncrementByOne(), QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Computer)
        sortOrder.Add(sortIndex.IncrementByOne(), QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.FarmMachineryScheduled)
        sortOrder.Add(sortIndex.IncrementByOne(), QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_With_Breakage)
        sortOrder.Add(sortIndex.IncrementByOne(), QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Fine_Arts_Without_Breakage)
        sortOrder.Add(sortIndex.IncrementByOne(), QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Furs)
        sortOrder.Add(sortIndex.IncrementByOne(), QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.GardenTractors)
        sortOrder.Add(sortIndex.IncrementByOne(), QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Golf)
        sortOrder.Add(sortIndex.IncrementByOne(), QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.GraveMarkers)
        sortOrder.Add(sortIndex.IncrementByOne(), QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Guns)
        sortOrder.Add(sortIndex.IncrementByOne(), QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.HearingAids)
        sortOrder.Add(sortIndex.IncrementByOne(), QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.MedicalItemsAndEquipment)
        sortOrder.Add(sortIndex.IncrementByOne(), QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Musical_Instruments_Non_Professional)
        sortOrder.Add(sortIndex.IncrementByOne(), QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Miscellaneous_Class_I)
        sortOrder.Add(sortIndex.IncrementByOne(), QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.InlandMarineSportsEquipment)
        sortOrder.Add(sortIndex.IncrementByOne(), QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.TelephonesCarOrMobile)
        sortOrder.Add(sortIndex.IncrementByOne(), QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.InlandMarineToolsAndEquipment)


        Dim newVals As New List(Of Int32)
        For Each v In System.Enum.GetValues(GetType(QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType))
            newVals.Add(CInt(v))
        Next

        Dim newEnumNotListed As List(Of Int32) = (From x In newVals Where sortOrder.Values.Contains(CInt(x)) = False Select x).ToList()

        For Each enumVal In newEnumNotListed
            sortOrder.Add(sortIndex.IncrementByOne(), CType(enumVal, QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType))
        Next

        Dim sortedInlandMarineItems = From i In Me.MyLocation.InlandMarines
                                      Join type In sortOrder On i.InlandMarineType Equals type.Value
                                      Order By type.Key
                                      Select i

        Dim inlandMarineTotal As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.InlandMarinesTotal_CoveragePremium)
        Dim rvWaterTotal As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.RvWatercraftsTotal_Premium)
        If Me.MyLocation IsNot Nothing Then
            If Me.MyLocation.InlandMarines IsNot Nothing Then
                For Each cov In sortedInlandMarineItems
                    Dim deductible As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteInlandMarine, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleLimitId, cov.DeductibleLimitId)
                    Dim inlandType As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteInlandMarine, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, cov.CoverageCodeId)

                    Select Case cov.InlandMarineType
                        Case QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Miscellaneous_Class_I
                            inlandType = "Silverware"
                        Case QuickQuote.CommonObjects.QuickQuoteInlandMarine.QuickQuoteInlandMarineType.MedicalItemsAndEquipment
                            inlandType = "Medical Items and Equipment"
                        Case Else
                            inlandType = inlandType.Replace("_", " ").Replace("Inland Marine", "")
                    End Select

                    'added 4/4/18 for HOM 2011 Upgrade MLW
                    If QQHelper.doUseNewVersionOfLOB(Quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                        If inlandType = " Golf" Then
                            inlandType = " Golf (Excl Golf Carts)"
                        End If
                    End If
                    opCovList.Add(New With {.Name = inlandType, .Description = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cov.Description.ToLower()), .Deductible = deductible, .Limit = cov.IncreasedLimit, .Premium = String.Format("{0:C2}", IFM.Common.InputValidation.InputHelpers.TryToGetDouble(cov.CoveragePremium))})
                Next
            End If
        End If

        Dim html As New StringBuilder()
        html.AppendLine("<div class=""hom_qs_Main_Sections"">")
        html.AppendLine("<span class=""qs_section_headers"">Inland Marine</span>")
        html.AppendLine("<div class=""hom_qs_Sub_Sections"">")
        html.AppendLine("<table class=""hom_qa_table_shades"">")
        html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")

        WriteCell(html, "Coverage")
        WriteCell(html, "Description")
        WriteCell(html, "Deductible", "", "qs_Grid_cell_premium")
        WriteCell(html, "Limit <sup>3</sup>", "", "qs_Grid_cell_premium")
        WriteCell(html, "Premium", "", "qs_Grid_cell_premium")
        html.AppendLine("</tr>")

        If opCovList.Any() Then
            For Each c In opCovList
                html.AppendLine("<tr>")
                WriteCell(html, c.Name)
                WriteCell(html, c.Description)
                WriteCell(html, c.Deductible)
                WriteCell(html, c.Limit)
                WriteCell(html, c.Premium)

                html.AppendLine("</tr>")
            Next
        End If

        Dim inlandMarineMin As Double = 25

        If (inlandMarineTotal + rvWaterTotal) <> 0 Then
            If (inlandMarineTotal + rvWaterTotal) < inlandMarineMin Then
                'min line
                html.AppendLine("<tr>")
                html.AppendLine("<td style=""height: 20px;"" colspan=""5""/>")
                html.AppendLine("</tr>")
                html.AppendLine("<tr>")
                html.AppendLine("<td colspan=""2"" style=""font-weight=bold;"">Additional Amount to meet minimum</td>")
                html.AppendLine("<td>N/A</td>")
                html.AppendLine("<td>N/A</td>")
                html.AppendLine("<td>" + String.Format("{0:C2}", inlandMarineMin - (inlandMarineTotal + rvWaterTotal)) + "</td>")
                html.AppendLine("</tr>")

                html.AppendLine("<tr class=""qs_Grid_Total_Row"">")
                html.AppendLine("<td colspan=""4"" style=""font-weight=bold;"">Total Inland Marine Premium</td>")
                html.AppendLine("<td>" + String.Format("{0:C2}", inlandMarineTotal + (inlandMarineMin - (inlandMarineTotal + rvWaterTotal))) + "</td>")
                html.AppendLine("</tr>")
            Else
                If inlandMarineTotal = 0.0 Then
                    ' no inland marine items
                    html.AppendLine("<tr>")
                    html.AppendLine("<td colspan=""5"">N/A</td>")
                    html.AppendLine("</tr>")
                Else
                    html.AppendLine("<tr class=""qs_Grid_Total_Row"">")
                    html.AppendLine("<td colspan=""4"" style=""font-weight=bold;"">Total Inland Marine Premium</td>")
                    html.AppendLine("<td>" + String.Format("{0:C2}", inlandMarineTotal) + "</td>")
                    html.AppendLine("</tr>")
                End If
            End If
        Else
            ' no inland marine items
            html.AppendLine("<tr>")
            html.AppendLine("<td colspan=""5"">N/A</td>")
            html.AppendLine("</tr>")
        End If

        html.AppendLine("</table>")
        html.AppendLine("</div>")
        html.AppendLine("</div>")

        Me.tblInlandMarine.Text = html.ToString()

    End Sub

    Private Sub PopulateRvWaterCraft()
        Dim CyberEffDate As DateTime = Nothing
        If System.Configuration.ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate") IsNot Nothing _
            AndAlso IsDate(System.Configuration.ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate")) Then
            CyberEffDate = CDate(System.Configuration.ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate"))
        Else
            CyberEffDate = CDate("9/1/2020")
        End If

        Dim opCovList = {(New With {.Type = "", .Description = "", .Coverage = "", .Deductible = "", .Limit = "", .Premium = ""})}.Take(0).ToList()

        Dim rvWaterCraftTotal As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.RvWatercraftsTotal_CoveragesPremium)
        If Me.MyLocation IsNot Nothing Then
            If Me.MyLocation.RvWatercrafts IsNot Nothing Then
                For Each cov In Me.MyLocation.RvWatercrafts
                    Dim Type As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraft, QuickQuoteHelperClass.QuickQuotePropertyName.RvWatercraftTypeId, cov.RvWatercraftTypeId)

                    Dim deductible As String = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteRvWatercraft, QuickQuoteHelperClass.QuickQuotePropertyName.PropertyDeductibleLimitId, cov.PropertyDeductibleLimitId, QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal)
                    Dim limit As String = String.Format("{0:N0}", IFM.Common.InputValidation.InputHelpers.TryToGetDouble(cov.CostNew))

                    Dim CoveragesText As New List(Of String)

                    If cov.HasLiability And cov.HasLiabilityOnly = False Then
                        CoveragesText.Add("PD,Liab")
                    End If

                    If QQHelper.doUseNewVersionOfLOB(Quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                        If cov.HasLiability = True AndAlso cov.HasLiabilityOnly Then
                            CoveragesText.Add("Liab")
                        End If
                    Else
                        '???? is this correct? shouldnt this be testing if both are true?
                        If cov.HasLiability = False And cov.HasLiabilityOnly Then
                            CoveragesText.Add("Liab")
                        End If
                    End If

                    If cov.HasLiability = False And cov.HasLiabilityOnly = False Then
                        CoveragesText.Add("PD")
                    End If

                    If cov.UninsuredMotoristBodilyInjuryLimitId <> "" And cov.UninsuredMotoristBodilyInjuryLimitId <> "0" Then
                        CoveragesText.Add("UWBI")
                    End If

                    Dim PremiumSum As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(cov.CoveragesPremium) ' 0.0
                    'For Each c In cov.Coverages
                    '    PremiumSum += IFM.Common.InputValidation.InputHelpers.TryToGetDouble(c.WrittenPremium)
                    'Next

                    Dim desc As String = cov.Year
                    Select Case cov.RvWatercraftTypeId
                        Case "3" '3 = Boat Motor Only
                            desc = String.Format("{0}, {1} HP", If(cov.RvWatercraftMotors IsNot Nothing AndAlso cov.RvWatercraftMotors.Any(), cov.RvWatercraftMotors(0).Year, ""), cov.HorsepowerCC)
                            ' Boat Motor limit is different from other limits.
                            limit = String.Format("{0:N0}", IFM.Common.InputValidation.InputHelpers.TryToGetDouble(cov.RvWatercraftMotors.FirstOrDefault.CostNew))
                        Case "5" '5 = Accessories
                            desc = If(cov.Description.Length > 40, cov.Description.Substring(0, 40), cov.Description)

                    End Select

                    opCovList.Add(New With {.Type = Type, .Description = desc, .Coverage = IFM.Common.InputValidation.InputHelpers.ListToCSV(CoveragesText), .Deductible = deductible, .limit = limit, .Premium = String.Format("{0:C2}", PremiumSum)})
                    'rvWaterCraftTotal += IFM.Common.InputValidation.InputHelpers.TryToGetDouble(PremiumSum)
                Next
            End If
        End If

        Dim html As New StringBuilder()
        html.AppendLine("<div class=""hom_qs_Main_Sections"">")
        html.AppendLine("<span class=""qs_section_headers"">RV and Watercraft</span>")
        html.AppendLine("<div class=""hom_qs_Sub_Sections"">")
        html.AppendLine("<table class=""hom_qa_table_shades"">")
        html.AppendLine("<tr class=""qs_hom_section_grid_headers ui-widget-header"">")

        WriteCell(html, "Type")
        WriteCell(html, "Description")
        WriteCell(html, "Coverage", "", "qs_Grid_cell_premium")
        WriteCell(html, "Deductible", "", "qs_Grid_cell_premium")
        WriteCell(html, "Limit <sup>3</sup>", "", "qs_Grid_cell_premium")
        WriteCell(html, "Premium", "", "qs_Grid_cell_premium")

        html.AppendLine("</tr>")
        If opCovList.Any() Then
            For Each c In opCovList.OrderBy(Function(x)
                                                Return x.Type
                                            End Function)
                html.AppendLine("<tr>")

                WriteCell(html, c.Type)
                WriteCell(html, c.Description)
                WriteCell(html, c.Coverage)
                WriteCell(html, c.Deductible)
                WriteCell(html, c.Limit)
                WriteCell(html, c.Premium)

                html.AppendLine("</tr>")
            Next

            html.AppendLine("<tr class=""qs_Grid_Total_Row"">")
            html.AppendLine("<td colspan=""5"" style=""font-weight=bold;"">Total RV and Watercraft Premium</td>")
            html.AppendLine("<td>" + String.Format("{0:C2}", rvWaterCraftTotal) + "</td>")
            html.AppendLine("</tr>")
        Else
            'no items
            html.AppendLine("<tr>")
            html.AppendLine("<td colspan=""6"">N/A</td>")
            html.AppendLine("</tr>")
        End If
        html.AppendLine("</table>")
        html.AppendLine("<br/><sup>1</sup>Limit values are rounded to the next highest $1000.")
        html.AppendLine("<br/><sup>2</sup>Actual Loss Sustained within 12 months of a covered loss")
        html.AppendLine("<br/><sup>3</sup>Limit values are rounded to the next highest $100.")
        html.AppendLine("<br/><sup>4</sup>Coverage E and F Limits are extended to this endorsement.")
        html.AppendLine("<br/><sup>5</sup>Coverage E limit is extended to this endorsement.")
        If QQHelper.doUseNewVersionOfLOB(Quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
            html.AppendLine("<br/><sup>6</sup>10% of Coverage C")
        End If
        If CDate(Quote.EffectiveDate) >= CyberEffDate Then
            html.AppendLine("<br/><sup>7</sup>Deductible for this coverage is always $500.")
        End If
        html.AppendLine("</div>")
        html.AppendLine("</div>")
        Me.tblRvWaterCraft.Text = html.ToString()
    End Sub

    Public Overrides Function Save() As Boolean
        ' no saving needed
        Return True
    End Function

    Protected Sub lnkPrint_Click(sender As Object, e As EventArgs) Handles lnkPrint.Click
        'Response.Redirect(String.Format("~/Reports/HOM/PFQuoteSummary_HOM.aspx?quoteid={0}&summarytype={1}", Request.QueryString("QuoteId").ToString, Session("SumType")))
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
            Response.Redirect(String.Format("~/Reports/HOM/PFQuoteSummary_HOM.aspx?{0}&summarytype={1}", quoteOrPolicyInfo, sumType))
        End If
    End Sub

    'added 2/18/2020
    Public Sub CheckForReRateAfterEffDateChange(Optional ByVal qqTranType As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.None, Optional ByVal newEffectiveDate As String = "", Optional ByVal oldEffectiveDate As String = "")
        Me.ctlQuoteSummaryActions.CheckForReRateAfterEffDateChange(qqTranType:=qqTranType, newEffectiveDate:=newEffectiveDate, oldEffectiveDate:=oldEffectiveDate)
    End Sub

End Class