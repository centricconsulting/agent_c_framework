Imports IFM.PrimativeExtensions

Public Class ctl_CGL_GeneralInformation
    Inherits VRControlBase

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.MainAccordionDivId, Me.hdnAccord, "0")
        Me.VRScript.CreateAccordion(Me.divDeductibles.ClientID, Me.hdnDeductibleAccord, "0")

        Me.VRScript.CreateJSBinding(Me.ddOccuranceLibLimit, ctlPageStartupScript.JsEventType.onchange, "Cgl.OccurenceLiabilityLimitChanged('" & ddOccuranceLibLimit.ClientID & "','" & ddGeneralAgg.ClientID & "','" & ddOperationsAgg.ClientID & "','" & ddPersonalInjury.ClientID & "');")
        Me.VRScript.CreateJSBinding(Me.chkGLEnhancement, ctlPageStartupScript.JsEventType.onchange, "Cgl.EnhancementCheckboxChanged('" & chkGLEnhancement.ClientID & "','" & trAddGLBlanketWaiverOfSubroRow.ClientID & "','" & txtRented.ClientID & "','" & txtMedicalExpense.ClientID & "');")
        Me.VRScript.CreateJSBinding(Me.ddlAddlBlanketOfSubroOptions, ctlPageStartupScript.JsEventType.onchange, "Cgl.BlanketWaiverOfSubroChanged('" & ddlAddlBlanketOfSubroOptions.ClientID & "','" & trEnhancementMessageRow.ClientID & "');")
        Me.VRScript.CreateJSBinding(Me.chkAddaGeneralLiabilityDeductible, ctlPageStartupScript.JsEventType.onchange, "Cgl.GLLiabilityDeductibleChanged('" & chkAddaGeneralLiabilityDeductible.ClientID & "','" & divDeductibles.ClientID & "');")

        Me.VRScript.StopEventPropagation(Me.lnkBtnSave.ClientID)
        Me.VRScript.StopEventPropagation(Me.btnSaveDeductible.ClientID)

        ' Used by the ctl_CGL_Coverages so that the Employee and Liquor Occurrence Limits mirror this one in the General Information
        Me.VRScript.AddVariableLine("var genInfoDdOccurenceLimitId = '{0}';".FormatIFM(Me.ddOccuranceLibLimit.ClientID))

    End Sub

    Public Overrides Sub LoadStaticData()
        If ddProgramType.Items Is Nothing OrElse ddProgramType.Items.Count <= 0 Then
            ' Program
            QQHelper.LoadStaticDataOptionsDropDown(ddProgramType, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.ProgramTypeId, , Quote.LobType)
            'Occurrence Liability Limit
            QQHelper.LoadStaticDataOptionsDropDown(ddOccuranceLibLimit, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.OccurrenceLiabilityLimitId, , Quote.LobType)
            ' General Aggregate
            QQHelper.LoadStaticDataOptionsDropDown(ddGeneralAgg, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.GeneralAggregateLimitId, , Quote.LobType)
            ' Product/Completed Ops Aggregate
            QQHelper.LoadStaticDataOptionsDropDown(ddOperationsAgg, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.ProductsCompletedOperationsAggregateLimitId, , Quote.LobType)
            ' Personal and Advertising Injury
            QQHelper.LoadStaticDataOptionsDropDown(ddPersonalInjury, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.PersonalAndAdvertisingInjuryLimitId, , Quote.LobType)
            ' Deductible Type
            QQHelper.LoadStaticDataOptionsDropDown(ddType, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.GL_PremisesAndProducts_DeductibleCategoryTypeId, , Quote.LobType)
            ' Deductible Amount
            QQHelper.LoadStaticDataOptionsDropDown(ddAmount, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.GL_PremisesAndProducts_DeductibleId, , Quote.LobType)
            ' Deductible Per Type (basis)
            QQHelper.LoadStaticDataOptionsDropDown(ddBasis, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuotePropertyName.GL_PremisesAndProducts_DeductiblePerTypeId, , Quote.LobType)
        End If
    End Sub

    Public Overrides Sub Populate()
        LoadStaticData()

        ' SET THE DEFAULTS
        ' Program: CGL Standard
        ddProgramType.SelectedIndex = 0
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
        Me.txtMedicalExpense.Text = "5,000"
        ' Deductible Type: Property Damage
        Me.ddType.SelectedValue = "6"
        ' Deductible Amount: 500
        Me.ddAmount.SelectedValue = "8"
        ' Deductible Basis Type: Per Occurrence
        Me.ddBasis.SelectedValue = "1"

        ' POPULATE
        If Me.Quote.IsNotNull Then
            Me.ddProgramType.SetFromValue(Me.Quote.ProgramTypeId)
            Me.ddGeneralAgg.SetFromValue(Me.Quote.GeneralAggregateLimitId, "65")
            Me.ddOperationsAgg.SetFromValue(Me.Quote.ProductsCompletedOperationsAggregateLimitId, "65")
            Me.ddPersonalInjury.SetFromValue(Me.Quote.PersonalAndAdvertisingInjuryLimitId, "56")
            Me.ddOccuranceLibLimit.SetFromValue(Me.Quote.OccurrenceLiabilityLimitId, "56")

            If Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability Then
                Me.chkGLEnhancement.Checked = Me.Quote.HasBusinessMasterEnhancement
                If chkGLEnhancement.Checked Then
                    trEnhancementMessageRow.Attributes.Add("style", "display:''")
                    trAddGLBlanketWaiverOfSubroRow.Attributes.Add("style", "display:''")
                Else
                    trEnhancementMessageRow.Attributes.Add("style", "display:none")
                    trAddGLBlanketWaiverOfSubroRow.Attributes.Add("style", "display:none")
                End If
            Else
                If Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                    Me.chkGLEnhancement.Checked = Me.Quote.Has_PackageGL_EnhancementEndorsement
                End If
            End If

            Me.ddlAddlBlanketOfSubroOptions.SetFromValue(Me.Quote.BlanketWaiverOfSubrogation) 'Look to Bug 4040 for additional information

            Me.chkAddaGeneralLiabilityDeductible.Checked = String.IsNullOrWhiteSpace(Me.Quote.GL_PremisesAndProducts_DeductibleCategoryTypeId) = False Or String.IsNullOrWhiteSpace(Me.Quote.GL_PremisesAndProducts_DeductibleId) = False Or String.IsNullOrWhiteSpace(Me.Quote.GL_PremisesAndProducts_DeductiblePerTypeId) = False
            If chkAddaGeneralLiabilityDeductible.Checked Then
                divDeductibles.Attributes.Add("style", "display:''")
            Else
                divDeductibles.Attributes.Add("style", "display:none")
            End If

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
            Me.Quote.HasBusinessMasterEnhancement = Me.chkGLEnhancement.Checked
        Else
            If Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialPackage Then
                Me.Quote.Has_PackageGL_EnhancementEndorsement = Me.chkGLEnhancement.Checked
            End If
        End If

        If Me.chkGLEnhancement.Checked Then 'only available with the enhancement - Look to Bug 4040 for additional information
            ' String.Empty = 'None'
            ' 1 = 'Yes' CGL1004
            ' 2 = 'YES With Completed Ops' CGL1002
            Me.ddlAddlBlanketOfSubroOptions.GetFromValue(Me.Quote.BlanketWaiverOfSubrogation)
        Else
            Me.Quote.BlanketWaiverOfSubrogation = String.Empty
        End If

        If Me.chkAddaGeneralLiabilityDeductible.Checked Then
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

    Protected Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkBtnSave.Click, btnSaveDeductible.Click
        Me.Save_FireSaveEvent()
        Populate()
    End Sub
End Class