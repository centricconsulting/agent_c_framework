Imports IFM.PrimativeExtensions

Public Class ctl_CGL_Coverages
    Inherits VRControlBase

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.MainAccordionDivId, Me.hddnAccord, "0")

        Me.VRScript.CreateJSBinding(Me.chkAdditional, ctlPageStartupScript.JsEventType.onchange, "if ($('#" + Me.chkAdditional.ClientID + "').is(':checked')){$('#divAdditional').show();$('#divAdditional input:enabled:visible').first().focus();}else{$('#divAdditional').hide();$('#divAdditional input:enabled').each(function(){$(this).val('');});}", True)
        Me.VRScript.CreateJSBinding(Me.chkEmployee, ctlPageStartupScript.JsEventType.onchange, "if ($('#" + Me.chkEmployee.ClientID + "').is(':checked')){$('#divEmployee').show();$('#divEmployee input:enabled:visible').first().focus();}else{$('#divEmployee').hide();$('#divEmployee input:enabled').each(function(){$(this).val('');});}", True)
        Me.VRScript.CreateJSBinding(Me.chkLiquor, ctlPageStartupScript.JsEventType.onchange, "if ($('#" + Me.chkLiquor.ClientID + "').is(':checked')){$('#divLiquor').show();$('#divLiquor input:enabled:visible').first().focus();}else{$('#divLiquor').hide();$('#divLiquor input:enabled').each(function(){$(this).val('');});}", True)
        Me.VRScript.CreateJSBinding(Me.chkProfessional, ctlPageStartupScript.JsEventType.onchange, "if ($('#" + Me.chkProfessional.ClientID + "').is(':checked')){$('#divProfessional').show();$('#divProfessional input:enabled:visible').first().focus();}else{$('#divProfessional').hide();$('#divProfessional input:enabled').each(function(){$(this).val('');});}", True)

        Me.VRScript.CreateTextBoxFormatter(Me.txtAdditionalInsuredPremiumCharge, ctlPageStartupScript.FormatterType.Currency, ctlPageStartupScript.JsEventType.onkeyup)
        Me.VRScript.CreateTextBoxFormatter(Me.txtAdditionalInsuredPremiumCharge, ctlPageStartupScript.FormatterType.CurrencyNoCents, ctlPageStartupScript.JsEventType.onblur)

        Me.VRScript.CreateTextBoxFormatter(Me.txtEmployeeNumberOfEmployees, ctlPageStartupScript.FormatterType.PositiveNumberWithCommas, ctlPageStartupScript.JsEventType.onkeyup)
        Me.VRScript.CreateTextBoxFormatter(Me.txtEmployeeNumberOfEmployees, ctlPageStartupScript.FormatterType.PositiveWholeNumberWithCommas, ctlPageStartupScript.JsEventType.onblur)

        Me.VRScript.CreateTextBoxFormatter(Me.txtLiquorSales, ctlPageStartupScript.FormatterType.Currency, ctlPageStartupScript.JsEventType.onkeyup)
        Me.VRScript.CreateTextBoxFormatter(Me.txtLiquorSales, ctlPageStartupScript.FormatterType.CurrencyNoCents, ctlPageStartupScript.JsEventType.onblur)

        Me.VRScript.CreateTextBoxFormatter(Me.txtprofessionalBurials, ctlPageStartupScript.FormatterType.PositiveNumberWithCommas, ctlPageStartupScript.JsEventType.onkeyup)
        Me.VRScript.CreateTextBoxFormatter(Me.txtprofessionalBurials, ctlPageStartupScript.FormatterType.PositiveWholeNumberWithCommas, ctlPageStartupScript.JsEventType.onblur)

        Me.VRScript.CreateTextBoxFormatter(Me.txtProfessionalClergy, ctlPageStartupScript.FormatterType.PositiveNumberWithCommas, ctlPageStartupScript.JsEventType.onkeyup)
        Me.VRScript.CreateTextBoxFormatter(Me.txtProfessionalClergy, ctlPageStartupScript.FormatterType.PositiveWholeNumberWithCommas, ctlPageStartupScript.JsEventType.onblur)

        Me.VRScript.CreateTextBoxFormatter(Me.txtProfessionalBodies, ctlPageStartupScript.FormatterType.PositiveNumberWithCommas, ctlPageStartupScript.JsEventType.onkeyup)
        Me.VRScript.CreateTextBoxFormatter(Me.txtProfessionalBodies, ctlPageStartupScript.FormatterType.PositiveWholeNumberWithCommas, ctlPageStartupScript.JsEventType.onblur)

        Me.VRScript.StopEventPropagation(Me.lnkBtnSave.ClientID)

        ' copies the Occurrence LImit set up in the General information to the two coverages Occurrence limits
        Me.VRScript.CreateJSBinding_CustomSelector("'#' + genInfoDdOccurenceLimitId", ctlPageStartupScript.JsEventType.onchange, "$('#" + Me.txtEmployeeOccurrenceLimit.ClientID + "').val($('#' + genInfoDdOccurenceLimitId + ' option:selected').text());")
        Me.VRScript.CreateJSBinding_CustomSelector("'#' + genInfoDdOccurenceLimitId", ctlPageStartupScript.JsEventType.onchange, "$('#" + Me.txtLiquorOccurrenceLimit.ClientID + "').val($('#' + genInfoDdOccurenceLimitId + ' option:selected').text());")


    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        If Me.Quote.IsNotNull Then
            Me.txtAdditionalInsuredPremiumCharge.Text = Me.Quote.AdditionalInsuredsManualCharge.ReturnEmptyIfLessThanOrEqualToZero
            Me.chkAdditional.Checked = Not Me.Quote.AdditionalInsuredsManualCharge.ReturnEmptyIfLessThanOrEqualToZero = String.Empty

            Me.txtEmployeeOccurrenceLimit.Text = Me.Quote.OccurrenceLiabilityLimit 'always mirrors this limit
            Me.txtEmployeeNumberOfEmployees.Text = Me.Quote.EmployeeBenefitsLiabilityText.ReturnEmptyIfLessThanOrEqualToZero
            Me.chkEmployee.Checked = Not Me.Quote.EmployeeBenefitsLiabilityText.ReturnEmptyIfLessThanOrEqualToZero = String.Empty

            Me.chkEPLI.Enabled = Not IFM.VR.Common.Helpers.CGL.EPLIHelper.EPLINotAvailableDoToClassCode(Me.Quote)
            Me.chkEPLI.Checked = If(Me.chkEPLI.Enabled, IFM.VR.Common.Helpers.CGL.EPLIHelper.EPLI_Is_Applied(Me.Quote), False)

            Me.chkHired.Checked = Me.Quote.HasHiredAuto Or Me.Quote.HasNonOwnedAuto

            Me.txtLiquorOccurrenceLimit.Text = Me.Quote.OccurrenceLiabilityLimit 'always mirrors this limit
            Me.ddLiquorClassification.SetFromValue(Me.Quote.LiquorLiabilityClassificationId)
            Me.txtLiquorSales.Text = Me.Quote.LiquorSales.ReturnEmptyIfLessThanOrEqualToZero
            Me.chkLiquor.Checked = Not Me.Quote.LiquorSales.ReturnEmptyIfLessThanOrEqualToZero = String.Empty


            Me.txtprofessionalBurials.Text = Me.Quote.ProfessionalLiabilityCemetaryNumberOfBurials.ReturnEmptyIfLessThanOrEqualToZero
            Me.txtProfessionalBodies.Text = Me.Quote.ProfessionalLiabilityFuneralDirectorsNumberOfBodies.ReturnEmptyIfLessThanOrEqualToZero
            Me.txtProfessionalClergy.Text = Me.Quote.ProfessionalLiabilityPastoralNumberOfClergy.ReturnEmptyIfLessThanOrEqualToZero
            Me.chkProfessional.Checked = Not (Me.Quote.ProfessionalLiabilityCemetaryNumberOfBurials.ReturnEmptyIfLessThanOrEqualToZero = String.Empty And Me.Quote.ProfessionalLiabilityFuneralDirectorsNumberOfBodies.ReturnEmptyIfLessThanOrEqualToZero = String.Empty And Me.Quote.ProfessionalLiabilityPastoralNumberOfClergy.ReturnEmptyIfLessThanOrEqualToZero = String.Empty)
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Me.MainAccordionDivId = divMain.ClientID
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        Me.Quote.AdditionalInsuredsManualCharge = Me.txtAdditionalInsuredPremiumCharge.Text.Trim()
        If Me.txtAdditionalInsuredPremiumCharge.Text.IsNullEmptyorWhitespace Then
            Me.Quote.AdditionalInsuredsCount = 0 ' added by Dons request 10/25/2012
        End If

        Me.Quote.EmployeeBenefitsLiabilityOccurrenceLimit = If(Me.chkEmployee.Checked, Me.txtEmployeeOccurrenceLimit.Text, String.Empty) 'mirrors OccurrenceLiabilityLimit if the coverage is applied
        Me.Quote.EmployeeBenefitsLiabilityText = If(Me.chkEmployee.Checked, Me.txtEmployeeNumberOfEmployees.Text.Trim(), String.Empty)
        Me.Quote.EmployeeBenefitsLiabilityAggregateLimit = If(Me.chkEmployee.Checked, (Me.Quote.EmployeeBenefitsLiabilityOccurrenceLimit.TryToGetDouble * 3).ToString(), String.Empty)
        Me.Quote.EmployeeBenefitsLiabilityDeductible = If(Me.chkEmployee.Checked, "1,000", String.Empty)

        IFM.VR.Common.Helpers.CGL.EPLIHelper.Toggle_EPLI_Is_Applied(Me.Quote, Me.chkEPLI.Checked)
        Me.Quote.HasHiredAuto = Me.chkHired.Checked
        Me.Quote.HasNonOwnedAuto = Me.chkHired.Checked


        Me.Quote.LiquorLiabilityOccurrenceLimit = If(Me.chkLiquor.Checked, Me.txtEmployeeOccurrenceLimit.Text, String.Empty) 'mirrors OccurrenceLiabilityLimit if the coverage is applied
        If Me.chkLiquor.Checked Then
            Me.ddLiquorClassification.getFromValue(Me.Quote.LiquorLiabilityClassificationId)
        Else
            Me.Quote.LiquorLiabilityClassificationId = String.Empty
        End If

        Me.Quote.LiquorSales = Me.txtLiquorSales.Text.Trim()

        Me.Quote.ProfessionalLiabilityCemetaryNumberOfBurials = If(Me.chkProfessional.Checked, Me.txtprofessionalBurials.Text.Trim().ReturnEmptyIfLessThanOrEqualToZero, String.Empty)
        Me.Quote.ProfessionalLiabilityFuneralDirectorsNumberOfBodies = If(Me.chkProfessional.Checked, Me.txtProfessionalBodies.Text.Trim().ReturnEmptyIfLessThanOrEqualToZero, String.Empty)
        Me.Quote.ProfessionalLiabilityPastoralNumberOfClergy = If(Me.chkProfessional.Checked, Me.txtProfessionalClergy.Text.Trim().ReturnEmptyIfLessThanOrEqualToZero, String.Empty)
        Me.Quote.HasFuneralDirectorsProfessionalLiability = False 'don't know why this is here but it was on the old code
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = "Policy Coverages"

        Dim vals = IFM.VR.Validation.ObjectValidation.CommLines.LOB.CGL.CGL_PolicyCoveragesValidator.ValidatePolicyCoverages(Me.Quote, valArgs.ValidationType, Me.chkAdditional.Checked, Me.chkProfessional.Checked)
        If vals.Any() Then
            Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
            For Each v In vals
                Select Case v.FieldId
                    Case IFM.VR.Validation.ObjectValidation.CommLines.LOB.CGL.CGL_PolicyCoveragesValidator.additionalInsured_PremiumCharge
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtAdditionalInsuredPremiumCharge, v, accordList)
                    Case IFM.VR.Validation.ObjectValidation.CommLines.LOB.CGL.CGL_PolicyCoveragesValidator.employeeOccurrenceLimit
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtEmployeeOccurrenceLimit, v, accordList)
                    Case IFM.VR.Validation.ObjectValidation.CommLines.LOB.CGL.CGL_PolicyCoveragesValidator.employeeNumberOfEmployees
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtEmployeeNumberOfEmployees, v, accordList)
                    Case IFM.VR.Validation.ObjectValidation.CommLines.LOB.CGL.CGL_PolicyCoveragesValidator.liquorOccurrenceLimit
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtLiquorOccurrenceLimit, v, accordList)
                    Case IFM.VR.Validation.ObjectValidation.CommLines.LOB.CGL.CGL_PolicyCoveragesValidator.liquorClassification
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddLiquorClassification, v, accordList)
                    Case IFM.VR.Validation.ObjectValidation.CommLines.LOB.CGL.CGL_PolicyCoveragesValidator.liquorSales
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtLiquorSales, v, accordList)
                    Case IFM.VR.Validation.ObjectValidation.CommLines.LOB.CGL.CGL_PolicyCoveragesValidator.professionalBurial
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtprofessionalBurials, v, accordList)
                    Case IFM.VR.Validation.ObjectValidation.CommLines.LOB.CGL.CGL_PolicyCoveragesValidator.professinalBodies
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtProfessionalBodies, v, accordList)
                    Case IFM.VR.Validation.ObjectValidation.CommLines.LOB.CGL.CGL_PolicyCoveragesValidator.professionalClergy
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtProfessionalClergy, v, accordList)
                    Case IFM.VR.Validation.ObjectValidation.CommLines.LOB.CGL.CGL_PolicyCoveragesValidator.professionalAll
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtprofessionalBurials, v, accordList)
                    Case IFM.VR.Validation.ObjectValidation.CommLines.LOB.CGL.CGL_PolicyCoveragesValidator.HiredAutoNonOwnedMismatch
                        Me.ValidationHelper.AddWarning("Hired Auto and Non Owner do not match. Contact IT.") 'warning because they wont that the ability to fix it by themselves

                End Select
            Next

        End If

    End Sub

    Protected Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkBtnSave.Click
        Me.Save_FireSaveEvent()
    End Sub
End Class