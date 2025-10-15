Imports QuickQuote.CommonObjects 'added 5/19/2017
Imports QuickQuote.CommonMethods 'added 5/19/201

Imports System.Data
Imports System.Xml

'------------------------------------
'This page is just for easier testing. This page is actually located in the DiamondQuickQuote project
'------------------------------------
Partial Class DiamondQuoteSummary_QQ
    Inherits System.Web.UI.Page

    Dim quickQuote As QuickQuoteObject
    Dim QQxml As New QuickQuoteXML
    Dim qqHelper As New QuickQuoteHelperClass
    Dim BuildAutoIncreaseRow As HtmlTableRow

    Public ReadOnly Property MyQuote As QuickQuoteObject
        Get

            If quickQuote Is Nothing Then
                Dim rateType As QuickQuoteXML.QuickQuoteSaveType = Nothing
                Dim errorMsg As String = ""
                QQxml.GetRatedQuote(Me.lblQuoteId.Text, quickQuote, rateType, errorMsg)
            End If
            Return quickQuote
        End Get
    End Property

    Public Shared Function IsStaffUser() As Boolean
        Dim qqHelper As New QuickQuoteHelperClass()
        Return qqHelper.IsHomeOfficeStaffUser()
    End Function

    Public ReadOnly Property BusinessIncomeLabel As String
        Get
            If quickQuote IsNot Nothing Then
                If quickQuote.HasBusinessIncomeALS Then
                    Return "Business Income ALS"
                End If
            End If
            Return "Business Income"
        End Get
    End Property

    Public ReadOnly Property HasBusinessIncomeALS As Boolean
        Get
            If quickQuote IsNot Nothing Then
                Return quickQuote.HasBusinessIncomeALS
            End If
            Return False
        End Get
    End Property

    Enum PrintType
        All = 1
        JustWorksheet = 2
    End Enum
    'added 1/11/2013 to better identify Route To Underwriting scenarios
    Enum RouteToUnderwritingType
        QuoteSuccess = 1
        QuoteFail = 2
        AppGapSuccess = 3 'added 3/1/2013
        AppGapFail = 4 'changed from 3 on 3/1/2013
    End Enum

    Public Sub HideNewBOPHTMLFieldsByJS()
        Dim jScript = "HideNewBOPHTMLFields();"
        ScriptManager.RegisterStartupScript(Me.Page, Page.GetType, "HideNewBOPHTMLFields", jScript, True)
    End Sub

    Protected Sub Page_PreInit(sender As Object, e As System.EventArgs) Handles Me.PreInit
        'added printer friendly functionality 8/29/2012
        If Request.QueryString("PrinterFriendlyQuoteId") IsNot Nothing AndAlso Request.QueryString("PrinterFriendlyQuoteId").ToString <> "" AndAlso IsNumeric(Request.QueryString("PrinterFriendlyQuoteId").ToString) = True Then
            Me.MasterPageFile = ConfigurationManager.AppSettings("DiamondQuickQuoteMasterPF")
        Else
            Me.MasterPageFile = ConfigurationManager.AppSettings("DiamondQuickQuoteMaster")
        End If
    End Sub
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Page.MaintainScrollPositionOnPostBack = True 'added 8/21/2012

        If Page.IsPostBack = False Then
            ClearQuotesFromSession(Session) 'added 10/30/2014

            '--uncomment below section for normal page processing
            Me.QuoteErrorSection.Visible = False
            Me.QuoteSummarySection.Visible = False
            Me.lblQuoteErrors.Text = ""

            Me.lblErrorQuoteNumber.Visible = False
            Me.lblErrorQuoteNumber.Text = ""

            Me.lblQuoteMessages.Text = ""
            Me.QuoteMessagesSpacerRow.Visible = False
            Me.QuoteMessagesHeaderRow.Visible = False
            Me.QuoteMessagesValueRow.Visible = False
            Me.PrintHistoryRow.Visible = False

            Me.lblAppGapText.Text = ""
            Me.lblAppGapText.Visible = False

            Me.NormalQuoteButtonsRow.Visible = False 'added 3/1/2013 to initialize for old setup
            Me.QuoteSuccessButtonsRow.Visible = False
            Me.AppGapSuccessButtonsRow.Visible = False
            Me.CopyQuoteSection.Visible = False
            Me.DiamondPortalRow.Visible = False
            Me.NewQuoteForClientSection.Visible = False 'added 9/6/2012
            Me.SecondaryReturnToQuoteSection.Visible = False 'added 1/11/2013
            Me.SecondaryRouteToUnderwritingSection.Visible = False 'added 1/11/2013; for Quote Fail
            Me.SecondaryReturnToQuoteSection_RouteToUW.Visible = False 'added 1/11/2013; for AppGap Fail

            'added 3/1/2013 to initialize new buttons
            Me.CommonButtonsRow.Visible = False
            Me.OptionsButtonsRow.Visible = False
            Me.NavButtonsRow.Visible = False
            'Me.OptionalNavSection.Visible = False '3/4/2013 - now 2 spans
            Me.OptionalNavSection1.Visible = False
            Me.OptionalNavSection2.Visible = False
            Me.ExtraButtonsRow.Visible = False

            'added printer friendly functionality 8/29/2012
            Me.lblIsPrinterFriendly.Text = "NO"
            Me.PrinterFriendlyRow.Visible = False

            If ConfigurationManager.AppSettings("QuickQuote_UseNewButtonLayout") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_UseNewButtonLayout").ToString) = "YES" Then
                'new buttons
                NewButtonsInitialSetup()
            Else
                'old buttons
                OldButtonsInitialSetup()
            End If

            'added printer friendly functionality 8/29/2012
            If UCase(Me.lblIsPrinterFriendly.Text) = "YES" Then
                Me.PrintHistoryRow.Visible = False
                Me.NormalQuoteButtonsRow.Visible = False
                Me.QuoteSuccessButtonsRow.Visible = False
                Me.AppGapSuccessButtonsRow.Visible = False
                Me.DiamondPortalRow.Visible = False
                '2/28/2013 - replaced old button rows
                Me.CommonButtonsRow.Visible = False


            End If
        End If
        If Not qqHelper.doUseNewBOPVersion(MyQuote.EffectiveDate) Then
            HideNewBOPHTMLFieldsByJS()
        End If
    End Sub
    'added 3/1/2013
    Private Sub OldButtonsInitialSetup()
        'added new js stuff 8/21/2012
        Me.btnContinueToAppGap.Attributes.Add("onclick", "btnSubmit_Click(this, 'Continuing...');") 'for disable button and server-side logic
        Me.btnCopyQuote.Attributes.Add("onclick", "btnSubmit_Click(this, 'Copying...');") 'for disable button and server-side logic
        Me.btnCreditDebits.Attributes.Add("onclick", "btnSubmit_Click(this, 'Loading...');") 'for disable button and server-side logic
        Me.btnFinalizeInDiamond.Attributes.Add("onclick", "btnSubmit_Click(this, 'Finalizing...');") 'for disable button and server-side logic
        Me.btnGoToDiamondPortal.Attributes.Add("onclick", "btnSubmit_Click(this, 'Loading...');") 'for disable button and server-side logic
        Me.btnGoToPrintHistory.Attributes.Add("onclick", "btnSubmit_Click(this, 'Loading...');") 'for disable button and server-side logic
        Me.btnReturnToQuote.Attributes.Add("onclick", "btnSubmit_Click(this, 'Loading...');") 'for disable button and server-side logic
        Me.btnRouteToUnderwriting.Attributes.Add("onclick", "btnSubmit_Click(this, 'Routing...');") 'for disable button and server-side logic
        Me.btnExitQuote.Attributes.Add("onclick", "btnSubmit_Click(this, 'Exiting...');") 'for disable button and server-side logic
        Me.btnNewQuoteForClient.Attributes.Add("onclick", "btnSubmit_Click(this, 'Creating New Quote...');") 'for disable button and server-side logic
        'added 1/10/2013 to go back to Quote Edit after App Gap rating error
        Me.btnReturnToQuote2.Attributes.Add("onclick", "btnSubmit_Click(this, 'Loading...');") 'for disable button and server-side logic
        'added 1/11/2013 to allow user to Route on rating error (when applicable - quote is in Diamond)
        Me.btnRouteToUnderwriting2.Attributes.Add("onclick", "btnSubmit_Click(this, 'Routing...');") 'for disable button and server-side logic
        Me.btnRouteToUnderwriting3.Attributes.Add("onclick", "btnSubmit_Click(this, 'Routing...');") 'for disable button and server-side logic

        'Me.btnReturnToQuote.Enabled = False
        'Me.btnReturnToQuote.ToolTip = "No information found for LOB"
        'Me.lblReturnToQuoteLink.Text = "SavedQuotes.aspx"
        Me.lblReturnToQuoteLink.Text = ConfigurationManager.AppSettings("QuickQuote_SavedQuotes").ToString
        Me.btnReturnToQuote.Text = "Return to Saved Quotes"

        'added 1/10/2013 to go back to Quote Edit after App Gap rating error
        Me.lblReturnToQuoteLink2.Text = ConfigurationManager.AppSettings("QuickQuote_SavedQuotes").ToString
        Me.btnReturnToQuote2.Text = "Return to Saved Quotes"

        'Me.lblGoToPortalLink.Text = "https://www.ifmig.net/agentsonly/controlloader.aspx?QueryString=SavedQuotes"
        Me.lblGoToPortalLink.Text = ConfigurationManager.AppSettings("DiamondPortalSavedQuotesLink").ToString
        'Me.btnGoToPortal.Text = "Go to Diamond Portal"

        Me.btnContinueToAppGap.Visible = False
        Me.lblAppGapLink.Text = ""
        Me.btnContinueToAppGap.Text = "Continue to Application"

        Me.lblCreditsDebitsLink.Text = ConfigurationManager.AppSettings("QuickQuote_IRPM_Input").ToString

        Me.NormalQuoteButtonsRow.Visible = True

        If Request.QueryString("QuoteId") IsNot Nothing AndAlso Request.QueryString("QuoteId").ToString <> "" AndAlso IsNumeric(Request.QueryString("QuoteId").ToString) = True Then
            Me.lblQuoteId.Text = Request.QueryString("QuoteId").ToString
            Me.PrinterFriendlyLink.HRef = ConfigurationManager.AppSettings("QuickQuote_QuoteSummary").ToString & "?PrinterFriendlyQuoteId=" & Me.lblQuoteId.Text
            'Me.PrinterFriendlyRow.Visible = True
            GetQuoteFromDb()
        ElseIf Request.QueryString("PrinterFriendlyQuoteId") IsNot Nothing AndAlso Request.QueryString("PrinterFriendlyQuoteId").ToString <> "" AndAlso IsNumeric(Request.QueryString("PrinterFriendlyQuoteId").ToString) = True Then
            'added printer friendly functionality 8/29/2012
            Me.lblIsPrinterFriendly.Text = "YES"
            Me.lblQuoteId.Text = Request.QueryString("PrinterFriendlyQuoteId").ToString
            GetQuoteFromDb()
        Else
            Me.QuoteErrorSection.Visible = True
            Me.lblQuoteErrors.Text = "A valid parameter for QuoteId was not sent thru the querystring."
        End If
    End Sub
    'added 3/1/2013
    Private Sub NewButtonsInitialSetup()
        '2/28/2013 - replaced old button logic
        Me.btnApplication.Attributes.Add("onclick", "btnSubmit_Click(this, 'Continuing...');") 'for disable button and server-side logic
        Me.btnCopy.Attributes.Add("onclick", "btnSubmit_Click(this, 'Copying...');") 'for disable button and server-side logic
        Me.btnIRPM.Attributes.Add("onclick", "btnSubmit_Click(this, 'Loading...');") 'for disable button and server-side logic
        Me.btnFinalize.Attributes.Add("onclick", "btnSubmit_Click(this, 'Finalizing...');") 'for disable button and server-side logic
        Me.btnPortal.Attributes.Add("onclick", "btnSubmit_Click(this, 'Loading...');") 'for disable button and server-side logic
        Me.btnPrint.Attributes.Add("onclick", "btnSubmit_Click(this, 'Loading...');") 'for disable button and server-side logic
        Me.btnQuote.Attributes.Add("onclick", "btnSubmit_Click(this, 'Loading...');") 'for disable button and server-side logic
        Me.btnRoute.Attributes.Add("onclick", "btnSubmit_Click(this, 'Routing...');") 'for disable button and server-side logic
        Me.btnExit.Attributes.Add("onclick", "btnSubmit_Click(this, 'Exiting...');") 'for disable button and server-side logic
        Me.btnNew.Attributes.Add("onclick", "btnSubmit_Click(this, 'Creating New Quote...');") 'for disable button and server-side logic
        Me.btnProposal.Attributes.Add("onclick", "btnSubmit_Click(this, 'Loading...');") 'for disable button and server-side logic; added 5/7/2013

        '2/28/2013 - replaced old button logic
        Me.lblQuoteLink.Text = ConfigurationManager.AppSettings("QuickQuote_SavedQuotes").ToString
        Me.btnQuote.Text = "Return to Saved Quotes"

        '2/28/2013 - replaced old button logic
        Me.lblPortalLink.Text = ConfigurationManager.AppSettings("DiamondPortalSavedQuotesLink").ToString

        '2/28/2013 - replaced old button logic
        Me.lblApplicationLink.Text = ""

        '2/28/2013 - replaced old button logic; really just need the 2 visible ones here since others are made invisible in normal setup
        Me.CommonButtonsRow.Visible = True
        Me.OptionsButtonsRow.Visible = False
        Me.NavButtonsRow.Visible = True
        'Me.OptionalNavSection.Visible = False '3/4/2013 - now 2 spans
        Me.OptionalNavSection1.Visible = False
        Me.OptionalNavSection2.Visible = False
        Me.ExtraButtonsRow.Visible = False

        If Request.QueryString("QuoteId") IsNot Nothing AndAlso Request.QueryString("QuoteId").ToString <> "" AndAlso IsNumeric(Request.QueryString("QuoteId").ToString) = True Then
            Me.lblQuoteId.Text = Request.QueryString("QuoteId").ToString
            Me.PrinterFriendlyLink.HRef = ConfigurationManager.AppSettings("QuickQuote_QuoteSummary").ToString & "?PrinterFriendlyQuoteId=" & Me.lblQuoteId.Text
            'Me.PrinterFriendlyRow.Visible = True
            GetQuoteFromDb_NewButtons()
        ElseIf Request.QueryString("PrinterFriendlyQuoteId") IsNot Nothing AndAlso Request.QueryString("PrinterFriendlyQuoteId").ToString <> "" AndAlso IsNumeric(Request.QueryString("PrinterFriendlyQuoteId").ToString) = True Then
            'added printer friendly functionality 8/29/2012
            Me.lblIsPrinterFriendly.Text = "YES"
            Me.lblQuoteId.Text = Request.QueryString("PrinterFriendlyQuoteId").ToString
            GetQuoteFromDb_NewButtons()
        Else
            Me.QuoteErrorSection.Visible = True
            Me.lblQuoteErrors.Text = "A valid parameter for QuoteId was not sent thru the querystring."
        End If
    End Sub
    Private Sub HideVariableFields()
        'general
        Me.LegalEntityTypeRow.Visible = False 'WC
        Me.EmployersLiabilityRow.Visible = False 'WC
        Me.ExperienceModificationRow.Visible = False 'WC
        Me.OccLiabRow.Visible = False 'BOP/GL
        Me.TenFireLiabRow.Visible = False 'BOP
        Me.PropDamLiabDeductSelectedRow.Visible = False 'BOP; added 2/14/2013
        Me.PropDamLiabDeductRow.Visible = False 'BOP
        Me.BusMasterEnhRow.Visible = False 'BOP; also GL/CAP (updated 9/24/2012)
        Me.BlanketRatingRow.Visible = False 'BOP; also CPR/CPP as-of 5/7/2013
        'added 8/27/2012
        Me.GenAggRow.Visible = False 'GL
        Me.ProdCompOpsAggRow.Visible = False 'GL
        Me.PersAndAdInjuryRow.Visible = False 'GL
        Me.DamageToPremsRentedRow.Visible = False 'GL
        Me.MedicalExpensesRow.Visible = False 'GL
        Me.GL_Deductible_HeaderRow.Visible = False 'GL
        Me.GL_Deductible_TypeRow.Visible = False 'GL
        Me.GL_Deductible_AmountRow.Visible = False 'GL
        Me.GL_Deductible_BasisRow.Visible = False 'GL
        'added 9/24/2012 for CAP
        Me.Liability_UM_UIM_LimitRow.Visible = False 'CAP
        Me.MedicalPaymentsLimitRow.Visible = False 'CAP
        'added 10/16/2012 for CPR
        Me.PolicyTypeRow.Visible = False 'CPR
        'added 11/19/2012 for CPP
        Me.PackageTypeRow.Visible = False 'CPP
        Me.PackageModAssignTypeRow.Visible = False 'CPP
        Me.Cpp_Prop_QuotedPremRow.Visible = False 'CPP
        Me.Cpp_GL_QuotedPremRow.Visible = False 'CPP
        Me.Cpp_Prop_EnhanceEndorseRow.Visible = False 'CPP
        Me.Cpp_GL_EnhanceEndorseRow.Visible = False 'CPP

        'policy level
        Me.PolicyLevelCovOptionsSpacerRow.Visible = False 'BOP/GL
        Me.PolicyLevelCovOptionsMainRow.Visible = False 'BOP/GL
        Me.PolicyLevelCovOptionsQuotedPremRow.Visible = False 'BOP/GL
        Me.AdditionalInsuredsRow.Visible = False 'BOP/GL
        Me.EmpBenLiabRow.Visible = False 'BOP/GL
        Me.EmpBenLiabOccLimitRow.Visible = False 'GL; added 8/27/2012
        Me.ContractorsEquipSelectedRow.Visible = False 'BOP; added 2/14/2013
        Me.ContractorsEquipInstallRow.Visible = False 'BOP
        Me.ContractorsEquipBlanketRow.Visible = False 'BOP
        Me.ContractorsEquipScheduledRow.Visible = False 'BOP
        Me.ContractorsEquipRentedRow.Visible = False 'BOP
        Me.ContractorsEmpToolsRow.Visible = False 'BOP

        Me.CondoDandORow.Visible = False 'CGL/CPP; added 3/6/2017

        Me.CrimeSelectedRow.Visible = False 'BOP; added 2/14/2013
        Me.CrimeEmpDishonestyRow.Visible = False 'BOP
        Me.CrimeEmpDishonestyEmpNumRow.Visible = False 'BOP
        Me.CrimeEmpDishonestyLocNumRow.Visible = False 'BOP
        Me.CrimeEmpDishonestyLimitRow.Visible = False 'BOP
        Me.CrimeForgeryRow.Visible = False 'BOP
        Me.CrimeForgeryLimitRow.Visible = False 'BOP
        Me.EarthquakeRow.Visible = False 'BOP
        Me.HiredAutoRow.Visible = False 'BOP/GL
        Me.NonOwnedAutoRow.Visible = False 'BOP/GL
        'added 8/27/2012
        Me.LiquorLiabilityMainRow.Visible = False 'GL
        Me.LiquorLiabilityOccLimitRow.Visible = False 'GL
        Me.LiquorLiabilityClassificationRow.Visible = False 'GL
        Me.LiquorLiabilitySalesRow.Visible = False 'GL
        Me.GL_ProfessionalLiabilityMainRow.Visible = False 'GL
        Me.GL_ProfessionalLiabilityCemetaryMainRow.Visible = False 'GL
        Me.GL_ProfessionalLiabilityCemetaryBurialNumRow.Visible = False 'GL
        Me.GL_ProfessionalLiabilityFuneralDirectorsMainRow.Visible = False 'GL
        Me.GL_ProfessionalLiabilityFuneralDirectorsBodyNumRow.Visible = False 'GL
        Me.GL_ProfessionalLiabilityPastoralMainRow.Visible = False 'GL
        Me.GL_ProfessionalLiabilityPastoralClergyNumRow.Visible = False 'GL
        'added 9/24/2012 for CAP
        Me.NonOwnedAutoLiabilityRow.Visible = False 'CAP
        Me.HiredAutoLiabilityRow.Visible = False 'CAP
        Me.FarmPollutionLiabilityRow.Visible = False 'CAP

        'optional policy level
        Me.AdditionalOptionalCovsSpacerRow.Visible = False 'BOP
        Me.AdditionalOptionalCovsMainRow.Visible = False 'BOP
        Me.AdditionalOptionalCovsQuotedPremRow.Visible = False 'BOP
        Me.BarbProfLiabMainRow.Visible = False 'BOP
        Me.BarbProfLiabFullTimeEmpNumRow.Visible = False 'BOP
        Me.BarbProfLiabPartTimeEmpNumRow.Visible = False 'BOP
        Me.BeautProfLiabMainRow.Visible = False 'BOP
        Me.BeautProfLiabFullTimeEmpNumRow.Visible = False 'BOP
        Me.BeautProfLiabPartTimeEmpNumRow.Visible = False 'BOP
        Me.FuneralDirectProfLiabMainRow.Visible = False 'BOP
        Me.FuneralDirectProfLiabEmpNumRow.Visible = False 'BOP
        Me.PrintersProfLiabMainRow.Visible = False 'BOP
        Me.PrintersProfLiabLocNumRow.Visible = False 'BOP
        Me.SelfStorageFacilityMainRow.Visible = False 'BOP
        Me.SelfStorageFacilityLimitRow.Visible = False 'BOP
        Me.VetProfLiabMainRow.Visible = False 'BOP
        Me.VetProfLiabEmpNumRow.Visible = False 'BOP
        Me.OptAndHearingProfLiabMainRow.Visible = False 'BOP
        Me.OptAndHearingProfLiabEmpNumRow.Visible = False 'BOP

        'This is for new BOP version but this will actually hide all of the HTML Markup so we want this to fire
        If qqHelper.doUseNewBOPVersion(quickQuote.EffectiveDate) Then
            Me.MotelGuestPropertyLimit.Visible = False
            Me.MotelSafeDepositBoxLimit.Visible = False
            Me.MotelSafeDepositBoxDeductible.Visible = False
            Me.MotelMainRow.Visible = False
            Me.MotelSafeDepositBoxMain.Visible = False
            Me.LiquorLiability2MainRow.Visible = False
            Me.LiquorLiability2AnnualGrossAlcoholSales.Visible = False
            Me.LiquorLiability2AnnualGrossPackageSalesReceipts.Visible = False
            Me.LiquorLiability2Limit.Visible = False
            'Me.FineArtsMainRow.Visible = False
            Me.RestaurantMainRow.Visible = False
            Me.ApartmentMainRow.Visible = False
            Me.ApartmentNumOfLocations.Visible = False
            'Me.CustomerAutoLegalMainRow.Visible = False
            'Me.TenantAutoLegalMainRow.Visible = False
            Me.PharmacistMainRow.Visible = False
            Me.PharmacistReceipts.Visible = False
            Me.PhotographicEquipmentMakeupAndHairMainRow.Visible = False
            Me.PhotographicEquipmentScheduleOfEquipment.Visible = False
            Me.ResidentialCleaningMainRow.Visible = False
        End If


        'Named Individuals
        Me.NamedIndividualsSpacerRow.Visible = False 'WC
        Me.NamedIndividualsMainRow.Visible = False 'WC
        Me.NamedIndividualsIncOfSolePropMainRow.Visible = False 'WC
        Me.NamedIndividualsWaiverOfSubroMainRow.Visible = False 'WC
        Me.NamedIndividualsWaiverOfSubroWaiverNumRow.Visible = False 'WC
        Me.NamedIndividualsWaiverOfSubroWaiverAmtRow.Visible = False 'WC
        Me.NamedIndividualsBlanketWaiverOfSubroRow.Visible = False 'WC 'Matt A 3-18-17 Bugs 8195/8199
        Me.NamedIndividualsExcOfAmishMainRow.Visible = False 'WC
        Me.NamedIndividualsExcOfSolePropMainRow.Visible = False 'WC

        'IRPM
        'made visible again 8/8/2012; and invisible again 8/9/2012
        Me.IrpmSpacerRow.Visible = False 'BOP/WC
        Me.IrpmMainRow.Visible = False 'BOP/WC
        Me.IrpmMgmtCoopRow.Visible = False 'BOP/WC
        Me.IrpmLocationRow.Visible = False 'BOP
        Me.IrpmBuildingFeaturesRow.Visible = False 'BOP
        Me.IrpmPremisesRow.Visible = False 'BOP/WC
        Me.IrpmEquipmentRow.Visible = False 'WC 'added 8/16/2012
        Me.IrpmEmployeesRow.Visible = False 'BOP/WC
        Me.IrpmProtectionRow.Visible = False 'BOP
        Me.IrpmCatHazRow.Visible = False 'BOP
        Me.IrpmMgmtExpRow.Visible = False 'BOP
        'added 8/16/2012
        Me.IrpmMedFacRow.Visible = False 'WC
        Me.IrpmClsPecRow.Visible = False 'WC

        'Dec (added 8/14/2012)
        Me.DecSpacerRow.Visible = False 'BOP/WC/GL/CAP
        Me.DecMainRow.Visible = False 'BOP/WC/GL/CAP
        Me.DecHeaderRow.Visible = False 'BOP/WC/GL
        Me.DecBuildingRow.Visible = False 'BOP
        Me.DecPersPropRow.Visible = False 'BOP
        Me.DecOccLiabRow.Visible = False 'BOP
        Me.DecEnhEndRow.Visible = False 'BOP/GL
        Me.DecBopOptCovsRow.Visible = False 'BOP
        Me.DecBlankRow.Visible = False 'BOP/WC/GL
        Me.DecTotalRow.Visible = False 'BOP/GL
        Me.DecBopMinPremAdjRow.Visible = False 'BOP; added 1/29/2013

        Me.DecTotEstPlanPremRow.Visible = False 'WC
        Me.DecIncreasedLimitRow.Visible = False 'WC
        Me.DecExpModRow.Visible = False 'WC
        Me.DecSchedModRow.Visible = False 'WC
        Me.DecPremDiscountRow.Visible = False 'WC
        Me.DecLossConstantRow.Visible = False 'WC
        Me.DecExpenseConstantRow.Visible = False 'WC
        Me.DecTerrorismRow.Visible = False 'WC
        Me.DecMinPremAdjRow.Visible = False 'WC
        Me.DecTotEstPremRow.Visible = False 'WC
        Me.DecSecInjFundRow.Visible = False 'WC
        Me.DecWCTotalRow.Visible = False 'WC

        'added for GL 8/28/2012
        Me.DecPremisesPremiumRow.Visible = False 'GL
        Me.DecProductsPremiumRow.Visible = False 'GL
        Me.DecGlOptCovsRow.Visible = False 'GL
        Me.DecPremisesMinPremAdjRow.Visible = False 'GL
        Me.DecProductsMinPremAdjRow.Visible = False 'GL

        'added for CAP 10/4/2012
        Me.DecHeaderRow2.Visible = False 'CAP
        Me.DecBlankRow2.Visible = False 'CAP
        Me.DecCapLiabilityRow.Visible = False 'CAP
        Me.DecCapMedicalPaymentsRow.Visible = False 'CAP
        Me.DecCap_UM_UIM_Row.Visible = False 'CAP
        Me.DecCapCompRow.Visible = False 'CAP
        Me.DecCapCollRow.Visible = False 'CAP
        Me.DecCapTowingAndLaborRow.Visible = False 'CAP
        Me.DecCapRentalReimbursementRow.Visible = False 'CAP
        Me.DecCapOptionalCoveragesRow.Visible = False 'CAP
        Me.DecCAPTotalRow.Visible = False 'CAP
        Me.DecCapMessageRow.Visible = False 'CAP

        'added 11/15/2012 for CPR
        Me.DecCprBuildingCovRow.Visible = False 'CPR
        Me.DecCprPersPropCovRow.Visible = False 'CPR
        Me.DecCprPersPropOfOthersRow.Visible = False 'CPR
        Me.DecCprBusIncomeCovRow.Visible = False 'CPR (added 11/26/2012)
        Me.DecCprPropertyInTheOpenRow.Visible = False 'CPR (added 5/6/2013)
        Me.DecCprEarthquakeRow.Visible = False 'CPR
        'added 11/16/2012 for CPR
        Me.DecEquipBreakRow.Visible = False 'CPR

        'added 11/19/2012 for CPP

        Me.Dec_CPP_TotalRow.Visible = False 'CPP

        'added 2/19/2013 for spacers at the top of each section
        Me.GeneralInformationHeaderSpacerRow.Visible = False 'all LOBs
        Me.PolicyLevelCovOptionsHeaderSpacerRow.Visible = False 'BOP, GL, CPP
        'Me.LocationHeaderSpacerRow.Visible = False
        'Me.OptionalLocationCoveragesHeaderSpacerRow.Visible = False
        'Me.BuildingHeaderSpacerRow.Visible = False
        'Me.OptionalBuildingCoveragesHeaderSpacerRow.Visible = False
        'Me.ClassCodesHeaderSpacerRow.Visible = False
        'Me.LocationGlClassCodeHeaderSpacerRow.Visible = False
        'Me.PolicyGlClassCodesHeaderSpacerRow.Visible = False
        Me.VehiclesHeaderSpacerRow.Visible = False 'CAP; only shown visible when VehicleRow is visible (this row is in sub table of that row)
        Me.AdditionalOptionalCovsHeaderSpacerRow.Visible = False 'BOP
        Me.NamedIndividualsHeaderSpacerRow.Visible = False 'WCP
        Me.IrpmHeaderSpacerRow.Visible = False 'never going to be visible as-of now
        Me.DecHeaderSpacerRow.Visible = False 'all LOBs

        ' added 2/11/15 for GL CGL1002/CGL1004 coverages Bug 4040 MGB
        Me.BlanketWaiverOfSubroRow.Visible = False
        Me.BlanketWaiverOfSubroWithCompletedOpsRow.Visible = False

    End Sub
    Private Sub ShowFieldsForEachLOB()

        'added 2/19/2013 for spacers at the top of each section
        Me.GeneralInformationHeaderSpacerRow.Visible = True
        Me.DecHeaderSpacerRow.Visible = True

        Me.ComputerRow.Visible = False ' Matt A 4-20-17 Hide by Default only show if conditions apply to BOP
        Me.PhotographicEquipmentMainRow.Visible = False ' Matt A 4-20-17 Hide by Default only show if conditions apply to BOP

        If quickQuote IsNot Nothing AndAlso quickQuote.LobType <> Nothing AndAlso quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP Then
            'general
            Me.OccLiabRow.Visible = True
            Me.TenFireLiabRow.Visible = True
            'Me.PropDamLiabDeductRow.Visible = True
            'updated 2/14/2013 (styles and cleanup)
            If UCase(Me.lblPropDamLiabilityDeductSelected.Text) = "N/A" OrElse UCase(Me.lblPropDamLiabilityDeductSelected.Text) = "NOT SELECTED" Then
                Me.PropDamLiabDeductSelectedRow.Visible = True
                Me.PropDamLiabDeductRow.Visible = False
            Else
                Me.PropDamLiabDeductSelectedRow.Visible = False
                Me.PropDamLiabDeductRow.Visible = True
            End If
            Me.BusMasterEnhRow.Visible = True
            Me.lblEnhancementEndorsementText.Text = "Business Master Enhancement" 'added 11/16/2012 to change text based off of LOB
            Me.BlanketRatingRow.Visible = True

            'policy level
            Me.PolicyLevelCovOptionsHeaderSpacerRow.Visible = True 'added 2/19/2013
            Me.PolicyLevelCovOptionsSpacerRow.Visible = True
            Me.PolicyLevelCovOptionsMainRow.Visible = True
            Me.PolicyLevelCovOptionsQuotedPremRow.Visible = True
            Me.AdditionalInsuredsRow.Visible = True
            Me.EmpBenLiabRow.Visible = True

            'Me.DECEPLIRow.Visible = quickQuote.HasEPLI '4/10/2014
            'Me.ContractorsEquipInstallRow.Visible = True
            'Me.ContractorsEquipBlanketRow.Visible = True
            'Me.ContractorsEquipScheduledRow.Visible = True
            'Me.ContractorsEquipRentedRow.Visible = True
            'Me.ContractorsEmpToolsRow.Visible = True
            'updated 2/14/2013 (styles and cleanup)
            If UCase(Me.lblContractEquipSelected.Text) = "NOT SELECTED" Then
                Me.ContractorsEquipSelectedRow.Visible = True 'new 2/14/2013
                Me.ContractorsEquipInstallRow.Visible = False
                Me.ContractorsEquipBlanketRow.Visible = False
                Me.ContractorsEquipScheduledRow.Visible = False
                Me.ContractorsEquipRentedRow.Visible = False
                Me.ContractorsEmpToolsRow.Visible = False
            Else
                Me.ContractorsEquipSelectedRow.Visible = False 'new 2/14/2013
                Me.ContractorsEquipInstallRow.Visible = True
                Me.ContractorsEquipBlanketRow.Visible = True
                Me.ContractorsEquipScheduledRow.Visible = True
                Me.ContractorsEquipRentedRow.Visible = True
                Me.ContractorsEmpToolsRow.Visible = True
            End If

            If qqHelper.doUseNewBOPVersion(quickQuote.EffectiveDate) Then
                Me.ComputerRow.Visible = True
            Else
                Me.ComputerRow.Visible = False
            End If


            'Me.CrimeEmpDishonestyRow.Visible = True
            'Me.CrimeEmpDishonestyEmpNumRow.Visible = True
            'Me.CrimeEmpDishonestyLocNumRow.Visible = True
            'Me.CrimeEmpDishonestyLimitRow.Visible = True
            ''added 2/14/2013 (styles and cleanup)
            'If Me.lblCrimeLoc.Text = "" AndAlso Me.lblCrimeEmp.Text = "" AndAlso (Me.lblCrimeLimit.Text = "" OrElse Me.lblCrimeLimit.Text = "N/A") Then
            '    Me.CrimeEmpDishonestyEmpNumRow.Visible = False
            '    Me.CrimeEmpDishonestyLocNumRow.Visible = False
            'End If
            'Me.CrimeForgeryRow.Visible = True
            'Me.CrimeForgeryLimitRow.Visible = True
            'updated 2/14/2013 (styles and cleanup)
            If UCase(Me.lblCrimeSelected.Text) = "NOT SELECTED" Then
                Me.CrimeSelectedRow.Visible = True 'new 2/14/2013
                Me.CrimeEmpDishonestyRow.Visible = False
                Me.CrimeEmpDishonestyEmpNumRow.Visible = False
                Me.CrimeEmpDishonestyLocNumRow.Visible = False
                Me.CrimeEmpDishonestyLimitRow.Visible = False
                Me.CrimeForgeryRow.Visible = False
                Me.CrimeForgeryLimitRow.Visible = False
            Else
                Me.CrimeSelectedRow.Visible = False 'new 2/14/2013
                Me.CrimeEmpDishonestyRow.Visible = True
                Me.CrimeEmpDishonestyEmpNumRow.Visible = True
                Me.CrimeEmpDishonestyLocNumRow.Visible = True
                Me.CrimeEmpDishonestyLimitRow.Visible = True
                'added 2/14/2013 (styles and cleanup)
                If Me.lblCrimeLoc.Text = "" AndAlso Me.lblCrimeEmp.Text = "" AndAlso (Me.lblCrimeLimit.Text = "" OrElse Me.lblCrimeLimit.Text = "N/A") Then
                    Me.CrimeEmpDishonestyEmpNumRow.Visible = False
                    Me.CrimeEmpDishonestyLocNumRow.Visible = False
                End If
                Me.CrimeForgeryRow.Visible = True
                Me.CrimeForgeryLimitRow.Visible = True
            End If
            Me.EarthquakeRow.Visible = True
            Me.HiredAutoRow.Visible = True
            Me.NonOwnedAutoRow.Visible = True

            'optional policy level
            Me.AdditionalOptionalCovsHeaderSpacerRow.Visible = True 'added 2/19/2013
            Me.AdditionalOptionalCovsSpacerRow.Visible = True
            Me.AdditionalOptionalCovsMainRow.Visible = True
            Me.AdditionalOptionalCovsQuotedPremRow.Visible = True
            Me.BarbProfLiabMainRow.Visible = True
            If quickQuote.HasBarbersProfessionalLiability = True Then
                Me.BarbProfLiabFullTimeEmpNumRow.Visible = True
                Me.BarbProfLiabPartTimeEmpNumRow.Visible = True
            End If
            Me.BeautProfLiabMainRow.Visible = True
            If quickQuote.HasBeauticiansProfessionalLiability = True Then
                Me.BeautProfLiabFullTimeEmpNumRow.Visible = True
                Me.BeautProfLiabPartTimeEmpNumRow.Visible = True
            End If
            Me.FuneralDirectProfLiabMainRow.Visible = True
            If quickQuote.HasFuneralDirectorsProfessionalLiability = True Then
                Me.FuneralDirectProfLiabEmpNumRow.Visible = True
            End If
            Me.PrintersProfLiabMainRow.Visible = True
            If quickQuote.HasPrintersProfessionalLiability = True Then
                Me.PrintersProfLiabLocNumRow.Visible = True
            End If
            Me.SelfStorageFacilityMainRow.Visible = True
            If quickQuote.HasSelfStorageFacility = True Then
                Me.SelfStorageFacilityLimitRow.Visible = True
            End If
            Me.VetProfLiabMainRow.Visible = True
            If quickQuote.HasVeterinariansProfessionalLiability = True Then
                Me.VetProfLiabEmpNumRow.Visible = True
            End If
            Me.OptAndHearingProfLiabMainRow.Visible = True
            If quickQuote.HasOpticalAndHearingAidProfessionalLiability = True Then
                Me.OptAndHearingProfLiabEmpNumRow.Visible = True
            End If

            If qqHelper.doUseNewBOPVersion(quickQuote.EffectiveDate) Then
                Me.MotelMainRow.Visible = True
                If quickQuote.HasMotelCoverage = True Then
                    Me.MotelGuestPropertyLimit.Visible = True
                    If Not String.IsNullOrWhiteSpace(quickQuote.MotelCoverageSafeDepositLimitId) AndAlso Not String.IsNullOrWhiteSpace(quickQuote.MotelCoverageSafeDepositDeductibleId) Then
                        Me.MotelSafeDepositBoxMain.Visible = True
                        Me.MotelSafeDepositBoxDeductible.Visible = True
                        Me.MotelSafeDepositBoxLimit.Visible = True
                    End If
                End If
                Me.LiquorLiability2MainRow.Visible = True
                If quickQuote.HasLiquorLiability = True Then
                    Me.LiquorLiability2Limit.Visible = True
                    If quickQuote.LiquorLiabilityClassCodeTypeId = 59211 Then
                        Me.LiquorLiability2AnnualGrossPackageSalesReceipts.Visible = True
                        Me.LiquorLiability2AnnualGrossAlcoholSales.Visible = False
                    Else
                        Me.LiquorLiability2AnnualGrossAlcoholSales.Visible = True
                        Me.LiquorLiability2AnnualGrossPackageSalesReceipts.Visible = False
                    End If
                End If
                'Me.FineArtsMainRow.Visible = True
                Me.RestaurantMainRow.Visible = True
                Me.ApartmentMainRow.Visible = True
                If quickQuote.HasApartmentBuildings = True Then
                    Me.ApartmentNumOfLocations.Visible = True
                End If
                'Me.CustomerAutoLegalMainRow.Visible = True
                'Me.TenantAutoLegalMainRow.Visible = True
                Me.PhotographicEquipmentMainRow.Visible = True
                If quickQuote.HasPhotographyCoverage = True Then
                    If quickQuote.HasPhotographyCoverageScheduledCoverages = True Then
                        Me.PhotographicEquipmentScheduleOfEquipment.Visible = True
                    End If
                    If quickQuote.HasPhotographyMakeupAndHair = True Then
                        Me.PhotographicEquipmentMakeupAndHairMainRow.Visible = True
                    End If
                End If
                Me.ResidentialCleaningMainRow.Visible = True
                Me.PharmacistMainRow.Visible = True
                If quickQuote.HasPharmacistProfessionalLiability = True Then
                    Me.PharmacistReceipts.Visible = True
                End If
            End If

            'Dec (added 8/14/2012)
            Me.DecSpacerRow.Visible = True 'BOP/WC/GL
            Me.DecMainRow.Visible = True 'BOP/WC/GL
            Me.DecHeaderRow.Visible = True 'BOP/WC/GL
            Me.DecBuildingRow.Visible = True 'BOP
            Me.DecPersPropRow.Visible = True 'BOP
            Me.DecOccLiabRow.Visible = True 'BOP
            Me.DecEnhEndRow.Visible = True 'BOP/GL
            Me.DecBopOptCovsRow.Visible = True 'BOP
            Me.DecBlankRow.Visible = True 'BOP/WC/GL
            Me.DecTotalRow.Visible = True 'BOP/GL
            Me.DecBopMinPremAdjRow.Visible = True 'BOP; added 1/29/2013
        ElseIf quickQuote IsNot Nothing AndAlso quickQuote.LobType <> Nothing AndAlso quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.WorkersCompensation Then
            'general
            Me.LegalEntityTypeRow.Visible = True
            Me.EmployersLiabilityRow.Visible = True
            Me.ExperienceModificationRow.Visible = True

            'Named Individuals
            Me.NamedIndividualsHeaderSpacerRow.Visible = True 'added 2/19/2013
            Me.NamedIndividualsSpacerRow.Visible = True
            Me.NamedIndividualsMainRow.Visible = True
            Me.NamedIndividualsIncOfSolePropMainRow.Visible = True
            Me.NamedIndividualsWaiverOfSubroMainRow.Visible = True
            If quickQuote.HasWaiverOfSubrogation = True Then
                Me.NamedIndividualsWaiverOfSubroWaiverNumRow.Visible = True
                Me.NamedIndividualsWaiverOfSubroWaiverAmtRow.Visible = True
            End If
            Me.NamedIndividualsBlanketWaiverOfSubroRow.Visible = True 'Matt A 3-18-17 Bugs 8195/8199
            Me.NamedIndividualsExcOfAmishMainRow.Visible = True
            Me.NamedIndividualsExcOfSolePropMainRow.Visible = True

            'Dec (added 8/14/2012)
            Me.DecSpacerRow.Visible = True 'BOP/WC/GL
            Me.DecMainRow.Visible = True 'BOP/WC/GL
            Me.DecHeaderRow.Visible = True 'BOP/WC/GL
            Me.DecTotEstPlanPremRow.Visible = True 'WC
            Me.DecIncreasedLimitRow.Visible = True 'WC
            Me.DecExpModRow.Visible = True 'WC
            Me.DecSchedModRow.Visible = True 'WC
            Me.DecPremDiscountRow.Visible = True 'WC
            Me.DecLossConstantRow.Visible = True 'WC
            Me.DecExpenseConstantRow.Visible = True 'WC
            Me.DecTerrorismRow.Visible = True 'WC
            Me.DecMinPremAdjRow.Visible = True 'WC
            Me.DecTotEstPremRow.Visible = True 'WC
            Me.DecSecInjFundRow.Visible = True 'WC
            Me.DecBlankRow.Visible = True 'BOP/WC/GL
            Me.DecWCTotalRow.Visible = True 'WC
        ElseIf quickQuote IsNot Nothing AndAlso quickQuote.LobType <> Nothing AndAlso quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability Then
            'started adding GL logic 8/27/2012; *11/19/2012 make sure to keep CPP part in sync w/ any changes
            Me.GenAggRow.Visible = True 'GL
            Me.ProdCompOpsAggRow.Visible = True 'GL
            Me.PersAndAdInjuryRow.Visible = True 'GL
            Me.OccLiabRow.Visible = True
            Me.DamageToPremsRentedRow.Visible = True 'GL
            Me.MedicalExpensesRow.Visible = True 'GL
            Me.BusMasterEnhRow.Visible = True
            Me.lblEnhancementEndorsementText.Text = "General Liability Enhancement Endorsement" 'added 11/16/2012 to change text based off of LOB
            Me.GL_Deductible_HeaderRow.Visible = True 'GL
            If quickQuote.Has_GL_PremisesAndProducts = True Then
                Me.GL_Deductible_TypeRow.Visible = True 'GL
                Me.GL_Deductible_AmountRow.Visible = True 'GL
                Me.GL_Deductible_BasisRow.Visible = True 'GL
            End If

            'policy level
            'Me.DECEPLIRow.Visible = quickQuote.HasEPLI '4/10/2014
            Me.PolicyLevelCovOptionsHeaderSpacerRow.Visible = True 'added 2/19/2013
            Me.PolicyLevelCovOptionsSpacerRow.Visible = True
            Me.PolicyLevelCovOptionsMainRow.Visible = True
            Me.PolicyLevelCovOptionsQuotedPremRow.Visible = True
            Me.AdditionalInsuredsRow.Visible = True 'BOP/GL
            Me.EmpBenLiabRow.Visible = True 'BOP/GL
            Me.EmpBenLiabOccLimitRow.Visible = True 'GL; added 8/27/2012
            'added 2/14/2013 (styles and cleanup)
            If (Me.lblEmpBenLiability.Text = "" OrElse Me.lblEmpBenLiability.Text = "N/A" OrElse UCase(Me.lblEmpBenLiability.Text) = "NOT SELECTED") AndAlso Me.lblEmpBenLiabOccLimit.Text = "" Then
                Me.EmpBenLiabOccLimitRow.Visible = False
            End If
            Me.HiredAutoRow.Visible = True
            Me.NonOwnedAutoRow.Visible = True
            Me.LiquorLiabilityMainRow.Visible = True 'GL
            If quickQuote.LiquorLiabilityOccurrenceLimitId <> "" OrElse quickQuote.LiquorLiabilityClassificationId <> "" Then
                Me.LiquorLiabilityOccLimitRow.Visible = True 'GL
                Me.LiquorLiabilityClassificationRow.Visible = True 'GL
                Me.LiquorLiabilitySalesRow.Visible = True 'GL
            End If
            Me.GL_ProfessionalLiabilityMainRow.Visible = True 'GL
            If quickQuote.ProfessionalLiabilityCemetaryNumberOfBurials <> "" OrElse quickQuote.ProfessionalLiabilityFuneralDirectorsNumberOfBodies <> "" OrElse quickQuote.ProfessionalLiabilityPastoralNumberOfClergy <> "" Then
                Me.GL_ProfessionalLiabilityCemetaryMainRow.Visible = True 'GL
                Me.GL_ProfessionalLiabilityCemetaryBurialNumRow.Visible = True 'GL
                Me.GL_ProfessionalLiabilityFuneralDirectorsMainRow.Visible = True 'GL
                Me.GL_ProfessionalLiabilityFuneralDirectorsBodyNumRow.Visible = True 'GL
                Me.GL_ProfessionalLiabilityPastoralMainRow.Visible = True 'GL
                Me.GL_ProfessionalLiabilityPastoralClergyNumRow.Visible = True 'GL
            End If

            ' added 2/11/15 for GL CGL1002/CGL1004 coverages Bug 4040 MGB
            If quickQuote.BlanketWaiverOfSubrogation IsNot Nothing AndAlso quickQuote.BlanketWaiverOfSubrogation <> String.Empty Then
                Select Case quickQuote.BlanketWaiverOfSubrogation
                    Case "", "0" ' None
                        Exit Select
                    Case "1"  ' CGL 1004
                        BlanketWaiverOfSubroRow.Visible = True
                        Exit Select
                    Case "2"  ' CGL 1002
                        BlanketWaiverOfSubroWithCompletedOpsRow.Visible = True
                        Exit Select
                    Case Else
                        Exit Select
                End Select
            End If

            'Dec (added 8/28/2012)
            Me.DecSpacerRow.Visible = True 'BOP/WC/GL
            Me.DecMainRow.Visible = True 'BOP/WC/GL
            Me.DecHeaderRow.Visible = True 'BOP/WC/GL
            Me.DecEnhEndRow.Visible = True 'BOP/GL
            Me.DecPremisesPremiumRow.Visible = True 'GL
            Me.DecProductsPremiumRow.Visible = True 'GL
            Me.DecGlOptCovsRow.Visible = True 'GL
            Me.DecPremisesMinPremAdjRow.Visible = True 'GL
            Me.DecProductsMinPremAdjRow.Visible = True 'GL
            Me.DecBlankRow.Visible = True 'BOP/WC/GL
            Me.DecTotalRow.Visible = True 'BOP/GL
        ElseIf quickQuote IsNot Nothing AndAlso quickQuote.LobType <> Nothing AndAlso quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
            'general
            Me.MedicalPaymentsLimitRow.Visible = True 'CAP
            Me.Liability_UM_UIM_LimitRow.Visible = True 'CAP
            Me.BusMasterEnhRow.Visible = True
            Me.lblEnhancementEndorsementText.Text = "Enhancement Endorsement" 'added 11/16/2012 to change text based off of LOB
            Me.CAP_BlanketWaiverOfSubroRow.Visible = True 'Matt A 3-18-17 Bugs 8195/8199

            'policy level (2/19/2013 note - still technically in general section)
            Me.NonOwnedAutoLiabilityRow.Visible = True 'CAP
            Me.HiredAutoLiabilityRow.Visible = True 'CAP
            Me.FarmPollutionLiabilityRow.Visible = True 'CAP

            'added for CAP 10/4/2012
            Me.DecSpacerRow.Visible = True 'BOP/WC/GL/CAP
            Me.DecMainRow.Visible = True 'BOP/WC/GL/CAP
            Me.DecHeaderRow2.Visible = True 'CAP
            Me.DecBlankRow2.Visible = True 'CAP
            Me.DecCapLiabilityRow.Visible = True 'CAP
            Me.DecCapMedicalPaymentsRow.Visible = True 'CAP
            Me.DecCap_UM_UIM_Row.Visible = True 'CAP
            Me.DecCapCompRow.Visible = True 'CAP
            Me.DecCapCollRow.Visible = True 'CAP
            Me.DecCapTowingAndLaborRow.Visible = True 'CAP
            Me.DecCapRentalReimbursementRow.Visible = True 'CAP
            Me.DecCapOptionalCoveragesRow.Visible = True 'CAP
            Me.DecCAPTotalRow.Visible = True 'CAP
            Me.DecCapMessageRow.Visible = True 'CAP
        ElseIf quickQuote IsNot Nothing AndAlso quickQuote.LobType <> Nothing AndAlso quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty Then
            'general; *11/19/2012 make sure to keep CPP part in sync w/ any changes
            Me.PolicyTypeRow.Visible = True 'CPR
            Me.BusMasterEnhRow.Visible = True
            Me.lblEnhancementEndorsementText.Text = "Property Enhancement Endorsement" 'added 11/16/2012 to change text based off of LOB
            Me.BlanketRatingRow.Visible = True 'added for CPR/CPP 5/7/2013; was previously just BOP

            'added for CPR 11/15/2012
            Me.DecSpacerRow.Visible = True 'BOP/WC/GL/CPR
            Me.DecMainRow.Visible = True 'BOP/WC/GL/CPR
            Me.DecHeaderRow.Visible = True 'BOP/WC/GL/CPR
            Me.DecCprBuildingCovRow.Visible = True 'CPR
            Me.DecCprPersPropCovRow.Visible = True 'CPR
            Me.DecCprPersPropOfOthersRow.Visible = True 'CPR
            Me.DecCprBusIncomeCovRow.Visible = True 'CPR (added 11/26/2012)
            Me.DecCprPropertyInTheOpenRow.Visible = True 'CPR (added 5/6/2013)
            Me.DecEnhEndRow.Visible = True 'BOP/GL/CPR
            Me.DecEquipBreakRow.Visible = True 'CPR; added 11/16/2012 for CPR
            Me.DecCprEarthquakeRow.Visible = True 'CPR
            'Me.DecPremisesPremiumRow.Visible = True 'GL
            'Me.DecProductsPremiumRow.Visible = True 'GL
            'Me.DecGlOptCovsRow.Visible = True 'GL
            'Me.DecPremisesMinPremAdjRow.Visible = True 'GL
            'Me.DecProductsMinPremAdjRow.Visible = True 'GL
            Me.DecBlankRow.Visible = True 'BOP/WC/GL/CPR
            Me.DecTotalRow.Visible = True 'BOP/GL/CPR
        ElseIf quickQuote IsNot Nothing AndAlso quickQuote.LobType <> Nothing AndAlso quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
            'general
            'added 11/19/2012 for CPP
            Me.trCpp_ContractorsEnhancement.Visible = True
            Me.trCpp_ManufacturesEnhancement.Visible = True
            Me.PackageTypeRow.Visible = True 'CPP
            Me.PackageModAssignTypeRow.Visible = True 'CPP
            Me.Cpp_Prop_QuotedPremRow.Visible = True 'CPP
            Me.Cpp_GL_QuotedPremRow.Visible = True 'CPP
            Me.Cpp_Prop_EnhanceEndorseRow.Visible = True 'CPP
            Me.Cpp_GL_EnhanceEndorseRow.Visible = True 'CPP
            '--from GL--
            Me.GenAggRow.Visible = True 'GL
            Me.ProdCompOpsAggRow.Visible = True 'GL
            Me.PersAndAdInjuryRow.Visible = True 'GL
            Me.OccLiabRow.Visible = True
            Me.DamageToPremsRentedRow.Visible = True 'GL
            Me.MedicalExpensesRow.Visible = True 'GL
            'Me.BusMasterEnhRow.Visible = True'this one's only for non-CPP; need to use CPR/GL values
            'Me.lblEnhancementEndorsementText.Text = "General Liability Enhancement Endorsement" 'added 11/16/2012 to change text based off of LOB
            Me.GL_Deductible_HeaderRow.Visible = True 'GL
            If quickQuote.Has_GL_PremisesAndProducts = True Then
                Me.GL_Deductible_TypeRow.Visible = True 'GL
                Me.GL_Deductible_AmountRow.Visible = True 'GL
                Me.GL_Deductible_BasisRow.Visible = True 'GL
            End If
            '--from Prop--
            Me.PolicyTypeRow.Visible = True 'CPR
            'Me.BusMasterEnhRow.Visible = True'this one's only for non-CPP; need to use CPR/GL values
            'Me.lblEnhancementEndorsementText.Text = "Property Enhancement Endorsement" 'added 11/16/2012 to change text based off of LOB
            Me.BlanketRatingRow.Visible = True 'added for CPR/CPP 5/7/2013; was previously just BOP

            'policy level
            'from GL
            Me.PolicyLevelCovOptionsHeaderSpacerRow.Visible = True 'added 2/19/2013
            'Me.DECEPLIRow.Visible = quickQuote.HasEPLI '4/10/2014
            Me.PolicyLevelCovOptionsSpacerRow.Visible = True
            Me.PolicyLevelCovOptionsMainRow.Visible = True
            Me.PolicyLevelCovOptionsQuotedPremRow.Visible = True
            Me.AdditionalInsuredsRow.Visible = True 'BOP/GL
            Me.EmpBenLiabRow.Visible = True 'BOP/GL
            Me.EmpBenLiabOccLimitRow.Visible = True 'GL; added 8/27/2012
            'added 2/14/2013 (styles and cleanup)
            If (Me.lblEmpBenLiability.Text = "" OrElse Me.lblEmpBenLiability.Text = "N/A" OrElse UCase(Me.lblEmpBenLiability.Text) = "NOT SELECTED") AndAlso Me.lblEmpBenLiabOccLimit.Text = "" Then
                Me.EmpBenLiabOccLimitRow.Visible = False
            End If
            Me.HiredAutoRow.Visible = True
            Me.NonOwnedAutoRow.Visible = True
            Me.LiquorLiabilityMainRow.Visible = True 'GL
            If quickQuote.LiquorLiabilityOccurrenceLimitId <> "" OrElse quickQuote.LiquorLiabilityClassificationId <> "" Then
                Me.LiquorLiabilityOccLimitRow.Visible = True 'GL
                Me.LiquorLiabilityClassificationRow.Visible = True 'GL
                Me.LiquorLiabilitySalesRow.Visible = True 'GL
            End If
            Me.GL_ProfessionalLiabilityMainRow.Visible = True 'GL
            If quickQuote.ProfessionalLiabilityCemetaryNumberOfBurials <> "" OrElse quickQuote.ProfessionalLiabilityFuneralDirectorsNumberOfBodies <> "" OrElse quickQuote.ProfessionalLiabilityPastoralNumberOfClergy <> "" Then
                Me.GL_ProfessionalLiabilityCemetaryMainRow.Visible = True 'GL
                Me.GL_ProfessionalLiabilityCemetaryBurialNumRow.Visible = True 'GL
                Me.GL_ProfessionalLiabilityFuneralDirectorsMainRow.Visible = True 'GL
                Me.GL_ProfessionalLiabilityFuneralDirectorsBodyNumRow.Visible = True 'GL
                Me.GL_ProfessionalLiabilityPastoralMainRow.Visible = True 'GL
                Me.GL_ProfessionalLiabilityPastoralClergyNumRow.Visible = True 'GL
            End If

            'Dec (added 11/19/2012 for CPP)
            Me.DecSpacerRow.Visible = True 'already used for other LOBs
            Me.DecMainRow.Visible = True 'already used for other LOBs            
            Me.Dec_CPP_TotalRow.Visible = True 'CPP
        Else
            'no lob

            Me.lblEnhancementEndorsementText.Text = "Enhancement Endorsement" 'added 11/16/2012 to change text based off of LOB

        End If
    End Sub

    Private Sub LoadValuesFromQuickQuote()
        HideVariableFields()

        If quickQuote IsNot Nothing Then
            'agencyId = ResponseData/Image/Agency/AgencyId
            'agencyCode = quickQuote.AgencyCode
            'portal URL = ResponseData/Image/PolicyBridgingUrl

            'general info
            Me.lblQuoteNumber.Text = quickQuote.QuoteNumber
            'Me.lblName.Text = quickQuote.Policyholder.Name.DisplayName.Replace(vbCrLf, "<br />&nbsp;").Replace(vbLf, "<br />&nbsp;") '&nbsp; for space added to each table column before the label
            Me.lblName.Text = quickQuote.Policyholder.Name.DisplayNameForWeb.Replace(vbCrLf, "<br />&nbsp;").Replace(vbLf, "<br />&nbsp;") '&nbsp; for space added to each table column before the label; updated 9/5/2012 to append DBA text when needed
            Me.lblQuoteDescription.Text = quickQuote.QuoteDescription
            Me.lblState.Text = quickQuote.State
            Me.lblPolicyType.Text = quickQuote.PolicyType 'added 10/16/2012 for CPR
            Me.lblEffectiveDate.Text = quickQuote.EffectiveDate 'probably need to format
            Me.lblTotalQuotedPremium.Text = quickQuote.TotalQuotedPremium
            Me.lblLegalEntityType.Text = quickQuote.Policyholder.Name.EntityType
            Me.lblEmployersLiability.Text = quickQuote.EmployersLiability
            Me.lblEmployersLiabilityQuotedPremium.Text = quickQuote.EmployersLiabilityQuotedPremium 'added 8/30/2012
            Me.lblExperienceModification.Text = quickQuote.ExperienceModificationFactor
            Me.lblExperienceModificationEffDate.Text = quickQuote.RatingEffectiveDate
            Me.lblOccLiabilityLimit.Text = quickQuote.OccurrenceLiabilityLimit
            Me.lblOccLiabilityLimitQuotedPremium.Text = quickQuote.OccurrencyLiabilityQuotedPremium 'added 8/30/2012
            Me.lblTenFireLiability.Text = quickQuote.TenantsFireLiability
            Me.lblTenFireLiabilityQuotedPremium.Text = quickQuote.TenantsFireLiabilityQuotedPremium
            Me.lblPropDamLiabilityDeduct.Text = quickQuote.PropertyDamageLiabilityDeductible
            'added 2/14/2013 (styles and cleanup)
            If Me.lblPropDamLiabilityDeduct.Text = "" Then
                Me.lblPropDamLiabilityDeduct.Text = "N/A"
            End If
            Me.lblPropDamLiabilityDeductPerClaimOrOccur.Text = quickQuote.PropertyDamageLiabilityDeductibleOption
            'added 2/14/2013 (styles and cleanup)
            If Me.lblPropDamLiabilityDeductPerClaimOrOccur.Text = "" Then
                Me.lblPropDamLiabilityDeductPerClaimOrOccur.Text = "N/A"
            End If
            'updated 2/14/2013 (styles and cleanup)
            If (Me.lblPropDamLiabilityDeduct.Text = "" OrElse Me.lblPropDamLiabilityDeduct.Text = "N/A") AndAlso (Me.lblPropDamLiabilityDeductPerClaimOrOccur.Text = "" OrElse Me.lblPropDamLiabilityDeductPerClaimOrOccur.Text = "N/A") Then
                Me.lblPropDamLiabilityDeductSelected.Text = "N/A" 'qqHelper.getSelectedOrNotSelectedText(False)
            Else
                Me.lblPropDamLiabilityDeductSelected.Text = qqHelper.getSelectedOrNotSelectedText(True)
            End If

            Me.lblLiability_UM_UIM_Limit.Text = quickQuote.Liability_UM_UIM_Limit 'added 9/24/2012 for CAP
            Me.lblLiability_UM_UIM_LimitQuotedPremium.Text = quickQuote.Liability_UM_UIM_QuotedPremium 'added 9/24/2012 for CAP
            Me.lblMedicalPaymentsLimit.Text = quickQuote.MedicalPaymentsLimit 'added 9/24/2012 for CAP
            Me.lblMedicalPaymentsLimitQuotedPremium.Text = quickQuote.MedicalPaymentsQuotedPremium 'added 9/24/2012 for CAP
            Me.lblBusMasterEnhancement.Text = qqHelper.getSelectedOrNotSelectedText(quickQuote.HasBusinessMasterEnhancement) '*11/14/2012 - will need to do something different for CPP since there are separate ones for CGL/CPR
            Me.lblBusMasterEnhancementQuotedPremium.Text = quickQuote.BusinessMasterEnhancementQuotedPremium '*11/14/2012 - will need to do something different for CPP since there are separate ones for CGL/CPR

            Me.trCpp_ContractorsEnhancement.Visible = False
            Me.trCpp_ManufacturesEnhancement.Visible = False
            Me.CAP_BlanketWaiverOfSubroRow.Visible = False 'Matt A 3-18-17 Bugs 8195/8199


            If quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty OrElse quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                Dim blanketCovCounter As Integer = 0
                Me.lblBlanketRating.Text = ""
                If quickQuote.HasBlanketBuildingAndContents = True Then
                    blanketCovCounter += 1
                    'Me.lblBlanketRating.Text = "Combined Building and Personal Property"
                    Me.lblBlanketRating.Text = qqHelper.appendText(Me.lblBlanketRating.Text, "Combined Building and Personal Property", " and ")
                End If
                If quickQuote.HasBlanketBuilding = True Then
                    blanketCovCounter += 1
                    'Me.lblBlanketRating.Text = "Building Only"
                    Me.lblBlanketRating.Text = qqHelper.appendText(Me.lblBlanketRating.Text, "Building Only", " and ")
                End If
                If quickQuote.HasBlanketContents = True Then
                    blanketCovCounter += 1
                    'Me.lblBlanketRating.Text = "Personal Property Only"
                    Me.lblBlanketRating.Text = qqHelper.appendText(Me.lblBlanketRating.Text, "Personal Property Only", " and ")
                End If
                If quickQuote.HasBlanketBusinessIncome = True Then
                    blanketCovCounter += 1
                    'Me.lblBlanketRating.Text = "Business Income"
                    Me.lblBlanketRating.Text = qqHelper.appendText(Me.lblBlanketRating.Text, "Business Income", " and ")
                End If
                If blanketCovCounter > 1 Then
                    'Me.lblBlanketRating.Text &= "  (multiple coverages)"
                ElseIf blanketCovCounter = 0 Then
                    Me.lblBlanketRating.Text = "N/A"
                End If
                Me.lblBlanketRatingQuotedPremium.Text = quickQuote.CPR_BlanketCoverages_TotalPremium
            Else
                'original logic; added IF statement 5/7/2013 to show different values when CPR/CPP
                Me.lblBlanketRating.Text = quickQuote.BlanketRatingOption
                Me.lblBlanketRatingQuotedPremium.Text = quickQuote.BlanketRatingQuotedPremium
            End If
            'added 8/27/2012
            Me.lblGenAggLimit.Text = quickQuote.GeneralAggregateLimit
            Me.lblGenAggLimitQuotedPremium.Text = quickQuote.GeneralAggregateQuotedPremium 'added 8/30/2012
            Me.lblProdCompOpsAggLimit.Text = quickQuote.ProductsCompletedOperationsAggregateLimit
            Me.lblProdCompOpsAggLimitQuotedPremium.Text = quickQuote.ProductsCompletedOperationsAggregateQuotedPremium 'added 8/30/2012
            Me.lblPersAndAdInjuryLimit.Text = quickQuote.PersonalAndAdvertisingInjuryLimit
            Me.lblPersAndAdInjuryLimitQuotedPremium.Text = quickQuote.PersonalAndAdvertisingInjuryQuotedPremium 'added 8/30/2012
            Me.lblDamageToPremsRentedLimit.Text = quickQuote.DamageToPremisesRentedLimit
            Me.lblDamageToPremsRentedLimitQuotedPremium.Text = quickQuote.DamageToPremisesRentedQuotedPremium 'added 8/30/2012
            Me.lblMedicalExpensesLimit.Text = quickQuote.MedicalExpensesLimit
            Me.lblMedicalExpensesLimitQuotedPremium.Text = quickQuote.MedicalExpensesQuotedPremium 'added 8/30/2012
            'added 10/18/2012 to give agents a better visual of figures w/ enhancment endorsment
            'If quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability AndAlso quickQuote.HasBusinessMasterEnhancement = True Then
            'updated 11/28/2012 for CPP also
            If (quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability AndAlso quickQuote.HasBusinessMasterEnhancement = True) OrElse (quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage AndAlso quickQuote.Has_PackageGL_EnhancementEndorsement = True) Then
                If IsNumeric(Me.lblDamageToPremsRentedLimit.Text) = True AndAlso CInt(Me.lblDamageToPremsRentedLimit.Text) = 100000 Then
                    Me.lblDamageToPremsRentedLimit.Text = "300000"
                    qqHelper.ConvertToLimitFormat(Me.lblDamageToPremsRentedLimit.Text)
                End If
                If IsNumeric(Me.lblMedicalExpensesLimit.Text) = True AndAlso CInt(Me.lblMedicalExpensesLimit.Text) = 5000 Then
                    Me.lblMedicalExpensesLimit.Text = "10000"
                    qqHelper.ConvertToLimitFormat(Me.lblMedicalExpensesLimit.Text)
                End If
            End If
            Me.lblGlDeduct.Text = qqHelper.getSelectedOrNotSelectedText(quickQuote.Has_GL_PremisesAndProducts)
            Me.lblGlDeductType.Text = quickQuote.GL_PremisesAndProducts_DeductibleCategoryType
            Me.lblGlDeductAmount.Text = quickQuote.GL_PremisesAndProducts_Deductible
            Me.lblGlDeductBasis.Text = quickQuote.GL_PremisesAndProducts_DeductiblePerType
            'added 11/19/2012 for CPP
            Me.lblPackageType.Text = quickQuote.PackageType
            Me.lblPackageModAssignType.Text = quickQuote.PackageModificationAssignmentType
            Me.lblCPP_Prop_QuotedPremium.Text = quickQuote.CPP_CPR_PackagePart_QuotedPremium
            Me.lblCPP_GL_QuotedPremium.Text = quickQuote.CPP_GL_PackagePart_QuotedPremium
            Me.lblCPP_Prop_EnhanceEndorse.Text = qqHelper.getSelectedOrNotSelectedText(quickQuote.Has_PackageCPR_EnhancementEndorsement)
            Me.lblCPP_Prop_EnhanceEndorseQuotedPremium.Text = quickQuote.PackageCPR_EnhancementEndorsementQuotedPremium
            Me.lblCPP_GL_EnhanceEndorse.Text = qqHelper.getSelectedOrNotSelectedText(quickQuote.Has_PackageGL_EnhancementEndorsement)
            Me.lblCPP_GL_EnhanceEndorseQuotedPremium.Text = quickQuote.PackageGL_EnhancementEndorsementQuotedPremium


            Me.lblCAP_BlanketWaiverOfSubro.Text = qqHelper.getSelectedOrNotSelectedText(quickQuote.BlanketWaiverOfSubrogation = "3") 'Matt A 3-18-17 Bugs 8195/8199
            Me.lblCAP_BlanketWaiverOfSubroPremium.Text = If(quickQuote.BlanketWaiverOfSubrogation = "3", TryToFormatAsCurrency(quickQuote.BlanketWaiverOfSubrogationQuotedPremium), "") 'Matt A 3-18-17 Bugs 8195/8199 ' Premium added to Optional Coverages Roolup

            Me.lblCpp_ContractorsEnhancement.Text = qqHelper.getSelectedOrNotSelectedText(quickQuote.HasContractorsEnhancement)
            Me.lblCpp_ContractorsEnhancementPremium.Text = quickQuote.ContractorsEnhancementQuotedPremium
            Me.lblCpp_ManufacturesEnhancement.Text = qqHelper.getSelectedOrNotSelectedText(quickQuote.HasManufacturersEnhancement)
            Me.lblCpp_ManufacturesEnhancementPremium.Text = quickQuote.ManufacturersEnhancementQuotedPremium

            Me.lblDec_CPP_Cpr_ContractorEnhancement_Prem.Text = TryToFormatAsCurrency(quickQuote.CPP_CPR_ContractorsEnhancementQuotedPremium)
            Me.lblDec_Cpp_Cpr_ManufactueEnhancement_Prem.Text = TryToFormatAsCurrency(quickQuote.CPP_CPR_ManufacturersEnhancementQuotedPremium)

            Me.lblDec_CPP_GL_ContractorEnhancement.Text = TryToFormatAsCurrency(quickQuote.CPP_CGL_ContractorsEnhancementQuotedPremium)
            Me.lblDec_CPP_GL_ManufactureEnhancement.Text = TryToFormatAsCurrency(quickQuote.CPP_CGL_ManufacturersEnhancementQuotedPremium)

            Me.trInlandMarine_Contractors_Enhancement.Visible = quickQuote.HasContractorsEnhancement



            'policy level coverage options - ResponseData/Image/LOB/PolicyLevel
            'Me.lblAddInsureds.Text = quickQuote.AdditionalInsuredsText
            If quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP AndAlso qqHelper.doUseNewBOPVersion(quickQuote.EffectiveDate) AndAlso quickQuote.AdditionalInsuredsCheckboxBOP IsNot Nothing AndAlso quickQuote.AdditionalInsuredsCheckboxBOP.Count > 0 Then
                Me.lblAddInsureds.Text = (quickQuote.AdditionalInsuredsCount + quickQuote.AdditionalInsuredsCheckboxBOP.Count).ToString()
            Else
                Me.lblAddInsureds.Text = quickQuote.AdditionalInsuredsCount.ToString
            End If
            'added 2/14/2013 (styles and cleanup)
            If Me.lblAddInsureds.Text = "" OrElse Me.lblAddInsureds.Text = "0" Then
                Me.lblAddInsureds.Text = qqHelper.getSelectedOrNotSelectedText(False)
            End If

            ' CONDO D&O 1-3-2017 MGB
            If quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability OrElse quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                If quickQuote.HasCondoDandO Then
                    CondoDandORow.Visible = True
                    Me.lblCondoDandODeductible.Text = quickQuote.CondoDandODeductible & " Deductible"
                    Me.lblCondoDandOQuotedPremium.Text = quickQuote.CondoDandOPremium
                Else
                    CondoDandORow.Visible = False
                End If
            End If

            'added for GL 8/27/2012
            'If quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability Then

            ' CONDO D&O 1-3-2017 MGB
            If quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability OrElse quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                If quickQuote.HasCondoDandO Then
                    CondoDandORow.Visible = True
                    Me.lblCondoDandODeductible.Text = quickQuote.CondoDandODeductible & " Deductible"
                    Me.lblCondoDandOQuotedPremium.Text = quickQuote.CondoDandOPremium
                Else
                    CondoDandORow.Visible = False
                End If
            Else
                CondoDandORow.Visible = False ' Matt A 4-20-17 Added so that it doesn't show on other LOBs
            End If

            'updated 11/28/2012 for CPP
            If quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability OrElse quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                If (quickQuote.AdditionalInsureds IsNot Nothing AndAlso quickQuote.AdditionalInsureds.Count > 0) OrElse quickQuote.AdditionalInsuredsCount > 0 OrElse quickQuote.AdditionalInsuredsManualCharge <> "" Then
                    'Me.lblAddInsureds.Text = "Selected"
                    'updated 2/14/2013 (styles and cleanup); just consistency here
                    Me.lblAddInsureds.Text = qqHelper.getSelectedOrNotSelectedText(True)
                Else
                    'Me.lblAddInsureds.Text = "Not Selected"
                    'updated 2/14/2013 (styles and cleanup); just consistency here
                    Me.lblAddInsureds.Text = qqHelper.getSelectedOrNotSelectedText(False)
                End If
            End If
            Me.lblAddInsuredsQuotedPremium.Text = quickQuote.AdditionalInsuredsQuotedPremium
            If quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP And qqHelper.doUseNewBOPVersion(quickQuote.EffectiveDate) Then
                If quickQuote.AdditionalInsuredsCheckboxBOP IsNot Nothing AndAlso quickQuote.AdditionalInsuredsCheckboxBOP.Count > 0 Then
                    Dim prem As Decimal = 0
                    For Each ai As QuickQuoteAdditionalInsured In quickQuote.AdditionalInsuredsCheckboxBOP
                        If Not String.IsNullOrWhiteSpace(ai.FullTermPremium) AndAlso IsNumeric(ai.FullTermPremium) AndAlso CDec(ai.FullTermPremium) <> 0 Then
                            prem += CDec(ai.FullTermPremium)
                        End If
                    Next
                    If Not String.IsNullOrWhiteSpace(lblAddInsuredsQuotedPremium.Text) AndAlso IsNumeric(lblAddInsuredsQuotedPremium.Text) AndAlso CDec(lblAddInsuredsQuotedPremium.Text) <> 0 Then
                        lblAddInsuredsQuotedPremium.Text = (CDec(lblAddInsuredsQuotedPremium.Text) + prem).ToString()
                    Else
                        lblAddInsuredsQuotedPremium.Text = prem.ToString()
                    End If
                    qqHelper.ConvertToQuotedPremiumFormat(lblAddInsuredsQuotedPremium.Text)
                End If
            End If
            Me.lblEmpBenLiability.Text = quickQuote.EmployeeBenefitsLiabilityText
            'added text appending 8/6/2012
            If IsNumeric(Me.lblEmpBenLiability.Text) = True Then
                Me.lblEmpBenLiability.Text &= " Employee" & If(CInt(Me.lblEmpBenLiability.Text) = 1, "", "s")
            End If
            'added 2/14/2013 (styles and cleanup)
            If Me.lblEmpBenLiability.Text = "" Then
                Me.lblEmpBenLiability.Text = "N/A"
            End If
            Me.lblEmpBenLiabOccLimit.Text = quickQuote.EmployeeBenefitsLiabilityOccurrenceLimit 'added 8/27/2012 for GL
            Me.lblEmpBenLiabilityQuotedPremium.Text = quickQuote.EmployeeBenefitsLiabilityQuotedPremium
            '2/14/2013 - updated EmpBenLiab text to Select/Not Selected; also logic in ShowFieldsForEachLOB (styles and cleanup)
            If (Me.lblEmpBenLiability.Text = "" OrElse Me.lblEmpBenLiability.Text = "N/A") AndAlso Me.lblEmpBenLiabOccLimit.Text = "" Then
                Me.lblEmpBenLiability.Text = qqHelper.getSelectedOrNotSelectedText(False)
            End If
            Me.lblContractEquipInstallLimit.Text = quickQuote.ContractorsEquipmentInstallationLimit
            'added 2/14/2013 (styles and cleanup)
            If Me.lblContractEquipInstallLimit.Text = "" Then
                Me.lblContractEquipInstallLimit.Text = "N/A"
            End If
            Me.lblContractEquipInstallLimitQuotedPremium.Text = quickQuote.ContractorsEquipmentInstallationLimitQuotedPremium
            Me.lblContractEquipBlanket.Text = quickQuote.ContractorsToolsEquipmentBlanket
            'added 2/14/2013 (styles and cleanup)
            If Me.lblContractEquipBlanket.Text = "" Then
                Me.lblContractEquipBlanket.Text = "N/A"
            End If
            Me.lblContractEquipBlanketQuotedPremium.Text = quickQuote.ContractorsToolsEquipmentBlanketQuotedPremium
            Me.lblContractEquipScheduled.Text = quickQuote.ContractorsToolsEquipmentScheduled
            'added 2/14/2013 (styles and cleanup)
            If Me.lblContractEquipScheduled.Text = "" Then
                Me.lblContractEquipScheduled.Text = "N/A"
            End If
            Me.lblContractEquipScheduledQuotedPremium.Text = quickQuote.ContractorsToolsEquipmentScheduledQuotedPremium
            Me.lblContractEquipRented.Text = quickQuote.ContractorsToolsEquipmentRented
            'added 2/14/2013 (styles and cleanup)
            If Me.lblContractEquipRented.Text = "" Then
                Me.lblContractEquipRented.Text = "N/A"
            End If
            Me.lblContractEquipRentedQuotedPremium.Text = quickQuote.ContractorsToolsEquipmentRentedQuotedPremium
            Me.lblContractEquipEmp.Text = quickQuote.ContractorsEmployeeTools
            'added 2/14/2013 (styles and cleanup)
            If Me.lblContractEquipEmp.Text = "" Then
                Me.lblContractEquipEmp.Text = "N/A"
            End If
            Me.lblContractEquipEmpQuotedPremium.Text = quickQuote.ContractorsEmployeeToolsQuotedPremium
            'updated 2/14/2013 (styles and cleanup)
            If (Me.lblContractEquipInstallLimit.Text = "" OrElse Me.lblContractEquipInstallLimit.Text = "N/A") AndAlso (Me.lblContractEquipBlanket.Text = "" OrElse Me.lblContractEquipBlanket.Text = "N/A") AndAlso (Me.lblContractEquipScheduled.Text = "" OrElse Me.lblContractEquipScheduled.Text = "N/A") AndAlso (Me.lblContractEquipRented.Text = "" OrElse Me.lblContractEquipRented.Text = "N/A") AndAlso (Me.lblContractEquipEmp.Text = "" OrElse Me.lblContractEquipEmp.Text = "N/A") Then
                Me.lblContractEquipSelected.Text = qqHelper.getSelectedOrNotSelectedText(False)
            Else
                Me.lblContractEquipSelected.Text = qqHelper.getSelectedOrNotSelectedText(True)
            End If
            If String.IsNullOrWhiteSpace(Me.lblComputerLimit.Text) Then
                If quickQuote.HasElectronicData = True Then
                    Me.lblComputerLimit.Text = quickQuote.ElectronicDataLimit
                    Me.lblComputerQuotedPremium.Text = quickQuote.ElectronicDataQuotedPremium
                Else
                    Me.lblComputerLimit.Text = "Not Selected"
                End If
            End If
            Me.lblCrimeEmp.Text = quickQuote.CrimeEmpDisEmployeeText
            'added text appending 8/6/2012
            If IsNumeric(Me.lblCrimeEmp.Text) = True Then
                Me.lblCrimeEmp.Text &= " Employee" & If(CInt(Me.lblCrimeEmp.Text) = 1, "", "s")
            End If
            Me.lblCrimeLoc.Text = quickQuote.CrimeEmpDisLocationText
            'added text appending 8/6/2012
            If IsNumeric(Me.lblCrimeLoc.Text) = True Then
                Me.lblCrimeLoc.Text &= " Location" & If(CInt(Me.lblCrimeLoc.Text) = 1, "", "s")
            End If
            Me.lblCrimeLimit.Text = quickQuote.CrimeEmpDisLimit
            'added 2/14/2013 (styles and cleanup)
            If Me.lblCrimeLimit.Text = "" Then
                Me.lblCrimeLimit.Text = "N/A"
            End If
            Me.lblCrimeLimitQuotedPremium.Text = quickQuote.CrimeEmpDisQuotedPremium
            Me.lblForgeLimit.Text = quickQuote.CrimeForgeryLimit
            'added 2/14/2013 (styles and cleanup)
            If Me.lblForgeLimit.Text = "" Then
                Me.lblForgeLimit.Text = "N/A"
            End If
            Me.lblForgeLimitQuotedPremium.Text = quickQuote.CrimeForgeryQuotedPremium
            'updated 2/14/2013 (styles and cleanup)
            If Me.lblCrimeLoc.Text = "" AndAlso Me.lblCrimeEmp.Text = "" AndAlso (Me.lblCrimeLimit.Text = "" OrElse Me.lblCrimeLimit.Text = "N/A") AndAlso (Me.lblForgeLimit.Text = "" OrElse Me.lblForgeLimit.Text = "N/A") Then
                Me.lblCrimeSelected.Text = qqHelper.getSelectedOrNotSelectedText(False)
            Else
                Me.lblCrimeSelected.Text = qqHelper.getSelectedOrNotSelectedText(True)
            End If
            Me.lblEarthquake.Text = qqHelper.getSelectedOrNotSelectedText(quickQuote.HasEarthquake)
            Me.lblEarthquakeQuotedPremium.Text = quickQuote.EarthquakeQuotedPremium
            Me.lblHiredAuto.Text = qqHelper.getSelectedOrNotSelectedText(quickQuote.HasHiredAuto)
            Me.lblHiredAutoQuotedPremium.Text = quickQuote.HiredAutoQuotedPremium
            Me.lblNonOwnedAuto.Text = qqHelper.getSelectedOrNotSelectedText(quickQuote.HasNonOwnedAuto)
            Me.lblNonOwnedAutoQuotedPremium.Text = quickQuote.NonOwnedAutoQuotedPremium
            'added 8/27/2012 for GL
            If quickQuote.LiquorLiabilityOccurrenceLimitId <> "" OrElse quickQuote.LiquorLiabilityClassificationId <> "" Then
                'Me.lblLiquorLiability.Text = "Selected"
                'updated 2/14/2013 (styles and cleanup); just consistency here
                Me.lblLiquorLiability.Text = qqHelper.getSelectedOrNotSelectedText(True)
            Else
                'Me.lblLiquorLiability.Text = "Not Selected"
                'updated 2/14/2013 (styles and cleanup); just consistency here
                Me.lblLiquorLiability.Text = qqHelper.getSelectedOrNotSelectedText(False)
            End If
            Me.lblLiquorLiabilityQuotedPremium.Text = quickQuote.LiquorLiabilityQuotedPremium
            Me.lblLiquorLiabilityOccLimit.Text = quickQuote.LiquorLiabilityOccurrenceLimit
            Me.lblLiquorLiabilityClassification.Text = quickQuote.LiquorLiabilityClassification
            Me.lblLiquorLiabilitySales.Text = quickQuote.LiquorSales
            qqHelper.ConvertToQuotedPremiumFormat(Me.lblLiquorLiabilitySales.Text) 'to format
            If quickQuote.ProfessionalLiabilityCemetaryNumberOfBurials <> "" OrElse quickQuote.ProfessionalLiabilityFuneralDirectorsNumberOfBodies <> "" OrElse quickQuote.ProfessionalLiabilityPastoralNumberOfClergy <> "" Then
                'Me.lbl_GL_ProfessionalLiability.Text = "Selected"
                'updated 2/14/2013 (styles and cleanup); just consistency here
                Me.lbl_GL_ProfessionalLiability.Text = qqHelper.getSelectedOrNotSelectedText(True)
            Else
                'Me.lbl_GL_ProfessionalLiability.Text = "Not Selected"
                'updated 2/14/2013 (styles and cleanup); just consistency here
                Me.lbl_GL_ProfessionalLiability.Text = qqHelper.getSelectedOrNotSelectedText(False)
            End If
            Me.lbl_GL_ProfLiabCemBurialNum.Text = quickQuote.ProfessionalLiabilityCemetaryNumberOfBurials
            Me.lbl_GL_ProfLiabCemQuotedPremium.Text = quickQuote.ProfessionalLiabilityCemetaryQuotedPremium 'added 8/30/2012
            Me.lbl_GL_ProfLiabFunDirBodyNum.Text = quickQuote.ProfessionalLiabilityFuneralDirectorsNumberOfBodies
            Me.lbl_GL_ProfLiabFunDirQuotedPremium.Text = quickQuote.FuneralDirectorsProfessionalLiabilityQuotedPremium 'added 8/30/2012
            Me.lbl_GL_ProfLiabPastClergyNum.Text = quickQuote.ProfessionalLiabilityPastoralNumberOfClergy
            Me.lbl_GL_ProfLiabPastoralQuotedPremium.Text = quickQuote.ProfessionalLiabilityPastoralQuotedPremium 'added 8/30/2012
            'added 9/24/2012 for CAP
            Me.lblNonOwnedAutoLiability.Text = qqHelper.getSelectedOrNotSelectedText(quickQuote.HasNonOwnershipLiability)
            Me.lblNonOwnedAutoLiabilityQuotedPremium.Text = quickQuote.NonOwnershipLiabilityQuotedPremium
            Me.lblHiredAutoLiability.Text = qqHelper.getSelectedOrNotSelectedText(quickQuote.HasHiredBorrowedLiability)
            Me.lblHiredAutoLiabilityQuotedPremium.Text = quickQuote.HiredBorrowedLiabilityQuotedPremium
            Me.lblFarmPollutionLiability.Text = qqHelper.getSelectedOrNotSelectedText(quickQuote.HasFarmPollutionLiability)
            Me.lblFarmPollutionLiabilityQuotedPremium.Text = quickQuote.FarmPollutionLiabilityQuotedPremium

            If quickQuote.Locations IsNot Nothing AndAlso quickQuote.Locations.Count > 0 Then
                Me.LocationInformationRepeaterRow.Visible = True

                Dim dt As New DataTable
                'location info (can have multiple) - ResponseData/Image/LOB/RiskLevel/Locations/Location
                dt.Columns.Add("LocIndex", System.Type.GetType("System.String"))
                dt.Columns.Add("LocDescription", System.Type.GetType("System.String"))
                dt.Columns.Add("LocName", System.Type.GetType("System.String"))
                dt.Columns.Add("LocHouseNum", System.Type.GetType("System.String"))
                dt.Columns.Add("LocStreet", System.Type.GetType("System.String"))
                dt.Columns.Add("LocAptNum", System.Type.GetType("System.String")) 'added 8/27/2012
                dt.Columns.Add("LocPoBox", System.Type.GetType("System.String"))
                dt.Columns.Add("LocCity", System.Type.GetType("System.String"))
                dt.Columns.Add("LocState", System.Type.GetType("System.String"))
                dt.Columns.Add("LocZip", System.Type.GetType("System.String"))
                dt.Columns.Add("LocCounty", System.Type.GetType("System.String"))
                dt.Columns.Add("LocProtClass", System.Type.GetType("System.String"))
                dt.Columns.Add("LocNumOfPools", System.Type.GetType("System.String"))
                dt.Columns.Add("LocPoolsQuotedPremium", System.Type.GetType("System.String"))
                'If qqHelper.doUseNewBOPVersion(quickQuote.EffectiveDate) Then
                dt.Columns.Add("LocNumOfAmusements", System.Type.GetType("System.String"))
                dt.Columns.Add("LocAmusementsQuotedPremium", System.Type.GetType("System.String"))
                dt.Columns.Add("LocNumOfPlaygrounds", System.Type.GetType("System.String"))
                dt.Columns.Add("LocPlaygroundsQuotedPremium", System.Type.GetType("System.String"))
                'End If

                'optional location coverages
                dt.Columns.Add("EquipBreakDeductSelected", System.Type.GetType("System.String")) 'added 2/14/2013 (styles and cleanup)
                dt.Columns.Add("EquipBreakDeduct", System.Type.GetType("System.String"))
                dt.Columns.Add("EquipBreakDeductQuotedPremium", System.Type.GetType("System.String"))
                dt.Columns.Add("MoneyOnPremSelected", System.Type.GetType("System.String")) 'added 2/14/2013 (styles and cleanup)
                dt.Columns.Add("MoneyOnPrem", System.Type.GetType("System.String"))
                dt.Columns.Add("MoneyOffPremSelected", System.Type.GetType("System.String")) 'added 2/14/2013 (styles and cleanup)
                dt.Columns.Add("MoneyOffPrem", System.Type.GetType("System.String"))
                dt.Columns.Add("MoneyOnOffPremQuotedPremium", System.Type.GetType("System.String"))
                dt.Columns.Add("OutdoorSignsSelected", System.Type.GetType("System.String")) 'added 2/14/2013 (styles and cleanup)
                dt.Columns.Add("OutdoorSigns", System.Type.GetType("System.String"))
                dt.Columns.Add("OutdoorSignsQuotedPremium", System.Type.GetType("System.String"))
                'If qqHelper.doUseNewBOPVersion(quickQuote.EffectiveDate) Then
                dt.Columns.Add("FineArtsSelected", System.Type.GetType("System.String"))
                dt.Columns.Add("FineArtsQuotedPremium", System.Type.GetType("System.String"))
                dt.Columns.Add("CustomerAutoLegalLimit", System.Type.GetType("System.String"))
                dt.Columns.Add("CustomerAutoLegalDeductible", System.Type.GetType("System.String"))
                dt.Columns.Add("CustomerAutoLegalSelected", System.Type.GetType("System.String"))
                dt.Columns.Add("CustomerAutoLegalQuotedPremium", System.Type.GetType("System.String"))
                dt.Columns.Add("TenantAutoLegalLimit", System.Type.GetType("System.String"))
                dt.Columns.Add("TenantAutoLegalDeductible", System.Type.GetType("System.String"))
                dt.Columns.Add("TenantAutoLegalSelected", System.Type.GetType("System.String"))
                dt.Columns.Add("TenantAutoLegalQuotedPremium", System.Type.GetType("System.String"))
                'End If

                For i As Integer = 0 To quickQuote.Locations.Count - 1
                    Dim newRow As DataRow = dt.NewRow
                    newRow.Item("LocIndex") = i
                    newRow.Item("LocDescription") = quickQuote.Locations(i).Description
                    'newRow.Item("LocName") = quickQuote.Locations(i).Name.DisplayName.Replace(vbCrLf, "<br />&nbsp;").Replace(vbLf, "<br />&nbsp;") '&nbsp; for space added to each table column before the label
                    newRow.Item("LocName") = quickQuote.Locations(i).Name.DisplayNameForWeb.Replace(vbCrLf, "<br />&nbsp;").Replace(vbLf, "<br />&nbsp;") '&nbsp; for space added to each table column before the label; updated 9/5/2012 to append DBA text when needed
                    newRow.Item("LocHouseNum") = quickQuote.Locations(i).Address.HouseNum
                    newRow.Item("LocStreet") = quickQuote.Locations(i).Address.StreetName
                    newRow.Item("LocAptNum") = quickQuote.Locations(i).Address.ApartmentNumber
                    newRow.Item("LocPoBox") = quickQuote.Locations(i).Address.POBox
                    newRow.Item("LocCity") = quickQuote.Locations(i).Address.City
                    newRow.Item("LocState") = quickQuote.Locations(i).Address.State
                    newRow.Item("LocZip") = quickQuote.Locations(i).Address.Zip
                    newRow.Item("LocCounty") = quickQuote.Locations(i).Address.County
                    newRow.Item("LocProtClass") = quickQuote.Locations(i).ProtectionClass
                    newRow.Item("LocNumOfPools") = quickQuote.Locations(i).NumberOfPools
                    newRow.Item("LocPoolsQuotedPremium") = quickQuote.Locations(i).PoolsQuotedPremium
                    If qqHelper.doUseNewBOPVersion(quickQuote.EffectiveDate) Then
                        newRow.Item("LocNumOfPlaygrounds") = quickQuote.Locations(i).NumberOfPlaygrounds
                        newRow.Item("LocAmusementsQuotedPremium") = quickQuote.Locations(i).AmusementAreasQuotedPremium
                        newRow.Item("LocNumOfAmusements") = quickQuote.Locations(i).NumberOfAmusementAreas
                        newRow.Item("LocPlaygroundsQuotedPremium") = quickQuote.Locations(i).PlaygroundsQuotedPremium
                    End If

                    '2/14/2013 - added IF statements to set some fields to N/A if blank (styles and cleanup)
                    'newRow.Item("EquipBreakDeduct") = quickQuote.Locations(i).EquipmentBreakdownDeductible
                    newRow.Item("EquipBreakDeduct") = If(quickQuote.Locations(i).EquipmentBreakdownDeductible <> "", quickQuote.Locations(i).EquipmentBreakdownDeductible, "N/A")
                    newRow.Item("EquipBreakDeductSelected") = If(quickQuote.Locations(i).EquipmentBreakdownDeductible = "" OrElse quickQuote.Locations(i).EquipmentBreakdownDeductible = "N/A", qqHelper.getSelectedOrNotSelectedText(False), qqHelper.getSelectedOrNotSelectedText(True)) 'added 2/14/2013 (styles and cleanup)
                    newRow.Item("EquipBreakDeductQuotedPremium") = quickQuote.Locations(i).EquipmentBreakdownDeductibleQuotedPremium
                    'newRow.Item("MoneyOnPrem") = quickQuote.Locations(i).MoneySecuritiesOnPremises
                    newRow.Item("MoneyOnPrem") = If(quickQuote.Locations(i).MoneySecuritiesOnPremises <> "", quickQuote.Locations(i).MoneySecuritiesOnPremises, "N/A")
                    newRow.Item("MoneyOnPremSelected") = If(quickQuote.Locations(i).MoneySecuritiesOnPremises = "" OrElse quickQuote.Locations(i).MoneySecuritiesOnPremises = "N/A", qqHelper.getSelectedOrNotSelectedText(False), qqHelper.getSelectedOrNotSelectedText(True)) 'added 2/14/2013 (styles and cleanup)
                    'newRow.Item("MoneyOffPrem") = quickQuote.Locations(i).MoneySecuritiesOffPremises
                    newRow.Item("MoneyOffPrem") = If(quickQuote.Locations(i).MoneySecuritiesOffPremises <> "", quickQuote.Locations(i).MoneySecuritiesOffPremises, "N/A")
                    newRow.Item("MoneyOffPremSelected") = If(quickQuote.Locations(i).MoneySecuritiesOffPremises = "" OrElse quickQuote.Locations(i).MoneySecuritiesOffPremises = "N/A", qqHelper.getSelectedOrNotSelectedText(False), qqHelper.getSelectedOrNotSelectedText(True)) 'added 2/14/2013 (styles and cleanup)
                    newRow.Item("MoneyOnOffPremQuotedPremium") = quickQuote.Locations(i).MoneySecuritiesQuotedPremium
                    'newRow.Item("OutdoorSigns") = quickQuote.Locations(i).OutdoorSignsLimit
                    newRow.Item("OutdoorSigns") = If(quickQuote.Locations(i).OutdoorSignsLimit <> "", quickQuote.Locations(i).OutdoorSignsLimit, "N/A")
                    newRow.Item("OutdoorSignsSelected") = If(quickQuote.Locations(i).OutdoorSignsLimit = "" OrElse quickQuote.Locations(i).OutdoorSignsLimit = "N/A", qqHelper.getSelectedOrNotSelectedText(False), qqHelper.getSelectedOrNotSelectedText(True)) 'added 2/14/2013 (styles and cleanup)
                    newRow.Item("OutdoorSignsQuotedPremium") = quickQuote.Locations(i).OutdoorSignsQuotedPremium


                    If qqHelper.doUseNewBOPVersion(quickQuote.EffectiveDate) Then
                        newRow.Item("FineArtsSelected") = qqHelper.getSelectedOrNotSelectedText(quickQuote.Locations(i).HasFineArts)
                        If quickQuote.Locations(i).HasFineArts = True Then
                            newRow.Item("FineArtsQuotedPremium") = quickQuote.Locations(i).FineArtsQuotedPremium
                        End If

                        newRow.Item("CustomerAutoLegalSelected") = qqHelper.getSelectedOrNotSelectedText(quickQuote.Locations(i).HasCustomerAutoLegalLiability)
                        If quickQuote.Locations(i).HasCustomerAutoLegalLiability = True Then
                            newRow.Item("CustomerAutoLegalQuotedPremium") = quickQuote.Locations(i).CustomerAutoLegalLiabilityQuotedPremium
                            newRow.Item("CustomerAutoLegalLimit") = quickQuote.Locations(i).CustomerAutoLegalLiabilityLimitOfLiability
                            newRow.Item("CustomerAutoLegalDeductible") = quickQuote.Locations(i).CustomerAutoLegalLiabilityDeductible
                        End If

                        newRow.Item("TenantAutoLegalSelected") = qqHelper.getSelectedOrNotSelectedText(quickQuote.Locations(i).HasTenantAutoLegalLiability)
                        If quickQuote.Locations(i).HasTenantAutoLegalLiability = True Then
                            newRow.Item("TenantAutoLegalQuotedPremium") = quickQuote.Locations(i).TenantAutoLegalLiabilityQuotedPremium
                            newRow.Item("TenantAutoLegalLimit") = quickQuote.Locations(i).TenantAutoLegalLiabilityLimitOfLiability
                            newRow.Item("TenantAutoLegalDeductible") = quickQuote.Locations(i).TenantAutoLegalLiabilityDeductible
                        End If
                    End If
                    dt.Rows.Add(newRow)
                Next

                Me.rptLocations.DataSource = dt
                Me.rptLocations.DataBind()
            Else
                Me.LocationInformationRepeaterRow.Visible = False
            End If

            'added 8/27/2012
            If quickQuote.GLClassifications IsNot Nothing AndAlso quickQuote.GLClassifications.Count > 0 Then
                Me.GL_ClassCodesInformationRepeaterRow.Visible = True
                'Me.PolicyGlClassCodesHeaderSpacerRow.Visible = True 'added 2/19/2013; not used since on header row

                Dim dt As New DataTable
                dt.Columns.Add("ClassCode", System.Type.GetType("System.String"))
                dt.Columns.Add("ClassCodeDescription", System.Type.GetType("System.String"))
                dt.Columns.Add("ClassCodePremExp", System.Type.GetType("System.String"))
                dt.Columns.Add("ClassCodePremBase", System.Type.GetType("System.String"))
                dt.Columns.Add("ClassCodeProductsPrem", System.Type.GetType("System.String"))
                dt.Columns.Add("ClassCodePremisesPrem", System.Type.GetType("System.String"))

                For Each cls As QuickQuoteGLClassification In quickQuote.GLClassifications
                    Dim newRow As DataRow = dt.NewRow
                    newRow.Item("ClassCode") = cls.ClassCode
                    newRow.Item("ClassCodeDescription") = cls.ClassDescription
                    newRow.Item("ClassCodePremExp") = cls.PremiumExposure
                    newRow.Item("ClassCodePremBase") = cls.PremiumBase
                    newRow.Item("ClassCodeProductsPrem") = cls.ProductsQuotedPremium
                    newRow.Item("ClassCodePremisesPrem") = cls.PremisesQuotedPremium
                    dt.Rows.Add(newRow)
                Next

                Me.rpt_GL_ClassCodes.DataSource = dt
                Me.rpt_GL_ClassCodes.DataBind()
            Else
                Me.GL_ClassCodesInformationRepeaterRow.Visible = False
                'Me.PolicyGlClassCodesHeaderSpacerRow.Visible = False 'added 2/19/2013; not used since on header row
            End If

            'added 9/24/2012
            If quickQuote.Vehicles IsNot Nothing AndAlso quickQuote.Vehicles.Count > 0 Then
                Me.VehiclesRow.Visible = True
                Me.VehiclesHeaderSpacerRow.Visible = True 'added 2/19/2013

                Dim dt As New DataTable
                dt.Columns.Add("VehicleNum", System.Type.GetType("System.String"))
                dt.Columns.Add("Year", System.Type.GetType("System.String"))
                dt.Columns.Add("Make", System.Type.GetType("System.String"))
                dt.Columns.Add("Model", System.Type.GetType("System.String"))
                dt.Columns.Add("CostNew", System.Type.GetType("System.String"))
                dt.Columns.Add("Class", System.Type.GetType("System.String"))
                dt.Columns.Add("LiabPrem", System.Type.GetType("System.String"))
                dt.Columns.Add("MedPrem", System.Type.GetType("System.String"))
                dt.Columns.Add("CompDed", System.Type.GetType("System.String"))
                dt.Columns.Add("CompPrem", System.Type.GetType("System.String"))
                dt.Columns.Add("CollDed", System.Type.GetType("System.String"))
                dt.Columns.Add("CollPrem", System.Type.GetType("System.String"))
                dt.Columns.Add("Rntl", System.Type.GetType("System.String"))
                dt.Columns.Add("Tow", System.Type.GetType("System.String"))
                dt.Columns.Add("TotlPrem", System.Type.GetType("System.String"))
                dt.Columns.Add("Terr#", System.Type.GetType("System.String"))

                Dim vCounter As Integer = 0
                For Each v As QuickQuoteVehicle In quickQuote.Vehicles
                    vCounter += 1
                    Dim newRow As DataRow = dt.NewRow
                    newRow.Item("VehicleNum") = vCounter.ToString
                    newRow.Item("Year") = v.Year
                    newRow.Item("Make") = v.Make
                    newRow.Item("Model") = v.Model
                    newRow.Item("CostNew") = v.CostNew
                    newRow.Item("Class") = v.ClassCode
                    newRow.Item("LiabPrem") = If(v.HasLiability_UM_UIM = True, v.Liability_UM_UIM_QuotedPremium, "NS")
                    newRow.Item("MedPrem") = If(v.HasMedicalPayments = True, v.MedicalPaymentsQuotedPremium, "NS")
                    newRow.Item("CompDed") = If(v.HasComprehensive = True, v.ComprehensiveDeductible, "NS")
                    newRow.Item("CompPrem") = If(v.HasComprehensive = True, v.ComprehensiveQuotedPremium, "NS")
                    newRow.Item("CollDed") = If(v.HasCollision = True, v.CollisionDeductible, "NS")
                    newRow.Item("CollPrem") = If(v.HasCollision = True, v.CollisionQuotedPremium, "NS")
                    newRow.Item("Rntl") = If(v.HasRentalReimbursement = True, v.RentalReimbursementQuotedPremium, "NS")
                    newRow.Item("Tow") = If(v.HasTowingAndLabor = True, v.TowingAndLaborQuotedPremium, "NS")
                    newRow.Item("TotlPrem") = v.PremiumFullTerm
                    newRow.Item("Terr#") = v.TerritoryNum
                    dt.Rows.Add(newRow)
                Next

                Me.dgrdVehicles.DataSource = dt
                Me.dgrdVehicles.DataBind()
            Else
                Me.VehiclesRow.Visible = False
                Me.VehiclesHeaderSpacerRow.Visible = False 'added 2/19/2013
            End If


            'additional optional coverages
            Me.lblBarbProfLiab.Text = qqHelper.getSelectedOrNotSelectedText(quickQuote.HasBarbersProfessionalLiability)
            Me.lblBarbProfLiabQuotedPremium.Text = quickQuote.BarbersProfessionalLiabiltyQuotedPremium
            Me.lblBarbProfLiabFullEmpNum.Text = quickQuote.BarbersProfessionalLiabilityFullTimeEmpNum
            Me.lblBarbProfLiabPartEmpNum.Text = quickQuote.BarbersProfessionalLiabilityPartTimeEmpNum
            Me.lblBeautProfLiab.Text = qqHelper.getSelectedOrNotSelectedText(quickQuote.HasBeauticiansProfessionalLiability)
            Me.lblBeautProfLiabQuotedPremium.Text = quickQuote.BeauticiansProfessionalLiabilityQuotedPremium
            Me.lblBeautProfLiabFullEmpNum.Text = quickQuote.BeauticiansProfessionalLiabilityFullTimeEmpNum
            Me.lblBeautProfLiabPartEmpNum.Text = quickQuote.BeauticiansProfessionalLiabilityPartTimeEmpNum
            Me.lblFunDirProfLiab.Text = qqHelper.getSelectedOrNotSelectedText(quickQuote.HasFuneralDirectorsProfessionalLiability)
            Me.lblFunDirProfLiabQuotedPremium.Text = quickQuote.FuneralDirectorsProfessionalLiabilityQuotedPremium
            Me.lblFunDirProfLiabEmpNum.Text = quickQuote.FuneralDirectorsProfessionalLiabilityEmpNum
            Me.lblPrintProfLiab.Text = qqHelper.getSelectedOrNotSelectedText(quickQuote.HasPrintersProfessionalLiability)
            Me.lblPrintProfLiabQuotedPremium.Text = quickQuote.PrintersProfessionalLiabilityQuotedPremium
            Me.lblPrintProfLiabLocNum.Text = quickQuote.PrintersProfessionalLiabilityLocNum
            Me.lblSelfStoreFac.Text = qqHelper.getSelectedOrNotSelectedText(quickQuote.HasSelfStorageFacility)
            Me.lblSelfStoreFacQuotedPremium.Text = quickQuote.SelfStorageFacilityQuotedPremium
            'Me.lblSelfStoreFacLocNum.Text = quickQuote.SelfStorageFacilityLocNum'removed 7/3/2012
            Me.lblSelfStoreFacLimit.Text = quickQuote.SelfStorageFacilityLimit 'added 7/3/2012
            'added 7/3/2012
            Me.lblVetProfLiab.Text = qqHelper.getSelectedOrNotSelectedText(quickQuote.HasVeterinariansProfessionalLiability)
            Me.lblVetProfLiabQuotedPremium.Text = quickQuote.VeterinariansProfessionalLiabilityQuotedPremium
            Me.lblVetProfLiabEmpNum.Text = quickQuote.VeterinariansProfessionalLiabilityEmpNum
            Me.lblOptAndHearProfLiab.Text = qqHelper.getSelectedOrNotSelectedText(quickQuote.HasOpticalAndHearingAidProfessionalLiability)
            Me.lblOptAndHearProfLiabQuotedPremium.Text = quickQuote.OpticalAndHearingAidProfessionalLiabilityQuotedPremium
            Me.lblOptAndHearProfLiabEmpNum.Text = quickQuote.OpticalAndHearingAidProfessionalLiabilityEmpNum

            If qqHelper.doUseNewBOPVersion(quickQuote.EffectiveDate) Then
                Me.lblMotel.Text = qqHelper.getSelectedOrNotSelectedText(quickQuote.HasMotelCoverage)
                Me.lblMotelQuotedPremium.Text = quickQuote.MotelCoveragePerGuestQuotedPremium
                Me.lblMotelSafeDepositBoxQuotedPremium.Text = quickQuote.MotelCoverageSafeDepositQuotedPremium
                Me.lblMotelGuestPropertyLimit.Text = quickQuote.MotelCoveragePerGuestLimit
                Me.lblMotelSafeDepositBoxDeductible.Text = quickQuote.MotelCoverageSafeDepositDeductible
                Me.lblMotelSafeDepositBoxLimit.Text = quickQuote.MotelCoverageSafeDepositLimit
                Me.lblMotelSafeDepositBox.Text = qqHelper.getSelectedOrNotSelectedText(quickQuote.HasMotelCoverage AndAlso quickQuote.MotelCoverageSafeDepositDeductible > 0 AndAlso quickQuote.MotelCoverageSafeDepositLimit > 0)
                Me.lblLiquorLiability2.Text = qqHelper.getSelectedOrNotSelectedText(quickQuote.HasLiquorLiability)
                If quickQuote.HasLiquorLiability = True Then 'Premium Was coming back as 0 instead of nothing. Didn't want to mess up existing code so put in this IF statement.
                    Me.lblLiquorLiability2QuotedPremium.Text = quickQuote.LiquorLiabilityQuotedPremium
                Else
                    Me.lblLiquorLiability2QuotedPremium.Text = ""
                End If
                Me.lblLiquorLiability2AnnualGrossAlcoholSales.Text = quickQuote.LiquorLiabilityAnnualGrossAlcoholSalesReceipts
                Me.lblLiquorLiability2AnnualGrossPackageSalesReceipts.Text = quickQuote.LiquorLiabilityAnnualGrossPackageSalesReceipts
                Me.lblLiquorLiability2Limit.Text = quickQuote.OccurrenceLiabilityLimit
                Me.lblRestaurant.Text = qqHelper.getSelectedOrNotSelectedText(quickQuote.HasRestaurantEndorsement)
                Me.lblRestaurantQuotedPremium.Text = quickQuote.RestaurantQuotedPremium
                Me.lblApartment.Text = qqHelper.getSelectedOrNotSelectedText(quickQuote.HasApartmentBuildings)
                Me.lblApartmentQuotedPremium.Text = quickQuote.ApartmentQuotedPremium
                Me.lblApartmentNumOfLocs.Text = quickQuote.NumberOfLocationsWithApartments
                If IsNumeric(quickQuote.NumberOfLocationsWithApartments) AndAlso CInt(quickQuote.NumberOfLocationsWithApartments) = 1 Then
                    Me.lblApartmentNumOfLocsTitle.InnerText = "Location with Apartments"
                End If

                Me.lblPhotographicEquipment.Text = qqHelper.getSelectedOrNotSelectedText(quickQuote.HasPhotographyCoverage)
                Me.lblPhotographicEquipmentQuotedPremium.Text = quickQuote.PhotographyCoverageQuotedPremium
                Me.lblPhotographicEquipmentMakeupAndHair.Text = qqHelper.getSelectedOrNotSelectedText(quickQuote.HasPhotographyMakeupAndHair)
                Me.lblPhotographicEquipmentMakeupAndHairQuotedPremium.Text = quickQuote.PhotographyMakeupAndHairQuotedPremium
                Me.lblPhotographicEquipmentScheduleOfEquipment.Text = quickQuote.PhotographyTotalScheduledLimits
                Me.lblPharmacist.Text = qqHelper.getSelectedOrNotSelectedText(quickQuote.HasPharmacistProfessionalLiability)
                Me.lblPharmacistQuotedPremium.Text = quickQuote.PharmacistQuotedPremium
                Me.lblPharmacistReceipts.Text = quickQuote.PharmacistAnnualGrossSales
                Me.lblResidentialCleaning.Text = qqHelper.getSelectedOrNotSelectedText(quickQuote.HasResidentialCleaning)
                Me.lblResidentialCleaningQuotedPremium.Text = quickQuote.ResidentialCleaningQuotedPremium
            End If

            'Named Individuals
            Me.lblNamedIndividualsIncOfSoleProp.Text = qqHelper.getSelectedOrNotSelectedText(quickQuote.HasInclusionOfSoleProprietorsPartnersOfficersAndOthers)
            Me.lblNamedIndividualsWaiverOfSubro.Text = qqHelper.getSelectedOrNotSelectedText(quickQuote.HasWaiverOfSubrogation)
            Me.lblNamedIndividualsWaiverOfSubroWaiverNum.Text = quickQuote.WaiverOfSubrogationNumberOfWaivers
            Me.lblNamedIndividualsWaiverOfSubroWaiverAmt.Text = If(quickQuote.BlanketWaiverOfSubrogation = "4", "0", quickQuote.WaiverOfSubrogationPremium)
            Me.lblNamedIndividualsBlanketWaiverOfSubro.Text = qqHelper.getSelectedOrNotSelectedText(quickQuote.BlanketWaiverOfSubrogation = "4") 'Matt A 3-18-17 Bugs 8195/8199
            Me.lblNamedIndividualsExcOfAmish.Text = qqHelper.getSelectedOrNotSelectedText(quickQuote.HasExclusionOfAmishWorkers)
            Me.lblNamedIndividualsExcOfSoleProp.Text = qqHelper.getSelectedOrNotSelectedText(quickQuote.HasExclusionOfSoleProprietorsPartnersOfficersAndOthers)

            'IRPM
            Me.lblIRPM_MgmtCoop.Text = quickQuote.IRPM_ManagementCooperation
            Me.lblIRPM_MgmtCoopDesc.Text = quickQuote.IRPM_ManagementCooperationDesc
            Me.lblIRPM_Loc.Text = quickQuote.IRPM_Location
            Me.lblIRPM_LocDesc.Text = quickQuote.IRPM_LocationDesc
            Me.lblIRPM_BuildFeat.Text = quickQuote.IRPM_BuildingFeatures
            Me.lblIRPM_BuildFeatDesc.Text = quickQuote.IRPM_BuildingFeaturesDesc
            Me.lblIRPM_Premises.Text = quickQuote.IRPM_Premises
            Me.lblIRPM_PremisesDesc.Text = quickQuote.IRPM_PremisesDesc
            Me.lblIRPM_Emp.Text = quickQuote.IRPM_Employees
            Me.lblIRPM_EmpDesc.Text = quickQuote.IRPM_EmployeesDesc
            Me.lblIRPM_Prot.Text = quickQuote.IRPM_Protection
            Me.lblIRPM_ProtDesc.Text = quickQuote.IRPM_ProtectionDesc
            Me.lblIRPM_CatHaz.Text = quickQuote.IRPM_CatostrophicHazards
            Me.lblIRPM_CatHazDesc.Text = quickQuote.IRPM_CatostrophicHazardsDesc
            Me.lblIRPM_MgmtExp.Text = quickQuote.IRPM_ManagementExperience
            Me.lblIRPM_MgmtExpDesc.Text = quickQuote.IRPM_ManagementExperienceDesc
            'updated 8/16/2012 for WC IRPM
            Me.lblIRPM_Equipment.Text = quickQuote.IRPM_Equipment
            Me.lblIRPM_EquipmentDesc.Text = quickQuote.IRPM_EquipmentDesc
            Me.lblIRPM_MedFac.Text = quickQuote.IRPM_MedicalFacilities
            Me.lblIRPM_MedFacDesc.Text = quickQuote.IRPM_MedicalFacilitiesDesc
            Me.lblIRPM_ClsPec.Text = quickQuote.IRPM_ClassificationPeculiarities
            Me.lblIRPM_ClsPecDesc.Text = quickQuote.IRPM_ClassificationPeculiaritiesDesc
            '10/4/2012 note:  IRPM isn't shown here so there's no need to add the other fields that are specific to other lobs

            'Dec (added 8/14/2012)
            'quickQuote.CalculateDecPremiums()'commented out 8/16/2012 (always being done now)
            Me.lblDec_Building_Prem.Text = quickQuote.Dec_BuildingLimit_All_Premium
            Me.lblDec_PersProp_Prem.Text = quickQuote.Dec_BuildingPersPropLimit_All_Premium
            Me.lblDec_OccLiab_Prem.Text = quickQuote.OccurrencyLiabilityQuotedPremium
            Me.lblDec_EnhEnd_Prem.Text = quickQuote.BusinessMasterEnhancementQuotedPremium
            Me.lblDecBop_MinPremAdj_Prem.Text = quickQuote.MinimumPremiumAdjustment 'added 1/29/2013 for BOP (phase 2 item)
            Me.lblDecBop_OptCovs_Prem.Text = quickQuote.Dec_BOP_OptCovs_Premium
            Me.lblDec_Total_Prem.Text = quickQuote.TotalQuotedPremium

            Me.lblDec_TotEstPlan_Prem.Text = quickQuote.TotalEstimatedPlanPremium
            Me.lblDec_IncreasedLimit_Prem.Text = quickQuote.EmployersLiabilityQuotedPremium
            Me.lblDec_ExpMod_Prem.Text = quickQuote.ExpModQuotedPremium
            Me.lblDec_SchedMod_Prem.Text = quickQuote.ScheduleModQuotedPremium
            Me.lblDec_PremDiscount_Prem.Text = quickQuote.PremDiscountQuotedPremium
            Me.lblDec_LossConstant_Prem.Text = quickQuote.Dec_LossConstantPremium
            Me.lblDec_ExpConstant_Prem.Text = quickQuote.Dec_ExpenseConstantPremium
            Me.lblDec_Terrorism_Prem.Text = quickQuote.TerrorismQuotedPremium
            Me.lblDec_Minimum_Prem.Text = quickQuote.MinimumQuotedPremium
            Me.lblDec_MinPremAdj_Prem.Text = quickQuote.MinimumPremiumAdjustment
            Me.lblDec_TotEst_Prem.Text = quickQuote.TotalQuotedPremium
            Me.lblDec_SecInjFund_Prem.Text = quickQuote.SecondInjuryFundQuotedPremium
            Me.lblDecWC_Total_Prem.Text = quickQuote.Dec_WC_TotalPremiumDue
            'added 8/28/2012 for GL
            Me.lblDec_Premises_Prem.Text = quickQuote.GL_PremisesTotalQuotedPremium
            Me.lblDec_Products_Prem.Text = quickQuote.GL_ProductsTotalQuotedPremium
            Me.lblDec_Premises_Minimum_Prem.Text = quickQuote.GL_PremisesMinimumQuotedPremium
            Me.lblDec_Premises_MinPremAdj_Prem.Text = quickQuote.GL_PremisesMinimumPremiumAdjustment
            Me.lblDec_Products_Minimum_Prem.Text = quickQuote.GL_ProductsMinimumQuotedPremium
            Me.lblDec_Products_MinPremAdj_Prem.Text = quickQuote.GL_ProductsMinimumPremiumAdjustment
            Me.lblDecGl_OptCovs_Prem.Text = quickQuote.Dec_GL_OptCovs_Premium
            'added 10/4/2012 for CAP
            Me.lblDecCap_Liability_Limit.Text = quickQuote.Liability_UM_UIM_Limit
            Me.lblDecCap_Liability_Prem.Text = quickQuote.VehiclesTotal_CombinedSingleLimitLiablityQuotedPremium
            Me.lblDecCap_MedicalPayments_Limit.Text = quickQuote.MedicalPaymentsLimit
            Me.lblDecCap_MedicalPayments_Prem.Text = quickQuote.VehiclesTotal_MedicalPaymentsQuotedPremium
            Me.lblDecCap_UM_UIM_Limit.Text = quickQuote.Liability_UM_UIM_Limit
            Me.lblDecCap_UM_UIM_Prem.Text = quickQuote.VehiclesTotal_UM_UIM_CovsQuotedPremium
            Me.lblDecCap_Comp_Prem.Text = quickQuote.VehiclesTotal_ComprehensiveCoverageQuotedPremium
            Me.lblDecCap_Coll_Prem.Text = quickQuote.VehiclesTotal_CollisionCoverageQuotedPremium
            Me.lblDecCap_TowingAndLabor_Prem.Text = quickQuote.VehiclesTotal_TowingAndLaborQuotedPremium
            Me.lblDecCap_RentalReimbursement_Prem.Text = quickQuote.VehiclesTotal_RentalReimbursementQuotedPremium
            Me.lblDecCap_OptionalCoverages_Prem.Text = quickQuote.Dec_CAP_OptCovs_Premium
            Me.lblDecCAP_Total_Prem.Text = quickQuote.TotalQuotedPremium
            'added 11/15/2012 for CPR
            Me.lblDecCpr_BuildingCov_Prem.Text = quickQuote.CPR_BuildingsTotal_BuildingCovQuotedPremium
            Me.lblDecCpr_PersPropCov_Prem.Text = quickQuote.CPR_BuildingsTotal_PersPropCoverageQuotedPremium
            Me.lblDecCpr_PersPropOfOthers_Prem.Text = quickQuote.CPR_BuildingsTotal_PersPropOfOthersQuotedPremium
            Me.lblDecCpr_BusIncomeCov_Prem.Text = quickQuote.CPR_BuildingsTotal_BusinessIncomeCovQuotedPremium
            Me.lblDecCpr_PropertyInTheOpen_Prem.Text = quickQuote.LocationsTotal_PropertyInTheOpenRecords_QuotedPremium 'added 5/6/2013 for CPR
            'Me.lblDecCpr_Earthquake_Prem.Text = quickQuote.CPR_BuildingsTotal_EQ_QuotedPremium
            'updated 5/6/2013 to include PITO
            Me.lblDecCpr_Earthquake_Prem.Text = quickQuote.LocationsTotal_PitoRecords_And_BuildingsTotal_Combined_EQ_Premium
            '*get EB prem for all locations 11/16/2012
            Me.lblDec_EquipBreak_Prem.Text = quickQuote.LocationsTotal_EquipmentBreakdownQuotedPremium
            'added 11/19/2012 for CPP
            Me.lblDec_CPP_Total_Prem.Text = quickQuote.TotalQuotedPremium
            Me.lblDec_CPP_Cpr_BuildingCov_Prem.Text = quickQuote.CPR_BuildingsTotal_BuildingCovQuotedPremium
            Me.lblDec_CPP_Cpr_PersPropCov_Prem.Text = quickQuote.CPR_BuildingsTotal_PersPropCoverageQuotedPremium
            Me.lblDec_CPP_Cpr_PersPropOfOthers_Prem.Text = quickQuote.CPR_BuildingsTotal_PersPropOfOthersQuotedPremium
            Me.lblDec_CPP_Cpr_BusIncomeCov_Prem.Text = quickQuote.CPR_BuildingsTotal_BusinessIncomeCovQuotedPremium 'If(quickQuote.HasBusinessIncomeALS, quickQuote.BusinessIncomeALSQuotedPremium, quickQuote.CPR_BuildingsTotal_BusinessIncomeCovQuotedPremium)
            Me.lblDec_CPP_Cpr_PropertyInTheOpen_Prem.Text = quickQuote.LocationsTotal_PropertyInTheOpenRecords_QuotedPremium 'added 5/6/2013 for CPR
            Me.lblDec_CPP_Cpr_EnhanceEndorse_Prem.Text = quickQuote.PackageCPR_EnhancementEndorsementQuotedPremium
            Me.lblDec_CPP_Cpr_EquipBreak_Prem.Text = quickQuote.LocationsTotal_EquipmentBreakdownQuotedPremium
            'Me.lblDec_CPP_Cpr_Earthquake_Prem.Text = quickQuote.CPR_BuildingsTotal_EQ_QuotedPremium
            'updated 5/6/2013 to include PITO
            Me.lblDec_CPP_Cpr_Earthquake_Prem.Text = quickQuote.LocationsTotal_PitoRecords_And_BuildingsTotal_Combined_EQ_Premium
            Me.lblDec_CPP_Gl_EnhanceEndorse_Prem.Text = TryToFormatAsCurrency(quickQuote.PackageGL_EnhancementEndorsementQuotedPremium)
            Me.lblDec_CPP_Gl_Premises_Prem.Text = quickQuote.GL_PremisesTotalQuotedPremium
            Me.lblDec_CPP_Gl_Products_Prem.Text = quickQuote.GL_ProductsTotalQuotedPremium
            Me.lblDec_CPP_Gl_OptCovs_Prem.Text = quickQuote.Dec_GL_OptCovs_Premium
            Me.lblDec_CPP_Gl_Premises_Minimum_Prem.Text = quickQuote.GL_PremisesMinimumQuotedPremium
            Me.lblDec_CPP_Gl_Premises_MinPremAdj_Prem.Text = quickQuote.GL_PremisesMinimumPremiumAdjustment
            Me.lblDec_CPP_Gl_Products_Minimum_Prem.Text = quickQuote.GL_ProductsMinimumQuotedPremium
            Me.lblDec_CPP_Gl_Products_MinPremAdj_Prem.Text = quickQuote.GL_ProductsMinimumPremiumAdjustment

            Me.DECEPLIRow2.Visible = False

            If quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability Or quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP Then
                Me.DECEPLIRow2.Visible = quickQuote.HasEPLI
                Me.lblDec_EPLI_Cov2.Text = "EPLI:"
                Me.lblDec_EPLI_Cov_Selected.Text = quickQuote.EPLICoverageType.Replace("EPLI", "")
                Me.lblDec_EPLI_PREM2.Text = quickQuote.EPLIPremium
            End If

            If quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                Me.DECEPLIRow2.Visible = quickQuote.HasEPLI
                'Me.lblDec_EPLI_Cov2.Text = String.Format("EPLI: {0}", quickQuote.EPLICoverageType.Replace("EPLI", ""))
                Me.lblDec_EPLI_Cov2.Text = "EPLI:"
                Me.lblDec_EPLI_Cov_Selected.Text = quickQuote.EPLICoverageType.Replace("EPLI", "")
                Me.lblDec_EPLI_PREM2.Text = quickQuote.EPLIPremium
            End If

            ' Added 2/11/15 MGB Bug 4040 - GL CGL1002/CGL1004 coverages
            If quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability OrElse quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                If quickQuote.BlanketWaiverOfSubrogation IsNot Nothing AndAlso quickQuote.BlanketWaiverOfSubrogation <> String.Empty Then
                    Select Case quickQuote.BlanketWaiverOfSubrogation
                        Case "", "0"
                            BlanketWaiverOfSubroRow.Visible = False
                            BlanketWaiverOfSubroWithCompletedOpsRow.Visible = False
                            Exit Select
                        Case "1"  ' CGL 1004
                            BlanketWaiverOfSubroRow.Visible = True
                            lblBlanketWaiverOfSubro.Text = "Selected"
                            lblBlanketWaiverOfSubroQuotedPremium.Text = "$100.00"
                            Exit Select
                        Case "2"  ' CGL 1002
                            BlanketWaiverOfSubroWithCompletedOpsRow.Visible = True
                            lblBlanketWaiverOfSubroWithCompletedOps.Text = "Selected"
                            lblBlanketWaiverOfSubroWithCompletedOpsQuotedPremium.Text = "$300.00"
                            Exit Select
                        Case Else
                            BlanketWaiverOfSubroRow.Visible = False
                            BlanketWaiverOfSubroWithCompletedOpsRow.Visible = False
                            Exit Select
                    End Select
                End If
            End If

            ShowFieldsForEachLOB()
        End If
    End Sub

    Protected Sub rptLocations_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptLocations.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim lblLocIndex As Label = e.Item.FindControl("lblLocIndex")
            'Dim BuildingInformationRepeaterRow As TableRow = e.Item.FindControl("BuildingInformationRepeaterRow")
            Dim BuildingInformationRepeaterRow As HtmlControls.HtmlTableRow = e.Item.FindControl("BuildingInformationRepeaterRow")
            Dim rptBuildings As Repeater = e.Item.FindControl("rptBuildings")
            'updated 5/6/2013 for PITO
            Dim PropertyInTheOpenRow As HtmlTableRow = e.Item.FindControl("PropertyInTheOpenRow")
            'PropertyInTheOpenHeaderSpacerRow will always be visible
            Dim dgrdPropertyInTheOpen As DataGrid = e.Item.FindControl("dgrdPropertyInTheOpen")

            Dim LocDescriptionRow As HtmlTableRow = e.Item.FindControl("LocDescriptionRow")
            Dim LocNameRow As HtmlTableRow = e.Item.FindControl("LocNameRow")
            Dim LocStreetNumRow As HtmlTableRow = e.Item.FindControl("LocStreetNumRow")
            Dim LocStreetNameRow As HtmlTableRow = e.Item.FindControl("LocStreetNameRow")
            Dim LocAptNumRow As HtmlTableRow = e.Item.FindControl("LocAptNumRow")
            Dim LocPoBoxRow As HtmlTableRow = e.Item.FindControl("LocPoBoxRow")
            Dim LocProtClassRow As HtmlTableRow = e.Item.FindControl("LocProtClassRow")
            Dim LocNumberOfPoolsRow As HtmlTableRow = e.Item.FindControl("LocNumberOfPoolsRow")
            Dim LocNumberOfAmusementsRow As HtmlTableRow
            Dim LocNumberOfPlaygroundsRow As HtmlTableRow
            'If qqHelper.doUseNewBOPVersion(quickQuote.EffectiveDate) Then
            LocNumberOfAmusementsRow = e.Item.FindControl("LocNumberOfAmusementsRow")
            LocNumberOfPlaygroundsRow = e.Item.FindControl("LocNumberOfPlaygroundsRow")
            'End If
            'Dim LocationHeaderSpacerRow As HtmlTableRow = e.Item.FindControl("LocationHeaderSpacerRow") 'added 2/19/2013; removed since on header and not on item

            Dim OptionalLocationCoveragesSpacerRow As HtmlTableRow = e.Item.FindControl("OptionalLocationCoveragesSpacerRow")
            Dim OptionalLocationCoveragesMainRow As HtmlTableRow = e.Item.FindControl("OptionalLocationCoveragesMainRow")
            Dim OptionalLocationCoveragesHeaderSpacerRow As HtmlTableRow = e.Item.FindControl("OptionalLocationCoveragesHeaderSpacerRow") 'added 2/19/2013
            Dim OptionalLocationCoveragesQuotedPremRow As HtmlTableRow = e.Item.FindControl("OptionalLocationCoveragesQuotedPremRow")
            'added 2/14/2013 (styles and cleanup)
            Dim LocEquipBreakDeductSelectedRow As HtmlTableRow = e.Item.FindControl("LocEquipBreakDeductSelectedRow")
            Dim LocEquipBreakDeductRow As HtmlTableRow = e.Item.FindControl("LocEquipBreakDeductRow")
            'added 2/14/2013 (styles and cleanup)
            Dim LocMoneySecuritiesOnPremisesSelectedRow As HtmlTableRow = e.Item.FindControl("LocMoneySecuritiesOnPremisesSelectedRow")
            Dim LocMoneySecuritiesOnPremisesRow As HtmlTableRow = e.Item.FindControl("LocMoneySecuritiesOnPremisesRow")
            'added 2/14/2013 (styles and cleanup)
            Dim LocMoneySecuritiesOffPremisesSelectedRow As HtmlTableRow = e.Item.FindControl("LocMoneySecuritiesOffPremisesSelectedRow")
            Dim LocMoneySecuritiesOffPremisesRow As HtmlTableRow = e.Item.FindControl("LocMoneySecuritiesOffPremisesRow")
            'added 2/14/2013 (styles and cleanup)
            Dim LocOutdoorSignsSelectedRow As HtmlTableRow = e.Item.FindControl("LocOutdoorSignsSelectedRow")
            Dim LocOutdoorSignsRow As HtmlTableRow = e.Item.FindControl("LocOutdoorSignsRow")

            Dim LocFineArtsSelectedRow As HtmlTableRow
            Dim LocCustomerAutoLegalSelectedRow As HtmlTableRow
            Dim LocCustomerAutoLegalLimitRow As HtmlTableRow
            Dim LocCustomerAutoLegalDeductibleRow As HtmlTableRow
            Dim LocTenantAutoLegalSelectedRow As HtmlTableRow
            Dim LocTenantAutoLegalLimitRow As HtmlTableRow
            Dim LocTenantAutoLegalDeductibleRow As HtmlTableRow
            'If qqHelper.doUseNewBOPVersion(quickQuote.EffectiveDate) Then
            LocFineArtsSelectedRow = e.Item.FindControl("LocFineArtsSelectedRow")
            LocCustomerAutoLegalSelectedRow = e.Item.FindControl("LocCustomerAutoLegalSelectedRow")
            LocCustomerAutoLegalLimitRow = e.Item.FindControl("LocCustomerAutoLegalLimitRow")
            LocCustomerAutoLegalDeductibleRow = e.Item.FindControl("LocCustomerAutoLegalDeductibleRow")
            LocTenantAutoLegalSelectedRow = e.Item.FindControl("LocTenantAutoLegalSelectedRow")
            LocTenantAutoLegalLimitRow = e.Item.FindControl("LocTenantAutoLegalLimitRow")
            LocTenantAutoLegalDeductibleRow = e.Item.FindControl("LocTenantAutoLegalDeductibleRow")
            'End If

            Dim rptClassCodes As Repeater = e.Item.FindControl("rptClassCodes")
            Dim ClassCodesInformationRepeaterRow As HtmlTableRow = e.Item.FindControl("ClassCodesInformationRepeaterRow")
            'Dim ClassCodesHeaderSpacerRow As HtmlTableRow = e.Item.FindControl("ClassCodesHeaderSpacerRow") 'added 2/19/2013; not used since on header row

            Dim rpt_GL_ClassCodes As Repeater = e.Item.FindControl("rpt_GL_ClassCodes")
            Dim GL_ClassCodesInformationRepeaterRow As HtmlTableRow = e.Item.FindControl("GL_ClassCodesInformationRepeaterRow")
            'Dim LocationGlClassCodeHeaderSpacerRow As HtmlTableRow = e.Item.FindControl("LocationGlClassCodeHeaderSpacerRow")'added 2/19/2013; not used since on header row

            Dim lblLocHouseNum As Label = e.Item.FindControl("lblLocHouseNum")
            Dim lblLocStreet As Label = e.Item.FindControl("lblLocStreet")
            Dim lblLocPoBox As Label = e.Item.FindControl("lblLocPoBox")
            Dim lblLocAptNum As Label = e.Item.FindControl("lblLocAptNum")

            'added 2/14/2013
            Dim lblEquipBreakDeductSelected As Label = e.Item.FindControl("lblEquipBreakDeductSelected")
            Dim lblMoneyOnPremSelected As Label = e.Item.FindControl("lblMoneyOnPremSelected")
            Dim lblMoneyOffPremSelected As Label = e.Item.FindControl("lblMoneyOffPremSelected")
            Dim lblOutdoorSignsSelected As Label = e.Item.FindControl("lblOutdoorSignsSelected")
            Dim lblFineArtsSelected As Label
            Dim lblFineArtsQuotedPremium As Label
            Dim lblCustomerAutoLegalSelected As Label
            Dim lblCustomerAutoLegalQuotedPremium As Label
            Dim lblTenantAutoLegalSelected As Label
            Dim lblTenantAutoLegalQuotedPremium As Label
            'If qqHelper.doUseNewBOPVersion(quickQuote.EffectiveDate) Then
            lblFineArtsSelected = e.Item.FindControl("lblFineArtsSelected")
            lblFineArtsQuotedPremium = e.Item.FindControl("lblFineArtsQuotedPremium")
            lblCustomerAutoLegalSelected = e.Item.FindControl("lblCustomerAutoLegalSelected")
            lblCustomerAutoLegalQuotedPremium = e.Item.FindControl("lblCustomerAutoLegalQuotedPremium")
            lblTenantAutoLegalSelected = e.Item.FindControl("lblTenantAutoLegalSelected")
            lblTenantAutoLegalQuotedPremium = e.Item.FindControl("lblTenantAutoLegalQuotedPremium")
            'End If

            LocDescriptionRow.Visible = False 'BOP
            LocNameRow.Visible = False 'WC
            LocStreetNumRow.Visible = False 'all
            LocStreetNameRow.Visible = False 'all
            LocAptNumRow.Visible = False 'all
            LocPoBoxRow.Visible = False 'all
            LocProtClassRow.Visible = False 'BOP
            LocNumberOfPoolsRow.Visible = False 'BOP
            If qqHelper.doUseNewBOPVersion(quickQuote.EffectiveDate) Then
                LocNumberOfAmusementsRow.Visible = False 'BOP
                LocNumberOfPlaygroundsRow.Visible = False 'BOP
            End If

            'LocationHeaderSpacerRow.Visible = True 'added 2/19/2013; all LOBs; removed since on header and not on item

            OptionalLocationCoveragesSpacerRow.Visible = False 'BOP
            OptionalLocationCoveragesMainRow.Visible = False 'BOP
            OptionalLocationCoveragesHeaderSpacerRow.Visible = False 'added 2/19/2013; BOP
            OptionalLocationCoveragesQuotedPremRow.Visible = False 'BOP
            LocEquipBreakDeductSelectedRow.Visible = False 'BOP/CPR/CPP (currently only used w/ BOP); added 2/14/2013
            LocEquipBreakDeductRow.Visible = False 'BOP (2/14/2013 - added note: also CPR/CPP)
            LocMoneySecuritiesOnPremisesSelectedRow.Visible = False 'BOP; added 2/14/2013
            LocMoneySecuritiesOnPremisesRow.Visible = False 'BOP
            LocMoneySecuritiesOffPremisesSelectedRow.Visible = False 'BOP; added 2/14/2013
            LocMoneySecuritiesOffPremisesRow.Visible = False 'BOP
            LocOutdoorSignsSelectedRow.Visible = False 'BOP; added 2/14/2013
            LocOutdoorSignsRow.Visible = False 'BOP
            'If qqHelper.doUseNewBOPVersion(quickQuote.EffectiveDate) Then
            LocFineArtsSelectedRow.Visible = False
            LocCustomerAutoLegalLimitRow.Visible = False
            LocCustomerAutoLegalDeductibleRow.Visible = False
            LocCustomerAutoLegalSelectedRow.Visible = False
            LocTenantAutoLegalLimitRow.Visible = False
            LocTenantAutoLegalDeductibleRow.Visible = False
            LocTenantAutoLegalSelectedRow.Visible = False
            lblFineArtsQuotedPremium.Visible = False
            lblCustomerAutoLegalQuotedPremium.Visible = False
            lblTenantAutoLegalQuotedPremium.Visible = False
            'End If

            If lblLocPoBox.Text <> "" AndAlso (lblLocHouseNum.Text = "" OrElse lblLocStreet.Text = "") Then
                LocPoBoxRow.Visible = True
            Else
                LocStreetNumRow.Visible = True
                LocStreetNameRow.Visible = True
                'added 8/27/2012
                If lblLocAptNum.Text <> "" Then
                    LocAptNumRow.Visible = True
                End If
            End If

            If quickQuote IsNot Nothing AndAlso quickQuote.LobType <> Nothing AndAlso quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP Then
                LocDescriptionRow.Visible = True
                'LocProtClassRow.Visible = True'now hiding here as-of 8/30/2012 (moved to building)
                LocNumberOfPoolsRow.Visible = True
                If qqHelper.doUseNewBOPVersion(quickQuote.EffectiveDate) Then
                    LocNumberOfPlaygroundsRow.Visible = True
                    LocNumberOfAmusementsRow.Visible = True
                End If
                OptionalLocationCoveragesSpacerRow.Visible = True
                OptionalLocationCoveragesMainRow.Visible = True
                OptionalLocationCoveragesHeaderSpacerRow.Visible = True 'added 2/19/2013; BOP
                OptionalLocationCoveragesQuotedPremRow.Visible = True
                'LocEquipBreakDeductRow.Visible = True
                'updated 2/14/2013 (styles and cleanup)
                If UCase(lblEquipBreakDeductSelected.Text) = "NOT SELECTED" Then
                    LocEquipBreakDeductSelectedRow.Visible = True
                    LocEquipBreakDeductRow.Visible = False
                Else
                    LocEquipBreakDeductSelectedRow.Visible = False
                    LocEquipBreakDeductRow.Visible = True
                End If
                'LocMoneySecuritiesOnPremisesRow.Visible = True
                'updated 2/14/2013 (styles and cleanup)
                If UCase(lblMoneyOnPremSelected.Text) = "NOT SELECTED" Then
                    LocMoneySecuritiesOnPremisesSelectedRow.Visible = True
                    LocMoneySecuritiesOnPremisesRow.Visible = False
                Else
                    LocMoneySecuritiesOnPremisesSelectedRow.Visible = False
                    LocMoneySecuritiesOnPremisesRow.Visible = True
                End If
                'LocMoneySecuritiesOffPremisesRow.Visible = True
                'updated 2/14/2013 (styles and cleanup)
                If UCase(lblMoneyOffPremSelected.Text) = "NOT SELECTED" Then
                    LocMoneySecuritiesOffPremisesSelectedRow.Visible = True
                    LocMoneySecuritiesOffPremisesRow.Visible = False
                Else
                    LocMoneySecuritiesOffPremisesSelectedRow.Visible = False
                    LocMoneySecuritiesOffPremisesRow.Visible = True
                End If
                'LocOutdoorSignsRow.Visible = True
                'updated 2/14/2013 (styles and cleanup)
                If UCase(lblOutdoorSignsSelected.Text) = "NOT SELECTED" Then
                    LocOutdoorSignsSelectedRow.Visible = True
                    LocOutdoorSignsRow.Visible = False
                Else
                    LocOutdoorSignsSelectedRow.Visible = False
                    LocOutdoorSignsRow.Visible = True
                End If

                LocFineArtsSelectedRow.Visible = True
                If qqHelper.doUseNewBOPVersion(quickQuote.EffectiveDate) Then
                    If UCase(lblFineArtsSelected.Text) = "NOT SELECTED" Then
                        'do nothing
                    Else
                        lblFineArtsQuotedPremium.Visible = True
                    End If

                    LocCustomerAutoLegalSelectedRow.Visible = True
                    If UCase(lblCustomerAutoLegalSelected.Text) = "NOT SELECTED" Then
                        LocCustomerAutoLegalLimitRow.Visible = False
                        LocCustomerAutoLegalDeductibleRow.Visible = False
                    Else
                        LocCustomerAutoLegalLimitRow.Visible = True
                        LocCustomerAutoLegalDeductibleRow.Visible = True
                        lblCustomerAutoLegalQuotedPremium.Visible = True
                    End If

                    LocTenantAutoLegalSelectedRow.Visible = True
                    If UCase(lblTenantAutoLegalSelected.Text) = "NOT SELECTED" Then
                        LocTenantAutoLegalLimitRow.Visible = False
                        LocTenantAutoLegalDeductibleRow.Visible = False
                    Else
                        LocTenantAutoLegalLimitRow.Visible = True
                        LocTenantAutoLegalDeductibleRow.Visible = True
                        lblTenantAutoLegalQuotedPremium.Visible = True
                    End If
                End If
            ElseIf quickQuote IsNot Nothing AndAlso quickQuote.LobType <> Nothing AndAlso quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.WorkersCompensation Then
                LocNameRow.Visible = True
                'ElseIf quickQuote IsNot Nothing AndAlso quickQuote.LobType <> Nothing AndAlso quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty Then 'added 10/16/2012 for CPR
                'updated 11/19/2012 for CPP
            ElseIf quickQuote IsNot Nothing AndAlso quickQuote.LobType <> Nothing AndAlso (quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty OrElse quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage) Then 'added 10/16/2012 for CPR
                LocDescriptionRow.Visible = True
                OptionalLocationCoveragesSpacerRow.Visible = True 'added CPR logic 10/17/2012
                OptionalLocationCoveragesMainRow.Visible = True 'added CPR logic 10/17/2012
                OptionalLocationCoveragesHeaderSpacerRow.Visible = True 'added 2/19/2013; BOP
                OptionalLocationCoveragesQuotedPremRow.Visible = True 'added CPR logic 10/17/2012
                LocEquipBreakDeductRow.Visible = True 'added CPR logic 10/17/2012; 2/14/2013 - could use Selected/Not Selected logic like BOP, but won't for now
            Else
                'no lob
            End If

            If lblLocIndex.Text = "test" OrElse (quickQuote IsNot Nothing AndAlso quickQuote.Locations IsNot Nothing AndAlso IsNumeric(lblLocIndex.Text) = True AndAlso (quickQuote.Locations.Count - 1) >= CInt(lblLocIndex.Text) AndAlso quickQuote.Locations(CInt(lblLocIndex.Text)).Buildings IsNot Nothing AndAlso quickQuote.Locations(CInt(lblLocIndex.Text)).Buildings.Count > 0) Then
                BuildingInformationRepeaterRow.Visible = True

                Dim dt As New DataTable
                'building information (can have multiple per location) - ResponseData/Image/LOB/RiskLevel/Locations/Location/BarnsBuildings/BarnBuilding
                dt.Columns.Add("BuildingDescription", System.Type.GetType("System.String"))
                dt.Columns.Add("BuildingProgram", System.Type.GetType("System.String"))
                dt.Columns.Add("BuildingClassification", System.Type.GetType("System.String"))
                dt.Columns.Add("BuildingClassCode", System.Type.GetType("System.String"))
                dt.Columns.Add("BuildingOccupancy", System.Type.GetType("System.String"))
                dt.Columns.Add("BuildingConstruction", System.Type.GetType("System.String"))
                dt.Columns.Add("BuildingAutoIncrease", System.Type.GetType("System.String"))
                dt.Columns.Add("BuildingAutoIncreaseQuotedPremium", System.Type.GetType("System.String"))
                dt.Columns.Add("BuildingPropDeduct", System.Type.GetType("System.String"))
                dt.Columns.Add("BuildingLimit", System.Type.GetType("System.String"))
                dt.Columns.Add("BuildingLimitQuotedPremium", System.Type.GetType("System.String"))
                dt.Columns.Add("BuildingValuation", System.Type.GetType("System.String"))
                dt.Columns.Add("BuildingIncludedInBlanket", System.Type.GetType("System.String"))
                dt.Columns.Add("BuildingMineSubsidence", System.Type.GetType("System.String"))
                dt.Columns.Add("BuildingMineSubsidenceQuotedPremium", System.Type.GetType("System.String"))
                dt.Columns.Add("BuildingSprinklered", System.Type.GetType("System.String"))
                dt.Columns.Add("PersPropLimit", System.Type.GetType("System.String"))
                dt.Columns.Add("PersPropLimitQuotedPremium", System.Type.GetType("System.String"))
                dt.Columns.Add("BuildingACVRoofing", System.Type.GetType("System.String"))
                dt.Columns.Add("BuildingValuationMethod", System.Type.GetType("System.String"))
                dt.Columns.Add("BuildingIncludedInBlanket2", System.Type.GetType("System.String"))
                dt.Columns.Add("BuildProtClass", System.Type.GetType("System.String"))
                dt.Columns.Add("Build_CPR_TotalPremium", System.Type.GetType("System.String")) 'added 10/16/2012 for CPR
                'added 10/16/2012 for CPR Covs table
                dt.Columns.Add("BuildingCov_Limit", System.Type.GetType("System.String"))
                dt.Columns.Add("BuildingCov_ClassCode", System.Type.GetType("System.String"))
                dt.Columns.Add("BuildingCov_ClassCodeDescription", System.Type.GetType("System.String"))
                dt.Columns.Add("BuildingCov_CauseOfLoss", System.Type.GetType("System.String"))
                dt.Columns.Add("BuildingCov_IncludedInBlanket", System.Type.GetType("System.String")) 'added 5/7/2013
                dt.Columns.Add("BuildingCov_CoInsurance", System.Type.GetType("System.String"))
                dt.Columns.Add("BuildingCov_IsAgreedValue", System.Type.GetType("System.String")) 'bug 4845 Matt A 2/1/2016
                dt.Columns.Add("BuildingCov_Valuation", System.Type.GetType("System.String"))
                dt.Columns.Add("BuildingCov_Deductible", System.Type.GetType("System.String"))
                dt.Columns.Add("BuildingCov_TheftDeductible", System.Type.GetType("System.String")) 'added 5/7/2013
                dt.Columns.Add("BuildingCov_WindHailDeductible", System.Type.GetType("System.String")) 'added 5/7/2013
                dt.Columns.Add("BuildingCov_InflationGuard", System.Type.GetType("System.String"))
                dt.Columns.Add("BuildingCov_EQ_Deductible", System.Type.GetType("System.String"))
                dt.Columns.Add("BuildingCov_EQ_Premium", System.Type.GetType("System.String"))
                dt.Columns.Add("BuildingCov_TotalPremium", System.Type.GetType("System.String"))
                dt.Columns.Add("Building_PersPropCov_Limit", System.Type.GetType("System.String"))
                dt.Columns.Add("Building_PersPropCov_ClassCode", System.Type.GetType("System.String"))
                dt.Columns.Add("Building_PersPropCov_ClassCodeDescription", System.Type.GetType("System.String"))
                dt.Columns.Add("Building_PersPropCov_CauseOfLoss", System.Type.GetType("System.String"))
                dt.Columns.Add("Building_PersPropCov_IncludedInBlanket", System.Type.GetType("System.String")) 'added 5/7/2013
                dt.Columns.Add("Building_PersPropCov_CoInsurance", System.Type.GetType("System.String"))
                dt.Columns.Add("Building_PersPropCov_IsAgreedValue", System.Type.GetType("System.String")) 'Bug 4845 2/1/2016
                dt.Columns.Add("Building_PersPropCov_Valuation", System.Type.GetType("System.String"))
                dt.Columns.Add("Building_PersPropCov_Deductible", System.Type.GetType("System.String"))
                dt.Columns.Add("Building_PersPropCov_TheftDeductible", System.Type.GetType("System.String")) 'added 5/7/2013
                dt.Columns.Add("Building_PersPropCov_WindHailDeductible", System.Type.GetType("System.String")) 'added 5/7/2013
                dt.Columns.Add("Building_PersPropCov_InflationGuard", System.Type.GetType("System.String"))
                dt.Columns.Add("Building_PersPropCov_EQ_Deductible", System.Type.GetType("System.String"))
                dt.Columns.Add("Building_PersPropCov_EQ_Premium", System.Type.GetType("System.String"))
                dt.Columns.Add("Building_PersPropCov_TotalPremium", System.Type.GetType("System.String"))
                dt.Columns.Add("Building_PersPropOfOthersCov_Limit", System.Type.GetType("System.String"))
                dt.Columns.Add("Building_PersPropOfOthersCov_ClassCode", System.Type.GetType("System.String"))
                dt.Columns.Add("Building_PersPropOfOthersCov_ClassCodeDescription", System.Type.GetType("System.String"))
                dt.Columns.Add("Building_PersPropOfOthersCov_CauseOfLoss", System.Type.GetType("System.String"))
                dt.Columns.Add("Building_PersPropOfOthersCov_IncludedInBlanket", System.Type.GetType("System.String")) 'added 5/7/2013
                dt.Columns.Add("Building_PersPropOfOthersCov_CoInsurance", System.Type.GetType("System.String"))
                dt.Columns.Add("Building_PersPropOfOthersCov_IsAgreedValue", System.Type.GetType("System.String")) ' Bug 4845 2/1/16
                dt.Columns.Add("Building_PersPropOfOthersCov_Valuation", System.Type.GetType("System.String"))
                dt.Columns.Add("Building_PersPropOfOthersCov_Deductible", System.Type.GetType("System.String"))
                dt.Columns.Add("Building_PersPropOfOthersCov_TheftDeductible", System.Type.GetType("System.String")) 'added 5/7/2013
                dt.Columns.Add("Building_PersPropOfOthersCov_WindHailDeductible", System.Type.GetType("System.String")) 'added 5/7/2013
                dt.Columns.Add("Building_PersPropOfOthersCov_InflationGuard", System.Type.GetType("System.String"))
                dt.Columns.Add("Building_PersPropOfOthersCov_EQ_Deductible", System.Type.GetType("System.String"))
                dt.Columns.Add("Building_PersPropOfOthersCov_EQ_Premium", System.Type.GetType("System.String"))
                dt.Columns.Add("Building_PersPropOfOthersCov_TotalPremium", System.Type.GetType("System.String"))
                dt.Columns.Add("Building_BusinessIncomeCov_Limit", System.Type.GetType("System.String"))
                dt.Columns.Add("Building_BusinessIncomeCov_ClassCode", System.Type.GetType("System.String"))
                dt.Columns.Add("Building_BusinessIncomeCov_ClassCodeDescription", System.Type.GetType("System.String"))
                dt.Columns.Add("Building_BusinessIncomeCov_CauseOfLoss", System.Type.GetType("System.String"))
                dt.Columns.Add("Building_BusinessIncomeCov_IncludedInBlanket", System.Type.GetType("System.String")) 'added 5/7/2013
                dt.Columns.Add("Building_BusinessIncomeCov_CoInsurance", System.Type.GetType("System.String"))
                dt.Columns.Add("Building_BusinessIncomeCov_IsAgreedValue", System.Type.GetType("System.String")) ' Bug 4845 2/1/16
                dt.Columns.Add("Building_BusinessIncomeCov_Valuation", System.Type.GetType("System.String"))
                dt.Columns.Add("Building_BusinessIncomeCov_Deductible", System.Type.GetType("System.String"))
                dt.Columns.Add("Building_BusinessIncomeCov_TheftDeductible", System.Type.GetType("System.String")) 'added 5/7/2013
                dt.Columns.Add("Building_BusinessIncomeCov_WindHailDeductible", System.Type.GetType("System.String")) 'added 5/7/2013
                dt.Columns.Add("Building_BusinessIncomeCov_InflationGuard", System.Type.GetType("System.String"))
                dt.Columns.Add("Building_BusinessIncomeCov_EQ_Deductible", System.Type.GetType("System.String"))
                dt.Columns.Add("Building_BusinessIncomeCov_EQ_Premium", System.Type.GetType("System.String"))
                dt.Columns.Add("Building_BusinessIncomeCov_TotalPremium", System.Type.GetType("System.String"))

                'optional location coverages (building)
                dt.Columns.Add("AcctsRecSelected", System.Type.GetType("System.String")) 'added 2/14/2013 (styles and cleanup)
                dt.Columns.Add("AcctsRecOnPrem", System.Type.GetType("System.String"))
                dt.Columns.Add("AcctsRecQuotedPremium", System.Type.GetType("System.String"))
                dt.Columns.Add("AcctsRecOffPrem", System.Type.GetType("System.String"))
                dt.Columns.Add("ValPapersSelected", System.Type.GetType("System.String")) 'added 2/14/2013 (styles and cleanup)
                dt.Columns.Add("ValPapersOnPrem", System.Type.GetType("System.String"))
                dt.Columns.Add("ValPapersQuotedPremium", System.Type.GetType("System.String"))
                dt.Columns.Add("ValPapersOffPrem", System.Type.GetType("System.String"))
                dt.Columns.Add("CondoCommUnitOwnersSelected", System.Type.GetType("System.String")) 'added 2/14/2013 (styles and cleanup)
                dt.Columns.Add("CondoCommUnitOwners", System.Type.GetType("System.String"))
                dt.Columns.Add("CondoCommUnitOwnersQuotedPremium", System.Type.GetType("System.String"))
                dt.Columns.Add("OrdOrLaw", System.Type.GetType("System.String"))
                dt.Columns.Add("OrdOrLawCov1", System.Type.GetType("System.String"))
                dt.Columns.Add("OrdOrLawCov1QuotedPremium", System.Type.GetType("System.String"))
                dt.Columns.Add("OrdOrLawCov2", System.Type.GetType("System.String"))
                dt.Columns.Add("OrdOrLawCov2QuotedPremium", System.Type.GetType("System.String"))
                dt.Columns.Add("OrdOrLawCov3", System.Type.GetType("System.String"))
                dt.Columns.Add("OrdOrLawCov3QuotedPremium", System.Type.GetType("System.String"))
                dt.Columns.Add("OrdOrLawCov4", System.Type.GetType("System.String"))
                dt.Columns.Add("OrdOrLawCov4QuotedPremium", System.Type.GetType("System.String"))
                dt.Columns.Add("Spoilage", System.Type.GetType("System.String"))
                dt.Columns.Add("SpoilageQuotedPremium", System.Type.GetType("System.String"))
                dt.Columns.Add("SpoilagePropClass", System.Type.GetType("System.String"))
                dt.Columns.Add("SpoilageTotLimit", System.Type.GetType("System.String"))
                dt.Columns.Add("SpoilageRefrig", System.Type.GetType("System.String"))
                dt.Columns.Add("SpoilageBreakdown", System.Type.GetType("System.String"))
                dt.Columns.Add("SpoilagePowerOutage", System.Type.GetType("System.String"))

                If lblLocIndex.Text = "test" Then
                    Dim newRow As DataRow = dt.NewRow
                    newRow.Item("BuildingDescription") = "Building #1 and Location #1"
                    newRow.Item("BuildingProgram") = "Apartment"
                    newRow.Item("BuildingClassification") = "Apart - < 4 families - Merc/Office - Lessors Risk - Incl. 3/4 family &"
                    newRow.Item("BuildingClassCode") = "65141"
                    newRow.Item("BuildingOccupancy") = "Non-Owner Occupied Bldg/Lessor's"
                    newRow.Item("BuildingConstruction") = "Frame"
                    newRow.Item("BuildingAutoIncrease") = "4%"
                    If qqHelper.doUseNewBOPVersion(quickQuote.EffectiveDate) Then
                        newRow.Item("BuildingAutoIncreaseQuotedPremium") = ""
                    End If
                    newRow.Item("BuildingPropDeduct") = "$250"
                    newRow.Item("BuildingLimit") = "100000"
                    newRow.Item("BuildingLimitQuotedPremium") = "250"
                    newRow.Item("BuildingValuation") = "Replacement Cost"
                    newRow.Item("BuildingIncludedInBlanket") = "Not Selected"
                    newRow.Item("BuildingMineSubsidence") = "Selected"
                    newRow.Item("BuildingMineSubsidenceQuotedPremium") = "250"
                    newRow.Item("BuildingSprinklered") = "Selected"
                    newRow.Item("PersPropLimit") = "100000"
                    newRow.Item("PersPropLimitQuotedPremium") = "250"
                    newRow.Item("BuildingACVRoofing") = ""
                    newRow.Item("BuildingValuationMethod") = "Replacement Cost"
                    newRow.Item("BuildingIncludedInBlanket2") = "Selected"
                    newRow.Item("BuildProtClass") = "01"
                    'added 10/16/2012
                    newRow.Item("Build_CPR_TotalPremium") = ""
                    newRow.Item("BuildingCov_Limit") = ""
                    newRow.Item("BuildingCov_ClassCode") = ""
                    newRow.Item("BuildingCov_ClassCodeDescription") = ""
                    newRow.Item("BuildingCov_CauseOfLoss") = ""
                    newRow.Item("BuildingCov_IncludedInBlanket") = ""
                    newRow.Item("BuildingCov_CoInsurance") = ""
                    newRow.Item("BuildingCov_IsAgreedValue") = "" ' Bug 4845 2-1-2016 Matt A
                    newRow.Item("BuildingCov_Valuation") = ""
                    newRow.Item("BuildingCov_Deductible") = ""
                    newRow.Item("BuildingCov_TheftDeductible") = ""
                    newRow.Item("BuildingCov_WindHailDeductible") = ""
                    newRow.Item("BuildingCov_InflationGuard") = ""
                    newRow.Item("BuildingCov_EQ_Deductible") = ""
                    newRow.Item("BuildingCov_EQ_Premium") = ""
                    newRow.Item("BuildingCov_TotalPremium") = ""
                    newRow.Item("Building_PersPropCov_Limit") = ""
                    newRow.Item("Building_PersPropCov_ClassCode") = ""
                    newRow.Item("Building_PersPropCov_ClassCodeDescription") = ""
                    newRow.Item("Building_PersPropCov_CauseOfLoss") = ""
                    newRow.Item("Building_PersPropCov_IncludedInBlanket") = ""
                    newRow.Item("Building_PersPropCov_CoInsurance") = ""
                    newRow.Item("Building_PersPropCov_IsAgreedValue") = "" ' Bug 4845 2-1-2016 Matt A
                    newRow.Item("Building_PersPropCov_Valuation") = ""
                    newRow.Item("Building_PersPropCov_Deductible") = ""
                    newRow.Item("Building_PersPropCov_TheftDeductible") = ""
                    newRow.Item("Building_PersPropCov_WindHailDeductible") = ""
                    newRow.Item("Building_PersPropCov_InflationGuard") = ""
                    newRow.Item("Building_PersPropCov_EQ_Deductible") = ""
                    newRow.Item("Building_PersPropCov_EQ_Premium") = ""
                    newRow.Item("Building_PersPropCov_TotalPremium") = ""
                    newRow.Item("Building_PersPropOfOthersCov_Limit") = ""
                    newRow.Item("Building_PersPropOfOthersCov_ClassCode") = ""
                    newRow.Item("Building_PersPropOfOthersCov_ClassCodeDescription") = ""
                    newRow.Item("Building_PersPropOfOthersCov_CauseOfLoss") = ""
                    newRow.Item("Building_PersPropOfOthersCov_IncludedInBlanket") = ""
                    newRow.Item("Building_PersPropOfOthersCov_CoInsurance") = ""
                    newRow.Item("Building_PersPropOfOthersCov_IsAgreedValue") = "" ' Bug 4845 2-1-2016 Matt A
                    newRow.Item("Building_PersPropOfOthersCov_Valuation") = ""
                    newRow.Item("Building_PersPropOfOthersCov_Deductible") = ""
                    newRow.Item("Building_PersPropOfOthersCov_TheftDeductible") = ""
                    newRow.Item("Building_PersPropOfOthersCov_WindHailDeductible") = ""
                    newRow.Item("Building_PersPropOfOthersCov_InflationGuard") = ""
                    newRow.Item("Building_PersPropOfOthersCov_EQ_Deductible") = ""
                    newRow.Item("Building_PersPropOfOthersCov_EQ_Premium") = ""
                    newRow.Item("Building_PersPropOfOthersCov_TotalPremium") = ""
                    newRow.Item("Building_BusinessIncomeCov_Limit") = ""
                    newRow.Item("Building_BusinessIncomeCov_ClassCode") = ""
                    newRow.Item("Building_BusinessIncomeCov_ClassCodeDescription") = ""
                    newRow.Item("Building_BusinessIncomeCov_CauseOfLoss") = ""
                    newRow.Item("Building_BusinessIncomeCov_IncludedInBlanket") = ""
                    newRow.Item("Building_BusinessIncomeCov_CoInsurance") = ""
                    newRow.Item("Building_BusinessIncomeCov_IsAgreedValue") = "" ' Bug 4845 2-1-2016 Matt A
                    newRow.Item("Building_BusinessIncomeCov_Valuation") = ""
                    newRow.Item("Building_BusinessIncomeCov_Deductible") = ""
                    newRow.Item("Building_BusinessIncomeCov_TheftDeductible") = ""
                    newRow.Item("Building_BusinessIncomeCov_WindHailDeductible") = ""
                    newRow.Item("Building_BusinessIncomeCov_InflationGuard") = ""
                    newRow.Item("Building_BusinessIncomeCov_EQ_Deductible") = ""
                    newRow.Item("Building_BusinessIncomeCov_EQ_Premium") = ""
                    newRow.Item("Building_BusinessIncomeCov_TotalPremium") = ""

                    newRow.Item("AcctsRecSelected") = "Selected" 'added 2/14/2013 (styles and cleanup)
                    newRow.Item("AcctsRecOnPrem") = "60000"
                    newRow.Item("AcctsRecQuotedPremium") = "250"
                    newRow.Item("AcctsRecOffPrem") = "25000"
                    newRow.Item("ValPapersSelected") = "Selected" 'added 2/14/2013 (styles and cleanup)
                    newRow.Item("ValPapersOnPrem") = "25000"
                    newRow.Item("ValPapersQuotedPremium") = "250"
                    newRow.Item("ValPapersOffPrem") = "10000"
                    newRow.Item("CondoCommUnitOwnersSelected") = "Selected" 'added 2/14/2013 (styles and cleanup)
                    newRow.Item("CondoCommUnitOwners") = "25000"
                    newRow.Item("CondoCommUnitOwnersQuotedPremium") = "250"
                    newRow.Item("OrdOrLaw") = "Selected"
                    newRow.Item("OrdOrLawCov1") = "Not Selected"
                    newRow.Item("OrdOrLawCov1QuotedPremium") = "?"
                    newRow.Item("OrdOrLawCov2") = "50000"
                    newRow.Item("OrdOrLawCov2QuotedPremium") = "250"
                    newRow.Item("OrdOrLawCov3") = "50000"
                    newRow.Item("OrdOrLawCov3QuotedPremium") = "250"
                    newRow.Item("OrdOrLawCov4") = "100000"
                    newRow.Item("OrdOrLawCov4QuotedPremium") = "250"
                    newRow.Item("Spoilage") = "Not Selected"
                    newRow.Item("SpoilageQuotedPremium") = "?"
                    newRow.Item("SpoilagePropClass") = "?"
                    newRow.Item("SpoilageTotLimit") = "?"
                    newRow.Item("SpoilageRefrig") = "?"
                    newRow.Item("SpoilageBreakdown") = "?"
                    newRow.Item("SpoilagePowerOutage") = "?"
                    dt.Rows.Add(newRow)
                Else
                    For Each b As QuickQuoteBuilding In quickQuote.Locations(CInt(lblLocIndex.Text)).Buildings
                        Dim newRow As DataRow = dt.NewRow
                        newRow.Item("BuildingDescription") = b.Description
                        newRow.Item("BuildingProgram") = b.Program
                        newRow.Item("BuildingClassification") = b.Classification
                        newRow.Item("BuildingClassCode") = b.ClassCode
                        newRow.Item("BuildingOccupancy") = b.Occupancy
                        newRow.Item("BuildingConstruction") = b.Construction
                        newRow.Item("BuildingAutoIncrease") = b.AutoIncrease
                        newRow.Item("BuildingAutoIncreaseQuotedPremium") = ""
                        If qqHelper.doUseNewBOPVersion(quickQuote.EffectiveDate) AndAlso b.HasAutoIncreasePremium = True Then
                            If Not String.IsNullOrWhiteSpace(b.AutoIncreasePremium) AndAlso IsNumeric(b.AutoIncreasePremium) AndAlso (b.AutoIncreasePremium > 0 OrElse b.AutoIncreasePremium < 0) Then
                                newRow.Item("BuildingAutoIncreaseQuotedPremium") = b.AutoIncreasePremium
                            End If
                        End If
                        newRow.Item("BuildingPropDeduct") = b.PropertyDeductible
                        newRow.Item("BuildingLimit") = b.Limit
                        newRow.Item("BuildingLimitQuotedPremium") = b.LimitQuotedPremium
                        newRow.Item("BuildingValuation") = b.Valuation
                        newRow.Item("BuildingIncludedInBlanket") = qqHelper.getSelectedOrNotSelectedText(b.IsBuildingValIncludedInBlanketRating)
                        newRow.Item("BuildingMineSubsidence") = qqHelper.getSelectedOrNotSelectedText(b.HasMineSubsidence)
                        newRow.Item("BuildingMineSubsidenceQuotedPremium") = b.MineSubsidenceQuotedPremium
                        newRow.Item("BuildingSprinklered") = qqHelper.getSelectedOrNotSelectedText(b.HasSprinklered)
                        newRow.Item("PersPropLimit") = b.PersonalPropertyLimit
                        newRow.Item("PersPropLimitQuotedPremium") = b.PersonalPropertyLimitQuotedPremium
                        If qqHelper.doUseNewBOPVersion(quickQuote.EffectiveDate) Then
                            newRow.Item("BuildingACVRoofing") = qqHelper.getSelectedOrNotSelectedText(b.HasACVRoofing)
                        Else
                            newRow.Item("BuildingACVRoofing") = ""
                        End If
                        newRow.Item("BuildingValuationMethod") = b.ValuationMethod
                        newRow.Item("BuildingIncludedInBlanket2") = qqHelper.getSelectedOrNotSelectedText(b.IsValMethodIncludedInBlanketRating)
                        newRow.Item("BuildProtClass") = b.ProtectionClass
                        'added 10/16/2012
                        'newRow.Item("Build_CPR_TotalPremium") = b.CPR_Covs_TotalBuildingPremium
                        'updated 11/26/2012 to include EQ
                        newRow.Item("Build_CPR_TotalPremium") = b.CPR_Covs_TotalBuilding_With_EQ_Premium
                        Dim has_CPR_building_buildingCov As Boolean = False 'added 11/27/2012
                        If b.Limit <> "" OrElse (b.ValuationId <> "" AndAlso IsNumeric(b.ValuationId) = True) OrElse (b.ClassificationCode IsNot Nothing AndAlso b.ClassificationCode.ClassCode <> "") OrElse b.EarthquakeApplies = True OrElse (b.CauseOfLossTypeId <> "" AndAlso IsNumeric(b.CauseOfLossTypeId) = True) OrElse (b.CoinsuranceTypeId <> "" AndAlso IsNumeric(b.CoinsuranceTypeId) = True) OrElse (b.DeductibleId <> "" AndAlso IsNumeric(b.DeductibleId) = True) OrElse (b.RatingTypeId <> "" AndAlso IsNumeric(b.RatingTypeId) = True) OrElse (b.InflationGuardTypeId <> "" AndAlso IsNumeric(b.InflationGuardTypeId) = True) Then
                            has_CPR_building_buildingCov = True
                        End If
                        newRow.Item("BuildingCov_Limit") = If(has_CPR_building_buildingCov = True, b.Limit, "")
                        newRow.Item("BuildingCov_ClassCode") = If(has_CPR_building_buildingCov = True, b.ClassificationCode.ClassCode, "")
                        newRow.Item("BuildingCov_ClassCodeDescription") = If(has_CPR_building_buildingCov = True, b.ClassificationCode.ClassDescription, "")
                        newRow.Item("BuildingCov_CauseOfLoss") = If(has_CPR_building_buildingCov = True, b.CauseOfLossType, "")
                        newRow.Item("BuildingCov_IncludedInBlanket") = If(has_CPR_building_buildingCov = True, If(b.IsBuildingValIncludedInBlanketRating = True, "Yes", "No"), "") 'added 5/7/2013
                        newRow.Item("BuildingCov_CoInsurance") = If(has_CPR_building_buildingCov = True, b.CoinsuranceType, "")
                        newRow.Item("BuildingCov_IsAgreedValue") = If(has_CPR_building_buildingCov = True, If(b.IsAgreedValue = True, "Yes", "No"), "") ' Bug 4845 Matt A 2/1/2016
                        newRow.Item("BuildingCov_Valuation") = If(has_CPR_building_buildingCov = True, b.Valuation, "")
                        newRow.Item("BuildingCov_Deductible") = If(has_CPR_building_buildingCov = True, b.Deductible, "")
                        newRow.Item("BuildingCov_TheftDeductible") = If(has_CPR_building_buildingCov = True, b.OptionalTheftDeductible, "") 'added 5/7/2013
                        newRow.Item("BuildingCov_WindHailDeductible") = If(has_CPR_building_buildingCov = True, b.OptionalWindstormOrHailDeductible, "") 'added 5/7/2013
                        newRow.Item("BuildingCov_InflationGuard") = If(has_CPR_building_buildingCov = True, b.InflationGuardType, "")
                        Dim BuildingCov_EQ_Deduct As String = "" 'updated 11/28/2012
                        If b.CPR_BuildingLimit_EarthquakeBuildingClassificationPercentage <> "" AndAlso IsNumeric(b.CPR_BuildingLimit_EarthquakeBuildingClassificationPercentage) = True Then
                            BuildingCov_EQ_Deduct = b.CPR_BuildingLimit_EarthquakeBuildingClassificationPercentage & "%"
                        End If
                        newRow.Item("BuildingCov_EQ_Deductible") = If(has_CPR_building_buildingCov = True, BuildingCov_EQ_Deduct, "")
                        newRow.Item("BuildingCov_EQ_Premium") = If(has_CPR_building_buildingCov = True, b.EarthquakeQuotedPremium, "") 'added 11/13/2012 for CPR
                        'newRow.Item("BuildingCov_TotalPremium") = b.LimitQuotedPremium
                        'updated 11/26/2012 to include EQ
                        newRow.Item("BuildingCov_TotalPremium") = If(has_CPR_building_buildingCov = True, b.CPR_BuildingLimit_With_EQ_QuotedPremium, "")
                        Dim has_CPR_building_persPropCov As Boolean = False 'added 11/27/2012
                        If b.PersPropCov_PersonalPropertyLimit <> "" OrElse (b.PersPropCov_PropertyTypeId <> "" AndAlso IsNumeric(b.PersPropCov_PropertyTypeId) = True) OrElse (b.PersPropCov_RiskTypeId <> "" AndAlso IsNumeric(b.PersPropCov_RiskTypeId) = True) OrElse b.PersPropCov_EarthquakeApplies = True OrElse (b.PersPropCov_RatingTypeId <> "" AndAlso IsNumeric(b.PersPropCov_RatingTypeId) = True) OrElse (b.PersPropCov_CauseOfLossTypeId <> "" AndAlso IsNumeric(b.PersPropCov_CauseOfLossTypeId) = True) OrElse (b.PersPropCov_DeductibleId <> "" AndAlso IsNumeric(b.PersPropCov_DeductibleId) = True) OrElse (b.PersPropCov_CoinsuranceTypeId <> "" AndAlso IsNumeric(b.PersPropCov_CoinsuranceTypeId) = True) OrElse (b.PersPropCov_ValuationId <> "" AndAlso IsNumeric(b.PersPropCov_ValuationId) = True) OrElse (b.PersPropCov_ClassificationCode IsNot Nothing AndAlso b.PersPropCov_ClassificationCode.ClassCode <> "") OrElse b.PersPropOfOthers_PersonalPropertyLimit <> "" OrElse (b.PersPropOfOthers_RiskTypeId <> "" AndAlso IsNumeric(b.PersPropOfOthers_RiskTypeId) = True) OrElse b.PersPropOfOthers_EarthquakeApplies = True OrElse (b.PersPropOfOthers_RatingTypeId <> "" AndAlso IsNumeric(b.PersPropOfOthers_RatingTypeId) = True) OrElse (b.PersPropOfOthers_CauseOfLossTypeId <> "" AndAlso IsNumeric(b.PersPropOfOthers_CauseOfLossTypeId) = True) OrElse (b.PersPropOfOthers_DeductibleId <> "" AndAlso IsNumeric(b.PersPropOfOthers_DeductibleId) = True) OrElse (b.PersPropOfOthers_CoinsuranceTypeId <> "" AndAlso IsNumeric(b.PersPropOfOthers_CoinsuranceTypeId) = True) OrElse (b.PersPropOfOthers_ValuationId <> "" AndAlso IsNumeric(b.PersPropOfOthers_ValuationId) = True) OrElse (b.PersPropOfOthers_ClassificationCode IsNot Nothing AndAlso b.PersPropOfOthers_ClassificationCode.ClassCode <> "") Then
                            has_CPR_building_persPropCov = True
                        End If
                        newRow.Item("Building_PersPropCov_Limit") = If(has_CPR_building_persPropCov = True, b.PersPropCov_PersonalPropertyLimit, "")
                        newRow.Item("Building_PersPropCov_ClassCode") = If(has_CPR_building_persPropCov = True, b.PersPropCov_ClassificationCode.ClassCode, "")
                        newRow.Item("Building_PersPropCov_ClassCodeDescription") = If(has_CPR_building_persPropCov = True, b.PersPropCov_ClassificationCode.ClassDescription, "")
                        newRow.Item("Building_PersPropCov_CauseOfLoss") = If(has_CPR_building_persPropCov = True, b.PersPropCov_CauseOfLossType, "")
                        newRow.Item("Building_PersPropCov_IncludedInBlanket") = If(has_CPR_building_persPropCov = True, If(b.PersPropCov_IncludedInBlanketCoverage = True, "Yes", "No"), "") 'added 5/7/2013
                        newRow.Item("Building_PersPropCov_CoInsurance") = If(has_CPR_building_persPropCov = True, b.PersPropCov_CoinsuranceType, "")
                        newRow.Item("Building_PersPropCov_IsAgreedValue") = If(has_CPR_building_persPropCov = True, If(b.PersPropCov_IsAgreedValue = True, "Yes", "No"), "") ' Bug 4845 Matt A 2/1/2016
                        newRow.Item("Building_PersPropCov_Valuation") = If(has_CPR_building_persPropCov = True, b.PersPropCov_Valuation, "")
                        newRow.Item("Building_PersPropCov_Deductible") = If(has_CPR_building_persPropCov = True, b.PersPropCov_Deductible, "")
                        newRow.Item("Building_PersPropCov_TheftDeductible") = If(has_CPR_building_persPropCov = True, b.PersPropCov_OptionalTheftDeductible, "") 'added 5/7/2013
                        newRow.Item("Building_PersPropCov_WindHailDeductible") = If(has_CPR_building_persPropCov = True, b.PersPropCov_OptionalWindstormOrHailDeductible, "") 'added 5/7/2013
                        newRow.Item("Building_PersPropCov_InflationGuard") = If(has_CPR_building_persPropCov = True, "", "") '11/28/2012 - updated from NS to blank
                        Dim PersPropCov_EQ_Deduct As String = "" 'updated 11/28/2012
                        If b.CPR_PersPropCov_EarthquakeBuildingClassificationPercentage <> "" AndAlso IsNumeric(b.CPR_PersPropCov_EarthquakeBuildingClassificationPercentage) = True Then
                            PersPropCov_EQ_Deduct = b.CPR_PersPropCov_EarthquakeBuildingClassificationPercentage & "%"
                        End If
                        newRow.Item("Building_PersPropCov_EQ_Deductible") = If(has_CPR_building_persPropCov = True, PersPropCov_EQ_Deduct, "")
                        newRow.Item("Building_PersPropCov_EQ_Premium") = If(has_CPR_building_persPropCov = True, b.PersPropCov_EarthquakeQuotedPremium, "") 'added 11/13/2012 for CPR
                        'newRow.Item("Building_PersPropCov_TotalPremium") = b.PersPropCov_QuotedPremium
                        'updated 11/26/2012 to include EQ
                        newRow.Item("Building_PersPropCov_TotalPremium") = If(has_CPR_building_persPropCov = True, b.CPR_PersPropCov_With_EQ_QuotedPremium, "")
                        Dim has_CPR_building_persPropOfOthers As Boolean = False 'added 11/27/2012
                        If b.PersPropOfOthers_PersonalPropertyLimit <> "" OrElse (b.PersPropOfOthers_RiskTypeId <> "" AndAlso IsNumeric(b.PersPropOfOthers_RiskTypeId) = True) OrElse b.PersPropOfOthers_EarthquakeApplies = True OrElse (b.PersPropOfOthers_RatingTypeId <> "" AndAlso IsNumeric(b.PersPropOfOthers_RatingTypeId) = True) OrElse (b.PersPropOfOthers_CauseOfLossTypeId <> "" AndAlso IsNumeric(b.PersPropOfOthers_CauseOfLossTypeId) = True) OrElse (b.PersPropOfOthers_DeductibleId <> "" AndAlso IsNumeric(b.PersPropOfOthers_DeductibleId) = True) OrElse (b.PersPropOfOthers_CoinsuranceTypeId <> "" AndAlso IsNumeric(b.PersPropOfOthers_CoinsuranceTypeId) = True) OrElse (b.PersPropOfOthers_ValuationId <> "" AndAlso IsNumeric(b.PersPropOfOthers_ValuationId) = True) OrElse (b.PersPropOfOthers_ClassificationCode IsNot Nothing AndAlso b.PersPropOfOthers_ClassificationCode.ClassCode <> "") Then
                            has_CPR_building_persPropOfOthers = True
                        End If
                        newRow.Item("Building_PersPropOfOthersCov_Limit") = If(has_CPR_building_persPropOfOthers = True, b.PersPropOfOthers_PersonalPropertyLimit, "")
                        newRow.Item("Building_PersPropOfOthersCov_ClassCode") = If(has_CPR_building_persPropOfOthers = True, b.PersPropOfOthers_ClassificationCode.ClassCode, "")
                        newRow.Item("Building_PersPropOfOthersCov_ClassCodeDescription") = If(has_CPR_building_persPropOfOthers = True, b.PersPropOfOthers_ClassificationCode.ClassDescription, "")
                        newRow.Item("Building_PersPropOfOthersCov_CauseOfLoss") = If(has_CPR_building_persPropOfOthers = True, b.PersPropOfOthers_CauseOfLossType, "")
                        newRow.Item("Building_PersPropOfOthersCov_IncludedInBlanket") = If(has_CPR_building_persPropOfOthers = True, If(b.PersPropOfOthers_IncludedInBlanketCoverage = True, "Yes", "No"), "") 'added 5/7/2013
                        newRow.Item("Building_PersPropOfOthersCov_CoInsurance") = If(has_CPR_building_persPropOfOthers = True, b.PersPropOfOthers_CoinsuranceType, "")
                        newRow.Item("Building_PersPropOfOthersCov_IsAgreedValue") = If(has_CPR_building_persPropOfOthers = True, "No", "") ' Bug 4845 Matt A 2/1/2016
                        newRow.Item("Building_PersPropOfOthersCov_Valuation") = If(has_CPR_building_persPropOfOthers = True, b.PersPropOfOthers_Valuation, "")
                        newRow.Item("Building_PersPropOfOthersCov_Deductible") = If(has_CPR_building_persPropOfOthers = True, b.PersPropOfOthers_Deductible, "")
                        newRow.Item("Building_PersPropOfOthersCov_TheftDeductible") = If(has_CPR_building_persPropOfOthers = True, b.PersPropOfOthers_OptionalTheftDeductible, "") 'added 5/7/2013
                        newRow.Item("Building_PersPropOfOthersCov_WindHailDeductible") = If(has_CPR_building_persPropOfOthers = True, b.PersPropOfOthers_OptionalWindstormOrHailDeductible, "") 'added 5/7/2013
                        newRow.Item("Building_PersPropOfOthersCov_InflationGuard") = If(has_CPR_building_persPropOfOthers = True, "", "") '11/28/2012 - updated from NS to blank
                        Dim PersPropOfOthers_EQ_Deduct As String = "" 'updated 11/28/2012
                        If b.CPR_PersPropOfOthers_EarthquakeBuildingClassificationPercentage <> "" AndAlso IsNumeric(b.CPR_PersPropOfOthers_EarthquakeBuildingClassificationPercentage) = True Then
                            PersPropOfOthers_EQ_Deduct = b.CPR_PersPropOfOthers_EarthquakeBuildingClassificationPercentage & "%"
                        End If
                        newRow.Item("Building_PersPropOfOthersCov_EQ_Deductible") = If(has_CPR_building_persPropOfOthers = True, PersPropOfOthers_EQ_Deduct, "")
                        newRow.Item("Building_PersPropOfOthersCov_EQ_Premium") = If(has_CPR_building_persPropOfOthers = True, b.PersPropOfOthers_EarthquakeQuotedPremium, "") 'added 11/13/2012 for CPR
                        'newRow.Item("Building_PersPropOfOthersCov_TotalPremium") = b.PersPropOfOthers_QuotedPremium
                        'updated 11/26/2012 to include EQ
                        newRow.Item("Building_PersPropOfOthersCov_TotalPremium") = If(has_CPR_building_persPropOfOthers = True, b.CPR_PersPropOfOthers_With_EQ_QuotedPremium, "")
                        Dim has_CPR_building_busIncomeCov As Boolean = False 'added 11/27/2012
                        If b.BusinessIncomeCov_Limit <> "" OrElse (b.BusinessIncomeCov_CoinsuranceTypeId <> "" AndAlso IsNumeric(b.BusinessIncomeCov_CoinsuranceTypeId) = True) OrElse (b.BusinessIncomeCov_MonthlyPeriodTypeId <> "" AndAlso IsNumeric(b.BusinessIncomeCov_MonthlyPeriodTypeId) = True) OrElse (b.BusinessIncomeCov_BusinessIncomeTypeId <> "" AndAlso IsNumeric(b.BusinessIncomeCov_BusinessIncomeTypeId) = True) OrElse (b.BusinessIncomeCov_RiskTypeId <> "" AndAlso IsNumeric(b.BusinessIncomeCov_RiskTypeId) = True) OrElse (b.BusinessIncomeCov_RatingTypeId <> "" AndAlso IsNumeric(b.BusinessIncomeCov_RatingTypeId) = True) OrElse (b.BusinessIncomeCov_CauseOfLossTypeId <> "" AndAlso IsNumeric(b.BusinessIncomeCov_CauseOfLossTypeId) = True) OrElse (b.BusinessIncomeCov_ClassificationCode IsNot Nothing AndAlso b.BusinessIncomeCov_ClassificationCode.ClassCode <> "") OrElse b.BusinessIncomeCov_EarthquakeApplies = True Then
                            has_CPR_building_busIncomeCov = True
                        End If
                        newRow.Item("Building_BusinessIncomeCov_Limit") = If(has_CPR_building_busIncomeCov = True, b.BusinessIncomeCov_Limit, "")
                        newRow.Item("Building_BusinessIncomeCov_ClassCode") = If(has_CPR_building_busIncomeCov = True, b.BusinessIncomeCov_ClassificationCode.ClassCode, "")
                        newRow.Item("Building_BusinessIncomeCov_ClassCodeDescription") = If(has_CPR_building_busIncomeCov = True, b.BusinessIncomeCov_ClassificationCode.ClassDescription, "")
                        newRow.Item("Building_BusinessIncomeCov_CauseOfLoss") = If(has_CPR_building_busIncomeCov = True, b.BusinessIncomeCov_CauseOfLossType, "")
                        newRow.Item("Building_BusinessIncomeCov_IncludedInBlanket") = If(has_CPR_building_busIncomeCov = True, If(b.BusinessIncomeCov_IncludedInBlanketCoverage = True, "Yes", "No"), "") 'added 5/7/2013
                        newRow.Item("Building_BusinessIncomeCov_CoInsurance") = If(has_CPR_building_busIncomeCov = True, b.BusinessIncomeCov_CoinsuranceType, "")
                        newRow.Item("Building_BusinessIncomeCov_IsAgreedValue") = If(has_CPR_building_busIncomeCov = True, "No", "") ' Bug 4845 Matt A 2/1/2016
                        newRow.Item("Building_BusinessIncomeCov_Valuation") = If(has_CPR_building_busIncomeCov = True, "", "") '11/28/2012 - updated from NS to blank
                        newRow.Item("Building_BusinessIncomeCov_Deductible") = If(has_CPR_building_busIncomeCov = True, "", "") '11/28/2012 - updated from NS to blank
                        newRow.Item("Building_BusinessIncomeCov_TheftDeductible") = If(has_CPR_building_busIncomeCov = True, "", "") 'added 5/7/2013
                        newRow.Item("Building_BusinessIncomeCov_WindHailDeductible") = If(has_CPR_building_busIncomeCov = True, "", "") 'added 5/7/2013
                        newRow.Item("Building_BusinessIncomeCov_InflationGuard") = If(has_CPR_building_busIncomeCov = True, "", "") '11/28/2012 - updated from NS to blank
                        Dim BusIncomeCov_EQ_Deduct As String = "" 'updated 11/28/2012
                        If b.CPR_BusinessIncomeCov_EarthquakeBuildingClassificationPercentage <> "" AndAlso IsNumeric(b.CPR_BusinessIncomeCov_EarthquakeBuildingClassificationPercentage) = True Then
                            BusIncomeCov_EQ_Deduct = b.CPR_BusinessIncomeCov_EarthquakeBuildingClassificationPercentage & "%"
                        End If
                        newRow.Item("Building_BusinessIncomeCov_EQ_Deductible") = If(has_CPR_building_busIncomeCov = True, BusIncomeCov_EQ_Deduct, "")
                        newRow.Item("Building_BusinessIncomeCov_EQ_Premium") = If(has_CPR_building_busIncomeCov = True, b.BusinessIncomeCov_EarthquakeQuotedPremium, "") 'added 11/13/2012 for CPR
                        'newRow.Item("Building_BusinessIncomeCov_TotalPremium") = b.BusinessIncomeCov_QuotedPremium
                        'updated 11/26/2012 to include EQ
                        newRow.Item("Building_BusinessIncomeCov_TotalPremium") = If(has_CPR_building_busIncomeCov = True, b.CPR_BusinessIncomeCov_With_EQ_QuotedPremium, "")

                        '2/14/2013 - added IF statements to set some fields to N/A if blank (styles and cleanup)
                        'newRow.Item("AcctsRecOnPrem") = b.AccountsReceivableOnPremises
                        newRow.Item("AcctsRecOnPrem") = If(b.AccountsReceivableOnPremises <> "", b.AccountsReceivableOnPremises, "N/A")
                        newRow.Item("AcctsRecQuotedPremium") = b.AccountsReceivableQuotedPremium
                        'newRow.Item("AcctsRecOffPrem") = b.AccountsReceivableOffPremises
                        newRow.Item("AcctsRecOffPrem") = If(b.AccountsReceivableOffPremises <> "", b.AccountsReceivableOffPremises, "N/A")
                        newRow.Item("AcctsRecSelected") = If((b.AccountsReceivableOnPremises = "" OrElse b.AccountsReceivableOnPremises = "N/A") AndAlso (b.AccountsReceivableOffPremises = "" OrElse b.AccountsReceivableOffPremises = "N/A"), qqHelper.getSelectedOrNotSelectedText(False), qqHelper.getSelectedOrNotSelectedText(True)) 'added 2/14/2013 (styles and cleanup)
                        'newRow.Item("ValPapersOnPrem") = b.ValuablePapersOnPremises
                        newRow.Item("ValPapersOnPrem") = If(b.ValuablePapersOnPremises <> "", b.ValuablePapersOnPremises, "N/A")
                        newRow.Item("ValPapersQuotedPremium") = b.ValuablePapersQuotedPremium
                        'newRow.Item("ValPapersOffPrem") = b.ValuablePapersOffPremises
                        newRow.Item("ValPapersOffPrem") = If(b.ValuablePapersOffPremises <> "", b.ValuablePapersOffPremises, "N/A")
                        newRow.Item("ValPapersSelected") = If((b.ValuablePapersOnPremises = "" OrElse b.ValuablePapersOnPremises = "N/A") AndAlso (b.ValuablePapersOffPremises = "" OrElse b.ValuablePapersOffPremises = "N/A"), qqHelper.getSelectedOrNotSelectedText(False), qqHelper.getSelectedOrNotSelectedText(True)) 'added 2/14/2013 (styles and cleanup)
                        'newRow.Item("CondoCommUnitOwners") = b.CondoCommercialUnitOwnersLimit
                        newRow.Item("CondoCommUnitOwners") = If(b.CondoCommercialUnitOwnersLimit <> "", b.CondoCommercialUnitOwnersLimit, "N/A")
                        newRow.Item("CondoCommUnitOwnersSelected") = If(b.CondoCommercialUnitOwnersLimit = "" OrElse b.CondoCommercialUnitOwnersLimit = "N/A", qqHelper.getSelectedOrNotSelectedText(False), qqHelper.getSelectedOrNotSelectedText(True)) 'added 2/14/2013 (styles and cleanup)
                        newRow.Item("CondoCommUnitOwnersQuotedPremium") = b.CondoCommercialUnitOwnersLimitQuotedPremium
                        newRow.Item("OrdOrLaw") = qqHelper.getSelectedOrNotSelectedText(b.HasOrdinanceOrLaw)
                        newRow.Item("OrdOrLawCov1") = qqHelper.getSelectedOrNotSelectedText(b.HasOrdOrLawUndamagedPortion)
                        newRow.Item("OrdOrLawCov1QuotedPremium") = b.OrdOrLawUndamagedPortionQuotedPremium
                        newRow.Item("OrdOrLawCov2") = b.OrdOrLawDemoCostLimit
                        newRow.Item("OrdOrLawCov2QuotedPremium") = b.OrdOrLawDemoCostLimitQuotedPremium
                        newRow.Item("OrdOrLawCov3") = b.OrdOrLawIncreasedCostLimit
                        newRow.Item("OrdOrLawCov3QuotedPremium") = b.OrdOrLawIncreaseCostLimitQuotedPremium
                        newRow.Item("OrdOrLawCov4") = b.OrdOrLawDemoAndIncreasedCostLimit
                        newRow.Item("OrdOrLawCov4QuotedPremium") = b.OrdOrLawDemoAndIncreasedCostLimitQuotedPremium
                        newRow.Item("Spoilage") = qqHelper.getSelectedOrNotSelectedText(b.HasSpoilage)
                        newRow.Item("SpoilageQuotedPremium") = b.SpoilageQuotedPremium
                        newRow.Item("SpoilagePropClass") = b.SpoilagePropertyClassification
                        newRow.Item("SpoilageTotLimit") = b.SpoilageTotalLimit
                        newRow.Item("SpoilageRefrig") = qqHelper.getSelectedOrNotSelectedText(b.IsSpoilageRefrigerationMaintenanceAgreement)
                        newRow.Item("SpoilageBreakdown") = qqHelper.getSelectedOrNotSelectedText(b.IsSpoilageBreakdownOrContamination)
                        newRow.Item("SpoilagePowerOutage") = qqHelper.getSelectedOrNotSelectedText(b.IsSpoilagePowerOutage)
                        dt.Rows.Add(newRow)
                    Next
                End If

                rptBuildings.DataSource = dt
                rptBuildings.DataBind()
                SetupBuildingsRepeater(rptBuildings)
            Else
                BuildingInformationRepeaterRow.Visible = False
            End If

            'added 5/6/2013 for PITO
            If lblLocIndex.Text = "test" OrElse (quickQuote IsNot Nothing AndAlso quickQuote.Locations IsNot Nothing AndAlso IsNumeric(lblLocIndex.Text) = True AndAlso (quickQuote.Locations.Count - 1) >= CInt(lblLocIndex.Text) AndAlso quickQuote.Locations(CInt(lblLocIndex.Text)).PropertyInTheOpenRecords IsNot Nothing AndAlso quickQuote.Locations(CInt(lblLocIndex.Text)).PropertyInTheOpenRecords.Count > 0) Then
                PropertyInTheOpenRow.Visible = True

                Dim dt As New DataTable
                dt.Columns.Add("PITONum", System.Type.GetType("System.String"))
                dt.Columns.Add("Description", System.Type.GetType("System.String"))
                dt.Columns.Add("Limit", System.Type.GetType("System.String"))
                dt.Columns.Add("CauseOfLoss", System.Type.GetType("System.String"))
                dt.Columns.Add("IncludedInBlanket", System.Type.GetType("System.String"))
                dt.Columns.Add("CoIns", System.Type.GetType("System.String"))
                dt.Columns.Add("Valuation", System.Type.GetType("System.String"))
                dt.Columns.Add("ClassCode", System.Type.GetType("System.String"))
                dt.Columns.Add("Deductible", System.Type.GetType("System.String"))
                dt.Columns.Add("EqPrem", System.Type.GetType("System.String"))
                dt.Columns.Add("TotalPrem", System.Type.GetType("System.String"))

                If lblLocIndex.Text = "test" Then
                    Dim newRow As DataRow = dt.NewRow
                    newRow.Item("PITONum") = "1"
                    newRow.Item("Description") = "Test Prop in the Open"
                    newRow.Item("Limit") = "20,000"
                    newRow.Item("CauseOfLoss") = "Broad Form"
                    newRow.Item("IncludedInBlanket") = "Yes"
                    newRow.Item("CoIns") = "N/A"
                    newRow.Item("Valuation") = "Replacement Cost"
                    newRow.Item("ClassCode") = "1190 - Aircraft Stored In The Open"
                    newRow.Item("Deductible") = "500"
                    newRow.Item("EqPrem") = "$10.00"
                    newRow.Item("TotalPrem") = "$100.00"
                    dt.Rows.Add(newRow)
                Else
                    Dim pitoCounter As Integer = 0
                    For Each p As QuickQuotePropertyInTheOpenRecord In quickQuote.Locations(CInt(lblLocIndex.Text)).PropertyInTheOpenRecords
                        pitoCounter += 1
                        Dim newRow As DataRow = dt.NewRow
                        newRow.Item("PITONum") = pitoCounter.ToString
                        newRow.Item("Description") = p.Description
                        newRow.Item("Limit") = p.Limit
                        newRow.Item("CauseOfLoss") = p.CauseOfLossType
                        newRow.Item("IncludedInBlanket") = If(p.IncludedInBlanketCoverage = True, "Yes", "No")
                        newRow.Item("CoIns") = p.CoinsuranceType
                        newRow.Item("Valuation") = p.Valuation
                        newRow.Item("ClassCode") = qqHelper.appendText(p.SpecialClassCode, p.SpecialClassCodeType, " - ")
                        newRow.Item("Deductible") = p.Deductible
                        newRow.Item("EqPrem") = p.EarthquakeQuotedPremium
                        newRow.Item("TotalPrem") = p.QuotedPremium_With_EQ
                        dt.Rows.Add(newRow)
                    Next
                End If

                dgrdPropertyInTheOpen.DataSource = dt
                dgrdPropertyInTheOpen.DataBind()
            Else
                PropertyInTheOpenRow.Visible = False
            End If

            If lblLocIndex.Text = "test" OrElse (quickQuote IsNot Nothing AndAlso quickQuote.Locations IsNot Nothing AndAlso IsNumeric(lblLocIndex.Text) = True AndAlso (quickQuote.Locations.Count - 1) >= CInt(lblLocIndex.Text) AndAlso quickQuote.Locations(CInt(lblLocIndex.Text)).Classifications IsNot Nothing AndAlso quickQuote.Locations(CInt(lblLocIndex.Text)).Classifications.Count > 0) Then
                ClassCodesInformationRepeaterRow.Visible = True
                'ClassCodesHeaderSpacerRow.Visible = True 'added 2/19/2013; not used since on header row

                Dim dt As New DataTable
                dt.Columns.Add("ClassCode", System.Type.GetType("System.String"))
                dt.Columns.Add("ClassCodeDescription", System.Type.GetType("System.String"))
                dt.Columns.Add("ClassCodeEmpPayroll", System.Type.GetType("System.String"))
                dt.Columns.Add("ClassCodeEmpNum", System.Type.GetType("System.String"))
                'added 8/20/2012
                dt.Columns.Add("ClassCodePremium", System.Type.GetType("System.String"))

                If lblLocIndex.Text = "test" Then
                    Dim newRow As DataRow = dt.NewRow
                    newRow.Item("ClassCode") = ""
                    newRow.Item("ClassCodeDescription") = "Abrasive paper or cloth preparation"
                    newRow.Item("ClassCodeEmpPayroll") = "30000"
                    newRow.Item("ClassCodeEmpNum") = "2"
                    newRow.Item("ClassCodePremium") = "50"
                    dt.Rows.Add(newRow)
                Else
                    For Each cls As QuickQuoteClassification In quickQuote.Locations(CInt(lblLocIndex.Text)).Classifications
                        Dim newRow As DataRow = dt.NewRow
                        newRow.Item("ClassCode") = cls.ClassCode
                        newRow.Item("ClassCodeDescription") = cls.Description
                        newRow.Item("ClassCodeEmpPayroll") = cls.Payroll
                        newRow.Item("ClassCodeEmpNum") = cls.NumberOfEmployees
                        newRow.Item("ClassCodePremium") = cls.QuotedPremium
                        dt.Rows.Add(newRow)
                    Next
                End If

                rptClassCodes.DataSource = dt
                rptClassCodes.DataBind()
                SetupLocClassCodesRepeater(rptClassCodes)
            Else
                ClassCodesInformationRepeaterRow.Visible = False
                'ClassCodesHeaderSpacerRow.Visible = False 'added 2/19/2013; not used since on header row
            End If

            'added 8/27/2012 for GL
            If lblLocIndex.Text = "test" OrElse (quickQuote IsNot Nothing AndAlso quickQuote.Locations IsNot Nothing AndAlso IsNumeric(lblLocIndex.Text) = True AndAlso (quickQuote.Locations.Count - 1) >= CInt(lblLocIndex.Text) AndAlso quickQuote.Locations(CInt(lblLocIndex.Text)).GLClassifications IsNot Nothing AndAlso quickQuote.Locations(CInt(lblLocIndex.Text)).GLClassifications.Count > 0) Then
                GL_ClassCodesInformationRepeaterRow.Visible = True
                'LocationGlClassCodeHeaderSpacerRow.Visible = True 'added 2/19/2013; not used since on header row

                Dim dt As New DataTable
                dt.Columns.Add("ClassCode", System.Type.GetType("System.String"))
                dt.Columns.Add("ClassCodeDescription", System.Type.GetType("System.String"))
                dt.Columns.Add("ClassCodePremExp", System.Type.GetType("System.String"))
                dt.Columns.Add("ClassCodePremBase", System.Type.GetType("System.String"))
                dt.Columns.Add("ClassCodeProductsPrem", System.Type.GetType("System.String"))
                dt.Columns.Add("ClassCodePremisesPrem", System.Type.GetType("System.String"))

                If lblLocIndex.Text = "test" Then
                    Dim newRow As DataRow = dt.NewRow
                    newRow.Item("ClassCode") = "50017"
                    newRow.Item("ClassCodeDescription") = "Abrasives or Abrasive Products Mfg."
                    newRow.Item("ClassCodePremExp") = "10,000"
                    newRow.Item("ClassCodePremBase") = "Gross Sales"
                    newRow.Item("ClassCodeProductsPrem") = "100"
                    newRow.Item("ClassCodePremisesPrem") = "100"
                    dt.Rows.Add(newRow)
                Else
                    For Each cls As QuickQuoteGLClassification In quickQuote.Locations(CInt(lblLocIndex.Text)).GLClassifications
                        Dim newRow As DataRow = dt.NewRow
                        newRow.Item("ClassCode") = cls.ClassCode
                        newRow.Item("ClassCodeDescription") = cls.ClassDescription
                        newRow.Item("ClassCodePremExp") = cls.PremiumExposure
                        newRow.Item("ClassCodePremBase") = cls.PremiumBase
                        newRow.Item("ClassCodeProductsPrem") = cls.ProductsQuotedPremium
                        newRow.Item("ClassCodePremisesPrem") = cls.PremisesQuotedPremium
                        dt.Rows.Add(newRow)
                    Next
                End If

                rpt_GL_ClassCodes.DataSource = dt
                rpt_GL_ClassCodes.DataBind()
                SetupLocGlClassCodesRepeater(rpt_GL_ClassCodes)
            Else
                GL_ClassCodesInformationRepeaterRow.Visible = False
                'LocationGlClassCodeHeaderSpacerRow.Visible = False 'added 2/19/2013; not used since on header row
            End If


        End If
    End Sub
    Private Sub SetupBuildingsRepeater(ByVal rptBuildings As Repeater)
        For Each i As RepeaterItem In rptBuildings.Items
            'Dim BuildingHeaderSpacerRow As HtmlTableRow = i.FindControl("BuildingHeaderSpacerRow") 'added 2/19/2013; removed since on header and not on item
            Dim BuildProgramRow As HtmlTableRow = i.FindControl("BuildProgramRow")
            Dim BuildClassificationRow As HtmlTableRow = i.FindControl("BuildClassificationRow")
            Dim BuildClassCodeRow As HtmlTableRow = i.FindControl("BuildClassCodeRow")
            Dim BuildOccupancyRow As HtmlTableRow = i.FindControl("BuildOccupancyRow") 'added 10/11/2012 (should have already been hiding/showing based on LOB)
            Dim BuildConstructionRow As HtmlTableRow = i.FindControl("BuildConstructionRow")
            BuildAutoIncreaseRow = i.FindControl("BuildAutoIncreaseRow") 'Changed to global variable so that it could be accessed else where for easier show/hide ability
            Dim BuildPropDeductRow As HtmlTableRow = i.FindControl("BuildPropDeductRow")
            Dim BuildLimitRow As HtmlTableRow = i.FindControl("BuildLimitRow")
            Dim BuildValuationRow As HtmlTableRow = i.FindControl("BuildValuationRow")
            Dim BuildValuationBlanketRow As HtmlTableRow = i.FindControl("BuildValuationBlanketRow")
            Dim BuildMineSubsidenceRow As HtmlTableRow = i.FindControl("BuildMineSubsidenceRow")
            Dim BuildSprinkleredRow As HtmlTableRow = i.FindControl("BuildSprinkleredRow")
            Dim BuildPersPropLimitRow As HtmlTableRow = i.FindControl("BuildPersPropLimitRow")
            Dim BuildACVRoofingRow As HtmlTableRow
            If qqHelper.doUseNewBOPVersion(quickQuote.EffectiveDate) Then
                BuildACVRoofingRow = i.FindControl("BuildACVRoofingRow")
            End If
            Dim BuildValuationMethodRow As HtmlTableRow = i.FindControl("BuildValuationMethodRow")
            Dim BuildValuationMethodBlanketRow As HtmlTableRow = i.FindControl("BuildValuationMethodBlanketRow")
            Dim BuildProtClassRow As HtmlTableRow = i.FindControl("BuildProtClassRow")
            Dim Build_CPR_TotalPremiumRow As HtmlTableRow = i.FindControl("Build_CPR_TotalPremiumRow") 'added 10/16/2012 for CPR
            Dim Build_CPR_Covs_Row As HtmlTableRow = i.FindControl("Build_CPR_Covs_Row") 'added 10/16/2012 for CPR

            Dim OptionalBuildingCoveragesSpacerRow As HtmlTableRow = i.FindControl("OptionalBuildingCoveragesSpacerRow") 'added 10/11/2012 to hide/show based on LOB
            Dim OptionalBuildingCoveragesMainRow As HtmlTableRow = i.FindControl("OptionalBuildingCoveragesMainRow") 'added 10/11/2012 to hide/show based on LOB
            Dim OptionalBuildingCoveragesHeaderSpacerRow As HtmlTableRow = i.FindControl("OptionalBuildingCoveragesHeaderSpacerRow") 'added 2/19/2013
            Dim OptionalBuildingCoveragesQuotedPremiumRow As HtmlTableRow = i.FindControl("OptionalBuildingCoveragesQuotedPremiumRow") 'added 10/11/2012 to hide/show based on LOB
            'added 2/14/2013 (styles and cleanup)
            Dim BuildAcctsRecSelectedRow As HtmlTableRow = i.FindControl("BuildAcctsRecSelectedRow")
            Dim BuildAcctsRecOnPremisesRow As HtmlTableRow = i.FindControl("BuildAcctsRecOnPremisesRow")
            Dim BuildAcctsRecOffPremisesRow As HtmlTableRow = i.FindControl("BuildAcctsRecOffPremisesRow")
            'added 2/14/2013 (styles and cleanup)
            Dim BuildValuablePapersSelectedRow As HtmlTableRow = i.FindControl("BuildValuablePapersSelectedRow")
            Dim BuildValuablePapersOnPremisesRow As HtmlTableRow = i.FindControl("BuildValuablePapersOnPremisesRow")
            Dim BuildValuablePapersOffPremisesRow As HtmlTableRow = i.FindControl("BuildValuablePapersOffPremisesRow")
            'added 2/14/2013 (styles and cleanup)
            Dim BuildCondoCommercialUnitLimitSelectedRow As HtmlTableRow = i.FindControl("BuildCondoCommercialUnitLimitSelectedRow")
            Dim BuildCondoCommercialUnitLimitRow As HtmlTableRow = i.FindControl("BuildCondoCommercialUnitLimitRow")
            Dim BuildOrdOrLawMainRow As HtmlTableRow = i.FindControl("BuildOrdOrLawMainRow")
            Dim BuildOrdOrLawUndamagedRow As HtmlTableRow = i.FindControl("BuildOrdOrLawUndamagedRow")
            Dim BuildOrdOrLawDemoRow As HtmlTableRow = i.FindControl("BuildOrdOrLawDemoRow")
            Dim BuildOrdOrLawIncreasedCostRow As HtmlTableRow = i.FindControl("BuildOrdOrLawIncreasedCostRow")
            Dim BuildOrdOrLawDemoAndIncreasedCostRow As HtmlTableRow = i.FindControl("BuildOrdOrLawDemoAndIncreasedCostRow")
            Dim BuildSpoilageMainRow As HtmlTableRow = i.FindControl("BuildSpoilageMainRow")
            Dim BuildSpoilagePropClassificationRow As HtmlTableRow = i.FindControl("BuildSpoilagePropClassificationRow")
            Dim BuildSpoilageLimitRow As HtmlTableRow = i.FindControl("BuildSpoilageLimitRow")
            Dim BuildSpoilageRefrigRow As HtmlTableRow = i.FindControl("BuildSpoilageRefrigRow")
            Dim BuildSpoilageBreakdownRow As HtmlTableRow = i.FindControl("BuildSpoilageBreakdownRow")
            Dim BuildSpoilagePowerOutageRow As HtmlTableRow = i.FindControl("BuildSpoilagePowerOutageRow")

            Dim lblAcctsRecSelected As Label = i.FindControl("lblAcctsRecSelected") 'added 2/14/2013
            Dim lblValPapersSelected As Label = i.FindControl("lblValPapersSelected") 'added 2/14/2013
            Dim lblCondoCommUnitOwnersSelected As Label = i.FindControl("lblCondoCommUnitOwnersSelected") 'added 2/14/2013
            Dim lblOrdOrLaw As Label = i.FindControl("lblOrdOrLaw")
            Dim lblSpoilage As Label = i.FindControl("lblSpoilage")

            'BuildingHeaderSpacerRow.Visible = True 'added 2/19/2013; all LOBs; removed since on header and not on item
            BuildProgramRow.Visible = False 'BOP
            BuildClassificationRow.Visible = False 'BOP
            BuildClassCodeRow.Visible = False 'BOP
            BuildOccupancyRow.Visible = False 'BOP
            BuildConstructionRow.Visible = False 'BOP
            BuildAutoIncreaseRow.Visible = False 'BOP
            BuildPropDeductRow.Visible = False 'BOP
            BuildLimitRow.Visible = False 'BOP
            BuildValuationRow.Visible = False 'BOP
            BuildValuationBlanketRow.Visible = False 'BOP
            BuildMineSubsidenceRow.Visible = False 'BOP
            BuildSprinkleredRow.Visible = False 'BOP
            BuildPersPropLimitRow.Visible = False 'BOP
            If qqHelper.doUseNewBOPVersion(quickQuote.EffectiveDate) Then
                BuildACVRoofingRow.Visible = False
            End If
            BuildValuationMethodRow.Visible = False 'BOP
            BuildValuationMethodBlanketRow.Visible = False 'BOP
            BuildProtClassRow.Visible = False 'BOP
            Build_CPR_TotalPremiumRow.Visible = False 'CPR
            Build_CPR_Covs_Row.Visible = False 'CPR

            OptionalBuildingCoveragesSpacerRow.Visible = False 'BOP
            OptionalBuildingCoveragesMainRow.Visible = False 'BOP
            OptionalBuildingCoveragesHeaderSpacerRow.Visible = False 'added 2/19/2013; BOP
            OptionalBuildingCoveragesQuotedPremiumRow.Visible = False 'BOP
            BuildAcctsRecSelectedRow.Visible = False 'BOP; added 2/14/2013
            BuildAcctsRecOnPremisesRow.Visible = False 'BOP
            BuildAcctsRecOffPremisesRow.Visible = False 'BOP
            BuildValuablePapersSelectedRow.Visible = False 'BOP; added 2/14/2013
            BuildValuablePapersOnPremisesRow.Visible = False 'BOP
            BuildValuablePapersOffPremisesRow.Visible = False 'BOP
            BuildCondoCommercialUnitLimitSelectedRow.Visible = False 'BOP; added 2/14/2013
            BuildCondoCommercialUnitLimitRow.Visible = False 'BOP
            BuildOrdOrLawMainRow.Visible = False 'BOP
            BuildOrdOrLawUndamagedRow.Visible = False 'BOP
            BuildOrdOrLawDemoRow.Visible = False 'BOP
            BuildOrdOrLawIncreasedCostRow.Visible = False 'BOP
            BuildOrdOrLawDemoAndIncreasedCostRow.Visible = False 'BOP
            BuildSpoilageMainRow.Visible = False 'BOP
            BuildSpoilagePropClassificationRow.Visible = False 'BOP
            BuildSpoilageLimitRow.Visible = False 'BOP
            BuildSpoilageRefrigRow.Visible = False 'BOP
            BuildSpoilageBreakdownRow.Visible = False 'BOP
            BuildSpoilagePowerOutageRow.Visible = False 'BOP

            If quickQuote IsNot Nothing AndAlso quickQuote.LobType <> Nothing AndAlso quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP Then
                BuildProgramRow.Visible = True
                BuildClassificationRow.Visible = True
                BuildClassCodeRow.Visible = True
                BuildOccupancyRow.Visible = True
                BuildConstructionRow.Visible = True
                BuildAutoIncreaseRow.Visible = True
                BuildPropDeductRow.Visible = True
                BuildLimitRow.Visible = True
                BuildValuationRow.Visible = True
                BuildValuationBlanketRow.Visible = True
                BuildMineSubsidenceRow.Visible = True
                BuildSprinkleredRow.Visible = True
                BuildPersPropLimitRow.Visible = True
                If qqHelper.doUseNewBOPVersion(quickQuote.EffectiveDate) Then
                    BuildACVRoofingRow.Visible = True
                End If
                BuildValuationMethodRow.Visible = True
                BuildValuationMethodBlanketRow.Visible = True
                BuildProtClassRow.Visible = True

                OptionalBuildingCoveragesSpacerRow.Visible = True
                OptionalBuildingCoveragesMainRow.Visible = True
                OptionalBuildingCoveragesHeaderSpacerRow.Visible = True 'added 2/19/2013; BOP
                OptionalBuildingCoveragesQuotedPremiumRow.Visible = True
                'BuildAcctsRecOnPremisesRow.Visible = True
                'BuildAcctsRecOffPremisesRow.Visible = True
                'updated 2/14/2013 (styles and cleanup)
                If UCase(lblAcctsRecSelected.Text) = "NOT SELECTED" Then
                    BuildAcctsRecSelectedRow.Visible = True
                    BuildAcctsRecOnPremisesRow.Visible = False
                    BuildAcctsRecOffPremisesRow.Visible = False
                Else
                    BuildAcctsRecSelectedRow.Visible = False
                    BuildAcctsRecOnPremisesRow.Visible = True
                    BuildAcctsRecOffPremisesRow.Visible = True
                End If
                'BuildValuablePapersOnPremisesRow.Visible = True
                'BuildValuablePapersOffPremisesRow.Visible = True
                'updated 2/14/2013 (styles and cleanup)
                If UCase(lblValPapersSelected.Text) = "NOT SELECTED" Then
                    BuildValuablePapersSelectedRow.Visible = True
                    BuildValuablePapersOnPremisesRow.Visible = False
                    BuildValuablePapersOffPremisesRow.Visible = False
                Else
                    BuildValuablePapersSelectedRow.Visible = False
                    BuildValuablePapersOnPremisesRow.Visible = True
                    BuildValuablePapersOffPremisesRow.Visible = True
                End If
                'BuildCondoCommercialUnitLimitRow.Visible = True
                'updated 2/14/2013 (styles and cleanup)
                If UCase(lblCondoCommUnitOwnersSelected.Text) = "NOT SELECTED" Then
                    BuildCondoCommercialUnitLimitSelectedRow.Visible = True
                    BuildCondoCommercialUnitLimitRow.Visible = False
                Else
                    BuildCondoCommercialUnitLimitSelectedRow.Visible = False
                    BuildCondoCommercialUnitLimitRow.Visible = True
                End If
                BuildOrdOrLawMainRow.Visible = True
                If UCase(lblOrdOrLaw.Text) = "SELECTED" Then
                    BuildOrdOrLawUndamagedRow.Visible = True
                    BuildOrdOrLawDemoRow.Visible = True
                    BuildOrdOrLawIncreasedCostRow.Visible = True
                    BuildOrdOrLawDemoAndIncreasedCostRow.Visible = True
                End If
                BuildSpoilageMainRow.Visible = True
                If UCase(lblSpoilage.Text) = "SELECTED" Then
                    BuildSpoilagePropClassificationRow.Visible = True
                    BuildSpoilageLimitRow.Visible = True
                    BuildSpoilageRefrigRow.Visible = True
                    BuildSpoilageBreakdownRow.Visible = True
                    BuildSpoilagePowerOutageRow.Visible = True
                End If
                'ElseIf quickQuote IsNot Nothing AndAlso quickQuote.LobType <> Nothing AndAlso quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty Then 'added 10/16/2012 for CPR
                'updated 11/19/2012 for CPP
            ElseIf quickQuote IsNot Nothing AndAlso quickQuote.LobType <> Nothing AndAlso (quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty OrElse quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage) Then 'added 10/16/2012 for CPR
                BuildConstructionRow.Visible = True
                BuildProtClassRow.Visible = True 'may hide since it's entered at the Location level
                Build_CPR_TotalPremiumRow.Visible = True
                Build_CPR_Covs_Row.Visible = True 'CPR
            End If
        Next
    End Sub
    Private Sub SetupLocClassCodesRepeater(ByVal rptClassCodes As Repeater)
        For Each i As RepeaterItem In rptClassCodes.Items
            Dim ClassCodeRow As HtmlTableRow = i.FindControl("ClassCodeRow")
            Dim ClassCodeDescriptionRow As HtmlTableRow = i.FindControl("ClassCodeDescriptionRow")
            Dim ClassCodeEmpPayrollRow As HtmlTableRow = i.FindControl("ClassCodeEmpPayrollRow")
            Dim ClassCodeEmpNumRow As HtmlTableRow = i.FindControl("ClassCodeEmpNumRow")
            'added 8/20/2012
            Dim ClassCodePremiumRow As HtmlTableRow = i.FindControl("ClassCodePremiumRow")

            ClassCodeRow.Visible = False 'WC
            ClassCodeDescriptionRow.Visible = False 'WC
            ClassCodeEmpPayrollRow.Visible = False 'WC
            ClassCodeEmpNumRow.Visible = False 'WC
            ClassCodePremiumRow.Visible = False 'WC

            If quickQuote IsNot Nothing AndAlso quickQuote.LobType <> Nothing AndAlso quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.WorkersCompensation Then
                ClassCodeRow.Visible = True
                ClassCodeDescriptionRow.Visible = True
                ClassCodeEmpPayrollRow.Visible = True
                ClassCodeEmpNumRow.Visible = True
                ClassCodePremiumRow.Visible = True
            End If
        Next
    End Sub
    Private Sub SetupLocGlClassCodesRepeater(ByVal rpt_GL_ClassCodes As Repeater) 'added 8/27/2012 for GL
        For Each i As RepeaterItem In rpt_GL_ClassCodes.Items
            Dim ClassCodeRow As HtmlTableRow = i.FindControl("ClassCodeRow")
            Dim ClassCodeDescriptionRow As HtmlTableRow = i.FindControl("ClassCodeDescriptionRow")
            Dim ClassCodePremExpRow As HtmlTableRow = i.FindControl("ClassCodePremExpRow")
            Dim ClassCodePremBaseRow As HtmlTableRow = i.FindControl("ClassCodePremBaseRow")
            Dim ClassCodeProductsPremRow As HtmlTableRow = i.FindControl("ClassCodeProductsPremRow")
            Dim ClassCodePremisesPremRow As HtmlTableRow = i.FindControl("ClassCodePremisesPremRow")

            ClassCodeRow.Visible = False 'GL
            ClassCodeDescriptionRow.Visible = False 'GL
            ClassCodePremExpRow.Visible = False 'GL
            ClassCodePremBaseRow.Visible = False 'GL
            ClassCodeProductsPremRow.Visible = False 'GL
            ClassCodePremisesPremRow.Visible = False 'GL

            'If quickQuote IsNot Nothing AndAlso quickQuote.LobType <> Nothing AndAlso quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability Then
            'updated 11/19/2012 for CPP
            If quickQuote IsNot Nothing AndAlso quickQuote.LobType <> Nothing AndAlso (quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability OrElse quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage) Then
                ClassCodeRow.Visible = True
                ClassCodeDescriptionRow.Visible = True
                ClassCodePremExpRow.Visible = True
                ClassCodePremBaseRow.Visible = True
                ClassCodeProductsPremRow.Visible = True
                ClassCodePremisesPremRow.Visible = True
            End If
        Next
    End Sub

    Public Function BuildingHasLimit(buildingLimit As String) As Boolean
        If qqHelper.doUseNewBOPVersion(quickQuote.EffectiveDate) Then
            If Not String.IsNullOrWhiteSpace(buildingLimit) AndAlso IsNumeric(buildingLimit) Then
                Return True
            Else
                Return False
            End If
        Else
            Return True
        End If
    End Function

    Public Sub GetQuoteFromDb()
        'added 8/21/2012
        Dim autoFinalize As Boolean = False

        Dim valItemsMsg As String = "" 'added 4/11/2013 to use w/ Route To UW
        Dim val_item_counter As Integer = 0 'added 4/11/2013
        Dim businessDateAlreadyShown As Boolean = False

        If IsNumeric(Me.lblQuoteId.Text) = True Then
            Dim errorMsg As String = ""
            'QQxml.GetQuote(quoteId, QuickQuoteXML.QuickQuoteXmlType.RatedQuote, quickQuote, errorMsg)
            Dim rateType As QuickQuoteXML.QuickQuoteSaveType = Nothing
            QQxml.GetRatedQuote(Me.lblQuoteId.Text, quickQuote, rateType, errorMsg)
            If quickQuote IsNot Nothing Then
                Me.CopyQuoteSection.Visible = True
                If rateType = QuickQuoteXML.QuickQuoteSaveType.AppGap Then
                    Me.lblAppGapText.Text = "&nbsp;(Application)"
                    Me.lblAppGapText.Visible = True
                End If
                If quickQuote.PolicyBridgingURL <> "" AndAlso UCase(Me.lblIsPrinterFriendly.Text) <> "YES" Then 'added printer friendly functionality 8/29/2012
                    Me.lblGoToPortalLink.Text = quickQuote.PolicyBridgingURL
                    'Me.btnGoToPortal.Text = "Finish Quote in Diamond Portal"

                    'added 8/20/2012 to work w/ setting tab from controlloader page
                    If quickQuote.QuoteNumber <> "" AndAlso ConfigurationManager.AppSettings("DiamondPortalMainPageLink") IsNot Nothing AndAlso ConfigurationManager.AppSettings("DiamondPortalMainPageLink").ToString <> "" Then
                        Me.lblGoToPortalLink.Text = ConfigurationManager.AppSettings("DiamondPortalMainPageLink").ToString & "?QuickQuoteNumber=" & quickQuote.QuoteNumber
                    End If

                    'added 8/10/2012
                    If Me.lblGoToPortalLink.Text <> "" AndAlso ConfigurationManager.AppSettings("QuickQuote_ShowDiamondPortalButton") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_ShowDiamondPortalButton").ToString <> "" AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_ShowDiamondPortalButton").ToString) = "YES" Then
                        Me.DiamondPortalRow.Visible = True
                    End If
                End If
                If quickQuote.QuoteNumber <> "" Then
                    Me.lblErrorQuoteNumber.Text = " (" & quickQuote.QuoteNumber & ")"
                    Me.lblErrorQuoteNumber.Visible = True
                End If
                If quickQuote.LobType <> QuickQuoteObject.QuickQuoteLobType.None AndAlso UCase(Me.lblIsPrinterFriendly.Text) <> "YES" Then 'added printer friendly functionality 8/29/2012
                    ViewState.Add("QQ_Lob_Type", quickQuote.LobType)
                    Select Case quickQuote.LobType
                        Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                            If rateType = QuickQuoteXML.QuickQuoteSaveType.AppGap Then
                                Me.btnReturnToQuote.Text = "Return to Application and Edit"
                                Me.lblReturnToQuoteLink.Text = ConfigurationManager.AppSettings("QuickQuote_BOP_AppGap").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                            Else
                                Me.btnReturnToQuote.Text = "Return to Quote and Edit"
                                'Me.lblReturnToQuoteLink.Text = "BOP Pages/BOPQuoteInputPage.aspx?QuoteId=" & quoteId
                                Me.lblReturnToQuoteLink.Text = ConfigurationManager.AppSettings("QuickQuote_BOP_Input").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                                'Me.btnContinueToAppGap.Visible = True '*set to True if there's a link
                                Me.lblAppGapLink.Text = ConfigurationManager.AppSettings("QuickQuote_BOP_AppGap").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                            End If
                            'added 1/10/2013 to go back to Quote Edit after App Gap rating error
                            Me.btnReturnToQuote2.Text = "Go back to Quote and Edit"
                            Me.lblReturnToQuoteLink2.Text = ConfigurationManager.AppSettings("QuickQuote_BOP_Input").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                        Case QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
                            If rateType = QuickQuoteXML.QuickQuoteSaveType.AppGap Then
                                Me.btnReturnToQuote.Text = "Return to Application and Edit"
                                Me.lblReturnToQuoteLink.Text = ConfigurationManager.AppSettings("QuickQuote_CGL_AppGap").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                            Else
                                Me.btnReturnToQuote.Text = "Return to Quote and Edit"
                                Me.lblReturnToQuoteLink.Text = ConfigurationManager.AppSettings("QuickQuote_CGL_Input").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                                Me.lblAppGapLink.Text = ConfigurationManager.AppSettings("QuickQuote_CGL_AppGap").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                            End If
                            'added 1/10/2013 to go back to Quote Edit after App Gap rating error
                            Me.btnReturnToQuote2.Text = "Go back to Quote and Edit"
                            Me.lblReturnToQuoteLink2.Text = ConfigurationManager.AppSettings("QuickQuote_CGL_Input").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                        Case QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                            If rateType = QuickQuoteXML.QuickQuoteSaveType.AppGap Then
                                Me.btnReturnToQuote.Text = "Return to Application and Edit"
                                Me.lblReturnToQuoteLink.Text = ConfigurationManager.AppSettings("QuickQuote_WC_AppGap").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                            Else
                                Me.btnReturnToQuote.Text = "Return to Quote and Edit"
                                Me.lblReturnToQuoteLink.Text = ConfigurationManager.AppSettings("QuickQuote_WC_Input").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                                Me.lblAppGapLink.Text = ConfigurationManager.AppSettings("QuickQuote_WC_AppGap").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                            End If
                            'added 1/10/2013 to go back to Quote Edit after App Gap rating error
                            Me.btnReturnToQuote2.Text = "Go back to Quote and Edit"
                            Me.lblReturnToQuoteLink2.Text = ConfigurationManager.AppSettings("QuickQuote_WC_Input").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                        Case QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                            If rateType = QuickQuoteXML.QuickQuoteSaveType.AppGap Then
                                Me.btnReturnToQuote.Text = "Return to Application and Edit"
                                Me.lblReturnToQuoteLink.Text = ConfigurationManager.AppSettings("QuickQuote_CPR_AppGap").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                            Else
                                Me.btnReturnToQuote.Text = "Return to Quote and Edit"
                                Me.lblReturnToQuoteLink.Text = ConfigurationManager.AppSettings("QuickQuote_CPR_Input").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                                Me.lblAppGapLink.Text = ConfigurationManager.AppSettings("QuickQuote_CPR_AppGap").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                            End If
                            'added 1/10/2013 to go back to Quote Edit after App Gap rating error
                            Me.btnReturnToQuote2.Text = "Go back to Quote and Edit"
                            Me.lblReturnToQuoteLink2.Text = ConfigurationManager.AppSettings("QuickQuote_CPR_Input").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                        Case QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                            If rateType = QuickQuoteXML.QuickQuoteSaveType.AppGap Then
                                Me.btnReturnToQuote.Text = "Return to Application and Edit"
                                Me.lblReturnToQuoteLink.Text = ConfigurationManager.AppSettings("QuickQuote_CPP_AppGap").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                            Else
                                Me.btnReturnToQuote.Text = "Return to Quote and Edit"
                                Me.lblReturnToQuoteLink.Text = ConfigurationManager.AppSettings("QuickQuote_CPP_Input").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                                Me.lblAppGapLink.Text = ConfigurationManager.AppSettings("QuickQuote_CPP_AppGap").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                            End If
                            'added 1/10/2013 to go back to Quote Edit after App Gap rating error
                            Me.btnReturnToQuote2.Text = "Go back to Quote and Edit"
                            Me.lblReturnToQuoteLink2.Text = ConfigurationManager.AppSettings("QuickQuote_CPP_Input").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                        Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                            If rateType = QuickQuoteXML.QuickQuoteSaveType.AppGap Then
                                Me.btnReturnToQuote.Text = "Return to Application and Edit"
                                Me.lblReturnToQuoteLink.Text = ConfigurationManager.AppSettings("QuickQuote_CAP_AppGap").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                            Else
                                Me.btnReturnToQuote.Text = "Return to Quote and Edit"
                                Me.lblReturnToQuoteLink.Text = ConfigurationManager.AppSettings("QuickQuote_CAP_Input").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                                Me.lblAppGapLink.Text = ConfigurationManager.AppSettings("QuickQuote_CAP_AppGap").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                            End If
                            'added 1/10/2013 to go back to Quote Edit after App Gap rating error
                            Me.btnReturnToQuote2.Text = "Go back to Quote and Edit"
                            Me.lblReturnToQuoteLink2.Text = ConfigurationManager.AppSettings("QuickQuote_CAP_Input").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                    End Select
                End If

                Dim startDate As DateTime = Nothing


                Dim limitsExceedAllowed As Boolean = TotalLimitsExceedValue(quickQuote, 3000000) ' Matt A 6-3-15

                If quickQuote.Success = True Then
                    Me.lblPolicyId.Text = quickQuote.PolicyId
                    Me.lblPolicyImageNum.Text = quickQuote.PolicyImageNum
                    Me.lblClientId.Text = quickQuote.Client.ClientId 'added 9/5/2012 for future use w/ 'Start New Quote for this Client'
                    'added more 9/6/2012
                    If quickQuote.AgencyId <> "" AndAlso quickQuote.AgencyCode <> "" AndAlso quickQuote.Client IsNot Nothing AndAlso quickQuote.Client.Name.HasData = True Then
                        Me.lblDiaAgencyId.Text = quickQuote.AgencyId
                        Me.lblDiaAgencyCode.Text = quickQuote.AgencyCode
                        ViewState.Add("QQ_Client", quickQuote.Client)
                        Me.NewQuoteForClientSection.Visible = True
                    End If
                    Me.QuoteSummarySection.Visible = True
                    LoadValuesFromQuickQuote()
                    If UCase(Me.lblIsPrinterFriendly.Text) <> "YES" Then 'added printer friendly functionality 8/29/2012
                        If ConfigurationManager.AppSettings("QuickQuotePrintHistory_Worksheet_or_All") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuotePrintHistory_Worksheet_or_All").ToString <> "" AndAlso UCase(ConfigurationManager.AppSettings("QuickQuotePrintHistory_Worksheet_or_All").ToString) = "WORKSHEET" Then
                            Me.btnGoToPrintHistory.Text = "View Worksheet" '"View Diamond Worksheet"'changed 9/7/2012
                            GetDiamondPrintHistory(PrintType.JustWorksheet)
                        Else
                            Me.btnGoToPrintHistory.Text = "View Print" '"View Diamond Print"'changed 9/7/2012
                            GetDiamondPrintHistory()
                        End If
                    End If
                    If quickQuote.ValidationItems IsNot Nothing AndAlso quickQuote.ValidationItems.Count > 0 Then
                        Me.QuoteMessagesSpacerRow.Visible = True
                        Me.QuoteMessagesHeaderRow.Visible = True
                        Me.QuoteMessagesValueRow.Visible = True
                        For Each vi As QuickQuoteValidationItem In quickQuote.ValidationItems
                            Me.lblQuoteMessages.Text = qqHelper.appendText(Me.lblQuoteMessages.Text, vi.Message, "<br />")

                            'added 4/11/2013
                            val_item_counter += 1
                            If valItemsMsg = "" Then
                                valItemsMsg = vi.Message
                            Else
                                valItemsMsg &= "; " & vi.Message
                            End If
                        Next
                    End If
                    If limitsExceedAllowed AndAlso IsStaffUser() = False Then ' Matt A 6-3-15
                        'add validation message
                        Me.lblQuoteMessages.Text = qqHelper.appendText(Me.lblQuoteMessages.Text, "Limits of insurance exceed 3 million; please contact your underwriter to continue.", "<br />")
                        Me.btnApplication.Enabled = False
                        Me.btnApplication.ToolTip = "Limits of insurance exceed 3 million; please contact your underwriter to continue."
                    End If
                    If UCase(Me.lblIsPrinterFriendly.Text) <> "YES" Then 'added printer friendly functionality 8/29/2012
                        If Me.PrinterFriendlyLink.HRef <> "" Then
                            Me.PrinterFriendlyRow.Visible = True
                        End If

                        'added 8/8/2012
                        If rateType = QuickQuoteXML.QuickQuoteSaveType.Quote Then
                            Me.QuoteSuccessButtonsRow.Visible = True
                            If Me.lblAppGapLink.Text <> "" Then
                                Me.btnContinueToAppGap.Visible = True
                            End If
                            'added 10/8/2012 for CAP (IRPM validation)
                            If quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto AndAlso (quickQuote.Vehicles Is Nothing OrElse quickQuote.Vehicles.Count < 2) Then
                                Me.btnCreditDebits.Enabled = False
                                Me.btnCreditDebits.ToolTip = "You cannot edit CAP IRPM factors unless there are at least 2 vehicles."
                            Else
                                Me.btnCreditDebits.Enabled = True
                                Me.btnCreditDebits.ToolTip = ""
                            End If
                        ElseIf rateType = QuickQuoteXML.QuickQuoteSaveType.AppGap Then


                            Me.AppGapSuccessButtonsRow.Visible = True

                            'added 8/21/2012
                            If ConfigurationManager.AppSettings("QuickQuote_AutoFinalizeFromAppGapSummary") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_AutoFinalizeFromAppGapSummary").ToString) = "YES" Then
                                autoFinalize = True
                            End If

                        End If
                    End If
                ElseIf quickQuote.ValidationItems IsNot Nothing AndAlso quickQuote.ValidationItems.Count > 0 Then
                    Me.QuoteErrorSection.Visible = True
                    For Each vi As QuickQuoteValidationItem In quickQuote.ValidationItems
                        Me.lblQuoteErrors.Text = qqHelper.appendText(Me.lblQuoteErrors.Text, vi.Message, "<br />")

                        'added 4/11/2013
                        val_item_counter += 1
                        If valItemsMsg = "" Then
                            valItemsMsg = vi.Message
                        Else
                            valItemsMsg &= "; " & vi.Message
                        End If
                    Next
                    'added 8/21/2012
                    If Me.lblQuoteErrors.Text = "" Then
                        Me.lblQuoteErrors.Text = "The quote failed, but no validation items were found."
                    End If
                Else
                    Me.QuoteErrorSection.Visible = True
                    Me.lblQuoteErrors.Text = "The quote failed, but no validation items were found."
                End If

                'added 1/10/2013 to go back to Quote Edit after App Gap rating error; can also use for App Gap success if we aren't auto-finalizing
                'If quickQuote.Success = False AndAlso rateType = QuickQuoteXML.QuickQuoteSaveType.AppGap Then
                If rateType = QuickQuoteXML.QuickQuoteSaveType.AppGap AndAlso ConfigurationManager.AppSettings("QuickQuote_ShowQuoteEditButtonOnAppGapSummary") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_ShowQuoteEditButtonOnAppGapSummary").ToString) = "YES" Then
                    Me.SecondaryReturnToQuoteSection.Visible = True
                    'added 1/11/2013 to allow user to Route on rating error (when applicable - quote is in Diamond); for AppGap Fail
                    If quickQuote.Success = False AndAlso Me.lblErrorQuoteNumber.Text <> "" Then
                        Me.SecondaryReturnToQuoteSection_RouteToUW.Visible = True
                    End If
                End If
                'added 1/11/2013 to allow user to Route on rating error (when applicable - quote is in Diamond); for Quote Fail
                If quickQuote.Success = False AndAlso Me.lblErrorQuoteNumber.Text <> "" AndAlso Me.SecondaryReturnToQuoteSection_RouteToUW.Visible = False Then 'also make sure another 'Route to UW' button isn't already visible
                    Me.SecondaryRouteToUnderwritingSection.Visible = True
                End If

                'added 1/14/2013 as secondary measure to see if policy id, policy image number, and agency id are available (for Route To Underwriting; since these were previously only stored on Success)
                If Me.lblPolicyId.Text = "" AndAlso quickQuote.PolicyId <> "" Then
                    Me.lblPolicyId.Text = quickQuote.PolicyId
                End If
                If Me.lblPolicyImageNum.Text = "" AndAlso quickQuote.PolicyImageNum <> "" Then
                    Me.lblPolicyImageNum.Text = quickQuote.PolicyImageNum
                End If
                If Me.lblDiaAgencyId.Text = "" AndAlso quickQuote.AgencyId <> "" Then
                    Me.lblDiaAgencyId.Text = quickQuote.AgencyId
                End If
                If Me.lblDiaAgencyCode.Text = "" AndAlso quickQuote.AgencyCode <> "" Then
                    Me.lblDiaAgencyCode.Text = quickQuote.AgencyCode
                End If
                If Me.lblClientId.Text = "" AndAlso quickQuote.Client IsNot Nothing AndAlso quickQuote.Client.ClientId <> "" Then
                    Me.lblClientId.Text = quickQuote.Client.ClientId
                End If
                'may also add client to ViewState if not already there (for 'New quote for Client' functionality)

                'quickQuote.Dispose() 'removed 9/6/2012 because client stored in viewstate was losing value
            Else
                If errorMsg <> "" Then
                    Me.QuoteErrorSection.Visible = True
                    Me.lblQuoteErrors.Text = errorMsg
                Else
                    Me.QuoteErrorSection.Visible = True
                    Me.lblQuoteErrors.Text = "There was a problem locating that quoteId in the database."
                End If
            End If
        Else
            Me.QuoteErrorSection.Visible = True
            Me.lblQuoteErrors.Text = "A valid parameter for QuoteId was not sent thru the querystring."
        End If

        'added 4/11/2013
        If valItemsMsg <> "" Then
            valItemsMsg = "Validation Item" & If(val_item_counter = 1, "", "s") & ":  " & valItemsMsg
            If ViewState.Item("valItemsMsg") Is Nothing Then
                ViewState.Add("valItemsMsg", valItemsMsg)
            Else
                ViewState.Item("valItemsMsg") = valItemsMsg
            End If
        End If

        'added 8/21/2012
        If autoFinalize = True Then
            btnFinalizeInDiamond_Click(Nothing, Nothing)
        End If
    End Sub
    'added 2/28/2013 for new buttons
    Public Sub GetQuoteFromDb_NewButtons()
        'added 8/21/2012
        Dim autoFinalize As Boolean = False

        Dim valItemsMsg As String = "" 'added 4/11/2013 to use w/ Route To UW
        Dim val_item_counter As Integer = 0 'added 4/11/2013
        Dim businessDateAlreadyShown As Boolean = False

        If IsNumeric(Me.lblQuoteId.Text) = True Then
            Dim errorMsg As String = ""
            'QQxml.GetQuote(quoteId, QuickQuoteXML.QuickQuoteXmlType.RatedQuote, quickQuote, errorMsg)
            Dim rateType As QuickQuoteXML.QuickQuoteSaveType = Nothing
            QQxml.GetRatedQuote(Me.lblQuoteId.Text, quickQuote, rateType, errorMsg)

            If quickQuote IsNot Nothing Then
                '2/28/2013 - replaced old button logic
                'Me.OptionalNavSection.Visible = True 'inside NavButtonsRow; App and Route (Quote and Exit are always there); now 2 spans (OptionalNavSection1 and OptionalNavSection2)
                Me.OptionsButtonsRow.Visible = True 'Print, IRPM, Copy, and New
                Me.btnPrint.Enabled = False
                Me.btnPrint.ToolTip = "Diamond Print is not available for this quote"
                Me.btnIRPM.Enabled = False
                Me.btnIRPM.ToolTip = "IRPM edits cannot be done for this quote"
                Me.btnNew.Enabled = False
                Me.btnNew.ToolTip = "You cannot start a new quote for this client"
                Me.btnApplication.Enabled = False
                Me.btnApplication.ToolTip = "You cannot edit the Application for this quote"
                Me.btnRoute.Enabled = False
                Me.btnRoute.ToolTip = "You cannot route this quote"

                If quickQuote.PolicyId <> "" Then
                    Me.lblPolicyId.Text = quickQuote.PolicyId
                End If
                If quickQuote.PolicyImageNum <> "" Then
                    Me.lblPolicyImageNum.Text = quickQuote.PolicyImageNum
                End If
                If quickQuote.AgencyId <> "" Then
                    Me.lblDiaAgencyId.Text = quickQuote.AgencyId
                End If
                If quickQuote.AgencyCode <> "" Then
                    Me.lblDiaAgencyCode.Text = quickQuote.AgencyCode
                End If
                If quickQuote.Client IsNot Nothing AndAlso quickQuote.Client.ClientId <> "" Then
                    Me.lblClientId.Text = quickQuote.Client.ClientId
                End If
                If quickQuote.Client IsNot Nothing AndAlso quickQuote.Client.Name.HasData = True Then
                    If ViewState.Item("QQ_Client") Is Nothing Then
                        ViewState.Add("QQ_Client", quickQuote.Client)
                    Else
                        ViewState.Item("QQ_Client") = quickQuote.Client
                    End If
                End If

                'new quote for client
                'If Me.lblClientId.Text <> "" AndAlso ViewState.Item("QQ_Client") IsNot Nothing Then
                '3/5/2013 - updated IF for agency id and agency code
                If Me.lblClientId.Text <> "" AndAlso ViewState.Item("QQ_Client") IsNot Nothing AndAlso Me.lblDiaAgencyId.Text <> "" AndAlso Me.lblDiaAgencyCode.Text <> "" Then
                    Me.btnNew.Enabled = True
                    Me.btnNew.ToolTip = ""
                End If

                If quickQuote.PolicyBridgingURL <> "" AndAlso UCase(Me.lblIsPrinterFriendly.Text) <> "YES" Then 'added printer friendly functionality 8/29/2012
                    Me.lblPortalLink.Text = quickQuote.PolicyBridgingURL

                    'added 8/20/2012 to work w/ setting tab from controlloader page
                    If quickQuote.QuoteNumber <> "" AndAlso ConfigurationManager.AppSettings("DiamondPortalMainPageLink") IsNot Nothing AndAlso ConfigurationManager.AppSettings("DiamondPortalMainPageLink").ToString <> "" Then
                        Me.lblPortalLink.Text = ConfigurationManager.AppSettings("DiamondPortalMainPageLink").ToString & "?QuickQuoteNumber=" & quickQuote.QuoteNumber
                    End If

                    If Me.lblPortalLink.Text <> "" AndAlso ConfigurationManager.AppSettings("QuickQuote_ShowDiamondPortalButton") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_ShowDiamondPortalButton").ToString <> "" AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_ShowDiamondPortalButton").ToString) = "YES" Then
                        Me.ExtraButtonsRow.Visible = True
                        Me.btnPortal.Visible = True
                    End If
                End If
                If quickQuote.QuoteNumber <> "" OrElse rateType = QuickQuoteXML.QuickQuoteSaveType.AppGap Then
                    'updated 3/4/2013 to also show other buttons for app gap (since you need the Edit AppGap button to correct errors even if quoteNumber is missing)
                    'Me.OptionalNavSection.Visible = True 'inside NavButtonsRow; App and Route (Quote and Exit are always there regardless); now 2 spans
                    Me.OptionalNavSection1.Visible = True 'App
                    Me.OptionalNavSection2.Visible = True 'Route
                    Me.OptionalOptionsSection.Visible = True 'inside OptionsButtonsRow; Print, IRPM (Copy and New are always there when OptionsButtonsRow is visible)

                    If quickQuote.QuoteNumber <> "" Then
                        Me.lblErrorQuoteNumber.Text = " (" & quickQuote.QuoteNumber & ")"
                        Me.lblErrorQuoteNumber.Visible = True

                        '3/5/2013 - wrapped Route ability in IF for policy id, image num, and agency id
                        If Me.lblPolicyId.Text <> "" AndAlso Me.lblPolicyImageNum.Text <> "" AndAlso Me.lblDiaAgencyId.Text <> "" Then
                            Me.btnRoute.Enabled = True
                            Me.btnRoute.ToolTip = ""
                        End If
                    End If
                End If

                If quickQuote.LobType <> QuickQuoteObject.QuickQuoteLobType.None AndAlso UCase(Me.lblIsPrinterFriendly.Text) <> "YES" Then 'added printer friendly functionality 8/29/2012
                    If ViewState.Item("QQ_Lob_Type") Is Nothing Then
                        ViewState.Add("QQ_Lob_Type", quickQuote.LobType)
                    Else
                        ViewState.Item("QQ_Lob_Type") = quickQuote.LobType
                    End If
                    '2/28/2013 - replaced old button logic
                    Select Case quickQuote.LobType
                        Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                            Me.lblQuoteLink.Text = ConfigurationManager.AppSettings("QuickQuote_BOP_Input").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                            Me.lblApplicationLink.Text = ConfigurationManager.AppSettings("QuickQuote_BOP_AppGap").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                        Case QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
                            Me.lblQuoteLink.Text = ConfigurationManager.AppSettings("QuickQuote_CGL_Input").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                            Me.lblApplicationLink.Text = ConfigurationManager.AppSettings("QuickQuote_CGL_AppGap").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                        Case QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                            Me.lblQuoteLink.Text = ConfigurationManager.AppSettings("QuickQuote_WC_Input").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                            Me.lblApplicationLink.Text = ConfigurationManager.AppSettings("QuickQuote_WC_AppGap").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                        Case QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                            Me.lblQuoteLink.Text = ConfigurationManager.AppSettings("QuickQuote_CPR_Input").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                            Me.lblApplicationLink.Text = ConfigurationManager.AppSettings("QuickQuote_CPR_AppGap").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                        Case QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                            Me.lblQuoteLink.Text = ConfigurationManager.AppSettings("QuickQuote_CPP_Input").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                            Me.lblApplicationLink.Text = ConfigurationManager.AppSettings("QuickQuote_CPP_AppGap").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                        Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                            Me.lblQuoteLink.Text = ConfigurationManager.AppSettings("QuickQuote_CAP_Input").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                            Me.lblApplicationLink.Text = ConfigurationManager.AppSettings("QuickQuote_CAP_AppGap").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                        Case Else
                            Me.btnQuote.Enabled = False
                            Me.btnQuote.ToolTip = "No link available for this LOB"
                            Me.btnApplication.Enabled = False
                            Me.btnApplication.ToolTip = "No link available for this LOB"
                    End Select
                ElseIf quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.None Then
                    Me.btnQuote.Enabled = False
                    Me.btnQuote.ToolTip = "No link available for this LOB"
                    Me.btnApplication.Enabled = False
                    Me.btnApplication.ToolTip = "No link available for this LOB"
                End If

                If rateType = QuickQuoteXML.QuickQuoteSaveType.AppGap Then
                    Me.lblAppGapText.Text = "&nbsp;(Application)"
                    Me.lblAppGapText.Visible = True

                    Me.btnQuote.Text = "Go back to Quote and Edit"
                    Me.btnApplication.Text = "Return to Application and Edit"

                    Me.btnApplication.Enabled = True
                    Me.btnApplication.ToolTip = ""

                    If quickQuote.Success = True Then
                        If ViewState.Item("QQ_Route_Type") Is Nothing Then
                            ViewState.Add("QQ_Route_Type", RouteToUnderwritingType.AppGapSuccess)
                        Else
                            ViewState.Item("QQ_Route_Type") = RouteToUnderwritingType.AppGapSuccess
                        End If

                        Me.btnIRPM.Enabled = False
                        Me.btnIRPM.ToolTip = "You can only edit IRPM factors from the Quote workflow."

                        Me.ExtraButtonsRow.Visible = True
                        Me.btnFinalize.Visible = True


                        Dim startDate As DateTime = Nothing
                        Me.AppGapSuccessButtonsRow.Visible = True

                        'added 8/21/2012
                        If ConfigurationManager.AppSettings("QuickQuote_AutoFinalizeFromAppGapSummary") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_AutoFinalizeFromAppGapSummary").ToString) = "YES" Then
                            autoFinalize = True
                        End If


                    Else
                        If ViewState.Item("QQ_Route_Type") Is Nothing Then
                            ViewState.Add("QQ_Route_Type", RouteToUnderwritingType.AppGapFail)
                        Else
                            ViewState.Item("QQ_Route_Type") = RouteToUnderwritingType.AppGapFail
                        End If
                    End If
                Else

                    Me.btnQuote.Text = "Return to Quote and Edit"
                    Me.btnApplication.Text = "Continue to Application"
                    If quickQuote.Success = True Then
                        Me.btnApplication.Enabled = True
                        Me.btnApplication.ToolTip = ""

                        If ViewState.Item("QQ_Route_Type") Is Nothing Then
                            ViewState.Add("QQ_Route_Type", RouteToUnderwritingType.QuoteSuccess)
                        Else
                            ViewState.Item("QQ_Route_Type") = RouteToUnderwritingType.QuoteSuccess
                        End If

                        If quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto AndAlso (quickQuote.Vehicles Is Nothing OrElse quickQuote.Vehicles.Count < 2) Then
                            Me.btnIRPM.Enabled = False
                            Me.btnIRPM.ToolTip = "You cannot edit CAP IRPM factors unless there are at least 2 vehicles."
                        Else
                            Me.btnIRPM.Enabled = True
                            Me.btnIRPM.ToolTip = ""
                        End If

                        '5/8/2013 - added for proposal
                        If ConfigurationManager.AppSettings("QuickQuote_QuoteProposalSelection") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_QuoteProposalSelection").ToString <> "" Then
                            If ConfigurationManager.AppSettings("QuickQuote_ShowProposalSelectionButton") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_ShowProposalSelectionButton").ToString) = "YES" Then
                                Me.ExtraButtonsRow.Visible = True
                                Me.btnProposal.Visible = True
                            End If
                            If ConfigurationManager.AppSettings("QuickQuote_ShowProposalSelectionLink") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_ShowProposalSelectionLink").ToString) = "YES" Then
                                Me.lblProposalLinkText.Visible = True
                                Dim proposalLink As String = ConfigurationManager.AppSettings("QuickQuote_QuoteProposalSelection").ToString & "?QuoteId=" & Me.lblQuoteId.Text
                                Me.lblProposalLinkText.Text = "<br /><a href=""" & proposalLink & """"
                                If ConfigurationManager.AppSettings("QuickQuote_OpenProposalSelectionInNewWindow") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_OpenProposalSelectionInNewWindow").ToString) = "YES" Then
                                    Me.lblProposalLinkText.Text &= " target=""_blank"""
                                End If
                                Me.lblProposalLinkText.Text &= ">Prepare Proposal</a>" '5/29/2013 - changed text from Print Proposal (also changed on button)
                            End If
                        End If
                    Else
                        'redundant since they are already disabled above; only needed if different message is desired
                        'Me.btnApplication.Enabled = False
                        'Me.btnApplication.ToolTip = "You cannot continue to the Application until you get a successful quote rate"
                        'Me.btnIRPM.Enabled = False
                        'Me.btnIRPM.ToolTip = "You cannot edit IRPM factors until you get a successful quote rate."

                        If ViewState.Item("QQ_Route_Type") Is Nothing Then
                            ViewState.Add("QQ_Route_Type", RouteToUnderwritingType.QuoteFail)
                        Else
                            ViewState.Item("QQ_Route_Type") = RouteToUnderwritingType.QuoteFail
                        End If
                    End If
                End If

                If quickQuote.Success = True Then

                    Dim startDate As DateTime = Nothing

                    Dim limitsExceedAllowed As Boolean = TotalLimitsExceedValue(quickQuote, 3000000) ' Matt A 6-3-15
                    Me.QuoteSummarySection.Visible = True
                    LoadValuesFromQuickQuote()
                    If UCase(Me.lblIsPrinterFriendly.Text) <> "YES" Then 'added printer friendly functionality 8/29/2012
                        '3/4/2013 - updated for new buttons
                        Me.btnPrint.Enabled = True
                        Me.btnPrint.ToolTip = ""
                        If ConfigurationManager.AppSettings("QuickQuotePrintHistory_Worksheet_or_All") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuotePrintHistory_Worksheet_or_All").ToString <> "" AndAlso UCase(ConfigurationManager.AppSettings("QuickQuotePrintHistory_Worksheet_or_All").ToString) = "WORKSHEET" Then
                            Me.btnPrint.Text = "View Worksheet"
                            GetDiamondPrintHistory(PrintType.JustWorksheet)
                        Else
                            Me.btnPrint.Text = "View Print"
                            GetDiamondPrintHistory()
                        End If

                        If Me.PrinterFriendlyLink.HRef <> "" Then
                            Me.PrinterFriendlyRow.Visible = True
                        End If

                    End If
                    If quickQuote.ValidationItems IsNot Nothing AndAlso quickQuote.ValidationItems.Count > 0 Then
                        Me.QuoteMessagesSpacerRow.Visible = True
                        Me.QuoteMessagesHeaderRow.Visible = True
                        Me.QuoteMessagesValueRow.Visible = True
                        For Each vi As QuickQuoteValidationItem In quickQuote.ValidationItems
                            Me.lblQuoteMessages.Text = qqHelper.appendText(Me.lblQuoteMessages.Text, vi.Message, "<br />")

                            'added 4/11/2013
                            val_item_counter += 1
                            If valItemsMsg = "" Then
                                valItemsMsg = vi.Message
                            Else
                                valItemsMsg &= "; " & vi.Message
                            End If
                        Next
                    End If

                    If limitsExceedAllowed AndAlso IsStaffUser() = False Then ' Matt A 6-3-15
                        'add validation message
                        Me.lblQuoteMessages.Text = qqHelper.appendText(Me.lblQuoteMessages.Text, "Limits of insurance exceed 3 million; please contact your underwriter to continue.", "<br />")
                        Me.btnApplication.Enabled = False
                        Me.btnApplication.ToolTip = "Limits of insurance exceed 3 million; please contact your underwriter to continue."
                    End If

                ElseIf quickQuote.ValidationItems IsNot Nothing AndAlso quickQuote.ValidationItems.Count > 0 Then
                    Me.QuoteErrorSection.Visible = True
                    For Each vi As QuickQuoteValidationItem In quickQuote.ValidationItems
                        Me.lblQuoteErrors.Text = qqHelper.appendText(Me.lblQuoteErrors.Text, vi.Message, "<br />")

                        'added 4/11/2013
                        val_item_counter += 1
                        If valItemsMsg = "" Then
                            valItemsMsg = vi.Message
                        Else
                            valItemsMsg &= "; " & vi.Message
                        End If
                    Next
                    'added 8/21/2012
                    If Me.lblQuoteErrors.Text = "" Then
                        Me.lblQuoteErrors.Text = "The quote failed, but no validation items were found."
                    End If
                Else
                    Me.QuoteErrorSection.Visible = True
                    Me.lblQuoteErrors.Text = "The quote failed, but no validation items were found."
                End If

                'quickQuote.Dispose() 'removed 9/6/2012 because client stored in viewstate was losing value

                '3/4/2013 - added to still show Quote Number if we previously had it w/o affecting other logic dependent on QuoteNumber (like ability to Route to UW)
                If quickQuote.QuoteNumber = "" AndAlso Me.lblErrorQuoteNumber.Text = "" AndAlso Me.quickQuote.Database_LastAvailableQuoteNumber <> "" Then
                    Me.lblErrorQuoteNumber.Text = " (previous number:  " & quickQuote.Database_LastAvailableQuoteNumber & ")"
                    Me.lblErrorQuoteNumber.Visible = True
                End If
            Else
                If errorMsg <> "" Then
                    Me.QuoteErrorSection.Visible = True
                    Me.lblQuoteErrors.Text = errorMsg
                Else
                    Me.QuoteErrorSection.Visible = True
                    Me.lblQuoteErrors.Text = "There was a problem locating that quoteId in the database."
                End If
            End If
        Else
            Me.QuoteErrorSection.Visible = True
            Me.lblQuoteErrors.Text = "A valid parameter for QuoteId was not sent thru the querystring."
        End If

        'added 4/11/2013
        If valItemsMsg <> "" Then
            valItemsMsg = "Validation Item" & If(val_item_counter = 1, "", "s") & ":  " & valItemsMsg
            If ViewState.Item("valItemsMsg") Is Nothing Then
                ViewState.Add("valItemsMsg", valItemsMsg)
            Else
                ViewState.Item("valItemsMsg") = valItemsMsg
            End If
        End If

        'added 8/21/2012
        If autoFinalize = True Then
            'btnFinalizeInDiamond_Click(Nothing, Nothing)
            '3/5/2013 - replaced w/ new button_click
            btnFinalize_Click(Nothing, Nothing)
        End If
    End Sub

    Protected Sub btnReturnToQuote_Click(sender As Object, e As System.EventArgs) Handles btnReturnToQuote.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        If Me.lblReturnToQuoteLink.Text <> "" Then
            Response.Redirect(Me.lblReturnToQuoteLink.Text)
        End If

    End Sub
    '2/28/2013 - replaced old button logic
    Protected Sub btnQuote_Click(sender As Object, e As System.EventArgs) Handles btnQuote.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        If Me.lblQuoteLink.Text <> "" Then
            Response.Redirect(Me.lblQuoteLink.Text)
        End If

    End Sub

    Protected Sub btnContinueToAppGap_Click(sender As Object, e As System.EventArgs) Handles btnContinueToAppGap.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        If Me.lblAppGapLink.Text <> "" Then
            Response.Redirect(Me.lblAppGapLink.Text)
        End If
    End Sub
    '2/28/2013 - replaced old button logic
    Protected Sub btnApplication_Click(sender As Object, e As System.EventArgs) Handles btnApplication.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        If Me.lblApplicationLink.Text <> "" Then
            Response.Redirect(Me.lblApplicationLink.Text)
        End If
    End Sub

    Private Sub GetDiamondPrintHistory(Optional ByVal pType As PrintType = PrintType.All)
        If Me.lblPolicyId.Text <> "" AndAlso IsNumeric(Me.lblPolicyId.Text) = True Then
            Using dia As New DiamondWebClass.DiamondPrinting
                Dim forms As Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Printing.PrintForm)
                'forms = dia.getPrintFormsForPolicyId(dia.loginDiamond("PrintServices", "PrintServices"), 245276) 'policy id for QBOP010535 test on prodpatch
                'forms = dia.getPrintFormsForPolicyId(dia.loginDiamond("PrintServices", "PrintServices"), CInt(Me.lblPolicyId.Text))
                'updated 3/12/2013 to not send new credentials to Diamond
                Dim loginName As String = ""
                Dim loginPassword As String = ""
                If System.Web.HttpContext.Current.Session("DiamondUsername") IsNot Nothing AndAlso System.Web.HttpContext.Current.Session("DiamondUsername").ToString <> "" Then
                    loginName = System.Web.HttpContext.Current.Session("DiamondUsername").ToString
                ElseIf ConfigurationManager.AppSettings("TestOrProd") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("TestOrProd").ToString) = "TEST" AndAlso ConfigurationManager.AppSettings("QuickQuote_UseTestVariables") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_UseTestVariables").ToString) = "YES" Then
                    loginName = ConfigurationManager.AppSettings("QuickQuoteTestUsername").ToString
                End If
                If System.Web.HttpContext.Current.Session("DiamondPassword") IsNot Nothing AndAlso System.Web.HttpContext.Current.Session("DiamondPassword").ToString <> "" Then
                    loginPassword = System.Web.HttpContext.Current.Session("DiamondPassword").ToString
                ElseIf ConfigurationManager.AppSettings("TestOrProd") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("TestOrProd").ToString) = "TEST" AndAlso ConfigurationManager.AppSettings("QuickQuote_UseTestVariables") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_UseTestVariables").ToString) = "YES" Then
                    loginPassword = ConfigurationManager.AppSettings("QuickQuoteTestPassword").ToString
                End If
                If loginName <> "" AndAlso loginPassword <> "" Then
                    '5/7/2013 - doesn't need Try/Catch to prevent unhandled exception (since method in DiamondPrinting class already has it)
                    forms = dia.getPrintFormsForPolicyId(dia.loginDiamond(loginName, loginPassword), CInt(Me.lblPolicyId.Text))
                End If

                If forms IsNot Nothing Then
                    If pType = PrintType.JustWorksheet Then
                        Dim pfs As Generic.List(Of Diamond.Common.Objects.Printing.PrintForm) 'added 11/27/2012 for CPP (multiple worksheets; CPR and CGL)
                        For Each pf As Diamond.Common.Objects.Printing.PrintForm In forms
                            If UCase(pf.Description).Contains("WORKSHEET") Then
                                'Session("DiamondPrintFormBytes_" & Me.lblPolicyId.Text) = GetPrintForm(pf)
                                'Exit For
                                'updated 11/27/2012 for CPP
                                If pfs Is Nothing Then
                                    pfs = New Generic.List(Of Diamond.Common.Objects.Printing.PrintForm)
                                End If
                                pfs.Add(pf)
                            End If
                        Next
                        'added 11/27/2012 for CPP
                        If pfs IsNot Nothing Then
                            Session("DiamondPrintFormBytes_" & Me.lblPolicyId.Text) = GetPrintForm(pfs)
                        End If
                    Else
                        'all
                        Dim pfs As Generic.List(Of Diamond.Common.Objects.Printing.PrintForm)
                        For Each pf As Diamond.Common.Objects.Printing.PrintForm In forms
                            If pfs Is Nothing Then
                                pfs = New Generic.List(Of Diamond.Common.Objects.Printing.PrintForm)
                            End If
                            pfs.Add(pf)
                        Next
                        If pfs IsNot Nothing Then
                            Session("DiamondPrintFormBytes_" & Me.lblPolicyId.Text) = GetPrintForm(pfs)
                        End If
                    End If
                    'For Each pf As Diamond.Common.Objects.Printing.PrintForm In forms
                    '    ''If UCase(pf.Description).Contains("DECLA") = True Then
                    '    'Me.lblLogger.Text &= "<br><br>"
                    '    'Me.lblLogger.Text &= "TypeId=" & pf.FormCategoryTypeId
                    '    'Me.lblLogger.Text &= "<br>Type=" & pf.FormCategoryTypeDescription
                    '    'Me.lblLogger.Text &= "<br>Desc=" & pf.Description
                    '    'Me.lblLogger.Text &= "<br>PrintXMLid=" & pf.PrintXmlId
                    '    'Me.lblLogger.Text &= "<br>Form #=" & pf.FormNumber
                    '    'Me.lblLogger.Text &= "<br>Added=" & pf.AddedDate.ToString
                    '    ''End If

                    '    'to just get Worksheet
                    '    'If UCase(pf.Description).Contains("WORKSHEET") Then
                    '    '    Session("DiamondPrintFormBytes") = GetPrintForm(pf)
                    '    '    Exit For
                    '    'End If


                    'Next
                End If
            End Using
            If Session("DiamondPrintFormBytes_" & Me.lblPolicyId.Text) IsNot Nothing Then
                'Response.Redirect("DiamondPrintForm.aspx")
                'updated 2/28/2013 for new buttons
                If ConfigurationManager.AppSettings("QuickQuote_UseNewButtonLayout") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_UseNewButtonLayout").ToString) = "YES" Then
                    Me.btnPrint.Enabled = True
                    Me.btnPrint.ToolTip = ""
                Else
                    Me.PrintHistoryRow.Visible = True
                End If

                'Me.lblByteString.Text = qqHelper.StringFromBytes(CType(Session("DiamondPrintFormBytes"), Byte()))
                'Session("DiamondPrintFormBytes") = Nothing

                'using the querystring overloads max url length; may try POSTing to print page

            End If
        End If
    End Sub
    Private Function GetPrintForm(ByVal pf As Diamond.Common.Objects.Printing.PrintForm) As Byte()
        Dim reprintRequest As New Diamond.Common.Services.Messages.PrintingService.ReprintJob.Request
        Dim reprintResponse As New Diamond.Common.Services.Messages.PrintingService.ReprintJob.Response

        With reprintRequest.RequestData
            '.PolicyId = policyID
            '.PolicyImageNum = imageNum

            .PolicyId = pf.PolicyId
            .PolicyImageNum = pf.PolicyImageNum

            .PrintForms = New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Printing.PrintForm)
            .PrintForms.Add(New Diamond.Common.Objects.Printing.PrintForm() {pf})
        End With

        Using reprintProxy As New Diamond.Common.Services.Proxies.PrintingServiceProxy
            Try
                '5/7/2013 - enclosed in Try/Catch to prevent unhandled exception (so page would still work)
                reprintResponse = reprintProxy.ReprintJob(reprintRequest)
            Catch ex As Exception

            End Try
        End Using

        If reprintResponse IsNot Nothing AndAlso reprintResponse.ResponseData IsNot Nothing AndAlso reprintResponse.ResponseData.Data IsNot Nothing Then
            Return reprintResponse.ResponseData.Data
        Else
            If reprintResponse.DiamondValidation.HasErrors Then
                For Each diaVal As Diamond.Common.Objects.ValidationItem In reprintResponse.DiamondValidation.ValidationItems
                    If diaVal.ItemType = Diamond.Common.Objects.ValidationItemType.ValidationError Then
                        'errMsg &= diaVal.Message & Environment.NewLine & Environment.NewLine
                    End If
                Next
            End If
            Return Nothing
        End If
    End Function
    Private Function GetPrintForm(ByVal pfs As Generic.List(Of Diamond.Common.Objects.Printing.PrintForm)) As Byte()
        If pfs IsNot Nothing AndAlso pfs.Count > 0 Then
            Dim reprintRequest As New Diamond.Common.Services.Messages.PrintingService.ReprintJob.Request
            Dim reprintResponse As New Diamond.Common.Services.Messages.PrintingService.ReprintJob.Response

            With reprintRequest.RequestData
                .PrintForms = New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Printing.PrintForm)
                For Each pf As Diamond.Common.Objects.Printing.PrintForm In pfs
                    .PolicyId = pf.PolicyId
                    .PolicyImageNum = pf.PolicyImageNum

                    .PrintForms.Add(New Diamond.Common.Objects.Printing.PrintForm() {pf})
                Next
            End With

            Using reprintProxy As New Diamond.Common.Services.Proxies.PrintingServiceProxy
                Try
                    '5/7/2013 - enclosed in Try/Catch to prevent unhandled exception (so page would still work)
                    reprintResponse = reprintProxy.ReprintJob(reprintRequest)
                Catch ex As Exception

                End Try
            End Using

            If reprintResponse IsNot Nothing AndAlso reprintResponse.ResponseData IsNot Nothing AndAlso reprintResponse.ResponseData.Data IsNot Nothing Then
                Return reprintResponse.ResponseData.Data
            Else
                If reprintResponse.DiamondValidation.HasErrors Then
                    For Each diaVal As Diamond.Common.Objects.ValidationItem In reprintResponse.DiamondValidation.ValidationItems
                        If diaVal.ItemType = Diamond.Common.Objects.ValidationItemType.ValidationError Then
                            'errMsg &= diaVal.Message & Environment.NewLine & Environment.NewLine
                        End If
                    Next
                End If
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function

    Protected Sub btnGoToPrintHistory_Click(sender As Object, e As System.EventArgs) Handles btnGoToPrintHistory.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        '8/21/2012 - added configurable option to open in new window
        If ConfigurationManager.AppSettings("QuickQuote_OpenPrintFormInNewWindow") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_OpenPrintFormInNewWindow").ToString) = "YES" Then
            OpenNewWindow("DiamondPrintForm.aspx?PolicyId=" & Me.lblPolicyId.Text)
        Else
            Response.Redirect("DiamondPrintForm.aspx?PolicyId=" & Me.lblPolicyId.Text)
        End If
    End Sub
    '2/28/2013 - replaced old button logic
    Protected Sub btnPrint_Click(sender As Object, e As System.EventArgs) Handles btnPrint.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        '8/21/2012 - added configurable option to open in new window
        If ConfigurationManager.AppSettings("QuickQuote_OpenPrintFormInNewWindow") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_OpenPrintFormInNewWindow").ToString) = "YES" Then
            OpenNewWindow("DiamondPrintForm.aspx?PolicyId=" & Me.lblPolicyId.Text)
        Else
            Response.Redirect("DiamondPrintForm.aspx?PolicyId=" & Me.lblPolicyId.Text)
        End If
    End Sub

    Protected Sub btnCreditDebits_Click(sender As Object, e As System.EventArgs) Handles btnCreditDebits.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        If Me.lblCreditsDebitsLink.Text <> "" Then
            Response.Redirect(Me.lblCreditsDebitsLink.Text & "?QuoteId=" & Me.lblQuoteId.Text)
        End If
    End Sub
    '2/28/2013 - replaced old button logic
    Protected Sub btnIRPM_Click(sender As Object, e As System.EventArgs) Handles btnIRPM.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        Dim IrpmLink As String = ""
        If ConfigurationManager.AppSettings("QuickQuote_IRPM_Input") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_IRPM_Input").ToString <> "" Then
            IrpmLink = ConfigurationManager.AppSettings("QuickQuote_IRPM_Input").ToString
        Else
            IrpmLink = "DiamondQuoteIRPM.aspx"
        End If
        Response.Redirect(IrpmLink & "?QuoteId=" & Me.lblQuoteId.Text)
    End Sub

    Protected Sub btnRouteToUnderwriting_Click(sender As Object, e As System.EventArgs) Handles btnRouteToUnderwriting.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        '1/11/2013 - moved logic to seperate Sub so other btn_Clicks could also call
        RouteToUW(RouteToUnderwritingType.QuoteSuccess)
    End Sub
    '2/28/2013 - replaced old button logic
    Protected Sub btnRoute_Click(sender As Object, e As System.EventArgs) Handles btnRoute.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        Dim routeType As RouteToUnderwritingType = Nothing
        If ViewState("QQ_Route_Type") IsNot Nothing Then
            routeType = CType(ViewState("QQ_Route_Type"), RouteToUnderwritingType)
        End If

        '1/11/2013 - moved logic to seperate Sub so other btn_Clicks could also call
        RouteToUW(routeType)

    End Sub
    Private Sub RouteToUW(ByVal routeType As RouteToUnderwritingType)
        Dim policyID As Integer = 0
        Dim policyImageNum As Integer = 0
        Dim errorMsg As String = ""
        Dim success As Boolean = False
        Dim agencyID As Integer = 0 'added 11/8/2012
        Dim userID As Integer = 0 'added 11/8/2012

        'If Me.lblPolicyId.Text <> "" AndAlso Me.lblPolicyImageNum.Text <> "" AndAlso IsNumeric(Me.lblPolicyId.Text) = True AndAlso IsNumeric(Me.lblPolicyImageNum.Text) = True Then
        'updated 11/8/2012 to also check agencyID
        If Me.lblPolicyId.Text <> "" AndAlso Me.lblPolicyImageNum.Text <> "" AndAlso Me.lblDiaAgencyId.Text <> "" AndAlso IsNumeric(Me.lblPolicyId.Text) = True AndAlso IsNumeric(Me.lblPolicyImageNum.Text) = True AndAlso IsNumeric(Me.lblDiaAgencyId.Text) = True Then
            policyID = CInt(Me.lblPolicyId.Text)
            policyImageNum = CInt(Me.lblPolicyImageNum.Text)
            agencyID = CInt(Me.lblDiaAgencyId.Text) 'added 11/8/2012
            'added 11/8/2012
            If System.Web.HttpContext.Current.Session("DiamondUserId") IsNot Nothing AndAlso System.Web.HttpContext.Current.Session("DiamondUserId").ToString <> "" AndAlso IsNumeric(System.Web.HttpContext.Current.Session("DiamondUserId").ToString) = True Then
                userID = CInt(System.Web.HttpContext.Current.Session("DiamondUserId").ToString)
            ElseIf ConfigurationManager.AppSettings("TestOrProd") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("TestOrProd").ToString) = "TEST" AndAlso ConfigurationManager.AppSettings("QuickQuote_UseTestVariables") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_UseTestVariables").ToString) = "YES" Then
                userID = CInt(ConfigurationManager.AppSettings("QuickQuoteTestUserId").ToString)
            End If

            Try
                'Dim req As New Diamond.Common.Services.Messages.PolicyService.RouteQuoteToUnderwriting.Request
                'Dim res As New Diamond.Common.Services.Messages.PolicyService.RouteQuoteToUnderwriting.Response
                'req.RequestData.PolicyId = policyID
                'req.RequestData.PolicyImageNum = policyImageNum
                'Using proxy As New Diamond.Common.Services.Proxies.PolicyServices.PolicyServiceProxy
                '    res = proxy.RouteQuoteToUnderwriting(req)
                'End Using
                'If res IsNot Nothing AndAlso res.ResponseData IsNot Nothing Then
                '    success = res.ResponseData.Success
                '    If res.DiamondValidation IsNot Nothing AndAlso res.DiamondValidation.ValidationItems IsNot Nothing AndAlso res.DiamondValidation.ValidationItems.Count > 0 Then
                '        For Each vi As Diamond.Common.Objects.ValidationItem In res.DiamondValidation.ValidationItems
                '            If errorMsg <> "" Then
                '                errorMsg &= "<br />"
                '            End If
                '            errorMsg &= vi.Message
                '        Next
                '    End If
                'Else
                '    'no response data
                'End If

                'updated 11/8/2012 to use different service call
                Dim request As New Diamond.Common.Services.Messages.WorkflowService.TransferTaskToAgencyQueue.Request
                Dim response As New Diamond.Common.Services.Messages.WorkflowService.TransferTaskToAgencyQueue.Response

                With request.RequestData
                    .PolicyId = policyID
                    .PolicyImageNum = policyImageNum
                    .AgencyId = agencyID
                    .CurrentUsersId = userID
                    .NewUsersId = userID 'testing 11/9/2012; not sure if this is needed (works now; was previously just working w/ 524)
                    .WorkflowQueueTypeId = Diamond.Common.Enums.Workflow.WorkflowQueueType.Help

                    'added 1/11/2013 to better identify Route To Underwriting scenarios
                    Dim remarksAppendedText As String = ""
                    If routeType <> Nothing Then 'added IF on 3/1/2013
                        Select Case routeType
                            Case RouteToUnderwritingType.QuoteSuccess 'everything would have been this before 1/11/2013
                                remarksAppendedText = " (after Quote Success)"
                            Case RouteToUnderwritingType.QuoteFail
                                remarksAppendedText = " (after Quote Fail)"
                            Case RouteToUnderwritingType.AppGapSuccess 'added 3/1/2013
                                remarksAppendedText = " (after AppGap Success)"
                            Case RouteToUnderwritingType.AppGapFail
                                remarksAppendedText = " (after AppGap Fail)"
                        End Select
                    End If

                    Dim valItemsMsg As String = "" 'added 4/11/2013 to use w/ Route To UW
                    If ViewState("valItemsMsg") IsNot Nothing Then
                        valItemsMsg = CType(ViewState("valItemsMsg"), String)
                        If valItemsMsg <> "" Then
                            remarksAppendedText &= "; " & valItemsMsg
                        End If
                    End If

                    '.Remarks = "routed to UW from VelociRater Quote Summary page"
                    'updated 1/11/2013 to better identify Route To Underwriting scenarios
                    .Remarks = "routed to UW from VelociRater Quote Summary page" & remarksAppendedText
                    .Urgent = True
                    .Mandatory = True
                End With

                Using workflowService As New Diamond.Common.Services.Proxies.Workflow.WorkflowServiceProxy
                    response = workflowService.TransferTaskToAgencyQueue(request)
                End Using

                If response IsNot Nothing AndAlso response.ResponseData IsNot Nothing Then
                    success = response.ResponseData.OperationSuccessful
                    If response.DiamondValidation IsNot Nothing AndAlso response.DiamondValidation.ValidationItems IsNot Nothing AndAlso response.DiamondValidation.ValidationItems.Count > 0 Then
                        For Each vi As Diamond.Common.Objects.ValidationItem In response.DiamondValidation.ValidationItems
                            If errorMsg <> "" Then
                                errorMsg &= "<br />"
                            End If
                            errorMsg &= vi.Message
                        Next
                    End If
                Else
                    'no response data
                End If

            Catch ex As Exception
                'unhandled error
            End Try
        Else
            'missing policyId and/or policyImageNum params
        End If

        If success = True Then
            'okay
            'update status to Referred (Routed) To Underwriting
            QQxml.UpdateQuoteStatus(Me.lblQuoteId.Text, QuickQuoteXML.QuickQuoteStatusType.ReferredToUW, errorMsg)
            'If errorMsg <> "" Then
            '    ShowError("There was a problem referring this quote:  " & errorMsg)
            'Else

            'End If
            'added 7/8/2013
            If errorMsg = "" Then
                QQxml.GetQuoteHistoryAndSaveNote(qqHelper.IntegerForString(Me.lblQuoteId.Text), policyID, policyImageNum, userID)
            End If

            ShowError("Your quote was successfully routed.", True)
        Else
            If errorMsg <> "" Then
                ShowError("The following errors were encountered while routing your quote:  " & errorMsg)
                'added 11/8/2012 PM
                Dim emBody As String = "Error Message:  " & errorMsg
                emBody &= "<br /><br />"
                emBody &= "PolicyId = " & policyID
                emBody &= "<br />PolicyImageNum = " & policyImageNum
                emBody &= "<br />AgencyId = " & agencyID
                emBody &= "<br />UserId = " & userID
                qqHelper.SendEmail("VelociRaterErrors@indianafarmers.com", "dmink@indianafarmers.com", "VR Route To UW Error", emBody)
            Else
                ShowError("There was a problem routing your quote.  Please try again later.")
            End If
        End If
    End Sub
    Private Sub ShowError(ByVal message As String, Optional ByVal redirect As Boolean = False, Optional ByVal redirectPage As String = "")
        message = Replace(message, "\", "\\")
        message = Replace(message, "<br>", "\n")
        message = Replace(message, vbCrLf, "\n")

        Dim strScript As String = "<script language=JavaScript>"
        strScript &= "alert(""" & message & """);"
        If redirect = True Then
            If redirectPage = "" Then
                redirectPage = ConfigurationManager.AppSettings("QuickQuote_SavedQuotes").ToString
            End If
            strScript &= " window.location.href='" & redirectPage & "';"
        End If
        strScript &= "</script>"

        Page.RegisterStartupScript("clientScript", strScript)

    End Sub

    Protected Sub btnFinalizeInDiamond_Click(sender As Object, e As System.EventArgs) Handles btnFinalizeInDiamond.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        If Me.lblGoToPortalLink.Text <> "" Then
            'change status in Quote table
            Dim errorMsg As String = ""
            QQxml.UpdateQuoteStatus(Me.lblQuoteId.Text, QuickQuoteXML.QuickQuoteStatusType.Finalized, errorMsg)
            If errorMsg <> "" Then
                ShowError("There was a problem finalizing this quote:  " & errorMsg)
            Else
                'added 7/8/2013
                Dim userId As Integer = 0
                If System.Web.HttpContext.Current.Session("DiamondUserId") IsNot Nothing AndAlso System.Web.HttpContext.Current.Session("DiamondUserId").ToString <> "" AndAlso IsNumeric(System.Web.HttpContext.Current.Session("DiamondUserId").ToString) = True Then
                    userId = CInt(System.Web.HttpContext.Current.Session("DiamondUserId").ToString)
                ElseIf ConfigurationManager.AppSettings("TestOrProd") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("TestOrProd").ToString) = "TEST" AndAlso ConfigurationManager.AppSettings("QuickQuote_UseTestVariables") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_UseTestVariables").ToString) = "YES" Then
                    userId = CInt(ConfigurationManager.AppSettings("QuickQuoteTestUserId").ToString)
                End If
                QQxml.GetQuoteHistoryAndSaveNote(qqHelper.IntegerForString(Me.lblQuoteId.Text), qqHelper.IntegerForString(Me.lblPolicyId.Text), qqHelper.IntegerForString(Me.lblPolicyImageNum.Text), userId)
                Response.Redirect(Me.lblGoToPortalLink.Text)
            End If
        Else
            ShowError("This quote cannot be finalized at this time.  Please try again later.")
        End If
    End Sub
    '2/28/2013 - replaced old button logic
    Protected Sub btnFinalize_Click(sender As Object, e As System.EventArgs) Handles btnFinalize.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        If Me.lblPortalLink.Text <> "" Then
            'change status in Quote table
            Dim errorMsg As String = ""
            QQxml.UpdateQuoteStatus(Me.lblQuoteId.Text, QuickQuoteXML.QuickQuoteStatusType.Finalized, errorMsg)
            If errorMsg <> "" Then
                ShowError("There was a problem finalizing this quote:  " & errorMsg)
            Else
                'added 7/8/2013
                Dim userId As Integer = 0
                If System.Web.HttpContext.Current.Session("DiamondUserId") IsNot Nothing AndAlso System.Web.HttpContext.Current.Session("DiamondUserId").ToString <> "" AndAlso IsNumeric(System.Web.HttpContext.Current.Session("DiamondUserId").ToString) = True Then
                    userId = CInt(System.Web.HttpContext.Current.Session("DiamondUserId").ToString)
                ElseIf ConfigurationManager.AppSettings("TestOrProd") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("TestOrProd").ToString) = "TEST" AndAlso ConfigurationManager.AppSettings("QuickQuote_UseTestVariables") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_UseTestVariables").ToString) = "YES" Then
                    userId = CInt(ConfigurationManager.AppSettings("QuickQuoteTestUserId").ToString)
                End If
                QQxml.GetQuoteHistoryAndSaveNote(qqHelper.IntegerForString(Me.lblQuoteId.Text), qqHelper.IntegerForString(Me.lblPolicyId.Text), qqHelper.IntegerForString(Me.lblPolicyImageNum.Text), userId)
                Response.Redirect(Me.lblPortalLink.Text)
            End If
        Else
            ShowError("This quote cannot be finalized at this time.  Please try again later.")
        End If
    End Sub

    Protected Sub btnCopyQuote_Click(sender As Object, e As System.EventArgs) Handles btnCopyQuote.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        Dim errorMsg As String = ""
        Dim newQuoteId As String = ""

        If Me.lblQuoteId.Text <> "" AndAlso IsNumeric(Me.lblQuoteId.Text) = True Then
            QQxml.CopyQuote(Me.lblQuoteId.Text, newQuoteId, errorMsg)
        Else
            errorMsg = "no quote id was found"
        End If

        If errorMsg = "" AndAlso newQuoteId <> "" Then
            'success
            Dim lobType As QuickQuoteObject.QuickQuoteLobType = QuickQuoteObject.QuickQuoteLobType.None
            If ViewState("QQ_Lob_Type") IsNot Nothing Then
                lobType = CType(ViewState("QQ_Lob_Type"), QuickQuoteObject.QuickQuoteLobType)
            End If

            Dim redirectLink As String = ""

            If lobType <> QuickQuoteObject.QuickQuoteLobType.None Then
                Select Case lobType
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                        redirectLink = ConfigurationManager.AppSettings("QuickQuote_BOP_Input").ToString & "?QuoteId=" & newQuoteId
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
                        redirectLink = ConfigurationManager.AppSettings("QuickQuote_CGL_Input").ToString & "?QuoteId=" & newQuoteId
                    Case QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                        redirectLink = ConfigurationManager.AppSettings("QuickQuote_WC_Input").ToString & "?QuoteId=" & newQuoteId
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                        redirectLink = ConfigurationManager.AppSettings("QuickQuote_CPR_Input").ToString & "?QuoteId=" & newQuoteId
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                        redirectLink = ConfigurationManager.AppSettings("QuickQuote_CPP_Input").ToString & "?QuoteId=" & newQuoteId
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                        redirectLink = ConfigurationManager.AppSettings("QuickQuote_CAP_Input").ToString & "?QuoteId=" & newQuoteId
                End Select
            End If

            ShowError("Your quote was copied successfully.", True, redirectLink)
        Else
            ShowError("There was a problem copying this quote:  " & errorMsg)
        End If
    End Sub
    '2/28/2013 - replaced old button logic
    Protected Sub btnCopy_Click(sender As Object, e As System.EventArgs) Handles btnCopy.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        Dim errorMsg As String = ""
        Dim newQuoteId As String = ""

        If Me.lblQuoteId.Text <> "" AndAlso IsNumeric(Me.lblQuoteId.Text) = True Then
            QQxml.CopyQuote(Me.lblQuoteId.Text, newQuoteId, errorMsg)
        Else
            errorMsg = "no quote id was found"
        End If

        If errorMsg = "" AndAlso newQuoteId <> "" Then
            'success
            Dim lobType As QuickQuoteObject.QuickQuoteLobType = QuickQuoteObject.QuickQuoteLobType.None
            If ViewState("QQ_Lob_Type") IsNot Nothing Then
                lobType = CType(ViewState("QQ_Lob_Type"), QuickQuoteObject.QuickQuoteLobType)
            End If

            Dim redirectLink As String = ""

            If lobType <> QuickQuoteObject.QuickQuoteLobType.None Then
                Select Case lobType
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                        redirectLink = ConfigurationManager.AppSettings("QuickQuote_BOP_Input").ToString & "?QuoteId=" & newQuoteId
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
                        redirectLink = ConfigurationManager.AppSettings("QuickQuote_CGL_Input").ToString & "?QuoteId=" & newQuoteId
                    Case QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                        redirectLink = ConfigurationManager.AppSettings("QuickQuote_WC_Input").ToString & "?QuoteId=" & newQuoteId
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                        redirectLink = ConfigurationManager.AppSettings("QuickQuote_CPR_Input").ToString & "?QuoteId=" & newQuoteId
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                        redirectLink = ConfigurationManager.AppSettings("QuickQuote_CPP_Input").ToString & "?QuoteId=" & newQuoteId
                    Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                        redirectLink = ConfigurationManager.AppSettings("QuickQuote_CAP_Input").ToString & "?QuoteId=" & newQuoteId
                End Select
            End If

            ShowError("Your quote was copied successfully.", True, redirectLink)
        Else
            ShowError("There was a problem copying this quote:  " & errorMsg)
        End If
    End Sub

    Protected Sub btnGoToDiamondPortal_Click(sender As Object, e As System.EventArgs) Handles btnGoToDiamondPortal.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        If Me.lblGoToPortalLink.Text <> "" Then
            Response.Redirect(Me.lblGoToPortalLink.Text)
        Else
            ShowError("This quote cannot be loaded in the Diamond portal at this time.  Please try again later.")
        End If
    End Sub
    '2/28/2013 - replaced old button logic
    Protected Sub btnPortal_Click(sender As Object, e As System.EventArgs) Handles btnPortal.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        If Me.lblPortalLink.Text <> "" Then
            Response.Redirect(Me.lblPortalLink.Text)
        Else
            ShowError("This quote cannot be loaded in the Diamond portal at this time.  Please try again later.")
        End If
    End Sub

    Private Sub PromoteTest()
        Dim policyID As Integer = 0
        Dim policyImageNum As Integer = 0
        Dim errorMsg As String = ""
        Dim success As Boolean = False

        If Me.lblPolicyId.Text <> "" AndAlso Me.lblPolicyImageNum.Text <> "" AndAlso IsNumeric(Me.lblPolicyId.Text) = True AndAlso IsNumeric(Me.lblPolicyImageNum.Text) = True Then
            policyID = CInt(Me.lblPolicyId.Text)
            policyImageNum = CInt(Me.lblPolicyImageNum.Text)

            Try
                Dim req As New Diamond.Common.Services.Messages.PolicyService.PromoteQuoteToPending.Request
                Dim res As New Diamond.Common.Services.Messages.PolicyService.PromoteQuoteToPending.Response
                req.RequestData.PolicyId = policyID
                req.RequestData.PolicyImageNum = policyImageNum
                Using proxy As New Diamond.Common.Services.Proxies.PolicyServices.PolicyServiceProxy
                    res = proxy.PromoteQuoteToPending(req)
                End Using
                If res IsNot Nothing AndAlso res.ResponseData IsNot Nothing Then
                    'success = res.ResponseData.Success
                    If res.DiamondValidation IsNot Nothing AndAlso res.DiamondValidation.ValidationItems IsNot Nothing AndAlso res.DiamondValidation.ValidationItems.Count > 0 Then
                        For Each vi As Diamond.Common.Objects.ValidationItem In res.DiamondValidation.ValidationItems
                            If errorMsg <> "" Then
                                errorMsg &= "<br />"
                            End If
                            errorMsg &= vi.Message
                        Next
                    End If
                Else
                    'no response data
                End If
            Catch ex As Exception
                'unhandled error
            End Try
        Else
            'missing policyId and/or policyImageNum params
        End If

        If success = True Then
            'okay
            'update status to Referred (Routed) To Underwriting
            QQxml.UpdateQuoteStatus(Me.lblQuoteId.Text, QuickQuoteXML.QuickQuoteStatusType.ReferredToUW, errorMsg)
            'If errorMsg <> "" Then
            '    ShowError("There was a problem referring this quote:  " & errorMsg)
            'Else

            'End If

            ShowError("Your quote was successfully promoted.", True)
        Else
            If errorMsg <> "" Then
                ShowError("The following errors were encountered while promoting your quote:  " & errorMsg)
            Else
                ShowError("There was a problem promoting your quote.  Please try again later.")
            End If
        End If
    End Sub

    'added for testing 7/2/2013
    Private Sub AddNoteTest()
        Dim policyID As Integer = 0
        Dim policyImageNum As Integer = 0
        Dim errorMsg As String = ""
        Dim success As Boolean = False

        If Me.lblPolicyId.Text <> "" AndAlso Me.lblPolicyImageNum.Text <> "" AndAlso IsNumeric(Me.lblPolicyId.Text) = True AndAlso IsNumeric(Me.lblPolicyImageNum.Text) = True Then
            policyID = CInt(Me.lblPolicyId.Text)
            policyImageNum = CInt(Me.lblPolicyImageNum.Text)

            Try
                Dim req As New Diamond.Common.Services.Messages.NotesService.CreateNote.Request
                Dim res As New Diamond.Common.Services.Messages.NotesService.CreateNote.Response
                'Dim req As New Diamond.Common.Services.Messages.NotesService.SaveNote.Request
                'Dim res As New Diamond.Common.Services.Messages.NotesService.SaveNote.Response

                With req.RequestData.NoteStruct
                    .Key01 = policyID
                    .Key02 = policyImageNum
                    .Key03 = 0
                    .Key04 = 0

                    With .Note
                        .CreateKey = policyID
                        .Note = "Note text"
                        .Title = "Policy Task Transferred"
                        .UserId = 0
                        '.UserName = "" 'probably don't need to set
                        .IsSticky = False
                        .NoteStatus = "Y"
                        .AttachLevelId = 2
                        .OwnerId = 0 '?
                        '.OwnerName = ""
                        .IsPrivate = True
                        .IsUrgent = False
                        .CheckOnRenewal = False
                        '.NotesTypeId = 0 'N/A
                        'updated 11/12/2016 for 531.009 since .NotesTypeId is no longer available; may not even need .NotesTypeIds... Diamond may automatically create (wouldn't be able to set .NoteId anyway); will not do anything w/ .NotesTypeIds for now
                        'If .NotesTypeIds Is Nothing Then
                        '    .NotesTypeIds = New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.Notes.NotesTypeLink)
                        'End If
                        'If .NotesTypeIds.Count = 0 Then
                        '    Dim ntl As New Diamond.Common.Objects.Notes.NotesTypeLink
                        '    With ntl
                        '        '.NoteId = 0
                        '        '.NotesTypeDescription = ""
                        '        .NotesTypeId = 0 'N/A
                        '    End With
                        '    .NotesTypeIds.Add(ntl)
                        'End If
                    End With

                End With

                Using proxy As New Diamond.Common.Services.Proxies.NotesServices.NotesServiceProxy
                    res = proxy.CreateNote(req)
                    'res = proxy.SaveNote(req)
                End Using
                If res IsNot Nothing AndAlso res.ResponseData IsNot Nothing Then
                    'success = res.ResponseData.Success
                    If res.DiamondValidation IsNot Nothing AndAlso res.DiamondValidation.ValidationItems IsNot Nothing AndAlso res.DiamondValidation.ValidationItems.Count > 0 Then
                        For Each vi As Diamond.Common.Objects.ValidationItem In res.DiamondValidation.ValidationItems
                            If errorMsg <> "" Then
                                errorMsg &= "<br />"
                            End If
                            errorMsg &= vi.Message
                        Next
                    End If
                Else
                    'no response data
                End If
            Catch ex As Exception
                'unhandled error
            End Try
        Else
            'missing policyId and/or policyImageNum params
        End If

        If success = True Then
            'okay
            'update status to Referred (Routed) To Underwriting
            'QQxml.UpdateQuoteStatus(Me.lblQuoteId.Text, QuickQuoteXML.QuickQuoteStatusType.ReferredToUW, errorMsg)
            'If errorMsg <> "" Then
            '    ShowError("There was a problem referring this quote:  " & errorMsg)
            'Else

            'End If

            ShowError("Your note was successfully added.", True)
        Else
            If errorMsg <> "" Then
                ShowError("The following errors were encountered while adding your note:  " & errorMsg)
            Else
                ShowError("There was a problem adding your note.  Please try again later.")
            End If
        End If
    End Sub
    Private Sub OpenNewWindow(ByVal url As String)
        If url <> "" Then
            Dim strScript As String = "<script language=JavaScript>"
            'strScript &= " window.open('" & url & "');"
            strScript &= " window.open(""" & url & """);"
            strScript &= "</script>"

            Page.RegisterStartupScript("clientScript", strScript)
        End If
    End Sub

    Protected Sub rpt_GL_ClassCodes_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rpt_GL_ClassCodes.ItemDataBound
        'added 8/27/2012
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim ClassCodeRow As HtmlTableRow = e.Item.FindControl("ClassCodeRow")
            Dim ClassCodeDescriptionRow As HtmlTableRow = e.Item.FindControl("ClassCodeDescriptionRow")
            Dim ClassCodePremExpRow As HtmlTableRow = e.Item.FindControl("ClassCodePremExpRow")
            Dim ClassCodePremBaseRow As HtmlTableRow = e.Item.FindControl("ClassCodePremBaseRow")
            Dim ClassCodeProductsPremRow As HtmlTableRow = e.Item.FindControl("ClassCodeProductsPremRow")
            Dim ClassCodePremisesPremRow As HtmlTableRow = e.Item.FindControl("ClassCodePremisesPremRow")

            ClassCodeRow.Visible = False 'GL
            ClassCodeDescriptionRow.Visible = False 'GL
            ClassCodePremExpRow.Visible = False 'GL
            ClassCodePremBaseRow.Visible = False 'GL
            ClassCodeProductsPremRow.Visible = False 'GL
            ClassCodePremisesPremRow.Visible = False 'GL

            'If quickQuote IsNot Nothing AndAlso quickQuote.LobType <> Nothing AndAlso quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability Then
            'updated 11/19/2012 for CPP
            If quickQuote IsNot Nothing AndAlso quickQuote.LobType <> Nothing AndAlso (quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability OrElse quickQuote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage) Then
                ClassCodeRow.Visible = True
                ClassCodeDescriptionRow.Visible = True
                ClassCodePremExpRow.Visible = True
                ClassCodePremBaseRow.Visible = True
                ClassCodeProductsPremRow.Visible = True
                ClassCodePremisesPremRow.Visible = True
            End If
        End If
    End Sub

    Protected Sub btnExitQuote_Click(sender As Object, e As System.EventArgs) Handles btnExitQuote.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        If ConfigurationManager.AppSettings("QuickQuote_HomePage") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_HomePage").ToString <> "" Then
            Response.Redirect(ConfigurationManager.AppSettings("QuickQuote_HomePage").ToString)
        Else
            Response.Redirect("DiamondQuickQuoteHome.aspx")
        End If
    End Sub
    '2/28/2013 - replaced old button logic
    Protected Sub btnExit_Click(sender As Object, e As System.EventArgs) Handles btnExit.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        If ConfigurationManager.AppSettings("QuickQuote_HomePage") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_HomePage").ToString <> "" Then
            Response.Redirect(ConfigurationManager.AppSettings("QuickQuote_HomePage").ToString)
        Else
            Response.Redirect("DiamondQuickQuoteHome.aspx")
        End If
    End Sub

    Protected Sub btnNewQuoteForClient_Click(sender As Object, e As System.EventArgs) Handles btnNewQuoteForClient.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        If Me.lblDiaAgencyId.Text <> "" AndAlso Me.lblDiaAgencyCode.Text <> "" AndAlso ViewState.Item("QQ_Client") IsNot Nothing Then
            Dim qq As New QuickQuoteObject
            qq.AgencyId = Me.lblDiaAgencyId.Text
            qq.AgencyCode = Me.lblDiaAgencyCode.Text
            qq.Client = CType(ViewState.Item("QQ_Client"), QuickQuoteClient)
            Dim qId As String = ""
            Dim errMsg As String = ""
            QQxml.SaveQuote(QuickQuoteXML.QuickQuoteSaveType.Quote, qq, qId, errMsg)
            If errMsg = "" Then
                'okay
                If qId <> "" Then
                    ShowError("Your quote was created successfully.  You will now be redirected to the LOB Selection page.", True, ConfigurationManager.AppSettings("QuickQuote_LOB_Selection").ToString & "?QuoteId=" & qId)
                Else
                    'problem
                    ShowError("There was a problem creating your new quote.  Please try again later.")
                End If
            Else
                ShowError("There was a problem creating your new quote:  " & errMsg)
            End If
        Else
            ShowError("A new quote cannot be created for this client right now.  Please try again later.")
        End If
    End Sub
    '2/28/2013 - replaced old button logic
    Protected Sub btnNew_Click(sender As Object, e As System.EventArgs) Handles btnNew.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        If Me.lblDiaAgencyId.Text <> "" AndAlso Me.lblDiaAgencyCode.Text <> "" AndAlso ViewState.Item("QQ_Client") IsNot Nothing Then
            Dim qq As New QuickQuoteObject
            qq.AgencyId = Me.lblDiaAgencyId.Text
            qq.AgencyCode = Me.lblDiaAgencyCode.Text
            qq.Client = CType(ViewState.Item("QQ_Client"), QuickQuoteClient)
            Dim qId As String = ""
            Dim errMsg As String = ""
            QQxml.SaveQuote(QuickQuoteXML.QuickQuoteSaveType.Quote, qq, qId, errMsg)
            If errMsg = "" Then
                'okay
                If qId <> "" Then
                    ShowError("Your quote was created successfully.  You will now be redirected to the LOB Selection page.", True, ConfigurationManager.AppSettings("QuickQuote_LOB_Selection").ToString & "?QuoteId=" & qId)
                Else
                    'problem
                    ShowError("There was a problem creating your new quote.  Please try again later.")
                End If
            Else
                ShowError("There was a problem creating your new quote:  " & errMsg)
            End If
        Else
            ShowError("A new quote cannot be created for this client right now.  Please try again later.")
        End If
    End Sub





    Protected Sub btnReturnToQuote2_Click(sender As Object, e As System.EventArgs) Handles btnReturnToQuote2.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        If Me.lblReturnToQuoteLink2.Text <> "" Then
            Response.Redirect(Me.lblReturnToQuoteLink2.Text)
        End If

    End Sub

    Protected Sub btnRouteToUnderwriting2_Click(sender As Object, e As System.EventArgs) Handles btnRouteToUnderwriting2.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        RouteToUW(RouteToUnderwritingType.QuoteFail)
    End Sub

    Protected Sub btnRouteToUnderwriting3_Click(sender As Object, e As System.EventArgs) Handles btnRouteToUnderwriting3.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        RouteToUW(RouteToUnderwritingType.AppGapFail)
    End Sub

    Protected Sub btnProposal_Click(sender As Object, e As System.EventArgs) Handles btnProposal.Click 'added 5/7/2013
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        If ConfigurationManager.AppSettings("QuickQuote_QuoteProposalSelection") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_QuoteProposalSelection").ToString <> "" Then
            Dim proposalLink As String = ConfigurationManager.AppSettings("QuickQuote_QuoteProposalSelection").ToString & "?QuoteId=" & Me.lblQuoteId.Text
            'uses configurable option to open in new window
            If ConfigurationManager.AppSettings("QuickQuote_OpenProposalSelectionInNewWindow") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_OpenProposalSelectionInNewWindow").ToString) = "YES" Then
                OpenNewWindow(proposalLink)
            Else
                Response.Redirect(proposalLink)
            End If
        End If

    End Sub
    'added 10/30/2014 to clear quotes out of session... to avoid any scenarios where a change is made outside of quote input page (i.e. IRPM page) that is not persisted the next time you return to the quote
    Public Shared Sub ClearQuotesFromSession(ByVal session As HttpSessionState)
        Dim removeIndexes As New List(Of Int32)
        Dim i As Int32 = 0
        For Each k As String In session.Keys
            If k.StartsWith("sq_") Then
                removeIndexes.Add(i)
            End If
            i += 1
        Next
        removeIndexes.Reverse()
        For Each n As Int32 In removeIndexes
            session.RemoveAt(n)
        Next

    End Sub


#Region "CRM/CIM"

    Public Shared Function TryToGetDouble(ByVal inputText As String, Optional setToZeroIfNegative As Boolean = False) As Double
        Try
            If String.IsNullOrWhiteSpace(inputText) = False Then
                inputText = inputText.Replace("$", "")
                inputText = inputText.Replace(",", "")
                Dim val As Double = CDbl(inputText)
                If setToZeroIfNegative AndAlso val < 0.0 Then
                    Return 0.0
                End If
                Return val
            Else
                Return 0.0
            End If

        Catch ex As Exception
        End Try
        Return 0
    End Function

    Public Shared Function TryToFormatAsCurrency(ByVal inputText As String, Optional showCents As Boolean = True, Optional showEmptyWhenZero As Boolean = False) As String
        Try
            Dim dolVal As Double = TryToGetDouble(inputText)
            If dolVal = 0 AndAlso showEmptyWhenZero Then
                Return ""
            Else
                If showCents Then
                    Return String.Format("{0:C2}", dolVal)
                Else
                    Return String.Format("{0:C0}", dolVal)
                End If
            End If

        Catch ex As Exception
        End Try
        Return inputText
    End Function

    Public Shared Function TryToFormatAsWholeNumber(ByVal inputText As String) As String
        Try
            Dim dolVal As Double = TryToGetDouble(inputText)
            If dolVal > 0 Then
                Return String.Format("{0:N0}", dolVal)
            Else
                Return ""
            End If

        Catch ex As Exception
        End Try
        Return inputText
    End Function

    Protected Shared Function GetDeductibleTextForValue(deductibleId As String) As String
        Select Case deductibleId.Trim()
            Case "4"
                Return "250"
            Case "8"
                Return "500"
            Case "9"
                Return "1,000"
            Case "15"
                Return "2,500"
            Case "16"
                Return "5,000"
            Case "17"
                Return "10,000"
            Case "19"
                Return "25,000"

            Case "34"
                Return "5%"
            Case "36"
                Return "10%"
            Case "37"
                Return "15%"
            Case "38"
                Return "20%"
            Case "39"
                Return "25%"

            Case Else
                Return ""
        End Select
    End Function

    ''' <summary>
    ''' Returns all buildings under all Location. Has sub properties that indicate the orginal location and building indexes.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Friend ReadOnly Property Buildings As List(Of Summary_Building)
        Get
            Dim list As New List(Of Summary_Building)
            Dim qso As QuickQuoteObject = Me.MyQuote
            If qso IsNot Nothing Then
                If qso.Locations IsNot Nothing Then
                    For locIndex As Int32 = 0 To qso.Locations.Count - 1
                        If qso.Locations(locIndex).Buildings IsNot Nothing Then
                            For bIndex As Int32 = 0 To qso.Locations(locIndex).Buildings.Count - 1
                                list.Add(New Summary_Building(locIndex, bIndex, qso.Locations(locIndex).Buildings(bIndex)))
                            Next
                        End If
                    Next
                End If
            End If
            Return list
        End Get
    End Property

#End Region

    ''' <summary>
    ''' Adds up all limits of Bop, CPR/CPP Policies. Will ignore any negative limits.
    ''' </summary>
    ''' <param name="qso"></param>
    ''' <param name="maxLimitValue"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function TotalLimitsExceedValue(ByVal qso As QuickQuoteObject, ByVal maxLimitValue As Double) As Boolean

        '"Limits of insurance exceed 3 million; please contact your underwriter to continue."

        Dim totalLimit As Double = 0
        If qso IsNot Nothing Then

            'CPR/CPP
            If qso.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Or qso.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty Then
                If qso.CPP_Has_InlandMarine_PackagePart Then
                    'DONT INCLUDE - BR
                    'If qso.BuildersRiskScheduledLocations IsNot Nothing Then
                    '    For Each i As QuickQuoteBuildersRiskScheduledLocation In qso.BuildersRiskScheduledLocations
                    '        totalLimit += TryToGetDouble(i.Limit, True)
                    '    Next
                    'End If

                    ' DO NOT INCLUDE  - installation Floater 

                    If qso.ScheduledPropertyItems IsNot Nothing Then
                        For Each se As QuickQuoteScheduledPropertyItem In qso.ScheduledPropertyItems
                            totalLimit += TryToGetDouble(se.Limit, True)
                        Next
                    End If

                    If qso.MotorTruckCargoScheduledVehicles IsNot Nothing Then
                        For Each tc As QuickQuoteScheduledVehicle In qso.MotorTruckCargoScheduledVehicles
                            totalLimit += TryToGetDouble(tc.Limit, True)
                        Next
                    End If

                    totalLimit += TryToGetDouble(qso.ContractorsEquipmentLeasedRentedFromOthersLimit)

                    If qso.ContractorsEquipmentScheduledItems IsNot Nothing Then
                        For Each equip As QuickQuoteContractorsEquipmentScheduledItem In qso.ContractorsEquipmentScheduledItems
                            totalLimit += TryToGetDouble(equip.Limit, True)
                        Next
                    End If

                    totalLimit += TryToGetDouble(qso.SmallToolsAnyOneLossCatastropheLimit)



                    If qso.Locations IsNot Nothing Then
                        For Each l As QuickQuoteLocation In qso.Locations
                            If l.Buildings IsNot Nothing Then
                                For Each b As QuickQuoteBuilding In l.Buildings
                                    totalLimit += TryToGetDouble(b.ComputerHardwareLimit, True)
                                    totalLimit += TryToGetDouble(b.ComputerProgramsApplicationsAndMediaLimit, True)
                                    totalLimit += TryToGetDouble(b.ComputerBusinessIncomeLimit, True)

                                    'Fine Arts Floater ?????????
                                    If b.FineArtsScheduledItems IsNot Nothing Then
                                        For Each fa As QuickQuoteFineArtsScheduledItem In b.FineArtsScheduledItems
                                            totalLimit += TryToGetDouble(fa.Limit, True)
                                        Next
                                    End If

                                    If b.ScheduledSigns IsNot Nothing Then
                                        For Each sign As QuickQuoteScheduledSign In b.ScheduledSigns
                                            totalLimit += TryToGetDouble(sign.Limit, True)
                                        Next
                                    End If
                                Next
                            End If
                        Next
                    End If


                    totalLimit += TryToGetDouble(qso.OwnersCargoCatastropheLimit, True)
                    totalLimit += TryToGetDouble(qso.TransportationCatastropheLimit, True)

                    'totalLimit += TryToGetDouble()
                End If

                If qso.Locations IsNot Nothing Then
                    For Each l As QuickQuoteLocation In qso.Locations
                        If l.Buildings IsNot Nothing Then
                            For Each b As QuickQuoteBuilding In l.Buildings
                                If qso.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty Or qso.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                                    totalLimit += TryToGetDouble(b.Limit, True)
                                    totalLimit += TryToGetDouble(b.PersPropCov_PersonalPropertyLimit, True)
                                    totalLimit += TryToGetDouble(b.BusinessIncomeCov_Limit, True)
                                    totalLimit += TryToGetDouble(b.PersPropOfOthers_PersonalPropertyLimit, True)
                                End If
                            Next
                        End If
                        If qso.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialProperty Or qso.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                            If l.PropertyInTheOpenRecords IsNot Nothing Then
                                For Each po As QuickQuotePropertyInTheOpenRecord In l.PropertyInTheOpenRecords
                                    totalLimit += TryToGetDouble(po.Limit, True)
                                Next
                            End If
                        End If
                    Next
                End If

            End If

            ' BOP
            If qso.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialBOP Then
                If qso.Locations IsNot Nothing Then
                    For Each l As QuickQuoteLocation In qso.Locations
                        If l.Buildings IsNot Nothing Then
                            For Each b As QuickQuoteBuilding In l.Buildings
                                totalLimit += TryToGetDouble(b.Limit, True)
                                totalLimit += TryToGetDouble(b.PersonalPropertyLimit, True)
                            Next
                        End If
                    Next
                End If
            End If


        End If
        Return totalLimit > maxLimitValue
    End Function

End Class

Friend Class Summary_Building
    Public Property LocationIndex As Int32
    Public Property BuildingIndex As Int32
    Public Property Building As QuickQuoteBuilding
    Public Sub New(locationIndex As Int32, buildingIndex As Int32, building As QuickQuoteBuilding)
        Me.LocationIndex = locationIndex
        Me.BuildingIndex = buildingIndex
        Me.Building = building
    End Sub
End Class