Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers
Imports QuickQuote.CommonObjects
Imports IFM.VR.Common.Helpers.MultiState.General
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Common.Helpers.CGL


Public Class ctl_CGL_Coverages
    Inherits VRControlBase

    Private ReadOnly Property QuoteISCPP As Boolean
        Get
            ' Determine if this is CPP by looking at the first quote object
            If Quote IsNot Nothing Then
                If Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then Return True Else Return False
            Else
                Return False
            End If
        End Get
    End Property

    Private ReadOnly Property QuoteCanHaveLiquorLiability() As Boolean
        Get
            If ddGeneralAgg.SelectedItem IsNot Nothing Then
                If ddGeneralAgg.SelectedValue = "9" OrElse ddGeneralAgg.SelectedValue = "10" OrElse ddGeneralAgg.SelectedValue = "32" Then
                    Return False
                End If
            End If
            Return True
        End Get
    End Property
    Private ReadOnly Property HasStopGap As Boolean
        Get
            Dim gsQuote As QuickQuoteObject = Me.GoverningStateQuote()
            If gsQuote IsNot Nothing Then
                If gsQuote.StopGapLimitId IsNot Nothing _
                    AndAlso gsQuote.StopGapLimitId.Trim <> "" _
                    AndAlso IsNumeric(gsQuote.StopGapLimitId) Then
                    Return True
                End If
            End If
            Return False
        End Get
    End Property

    ''' <summary>
    ''' Returns a comma delimited list of all of the states on the quote
    ''' </summary>
    ''' <returns></returns>
    Private ReadOnly Property StatesOnQuote As String
        Get
            Dim s As String = ""

            For x = 0 To SubQuotes.Count - 1
                Select Case x
                    Case 0
                        ' First one
                        s += SubQuotes(x).State
                        Exit Select
                    Case Else
                        ' The rest
                        s += "," & SubQuotes(x).State
                End Select
            Next

            Return s
        End Get
    End Property

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(divGenInfo.ClientID, Me.hdnAccordGenInfo, "0")
        Me.VRScript.CreateAccordion(divDeductibles.ClientID, Me.hdnAccordDeductible, "0")
        Me.VRScript.CreateAccordion(divCoverages.ClientID, Me.hdnAccordCoverages, "0")

        Me.VRScript.StopEventPropagation(Me.btnSaveDeductible.ClientID)
        Me.VRScript.StopEventPropagation(Me.btnSaveGenInfo.ClientID)
        Me.VRScript.StopEventPropagation(Me.btnSaveCoverages.ClientID)

        Me.VRScript.CreateTextBoxFormatter(Me.txtEmployeeNumberOfEmployees, ctlPageStartupScript.FormatterType.PositiveNumberWithCommas, ctlPageStartupScript.JsEventType.onkeyup)
        Me.VRScript.CreateTextBoxFormatter(Me.txtEmployeeNumberOfEmployees, ctlPageStartupScript.FormatterType.PositiveWholeNumberWithCommas, ctlPageStartupScript.JsEventType.onblur)

        Me.VRScript.CreateTextBoxFormatter(Me.txtManufacturerLiquorSales_IN, ctlPageStartupScript.FormatterType.Currency, ctlPageStartupScript.JsEventType.onkeyup)
        Me.VRScript.CreateTextBoxFormatter(Me.txtManufacturerLiquorSales_IN, ctlPageStartupScript.FormatterType.CurrencyNoCents, ctlPageStartupScript.JsEventType.onblur)
        Me.VRScript.CreateTextBoxFormatter(Me.txtRestaurantLiquorSales_IN, ctlPageStartupScript.FormatterType.Currency, ctlPageStartupScript.JsEventType.onkeyup)
        Me.VRScript.CreateTextBoxFormatter(Me.txtRestaurantLiquorSales_IN, ctlPageStartupScript.FormatterType.CurrencyNoCents, ctlPageStartupScript.JsEventType.onblur)
        Me.VRScript.CreateTextBoxFormatter(Me.txtPackageStoreLiquorSales_IN, ctlPageStartupScript.FormatterType.Currency, ctlPageStartupScript.JsEventType.onkeyup)
        Me.VRScript.CreateTextBoxFormatter(Me.txtPackageStoreLiquorSales_IN, ctlPageStartupScript.FormatterType.CurrencyNoCents, ctlPageStartupScript.JsEventType.onblur)
        Me.VRScript.CreateTextBoxFormatter(Me.txtClubLiquorSales_IN, ctlPageStartupScript.FormatterType.Currency, ctlPageStartupScript.JsEventType.onkeyup)
        Me.VRScript.CreateTextBoxFormatter(Me.txtClubLiquorSales_IN, ctlPageStartupScript.FormatterType.CurrencyNoCents, ctlPageStartupScript.JsEventType.onblur)

        Me.VRScript.CreateTextBoxFormatter(Me.txtManufacturerLiquorSales_IL, ctlPageStartupScript.FormatterType.Currency, ctlPageStartupScript.JsEventType.onkeyup)
        Me.VRScript.CreateTextBoxFormatter(Me.txtManufacturerLiquorSales_IL, ctlPageStartupScript.FormatterType.CurrencyNoCents, ctlPageStartupScript.JsEventType.onblur)
        Me.VRScript.CreateTextBoxFormatter(Me.txtRestaurantLiquorSales_IL, ctlPageStartupScript.FormatterType.Currency, ctlPageStartupScript.JsEventType.onkeyup)
        Me.VRScript.CreateTextBoxFormatter(Me.txtRestaurantLiquorSales_IL, ctlPageStartupScript.FormatterType.CurrencyNoCents, ctlPageStartupScript.JsEventType.onblur)
        Me.VRScript.CreateTextBoxFormatter(Me.txtPackageStoreLiquorSales_IL, ctlPageStartupScript.FormatterType.Currency, ctlPageStartupScript.JsEventType.onkeyup)
        Me.VRScript.CreateTextBoxFormatter(Me.txtPackageStoreLiquorSales_IL, ctlPageStartupScript.FormatterType.CurrencyNoCents, ctlPageStartupScript.JsEventType.onblur)
        Me.VRScript.CreateTextBoxFormatter(Me.txtClubLiquorSales_IL, ctlPageStartupScript.FormatterType.Currency, ctlPageStartupScript.JsEventType.onkeyup)
        Me.VRScript.CreateTextBoxFormatter(Me.txtClubLiquorSales_IL, ctlPageStartupScript.FormatterType.CurrencyNoCents, ctlPageStartupScript.JsEventType.onblur)

        ' Cache the page controls
        Me.VRScript.AddVariableLine("Cgl.CGLCoveragPageBindings.push(new Cgl.CGLCoverageUiBinding('" &
            Me.ddProgramType.ClientID & "','" &
            Me.ddOccuranceLibLimit.ClientID & "','" &
            Me.ddGeneralAgg.ClientID & "','" &
            Me.txtRented.ClientID & "','" &
            Me.ddOperationsAgg.ClientID & "','" &
            Me.ddMedicalExpense.ClientID & "','" &
            Me.ddPersonalInjury.ClientID & "','" &
            Me.chkGLEnhancement.ClientID & "','" &
            Me.ddlAddlBlanketOfSubroOptions.ClientID & "','" &
            Me.chkContractorsGLEnhancement.ClientID & "','" &
            Me.chkManufacturersGLEnhancement.ClientID & "','" &
            Me.ddlAddAGLDeductible.ClientID & "','" &
            Me.ddType.ClientID & "','" &
            Me.ddAmount.ClientID & "','" &
            Me.ddBasis.ClientID & "','" &
            Me.chkAdditionalInsured.ClientID & "','" &
            Me.ddlNumberOfAddlInsureds.ClientID & "','" &
            Me.chkCondoDAndO.ClientID & "','" &
            Me.txtNamedAssociation.ClientID & "','" &
            Me.txtCondoDAndOLimit.ClientID & "','" &
            Me.ddCondoDAndODeductible.ClientID & "','" &
            Me.chkEmployee.ClientID & "','" &
            Me.txtEmployeeOccurrenceLimit.ClientID & "','" &
            Me.txtEmployeeNumberOfEmployees.ClientID & "','" &
            Me.chkCLI.ClientID & "','" &
            Me.chkEPLI.ClientID & "','" &
            Me.chkHired.ClientID & "','" &
            Me.chkLiquor_IN.ClientID & "','" &
            Me.txtLiquorOccurrenceLimit_IN.ClientID & "','" &
            Me.chkLiquor_IL.ClientID & "','" &
            Me.txtLiquorLiabilityLimit_IL.ClientID & "','" &
            Me.chkContractorsHomeRepairAndRemodeling_IL.ClientID & "','" &
            Me.trProgramTypeRow.ClientID & "','" &
            Me.trAddGLBlanketWaiverOfSubroRow.ClientID & "','" &
            Me.trContractorsGLEnhancementRow.ClientID & "','" &
            Me.trManufacturersGLEnhancement.ClientID & "','" &
            Me.trAdditionalInsuredsRow.ClientID & "','" &
            Me.trCondoDAndORow.ClientID & "','" &
            Me.trCondoDAndODataRow1.ClientID & "','" &
            Me.trCondoDAndODataRow2.ClientID & "','" &
            Me.trCondoDAndODataRow3.ClientID & "','" &
            Me.trEmployeeBenefitsLiabilityDataRow1.ClientID & "','" &
            Me.trEmployeeBenefitsLiabilityDataRow2.ClientID & "','" &
            Me.trEmployeeBenefitsLiabilityInfoRow.ClientID & "','" &
            Me.trCLIInfoRow1.ClientID & "','" &
            Me.trCLIInfoRow2.ClientID & "','" &
            Me.trEPLIInfoRow1.ClientID & "','" &
            Me.trEPLIInfoRow2.ClientID & "','" &
            Me.trLiquorLiabilityCheckBoxRow_IN.ClientID & "','" &
            Me.trLiquorInfoRow_IN.ClientID & "','" &
            Me.trLiquorDataRow1_IN.ClientID & "','" &
            Me.trLiquorInfoRow2_IN.ClientID & "','" &
            Me.trLiquorLiabilityCheckboxRow_IL.ClientID & "','" &
            Me.trLiquorInfoRow_IL.ClientID & "','" &
            Me.trLiquorInfoRow2_IL.ClientID & "','" &
            Me.trLiquorDataRow1_IL.ClientID & "','" &
            Me.trLiquorInfoRow3_IL.ClientID & "','" &
            Me.trContractorsHomeRepairAndRemodelingRow.ClientID & "','" &
            Me.trContractorsHomeRepairAndRemodelingInfoRow.ClientID & "','" &
            Me.chkManufacturerLiquorSalesTableRow_IN.ClientID & "','" &
            Me.chkRestaurantLiquorSalesTableRow_IN.ClientID & "','" &
            Me.chkPackageStoreLiquorSalesTableRow_IN.ClientID & "','" &
            Me.chkClubLiquorSalesTableRow_IN.ClientID & "','" &
            Me.chkManufacturerLiquorSales_IN.ClientID & "','" &
            Me.chkRestaurantLiquorSales_IN.ClientID & "','" &
            Me.chkPackageStoreLiquorSales_IN.ClientID & "','" &
            Me.chkClubLiquorSales_IN.ClientID & "','" &
            Me.txtManufacturerLiquorSalesTableRow_IN.ClientID & "','" &
            Me.txtRestaurantLiquorSalesTableRow_IN.ClientID & "','" &
            Me.txtPackageStoreLiquorSalesTableRow_IN.ClientID & "','" &
            Me.txtClubLiquorSalesTableRow_IN.ClientID & "','" &
            Me.txtManufacturerLiquorSales_IN.ClientID & "','" &
            Me.txtRestaurantLiquorSales_IN.ClientID & "','" &
            Me.txtPackageStoreLiquorSales_IN.ClientID & "','" &
            Me.txtClubLiquorSales_IN.ClientID & "','" &
            Me.chkManufacturerLiquorSalesTableRow_IL.ClientID & "','" &
            Me.chkRestaurantLiquorSalesTableRow_IL.ClientID & "','" &
            Me.chkPackageStoreLiquorSalesTableRow_IL.ClientID & "','" &
            Me.chkClubLiquorSalesTableRow_IL.ClientID & "','" &
            Me.chkManufacturerLiquorSales_IL.ClientID & "','" &
            Me.chkRestaurantLiquorSales_IL.ClientID & "','" &
            Me.chkPackageStoreLiquorSales_IL.ClientID & "','" &
            Me.chkClubLiquorSales_IL.ClientID & "','" &
            Me.txtManufacturerLiquorSalesTableRow_IL.ClientID & "','" &
            Me.txtRestaurantLiquorSalesTableRow_IL.ClientID & "','" &
            Me.txtPackageStoreLiquorSalesTableRow_IL.ClientID & "','" &
            Me.txtClubLiquorSalesTableRow_IL.ClientID & "','" &
            Me.txtManufacturerLiquorSales_IL.ClientID & "','" &
            Me.txtRestaurantLiquorSales_IL.ClientID & "','" &
            Me.txtPackageStoreLiquorSales_IL.ClientID & "','" &
            Me.txtClubLiquorSales_IL.ClientID &
            "'));")

        Me.VRScript.CreateJSBinding(Me.chkGLEnhancement, ctlPageStartupScript.JsEventType.onchange, "Cgl.EnhancementCheckboxChanged('" & chkGLEnhancement.ClientID & "','" & trAddGLBlanketWaiverOfSubroRow.ClientID & "','" & txtRented.ClientID & "','" & ddMedicalExpense.ClientID & "','" & chkGLPlusEnhancement.ClientID & "');")

        Me.VRScript.CreateJSBinding(Me.chkGLPlusEnhancement, ctlPageStartupScript.JsEventType.onchange, "Cgl.PlusEnhancementCheckboxChanged('" & chkGLPlusEnhancement.ClientID & "','" & chkGLEnhancement.ClientID & "','" & ddMedicalExpense.ClientID & "');")

        Me.VRScript.CreateJSBinding(Me.ddMedicalExpense, ctlPageStartupScript.JsEventType.onchange, "Cgl.MedicalExpenseChanged('" & ddMedicalExpense.ClientID & "', '" & chkGLPlusEnhancement.ClientID & "','" & chkGLEnhancement.ClientID & "', '" & chkContractorsGLEnhancement.ClientID & "','" & chkManufacturersGLEnhancement.ClientID & "');")

        Me.VRScript.CreateJSBinding(Me.ddlAddlBlanketOfSubroOptions, ctlPageStartupScript.JsEventType.onchange, "Cgl.BlanketWaiverOfSubroChanged('" & ddlAddlBlanketOfSubroOptions.ClientID & "');")
        Me.VRScript.CreateJSBinding(Me.ddlAddAGLDeductible, ctlPageStartupScript.JsEventType.onchange, "Cgl.GLLiabilityDeductibleChanged('" & ddlAddAGLDeductible.ClientID & "','" & divDeductibles.ClientID & "');")
        'Me.VRScript.CreateJSBinding(Me.chkAddaGeneralLiabilityDeductible, ctlPageStartupScript.JsEventType.onchange, "Cgl.GLLiabilityDeductibleChanged('" & chkAddaGeneralLiabilityDeductible.ClientID & "','" & divDeductibles.ClientID & "');")
        'Me.VRScript.CreateJSBinding(Me.ddOccuranceLibLimit, ctlPageStartupScript.JsEventType.onchange, "Cgl.OccurenceLiabilityLimitChanged('" & ddOccuranceLibLimit.ClientID & "','" & ddGeneralAgg.ClientID & "','" & ddOperationsAgg.ClientID & "','" & ddPersonalInjury.ClientID & "','" & txtLiquorOccurrenceLimit.ClientID & "','" & txtEmployeeOccurrenceLimit.ClientID & "');")
        'Me.VRScript.CreateJSBinding(Me.ddOccuranceLibLimit, ctlPageStartupScript.JsEventType.onchange, "Cgl.OccurenceLiabilityLimitChanged('" & ddOccuranceLibLimit.ClientID & "','" & ddGeneralAgg.ClientID & "','" & ddOperationsAgg.ClientID & "','" & ddPersonalInjury.ClientID & "','" & txtLiquorOccurrenceLimit_IN.ClientID & "','" & txtEmployeeOccurrenceLimit.ClientID & "','" & txtLiquorLiabilityLimit_IL.ClientID & "');")

        ' Use new script for OLL
        Dim isNewCoFlag As String = IsNewCo().ToString
        If IsNewCo() Then
            Me.VRScript.CreateJSBinding(Me.ddOccuranceLibLimit, ctlPageStartupScript.JsEventType.onchange, "Cgl.OccurrenceLiabilityLimitChanged('" & StatesOnQuote & "','" & isNewCoFlag & "');", True)
            Me.VRScript.CreateJSBinding(Me.ddGeneralAgg, ctlPageStartupScript.JsEventType.onchange, "Cgl.GeneralAggregateChanged('" & isNewCoFlag & "');")
        Else
            Me.VRScript.CreateJSBinding(Me.ddOccuranceLibLimit, ctlPageStartupScript.JsEventType.onchange, "Cgl.OccurrenceLiabilityLimitChanged('" & StatesOnQuote & "','" & isNewCoFlag & "');")
        End If
        Dim isNewCyberCovNameAvail As Boolean = CPP.CyberCoverageHelper.IsCyberCoverageAvailable(Quote)
        'Me.VRScript.CreateJSBinding(Me.chkAdditionalInsured, ctlPageStartupScript.JsEventType.onchange, "Cgl.CoverageCheckboxChanged('AI','" & chkAdditionalInsured.ClientID & "','" & trAdditionalInsuredsRow.ClientID & "','','','" & trAddlInsuredsInfoRow.ClientID & "','');")
        Me.VRScript.CreateJSBinding(Me.chkAdditionalInsured, ctlPageStartupScript.JsEventType.onchange, "Cgl.CoverageCheckboxChanged('AI','" & chkAdditionalInsured.ClientID & "','" & trAdditionalInsuredsRow.ClientID & "','','','','','','');")
        Me.VRScript.CreateJSBinding(Me.chkEmployee, ctlPageStartupScript.JsEventType.onchange, "Cgl.CoverageCheckboxChanged('EBL','" & chkEmployee.ClientID & "','" & trEmployeeBenefitsLiabilityDataRow1.ClientID & "','" & trEmployeeBenefitsLiabilityDataRow2.ClientID & "','','" & trEmployeeBenefitsLiabilityInfoRow.ClientID & "','','','');")
        Me.VRScript.CreateJSBinding(Me.chkCLI, ctlPageStartupScript.JsEventType.onchange, "Cgl.CoverageCheckboxChanged('CLI','" & chkCLI.ClientID & "','','','','" & trCLIInfoRow1.ClientID & "','" & trCLIInfoRow2.ClientID & "','','" & isNewCyberCovNameAvail & "');")
        Me.VRScript.CreateJSBinding(Me.chkEPLI, ctlPageStartupScript.JsEventType.onchange, "Cgl.CoverageCheckboxChanged('EPLI','" & chkEPLI.ClientID & "','','','','" & trEPLIInfoRow1.ClientID & "','" & trEPLIInfoRow2.ClientID & "','','');")
        Me.VRScript.CreateJSBinding(Me.chkLiquor_IN, ctlPageStartupScript.JsEventType.onchange, "Cgl.CoverageCheckboxChanged('LIQ_IN','" & chkLiquor_IN.ClientID & "','" & trLiquorDataRow1_IN.ClientID & "','','','','" & trLiquorInfoRow_IN.ClientID & "','" & trLiquorInfoRow2_IN.ClientID & "','','');")
        Me.VRScript.CreateJSBinding(Me.chkLiquor_IL, ctlPageStartupScript.JsEventType.onchange, "Cgl.CoverageCheckboxChanged('LIQ_IL','" & chkLiquor_IL.ClientID & "','" & trLiquorDataRow1_IL.ClientID & "','','','" & trLiquorInfoRow_IL.ClientID & "','" & trLiquorInfoRow2_IL.ClientID & "','" & trLiquorInfoRow3_IL.ClientID & "','');")
        Me.VRScript.CreateJSBinding(Me.chkHired, ctlPageStartupScript.JsEventType.onchange, "Cgl.CoverageCheckboxChanged('HNO','" & chkHired.ClientID & "','','','','','','','');")

        Me.VRScript.CreateJSBinding(Me.chkManufacturerLiquorSales_IN, ctlPageStartupScript.JsEventType.onchange, "Cgl.CoverageCheckboxChanged('MAN','" & chkManufacturerLiquorSales_IN.ClientID & "','" & txtManufacturerLiquorSalesTableRow_IN.ClientID & "','','','','','','');")
        Me.VRScript.CreateJSBinding(Me.chkRestaurantLiquorSales_IN, ctlPageStartupScript.JsEventType.onchange, "Cgl.CoverageCheckboxChanged('RES','" & chkRestaurantLiquorSales_IN.ClientID & "','" & txtRestaurantLiquorSalesTableRow_IN.ClientID & "','','','','','','');")
        Me.VRScript.CreateJSBinding(Me.chkPackageStoreLiquorSales_IN, ctlPageStartupScript.JsEventType.onchange, "Cgl.CoverageCheckboxChanged('PAC','" & chkPackageStoreLiquorSales_IN.ClientID & "','" & txtPackageStoreLiquorSalesTableRow_IN.ClientID & "','','','','','','');")
        Me.VRScript.CreateJSBinding(Me.chkClubLiquorSales_IN, ctlPageStartupScript.JsEventType.onchange, "Cgl.CoverageCheckboxChanged('CLU','" & chkClubLiquorSales_IN.ClientID & "','" & txtClubLiquorSalesTableRow_IN.ClientID & "','','','','','','');")

        Me.VRScript.CreateJSBinding(Me.chkManufacturerLiquorSales_IL, ctlPageStartupScript.JsEventType.onchange, "Cgl.CoverageCheckboxChanged('MAN','" & chkManufacturerLiquorSales_IL.ClientID & "','" & txtManufacturerLiquorSalesTableRow_IL.ClientID & "','','','','','','');")
        Me.VRScript.CreateJSBinding(Me.chkRestaurantLiquorSales_IL, ctlPageStartupScript.JsEventType.onchange, "Cgl.CoverageCheckboxChanged('RES','" & chkRestaurantLiquorSales_IL.ClientID & "','" & txtRestaurantLiquorSalesTableRow_IL.ClientID & "','','','','','','');")
        Me.VRScript.CreateJSBinding(Me.chkPackageStoreLiquorSales_IL, ctlPageStartupScript.JsEventType.onchange, "Cgl.CoverageCheckboxChanged('PAC','" & chkPackageStoreLiquorSales_IL.ClientID & "','" & txtPackageStoreLiquorSalesTableRow_IL.ClientID & "','','','','','','');")
        Me.VRScript.CreateJSBinding(Me.chkClubLiquorSales_IL, ctlPageStartupScript.JsEventType.onchange, "Cgl.CoverageCheckboxChanged('CLU','" & chkClubLiquorSales_IL.ClientID & "','" & txtClubLiquorSalesTableRow_IL.ClientID & "','','','','','','');")

        ' CPP - Condo D&O
        'Updated 09/02/2021 for bug 51550 MLW
        'Me.VRScript.CreateJSBinding(Me.chkCondoDAndO, ctlPageStartupScript.JsEventType.onchange, "Cgl.CoverageCheckboxChanged('CDO','" & chkCondoDAndO.ClientID & "','" & trCondoDAndODataRow1.ClientID & "','" & trCondoDAndODataRow2.ClientID & "','" & trCondoDAndODataRow3.ClientID & "','','');")
        Me.VRScript.CreateJSBinding(Me.chkCondoDAndO, ctlPageStartupScript.JsEventType.onchange, "Cgl.CoverageCheckboxChanged('CDO','" & chkCondoDAndO.ClientID & "','" & trCondoDAndODataRow1.ClientID & "','" & trCondoDAndODataRow2.ClientID & "','" & trCondoDAndODataRow3.ClientID & "','','','','');Cgl.ToggleOccurrenceLiabLimit('" & chkCondoDAndO.ClientID & "','" & ddOccuranceLibLimit.ClientID & "');")

        ' Illinois Contractors
        Me.VRScript.CreateJSBinding(chkContractorsHomeRepairAndRemodeling_IL, ctlPageStartupScript.JsEventType.onchange, "Cgl.ILContractorsHomeRepairCheckboxChanged('" & chkContractorsHomeRepairAndRemodeling_IL.ClientID & "','" & trContractorsHomeRepairAndRemodelingInfoRow.ClientID & "');")

        ' Handle stop gap checkbox clicks
        Me.VRScript.CreateJSBinding(chkStopGap, ctlPageStartupScript.JsEventType.onclick, "Cgl.HandleStopGapClicks('" & chkStopGap.ClientID & "','" & trStopGapDataRow.ClientID & "');")

        ' Handles changes to the Policy Type dropdown Preferred Option for CGL 2/4/21 
        If Me.Quote.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability Then
            Me.ddProgramType.SelectedValue = "54"
            Me.ddProgramType.CssClass = "CGL_GI_DataColumn"
            Dim preferredCGLPopupMessage As String = "<div><b>To qualify for the preferred program, the risk must have the following criteria-</b></div><div>"
            preferredCGLPopupMessage = preferredCGLPopupMessage & "• 3 year policy loss ratio of 55% or less.<br />"
            preferredCGLPopupMessage = preferredCGLPopupMessage & "• Building age of 25 years or less.<br />"
            preferredCGLPopupMessage = preferredCGLPopupMessage & "• Building age more than 25 years of age that have had major upgrades to roof, hvac, plumbing within the past 10 years and shown to be in better than average condition based on photos and/or loss control.<br />"
            preferredCGLPopupMessage = preferredCGLPopupMessage & "• Business is managed by an experienced insured (minimum of 3 years of experience) in the same business.<br />"
            preferredCGLPopupMessage = preferredCGLPopupMessage & "• Insured location has additional property safeguards than the average risk such as automatic sprinkler systems, central station alarms, fenced or otherwise better protected from losses.<br />"
            preferredCGLPopupMessage = preferredCGLPopupMessage & "• Above average maintenance and housekeeping verified by agent, loss control inspection or other reliable source.<br />"
            preferredCGLPopupMessage = preferredCGLPopupMessage & "• Property located in Protection Class 7 or better.<br />"
            preferredCGLPopupMessage = preferredCGLPopupMessage & "• Exposure with Risk Grade 1 or 2.<br />"
            preferredCGLPopupMessage = preferredCGLPopupMessage & "•Risk has a formal safety program in place as confirmed by agent or loss control.<br />"
            preferredCGLPopupMessage = preferredCGLPopupMessage & "</div>"
            'updated 2/2/21 
            Using popupSpecial As New PopupMessageClass.PopupMessageObject(Me.Page, preferredCGLPopupMessage, "Preferred Rating Program Guidelines")
                With popupSpecial
                    .ControlEvent = PopupMessageClass.PopupMessageObject.ControlEvents.onchange
                    .DropDownValueToBindTo = "55"
                    .BindScript = PopupMessageClass.PopupMessageObject.BindTo.Control
                    .isModal = False
                    .AddButton("OK", True)
                    .width = 550
                    .height = 500
                    .AddControlToBindTo(ddProgramType)
                    .divId = "ddProgramTypePopup"
                    .CreateDynamicPopUpWindow()
                End With
            End Using
        End If
        Exit Sub
    End Sub

    Public Overrides Sub LoadStaticData()
        'If ddProgramType.Items Is Nothing OrElse ddProgramType.Items.Count <= 0 Then
        ' Program
        QQHelper.LoadStaticDataOptionsDropDown(ddProgramType, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.ProgramTypeId, , Quote.LobType)
        If IsNewCo() Then
            'Occurrence Liability Limit
            QQHelper.LoadStaticDataOptionsDropDownForStateAndCompany(ddOccuranceLibLimit, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.OccurrenceLiabilityLimitId, Quote.QuickQuoteState, Quote.Company, QuickQuote.CommonObjects.QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
            ' General Aggregate
            QQHelper.LoadStaticDataOptionsDropDownForStateAndCompany(ddGeneralAgg, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.GeneralAggregateLimitId, Quote.QuickQuoteState, Quote.Company, QuickQuote.CommonObjects.QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)
            ' Product/Completed Ops Aggregate
            QQHelper.LoadStaticDataOptionsDropDownForStateAndCompany(ddOperationsAgg, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.ProductsCompletedOperationsAggregateLimitId, Quote.QuickQuoteState, Quote.Company, QuickQuote.CommonObjects.QuickQuoteStaticDataOption.SortBy.None, Quote.LobType)
            ' Personal and Advertising Injury
            QQHelper.LoadStaticDataOptionsDropDownForStateAndCompany(ddPersonalInjury, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.PersonalAndAdvertisingInjuryLimitId, Quote.QuickQuoteState, Quote.Company, QuickQuote.CommonObjects.QuickQuoteStaticDataOption.SortBy.None, Quote.LobType)
        Else
            'Occurrence Liability Limit
            QQHelper.LoadStaticDataOptionsDropDown(ddOccuranceLibLimit, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.OccurrenceLiabilityLimitId, , Quote.LobType)
            ' General Aggregate
            QQHelper.LoadStaticDataOptionsDropDown(ddGeneralAgg, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.GeneralAggregateLimitId, , Quote.LobType)
            ' Product/Completed Ops Aggregate
            QQHelper.LoadStaticDataOptionsDropDown(ddOperationsAgg, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.ProductsCompletedOperationsAggregateLimitId, , Quote.LobType)
            ' Personal and Advertising Injury
            QQHelper.LoadStaticDataOptionsDropDown(ddPersonalInjury, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.PersonalAndAdvertisingInjuryLimitId, , Quote.LobType)
        End If
        ' Medical Expenses
        QQHelper.LoadStaticDataOptionsDropDown(ddMedicalExpense, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.MedicalExpensesLimitId, , Quote.LobType)
        ' Personal and Advertising Injury
        QQHelper.LoadStaticDataOptionsDropDown(ddPersonalInjury, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.PersonalAndAdvertisingInjuryLimitId, , Quote.LobType)
        ' Deductible Type
        QQHelper.LoadStaticDataOptionsDropDown(ddType, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.GL_PremisesAndProducts_DeductibleCategoryTypeId, , Quote.LobType)
        ' Deductible Amount
        QQHelper.LoadStaticDataOptionsDropDown(ddAmount, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.GL_PremisesAndProducts_DeductibleId, , Quote.LobType)
        ' Deductible Per Type (basis)
        QQHelper.LoadStaticDataOptionsDropDown(ddBasis, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.GL_PremisesAndProducts_DeductiblePerTypeId, , Quote.LobType)

        ' CPP
        ' Condo D&O Deductible
        If QuoteISCPP Then
            QQHelper.LoadStaticDataOptionsDropDown(ddCondoDAndODeductible, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.CondoDandODeductibleId, , Quote.LobType)
        End If

        ' Stop Gap Limit
        QQHelper.LoadStaticDataOptionsDropDown(ddStopGapLimit, QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.StopGapLimitId, QuickQuoteStaticDataOption.SortBy.None, Me.Quote.LobType)


        ' End If
        CGL.GenAggProducts3MHelper.RemoveDropDownOptionIfNecessary(Quote, ddGeneralAgg)
        CGL.GenAggProducts3MHelper.RemoveDropDownOptionIfNecessary(Quote, ddOperationsAgg)
    End Sub

    Public Sub EffectiveDateChanging(ByVal NewDate As String, ByVal OldDate As String)
        'LoadStaticData() commented out for bug 68069

        ' Check for Liquor Liability layout change
        If IsDate(NewDate) AndAlso SubQuotesContainsState("IL") AndAlso QuoteCanHaveLiquorLiability() AndAlso chkLiquor_IL.Checked Then
            txtLiquorLiabilityLimit_IL.Text = ""
            If ddOccuranceLibLimit.SelectedIndex >= 0 Then
                If CDate(NewDate) < CDate("4/1/2020") Then
                    txtLiquorLiabilityLimit_IL.Width = Unit.Percentage(40)
                    Select Case ddOccuranceLibLimit.SelectedValue
                        Case "33" ' 300,000
                            txtLiquorLiabilityLimit_IL.Text = "STATE STATUTORY LIMITS WILL APPLY"
                            Exit Select
                        Case "34" ' 500,000
                            txtLiquorLiabilityLimit_IL.Text = "STATE STATUTORY LIMITS WILL APPLY"
                            Exit Select
                        Case "56" ' 1,000,000
                            txtLiquorLiabilityLimit_IL.Text = "STATE STATUTORY LIMITS WILL APPLY"
                            Exit Select
                        Case Else
                            ' Default - if we got here something is wrong
                            txtLiquorLiabilityLimit_IL.Text = ""
                            Exit Select
                    End Select
                Else
                    ' New IL Liquor Liability Limit Logic per Task 40486 MGB 3/9/2020
                    txtLiquorLiabilityLimit_IL.Width = Unit.Percentage(75)
                    txtLiquorLiabilityLimit_IL.Text = "STATE STATUTORY LIMITS WILL APPLY"
                End If
            End If
        End If ' If QuoteCanHaveLiquorLiability

        ' Format Ohio Stop Gap section
        '''TODO: evaluate how we should remove  this; create task
        If CDate(NewDate) >= IFM.VR.Common.Helpers.GenericHelper.GetOhioEffectiveDate() Then
            Dim gsQuote As QuickQuoteObject = Me.GoverningStateQuote()
            trStopGapCheckboxRow.Attributes.Add("style", "display:'';")
            If HasStopGap() Then
                chkStopGap.Checked = True
                trStopGapDataRow.Attributes.Add("style", "display:'';width:100%;text-indent:15px;")
                If gsQuote.StopGapLimitId IsNot Nothing AndAlso IsNumeric(gsQuote.StopGapLimitId) Then
                    ddStopGapLimit.SetFromValue(gsQuote.StopGapLimitId)
                Else
                    ddStopGapLimit.SetFromValue("0")
                End If
                txtStopGapPayroll.Text = ""
                If gsQuote.StopGapPayroll IsNot Nothing AndAlso IsNumeric(gsQuote.StopGapPayroll) Then
                    txtStopGapPayroll.Text = Format(CDec(gsQuote.StopGapPayroll), "#########")
                End If
            Else
                ' OH effective, but doesn't have stop gap
                chkStopGap.Checked = False
                trStopGapDataRow.Attributes.Add("style", "display:none;")
                txtStopGapPayroll.Text = ""
                ddStopGapLimit.SelectedIndex = -1
            End If
        Else
            ' OH not effective
            trStopGapCheckboxRow.Attributes.Add("style", "display:none;")
            trStopGapDataRow.Attributes.Add("style", "display:none;")
            chkStopGap.Checked = False
            txtStopGapPayroll.Text = ""
            ddStopGapLimit.SelectedIndex = -1
            IFM.VR.Common.Helpers.MultiState.General.RemoveOHStopGapCoverageFromSubquotes(SubQuotes)
        End If

        PopulateCyberCoverage()

        'Populate()
        Exit Sub
    End Sub

    Public Overrides Sub Populate()
        Dim IndianaQuote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing
        Dim IllinoisQuote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing
        Dim OhioQuote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing

        LoadStaticData()

        'Updated 12/08/2021 for CPP Endorsements Task 66660 MLW
        If Not IsQuoteReadOnly() Then
            ' SET THE DEFAULTS
            ' Program: CGL Standard
            If Not QuoteISCPP Then ddProgramType.SelectedIndex = 0
            ' OLL: 1,000,000
            ddOccuranceLibLimit.SelectedValue = "56"
            ' Gen Agg: 2,000,000
            ddGeneralAgg.SelectedValue = "65"
            ' Product Agg: 2,000,000
            ddOperationsAgg.SelectedValue = "65"
            ' Pers Adv Inj: 1,000,000
            ddPersonalInjury.SelectedValue = "56"
            ' These two are always the set values below
            Me.txtRented.Text = "100,000"
            Me.ddMedicalExpense.SelectedValue = "15"
            ' Deductible Type: Property Damage
            Me.ddType.SelectedValue = "6"
            ' Deductible Amount: 500
            Me.ddAmount.SelectedValue = "8"
            ' Deductible Basis Type: Per Occurrence
            Me.ddBasis.SelectedValue = "1"
            ' Add a GL Deductible - No
            ddlAddAGLDeductible.SelectedValue = 0
            ' Condo D&O deductible
        End If

        ' HIDE ALL THE COVERAGE DETAILS
        trProgramTypeRow.Attributes.Add("style", "display:none")
        trAdditionalInsuredsRow.Attributes.Add("style", "display:none")
        'trAddlInsuredsInfoRow.Attributes.Add("style", "display:none")
        trEmployeeBenefitsLiabilityDataRow1.Attributes.Add("style", "display:none")
        trEmployeeBenefitsLiabilityDataRow2.Attributes.Add("style", "display:none")
        trEmployeeBenefitsLiabilityInfoRow.Attributes.Add("style", "display:none")
        trCLIInfoRow1.Attributes.Add("style", "display:none")
        trCLIInfoRow2.Attributes.Add("style", "display:none")
        trEPLIInfoRow1.Attributes.Add("style", "display:none")
        trEPLIInfoRow2.Attributes.Add("style", "display:none")
        trLiquorLiabilityCheckBoxRow_IN.Attributes.Add("style", "display:none")
        trLiquorDataRow1_IN.Attributes.Add("style", "display:none")
        trLiquorInfoRow_IN.Attributes.Add("style", "display:none")
        trLiquorInfoRow2_IN.Attributes.Add("style", "display:none")
        trCondoDAndORow.Attributes.Add("style", "display:none")
        trCondoDAndODataRow1.Attributes.Add("style", "display:none")
        trCondoDAndODataRow2.Attributes.Add("style", "display:none")
        trCondoDAndODataRow3.Attributes.Add("style", "display:none")
        trContractorsHomeRepairAndRemodelingRow.Attributes.Add("style", "display:none")
        trContractorsHomeRepairAndRemodelingInfoRow.Attributes.Add("style", "display:none")
        trLiquorLiabilityCheckboxRow_IL.Attributes.Add("style", "display:none")
        trLiquorDataRow1_IL.Attributes.Add("style", "display:none")
        trLiquorInfoRow_IL.Attributes.Add("style", "display:none")
        trLiquorInfoRow2_IL.Attributes.Add("style", "display:none")
        trLiquorInfoRow3_IL.Attributes.Add("style", "display:none")

        ' POPULATE
        If Me.Quote IsNot Nothing Then
            If Me.SubQuoteFirst IsNot Nothing Then

                If Not QuoteISCPP Then
                    ' Only show Program Type for CGL
                    trProgramTypeRow.Attributes.Add("style", "display:''")
                    Me.ddProgramType.SetFromValue(Me.SubQuoteFirst.ProgramTypeId)
                End If

                If IsNullEmptyorWhitespace(Me.SubQuoteFirst.GeneralAggregateLimitId) = False Then
                    WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddGeneralAgg, Me.SubQuoteFirst.GeneralAggregateLimitId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.GeneralAggregateLimitId)
                End If

                If IsNullEmptyorWhitespace(Me.SubQuoteFirst.ProductsCompletedOperationsAggregateLimitId) Then
                    Me.ddOperationsAgg.SetFromValue(Me.SubQuoteFirst.ProductsCompletedOperationsAggregateLimitId, "65")
                Else
                    WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddOperationsAgg, Me.SubQuoteFirst.ProductsCompletedOperationsAggregateLimitId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.ProductsCompletedOperationsAggregateLimitId)
                End If

                If IsNullEmptyorWhitespace(Me.SubQuoteFirst.PersonalAndAdvertisingInjuryLimitId) Then
                    Me.ddPersonalInjury.SetFromValue(Me.SubQuoteFirst.PersonalAndAdvertisingInjuryLimitId, "56")
                Else
                    WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddPersonalInjury, Me.SubQuoteFirst.PersonalAndAdvertisingInjuryLimitId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.PersonalAndAdvertisingInjuryLimitId)
                End If

                If IsNullEmptyorWhitespace(Me.SubQuoteFirst.OccurrenceLiabilityLimitId) = False Then
                    WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddOccuranceLibLimit, Me.SubQuoteFirst.OccurrenceLiabilityLimitId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.OccurrenceLiabilityLimitId)
                End If



                'Added 12/07/2021 for CPP Endorsements Task 66660 MLW
                If IsQuoteReadOnly() Then
                    Me.txtRented.Text = Me.SubQuoteFirst.DamageToPremisesRentedLimit
                End If

                If CGL.CGL_GenLiabPlusEnhancementEndorsement.IsGlPlusEnhancementAvailable(Quote) Then
                    trGLPlusEnhancementRow.Attributes.Add("style", "display:''")
                Else
                    trGLPlusEnhancementRow.Attributes.Add("style", "display:none")
                    chkGLPlusEnhancement.Checked = False
                End If

                If QuoteISCPP Then
                    ' Enhancements CPP
                    ' For Package Type of "Apartment", disable the General Liability Enhancement (only applies to CPP)
                    ' Task 52991 MGB 5/17/21
                    If SubQuoteFirst.PackageModificationAssignmentTypeId IsNot Nothing AndAlso SubQuoteFirst.PackageModificationAssignmentTypeId.Trim <> "" Then
                        If SubQuoteFirst.PackageModificationAssignmentTypeId = "1" Then
                            chkGLEnhancement.Enabled = False
                            Me.chkGLEnhancement.Checked = False
                            chkGLPlusEnhancement.Enabled = False
                            Me.chkGLPlusEnhancement.Checked = False
                        Else
                            chkGLEnhancement.Enabled = True
                            Me.chkGLEnhancement.Checked = Me.SubQuoteFirst.Has_PackageGL_EnhancementEndorsement
                            chkGLPlusEnhancement.Enabled = True
                            Me.chkGLPlusEnhancement.Checked = Me.SubQuoteFirst.Has_PackageGL_PlusEnhancementEndorsement
                        End If
                    Else
                        ' Pkg mod type is nothing or empty
                        chkGLEnhancement.Enabled = True
                        Me.chkGLEnhancement.Checked = Me.SubQuoteFirst.Has_PackageGL_EnhancementEndorsement
                        chkGLPlusEnhancement.Enabled = True
                        Me.chkGLPlusEnhancement.Checked = Me.SubQuoteFirst.Has_PackageGL_PlusEnhancementEndorsement
                    End If

                    ' You can only have one of the enhancements selected
                    ' These checkboxes are always disabled - you must change the values on the CPR coverages page
                    chkContractorsGLEnhancement.Enabled = False
                    chkManufacturersGLEnhancement.Enabled = False
                    Me.chkContractorsGLEnhancement.Checked = Me.SubQuoteFirst.HasContractorsEnhancement
                    Me.chkManufacturersGLEnhancement.Checked = Me.SubQuoteFirst.HasManufacturersEnhancement
                    If chkGLEnhancement.Checked Then
                        chkContractorsGLEnhancement.Checked = False
                        chkManufacturersGLEnhancement.Checked = False
                        chkGLPlusEnhancement.Checked = False
                        chkGLPlusEnhancement.Enabled = False
                    ElseIf chkGLPlusEnhancement.Checked Then
                        chkGLEnhancement.Checked = False
                        chkGLEnhancement.Enabled = False
                        chkContractorsGLEnhancement.Checked = False
                        chkManufacturersGLEnhancement.Checked = False
                    ElseIf chkContractorsGLEnhancement.Checked Then
                        chkGLEnhancement.Checked = False
                        chkGLEnhancement.Enabled = False
                        chkManufacturersGLEnhancement.Checked = False
                        chkGLPlusEnhancement.Checked = False
                        chkGLPlusEnhancement.Enabled = False
                    ElseIf chkManufacturersGLEnhancement.Checked Then
                        chkGLEnhancement.Checked = False
                        chkGLEnhancement.Enabled = False
                        chkContractorsGLEnhancement.Checked = False
                        chkGLPlusEnhancement.Checked = False
                        chkGLPlusEnhancement.Enabled = False
                    End If
                Else
                    ' Enhancement CGL
                    If Me.SubQuoteFirst.HasBusinessMasterEnhancement Then
                        Me.chkGLEnhancement.Checked = Me.SubQuoteFirst.HasBusinessMasterEnhancement
                    ElseIf Me.SubQuoteFirst.Has_PackageGL_PlusEnhancementEndorsement Then
                        Me.chkGLPlusEnhancement.Checked = Me.SubQuoteFirst.Has_PackageGL_PlusEnhancementEndorsement
                    End If

                End If

                If chkGLEnhancement.Checked Then
                    trAddGLBlanketWaiverOfSubroRow.Attributes.Add("style", "display:''")
                Else
                    trAddGLBlanketWaiverOfSubroRow.Attributes.Add("style", "display:none")
                End If

                ' CPP - Contractors & Manufacturers Enhancements
                If QuoteISCPP Then
                    trContractorsGLEnhancementRow.Attributes.Add("style", "display:''")
                    chkContractorsGLEnhancement.Checked = SubQuoteFirst.HasContractorsEnhancement
                    trManufacturersGLEnhancement.Attributes.Add("style", "display:''")
                    chkManufacturersGLEnhancement.Checked = SubQuoteFirst.HasManufacturersEnhancement
                End If

                PopulateMedicalExpenses(Quote)
                PopulateGLEnhancementEndorsementCheckboxes(Quote)

                Me.ddlAddlBlanketOfSubroOptions.SetFromValue(Me.SubQuoteFirst.BlanketWaiverOfSubrogation) 'Look to Bug 4040 for additional information

                If Me.SubQuoteFirst.GL_PremisesAndProducts_DeductibleCategoryTypeId <> "" Then
                    ddlAddAGLDeductible.SelectedValue = "1"
                    divDeductibles.Attributes.Add("style", "display:''")
                Else
                    ddlAddAGLDeductible.SelectedValue = "0"
                    divDeductibles.Attributes.Add("style", "display:none")
                End If

                Me.ddType.SetFromValue(Me.SubQuoteFirst.GL_PremisesAndProducts_DeductibleCategoryTypeId, "6")
                Me.ddAmount.SetFromValue(Me.SubQuoteFirst.GL_PremisesAndProducts_DeductibleId, "8")
                Me.ddBasis.SetFromValue(Me.SubQuoteFirst.GL_PremisesAndProducts_DeductiblePerTypeId, "1")

                ' Don't uncheck the additonal insured checkbox if already checked!
                'If Not chkAdditionalInsured.Checked Then Me.chkAdditionalInsured.Checked = Not Me.SubQuoteFirst.AdditionalInsuredsManualCharge.ReturnEmptyIfLessThanOrEqualToZero = String.Empty
                chkAdditionalInsured.Checked = Quote.AdditionalInsuredsCount > 0
                If chkAdditionalInsured.Checked Then
                    trAdditionalInsuredsRow.Attributes.Add("style", "display:''")
                    ' Use the top level quote for add'l insureds
                    Dim numAI As Integer = NumberOfNonCheckboxedAIs()
                    If numAI <= 4 Then
                        ddlNumberOfAddlInsureds.SetFromValue(numAI.ToString())
                    Else
                        ' More than 4 AI's
                        ddlNumberOfAddlInsureds.SetFromValue("4")
                    End If
                    ' Add'l Insureds AI Checkboxes
                    chkAI_ControllingInterests.Checked = False
                    chkAI_CoOwnerOfInsuredPremises.Checked = False
                    chkAI_EngineersArchitectsOrSurveyors.Checked = False
                    'chkAI_EngineersArchitectsOrSurveyorsNotEngaged.Checked = False
                    chkAI_MortgageeAssigneeOrReceiver.Checked = False
                    chkAI_OwnerOrOtherInterestsFromWhomLandHasBeenLeased.Checked = False
                    If Quote.AdditionalInsureds IsNot Nothing AndAlso Quote.AdditionalInsureds.Count > 0 Then
                        For Each AI As QuickQuote.CommonObjects.QuickQuoteAdditionalInsured In Quote.AdditionalInsureds
                            Select Case AI.AdditionalInsuredType
                                Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.CoOwnerOfInsuredPremises
                                    chkAI_CoOwnerOfInsuredPremises.Checked = True
                                    Exit Select
                                Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.AdditionalInsuredControllingInterest
                                    chkAI_ControllingInterests.Checked = True
                                    Exit Select
                                Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.EngineersArchitectsOrSurveyors
                                    chkAI_EngineersArchitectsOrSurveyors.Checked = True
                                    Exit Select
                            'Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.EngineersArchitectsOrSurveyorsNotEngagedByTheNamedInsured
                            '    chkAI_EngineersArchitectsOrSurveyorsNotEngaged.Checked = True
                            '    Exit Select
                                Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.MortgageeAssigneeOrReceiver
                                    chkAI_MortgageeAssigneeOrReceiver.Checked = True
                                    Exit Select
                                Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnerOrOtherInterestsFromWhomLandHasBeenLeased
                                    chkAI_OwnerOrOtherInterestsFromWhomLandHasBeenLeased.Checked = True
                                    Exit Select
                                Case Else
                                    Exit Select
                            End Select
                        Next
                    End If
                End If

                ' CPP - Condo Directors & Officers
                If QuoteISCPP And CondoDandOEligible() Then
                    trCondoDAndORow.Attributes.Add("style", "display:''")
                    ' Condo D&O - CPP
                    If SubQuoteFirst.HasCondoDandO Then
                        chkCondoDAndO.Checked = True
                        trCondoDAndODataRow1.Attributes.Add("style", "display:''")
                        trCondoDAndODataRow2.Attributes.Add("style", "display:''")
                        trCondoDAndODataRow3.Attributes.Add("style", "display:''")
                        txtNamedAssociation.Text = SubQuoteFirst.CondoDandOAssociatedName
                        SetFromValue(ddCondoDAndODeductible, SubQuoteFirst.CondoDandODeductibleId)
                        ddOccuranceLibLimit.Enabled = False 'Added 09/02/2021 for bug 51550 MLW
                    Else
                        trCondoDAndODataRow1.Attributes.Add("style", "display:none")
                        trCondoDAndODataRow2.Attributes.Add("style", "display:none")
                        trCondoDAndODataRow3.Attributes.Add("style", "display:none")
                        ddOccuranceLibLimit.Enabled = True 'Added 09/02/2021 for bug 51550 MLW
                    End If
                End If

                Me.txtEmployeeOccurrenceLimit.Text = SubQuoteFirst.OccurrenceLiabilityLimit
                'Me.txtEmployeeOccurrenceLimit.Text = "500,000"

                Me.txtEmployeeNumberOfEmployees.Text = Me.SubQuoteFirst.EmployeeBenefitsLiabilityText.ReturnEmptyIfLessThanOrEqualToZero
                ' Don't uncheck EBL if already checked!
                If Not chkEmployee.Checked Then chkEmployee.Checked = Not Me.SubQuoteFirst.EmployeeBenefitsLiabilityText.ReturnEmptyIfLessThanOrEqualToZero = String.Empty
                If chkEmployee.Checked Then
                    trEmployeeBenefitsLiabilityDataRow1.Attributes.Add("style", "display:''")
                    trEmployeeBenefitsLiabilityDataRow2.Attributes.Add("style", "display:''")
                    trEmployeeBenefitsLiabilityInfoRow.Attributes.Add("style", "display:''")
                End If

                ' CLI
                If QQHelper.doUseNewVersionOfLOB(Quote, QuickQuote.CommonMethods.QuickQuoteHelperClass.LOBIFMVersions.CyberLiabilityUpgrade) Then
                    PopulateCyberCoverage()
                    Me.chkCLI.Enabled = Not IFM.VR.Common.Helpers.CGL.CLIHelper.CLINotAvailableDoToClassCode(Me.Quote)
                    If chkCLI.Enabled = False Then
                        chkCLIWrapper.Attributes.Add("style", "display:none")
                        trCLIInfoRow1.Attributes.Add("style", "display:none")
                        trCLIInfoRow2.Attributes.Add("style", "display:none")
                    Else
                        chkCLIWrapper.Attributes.Add("style", "display:''")
                        Me.chkCLI.Checked = If(Me.chkCLI.Enabled, IFM.VR.Common.Helpers.CGL.CLIHelper.CLI_Is_Applied(Me.Quote), False)
                        If chkCLI.Checked Then
                            trCLIInfoRow1.Attributes.Add("style", "display:''")
                            trCLIInfoRow2.Attributes.Add("style", "display:''")
                        End If
                    End If
                Else
                    chkCLIWrapper.Attributes.Add("style", "display:none")
                    trCLIInfoRow1.Attributes.Add("style", "display:none")
                    trCLIInfoRow2.Attributes.Add("style", "display:none")
                End If

                ' EPLI
                Me.chkEPLI.Enabled = Not IFM.VR.Common.Helpers.CGL.EPLIHelper.EPLINotAvailableDoToClassCode(Me.Quote)
                Me.chkEPLI.Checked = If(Me.chkEPLI.Enabled, IFM.VR.Common.Helpers.CGL.EPLIHelper.EPLI_Is_Applied(Me.Quote), False)
                If (Me.chkEPLI.Checked) Then
                    trEPLIInfoRow1.Attributes.Add("style", "display:''")
                    trEPLIInfoRow2.Attributes.Add("style", "display:''")
                End If

                ' Stop Gap (OH)
                '''TODO: evaluate how we should remove  this; create task
                If IFM.VR.Common.Helpers.MultiState.General.IsOhioEffective(Quote) Then
                    trStopGapCheckboxRow.Attributes.Add("style", "display:'';")
                    If HasStopGap() Then
                        Dim gsQuote As QuickQuoteObject = Me.GoverningStateQuote()
                        chkStopGap.Checked = True
                        trStopGapDataRow.Attributes.Add("style", "display:'';width:100%;text-indent:15px;")
                        If gsQuote.StopGapLimitId IsNot Nothing AndAlso IsNumeric(gsQuote.StopGapLimitId) Then
                            ddStopGapLimit.SetFromValue(gsQuote.StopGapLimitId)
                        Else
                            ddStopGapLimit.SetFromValue("0")
                        End If
                        txtStopGapPayroll.Text = ""
                        If gsQuote.StopGapPayroll IsNot Nothing AndAlso IsNumeric(gsQuote.StopGapPayroll) Then
                            txtStopGapPayroll.Text = Format(CDec(gsQuote.StopGapPayroll), "#########")
                        End If
                    Else
                        ' OH effective, but doesn't have stop gap
                        chkStopGap.Checked = False
                        trStopGapDataRow.Attributes.Add("style", "display:none;")
                        txtStopGapPayroll.Text = ""
                        ddStopGapLimit.SelectedIndex = -1
                    End If
                Else
                    ' OH not effective
                    trStopGapCheckboxRow.Attributes.Add("style", "display:none;")
                    trStopGapDataRow.Attributes.Add("style", "display:none;")
                    chkStopGap.Checked = False
                    txtStopGapPayroll.Text = ""
                    ddStopGapLimit.SelectedIndex = -1
                    IFM.VR.Common.Helpers.MultiState.General.RemoveOHStopGapCoverageFromSubquotes(SubQuotes)
                End If

                ' Hired/Non-Owned
                Me.chkHired.Checked = Me.SubQuoteFirst.HasHiredAuto Or Me.SubQuoteFirst.HasNonOwnedAuto

                ' Liquor Liability - INDIANA
                ' Should only be shown when the quote has Indiana location & general aggregate is above 200k
                If QuoteCanHaveLiquorLiability() Then
                    If SubQuotesContainsState("IN") Then
                        IndianaQuote = SubQuoteForState("IN")
                        If IndianaQuote IsNot Nothing Then
                            trLiquorLiabilityCheckBoxRow_IN.Attributes.Add("style", "display:''")
                            txtLiquorOccurrenceLimit_IN.Text = ""
                            Me.txtLiquorOccurrenceLimit_IN.Text = IndianaQuote.OccurrenceLiabilityLimit 'always mirrors this limit
                            If txtLiquorOccurrenceLimit_IN.Text = "" Then
                                If ddOccuranceLibLimit.SelectedIndex >= 0 Then
                                    txtLiquorOccurrenceLimit_IN.Text = ddOccuranceLibLimit.SelectedItem.Text
                                End If
                            End If
                            ' Don't uncheck Liquor Liability if it's already checked!
                            If Not chkLiquor_IN.Checked Then Me.chkLiquor_IN.Checked = Not IndianaQuote.LiquorSales.ReturnEmptyIfLessThanOrEqualToZero = String.Empty
                            If chkLiquor_IN.Checked Then
                                trLiquorDataRow1_IN.Attributes.Add("style", "display:''")
                                trLiquorInfoRow_IN.Attributes.Add("style", "display:''")
                                trLiquorInfoRow2_IN.Attributes.Add("style", "display:''")
                                SetLiquorLiabilitySales(IndianaQuote)
                            End If
                        End If
                    End If
                End If

                ' Liquor Liability - OHIO
                ' Works exactly like Indiana per BA
                ' Uses the Indiana Liquor Liability section
                If QuoteCanHaveLiquorLiability() Then
                    If SubQuotesContainsState("OH") Then
                        OhioQuote = SubQuoteForState("OH")
                        If OhioQuote IsNot Nothing Then
                            trLiquorLiabilityCheckBoxRow_IN.Attributes.Add("style", "display:''")
                            txtLiquorOccurrenceLimit_IN.Text = ""
                            Me.txtLiquorOccurrenceLimit_IN.Text = OhioQuote.OccurrenceLiabilityLimit 'always mirrors this limit
                            If txtLiquorOccurrenceLimit_IN.Text = "" Then
                                If ddOccuranceLibLimit.SelectedIndex >= 0 Then
                                    txtLiquorOccurrenceLimit_IN.Text = ddOccuranceLibLimit.SelectedItem.Text
                                End If
                            End If
                            ' Don't uncheck Liquor Liability if it's already checked!
                            If Not chkLiquor_IN.Checked Then Me.chkLiquor_IN.Checked = Not OhioQuote.LiquorSales.ReturnEmptyIfLessThanOrEqualToZero = String.Empty
                            If chkLiquor_IN.Checked Then
                                trLiquorDataRow1_IN.Attributes.Add("style", "display:''")
                                trLiquorInfoRow_IN.Attributes.Add("style", "display:''")
                                trLiquorInfoRow2_IN.Attributes.Add("style", "display:''")
                                SetLiquorLiabilitySales(OhioQuote)
                            End If
                        End If
                    End If
                End If

                ' Liquor Liability - ILLINOIS
                If QuoteCanHaveLiquorLiability() Then
                    ' Should only be shown when the quote has Illinois location
                    If IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then   ' Only show the Illinois LL section if multistate is active
                        If SubQuotesContainsState("IL") Then
                            IllinoisQuote = SubQuoteForState("IL")
                            If IllinoisQuote IsNot Nothing Then
                                Dim showCov As Boolean = False
                                txtLiquorLiabilityLimit_IL.Text = ""
                                If ddOccuranceLibLimit.SelectedIndex >= 0 Then
                                    If CDate(Quote.EffectiveDate) < CDate("4/1/2020") Then
                                        txtLiquorLiabilityLimit_IL.Width = Unit.Percentage(40)
                                        Select Case ddOccuranceLibLimit.SelectedValue
                                        ' Liquor Liability is not available when 100k or 200k OLL is chosen, only shown for 300k and above
                                        'Case "10"  ' 100,000
                                        '    txtLiquorLiabilityLimit_IL.Text = "69/69/85/600"
                                        '    Exit Select
                                        'Case "32" ' 200,000
                                        '    txtLiquorLiabilityLimit_IL.Text = "69/69/85/600"
                                        '    Exit Select                                           
                                            Case "33" ' 300,000
                                                txtLiquorLiabilityLimit_IL.Text = "STATE STATUTORY LIMITS WILL APPLY"
                                                showCov = True
                                                Exit Select
                                            Case "34" ' 500,000
                                                txtLiquorLiabilityLimit_IL.Text = "STATE STATUTORY LIMITS WILL APPLY"
                                                showCov = True
                                                Exit Select
                                            Case "56" ' 1,000,000
                                                txtLiquorLiabilityLimit_IL.Text = "STATE STATUTORY LIMITS WILL APPLY"
                                                showCov = True
                                                Exit Select
                                            Case Else
                                                ' Default - if we got here something is wrong
                                                txtLiquorLiabilityLimit_IL.Text = ""
                                                Exit Select
                                        End Select
                                    Else
                                        ' New IL Liquor Liability Limit Logic per Task 40486 MGB 3/9/2020
                                        txtLiquorLiabilityLimit_IL.Width = Unit.Percentage(75)
                                        txtLiquorLiabilityLimit_IL.Text = "STATE STATUTORY LIMITS WILL APPLY"
                                        showCov = True
                                    End If
                                End If

                                ' If the OLL is eligible for Liquor Liability, show the coverage 
                                If showCov Then
                                    ' Show the checkbox
                                    trLiquorLiabilityCheckboxRow_IL.Attributes.Add("style", "display:''")

                                    ' Don't uncheck Liquor Liability if it's already checked!
                                    If Not chkLiquor_IL.Checked Then Me.chkLiquor_IL.Checked = Not IllinoisQuote.LiquorSales.ReturnEmptyIfLessThanOrEqualToZero = String.Empty
                                    If chkLiquor_IL.Checked Then
                                        trLiquorLiabilityCheckboxRow_IL.Attributes.Add("style", "display:''")
                                        trLiquorDataRow1_IL.Attributes.Add("style", "display:''")
                                        trLiquorInfoRow_IL.Attributes.Add("style", "display:''")
                                        trLiquorInfoRow2_IL.Attributes.Add("style", "display:''")
                                        trLiquorInfoRow3_IL.Attributes.Add("style", "display:''")
                                        SetLiquorLiabilitySales(IllinoisQuote)
                                    End If
                                End If   ' if showCov
                            End If  ' if illinoisQuote isnot nothing
                        End If  ' is subquotestates contain IL
                    End If  ' If isMultistateCapableDate
                End If ' If QuoteCanHaveLiquorLiability

                ' Contractors Home Remodeling and Repair (IL)
                If IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then
                    If GoverningStateQuote.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois OrElse SubQuotesContainsState("IL") Then
                        trContractorsHomeRepairAndRemodelingRow.Attributes.Add("style", "display:''")
                        'trContractorsHomeRepairAndRemodelingInfoRow.Attributes.Add("style", "display:''") 'removed 11/19/2018
                        Dim ILQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState("IL")
                        chkContractorsHomeRepairAndRemodeling_IL.Checked = ILQuote.HasIllinoisContractorsHomeRepairAndRemodeling
                        If chkContractorsHomeRepairAndRemodeling_IL.Checked Then
                            trContractorsHomeRepairAndRemodelingInfoRow.Attributes.Add("style", "display:''")
                        End If
                    End If
                End If
            End If
        End If

    End Sub

    Public Sub PopulateCyberCoverage()
        If IFM.VR.Common.Helpers.CPP.CyberCoverageHelper.IsCyberCoverageAvailable(Quote) Then
            chkCLI.Text = "Cyber Coverage"
        Else
            chkCLI.Text = "Cyber Liability"
        End If
    End Sub

    Private Function NumberOfNonCheckboxedAIs() As Integer
        Dim num As Integer = 0
        If Quote.AdditionalInsureds IsNot Nothing AndAlso Quote.AdditionalInsureds.Count > 0 Then
            For Each AI As QuickQuote.CommonObjects.QuickQuoteAdditionalInsured In Quote.AdditionalInsureds
                Select Case AI.AdditionalInsuredType
                    Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.CoOwnerOfInsuredPremises
                        chkAI_CoOwnerOfInsuredPremises.Checked = True
                        Exit Select
                    Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.AdditionalInsuredControllingInterest
                        chkAI_ControllingInterests.Checked = True
                        Exit Select
                    Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.EngineersArchitectsOrSurveyors
                        chkAI_EngineersArchitectsOrSurveyors.Checked = True
                        Exit Select
                'Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.EngineersArchitectsOrSurveyorsNotEngagedByTheNamedInsured
                '    chkAI_EngineersArchitectsOrSurveyorsNotEngaged.Checked = True
                '    Exit Select
                    Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.MortgageeAssigneeOrReceiver
                        chkAI_MortgageeAssigneeOrReceiver.Checked = True
                        Exit Select
                    Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnerOrOtherInterestsFromWhomLandHasBeenLeased
                        chkAI_OwnerOrOtherInterestsFromWhomLandHasBeenLeased.Checked = True
                        Exit Select
                    Case Else
                        num += 1
                        Exit Select
                End Select
            Next
        End If
        Return num
    End Function

    Private Function InsertCheckboxAdditionalInsured(ByVal AI As QuickQuote.CommonObjects.QuickQuoteAdditionalInsured) As Boolean
        If AI.AdditionalInsuredType <> QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.None AndAlso QQHelper.IsNumericString(AI.CoverageCodeId) = True Then
            ' Only add the add'l insured if one of the same type does not already exist
            If Quote.AdditionalInsureds Is Nothing Then Quote.AdditionalInsureds = New List(Of QuickQuoteAdditionalInsured)
            If Not AdditionalInsuredTypeExists(AI.AdditionalInsuredType) Then
                Quote.AdditionalInsureds.Add(AI)
            End If
        End If
        Return True
    End Function

    Private Function RemoveAdditionalInsuredIfPresent(ByVal AIType As QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType) As Boolean
        If Quote.AdditionalInsureds IsNot Nothing AndAlso Quote.AdditionalInsureds.Count > 0 Then
AILoop:
            For Each AI As QuickQuoteAdditionalInsured In Quote.AdditionalInsureds
                If AI.AdditionalInsuredType = AIType Then
                    Quote.AdditionalInsureds.Remove(AI)
                    GoTo AILoop
                End If
            Next
        End If
        Return True
    End Function

    Private Function AdditionalInsuredTypeExists(ByVal AIType As QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType) As Boolean
        If Quote.AdditionalInsureds IsNot Nothing AndAlso Quote.AdditionalInsureds.Count > 0 Then
            For Each AI As QuickQuoteAdditionalInsured In Quote.AdditionalInsureds
                If AI.AdditionalInsuredType = AIType Then Return True
            Next
        End If
        Return False
    End Function

    Private Function CondoDandOEligible() As Boolean
        Dim riskgradecode As String = Nothing
        Dim condoclasses As New List(Of String)
        Dim rtn As String = Nothing

        Try
            condoclasses.Add("62000")
            condoclasses.Add("62001")
            condoclasses.Add("62002")
            condoclasses.Add("62003")
            condoclasses.Add("68500")

            Dim qqh As New QuickQuote.CommonMethods.QuickQuoteHelperClass()
            ' Updated the stored proc and added state id to the parameters
            ' Note that Condo D&O is CPP only.  if we were to enable it for CGL, we would need to pass
            ' in the CGL Risk Grade Lookup ID in the @ID field.
            Dim spm As New SPManager("connDiamondReports", "usp_Get_RiskGradeLookup")
            spm.AddIntegerParamater("@Id", qqh.IntegerForString(SubQuoteFirst.CPP_CPR_RiskGradeLookupId))
            spm.AddIntegerParamater("StateId", qqh.IntegerForString(Quote.StateId))
            Dim tbl As DataTable = spm.ExecuteSPQuery()
            If tbl IsNot Nothing AndAlso tbl.Rows.Count > 0 Then
                rtn = tbl.Rows(0)("glclasscode").ToString
                If condoclasses.Contains(rtn) Then Return True
            End If

            Return False
        Catch ex As Exception
            Return False
        End Try
    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        Dim HasAI As Boolean = False
        Dim gsQuote As QuickQuoteObject = Me.GoverningStateQuote()

        If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then

            IFM.VR.Common.Helpers.CGL.EPLIHelper.Toggle_EPLI_Is_Applied(Me.Quote, Me.chkEPLI.Checked)
            IFM.VR.Common.Helpers.CGL.CLIHelper.Toggle_CLI_Is_Applied(Me.Quote, Me.chkCLI.Checked) 'zts

            'MODIFIED 08/10/2022 FOR 76291
            '** stop gap will only be added to the governing state quote to prevent overcharging for this coverage 
            ' Add stop gap at the quote level as well as the subquote level
            With gsQuote
                IFM.VR.Common.Helpers.MultiState.General.RemoveOHStopGapCoverageFromSubquotes(SubQuotes)
                If chkStopGap.Checked Then
                    .StopGapLimitId = ddStopGapLimit.SelectedValue
                    .StopGapPayroll = txtStopGapPayroll.Text
                End If
            End With
            For Each sq As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
                If Not QuoteISCPP Then Me.ddProgramType.GetFromValue(sq.ProgramTypeId)  ' Only save program type on CGL
                Me.ddGeneralAgg.GetFromValue(sq.GeneralAggregateLimitId)
                'If sq.GLClassifications.Count > 0 AndAlso 'Adding this IF condition for 59049 BB
                '   (sq.GLClassifications.Item(0).ClassCode = "18707" OrElse
                '            sq.GLClassifications.Item(0).ClassCode = "18708" OrElse
                '            sq.GLClassifications.Item(0).ClassCode = "99760") Then
                '    sq.ProductsCompletedOperationsAggregateLimitId = 327
                'Else
                '    Me.ddOperationsAgg.GetFromValue(sq.ProductsCompletedOperationsAggregateLimitId)
                'End If
                'updated 3/15/2021 to make sure GLClassifications is something before checking count; 3/16/2021: will just use new function now
                If HasSpecialTobaccoClassCode(sq) = True Then
                    sq.ProductsCompletedOperationsAggregateLimitId = 327
                Else
                    Me.ddOperationsAgg.GetFromValue(sq.ProductsCompletedOperationsAggregateLimitId)
                End If

                ' if productCompleted is excluded you need all the classcodes Subline336_ExcludeProductsCompletedOperations  to be true, else false
                IFM.VR.Common.Helpers.CGL.ClassCodeHelper.ConvertAllClassCodeSubline336ExcludedTo(Me.Quote) 'Todo - Is this safe to do outside the subquote loop????

                Me.ddPersonalInjury.GetFromValue(sq.PersonalAndAdvertisingInjuryLimitId)
                Me.ddOccuranceLibLimit.GetFromValue(sq.OccurrenceLiabilityLimitId)

                sq.DamageToPremisesRentedLimit = "100,000" ' must always go through as 100,000 - Diamond will adjust if needed - what you see on the screen is informational only

                If QuoteISCPP Then
                    sq.Has_PackageGL_EnhancementEndorsement = Me.chkGLEnhancement.Checked
                    sq.Has_PackageGL_PlusEnhancementEndorsement = Me.chkGLPlusEnhancement.Checked
                Else
                    sq.HasBusinessMasterEnhancement = Me.chkGLEnhancement.Checked
                    sq.Has_PackageGL_PlusEnhancementEndorsement = Me.chkGLPlusEnhancement.Checked
                End If

                If Me.chkGLEnhancement.Checked Then 'only available with the enhancement - Look to Bug 4040 for additional information
                    ' String.Empty = 'None'
                    ' 1 = 'Yes' CGL1004
                    ' 2 = 'YES With Completed Ops' CGL1002
                    Me.ddlAddlBlanketOfSubroOptions.GetFromValue(sq.BlanketWaiverOfSubrogation)
                Else
                    sq.BlanketWaiverOfSubrogation = String.Empty
                End If

                ' NOTE that we don't save the Contractors/Manufacturers Enhancement checkboxes - they are display only
                '      the values are set on the CPR Policy Coverages page and can't be changed here.

                Select Case ddlAddAGLDeductible.SelectedValue
                    Case "1"
                        Me.ddType.GetFromValue(sq.GL_PremisesAndProducts_DeductibleCategoryTypeId)
                        Me.ddAmount.GetFromValue(sq.GL_PremisesAndProducts_DeductibleId)
                        Me.ddBasis.GetFromValue(sq.GL_PremisesAndProducts_DeductiblePerTypeId)
                        Exit Select
                    Case "0"
                        sq.GL_PremisesAndProducts_DeductibleCategoryTypeId = String.Empty
                        sq.GL_PremisesAndProducts_DeductibleId = String.Empty
                        sq.GL_PremisesAndProducts_DeductiblePerTypeId = String.Empty
                        Exit Select
                End Select

                ' New Additional Insureds code 10/25/2017 MGB
                If chkAdditionalInsured.Checked Then
                    If Quote.AdditionalInsureds Is Nothing Then Quote.AdditionalInsureds = New List(Of QuickQuoteAdditionalInsured)
                    Dim numddlAI As Integer = CInt(ddlNumberOfAddlInsureds.SelectedValue)
                    Dim numnonchkAI As Integer = NumberOfNonCheckboxedAIs()
                    Quote.AdditionalInsuredsCount = numnonchkAI

                    ' Create blank ai's for the value in the number of additional insureds dropdown.  So if they select "2", create 2 blank ai's
                    If IsNumeric(ddlNumberOfAddlInsureds.SelectedValue) AndAlso CInt(ddlNumberOfAddlInsureds.SelectedValue) > 0 Then
                        ' Make sure we have the correct number of non-checkboxed AI's added
                        If numnonchkAI < numddlAI Then
                            For a = 1 To (numddlAI - numnonchkAI)
                                Dim newAI As New QuickQuote.CommonObjects.QuickQuoteAdditionalInsured()
                                newAI.AdditionalInsuredType = QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.Vendors
                                Quote.AdditionalInsureds.Add(newAI)
                            Next
                        End If
                    End If

                    ' AI Checkboxes - add additional interest if checked, remove if not
                    If chkAI_ControllingInterests.Checked OrElse chkAI_CoOwnerOfInsuredPremises.Checked OrElse chkAI_EngineersArchitectsOrSurveyors.Checked _
                        OrElse chkAI_MortgageeAssigneeOrReceiver.Checked OrElse chkAI_OwnerOrOtherInterestsFromWhomLandHasBeenLeased.Checked Then
                        Dim NewAI As New QuickQuote.CommonObjects.QuickQuoteAdditionalInsured()

                        If chkAI_CoOwnerOfInsuredPremises.Checked Then
                            NewAI = New QuickQuoteAdditionalInsured()
                            NewAI.AdditionalInsuredType = QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.CoOwnerOfInsuredPremises
                            InsertCheckboxAdditionalInsured(NewAI)
                            HasAI = True
                        Else
                            RemoveAdditionalInsuredIfPresent(QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.CoOwnerOfInsuredPremises)
                        End If
                        If chkAI_ControllingInterests.Checked Then
                            NewAI = New QuickQuoteAdditionalInsured()
                            NewAI.AdditionalInsuredType = QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.AdditionalInsuredControllingInterest
                            InsertCheckboxAdditionalInsured(NewAI)
                            HasAI = True
                        Else
                            RemoveAdditionalInsuredIfPresent(QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.AdditionalInsuredControllingInterest)
                        End If
                        If chkAI_EngineersArchitectsOrSurveyors.Checked Then
                            NewAI = New QuickQuoteAdditionalInsured()
                            NewAI.AdditionalInsuredType = QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.EngineersArchitectsOrSurveyors
                            InsertCheckboxAdditionalInsured(NewAI)
                            HasAI = True
                        Else
                            RemoveAdditionalInsuredIfPresent(QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.EngineersArchitectsOrSurveyors)
                        End If
                        If chkAI_MortgageeAssigneeOrReceiver.Checked Then
                            NewAI = New QuickQuoteAdditionalInsured()
                            NewAI.AdditionalInsuredType = QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.MortgageeAssigneeOrReceiver
                            InsertCheckboxAdditionalInsured(NewAI)
                            HasAI = True
                        Else
                            RemoveAdditionalInsuredIfPresent(QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.MortgageeAssigneeOrReceiver)
                        End If
                        If chkAI_OwnerOrOtherInterestsFromWhomLandHasBeenLeased.Checked Then
                            NewAI = New QuickQuoteAdditionalInsured()
                            NewAI.AdditionalInsuredType = QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnerOrOtherInterestsFromWhomLandHasBeenLeased
                            InsertCheckboxAdditionalInsured(NewAI)
                            HasAI = True
                        Else
                            RemoveAdditionalInsuredIfPresent(QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnerOrOtherInterestsFromWhomLandHasBeenLeased)
                        End If
                    End If
                Else
                    Quote.AdditionalInsuredsCount = 0
                End If
                'Quote.HasAdditionalInsuredsCheckboxBOP = HasAI
                Quote.HasAdditionalInsuredsCheckboxBOP = False

                ' Update the premium amount for each non-checkboxed ai.  
                UpdateAIManualPremiumAmounts()
                'QQHelper.UpdateAdditionalInsuredManualPremiumAmountForAllInList("25", Quote.AdditionalInsureds)

                If QuoteISCPP Then
                    ' Condo D&O - CPP
                    sq.HasCondoDandO = chkCondoDAndO.Checked
                    If sq.HasCondoDandO Then
                        sq.CondoDandOAssociatedName = txtNamedAssociation.Text
                        sq.CondoDandOManualLimit = "1000000"
                        sq.CondoDandODeductibleId = ddCondoDAndODeductible.SelectedValue
                    Else
                        sq.CondoDandOAssociatedName = ""
                        sq.CondoDandOManualLimit = ""
                        sq.CondoDandODeductibleId = ""
                    End If
                End If

                sq.EmployeeBenefitsLiabilityOccurrenceLimit = If(Me.chkEmployee.Checked, Me.txtEmployeeOccurrenceLimit.Text, String.Empty) 'mirrors OccurrenceLiabilityLimit if the coverage is applied
                sq.EmployeeBenefitsLiabilityText = If(Me.chkEmployee.Checked, Me.txtEmployeeNumberOfEmployees.Text.Trim(), String.Empty)
                sq.EmployeeBenefitsLiabilityAggregateLimit = If(Me.chkEmployee.Checked, (sq.EmployeeBenefitsLiabilityOccurrenceLimit.TryToGetDouble * 3).ToString(), String.Empty)
                sq.EmployeeBenefitsLiabilityDeductible = If(Me.chkEmployee.Checked, "1,000", String.Empty)


                ''MODIFIED 08/10/2022 FOR 76291
                '** stop gap will only be added to the governing state quote to prevent overcharging for this coverage 
                '' Stop Gap (OH)
                'If chkStopGap.Checked Then
                '    sq.StopGapLimitId = ddStopGapLimit.SelectedValue
                '    sq.StopGapPayroll = txtStopGapPayroll.Text
                'Else
                '    sq.StopGapLimitId = ""
                '    sq.StopGapPayroll = ""
                'End If

                sq.HasHiredAuto = Me.chkHired.Checked
                sq.HasNonOwnedAuto = Me.chkHired.Checked

                ' Liquor - Indiana
                ' NOTE: Indiana and Ohio both use the same logic
                If QuoteCanHaveLiquorLiability() Then
                    If Me.chkLiquor_IN.Checked Then
                        ' CHECKED
                        ' Indiana
                        Dim IndianaQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana)
                        If IndianaQuote IsNot Nothing Then
                            IndianaQuote.LiquorLiabilityOccurrenceLimit = txtLiquorOccurrenceLimit_IN.Text
                            SaveLiquorLiabilitySales(IndianaQuote)
                        End If
                        ' Ohio
                        Dim OhioQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio)
                        If OhioQuote IsNot Nothing Then
                            OhioQuote.LiquorLiabilityOccurrenceLimit = txtLiquorOccurrenceLimit_IN.Text
                            SaveLiquorLiabilitySales(OhioQuote)
                        End If
                    Else
                        ' NOT CHECKED
                        ' Indiana
                        Dim IndianaQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana)
                        If IndianaQuote IsNot Nothing Then
                            IndianaQuote.LiquorLiabilityClassificationId = String.Empty
                            IndianaQuote.LiquorLiabilityOccurrenceLimit = ""
                            IndianaQuote.LiquorSales = ""
                            IndianaQuote.LiquorManufacturersSales = ""
                            IndianaQuote.LiquorRestaurantsSales = ""
                            IndianaQuote.LiquorPackageStoresSales = ""
                            IndianaQuote.LiquorClubsSales = ""
                        End If
                        ' Ohio
                        Dim OhioQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio)
                        If OhioQuote IsNot Nothing Then
                            OhioQuote.LiquorLiabilityClassificationId = String.Empty
                            OhioQuote.LiquorLiabilityOccurrenceLimit = ""
                            OhioQuote.LiquorSales = ""
                            OhioQuote.LiquorManufacturersSales = ""
                            OhioQuote.LiquorRestaurantsSales = ""
                            OhioQuote.LiquorPackageStoresSales = ""
                            OhioQuote.LiquorClubsSales = ""
                        End If
                    End If
                Else
                    ' Quote can't have Liquor Liability
                    ' Indiana
                    Dim IndianaQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana)
                    If IndianaQuote IsNot Nothing Then
                        IndianaQuote.LiquorLiabilityClassificationId = String.Empty
                        IndianaQuote.LiquorLiabilityOccurrenceLimit = ""
                        IndianaQuote.LiquorSales = ""
                        IndianaQuote.LiquorManufacturersSales = ""
                        IndianaQuote.LiquorRestaurantsSales = ""
                        IndianaQuote.LiquorPackageStoresSales = ""
                        IndianaQuote.LiquorClubsSales = ""
                    End If
                    ' Ohio
                    Dim OhioQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio)
                    If OhioQuote IsNot Nothing Then
                        OhioQuote.LiquorLiabilityClassificationId = String.Empty
                        OhioQuote.LiquorLiabilityOccurrenceLimit = ""
                        OhioQuote.LiquorSales = ""
                        OhioQuote.LiquorManufacturersSales = ""
                        OhioQuote.LiquorRestaurantsSales = ""
                        OhioQuote.LiquorPackageStoresSales = ""
                        OhioQuote.LiquorClubsSales = ""
                    End If
                End If

                ' Liquor - Illinois
                If QuoteCanHaveLiquorLiability() Then
                    If Me.chkLiquor_IL.Checked Then
                        Dim IllinoisQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois)
                        If IllinoisQuote IsNot Nothing Then
                            If CDate(Quote.EffectiveDate) < CDate("4/1/2020") Then
                                Select Case txtLiquorLiabilityLimit_IL.Text
                                    Case "69/69/85/600"
                                        IllinoisQuote.LiquorLiabilityOccurrenceLimitId = "396"
                                        Exit Select
                                    Case "69/69/85/1000"
                                        IllinoisQuote.LiquorLiabilityOccurrenceLimitId = "397"
                                        Exit Select
                                    Case "69/69/85/2000"
                                        IllinoisQuote.LiquorLiabilityOccurrenceLimitId = "399"
                                        Exit Select
                                    Case Else
                                        IllinoisQuote.LiquorLiabilityOccurrenceLimitId = ""
                                End Select
                            Else
                                ' New limit id's effective after 4/1/2020 Task 404086 MGB 3/9/2020
                                ' Limit is based on the policy occurrence liability limit
                                'Select Case ddOccuranceLibLimit.SelectedValue
                                '    Case "33" ' 300k  71/71/86/600
                                '        IllinoisQuote.LiquorLiabilityOccurrenceLimitId = "427"
                                '        Exit Select
                                '    Case "34" ' 500k  71/71/86/1000
                                '        IllinoisQuote.LiquorLiabilityOccurrenceLimitId = "428"
                                '        Exit Select
                                '    Case "56" ' 1M  71/71/86/2000
                                '        IllinoisQuote.LiquorLiabilityOccurrenceLimitId = "429"
                                '        Exit Select
                                '    Case Else
                                '        IllinoisQuote.LiquorLiabilityOccurrenceLimitId = ""
                                '        Exit Select
                                'End Select

                                ' Task 58682 BD
                                Select Case ddOccuranceLibLimit.SelectedValue

                                    Case "33" ' 300k(600K)
                                        IllinoisQuote.LiquorLiabilityOccurrenceLimitId = "439"
                                        Exit Select
                                    Case "34" ' 500K(1M) 
                                        IllinoisQuote.LiquorLiabilityOccurrenceLimitId = "440"
                                        Exit Select

                                    Case "56" ' 1M(2M) 
                                        IllinoisQuote.LiquorLiabilityOccurrenceLimitId = "441"
                                        Exit Select
                                End Select
                            End If

                            SaveLiquorLiabilitySales(IllinoisQuote)
                        End If
                    Else
                        Dim IllinoisQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois)
                        If IllinoisQuote IsNot Nothing Then
                            IllinoisQuote.LiquorLiabilityClassificationId = String.Empty
                            IllinoisQuote.LiquorLiabilityOccurrenceLimit = ""
                            IllinoisQuote.LiquorSales = ""
                            IllinoisQuote.LiquorManufacturersSales = ""
                            IllinoisQuote.LiquorRestaurantsSales = ""
                            IllinoisQuote.LiquorPackageStoresSales = ""
                            IllinoisQuote.LiquorClubsSales = ""
                        End If
                    End If
                Else
                    ' Quote can't have liquor liability
                    Dim IllinoisQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois)
                    If IllinoisQuote IsNot Nothing Then
                        IllinoisQuote.LiquorLiabilityClassificationId = String.Empty
                        IllinoisQuote.LiquorLiabilityOccurrenceLimit = ""
                        IllinoisQuote.LiquorSales = ""
                        IllinoisQuote.LiquorManufacturersSales = ""
                        IllinoisQuote.LiquorRestaurantsSales = ""
                        IllinoisQuote.LiquorPackageStoresSales = ""
                        IllinoisQuote.LiquorClubsSales = ""
                    End If
                End If

                If IsMultistateCapableEffectiveDate(Quote.EffectiveDate) AndAlso SubQuotesContainsState("IL") Then
                    'If chkContractorsHomeRepairAndRemodeling_IL.Checked Then 'removed IF 11/19/2018 so user can remove coverage; previously only adding w/o ability to remove
                    Dim ILQuote As QuickQuote.CommonObjects.QuickQuoteObject = SubQuoteForState(QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois)
                    If ILQuote IsNot Nothing Then
                        ILQuote.HasIllinoisContractorsHomeRepairAndRemodeling = chkContractorsHomeRepairAndRemodeling_IL.Checked
                        If ILQuote.HasIllinoisContractorsHomeRepairAndRemodeling Then
                            ILQuote.IllinoisContractorsHomeRepairAndRemodelingManualLimitAmount = "10000"
                        Else
                            ILQuote.IllinoisContractorsHomeRepairAndRemodelingManualLimitAmount = ""
                        End If
                    End If
                    'End If
                End If
                If Not IsQuoteReadOnly() Then
                    SaveMedicalExpensesDropdownForExcludedCGLClassCodes(Quote, sq)
                End If
                Me.ddMedicalExpense.GetFromValue(sq.MedicalExpensesLimitId) 'updated MedicalExpensesLimit to be a dropdown list instead of a text box.
            Next
        End If
        Return True
    End Function

    Private Function HasSpecialTobaccoClassCode(ByVal qqo As QuickQuoteObject) As Boolean 'added 3/16/2021
        Dim hasIt As Boolean = False
        If qqo IsNot Nothing AndAlso qqo.GLClassifications IsNot Nothing AndAlso qqo.GLClassifications.Count > 0 Then
            For Each c As QuickQuoteGLClassification In qqo.GLClassifications
                If c IsNot Nothing AndAlso String.IsNullOrWhiteSpace(c.ClassCode) = False Then
                    If c.ClassCode = "18707" OrElse c.ClassCode = "18708" OrElse c.ClassCode = "99760" Then
                        hasIt = True
                        Exit For
                    End If
                End If
            Next
        End If
        Return hasIt
    End Function

    Public Sub SaveMedicalExpensesDropdownForExcludedCGLClassCodes(Quote As QuickQuoteObject, sq As QuickQuoteObject)
        If Quote IsNot Nothing Then
            If Not IsQuoteReadOnly() Then
                If CGLMedicalExpensesExcludedClassCodesHelper.HasCGLMedicalExpensesExcludedClassCode(Quote) = True AndAlso ddMedicalExpense.SelectedValue <> "327" Then
                    ' If the medical expenses limit is not already set to "327", then set it to "327", disable the dropdown and show the message
                    Me.ddMedicalExpense.SelectedValue = "327"
                    CGLMedicalExpensesExcludedClassCodesHelper.ShowCGLMedicalExpensesPopupMessage(Me.Page, CGLMedicalExpensesExcludedClassCodesHelper.CGLMedicalExpensesExcludedClassCodesMsg)
                ElseIf CGLMedicalExpensesExcludedClassCodesHelper.HasCGLMedicalExpensesExcludedClassCode(Quote) = False AndAlso CGLMedicalExpensesExcludedClassCodesHelper.HasCGLEnhancementEndorsementOrCPRPackageEnhancementEndorsement(Quote) = True AndAlso ddMedicalExpense.SelectedValue = "327" Then
                    CGLMedicalExpensesExcludedClassCodesHelper.ShowCGLMedicalExpensesPopupMessage(Me.Page, CGLMedicalExpensesExcludedClassCodesHelper.GeneralLiabilityEnhancementEndorsementMsg)
                ElseIf CGLMedicalExpensesExcludedClassCodesHelper.HasCGLMedicalExpensesExcludedClassCode(Quote) = False AndAlso Me.ddMedicalExpense.Enabled = False Then
                    ddMedicalExpense.SelectedValue = "327"
                    Me.ddMedicalExpense.Enabled = True
                    Me.chkGLEnhancement.Enabled = True
                    Me.chkGLPlusEnhancement.Enabled = True
                    CGLMedicalExpensesExcludedClassCodesHelper.ShowCGLMedicalExpensesPopupMessage(Me.Page, CGLMedicalExpensesExcludedClassCodesHelper.CGLMedicalExpensesNonExcludedClassCodesMsg)
                End If
                If ddMedicalExpense.SelectedValue = "327" Then
                    sq.Has_PackageGL_PlusEnhancementEndorsement = False
                    sq.Has_PackageGL_EnhancementEndorsement = False
                    sq.HasContractorsEnhancement = False
                    sq.HasFoodManufacturersEnhancement = False
                    sq.Has_PackageCPR_EnhancementEndorsement = False
                    sq.Has_PackageCPR_PlusEnhancementEndorsement = False
                    If CGLMedicalExpensesExcludedClassCodesHelper.HasCGLMedicalExpensesExcludedClassCode(Quote) = True Then
                        Me.ddMedicalExpense.Enabled = False
                    End If
                End If
            End If
        End If
    End Sub

    Public Sub PopulateGLEnhancementEndorsementCheckboxes(Quote As QuickQuoteObject)
        If Quote IsNot Nothing Then
            If Not IsQuoteReadOnly() Then
                If Quote IsNot Nothing AndAlso SubQuoteFirst IsNot Nothing Then
                    If CGLMedicalExpensesExcludedClassCodesHelper.HasCGLMedicalExpensesExcludedClassCode(Quote) = True Then
                        Me.ddMedicalExpense.SelectedValue = "327"
                        Me.ddMedicalExpense.Enabled = False
                    Else
                        Me.ddMedicalExpense.Enabled = True
                    End If
                    If Me.ddMedicalExpense.SelectedValue = "327" Then
                        Me.chkContractorsGLEnhancement.Checked = False
                        Me.chkContractorsGLEnhancement.Enabled = False
                        Me.chkManufacturersGLEnhancement.Checked = False
                        Me.chkManufacturersGLEnhancement.Enabled = False
                        Me.chkGLEnhancement.Checked = False
                        Me.chkGLPlusEnhancement.Checked = False
                        Me.chkGLEnhancement.Enabled = False
                        Me.chkGLPlusEnhancement.Enabled = False
                    Else
                        Me.ddMedicalExpense.Enabled = True
                        Me.chkGLEnhancement.Enabled = True
                        Me.chkGLPlusEnhancement.Enabled = True
                    End If
                End If
            End If
        End If
    End Sub

    Public Sub PopulateMedicalExpenses(Quote As QuickQuoteObject)
        If Quote IsNot Nothing AndAlso IsNullEmptyorWhitespace(Me.SubQuoteFirst.MedicalExpensesLimitId) Then
            Me.ddMedicalExpense.SetFromValue(Me.SubQuoteFirst.MedicalExpensesLimitId, "15")
        Else
            WebHelper_Personal.SetDropDownValue_ForceDiamondValue(ddMedicalExpense, Me.SubQuoteFirst.MedicalExpensesLimitId, QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.MedicalExpensesLimitId)
            If Not IsQuoteReadOnly() Then
                If CGLMedicalExpensesExcludedClassCodesHelper.HasCGLMedicalExpensesExcludedClassCode(Quote) = True Then
                    Me.ddMedicalExpense.SelectedValue = "327"
                    Me.ddMedicalExpense.Enabled = False
                Else
                    If Me.ddMedicalExpense.SelectedValue = "327" AndAlso Me.ddMedicalExpense.Enabled = False Then
                        CGLMedicalExpensesExcludedClassCodesHelper.ShowCGLMedicalExpensesPopupMessage(Me.Page, CGLMedicalExpensesExcludedClassCodesHelper.CGLMedicalExpensesNonExcludedClassCodesMsg)
                        Me.ddMedicalExpense.Enabled = True
                        Me.ddMedicalExpense.SetFromValue(Me.SubQuoteFirst.MedicalExpensesLimitId, "15")
                        Me.ddMedicalExpense.SelectedValue = "15"
                        Me.chkGLEnhancement.Enabled = True
                        Me.chkGLPlusEnhancement.Enabled = True
                    End If
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Updates the manual premium amount for non-checkboxed additional insureds.
    ''' Non-checkboxed ai's get a 25 dollar premium.  
    ''' Checkboxed ai's don't get a premium update.
    ''' </summary>
    Private Sub UpdateAIManualPremiumAmounts()
        If Quote.AdditionalInsureds IsNot Nothing AndAlso Quote.AdditionalInsureds.Count > 0 Then
            For Each AI As QuickQuote.CommonObjects.QuickQuoteAdditionalInsured In Quote.AdditionalInsureds
                If AI.AdditionalInsuredType = QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.CoOwnerOfInsuredPremises _
                    OrElse AI.AdditionalInsuredType = QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.AdditionalInsuredControllingInterest _
                    OrElse AI.AdditionalInsuredType = QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.EngineersArchitectsOrSurveyors _
                    OrElse AI.AdditionalInsuredType = QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.MortgageeAssigneeOrReceiver _
                    OrElse AI.AdditionalInsuredType = QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnerOrOtherInterestsFromWhomLandHasBeenLeased Then
                    ' No premium for checkboxed ai's
                    AI.ManualPremiumAmount = "0"
                Else
                    ' Non-checkboxed ai's: Premium = $25
                    AI.ManualPremiumAmount = "25"
                End If
            Next
        End If
    End Sub

    Protected Function GetAIObject(ClassCodeID As QuickQuote.CommonObjects.QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType, Optional count As Integer = 0) As QuickQuote.CommonObjects.QuickQuoteAdditionalInsured
        Dim ai As New QuickQuote.CommonObjects.QuickQuoteAdditionalInsured()
        ai.AdditionalInsuredType = ClassCodeID
        ai.CoverageCodeId = ClassCodeID
        If ClassCodeID = QuickQuote.CommonObjects.QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnersLesseesOrContractorsCompletedOperations Then
            ai.Description = UCase("Description " & count.ToString())
            ai.DesignationOfPremises = UCase("Address " & count.ToString())
            ai.NameOfPersonOrOrganization = UCase("Name " & count.ToString())
        End If

        Return ai
    End Function


    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        Me.ValidationHelper.GroupName = "General Information"

        Dim vals = IFM.VR.Validation.ObjectValidation.CommLines.LOB.CGL.GeneralInformationValidator.ValidateGeneralInformation(Me.SubQuoteFirst, valArgs.ValidationType)
        If vals.Any() Then
            For Each v In vals
                Select Case v.FieldId
                    Case IFM.VR.Validation.ObjectValidation.CommLines.LOB.CGL.GeneralInformationValidator.advertisingInjury
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddPersonalInjury, v, accordList)
                    Case IFM.VR.Validation.ObjectValidation.CommLines.LOB.CGL.GeneralInformationValidator.DamageToPremisesRentedLimit
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtRented, v, accordList)
                    Case IFM.VR.Validation.ObjectValidation.CommLines.LOB.CGL.GeneralInformationValidator.deductibleAmount
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddAmount, v, accordList)
                    Case IFM.VR.Validation.ObjectValidation.CommLines.LOB.CGL.GeneralInformationValidator.deductibleBasis
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddBasis, v, accordList)
                    Case IFM.VR.Validation.ObjectValidation.CommLines.LOB.CGL.GeneralInformationValidator.deductibleType
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddType, v, accordList)
                    Case IFM.VR.Validation.ObjectValidation.CommLines.LOB.CGL.GeneralInformationValidator.generalAggregate
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddGeneralAgg, v, accordList)
                    Case IFM.VR.Validation.ObjectValidation.CommLines.LOB.CGL.GeneralInformationValidator.MedicalExpensesLimit
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddMedicalExpense, v, accordList)
                    Case IFM.VR.Validation.ObjectValidation.CommLines.LOB.CGL.GeneralInformationValidator.occurrenceLibLimit
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddOccuranceLibLimit, v, accordList)
                    Case IFM.VR.Validation.ObjectValidation.CommLines.LOB.CGL.GeneralInformationValidator.ProductOperationsAg
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddOperationsAgg, v, accordList)
                    Case IFM.VR.Validation.ObjectValidation.CommLines.LOB.CGL.GeneralInformationValidator.programTypeId
                        If SubQuoteFirst.LobType <> QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then  ' Program Type does not apply to CPP
                            Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddProgramType, v, accordList)
                        End If
                End Select
            Next
        End If

        Me.ValidationHelper.GroupName = "Policy Coverages"

        If chkAdditionalInsured.Checked Then
            If Not (chkAI_ControllingInterests.Checked OrElse chkAI_CoOwnerOfInsuredPremises.Checked OrElse chkAI_EngineersArchitectsOrSurveyors.Checked OrElse chkAI_MortgageeAssigneeOrReceiver.Checked OrElse chkAI_OwnerOrOtherInterestsFromWhomLandHasBeenLeased.Checked) _
            And ddlNumberOfAddlInsureds.SelectedValue = "0" Then
                Me.ValidationHelper.AddError(ddlNumberOfAddlInsureds, "Missing Number of Additional Insureds", accordList)
            End If
        End If

        If QuoteISCPP Then
            ' CPP - Condo D&O
            If chkCondoDAndO.Checked Then
                ' (no fields to validate)
            End If
        End If

        If chkEmployee.Checked Then
            If txtEmployeeNumberOfEmployees.Text.Trim = String.Empty Then
                Me.ValidationHelper.AddError(txtEmployeeNumberOfEmployees, "Missing Number of Employees", accordList)
            Else
                If (Not IsNumeric(txtEmployeeNumberOfEmployees.Text)) OrElse CInt(txtEmployeeNumberOfEmployees.Text) <= 0 Then
                    Me.ValidationHelper.AddError(txtEmployeeNumberOfEmployees, "Invalid Number of Employees", accordList)
                End If
            End If
        End If

        ' INDIANA/OHIO Liquor Liability
        If chkLiquor_IN.Checked Then
            If Not chkManufacturerLiquorSales_IN.Checked AndAlso Not chkRestaurantLiquorSales_IN.Checked AndAlso
                Not chkPackageStoreLiquorSales_IN.Checked AndAlso Not chkClubLiquorSales_IN.Checked Then
                Me.ValidationHelper.AddError("Must add at least one Liquor Liability Classification")
            Else
                If chkManufacturerLiquorSales_IN.Checked Then
                    If txtManufacturerLiquorSales_IN.Text.Trim = String.Empty Then
                        Me.ValidationHelper.AddError(txtManufacturerLiquorSales_IN, "Missing Manufacturer, WholeSalers & Distributors Liquor Sales", accordList)
                    Else
                        If Not IsNumeric(txtManufacturerLiquorSales_IN.Text) OrElse CInt(txtManufacturerLiquorSales_IN.Text) <= 0 Then
                            Me.ValidationHelper.AddError(txtManufacturerLiquorSales_IN, "Invalid Manufacturer, WholeSalers & Distributors Liquor Sales", accordList)
                        End If
                    End If
                End If

                If chkRestaurantLiquorSales_IN.Checked Then
                    If txtRestaurantLiquorSales_IN.Text.Trim = String.Empty Then
                        Me.ValidationHelper.AddError(txtRestaurantLiquorSales_IN, "Missing Restaurants Or Hotels Liquor Sales", accordList)
                    Else
                        If Not IsNumeric(txtRestaurantLiquorSales_IN.Text) OrElse CInt(txtRestaurantLiquorSales_IN.Text) <= 0 Then
                            Me.ValidationHelper.AddError(txtRestaurantLiquorSales_IN, "Invalid Restaurants Or Hotels Liquor Sales", accordList)
                        End If
                    End If
                End If

                If chkPackageStoreLiquorSales_IN.Checked Then
                    If txtPackageStoreLiquorSales_IN.Text.Trim = String.Empty Then
                        Me.ValidationHelper.AddError(txtPackageStoreLiquorSales_IN, "Missing Package Stores Liquor Sales", accordList)
                    Else
                        If Not IsNumeric(txtPackageStoreLiquorSales_IN.Text) OrElse CInt(txtPackageStoreLiquorSales_IN.Text) <= 0 Then
                            Me.ValidationHelper.AddError(txtPackageStoreLiquorSales_IN, "Invalid Package Stores Liquor Sales", accordList)
                        End If
                    End If
                End If

                If chkClubLiquorSales_IN.Checked Then
                    If txtClubLiquorSales_IN.Text.Trim = String.Empty Then
                        Me.ValidationHelper.AddError(txtClubLiquorSales_IN, "Missing Clubs Liquor Sales", accordList)
                    Else
                        If Not IsNumeric(txtClubLiquorSales_IN.Text) OrElse CInt(txtClubLiquorSales_IN.Text) <= 0 Then
                            Me.ValidationHelper.AddError(txtClubLiquorSales_IN, "Invalid Clubs Liquor Sales", accordList)
                        End If
                    End If
                End If
            End If
        End If

        ' ILLINOIS Liquor Liability
        If chkLiquor_IL.Checked Then
            If Not chkManufacturerLiquorSales_IL.Checked AndAlso Not chkRestaurantLiquorSales_IL.Checked AndAlso
                Not chkPackageStoreLiquorSales_IL.Checked AndAlso Not chkClubLiquorSales_IL.Checked Then
                Me.ValidationHelper.AddError("Must add at least one Liquor Liability Classification")
            Else
                If chkManufacturerLiquorSales_IL.Checked Then
                    If txtManufacturerLiquorSales_IL.Text.Trim = String.Empty Then
                        Me.ValidationHelper.AddError(txtManufacturerLiquorSales_IL, "Missing Manufacturer, WholeSalers & Distributors Liquor Sales", accordList)
                    Else
                        If Not IsNumeric(txtManufacturerLiquorSales_IL.Text) OrElse CInt(txtManufacturerLiquorSales_IL.Text) <= 0 Then
                            Me.ValidationHelper.AddError(txtManufacturerLiquorSales_IL, "Invalid Manufacturer, WholeSalers & Distributors Liquor Sales", accordList)
                        End If
                    End If
                End If

                If chkRestaurantLiquorSales_IL.Checked Then
                    If txtRestaurantLiquorSales_IL.Text.Trim = String.Empty Then
                        Me.ValidationHelper.AddError(txtRestaurantLiquorSales_IL, "Missing Restaurants Or Hotels Liquor Sales", accordList)
                    Else
                        If Not IsNumeric(txtRestaurantLiquorSales_IL.Text) OrElse CInt(txtRestaurantLiquorSales_IL.Text) <= 0 Then
                            Me.ValidationHelper.AddError(txtRestaurantLiquorSales_IL, "Invalid Restaurants Or Hotels Liquor Sales", accordList)
                        End If
                    End If
                End If

                If chkPackageStoreLiquorSales_IL.Checked Then
                    If txtPackageStoreLiquorSales_IL.Text.Trim = String.Empty Then
                        Me.ValidationHelper.AddError(txtPackageStoreLiquorSales_IL, "Missing Package Stores Liquor Sales", accordList)
                    Else
                        If Not IsNumeric(txtPackageStoreLiquorSales_IL.Text) OrElse CInt(txtPackageStoreLiquorSales_IL.Text) <= 0 Then
                            Me.ValidationHelper.AddError(txtPackageStoreLiquorSales_IL, "Invalid Package Stores Liquor Sales", accordList)
                        End If
                    End If
                End If

                If chkClubLiquorSales_IL.Checked Then
                    If txtClubLiquorSales_IL.Text.Trim = String.Empty Then
                        Me.ValidationHelper.AddError(txtClubLiquorSales_IL, "Missing Clubs Liquor Sales", accordList)
                    Else
                        If Not IsNumeric(txtClubLiquorSales_IL.Text) OrElse CInt(txtClubLiquorSales_IL.Text) <= 0 Then
                            Me.ValidationHelper.AddError(txtClubLiquorSales_IL, "Invalid Clubs Liquor Sales", accordList)
                        End If
                    End If
                End If
            End If
        End If

        ' Stop Gap
        If chkStopGap.Checked Then
            If txtStopGapPayroll.Text.Trim = "" Then
                Me.ValidationHelper.AddError(txtStopGapPayroll, "Missing Payroll Amount", accordList)
            Else
                If Not txtStopGapPayroll.Text.IsNumeric OrElse CInt(txtStopGapPayroll.Text) <= 0 Then
                    Me.ValidationHelper.AddError(txtStopGapPayroll, "Invalid Payroll Amount", accordList)
                End If
            End If
        End If

        Exit Sub
    End Sub

    Protected Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles btnSaveCoverages.Click, btnSaveDeductible.Click, btnSaveGenInfo.Click
        Me.Save_FireSaveEvent()
        Populate()
    End Sub

    ''' <summary>
    ''' Saves the liquor liability sales and classification ID based on the quote state and selected check boxes.
    ''' </summary>
    ''' <param name="quote"></param>
    Private Sub SaveLiquorLiabilitySales(quote As QuickQuoteObject)
        Dim liquorSales = String.Empty
        Dim classificationId As String = String.Empty

        If quote.State = "IL" Then
            If chkManufacturerLiquorSales_IL.Checked Then
                quote.LiquorManufacturersSales = txtManufacturerLiquorSales_IL.Text.Trim()
                liquorSales = quote.LiquorManufacturersSales
                classificationId = "50911"
            Else
                quote.LiquorManufacturersSales = String.Empty
                txtManufacturerLiquorSales_IL.Text = String.Empty
            End If
            If chkRestaurantLiquorSales_IL.Checked Then
                quote.LiquorRestaurantsSales = txtRestaurantLiquorSales_IL.Text.Trim()
                liquorSales = quote.LiquorRestaurantsSales
                classificationId = "58161"
            Else
                quote.LiquorRestaurantsSales = String.Empty
                txtRestaurantLiquorSales_IL.Text = String.Empty
            End If
            If chkPackageStoreLiquorSales_IL.Checked Then
                quote.LiquorPackageStoresSales = txtPackageStoreLiquorSales_IL.Text.Trim()
                liquorSales = quote.LiquorPackageStoresSales
                classificationId = "59211"
            Else
                quote.LiquorPackageStoresSales = String.Empty
                txtPackageStoreLiquorSales_IL.Text = String.Empty
            End If
            If chkClubLiquorSales_IL.Checked Then
                quote.LiquorClubsSales = txtClubLiquorSales_IL.Text.Trim()
                liquorSales = quote.LiquorClubsSales
                classificationId = "70412"
            Else
                quote.LiquorClubsSales = String.Empty
                txtClubLiquorSales_IL.Text = String.Empty
            End If
        Else
            If chkManufacturerLiquorSales_IN.Checked Then
                quote.LiquorManufacturersSales = txtManufacturerLiquorSales_IN.Text.Trim()
                liquorSales = quote.LiquorManufacturersSales
                classificationId = "50911"
            Else
                quote.LiquorManufacturersSales = String.Empty
                txtManufacturerLiquorSales_IN.Text = String.Empty
            End If
            If chkRestaurantLiquorSales_IN.Checked Then
                quote.LiquorRestaurantsSales = txtRestaurantLiquorSales_IN.Text.Trim()
                liquorSales = quote.LiquorRestaurantsSales
                classificationId = "58161"
            Else
                quote.LiquorRestaurantsSales = String.Empty
                txtRestaurantLiquorSales_IN.Text = String.Empty
            End If
            If chkPackageStoreLiquorSales_IN.Checked Then
                quote.LiquorPackageStoresSales = txtPackageStoreLiquorSales_IN.Text.Trim()
                liquorSales = quote.LiquorPackageStoresSales
                classificationId = "59211"
            Else
                quote.LiquorPackageStoresSales = String.Empty
                txtPackageStoreLiquorSales_IN.Text = String.Empty
            End If
            If chkClubLiquorSales_IN.Checked Then
                quote.LiquorClubsSales = txtClubLiquorSales_IN.Text.Trim()
                liquorSales = quote.LiquorClubsSales
                classificationId = "70412"
            Else
                quote.LiquorClubsSales = String.Empty
                txtClubLiquorSales_IN.Text = String.Empty
            End If
        End If

        quote.LiquorSales = liquorSales
        quote.LiquorLiabilityClassificationId = classificationId
    End Sub

    ''' <summary>
    ''' Sets the liquor liability sales text boxes and check boxes based on the quote state and values.
    ''' </summary>
    ''' <param name="quote"></param>
    Private Sub SetLiquorLiabilitySales(quote As QuickQuoteObject)
        If quote.State = "IL" Then

            chkManufacturerLiquorSales_IL.Attributes.Add("style", "display:''")
            chkRestaurantLiquorSales_IL.Attributes.Add("style", "display:''")
            chkPackageStoreLiquorSales_IL.Attributes.Add("style", "display:''")
            chkClubLiquorSales_IL.Attributes.Add("style", "display:''")

            If Not chkManufacturerLiquorSales_IL.Checked Then chkManufacturerLiquorSales_IL.Checked = Not quote.LiquorManufacturersSales.ReturnEmptyIfLessThanOrEqualToZero() = String.Empty
            If Not chkRestaurantLiquorSales_IL.Checked Then chkRestaurantLiquorSales_IL.Checked = Not quote.LiquorRestaurantsSales.ReturnEmptyIfLessThanOrEqualToZero() = String.Empty
            If Not chkPackageStoreLiquorSales_IL.Checked Then chkPackageStoreLiquorSales_IL.Checked = Not quote.LiquorPackageStoresSales.ReturnEmptyIfLessThanOrEqualToZero() = String.Empty
            If Not chkClubLiquorSales_IL.Checked Then chkClubLiquorSales_IL.Checked = Not quote.LiquorClubsSales.ReturnEmptyIfLessThanOrEqualToZero() = String.Empty

            If chkManufacturerLiquorSales_IL.Checked Then
                txtManufacturerLiquorSalesTableRow_IL.Attributes.Add("style", "display:''")
                txtManufacturerLiquorSales_IL.Text = quote.LiquorManufacturersSales
            Else
                txtManufacturerLiquorSalesTableRow_IL.Attributes.Add("style", "display:none")
            End If

            If chkRestaurantLiquorSales_IL.Checked Then
                txtRestaurantLiquorSalesTableRow_IL.Attributes.Add("style", "display:''")
                txtRestaurantLiquorSales_IL.Text = quote.LiquorRestaurantsSales
            Else
                txtRestaurantLiquorSalesTableRow_IL.Attributes.Add("style", "display:none")
            End If

            If chkPackageStoreLiquorSales_IL.Checked Then
                txtPackageStoreLiquorSalesTableRow_IL.Attributes.Add("style", "display:''")
                txtPackageStoreLiquorSales_IL.Text = quote.LiquorPackageStoresSales
            Else
                txtPackageStoreLiquorSalesTableRow_IL.Attributes.Add("style", "display:none")
            End If

            If chkClubLiquorSales_IL.Checked Then
                txtClubLiquorSalesTableRow_IL.Attributes.Add("style", "display:''")
                txtClubLiquorSales_IL.Text = quote.LiquorClubsSales
            Else
                txtClubLiquorSalesTableRow_IL.Attributes.Add("style", "display:none")
            End If
        Else
            chkManufacturerLiquorSalesTableRow_IN.Attributes.Add("style", "display:''")
            chkRestaurantLiquorSalesTableRow_IN.Attributes.Add("style", "display:''")
            chkPackageStoreLiquorSalesTableRow_IN.Attributes.Add("style", "display:''")
            chkClubLiquorSalesTableRow_IN.Attributes.Add("style", "display:''")

            If Not chkManufacturerLiquorSales_IN.Checked Then chkManufacturerLiquorSales_IN.Checked = Not quote.LiquorManufacturersSales.ReturnEmptyIfLessThanOrEqualToZero() = String.Empty
            If Not chkRestaurantLiquorSales_IN.Checked Then chkRestaurantLiquorSales_IN.Checked = Not quote.LiquorRestaurantsSales.ReturnEmptyIfLessThanOrEqualToZero() = String.Empty
            If Not chkPackageStoreLiquorSales_IN.Checked Then chkPackageStoreLiquorSales_IN.Checked = Not quote.LiquorPackageStoresSales.ReturnEmptyIfLessThanOrEqualToZero() = String.Empty
            If Not chkClubLiquorSales_IN.Checked Then chkClubLiquorSales_IN.Checked = Not quote.LiquorClubsSales.ReturnEmptyIfLessThanOrEqualToZero() = String.Empty

            If chkManufacturerLiquorSales_IN.Checked Then
                txtManufacturerLiquorSalesTableRow_IN.Attributes.Add("style", "display:''")
                txtManufacturerLiquorSales_IN.Text = quote.LiquorManufacturersSales
            Else
                txtManufacturerLiquorSalesTableRow_IN.Attributes.Add("style", "display:none")
            End If

            If chkRestaurantLiquorSales_IN.Checked Then
                txtRestaurantLiquorSalesTableRow_IN.Attributes.Add("style", "display:''")
                txtRestaurantLiquorSales_IN.Text = quote.LiquorRestaurantsSales
            Else
                txtRestaurantLiquorSalesTableRow_IN.Attributes.Add("style", "display:none")
            End If

            If chkPackageStoreLiquorSales_IN.Checked Then
                txtPackageStoreLiquorSalesTableRow_IN.Attributes.Add("style", "display:''")
                txtPackageStoreLiquorSales_IN.Text = quote.LiquorPackageStoresSales
            Else
                txtPackageStoreLiquorSalesTableRow_IN.Attributes.Add("style", "display:none")
            End If

            If chkClubLiquorSales_IN.Checked Then
                txtClubLiquorSalesTableRow_IN.Attributes.Add("style", "display:''")
                txtClubLiquorSales_IN.Text = quote.LiquorClubsSales
            Else
                txtClubLiquorSalesTableRow_IN.Attributes.Add("style", "display:none")
            End If
        End If
    End Sub
End Class