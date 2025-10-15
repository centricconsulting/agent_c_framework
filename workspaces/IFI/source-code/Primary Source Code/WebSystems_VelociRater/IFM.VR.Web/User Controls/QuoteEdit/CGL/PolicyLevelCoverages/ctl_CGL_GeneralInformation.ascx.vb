Imports IFM.PrimativeExtensions

Public Class ctl_CGL_GeneralInformation
    Inherits VRControlBase

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.MainAccordionDivId, Me.hddAccord, "0")

        Me.VRScript.CreateJSBinding(Me.chkGenLibEnhancement, ctlPageStartupScript.JsEventType.onchange, "if ($('#" + Me.chkGenLibEnhancement.ClientID + "').is(':checked')){$('#divSubrogation').show();}else{$('#divSubrogation').hide();$('#" + Me.ddlAddlBlanketOfSubroOptions.ClientID + "').val('');$('#" + Me.ddlAddlBlanketOfSubroOptions.ClientID + "').change();}", True)
        Me.VRScript.CreateJSBinding(Me.ddlAddlBlanketOfSubroOptions, ctlPageStartupScript.JsEventType.onchange, "if (!($('#" + Me.ddlAddlBlanketOfSubroOptions.ClientID + "').val() == '')){$('#divSubrogationInfo').show();}else{$('#divSubrogationInfo').hide();}", True)
        Me.VRScript.CreateJSBinding(Me.chkDeductible, ctlPageStartupScript.JsEventType.onchange, "if ($('#" + Me.chkDeductible.ClientID + "').is(':checked')){$('#divDeductible').show();}else{$('#divDeductible').hide();}", True)

        Me.VRScript.CreateJSBinding(Me.chkGenLibEnhancement, ctlPageStartupScript.JsEventType.onchange, "if ($('#" + Me.chkGenLibEnhancement.ClientID + "').is(':checked')){$('#" + Me.txtRented.ClientID + "').val('300,000');$('#" + Me.txtMedicalExpense.ClientID + "').val('10,000');}else{$('#" + Me.txtRented.ClientID + "').val('100,000');$('#" + Me.txtMedicalExpense.ClientID + "').val('5,000');}", True)
        Me.VRScript.StopEventPropagation(Me.lnkBtnSave.ClientID)

        ' Used by the ctl_CGL_Coverages so that the Employee and Liquor Occurrence Limits mirror this one in the General Information
        Me.VRScript.AddVariableLine("var genInfoDdOccurenceLimitId = '{0}';".FormatIFM(Me.ddOccuranceLibLimit.ClientID))

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        If Me.Quote.IsNotNull Then
            Me.ddProgramType.SetFromValue(Me.Quote.ProgramTypeId)
            Me.ddGeneralAgg.SetFromValue(Me.Quote.GeneralAggregateLimitId, "8")
            Me.ddOperationsAgg.SetFromValue(Me.Quote.ProductsCompletedOperationsAggregateLimitId, "9")
            Me.ddPersonalInjury.SetFromValue(Me.Quote.PersonalAndAdvertisingInjuryLimitId, "7")
            Me.ddOccuranceLibLimit.SetFromValue(Me.Quote.OccurrenceLiabilityLimitId, "6")

            'Me.txtRented.Text = "" set via javascript
            'Me.txtMedicalExpense.Text = "" ' set via javascript

            If Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability Then
                Me.chkGenLibEnhancement.Checked = Me.Quote.HasBusinessMasterEnhancement
            Else
                If Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                    Me.chkGenLibEnhancement.Checked = Me.Quote.Has_PackageGL_EnhancementEndorsement
                End If
            End If

            Me.ddlAddlBlanketOfSubroOptions.SetFromValue(Me.Quote.BlanketWaiverOfSubrogation) 'Look to Bug 4040 for additional information

            Me.chkDeductible.Checked = String.IsNullOrWhiteSpace(Me.Quote.GL_PremisesAndProducts_DeductibleCategoryTypeId) = False Or String.IsNullOrWhiteSpace(Me.Quote.GL_PremisesAndProducts_DeductibleId) = False Or String.IsNullOrWhiteSpace(Me.Quote.GL_PremisesAndProducts_DeductiblePerTypeId) = False
            Me.ddType.SetFromValue(Me.Quote.GL_PremisesAndProducts_DeductibleCategoryTypeId, "6")
            Me.ddAmount.SetFromValue(Me.Quote.GL_PremisesAndProducts_DeductibleId, "8")
            Me.ddBasis.SetFromValue(Me.Quote.GL_PremisesAndProducts_DeductiblePerTypeId, "1")

        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsPostBack = False Then
            Me.MainAccordionDivId = Me.divMain.ClientID
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        Me.ddProgramType.getFromValue(Me.Quote.ProgramTypeId)
        Me.ddGeneralAgg.getFromValue(Me.Quote.GeneralAggregateLimitId)
        Me.ddOperationsAgg.getFromValue(Me.Quote.ProductsCompletedOperationsAggregateLimitId)

        ' if productCompleted is excluded you need all the classcodes Subline336_ExcludeProductsCompletedOperations  to be true, else false
        IFM.VR.Common.Helpers.CGL.ClassCodeHelper.ConvertAllClassCodeSubline336ExcludedTo(Me.Quote)

        Me.ddPersonalInjury.getFromValue(Me.Quote.PersonalAndAdvertisingInjuryLimitId)
        Me.ddOccuranceLibLimit.GetFromValue(Me.Quote.OccurrenceLiabilityLimitId)

        Me.Quote.DamageToPremisesRentedLimit = "100,000" ' must always go through as 100,000 - Diamond will adjust if needed - what you see on the screen is informational only
        Me.Quote.MedicalExpensesLimit = "5,000" ' must alway go through as 5,000

        If Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability Then
            Me.Quote.HasBusinessMasterEnhancement = Me.chkGenLibEnhancement.Checked
        Else
            If Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                Me.Quote.Has_PackageGL_EnhancementEndorsement = Me.chkGenLibEnhancement.Checked
            End If
        End If

        If Me.chkGenLibEnhancement.Checked Then 'only available with the enhancement - Look to Bug 4040 for additional information
            ' String.Empty = 'None'
            ' 1 = 'Yes' CGL1004
            ' 2 = 'YES With Completed Ops' CGL1002
            Me.ddlAddlBlanketOfSubroOptions.GetFromValue(Me.Quote.BlanketWaiverOfSubrogation)
        Else
            Me.Quote.BlanketWaiverOfSubrogation = String.Empty
        End If


        If Me.chkDeductible.Checked Then
            Me.ddType.getFromValue(Me.Quote.GL_PremisesAndProducts_DeductibleCategoryTypeId)
            Me.ddAmount.getFromValue(Me.Quote.GL_PremisesAndProducts_DeductibleId)
            Me.ddBasis.getFromValue(Me.Quote.GL_PremisesAndProducts_DeductiblePerTypeId)
        Else
            Me.Quote.GL_PremisesAndProducts_DeductibleCategoryTypeId = String.Empty
            Me.Quote.GL_PremisesAndProducts_DeductibleId = String.Empty
            Me.Quote.GL_PremisesAndProducts_DeductiblePerTypeId = String.Empty
        End If

        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = "General Information"

        Dim vals = IFM.VR.Validation.ObjectValidation.CommLines.LOB.CGL.GeneralInformationValidator.ValidateGeneralInformation(Me.Quote, valArgs.ValidationType)
        If vals.Any() Then
            Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
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
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtMedicalExpense, v, accordList)
                    Case IFM.VR.Validation.ObjectValidation.CommLines.LOB.CGL.GeneralInformationValidator.occurrenceLibLimit
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddOccuranceLibLimit, v, accordList)
                    Case IFM.VR.Validation.ObjectValidation.CommLines.LOB.CGL.GeneralInformationValidator.ProductOperationsAg
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddOperationsAgg, v, accordList)
                    Case IFM.VR.Validation.ObjectValidation.CommLines.LOB.CGL.GeneralInformationValidator.programTypeId
                        Me.ValidationHelper.Val_BindValidationItemToControl(Me.ddProgramType, v, accordList)

                End Select
            Next

        End If

    End Sub

    Protected Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkBtnSave.Click
        Me.Save_FireSaveEvent()
    End Sub
End Class