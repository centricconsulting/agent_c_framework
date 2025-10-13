Imports System.IO
Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.CPP
Imports IFM.VR.Common.Helpers.CPR
Imports IFM.VR.Common.Helpers.MultiState.General
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects

Public Class ctl_CPP_PFSummary
    Inherits VRControlBase

    Dim indent As String = "&nbsp;&nbsp;"
    Dim dblindent As String = "&nbsp;&nbsp;&nbsp;&nbsp;"
    'Added 12/17/18 for multi state bug 30351 MLW
    Private NumFormatWithCents As String = "$###,###,###.00"
    Private NumFormat As String = "###,###,###"

    Private ReadOnly Property UnscheduledToolsPremium As String
        Get
            Dim sm1 As Decimal = 0
            Dim sm2 As Decimal = 0

            'Updated 12/21/18 for multi state MLW (merge down from trunk on another project, needs updated to multi state format)
            'If (Not Quote.SmallToolsQuotedPremium.IsNullEmptyorWhitespace) AndAlso (IsNumeric(Quote.SmallToolsQuotedPremium)) Then sm1 = CDec(Quote.SmallToolsQuotedPremium)
            'If (Not Quote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceQuotedPremium.IsNullEmptyorWhitespace) AndAlso (IsNumeric(Quote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceQuotedPremium)) Then sm2 = CDec(Quote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceQuotedPremium)

            Dim premSmallTools As String = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.SmallToolsQuotedPremium)
            Dim premContractorsSmallTools As String = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceQuotedPremium)

            If (Not premSmallTools.IsNullEmptyorWhitespace) AndAlso (IsNumeric(premSmallTools)) Then sm1 = CDec(premSmallTools)
            If (Not premContractorsSmallTools.IsNullEmptyorWhitespace) AndAlso (IsNumeric(premContractorsSmallTools)) Then sm2 = CDec(premContractorsSmallTools)

            Return Format(sm1 + sm2, "c")
        End Get
    End Property

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion("divCPPQuoteSummary", HiddenField1, "0")
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
                    'Added 12/20/2021 for CPP Endorsements Task 67649 MLW
                    Me.ctlEndorsementOrChangeHeader.Visible = True
                    Me.ctlEndorsementOrChangeHeader.Quote = Quote
                    Me.EndorsementPrintSection.Visible = True
                    Me.quoteSummaryHeader.Visible = False
                    Me.quoteSummarySection.Visible = False

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
                    'Added 12/20/2021 for CPP Endorsements Task 67649 MLW
                    Me.ctlEndorsementOrChangeHeader.Visible = False
                    Me.EndorsementPrintSection.Visible = False
                    Me.quoteSummaryHeader.Visible = True
                    Me.quoteSummarySection.Visible = True

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

            ' Property, Liability, IM, and Crime Premiums - MGB 3/12/18
            'Updated 12/20/18 for multi state bug 30510 MLW
            'Me.lblPropertyQuotedPremium.Text = String.Format("{0:C2}", Me.Quote.CPP_CPR_PackagePart_QuotedPremium)
            'Me.lblGeneralLiabilityQuotedPremium.Text = String.Format("{0:C2}", Me.Quote.CPP_GL_PackagePart_QuotedPremium)
            'Me.lblInlandMarineQuotedPremium.Text = String.Format("{0:C2}", Me.Quote.CPP_CIM_PackagePart_QuotedPremium)
            'Me.lblCrimeQuotedPremium.Text = String.Format("{0:C2}", Me.Quote.CPP_CRM_PackagePart_QuotedPremium)
            Me.lblPropertyQuotedPremium.Text = String.Format("{0:C2}", QQHelper.GetSumForPropertyValues(SubQuotes, Function() Me.Quote.CPP_CPR_PackagePart_QuotedPremium))
            Me.lblGeneralLiabilityQuotedPremium.Text = String.Format("{0:C2}", QQHelper.GetSumForPropertyValues(SubQuotes, Function() Me.Quote.CPP_GL_PackagePart_QuotedPremium))
            Me.lblInlandMarineQuotedPremium.Text = String.Format("{0:C2}", QQHelper.GetSumForPropertyValues(SubQuotes, Function() Me.Quote.CPP_CIM_PackagePart_QuotedPremium))
            Me.lblCrimeQuotedPremium.Text = String.Format("{0:C2}", QQHelper.GetSumForPropertyValues(SubQuotes, Function() Me.Quote.CPP_CRM_PackagePart_QuotedPremium))

            Me.lblFullPremium.Text = String.Format("{0:C2}", Me.Quote.TotalQuotedPremium)

            PopulatePolicyLevelCoverages()
            ctlPolicyDiscounts.Populate()
            PopulateLocationInformation()
            PopulateClassifications()

            ' InlandMarine sections
            'Updated 12/18/18 for multi state Bug 30310 MLW
            'If Quote.CPP_Has_InlandMarine_PackagePart = True Then
            If GoverningStateQuote.CPP_Has_InlandMarine_PackagePart = True Then
                PopulateInlandMarine()
            Else
                tblInlandMarine.Text = ""
            End If

            ' Crime sections
            'Updated 12/14/18 for multi state Bug 30351 MLW
            'If Quote.CPP_Has_Crime_PackagePart = True Then
            If GoverningStateQuote.CPP_Has_Crime_PackagePart = True Then
                PopulateCrime()
            Else
                tblCrime.Text = ""
            End If

            ' App-side sections
            If IsOnAppPage AndAlso Me.Quote IsNot Nothing AndAlso Me.GoverningStateQuote IsNot Nothing AndAlso Me.GoverningStateQuote.LossHistoryRecords IsNot Nothing AndAlso Me.GoverningStateQuote.LossHistoryRecords.Count > 0 Then
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
        Dim indent As String = "&nbsp;&nbsp;&nbsp;&nbsp;"
        'Added 11/20/18 for multi state MLW
        Dim IndianaQuote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing
        Dim IllinoisQuote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing
        Dim OhioQuote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing

        If Quote IsNot Nothing AndAlso SubQuoteFirst IsNot Nothing Then
            'Intialize - Hide all the rows that are optional
            rowPropEnhEndo.Visible = False
            rowPropPlusEnhEndo.Visible = False
            rowGLEnhEndo.Visible = False
            rowGlPlusEnhancement.Visible = False
            rowContEnhEndo.Visible = False
            rowManuEnhEndo.Visible = False
            rowFoodManuEndo.Visible = False
            'rowGenAgg.Visible = False                  ' Always shown
            'rowProdCompOps.Visible = False             ' Always shown
            'rowPersAdvInj.Visible = False              ' Always shown
            'rowOccLiabLimit.Visible = False            ' Always shown
            'rowDmgToPremRentedByYou.Visible = False    ' Always shown
            'rowMedicalExpenses.Visible = False         ' Always shown
            rowGLEnhEndo.Visible = False
            rowBlanketWaiverOfSubro.Visible = False
            rowBlanketWaiverOfSubroCompletedOps.Visible = False
            rowGLDeductible.Visible = False
            rowGLDeductibleType.Visible = False
            rowGLDeductibleAmount.Visible = False
            rowGLDeductibleBasis.Visible = False
            rowAdditionalInsureds.Visible = False
            rowCondoDirectorsAndOfficers.Visible = False
            rowEBL.Visible = False
            rowEPLI.Visible = False
            rowHiredAuto.Visible = False
            rowNonOwnedAuto.Visible = False
            'Updated 11/20/18 for multi state MLW
            pnlLiquorLiability_IN.Visible = False
            pnlLiquorLiability_IL.Visible = False
            pnlLiquorLiability_OH.Visible = False
            rowBlanketRating.Visible = False
            rowMinPremAdj.Visible = False
            'Added 11/20/18 for multi state MLW
            pnlContractorsHomeRepairAndRemodeling.Visible = False

            '*** GENERAL INFORMATION
            ' Property Enhancement
            If SubQuoteFirst.Has_PackageCPR_EnhancementEndorsement Then
                rowPropEnhEndo.Visible = True
                tdPropertyEnhancementEndorsementPremium.InnerText = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.PackageCPR_EnhancementEndorsementQuotedPremium)
            End If

            'Property Plus Enhancement
            If SubQuoteFirst.Has_PackageCPR_PlusEnhancementEndorsement Then
                rowPropPlusEnhEndo.Visible = True
                tdPropertyPlusEnhancementEndorsementPremium.InnerText = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.PackageCPR_PlusEnhancementEndorsementQuotedPremium)
            End If

            ' GL Enhancement
            If SubQuoteFirst.Has_PackageGL_EnhancementEndorsement Then
                rowGLEnhEndo.Visible = True
                tdGeneralLiabilityEnhancementEndorsementPremium.InnerText = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.PackageGL_EnhancementEndorsementQuotedPremium)
            End If

            'GL PLUS Enhancement
            If SubQuoteFirst.Has_PackageGL_PlusEnhancementEndorsement Then
                rowGlPlusEnhancement.Visible = True
                GlPlusEnhancementQuotedPremium.InnerText = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.PackageGL_PlusEnhancementEndorsementQuotedPremium)
            End If

            ' Contractors Enhancement
            If SubQuoteFirst.HasContractorsEnhancement Then
                rowContEnhEndo.Visible = True
                tdContractorsEnhancementEndorsementPremium.InnerText = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.ContractorsEnhancementQuotedPremium)
            End If

            ' Manufacturers Enhancement
            If SubQuoteFirst.HasManufacturersEnhancement Then
                rowManuEnhEndo.Visible = True
                tdManufacturerEnhancementEndorsementPremium.InnerText = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.ManufacturersEnhancementQuotedPremium)
            End If

            ' Food Manufacturers Enhancement Package
            If SubQuoteFirst.HasFoodManufacturersEnhancement Then
                rowFoodManuEndo.Visible = True
                Dim foodPrem As String = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.FoodManufacturersEnhancementQuotedPremium)
                If IsNumeric(foodPrem) Then
                    tdFoodManufacturersEnhancementEndorsementPackagePremium.InnerText = foodPrem
                Else
                    tdFoodManufacturersEnhancementEndorsementPackagePremium.InnerText = ""
                End If
            End If

            ' Per Kathy W. (bug 30551) - these first 6 premiums should be blank - CAH 01/08/2019
            ' General Aggregate
            tdGeneralAggregateLimit.InnerText = SubQuoteFirst.GeneralAggregateLimit
            'tdGeneralAggregatePremium.InnerText = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.GeneralAggregateQuotedPremium)

            ' Products/Completed Ops Aggregate
            tdProductsCompletedOpsAggregateLimit.InnerText = SubQuoteFirst.ProductsCompletedOperationsAggregateLimit
            'tdProductsCompletedOpsAggregatePremium.InnerText = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.ProductsCompletedOperationsAggregateQuotedPremium)

            ' Personal and Advertising Injury
            tdPersonalAndAdvertisingInjuryLimit.InnerText = SubQuoteFirst.PersonalAndAdvertisingInjuryLimit
            'tdPersonalAndAdvertisingInjuryPremium.InnerText = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.PersonalAndAdvertisingInjuryQuotedPremium)

            ' Occurrence Liability Limit
            tdOccurrenceLiabilityLimitLimit.InnerText = SubQuoteFirst.OccurrenceLiabilityLimit
            'tdOccurrenceLiabilityLimitPremium.InnerText = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.OccurrencyLiabilityQuotedPremium)

            ' Damage to premises rented by you
            tdDmgToPremisesRentedByYouLimit.InnerText = SubQuoteFirst.DamageToPremisesRentedLimit
            'tdDmgToPremisesRentedByYouPremium.InnerText = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.DamageToPremisesRentedQuotedPremium)

            ' Medical Expenses
            tdMedicalExpensesLimit.InnerText = SubQuoteFirst.MedicalExpensesLimit
            'tdMedicalExpensesPremium.InnerText = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.MedicalExpensesQuotedPremium)

            ' GL Enhancement
            If SubQuoteFirst.Has_PackageGL_EnhancementEndorsement Then
                tdGeneralLiabilityEnhancementEndorsementPremium.InnerText = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.PackageGL_EnhancementEndorsementQuotedPremium)
            End If
            ' GL Enhancement
            If SubQuoteFirst.Has_PackageGL_EnhancementEndorsement Then
                tdGeneralLiabilityEnhancementEndorsementPremium.InnerText = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.PackageGL_EnhancementEndorsementQuotedPremium)
            End If

            ' Blanket Waiver of Subro
            If SubQuoteFirst.BlanketWaiverOfSubrogationQuotedPremium <> "" AndAlso SubQuoteFirst.BlanketWaiverOfSubrogationQuotedPremium <> "0" Then
                Select Case SubQuoteFirst.BlanketWaiverOfSubrogation
                    Case "1"
                        ' Blanket Waiver
                        rowBlanketWaiverOfSubro.Visible = True
                        tdBlanketWaiverOfSubroPremium.InnerText = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.BlanketWaiverOfSubrogationQuotedPremium)
                        Exit Select
                    Case "2"
                        ' Blanket Waiver w/completed ops
                        rowBlanketWaiverOfSubroCompletedOps.Visible = True
                        tdBlanketWaiverSubroCompletedOpsPremium.InnerText = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.BlanketWaiverOfSubrogationQuotedPremium)
                        Exit Select
                End Select
            End If

            ' General Liability Deductible - Type, Amount, Basis
            If SubQuoteFirst.GL_PremisesAndProducts_DeductibleCategoryTypeId IsNot Nothing AndAlso SubQuoteFirst.GL_PremisesAndProducts_DeductibleCategoryTypeId <> "" Then
                Dim typ As String = ""
                Dim amt As String = ""
                Dim basis As String = ""

                rowGLDeductible.Visible = True
                rowGLDeductibleType.Visible = True
                rowGLDeductibleAmount.Visible = True
                rowGLDeductibleBasis.Visible = True

                ' Get the type
                Select Case SubQuoteFirst.GL_PremisesAndProducts_DeductibleCategoryTypeId
                    Case "5"
                        typ = "Bodily Injury"
                        Exit Select
                    Case "6"
                        typ = "Property Damage"
                        Exit Select
                    Case "7"
                        typ = "Property Damage & Bodily Injury"
                        Exit Select
                End Select

                ' Get the Amount
                Select Case SubQuoteFirst.GL_PremisesAndProducts_DeductibleId
                    Case "4"
                        amt = "$250"
                        Exit Select
                    Case "8"
                        amt = "$500"
                        Exit Select
                    Case "27"
                        amt = "$750"
                        Exit Select
                    Case "9"
                        amt = "$1,000"
                        Exit Select
                    Case "28"
                        amt = "$2,000"
                        Exit Select
                    Case "29"
                        amt = "$3,000"
                        Exit Select
                    Case "30"
                        amt = "$4,000"
                        Exit Select
                    Case "16"
                        amt = "$5,000"
                        Exit Select
                    Case "17"
                        amt = "$10,000"
                        Exit Select
                    Case "31"
                        amt = "$15,000"
                        Exit Select
                    Case "18"
                        amt = "$20,000"
                        Exit Select
                    Case "19"
                        amt = "$25,000"
                        Exit Select
                    Case "20"
                        amt = "$50,000"
                        Exit Select
                    Case "21"
                        amt = "$75,000"
                        Exit Select
                End Select

                ' Get the Basis
                Select Case SubQuoteFirst.GL_PremisesAndProducts_DeductiblePerTypeId
                    Case "1"
                        basis = "Per Occurrence"
                        Exit Select
                    Case "2"
                        basis = "Per Claim"
                        Exit Select
                End Select

                tdGLDeductibleType.InnerHtml = indent & "Type: " & typ
                tdGLDeductibleAmount.InnerHtml = indent & "Amount: " & amt
                tdGLDeductibleBasis.InnerHtml = indent & "Basis: " & basis
            End If

            ' Additional Insureds
            'Updated 12/21/18 for multi state Bug 30552 MLW - to match CGL's logic
            'If SubQuoteFirst.AdditionalInsuredsQuotedPremium <> "" Then
            If SubQuoteFirst.AdditionalInsuredsCount > 0 Then
                rowAdditionalInsureds.Visible = True
                tdAdditionalInsuredsPremium.InnerHtml = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.AdditionalInsuredsQuotedPremium)
            End If

            ' Condo D&O
            If SubQuoteFirst.HasCondoDandO Then
                rowCondoDirectorsAndOfficers.Visible = True
                tdCondoCirectorsAndOfficersPremium.InnerHtml = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.CondoDandOPremium)
            End If

            ' Employee Benefits Liability
            'Updated 12/21/18 for multi state Bug 30552 MLW - to match CGL's logic
            'If SubQuoteFirst.EmployeeBenefitsLiabilityQuotedPremium <> "" Then
            If IsNumeric(SubQuoteFirst.EmployeeBenefitsLiabilityText) = True Then
                rowEBL.Visible = True
                tdEBLInfo.InnerHtml = "Employee Benefits Liability: " & SubQuoteFirst.EmployeeBenefitsLiabilityText & " employees"
                tdEBLPremium.InnerHtml = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.EmployeeBenefitsLiabilityQuotedPremium)
            End If

            ' EPLI
            If SubQuoteFirst.HasEPLI AndAlso SubQuoteFirst.EPLICoverageTypeID = "22" Then
                rowEPLI.Visible = True
                tdEPLIPremium.InnerHtml = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.EPLIPremium)
            End If

            ' CLI
            If SubQuoteFirst.CyberLiability And SubQuoteFirst.CyberLiabilityTypeId = "23" Then
                If IFM.VR.Common.Helpers.CPP.CyberCoverageHelper.IsCyberCoverageAvailable(Quote) Then
                    tdCLICovName.InnerHtml = "Cyber Coverage"
                Else
                    tdCLICovName.InnerHtml = "Cyber Liability"
                End If
                rowCLI.Visible = True
                tdCLIPremium.InnerHtml = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.CyberLiabilityPremium)
            End If

            ' Hired Auto
            If SubQuoteFirst.HasHiredAuto Then
                rowHiredAuto.Visible = True
                tdHiredAutoPremium.InnerHtml = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.HiredAutoQuotedPremium)
            End If

            ' Non-Owned Auto
            If SubQuoteFirst.HasNonOwnedAuto Then
                rowNonOwnedAuto.Visible = True
                tdNonOwnedAutoPremium.InnerHtml = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.NonOwnedAutoQuotedPremium)
            End If

            'Updated 11/20/18 for multi state MLW
            ' Indiana Liquor Liability
            If SubQuotesContainsState("IN") Then
                IndianaQuote = SubQuoteForState("IN")
                If IndianaQuote.LiquorLiabilityOccurrenceLimitId <> "" OrElse IndianaQuote.LiquorLiabilityClassificationId <> "" Then
                    pnlLiquorLiability_IN.Visible = True
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
                    LiquorLiabilityLSales_OH.InnerText = QQHelper.SetLiquorSales(OhioQuote)
                End If
            End If
            ' Illinois Liquor Liability
            If IsMultistateCapableEffectiveDate(Quote.EffectiveDate) AndAlso SubQuotesContainsState("IL") Then
                IllinoisQuote = SubQuoteForState("IL")
                If IllinoisQuote.LiquorLiabilityOccurrenceLimitId <> "" OrElse IllinoisQuote.LiquorLiabilityClassificationId <> "" Then
                    pnlLiquorLiability_IL.Visible = True
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
            'Added 11/20/18 for multi state MLW
            ' IL Contractors Home Repair and Remodeling  (new coverage for multistate)
            If IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then
                Dim ILQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois)
                If ILQuote IsNot Nothing AndAlso ILQuote.HasIllinoisContractorsHomeRepairAndRemodeling Then
                    pnlContractorsHomeRepairAndRemodeling.Visible = True
                    tdContractorsHomeRepairAndRemodelingPremium.InnerText = ILQuote.IllinoisContractorsHomeRepairAndRemodelingQuotedPremium
                End If
            End If
            ' Blanket Rating
            'Updated 12/18/18 for multi state bug 30442 MLW
            'If SubQuoteFirst.BlanketRatingOption IsNot Nothing AndAlso SubQuoteFirst.BlanketRatingOption <> String.Empty Then
            If Quote.BlanketRatingOption IsNot Nothing AndAlso Quote.BlanketRatingOption <> String.Empty Then
                Dim btyp As String = "N/A"
                Dim bprem As String = ""
                rowBlanketRating.Visible = True
                If SubQuoteFirst.HasBlanketBuildingAndContents = True Then
                    btyp = "Combined Building And Personal Property"
                    bprem = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.BlanketBuildingAndContentsQuotedPremium)
                ElseIf SubQuoteFirst.HasBlanketBuilding = True Then
                    btyp = "Building Only"
                    bprem = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.BlanketBuildingQuotedPremium)
                ElseIf SubQuoteFirst.HasBlanketContents = True Then
                    btyp = "Personal Property Only"
                    bprem = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.BlanketContentsQuotedPremium)
                End If

                tdBlanketRatingName.InnerHtml = "Blanket Rating: " & btyp
                tdBlanketRatingPremium.InnerHtml = bprem
            End If

            ' Ohio Stop Gap

            If IFM.VR.Common.Helpers.MultiState.General.IsOhioEffective(Quote) Then
                Dim gsQuote As QuickQuoteObject = Me.GoverningStateQuote()
                If gsQuote.StopGapLimitId <> "" Then
                    rowStopGapOH.Visible = True
                    If gsQuote.StopGapQuotedPremium.IsNumeric Then
                        tdStopGapPremium.InnerHtml = Format(CDec(gsQuote.StopGapQuotedPremium), NumFormatWithCents)
                    End If
                End If
            End If

            ' Minimum Premium Adjustment
            If IFM.Common.InputValidation.CommonValidations.IsPositiveNumber(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.MinimumPremiumAdjustment)) = True Then
                rowMinPremAdj.Visible = True
                tdMinimumPremiumAdjustmentPremium.InnerText = QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.MinimumPremiumAdjustment)
            End If

            Exit Sub
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
                PopulateBuildings(html, LOC)
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
            If LOC.EquipmentBreakdownDeductible.Trim <> "" Then
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
                ' Equipment Breakdown

                html.AppendLine("<tr class=""qs_basic_info_labels_cell qs_grid_4_columns"">")
                WriteCell(html, "Equipment Breakdown")
                WriteCell(html, LOC.EquipmentBreakdownDeductibleQuotedPremium, "", "qs_rightJustify qs_padRight")
                html.AppendLine("</tr>")


                ' Spacer line
                'html.AppendLine("<tr><td colspan=4>&nbsp;</td></tr>")

                ' Close the table and the divs
                html.AppendLine("</table>")
                html.AppendLine("</div>")
                html.AppendLine("</div>")
            End If
            Exit Sub
        Catch ex As Exception
            Exit Sub
        End Try
    End Sub

    Private Sub PopulateBuildings(ByRef html As StringBuilder, ByVal LOC As QuickQuote.CommonObjects.QuickQuoteLocation)
        Try
            Dim ShowBlanketHelp = False
            ' WindHail is set at the location level and applies to all buildings
            Dim windHailPercent = QQHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteBuilding, QuickQuotePropertyName.WindHailDeductibleLimitId, LOC.WindHailDeductibleLimitId)

            For Each BLD As QuickQuote.CommonObjects.QuickQuoteBuilding In LOC.Buildings

                Dim has_CPP_building_buildingCov As Boolean = False 'added 11/27/2012
                If BLD.Limit <> "" OrElse (BLD.ValuationId <> "" AndAlso IsNumeric(BLD.ValuationId) = True) OrElse (BLD.ClassificationCode IsNot Nothing AndAlso BLD.ClassificationCode.ClassCode <> "") OrElse BLD.EarthquakeApplies = True OrElse (BLD.CauseOfLossTypeId <> "" AndAlso IsNumeric(BLD.CauseOfLossTypeId) = True AndAlso BLD.CauseOfLossTypeId > 0) OrElse (BLD.CoinsuranceTypeId <> "" AndAlso IsNumeric(BLD.CoinsuranceTypeId) = True) OrElse (BLD.DeductibleId <> "" AndAlso IsNumeric(BLD.DeductibleId) = True) OrElse (BLD.RatingTypeId <> "" AndAlso IsNumeric(BLD.RatingTypeId) = True) OrElse (BLD.InflationGuardTypeId <> "" AndAlso IsNumeric(BLD.InflationGuardTypeId) = True) Then
                    has_CPP_building_buildingCov = True
                End If

                Dim has_CPP_building_busIncomeCov As Boolean = False 'added 11/27/2012
                If BLD.BusinessIncomeCov_Limit <> "" AndAlso BLD.BusinessIncomeCov_Limit > 0 OrElse (BLD.BusinessIncomeCov_CoinsuranceTypeId <> "" AndAlso IsNumeric(BLD.BusinessIncomeCov_CoinsuranceTypeId) = True) AndAlso BLD.BusinessIncomeCov_CoinsuranceTypeId > 0 OrElse (BLD.BusinessIncomeCov_MonthlyPeriodTypeId <> "" AndAlso IsNumeric(BLD.BusinessIncomeCov_MonthlyPeriodTypeId) = True) AndAlso BLD.BusinessIncomeCov_MonthlyPeriodTypeId > 0 OrElse (BLD.BusinessIncomeCov_BusinessIncomeTypeId <> "" AndAlso IsNumeric(BLD.BusinessIncomeCov_BusinessIncomeTypeId) = True) AndAlso BLD.BusinessIncomeCov_BusinessIncomeTypeId > 0 OrElse (BLD.BusinessIncomeCov_RiskTypeId <> "" AndAlso IsNumeric(BLD.BusinessIncomeCov_RiskTypeId) = True) AndAlso BLD.BusinessIncomeCov_RiskTypeId > 0 OrElse (BLD.BusinessIncomeCov_RatingTypeId <> "" AndAlso IsNumeric(BLD.BusinessIncomeCov_RatingTypeId) = True) AndAlso BLD.BusinessIncomeCov_RatingTypeId > 0 OrElse (BLD.BusinessIncomeCov_CauseOfLossTypeId <> "" AndAlso IsNumeric(BLD.BusinessIncomeCov_CauseOfLossTypeId) = True) AndAlso BLD.BusinessIncomeCov_CauseOfLossTypeId > 0 OrElse (BLD.BusinessIncomeCov_ClassificationCode IsNot Nothing AndAlso BLD.BusinessIncomeCov_ClassificationCode.ClassCode <> "") OrElse BLD.BusinessIncomeCov_EarthquakeApplies = True Then
                    has_CPP_building_busIncomeCov = True
                End If

                Dim has_CPP_building_persPropCov As Boolean = False 'added 11/27/2012
                'If BLD.PersPropCov_PersonalPropertyLimit <> "" OrElse (BLD.PersPropCov_PropertyTypeId <> "" AndAlso IsNumeric(BLD.PersPropCov_PropertyTypeId) = True) OrElse (BLD.PersPropCov_RiskTypeId <> "" AndAlso IsNumeric(BLD.PersPropCov_RiskTypeId) = True) OrElse BLD.PersPropCov_EarthquakeApplies = True OrElse (BLD.PersPropCov_RatingTypeId <> "" AndAlso IsNumeric(BLD.PersPropCov_RatingTypeId) = True) OrElse (BLD.PersPropCov_CauseOfLossTypeId <> "" AndAlso IsNumeric(BLD.PersPropCov_CauseOfLossTypeId) = True) OrElse (BLD.PersPropCov_DeductibleId <> "" AndAlso IsNumeric(BLD.PersPropCov_DeductibleId) = True) OrElse (BLD.PersPropCov_CoinsuranceTypeId <> "" AndAlso IsNumeric(BLD.PersPropCov_CoinsuranceTypeId) = True) OrElse (BLD.PersPropCov_ValuationId <> "" AndAlso IsNumeric(BLD.PersPropCov_ValuationId) = True) OrElse (BLD.PersPropCov_ClassificationCode IsNot Nothing AndAlso BLD.PersPropCov_ClassificationCode.ClassCode <> "") OrElse BLD.PersPropOfOthers_PersonalPropertyLimit <> "" OrElse (BLD.PersPropOfOthers_RiskTypeId <> "" AndAlso IsNumeric(BLD.PersPropOfOthers_RiskTypeId) = True) OrElse BLD.PersPropOfOthers_EarthquakeApplies = True OrElse (BLD.PersPropOfOthers_RatingTypeId <> "" AndAlso IsNumeric(BLD.PersPropOfOthers_RatingTypeId) = True) OrElse (BLD.PersPropOfOthers_CauseOfLossTypeId <> "" AndAlso IsNumeric(BLD.PersPropOfOthers_CauseOfLossTypeId) = True) OrElse (BLD.PersPropOfOthers_DeductibleId <> "" AndAlso IsNumeric(BLD.PersPropOfOthers_DeductibleId) = True) OrElse (BLD.PersPropOfOthers_CoinsuranceTypeId <> "" AndAlso IsNumeric(BLD.PersPropOfOthers_CoinsuranceTypeId) = True) OrElse (BLD.PersPropOfOthers_ValuationId <> "" AndAlso IsNumeric(BLD.PersPropOfOthers_ValuationId) = True) OrElse (BLD.PersPropOfOthers_ClassificationCode IsNot Nothing AndAlso BLD.PersPropOfOthers_ClassificationCode.ClassCode <> "") Then
                ' Not sure why we were checking the Pers Prop of Others fields
                If BLD.PersPropCov_PersonalPropertyLimit <> "" OrElse (BLD.PersPropCov_PropertyTypeId <> "" AndAlso IsNumeric(BLD.PersPropCov_PropertyTypeId) = True) OrElse (BLD.PersPropCov_RiskTypeId <> "" AndAlso IsNumeric(BLD.PersPropCov_RiskTypeId) = True) OrElse BLD.PersPropCov_EarthquakeApplies = True OrElse (BLD.PersPropCov_RatingTypeId <> "" AndAlso IsNumeric(BLD.PersPropCov_RatingTypeId) = True) OrElse (BLD.PersPropCov_CauseOfLossTypeId <> "" AndAlso IsNumeric(BLD.PersPropCov_CauseOfLossTypeId) = True) OrElse (BLD.PersPropCov_DeductibleId <> "" AndAlso IsNumeric(BLD.PersPropCov_DeductibleId) = True) OrElse (BLD.PersPropCov_CoinsuranceTypeId <> "" AndAlso IsNumeric(BLD.PersPropCov_CoinsuranceTypeId) = True) OrElse (BLD.PersPropCov_ValuationId <> "" AndAlso IsNumeric(BLD.PersPropCov_ValuationId) = True) OrElse (BLD.PersPropCov_ClassificationCode IsNot Nothing AndAlso BLD.PersPropCov_ClassificationCode.ClassCode <> "") Then
                    has_CPP_building_persPropCov = True
                End If

                Dim has_CPP_building_persPropOfOthers As Boolean = False 'added 11/27/2012
                If BLD.PersPropOfOthers_PersonalPropertyLimit <> "" OrElse (BLD.PersPropOfOthers_RiskTypeId <> "" AndAlso IsNumeric(BLD.PersPropOfOthers_RiskTypeId) = True) OrElse BLD.PersPropOfOthers_EarthquakeApplies = True OrElse (BLD.PersPropOfOthers_RatingTypeId <> "" AndAlso IsNumeric(BLD.PersPropOfOthers_RatingTypeId) = True) OrElse (BLD.PersPropOfOthers_CauseOfLossTypeId <> "" AndAlso IsNumeric(BLD.PersPropOfOthers_CauseOfLossTypeId) = True) OrElse (BLD.PersPropOfOthers_DeductibleId <> "" AndAlso IsNumeric(BLD.PersPropOfOthers_DeductibleId) = True) OrElse (BLD.PersPropOfOthers_CoinsuranceTypeId <> "" AndAlso IsNumeric(BLD.PersPropOfOthers_CoinsuranceTypeId) = True) OrElse (BLD.PersPropOfOthers_ValuationId <> "" AndAlso IsNumeric(BLD.PersPropOfOthers_ValuationId) = True) OrElse (BLD.PersPropOfOthers_ClassificationCode IsNot Nothing AndAlso BLD.PersPropOfOthers_ClassificationCode.ClassCode <> "") Then
                    has_CPP_building_persPropOfOthers = True
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

                ' Equipment Breakdown - CPP ONLY
                'If Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                '    If LOC.EquipmentBreakdownDeductibleQuotedPremium <> "" AndAlso LOC.EquipmentBreakdownDeductibleQuotedPremium <> "0" Then
                '        html.AppendLine("<tr class=""qs_basic_info_labels_cell"">")
                '        WriteCell(html, "Equipment Breakdown")
                '        WriteCell(html, "&nbsp;", 2)
                '        WriteCell(html, LOC.EquipmentBreakdownDeductibleQuotedPremium, "text-align:right;")
                '        html.AppendLine("</tr>")
                '    End If
                'End If

                '=================================================
                ' Spacer line
                html.AppendLine("<tr><td colspan=4>&nbsp;</td></tr>")

                If has_CPP_building_buildingCov Then
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
                    If LocationWindHailHelper.IsLocationWindHailAvailable(Quote) AndAlso LOC.WindHailDeductibleLimitId <> BLD.OptionalWindstormOrHailDeductibleId Then
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

                'Mine Subsidence - Added 11/16/18 for multi state MLW
                If IsMultistateCapableEffectiveDate(Me.Quote.EffectiveDate) Then
                    If BLD.HasMineSubsidence Then
                        html.AppendLine("<tr class=""qs_basic_info_labels_cell qs_grid_4_columns"">")
                        WriteCell(html, "Mine Subsidence", 3)
                        WriteCell(html, BLD.MineSubsidenceQuotedPremium, "text-align:right;")
                        html.AppendLine("</tr>")
                    End If
                End If

                '=================================================
                ' Spacer line
                html.AppendLine("<tr><td colspan=4>&nbsp;</td></tr>")

                If has_CPP_building_busIncomeCov Then
                    'Updated 12/20/18 for multi state MLW
                    'If Quote.HasBusinessIncomeALS Then
                    If SubQuoteFirst.HasBusinessIncomeALS Then
                        'ALS
                        html.AppendLine("<tr class=""qs_basic_info_labels_cell  qs_grid_4_columns"">")
                        WriteCell(html, "Business Income ALS", 3)
                        'WriteCell(html, Quote.BusinessIncomeALSQuotedPremium, "text-align:right;")
                        WriteCell(html, BLD.CPR_BusinessIncomeCov_With_EQ_QuotedPremium, "text-align:right;")
                        html.AppendLine("</tr>")
                    Else
                        'Business Income Limit
                        html.AppendLine("<tr class=""qs_basic_info_labels_cell  qs_grid_4_columns"">")
                        WriteCell(html, "Business Income Limit: " & BLD.BusinessIncomeCov_Limit, 3)
                        WriteCell(html, BLD.BusinessIncomeCov_QuotedPremium, "text-align:right;")
                        html.AppendLine("</tr>")

                        'Cause of Loss
                        html.AppendLine("<tr class=""qs_basic_info_labels_cell  qs_grid_4_columns"">")
                        WriteCell(html, "Cause of Loss: " & BLD.BusinessIncomeCov_CauseOfLossType, 4)
                        html.AppendLine("</tr>")
                    End If

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
                html.AppendLine("<tr><td colspan=4>&nbsp;</td></tr>")

                If has_CPP_building_persPropCov Then
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
                    If LocationWindHailHelper.IsLocationWindHailAvailable(Quote) AndAlso LOC.WindHailDeductibleLimitId <> BLD.PersPropCov_OptionalWindstormOrHailDeductibleId Then
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
                html.AppendLine("<tr><td colspan=4>&nbsp;</td></tr>")

                If has_CPP_building_persPropOfOthers Then
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
                    If LocationWindHailHelper.IsLocationWindHailAvailable(Quote) AndAlso LOC.WindHailDeductibleLimitId <> BLD.PersPropOfOthers_OptionalWindstormOrHailDeductibleId Then
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
                End If
                If pito.IsAgreedValue Then
                    agreedTxt = "/Agreed Amount"
                End If
                html.AppendLine("<tr class=""qs_basic_info_labels_cell qs_grid_4_columns"">")
                WriteCell(html, "Building Limit: " & pito.Limit & " (" & pito.Valuation & blanketTxt & agreedTxt & ")", 3)
                WriteCell(html, pito.QuotedPremium, "text-align:right;")
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
    Private Sub PopulateClassifications()
        Dim html As New StringBuilder()
        Dim showLocation As Boolean
        Dim showPolicy As Boolean

        If Quote Is Nothing OrElse Quote.Locations Is Nothing Then Exit Sub

        ' Section Header
        html.AppendLine("<span Class='qs_section_headers'>CLASSIFICATIONS</span>")
        html.AppendLine("<div Class='qs_Sub_Sections'>")

        For Each LOC As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
            If LOC.GLClassifications.Count > 0 Then
                showLocation = True
                Exit For
            End If
        Next

        If showLocation Then
            ' Header Title Row - Location Class Codes
            html.AppendLine("<table class=""qa_table_shades"">")
            html.AppendLine("<tr class=""qs_section_grid_headers ui-widget-header"">")
            WriteCell(html, "Location GL Class Code", 2)
            html.AppendLine("</tr>")

            ' Header Row - Location Class Codes
            html.AppendLine("<tr class=""qs_section_grid_headers ui-widget-header"">")
            WriteCell(html, "Description")
            WriteCell(html, "ClassCode", "", "qs_rightJustify qs_padRight")
            html.AppendLine("</tr>")

            ' LOCATION CLASS CODE DETAIL LINES
            For Each L As QuickQuote.CommonObjects.QuickQuoteLocation In Quote.Locations
                For Each c As QuickQuote.CommonObjects.QuickQuoteGLClassification In L.GLClassifications

                    ' Line 1 - Location Address
                    html.AppendLine("<tr>")
                    WriteCell(html, L.Address.DisplayAddress)
                    WriteCell(html, "&nbsp;")
                    html.AppendLine("</tr>")

                    ' Line 2 - Class Code Description and Class Code
                    html.AppendLine("<tr>")
                    WriteCell(html, c.ClassDescription)
                    WriteCell(html, c.ClassCode, "", "qs_rightJustify qs_padRight")
                    html.AppendLine("</tr>")

                    ' Line 3 - Premium Exposure
                    html.AppendLine("<tr>")
                    WriteCell(html, "Premium Exposure: &nbsp;" & c.PremiumExposure, 2)
                    html.AppendLine("</tr>")

                    ' Line 4 - Premium Base
                    html.AppendLine("<tr>")
                    WriteCell(html, "Premium Base: &nbsp;" & c.PremiumBase, 2)
                    html.AppendLine("</tr>")

                    ' Line 5 - Premises Premium
                    html.AppendLine("<tr>")
                    WriteCell(html, "Premises Premium: &nbsp;" & c.PremisesQuotedPremium, 2)
                    html.AppendLine("</tr>")

                    ' Line 5 - Products Premium
                    html.AppendLine("<tr>")
                    WriteCell(html, "Products Premium: &nbsp;" & c.ProductsQuotedPremium, 2)
                    html.AppendLine("</tr>")

                    ' Spacer row between each classification
                    html.AppendLine("<tr>")
                    WriteCell(html, "&nbsp;", 2)
                    html.AppendLine("</tr>")
                Next
            Next
        End If

        If Quote.GLClassifications.Count > 0 Then
            showPolicy = True
        End If

        If showPolicy Then
            ' Header Title Row - Policy Class Codes
            html.AppendLine("<table class=""qa_table_shades"">")
            html.AppendLine("<tr class=""qs_section_grid_headers ui-widget-header"">")
            WriteCell(html, "Policy GL Class Code", 2)
            html.AppendLine("</tr>")

            ' Header Row - Location Class Codes
            html.AppendLine("<tr class=""qs_section_grid_headers ui-widget-header"">")
            WriteCell(html, "Description")
            WriteCell(html, "ClassCode", "", "qs_rightJustify qs_padRight")
            html.AppendLine("</tr>")

            ' POLICY CLASS CODES DETAIL LINES
            For Each c As QuickQuote.CommonObjects.QuickQuoteGLClassification In Quote.GLClassifications

                ' Line 1 - Class Code Description and Class Code
                html.AppendLine("<tr>")
                WriteCell(html, c.ClassDescription)
                WriteCell(html, c.ClassCode, "", "qs_rightJustify qs_padRight")
                html.AppendLine("</tr>")

                ' Line 2 - Premium Exposure
                html.AppendLine("<tr>")
                WriteCell(html, "Premium Exposure: &nbsp;" & c.PremiumExposure, 2)
                html.AppendLine("</tr>")

                ' Line 3 - Premium Base
                html.AppendLine("<tr>")
                WriteCell(html, "Premium Base: &nbsp;" & c.PremiumBase, 2)
                html.AppendLine("</tr>")

                ' Line 4 - Premises Premium
                html.AppendLine("<tr>")
                WriteCell(html, "Premises Premium: &nbsp;" & c.PremisesQuotedPremium, 2)
                html.AppendLine("</tr>")

                ' Line 5 - Products Premium
                html.AppendLine("<tr>")
                WriteCell(html, "Products Premium: &nbsp;" & c.ProductsQuotedPremium, 2)
                html.AppendLine("</tr>")

                ' Spacer row between each classification
                html.AppendLine("<tr>")
                WriteCell(html, "&nbsp;", 2)
                html.AppendLine("</tr>")
            Next
        End If



        ' Close table
        html.AppendLine("</table>")

        ' Close div
        html.AppendLine("</div>")

        ' Write the section to the page
        tblClassifications.Text = html.ToString()

        Exit Sub
    End Sub

    Private Sub PopulateInlandMarine()
        Dim html As New StringBuilder()
        Dim DedText As String = ""

        html.AppendLine("<div Class='qs_Main_Sections'>")
        html.AppendLine("<span Class='qs_section_headers'>Inland Marine</span>")
        html.AppendLine("<div class=""qs_Sub_Sections"">")

        ' Start the table
        html.AppendLine("<table class=""qa_table_shades"">")

        ' Orange Header Row
        html.AppendLine("<tr class=""qs_section_grid_headers ui-widget-header qs_grid_4_columns"">")
        WriteCell(html, "Coverage", "width:55%")
        WriteCell(html, "Limit", "width:15%")
        WriteCell(html, "Deductible", "width:15%")
        WriteCell(html, "Premium", "text-align:right;width:15%")
        html.AppendLine("</tr>")

        ' *** Data rows ***
        ' Builders Risk - Scheduled
        'Updated 12/18/18 for multi state Bug 30310 MLW - Total Premiums and use GoverningStateQuote
        Dim premBuildersRisk As String = Format(CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.BuildersRiskQuotedPremium)), NumFormatWithCents)
        'If Quote.BuildersRiskQuotedPremium <> "" AndAlso Quote.BuildersRiskQuotedPremium <> "0" AndAlso Quote.BuildersRiskQuotedPremium <> "$0.00" Then
        If premBuildersRisk <> "" AndAlso premBuildersRisk <> "0" AndAlso premBuildersRisk <> "0.00" AndAlso premBuildersRisk <> "$.00" AndAlso premBuildersRisk <> "$0.00" Then
            DedText = "**"
            DedText = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.ComputerAllPerilsDeductibleId, Quote.BuildersRiskDeductibleId)
            html.AppendLine("<tr class=""qs_basic_info_labels_cell"">")
            WriteCell(html, "Builders Risk - Scheduled")
            'WriteCell(html, Quote.BuildersRiskScheduledLocationsTotalLimit)
            WriteCell(html, GoverningStateQuote.BuildersRiskScheduledLocationsTotalLimit)
            WriteCell(html, DedText)
            'WriteCell(html, Quote.BuildersRiskQuotedPremium, "", "qs_rightJustify qs_padRight")
            WriteCell(html, premBuildersRisk, "", "qs_rightJustify qs_padRight")
            html.AppendLine("</tr>")
        End If

        ' Computer
        'Updated 12/18/18 for multi state Bug 30310 MLW - Total Premiums and use GoverningStateQuote
        Dim premComputer As String = Format(CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.ComputerQuotedPremium)), NumFormatWithCents)
        Dim limComputer As String = Format(CInt(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.ComputerBuildingsTotalLimit)), NumFormat)
        'If Quote.ComputerQuotedPremium <> "" AndAlso Quote.ComputerQuotedPremium <> "0" AndAlso Quote.ComputerQuotedPremium <> "$0.00" Then
        If premComputer <> "" AndAlso premComputer <> "0" AndAlso premComputer <> "0.00" AndAlso premComputer <> "$.00" AndAlso premComputer <> "$0.00" Then
            DedText = "**"
            DedText = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.ComputerAllPerilsDeductibleId, Quote.ComputerAllPerilsDeductibleId)
            html.AppendLine("<tr class=""qs_basic_info_labels_cell"">")
            WriteCell(html, "Computer")
            'WriteCell(html, Quote.ComputerBuildingsTotalLimit)
            'WriteCell(html, GoverningStateQuote.ComputerBuildingsTotalLimit)
            WriteCell(html, limComputer)
            WriteCell(html, DedText)
            'WriteCell(html, Quote.ComputerQuotedPremium, "", "qs_rightJustify qs_padRight")
            WriteCell(html, premComputer, "", "qs_rightJustify qs_padRight")
            html.AppendLine("</tr>")
        End If

        ' Contractors Equipment
        'Updated 12/18/18 for multi state Bug 30310 MLW - Total Premiums and use GoverningStateQuote
        ' Updated 1/7/19 To remove the the Small Tools calcs.  This is handled by the UnscheduledToolsPremium property.
        Dim premContractorsLeased As String = CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.ContractorsEquipmentLeasedRentedFromOthersQuotedPremium))
        Dim premContractorsSchedule As String = CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.ContractorsEquipmentScheduleQuotedPremium))
        'Dim premContractorsSmallTools As String = CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceQuotedPremium))
        Dim UnscheduledToolsPremiumValue As String = UnscheduledToolsPremium
        If HasContractorsEquipment(UnscheduledToolsPremiumValue) Then
            ' Get the total premium from the 3 contractor premium fields
            Dim totprem As Decimal = 0
            'If IsNumeric(Quote.ContractorsEquipmentLeasedRentedFromOthersQuotedPremium) Then totprem += CDec(Quote.ContractorsEquipmentLeasedRentedFromOthersQuotedPremium)
            'If IsNumeric(Quote.ContractorsEquipmentScheduleQuotedPremium) Then totprem += CDec(Quote.ContractorsEquipmentScheduleQuotedPremium)
            'If IsNumeric(Quote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceQuotedPremium) Then totprem += CDec(Quote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceQuotedPremium)
            If IsNumeric(premContractorsLeased) Then totprem += CDec(premContractorsLeased)
            If IsNumeric(premContractorsSchedule) Then totprem += CDec(premContractorsSchedule)
            'If IsNumeric(premContractorsSmallTools) Then totprem += CDec(premContractorsSmallTools)
            If IsNumeric(UnscheduledToolsPremiumValue) Then totprem += CDec(UnscheduledToolsPremiumValue)
            'If CDec(premContractorsSmallTools) = 0 Then
            '    If IsNumeric(UnscheduledToolsPremiumValue) Then totprem += CDec(UnscheduledToolsPremiumValue)
            'End If
            ' Display the summarized premium
            html.AppendLine("<tr class=""qs_basic_info_labels_cell"">")
            WriteCell(html, "Contractors Equipment", 3)
            WriteCell(html, Format(totprem, "c"), "", "qs_rightJustify qs_padRight")
            html.AppendLine("</tr>")
        End If

        ' Fine Arts Floater
        'Updated 12/18/18 for multi state Bug 30310 MLW - Total Premiums and use GoverningStateQuote
        Dim premFineArts As String = Format(CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.FineArtsBuildingsTotalQuotedPremium)), NumFormatWithCents)
        Dim limFineArts As String = Format(CInt(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.FineArtsBuildingsTotalLimit)), NumFormat)
        'If Quote.FineArtsBuildingsTotalQuotedPremium <> "" AndAlso Quote.FineArtsBuildingsTotalQuotedPremium <> "0" AndAlso Quote.FineArtsBuildingsTotalQuotedPremium <> "$0.00" Then
        If premFineArts <> "" AndAlso premFineArts <> "0" AndAlso premFineArts <> "0.00" AndAlso premFineArts <> "$.00" AndAlso premFineArts <> "$0.00" Then
            DedText = "**"
            DedText = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.FineArtsDeductibleId, Quote.FineArtsDeductibleId)
            html.AppendLine("<tr class=""qs_basic_info_labels_cell"">")
            WriteCell(html, "Fine Arts Floater")
            'WriteCell(html, Quote.FineArtsBuildingsTotalLimit)
            'WriteCell(html, GoverningStateQuote.FineArtsBuildingsTotalLimit)
            WriteCell(html, limFineArts)
            WriteCell(html, DedText)
            'WriteCell(html, Quote.FineArtsBuildingsTotalQuotedPremium, "", "qs_rightJustify qs_padRight")
            WriteCell(html, premFineArts, "", "qs_rightJustify qs_padRight")
            html.AppendLine("</tr>")
        End If

        ' Installation Floater
        'Updated 12/18/18 for multi state Bug 30310 MLW - Total Premiums and use GoverningStateQuote
        Dim premInstallation As String = Format(CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.InstallationBlanketQuotedPremium)), NumFormatWithCents)
        'If Quote.InstallationBlanketQuotedPremium <> "" AndAlso Quote.InstallationBlanketQuotedPremium <> "0" AndAlso Quote.InstallationBlanketQuotedPremium <> "$0.00" Then
        If premInstallation <> "" AndAlso premInstallation <> "0" AndAlso premInstallation <> "0.00" AndAlso premInstallation <> "$.00" AndAlso premInstallation <> "$0.00" Then
            DedText = "**"
            DedText = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.InstallationBlanketDeductibleId, Quote.InstallationBlanketDeductibleId)
            html.AppendLine("<tr class=""qs_basic_info_labels_cell"">")
            WriteCell(html, "Installation Floater")
            'WriteCell(html, Quote.InstallationBlanketLimit)
            WriteCell(html, GoverningStateQuote.InstallationBlanketLimit)
            WriteCell(html, DedText)
            'WriteCell(html, Quote.InstallationBlanketQuotedPremium, "", "qs_rightJustify qs_padRight")
            WriteCell(html, premInstallation, "", "qs_rightJustify qs_padRight")
            html.AppendLine("</tr>")
        End If

        'Motor Truck Cargo
        Dim motorTruckCargoVehicleLimit As String = String.Empty
        Dim premMotorTruckCargo As String = String.Empty
        If UnScheduledMotorTruckCargoHelper.IsUnScheduledMotorTruckCargoAvailable(Quote) Then
            motorTruckCargoVehicleLimit = GoverningStateQuote.MotorTruckCargoUnScheduledAnyVehicleLimit
            premMotorTruckCargo = Format(CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.MotorTruckCargoUnScheduledVehicleQuotedPremium)), NumFormatWithCents)
            DedText = "**"
            DedText = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.MotorTruckCargoUnScheduledVehicleDeductibleId, Quote.MotorTruckCargoUnScheduledVehicleDeductibleId)
        Else
            motorTruckCargoVehicleLimit = GoverningStateQuote.MotorTruckCargoScheduledVehiclesTotalLimit
            premMotorTruckCargo = Format(CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.MotorTruckCargoScheduledVehicleQuotedPremium)), NumFormatWithCents)
            DedText = "**"
            DedText = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.MotorTruckCargoScheduledVehicleDeductibleId, Quote.MotorTruckCargoScheduledVehicleDeductibleId)
        End If
        If premMotorTruckCargo <> "" AndAlso premMotorTruckCargo <> "0" AndAlso premMotorTruckCargo <> "0.00" AndAlso premMotorTruckCargo <> "$.00" AndAlso premMotorTruckCargo <> "$0.00" Then
            html.AppendLine("<tr class=""qs_basic_info_labels_cell"">")
            WriteCell(html, "Motor Truck Cargo")
            'WriteCell(html, Quote.MotorTruckCargoScheduledVehiclesTotalLimit)
            WriteCell(html, motorTruckCargoVehicleLimit)
            WriteCell(html, DedText)
            'WriteCell(html, Quote.MotorTruckCargoScheduledVehicleQuotedPremium, "", "qs_rightJustify qs_padRight")
            WriteCell(html, premMotorTruckCargo, "", "qs_rightJustify qs_padRight")
            html.AppendLine("</tr>")
        End If

        ' Owners Cargo
        'Updated 12/18/18 for multi state Bug 30310 MLW - Total Premiums and use GoverningStateQuote
        Dim premOwnersCargo As String = Format(CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.OwnersCargoAnyOneOwnedVehicleQuotedPremium)), NumFormatWithCents)
        'If Quote.OwnersCargoAnyOneOwnedVehicleQuotedPremium <> "" AndAlso Quote.OwnersCargoAnyOneOwnedVehicleQuotedPremium <> "0" AndAlso Quote.OwnersCargoAnyOneOwnedVehicleQuotedPremium <> "$0.00" Then
        If premOwnersCargo <> "" AndAlso premOwnersCargo <> "0" AndAlso premOwnersCargo <> "0.00" AndAlso premOwnersCargo <> "$.00" AndAlso premOwnersCargo <> "$0.00" Then
            DedText = "**"
            DedText = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.OwnersCargoAnyOneOwnedVehicleDeductibleId, Quote.OwnersCargoAnyOneOwnedVehicleDeductibleId)
            html.AppendLine("<tr class=""qs_basic_info_labels_cell"">")
            WriteCell(html, "Owners Cargo")
            'WriteCell(html, Quote.OwnersCargoAnyOneOwnedVehicleLimit)
            WriteCell(html, GoverningStateQuote.OwnersCargoAnyOneOwnedVehicleLimit)
            WriteCell(html, DedText)
            'WriteCell(html, Quote.OwnersCargoAnyOneOwnedVehicleQuotedPremium, "", "qs_rightJustify qs_padRight")
            WriteCell(html, premOwnersCargo, "", "qs_rightJustify qs_padRight")
            html.AppendLine("</tr>")
        End If

        ' Scheduled Property Floater
        'Updated 12/18/18 for multi state Bug 30310 MLW - Total Premiums and use GoverningStateQuote
        Dim premScheduledProperty As String = Format(CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.ScheduledPropertyQuotedPremium)), NumFormatWithCents)
        'If Quote.ScheduledPropertyQuotedPremium <> "" AndAlso Quote.ScheduledPropertyQuotedPremium <> "0" AndAlso Quote.ScheduledPropertyQuotedPremium <> "$0.00" Then
        If premScheduledProperty <> "" AndAlso premScheduledProperty <> "0" AndAlso premScheduledProperty <> "0.00" AndAlso premScheduledProperty <> "$.00" AndAlso premScheduledProperty <> "$0.00" Then
            DedText = "**"
            DedText = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.ScheduledPropertyDeductibleId, Quote.ScheduledPropertyDeductibleId)
            html.AppendLine("<tr class=""qs_basic_info_labels_cell"">")
            WriteCell(html, "Scheduled Property Floater")
            'WriteCell(html, Quote.ScheduledPropertyItemsTotalLimit)
            WriteCell(html, GoverningStateQuote.ScheduledPropertyItemsTotalLimit)
            WriteCell(html, DedText)
            'WriteCell(html, Quote.ScheduledPropertyQuotedPremium, "", "qs_rightJustify qs_padRight")
            WriteCell(html, premScheduledProperty, "", "qs_rightJustify qs_padRight")
            html.AppendLine("</tr>")
        End If

        ' Signs
        'Updated 12/18/18 for multi state Bug 30310 MLW - Total Premiums and use GoverningStateQuote
        Dim premSigns As String = Format(CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.SignsBuildingTotalQuotedPremium)), NumFormatWithCents)
        Dim limSigns As String = Format(CInt(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.SignsBuildingTotalLimit)), NumFormat)
        'If Quote.SignsBuildingTotalQuotedPremium <> "" AndAlso Quote.SignsBuildingTotalQuotedPremium <> "0" AndAlso Quote.SignsBuildingTotalQuotedPremium <> "$0.00" Then
        If premSigns <> "" AndAlso premSigns <> "0" AndAlso premSigns <> "0.00" AndAlso premSigns <> "$.00" AndAlso premSigns <> "$0.00" Then
            DedText = "**"
            DedText = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.SignsDeductibleId, Quote.SignsDeductibleId)
            html.AppendLine("<tr class=""qs_basic_info_labels_cell"">")
            WriteCell(html, "Signs")
            'WriteCell(html, Quote.SignsBuildingTotalLimit)
            'WriteCell(html, GoverningStateQuote.SignsBuildingTotalLimit)
            WriteCell(html, limSigns)
            WriteCell(html, DedText)
            'WriteCell(html, Quote.SignsBuildingTotalQuotedPremium, "", "qs_rightJustify qs_padRight")
            WriteCell(html, premSigns, "", "qs_rightJustify qs_padRight")
            html.AppendLine("</tr>")
        End If

        ' Transportation
        'Updated 12/18/18 for multi state Bug 30310 MLW - Total Premiums and use GoverningStateQuote
        Dim premTransportation As String = Format(CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.TransportationCatastropheQuotedPremium)), NumFormatWithCents)
        'If Quote.TransportationCatastropheQuotedPremium <> "" AndAlso Quote.TransportationCatastropheQuotedPremium <> "0" AndAlso Quote.TransportationCatastropheQuotedPremium <> "$0.00" Then
        If premTransportation <> "" AndAlso premTransportation <> "0" AndAlso premTransportation <> "0.00" AndAlso premTransportation <> "$.00" AndAlso premTransportation <> "$0.00" Then
            DedText = "**"
            DedText = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.TransportationCatastropheDeductibleId, Quote.TransportationCatastropheDeductibleId)
            html.AppendLine("<tr class=""qs_basic_info_labels_cell"">")
            WriteCell(html, "Transportation")
            'WriteCell(html, Quote.TransportationCatastropheLimit)
            WriteCell(html, GoverningStateQuote.TransportationCatastropheLimit)
            WriteCell(html, DedText)
            'WriteCell(html, Quote.TransportationCatastropheQuotedPremium, "", "qs_rightJustify qs_padRight")
            WriteCell(html, premTransportation, "", "qs_rightJustify qs_padRight")
            html.AppendLine("</tr>")
        End If

        ' Close table
        html.AppendLine("</table>")

        ' Close divs
        html.AppendLine("</div>")
        html.AppendLine("</div>")

        ' Write the section to the page
        tblInlandMarine.Text = html.ToString()

        Exit Sub
    End Sub

    'Updated 12/18/18 for multi state Bug 30310 MLW
    Private Function HasContractorsEquipment(UST As String) As Boolean
        'If Quote.ContractorsEquipmentLeasedRentedFromOthersQuotedPremium IsNot Nothing AndAlso Quote.ContractorsEquipmentLeasedRentedFromOthersQuotedPremium <> "" AndAlso Quote.ContractorsEquipmentLeasedRentedFromOthersQuotedPremium <> "0" AndAlso Quote.ContractorsEquipmentLeasedRentedFromOthersQuotedPremium <> "$0.00" Then Return True
        'If Quote.ContractorsEquipmentScheduleQuotedPremium IsNot Nothing AndAlso Quote.ContractorsEquipmentScheduleQuotedPremium <> "" AndAlso Quote.ContractorsEquipmentScheduleQuotedPremium <> "0" AndAlso Quote.ContractorsEquipmentScheduleQuotedPremium <> "$0.00" Then Return True
        'If Quote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceQuotedPremium IsNot Nothing AndAlso Quote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceQuotedPremium <> "" AndAlso Quote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceQuotedPremium <> "0" AndAlso Quote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceQuotedPremium <> "$0.00" Then Return True
        Dim premContractorsLeased As String = CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.ContractorsEquipmentLeasedRentedFromOthersQuotedPremium))
        Dim premContractorsSchedule As String = CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.ContractorsEquipmentScheduleQuotedPremium))
        Dim premContractorsSmallTools As String = CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.ContractorsEquipmentSmallToolsEndorsementPerOccurrenceQuotedPremium))

        If premContractorsLeased IsNot Nothing AndAlso premContractorsLeased <> "" AndAlso premContractorsLeased <> "0" AndAlso premContractorsLeased <> "0.00" AndAlso premContractorsLeased <> "$.00" AndAlso premContractorsLeased <> "$0.00" Then Return True
        If premContractorsSchedule IsNot Nothing AndAlso premContractorsSchedule <> "" AndAlso premContractorsSchedule <> "0" AndAlso premContractorsSchedule <> "0.00" AndAlso premContractorsSchedule <> "$.00" AndAlso premContractorsSchedule <> "$0.00" Then Return True
        If premContractorsSmallTools IsNot Nothing AndAlso premContractorsSmallTools <> "" AndAlso premContractorsSmallTools <> "0" AndAlso premContractorsSmallTools <> "0.00" AndAlso premContractorsSmallTools <> "$.00" AndAlso premContractorsSmallTools <> "$0.00" Then Return True

        'Added 12/26/18 to match quote summary display MLW
        If CDec(UST) > 0 Then Return True

        Return False
    End Function

    Private Sub PopulateCrime()
        Dim html As New StringBuilder()
        Dim DedText As String = ""

        html.AppendLine("<div Class='qs_Main_Sections'>")
        html.AppendLine("<span Class='qs_section_headers'>Crime</span>")
        html.AppendLine("<div class=""qs_Sub_Sections"">")

        ' Start the table
        html.AppendLine("<table class=""qa_table_shades"">")

        ' First Orange Header Row - Class Code Header
        html.AppendLine("<tr class=""qs_section_grid_headers ui-widget-header qs_grid_4_columns"">")
        WriteCell(html, "ClassCode" & "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" & "Description", 4)
        html.AppendLine("</tr>")

        ' Second Orange Header Row - Coverage Header
        html.AppendLine("<tr class=""qs_section_grid_headers ui-widget-header qs_grid_4_columns"">")
        WriteCell(html, "Coverage", "width:55%")
        WriteCell(html, "Limit", "width:15%")
        WriteCell(html, "Deductible", "width:15%")
        WriteCell(html, "Premium", "text-align:right;width:15%")
        html.AppendLine("</tr>")

        ' *** Data rows ***
        ' Class Code
        'Updated 12/17/18 for multi state bug 30351 MLW
        'If Quote.ClassificationCodes IsNot Nothing AndAlso Quote.ClassificationCodes.Count > 0 Then
        If GoverningStateQuote.ClassificationCodes IsNot Nothing AndAlso GoverningStateQuote.ClassificationCodes.Count > 0 Then
            'Dim cc As QuickQuote.CommonObjects.QuickQuoteClassificationCode = Quote.ClassificationCodes(0)
            Dim cc As QuickQuote.CommonObjects.QuickQuoteClassificationCode = GoverningStateQuote.ClassificationCodes(0)
            html.AppendLine("<tr class=""qs_basic_info_labels_cell"">")
            WriteCell(html, cc.ClassCode & "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" & cc.ClassDescription, 4)
            html.AppendLine("</tr>")
        End If

        ' COVERAGES
        ' Employee Theft
        'Updated 12/17/18 for multi state bug 30351 MLW
        Dim premEmpTheft As String = Format(CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.EmployeeTheftQuotedPremium)), NumFormatWithCents)
        'If Quote.EmployeeTheftQuotedPremium <> "" AndAlso Quote.EmployeeTheftQuotedPremium <> "0" AndAlso Quote.EmployeeTheftQuotedPremium <> "$0.00" Then
        If premEmpTheft <> "" AndAlso premEmpTheft <> "0" AndAlso premEmpTheft <> "0.00" AndAlso premEmpTheft <> "$.00" AndAlso premEmpTheft <> "$0.00" Then
            DedText = "**"
            DedText = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.EmployeeTheftDeductibleId, GoverningStateQuote.EmployeeTheftDeductibleId)
            html.AppendLine("<tr class=""qs_basic_info_labels_cell"">")
            WriteCell(html, "Employee Theft")
            'WriteCell(html, Quote.EmployeeTheftLimit)
            WriteCell(html, GoverningStateQuote.EmployeeTheftLimit)
            WriteCell(html, DedText)
            'WriteCell(html, Quote.EmployeeTheftQuotedPremium, "", "qs_rightJustify qs_padRight")
            WriteCell(html, premEmpTheft, "", "qs_rightJustify qs_padRight")
            html.AppendLine("</tr>")
        End If
        ' Inside Premises
        'Updated 12/17/18 for multi state bug 30351 MLW
        Dim premInsideTheft As String = Format(CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.InsidePremisesTheftOfMoneyAndSecuritiesQuotedPremium)), NumFormatWithCents)
        'If Quote.InsidePremisesTheftOfMoneyAndSecuritiesQuotedPremium <> "" AndAlso Quote.InsidePremisesTheftOfMoneyAndSecuritiesQuotedPremium <> "0" AndAlso Quote.InsidePremisesTheftOfMoneyAndSecuritiesQuotedPremium <> "$0.00" Then
        If premInsideTheft <> "" AndAlso premInsideTheft <> "0" AndAlso premInsideTheft <> "0.00" AndAlso premInsideTheft <> "$.00" AndAlso premInsideTheft <> "$0.00" Then
            DedText = "**"
            DedText = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.InsidePremisesTheftOfMoneyAndSecuritiesDeductibleId, GoverningStateQuote.InsidePremisesTheftOfMoneyAndSecuritiesDeductibleId)
            html.AppendLine("<tr class=""qs_basic_info_labels_cell"">")
            WriteCell(html, "Inside Premises - Theft of Money and Securities")
            'WriteCell(html, Quote.InsidePremisesTheftOfMoneyAndSecuritiesLimit)
            WriteCell(html, GoverningStateQuote.InsidePremisesTheftOfMoneyAndSecuritiesLimit)
            WriteCell(html, DedText)
            'WriteCell(html, Quote.InsidePremisesTheftOfMoneyAndSecuritiesQuotedPremium, "", "qs_rightJustify qs_padRight")
            WriteCell(html, premInsideTheft, "", "qs_rightJustify qs_padRight")
            html.AppendLine("</tr>")
        End If
        ' Outside the Premises
        'Updated 12/17/18 for multi state bug 30351 MLW
        Dim premOutsideTheft As String = Format(CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.OutsideThePremisesQuotedPremium)), NumFormatWithCents)
        'If Quote.OutsideThePremisesQuotedPremium <> "" AndAlso Quote.OutsideThePremisesQuotedPremium <> "0" AndAlso Quote.OutsideThePremisesQuotedPremium <> "$0.00" Then
        If premOutsideTheft <> "" AndAlso premOutsideTheft <> "0" AndAlso premOutsideTheft <> "0.00" AndAlso premOutsideTheft <> "$.00" AndAlso premOutsideTheft <> "$0.00" Then
            DedText = "**"
            DedText = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.OutsideThePremisesDeductibleId, GoverningStateQuote.OutsideThePremisesDeductibleId)
            html.AppendLine("<tr class=""qs_basic_info_labels_cell"">")
            WriteCell(html, "Outside the Premises")
            'WriteCell(html, Quote.OutsideThePremisesLimit)
            WriteCell(html, GoverningStateQuote.OutsideThePremisesLimit)
            WriteCell(html, DedText)
            'WriteCell(html, Quote.OutsideThePremisesQuotedPremium, "", "qs_rightJustify qs_padRight")
            WriteCell(html, premOutsideTheft, "", "qs_rightJustify qs_padRight")
            html.AppendLine("</tr>")
        End If

        'added for new coverages 02/03/2020
        ' Forgery or Alteration
        Dim premForgeryAlteration As String = Format(CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.ForgeryAlterationQuotedPremium)), NumFormatWithCents)
        If premForgeryAlteration <> "" AndAlso premForgeryAlteration <> "0" AndAlso premForgeryAlteration <> "0.00" AndAlso premForgeryAlteration <> "$.00" AndAlso premForgeryAlteration <> "$0.00" Then
            DedText = "**"
            DedText = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.ForgeryAlterationDeductibleId, GoverningStateQuote.ForgeryAlterationDeductibleId)
            html.AppendLine("<tr class=""qs_basic_info_labels_cell"">")
            WriteCell(html, "Forgery or Alteration")
            WriteCell(html, GoverningStateQuote.ForgeryAlterationLimit)
            WriteCell(html, DedText)
            WriteCell(html, premForgeryAlteration, "", "qs_rightJustify qs_padRight")
            html.AppendLine("</tr>")
        End If

        ' Computer Fraud
        Dim premComputerFraud As String = Format(CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.ComputerFraudQuotedPremium)), NumFormatWithCents)
        If premComputerFraud <> "" AndAlso premComputerFraud <> "0" AndAlso premComputerFraud <> "0.00" AndAlso premComputerFraud <> "$.00" AndAlso premComputerFraud <> "$0.00" Then
            DedText = "**"
            DedText = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.ComputerFraudDeductibleId, GoverningStateQuote.ComputerFraudDeductibleId)
            html.AppendLine("<tr class=""qs_basic_info_labels_cell"">")
            WriteCell(html, "Computer Fraud")
            WriteCell(html, GoverningStateQuote.ComputerFraudLimit)
            WriteCell(html, DedText)
            WriteCell(html, premComputerFraud, "", "qs_rightJustify qs_padRight")
            html.AppendLine("</tr>")
        End If

        ' Funds Transfer Fraud
        Dim premFundsTransferFraud As String = Format(CDec(QQHelper.GetSumForPropertyValues(SubQuotes, Function() Quote.FundsTransferFraudQuotedPremium)), NumFormatWithCents)
        If premFundsTransferFraud <> "" AndAlso premFundsTransferFraud <> "0" AndAlso premFundsTransferFraud <> "0.00" AndAlso premFundsTransferFraud <> "$.00" AndAlso premFundsTransferFraud <> "$0.00" Then
            DedText = "**"
            DedText = QQHelper.GetStaticDataTextForValue(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.FundsTransferDeductibleId, GoverningStateQuote.FundsTransferFraudDeductibleId)
            html.AppendLine("<tr class=""qs_basic_info_labels_cell"">")
            WriteCell(html, "Funds Transfer Fraud")
            WriteCell(html, GoverningStateQuote.FundsTransferFraudLimit)
            WriteCell(html, DedText)
            WriteCell(html, premFundsTransferFraud, "", "qs_rightJustify qs_padRight")
            html.AppendLine("</tr>")
        End If

        ' Close table
        html.AppendLine("</table>")

        ' Close divs
        html.AppendLine("</div>")
        html.AppendLine("</div>")

        ' Write the section to the page
        tblCrime.Text = html.ToString()

        Exit Sub
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